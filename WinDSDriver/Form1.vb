Option Explicit On
Option Strict On

Public Class Form1
    Private Const READY As Char = "r"c
    Private Const IN_WORK As Char = "w"c
    Private Const ANSWER As Char = "o"c
    Private Const FIRE As Char = "f"c
    Private Const SCAN_TIMEOUT As Integer = 100 * 2
    Private Const CHECK_DEVICE_TIMEOUT As Integer = 1000 * 2
    Private mHasConnection As Boolean = False
    Private mWorkingThread As Threading.Thread
    Private mWatching As Boolean = True

    Private Const VK_LCONTROL As Byte = &HA2
    Private Const VK_LWIN As Byte = &H5B
    Private Const VK_RIGHT As Byte = &H27
    Private Const KEYEVENTF_KEYUP As Byte = &H2

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'icon_NI.ContextMenu
        Dim mi As New MenuItem("Выход", Sub() Application.Exit())
        Dim cm As New ContextMenu({mi})
        icon_NI.ContextMenu = cm
    End Sub

    Private Sub Bt_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If mHasConnection Then
            If mWorkingThread IsNot Nothing Then mWorkingThread.Abort()
        Else
            Dim buff(0) As Char
            devPort.ReadTimeout = SCAN_TIMEOUT
            For Each sp As String In My.Computer.Ports.SerialPortNames
                devPort.PortName = sp
                Try
                    devPort.Open()
                    devPort.Read(buff, 0, 1)
                Catch ex As Exception
                    buff(0) = vbNullChar.Chars(0)
                End Try
                If buff(0) = READY Then
                    mHasConnection = True
                    Exit For
                End If
                devPort.Close()
            Next
            If Not mHasConnection Then
                log("Устройство не найдено.")
                Return
            End If
            log("Соединение установлено (" & devPort.PortName & ")")
            devPort.Write(ANSWER)
            Threading.Thread.Sleep(CHECK_DEVICE_TIMEOUT)
            'Читаем порт, проверяем, что устройство подключено.
            devPort.ReadTimeout = CHECK_DEVICE_TIMEOUT
            mWorkingThread = New Threading.Thread(New Threading.ThreadStart(AddressOf working))
            mWorkingThread.IsBackground = True
            mWorkingThread.Start()
        End If
    End Sub

    Private Sub switchDesktop()
        keybd_event(VK_LCONTROL, 0, 0, 0)
        keybd_event(VK_LWIN, 0, 0, 0)
        keybd_event(VK_RIGHT, 0, 0, 0)
        keybd_event(VK_RIGHT, 0, KEYEVENTF_KEYUP, 0)
        keybd_event(VK_LWIN, 0, KEYEVENTF_KEYUP, 0)
        keybd_event(VK_LCONTROL, 0, KEYEVENTF_KEYUP, 0)
    End Sub

    Private Sub onFire()
        log("Fire!")
        If mWatching Then
            switchDesktop()
            mWatching = False
        End If
    End Sub

    Private Delegate Sub SafeLog(m As String)

    Private Sub log(m As String)
        icon_NI.ShowBalloonTip(1000, "", m, ToolTipIcon.Info)

        If console_RTB.InvokeRequired Then
            console_RTB.Invoke(New SafeLog(AddressOf log), {m})
        Else
            console_RTB.AppendText(m & vbCrLf)
        End If
    End Sub

    Private Sub working()
        Dim buff(0) As Char
        Do
            Try
                devPort.Read(buff, 0, 1)
            Catch ex As Exception
                log("Устройство перестало отвечать.")
                devPort.Close()
                mHasConnection = False
                Return
            End Try
            If Not buff(0) = IN_WORK Then
                If buff(0) = FIRE Then
                    onFire()
                Else
                    log("От устройства пришли неожиданные данные: " & buff(0))
                    devPort.Close()
                    mHasConnection = False
                    Return
                End If
            End If
        Loop
    End Sub

    Declare Auto Sub keybd_event Lib "user32" (ByVal bVk As Byte, ByVal bScan As Byte, ByVal dwFlags As Integer, ByVal dwExtraInfo As Integer)

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        mWatching = True
    End Sub
End Class

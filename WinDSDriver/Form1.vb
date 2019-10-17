Option Explicit On
Option Strict On

Public Class Form1
    Private Const COMMAND_READY As Char = "r"c
    Private Const COMMAND_IN_WORK As Char = "w"c
    Private Const COMMAND_ANSWER As Char = "o"c
    Private Const COMMAND_FIRE As Char = "f"c
    Private Const COMMAND_DISCONNECT As Char = "d"c
    Private Const SCAN_TIMEOUT As Integer = 100 * 2
    Private Const CHECK_DEVICE_TIMEOUT As Integer = 1000 * 2
    Private mHasConnection As Boolean = False
    Private mWorkingThread As Threading.Thread
    Public Property Watching As Boolean = True

    Private Const VK_LCONTROL As Byte = &HA2
    Private Const VK_LWIN As Byte = &H5B
    Private Const VK_RIGHT As Byte = &H27
    Private Const KEYEVENTF_KEYUP As Byte = &H2

    Private Const CONNECT_MENU_TEXT As String = "Соединить"
    Private Const DISCONNECT_MENU_TEXT As String = "Разъединить"
    Private Const EXIT_MENU_TEXT As String = "Выход"

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'Dim menu(1) As MenuItem
        'menu(0) = New MenuItem(EXIT_MENU_TEXT, Sub() Application.Exit())
        'menu(1) = New MenuItem(CONNECT_MENU_TEXT, Sub()
        '                                              If mHasConnection Then
        '                                                  disconnect()
        '                                              Else
        '                                                  connect()
        '                                              End If
        '                                          End Sub)
        'Dim cm As New ContextMenu(menu)

        Dim cms = New ContextMenuStrip()
        Dim tsMenu(1) As ToolStripMenuItem
        tsMenu(0) = New ToolStripMenuItem(EXIT_MENU_TEXT)
        AddHandler tsMenu(0).Click, Sub() Application.Exit()
        tsMenu(1) = New ToolStripMenuItem(CONNECT_MENU_TEXT)
        AddHandler tsMenu(1).Click, Sub()
                                        If mHasConnection Then
                                            disconnect()
                                        Else
                                            connect()
                                        End If
                                    End Sub
        cms.Items.AddRange(tsMenu)
        icon_NI.ContextMenuStrip = cms
    End Sub

    Private Delegate Sub SafechangeConnectMenuText()

    Private Sub changeConnectMenuText()
        If icon_NI.ContextMenuStrip.InvokeRequired Then
            icon_NI.ContextMenuStrip.Invoke(New SafechangeConnectMenuText(AddressOf changeConnectMenuText))
        End If
        If mHasConnection Then
            icon_NI.ContextMenuStrip.Items.Item(1).Text = DISCONNECT_MENU_TEXT
        Else
            icon_NI.ContextMenuStrip.Items.Item(1).Text = CONNECT_MENU_TEXT
        End If
    End Sub

    Public Sub disconnect()
        If Not mHasConnection Then Return
        If RefreshWatch.IsHandleCreated Then showRefreshWatching(False)
        mHasConnection = False
        mWorkingThread.Join()
        devPort.Write(COMMAND_DISCONNECT)
        devPort.Close()
        log("Соединение сброшено")
        changeConnectMenuText()
    End Sub

    Private Sub connect()
        If mHasConnection Then Return
        Dim buff(0) As Char
        mHasConnection = False
        Watching = True
        devPort.ReadTimeout = SCAN_TIMEOUT
        For Each sp As String In My.Computer.Ports.SerialPortNames
            devPort.PortName = sp
            Try
                devPort.Open()
                devPort.ReadExisting() 'Очищаем буфер com порта от старых данных и мусора, что возможно пришло.
                devPort.Read(buff, 0, 1)
            Catch ex As Exception
                buff(0) = vbNullChar.Chars(0)
            End Try
            If buff(0) = COMMAND_READY Then
                mHasConnection = True
                Exit For
            End If
            devPort.Close()
        Next
        If Not mHasConnection Then
            log("Устройство не найдено.")
            changeConnectMenuText()
            Return
        End If
        log("Соединение установлено (" & devPort.PortName & ")")
        changeConnectMenuText()
        devPort.Write(COMMAND_ANSWER)
        Threading.Thread.Sleep(CHECK_DEVICE_TIMEOUT)
        devPort.ReadTimeout = CHECK_DEVICE_TIMEOUT
        mWorkingThread = New Threading.Thread(New Threading.ThreadStart(AddressOf working))
        mWorkingThread.IsBackground = True
        mWorkingThread.Start()
    End Sub

    Private Sub switchDesktop()
        keybd_event(VK_LCONTROL, 0, 0, 0)
        keybd_event(VK_LWIN, 0, 0, 0)
        keybd_event(VK_RIGHT, 0, 0, 0)
        keybd_event(VK_RIGHT, 0, KEYEVENTF_KEYUP, 0)
        keybd_event(VK_LWIN, 0, KEYEVENTF_KEYUP, 0)
        keybd_event(VK_LCONTROL, 0, KEYEVENTF_KEYUP, 0)
    End Sub

    Private Delegate Sub SafeShowRefreshWatching(isShow As Boolean)

    Private Sub showRefreshWatching(isShow As Boolean)
        If Me.InvokeRequired Then
            Me.Invoke(New SafeShowRefreshWatching(AddressOf showRefreshWatching), {isShow})
        Else
            If isShow Then
                RefreshWatch.Show()
            Else
                RefreshWatch.InternalClose = True
                RefreshWatch.Close()
                RefreshWatch.InternalClose = False
            End If
        End If
    End Sub

    Private Sub onFire()
        If Watching Then
            Watching = False
            showRefreshWatching(True)
            switchDesktop()
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
                changeConnectMenuText()
                Return
            End Try
            If Not buff(0) = COMMAND_IN_WORK Then
                If buff(0) = COMMAND_FIRE Then
                    onFire()
                Else
                    log("От устройства пришли неожиданные данные: " & buff(0))
                    devPort.Write(COMMAND_DISCONNECT) 'на всякий случай посылаем команду перезагрузиться устройству.
                    devPort.Close()
                    mHasConnection = False
                    changeConnectMenuText()
                    Return
                End If
            End If
            If Not mHasConnection Then Exit Do
        Loop
    End Sub

    Private Sub Form1_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed
        'disconnect()
    End Sub

    Declare Auto Sub keybd_event Lib "user32" (ByVal bVk As Byte, ByVal bScan As Byte, ByVal dwFlags As Integer, ByVal dwExtraInfo As Integer)
End Class

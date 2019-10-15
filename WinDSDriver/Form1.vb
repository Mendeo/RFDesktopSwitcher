Public Class Form1
    Private Const READY As Char = "r"c
    Private Const IN_WORK As Char = "w"c
    Private Const ANSWER As Char = "o"c
    Private Const FIRE As Char = "f"c
    Private Const SCAN_TIMEOUT As Integer = 100 * 2
    Private Const CHECK_DEVICE_TIMEOUT As Integer = 1000 * 2
    Private mHasConnection As Boolean = False
    Private mWorkingThread As Threading.Thread

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
                    buff(0) = vbNullChar
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
            mWorkingThread.Start()
        End If
    End Sub

    Private Sub onFire()
        log("Fire!")
    End Sub
    Private Delegate Sub SafeLog(m As String)
    Private Sub log(m As String)
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

    Private Sub Form1_Closed(sender As Object, e As EventArgs) Handles Me.Closed
        mWorkingThread.Abort()
    End Sub
End Class

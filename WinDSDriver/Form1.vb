Public Class Form1
    Private Const READY As Char = "r"c
    Private Const IN_WORK As Char = "w"c
    Private Const ANSWER As Char = "o"c
    Private Const FIRE As Char = "f"c
    Private Const SCAN_TIMEOUT As Integer = 100
    Private Const CHECK_DEVICE_TIMEOUT As Integer = 1000

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim hasConnection As Boolean = False
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
                hasConnection = True
                Exit For
            End If
            devPort.Close()
        Next
        If Not hasConnection Then
            MsgBox("Устройство не найдено.", MsgBoxStyle.Exclamation)
            Return
        End If
        devPort.Write(ANSWER)
        'Читаем порт, проверяем, что устройство подключено.
        devPort.ReadTimeout = CHECK_DEVICE_TIMEOUT
        Do
            Try
                devPort.Read(buff, 0, 1)
            Catch ex As Exception
                MsgBox("Устройство перестало отвечать.", MsgBoxStyle.Critical)
                Return
            End Try
            If Not buff(0) = IN_WORK Then
                If buff(0) = FIRE Then
                    onFire()
                Else
                    MsgBox("От устройства пришли неожиданные данные.", MsgBoxStyle.Critical)
                    Return
                End If
            End If
        Loop
    End Sub
    Private Sub onFire()
        console_RTB.AppendText("Fire!" & vbCrLf)
    End Sub
End Class

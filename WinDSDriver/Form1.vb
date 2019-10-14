Public Class Form1
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        For Each sp As String In My.Computer.Ports.SerialPortNames
            console_RTB.AppendText(sp)
            devPort.PortName = sp
            Dim buff(0) As Char
            Try
                devPort.Open()
                devPort.Read(buff, 0, 1)
            Catch ex As Exception
                buff(0) = vbNullChar
            End Try
            If buff(0) = "r"c Then
                console_RTB.AppendText("+")
            Else
                console_RTB.AppendText("-")
            End If
            console_RTB.AppendText(vbCrLf)
            devPort.Close()
        Next
    End Sub
End Class

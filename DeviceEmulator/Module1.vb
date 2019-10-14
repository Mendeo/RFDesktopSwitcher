Option Explicit On
Option Strict On

Module Module1
    Private mDataReceived As Boolean = False
    Sub Main()
        Dim port As New IO.Ports.SerialPort("COM1", 115200, IO.Ports.Parity.None, 8, IO.Ports.StopBits.One)
        port.ReceivedBytesThreshold = 1
        AddHandler port.DataReceived, Sub() mDataReceived = True
        port.Open()

        Dim buff(0) As Char
        Do
            If mDataReceived Then
                mDataReceived = False
                port.Read(buff, 0, 1)
                If buff(0) = "o"c Then
                    Exit Do
                End If
            End If
            port.Write("r"c)
            Threading.Thread.Sleep(100)
        Loop
        port.Write("w"c)
    End Sub
End Module

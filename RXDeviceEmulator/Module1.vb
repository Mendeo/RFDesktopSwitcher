Option Explicit On
Option Strict On

Module Module1
    Private Const READY As Char = "r"c
    Private Const IN_WORK As Char = "w"c
    Private Const ANSWER As Char = "o"c
    Private Const FIRE As Char = "f"c
    Private Const SCAN_TIMEOUT As Integer = 100
    Private Const CHECK_DEVICE_TIMEOUT As Integer = 1000

    Private mDataReceived As Boolean = False
    Private mPort As New IO.Ports.SerialPort("COM1", 115200, IO.Ports.Parity.None, 8, IO.Ports.StopBits.One)
    Private mLock As New Object()

    Sub Main()

        mPort.ReceivedBytesThreshold = 1
        AddHandler mPort.DataReceived, Sub() mDataReceived = True
        mPort.Open()

        Do
            Console.WriteLine("ready")
            Dim buff(0) As Char
            Do
                If mDataReceived Then
                    mDataReceived = False
                    mPort.Read(buff, 0, 1)
                    If buff(0) = ANSWER Then
                        Exit Do
                    End If
                End If
                mPort.Write(READY)
                Threading.Thread.Sleep(SCAN_TIMEOUT)
            Loop
            Console.WriteLine("working")
            'Запускаем поток working
            Dim tWorking As New Threading.Thread(New Threading.ThreadStart(AddressOf working))
            tWorking.Start()
            Console.WriteLine("Echo:")
            Do
                Dim str As String = Console.ReadLine
                If str.Equals("reboot") Then
                    tWorking.Abort()
                    Exit Do
                End If
                SyncLock mLock
                    mPort.Write(str)
                End SyncLock
            Loop
        Loop
    End Sub
    Private Sub working()
        Do
            SyncLock mLock
                mPort.Write(IN_WORK)
            End SyncLock
            Threading.Thread.Sleep(CHECK_DEVICE_TIMEOUT)
        Loop
    End Sub
End Module

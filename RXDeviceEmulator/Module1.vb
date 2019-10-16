Option Explicit On
Option Strict On

Module Module1
    Private Const COMMAND_READY As Char = "r"c
    Private Const COMMAND_IN_WORK As Char = "w"c
    Private Const COMMAND_ANSWER As Char = "o"c
    Private Const COMMAND_FIRE As Char = "f"c
    Private Const COMMAND_DISCONNECT As Char = "d"c
    Private Const SCAN_TIMEOUT As Integer = 100
    Private Const CHECK_DEVICE_TIMEOUT As Integer = 1000

    Private mDataReceived As Boolean = False
    Private mPort As IO.Ports.SerialPort
    Private mLock As New Object()
    Private Event NeedReboot()
    Private mNeedReboot As Boolean = False
    Dim mTWorking As Threading.Thread, mRebootComListener As Threading.Thread, mConListener As Threading.Thread

    Sub Main()
        Console.Write("Port name: ")
        mPort = New IO.Ports.SerialPort(Console.ReadLine, 115200, IO.Ports.Parity.None, 8, IO.Ports.StopBits.One)
        mPort.ReadTimeout = 100
        mPort.ReceivedBytesThreshold = 1
        AddHandler mPort.DataReceived, Sub() mDataReceived = True
        AddHandler NeedReboot, AddressOf start
        mPort.Open()
        start()
    End Sub

    Private Sub start()
        Console.WriteLine("start")
        If mTWorking IsNot Nothing AndAlso mTWorking.IsAlive Then mTWorking.Join()
        If mRebootComListener IsNot Nothing AndAlso mRebootComListener.IsAlive Then mRebootComListener.Join()
        If mConListener IsNot Nothing AndAlso mConListener.IsAlive Then mConListener.Abort()
        Console.WriteLine("ready")
        Dim buff(0) As Char
        Do
            If mDataReceived Then
                mDataReceived = False
                mPort.Read(buff, 0, 1)
                If buff(0) = COMMAND_ANSWER Then
                    Exit Do
                End If
            End If
            mPort.Write(COMMAND_READY)
            Threading.Thread.Sleep(SCAN_TIMEOUT)
        Loop
        mNeedReboot = False
        mTWorking = New Threading.Thread(New Threading.ThreadStart(AddressOf working))
        mTWorking.Start()
        Console.WriteLine("Echo:")
        mRebootComListener = New Threading.Thread(New Threading.ThreadStart(AddressOf rebootComPortListener))
        mRebootComListener.Start()
        mConListener = New Threading.Thread(New Threading.ThreadStart(AddressOf consoleListener))
        mConListener.Start()
    End Sub

    Private Sub consoleListener()
        Do
            Dim str As String = Console.ReadLine
            If str.Equals("reboot") Then
                mNeedReboot = True
                RaiseEvent NeedReboot()
                Exit Do
            ElseIf Not mNeedReboot Then
                SyncLock mLock
                    mPort.Write(str)
                End SyncLock
            End If
            If mNeedReboot Then Exit Do
        Loop
    End Sub
    Private Sub rebootComPortListener()
        Dim buff(0) As Char
        Do
            Try
                mPort.Read(buff, 0, 1)
            Catch ex As Exception

            End Try
            If buff(0) = COMMAND_DISCONNECT Then
                mNeedReboot = True
                RaiseEvent NeedReboot()
                Exit Do
            End If
            If mNeedReboot Then Exit Do
        Loop
    End Sub
    Private Sub working()
        Do
            SyncLock mLock
                mPort.Write(COMMAND_IN_WORK)
            End SyncLock
            Threading.Thread.Sleep(CHECK_DEVICE_TIMEOUT)
            If mNeedReboot Then Exit Do
        Loop
    End Sub
End Module

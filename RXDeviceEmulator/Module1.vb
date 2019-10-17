﻿Option Explicit On
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
    Private mWorking As Boolean = False
    Private mTWorking As Threading.Thread, mConListener As Threading.Thread, mComListener As Threading.Thread
    Private mEWH As New Threading.EventWaitHandle(False, Threading.EventResetMode.AutoReset)

    Sub Main()
        Console.Write("Port name: ")
        mPort = New IO.Ports.SerialPort(Console.ReadLine, 115200, IO.Ports.Parity.None, 8, IO.Ports.StopBits.One)
        mPort.ReadTimeout = -1
        mPort.ReceivedBytesThreshold = 1
        AddHandler mPort.DataReceived, Sub() mDataReceived = True
        mPort.Open()
        Do
            start()
            mEWH.WaitOne()
        Loop
    End Sub

    Private Sub start()
        mPort.Close()
        Console.WriteLine("start " & Threading.Thread.CurrentThread.ManagedThreadId)
        If mConListener IsNot Nothing AndAlso mConListener.IsAlive Then
            mConListener.Abort()
            Console.WriteLine("wait mConListener")
            'mConListener.Join()
        End If
        If mTWorking IsNot Nothing AndAlso mTWorking.IsAlive Then
            Console.WriteLine("wait mTWorking")
            mTWorking.Join()
        End If
        If mComListener IsNot Nothing AndAlso mComListener.IsAlive Then
            Console.WriteLine("wait mComListener")
            mComListener.Join()
        End If
        Console.WriteLine("ready")
        mPort.Open()
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

        mWorking = True
        mTWorking = New Threading.Thread(New Threading.ThreadStart(AddressOf working))
        mTWorking.IsBackground = True
        mTWorking.Start()
        Console.WriteLine("Echo:")
        mConListener = New Threading.Thread(New Threading.ThreadStart(AddressOf consoleListener))
        mConListener.Start()
        mComListener = New Threading.Thread(New Threading.ThreadStart(AddressOf rebootComPortSetListener))
        mComListener.IsBackground = True
        mComListener.Start()
    End Sub

    Private Sub consoleListener()
        Do
            Dim str As String = Console.ReadLine
            If str.Equals("reboot") Then
                mWorking = False
                mEWH.Set() 'Запускаем start в главном потоке.
                Exit Do
            ElseIf mWorking Then
                'SyncLock mLock
                mPort.Write(str)
                Console.WriteLine("Ok!")
                'End SyncLock
            End If
            If Not mWorking Then Exit Do
        Loop
    End Sub
    Private Sub rebootComPortSetListener()
        AddHandler mPort.DataReceived, AddressOf rebootComPortListener
    End Sub
    Private Sub rebootComPortListener(sender As Object, e As IO.Ports.SerialDataReceivedEventArgs)
        If mWorking Then
            Dim buff(0) As Char
            'SyncLock mLock
            mPort.Read(buff, 0, 1)
            'End SyncLock
            If buff(0) = COMMAND_DISCONNECT Then
                mWorking = False
                mDataReceived = False
                RemoveHandler mPort.DataReceived, AddressOf rebootComPortListener
                mEWH.Set()
            End If
        End If
    End Sub
    Private Sub working()
        Do
            'SyncLock mLock
            mPort.Write(COMMAND_IN_WORK)
            'End SyncLock
            Threading.Thread.Sleep(CHECK_DEVICE_TIMEOUT)
            If Not mWorking Then Exit Do
        Loop
    End Sub
End Module

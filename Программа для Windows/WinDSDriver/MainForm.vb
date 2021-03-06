﻿Option Explicit On
Option Strict On

Public Class MainForm
    Private Const COMMAND_READY As Char = "r"c
    Private Const COMMAND_IN_WORK As Char = "w"c
    Private Const COMMAND_ANSWER As Char = "o"c
    Private Const COMMAND_FIRE As Char = "f"c
    Private Const COMMAND_DISCONNECT As Char = "d"c
    Private Const SCAN_TIMEOUT As Integer = 100 * 2
    Private Const CHECK_DEVICE_TIMEOUT As Integer = 1000 * 2
    Private Const SEND_COMP_ALIVE_TIMEOUT As Integer = 1000
    Private Const COMMAND_COMP_ALIVE As Char = "a"c
    Private mHasConnection As Boolean = False
    Private mWorkingThread As Threading.Thread
    Private mWatching As Boolean = True

    Private Const VK_LCONTROL As Byte = &HA2
    Private Const VK_LWIN As Byte = &H5B
    Private Const VK_RIGHT As Byte = &H27
    Private Const KEYEVENTF_KEYUP As Byte = &H2

    Private Const CONNECT_MENU_TEXT As String = "Соединить"
    Private Const DISCONNECT_MENU_TEXT As String = "Разъединить"
    Private Const EXIT_MENU_TEXT As String = "Выход"

    Private mIsRefreshWatchFormShown As Boolean
    Private mMakeFire As Boolean = False
    Private mAliveTimer As New Threading.Timer(Sub() If devPort.IsOpen Then devPort.Write(COMMAND_COMP_ALIVE))

    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        AddHandler RefreshWatch.RefreshWatchingEvent, Sub()
                                                          If mHasConnection Then
                                                              mWatching = True
                                                              icon_NI.Icon = My.Resources.green
                                                              showRefreshWatching(False)
                                                          End If
                                                      End Sub
        AddHandler RefreshWatch.DisconnectEvent, Sub() disconnect()
        AddHandler RefreshWatch.OnShownEvent, Sub()
                                                  If mMakeFire Then
                                                      mMakeFire = False
                                                      mIsRefreshWatchFormShown = True
                                                      switchDesktop()
                                                      icon_NI.Icon = My.Resources.yellow
                                                  End If
                                              End Sub
        AddHandler RefreshWatch.OnClosedEvent, Sub() mIsRefreshWatchFormShown = False
        Dim cms = New ContextMenuStrip()
        Dim tsMenu(1) As ToolStripMenuItem
        tsMenu(0) = New ToolStripMenuItem(CONNECT_MENU_TEXT)
        AddHandler tsMenu(0).Click, Sub()
                                        If mHasConnection Then
                                            disconnect()
                                        Else
                                            connect()
                                        End If
                                    End Sub
        tsMenu(1) = New ToolStripMenuItem(EXIT_MENU_TEXT)
        AddHandler tsMenu(1).Click, Sub() Application.Exit()
        cms.Items.AddRange(tsMenu)
        icon_NI.ContextMenuStrip = cms
        icon_NI.Icon = My.Resources.red
        Me.TransparencyKey = BackColor
    End Sub

    Private Delegate Sub SafechangeConnectMenuText()

    Private Sub changeConnectMenuText()
        If icon_NI.ContextMenuStrip.InvokeRequired Then
            icon_NI.ContextMenuStrip.Invoke(New SafechangeConnectMenuText(AddressOf changeConnectMenuText))
        End If
        If mHasConnection Then
            icon_NI.ContextMenuStrip.Items.Item(0).Text = DISCONNECT_MENU_TEXT
        Else
            icon_NI.ContextMenuStrip.Items.Item(0).Text = CONNECT_MENU_TEXT
        End If
    End Sub

    Private Sub disconnect()
        If Not mHasConnection Then Return
        showRefreshWatching(False)
        mHasConnection = False
        mWorkingThread.Join()
        devPort.Write(COMMAND_DISCONNECT)
        mAliveTimer.Change(Threading.Timeout.Infinite, SEND_COMP_ALIVE_TIMEOUT)
        devPort.Close()
        log("Соединение сброшено", ToolTipIcon.Info)
        changeConnectMenuText()
        icon_NI.Icon = My.Resources.red
    End Sub

    Private Sub connect()
        If mHasConnection Then Return
        Dim buff(0) As Char
        mWatching = True
        devPort.ReadTimeout = SCAN_TIMEOUT
        For Each sp As String In My.Computer.Ports.SerialPortNames
            Debug.WriteLine(sp)
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
            log("Устройство не найдено.", ToolTipIcon.Warning)
            changeConnectMenuText()
            Return
        End If
        log("Соединение установлено (" & devPort.PortName & ")", ToolTipIcon.Info)
        changeConnectMenuText()
        devPort.Write(COMMAND_ANSWER)
        mAliveTimer.Change(0, SEND_COMP_ALIVE_TIMEOUT)
        Threading.Thread.Sleep(CHECK_DEVICE_TIMEOUT)
        devPort.ReadTimeout = CHECK_DEVICE_TIMEOUT
        mWorkingThread = New Threading.Thread(New Threading.ThreadStart(AddressOf working))
        mWorkingThread.IsBackground = True
        mWorkingThread.Start()
        icon_NI.Icon = My.Resources.green
    End Sub

    Private Sub switchDesktop()
        keybd_event(VK_LCONTROL, 0, 0, 0)
        Threading.Thread.Sleep(20)
        keybd_event(VK_LWIN, 0, 0, 0)
        Threading.Thread.Sleep(20)
        keybd_event(VK_RIGHT, 0, 0, 0)
        Threading.Thread.Sleep(300)
        keybd_event(VK_RIGHT, 0, KEYEVENTF_KEYUP, 0)
        Threading.Thread.Sleep(20)
        keybd_event(VK_LWIN, 0, KEYEVENTF_KEYUP, 0)
        Threading.Thread.Sleep(20)
        keybd_event(VK_LCONTROL, 0, KEYEVENTF_KEYUP, 0)
    End Sub

    Private Delegate Sub SafeShowRefreshWatching(isShow As Boolean)

    Private Sub showRefreshWatching(isShow As Boolean)
        If mIsRefreshWatchFormShown = isShow Then Return
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
        If mWatching Then
            mWatching = False
            mMakeFire = True
            showRefreshWatching(True) 'Когда окно запустится, только после этого переключим рабочий стол по событию RefreshWatch.OnShownEvent
        End If
    End Sub

    Private Delegate Sub SafeLog(m As String, toolTipIcon As ToolTipIcon)

    Private Sub log(m As String, toolTipIcon As ToolTipIcon)
        icon_NI.ShowBalloonTip(1000, "", m, toolTipIcon)
    End Sub

    Private Sub working()
        Dim buff(0) As Char
        Do
            Try
                devPort.Read(buff, 0, 1)
            Catch ex As Exception
                log("Устройство перестало отвечать.", ToolTipIcon.Warning)
                showRefreshWatching(False)
                mAliveTimer.Change(Threading.Timeout.Infinite, SEND_COMP_ALIVE_TIMEOUT)
                devPort.Close()
                mHasConnection = False
                changeConnectMenuText()
                icon_NI.Icon = My.Resources.red
                Return
            End Try
            If Not buff(0) = COMMAND_IN_WORK Then
                If buff(0) = COMMAND_FIRE Then
                    onFire()
                ElseIf buff(0) = COMMAND_READY Then 'Компьютер завис и устройство отключилось, переподключаем.
                    mHasConnection = False
                    connect()
                Else
                    log("От устройства пришли неожиданные данные: " & buff(0), ToolTipIcon.Error)
                    showRefreshWatching(False)
                    devPort.Write(COMMAND_DISCONNECT) 'на всякий случай посылаем команду перезагрузиться устройству.
                    mAliveTimer.Change(Threading.Timeout.Infinite, SEND_COMP_ALIVE_TIMEOUT)
                    devPort.Close()
                    mHasConnection = False
                    changeConnectMenuText()
                    icon_NI.Icon = My.Resources.red
                    Return
                End If
            End If
            If Not mHasConnection Then Exit Do
        Loop
    End Sub

    Private Sub MainForm_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed
        disconnect()
    End Sub

    Private Sub MainForm_Paint(sender As Object, e As PaintEventArgs) Handles Me.Paint
        logoTimer.Start()
    End Sub

    Private Sub logoTimer_Tick(sender As Object, e As EventArgs) Handles logoTimer.Tick
        Me.Hide()
        logoTimer.Stop()
    End Sub

    Declare Auto Sub keybd_event Lib "user32" (ByVal bVk As Byte, ByVal bScan As Byte, ByVal dwFlags As Integer, ByVal dwExtraInfo As Integer)
End Class

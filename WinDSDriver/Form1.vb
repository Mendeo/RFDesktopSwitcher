Public Class Form1
    Private Const READY As Char = "r"c
    Private Const IN_WORK As Char = "w"c
    Private Const ANSWER As Char = "o"c
    Private Const FIRE As Char = "f"c
    Private Const SCAN_TIMEOUT As Integer = 100 * 2
    Private Const CHECK_DEVICE_TIMEOUT As Integer = 1000 * 2
    Private mHasConnection As Boolean = False
    Private mWorkingThread As Threading.Thread

    Private Const VK_LCONTROL As Byte = &HA2
    Private Const VK_LWIN As Byte = &H5B
    Private Const VK_RIGHT As Byte = &H27
    Private Const KEYEVENTF_KEYUP As Byte = &H2

    Private Sub Bt_Click(sender As Object, e As EventArgs) Handles Button1.Click
        'log(OpenDesktopA("Themen Desktop", 0, False, ACCESS_MASK.GENERIC_ALL))

        'Dim hdesk As Integer '= OpenInputDesktop(0, False, ACCESS_MASK.GENERIC_ALL)
        'hdesk = CreateDesktopA("Hi", "df", IntPtr.Zero, 0, ACCESS_MASK.GENERIC_ALL, IntPtr.Zero)
        'hdesk = GetThreadDesktop(GetCurrentThreadId()) 'Threading.Thread.CurrentThread.ManagedThreadId)
        'log(hdesk)
        'Threading.Thread.Sleep(10000)
        'log(SetThreadDesktop(hdesk))
        'log(Hex(ACCESS_MASK.GENERIC_ALL))

        keybd_event(VK_LCONTROL, 0, 0, 0)
        keybd_event(VK_LWIN, 0, 0, 0)
        keybd_event(VK_RIGHT, 0, 0, 0)
        keybd_event(VK_RIGHT, 0, KEYEVENTF_KEYUP, 0)
        keybd_event(VK_LWIN, 0, KEYEVENTF_KEYUP, 0)
        keybd_event(VK_LCONTROL, 0, KEYEVENTF_KEYUP, 0)





        'If mHasConnection Then
        '    If mWorkingThread IsNot Nothing Then mWorkingThread.Abort()
        'Else
        '    Dim buff(0) As Char
        '    devPort.ReadTimeout = SCAN_TIMEOUT
        '    For Each sp As String In My.Computer.Ports.SerialPortNames
        '        devPort.PortName = sp
        '        Try
        '            devPort.Open()
        '            devPort.Read(buff, 0, 1)
        '        Catch ex As Exception
        '            buff(0) = vbNullChar
        '        End Try
        '        If buff(0) = READY Then
        '            mHasConnection = True
        '            Exit For
        '        End If
        '        devPort.Close()
        '    Next
        '    If Not mHasConnection Then
        '        log("Устройство не найдено.")
        '        Return
        '    End If
        '    log("Соединение установлено (" & devPort.PortName & ")")
        '    devPort.Write(ANSWER)
        '    Threading.Thread.Sleep(CHECK_DEVICE_TIMEOUT)
        '    'Читаем порт, проверяем, что устройство подключено.
        '    devPort.ReadTimeout = CHECK_DEVICE_TIMEOUT
        '    mWorkingThread = New Threading.Thread(New Threading.ThreadStart(AddressOf working))
        '    mWorkingThread.Start()
        'End If
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
        'mWorkingThread.Abort()
    End Sub

    <Flags>
    Private Enum ACCESS_MASK As Integer
        DESKTOP_READOBJECTS = &H1
        DESKTOP_CREATEWINDOW = &H2
        DESKTOP_CREATEMENU = &H4
        DESKTOP_HOOKCONTROL = &H8
        DESKTOP_JOURNALRECORD = &H10
        DESKTOP_JOURNALPLAYBACK = &H20
        DESKTOP_ENUMERATE = &H40
        DESKTOP_WRITEOBJECTS = &H80
        DESKTOP_SWITCHDESKTOP = &H100

        DELETE = &H10000
        READ_CONTROL = &H20000
        WRITE_DAC = &H40000
        WRITE_OWNER = &H80000
        SYNCHRONIZE = &H100000

        STANDARD_RIGHTS_ALL = DELETE Or READ_CONTROL Or WRITE_DAC Or WRITE_OWNER Or SYNCHRONIZE
        STANDARD_RIGHTS_EXECUTE = READ_CONTROL
        STANDARD_RIGHTS_READ = READ_CONTROL
        STANDARD_RIGHTS_REQUIRED = DELETE Or READ_CONTROL Or WRITE_DAC Or WRITE_OWNER
        STANDARD_RIGHTS_WRITE = READ_CONTROL

        GENERIC_READ = DESKTOP_ENUMERATE Or DESKTOP_READOBJECTS Or STANDARD_RIGHTS_READ
        GENERIC_WRITE = DESKTOP_CREATEMENU Or DESKTOP_CREATEWINDOW Or DESKTOP_HOOKCONTROL Or DESKTOP_JOURNALPLAYBACK Or DESKTOP_JOURNALRECORD Or DESKTOP_WRITEOBJECTS Or STANDARD_RIGHTS_WRITE
        GENERIC_EXECUTE = DESKTOP_SWITCHDESKTOP Or STANDARD_RIGHTS_EXECUTE
        GENERIC_ALL = DESKTOP_CREATEMENU Or DESKTOP_CREATEWINDOW Or DESKTOP_ENUMERATE Or DESKTOP_HOOKCONTROL Or DESKTOP_JOURNALPLAYBACK Or DESKTOP_JOURNALRECORD Or DESKTOP_READOBJECTS Or DESKTOP_SWITCHDESKTOP Or DESKTOP_WRITEOBJECTS Or STANDARD_RIGHTS_REQUIRED
    End Enum

    '<Flags>
    'Public Enum ACCESS_MASK As Integer
    '    DELETE = &H10000
    '    READ_CONTROL = &H20000
    '    WRITE_DAC = &H40000
    '    WRITE_OWNER = &H80000
    '    SYNCHRONIZE = &H100000

    '    STANDARD_RIGHTS_REQUIRED = &HF0000

    '    STANDARD_RIGHTS_READ = &H20000
    '    STANDARD_RIGHTS_WRITE = &H20000
    '    STANDARD_RIGHTS_EXECUTE = &H20000

    '    STANDARD_RIGHTS_ALL = &H1F0000

    '    SPECIFIC_RIGHTS_ALL = &HFFFF

    '    ACCESS_SYSTEM_SECURITY = &H1000000

    '    MAXIMUM_ALLOWED = &H2000000

    '    GENERIC_READ = &H80000000
    '    GENERIC_WRITE = &H40000000
    '    GENERIC_EXECUTE = &H20000000
    '    GENERIC_ALL = &H10000000

    '    DESKTOP_READOBJECTS = &H1
    '    DESKTOP_CREATEWINDOW = &H2
    '    DESKTOP_CREATEMENU = &H4
    '    DESKTOP_HOOKCONTROL = &H8
    '    DESKTOP_JOURNALRECORD = &H10
    '    DESKTOP_JOURNALPLAYBACK = &H20
    '    DESKTOP_ENUMERATE = &H40
    '    DESKTOP_WRITEOBJECTS = &H80
    '    DESKTOP_SWITCHDESKTOP = &H100

    '    WINSTA_ENUMDESKTOPS = &H1
    '    WINSTA_READATTRIBUTES = &H2
    '    WINSTA_ACCESSCLIPBOARD = &H4
    '    WINSTA_CREATEDESKTOP = &H8
    '    WINSTA_WRITEATTRIBUTES = &H10
    '    WINSTA_ACCESSGLOBALATOMS = &H20
    '    WINSTA_EXITWINDOWS = &H40
    '    WINSTA_ENUMERATE = &H100
    '    WINSTA_READSCREEN = &H200

    '    WINSTA_ALL_ACCESS = &H37F
    'End Enum

    '<StructLayout(LayoutKind.Sequential)>
    'Public Structure SECURITY_ATTRIBUTES
    '    Public nLength As Integer
    '    Public lpSecurityDescriptor As IntPtr
    '    Public bInheritHandle As Integer
    'End Structure

    Declare Auto Function OpenDesktopA Lib "user32" Alias "OpenDesktopA" (lpszDesktop As String, dwFlags As Integer, fInherit As Boolean, dwDesiredAccess As Integer) As Integer
    Declare Auto Function OpenInputDesktop Lib "user32" Alias "OpenInputDesktop" (dwFlags As Integer, fInherit As Boolean, dwDesiredAccess As Integer) As Integer
    Declare Auto Function CloseDesktop Lib "user32" Alias "CloseDesktop" (hDesktop As Integer) As Boolean
    Declare Auto Function CreateDesktopA Lib "user32" Alias "CreateDesktopA" (lpszDesktop As String, lpszDevice As String, pDevmode As IntPtr, dwFlags As Integer, dwDesiredAccess As Integer, lpsa As IntPtr) As Integer
    Declare Auto Function SwitchDesktop Lib "user32" Alias "SwitchDesktop" (hDesktop As Integer) As Boolean
    Declare Auto Function GetThreadDesktop Lib "user32" Alias "GetThreadDesktop" (dwThreadId As Integer) As Integer
    Declare Auto Function GetCurrentThreadId Lib "Kernel32" Alias "GetCurrentThreadId" () As Integer
    Declare Auto Function SetThreadDesktop Lib "user32" Alias "SetThreadDesktop" (hDesktop As Integer) As Boolean

    Declare Auto Sub keybd_event Lib "user32" (ByVal bVk As Byte, ByVal bScan As Byte, ByVal dwFlags As Integer, ByVal dwExtraInfo As Integer)
End Class

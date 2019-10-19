<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class MainForm
    Inherits System.Windows.Forms.Form

    'Форма переопределяет dispose для очистки списка компонентов.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Является обязательной для конструктора форм Windows Forms
    Private components As System.ComponentModel.IContainer

    'Примечание: следующая процедура является обязательной для конструктора форм Windows Forms
    'Для ее изменения используйте конструктор форм Windows Form.  
    'Не изменяйте ее в редакторе исходного кода.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainForm))
        Me.devPort = New System.IO.Ports.SerialPort(Me.components)
        Me.icon_NI = New System.Windows.Forms.NotifyIcon(Me.components)
        Me.logoTimer = New System.Windows.Forms.Timer(Me.components)
        Me.SuspendLayout()
        '
        'devPort
        '
        Me.devPort.BaudRate = 115200
        Me.devPort.ReadTimeout = 200
        '
        'icon_NI
        '
        Me.icon_NI.Icon = CType(resources.GetObject("icon_NI.Icon"), System.Drawing.Icon)
        Me.icon_NI.Text = "Desktop Switcher"
        Me.icon_NI.Visible = True
        '
        'logoTimer
        '
        Me.logoTimer.Interval = 3000
        '
        'MainForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackgroundImage = Global.WinDSDriver.My.Resources.Resources.Logo
        Me.ClientSize = New System.Drawing.Size(475, 175)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "MainForm"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Desktop Switcher"
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents devPort As IO.Ports.SerialPort
    Friend WithEvents icon_NI As NotifyIcon
    Friend WithEvents logoTimer As Timer
End Class

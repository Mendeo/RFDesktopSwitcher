<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Форма переопределяет dispose для очистки списка компонентов.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        Me.devPort = New System.IO.Ports.SerialPort(Me.components)
        Me.icon_NI = New System.Windows.Forms.NotifyIcon(Me.components)
        Me.Button1 = New System.Windows.Forms.Button()
        Me.console_RTB = New System.Windows.Forms.RichTextBox()
        Me.Button2 = New System.Windows.Forms.Button()
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
        Me.icon_NI.Text = "NotifyIcon1"
        Me.icon_NI.Visible = True
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(356, 12)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(75, 23)
        Me.Button1.TabIndex = 0
        Me.Button1.Text = "Button1"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'console_RTB
        '
        Me.console_RTB.Location = New System.Drawing.Point(13, 41)
        Me.console_RTB.Name = "console_RTB"
        Me.console_RTB.Size = New System.Drawing.Size(775, 397)
        Me.console_RTB.TabIndex = 1
        Me.console_RTB.Text = ""
        '
        'Button2
        '
        Me.Button2.Location = New System.Drawing.Point(506, 12)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(75, 23)
        Me.Button2.TabIndex = 2
        Me.Button2.Text = "Button2"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(800, 450)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.console_RTB)
        Me.Controls.Add(Me.Button1)
        Me.Name = "Form1"
        Me.Text = "Desktop Switcher"
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents devPort As IO.Ports.SerialPort
    Friend WithEvents icon_NI As NotifyIcon
    Friend WithEvents Button1 As Button
    Friend WithEvents console_RTB As RichTextBox
    Friend WithEvents Button2 As Button
End Class

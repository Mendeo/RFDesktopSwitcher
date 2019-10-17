<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class RefreshWatch
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
        Me.refreshWatching_BT = New System.Windows.Forms.Button()
        Me.disconnect_BT = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'refreshWatching_BT
        '
        Me.refreshWatching_BT.Location = New System.Drawing.Point(12, 12)
        Me.refreshWatching_BT.Name = "refreshWatching_BT"
        Me.refreshWatching_BT.Size = New System.Drawing.Size(191, 23)
        Me.refreshWatching_BT.TabIndex = 0
        Me.refreshWatching_BT.Text = "Возобновить отслеживание"
        Me.refreshWatching_BT.UseVisualStyleBackColor = True
        '
        'disconnect_BT
        '
        Me.disconnect_BT.Location = New System.Drawing.Point(12, 59)
        Me.disconnect_BT.Name = "disconnect_BT"
        Me.disconnect_BT.Size = New System.Drawing.Size(191, 23)
        Me.disconnect_BT.TabIndex = 1
        Me.disconnect_BT.Text = "Разорвать соединение"
        Me.disconnect_BT.UseVisualStyleBackColor = True
        '
        'RefreshWatch
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(221, 96)
        Me.Controls.Add(Me.disconnect_BT)
        Me.Controls.Add(Me.refreshWatching_BT)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Name = "RefreshWatch"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Desktop Switcher"
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents refreshWatching_BT As Button
    Friend WithEvents disconnect_BT As Button
End Class

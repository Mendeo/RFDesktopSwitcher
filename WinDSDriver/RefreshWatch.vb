Public Class RefreshWatch
    Private mSomeButtonClicked As Boolean = False
    Public InternalClose As Boolean = False
    Private Sub refreshWatching_BT_Click(sender As Object, e As EventArgs) Handles refreshWatching_BT.Click
        Form1.Watching = True
        mSomeButtonClicked = True
        Me.Close()
    End Sub

    Private Sub disconnect_BT_Click(sender As Object, e As EventArgs) Handles disconnect_BT.Click
        Form1.disconnect()
        mSomeButtonClicked = True
        Me.Close()
    End Sub

    Private Sub RefreshWatch_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed
        If Not mSomeButtonClicked AndAlso Not InternalClose Then
            Form1.disconnect()
        End If
    End Sub
End Class
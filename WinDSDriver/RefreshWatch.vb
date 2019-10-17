Public Class RefreshWatch
    Private mSomeButtonClicked As Boolean = False
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

    Private Sub RefreshWatch_Closed(sender As Object, e As EventArgs) Handles Me.Closed
        If Not mSomeButtonClicked Then Form1.disconnect()
    End Sub
End Class
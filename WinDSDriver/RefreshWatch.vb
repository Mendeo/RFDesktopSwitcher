Public Class RefreshWatch
    Private Sub refreshWatching_BT_Click(sender As Object, e As EventArgs) Handles refreshWatching_BT.Click
        Form1.Watching = True
        Me.Close()
    End Sub

    Private Sub disconnect_BT_Click(sender As Object, e As EventArgs) Handles disconnect_BT.Click
        Form1.disconnect()
        Me.Close()
    End Sub

    Private Sub RefreshWatch_Closed(sender As Object, e As EventArgs) Handles Me.Closed
        If Not Form1.Watching Then Form1.disconnect()
    End Sub
End Class
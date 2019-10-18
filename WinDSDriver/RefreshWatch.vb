Public Class RefreshWatch
    Private mSomeButtonClicked As Boolean = False
    Public Shared InternalClose As Boolean = False
    Public Shared IsShown As Boolean = False
    Public Shared Event RefreshWatchingEvent()
    Public Shared Event DisconnectEvent()

    Private Sub refreshWatching_BT_Click(sender As Object, e As EventArgs) Handles refreshWatching_BT.Click
        RaiseEvent RefreshWatchingEvent()
        mSomeButtonClicked = True
    End Sub

    Private Sub disconnect_BT_Click(sender As Object, e As EventArgs) Handles disconnect_BT.Click
        RaiseEvent DisconnectEvent()
        mSomeButtonClicked = True
    End Sub

    Private Sub RefreshWatch_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed
        If Not mSomeButtonClicked AndAlso Not InternalClose Then
            RaiseEvent DisconnectEvent()
        End If
        IsShown = False
    End Sub

    Private Sub RefreshWatch_Paint(sender As Object, e As PaintEventArgs) Handles Me.Paint
        IsShown = True
    End Sub
End Class
Imports System.Net.Http
Imports System.Threading
Imports System.Threading.Tasks

Public Class DownloadForm
    Private ReadOnly DownloadUrl As String
    Private ReadOnly DownloadCancellation As New CancellationTokenSource()

    Property Path As String

    Sub New(url As String, path As String)
        InitializeComponent()
        DownloadUrl = url
        Me.Path = path
    End Sub

    Protected Overrides Async Sub OnShown(e As EventArgs)
        MyBase.OnShown(e)
        Dim progress As New Progress(Of (Received As Long, Total As Long?))(AddressOf UpdateProgress)
        Try
            Await HttpFileDownload.DownloadAsync(DownloadUrl, Path, progress, DownloadCancellation.Token)
            If Not IsDisposed Then DialogResult = DialogResult.OK
        Catch ex As OperationCanceledException
            If Not IsDisposed Then DialogResult = DialogResult.Cancel
        Catch ex As Exception
            If Not IsDisposed Then
                DialogResult = DialogResult.Cancel
                g.ShowException(ex)
            End If
        Finally
            If Not IsDisposed Then Close()
            DownloadCancellation.Dispose()
        End Try
    End Sub

    Private Sub UpdateProgress(value As (Received As Long, Total As Long?))
        If IsDisposed OrElse Disposing Then Return
        Dim unit = PrefixedSize(2)
        Dim received = value.Received / unit.Factor
        If value.Total.HasValue AndAlso value.Total.Value > 0 Then
            Dim total = value.Total.Value / unit.Factor
            Text = $"Download - {received:0.#} {unit.Unit} of {total:0.#} {unit.Unit}"
            ProgressBar.Style = ProgressBarStyle.Blocks
            ProgressBar.Maximum = 1000
            ProgressBar.Value = CInt(Math.Min(1000.0, value.Received / CDbl(value.Total.Value) * 1000.0))
        Else
            Text = $"Download - {received:0.#} {unit.Unit}"
            ProgressBar.Style = ProgressBarStyle.Marquee
        End If
    End Sub

    Sub bnCancel_Click(sender As Object, e As EventArgs) Handles bnCancel.Click
        DownloadCancellation.Cancel()
        Close()
    End Sub

    Protected Overrides Sub OnFormClosed(e As FormClosedEventArgs)
        DownloadCancellation.Cancel()
        MyBase.OnFormClosed(e)
    End Sub
End Class

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

' Streams downloads without buffering the complete file in memory.
Public NotInheritable Class HttpFileDownload
    Public Shared Async Function DownloadAsync(url As String, path As String,
        progress As IProgress(Of (Received As Long, Total As Long?)),
        cancellation As CancellationToken) As Task

        Using client As New HttpClient()
            Using request As New HttpRequestMessage(HttpMethod.Get, url)
                request.Headers.UserAgent.ParseAdd("StaxRipNextGen/PreRelease")
                request.Headers.Referrer = New Uri(url)
                Using response = Await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellation).ConfigureAwait(False)
                    response.EnsureSuccessStatusCode()
                    Dim total = response.Content.Headers.ContentLength
                    Using input = Await response.Content.ReadAsStreamAsync(cancellation).ConfigureAwait(False)
                        Using output As New IO.FileStream(path, IO.FileMode.Create, IO.FileAccess.Write, IO.FileShare.None, 81920, True)
                            Dim buffer(81919) As Byte
                            Dim received As Long = 0
                            Do
                                Dim count = Await input.ReadAsync(buffer.AsMemory(), cancellation).ConfigureAwait(False)
                                If count = 0 Then Exit Do
                                Await output.WriteAsync(buffer.AsMemory(0, count), cancellation).ConfigureAwait(False)
                                received += count
                                progress?.Report((received, total))
                            Loop
                            Await output.FlushAsync(cancellation).ConfigureAwait(False)
                            If total.HasValue AndAlso received <> total.Value Then
                                Throw New IO.IOException("The download ended before the complete file was received.")
                            End If
                        End Using
                    End Using
                End Using
            End Using
        End Using
    End Function
End Class

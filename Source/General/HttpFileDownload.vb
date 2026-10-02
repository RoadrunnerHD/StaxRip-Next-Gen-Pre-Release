Imports System.Net.Http
Imports System.Threading
Imports System.Threading.Tasks

' Streams downloads without buffering the complete file in memory.
Public NotInheritable Class HttpFileDownload
    Public Shared Async Function DownloadAsync(url As String, path As String,
        progress As IProgress(Of (Received As Long, Total As Long?)),
        cancellation As CancellationToken) As Task

        Dim temporaryPath = IO.Path.Combine(IO.Path.GetDirectoryName(IO.Path.GetFullPath(path)),
            ".staxrip-download-" & Guid.NewGuid().ToString("N") & ".part")
        Try
            Using client As New HttpClient()
                Using request As New HttpRequestMessage(HttpMethod.Get, url)
                    request.Headers.UserAgent.ParseAdd("StaxRipNextGen/PreRelease")
                    request.Headers.Referrer = New Uri(url)
                    Using response = Await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellation).ConfigureAwait(False)
                        response.EnsureSuccessStatusCode()
                        Dim total = response.Content.Headers.ContentLength
                        Using input = Await response.Content.ReadAsStreamAsync(cancellation).ConfigureAwait(False)
                            Using output As New IO.FileStream(temporaryPath, IO.FileMode.CreateNew, IO.FileAccess.Write, IO.FileShare.None, 81920, True)
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
            cancellation.ThrowIfCancellationRequested()
            IO.File.Move(temporaryPath, path, True)
        Finally
            If IO.File.Exists(temporaryPath) Then IO.File.Delete(temporaryPath)
        End Try
    End Function
End Class

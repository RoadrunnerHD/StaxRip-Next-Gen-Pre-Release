Imports System.Net.Http
Imports System.Threading
Imports System.Threading.Tasks

' Streams downloads without buffering the complete file in memory.
Public NotInheritable Class HttpFileDownload
    Public Shared Async Function DownloadAsync(url As String, path As String,
        progress As IProgress(Of (Received As Long, Total As Long?)),
        cancellation As CancellationToken) As Task

        Using client = SecureHttp.CreateClient()
            Await DownloadAsync(client, url, path, progress, cancellation).ConfigureAwait(False)
        End Using
    End Function

    Friend Shared Async Function DownloadAsync(client As HttpClient, url As String, path As String,
        progress As IProgress(Of (Received As Long, Total As Long?)),
        cancellation As CancellationToken) As Task

        Dim temporaryPath = IO.Path.Combine(IO.Path.GetDirectoryName(IO.Path.GetFullPath(path)),
            ".staxrip-download-" & Guid.NewGuid().ToString("N") & ".part")
        Try
            Using response = Await SecureHttp.GetAsync(client, url, cancellation).ConfigureAwait(False)
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
            cancellation.ThrowIfCancellationRequested()
            IO.File.Move(temporaryPath, path, True)
        Finally
            If IO.File.Exists(temporaryPath) Then IO.File.Delete(temporaryPath)
        End Try
    End Function
End Class

' Automatic redirects must stay disabled: validate every hop before sending it.
Friend NotInheritable Class SecureHttp
    Public Shared Function CreateClient() As HttpClient
        Return New HttpClient(New HttpClientHandler With {.AllowAutoRedirect = False})
    End Function

    Public Shared Async Function GetAsync(client As HttpClient, url As String,
        cancellation As CancellationToken) As Task(Of HttpResponseMessage)
        Dim address As Uri = Nothing
        If Not Uri.TryCreate(url, UriKind.Absolute, address) Then Throw New HttpRequestException("The download URL is invalid.")
        For hop = 0 To 10
            If address.Scheme <> Uri.UriSchemeHttps OrElse address.UserInfo <> "" Then
                Throw New HttpRequestException("Tool downloads require HTTPS without credentials in the URL.")
            End If
            Using request As New HttpRequestMessage(HttpMethod.Get, address)
                request.Headers.UserAgent.ParseAdd("StaxRipNextGen/PreRelease")
                Dim response = Await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellation).ConfigureAwait(False)
                Dim status = CInt(response.StatusCode)
                If status = 301 OrElse status = 302 OrElse status = 303 OrElse status = 307 OrElse status = 308 Then
                    Dim location = response.Headers.Location
                    response.Dispose()
                    If location Is Nothing Then Throw New HttpRequestException("The download redirect has no destination.")
                    address = New Uri(address, location)
                Else
                    Try
                        response.EnsureSuccessStatusCode()
                        Return response
                    Catch
                        response.Dispose()
                        Throw
                    End Try
                End If
            End Using
        Next
        Throw New HttpRequestException("The download redirected too many times.")
    End Function
End Class

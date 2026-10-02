Imports System.IO
Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Threading
Imports System.Threading.Tasks
Imports System.Diagnostics
Imports System.Net.Http
Imports System.Net.Security
Imports System.Security.Authentication
Imports System.Security.Cryptography
Imports System.Security.Cryptography.X509Certificates

Module FileSafetyTests
    Sub Main()
        Dim arguments = Environment.GetCommandLineArgs()
        If arguments.Length = 3 AndAlso arguments(1) = "--echo-argument" Then
            Console.Write(arguments(2))
            Return
        End If
        RunAsync().GetAwaiter().GetResult()
    End Sub

    Private Async Function RunAsync() As Task
        Dim root = Path.Combine(Path.GetTempPath(), "staxrip-safety-test-" & Guid.NewGuid().ToString("N"))
        Directory.CreateDirectory(root)
        Try
            Await RedirectTests()
            For Each mode In {"success", "truncated", "cancel"}
                Await DownloadTest(root, mode)
            Next
            Dim source = Path.Combine(root, "source")
            Dim target = Path.Combine(root, "target")
            Directory.CreateDirectory(source)
            Directory.CreateDirectory(target)
            File.WriteAllText(Path.Combine(source, "tool.exe"), "new")
            File.WriteAllText(Path.Combine(source, "keep.ini"), "replacement")
            File.WriteAllText(Path.Combine(target, "tool.exe"), "old")
            File.WriteAllText(Path.Combine(target, "obsolete.txt"), "obsolete")
            File.WriteAllText(Path.Combine(target, "keep.ini"), "user settings")
            If OperatingSystem.IsWindows() Then
                Using locked As New FileStream(Path.Combine(source, "tool.exe"), FileMode.Open, FileAccess.Read, FileShare.None)
                    Try
                        ToolDirectoryTransaction.Install(source, target, {"keep.ini"})
                        Throw New Exception("A locked source must fail the update.")
                    Catch ex As IOException
                        Require(File.ReadAllText(Path.Combine(target, "tool.exe")) = "old", "Rollback must restore old tool.")
                        Require(File.Exists(Path.Combine(target, "obsolete.txt")), "Rollback must restore all old files.")
                    End Try
                End Using
            End If
            ToolDirectoryTransaction.Install(source, target, {"KEEP.INI"})
            Require(File.ReadAllText(Path.Combine(target, "tool.exe")) = "new", "New tool must be installed.")
            Require(File.ReadAllText(Path.Combine(target, "keep.ini")) = "user settings", "Keep entries must survive.")
            Require(Not File.Exists(Path.Combine(target, "obsolete.txt")), "Obsolete tool files must be removed.")
            Try
                ToolDirectoryTransaction.Install(target, target, Array.Empty(Of String)())
                Throw New Exception("Overlapping paths must be rejected.")
            Catch ex As IOException
                Require(File.ReadAllText(Path.Combine(target, "tool.exe")) = "new", "Rejected update must leave installed tool intact.")
            End Try
            If OperatingSystem.IsWindows() Then
                ShellTest(root)
            Else
                Console.WriteLine("SKIP: Windows file-lock rollback and cmd path tests require Windows.")
            End If
            Console.WriteLine("PASS: download replacement, truncated/canceled download preservation, tool replacement, kept files, overlapping paths.")
        Finally
            Directory.Delete(root, True)
        End Try
    End Function

    Private Async Function DownloadTest(root As String, mode As String) As Task
        Using key = RSA.Create(2048)
            Dim certificateRequest As New CertificateRequest("CN=localhost", key, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1)
            Using certificate = certificateRequest.CreateSelfSigned(DateTimeOffset.UtcNow.AddMinutes(-1), DateTimeOffset.UtcNow.AddHours(1))
                Dim path = IO.Path.Combine(root, mode & ".bin")
                File.WriteAllText(path, "previous download")
                Dim listener As New TcpListener(IPAddress.Loopback, 0)
                listener.Start()
                Dim port = DirectCast(listener.LocalEndpoint, IPEndPoint).Port
                Using cancellation As New CancellationTokenSource()
                    Dim server = Task.Run(Async Function()
                        Using connection = Await listener.AcceptTcpClientAsync()
                            Using stream As New SslStream(connection.GetStream())
                                Await stream.AuthenticateAsServerAsync(certificate, False, SslProtocols.Tls12, False)
                                Using reader As New StreamReader(stream, Encoding.ASCII, False, 1024, True)
                                    While Not String.IsNullOrEmpty(Await reader.ReadLineAsync())
                                    End While
                                End Using
                                Dim length = If(mode = "success", 3, 100)
                                Dim response = Encoding.ASCII.GetBytes("HTTP/1.1 200 OK" & vbCrLf & "Content-Length: " & length & vbCrLf & "Connection: close" & vbCrLf & vbCrLf & "new")
                                Await stream.WriteAsync(response.AsMemory())
                                If mode = "cancel" Then
                                    ' Cancel after the downloader has actually received bytes.
                                    Await Task.Delay(5000, cancellation.Token)
                                End If
                            End Using
                        End Using
                    End Function)
                    Dim progress As IProgress(Of (Received As Long, Total As Long?)) =
                        New InlineProgress(Sub(value)
                            If mode = "cancel" Then cancellation.Cancel()
                        End Sub)
                    Dim failed = False
                    Try
                        Using handler As New HttpClientHandler With {.AllowAutoRedirect = False}
                            ' Trust only this test's ephemeral local server certificate.
                            handler.ServerCertificateCustomValidationCallback = Function(request, remote, chain, errors) remote.GetCertHashString() = certificate.GetCertHashString()
                            Using client As New HttpClient(handler)
                                Await HttpFileDownload.DownloadAsync(client, "https://localhost:" & port & "/file", path, progress, cancellation.Token)
                            End Using
                        End Using
                    Catch ex As Exception When TypeOf ex Is IOException OrElse TypeOf ex Is OperationCanceledException OrElse TypeOf ex Is Net.Http.HttpRequestException
                        failed = True
                    Finally
                        listener.Stop()
                    End Try
                    Try
                        Await server
                    Catch ex As OperationCanceledException
                    End Try
                    Require(failed = (mode <> "success"), "Unexpected download outcome: " & mode)
                    Require(File.ReadAllText(path) = If(mode = "success", "new", "previous download"), "Download damaged existing file: " & mode)
                    Require(Directory.GetFiles(root, "*.part").Length = 0, "Temporary downloads must be cleaned up.")
                End Using
            End Using
        End Using
    End Function

    Private Async Function RedirectTests() As Task
        For Each mode In {"initial-http", "downgrade", "credentials", "loop", "secure"}
            Using handler As New RedirectHandler(mode)
                Using client As New HttpClient(handler)
                    Dim rejected = False
                    Try
                        Using response = Await SecureHttp.GetAsync(client, If(mode = "initial-http", "http://example.test/file", "https://example.test/file"), CancellationToken.None)
                            Require(mode = "secure", "Insecure redirects must not return a response.")
                        End Using
                    Catch ex As HttpRequestException
                        rejected = True
                    End Try
                    Require(rejected = (mode <> "secure"), "Incorrect redirect policy: " & mode)
                    If mode = "initial-http" Then Require(handler.Count = 0, "HTTP must be rejected before sending.")
                    If mode = "downgrade" OrElse mode = "credentials" Then Require(handler.Count = 1, "Unsafe redirect destination must never be requested.")
                    If mode = "secure" Then Require(handler.Count = 2, "HTTPS redirect must complete.")
                    If mode = "loop" Then Require(handler.Count = 11, "Redirect loops must be bounded.")
                End Using
            End Using
        Next
        Console.WriteLine("PASS: HTTPS-only requests, validated redirects, downgrade/URL-credential rejection and redirect limits.")
    End Function

    Private Class RedirectHandler
        Inherits HttpMessageHandler
        Private ReadOnly Mode As String
        Public Count As Integer
        Sub New(value As String)
            Mode = value
        End Sub
        Protected Overrides Function SendAsync(request As HttpRequestMessage, cancellationToken As CancellationToken) As Task(Of HttpResponseMessage)
            Count += 1
            Require(request.RequestUri.Scheme = "https", "Only HTTPS destinations may be sent.")
            If Mode = "secure" AndAlso Count = 2 Then Return Task.FromResult(New HttpResponseMessage(HttpStatusCode.OK) With {.Content = New StringContent("ok")})
            Dim response As New HttpResponseMessage(HttpStatusCode.Found)
            response.Headers.Location = New Uri(If(Mode = "downgrade", "http://example.test/file", If(Mode = "credentials", "https://user:password@example.test/file", "https://example.test/next")))
            Return Task.FromResult(response)
        End Function
    End Class

    Private Sub ShellTest(root As String)
        Dim input = Path.Combine(root, "path %PATH% ! & ^ (test).txt")
        Dim output = Path.Combine(root, "result %PATH% ! & ^ (test).txt")
        File.WriteAllText(input, "literal path worked")
        Dim start As New ProcessStartInfo("cmd.exe") With {.UseShellExecute = False}
        start.Arguments = EncoderShellCommand.Prepare("type """ & input & """ | findstr /C:""literal path worked"" > """ & output & """", start.Environment)
        Using process = Diagnostics.Process.Start(start)
            If Not process.WaitForExit(30000) Then
                process.Kill(True)
                Throw New Exception("cmd pipeline timed out.")
            End If
            Require(process.ExitCode = 0, "cmd pipeline failed.")
        End Using
        Require(File.ReadAllText(output).Trim() = "literal path worked", "cmd must preserve percent/exclamation/metacharacters in paths.")
        Dim directoryArgument = root & "\directory %PATH% ! & ^\"
        Dim echoStart As New ProcessStartInfo("cmd.exe") With {.UseShellExecute = False, .RedirectStandardOutput = True}
        echoStart.Arguments = EncoderShellCommand.Prepare("""" & Environment.ProcessPath & """ --echo-argument """ & directoryArgument & """", echoStart.Environment)
        Using echoProcess = Diagnostics.Process.Start(echoStart)
            If Not echoProcess.WaitForExit(30000) Then
                echoProcess.Kill(True)
                Throw New Exception("Direct cmd test timed out.")
            End If
            Dim text = echoProcess.StandardOutput.ReadToEnd()
            Require(echoProcess.ExitCode = 0 AndAlso text = directoryArgument, "External command must preserve a quoted trailing backslash.")
        End Using
        Console.WriteLine("PASS: Windows rollback and literal paths through cmd pipeline.")
    End Sub

    Private Sub Require(condition As Boolean, message As String)
        If Not condition Then Throw New Exception(message)
    End Sub

    Private Class InlineProgress
        Implements IProgress(Of (Received As Long, Total As Long?))
        Private ReadOnly callback As Action(Of (Received As Long, Total As Long?))
        Public Sub New(callback As Action(Of (Received As Long, Total As Long?)))
            Me.callback = callback
        End Sub
        Public Sub Report(value As (Received As Long, Total As Long?)) Implements IProgress(Of (Received As Long, Total As Long?)).Report
            callback(value)
        End Sub
    End Class
End Module

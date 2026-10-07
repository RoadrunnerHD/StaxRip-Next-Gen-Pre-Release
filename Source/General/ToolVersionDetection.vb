Imports System.Collections.Concurrent
Imports System.Diagnostics
Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Threading
Imports System.Threading.Tasks

' No DLL loading and no arbitrary executable switches. Only recognized console tools
' are queried; other EXE/DLL files are inspected through their version resources.
Public NotInheritable Class ToolVersionDetection
    Private Shared ReadOnly Cache As New ConcurrentDictionary(Of String, Lazy(Of String))(StringComparer.OrdinalIgnoreCase)

    Public Shared Function GetVersion(filePath As String, toolName As String) As String
        If String.IsNullOrWhiteSpace(filePath) OrElse Not File.Exists(filePath) Then Return ""
        Try
            Dim info As New FileInfo(filePath)
            Dim key = info.FullName & "|" & info.Length.ToString() & "|" & info.LastWriteTimeUtc.Ticks.ToString() & "|" & toolName
            Return Cache.GetOrAdd(key, Function(unused) New Lazy(Of String)(Function() Detect(info.FullName, toolName), LazyThreadSafetyMode.ExecutionAndPublication)).Value
        Catch
            Return ""
        End Try
    End Function

    Private Shared Function Detect(filePath As String, toolName As String) As String
        Dim args As String = Nothing
        Select Case toolName.ToLowerInvariant()
            Case "qsvencc", "nvencc", "vceencc", "python", "vspipe", "mkvmerge", "mkvextract", "mkvinfo", "dovi_tool"
                args = "--version"
            Case "ffmpeg", "ffprobe"
                args = "-version"
            Case "7zip"
                args = "i"
        End Select
        If args IsNot Nothing AndAlso System.IO.Path.GetExtension(filePath).Equals(".exe", StringComparison.OrdinalIgnoreCase) Then
            Dim detected = ParseConsoleVersion(toolName, ReadConsoleVersion(filePath, args))
            If detected <> "" Then Return detected
        End If
        Try
            Dim info = FileVersionInfo.GetVersionInfo(filePath)
            Dim product = info.ProductVersion
            Dim file = info.FileVersion
            Dim pv As Version = Nothing
            Dim fv As Version = Nothing
            If Version.TryParse(product, pv) AndAlso Version.TryParse(file, fv) AndAlso
                pv.Major = fv.Major AndAlso pv.Minor = fv.Minor AndAlso
                (fv.Build > Math.Max(pv.Build, 0) OrElse fv.Revision > Math.Max(pv.Revision, 0)) Then
                product = file ' Preserve a build/revision omitted from ProductVersion.
            End If
            For Each candidate In {product, file}
                If Not String.IsNullOrWhiteSpace(candidate) AndAlso Regex.IsMatch(candidate, "\d+\.\d+") AndAlso Not Regex.IsMatch(candidate.Trim(), "^0(?:\.0)+$") Then
                    Return candidate.Trim().Replace(";", "_")
                End If
            Next
        Catch
        End Try
        Return ""
    End Function

    Public Shared Function ParseConsoleVersion(toolName As String, output As String) As String
        If String.IsNullOrWhiteSpace(output) Then Return ""
        Dim pattern As String = Nothing
        Select Case toolName.ToLowerInvariant()
            Case "qsvencc", "nvencc", "vceencc"
                pattern = Regex.Escape(If(toolName.Equals("VCEEncC", StringComparison.OrdinalIgnoreCase), "VCEEnc", toolName)) & If(toolName.Equals("VCEEncC", StringComparison.OrdinalIgnoreCase), "C?", "") & "(?:\s+\([^\r\n)]*\))?\s+(?<v>\d+(?:\.\d+)+)(?:\s*\((?<r>r\d+)\))?"
            Case "ffmpeg", "ffprobe"
                pattern = Regex.Escape(toolName) & "\s+version\s+(?<v>[^\s]+)"
            Case "7zip"
                pattern = "7-Zip(?:\s+\([^\r\n)]*\))?\s+(?<v>\d+(?:\.\d+)+)"
            Case "python"
                pattern = "Python\s+(?<v>\d+(?:\.\d+)+)"
            Case "mkvmerge", "mkvextract", "mkvinfo", "dovi_tool"
                pattern = Regex.Escape(toolName) & "\s+v?(?<v>\d+(?:\.\d+)+)"
            Case "vspipe"
                pattern = "VapourSynth\s+Video\s+Processing\s+Library\s+Core\s+(?<v>R?\d+)"
        End Select
        If pattern Is Nothing Then Return ""
        Dim match = Regex.Match(output, pattern, RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(100))
        If Not match.Success Then Return ""
        Return match.Groups("v").Value & If(match.Groups("r").Success, " (" & match.Groups("r").Value & ")", "")
    End Function

    Private Shared Async Function ReadBounded(reader As StreamReader) As Task(Of String)
        Dim result As New System.Text.StringBuilder()
        Dim buffer(4095) As Char
        While result.Length < 65536
            Dim count = Await reader.ReadAsync(buffer, 0, Math.Min(buffer.Length, 65536 - result.Length)).ConfigureAwait(False)
            If count = 0 Then Exit While
            result.Append(buffer, 0, count)
        End While
        Return result.ToString()
    End Function

    Private Shared Function ReadConsoleVersion(filePath As String, args As String) As String
        Try
            Using process As New Process()
                process.StartInfo = New ProcessStartInfo(filePath, args) With {
                    .UseShellExecute = False, .CreateNoWindow = True,
                    .RedirectStandardOutput = True, .RedirectStandardError = True,
                    .WorkingDirectory = System.IO.Path.GetDirectoryName(filePath)}
                If Not process.Start() Then Return ""
                Dim stdout = ReadBounded(process.StandardOutput)
                Dim stderr = ReadBounded(process.StandardError)
                If Not process.WaitForExit(2000) Then
                    Try
                        process.Kill(True)
                    Catch
                    End Try
                    Return ""
                End If
                Dim both = Task.WhenAll(stdout, stderr)
                If Not both.Wait(500) Then Return ""
                Return String.Join(Environment.NewLine, both.Result)
            End Using
        Catch
            Return ""
        End Try
    End Function
End Class

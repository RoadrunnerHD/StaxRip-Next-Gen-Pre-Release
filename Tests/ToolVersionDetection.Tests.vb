Imports System.IO
Imports System.Diagnostics
Module ToolVersionTests
    Private Sub Check(actual As String, expected As String)
        If actual <> expected Then Throw New Exception($"Expected {expected}; got {actual}")
    End Sub
    Sub Main()
        Check(ToolVersionDetection.ParseConsoleVersion("QSVEncC", "QSVEncC (x64) 8.32 (r4654) by rigaya"), "8.32 (r4654)")
        Check(ToolVersionDetection.ParseConsoleVersion("QSVEncC", "QSVEncC 8.32 (r4654)"), "8.32 (r4654)")
        Check(ToolVersionDetection.ParseConsoleVersion("NVEncC", "NVEncC (x64) 9.35 (r4120)"), "9.35 (r4120)")
        Check(ToolVersionDetection.ParseConsoleVersion("QSVEncC", "failed; stale 8.32 (r4604)"), "")
        Check(ToolVersionDetection.ParseConsoleVersion("QSVEncC", "QSVEncC (x64) 8.32"), "8.32")
        Check(ToolVersionDetection.ParseConsoleVersion("VCEEncC", "VCEEnc (x64) 9.19 (r1234)"), "9.19 (r1234)")
        Check(ToolVersionDetection.ParseConsoleVersion("7zip", "7-Zip (a) 26.00 (x64)"), "26.00")
        Check(ToolVersionDetection.ParseConsoleVersion("ffmpeg", "ffmpeg version N-123456-gabc Copyright"), "N-123456-gabc")
        Check(ToolVersionDetection.GetVersion("missing.exe", "QSVEncC"), "")
        If Not OperatingSystem.IsWindows() Then
            Dim root = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"))
            Directory.CreateDirectory(root)
            Try
                Dim exe = Path.Combine(root, "Tool with spaces.exe")
                Dim counter = Path.Combine(root, "counter")
                File.WriteAllText(exe, "#!/bin/sh" & vbLf & "echo run >> '" & counter & "'" & vbLf & "echo 'QSVEncC (x64) 8.32 (r4654)' >&2" & vbLf)
                File.SetUnixFileMode(exe, UnixFileMode.UserRead Or UnixFileMode.UserWrite Or UnixFileMode.UserExecute)
                Check(ToolVersionDetection.GetVersion(exe, "QSVEncC"), "8.32 (r4654)")
                Check(ToolVersionDetection.GetVersion(exe, "QSVEncC"), "8.32 (r4654)")
                If File.ReadAllLines(counter).Length <> 1 Then Throw New Exception("Cache repeated the probe")
                File.WriteAllText(exe, File.ReadAllText(exe).Replace("r4654", "r4655"))
                File.SetLastWriteTimeUtc(exe, DateTime.UtcNow.AddSeconds(2))
                Check(ToolVersionDetection.GetVersion(exe, "QSVEncC"), "8.32 (r4655)")
                If File.ReadAllLines(counter).Length <> 2 Then Throw New Exception("Replacement not re-probed")
                Dim blocked = Path.Combine(root, "blocked.exe")
                File.Copy(exe, blocked)
                File.SetUnixFileMode(blocked, UnixFileMode.UserRead Or UnixFileMode.UserWrite Or UnixFileMode.UserExecute)
                Check(ToolVersionDetection.GetVersion(blocked, "UnrecognizedTool"), "")
                If File.ReadAllLines(counter).Length <> 2 Then Throw New Exception("Unknown executable was launched")
                Dim slow = Path.Combine(root, "slow.exe")
                File.WriteAllText(slow, "#!/bin/sh" & vbLf & "sleep 20" & vbLf)
                File.SetUnixFileMode(slow, UnixFileMode.UserRead Or UnixFileMode.UserWrite Or UnixFileMode.UserExecute)
                Dim watch = Stopwatch.StartNew()
                Check(ToolVersionDetection.GetVersion(slow, "QSVEncC"), "")
                If watch.Elapsed.TotalSeconds > 5 Then Throw New Exception("Probe timed out too late")
            Finally
                Directory.Delete(root, True)
            End Try
        End If
        Console.WriteLine("PASS: versions/revisions, stderr, paths with spaces, cache, replacement invalidation, unknown-tool protection and probe timeout.")
    End Sub
End Module

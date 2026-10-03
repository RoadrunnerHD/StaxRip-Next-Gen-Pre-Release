Imports System
Imports System.IO
Imports System.Linq

Module DolbyVisionLevel5CopyTests
    Sub Assert(condition As Boolean, message As String)
        If Not condition Then Throw New Exception(message)
    End Sub

    Sub Main()
        Dim root = Path.Combine(Path.GetTempPath(), "staxrip-l5-test-" & Guid.NewGuid().ToString("N"))
        Directory.CreateDirectory(root)
        Try
            Dim temp = Path.Combine(root, "Wahre Lügen.mkv_temp")
            Directory.CreateDirectory(temp)
            Dim source = Path.Combine(temp, "HDRDVmetadata_L5.json")
            Dim bytes = Text.Encoding.UTF8.GetBytes("{""presets"":[{""id"":0,""left"":12}]}")
            File.WriteAllBytes(source, bytes)
            Dim stamp = File.GetLastWriteTimeUtc(source)
            Dim target = DolbyVisionLevel5Copy.CopyToParent(temp & Path.DirectorySeparatorChar, source)
            Assert(target = Path.Combine(root, "HDRDVmetadata_L5.json"), "Wrong destination")
            Assert(File.ReadAllBytes(target).SequenceEqual(bytes), "Copy differs")
            Assert(File.ReadAllBytes(source).SequenceEqual(bytes), "Original changed")
            Assert(File.GetLastWriteTimeUtc(source) = stamp, "Original timestamp changed")
            File.WriteAllText(target, "old")
            DolbyVisionLevel5Copy.CopyToParent(temp, source)
            Assert(File.ReadAllBytes(target).SequenceEqual(bytes), "Existing copy not refreshed")
            Assert(DolbyVisionLevel5Copy.CopyToParent(temp, Nothing) Is Nothing, "Missing metadata copied")
            Assert(DolbyVisionLevel5Copy.CopyToParent(temp, Path.Combine(temp, "missing.json")) Is Nothing, "Missing file copied")
            Assert(DolbyVisionLevel5Copy.CopyToParent(Path.Combine(root, "absent"), source) Is Nothing, "Missing temp accepted")
            Assert(DolbyVisionLevel5Copy.CopyToParent(temp, target) Is Nothing, "Self-copy accepted")
            File.Delete(target)
            Directory.CreateDirectory(target)
            Dim failed = False
            Try
                DolbyVisionLevel5Copy.CopyToParent(temp, source)
            Catch ex As IOException
                failed = True
            Catch ex As UnauthorizedAccessException
                failed = True
            End Try
            Assert(failed, "Failure case unexpectedly succeeded")
            Assert(File.ReadAllBytes(source).SequenceEqual(bytes), "Failure changed original")
            Assert(Not Directory.EnumerateFiles(root, ".staxrip-l5-*.part").Any(), "Staging file left behind")
            Console.WriteLine("PASS: identical L5 copy, unchanged original, Unicode paths, refresh, missing files, self-copy, failure cleanup.")
        Finally
            Directory.Delete(root, True)
        End Try
    End Sub
End Module

Imports System
Imports System.IO

Module DolbyVisionLevel5CopyTests
    Sub Main()
        Dim root = Path.Combine(Path.GetTempPath(), "staxrip-l5-test-" & Guid.NewGuid().ToString("N"))
        Directory.CreateDirectory(root)
        Try
            Dim parent = Path.Combine(root, "Wahre Lügen")
            Dim temp = Path.Combine(parent, "Film.mkv_temp")
            Directory.CreateDirectory(temp)
            Dim metadata = Path.Combine(temp, "HDRDVmetadata_L5.json")
            Dim expected = Path.Combine(parent, "HDRDVmetadata_L5.json")
            Dim original = System.Text.Encoding.UTF8.GetBytes("{""presets"": [], ""edits"": {}}")
            File.WriteAllBytes(metadata, original)
            Require(DolbyVisionLevel5Copy.CopyToParent(temp & Path.DirectorySeparatorChar, metadata) = expected, "Wrong parent directory")
            Require(Convert.ToHexString(File.ReadAllBytes(expected)) = Convert.ToHexString(original), "Copy differs")
            Require(Convert.ToHexString(File.ReadAllBytes(metadata)) = Convert.ToHexString(original), "Temp metadata changed")

            File.WriteAllText(expected, "old metadata")
            DolbyVisionLevel5Copy.CopyToParent(temp, metadata)
            Require(Convert.ToHexString(File.ReadAllBytes(expected)) = Convert.ToHexString(original), "Existing copy not refreshed")
            Require(DolbyVisionLevel5Copy.CopyToParent(temp, expected) Is Nothing, "Self-copy not skipped")
            File.Delete(expected)
            Require(DolbyVisionLevel5Copy.CopyToParent(temp, Path.Combine(temp, "missing.json")) Is Nothing, "Missing metadata should be skipped")
            Require(Not File.Exists(expected), "Missing metadata created a destination")
            Require(DolbyVisionLevel5Copy.CopyToParent("", metadata) Is Nothing, "Empty TempDir should be skipped")
            Require(DolbyVisionLevel5Copy.CopyToParent(Path.Combine(root, "missing_temp"), metadata) Is Nothing, "Missing TempDir should be skipped")

            Directory.CreateDirectory(expected)
            Dim failed = False
            Try
                DolbyVisionLevel5Copy.CopyToParent(temp, metadata)
            Catch ex As IOException
                failed = True
            Catch ex As UnauthorizedAccessException
                failed = True
            End Try
            Require(failed, "Blocked destination should report failure")
            Require(Directory.Exists(expected), "Blocked destination changed")
            Require(Convert.ToHexString(File.ReadAllBytes(metadata)) = Convert.ToHexString(original), "Failure changed Temp metadata")
            Require(Directory.GetFiles(parent, ".staxrip-l5-*.part").Length = 0, "Staging file left behind")
            Console.WriteLine("PASS: L5 copy above TempDir, Unicode/trailing paths, exact content, unchanged Temp metadata, refresh, missing inputs and failed-copy cleanup.")
        Finally
            Directory.Delete(root, True)
        End Try
    End Sub

    Private Sub Require(condition As Boolean, message As String)
        If Not condition Then Throw New Exception(message)
    End Sub
End Module

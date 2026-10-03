Imports System.IO

Friend NotInheritable Class DolbyVisionLevel5Copy
    Public Shared Function CopyToParent(tempDirectory As String, metadataPath As String) As String
        If String.IsNullOrWhiteSpace(tempDirectory) OrElse
            String.IsNullOrWhiteSpace(metadataPath) OrElse Not File.Exists(metadataPath) Then Return Nothing
        Dim tempPath = Path.TrimEndingDirectorySeparator(Path.GetFullPath(tempDirectory))
        Dim parent = Directory.GetParent(tempPath)
        If parent Is Nothing OrElse Not Directory.Exists(tempPath) Then Return Nothing
        Dim destination = Path.Combine(parent.FullName, "HDRDVmetadata_L5.json")
        If String.Equals(Path.GetFullPath(metadataPath), destination, StringComparison.OrdinalIgnoreCase) Then Return Nothing
        Dim stagingPath = Path.Combine(parent.FullName, ".staxrip-l5-" & Guid.NewGuid().ToString("N") & ".part")
        Try
            File.Copy(metadataPath, stagingPath, False)
            File.Move(stagingPath, destination, True)
            Return destination
        Finally
            If File.Exists(stagingPath) Then File.Delete(stagingPath)
        End Try
    End Function
End Class

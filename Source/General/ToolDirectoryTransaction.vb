Imports System.IO
Imports System.Collections.Generic
Imports System.Linq

Public NotInheritable Class ToolDirectoryTransaction
    Public Shared Sub Install(source As String, target As String, keep As IEnumerable(Of String))
        source = Path.GetFullPath(source)
        target = Path.GetFullPath(target)
        If target.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) = Path.GetPathRoot(target).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) Then
            Throw New IOException("A tool update cannot replace a drive root.")
        End If
        target = target.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
        source = source.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
        Dim sourcePrefix = source.TrimEnd(Path.DirectorySeparatorChar) & Path.DirectorySeparatorChar
        Dim targetPrefix = target.TrimEnd(Path.DirectorySeparatorChar) & Path.DirectorySeparatorChar
        If source.Equals(target, StringComparison.OrdinalIgnoreCase) OrElse
            sourcePrefix.StartsWith(targetPrefix, StringComparison.OrdinalIgnoreCase) OrElse
            targetPrefix.StartsWith(sourcePrefix, StringComparison.OrdinalIgnoreCase) Then
            Throw New IOException("The extracted files and the tool directory must be separate.")
        End If
        ValidateTree(source)
        ValidateTree(target)
        Dim preserved As New HashSet(Of String)(If(keep, Array.Empty(Of String)()), StringComparer.OrdinalIgnoreCase)
        Dim originals = Directory.GetFileSystemEntries(target).
            Where(Function(entry) Not preserved.Contains(Path.GetFileName(entry))).ToArray()
        Dim incoming = Directory.GetFileSystemEntries(source).
            Where(Function(entry) Not preserved.Contains(Path.GetFileName(entry))).ToArray()
        Dim backup = Path.Combine(Path.GetDirectoryName(target), ".staxrip-backup-" & Guid.NewGuid().ToString("N"))
        Directory.CreateDirectory(backup)
        Dim moved As New List(Of String)
        Dim installed As New List(Of String)
        Try
            For Each entry In originals
                MoveEntry(entry, Path.Combine(backup, Path.GetFileName(entry)))
                moved.Add(Path.GetFileName(entry))
            Next
            For Each entry In incoming
                Dim destination = Path.Combine(target, Path.GetFileName(entry))
                installed.Add(destination) ' Include partial copies in rollback.
                CopyEntry(entry, destination)
            Next
        Catch installError As Exception
            Try
                For Each entry In installed
                    DeleteEntry(entry)
                Next
                For Each name In moved
                    MoveEntry(Path.Combine(backup, name), Path.Combine(target, name))
                Next
            Catch rollbackError As Exception
                Throw New AggregateException("The tool update failed. Recovery files were retained in: " & backup,
                    installError, rollbackError)
            End Try
            Directory.Delete(backup, True)
            Throw New IOException("The tool update failed; the previous files were restored.", installError)
        End Try
        ' Failure to clean up is not a failed installation. Retain the backup
        ' rather than attempting a rollback after installation has succeeded.
        Try
            Directory.Delete(backup, True)
        Catch ex As IOException
        Catch ex As UnauthorizedAccessException
        End Try
    End Sub

    Private Shared Sub ValidateTree(path As String)
        If (File.GetAttributes(path) And FileAttributes.ReparsePoint) <> 0 Then
            Throw New IOException("Tool updates cannot follow directory links: " & path)
        End If
        If Directory.Exists(path) Then
            For Each entry In Directory.GetFileSystemEntries(path)
                ValidateTree(entry)
            Next
        End If
    End Sub

    Private Shared Sub MoveEntry(source As String, destination As String)
        If Directory.Exists(source) Then
            Directory.Move(source, destination)
        Else
            File.Move(source, destination)
        End If
    End Sub

    Private Shared Sub CopyEntry(source As String, destination As String)
        If Directory.Exists(source) Then
            Directory.CreateDirectory(destination)
            For Each entry In Directory.GetFileSystemEntries(source)
                CopyEntry(entry, Path.Combine(destination, Path.GetFileName(entry)))
            Next
        Else
            File.Copy(source, destination, False)
        End If
    End Sub

    Private Shared Sub DeleteEntry(path As String)
        If Directory.Exists(path) Then
            Directory.Delete(path, True)
        ElseIf File.Exists(path) Then
            File.Delete(path)
        End If
    End Sub
End Class

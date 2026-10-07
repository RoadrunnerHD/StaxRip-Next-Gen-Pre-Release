Imports System.Collections.Generic
Imports System.Text.RegularExpressions
Imports System.Text
Imports System.Linq

Public NotInheritable Class EncoderShellCommand
    Public Shared Function Prepare(command As String, environment As IDictionary(Of String, String)) As String
        ' Expand literal arguments only in the final child command's delayed
        ' expansion phase. Inserted values are not rescanned for %, ! or shell
        ' operators. A pipeline needs a separate child shell for each side:
        ' cmd otherwise reparses pipeline commands after expanding their paths.
        ' Keep the existing Windows argument escaping for a trailing backslash.
        command = command.Replace("\""", "\\""")
        Dim prefix = "STAXRIP_LITERAL_" & Guid.NewGuid().ToString("N") & "_"
        Dim index = 0
        Dim prepared = Regex.Replace(command, """[^""]*""", Function(match)
            Dim name = prefix & index.ToString(Globalization.CultureInfo.InvariantCulture)
            index += 1
            environment(name) = match.Value.Substring(1, match.Length - 2)
            Return """!" & name & "!"""
        End Function)
        Dim parts As New List(Of String)
        Dim part As New StringBuilder()
        Dim quoted = False
        For Each character In prepared
            If character = """"c Then quoted = Not quoted
            If character = "|"c AndAlso Not quoted Then
                parts.Add(part.ToString().Trim())
                part.Clear()
            Else
                part.Append(character)
            End If
        Next
        parts.Add(part.ToString().Trim())
        If parts.Count = 1 Then Return "/D /V:ON /S /C """ & prepared & """"
        If parts.Any(Function(value) value.Length = 0) Then Throw New ArgumentException("An encoder pipeline contains an empty command.")
        Dim children = parts.Select(Function(value) "cmd.exe /D /V:ON /S /C """ & value & """")
        Return "/D /V:OFF /S /C """ & String.Join(" | ", children) & """"
    End Function
End Class

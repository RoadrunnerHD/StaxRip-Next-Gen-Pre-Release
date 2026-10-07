Imports System
Imports System.Text.RegularExpressions

' Script/pipe readers cannot copy container VUI with QSVEnc's "auto" values.
Public NotInheritable Class QsvColorMetadata
    Public Shared Function Resolve(commandLine As String, reader As String, metadata As Func(Of String, String)) As String
        If String.Equals(reader, "avhw", StringComparison.OrdinalIgnoreCase) OrElse
            String.Equals(reader, "avsw", StringComparison.OrdinalIgnoreCase) Then Return commandLine

        ' Consume quoted arguments separately so filenames and custom quoted text stay intact.
        Dim pattern = """[^""]*""|(?<!\S)--(?<name>colormatrix|colorprim|transfer|atc-sei|colorrange|range)(?:\s+|=)(?:auto|""auto"")(?!\S)"
        Return Regex.Replace(commandLine, pattern,
            Function(match As Match)
                If Not match.Groups("name").Success Then Return match.Value
                Dim name = match.Groups("name").Value.ToLowerInvariant()
                ' Alternative-transfer SEI is not the regular transfer characteristic.
                ' Do not fabricate ATC-SEI from the regular transfer metadata.
                If name = "atc-sei" Then Return ""
                Dim field As String
                Select Case name
                    Case "colormatrix" : field = "matrix_coefficients"
                    Case "colorprim" : field = "colour_primaries"
                    Case "transfer" : field = "transfer_characteristics"
                    Case Else : field = "colour_range"
                End Select
                Dim value = MapValue(name, If(metadata Is Nothing, Nothing, metadata(field)))
                If value Is Nothing Then Return ""
                Return "--" & name & " " & value
            End Function, RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant)
    End Function

    Private Shared Function MapValue(optionName As String, raw As String) As String
        If String.IsNullOrWhiteSpace(raw) Then Return Nothing
        Dim key = Regex.Replace(raw.Trim().ToLowerInvariant(), "[\s._-]", "")
        Select Case optionName
            Case "colorrange", "range"
                Select Case key
                    Case "limited" : Return "limited"
                    Case "full" : Return "full"
                End Select
            Case "colormatrix"
                Select Case key
                    Case "bt709" : Return "bt709"
                    Case "smpte170m" : Return "smpte170m"
                    Case "bt470bg" : Return "bt470bg"
                    Case "smpte240m" : Return "smpte240m"
                    Case "ycgco" : Return "YCgCo"
                    Case "fcc" : Return "fcc"
                    Case "gbr", "identity" : Return "GBR"
                    Case "bt2020nonconstant", "bt2020nc" : Return "bt2020nc"
                    Case "bt2020constant", "bt2020c" : Return "bt2020c"
                End Select
            Case "colorprim"
                Select Case key
                    Case "bt709" : Return "bt709"
                    Case "smpte170m" : Return "smpte170m"
                    Case "bt470m" : Return "bt470m"
                    Case "bt470bg" : Return "bt470bg"
                    Case "smpte240m" : Return "smpte240m"
                    Case "film" : Return "film"
                    Case "bt2020" : Return "bt2020"
                End Select
            Case "transfer"
                Select Case key
                    Case "bt709" : Return "bt709"
                    Case "smpte170m" : Return "smpte170m"
                    Case "bt470m" : Return "bt470m"
                    Case "bt470bg" : Return "bt470bg"
                    Case "smpte240m" : Return "smpte240m"
                    Case "linear" : Return "linear"
                    Case "log100" : Return "log100"
                    Case "log316" : Return "log316"
                    Case "iec6196624" : Return "iec61966-2-4"
                    Case "bt1361e" : Return "bt1361e"
                    Case "iec6196621", "srgb" : Return "iec61966-2-1"
                    Case "bt202010", "bt2020(10bit)" : Return "bt2020-10"
                    Case "bt202012", "bt2020(12bit)" : Return "bt2020-12"
                    Case "pq", "smptest2084", "smpte2084" : Return "smpte2084"
                    Case "smpte428", "smptest428" : Return "smpte428"
                    Case "hlg", "aribstdb67" : Return "arib-std-b67"
                End Select
        End Select
        ' Absent or unsupported metadata: omit the unsupported copy option.
        ' Keep explicit user selections and let the encoder use its normal defaults.
        Return Nothing
    End Function
End Class

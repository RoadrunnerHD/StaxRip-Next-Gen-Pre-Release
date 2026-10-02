Imports System
Imports System.Collections.Generic
Imports System.Text.RegularExpressions

Module ColorMetadataTests
    Private count As Integer
    Sub Main()
        Dim original = "--vbr 12000 --colormatrix auto --colorprim auto --transfer auto --atc-sei auto --d3d11"
        Dim sdr As New Dictionary(Of String, String) From {{"matrix_coefficients", "BT.709"}, {"colour_primaries", "BT.709"}, {"transfer_characteristics", "BT.709"}, {"colour_range", "Limited"}}
        Dim hdr As New Dictionary(Of String, String) From {{"matrix_coefficients", "BT.2020 non-constant"}, {"colour_primaries", "BT.2020"}, {"transfer_characteristics", "SMPTE ST 2084"}}
        For Each reader In {"avs", "ffqsv", "ffdxva"}
            Equal("SDR " & reader, Resolve(original, reader, sdr), "--vbr 12000 --colormatrix bt709 --colorprim bt709 --transfer bt709 --d3d11")
            Equal("HDR " & reader, Resolve(original, reader, hdr), "--vbr 12000 --colormatrix bt2020nc --colorprim bt2020 --transfer smpte2084 --d3d11")
        Next
        For Each reader In {"avhw", "avsw"}
            Dim result = QsvColorMetadata.Resolve(original, reader, Function(field) ThrowForMetadata())
            Equal("Direct reader unchanged " & reader, result, original)
        Next
        Equal("Unknown metadata", Resolve(original, "avs", New Dictionary(Of String, String)), "--vbr 12000 --d3d11")
        Equal("Null metadata delegate", QsvColorMetadata.Resolve("--transfer auto --vbr 12000", "avs", Nothing), "--vbr 12000")
        Dim manual = "--colormatrix bt709 --colorprim bt709 --transfer smpte2084 --atc-sei arib-std-b67 --vpp-colorspace matrix=auto:bt709 --atc-sei auto_res"
        Equal("Explicit selections and VPP untouched", Resolve(manual, "avs", hdr), manual)
        Equal("Range", Resolve("--colorrange auto --range auto", "avs", sdr), "--colorrange limited --range limited")
        hdr("transfer_characteristics") = "HLG"
        Equal("HLG", Resolve("--transfer auto --atc-sei arib-std-b67", "avs", hdr), "--transfer arib-std-b67 --atc-sei arib-std-b67")
        Equal("Unknown metadata is not guessed", Resolve("--colormatrix auto --transfer auto --vbr 12000", "avs", New Dictionary(Of String, String) From {{"matrix_coefficients", "unknown"}, {"transfer_characteristics", "new unsupported metadata"}}), "--vbr 12000")
        Equal("Quoted argument preserved", Resolve("--caption ""--transfer auto"" --transfer auto", "avs", sdr), "--caption ""--transfer auto"" --transfer bt709")
        Equal("Case, equals and quoted auto", Resolve("--COLORMATRIX=AUTO --transfer ""auto""", "avs", sdr), "--colormatrix bt709 --transfer bt709")
        Equal("PQ alias", Resolve("--transfer auto", "avs", New Dictionary(Of String, String) From {{"transfer_characteristics", "PQ"}}), "--transfer smpte2084")
        Equal("Manual override retained", Resolve("--transfer auto --transfer bt709", "avs", hdr), "--transfer arib-std-b67 --transfer bt709")
        Console.WriteLine("PASS: " & count.ToString() & " QSV color metadata regression cases.")
    End Sub
    Private Function Resolve(args As String, reader As String, values As Dictionary(Of String, String)) As String
        Return QsvColorMetadata.Resolve(args, reader,
            Function(field)
                Dim value As String = Nothing
                values.TryGetValue(field, value)
                Return value
            End Function)
    End Function
    Private Function ThrowForMetadata() As String
        Throw New Exception("Direct QSV reader must not query metadata in the resolver.")
    End Function
    Private Sub Equal(label As String, actual As String, expected As String)
        ' Ignore whitespace between tokens introduced by removing unsupported switches.
        actual = Regex.Replace(actual.Trim(), "\s+", " ")
        If actual <> expected Then Throw New Exception(label & ": " & actual & " <> " & expected)
        count += 1
    End Sub
End Module

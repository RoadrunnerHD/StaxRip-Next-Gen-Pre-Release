Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Text

' Headless Windows CI checks for the migrated runtime and embedded dependencies.
Public Class RuntimeSmokeTest
    Public Shared Sub Run()
        Dim output = IO.Path.Combine(Folder.Startup, "net10-smoke-test.txt")
        Try
            If Environment.Version.Major <> 10 Then Throw New Exception("Not running on .NET 10.")
            If TextEncoding.CodePageOfProcess <> 65001 Then Throw New Exception("The UTF-8 process manifest is not active.")
            Dim scriptEncodingFile = IO.Path.Combine(Folder.Startup, "smoke-encoding.avs")
            Dim scriptEncodingText = "LWLibavVideoSource(""Wahre Lügen – 日本語.mkv"")"
            scriptEncodingText.WriteFileProcessEncoding(scriptEncodingFile)
            Dim strictUtf8 As New UTF8Encoding(False, True)
            Dim scriptBytes = IO.File.ReadAllBytes(scriptEncodingFile)
            If Convert.ToBase64String(scriptBytes) <> Convert.ToBase64String(strictUtf8.GetBytes(scriptEncodingText)) Then Throw New Exception("Generated AviSynth script must be UTF-8 without BOM.")
            If IO.File.ReadAllText(scriptEncodingFile, strictUtf8) <> scriptEncodingText Then Throw New Exception("Generated AviSynth script is not valid UTF-8.")
            If scriptEncodingFile.ReadAllTextDefault() <> scriptEncodingText Then Throw New Exception("Script editor encoding roundtrip failed.")
            IO.File.Delete(scriptEncodingFile)
            Dim icon = My.Resources.Black
            If icon Is Nothing Then Throw New Exception("Embedded icon is missing.")
            Dim formatter = SerializationCompatibility.CreateFormatter()
            Dim values As New List(Of Object) From {Color.FromArgb(255, 20, 30, 40), New Dictionary(Of String, Integer) From {{"test", 42}}}
            Using stream As New IO.MemoryStream()
                formatter.Serialize(stream, values)
                stream.Position = 0
                Dim restored = DirectCast(formatter.Deserialize(stream), List(Of Object))
                If DirectCast(restored(0), Color).ToArgb() <> DirectCast(values(0), Color).ToArgb() Then Throw New Exception("Color roundtrip failed.")
                If DirectCast(restored(1), Dictionary(Of String, Integer))("test") <> 42 Then Throw New Exception("Settings collection roundtrip failed.")
            End Using
            Dim cultures As New List(Of Language) From {
                New Language(New Globalization.CultureInfo("de-DE")),
                New Language(Globalization.CultureInfo.InvariantCulture),
                New Language(New CustomCultureInfo("zh", "zh", "Custom Chinese", "zh", "chi"))}
            Dim restoredCultures = ObjectHelp.GetCopy(cultures)
            If restoredCultures(0).CultureInfo.Name <> "de-DE" Then Throw New Exception("German culture roundtrip failed.")
            If restoredCultures(1).CultureInfo.Name <> "" Then Throw New Exception("Invariant culture roundtrip failed.")
            If restoredCultures(2).CultureInfo.EnglishName <> "Custom Chinese" Then Throw New Exception("Custom language roundtrip failed.")
            Dim text = DirectCast(New Globalization.CultureInfo("tr-TR").TextInfo.Clone(), Globalization.TextInfo)
            text.ListSeparator = "|"
            Dim textCopy = ObjectHelp.GetCopy(text)
            If textCopy.ToUpper("i") <> "İ" OrElse textCopy.ListSeparator <> "|" Then Throw New Exception("TextInfo roundtrip failed.")
            Dim compareCopy = ObjectHelp.GetCopy(New Globalization.CultureInfo("de-DE").CompareInfo)
            If compareCopy.Name <> "de-DE" Then Throw New Exception("CompareInfo roundtrip failed.")
            Dim fixture = IO.Path.Combine(Folder.Startup, "legacy-cultures.bin")
            If IO.File.Exists(fixture) Then
                Using stream = IO.File.OpenRead(fixture)
                    Dim legacy = DirectCast(formatter.Deserialize(stream), List(Of Object))
                    If DirectCast(legacy(0), Globalization.CultureInfo).Name <> "de-DE" Then Throw New Exception("Framework culture import failed.")
                    If DirectCast(legacy(1), Globalization.CultureInfo).Name <> "" Then Throw New Exception("Framework invariant culture import failed.")
                    Dim legacyText = DirectCast(legacy(2), Globalization.TextInfo)
                    If legacyText.ToUpper("i") <> "İ" OrElse legacyText.ListSeparator <> "|" Then Throw New Exception("Framework TextInfo import failed.")
                    If DirectCast(legacy(3), Globalization.CompareInfo).Name <> "de-DE" Then Throw New Exception("Framework CompareInfo import failed.")
                    If DirectCast(legacy(4), Globalization.NumberFormatInfo).NumberDecimalSeparator <> "~" Then Throw New Exception("Framework number format import failed.")
                    If DirectCast(legacy(5), Globalization.DateTimeFormatInfo).ShortDatePattern <> "yyyy/MM/dd" Then Throw New Exception("Framework date format import failed.")
                    If DirectCast(legacy(6), Globalization.GregorianCalendar).TwoDigitYearMax <> 2099 Then Throw New Exception("Framework calendar import failed.")
                End Using
            Else
                Throw New Exception("Missing .NET Framework culture import fixture.")
            End If
            If Not IO.Directory.Exists(IO.Path.Combine(Folder.Startup, "Runtime")) Then Throw New Exception("Runtime folder is missing.")
            If Not IO.File.Exists(IO.Path.Combine(Folder.Startup, "StaxRipNG.exe")) Then Throw New Exception("Root launcher is missing.")
            If g.MainForm IsNot Nothing Then Throw New Exception("Headless profile test unexpectedly has a main window.")
            s.Init()
            If Not s.VideoEncoderProfiles.Any(Function(encoder) TypeOf encoder Is NVEnc) Then Throw New Exception("NVEnc default profiles are missing.")
            If Not s.VideoEncoderProfiles.Any(Function(encoder) TypeOf encoder Is QSVEnc) Then Throw New Exception("QSVEnc default profiles are missing.")
            If Not s.VideoEncoderProfiles.Any(Function(encoder) TypeOf encoder Is VCEEnc) Then Throw New Exception("VCEEnc default profiles are missing.")
            For Each encoder In s.VideoEncoderProfiles
                If encoder.GetError() IsNot Nothing Then Throw New Exception("Default encoder unexpectedly reports an error: " & encoder.Name)
            Next
            Dim copiedSettings = ObjectHelp.GetCopy(s)
            If copiedSettings.Version <> s.Version Then Throw New Exception("Full settings roundtrip failed.")
            If copiedSettings.AudioProfiles.Count <> s.AudioProfiles.Count Then Throw New Exception("Audio profiles roundtrip failed.")
            Dim settingsFile = IO.Path.Combine(Folder.Startup, "smoke-settings.dat")
            SafeSerialization.Serialize(s, settingsFile)
            Dim reloadedSettings = SafeSerialization.Deserialize(New ApplicationSettings(), settingsFile)
            If reloadedSettings.AudioProfiles.Count <> s.AudioProfiles.Count Then Throw New Exception("Settings file reload failed.")
            If reloadedSettings.VapourSynthProfiles.Count <> s.VapourSynthProfiles.Count Then Throw New Exception("Filter profiles reload failed.")
            IO.File.Delete(settingsFile)
            Dim project As New Project()
            project.Init()
            project.SourceFile = "source %PATH% ! & test.mkv"
            project.Log.WriteLine("project log roundtrip")
            Dim projectFile = IO.Path.Combine(Folder.Startup, "smoke-project.srip")
            SafeSerialization.Serialize(project, projectFile)
            Dim projectCopy = SafeSerialization.Deserialize(New Project(), projectFile)
            If projectCopy.SourceFile <> project.SourceFile Then Throw New Exception("Project source roundtrip failed.")
            If projectCopy.Log.ToString() <> project.Log.ToString() Then Throw New Exception("Project log roundtrip failed.")
            IO.File.Delete(projectFile)
            Using ps = System.Management.Automation.PowerShell.Create()
                Dim result = ps.AddScript("6 * 7").Invoke()
                If ps.HadErrors OrElse result.Count <> 1 OrElse result(0).ToString() <> "42" Then Throw New Exception("Embedded PowerShell failed.")
            End Using
            IO.File.WriteAllText(output, "PASS: .NET 10, embedded icon, full settings, languages, Framework import, compact launcher, PowerShell, default encoder error validation", Encoding.UTF8)
        Catch ex As Exception
            IO.File.WriteAllText(output, ex.ToString(), Encoding.UTF8)
            Environment.ExitCode = 1
        End Try
    End Sub
End Class

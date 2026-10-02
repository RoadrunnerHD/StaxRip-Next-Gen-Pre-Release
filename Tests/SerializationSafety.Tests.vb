Imports System.IO
Imports System.Drawing
Imports System.Globalization
Imports System.Runtime.Serialization

Module SerializationSafetyTests
    Private formatter As New CompatibleDataFormatter()

    Sub Main()
        Dim sample As New Sample With {.Name = "settings", .Items = New List(Of String) From {"one", Nothing, "two"}}
        sample.Next = sample
        Dim copy = CopyOf(sample)
        Require(copy.Name = sample.Name AndAlso copy.Items.Count = 3 AndAlso copy.Items(1) Is Nothing, "Application fields and lists failed.")
        Require(copy Is copy.Next, "References and cycles failed.")
        Require(Sample.Callbacks = 0, "Serialized callbacks must never execute.")
        Require(CopyOf(Color.FromArgb(255, 20, 30, 40)).ToArgb() = Color.FromArgb(255, 20, 30, 40).ToArgb(), "Color failed.")
        Require(CopyOf(New Dictionary(Of String, Integer) From {{"test", 42}})("test") = 42, "Dictionary failed.")
        Dim insensitive = CopyOf(New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase) From {{"Case", 42}})
        Require(insensitive("case") = 42, "Dictionary comparison rules failed.")
        Require(CopyOf(New Dictionary(Of String, Integer)(StringComparer.InvariantCultureIgnoreCase) From {{"Case", 42}})("case") = 42, "Culture comparison rules failed.")
        Require(CopyOf(New Text.StringBuilder("log contents")).ToString() = "log contents", "Log contents failed.")
        Require(CopyOf(New CultureInfo("de-DE")).Name = "de-DE", "Culture failed.")
        Require(CopyOf(CultureInfo.InvariantCulture).Name = "", "Invariant culture failed.")
        Dim custom = CopyOf(New CustomCultureInfo("zh", "zh", "Custom Chinese", "zh", "chi"))
        Require(custom.EnglishName = "Custom Chinese", "Custom culture failed.")
        Dim text = DirectCast(New CultureInfo("tr-TR").TextInfo.Clone(), TextInfo)
        text.ListSeparator = "|"
        Require(CopyOf(text).ListSeparator = "|", "TextInfo failed.")
        Require(CopyOf(New CultureInfo("de-DE").CompareInfo).Name = "de-DE", "CompareInfo failed.")
        Dim number As New NumberFormatInfo With {.NumberDecimalSeparator = "~"}
        Require(CopyOf(number).NumberDecimalSeparator = "~", "NumberFormatInfo failed.")
        Dim dateFormat As New DateTimeFormatInfo With {.ShortDatePattern = "yyyy/MM/dd"}
        Require(CopyOf(dateFormat).ShortDatePattern = "yyyy/MM/dd", "DateTimeFormatInfo failed.")
        Require(CopyOf(New GregorianCalendar With {.TwoDigitYearMax = 2099}).TwoDigitYearMax = 2099, "Calendar failed.")
        Require(CopyOf(New List(Of Sample) From {sample})(0).Name = "settings", "Typed object array failed.")
        Reject(New ConstructorGadget(), "ISerializable constructor")
        Require(ConstructorGadget.Calls = 0, "Serialized constructor must never execute.")
        Using stream As New MemoryStream()
            Dim writer = SerializationCompatibility.CreateWriter()
            writer.Binder = New RemappedTypeBinder("System.IO.FileInfo", GetType(Object).Assembly.FullName)
            writer.Serialize(stream, sample)
            stream.Position = 0
            RejectStream(stream, "Unsupported framework class")
        End Using
        Using stream As New MemoryStream()
            Dim writer = SerializationCompatibility.CreateWriter()
            writer.Binder = New RemappedTypeBinder(sample.GetType().FullName, "StaxRip, Version=0.1.0.0, Culture=neutral, PublicKeyToken=null")
            writer.Serialize(stream, sample)
            stream.Position = 0
            Require(DirectCast(formatter.Deserialize(stream), Sample).Name = sample.Name, "Legacy application assembly mapping failed.")
        End Using
        Reject(New Byte(8000000) {}, "Excessive array allocation")
        Dim deep As New Sample()
        For index = 0 To 140
            deep = New Sample With {.Next = deep}
        Next
        Reject(deep, "Excessive nesting")
        Console.WriteLine("PASS: data-only settings, references, collections, colors, cultures; callbacks/constructors suppressed; unsafe types and resource limits rejected.")
    End Sub

    Private Function CopyOf(Of T)(value As T) As T
        Using stream As New MemoryStream()
            formatter.Serialize(stream, value)
            stream.Position = 0
            Return DirectCast(formatter.Deserialize(stream), T)
        End Using
    End Function

    Private Sub Reject(value As Object, description As String)
        Using stream As New MemoryStream()
            formatter.Serialize(stream, value)
            stream.Position = 0
            RejectStream(stream, description)
        End Using
    End Sub

    Private Sub RejectStream(stream As Stream, description As String)
        Try
            formatter.Deserialize(stream)
        Catch ex As SerializationException
            Return
        End Try
        Throw New Exception(description & " must be rejected.")
    End Sub

    Private Class RemappedTypeBinder
        Inherits SerializationBinder
        Private ReadOnly name As String
        Private ReadOnly assembly As String
        Public Sub New(name As String, assembly As String)
            Me.name = name
            Me.assembly = assembly
        End Sub
        Public Overrides Sub BindToName(serializedType As Type, ByRef assemblyName As String, ByRef typeName As String)
            If serializedType Is GetType(Sample) Then
                assemblyName = assembly
                typeName = name
            End If
        End Sub
        Public Overrides Function BindToType(assemblyName As String, typeName As String) As Type
            Throw New NotSupportedException("The test binder is only used for writing.")
        End Function
    End Class

    Private Sub Require(condition As Boolean, message As String)
        If Not condition Then Throw New Exception(message)
    End Sub

    <Serializable>
    Private Class Sample
        Public Shared Callbacks As Integer
        Public Name As String
        Public Items As List(Of String)
        Public [Next] As Sample
        <OnDeserialized>
        Private Sub Callback(context As StreamingContext)
            Callbacks += 1
        End Sub
    End Class

    <Serializable>
    Private Class ConstructorGadget
        Implements ISerializable
        Public Shared Calls As Integer
        Public Sub New()
        End Sub
        Private Sub New(info As SerializationInfo, context As StreamingContext)
            Calls += 1
        End Sub
        Public Sub GetObjectData(info As SerializationInfo, context As StreamingContext) Implements ISerializable.GetObjectData
            info.AddValue("value", "payload")
        End Sub
    End Class
End Module

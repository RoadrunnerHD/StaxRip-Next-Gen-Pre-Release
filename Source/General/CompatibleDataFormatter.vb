Imports System.Collections
Imports System.Collections.Generic
Imports System.Globalization
Imports System.IO
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Runtime.Serialization
Imports System.Formats.Nrbf

' Keep the existing on-disk format, but never invoke BinaryFormatter.Deserialize.
' Decode records as data, then restore only approved fields and collections.
' No serialized constructors, callbacks, delegates or IObjectReference are run.
Public NotInheritable Class CompatibleDataFormatter
    Public Sub Serialize(stream As Stream, value As Object)
        SerializationCompatibility.CreateWriter().Serialize(stream, value)
    End Sub

    Public Function Deserialize(stream As Stream) As Object
        Const maximumBytes As Integer = 64 * 1024 * 1024
        Using bounded As New MemoryStream()
            Dim buffer(8191) As Byte
            Do
                Dim count = stream.Read(buffer, 0, buffer.Length)
                If count = 0 Then Exit Do
                If bounded.Length + count > maximumBytes Then Throw New SerializationException("The stored data exceeds the size limit.")
                bounded.Write(buffer, 0, count)
            Loop
            bounded.Position = 0
            Dim records As IReadOnlyDictionary(Of SerializationRecordId, SerializationRecord) = Nothing
            Dim root = NrbfDecoder.Decode(bounded, records, leaveOpen:=True)
            If records.Count > 200000 Then Throw New SerializationException("The stored data contains too many records.")
            Return New DataReader().Restore(root, 0)
        End Using
    End Function

    Private NotInheritable Class DataReader
        Private ReadOnly restored As New Dictionary(Of SerializationRecordId, Object)
        Private totalElements As Long

        Public Function Restore(value As Object, depth As Integer) As Object
            If depth > 128 Then Throw New SerializationException("The stored data is nested too deeply.")
            Dim record = TryCast(value, SerializationRecord)
            If record Is Nothing Then Return value
            Dim primitive = TryCast(record, PrimitiveTypeRecord)
            If primitive IsNot Nothing Then Return primitive.Value
            If restored.ContainsKey(record.Id) Then Return restored(record.Id)
            Dim type = ResolveType(record.TypeName.AssemblyQualifiedName)
            Dim arrayRecord = TryCast(record, ArrayRecord)
            If arrayRecord IsNot Nothing Then
                If arrayRecord.Rank <> 1 Then Throw New SerializationException("Only one-dimensional stored arrays are supported.")
                Dim length = arrayRecord.Lengths.ToArray()(0)
                totalElements += length
                If length < 0 OrElse totalElements > 8000000 Then Throw New SerializationException("The stored arrays exceed the allocation limit.")
                Dim raw = arrayRecord.GetArray(type)
                Dim result = Array.CreateInstance(type.GetElementType(), length)
                restored(record.Id) = result
                For index = 0 To length - 1
                    result.SetValue(Restore(raw.GetValue(index), depth + 1), index)
                Next
                Return result
            End If
            Dim data = TryCast(record, ClassRecord)
            If data Is Nothing Then Throw New SerializationException("Unsupported stored record.")
            If IsGlobalization(type) Then
                Dim fields As New Dictionary(Of String, Object)(StringComparer.OrdinalIgnoreCase)
                For Each name In data.MemberNames
                    fields(name) = Restore(data.GetRawValue(name), depth + 1)
                Next
                Dim result = SerializationCompatibility.RestoreGlobalization(type, fields)
                restored(record.Id) = result
                Return result
            End If
            If type Is GetType(Text.StringBuilder) Then
                Dim contents As String = ""
                For Each name In {"m_StringValue", "_stringValue"}
                    If data.HasMember(name) Then contents = CStr(Restore(data.GetRawValue(name), depth + 1)) : Exit For
                Next
                Dim result As New Text.StringBuilder(contents)
                restored(record.Id) = result
                Return result
            End If
            If type.IsEnum Then Return [Enum].ToObject(type, Restore(data.GetRawValue("value__"), depth + 1))
            If type Is GetType(Version) Then
                ' Framework settings contain System.Version. Restore through
                ' its validated public constructors instead of runtime fields.
                Dim major = VersionComponent(data, "Major")
                Dim minor = VersionComponent(data, "Minor")
                Dim build = VersionComponent(data, "Build")
                Dim revision = VersionComponent(data, "Revision")
                Dim result As Version
                If revision >= 0 Then
                    result = New Version(major, minor, build, revision)
                ElseIf build >= 0 Then
                    result = New Version(major, minor, build)
                Else
                    result = New Version(major, minor)
                End If
                restored(record.Id) = result
                Return result
            End If
            If GetType(IDictionary).IsAssignableFrom(type) Then
                Dim comparer As Object = Nothing
                If data.HasMember("Comparer") Then comparer = RestoreComparer(data.GetRawValue("Comparer"), type.GetGenericArguments()(0), depth + 1)
                Dim result = DirectCast(If(comparer Is Nothing, Activator.CreateInstance(type), Activator.CreateInstance(type, {comparer})), IDictionary)
                restored(record.Id) = result
                If data.HasMember("KeyValuePairs") Then
                    Dim pairs = DirectCast(Restore(data.GetRawValue("KeyValuePairs"), depth + 1), Array)
                    If pairs IsNot Nothing Then
                        For Each pair In pairs
                            Dim key = pair.GetType().GetProperty("Key").GetValue(pair)
                            Dim item = pair.GetType().GetProperty("Value").GetValue(pair)
                            result.Add(key, item)
                        Next
                    End If
                End If
                Return result
            End If
            If GetType(IList).IsAssignableFrom(type) Then
                Dim result = DirectCast(Activator.CreateInstance(type), IList)
                restored(record.Id) = result
                Dim items = DirectCast(Restore(data.GetRawValue("_items"), depth + 1), Array)
                Dim count = Convert.ToInt32(data.GetRawValue("_size"))
                If count < 0 OrElse count > items.Length Then Throw New SerializationException("Invalid stored list length.")
                For index = 0 To count - 1
                    result.Add(items.GetValue(index))
                Next
                Return result
            End If
            Dim instance = RuntimeHelpers.GetUninitializedObject(type)
            restored(record.Id) = instance
            For Each name In data.MemberNames
                Dim field = FindField(type, name)
                If field IsNot Nothing AndAlso Not field.IsNotSerialized Then
                    instance = FormatterServices.PopulateObjectMembers(instance, {field}, {Restore(data.GetRawValue(name), depth + 1)})
                End If
            Next
            ' Preserve the old Muxer callback's data default without executing
            ' any callback selected by the serialized input.
            Dim tagField = FindField(type, "Muxer+_TagFile")
            If tagField IsNot Nothing AndAlso tagField.GetValue(instance) Is Nothing Then
                instance = FormatterServices.PopulateObjectMembers(instance, {tagField}, {CObj("")})
            End If
            restored(record.Id) = instance
            Return instance
        End Function

        Private Shared Function VersionComponent(data As ClassRecord, component As String) As Integer
            For Each name In {"_" & component, component, "_" & component.ToLowerInvariant(), component.ToLowerInvariant()}
                If data.HasMember(name) Then Return Convert.ToInt32(data.GetRawValue(name), CultureInfo.InvariantCulture)
            Next
            Throw New SerializationException("Stored version is missing component: " & component)
        End Function

        Private Function RestoreComparer(value As Object, keyType As Type, depth As Integer) As Object
            If value Is Nothing Then Return Nothing
            Dim record = TryCast(value, ClassRecord)
            If record Is Nothing Then Throw New SerializationException("Unsupported stored dictionary comparer.")
            Dim name = record.TypeName.FullName
            If name.StartsWith("System.Collections.Generic.GenericEqualityComparer`1", StringComparison.Ordinal) OrElse
                name.StartsWith("System.Collections.Generic.ObjectEqualityComparer`1", StringComparison.Ordinal) OrElse
                name.StartsWith("System.Collections.Generic.EnumEqualityComparer`1", StringComparison.Ordinal) OrElse
                name.StartsWith("System.Collections.Generic.NullableEqualityComparer`1", StringComparison.Ordinal) Then Return Nothing
            If keyType Is GetType(String) Then
                If name = "System.OrdinalIgnoreCaseComparer" Then Return StringComparer.OrdinalIgnoreCase
                If name = "System.OrdinalCaseSensitiveComparer" Then Return StringComparer.Ordinal
                If name = "System.OrdinalComparer" Then
                    Dim ignoreCase = False
                    For Each member In {"_ignoreCase", "m_ignoreCase"}
                        If record.HasMember(member) Then ignoreCase = CBool(record.GetRawValue(member))
                    Next
                    Return If(ignoreCase, StringComparer.OrdinalIgnoreCase, StringComparer.Ordinal)
                End If
                If name = "System.CultureAwareComparer" Then
                    Dim compare As CompareInfo = Nothing
                    For Each member In {"_compareInfo", "m_compareInfo"}
                        If record.HasMember(member) Then compare = DirectCast(Restore(record.GetRawValue(member), depth + 1), CompareInfo)
                    Next
                    Dim ignoreCase = False
                    For Each member In {"_options", "m_options"}
                        If record.HasMember(member) Then
                            Dim options = CType(Restore(record.GetRawValue(member), depth + 1), CompareOptions)
                            If options <> CompareOptions.None AndAlso options <> CompareOptions.IgnoreCase Then Throw New SerializationException("Unsupported stored string comparison options.")
                            ignoreCase = options = CompareOptions.IgnoreCase
                        End If
                    Next
                    If compare IsNot Nothing Then Return StringComparer.Create(CultureInfo.GetCultureInfo(compare.Name), ignoreCase)
                End If
            End If
            Throw New SerializationException("Unsupported stored dictionary comparer: " & name)
        End Function

        Private Shared Function FindField(type As Type, name As String) As FieldInfo
            Dim separator = name.LastIndexOf("+"c)
            Dim fieldName = If(separator >= 0, name.Substring(separator + 1), name)
            While type IsNot Nothing
                If separator < 0 OrElse type.Name = name.Substring(0, separator) Then
                    Dim field = type.GetField(fieldName, BindingFlags.DeclaredOnly Or BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.NonPublic)
                    If field IsNot Nothing Then Return field
                End If
                type = type.BaseType
            End While
            Return Nothing ' Removed fields from older application versions.
        End Function

        Private Shared Function ResolveType(name As String) As Type
            ' The assembly resolver never loads anything named by the input.
            Dim assemblies = {GetType(CompatibleDataFormatter).Assembly, GetType(Object).Assembly,
                GetType(Drawing.Color).Assembly, GetType(System.Windows.Forms.Padding).Assembly,
                GetType(System.Windows.Forms.FormBorderStyle).Assembly, GetType(Diagnostics.ProcessPriorityClass).Assembly, GetType(Drawing.FontStyle).Assembly}
            Dim type As Type = System.Type.GetType(name,
                Function(assemblyName) assemblies.FirstOrDefault(Function(assembly) assembly.GetName().Name = assemblyName.Name OrElse
                    (assembly Is GetType(CompatibleDataFormatter).Assembly AndAlso assemblyName.Name = "StaxRip") OrElse
                    (assembly Is GetType(Object).Assembly AndAlso assemblyName.Name = "mscorlib") OrElse
                    (assembly Is GetType(Drawing.Color).Assembly AndAlso assemblyName.Name = "System.Drawing") OrElse
                    (assembly Is GetType(System.Windows.Forms.Padding).Assembly AndAlso assemblyName.Name = "System.Windows.Forms") OrElse
                    (assembly Is GetType(Diagnostics.ProcessPriorityClass).Assembly AndAlso assemblyName.Name = "System")),
                Function(assembly, typeName, ignoreCase)
                    If assembly IsNot Nothing Then
                        Dim match = assembly.GetType(typeName, False, ignoreCase)
                        If match IsNot Nothing Then Return match
                    End If
                    For Each candidate In assemblies
                        Dim match = candidate.GetType(typeName, False, ignoreCase)
                        If match IsNot Nothing Then Return match
                    Next
                    Return Nothing
                End Function, False)
            ' Surrogate records carry only explicitly supported culture data.
            If type IsNot Nothing AndAlso type.IsGenericType AndAlso type.GetGenericTypeDefinition().FullName =
                "StaxRip.SerializationCompatibility+GlobalizationRecord`1" Then type = type.GetGenericArguments()(0)
            If type Is Nothing OrElse Not IsApproved(type) Then Throw New SerializationException("Unsupported stored type: " & name)
            Return type
        End Function

        Private Shared Function IsGlobalization(type As Type) As Boolean
            Return type Is GetType(CultureInfo) OrElse type Is GetType(CustomCultureInfo) OrElse
                type Is GetType(TextInfo) OrElse type Is GetType(CompareInfo) OrElse
                type Is GetType(NumberFormatInfo) OrElse type Is GetType(DateTimeFormatInfo) OrElse
                (type.Assembly Is GetType(Calendar).Assembly AndAlso type.Namespace = "System.Globalization" AndAlso
                 Not type.IsAbstract AndAlso GetType(Calendar).IsAssignableFrom(type))
        End Function

        Private Shared Function IsApproved(type As Type) As Boolean
            If type.IsArray Then Return type.GetArrayRank() = 1 AndAlso IsApproved(type.GetElementType())
            If type.IsPrimitive OrElse type.IsEnum OrElse type Is GetType(String) OrElse type Is GetType(Object) OrElse
                type Is GetType(Decimal) OrElse type Is GetType(DateTime) OrElse type Is GetType(TimeSpan) OrElse
                type Is GetType(Version) OrElse type Is GetType(Text.StringBuilder) Then Return True
            If GetType([Delegate]).IsAssignableFrom(type) Then Return False
            If IsGlobalization(type) Then Return True
            If type.IsGenericType Then
                If Not type.GetGenericArguments().All(Function(argument) IsApproved(argument)) Then Return False
                Dim definition = type.GetGenericTypeDefinition()
                If definition Is GetType(List(Of )) OrElse definition Is GetType(Dictionary(Of ,)) OrElse
                    definition Is GetType(KeyValuePair(Of ,)) OrElse definition Is GetType(Nullable(Of )) Then Return True
            End If
            If type.Assembly Is GetType(CompatibleDataFormatter).Assembly Then
                If GetType(IList).IsAssignableFrom(type) AndAlso type.FullName <> "StaxRip.StringPairList" Then Return False
                If GetType(IDictionary).IsAssignableFrom(type) Then Return False
                Return type.IsSerializable AndAlso Not GetType(ISerializable).IsAssignableFrom(type)
            End If
            Return {GetType(Drawing.Color), GetType(Drawing.Point), GetType(Drawing.PointF), GetType(Drawing.Size),
                GetType(Drawing.SizeF), GetType(Drawing.Rectangle), GetType(Drawing.RectangleF), GetType(System.Windows.Forms.Padding)}.Contains(type)
        End Function
    End Class
End Class

Imports System.Globalization
Imports System.Runtime.Serialization
Imports System.Runtime.Serialization.Formatters.Binary

' Central factory for the legacy data format during the runtime migration.
Public NotInheritable Class SerializationCompatibility
    Public Shared Function CreateFormatter() As BinaryFormatter
        Dim selector As New SurrogateSelector()
        Dim context As New StreamingContext(StreamingContextStates.All)
        Dim culture As New CultureSurrogate()
        selector.AddSurrogate(GetType(CultureInfo), context, culture)
        selector.AddSurrogate(GetType(CustomCultureInfo), context, culture)
        selector.AddSurrogate(GetType(TextInfo), context, New TextSurrogate())
        selector.AddSurrogate(GetType(NumberFormatInfo), context, New FormatSurrogate())
        selector.AddSurrogate(GetType(DateTimeFormatInfo), context, New FormatSurrogate())
        For Each calendarType In GetType(Calendar).Assembly.GetTypes()
            If Not calendarType.IsAbstract AndAlso GetType(Calendar).IsAssignableFrom(calendarType) AndAlso calendarType.Namespace = "System.Globalization" Then
                selector.AddSurrogate(calendarType, context, New CalendarSurrogate())
            End If
        Next
        Return New BinaryFormatter With {.SurrogateSelector = selector, .Context = context, .Binder = New GlobalizationBinder()}
    End Function

    ' .NET 10 TextInfo still exposes an obsolete deserialization callback that throws.
    ' Map legacy auxiliary records to a proxy so callbacks on unsupported BCL objects
    ' are never registered by BinaryFormatter. Resolve the proxy through public APIs.
    Private Class GlobalizationBinder
        Inherits SerializationBinder
        Public Overrides Function BindToType(assemblyName As String, typeName As String) As Type
            If typeName = GetType(TextInfo).FullName Then Return GetType(GlobalizationRecord(Of TextInfo))
            If typeName = GetType(NumberFormatInfo).FullName Then Return GetType(GlobalizationRecord(Of NumberFormatInfo))
            If typeName = GetType(DateTimeFormatInfo).FullName Then Return GetType(GlobalizationRecord(Of DateTimeFormatInfo))
            If typeName.StartsWith("System.Globalization.", StringComparison.Ordinal) Then
                Dim type = GetType(Calendar).Assembly.GetType(typeName)
                If type IsNot Nothing AndAlso Not type.IsAbstract AndAlso GetType(Calendar).IsAssignableFrom(type) Then Return GetType(GlobalizationRecord(Of )).MakeGenericType(type)
            End If
            Return Nothing ' Preserve the formatter's normal binding for application and forwarded types.
        End Function
    End Class

    <Serializable>
    Private Class GlobalizationRecord(Of T)
        Implements ISerializable, IObjectReference
        Private ReadOnly info As SerializationInfo
        Private ReadOnly context As StreamingContext
        Private resolved As Object

        Private Sub New(info As SerializationInfo, context As StreamingContext)
            Me.info = info
            Me.context = context
        End Sub

        Public Sub GetObjectData(info As SerializationInfo, context As StreamingContext) Implements ISerializable.GetObjectData
            Throw New SerializationException("Globalization import records must resolve before being stored.")
        End Sub

        Public Function GetRealObject(context As StreamingContext) As Object Implements IObjectReference.GetRealObject
            If resolved Is Nothing Then
                Dim surrogate As ISerializationSurrogate
                If GetType(T) Is GetType(TextInfo) Then
                    surrogate = New TextSurrogate()
                ElseIf GetType(Calendar).IsAssignableFrom(GetType(T)) Then
                    surrogate = New CalendarSurrogate()
                Else
                    surrogate = New FormatSurrogate()
                End If
                resolved = surrogate.SetObjectData(FormatterServices.GetUninitializedObject(GetType(T)), info, Me.context, Nothing)
            End If
            Return resolved
        End Function
    End Class

    ' Legacy Framework cultures include cached TextInfo/CompareInfo objects.
    ' Recreate these through public culture APIs instead of restoring private runtime fields.
    Private Shared Function ReadFields(info As SerializationInfo) As Dictionary(Of String, Object)
        Dim fields As New Dictionary(Of String, Object)(StringComparer.OrdinalIgnoreCase)
        For Each entry As SerializationEntry In info
            fields(entry.Name) = entry.Value
            Dim separator = entry.Name.LastIndexOf("+"c)
            If separator >= 0 Then fields(entry.Name.Substring(separator + 1)) = entry.Value
        Next
        Return fields
    End Function

    Private Shared Function ReadCultureName(fields As Dictionary(Of String, Object), names As String(), ids As String()) As String
        For Each key In names
            If fields.ContainsKey(key) AndAlso TypeOf fields(key) Is String Then Return CStr(fields(key))
        Next
        For Each key In ids
            If fields.ContainsKey(key) Then Return CultureInfo.GetCultureInfo(Convert.ToInt32(fields(key))).Name
        Next
        Throw New SerializationException("Stored globalization object has no culture name or identifier.")
    End Function

    Private Class TextSurrogate
        Implements ISerializationSurrogate

        Public Sub GetObjectData(value As Object, info As SerializationInfo, context As StreamingContext) Implements ISerializationSurrogate.GetObjectData
            info.SetType(GetType(GlobalizationRecord(Of TextInfo)))
            Dim text = DirectCast(value, TextInfo)
            info.AddValue("CultureName", text.CultureName)
            info.AddValue("ListSeparator", text.ListSeparator)
            info.AddValue("ReadOnly", text.IsReadOnly)
        End Sub

        Public Function SetObjectData(value As Object, info As SerializationInfo, context As StreamingContext, selector As ISurrogateSelector) As Object Implements ISerializationSurrogate.SetObjectData
            Dim fields = ReadFields(info)
            Dim name = ReadCultureName(fields, {"CultureName", "m_cultureName", "customCultureName"}, {"m_win32LangID"})
            Dim text = DirectCast(New CultureInfo(name).TextInfo.Clone(), TextInfo)
            For Each key In {"ListSeparator", "m_listSeparator"}
                If fields.ContainsKey(key) AndAlso TypeOf fields(key) Is String Then
                    text.ListSeparator = CStr(fields(key))
                    Exit For
                End If
            Next
            For Each key In {"ReadOnly", "m_isReadOnly"}
                If fields.ContainsKey(key) AndAlso CBool(fields(key)) Then Return TextInfo.ReadOnly(text)
            Next
            Return text
        End Function
    End Class


    Private Shared Function StoredReadOnly(fields As Dictionary(Of String, Object)) As Boolean
        For Each key In {"ReadOnly", "m_isReadOnly", "isReadOnly"}
            If fields.ContainsKey(key) AndAlso CBool(fields(key)) Then Return True
        Next
        Return False
    End Function

    ' Framework cultures can also contain already initialized number/date/calendar caches.
    Private Class CalendarSurrogate
        Implements ISerializationSurrogate

        Public Sub GetObjectData(value As Object, info As SerializationInfo, context As StreamingContext) Implements ISerializationSurrogate.GetObjectData
            info.SetType(GetType(GlobalizationRecord(Of )).MakeGenericType(value.GetType()))
            Dim calendar = DirectCast(value, Calendar)
            info.AddValue("TwoDigitYearMax", calendar.TwoDigitYearMax)
            info.AddValue("ReadOnly", calendar.IsReadOnly)
            If TypeOf calendar Is GregorianCalendar Then info.AddValue("CalendarType", CInt(DirectCast(calendar, GregorianCalendar).CalendarType))
        End Sub

        Public Function SetObjectData(value As Object, info As SerializationInfo, context As StreamingContext, selector As ISurrogateSelector) As Object Implements ISerializationSurrogate.SetObjectData
            Dim fields = ReadFields(info)
            Dim calendar = DirectCast(Activator.CreateInstance(value.GetType()), Calendar)
            If TypeOf calendar Is GregorianCalendar Then
                For Each key In {"CalendarType", "m_type"}
                    If fields.ContainsKey(key) Then
                        DirectCast(calendar, GregorianCalendar).CalendarType = CType(Convert.ToInt32(fields(key)), GregorianCalendarTypes)
                        Exit For
                    End If
                Next
            End If
            For Each key In {"TwoDigitYearMax", "twoDigitYearMax"}
                If fields.ContainsKey(key) AndAlso Convert.ToInt32(fields(key)) >= 99 Then
                    calendar.TwoDigitYearMax = Convert.ToInt32(fields(key))
                    Exit For
                End If
            Next
            If StoredReadOnly(fields) Then calendar = Calendar.ReadOnly(calendar)
            Return calendar
        End Function
    End Class

    Private Class FormatSurrogate
        Implements ISerializationSurrogate

        Public Sub GetObjectData(value As Object, info As SerializationInfo, context As StreamingContext) Implements ISerializationSurrogate.GetObjectData
            info.SetType(GetType(GlobalizationRecord(Of )).MakeGenericType(value.GetType()))
            For Each prop In value.GetType().GetProperties(Reflection.BindingFlags.Public Or Reflection.BindingFlags.Instance)
                If prop.CanRead AndAlso prop.CanWrite AndAlso prop.GetIndexParameters().Length = 0 Then info.AddValue(prop.Name, prop.GetValue(value))
            Next
            info.AddValue("ReadOnly", If(TypeOf value Is NumberFormatInfo, DirectCast(value, NumberFormatInfo).IsReadOnly, DirectCast(value, DateTimeFormatInfo).IsReadOnly))
        End Sub

        Public Function SetObjectData(value As Object, info As SerializationInfo, context As StreamingContext, selector As ISurrogateSelector) As Object Implements ISerializationSurrogate.SetObjectData
            Dim fields = ReadFields(info)
            Dim format As Object
            If TypeOf value Is NumberFormatInfo Then
                format = New NumberFormatInfo()
            Else
                Dim name = If(fields.ContainsKey("m_name") AndAlso TypeOf fields("m_name") Is String, CStr(fields("m_name")), "")
                format = New CultureInfo(name).DateTimeFormat.Clone()
                If fields.ContainsKey("Calendar") AndAlso TypeOf fields("Calendar") Is Calendar Then DirectCast(format, DateTimeFormatInfo).Calendar = DirectCast(fields("Calendar"), Calendar)
                For Each aliasPair In {New String() {"genitiveMonthNames", "MonthGenitiveNames"}, New String() {"m_genitiveAbbreviatedMonthNames", "AbbreviatedMonthGenitiveNames"}, New String() {"m_superShortDayNames", "ShortestDayNames"}}
                    If fields.ContainsKey(aliasPair(0)) Then fields(aliasPair(1)) = fields(aliasPair(0))
                Next
            End If
            For Each prop In format.GetType().GetProperties(Reflection.BindingFlags.Public Or Reflection.BindingFlags.Instance)
                If Not prop.CanWrite OrElse prop.GetIndexParameters().Length <> 0 OrElse prop.Name = "Calendar" OrElse Not fields.ContainsKey(prop.Name) Then Continue For
                Dim stored = fields(prop.Name)
                If stored Is Nothing Then Continue For
                If prop.PropertyType.IsEnum Then
                    If Convert.ToInt32(stored) < 0 Then Continue For ' Framework lazily initializes these fields.
                    stored = [Enum].ToObject(prop.PropertyType, stored)
                End If
                prop.SetValue(format, stored)
            Next
            If StoredReadOnly(fields) Then
                If TypeOf format Is NumberFormatInfo Then Return NumberFormatInfo.ReadOnly(DirectCast(format, NumberFormatInfo))
                Return DateTimeFormatInfo.ReadOnly(DirectCast(format, DateTimeFormatInfo))
            End If
            Return format
        End Function
    End Class

    Private Class CultureSurrogate
        Implements ISerializationSurrogate

        Public Sub GetObjectData(value As Object, info As SerializationInfo, context As StreamingContext) Implements ISerializationSurrogate.GetObjectData
            Dim culture = DirectCast(value, CultureInfo)
            info.AddValue("Name", culture.Name)
            info.AddValue("UseUserOverride", culture.UseUserOverride)
            info.AddValue("ReadOnly", culture.IsReadOnly)
            If TypeOf culture Is CustomCultureInfo Then
                info.AddValue("Custom", True)
                info.AddValue("DisplayName", culture.EnglishName)
                info.AddValue("TwoLetter", culture.TwoLetterISOLanguageName)
                info.AddValue("ThreeLetter", culture.ThreeLetterISOLanguageName)
            End If
        End Sub

        Public Function SetObjectData(value As Object, info As SerializationInfo, context As StreamingContext, selector As ISurrogateSelector) As Object Implements ISerializationSurrogate.SetObjectData
            Dim values As New Dictionary(Of String, Object)(StringComparer.OrdinalIgnoreCase)
            For Each entry As SerializationEntry In info
                values(entry.Name) = entry.Value
                Dim inheritedSeparator = entry.Name.LastIndexOf("+"c)
                If inheritedSeparator >= 0 Then values(entry.Name.Substring(inheritedSeparator + 1)) = entry.Value
            Next
            Dim name As String = Nothing
            For Each key In {"Name", "m_name", "_name"}
                If values.ContainsKey(key) AndAlso TypeOf values(key) Is String Then
                    name = CStr(values(key))
                    Exit For
                End If
            Next
            If name Is Nothing Then
                For Each key In {"cultureID", "m_cultureID", "m_lcid"}
                    If values.ContainsKey(key) Then
                        name = CultureInfo.GetCultureInfo(Convert.ToInt32(values(key))).Name
                        Exit For
                    End If
                Next
            End If
            If name Is Nothing Then Throw New SerializationException("Stored culture has no name or culture identifier.")
            Dim userOverride = If(values.ContainsKey("UseUserOverride"), CBool(values("UseUserOverride")), If(values.ContainsKey("m_useUserOverride"), CBool(values("m_useUserOverride")), True))
            Dim culture As CultureInfo = New CultureInfo(name, userOverride)
            If values.ContainsKey("Custom") AndAlso CBool(values("Custom")) Then
                culture = New CustomCultureInfo(name, name, CStr(values("DisplayName")), CStr(values("TwoLetter")), CStr(values("ThreeLetter")))
            ElseIf TypeOf value Is CustomCultureInfo AndAlso values.ContainsKey("_displayName") Then
                culture = New CustomCultureInfo(name, name, CStr(values("_displayName")), CStr(values("_twoLetterCode")), CStr(values("_threeLetterCode")))
            End If
            If (values.ContainsKey("ReadOnly") AndAlso CBool(values("ReadOnly"))) OrElse (values.ContainsKey("m_isReadOnly") AndAlso CBool(values("m_isReadOnly"))) Then culture = CultureInfo.ReadOnly(culture)
            Return culture
        End Function
    End Class
End Class

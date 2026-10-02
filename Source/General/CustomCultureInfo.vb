Imports System.Globalization
Imports System.Runtime.Serialization

<Serializable()>
Public Class CustomCultureInfo
    Inherits CultureInfo
    Implements IDeserializationCallback

    Private _name As String
    Private _displayName As String
    Private _twoLetterCode As String
    Private _threeLetterCode As String

    Public Overrides ReadOnly Property Name As String
        Get
            Return If(String.IsNullOrWhiteSpace(_name), MyBase.Name, _name)
        End Get
    End Property

    Public Overrides ReadOnly Property EnglishName As String
        Get
            Return If(String.IsNullOrWhiteSpace(_displayName), MyBase.EnglishName, _displayName)
        End Get
    End Property

    Public Overrides ReadOnly Property DisplayName As String
        Get
            Return If(String.IsNullOrWhiteSpace(_displayName), MyBase.DisplayName, _displayName)
        End Get
    End Property

    Public Overrides ReadOnly Property TwoLetterISOLanguageName As String
        Get
            Return If(String.IsNullOrWhiteSpace(_twoLetterCode), MyBase.TwoLetterISOLanguageName, _twoLetterCode)
        End Get
    End Property

    Public Overrides ReadOnly Property ThreeLetterISOLanguageName As String
        Get
            Return If(String.IsNullOrWhiteSpace(_threeLetterCode), MyBase.ThreeLetterISOLanguageName, _threeLetterCode)
        End Get
    End Property

    Protected Sub New(baseCultureName As String)
        MyBase.New(baseCultureName)
    End Sub

    Public Sub New(baseCultureName As String, name As String, displayName As String, twoLetterCode As String, threeLetterCode As String)
        MyBase.New(baseCultureName)

        _name = name
        _displayName = displayName
        _twoLetterCode = twoLetterCode
        _threeLetterCode = threeLetterCode

        PatchInternals()
    End Sub

    Public Sub OnDeserialization(sender As Object) Implements IDeserializationCallback.OnDeserialization
        PatchInternals()
    End Sub

    Private Sub PatchInternals()
        Dim cultureField = GetType(CultureInfo).GetField("m_name", Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance)
        cultureField?.SetValue(Me, _name)

        Dim displayNameField = GetType(CultureInfo).GetField("m_cultureData", Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance)
        If displayNameField IsNot Nothing Then
            Dim cultureData = displayNameField.GetValue(Me)
            Dim displayNameProp = cultureData.GetType().GetField("sLocalizedDisplayName", Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance)
            displayNameProp?.SetValue(cultureData, _displayName)
            displayNameProp = cultureData.GetType().GetField("sEnglishDisplayName", Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance)
            displayNameProp?.SetValue(cultureData, _displayName)
            displayNameProp = cultureData.GetType().GetField("sISO639Language", Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance)
            displayNameProp?.SetValue(cultureData, _twoLetterCode)
            displayNameProp = cultureData.GetType().GetField("sISO639Language2", Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance)
            displayNameProp?.SetValue(cultureData, _threeLetterCode)
            displayNameProp = cultureData.GetType().GetField("sName", Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance)
            displayNameProp?.SetValue(cultureData, _name)
        End If
    End Sub
End Class


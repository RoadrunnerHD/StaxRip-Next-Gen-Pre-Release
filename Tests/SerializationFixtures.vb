Namespace Global.System.Windows.Forms
    Public Enum FormBorderStyle
        Sizable = 4
    End Enum
    <Serializable>
    Public Structure Padding
        Public All As Integer
    End Structure
End Namespace

Namespace Global.System.Drawing
    Public Enum FontStyle
        Regular = 0
        Bold = 1
    End Enum
End Namespace

<Serializable>
Public Class StringPairList
    Inherits System.Collections.Generic.List(Of String)
End Class

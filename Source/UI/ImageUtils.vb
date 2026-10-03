
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
Imports System.Drawing.Text
Imports System.Globalization
Imports System.Threading.Tasks

Public Class ImageHelp
    Private Shared Coll As PrivateFontCollection
    Private Shared ReadOnly AwesomePath As String = Path.Combine(Folder.Fonts, "Icons", "FontAwesome.ttf")
    Private Shared ReadOnly SegoePath As String = Path.Combine(Folder.Fonts, "Icons", "Segoe-MDL2-Assets.ttf")
    Private Shared ReadOnly FontFilesExist As Boolean = File.Exists(AwesomePath) AndAlso File.Exists(SegoePath)

    Shared Async Function GetSymbolImageAsync(symbol As Symbol, Optional color As Color = Nothing) As Task(Of Image)
        Return Await Task.Run(Function() GetSymbolImage(symbol, color))
    End Function

    Shared Function GetSymbolImage(
        symbol As Symbol,
        Optional color As Color = Nothing,
        Optional fontSize As Integer = 12) As Image

        If symbol = Symbol.DVOffset Then
            color = If(color = Nothing, ThemeManager.CurrentTheme.General.Controls.ToolStrip.SymbolImageColor.ToColor(), color)
            Dim side As Integer
            Using sizeReference = GetSymbolImage(Symbol.MultiSelectLegacy, color, fontSize)
                side = If(sizeReference Is Nothing, CInt(fontSize * 1.5F), sizeReference.Width)
            End Using
            Dim dvBitmap As New Bitmap(side, side)
            Using graphics As Graphics = Graphics.FromImage(dvBitmap)
                graphics.SmoothingMode = SmoothingMode.AntiAlias
                graphics.ScaleTransform(side / 18.0F, side / 18.0F)
                Using pen As New Pen(color, 1.25F),
                    letterD As New GraphicsPath()
                    graphics.DrawRectangle(pen, 1, 2, 16, 14)
                    letterD.AddLine(3, 5, 3, 13)
                    letterD.AddBezier(3, 13, 10, 13, 10, 5, 3, 5)
                    graphics.DrawPath(pen, letterD)
                    graphics.DrawLines(pen, {New PointF(10, 5), New PointF(12.5F, 13), New PointF(15, 5)})
                End Using
            End Using
            Return dvBitmap
        End If

        If Not FontFilesExist Then
            Return Nothing
        End If

        If Coll Is Nothing Then
            Coll = New PrivateFontCollection
            Coll.AddFontFile(AwesomePath)
            Coll.AddFontFile(SegoePath)
        End If

        Dim family = If(symbol > 61400,
            Coll.Families.FirstOrDefault(Function(f) f.Name = "FontAwesome"),
            Coll.Families.FirstOrDefault(Function(f) f.Name = "Segoe MDL2 Assets"))

        If family Is Nothing Then
            MsgWarn("Correct font was not found, using default instead!")
            family = FontFamily.GenericSerif
        End If

        color = If(color = Nothing, ThemeManager.CurrentTheme.General.Controls.ToolStrip.SymbolImageColor.ToColor(), color)

        Dim bitmap As Bitmap

        Using font As New Font(family, fontSize)
            Dim fontHeight = font.Height
            bitmap = New Bitmap(CInt(fontHeight * 1.1F), CInt(fontHeight * 1.1F))

            Using graphics As Graphics = Graphics.FromImage(bitmap)
                graphics.TextRenderingHint = TextRenderingHint.AntiAlias
                Using brush As Brush = New SolidBrush(color)
                    graphics.DrawString(Convert.ToChar(symbol), font, brush, -fontHeight * 0.1F, fontHeight * 0.07F)
                End Using
            End Using
        End Using

        Return bitmap
    End Function
End Class

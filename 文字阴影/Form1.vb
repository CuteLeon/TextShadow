Public Class Form1
    Dim MyFont As Font = New Font("微软雅黑", 36.0)
    Dim Radius As Integer = 10
    Dim DoubleRadius As Integer = Radius * 2

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Label1.AutoSize = True
        Label1.Font = MyFont
        Label1.Text = TextBox1.Text
        Dim D1 As Double = My.Computer.Clock.TickCount
        Label1.Image = TextShadowStroke(Label1.Text, MyFont, 10, Color.White, Color.Black, Color.Black, New Size(Label1.Width + DoubleRadius, Label1.Height + DoubleRadius))
        Dim D2 As Double = My.Computer.Clock.TickCount
        Label1.Text = vbNullString
        Label1.AutoSize = False
        Label1.Size = Label1.Image.Size
        'MsgBox((D2 - D1) / 1000)
    End Sub

    Private Function TextShadowStroke(ByVal DrawText As String, ByVal TextFont As Font, ByVal ShadowRadius As Integer, ByVal ForeColor As Color, ByVal ShadowColor As Color, ByVal StrokeColor As Color, ByVal BitmapSize As Size) As Bitmap
        Dim DoubleRadius As Integer = ShadowRadius * 2
        Dim ShadowColorCell() As Byte = {ShadowColor.R, ShadowColor.G, ShadowColor.B}
        Dim ResualtBitmap As Bitmap = New Bitmap(BitmapSize.Width, BitmapSize.Height)
        Dim ResualtGraphics As Graphics = Graphics.FromImage(ResualtBitmap)
        ResualtGraphics.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
        ResualtGraphics.TextRenderingHint = Drawing.Text.TextRenderingHint.ClearTypeGridFit

        '绘制阴影
        ResualtGraphics.DrawString(DrawText, TextFont, New SolidBrush(ShadowColor), New PointF(ShadowRadius, ShadowRadius))
        Dim ResualtBitmapData As Imaging.BitmapData = New Imaging.BitmapData
        ResualtBitmapData = ResualtBitmap.LockBits(New Rectangle(0, 0, BitmapSize.Width, BitmapSize.Height), Imaging.ImageLockMode.WriteOnly, ResualtBitmap.PixelFormat)
        Dim DataStride As Integer = ResualtBitmapData.Stride
        Dim DataWidth As Integer = ResualtBitmapData.Width
        Dim DataHeight As Integer = ResualtBitmapData.Height
        Dim InitalDataArray(DataStride * DataHeight - 1) As Byte
        Dim DataArray(DataStride * DataHeight - 1) As Byte
        Dim Position(DoubleRadius, DoubleRadius) As Integer
        Runtime.InteropServices.Marshal.Copy(ResualtBitmapData.Scan0, InitalDataArray, 0, InitalDataArray.Length)
        Dim Index = 0, IndexX, IndexY As Integer, Round, RoundX, RoundY As Integer
        Dim ByteSum, AValue As Integer
        Dim Boundary(DataHeight) As Integer, LineIndex As Integer
        For RoundY = 0 To DoubleRadius
            Index = (RoundY - ShadowRadius) * DataWidth - ShadowRadius
            For RoundX = 0 To DoubleRadius
                Position(RoundY, RoundX) = IIf((RoundX - ShadowRadius) ^ 2 + (RoundY - ShadowRadius) ^ 2 <= ShadowRadius ^ 2, 4 * (Index + RoundX), 0)
            Next
        Next
        For IndexY = 0 To DataHeight - 1
            Boundary(IndexY + 1) = Boundary(IndexY) + DataStride
        Next
        For IndexY = 0 To DataHeight - 1
            For IndexX = 0 To DataWidth - 1
                ByteSum = 0 : AValue = 0
                Index = IndexY * DataStride + IndexX * 4
                For RoundY = 0 To DoubleRadius
                    LineIndex = IndexY + RoundY - ShadowRadius
                    If 0 <= LineIndex AndAlso LineIndex < Boundary.Count - 1 Then
                        For RoundX = 0 To DoubleRadius
                            Round = Index + Position(RoundY, RoundX)
                            If Boundary(LineIndex) <= Round AndAlso Round < Boundary(LineIndex + 1) Then
                                AValue += InitalDataArray(Round + 3)
                                ByteSum += 1
                            End If
                        Next
                    End If
                Next
                AValue /= ByteSum
                DataArray(Index) = ShadowColorCell(2)
                DataArray(Index + 1) = ShadowColorCell(1)
                DataArray(Index + 2) = ShadowColorCell(0)
                DataArray(Index + 3) = AValue
            Next
        Next
        Runtime.InteropServices.Marshal.Copy(DataArray, 0, ResualtBitmapData.Scan0, DataArray.Length)
        ResualtBitmap.UnlockBits(ResualtBitmapData)
        '文字描边
        Dim DrawBrush As Brush = New System.Drawing.Drawing2D.LinearGradientBrush(New Point(0, 0), New Point(0, 1), StrokeColor, StrokeColor)
        ResualtGraphics.DrawString(DrawText, TextFont, DrawBrush, New Point(ShadowRadius - 1, ShadowRadius + 1))
        ResualtGraphics.DrawString(DrawText, TextFont, DrawBrush, New Point(ShadowRadius - 1, ShadowRadius - 1))
        ResualtGraphics.DrawString(DrawText, TextFont, DrawBrush, New Point(ShadowRadius + 1, ShadowRadius + 1))
        ResualtGraphics.DrawString(DrawText, TextFont, DrawBrush, New Point(ShadowRadius + 1, ShadowRadius - 1))
        ResualtGraphics.DrawString(DrawText, TextFont, DrawBrush, New Point(ShadowRadius + 1, ShadowRadius))
        ResualtGraphics.DrawString(DrawText, TextFont, DrawBrush, New Point(ShadowRadius - 1, ShadowRadius))
        ResualtGraphics.DrawString(DrawText, TextFont, DrawBrush, New Point(ShadowRadius, ShadowRadius + 1))
        ResualtGraphics.DrawString(DrawText, TextFont, DrawBrush, New Point(ShadowRadius, ShadowRadius - 1))
        '绘制原文字
        ResualtGraphics.DrawString(DrawText, TextFont, New SolidBrush(ForeColor), New PointF(ShadowRadius, ShadowRadius))
        ResualtGraphics.Dispose()
        Return ResualtBitmap
    End Function

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If FontDialog1.ShowDialog = DialogResult.OK Then MyFont = FontDialog1.Font
    End Sub
End Class

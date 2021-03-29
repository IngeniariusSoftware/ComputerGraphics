namespace GraphicsEditor.VisualTools
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Geometry;
    using LineSegment = Geometry.LineSegment;

    public class RasterFigureTool : RasterTool
    {
        public RasterFigureTool(Image background, Image foreground, byte[] backgroundBuffer)
            : base(background, foreground, backgroundBuffer)
        {
        }

        public override void EndDrawing(Point currentPoint, Color color)
        {
            base.EndDrawing(currentPoint, color);
            ClearBuffer();
            DrawFigure(Background, currentPoint, color);
        }

        public override string ToString() =>
            $"∠:  {-new LineSegment(StartPoint, LastPoint).TiltAngle,6:F}°\nД:  {MathGeometry.Length(StartPoint, LastPoint),6:F} пикс.";

        protected override void Drawing(Point currentPoint, Color color)
        {
            ClearBuffer();
            DrawFigure(Foreground, currentPoint, color);
            base.Drawing(currentPoint, color);
        }

        protected virtual void DrawFigure(WriteableBitmap bitmap, Point currentPoint, Color color)
        {
        }

        protected void ClearBuffer()
        {
            var start = new Point(Math.Min(StartPoint.X, LastPoint.X), Math.Min(StartPoint.Y, LastPoint.Y));
            var end = new Point(Math.Max(StartPoint.X, LastPoint.X), Math.Max(StartPoint.Y, LastPoint.Y));
            var size = new Int32Rect(
                (int)start.X,
                (int)start.Y,
                (int)(end.X - start.X + 2),
                (int)(end.Y - start.Y + 2));
            Foreground.WritePixels(size, BackgroundBuffer, 4 * size.Width, 0);
        }
    }
}
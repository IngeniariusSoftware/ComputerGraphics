
namespace Lesson1.Tools
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    public class RasterLineTool : RasterTool
    {
        public RasterLineTool(Image background, Image foreground, byte[] backgroundBuffer)
            : base(background, foreground, backgroundBuffer)
        {
        }

        public override void EndDrawing(Point currentPoint, Color color)
        {
            base.EndDrawing(currentPoint, color);
            ClearBuffer();
            DrawLine(Background, currentPoint, color);
        }

        public override string ToString() => $"У:  {Mathp.Angle(StartPoint, LastPoint),6:F}°\nД:  {Mathp.Distance(StartPoint, LastPoint),6:F} пикс.";

        protected override void Drawing(Point currentPoint, Color color)
        {
            ClearBuffer();
            DrawLine(Foreground, currentPoint, color);
            base.Drawing(currentPoint, color);
        }

        protected virtual void DrawLine(WriteableBitmap bitmap, Point currentPoint, Color color)
        {
        }

        protected void ClearBuffer()
        {
            Point start = new Point(Math.Min(StartPoint.X, LastPoint.X), Math.Min(StartPoint.Y, LastPoint.Y));
            Point end = new Point(Math.Max(StartPoint.X, LastPoint.X), Math.Max(StartPoint.Y, LastPoint.Y));
            var rect = new Int32Rect(
                (int)start.X,
                (int)start.Y,
                (int)(end.X - start.X + 2),
                (int)(end.Y - start.Y + 2));
            Foreground.WritePixels(rect, BackgroundBuffer, 4 * rect.Width, 0);
        }
    }
}
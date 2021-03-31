namespace GraphicsEditor.VisualTools
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Geometry;
    using Extensions;

    // https://habr.com/ru/post/185086/
    public class BresenhamCircleTool : RasterFigureTool
    {
        public BresenhamCircleTool(WriteableBitmap background, WriteableBitmap foreground)
            : base(background, foreground)
        {
        }

        public override string ToString() =>
            $"Р:  {MathGeometry.Length(StartPoint, LastPoint),5:F} пикс.\nX: {StartPoint.X} Y: {StartPoint.Y}";

        protected override void DrawFigure(WriteableBitmap bitmap, Point currentPoint, Color color)
        {
            int radius = (int)Math.Round(MathGeometry.Length(StartPoint, currentPoint));
            int deltaX = (int)Math.Abs(currentPoint.X - StartPoint.X - 1);
            int deltaY = (int)Math.Abs(currentPoint.Y - StartPoint.Y - 1);
            int maxRadius = MathExtension.Min((int)StartPoint.X, (int)StartPoint.Y, deltaX, deltaY);
            radius = Math.Min(radius, maxRadius);
            if (radius < 1) return;
            BresenhamCircle(bitmap, color, StartPoint, radius);
        }

        private void BresenhamCircle(WriteableBitmap bitmap, Color color, Point center, int radius)
        {
            int centerX = (int)center.X;
            int centerY = (int)center.Y;
            int x = radius;
            int y = 0;
            int radiusError = 1 - x;
            while (x >= y)
            {
                WritePixel(bitmap, color, x + centerX, y + centerY);
                WritePixel(bitmap, color, y + centerX, x + centerY);
                WritePixel(bitmap, color, -x + centerX, y + centerY);
                WritePixel(bitmap, color, -y + centerX, x + centerY);
                WritePixel(bitmap, color, -x + centerX, -y + centerY);
                WritePixel(bitmap, color, -y + centerX, -x + centerY);
                WritePixel(bitmap, color, x + centerX, -y + centerY);
                WritePixel(bitmap, color, y + centerX, -x + centerY);

                y++;
                if (radiusError < 0)
                {
                    radiusError += (y * 2) + 1;
                }
                else
                {
                    x--;
                    radiusError += (y - x + 1) * 2;
                }
            }
        }
    }
}

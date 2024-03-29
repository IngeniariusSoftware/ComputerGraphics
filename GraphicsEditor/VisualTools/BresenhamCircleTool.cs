﻿namespace GraphicsEditor.VisualTools
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using Geometry;
    using Extensions;

    /// <summary>
    /// Алгоритм сглаживания Брезенхэма https://habr.com/ru/post/185086/
    /// </summary>
    public class BresenhamCircleTool : RasterFigureTool
    {
        public BresenhamCircleTool(IWriteableBitmap background, IWriteableBitmap foreground)
            : base(background, foreground)
        {
        }

        public override string ToString() =>
            $"Р:  {MathGeometry.Length(StartPoint, LastPoint),5:F} пикс.\nX: {StartPoint.X} Y: {StartPoint.Y}";

        protected override void DrawFigure(IWriteableBitmap bitmap, Point currentPoint, Color color)
        {
            int radius = (int)Math.Round(MathGeometry.Length(StartPoint, currentPoint));
            int deltaX = (int)Math.Abs(bitmap.PixelWidth - StartPoint.X) - 1;
            int deltaY = (int)Math.Abs(bitmap.PixelHeight - StartPoint.Y) - 1;
            int maxRadius = MathExtension.Min((int)StartPoint.X, (int)StartPoint.Y, deltaX, deltaY);
            radius = Math.Min(radius, maxRadius);
            if (radius < 1) return;
            BresenhamCircle(bitmap, color, StartPoint, radius);
        }

        private void BresenhamCircle(IWriteableBitmap bitmap, Color color, Point center, int radius)
        {
            int centerX = (int)center.X;
            int centerY = (int)center.Y;
            int x = radius;
            int y = 0;
            int radiusError = 1 - x;
            while (x >= y)
            {
                WritePixelColor(bitmap, color, x + centerX, y + centerY);
                WritePixelColor(bitmap, color, y + centerX, x + centerY);
                WritePixelColor(bitmap, color, -x + centerX, y + centerY);
                WritePixelColor(bitmap, color, -y + centerX, x + centerY);
                WritePixelColor(bitmap, color, -x + centerX, -y + centerY);
                WritePixelColor(bitmap, color, -y + centerX, -x + centerY);
                WritePixelColor(bitmap, color, x + centerX, -y + centerY);
                WritePixelColor(bitmap, color, y + centerX, -x + centerY);

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

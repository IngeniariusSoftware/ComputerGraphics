﻿namespace GraphicsEditor.VisualTools
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using Geometry;

    /// <summary>
    /// Алгоритм сглаживания Брезенхэма https://deru.qaz.wiki/wiki/Bresenham-Algorithmus
    /// </summary>
    public class BresenhamEllipseTool : RasterFigureTool
    {
        public BresenhamEllipseTool(IWriteableBitmap background, IWriteableBitmap foreground)
            : base(background, foreground)
        {
        }

        public override string ToString() =>
            $"Ш:  {Math.Abs(StartPoint.X - LastPoint.X),5:F} пикс.\nВ:  {Math.Abs(StartPoint.Y - LastPoint.Y),6:F} пикс.";

        protected override void DrawFigure(IWriteableBitmap bitmap, Point currentPoint, Color color)
        {
            int a = (int)Math.Abs(currentPoint.X - StartPoint.X) / 2;
            int b = (int)Math.Abs(currentPoint.Y - StartPoint.Y) / 2;
            if (a < 1 || b < 1) return;
            var center = new Point(Math.Min(StartPoint.X, currentPoint.X) + a,
                Math.Min(StartPoint.Y, currentPoint.Y) + b);
            BresenhamEllipse(bitmap, center, a, b, color);
        }

        private void ReflectWritePixels(IWriteableBitmap bitmap, int centerX, int centerY, int x, int y, Color color)
        {
            WritePixelColor(bitmap, color, centerX + x, centerY + y);
            WritePixelColor(bitmap, color, centerX + x, centerY - y);
            WritePixelColor(bitmap, color, centerX - x, centerY - y);
            WritePixelColor(bitmap, color, centerX - x, centerY + y);
        }

        private void BresenhamEllipse(IWriteableBitmap bitmap, Point center, int a, int b, Color color)
        {
            int centerX = (int)center.X;
            int centerY = (int)center.Y;
            int x = 0;
            int y = b;
            long a2 = a * a;
            long b2 = b * b;
            long error = b2 - (((b * 2) - 1) * a2);
            do
            {
                ReflectWritePixels(bitmap, centerX, centerY, x, y, color);
                long error2 = error * 2;
                if (error2 < ((x * 2) + 1) * b2)
                {
                    x++;
                    error += ((x * 2) + 1) * b2;
                }

                if (error2 > -((y * 2) - 1) * a2)
                {
                    y--;
                    error -= ((y * 2) - 1) * a2;
                }
            }
            while (y >= 0);

            while (x < a)
            {
                x++;
                WritePixelColor(bitmap, color, centerX + x, centerY);
                WritePixelColor(bitmap, color, centerX - x, centerY);
            }
        }
    }
}

﻿namespace GraphicsEditor.VisualTools
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    // https://deru.qaz.wiki/wiki/Bresenham-Algorithmus
    public class BresenhamEllipseTool : RasterFigureTool
    {
        public BresenhamEllipseTool(WriteableBitmap background, WriteableBitmap foreground)
            : base(background, foreground)
        {
        }

        protected override void DrawFigure(WriteableBitmap bitmap, Point currentPoint, Color color)
        {
            int a = (int)Math.Abs(currentPoint.X - StartPoint.X) / 2;
            int b = (int)Math.Abs(currentPoint.Y - StartPoint.Y) / 2;
            if (a < 1 || b < 1) return;
            var center = new Point(Math.Min(StartPoint.X, currentPoint.X) + a,
                Math.Min(StartPoint.Y, currentPoint.Y) + b);
            BresenhamEllipse(bitmap, center, a, b, color);
        }

        private void ReflectWritePixels(WriteableBitmap bitmap, int centerX, int centerY, int x, int y, Color color)
        {
            WritePixel(bitmap, color, centerX + x, centerY + y);
            WritePixel(bitmap, color, centerX + x, centerY - y);
            WritePixel(bitmap, color, centerX - x, centerY - y);
            WritePixel(bitmap, color, centerX - x, centerY + y);
        }

        private void BresenhamEllipse(WriteableBitmap bitmap, Point center, int a, int b, Color color)
        {
            int centerX = (int)center.X;
            int centerY = (int)center.Y;
            int x = 0;
            int y = b;
            long a2 = a * a;
            long b2 = b * b;
            long err = b2 - (((b * 2) - 1) * a2);
            do
            {
                ReflectWritePixels(bitmap, centerX, centerY, x, y, color);
                long e2 = err * 2;
                if (e2 < ((x * 2) + 1) * b2)
                {
                    x++;
                    err += ((x * 2) + 1) * b2;
                }

                if (e2 > -((y * 2) - 1) * a2)
                {
                    y--;
                    err -= ((y * 2) - 1) * a2;
                }
            }
            while (y >= 0);

            while (x < a)
            {
                x++;
                WritePixel(bitmap, color, centerX + x, centerY);
                WritePixel(bitmap, color, centerX + x, centerY);
            }
        }
    }
}
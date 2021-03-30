using Microsoft.Win32.SafeHandles;

namespace GraphicsEditor.VisualTools
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Geometry;

    public class BresenhamCircleTool : RasterFigureTool
    {
        public BresenhamCircleTool(Image background, Image foreground, byte[] backgroundBuffer)
            : base(background, foreground, backgroundBuffer)
        {
        }

        public override string ToString() =>
            $"Р:  {(int)MathGeometry.Length(StartPoint, LastPoint),5:F} пикс.\nX: {StartPoint.X} Y: {StartPoint.Y}";

        protected override void DrawFigure(WriteableBitmap bitmap, Point currentPoint, Color color)
        {
            int radius = (int)Math.Min((int)MathGeometry.Length(StartPoint, currentPoint),
                Math.Min(StartPoint.X, StartPoint.Y));

            if (radius <= 0) return;
            var points = new int[radius * 4];
            int x = 0;
            int y = radius;
            int maxY = (int)Math.Round(y / Math.Sqrt(2));
            int δ = (1 - radius) >> 1;
            int count = 0;
            while (y >= maxY)
            {
                points[count] = x;
                points[count + 1] = y;
                count += 2;

                switch (δ)
                {
                    case < 0:
                    {
                        x++;
                        if (((δ >> 1) + (y >> 1) - 1) <= 0)
                        {
                            y--;
                            δ += (x - y + 1) >> 1;
                        }
                        else
                        {
                            δ += (x >> 1) + 1;
                        }

                        continue;
                    }

                    case > 0:
                    {
                        y--;
                        if (((δ >> 1) - (x >> 1) - 1) <= 0)
                        {
                            δ -= (y >> 1) - 1;
                        }
                        else
                        {
                            x++;
                            δ += (x >> 1) + 1;
                        }

                        continue;
                    }

                    default:
                    {
                        x++;
                        y--;
                        δ += (x - y + 1) >> 1;
                        break;
                    }
                }
            }

            if (count < 2) return;
            if (count > 2)
            {
                points = CircleReflection(points, count);
            }

            byte[] colorData = { color.B, color.G, color.R, color.A };
            DrawCircle(points, bitmap, colorData);
        }

        protected int[] CircleReflection(int[] points, int count)
        {
            var circle = new int[count * 8];
            for (int i = 0; i < count; i++)
            {
                circle[i] = points[i];
            }

            var reflections = new (int x1, int y1, int x2, int y2)[] { (0, 1, 1, 0), (-1, 0, 0, 1), (1, 0, 0, -1) };
            foreach (var reflection in reflections)
            {
                for (int i = 0; i < count; i += 2)
                {
                    circle[count + i] = (circle[i] * reflection.x1) + (circle[i + 1] * reflection.y1);
                    circle[count + i + 1] = (circle[i] * reflection.x2) + (circle[i + 1] * reflection.y2);
                }

                count <<= 1;
            }

            return circle;
        }

        protected void DrawCircle(int[] points, WriteableBitmap bitmap, byte[] color)
        {
            int startX = (int)StartPoint.X;
            int startY = (int)StartPoint.Y;

            for (int i = 0; i < points.Length; i += 2)
            {
                DrawPoint(bitmap, startX + points[i], startY + points[i + 1], color);
            }
        }

        protected override void ClearBuffer()
        {
            int radius = (int)Math.Min((int)MathGeometry.Length(StartPoint, LastPoint),
                Math.Min(StartPoint.X, StartPoint.Y));
            var size = new Int32Rect(
                (int)StartPoint.X - radius,
                (int)StartPoint.Y - radius,
                (int)(LastPoint.X + radius),
                (int)(LastPoint.Y + radius));
            Foreground.WritePixels(size, BackgroundBuffer, 4 * size.Width, 0);
        }
    }
}

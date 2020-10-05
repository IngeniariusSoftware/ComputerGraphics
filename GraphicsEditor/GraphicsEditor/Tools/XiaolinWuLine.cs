﻿
namespace Lesson1.Tools
{
    using System;
    using System.Windows;
    using System.Windows.Controls;

    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    public class XiaolinWuLine : RasterLineTool
    {
        public XiaolinWuLine(Image background, Image foreground, byte[] backgroundBuffer)
            : base(background, foreground, backgroundBuffer)
        {
        }

        protected override void DrawLine(WriteableBitmap bitmap, Point currentPoint, Color color)
        {
            byte[] colorData = { color.B, color.G, color.R, color.A };
            Point startPoint = StartPoint;

            bool isVertical = Math.Abs(currentPoint.Y - startPoint.Y) > Math.Abs(currentPoint.X - startPoint.X);
            if (isVertical)
            {
                startPoint = Mathp.Swap(startPoint);
                currentPoint = Mathp.Swap(currentPoint);
            }

            if (startPoint.X > currentPoint.X)
            {
                (startPoint, currentPoint) = (currentPoint, startPoint);
            }

            DrawPoint(bitmap, (int)startPoint.X, (int)startPoint.Y, colorData, isVertical);
            DrawPoint(bitmap, (int)currentPoint.X, (int)currentPoint.Y, colorData, isVertical);

            double dx = currentPoint.X - startPoint.X;
            double dy = currentPoint.Y - startPoint.Y;
            double gradient = dy / dx;
            double y = startPoint.Y + gradient;

            for (int x = (int)startPoint.X + 1; x <= currentPoint.X - 1; x++)
            {
                DrawPoint(bitmap, x, (int)y, colorData, isVertical, 1 - (y - (int)y));
                DrawPoint(bitmap, x, (int)y + 1, colorData, isVertical, y - (int)y);
                y += gradient;
            }
        }
    }
}
﻿namespace GraphicsEditor.VisualTools
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Extensions;

    public class BresenhamLineTool : RasterFigureTool
    {
        public BresenhamLineTool(Image background, Image foreground, byte[] backgroundBuffer)
            : base(background, foreground, backgroundBuffer)
        {
        }

        protected override void DrawFigure(WriteableBitmap bitmap, Point currentPoint, Color color)
        {
            byte[] colorData = { color.B, color.G, color.R, color.A };
            Point startPoint = StartPoint;
            bool isVertical = Math.Abs(currentPoint.Y - startPoint.Y) > Math.Abs(currentPoint.X - startPoint.X);
            if (isVertical)
            {
                startPoint = startPoint.Swap();
                currentPoint = currentPoint.Swap();
            }

            if (startPoint.X > currentPoint.X) (startPoint, currentPoint) = (currentPoint, startPoint);
            int dx = (int)(currentPoint.X - startPoint.X);
            int dy = (int)Math.Abs(currentPoint.Y - startPoint.Y);
            int error = dx >> 1;
            int stepY = startPoint.Y < currentPoint.Y ? 1 : -1;
            int y = (int)startPoint.Y;
            for (int x = (int)startPoint.X; x <= currentPoint.X; x++)
            {
                DrawPoint(bitmap, x, y, colorData, isVertical);
                error -= dy;

                if (error >= 0) continue;
                y += stepY;
                error += dx;
            }
        }
    }
}

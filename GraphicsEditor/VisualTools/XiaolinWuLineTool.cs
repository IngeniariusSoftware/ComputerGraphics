namespace GraphicsEditor.VisualTools
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using Extensions;
    using Geometry;

    /// <summary>
    /// Алгоритм сглаживания линии У Сяолиня https://habr.com/ru/post/185086/
    /// </summary>
    public class XiaolinWuLineTool : RasterFigureTool
    {
        public XiaolinWuLineTool(IWriteableBitmap background, IWriteableBitmap foreground)
            : base(background, foreground)
        {
        }

        protected override void DrawFigure(IWriteableBitmap bitmap, Point currentPoint, Color color)
        {
            Point startPoint = StartPoint;
            bool isVertical = Math.Abs(currentPoint.Y - startPoint.Y) > Math.Abs(currentPoint.X - startPoint.X);
            if (isVertical)
            {
                startPoint = startPoint.Swap();
                currentPoint = currentPoint.Swap();
            }

            if (startPoint.X > currentPoint.X) (startPoint, currentPoint) = (currentPoint, startPoint);
            int x1 = (int)startPoint.X;
            int y1 = (int)startPoint.Y;
            int x2 = (int)currentPoint.X;
            int y2 = (int)currentPoint.Y;
            WritePixel(bitmap, color, x1, y1, isVertical);
            WritePixel(bitmap, color, x2, y2, isVertical);
            double dx = x2 - x1;
            double dy = y2 - y1;
            double gradient = dy / dx;
            double y = y1 + gradient;
            for (int x = x1 + 1; x <= x2 - 1; x++)
            {
                int intY = (int)y;
                color.A = (byte)(255 * (1 - (y - intY)));
                WritePixel(bitmap, color, x, intY, isVertical);
                color.A = (byte)(255 * (y - intY));
                WritePixel(bitmap, color, x, intY + 1, isVertical);
                y += gradient;
            }
        }
    }
}

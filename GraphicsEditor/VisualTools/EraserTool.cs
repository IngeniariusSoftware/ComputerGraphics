﻿namespace GraphicsEditor.VisualTools
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using Geometry;

    public class EraserTool : BaseTool
    {
        public EraserTool(Panel panel, IWriteableBitmap background, IWriteableBitmap foreground, byte[] backgroundBuffer)
        {
            Panel = panel;
            Background = background;
            Foreground = foreground;
            BackgroundBuffer = backgroundBuffer;
        }

        protected Panel Panel { get; }

        protected IWriteableBitmap Background { get; }

        protected IWriteableBitmap Foreground { get; }

        protected byte[] BackgroundBuffer { get; }

        public override void EndDrawing(Point currentPoint, Color color)
        {
            base.EndDrawing(currentPoint, color);
            var size = new Int32Rect(0, 0, Background.PixelWidth, Background.PixelHeight);
            Background.WritePixels(size, BackgroundBuffer, 4 * Background.PixelWidth, 0);
            size = new Int32Rect(0, 0, Foreground.PixelWidth, Foreground.PixelHeight);
            Foreground.WritePixels(size, BackgroundBuffer, 4 * Foreground.PixelWidth, 0);
            Panel.Children.Clear();
        }
    }
}

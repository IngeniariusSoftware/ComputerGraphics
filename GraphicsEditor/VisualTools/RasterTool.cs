﻿namespace GraphicsEditor.VisualTools
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    public class RasterTool : BaseTool
    {
        public RasterTool(WriteableBitmap background, WriteableBitmap foreground)
        {
            Background = background;
            Foreground = foreground;
        }

        protected WriteableBitmap Background { get; }

        protected WriteableBitmap Foreground { get; }

        protected bool IsEraseMode { get; set; }

        protected void WritePixel(WriteableBitmap bitmap, Color color, int x, int y, bool isVertical = false)
        {
            if (isVertical) (x, y) = (y, x);
            if (IsEraseMode)
            {
                ErasePixel(bitmap, y, x);
                return;
            }

            bitmap.Lock();
            unsafe
            {
                IntPtr pBackBuffer = bitmap.BackBuffer;
                pBackBuffer += y * bitmap.BackBufferStride;
                pBackBuffer += x * 4;
                int colorData = color.A << 24;
                colorData |= color.R << 16;
                colorData |= color.G << 8;
                colorData |= color.B << 0;
                *((int*)pBackBuffer) = colorData;
            }

            bitmap.AddDirtyRect(new Int32Rect(x, y, 1, 1));
            bitmap.Unlock();
        }

        protected void ErasePixel(WriteableBitmap bitmap, int y, int x)
        {
            bitmap.Lock();
            unsafe
            {
                IntPtr pBackBuffer = bitmap.BackBuffer;
                pBackBuffer += y * bitmap.BackBufferStride;
                pBackBuffer += x * 4;
                *((int*)pBackBuffer) = 0;
            }

            bitmap.AddDirtyRect(new Int32Rect(x, y, 1, 1));
            bitmap.Unlock();
        }
    }
}

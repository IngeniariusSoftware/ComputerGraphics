namespace GraphicsEditor.VisualTools
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
                // Get a pointer to the back buffer.
                IntPtr pBackBuffer = bitmap.BackBuffer;

                // Find the address of the pixel to draw.
                pBackBuffer += y * bitmap.BackBufferStride;
                pBackBuffer += x * 4;

                // Compute the pixel's color.
                int colorData = color.A << 24;
                colorData |= color.R << 16; // R
                colorData |= color.G << 8; // G
                colorData |= color.B << 0; // B

                // Assign the color data to the pixel.
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
                // Get a pointer to the back buffer.
                IntPtr pBackBuffer = bitmap.BackBuffer;

                // Find the address of the pixel to draw.
                pBackBuffer += y * bitmap.BackBufferStride;
                pBackBuffer += x * 4;

                // Assign the color data to the pixel.
                *((int*)pBackBuffer) = 0;
            }

            bitmap.AddDirtyRect(new Int32Rect(x, y, 1, 1));
            bitmap.Unlock();
        }
    }
}

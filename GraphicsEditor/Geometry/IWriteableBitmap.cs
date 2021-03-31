namespace GraphicsEditor.Geometry
{
    using System;
    using System.Windows;

    public interface IWriteableBitmap
    {
        int PixelWidth { get; set; }

        int PixelHeight { get; set; }

        IntPtr BackBuffer { get; }

        int BackBufferStride { get; }

        void Lock();

        void Unlock();

        void AddDirtyRect(Int32Rect rect);

        void WritePixels(Int32Rect sourceRect, Array pixels, int stride, int offset);
    }
}

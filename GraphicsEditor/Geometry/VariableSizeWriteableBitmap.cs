namespace GraphicsEditor.Geometry
{
    using System;
    using System.Windows;
    using System.Windows.Media.Imaging;

    public class VariableSizeWriteableBitmap : IWriteableBitmap
    {
        private int _pixelWidth;

        private int _pixelHeight;

        private WriteableBitmap _bitmap;

        public VariableSizeWriteableBitmap()
        {
        }

        public VariableSizeWriteableBitmap(WriteableBitmap bitmap)
        {
            Bitmap = bitmap;
        }

        public VariableSizeWriteableBitmap(WriteableBitmap bitmap, int pixelWidth, int pixelHeight)
        {
            Bitmap = bitmap;
            PixelHeight = pixelHeight;
            PixelWidth = pixelWidth;
        }

        public WriteableBitmap Bitmap
        {
            get => _bitmap;

            set
            {
                _bitmap = value;
                _pixelWidth = Math.Clamp(_pixelWidth, 0, Bitmap.PixelWidth);
                _pixelHeight = Math.Clamp(_pixelHeight, 0, Bitmap.PixelHeight);
            }
        }

        public int PixelWidth
        {
            get => Math.Min(Bitmap?.PixelWidth ?? _pixelWidth, _pixelWidth);

            set
            {
                if (value < 0 || value > Bitmap?.PixelWidth) throw new Exception("Выход за пределы размера Bitmap");
                _pixelWidth = value;
            }
        }

        public int PixelHeight
        {
            get => Math.Min(Bitmap?.PixelHeight ?? _pixelHeight, _pixelHeight);

            set
            {
                if (value < 0 || value > Bitmap?.PixelHeight) throw new Exception("Выход за пределы размера Bitmap");
                _pixelHeight = value;
            }
        }

        public int MaxPixelWidth => Bitmap.PixelWidth;

        public int MaxPixelHeight => Bitmap.PixelHeight;

        public IntPtr BackBuffer => Bitmap.BackBuffer;

        public int BackBufferStride => Bitmap.BackBufferStride;

        public void AddDirtyRect(Int32Rect rect) => Bitmap.AddDirtyRect(rect);

        public void Lock() => Bitmap.Lock();

        public void Unlock() => Bitmap.Unlock();

        public void WritePixels(Int32Rect sourceRect, Array pixels, int stride, int offset) =>
            Bitmap.WritePixels(sourceRect, pixels, stride, offset);
    }
}

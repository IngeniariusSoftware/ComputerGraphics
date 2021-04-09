namespace GraphicsEditor.VisualTools
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using Geometry;

    public class ByLineFillTool : RasterTool
    {
        private readonly int[] _coordinates;

        private readonly byte[] _fillingPixels;

        private int _maxX;

        private int _maxY;

        private int _start;

        private int _end;

        public ByLineFillTool(IWriteableBitmap background, IWriteableBitmap foreground, int[] coordinatesBuffer)
            : base(background, foreground)
        {
            _coordinates = coordinatesBuffer;
            _fillingPixels = new byte[4 * background.MaxPixelWidth];
        }

        public override void StartDrawing(Point startPoint, Color color)
        {
            InitializeFillingPixels(color);
            int targetColor = ReadPixel(Background, (int)startPoint.X, (int)startPoint.Y);
            int fillColor = BitConverter.ToInt32(new[] { color.B, color.G, color.R, color.A }, 0);
            if (targetColor == fillColor) return;
            _maxX = Math.Min(Background.PixelWidth, Background.MaxPixelWidth - 1);
            _maxY = Math.Min(Background.PixelHeight, Background.MaxPixelHeight - 1);
            _start = 0;
            _end = 0;
            _coordinates[_end] = (int)startPoint.X;
            _coordinates[_end + 1] = (int)startPoint.Y;
            _end += 2;
            base.StartDrawing(startPoint, color);
            while (_start < _end)
            {
                int x = _coordinates[_start];
                int y = _coordinates[_start + 1];
                _start += 2;
                int pixel = ReadPixel(Background, x, y);

                if (pixel != targetColor) continue;
                AddVerticalNeighbors(x, y);
                int leftX = x;
                while (leftX > 0)
                {
                    leftX--;
                    if (ReadPixel(Background, leftX, y) == targetColor)
                    {
                        AddVerticalNeighbors(leftX, y);
                    }
                    else
                    {
                        leftX++;
                        break;
                    }
                }

                int rightX = x;
                while (rightX < _maxX)
                {
                    rightX++;
                    if (ReadPixel(Background, rightX, y) == targetColor)
                    {
                        AddVerticalNeighbors(rightX, y);
                    }
                    else
                    {
                        rightX--;
                        break;
                    }
                }

                int pixelCount = rightX - leftX + 1;
                Background.WritePixels(new Int32Rect(leftX, y, pixelCount, 1), _fillingPixels, pixelCount * 4, 0);
            }
        }

        private void InitializeFillingPixels(Color color)
        {
            for (int i = 0; i < _fillingPixels.Length; i += 4)
            {
                _fillingPixels[i] = color.B;
                _fillingPixels[i + 1] = color.G;
                _fillingPixels[i + 2] = color.R;
                _fillingPixels[i + 3] = color.A;
            }
        }

        private void AddVerticalNeighbors(int x, int y)
        {
            if (y > 0)
            {
                _coordinates[_end] = x;
                _coordinates[_end + 1] = y - 1;
                _end += 2;
            }

            if (y < _maxY)
            {
                _coordinates[_end] = x;
                _coordinates[_end + 1] = y + 1;
                _end += 2;
            }
        }
    }
}

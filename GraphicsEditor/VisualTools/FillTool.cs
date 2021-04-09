namespace GraphicsEditor.VisualTools
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using Geometry;

    public class FillTool : RasterTool
    {
        private readonly int[] _coordinates;

        private int _maxX;

        private int _maxY;

        private int _start;

        private int _end;

        public FillTool(IWriteableBitmap background, IWriteableBitmap foreground, int[] coordinatesBuffer)
            : base(background, foreground)
        {
            _coordinates = coordinatesBuffer;
        }

        public override void StartDrawing(Point startPoint, Color color)
        {
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
                WritePixel(Background, fillColor, x, y);
                if (x > 0) AddCoordinates(x - 1, y);
                if (x < _maxX) AddCoordinates(x + 1, y);
                if (y > 0) AddCoordinates(x, y - 1);
                if (y < _maxY) AddCoordinates(x, y + 1);
            }
        }

        private void AddCoordinates(int x, int y)
        {
            _coordinates[_end] = x;
            _coordinates[_end + 1] = y;
            _end += 2;
        }
    }
}

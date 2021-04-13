namespace GraphicsEditor.VisualTools
{
    using System.Windows;
    using System.Windows.Media;
    using Geometry;
    using LineSegment = Geometry.LineSegment;

    public abstract class RasterFigureTool : RasterTool
    {
        protected RasterFigureTool(IWriteableBitmap background, IWriteableBitmap foreground)
            : base(background, foreground)
        {
            EmptyColor = Color.FromArgb(0, 0, 0, 0);
        }

        public Color EmptyColor { get; }

        public override void EndDrawing(Point currentPoint, Color color)
        {
            base.EndDrawing(currentPoint, color);
            Clear();
            DrawFigure(Background, currentPoint, color);
        }

        public override string ToString() =>
            $"∠:  {-new LineSegment(StartPoint, LastPoint).TiltAngle,6:F}°\nД:  {MathGeometry.Length(StartPoint, LastPoint),6:F} пикс.";

        protected override void Drawing(Point currentPoint, Color color)
        {
            Clear();
            DrawFigure(Foreground, currentPoint, color);
            base.Drawing(currentPoint, color);
        }

        protected abstract void DrawFigure(IWriteableBitmap bitmap, Point currentPoint, Color color);

        protected virtual void Clear()
        {
            IsEraseMode = true;
            DrawFigure(Foreground, LastPoint, EmptyColor);
            IsEraseMode = false;
        }
    }
}

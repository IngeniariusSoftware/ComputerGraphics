namespace GraphicsEditor.VisualTools
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Geometry;

    public class BresenhamCircleTool : RasterFigureTool
    {
        public BresenhamCircleTool(Image background, Image foreground, byte[] backgroundBuffer)
            : base(background, foreground, backgroundBuffer)
        {
        }

        public override string ToString() =>
            $"Р:  {(int)MathGeometry.Length(StartPoint, LastPoint),5:F} пикс.\nX: {StartPoint.X} Y: {StartPoint.Y}";

        protected override void DrawFigure(WriteableBitmap bitmap, Point currentPoint, Color color)
        {
            int radius = (int)MathGeometry.Length(StartPoint, currentPoint);
            byte[] colorData = { color.B, color.G, color.R, color.A };
            int x = radius;
            int y = 0;
            int Δ = 2;
            int δ = 2;
            DrawPoint(bitmap, 0, 0, colorData);
        }
    }
}

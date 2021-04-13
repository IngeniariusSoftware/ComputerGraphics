namespace GraphicsEditor.VisualTools
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using Geometry;
    using UIElements;
    using LineSegment = Geometry.LineSegment;

    public class ShapeLineTool : ShapeTool
    {
        public ShapeLineTool(IPanel background, IPanel foreground)
            : base(background, foreground)
        {
        }

        public override string ToString() =>
            $"∠:  {-new LineSegment(StartPoint, LastPoint).TiltAngle,6:F}°\nД:  {MathGeometry.Length(StartPoint, LastPoint),6:F} пикс.";

        protected override void Drawing(Point currentPoint, Color color)
        {
            base.Drawing(currentPoint, color);
            if (Shape is not Line line) throw new Exception("Данный тип фигуры не поддерживается");
            line.X2 = currentPoint.X;
            line.Y2 = currentPoint.Y;
        }

        protected override Shape GenerateShape(Color color) => new Line
        {
            Y1 = StartPoint.Y,
            X1 = StartPoint.X,
            Y2 = StartPoint.Y,
            X2 = StartPoint.X,
            Stroke = new SolidColorBrush(color),
        };
    }
}

﻿namespace GraphicsEditor.VisualTools
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using Geometry;
    using LineSegment = Geometry.LineSegment;

    public class ShapeLineTool : ShapeTool
    {
        public ShapeLineTool(Panel panel)
            : base(panel)
        {
        }

        private Line Line { get; set; }

        public override string ToString() =>
            $"∠:  {-new LineSegment(StartPoint, LastPoint).TiltAngle,6:F}°\nД:  {MathGeometry.Length(StartPoint, LastPoint),6:F} пикс.";

        protected override void Drawing(Point currentPoint, Color color)
        {
            base.Drawing(currentPoint, color);
            Line.X2 = currentPoint.X;
            Line.Y2 = currentPoint.Y;
        }

        protected override Shape GenerateShape(Color color)
        {
            return Line = new Line
            {
                Y1 = StartPoint.Y,
                X1 = StartPoint.X,
                Y2 = StartPoint.Y,
                X2 = StartPoint.X,
                Stroke = new SolidColorBrush(color),
            };
        }
    }
}

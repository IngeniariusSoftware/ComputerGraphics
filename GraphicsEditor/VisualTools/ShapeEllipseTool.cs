namespace GraphicsEditor.VisualTools
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using Geometry;
    using UIElements;
    using LineSegment = Geometry.LineSegment;

    public class ShapeEllipseTool : ShapeTool
    {
        public ShapeEllipseTool(IPanel background, IPanel foreground)
            : base(background, foreground)
        {
        }

        private Ellipse Ellipse { get; set; }

        public override Point RotateToRoundAngle(LineSegment lineSegment) =>
            MathGeometry.CeilingToPeriod(lineSegment, 90, -45);

        public override string ToString() =>
            $"Ш:  {Math.Abs(StartPoint.X - LastPoint.X),5:F} пикс.\nВ:  {Math.Abs(StartPoint.Y - LastPoint.Y),6:F} пикс.";

        protected override void Drawing(Point currentPoint, Color color)
        {
            base.Drawing(currentPoint, color);
            Ellipse.Margin = new Thickness(
                Math.Min(StartPoint.X, currentPoint.X),
                Math.Min(StartPoint.Y, currentPoint.Y),
                0,
                0);
            Ellipse.Width = Math.Abs(StartPoint.X - currentPoint.X);
            Ellipse.Height = Math.Abs(StartPoint.Y - currentPoint.Y);
        }

        protected override Shape GenerateShape(Color color)
        {
            return Ellipse = new Ellipse
            {
                Margin = new Thickness(StartPoint.X, StartPoint.Y, 0, 0),
                Height = 0,
                Width = 0,
                Fill = new SolidColorBrush(Colors.Transparent),
                Stroke = new SolidColorBrush(color),
            };
        }
    }
}

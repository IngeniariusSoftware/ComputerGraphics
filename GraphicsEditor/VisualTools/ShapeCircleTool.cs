namespace GraphicsEditor.VisualTools
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using Geometry;

    public class ShapeCircleTool : ShapeTool
    {
        public ShapeCircleTool(Panel panel)
            : base(panel)
        {
        }

        private Ellipse Ellipse { get; set; }

        public override string ToString() =>
            $"Р:  {MathGeometry.Length(StartPoint, LastPoint),5:F} пикс.\nX: {StartPoint.X} Y: {StartPoint.Y}";

        protected override void Drawing(Point currentPoint, Color color)
        {
            base.Drawing(currentPoint, color);
            double length = Math.Min(MathGeometry.Length(StartPoint, currentPoint),
                Math.Min(StartPoint.X, StartPoint.Y));
            Ellipse.Width = length * 2;
            Ellipse.Height = length * 2;
            Ellipse.Margin = new Thickness(StartPoint.X - length, StartPoint.Y - length, 0, 0);
        }

        protected override Shape GenerateShape(Color color)
        {
            return Ellipse = new Ellipse
            {
                Margin = new Thickness(StartPoint.X, StartPoint.Y, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Height = 0,
                Width = 0,
                Stroke = new SolidColorBrush(color),
            };
        }
    }
}

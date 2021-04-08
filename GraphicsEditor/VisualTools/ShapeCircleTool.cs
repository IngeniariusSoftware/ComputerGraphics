namespace GraphicsEditor.VisualTools
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using Extensions;
    using Geometry;
    using UIElements;

    public class ShapeCircleTool : ShapeTool
    {
        public ShapeCircleTool(IPanel background, IPanel foreground)
            : base(background, foreground)
        {
        }

        private Ellipse Ellipse { get; set; }

        public override string ToString() =>
            $"Р:  {MathGeometry.Length(StartPoint, LastPoint),5:F} пикс.\nX: {StartPoint.X} Y: {StartPoint.Y}";

        protected override void Drawing(Point currentPoint, Color color)
        {
            base.Drawing(currentPoint, color);
            int radius = (int)Math.Round(MathGeometry.Length(StartPoint, currentPoint));
            int deltaX = (int)Math.Abs((int)Background.ActualWidth - StartPoint.X) - 1;
            int deltaY = (int)Math.Abs((int)Background.ActualHeight - StartPoint.Y) - 1;
            int maxRadius = MathExtension.Min((int)StartPoint.X, (int)StartPoint.Y, deltaX, deltaY);
            radius = Math.Min(radius, maxRadius);
            Ellipse.Width = radius * 2;
            Ellipse.Height = radius * 2;
            Ellipse.Margin = new Thickness(StartPoint.X - radius, StartPoint.Y - radius, 0, 0);
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

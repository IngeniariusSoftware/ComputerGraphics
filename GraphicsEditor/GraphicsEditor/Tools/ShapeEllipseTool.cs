
namespace Lesson1.Tools
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Shapes;

    public class ShapeEllipseTool : ShapeTool
    {
        private Ellipse _ellipse;

        public ShapeEllipseTool(Panel panel)
            : base(panel)
        {
        }

        public override int RoundAngle(Point currentPoint) =>
            (int)((90 * Math.Ceiling(Mathp.Angle(StartPoint, currentPoint) / 90)) - 45);


        public override string ToString() =>
            $"Ш:  {Math.Abs(StartPoint.X - LastPoint.X),5:F} пикс.\nВ:  {Math.Abs(StartPoint.Y - LastPoint.Y),6:F} пикс.";

        protected override void Drawing(Point currentPoint, Color color)
        {
            base.Drawing(currentPoint, color);
            _ellipse.Margin = new Thickness(
                Math.Min(StartPoint.X, currentPoint.X),
                Math.Min(StartPoint.Y, currentPoint.Y),
                0,
                0);
            _ellipse.Width = Math.Abs(StartPoint.X - currentPoint.X);
            _ellipse.Height = Math.Abs(StartPoint.Y - currentPoint.Y);
        }

        protected override Shape DrawingShape(Color color)
        {
            return _ellipse = new Ellipse
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


namespace Lesson1.Tools
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Shapes;

    public class ShapeLineTool : ShapeTool
    {
        private Line _line;

        public ShapeLineTool(Panel panel)
            : base(panel)
        {
        }

        public override string ToString() => $"∠:  {Mathp.Angle(StartPoint, LastPoint),6:F}°\nД:  {Mathp.Distance(StartPoint, LastPoint),6:F} пикс.";

        protected override void Drawing(Point currentPoint, Color color)
        {
            base.Drawing(currentPoint, color);
            _line.X2 = currentPoint.X;
            _line.Y2 = currentPoint.Y;
        }

        protected override Shape DrawingShape(Color color)
        {
            return _line = new Line
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

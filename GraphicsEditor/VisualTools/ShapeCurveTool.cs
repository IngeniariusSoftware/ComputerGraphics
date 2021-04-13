namespace GraphicsEditor.VisualTools
{
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using Geometry;
    using UIElements;

    public abstract class ShapeCurveTool : ShapeTool
    {
        protected ShapeCurveTool(IPanel background, IPanel foreground)
            : base(background, foreground)
        {
        }

        public ICurveAlgorithm Algorithm { get; protected set; }

        public Curve Curve { get; set; }

        public override void Open()
        {
            base.Open();
            Curve ??= new Curve(Background, Algorithm, 0.01);
        }

        public override void Close()
        {
            base.Close();
            Curve = null;
        }

        public override void EndDrawing(Point currentPoint, Color color)
        {
            Shape.Tag = Curve;
            base.EndDrawing(currentPoint, color);
        }

        protected override void Drawing(Point currentPoint, Color color)
        {
            base.Drawing(currentPoint, color);
            Shape.Margin = new Thickness(currentPoint.X, currentPoint.Y, 0, 0);
            Shape.DataContext = Shape.Margin;
        }

        protected override Shape GenerateShape(Color color)
        {
            var ellipse = new Ellipse
            {
                Margin = new Thickness(StartPoint.X, StartPoint.Y, 0, 0),
                Height = Curve.Size.Height,
                Width = Curve.Size.Width,
                Fill = new RadialGradientBrush(Colors.Red, Colors.White),
                Stroke = new SolidColorBrush(Colors.Red),
            };

            Curve.AddPoint(ellipse, color);
            return ellipse;
        }
    }
}

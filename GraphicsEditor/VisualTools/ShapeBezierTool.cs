namespace GraphicsEditor.VisualTools
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using UIElements;

    public class ShapeBezierTool : ShapeTool
    {
        public ShapeBezierTool(IPanel background, IPanel foreground)
            : base(background, foreground)
        {
            Points = new List<Ellipse>();
        }

        protected List<Ellipse> Points { get; }

        protected override void Drawing(Point currentPoint, Color color)
        {
            base.Drawing(currentPoint, color);
            //Ellipse.Margin = new Thickness(
            //    Math.Min(StartPoint.X, currentPoint.X),
            //    Math.Min(StartPoint.Y, currentPoint.Y),
            //    0,
            //    0);
            //Ellipse.Width = Math.Abs(StartPoint.X - currentPoint.X);
            //Ellipse.Height = Math.Abs(StartPoint.Y - currentPoint.Y);
        }

        protected override Shape GenerateShape(Color color)
        {
            //return Ellipse = new Ellipse
            //{
            //    Margin = new Thickness(StartPoint.X, StartPoint.Y, 0, 0),
            //    HorizontalAlignment = HorizontalAlignment.Left,
            //    VerticalAlignment = VerticalAlignment.Top,
            //    Height = 0,
            //    Width = 0,
            //    Stroke = new SolidColorBrush(color),
            //};
            return null;
        }
    }
}

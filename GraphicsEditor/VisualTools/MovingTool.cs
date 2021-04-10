namespace GraphicsEditor.VisualTools
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Shapes;

    public class MovingTool : BaseTool
    {
        public MovingTool(Panel panel)
        {
            Panel = panel;
            Panel.PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
        }

        protected Panel Panel { get; }

        private Shape MovingShape { get; set; }

        private Vector Delta { get; set; }

        public override string ToString() =>
            $"→:  {LastPoint.X - StartPoint.X} пикс.\n↓:  {LastPoint.Y - StartPoint.Y} пикс.";

        public override void EndDrawing(Point currentPoint, Color color)
        {
            base.EndDrawing(currentPoint, color);
            if (MovingShape == null) return;
            Mouse.Capture(null);
            MovingShape = null;
        }

        protected override void Drawing(Point currentPoint, Color color)
        {
            if (MovingShape == null) return;
            base.Drawing(currentPoint, color);
            double left = Math.Min(Math.Max(currentPoint.X - Delta.X, 0), Panel.ActualWidth - MovingShape.ActualWidth);
            double top = Math.Min(Math.Max(currentPoint.Y - Delta.Y, 0), Panel.ActualHeight - MovingShape.ActualHeight);
            MovingShape.Margin = new Thickness(left, top, 0, 0);
        }

        private void OnPreviewMouseLeftButtonDown(object sender, MouseEventArgs args)
        {
            if (args.Source is not Shape shape) return;
            MovingShape = shape;
            Mouse.Capture(MovingShape);
            Delta = (Vector)Mouse.GetPosition(MovingShape);
        }
    }
}

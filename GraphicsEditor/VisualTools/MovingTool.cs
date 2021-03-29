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
            MovingShape.ReleaseMouseCapture();
            MovingShape = null;
        }

        protected override void Drawing(Point currentPoint, Color color)
        {
            if (MovingShape == null) return;
            base.Drawing(currentPoint, color);
            double left = Math.Max(currentPoint.X - Delta.X, 0);
            double top = Math.Max(currentPoint.Y - Delta.Y, 0);
            MovingShape.Margin = new Thickness(left, top, 0, 0);
        }

        private void OnPreviewMouseLeftButtonDown(object sender, MouseEventArgs args)
        {
            if (args.Source is not Shape shape) return;
            MovingShape = shape;
            MovingShape.CaptureMouse();
            Delta = (Vector)Mouse.GetPosition(MovingShape);
        }
    }
}

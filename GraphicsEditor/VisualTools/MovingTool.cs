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

        private Point FirstLinePoint { get; set; }

        private Point SecondLinePoint { get; set; }

        public override string ToString() =>
            $"→:  {Math.Round(LastPoint.X - StartPoint.X)} пикс.\n↓:  {Math.Round(LastPoint.Y - StartPoint.Y)} пикс.";

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
            switch (MovingShape)
            {
                case Line line:
                {
                    double minX = -Math.Min(FirstLinePoint.X, SecondLinePoint.X);
                    double maxX = Panel.ActualWidth - Math.Max(FirstLinePoint.X, SecondLinePoint.X);
                    double minY = -Math.Min(FirstLinePoint.Y, SecondLinePoint.Y);
                    double maxY = Panel.ActualHeight - Math.Max(FirstLinePoint.Y, SecondLinePoint.Y);
                    double deltaX = Math.Clamp(currentPoint.X - Delta.X, minX, maxX);
                    double deltaY = Math.Clamp(currentPoint.Y - Delta.Y, minY, maxY);
                    line.X1 = FirstLinePoint.X + deltaX;
                    line.Y1 = FirstLinePoint.Y + deltaY;
                    line.X2 = SecondLinePoint.X + deltaX;
                    line.Y2 = SecondLinePoint.Y + deltaY;
                    break;
                }

                default:
                {
                    double left = Math.Clamp(currentPoint.X - Delta.X, 0, Panel.ActualWidth - MovingShape.ActualWidth);
                    double top = Math.Clamp(currentPoint.Y - Delta.Y, 0, Panel.ActualHeight - MovingShape.ActualHeight);
                    MovingShape.Margin = new Thickness(left, top, 0, 0);
                    MovingShape.DataContext = MovingShape.Margin;
                    break;
                }
            }
        }

        private void OnPreviewMouseLeftButtonDown(object sender, MouseEventArgs args)
        {
            if (args.Source is not Shape shape || !IsOpen)
            {
                MovingShape = null;
                return;
            }

            MovingShape = shape;
            Mouse.Capture(MovingShape);
            Delta = (Vector)Mouse.GetPosition(MovingShape);

            if (MovingShape is not Line line) return;
            FirstLinePoint = new Point(line.X1, line.Y1);
            SecondLinePoint = new Point(line.X2, line.Y2);
        }
    }
}

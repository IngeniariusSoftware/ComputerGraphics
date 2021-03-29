namespace GraphicsEditor.Controllers
{
    using System;
    using System.Windows;
    using Geometry;
    using Views;
    using VisualTools;
    using LineSegment = Geometry.LineSegment;

    public class MainController
    {
        public MainController(MainView mainView)
        {
            View = mainView;
            View.ToolSelected += OnToolSelected;
            View.StartDrawing += OnStartDrawing;
            View.Drawing += OnDrawing;
            View.EndDrawing += OnEndDrawing;
            View.AlternativeModeSwitched += OnAlternativeModeSwitched;
            View.DrawAreaSizeChanged += OnDrawAreaSizeChanged;
            EmptyTool = new BaseTool();
            CurrentTool = EmptyTool;
            DrawArea = new Rectangle(new Point(0, 0), new Point(0, 0));
        }

        private BaseTool EmptyTool { get; }

        private BaseTool CurrentTool { get; set; }

        private MainView View { get; }

        private Rectangle DrawArea { get; set; }

        private bool IsAlternativeMode { get; set; }

        private void OnAlternativeModeSwitched(object sender, bool value) => IsAlternativeMode = value;

        private void OnStartDrawing(object sender, Point point) => CurrentTool.StartDrawing(point, View.CurrentColor);

        private void OnDrawAreaSizeChanged(object sender, Size size) =>
            DrawArea = DrawArea with { End = new Point(size.Width, size.Height) };

        private void OnDrawing(object sender, Point point)
        {
            if (!CurrentTool.IsDrawing) return;
            point = TransformPoint(new LineSegment(CurrentTool.StartPoint, point), DrawArea);
            CurrentTool.TryDrawing(point, View.CurrentColor);
            View.ShowDrawingInformation(CurrentTool.ToString());
        }

        private void OnEndDrawing(object sender, Point point)
        {
            if (!CurrentTool.IsDrawing) return;
            point = TransformPoint(new LineSegment(CurrentTool.StartPoint, point), DrawArea);
            CurrentTool.EndDrawing(point, View.CurrentColor);
        }

        private Point TransformPoint(LineSegment lineSegment, Rectangle rectangle)
        {
            lineSegment = MathGeometry.LineSegmentClamp(lineSegment, rectangle);
            if (lineSegment is null) throw new Exception("Начало точки находится за пределами окна");
            Point result = lineSegment.End;
            if (IsAlternativeMode) result = CurrentTool.RotateToRoundAngle(lineSegment);
            return result;
        }

        private void OnToolSelected(object sender, BaseTool tool)
        {
            if (CurrentTool == tool)
            {
                CurrentTool.Close();
                CurrentTool = EmptyTool;
            }
            else
            {
                CurrentTool = tool;
                CurrentTool.Open();
            }
        }
    }
}

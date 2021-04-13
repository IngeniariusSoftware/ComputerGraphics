namespace GraphicsEditor.VisualTools
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using Geometry;
    using LineSegment = Geometry.LineSegment;

    public class BaseTool
    {
        public Point StartPoint { get; private set; }

        public bool IsDrawing { get; private set; }

        protected bool IsOpen { get; private set; }

        protected Point LastPoint { get; private set; }

        public void TryDrawing(Point currentPoint, Color color)
        {
            if (IsDrawing) Drawing(currentPoint, color);
        }

        public virtual void StartDrawing(Point startPoint, Color color)
        {
            StartPoint = startPoint;
            LastPoint = startPoint;
            IsDrawing = true;
        }

        public virtual void EndDrawing(Point currentPoint, Color color) => IsDrawing = false;

        public virtual Point RotateToRoundAngle(LineSegment lineSegment) =>
            MathGeometry.RoundToPeriod(lineSegment, 45);

        public virtual void Open() => IsOpen = true;

        public virtual void Close() => IsOpen = false;

        public override string ToString() => string.Empty;

        protected virtual void Drawing(Point currentPoint, Color color)
        {
            if (!IsDrawing) throw new Exception("Режим рисования выключен");
            LastPoint = currentPoint;
        }
    }
}
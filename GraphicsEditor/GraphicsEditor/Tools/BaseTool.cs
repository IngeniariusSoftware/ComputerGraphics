
namespace Lesson1.Tools
{
    using System;
    using System.Windows;
    using System.Windows.Media;

    public class BaseTool
    {
        private bool _isDrawing;

        private Point _startPoint;

        private Point _lastPoint;

        public Point StartPoint => _startPoint;

        protected bool IsDrawing => _isDrawing;

        protected Point LastPoint => _lastPoint;

        public void TryDrawing(Point currentPoint, Color color)
        {
            if (IsDrawing)
            {
                Drawing(currentPoint, color);
            }
        }

        public virtual int RoundAngle(Point currentPoint) =>
            (int)(45 * Math.Round(Mathp.Angle(StartPoint, currentPoint) / 45));

        public virtual void StartDrawing(Point startPoint, Color color)
        {
            if (IsDrawing)
            {
                throw new Exception("Рисование уже включено");
            }

            _startPoint = startPoint;
            _lastPoint = startPoint;
            _isDrawing = true;
        }

        public virtual void EndDrawing(Point currentPoint, Color color)
        {
            _isDrawing = false;
        }

        public override string ToString() => string.Empty;

        protected virtual void Drawing(Point currentPoint, Color color)
        {
            if (IsDrawing)
            {
                _lastPoint = currentPoint;
            }
            else
            {
                throw new Exception("Режим рисования выключен");
            }
        }
    }
}

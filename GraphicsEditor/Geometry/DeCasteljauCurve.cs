namespace GraphicsEditor.Geometry
{
    using System.Collections.Generic;
    using System.Windows;

    public class DeCasteljauCurve : ICurveAlgorithm
    {
        private Point[] _points;

        public DeCasteljauCurve()
        {
            _points = new Point[8];
        }

        public Point GetPoint(double t, List<Point> points)
        {
            if (points.Count > _points.Length) _points = new Point[points.Capacity];
            points.CopyTo(_points);

            for (int count = points.Count - 1; count > 0; count--)
            {
                for (int i = 0; i < count; i++)
                {
                    _points[i] = _points[i] + ((_points[i + 1] - _points[i]) * t);
                }
            }

            return _points[0];
        }
    }
}

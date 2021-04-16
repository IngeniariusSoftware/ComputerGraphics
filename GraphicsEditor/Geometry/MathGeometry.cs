#nullable enable
namespace GraphicsEditor.Geometry
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using Extensions;
    using static System.Math;

    public static class MathGeometry
    {
        public static LineSegment? LineSegmentClamp(LineSegment lineSegment, Rectangle rectangle)
        {
            int startCode = VisibilityPointCode(lineSegment.Start, rectangle);
            int endCode = VisibilityPointCode(lineSegment.End, rectangle);
            if (startCode == 0 && endCode == 0) return lineSegment;

            int lineCode = startCode & endCode;
            if (lineCode > 0) return null;

            List<Point> points = IntersectionPoints(lineSegment, rectangle);
            points = points.Where(rectangle.Contains).ToList();
            if (points.Count == 0) return null;
            Point start = startCode == 0 ? lineSegment.Start : NearestPoint(lineSegment.Start, points);
            Point end = endCode == 0 ? lineSegment.End : NearestPoint(lineSegment.End, points);
            return new LineSegment(start, end);
        }

        public static Point RoundToPeriod(LineSegment lineSegment, int period, int bias = 0)
        {
            int angle = RoundToPeriod(lineSegment.TiltAngle, period) + bias;
            return ToAngle(lineSegment, angle);
        }

        public static Point CeilingToPeriod(LineSegment lineSegment, int period, int bias = 0)
        {
            int angle = CeilingToPeriod(lineSegment.TiltAngle, period) + bias;
            return ToAngle(lineSegment, angle);
        }

        public static Point ToAngle(LineSegment lineSegment, double angle)
        {
            double rad = angle.ToRad();
            Vector diff = lineSegment.Diff.Abs();
            double len = angle % 90 == 0 ? Max(diff.X, diff.Y) : Min(diff.X, diff.Y);
            double sin = Sin(rad);
            double cos = Cos(rad);
            return lineSegment.Start + (Vector)(angle % 90 == 0
                ? new Point(len * cos, len * sin)
                : new Point(len * Sign(cos), len * Sign(sin)));
        }

        public static Point NearestPoint(LineSegment lineSegment, Point point)
        {
            if (double.IsInfinity(lineSegment.Slope)) return new Point(lineSegment.Start.X, point.Y);
            if (lineSegment.Slope == 0) return new Point(point.X, lineSegment.Start.Y);
            double slope = -1 / lineSegment.Slope;
            double interceptY = point.Y - (slope * point.X);
            double x = (interceptY - lineSegment.InterceptY) / (lineSegment.Slope - slope);
            double y = (x * slope) + interceptY;
            return new Point(x, y);
        }

        public static List<Point> IntersectionPoints(LineSegment lineSegment, Rectangle rectangle)
        {
            var points = new List<Point>();

            if (lineSegment.Slope == 0)
            {
                points.Add(new Point(rectangle.Start.X, lineSegment.Start.Y));
                points.Add(new Point(rectangle.End.X, lineSegment.Start.Y));
                return points;
            }

            if (double.IsInfinity(lineSegment.Slope))
            {
                points.Add(new Point(lineSegment.Start.X, rectangle.Start.Y));
                points.Add(new Point(lineSegment.Start.X, rectangle.End.Y));
                return points;
            }

            points.Add(new Point(
                (lineSegment.InterceptY - rectangle.Start.Y) / (0.0 - lineSegment.Slope),
                rectangle.Start.Y));
            points.Add(new Point(
                (lineSegment.InterceptY - rectangle.End.Y) / (0.0 - lineSegment.Slope),
                rectangle.End.Y));
            points.Add(new Point(rectangle.Start.X, (rectangle.Start.X * lineSegment.Slope) + lineSegment.InterceptY));
            points.Add(new Point(rectangle.End.X, (rectangle.End.X * lineSegment.Slope) + lineSegment.InterceptY));
            return points;
        }

        public static double Length(Point p1, Point p2) => (p1 - p2).Length;

        public static int RoundToPeriod(double angle, int period) => ((int)Round(angle / period)) * period;

        public static int CeilingToPeriod(double angle, int period) => ((int)Ceiling(angle / period)) * period;

        private static int VisibilityPointCode(Point point, Rectangle rectangle)
        {
            int code = 0;
            if (point.X < rectangle.Start.X) code |= 0b_0001;
            if (point.X > rectangle.End.X) code |= 0b_0010;
            if (point.Y < rectangle.Start.Y) code |= 0b_0100;
            if (point.Y > rectangle.End.Y) code |= 0b_1000;
            return code;
        }

        private static Point NearestPoint(Point point, IReadOnlyCollection<Point> neighbors)
        {
            Point result = neighbors.First();
            double minLength = (point - result).Length;
            foreach (var neighbor in neighbors)
            {
                double length = (point - neighbor).Length;
                if (length >= minLength) continue;
                minLength = length;
                result = neighbor;
            }

            return result;
        }
    }
}

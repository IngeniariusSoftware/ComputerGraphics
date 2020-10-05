
namespace Lesson1
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;

    public static class Mathp
    {
        public static Point DiagonalClamp(Point start, Point end, Point min, Point max)
        {
            if (min.X <= max.X && min.Y <= max.Y)
            {
                if (start.X.CheckRange(min.X, max.X) && start.Y.CheckRange(min.Y, max.Y))
                {
                    if (end.X.CheckRange(min.X, max.X) && end.Y.CheckRange(min.Y, max.Y))
                    {
                        return end;
                    }
                    else
                    {
                        var points = new List<Point>
                                         {
                                             IntersectionPoint(start, end, min, new Point(max.X, min.Y)),
                                             IntersectionPoint(start, end, min, new Point(min.X, max.Y)),
                                             IntersectionPoint(start, end, max, new Point(min.X, max.Y)),
                                             IntersectionPoint(start, end, max, new Point(max.X, min.Y))
                                         };

                        return points.Where(p => p.X.CheckRange(min.X, max.X) && p.Y.CheckRange(min.Y, max.Y))
                            .OrderBy(p => Distance(p, end)).First();
                    }
                }
                else
                {
                    throw new Exception("Точка находится за пределами указанной области");
                }
            }
            else
            {
                throw new Exception("Отрицательный размер области");
            }
        }

        public static Point ToAngle(Point start, Point end, int angleDeg)
        {
            if (angleDeg % 45 == 0)
            {
                double angleRad = angleDeg.ToRad();
                Point diff = Abs(Diff(start, end));
                double len = angleDeg % 90 == 0 ? Math.Max(diff.X, diff.Y) : Math.Min(diff.X, diff.Y);
                return Sum(
                    start,
                    angleDeg % 90 == 0
                        ? new Point(len * Math.Cos(angleRad), len * -Math.Sin(angleRad))
                        : new Point(len * Math.Sign(Math.Cos(angleRad)), len * Math.Sign(-Math.Sin(angleRad))));
            }

            throw new Exception("Данный угол не поддерживается");
        }

        public static Point IntersectionPoint(Point start1, Point end1, Point start2, Point end2)
        {
            Point d1 = Diff(start1, end1);
            Point d2 = Diff(start2, end2);
            double a1 = Tan(d1);
            double b1 = start1.Y - (a1 * start1.X);
            double a2 = Tan(d2);
            double b2 = start2.Y - (a2 * start2.X);

            if (!double.IsInfinity(a1) && !double.IsNaN(a1))
            {
                if (double.IsInfinity(a2))
                {
                    return new Point(end2.X, (a1 * end2.X) + b1);
                }

                if (Math.Abs(a2) == 0)
                {
                    return new Point((end2.Y - b1) / a1, end2.Y);
                }
            }

            if (double.IsInfinity(a1) && Math.Abs(a2) == 0)
            {
                return new Point(end1.X, end2.Y);
            }

            if (double.IsInfinity(a2) && Math.Abs(a1) == 0)
            {
                return new Point(end2.X, end1.Y);
            }

            double x = (b2 - b1) / (a1 - a2);
            double y = (a1 * x) + b1;
            return new Point(x, y);
        }

        public static double Distance(Point p1, Point p2) =>
            Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));

        public static double Angle(Point start, Point end) => -Math.Atan2(end.Y - start.Y, end.X - start.X).ToDeg();

        public static Point Swap(Point p) => new Point(p.Y, p.X);

        public static Point Diff(Point start, Point end) => new Point(end.X - start.X, end.Y - start.Y);

        public static Point Sum(Point p1, Point p2) => new Point(p1.X + p2.X, p1.Y + p2.Y);

        public static double Tan(Point p) => p.Y / p.X;

        public static Point Abs(Point p) => new Point(Math.Abs(p.X), Math.Abs(p.Y));
    }
}
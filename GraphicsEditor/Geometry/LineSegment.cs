namespace GraphicsEditor.Geometry
{
    using System;
    using System.Windows;
    using System.Windows.Shapes;
    using Extensions;

    public record LineSegment
    {
        public LineSegment(Point start, Point end)
        {
            Start = start;
            End = end;
            Length = (end - start).Length;
            Diff = End - Start;
            TiltAngle = Math.Atan2(Diff.Y, Diff.X).ToDeg();
            Slope = (end.Y - start.Y) / (end.X - start.X);
            InterceptY = double.IsInfinity(Slope) ? double.NaN : start.Y - (start.X * Slope);
        }

        public Point Start { get; init; }

        public Point End { get; init; }

        public Vector Diff { get; }

        public double Length { get; }

        public double TiltAngle { get; }

        public double Slope { get; }

        public double InterceptY { get; }

        public static implicit operator LineSegment(Line line) =>
            new LineSegment(new Point(line.X1, line.Y1), new Point(line.X2, line.Y2));
    }
}

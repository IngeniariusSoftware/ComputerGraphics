namespace GraphicsEditor.Geometry
{
    using System;
    using System.Windows;

    public record Rectangle
    {
        public Rectangle(Point start, Point end)
        {
            if (start.X > end.X || start.Y > end.Y) throw new Exception("Заданы неверные размеры прямоугольника");
            Start = start;
            End = end;
        }

        public Point Start { get; init; }

        public Point End { get; init; }

        public bool Contains(Point point)
        {
            if (point.X < Start.X || point.X > End.X) return false;
            if (point.Y < Start.Y || point.Y > End.Y) return false;
            return true;
        }
    }
}

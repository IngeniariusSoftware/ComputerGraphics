namespace GraphicsEditor.Extensions
{
    using System;
    using System.Linq;
    using System.Windows;

    public static class MathExtension
    {
        public static bool InRange<T>(this T val, T min, T max)
            where T : IComparable<T> => val.CompareTo(min) > -1 && val.CompareTo(max) < 1;

        public static double ToRad(this int angle) => ((double)angle).ToRad();

        public static double ToRad(this double angle) => angle / 180.0 * Math.PI;

        public static double ToDeg(this double angle) => angle * 180.0 / Math.PI;

        public static Vector Abs(this Point p) => new(Math.Abs(p.X), Math.Abs(p.Y));

        public static Vector Abs(this Vector p) => new(Math.Abs(p.X), Math.Abs(p.Y));

        public static Point Swap(this Point p) => new(p.Y, p.X);

        public static int Min(params int[] args) => args.Min();

        public static int Max(params int[] args) => args.Max();
    }
}

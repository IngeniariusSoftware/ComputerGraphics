
namespace Lesson1
{
    using System;

    public static class Extensions
    {
        public static bool CheckRange<T>(this T val, T min, T max)
            where T : IComparable<T>
        {
            return val.CompareTo(min) > -1 && val.CompareTo(max) < 1;
        }

        public static T Clamp<T>(this T val, T min, T max)
            where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0)
            {
                return min;
            }

            if (val.CompareTo(max) > 0)
            {
                return max;
            }

            return val;
        }

        public static double ToRad(this int angle) => ((double)angle).ToRad();

        public static double ToRad(this double angle) => angle / 180 * Math.PI;

        public static double ToDeg(this double angle) => angle * 180 / Math.PI;
    }
}

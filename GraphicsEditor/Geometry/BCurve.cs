namespace GraphicsEditor.Geometry
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using static System.Math;

    public class BCurve : ICurveAlgorithm
    {
        private readonly Dictionary<double, double[]> Coefficients;

        public BCurve()
        {
            Coefficients = new Dictionary<double, double[]>();
        }

        public Point GetPoint(double t, List<Point> points)
        {
            double x = 0;
            double y = 0;
            if (points.Count != 4) throw new Exception("Длина должна быть равна 4");
            var coefficients = GetCoefficients(t);
            for (int i = 0; i < coefficients.Length; i++)
            {
                x += coefficients[i] * points[i].X;
                y += coefficients[i] * points[i].Y;
            }

            return new Point(x, y);
        }

        public double[] GetCoefficients(double t)
        {
            if (Coefficients.ContainsKey(t)) return Coefficients[t];
            var coefficients = new double[4];
            double coefficient = 1.0 / 6.0;
            coefficients[0] = Pow(1 - t, 3) * coefficient;
            coefficients[1] = ((3 * Pow(t, 3)) - (6 * Pow(t, 2)) + 4) * coefficient;
            coefficients[2] = ((3 * (-Pow(t, 3) + Pow(t, 2) + t)) + 1) * coefficient;
            coefficients[3] = Pow(t, 3) * coefficient;
            Coefficients[t] = coefficients;
            return coefficients;
        }
    }
}

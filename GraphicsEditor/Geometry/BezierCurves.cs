namespace GraphicsEditor.Geometry
{
    using System.Collections.Generic;
    using System.Windows;
    using Algebra;

    public class BezierCurves : ICurveAlgorithm
    {
        private readonly Dictionary<decimal, Dictionary<int, decimal[]>> Coefficients;

        private readonly Dictionary<int, decimal[]> BezierCoefficients;

        public BezierCurves()
        {
            Coefficients = new Dictionary<decimal, Dictionary<int, decimal[]>>();
            BezierCoefficients = new Dictionary<int, decimal[]>();
        }

        public Point GetPoint(double t, List<Point> points)
        {
            decimal x = 0;
            decimal y = 0;
            int order = points.Count - 1;
            var coefficients = GetCoefficients(order, (decimal)t);
            int stride = 0;
            int j = 0;
            for (int i = 0; i < coefficients.Length; i++)
            {
                x += coefficients[i] * (decimal)points[stride].X;
                y += coefficients[i] * (decimal)points[stride].Y;
                j++;
                if (j <= order) continue;
                stride++;
                j = stride;
            }

            return new Point((double)x, (double)y);
        }

        public decimal[] GetCoefficients(int order, decimal t)
        {
            if (Coefficients.ContainsKey(t))
            {
                if (Coefficients[t].ContainsKey(order)) return Coefficients[t][order];

                decimal[] bezierCoefficients = GetBezierCoefficients(order);
                var coefficients = new decimal[bezierCoefficients.Length];
                var terms = new decimal[order + 1];
                terms[0] = 1;
                for (int i = 1; i < terms.Length; i++)
                {
                    terms[i] = terms[i - 1] * t;
                }

                int stride = 0;
                int j = 0;
                for (int i = 0; i < bezierCoefficients.Length; i++)
                {
                    coefficients[i] = bezierCoefficients[i] * terms[j];
                    j++;
                    if (j <= order) continue;
                    stride++;
                    j = stride;
                }

                Coefficients[t][order] = coefficients;
                return coefficients;
            }
            else
            {
                Coefficients[t] = new Dictionary<int, decimal[]>();
                return GetCoefficients(order, t);
            }
        }

        public decimal[] GetBezierCoefficients(int order)
        {
            if (BezierCoefficients.ContainsKey(order)) return BezierCoefficients[order];
            var coefficients = new decimal[(order + 1) * (order + 2) / 2];
            int stride = 0;
            double[] factors = PascalTriangle.GetCoefficients(order);
            for (int i = order; i >= 0; i--)
            {
                double[] binomialCoefficients = PascalTriangle.GetCoefficients(i);
                for (int j = 0; j <= i; j++)
                {
                    coefficients[stride + j] = (decimal)binomialCoefficients[j] *
                                               (decimal)(j % 2 == 0 ? factors[i] : -factors[i]);
                }

                stride += i + 1;
            }

            BezierCoefficients[order] = coefficients;
            return coefficients;
        }
    }
}

namespace GraphicsEditor.Algebra
{
    using System.Collections.Generic;

    public static class PascalTriangle
    {
        private static readonly List<double[]> Coefficients;

        static PascalTriangle()
        {
            Coefficients = new List<double[]> { new[] { 1.0 } };
        }

        public static double[] GetCoefficients(int rowPos)
        {
            if (rowPos < Coefficients.Count) return Coefficients[rowPos];
            double[] previousRow = GetCoefficients(rowPos - 1);
            var row = new double[rowPos + 1];
            row[0] = 1;
            row[^1] = 1;
            for (int i = 1; i < row.Length - 1; i++)
            {
                row[i] = previousRow[i - 1] + previousRow[i];
            }

            Coefficients.Add(row);
            return row;
        }
    }
}

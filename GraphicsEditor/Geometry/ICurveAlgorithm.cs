namespace GraphicsEditor.Geometry
{
    using System.Collections.Generic;
    using System.Windows;

    public interface ICurveAlgorithm
    {
        Point GetPoint(double t, List<Point> points);
    }
}

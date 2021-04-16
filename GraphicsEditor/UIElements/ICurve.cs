namespace GraphicsEditor.UIElements
{
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using Geometry;

    public interface ICurve
    {
        void AddPoint(Ellipse ellipse, Color color);

        ICurveAlgorithm Algorithm { get; }

        Size Size { get; }
    }
}

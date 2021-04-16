namespace GraphicsEditor.VisualTools
{
    using Geometry;
    using UIElements;

    public class ShapeCompoundBezierTool : ShapeCurveTool
    {
        public ShapeCompoundBezierTool(IPanel background, IPanel foreground, ICurveAlgorithm algorithm,
            double resolution)
            : base(background, foreground, algorithm, resolution)
        {
        }

        protected override ICurve GenerateCurve()
        {
            return new CompoundBezierCurve(Background, Algorithm, Resolution);
        }
    }
}
namespace GraphicsEditor.VisualTools
{
    using Geometry;
    using UIElements;

    public class ShapeCompoundBTool : ShapeCurveTool
    {
        public ShapeCompoundBTool(IPanel background, IPanel foreground, ICurveAlgorithm algorithm,
            double resolution)
            : base(background, foreground, algorithm, resolution)
        {
        }

        protected override ICurve GenerateCurve()
        {
            return new CompoundBCurve(Background, Algorithm, Resolution);
        }
    }
}
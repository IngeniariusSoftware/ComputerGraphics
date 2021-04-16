namespace GraphicsEditor.VisualTools
{
    using Geometry;
    using UIElements;

    public class ShapeElementaryDeCasteljauTool : ShapeCurveTool
    {
        public ShapeElementaryDeCasteljauTool(IPanel background, IPanel foreground, ICurveAlgorithm algorithm,
            double resolution)
            : base(background, foreground, algorithm, resolution)
        {
        }

        protected override ICurve GenerateCurve()
        {
            return new ElementaryCurve(Background, Algorithm, Resolution);
        }
    }
}

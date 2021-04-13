namespace GraphicsEditor.VisualTools
{
    using Geometry;
    using UIElements;

    public class ShapeElementaryDeCasteljau : ShapeCurveTool
    {
        public ShapeElementaryDeCasteljau(IPanel background, IPanel foreground)
            : base(background, foreground)
        {
            Algorithm = new DeCasteljauCurve();
        }
    }
}

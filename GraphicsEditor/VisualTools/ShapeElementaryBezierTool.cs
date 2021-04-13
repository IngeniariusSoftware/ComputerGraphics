namespace GraphicsEditor.VisualTools
{
    using Geometry;
    using UIElements;

    public class ShapeElementaryBezierTool : ShapeCurveTool
    {
        public ShapeElementaryBezierTool(IPanel background, IPanel foreground)
            : base(background, foreground)
        {
            Algorithm = new BezierCurves();
        }
    }
}

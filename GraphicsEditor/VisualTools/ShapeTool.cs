namespace GraphicsEditor.VisualTools
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Shapes;

    public class ShapeTool : BaseTool
    {
        public ShapeTool(Panel panel)
        {
            Panel = panel;
        }

        protected Panel Panel { get; }

        public override void StartDrawing(Point startPoint, Color color)
        {
            base.StartDrawing(startPoint, color);
            Panel.Children.Add(DrawingShape(color));
        }

        protected virtual Shape DrawingShape(Color color) => null;
    }
}

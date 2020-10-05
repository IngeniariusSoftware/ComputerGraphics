
namespace Lesson1.Tools
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Shapes;

    public class ShapeTool : BaseTool
    {
        protected readonly Panel Panel;

        public ShapeTool(Panel panel)
        {
            Panel = panel;
        }

        public override void StartDrawing(Point startPoint, Color color)
        {
            base.StartDrawing(startPoint, color);
            Panel.Children.Add(DrawingShape(color));
        }

        protected virtual Shape DrawingShape(Color color)
        {
            return null;
        }
    }
}

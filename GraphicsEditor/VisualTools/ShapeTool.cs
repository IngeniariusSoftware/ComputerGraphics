namespace GraphicsEditor.VisualTools
{
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using UIElements;

    public class ShapeTool : BaseTool
    {
        public ShapeTool(IPanel background, IPanel foreground)
        {
            Background = background;
            Foreground = foreground;
        }

        protected IPanel Background { get; }

        protected IPanel Foreground { get; }

        protected Shape Shape { get; private set; }

        public override void StartDrawing(Point startPoint, Color color)
        {
            base.StartDrawing(startPoint, color);
            Shape = GenerateShape(color);
            Foreground.Children.Add(Shape);
        }

        public override void EndDrawing(Point currentPoint, Color color)
        {
            base.EndDrawing(currentPoint, color);
            Foreground.Children.Remove(Shape);
            Background.Children.Add(Shape);
        }

        protected virtual Shape GenerateShape(Color color) => null;
    }
}

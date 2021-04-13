namespace GraphicsEditor.VisualTools
{
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using UIElements;

    public abstract class ShapeTool : BaseTool
    {
        protected ShapeTool(IPanel background, IPanel foreground)
        {
            Background = background;
            Foreground = foreground;
        }

        public Shape Shape { get; set; }

        protected IPanel Background { get; }

        protected IPanel Foreground { get; }

        public override void StartDrawing(Point startPoint, Color color)
        {
            base.StartDrawing(startPoint, color);
            if (Shape == null)
            {
                Shape = GenerateShape(color);
            }
            else
            {
                Background.Children.Remove(Shape);
            }

            Shape.Tag = this;
            Foreground.Children.Add(Shape);
        }

        public override void EndDrawing(Point currentPoint, Color color)
        {
            base.EndDrawing(currentPoint, color);
            Foreground.Children.Remove(Shape);
            Background.Children.Add(Shape);
            Shape = null;
        }

        protected abstract Shape GenerateShape(Color color);
    }
}

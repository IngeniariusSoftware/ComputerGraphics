namespace GraphicsEditor.UIElements
{
    using System.Windows;
    using System.Windows.Controls;

    public class ExtendedPanel : IPanel
    {
        public ExtendedPanel(Canvas canvas, IUIElementCollection children)
        {
            Canvas = canvas;
            Children = children;
        }

        public IUIElementCollection Children { get; }

        public double ActualWidth => Canvas.ActualWidth;

        public double ActualHeight => Canvas.ActualHeight;

        public Visibility Visibility
        {
            get => Canvas.Visibility;

            set => Canvas.Visibility = value;
        }

        private Canvas Canvas { get; }
    }
}

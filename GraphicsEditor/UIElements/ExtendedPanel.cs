namespace GraphicsEditor.UIElements
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    public class ExtendedPanel : IPanel
    {
        public ExtendedPanel(Canvas canvas, IUIElementCollection children)
        {
            Canvas = canvas;
            Children = children;
            Canvas.PreviewMouseLeftButtonDown += (sender, args) => PreviewMouseLeftButtonDown(sender, args);
            Canvas.PreviewMouseRightButtonDown += (sender, args) => PreviewMouseRightButtonDown(sender, args);
        }

        public event MouseButtonEventHandler PreviewMouseLeftButtonDown = delegate { };

        public event MouseButtonEventHandler PreviewMouseRightButtonDown = delegate { };

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

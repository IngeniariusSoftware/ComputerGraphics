namespace GraphicsEditor.Controllers
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    public class MovingController
    {
        public MovingController(FrameworkElement element)
        {
            Element = element;
            Parent = (Panel)Element.Parent;
            Element.MouseMove += OnMouseMove;
            Element.MouseLeftButtonDown += OnMouseLeftButtonDown;
            Element.MouseLeftButtonUp += OnMouseLeftButtonUp;
        }

        public event EventHandler<Thickness> MarginChanged = delegate { };

        private FrameworkElement Element { get; }

        private Panel Parent { get; }

        private bool IsMoveableMode { get; set; }

        private Point ElementRelativeParent { get; set; }

        private Point MouseRelativeParent { get; set; }

        private void OnMouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            if (e.Handled) return;
            ElementRelativeParent = new Point(Element.Margin.Left, Element.Margin.Top);
            MouseRelativeParent = Mouse.GetPosition(Parent);
            IsMoveableMode = true;
            Mouse.Capture(Element);
            e.Handled = true;
        }

        private void OnMouseMove(object sender, RoutedEventArgs e)
        {
            if (!IsMoveableMode || e.Handled) return;
            Point mouse = Mouse.GetPosition(Parent);
            int margin = 20;
            Element.Margin =
                new Thickness(
                    Math.Clamp(ElementRelativeParent.X - MouseRelativeParent.X + mouse.X, margin,
                        Parent.ActualWidth - Element.ActualWidth - margin),
                    Math.Clamp(ElementRelativeParent.Y - MouseRelativeParent.Y + mouse.Y, margin,
                        Parent.ActualHeight - Element.ActualHeight - margin), 0,
                    0);
            e.Handled = true;
            MarginChanged(Element, Element.Margin);
        }

        private void OnMouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
            IsMoveableMode = false;
            Mouse.Capture(null);
            e.Handled = true;
        }
    }
}

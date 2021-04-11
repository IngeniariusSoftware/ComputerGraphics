namespace GraphicsEditor.Controllers
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    public class MovingController
    {
        public MovingController(FrameworkElement element, int margin)
        {
            Element = element;
            Parent = (Panel)Element.Parent;
            Margin = margin;
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

        private int Margin { get; }

        private void OnMouseLeftButtonDown(object sender, RoutedEventArgs args)
        {
            if (args.Handled) return;
            ElementRelativeParent = new Point(Element.Margin.Left, Element.Margin.Top);
            MouseRelativeParent = Mouse.GetPosition(Parent);
            IsMoveableMode = true;
            Mouse.Capture(Element);
            args.Handled = true;
        }

        private void OnMouseMove(object sender, RoutedEventArgs args)
        {
            if (!IsMoveableMode || args.Handled) return;
            Point mouse = Mouse.GetPosition(Parent);
            double left = Math.Clamp(ElementRelativeParent.X - MouseRelativeParent.X + mouse.X, Margin,
                Parent.ActualWidth - Element.ActualWidth - Margin);
            double top = Math.Clamp(ElementRelativeParent.Y - MouseRelativeParent.Y + mouse.Y, Margin,
                Parent.ActualHeight - Element.ActualHeight - Margin);
            Element.Margin = new Thickness(left, top, 0, 0);
            args.Handled = true;
            MarginChanged(Element, Element.Margin);
        }

        private void OnMouseLeftButtonUp(object sender, RoutedEventArgs args)
        {
            IsMoveableMode = false;
            Mouse.Capture(null);
            args.Handled = true;
        }
    }
}

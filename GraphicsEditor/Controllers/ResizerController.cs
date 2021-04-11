namespace GraphicsEditor.Controllers
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    public class ResizerController
    {
        public ResizerController(FrameworkElement resizer, FrameworkElement resizeTarget)
        {
            ResizerElement = resizer;
            ResizeTarget = resizeTarget;
            Parent = (Panel)resizeTarget.Parent;
            ResizerElement.MouseEnter += OnMouseEnter;
            ResizerElement.MouseLeftButtonDown += OnMouseLeftButtonDown;
            ResizerElement.MouseLeftButtonUp += OnMouseLeftButtonUp;
            ResizerElement.MouseLeave += OnMouseLeave;
            ResizerElement.MouseMove += OnMouseMove;
        }

        public event EventHandler<Thickness> MarginChanged = delegate { };

        private bool IsResizeMode { get; set; }

        private FrameworkElement ResizerElement { get; }

        private FrameworkElement ResizeTarget { get; }

        private Panel Parent { get; }

        private void OnMouseEnter(object sender, MouseEventArgs args)
        {
            ResizeTarget.Cursor = Cursors.SizeNWSE;
        }

        private void OnMouseLeftButtonDown(object sender, MouseEventArgs args)
        {
            IsResizeMode = true;
            Mouse.Capture(ResizerElement);
            ResizeTarget.Cursor = Cursors.SizeNWSE;
            args.Handled = true;
        }

        private void OnMouseMove(object sender, MouseEventArgs args)
        {
            if (!IsResizeMode) return;
            Point mouse = Mouse.GetPosition(ResizeTarget);
            int margin = 20;
            ResizeTarget.Margin = new Thickness(ResizeTarget.Margin.Left, ResizeTarget.Margin.Top, 0, 0);
            ResizeTarget.Width = Math.Clamp(mouse.X, margin, Parent.ActualWidth - ResizeTarget.Margin.Left - margin);
            ResizeTarget.Height = Math.Clamp(mouse.Y, margin, Parent.ActualHeight - ResizeTarget.Margin.Top - margin);
            args.Handled = true;
            MarginChanged(ResizeTarget, ResizeTarget.Margin);
        }

        private void OnMouseLeftButtonUp(object sender, MouseEventArgs args)
        {
            IsResizeMode = false;
            Mouse.Capture(null);
            ResizeTarget.Cursor = Cursors.Arrow;
            args.Handled = true;
        }

        private void OnMouseLeave(object sender, MouseEventArgs args)
        {
            if (IsResizeMode) return;
            ResizeTarget.Cursor = Cursors.Arrow;
        }
    }
}

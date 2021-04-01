﻿namespace GraphicsEditor.Controllers
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

        private bool IsResizeMode { get; set; }

        private FrameworkElement ResizerElement { get; }

        private FrameworkElement ResizeTarget { get; }

        private Panel Parent { get; }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            ResizeTarget.Cursor = Cursors.SizeNWSE;
        }

        private void OnMouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            IsResizeMode = true;
            Mouse.Capture(ResizerElement);
            ResizeTarget.Cursor = Cursors.SizeNWSE;
            e.Handled = true;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!IsResizeMode) return;
            Point mouse = Mouse.GetPosition(ResizeTarget);
            int margin = 20;
            ResizeTarget.Margin = new Thickness(ResizeTarget.Margin.Left, ResizeTarget.Margin.Top, 0, 0);
            ResizeTarget.Width = Math.Clamp(mouse.X, margin, Parent.ActualWidth - ResizeTarget.Margin.Left - margin);
            ResizeTarget.Height = Math.Clamp(mouse.Y, margin, Parent.ActualHeight - ResizeTarget.Margin.Top - margin);
            e.Handled = true;
        }

        private void OnMouseLeftButtonUp(object sender, MouseEventArgs e)
        {
            IsResizeMode = false;
            Mouse.Capture(null);
            ResizeTarget.Cursor = Cursors.Arrow;
            e.Handled = true;
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            if (IsResizeMode) return;
            ResizeTarget.Cursor = Cursors.Arrow;
        }
    }
}

﻿namespace GraphicsEditor.VisualTools
{
    using System.Windows.Input;
    using System.Windows.Shapes;
    using UIElements;

    public class ShapeEraserTool : BaseTool
    {
        public ShapeEraserTool(IPanel panel)
        {
            Panel = panel;
            Panel.PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
            Panel.PreviewMouseRightButtonDown += OnPreviewMouseRightButtonDown;
        }

        protected IPanel Panel { get; }

        private void OnPreviewMouseLeftButtonDown(object sender, MouseEventArgs args)
        {
            if (args.Source is not Shape shape) return;
            if (!IsOpen) return;
            Panel.Children.Remove(shape);
        }

        private void OnPreviewMouseRightButtonDown(object sender, MouseEventArgs args)
        {
            if (args.Source is not Shape) return;
            if (!IsOpen) return;
            Panel.Children.Clear();
        }
    }
}

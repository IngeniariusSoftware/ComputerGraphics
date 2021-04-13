namespace GraphicsEditor.VisualTools
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

        private bool IsActive { get; set; }

        public override void Open()
        {
            base.Open();
            IsActive = true;
        }

        public override void Close()
        {
            base.Close();
            IsActive = false;
        }

        private void OnPreviewMouseLeftButtonDown(object sender, MouseEventArgs args)
        {
            if (args.Source is not Shape shape) return;
            if (!IsActive) return;
            Panel.Children.Remove(shape);
        }

        private void OnPreviewMouseRightButtonDown(object sender, MouseEventArgs args)
        {
            if (args.Source is not Shape) return;
            if (!IsActive) return;
            Panel.Children.Clear();
        }
    }
}

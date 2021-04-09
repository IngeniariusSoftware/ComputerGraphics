namespace GraphicsEditor.VisualTools
{
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Shapes;

    public class ShapeEraserTool : BaseTool
    {
        public ShapeEraserTool(Panel panel)
        {
            Panel = panel;
            Panel.PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
            Panel.PreviewMouseRightButtonDown += OnPreviewMouseRightButtonDown;
        }

        protected Panel Panel { get; }

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

        private void OnPreviewMouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            if (e.Source is not Shape shape) return;
            if (!IsActive) return;
            Panel.Children.Remove(shape);
        }

        private void OnPreviewMouseRightButtonDown(object sender, MouseEventArgs e)
        {
            if (e.Source is not Shape shape) return;
            if (!IsActive) return;
            Panel.Children.Clear();
        }
    }
}

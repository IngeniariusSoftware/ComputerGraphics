namespace GraphicsEditor.VisualTools
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using UIElements;

    public class SelectionTool : BaseTool
    {
        public SelectionTool(Panel panel, List<ListBox> toolPickers)
        {
            ToolPickers = toolPickers;
            panel.PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
        }

        private Shape SelectedShape { get; set; }

        private List<ListBox> ToolPickers { get; }

        public override string ToString() => SelectedShape switch
        {
            Line => "Отрезок",
            Ellipse { Tag: Curve } => "Кривая",
            Ellipse { Tag: ShapeEllipseTool } => "Эллипс",
            Ellipse { Tag: ShapeCircleTool } => "Окружность",
            _ => string.Empty
        };

        public override void EndDrawing(Point currentPoint, Color color)
        {
            base.EndDrawing(currentPoint, color);
            if (SelectedShape == null) return;
            Mouse.Capture(null);

            foreach (ListBox picker in ToolPickers)
            {
                FrameworkElement toolContainer = null;
                foreach (FrameworkElement item in picker.Items)
                {
                    var tool = (item.DataContext as FrameworkElement)?.DataContext as ShapeTool;
                    if (tool == null) continue;
                    if (tool == SelectedShape.Tag)
                    {
                        tool.Shape = SelectedShape;
                        toolContainer = item;
                    }
                    else if (tool is ShapeCurveTool curveTool && SelectedShape.Tag is Curve curve &&
                             curveTool.Algorithm == curve.Algorithm)
                    {
                        curveTool.Curve = curve;
                        toolContainer = item;
                    }
                }

                picker.SelectedItem = toolContainer;
            }

            SelectedShape = null;
        }

        private void OnPreviewMouseLeftButtonDown(object sender, MouseEventArgs args)
        {
            if (args.Source is not Shape shape || !IsOpen)
            {
                SelectedShape = null;
                return;
            }

            SelectedShape = shape;
            Mouse.Capture(SelectedShape);
        }
    }
}

namespace GraphicsEditor.VisualTools
{
    using System.Windows;
    using System.Windows.Controls;
    using System;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    public class VisibilityWindowTool : BaseTool
    {
        public VisibilityWindowTool(FrameworkElement window, FrameworkElement shapesContainer,
            FrameworkElement shapesWindow, Button visibilityButton)
        {
            Window = window;
            ShapesContainer = shapesContainer;
            ShapesWindow = shapesWindow;
            VisibilityButton = visibilityButton;
            VisibilityButton.PreviewMouseLeftButtonDown += OnLeftMouseDown;
        }

        private FrameworkElement Window { get; }

        private FrameworkElement ShapesContainer { get; }

        private FrameworkElement ShapesWindow { get; }

        private Button VisibilityButton { get; }

        public override void Open()
        {
            base.Open();
            Window.Visibility = Visibility.Visible;
            ShapesWindow.Visibility = Visibility.Visible;
        }

        public override void Close()
        {
            base.Close();
            Window.Visibility = Visibility.Collapsed;
            ShapesWindow.Visibility = Visibility.Collapsed;
        }

        private void OnLeftMouseDown(object sender, MouseEventArgs e)
        {
            string mode = (string)VisibilityButton.DataContext switch
            {
                "full" => "partial",
                "partial" => "none",
                "none" => "full",
                _ => throw new Exception("Непредвиденное состояние")
            };
            VisibilityButton.DataContext = mode;
            SetVisibilityMode(mode);
        }

        private void SetVisibilityMode(string mode)
        {
            switch (mode)
            {
                case "full":
                {
                    VisibilityButton.Background =
                        new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Images/FullVisibility.png")));
                    break;
                }

                case "partial":
                {
                    VisibilityButton.Background =
                        new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Images/PartialVisibility.png")));
                    break;
                }

                case "none":
                {
                    VisibilityButton.Background =
                        new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Images/NoneVisibility.png")));
                    break;
                }
            }
        }

        private void FullView()
        {
            ShapesContainer.Visibility = Visibility.Visible;
            ShapesWindow.Visibility = Visibility.Collapsed;
        }

        private void PartialView()
        {
            ShapesContainer.Visibility = Visibility.Collapsed;
        }

        private void NoneView()
        {
            ShapesContainer.Visibility = Visibility.Collapsed;
        }
    }
}

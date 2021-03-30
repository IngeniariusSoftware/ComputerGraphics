namespace GraphicsEditor.Views
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Controllers;
    using VisualTools;
    using WindowState = System.Windows.WindowState;

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml.
    /// </summary>
    public partial class MainView
    {
        public MainView()
        {
            Watcher = Stopwatch.StartNew();
            InitializeComponent();
            var controller = new MainController(this);

            CloseButton.Click += (_, _) => Close();
            MinimizeButton.Click += (_, _) => WindowState = WindowState.Minimized;
            ResizeButton.Click += (_, _) => Resize();

            DrawCanvas.PreviewMouseLeftButtonDown += DrawCanvas_PreviewMouseLeftButtonDown;
            DrawCanvas.MouseMove += DrawCanvas_MouseMove;
            DrawCanvas.PreviewMouseLeftButtonUp += DrawCanvas_PreviewMouseLeftButtonUp;
            DrawCanvas.SizeChanged += DrawCanvas_SizeChanged;

            BorderGrid.PreviewMouseLeftButtonDown += BorderGrid_PreviewMouseLeftButtonDown;
            BorderGrid.MouseMove += BorderGrid_MouseMove;
            BorderGrid.PreviewMouseLeftButtonUp += BorderGrid_PreviewMouseLeftButtonUp;

            AboutButton.Click += AboutButton_Click;

            StateChanged += (_, _) => ResizeButton.Content = WindowState == WindowState.Maximized ? "🗗" : "🗖";
            Loaded += Windows_Loaded;
            KeyDown += LeftShift_Down;
            KeyUp += LeftShift_Up;

            ColorPicker.SelectedColorChanged +=
                (_, _) => ColorPicker.Background = new SolidColorBrush(CurrentColor);
            ToolPicker.PreviewMouseDown += ToolPicker_MouseDown;
            ColorPicker.SelectedColor = Colors.White;
        }

        public event EventHandler<Point> StartDrawing = delegate { };

        public event EventHandler<Point> Drawing = delegate { };

        public event EventHandler<Point> EndDrawing = delegate { };

        public event EventHandler<BaseTool> ToolSelected = delegate { };

        public event EventHandler<bool> AlternativeModeSwitched = delegate { };

        public event EventHandler<Size> DrawAreaSizeChanged = delegate { };

        public Color CurrentColor => ColorPicker.SelectedColor ?? Colors.White;

        public bool IsDragMode { get; private set; }

        private Stopwatch Watcher { get; }

        public void ShowDrawingInformation(string information)
        {
            if (string.IsNullOrWhiteSpace(information))
            {
                if (AboutDrawing.Visibility != Visibility.Visible) return;
                AboutDrawing.Visibility = Visibility.Collapsed;
            }
            else
            {
                AboutDrawing.Text = information;
                Point mousePoint = Mouse.GetPosition(DrawCanvas);
                AboutDrawing.Margin = new Thickness(mousePoint.X + 20, mousePoint.Y + 10, 0, 0);
                if (AboutDrawing.Visibility == Visibility.Visible) return;
                AboutDrawing.Visibility = Visibility.Visible;
            }
        }

        private void LeftShift_Down(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.LeftShift) return;
            ShiftModeLineGif.Visibility = Visibility.Visible;
            ShiftModeEllipseGif.Visibility = Visibility.Visible;
            ShapeLineGif.Visibility = Visibility.Collapsed;
            ShapeEllipseGif.Visibility = Visibility.Collapsed;
            AlternativeModeSwitched(this, true);
        }

        private void LeftShift_Up(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.LeftShift) return;
            ShiftModeLineGif.Visibility = Visibility.Collapsed;
            ShiftModeEllipseGif.Visibility = Visibility.Collapsed;
            ShapeLineGif.Visibility = Visibility.Visible;
            ShapeEllipseGif.Visibility = Visibility.Visible;
            AlternativeModeSwitched(this, false);
        }

        private void Windows_Loaded(object sender, RoutedEventArgs e)
        {
            var background = new WriteableBitmap(
                (int)SystemParameters.PrimaryScreenWidth,
                (int)SystemParameters.PrimaryScreenHeight,
                96,
                96,
                PixelFormats.Bgra32,
                null);
            BackgroundImage.Source = background;
            var buffer = new byte[4 * (int)BackgroundImage.Source.Width * (int)BackgroundImage.Source.Height];
            var foreground = new WriteableBitmap((BitmapSource)BackgroundImage.Source);
            ForegroundImage.Source = foreground;
            BaseLineIcon.DataContext = new ShapeLineTool(ShapeCanvas);
            BresenhamLineIcon.DataContext = new BresenhamLineTool(background, foreground);
            XiaolinWuLineIcon.DataContext = new XiaolinWuLineTool(background, foreground);
            EllipseIcon.DataContext = new ShapeEllipseTool(ShapeCanvas);
            MagnifierIcon.DataContext = new MagnifierTool();
            CircleIcon.DataContext = new ShapeCircleTool(ShapeCanvas);
            EraserIcon.DataContext = new EraserTool(ShapeCanvas, background, foreground, buffer);
            MovingIcon.DataContext = new MovingTool(ShapeCanvas);
            BresenhamCircleIcon.DataContext = new BresenhamCircleTool(background, foreground);
            Watcher.Stop();
            Thread.Sleep((int)Math.Max(3000 - Watcher.ElapsedMilliseconds, 0));
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            About.IsOpen = false;
            About.IsOpen = true;
        }

        private void ToolPicker_MouseDown(object sender, MouseEventArgs e)
        {
            e.Handled = true;
            if (e.Source is not Image image) return;
            ToolSelected(this, image.DataContext as BaseTool);
            ToolPicker.SelectedItem = ToolPicker.SelectedItem == image ? null : image;
        }

        private void DrawCanvas_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            StartDrawing(this, e.GetPosition(DrawCanvas));
            Mouse.Capture(DrawCanvas);
        }

        private void DrawCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            Point mousePoint = e.GetPosition(DrawCanvas);
            Information.Text = $"x: {(int)mousePoint.X}; y: {(int)mousePoint.Y}";

            if (e.LeftButton != MouseButtonState.Pressed) return;
            Drawing(this, mousePoint);
        }

        private void DrawCanvas_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            EndDrawing(this, e.GetPosition(DrawCanvas));
            AboutDrawing.Visibility = Visibility.Collapsed;
            Mouse.Capture(null);
        }

        private void BorderGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => IsDragMode = true;

        private void BorderGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && IsDragMode) DragWindow(e);
        }

        private void BorderGrid_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e) => IsDragMode = false;

        private void DragWindow(MouseEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                Point mousePosition = PointToScreen(e.GetPosition(this));
                WindowState = WindowState.Normal;
                Left = mousePosition.X - (Width / 2);
                Top = mousePosition.Y;
            }

            DragMove();
        }

        private void DrawCanvas_SizeChanged(object sender, SizeChangedEventArgs e) =>
            DrawAreaSizeChanged(this, e.NewSize);

        private void Resize() =>
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
    }
}

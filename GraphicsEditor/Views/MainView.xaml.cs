namespace GraphicsEditor.Views
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Controllers;
    using Geometry;
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

            DrawCanvas.MouseLeftButtonDown += DrawCanvas_MouseLeftButtonDown;
            DrawCanvas.MouseMove += DrawCanvas_MouseMove;
            DrawCanvas.MouseLeftButtonUp += DrawCanvas_MouseLeftButtonUp;
            DrawCanvas.SizeChanged += DrawCanvas_SizeChanged;

            BorderGrid.MouseLeftButtonDown += BorderGrid_MouseLeftButtonDown;
            BorderGrid.MouseMove += BorderGrid_MouseMove;
            BorderGrid.MouseLeftButtonUp += BorderGrid_MouseLeftButtonUp;

            AboutButton.Click += AboutButton_Click;

            StateChanged += (_, _) => ResizeButton.Content = WindowState == WindowState.Maximized ? "🗗" : "🗖";
            Loaded += Windows_Loaded;
            KeyDown += LeftShift_Down;
            KeyUp += LeftShift_Up;

            ColorPicker.SelectedColorChanged +=
                (_, _) => ColorPicker.Background = new SolidColorBrush(CurrentColor);
            ToolPicker.PreviewMouseDown += ToolPicker_MouseDown;

            ColorPicker.SelectedColor = Colors.White;
            BackBitmap = new VariableSizeWriteableBitmap();
            ForeBitmap = new VariableSizeWriteableBitmap();
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

        private VariableSizeWriteableBitmap BackBitmap { get; }

        private VariableSizeWriteableBitmap ForeBitmap { get; }

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
                if (AboutDrawing.Visibility != Visibility.Visible) AboutDrawing.Visibility = Visibility.Visible;
                AboutDrawing.Margin = new Thickness(
                    Math.Clamp(mousePoint.X + 20, 0, DrawCanvas.ActualWidth - AboutDrawing.ActualWidth),
                    Math.Clamp(mousePoint.Y + 10, 0, DrawCanvas.ActualHeight - AboutDrawing.ActualHeight), 0, 0);
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
            BackBitmap.Bitmap = background;
            ForeBitmap.Bitmap = foreground;
            ShapeLineIcon.DataContext = new ShapeLineTool(ShapesCanvas);
            BresenhamLineIcon.DataContext = new BresenhamLineTool(BackBitmap, ForeBitmap);
            XiaolinWuLineIcon.DataContext = new XiaolinWuLineTool(BackBitmap, ForeBitmap);
            ShapeEllipseIcon.DataContext = new ShapeEllipseTool(ShapesCanvas);
            MagnifierIcon.DataContext = new MagnifierTool();
            ShapeCircleIcon.DataContext = new ShapeCircleTool(ShapesCanvas);
            EraserIcon.DataContext = new EraserTool(ShapesCanvas, BackBitmap, ForeBitmap, buffer);
            MovingIcon.DataContext = new MovingTool(ShapesCanvas);
            BresenhamCircleIcon.DataContext = new BresenhamCircleTool(BackBitmap, ForeBitmap);
            BresenhamEllipseIcon.DataContext = new BresenhamEllipseTool(BackBitmap, ForeBitmap);
            VisibilityWindowIcon.DataContext =
                new VisibilityWindowTool(VisibleArea, ShapesCanvas, ShapesWindow, VisibilityButton);
            var resizer = new ResizerController(ResizerIcon, VisibleArea);
            var moveable = new MovingController(VisibleArea);
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
            bool isToolDeselected = ToolPicker.SelectedItem == image;
            if (isToolDeselected)
            {
                if (ToolPicker.SelectionMode == SelectionMode.Multiple)
                {
                    ToolPicker.SelectedItems.Remove(e.Source);
                }
                else
                {
                    ToolPicker.SelectedItem = null;
                }
            }
            else
            {
                if (ToolPicker.SelectionMode == SelectionMode.Multiple)
                {
                    if (ToolPicker.SelectedItems.Count > 1) ToolPicker.SelectedItems.RemoveAt(1);
                    ToolPicker.SelectedItems.Add(e.Source);
                }
                else
                {
                    ToolPicker.SelectedItem = e.Source;
                }
            }

            ToolSelected(this, image.DataContext as BaseTool);

            if (e.Source != VisibilityWindowIcon) return;
            if (isToolDeselected)
            {
                ToolPicker.Items.Filter = _ => true;
                ToolPicker.SelectionMode = SelectionMode.Single;
            }
            else
            {
                var shapeInstruments = new Image[] { VisibilityWindowIcon, ShapeLineIcon };
                ToolPicker.Items.Filter = x => shapeInstruments.Contains(x);
                ToolPicker.SelectionMode = SelectionMode.Multiple;
            }
        }

        private void DrawCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Handled) return;
            StartDrawing(this, e.GetPosition(DrawCanvas));
            Mouse.Capture(DrawCanvas);
        }

        private void DrawCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Handled) return;
            Point mousePoint = e.GetPosition(DrawCanvas);
            Information.Text = $"x: {(int)mousePoint.X}; y: {(int)mousePoint.Y}";

            if (e.LeftButton != MouseButtonState.Pressed) return;
            Drawing(this, mousePoint);
        }

        private void DrawCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.Handled) return;
            EndDrawing(this, e.GetPosition(DrawCanvas));
            AboutDrawing.Visibility = Visibility.Collapsed;
            Mouse.Capture(null);
        }

        private void BorderGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => IsDragMode = true;

        private void BorderGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && IsDragMode) DragWindow(e);
        }

        private void BorderGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => IsDragMode = false;

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

        private void DrawCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            int margin = 20;

            if (VisibleArea.Margin.Left + VisibleArea.ActualWidth + margin > e.NewSize.Width)
            {
                if (VisibleArea.Margin.Left > margin)
                {
                    Thickness tmp = VisibleArea.Margin;
                    tmp.Left = Math.Max(e.NewSize.Width - VisibleArea.ActualWidth - margin, margin);
                    VisibleArea.Margin = tmp;
                }
                else
                {
                    VisibleArea.Width = Math.Max(e.NewSize.Width - (margin * 2), margin);
                }
            }

            if (VisibleArea.Margin.Top + VisibleArea.ActualHeight + margin > e.NewSize.Height)
            {
                if (VisibleArea.Margin.Top > margin)
                {
                    Thickness tmp = VisibleArea.Margin;
                    tmp.Top = Math.Max(e.NewSize.Height - VisibleArea.ActualHeight - margin, margin);
                    VisibleArea.Margin = tmp;
                }
                else
                {
                    VisibleArea.Height = Math.Max(e.NewSize.Height - (margin * 2), margin);
                }
            }

            DrawAreaSizeChanged(this, e.NewSize);
            BackBitmap.PixelWidth = (int)e.NewSize.Width;
            BackBitmap.PixelHeight = (int)e.NewSize.Height;
            ForeBitmap.PixelWidth = (int)e.NewSize.Width;
            ForeBitmap.PixelHeight = (int)e.NewSize.Height;
        }

        private void Resize() =>
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
    }
}

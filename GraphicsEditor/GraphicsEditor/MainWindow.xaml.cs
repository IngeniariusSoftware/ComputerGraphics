
namespace Lesson1
{
    using System.Diagnostics;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    using Lesson1.Tools;

    using WindowsInput;
    using WindowsInput.Native;

    using MouseButton = System.Windows.Input.MouseButton;

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly Stopwatch _watcher = Stopwatch.StartNew();

        private readonly IKeyboardSimulator _keyboard = new InputSimulator().Keyboard;

        private BaseTool _tool;

        private bool _isDragMode;

        private bool _isShiftMode;

        private bool _isMagnifierMode;

        public MainWindow()
        {
            InitializeComponent();
            CloseButton.Click += (sender, args) => Close();
            MinimizeButton.Click += (sender, args) => WindowState = WindowState.Minimized;
            ResizeButton.Click += (sender, args) => Resize();

            DrawCanvas.MouseDown += DrawCanvas_MouseDown;
            DrawCanvas.MouseMove += DrawCanvas_MouseMove;
            DrawCanvas.MouseUp += DrawCanvas_MouseUp;

            BorderGrid.MouseDown += BorderGrid_MouseDown;
            BorderGrid.MouseMove += BorderGrid_MouseMove;
            BorderGrid.MouseUp += BorderGrid_MouseUp;

            AboutButton.Click += AboutButton_Click;

            StateChanged += (sender, args) => ResizeButton.Content = WindowState == WindowState.Maximized ? "🗗" : "🗖";
            Loaded += Windows_Loaded;
            KeyDown += LeftShift_Down;
            KeyUp += LeftShift_Up;

            ColorPicker.SelectedColorChanged +=
                (sender, args) => ColorPicker.Background = new SolidColorBrush(CurrentColor);
            ToolPicker.SelectionChanged += ToolPicker_SelectedItemChanged;
            ColorPicker.SelectedColor = Colors.White;
        }

        private BaseTool Tool
        {
            get => _tool ?? new BaseTool();

            set => _tool = value;
        }

        private bool IsShiftMode
        {
            get => _isShiftMode;

            set
            {
                if (IsShiftMode != value)
                {
                    if (!_isShiftMode)
                    {
                        _isShiftMode = true;
                        ShiftModeLineGif.Visibility = Visibility.Visible;
                        ShiftModeEllipseGif.Visibility = Visibility.Visible;
                        ShapeLineGif.Visibility = Visibility.Collapsed;
                        ShapeEllipseGif.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        _isShiftMode = false;
                        ShiftModeLineGif.Visibility = Visibility.Collapsed;
                        ShiftModeEllipseGif.Visibility = Visibility.Collapsed;
                        ShapeLineGif.Visibility = Visibility.Visible;
                        ShapeEllipseGif.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        private Color CurrentColor => ColorPicker.SelectedColor ?? Colors.White;

        private void LeftShift_Down(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift)
            {
                IsShiftMode = true;
            }
        }

        private void LeftShift_Up(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift)
            {
                IsShiftMode = false;
            }
        }

        private void Windows_Loaded(object sender, RoutedEventArgs e)
        {
            BackgroundImage.Source = new WriteableBitmap(
                (int)SystemParameters.PrimaryScreenWidth,
                (int)SystemParameters.PrimaryScreenHeight,
                96,
                96,
                PixelFormats.Bgra32,
                null);
            Color backgroundColor = ((SolidColorBrush)DrawCanvas.Background).Color;
            var buffer = new byte[4 * (int)BackgroundImage.Source.Width * (int)BackgroundImage.Source.Height];
            ForegroundImage.Source = new WriteableBitmap((BitmapSource)BackgroundImage.Source);
            BaseLineIcon.DataContext = new ShapeLineTool(DrawCanvas);
            BresenhamLineIcon.DataContext = new BresenhamLineTool(BackgroundImage, ForegroundImage, buffer);
            XiaolinWuLineIcon.DataContext = new XiaolinWuLine(BackgroundImage, ForegroundImage, buffer);
            EllipseIcon.DataContext = new ShapeEllipseTool(DrawCanvas);
            Tool = new BaseTool();
            _watcher.Stop();
            Thread.Sleep((int)(2000 - (_watcher.ElapsedMilliseconds / 1000.0)));
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            About.IsOpen = false;
            About.IsOpen = true;
        }

        private void ToolPicker_SelectedItemChanged(object sender, SelectionChangedEventArgs args)
        {
            if (ToolPicker.SelectedItem is Image image)
            {
                if (image == MagnifierIcon)
                {
                    _isMagnifierMode = true;
                    _keyboard.KeyDown(VirtualKeyCode.LWIN);
                    _keyboard.KeyPress(VirtualKeyCode.OEM_PLUS);
                    _keyboard.KeyUp(VirtualKeyCode.LWIN);
                    Tool = new BaseTool();
                }
                else
                {
                    if (_isMagnifierMode)
                    {
                        _keyboard.KeyDown(VirtualKeyCode.LWIN);
                        _keyboard.KeyPress(VirtualKeyCode.ESCAPE);
                        _keyboard.KeyUp(VirtualKeyCode.LWIN);
                        _isMagnifierMode = false;
                    }

                    Tool = image.DataContext as BaseTool;
                }
            }
        }

        private void DrawCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                Tool.StartDrawing(Mouse.GetPosition(DrawCanvas), CurrentColor);
                Mouse.Capture(DrawCanvas);
            }
        }

        private void DrawCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            Point mousePoint = Mathp.DiagonalClamp(
                Tool.StartPoint,
                e.GetPosition(DrawCanvas),
                new Point(0, 0),
                new Point(DrawCanvas.ActualWidth, DrawCanvas.ActualHeight));

            if (Keyboard.IsKeyDown(Key.LeftShift))
            {
                mousePoint = Mathp.ToAngle(Tool.StartPoint, mousePoint, Tool.RoundAngle(mousePoint));
            }

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (!string.IsNullOrEmpty(_tool?.ToString()))
                {
                    AboutDrawing.Visibility = Visibility.Visible;
                    AboutDrawing.Text = _tool.ToString();
                    AboutDrawing.Margin = new Thickness(mousePoint.X + 20, mousePoint.Y + 10, 0, 0);
                }

                Tool.TryDrawing(mousePoint, CurrentColor);
            }

            Information.Text = $"x: {(int)mousePoint.X}; y: {(int)mousePoint.Y}";
        }

        private void DrawCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                AboutDrawing.Visibility = Visibility.Collapsed;
                Tool.EndDrawing(
                    Mathp.DiagonalClamp(
                        Tool.StartPoint,
                        e.GetPosition(DrawCanvas),
                        new Point(0, 0),
                        new Point(DrawCanvas.ActualWidth, DrawCanvas.ActualHeight)),
                    CurrentColor);
                Mouse.Capture(null);
            }
        }

        private void BorderGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                _isDragMode = true;
            }
        }

        private void BorderGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && _isDragMode)
            {
                DragWindow(e);
            }
        }

        private void BorderGrid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                _isDragMode = false;
            }
        }

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

        private void Resize() =>
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
    }
}
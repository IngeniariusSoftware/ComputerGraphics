namespace GraphicsEditor.Views
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Controllers;
    using Geometry;
    using UIElements;
    using VisualTools;
    using WindowState = System.Windows.WindowState;

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml.
    /// </summary>
    public partial class MainView
    {
        public MainView()
        {
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

            AboutButton.DataContext = About;
            AboutButton.PreviewMouseLeftButtonUp += (_, _) => About.IsOpen = true;
            AboutButton.MouseLeave += (_, _) => About.IsOpen = false;

            StateChanged += (_, _) => ResizeButton.Content = WindowState == WindowState.Maximized ? "🗗" : "🗖";
            Loaded += Windows_Loaded;
            KeyDown += LeftShift_Down;
            KeyUp += LeftShift_Up;

            ColorPicker.SelectedColorChanged +=
                (_, _) => ColorPicker.Background = new SolidColorBrush(CurrentColor);
            ToolPicker.PreviewMouseLeftButtonDown += ToolPicker_PreviewMouseLeftButtonDown;
            ToolPicker.PreviewMouseRightButtonDown += (_, args) => args.Handled = true;

            ColorPicker.SelectedColor = Colors.White;
            BackgroundBitmap = new VariableSizeWriteableBitmap();
            ForegroundBitmap = new VariableSizeWriteableBitmap();
        }

        public event EventHandler<Point> StartDrawing = delegate { };

        public event EventHandler<Point> Drawing = delegate { };

        public event EventHandler<Point> EndDrawing = delegate { };

        public event EventHandler<BaseTool> ToolSelected = delegate { };

        public event EventHandler<bool> AlternativeModeSwitched = delegate { };

        public event EventHandler<Size> DrawAreaSizeChanged = delegate { };

        public Color CurrentColor => ColorPicker.SelectedColor ?? Colors.White;

        public bool IsDragMode { get; private set; }

        private VariableSizeWriteableBitmap BackgroundBitmap { get; }

        private VariableSizeWriteableBitmap ForegroundBitmap { get; }

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

        private void LeftShift_Down(object sender, KeyEventArgs args)
        {
            if (args.Key != Key.LeftShift) return;
            ShiftModeLineGif.Visibility = Visibility.Visible;
            ShiftModeEllipseGif.Visibility = Visibility.Visible;
            ShapeLineGif.Visibility = Visibility.Collapsed;
            ShapeEllipseGif.Visibility = Visibility.Collapsed;
            AlternativeModeSwitched(this, true);
        }

        private void LeftShift_Up(object sender, KeyEventArgs args)
        {
            if (args.Key != Key.LeftShift) return;
            ShiftModeLineGif.Visibility = Visibility.Collapsed;
            ShiftModeEllipseGif.Visibility = Visibility.Collapsed;
            ShapeLineGif.Visibility = Visibility.Visible;
            ShapeEllipseGif.Visibility = Visibility.Visible;
            AlternativeModeSwitched(this, false);
        }

        private void Windows_Loaded(object sender, RoutedEventArgs args)
        {
            var background = new WriteableBitmap(
                (int)SystemParameters.PrimaryScreenWidth,
                (int)SystemParameters.PrimaryScreenHeight,
                96,
                96,
                PixelFormats.Bgra32,
                null);
            RasterBackground.Source = background;
            var buffer = new byte[4 * (int)RasterBackground.Source.Width * (int)RasterBackground.Source.Height];
            var coordinatesBuffer =
                new int[(((int)RasterBackground.Source.Width * (int)RasterBackground.Source.Height) + 1) * 8];
            var foreground = new WriteableBitmap((BitmapSource)RasterBackground.Source);
            RasterForeground.Source = foreground;
            BackgroundBitmap.Bitmap = background;
            ForegroundBitmap.Bitmap = foreground;
            IPanel vectorBackground = new ExtendedPanel(VectorBackground,
                new ExtendedUIElementCollection(VectorBackground.Children));
            IPanel vectorForeground = new ExtendedPanel(VectorForeground,
                new ExtendedUIElementCollection(VectorForeground.Children));
            ShapeLineIcon.DataContext = new ShapeLineTool(vectorBackground, vectorForeground);
            ShapeLinePanel.DataContext = ShapeLineIcon;
            BresenhamLineIcon.DataContext = new BresenhamLineTool(BackgroundBitmap, ForegroundBitmap);
            BresenhamLinePanel.DataContext = BresenhamLineIcon;
            XiaolinWuLineIcon.DataContext = new XiaolinWuLineTool(BackgroundBitmap, ForegroundBitmap);
            XiaolinWuLinePanel.DataContext = XiaolinWuLineIcon;
            ShapeEllipseIcon.DataContext = new ShapeEllipseTool(vectorBackground, vectorForeground);
            ShapeEllipsePanel.DataContext = ShapeEllipseIcon;
            BresenhamEllipseIcon.DataContext = new BresenhamEllipseTool(BackgroundBitmap, ForegroundBitmap);
            BresenhamEllipsePanel.DataContext = BresenhamEllipseIcon;
            ShapeCircleIcon.DataContext = new ShapeCircleTool(vectorBackground, vectorForeground);
            ShapeCirclePanel.DataContext = ShapeCircleIcon;
            BresenhamCircleIcon.DataContext = new BresenhamCircleTool(BackgroundBitmap, ForegroundBitmap);
            BresenhamCirclePanel.DataContext = BresenhamCircleIcon;
            ICurveAlgorithm bezier = new BezierCurve();
            ShapeElementaryBezierIcon.DataContext =
                new ShapeElementaryBezierTool(vectorBackground, vectorForeground, bezier, 0.01);
            ICurveAlgorithm deCasteljau = new DeCasteljauCurve();
            ShapeElementaryBezierPanel.DataContext = ShapeElementaryBezierIcon;
            ShapeElementaryDeCasteljauIcon.DataContext =
                new ShapeElementaryDeCasteljauTool(vectorBackground, vectorForeground, deCasteljau, 0.01);
            ShapeElementaryDeCasteljauPanel.DataContext = ShapeElementaryDeCasteljauIcon;
            ShapeCompoundBezierIcon.DataContext =
                new ShapeCompoundBezierTool(vectorBackground, vectorForeground, bezier, 0.05);
            ShapeCompoundBezierPanel.DataContext = ShapeCompoundBezierIcon;
            ShapeCompoundBIcon.DataContext =
                new ShapeCompoundBTool(vectorBackground, vectorForeground, new BCurve(), 0.05);
            ShapeCompoundBPanel.DataContext = ShapeCompoundBIcon;
            FillIcon.DataContext = new FillTool(BackgroundBitmap, ForegroundBitmap, coordinatesBuffer);
            FillPanel.DataContext = FillIcon;
            ByLineFillIcon.DataContext = new ByLineFillTool(BackgroundBitmap, ForegroundBitmap, coordinatesBuffer);
            ByLineFillPanel.DataContext = ByLineFillIcon;
            RasterEraserIcon.DataContext = new RasterEraserTool(BackgroundBitmap, buffer);
            ShapeEraserIcon.DataContext = new ShapeEraserTool(vectorBackground);
            MovingIcon.DataContext = new MovingTool(VectorBackground);
            MagnifierIcon.DataContext = new MagnifierTool();
            SelectionIcon.DataContext =
                new SelectionTool(VectorBackground, new List<ListBox> { LinesPicker, EllipsesPicker, CurvesPicker });
            var resizerController = new ResizerController(ResizerIcon, VisibleArea);
            var movingController = new MovingController(VisibleArea, 20);
            VisibilityWindowIcon.DataContext =
                new VisibilityWindowTool(VisibleArea, vectorBackground, ShapesWindow, VisibilityIcon, movingController,
                    resizerController);

            var lines = new List<FrameworkElement> { ShapeLineIcon, BresenhamLineIcon, XiaolinWuLineIcon };
            lines.ForEach(x => x.MouseRightButtonUp += (_, _) => LinesPopup.IsOpen = true);
            LinesPicker.DataContext = lines;
            LinesPicker.SelectionChanged += NestedToolPicker_SelectionChanged;
            LinesPicker.SelectionChanged += (_, _) => LinesPopup.IsOpen = false;

            var ellipses = new List<FrameworkElement>
                { ShapeEllipseIcon, BresenhamEllipseIcon, ShapeCircleIcon, BresenhamCircleIcon };
            ellipses.ForEach(x => x.MouseRightButtonUp += (_, _) => EllipsesPopup.IsOpen = true);
            EllipsesPicker.DataContext = ellipses;
            EllipsesPicker.SelectionChanged += NestedToolPicker_SelectionChanged;
            EllipsesPicker.SelectionChanged += (_, _) => EllipsesPopup.IsOpen = false;

            var curves = new List<FrameworkElement>
            {
                ShapeElementaryBezierIcon, ShapeElementaryDeCasteljauIcon, ShapeCompoundBezierIcon, ShapeCompoundBIcon,
            };
            curves.ForEach(x => x.MouseRightButtonUp += (_, _) => CurvesPopup.IsOpen = true);
            CurvesPicker.DataContext = curves;
            CurvesPicker.SelectionChanged += NestedToolPicker_SelectionChanged;
            CurvesPicker.SelectionChanged += (_, _) => CurvesPopup.IsOpen = false;

            var fillers = new List<FrameworkElement> { FillIcon, ByLineFillIcon };
            fillers.ForEach(x => x.MouseRightButtonUp += (_, _) => FillPopup.IsOpen = true);
            FillPicker.DataContext = fillers;
            FillPicker.SelectionChanged += NestedToolPicker_SelectionChanged;
            FillPicker.SelectionChanged += (_, _) => FillPopup.IsOpen = false;
        }

        private void NestedToolPicker_SelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (sender is not ListBox list) return;
            if (list.SelectedItem == null) return;
            if (list.DataContext is not IEnumerable<FrameworkElement> tools) return;
            foreach (FrameworkElement t in tools)
            {
                t.Visibility = Visibility.Collapsed;
            }

            if (list.SelectedItem is not FrameworkElement selected) throw new Exception();
            if (selected.DataContext is not FrameworkElement tool) throw new Exception();

            tool.Visibility = Visibility.Visible;
            SelectTool(list, tool);
            list.UnselectAll();
        }

        private void ToolPicker_PreviewMouseLeftButtonDown(object sender, MouseEventArgs args)
        {
            if (args.Source == sender)
            {
                args.Handled = true;
                return;
            }

            if (args.Source is not FrameworkElement element) return;
            if (element.GetType().Name == "PopupRoot") args.Handled = true;
            if (element.DataContext is not BaseTool) return;
            args.Handled = true;
            SelectTool(ToolPicker, element);
        }

        private void SelectTool(ListBox list, FrameworkElement tool)
        {
            if (list != ToolPicker && tool == ToolPicker.SelectedItem) return;
            bool isToolDeselected = ToolPicker.SelectedItems.Contains(tool);
            if (isToolDeselected)
            {
                if (ToolPicker.SelectionMode == SelectionMode.Multiple)
                {
                    ToolPicker.SelectedItems.Remove(tool);
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
                    ToolPicker.SelectedItems.Add(tool);
                }
                else
                {
                    ToolPicker.SelectedItem = tool;
                }
            }

            ToolSelected(this, tool.DataContext as BaseTool);

            if (tool != VisibilityWindowIcon) return;
            if (isToolDeselected)
            {
                ToolPicker.Items.Filter = _ => true;
                ToolPicker.SelectedItems.Clear();
                ToolPicker.SelectionMode = SelectionMode.Single;
            }
            else
            {
                var shapeInstruments = new[] { VisibilityWindowIcon, ShapeLineIcon };
                ToolPicker.Items.Filter = x => shapeInstruments.Contains(x);
                ToolPicker.SelectionMode = SelectionMode.Multiple;
            }
        }

        private void DrawCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs args)
        {
            if (args.Handled) return;
            StartDrawing(this, args.GetPosition(DrawCanvas));
            Mouse.Capture(DrawCanvas);
        }

        private void DrawCanvas_MouseMove(object sender, MouseEventArgs args)
        {
            if (args.Handled) return;
            Point mousePoint = args.GetPosition(DrawCanvas);
            Information.Text = $"x: {(int)mousePoint.X}; y: {(int)mousePoint.Y}";

            if (args.LeftButton != MouseButtonState.Pressed) return;
            Drawing(this, mousePoint);
        }

        private void DrawCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs args)
        {
            if (args.Handled) return;
            EndDrawing(this, args.GetPosition(DrawCanvas));
            AboutDrawing.Visibility = Visibility.Collapsed;
            Mouse.Capture(null);
        }

        private void BorderGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs args) => IsDragMode = true;

        private void BorderGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs args) => IsDragMode = false;

        private void BorderGrid_MouseMove(object sender, MouseEventArgs args)
        {
            if (args.LeftButton == MouseButtonState.Pressed && IsDragMode) DragWindow(args);
        }

        private void DragWindow(MouseEventArgs args)
        {
            if (WindowState == WindowState.Maximized)
            {
                Point mousePosition = PointToScreen(args.GetPosition(this));
                WindowState = WindowState.Normal;
                Left = mousePosition.X - (Width / 2);
                Top = mousePosition.Y;
            }

            DragMove();
        }

        private void DrawCanvas_SizeChanged(object sender, SizeChangedEventArgs args)
        {
            int margin = 20;

            if (VisibleArea.Margin.Left + VisibleArea.ActualWidth + margin > args.NewSize.Width)
            {
                if (VisibleArea.Margin.Left > margin)
                {
                    Thickness tmp = VisibleArea.Margin;
                    tmp.Left = Math.Max(args.NewSize.Width - VisibleArea.ActualWidth - margin, margin);
                    VisibleArea.Margin = tmp;
                }
                else
                {
                    VisibleArea.Width = Math.Max(args.NewSize.Width - (margin * 2), margin);
                }
            }

            if (VisibleArea.Margin.Top + VisibleArea.ActualHeight + margin > args.NewSize.Height)
            {
                if (VisibleArea.Margin.Top > margin)
                {
                    Thickness tmp = VisibleArea.Margin;
                    tmp.Top = Math.Max(args.NewSize.Height - VisibleArea.ActualHeight - margin, margin);
                    VisibleArea.Margin = tmp;
                }
                else
                {
                    VisibleArea.Height = Math.Max(args.NewSize.Height - (margin * 2), margin);
                }
            }

            DrawAreaSizeChanged(this, args.NewSize);
            BackgroundBitmap.PixelWidth = (int)args.NewSize.Width;
            BackgroundBitmap.PixelHeight = (int)args.NewSize.Height;
            ForegroundBitmap.PixelWidth = (int)args.NewSize.Width;
            ForegroundBitmap.PixelHeight = (int)args.NewSize.Height;
        }

        private void Resize() =>
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
    }
}

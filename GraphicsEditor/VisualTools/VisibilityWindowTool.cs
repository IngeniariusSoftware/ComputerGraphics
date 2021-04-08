namespace GraphicsEditor.VisualTools
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;
    using Controllers;
    using Geometry;
    using UIElements;
    using LineSegment = Geometry.LineSegment;
    using Rectangle = Geometry.Rectangle;

    public enum VisibilityModes
    {
        Full,

        Partial,

        None,
    }

    public class VisibilityWindowTool : BaseTool
    {
        private VisibilityModes _visibilityMode;

        public VisibilityWindowTool(FrameworkElement window, IPanel shapesContainer,
            Panel shapesWindow, Image visibilityIcon, MovingController movingController,
            ResizerController resizerController)
        {
            Window = window;
            ShapesContainer = shapesContainer;
            ShapesWindow = shapesWindow;
            movingController.MarginChanged += OnVisibilityWindowMarginChanged;
            resizerController.MarginChanged += OnVisibilityWindowMarginChanged;
            VisibilityIcon = visibilityIcon;
            VisibilityIcon.PreviewMouseLeftButtonDown += OnLeftMouseDown;
            VisibilityIcon.MouseEnter += (_, _) => VisibilityIcon.Cursor = Cursors.Hand;
            VisibilityIcon.MouseLeave += (_, _) => VisibilityIcon.Cursor = Cursors.Arrow;
            VisibilityIcon.DataContext = VisibilityModes.Full;
            Window.SizeChanged += OnVisibilityWindowSizeChanged;
            UpdateButtonImage();
            Lines = new List<Line>();
        }

        private FrameworkElement Window { get; }

        private IPanel ShapesContainer { get; }

        private Panel ShapesWindow { get; }

        private Image VisibilityIcon { get; }

        private List<Line> Lines { get; set; }

        private VisibilityModes VisibilityMode
        {
            get => _visibilityMode;

            set
            {
                _visibilityMode = value;
                UpdateButtonImage();
                UpdateLinesView();
            }
        }

        public override void Open()
        {
            base.Open();
            Window.Visibility = Visibility.Visible;
            ShapesContainer.Visibility = Visibility.Collapsed;
            ShapesWindow.Visibility = Visibility.Visible;
            Lines = ShapesContainer.Children.Where(x => x is Line).Cast<Line>().ToList();
            ShapesContainer.Children.Added += OnChildAdded;
            ShapesContainer.Children.Removed += OnChildRemoved;
            UpdateLinesView();
        }

        public override void Close()
        {
            base.Close();
            Window.Visibility = Visibility.Collapsed;
            ShapesWindow.Visibility = Visibility.Collapsed;
            ShapesContainer.Visibility = Visibility.Visible;
            ShapesContainer.Children.Added -= OnChildAdded;
            ShapesContainer.Children.Removed -= OnChildRemoved;
            Lines.Clear();
        }

        private void OnLeftMouseDown(object sender, MouseEventArgs e)
        {
            var mode = (VisibilityModes)VisibilityIcon.DataContext switch
            {
                VisibilityModes.Full => VisibilityModes.Partial,
                VisibilityModes.Partial => VisibilityModes.None,
                VisibilityModes.None => VisibilityModes.Full,
                _ => throw new Exception("Непредвиденное состояние")
            };

            VisibilityIcon.DataContext = mode;
            VisibilityMode = mode;
            e.Handled = true;
        }

        private void UpdateButtonImage()
        {
            string image = VisibilityMode switch
            {
                VisibilityModes.Full => "Full",
                VisibilityModes.Partial => "Partial",
                VisibilityModes.None => "None",
                _ => throw new Exception("Непредвиденное состояние")
            };

            VisibilityIcon.Source = new BitmapImage(new Uri($"pack://application:,,,/Images/{image}Visibility.png"));
        }

        private void OnChildAdded(object sender, UIElement e)
        {
            if (e is not Line line) return;
            Lines.Add(line);
            UpdateLinesView();
        }

        private void OnChildRemoved(object sender, UIElement e)
        {
            if (e is not Line line) return;
            Lines.Remove(line);
            UpdateLinesView();
        }

        private void OnVisibilityWindowSizeChanged(object sender, SizeChangedEventArgs e) => UpdateLinesView();

        private void OnVisibilityWindowMarginChanged(object sender, Thickness e) => UpdateLinesView();

        private void UpdateLinesView()
        {
            ShapesWindow.Children.Clear();
            switch (VisibilityMode)
            {
                case VisibilityModes.Full:
                {
                    FullView();
                    break;
                }

                case VisibilityModes.Partial:
                {
                    PartialView();
                    break;
                }

                case VisibilityModes.None:
                {
                    NoneView();
                    break;
                }

                default: throw new ArgumentOutOfRangeException();
            }
        }

        private void FullView()
        {
            foreach (Line line in Lines)
            {
                var lineView = new Line()
                {
                    X1 = line.X1,
                    Y1 = line.Y1,
                    X2 = line.X2,
                    Y2 = line.Y2,
                    Stroke = new SolidColorBrush(Colors.White),
                };

                ShapesWindow.Children.Add(lineView);
            }
        }

        [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
        private void PartialView()
        {
            Rectangle rectangle = new Rectangle(new Point(Window.Margin.Left, Window.Margin.Top),
                new Point(Window.Margin.Left + Window.ActualWidth, Window.Margin.Top + Window.ActualHeight));

            foreach (Line line in Lines)
            {
                LineSegment lineSegment = MathGeometry.LineSegmentClamp(line, rectangle);

                Color color = Colors.Green;

                if (lineSegment == null)
                {
                    color = Colors.Red;
                }
                else if (lineSegment.Start.X != line.X1 || lineSegment.Start.Y != line.Y1 ||
                         lineSegment.End.X != line.X2 || lineSegment.End.Y != line.Y2)
                {
                    color = Colors.Yellow;
                }

                var lineView = new Line
                {
                    X1 = line.X1,
                    Y1 = line.Y1,
                    X2 = line.X2,
                    Y2 = line.Y2,
                    Stroke = new SolidColorBrush(color),
                };

                ShapesWindow.Children.Add(lineView);
            }
        }

        private void NoneView()
        {
            Rectangle rectangle = new Rectangle(new Point(Window.Margin.Left, Window.Margin.Top),
                new Point(Window.Margin.Left + Window.ActualWidth, Window.Margin.Top + Window.ActualHeight));

            foreach (Line line in Lines)
            {
                LineSegment lineSegment = MathGeometry.LineSegmentClamp(line, rectangle);
                if (lineSegment == null) continue;
                var lineView = new Line
                {
                    X1 = lineSegment.Start.X,
                    Y1 = lineSegment.Start.Y,
                    X2 = lineSegment.End.X,
                    Y2 = lineSegment.End.Y,
                    Stroke = new SolidColorBrush(Colors.White),
                };

                ShapesWindow.Children.Add(lineView);
            }
        }
    }
}

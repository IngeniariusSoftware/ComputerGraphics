namespace GraphicsEditor.UIElements
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using Extensions;
    using Geometry;
    using LineSegment = Geometry.LineSegment;

    public class CompoundBezierCurve : ICurve
    {
        public CompoundBezierCurve(IPanel background, ICurveAlgorithm algorithm, double resolution)
        {
            Algorithm = algorithm;
            Background = background;
            Background.Children.Removed += OnRemoved;
            EllipsePoints = new List<Ellipse>();
            ReferencePoints = new List<Point>();
            InterpolationSegments = new List<Line>();
            ConnectionLines = new List<Line>();
            Resolution = resolution;
            for (double i = 0; i < 1; i += Resolution)
            {
                InterpolationSegments.Add(new Line
                    { Stroke = new SolidColorBrush(Colors.Transparent), IsEnabled = false });
            }

            InterpolationSegments.Add(new Line { Stroke = new SolidColorBrush(Colors.Transparent) });
            CurveSegmentsCount = InterpolationSegments.Count;
            Size = new Size(16, 16);
            Bias = new Vector(Size.Width / 2, Size.Height / 2);
        }

        public List<Ellipse> EllipsePoints { get; }

        public List<Point> ReferencePoints { get; }

        public List<Line> ConnectionLines { get; }

        public List<Line> InterpolationSegments { get; }

        public double Resolution { get; }

        public Size Size { get; }

        public Vector Bias { get; }

        public int CurveSegmentsCount { get; }

        public ICurveAlgorithm Algorithm { get; }

        private IPanel Background { get; }

        public void AddPoint(Ellipse ellipse, Color color)
        {
            if (EllipsePoints.Count > 3 && EllipsePoints.Count % 3 == 1)
            {
                var dockingPoint = new Point(ellipse.Margin.Left, ellipse.Margin.Top);
                var lineSegment = new LineSegment(ReferencePoints[^2], ReferencePoints[^1]);
                dockingPoint = MathGeometry.NearestPoint(lineSegment, dockingPoint);
                ellipse.Margin = new Thickness(dockingPoint.X, dockingPoint.Y, 0, 0);
            }

            EllipsePoints.Add(ellipse);
            ReferencePoints.Add(new Point(ellipse.Margin.Left + Bias.X, ellipse.Margin.Top + Bias.Y));
            EllipsePoints.Last().DataContextChanged += OnDataContextChanged;
            if (ReferencePoints.Count > 1)
            {
                Point first = ReferencePoints[^2];
                Point second = ReferencePoints[^1];
                ConnectionLines.Add(new Line
                {
                    Stroke = new SolidColorBrush(Colors.Red), X1 = first.X, Y1 = first.Y, X2 = second.X, Y2 = second.Y,
                    IsEnabled = false,
                });

                Background.Children.Add(ConnectionLines.Last());
            }

            if (ReferencePoints.Count == 1) Background.Children.AddRange(InterpolationSegments);
            if (ReferencePoints.Count > 4 && ReferencePoints.Count % 3 == 2)
            {
                for (int i = 0; i < CurveSegmentsCount; i++)
                {
                    var line = new Line { Stroke = new SolidColorBrush(Colors.Transparent), IsEnabled = false };
                    InterpolationSegments.Add(line);
                    Background.Children.Add(line);
                }
            }

            var brush = new SolidColorBrush(color);
            InterpolationSegments.ForEach(x => { x.Stroke = brush; });
            RebuildBezier();
        }

        protected virtual void RebuildBezier()
        {
            if (ReferencePoints.Count < 1) return;
            int stride = 0;
            for (int i = 0; i < ReferencePoints.Count - 1 || (ReferencePoints.Count == 1 && i == 0); i += 3)
            {
                int pointsCount = Math.Min(ReferencePoints.Count - i, 4);
                List<Point> points = ReferencePoints.GetRange(i, pointsCount);
                int j = 0;
                Line firstSegment = InterpolationSegments[stride];
                firstSegment.X1 = ReferencePoints[i].X;
                firstSegment.Y1 = ReferencePoints[i].Y;
                Line lastSegment = InterpolationSegments[stride + CurveSegmentsCount - 1];
                lastSegment.X2 = ReferencePoints[i + pointsCount - 1].X;
                lastSegment.Y2 = ReferencePoints[i + pointsCount - 1].Y;

                for (double t = 0; t < 1; t += Resolution, j++)
                {
                    Line line = InterpolationSegments[stride + j];
                    Line nextLine = InterpolationSegments[stride + j + 1];
                    Point point = Algorithm.GetPoint(t, points);
                    line.X2 = point.X;
                    line.Y2 = point.Y;
                    nextLine.X1 = line.X2;
                    nextLine.Y1 = line.Y2;
                }

                stride += CurveSegmentsCount;
            }
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is not Ellipse ellipse) return;
            int index = EllipsePoints.IndexOf(ellipse);
            if (index == -1) return;
            if (index > 3 && index % 3 == 1)
            {
                var dockingPoint = new Point(ellipse.Margin.Left, ellipse.Margin.Top);
                var margin1 = EllipsePoints[index - 2].Margin;
                var margin2 = EllipsePoints[index - 1].Margin;
                var point1 = new Point(margin1.Left, margin1.Top);
                var point2 = new Point(margin2.Left, margin2.Top);
                var lineSegment = new LineSegment(point1, point2);
                dockingPoint = MathGeometry.NearestPoint(lineSegment, dockingPoint);
                ellipse.Margin = new Thickness(dockingPoint.X, dockingPoint.Y, 0, 0);
            }

            ReferencePoints[index] = new Point(ellipse.Margin.Left + Bias.X, ellipse.Margin.Top + Bias.Y);

            if (ConnectionLines.Count == 0) return;
            if (index == 0)
            {
                ConnectionLines.First().X1 = ReferencePoints[index].X;
                ConnectionLines.First().Y1 = ReferencePoints[index].Y;
            }

            if (index.InRange(1, ReferencePoints.Count - 2))
            {
                ConnectionLines[index - 1].X2 = ReferencePoints[index].X;
                ConnectionLines[index - 1].Y2 = ReferencePoints[index].Y;
                ConnectionLines[index].X1 = ReferencePoints[index].X;
                ConnectionLines[index].Y1 = ReferencePoints[index].Y;
            }

            if (index == ReferencePoints.Count - 1)
            {
                ConnectionLines.Last().X2 = ReferencePoints[index].X;
                ConnectionLines.Last().Y2 = ReferencePoints[index].Y;
            }

            RebuildBezier();
        }

        private void OnRemoved(object sender, IEnumerable<UIElement> elements)
        {
            foreach (UIElement element in elements)
            {
                OnRemovedElement(element);
            }
        }

        private void OnRemovedElement(UIElement element)
        {
            if (element is not Ellipse ellipse) return;
            int index = EllipsePoints.IndexOf(ellipse);
            var neighbors = new List<Line>();
            if (index == -1) return;
            if (index > 0) neighbors.Add(ConnectionLines[index - 1]);
            if (index < ReferencePoints.Count - 1) neighbors.Add(ConnectionLines[index]);
            if (index.InRange(1, ReferencePoints.Count - 2))
            {
                Point first = ReferencePoints[index - 1];
                Point second = ReferencePoints[index + 1];
                ConnectionLines.Insert(index - 1, new Line
                {
                    Stroke = new SolidColorBrush(Colors.Red), X1 = first.X, Y1 = first.Y, X2 = second.X, Y2 = second.Y,
                    IsEnabled = false,
                });

                Background.Children.Add(ConnectionLines[index - 1]);
            }

            neighbors.ForEach(x => ConnectionLines.Remove(x));
            neighbors.ForEach(x => Background.Children.Remove(x));
            EllipsePoints.Remove(ellipse);
            ReferencePoints.RemoveAt(index);
            if (ReferencePoints.Count > 0)
            {
                if (ReferencePoints.Count % 3 == 1)
                {
                    Background.Children.Removed -= OnRemoved;
                    List<UIElement> removedLines =
                        InterpolationSegments.GetRange(InterpolationSegments.Count - CurveSegmentsCount,
                            CurveSegmentsCount).Cast<UIElement>().ToList();
                    InterpolationSegments.RemoveRange(InterpolationSegments.Count - CurveSegmentsCount,
                        CurveSegmentsCount);
                    Background.Children.RemoveRange(removedLines);
                    Background.Children.Removed += OnRemoved;
                }

                for (int i = 0; i < EllipsePoints.Count; i++)
                {
                    if (i % 3 == 1) EllipsePoints[i].DataContext = EllipsePoints[i].Margin;
                }

                return;
            }

            InterpolationSegments.ForEach(x => Background.Children.Remove(x));
            InterpolationSegments.Clear();
            EllipsePoints.Clear();
            ReferencePoints.Clear();
            ConnectionLines.Clear();
        }
    }
}

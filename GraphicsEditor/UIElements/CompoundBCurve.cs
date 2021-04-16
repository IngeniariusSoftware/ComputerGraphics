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

    public class CompoundBCurve : ICurve
    {
        public CompoundBCurve(IPanel background, ICurveAlgorithm algorithm, double resolution)
        {
            Algorithm = algorithm;
            Background = background;
            Background.Children.Removed += OnRemoved;
            EllipsePoints = new List<Ellipse>();
            ReferencePoints = new List<Point>();
            InterpolationSegments = new List<Line>();
            ConnectionLines = new List<Line>();
            Resolution = resolution;
            CurveSegmentsCount = (int)Math.Ceiling(1.0 / Resolution);
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

            if (ReferencePoints.Count > 3)
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
            if (ReferencePoints.Count < 4) return;
            int stride = 0;
            for (int i = 0; i < ReferencePoints.Count - 3; i++)
            {
                List<Point> points = ReferencePoints.GetRange(i, 4);
                Line firstSegment = InterpolationSegments[stride];
                Point firstPoint = Algorithm.GetPoint(0, points);
                firstSegment.X1 = firstPoint.X;
                firstSegment.Y1 = firstPoint.Y;
                Line lastSegment = InterpolationSegments[stride + CurveSegmentsCount - 1];
                Point secondPoint = Algorithm.GetPoint(1, points);
                lastSegment.X2 = secondPoint.X;
                lastSegment.Y2 = secondPoint.Y;
                for (int j = 0; j < CurveSegmentsCount - 1; j++)
                {
                    Line line = InterpolationSegments[stride + j];
                    Line nextLine = InterpolationSegments[stride + j + 1];
                    Point point = Algorithm.GetPoint(Resolution * j, points);
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
                if (ReferencePoints.Count <= 2) return;
                Background.Children.Removed -= OnRemoved;
                List<UIElement> removedLines =
                    InterpolationSegments.GetRange(InterpolationSegments.Count - CurveSegmentsCount,
                        CurveSegmentsCount).Cast<UIElement>().ToList();
                InterpolationSegments.RemoveRange(InterpolationSegments.Count - CurveSegmentsCount,
                    CurveSegmentsCount);
                Background.Children.RemoveRange(removedLines);
                Background.Children.Removed += OnRemoved;
                RebuildBezier();
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

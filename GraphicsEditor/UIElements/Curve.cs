namespace GraphicsEditor.UIElements
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using Extensions;
    using Geometry;

    public class Curve
    {
        public Curve(IPanel background, ICurveAlgorithm algorithm, double resolution)
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
            Size = new Size(10, 10);
            Bias = new Vector(Size.Width / 2, Size.Height / 2);
        }

        public List<Ellipse> EllipsePoints { get; }

        public List<Point> ReferencePoints { get; }

        public List<Line> ConnectionLines { get; }

        public List<Line> InterpolationSegments { get; }

        public double Resolution { get; }

        public Size Size { get; }

        public Vector Bias { get; }

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

            if (ReferencePoints.Count == 1) InterpolationSegments.ForEach(x => Background.Children.Add(x));
            var brush = new SolidColorBrush(color);
            InterpolationSegments.ForEach(x => { x.Stroke = brush; });
            RebuildBezier();
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

        public void OnRemoved(object sender, UIElement element)
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

            Background.Children.Removed -= OnRemoved;
            neighbors.ForEach(x => ConnectionLines.Remove(x));
            neighbors.ForEach(x => Background.Children.Remove(x));
            EllipsePoints.Remove((Ellipse)element);
            ReferencePoints.RemoveAt(index);
            Background.Children.Remove(element);
            if (ReferencePoints.Count > 0)
            {
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

        private void RebuildBezier()
        {
            if (ReferencePoints.Count < 1) return;
            Line firstSegment = InterpolationSegments.First();
            firstSegment.X1 = ReferencePoints.First().X;
            firstSegment.Y1 = ReferencePoints.First().Y;
            Line lastSegment = InterpolationSegments.Last();
            lastSegment.X2 = ReferencePoints.Last().X;
            lastSegment.Y2 = ReferencePoints.Last().Y;

            int i = 0;
            for (double t = 0; t < 1; t += Resolution, i++)
            {
                Line line = InterpolationSegments[i];
                Line nextLine = InterpolationSegments[i + 1];
                Point point = Algorithm.GetPoint(t, ReferencePoints);
                line.X2 = point.X;
                line.Y2 = point.Y;
                nextLine.X1 = line.X2;
                nextLine.Y1 = line.Y2;
            }
        }
    }
}
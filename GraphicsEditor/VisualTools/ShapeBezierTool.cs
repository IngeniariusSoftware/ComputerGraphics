namespace GraphicsEditor.VisualTools
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using Extensions;
    using UIElements;
    using static System.Math;

    public class ShapeBezierTool : ShapeTool
    {
        public ShapeBezierTool(IPanel background, IPanel foreground)
            : base(background, foreground)
        {
            ReferencePoints = new List<Ellipse>();
            InterpolationSegments = new List<Line>();
            ConnectionLines = new List<Line>();
            Resolution = 0.01;
            Background.Children.Removed += OnRemoved;
            for (double i = 0; i < 1; i += Resolution)
            {
                InterpolationSegments.Add(new Line
                    { Stroke = new SolidColorBrush(Colors.Transparent), IsEnabled = false });
            }

            InterpolationSegments.Add(new Line { Stroke = new SolidColorBrush(Colors.Transparent) });
            Size = new Size(10, 10);
            Bias = new Vector(Size.Width / 2, Size.Height / 2);
        }

        protected List<Ellipse> ReferencePoints { get; }

        protected List<Line> ConnectionLines { get; }

        protected List<Line> InterpolationSegments { get; }

        protected double Resolution { get; }

        protected Size Size { get; }

        protected Vector Bias { get; }

        protected override void Drawing(Point currentPoint, Color color)
        {
            base.Drawing(currentPoint, color);
            ReferencePoints.Last().Margin = new Thickness(currentPoint.X, currentPoint.Y, 0, 0);
            ReferencePoints.Last().DataContext = ReferencePoints.Last().Margin;
        }

        protected override Shape GenerateShape(Color color)
        {
            ReferencePoints.Add(new Ellipse
            {
                Margin = new Thickness(StartPoint.X, StartPoint.Y, 0, 0),
                Height = Size.Height,
                Width = Size.Width,
                Fill = new RadialGradientBrush(Colors.Red, Colors.White),
                Stroke = new SolidColorBrush(Colors.Red),
            });

            ReferencePoints.Last().DataContextChanged += OnDataContextChanged;
            if (ReferencePoints.Count > 1)
            {
                Thickness first = ReferencePoints[^2].Margin;
                Thickness second = ReferencePoints[^1].Margin;
                ConnectionLines.Add(new Line
                {
                    Stroke = new SolidColorBrush(Colors.Red), X1 = first.Left + Bias.X, Y1 = first.Top + Bias.Y,
                    X2 = second.Left + Bias.X,
                    Y2 = second.Top + Bias.Y,
                    IsEnabled = false,
                });

                Background.Children.Add(ConnectionLines.Last());
            }

            BuildBezier(color);
            return ReferencePoints.Last();
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            if (ConnectionLines.Count == 0) return;
            if (sender is not Ellipse ellipse) return;
            int index = ReferencePoints.IndexOf(ellipse);
            if (index == -1) return;
            if (index == 0)
            {
                ConnectionLines.First().X1 = ellipse.Margin.Left + Bias.X;
                ConnectionLines.First().Y1 = ellipse.Margin.Top + Bias.Y;
            }

            if (index.InRange(1, ReferencePoints.Count - 2))
            {
                ConnectionLines[index - 1].X2 = ellipse.Margin.Left + Bias.X;
                ConnectionLines[index - 1].Y2 = ellipse.Margin.Top + Bias.Y;
                ConnectionLines[index].X1 = ellipse.Margin.Left + Bias.X;
                ConnectionLines[index].Y1 = ellipse.Margin.Top + Bias.Y;
            }

            if (index == ReferencePoints.Count - 1)
            {
                ConnectionLines.Last().X2 = ellipse.Margin.Left + Bias.X;
                ConnectionLines.Last().Y2 = ellipse.Margin.Top + Bias.Y;
            }

            RebuildBezier();
        }

        private void OnRemoved(object sender, UIElement element)
        {
            if (element is not Ellipse ellipse) return;
            int index = ReferencePoints.IndexOf(ellipse);
            var neighbors = new List<Line>();
            if (index == -1) return;
            if (index > 0) neighbors.Add(ConnectionLines[index - 1]);
            if (index < ReferencePoints.Count - 1) neighbors.Add(ConnectionLines[index]);
            if (index.InRange(1, ReferencePoints.Count - 2))
            {
                Thickness first = ReferencePoints[index - 1].Margin;
                Thickness second = ReferencePoints[index + 1].Margin;
                ConnectionLines.Insert(index - 1, new Line
                {
                    Stroke = new SolidColorBrush(Colors.Red), X1 = first.Left + Bias.X, Y1 = first.Top + Bias.Y,
                    X2 = second.Left + Bias.X,
                    Y2 = second.Top + Bias.Y,
                    IsEnabled = false,
                });

                Background.Children.Add(ConnectionLines[index - 1]);
            }

            Background.Children.Removed -= OnRemoved;
            neighbors.ForEach(x => ConnectionLines.Remove(x));
            neighbors.ForEach(x => Background.Children.Remove(x));
            ReferencePoints.Remove((Ellipse)element);
            Background.Children.Remove(element);
            if (ReferencePoints.Count >= 4)
            {
                Background.Children.Removed += OnRemoved;
                RebuildBezier();
                return;
            }

            InterpolationSegments.ForEach(x => Background.Children.Remove(x));
            Background.Children.Removed += OnRemoved;
        }

        private void BuildBezier(Color color)
        {
            if (ReferencePoints.Count != 4) return;
            var brush = new SolidColorBrush(color);
            InterpolationSegments.ForEach(x =>
            {
                x.Stroke = brush;
                Background.Children.Add(x);
            });

            RebuildBezier();
        }

        private void RebuildBezier()
        {
            if (ReferencePoints.Count < 4) return;
            double[] x = ReferencePoints.Take(4).Select(e => e.Margin.Left + Bias.X).ToArray();
            double[] y = ReferencePoints.Take(4).Select(e => e.Margin.Top + Bias.Y).ToArray();
            Line firstSegment = InterpolationSegments.First();
            firstSegment.X1 = x.First();
            firstSegment.Y1 = y.First();
            Line lastSegment = InterpolationSegments.Last();
            lastSegment.X2 = x.Last();
            lastSegment.Y2 = y.Last();

            int i = 0;
            for (double t = 0; t < 1; t += Resolution, i++)
            {
                Line line = InterpolationSegments[i];
                Line nextLine = InterpolationSegments[i + 1];
                double t0 = Pow(1 - t, 3);
                double t1 = (3 * t) * Pow(1 - t, 2);
                double t2 = (3 * Pow(t, 2)) * (1 - t);
                double t3 = Pow(t, 3);
                line.X2 = (t0 * x[0]) + (t1 * x[1]) + (t2 * x[2]) + (t3 * x[3]);
                line.Y2 = (t0 * y[0]) + (t1 * y[1]) + (t2 * y[2]) + (t3 * y[3]);
                nextLine.X1 = line.X2;
                nextLine.Y1 = line.Y2;
            }
        }
    }
}

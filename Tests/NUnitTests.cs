namespace Tests
{
    using System.Windows;
    using GraphicsEditor.Geometry;
    using NUnit.Framework;

    public class NUnitTests
    {
        [SetUp] public void Setup()
        {
        }

        [Test] public void LineSegmentClampTestAllVisibleUnparallelXY()
        {
            var lineSegment = new LineSegment(new Point(0, 0), new Point(10, 10));
            var rectangle = new Rectangle(new Point(0, 0), new Point(10, 10));
            LineSegment result = MathGeometry.LineSegmentClamp(lineSegment, rectangle);
            Assert.NotNull(result);
            Assert.AreEqual(rectangle.Start, result.Start);
            Assert.AreEqual(rectangle.End, result.End);
        }

        [Test] public void LineSegmentClampTestAllInvisibleParallelX()
        {
            var lineSegment = new LineSegment(new Point(10, 20), new Point(20, 20));
            var rectangle = new Rectangle(new Point(0, 0), new Point(10, 10));
            var result = MathGeometry.LineSegmentClamp(lineSegment, rectangle);
            Assert.AreEqual(null, result);
        }

        [Test] public void LineSegmentClampTestAllOnePartlyVisibleUnparallelXY()
        {
            var lineSegment = new LineSegment(new Point(0, 0), new Point(15, 15));
            var rectangle = new Rectangle(new Point(0, 0), new Point(10, 10));
            var result = MathGeometry.LineSegmentClamp(lineSegment, rectangle);
            Assert.NotNull(result);
            Assert.AreEqual(rectangle.Start, result.Start);
            Assert.AreEqual(rectangle.End, result.End);
        }

        [Test] public void LineSegmentClampTestAllOnePartlyVisibleFromCenterToRightParallelX()
        {
            var lineSegment = new LineSegment(new Point(5, 5), new Point(15, 5));
            var rectangle = new Rectangle(new Point(0, 0), new Point(10, 10));
            var result = MathGeometry.LineSegmentClamp(lineSegment, rectangle);
            Assert.NotNull(result);
            Assert.AreEqual(lineSegment.Start, result.Start);
            Assert.AreEqual(new Point(10, 5), result.End);
        }

        [Test] public void LineSegmentClampTestAllOnePartlyVisibleFromCenterToLeftParallelX()
        {
            var lineSegment = new LineSegment(new Point(5, 5), new Point(-10, 5));
            var rectangle = new Rectangle(new Point(0, 0), new Point(10, 10));
            var result = MathGeometry.LineSegmentClamp(lineSegment, rectangle);
            Assert.NotNull(result);
            Assert.AreEqual(lineSegment.Start, result.Start);
            Assert.AreEqual(new Point(0, 5), result.End);
        }

        [Test] public void LineSegmentClampTestAllOnePartlyVisibleToRightUnparallelX()
        {
            var lineSegment = new LineSegment(new Point(1, 5), new Point(-1, 3));
            var rectangle = new Rectangle(new Point(0, 0), new Point(10, 10));
            var result = MathGeometry.LineSegmentClamp(lineSegment, rectangle);
            Assert.NotNull(result);
            Assert.AreEqual(lineSegment.Start, result.Start);
            Assert.AreEqual(new Point(0, 4), result.End);
        }

        [Test] public void LineSegmentClampTestAllBothPartlyVisibleParallelX()
        {
            var lineSegment = new LineSegment(new Point(-5, 5), new Point(15, 5));
            var rectangle = new Rectangle(new Point(0, 0), new Point(10, 10));
            var result = MathGeometry.LineSegmentClamp(lineSegment, rectangle);
            Assert.NotNull(result);
            Assert.AreEqual(new Point(0, 5), result.Start);
            Assert.AreEqual(new Point(10, 5), result.End);
        }

        [Test] public void IntersectionPointsTestLineX()
        {
            LineSegment lineSegment = new(new Point(-5, 5), new Point(15, 5));
            var rectangle = new Rectangle(new Point(0, 0), new Point(10, 10));
            var result = MathGeometry.IntersectionPoints(lineSegment, rectangle);
            Assert.AreEqual(2, result.Count);
        }

        [Test] public void IntersectionPointsTestLineY()
        {
            var lineSegment = new LineSegment(new Point(5, -5), new Point(5, 15));
            var rectangle = new Rectangle(new Point(0, 0), new Point(10, 10));
            var result = MathGeometry.IntersectionPoints(lineSegment, rectangle);
            Assert.AreEqual(2, result.Count);
        }

        [Test] public void OnePointLineSegment()
        {
            var lineSegment = new LineSegment(new Point(-5, 5), new Point(-5, -5));
            var rectangle = new Rectangle(new Point(0, 0), new Point(10, 10));
            var result = MathGeometry.LineSegmentClamp(lineSegment, rectangle);
            Assert.NotNull(result);
            Assert.AreEqual(new Point(0, 0), result.Start);
            Assert.AreEqual(new Point(0, 0), result.End);
        }

        [Test] public void NullLineSegment()
        {
            var lineSegment = new LineSegment(new Point(-5, 5), new Point(-5, -5));
            var rectangle = new Rectangle(new Point(1, 1), new Point(10, 10));
            var result = MathGeometry.LineSegmentClamp(lineSegment, rectangle);
            Assert.Null(result);
        }
    }
}

namespace GraphicsEditor.VisualTools
{
    using System.Windows;
    using System.Windows.Media;
    using Geometry;

    public class RasterEraserTool : BaseTool
    {
        public RasterEraserTool(IWriteableBitmap background, byte[] backgroundBuffer)
        {
            Background = background;
            BackgroundBuffer = backgroundBuffer;
        }

        protected IWriteableBitmap Background { get; }

        protected byte[] BackgroundBuffer { get; }

        public override void EndDrawing(Point currentPoint, Color color)
        {
            base.EndDrawing(currentPoint, color);
            var size = new Int32Rect(0, 0, Background.PixelWidth, Background.PixelHeight);
            Background.WritePixels(size, BackgroundBuffer, 4 * Background.PixelWidth, 0);
        }
    }
}


namespace Lesson1.Tools
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media.Imaging;

    public class RasterTool : BaseTool
    {
        protected readonly WriteableBitmap Background;

        protected readonly WriteableBitmap Foreground;

        protected readonly byte[] BackgroundBuffer;

        public RasterTool(Image background, Image foreground, byte[] backgroundBuffer)
        {
            Background = (WriteableBitmap)background.Source;
            Foreground = (WriteableBitmap)foreground.Source;
            BackgroundBuffer = backgroundBuffer;
        }

        protected void DrawPoint(
            WriteableBitmap bitmap,
            int x,
            int y,
            byte[] color,
            bool swap = false,
            double intensity = 1)
        {
            byte alpha = color[3];
            color[3] = (byte)(alpha * intensity);
            bitmap.WritePixels(new Int32Rect(swap ? y : x, swap ? x : y, 1, 1), color, 4, 0);
            color[3] = alpha;
        }
    }
}

namespace Benchmarks
{
    using BenchmarkDotNet.Configs;
    using BenchmarkDotNet.Jobs;
    using BenchmarkDotNet.Toolchains.CsProj;
    using BenchmarkDotNet.Toolchains.DotNetCli;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;
    using System.Windows;
    using System.Windows.Media;
    using GraphicsEditor.VisualTools;
    using System.Windows.Media.Imaging;
    using GraphicsEditor.Geometry;

    public class Program
    {
        public static void Main(string[] args)
        {
            var config = DefaultConfig.Instance
                .AddJob(
                    Job.Default.WithToolchain(
                            CsProjCoreToolchain.From(
                                new NetCoreAppSettings("net5.0-windows", // the key to make it work
                                    null,
                                    "5.0")))
                        .AsDefault());

            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, config);
        }
    }

    public class DrawingBenchmark
    {
        public BresenhamLineTool BresenhamLineTool;

        public BresenhamEllipseTool BresenhamEllipseTool;

        public BresenhamCircleTool BresenhamCircleTool;

        public XiaolinWuLineTool XiaolinWuLineTool;

        public int Height = 3000;

        public int Width = 3000;

        public DrawingBenchmark()
        {
            var background = new WriteableBitmap(
                Width,
                Height,
                96,
                96,
                PixelFormats.Bgra32,
                null);
            IWriteableBitmap backBitmap = new VariableSizeWriteableBitmap(background, Width, Height);
            IWriteableBitmap foreBitmap =
                new VariableSizeWriteableBitmap(new WriteableBitmap(background), Width, Height);
            BresenhamLineTool = new BresenhamLineTool(backBitmap, foreBitmap);
            BresenhamLineTool.StartDrawing(new Point(0, 0), Color.FromRgb(0, 0, 0));
            BresenhamEllipseTool = new BresenhamEllipseTool(backBitmap, foreBitmap);
            BresenhamEllipseTool.StartDrawing(new Point(0, 0), Color.FromRgb(0, 0, 0));
            XiaolinWuLineTool = new XiaolinWuLineTool(backBitmap, foreBitmap);
            XiaolinWuLineTool.StartDrawing(new Point(0, 0), Color.FromRgb(0, 0, 0));
            BresenhamCircleTool = new BresenhamCircleTool(backBitmap, foreBitmap);
            BresenhamCircleTool.StartDrawing(new Point((Width / 2) - 10, (Height / 2) - 10), Color.FromRgb(0, 0, 0));
        }

        [Benchmark] public int BresenhamCircleToolDrawing()
        {
            BresenhamCircleTool.TryDrawing(new Point(Width - 10, Height - 10), Color.FromRgb(0, 0, 0));
            return 0;
        }

        [Benchmark] public int BresenhamLineToolDrawing()
        {
            BresenhamLineTool.TryDrawing(new Point(Width - 10, Height - 10), Color.FromRgb(0, 0, 0));
            return 0;
        }

        [Benchmark] public int BresenhamEllipseToolDrawing()
        {
            BresenhamEllipseTool.TryDrawing(new Point(Width - 10, Height - 10), Color.FromRgb(0, 0, 0));
            return 0;
        }


        [Benchmark] public int XiaolinWuLineToolDrawing()
        {
            XiaolinWuLineTool.TryDrawing(new Point(Width - 10, Height - 10), Color.FromRgb(0, 0, 0));
            return 0;
        }
    }
}

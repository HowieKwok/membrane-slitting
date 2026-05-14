using System.Windows;

namespace ConvexPeakWpfDemo;

public partial class App : System.Windows.Application
{
    private void Application_Startup(object sender, StartupEventArgs e)
    {
        ConfigureHalconRuntime();

        if (e.Args.Length > 0 && e.Args[0].Equals("--self-test", StringComparison.OrdinalIgnoreCase))
        {
            RunSelfTest(e.Args);
            Shutdown();
            return;
        }

        MainWindow window = new MainWindow();
        MainWindow = window;
        window.Show();
    }

    private static void ConfigureHalconRuntime()
    {
        const string halconRoot = @"E:\halconPath";
        const string halconArch = "x64-win64";
        string halconBin = System.IO.Path.Combine(halconRoot, "bin", halconArch);

        Environment.SetEnvironmentVariable("HALCONROOT", halconRoot);
        Environment.SetEnvironmentVariable("HALCONARCH", halconArch);

        string currentPath = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
        if (!currentPath.Split(';').Any(p => string.Equals(p.Trim(), halconBin, StringComparison.OrdinalIgnoreCase)))
            Environment.SetEnvironmentVariable("PATH", halconBin + ";" + currentPath);
    }

    private static void RunSelfTest(string[] args)
    {
        string imagePath = args.Length > 1 ? args[1] : @"C:\Users\1\Desktop\qq.jpg";
        string resultPath = System.IO.Path.Combine(Environment.CurrentDirectory, "ConvexPeakWpfDemo_self_test_result.txt");

        try
        {
            VCornerDetectionResult result = HalconVCornerDetector.Detect(
                imagePath,
                new VCornerRoi
                {
                    CenterRow = 2390,
                    CenterCol = 2980,
                    PhiDeg = 0,
                    Length1 = 340,
                    Length2 = 240
                },
                new HalconVCornerOptions());

            double imageDeltaX = result.Point2.Col - result.Point1.Col;
            double imageDeltaY = result.Point2.Row - result.Point1.Row;
            double distanceX = imageDeltaX;
            double distanceY = imageDeltaY;
            double baseAngle = 0;
            if (result.BaseLine != null)
            {
                ImageLine line = result.BaseLine.Value;
                double dirX = line.DirectionCol;
                double dirY = line.DirectionRow;
                double len = Math.Sqrt(dirX * dirX + dirY * dirY);
                if (len > 1e-9)
                {
                    dirX /= len;
                    dirY /= len;
                    if (imageDeltaX * dirX + imageDeltaY * dirY < 0)
                    {
                        dirX = -dirX;
                        dirY = -dirY;
                    }

                    distanceX = imageDeltaX * dirX + imageDeltaY * dirY;
                    distanceY = imageDeltaX * (-dirY) + imageDeltaY * dirX;
                    baseAngle = Math.Atan2(dirY, dirX) * 180.0 / Math.PI;
                }
            }

            double distance = Math.Sqrt(distanceX * distanceX + distanceY * distanceY);

            string text =
                $"Image={imagePath}{Environment.NewLine}" +
                $"HasResult={result.HasResult}{Environment.NewLine}" +
                $"Mode={result.Mode}{Environment.NewLine}" +
                $"P1.Row={result.Point1.Row:F3}{Environment.NewLine}" +
                $"P1.Col={result.Point1.Col:F3}{Environment.NewLine}" +
                $"P2.Row={result.Point2.Row:F3}{Environment.NewLine}" +
                $"P2.Col={result.Point2.Col:F3}{Environment.NewLine}" +
                $"ImageDeltaX.Col={imageDeltaX:F3}{Environment.NewLine}" +
                $"ImageDeltaY.Row={imageDeltaY:F3}{Environment.NewLine}" +
                $"BaseLine.AngleDeg={baseAngle:F3}{Environment.NewLine}" +
                $"DistanceX.ParallelBase.UnitPerPixel1={distanceX:F3}{Environment.NewLine}" +
                $"DistanceY.PerpendicularBase.UnitPerPixel1={distanceY:F3}{Environment.NewLine}" +
                $"DistanceTotal.UnitPerPixel1={distance:F3}{Environment.NewLine}" +
                $"NumUnionContours={result.NumUnionContours}{Environment.NewLine}" +
                $"NumParts={result.NumParts}{Environment.NewLine}";

            System.IO.File.WriteAllText(resultPath, text);
        }
        catch (Exception ex)
        {
            System.IO.File.WriteAllText(resultPath, "SelfTestFailed=" + ex);
        }
    }
}

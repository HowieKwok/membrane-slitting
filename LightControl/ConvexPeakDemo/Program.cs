using System.Globalization;
using System.Text;

namespace ConvexPeakDemo;

internal static class Program
{
    private static void Main()
    {
        string outputDir = Path.Combine(AppContext.BaseDirectory, "output");
        Directory.CreateDirectory(outputDir);

        List<Point2D> rawPoints = DemoData.CreateNoisyContour();

        ConvexPeakOptions options = new ConvexPeakOptions
        {
            ClosedContour = false,
            OutlierRadius = 18,
            OutlierMinNeighbors = 3,
            SmoothWindow = 5,
            DerivativeStep = 4,
            MinCurvature = 0.20,
            MinPeakDistance = 18,
            KeepConcave = false
        };

        ConvexPeakResult result = ConvexPeakFinder.Find(rawPoints, options);

        CsvWriter.WritePoints(Path.Combine(outputDir, "01_raw_points.csv"), rawPoints);
        CsvWriter.WritePoints(Path.Combine(outputDir, "02_filtered_points.csv"), result.FilteredPoints);
        CsvWriter.WritePeaks(Path.Combine(outputDir, "03_convex_peaks.csv"), result.Peaks);
        SvgWriter.Write(Path.Combine(outputDir, "04_result.svg"), rawPoints, result.FilteredPoints, result.Peaks);

        Console.WriteLine("Demo finished.");
        Console.WriteLine("Raw points      : " + rawPoints.Count);
        Console.WriteLine("Filtered points : " + result.FilteredPoints.Count);
        Console.WriteLine("Convex peaks    : " + result.Peaks.Count);
        Console.WriteLine("Output folder   : " + outputDir);
    }
}

public sealed class ConvexPeakOptions
{
    public bool ClosedContour { get; set; }
    public double OutlierRadius { get; set; } = 15;
    public int OutlierMinNeighbors { get; set; } = 2;
    public int SmoothWindow { get; set; } = 5;
    public int DerivativeStep { get; set; } = 3;
    public double MinCurvature { get; set; } = 0.15;
    public double MinPeakDistance { get; set; } = 10;
    public bool KeepConcave { get; set; }
}

public sealed class ConvexPeakResult
{
    public List<Point2D> FilteredPoints { get; init; } = new();
    public List<Point2D> SmoothedPoints { get; init; } = new();
    public List<PeakInfo> Peaks { get; init; } = new();
}

public readonly record struct Point2D(double X, double Y)
{
    public double DistanceTo(Point2D other)
    {
        double dx = X - other.X;
        double dy = Y - other.Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }
}

public sealed class PeakInfo
{
    public int Index { get; init; }
    public Point2D Point { get; init; }
    public double Curvature { get; init; }
}

public static class ConvexPeakFinder
{
    public static ConvexPeakResult Find(IList<Point2D> input, ConvexPeakOptions options)
    {
        if (input == null || input.Count < 5)
            return new ConvexPeakResult();

        List<Point2D> filtered = RemoveIsolatedOutliers(
            input,
            options.OutlierRadius,
            options.OutlierMinNeighbors);

        if (filtered.Count < 5)
            return new ConvexPeakResult { FilteredPoints = filtered };

        List<Point2D> smoothed = SmoothByIntegral(filtered, options.SmoothWindow, options.ClosedContour);
        double[] curvature = CalculateSignedCurvature(smoothed, options.DerivativeStep, options.ClosedContour);
        List<PeakInfo> peaks = PickPeaks(filtered, curvature, options);

        return new ConvexPeakResult
        {
            FilteredPoints = filtered,
            SmoothedPoints = smoothed,
            Peaks = peaks
        };
    }

    private static List<Point2D> RemoveIsolatedOutliers(IList<Point2D> points, double radius, int minNeighbors)
    {
        List<Point2D> kept = new();

        for (int i = 0; i < points.Count; i++)
        {
            int neighborCount = 0;

            for (int j = 0; j < points.Count; j++)
            {
                if (i == j)
                    continue;

                if (points[i].DistanceTo(points[j]) <= radius)
                    neighborCount++;

                if (neighborCount >= minNeighbors)
                    break;
            }

            if (neighborCount >= minNeighbors)
                kept.Add(points[i]);
        }

        return kept;
    }

    private static List<Point2D> SmoothByIntegral(IList<Point2D> points, int window, bool closed)
    {
        window = Math.Max(1, window);

        if (closed)
            return SmoothClosed(points, window);

        int count = points.Count;
        double[] sumX = new double[count + 1];
        double[] sumY = new double[count + 1];

        for (int i = 0; i < count; i++)
        {
            sumX[i + 1] = sumX[i] + points[i].X;
            sumY[i + 1] = sumY[i] + points[i].Y;
        }

        List<Point2D> smoothed = new();
        for (int i = 0; i < count; i++)
        {
            int left = Math.Max(0, i - window);
            int right = Math.Min(count - 1, i + window);
            int n = right - left + 1;

            smoothed.Add(new Point2D(
                (sumX[right + 1] - sumX[left]) / n,
                (sumY[right + 1] - sumY[left]) / n));
        }

        return smoothed;
    }

    private static List<Point2D> SmoothClosed(IList<Point2D> points, int window)
    {
        int count = points.Count;
        List<Point2D> smoothed = new();

        for (int i = 0; i < count; i++)
        {
            double sumX = 0;
            double sumY = 0;
            int n = 0;

            for (int k = -window; k <= window; k++)
            {
                int index = Mod(i + k, count);
                sumX += points[index].X;
                sumY += points[index].Y;
                n++;
            }

            smoothed.Add(new Point2D(sumX / n, sumY / n));
        }

        return smoothed;
    }

    private static double[] CalculateSignedCurvature(IList<Point2D> points, int step, bool closed)
    {
        step = Math.Max(1, step);
        int count = points.Count;
        double[] curvature = new double[count];

        for (int i = 0; i < count; i++)
        {
            int prevIndex = closed ? Mod(i - step, count) : Math.Max(0, i - step);
            int nextIndex = closed ? Mod(i + step, count) : Math.Min(count - 1, i + step);

            if (prevIndex == i || nextIndex == i)
                continue;

            Point2D prev = points[prevIndex];
            Point2D curr = points[i];
            Point2D next = points[nextIndex];

            double vx1 = curr.X - prev.X;
            double vy1 = curr.Y - prev.Y;
            double vx2 = next.X - curr.X;
            double vy2 = next.Y - curr.Y;

            double len1 = Math.Sqrt(vx1 * vx1 + vy1 * vy1);
            double len2 = Math.Sqrt(vx2 * vx2 + vy2 * vy2);

            if (len1 < 1e-6 || len2 < 1e-6)
                continue;

            double cross = vx1 * vy2 - vy1 * vx2;
            double dot = vx1 * vx2 + vy1 * vy2;
            curvature[i] = Math.Atan2(cross, dot);
        }

        return curvature;
    }

    private static List<PeakInfo> PickPeaks(IList<Point2D> points, double[] curvature, ConvexPeakOptions options)
    {
        List<PeakInfo> candidates = new();

        for (int i = 1; i < curvature.Length - 1; i++)
        {
            double value = options.KeepConcave ? Math.Abs(curvature[i]) : curvature[i];

            if (value < options.MinCurvature)
                continue;

            double prev = options.KeepConcave ? Math.Abs(curvature[i - 1]) : curvature[i - 1];
            double next = options.KeepConcave ? Math.Abs(curvature[i + 1]) : curvature[i + 1];

            if (value >= prev && value >= next)
            {
                candidates.Add(new PeakInfo
                {
                    Index = i,
                    Point = points[i],
                    Curvature = curvature[i]
                });
            }
        }

        return candidates
            .OrderByDescending(p => Math.Abs(p.Curvature))
            .Aggregate(new List<PeakInfo>(), (picked, candidate) =>
            {
                bool tooClose = picked.Any(p => p.Point.DistanceTo(candidate.Point) < options.MinPeakDistance);
                if (!tooClose)
                    picked.Add(candidate);

                return picked;
            })
            .OrderBy(p => p.Index)
            .ToList();
    }

    private static int Mod(int value, int modulus)
    {
        int result = value % modulus;
        return result < 0 ? result + modulus : result;
    }
}

internal static class DemoData
{
    public static List<Point2D> CreateNoisyContour()
    {
        List<Point2D> points = new();
        Random random = new(7);

        AddLine(points, new Point2D(70, 380), new Point2D(180, 250), 36, random, 1.8);
        AddLine(points, new Point2D(180, 250), new Point2D(240, 210), 18, random, 1.8);

        points.Add(new Point2D(260, 170));
        points.Add(new Point2D(278, 132));
        points.Add(new Point2D(292, 170));

        AddLine(points, new Point2D(292, 170), new Point2D(355, 215), 22, random, 1.8);
        AddLine(points, new Point2D(355, 215), new Point2D(470, 220), 34, random, 1.8);

        points.Add(new Point2D(150, 120));
        points.Add(new Point2D(315, 90));
        points.Add(new Point2D(520, 160));
        points.Add(new Point2D(235, 330));
        points.Add(new Point2D(410, 340));

        return points;
    }

    private static void AddLine(
        List<Point2D> points,
        Point2D start,
        Point2D end,
        int count,
        Random random,
        double noise)
    {
        for (int i = 0; i < count; i++)
        {
            double t = count == 1 ? 0 : i / (double)(count - 1);
            double x = start.X + (end.X - start.X) * t + (random.NextDouble() - 0.5) * noise;
            double y = start.Y + (end.Y - start.Y) * t + (random.NextDouble() - 0.5) * noise;
            points.Add(new Point2D(x, y));
        }
    }
}

internal static class CsvWriter
{
    public static void WritePoints(string path, IList<Point2D> points)
    {
        StringBuilder sb = new();
        sb.AppendLine("Index,X,Y");

        for (int i = 0; i < points.Count; i++)
            sb.AppendLine(FormattableString.Invariant($"{i},{points[i].X:F3},{points[i].Y:F3}"));

        File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
    }

    public static void WritePeaks(string path, IList<PeakInfo> peaks)
    {
        StringBuilder sb = new();
        sb.AppendLine("Index,X,Y,CurvatureRad,CurvatureDeg");

        foreach (PeakInfo peak in peaks)
        {
            double deg = peak.Curvature * 180.0 / Math.PI;
            sb.AppendLine(FormattableString.Invariant(
                $"{peak.Index},{peak.Point.X:F3},{peak.Point.Y:F3},{peak.Curvature:F6},{deg:F3}"));
        }

        File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
    }
}

internal static class SvgWriter
{
    public static void Write(string path, IList<Point2D> raw, IList<Point2D> filtered, IList<PeakInfo> peaks)
    {
        const int width = 640;
        const int height = 460;

        StringBuilder sb = new();
        sb.AppendLine($"<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"{width}\" height=\"{height}\" viewBox=\"0 0 {width} {height}\">");
        sb.AppendLine("<rect x=\"0\" y=\"0\" width=\"100%\" height=\"100%\" fill=\"#1f1f1f\"/>");
        sb.AppendLine("<text x=\"18\" y=\"28\" fill=\"#e8e8e8\" font-family=\"Consolas\" font-size=\"16\">Convex peak demo: gray=raw, cyan=filtered, red=convex peaks</text>");

        foreach (Point2D point in raw)
            sb.AppendLine(Circle(point, 2.5, "#8f8f8f", 0.85));

        sb.AppendLine($"<polyline points=\"{PolylinePoints(filtered)}\" fill=\"none\" stroke=\"#00d9ff\" stroke-width=\"2\"/>");

        foreach (PeakInfo peak in peaks)
        {
            sb.AppendLine(Circle(peak.Point, 8, "#ff3344", 1));
            sb.AppendLine($"<text x=\"{peak.Point.X + 10:F1}\" y=\"{peak.Point.Y - 10:F1}\" fill=\"#ffeb3b\" font-family=\"Consolas\" font-size=\"13\">{peak.Curvature.ToString("F2", CultureInfo.InvariantCulture)}</text>");
        }

        sb.AppendLine("</svg>");
        File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
    }

    private static string PolylinePoints(IList<Point2D> points)
    {
        return string.Join(" ", points.Select(p => FormattableString.Invariant($"{p.X:F1},{p.Y:F1}")));
    }

    private static string Circle(Point2D p, double r, string color, double opacity)
    {
        return FormattableString.Invariant(
            $"<circle cx=\"{p.X:F1}\" cy=\"{p.Y:F1}\" r=\"{r:F1}\" fill=\"{color}\" opacity=\"{opacity:F2}\"/>");
    }
}

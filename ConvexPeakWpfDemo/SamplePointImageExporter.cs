using System.Globalization;
using System.IO;
using System.Text;

namespace ConvexPeakWpfDemo;

public static class SamplePointImageExporter
{
    public static void ExportDefaultSample(string folder)
    {
        Directory.CreateDirectory(folder);

        List<Point2D> raw = CreateUserSample();
        ConvexPeakOptions options = new ConvexPeakOptions
        {
            ClosedContour = false,
            OutlierRadius = 28,
            OutlierMinNeighbors = 1,
            MinClusterSize = 5,
            SmoothWindow = 4,
            DerivativeStep = 3,
            MinCurvature = 0.18,
            MinPeakDistance = 18,
            KeepConcave = true
        };

        ConvexPeakResult result = ConvexPeakFinder.Find(raw, options);

        WriteCsv(Path.Combine(folder, "sample_points.csv"), raw);
        WritePeaks(Path.Combine(folder, "corner_points.csv"), result.Peaks);
        WriteSvg(Path.Combine(folder, "corner_result.svg"), raw, result.FilteredPoints, result.SmoothedPoints, result.Peaks, result.NoisePoints);
    }

    public static List<Point2D> CreateUserSample()
    {
        return new List<Point2D>
        {
            new(2716.46158973465, 2481.69797343338),
            new(2718.28650076347, 2476.39290491595),
            new(2718.36207870322, 2469.03439266236),
            new(2718.21485858738, 2462.53905061479),
            new(2718.16632975762, 2455.04270382981),
            new(2718.10428110909, 2446.53134176491),
            new(2718.24690373486, 2440.3579026935),
            new(2718.26338938312, 2432.93376180145),
            new(2718.4153937577, 2426.77074221768),
            new(2718.37092994854, 2418.27891009545),
            new(2718.44413600831, 2410.91776360229),
            new(2718.24533193665, 2404.36513176781),
            new(2718.29109529088, 2395.97350706242),
            new(2718.27040381199, 2388.5080767877),
            new(2718.17466506241, 2382.06991058978),
            new(2718.2021882074, 2374.65806207929),
            new(2718.3143833044, 2367.45130116058),
            new(2718.30971726926, 2360.00316359774),
            new(2718.28492710969, 2352.53318127695),
            new(2718.39980222012, 2346.32892546776),
            new(2718.41133114244, 2337.89994594136),
            new(2718.34226640361, 2331.49073786608),
            new(2718.42572207246, 2324.14097471703),
            new(2719.93468547615, 2318.37439829827),
            new(2725.71436602495, 2317.46154630299),
            new(2732.40147569169, 2317.44588392759),
            new(2737.76988693199, 2315.9656585765),
            new(2741.37457017069, 2313.6372273489),
            new(2745.75558987982, 2310.06039260627),
            new(2748.49701420789, 2306.77321522899),
            new(2751.26425482955, 2302.40409723564),
            new(2752.7844608645, 2296.65000702385),
            new(2752.85294560945, 2289.39422949418),
            new(2754.04569846931, 2283.27646568812),
            new(2759.58049972592, 2281.98103517148),
            new(2765.1978954926, 2281.88794766622),
            new(2733.58010374357, 2238.33038239917),
            new(2778.43211109155, 2281.81174553056),
            new(2785.56383838598, 2282.28988105678),
            new(2741.78813849608, 2226.22959085434),
            new(2741.4862567784, 2218.56247969619),
            new(2742.22952229501, 2211.94550962147),
            new(2811.06735946476, 2282.06583553148),
            new(2817.85265953384, 2282.15922464576),
            new(2825.38925526817, 2282.08701212819),
            new(2831.07483809733, 2282.06965402787),
            new(2837.88849625199, 2282.19453798689),
            new(2845.33258552023, 2282.01958663038),
            new(2851.36510738828, 2282.38754336853),
            new(2857.96592034564, 2282.2760387877),
            new(2863.71217749759, 2282.3260663496),
            new(2871.39030457359, 2282.41104031133),
            new(2877.97311894678, 2282.27954627768),
            new(2883.7022993225, 2282.31060815819),
            new(2890.22694430341, 2282.11451046948),
            new(2896.46741418363, 2281.71341687238),
            new(2903.58027543504, 2282.17059953512),
            new(2910.30789084448, 2282.19992334449)
        };
    }

    private static void WriteCsv(string path, IList<Point2D> points)
    {
        StringBuilder sb = new();
        sb.AppendLine("X,Y");

        foreach (Point2D point in points)
            sb.AppendLine(FormattableString.Invariant($"{point.X:F8},{point.Y:F8}"));

        File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
    }

    private static void WritePeaks(string path, IList<PeakInfo> peaks)
    {
        StringBuilder sb = new();
        sb.AppendLine("Index,X,Y,CurvatureRad,CurvatureDeg");

        foreach (PeakInfo peak in peaks)
        {
            double deg = peak.Curvature * 180.0 / Math.PI;
            sb.AppendLine(FormattableString.Invariant($"{peak.Index},{peak.Point.X:F8},{peak.Point.Y:F8},{peak.Curvature:F6},{deg:F3}"));
        }

        File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
    }

    private static void WriteSvg(
        string path,
        IList<Point2D> raw,
        IList<Point2D> filtered,
        IList<Point2D> smooth,
        IList<PeakInfo> peaks,
        IList<Point2D> noise)
    {
        const int width = 1000;
        const int height = 760;
        const int pad = 60;

        List<Point2D> all = raw.Concat(filtered).Concat(smooth).ToList();
        double minX = all.Min(p => p.X);
        double maxX = all.Max(p => p.X);
        double minY = all.Min(p => p.Y);
        double maxY = all.Max(p => p.Y);
        double scale = Math.Min((width - pad * 2.0) / Math.Max(1, maxX - minX), (height - pad * 2.0) / Math.Max(1, maxY - minY));

        (double x, double y) Map(Point2D p)
        {
            double x = pad + (p.X - minX) * scale;
            double y = height - pad - (p.Y - minY) * scale;
            return (x, y);
        }

        StringBuilder sb = new();
        sb.AppendLine($"<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"{width}\" height=\"{height}\" viewBox=\"0 0 {width} {height}\">");
        sb.AppendLine("<rect width=\"100%\" height=\"100%\" fill=\"#161616\"/>");
        sb.AppendLine("<text x=\"24\" y=\"34\" fill=\"#eeeeee\" font-family=\"Consolas\" font-size=\"18\">gray=raw, purple=noise, cyan=filtered, yellow=smooth, red=corner</text>");
        sb.AppendLine($"<polyline points=\"{ToPoints(filtered, Map)}\" fill=\"none\" stroke=\"#00d7ff\" stroke-width=\"3\" opacity=\"0.95\"/>");
        sb.AppendLine($"<polyline points=\"{ToPoints(smooth, Map)}\" fill=\"none\" stroke=\"#ffd23f\" stroke-width=\"3\" opacity=\"0.95\"/>");

        foreach (Point2D point in raw)
            sb.AppendLine(Circle(Map(point), 3, "#9a9a9a", 0.78));

        foreach (Point2D point in noise)
        {
            (double x, double y) = Map(point);
            sb.AppendLine(Circle((x, y), 8, "#9b5cff", 0.95));
            sb.AppendLine($"<circle cx=\"{x:F1}\" cy=\"{y:F1}\" r=\"13\" fill=\"none\" stroke=\"#d7c1ff\" stroke-width=\"2\"/>");
        }

        int no = 1;
        foreach (PeakInfo peak in peaks)
        {
            (double x, double y) = Map(peak.Point);
            sb.AppendLine(Circle((x, y), 9, "#ff2f3f", 1));
            sb.AppendLine($"<text x=\"{x + 12:F1}\" y=\"{y - 12:F1}\" fill=\"#ffffff\" font-family=\"Consolas\" font-size=\"16\">P{no} K={peak.Curvature.ToString("F2", CultureInfo.InvariantCulture)}</text>");
            no++;
        }

        sb.AppendLine("</svg>");
        File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
    }

    private static string ToPoints(IList<Point2D> points, Func<Point2D, (double x, double y)> map)
    {
        return string.Join(" ", points.Select(p =>
        {
            (double x, double y) = map(p);
            return FormattableString.Invariant($"{x:F1},{y:F1}");
        }));
    }

    private static string Circle((double x, double y) p, double r, string color, double opacity)
    {
        return FormattableString.Invariant($"<circle cx=\"{p.x:F1}\" cy=\"{p.y:F1}\" r=\"{r:F1}\" fill=\"{color}\" opacity=\"{opacity:F2}\"/>");
    }
}

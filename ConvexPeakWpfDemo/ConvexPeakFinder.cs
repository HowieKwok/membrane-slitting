namespace ConvexPeakWpfDemo;

public static class ConvexPeakFinder
{
    public static ConvexPeakResult Find(IList<Point2D> input, ConvexPeakOptions options)
    {
        if (input == null || input.Count < 5)
            return new ConvexPeakResult();

        NoiseFilterResult noiseResult = RemoveSpatialOutliers(
            input,
            options.OutlierRadius,
            options.OutlierMinNeighbors,
            options.MinClusterSize);
        List<Point2D> filtered = noiseResult.KeptPoints;

        if (filtered.Count < 5)
            return new ConvexPeakResult { FilteredPoints = filtered, NoisePoints = noiseResult.NoisePoints };

        List<Point2D> smoothed = SmoothByIntegral(filtered, options.SmoothWindow, options.ClosedContour);
        double[] curvature = CalculateSignedCurvature(smoothed, options.DerivativeStep, options.ClosedContour);
        List<PeakInfo> curvaturePeaks = PickPeaks(filtered, curvature, options);
        List<PeakInfo> cornerPeaks = PickCornersByRdp(filtered, curvature, options);
        List<PeakInfo> peaks = cornerPeaks.Count > 0 ? cornerPeaks : curvaturePeaks;

        return new ConvexPeakResult
        {
            FilteredPoints = filtered,
            NoisePoints = noiseResult.NoisePoints,
            SmoothedPoints = smoothed,
            Curvatures = curvature,
            Peaks = peaks
        };
    }

    private static NoiseFilterResult RemoveSpatialOutliers(IList<Point2D> points, double radius, int minNeighbors, int minClusterSize)
    {
        int count = points.Count;
        bool[] core = new bool[count];
        List<int>[] neighbors = new List<int>[count];

        for (int i = 0; i < count; i++)
        {
            neighbors[i] = new List<int>();

            for (int j = 0; j < count; j++)
            {
                if (i == j)
                    continue;

                if (points[i].DistanceTo(points[j]) <= radius)
                    neighbors[i].Add(j);
            }

            core[i] = neighbors[i].Count >= minNeighbors;
        }

        int[] labels = Enumerable.Repeat(-1, count).ToArray();
        int clusterId = 0;

        for (int i = 0; i < count; i++)
        {
            if (!core[i] || labels[i] >= 0)
                continue;

            Queue<int> queue = new();
            queue.Enqueue(i);
            labels[i] = clusterId;

            while (queue.Count > 0)
            {
                int current = queue.Dequeue();

                foreach (int next in neighbors[current])
                {
                    if (labels[next] >= 0)
                        continue;

                    labels[next] = clusterId;

                    if (core[next])
                        queue.Enqueue(next);
                }
            }

            clusterId++;
        }

        Dictionary<int, int> clusterCounts = labels
            .Where(label => label >= 0)
            .GroupBy(label => label)
            .ToDictionary(group => group.Key, group => group.Count());

        List<Point2D> kept = new();
        List<Point2D> noise = new();

        for (int i = 0; i < count; i++)
        {
            int label = labels[i];
            bool keep = label >= 0 && clusterCounts.TryGetValue(label, out int size) && size >= minClusterSize;

            if (keep)
                kept.Add(points[i]);
            else
                noise.Add(points[i]);
        }

        return new NoiseFilterResult(kept, noise);
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

    private static List<PeakInfo> PickCornersByRdp(IList<Point2D> points, double[] curvature, ConvexPeakOptions options)
    {
        if (points.Count < 3)
            return new List<PeakInfo>();

        List<int> keep = new() { 0, points.Count - 1 };
        Rdp(points, 0, points.Count - 1, Math.Max(1, options.RdpTolerance), keep);

        return keep
            .Distinct()
            .Where(i => i > 0 && i < points.Count - 1)
            .OrderBy(i => i)
            .Select(i => new PeakInfo
            {
                Index = i,
                Point = points[i],
                Curvature = i < curvature.Length ? curvature[i] : 0
            })
            .Where(p => Math.Abs(TurnAngle(points, p.Index)) >= options.MinCurvature)
            .ToList();
    }

    private static void Rdp(IList<Point2D> points, int start, int end, double tolerance, List<int> keep)
    {
        if (end <= start + 1)
            return;

        double maxDistance = -1;
        int maxIndex = -1;

        for (int i = start + 1; i < end; i++)
        {
            double distance = DistancePointToLine(points[i], points[start], points[end]);
            if (distance > maxDistance)
            {
                maxDistance = distance;
                maxIndex = i;
            }
        }

        if (maxDistance < tolerance || maxIndex < 0)
            return;

        keep.Add(maxIndex);
        Rdp(points, start, maxIndex, tolerance, keep);
        Rdp(points, maxIndex, end, tolerance, keep);
    }

    private static double DistancePointToLine(Point2D point, Point2D a, Point2D b)
    {
        double dx = b.X - a.X;
        double dy = b.Y - a.Y;
        double length = Math.Sqrt(dx * dx + dy * dy);

        if (length < 1e-6)
            return point.DistanceTo(a);

        return Math.Abs(dy * point.X - dx * point.Y + b.X * a.Y - b.Y * a.X) / length;
    }

    private static double TurnAngle(IList<Point2D> points, int index)
    {
        Point2D prev = points[index - 1];
        Point2D curr = points[index];
        Point2D next = points[index + 1];

        double vx1 = curr.X - prev.X;
        double vy1 = curr.Y - prev.Y;
        double vx2 = next.X - curr.X;
        double vy2 = next.Y - curr.Y;
        double cross = vx1 * vy2 - vy1 * vx2;
        double dot = vx1 * vx2 + vy1 * vy2;

        return Math.Atan2(cross, dot);
    }

    private static int Mod(int value, int modulus)
    {
        int result = value % modulus;
        return result < 0 ? result + modulus : result;
    }
}

internal sealed record NoiseFilterResult(List<Point2D> KeptPoints, List<Point2D> NoisePoints);

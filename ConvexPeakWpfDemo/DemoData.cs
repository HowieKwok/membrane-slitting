namespace ConvexPeakWpfDemo;

public static class DemoData
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

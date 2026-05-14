using System.Globalization;
using System.IO;

namespace ConvexPeakWpfDemo;

public static class CsvPointReader
{
    public static List<Point2D> Read(string path)
    {
        List<Point2D> points = new();

        foreach (string line in File.ReadLines(path))
        {
            string text = line.Trim();
            if (string.IsNullOrWhiteSpace(text))
                continue;

            string[] parts = text.Split(',', ';', '\t', ' ');
            List<double> values = new();

            foreach (string part in parts)
            {
                if (double.TryParse(part.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out double value) ||
                    double.TryParse(part.Trim(), NumberStyles.Float, CultureInfo.CurrentCulture, out value))
                {
                    values.Add(value);
                }
            }

            if (values.Count >= 2)
                points.Add(new Point2D(values[0], values[1]));
        }

        return points;
    }
}

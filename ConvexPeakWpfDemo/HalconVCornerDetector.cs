using HalconDotNet;
using System.IO;
using System.Windows.Shapes;

namespace ConvexPeakWpfDemo;

public static class HalconVCornerDetector
{
    public static void SaveTemplateFromRoi(string imagePath, VCornerRoi roi, string templatePath)
    {
        if (string.IsNullOrWhiteSpace(imagePath) || !File.Exists(imagePath))
            throw new FileNotFoundException("Image path does not exist.", imagePath);
        if (string.IsNullOrWhiteSpace(templatePath))
            throw new ArgumentException("Template path is empty.", nameof(templatePath));

        string? directory = System.IO.Path.GetDirectoryName(templatePath);
        if (!string.IsNullOrWhiteSpace(directory))
            Directory.CreateDirectory(directory);

        HObject? image = null;
        HObject? templateImage = null;

        try
        {
            HOperatorSet.ReadImage(out image, imagePath);
            HOperatorSet.CropRectangle2(
                image,
                out templateImage,
                roi.CenterRow,
                roi.CenterCol,
                roi.PhiRad,
                roi.Length1,
                roi.Length2,
                "true",
                "nearest_neighbor");

            HOperatorSet.WriteImage(templateImage, "png", 0, templatePath);
        }
        catch (HalconException ex)
        {
            throw new InvalidOperationException("HALCON template image creation failed: " + ex.GetErrorMessage(), ex);
        }
        finally
        {
            templateImage?.Dispose();
            image?.Dispose();
        }
    }

    public static ShapeMatchResult LocateShapeModel(string imagePath, string templatePath, ShapeMatchOptions options)
    {
        if (string.IsNullOrWhiteSpace(imagePath) || !File.Exists(imagePath))
            throw new FileNotFoundException("Image path does not exist.", imagePath);
        if (string.IsNullOrWhiteSpace(templatePath) || !File.Exists(templatePath))
            throw new FileNotFoundException("Template image path does not exist.", templatePath);

        HObject? image = null;
        HObject? grayImage = null;
        HObject? templateImage = null;
        HObject? templateGray = null;
        HTuple modelId = new();

        try
        {
            HOperatorSet.ReadImage(out image, imagePath);
            HOperatorSet.ReadImage(out templateImage, templatePath);
            ConvertToGray(image, out grayImage);
            ConvertToGray(templateImage, out templateGray);

            // 调试工具里每次点击创建模型；正式产线建议建模后缓存 ModelID。
            HOperatorSet.CreateShapeModel(
                templateGray,
                "auto",
                DegToRad(options.AngleStartDeg),
                DegToRad(options.AngleExtentDeg),
                "auto",
                "auto",
                "use_polarity",
                "auto",
                "auto",
                out modelId);

            HOperatorSet.FindShapeModel(
                grayImage,
                modelId,
                DegToRad(options.AngleStartDeg),
                DegToRad(options.AngleExtentDeg),
                options.MinScore,
                1,
                options.MaxOverlap,
                "least_squares",
                0,
                options.Greediness,
                out HTuple rows,
                out HTuple cols,
                out HTuple angles,
                out HTuple scores);

            if (scores.Length <= 0)
                return new ShapeMatchResult { Found = false };

            return new ShapeMatchResult
            {
                Found = true,
                Row = rows[0].D,
                Col = cols[0].D,
                AngleDeg = RadToDeg(angles[0].D),
                Score = scores[0].D
            };
        }
        catch (HalconException ex)
        {
            throw new InvalidOperationException("HALCON template matching failed: " + ex.GetErrorMessage(), ex);
        }
        finally
        {
            if (modelId.Length > 0)
                HOperatorSet.ClearShapeModel(modelId);
            templateGray?.Dispose();
            templateImage?.Dispose();
            grayImage?.Dispose();
            image?.Dispose();
        }
    }

    public static VCornerDetectionResult Detect(string imagePath, VCornerRoi roi, HalconVCornerOptions options)
    {
        if (string.IsNullOrWhiteSpace(imagePath) || !File.Exists(imagePath))
            throw new FileNotFoundException("Image path does not exist.", imagePath);

        HObject? image = null;
        HObject? grayImage = null;
        HObject? searchRoi = null;
        HObject? imageRoi = null;
        HObject? edges = null;
        HObject? selectedEdges = null;
        HObject? unionContours = null;
        HObject? mainContour = null;
        HObject? mainContourSmooth = null;
        HObject? contourParts = null;
        HObject? bestFeatureContour = null;

        try
        {
           
            HOperatorSet.ReadImage(out image, imagePath);
            ConvertToGray(image, out grayImage);
            HOperatorSet.CountSeconds(out HTuple start);
     
            // ROI 可以来自鼠标框选、右侧数值框，也可以来自模板匹配输出。
            // 后续只分析 ROI 内轮廓几何，不按左/右/上/下方向硬编码。
            HOperatorSet.GenRectangle2(out searchRoi, roi.CenterRow, roi.CenterCol, roi.PhiRad, roi.Length1, roi.Length2);
            HOperatorSet.ReduceDomain(grayImage, searchRoi, out imageRoi);

            HOperatorSet.EdgesSubPix(imageRoi, out edges, "canny", options.CannyAlpha, options.CannyLow, options.CannyHigh);
            HOperatorSet.SelectContoursXld(edges, out selectedEdges, "contour_length", options.MinContourLength, 999999, -0.5, 0.5);
            HOperatorSet.UnionAdjacentContoursXld(selectedEdges, out unionContours, options.UnionMaxDistAbs, options.UnionMaxDistRel, "attr_keep");
            HOperatorSet.CountObj(unionContours, out HTuple numUnionContoursTuple);
            int numUnionContours = numUnionContoursTuple.I;

            VCornerDetectionResult result = new()
            {
                NumUnionContours = numUnionContours,
                Mode = "none"
            };

            if (numUnionContours <= 0)
                return result;

            int mainIndex = SelectLongestContourIndex(unionContours);
            HOperatorSet.SelectObj(unionContours, out mainContour, mainIndex);
            HOperatorSet.SmoothContoursXld(mainContour, out mainContourSmooth, 7);

            result.MainContour = ReadContourPoints(mainContourSmooth);

         

            // 优先让 HALCON 把主轮廓拆成直线/圆弧，取圆弧过渡段两端作为交点。
            HOperatorSet.SegmentContoursXld(
                mainContourSmooth,
                out contourParts,
                "lines_circles",
                options.SegmentSmooth,
                options.SegmentMaxLineDist1,
                options.SegmentMaxLineDist2);

            HOperatorSet.CountObj(contourParts, out HTuple numPartsTuple);
            result.NumParts = numPartsTuple.I;
            result.ContourParts = ReadContourList(contourParts);
            result.BaseLine = FindBaseLine(result.ContourParts);

            if (TryFindBestArc(contourParts, options, out int bestArcIndex))
            {
                HOperatorSet.SelectObj(contourParts, out bestFeatureContour, bestArcIndex);
                List<ImagePoint> featurePoints = ReadContourPoints(bestFeatureContour);
                if (featurePoints.Count >= 2 && EndPointDistance(featurePoints[0], featurePoints[^1]) >= options.MinPointDistance)
                {
                    result.FeatureContour = featurePoints;
                    result.Point1 = featurePoints[0];
                    result.Point2 = featurePoints[^1];
                    result.HasResult = true;
                    result.Mode = "lines_circles_arc";
                    HOperatorSet.CountSeconds(out HTuple end);
                    end = (end - start) * 1000;
                    Console.WriteLine(end.D.ToString());
                    return result;
                }
            }

            // 非标准圆弧时，用方向无关的局部切线角变化连续段兜底。
            List<ImagePoint> fallbackFeature = FindCurvatureRun(result.MainContour, options);
            if (fallbackFeature.Count >= 2 && EndPointDistance(fallbackFeature[0], fallbackFeature[^1]) >= options.MinPointDistance)
            {
                result.FeatureContour = fallbackFeature;
                result.Point1 = fallbackFeature[0];
                result.Point2 = fallbackFeature[^1];
                result.HasResult = true;
                result.Mode = "curvature_run";
            }
            else
            {
                List<ImagePoint> tangentFeature = FindTangentDepartureRun(result.MainContour, options);
                if (tangentFeature.Count >= 2 && EndPointDistance(tangentFeature[0], tangentFeature[^1]) >= options.MinPointDistance)
                {
                    result.FeatureContour = tangentFeature;
                    result.Point1 = tangentFeature[0];
                    result.Point2 = tangentFeature[^1];
                    result.HasResult = true;
                    result.Mode = "tangent_line_departure";
                }
                else
                {
                    result.Mode = "no_stable_arc_curvature_or_tangent_departure";
                }
            }
            //HOperatorSet.CountSeconds(out HTuple end);
            //end = (end - start) * 1000;
            //Console.WriteLine(end.D.ToString());
            return result;
        }
        catch (HalconException ex)
        {
            throw new InvalidOperationException("HALCON detection failed: " + ex.GetErrorMessage(), ex);
        }
        finally
        {
            bestFeatureContour?.Dispose();
            contourParts?.Dispose();
            mainContourSmooth?.Dispose();
            mainContour?.Dispose();
            unionContours?.Dispose();
            selectedEdges?.Dispose();
            edges?.Dispose();
            imageRoi?.Dispose();
            searchRoi?.Dispose();
            grayImage?.Dispose();
            image?.Dispose();
        }
    }

    private static void ConvertToGray(HObject source, out HObject gray)
    {
        HOperatorSet.CountChannels(source, out HTuple channels);
        if (channels.I == 1)
            HOperatorSet.CopyImage(source, out gray);
        else
            HOperatorSet.Rgb1ToGray(source, out gray);
    }

    private static int SelectLongestContourIndex(HObject contours)
    {
        HOperatorSet.LengthXld(contours, out HTuple lengths);
        int bestIndex = 1;
        double bestLength = double.MinValue;

        for (int i = 0; i < lengths.Length; i++)
        {
            double length = lengths[i].D;
            if (length > bestLength)
            {
                bestLength = length;
                bestIndex = i + 1;
            }
        }

        return bestIndex;
    }

    private static bool TryFindBestArc(HObject contourParts, HalconVCornerOptions options, out int bestArcIndex)
    {
        bestArcIndex = -1;
        double bestScore = -1;

        HOperatorSet.CountObj(contourParts, out HTuple numPartsTuple);
        int numParts = numPartsTuple.I;

        for (int partIndex = 1; partIndex <= numParts; partIndex++)
        {
            HObject? part = null;
            try
            {
                HOperatorSet.SelectObj(contourParts, out part, partIndex);
                HOperatorSet.GetContourGlobalAttribXld(part, "cont_approx", out HTuple contApprox);
                if (contApprox.Length == 0 || contApprox.D <= 0)
                    continue;

                HOperatorSet.LengthXld(part, out HTuple arcLengthTuple);
                double arcLength = arcLengthTuple.D;
                HOperatorSet.FitCircleContourXld(
                    part,
                    "ahuber",
                    -1,
                    2,
                    0,
                    3,
                    2,
                    out _,
                    out _,
                    out HTuple radiusTuple,
                    out _,
                    out _,
                    out _);

                double radius = Math.Abs(radiusTuple.D);
                bool isGoodArc =
                    arcLength >= options.MinArcLength &&
                    radius >= options.MinArcRadius &&
                    radius <= options.MaxArcRadius;

                if (isGoodArc && arcLength > bestScore)
                {
                    bestScore = arcLength;
                    bestArcIndex = partIndex;
                }
            }
            catch (HalconException)
            {
                // 某些短分段没有稳定圆弧属性，跳过即可。
            }
            finally
            {
                part?.Dispose();
            }
        }

        return bestArcIndex > 0;
    }

    private static List<ImagePoint> FindCurvatureRun(IReadOnlyList<ImagePoint> contour, HalconVCornerOptions options)
    {
        int step = Math.Max(1, options.CurvatureStep);
        if (contour.Count <= 2 * step + 1)
            return new List<ImagePoint>();

        double threshold = DegToRad(options.CurvatureAngleThresholdDeg);
        int minRunPoints = Math.Max(1, options.MinCurveRunPoints);

        int bestStart = -1;
        int bestEnd = -1;
        double bestScore = -1;
        bool inRun = false;
        int runStart = -1;
        int runEnd = -1;
        int runCount = 0;
        double runScore = 0;

        for (int i = step; i < contour.Count - step; i++)
        {
            ImagePoint prev = contour[i - step];
            ImagePoint now = contour[i];
            ImagePoint next = contour[i + step];

            double angle1 = Math.Atan2(now.Row - prev.Row, now.Col - prev.Col);
            double angle2 = Math.Atan2(next.Row - now.Row, next.Col - now.Col);
            double curvature = Math.Abs(NormalizeAngle(angle2 - angle1));

            if (curvature >= threshold)
            {
                if (!inRun)
                {
                    inRun = true;
                    runStart = i;
                    runScore = 0;
                    runCount = 0;
                }

                runEnd = i;
                runScore += curvature;
                runCount++;
            }
            else if (inRun)
            {
                CommitRun();
                inRun = false;
            }
        }

        if (inRun)
            CommitRun();

        if (bestStart < 0 || bestEnd <= bestStart)
            return new List<ImagePoint>();

        List<ImagePoint> feature = new();
        for (int i = bestStart; i <= bestEnd; i++)
            feature.Add(contour[i]);

        return feature;

        void CommitRun()
        {
            if (runCount >= minRunPoints && runScore > bestScore)
            {
                bestStart = runStart;
                bestEnd = runEnd;
                bestScore = runScore;
            }
        }
    }

    private static List<ImagePoint> FindTangentDepartureRun(IReadOnlyList<ImagePoint> contour, HalconVCornerOptions options)
    {
        int fitCount = Math.Max(8, Math.Min(options.TangentFitPoints, contour.Count / 3));
        if (contour.Count < fitCount * 2 + 5)
            return new List<ImagePoint>();

        ImageLine startLine = FitLine(contour.Take(fitCount).ToList());
        ImageLine endLine = FitLine(contour.Skip(contour.Count - fitCount).ToList());
        double threshold = Math.Max(0.5, options.TangentDepartureDistance);

        int startIndex = -1;
        int consecutive = 0;
        for (int i = fitCount; i < contour.Count - fitCount; i++)
        {
            if (DistanceToLine(contour[i], startLine) >= threshold)
            {
                consecutive++;
                if (consecutive >= 3)
                {
                    startIndex = Math.Max(fitCount, i - consecutive + 1);
                    break;
                }
            }
            else
            {
                consecutive = 0;
            }
        }

        int endIndex = -1;
        consecutive = 0;
        for (int i = contour.Count - fitCount - 1; i >= fitCount; i--)
        {
            if (DistanceToLine(contour[i], endLine) >= threshold)
            {
                consecutive++;
                if (consecutive >= 3)
                {
                    endIndex = Math.Min(contour.Count - fitCount - 1, i + consecutive - 1);
                    break;
                }
            }
            else
            {
                consecutive = 0;
            }
        }

        if (startIndex < 0 || endIndex < 0 || endIndex <= startIndex)
            return new List<ImagePoint>();

        List<ImagePoint> feature = new();
        for (int i = startIndex; i <= endIndex; i++)
            feature.Add(contour[i]);

        return feature;
    }

    private static double DistanceToLine(ImagePoint point, ImageLine line)
    {
        double dr = point.Row - line.Point.Row;
        double dc = point.Col - line.Point.Col;
        return Math.Abs(dc * line.DirectionRow - dr * line.DirectionCol);
    }

    private static double NormalizeAngle(double angle)
    {
        while (angle > Math.PI)
            angle -= Math.PI * 2;
        while (angle < -Math.PI)
            angle += Math.PI * 2;
        return angle;
    }

    private static List<List<ImagePoint>> ReadContourList(HObject contours)
    {
        List<List<ImagePoint>> all = new();
        HOperatorSet.CountObj(contours, out HTuple countTuple);
        int count = countTuple.I;

        for (int index = 1; index <= count; index++)
        {
            HObject? one = null;
            try
            {
                HOperatorSet.SelectObj(contours, out one, index);
                List<ImagePoint> points = ReadContourPoints(one);
                if (points.Count > 1)
                    all.Add(points);
            }
            finally
            {
                one?.Dispose();
            }
        }

        return all;
    }

    private static ImageLine? FindBaseLine(IReadOnlyList<List<ImagePoint>> contourParts)
    {
        List<ImagePoint>? best = null;
        double bestLength = -1;

        foreach (List<ImagePoint> part in contourParts)
        {
            if (part.Count < 2)
                continue;

            double length = PolylineLength(part);
            double straightness = EndPointDistance(part[0], part[^1]) / Math.Max(1e-9, length);
            if (straightness < 0.92)
                continue;

            if (length > bestLength)
            {
                best = part;
                bestLength = length;
            }
        }

        if (best == null)
            return null;

        return FitLine(best);
    }

    private static ImageLine FitLine(IReadOnlyList<ImagePoint> points)
    {
        double meanRow = points.Average(p => p.Row);
        double meanCol = points.Average(p => p.Col);
        double srr = 0;
        double src = 0;
        double scc = 0;

        foreach (ImagePoint point in points)
        {
            double dr = point.Row - meanRow;
            double dc = point.Col - meanCol;
            srr += dr * dr;
            src += dr * dc;
            scc += dc * dc;
        }

        double theta = 0.5 * Math.Atan2(2 * src, scc - srr);
        double directionCol = Math.Cos(theta);
        double directionRow = Math.Sin(theta);

        ImagePoint start = points[0];
        ImagePoint end = points[^1];
        double dot = (end.Col - start.Col) * directionCol + (end.Row - start.Row) * directionRow;
        if (dot < 0)
        {
            directionCol = -directionCol;
            directionRow = -directionRow;
        }

        return new ImageLine(new ImagePoint(meanRow, meanCol), directionRow, directionCol, PolylineLength(points));
    }

    private static double PolylineLength(IReadOnlyList<ImagePoint> points)
    {
        double length = 0;
        for (int i = 1; i < points.Count; i++)
            length += EndPointDistance(points[i - 1], points[i]);
        return length;
    }

    private static double EndPointDistance(ImagePoint a, ImagePoint b)
    {
        double dr = b.Row - a.Row;
        double dc = b.Col - a.Col;
        return Math.Sqrt(dr * dr + dc * dc);
    }

    private static List<ImagePoint> ReadContourPoints(HObject contour)
    {
        HOperatorSet.GetContourXld(contour, out HTuple rows, out HTuple cols);
        int count = Math.Min(rows.Length, cols.Length);
        List<ImagePoint> points = new(count);

        int stride = Math.Max(1, count / 5000);
        for (int i = 0; i < count; i += stride)
            points.Add(new ImagePoint(rows[i].D, cols[i].D));

        if (count > 0 && (count - 1) % stride != 0)
            points.Add(new ImagePoint(rows[count - 1].D, cols[count - 1].D));

        return points;
    }

    private static double DegToRad(double deg) => deg * Math.PI / 180.0;

    private static double RadToDeg(double rad) => rad * 180.0 / Math.PI;
}

public sealed class ShapeMatchOptions
{
    public double MinScore { get; set; } = 0.5;
    public double AngleStartDeg { get; set; } = -180;
    public double AngleExtentDeg { get; set; } = 360;
    public double MaxOverlap { get; set; } = 0.5;
    public double Greediness { get; set; } = 0.7;
}

public sealed class ShapeMatchResult
{
    public bool Found { get; set; }
    public double Row { get; set; }
    public double Col { get; set; }
    public double AngleDeg { get; set; }
    public double Score { get; set; }
}

public sealed class HalconVCornerOptions
{
    public double CannyAlpha { get; set; } = 1.2;
    public double CannyLow { get; set; } = 12;
    public double CannyHigh { get; set; } = 35;
    public double MinContourLength { get; set; } = 80;
    public double UnionMaxDistAbs { get; set; } = 20;
    public double UnionMaxDistRel { get; set; } = 2;
    public int SegmentSmooth { get; set; } = 5;
    public double SegmentMaxLineDist1 { get; set; } = 4;
    public double SegmentMaxLineDist2 { get; set; } = 3;
    public double MinArcLength { get; set; } = 10;
    public double MinArcRadius { get; set; } = 3;
    public double MaxArcRadius { get; set; } = 300;
    public int CurvatureStep { get; set; } = 8;
    public double CurvatureAngleThresholdDeg { get; set; } = 7;
    public int MinCurveRunPoints { get; set; } = 4;
    public double MinPointDistance { get; set; } = 20;
    public int TangentFitPoints { get; set; } = 45;
    public double TangentDepartureDistance { get; set; } = 4;
}

public sealed class VCornerDetectionResult
{
    public bool HasResult { get; set; }
    public string Mode { get; set; } = "none";
    public ImagePoint Point1 { get; set; }
    public ImagePoint Point2 { get; set; }
    public int NumUnionContours { get; set; }
    public int NumParts { get; set; }
    public ImageLine? BaseLine { get; set; }
    public List<ImagePoint> MainContour { get; set; } = new();
    public List<List<ImagePoint>> ContourParts { get; set; } = new();
    public List<ImagePoint> FeatureContour { get; set; } = new();
}

public readonly record struct ImageLine(ImagePoint Point, double DirectionRow, double DirectionCol, double Length);

public sealed class VCornerRoi
{
    public double CenterRow { get; set; }
    public double CenterCol { get; set; }
    public double PhiDeg { get; set; }
    public double PhiRad => PhiDeg * Math.PI / 180.0;
    public double Length1 { get; set; }
    public double Length2 { get; set; }

    public ImagePoint[] GetCorners()
    {
        double phi = PhiRad;
        double cos = Math.Cos(phi);
        double sin = Math.Sin(phi);

        return new[]
        {
            MakePoint(-Length1, -Length2),
            MakePoint( Length1, -Length2),
            MakePoint( Length1,  Length2),
            MakePoint(-Length1,  Length2)
        };

        ImagePoint MakePoint(double u, double v)
        {
            double row = CenterRow + u * sin - v * cos;
            double col = CenterCol + u * cos + v * sin;
            return new ImagePoint(row, col);
        }
    }
}

public readonly struct ImagePoint
{
    public ImagePoint(double row, double col)
    {
        Row = row;
        Col = col;
    }

    public double Row { get; }
    public double Col { get; }
}

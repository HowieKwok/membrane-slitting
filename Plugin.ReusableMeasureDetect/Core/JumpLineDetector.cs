using Frame_Core.pluginParams;
using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Plugin.ReusableMeasureDetect.Core
{
    /// <summary>
    /// 突变边检测器。
    /// 输入是一条有序或可排序的边点链，输出主边、副边、交点、夹角和拟合误差；算法按 demo 思路做主边簇和副边候选搜索。
    /// </summary>
    internal static class JumpLineDetector
    {
        /// <summary>
        /// 执行突变检测并写入 JumpOutput 的数值结果和 HALCON 叠加对象。
        /// </summary>
        /// <param name="p">插件参数，读取 RuntimeDetectionPoints/InputPoints，写入 JumpOutput。</param>
        public static void DetectAndWriteOverlay(ParamsReusableMeasureDetect p)
        {
            p.EnsureOutputs();
            JumpDetectionOutputParam output = p.JumpOutput;
            output.Clear();
            output.Success = false;

            List<MeasurePointParam> points = BuildInputPoints(p);
            int minFitPoints = Math.Max(2, p.MinFitPoints);
            if (points.Count < Math.Max(6, minFitPoints * 2))
            {
                output.RejectReason = "点数不足，无法拟合突变主边和副边";
                return;
            }

            string mode = string.Equals(p.JumpMode, "Col", StringComparison.OrdinalIgnoreCase) ? "Col" : "Row";
            output.Mode = mode;
            double threshold = Math.Max(0.001, mode == "Row" ? p.RowJumpThreshold : p.ColJumpThreshold);
            int mainMax = Math.Max(minFitPoints, p.MainMaxFitPoints);
            int secondaryMax = Math.Max(minFitPoints, p.SecondaryMaxFitPoints);

            List<JumpCandidate> candidates = FindJumpCandidates(points, mode, threshold, mainMax, secondaryMax);
            if (candidates.Count == 0)
            {
                output.RejectReason = "未找到突变候选主边/副边";
                return;
            }

            JumpDebugInfo bestDebug = null;
            for (int i = 0; i < candidates.Count; i++)
            {
                JumpDebugInfo debug;
                if (!TryBuildJumpDebugInfo(points, candidates[i], mode, threshold, p, out debug))
                    continue;

                if (bestDebug == null || debug.Score > bestDebug.Score)
                    bestDebug = debug;

                if (!debug.HasIntersection)
                    continue;

                WriteResultValues(p, points, debug);
                output.Success = true;
                output.RejectReason = string.Empty;
                return;
            }

            if (bestDebug != null)
            {
                WriteResultValues(p, points, bestDebug);
                output.Success = false;
                output.RejectReason = string.IsNullOrEmpty(bestDebug.RejectReason)
                    ? "突变候选未通过角度、拟合误差或交点校验"
                    : bestDebug.RejectReason;
                return;
            }

            output.RejectReason = "突变候选 HALCON 直线拟合失败";
        }

        /// <summary>
        /// 取得参与突变检测的点集。
        /// RuntimeDetectionPoints 来自当前滤波步骤；没有可用滤波输出时回退到 InputPoints。
        /// </summary>
        private static List<MeasurePointParam> BuildInputPoints(ParamsReusableMeasureDetect p)
        {
            if (p.RuntimeDetectionPoints != null && p.RuntimeDetectionPoints.Count >= 2)
                return p.RuntimeDetectionPoints.Select(point => new MeasurePointParam(point.Row, point.Col)).ToList();

            if (p.InputPoints == null) p.InputPoints = new List<MeasurePointParam>();
            return p.InputPoints.Select(point => new MeasurePointParam(point.Row, point.Col)).ToList();
        }

        /// <summary>
        /// 枚举主边范围簇和副边方向候选，并按分离度、点数和主边稳定性计算候选分数。
        /// </summary>
        private static List<JumpCandidate> FindJumpCandidates(
            IReadOnlyList<MeasurePointParam> points,
            string mode,
            double threshold,
            int mainMaxFitPoints,
            int secondaryMaxFitPoints)
        {
            List<JumpCandidate> candidates = new List<JumpCandidate>();
            HashSet<string> visitedMainClusters = new HashSet<string>();

            for (int seedIndex = 0; seedIndex < points.Count; seedIndex++)
            {
                double seedValue = Value(points[seedIndex], mode);
                List<int> mainSourceIndexes = BuildMainRangeCluster(points, mode, seedValue, threshold);
                if (mainSourceIndexes.Count < 2)
                    continue;

                string clusterKey = string.Join(",", mainSourceIndexes);
                if (!visitedMainClusters.Add(clusterKey))
                    continue;

                double mainMean = Average(points, mainSourceIndexes, mode);
                List<int> selectedMainIndexes = SelectFitIndexes(points, mainSourceIndexes, mode, mainMean, mainMaxFitPoints);
                if (selectedMainIndexes.Count < 2)
                    continue;

                foreach (List<int> secondarySourceIndexes in BuildSecondarySideCandidateSets(points, mainSourceIndexes, mode, mainMean))
                {
                    List<int> selectedSecondaryIndexes = SelectFitIndexes(points, secondarySourceIndexes, mode, mainMean, secondaryMaxFitPoints);
                    if (selectedSecondaryIndexes.Count < 2)
                        continue;

                    double secondaryMean = Average(points, selectedSecondaryIndexes, mode);
                    double separation = Math.Abs(secondaryMean - mainMean);
                    double mainSpread = IndexSpread(points, selectedMainIndexes, mode, mainMean);
                    int splitIndex = EstimateJumpSplitIndex(selectedMainIndexes, selectedSecondaryIndexes, points.Count);
                    double score = separation
                        + selectedMainIndexes.Count * 0.2
                        + selectedSecondaryIndexes.Count * 0.35
                        - mainSpread * 0.1;

                    candidates.Add(new JumpCandidate
                    {
                        SplitIndex = splitIndex,
                        Score = score,
                        MainMean = mainMean,
                        SecondaryMean = secondaryMean,
                        MainIndexes = selectedMainIndexes,
                        SecondaryIndexes = selectedSecondaryIndexes
                    });
                }
            }

            return candidates
                .OrderByDescending(candidate => candidate.Score)
                .ThenBy(candidate => candidate.SplitIndex)
                .ToList();
        }

        /// <summary>
        /// 以某个点的 Row/Col 值为种子，查找阈值范围内的主边点簇，并迭代两次收敛到均值附近。
        /// </summary>
        private static List<int> BuildMainRangeCluster(IReadOnlyList<MeasurePointParam> points, string mode, double seedValue, double threshold)
        {
            List<int> indexes = Enumerable.Range(0, points.Count)
                .Where(index => Math.Abs(Value(points[index], mode) - seedValue) <= threshold)
                .ToList();

            for (int iteration = 0; iteration < 2 && indexes.Count >= 2; iteration++)
            {
                double mean = Average(points, indexes, mode);
                indexes = Enumerable.Range(0, points.Count)
                    .Where(index => Math.Abs(Value(points[index], mode) - mean) <= threshold)
                    .ToList();
            }

            return indexes.OrderBy(index => index).ToList();
        }

        /// <summary>
        /// 从非主边点中按主边均值两侧拆分副边候选，避免主边点被重复拿去拟合副边。
        /// </summary>
        private static List<List<int>> BuildSecondarySideCandidateSets(
            IReadOnlyList<MeasurePointParam> points,
            IReadOnlyList<int> mainSourceIndexes,
            string mode,
            double mainMean)
        {
            HashSet<int> mainSet = new HashSet<int>(mainSourceIndexes);
            List<int> greaterSide = new List<int>();
            List<int> lessSide = new List<int>();

            for (int index = 0; index < points.Count; index++)
            {
                if (mainSet.Contains(index))
                    continue;

                double value = Value(points[index], mode);
                if (value > mainMean)
                    greaterSide.Add(index);
                else if (value < mainMean)
                    lessSide.Add(index);
            }

            return new[] { greaterSide, lessSide }
                .Where(side => side.Count >= 2)
                .OrderByDescending(side => side.Count)
                .ToList();
        }

        /// <summary>
        /// 从候选点中选择最靠近主边均值的拟合点，限制最大点数并保持原始顺序。
        /// </summary>
        private static List<int> SelectFitIndexes(
            IReadOnlyList<MeasurePointParam> points,
            IEnumerable<int> sourceIndexes,
            string mode,
            double mainMean,
            int maxFitPoints)
        {
            return sourceIndexes
                .Distinct()
                .OrderBy(index => Math.Abs(Value(points[index], mode) - mainMean))
                .ThenBy(index => index)
                .Take(Math.Max(2, maxFitPoints))
                .OrderBy(index => index)
                .ToList();
        }

        /// <summary>
        /// 对单个候选执行 HALCON 直线拟合、交点计算、角度校验和误差校验。
        /// </summary>
        /// <returns>HALCON 拟合成功返回 true；业务校验是否通过由 debug.HasIntersection 表示。</returns>
        private static bool TryBuildJumpDebugInfo(
            IReadOnlyList<MeasurePointParam> points,
            JumpCandidate candidate,
            string mode,
            double threshold,
            ParamsReusableMeasureDetect p,
            out JumpDebugInfo debug)
        {
            debug = null;
            int minFitPoints = Math.Max(2, p.MinFitPoints);
            int mainMaxFitPoints = Math.Max(minFitPoints, p.MainMaxFitPoints);
            int secondaryMaxFitPoints = Math.Max(minFitPoints, p.SecondaryMaxFitPoints);

            List<int> mainIndexes = candidate.MainIndexes.Distinct().Take(mainMaxFitPoints).OrderBy(index => index).ToList();
            List<int> secondaryIndexes = candidate.SecondaryIndexes.Distinct().Take(secondaryMaxFitPoints).OrderBy(index => index).ToList();
            if (mainIndexes.Count < minFitPoints || secondaryIndexes.Count < minFitPoints)
                return false;

            LineFit mainLine;
            LineFit secondaryLine;
            if (!FitLine(points, mainIndexes, out mainLine))
                return false;
            if (!FitLine(points, secondaryIndexes, out secondaryLine))
                return false;

            Point2 intersection = Intersect(mainLine.Start, mainLine.End, secondaryLine.Start, secondaryLine.End);
            double turnAngle = UndirectedAngle(mainLine.Direction, secondaryLine.Direction);
            double minTurn = Clamp0To90(p.MinTurnAngleDeg);
            double maxFitError = Math.Max(1.0, p.MaxFitError);
            double mainAngle = GetAngleLxAbsDeg(mainLine.Start, mainLine.End);
            double secondaryAngle = GetAngleLxAbsDeg(secondaryLine.Start, secondaryLine.End);
            double mainAngleMin = Clamp0To90(mode == "Row" ? p.RowMainAngleMinDeg : p.ColMainAngleMinDeg);
            double mainAngleMax = Clamp0To90(mode == "Row" ? p.RowMainAngleMaxDeg : p.ColMainAngleMaxDeg);

            bool intersectionOk = IsFinite(intersection.Row) && IsFinite(intersection.Col);
            bool mainAngleOk = IsAngleInRange0To90(mainAngle, mainAngleMin, mainAngleMax);
            bool turnAngleOk = turnAngle >= minTurn;
            bool mainFitOk = mainLine.Error <= maxFitError;
            bool secondaryFitOk = secondaryLine.Error <= maxFitError;
            bool accepted = intersectionOk && mainAngleOk && turnAngleOk && mainFitOk && secondaryFitOk;

            string rejectReason = string.Empty;
            if (!accepted)
            {
                if (!intersectionOk)
                    rejectReason = "拟合线无有效交点";
                else if (!mainAngleOk)
                    rejectReason = string.Format("主边角度超范围：{0:F2}° 不在 {1:F2}°~{2:F2}°", mainAngle, Math.Min(mainAngleMin, mainAngleMax), Math.Max(mainAngleMin, mainAngleMax));
                else if (!turnAngleOk)
                    rejectReason = string.Format("主副边夹角不足：{0:F2}° < {1:F2}°", turnAngle, minTurn);
                else if (!mainFitOk)
                    rejectReason = string.Format("主边拟合误差过大：{0:F2} > {1:F2}", mainLine.Error, maxFitError);
                else if (!secondaryFitOk)
                    rejectReason = string.Format("副边拟合误差过大：{0:F2} > {1:F2}", secondaryLine.Error, maxFitError);
            }

            debug = new JumpDebugInfo
            {
                SplitIndex = candidate.SplitIndex,
                Score = candidate.Score,
                Mode = mode,
                Threshold = threshold,
                MainMean = candidate.MainMean,
                SecondaryMean = candidate.SecondaryMean,
                MainLine = mainLine,
                SecondaryLine = secondaryLine,
                Intersection = intersection,
                TurnAngleDeg = turnAngle,
                MainAngleDeg = mainAngle,
                SecondaryAngleDeg = secondaryAngle,
                HasIntersection = accepted,
                RejectReason = rejectReason,
                MainIndexes = mainIndexes,
                SecondaryIndexes = secondaryIndexes
            };
            return true;
        }

        /// <summary>
        /// 使用 HALCON FitLineContourXld 对指定点索引拟合直线，并计算平均点到线距离作为拟合误差。
        /// </summary>
        private static bool FitLine(IReadOnlyList<MeasurePointParam> points, IReadOnlyList<int> indexes, out LineFit fit)
        {
            fit = new LineFit();
            if (indexes == null || indexes.Count < 2)
                return false;

            List<int> ordered = indexes.Distinct().OrderBy(index => index).ToList();
            if (ordered.Count < 2)
                return false;

            HObject contour = null;
            try
            {
                double[] rows = ordered.Select(index => points[index].Row).ToArray();
                double[] cols = ordered.Select(index => points[index].Col).ToArray();
                HOperatorSet.GenContourPolygonXld(out contour, new HTuple(rows), new HTuple(cols));
                HOperatorSet.FitLineContourXld(contour, "regression", -1, 0, 5, 2.0,
                    out HTuple rowBegin, out HTuple colBegin, out HTuple rowEnd, out HTuple colEnd,
                    out HTuple nr, out HTuple nc, out HTuple dist);

                fit.Start = new Point2(rowBegin.D, colBegin.D);
                fit.End = new Point2(rowEnd.D, colEnd.D);
                fit.Direction = Normalize(new Point2(fit.End.Row - fit.Start.Row, fit.End.Col - fit.Start.Col));
                if (Math.Abs(fit.Direction.Row) < 1e-9 && Math.Abs(fit.Direction.Col) < 1e-9)
                    fit.Direction = Normalize(new Point2(points[ordered.Last()].Row - points[ordered.First()].Row, points[ordered.Last()].Col - points[ordered.First()].Col));

                double errorSum = 0;
                for (int i = 0; i < ordered.Count; i++)
                    errorSum += DistanceToLine(new Point2(points[ordered[i]].Row, points[ordered[i]].Col), fit.Start, fit.End);
                fit.Error = errorSum / ordered.Count;
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                if (contour != null) contour.Dispose();
            }
        }

        /// <summary>
        /// 把通过或最接近通过的候选写入 JumpOutput，包括点集、直线端点、交点和显示 XLD。
        /// </summary>
        private static void WriteResultValues(ParamsReusableMeasureDetect p, IReadOnlyList<MeasurePointParam> points, JumpDebugInfo debug)
        {
            JumpDetectionOutputParam output = p.JumpOutput;
            output.Mode = debug.Mode;
            output.MainEdge.Points = debug.MainIndexes.Select(index => new MeasurePointParam(points[index].Row, points[index].Col)).ToList();
            output.SecondaryEdge.Points = debug.SecondaryIndexes.Select(index => new MeasurePointParam(points[index].Row, points[index].Col)).ToList();
            WriteLine(output.MainEdge, debug.MainLine, debug.MainAngleDeg);
            WriteLine(output.SecondaryEdge, debug.SecondaryLine, debug.SecondaryAngleDeg);
            output.MainAngleDeg = debug.MainAngleDeg;
            output.TurnAngleDeg = debug.TurnAngleDeg;
            output.IntersectionRow = debug.Intersection.Row;
            output.IntersectionCol = debug.Intersection.Col;
            output.MainMean = debug.MainMean;
            output.SecondaryMean = debug.SecondaryMean;

            if (output.ho_MainEdgeXld != null) output.ho_MainEdgeXld.Dispose();
            if (output.ho_SecondaryEdgeXld != null) output.ho_SecondaryEdgeXld.Dispose();
            if (output.ho_IntersectionCross != null) output.ho_IntersectionCross.Dispose();
            output.ho_MainEdgeXld = BuildLineXld(debug.MainLine.Start, debug.MainLine.End);
            output.ho_SecondaryEdgeXld = BuildLineXld(debug.SecondaryLine.Start, debug.SecondaryLine.End);
            if (IsFinite(debug.Intersection.Row) && IsFinite(debug.Intersection.Col))
                HOperatorSet.GenCrossContourXld(out output.ho_IntersectionCross, debug.Intersection.Row, debug.Intersection.Col, 28, 0.785398);
            else
                HOperatorSet.GenEmptyObj(out output.ho_IntersectionCross);
        }

        /// <summary>
        /// 将内部拟合线结构复制到可序列化输出结构。
        /// </summary>
        private static void WriteLine(FittedEdgeOutputParam target, LineFit fit, double angle)
        {
            target.StartRow = fit.Start.Row;
            target.StartCol = fit.Start.Col;
            target.EndRow = fit.End.Row;
            target.EndCol = fit.End.Col;
            target.FitError = fit.Error;
            target.AngleDeg = angle;
        }

        /// <summary>
        /// 根据拟合线起止点生成 HALCON 轮廓对象，调用方负责在输出 Clear/Dispose 时释放。
        /// </summary>
        private static HObject BuildLineXld(Point2 start, Point2 end)
        {
            HObject line;
            HOperatorSet.GenContourPolygonXld(out line, new HTuple(new[] { start.Row, end.Row }), new HTuple(new[] { start.Col, end.Col }));
            return line;
        }

        /// <summary>
        /// 计算两条拟合线交点；重合、平行或 HALCON 异常时返回 NaN。
        /// </summary>
        private static Point2 Intersect(Point2 p1, Point2 p2, Point2 p3, Point2 p4)
        {
            try
            {
                HOperatorSet.IntersectionLines(p1.Row, p1.Col, p2.Row, p2.Col, p3.Row, p3.Col, p4.Row, p4.Col, out HTuple row, out HTuple col, out HTuple isOverlapping);
                if (isOverlapping.I != 0)
                    return new Point2(double.NaN, double.NaN);
                return new Point2(row.D, col.D);
            }
            catch
            {
                return new Point2(double.NaN, double.NaN);
            }
        }

        /// <summary>
        /// 估计主边和副边在点链中的分界位置，用于候选排序和调试显示。
        /// </summary>
        private static int EstimateJumpSplitIndex(IReadOnlyList<int> mainIndexes, IReadOnlyList<int> secondaryIndexes, int pointCount)
        {
            int splitIndex = secondaryIndexes.Count > 0 ? secondaryIndexes[0] : 0;
            int bestDistance = int.MaxValue;
            for (int i = 0; i < mainIndexes.Count; i++)
            {
                for (int j = 0; j < secondaryIndexes.Count; j++)
                {
                    int distance = Math.Abs(mainIndexes[i] - secondaryIndexes[j]);
                    if (distance < bestDistance)
                    {
                        bestDistance = distance;
                        splitIndex = Math.Max(mainIndexes[i], secondaryIndexes[j]);
                    }
                }
            }

            return Math.Max(1, Math.Min(pointCount - 1, splitIndex));
        }

        /// <summary>
        /// 计算点集在检测方向上相对均值的平均偏差，偏差越小主边越稳定。
        /// </summary>
        private static double IndexSpread(IReadOnlyList<MeasurePointParam> points, IReadOnlyList<int> indexes, string mode, double mean)
        {
            if (indexes.Count == 0)
                return double.PositiveInfinity;

            double sum = 0;
            for (int i = 0; i < indexes.Count; i++)
                sum += Math.Abs(Value(points[indexes[i]], mode) - mean);
            return sum / indexes.Count;
        }

        /// <summary>
        /// 计算指定点索引在 Row 或 Col 方向的平均值。
        /// </summary>
        private static double Average(IReadOnlyList<MeasurePointParam> points, IEnumerable<int> indexes, string mode)
        {
            List<int> list = indexes.ToList();
            if (list.Count == 0)
                return 0;
            return list.Average(index => Value(points[index], mode));
        }

        /// <summary>
        /// 按突变模式读取点的 Row 或 Col 值。
        /// </summary>
        private static double Value(MeasurePointParam point, string mode)
        {
            return mode == "Row" ? point.Row : point.Col;
        }

        /// <summary>
        /// 单位化方向向量，零向量回退为水平方向，避免角度计算异常。
        /// </summary>
        private static Point2 Normalize(Point2 point)
        {
            double length = Math.Sqrt(point.Row * point.Row + point.Col * point.Col);
            return length < 1e-9 ? new Point2(0, 1) : new Point2(point.Row / length, point.Col / length);
        }

        /// <summary>
        /// 计算点到直线的垂直距离，用于拟合误差评估。
        /// </summary>
        private static double DistanceToLine(Point2 point, Point2 a, Point2 b)
        {
            double dr = b.Row - a.Row;
            double dc = b.Col - a.Col;
            double length = Math.Sqrt(dr * dr + dc * dc);
            if (length < 1e-9)
                return 0;
            return Math.Abs((point.Row - a.Row) * dc - (point.Col - a.Col) * dr) / length;
        }

        /// <summary>
        /// 计算两条无向边之间的夹角，并折算到 0 到 90 度。
        /// </summary>
        private static double UndirectedAngle(Point2 a, Point2 b)
        {
            double dot = a.Row * b.Row + a.Col * b.Col;
            double cross = a.Row * b.Col - a.Col * b.Row;
            double angle = Math.Abs(Math.Atan2(cross, dot) * 180.0 / Math.PI);
            return angle > 90.0 ? 180.0 - angle : angle;
        }

        /// <summary>
        /// 使用 HALCON AngleLx 得到直线与 X 轴的绝对夹角，并限制到 0 到 90 度。
        /// </summary>
        private static double GetAngleLxAbsDeg(Point2 start, Point2 end)
        {
            HOperatorSet.AngleLx(start.Row, start.Col, end.Row, end.Col, out HTuple angle);
            HOperatorSet.TupleDeg(angle, out HTuple deg);
            HOperatorSet.TupleAbs(deg, out HTuple abs);
            return Clamp0To90(abs.D);
        }

        /// <summary>
        /// 判断角度是否落在 0 到 90 度归一化后的范围内，允许用户把最小和最大写反。
        /// </summary>
        private static bool IsAngleInRange0To90(double angleDeg, double minDeg, double maxDeg)
        {
            if (minDeg > maxDeg)
            {
                double temp = minDeg;
                minDeg = maxDeg;
                maxDeg = temp;
            }
            return angleDeg >= minDeg && angleDeg <= maxDeg;
        }

        /// <summary>
        /// 判断数值是否为有限值，过滤 NaN 和 Infinity。
        /// </summary>
        private static bool IsFinite(double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }

        /// <summary>
        /// 将角度限制到 0 到 90 度。
        /// </summary>
        private static double Clamp0To90(double value)
        {
            if(value>90)
            {
                return Math.Abs(180-value);
            }
            value=Math.Abs(value);
            return value;
        }

        /// <summary>
        /// 主边/副边候选，保存筛选阶段的点索引和排序分数。
        /// </summary>
        private sealed class JumpCandidate
        {
            public int SplitIndex;
            public double Score;
            public double MainMean;
            public double SecondaryMean;
            public List<int> MainIndexes;
            public List<int> SecondaryIndexes;
        }

        /// <summary>
        /// 单个突变候选经过拟合和校验后的完整调试信息，也是写输出的中间结构。
        /// </summary>
        private sealed class JumpDebugInfo
        {
            public int SplitIndex;
            public double Score;
            public string Mode;
            public double Threshold;
            public double MainMean;
            public double SecondaryMean;
            public LineFit MainLine;
            public LineFit SecondaryLine;
            public Point2 Intersection;
            public double TurnAngleDeg;
            public double MainAngleDeg;
            public double SecondaryAngleDeg;
            public bool HasIntersection;
            public string RejectReason;
            public List<int> MainIndexes;
            public List<int> SecondaryIndexes;
        }

        /// <summary>
        /// 内部几何点，按 HALCON 坐标顺序保存 Row 和 Col。
        /// </summary>
        private struct Point2
        {
            public readonly double Row;
            public readonly double Col;

            /// <summary>
            /// 创建一个 Row/Col 点。
            /// </summary>
            public Point2(double row, double col)
            {
                Row = row;
                Col = col;
            }
        }

        /// <summary>
        /// HALCON 拟合线结果，包含端点、单位方向和平均拟合误差。
        /// </summary>
        private struct LineFit
        {
            public Point2 Start;
            public Point2 End;
            public Point2 Direction;
            public double Error;
        }
    }
}

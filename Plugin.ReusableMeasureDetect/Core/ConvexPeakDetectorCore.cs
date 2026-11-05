using Frame_Core.pluginParams;
using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Plugin.ReusableMeasureDetect.Core
{
    /// <summary>
    /// 传统角点凸点检测器。
    /// 通过平滑点链、左右窗口拟合直线、计算转角/突出度/坡度差/拟合误差来定位最明显凸点。
    /// </summary>
    internal static class ConvexPeakDetectorCore
    {
        /// <summary>
        /// 执行凸点检测并写入 ConvexOutput 的数值结果和 HALCON 叠加对象。
        /// </summary>
        /// <param name="p">插件参数，读取 RuntimeDetectionPoints/InputPoints，写入 ConvexOutput。</param>
        /// <param name="overwriteMainSecondary">保留给外部区分是否允许凸点结果独占主副边显示；当前只写 ConvexOutput。</param>
        /// <returns>找到满足阈值的凸点返回 true，否则返回 false 并写入 RejectReason。</returns>
        public static bool DetectAndWriteOverlay(ParamsReusableMeasureDetect p, bool overwriteMainSecondary)
        {
            p.EnsureOutputs();
            ConvexPeakOutputParam output = p.ConvexOutput;
            output.Clear();
            output.Success = false;
            output.RejectReason = string.Empty;

            if (!p.EnableCornerPeakDetection)
            {
                output.RejectReason = "传统角点凸点未启用";
                return false;
            }

            // 凸点优先使用滤波管线选中的点集，避免检测和界面显示的点集不一致。
            List<MeasurePointParam> sourcePoints = BuildInputPoints(p);
            sourcePoints = RemoveIsolatedOutliers(sourcePoints, p.ConvexOutlierRadius, p.ConvexOutlierMinNeighbors);

            int support = Math.Max(2, p.ConvexSupportWindow);
            if (sourcePoints.Count < Math.Max(p.ConvexMinChainPoints, support * 2 + 1))
            {
                output.RejectReason = "凸点检测点数不足";
                return false;
            }

            List<Point2> points = Smooth(sourcePoints, Math.Max(1, p.ConvexSmoothWindow));
            double minTurn = Clamp0To90(p.ConvexMinTurnAngleDeg);
            double minProminence = Math.Max(0.0, p.ConvexMinProminence);
            double minSlopeDelta = Math.Max(0.0, p.ConvexMinSlopeDelta);
            double minScore = Math.Max(0.0, p.ConvexMinPeakScore);
            double maxFitError = Math.Max(1.0, p.MaxFitError);

            ConvexCandidate best = null;
            for (int center = support; center < points.Count - support; center++)
            {
                List<int> leftIndexes = BuildIndexRange(center - support, center);
                List<int> rightIndexes = BuildIndexRange(center, center + support);
                LineFit leftLine;
                LineFit rightLine;
                if (!FitLine(points, leftIndexes, out leftLine)) continue;
                if (!FitLine(points, rightIndexes, out rightLine)) continue;

                double turnAngle = UndirectedAngle(leftLine.Direction, rightLine.Direction);
                double prominence = DistanceToLine(points[center], points[center - support], points[center + support]);
                double slopeDelta = Math.Abs(DirectionSlope(leftLine.Direction) - DirectionSlope(rightLine.Direction));
                double fitError = Math.Max(leftLine.Error, rightLine.Error);
                double score = ScoreCandidate(turnAngle, prominence, slopeDelta, fitError, maxFitError, minProminence, minSlopeDelta);

                if (turnAngle < minTurn) continue;
                if (prominence < minProminence) continue;
                if (slopeDelta < minSlopeDelta) continue;
                if (fitError > maxFitError) continue;
                if (score < minScore) continue;

                ConvexCandidate candidate = new ConvexCandidate
                {
                    CenterIndex = center,
                    Score = score,
                    TurnAngleDeg = turnAngle,
                    Prominence = prominence,
                    SlopeDelta = slopeDelta,
                    FitError = fitError,
                    Peak = points[center],
                    LeftIndexes = leftIndexes,
                    RightIndexes = rightIndexes,
                    LeftLine = leftLine,
                    RightLine = rightLine
                };

                if (best == null || candidate.Score > best.Score)
                    best = candidate;
            }

            if (best == null)
            {
                output.RejectReason = "未找到满足角度、坡度差、突出度、拟合误差和得分的凸点";
                return false;
            }

            WriteConvexResult(p, sourcePoints, best);
            output.Success = true;
            return true;
        }

        /// <summary>
        /// 取得参与凸点检测的点集。
        /// RuntimeDetectionPoints 需要至少 5 个点，否则回退到 InputPoints，避免少量点无法形成左右窗口。
        /// </summary>
        private static List<MeasurePointParam> BuildInputPoints(ParamsReusableMeasureDetect p)
        {
            if (p.RuntimeDetectionPoints != null && p.RuntimeDetectionPoints.Count >= 5)
                return p.RuntimeDetectionPoints.Select(point => new MeasurePointParam(point.Row, point.Col)).ToList();

            if (p.InputPoints == null) p.InputPoints = new List<MeasurePointParam>();
            return p.InputPoints.Select(point => new MeasurePointParam(point.Row, point.Col)).ToList();
        }

        /// <summary>
        /// 基于邻域数量剔除孤立点；当剔除后点数不足时回退原始点，防止过度过滤。
        /// </summary>
        private static List<MeasurePointParam> RemoveIsolatedOutliers(IReadOnlyList<MeasurePointParam> points, double radius, int minNeighbors)
        {
            if (points == null) return new List<MeasurePointParam>();
            if (points.Count <= 5 || radius <= 0 || minNeighbors <= 0)
                return points.Select(point => new MeasurePointParam(point.Row, point.Col)).ToList();

            List<MeasurePointParam> kept = new List<MeasurePointParam>();
            for (int i = 0; i < points.Count; i++)
            {
                int neighbors = 0;
                for (int j = 0; j < points.Count; j++)
                {
                    if (i == j) continue;
                    if (Distance(points[i], points[j]) <= radius) neighbors++;
                    if (neighbors >= minNeighbors) break;
                }
                if (neighbors >= minNeighbors) kept.Add(new MeasurePointParam(points[i].Row, points[i].Col));
            }
            return kept.Count >= 5 ? kept : points.Select(point => new MeasurePointParam(point.Row, point.Col)).ToList();
        }

        /// <summary>
        /// 对点链做滑动平均平滑，降低单点抖动对转角和坡度差的影响。
        /// </summary>
        private static List<Point2> Smooth(IReadOnlyList<MeasurePointParam> sourcePoints, int window)
        {
            int radius = Math.Max(1, window);
            List<Point2> smoothed = new List<Point2>(sourcePoints.Count);
            for (int i = 0; i < sourcePoints.Count; i++)
            {
                int start = Math.Max(0, i - radius);
                int end = Math.Min(sourcePoints.Count - 1, i + radius);
                double row = 0;
                double col = 0;
                int count = 0;
                for (int j = start; j <= end; j++)
                {
                    row += sourcePoints[j].Row;
                    col += sourcePoints[j].Col;
                    count++;
                }
                smoothed.Add(new Point2(row / count, col / count));
            }
            return smoothed;
        }

        /// <summary>
        /// 生成闭区间索引列表，用作凸点左右拟合窗口。
        /// </summary>
        private static List<int> BuildIndexRange(int start, int end)
        {
            List<int> indexes = new List<int>();
            for (int i = start; i <= end; i++) indexes.Add(i);
            return indexes;
        }

        /// <summary>
        /// 将转角、突出度、坡度差和拟合误差归一化为综合得分，得分越高越像真实凸点。
        /// </summary>
        private static double ScoreCandidate(double turnAngle, double prominence, double slopeDelta, double fitError, double maxFitError, double minProminence, double minSlopeDelta)
        {
            double angleScore = Clamp0To90(turnAngle) / 90.0;
            double prominenceScore = prominence / Math.Max(0.001, Math.Max(minProminence, prominence));
            double slopeScore = slopeDelta / Math.Max(0.001, Math.Max(minSlopeDelta * 4.0, slopeDelta));
            double errorScore = 1.0 - Math.Min(1.0, fitError / Math.Max(1.0, maxFitError));
            return angleScore * 0.30 + prominenceScore * 0.25 + slopeScore * 0.25 + errorScore * 0.20;
        }

        /// <summary>
        /// 使用 HALCON FitLineContourXld 对指定窗口点拟合直线，并计算窗口内平均点到线距离。
        /// </summary>
        private static bool FitLine(List<Point2> points, List<int> indexes, out LineFit fit)
        {
            fit = new LineFit();
            if (indexes == null || indexes.Count < 2) return false;
            HObject contour = null;
            try
            {
                double[] rows = indexes.Select(i => points[i].Row).ToArray();
                double[] cols = indexes.Select(i => points[i].Col).ToArray();
                HOperatorSet.GenContourPolygonXld(out contour, new HTuple(rows), new HTuple(cols));
                HOperatorSet.FitLineContourXld(contour, "regression", -1, 0, 5, 2.0,
                    out HTuple rowBegin, out HTuple colBegin, out HTuple rowEnd, out HTuple colEnd,
                    out HTuple nr, out HTuple nc, out HTuple dist);

                fit.Start = new Point2(rowBegin.D, colBegin.D);
                fit.End = new Point2(rowEnd.D, colEnd.D);
                fit.Direction = Normalize(new Point2(fit.End.Row - fit.Start.Row, fit.End.Col - fit.Start.Col));
                double sum = 0;
                foreach (int index in indexes) sum += DistanceToLine(points[index], fit.Start, fit.End);
                fit.Error = sum / indexes.Count;
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
        /// 将最佳凸点候选写入 ConvexOutput，并生成主边、副边和凸点十字叠加对象。
        /// </summary>
        private static void WriteConvexResult(ParamsReusableMeasureDetect p, IReadOnlyList<MeasurePointParam> sourcePoints, ConvexCandidate best)
        {
            ConvexPeakOutputParam output = p.ConvexOutput;
            output.PeakRow = best.Peak.Row;
            output.PeakCol = best.Peak.Col;
            output.Score = best.Score;
            output.TurnAngleDeg = best.TurnAngleDeg;
            output.Prominence = best.Prominence;
            output.SlopeDelta = best.SlopeDelta;
            output.FitError = best.FitError;
            output.Method = "CornerConvex";
            output.RejectReason = string.Empty;
            output.PeakPoints = new List<MeasurePointParam> { new MeasurePointParam(best.Peak.Row, best.Peak.Col) };
            output.MainEdge.Points = best.LeftIndexes.Select(i => new MeasurePointParam(sourcePoints[i].Row, sourcePoints[i].Col)).ToList();
            output.SecondaryEdge.Points = best.RightIndexes.Select(i => new MeasurePointParam(sourcePoints[i].Row, sourcePoints[i].Col)).ToList();
            WriteLine(output.MainEdge, best.LeftLine);
            WriteLine(output.SecondaryEdge, best.RightLine);

            if (output.ho_MainEdgeXld != null) output.ho_MainEdgeXld.Dispose();
            if (output.ho_SecondaryEdgeXld != null) output.ho_SecondaryEdgeXld.Dispose();
            if (output.ho_PeakCross != null) output.ho_PeakCross.Dispose();
            output.ho_MainEdgeXld = BuildLineXld(best.LeftLine.Start, best.LeftLine.End);
            output.ho_SecondaryEdgeXld = BuildLineXld(best.RightLine.Start, best.RightLine.End);
            HOperatorSet.GenCrossContourXld(out output.ho_PeakCross, best.Peak.Row, best.Peak.Col, 32, 0.785398);
        }

        /// <summary>
        /// 将内部拟合线复制到可序列化输出结构。
        /// </summary>
        private static void WriteLine(FittedEdgeOutputParam target, LineFit source)
        {
            target.StartRow = source.Start.Row;
            target.StartCol = source.Start.Col;
            target.EndRow = source.End.Row;
            target.EndCol = source.End.Col;
            target.FitError = source.Error;
            target.AngleDeg = GetAngleLxAbsDeg(source.Start, source.End);
        }

        /// <summary>
        /// 根据拟合线端点生成 HALCON XLD 线对象，调用方负责通过输出 Clear/Dispose 释放。
        /// </summary>
        private static HObject BuildLineXld(Point2 start, Point2 end)
        {
            HObject line;
            HOperatorSet.GenContourPolygonXld(out line, new HTuple(new[] { start.Row, end.Row }), new HTuple(new[] { start.Col, end.Col }));
            return line;
        }

        /// <summary>
        /// 使用 HALCON AngleLx 计算直线绝对角度，并限制到 0 到 90 度。
        /// </summary>
        private static double GetAngleLxAbsDeg(Point2 start, Point2 end)
        {
            HOperatorSet.AngleLx(start.Row, start.Col, end.Row, end.Col, out HTuple angle);
            HOperatorSet.TupleDeg(angle, out HTuple deg);
            HOperatorSet.TupleAbs(deg, out HTuple abs);
            return Clamp0To90(abs.D);
        }

        /// <summary>
        /// 单位化方向向量，零向量回退为水平方向。
        /// </summary>
        private static Point2 Normalize(Point2 p)
        {
            double len = Math.Sqrt(p.Row * p.Row + p.Col * p.Col);
            return len < 1e-9 ? new Point2(0, 1) : new Point2(p.Row / len, p.Col / len);
        }

        /// <summary>
        /// 计算两个测量点之间的欧氏距离。
        /// </summary>
        private static double Distance(MeasurePointParam a, MeasurePointParam b)
        {
            double dr = a.Row - b.Row;
            double dc = a.Col - b.Col;
            return Math.Sqrt(dr * dr + dc * dc);
        }

        /// <summary>
        /// 计算点到直线距离，用于突出度和拟合误差。
        /// </summary>
        private static double DistanceToLine(Point2 point, Point2 a, Point2 b)
        {
            double dr = b.Row - a.Row;
            double dc = b.Col - a.Col;
            double len = Math.Sqrt(dr * dr + dc * dc);
            if (len < 1e-9) return 0;
            return Math.Abs((point.Row - a.Row) * dc - (point.Col - a.Col) * dr) / len;
        }

        /// <summary>
        /// 计算两条无向方向之间的夹角，并折算到 0 到 90 度。
        /// </summary>
        private static double UndirectedAngle(Point2 a, Point2 b)
        {
            double dot = a.Row * b.Row + a.Col * b.Col;
            double cross = a.Row * b.Col - a.Col * b.Row;
            double angle = Math.Abs(Math.Atan2(cross, dot) * 180.0 / Math.PI);
            return angle > 90.0 ? 180.0 - angle : angle;
        }

        /// <summary>
        /// 将方向向量转换为近似斜率，垂直方向用有限大值代替，避免无穷参与打分。
        /// </summary>
        private static double DirectionSlope(Point2 direction)
        {
            if (Math.Abs(direction.Col) < 1e-6)
                return direction.Row >= 0 ? 999.0 : -999.0;
            double slope = direction.Row / direction.Col;
            if (slope > 999.0) return 999.0;
            if (slope < -999.0) return -999.0;
            return slope;
        }

        /// <summary>
        /// 将角度限制到 0 到 90 度。
        /// </summary>
        private static double Clamp0To90(double value)
        {
            if (value < 0) return 0;
            if (value > 90) return 90;
            return value;
        }

        /// <summary>
        /// 凸点候选缓存，保存评分、几何量、左右窗口索引和左右拟合线。
        /// </summary>
        private sealed class ConvexCandidate
        {
            public int CenterIndex;
            public double Score;
            public double TurnAngleDeg;
            public double Prominence;
            public double SlopeDelta;
            public double FitError;
            public Point2 Peak;
            public List<int> LeftIndexes;
            public List<int> RightIndexes;
            public LineFit LeftLine;
            public LineFit RightLine;
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
            public Point2(double row, double col) { Row = row; Col = col; }
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

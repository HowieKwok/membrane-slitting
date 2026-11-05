using Frame_Core.pluginParams;
using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Plugin.ReusableMeasureDetect.Core
{
    /// <summary>
    /// MeasureLine 抓边执行器。
    /// 它只负责从图像和 ROI 参数生成原始边点，不负责突变、凸点或滤波逻辑，保证抓边阶段可以独立复用和排查。
    /// </summary>
    internal static class MeasureLineGrabEngine
    {
        /// <summary>
        /// 按当前 ROI、位置跟随和 HALCON metrology 参数执行线测量，结果写入 InputPoints 与 GrabOutput。
        /// </summary>
        /// <param name="p">插件参数，包含图像、ROI、MeasureLine 参数和输出对象。</param>
        /// <returns>找到至少一个有效边点返回 true，否则返回 false 并写入失败消息。</returns>
        public static bool Run(ParamsReusableMeasureDetect p)
        {
            p.EnsureOutputs();
            p.GrabOutput.Clear();
            p.InputPoints = new List<MeasurePointParam>();

            if (!HasValidImage(p.ho_Image))
            {
                p.GrabOutput.Message = "无有效输入图像";
                return false;
            }

            //if (p.m_bUsePositionFollow && !p.m_bCurrentPoseValid)
            //{
            //    p.GrabOutput.Message = "位置跟随已启用，但当前坐标无效";
            //    return false;
            //}

            // ROI 先经过位置跟随，再转换成 HALCON AddMetrologyObjectLineMeasure 需要的线段端点。
            int pointNum = Math.Max(2, p.RoiPointNum);
            ReusableMeasureRoiRect2 followedRoi = new ReusableMeasureRoiFollower(p, true).Transform(
                p.RoiCenterRow,
                p.RoiCenterCol,
                p.RoiAngleDeg,
                p.RoiHalfLength1,
                p.RoiHalfLength2,
                pointNum);
            double roiHeight = Math.Max(1.0, Math.Abs(followedRoi.HalfLength1) * 2.0);
            double roiWidth = Math.Max(1.0, Math.Abs(followedRoi.HalfLength2) * 2.0);
            double phi = followedRoi.AngleDeg * Math.PI / 180.0;

            HTuple measureRow1 = followedRoi.Row - roiHeight * Math.Sin(phi) / 2.0;
            HTuple measureColumn1 = followedRoi.Col - roiHeight * Math.Cos(phi) / 2.0;
            HTuple measureRow2 = followedRoi.Row + roiHeight * Math.Sin(phi) / 2.0;
            HTuple measureColumn2 = followedRoi.Col + roiHeight * Math.Cos(phi) / 2.0;

            HTuple metrologyHandle = null;
            HObject contours = null;
            HObject gray = null;
            bool grayOwned = false;

            try
            {
                HOperatorSet.CountChannels(p.ho_Image, out HTuple channels);
                if (channels.I > 1)
                {
                    HOperatorSet.Rgb1ToGray(p.ho_Image, out gray);
                    grayOwned = true;
                }
                else
                {
                    gray = p.ho_Image;
                    grayOwned = false;
                }

                // ScaleMean 对图像做灰度开运算平滑，等价于抓边前的简单背景滤波。
                if (p.ScaleMean > 0)
                {
                    int mean = Math.Max(2, (int)Math.Round(p.ScaleMean));
                    HObject opened;
                    HOperatorSet.GrayOpeningRect(gray, out opened, mean, mean);
                    if (grayOwned && gray != null) gray.Dispose();
                    gray = opened;
                    grayOwned = true;
                }

                HOperatorSet.CreateMetrologyModel(out metrologyHandle);
                HOperatorSet.AddMetrologyObjectLineMeasure(
                    metrologyHandle,
                    measureRow1,
                    measureColumn1,
                    measureRow2,
                    measureColumn2,
                    roiWidth / 2.0,
                    roiHeight / pointNum / 2.0,
                    Math.Max(0.1, p.MeasureSigma),
                    Math.Max(0.0, p.MeasureThreshold),
                    new HTuple(),
                    new HTuple(),
                    out HTuple index);

                HOperatorSet.SetMetrologyObjectParam(metrologyHandle, index, "num_instances", Math.Max(1, p.MeasureNumInstances));
                HOperatorSet.SetMetrologyObjectParam(metrologyHandle, index, "distance_threshold", Math.Max(0, p.MeasureDistThreshold));
                HOperatorSet.SetMetrologyObjectParam(metrologyHandle, index, "measure_transition", NormalizeTransition(p.MeasureTransition));
                HOperatorSet.SetMetrologyObjectParam(metrologyHandle, index, "measure_select", NormalizeSelect(p.MeasureSelect));
                HOperatorSet.SetMetrologyObjectParam(metrologyHandle, index, "num_measures", pointNum);
                HOperatorSet.SetMetrologyObjectParam(metrologyHandle, index, "min_score", Clamp01(p.MeasureMinScore));
                HOperatorSet.SetMetrologyObjectParam(metrologyHandle, index, "max_num_iterations", p.MaxNumIterations);
                HOperatorSet.SetMetrologyObjectParam(metrologyHandle, index, "measure_interpolation", NormalizeInterpolation(p.MeasureInterpolation));
                HOperatorSet.SetMetrologyObjectParam(metrologyHandle, index, "instances_outside_measure_regions", NormalizeBoolString(p.MeasureIORegions));

                HOperatorSet.ApplyMetrologyModel(gray, metrologyHandle);
                HOperatorSet.GetMetrologyObjectMeasures(out contours, metrologyHandle, "all", "all", out HTuple crossRow, out HTuple crossColumn);

                List<MeasurePointParam> points = new List<MeasurePointParam>();
                int count = Math.Min(crossRow.Length, crossColumn.Length);
                for (int i = 0; i < count; i++)
                    points.Add(new MeasurePointParam(crossRow[i].D, crossColumn[i].D));

                int removedCount = 0;
                // 抓到的点只做沿 ROI 法向的中值离群剔除，不再做二次 ROI 裁剪，避免把 demo 中的副边点筛没。
                if (p.EnablePointMedianReject)
                    points = SortAndMedianReject(points, followedRoi.Row, followedRoi.Col, phi, roiWidth / 2.0, p.PointMedianRangeFactor, out removedCount);

                p.InputPoints = Clone(points);
                p.GrabOutput.Success = points.Count > 0;
                p.GrabOutput.RawPoints = Clone(points);
                p.GrabOutput.RawPointCount = points.Count;
                p.GrabOutput.RemovedPointCount = removedCount;
                p.GrabOutput.Message = points.Count > 0
                    ? string.Format("抓边成功，点数 {0}，剔除离群点 {1}", points.Count, removedCount)
                    : "未找到有效边点";
                WriteRawCross(p);
                return p.GrabOutput.Success;
            }
            catch (Exception ex)
            {
                p.GrabOutput.Success = false;
                p.GrabOutput.Message = "抓边异常：" + ex.Message;
                return false;
            }
            finally
            {
                if (contours != null) contours.Dispose();
                if (grayOwned && gray != null) gray.Dispose();
                if (metrologyHandle != null) HOperatorSet.ClearMetrologyModel(metrologyHandle);
            }
        }

        /// <summary>
        /// 根据 InputPoints 生成绿色原始抓边十字 XLD，供界面叠加显示。
        /// </summary>
        private static void WriteRawCross(ParamsReusableMeasureDetect p)
        {
            if (p.GrabOutput.ho_RawPointsCross != null) p.GrabOutput.ho_RawPointsCross.Dispose();
            if (p.InputPoints == null || p.InputPoints.Count == 0)
            {
                HOperatorSet.GenEmptyObj(out p.GrabOutput.ho_RawPointsCross);
                return;
            }

            double[] rows = p.InputPoints.Select(point => point.Row).ToArray();
            double[] cols = p.InputPoints.Select(point => point.Col).ToArray();
            HOperatorSet.GenCrossContourXld(out p.GrabOutput.ho_RawPointsCross, new HTuple(rows), new HTuple(cols), 8.0, 0.785398);
        }

        /// <summary>
        /// 将点投影到 ROI 主轴和法向，按主轴排序，并用法向中值剔除离群点。
        /// 如果剔除后点数过少则回退到原始点，防止误删导致后续拟合失败。
        /// </summary>
        private static List<MeasurePointParam> SortAndMedianReject(
            IReadOnlyList<MeasurePointParam> input,
            double roiRow,
            double roiCol,
            double roiPhi,
            double measureHalfWidth,
            double rangeFactor,
            out int removedCount)
        {
            removedCount = 0;
            if (input == null || input.Count == 0) return new List<MeasurePointParam>();

            double axisRow = Math.Sin(roiPhi);
            double axisCol = Math.Cos(roiPhi);
            double normalRow = Math.Cos(roiPhi);
            double normalCol = -Math.Sin(roiPhi);

            List<ProjectedPoint> projected = input
                .Select(point => new ProjectedPoint(
                    point,
                    (point.Row - roiRow) * axisRow + (point.Col - roiCol) * axisCol,
                    (point.Row - roiRow) * normalRow + (point.Col - roiCol) * normalCol))
                .OrderBy(point => point.Axis)
                .ThenBy(point => point.Normal)
                .ToList();

            if (projected.Count < 3) return projected.Select(point => ClonePoint(point.Point)).ToList();

            double range = Math.Max(1.0, Math.Max(0.1, rangeFactor) * Math.Max(1.0, measureHalfWidth));
            double medianNormal = Median(projected.Select(point => point.Normal).ToList());
            List<MeasurePointParam> kept = projected
                .Where(point => Math.Abs(point.Normal - medianNormal) <= range)
                .OrderBy(point => point.Axis)
                .ThenBy(point => point.Normal)
                .Select(point => ClonePoint(point.Point))
                .ToList();

            if (kept.Count < 3) return projected.Select(point => ClonePoint(point.Point)).ToList();
            removedCount = projected.Count - kept.Count;
            return kept;
        }

        /// <summary>
        /// 校验 HObject 是否是已初始化的 HALCON 图像对象。
        /// </summary>
        private static bool HasValidImage(HObject image)
        {
            if (image == null || !image.IsInitialized()) return false;
            try
            {
                HOperatorSet.CountObj(image, out HTuple count);
                if (count.I <= 0) return false;
                HOperatorSet.GetObjClass(image, out HTuple objClass);
                return string.Equals(objClass.S, "image", StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 规范化 HALCON measure_transition 参数，只允许 HALCON 支持的枚举值。
        /// </summary>
        private static string NormalizeTransition(string value)
        {
            if (string.Equals(value, "negative", StringComparison.OrdinalIgnoreCase)) return "negative";
            if (string.Equals(value, "positive", StringComparison.OrdinalIgnoreCase)) return "positive";
            if (string.Equals(value, "uniform", StringComparison.OrdinalIgnoreCase)) return "uniform";
            return "all";
        }

        /// <summary>
        /// 规范化 HALCON measure_select 参数，只允许 first、last、all。
        /// </summary>
        private static string NormalizeSelect(string value)
        {
            if (string.Equals(value, "first", StringComparison.OrdinalIgnoreCase)) return "first";
            if (string.Equals(value, "last", StringComparison.OrdinalIgnoreCase)) return "last";
            return "all";
        }

        /// <summary>
        /// 规范化 HALCON measure_interpolation 参数，非法值回退到 bilinear。
        /// </summary>
        private static string NormalizeInterpolation(string value)
        {
            if (string.Equals(value, "nearest_neighbor", StringComparison.OrdinalIgnoreCase)) return "nearest_neighbor";
            if (string.Equals(value, "bicubic", StringComparison.OrdinalIgnoreCase)) return "bicubic";
            return "bilinear";
        }

        /// <summary>
        /// 把界面字符串参数转换为 HALCON 需要的 true/false 字符串。
        /// </summary>
        private static string NormalizeBoolString(string value)
        {
            return string.Equals(value, "true", StringComparison.OrdinalIgnoreCase) ? "true" : "false";
        }

        /// <summary>
        /// 将得分阈值限制到 0 到 1。
        /// </summary>
        private static double Clamp01(double value)
        {
            if (value < 0) return 0;
            if (value > 1) return 1;
            return value;
        }

        /// <summary>
        /// 计算 double 集合中值，调用前要求集合非空。
        /// </summary>
        private static double Median(List<double> values)
        {
            values.Sort();
            int mid = values.Count / 2;
            return values.Count % 2 == 1 ? values[mid] : (values[mid - 1] + values[mid]) * 0.5;
        }

        /// <summary>
        /// 深拷贝点集，避免滤波或检测阶段修改原始抓边输出。
        /// </summary>
        private static List<MeasurePointParam> Clone(IReadOnlyList<MeasurePointParam> points)
        {
            if (points == null) return new List<MeasurePointParam>();
            return points.Select(ClonePoint).ToList();
        }

        /// <summary>
        /// 深拷贝单个点；空点回退为默认点，保证列表内不出现 null。
        /// </summary>
        private static MeasurePointParam ClonePoint(MeasurePointParam point)
        {
            return point == null ? new MeasurePointParam() : new MeasurePointParam(point.Row, point.Col);
        }

        /// <summary>
        /// 抓边点在 ROI 坐标系下的投影缓存，用于排序和离群剔除。
        /// </summary>
        private sealed class ProjectedPoint
        {
            public readonly MeasurePointParam Point;
            public readonly double Axis;
            public readonly double Normal;

            /// <summary>
            /// 保存原点、主轴投影值和法向投影值。
            /// </summary>
            public ProjectedPoint(MeasurePointParam point, double axis, double normal)
            {
                Point = point;
                Axis = axis;
                Normal = normal;
            }
        }
    }
}

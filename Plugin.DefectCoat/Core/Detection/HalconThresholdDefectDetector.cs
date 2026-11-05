using DefectRuleClassifier.Core.Features;
using DefectRuleClassifier.Core.Models;
using Frame_Core.pluginParams;
using FrameCommunity.公共类;
using HalconDotNet;
using Plugin.DefectCoat.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
namespace Plugin.DefectCoat.Core.Detection
{
    /// <summary>
    /// HALCON 阈值缺陷检测器。
    /// 职责只包含图像预处理、检测 ROI/屏蔽 ROI、亮暗阈值分割、候选缺陷连通域输出。
    /// 分类特征不在这里硬凑，统一交给 DefectFeatureExtractor 按源程序 FeatureXXX 方法计算。
    /// </summary>
    public sealed class HalconThresholdDefectDetector : IDefectDetector
    {
        /// <summary>
        /// 主检测流程：
        /// 1. 输入图像转灰度；
        /// 2. 生成检测 ROI 和屏蔽 ROI；
        /// 3. 可选按源程序 PretreatmentImg 思路进行背景灰度归一化；默认关闭时 scale=1.0，直接使用原灰度图；
        /// 4. 用参考灰度上下阈值分出暗区、亮区；
        /// 5. 合并亮暗区并筛选候选缺陷；
        /// 6. 为每个缺陷输出基础几何数据和 38 项分类特征。
        /// </summary>
        public DefectDetectResult Detect(DefectDetectRequest request)
        {
            DefectDetectResult result = new DefectDetectResult();
            // 以下局部 HObject 均为本方法临时对象，finally 中统一释放。
            // 需要带出给调用方的图像或区域，必须先 CopyObj 到 result 中，避免返回后引用已释放对象。
            HObject gray = null;
            HObject scaledGray = null;
            HObject roi = null;
            HObject reduced = null;
            HObject dark = null;
            HObject light = null;
            HObject union = null;
            HObject maskedUnion = null;
            HObject maskUnionRegion = null;
            HObject opened = null;
            HObject closed = null;
            HObject connected = null;
            HObject selected = null;

            try
            {
                if (request == null || !HasObject(request.Image))
                {
                    result.Message = "未加载图像";
                    result.DebugLogs.Add("检测终止：Image为空或未初始化。");
                    return result;
                }

                HOperatorSet.CountChannels(request.Image, out HTuple channels);
                if (channels.I > 1)
                {
                    HOperatorSet.Rgb1ToGray(request.Image, out gray);
                    result.DebugLogs.Add("预处理：RGB图像已转换为灰度图。");
                }
                else
                {
                    gray = request.Image.CopyObj(1, -1);
                    result.DebugLogs.Add("预处理：输入已是单通道灰度图。");
                }

                DefectRoiFollowTransformer transformer = new DefectRoiFollowTransformer(request);
                CoatRoiParam transformedLocalRoi = null;
                if (request.DetectMode == DetectRangeMode.LocalRoi && request.EnableLocalRoi && request.LocalRoi != null)
                    transformedLocalRoi = transformer.TransformRoi(request.LocalRoi);

                if (transformedLocalRoi != null)
                {
                    transformer.CreateRegion(transformedLocalRoi, out roi);
                    result.DebugLogs.Add(string.Format("检测区域ROI：Rectangle1, Follow={0}, Region使用刚性仿射旋转", request.EnablePositionFollow));
                }
                else
                {
                    result.DebugLogs.Add("检测区域ROI：全局检测。");
                }

                if (ShouldRunByPartitions(request, transformedLocalRoi))
                {

                    RunPartitionDetection(gray, transformedLocalRoi, request, transformer, result);
             
                    if (!HasObject(result.RoiRegion) && roi != null)
                        result.RoiRegion = roi.CopyObj(1, -1);
                    result.Success = true;
                    result.Message = result.IsCandidateCountOverflow
                        ? "候选数量超限，已按合并区域输出"
                        : (result.Candidates.Count == 0 ? "未检测到缺陷" : "检测完成");
                    return result;
                }

                scaledGray = BuildScaledGrayImage(gray, roi, request, result);
                if (roi != null)
                    HOperatorSet.ReduceDomain(scaledGray, roi, out reduced);
                else
                    reduced = scaledGray.CopyObj(1, -1);

                int darkMax = request.ReferenceGray - request.DownGrayOffset > 0 ? request.ReferenceGray - request.DownGrayOffset : 1;
                int lightMin = Math.Min(255, request.ReferenceGray + request.UpGrayOffset);
                result.DebugLogs.Add(string.Format("阈值：暗缺陷[0,{0}]，亮缺陷[{1},255]，参考灰={2}", darkMax, lightMin, request.ReferenceGray));

                HOperatorSet.Threshold(reduced, out dark, 0, darkMax);
                HOperatorSet.Threshold(reduced, out light, lightMin, 255);
                MergeSmallRegion(ref dark, request, result, "暗区域");
                MergeSmallRegion(ref light, request, result, "亮区域");

                result.PreprocessImage = scaledGray.CopyObj(1, -1);
                result.DarkRegion = dark.CopyObj(1, -1);
                result.LightRegion = light.CopyObj(1, -1);
                HOperatorSet.AreaCenter(dark, out HTuple darkAreaTotal, out HTuple _, out HTuple __);
                HOperatorSet.AreaCenter(light, out HTuple lightAreaTotal, out HTuple ___, out HTuple ____);
                result.DebugLogs.Add(string.Format("阈值结果：暗区域面积={0:F2}，亮区域面积={1:F2}", darkAreaTotal.D, lightAreaTotal.D));

                HOperatorSet.Union2(dark, light, out union);
                MergeSmallRegion(ref union, request, result, "亮暗合并区域");
                ApplyMaskRois(union, request, transformer, out maskedUnion, out maskUnionRegion);
                result.MaskRoiRegion = maskUnionRegion;
                maskUnionRegion = null;

                if (request.OpenRadius > 0)
                {
                    HOperatorSet.OpeningCircle(maskedUnion, out opened, request.OpenRadius);
                    result.DebugLogs.Add("形态学：OpeningCircle 半径=" + request.OpenRadius);
                }
                else
                {
                    opened = maskedUnion.CopyObj(1, -1);
                    result.DebugLogs.Add("形态学：跳过开运算。");
                }

                if (request.CloseRadius > 0)
                {
                    HOperatorSet.ClosingCircle(opened, out closed, request.CloseRadius);
                    result.DebugLogs.Add("形态学：ClosingCircle 半径=" + request.CloseRadius);
                }
                else
                {
                    closed = opened.CopyObj(1, -1);
                    result.DebugLogs.Add("形态学：跳过闭运算。");
                }

                double pixelToMmH = request.PixelToMmH <= 0 ? 1 : request.PixelToMmH;
                double pixelToMmV = request.PixelToMmV <= 0 ? 1 : request.PixelToMmV;
                double areaScale = pixelToMmH * pixelToMmV;

                HOperatorSet.Connection(closed, out connected);
                HOperatorSet.CountObj(connected, out HTuple connectedCount);
                HOperatorSet.SelectShape(connected, out selected, "area", "and", Math.Max(0, request.MinCandidateArea / areaScale), 999999999);
                HOperatorSet.CountObj(selected, out HTuple count);
                result.PixelToMmH = pixelToMmH;
                result.PixelToMmV = pixelToMmV;
                result.ReferenceGray = request.ReferenceGray;
                result.CandidateCount = count.I;
                result.DebugLogs.Add(string.Format("分辨率：横向={0:F6} mm/pixel，纵向={1:F6} mm/pixel", pixelToMmH, pixelToMmV));
                result.DebugLogs.Add(string.Format("连通域：原始数量={0}，面积筛选后={1}，最小面积={2:F2} mm²", connectedCount.I, count.I, request.MinCandidateArea));

                result.RoiRegion = roi == null ? null : roi.CopyObj(1, -1);

                // 候选数量硬上限必须在 AppendCandidates 之前判断。
                // 一旦超限，直接使用已经完成阈值、屏蔽和形态学后的总缺陷区域输出，
                // 不再逐个 SelectObj、CropRectangle1、Intersection 或 CopyObj，避免超限分支继续耗时。
                if (TryMarkCandidateCountOverflow(result, request, closed, count.I))
                {
                    result.Success = true;
                    result.Message = "候选数量超限，已按合并区域输出";
                    return result;
                }

                result.ResultRegion = selected.CopyObj(1, -1);

                AppendCandidates(
                    result,
                    selected,
                    dark,
                    light,
                    scaledGray,
                    pixelToMmH,
                    pixelToMmV,
                    areaScale,
                    request);

                result.Success = true;
                result.Message = result.Candidates.Count == 0 ? "未检测到缺陷" : "检测完成";
                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "检测异常：" + ex.Message;
                result.DebugLogs.Add(result.Message);
                return result;
            }
            finally
            {
                gray?.Dispose();
                scaledGray?.Dispose();
                roi?.Dispose();
                reduced?.Dispose();
                dark?.Dispose();
                light?.Dispose();
                union?.Dispose();
                maskedUnion?.Dispose();
                maskUnionRegion?.Dispose();
                opened?.Dispose();
                closed?.Dispose();
                connected?.Dispose();
                selected?.Dispose();
            }
        }

        /// <summary>
        /// 将连通域转换成缺陷对象。这里仅写入基础输出字段；
        /// 规则分类使用的 Feature.* 特征由 DefectFeatureExtractor 统一计算。
        /// </summary>
        private static void AppendCandidates(
            DefectDetectResult result,
            HObject selected,
            HObject dark,
            HObject light,
            HObject scaledGray,
            double pixelToMmH,
            double pixelToMmV,
            double areaScale,
            DefectDetectRequest request)
        {
            HOperatorSet.CountObj(selected, out HTuple count);

            for (int i = 1; i <= count.I; i++)
            {
                HObject one = null;
                HObject crop = null;
                HObject Region = null;
                HObject candidateDark = null;
                HObject candidateLight = null;
                try
                {
                    HOperatorSet.SelectObj(selected, out one, i);
                  
                    HOperatorSet.SmallestRectangle1(one, out HTuple row1, out HTuple col1, out HTuple row2, out HTuple col2);
                    
                    HOperatorSet.AreaCenter(one, out HTuple area, out HTuple row, out HTuple col);
                    HOperatorSet.Intensity(one, scaledGray, out HTuple mean, out HTuple _);
                    
                    HOperatorSet.Intersection(one, dark, out candidateDark);
                    HOperatorSet.Intersection(one, light, out candidateLight);

                    double widthPx = Math.Max(0, col2.D - col1.D + 1);
                    double heightPx = Math.Max(0, row2.D - row1.D + 1);
                    double width = widthPx * pixelToMmH;
                    double height = heightPx * pixelToMmV;
                    double areaMm = area.D * areaScale;
                    double length = Math.Max(width, height);
                    double shortSide = Math.Min(width, height);
                    double darkArea = RegionArea(candidateDark);
                    double lightArea = RegionArea(candidateLight);
                    string polarity = lightArea >= darkArea ? "亮缺陷" : "暗缺陷";

                    //HOperatorSet.ZoomRegion(crop,out crop, widthPx*1.3, heightPx*1.3);
                    //HOperatorSet.CropRectangle2(scaledGray, out crop, row, col, 0, (widthPx / 2) <64?64: widthPx / 2 * 1.2, (heightPx/2)<64? 64: heightPx / 2 * 1.1, "true", "constant");
                    //HOperatorSet.ExpandRegion(one,)
                    //HOperatorSet.OpeningRectangle1(one, out Region, 40, 40);
                    //HOperatorSet.SmallestRectangle1(Region, out  row1, out  col1, out  row2, out  col2);
                    HOperatorSet.CropRectangle2(scaledGray,out crop,row,col,0,(widthPx + 20)/2,(heightPx  + 20)/2, "true", "constant");
                    //HOperatorSet.GenRectangle1(out  Region, (row1-10), (col1-10), (row2+10), (col2+10));

                   // HOperatorSet.ReduceDomain(scaledGray, Region, out crop);
                   // HOperatorSet.CropDomain(crop, out crop);

                    // HOperatorSet.CropRectangle1(scaledGray, out crop, row1, col1, row2, col2);
                    //HOperatorSet.CropRectangle2(scaledGray, out crop, row, col, 0, (heightPx / 2) < 64 ? 64 : heightPx / 2 * 1.1, (widthPx / 2) < 64 ? 64 : widthPx / 2 * 1.2, "true", "constant");
                    DefectCandidate candidate = new DefectCandidate
                    {
                        Index = i,
                        Polarity = polarity,
                        Region = one.CopyObj(1, -1),
                        CropImage = crop.CopyObj(1, -1),
                        DarkRegion = candidateDark.CopyObj(1, -1),
                        LightRegion = candidateLight.CopyObj(1, -1),
                        InvalidRegion = result.MaskRoiRegion == null ? null : result.MaskRoiRegion.CopyObj(1, -1),
                        WidthPx = widthPx,
                        HeightPx = heightPx,
                        PixelToMmH = pixelToMmH,
                        PixelToMmV = pixelToMmV,
                        ReferenceGray = request.ReferenceGray
                    };

                    WriteBaseFeatures(candidate, areaMm, area.D, length, shortSide, height, width, heightPx, widthPx, mean.D, row.D, col.D, polarity);

                    result.Candidates.Add(candidate);
                    result.DebugLogs.Add(string.Format(
                        "候选#{0}：{1} Area={2:F2}, L={3:F2}, W={4:F2}, Mean={5:F2}, Row={6:F2}, Col={7:F2}",
                        i, polarity, areaMm, length, shortSide, mean.D, row.D, col.D));
                }
                finally
                {
                  
                    one?.Dispose();
                    crop?.Dispose();
                    Region?.Dispose();
                    candidateDark?.Dispose();
                    candidateLight?.Dispose();
                }
            }
        }

        /// <summary>
        /// 基础检测结果字段，只服务于外层列表、位置显示、结果导出。
        /// 注意：这些字段不是 FeatureXXX 分类特征，分类特征全部使用 FeatureCatalog 中的 Feature.* 键。
        /// </summary>
        private static void WriteBaseFeatures(
            DefectCandidate candidate,
            double areaMm,
            double areaPx,
            double length,
            double shortSide,
            double height,
            double width,
            double heightPx,
            double widthPx,
            double meanGray,
            double row,
            double col,
            string polarity)
        {
            candidate.Features.Set("area", areaMm);
            candidate.Features.Set("area_px", areaPx);
            candidate.Features.Set("length", length);
            candidate.Features.Set("width", shortSide);
            candidate.Features.Set("height", height);
            candidate.Features.Set("width_px", widthPx);
            candidate.Features.Set("height_px", heightPx);
            candidate.Features.Set("mean_gray", meanGray);
            candidate.Features.Set("row", row);
            candidate.Features.Set("column", col);
            candidate.Features.Set("polarity_id", polarity == "亮缺陷" ? 1 : -1);
            candidate.Features.Set("bbox_width", width);
        }

        /// <summary>
        /// 判断 HALCON 对象是否可用。
        /// CountObj 可能因对象未初始化抛异常，因此这里统一吞掉异常并按空对象处理。
        /// </summary>
        private static bool HasObject(HObject obj)
        {
            if (obj == null || !obj.IsInitialized()) return false;
            try
            {
                HOperatorSet.CountObj(obj, out HTuple count);
                return count.I > 0;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>计算 Region 总面积，空对象或异常时返回 0，便于预处理和屏蔽逻辑安全降级。</summary>
        private static double RegionArea(HObject region)
        {
            if (!HasObject(region)) return 0;
            try
            {
                HOperatorSet.AreaCenter(region, out HTuple area, out HTuple _, out HTuple __);
                return area.TupleLength() == 0 ? 0 : area.TupleSum().D;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 判断本次是否走分区检测。
        /// 当前分区只基于局部 Rectangle1 ROI，启用全局检测或没有局部 ROI 时走普通单区流程。
        /// </summary>
        private static bool ShouldRunByPartitions(DefectDetectRequest request, CoatRoiParam localRoi)
        {
            return request != null
                && localRoi != null
                && request.DetectMode == DetectRangeMode.LocalRoi
                && request.EnableLocalRoi;
        }

        /// <summary>
        /// 分区检测。
        /// 分区只沿 Rectangle1 的高度方向切分，宽度不变；每个分区使用自己的阈值、面积和形态学参数。
        /// PartitionCount=1 时也进入这里，相当于一个完整检测区域，不再维护另一套“主检测”逻辑。
        /// 所有分区检测完成后，候选缺陷统一追加到同一个结果中，后续分类仍然只看一份候选列表。
        /// </summary>
        private static void RunPartitionDetection(
            HObject gray,
            CoatRoiParam totalRoi,
            DefectDetectRequest request,
            DefectRoiFollowTransformer transformer,
            DefectDetectResult totalResult)
        {
            // 分区检测总入口。
            // 统一以分区检测为主：PartitionCount=1 时就是一个完整检测区域，多分区时先逐区检测再汇总。
            // 关键要求：AppendCandidates 必须放在总数超限判断之后，否则超限时仍会裁图、求交、复制对象。
            int partitionCount = Math.Max(1, request.PartitionCount);
            
            NormalizeRectangle1(totalRoi);

            bool splitByWideSide = request.PartitionDirection == PartitionSplitDirection.WideSide;
           
            double totalLength = splitByWideSide ? Math.Max(1, totalRoi.Col2 - totalRoi.Col1) : Math.Max(1, totalRoi.Row2 - totalRoi.Row1);
            double fixedWidth = splitByWideSide ? Math.Max(1, totalRoi.Row2 - totalRoi.Row1) : Math.Max(1, totalRoi.Col2 - totalRoi.Col1);

            // NormalizePartitions 只处理运行用副本：
            // 1. 补齐缺失分区；
            // 2. 固定用户手动设置的长度；
            // 3. 将剩余长度平均分给未固定分区。
            var partitions = NormalizePartitions(request, totalLength, partitionCount);
            totalResult.DebugLogs.Add(string.Format("分区检测：方向={0}，分段总长={1:F2}px，固定宽度={2:F2}px，分区数={3}",
                splitByWideSide ? "宽向分段" : "长向分段", totalLength, fixedWidth, partitions.Count));

            double consumedLength = 0;
            int totalCandidateCount = 0;
            List<DefectDetectResult> rawResults = new List<DefectDetectResult>();
            totalResult.PixelToMmH = request.PixelToMmH <= 0 ? 1 : request.PixelToMmH;
            totalResult.PixelToMmV = request.PixelToMmV <= 0 ? 1 : request.PixelToMmV;
            totalResult.ReferenceGray = request.ReferenceGray;

            try
            {
                foreach (CoatPartitionParam partition in partitions)
                {
                    // 未启用分区不检测，但 consumedLength 仍然累加，保证后续分区的位置不前移。
                    if (!partition.Enabled)
                    {
                        consumedLength += partition.Length;
                        totalResult.DebugLogs.Add(string.Format("分区#{0}：未启用，跳过。", partition.Index));
                        continue;
                    }

                    // 根据当前已消耗长度切出本分区 Rectangle1。
                    // LongSide：沿 Row 方向切；WideSide：沿 Col 方向切。
                    CoatRoiParam partitionRoi = BuildPartitionRoi(totalRoi, partition, consumedLength, request.PartitionDirection);
                    consumedLength += partition.Length;

                    // 将分区参数写入子请求。子请求只代表一个分区，所以 PartitionCount 固定为 1。
                    // 子请求仍然保留位置跟随参数，DetectSingleRegion 会对该分区 Region 做 HALCON 仿射。
                    DefectDetectRequest partitionRequest = CloneRequestForPartition(request, partition, partitionRoi);

                    // 第一轮只做原始检测：得到阈值区域、屏蔽区域、连通域筛选结果和候选数量。
                    // 这一步不裁 NG 小图，不生成 DefectCandidate，等待所有分区总数判断通过后再统一追加候选。
                    DefectDetectResult oneResult = DetectSingleRegion(gray, partitionRequest, transformer, partitionRoi, false);
                    rawResults.Add(oneResult);
                    totalCandidateCount += oneResult.CandidateCount;
                    AppendPartitionRawResult(totalResult, oneResult);

                    totalResult.DebugLogs.Add(string.Format(
                        "分区#{0}：长度={1:F2}px，中心Row={2:F2}，中心Col={3:F2}，候选数={4}",
                        partition.Index,
                        partition.Length,
                        GetRoiCenterRow(partitionRoi),
                        GetRoiCenterCol(partitionRoi),
                        oneResult.CandidateCount));
                }

                totalResult.CandidateCount = totalCandidateCount;

                // 所有分区都检测完成后，再按总候选数判断硬上限。
                // 超限时直接使用各分区已经合并好的亮暗区域，不回头逐个构造候选，也不逐个 Union 候选 Region。
                if (TryMarkCandidateCountOverflow(totalResult, request, totalResult.OverflowRegion ?? totalResult.ResultRegion, totalCandidateCount))
                    return;

                foreach (DefectDetectResult oneResult in rawResults)
                {
                    AppendCandidates(
                        totalResult,
                        oneResult.ResultRegion,
                        oneResult.DarkRegion,
                        oneResult.LightRegion,
                        oneResult.PreprocessImage,
                        oneResult.PixelToMmH,
                        oneResult.PixelToMmV,
                        oneResult.PixelToMmH * oneResult.PixelToMmV,
                        request);
                }

                for (int i = 0; i < totalResult.Candidates.Count; i++)
                    totalResult.Candidates[i].Index = i + 1;
            }
            finally
            {
                foreach (DefectDetectResult item in rawResults)
                    item?.Dispose();
            }
        }

        /// <summary>
        /// 归一化 Rectangle1 参数。
        /// Row/Col 四坐标是主数据，不再回写中心点、角度或半宽半高等无效 ROI 参数。
        /// </summary>
        private static void NormalizeRectangle1(CoatRoiParam roi)
        {
            if (roi == null) return;
            double row1 = Math.Min(roi.Row1, roi.Row2);
            double row2 = Math.Max(roi.Row1, roi.Row2);
            double col1 = Math.Min(roi.Col1, roi.Col2);
            double col2 = Math.Max(roi.Col1, roi.Col2);
            roi.Row1 = row1;
            roi.Row2 = row2;
            roi.Col1 = col1;
            roi.Col2 = col2;
        }

        /// <summary>按 Rectangle1 四坐标临时计算中心行，不把中心点保存回 ROI 参数。</summary>
        private static double GetRoiCenterRow(CoatRoiParam roi)
        {
            return roi == null ? 0 : (Math.Min(roi.Row1, roi.Row2) + Math.Max(roi.Row1, roi.Row2)) / 2.0;
        }

        /// <summary>按 Rectangle1 四坐标临时计算中心列，不把中心点保存回 ROI 参数。</summary>
        private static double GetRoiCenterCol(CoatRoiParam roi)
        {
            return roi == null ? 0 : (Math.Min(roi.Col1, roi.Col2) + Math.Max(roi.Col1, roi.Col2)) / 2.0;
        }

        /// <summary>
        /// 生成运行时分区副本。
        /// 只克隆 request.Partitions，不修改 Params 中保存的原始分区集合。
        /// </summary>
        private static System.Collections.Generic.List<CoatPartitionParam> NormalizePartitions(DefectDetectRequest request, double totalLength, int partitionCount)
        {
            var result = new System.Collections.Generic.List<CoatPartitionParam>();
            for (int i = 0; i < partitionCount; i++)
            {
                CoatPartitionParam source = request.Partitions != null && i < request.Partitions.Count && request.Partitions[i] != null
                    ? request.Partitions[i].Clone()
                    : CreateDefaultPartition(request, i + 1);

                source.Index = i + 1;
                source.Name = string.IsNullOrWhiteSpace(source.Name) ? "分区" + source.Index : source.Name;
                result.Add(source);
            }

            double fixedSum = 0;
            int autoCount = 0;
            foreach (CoatPartitionParam item in result)
            {
                if (item.Length <= 0)
                    item.SetLengthFromSystem(totalLength / partitionCount);

                if (item.IsLengthFixed)
                    fixedSum += Math.Max(0, item.Length);
                else
                    autoCount++;
            }

            if (autoCount > 0)
            {
                double autoLength = Math.Max(1, (totalLength - fixedSum) / autoCount);
                foreach (CoatPartitionParam item in result)
                    if (!item.IsLengthFixed)
                        item.SetLengthFromSystem(autoLength);
            }

            return result;
        }

        /// <summary>
        /// 创建默认分区参数。
        /// 当用户未配置足够分区时，默认分区继承当前全局阈值、面积和形态学参数。
        /// </summary>
        private static CoatPartitionParam CreateDefaultPartition(DefectDetectRequest request, int index)
        {
            return new CoatPartitionParam
            {
                Index = index,
                Name = "分区" + index,
                ReferenceGray = request.ReferenceGray,
                UpGrayOffset = request.UpGrayOffset,
                DownGrayOffset = request.DownGrayOffset,
                OpenRadius = request.OpenRadius,
                CloseRadius = request.CloseRadius,
                MinCandidateArea = request.MinCandidateArea,
                EnableBackgroundGrayScale = request.EnableBackgroundGrayScale,
                EnableSmallRegionMerge = request.EnableSmallRegionMerge,
                SmallRegionMergeMaxArea = request.SmallRegionMergeMaxArea,
                SmallRegionMergeWidth = request.SmallRegionMergeWidth,
                SmallRegionMergeHeight = request.SmallRegionMergeHeight,
                MaxGrayScaleAbs = request.MaxGrayScaleAbs
            };
        }

        /// <summary>
        /// 根据已消耗长度从总 Rectangle1 中切出一个分区 ROI。
        /// 这里保持未旋转的 Rectangle1 坐标，位置跟随统一交给 DefectRoiFollowTransformer。
        /// </summary>
        private static CoatRoiParam BuildPartitionRoi(CoatRoiParam totalRoi, CoatPartitionParam partition, double consumedLength, PartitionSplitDirection direction)
        {
            // 构造单分区 ROI。这里故意不做旋转：
            // 分区长度必须基于用户设置的 Rectangle1 原始宽高计算；
            // 旋转跟随交给 DefectRoiFollowTransformer.CreateRegion 统一用 HALCON 仿射实现。
            NormalizeRectangle1(totalRoi);
            bool splitByWideSide = direction == PartitionSplitDirection.WideSide;
            double row1 = totalRoi.Row1;
            double row2 = totalRoi.Row2;
            double col1 = totalRoi.Col1;
            double col2 = totalRoi.Col2;
            if (splitByWideSide)
            {
                col1 = totalRoi.Col1 + consumedLength;
                col2 = Math.Min(totalRoi.Col2, col1 + Math.Max(1, partition.Length));
            }
            else
            {
                row1 = totalRoi.Row1 + consumedLength;
                row2 = Math.Min(totalRoi.Row2, row1 + Math.Max(1, partition.Length));
            }

            return new CoatRoiParam
            {
                Enabled = true,
                Index = partition.Index,
                Name = partition.Name,
                Row1 = row1,
                Col1 = col1,
                Row2 = row2,
                Col2 = col2
            };
        }

        /// <summary>
        /// 为单个分区构造检测请求。
        /// 分区自己的阈值、开闭半径、面积等参数覆盖全局参数，但图像、屏蔽 ROI 和位置跟随信息沿用主请求。
        /// </summary>
        private static DefectDetectRequest CloneRequestForPartition(DefectDetectRequest request, CoatPartitionParam partition, CoatRoiParam partitionRoi)
        {
            // 将“分区自己的算法参数”写入子请求。
            // 这些字段覆盖主默认参数：ReferenceGray、UpGrayOffset、DownGrayOffset、OpenRadius、CloseRadius、
            // MinCandidateArea、EnableBackgroundGrayScale、EnableSmallRegionMerge、SmallRegionMergeMaxArea、
            // SmallRegionMergeWidth、SmallRegionMergeHeight、MaxGrayScaleAbs。
            return new DefectDetectRequest
            {
                Image = request.Image,
                PartitionCount = 1,
                PartitionDirection = request.PartitionDirection,
                ReferenceGray = partition.ReferenceGray,
                UpGrayOffset = partition.UpGrayOffset,
                DownGrayOffset = partition.DownGrayOffset,
                OpenRadius = partition.OpenRadius,
                CloseRadius = partition.CloseRadius,
                MinCandidateArea = partition.MinCandidateArea,
                EnableBackgroundGrayScale = partition.EnableBackgroundGrayScale,
                EnableSmallRegionMerge = partition.EnableSmallRegionMerge,
                SmallRegionMergeMaxArea = partition.SmallRegionMergeMaxArea,
                SmallRegionMergeWidth = partition.SmallRegionMergeWidth,
                SmallRegionMergeHeight = partition.SmallRegionMergeHeight,
                MaxGrayScaleAbs = partition.MaxGrayScaleAbs,
                PixelToMmH = request.PixelToMmH,
                PixelToMmV = request.PixelToMmV,
                EnableCandidateCountLimit = request.EnableCandidateCountLimit,
                MaxCandidateCount = request.MaxCandidateCount,
                OverflowClassName = request.OverflowClassName,
                DetectMode = DetectRangeMode.LocalRoi,
                EnableLocalRoi = true,
                EnableMaskRoi = request.EnableMaskRoi,
                // 分区 ROI 是在原始 Rectangle1 坐标内切出来的，真正检测时仍需走同一套 HALCON 仿射跟随。
                // 不能先把总 ROI 变成旋转后的外接框再切分，否则分区长度和位置都会被放大。
                EnablePositionFollow = request.EnablePositionFollow,
                LocalRoi = partitionRoi,
                MaskRois = request.MaskRois,
                Partitions = null,
                BaseRow = request.BaseRow,
                BaseCol = request.BaseCol,
                BaseAngle = request.BaseAngle,
                CurrentRow = request.CurrentRow,
                CurrentCol = request.CurrentCol,
                CurrentAngle = request.CurrentAngle
            };
        }

        /// <summary>
        /// 单区域检测核心步骤。
        /// appendCandidates=false 时只输出 raw 区域和候选数量，用于分区汇总后再统一判断候选超限。
        /// </summary>
        private static DefectDetectResult DetectSingleRegion(HObject gray, DefectDetectRequest request, DefectRoiFollowTransformer transformer, CoatRoiParam roiParam, bool appendCandidates)
        {
            // 单区域检测 Run。分区检测和单 ROI 检测最终都应该复用这一套算法步骤。
            // 这里的 roiParam 仍然是 Rectangle1 参数；transformer.CreateRegion 会生成实际检测 Region：
            // - 未启用位置跟随：GenRectangle1；
            // - 启用位置跟随：GenRectangle1 后 AffineTransRegion 旋转/平移。
            DefectDetectResult result = new DefectDetectResult();
            HObject roi = null;
            HObject scaledGray = null;
            HObject reduced = null;

            HObject dark = null;
            HObject light = null;
            HObject union = null;
            HObject maskedUnion = null;
            HObject maskUnionRegion = null;
            HObject opened = null;
            HObject closed = null;
            HObject connected = null;
            HObject selected = null;

            try
            {
                transformer.CreateRegion(roiParam, out roi);
                scaledGray = BuildScaledGrayImage(gray, roi, request, result);
                HOperatorSet.ReduceDomain(scaledGray, roi, out reduced);

                int darkMax = request.ReferenceGray - request.DownGrayOffset > 0 ? request.ReferenceGray - request.DownGrayOffset : 1;
                int lightMin = Math.Min(255, request.ReferenceGray + request.UpGrayOffset);
                HOperatorSet.Threshold(reduced, out dark, 0, darkMax);
                HOperatorSet.Threshold(reduced, out light, lightMin, 255);
                MergeSmallRegion(ref dark, request, result, "暗区域");
                MergeSmallRegion(ref light, request, result, "亮区域");

                HOperatorSet.Union2(dark, light, out union);
                MergeSmallRegion(ref union, request, result, "亮暗合并区域");
                ApplyMaskRois(union, request, transformer, out maskedUnion, out maskUnionRegion);
                result.MaskRoiRegion = maskUnionRegion;
                maskUnionRegion = null;

                if (request.OpenRadius > 0)
                    HOperatorSet.OpeningCircle(maskedUnion, out opened, request.OpenRadius);
                else
                    opened = maskedUnion.CopyObj(1, -1);

                if (request.CloseRadius > 0)
                    HOperatorSet.ClosingCircle(opened, out closed, request.CloseRadius);
                else
                    closed = opened.CopyObj(1, -1);

                double pixelToMmH = request.PixelToMmH <= 0 ? 1 : request.PixelToMmH;
                double pixelToMmV = request.PixelToMmV <= 0 ? 1 : request.PixelToMmV;
                double areaScale = pixelToMmH * pixelToMmV;
                HOperatorSet.Connection(closed, out connected);
                HOperatorSet.SelectShape(connected, out selected, "area", "and", Math.Max(0, request.MinCandidateArea / areaScale), 999999999);
                HOperatorSet.CountObj(selected, out HTuple count);

                result.PixelToMmH = pixelToMmH;
                result.PixelToMmV = pixelToMmV;
                result.ReferenceGray = request.ReferenceGray;
                result.CandidateCount = count.I;
                result.RoiRegion = roi.CopyObj(1, -1);
                result.ResultRegion = selected.CopyObj(1, -1);
                result.PreprocessImage = scaledGray.CopyObj(1, -1);
                result.DarkRegion = dark.CopyObj(1, -1);
                result.LightRegion = light.CopyObj(1, -1);

                // raw 模式用于分区汇总阶段：只保留已经算好的区域和数量。
                // 不在这里裁 NG 小图，不生成候选对象，等待所有分区总数判断通过后再统一 AppendCandidates。
                result.OverflowRegion = closed.CopyObj(1, -1);
                if (appendCandidates)
                    AppendCandidates(result, selected, dark, light, scaledGray, pixelToMmH, pixelToMmV, areaScale, request);

                return result;
            }
            finally
            {
                roi?.Dispose();
                scaledGray?.Dispose();
                reduced?.Dispose();
                dark?.Dispose();
                light?.Dispose();
                union?.Dispose();
                maskedUnion?.Dispose();
                maskUnionRegion?.Dispose();
                opened?.Dispose();
                closed?.Dispose();
                connected?.Dispose();
                selected?.Dispose();
            }
        }

        /// <summary>
        /// 将一个分区 raw 检测结果合并到总结果。
        /// 这里只合并区域、日志和候选数量，不复制候选对象，因为候选对象要等总数超限判断后再生成。
        /// </summary>
        private static void AppendPartitionRawResult(DefectDetectResult total, DefectDetectResult one)
        {
            if (one == null) return;
            foreach (string log in one.DebugLogs)
                total.DebugLogs.Add(log);

            HObject resultRegion = total.ResultRegion;
            AppendRegion(ref resultRegion, one.ResultRegion);
            total.ResultRegion = resultRegion;

            HObject roiRegion = total.RoiRegion;
            AppendRegionObject(ref roiRegion, one.RoiRegion);
            total.RoiRegion = roiRegion;

            HObject maskRegion = total.MaskRoiRegion;
            AppendRegion(ref maskRegion, one.MaskRoiRegion);
            total.MaskRoiRegion = maskRegion;

            HObject darkRegion = total.DarkRegion;
            AppendRegion(ref darkRegion, one.DarkRegion);
            total.DarkRegion = darkRegion;

            HObject lightRegion = total.LightRegion;
            AppendRegion(ref lightRegion, one.LightRegion);
            total.LightRegion = lightRegion;

            // OverflowRegion 在 raw 阶段临时保存“亮暗合并并完成形态学后的检测区域”。
            // 超限时直接用它作为总 NG 区域，不再逐个候选 Union。
            HObject overflowRegion = total.OverflowRegion;
            AppendRegion(ref overflowRegion, one.OverflowRegion);
            total.OverflowRegion = overflowRegion;

            if (total.PreprocessImage == null)
                total.PreprocessImage = CopyHObject(one.PreprocessImage);
        }

        /// <summary>
        /// 用 Union2 合并两个 Region。
        /// target 会被替换为新 Region，旧 target 在方法内释放；source 只读，不释放。
        /// </summary>
        private static void AppendRegion(ref HObject target, HObject source)
        {
            if (!HasObject(source)) return;
            if (target == null || !target.IsInitialized())
            {
                target = source.CopyObj(1, -1);
                return;
            }

            HObject merged = null;
            try
            {
                HOperatorSet.Union2(target, source, out merged);
                target.Dispose();
                target = merged;
                merged = null;
            }
            finally
            {
                merged?.Dispose();
            }
        }

        /// <summary>
        /// 用 ConcatObj 追加 ROI 对象集合。
        /// ROI 显示需要保留多个对象边界时使用，区别于 Union2 的几何合并。
        /// </summary>
        private static void AppendRegionObject(ref HObject target, HObject source)
        {
            if (!HasObject(source)) return;
            if (target == null || !target.IsInitialized())
            {
                target = source.CopyObj(1, -1);
                return;
            }

            HObject concat = null;
            try
            {
                HOperatorSet.ConcatObj(target, source, out concat);
                target.Dispose();
                target = concat;
                concat = null;
            }
            finally
            {
                concat?.Dispose();
            }
        }

        /// <summary>
        /// 检测阶段候选数量硬上限判断。
        /// 触发后记录合并区域，后续 Pipeline 直接按合并区域构造一个输出结果。
        /// </summary>
        private static bool TryMarkCandidateCountOverflow(DefectDetectResult result, DefectDetectRequest request, HObject mergedDefectRegion, int candidateCount)
        {
            result.CandidateCount = candidateCount;
            if (request == null || !request.EnableCandidateCountLimit || request.MaxCandidateCount <= 0 || candidateCount <= request.MaxCandidateCount)
                return false;

            result.IsCandidateCountOverflow = true;
            result.OverflowClassName = string.IsNullOrWhiteSpace(request.OverflowClassName) ? "涂布不良" : request.OverflowClassName.Trim();
            result.OverflowCandidateCount = candidateCount;
            result.OverflowCandidateLimit = request.MaxCandidateCount;
            result.DebugLogs.Add(string.Format(
                "检测阶段候选数量超限：候选数={0}，上限={1}，直接输出{2}，不生成单个候选。",
                candidateCount,
                request.MaxCandidateCount,
                result.OverflowClassName));

            // mergedDefectRegion 可能就是 result.OverflowRegion 本身。
            // 必须先复制到临时对象，再释放旧输出，避免把准备复制的 HObject 先 Dispose 掉。
            HObject overflowCopy = CopyHObject(mergedDefectRegion);
            if (overflowCopy == null)
                HOperatorSet.GenEmptyRegion(out overflowCopy);

            result.OverflowRegion?.Dispose();
            result.OverflowRegion = overflowCopy;

            result.ResultRegion?.Dispose();
            result.ResultRegion = result.OverflowRegion.CopyObj(1, -1);
            return true;
        }

        /// <summary>复制一个可用 HObject；返回值由调用方负责释放。</summary>
        private static HObject CopyHObject(HObject source)
        {
            if (!HasObject(source)) return null;
            return source.CopyObj(1, -1);
        }

        /// <summary>
        /// 背景灰度归一化。源程序通过检测区外背景平均灰度，把图像缩放到 ReferenceGray 后再阈值。
        /// </summary>
        private static HObject BuildScaledGrayImage(HObject gray, HObject roi, DefectDetectRequest request, DefectDetectResult result)
        {
            if (!request.EnableBackgroundGrayScale)
            {
                result.PreprocessInfo = "预处理：跳过背景灰度归一化，Scale=1.000000";
                result.DebugLogs.Add(result.PreprocessInfo);
                return gray.CopyObj(1, -1);
            }

            HObject background = null;
            try
            {
                background = BuildRingOutRegion(gray, roi, result);

                if (!HasObject(background) || RegionArea(background) <= 0)
                {
                    result.PreprocessInfo = "预处理：背景区域为空，按源程序策略跳过 ScaleImage";
                    result.DebugLogs.Add(result.PreprocessInfo);
                    return gray.CopyObj(1, -1);
                }

                HOperatorSet.Intensity(background, gray, out HTuple meanGray, out HTuple deviation);
                double mean = meanGray.D;
                double scale = mean > 0 ? request.ReferenceGray / mean : 1.0;
                double maxAbs = request.MaxGrayScaleAbs <= 0 ? 1024 : request.MaxGrayScaleAbs;
                if (Math.Abs(scale) > maxAbs)
                    scale = 1.0;

                HOperatorSet.ScaleImage(gray, out HObject scaled, scale, 0);
                result.PreprocessInfo = string.Format("预处理：背景均值={0:F3}，方差={1:F3}，Scale={2:F6}", mean, deviation.D, scale);
                result.DebugLogs.Add(result.PreprocessInfo);
                return scaled;
            }
            catch (Exception ex)
            {
                result.PreprocessInfo = "预处理：背景灰度归一化失败，回退原图。" + ex.Message;
                result.DebugLogs.Add(result.PreprocessInfo);
                return gray.CopyObj(1, -1);
            }
            finally
            {
                background?.Dispose();
            }
        }

        /// <summary>
        /// 生成背景灰度区域。多连通 ROI 时按源程序 SelectObj(2) 思路取外环，否则用整图 Domain 减检测 ROI。
        /// </summary>
        private static HObject BuildRingOutRegion(HObject gray, HObject roi, DefectDetectResult result)
        {
            HObject domain = null;
            HObject connection = null;
            HObject background = null;
            try
            {
                HOperatorSet.GetDomain(gray, out domain);
                if (roi == null || !roi.IsInitialized())
                {
                    background = domain.CopyObj(1, -1);
                    result.DebugLogs.Add("PretreatmentImg：无检测ROI，使用整图Domain作为背景灰度区域。");
                    return background;
                }

                HOperatorSet.Connection(roi, out connection);
                HOperatorSet.CountObj(connection, out HTuple count);
                if (count.I > 1)
                {
                    HOperatorSet.SelectObj(connection, out background, 2);
                    result.DebugLogs.Add("PretreatmentImg：检测ROI存在多个连通域，按源算法 SelectObj(2) 作为背景区域。");
                }
                else
                {
                    HOperatorSet.Difference(domain, roi, out background);
                    result.DebugLogs.Add("PretreatmentImg：检测ROI单连通域，使用 GetDomain - ROI 作为背景区域。");
                }

                return background;
            }
            catch
            {
                background?.Dispose();
                throw;
            }
            finally
            {
                domain?.Dispose();
                connection?.Dispose();
            }
        }

        /// <summary>
        /// 小区域合并。源程序只在最大区域面积小于阈值时执行 ClosingRectangle1，避免大缺陷合并过慢。
        /// </summary>
        private static void MergeSmallRegion(ref HObject region, DefectDetectRequest request, DefectDetectResult result, string name)
        {
            if (!request.EnableSmallRegionMerge || !HasObject(region)) return;

            double maxArea = request.SmallRegionMergeMaxArea <= 0 ? 1000 : request.SmallRegionMergeMaxArea;
            int width = Math.Max(1, request.SmallRegionMergeWidth);
            int height = Math.Max(1, request.SmallRegionMergeHeight);
            try
            {
                HOperatorSet.AreaCenter(region, out HTuple area, out HTuple _, out HTuple __);
                double tupleMax = area.TupleLength() == 0 ? 0 : area.TupleMax().D;
                if (tupleMax <= 0 || tupleMax >= maxArea)
                {
                    result.DebugLogs.Add(string.Format("PretreatmentImg：{0} 最大面积={1:F2}，不执行 ClosingRectangle1。", name, tupleMax));
                    return;
                }

                HObject old = region;
                HOperatorSet.ClosingRectangle1(old, out region, width, height);
                old.Dispose();
                result.DebugLogs.Add(string.Format("PretreatmentImg：{0} 最大面积={1:F2} < {2:F2}，执行 ClosingRectangle1({3},{4})。", name, tupleMax, maxArea, width, height));
            }
            catch (Exception ex)
            {
                result.DebugLogs.Add("PretreatmentImg：" + name + " 小区域合并失败，保留原区域。" + ex.Message);
            }
        }

        /// <summary>
        /// 应用屏蔽 ROI。所有屏蔽 ROI 先跟随位姿转换，再合成一个总屏蔽区，最后从检测结果中扣除。
        /// </summary>
        private static void ApplyMaskRois(HObject source, DefectDetectRequest request, DefectRoiFollowTransformer transformer, out HObject output, out HObject unionMaskOutput)
        {
            output = source.CopyObj(1, -1);
            unionMaskOutput = null;
            if (!request.EnableMaskRoi || request.MaskRois == null) return;

            HObject unionMask = null;
            foreach (CoatRoiParam roiParam in request.MaskRois)
            {
                if (roiParam == null || !roiParam.Enabled) continue;
                HObject mask = null;
                HObject concat = null;
                try
                {
                    transformer.CreateRegion(roiParam, out mask);
                    if (unionMask == null)
                    {
                        unionMask = mask.CopyObj(1, -1);
                    }
                    else
                    {
                        HOperatorSet.Union2(unionMask, mask, out concat);
                        unionMask.Dispose();
                        unionMask = concat;
                        concat = null;
                    }
                }
                finally
                {
                    mask?.Dispose();
                    concat?.Dispose();
                }
            }

            if (unionMask == null) return;
            unionMaskOutput = unionMask.CopyObj(1, -1);
            HObject diff = null;
            try
            {
                HOperatorSet.Difference(output, unionMask, out diff);
                output.Dispose();
                output = diff;
                diff = null;
            }
            finally
            {
                unionMask.Dispose();
                diff?.Dispose();
            }
        }

    }
}



using DefectRuleClassifier.Core.Models;
using HalconDotNet;
using System;

namespace DefectRuleClassifier.Core.Features
{
    /// <summary>
    /// 单个缺陷的特征计算上下文。
    /// 检测器负责生成这些 HALCON 对象，本类只读取上下文计算 FeatureXXX 特征值。
    /// </summary>
    public sealed class DefectFeatureContext
    {
        /// <summary>灰度归一化后的图像，对应源程序 hScalImg。</summary>
        public HObject Image { get; set; }

        /// <summary>当前缺陷区域，对应源程序 hoRgnDefect。</summary>
        public HObject DefectRegion { get; set; }

        /// <summary>当前缺陷与暗阈值区域的交集，对应源程序 hoDarkRegion。</summary>
        public HObject DarkRegion { get; set; }

        /// <summary>当前缺陷与亮阈值区域的交集，对应源程序 hoLightRegion。</summary>
        public HObject LightRegion { get; set; }

        /// <summary>屏蔽 ROI 或其它无效区域，对应源程序 hoRgnInvalid。</summary>
        public HObject InvalidRegion { get; set; }

        /// <summary>缺陷横向像素尺寸，对应源程序传入 GetFeatureValue 的 dWidth。</summary>
        public double WidthPx { get; set; }

        /// <summary>缺陷纵向像素尺寸，对应源程序传入 GetFeatureValue 的 dHeight。</summary>
        public double HeightPx { get; set; }

        /// <summary>横向单量，mm/pixel，用于面积换算。</summary>
        public double PixelToMmH { get; set; } = 1;

        /// <summary>纵向单量，mm/pixel，用于面积换算。</summary>
        public double PixelToMmV { get; set; } = 1;

        /// <summary>参考灰度，源程序默认 REFERENCE_GRAY=100，demo 中开放为参数。</summary>
        public int ReferenceGray { get; set; }
    }

    /// <summary>
    /// 缺陷分类特征提取器。
    /// 这里承接源程序 GetFeatureValue 的职责：根据 PicFeatureType 分发到对应 FeatureXXX 方法，
    /// 并把每个缺陷的全部特征写入 DefectFeatureBag，供分类规则和结果明细直接读取。
    /// </summary>
    public sealed class DefectFeatureExtractor
    {
        /// <summary>
        /// 计算所有分类特征。基础几何输出 area/length/width 等由检测器单独写入，
        /// 本方法只写 Feature.* 键，避免基础字段冒充分类特征。
        /// </summary>
        public void ExtractAll(DefectFeatureContext context, DefectFeatureBag bag)
        {
            if (context == null || bag == null) return;

            foreach (FeatureTypeOption option in FeatureCatalog.All)
                bag.Set(option.FeatureKey, GetFeatureValue(option.Value, context));
        }

        /// <summary>
        /// 按特征枚举获取单项特征值。未命中或区域为空时保持源程序风格返回 -1/-999999 等哨兵值。
        /// </summary>
        public double GetFeatureValue(PicFeatureType feature, DefectFeatureContext c)
        {
            switch (feature)
            {
                case PicFeatureType.DefectGray: return FeatureDefectGray(c.Image, c.DefectRegion);
                case PicFeatureType.BrightDarkDiff: return FeatureBrightDarkDiff(c.Image, c.DarkRegion, c.LightRegion);
                case PicFeatureType.DefectContourChange: return FeatureDefectContourChange(c.Image, c.DefectRegion);
                case PicFeatureType.DarkGray: return FeatureDarkGray(c.Image, c.DarkRegion);
                case PicFeatureType.UltraDarkGray: return FeatureUltraDarkGray(c.Image, c.DarkRegion);
                case PicFeatureType.DarkContourChange: return FeatureDarkContourChange(c.Image, c.DarkRegion);
                case PicFeatureType.BrightGray: return FeatureBrightGray(c.Image, c.LightRegion);
                case PicFeatureType.UltraBrightGray: return FeatureUltraBrightGray(c.Image, c.LightRegion);
                case PicFeatureType.BrightContourChange: return FeatureBrightContourChange(c.Image, c.LightRegion);
                case PicFeatureType.WidthHeightRate: return FeatureWidthHeightRate(c.WidthPx, c.HeightPx);
                case PicFeatureType.HorVerRate: return FeatureHorVerRate(c.WidthPx, c.HeightPx);
                case PicFeatureType.DarkBrightAreaRatio: return FeatureDarkBrightAreaRatio(c.DarkRegion, c.LightRegion);
                case PicFeatureType.BrightOfBubbleRate: return FeatureBrightOfBubbleRate(c.Image, c.DarkRegion, c.DefectRegion, c.InvalidRegion, c.ReferenceGray);
                case PicFeatureType.MinCoateRate: return FeatureMinCoateRate();
                case PicFeatureType.BrightDarkDistanceRate: return FeatureBrightDarkDistanceRate(c.DarkRegion, c.LightRegion);
                case PicFeatureType.BrightDarkAreaRate: return FeatureBrightDarkAreaRate(c.DarkRegion, c.LightRegion);
                case PicFeatureType.BrightArea: return FeatureAreaMm(c.LightRegion, c);
                case PicFeatureType.DarkArea: return FeatureAreaMm(c.DarkRegion, c);
                case PicFeatureType.DecarburizationBlackZone: return FeatureDecarburizationBlackZone(c.Image, c.DefectRegion, c);
                case PicFeatureType.DarkGradient: return FeatureGradient(c.Image, c.DarkRegion, 5, 10);
                case PicFeatureType.BubbleOuterRoundness: return FeatureBubbleOuterRoundness(c.Image, c.DefectRegion);
                case PicFeatureType.BubbleOuterDarkConcentricity: return FeatureBubbleOuterDarkConcentricity(c.Image, c.DefectRegion);
                case PicFeatureType.DarkWidthHeightRate: return FeatureDarkWidthHeightRate(c.DarkRegion);
                case PicFeatureType.DefectGradient: return FeatureGradient(c.Image, c.DefectRegion, 5, 10);
                case PicFeatureType.DefectGradientCount: return FeatureDefectGradientCount(c.Image, c.DefectRegion, c);
                case PicFeatureType.DefectBright: return FeatureDefectBright(c.Image, c.DefectRegion);
                case PicFeatureType.BrightEdgeGradient: return FeatureEdgeGradient(c.Image, c.LightRegion, true);
                case PicFeatureType.DarkEdgeGradient: return FeatureEdgeGradient(c.Image, c.DarkRegion, false);
                case PicFeatureType.DarkBrightWidthHeightRate: return FeatureDarkBrightWidthHeightRate(c.DarkRegion, c.LightRegion);
                case PicFeatureType.BubbleDarkRoundness: return FeatureBubbleDarkRoundness(c.DefectRegion);
                case PicFeatureType.Lengthness: return FeatureLengthness(c.DefectRegion);
                case PicFeatureType.Convexity: return RegionFeature(c.DefectRegion, "convexity", -1);
                case PicFeatureType.Holes: return RegionFeature(c.LightRegion, "holes_num", -1);
                case PicFeatureType.Rect2_len1: return RegionFeature(c.DefectRegion, "rect2_len1", -1);
                case PicFeatureType.Rect2_len2: return RegionFeature(c.DefectRegion, "rect2_len2", -1);
                case PicFeatureType.DefectRoundness: return FeatureBubbleDarkRoundness(c.DefectRegion);
                case PicFeatureType.LightAreaRate: return FeatureAreaRate(c.LightRegion, c.DefectRegion);
                case PicFeatureType.DarkAreaRate: return FeatureAreaRate(c.DarkRegion, c.DefectRegion);
                default: return -9999;
            }
        }

        private static double FeatureDefectGray(HObject image, HObject defectRegion)
        {
            return GrayFeature(defectRegion, image, "mean", -999999);
        }

        private static double FeatureBrightDarkDiff(HObject image, HObject darkRegion, HObject lightRegion)
        {
            double minDark = 0;
            double maxLight = 0;
            if (!IsRegionEmpty(darkRegion))
                minDark = MinMaxGray(darkRegion, image).Min;
            if (!IsRegionEmpty(lightRegion))
                maxLight = MinMaxGray(lightRegion, image).Max;
            return minDark > 0 && maxLight > 0 ? maxLight - minDark : -1;
        }

        private static double FeatureDefectContourChange(HObject image, HObject defectRegion)
        {
            return FeatureGradient(image, defectRegion, 3, 5);
        }

        private static double FeatureDarkGray(HObject image, HObject darkRegion)
        {
            return GrayFeature(darkRegion, image, "mean", -1);
        }

        private static double FeatureUltraDarkGray(HObject image, HObject darkRegion)
        {
            return ThresholdMean(image, darkRegion, 0, 40, -1);
        }

        private static double FeatureDarkContourChange(HObject image, HObject darkRegion)
        {
            return FeatureGradient(image, darkRegion, 3, 5);
        }

        private static double FeatureBrightGray(HObject image, HObject lightRegion)
        {
            return GrayFeature(lightRegion, image, "mean", -1);
        }

        private static double FeatureUltraBrightGray(HObject image, HObject lightRegion)
        {
            return ThresholdMean(image, lightRegion, 180, 255, -999999);
        }

        private static double FeatureBrightContourChange(HObject image, HObject lightRegion)
        {
            return FeatureGradient(image, lightRegion, 3, 5);
        }

        private static double FeatureWidthHeightRate(double width, double height)
        {
            if (width <= 0 || height <= 0) return 0.001;
            double min = Math.Min(width, height);
            double max = Math.Max(width, height);
            return min / max;
        }

        private static double FeatureHorVerRate(double width, double height)
        {
            if (height <= 0) return 999999;
            return Clamp(width / height, 0.001, 999999);
        }

        private static double FeatureDarkBrightAreaRatio(HObject darkRegion, HObject lightRegion)
        {
            double darkArea = RegionArea(darkRegion);
            double lightArea = RegionArea(lightRegion);
            if (darkArea > 0 && lightArea > 0) return Clamp(darkArea / lightArea, 0.001, 999);
            if (lightArea > 0) return 0.001;
            if (darkArea > 0) return 999;
            return -1;
        }

        private static double FeatureBrightOfBubbleRate(HObject image, HObject darkRegion, HObject defectRegion, HObject invalidRegion, int referenceGray)
        {
            if (IsRegionEmpty(darkRegion) || IsRegionEmpty(defectRegion)) return 0.001;

            HObject dilated = null;
            HObject validArea = null;
            HObject reduced = null;
            HObject ringInner = null;
            HObject closed = null;
            HObject opened = null;
            HObject ringOut = null;
            HObject ringValid = null;
            HObject ringImage = null;
            HObject bright = null;
            HObject ringDilated = null;
            try
            {
                HOperatorSet.DilationRectangle1(defectRegion, out dilated, 60, 60);
                validArea = DifferenceIfNeeded(dilated, invalidRegion);
                HOperatorSet.ReduceDomain(image, validArea, out reduced);
                HOperatorSet.Threshold(reduced, out ringInner, 0, referenceGray - 15);
                HOperatorSet.ClosingRectangle1(ringInner, out closed, 4, 4);
                HOperatorSet.OpeningRectangle1(closed, out opened, 4, 4);
                HOperatorSet.DilationCircle(opened, out ringDilated, 1.5);
                HOperatorSet.Difference(ringDilated, opened, out ringOut);
                ringValid = DifferenceIfNeeded(ringOut, invalidRegion);
                HOperatorSet.ReduceDomain(image, ringValid, out ringImage);
                HOperatorSet.Threshold(ringImage, out bright, referenceGray + 10, 255);

                double ringArea = RegionArea(ringValid);
                double brightArea = RegionArea(bright);
                return ringArea > 0 && brightArea > 0 ? Math.Max(brightArea / ringArea, 0.001) : 0.001;
            }
            catch
            {
                return 0.001;
            }
            finally
            {
                dilated?.Dispose();
                validArea?.Dispose();
                reduced?.Dispose();
                ringInner?.Dispose();
                closed?.Dispose();
                opened?.Dispose();
                ringOut?.Dispose();
                ringValid?.Dispose();
                ringImage?.Dispose();
                bright?.Dispose();
                ringDilated?.Dispose();
            }
        }

        private static double FeatureMinCoateRate()
        {
            return -1;
        }

        private static double FeatureBrightDarkDistanceRate(HObject darkRegion, HObject lightRegion)
        {
            if (IsRegionEmpty(darkRegion) || IsRegionEmpty(lightRegion)) return -1;

            try
            {
                HOperatorSet.SmallestCircle(lightRegion, out HTuple lightRow, out HTuple lightCol, out HTuple lightRadius);
                HOperatorSet.SmallestCircle(darkRegion, out HTuple darkRow, out HTuple darkCol, out HTuple darkRadius);
                double distance = Math.Sqrt(Math.Pow(lightCol.D - darkCol.D, 2) + Math.Pow(lightRow.D - darkRow.D, 2));
                return distance >= 1 ? (lightRadius.D + darkRadius.D) / distance : -1;
            }
            catch
            {
                return -1;
            }
        }

        private static double FeatureBrightDarkAreaRate(HObject darkRegion, HObject lightRegion)
        {
            double darkArea = RegionArea(darkRegion);
            double lightArea = RegionArea(lightRegion);
            return darkArea > 0 && lightArea > 0 ? lightArea / darkArea : -1;
        }

        private static double FeatureAreaMm(HObject region, DefectFeatureContext c)
        {
            if (IsRegionEmpty(region)) return -1;
            return RegionArea(region) * PixelArea(c);
        }

        private static double FeatureDecarburizationBlackZone(HObject image, HObject defectRegion, DefectFeatureContext c)
        {
            if (IsRegionEmpty(defectRegion)) return -1;

            HObject reduced = null;
            HObject threshold = null;
            HObject connection = null;
            HObject selected = null;
            HObject union = null;
            try
            {
                HOperatorSet.ReduceDomain(image, defectRegion, out reduced);
                HOperatorSet.Threshold(reduced, out threshold, 0, 30);
                HOperatorSet.Connection(threshold, out connection);
                HOperatorSet.SelectShapeStd(connection, out selected, "max_area", 70);
                HOperatorSet.Union1(selected, out union);
                return IsRegionEmpty(union) ? -1 : RegionArea(union) * PixelArea(c);
            }
            catch
            {
                return -1;
            }
            finally
            {
                reduced?.Dispose();
                threshold?.Dispose();
                connection?.Dispose();
                selected?.Dispose();
                union?.Dispose();
            }
        }

        private static double FeatureGradient(HObject image, HObject region, int sobelSize, int percent)
        {
            if (IsRegionEmpty(region)) return -1;

            HObject reduced = null;
            HObject edge = null;
            try
            {
                HOperatorSet.ReduceDomain(image, region, out reduced);
                HOperatorSet.GetImageSize(image, out HTuple width, out HTuple height);
                if (width.D <= 5 || height.D <= 5) return -1;
                HOperatorSet.SobelAmp(reduced, out edge, "sum_abs", sobelSize);
                return MinMaxGray(region, edge, percent).Max;
            }
            catch
            {
                return -1;
            }
            finally
            {
                reduced?.Dispose();
                edge?.Dispose();
            }
        }

        private static double FeatureBubbleOuterRoundness(HObject image, HObject defectRegion)
        {
            HObject ring = null;
            try
            {
                ring = GenBubbleCircle(image, defectRegion);
                return IsRegionEmpty(ring) ? -1 : Circularity(ring);
            }
            finally
            {
                ring?.Dispose();
            }
        }

        private static double FeatureBubbleOuterDarkConcentricity(HObject image, HObject defectRegion)
        {
            if (IsRegionEmpty(defectRegion)) return -1;

            HObject ring = null;
            HObject intersection = null;
            try
            {
                ring = GenBubbleCircle(image, defectRegion);
                if (IsRegionEmpty(ring)) return -1;
                HOperatorSet.Intersection(defectRegion, ring, out intersection);
                double defectArea = RegionArea(defectRegion);
                return defectArea > 0 ? RegionArea(intersection) / defectArea : -1;
            }
            catch
            {
                return -1;
            }
            finally
            {
                ring?.Dispose();
                intersection?.Dispose();
            }
        }

        private static double FeatureDarkWidthHeightRate(HObject darkRegion)
        {
            if (IsRegionEmpty(darkRegion)) return -1;
            try
            {
                HOperatorSet.Eccentricity(darkRegion, out HTuple anisometry, out HTuple _, out HTuple __);
                return anisometry.D;
            }
            catch
            {
                return -1;
            }
        }

        private static double FeatureDefectGradientCount(HObject image, HObject defectRegion, DefectFeatureContext c)
        {
            if (IsRegionEmpty(defectRegion)) return -1;

            HObject reduced = null;
            HObject edge = null;
            HObject edgeRegion = null;
            try
            {
                HOperatorSet.ReduceDomain(image, defectRegion, out reduced);
                HOperatorSet.GetImageSize(image, out HTuple width, out HTuple height);
                if (width.D <= 5 || height.D <= 5) return -1;
                HOperatorSet.SobelAmp(reduced, out edge, "sum_abs", 5);
                HOperatorSet.Threshold(edge, out edgeRegion, 40, 255);
                return RegionArea(edgeRegion) * PixelArea(c);
            }
            catch
            {
                return -1;
            }
            finally
            {
                reduced?.Dispose();
                edge?.Dispose();
                edgeRegion?.Dispose();
            }
        }

        private static double FeatureDefectBright(HObject image, HObject defectRegion)
        {
            return GrayFeature(defectRegion, image, "mean", -1);
        }

        private static double FeatureEdgeGradient(HObject image, HObject region, bool brightEdge)
        {
            if (IsRegionEmpty(region)) return -1;

            HObject dilation = null;
            HObject erosion = null;
            HObject edgeBand = null;
            HObject reduced = null;
            HObject edgeImage = null;
            HObject edgeRegion = null;
            try
            {
                HOperatorSet.DilationCircle(region, out dilation, 3.5);
                HOperatorSet.ErosionCircle(region, out erosion, 3.5);
                HOperatorSet.Difference(dilation, erosion, out edgeBand);
                HOperatorSet.ReduceDomain(image, edgeBand, out reduced);
                HOperatorSet.GetImageSize(reduced, out HTuple width, out HTuple height);
                if (width.D <= 5 || height.D <= 5) return -1;

                HOperatorSet.SobelAmp(reduced, out edgeImage, "sum_abs", 5);
                var grayRange = MinMaxGray(edgeBand, image, 10);
                var edgeRange = MinMaxGray(edgeBand, edgeImage, 10);
                if (brightEdge) return edgeRange.Max;

                HOperatorSet.Threshold(edgeImage, out edgeRegion, 40, 55);
                return grayRange.Min > 40 && edgeRange.Max > 100 ? grayRange.Max : -1;
            }
            catch
            {
                return -1;
            }
            finally
            {
                dilation?.Dispose();
                erosion?.Dispose();
                edgeBand?.Dispose();
                reduced?.Dispose();
                edgeImage?.Dispose();
                edgeRegion?.Dispose();
            }
        }

        private static double FeatureDarkBrightWidthHeightRate(HObject darkRegion, HObject lightRegion)
        {
            if (IsRegionEmpty(darkRegion) || IsRegionEmpty(lightRegion)) return -1;

            HObject union = null;
            try
            {
                HOperatorSet.Union2(lightRegion, darkRegion, out union);
                HOperatorSet.Eccentricity(union, out HTuple anisometry, out HTuple _, out HTuple __);
                return anisometry.D;
            }
            catch
            {
                return -1;
            }
            finally
            {
                union?.Dispose();
            }
        }

        private static double FeatureBubbleDarkRoundness(HObject region)
        {
            return IsRegionEmpty(region) ? -1 : Circularity(region);
        }

        private static double FeatureLengthness(HObject defectRegion)
        {
            double len1 = RegionFeature(defectRegion, "rect2_len1", -1);
            double len2 = RegionFeature(defectRegion, "rect2_len2", -1);
            return len1 > 0 && len2 >= 0 ? len2 / len1 : -1;
        }

        private static double FeatureAreaRate(HObject partRegion, HObject defectRegion)
        {
            double total = RegionArea(defectRegion);
            if (total <= 0 || IsRegionEmpty(partRegion)) return 0;
            return RegionArea(partRegion) / total;
        }

        private static HObject GenBubbleCircle(HObject image, HObject region)
        {
            HObject empty = null;
            HOperatorSet.GenEmptyRegion(out empty);
            if (!HasObject(image) || IsRegionEmpty(region)) return empty;

            HObject domain = null;
            HObject circleIn = null;
            HObject circleOut = null;
            HObject ring = null;
            HObject median = null;
            HObject refer = null;
            HObject sub = null;
            HObject scaleMax = null;
            HObject threshold = null;
            HObject connection = null;
            HObject selected = null;
            HObject union = null;
            HObject intersection = null;
            HObject shape = null;
            try
            {
                HOperatorSet.GetDomain(image, out domain);
                HOperatorSet.GrayHisto(image, domain, out HTuple absoluteHisto, out HTuple _);
                HTuple maxValue = absoluteHisto.TupleMax();
                HTuple indices = absoluteHisto.TupleFind(maxValue);
                HTuple thresholdValue = indices.TupleMean().TupleRound().TupleMax2(50).TupleMax2(130);
                if (MinMaxGray(region, image, 10).Max >= thresholdValue.D) return empty;

                HOperatorSet.SmallestCircle(region, out HTuple row, out HTuple col, out HTuple radius);
                HOperatorSet.GenCircle(out circleIn, row, col, radius * 1.3);
                HOperatorSet.GenCircle(out circleOut, row, col, radius * 3);
                HOperatorSet.Difference(circleOut, circleIn, out ring);

                HTuple referMean = (thresholdValue * 1.05).TupleRound();
                HOperatorSet.MedianImage(image, out median, "circle", 3, "mirrored");
                HOperatorSet.GetImageSize(image, out HTuple width, out HTuple height);
                HOperatorSet.GenImageGrayRamp(out refer, 0, 0, referMean, 0, 0, width, height);
                HOperatorSet.SubImage(median, refer, out sub, 1.0, 0);
                HOperatorSet.ScaleImageMax(sub, out scaleMax);
                HOperatorSet.Threshold(scaleMax, out threshold, 40, 255);
                HOperatorSet.Connection(threshold, out connection);
                HOperatorSet.SelectShape(connection, out selected, "area", "and", 50, 9999999);
                HOperatorSet.Union1(selected, out union);
                HOperatorSet.Intersection(union, ring, out intersection);
                HOperatorSet.ShapeTrans(intersection, out shape, "convex");

                empty.Dispose();
                empty = shape.CopyObj(1, -1);
                return empty;
            }
            catch
            {
                return empty;
            }
            finally
            {
                domain?.Dispose();
                circleIn?.Dispose();
                circleOut?.Dispose();
                ring?.Dispose();
                median?.Dispose();
                refer?.Dispose();
                sub?.Dispose();
                scaleMax?.Dispose();
                threshold?.Dispose();
                connection?.Dispose();
                selected?.Dispose();
                union?.Dispose();
                intersection?.Dispose();
                shape?.Dispose();
            }
        }

        private static double ThresholdMean(HObject image, HObject region, double min, double max, double emptyValue)
        {
            if (IsRegionEmpty(region)) return emptyValue;

            HObject reduced = null;
            HObject threshold = null;
            try
            {
                HOperatorSet.ReduceDomain(image, region, out reduced);
                HOperatorSet.Threshold(reduced, out threshold, min, max);
                return IsRegionEmpty(threshold) ? emptyValue : GrayFeature(threshold, image, "mean", emptyValue);
            }
            catch
            {
                return emptyValue;
            }
            finally
            {
                reduced?.Dispose();
                threshold?.Dispose();
            }
        }

        private static double GrayFeature(HObject region, HObject image, string featureName, double emptyValue)
        {
            if (IsRegionEmpty(region) || !HasObject(image)) return emptyValue;
            try
            {
                HOperatorSet.GrayFeatures(region, image, featureName, out HTuple value);
                return First(value, emptyValue);
            }
            catch
            {
                return emptyValue;
            }
        }

        private static double RegionFeature(HObject region, string featureName, double emptyValue)
        {
            if (IsRegionEmpty(region)) return emptyValue;
            try
            {
                HOperatorSet.RegionFeatures(region, featureName, out HTuple value);
                return First(value, emptyValue);
            }
            catch
            {
                return emptyValue;
            }
        }

        private static double RegionArea(HObject region)
        {
            if (IsRegionEmpty(region)) return 0;
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

        private static GrayRange MinMaxGray(HObject region, HObject image, int percent = 1)
        {
            try
            {
                HOperatorSet.MinMaxGray(region, image, percent, out HTuple min, out HTuple max, out HTuple range);
                return new GrayRange(First(min, 0), First(max, 0), First(range, 0));
            }
            catch
            {
                return new GrayRange(0, 0, 0);
            }
        }

        private static double Circularity(HObject region)
        {
            try
            {
                HOperatorSet.Circularity(region, out HTuple value);
                return First(value, -1);
            }
            catch
            {
                return -1;
            }
        }

        private static HObject DifferenceIfNeeded(HObject source, HObject invalidRegion)
        {
            if (IsRegionEmpty(invalidRegion)) return source.CopyObj(1, -1);
            HOperatorSet.Difference(source, invalidRegion, out HObject diff);
            return diff;
        }

        private static bool IsRegionEmpty(HObject region)
        {
            if (!HasObject(region)) return true;
            return RegionAreaUnsafe(region) <= 0;
        }

        private static double RegionAreaUnsafe(HObject region)
        {
            try
            {
                HOperatorSet.AreaCenter(region, out HTuple area, out HTuple _, out HTuple __);
                double a = area.TupleSum().D;
                double b = area.TupleLength();
                return area.TupleLength() == 0 ? 0 : area.TupleSum().D==0?1: area.TupleSum().D;
            }
            catch
            {
                return 0;
            }
        }

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

        private static double PixelArea(DefectFeatureContext c)
        {
            double h = c.PixelToMmH <= 0 ? 1 : c.PixelToMmH;
            double v = c.PixelToMmV <= 0 ? 1 : c.PixelToMmV;
            return h * v;
        }

        private static double First(HTuple tuple, double defaultValue)
        {
            if (tuple == null || tuple.TupleLength() == 0) return defaultValue;
            return tuple.D;
        }

        private static double Clamp(double value, double min, double max)
        {
            return Math.Max(min, Math.Min(max, value));
        }

        private struct GrayRange
        {
            public GrayRange(double min, double max, double range)
            {
                Min = min;
                Max = max;
                Range = range;
            }

            public double Min { get; }
            public double Max { get; }
            public double Range { get; }
        }
    }
}

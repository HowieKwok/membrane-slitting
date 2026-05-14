using Plugin.BlobAnalysis.Models;
using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Plugin.BlobAnalysis.Core
{
    public interface IBlobAnalysisEngine
    {
        BlobAnalysisResult Run(HObject image, BlobAnalysisParams parameters);
    }

    public sealed class BlobAnalysisEngine : IBlobAnalysisEngine
    {
        public BlobAnalysisResult Run(HObject image, BlobAnalysisParams parameters)
        {
            BlobAnalysisResult result = new BlobAnalysisResult();
            Stopwatch watch = Stopwatch.StartNew();

            HObject gray = null;
            HObject domain = null;
            HObject reduced = null;
            HObject thresholdRegion = null;
            HObject maskedRegion = null;
            HObject maskUnionRegion = null;
            HObject morphRegion = null;
            HObject connected = null;
            HObject selected = null;
            HObject sorted = null;

            try
            {
                if (image == null || !image.IsInitialized())
                {
                    result.Success = false;
                    result.Message = "Input image is empty.";
                    return result;
                }

                parameters = parameters ?? new BlobAnalysisParams();
                NormalizeThreshold(parameters);
                BlobAnalysisFollowTransformer transformer = new BlobAnalysisFollowTransformer(parameters);

                HOperatorSet.CountChannels(image, out HTuple channels);
                if (channels.I > 1) HOperatorSet.Rgb1ToGray(image, out gray);
                else gray = image.CopyObj(1, -1);

                if (parameters.DetectMode == DetectRangeMode.LocalRoi && parameters.EnableLocalRoi)
                {
                    RoiParam roi = transformer.TransformRect(parameters.LocalRoi);
                    HOperatorSet.GenRectangle1(out domain, roi.Row1, roi.Col1, roi.Row2, roi.Col2);
                    result.LocalRoiRegion = domain.CopyObj(1, -1);
                    HOperatorSet.ReduceDomain(gray, domain, out reduced);
                }
                else
                {
                    reduced = gray.CopyObj(1, -1);
                }

                HOperatorSet.Threshold(reduced, out thresholdRegion, parameters.ThresholdMin, parameters.ThresholdMax);
                result.ThresholdRegions = thresholdRegion.CopyObj(1, -1);

                ApplyMaskRois(thresholdRegion, parameters, transformer, out maskedRegion, out maskUnionRegion);
                result.MaskRoiRegion = maskUnionRegion;
                maskUnionRegion = null;

                ApplyMorphology(maskedRegion, parameters, out morphRegion);
                HOperatorSet.Connection(morphRegion, out connected);
                selected = connected.CopyObj(1, -1);

                foreach (FilterRuleParam filter in parameters.Filters)
                {
                    if (filter == null || !filter.Enabled) continue;
                    HObject temp = null;
                    HOperatorSet.SelectShape(selected, out temp, filter.Feature, filter.Operation, filter.Min, filter.Max);
                    selected.Dispose();
                    selected = temp;
                }

                ApplyRegionSort(selected, parameters, out sorted);
                result.ResultRegions = sorted.CopyObj(1, -1);
                FillFeatures(sorted, gray, result);
                result.Success = result.Regions.Count > 0;
                result.Message = result.Success ? "OK" : "No region passed filters.";
                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                return result;
            }
            finally
            {
                watch.Stop();
                result.ElapsedMs = watch.ElapsedMilliseconds;
                gray?.Dispose();
                domain?.Dispose();
                reduced?.Dispose();
                thresholdRegion?.Dispose();
                maskedRegion?.Dispose();
                maskUnionRegion?.Dispose();
                morphRegion?.Dispose();
                connected?.Dispose();
                selected?.Dispose();
                sorted?.Dispose();
            }
        }

        private static void NormalizeThreshold(BlobAnalysisParams p)
        {
            if (p.ThresholdMax >= p.ThresholdMin) return;
            double t = p.ThresholdMin;
            p.ThresholdMin = p.ThresholdMax;
            p.ThresholdMax = t;
        }

        private static void ApplyMaskRois(HObject source, BlobAnalysisParams p, BlobAnalysisFollowTransformer transformer, out HObject output, out HObject unionMaskOutput)
        {
            output = source.CopyObj(1, -1);
            unionMaskOutput = null;
            if (!p.EnableMaskRoi || p.MaskRois == null) return;

            HObject unionMask = null;
            foreach (RoiParam roiParam in p.MaskRois)
            {
                if (roiParam == null || !roiParam.Enabled) continue;
                HObject mask = null;
                HObject concat = null;
                try
                {
                    RoiParam roi = transformer.TransformRoi(roiParam);
                    CreateRoiRegion(roi, out mask);
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

        private static void CreateRoiRegion(RoiParam roi, out HObject region)
        {
            switch (roi.Shape)
            {
                case RoiShapeType.Rectangle2:
                    HOperatorSet.GenRectangle2(out region, roi.CenterRow, roi.CenterCol, roi.Angle, roi.Length1, roi.Length2);
                    break;
                case RoiShapeType.Circle:
                    HOperatorSet.GenCircle(out region, roi.CenterRow, roi.CenterCol, roi.Radius);
                    break;
                default:
                    HOperatorSet.GenRectangle1(out region, roi.Row1, roi.Col1, roi.Row2, roi.Col2);
                    break;
            }
        }

        private static void ApplyMorphology(HObject source, BlobAnalysisParams p, out HObject output)
        {
            HObject temp = null;
            double radius = Math.Max(0.1, p.MorphRadius);
            try
            {
                switch (p.MorphMode)
                {
                    case MorphMode.Open:
                        HOperatorSet.OpeningCircle(source, out output, radius);
                        break;
                    case MorphMode.Close:
                        HOperatorSet.ClosingCircle(source, out output, radius);
                        break;
                    case MorphMode.FillUp:
                        HOperatorSet.FillUp(source, out output);
                        break;
                    case MorphMode.OpenClose:
                        HOperatorSet.OpeningCircle(source, out temp, radius);
                        HOperatorSet.ClosingCircle(temp, out output, radius);
                        break;
                    case MorphMode.CloseOpen:
                        HOperatorSet.ClosingCircle(source, out temp, radius);
                        HOperatorSet.OpeningCircle(temp, out output, radius);
                        break;
                    default:
                        output = source.CopyObj(1, -1);
                        break;
                }
            }
            finally
            {
                temp?.Dispose();
            }
        }

        private static void ApplyRegionSort(HObject source, BlobAnalysisParams p, out HObject output)
        {
            HOperatorSet.CountObj(source, out HTuple count);
            if (count.I <= 1)
            {
                output = source.CopyObj(1, -1);
                return;
            }

            List<SortItem> items = new List<SortItem>();
            for (int i = 1; i <= count.I; i++)
            {
                HObject one = null;
                try
                {
                    HOperatorSet.SelectObj(source, out one, i);
                    HOperatorSet.AreaCenter(one, out HTuple area, out HTuple row, out HTuple col);
                    double key = area.D;
                    if (p.SortFeature == SortFeature.Row) key = row.D;
                    if (p.SortFeature == SortFeature.Column) key = col.D;
                    items.Add(new SortItem { Index = i, Key = key });
                }
                finally
                {
                    one?.Dispose();
                }
            }

            bool desc = p.SortDirection == SortDirection.Desc;
            items.Sort((a, b) => desc ? b.Key.CompareTo(a.Key) : a.Key.CompareTo(b.Key));
            output = null;
            foreach (SortItem item in items)
            {
                HObject one = null;
                HObject concat = null;
                try
                {
                    HOperatorSet.SelectObj(source, out one, item.Index);
                    if (output == null) output = one.CopyObj(1, -1);
                    else
                    {
                        HOperatorSet.ConcatObj(output, one, out concat);
                        output.Dispose();
                        output = concat;
                        concat = null;
                    }
                }
                finally
                {
                    one?.Dispose();
                    concat?.Dispose();
                }
            }
            if (output == null) output = source.CopyObj(1, -1);
        }

        private static void FillFeatures(HObject regions, HObject gray, BlobAnalysisResult result)
        {
            HOperatorSet.CountObj(regions, out HTuple count);
            for (int i = 1; i <= count.I; i++)
            {
                HObject one = null;
                try
                {
                    HOperatorSet.SelectObj(regions, out one, i);
                    HOperatorSet.AreaCenter(one, out HTuple area, out HTuple row, out HTuple col);
                    HOperatorSet.Intensity(one, gray, out HTuple mean, out HTuple deviation);
                    HOperatorSet.Circularity(one, out HTuple circularity);
                    HOperatorSet.Rectangularity(one, out HTuple rectangularity);
                    HOperatorSet.SmallestRectangle2(one, out HTuple rectRow, out HTuple rectCol, out HTuple phi, out HTuple length1, out HTuple length2);
                    HOperatorSet.EllipticAxis(one, out HTuple ra, out HTuple rb, out HTuple phiAxis);

                    double minGray = 0;
                    double maxGray = 0;
                    TryGetGrayMinMax(one, gray, out minGray, out maxGray);

                    double shortLen = Math.Min(length1.D, length2.D);
                    double longLen = Math.Max(length1.D, length2.D);
                    double ratio = shortLen <= 0 ? 0 : longLen / shortLen;
                    double phiValue = phiAxis.Length > 0 ? phiAxis.D : phi.D;

                    result.Regions.Add(new BlobRegionFeature
                    {
                        Index = i,
                        Area = area.D,
                        Row = row.D,
                        Col = col.D,
                        Rect2Row = rectRow.D,
                        Rect2Col = rectCol.D,
                        Ra = ra.D,
                        Rb = rb.D,
                        Ratio = ratio,
                        MeanGray = mean.D,
                        DeviationGray = deviation.D,
                        MinGray = minGray,
                        MaxGray = maxGray,
                        Circularity = circularity.D,
                        Rectangularity = rectangularity.D,
                        Phi = phiValue,
                        AngleDeg = phiValue * 180.0 / Math.PI,
                        Length1 = length1.D,
                        Length2 = length2.D
                    });
                }
                finally
                {
                    one?.Dispose();
                }
            }
        }

        private static void TryGetGrayMinMax(HObject region, HObject gray, out double minGray, out double maxGray)
        {
            minGray = 0;
            maxGray = 0;
            try
            {
                HOperatorSet.GrayFeatures(region, gray, "min", out HTuple minValue);
                HOperatorSet.GrayFeatures(region, gray, "max", out HTuple maxValue);
                minGray = minValue.D;
                maxGray = maxValue.D;
            }
            catch
            {
                minGray = 0;
                maxGray = 0;
            }
        }

        private sealed class SortItem
        {
            public int Index;
            public double Key;
        }
    }
}
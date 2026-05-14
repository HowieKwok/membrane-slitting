using Frame_Core.pluginParams;
using Plugin.BlobAnalysis.Models;
using System.Collections.Generic;

namespace Plugin.BlobAnalysis.Core
{

    public static class BlobParamsMapper
    {
        public static BlobAnalysisParams ToRuntime(ParamsBlobAnalysis source)
        {
            BlobAnalysisParams target = new BlobAnalysisParams();
            if (source == null) return target;

            target.InputImage = source.ho_Image;
            target.DetectMode = source.m_nDetectMode == 0 ? DetectRangeMode.Global : DetectRangeMode.LocalRoi;
            target.EnableLocalRoi = source.m_bUseLocalRoi;
            target.EnableMaskRoi = source.m_bUseMaskRoi;
            target.ShowViewRoi = source.m_bShowViewRoi;
            target.EnablePositionFollow = source.m_bUsePositionFollow;
            target.LocalRoi = new RoiParam
            {
                Index = 1,
                Enabled = source.m_bUseLocalRoi,
                Name = "灞€閮≧OI",
                Shape = RoiShapeType.Rectangle1,
                Row1 = source.m_fLocalRoiRow1,
                Col1 = source.m_fLocalRoiCol1,
                Row2 = source.m_fLocalRoiRow2,
                Col2 = source.m_fLocalRoiCol2
            };

            target.ThresholdMin = source.m_fThresholdMin;
            target.ThresholdMax = source.m_fThresholdMax;
            target.MorphMode = (MorphMode)source.m_nMorphMode;
            target.MorphRadius = source.m_fMorphRadius;
            target.SortFeature = (SortFeature)source.m_nSortFeature;
            target.SortDirection = (SortDirection)source.m_nSortDirection;
            target.BaseRow = source.m_fBaseRow;
            target.BaseCol = source.m_fBaseCol;
            target.BaseAngle = source.m_fBaseAngle;
            target.CurrentRow = source.m_fCurrentRow;
            target.CurrentCol = source.m_fCurrentCol;
            target.CurrentAngle = source.m_fCurrentAngle;

            target.MaskRois = new List<RoiParam>();
            if (source.m_lMaskRoiParams != null)
            {
                foreach (BlobMaskRoiParam item in source.m_lMaskRoiParams)
                {
                    if (item == null) continue;
                    target.MaskRois.Add(ToRuntime(item));
                }
            }

            target.Filters = new List<FilterRuleParam>();
            if (source.m_lFilterRules != null)
            {
                foreach (BlobFilterRuleParam item in source.m_lFilterRules)
                {
                    if (item == null) continue;
                    target.Filters.Add(new FilterRuleParam
                    {
                        Enabled = item.m_bIsEnable,
                        Feature = item.m_strFeature,
                        Min = item.m_fMin,
                        Max = item.m_fMax,
                        Operation = item.m_strOperation
                    });
                }
            }

            return target;
        }

        public static void ApplyToFrame(BlobAnalysisParams source, ParamsBlobAnalysis target)
        {
            if (source == null || target == null) return;

            target.m_nDetectMode = source.DetectMode == DetectRangeMode.Global ? 0 : 1;
            target.m_bUseLocalRoi = source.EnableLocalRoi;
            target.m_bUseMaskRoi = source.EnableMaskRoi;
            target.m_bShowViewRoi = source.ShowViewRoi;
            target.m_bUsePositionFollow = source.EnablePositionFollow;
            target.m_fLocalRoiRow1 = source.LocalRoi.Row1;
            target.m_fLocalRoiCol1 = source.LocalRoi.Col1;
            target.m_fLocalRoiRow2 = source.LocalRoi.Row2;
            target.m_fLocalRoiCol2 = source.LocalRoi.Col2;
            target.m_fThresholdMin = source.ThresholdMin;
            target.m_fThresholdMax = source.ThresholdMax;
            target.m_nMorphMode = (int)source.MorphMode;
            target.m_fMorphRadius = source.MorphRadius;
            target.m_nSortFeature = (int)source.SortFeature;
            target.m_nSortDirection = (int)source.SortDirection;
            target.m_fBaseRow = source.BaseRow;
            target.m_fBaseCol = source.BaseCol;
            target.m_fBaseAngle = source.BaseAngle;
            target.m_fCurrentRow = source.CurrentRow;
            target.m_fCurrentCol = source.CurrentCol;
            target.m_fCurrentAngle = source.CurrentAngle;

            target.m_lMaskRoiParams.Clear();
            foreach (RoiParam item in source.MaskRois)
            {
                target.m_lMaskRoiParams.Add(ToFrame(item));
            }

            target.m_lFilterRules.Clear();
            foreach (FilterRuleParam item in source.Filters)
            {
                target.m_lFilterRules.Add(new BlobFilterRuleParam
                {
                    m_bIsEnable = item.Enabled,
                    m_strFeature = item.Feature,
                    m_fMin = item.Min,
                    m_fMax = item.Max,
                    m_strOperation = item.Operation
                });
            }
        }

        public static void ApplyResult(BlobAnalysisResult result, ParamsBlobAnalysis target)
        {
            if (target == null) return;

            ReleaseOutputHObjects(target);
            target.m_lOutRegionResults = new List<BlobRegionResultParam>();
            target.m_bRunSuccess = result != null && result.Success;
            target.m_bIsFind = result != null && result.Regions.Count > 0;
            target.m_nRegionCount = result == null ? 0 : result.Regions.Count;
            target.m_fElapsedMs = result == null ? 0 : result.ElapsedMs;
            target.m_strRunMessage = result == null ? "Not run" : result.Message;

            if (result == null) return;

            if (result.ThresholdRegions != null && result.ThresholdRegions.IsInitialized())
            {
                target.m_ho_OutRegion = result.ThresholdRegions.CopyObj(1, -1);
            }

            if (result.ResultRegions != null && result.ResultRegions.IsInitialized())
            {
                target.m_ho_OutSelectedRegion = result.ResultRegions.CopyObj(1, -1);
            }

            if (result.LocalRoiRegion != null && result.LocalRoiRegion.IsInitialized())
            {
                target.m_ho_OutLocalRoiRegion = result.LocalRoiRegion.CopyObj(1, -1);
            }

            if (result.MaskRoiRegion != null && result.MaskRoiRegion.IsInitialized())
            {
                target.m_ho_OutMaskRoiRegion = result.MaskRoiRegion.CopyObj(1, -1);
            }

            foreach (BlobRegionFeature item in result.Regions)
            {
                target.m_lOutRegionResults.Add(new BlobRegionResultParam
                {
                    m_nIndex = item.Index,
                    m_fArea = item.Area,
                    m_fCenterRow = item.Row,
                    m_fCenterCol = item.Col,
                    m_fRect2Row = item.Rect2Row,
                    m_fRect2Col = item.Rect2Col,
                    m_fRa = item.Ra,
                    m_fRb = item.Rb,
                    m_fLength1 = item.Length1,
                    m_fLength2 = item.Length2,
                    m_fAspectRatio = item.Ratio,
                    m_fPhi = item.Phi,
                    m_fAngleDeg = item.AngleDeg,
                    m_fCircularity = item.Circularity,
                    m_fRectangularity = item.Rectangularity,
                    m_fMeanGray = item.MeanGray,
                    m_fDeviationGray = item.DeviationGray,
                    m_fMinGray = item.MinGray,
                    m_fMaxGray = item.MaxGray
                });
            }
        }

        /// <summary>
        /// 释放 Blob 插件写入 ParamsBlobAnalysis 的输出 HObject。
        /// 注意：不释放 target.ho_Image，输入图像由流程或上游模块管理。
        /// </summary>
        public static void ReleaseOutputHObjects(ParamsBlobAnalysis target)
        {
            if (target == null) return;

            target.m_ho_OutRegion?.Dispose();
            target.m_ho_OutRegion = null;

            target.m_ho_OutSelectedRegion?.Dispose();
            target.m_ho_OutSelectedRegion = null;

            target.m_ho_OutLocalRoiRegion?.Dispose();
            target.m_ho_OutLocalRoiRegion = null;

            target.m_ho_OutMaskRoiRegion?.Dispose();
            target.m_ho_OutMaskRoiRegion = null;
        }
        private static RoiParam ToRuntime(BlobMaskRoiParam item)
        {
            return new RoiParam
            {
                Enabled = item.m_bIsEnable,
                Index = item.m_nDisplayNo,
                Name = item.m_strRoiName,
                Shape = (RoiShapeType)item.m_nRoiShape,
                Row1 = item.m_fRow1,
                Col1 = item.m_fCol1,
                Row2 = item.m_fRow2,
                Col2 = item.m_fCol2,
                CenterRow = item.m_fCenterRow,
                CenterCol = item.m_fCenterCol,
                Angle = item.m_fAngle,
                Length1 = item.m_fLength1,
                Length2 = item.m_fLength2,
                Radius = item.m_fRadius
            };
        }

        private static BlobMaskRoiParam ToFrame(RoiParam item)
        {
            return new BlobMaskRoiParam
            {
                m_bIsEnable = item.Enabled,
                m_nDisplayNo = item.Index,
                m_strRoiName = item.Name,
                m_nRoiShape = (int)item.Shape,
                m_fRow1 = item.Row1,
                m_fCol1 = item.Col1,
                m_fRow2 = item.Row2,
                m_fCol2 = item.Col2,
                m_fCenterRow = item.CenterRow,
                m_fCenterCol = item.CenterCol,
                m_fAngle = item.Angle,
                m_fLength1 = item.Length1,
                m_fLength2 = item.Length2,
                m_fRadius = item.Radius
            };
        }
    }
}

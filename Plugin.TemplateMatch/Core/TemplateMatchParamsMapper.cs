using Frame_Core.pluginParams;
using HalconDotNet;
using System.Collections.Generic;

namespace Plugin.TemplateMatch.Core
{
    public static class TemplateMatchParamsMapper
    {
        public static TemplateMatchRuntimeParams ToRuntime(ParamsTemplateMatch source)
        {
            return new TemplateMatchRuntimeParams
            {
                UseLocalRoi = source.m_bUseLocalRoi,
                RoiFollowMatch = source.m_bRoiFollowMatch,
                UsePositionFollow = source.m_bUsePositionFollow,
                FollowBaseRow = source.m_fFollowBaseRow,
                FollowBaseCol = source.m_fFollowBaseCol,
                FollowBaseAngleDeg = source.m_fFollowBaseAngleDeg,
                FollowCurrentRow = source.m_fFollowCurrentRow,
                FollowCurrentCol = source.m_fFollowCurrentCol,
                FollowCurrentAngleDeg = source.m_fFollowCurrentAngleDeg,
                LocalRoiRow1 = source.m_fLocalRoiRow1,
                LocalRoiCol1 = source.m_fLocalRoiCol1,
                LocalRoiRow2 = source.m_fLocalRoiRow2,
                LocalRoiCol2 = source.m_fLocalRoiCol2,
                UseMaskRoi = source.m_bUseMaskRoi,
                MaskRois = source.m_lMaskRoiParams,
                FindOptions = source.FindOptions,
                Templates = source.Templates
            };
        }

        public static void ApplyResult(TemplateMatchResult result, ParamsTemplateMatch target)
        {
            ReleaseRuntimeOutputs(target);
            target.Results = new List<TemplateMatchResultParam>(result.Results);
            target.m_bRunSuccess = result.Success;
            target.m_strRunMessage = result.Message;
            target.m_fElapsedMs = result.ElapsedMs;

            if (result.OutputContours != null && result.OutputContours.IsInitialized())
            {
                target.ho_OutContours = result.OutputContours.CopyObj(1, -1);
            }

            if (result.OutputMatchRoiRegion != null && result.OutputMatchRoiRegion.IsInitialized())
            {
                target.ho_OutMatchRoiRegion = result.OutputMatchRoiRegion.CopyObj(1, -1);
            }

            if (result.OutputMaskRoiRegion != null && result.OutputMaskRoiRegion.IsInitialized())
            {
                target.ho_OutMaskRoiRegion = result.OutputMaskRoiRegion.CopyObj(1, -1);
            }

            target.hv_OutBaseToCurrentMatrix = result.OutputMatrix;
        }

        public static void ReleaseRuntimeOutputs(ParamsTemplateMatch target, bool clearMessage = true)
        {
            if (target == null) return;
            HalconObjectGuard.DisposeObject(ref target.ho_OutContours);
            HalconObjectGuard.DisposeObject(ref target.ho_OutMatchRoiRegion);
            HalconObjectGuard.DisposeObject(ref target.ho_OutMaskRoiRegion);
            target.hv_OutBaseToCurrentMatrix = null;
            if (target.Results != null) target.Results.Clear();
            target.m_bRunSuccess = false;
            if (clearMessage) target.m_strRunMessage = string.Empty;
        }

        public static void ApplyFailure(ParamsTemplateMatch target, string message)
        {
            ReleaseRuntimeOutputs(target, false);
            target.m_bRunSuccess = false;
            target.m_strRunMessage = string.IsNullOrWhiteSpace(message) ? "运行失败" : message;
            if (target.Results == null) target.Results = new List<TemplateMatchResultParam>();
            target.Results.Clear();
            target.Results.Add(new TemplateMatchResultParam
            {
                TemplateName = "-",
                StatusText = "NG",
                ErrorMessage = target.m_strRunMessage,
                IsSuccess = false
            });
        }
    }
}

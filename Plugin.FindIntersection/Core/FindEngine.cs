using System;
using Frame_Core.pluginParams;
using HalconAlgorithm;
using HalconDotNet;
using LogTool;

namespace Plugin.FindIntersection.Core
{
    internal sealed class FindEngine
    {
        private readonly FittingLine fittingLine = new FittingLine();

        public bool Run(ParamsFindIntersection p)
        {
            if (p == null) return false;

            ResetOutput(p);

            if (p.ho_Image == null || !p.ho_Image.IsInitialized() || p.ho_Image.CountObj() == 0)
            {
                p.m_strRunMessage = "输入图像为空";
                Log4netRecord.ErrorLog.Debug("两线交点工具：输入图像为空");
                return false;
            }

            //if (p.m_bUsePositionFollow && !p.m_bCurrentPoseValid)
            //{
            //    p.m_strRunMessage = "位置跟随已启用，但当前输入坐标无效";
            //    Log4netRecord.ErrorLog.Debug("两线交点工具：位置跟随已启用，但当前输入坐标无效");
            //    return false;
            //}

            RoiFollower follower = new RoiFollower(p);
            RoiRect2 line1 = follower.Transform(
                p.hv_Line1_RoiMidRow, p.hv_Line1_RoiMidColumn,
                p.hv_Line1_RoiAngle, p.hv_Line1_RoiHeight,
                p.hv_Line1_RoiWidth, p.hv_Line1_RoiPointsNum);
            RoiRect2 line2 = follower.Transform(
                p.hv_Line2_RoiMidRow, p.hv_Line2_RoiMidColumn,
                p.hv_Line2_RoiAngle, p.hv_Line2_RoiHeight,
                p.hv_Line2_RoiWidth, p.hv_Line2_RoiPointsNum);

            p.bl_Line1_FitSuccess = FitSingleLine(
                p, line1,
                p.hv_Line1_MeasureSigma, p.hv_Line1_MeasureThreshold,
                p.hv_Line1_MeasureNumInstances, p.hv_Line1_MeasureDistThreshold,
                p.hv_Line1_MeasureTransition, p.hv_Line1_MeasureSelect,
                p.hv_Line1_MeasureMinScore, p.hv_Line1_MaxNumIterations,
                p.hv_Line1_MeasureInterpolation, p.hv_Line1_MeasureIORegions,
                out p.hv_Line1_CrossRow, out p.hv_Line1_CrossColumn,
                out p.hv_Line1_RowBegin, out p.hv_Line1_ColumnBegin,
                out p.hv_Line1_RowEnd, out p.hv_Line1_ColumnEnd);

            p.bl_Line2_FitSuccess = FitSingleLine(
                p, line2,
                p.hv_Line2_MeasureSigma, p.hv_Line2_MeasureThreshold,
                p.hv_Line2_MeasureNumInstances, p.hv_Line2_MeasureDistThreshold,
                p.hv_Line2_MeasureTransition, p.hv_Line2_MeasureSelect,
                p.hv_Line2_MeasureMinScore, p.hv_Line2_MaxNumIterations,
                p.hv_Line2_MeasureInterpolation, p.hv_Line2_MeasureIORegions,
                out p.hv_Line2_CrossRow, out p.hv_Line2_CrossColumn,
                out p.hv_Line2_RowBegin, out p.hv_Line2_ColumnBegin,
                out p.hv_Line2_RowEnd, out p.hv_Line2_ColumnEnd);

            if (!p.bl_Line1_FitSuccess || !p.bl_Line2_FitSuccess)
            {
                p.bl_IntersectionSuccess = false;
                p.m_strRunMessage = "直线拟合失败";
                return false;
            }

            CalculateIntersection(p);
            if (p.bl_IntersectionSuccess) p.m_strRunMessage = "运行成功";
            return p.bl_IntersectionSuccess;
        }

        public void ResetOutput(ParamsFindIntersection p)
        {
            p.m_fFollowOffsetY = 0;
            p.m_fFollowOffsetX = 0;
            p.m_fFollowOffsetR = 0;

            p.hv_Line1_CrossRow = null;
            p.hv_Line1_CrossColumn = null;
            p.hv_Line1_RowBegin = null;
            p.hv_Line1_ColumnBegin = null;
            p.hv_Line1_RowEnd = null;
            p.hv_Line1_ColumnEnd = null;
            p.bl_Line1_FitSuccess = false;

            p.hv_Line2_CrossRow = null;
            p.hv_Line2_CrossColumn = null;
            p.hv_Line2_RowBegin = null;
            p.hv_Line2_ColumnBegin = null;
            p.hv_Line2_RowEnd = null;
            p.hv_Line2_ColumnEnd = null;
            p.bl_Line2_FitSuccess = false;

            p.hv_IntersectionRow = null;
            p.hv_IntersectionColumn = null;
            p.hv_IntersectionAngle = null;
            p.hv_LineAngle = null;
            p.hv_IsParallel = null;
            p.bl_IntersectionSuccess = false;
        }

        private bool FitSingleLine(
            ParamsFindIntersection p,
            RoiRect2 roi,
            double measureSigma,
            int measureThreshold,
            int measureNumInstances,
            double measureDistThreshold,
            string measureTransition,
            string measureSelect,
            double measureMinScore,
            int maxNumIterations,
            string measureInterpolation,
            string measureIORegions,
            out HTuple crossRow,
            out HTuple crossColumn,
            out HTuple lineRowBegin,
            out HTuple lineColumnBegin,
            out HTuple lineRowEnd,
            out HTuple lineColumnEnd)
        {
            crossRow = null;
            crossColumn = null;
            lineRowBegin = null;
            lineColumnBegin = null;
            lineRowEnd = null;
            lineColumnEnd = null;

            try
            {
                if (roi.PointCount < 2)
                {
                    Log4netRecord.ErrorLog.Debug("两线交点工具：采样点数不能小于2");
                    return false;
                }

                if (roi.Height <= 0 || roi.Width <= 0)
                {
                    Log4netRecord.ErrorLog.Debug("两线交点工具：ROI 宽度和高度必须大于0");
                    return false;
                }

                if (measureSigma <= 0)
                {
                    Log4netRecord.ErrorLog.Debug("两线交点工具：平滑系数必须大于0");
                    return false;
                }

                HOperatorSet.TupleRad(roi.AngleDeg, out HTuple roiPhi);
                HTuple measureRow1 = roi.Row - roi.Height * Math.Sin(roiPhi) / 2;
                HTuple measureColumn1 = roi.Col - roi.Height * Math.Cos(roiPhi) / 2;
                HTuple measureRow2 = roi.Row + roi.Height * Math.Sin(roiPhi) / 2;
                HTuple measureColumn2 = roi.Col + roi.Height * Math.Cos(roiPhi) / 2;

                fittingLine.CreateLineModel(
                    measureRow1, measureColumn1,
                    measureRow2, measureColumn2,
                    roi.Width / 2, roi.Height / roi.PointCount / 2,
                    measureSigma, measureThreshold,
                    out HTuple metrologyHandle, out HTuple index);

                if (metrologyHandle == null)
                {
                    Log4netRecord.ErrorLog.Debug("两线交点工具：创建测量模型失效");
                    return false;
                }

                fittingLine.FindLine(
                    p.ho_Image, metrologyHandle, index,
                    measureNumInstances, measureDistThreshold,
                    measureTransition, measureSelect,
                    roi.PointCount, measureMinScore, maxNumIterations,
                    measureInterpolation, measureIORegions,
                    out crossRow, out crossColumn,
                    out lineRowBegin, out lineColumnBegin,
                    out lineRowEnd, out lineColumnEnd);

                return lineRowBegin != null && lineRowBegin.Length > 0 && lineRowBegin >= 0;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug("两线交点工具拟合直线异常：" + ex);
                return false;
            }
        }

        private static void CalculateIntersection(ParamsFindIntersection p)
        {
            try
            {
                HOperatorSet.IntersectionLines(
                    p.hv_Line1_RowBegin, p.hv_Line1_ColumnBegin,
                    p.hv_Line1_RowEnd, p.hv_Line1_ColumnEnd,
                    p.hv_Line2_RowBegin, p.hv_Line2_ColumnBegin,
                    p.hv_Line2_RowEnd, p.hv_Line2_ColumnEnd,
                    out HTuple row, out HTuple column, out HTuple isOverlapping);
                HOperatorSet.AngleLl(
                    p.hv_Line1_RowBegin, p.hv_Line1_ColumnBegin,
                    p.hv_Line1_RowEnd, p.hv_Line1_ColumnEnd,
                    p.hv_Line2_RowBegin, p.hv_Line2_ColumnBegin,
                    p.hv_Line2_RowEnd, p.hv_Line2_ColumnEnd,
                    out HTuple angle);
                HOperatorSet.TupleDeg(angle, out angle);

                p.hv_IntersectionRow = row;
                p.hv_IntersectionColumn = column;
                p.hv_IntersectionAngle = angle;
                p.hv_LineAngle = angle;
                p.hv_IsParallel = isOverlapping;
                p.bl_IntersectionSuccess = isOverlapping != null && isOverlapping.I == 0;

                if (!p.bl_IntersectionSuccess)
                {
                    p.m_strRunMessage = "两条直线平行或重合，无唯一交点";
                    Log4netRecord.ErrorLog.Debug("两线交点工具：两条直线平行或重合，无唯一交点");
                }
            }
            catch (Exception ex)
            {
                p.bl_IntersectionSuccess = false;
                p.m_strRunMessage = ex.Message;
                Log4netRecord.ErrorLog.Debug("两线交点工具计算交点异常:" + ex);
            }
        }
    }
}

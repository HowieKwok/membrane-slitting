using System;
using Frame_Core;
using Frame_Core.pluginParams;
using HalconAlgorithm;
using HalconDotNet;
using LogTool;

namespace Plugin.MeasureLine.Core
{
    internal sealed class FindEngine
    {
        private readonly FittingLine fittingLine = new FittingLine();

        public bool Run(ParamsMeasureLine p)
        {
            if (p == null) return false;

            if (p.ho_Image == null || !p.ho_Image.IsInitialized() || p.ho_Image.CountObj() == 0)
            {
                p.m_strRunMessage = "输入图像为空";
                Log4netRecord.ErrorLog.Debug("找边工具：输入图像为空");
                return false;
            }

            //if (p.m_bUsePositionFollow && !p.m_bCurrentPoseValid)
            //{
            //    p.m_strRunMessage = "找边工具已启用，但当前输入坐标无效";
            //    Log4netRecord.ErrorLog.Debug("找边工具：位置跟随已启用，但当前输入坐标无效");
            //    return false;
            //}

            RoiFollower follower = new RoiFollower(p);
            RoiRect2 line1 = follower.Transform(
                p.hv_RoiMidRow, p.hv_RoiMidColumn,
                p.hv_RoiAngle, p.hv_RoiHeight,
                p.hv_RoiWidth, p.hv_RoiPointsNum);
        

             p.m_bRunSuccess = FitSingleLine(
                p, line1,
                p.hv_MeasureSigma, p.hv_MeasureThreshold,
                p.hv_MeasureNumInstances, p.hv_MeasureDistThreshold,
                p.hv_MeasureTransition, p.hv_MeasureSelect,
                p.hv_MeasureMinScore, p.hv_MaxNumIterations,
                p.hv_MeasureInterpolation, p.hv_MeasureIORegions,
                out p.hv_CrossRow, out p.hv_CrossColumn,
                out p.hv_LineRowBegin, out p.hv_LineColumnBegin,
                out p.hv_LineRowEnd, out p.hv_LineColumnEnd);

          

            if (!p.m_bRunSuccess )
            {
            
                p.m_strRunMessage = "直线拟合失败";
                return false;
            }

            if (p.m_bRunSuccess) p.m_strRunMessage = "运行成功";
            return p.m_bRunSuccess;
        }



        private bool FitSingleLine(
            ParamsMeasureLine p,
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
                    Log4netRecord.ErrorLog.Debug("找边工具：采样点数不能小于2");
                    return false;
                }

                if (roi.Height <= 0 || roi.Width <= 0)
                {
                    Log4netRecord.ErrorLog.Debug("找边工具：ROI 宽度和高度必须大于0");
                    return false;
                }

                if (measureSigma <= 0)
                {
                    Log4netRecord.ErrorLog.Debug("找边工具：平滑系数必须大于0");
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
                    Log4netRecord.ErrorLog.Debug("找边工具：创建测量模型失效");
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
                Log4netRecord.ErrorLog.Debug("找边工具拟合直线异常：" + ex);
                return false;
            }
        }

 
    }
}


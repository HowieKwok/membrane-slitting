using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frame_Core;
using System.ComponentModel.Composition;
using System.ComponentModel;
using HalconDotNet;
using HalconAlgorithm;
using LogTool;
using Plugin.MeasureLine.Core;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Plugin.MeasureLine
{
    [Category("尺寸测量")]
    [DisplayName("直线抓边工具")]
    //[ExportMetadata("measure1", "IPlugin")]
    [Export("MeasLineObj", typeof(IPlugin))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Serializable]
    public class MeasLineObj : IPlugin
    {
        public event EventHandler<PluginExecuteCompletedEventArgs> ExecuteCompleted;
        public PluginParamsBase PluginParams { get; set; } = new ParamsMeasureLine();
        FittingLine fittingLine = new FittingLine();
        ParamsMeasureLine measLineParams;

        public ModuleInfo moduleInfo { get; set; } = new ModuleInfo();
        public bool isExistModel = false;

        public bool ExecuteObj()
        {
 
            HOperatorSet.CountSeconds(out HTuple start);
            measLineParams = PluginParams as ParamsMeasureLine; //里氏转换
            if (measLineParams.ImageSourceIndex == 1)
            {
                setInPutParams(measLineParams.ImageSourceModule, measLineParams.ImageSourceName);
            }

            HTuple hv_MeasureRow1, hv_MeasureColumn1, hv_MeasureRow2, hv_MeasureColumn2, hv_RoiPhi;
            HTuple hv_RoiMidRow = measLineParams.hv_RoiMidRow;
            HTuple hv_RoiMidColumn = measLineParams.hv_RoiMidColumn;
            double roiAngleDeg = measLineParams.hv_RoiAngle;
            double roiHeight = measLineParams.hv_RoiHeight;
            double roiWidth = measLineParams.hv_RoiWidth;
            int roiPointsNum = measLineParams.hv_RoiPointsNum;
            HObject ResultLine;
            HOperatorSet.GenEmptyObj(out ResultLine);
            HObject ho_Rectangle2, ho_ImagePart, ho_ImagePartProcessed;
            HOperatorSet.GenEmptyObj(out ho_Rectangle2);
            HOperatorSet.GenEmptyObj(out ho_ImagePart);
            HOperatorSet.GenEmptyObj(out ho_ImagePartProcessed);

            HOperatorSet.GenEmptyObj(out measLineParams.ho_ImageProcessed);
            measLineParams.hv_MetrologyHandle = null;
            measLineParams.hv_Index = null;
            measLineParams.hv_CrossRow = null;
            measLineParams.hv_CrossColumn = null;
            measLineParams.hv_LineRowBegin = null;
            measLineParams.hv_LineColumnBegin = null;
            measLineParams.hv_LineRowEnd = null;
            measLineParams.hv_LineColumnEnd = null;
            measLineParams.m_bRunSuccess = false;
            measLineParams.bl_FitSuccess = false;
            measLineParams.m_strRunMessage = "";
            // HOperatorSet.CountSeconds(out Stime2);

            try
            {
                //ROI运动
                switch (measLineParams.str_RoiMoveIndex)
                {
                    case "StaticROI":
                        break;

                    case "StandardROI":
                        hv_RoiMidRow = measLineParams.hv_RoiStandardRow + measLineParams.hv_RoiStandardOffsetRow;
                        hv_RoiMidColumn = measLineParams.hv_RoiStandardColumn + measLineParams.hv_RoiStandardOffsetColumn;
                        break;

                    case "IncreaseROI":
                        hv_RoiMidRow = measLineParams.hv_RoiMidRow + measLineParams.hv_RoiIncreaseRow * measLineParams.hv_RoiIncreaseNum;
                        hv_RoiMidColumn = measLineParams.hv_RoiMidColumn + measLineParams.hv_RoiIncreaseColumn * measLineParams.hv_RoiIncreaseNum;
                        break;
                }

                if (measLineParams.m_bUsePositionFollow)
                {
                    //if (!measLineParams.m_bCurrentPoseValid)
                    //{
                    //    measLineParams.m_strRunMessage = "位置跟随已启用，但当前坐标无效";
                    //    measLineParams.bl_FitSuccess = false;
                    //    ExecuteCompleted?.Invoke(this, new PluginExecuteCompletedEventArgs(PluginParams, false, measLineParams.m_strRunMessage, 0));
                    //    return false;
                    //}

                    RoiFollower follower = new RoiFollower(measLineParams);
                    RoiRect2 followedRoi = follower.Transform(hv_RoiMidRow, hv_RoiMidColumn, roiAngleDeg, roiHeight, roiWidth, roiPointsNum);
                    hv_RoiMidRow = followedRoi.Row;
                    hv_RoiMidColumn = followedRoi.Col;
                    roiAngleDeg = followedRoi.AngleDeg;
                    roiHeight = followedRoi.Height;
                    roiWidth = followedRoi.Width;
                    roiPointsNum = followedRoi.PointCount;
                }

                HOperatorSet.TupleRad(roiAngleDeg, out hv_RoiPhi);
                HTuple hv_Channels;
                //图像预处理
                if (measLineParams.bl_ImageProcessIndex)
                {
                    HOperatorSet.CountChannels(measLineParams.ho_Image, out hv_Channels);
                    if (hv_Channels > 1)
                    {
                        HOperatorSet.Rgb1ToGray(measLineParams.ho_Image, out measLineParams.ho_ImageProcessed);

                    }
                    else
                    {
                        measLineParams.ho_ImageProcessed = measLineParams.ho_Image;
                    }

                    if (measLineParams.hv_ScaleMean > 0)
                    {
                        try
                        {
                            HOperatorSet.GenRectangle2(out ho_Rectangle2, hv_RoiMidRow, hv_RoiMidColumn, -hv_RoiPhi, roiHeight / 2, roiWidth / 2);
                            if (measLineParams.hv_ScaleMean < 2)
                            {
                                measLineParams.hv_ScaleMean = 2;
                            }
                            HOperatorSet.GrayOpeningRect(measLineParams.ho_ImageProcessed, out measLineParams.ho_ImageProcessed, measLineParams.hv_ScaleMean, measLineParams.hv_ScaleMean);
                        }
                        catch (Exception ee)
                        {
                            measLineParams.ho_ImageProcessed = measLineParams.ho_Image;
                            Log4netRecord.ErrorLog.Debug("抓边模块执行失败！原因：图像预处理-捕获异常" + ee.ToString());
                        }
                    }

                    //if (measLineParams.hv_ScaleFactor != 0 || measLineParams.hv_SharpenIterations != 0)
                    //{
                    // DefectDetection.ImageCrop(measLineParams.ho_ImageProcessed, ho_Rectangle2, out ho_ImagePart);
                    // HOperatorSet.ReduceDomain(measLineParams.ho_ImageProcessed, ho_Rectangle2, out HObject imageReduced);

                    // DefectDetection.AffineObj(ho_ImagePart, measLineParams.hv_RoiHeight / 2, measLineParams.hv_RoiWidth / 2, -hv_RoiPhi, hv_RoiMidRow, hv_RoiMidColumn,0, out measLineParams.ho_ImageProcessed);

                    //if (measLineParams.hv_ScaleFactor != 0 && measLineParams.hv_ScaleMean != 0)
                    //{
                    //    DefectDetection.ScaleEmphasis(ho_ImagePart, measLineParams.hv_ScaleMean, measLineParams.hv_ScaleFactor, out ho_ImagePartProcessed);
                    //}
                    //else
                    //{
                    //    ho_ImagePartProcessed = ho_ImagePart;
                    //}

                    //if (measLineParams.hv_SharpenIterations != 0)
                    //{
                    //    DefectDetection.OutlineSharpen(ho_ImagePartProcessed, out ho_ImagePartProcessed, measLineParams.hv_SharpenIterations, measLineParams.hv_SharpenSigma);
                    //}
                    //DefectDetection.ImageStitching(measLineParams.ho_ImageProcessed, ho_ImagePartProcessed, ho_Rectangle2, out measLineParams.ho_ImageProcessed);
                    // }
                }
                hv_MeasureRow1 = hv_RoiMidRow - roiHeight * Math.Sin(hv_RoiPhi) / 2;
                hv_MeasureColumn1 = hv_RoiMidColumn - roiHeight * Math.Cos(hv_RoiPhi) / 2;
                hv_MeasureRow2 = hv_RoiMidRow + roiHeight * Math.Sin(hv_RoiPhi) / 2;
                hv_MeasureColumn2 = hv_RoiMidColumn + roiHeight * Math.Cos(hv_RoiPhi) / 2;

                //抓边默认输出箭头处ROI边界
                HTuple hv_StartRow = hv_RoiMidRow + roiWidth * Math.Cos(hv_RoiPhi) / 2;
                HTuple hv_StartColumn = hv_RoiMidColumn - roiWidth * Math.Sin(hv_RoiPhi) / 2;
                // Console.WriteLine("基准点：" + hv_StartRow.D.ToString() + "--" + hv_StartColumn.D.ToString());
                //抓边算法
                if (roiPointsNum > 1 & measLineParams.ho_Image.CountObj() != 0)
                {


                    fittingLine.CreateLineModel(hv_MeasureRow1, hv_MeasureColumn1, hv_MeasureRow2, hv_MeasureColumn2, roiWidth / 2,
                             roiHeight / roiPointsNum / 2, measLineParams.hv_MeasureSigma, measLineParams.hv_MeasureThreshold, out measLineParams.hv_MetrologyHandle, out measLineParams.hv_Index);

                    // HOperatorSet.CountSeconds(out Stime3);

                    if (measLineParams.bl_ImageProcessIndex & measLineParams.ho_ImageProcessed.CountObj() != 0)
                    {
                        fittingLine.FindLine(measLineParams.ho_ImageProcessed, measLineParams.hv_MetrologyHandle, measLineParams.hv_Index, measLineParams.hv_MeasureNumInstances, measLineParams.hv_MeasureDistThreshold, measLineParams.hv_MeasureTransition, measLineParams.hv_MeasureSelect,
                                  roiPointsNum, measLineParams.hv_MeasureMinScore, measLineParams.hv_MaxNumIterations, measLineParams.hv_MeasureInterpolation, measLineParams.hv_MeasureIORegions, out measLineParams.hv_CrossRow,
                                 out measLineParams.hv_CrossColumn, out measLineParams.hv_LineRowBegin, out measLineParams.hv_LineColumnBegin, out measLineParams.hv_LineRowEnd, out measLineParams.hv_LineColumnEnd);
                    }
                    else
                    {
                        fittingLine.FindLine(measLineParams.ho_Image, measLineParams.hv_MetrologyHandle, measLineParams.hv_Index, measLineParams.hv_MeasureNumInstances, measLineParams.hv_MeasureDistThreshold, measLineParams.hv_MeasureTransition, measLineParams.hv_MeasureSelect,
                             roiPointsNum, measLineParams.hv_MeasureMinScore, measLineParams.hv_MaxNumIterations, measLineParams.hv_MeasureInterpolation, measLineParams.hv_MeasureIORegions, out measLineParams.hv_CrossRow,
                            out measLineParams.hv_CrossColumn, out measLineParams.hv_LineRowBegin, out measLineParams.hv_LineColumnBegin, out measLineParams.hv_LineRowEnd, out measLineParams.hv_LineColumnEnd);
                    }
                    // HOperatorSet.CountSeconds(out Stime4);

                    if (measLineParams.hv_LineRowBegin >= 0)
                    {
                        if (measLineParams.hv_PeakValleySelect != "None")
                        {
                            double maxmin = 0;
                            int indexPV = 0;
                            int pointsNum = measLineParams.hv_CrossRow.Length;

                            HTuple rectStartRow = hv_RoiMidRow - roiWidth * Math.Cos(hv_RoiPhi) / 2;
                            HTuple rectStartColumn = hv_RoiMidColumn + roiWidth * Math.Sin(hv_RoiPhi) / 2;

                            HTuple rectLineRow1 = rectStartRow - roiHeight * Math.Sin(hv_RoiPhi) / 2;
                            HTuple rectLineCol1 = rectStartColumn - roiHeight * Math.Cos(hv_RoiPhi) / 2;
                            HTuple rectLineRow2 = rectStartRow + roiHeight * Math.Sin(hv_RoiPhi) / 2;
                            HTuple rectLineCol2 = rectStartColumn + roiHeight * Math.Cos(hv_RoiPhi) / 2;


                            //HTuple rectLineRow1 = hv_RoiMidRow - measLineParams.hv_RoiHeight * Math.Sin(hv_RoiPhi) / 2;
                            //HTuple rectLineCol1 = hv_RoiMidColumn - measLineParams.hv_RoiHeight * Math.Cos(hv_RoiPhi) / 2;
                            //HTuple rectLineRow2 = hv_RoiMidRow + measLineParams.hv_RoiHeight * Math.Sin(hv_RoiPhi) / 2;
                            //HTuple rectLineCol2 = hv_RoiMidColumn + measLineParams.hv_RoiHeight * Math.Cos(hv_RoiPhi) / 2;

                            Console.WriteLine("基准线：" + rectLineRow1.D.ToString() + "--" + rectLineCol1.D.ToString() + "--" + rectLineRow2.D.ToString() + "--" + rectLineCol2.D.ToString());

                            List<double> distancelist = new List<double>();
                            List<double> RemainDis = new List<double>();
                            double AverageDis = 0;
                            double AverageDis1 = 0;
                            int dumpNum = (int)(pointsNum / 4) - 1;
                            dumpNum = dumpNum > 0 ? dumpNum : 0;
                            for (int i = 0; i < pointsNum; i++)
                            {
                                HTuple hdistance = new HTuple();
                                HOperatorSet.DistancePl(measLineParams.hv_CrossRow[i], measLineParams.hv_CrossColumn[i], rectLineRow1, rectLineCol1, rectLineRow2, rectLineCol2, out hdistance);
                                if (i == 0)
                                {
                                    maxmin = hdistance.D;
                                }
                                if (measLineParams.hv_PeakValleySelect == "peak")
                                {
                                    if (hdistance.D > maxmin)
                                    {
                                        maxmin = hdistance.D;
                                        indexPV = i;
                                    }
                                }
                                else if (measLineParams.hv_PeakValleySelect == "valley")
                                {
                                    if (hdistance.D < maxmin)
                                    {
                                        maxmin = hdistance.D;
                                        indexPV = i;
                                    }
                                }
                                distancelist.Add(hdistance.D);
                            }
                            measLineParams.hv_PeakpointRow = measLineParams.hv_CrossRow[indexPV];
                            measLineParams.hv_PeakpointColumn = measLineParams.hv_CrossColumn[indexPV];

                            //Console.WriteLine("最大值：" + maxmin.ToString() + "弧度：" + hv_RoiPhi.D.ToString() + "indexPV:" + indexPV.ToString() );
                            //Console.WriteLine("一次：" + measLineParams.hv_LineRowBegin.D.ToString() + "--" + measLineParams.hv_LineColumnBegin.D.ToString()
                            //    + "--" + measLineParams.hv_LineRowEnd.D.ToString() + "--" + measLineParams.hv_LineColumnEnd.D.ToString());

                            measLineParams.hv_LineRowBegin = measLineParams.hv_PeakpointRow - roiHeight * Math.Sin(hv_RoiPhi) * indexPV / pointsNum;
                            measLineParams.hv_LineColumnBegin = measLineParams.hv_PeakpointColumn - roiHeight * Math.Cos(hv_RoiPhi) * indexPV / pointsNum;
                            measLineParams.hv_LineRowEnd = measLineParams.hv_PeakpointRow + roiHeight * Math.Sin(hv_RoiPhi) * (pointsNum - indexPV) / pointsNum;
                            measLineParams.hv_LineColumnEnd = measLineParams.hv_PeakpointColumn + roiHeight * Math.Cos(hv_RoiPhi) * (pointsNum - indexPV) / pointsNum;

                            if (measLineParams.hv_PeakValleySelect == "RejectionAverage")
                            {
                                //剔值平均线
                                distancelist.Sort();
                                RemainDis = distancelist.Skip(dumpNum).Take(dumpNum * 2).ToList();
                                AverageDis = distancelist.Average();
                                AverageDis1 = RemainDis.Average();

                                //Console.WriteLine("均值：" + AverageDis.ToString() + "--" + AverageDis1.ToString() + "-保留个数-" + RemainDis.Count().ToString());

                                HTuple rectDisRow = rectStartRow + AverageDis1 * Math.Cos(hv_RoiPhi);
                                HTuple rectDisColumn = rectStartColumn - AverageDis1 * Math.Sin(hv_RoiPhi);

                                measLineParams.hv_LineRowBegin = rectDisRow - roiHeight * Math.Sin(hv_RoiPhi) / 2;
                                measLineParams.hv_LineColumnBegin = rectDisColumn - roiHeight * Math.Cos(hv_RoiPhi) / 2;
                                measLineParams.hv_LineRowEnd = rectDisRow + roiHeight * Math.Sin(hv_RoiPhi) / 2;
                                measLineParams.hv_LineColumnEnd = rectDisColumn + roiHeight * Math.Cos(hv_RoiPhi) / 2;

                                //   Console.WriteLine("二次：" + measLineParams.hv_LineRowBegin.D.ToString() + "--" + measLineParams.hv_LineColumnBegin.D.ToString()
                                //+ "--" + measLineParams.hv_LineRowEnd.D.ToString() + "--" + measLineParams.hv_LineColumnEnd.D.ToString());
                            }
                        }
                        measLineParams.bl_FitSuccess = true;

                        //HOperatorSet.GenRegionLine(out ResultLine , measLineParams.hv_LineRowBegin, measLineParams.hv_LineColumnBegin, measLineParams.hv_LineRowEnd, measLineParams.hv_LineColumnEnd);
                        //Console.WriteLine("----抓边膜宽执行成功---！");
                        //GetResultParams(measLineParams.str_ModelName, ResultLine);
                        //Console.WriteLine("抓边结果：起点行：{0}，起点列：{1}，终点行：{2}，终点列：{3}", 1,2,3,4/*measLineParams.hv_LineRowBegin, measLineParams.hv_LineColumnBegin, measLineParams.hv_LineRowEnd, measLineParams.hv_LineColumnEnd*/);
                        // HOperatorSet.CountSeconds(out Stime5);
                    }
                    else if (measLineParams.bl_DefaultLine == true)
                    {
                        measLineParams.hv_LineRowBegin = hv_StartRow - roiHeight * Math.Sin(hv_RoiPhi) / 2;
                        measLineParams.hv_LineColumnBegin = hv_StartColumn - roiHeight * Math.Cos(hv_RoiPhi) / 2;
                        measLineParams.hv_LineRowEnd = hv_StartRow + roiHeight * Math.Sin(hv_RoiPhi) / 2;
                        measLineParams.hv_LineColumnEnd = hv_StartColumn + roiHeight * Math.Cos(hv_RoiPhi) / 2;
                        //返回默认值
                        measLineParams.bl_FitSuccess = true;
                        // Console.WriteLine("抓边膜宽执行失败！原因：抓边参数异常，未抓到边。");
                    }

                }
                else
                {
                    measLineParams.bl_FitSuccess = false;
                    // Console.WriteLine("抓边膜宽执行失败！原因：无ROI或无img");
                }
            }
            catch (Exception ee)
            {
                Log4netRecord.ErrorLog.Debug("抓边模块执行失败！原因：try_catch捕获异常" + ee.ToString());
                measLineParams.bl_FitSuccess = false;
                
            }
            //HOperatorSet.CountSeconds(out Stime6);
            ho_Rectangle2.Dispose();
            ho_ImagePart.Dispose();
            ho_ImagePartProcessed.Dispose();

            bool success = measLineParams.bl_FitSuccess;
            string message = string.IsNullOrWhiteSpace(measLineParams.m_strRunMessage) ? (success ? "运行成功" : "运行失败") : measLineParams.m_strRunMessage;
            HOperatorSet.CountSeconds(out HTuple end);
            end = (end - start) * 1000;
            PluginParams.m_fElapsedMs = end;
            PluginParams.m_bRunSuccess = success;
            ExecuteCompleted?.Invoke(this, new PluginExecuteCompletedEventArgs(PluginParams, success, message, (long)end.D));
            return success;

        }

        public bool saveParams()
        {
            PluginParams.filePath = moduleInfo.filePath;
            if (PluginParams.Save())
            {
                return true;
            }
            return false;
        }

        public void SetParams(PluginParamsBase param)
        {
            this.PluginParams = (ParamsMeasureLine)param;
            measLineParams = PluginParams as ParamsMeasureLine; //里氏转换
        }

        public void GetResultParams(string varName, object obj)
        {
            Process proc = Project.Instance.GetProcessById(moduleInfo.ProcessID);
            proc.AddOutPutParam(moduleInfo.ModuleName, varName, obj);
        }
        public void setInPutParams(string module, string valueName)
        {
            Process proc = Project.Instance.GetProcessById(moduleInfo.ProcessID);
            measLineParams.ho_Image = (HImage)proc.GetOutPutParamByName(module, valueName);
        }
        public void IniObj()
        {
            measLineParams = PluginParams as ParamsMeasureLine; //里氏转换
            measLineParams.hv_HistoryMeasureTransition = measLineParams.hv_MeasureTransition;
        }

        public void DisPoseObj()
        {

        }

    }
}

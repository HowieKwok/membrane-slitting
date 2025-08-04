using HalconDotNet;

using LogTool;

using NPOI.OpenXmlFormats.Vml;
using NPOI.OpenXmlFormats.Wordprocessing;
using NPOI.SS.Formula.Functions;
using NPOI.XSSF.Streaming.Values;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Media3D;

using ViewROI3;

namespace Frame_Core
{
    /// <summary>
    /// 点检
    /// </summary>
    public class PointCheck
    {
        static IntervalExecution intervalTimeExecute = new IntervalExecution();
        public static FileOperationINI FileOperationINI = new FileOperationINI();
        /// <summary>
        /// 点检提示
        /// </summary>
        public static void PointCheckHint()
        {
            try
            {
                if (Project.Instance.systemInfo.djEnabled)
                {
                    DateTime currentTime = DateTime.Now;
                    TimeSpan currentTimePart = currentTime.TimeOfDay;

                    TimeSpan BaibanTimePart = Project.Instance.systemInfo.djCaliTimeBaiban.TimeOfDay;
                    TimeSpan YeBanTimePart = Project.Instance.systemInfo.djCaliTimeYeBan.TimeOfDay;
                    DateTime lastOKTime = Project.Instance.systemInfo.djLastOKTime;

                    ///上一次点检时间超过1小时，进入白班或夜班时间段判断是否需要点检
                    if (currentTime.Subtract(lastOKTime).TotalHours >= 1)
                    {
                        //进入白班时间段
                        if (currentTimePart.Subtract(BaibanTimePart).TotalSeconds > 0 && currentTimePart.Subtract(BaibanTimePart).TotalHours < 1)
                        {
                            //复位点检完成标志
                            Project.Instance.systemInfo.djFinish = false;
                            Project.Instance.systemInfo.Save();
                        }
                        //进入夜班时间段
                        else if (currentTimePart.Subtract(YeBanTimePart).TotalSeconds > 0 && currentTimePart.Subtract(YeBanTimePart).TotalHours < 1)
                        {
                            //复位点检完成标志
                            Project.Instance.systemInfo.djFinish = false;
                            Project.Instance.systemInfo.Save();
                        }
                    }

                    if (!Project.Instance.systemInfo.djFinish)
                    {
                        //1分钟内不重复报警
                        if (true/*intervalTimeExecute.GetWhetherToReachIntervalTime(60 * 1000)*/)
                        {
                            //点检提示
                            TJBJInfo.AlarmWriteState(AlarmName.已到点检时间请进行点检操作, StopAlarmStateEnum.停机, "");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }

        /// <summary>
        /// 获取点检结果
        /// </summary>
        /// <returns></returns>
        public static bool GetPointCheckResult()
        {
            try
            {
                ////判断是否点检完成
                if (Project.Instance.systemInfo.djEnabled)
                {
                    return Project.Instance.systemInfo.djFinish;
                }
                //未启用返回true
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
            return false;
        }



        #region 菲林片点检

        /// <summary>
        /// 获取点检数据表头
        /// </summary>
        /// <returns></returns>
        public static string[] GetPointCheckTitles()
        {
            string FilmWidth = "菲林片宽";
            string FilmHeight = "菲林片高";
            string Result = "结果";
            string TotalResult = "总结果";
            try
            {
                //FilmWidth = LanguageProject.GetLangeuageAttribute(ShowMessageEnum.菲林片宽, Project.Instance.m_SystemSettingParamter.m_CurrentLanguageType);
                //FilmHeight = LanguageProject.GetLangeuageAttribute(ShowMessageEnum.菲林片高, Project.Instance.m_SystemSettingParamter.m_CurrentLanguageType);
                //Result = LanguageProject.GetLangeuageAttribute(ShowMessageEnum.结果, Project.Instance.m_SystemSettingParamter.m_CurrentLanguageType);
                //TotalResult = LanguageProject.GetLangeuageAttribute(ShowMessageEnum.总结果, Project.Instance.m_SystemSettingParamter.m_CurrentLanguageType);


            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
            }
            return new string[]
                  {
                FilmWidth,
                Result,
                FilmHeight,
                Result,
                TotalResult
                  };

        }
        /// <summary>
        /// 实时点检是否开始判断
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="sizeCheckPointParam"></param>
        /// <returns></returns>
        public static bool RealTimePointCheckResetStartDJJudge(long frame, double currentSpeed, SizeCheckPointParam sizeCheckPointParam)
        {
            bool isStart = false;
            try
            {
                if (!sizeCheckPointParam.SSDJ) return false;

                //帧号到位置开始点检
                if (!sizeCheckPointParam.StartDJFlag && !sizeCheckPointParam.EndDJFlag)
                {
                    if (frame >= sizeCheckPointParam.CheckStartFrame && currentSpeed <= sizeCheckPointParam.PointCheckSpeedRestrict)
                    {
                        //开始点检
                        sizeCheckPointParam.ResetStartDJ();

                    }
                }


                //结束点检
                if (sizeCheckPointParam.CueerntDJCount >= sizeCheckPointParam.CheckNumber)
                {
                    sizeCheckPointParam.StartDJFlag = false;
                    sizeCheckPointParam.EndDJFlag = true;
                }
                isStart = sizeCheckPointParam.StartDJFlag;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
            }

            return isStart;
        }

        /// <summary>
        /// 点检流程
        /// </summary>
        public static void DJProcess(CameraEnum cameraEnum, HObject ho_Image, double calibrationX, double calibrationY, SizeCheckPointParam sizeCheckPointParam, out HObject preproccessRegion, out List<RectangleInfo> rectangleInfos)
        {
            rectangleInfos = new List<RectangleInfo>();
            preproccessRegion = null;
            try
            {

                ImageAlgorithm.FindFilmRectangleInfo(cameraEnum.ToString(), ho_Image, calibrationX, calibrationY, sizeCheckPointParam, out preproccessRegion, out rectangleInfos);

                if (rectangleInfos == null || rectangleInfos.Count <= 0) return;

                for (int i = 0; i < rectangleInfos.Count; i++)
                {
                    sizeCheckPointParam.CueerntDJCount++;

                    //宽度判断
                    if (Math.Abs(sizeCheckPointParam.CalibBlockWidth - rectangleInfos[i].widthReality) > sizeCheckPointParam.CalibBlockWidthTolerance)
                    {
                        rectangleInfos[i].widthResult = false;
                    }
                    else
                    {
                        rectangleInfos[i].widthResult = true;
                    }

                    //高度判断
                    if (Math.Abs(sizeCheckPointParam.CalibBlockHeiht - rectangleInfos[i].heightReality) > sizeCheckPointParam.CalibBlockHeihtTolerance)
                    {
                        rectangleInfos[i].heightResult = false;

                    }
                    else
                    {
                        rectangleInfos[i].heightResult = true;
                    }

                    //点检结果判断
                    if (!rectangleInfos[i].widthResult || !rectangleInfos[i].heightResult)
                    {
                        sizeCheckPointParam.FailAccumulativeCount++;
                        sizeCheckPointParam.AlarmAccumulativeCount++;
                        sizeCheckPointParam.StopAccumulativeCount++;
                    }

                    //点检总结果判断
                    if (sizeCheckPointParam.FailAccumulativeCount >= sizeCheckPointParam.FailAccumulativeNum)
                    {
                        sizeCheckPointParam.SizeDJResult = false;
                    }
                    else
                    {
                        sizeCheckPointParam.SizeDJResult = true;
                    }

                    //报警
                    bool isTJ = false;
                    if (sizeCheckPointParam.Stop)
                    {
                        if (sizeCheckPointParam.StopAccumulativeCount >= sizeCheckPointParam.StopAccumulativeNum)
                        {
                            TJBJInfo.AlarmWriteState(AlarmName.尺寸点检失败, StopAlarmStateEnum.停机, cameraEnum.ToString());
                            isTJ = true;
                        }
                    }

                    //报警
                    if (!isTJ)
                    {
                        if (sizeCheckPointParam.Alarm)
                        {
                            if (sizeCheckPointParam.AlarmAccumulativeCount >= sizeCheckPointParam.AlarmAccumulativeNum)
                            {
                                TJBJInfo.AlarmWriteState(AlarmName.尺寸点检失败, StopAlarmStateEnum.报警, cameraEnum.ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(cameraEnum.ToString(), ex);
            }
        }
        #endregion

        #region 3D尺寸点检
        public static void DJ3DProcess(CameraEnum cameraEnum,  Param_MsaThick3D param_MsaThick3D, ShowHalconWindow showHalconWindow)
        {
            try
            {
                if (param_MsaThick3D.LtDictionary.Count() != param_MsaThick3D.LtROIParams.Count())
                {
                    TJBJInfo.AlarmWriteState(AlarmName.尺寸点检失败, StopAlarmStateEnum.停机, $"{cameraEnum.ToString()}: 点检参数不匹配,请检查参数设置");
                    return;
                }

                for (int i = 0; i < param_MsaThick3D.LtROIParams.Count(); i++)
                {
                    ROIParams rOIParams = param_MsaThick3D.LtROIParams[i];
                    Dictionary<string, System.Windows.Forms.Control> ltDictionary = param_MsaThick3D.LtDictionary[i];
                    ViewROI3.ROI rOI = new ViewROI3.ROI() { roiParams = rOIParams, ControlDictionary = ltDictionary };

                    if (rOIParams.rOItype != ROIType.矩形1 | ltDictionary.Count() == 0)
                    {
                        continue;
                    }
                    measureMethod(cameraEnum, rOI, showHalconWindow);

                }


            }
            catch (Exception ee)
            {
                MessageNotify.AddErrorLog("3D尺寸点检过程失败："+ee.Message);

                //throw;
            }

        }


        static void  measureMethod(CameraEnum cameraEnum,ROI rOI,ShowHalconWindow showHalconWindow)
        {
            try 
            {
                ROIParams rOIParams = rOI.roiParams;
                if (rOI.roiParams.rOItype == ROIType.矩形1)
                {
                    int offsetCol = Convert.ToInt32(rOI.GetDicValue("矩形平移Col"));
                    int offsetRow = Convert.ToInt32(rOI.GetDicValue("矩形平移row"));
                    double z = Convert.ToDouble(rOI.GetDicValue("数据补偿B"));
                    int hv_Iterations = Convert.ToInt32(rOI.GetDicValue("最大迭代次数"));
                    int hv_ClippingFactor = Convert.ToInt32(rOI.GetDicValue("裁剪因子"));
                    double hv_Mult = Convert.ToDouble(rOI.GetDicValue("矫正因子"));
                    double hv_Add = Convert.ToDouble(rOI.GetDicValue("修正值"));

                    string hv_Algorithm = Convert.ToString(rOI.GetDicValue2("拟合算法"));
                    string hv_ImageType = Convert.ToString(rOI.GetDicValue2("像素类型"));
                    bool b_DataCalibration = Convert.ToBoolean(rOI.GetDicValue("启用数据校准"));
                    bool b_DataFilter = Convert.ToBoolean(rOI.GetDicValue("是否启用滤波"));
                    double d_BaseValue = Convert.ToDouble(rOI.GetDicValue("基准值"));
                    //上公差
                    double d_UpValue = Convert.ToDouble(rOI.GetDicValue("上公差"));
                    //下公差
                    double d_DownValue = Convert.ToDouble(rOI.GetDicValue("下公差"));

                    double d_BaseTolerance = Convert.ToDouble(rOI.GetDicValue("基准公差"));
                    double d_CalibrationTolerance = Convert.ToDouble(rOI.GetDicValue("校准公差"));
                    double d_FilterLowPercent = Convert.ToDouble(rOI.GetDicValue("滤除低点百分比"));
                    double d_FilterHighPercent = Convert.ToDouble(rOI.GetDicValue("滤除高点百分比"));
                    bool b_DataMeanFilter = Convert.ToBoolean(rOI.GetDicValue("是否启用均值滤波"));
                    int i_FilterNum = Convert.ToInt32(rOI.GetDicValue("滤波个数"));
                    //取点方式  
                    string strSelectMethod = Convert.ToString(rOI.GetDicValue2("取点方式"));
                    int i_LineNum = Convert.ToInt32(rOI.GetDicValue("线条数"));


                    HOperatorSet.GenRectangle1(out HObject rectangle, rOIParams.row1, rOIParams.col1, rOIParams.row2, rOIParams.col2);
                    HOperatorSet.MoveRegion(rectangle, out HObject rectanglePlane, offsetRow, offsetCol);
                    showHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = rectangle, m_stuGetColor = HalconColor.orange });
                    showHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = rectanglePlane, m_stuGetColor = HalconColor.orange });
                    HTuple Rows = 0, Columns = 0;
                    HTuple PlaneRows = 0, PlaneColumns = 0;
                    HTuple grayvalDiff = 0;


                    HOperatorSet.GetImageSize(showHalconWindow.Image3DClass.Image3D, out HTuple width, out HTuple height);

                    HOperatorSet.FitSurfaceFirstOrder(rectanglePlane, showHalconWindow.Image3DClass.Image3D, hv_Algorithm,
                    hv_Iterations, hv_ClippingFactor, out HTuple alpha, out HTuple beta, out HTuple gamma);
                    HOperatorSet.AreaCenter(rectanglePlane, out HTuple area, out HTuple row, out HTuple column);
                    HOperatorSet.GenImageSurfaceFirstOrder(out HObject imageSurface, hv_ImageType, alpha, beta, gamma, row, column, width, height);
                    HOperatorSet.SubImage(showHalconWindow.Image3DClass.Image3D, imageSurface, out HObject imageSub, hv_Mult, hv_Add);

                    if (strSelectMethod == "线取点")
                    {
                        //行数间隔
                        int rowInterval = (int)(rOIParams.row2 - rOIParams.row1) / i_LineNum;


                        for (int i = 0; i < i_LineNum; i++)
                        {
                            int row1 = (int)rOIParams.row1 + i * rowInterval;
                            int row2 = (int)rOIParams.row1 + i * rowInterval;
                            HOperatorSet.GenRegionLine(out HObject lineRegion, row1, rOIParams.col1, row2, rOIParams.col2);
                            showHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = lineRegion, m_stuGetColor = HalconColor.orange });
                            HOperatorSet.GetRegionPoints(lineRegion, out HTuple rows, out HTuple columns);
                            HOperatorSet.GetGrayval(imageSub, rows, columns, out HTuple grayvalDiffTemp);
                            if (grayvalDiffTemp.TupleLength() > 0)
                            {
                                Rows = Rows.TupleConcat(rows);
                                Columns = Columns.TupleConcat(columns);
                                grayvalDiff = grayvalDiff.TupleConcat(grayvalDiffTemp);
                            }


                        }




                    }
                    else if (strSelectMethod == "面取点")
                    {
                        HOperatorSet.GetRegionPoints(rectangle, out Rows, out Columns);
                        HOperatorSet.GetRegionPoints(rectanglePlane, out PlaneRows, out PlaneColumns);
                        HOperatorSet.GetGrayval(imageSub, Rows, Columns, out grayvalDiff);

                    }




                    imageSurface?.Dispose();
                    imageSub?.Dispose();
                    HTuple mean = 0;
                    if (grayvalDiff?.Length > 0)
                    {

                        //判断是否启用滤波
                        if (b_DataFilter)
                        {
                            //data_MaxMin_Filter(grayvalDiff, d_FilterLowPercent, d_FilterHighPercent, out grayvalDiff);
                            OtherAlgorithm.data_filter_output_index(grayvalDiff, d_FilterLowPercent, d_FilterHighPercent, out grayvalDiff, out HTuple dataFilterIndex);
                        }
                        if (b_DataMeanFilter)
                        {

                            grayvalDiff = OtherAlgorithm.AvgFilter(grayvalDiff, i_FilterNum);
                            //grayvalDiff = new HTuple(grayvalDiffList.ToArray());
                        }


                        HTuple point_Z_diff = new HTuple((-grayvalDiff));
                        HOperatorSet.TupleMean(point_Z_diff, out  mean);
                        HOperatorSet.TupleMax(point_Z_diff, out HTuple max);
                        HOperatorSet.TupleMin(point_Z_diff, out HTuple min);
                        mean = mean.TupleAbs() * 1000 + z;
                        max = max.TupleAbs() * 1000 + z;
                        min = min.TupleAbs() * 1000 + z;
                        //校准数据
                        double meanvalue = mean.D;
                        OtherAlgorithm.Calibration(b_DataCalibration, d_BaseValue, d_BaseTolerance, d_CalibrationTolerance, ref meanvalue);
                        mean = new HTuple(meanvalue);
                       //判断是否满足上下公差
                       if(!mean.D.Between(d_UpValue,d_DownValue))
                        {
                            //停机报警
                            TJBJInfo.AlarmWriteState(AlarmName.尺寸点检失败, StopAlarmStateEnum.停机, $"{cameraEnum}尺寸点检失败,ROI_{rOIParams.roiIndex}:测量值不满足上下公差");
                            showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"{rOIParams.roiIndex}_{mean.D.ToString("0.00")}um", m_stuGetColor = HalconColor.red, m_bIsWindow = false, m_fRow = (rOIParams.row1 + rOIParams.row2) / 2, m_fCol = (rOIParams.col1 + rOIParams.col2) / 2 });

                        }
                        else
                        {
                            showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"{rOIParams.roiIndex}_{mean.D.ToString("0.00")}um", m_stuGetColor = HalconColor.green, m_bIsWindow = false, m_fRow = (rOIParams.row1 + rOIParams.row2) / 2, m_fCol = (rOIParams.col1 + rOIParams.col2) / 2 });

                        }

                    }
                    else
                    {
                        //报警停机
                        TJBJInfo.AlarmWriteState(AlarmName.尺寸点检失败, StopAlarmStateEnum.停机, $"{cameraEnum}尺寸点检失败,,ROI_{rOIParams.roiIndex}:未检测出数据");
                        showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"{rOIParams.roiIndex}_{mean.D.ToString("0.00")}um", m_stuGetColor = HalconColor.red, m_bIsWindow = false, m_fRow = (rOIParams.row1 + rOIParams.row2) / 2, m_fCol = (rOIParams.col1 + rOIParams.col2) / 2 });

                    }
                    //生成文档
                    List<string> stringTitles = new List<string>();
                    stringTitles.Add("时间");
                    stringTitles.Add("帧号");
                    stringTitles.Add("ROI索引");
                    stringTitles.Add("ROI类型");
                    string strCam = cameraEnum.ToString();

                    List<string> stringData = new List<string>();

                    stringData = mean.DArr.ToList().ConvertAll(x => x.ToString("0.00")).ToList();

                    stringData.Insert(0, rOIParams.rOItype.ToString());
                    stringData.Insert(0, rOIParams.roiIndex.ToString());
                    stringData.Insert(0, showHalconWindow.m_nImageFrameID.ToString());
                    //当前时间精确到毫秒写入到第一个
                    stringData.Insert(0, DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss_fff"));

                    FileOperationCSV.WriteData_CSV(Project.Instance.paramPathSet.ProductMojuanPath + strCam + "MSA3D在线点检数据", stringTitles.ToArray(), stringData.ToArray(), "");

                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion
    }
}

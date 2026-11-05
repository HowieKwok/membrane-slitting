using HalconDotNet;
using iTextSharp.xmp.impl.xpath;
using LogTool;
using NPOI.OpenXmlFormats.Dml.Diagram;
using NPOI.OpenXmlFormats.Vml;
using Org.BouncyCastle.Utilities.IO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ViewROI3;

namespace Frame_Core
{
    public static class SizePointInspectionWorks
    {
        #region 2D尺寸点检
        /// <summary>
        /// 尺寸测量
        /// </summary>
        /// <param name="cameraEnum"></param>
        /// <param name="ho_Image"></param>
        /// <param name="showHalconWindow"></param>
        public static ExecuteResult SizeMeasureFlow_FilmRectangle(CameraEnum cameraEnum, HObject ho_Image, SizePointInspectionParam sizePointInspectionParam, ref ShowHalconWindow showHalconWindow, out List<List<SizeInfo_P>> sizeInfos)
        {
            ExecuteResult executeResult = new ExecuteResult();
            HTuple start = null, end = null;
            //TODO: 线线距离测量功能
            sizeInfos = new List<List<SizeInfo_P>>();
            HOperatorSet.CountSeconds(out start);
            try
            {
                CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
                CultureInfo cultureInfo1 = Thread.CurrentThread.CurrentUICulture;

                if (showHalconWindow.m_messageShowInfos == null) showHalconWindow.m_messageShowInfos = new List<MessageShowInfo>();
                if (showHalconWindow.m_lineShowInfos == null) showHalconWindow.m_lineShowInfos = new List<LineInfo>();
                if (showHalconWindow.m_regionShowInfos == null) showHalconWindow.m_regionShowInfos = new List<RegionShowInfo>();

                ImageAlgorithm.FindFilmRectangleInfo(cameraEnum, ho_Image, sizePointInspectionParam.p_RectangleFilm, ref showHalconWindow, out List<PointInfo> pointInfos);

                if (pointInfos.Count > 0)
                {
                    for (int i = 0; i < pointInfos.Count; i++)
                    {
                        for (int j = 0; j < sizePointInspectionParam.LineParams.Count; j++)
                        {
                            sizePointInspectionParam.LineParams[j].IsROIFollow = true;
                            if (sizePointInspectionParam.LineParams[j].IsROIFollow)
                            {
                                sizePointInspectionParam.LineParams[j].BasePointX = (int)pointInfos[i].Col;
                                sizePointInspectionParam.LineParams[j].BasePointY = (int)pointInfos[i].Row;
                            }
                        }
                        Model_LineToLineDist.FindAllLine(cameraEnum, ho_Image, sizePointInspectionParam.LineParams, out List<LineInfo1> lineInfo1s);

                        Model_LineToLineDist.DetectEdgeValueAssign(cameraEnum, lineInfo1s,  showHalconWindow.m_messageShowInfos,  showHalconWindow.m_lineShowInfos,  showHalconWindow.m_regionShowInfos);

                        Model_LineToLineDist.SizeLogicCalculate(cameraEnum, lineInfo1s, sizePointInspectionParam.LineParams, sizePointInspectionParam.SizeLogicParams_UserDefineds, out List<SizeInfo_P> sizeInfos2);

                        Model_LineToLineDist.DetectEdgeValueAssign(cameraEnum, sizeInfos2,  showHalconWindow.m_messageShowInfos);

                        if (sizeInfos2.Count > 0) sizeInfos.Add(sizeInfos2);

                    }

                }

                executeResult.IsSuccess = true;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(cameraEnum.ToString(), ex);
                executeResult.IsSuccess = false;
            }
            finally
            {
                HOperatorSet.CountSeconds(out end);
                executeResult.Time = ((end - start) * 1000).D;
                executeResult.Time = Math.Round(executeResult.Time, 2);
            }
            return executeResult;
        }

        /// <summary>
        /// 尺寸测量
        /// </summary>
        /// <param name="cameraEnum"></param>
        /// <param name="ho_Image"></param>
        /// <param name="showHalconWindow"></param>
        public static ExecuteResult SizeMeasureFlow_FilmLineWidth(CameraEnum cameraEnum, HObject ho_Image, SizePointInspectionParam sizePointInspectionParam, ref ShowHalconWindow showHalconWindow, out List<List<SizeInfo_P>> sizeInfos)
        {
            ExecuteResult executeResult = new ExecuteResult();
            HTuple start = null, end = null;
            //TODO: 线线距离测量功能
            sizeInfos = new List<List<SizeInfo_P>>();
            HOperatorSet.CountSeconds(out start);
            try
            {
                CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
                CultureInfo cultureInfo1 = Thread.CurrentThread.CurrentUICulture;

                if (showHalconWindow.m_messageShowInfos == null) showHalconWindow.m_messageShowInfos = new List<MessageShowInfo>();
                if (showHalconWindow.m_lineShowInfos == null) showHalconWindow.m_lineShowInfos = new List<LineInfo>();
                if (showHalconWindow.m_regionShowInfos == null) showHalconWindow.m_regionShowInfos = new List<RegionShowInfo>();

                ImageAlgorithm.FindFilmLineWidthInfo(cameraEnum, ho_Image, sizePointInspectionParam.p_LineWidthFilm, ref showHalconWindow, out List<PointInfo> pointInfos);

                if (pointInfos.Count > 0)
                {
                    for (int i = 0; i < pointInfos.Count; i++)
                    {
                        for (int j = 0; j < sizePointInspectionParam.LineParams.Count; j++)
                        {
                            sizePointInspectionParam.LineParams[j].IsROIFollow = true;
                            if (sizePointInspectionParam.LineParams[j].IsROIFollow)
                            {
                                sizePointInspectionParam.LineParams[j].BasePointX = (int)pointInfos[i].Col;
                                sizePointInspectionParam.LineParams[j].BasePointY = (int)pointInfos[i].Row;

                                sizePointInspectionParam.LineParams[j].RoiLength2 = sizePointInspectionParam.p_LineWidthFilm.DetectionStartRowToCentreDistance * 2;
                            }
                        }
                        Model_LineToLineDist.FindAllLine(cameraEnum, ho_Image, sizePointInspectionParam.LineParams, out List<LineInfo1> lineInfo1s);

                        Model_LineToLineDist.DetectEdgeValueAssign(cameraEnum, lineInfo1s,  showHalconWindow.m_messageShowInfos,  showHalconWindow.m_lineShowInfos,  showHalconWindow.m_regionShowInfos);

                        Model_LineToLineDist.SizeLogicCalculate(cameraEnum, lineInfo1s, sizePointInspectionParam.LineParams, sizePointInspectionParam.SizeLogicParams_UserDefineds, out List<SizeInfo_P> sizeInfos2);

                        Model_LineToLineDist.DetectEdgeValueAssign(cameraEnum, sizeInfos2,  showHalconWindow.m_messageShowInfos);

                        if (sizeInfos2.Count > 0) sizeInfos.Add(sizeInfos2);

                    }

                }

                executeResult.IsSuccess = true;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(cameraEnum.ToString(), ex);
                executeResult.IsSuccess = false;
            }
            finally
            {
                HOperatorSet.CountSeconds(out end);
                executeResult.Time = ((end - start) * 1000).D;
                executeResult.Time = Math.Round(executeResult.Time, 2);
            }
            return executeResult;
        }

        /// <summary>
        /// 获取仿真点检表头
        /// </summary>
        /// <param name="param_SizeLogics"></param>
        /// <returns></returns>
        public static List<string> GetSizePointInspectionTitle(List<Param_SizeLogic_UserDefined> param_SizeLogics)
        {
            List<string> titles = new List<string>();
            try
            {
                string str1 = "时间";
                string str2 = "帧号";
                string str3 = "图像路径";
                string str4 = "所属相机";
                string str5 = "是否通过";
                string str6 = "结果";
                string str7 = "总结果";


                titles.Add(str1);
                titles.Add(str2);
                titles.Add(str3);
                titles.Add(str4);

                for (int i = 0; i < param_SizeLogics.Count; i++)
                {
                    titles.Add(param_SizeLogics[i].Name);
                    titles.Add(str6);
                }
                titles.Add(str7);
                titles.Add(str5);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
            }
            return titles;
        }


        /// <summary>
        /// 获取是否开始点检
        /// </summary>
        /// <param name="cameraEnum"></param>
        /// <param name="frame"></param>
        /// <param name="currentSpeed"></param>
        /// <param name="sizeCheckPointParam"></param>
        /// <returns></returns>
        public static bool GetIsStratPointCheck(long frame, double currentSpeed, SizePointInspectionWorkParam sizePointInspectionWorkParam)
        {
            if (!sizePointInspectionWorkParam.SSDJ) return false;

            //帧号到位置开始点检
            if(sizePointInspectionWorkParam.LastCheckFrame==0) return true;
            if (frame - sizePointInspectionWorkParam.LastCheckFrame >= sizePointInspectionWorkParam.CheckIntervalFrame
                     && currentSpeed <= sizePointInspectionWorkParam.PointCheckSpeedRestrict)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 点检结果判断
        /// </summary>
        /// <param name="cameraEnum"></param>
        /// <param name="sizeCheckPointParam"></param>
        /// <param name="result"></param>
        public static void PointInspectionResultJudge(CameraEnum cameraEnum, int frame, bool result, SizePointInspectionWorkParam sizePointInspectionWorkParam, out bool isPass)
        {
            isPass = false;
            try
            {

                if (!result)
                {
                    sizePointInspectionWorkParam.FailAccumulativeCount++;
                }
                sizePointInspectionWorkParam.CueerntDJCount++;
                //点检总结果判断
                if (sizePointInspectionWorkParam.FailAccumulativeCount >= sizePointInspectionWorkParam.FailAccumulativeNum)
                {
                    sizePointInspectionWorkParam.SizeDJResult = false;


                    //停机                
                    if (sizePointInspectionWorkParam.Stop)
                    {
                        TJBJInfo.AlarmWriteState(AlarmName.尺寸点检失败, StopAlarmStateEnum.停机, cameraEnum.ToString());
                    }
                    //报警
                    else if (sizePointInspectionWorkParam.Alarm)
                    {
                        TJBJInfo.AlarmWriteState(AlarmName.尺寸点检失败, StopAlarmStateEnum.报警, cameraEnum.ToString());
                    }

                }
                else
                {
                    sizePointInspectionWorkParam.SizeDJResult = true;
                }
                isPass = sizePointInspectionWorkParam.SizeDJResult;
                //点检完成
                if (sizePointInspectionWorkParam.CueerntDJCount >= sizePointInspectionWorkParam.CheckNumber)
                {
                    sizePointInspectionWorkParam.ResetStartDJ();
                    sizePointInspectionWorkParam.LastCheckFrame = frame;
                    sizePointInspectionWorkParam.SizeDJResult = isPass;
                }


            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error($"FilmWorkFlow_{cameraEnum}", ex);
            }
        }

        /// <summary>
        /// 相机一键点检流程
        /// </summary>
        /// <param name="cameraEnum"></param>
        /// <param name="filePath"></param>
        /// <param name="fileName"></param>
        /// <param name="isPass"></param>
        public static ExecuteResult OnekeyPointInspectionFlow(CameraEnum cameraEnum, string filePath, out bool isPass, out List<RowData[]> sizeDatass)
        {
            ExecuteResult executeResult = new ExecuteResult();
            HTuple start = null, end = null;
            HOperatorSet.CountSeconds(out start);

            isPass = false;
            sizeDatass = new List<RowData[]>();
            try
            {
               
                if (Service_Camera.Get_CameraRelevancy((int)cameraEnum).m_PointInspectionStateParam.PointInspectionState_Size == (int)PointInspectionStateEnum.已忽略) return executeResult;


                string pointCheckResultStr = "尺寸点检结果";
                string strPass = "通过";
                string strFail = "不通过";
                SizePointInspectionParam sizePointInspectionParam = Service_Camera.Get_CameraRelevancy((int)cameraEnum).m_SizePointInspectionManage.sizePointInspectionParam;
                sizePointInspectionParam.sizePointInspectionWorkParam.ResetStartDJ();
                PointInspectionStateParam pointInspectionStateParam = Service_Camera.Get_CameraRelevancy((int)cameraEnum).m_PointInspectionStateParam;
                pointInspectionStateParam.PointInspectionState_Size = (int)PointInspectionStateEnum.点检中;

                //加载图像路径
                ImageSimManage m_ImageSimManage = new ImageSimManage();
                m_ImageSimManage.InitSimFlag();
                m_ImageSimManage.ReadImageSimPath(sizePointInspectionParam.PointCheckImgPath);

                //获取数据表头
                List<string> title = SizePointInspectionWorks.GetSizePointInspectionTitle(sizePointInspectionParam.SizeLogicParams_UserDefineds);
                string camName = cameraEnum.ToString();

                //图像信息显示
                ShowHalconWindow currentShowHalconWindow = new ShowHalconWindow();

                int imgNum = m_ImageSimManage.ImageEmulationPaths.Length > sizePointInspectionParam.sizePointInspectionWorkParam.CheckNumber ? sizePointInspectionParam.sizePointInspectionWorkParam.CheckNumber : m_ImageSimManage.ImageEmulationPaths.Length; //单相机点检数量
                for (int i = 0; i < imgNum; i++)
                {
                    m_ImageSimManage.GetZeroIndexStartNextImage(out HObject ho_Image, out string imgPath);
                    currentShowHalconWindow.Dispose();
                    currentShowHalconWindow.m_hImage = ho_Image;

                    PointInspectionFlow(cameraEnum, ho_Image, 0, imgPath,  currentShowHalconWindow, out List<RowData[]> sizeDatas);
                    sizeDatass.AddRange(sizeDatas);
                    foreach (var item in sizeDatas)
                    {
                        // 写入点检数据                                  
                        FileOperationCSV.WriteData_CSV(filePath, title.ToArray(), item, "");
                    }
         
                }

                if (isPass)
                {
                    pointInspectionStateParam.djLastOKTime_Size = DateTime.Now;
                    pointInspectionStateParam.PointInspectionState_Size = (int)PointInspectionStateEnum.点检成功;

                    // 写入点检日志
                    string strResult = cameraEnum.ToString() + pointCheckResultStr + "：" + (isPass ? strPass : strFail) + "\r\n";
                    PointInspectionWorks.FileOperationINI.Write_SendLog_ini(Project.Instance.paramPathSet.CheckPointRecordPath, strResult);
                }
                else
                {
                    pointInspectionStateParam.PointInspectionState_Size = (int)PointInspectionStateEnum.点检失败;
                }
                pointInspectionStateParam.Save();


                //释放资源
                currentShowHalconWindow.Dispose();

                executeResult.IsSuccess = true;
            }
            catch (Exception ex)
            {
                executeResult.IsSuccess = false;
                executeResult.Message = ex.Message;
                Log4netRecord.ErrorLog.Debug(ex);
                MessageBox.Show(ex.Message);
            }
            finally
            {
                HOperatorSet.CountSeconds(out end);
                executeResult.Time = ((end - start) * 1000).D;
                executeResult.Time = Math.Round(executeResult.Time, 2);
            }
            return executeResult;
        }

        /// <summary>
        /// 点检流程
        /// </summary>
        /// <param name="cameraEnum"></param>
        /// <param name="ho_Image"></param>
        /// <param name="frame"></param>
        /// <param name="imgPath"></param>
        /// <param name="showHalconWindow"></param>
        /// <param name="sizeDatass"></param>
        /// <returns></returns>
        public static ExecuteResult PointInspectionFlow(CameraEnum cameraEnum, HObject ho_Image, int frame, string imgPath,  ShowHalconWindow showHalconWindow, out List<RowData[]> sizeDatass)
        {

            ExecuteResult executeResult = new ExecuteResult();
            HTuple start = null, end = null;
            HOperatorSet.CountSeconds(out start);
            sizeDatass = new List<RowData[]>();
            string strPass = "通过";
            string strFail = "不通过";
            try
            {
                SizePointInspectionParam sizePointInspectionParam = Service_Camera.Get_CameraRelevancy((int)cameraEnum).m_SizePointInspectionManage.sizePointInspectionParam;
                //点检流程
                List<List<SizeInfo_P>> sizeInfoss = new List<List<SizeInfo_P>>();
                if (sizePointInspectionParam.SizePointInspectionArithmetic == (int)SizePointInspectionArithmeticEnum.矩形菲林片)
                {
                    SizePointInspectionWorks.SizeMeasureFlow_FilmRectangle(cameraEnum, ho_Image, sizePointInspectionParam, ref showHalconWindow, out sizeInfoss);
                }
                else
                {
                    SizePointInspectionWorks.SizeMeasureFlow_FilmLineWidth(cameraEnum, ho_Image, sizePointInspectionParam, ref showHalconWindow, out sizeInfoss);
                }

                //数据处理
                foreach (var item in sizeInfoss)//N个菲林片
                {
                    List<SizeInfo_P> sizeInfos = item;
                    bool totalResult = sizeInfos.Where(x => x.result == false).Count() == 0;

                    SizePointInspectionWorks.PointInspectionResultJudge(cameraEnum, frame, totalResult, sizePointInspectionParam.sizePointInspectionWorkParam, out bool isPass);

                    List<RowData> rowDatas = new List<RowData>();
                    RowData timeRowData = new RowData() { dataStr = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"), resultType = ResultType.OK };
                    RowData frameData = new RowData() { dataStr = frame.ToString() };
                    RowData imgPathRowData = new RowData() { dataStr = imgPath, resultType = ResultType.OK };
                    RowData camRowData = new RowData() { dataStr = cameraEnum.ToString() };
                    rowDatas.Add(timeRowData);
                    rowDatas.Add(frameData);
                    rowDatas.Add(imgPathRowData);
                    rowDatas.Add(camRowData);

                    for (int j = 0; j < sizeInfos.Count; j++)//N个尺寸
                    {
                        RowData rowData = new RowData();
                        rowData.dataStr = sizeInfos[j].value.ToString();
                        rowData.resultType = sizeInfos[j].result ? ResultType.OK : ResultType.NG;
                        rowDatas.Add(rowData);

                        RowData rowResultData = new RowData();
                        rowResultData.dataStr = sizeInfos[j].result ? "OK" : "NG";
                        rowResultData.resultType = sizeInfos[j].result ? ResultType.OK : ResultType.NG;
                        rowDatas.Add(rowResultData);
                    }


                    RowData totalResultRowData = new RowData() { dataStr = totalResult ? "OK" : "NG", resultType = totalResult ? ResultType.OK : ResultType.NG };
                    RowData camResultRowData = new RowData() { dataStr = isPass ? strPass : strFail, resultType = isPass ? ResultType.OK : ResultType.NG };

                    rowDatas.Add(totalResultRowData);
                    rowDatas.Add(camResultRowData);

                    sizeDatass.Add(rowDatas.ToArray());

                }
                executeResult.IsSuccess = true;
            }
            catch (Exception ex)
            {
                executeResult.IsSuccess = false;
                executeResult.Message = ex.Message;
                Log4netRecord.ErrorLog.Debug(ex);
                MessageBox.Show(ex.Message);
            }
            finally
            {
                HOperatorSet.CountSeconds(out end);
                executeResult.Time = ((end - start) * 1000).D;
                executeResult.Time = Math.Round(executeResult.Time, 2);
            }
            return executeResult;
        }
        #endregion

        #region 3D尺寸点检

        static void measureMethod(CameraEnum cameraEnum, ROI rOI, ShowHalconWindow showHalconWindow)
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
                        HOperatorSet.TupleMean(point_Z_diff, out mean);
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
                        if (!mean.D.Between(d_UpValue, d_DownValue))
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

using Eholly.ZChart;
using Frame_Core;
using HalconDotNet;
using IKapBoardClassLibrary;
using LogTool;
using Mef_VisionFrame;
using NPOI.OpenXmlFormats.Vml;
using NPOI.SS.Formula.Functions;
using OpenTK.Input;
using ScottPlot;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mef_VisionFram
{
    public class BIZ_DataWorker_Pretargeting : IBizWorkerBase
    {
        private BIZ_ThreadManage_Pretargeting bIZ_ThreadManage = new BIZ_ThreadManage_Pretargeting();
        BIZ_Variable_Pretargeting bIZ_Variable = new BIZ_Variable_Pretargeting();
        Form_Pretargeting form;
        OverHangCraft overHangCraft;

        public DisplayWindowClass HWC1;
        public DisplayWindowClass HWC2;
        public DisplayWindowClass HWC3;
        public DisplayWindowClass HWC4;

        //曲线图
        Dictionary<string, ChartUIParam> chartUIParams = new Dictionary<string, ChartUIParam>();

        IntervalExecution intervalExecution = new IntervalExecution();
        IntervalExecution intervalExecution_Read = new IntervalExecution();
        public object Prod_Lock = new object();

        public BIZ_DataWorker_Pretargeting(Form_Pretargeting _form)
        {

            form = _form;
            UpdateCraft();

            HWC1 = new DisplayWindowClass(form.HWC1, form.TB_PositionAndGray1, form.PB_FitWin1, CameraEnum.Cam1, form.HWC1.Name);
            HWC2 = new DisplayWindowClass(form.HWC2, form.TB_PositionAndGray2, form.PB_FitWin2, CameraEnum.Cam2, form.HWC2.Name);
            HWC3 = new DisplayWindowClass(form.HWC3, form.TB_PositionAndGray3, form.PB_FitWin3, CameraEnum.Cam3, form.HWC3.Name);
            HWC4 = new DisplayWindowClass(form.HWC4, form.TB_PositionAndGray4, form.PB_FitWin4, CameraEnum.Cam4, form.HWC4.Name);

            InitVariable();
            InitForm();

            ThreadManage = bIZ_ThreadManage;

            bIZ_ThreadManage.InitThread();

            bIZ_ThreadManage.thd_ImageShowAndSave.Start(5, "ThdImageShowAndSave");
            bIZ_ThreadManage.thd_ImageShowAndSave.PerformWork -= ImageShowSave;
            bIZ_ThreadManage.thd_ImageShowAndSave.PerformWork += ImageShowSave;

            bIZ_ThreadManage.thd_Communication.Start(500, "ThdCommunication");
            bIZ_ThreadManage.thd_Communication.PerformWork -= Communication;
            bIZ_ThreadManage.thd_Communication.PerformWork += Communication;

        }

        bool cam2_DealWithFinish = false;
        bool cam3_DealWithFinish = false;
        bool cam4_DealWithFinish = false;
        public void WorkerProcess()
        {
            CamEACornerProduct camEAProduct = null;
            bool result = false;
            try
            {
                CamDetectionVariable camDetectionVariable = Project.Instance.cameraManager[CameraEnum.Cam1].m_CamDetectionVariable;

                //========================================================
                //【1】开始处理数据,在这里处理数据，超时时间为1000ms
                ProcessExecutionResult processExecutionResult = IntervalExecution.DelayExecute(DateTime.Now, Project.Instance.cutOverlayPLCManage.m_DevParam.TimeOutTime, GetProdResult);

                //综合数据
                if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面A极片纠偏工位 ||
                 Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面B极片纠偏工位)
                {
                    camEAProduct = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam4;
                }
                else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面A极片纠偏工位 ||
                       Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面B极片纠偏工位)
                {
                    camEAProduct = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam1;
                }
                else
                {
                    camEAProduct = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam1;
                }

                double pointX = 0;
                double pointY = 0;
                double BaseLineX1 = 0;
                double BaseLineY1 = 0;
                double BaseLineX2 = 0;
                double BaseLineY2 = 0;
                double coord_x = -999;
                double coord_y = -999;
                double coord_angle = -999;

                double ImgInitPointX = -999;
                double ImgInitPointY = -999;
                double ImgInitPointAngle = -999;

                double UpperLimitX = -999;
                double LowerlimitX = -999;
                double UpperLimitY = -999;
                double LowerlimitY = -999;
                double UpperLimitAngle = -999;
                double LowerlimitAngle = -999;
                double offsetValueX = -999;
                double offsetValueY = -999;
                double offsetValueR = -999;

                double MachineHandBaseX = -999;
                double MachineHandBaseY = -999;
                double MachineHandBaseR = -999;

                double PlatformX = -999;
                double PlatformY = -999;
                double PlatformR = -999;

                bool coordResult_x = false;
                bool coordResult_y = false;
                bool coordResult_angle = false;

                short returnResult = 0;
                List<SizeInfo> sizeInfos = new List<SizeInfo>();
                RowData timeRowData = new RowData() { dataStr = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff"), resultType = ResultType.OK };
                //读取相机拍照信号
                bIZ_Variable.PhotoSignal = ReadPhotoSignal();
                bIZ_Variable.pretargetingBatteryProducts.EAIndex = bIZ_Variable.PhotoSignal;

                //读取上电芯或下电芯信号
                int UpOrDownCell = 0;
                if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面A极片纠偏工位)
                {
                    Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_UpOrDownCell_AOVHResult, ReadLogSaveType.True, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    UpOrDownCell = (int)Project.Instance.cutOverlayPLCManage.m_DevAddress.In_UpOrDownCell_AOVHResult.Value;
                }
                else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面A极片纠偏工位)
                {
                    Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_UpOrDownCell_BOVHResult, ReadLogSaveType.True, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    UpOrDownCell = (int)Project.Instance.cutOverlayPLCManage.m_DevAddress.In_UpOrDownCell_BOVHResult.Value;
                }
                bIZ_Variable.UpOrDownCell = UpOrDownCell;

                if (processExecutionResult == ProcessExecutionResult.Success)
                {

                    //获取当前平台坐标
                    Project.Instance.cutOverlayPLCManage.Pretargeting_ReadPhotoPlatformPos(ReadLogSaveType.True, out PlatformX, out PlatformY, out PlatformR);

                    //========================================================
                    //【2】处理数据              
                    IntegratedData(PlatformX, PlatformY, PlatformR, out coord_x, out coord_y, out coord_angle, out result, out pointX, out pointY,
                          out BaseLineX1, out BaseLineY1, out BaseLineX2, out BaseLineY2, out coordResult_x, out coordResult_y, out coordResult_angle,
                           out ImgInitPointX, out ImgInitPointY, out ImgInitPointAngle, out UpperLimitX, out LowerlimitX, out UpperLimitY, out LowerlimitY,
                           out UpperLimitAngle, out LowerlimitAngle, out offsetValueX, out offsetValueY, out offsetValueR);

                    MachineHandBaseX = Project.Instance.cameraManager[overHangCraft.BaseCamera].param_PretargetingCalib.MachineHandBCValue.Col;
                    MachineHandBaseY = Project.Instance.cameraManager[overHangCraft.BaseCamera].param_PretargetingCalib.MachineHandBCValue.Row;
                    MachineHandBaseR = Project.Instance.cameraManager[overHangCraft.BaseCamera].param_PretargetingCalib.MachineHandBCValue.Angle;



                    //计算尺寸
                    CalculateSize(out sizeInfos, out bool sizeAllResult);

                    //写入尺寸到PLC
                    Project.Instance.cutOverlayPLCManage.Pretargeting_WriteSize(sizeInfos, ReadLogSaveType.True);


                    result = (result && sizeAllResult);

                    returnResult = result ? (short)1 : (short)2;

                    //添加曲线图数据
                    //第一次拍照
                    if (bIZ_Variable.PhotoSignal >= 0 && bIZ_Variable.PhotoSignal < 10)
                    {
                        AddChartData("第一次拍照X偏移", coord_x);
                        AddChartData("第一次拍照Y偏移", coord_y);
                        AddChartData("第一次拍照角度", coord_angle);
                    }
                    //第二次拍照
                    else if (bIZ_Variable.PhotoSignal >= 10 && bIZ_Variable.PhotoSignal < 20)
                    {
                        AddChartData("第二次拍照X偏移", coord_x);
                        AddChartData("第二次拍照Y偏移", coord_y);
                        AddChartData("第二次拍照角度", coord_angle);
                    }

                    foreach (var item in sizeInfos)
                    {
                        AddChartData(item.Description, item.DataResult);
                    }


                }
                else if (processExecutionResult == ProcessExecutionResult.Overtime)
                {

                }
                else if (processExecutionResult == ProcessExecutionResult.Exception)
                {

                }


                RowData ReturnResultData = new RowData() { dataStr = "0" };
                if (bIZ_Variable.PhotoSignal > 0)
                {
                    //================================================================
                    //【3.1】发送数据到PLC
                    SendResult((float)coord_x, (float)coord_y, (float)coord_angle, returnResult);
                    ReturnResultData.dataStr = returnResult.ToString();
                    ReturnResultData.resultType = returnResult == 1 ? ResultType.OK : ResultType.NG;
                }


                //================================================================
                //【3】数据显示、保存
                List<RowData> showRowDataList = new List<RowData>();
                List<RowData> saveRowDataList = new List<RowData>();


                RowData XOffsetData = new RowData() { dataStr = coord_x.ToString("F3"), resultType = coordResult_x ? ResultType.OK : ResultType.NG };
                RowData YOffsetData = new RowData() { dataStr = coord_y.ToString("F3"), resultType = coordResult_y ? ResultType.OK : ResultType.NG };
                RowData AngleData = new RowData() { dataStr = coord_angle.ToString("F3"), resultType = coordResult_angle ? ResultType.OK : ResultType.NG };
                RowData resultData = new RowData() { dataStr = result ? "OK" : "NG", resultType = result ? ResultType.OK : ResultType.NG };
                RowData upOrDownCellData = new RowData() { dataStr = bIZ_Variable.UpOrDownCell.ToString(), resultType = ResultType.OK };
                RowData PolePlateNumberData = new RowData() { dataStr = bIZ_Variable.PhotoSignal.ToString(), resultType = ResultType.OK };

                RowData ImgInitPointXData = new RowData() { dataStr = ImgInitPointX.ToString("F3"), resultType = ResultType.OK };
                RowData ImgInitPointYData = new RowData() { dataStr = ImgInitPointY.ToString("F3"), resultType = ResultType.OK };
                RowData ImgInitPointAngleData = new RowData() { dataStr = ImgInitPointAngle.ToString("F3"), resultType = ResultType.OK };

                RowData MachineHandBaseXData = new RowData() { dataStr = MachineHandBaseX.ToString("F3"), resultType = ResultType.OK };
                RowData MachineHandBaseYData = new RowData() { dataStr = MachineHandBaseY.ToString("F3"), resultType = ResultType.OK };
                RowData MachineHandBaseRData = new RowData() { dataStr = MachineHandBaseR.ToString("F3"), resultType = ResultType.OK };

                RowData offsetValueXData = new RowData() { dataStr = offsetValueX.ToString("F3"), resultType = ResultType.OK };
                RowData offsetValueYData = new RowData() { dataStr = offsetValueY.ToString("F3"), resultType = ResultType.OK };
                RowData offsetValueRData = new RowData() { dataStr = offsetValueR.ToString("F3"), resultType = ResultType.OK };

                RowData UpperLimitXData = new RowData() { dataStr = UpperLimitX.ToString("F3"), resultType = ResultType.OK };
                RowData LowerlimitXData = new RowData() { dataStr = LowerlimitX.ToString("F3"), resultType = ResultType.OK };
                RowData UpperLimitYData = new RowData() { dataStr = UpperLimitY.ToString("F3"), resultType = ResultType.OK };
                RowData LowerlimitYData = new RowData() { dataStr = LowerlimitY.ToString("F3"), resultType = ResultType.OK };
                RowData UpperLimitAngleData = new RowData() { dataStr = UpperLimitAngle.ToString("F3"), resultType = ResultType.OK };
                RowData LowerlimitAngleData = new RowData() { dataStr = LowerlimitAngle.ToString("F3"), resultType = ResultType.OK };

                RowData PlatformXData = new RowData() { dataStr = PlatformX.ToString("F3"), resultType = ResultType.OK };
                RowData PlatformYData = new RowData() { dataStr = PlatformY.ToString("F3"), resultType = ResultType.OK };
                RowData PlatformRData = new RowData() { dataStr = PlatformR.ToString("F3"), resultType = ResultType.OK };

                RowData cam1FrameData = new RowData() { dataStr = bIZ_Variable.pretargetingBatteryProducts?.EAProduct_Cam1?.showHalconWindow?.m_nImageFrameID.ToString(), resultType = ResultType.OK };
                RowData cam2FrameData = new RowData() { dataStr = bIZ_Variable.pretargetingBatteryProducts?.EAProduct_Cam2?.showHalconWindow?.m_nImageFrameID.ToString(), resultType = ResultType.OK };
                RowData cam3FrameData = new RowData() { dataStr = bIZ_Variable.pretargetingBatteryProducts?.EAProduct_Cam3?.showHalconWindow?.m_nImageFrameID.ToString(), resultType = ResultType.OK };
                RowData cam4FrameData = new RowData() { dataStr = bIZ_Variable.pretargetingBatteryProducts?.EAProduct_Cam4?.showHalconWindow?.m_nImageFrameID.ToString(), resultType = ResultType.OK };

                showRowDataList.Add(timeRowData);

                showRowDataList.Add(XOffsetData);
                showRowDataList.Add(YOffsetData);
                showRowDataList.Add(AngleData);
                showRowDataList.Add(resultData);
                showRowDataList.Add(upOrDownCellData);
                showRowDataList.Add(PolePlateNumberData);
                showRowDataList.Add(ReturnResultData);

                showRowDataList.Add(ImgInitPointXData);
                showRowDataList.Add(ImgInitPointYData);
                showRowDataList.Add(ImgInitPointAngleData);
                showRowDataList.Add(MachineHandBaseXData);
                showRowDataList.Add(MachineHandBaseYData);
                showRowDataList.Add(MachineHandBaseRData);
                showRowDataList.Add(offsetValueXData);
                showRowDataList.Add(offsetValueYData);
                showRowDataList.Add(offsetValueRData);
                showRowDataList.Add(UpperLimitXData);
                showRowDataList.Add(LowerlimitXData);
                showRowDataList.Add(UpperLimitYData);
                showRowDataList.Add(LowerlimitYData);
                showRowDataList.Add(UpperLimitAngleData);
                showRowDataList.Add(LowerlimitAngleData);
                showRowDataList.Add(PlatformXData);
                showRowDataList.Add(PlatformYData);
                showRowDataList.Add(PlatformRData);

                showRowDataList.Add(cam1FrameData);
                showRowDataList.Add(cam2FrameData);
                showRowDataList.Add(cam3FrameData);
                showRowDataList.Add(cam4FrameData);


                saveRowDataList = showRowDataList;
                NListView.Add(form.listView1, showRowDataList.ToArray(), true);

                FileOperationCSV.WriteData_CSV(Project.Instance.paramPathSet.PretargetingProdDataPath, camDetectionVariable.DataTitlesSave, saveRowDataList.ToArray(), "预定位");

                //尺寸数据
                List<RowData> sizeRowDataList = new List<RowData>();
                sizeRowDataList.Add(timeRowData);
                for (int i = 0; i < sizeInfos.Count; i++)
                {
                    sizeRowDataList.Add(new RowData() { dataStr = sizeInfos[i].DataResult.ToString(), resultType = sizeInfos[i].resultType });
                }
                sizeRowDataList.Add(resultData);
                sizeRowDataList.Add(upOrDownCellData);
                sizeRowDataList.Add(PolePlateNumberData);
                sizeRowDataList.Add(ReturnResultData);
                sizeRowDataList.Add(cam1FrameData);
                sizeRowDataList.Add(cam2FrameData);
                sizeRowDataList.Add(cam3FrameData);
                sizeRowDataList.Add(cam4FrameData);

                NListView.Add(form.listView2, sizeRowDataList.ToArray(), true);
                FileOperationCSV.WriteData_CSV(Project.Instance.paramPathSet.PretargetingProdSizeDataPath, bIZ_Variable.sizeDataTitle.ToArray(), sizeRowDataList.ToArray(), "预定位尺寸数据");

                //================================================================
                //【4】原图显示、保存
                CamEACornerProduct EAProduct = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam1;
                string str1 = $"X偏移:{coord_x.ToString("F3")}mm  #[{bIZ_Variable.XBiasMin}_{bIZ_Variable.XBiasMax}]";
                string str2 = $"Y偏移:{coord_y.ToString("F3")}mm  #[{bIZ_Variable.YBiasMin}_{bIZ_Variable.YBiasMax}]";
                string str3 = $"角度:{coord_angle.ToString("F3")}  #[{bIZ_Variable.AngleBiasMin}_{bIZ_Variable.AngleBiasMax}]";
                camEAProduct.showHalconWindow?.m_messageShowInfos?.Add(new MessageShowInfo() { m_strMessage = result ? "OK" : "NG", m_stuGetColor = result ? HalconColor.green : HalconColor.red, m_nFontSize = 20 });
                camEAProduct.showHalconWindow?.m_messageShowInfos?.Add(new MessageShowInfo() { m_strMessage = str1, m_stuGetColor = coordResult_x ? HalconColor.green : HalconColor.red });
                camEAProduct.showHalconWindow?.m_messageShowInfos?.Add(new MessageShowInfo() { m_strMessage = str2, m_stuGetColor = coordResult_y ? HalconColor.green : HalconColor.red });
                camEAProduct.showHalconWindow?.m_messageShowInfos?.Add(new MessageShowInfo() { m_strMessage = str3, m_stuGetColor = coordResult_angle ? HalconColor.green : HalconColor.red });
                BIZ_General.AddMessageShowWindow(sizeInfos, camEAProduct.showHalconWindow);
                //camEAProduct.showHalconWindow?.m_messageShowInfos?.Add(new MessageShowInfo() { m_strMessage = $"基准点X：{pointX}", m_stuGetColor = HalconColor.green });
                //camEAProduct.showHalconWindow?.m_messageShowInfos?.Add(new MessageShowInfo() { m_strMessage = $"基准点Y：{pointY}", m_stuGetColor = HalconColor.green });
                //camEAProduct.showHalconWindow?.m_messageShowInfos?.Add(new MessageShowInfo() { m_strMessage = $"基准线X1：{BaseLineX1}", m_stuGetColor = HalconColor.green });
                //camEAProduct.showHalconWindow?.m_messageShowInfos?.Add(new MessageShowInfo() { m_strMessage = $"基准线Y1：{BaseLineY1}", m_stuGetColor = HalconColor.green });
                //camEAProduct.showHalconWindow?.m_messageShowInfos?.Add(new MessageShowInfo() { m_strMessage = $"基准线X2：{BaseLineX2}", m_stuGetColor = HalconColor.green });
                //camEAProduct.showHalconWindow?.m_messageShowInfos?.Add(new MessageShowInfo() { m_strMessage = $"基准线Y2：{BaseLineY2}", m_stuGetColor = HalconColor.green });
                //camEAProduct.showHalconWindow?.m_messageShowInfos?.Add(new MessageShowInfo() { m_strMessage = $"Cam2X：{bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam2.coorEaPoints[0].AffinePointX}", m_stuGetColor = HalconColor.green });
                //camEAProduct.showHalconWindow?.m_messageShowInfos?.Add(new MessageShowInfo() { m_strMessage = $"Cam2Y：{bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam2.coorEaPoints[0].AffinePointY}", m_stuGetColor = HalconColor.green });
                //camEAProduct.showHalconWindow?.m_messageShowInfos?.Add(new MessageShowInfo() { m_strMessage = $"Cam3X：{bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam3.coorEaPoints[0].AffinePointX}", m_stuGetColor = HalconColor.green });
                //camEAProduct.showHalconWindow?.m_messageShowInfos?.Add(new MessageShowInfo() { m_strMessage = $"Cam3Y：{bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam3.coorEaPoints[0].AffinePointY}", m_stuGetColor = HalconColor.green });

                LineInfo currentLine = new LineInfo();
                currentLine.RowBegin = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam2.coorEaPoints[0].AffinePointY;
                currentLine.ColBegin = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam2.coorEaPoints[0].AffinePointX;
                currentLine.RowEnd = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam3.coorEaPoints[0].AffinePointY;
                currentLine.ColEnd = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam3.coorEaPoints[0].AffinePointX;

                if (lineInfoTemp != null)
                {
                    HOperatorSet.IntersectionLines(currentLine.RowBegin, currentLine.ColBegin, currentLine.RowEnd, currentLine.ColEnd,
                                              lineInfoTemp.RowBegin, lineInfoTemp.ColBegin, lineInfoTemp.RowEnd, lineInfoTemp.ColEnd, out HTuple row1, out HTuple col1, out HTuple dd);
                    //camEAProduct.showHalconWindow?.m_messageShowInfos?.Add(new MessageShowInfo() { m_strMessage = $"Row:{row1}    Col:{col1}", m_stuGetColor = HalconColor.blue });
                }
                else
                {
                    lineInfoTemp = new LineInfo();
                }
                lineInfoTemp.RowBegin = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam2.coorEaPoints[0].AffinePointY;
                lineInfoTemp.ColBegin = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam2.coorEaPoints[0].AffinePointX;
                lineInfoTemp.RowEnd = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam3.coorEaPoints[0].AffinePointY;
                lineInfoTemp.ColEnd = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam3.coorEaPoints[0].AffinePointX;




            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
            try
            {
                //图像显示
                if (true)
                {

                    PrimitiveTransition(CameraEnum.Cam1, bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam1.showHalconWindow, result, camEAProduct.showHalconWindow.m_nImageFrameID, bIZ_Variable.PhotoSignal, out Operation_ImageInfo operation_ImageInfo);
                    HWC1.operation_Image.imageInfosShow.Enqueue(operation_ImageInfo);
                }

                //图像显示
                if (true)
                {
                    PrimitiveTransition(CameraEnum.Cam2, bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam2.showHalconWindow, result, camEAProduct.showHalconWindow.m_nImageFrameID, bIZ_Variable.PhotoSignal, out Operation_ImageInfo operation_ImageInfo);
                    HWC2.operation_Image.imageInfosShow.Enqueue(operation_ImageInfo);
                }

                //图像显示
                if (true)
                {
                    PrimitiveTransition(CameraEnum.Cam3, bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam3.showHalconWindow, result, camEAProduct.showHalconWindow.m_nImageFrameID, bIZ_Variable.PhotoSignal, out Operation_ImageInfo operation_ImageInfo);
                    HWC3.operation_Image.imageInfosShow.Enqueue(operation_ImageInfo);
                }

                //图像显示
                if (true)
                {
                    PrimitiveTransition(CameraEnum.Cam4, bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam4.showHalconWindow, result, camEAProduct.showHalconWindow.m_nImageFrameID, bIZ_Variable.PhotoSignal, out Operation_ImageInfo operation_ImageInfo);
                    HWC4.operation_Image.imageInfosShow.Enqueue(operation_ImageInfo);
                }

                //初始化变量
                bIZ_Variable.pretargetingBatteryProducts = new PretargetingBatteryProduct();
            }
            catch (Exception ex)
            {
                bIZ_Variable.pretargetingBatteryProducts = new PretargetingBatteryProduct();
                Log4netRecord.ErrorLog.Error(ex);
            }
        }
        LineInfo lineInfoTemp = null;
        public void ProcessingData_Cam1(ProductBase camEAProduct1)
        {

            CameraEnum cameraEnum = CameraEnum.Cam1;

            try
            {

                HOperatorSet.CountSeconds(out camEAProduct1.Image_StartDataProcessTime);
                CamEACornerProduct camEAProduct = camEAProduct1 as CamEACornerProduct;

                //计算图像坐标在世界坐标系下目标点
                double currentPointCamX = camEAProduct.coorEaPoints[0].CPointX;
                double currentPointCamY = camEAProduct.coorEaPoints[0].CPointY;

                if (false)
                {
                    ////非主相机，需要进行坐标转换
                    //CameraEnum mainCameraEnum = overHangCraft.BaseCamera;
                    //if (cameraEnum != mainCameraEnum)
                    //{
                    //    //获取当前相机到主相机到的变换矩阵
                    //    bool isSuccess = ImageAlgorithm.GetHomMat2dCompose(Project.Instance.cameraManager[mainCameraEnum].param_Checkerboard.HomMat2DInvert, Project.Instance.cameraManager[cameraEnum].param_Checkerboard.HomMat2D, out HTuple homMat2d);
                    //    if (isSuccess)
                    //    {
                    //        //计算图像坐标在主相机坐标系下目标点
                    //        isSuccess = ImageAlgorithm.GetAffinePoints2d(homMat2d, currentPointCamX, currentPointCamY, out double out_currentX, out double out_currentY);

                    //        if (isSuccess)
                    //        {
                    //            camEAProduct.coorEaPoints[0].AffinePointX = out_currentX;
                    //            camEAProduct.coorEaPoints[0].AffinePointY = out_currentY;
                    //            camEAProduct.coorEaPoints[0].MainCamCoordGetSucceed = true;
                    //        }
                    //        else
                    //        {
                    //            Log4netRecord.RunLog.Info($"获取当前相机到主相机坐标失败：{cameraEnum}->{mainCameraEnum}");
                    //        }
                    //    }
                    //    else
                    //    {
                    //        Log4netRecord.RunLog.Info($"获取当前相机到主相机变换矩阵失败：{cameraEnum}->{mainCameraEnum}");
                    //    }
                    //}
                    ////主相机，直接赋值
                    //else
                    //{
                    //    camEAProduct.coorEaPoints[0].AffinePointX = currentPointCamX;
                    //    camEAProduct.coorEaPoints[0].AffinePointY = currentPointCamY;
                    //    camEAProduct.coorEaPoints[0].MainCamCoordGetSucceed = true;
                    //}
                }


                //添加基准边显示
                if (Project.Instance.cameraManager[cameraEnum].param_PretargetingCalib.BaseLines != null)
                {
                    foreach (var item in Project.Instance.cameraManager[cameraEnum].param_PretargetingCalib.BaseLines)
                    {
                        item.Color = HalconColor.blue;
                        LineInfo lineInfo1 = ImageAlgorithm.ExtendLin(item, 5000);
                        if (lineInfo1 != null)
                        {
                            camEAProduct.showHalconWindow?.m_lineShowInfos?.Add(lineInfo1);
                        }

                    }
                }

                bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam1 = camEAProduct;
                bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam1.CamFinish = true;


                //综合数据流程
                WorkerProcess();


                HOperatorSet.CountSeconds(out camEAProduct1.Image_DataProcessTime);
                HOperatorSet.CountSeconds(out camEAProduct1.Image_EndTime);

                //计算时间
                BIZ_General.CalculateProcessingTime(camEAProduct1);



                Log4netRecord.RunLog.Info($"{cameraEnum}处理完成");

                //综合数据完成其他相机处理线程才完成
                cam2_DealWithFinish = true;
                cam3_DealWithFinish = true;
                cam4_DealWithFinish = true;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }

        }

        public void ProcessingData_Cam2(ProductBase camEAProduct1)
        {

            CameraEnum cameraEnum = CameraEnum.Cam2;

            try
            {

                HOperatorSet.CountSeconds(out camEAProduct1.Image_StartDataProcessTime);
                CamEACornerProduct camEAProduct = camEAProduct1 as CamEACornerProduct;

                //计算图像坐标在世界坐标系下目标点
                double currentPointCamX = camEAProduct.coorEaPoints[0].CPointX;
                double currentPointCamY = camEAProduct.coorEaPoints[0].CPointY;

                if (false)
                {
                    ////非主相机，需要进行坐标转换
                    //CameraEnum mainCameraEnum = overHangCraft.BaseCamera;
                    //if (cameraEnum != mainCameraEnum)
                    //{
                    //    //获取当前相机到主相机到的变换矩阵
                    //    bool isSuccess = ImageAlgorithm.GetHomMat2dCompose(Project.Instance.cameraManager[mainCameraEnum].param_Checkerboard.HomMat2DInvert, Project.Instance.cameraManager[cameraEnum].param_Checkerboard.HomMat2D, out HTuple homMat2d);
                    //    if (isSuccess)
                    //    {
                    //        //计算图像坐标在主相机坐标系下目标点
                    //        isSuccess = ImageAlgorithm.GetAffinePoints2d(homMat2d, currentPointCamX, currentPointCamY, out double out_currentX, out double out_currentY);

                    //        if (isSuccess)
                    //        {
                    //            camEAProduct.coorEaPoints[0].AffinePointX = out_currentX;
                    //            camEAProduct.coorEaPoints[0].AffinePointY = out_currentY;
                    //            camEAProduct.coorEaPoints[0].MainCamCoordGetSucceed = true;
                    //        }
                    //        else
                    //        {
                    //            Log4netRecord.RunLog.Info($"获取当前相机到主相机坐标失败：{cameraEnum}->{mainCameraEnum}");
                    //        }
                    //    }
                    //    else
                    //    {
                    //        Log4netRecord.RunLog.Info($"获取当前相机到主相机变换矩阵失败：{cameraEnum}->{mainCameraEnum}");
                    //    }
                    //}
                    ////主相机，直接赋值
                    //else
                    //{
                    //    camEAProduct.coorEaPoints[0].AffinePointX = currentPointCamX;
                    //    camEAProduct.coorEaPoints[0].AffinePointY = currentPointCamY;
                    //    camEAProduct.coorEaPoints[0].MainCamCoordGetSucceed = true;
                    //}
                }

                //当前图像坐标映射到棋盘格              
                bool isSuccess = ImageAlgorithm.GetAffinePoints2d(Project.Instance.cameraManager[cameraEnum].param_PretargetingCalib.HomMat2D, currentPointCamX, currentPointCamY, out double out_currentX, out double out_currentY);

                //棋盘格映射到平台
                if (isSuccess)
                {
                    camEAProduct.coorEaPoints[0].AffinePointX = out_currentX;
                    camEAProduct.coorEaPoints[0].AffinePointY = out_currentY;
                    camEAProduct.coorEaPoints[0].MainCamCoordGetSucceed = true;

                }
                else
                {
                    Log4netRecord.RunLog.Info($"获取当前相机到棋盘格坐标失败：{cameraEnum}");
                }



                //添加基准边显示
                if (Project.Instance.cameraManager[cameraEnum].param_PretargetingCalib.BaseLines != null)
                {
                    foreach (var item in Project.Instance.cameraManager[cameraEnum].param_PretargetingCalib.BaseLines)
                    {
                        item.Color = HalconColor.blue;
                        LineInfo lineInfo1 = ImageAlgorithm.ExtendLin(item, 5000);
                        if (lineInfo1 != null)
                        {
                            camEAProduct.showHalconWindow?.m_lineShowInfos?.Add(lineInfo1);
                        }

                    }
                }



                HOperatorSet.CountSeconds(out camEAProduct1.Image_DataProcessTime);
                HOperatorSet.CountSeconds(out camEAProduct1.Image_EndTime);

                //计算时间
                BIZ_General.CalculateProcessingTime(camEAProduct1);

                bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam2 = camEAProduct;
                bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam2.CamFinish = true;

                Log4netRecord.RunLog.Info($"{cameraEnum}处理完成");

                //延时执行，等待相机综合数据处理完成
                DelayExecuteProcess delayExecuteProcess = () =>
                {
                    if (cam2_DealWithFinish)
                    {
                        return ProcessExecutionResult.Success;
                    }
                    return ProcessExecutionResult.ContinueExecute;
                };
                ProcessExecutionResult processExecutionResult = IntervalExecution.DelayExecute(DateTime.Now, 500, delayExecuteProcess);
                if (processExecutionResult == ProcessExecutionResult.Overtime)
                {
                    Log4netRecord.RunLog.Info($"相机综合数据处理超时：{cameraEnum}");
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }

        public void ProcessingData_Cam3(ProductBase camEAProduct1)
        {

            CameraEnum cameraEnum = CameraEnum.Cam3;

            try
            {

                HOperatorSet.CountSeconds(out camEAProduct1.Image_StartDataProcessTime);
                CamEACornerProduct camEAProduct = camEAProduct1 as CamEACornerProduct;

                //计算图像坐标在世界坐标系下目标点
                double currentPointCamX = camEAProduct.coorEaPoints[0].CPointX;
                double currentPointCamY = camEAProduct.coorEaPoints[0].CPointY;

                if (false)
                {
                    ////非主相机，需要进行坐标转换
                    //CameraEnum mainCameraEnum = overHangCraft.BaseCamera;
                    //if (cameraEnum != mainCameraEnum)
                    //{
                    //    //获取当前相机到主相机到的变换矩阵
                    //    bool isSuccess = ImageAlgorithm.GetHomMat2dCompose(Project.Instance.cameraManager[mainCameraEnum].param_Checkerboard.HomMat2DInvert, Project.Instance.cameraManager[cameraEnum].param_Checkerboard.HomMat2D, out HTuple homMat2d);
                    //    if (isSuccess)
                    //    {
                    //        //计算图像坐标在主相机坐标系下目标点
                    //        isSuccess = ImageAlgorithm.GetAffinePoints2d(homMat2d, currentPointCamX, currentPointCamY, out double out_currentX, out double out_currentY);

                    //        if (isSuccess)
                    //        {
                    //            camEAProduct.coorEaPoints[0].AffinePointX = out_currentX;
                    //            camEAProduct.coorEaPoints[0].AffinePointY = out_currentY;
                    //            camEAProduct.coorEaPoints[0].MainCamCoordGetSucceed = true;
                    //        }
                    //        else
                    //        {
                    //            Log4netRecord.RunLog.Info($"获取当前相机到主相机坐标失败：{cameraEnum}->{mainCameraEnum}");
                    //        }
                    //    }
                    //    else
                    //    {
                    //        Log4netRecord.RunLog.Info($"获取当前相机到主相机变换矩阵失败：{cameraEnum}->{mainCameraEnum}");
                    //    }
                    //}
                    ////主相机，直接赋值
                    //else
                    //{
                    //    camEAProduct.coorEaPoints[0].AffinePointX = currentPointCamX;
                    //    camEAProduct.coorEaPoints[0].AffinePointY = currentPointCamY;
                    //    camEAProduct.coorEaPoints[0].MainCamCoordGetSucceed = true;
                    //}
                }



                //当前图像坐标映射到棋盘格              
                bool isSuccess = ImageAlgorithm.GetAffinePoints2d(Project.Instance.cameraManager[cameraEnum].param_PretargetingCalib.HomMat2D, currentPointCamX, currentPointCamY, out double out_currentX, out double out_currentY);

                //棋盘格映射到平台
                if (isSuccess)
                {
                    camEAProduct.coorEaPoints[0].AffinePointX = out_currentX;
                    camEAProduct.coorEaPoints[0].AffinePointY = out_currentY;
                    camEAProduct.coorEaPoints[0].MainCamCoordGetSucceed = true;

                }
                else
                {
                    Log4netRecord.RunLog.Info($"获取当前相机到棋盘格坐标失败：{cameraEnum}");
                }




                //添加基准边显示
                if (Project.Instance.cameraManager[cameraEnum].param_PretargetingCalib.BaseLines != null)
                {
                    foreach (var item in Project.Instance.cameraManager[cameraEnum].param_PretargetingCalib.BaseLines)
                    {
                        item.Color = HalconColor.blue;
                        LineInfo lineInfo1 = ImageAlgorithm.ExtendLin(item, 5000);
                        if (lineInfo1 != null)
                        {
                            camEAProduct.showHalconWindow?.m_lineShowInfos?.Add(lineInfo1);
                        }

                    }
                }




                HOperatorSet.CountSeconds(out camEAProduct1.Image_DataProcessTime);
                HOperatorSet.CountSeconds(out camEAProduct1.Image_EndTime);

                //计算时间
                BIZ_General.CalculateProcessingTime(camEAProduct1);

                bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam3 = camEAProduct;
                bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam3.CamFinish = true;

                Log4netRecord.RunLog.Info($"{cameraEnum}处理完成");

                //延时执行，等待相机综合数据处理完成
                DelayExecuteProcess delayExecuteProcess = () =>
                {
                    if (cam3_DealWithFinish)
                    {
                        return ProcessExecutionResult.Success;
                    }
                    return ProcessExecutionResult.ContinueExecute;
                };
                ProcessExecutionResult processExecutionResult = IntervalExecution.DelayExecute(DateTime.Now, 500, delayExecuteProcess);
                if (processExecutionResult == ProcessExecutionResult.Overtime)
                {
                    Log4netRecord.RunLog.Info($"相机综合数据处理超时：{cameraEnum}");
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }

        public void ProcessingData_Cam4(ProductBase camEAProduct1)
        {

            CameraEnum cameraEnum = CameraEnum.Cam4;

            try
            {

                HOperatorSet.CountSeconds(out camEAProduct1.Image_StartDataProcessTime);
                CamEACornerProduct camEAProduct = camEAProduct1 as CamEACornerProduct;

                //计算图像坐标在世界坐标系下目标点
                double currentPointCamX = camEAProduct.coorEaPoints[0].CPointX;
                double currentPointCamY = camEAProduct.coorEaPoints[0].CPointY;

                if (false)
                {
                    //非主相机，需要进行坐标转换
                    CameraEnum mainCameraEnum = overHangCraft.BaseCamera;
                    if (cameraEnum != mainCameraEnum)
                    {
                        //获取当前相机到主相机到的变换矩阵
                        bool isSuccess = ImageAlgorithm.GetHomMat2dCompose(Project.Instance.cameraManager[mainCameraEnum].param_Checkerboard.HomMat2DInvert, Project.Instance.cameraManager[cameraEnum].param_Checkerboard.HomMat2D, out HTuple homMat2d);
                        if (isSuccess)
                        {
                            //计算图像坐标在主相机坐标系下目标点
                            isSuccess = ImageAlgorithm.GetAffinePoints2d(homMat2d, currentPointCamX, currentPointCamY, out double out_currentX, out double out_currentY);

                            if (isSuccess)
                            {
                                camEAProduct.coorEaPoints[0].AffinePointX = out_currentX;
                                camEAProduct.coorEaPoints[0].AffinePointY = out_currentY;
                                camEAProduct.coorEaPoints[0].MainCamCoordGetSucceed = true;
                            }
                            else
                            {
                                Log4netRecord.RunLog.Info($"获取当前相机到主相机坐标失败：{cameraEnum}->{mainCameraEnum}");
                            }
                        }
                        else
                        {
                            Log4netRecord.RunLog.Info($"获取当前相机到主相机变换矩阵失败：{cameraEnum}->{mainCameraEnum}");
                        }
                    }
                    //主相机，直接赋值
                    else
                    {
                        camEAProduct.coorEaPoints[0].AffinePointX = currentPointCamX;
                        camEAProduct.coorEaPoints[0].AffinePointY = currentPointCamY;
                        camEAProduct.coorEaPoints[0].MainCamCoordGetSucceed = true;
                    }
                }


                //添加基准边显示
                if (Project.Instance.cameraManager[cameraEnum].param_PretargetingCalib.BaseLines != null)
                {
                    foreach (var item in Project.Instance.cameraManager[cameraEnum].param_PretargetingCalib.BaseLines)
                    {
                        item.Color = HalconColor.blue;
                        LineInfo lineInfo1 = ImageAlgorithm.ExtendLin(item, 5000);
                        if (lineInfo1 != null)
                        {
                            camEAProduct.showHalconWindow?.m_lineShowInfos?.Add(lineInfo1);
                        }

                    }
                }




                HOperatorSet.CountSeconds(out camEAProduct1.Image_DataProcessTime);
                HOperatorSet.CountSeconds(out camEAProduct1.Image_EndTime);
                //计算时间
                BIZ_General.CalculateProcessingTime(camEAProduct1);

                bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam4 = camEAProduct;
                bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam4.CamFinish = true;

                Log4netRecord.RunLog.Info($"{cameraEnum}处理完成");

                //延时执行，等待相机综合数据处理完成
                DelayExecuteProcess delayExecuteProcess = () =>
                {
                    if (cam4_DealWithFinish)
                    {
                        return ProcessExecutionResult.Success;
                    }
                    return ProcessExecutionResult.ContinueExecute;
                };
                ProcessExecutionResult processExecutionResult = IntervalExecution.DelayExecute(DateTime.Now, 500, delayExecuteProcess);
                if (processExecutionResult == ProcessExecutionResult.Overtime)
                {
                    Log4netRecord.RunLog.Info($"相机综合数据处理超时：{cameraEnum}");
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }



        public void IntegratedData(double PlatformX, double PlatformY, double PlatformR, out double coord_x, out double coord_y, out double coord_angle, out bool result, out double pointX, out double pointY,
         out double BaseLineX1, out double BaseLineY1, out double BaseLineX2, out double BaseLineY2,
         out bool coordResult_x, out bool coordResult_y, out bool coordResult_angle, out double ImgInitPointX, out double ImgInitPointY, out double ImgInitPointAngle,
         out double UpperLimitX, out double LowerlimitX, out double UpperLimitY, out double LowerlimitY, out double UpperLimitAngle, out double LowerlimitAngle,
         out double offsetValueX, out double offsetValueY, out double offsetValueR)
        {
            pointX = 0;
            pointY = 0;
            BaseLineX1 = 0;
            BaseLineY1 = 0;
            BaseLineX2 = 0;
            BaseLineY2 = 0;
            coord_x = -999;
            coord_y = -999;
            coord_angle = -999;
            result = false;
            coordResult_x = false;
            coordResult_y = false;
            coordResult_angle = false;

            ImgInitPointX = -999;
            ImgInitPointY = -999;
            ImgInitPointAngle = -999;

            UpperLimitX = -999;
            LowerlimitX = -999;
            UpperLimitY = -999;
            LowerlimitY = -999;
            UpperLimitAngle = -999;
            LowerlimitAngle = -999;

            offsetValueX = -999;
            offsetValueY = -999;
            offsetValueR = -999;


            try
            {
                //================================================================
                //【1】处理数据、计算X、Y、角度           
                //使用对角相机计算
                if (true)
                {
                    //抓边未抓到退出检测
                    if (!bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam1.showHalconWindow.m_lineShowInfos[0].IsLineSuccessfull ||
                      !bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam1.showHalconWindow.m_lineShowInfos[1].IsLineSuccessfull)
                    {
                        return;
                    }

                    if (!bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam2.showHalconWindow.m_lineShowInfos[0].IsLineSuccessfull ||
                    !bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam2.showHalconWindow.m_lineShowInfos[1].IsLineSuccessfull)
                    {
                        return;
                    }


                    if (!bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam3.showHalconWindow.m_lineShowInfos[0].IsLineSuccessfull ||
                    !bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam3.showHalconWindow.m_lineShowInfos[1].IsLineSuccessfull)
                    {
                        return;
                    }

                    if (!bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam4.showHalconWindow.m_lineShowInfos[0].IsLineSuccessfull ||
                      !bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam4.showHalconWindow.m_lineShowInfos[1].IsLineSuccessfull ||
                      !bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam4.showHalconWindow.m_lineShowInfos[2].IsLineSuccessfull ||
                      !bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam4.showHalconWindow.m_lineShowInfos[3].IsLineSuccessfull)
                    {
                        return;
                    }

                    CameraEnum mainCameraEnum = overHangCraft.BaseCamera;



                    //获取棋盘格角点,求对角线中心点
                    double leftUpRow = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam1.coorEaPoints[0].AffinePointY;
                    double leftUpCol = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam1.coorEaPoints[0].AffinePointX;
                    double rightUpRow = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam2.coorEaPoints[0].AffinePointY;
                    double rightUpCol = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam2.coorEaPoints[0].AffinePointX;
                    double leftDownRow = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam3.coorEaPoints[0].AffinePointY;
                    double leftDownCol = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam3.coorEaPoints[0].AffinePointX;
                    double rightDownRow = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam4.coorEaPoints[0].AffinePointY;
                    double rightDownCol = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam4.coorEaPoints[0].AffinePointX;
                    pointX = (leftDownCol + rightUpCol) / 2;
                    pointY = (leftDownRow + rightUpRow) / 2;
                    Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.BasePointXTemp = pointX;
                    Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.BasePointYTemp = pointY;


                    BaseLineX1 = leftDownCol;
                    BaseLineY1 = leftDownRow;

                    BaseLineX2 = rightUpCol;
                    BaseLineY2 = rightUpRow;



                    Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.BaseLineBeginRowTemp = BaseLineY1;
                    Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.BaseLineBeginColTemp = BaseLineX1;
                    Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.BaseLineEndRowTemp = BaseLineY2;
                    Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.BaseLineEndColTemp = BaseLineX2;

                    bool isSuccess = ImageAlgorithm.Calculate_LevelLine_AcuteAngle(
                        Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.BaseLineBeginRow,
                        Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.BaseLineBeginCol,
                        Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.BaseLineEndRow,
                        Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.BaseLineEndCol,
                        BaseLineY1, BaseLineX1, BaseLineY2, BaseLineX2, out double lineAngle);



                    if (!isSuccess)
                    {
                        Log4netRecord.RunLog.Info("计算两条线的角度失败");
                        return;
                    }


                    double centreColX = pointX;
                    double centreRowY = pointY;
                    //第一次拍照或者无拍照信号
                    if (bIZ_Variable.PhotoSignal >= 0 && bIZ_Variable.PhotoSignal < 10)
                    {
                        //计算一个带旋转的变换矩阵
                        isSuccess = ImageAlgorithm.Calculate_HomMat2dRotate(lineAngle, Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.CenterOfRotationX,
                                      Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.CenterOfRotationY, out HTuple homMat2dRotate);
                        //计算一个带旋转的变换矩阵
                        //isSuccess = ImageAlgorithm.Calculate_HomMat2dRotate(lineAngle, Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.CenterOfRotationX + x,
                        //              Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.CenterOfRotationY + y, out HTuple homMat2dRotate);

                        if (!isSuccess)
                        {
                            Log4netRecord.RunLog.Info("计算旋转矩阵失败");
                            return;
                        }

                        //计算旋转后的中心点坐标
                        isSuccess = ImageAlgorithm.GetAffinePoints2d(homMat2dRotate, pointX, pointY, out centreColX, out centreRowY);

                        if (!isSuccess)
                        {
                            Log4netRecord.RunLog.Info("计算旋转后的中心点坐标失败");
                            return;
                        }


                        UpperLimitX = bIZ_Variable.XBiasMax;
                        LowerlimitX = bIZ_Variable.XBiasMin;
                        UpperLimitY = bIZ_Variable.YBiasMax;
                        LowerlimitY = bIZ_Variable.YBiasMin;
                        UpperLimitAngle = bIZ_Variable.AngleBiasMax;
                        LowerlimitAngle = bIZ_Variable.AngleBiasMin;

                    }

                    //第二次拍照
                    else if (bIZ_Variable.PhotoSignal > 10 && bIZ_Variable.PhotoSignal < 20)
                    {

                        //计算一个带旋转的变换矩阵
                        isSuccess = ImageAlgorithm.Calculate_HomMat2dRotate(lineAngle, Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.CenterOfRotationX + PlatformX,
                                      Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.CenterOfRotationY + PlatformY, out HTuple homMat2dRotate);



                        if (!isSuccess)
                        {
                            Log4netRecord.RunLog.Info("计算旋转矩阵失败");
                            return;
                        }

                        //计算旋转后的中心点坐标
                        isSuccess = ImageAlgorithm.GetAffinePoints2d(homMat2dRotate, pointX, pointY, out centreColX, out centreRowY);

                        if (!isSuccess)
                        {
                            Log4netRecord.RunLog.Info("计算旋转后的中心点坐标失败");
                            return;
                        }

                        Log4netRecord.RunLog.Info($"二次纠偏平台X:{PlatformX},Y:{PlatformY},R:{PlatformR}，" +
                            $"旋转中心X:{Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.CenterOfRotationX}," +
                            $"旋转中心Y:{Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.CenterOfRotationY}");


                        UpperLimitX = overHangCraft.XBiasMax;
                        LowerlimitX = overHangCraft.XBiasMin;
                        UpperLimitY = overHangCraft.YBiasMax;
                        LowerlimitY = overHangCraft.YBiasMin;
                        UpperLimitAngle = overHangCraft.AngleBiasMax;
                        LowerlimitAngle = overHangCraft.AngleBiasMin;
                    }




                    //计算平台坐标
                    isSuccess = ImageAlgorithm.GetAffinePoints2d(Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.HomMat2DPlatform,
                        centreColX, centreRowY, out double NewCentreColX, out double NewCentreRowY);
                    if (!isSuccess)
                    {
                        Log4netRecord.RunLog.Info("计算平台矩阵的中心点坐标失败");
                        return;
                    }

                    isSuccess = ImageAlgorithm.GetAffinePoints2d(Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.HomMat2DPlatform,
                       Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.BasePointX,
                       Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.BasePointY,
                       out double NewBasePointX, out double NewBasePointY);
                    if (!isSuccess)
                    {
                        Log4netRecord.RunLog.Info("计算平台矩阵的基准点坐标失败");
                        return;
                    }

                    centreColX = NewCentreColX - NewBasePointX;
                    centreRowY = NewCentreRowY - NewBasePointY;




                    //是否需要取反
                    if (Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.NotBCValueX)
                    {
                        centreColX = -centreColX;
                    }
                    if (Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.NotBCValueY)
                    {
                        centreRowY = -centreRowY;
                    }
                    if (Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.NotBCValueR)
                    {
                        lineAngle = -lineAngle;
                    }

                    ImgInitPointX = centreColX;
                    ImgInitPointY = centreRowY;
                    ImgInitPointAngle = lineAngle;
                    //centreRowY = centreRowY + (lineAngle * 0.11);
                    //================================================================
                    //【2】工艺判断

                    //1.A下单片
                    //2.A上单片
                    //3.A极片
                    //4.B极片
                    //11.A下单片第二次拍照
                    //12.A上单片第二次拍照
                    //13.A极片第二次拍照
                    //14.B极片第二次拍照

                    //偏移信息记录


                    if (bIZ_Variable.PhotoSignal == 1 || bIZ_Variable.PhotoSignal == 11)//下单片
                    {
                        //下电芯
                        if (bIZ_Variable.UpOrDownCell == 0 || bIZ_Variable.UpOrDownCell == 1)
                        {
                            offsetValueY = Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineDownBCOffectValue.Row;
                            offsetValueX = Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineDownBCOffectValue.Col;
                            offsetValueR = Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineDownBCOffectValue.Angle;


                        }
                        //上电芯
                        else if (bIZ_Variable.UpOrDownCell == 2)
                        {
                            offsetValueY = Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineDownBCOffectValue_Up.Row;
                            offsetValueX = Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineDownBCOffectValue_Up.Col;
                            offsetValueR = Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineDownBCOffectValue_Up.Angle;

                        }

                    }
                    else if (bIZ_Variable.PhotoSignal == 2 || bIZ_Variable.PhotoSignal == 12)//上单片
                    {
                        //下电芯
                        if (bIZ_Variable.UpOrDownCell == 0 || bIZ_Variable.UpOrDownCell == 1)
                        {
                            offsetValueY = Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineUpBCOffectValue.Row;
                            offsetValueX = Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineUpBCOffectValue.Col;
                            offsetValueR = Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineUpBCOffectValue.Angle;

                        }
                        //上电芯
                        else if (bIZ_Variable.UpOrDownCell == 2)
                        {
                            offsetValueY = Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineUpBCOffectValue_Up.Row;
                            offsetValueX = Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineUpBCOffectValue_Up.Col;
                            offsetValueR = Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineUpBCOffectValue_Up.Angle;

                        }

                    }
                    else if (bIZ_Variable.PhotoSignal == 3 || bIZ_Variable.PhotoSignal == 13 ||
                             bIZ_Variable.PhotoSignal == 4 || bIZ_Variable.PhotoSignal == 14)//极片
                    {
                        offsetValueY = Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineHandBCOffectValue.Row;
                        offsetValueX = Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineHandBCOffectValue.Col;
                        offsetValueR = Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineHandBCOffectValue.Angle;

                    }
                    else if (Project.Instance.Is_simulation)
                    {
                        offsetValueY = Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineHandBCOffectValue.Row;
                        offsetValueX = Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineHandBCOffectValue.Col;
                        offsetValueR = Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineHandBCOffectValue.Angle;
                    }



                    coord_y = centreRowY + Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineHandBCValue.Row + offsetValueY;
                    coord_x = centreColX + Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineHandBCValue.Col + offsetValueX;
                    coord_angle = lineAngle + Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineHandBCValue.Angle + offsetValueR;
                    coord_y = Math.Round(coord_y, 3);
                    coord_x = Math.Round(coord_x, 3);
                    coord_angle = Math.Round(coord_angle, 3);


                    if (coord_y >= LowerlimitY &&
                        coord_y <= UpperLimitY)
                    {
                        coordResult_y = true;
                    }
                    if (coord_x >= LowerlimitX &&
                        coord_x <= UpperLimitX)
                    {
                        coordResult_x = true;
                    }
                    if (coord_angle >= LowerlimitAngle &&
                        coord_angle <= UpperLimitAngle)
                    {
                        coordResult_angle = true;
                    }


                    result = coordResult_y && coordResult_x && coordResult_angle;
                }

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }

        }



        /// <summary>
        /// 综合数据
        /// </summary>
        //public void IntegratedData(out double coord_x, out double coord_y, out double coord_angle, out bool result, out double pointX, out double pointY,
        //    out double BaseLineX1, out double BaseLineY1, out double BaseLineX2, out double BaseLineY2,
        //    out bool coordResult_x, out bool coordResult_y, out bool coordResult_angle,out double ImgInitPointX,out double ImgInitPointY,out double ImgInitPointAngle,
        //    out double UpperLimitX,out double LowerlimitX, out double UpperLimitY, out double LowerlimitY, out double UpperLimitAngle, out double LowerlimitAngle,
        //    out double offsetValueX, out double offsetValueY, out double offsetValueR)
        //{
        //    pointX = 0;
        //    pointY = 0;
        //    BaseLineX1 = 0;
        //    BaseLineY1 = 0;
        //    BaseLineX2 = 0;
        //    BaseLineY2 = 0;
        //    coord_x = -999;
        //    coord_y = -999;
        //    coord_angle = -999;
        //    result = false;
        //    coordResult_x = false;
        //    coordResult_y = false;
        //    coordResult_angle = false;

        //    ImgInitPointX = -999;
        //    ImgInitPointY = -999;
        //    ImgInitPointAngle = -999;

        //    UpperLimitX = -999;
        //    LowerlimitX = -999;
        //    UpperLimitY = -999;
        //    LowerlimitY = -999;
        //    UpperLimitAngle = -999;
        //    LowerlimitAngle = -999;

        //    offsetValueX = -999;
        //    offsetValueY = -999;
        //    offsetValueR = -999;


        //    try
        //    {
        //        //================================================================
        //        //【1】处理数据、计算X、Y、角度           
        //        //使用对角相机计算
        //        if (true)
        //        {
        //            //抓边未抓到退出检测
        //            if (!bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam1.showHalconWindow.m_lineShowInfos[0].IsLineSuccessfull ||
        //              !bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam1.showHalconWindow.m_lineShowInfos[1].IsLineSuccessfull)
        //            {
        //                return;
        //            }

        //            if (!bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam2.showHalconWindow.m_lineShowInfos[0].IsLineSuccessfull ||
        //            !bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam2.showHalconWindow.m_lineShowInfos[1].IsLineSuccessfull)
        //            {
        //                return;
        //            }


        //            if (!bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam3.showHalconWindow.m_lineShowInfos[0].IsLineSuccessfull ||
        //            !bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam3.showHalconWindow.m_lineShowInfos[1].IsLineSuccessfull)
        //            {
        //                return;
        //            }

        //            if (!bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam4.showHalconWindow.m_lineShowInfos[0].IsLineSuccessfull ||
        //              !bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam4.showHalconWindow.m_lineShowInfos[1].IsLineSuccessfull ||
        //              !bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam4.showHalconWindow.m_lineShowInfos[2].IsLineSuccessfull ||
        //              !bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam4.showHalconWindow.m_lineShowInfos[3].IsLineSuccessfull)
        //            {
        //                return;
        //            }

        //            CameraEnum mainCameraEnum = overHangCraft.BaseCamera;



        //            //获取棋盘格角点,求对角线中心点
        //            double leftUpRow = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam1.coorEaPoints[0].AffinePointY;
        //            double leftUpCol = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam1.coorEaPoints[0].AffinePointX;
        //            double rightUpRow = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam2.coorEaPoints[0].AffinePointY;
        //            double rightUpCol = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam2.coorEaPoints[0].AffinePointX;
        //            double leftDownRow = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam3.coorEaPoints[0].AffinePointY;
        //            double leftDownCol = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam3.coorEaPoints[0].AffinePointX;
        //            double rightDownRow = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam4.coorEaPoints[0].AffinePointY;
        //            double rightDownCol = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam4.coorEaPoints[0].AffinePointX;
        //            pointX = (leftDownCol + rightUpCol) / 2;
        //            pointY = (leftDownRow + rightUpRow) / 2;
        //            Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.BasePointXTemp = pointX;
        //            Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.BasePointYTemp = pointY;


        //            BaseLineX1 = leftDownCol;
        //            BaseLineY1 = leftDownRow;

        //            BaseLineX2 = rightUpCol;
        //            BaseLineY2 = rightUpRow;



        //            Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.BaseLineBeginRowTemp = BaseLineY1;
        //            Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.BaseLineBeginColTemp = BaseLineX1;
        //            Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.BaseLineEndRowTemp = BaseLineY2;
        //            Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.BaseLineEndColTemp = BaseLineX2;

        //            bool isSuccess = ImageAlgorithm.Calculate_LevelLine_AcuteAngle(
        //                Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.BaseLineBeginRow,
        //                Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.BaseLineBeginCol,
        //                Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.BaseLineEndRow,
        //                Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.BaseLineEndCol,
        //                BaseLineY1, BaseLineX1, BaseLineY2, BaseLineX2, out double lineAngle);



        //            if (!isSuccess)
        //            {
        //                Log4netRecord.RunLog.Info("计算两条线的角度失败");
        //                return;
        //            }


        //            double centreColX = pointX;
        //            double centreRowY = pointY;
        //            //第一次拍照或者无拍照信号
        //            if (bIZ_Variable.PhotoSignal >= 0 && bIZ_Variable.PhotoSignal < 10)
        //            {
        //                //计算一个带旋转的变换矩阵
        //                isSuccess = ImageAlgorithm.Calculate_HomMat2dRotate(lineAngle, Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.CenterOfRotationX,
        //                              Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.CenterOfRotationY, out HTuple homMat2dRotate);


        //                if (!isSuccess)
        //                {
        //                    Log4netRecord.RunLog.Info("计算旋转矩阵失败");
        //                    return;
        //                }

        //                //计算旋转后的中心点坐标
        //                isSuccess = ImageAlgorithm.GetAffinePoints2d(homMat2dRotate, pointX, pointY, out centreColX, out centreRowY);

        //                if (!isSuccess)
        //                {
        //                    Log4netRecord.RunLog.Info("计算旋转后的中心点坐标失败");
        //                    return;
        //                }


        //                UpperLimitX = bIZ_Variable.XBiasMax;
        //                LowerlimitX = bIZ_Variable.XBiasMin;
        //                UpperLimitY = bIZ_Variable.YBiasMax;
        //                LowerlimitY = bIZ_Variable.YBiasMin;
        //                UpperLimitAngle = bIZ_Variable.AngleBiasMax;
        //                LowerlimitAngle = bIZ_Variable.AngleBiasMin;

        //            }

        //            //第二次拍照
        //            else if (bIZ_Variable.PhotoSignal > 10 && bIZ_Variable.PhotoSignal < 20)
        //            {

        //                //计算一个带旋转的变换矩阵
        //                isSuccess = ImageAlgorithm.Calculate_HomMat2dRotate(lineAngle, Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.CenterOfRotationX,
        //                              Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.CenterOfRotationY, out HTuple homMat2dRotate);


        //                if (!isSuccess)
        //                {
        //                    Log4netRecord.RunLog.Info("计算旋转矩阵失败");
        //                    return;
        //                }

        //                //计算旋转后的中心点坐标
        //                isSuccess = ImageAlgorithm.GetAffinePoints2d(homMat2dRotate, pointX, pointY, out centreColX, out centreRowY);

        //                if (!isSuccess)
        //                {
        //                    Log4netRecord.RunLog.Info("计算旋转后的中心点坐标失败");
        //                    return;
        //                }


        //                UpperLimitX = overHangCraft.XBiasMax;
        //                LowerlimitX = overHangCraft.XBiasMin;
        //                UpperLimitY = overHangCraft.YBiasMax;
        //                LowerlimitY = overHangCraft.YBiasMin;
        //                UpperLimitAngle = overHangCraft.AngleBiasMax;
        //                LowerlimitAngle = overHangCraft.AngleBiasMin;
        //            }




        //            //计算平台坐标
        //            isSuccess = ImageAlgorithm.GetAffinePoints2d(Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.HomMat2DPlatform,
        //                centreColX, centreRowY, out double NewCentreColX, out double NewCentreRowY);
        //            if (!isSuccess)
        //            {
        //                Log4netRecord.RunLog.Info("计算平台矩阵的中心点坐标失败");
        //                return;
        //            }

        //            isSuccess = ImageAlgorithm.GetAffinePoints2d(Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.HomMat2DPlatform,
        //               Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.BasePointX,
        //               Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.BasePointY,
        //               out double NewBasePointX, out double NewBasePointY);
        //            if (!isSuccess)
        //            {
        //                Log4netRecord.RunLog.Info("计算平台矩阵的基准点坐标失败");
        //                return;
        //            }

        //            centreColX = NewCentreColX - NewBasePointX;
        //            centreRowY = NewCentreRowY - NewBasePointY;




        //            //是否需要取反
        //            if (Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.NotBCValueX)
        //            {
        //                centreColX = -centreColX;
        //            }
        //            if (Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.NotBCValueY)
        //            {
        //                centreRowY = -centreRowY;
        //            }
        //            if (Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.NotBCValueR)
        //            {
        //                lineAngle = -lineAngle;
        //            }

        //            ImgInitPointX = centreColX;
        //            ImgInitPointY = centreRowY;
        //            ImgInitPointAngle = lineAngle;
        //            //centreRowY = centreRowY + (lineAngle * 0.11);
        //            //================================================================
        //            //【2】工艺判断

        //            //1.A下单片
        //            //2.A上单片
        //            //3.A极片
        //            //4.B极片
        //            //11.A下单片第二次拍照
        //            //12.A上单片第二次拍照
        //            //13.A极片第二次拍照
        //            //14.B极片第二次拍照

        //            //偏移信息记录


        //            if (bIZ_Variable.PhotoSignal == 1 || bIZ_Variable.PhotoSignal == 11)//下单片
        //            {
        //                //下电芯
        //                if (bIZ_Variable.UpOrDownCell == 0 || bIZ_Variable.UpOrDownCell == 1)
        //                {
        //                    offsetValueY = Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineDownBCOffectValue.Row;
        //                    offsetValueX = Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineDownBCOffectValue.Col;
        //                    offsetValueR = Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineDownBCOffectValue.Angle;


        //                }
        //                //上电芯
        //                else if (bIZ_Variable.UpOrDownCell == 2)
        //                {
        //                    offsetValueY = Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineDownBCOffectValue_Up.Row;
        //                    offsetValueX = Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineDownBCOffectValue_Up.Col;
        //                    offsetValueR = Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineDownBCOffectValue_Up.Angle;

        //                }

        //            }
        //            else if (bIZ_Variable.PhotoSignal == 2 || bIZ_Variable.PhotoSignal == 12)//上单片
        //            {
        //                //下电芯
        //                if (bIZ_Variable.UpOrDownCell == 0 || bIZ_Variable.UpOrDownCell == 1)
        //                {
        //                    offsetValueY = Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineUpBCOffectValue.Row;
        //                    offsetValueX = Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineUpBCOffectValue.Col;
        //                    offsetValueR = Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineUpBCOffectValue.Angle;

        //                }
        //                //上电芯
        //                else if (bIZ_Variable.UpOrDownCell == 2)
        //                {
        //                    offsetValueY = Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineUpBCOffectValue_Up.Row;
        //                    offsetValueX = Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineUpBCOffectValue_Up.Col;
        //                    offsetValueR = Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineUpBCOffectValue_Up.Angle;

        //                }

        //            }
        //            else if (bIZ_Variable.PhotoSignal == 3 || bIZ_Variable.PhotoSignal == 13 ||
        //                     bIZ_Variable.PhotoSignal == 4 || bIZ_Variable.PhotoSignal == 14)//极片
        //            {
        //                offsetValueY = Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineHandBCOffectValue.Row;
        //                offsetValueX = Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineHandBCOffectValue.Col;
        //                offsetValueR = Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineHandBCOffectValue.Angle;

        //            }
        //            else if (Project.Instance.Is_simulation)
        //            {
        //                offsetValueY = Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineHandBCOffectValue.Row;
        //                offsetValueX = Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineHandBCOffectValue.Col;
        //                offsetValueR = Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineHandBCOffectValue.Angle;
        //            }



        //            coord_y = centreRowY + Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineHandBCValue.Row + offsetValueY;
        //            coord_x = centreColX + Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineHandBCValue.Col + offsetValueX;
        //            coord_angle = lineAngle + Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineHandBCValue.Angle + offsetValueR;
        //            coord_y = Math.Round(coord_y, 3);
        //            coord_x = Math.Round(coord_x, 3);
        //            coord_angle = Math.Round(coord_angle, 3);


        //            if (coord_y >= LowerlimitY &&
        //                coord_y <= UpperLimitY)
        //            {
        //                coordResult_y = true;
        //            }
        //            if (coord_x >= LowerlimitX &&
        //                coord_x <= UpperLimitX)
        //            {
        //                coordResult_x = true;
        //            }
        //            if (coord_angle >= LowerlimitAngle &&
        //                coord_angle <= UpperLimitAngle)
        //            {
        //                coordResult_angle = true;
        //            }
        //            if (bIZ_Variable.PhotoSignal >= 0 && bIZ_Variable.PhotoSignal < 10)
        //            {
        //                x=coord_x;
        //                y=coord_y;

        //            }

        //                result = coordResult_y && coordResult_x && coordResult_angle;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Log4netRecord.ErrorLog.Error(ex);
        //    }

        //}



        ///// <summary>
        ///// 综合数据
        ///// </summary>
        //public void IntegratedData(out double coord_x, out double coord_y, out double coord_angle, out bool result, out double pointX, out double pointY,
        //    out double BaseLineX1, out double BaseLineY1, out double BaseLineX2, out double BaseLineY2,
        //    out bool coordResult_x, out bool coordResult_y, out bool coordResult_angle)
        //{
        //    pointX = 0;
        //    pointY = 0;
        //    BaseLineX1 = 0;
        //    BaseLineY1 = 0;
        //    BaseLineX2 = 0;
        //    BaseLineY2 = 0;
        //    coord_x = -999;
        //    coord_y = -999;
        //    coord_angle = -999;
        //    result = false;
        //    coordResult_x = false;
        //    coordResult_y = false;
        //    coordResult_angle = false;
        //    try
        //    {
        //        //================================================================
        //        //【1】处理数据、计算X、Y、角度           
        //        //使用对角相机计算
        //        if (true)
        //        {
        //            //抓边未抓到退出检测
        //            if (!bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam1.showHalconWindow.m_lineShowInfos[0].IsLineSuccessfull ||
        //              !bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam1.showHalconWindow.m_lineShowInfos[1].IsLineSuccessfull)
        //            {
        //                return;
        //            }

        //            if (!bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam2.showHalconWindow.m_lineShowInfos[0].IsLineSuccessfull ||
        //            !bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam2.showHalconWindow.m_lineShowInfos[1].IsLineSuccessfull)
        //            {
        //                return;
        //            }


        //            if (!bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam3.showHalconWindow.m_lineShowInfos[0].IsLineSuccessfull ||
        //            !bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam3.showHalconWindow.m_lineShowInfos[1].IsLineSuccessfull)
        //            {
        //                return;
        //            }

        //            if (!bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam4.showHalconWindow.m_lineShowInfos[0].IsLineSuccessfull ||
        //              !bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam4.showHalconWindow.m_lineShowInfos[1].IsLineSuccessfull ||
        //              !bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam4.showHalconWindow.m_lineShowInfos[2].IsLineSuccessfull ||
        //              !bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam4.showHalconWindow.m_lineShowInfos[3].IsLineSuccessfull)
        //            {
        //                return;
        //            }

        //            CameraEnum mainCameraEnum = overHangCraft.BaseCamera;


        //            //单相机交点测试不准
        //            if (false)
        //            {
        //                //double line1_BeginRow = 0;
        //                //double line1_BeginCol = 0;
        //                //double line1_EndRow = 0;
        //                //double line1_EndCol = 0;

        //                //line1_BeginRow = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam3.showHalconWindow.m_lineShowInfos[0].RowBegin;
        //                //line1_BeginCol = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam3.showHalconWindow.m_lineShowInfos[0].ColBegin;
        //                //line1_EndRow = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam3.showHalconWindow.m_lineShowInfos[0].RowEnd;
        //                //line1_EndCol = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam3.showHalconWindow.m_lineShowInfos[0].ColEnd;


        //                //line2_BeginRow = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam3.showHalconWindow.m_lineShowInfos[1].RowBegin;
        //                //line2_BeginCol = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam3.showHalconWindow.m_lineShowInfos[1].ColBegin;
        //                //line2_EndRow = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam3.showHalconWindow.m_lineShowInfos[1].RowEnd;
        //                //line2_EndCol = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam3.showHalconWindow.m_lineShowInfos[1].ColEnd;

        //                ////=========================
        //                ////获取图像坐标及角度
        //                ////计算两条线的交点
        //                //HOperatorSet.IntersectionLines(line1_BeginRow, line1_BeginCol, line1_EndRow, line1_EndCol,
        //                //                       line2_BeginRow, line2_BeginCol, line2_EndRow, line2_EndCol,
        //                //                       out HTuple pointY, out HTuple pointX, out HTuple isOverlapping);
        //            }

        //            //获取棋盘格角点,求对角线中心点
        //            double leftUpRow = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam1.coorEaPoints[0].AffinePointY;
        //            double leftUpCol = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam1.coorEaPoints[0].AffinePointX;
        //            double rightUpRow = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam2.coorEaPoints[0].AffinePointY;
        //            double rightUpCol = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam2.coorEaPoints[0].AffinePointX;
        //            double leftDownRow = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam3.coorEaPoints[0].AffinePointY;
        //            double leftDownCol = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam3.coorEaPoints[0].AffinePointX;
        //            double rightDownRow = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam4.coorEaPoints[0].AffinePointY;
        //            double rightDownCol = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam4.coorEaPoints[0].AffinePointX;
        //            pointX = (leftDownCol + rightUpCol) / 2;
        //            pointY = (leftDownRow + rightUpRow) / 2;
        //            Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.BasePointXTemp = pointX;
        //            Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.BasePointYTemp = pointY;


        //            //bool isSuccess = ImageAlgorithm.Calculate_Angle_ll(Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.CenterOfRotationY,
        //            //                                 Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.CenterOfRotationX,
        //            //                                 Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.BasePointY,
        //            //                                 Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.BasePointX,
        //            //                                 pointY, pointX, out HTuple angle);

        //            //double lineAngle = angle.D;
        //            //计算两条线的角度
        //            //水平线
        //            //double line2_BeginRow = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam3.showHalconWindow.m_lineShowInfos[1].RowBegin;
        //            //double line2_BeginCol = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam3.showHalconWindow.m_lineShowInfos[1].ColBegin;
        //            //double line2_EndRow = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam3.showHalconWindow.m_lineShowInfos[1].RowEnd;
        //            //double line2_EndCol = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam3.showHalconWindow.m_lineShowInfos[1].ColEnd;


        //            //double line2_BeginRow = leftDownRow;
        //            //double line2_BeginCol = leftDownCol;
        //            //double line2_EndRow = rightUpRow;
        //            //double line2_EndCol = rightUpCol;

        //            BaseLineX1 = leftDownCol;
        //            BaseLineY1 = leftDownRow;
        //            //isSuccess = ImageAlgorithm.GetAffinePoints2d(Project.Instance.cameraManager[CameraEnum.Cam3].param_PretargetingCalib.HomMat2D, line2_EndCol, line2_EndRow,
        //            //  out double out_currentX2, out double out_currentY2);
        //            BaseLineX2 = rightUpCol;
        //            BaseLineY2 = rightUpRow;


        //            //当前图像坐标映射到棋盘格              
        //            //bool isSuccess = ImageAlgorithm.GetAffinePoints2d(Project.Instance.cameraManager[CameraEnum.Cam3].param_PretargetingCalib.HomMat2D, line2_BeginCol, line2_BeginRow,
        //            //    out double out_currentX1, out double out_currentY1);
        //            //BaseLineX1 = out_currentX1;
        //            //BaseLineY1 = out_currentY1;
        //            //isSuccess = ImageAlgorithm.GetAffinePoints2d(Project.Instance.cameraManager[CameraEnum.Cam3].param_PretargetingCalib.HomMat2D, line2_EndCol, line2_EndRow,
        //            //  out double out_currentX2, out double out_currentY2);
        //            //BaseLineX2 = out_currentX2;
        //            //BaseLineY2 = out_currentY2;


        //            Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.BaseLineBeginRowTemp = BaseLineY1;
        //            Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.BaseLineBeginColTemp = BaseLineX1;
        //            Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.BaseLineEndRowTemp = BaseLineY2;
        //            Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.BaseLineEndColTemp = BaseLineX2;

        //            bool isSuccess = ImageAlgorithm.Calculate_LevelLine_AcuteAngle(
        //                Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.BaseLineBeginRow,
        //                Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.BaseLineBeginCol,
        //                Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.BaseLineEndRow,
        //                Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.BaseLineEndCol,
        //                BaseLineY1, BaseLineX1, BaseLineY2, BaseLineX2, out double lineAngle);



        //            if (!isSuccess)
        //            {
        //                Log4netRecord.RunLog.Info("计算两条线的角度失败");
        //                return;
        //            }

        //            ////=================================================
        //            ////计算主相机坐标系到机械手平台坐标系的坐标
        //            //isSuccess = ImageAlgorithm.GetAffinePoints2d(Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.HomMat2D, pointX, pointY, out double leftDownColX, out double leftDownRowY);
        //            //if (!isSuccess)
        //            //{
        //            //    Log4netRecord.RunLog.Info("获取主相机坐标系到机械手平台坐标系的坐标失败");
        //            //    return;
        //            //}

        //            double centreColX = pointX;
        //            double centreRowY = pointY;
        //            //第一次拍照或者无拍照信号
        //            if (bIZ_Variable.PhotoSignal >= 0 && bIZ_Variable.PhotoSignal < 10)
        //            {
        //                //计算一个带旋转的变换矩阵
        //                isSuccess = ImageAlgorithm.Calculate_HomMat2dRotate(lineAngle, Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.CenterOfRotationX,
        //                              Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.CenterOfRotationY, out HTuple homMat2dRotate);


        //                if (!isSuccess)
        //                {
        //                    Log4netRecord.RunLog.Info("计算旋转矩阵失败");
        //                    return;
        //                }

        //                //计算旋转后的中心点坐标
        //                isSuccess = ImageAlgorithm.GetAffinePoints2d(homMat2dRotate, pointX, pointY, out centreColX, out centreRowY);

        //                if (!isSuccess)
        //                {
        //                    Log4netRecord.RunLog.Info("计算旋转后的中心点坐标失败");
        //                    return;
        //                }
        //                //HTuple rad = lineAngle;
        //                //HOperatorSet.TupleRad(lineAngle, out HTuple rad);
        //                //double Y = (pointY - centreRowY) * Math.Cos(rad.D) + (pointX - centreColX) * Math.Sin(rad.D) + centreRowY;

        //                //double X = (pointX - centreColX) * Math.Cos(rad.D) - (pointY - centreRowY) * Math.Sin(rad.D) + centreColX;
        //            }
        //            //第二次拍照
        //            else if (bIZ_Variable.PhotoSignal > 10 && bIZ_Variable.PhotoSignal < 20)
        //            {
        //            }

        //            //计算平台坐标
        //            isSuccess = ImageAlgorithm.GetAffinePoints2d(Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.HomMat2DPlatform,
        //                centreColX, centreRowY, out double NewCentreColX, out double NewCentreRowY);
        //            if (!isSuccess)
        //            {
        //                Log4netRecord.RunLog.Info("计算平台矩阵的中心点坐标失败");
        //                return;
        //            }

        //            isSuccess = ImageAlgorithm.GetAffinePoints2d(Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.HomMat2DPlatform,
        //               Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.BasePointX,
        //               Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.BasePointY,
        //               out double NewBasePointX, out double NewBasePointY);
        //            if (!isSuccess)
        //            {
        //                Log4netRecord.RunLog.Info("计算平台矩阵的基准点坐标失败");
        //                return;
        //            }

        //            centreColX = NewCentreColX - NewBasePointX;
        //            centreRowY = NewCentreRowY - NewBasePointY;

        //            //是否需要取反
        //            if (Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.NotBCValueX)
        //            {
        //                centreColX = -centreColX;
        //            }
        //            if (Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.NotBCValueY)
        //            {
        //                centreRowY = -centreRowY;
        //            }
        //            if (Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.NotBCValueR)
        //            {
        //                lineAngle = -lineAngle;
        //            }
        //            //centreRowY = centreRowY + (lineAngle * 0.11);
        //            //================================================================
        //            //【2】工艺判断
        //            double offsetValueY = 0;//补偿值
        //            double offsetValueX = 0;
        //            double offsetValueR = 0;

        //            //1.A下单片
        //            //2.A上单片
        //            //3.A极片
        //            //4.B极片
        //            //11.A下单片第二次拍照
        //            //12.A上单片第二次拍照
        //            //13.A极片第二次拍照
        //            //14.B极片第二次拍照

        //            //偏移信息记录
        //            string offsetInfo = "";


        //            if (bIZ_Variable.PhotoSignal == 1 || bIZ_Variable.PhotoSignal == 11)//下单片
        //            {
        //                //下电芯
        //                if (bIZ_Variable.UpOrDownCell == 0 || bIZ_Variable.UpOrDownCell == 1)
        //                {
        //                    offsetValueY = Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineDownBCOffectValue.Row;
        //                    offsetValueX = Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineDownBCOffectValue.Col;
        //                    offsetValueR = Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineDownBCOffectValue.Angle;

        //                    offsetInfo = "下电芯下单面补偿值：__" + "Y方向_" + offsetValueY + ",X方向_" + offsetValueX + ",R方向_" + offsetValueR;


        //                }
        //                //上电芯
        //                else if (bIZ_Variable.UpOrDownCell == 2)
        //                {
        //                    offsetValueY = Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineDownBCOffectValue_Up.Row;
        //                    offsetValueX = Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineDownBCOffectValue_Up.Col;
        //                    offsetValueR = Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineDownBCOffectValue_Up.Angle;
        //                    offsetInfo = "上电芯下单面补偿值：__" + "Y方向_" + offsetValueY + ",X方向_" + offsetValueX + ",R方向_" + offsetValueR;
        //                }

        //            }
        //            else if (bIZ_Variable.PhotoSignal == 2 || bIZ_Variable.PhotoSignal == 12)//上单片
        //            {
        //                //下电芯
        //                if (bIZ_Variable.UpOrDownCell == 0 || bIZ_Variable.UpOrDownCell == 1)
        //                {
        //                    offsetValueY = Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineUpBCOffectValue.Row;
        //                    offsetValueX = Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineUpBCOffectValue.Col;
        //                    offsetValueR = Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineUpBCOffectValue.Angle;
        //                    offsetInfo = "下电芯上单面补偿值：__" + "Y方向_" + offsetValueY + ",X方向_" + offsetValueX + ",R方向_" + offsetValueR;
        //                }
        //                //上电芯
        //                else if (bIZ_Variable.UpOrDownCell == 2)
        //                {
        //                    offsetValueY = Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineUpBCOffectValue_Up.Row;
        //                    offsetValueX = Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineUpBCOffectValue_Up.Col;
        //                    offsetValueR = Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineUpBCOffectValue_Up.Angle;
        //                    offsetInfo = "上电芯上单面补偿值：__" + "Y方向_" + offsetValueY + ",X方向_" + offsetValueX + ",R方向_" + offsetValueR;
        //                }

        //            }
        //            else if (bIZ_Variable.PhotoSignal == 3 || bIZ_Variable.PhotoSignal == 13 ||
        //                     bIZ_Variable.PhotoSignal == 4 || bIZ_Variable.PhotoSignal == 14)//极片
        //            {
        //                offsetValueY = Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineHandBCOffectValue.Row;
        //                offsetValueX = Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineHandBCOffectValue.Col;
        //                offsetValueR = Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineHandBCOffectValue.Angle;
        //                offsetInfo = "极片补偿值：__" + "Y方向_" + offsetValueY + ",X方向_" + offsetValueX + ",R方向_" + offsetValueR;
        //            }


        //            offsetInfo += ("\r\n" + "未补偿前数值：" + "Y方向_" + centreRowY.ToString("0.000") + ",X方向_" + centreColX.ToString("0.000") + ",R方向_" + lineAngle.ToString("0.000"));

        //            double coord_BaseOffsetY = Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineHandBCValue.Row;
        //            double coord_BaseOffsetX = Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineHandBCValue.Col;
        //            double coord_BaseOffsetR = Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineHandBCValue.Angle;

        //            offsetInfo += ("\r\n" + "基准补偿数值：" + "Y方向_" + coord_BaseOffsetY.ToString("0.000") + ",X方向_" + coord_BaseOffsetX.ToString("0.000") + ",R方向_" + coord_BaseOffsetR.ToString("0.000"));

        //            coord_y = centreRowY + Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineHandBCValue.Row + offsetValueY;
        //            coord_x = centreColX + Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineHandBCValue.Col + offsetValueX;
        //            coord_angle = lineAngle + Project.Instance.cameraManager[mainCameraEnum].param_PretargetingCalib.MachineHandBCValue.Angle + offsetValueR;
        //            coord_y = Math.Round(coord_y, 3);
        //            coord_x = Math.Round(coord_x, 3);
        //            coord_angle = Math.Round(coord_angle, 3);

        //            offsetInfo += ("\r\n" + "补偿后数值：" + "Y方向_" + coord_y.ToString("0.000") + ",X方向_" + coord_x.ToString("0.000") + ",R方向_" + coord_angle.ToString("0.000"));

        //            Log4netRecord.RunLog.Info(offsetInfo);
        //            if (coord_y >= bIZ_Variable.YBiasMin &&
        //                coord_y <= bIZ_Variable.YBiasMax)
        //            {
        //                coordResult_y = true;
        //            }
        //            if (coord_x >= bIZ_Variable.XBiasMin &&
        //                coord_x <= bIZ_Variable.XBiasMax)
        //            {
        //                coordResult_x = true;
        //            }
        //            if (coord_angle >= bIZ_Variable.YBiasMin &&
        //                coord_angle <= bIZ_Variable.YBiasMax)
        //            {
        //                coordResult_angle = true;
        //            }
        //            result = coordResult_y && coordResult_x && coordResult_angle;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Log4netRecord.ErrorLog.Error(ex);
        //    }

        //}

        /// <summary>
        /// 计算尺寸
        /// </summary>
        /// <param name="sizeInfos"></param>
        public void CalculateSize(out List<SizeInfo> sizeInfos, out bool sizeAllResult)
        {
            sizeInfos = new List<SizeInfo>();
            sizeAllResult = false;
            try
            {
                //获取尺寸检测项
                List<SizeItemInfo> sizeCraft = overHangCraft.ListSizeEdits[0];
                for (int i = 0; i < sizeCraft.Count; i++)
                {
                    if (sizeCraft[i].EditorType == EditorTypeEnum.线距离X.ToString() ||
                        sizeCraft[i].EditorType == EditorTypeEnum.线距离Y.ToString())
                    {
                        string sizeName = sizeCraft[i].Description;
                        SizeInfo sizeInfo = new SizeInfo() { SizeName = sizeName, Description = sizeName, DataResult = -999, resultType = ResultType.NG };
                        try
                        {
                            CameraEnum camera1 = (CameraEnum)Enum.Parse(typeof(CameraEnum), sizeCraft[i].camera1);
                            CameraEnum camera2 = (CameraEnum)Enum.Parse(typeof(CameraEnum), sizeCraft[i].camera2);
                            LineInfo line1 = GetCamLine(camera1, sizeCraft[i].Line1);
                            LineInfo line2 = GetCamLine(camera2, sizeCraft[i].Line2);
                            if (line1 != null && line2 != null)
                            {
                                if (line1.IsLineSuccessfull && line2.IsLineSuccessfull)
                                {
                                    LineInfo newLine1 = new LineInfo();
                                    LineInfo newLine2 = new LineInfo();
                                    bool isSuccess1 = ImageAlgorithm.GetAffinePoints2d(Project.Instance.cameraManager[camera1].param_PretargetingCalib.HomMat2D, line1.ColBegin, line1.RowBegin, out newLine1.ColBegin, out newLine1.RowBegin);
                                    bool isSuccess2 = ImageAlgorithm.GetAffinePoints2d(Project.Instance.cameraManager[camera1].param_PretargetingCalib.HomMat2D, line1.ColEnd, line1.RowEnd, out newLine1.ColEnd, out newLine1.RowEnd);
                                    bool isSuccess3 = ImageAlgorithm.GetAffinePoints2d(Project.Instance.cameraManager[camera2].param_PretargetingCalib.HomMat2D, line2.ColBegin, line2.RowBegin, out newLine2.ColBegin, out newLine2.RowBegin);
                                    bool isSuccess4 = ImageAlgorithm.GetAffinePoints2d(Project.Instance.cameraManager[camera2].param_PretargetingCalib.HomMat2D, line2.ColEnd, line2.RowEnd, out newLine2.ColEnd, out newLine2.RowEnd);

                                    if (isSuccess1 && isSuccess2 && isSuccess3 && isSuccess4)
                                    {
                                        //BIZ_General.CalculateLines_coefficient(newLine1, newLine2, sizeCraft[i].EditorType, sizeCraft[i].StD_RealFixRito, sizeCraft[i].StD_RealFixOffset, out double Val);
                                        BIZ_General.CalculateLinesPl(newLine1, newLine2, sizeCraft[i].EditorType, sizeCraft[i].StD_RealFixRito, sizeCraft[i].StD_RealFixOffset, out double Val);
                                        sizeInfo.DataResult = Val;
                                        sizeInfo.StandardValue = sizeCraft[i].StDValue;
                                        sizeInfo.lowerLimitValue = sizeCraft[i].StDValue + sizeCraft[i].StD_Down;
                                        sizeInfo.upperLimitValue = sizeCraft[i].StDValue + sizeCraft[i].StD_Up;
                                    }

                                }
                            }

                            //=============================================
                            //【】工艺判断
                            if (sizeInfo.DataResult != -999)
                            {
                                if (sizeInfo.DataResult >= sizeCraft[i].StDValue + sizeCraft[i].StD_Down &&
                                 sizeInfo.DataResult <= (sizeCraft[i].StDValue + sizeCraft[i].StD_Up))
                                {
                                    sizeInfo.resultType = ResultType.OK;
                                }
                                else
                                {
                                    if (sizeCraft[i].DB)
                                    {
                                        sizeInfo.resultType = ResultType.NG;
                                    }
                                    else
                                    {
                                        sizeInfo.resultType = ResultType.Warn;
                                    }
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            Log4netRecord.ErrorLog.Error($"{sizeName}尺寸计算异常", ex);
                        }

                        sizeInfos.Add(sizeInfo);
                    }

                }

                sizeAllResult = sizeInfos.Where(c => c.resultType == ResultType.NG).Count() == 0;

            }
            catch (Exception ex)
            {
                sizeAllResult = false;
                Log4netRecord.ErrorLog.Error(ex);
            }

        }

        public LineInfo GetCamLine(CameraEnum camera, string lineName)
        {
            LineInfo line = null;
            if (camera == CameraEnum.Cam1)
            {
                List<LineInfo> LineShowInfos = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam1.m_lineShowInfos;
                line = LineShowInfos.Find(c => c.StrLineName == lineName);
            }
            if (camera == CameraEnum.Cam2)
            {
                List<LineInfo> LineShowInfos = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam2.m_lineShowInfos;
                line = LineShowInfos.Find(c => c.StrLineName == lineName);
            }
            if (camera == CameraEnum.Cam3)
            {
                List<LineInfo> LineShowInfos = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam3.m_lineShowInfos;
                line = LineShowInfos.Find(c => c.StrLineName == lineName);
            }
            if (camera == CameraEnum.Cam4)
            {
                List<LineInfo> LineShowInfos = bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam4.m_lineShowInfos;
                line = LineShowInfos.Find(c => c.StrLineName == lineName);
            }
            return line;
        }

        public bool CalculateLines(CameraEnum cameraEnum1, LineInfo lineInfo1, CameraEnum cameraEnum2, LineInfo lineInfo2, string Edtype, double coefficient, out double Val)
        {
            Val = -999;
            try
            {
                if (lineInfo1 == null || lineInfo2 == null) return false;
                if (lineInfo1.IsLineSuccessfull == false || lineInfo2.IsLineSuccessfull == false) return false;

                double line1midcol = (lineInfo1.ColBegin + lineInfo1.ColEnd) / 2;
                double line1midrow = (lineInfo1.RowBegin + lineInfo1.RowEnd) / 2;
                double line2midcol = (lineInfo2.ColBegin + lineInfo2.ColEnd) / 2;
                double line2midrow = (lineInfo2.RowBegin + lineInfo2.RowEnd) / 2;

                bool isSuccess1 = ImageAlgorithm.GetAffinePoints2d(Project.Instance.cameraManager[cameraEnum1].param_PretargetingCalib.HomMat2D, line1midcol, line1midrow, out double out_currentX1, out double out_currentY1);
                bool isSuccess2 = ImageAlgorithm.GetAffinePoints2d(Project.Instance.cameraManager[cameraEnum2].param_PretargetingCalib.HomMat2D, line2midcol, line2midrow, out double out_currentX2, out double out_currentY2);

                if (!isSuccess1 || !isSuccess2) return false;


                if (Edtype == EditorTypeEnum.线距离X.ToString())
                {

                    Val = out_currentX1 - out_currentX2;
                    Val = Val * coefficient;
                }
                else if (Edtype == EditorTypeEnum.线距离Y.ToString())
                {
                    Val = out_currentY1 - out_currentY2;
                    Val = Val * coefficient;
                }
                Val = Math.Round(Val, 2);
                return true;


            }
            catch (Exception)
            {
                return false;
            }

        }



        /// <summary>
        /// 获取产品结果
        /// </summary>
        /// <returns></returns>
        public ProcessExecutionResult GetProdResult()
        {
            try
            {
                //================================================================
                //【1】产品判断         
                lock (Prod_Lock)
                {
                    if (bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam1.CamFinish == true &&
                        bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam2.CamFinish == true &&
                        bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam3.CamFinish == true &&
                        bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam4.CamFinish == true)
                    {
                        bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam1.CamFinish = false;
                        bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam2.CamFinish = false;
                        bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam3.CamFinish = false;
                        bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam4.CamFinish = false;
                        return ProcessExecutionResult.Success;
                    }
                }

                return ProcessExecutionResult.ContinueExecute;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                return ProcessExecutionResult.Exception;
            }
        }


        /// <summary>
        /// 将原始图像转换为显示图像
        /// </summary>
        /// <param name="cameraEnum"></param>
        /// <param name="showHalconWindow"></param>
        /// <param name="result"></param>
        /// <param name="frame"></param>
        /// <param name="PackgeNum"></param>
        /// <param name="EAIndex"></param>
        /// <param name="operation_ImageInfo"></param>
        private void PrimitiveTransition(CameraEnum cameraEnum, ShowHalconWindow showHalconWindow, bool result, long frame, int PoleplateNumber, out Operation_ImageInfo operation_ImageInfo)
        {
            operation_ImageInfo = null;
            try
            {
                ImageSaveParamer in_imageSaveParamer = Project.Instance.cameraManager[cameraEnum].m_ImageSaveParamer;
                showHalconWindow.m_bIsNG = !result;

                string imageSavePath = in_imageSaveParamer.ImageSave_Path_Primitive;
                ImageSaveFormat saveImageFormat = (ImageSaveFormat)in_imageSaveParamer.SaveImageFormat_Primitive;
                ImageSaveSelect imageSaveSelect = (ImageSaveSelect)in_imageSaveParamer.SaveImageSelect_Primitive;
                bool isShowImage = in_imageSaveParamer.IsShowImage_Primitive;
                bool HighDefinition_Screenshot = in_imageSaveParamer.HighDefinition_Screenshot;
                ImageSaveSelect screenshotSaveSelect = (ImageSaveSelect)in_imageSaveParamer.SaveImageSelect_Screenshot;
                string screenshotSavePath = in_imageSaveParamer.ImageSave_Path_Screenshot;
                ImageSaveFormat screenshotSaveFormat = (ImageSaveFormat)in_imageSaveParamer.SaveImageFormat_Screenshot;
                string imageSaveName = frame.ToString().PadLeft(5, '0');
                operation_ImageInfo = BIZ_General.ImageDataTypeTransition_Pretargeting(showHalconWindow, Project.Instance.paramPathSet.ProductMojuanPath, imageSavePath, imageSaveName, saveImageFormat, imageSaveSelect, isShowImage,
                   HighDefinition_Screenshot, screenshotSaveSelect, Project.Instance.paramPathSet.ProductMojuanPath, screenshotSavePath, imageSaveName, screenshotSaveFormat, cameraEnum.ToString(), $"拍照信号{PoleplateNumber}");
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(cameraEnum.ToString(), ex);
            }


        }

        /// <summary>
        /// 将图像队列转换为显示图像队列
        /// </summary>
        /// <param name="_source"></param>
        /// <param name="imageSaveParamer"></param>
        /// <param name="imageInfosShow"></param>
        /// <param name="ngTypeInfos"></param>
        private void NGImageQueueTransition(CameraEnum cameraEnum, List<NgTypeInfo> ngTypeInfos, int frame, int PackgeNum, int EAIndex, ref QueuePackage<Operation_ImageInfo> out_operationImageInfos)
        {
            try
            {
                if (ngTypeInfos == null || ngTypeInfos.Count == 0) return;

                ImageSaveParamer in_imageSaveParamer = Project.Instance.cameraManager[cameraEnum].m_ImageSaveParamer;
                //获取显示图像
                int count = ngTypeInfos.Count;
                for (int i = 0; i < count; i++)
                {
                    NgTypeInfo ngTypeInfo = ngTypeInfos[i];
                    string imageSavePath = in_imageSaveParamer.ImageSave_Path_Min;
                    imageSavePath += $"\\{ngTypeInfo.m_strNgTypeName}\\";
                    ImageSaveFormat saveImageFormat = (ImageSaveFormat)in_imageSaveParamer.SaveImageFormat_Min;
                    bool isSaveImage = in_imageSaveParamer.IsSaveImage_Min;
                    bool isShowImage = in_imageSaveParamer.IsShowImage_Min;
                    string imageSaveName = frame.ToString().PadLeft(5, '0') + $"_{PackgeNum}_{EAIndex}";
                    Operation_ImageInfo operation_ImageInfo = BIZ_General.NGImageDataTypeTransition(ngTypeInfo, Project.Instance.paramPathSet.ProductMojuanPath, imageSavePath, imageSaveName, saveImageFormat, isSaveImage, isShowImage,
                        cameraEnum.ToString());

                    out_operationImageInfos.Enqueue(operation_ImageInfo);
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(cameraEnum.ToString(), ex);
            }


        }



        /// <summary>
        /// 读取拍照信号
        /// </summary>
        /// <returns></returns>
        public short ReadPhotoSignal()
        {
            short readValue = 0;
            if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面A极片纠偏工位)
            {
                Project.Instance.cutOverlayPLCManage.ReadAndWriteZero(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PhotoSignal_AAYDW, Project.Instance.cutOverlayPLCManage.CommunicationLog, true, out readValue);
            }
            else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面B极片纠偏工位)
            {
                Project.Instance.cutOverlayPLCManage.ReadAndWriteZero(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PhotoSignal_ABYDW, Project.Instance.cutOverlayPLCManage.CommunicationLog, true, out readValue);
            }
            else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面A极片纠偏工位)
            {
                Project.Instance.cutOverlayPLCManage.ReadAndWriteZero(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PhotoSignal_BAYDW, Project.Instance.cutOverlayPLCManage.CommunicationLog, true, out readValue);
            }
            else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面B极片纠偏工位)
            {
                Project.Instance.cutOverlayPLCManage.ReadAndWriteZero(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PhotoSignal_BBYDW, Project.Instance.cutOverlayPLCManage.CommunicationLog, true, out readValue);
            }

            return readValue;
        }

        public void SendHeartBeat()
        {
            bool IsIntervalTime = intervalExecution.GetWhetherToReachIntervalTime(1000);
            if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面A极片纠偏工位)
            {
                if (IsIntervalTime)
                {
                    //心跳信号
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_Heart_CCD_AAYDW.Value = 1;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_Heart_CCD_AAYDW, true, Project.Instance.cutOverlayPLCManage.HeartbeatLog);
                }
            }
            else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面B极片纠偏工位)
            {
                if (IsIntervalTime)
                {
                    //心跳信号
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_Heart_CCD_ABYDW.Value = 1;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_Heart_CCD_ABYDW, true, Project.Instance.cutOverlayPLCManage.HeartbeatLog);
                }
            }
            else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面A极片纠偏工位)
            {
                if (IsIntervalTime)
                {
                    //心跳信号
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_Heart_CCD_BAYDW.Value = 1;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_Heart_CCD_BAYDW, true, Project.Instance.cutOverlayPLCManage.HeartbeatLog);
                }
            }
            else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面B极片纠偏工位)
            {
                if (IsIntervalTime)
                {
                    //心跳信号
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_Heart_CCD_BBYDW.Value = 1;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_Heart_CCD_BBYDW, true, Project.Instance.cutOverlayPLCManage.HeartbeatLog);
                }
            }
            else
            {
                Log4netRecord.ErrorLog.Debug("工位选择错误");
                TJBJInfo.AlarmWriteState(AlarmName.工位选择错误, StopAlarmStateEnum.停机, "");
                return;
            }
        }
        public void SendResult(float OffsetValueX, float OffsetValueY, float OffsetValueR, short PhotoResult)
        {
            if (Project.Instance.Is_simulation) return;

            if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面A极片纠偏工位)
            {


                //第一次拍照
                if (bIZ_Variable.pretargetingBatteryProducts.EAIndex > 0 && bIZ_Variable.pretargetingBatteryProducts.EAIndex < 10)
                {
                    //X偏移
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_OffsetValueX_AAYDW.Value = OffsetValueX;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_OffsetValueX_AAYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                    //Y偏移
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_OffsetValueY_AAYDW.Value = OffsetValueY;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_OffsetValueY_AAYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                    //角度
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_OffsetValueR_AAYDW.Value = OffsetValueR;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_OffsetValueR_AAYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                    //结果
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult1_AAYDW.Value = PhotoResult;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult1_AAYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }

                //第二次拍照
                if (bIZ_Variable.pretargetingBatteryProducts.EAIndex > 10 && bIZ_Variable.pretargetingBatteryProducts.EAIndex < 20)
                {
                    //结果
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult2_AAYDW.Value = PhotoResult;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult2_AAYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }


            }
            else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面B极片纠偏工位)
            {


                //第一次拍照
                if (bIZ_Variable.pretargetingBatteryProducts.EAIndex > 0 && bIZ_Variable.pretargetingBatteryProducts.EAIndex < 10)
                {
                    //X偏移
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_OffsetValueX_ABYDW.Value = OffsetValueX;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_OffsetValueX_ABYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                    //Y偏移
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_OffsetValueY_ABYDW.Value = OffsetValueY;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_OffsetValueY_ABYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                    //角度
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_OffsetValueR_ABYDW.Value = OffsetValueR;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_OffsetValueR_ABYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                    //结果
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult1_ABYDW.Value = PhotoResult;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult1_ABYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }

                //第二次拍照
                if (bIZ_Variable.pretargetingBatteryProducts.EAIndex > 10 && bIZ_Variable.pretargetingBatteryProducts.EAIndex < 20)
                {
                    //结果
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult2_ABYDW.Value = PhotoResult;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult2_ABYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }


            }

            else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面A极片纠偏工位)
            {


                //第一次拍照
                if (bIZ_Variable.pretargetingBatteryProducts.EAIndex > 0 && bIZ_Variable.pretargetingBatteryProducts.EAIndex < 10)
                {
                    //X偏移
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_OffsetValueX_BAYDW.Value = OffsetValueX;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_OffsetValueX_BAYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                    //Y偏移
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_OffsetValueY_BAYDW.Value = OffsetValueY;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_OffsetValueY_BAYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                    //角度
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_OffsetValueR_BAYDW.Value = OffsetValueR;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_OffsetValueR_BAYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                    //结果
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult1_BAYDW.Value = PhotoResult;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult1_BAYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }

                //第二次拍照
                if (bIZ_Variable.pretargetingBatteryProducts.EAIndex > 10 && bIZ_Variable.pretargetingBatteryProducts.EAIndex < 20)
                {
                    //结果
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult2_BAYDW.Value = PhotoResult;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult2_BAYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }


            }

            else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面B极片纠偏工位)
            {


                //第一次拍照
                if (bIZ_Variable.pretargetingBatteryProducts.EAIndex > 0 && bIZ_Variable.pretargetingBatteryProducts.EAIndex < 10)
                {
                    //X偏移
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_OffsetValueX_BBYDW.Value = OffsetValueX;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_OffsetValueX_BBYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                    //Y偏移
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_OffsetValueY_BBYDW.Value = OffsetValueY;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_OffsetValueY_BBYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                    //角度
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_OffsetValueR_BBYDW.Value = OffsetValueR;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_OffsetValueR_BBYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                    //结果
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult1_BBYDW.Value = PhotoResult;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult1_BBYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }

                //第二次拍照
                if (bIZ_Variable.pretargetingBatteryProducts.EAIndex > 10 && bIZ_Variable.pretargetingBatteryProducts.EAIndex < 20)
                {
                    //结果
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult2_BBYDW.Value = PhotoResult;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult2_BBYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }


            }
        }

        public void ReadCraft(ReadLogSaveType readLogSaveType)
        {
            try
            {
                if (Project.Instance.Is_simulation)
                {
                    bIZ_Variable.XBiasMax = 10;
                    bIZ_Variable.XBiasMin = -10;
                    bIZ_Variable.YBiasMax = 10;
                    bIZ_Variable.YBiasMin = -10;
                    bIZ_Variable.AngleBiasMax = 10;
                    bIZ_Variable.AngleBiasMin = -10;
                    return;
                }
                if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面A极片纠偏工位)
                {


                    Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_XAxisMax_AAYDW, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_XAxisMin_AAYDW, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_YAxisMax_AAYDW, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_YAxisMin_AAYDW, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_RAxisMax_AAYDW, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_RAxisMin_AAYDW, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                    bIZ_Variable.XBiasMax = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_XAxisMax_AAYDW.Value;
                    bIZ_Variable.XBiasMin = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_XAxisMin_AAYDW.Value;
                    bIZ_Variable.YBiasMax = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_YAxisMax_AAYDW.Value;
                    bIZ_Variable.YBiasMin = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_YAxisMin_AAYDW.Value;
                    bIZ_Variable.AngleBiasMax = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_RAxisMax_AAYDW.Value;
                    bIZ_Variable.AngleBiasMin = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_RAxisMin_AAYDW.Value;
                }
                else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面B极片纠偏工位)
                {

                    Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_XAxisMax_ABYDW, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_XAxisMin_ABYDW, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_YAxisMax_ABYDW, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_YAxisMin_ABYDW, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_RAxisMax_ABYDW, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_RAxisMin_ABYDW, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                    bIZ_Variable.XBiasMax = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_XAxisMax_ABYDW.Value;
                    bIZ_Variable.XBiasMin = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_XAxisMin_ABYDW.Value;
                    bIZ_Variable.YBiasMax = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_YAxisMax_ABYDW.Value;
                    bIZ_Variable.YBiasMin = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_YAxisMin_ABYDW.Value;
                    bIZ_Variable.AngleBiasMax = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_RAxisMax_ABYDW.Value;
                    bIZ_Variable.AngleBiasMin = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_RAxisMin_ABYDW.Value;
                }
                else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面A极片纠偏工位)
                {

                    Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_XAxisMax_BAYDW, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_XAxisMin_BAYDW, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_YAxisMax_BAYDW, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_YAxisMin_BAYDW, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_RAxisMax_BAYDW, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_RAxisMin_BAYDW, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                    bIZ_Variable.XBiasMax = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_XAxisMax_BAYDW.Value;
                    bIZ_Variable.XBiasMin = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_XAxisMin_BAYDW.Value;
                    bIZ_Variable.YBiasMax = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_YAxisMax_BAYDW.Value;
                    bIZ_Variable.YBiasMin = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_YAxisMin_BAYDW.Value;
                    bIZ_Variable.AngleBiasMax = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_RAxisMax_BAYDW.Value;
                    bIZ_Variable.AngleBiasMin = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_RAxisMin_BAYDW.Value;
                }
                else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面B极片纠偏工位)
                {

                    Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_XAxisMax_BBYDW, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_XAxisMin_BBYDW, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_YAxisMax_BBYDW, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_YAxisMin_BBYDW, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_RAxisMax_BBYDW, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_RAxisMin_BBYDW, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                    bIZ_Variable.XBiasMax = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_XAxisMax_BBYDW.Value;
                    bIZ_Variable.XBiasMin = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_XAxisMin_BBYDW.Value;
                    bIZ_Variable.YBiasMax = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_YAxisMax_BBYDW.Value;
                    bIZ_Variable.YBiasMin = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_YAxisMin_BBYDW.Value;
                    bIZ_Variable.AngleBiasMax = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_RAxisMax_BBYDW.Value;
                    bIZ_Variable.AngleBiasMin = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_RAxisMin_BBYDW.Value;
                }
                //overHangCraft.Save();

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }

        /// <summary>
        /// 获取尺寸工艺
        /// </summary>
        /// <param name="cameraEnum"></param>
        /// <param name="sizeName"></param>
        /// <param name="upper"></param>
        /// <param name="standard"></param>
        /// <param name="lower"></param>
        public void GetSizeCarft(CameraEnum cameraEnum, string sizeName, out string sizeNameDescription, out double upper, out double standard, out double lower)
        {
            upper = 1;
            standard = 0.5;
            lower = 0;
            sizeNameDescription = "default";
            if (overHangCraft.ListSizeEdits[(int)cameraEnum].Where(x => x.Name == sizeName).Count() > 0)
            {
                sizeNameDescription = overHangCraft.ListSizeEdits[(int)cameraEnum].Find(p => p.Name == sizeName).Description;
                standard = overHangCraft.ListSizeEdits[(int)cameraEnum].Find(p => p.Name == sizeName).StDValue;
                upper = overHangCraft.ListSizeEdits[(int)cameraEnum].Find(p => p.Name == sizeName).StD_Up;
                lower = overHangCraft.ListSizeEdits[(int)cameraEnum].Find(p => p.Name == sizeName).StD_Down;
            }
        }

        #region 曲线图
        public void InitChart()
        {
            try
            {
                CameraEnum cameraEnum = CameraEnum.Cam1;
                //初始化曲线图
                //////////////////////////

                if (true)
                {
                    string chartName = "第一次拍照X偏移";

                    ParamsChart paramsChart = new ParamsChart();
                    paramsChart.f_UpLimit = bIZ_Variable.XBiasMin;
                    paramsChart.f_DownLimit = bIZ_Variable.XBiasMax;
                    paramsChart.f_StandarValue = 0;
                    paramsChart.points.Add(new PointClass() { CurName = "测量值", Point = new Point(0, 0), Color = Color.Blue });
                    chartUIParams[chartName] = new ChartUIParam(chartName, form.formsPlot_FirstPhotoShootOffsetX);
                    chartUIParams[chartName].InitCurve(paramsChart);
                }

                if (true)
                {
                    string chartName = "第一次拍照Y偏移";

                    ParamsChart paramsChart = new ParamsChart();
                    paramsChart.f_UpLimit = bIZ_Variable.YBiasMin;
                    paramsChart.f_DownLimit = bIZ_Variable.YBiasMax;
                    paramsChart.f_StandarValue = 0;
                    paramsChart.points.Add(new PointClass() { CurName = "测量值", Point = new Point(0, 0), Color = Color.Blue });
                    chartUIParams[chartName] = new ChartUIParam(chartName, form.formsPlot_FirstPhotoShootOffsetY);
                    chartUIParams[chartName].InitCurve(paramsChart);
                }

                if (true)
                {
                    string chartName = "第一次拍照角度";

                    ParamsChart paramsChart = new ParamsChart();
                    paramsChart.f_UpLimit = bIZ_Variable.AngleBiasMin;
                    paramsChart.f_DownLimit = bIZ_Variable.AngleBiasMax;
                    paramsChart.f_StandarValue = 0;
                    paramsChart.points.Add(new PointClass() { CurName = "测量值", Point = new Point(0, 0), Color = Color.Blue });
                    chartUIParams[chartName] = new ChartUIParam(chartName, form.formsPlot_FirstPhotoShootOffsetR);
                    chartUIParams[chartName].InitCurve(paramsChart);
                }

                if (true)
                {
                    string chartName = "第二次拍照X偏移";

                    ParamsChart paramsChart = new ParamsChart();
                    paramsChart.f_UpLimit = bIZ_Variable.XBiasMin;
                    paramsChart.f_DownLimit = bIZ_Variable.XBiasMax;
                    paramsChart.f_StandarValue = 0;
                    paramsChart.points.Add(new PointClass() { CurName = "测量值", Point = new Point(0, 0), Color = Color.Blue });
                    chartUIParams[chartName] = new ChartUIParam(chartName, form.formsPlot_SecondPhotoShootOffsetX);
                    chartUIParams[chartName].InitCurve(paramsChart);
                }

                if (true)
                {
                    string chartName = "第二次拍照Y偏移";

                    ParamsChart paramsChart = new ParamsChart();
                    paramsChart.f_UpLimit = bIZ_Variable.YBiasMin;
                    paramsChart.f_DownLimit = bIZ_Variable.YBiasMax;
                    paramsChart.f_StandarValue = 0;
                    paramsChart.points.Add(new PointClass() { CurName = "测量值", Point = new Point(0, 0), Color = Color.Blue });
                    chartUIParams[chartName] = new ChartUIParam(chartName, form.formsPlot_SecondPhotoShootOffsetY);
                    chartUIParams[chartName].InitCurve(paramsChart);
                }

                if (true)
                {
                    string chartName = "第二次拍照角度";

                    ParamsChart paramsChart = new ParamsChart();
                    paramsChart.f_UpLimit = bIZ_Variable.AngleBiasMin;
                    paramsChart.f_DownLimit = bIZ_Variable.AngleBiasMax;
                    paramsChart.f_StandarValue = 0;
                    paramsChart.points.Add(new PointClass() { CurName = "测量值", Point = new Point(0, 0), Color = Color.Blue });
                    chartUIParams[chartName] = new ChartUIParam(chartName, form.formsPlot_SecondPhotoShootOffsetR);
                    chartUIParams[chartName].InitCurve(paramsChart);
                }

                //==================================
                cameraEnum = CameraEnum.Cam1;
                if (true)
                {
                    string sizeName = "W1";
                    GetSizeCarft(cameraEnum, sizeName, out string sizeNameDescription, out double upper, out double standard, out double lower);
                    string chartName = sizeNameDescription;

                    ParamsChart paramsChart = new ParamsChart();
                    paramsChart.f_UpLimit = standard + upper;
                    paramsChart.f_DownLimit = standard + lower;
                    paramsChart.f_StandarValue = standard;
                    paramsChart.points.Add(new PointClass() { CurName = "测量值", Point = new Point(0, 0), Color = Color.Blue });
                    chartUIParams[chartName] = new ChartUIParam(chartName, form.formsPlot_WholeWidth);
                    chartUIParams[chartName].InitCurve(paramsChart);
                }

                if (true)
                {
                    string sizeName = "W2";
                    GetSizeCarft(cameraEnum, sizeName, out string sizeNameDescription, out double upper, out double standard, out double lower);
                    string chartName = sizeNameDescription;

                    ParamsChart paramsChart = new ParamsChart();
                    paramsChart.f_UpLimit = standard + upper;
                    paramsChart.f_DownLimit = standard + lower;
                    paramsChart.f_StandarValue = standard;
                    paramsChart.points.Add(new PointClass() { CurName = "测量值", Point = new Point(0, 0), Color = Color.Blue });
                    chartUIParams[chartName] = new ChartUIParam(chartName, form.formsPlot_Width2);
                    chartUIParams[chartName].InitCurve(paramsChart);
                }

                if (true)
                {
                    string sizeName = "H1";
                    GetSizeCarft(cameraEnum, sizeName, out string sizeNameDescription, out double upper, out double standard, out double lower);
                    string chartName = sizeNameDescription;

                    ParamsChart paramsChart = new ParamsChart();
                    paramsChart.f_UpLimit = standard + upper;
                    paramsChart.f_DownLimit = standard + lower;
                    paramsChart.f_StandarValue = standard;
                    paramsChart.points.Add(new PointClass() { CurName = "测量值", Point = new Point(0, 0), Color = Color.Blue });
                    chartUIParams[chartName] = new ChartUIParam(chartName, form.formsPlot_WholeHeight);
                    chartUIParams[chartName].InitCurve(paramsChart);
                }

                if (true)
                {
                    string sizeName = "H2";
                    GetSizeCarft(cameraEnum, sizeName, out string sizeNameDescription, out double upper, out double standard, out double lower);
                    string chartName = sizeNameDescription;

                    ParamsChart paramsChart = new ParamsChart();
                    paramsChart.f_UpLimit = standard + upper;
                    paramsChart.f_DownLimit = standard + lower;
                    paramsChart.f_StandarValue = standard;
                    paramsChart.points.Add(new PointClass() { CurName = "测量值", Point = new Point(0, 0), Color = Color.Blue });
                    chartUIParams[chartName] = new ChartUIParam(chartName, form.formsPlot_Height2);
                    chartUIParams[chartName].InitCurve(paramsChart);
                }

                if (true)
                {
                    string sizeName = "JRW";
                    GetSizeCarft(cameraEnum, sizeName, out string sizeNameDescription, out double upper, out double standard, out double lower);
                    string chartName = sizeNameDescription;

                    ParamsChart paramsChart = new ParamsChart();
                    paramsChart.f_UpLimit = standard + upper;
                    paramsChart.f_DownLimit = standard + lower;
                    paramsChart.f_StandarValue = standard;
                    paramsChart.points.Add(new PointClass() { CurName = "测量值", Point = new Point(0, 0), Color = Color.Blue });
                    chartUIParams[chartName] = new ChartUIParam(chartName, form.formsPlot_JRWidth);
                    chartUIParams[chartName].InitCurve(paramsChart);
                }

                if (true)
                {
                    string sizeName = "JRH";
                    GetSizeCarft(cameraEnum, sizeName, out string sizeNameDescription, out double upper, out double standard, out double lower);
                    string chartName = sizeNameDescription;

                    ParamsChart paramsChart = new ParamsChart();
                    paramsChart.f_UpLimit = standard + upper;
                    paramsChart.f_DownLimit = standard + lower;
                    paramsChart.f_StandarValue = standard;
                    paramsChart.points.Add(new PointClass() { CurName = "测量值", Point = new Point(0, 0), Color = Color.Blue });
                    chartUIParams[chartName] = new ChartUIParam(chartName, form.formsPlot_JRHeight);
                    chartUIParams[chartName].InitCurve(paramsChart);
                }

                if (true)
                {
                    string sizeName = "JRD";
                    GetSizeCarft(cameraEnum, sizeName, out string sizeNameDescription, out double upper, out double standard, out double lower);
                    string chartName = sizeNameDescription;

                    ParamsChart paramsChart = new ParamsChart();
                    paramsChart.f_UpLimit = standard + upper;
                    paramsChart.f_DownLimit = standard + lower;
                    paramsChart.f_StandarValue = standard;
                    paramsChart.points.Add(new PointClass() { CurName = "测量值", Point = new Point(0, 0), Color = Color.Blue });
                    chartUIParams[chartName] = new ChartUIParam(chartName, form.formsPlot_JRCalcarineMargin);
                    chartUIParams[chartName].InitCurve(paramsChart);
                }

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }

        }

        public void CamChartDataShow(CameraEnum cameraEnum)
        {
            try
            {


                foreach (var item in bIZ_Variable.ChartSizeDetectionItemData)
                {
                    chartUIParams.TryGetValue(item.Key, out ChartUIParam chartUIParam);
                    if (chartUIParam != null)
                    {
                        List<ChartPoint> measureData = item.Value.lChartAllTempData.GetListAndClear().ToList();
                        ChartDataShow(item.Key, measureData, chartUIParam);
                    }

                }

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(cameraEnum.ToString(), ex);
            }
        }
        public void ChartDataShow(string sourec, List<ChartPoint> measureData, ChartUIParam chartUIParam)
        {
            try
            {
                if (measureData == null || measureData.Count == 0) return;

                for (int i = 0; i < measureData.Count; i++)
                {
                    ParamsChart paramsChart = new ParamsChart();
                    PointClass point = new PointClass();
                    //point.CurName = "平均值";
                    //point.pointInfo = new PointInfo() { Col = 0, Row = measureData[i].ChartMeanData };
                    //paramsChart.points.Add(point);

                    point = new PointClass();
                    point.CurName = "测量值";
                    point.pointInfo = new PointInfo() { Col = 0, Row = measureData[i].ChartDetectionData };
                    paramsChart.points.Add(point);

                    chartUIParam.UpDataCurve(paramsChart);
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(sourec, ex);
            }

        }

        public void AddChartData(string chartName, double data)
        {
            //添加曲线图数据
            BIZ_General.DictionaryKeySafety(chartName, ref bIZ_Variable.ChartSizeDetectionItemData);
            bIZ_Variable.ChartSizeDetectionItemData[chartName].lChartAllData.Add(new ChartPoint() { ChartDetectionData = data }, 100);

            ChartPoint chartTempPoint = new ChartPoint() { ChartDetectionData = data };
            bIZ_Variable.ChartSizeDetectionItemData[chartName].lChartAllTempData.Add(chartTempPoint, 100);
        }
        #endregion

        #region 通信
        /// <summary>
        /// 通信
        /// </summary>
        public void Communication()
        {
            try
            {
                if (bIZ_ThreadManage.thd_Communication.ExecuteTime > 20)
                {

                }
                //==========================
                //【】心跳信号
                if (Project.Instance.Is_Run)
                    SendHeartBeat();

                ReadCraft(ReadLogSaveType.ReadDifferentState);
                //PLC下发一键换卷
                AutoChangeRoll();
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }

        /// <summary>
        /// PLC触发，一键换型。以当天日期为膜卷号-A面模切
        /// </summary>
        /// <param name="reelNumber"></param>
        public void AutoChangeRoll()
        {
            try
            {
                string reelNumber = DateTime.Now.ToString("yyyyMMddHHmm");

                if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面A极片纠偏工位)
                {
                    //记录PLC纠偏量
                    Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.InOut_ChangeRollTrigger_AAYDW, ReadLogSaveType.ReadDifferentState, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    bool IsChangeRollTrigger = Project.Instance.cutOverlayPLCManage.m_DevAddress.InOut_ChangeRollTrigger_AAYDW.Value;
                    if (IsChangeRollTrigger)
                    {
                        bool result = Form_Main.FormMain.MainDetection.AutoHJ(reelNumber);
                        if (result)
                        {
                            Project.Instance.cutOverlayPLCManage.m_DevAddress.InOut_ChangeRollResult_AAYDW.Value = true;
                        }
                        else
                        {
                            Project.Instance.cutOverlayPLCManage.m_DevAddress.InOut_ChangeRollResult_AAYDW.Value = false;
                        }
                        Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.InOut_ChangeRollResult_AAYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    }
                }
                else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面B极片纠偏工位)
                {
                    //记录PLC纠偏量
                    Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.InOut_ChangeRollTrigger_ABYDW, ReadLogSaveType.ReadDifferentState, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    bool IsChangeRollTrigger = Project.Instance.cutOverlayPLCManage.m_DevAddress.InOut_ChangeRollTrigger_ABYDW.Value;
                    if (IsChangeRollTrigger)
                    {
                        bool result = Form_Main.FormMain.MainDetection.AutoHJ(reelNumber);
                        if (result)
                        {
                            Project.Instance.cutOverlayPLCManage.m_DevAddress.InOut_ChangeRollResult_ABYDW.Value = true;
                        }
                        else
                        {
                            Project.Instance.cutOverlayPLCManage.m_DevAddress.InOut_ChangeRollResult_ABYDW.Value = false;
                        }
                        Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.InOut_ChangeRollResult_ABYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    }
                }
                else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面A极片纠偏工位)
                {
                    //记录PLC纠偏量
                    Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.InOut_ChangeRollTrigger_BAYDW, ReadLogSaveType.ReadDifferentState, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    bool IsChangeRollTrigger = Project.Instance.cutOverlayPLCManage.m_DevAddress.InOut_ChangeRollTrigger_BAYDW.Value;
                    if (IsChangeRollTrigger)
                    {
                        bool result = Form_Main.FormMain.MainDetection.AutoHJ(reelNumber);
                        if (result)
                        {
                            Project.Instance.cutOverlayPLCManage.m_DevAddress.InOut_ChangeRollResult_BAYDW.Value = true;
                        }
                        else
                        {
                            Project.Instance.cutOverlayPLCManage.m_DevAddress.InOut_ChangeRollResult_BAYDW.Value = false;
                        }
                        Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.InOut_ChangeRollResult_BAYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    }
                }
                else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面B极片纠偏工位)
                {
                    //记录PLC纠偏量
                    Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.InOut_ChangeRollTrigger_BBYDW, ReadLogSaveType.ReadDifferentState, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    bool IsChangeRollTrigger = Project.Instance.cutOverlayPLCManage.m_DevAddress.InOut_ChangeRollTrigger_BBYDW.Value;
                    if (IsChangeRollTrigger)
                    {
                        bool result = Form_Main.FormMain.MainDetection.AutoHJ(reelNumber);
                        if (result)
                        {
                            Project.Instance.cutOverlayPLCManage.m_DevAddress.InOut_ChangeRollResult_BBYDW.Value = true;
                        }
                        else
                        {
                            Project.Instance.cutOverlayPLCManage.m_DevAddress.InOut_ChangeRollResult_BBYDW.Value = false;
                        }
                        Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.InOut_ChangeRollResult_BBYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    }
                }
                else
                {
                    Log4netRecord.ErrorLog.Debug("工位选择错误");
                    TJBJInfo.AlarmWriteState(AlarmName.工位选择错误, StopAlarmStateEnum.停机, "");
                    return;
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }
        #endregion


        #region 接口实现

        public AThreadManageBase ThreadManage { get; set; }

        public void InitForm()
        {
            NLabel.UpdateControls(form.LB_Formula, Project.Instance.systemInfo.CurrentCraft);
            NLabel.UpdateControls(form.LB_DetectionType, Project.Instance.craftBase.materialCrafEnum.ToString());


            //初始化数据显示界面
            CamDetectionVariable camDetectionVariable = Project.Instance.cameraManager[CameraEnum.Cam1].m_CamDetectionVariable;
            NListView.Init(form.listView1, camDetectionVariable.DataTitlesShow);
            NListView.Init(form.listView2, bIZ_Variable.sizeDataTitle.ToArray());

            NLabel.UpdateControls(form.LB_Process1, Project.Instance.cameraManager[CameraEnum.Cam1].m_CamParam.CameraName);
            NLabel.UpdateControls(form.LB_Process2, Project.Instance.cameraManager[CameraEnum.Cam2].m_CamParam.CameraName);
            NLabel.UpdateControls(form.LB_Process3, Project.Instance.cameraManager[CameraEnum.Cam3].m_CamParam.CameraName);
            NLabel.UpdateControls(form.LB_Process4, Project.Instance.cameraManager[CameraEnum.Cam4].m_CamParam.CameraName);

            InitChart();
        }

        public void ClearForm()
        {
            NLabel.UpdateControls(form.LB_Formula, Project.Instance.systemInfo.CurrentCraft);
            NLabel.UpdateControls(form.LB_DetectionType, Project.Instance.craftBase.materialCrafEnum.ToString());


            NListView.DeleteListAllItems(form.listView1);


        }

        public void UpdateForm()
        {
            try
            {
                //曲线图    
                CamChartDataShow(CameraEnum.Cam1);

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }

        public void InitVariable()
        {
            bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam1?.Dispose();
            bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam2?.Dispose();
            bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam3?.Dispose();
            bIZ_Variable.pretargetingBatteryProducts.EAProduct_Cam4?.Dispose();
            bIZ_Variable.pretargetingBatteryProducts = new PretargetingBatteryProduct();
            if (true)
            {
                CameraEnum cameraEnum = CameraEnum.Cam1;
                CamDetectionVariable camDetectionVariable = Project.Instance.cameraManager[cameraEnum].m_CamDetectionVariable;

                //数据表头初始化   
                camDetectionVariable.DataTitlesShow = new string[] { "时间", "X偏移", "Y偏移", "角度", "结果", "电芯编号", "极片编号", "返回结果", "图像初始值X", "图像初始值Y", "图像初始值R", "基准补偿值X", "基准补偿值Y", "基准补偿值R", "极片补偿值X", "极片补偿值Y", "极片补偿值R", "偏移值X上限", "偏移值X下限", "偏移值Y上限", "偏移值Y下限", "偏移值R上限", "偏移值R下限", "平台坐标X", "平台坐标Y", "平台坐标R", "相机1帧号", "相机2帧号", "相机3帧号", "相机4帧号" };
                camDetectionVariable.DataTitlesSave = camDetectionVariable.DataTitlesShow.ToArray();

                List<string> dataShowTitles = new List<string>();
                List<string> dataSaveTitles = new List<string>();
                dataShowTitles.Add("时间");
                dataSaveTitles.Add("时间");
                for (int i = 0; i < overHangCraft.ListSizeEdits[(int)cameraEnum].Count; i++)
                {
                    if (i != 0)
                    {
                        dataShowTitles.Add(overHangCraft.ListSizeEdits[(int)cameraEnum][i].Description);
                        dataSaveTitles.Add(overHangCraft.ListSizeEdits[(int)cameraEnum][i].Description);
                        dataSaveTitles.Add("结果");
                    }
                }
                dataShowTitles.Add("电芯编号");
                dataShowTitles.Add("极片编号");
                dataShowTitles.Add("返回结果");
                dataShowTitles.Add("相机1帧号");
                dataShowTitles.Add("相机2帧号");
                dataShowTitles.Add("相机3帧号");
                dataShowTitles.Add("相机4帧号");

                dataSaveTitles.Add("电芯编号");
                dataSaveTitles.Add("极片编号");
                dataSaveTitles.Add("返回结果");
                dataSaveTitles.Add("相机1帧号");
                dataSaveTitles.Add("相机2帧号");
                dataSaveTitles.Add("相机3帧号");
                dataSaveTitles.Add("相机4帧号");

                camDetectionVariable.DataTitlesShow2 = dataShowTitles.ToArray();
                camDetectionVariable.DataTitlesSave2 = dataSaveTitles.ToArray();

            }

            if (true)
            {
                bIZ_Variable.sizeDataTitle.Clear();
                bIZ_Variable.sizeDataTitle.Add("时间");
                List<SizeItemInfo> sizeCraft = overHangCraft.ListSizeEdits[0];
                for (int i = 0; i < sizeCraft.Count; i++)
                {
                    if (sizeCraft[i].EditorType == EditorTypeEnum.线距离X.ToString() ||
                        sizeCraft[i].EditorType == EditorTypeEnum.线距离Y.ToString())
                    {
                        bIZ_Variable.sizeDataTitle.Add(sizeCraft[i].Description);
                    }

                }
                bIZ_Variable.sizeDataTitle.AddRange(new string[] { "结果", "电芯编号", "极片编号", "返回结果", "相机1帧号", "相机2帧号", "相机3帧号", "相机4帧号" });
            }
        }

        public void ImageShowSave()
        {
            try
            {
                int disposeImageRetainLenght = 5;
                BIZ_General.Image_Process(HWC1.operation_Image, disposeImageRetainLenght, HWC1);
                BIZ_General.Image_Process(HWC2.operation_Image, disposeImageRetainLenght, HWC2);
                BIZ_General.Image_Process(HWC3.operation_Image, disposeImageRetainLenght, HWC3);
                BIZ_General.Image_Process(HWC4.operation_Image, disposeImageRetainLenght, HWC4);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }

        public void DisposeImageAndClearImageQueue()
        {
            HWC1?.Dispose();
            HWC2?.Dispose();
            HWC3?.Dispose();
            HWC4?.Dispose();
        }

        public void SimLoadImage(bool simAutoFlag, ref bool simRunFlag)
        {
            try
            {
                if (simRunFlag)
                {
                    simRunFlag = false;
                    simAutoFlag = false;
                    foreach (ProcessServices item in Project.Instance.m_ProcessList)
                    {
                        Project.Instance.StopRun(item.process.processInfo.ProcessID);
                    }


                    Parallel.ForEach(Project.Instance.baseProDevice.m_DicCamOperaParam_Sim, (item) =>
                    {
                        bool flag = SimControl.LoadImage_Sim(Project.Instance.cameraManager[(CameraEnum)item.Key].m_ImageSimManage,
                                            ref item.Value.QueueGrabImage, $"{((CameraEnum)item.Key).ToString()}仿真加载图片");
                        if (flag)
                        {
                            (item.Value as AreaCamParam).CallBackImgDataDeal();
                            //(item.Value as AreaCamParam).semaphoreSlimCam.Release();
                            string str = item.Key.ToString() + "信号量--" + (item.Value as AreaCamParam).semaphoreSlimCam.CurrentCount.ToString() + "图像数--" + item.Value.QueueGrabImage.Count.ToString();
                            //  Console.WriteLine(str);

                        }
                    });

                    //拍照信号
                    //bIZ_Variable.PhotoSignal = 1;

                    if (!simAutoFlag)
                    {
                        simRunFlag = false;
                        foreach (ProcessServices item in Project.Instance.m_ProcessList)
                        {
                            Project.Instance.ExecuteOnce(item.process.processInfo.ProcessID);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }

        public void StartMojuan(string mojuanName)
        {
            try
            {
                ReadCraft(ReadLogSaveType.True);
                if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面A极片纠偏工位)
                {
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NewFilmRoll_AAYDW.Value = 1;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NewFilmRoll_AAYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }
                else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面B极片纠偏工位)
                {
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NewFilmRoll_ABYDW.Value = 1;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NewFilmRoll_ABYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }
                else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面A极片纠偏工位)
                {
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NewFilmRoll_BAYDW.Value = 1;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NewFilmRoll_BAYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }
                else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面B极片纠偏工位)
                {
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NewFilmRoll_BBYDW.Value = 1;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NewFilmRoll_BBYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }

                InitChart();
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }

        }

        public void StopMojuan()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }

        public void StartRun()
        {
            try
            {
                if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面A极片纠偏工位)
                {
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_RunOrPause_AAYDW.Value = 1;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_RunOrPause_AAYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);


                }
                else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面B极片纠偏工位)
                {
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_RunOrPause_ABYDW.Value = 1;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_RunOrPause_ABYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);


                }
                else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面A极片纠偏工位)
                {
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_RunOrPause_BAYDW.Value = 1;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_RunOrPause_BAYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);


                }
                else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面B极片纠偏工位)
                {
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_RunOrPause_BBYDW.Value = 1;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_RunOrPause_BBYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);


                }


            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }

        }

        public void StopRun()
        {
            try
            {
                if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面A极片纠偏工位)
                {
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_RunOrPause_AAYDW.Value = 0;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_RunOrPause_AAYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }
                else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面B极片纠偏工位)
                {
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_RunOrPause_ABYDW.Value = 0;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_RunOrPause_ABYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }
                else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面A极片纠偏工位)
                {
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_RunOrPause_BAYDW.Value = 0;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_RunOrPause_BAYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }
                else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面B极片纠偏工位)
                {
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_RunOrPause_BBYDW.Value = 0;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_RunOrPause_BBYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }

        }

        public void AlarmReset()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }

        public void SendStopAlarm(int index)
        {
            try
            {
                if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面A极片纠偏工位)
                {
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NGHalt_AAYDW.Value = 1;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NGHalt_AAYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NGAlarm_AAYDW.Value = 1;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NGAlarm_AAYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }
                else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面B极片纠偏工位)
                {
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NGHalt_ABYDW.Value = 1;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NGHalt_ABYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NGAlarm_ABYDW.Value = 1;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NGAlarm_ABYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }
                else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面A极片纠偏工位)
                {
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NGHalt_BAYDW.Value = 1;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NGHalt_BAYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NGAlarm_BAYDW.Value = 1;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NGAlarm_BAYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }
                else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面B极片纠偏工位)
                {
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NGHalt_BBYDW.Value = 1;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NGHalt_BBYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NGAlarm_BBYDW.Value = 1;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NGAlarm_BBYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }

        }

        public void SendAlarm(int index)
        {
            try
            {
                if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面A极片纠偏工位)
                {

                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NGAlarm_AAYDW.Value = 1;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NGAlarm_AAYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }
                else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面B极片纠偏工位)
                {

                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NGAlarm_ABYDW.Value = 1;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NGAlarm_ABYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }
                else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面A极片纠偏工位)
                {

                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NGAlarm_BAYDW.Value = 1;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NGAlarm_BAYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }
                else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面B极片纠偏工位)
                {

                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NGAlarm_BBYDW.Value = 1;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NGAlarm_BBYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }

        }

        public void UpdateCraft()
        {
            overHangCraft = Project.Instance.craftBase as OverHangCraft;
        }
        #endregion
    }
}

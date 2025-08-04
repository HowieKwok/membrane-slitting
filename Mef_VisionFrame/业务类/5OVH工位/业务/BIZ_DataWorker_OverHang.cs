using Frame_Core;

using HalconDotNet;

using IKapBoardClassLibrary;

using LogTool;

using Mef_VisionFrame;

using NPOI.SS.Formula.Functions;
using NPOI.XSSF.Streaming.Values;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Media.Converters;

namespace Mef_VisionFram
{
    public class BIZ_DataWorker_OverHang : IBizWorkerBase
    {
        BIZ_Variable_OverHang bIZ_Variable = new BIZ_Variable_OverHang();
        Form_OverHang form;
        OverHangCraft overHangCraft;

        public DisplayWindowClass HWC1;
        public DisplayWindowClass HWC2;
        public DisplayWindowClass HWC3;
        public DisplayWindowClass HWC4;
        public DisplayWindowClass HWC5;
        public DisplayWindowClass HWC6;
        public DisplayWindowClass HWC7;
        public DisplayWindowClass HWC8;

        private BIZ_ThreadManage_OverHang bIZ_ThreadManage = new BIZ_ThreadManage_OverHang();

        ListViewAlternateColor ListViewAlternateColor = new ListViewAlternateColor() { Colors = new Color[] { Color.White, Color.FloralWhite } };
        IntervalExecution intervalExecutionA = new IntervalExecution();
        IntervalExecution intervalExecutionB = new IntervalExecution();
        IntervalExecution intervalExecutionGetDataA = new IntervalExecution();
        IntervalExecution intervalExecutionGetDatB = new IntervalExecution();
        object prodA_Lock = new object();
        object prodB_Lock = new object();
        static object prod_Lock = new object();
        static object prodAAOrCC_Lock = new object();
        object sendResult_Lock = new object();
        object Result_Lock = new object();
        static object ChartSize_Lock = new object();
        static object ChartSizeAACC_Lock = new object();
        static object AllCamData_Lock = new object();
        static object AllCamSizeData_Lock = new object();
        static object AllCamNGData_Lock = new object();
        object SaveWindowScreenshot_lockObj = new object();

        object EACornerProductLock_Cam1 = new object();
        object EACornerProductLock_Cam2 = new object();
        object EACornerProductLock_Cam3 = new object();
        object EACornerProductLock_Cam4 = new object();
        object EACornerProductLock_Cam5 = new object();
        object EACornerProductLock_Cam6 = new object();
        object EACornerProductLock_Cam7 = new object();
        object EACornerProductLock_Cam8 = new object();

        //曲线图
        ChartUIParam chartUIParam_1X;
        ChartUIParam chartUIParam_2X;
        ChartUIParam chartUIParam_3X;
        ChartUIParam chartUIParam_4X;
        ChartUIParam chartUIParam_5X;
        ChartUIParam chartUIParam_1Y;
        ChartUIParam chartUIParam_2Y;
        ChartUIParam chartUIParam_3Y;
        ChartUIParam chartUIParam_4Y;
        ChartUIParam chartUIParam_5Y;
        ChartUIParam chartUIParam_AS1;
        ChartUIParam chartUIParam_AS2;
        ChartUIParam chartUIParam_AS3;
        ChartUIParam chartUIParam_Corner1;
        ChartUIParam chartUIParam_ARC;
        ChartUIParam chartUIParam_Corner2;

        ChartUIParam chartUIParamAA_1X;
        ChartUIParam chartUIParamAA_2X;
        ChartUIParam chartUIParamAA_3X;
        ChartUIParam chartUIParamAA_4X;
        ChartUIParam chartUIParamAA_5X;
        ChartUIParam chartUIParamAA_1Y;
        ChartUIParam chartUIParamAA_2Y;
        ChartUIParam chartUIParamAA_3Y;
        ChartUIParam chartUIParamAA_5Y;

        ChartUIParam chartUIParamCC_1X;
        ChartUIParam chartUIParamCC_2X;
        ChartUIParam chartUIParamCC_3X;
        ChartUIParam chartUIParamCC_4X;
        ChartUIParam chartUIParamCC_5X;
        ChartUIParam chartUIParamCC_1Y;
        ChartUIParam chartUIParamCC_2Y;
        ChartUIParam chartUIParamCC_3Y;
        ChartUIParam chartUIParamCC_5Y;
        public BIZ_DataWorker_OverHang(Form_OverHang form_OverHang)
        {

            form = form_OverHang;
            UpdateCraft();

            HWC1 = new DisplayWindowClass(form.HWC1, form.TB_PositionAndGray1, form.PB_FitWin1, CameraEnum.Cam1, form.HWC1.Name);
            HWC2 = new DisplayWindowClass(form.HWC2, form.TB_PositionAndGray2, form.PB_FitWin2, CameraEnum.Cam2, form.HWC2.Name);
            HWC3 = new DisplayWindowClass(form.HWC3, form.TB_PositionAndGray3, form.PB_FitWin3, CameraEnum.Cam3, form.HWC3.Name);
            HWC4 = new DisplayWindowClass(form.HWC4, form.TB_PositionAndGray4, form.PB_FitWin4, CameraEnum.Cam4, form.HWC4.Name);
            HWC5 = new DisplayWindowClass(form.HWC5, form.TB_PositionAndGray5, form.PB_FitWin5, CameraEnum.Cam5, form.HWC5.Name);
            HWC6 = new DisplayWindowClass(form.HWC6, form.TB_PositionAndGray6, form.PB_FitWin6, CameraEnum.Cam6, form.HWC6.Name);
            HWC7 = new DisplayWindowClass(form.HWC7, form.TB_PositionAndGray7, form.PB_FitWin7, CameraEnum.Cam7, form.HWC7.Name);
            HWC8 = new DisplayWindowClass(form.HWC8, form.TB_PositionAndGray8, form.PB_FitWin8, CameraEnum.Cam8, form.HWC8.Name);

            InitVariable();
            InitForm();

            ThreadManage = bIZ_ThreadManage;

            bIZ_ThreadManage.InitThread();

            bIZ_ThreadManage.thd_ImageShowAndSave.Start(5, "ThdImageShowAndSave");
            bIZ_ThreadManage.thd_ImageShowAndSave.PerformWork -= ImageShowSave;
            bIZ_ThreadManage.thd_ImageShowAndSave.PerformWork += ImageShowSave;

            bIZ_ThreadManage.thd_Communication.Start(50, "ThdCommunication");
            bIZ_ThreadManage.thd_Communication.PerformWork -= Communication;
            bIZ_ThreadManage.thd_Communication.PerformWork += Communication;

        }

        public void WorkerProcessA(CamEACornerProduct camEAProduct, out bool EAResult)
        {
            EAResult = false;
            try
            {
                bIZ_Variable.UpOrDownCellA = camEAProduct.PackgeNum;
                bIZ_Variable.CurrentLayerA = camEAProduct.EAIndex;
                //==========================
                //【1】读取拍照信号                             
                if (!Project.Instance.Is_simulation)
                {
                    bIZ_Variable.PhotoSignal_A = ReadPhotoSignalA();
                    //读取当前电芯编号
                    Project.Instance.cutOverlayPLCManage.OVH_ReadUpOrDownCell();
                    //获取当前层数
                    Project.Instance.cutOverlayPLCManage.OVH_ReadCurrentLayerA();
                }

                for (int i = 0; i < 1; i++)
                {
                    if (Project.Instance.lightManager[i].LightParam.bIsEnableOverhang)
                    {
                        int layer = overHangCraft.StakeCathodeCount;
                        if (bIZ_Variable.CurrentLayerA == layer)
                        {
                            {
                                //Project.Instance.lightManager[i].Light.ChangeValue(2, Project.Instance.lightManager[i].LightParam.LightChannel1valueGroup[0]);
                                //Thread.Sleep(100);
                                Project.Instance.lightManager[i].Light.ChangeValue(1, Project.Instance.lightManager[i].LightParam.LightChannel1valueGroup[0]);
                                Thread.Sleep(5);
                                Project.Instance.lightManager[i].Light.ChangeValue(2, Project.Instance.lightManager[i].LightParam.LightChannel1valueGroup[0]);
                                Thread.Sleep(5);
                                Project.Instance.lightManager[i].Light.ChangeValue(3, Project.Instance.lightManager[i].LightParam.LightChannel1valueGroup[0]);
                                Thread.Sleep(5);
                                Project.Instance.lightManager[i].Light.ChangeValue(4, Project.Instance.lightManager[i].LightParam.LightChannel1valueGroup[0]);
                                Thread.Sleep(5);
                                Project.Instance.lightManager[i].Light.ChangeValue(5, Project.Instance.lightManager[i].LightParam.LightChannel1valueGroup[0]);
                            }
                        }

                        if (bIZ_Variable.CurrentLayerA == 1)
                        {
                            {
                                Project.Instance.lightManager[i].Light.ChangeValue(1, Project.Instance.lightManager[i].LightParam.LightChannel1valueGroup[1]);
                                Thread.Sleep(5);
                                Project.Instance.lightManager[i].Light.ChangeValue(2, Project.Instance.lightManager[i].LightParam.LightChannel1valueGroup[1]);
                                Thread.Sleep(5);
                                Project.Instance.lightManager[i].Light.ChangeValue(3, Project.Instance.lightManager[i].LightParam.LightChannel1valueGroup[1]);
                                Thread.Sleep(5);
                                Project.Instance.lightManager[i].Light.ChangeValue(4, Project.Instance.lightManager[i].LightParam.LightChannel1valueGroup[1]);
                                Thread.Sleep(5);
                                Project.Instance.lightManager[i].Light.ChangeValue(5, Project.Instance.lightManager[i].LightParam.LightChannel1valueGroup[1]);
                            }
                        }
                    }

                }

                //获取相机检测结果
                ProcessExecutionResult processExecutionResult = IntervalExecution.DelayExecute(DateTime.Now, Project.Instance.cutOverlayPLCManage.m_DevParam.TimeOutTime, GetProdResultA);

                //执行成功
                if (processExecutionResult == ProcessExecutionResult.Success)
                {
                    //获取结果                     
                    bool result = bIZ_Variable.CurrentEAResultA;

                    //返回结果
                    if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面极片复检工位)
                    {
                        Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult1_AAOVH.Value = result ? (short)1 : (short)2;
                        Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult1_AAOVH, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    }
                    else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面极片复检工位)
                    {
                        Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult1_BAOVH.Value = result ? (short)1 : (short)2;
                        Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult1_BAOVH, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    }

                }
                //执行异常
                else if (processExecutionResult == ProcessExecutionResult.Exception)
                {
                    //返回结果
                    if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面极片复检工位)
                    {
                        Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult1_AAOVH.Value = (short)2;
                        Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult1_AAOVH, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    }
                    else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面极片复检工位)
                    {
                        Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult1_BAOVH.Value = (short)2;
                        Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult1_BAOVH, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    }
                }
                //执行超时
                else if (processExecutionResult == ProcessExecutionResult.Overtime)
                {
                    Log4netRecord.RunLog.Info("OVH-A数据获取超时");
                }

                //电芯编号计数
                if (camEAProduct.EAIndex == 1)
                {
                    bIZ_Variable.CellNumber++;
                }

                string qrCode = "";
                if (camEAProduct.EAIndex == overHangCraft.StakeCathodeCount)
                {
                    qrCode = Project.Instance.cutOverlayPLCManage.OVH_ReadQRCode();
                }

                ProcessingData_AllACamData(Project.Instance.paramPathSet.OVHProdDataPath_AllCACam, OVHDetectionMethodeEnum.CA, camEAProduct.PackgeNum, camEAProduct.EAIndex, qrCode);
                ProcessingData_AllCamData_AAOrCC(Project.Instance.paramPathSet.OVHProdDataPath_AllAACam, OVHDetectionMethodeEnum.AA, qrCode);

                //临时AC表格加条码显示
                if (bIZ_Variable.CurrentLayerA == overHangCraft.StakeCathodeCount)
                {
                    lock (prod_Lock)
                    {
                        List<RowData> saveRowDataList = new List<RowData>();
                        RowData timeRowData = new RowData() { dataStr = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff"), resultType = ResultType.OK };
                        RowData QRRowData = new RowData() { dataStr = qrCode, resultType = ResultType.OK };

                        saveRowDataList.Add(timeRowData);
                        saveRowDataList.Add(QRRowData);
                        FileOperationCSV.WriteData_CSV(Project.Instance.paramPathSet.OVHProdDataPath_AllACCam, BIZ_Variable_OverHang.Title_AC.ToArray(), saveRowDataList.ToArray(), "全相机AC数据");
                    }
                }

                //汇总计算极片尺寸
                CalculateSize(out List<SizeInfo> sizeInfos, bIZ_Variable.CurrentLayerA,out bool isOK,true);
                camEAProduct.CurrenPoleWidth1 = sizeInfos[0].DataResult;
                camEAProduct.CurrenPoleWidth2 = sizeInfos[1].DataResult;
                camEAProduct.CurrenPoleHeight1= sizeInfos[2].DataResult;
                camEAProduct.CurrenPoleHeight2 = sizeInfos[3].DataResult;
                camEAProduct.PolePieceSizeResult = isOK;
                camEAProduct.showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"AW1:{sizeInfos[0].DataResult}", m_stuGetColor = sizeInfos[0].resultType==ResultType.OK?HalconColor.green: sizeInfos[0].resultType == ResultType.NG? HalconColor.red : HalconColor.yellow });
                camEAProduct.showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"AW2:{sizeInfos[1].DataResult}", m_stuGetColor = sizeInfos[1].resultType == ResultType.OK ? HalconColor.green : sizeInfos[1].resultType == ResultType.NG ? HalconColor.red : HalconColor.yellow });
                camEAProduct.showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"AH1:{sizeInfos[2].DataResult}", m_stuGetColor = sizeInfos[2].resultType == ResultType.OK ? HalconColor.green : sizeInfos[2].resultType == ResultType.NG ? HalconColor.red : HalconColor.yellow } );
                camEAProduct.showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"AH2:{sizeInfos[3].DataResult}", m_stuGetColor = sizeInfos[3].resultType == ResultType.OK ? HalconColor.green : sizeInfos[3].resultType == ResultType.NG ? HalconColor.red : HalconColor.yellow });
                WritePieSizeData(Project.Instance.paramPathSet.OVHProdDataPath_PieSize, camEAProduct, sizeInfos, bIZ_Variable.CellNumber, camEAProduct.PackgeNum, camEAProduct.EAIndex, "");


                //数据汇总
                CamEACornerProduct currentCamEAProduct1 = GetCamLastOneEAData(CameraEnum.Cam1);
                CamEACornerProduct currentCamEAProduct2 = GetCamLastOneEAData(CameraEnum.Cam2);
                CamEACornerProduct currentCamEAProduct3 = GetCamLastOneEAData(CameraEnum.Cam3);
                CamEACornerProduct currentCamEAProduct4 = GetCamLastOneEAData(CameraEnum.Cam4);
                CamEACornerProduct currentCamEAProduct5 = GetCamLastOneEAData(CameraEnum.Cam5);
                CamEACornerProduct currentCamEAProduct6 = GetCamLastOneEAData(CameraEnum.Cam6);
                CamEACornerProduct currentCamEAProduct7 = GetCamLastOneEAData(CameraEnum.Cam7);
                CamEACornerProduct currentCamEAProduct8 = GetCamLastOneEAData(CameraEnum.Cam8);
                WriteData_AllCamData(Project.Instance.paramPathSet.OVHProdDataPath_AllCam, currentCamEAProduct1, currentCamEAProduct2, currentCamEAProduct3, currentCamEAProduct4,
                    bIZ_Variable.CellNumber, camEAProduct.PackgeNum, camEAProduct.EAIndex, qrCode, out EAResult, out string resultStr);

                

                //NG数据汇总
                if (!EAResult)
                {
                    WriteNGData_AllCamData(Project.Instance.paramPathSet.OVHProdNGDataPath_AllCam, bIZ_Variable.CellNumber, camEAProduct.PackgeNum, camEAProduct.EAIndex, resultStr,
                    currentCamEAProduct1?.ScreenshotSaveRelativePath, currentCamEAProduct2?.ScreenshotSaveRelativePath, currentCamEAProduct3?.ScreenshotSaveRelativePath, currentCamEAProduct4?.ScreenshotSaveRelativePath,
                    currentCamEAProduct5?.ScreenshotSaveRelativePath, currentCamEAProduct6?.ScreenshotSaveRelativePath, currentCamEAProduct7?.ScreenshotSaveRelativePath, currentCamEAProduct8?.ScreenshotSaveRelativePath,
                    currentCamEAProduct1?.PrimitiveImgSaveRelativePath, currentCamEAProduct2?.PrimitiveImgSaveRelativePath, currentCamEAProduct3?.PrimitiveImgSaveRelativePath, currentCamEAProduct4?.PrimitiveImgSaveRelativePath,
                    currentCamEAProduct5?.PrimitiveImgSaveRelativePath, currentCamEAProduct6?.PrimitiveImgSaveRelativePath, currentCamEAProduct7?.PrimitiveImgSaveRelativePath, currentCamEAProduct8?.PrimitiveImgSaveRelativePath);
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }

        }

        public void WorkerProcessB(CamEACornerProduct camEAProduct, out bool EAResult)
        {
            EAResult = false;
            try
            {
                bIZ_Variable.UpOrDownCellB = camEAProduct.PackgeNum;
                bIZ_Variable.CurrentLayerB = camEAProduct.EAIndex;
                //==========================
                //【1】读取拍照信号
                if (!Project.Instance.Is_simulation)
                {
                    bIZ_Variable.PhotoSignal_B = ReadPhotoSignalB();
                    //读取当前电芯编号
                    Project.Instance.cutOverlayPLCManage.OVH_ReadUpOrDownCell();
                    //获取当前层数
                    Project.Instance.cutOverlayPLCManage.OVH_ReadCurrentLayerB();
                }


                for (int i = 0; i < 1; i++)
                {
                    if (Project.Instance.lightManager[i].LightParam.bIsEnableOverhang)
                    {
                        int layer = overHangCraft.StakeCathodeCount - 1;
                        if (bIZ_Variable.CurrentLayerB == layer)
                        {

                            Project.Instance.lightManager[i].Light.ChangeValue(1, Project.Instance.lightManager[i].LightParam.LightChannel1valueGroup[2]);
                            Thread.Sleep(millisecondsTimeout: 5);
                            Project.Instance.lightManager[i].Light.ChangeValue(2, Project.Instance.lightManager[i].LightParam.LightChannel1valueGroup[2]);
                            Thread.Sleep(5);
                            Project.Instance.lightManager[i].Light.ChangeValue(3, Project.Instance.lightManager[i].LightParam.LightChannel1valueGroup[2]);
                            Thread.Sleep(5);
                            Project.Instance.lightManager[i].Light.ChangeValue(4, Project.Instance.lightManager[i].LightParam.LightChannel1valueGroup[2]);
                            Thread.Sleep(5);
                            Project.Instance.lightManager[i].Light.ChangeValue(5, Project.Instance.lightManager[i].LightParam.LightChannel1valueGroup[2]);
                        }


                    }

                }


                //获取相机检测结果
                ProcessExecutionResult processExecutionResult = IntervalExecution.DelayExecute(DateTime.Now, Project.Instance.cutOverlayPLCManage.m_DevParam.TimeOutTime, GetProdResultB);

                //执行成功
                if (processExecutionResult == ProcessExecutionResult.Success)
                {
                    //获取结果
                    bool result = bIZ_Variable.CurrentEAResultB;

                    //返回结果
                    if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面极片复检工位)
                    {
                        Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult1_ABOVH.Value = result ? (short)1 : (short)2;
                        Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult1_ABOVH, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    }
                    else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面极片复检工位)
                    {
                        Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult1_BBOVH.Value = result ? (short)1 : (short)2;
                        Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult1_BBOVH, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    }

                }
                //执行异常
                else if (processExecutionResult == ProcessExecutionResult.Exception)
                {
                    //返回结果
                    if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面极片复检工位)
                    {
                        Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult1_ABOVH.Value = (short)2;
                        Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult1_ABOVH, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    }
                    else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面极片复检工位)
                    {
                        Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult1_BBOVH.Value = (short)2;
                        Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult1_BBOVH, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    }
                }
                //执行超时
                else if (processExecutionResult == ProcessExecutionResult.Overtime)
                {
                    Log4netRecord.RunLog.Info("OVH-B数据获取超时");
                }



                ProcessingData_AllACamData(Project.Instance.paramPathSet.OVHProdDataPath_AllACCam, OVHDetectionMethodeEnum.AC, camEAProduct.PackgeNum, camEAProduct.EAIndex, "");
                ProcessingData_AllCamData_AAOrCC(Project.Instance.paramPathSet.OVHProdDataPath_AllCCCam, OVHDetectionMethodeEnum.CC, "");


                //汇总计算极片尺寸
                CalculateSize(out List<SizeInfo> sizeInfos, bIZ_Variable.CurrentLayerA, out bool isNG, false);
                camEAProduct.CurrenPoleWidth1 = sizeInfos[0].DataResult;
                camEAProduct.CurrenPoleWidth2 = sizeInfos[1].DataResult;
                camEAProduct.CurrenPoleHeight1 = sizeInfos[2].DataResult;
                camEAProduct.CurrenPoleHeight2 = sizeInfos[3].DataResult;

                camEAProduct.showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"AW1:{sizeInfos[0].DataResult}", m_stuGetColor = sizeInfos[0].resultType == ResultType.OK ? HalconColor.green : sizeInfos[0].resultType == ResultType.NG ? HalconColor.red : HalconColor.yellow });
                camEAProduct.showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"AW2:{sizeInfos[1].DataResult}", m_stuGetColor = sizeInfos[1].resultType == ResultType.OK ? HalconColor.green : sizeInfos[1].resultType == ResultType.NG ? HalconColor.red : HalconColor.yellow });
                camEAProduct.showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"AH1:{sizeInfos[2].DataResult}", m_stuGetColor = sizeInfos[2].resultType == ResultType.OK ? HalconColor.green : sizeInfos[2].resultType == ResultType.NG ? HalconColor.red : HalconColor.yellow });
                camEAProduct.showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"AH2:{sizeInfos[3].DataResult}", m_stuGetColor = sizeInfos[3].resultType == ResultType.OK ? HalconColor.green : sizeInfos[3].resultType == ResultType.NG ? HalconColor.red : HalconColor.yellow });
                WritePieSizeData(Project.Instance.paramPathSet.OVHProdDataPath_PieSize, camEAProduct, sizeInfos, bIZ_Variable.CellNumber, camEAProduct.PackgeNum, camEAProduct.EAIndex, "");


                //数据汇总
                CamEACornerProduct currentCamEAProduct1 = GetCamLastOneEAData(CameraEnum.Cam1);
                CamEACornerProduct currentCamEAProduct2 = GetCamLastOneEAData(CameraEnum.Cam2);
                CamEACornerProduct currentCamEAProduct3 = GetCamLastOneEAData(CameraEnum.Cam3);
                CamEACornerProduct currentCamEAProduct4 = GetCamLastOneEAData(CameraEnum.Cam4);
                CamEACornerProduct currentCamEAProduct5 = GetCamLastOneEAData(CameraEnum.Cam5);
                CamEACornerProduct currentCamEAProduct6 = GetCamLastOneEAData(CameraEnum.Cam6);
                CamEACornerProduct currentCamEAProduct7 = GetCamLastOneEAData(CameraEnum.Cam7);
                CamEACornerProduct currentCamEAProduct8 = GetCamLastOneEAData(CameraEnum.Cam8);
                WriteData_AllCamData(Project.Instance.paramPathSet.OVHProdDataPath_AllCam, currentCamEAProduct5, currentCamEAProduct6, currentCamEAProduct7, currentCamEAProduct8,
                    bIZ_Variable.CellNumber, camEAProduct.PackgeNum, camEAProduct.EAIndex, "", out EAResult, out string resultStr);

                //NG数据汇总
                if (!EAResult)
                {
                    WriteNGData_AllCamData(Project.Instance.paramPathSet.OVHProdNGDataPath_AllCam, bIZ_Variable.CellNumber, camEAProduct.PackgeNum, camEAProduct.EAIndex, resultStr,
                   currentCamEAProduct1?.ScreenshotSaveRelativePath, currentCamEAProduct2?.ScreenshotSaveRelativePath, currentCamEAProduct3?.ScreenshotSaveRelativePath, currentCamEAProduct4?.ScreenshotSaveRelativePath,
                   currentCamEAProduct5?.ScreenshotSaveRelativePath, currentCamEAProduct6?.ScreenshotSaveRelativePath, currentCamEAProduct7?.ScreenshotSaveRelativePath, currentCamEAProduct8?.ScreenshotSaveRelativePath,
                   currentCamEAProduct1?.PrimitiveImgSaveRelativePath, currentCamEAProduct2?.PrimitiveImgSaveRelativePath, currentCamEAProduct3?.PrimitiveImgSaveRelativePath, currentCamEAProduct4?.PrimitiveImgSaveRelativePath,
                   currentCamEAProduct5?.PrimitiveImgSaveRelativePath, currentCamEAProduct6?.PrimitiveImgSaveRelativePath, currentCamEAProduct7?.PrimitiveImgSaveRelativePath, currentCamEAProduct8?.PrimitiveImgSaveRelativePath);
                }

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }

        }

        public void ProcessingData_Cam1(ProductBase camEAProduct1)
        {

            try
            {
                HOperatorSet.CountSeconds(out camEAProduct1.Image_StartDataProcessTime);
                CameraEnum cameraEnum = CameraEnum.Cam1;
                //当前相机片数据
                CamEACornerProduct camEAProduct = camEAProduct1 as CamEACornerProduct;
                //当前相机第一片数据
                CamEACornerProduct FirstEA = GetCamData_Cam1(camEAProduct.PackgeNum, 1);
                //当前相机倒数第二片数据
                CamEACornerProduct SecondToLast = GetCamData_Cam1(camEAProduct.PackgeNum, camEAProduct.EAIndex - 2);
                //对应相机前一片数据
                CamEACornerProduct LastEA = GetCamData_Cam5(camEAProduct.PackgeNum, camEAProduct.EAIndex - 1);

                List<CamEACornerProduct> EACornerProduct_CurrentCam = bIZ_Variable.batteryProduct.EACornerProduct_Cam1;

                //================================================================
                //【1】计算数据
                camEAProduct.m_lineShowInfos = camEAProduct.showHalconWindow.m_lineShowInfos.ToList();

                //================================================================
                //【2】数据处理
                List<SizeItemInfo> sizeItemInfo = new List<SizeItemInfo>();
                sizeItemInfo.AddRange(overHangCraft.ListSizeEdits[(int)cameraEnum]);
                sizeItemInfo.AddRange(overHangCraft.ListSizeEdits[(int)CameraEnum.Cam5]);

                ProcessingData_Cam(cameraEnum, overHangCraft, ElectrodeEnum.阳极, sizeItemInfo,
                    FirstEA, camEAProduct, SecondToLast, LastEA,
                    form.listView1, Project.Instance.paramPathSet.OVHProdDataPath_Cam1, HWC1, ref bIZ_Variable.ChartSizeDetectionItemData,
                    ref bIZ_Variable.ChartSizeDetectionItemData_AA, ref bIZ_Variable.ChartSizeDetectionItemData_CC);
                BIZ_General.AddMessageShowWindow(camEAProduct.sizeInfos, camEAProduct.showHalconWindow);

                //电芯结果判断
                PackgeResultJudgment(camEAProduct);


                lock (EACornerProductLock_Cam1)
                {
                    EACornerProduct_CurrentCam.Add(camEAProduct);
                }

                //================================================================
                //【4】原图显示、保存
                bool result = camEAProduct.DiaphragmResult && camEAProduct.PolePieceResult;
                PrimitiveTransition(cameraEnum, camEAProduct.showHalconWindow, result, camEAProduct.showHalconWindow.m_nImageFrameID, camEAProduct.PackgeNum, camEAProduct.EAIndex, out Operation_ImageInfo operation_ImageInfo);

                //EA片处理完成
                camEAProduct.PrimitiveImgSaveRelativePath = operation_ImageInfo.str_ImageSaveRelativePath + operation_ImageInfo.str_ImageSaveName;
                camEAProduct.ScreenshotSaveRelativePath = operation_ImageInfo.str_ScreenshotSaveRelativePath + operation_ImageInfo.str_ScreenshotSaveName;
                camEAProduct.CamFinish = true;

                //综合数据
                WorkerProcessA(camEAProduct, out bool EAResult);
               
                HOperatorSet.CountSeconds(out camEAProduct1.Image_DataProcessTime);
                HOperatorSet.CountSeconds(out camEAProduct1.Image_EndTime);
                //计算时间
                BIZ_General.CalculateProcessingTime(camEAProduct1);
                HWC1.operation_Image.imageInfosShow.Enqueue(operation_ImageInfo);

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }

        public void ProcessingData_Cam2(ProductBase camEAProduct1)
        {

            try
            {
                HOperatorSet.CountSeconds(out camEAProduct1.Image_StartDataProcessTime);
                CameraEnum cameraEnum = CameraEnum.Cam2;

                //当前相机片数据
                CamEACornerProduct camEAProduct = camEAProduct1 as CamEACornerProduct;
                //当前相机第一片数据
                CamEACornerProduct FirstEA = GetCamData_Cam2(camEAProduct.PackgeNum, 1);
                //当前相机倒数第二片数据
                CamEACornerProduct SecondToLast = GetCamData_Cam2(camEAProduct.PackgeNum, camEAProduct.EAIndex - 2);
                //对应相机前一片数据
                CamEACornerProduct LastEA = GetCamData_Cam6(camEAProduct.PackgeNum, camEAProduct.EAIndex - 1);

                List<CamEACornerProduct> EACornerProduct_CurrentCam = bIZ_Variable.batteryProduct.EACornerProduct_Cam2;

                //================================================================
                //【1】计算数据
                camEAProduct.m_lineShowInfos = camEAProduct.showHalconWindow.m_lineShowInfos.ToList();

                //================================================================
                //【2】数据处理
                List<SizeItemInfo> sizeItemInfo = new List<SizeItemInfo>();
                sizeItemInfo.AddRange(overHangCraft.ListSizeEdits[(int)cameraEnum]);
                sizeItemInfo.AddRange(overHangCraft.ListSizeEdits[(int)CameraEnum.Cam6]);

                ProcessingData_Cam(cameraEnum, overHangCraft, ElectrodeEnum.阳极, sizeItemInfo,
                       FirstEA, camEAProduct, SecondToLast, LastEA,
                    form.listView2, Project.Instance.paramPathSet.OVHProdDataPath_Cam2, HWC2, ref bIZ_Variable.ChartSizeDetectionItemData,
                     ref bIZ_Variable.ChartSizeDetectionItemData_AA, ref bIZ_Variable.ChartSizeDetectionItemData_CC);
                BIZ_General.AddMessageShowWindow(camEAProduct.sizeInfos, camEAProduct.showHalconWindow);

                //电芯结果判断
                PackgeResultJudgment(camEAProduct);


                lock (EACornerProductLock_Cam2)
                {
                    EACornerProduct_CurrentCam.Add(camEAProduct);
                }


                //================================================================
                //【4】原图显示、保存
                bool result = camEAProduct.DiaphragmResult && camEAProduct.PolePieceResult;
                PrimitiveTransition(cameraEnum, camEAProduct.showHalconWindow, result, camEAProduct.showHalconWindow.m_nImageFrameID, camEAProduct.PackgeNum, camEAProduct.EAIndex, out Operation_ImageInfo operation_ImageInfo);

                //EA片处理完成
                camEAProduct.PrimitiveImgSaveRelativePath = operation_ImageInfo.str_ImageSaveRelativePath + operation_ImageInfo.str_ImageSaveName;
                camEAProduct.ScreenshotSaveRelativePath = operation_ImageInfo.str_ScreenshotSaveRelativePath + operation_ImageInfo.str_ScreenshotSaveName;
                camEAProduct.CamFinish = true;

                HOperatorSet.CountSeconds(out camEAProduct1.Image_DataProcessTime);
                HOperatorSet.CountSeconds(out camEAProduct1.Image_EndTime);
                //计算时间
                BIZ_General.CalculateProcessingTime(camEAProduct1);
                HWC2.operation_Image.imageInfosShow.Enqueue(operation_ImageInfo);

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }

        public void ProcessingData_Cam3(ProductBase camEAProduct1)
        {

            try
            {
                HOperatorSet.CountSeconds(out camEAProduct1.Image_StartDataProcessTime);
                CameraEnum cameraEnum = CameraEnum.Cam3;

                //当前相机片数据
                CamEACornerProduct camEAProduct = camEAProduct1 as CamEACornerProduct;
                //当前相机第一片数据
                CamEACornerProduct FirstEA = GetCamData_Cam3(camEAProduct.PackgeNum, 1);
                //当前相机倒数第二片数据
                CamEACornerProduct SecondToLast = GetCamData_Cam3(camEAProduct.PackgeNum, camEAProduct.EAIndex - 2);
                //对应相机前一片数据
                CamEACornerProduct LastEA = GetCamData_Cam7(camEAProduct.PackgeNum, camEAProduct.EAIndex - 1);

                List<CamEACornerProduct> EACornerProduct_CurrentCam = bIZ_Variable.batteryProduct.EACornerProduct_Cam3;

                //================================================================
                //【1】计算数据
                camEAProduct.m_lineShowInfos = camEAProduct.showHalconWindow.m_lineShowInfos.ToList();

                //================================================================
                //【2】数据处理
                List<SizeItemInfo> sizeItemInfo = new List<SizeItemInfo>();
                sizeItemInfo.AddRange(overHangCraft.ListSizeEdits[(int)cameraEnum]);
                sizeItemInfo.AddRange(overHangCraft.ListSizeEdits[(int)CameraEnum.Cam7]);

                ProcessingData_Cam(cameraEnum, overHangCraft, ElectrodeEnum.阳极, sizeItemInfo,
                       FirstEA, camEAProduct, SecondToLast, LastEA,
                    form.listView3, Project.Instance.paramPathSet.OVHProdDataPath_Cam3, HWC3, ref bIZ_Variable.ChartSizeDetectionItemData,
                     ref bIZ_Variable.ChartSizeDetectionItemData_AA, ref bIZ_Variable.ChartSizeDetectionItemData_CC);
                BIZ_General.AddMessageShowWindow(camEAProduct.sizeInfos, camEAProduct.showHalconWindow);

                //电芯结果判断
                PackgeResultJudgment(camEAProduct);


                lock (EACornerProductLock_Cam3)
                {
                    EACornerProduct_CurrentCam.Add(camEAProduct);
                }


                //================================================================
                //【4】原图显示、保存
                bool result = camEAProduct.DiaphragmResult && camEAProduct.PolePieceResult;
                PrimitiveTransition(cameraEnum, camEAProduct.showHalconWindow, result, camEAProduct.showHalconWindow.m_nImageFrameID, camEAProduct.PackgeNum, camEAProduct.EAIndex, out Operation_ImageInfo operation_ImageInfo);

                //EA片处理完成
                camEAProduct.PrimitiveImgSaveRelativePath = operation_ImageInfo.str_ImageSaveRelativePath + operation_ImageInfo.str_ImageSaveName;
                camEAProduct.ScreenshotSaveRelativePath = operation_ImageInfo.str_ScreenshotSaveRelativePath + operation_ImageInfo.str_ScreenshotSaveName;
                camEAProduct.CamFinish = true;

                HOperatorSet.CountSeconds(out camEAProduct1.Image_DataProcessTime);
                HOperatorSet.CountSeconds(out camEAProduct1.Image_EndTime);
                //计算时间
                BIZ_General.CalculateProcessingTime(camEAProduct1);
                HWC3.operation_Image.imageInfosShow.Enqueue(operation_ImageInfo);

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }

        public void ProcessingData_Cam4(ProductBase camEAProduct1)
        {

            try
            {
                HOperatorSet.CountSeconds(out camEAProduct1.Image_StartDataProcessTime);
                CameraEnum cameraEnum = CameraEnum.Cam4;

                //当前相机片数据
                CamEACornerProduct camEAProduct = camEAProduct1 as CamEACornerProduct;
                //当前相机第一片数据
                CamEACornerProduct FirstEA = GetCamData_Cam4(camEAProduct.PackgeNum, 1);
                //当前相机倒数第二片数据
                CamEACornerProduct SecondToLast = GetCamData_Cam4(camEAProduct.PackgeNum, camEAProduct.EAIndex - 2);
                //对应相机前一片数据
                CamEACornerProduct LastEA = GetCamData_Cam8(camEAProduct.PackgeNum, camEAProduct.EAIndex - 1);

                List<CamEACornerProduct> EACornerProduct_CurrentCam = bIZ_Variable.batteryProduct.EACornerProduct_Cam4;

                //================================================================
                //【1】计算数据
                camEAProduct.m_lineShowInfos = camEAProduct.showHalconWindow.m_lineShowInfos.ToList();

                //================================================================
                //【2】数据处理
                List<SizeItemInfo> sizeItemInfo = new List<SizeItemInfo>();
                sizeItemInfo.AddRange(overHangCraft.ListSizeEdits[(int)cameraEnum]);
                sizeItemInfo.AddRange(overHangCraft.ListSizeEdits[(int)CameraEnum.Cam8]);

                ProcessingData_Cam(cameraEnum, overHangCraft, ElectrodeEnum.阳极, sizeItemInfo,
                       FirstEA, camEAProduct, SecondToLast, LastEA,
                    form.listView4, Project.Instance.paramPathSet.OVHProdDataPath_Cam4, HWC4, ref bIZ_Variable.ChartSizeDetectionItemData,
                     ref bIZ_Variable.ChartSizeDetectionItemData_AA, ref bIZ_Variable.ChartSizeDetectionItemData_CC);
                BIZ_General.AddMessageShowWindow(camEAProduct.sizeInfos, camEAProduct.showHalconWindow);

                //电芯结果判断
                PackgeResultJudgment(camEAProduct);


                lock (EACornerProductLock_Cam4)
                {
                    EACornerProduct_CurrentCam.Add(camEAProduct);
                }


                //================================================================
                //【4】原图显示、保存
                bool result = camEAProduct.DiaphragmResult && camEAProduct.PolePieceResult;
                PrimitiveTransition(cameraEnum, camEAProduct.showHalconWindow, result, camEAProduct.showHalconWindow.m_nImageFrameID, camEAProduct.PackgeNum, camEAProduct.EAIndex, out Operation_ImageInfo operation_ImageInfo);

                //EA片处理完成
                camEAProduct.PrimitiveImgSaveRelativePath = operation_ImageInfo.str_ImageSaveRelativePath + operation_ImageInfo.str_ImageSaveName;
                camEAProduct.ScreenshotSaveRelativePath = operation_ImageInfo.str_ScreenshotSaveRelativePath + operation_ImageInfo.str_ScreenshotSaveName;
                camEAProduct.CamFinish = true;

                HOperatorSet.CountSeconds(out camEAProduct1.Image_DataProcessTime);
                HOperatorSet.CountSeconds(out camEAProduct1.Image_EndTime);
                //计算时间
                BIZ_General.CalculateProcessingTime(camEAProduct1);
                HWC4.operation_Image.imageInfosShow.Enqueue(operation_ImageInfo);

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }

        public void ProcessingData_Cam5(ProductBase camEAProduct1)
        {

            try
            {
                HOperatorSet.CountSeconds(out camEAProduct1.Image_StartDataProcessTime);
                CameraEnum cameraEnum = CameraEnum.Cam5;

                //当前相机片数据
                CamEACornerProduct camEAProduct = camEAProduct1 as CamEACornerProduct;
                //当前相机第一片数据
                CamEACornerProduct FirstEA = GetCamData_Cam5(camEAProduct.PackgeNum, 1);
                //当前相机倒数第二片数据
                CamEACornerProduct SecondToLast = GetCamData_Cam5(camEAProduct.PackgeNum, camEAProduct.EAIndex - 2);
                //对应相机前一片数据
                CamEACornerProduct LastEA = GetCamData_Cam1(camEAProduct.PackgeNum, camEAProduct.EAIndex - 1);

                List<CamEACornerProduct> EACornerProduct_CurrentCam = bIZ_Variable.batteryProduct.EACornerProduct_Cam5;

                //================================================================
                //【1】计算数据
                camEAProduct.m_lineShowInfos = camEAProduct.showHalconWindow.m_lineShowInfos.ToList();

                //================================================================
                //【2】数据处理            
                ProcessingData_Cam(cameraEnum, overHangCraft, ElectrodeEnum.阴极, overHangCraft.ListSizeEdits[(int)cameraEnum],
                       FirstEA, camEAProduct, SecondToLast, LastEA,
                    form.listView5, Project.Instance.paramPathSet.OVHProdDataPath_Cam5, HWC5, ref bIZ_Variable.ChartSizeDetectionItemData,
                     ref bIZ_Variable.ChartSizeDetectionItemData_AA, ref bIZ_Variable.ChartSizeDetectionItemData_CC);
                BIZ_General.AddMessageShowWindow(camEAProduct.sizeInfos, camEAProduct.showHalconWindow);

                //电芯结果判断
                PackgeResultJudgment(camEAProduct);


                lock (EACornerProductLock_Cam5)
                {
                    EACornerProduct_CurrentCam.Add(camEAProduct);
                }

                //================================================================
                //【4】原图显示、保存
                bool result = camEAProduct.DiaphragmResult && camEAProduct.PolePieceResult;
                PrimitiveTransition(cameraEnum, camEAProduct.showHalconWindow, result, camEAProduct.showHalconWindow.m_nImageFrameID, camEAProduct.PackgeNum, camEAProduct.EAIndex, out Operation_ImageInfo operation_ImageInfo);

                //EA片处理完成
                camEAProduct.PrimitiveImgSaveRelativePath = operation_ImageInfo.str_ImageSaveRelativePath + operation_ImageInfo.str_ImageSaveName;
                camEAProduct.ScreenshotSaveRelativePath = operation_ImageInfo.str_ScreenshotSaveRelativePath + operation_ImageInfo.str_ScreenshotSaveName;
                camEAProduct.CamFinish = true;

                //综合数据
                WorkerProcessB(camEAProduct, out bool EAResult);
               
                HOperatorSet.CountSeconds(out camEAProduct1.Image_DataProcessTime);
                HOperatorSet.CountSeconds(out camEAProduct1.Image_EndTime);
                //计算时间
                BIZ_General.CalculateProcessingTime(camEAProduct1);
                HWC5.operation_Image.imageInfosShow.Enqueue(operation_ImageInfo);


            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }

        public void ProcessingData_Cam6(ProductBase camEAProduct1)
        {

            try
            {
                HOperatorSet.CountSeconds(out camEAProduct1.Image_StartDataProcessTime);
                CameraEnum cameraEnum = CameraEnum.Cam6;


                //当前相机片数据
                CamEACornerProduct camEAProduct = camEAProduct1 as CamEACornerProduct;
                //当前相机第一片数据
                CamEACornerProduct FirstEA = GetCamData_Cam6(camEAProduct.PackgeNum, 1);
                //当前相机倒数第二片数据
                CamEACornerProduct SecondToLast = GetCamData_Cam6(camEAProduct.PackgeNum, camEAProduct.EAIndex - 2);
                //对应相机前一片数据
                CamEACornerProduct LastEA = GetCamData_Cam2(camEAProduct.PackgeNum, camEAProduct.EAIndex - 1);

                List<CamEACornerProduct> EACornerProduct_CurrentCam = bIZ_Variable.batteryProduct.EACornerProduct_Cam6;

                //================================================================
                //【1】计算数据
                camEAProduct.m_lineShowInfos = camEAProduct.showHalconWindow.m_lineShowInfos.ToList();

                //================================================================
                //【2】数据处理                           
                ProcessingData_Cam(cameraEnum, overHangCraft, ElectrodeEnum.阴极, overHangCraft.ListSizeEdits[(int)cameraEnum],
                       FirstEA, camEAProduct, SecondToLast, LastEA,
                    form.listView6, Project.Instance.paramPathSet.OVHProdDataPath_Cam6, HWC6, ref bIZ_Variable.ChartSizeDetectionItemData,
                     ref bIZ_Variable.ChartSizeDetectionItemData_AA, ref bIZ_Variable.ChartSizeDetectionItemData_CC);
                BIZ_General.AddMessageShowWindow(camEAProduct.sizeInfos, camEAProduct.showHalconWindow);

                //电芯结果判断
                PackgeResultJudgment(camEAProduct);


                lock (EACornerProductLock_Cam6)
                {
                    EACornerProduct_CurrentCam.Add(camEAProduct);
                }

                //================================================================
                //【4】原图显示、保存
                bool result = camEAProduct.DiaphragmResult && camEAProduct.PolePieceResult;
                PrimitiveTransition(cameraEnum, camEAProduct.showHalconWindow, result, camEAProduct.showHalconWindow.m_nImageFrameID, camEAProduct.PackgeNum, camEAProduct.EAIndex, out Operation_ImageInfo operation_ImageInfo);

                //EA片处理完成
                camEAProduct.PrimitiveImgSaveRelativePath = operation_ImageInfo.str_ImageSaveRelativePath + operation_ImageInfo.str_ImageSaveName;
                camEAProduct.ScreenshotSaveRelativePath = operation_ImageInfo.str_ScreenshotSaveRelativePath + operation_ImageInfo.str_ScreenshotSaveName;
                camEAProduct.CamFinish = true;

                HOperatorSet.CountSeconds(out camEAProduct1.Image_DataProcessTime);
                HOperatorSet.CountSeconds(out camEAProduct1.Image_EndTime);
                //计算时间
                BIZ_General.CalculateProcessingTime(camEAProduct1);
                HWC6.operation_Image.imageInfosShow.Enqueue(operation_ImageInfo);

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }

        public void ProcessingData_Cam7(ProductBase camEAProduct1)
        {

            try
            {
                HOperatorSet.CountSeconds(out camEAProduct1.Image_StartDataProcessTime);
                CameraEnum cameraEnum = CameraEnum.Cam7;


                //当前相机片数据
                CamEACornerProduct camEAProduct = camEAProduct1 as CamEACornerProduct;
                //当前相机第一片数据
                CamEACornerProduct FirstEA = GetCamData_Cam7(camEAProduct.PackgeNum, 1);
                //当前相机倒数第二片数据
                CamEACornerProduct SecondToLast = GetCamData_Cam7(camEAProduct.PackgeNum, camEAProduct.EAIndex - 2);
                //对应相机前一片数据
                CamEACornerProduct LastEA = GetCamData_Cam3(camEAProduct.PackgeNum, camEAProduct.EAIndex - 1);

                List<CamEACornerProduct> EACornerProduct_CurrentCam = bIZ_Variable.batteryProduct.EACornerProduct_Cam7;

                //================================================================
                //【1】计算数据
                camEAProduct.m_lineShowInfos = camEAProduct.showHalconWindow.m_lineShowInfos.ToList();

                //================================================================
                //【2】数据处理                           
                ProcessingData_Cam(cameraEnum, overHangCraft, ElectrodeEnum.阴极, overHangCraft.ListSizeEdits[(int)cameraEnum],
                       FirstEA, camEAProduct, SecondToLast, LastEA,
                    form.listView7, Project.Instance.paramPathSet.OVHProdDataPath_Cam7, HWC7, ref bIZ_Variable.ChartSizeDetectionItemData,
                     ref bIZ_Variable.ChartSizeDetectionItemData_AA, ref bIZ_Variable.ChartSizeDetectionItemData_CC);
                BIZ_General.AddMessageShowWindow(camEAProduct.sizeInfos, camEAProduct.showHalconWindow);


                //电芯结果判断
                PackgeResultJudgment(camEAProduct);


                lock (EACornerProductLock_Cam7)
                {
                    EACornerProduct_CurrentCam.Add(camEAProduct);
                }

                //================================================================
                //【4】原图显示、保存
                bool result = camEAProduct.DiaphragmResult && camEAProduct.PolePieceResult;
                PrimitiveTransition(cameraEnum, camEAProduct.showHalconWindow, result, camEAProduct.showHalconWindow.m_nImageFrameID, camEAProduct.PackgeNum, camEAProduct.EAIndex, out Operation_ImageInfo operation_ImageInfo);

                //EA片处理完成
                camEAProduct.PrimitiveImgSaveRelativePath = operation_ImageInfo.str_ImageSaveRelativePath + operation_ImageInfo.str_ImageSaveName;
                camEAProduct.ScreenshotSaveRelativePath = operation_ImageInfo.str_ScreenshotSaveRelativePath + operation_ImageInfo.str_ScreenshotSaveName;
                camEAProduct.CamFinish = true;

                HOperatorSet.CountSeconds(out camEAProduct1.Image_DataProcessTime);
                HOperatorSet.CountSeconds(out camEAProduct1.Image_EndTime);
                //计算时间
                BIZ_General.CalculateProcessingTime(camEAProduct1);
                HWC7.operation_Image.imageInfosShow.Enqueue(operation_ImageInfo);

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }

        public void ProcessingData_Cam8(ProductBase camEAProduct1)
        {

            try
            {
                HOperatorSet.CountSeconds(out camEAProduct1.Image_StartDataProcessTime);
                CameraEnum cameraEnum = CameraEnum.Cam8;


                //当前相机片数据
                CamEACornerProduct camEAProduct = camEAProduct1 as CamEACornerProduct;
                //当前相机第一片数据
                CamEACornerProduct FirstEA = GetCamData_Cam8(camEAProduct.PackgeNum, 1);
                //当前相机倒数第二片数据
                CamEACornerProduct SecondToLast = GetCamData_Cam8(camEAProduct.PackgeNum, camEAProduct.EAIndex - 2);
                //对应相机前一片数据
                CamEACornerProduct LastEA = GetCamData_Cam4(camEAProduct.PackgeNum, camEAProduct.EAIndex - 1);

                List<CamEACornerProduct> EACornerProduct_CurrentCam = bIZ_Variable.batteryProduct.EACornerProduct_Cam8;

                //================================================================
                //【1】计算数据
                camEAProduct.m_lineShowInfos = camEAProduct.showHalconWindow.m_lineShowInfos.ToList();

                //================================================================
                //【2】数据处理                           
                ProcessingData_Cam(cameraEnum, overHangCraft, ElectrodeEnum.阴极, overHangCraft.ListSizeEdits[(int)cameraEnum],
                       FirstEA, camEAProduct, SecondToLast, LastEA,
                    form.listView8, Project.Instance.paramPathSet.OVHProdDataPath_Cam8, HWC8, ref bIZ_Variable.ChartSizeDetectionItemData,
                     ref bIZ_Variable.ChartSizeDetectionItemData_AA, ref bIZ_Variable.ChartSizeDetectionItemData_CC);
                BIZ_General.AddMessageShowWindow(camEAProduct.sizeInfos, camEAProduct.showHalconWindow);

                //电芯结果判断
                PackgeResultJudgment(camEAProduct);


                lock (EACornerProductLock_Cam8)
                {
                    EACornerProduct_CurrentCam.Add(camEAProduct);
                }

                //================================================================
                //【4】原图显示、保存
                bool result = camEAProduct.DiaphragmResult && camEAProduct.PolePieceResult;
                PrimitiveTransition(cameraEnum, camEAProduct.showHalconWindow, result, camEAProduct.showHalconWindow.m_nImageFrameID, camEAProduct.PackgeNum, camEAProduct.EAIndex, out Operation_ImageInfo operation_ImageInfo);

                //EA片处理完成
                camEAProduct.PrimitiveImgSaveRelativePath = operation_ImageInfo.str_ImageSaveRelativePath + operation_ImageInfo.str_ImageSaveName;
                camEAProduct.ScreenshotSaveRelativePath = operation_ImageInfo.str_ScreenshotSaveRelativePath + operation_ImageInfo.str_ScreenshotSaveName;
                camEAProduct.CamFinish = true;

                HOperatorSet.CountSeconds(out camEAProduct1.Image_DataProcessTime);
                HOperatorSet.CountSeconds(out camEAProduct1.Image_EndTime);
                //计算时间
                BIZ_General.CalculateProcessingTime(camEAProduct1);
                HWC8.operation_Image.imageInfosShow.Enqueue(operation_ImageInfo);

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }

        /// <summary>
        /// 获取隔膜点数量
        /// </summary>
        public List<LineInfo> GetCoorCoverPointsNum(CameraEnum cameraEnum)
        {
            if (overHangCraft == null) return null;
            if (overHangCraft.ListLines == null) return null;
            if (overHangCraft.ListLines.Count <= (int)cameraEnum) return null;
            return overHangCraft.ListLines[(int)cameraEnum].Where(p => p.IsStandLine).ToList();

        }

        public void GetMeanValue(List<CamEACornerProduct> EACornerProduct, int PackgeNum)
        {
            try
            {
                if (EACornerProduct == null) return;

                List<double> meanValues = new List<double>();
                for (int i = 0; i < EACornerProduct.Count; i++)
                {
                    if (EACornerProduct[i].PackgeNum == PackgeNum)
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }
        public void ShowMeanValue()
        {
            try
            {
                GetDataTitle(CameraEnum.Cam1, out List<string> show1, out List<string> save1);
                GetDataTitle(CameraEnum.Cam2, out List<string> show2, out List<string> save2);
                GetDataTitle(CameraEnum.Cam3, out List<string> show3, out List<string> save3);
                GetDataTitle(CameraEnum.Cam4, out List<string> show4, out List<string> save4);
                GetDataTitle(CameraEnum.Cam5, out List<string> show5, out List<string> save5);
                GetDataTitle(CameraEnum.Cam6, out List<string> show6, out List<string> save6);
                GetDataTitle(CameraEnum.Cam7, out List<string> show7, out List<string> save7);
                GetDataTitle(CameraEnum.Cam8, out List<string> show8, out List<string> save8);

                for (int i = 0; i < show1.Count; i++)
                {
                    BIZ_General.DictionaryKeySafety(show1[i], ref Project.Instance.cameraManager[CameraEnum.Cam1].m_CamDetectionVariable.sizeDetectionItemData);
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
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
        private static void PrimitiveTransition(CameraEnum cameraEnum, ShowHalconWindow showHalconWindow, bool result, long frame, int PackgeNum, int EAIndex, out Operation_ImageInfo operation_ImageInfo)
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
                string imageSaveName = frame.ToString().PadLeft(5, '0') + $"_{PackgeNum}_{EAIndex}";
                operation_ImageInfo = BIZ_General.ImageDataTypeTransition(showHalconWindow, Project.Instance.paramPathSet.ProductMojuanPath, imageSavePath, imageSaveName, saveImageFormat, imageSaveSelect, isShowImage,
                   HighDefinition_Screenshot, screenshotSaveSelect, Project.Instance.paramPathSet.ProductMojuanPath, screenshotSavePath, imageSaveName, screenshotSaveFormat, cameraEnum.ToString());
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
        private static void NGImageQueueTransition(CameraEnum cameraEnum, List<NgTypeInfo> ngTypeInfos, int frame, int PackgeNum, int EAIndex, ref QueuePackage<Operation_ImageInfo> out_operationImageInfos)
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
        /// 角点工艺判断
        /// </summary>
        /// <param name="cameraEnum"></param>
        /// <param name="pointEditsCraft"></param>
        /// <param name="currentCoorPoints"></param>
        /// <param name="camEACornerProducts"></param>
        /// <param name="pointEditsRowDatas"></param>
        public static void CoorPointCraftProcessing(CameraEnum cameraEnum, List<PointEdit> pointEditsCraft, CamEACornerProduct in_FirstEAProduct, CamEACornerProduct in_CurrentCamEAProduct, out List<RowData> pointEditsRowShowDatas, out List<RowData> pointEditsRowSaveDatas)
        {
            pointEditsRowShowDatas = new List<RowData>();
            pointEditsRowSaveDatas = new List<RowData>();
            //try
            //{
            //    bool DiaphragmResult = true;
            //    bool PolePieceResult = true;
            //    for (int i = 0; i < in_CurrentCamEAProduct.coorEaPoints.Count; i++)
            //    {
            //        //计算当前点的仿射坐标 
            //        double currentPointCamX = in_CurrentCamEAProduct.coorEaPoints[i].CPointX;
            //        double currentPointCamY = in_CurrentCamEAProduct.coorEaPoints[i].CPointY;
            //        bool isSuccess = ImageAlgorithm.GetAffinePoints2d(Project.Instance.cameraManager[cameraEnum].param_Checkerboard.HomMat2D, currentPointCamX, currentPointCamY, out HTuple out_currentX, out HTuple out_currentY);
            //        if (isSuccess)
            //        {
            //            in_CurrentCamEAProduct.coorEaPoints[i].AffinePointX = out_currentX.D;
            //            in_CurrentCamEAProduct.coorEaPoints[i].AffinePointY = out_currentY.D;

            //            //计算偏移值
            //            in_CurrentCamEAProduct.coorEaPoints[i].OffsetX = in_CurrentCamEAProduct.coorEaPoints[i].AffinePointX - in_FirstEAProduct.coorEaPoints[i].AffinePointX;
            //            in_CurrentCamEAProduct.coorEaPoints[i].OffsetY = in_CurrentCamEAProduct.coorEaPoints[i].AffinePointY - in_FirstEAProduct.coorEaPoints[i].AffinePointY;


            //            //================================================================
            //            //【2】工艺判断
            //            double centerX = in_FirstEAProduct.coorEaPoints[i].AffinePointX;
            //            double centerY = in_FirstEAProduct.coorEaPoints[i].AffinePointY;
            //            double radius = pointEditsCraft[i].Rbias;
            //            double pointX = in_CurrentCamEAProduct.coorEaPoints[i].AffinePointX;
            //            double pointY = in_CurrentCamEAProduct.coorEaPoints[i].AffinePointY;

            //            //判断是否在圆内
            //            if (OtherAlgorithm.IsPointOutsideCircle(centerX, centerY, radius, pointX, pointY, out double distance))
            //            {
            //                in_CurrentCamEAProduct.coorEaPoints[i].bl_PointResult = false;
            //            }
            //            else
            //            {
            //                in_CurrentCamEAProduct.coorEaPoints[i].bl_PointResult = true;
            //            }
            //            in_CurrentCamEAProduct.coorEaPoints[i].PointDist = distance;

            //            //判断X偏移是否在范围内
            //            if (Math.Abs(in_CurrentCamEAProduct.coorEaPoints[i].OffsetX) > pointEditsCraft[i].Xbias)
            //            {
            //                in_CurrentCamEAProduct.coorEaPoints[i].bl_OffsetX = false;
            //            }
            //            else
            //            {
            //                in_CurrentCamEAProduct.coorEaPoints[i].bl_OffsetX = true;
            //            }

            //            //判断Y偏移是否在范围内
            //            if (Math.Abs(in_CurrentCamEAProduct.coorEaPoints[i].OffsetY) > pointEditsCraft[i].Ybias)
            //            {
            //                in_CurrentCamEAProduct.coorEaPoints[i].bl_OffsetY = false;
            //            }
            //            else
            //            {
            //                in_CurrentCamEAProduct.coorEaPoints[i].bl_OffsetY = true;
            //            }

            //            //第一个为隔膜
            //            if (i == 0)
            //            {
            //                DiaphragmResult &= in_CurrentCamEAProduct.coorEaPoints[i].bl_OffsetY;
            //            }
            //            //其他为角点
            //            else
            //            {
            //                PolePieceResult &= in_CurrentCamEAProduct.coorEaPoints[i].bl_PointResult;
            //                PolePieceResult &= in_CurrentCamEAProduct.coorEaPoints[i].bl_OffsetX;
            //                PolePieceResult &= in_CurrentCamEAProduct.coorEaPoints[i].bl_OffsetY;
            //            }


            //            RowData PointDist = new RowData() { dataStr = in_CurrentCamEAProduct.coorEaPoints[i].PointDist.ToString("F3"), resultType = in_CurrentCamEAProduct.coorEaPoints[i].bl_PointResult ? ResultType.OK : ResultType.NG };
            //            RowData OffsetX = new RowData() { dataStr = in_CurrentCamEAProduct.coorEaPoints[i].OffsetX.ToString("F3"), resultType = in_CurrentCamEAProduct.coorEaPoints[i].bl_OffsetX ? ResultType.OK : ResultType.NG };
            //            RowData OffsetY = new RowData() { dataStr = in_CurrentCamEAProduct.coorEaPoints[i].OffsetY.ToString("F3"), resultType = in_CurrentCamEAProduct.coorEaPoints[i].bl_OffsetY ? ResultType.OK : ResultType.NG };

            //            pointEditsRowShowDatas.Add(PointDist);
            //            pointEditsRowShowDatas.Add(OffsetX);
            //            pointEditsRowShowDatas.Add(OffsetY);

            //            pointEditsRowSaveDatas.Add(PointDist);
            //            pointEditsRowSaveDatas.Add(new RowData() { dataStr = PointDist.resultType.ToString(), resultType = ResultType.OK });
            //            pointEditsRowSaveDatas.Add(OffsetX);
            //            pointEditsRowSaveDatas.Add(new RowData() { dataStr = OffsetX.resultType.ToString(), resultType = ResultType.OK });
            //            pointEditsRowSaveDatas.Add(OffsetY);
            //            pointEditsRowSaveDatas.Add(new RowData() { dataStr = OffsetY.resultType.ToString(), resultType = ResultType.OK });
            //        }

            //    }
            //    in_CurrentCamEAProduct.DiaphragmResult = DiaphragmResult;
            //    in_CurrentCamEAProduct.PolePieceResult = PolePieceResult;
            //}
            //catch (Exception ex)
            //{
            //    Log4netRecord.ErrorLog.Error(cameraEnum.ToString(), ex);
            //}
        }


        /// <summary>
        /// 计算尺寸数据（这一坨已经堆成屎了）
        /// </summary>
        /// <param name="cameraEnum"></param>
        /// <param name="electrodeEnum"></param>
        /// <param name="sizeItemInfo"></param>
        /// <param name="firstEAProduct">第一片EA</param>
        /// <param name="currentEAProduct">当前EA</param>
        /// <param name="secondToLastEAProduct">倒数第二片EA</param>
        /// <param name="lastEAProduct">上一片EA</param>
        /// <param name="in_LastCamEACornerProduct"></param>
        /// <param name="in_CurrentCamEACornerProduct"></param>
        public static void CalculateSizeData(CameraEnum cameraEnum, OverHangCraft craft, ElectrodeEnum electrodeEnum, List<SizeItemInfo> sizeItemInfo,
            CamEACornerProduct firstEAProduct, CamEACornerProduct currentEAProduct, CamEACornerProduct secondToLastEAProduct, CamEACornerProduct lastEAProduct)

        {
            //获取工艺
            List<SizeItemInfo> sizeCraft = sizeItemInfo;
            double CalibrationValue_X = Project.Instance.cameraManager[cameraEnum].m_CamParam.CalibrationValue_X;
            double CalibrationValue_Y = Project.Instance.cameraManager[cameraEnum].m_CamParam.CalibrationValue_Y;

            bool PolePieceResult = true;//极片尺寸结果
            bool DiaphragmResult = true;//隔膜尺寸结果
            //第一片默认OK，不检测数据
            if (currentEAProduct.EAIndex != 1)
            {
                for (int i = 0; i < sizeCraft.Count; i++)
                {
                    bool isDiaphragm = false;//是AS隔膜尺寸
                    string lineName1 = sizeCraft[i].Line1;//边1名称
                    string lineName2 = sizeCraft[i].Line2;//边2名称
                    string editorType = sizeCraft[i].EditorType;//编辑类型
                    string name = sizeCraft[i].Name;//尺寸名称
                    string sizeName = sizeCraft[i].Description;//尺寸名称
                    double realFixRito = sizeCraft[i].StD_RealFixRito;//校准系数
                    double RealFixOffset = sizeCraft[i].StD_RealFixOffset;//补偿值
                    double Offset1 = 0;//上下电芯单面片补偿值

                    //下电芯
                    if (currentEAProduct.PackgeNum == 1)
                    {

                    }
                    //上电芯
                    else if (currentEAProduct.PackgeNum == 2)
                    {

                    }


                    //当前尺寸数据
                    SizeInfo sizeInfo = new SizeInfo() { SizeName = sizeName, Name = name, resultType = ResultType.NG, DataResult = -999 };
                    sizeInfo.upperLimitValue = sizeCraft[i].StDValue + sizeCraft[i].StD_Up;
                    sizeInfo.lowerLimitValue = sizeCraft[i].StDValue + sizeCraft[i].StD_Down;
                    //=============================================
                    //【1.1】计算当前EA片的数据、隔膜边               
                    if (editorType == EditorTypeEnum.SAOH.ToString())
                    {
                        isDiaphragm = true;
                        if (currentEAProduct != null)
                        {
                            //下电芯
                            if (currentEAProduct.PackgeNum == 1)
                            {
                                //第一层
                                if (currentEAProduct.EAIndex == 1)
                                {
                                    Offset1 = sizeCraft[i].StD_DownCellDownOffsetGM;
                                }
                                //最后一层
                                else if (currentEAProduct.EAIndex == craft.StakeCathodeCount)
                                {
                                    Offset1 = sizeCraft[i].StD_DownCellUpOffsetGM;
                                }

                            }
                            //上电芯
                            else if (currentEAProduct.PackgeNum == 2)
                            {
                                //第一层
                                if (currentEAProduct.EAIndex == 1)
                                {
                                    Offset1 = sizeCraft[i].StD_UpCellDownOffsetGM;
                                }
                                //最后一层
                                else if (currentEAProduct.EAIndex == craft.StakeCathodeCount)
                                {
                                    Offset1 = sizeCraft[i].StD_UpCellUpOffsetGM;
                                }
                            }

                            //获取边1数据
                            LineInfo lineInfo1 = null;
                            LineInfo lineInfo2 = null;
                            if (currentEAProduct.m_lineShowInfos.Where(p => p.StrLineName == lineName1).Count() > 0)
                            {
                                lineInfo1 = currentEAProduct.m_lineShowInfos.Find(p => p.StrLineName == lineName1);
                            }
                            if (currentEAProduct.m_lineShowInfos.Where(p => p.StrLineName == lineName2).Count() > 0)
                            {
                                lineInfo2 = currentEAProduct.m_lineShowInfos.Find(p => p.StrLineName == lineName2);
                            }
                            double location1 = 0;
                            double location2 = 0;
                            if (lineInfo1 != null && lineInfo2 != null && lineInfo1.IsLineSuccessfull && lineInfo2.IsLineSuccessfull)
                            {
                                location1 = ((lineInfo1.RowBegin + lineInfo1.RowEnd) / 2);
                                location2 = ((lineInfo2.RowBegin + lineInfo2.RowEnd) / 2);

                                //计算Y距离
                                double dist = Math.Abs((location1) - (location2));
                                sizeInfo.DataResult = dist * CalibrationValue_Y * realFixRito + RealFixOffset + Offset1;
                                //和实测值不符
                                //BIZ_General.CalculateLines_DistanceSs(cameraEnum, lineInfo1, lineInfo2, sizeCraft[i].EditorType, realFixRito, out sizeInfo.DataResult);
                            }


                            //日志
                            string[] title = new string[]
                            {
                                        "帧号",
                                        "电芯编号",
                                        "极片编号",
                                        "当前相机",
                                        "检测名称",
                                        "行列位置1",
                                        "行列位置2",
                                        "标定值",
                                        "系数k",
                                        "补偿值b",
                                        "上下电芯单面片补偿值",
                                        "距离"

                            };

                            string[] data = new string[]
                                  {
                                         $"{currentEAProduct.CamFrame}",
                                         $"{currentEAProduct.PackgeNum}",
                                         $"{currentEAProduct.EAIndex}",
                                         $"{cameraEnum}",
                                         $"{sizeName}",
                                         $"{location1}",
                                         $"{location2}",
                                         $"{CalibrationValue_Y}",
                                         $"{realFixRito}",
                                         $"{RealFixOffset}",
                                         $"{Offset1}",
                                         $"{sizeInfo.DataResult}",
                                  };

                            FileOperationCSV.WriteDataLock_CSV(Project.Instance.paramPathSet.OVHProdDataSizeDiaphragmLogPath,
                                title, data, cameraEnum.ToString());

                        }
                    }

                    ////=============================================
                    ////【1.2】计算对齐度(暂时屏蔽)
                    //else if (false && editorType == EditorTypeEnum.基准偏移X.ToString() ||
                    //         editorType == EditorTypeEnum.基准偏移Y.ToString())
                    //{
                    //    if (firstEAProduct != null && currentEAProduct != null)
                    //    {
                    //        //获取边1数据
                    //        LineInfo lineInfo1 = null;
                    //        LineInfo lineInfo2 = null;
                    //        if (currentEAProduct.m_lineShowInfos.Where(p => p.StrLineName == lineName1).Count() > 0)
                    //        {
                    //            lineInfo1 = currentEAProduct.m_lineShowInfos.Find(p => p.StrLineName == lineName1);
                    //        }
                    //        if (firstEAProduct.m_lineShowInfos.Where(p => p.StrLineName == lineName2).Count() > 0)
                    //        {
                    //            lineInfo2 = firstEAProduct.m_lineShowInfos.Find(p => p.StrLineName == lineName2);
                    //        }
                    //        if (lineInfo1 != null && lineInfo2 != null && lineInfo1.IsLineSuccessfull && lineInfo2.IsLineSuccessfull)
                    //        {
                    //            BIZ_General.CalculateLines(cameraEnum, lineInfo1, lineInfo2, sizeCraft[i].EditorType, sizeCraft[i].StD_RealFixRito, out sizeInfo.DataResult);
                    //        }
                    //    }

                    //    //没有第一次EA片,结果为OK
                    //    else if (currentEAProduct != null && currentEAProduct.EAIndex == 1)
                    //    {
                    //        sizeInfo.resultType = ResultType.OK;
                    //        sizeInfo.DataResult = 0;
                    //    }

                    //}


                    //=============================================
                    //【1.3】计算上一片EA和当前EA片的间距
                    else if (editorType == EditorTypeEnum.ACOH_X.ToString() ||
                             editorType == EditorTypeEnum.ACOH_Y.ToString() ||
                             editorType == EditorTypeEnum.ACOH_Cor.ToString())
                    {
                        if (lastEAProduct != null && currentEAProduct != null)
                        {
                            currentEAProduct.LastCamFrame = lastEAProduct.CamFrame;
                            //基准点位置
                            double basePointRow = Project.Instance.cameraManager[cameraEnum].param_MarkSet.m_DetectionBaseCenterY;
                            double basePointCol = Project.Instance.cameraManager[cameraEnum].param_MarkSet.m_DetectionBaseCenterX;

                            //计算当前边到基准点的距离
                            LineInfo lineInfo1 = null;
                            LineInfo lineInfo2 = null;
                            if (currentEAProduct.m_lineShowInfos.Where(p => p.StrLineName == lineName1).Count() > 0)
                            {
                                lineInfo1 = currentEAProduct.m_lineShowInfos.Find(p => p.StrLineName == lineName1);
                            }
                            if (lastEAProduct.m_lineShowInfos.Where(p => p.StrLineName == lineName2).Count() > 0)
                            {
                                lineInfo2 = lastEAProduct.m_lineShowInfos.Find(p => p.StrLineName == lineName2);
                            }

                            if (lineInfo1 != null && lineInfo2 != null && lineInfo1.IsLineSuccessfull && lineInfo2.IsLineSuccessfull)
                            {
                                int b = 1;
                                OVHDetectionMethodeEnum oVHDetectionMethodeEnum = OVHDetectionMethodeEnum.AC;
                                if (electrodeEnum == ElectrodeEnum.阳极)
                                {
                                    oVHDetectionMethodeEnum = OVHDetectionMethodeEnum.CA;
                                    b = -1;
                                }

                                //下电芯
                                if (currentEAProduct.PackgeNum == 1)
                                {
                                    //第2层
                                    if (currentEAProduct.EAIndex == 2)
                                    {
                                        Offset1 = sizeCraft[i].StD_DownCellDownOffset;
                                    }
                                    //最后一层
                                    else if (currentEAProduct.EAIndex == craft.StakeCathodeCount)
                                    {
                                        Offset1 = sizeCraft[i].StD_DownCellUpOffset;
                                    }

                                }
                                //上电芯
                                else if (currentEAProduct.PackgeNum == 2)
                                {
                                    //第2层
                                    if (currentEAProduct.EAIndex == 2)
                                    {
                                        Offset1 = sizeCraft[i].StD_UpCellDownOffset;
                                    }
                                    //最后一层
                                    else if (currentEAProduct.EAIndex == craft.StakeCathodeCount)
                                    {
                                        Offset1 = sizeCraft[i].StD_UpCellUpOffset;
                                    }
                                }


                                //日志
                                string[] title = new string[]
                                {
                                        "检测类型",
                                        "帧号",
                                        "电芯编号",
                                        "极片编号",
                                        "当前相机",
                                        "计算偏移",
                                        "检测名称",
                                        "行列位置",
                                        "行列基准点",
                                        "标定值",
                                        "距离d1",

                                        "上一个相机",
                                        "行列位置",
                                        "行列基准点",
                                        "标定值",
                                        "距离d2",

                                        "系数k",
                                        "补偿值b",
                                        "上下电芯单面片补偿值",
                                        "上Tabneck值",
                                        "下Tabneck值",
                                        "4Y的Tabneck补偿值",
                                        "公式：（d1-d2)*k +b",
                                };


                                CameraEnum lastCameraEnum = (CameraEnum)lastEAProduct.intCamEnum;
                                double lastCameraCalibrationValue_X = Project.Instance.cameraManager[lastCameraEnum].m_CamParam.CalibrationValue_X;
                                double lastCameraCalibrationValue_Y = Project.Instance.cameraManager[lastCameraEnum].m_CamParam.CalibrationValue_Y;
                                double lastPointRow = Project.Instance.cameraManager[lastCameraEnum].param_MarkSet.m_DetectionBaseCenterY;
                                double lastPointCol = Project.Instance.cameraManager[lastCameraEnum].param_MarkSet.m_DetectionBaseCenterX;
                                //计算X距离
                                if (editorType == EditorTypeEnum.ACOH_X.ToString())
                                {

                                    double currentLacation = ((lineInfo1.ColBegin + lineInfo1.ColEnd) / 2);
                                    double dist1 = currentLacation - basePointCol;
                                    dist1 = dist1 * CalibrationValue_X;

                                    double lastLacation = ((lineInfo2.ColBegin + lineInfo2.ColEnd) / 2);
                                    double dist2 = lastLacation - lastPointCol;
                                    dist2 = dist2 * lastCameraCalibrationValue_X;
                                    double dist = (dist1 - dist2) * b * realFixRito + RealFixOffset + Offset1;
                                    sizeInfo.DataResult = dist;

                                    string[] data = new string[]
                                    {
                                         $"{oVHDetectionMethodeEnum}",
                                         $"{currentEAProduct.CamFrame}",
                                         $"{currentEAProduct.PackgeNum}",
                                         $"{currentEAProduct.EAIndex}",
                                         $"{cameraEnum}",
                                         "X偏移",
                                         $"{sizeName}",
                                         $"{currentLacation.ToString("F2")}",
                                         $"{basePointCol.ToString("F2")}",
                                         $"{CalibrationValue_X}",
                                         $"{dist1.ToString("F3")}",


                                         $"{lastCameraEnum}",
                                         $"{lastLacation.ToString("F2")}",
                                         $"{lastPointCol.ToString("F2")}",
                                         $"{lastCameraCalibrationValue_X}",
                                         $"{dist2.ToString("F3")}",

                                         $"{realFixRito}",
                                         $"{RealFixOffset}",
                                         $"{Offset1}",
                                         $"{currentEAProduct.UpTabneckValue}",
                                         $"{currentEAProduct.DownTabneckValue}",
                                         $"-",
                                         $"{dist}",

                                    };

                                    FileOperationCSV.WriteDataLock_CSV(Project.Instance.paramPathSet.OVHProdDataSizeLogPath,
                                        title, data, cameraEnum.ToString());

                                }
                                //计算Y距离
                                else if (editorType == EditorTypeEnum.ACOH_Y.ToString())
                                {
                                    if (sizeName == "4Y")
                                    {
                                        double TabneckVaule = 0;
                                        if (oVHDetectionMethodeEnum == OVHDetectionMethodeEnum.AC)
                                        {
                                            TabneckVaule = currentEAProduct.DownTabneckValue;
                                            //  TabneckVaule = currentEAProduct.UpTabneckValue;
                                        }
                                        else if (oVHDetectionMethodeEnum == OVHDetectionMethodeEnum.CA)
                                        {
                                            TabneckVaule = lastEAProduct.UpTabneckValue;
                                            //TabneckVaule = lastEAProduct.DownTabneckValue;
                                        }

                                        double currentLacation = ((lineInfo1.RowBegin + lineInfo1.RowEnd) / 2);
                                        double dist1 = currentLacation - basePointRow;
                                        dist1 = dist1 * CalibrationValue_Y;

                                        double lastLacation = ((lineInfo2.RowBegin + lineInfo2.RowEnd) / 2);
                                        double dist2 = lastLacation - lastPointRow;
                                        dist2 = dist2 * lastCameraCalibrationValue_Y;

                                        int b1 = realFixRito > 0 ? 1 : -1;

                                        double dist = (((dist1 - dist2) * b) - (TabneckVaule * b1)) * realFixRito + RealFixOffset + Offset1;
                                        sizeInfo.DataResult = dist;

                                        string[] data = new string[]
                                               {
                                                       $"{oVHDetectionMethodeEnum}",
                                                       $"{currentEAProduct.CamFrame}",
                                                       $"{currentEAProduct.PackgeNum}",
                                                       $"{currentEAProduct.EAIndex}",
                                                       $"{cameraEnum}",
                                                       "Y偏移",
                                                       $"{sizeName}",
                                                       $"{currentLacation.ToString("F2")}",
                                                       $"{basePointRow.ToString("F2")}",
                                                       $"{CalibrationValue_Y}",
                                                       $"{dist1.ToString("F3")}",


                                                       $"{lastCameraEnum}",
                                                       $"{lastLacation.ToString("F2")}",
                                                       $"{lastPointRow.ToString("F2")}",
                                                       $"{lastCameraCalibrationValue_Y}",
                                                       $"{dist2.ToString("F3")}",

                                                       $"{realFixRito}",
                                                       $"{RealFixOffset}",
                                                       $"{Offset1}",
                                                       $"{currentEAProduct.UpTabneckValue}",
                                                       $"{currentEAProduct.DownTabneckValue}",
                                                       $"{TabneckVaule}",
                                                       $"{dist}",

                                               };

                                        FileOperationCSV.WriteDataLock_CSV(Project.Instance.paramPathSet.OVHProdDataSizeLogPath,
                                            title, data, cameraEnum.ToString());
                                    }
                                    else
                                    {
                                        double currentLacation = ((lineInfo1.RowBegin + lineInfo1.RowEnd) / 2);
                                        double dist1 = currentLacation - basePointRow;
                                        dist1 = dist1 * CalibrationValue_Y;

                                        double lastLacation = ((lineInfo2.RowBegin + lineInfo2.RowEnd) / 2);
                                        double dist2 = lastLacation - lastPointRow;
                                        dist2 = dist2 * lastCameraCalibrationValue_Y;
                                        double dist = (dist1 - dist2) * b * realFixRito + RealFixOffset + Offset1;
                                        sizeInfo.DataResult = dist;

                                        string[] data = new string[]
                                               {
                                                       $"{oVHDetectionMethodeEnum}",
                                                       $"{currentEAProduct.CamFrame}",
                                                       $"{currentEAProduct.PackgeNum}",
                                                       $"{currentEAProduct.EAIndex}",
                                                       $"{cameraEnum}",
                                                       "Y偏移",
                                                       $"{sizeName}",
                                                       $"{currentLacation.ToString("F2")}",
                                                       $"{basePointRow.ToString("F2")}",
                                                       $"{CalibrationValue_Y}",
                                                       $"{dist1.ToString("F3")}",


                                                       $"{lastCameraEnum}",
                                                       $"{lastLacation.ToString("F2")}",
                                                       $"{lastPointRow.ToString("F2")}",
                                                       $"{lastCameraCalibrationValue_Y}",
                                                       $"{dist2.ToString("F3")}",

                                                       $"{realFixRito}",
                                                       $"{RealFixOffset}",
                                                       $"{Offset1}",
                                                       $"{currentEAProduct.UpTabneckValue}",
                                                       $"{currentEAProduct.DownTabneckValue}",
                                                       $"-",
                                                       $"{dist}",

                                               };

                                        FileOperationCSV.WriteDataLock_CSV(Project.Instance.paramPathSet.OVHProdDataSizeLogPath,
                                            title, data, cameraEnum.ToString());
                                    }







                                }
                                //计算Cor距离
                                else if (editorType == EditorTypeEnum.ACOH_Cor.ToString())
                                {
                                    //X距离
                                    double currentLacationX = ((lineInfo1.ColBegin + lineInfo1.ColEnd) / 2);
                                    double distX1 = currentLacationX - basePointCol;
                                    distX1 = distX1 * CalibrationValue_X;

                                    double lastLacationX = ((lineInfo2.ColBegin + lineInfo2.ColEnd) / 2);
                                    double distX2 = lastLacationX - lastPointCol;
                                    distX2 = distX2 * lastCameraCalibrationValue_X;
                                    double distX = (distX1 - distX2);

                                    //Y距离
                                    double currentLacationY = ((lineInfo1.RowBegin + lineInfo1.RowEnd) / 2);
                                    double distY1 = currentLacationY - basePointRow;
                                    distY1 = distY1 * CalibrationValue_Y;

                                    double lastLacationY = ((lineInfo2.RowBegin + lineInfo2.RowEnd) / 2);
                                    double distY2 = lastLacationY - lastPointRow;
                                    distY2 = distY2 * lastCameraCalibrationValue_Y;
                                    double distY = (distY1 - distY2);

                                    //
                                    double dist = Math.Sqrt((distX * distX) + (distY * distY));
                                    dist = dist * realFixRito + RealFixOffset + Offset1;
                                    sizeInfo.DataResult = dist;


                                    string[] data = new string[]
                                                                     {
                                         $"{oVHDetectionMethodeEnum}",
                                         $"{currentEAProduct.CamFrame}",
                                         $"{currentEAProduct.PackgeNum}",
                                         $"{currentEAProduct.EAIndex}",
                                         $"{cameraEnum}",
                                         "CorX偏移",
                                         $"{sizeName}",
                                         $"{currentLacationX.ToString("F2")}",
                                         $"{basePointCol.ToString("F2")}",
                                         $"{CalibrationValue_X}",
                                         $"{distX1.ToString("F3")}",


                                         $"{lastCameraEnum}",
                                         $"{lastLacationX.ToString("F2")}",
                                         $"{lastPointCol.ToString("F2")}",
                                         $"{lastCameraCalibrationValue_X}",
                                         $"{distX2.ToString("F3")}",

                                         $"{realFixRito}",
                                         $"{RealFixOffset}",
                                         $"{Offset1}",
                                         $"{currentEAProduct.UpTabneckValue}",
                                         $"{currentEAProduct.DownTabneckValue}",
                                         $"-",
                                         $"{dist}",

                                                                     };

                                    FileOperationCSV.WriteDataLock_CSV(Project.Instance.paramPathSet.OVHProdDataSizeLogPath,
                                        title, data, cameraEnum.ToString());


                                    data = new string[]
                                    {
                                         $"{oVHDetectionMethodeEnum}",
                                         $"{currentEAProduct.CamFrame}",
                                         $"{currentEAProduct.PackgeNum}",
                                         $"{currentEAProduct.EAIndex}",
                                         $"{cameraEnum}",
                                         "CorY偏移",
                                         $"{sizeName}",
                                         $"{currentLacationY.ToString("F2")}",
                                         $"{basePointRow.ToString("F2")}",
                                         $"{CalibrationValue_Y}",
                                         $"{distY1.ToString("F3")}",


                                         $"{lastCameraEnum}",
                                         $"{lastLacationY.ToString("F2")}",
                                         $"{lastPointRow.ToString("F2")}",
                                         $"{lastCameraCalibrationValue_Y}",
                                         $"{distY2.ToString("F3")}",

                                         $"{realFixRito}",
                                         $"{RealFixOffset}",
                                         $"{Offset1}",
                                         $"{currentEAProduct.UpTabneckValue}",
                                         $"{currentEAProduct.DownTabneckValue}",
                                         $"-",
                                         $"{dist}",

                                    };

                                    FileOperationCSV.WriteDataLock_CSV(Project.Instance.paramPathSet.OVHProdDataSizeLogPath,
                                        title, data, cameraEnum.ToString());

                                }

                            }
                        }
                        //没有上一次EA片,结果为OK
                        else if (currentEAProduct != null && currentEAProduct.EAIndex == 1)
                        {
                            sizeInfo.resultType = ResultType.OK;
                            sizeInfo.DataResult = 0;

                        }

                    }


                    //=============================================
                    //【1.4】工艺判断
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

                    currentEAProduct.sizeInfos.Add(sizeInfo);

                    if (sizeInfo.resultType == ResultType.NG)
                    {
                        if (isDiaphragm)
                        {
                            DiaphragmResult = false;
                        }
                        else
                        {
                            PolePieceResult = false;
                        }
                    }
                    //=============================================
                    //【1.5】AA、CC尺寸信息
                    OVHDetectionMethodeEnum oVHDetectionMethodeEnumAACC = OVHDetectionMethodeEnum.CC;
                    if (electrodeEnum == ElectrodeEnum.阳极)
                    {
                        oVHDetectionMethodeEnumAACC = OVHDetectionMethodeEnum.AA;
                    }

                    SizeInfo sizeMonitorInfo = new SizeInfo() { SizeName = sizeName, Name = name, resultType = ResultType.NG, DataResult = -999 };
                    if (currentEAProduct != null && secondToLastEAProduct != null)
                    {

                        //下电芯
                        if (currentEAProduct.PackgeNum == 1)
                        {
                            //第3层
                            if (currentEAProduct.EAIndex == 3)
                            {

                                Offset1 = sizeCraft[i].StD_DownCellDownOffsetAA;
                            }
                            //最后一层
                            else if (currentEAProduct.EAIndex == craft.StakeCathodeCount)
                            {
                                Offset1 = sizeCraft[i].StD_DownCellUpOffsetAA;
                            }

                        }
                        //上电芯
                        else if (currentEAProduct.PackgeNum == 2)
                        {
                            //第3层
                            if (currentEAProduct.EAIndex == 3)
                            {
                                Offset1 = sizeCraft[i].StD_UpCellDownOffsetAA;
                            }
                            //最后一层
                            else if (currentEAProduct.EAIndex == craft.StakeCathodeCount)
                            {
                                Offset1 = sizeCraft[i].StD_UpCellUpOffsetAA;
                            }
                        }

                        //计算当前边到基准点的距离
                        LineInfo lineInfo1 = null;
                        LineInfo lineInfo2 = null;
                        if (currentEAProduct.m_lineShowInfos.Where(p => p.StrLineName == lineName1).Count() > 0)
                        {
                            lineInfo1 = currentEAProduct.m_lineShowInfos.Find(p => p.StrLineName == lineName1);
                        }
                        if (secondToLastEAProduct.m_lineShowInfos.Where(p => p.StrLineName == lineName1).Count() > 0)
                        {
                            lineInfo2 = secondToLastEAProduct.m_lineShowInfos.Find(p => p.StrLineName == lineName1);
                        }

                        if (lineInfo1 != null && lineInfo2 != null && lineInfo1.IsLineSuccessfull && lineInfo2.IsLineSuccessfull)
                        {
                            double point1 = 0;
                            double point2 = 0;
                            double dist1 = 0;
                            double CalibrationValue = 0;
                            //计算X距离
                            if (editorType == EditorTypeEnum.ACOH_X.ToString())
                            {
                                CalibrationValue = CalibrationValue_X;
                                point1 = ((lineInfo1.ColBegin + lineInfo1.ColEnd) / 2);
                                point2 = ((lineInfo2.ColBegin + lineInfo2.ColEnd) / 2);
                                dist1 = (point1 - point2) * CalibrationValue + Offset1;
                                sizeMonitorInfo.DataResult = dist1;
                            }
                            //计算Y距离
                            else if (editorType == EditorTypeEnum.ACOH_Y.ToString() ||
                                editorType == EditorTypeEnum.SAOH.ToString())
                            {
                                CalibrationValue = CalibrationValue_Y;
                                point1 = ((lineInfo1.RowBegin + lineInfo1.RowEnd) / 2);
                                point2 = ((lineInfo2.RowBegin + lineInfo2.RowEnd) / 2);
                                dist1 = (point1 - point2) * CalibrationValue + Offset1;
                                sizeMonitorInfo.DataResult = dist1;
                            }

                            //日志
                            string[] title = new string[]
                            {
                                        "检测类型",
                                        "帧号",
                                        "电芯编号",
                                        "极片编号",
                                         "当前相机",
                                        "检测名称",
                                        "行列位置1",
                                        "行列位置2",
                                        "标定值",
                                        "上下电芯单面片补偿值",
                                        "距离",
                            };

                            string[] data = new string[]
                                 {
                                         $"{oVHDetectionMethodeEnumAACC}",
                                         $"{currentEAProduct.CamFrame}",
                                         $"{currentEAProduct.PackgeNum}",
                                         $"{currentEAProduct.EAIndex}",
                                          $"{cameraEnum}",
                                         $"{sizeName}",
                                         $"{point1}",
                                         $"{point2}",
                                         $"{CalibrationValue}",
                                         $"{Offset1}",
                                         $"{dist1}",
                                 };

                            FileOperationCSV.WriteDataLock1_CSV(Project.Instance.paramPathSet.OVHProdDataSizeMonitorLogPath,
                                title, data, cameraEnum.ToString());

                        }
                    }

                    //=============================================
                    //【1.6】工艺判断                 
                    if (oVHDetectionMethodeEnumAACC == OVHDetectionMethodeEnum.CC && currentEAProduct.EAIndex <= 3)
                    {
                        continue;
                    }
                    if (sizeCraft[i].Name == "4Y" ||
                        editorType == EditorTypeEnum.ACOH_Cor.ToString())
                    {
                        //这些尺寸不参与逻辑判断
                        sizeMonitorInfo.DataResult = Double.NaN;
                        sizeMonitorInfo.resultType = ResultType.OK;
                    }
                    else
                    {
                        if (sizeMonitorInfo.DataResult != -999)
                        {
                            if (sizeMonitorInfo.DataResult >= craft.AAStandardDown &&
                                 sizeMonitorInfo.DataResult <= craft.AAStandardUp)
                            {
                                sizeMonitorInfo.resultType = ResultType.OK;
                            }
                            else
                            {
                                if (sizeCraft[i].DB && craft.EnAACCDB)
                                {
                                    sizeMonitorInfo.resultType = ResultType.NG;
                                }
                                else
                                {
                                    sizeMonitorInfo.resultType = ResultType.Warn;
                                }
                            }
                        }
                    }

                    currentEAProduct.sizeMonitorInfos.Add(sizeMonitorInfo);

                    if (sizeMonitorInfo.resultType == ResultType.NG)
                    {
                        if (isDiaphragm)
                        {
                            DiaphragmResult = false;
                        }
                        else
                        {
                            PolePieceResult = false;
                        }
                    }
                }
            }


            //计算相机尺寸结果
            currentEAProduct.PolePieceResult = PolePieceResult;
            currentEAProduct.DiaphragmResult = DiaphragmResult;
        }

        public static void ProcessingData_Cam(CameraEnum cameraEnum, OverHangCraft craft, ElectrodeEnum electrodeEnum, List<SizeItemInfo> sizeItemInfo,
             CamEACornerProduct firstEAProduct, CamEACornerProduct currentEAProduct, CamEACornerProduct secondToLastEAProduct, CamEACornerProduct lastEAProduct,
            ListView listView, string fileSavePath, DisplayWindowClass displayWindowClass, ref Dictionary<string, SizeDetectionItemData> dicSizeDetectionItemData,
            ref Dictionary<string, SizeDetectionItemData> dicSizeDetectionItemDataAA, ref Dictionary<string, SizeDetectionItemData> dicSizeDetectionItemDataCC)
        {
            //================================================================
            //【1】计算数据

            //================================================================
            //【2】工艺判断         
            if (currentEAProduct != null)
            {
                CalculateSizeData(cameraEnum, craft, electrodeEnum, sizeItemInfo, firstEAProduct, currentEAProduct, secondToLastEAProduct, lastEAProduct);
            }

            //================================================================
            //【3】数据显示、保存        
            List<RowData> showRowDataList = new List<RowData>();
            List<RowData> saveRowDataList = new List<RowData>();
            RowData timeRowData = new RowData() { dataStr = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff"), resultType = ResultType.OK };
            RowData frameRowData = new RowData() { dataStr = currentEAProduct.showHalconWindow.m_nImageFrameID.ToString(), resultType = ResultType.OK };
            RowData PackgeNumData = new RowData() { dataStr = currentEAProduct.PackgeNum.ToString(), resultType = ResultType.OK };
            RowData EAIndexNumData = new RowData() { dataStr = currentEAProduct.EAIndex.ToString(), resultType = ResultType.OK };
            showRowDataList.Add(timeRowData);
            showRowDataList.Add(frameRowData);
            showRowDataList.Add(PackgeNumData);
            showRowDataList.Add(EAIndexNumData);

            saveRowDataList.Add(timeRowData);
            saveRowDataList.Add(frameRowData);
            saveRowDataList.Add(PackgeNumData);
            saveRowDataList.Add(EAIndexNumData);

            if (currentEAProduct != null && currentEAProduct.sizeInfos.Count > 0)
            {
                for (int i = 0; i < currentEAProduct.sizeInfos.Count; i++)
                {
                    RowData rowData = new RowData() { };
                    rowData.dataStr = currentEAProduct.sizeInfos[i].DataResult.ToString("F3");
                    rowData.resultType = currentEAProduct.sizeInfos[i].resultType;

                    RowData resultRowData = new RowData() { };
                    resultRowData.dataStr = currentEAProduct.sizeInfos[i].resultType.ToString();
                    resultRowData.resultType = currentEAProduct.sizeInfos[i].resultType;
                    showRowDataList.Add(rowData);


                    saveRowDataList.Add(rowData);
                    saveRowDataList.Add(resultRowData);

                    try
                    {
                        //添加曲线图数据
                        lock (ChartSize_Lock)
                        {
                            BIZ_General.DictionaryKeySafety(currentEAProduct.sizeInfos[i].Name, ref dicSizeDetectionItemData);
                            dicSizeDetectionItemData[currentEAProduct.sizeInfos[i].Name].lChartAllData.Add(new ChartPoint() { ChartDetectionData = currentEAProduct.sizeInfos[i].DataResult }, 100);
                            double meanValue = dicSizeDetectionItemData[currentEAProduct.sizeInfos[i].Name].lChartAllData.ToArray().Average(p => p.ChartDetectionData);

                            ChartPoint chartTempPoint = new ChartPoint() { ChartDetectionData = currentEAProduct.sizeInfos[i].DataResult, ChartMeanData = meanValue };
                            dicSizeDetectionItemData[currentEAProduct.sizeInfos[i].Name].lChartAllTempData.Add(chartTempPoint, 100);

                        }
                    }
                    catch (Exception ex)
                    {
                        Log4netRecord.ErrorLog.Error(cameraEnum.ToString(), ex);
                    }
                }
            }

            //添加AA和CC曲线图数据
            if (currentEAProduct != null && currentEAProduct.sizeMonitorInfos.Count > 0)
            {
                for (int i = 0; i < currentEAProduct.sizeMonitorInfos.Count; i++)
                {
                    try
                    {
                        lock (ChartSizeAACC_Lock)
                        {
                            if (currentEAProduct.EAIndex % 2 == 0)
                            {
                                BIZ_General.DictionaryKeySafety(currentEAProduct.sizeMonitorInfos[i].Name, ref dicSizeDetectionItemDataCC);
                                dicSizeDetectionItemDataCC[currentEAProduct.sizeMonitorInfos[i].Name].lChartAllData.Add(new ChartPoint() { ChartDetectionData = currentEAProduct.sizeMonitorInfos[i].DataResult }, 100);
                                double meanValue = dicSizeDetectionItemDataCC[currentEAProduct.sizeMonitorInfos[i].Name].lChartAllData.ToArray().Average(p => p.ChartDetectionData);

                                ChartPoint chartTempPoint = new ChartPoint() { ChartDetectionData = currentEAProduct.sizeMonitorInfos[i].DataResult, ChartMeanData = meanValue };
                                dicSizeDetectionItemDataCC[currentEAProduct.sizeMonitorInfos[i].Name].lChartAllTempData.Add(chartTempPoint, 100);
                            }
                            else if (currentEAProduct.EAIndex % 2 == 1)
                            {
                                BIZ_General.DictionaryKeySafety(currentEAProduct.sizeMonitorInfos[i].Name, ref dicSizeDetectionItemDataAA);
                                dicSizeDetectionItemDataAA[currentEAProduct.sizeMonitorInfos[i].Name].lChartAllData.Add(new ChartPoint() { ChartDetectionData = currentEAProduct.sizeMonitorInfos[i].DataResult }, 100);
                                double meanValue = dicSizeDetectionItemDataAA[currentEAProduct.sizeMonitorInfos[i].Name].lChartAllData.ToArray().Average(p => p.ChartDetectionData);

                                ChartPoint chartTempPoint = new ChartPoint() { ChartDetectionData = currentEAProduct.sizeMonitorInfos[i].DataResult, ChartMeanData = meanValue };
                                dicSizeDetectionItemDataAA[currentEAProduct.sizeMonitorInfos[i].Name].lChartAllTempData.Add(chartTempPoint, 100);
                            }


                        }
                    }
                    catch (Exception ex)
                    {
                        Log4netRecord.ErrorLog.Error(cameraEnum.ToString(), ex);
                    }
                }
            }

            NListView.Add(listView, showRowDataList.ToArray(), true);

            FileOperationCSV.WriteData_CSV(fileSavePath, Project.Instance.cameraManager[cameraEnum].m_CamDetectionVariable.DataTitlesSave, saveRowDataList.ToArray(), cameraEnum.ToString());

            ////================================================================
            ////【4】原图显示、保存
            //bool result = currentCamEAProduct.DiaphragmResult && currentCamEAProduct.PolePieceResult;
            //PrimitiveTransition(cameraEnum, currentCamEAProduct.showHalconWindow, result, currentCamEAProduct.CamFrame, currentCamEAProduct.PackgeNum, currentCamEAProduct.EAIndex, out Operation_ImageInfo operation_ImageInfo);
            //displayWindowClass.operation_Image.imageInfosShow.Enqueue(operation_ImageInfo);
        }


        public void ProcessingData_AllACamData(string fileSavePath, OVHDetectionMethodeEnum oVHDetectionMethodeEnum, int UpOrDownCell, int CurrentLayer, string qrCode)
        {
            lock (prod_Lock)
            {
                CamEACornerProduct currentCamEAProduct1 = null;
                CamEACornerProduct currentCamEAProduct2 = null;
                CamEACornerProduct currentCamEAProduct3 = null;
                CamEACornerProduct currentCamEAProduct4 = null;
                if (oVHDetectionMethodeEnum == OVHDetectionMethodeEnum.AC)
                {
                    currentCamEAProduct1 = GetCamLastOneEAData(CameraEnum.Cam5);
                    currentCamEAProduct2 = GetCamLastOneEAData(CameraEnum.Cam6);
                    currentCamEAProduct3 = GetCamLastOneEAData(CameraEnum.Cam7);
                    currentCamEAProduct4 = GetCamLastOneEAData(CameraEnum.Cam8);
                }
                else
                {
                    currentCamEAProduct1 = GetCamLastOneEAData(CameraEnum.Cam1);
                    currentCamEAProduct2 = GetCamLastOneEAData(CameraEnum.Cam2);
                    currentCamEAProduct3 = GetCamLastOneEAData(CameraEnum.Cam3);
                    currentCamEAProduct4 = GetCamLastOneEAData(CameraEnum.Cam4);
                }

                List<RowData> showRowDataList = new List<RowData>();
                List<RowData> saveRowDataList = new List<RowData>();
                RowData timeRowData = new RowData() { dataStr = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff"), resultType = ResultType.OK };
                RowData frameRowData = new RowData() { dataStr = currentCamEAProduct1.CamFrame.ToString(), resultType = ResultType.OK };
                RowData lastFrameRowData = new RowData() { dataStr = currentCamEAProduct1.LastCamFrame.ToString(), resultType = ResultType.OK };
                RowData PackgeNumData = new RowData() { dataStr = currentCamEAProduct1.PackgeNum.ToString(), resultType = ResultType.OK };
                RowData EAIndexNumData = new RowData() { dataStr = currentCamEAProduct1.EAIndex.ToString(), resultType = ResultType.OK };

                saveRowDataList.Add(timeRowData);
                saveRowDataList.Add(frameRowData);
                saveRowDataList.Add(lastFrameRowData);
                saveRowDataList.Add(PackgeNumData);
                saveRowDataList.Add(EAIndexNumData);

                //当前这一层总结果
                RowData sumResult = new RowData() { resultType = ResultType.OK };

                if (currentCamEAProduct1 != null && currentCamEAProduct1.sizeInfos?.Count > 0)
                {
                    for (int i = 0; i < currentCamEAProduct1.sizeInfos.Count; i++)
                    {
                        RowData rowData = new RowData() { };
                        rowData.dataStr = currentCamEAProduct1.sizeInfos[i].DataResult.ToString("F3");
                        rowData.resultType = currentCamEAProduct1.sizeInfos[i].resultType;

                        RowData resultRowData = new RowData() { };
                        resultRowData.dataStr = currentCamEAProduct1.sizeInfos[i].resultType.ToString();
                        resultRowData.resultType = currentCamEAProduct1.sizeInfos[i].resultType;


                        saveRowDataList.Add(rowData);
                        saveRowDataList.Add(resultRowData);

                        if (currentCamEAProduct1.sizeInfos[i].resultType == ResultType.NG)
                        {
                            sumResult.resultType = ResultType.NG;
                            sumResult.dataStr += $"{currentCamEAProduct1.sizeInfos[i].SizeName}/";
                        }
                    }
                }

                if (currentCamEAProduct2 != null && currentCamEAProduct2.sizeInfos?.Count > 0)
                {
                    for (int i = 0; i < currentCamEAProduct2.sizeInfos.Count; i++)
                    {
                        RowData rowData = new RowData() { };
                        rowData.dataStr = currentCamEAProduct2.sizeInfos[i].DataResult.ToString("F3");
                        rowData.resultType = currentCamEAProduct2.sizeInfos[i].resultType;

                        RowData resultRowData = new RowData() { };
                        resultRowData.dataStr = currentCamEAProduct2.sizeInfos[i].resultType.ToString();
                        resultRowData.resultType = currentCamEAProduct2.sizeInfos[i].resultType;


                        saveRowDataList.Add(rowData);
                        saveRowDataList.Add(resultRowData);

                        if (currentCamEAProduct2.sizeInfos[i].resultType == ResultType.NG)
                        {
                            sumResult.resultType = ResultType.NG;
                            sumResult.dataStr += $"{currentCamEAProduct2.sizeInfos[i].SizeName}/";
                        }
                    }
                }

                if (currentCamEAProduct3 != null && currentCamEAProduct3.sizeInfos?.Count > 0)
                {
                    for (int i = 0; i < currentCamEAProduct3.sizeInfos.Count; i++)
                    {
                        RowData rowData = new RowData() { };
                        rowData.dataStr = currentCamEAProduct3.sizeInfos[i].DataResult.ToString("F3");
                        rowData.resultType = currentCamEAProduct3.sizeInfos[i].resultType;

                        RowData resultRowData = new RowData() { };
                        resultRowData.dataStr = currentCamEAProduct3.sizeInfos[i].resultType.ToString();
                        resultRowData.resultType = currentCamEAProduct3.sizeInfos[i].resultType;


                        saveRowDataList.Add(rowData);
                        saveRowDataList.Add(resultRowData);

                        if (currentCamEAProduct3.sizeInfos[i].resultType == ResultType.NG)
                        {
                            sumResult.resultType = ResultType.NG;
                            sumResult.dataStr += $"{currentCamEAProduct3.sizeInfos[i].SizeName}/";
                        }
                    }
                }

                if (currentCamEAProduct4 != null && currentCamEAProduct4.sizeInfos?.Count > 0)
                {
                    for (int i = 0; i < currentCamEAProduct4.sizeInfos.Count; i++)
                    {
                        RowData rowData = new RowData() { };
                        rowData.dataStr = currentCamEAProduct4.sizeInfos[i].DataResult.ToString("F3");
                        rowData.resultType = currentCamEAProduct4.sizeInfos[i].resultType;

                        RowData resultRowData = new RowData() { };
                        resultRowData.dataStr = currentCamEAProduct4.sizeInfos[i].resultType.ToString();
                        resultRowData.resultType = currentCamEAProduct4.sizeInfos[i].resultType;


                        saveRowDataList.Add(rowData);
                        saveRowDataList.Add(resultRowData);

                        if (currentCamEAProduct4.sizeInfos[i].resultType == ResultType.NG)
                        {
                            sumResult.resultType = ResultType.NG;
                            sumResult.dataStr += $"{currentCamEAProduct4.sizeInfos[i].SizeName}/";
                        }
                    }
                }

                if (sumResult.resultType == ResultType.NG)
                {
                    sumResult.dataStr += "NG";
                }
                else
                {
                    sumResult.dataStr += "OK";
                }
                saveRowDataList.Add(sumResult);


                //下电芯最后一层
                if (UpOrDownCell == 1 && CurrentLayer == overHangCraft.StakeCathodeCount)
                {
                    GetDownCellResult(out short result, out string resultStr);
                    saveRowDataList.Add(new RowData() { dataStr = resultStr });
                }
                //上电芯最后一层
                if (UpOrDownCell == 2 && CurrentLayer == overHangCraft.StakeCathodeCount)
                {
                    GetUpCellResult(out short result, out string resultStr);
                    saveRowDataList.Add(new RowData() { dataStr = resultStr });
                }
                saveRowDataList.Add(new RowData() { dataStr = qrCode });

                if (oVHDetectionMethodeEnum == OVHDetectionMethodeEnum.AC)
                {
                    FileOperationCSV.WriteData_CSV(fileSavePath, BIZ_Variable_OverHang.Title_AC.ToArray(), saveRowDataList.ToArray(), "全相机AC数据");

                }
                else
                {
                    FileOperationCSV.WriteData_CSV(fileSavePath, BIZ_Variable_OverHang.Title_CA.ToArray(), saveRowDataList.ToArray(), "全相机CA数据");

                }

            }

        }

        public void ProcessingData_AllCamData_AAOrCC(string fileSavePath, OVHDetectionMethodeEnum oVHDetectionMethodeEnum, string qrCode)
        {
            lock (prodAAOrCC_Lock)
            {
                CamEACornerProduct currentCamEAProduct1 = null;
                CamEACornerProduct currentCamEAProduct2 = null;
                CamEACornerProduct currentCamEAProduct3 = null;
                CamEACornerProduct currentCamEAProduct4 = null;
                if (oVHDetectionMethodeEnum == OVHDetectionMethodeEnum.CC)
                {
                    currentCamEAProduct1 = GetCamLastOneEAData(CameraEnum.Cam5);
                    currentCamEAProduct2 = GetCamLastOneEAData(CameraEnum.Cam6);
                    currentCamEAProduct3 = GetCamLastOneEAData(CameraEnum.Cam7);
                    currentCamEAProduct4 = GetCamLastOneEAData(CameraEnum.Cam8);
                }
                else
                {
                    currentCamEAProduct1 = GetCamLastOneEAData(CameraEnum.Cam1);
                    currentCamEAProduct2 = GetCamLastOneEAData(CameraEnum.Cam2);
                    currentCamEAProduct3 = GetCamLastOneEAData(CameraEnum.Cam3);
                    currentCamEAProduct4 = GetCamLastOneEAData(CameraEnum.Cam4);
                }

                List<RowData> showRowDataList = new List<RowData>();
                List<RowData> saveRowDataList = new List<RowData>();
                RowData timeRowData = new RowData() { dataStr = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff"), resultType = ResultType.OK };
                RowData frameRowData = new RowData() { dataStr = currentCamEAProduct1.CamFrame.ToString(), resultType = ResultType.OK };
                RowData PackgeNumData = new RowData() { dataStr = currentCamEAProduct1.PackgeNum.ToString(), resultType = ResultType.OK };
                RowData EAIndexNumData = new RowData() { dataStr = currentCamEAProduct1.EAIndex.ToString(), resultType = ResultType.OK };

                saveRowDataList.Add(timeRowData);
                saveRowDataList.Add(frameRowData);
                saveRowDataList.Add(frameRowData);
                saveRowDataList.Add(PackgeNumData);
                saveRowDataList.Add(EAIndexNumData);


                //当前这一层总结果
                RowData sumResult = new RowData() { resultType = ResultType.OK };

                if (currentCamEAProduct1 != null && currentCamEAProduct1.sizeMonitorInfos?.Count > 0)
                {
                    for (int i = 0; i < currentCamEAProduct1.sizeMonitorInfos.Count; i++)
                    {
                        RowData rowData = new RowData() { };
                        rowData.dataStr = currentCamEAProduct1.sizeMonitorInfos[i].DataResult.ToString("F3");
                        rowData.resultType = currentCamEAProduct1.sizeMonitorInfos[i].resultType;

                        RowData resultRowData = new RowData() { };
                        resultRowData.dataStr = currentCamEAProduct1.sizeMonitorInfos[i].resultType.ToString();
                        resultRowData.resultType = currentCamEAProduct1.sizeMonitorInfos[i].resultType;


                        saveRowDataList.Add(rowData);
                        saveRowDataList.Add(resultRowData);

                        if (currentCamEAProduct1.sizeMonitorInfos[i].resultType == ResultType.NG)
                        {
                            sumResult.resultType = ResultType.NG;
                            sumResult.dataStr += $"{currentCamEAProduct1.sizeMonitorInfos[i].SizeName}/";
                        }
                    }
                }

                if (currentCamEAProduct2 != null && currentCamEAProduct2.sizeMonitorInfos?.Count > 0)
                {
                    for (int i = 0; i < currentCamEAProduct2.sizeMonitorInfos.Count; i++)
                    {
                        RowData rowData = new RowData() { };
                        rowData.dataStr = currentCamEAProduct2.sizeMonitorInfos[i].DataResult.ToString("F3");
                        rowData.resultType = currentCamEAProduct2.sizeMonitorInfos[i].resultType;

                        RowData resultRowData = new RowData() { };
                        resultRowData.dataStr = currentCamEAProduct2.sizeMonitorInfos[i].resultType.ToString();
                        resultRowData.resultType = currentCamEAProduct2.sizeMonitorInfos[i].resultType;


                        saveRowDataList.Add(rowData);
                        saveRowDataList.Add(resultRowData);

                        if (currentCamEAProduct2.sizeMonitorInfos[i].resultType == ResultType.NG)
                        {
                            sumResult.resultType = ResultType.NG;
                            sumResult.dataStr += $"{currentCamEAProduct2.sizeMonitorInfos[i].SizeName}/";
                        }
                    }
                }

                if (currentCamEAProduct3 != null && currentCamEAProduct3.sizeMonitorInfos?.Count > 0)
                {
                    for (int i = 0; i < currentCamEAProduct3.sizeMonitorInfos.Count; i++)
                    {
                        RowData rowData = new RowData() { };
                        rowData.dataStr = currentCamEAProduct3.sizeMonitorInfos[i].DataResult.ToString("F3");
                        rowData.resultType = currentCamEAProduct3.sizeMonitorInfos[i].resultType;

                        RowData resultRowData = new RowData() { };
                        resultRowData.dataStr = currentCamEAProduct3.sizeMonitorInfos[i].resultType.ToString();
                        resultRowData.resultType = currentCamEAProduct3.sizeMonitorInfos[i].resultType;


                        saveRowDataList.Add(rowData);
                        saveRowDataList.Add(resultRowData);

                        if (currentCamEAProduct3.sizeMonitorInfos[i].resultType == ResultType.NG)
                        {
                            sumResult.resultType = ResultType.NG;
                            sumResult.dataStr += $"{currentCamEAProduct3.sizeMonitorInfos[i].SizeName}/";
                        }
                    }
                }

                if (currentCamEAProduct4 != null && currentCamEAProduct4.sizeMonitorInfos?.Count > 0)
                {
                    for (int i = 0; i < currentCamEAProduct4.sizeMonitorInfos.Count; i++)
                    {
                        RowData rowData = new RowData() { };
                        rowData.dataStr = currentCamEAProduct4.sizeMonitorInfos[i].DataResult.ToString("F3");
                        rowData.resultType = currentCamEAProduct4.sizeMonitorInfos[i].resultType;

                        RowData resultRowData = new RowData() { };
                        resultRowData.dataStr = currentCamEAProduct4.sizeMonitorInfos[i].resultType.ToString();
                        resultRowData.resultType = currentCamEAProduct4.sizeMonitorInfos[i].resultType;


                        saveRowDataList.Add(rowData);
                        saveRowDataList.Add(resultRowData);

                        if (currentCamEAProduct4.sizeMonitorInfos[i].resultType == ResultType.NG)
                        {
                            sumResult.resultType = ResultType.NG;
                            sumResult.dataStr += $"{currentCamEAProduct4.sizeMonitorInfos[i].SizeName}/";
                        }
                    }
                }

                if (sumResult.resultType == ResultType.NG)
                {
                    sumResult.dataStr += "NG";
                }
                else
                {
                    sumResult.dataStr += "OK";
                }
                saveRowDataList.Add(sumResult);
                saveRowDataList.Add(new RowData() { dataStr = "" });
                saveRowDataList.Add(new RowData() { dataStr = qrCode });

                if (oVHDetectionMethodeEnum == OVHDetectionMethodeEnum.AA)
                {
                    FileOperationCSV.WriteData_CSV(fileSavePath, BIZ_Variable_OverHang.Title_AA.ToArray(), saveRowDataList.ToArray(), "全相机AA数据");

                }
                else
                {
                    FileOperationCSV.WriteData_CSV(fileSavePath, BIZ_Variable_OverHang.Title_CC.ToArray(), saveRowDataList.ToArray(), "全相机CC数据");

                }

            }

        }

        public void WriteData_AllCamData(string fileSavePath, CamEACornerProduct currentCamEAProduct1, CamEACornerProduct currentCamEAProduct2, CamEACornerProduct currentCamEAProduct3, CamEACornerProduct currentCamEAProduct4,
              long cellNumber, int UpOrDownCell, int CurrentLayer, string qrCode, out bool EAResult, out string EAResultStr)
        {
            lock (AllCamData_Lock)
            {
                List<RowData> showRowDataList = new List<RowData>();
                List<RowData> saveRowDataList = new List<RowData>();

                string UpOrDownCellStr = UpOrDownCell.ToString();
                if (UpOrDownCell == 1) UpOrDownCellStr = "Bot";
                if (UpOrDownCell == 2) UpOrDownCellStr = "Up";

                RowData timeRowData = new RowData() { dataStr = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff"), resultType = ResultType.OK };
                RowData frameRowData = new RowData() { dataStr = currentCamEAProduct1.CamFrame.ToString(), resultType = ResultType.OK };
                RowData lastFrameRowData = new RowData() { dataStr = currentCamEAProduct1.LastCamFrame.ToString(), resultType = ResultType.OK };
                RowData cellNumberData = new RowData() { dataStr = cellNumber.ToString(), resultType = ResultType.OK };
                RowData UpOrDownCellData = new RowData() { dataStr = UpOrDownCellStr, resultType = ResultType.OK };
                RowData CurrentLayerData = new RowData() { dataStr = CurrentLayer.ToString(), resultType = ResultType.OK };

                string[] prodSizeDataTitle_ACCA = new string[] { "4X", "5X", "5Y", "3Y", "3X", "2X", "2Y", "1X", "1Y", "4Y", "ARC", "Corner1", "Corner2", "AS1", "AS2", "AS3", };

                //当前这一层尺寸数据          
                RowData NGSizeItem = new RowData() { dataStr = "", resultType = ResultType.OK }; //当前这一层总结果    
                GetSizeDataInfo(currentCamEAProduct1, "4X", ref NGSizeItem, out RowData sizeData_4X, out RowData sizeResult_4X);
                GetSizeDataInfo(currentCamEAProduct3, "5X", ref NGSizeItem, out RowData sizeData_5X, out RowData sizeResult_5X);
                GetSizeDataInfo(currentCamEAProduct3, "5Y", ref NGSizeItem, out RowData sizeData_5Y, out RowData sizeResult_5Y);
                GetSizeDataInfo(currentCamEAProduct4, "3Y", ref NGSizeItem, out RowData sizeData_3Y, out RowData sizeResult_3Y);
                GetSizeDataInfo(currentCamEAProduct4, "3X", ref NGSizeItem, out RowData sizeData_3X, out RowData sizeResult_3X);
                GetSizeDataInfo(currentCamEAProduct4, "2X", ref NGSizeItem, out RowData sizeData_2X, out RowData sizeResult_2X);
                GetSizeDataInfo(currentCamEAProduct4, "2Y", ref NGSizeItem, out RowData sizeData_2Y, out RowData sizeResult_2Y);
                GetSizeDataInfo(currentCamEAProduct2, "1X", ref NGSizeItem, out RowData sizeData_1X, out RowData sizeResult_1X);
                GetSizeDataInfo(currentCamEAProduct2, "1Y", ref NGSizeItem, out RowData sizeData_1Y, out RowData sizeResult_1Y);
                GetSizeDataInfo(currentCamEAProduct1, "4Y", ref NGSizeItem, out RowData sizeData_4Y, out RowData sizeResult_4Y);
                GetSizeDataInfo(currentCamEAProduct4, "ARC", ref NGSizeItem, out RowData sizeData_ARC, out RowData sizeResult_ARC);
                GetSizeDataInfo(currentCamEAProduct4, "Corner1", ref NGSizeItem, out RowData sizeData_Corner1, out RowData sizeResult_Corner1);
                GetSizeDataInfo(currentCamEAProduct4, "Corner2", ref NGSizeItem, out RowData sizeData_Corner2, out RowData sizeResult_Corner2);
                GetSizeDataInfo(currentCamEAProduct2, "AS1", ref NGSizeItem, out RowData sizeData_AS1, out RowData sizeResult_AS1);
                GetSizeDataInfo(currentCamEAProduct4, "AS2", ref NGSizeItem, out RowData sizeData_AS2, out RowData sizeResult_AS2);
                GetSizeDataInfo(currentCamEAProduct3, "AS3", ref NGSizeItem, out RowData sizeData_AS3, out RowData sizeResult_AS3);

                //Tabneck值
                RowData ZM_TabneckData = new RowData() { dataStr = currentCamEAProduct1.UpTabneckValue.ToString("F3"), resultType = ResultType.OK };
                RowData FM_TabneckData = new RowData() { dataStr = currentCamEAProduct1.DownTabneckValue.ToString("F3"), resultType = ResultType.OK };



                //尺寸数据

                RowData PoleWidth1Data = new RowData() { dataStr = currentCamEAProduct1.PoleWidth1.ToString("F3"), resultType = ResultType.OK };
                RowData PoleWidth2Data = new RowData() { dataStr = currentCamEAProduct1.PoleWidth2.ToString("F3"), resultType = ResultType.OK };
                RowData PoleHeight1Data = new RowData() { dataStr = currentCamEAProduct1.PoleHeight1.ToString("F3"), resultType = ResultType.OK };
                RowData PoleHeight2Data = new RowData() { dataStr = currentCamEAProduct1.PoleHeight2.ToString("F3"), resultType = ResultType.OK };
                RowData PoleEarWData = new RowData() { dataStr = currentCamEAProduct1.PoleEarW.ToString("F3"), resultType = ResultType.OK };
                RowData PoleEarHData = new RowData() { dataStr = currentCamEAProduct1.PoleEarH.ToString("F3"), resultType = ResultType.OK };
                RowData PoleEarDData = new RowData() { dataStr = currentCamEAProduct1.PoleEarD.ToString("F3"), resultType = ResultType.OK };

                //各个相机Mark尺寸
                RowData MarkWDataCam1 = new RowData() { dataStr = currentCamEAProduct1.MarkSizeW.ToString("F3"), resultType = ResultType.OK };
                RowData MarkHDataCam1 = new RowData() { dataStr = currentCamEAProduct1.MarkSizeH.ToString("F3"), resultType = ResultType.OK };

                RowData MarkWDataCam2 = new RowData() { dataStr = currentCamEAProduct2.MarkSizeW.ToString("F3"), resultType = ResultType.OK };
                RowData MarkHDataCam2 = new RowData() { dataStr = currentCamEAProduct2.MarkSizeH.ToString("F3"), resultType = ResultType.OK };

                RowData MarkWDataCam3 = new RowData() { dataStr = currentCamEAProduct3.MarkSizeW.ToString("F3"), resultType = ResultType.OK };
                RowData MarkHDataCam3 = new RowData() { dataStr = currentCamEAProduct3.MarkSizeH.ToString("F3"), resultType = ResultType.OK };

                RowData MarkWDataCam4 = new RowData() { dataStr = currentCamEAProduct4.MarkSizeW.ToString("F3"), resultType = ResultType.OK };
                RowData MarkHDataCam4 = new RowData() { dataStr = currentCamEAProduct4.MarkSizeH.ToString("F3"), resultType = ResultType.OK };

                //当前这一层AACC尺寸数据 
                GetSizeAACCDataInfo(currentCamEAProduct1, "4Y", ref NGSizeItem, out RowData sizeAACCData_4Y, out RowData sizeAACCResult_4Y);
                GetSizeAACCDataInfo(currentCamEAProduct1, "4X", ref NGSizeItem, out RowData sizeAACCData_4X, out RowData sizeAACCResult_4X);
                GetSizeAACCDataInfo(currentCamEAProduct2, "1Y", ref NGSizeItem, out RowData sizeAACCData_1Y, out RowData sizeAACCResult_1Y);
                GetSizeAACCDataInfo(currentCamEAProduct2, "1X", ref NGSizeItem, out RowData sizeAACCData_1X, out RowData sizeAACCResult_1X);
                GetSizeAACCDataInfo(currentCamEAProduct4, "2Y", ref NGSizeItem, out RowData sizeAACCData_2Y, out RowData sizeAACCResult_2Y);
                GetSizeAACCDataInfo(currentCamEAProduct4, "2X", ref NGSizeItem, out RowData sizeAACCData_2X, out RowData sizeAACCResult_2X);
                GetSizeAACCDataInfo(currentCamEAProduct4, "3X", ref NGSizeItem, out RowData sizeAACCData_3X, out RowData sizeAACCResult_3X);
                GetSizeAACCDataInfo(currentCamEAProduct4, "3Y", ref NGSizeItem, out RowData sizeAACCData_3Y, out RowData sizeAACCResult_3Y);
                GetSizeAACCDataInfo(currentCamEAProduct3, "5Y", ref NGSizeItem, out RowData sizeAACCData_5Y, out RowData sizeAACCResult_5Y);
                GetSizeAACCDataInfo(currentCamEAProduct3, "5X", ref NGSizeItem, out RowData sizeAACCData_5X, out RowData sizeAACCResult_5X);



                saveRowDataList.Add(timeRowData);
                saveRowDataList.Add(frameRowData);
                saveRowDataList.Add(lastFrameRowData);
                saveRowDataList.Add(cellNumberData);
                saveRowDataList.Add(UpOrDownCellData);
                saveRowDataList.Add(CurrentLayerData);
                saveRowDataList.Add(sizeData_4X);
                saveRowDataList.Add(sizeData_5X);
                saveRowDataList.Add(sizeData_5Y);
                saveRowDataList.Add(sizeData_3Y);
                saveRowDataList.Add(sizeData_3X);
                saveRowDataList.Add(sizeData_2X);
                saveRowDataList.Add(sizeData_2Y);
                saveRowDataList.Add(sizeData_1X);
                saveRowDataList.Add(sizeData_1Y);
                saveRowDataList.Add(sizeData_4Y);
                saveRowDataList.Add(sizeData_ARC);
                saveRowDataList.Add(sizeData_Corner1);
                saveRowDataList.Add(sizeData_Corner2);
                saveRowDataList.Add(sizeData_AS1);
                saveRowDataList.Add(sizeData_AS2);
                saveRowDataList.Add(sizeData_AS3);

                saveRowDataList.Add(ZM_TabneckData);
                saveRowDataList.Add(FM_TabneckData);

                saveRowDataList.Add(PoleWidth1Data);
                saveRowDataList.Add(PoleWidth2Data);
                saveRowDataList.Add(PoleHeight1Data);
                saveRowDataList.Add(PoleHeight2Data);
                saveRowDataList.Add(PoleEarWData);
                saveRowDataList.Add(PoleEarHData);
                saveRowDataList.Add(PoleEarDData);

                saveRowDataList.Add(MarkWDataCam1);
                saveRowDataList.Add(MarkHDataCam1);
                saveRowDataList.Add(MarkWDataCam2);
                saveRowDataList.Add(MarkHDataCam2);
                saveRowDataList.Add(MarkWDataCam3);
                saveRowDataList.Add(MarkHDataCam3);
                saveRowDataList.Add(MarkWDataCam4);
                saveRowDataList.Add(MarkHDataCam4);


                saveRowDataList.Add(sizeAACCData_4Y);
                saveRowDataList.Add(sizeAACCData_4X);
                saveRowDataList.Add(sizeAACCData_1Y);
                saveRowDataList.Add(sizeAACCData_1X);
                saveRowDataList.Add(sizeAACCData_2Y);
                saveRowDataList.Add(sizeAACCData_2X);
                saveRowDataList.Add(sizeAACCData_3X);
                saveRowDataList.Add(sizeAACCData_3Y);
                saveRowDataList.Add(sizeAACCData_5Y);
                saveRowDataList.Add(sizeAACCData_5X);

                if (NGSizeItem.resultType == ResultType.NG)
                {
                    EAResult = false;
                    NGSizeItem.dataStr = "NG/" + NGSizeItem.dataStr;
                    EAResultStr = NGSizeItem.dataStr;
                }
                else
                {
                    EAResult = true;
                    NGSizeItem.dataStr = "OK";
                    EAResultStr = NGSizeItem.dataStr;
                }
                saveRowDataList.Add(NGSizeItem);
                //下电芯最后一层
                if (UpOrDownCell == 1 && CurrentLayer == overHangCraft.StakeCathodeCount)
                {
                    GetDownCellResult(out short result, out string resultStr);
                    saveRowDataList.Add(new RowData() { dataStr = resultStr });
                }
                //上电芯最后一层
                else if (UpOrDownCell == 2 && CurrentLayer == overHangCraft.StakeCathodeCount)
                {
                    GetUpCellResult(out short result, out string resultStr);
                    saveRowDataList.Add(new RowData() { dataStr = resultStr });
                }
                else
                {
                    saveRowDataList.Add(new RowData() { dataStr = "" });
                }
                saveRowDataList.Add(new RowData() { dataStr = qrCode });

                //显示数据
                showRowDataList = saveRowDataList.ToList();
                NListView2.Insert(form.listView9, showRowDataList.ToArray(), 100, ListViewShowType.RemoveAtEndRow, false, ListViewAlternateColor);

                FileOperationCSV.WriteData_CSV(fileSavePath, BIZ_Variable_OverHang.Title_TotalTable.ToArray(), saveRowDataList.ToArray(), "Overhang总表数据");
            }
        }

      
        public void WritePieSizeData(string fileSavePath, CamEACornerProduct currentCamEAProduct, List<SizeInfo> sizeInfos, long cellNumber, int UpOrDownCell, int CurrentLayer, string qrCode)
        {
            lock (AllCamSizeData_Lock)
            {
                List<RowData> saveRowDataList = new List<RowData>();

                string UpOrDownCellStr = UpOrDownCell.ToString();
                if (UpOrDownCell == 1) UpOrDownCellStr = "Bot";
                if (UpOrDownCell == 2) UpOrDownCellStr = "Up";
                List<string> titleList = new List<string>();
                RowData timeRowData = new RowData() { dataStr = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff"), resultType = ResultType.OK };
                RowData frameRowData = new RowData() { dataStr = currentCamEAProduct.CamFrame.ToString(), resultType = ResultType.OK };
                RowData lastFrameRowData = new RowData() { dataStr = currentCamEAProduct.LastCamFrame.ToString(), resultType = ResultType.OK };
                RowData cellNumberData = new RowData() { dataStr = cellNumber.ToString(), resultType = ResultType.OK };
                RowData UpOrDownCellData = new RowData() { dataStr = UpOrDownCellStr, resultType = ResultType.OK };
                RowData CurrentLayerData = new RowData() { dataStr = CurrentLayer.ToString(), resultType = ResultType.OK };
                titleList.Add("时间");
                titleList.Add("当前帧");
                titleList.Add("上一帧");
                titleList.Add("单元号");
                titleList.Add("上下电芯");
                titleList.Add("当前层");
                saveRowDataList.Add(timeRowData);
                saveRowDataList.Add(frameRowData);
                saveRowDataList.Add(lastFrameRowData);
                saveRowDataList.Add(cellNumberData);
                saveRowDataList.Add(UpOrDownCellData);
                saveRowDataList.Add(CurrentLayerData);
                foreach (SizeInfo sizeInfo in sizeInfos)
                {
                    saveRowDataList.Add(new RowData() { dataStr = sizeInfo.DataResult.ToString("F3"), resultType = sizeInfo.resultType });
                    saveRowDataList.Add(new RowData() { dataStr = sizeInfo.resultType== ResultType.OK?"OK":"NG" });
                    titleList.Add(sizeInfo.SizeName);
                    titleList.Add("结果");
                }
                titleList.Add("电芯码");
                saveRowDataList.Add(new RowData() { dataStr = qrCode });
                FileOperationCSV.WriteData_CSV(fileSavePath, titleList.ToArray(), saveRowDataList.ToArray(), "Overhang极片尺寸数据");
            }
        }

        public void WriteNGData_AllCamData(string fileSavePath, long cellNumber, int UpOrDownCell, int CurrentLayer, string EAResultStr,
            string screenshotRelativePath_Cam1, string screenshotRelativePath_Cam2, string screenshotRelativePath_Cam3, string screenshotRelativePath_Cam4,
            string screenshotRelativePath_Cam5, string screenshotRelativePath_Cam6, string screenshotRelativePath_Cam7, string screenshotRelativePath_Cam8,
            string primitiveImgSaveRelativePath_Cam1, string primitiveImgSaveRelativePath_Cam2, string primitiveImgSaveRelativePath_Cam3, string primitiveImgSaveRelativePath_Cam4,
            string primitiveImgSaveRelativePath_Cam5, string primitiveImgSaveRelativePath_Cam6, string primitiveImgSaveRelativePath_Cam7, string primitiveImgSaveRelativePath_Cam8)

        {
            lock (AllCamNGData_Lock)
            {
                string UpOrDownCellStr = UpOrDownCell.ToString();
                if (UpOrDownCell == 1) UpOrDownCellStr = "Bot";
                if (UpOrDownCell == 2) UpOrDownCellStr = "Up";

                string imageSaveName = $"{cellNumber}_{UpOrDownCell}_{CurrentLayer}" + "_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff") + ".png";
                //存窗口截图
                Task.Run(() =>
                {
                    SaveWindowScreenshot(Project.Instance.paramPathSet.ProductMojuanPath, Project.Instance.paramPathSet.OVHDataScreenshotRelativePath, imageSaveName, "");
                });

                List<string> ngDataList = new List<string>();
                ngDataList.Add(DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff"));
                ngDataList.Add(cellNumber.ToString());
                ngDataList.Add(UpOrDownCellStr);
                ngDataList.Add(CurrentLayer.ToString());
                ngDataList.Add(EAResultStr);
                ngDataList.Add(screenshotRelativePath_Cam1);
                ngDataList.Add(screenshotRelativePath_Cam2);
                ngDataList.Add(screenshotRelativePath_Cam3);
                ngDataList.Add(screenshotRelativePath_Cam4);
                ngDataList.Add(screenshotRelativePath_Cam5);
                ngDataList.Add(screenshotRelativePath_Cam6);
                ngDataList.Add(screenshotRelativePath_Cam7);
                ngDataList.Add(screenshotRelativePath_Cam8);
                ngDataList.Add(primitiveImgSaveRelativePath_Cam1);
                ngDataList.Add(primitiveImgSaveRelativePath_Cam2);
                ngDataList.Add(primitiveImgSaveRelativePath_Cam3);
                ngDataList.Add(primitiveImgSaveRelativePath_Cam4);
                ngDataList.Add(primitiveImgSaveRelativePath_Cam5);
                ngDataList.Add(primitiveImgSaveRelativePath_Cam6);
                ngDataList.Add(primitiveImgSaveRelativePath_Cam7);
                ngDataList.Add(primitiveImgSaveRelativePath_Cam8);
                ngDataList.Add(Project.Instance.paramPathSet.OVHDataScreenshotRelativePath + imageSaveName);

                //显示数据               
                NListView2.Insert(form.listView10, ngDataList.ToArray(), 100, ListViewShowType.RemoveAtEndRow, true);

                //保存数据
                FileOperationCSV.WriteData_CSV(fileSavePath, BIZ_Variable_OverHang.Title_NGData.ToArray(), ngDataList.ToArray(), "Overhang工位NG数据");
            }
        }

        public void GetSizeDataInfo(CamEACornerProduct currentCamEAProduct, string sizeName, ref RowData sizeNGData, out RowData sizeData, out RowData sizeResult)
        {
            sizeData = new RowData() { dataStr = "", resultType = ResultType.OK };
            sizeResult = new RowData() { dataStr = "NG", resultType = ResultType.NG };
            SizeInfo sizeInfo = null;
            if (currentCamEAProduct != null && currentCamEAProduct.sizeInfos?.Count > 0)
            {
                sizeInfo = currentCamEAProduct.sizeInfos.Find(p => p.Name == sizeName);
            }
            if (sizeInfo != null)
            {
                sizeData.dataStr = sizeInfo.DataResult.ToString("F3");
                sizeData.resultType = sizeInfo.resultType;

                sizeResult.dataStr = sizeInfo.resultType.ToString();
                sizeResult.resultType = sizeInfo.resultType;

                if (sizeInfo.resultType == ResultType.NG)
                {
                    sizeNGData.resultType = ResultType.NG;
                    sizeNGData.dataStr += $"{sizeInfo.SizeName}/";
                }
            }
        }

        public void GetSizeAACCDataInfo(CamEACornerProduct currentCamEAProduct, string sizeName, ref RowData sizeNGData, out RowData sizeData, out RowData sizeResult)
        {
            sizeData = new RowData() { dataStr = "", resultType = ResultType.OK };
            sizeResult = new RowData() { dataStr = "NG", resultType = ResultType.NG };
            SizeInfo sizeInfo = null;
            if (currentCamEAProduct != null && currentCamEAProduct.sizeMonitorInfos?.Count > 0)
            {
                sizeInfo = currentCamEAProduct.sizeMonitorInfos.Find(p => p.Name == sizeName);
            }
            if (sizeInfo != null)
            {
                sizeData.dataStr = sizeInfo.DataResult.ToString("F3");
                sizeData.resultType = sizeInfo.resultType;

                sizeResult.dataStr = sizeInfo.resultType.ToString();
                sizeResult.resultType = sizeInfo.resultType;

                if (sizeInfo.resultType == ResultType.NG)
                {
                    sizeNGData.resultType = ResultType.NG;
                    sizeNGData.dataStr += $"{sizeInfo.SizeName}/";
                }
            }
        }

        public void SaveWindowScreenshot(string imageSaveBasePath, string imageSaveRelativePath, string saveFileName, string source)
        {
            try
            {
                lock (SaveWindowScreenshot_lockObj)
                {
                    form.SaveWindowScreenshot(imageSaveBasePath, imageSaveRelativePath, saveFileName, source);
                }

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(source, ex);
            }
        }

        /// <summary>
        /// 获取相机数据
        /// </summary>
        /// <param name="cameraEnum"></param>
        /// <param name="PackgeNum"></param>
        /// <param name="EAIndex"></param>
        /// <returns></returns>
        private CamEACornerProduct GetCamData_Cam1(int PackgeNum, int EAIndex)
        {
            CamEACornerProduct camEACornerProduct = null;
            lock (EACornerProductLock_Cam1)
            {
                int count = bIZ_Variable.batteryProduct.EACornerProduct_Cam1.Where(p => p.PackgeNum == PackgeNum && p.EAIndex == EAIndex).Count();
                if (count > 0)
                {
                    camEACornerProduct = bIZ_Variable.batteryProduct.EACornerProduct_Cam1.Find(p => p.PackgeNum == PackgeNum && p.EAIndex == EAIndex);
                }
                if (count > 1)
                {
                    Log4netRecord.RunLog.Error($"相机1层数异常，数量：{count}，电芯编号：{PackgeNum}，层数{EAIndex}");
                }
            }
            return camEACornerProduct;
        }
        /// <summary>
        /// 获取相机数据
        /// </summary>
        /// <param name="cameraEnum"></param>
        /// <param name="PackgeNum"></param>
        /// <param name="EAIndex"></param>
        /// <returns></returns>
        private CamEACornerProduct GetCamData_Cam2(int PackgeNum, int EAIndex)
        {
            CamEACornerProduct camEACornerProduct = null;
            lock (EACornerProductLock_Cam2)
            {
                int count = bIZ_Variable.batteryProduct.EACornerProduct_Cam2.Where(p => p.PackgeNum == PackgeNum && p.EAIndex == EAIndex).Count();
                if (count > 0)
                {
                    camEACornerProduct = bIZ_Variable.batteryProduct.EACornerProduct_Cam2.Find(p => p.PackgeNum == PackgeNum && p.EAIndex == EAIndex);
                }
                if (count > 1)
                {
                    Log4netRecord.RunLog.Error($"相机2层数异常，数量：{count}，电芯编号：{PackgeNum}，层数{EAIndex}");
                }
            }
            return camEACornerProduct;
        }
        /// <summary>
        /// 获取相机数据
        /// </summary>
        /// <param name="cameraEnum"></param>
        /// <param name="PackgeNum"></param>
        /// <param name="EAIndex"></param>
        /// <returns></returns>
        private CamEACornerProduct GetCamData_Cam3(int PackgeNum, int EAIndex)
        {
            CamEACornerProduct camEACornerProduct = null;
            lock (EACornerProductLock_Cam3)
            {
                int count = bIZ_Variable.batteryProduct.EACornerProduct_Cam3.Where(p => p.PackgeNum == PackgeNum && p.EAIndex == EAIndex).Count();
                if (count > 0)
                {
                    camEACornerProduct = bIZ_Variable.batteryProduct.EACornerProduct_Cam3.Find(p => p.PackgeNum == PackgeNum && p.EAIndex == EAIndex);
                }
                if (count > 1)
                {
                    Log4netRecord.RunLog.Error($"相机3层数异常，数量：{count}，电芯编号：{PackgeNum}，层数{EAIndex}");
                }
            }
            return camEACornerProduct;
        }
        /// <summary>
        /// 获取相机数据
        /// </summary>
        /// <param name="cameraEnum"></param>
        /// <param name="PackgeNum"></param>
        /// <param name="EAIndex"></param>
        /// <returns></returns>
        private CamEACornerProduct GetCamData_Cam4(int PackgeNum, int EAIndex)
        {
            CamEACornerProduct camEACornerProduct = null;
            lock (EACornerProductLock_Cam4)
            {
                int count = bIZ_Variable.batteryProduct.EACornerProduct_Cam4.Where(p => p.PackgeNum == PackgeNum && p.EAIndex == EAIndex).Count();
                if (count > 0)
                {
                    camEACornerProduct = bIZ_Variable.batteryProduct.EACornerProduct_Cam4.Find(p => p.PackgeNum == PackgeNum && p.EAIndex == EAIndex);
                }
                if (count > 1)
                {
                    Log4netRecord.RunLog.Error($"相机4层数异常，数量：{count}，电芯编号：{PackgeNum}，层数{EAIndex}");
                }
            }
            return camEACornerProduct;
        }
        /// <summary>
        /// 获取相机数据
        /// </summary>
        /// <param name="cameraEnum"></param>
        /// <param name="PackgeNum"></param>
        /// <param name="EAIndex"></param>
        /// <returns></returns>
        private CamEACornerProduct GetCamData_Cam5(int PackgeNum, int EAIndex)
        {
            CamEACornerProduct camEACornerProduct = null;
            lock (EACornerProductLock_Cam5)
            {
                int count = bIZ_Variable.batteryProduct.EACornerProduct_Cam5.Where(p => p.PackgeNum == PackgeNum && p.EAIndex == EAIndex).Count();
                if (count > 0)
                {
                    camEACornerProduct = bIZ_Variable.batteryProduct.EACornerProduct_Cam5.Find(p => p.PackgeNum == PackgeNum && p.EAIndex == EAIndex);
                }
                if (count > 1)
                {
                    Log4netRecord.RunLog.Error($"相机5层数异常，数量：{count}，电芯编号：{PackgeNum}，层数{EAIndex}");
                }
            }
            return camEACornerProduct;
        }
        /// <summary>
        /// 获取相机数据
        /// </summary>
        /// <param name="cameraEnum"></param>
        /// <param name="PackgeNum"></param>
        /// <param name="EAIndex"></param>
        /// <returns></returns>
        private CamEACornerProduct GetCamData_Cam6(int PackgeNum, int EAIndex)
        {
            CamEACornerProduct camEACornerProduct = null;
            lock (EACornerProductLock_Cam6)
            {
                int count = bIZ_Variable.batteryProduct.EACornerProduct_Cam6.Where(p => p.PackgeNum == PackgeNum && p.EAIndex == EAIndex).Count();
                if (count > 0)
                {
                    camEACornerProduct = bIZ_Variable.batteryProduct.EACornerProduct_Cam6.Find(p => p.PackgeNum == PackgeNum && p.EAIndex == EAIndex);
                }
                if (count > 1)
                {
                    Log4netRecord.RunLog.Error($"相机6层数异常，数量：{count}，电芯编号：{PackgeNum}，层数{EAIndex}");
                }
            }
            return camEACornerProduct;
        }
        /// <summary>
        /// 获取相机数据
        /// </summary>
        /// <param name="cameraEnum"></param>
        /// <param name="PackgeNum"></param>
        /// <param name="EAIndex"></param>
        /// <returns></returns>
        private CamEACornerProduct GetCamData_Cam7(int PackgeNum, int EAIndex)
        {
            CamEACornerProduct camEACornerProduct = null;
            lock (EACornerProductLock_Cam7)
            {
                int count = bIZ_Variable.batteryProduct.EACornerProduct_Cam7.Where(p => p.PackgeNum == PackgeNum && p.EAIndex == EAIndex).Count();
                if (count > 0)
                {
                    camEACornerProduct = bIZ_Variable.batteryProduct.EACornerProduct_Cam7.Find(p => p.PackgeNum == PackgeNum && p.EAIndex == EAIndex);
                }
                if (count > 1)
                {
                    Log4netRecord.RunLog.Error($"相机7层数异常，数量：{count}，电芯编号：{PackgeNum}，层数{EAIndex}");
                }
            }
            return camEACornerProduct;
        }
        /// <summary>
        /// 获取相机数据
        /// </summary>
        /// <param name="cameraEnum"></param>
        /// <param name="PackgeNum"></param>
        /// <param name="EAIndex"></param>
        /// <returns></returns>
        private CamEACornerProduct GetCamData_Cam8(int PackgeNum, int EAIndex)
        {
            CamEACornerProduct camEACornerProduct = null;
            lock (EACornerProductLock_Cam8)
            {
                int count = bIZ_Variable.batteryProduct.EACornerProduct_Cam8.Where(p => p.PackgeNum == PackgeNum && p.EAIndex == EAIndex).Count();
                if (count > 0)
                {
                    camEACornerProduct = bIZ_Variable.batteryProduct.EACornerProduct_Cam8.Find(p => p.PackgeNum == PackgeNum && p.EAIndex == EAIndex);
                }
                if (count > 1)
                {
                    Log4netRecord.RunLog.Error($"相机8层数异常，数量：{count}，电芯编号：{PackgeNum}，层数{EAIndex}");
                }
            }
            return camEACornerProduct;
        }

        /// <summary>
        /// 获取相机最后一个EA数据
        /// </summary>
        /// <param name="cameraEnum"></param>
        /// <param name="PackgeNum"></param>
        /// <param name="EAIndex"></param>
        /// <returns></returns>
        private CamEACornerProduct GetCamLastOneEAData(CameraEnum cameraEnum)
        {
            CamEACornerProduct camEACornerProduct = null;
            if (cameraEnum == CameraEnum.Cam1)
            {
                lock (EACornerProductLock_Cam1)
                {
                    if (bIZ_Variable.batteryProduct.EACornerProduct_Cam1.Count() > 0)
                    {
                        camEACornerProduct = bIZ_Variable.batteryProduct.EACornerProduct_Cam1[bIZ_Variable.batteryProduct.EACornerProduct_Cam1.Count - 1];
                    }
                }
            }
            else if (cameraEnum == CameraEnum.Cam2)
            {
                lock (EACornerProductLock_Cam2)
                {
                    if (bIZ_Variable.batteryProduct.EACornerProduct_Cam2.Count() > 0)
                    {
                        camEACornerProduct = bIZ_Variable.batteryProduct.EACornerProduct_Cam2[bIZ_Variable.batteryProduct.EACornerProduct_Cam2.Count - 1];
                    }
                }
            }
            else if (cameraEnum == CameraEnum.Cam3)
            {
                lock (EACornerProductLock_Cam3)
                {
                    if (bIZ_Variable.batteryProduct.EACornerProduct_Cam3.Count() > 0)
                    {
                        camEACornerProduct = bIZ_Variable.batteryProduct.EACornerProduct_Cam3[bIZ_Variable.batteryProduct.EACornerProduct_Cam3.Count - 1];
                    }
                }
            }
            else if (cameraEnum == CameraEnum.Cam4)
            {
                lock (EACornerProductLock_Cam4)
                {
                    if (bIZ_Variable.batteryProduct.EACornerProduct_Cam4.Count() > 0)
                    {
                        camEACornerProduct = bIZ_Variable.batteryProduct.EACornerProduct_Cam4[bIZ_Variable.batteryProduct.EACornerProduct_Cam4.Count - 1];
                    }
                }
            }
            else if (cameraEnum == CameraEnum.Cam5)
            {
                lock (EACornerProductLock_Cam5)
                {
                    if (bIZ_Variable.batteryProduct.EACornerProduct_Cam5.Count() > 0)
                    {
                        camEACornerProduct = bIZ_Variable.batteryProduct.EACornerProduct_Cam5[bIZ_Variable.batteryProduct.EACornerProduct_Cam5.Count - 1];
                    }
                }
            }
            else if (cameraEnum == CameraEnum.Cam6)
            {
                lock (EACornerProductLock_Cam6)
                {
                    if (bIZ_Variable.batteryProduct.EACornerProduct_Cam6.Count() > 0)
                    {
                        camEACornerProduct = bIZ_Variable.batteryProduct.EACornerProduct_Cam6[bIZ_Variable.batteryProduct.EACornerProduct_Cam6.Count - 1];
                    }
                }
            }
            else if (cameraEnum == CameraEnum.Cam7)
            {
                lock (EACornerProductLock_Cam7)
                {
                    if (bIZ_Variable.batteryProduct.EACornerProduct_Cam7.Count() > 0)
                    {
                        camEACornerProduct = bIZ_Variable.batteryProduct.EACornerProduct_Cam7[bIZ_Variable.batteryProduct.EACornerProduct_Cam7.Count - 1];
                    }
                }
            }
            else if (cameraEnum == CameraEnum.Cam8)
            {
                lock (EACornerProductLock_Cam8)
                {
                    if (bIZ_Variable.batteryProduct.EACornerProduct_Cam8.Count() > 0)
                    {
                        camEACornerProduct = bIZ_Variable.batteryProduct.EACornerProduct_Cam8[bIZ_Variable.batteryProduct.EACornerProduct_Cam8.Count - 1];
                    }
                }
            }

            return camEACornerProduct;
        }

        /// <summary>
        /// 电芯结果判断
        /// </summary>
        /// <param name="camEAProduct"></param>
        private void PackgeResultJudgment(CamEACornerProduct camEAProduct)
        {
            lock (Result_Lock)
            {
                //下电芯结果
                if (camEAProduct.PackgeNum == 1)
                {
                    bIZ_Variable.DownDiaphragmResult &= camEAProduct.DiaphragmResult;
                    bIZ_Variable.DownPolePieceResult &= camEAProduct.PolePieceResult;
                }
                //上电芯结果
                if (camEAProduct.PackgeNum == 2)
                {
                    bIZ_Variable.UpDiaphragmResult &= camEAProduct.DiaphragmResult;
                    bIZ_Variable.UpPolePieceResult &= camEAProduct.PolePieceResult;
                }
            }
        }

        /// <summary>
        /// 计算电芯结果
        /// </summary>
        /// <returns>1:OK    2:ACNG(极片NG)   3:ASNG隔膜NG</returns>
        private void GetCellResult(out short downCellResult, out short upCellResult)
        {
            bool DownDiaphragmResult = bIZ_Variable.DownDiaphragmResult;
            bool DownPolePieceResult = bIZ_Variable.DownPolePieceResult;
            bool UpDiaphragmResult = bIZ_Variable.UpDiaphragmResult;
            bool UpPolePieceResult = bIZ_Variable.UpPolePieceResult;

            //========================================
            //判断下电芯结果
            downCellResult = 0;
            //OK
            if (DownDiaphragmResult && DownPolePieceResult)
            {
                downCellResult = 1;
            }
            //全部NG
            else if (!DownDiaphragmResult && !DownPolePieceResult)
            {
                downCellResult = 2;
            }
            //极片NG
            else if (!DownPolePieceResult)
            {
                downCellResult = 2;
            }
            //隔膜NG
            else if (!DownDiaphragmResult)
            {
                downCellResult = 3;
            }

            //========================================
            //判断上电芯结果
            upCellResult = 0;
            //OK
            if (UpDiaphragmResult && UpPolePieceResult)
            {
                upCellResult = 1;
            }
            //全部NG
            else if (!UpDiaphragmResult && !UpPolePieceResult)
            {
                upCellResult = 2;
            }
            //极片NG
            else if (!UpPolePieceResult)
            {
                upCellResult = 2;
            }
            //隔膜NG
            else if (!UpDiaphragmResult)
            {
                upCellResult = 3;
            }

        }

        /// <summary>
        /// 计算上电芯结果
        /// </summary>
        /// <returns>1:OK    2:ACNG(极片NG)   3:ASNG隔膜NG</returns>
        private void GetUpCellResult(out short cellResult, out string cellResultStr)
        {
            bool UpDiaphragmResult = bIZ_Variable.UpDiaphragmResult;
            bool UpPolePieceResult = bIZ_Variable.UpPolePieceResult;

            //========================================
            //判断上电芯结果
            cellResult = 0;
            cellResultStr = "OK";
            //OK
            if (UpDiaphragmResult && UpPolePieceResult)
            {
                cellResult = 1;
            }
            //全部NG
            else if (!UpDiaphragmResult && !UpPolePieceResult)
            {
                cellResult = 2;
                cellResultStr = "AC NG";
            }
            //极片NG
            else if (!UpPolePieceResult)
            {
                cellResult = 2;
                cellResultStr = "AC NG";
            }
            //隔膜NG
            else if (!UpDiaphragmResult)
            {
                cellResult = 3;
                cellResultStr = "AS NG";
            }

        }
        /// <summary>
        /// 计算下电芯结果
        /// </summary>
        /// <returns>1:OK    2:ACNG(极片NG)   3:ASNG隔膜NG</returns>
        private void GetDownCellResult(out short cellResult, out string cellResultStr)
        {
            bool DownDiaphragmResult = bIZ_Variable.DownDiaphragmResult;
            bool DownPolePieceResult = bIZ_Variable.DownPolePieceResult;

            //========================================
            //判断下电芯结果
            cellResult = 0;
            cellResultStr = "OK";
            //OK
            if (DownDiaphragmResult && DownPolePieceResult)
            {
                cellResult = 1;
            }
            //全部NG
            else if (!DownDiaphragmResult && !DownPolePieceResult)
            {
                cellResult = 2;
                cellResultStr = "AC NG";
            }
            //极片NG
            else if (!DownPolePieceResult)
            {
                cellResult = 2;
                cellResultStr = "AC NG";
            }
            //隔膜NG
            else if (!DownDiaphragmResult)
            {
                cellResult = 3;
                cellResultStr = "AS NG";
            }

        }
        public short ReadPhotoSignalA()
        {
            short readValue = 0;
            if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面极片复检工位)
                Project.Instance.cutOverlayPLCManage.ReadAndWriteZero(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PhotoSignal_AAOVH, Project.Instance.cutOverlayPLCManage.CommunicationLog, out readValue);
            else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面极片复检工位)
                Project.Instance.cutOverlayPLCManage.ReadAndWriteZero(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PhotoSignal_BAOVH, Project.Instance.cutOverlayPLCManage.CommunicationLog, out readValue);

            return readValue;
        }
        public short ReadPhotoSignalB()
        {
            short value = 0;
            if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面极片复检工位)
                Project.Instance.cutOverlayPLCManage.ReadAndWriteZero(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PhotoSignal_ABOVH, Project.Instance.cutOverlayPLCManage.CommunicationLog, out value);
            else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面极片复检工位)
                Project.Instance.cutOverlayPLCManage.ReadAndWriteZero(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PhotoSignal_BBOVH, Project.Instance.cutOverlayPLCManage.CommunicationLog, out value);

            return value;
        }
        public void SendHeartBeatA()
        {
            if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面极片复检工位)
            {
                bool IsIntervalTime = intervalExecutionA.GetWhetherToReachIntervalTime(1000);
                if (IsIntervalTime)
                {
                    //心跳信号
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_Heart_CCD_AAOVH.Value = 1;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_Heart_CCD_AAOVH, true, Project.Instance.cutOverlayPLCManage.HeartbeatLog);
                }
            }

            else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面极片复检工位)
            {
                bool IsIntervalTime = intervalExecutionA.GetWhetherToReachIntervalTime(1000);
                if (IsIntervalTime)
                {
                    //心跳信号
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_Heart_CCD_BAOVH.Value = 1;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_Heart_CCD_BAOVH, true, Project.Instance.cutOverlayPLCManage.HeartbeatLog);
                }
            }
        }
        public void SendHeartBeatB()
        {
            if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面极片复检工位)
            {
                bool IsIntervalTime = intervalExecutionB.GetWhetherToReachIntervalTime(1000);
                if (IsIntervalTime)
                {
                    //心跳信号
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_Heart_CCD_ABOVH.Value = 1;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_Heart_CCD_ABOVH, true, Project.Instance.cutOverlayPLCManage.HeartbeatLog);
                }
            }

            else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面极片复检工位)
            {
                bool IsIntervalTime = intervalExecutionB.GetWhetherToReachIntervalTime(1000);
                if (IsIntervalTime)
                {
                    //心跳信号
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_Heart_CCD_BBOVH.Value = 1;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_Heart_CCD_BBOVH, true, Project.Instance.cutOverlayPLCManage.HeartbeatLog);
                }
            }
        }

        /// <summary>
        /// 获取产品结果
        /// </summary>
        /// <returns></returns>
        public ProcessExecutionResult GetProdResultA()
        {
            try
            {
                //bool IsIntervalTime = intervalExecutionGetDataA.GetWhetherToReachIntervalTime(2);
                //if (!IsIntervalTime) return ProcessExecutionResult.ContinueExecute;
                //================================================================
                //【1】产品判断
                Thread.Sleep(2);
                CamEACornerProduct EACornerProduct_Cam1 = GetCamData_Cam1(bIZ_Variable.UpOrDownCellA, bIZ_Variable.CurrentLayerA);
                CamEACornerProduct EACornerProduct_Cam2 = GetCamData_Cam2(bIZ_Variable.UpOrDownCellA, bIZ_Variable.CurrentLayerA);
                CamEACornerProduct EACornerProduct_Cam3 = GetCamData_Cam3(bIZ_Variable.UpOrDownCellA, bIZ_Variable.CurrentLayerA);
                CamEACornerProduct EACornerProduct_Cam4 = GetCamData_Cam4(bIZ_Variable.UpOrDownCellA, bIZ_Variable.CurrentLayerA);


                if (EACornerProduct_Cam1 != null &&
                    EACornerProduct_Cam2 != null &&
                    EACornerProduct_Cam3 != null &&
                    EACornerProduct_Cam4 != null &&
                    EACornerProduct_Cam1.CamFinish == true &&
                    EACornerProduct_Cam2.CamFinish == true &&
                    EACornerProduct_Cam3.CamFinish == true &&
                    EACornerProduct_Cam4.CamFinish == true)
                {
                    bIZ_Variable.CurrentEAResultA = EACornerProduct_Cam1.DiaphragmResult &&
                                                    EACornerProduct_Cam2.DiaphragmResult &&
                                                    EACornerProduct_Cam3.DiaphragmResult &&
                                                    EACornerProduct_Cam4.DiaphragmResult &&
                                                    EACornerProduct_Cam1.PolePieceResult &&
                                                    EACornerProduct_Cam2.PolePieceResult &&
                                                    EACornerProduct_Cam3.PolePieceResult &&
                                                    EACornerProduct_Cam4.PolePieceResult;


                    return ProcessExecutionResult.Success;
                }

                return ProcessExecutionResult.ContinueExecute;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                return ProcessExecutionResult.ContinueExecute;
            }

        }
        /// <summary>
        /// 获取产品结果
        /// </summary>
        /// <returns></returns>
        public ProcessExecutionResult GetProdResultB()
        {
            try
            {
                //================================================================
                //【1】产品判断
                Thread.Sleep(2);
                CamEACornerProduct EACornerProduct_Cam1 = GetCamData_Cam5(bIZ_Variable.UpOrDownCellB, bIZ_Variable.CurrentLayerB);
                CamEACornerProduct EACornerProduct_Cam2 = GetCamData_Cam6(bIZ_Variable.UpOrDownCellB, bIZ_Variable.CurrentLayerB);
                CamEACornerProduct EACornerProduct_Cam3 = GetCamData_Cam7(bIZ_Variable.UpOrDownCellB, bIZ_Variable.CurrentLayerB);
                CamEACornerProduct EACornerProduct_Cam4 = GetCamData_Cam8(bIZ_Variable.UpOrDownCellB, bIZ_Variable.CurrentLayerB);

                if (EACornerProduct_Cam1 != null &&
                    EACornerProduct_Cam2 != null &&
                    EACornerProduct_Cam3 != null &&
                    EACornerProduct_Cam4 != null &&
                    EACornerProduct_Cam1.CamFinish == true &&
                    EACornerProduct_Cam2.CamFinish == true &&
                    EACornerProduct_Cam3.CamFinish == true &&
                    EACornerProduct_Cam4.CamFinish == true)
                {
                    bIZ_Variable.CurrentEAResultB = EACornerProduct_Cam1.DiaphragmResult &&
                                                    EACornerProduct_Cam2.DiaphragmResult &&
                                                    EACornerProduct_Cam3.DiaphragmResult &&
                                                    EACornerProduct_Cam4.DiaphragmResult &&
                                                    EACornerProduct_Cam1.PolePieceResult &&
                                                    EACornerProduct_Cam2.PolePieceResult &&
                                                    EACornerProduct_Cam3.PolePieceResult &&
                                                    EACornerProduct_Cam4.PolePieceResult;

                    return ProcessExecutionResult.Success;
                }

                return ProcessExecutionResult.ContinueExecute;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                return ProcessExecutionResult.ContinueExecute;
            }
        }


        /// <summary>
        /// 发送下电芯结果
        /// </summary>
        public void SendDownCellResult(short cellResult)
        {
            if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面极片复检工位)
            {
                Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult2_AOVHResult.Value = cellResult;
                Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult2_AOVHResult, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
            }
            else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面极片复检工位)
            {
                Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult2_BOVHResult.Value = cellResult;
                Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult2_BOVHResult, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
            }
        }

        /// <summary>
        /// 发送上电芯结果
        /// </summary>
        public void SendUpCellResult(short cellResult)
        {
            if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面极片复检工位)
            {
                Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult1_AOVHResult.Value = cellResult;
                Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult1_AOVHResult, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);


            }
            else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面极片复检工位)
            {
                Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult1_BOVHResult.Value = cellResult;
                Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult1_BOVHResult, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);

            }
        }

        /// <summary>
        /// 电芯结果判断
        /// </summary>
        public void ReturnResult()
        {
            lock (sendResult_Lock)
            {
                int endSignal = Project.Instance.cutOverlayPLCManage.OVH_ReadEndSignal();

                //电芯结束，单层电芯
                if (endSignal != 0)
                {
                    //获取电芯结果
                    GetCellResult(out short downCellResult, out short upCellResult);

                    //电芯结束，单层电芯
                    if (endSignal == 1)
                    {
                        //发送上电芯结果
                        SendUpCellResult(upCellResult);
                        Project.Instance.cutOverlayPLCManage.CommunicationLog.Info("结束电芯");
                        Log4netRecord.RunLog.Info("结束电芯");
                    }
                    //电芯结束，双层电芯
                    else if (endSignal == 2)
                    {
                        //发送下电芯结果
                        SendDownCellResult(downCellResult);

                        //发送上电芯结果
                        SendUpCellResult(upCellResult);

                        Project.Instance.cutOverlayPLCManage.CommunicationLog.Info("结束电芯");
                        Log4netRecord.RunLog.Info("结束电芯");
                    }

                }
            }
        }

        /// <summary>
        /// 工位选择判断
        /// </summary>
        private void StationSelectJudge()
        {
            if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面极片复检工位 ||
                Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面极片复检工位)
            {

            }
            else
            {
                Log4netRecord.ErrorLog.Debug("工位选择错误");
                TJBJInfo.AlarmWriteState(AlarmName.工位选择错误, StopAlarmStateEnum.停机, "");
                return;
            }
        }

        /// <summary>
        /// 初始化数据表头
        /// </summary>
        /// <param name="cameraEnum"></param>
        private void InitDataTitle(CameraEnum cameraEnum)
        {
            string[] tileName = new string[] { "时间", "帧号", "电芯包编号", "极片编号" };
            List<string> dataShowTitles = new List<string>();
            List<string> dataSaveTitles = new List<string>();
            if (true)
            {
                CamDetectionVariable camDetectionVariable = Project.Instance.cameraManager[cameraEnum].m_CamDetectionVariable;

                //生产统计初始化
                camDetectionVariable.statisticalData = new StatisticalData();


                dataShowTitles.AddRange(tileName);
                dataSaveTitles.AddRange(tileName);
                List<SizeItemInfo> sizeItemInfos = overHangCraft.ListSizeEdits[(int)cameraEnum];
                if (sizeItemInfos != null && sizeItemInfos.Count > 0)
                {
                    for (int i = 0; i < sizeItemInfos.Count; i++)
                    {
                        dataShowTitles.Add(sizeItemInfos[i].Description);

                        dataSaveTitles.Add(sizeItemInfos[i].Description);
                        dataSaveTitles.Add("结果");
                    }
                }

                //dataSaveTitles = dataShowTitles;
                camDetectionVariable.DataTitlesShow = dataShowTitles.ToArray();
                camDetectionVariable.DataTitlesSave = dataSaveTitles.ToArray();
                dataShowTitles.Clear();
                dataSaveTitles.Clear();
            }
        }


        /// <summary>
        /// 获取数据表头
        /// </summary>
        /// <param name="cameraEnum"></param>
        /// <returns></returns>
        private void GetDataTitle(CameraEnum cameraEnum, out List<string> show, out List<string> save)
        {
            List<string> strings = new List<string>();
            List<SizeItemInfo> sizeItemInfos = overHangCraft.ListSizeEdits[(int)cameraEnum];
            if (sizeItemInfos != null && sizeItemInfos.Count > 0)
            {
                for (int i = 0; i < sizeItemInfos.Count; i++)
                {
                    strings.Add(sizeItemInfos[i].Description);
                }
            }
            show = strings.ToList();
            save = strings.SelectMany(p => new string[] { p, "结果" }).ToList();
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
                cameraEnum = CameraEnum.Cam2;
                if (true)
                {
                    GetSizeCarft(cameraEnum, "AS1", out string sizeNameDescription, out double upper, out double standard, out double lower);
                    chartUIParam_AS1 = new ChartUIParam(sizeNameDescription, form.formsPlot_AS1);
                    ParamsChart paramsChart = new ParamsChart();
                    paramsChart.f_UpLimit = standard + upper;
                    paramsChart.f_DownLimit = standard + lower;
                    paramsChart.f_StandarValue = standard;
                    paramsChart.points.Add(new PointClass() { CurName = "平均值", Point = new Point(0, 0), Color = Color.Green });
                    paramsChart.points.Add(new PointClass() { CurName = "测量值", Point = new Point(0, 0), Color = Color.Blue });
                    chartUIParam_AS1.InitCurve(paramsChart);
                }

                //////////////////////////
                cameraEnum = CameraEnum.Cam3;
                if (true)
                {
                    GetSizeCarft(cameraEnum, "AS3", out string sizeNameDescription, out double upper, out double standard, out double lower);
                    chartUIParam_AS3 = new ChartUIParam(sizeNameDescription, form.formsPlot_AS3);
                    ParamsChart paramsChart = new ParamsChart();
                    paramsChart.f_UpLimit = standard + upper;
                    paramsChart.f_DownLimit = standard + lower;
                    paramsChart.f_StandarValue = standard;
                    paramsChart.points.Add(new PointClass() { CurName = "平均值", Point = new Point(0, 0), Color = Color.Green });
                    paramsChart.points.Add(new PointClass() { CurName = "测量值", Point = new Point(0, 0), Color = Color.Blue });
                    chartUIParam_AS3.InitCurve(paramsChart);
                }

                //////////////////////////
                cameraEnum = CameraEnum.Cam4;
                if (true)
                {
                    GetSizeCarft(cameraEnum, "AS2", out string sizeNameDescription, out double upper, out double standard, out double lower);
                    chartUIParam_AS2 = new ChartUIParam(sizeNameDescription, form.formsPlot_AS2);
                    ParamsChart paramsChart = new ParamsChart();
                    paramsChart.f_UpLimit = standard + upper;
                    paramsChart.f_DownLimit = standard + lower;
                    paramsChart.f_StandarValue = standard;
                    paramsChart.points.Add(new PointClass() { CurName = "平均值", Point = new Point(0, 0), Color = Color.Green });
                    paramsChart.points.Add(new PointClass() { CurName = "测量值", Point = new Point(0, 0), Color = Color.Blue });
                    chartUIParam_AS2.InitCurve(paramsChart);
                }

                //////////////////////////
                cameraEnum = CameraEnum.Cam5;
                if (true)
                {
                    GetSizeCarft(cameraEnum, "4X", out string sizeNameDescription, out double upper, out double standard, out double lower);
                    chartUIParam_4X = new ChartUIParam(sizeNameDescription, form.formsPlot_4X);
                    ParamsChart paramsChart = new ParamsChart();
                    paramsChart.f_UpLimit = standard + upper;
                    paramsChart.f_DownLimit = standard + lower;
                    paramsChart.f_StandarValue = standard;
                    paramsChart.points.Add(new PointClass() { CurName = "平均值", Point = new Point(0, 0), Color = Color.Green });
                    paramsChart.points.Add(new PointClass() { CurName = "测量值", Point = new Point(0, 0), Color = Color.Blue });
                    chartUIParam_4X.InitCurve(paramsChart);

                    //AA                  
                    chartUIParamAA_4X = new ChartUIParam(sizeNameDescription, form.formsPlotAA_4X);
                    ParamsChart paramsChart1 = new ParamsChart();
                    paramsChart1.f_UpLimit = overHangCraft.AAStandardUp;
                    paramsChart1.f_DownLimit = overHangCraft.AAStandardDown;
                    paramsChart1.f_StandarValue = 0;
                    paramsChart1.points.Add(new PointClass() { CurName = "平均值", Point = new Point(0, 0), Color = Color.Green });
                    paramsChart1.points.Add(new PointClass() { CurName = "测量值", Point = new Point(0, 0), Color = Color.Blue });
                    chartUIParamAA_4X.InitCurve(paramsChart1);

                    //CC                  
                    chartUIParamCC_4X = new ChartUIParam(sizeNameDescription, form.formsPlotCC_4X);
                    ParamsChart paramsChart2 = new ParamsChart();
                    paramsChart2.f_UpLimit = overHangCraft.AAStandardUp;
                    paramsChart2.f_DownLimit = overHangCraft.AAStandardDown;
                    paramsChart2.f_StandarValue = 0;
                    paramsChart2.points.Add(new PointClass() { CurName = "平均值", Point = new Point(0, 0), Color = Color.Green });
                    paramsChart2.points.Add(new PointClass() { CurName = "测量值", Point = new Point(0, 0), Color = Color.Blue });
                    chartUIParamCC_4X.InitCurve(paramsChart2);
                }

                if (true)
                {
                    GetSizeCarft(cameraEnum, "4Y", out string sizeNameDescription, out double upper, out double standard, out double lower);
                    chartUIParam_4Y = new ChartUIParam(sizeNameDescription, form.formsPlot_4Y);
                    ParamsChart paramsChart = new ParamsChart();
                    paramsChart.f_UpLimit = standard + upper;
                    paramsChart.f_DownLimit = standard + lower;
                    paramsChart.f_StandarValue = standard;
                    paramsChart.points.Add(new PointClass() { CurName = "平均值", Point = new Point(0, 0), Color = Color.Green });
                    paramsChart.points.Add(new PointClass() { CurName = "测量值", Point = new Point(0, 0), Color = Color.Blue });
                    chartUIParam_4Y.InitCurve(paramsChart);
                }

                //////////////////////////
                cameraEnum = CameraEnum.Cam6;
                if (true)
                {
                    GetSizeCarft(cameraEnum, "1X", out string sizeNameDescription, out double upper, out double standard, out double lower);
                    chartUIParam_1X = new ChartUIParam(sizeNameDescription, form.formsPlot_1X);
                    ParamsChart paramsChart = new ParamsChart();
                    paramsChart.f_UpLimit = standard + upper;
                    paramsChart.f_DownLimit = standard + lower;
                    paramsChart.f_StandarValue = standard;
                    paramsChart.points.Add(new PointClass() { CurName = "平均值", Point = new Point(0, 0), Color = Color.Green });
                    paramsChart.points.Add(new PointClass() { CurName = "测量值", Point = new Point(0, 0), Color = Color.Blue });
                    chartUIParam_1X.InitCurve(paramsChart);

                    //AA                  
                    chartUIParamAA_1X = new ChartUIParam(sizeNameDescription, form.formsPlotAA_1X);
                    ParamsChart paramsChart1 = new ParamsChart();
                    paramsChart1.f_UpLimit = overHangCraft.AAStandardUp;
                    paramsChart1.f_DownLimit = overHangCraft.AAStandardDown;
                    paramsChart1.f_StandarValue = 0;
                    paramsChart1.points.Add(new PointClass() { CurName = "平均值", Point = new Point(0, 0), Color = Color.Green });
                    paramsChart1.points.Add(new PointClass() { CurName = "测量值", Point = new Point(0, 0), Color = Color.Blue });
                    chartUIParamAA_1X.InitCurve(paramsChart1);

                    //CC                  
                    chartUIParamCC_1X = new ChartUIParam(sizeNameDescription, form.formsPlotCC_1X);
                    ParamsChart paramsChart2 = new ParamsChart();
                    paramsChart2.f_UpLimit = overHangCraft.AAStandardUp;
                    paramsChart2.f_DownLimit = overHangCraft.AAStandardDown;
                    paramsChart2.f_StandarValue = 0;
                    paramsChart2.points.Add(new PointClass() { CurName = "平均值", Point = new Point(0, 0), Color = Color.Green });
                    paramsChart2.points.Add(new PointClass() { CurName = "测量值", Point = new Point(0, 0), Color = Color.Blue });
                    chartUIParamCC_1X.InitCurve(paramsChart2);
                }

                if (true)
                {
                    GetSizeCarft(cameraEnum, "1Y", out string sizeNameDescription, out double upper, out double standard, out double lower);
                    chartUIParam_1Y = new ChartUIParam(sizeNameDescription, form.formsPlot_1Y);
                    ParamsChart paramsChart = new ParamsChart();
                    paramsChart.f_UpLimit = standard + upper;
                    paramsChart.f_DownLimit = standard + lower;
                    paramsChart.f_StandarValue = standard;
                    paramsChart.points.Add(new PointClass() { CurName = "平均值", Point = new Point(0, 0), Color = Color.Green });
                    paramsChart.points.Add(new PointClass() { CurName = "测量值", Point = new Point(0, 0), Color = Color.Blue });
                    chartUIParam_1Y.InitCurve(paramsChart);

                    //AA                  
                    chartUIParamAA_1Y = new ChartUIParam(sizeNameDescription, form.formsPlotAA_1Y);
                    ParamsChart paramsChart1 = new ParamsChart();
                    paramsChart1.f_UpLimit = overHangCraft.AAStandardUp;
                    paramsChart1.f_DownLimit = overHangCraft.AAStandardDown;
                    paramsChart1.f_StandarValue = 0;
                    paramsChart1.points.Add(new PointClass() { CurName = "平均值", Point = new Point(0, 0), Color = Color.Green });
                    paramsChart1.points.Add(new PointClass() { CurName = "测量值", Point = new Point(0, 0), Color = Color.Blue });
                    chartUIParamAA_1Y.InitCurve(paramsChart1);

                    //CC                  
                    chartUIParamCC_1Y = new ChartUIParam(sizeNameDescription, form.formsPlotCC_1Y);
                    ParamsChart paramsChart2 = new ParamsChart();
                    paramsChart2.f_UpLimit = overHangCraft.AAStandardUp;
                    paramsChart2.f_DownLimit = overHangCraft.AAStandardDown;
                    paramsChart2.f_StandarValue = 0;
                    paramsChart2.points.Add(new PointClass() { CurName = "平均值", Point = new Point(0, 0), Color = Color.Green });
                    paramsChart2.points.Add(new PointClass() { CurName = "测量值", Point = new Point(0, 0), Color = Color.Blue });
                    chartUIParamCC_1Y.InitCurve(paramsChart2);
                }

                //////////////////////////
                cameraEnum = CameraEnum.Cam7;
                if (true)
                {
                    GetSizeCarft(cameraEnum, "5X", out string sizeNameDescription, out double upper, out double standard, out double lower);
                    chartUIParam_5X = new ChartUIParam(sizeNameDescription, form.formsPlot_5X);
                    ParamsChart paramsChart = new ParamsChart();
                    paramsChart.f_UpLimit = standard + upper;
                    paramsChart.f_DownLimit = standard + lower;
                    paramsChart.f_StandarValue = standard;
                    paramsChart.points.Add(new PointClass() { CurName = "平均值", Point = new Point(0, 0), Color = Color.Green });
                    paramsChart.points.Add(new PointClass() { CurName = "测量值", Point = new Point(0, 0), Color = Color.Blue });
                    chartUIParam_5X.InitCurve(paramsChart);

                    //AA                  
                    chartUIParamAA_5X = new ChartUIParam(sizeNameDescription, form.formsPlotAA_5X);
                    ParamsChart paramsChart1 = new ParamsChart();
                    paramsChart1.f_UpLimit = overHangCraft.AAStandardUp;
                    paramsChart1.f_DownLimit = overHangCraft.AAStandardDown;
                    paramsChart1.f_StandarValue = 0;
                    paramsChart1.points.Add(new PointClass() { CurName = "平均值", Point = new Point(0, 0), Color = Color.Green });
                    paramsChart1.points.Add(new PointClass() { CurName = "测量值", Point = new Point(0, 0), Color = Color.Blue });
                    chartUIParamAA_5X.InitCurve(paramsChart1);

                    //CC                  
                    chartUIParamCC_5X = new ChartUIParam(sizeNameDescription, form.formsPlotCC_5X);
                    ParamsChart paramsChart2 = new ParamsChart();
                    paramsChart2.f_UpLimit = overHangCraft.AAStandardUp;
                    paramsChart2.f_DownLimit = overHangCraft.AAStandardDown;
                    paramsChart2.f_StandarValue = 0;
                    paramsChart2.points.Add(new PointClass() { CurName = "平均值", Point = new Point(0, 0), Color = Color.Green });
                    paramsChart2.points.Add(new PointClass() { CurName = "测量值", Point = new Point(0, 0), Color = Color.Blue });
                    chartUIParamCC_5X.InitCurve(paramsChart2);
                }

                if (true)
                {
                    GetSizeCarft(cameraEnum, "5Y", out string sizeNameDescription, out double upper, out double standard, out double lower);
                    chartUIParam_5Y = new ChartUIParam(sizeNameDescription, form.formsPlot_5Y);
                    ParamsChart paramsChart = new ParamsChart();
                    paramsChart.f_UpLimit = standard + upper;
                    paramsChart.f_DownLimit = standard + lower;
                    paramsChart.f_StandarValue = standard;
                    paramsChart.points.Add(new PointClass() { CurName = "平均值", Point = new Point(0, 0), Color = Color.Green });
                    paramsChart.points.Add(new PointClass() { CurName = "测量值", Point = new Point(0, 0), Color = Color.Blue });
                    chartUIParam_5Y.InitCurve(paramsChart);

                    //AA                  
                    chartUIParamAA_5Y = new ChartUIParam(sizeNameDescription, form.formsPlotAA_5Y);
                    ParamsChart paramsChart1 = new ParamsChart();
                    paramsChart1.f_UpLimit = overHangCraft.AAStandardUp;
                    paramsChart1.f_DownLimit = overHangCraft.AAStandardDown;
                    paramsChart1.f_StandarValue = 0;
                    paramsChart1.points.Add(new PointClass() { CurName = "平均值", Point = new Point(0, 0), Color = Color.Green });
                    paramsChart1.points.Add(new PointClass() { CurName = "测量值", Point = new Point(0, 0), Color = Color.Blue });
                    chartUIParamAA_5Y.InitCurve(paramsChart1);

                    //CC                  
                    chartUIParamCC_5Y = new ChartUIParam(sizeNameDescription, form.formsPlotCC_5Y);
                    ParamsChart paramsChart2 = new ParamsChart();
                    paramsChart2.f_UpLimit = overHangCraft.AAStandardUp;
                    paramsChart2.f_DownLimit = overHangCraft.AAStandardDown;
                    paramsChart2.f_StandarValue = 0;
                    paramsChart2.points.Add(new PointClass() { CurName = "平均值", Point = new Point(0, 0), Color = Color.Green });
                    paramsChart2.points.Add(new PointClass() { CurName = "测量值", Point = new Point(0, 0), Color = Color.Blue });
                    chartUIParamCC_5Y.InitCurve(paramsChart2);
                }


                //////////////////////////
                cameraEnum = CameraEnum.Cam8;
                if (true)
                {
                    GetSizeCarft(cameraEnum, "2X", out string sizeNameDescription, out double upper, out double standard, out double lower);
                    chartUIParam_2X = new ChartUIParam(sizeNameDescription, form.formsPlot_2X);
                    ParamsChart paramsChart = new ParamsChart();
                    paramsChart.f_UpLimit = standard + upper;
                    paramsChart.f_DownLimit = standard + lower;
                    paramsChart.f_StandarValue = standard;
                    paramsChart.points.Add(new PointClass() { CurName = "平均值", Point = new Point(0, 0), Color = Color.Green });
                    paramsChart.points.Add(new PointClass() { CurName = "测量值", Point = new Point(0, 0), Color = Color.Blue });
                    chartUIParam_2X.InitCurve(paramsChart);

                    //AA                  
                    chartUIParamAA_2X = new ChartUIParam(sizeNameDescription, form.formsPlotAA_2X);
                    ParamsChart paramsChart1 = new ParamsChart();
                    paramsChart1.f_UpLimit = overHangCraft.AAStandardUp;
                    paramsChart1.f_DownLimit = overHangCraft.AAStandardDown;
                    paramsChart1.f_StandarValue = 0;
                    paramsChart1.points.Add(new PointClass() { CurName = "平均值", Point = new Point(0, 0), Color = Color.Green });
                    paramsChart1.points.Add(new PointClass() { CurName = "测量值", Point = new Point(0, 0), Color = Color.Blue });
                    chartUIParamAA_2X.InitCurve(paramsChart1);

                    //CC                  
                    chartUIParamCC_2X = new ChartUIParam(sizeNameDescription, form.formsPlotCC_2X);
                    ParamsChart paramsChart2 = new ParamsChart();
                    paramsChart2.f_UpLimit = overHangCraft.AAStandardUp;
                    paramsChart2.f_DownLimit = overHangCraft.AAStandardDown;
                    paramsChart2.f_StandarValue = 0;
                    paramsChart2.points.Add(new PointClass() { CurName = "平均值", Point = new Point(0, 0), Color = Color.Green });
                    paramsChart2.points.Add(new PointClass() { CurName = "测量值", Point = new Point(0, 0), Color = Color.Blue });
                    chartUIParamCC_2X.InitCurve(paramsChart2);
                }

                if (true)
                {
                    GetSizeCarft(cameraEnum, "2Y", out string sizeNameDescription, out double upper, out double standard, out double lower);
                    chartUIParam_2Y = new ChartUIParam(sizeNameDescription, form.formsPlot_2Y);
                    ParamsChart paramsChart = new ParamsChart();
                    paramsChart.f_UpLimit = standard + upper;
                    paramsChart.f_DownLimit = standard + lower;
                    paramsChart.f_StandarValue = standard;
                    paramsChart.points.Add(new PointClass() { CurName = "平均值", Point = new Point(0, 0), Color = Color.Green });
                    paramsChart.points.Add(new PointClass() { CurName = "测量值", Point = new Point(0, 0), Color = Color.Blue });
                    chartUIParam_2Y.InitCurve(paramsChart);

                    //AA                  
                    chartUIParamAA_2Y = new ChartUIParam(sizeNameDescription, form.formsPlotAA_2Y);
                    ParamsChart paramsChart1 = new ParamsChart();
                    paramsChart1.f_UpLimit = overHangCraft.AAStandardUp;
                    paramsChart1.f_DownLimit = overHangCraft.AAStandardDown;
                    paramsChart1.f_StandarValue = 0;
                    paramsChart1.points.Add(new PointClass() { CurName = "平均值", Point = new Point(0, 0), Color = Color.Green });
                    paramsChart1.points.Add(new PointClass() { CurName = "测量值", Point = new Point(0, 0), Color = Color.Blue });
                    chartUIParamAA_2Y.InitCurve(paramsChart1);

                    //CC                  
                    chartUIParamCC_2Y = new ChartUIParam(sizeNameDescription, form.formsPlotCC_2Y);
                    ParamsChart paramsChart2 = new ParamsChart();
                    paramsChart2.f_UpLimit = overHangCraft.AAStandardUp;
                    paramsChart2.f_DownLimit = overHangCraft.AAStandardDown;
                    paramsChart2.f_StandarValue = 0;
                    paramsChart2.points.Add(new PointClass() { CurName = "平均值", Point = new Point(0, 0), Color = Color.Green });
                    paramsChart2.points.Add(new PointClass() { CurName = "测量值", Point = new Point(0, 0), Color = Color.Blue });
                    chartUIParamCC_2Y.InitCurve(paramsChart2);
                }

                if (true)
                {
                    GetSizeCarft(cameraEnum, "3X", out string sizeNameDescription, out double upper, out double standard, out double lower);
                    chartUIParam_3X = new ChartUIParam(sizeNameDescription, form.formsPlot_3X);
                    ParamsChart paramsChart = new ParamsChart();
                    paramsChart.f_UpLimit = standard + upper;
                    paramsChart.f_DownLimit = standard + lower;
                    paramsChart.f_StandarValue = standard;
                    paramsChart.points.Add(new PointClass() { CurName = "平均值", Point = new Point(0, 0), Color = Color.Green });
                    paramsChart.points.Add(new PointClass() { CurName = "测量值", Point = new Point(0, 0), Color = Color.Blue });
                    chartUIParam_3X.InitCurve(paramsChart);

                    //AA                  
                    chartUIParamAA_3X = new ChartUIParam(sizeNameDescription, form.formsPlotAA_3X);
                    ParamsChart paramsChart1 = new ParamsChart();
                    paramsChart1.f_UpLimit = overHangCraft.AAStandardUp;
                    paramsChart1.f_DownLimit = overHangCraft.AAStandardDown;
                    paramsChart1.f_StandarValue = 0;
                    paramsChart1.points.Add(new PointClass() { CurName = "平均值", Point = new Point(0, 0), Color = Color.Green });
                    paramsChart1.points.Add(new PointClass() { CurName = "测量值", Point = new Point(0, 0), Color = Color.Blue });
                    chartUIParamAA_3X.InitCurve(paramsChart1);

                    //CC                  
                    chartUIParamCC_3X = new ChartUIParam(sizeNameDescription, form.formsPlotCC_3X);
                    ParamsChart paramsChart2 = new ParamsChart();
                    paramsChart2.f_UpLimit = overHangCraft.AAStandardUp;
                    paramsChart2.f_DownLimit = overHangCraft.AAStandardDown;
                    paramsChart2.f_StandarValue = 0;
                    paramsChart2.points.Add(new PointClass() { CurName = "平均值", Point = new Point(0, 0), Color = Color.Green });
                    paramsChart2.points.Add(new PointClass() { CurName = "测量值", Point = new Point(0, 0), Color = Color.Blue });
                    chartUIParamCC_3X.InitCurve(paramsChart2);
                }

                if (true)
                {
                    GetSizeCarft(cameraEnum, "3Y", out string sizeNameDescription, out double upper, out double standard, out double lower);
                    chartUIParam_3Y = new ChartUIParam(sizeNameDescription, form.formsPlot_3Y);
                    ParamsChart paramsChart = new ParamsChart();
                    paramsChart.f_UpLimit = standard + upper;
                    paramsChart.f_DownLimit = standard + lower;
                    paramsChart.f_StandarValue = standard;
                    paramsChart.points.Add(new PointClass() { CurName = "平均值", Point = new Point(0, 0), Color = Color.Green });
                    paramsChart.points.Add(new PointClass() { CurName = "测量值", Point = new Point(0, 0), Color = Color.Blue });
                    chartUIParam_3Y.InitCurve(paramsChart);

                    //AA                  
                    chartUIParamAA_3Y = new ChartUIParam(sizeNameDescription, form.formsPlotAA_3Y);
                    ParamsChart paramsChart1 = new ParamsChart();
                    paramsChart1.f_UpLimit = overHangCraft.AAStandardUp;
                    paramsChart1.f_DownLimit = overHangCraft.AAStandardDown;
                    paramsChart1.f_StandarValue = 0;
                    paramsChart1.points.Add(new PointClass() { CurName = "平均值", Point = new Point(0, 0), Color = Color.Green });
                    paramsChart1.points.Add(new PointClass() { CurName = "测量值", Point = new Point(0, 0), Color = Color.Blue });
                    chartUIParamAA_3Y.InitCurve(paramsChart1);

                    //CC                  
                    chartUIParamCC_3Y = new ChartUIParam(sizeNameDescription, form.formsPlotCC_3Y);
                    ParamsChart paramsChart2 = new ParamsChart();
                    paramsChart2.f_UpLimit = overHangCraft.AAStandardUp;
                    paramsChart2.f_DownLimit = overHangCraft.AAStandardDown;
                    paramsChart2.f_StandarValue = 0;
                    paramsChart2.points.Add(new PointClass() { CurName = "平均值", Point = new Point(0, 0), Color = Color.Green });
                    paramsChart2.points.Add(new PointClass() { CurName = "测量值", Point = new Point(0, 0), Color = Color.Blue });
                    chartUIParamCC_3Y.InitCurve(paramsChart2);
                }

                if (true)
                {
                    GetSizeCarft(cameraEnum, "Corner1", out string sizeNameDescription, out double upper, out double standard, out double lower);
                    chartUIParam_Corner1 = new ChartUIParam(sizeNameDescription, form.formsPlot_Corner1);
                    ParamsChart paramsChart = new ParamsChart();
                    paramsChart.f_UpLimit = standard + upper;
                    paramsChart.f_DownLimit = standard + lower;
                    paramsChart.f_StandarValue = standard;
                    paramsChart.points.Add(new PointClass() { CurName = "平均值", Point = new Point(0, 0), Color = Color.Green });
                    paramsChart.points.Add(new PointClass() { CurName = "测量值", Point = new Point(0, 0), Color = Color.Blue });
                    chartUIParam_Corner1.InitCurve(paramsChart);
                }

                if (true)
                {
                    GetSizeCarft(cameraEnum, "ARC", out string sizeNameDescription, out double upper, out double standard, out double lower);
                    chartUIParam_ARC = new ChartUIParam(sizeNameDescription, form.formsPlot_ARC);
                    ParamsChart paramsChart = new ParamsChart();
                    paramsChart.f_UpLimit = standard + upper;
                    paramsChart.f_DownLimit = standard + lower;
                    paramsChart.f_StandarValue = standard;
                    paramsChart.points.Add(new PointClass() { CurName = "平均值", Point = new Point(0, 0), Color = Color.Green });
                    paramsChart.points.Add(new PointClass() { CurName = "测量值", Point = new Point(0, 0), Color = Color.Blue });
                    chartUIParam_ARC.InitCurve(paramsChart);
                }

                if (true)
                {
                    GetSizeCarft(cameraEnum, "Corner2", out string sizeNameDescription, out double upper, out double standard, out double lower);
                    chartUIParam_Corner2 = new ChartUIParam(sizeNameDescription, form.formsPlot_Corner2);
                    ParamsChart paramsChart = new ParamsChart();
                    paramsChart.f_UpLimit = standard + upper;
                    paramsChart.f_DownLimit = standard + lower;
                    paramsChart.f_StandarValue = standard;
                    paramsChart.points.Add(new PointClass() { CurName = "平均值", Point = new Point(0, 0), Color = Color.Green });
                    paramsChart.points.Add(new PointClass() { CurName = "测量值", Point = new Point(0, 0), Color = Color.Blue });
                    chartUIParam_Corner2.InitCurve(paramsChart);
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
                for (int i = 0; i < overHangCraft.ListSizeEdits[(int)cameraEnum].Count; i++)
                {
                    string sizeName = overHangCraft.ListSizeEdits[(int)cameraEnum][i].Name;
                    string sizeNameDescription = overHangCraft.ListSizeEdits[(int)cameraEnum][i].Description;

                    if (bIZ_Variable.ChartSizeDetectionItemData.ContainsKey(sizeName))
                    {
                        List<ChartPoint> measureData = bIZ_Variable.ChartSizeDetectionItemData[sizeName].lChartAllTempData.GetListAndClear().ToList();
                        if (sizeName == "1X")
                        {
                            ChartDataShow(sizeName, measureData, chartUIParam_1X);
                        }
                        else if (sizeName == "1Y")
                        {
                            ChartDataShow(sizeName, measureData, chartUIParam_1Y);
                        }
                        else if (sizeName == "2X")
                        {
                            ChartDataShow(sizeName, measureData, chartUIParam_2X);
                        }
                        else if (sizeName == "2Y")
                        {
                            ChartDataShow(sizeName, measureData, chartUIParam_2Y);
                        }
                        else if (sizeName == "3X")
                        {
                            ChartDataShow(sizeName, measureData, chartUIParam_3X);
                        }
                        else if (sizeName == "3Y")
                        {
                            ChartDataShow(sizeName, measureData, chartUIParam_3Y);
                        }
                        else if (sizeName == "4X")
                        {
                            ChartDataShow(sizeName, measureData, chartUIParam_4X);
                        }
                        else if (sizeName == "4Y")
                        {
                            ChartDataShow(sizeName, measureData, chartUIParam_4Y);
                        }
                        else if (sizeName == "5X")
                        {
                            ChartDataShow(sizeName, measureData, chartUIParam_5X);
                        }
                        else if (sizeName == "5Y")
                        {
                            ChartDataShow(sizeName, measureData, chartUIParam_5Y);
                        }
                        else if (sizeName == "AS1")
                        {
                            ChartDataShow(sizeName, measureData, chartUIParam_AS1);
                        }
                        else if (sizeName == "AS2")
                        {
                            ChartDataShow(sizeName, measureData, chartUIParam_AS2);
                        }
                        else if (sizeName == "AS3")
                        {
                            ChartDataShow(sizeName, measureData, chartUIParam_AS3);
                        }
                        else if (sizeName == "Corner1")
                        {
                            ChartDataShow(sizeName, measureData, chartUIParam_Corner1);
                        }
                        else if (sizeName == "ARC")
                        {
                            ChartDataShow(sizeName, measureData, chartUIParam_ARC);
                        }
                        else if (sizeName == "Corner2")
                        {
                            ChartDataShow(sizeName, measureData, chartUIParam_Corner2);
                        }
                    }

                    if (bIZ_Variable.ChartSizeDetectionItemData_AA.ContainsKey(sizeName))
                    {
                        List<ChartPoint> measureData = bIZ_Variable.ChartSizeDetectionItemData_AA[sizeName].lChartAllTempData.GetListAndClear().ToList();
                        if (sizeName == "1X")
                        {
                            ChartDataShow(sizeName, measureData, chartUIParamAA_1X);
                        }
                        else if (sizeName == "1Y")
                        {
                            ChartDataShow(sizeName, measureData, chartUIParamAA_1Y);
                        }
                        else if (sizeName == "2X")
                        {
                            ChartDataShow(sizeName, measureData, chartUIParamAA_2X);
                        }
                        else if (sizeName == "2Y")
                        {
                            ChartDataShow(sizeName, measureData, chartUIParamAA_2Y);
                        }
                        else if (sizeName == "3X")
                        {
                            ChartDataShow(sizeName, measureData, chartUIParamAA_3X);
                        }
                        else if (sizeName == "3Y")
                        {
                            ChartDataShow(sizeName, measureData, chartUIParamAA_3Y);
                        }
                        else if (sizeName == "4X")
                        {
                            ChartDataShow(sizeName, measureData, chartUIParamAA_4X);
                        }
                        else if (sizeName == "5X")
                        {
                            ChartDataShow(sizeName, measureData, chartUIParamAA_5X);
                        }
                        else if (sizeName == "5Y")
                        {
                            ChartDataShow(sizeName, measureData, chartUIParamAA_5Y);
                        }

                    }

                    if (bIZ_Variable.ChartSizeDetectionItemData_CC.ContainsKey(sizeName))
                    {
                        List<ChartPoint> measureData = bIZ_Variable.ChartSizeDetectionItemData_CC[sizeName].lChartAllTempData.GetListAndClear().ToList();
                        if (sizeName == "1X")
                        {
                            ChartDataShow(sizeName, measureData, chartUIParamCC_1X);
                        }
                        else if (sizeName == "1Y")
                        {
                            ChartDataShow(sizeName, measureData, chartUIParamCC_1Y);
                        }
                        else if (sizeName == "2X")
                        {
                            ChartDataShow(sizeName, measureData, chartUIParamCC_2X);
                        }
                        else if (sizeName == "2Y")
                        {
                            ChartDataShow(sizeName, measureData, chartUIParamCC_2Y);
                        }
                        else if (sizeName == "3X")
                        {
                            ChartDataShow(sizeName, measureData, chartUIParamCC_3X);
                        }
                        else if (sizeName == "3Y")
                        {
                            ChartDataShow(sizeName, measureData, chartUIParamCC_3Y);
                        }
                        else if (sizeName == "4X")
                        {
                            ChartDataShow(sizeName, measureData, chartUIParamCC_4X);
                        }
                        else if (sizeName == "5X")
                        {
                            ChartDataShow(sizeName, measureData, chartUIParamCC_5X);
                        }
                        else if (sizeName == "5Y")
                        {
                            ChartDataShow(sizeName, measureData, chartUIParamCC_5Y);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(cameraEnum.ToString(), ex);
            }
        }
        public void ChartDataShow(string sizeName, List<ChartPoint> measureData, ChartUIParam chartUIParam)
        {
            try
            {
                if (measureData == null || measureData.Count == 0) return;

                for (int i = 0; i < measureData.Count; i++)
                {
                    ParamsChart paramsChart = new ParamsChart();
                    PointClass point = new PointClass();
                    point.CurName = "平均值";
                    point.pointInfo = new PointInfo() { Col = 0, Row = measureData[i].ChartMeanData };

                    paramsChart.points.Add(point);
                    point = new PointClass();
                    point.CurName = "测量值";
                    point.pointInfo = new PointInfo() { Col = 0, Row = measureData[i].ChartDetectionData };
                    paramsChart.points.Add(point);

                    chartUIParam.UpDataCurve(paramsChart);
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(sizeName, ex);
            }

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

                int startEndSignal = Project.Instance.cutOverlayPLCManage.OVH_ReadStartSignal();
                if (startEndSignal == 1)
                {
                    //开始新的电芯
                    bIZ_Variable.batteryProduct?.Dispose();
                    bIZ_Variable = new BIZ_Variable_OverHang();
                    Project.Instance.cutOverlayPLCManage.CommunicationLog.Info("开始新的电芯");
                    //InitChart();
                    Log4netRecord.RunLog.Info("开始新的电芯");
                    Project.Instance.cutOverlayPLCManage.OVH_WriteStartSignalFeedback(startEndSignal);
                }

                //==========================
                //【1.1】处理电芯结果         
                ReturnResult();

                //==========================
                //【】心跳信号
                if (Project.Instance.Is_Run)
                {
                    SendHeartBeatA();

                    SendHeartBeatB();
                }

                //工位选择判断
                StationSelectJudge();

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
                if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面极片复检工位)
                {
                    //记录PLC纠偏量
                    Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.InOut_ChangeRollTrigger_AOVHResult, ReadLogSaveType.ReadDifferentState, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    bool IsChangeRollTrigger = Project.Instance.cutOverlayPLCManage.m_DevAddress.InOut_ChangeRollTrigger_AOVHResult.Value;
                    if (IsChangeRollTrigger)
                    {
                        bool result = Form_Main.FormMain.MainDetection.AutoHJ(reelNumber);
                        if (result)
                        {
                            Project.Instance.cutOverlayPLCManage.m_DevAddress.InOut_ChangeRollResult_AOVHResult.Value = true;
                        }
                        else
                        {
                            Project.Instance.cutOverlayPLCManage.m_DevAddress.InOut_ChangeRollResult_AOVHResult.Value = false;
                        }
                        Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.InOut_ChangeRollResult_AOVHResult, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    }
                }
                else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面极片复检工位)
                {
                    //记录PLC纠偏量
                    Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.InOut_ChangeRollTrigger_BOVHResult, ReadLogSaveType.ReadDifferentState, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    bool IsChangeRollTrigger = Project.Instance.cutOverlayPLCManage.m_DevAddress.InOut_ChangeRollTrigger_BOVHResult.Value;
                    if (IsChangeRollTrigger)
                    {
                        bool result = Form_Main.FormMain.MainDetection.AutoHJ(reelNumber);
                        if (result)
                        {
                            Project.Instance.cutOverlayPLCManage.m_DevAddress.InOut_ChangeRollResult_BOVHResult.Value = true;
                        }
                        else
                        {
                            Project.Instance.cutOverlayPLCManage.m_DevAddress.InOut_ChangeRollResult_BOVHResult.Value = false;
                        }
                        Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.InOut_ChangeRollResult_BOVHResult, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
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

            //生产统计初始化
            CameraEnum cameraEnum = CameraEnum.Cam1;
            CamDetectionVariable camDetectionVariable = Project.Instance.cameraManager[cameraEnum].m_CamDetectionVariable;
            NLabel.UpdateControls(form.lb_SumupCount_MainCam, camDetectionVariable.statisticalData.SumupCount.ToString());
            NLabel.UpdateControls(form.lb_Yield_MainCam, camDetectionVariable.statisticalData.Yield.ToString("F2"));
            NLabel.UpdateControls(form.lb_NGCount_MainCam, camDetectionVariable.statisticalData.NGCount.ToString());
            NLabel.UpdateControls(form.lb_OKCount_MainCam, camDetectionVariable.statisticalData.OKCount.ToString());

            //初始化数据显示界面
            NListView.Init(form.listView1, Project.Instance.cameraManager[CameraEnum.Cam1].m_CamDetectionVariable.DataTitlesShow);
            NListView.Init(form.listView2, Project.Instance.cameraManager[CameraEnum.Cam2].m_CamDetectionVariable.DataTitlesShow);
            NListView.Init(form.listView3, Project.Instance.cameraManager[CameraEnum.Cam3].m_CamDetectionVariable.DataTitlesShow);
            NListView.Init(form.listView4, Project.Instance.cameraManager[CameraEnum.Cam4].m_CamDetectionVariable.DataTitlesShow);
            NListView.Init(form.listView5, Project.Instance.cameraManager[CameraEnum.Cam5].m_CamDetectionVariable.DataTitlesShow);
            NListView.Init(form.listView6, Project.Instance.cameraManager[CameraEnum.Cam6].m_CamDetectionVariable.DataTitlesShow);
            NListView.Init(form.listView7, Project.Instance.cameraManager[CameraEnum.Cam7].m_CamDetectionVariable.DataTitlesShow);
            NListView.Init(form.listView8, Project.Instance.cameraManager[CameraEnum.Cam8].m_CamDetectionVariable.DataTitlesShow);
            NListView.Init(form.listView9, BIZ_Variable_OverHang.Title_TotalTable.ToArray());
            NListView.Init(form.listView10, BIZ_Variable_OverHang.Title_NGData.ToArray());

            NLabel.UpdateControls(form.LB_Process1, Project.Instance.cameraManager[CameraEnum.Cam1].m_CamParam.CameraName);
            NLabel.UpdateControls(form.LB_Process2, Project.Instance.cameraManager[CameraEnum.Cam2].m_CamParam.CameraName);
            NLabel.UpdateControls(form.LB_Process3, Project.Instance.cameraManager[CameraEnum.Cam3].m_CamParam.CameraName);
            NLabel.UpdateControls(form.LB_Process4, Project.Instance.cameraManager[CameraEnum.Cam4].m_CamParam.CameraName);
            NLabel.UpdateControls(form.LB_Process5, Project.Instance.cameraManager[CameraEnum.Cam5].m_CamParam.CameraName);
            NLabel.UpdateControls(form.LB_Process6, Project.Instance.cameraManager[CameraEnum.Cam6].m_CamParam.CameraName);
            NLabel.UpdateControls(form.LB_Process7, Project.Instance.cameraManager[CameraEnum.Cam7].m_CamParam.CameraName);
            NLabel.UpdateControls(form.LB_Process8, Project.Instance.cameraManager[CameraEnum.Cam8].m_CamParam.CameraName);

            NLabel.UpdateControls(form.lblv1, Project.Instance.cameraManager[CameraEnum.Cam1].m_CamParam.CameraName);
            NLabel.UpdateControls(form.lblv2, Project.Instance.cameraManager[CameraEnum.Cam2].m_CamParam.CameraName);
            NLabel.UpdateControls(form.lblv3, Project.Instance.cameraManager[CameraEnum.Cam3].m_CamParam.CameraName);
            NLabel.UpdateControls(form.lblv4, Project.Instance.cameraManager[CameraEnum.Cam4].m_CamParam.CameraName);
            NLabel.UpdateControls(form.lblv5, Project.Instance.cameraManager[CameraEnum.Cam5].m_CamParam.CameraName);
            NLabel.UpdateControls(form.lblv6, Project.Instance.cameraManager[CameraEnum.Cam6].m_CamParam.CameraName);
            NLabel.UpdateControls(form.lblv7, Project.Instance.cameraManager[CameraEnum.Cam7].m_CamParam.CameraName);
            NLabel.UpdateControls(form.lblv8, Project.Instance.cameraManager[CameraEnum.Cam8].m_CamParam.CameraName);

            InitChart();
        }

        public void ClearForm()
        {
            NLabel.UpdateControls(form.LB_Formula, Project.Instance.systemInfo.CurrentCraft);
            NLabel.UpdateControls(form.LB_DetectionType, Project.Instance.craftBase.materialCrafEnum.ToString());

            //生产统计初始化
            CameraEnum cameraEnum = CameraEnum.Cam1;
            CamDetectionVariable camDetectionVariable = Project.Instance.cameraManager[cameraEnum].m_CamDetectionVariable;
            NLabel.UpdateControls(form.lb_SumupCount_MainCam, camDetectionVariable.statisticalData.SumupCount.ToString());
            NLabel.UpdateControls(form.lb_Yield_MainCam, camDetectionVariable.statisticalData.Yield.ToString("F2"));
            NLabel.UpdateControls(form.lb_NGCount_MainCam, camDetectionVariable.statisticalData.NGCount.ToString());
            NLabel.UpdateControls(form.lb_OKCount_MainCam, camDetectionVariable.statisticalData.OKCount.ToString());

            NListView.DeleteListAllItems(form.listView1);
            NListView.DeleteListAllItems(form.listView2);
            NListView.DeleteListAllItems(form.listView3);
            NListView.DeleteListAllItems(form.listView4);
            NListView.DeleteListAllItems(form.listView5);
            NListView.DeleteListAllItems(form.listView6);
            NListView.DeleteListAllItems(form.listView7);
            NListView.DeleteListAllItems(form.listView8);
            NListView.DeleteListAllItems(form.listView9);
            NListView.DeleteListAllItems(form.listView10);

            InitChart();
        }

        public void UpdateForm()
        {
            try
            {
                //曲线图
                CamChartDataShow(CameraEnum.Cam1);
                CamChartDataShow(CameraEnum.Cam2);
                CamChartDataShow(CameraEnum.Cam3);
                CamChartDataShow(CameraEnum.Cam4);
                CamChartDataShow(CameraEnum.Cam5);
                CamChartDataShow(CameraEnum.Cam6);
                CamChartDataShow(CameraEnum.Cam7);
                CamChartDataShow(CameraEnum.Cam8);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }

        public void InitVariable()
        {

            bIZ_Variable.batteryProduct.Dispose();
            bIZ_Variable = new BIZ_Variable_OverHang();
            string[] tileName = new string[] { "时间", "帧号", "电芯包编号", "极片编号" };


            GetDataTitle(CameraEnum.Cam1, out List<string> show1, out List<string> save1);
            GetDataTitle(CameraEnum.Cam2, out List<string> show2, out List<string> save2);
            GetDataTitle(CameraEnum.Cam3, out List<string> show3, out List<string> save3);
            GetDataTitle(CameraEnum.Cam4, out List<string> show4, out List<string> save4);
            GetDataTitle(CameraEnum.Cam5, out List<string> show5, out List<string> save5);
            GetDataTitle(CameraEnum.Cam6, out List<string> show6, out List<string> save6);
            GetDataTitle(CameraEnum.Cam7, out List<string> show7, out List<string> save7);
            GetDataTitle(CameraEnum.Cam8, out List<string> show8, out List<string> save8);



            if (true)
            {
                CameraEnum cameraEnum = CameraEnum.Cam1;
                List<string> showTitle = new List<string>();
                List<string> saveTitle = new List<string>();

                showTitle.AddRange(tileName);
                showTitle.AddRange(show1);
                showTitle.AddRange(show5);

                saveTitle.AddRange(tileName);
                saveTitle.AddRange(save1);
                saveTitle.AddRange(save5);

                Project.Instance.cameraManager[cameraEnum].m_CamDetectionVariable.DataTitlesShow = showTitle.ToArray();
                Project.Instance.cameraManager[cameraEnum].m_CamDetectionVariable.DataTitlesSave = saveTitle.ToArray();
            }

            if (true)
            {
                CameraEnum cameraEnum = CameraEnum.Cam2;
                List<string> showTitle = new List<string>();
                List<string> saveTitle = new List<string>();

                showTitle.AddRange(tileName);
                showTitle.AddRange(show2);
                showTitle.AddRange(show6);

                saveTitle.AddRange(tileName);
                saveTitle.AddRange(save2);
                saveTitle.AddRange(save6);

                Project.Instance.cameraManager[cameraEnum].m_CamDetectionVariable.DataTitlesShow = showTitle.ToArray();
                Project.Instance.cameraManager[cameraEnum].m_CamDetectionVariable.DataTitlesSave = saveTitle.ToArray();
            }

            if (true)
            {
                CameraEnum cameraEnum = CameraEnum.Cam3;
                List<string> showTitle = new List<string>();
                List<string> saveTitle = new List<string>();

                showTitle.AddRange(tileName);
                showTitle.AddRange(show3);
                showTitle.AddRange(show7);

                saveTitle.AddRange(tileName);
                saveTitle.AddRange(save3);
                saveTitle.AddRange(save7);

                Project.Instance.cameraManager[cameraEnum].m_CamDetectionVariable.DataTitlesShow = showTitle.ToArray();
                Project.Instance.cameraManager[cameraEnum].m_CamDetectionVariable.DataTitlesSave = saveTitle.ToArray();
            }

            if (true)
            {
                CameraEnum cameraEnum = CameraEnum.Cam4;
                List<string> showTitle = new List<string>();
                List<string> saveTitle = new List<string>();

                showTitle.AddRange(tileName);
                showTitle.AddRange(show4);
                showTitle.AddRange(show8);

                saveTitle.AddRange(tileName);
                saveTitle.AddRange(save4);
                saveTitle.AddRange(save8);

                Project.Instance.cameraManager[cameraEnum].m_CamDetectionVariable.DataTitlesShow = showTitle.ToArray();
                Project.Instance.cameraManager[cameraEnum].m_CamDetectionVariable.DataTitlesSave = saveTitle.ToArray();
            }

            if (true)
            {
                CameraEnum cameraEnum = CameraEnum.Cam5;
                List<string> showTitle = new List<string>();
                List<string> saveTitle = new List<string>();

                showTitle.AddRange(tileName);
                showTitle.AddRange(show5);


                saveTitle.AddRange(tileName);
                saveTitle.AddRange(save5);

                Project.Instance.cameraManager[cameraEnum].m_CamDetectionVariable.DataTitlesShow = showTitle.ToArray();
                Project.Instance.cameraManager[cameraEnum].m_CamDetectionVariable.DataTitlesSave = saveTitle.ToArray();
            }

            if (true)
            {
                CameraEnum cameraEnum = CameraEnum.Cam6;
                List<string> showTitle = new List<string>();
                List<string> saveTitle = new List<string>();

                showTitle.AddRange(tileName);
                showTitle.AddRange(show6);


                saveTitle.AddRange(tileName);
                saveTitle.AddRange(save6);

                Project.Instance.cameraManager[cameraEnum].m_CamDetectionVariable.DataTitlesShow = showTitle.ToArray();
                Project.Instance.cameraManager[cameraEnum].m_CamDetectionVariable.DataTitlesSave = saveTitle.ToArray();
            }

            if (true)
            {
                CameraEnum cameraEnum = CameraEnum.Cam7;
                List<string> showTitle = new List<string>();
                List<string> saveTitle = new List<string>();

                showTitle.AddRange(tileName);
                showTitle.AddRange(show7);


                saveTitle.AddRange(tileName);
                saveTitle.AddRange(save7);

                Project.Instance.cameraManager[cameraEnum].m_CamDetectionVariable.DataTitlesShow = showTitle.ToArray();
                Project.Instance.cameraManager[cameraEnum].m_CamDetectionVariable.DataTitlesSave = saveTitle.ToArray();
            }

            if (true)
            {
                CameraEnum cameraEnum = CameraEnum.Cam8;
                List<string> showTitle = new List<string>();
                List<string> saveTitle = new List<string>();

                showTitle.AddRange(tileName);
                showTitle.AddRange(show8);


                saveTitle.AddRange(tileName);
                saveTitle.AddRange(save8);

                Project.Instance.cameraManager[cameraEnum].m_CamDetectionVariable.DataTitlesShow = showTitle.ToArray();
                Project.Instance.cameraManager[cameraEnum].m_CamDetectionVariable.DataTitlesSave = saveTitle.ToArray();
            }


            string[] tileName1 = new string[] { "时间", "帧号", "帧号2", "电芯包编号", "极片编号" };
            //综合数据AC表头
            if (true)
            {
                List<string> saveTitle = new List<string>();
                saveTitle.AddRange(tileName1);
                saveTitle.AddRange(save5);
                saveTitle.AddRange(save6);
                saveTitle.AddRange(save7);
                saveTitle.AddRange(save8);
                saveTitle.Add("总结果");
                saveTitle.Add("电芯结果");
                saveTitle.Add("二维码");

                BIZ_Variable_OverHang.Title_AC = saveTitle;
                BIZ_Variable_OverHang.Title_CC = saveTitle;
            }

            //综合数据CA表头
            if (true)
            {
                List<string> saveTitle = new List<string>();
                saveTitle.AddRange(tileName1);
                saveTitle.AddRange(save1);
                saveTitle.AddRange(save5);
                saveTitle.AddRange(save2);
                saveTitle.AddRange(save6);
                saveTitle.AddRange(save3);
                saveTitle.AddRange(save7);
                saveTitle.AddRange(save4);
                saveTitle.AddRange(save8);
                saveTitle.Add("总结果");
                saveTitle.Add("电芯结果");
                saveTitle.Add("二维码");

                BIZ_Variable_OverHang.Title_CA = saveTitle;
                BIZ_Variable_OverHang.Title_AA = saveTitle;
            }

            //InitDataTitleACAll();

            //InitDataTitleCAAll();


        }



        public void InitDataTitleACAll()
        {
            string[] tileName = new string[] { "时间", "帧号", "电芯包编号", "极片编号" };
            List<string> dataShowTitles = new List<string>();
            List<string> dataSaveTitles = new List<string>();
            if (true)
            {
                CamDetectionVariable camDetectionVariable = Project.Instance.cameraManager[CameraEnum.Cam1].m_CamDetectionACAllVariable;

                //生产统计初始化
                camDetectionVariable.statisticalData = new StatisticalData();


                dataShowTitles.AddRange(tileName);
                dataSaveTitles.AddRange(tileName);
                List<SizeItemInfo> sizeItemInfos = overHangCraft.ListSizeEdits[(int)4];
                if (sizeItemInfos != null && sizeItemInfos?.Count > 0)
                {
                    for (int i = 0; i < sizeItemInfos.Count; i++)
                    {
                        dataShowTitles.Add(sizeItemInfos[i].Description);

                        dataSaveTitles.Add(sizeItemInfos[i].Description);
                        dataSaveTitles.Add("结果");
                    }
                }

                sizeItemInfos = overHangCraft.ListSizeEdits[(int)5];
                if (sizeItemInfos != null && sizeItemInfos?.Count > 0)
                {
                    for (int i = 0; i < sizeItemInfos.Count; i++)
                    {
                        dataShowTitles.Add(sizeItemInfos[i].Description);

                        dataSaveTitles.Add(sizeItemInfos[i].Description);
                        dataSaveTitles.Add("结果");
                    }
                }

                sizeItemInfos = overHangCraft.ListSizeEdits[(int)6];
                if (sizeItemInfos != null && sizeItemInfos?.Count > 0)
                {
                    for (int i = 0; i < sizeItemInfos.Count; i++)
                    {
                        dataShowTitles.Add(sizeItemInfos[i].Description);

                        dataSaveTitles.Add(sizeItemInfos[i].Description);
                        dataSaveTitles.Add("结果");
                    }
                }

                sizeItemInfos = overHangCraft.ListSizeEdits[(int)7];
                if (sizeItemInfos != null && sizeItemInfos?.Count > 0)
                {
                    for (int i = 0; i < sizeItemInfos.Count; i++)
                    {
                        dataShowTitles.Add(sizeItemInfos[i].Description);

                        dataSaveTitles.Add(sizeItemInfos[i].Description);
                        dataSaveTitles.Add("结果");
                    }
                }

                dataSaveTitles = dataShowTitles;
                camDetectionVariable.DataTitlesSave = dataSaveTitles.ToArray();
                dataSaveTitles.Clear();
            }
        }
        public void InitDataTitleCAAll()
        {
            string[] tileName = new string[] { "时间", "帧号", "电芯包编号", "极片编号" };
            List<string> dataShowTitles = new List<string>();
            List<string> dataSaveTitles = new List<string>();
            if (true)
            {
                CamDetectionVariable camDetectionVariable = Project.Instance.cameraManager[CameraEnum.Cam1].m_CamDetectionCAAllVariable;

                //生产统计初始化
                camDetectionVariable.statisticalData = new StatisticalData();


                dataShowTitles.AddRange(tileName);
                dataSaveTitles.AddRange(tileName);
                List<SizeItemInfo> sizeItemInfos = overHangCraft.ListSizeEdits[(int)0];
                if (sizeItemInfos != null && sizeItemInfos?.Count > 0)
                {
                    for (int i = 0; i < sizeItemInfos.Count; i++)
                    {
                        dataShowTitles.Add(sizeItemInfos[i].Description);

                        dataSaveTitles.Add(sizeItemInfos[i].Description);
                        dataSaveTitles.Add("结果");
                    }
                }

                sizeItemInfos = overHangCraft.ListSizeEdits[(int)1];
                if (sizeItemInfos != null && sizeItemInfos?.Count > 0)
                {
                    for (int i = 0; i < sizeItemInfos.Count; i++)
                    {
                        dataShowTitles.Add(sizeItemInfos[i].Description);

                        dataSaveTitles.Add(sizeItemInfos[i].Description);
                        dataSaveTitles.Add("结果");
                    }
                }

                sizeItemInfos = overHangCraft.ListSizeEdits[(int)2];
                if (sizeItemInfos != null && sizeItemInfos?.Count > 0)
                {
                    for (int i = 0; i < sizeItemInfos.Count; i++)
                    {
                        dataShowTitles.Add(sizeItemInfos[i].Description);

                        dataSaveTitles.Add(sizeItemInfos[i].Description);
                        dataSaveTitles.Add("结果");
                    }
                }

                sizeItemInfos = overHangCraft.ListSizeEdits[(int)3];
                if (sizeItemInfos != null && sizeItemInfos?.Count > 0)
                {
                    for (int i = 0; i < sizeItemInfos.Count; i++)
                    {
                        dataShowTitles.Add(sizeItemInfos[i].Description);

                        dataSaveTitles.Add(sizeItemInfos[i].Description);
                        dataSaveTitles.Add("结果");
                    }
                }

                dataSaveTitles = dataShowTitles;
                camDetectionVariable.DataTitlesSave = dataSaveTitles.ToArray();
                dataSaveTitles.Clear();
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
                BIZ_General.Image_Process(HWC5.operation_Image, disposeImageRetainLenght, HWC5);
                BIZ_General.Image_Process(HWC6.operation_Image, disposeImageRetainLenght, HWC6);
                BIZ_General.Image_Process(HWC7.operation_Image, disposeImageRetainLenght, HWC7);
                BIZ_General.Image_Process(HWC8.operation_Image, disposeImageRetainLenght, HWC8);
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
            HWC5?.Dispose();
            HWC6?.Dispose();
            HWC7?.Dispose();
            HWC8?.Dispose();
        }

        public void SimLoadImage(bool simAutoFlag, ref bool simRunFlag)
        {
            try
            {
                if (simRunFlag)
                {
                    bIZ_Variable.OVHPackgeNumLayerSim_A.CalculatePackgeNumLayer(2, 37, 1, 1);
                    bIZ_Variable.OVHPackgeNumLayerSim_C.CalculatePackgeNumLayer(2, 37, 1, 1);
                    Parallel.ForEach(Project.Instance.baseProDevice.m_DicCamOperaParam_Sim, (item) =>
                    {
                        bool isLoadImage = false;
                        int PackgeNum = 0;
                        int CurrentLayer = 0;
                        int AffiliationStation = 0;
                        CameraEnum cameraEnum = (CameraEnum)item.Key;
                        if (cameraEnum == CameraEnum.Cam1 ||
                            cameraEnum == CameraEnum.Cam2 ||
                            cameraEnum == CameraEnum.Cam3 ||
                            cameraEnum == CameraEnum.Cam4)
                        {
                            if (bIZ_Variable.OVHPackgeNumLayerSim_A.CurrentLayer_StackTable % 2 == 1)
                            {
                                PackgeNum = bIZ_Variable.OVHPackgeNumLayerSim_A.CurrentPackageNum_StackTable;
                                CurrentLayer = bIZ_Variable.OVHPackgeNumLayerSim_A.CurrentLayer_StackTable;
                                AffiliationStation = 1;
                                isLoadImage = true;

                            }
                        }
                        else if (cameraEnum == CameraEnum.Cam5 ||
                            cameraEnum == CameraEnum.Cam6 ||
                            cameraEnum == CameraEnum.Cam7 ||
                            cameraEnum == CameraEnum.Cam8)
                        {
                            if (bIZ_Variable.OVHPackgeNumLayerSim_A.CurrentLayer_StackTable % 2 == 0)
                            {
                                PackgeNum = bIZ_Variable.OVHPackgeNumLayerSim_C.CurrentPackageNum_StackTable;
                                CurrentLayer = bIZ_Variable.OVHPackgeNumLayerSim_C.CurrentLayer_StackTable;
                                AffiliationStation = 2;
                                isLoadImage = true;
                            }
                        }

                        if (isLoadImage)
                        {
                            bool flag = SimControl.LoadImage_Sim(Project.Instance.cameraManager[(CameraEnum)item.Key].m_ImageSimManage,
                                             out ImageInfo imageInfo, $"{((CameraEnum)item.Key).ToString()}仿真加载图片");
                            if (flag)
                            {
                                imageInfo.otherInfo.PackgeNum = PackgeNum;
                                imageInfo.otherInfo.CurrentLayer = CurrentLayer;
                                imageInfo.otherInfo.AffiliationStation = AffiliationStation;
                                item.Value.QueueGrabImage.Enqueue(imageInfo);
                                (item.Value as AreaCamParam).CallBackImgDataDeal();
                                //(item.Value as AreaCamParam).semaphoreSlimCam.Release();
                                string str = item.Key.ToString() + "信号量--" + (item.Value as AreaCamParam).semaphoreSlimCam.CurrentCount.ToString() + "图像数--" + item.Value.QueueGrabImage.Count.ToString();
                                //  Console.WriteLine(str);
                            }
                        }


                    });
                    if (bIZ_Variable.OVHPackgeNumLayerSim_A.CurrentLayer_StackTable % 2 == 1)
                    {
                        bIZ_Variable.UpOrDownCellA = bIZ_Variable.OVHPackgeNumLayerSim_A.CurrentPackageNum_StackTable;
                        bIZ_Variable.CurrentLayerA = bIZ_Variable.OVHPackgeNumLayerSim_A.CurrentLayer_StackTable;
                        bIZ_Variable.PhotoSignal_A = 1;
                    }
                    else
                    {
                        bIZ_Variable.UpOrDownCellB = bIZ_Variable.OVHPackgeNumLayerSim_C.CurrentPackageNum_StackTable;
                        bIZ_Variable.CurrentLayerB = bIZ_Variable.OVHPackgeNumLayerSim_C.CurrentLayer_StackTable;
                        bIZ_Variable.PhotoSignal_B = 1;
                    }



                    if (!simAutoFlag)
                    {
                        simRunFlag = false;
                        foreach (ProcessServices item in Project.Instance.m_ProcessList)
                        {
                            bool isLoadImage = false;
                            CameraEnum cameraEnum = (CameraEnum)item.process.processInfo.ProcessID;
                            if (cameraEnum == CameraEnum.Cam1 ||
                                cameraEnum == CameraEnum.Cam2 ||
                                cameraEnum == CameraEnum.Cam3 ||
                                cameraEnum == CameraEnum.Cam4)
                            {
                                if (bIZ_Variable.OVHPackgeNumLayerSim_A.CurrentLayer_StackTable % 2 == 1)
                                {
                                    isLoadImage = true;
                                }
                            }
                            else if (cameraEnum == CameraEnum.Cam5 ||
                                cameraEnum == CameraEnum.Cam6 ||
                                cameraEnum == CameraEnum.Cam7 ||
                                cameraEnum == CameraEnum.Cam8)
                            {
                                if (bIZ_Variable.OVHPackgeNumLayerSim_A.CurrentLayer_StackTable % 2 == 0)
                                {
                                    isLoadImage = true;
                                }
                            }
                            if (isLoadImage)
                            {
                                Project.Instance.ExecuteOnce(item.process.processInfo.ProcessID);
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }

        public void StartMojuan(string mojuanType)
        {
            try
            {
                if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面极片复检工位)
                {
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NewFilmRoll_AAOVH.Value = 1;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NewFilmRoll_AAOVH, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NewFilmRoll_ABOVH.Value = 1;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NewFilmRoll_ABOVH, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }
                else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面极片复检工位)
                {
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NewFilmRoll_BAOVH.Value = 1;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NewFilmRoll_BAOVH, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NewFilmRoll_BBOVH.Value = 1;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NewFilmRoll_BBOVH, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }

        public void StopMojuan()
        {

        }

        public void StartRun()
        {
            try
            {
                if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面极片复检工位)
                {
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_RunOrPause_AAOVH.Value = 1;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_RunOrPause_AAOVH, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_RunOrPause_ABOVH.Value = 1;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_RunOrPause_ABOVH, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }
                else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面极片复检工位)
                {
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_RunOrPause_BAOVH.Value = 1;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_RunOrPause_BAOVH, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_RunOrPause_BBOVH.Value = 1;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_RunOrPause_BBOVH, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
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
                if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面极片复检工位)
                {
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_RunOrPause_AAOVH.Value = 0;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_RunOrPause_AAOVH, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_RunOrPause_ABOVH.Value = 0;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_RunOrPause_ABOVH, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }
                else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面极片复检工位)
                {
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_RunOrPause_BAOVH.Value = 0;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_RunOrPause_BAOVH, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_RunOrPause_BBOVH.Value = 0;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_RunOrPause_BBOVH, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }

        public void AlarmReset()
        {

        }

        public void SendStopAlarm(int index)
        {
            try
            {
                if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面极片复检工位)
                {
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NGHalt_AAOVH.Value = 1;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NGHalt_AAOVH, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NGAlarm_AAOVH.Value = 1;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NGAlarm_AAOVH, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                }
                else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面极片复检工位)
                {
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NGHalt_BAOVH.Value = 1;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NGHalt_BAOVH, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NGAlarm_BAOVH.Value = 1;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NGAlarm_BAOVH, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
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
                if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面极片复检工位)
                {
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NGAlarm_AAOVH.Value = 1;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NGAlarm_AAOVH, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }
                else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面极片复检工位)
                {
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NGAlarm_BAOVH.Value = 1;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_NGAlarm_BAOVH, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
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



        /// <summary>
        /// 计算尺寸
        /// </summary>
        /// <param name="sizeInfos"></param>
        public void CalculateSize(out List<SizeInfo> sizeInfos, int currentLayer, out bool sizeAllResult, bool IsAStation = true)
        {
            sizeInfos = new List<SizeInfo>();
            sizeAllResult = false;

            try
            {
                // 参数有效性检查
                if (overHangCraft?.ListEASizes?.Count == 0)
                {
                    Log4netRecord.ErrorLog.Error("尺寸工艺数据未配置");
                    return;
                }

                var sizeCraft = overHangCraft.ListEASizes;
                foreach (SizeItemInfo sizeItem in sizeCraft)
                {
                    if ((IsAStation && sizeItem.camera == CameraEnum.Cam1) || (!IsAStation && sizeItem.camera == CameraEnum.Cam5))
                    {
                        try
                        {
                            ProcessSizeItem(sizeItem, currentLayer, sizeInfos);
                        }
                        catch (Exception ex)
                        {
                            Log4netRecord.ErrorLog.Error($"{sizeItem.Description} 尺寸计算异常", ex);
                        }
                    }
                }

                sizeAllResult = !sizeInfos.Any(c => c.resultType == ResultType.NG);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error("尺寸计算全局异常", ex);
            }
        }

       
        private void ProcessSizeItem(SizeItemInfo sizeItem, int currentLayer, List<SizeInfo> results)
        {
            if (!IsValidEditorType(sizeItem.EditorType)) return;

            SizeInfo sizeInfo = CreateSizeInfo(sizeItem);

            if (!TryGetCameraInfo(sizeItem, out var camera1, out var camera2)) return;

            var line1 = GetCamLine(camera1, sizeItem.Line1, currentLayer);
            var line2 = GetCamLine(camera2, sizeItem.Line2, currentLayer);

            if (!AreLinesValid(line1, line2)) return;

            if (TryCalculateDistance(sizeItem, line1, line2, camera1, camera2, out double val))
            {
                UpdateSizeResult(sizeInfo, val, sizeItem);
            }

            results.Add(sizeInfo);
        }

        private bool IsValidEditorType(string editorType)
        {
            return editorType == EditorTypeEnum.线距离X.ToString() ||
                   editorType == EditorTypeEnum.线距离Y.ToString();
        }

        private SizeInfo CreateSizeInfo(SizeItemInfo item)
        {
            return new SizeInfo
            {
                SizeName = item.Description,
                Description = item.Description,
                DataResult = -999,
                resultType = ResultType.NG
            };
        }

        private bool TryGetCameraInfo(SizeItemInfo item, out CameraEnum camera1, out CameraEnum camera2)
        {
            try
            {
                camera1 = (CameraEnum)Enum.Parse(typeof(CameraEnum), item.camera1);
                camera2 = (CameraEnum)Enum.Parse(typeof(CameraEnum), item.camera2);
                return true;
            }
            catch
            {
                camera1 = camera2 = default;
                Log4netRecord.ErrorLog.Warn($"相机参数解析失败: {item.camera1}/{item.camera2}");
                return false;
            }
        }

        private bool AreLinesValid(LineInfo line1, LineInfo line2)
        {
            return line1 != null && line2 != null &&
                   line1.IsLineSuccessfull &&
                   line2.IsLineSuccessfull;
        }

        private bool TryCalculateDistance(SizeItemInfo item, LineInfo line1, LineInfo line2,
            CameraEnum camera1, CameraEnum camera2, out double result)
        {
            result = -999;

            if (!TryTransformLine(camera1, line1, out var newLine1)) return false;
            if (!TryTransformLine(camera2, line2, out var newLine2)) return false;

            BIZ_General.CalculateLinesPl(newLine1, newLine2, item.EditorType,
                item.StD_RealFixRito, item.StD_RealFixOffset, out result);
            return true;
        }

        private bool TryTransformLine(CameraEnum camera, LineInfo source, out LineInfo transformed)
        {
            transformed = new LineInfo();

            try
            {
                var calib = Project.Instance.cameraManager[camera].param_Checkerboard;
                return ImageAlgorithm.GetAffinePoints2d(calib.HomMat2D, source.ColBegin, source.RowBegin, out transformed.ColBegin, out transformed.RowBegin) &&
                       ImageAlgorithm.GetAffinePoints2d(calib.HomMat2D, source.ColEnd, source.RowEnd, out transformed.ColEnd, out transformed.RowEnd);
            }
            catch
            {
                return false;
            }
        }

     
        public LineInfo GetCamLine(CameraEnum camera, string lineName, int currentLayer)
        {
            switch(camera)
            {
                case CameraEnum.Cam1:
                    return GetLineFromCamera(bIZ_Variable.batteryProduct.EACornerProduct_Cam1, currentLayer, lineName);
                case CameraEnum.Cam2:
                    return GetLineFromCamera(bIZ_Variable.batteryProduct.EACornerProduct_Cam2, currentLayer, lineName);
                case CameraEnum.Cam3:
                    return GetLineFromCamera(bIZ_Variable.batteryProduct.EACornerProduct_Cam3, currentLayer, lineName); 
                case CameraEnum.Cam4:
                    return GetLineFromCamera(bIZ_Variable.batteryProduct.EACornerProduct_Cam4, currentLayer, lineName);
                case CameraEnum.Cam5:
                    return GetLineFromCamera(bIZ_Variable.batteryProduct.EACornerProduct_Cam5, currentLayer, lineName);
                case CameraEnum.Cam6:
                    return GetLineFromCamera(bIZ_Variable.batteryProduct.EACornerProduct_Cam6, currentLayer, lineName);
                case CameraEnum.Cam7:
                    return GetLineFromCamera(bIZ_Variable.batteryProduct.EACornerProduct_Cam7, currentLayer, lineName);
                case CameraEnum.Cam8:
                    return GetLineFromCamera(bIZ_Variable.batteryProduct.EACornerProduct_Cam8, currentLayer, lineName);
                default:
                    return null;
            }
               


            
        }


        private LineInfo GetLineFromCamera(List<CamEACornerProduct> cameraProduct, int layer, string lineName)
        {
            try
            {
               
                return ((List<LineInfo>)cameraProduct.Find(c => c.EAIndex == layer).m_lineShowInfos)
                    .Find(c => c.StrLineName == lineName);
            }
            catch
            {
                return null;
            }
        }
        private void UpdateSizeResult(SizeInfo sizeInfo, double value, SizeItemInfo item)
        {
            sizeInfo.DataResult = value;
            sizeInfo.StandardValue = item.StDValue;
            sizeInfo.lowerLimitValue = item.StDValue + item.StD_Down;
            sizeInfo.upperLimitValue = item.StDValue + item.StD_Up;

            sizeInfo.resultType = value.Between(sizeInfo.lowerLimitValue, sizeInfo.upperLimitValue)
             ? ResultType.OK
             : item.DB ? ResultType.NG : ResultType.Warn;
        }



        #endregion
    }
}

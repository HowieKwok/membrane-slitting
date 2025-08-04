using Basler.Pylon;
using Frame_Core;
using HalconDotNet;
using IKapBoardClassLibrary;
using LogTool;
using Mef_VisionFrame;
using NPOI.SS.Formula.Functions;
using NPOI.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mef_VisionFram
{
    public class BIZ_DataWorker_DoubleCathode : IBizWorkerBase
    {

        private BIZ_ThreadManage_DoubleCathode bIZ_ThreadManage = new BIZ_ThreadManage_DoubleCathode();
        BIZ_Variable_DoubleCathode bIZ_Variable = new BIZ_Variable_DoubleCathode();
        Form_DoubleCathode form;
        StackEACraft stackEACraft;

        public DisplayWindowClass HWC1;
        public DisplayWindowClass HWC2;
        public DisplayWindowClass HWC3;
        public DisplayWindowClass HWC4;
        public DisplayWindowClass HWC5;
        public DisplayWindowClass HWC6;
        public DisplayWindowClass HWC7;

        public DisplayWindowClass DisplayWindowClass1;
        public DisplayWindowClass DisplayWindowClass2;
        public DisplayWindowClass DisplayWindowClass3;
        public DisplayWindowClass DisplayWindowClass4;
        public DisplayWindowClass DisplayWindowClass5;
        public DisplayWindowClass DisplayWindowClass6;
        public DisplayWindowClass DisplayWindowClass7;
        public DisplayWindowClass DisplayWindowClass8;
        public DisplayWindowClass DisplayWindowClass9;

        CombinationHWindowControl CombinationHWindowControl_cam1;
        CombinationHWindowControl CombinationHWindowControl_cam2;
        CombinationHWindowControl CombinationHWindowControl_cam5;
        CombinationHWindowControl CombinationHWindowControl_cam6;
        CombinationHWindowControl CombinationHWindowControl_cam7;

        //曲线图
        Dictionary<string, ChartUIParam> chartUIParams = new Dictionary<string, ChartUIParam>();

        IntervalExecution intervalExecution = new IntervalExecution();
        public object Prod_Lock = new object();
        public BIZ_DataWorker_DoubleCathode(Form_DoubleCathode _form)
        {

            form = _form;
            UpdateCraft();

            HWC1 = new DisplayWindowClass(form.HWC1, form.TB_PositionAndGray1, form.PB_FitWin1, CameraEnum.Cam1, form.HWC1.Name);
            HWC2 = new DisplayWindowClass(form.HWC2, form.TB_PositionAndGray2, form.PB_FitWin2, CameraEnum.Cam2, form.HWC2.Name);
            HWC3 = new DisplayWindowClass(form.HWC3, form.TB_PositionAndGray3, form.PB_FitWin3, CameraEnum.Cam3, form.HWC3.Name);
            HWC4 = new DisplayWindowClass(form.HWC4, form.TB_PositionAndGray4, form.PB_FitWin4, CameraEnum.Cam4, form.HWC4.Name);
            HWC5 = new DisplayWindowClass(form.HWC5, form.TB_PositionAndGray5, form.PB_FitWin5, CameraEnum.Cam5, form.HWC5.Name);
            HWC6 = new DisplayWindowClass(form.HWC6, form.TB_PositionAndGray6, form.PB_FitWin6, CameraEnum.Cam6, form.HWC6.Name);
            HWC7 = new DisplayWindowClass(form.HWC7, form.TB_PositionAndGray7, form.PB_FitWin7, CameraEnum.Cam7, form.HWC7.Name);

            DisplayWindowClass1 = new DisplayWindowClass(form.hWindowControl1, "Cam1_MinImage1");
            DisplayWindowClass2 = new DisplayWindowClass(form.hWindowControl2, "Cam1_MinImage2");
            DisplayWindowClass3 = new DisplayWindowClass(form.hWindowControl3, "Cam2_MinImage3");
            DisplayWindowClass4 = new DisplayWindowClass(form.hWindowControl4, "Cam2_MinImage1");
            DisplayWindowClass5 = new DisplayWindowClass(form.hWindowControl5, "Cam5_MinImage2");
            DisplayWindowClass6 = new DisplayWindowClass(form.hWindowControl6, "Cam5_MinImage3");

            DisplayWindowClass7 = new DisplayWindowClass(form.hWindowControl7, "Cam6_MinImage1");
            DisplayWindowClass8 = new DisplayWindowClass(form.hWindowControl8, "Cam6_MinImage2");
            DisplayWindowClass9 = new DisplayWindowClass(form.hWindowControl9, "Cam7_MinImage3");
            CombinationHWindowControl_cam1 = new CombinationHWindowControl(CameraEnum.Cam1.ToString(), new List<DisplayWindowClass>() { DisplayWindowClass1, DisplayWindowClass2 });
            CombinationHWindowControl_cam2 = new CombinationHWindowControl(CameraEnum.Cam2.ToString(), new List<DisplayWindowClass>() { DisplayWindowClass3, DisplayWindowClass4 });
            CombinationHWindowControl_cam5 = new CombinationHWindowControl(CameraEnum.Cam5.ToString(), new List<DisplayWindowClass>() { DisplayWindowClass5, DisplayWindowClass6 });
            CombinationHWindowControl_cam6 = new CombinationHWindowControl(CameraEnum.Cam6.ToString(), new List<DisplayWindowClass>() { DisplayWindowClass7, DisplayWindowClass8 });
            CombinationHWindowControl_cam7 = new CombinationHWindowControl(CameraEnum.Cam7.ToString(), new List<DisplayWindowClass>() { DisplayWindowClass9 });

            InitVariable();
            InitForm();

            ThreadManage = bIZ_ThreadManage;

            bIZ_ThreadManage.InitThread();

            bIZ_ThreadManage.thd_ImageShowAndSave.Start(5, "ThdImageShowAndSave");
            bIZ_ThreadManage.thd_ImageShowAndSave.PerformWork -= ImageShowSave;
            bIZ_ThreadManage.thd_ImageShowAndSave.PerformWork += ImageShowSave;

            bIZ_ThreadManage.thd_Communication.Start(200, "ThdCommunication");
            bIZ_ThreadManage.thd_Communication.PerformWork -= Communication;
            bIZ_ThreadManage.thd_Communication.PerformWork += Communication;

            bIZ_ThreadManage.thd_Process.Start(50, "ThdProcess");
            bIZ_ThreadManage.thd_Process.PerformWork -= WorkerProcess;
            bIZ_ThreadManage.thd_Process.PerformWork += WorkerProcess;
        }

        public void WorkerProcess()
        {
            try
            {
                int PhotoSignal = 0;
                if (!Project.Instance.Is_simulation)
                {
                    //读取相机拍照信号
                    short readValue = 0;
                    if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A模切工位)
                    {
                        Project.Instance.cutOverlayPLCManage.ReadAndWriteZero(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_DownTabneckCamPhotoSignal_ADie, Project.Instance.cutOverlayPLCManage.CommunicationLog, out readValue);
                    }
                    else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B模切工位)
                    {
                        Project.Instance.cutOverlayPLCManage.ReadAndWriteZero(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_DownTabneckCamPhotoSignal_BDie, Project.Instance.cutOverlayPLCManage.CommunicationLog, out readValue);
                    }
                    PhotoSignal = readValue;
                }

                if (PhotoSignal > 0)
                {
                    PhotoSignal = 0;
                    //========================================================
                    //【1】开始处理数据,在这里处理数据
                    ProcessExecutionResult processExecutionResult = IntervalExecution.DelayExecute(DateTime.Now, Project.Instance.cutOverlayPLCManage.m_DevParam.TimeOutTime, GetProdResult);

                    //【2】处理结果
                    short result = 0;
                    if (processExecutionResult == ProcessExecutionResult.Success)
                    {
                        if (bIZ_Variable.Cam6_CurrentResult && bIZ_Variable.Cam7_CurrentResult)
                        {
                            result = 11;
                        }
                        else if (bIZ_Variable.Cam6_CurrentResult && !bIZ_Variable.Cam7_CurrentResult)
                        {
                            result = 12;
                        }
                        else if (!bIZ_Variable.Cam6_CurrentResult && bIZ_Variable.Cam7_CurrentResult)
                        {
                            result = 21;
                        }
                        else if (!bIZ_Variable.Cam6_CurrentResult && !bIZ_Variable.Cam7_CurrentResult)
                        {
                            result = 22;
                        }
                    }
                    else if (processExecutionResult == ProcessExecutionResult.Overtime)
                    {

                    }
                    else if (processExecutionResult == ProcessExecutionResult.Exception)
                    {

                    }

                    //================================================================
                    //【】发送数据到PLC          
                    if (!Project.Instance.Is_simulation && result > 0)
                    {
                        if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A模切工位)
                        {
                            if (true/*bIZ_Variable.Cam6_CurrentResult*/)
                            {
                                Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_DownTabneckCamReturnTabneckValue1_ADie.Value = (float)bIZ_Variable.Cam6_TabneckValue;
                                Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_DownTabneckCamReturnTabneckValue1_ADie, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                            }
                            if (true/*bIZ_Variable.Cam7_CurrentResult*/)
                            {
                                Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_DownTabneckCamReturnTabneckValue2_ADie.Value = (float)bIZ_Variable.Cam7_TabneckValue;
                                Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_DownTabneckCamReturnTabneckValue2_ADie, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                            }



                            Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_DownTabneckCamReturnResult_ADie.Value = result;
                            Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_DownTabneckCamReturnResult_ADie, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                        }
                        else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B模切工位)
                        {
                            if (true/*bIZ_Variable.Cam6_CurrentResult*/)
                            {
                                Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_DownTabneckCamReturnTabneckValue1_BDie.Value = (float)bIZ_Variable.Cam6_TabneckValue;
                                Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_DownTabneckCamReturnTabneckValue1_BDie, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                            }
                            if (true/*bIZ_Variable.Cam7_CurrentResult*/)
                            {
                                Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_DownTabneckCamReturnTabneckValue2_BDie.Value = (float)bIZ_Variable.Cam7_TabneckValue;
                                Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_DownTabneckCamReturnTabneckValue2_BDie, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                            }

                            Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_DownTabneckCamReturnResult_BDie.Value = result;
                            Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_DownTabneckCamReturnResult_BDie, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                        }
                    }


                    //复位
                    bIZ_Variable.Cam6_Finish = false;
                    bIZ_Variable.Cam7_Finish = false;
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }

        public void ProcessingData_Cam1(ProductBase camEAProduct1)
        {
            CameraEnum cameraEnum = CameraEnum.Cam1;

            try
            {
                HOperatorSet.CountSeconds(out camEAProduct1.Image_StartDataProcessTime);

                DieCutSliceProduct dieCutSliceProduct = camEAProduct1 as DieCutSliceProduct;
                CamDetectionVariable camDetectionVariable = Project.Instance.cameraManager[cameraEnum].m_CamDetectionVariable;
                //============================================================================
                //【1】工艺处理             
                bool resultTemp = false;
                ResultType AllResultType = ResultType.NG;
                ResultType SizeAllResultType = ResultType.NG;
                ResultType DefectResultType = ResultType.NG;

                //============================================================================
                //【1.1】尺寸判断
                //SizeCraftProcessing(cameraEnum, stackEACraft.ListSizeEdits[(int)cameraEnum], dieCutSliceProduct.sizeInfos, ref bIZ_Variable.sizeDetectionItemData_Cam1, out int sizeResult, out List<RowData> sizeRowDatas);
                bool findLineFail = false;
                foreach (var item in dieCutSliceProduct.showHalconWindow.m_lineShowInfos)
                {
                    if (!item.IsLineSuccessfull)
                    {
                        findLineFail = true;
                        break;
                    }
                }

                //============================================================================
                //【1.2】缺陷判断
                BIZ_General.DefectCraftProcessing_DieCutFolding(cameraEnum, stackEACraft.ListDefectInfos[(int)cameraEnum], dieCutSliceProduct.NgTypeInfos, ref camDetectionVariable.defectDetectionItemData,
                    ref DisplayWindowClass1, out bool isJD, out bool isDefect);
                BIZ_General.AddMessageShowWindow(dieCutSliceProduct.NgTypeInfos, dieCutSliceProduct.showHalconWindow);
                //============================================================================
                //【1.3】总结果判断            
                int result = 1;
                string resultStr = "";
                if (isJD)
                {
                    result = 3;
                    resultStr = "接带";
                    resultTemp = false;
                }
                else if (isDefect || findLineFail)
                {
                    result = 2;
                    resultStr = "瑕疵";
                    resultTemp = false;
                }
                else
                {
                    result = 1;
                    resultStr = "OK";
                    resultTemp = true;
                }

                //============================================================================
                //【1.2】数据显示、保存            
                List<RowData> showRowDataList = new List<RowData>();
                List<RowData> saveRowDataList = new List<RowData>();
                RowData timeRowData = new RowData() { dataStr = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff"), resultType = ResultType.OK };
                RowData frameRowData = new RowData() { dataStr = dieCutSliceProduct.nCurFrameImgId.ToString(), resultType = ResultType.OK };
                RowData resultData = new RowData() { dataStr = resultStr, resultType = result == 1 ? ResultType.OK : ResultType.NG };
                showRowDataList.Add(timeRowData);
                showRowDataList.Add(frameRowData);
                showRowDataList.Add(resultData);
                saveRowDataList = showRowDataList;

                NListView2.Add(form.listView1, showRowDataList.ToArray(), 10, ListViewShowType.RemoveAtStartRow, true);
                FileOperationCSV.WriteData_CSV(camDetectionVariable.ProdDataPath, camDetectionVariable.DataTitlesSave, saveRowDataList.ToArray(), $"{cameraEnum}Prod");

                //生产统计
                StatisticalData statisticalData = camDetectionVariable.statisticalData;
                if (!resultTemp) statisticalData.NGCount++;
                BIZ_General.ProdStatisticsCalculate(statisticalData, dieCutSliceProduct.nCurFrameImgId, statisticalData.NGCount);
                NLabel.UpdateControls(form.lb_SumupCount_ADefect, statisticalData.SumupCount.ToString(), true);
                NLabel.UpdateControls(form.lb_OKCount_ADefect, statisticalData.OKCount.ToString(), true);
                NLabel.UpdateControls(form.lb_NGCount_ADefect, statisticalData.NGCount.ToString(), true);
                NLabel.UpdateControls(form.lb_Yield_ADefect, statisticalData.Yield.ToString("F2") + "%", true);
                //============================================================================
                //【2】图像显示
                PrimitiveTransition(cameraEnum, dieCutSliceProduct.showHalconWindow, resultTemp, dieCutSliceProduct.nCurFrameImgId, out Operation_ImageInfo operation_ImageInfo);
                HWC1.operation_Image.imageInfosShow.Enqueue(operation_ImageInfo);

                HOperatorSet.CountSeconds(out camEAProduct1.Image_DataProcessTime);
                HOperatorSet.CountSeconds(out camEAProduct1.Image_EndTime);
                //计算时间
                BIZ_General.CalculateProcessingTime(camEAProduct1);

                //发送结果到PLC
                if (!Project.Instance.Is_simulation)
                {
                    if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A模切工位)
                    {
                        Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_UpLineCamReturnResult_ADie.Value = (short)result;
                        Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_UpLineCamReturnResult_ADie, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    }
                    else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B模切工位)
                    {
                        Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_UpLineCamReturnResult_BDie.Value = (short)result;
                        Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_UpLineCamReturnResult_BDie, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    }
                }

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

                DieCutSliceProduct dieCutSliceProduct = camEAProduct1 as DieCutSliceProduct;
                CamDetectionVariable camDetectionVariable = Project.Instance.cameraManager[cameraEnum].m_CamDetectionVariable;
                //============================================================================
                //【1】工艺处理             
                bool resultTemp = false;
                ResultType AllResultType = ResultType.NG;
                ResultType SizeAllResultType = ResultType.NG;
                ResultType DefectResultType = ResultType.NG;

                //============================================================================
                //【1.1】尺寸判断
                //SizeCraftProcessing(cameraEnum, stackEACraft.ListSizeEdits[(int)cameraEnum], dieCutSliceProduct.sizeInfos, ref bIZ_Variable.sizeDetectionItemData_Cam2, out int sizeResult, out List<RowData> sizeRowDatas);
                bool findLineFail = false;
                foreach (var item in dieCutSliceProduct.showHalconWindow.m_lineShowInfos)
                {
                    if (!item.IsLineSuccessfull)
                    {
                        findLineFail = true;
                        break;
                    }
                }

                //============================================================================
                //【1.2】缺陷判断
                BIZ_General.DefectCraftProcessing_DieCutFolding(cameraEnum, stackEACraft.ListDefectInfos[(int)cameraEnum], dieCutSliceProduct.NgTypeInfos, ref camDetectionVariable.defectDetectionItemData,
                       ref DisplayWindowClass3, out bool isJD, out bool isDefect);
                BIZ_General.AddMessageShowWindow(dieCutSliceProduct.NgTypeInfos, dieCutSliceProduct.showHalconWindow);
                //============================================================================
                //【1.3】总结果判断
                int result = 1;
                string resultStr = "";
                if (isJD)
                {
                    result = 3;
                    resultStr = "接带";
                    resultTemp = false;
                }
                else if (isDefect || findLineFail)
                {
                    result = 2;
                    resultStr = "瑕疵";
                    resultTemp = false;
                }
                else
                {
                    result = 1;
                    resultStr = "OK";
                    resultTemp = true;
                }

                //============================================================================
                //【1.2】数据显示、保存            
                List<RowData> showRowDataList = new List<RowData>();
                List<RowData> saveRowDataList = new List<RowData>();
                RowData timeRowData = new RowData() { dataStr = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff"), resultType = ResultType.OK };
                RowData frameRowData = new RowData() { dataStr = dieCutSliceProduct.nCurFrameImgId.ToString(), resultType = ResultType.OK };
                RowData resultData = new RowData() { dataStr = resultStr, resultType = result == 1 ? ResultType.OK : ResultType.NG };
                showRowDataList.Add(timeRowData);
                showRowDataList.Add(frameRowData);
                showRowDataList.Add(resultData);
                saveRowDataList = showRowDataList;

                NListView.Add(form.listView2, showRowDataList.ToArray(), true);
                FileOperationCSV.WriteData_CSV(camDetectionVariable.ProdDataPath, camDetectionVariable.DataTitlesSave, saveRowDataList.ToArray(), $"{cameraEnum}Prod");

                //生产统计
                StatisticalData statisticalData = camDetectionVariable.statisticalData;
                if (!resultTemp) statisticalData.NGCount++;
                BIZ_General.ProdStatisticsCalculate(statisticalData, dieCutSliceProduct.nCurFrameImgId, statisticalData.NGCount);
                NLabel.UpdateControls(form.lb_SumupCount_BDefect, statisticalData.SumupCount.ToString(), true);
                NLabel.UpdateControls(form.lb_OKCount_BDefect, statisticalData.OKCount.ToString(), true);
                NLabel.UpdateControls(form.lb_NGCount_BDefect, statisticalData.NGCount.ToString(), true);
                NLabel.UpdateControls(form.lb_Yield_BDefect, statisticalData.Yield.ToString("F2") + "%", true);
                //============================================================================
                //【2】图像显示         
                PrimitiveTransition(cameraEnum, dieCutSliceProduct.showHalconWindow, resultTemp, dieCutSliceProduct.nCurFrameImgId, out Operation_ImageInfo operation_ImageInfo);
                HWC2.operation_Image.imageInfosShow.Enqueue(operation_ImageInfo);

                HOperatorSet.CountSeconds(out camEAProduct1.Image_DataProcessTime);
                HOperatorSet.CountSeconds(out camEAProduct1.Image_EndTime);
                //计算时间
                BIZ_General.CalculateProcessingTime(camEAProduct1);

                //发送结果到PLC
                if (!Project.Instance.Is_simulation)
                {
                    if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A模切工位)
                    {
                        Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_DownLineCamReturnResult_ADie.Value = (short)result;
                        Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_DownLineCamReturnResult_ADie, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    }
                    else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B模切工位)
                    {
                        Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_DownLineCamReturnResult_BDie.Value = (short)result;
                        Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_DownLineCamReturnResult_BDie, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    }
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

                CamEAStackProduct camEAStackProduct = camEAProduct1 as CamEAStackProduct;
                CamDetectionVariable camDetectionVariable = Project.Instance.cameraManager[cameraEnum].m_CamDetectionVariable;
                //==============================================
                //【1】数据处理
                bool resultTemp = false;
                ResultType AllResultType = ResultType.NG;
                ResultType SizeAllResultType = ResultType.NG;
                ResultType DefectResultType = ResultType.NG;
                //============================================================================
                //【1.1】尺寸判断
                if (stackEACraft.UnwindFlag)
                {
                    camEAStackProduct.sizeInfos[0].DataResult = -camEAStackProduct.sizeInfos[0].DataResult;
                }
                camEAStackProduct.sizeInfos[0].DataResult += stackEACraft.UnwindOffsetY;

                BIZ_General.SizeOffsetCaftJudgment(cameraEnum, stackEACraft.ListSizeEdits[(int)cameraEnum], camEAStackProduct.sizeInfos, out SizeAllResultType);
                BIZ_General.AddMessageShowWindow(camEAStackProduct.sizeInfos, camEAStackProduct.showHalconWindow);
                //============================================================================
                //【1.2】总结果判断
                AllResultType = SizeAllResultType;
                resultTemp = AllResultType != ResultType.NG;

                //添加曲线图数据               
                foreach (var item in camEAStackProduct.sizeInfos)
                {
                    string chartName = Project.Instance.cameraManager[cameraEnum].m_CamParam.CameraName + "_" + item.Description;
                    BIZ_General.DictionaryKeySafety(chartName, ref camDetectionVariable.sizeDetectionItemData);
                    camDetectionVariable.sizeDetectionItemData[chartName].lChartAllData.Add(new ChartPoint() { ChartDetectionData = item.DataResult }, 100);

                    ChartPoint chartTempPoint = new ChartPoint() { ChartDetectionData = item.DataResult };
                    camDetectionVariable.sizeDetectionItemData[chartName].lChartAllTempData.Add(chartTempPoint, 100);
                }

                //==============================================
                //【2】数据显示、保存  
                List<RowData> showRowDataList = new List<RowData>();
                List<RowData> saveRowDataList = new List<RowData>();
                RowData timeRowData = new RowData() { dataStr = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff"), resultType = ResultType.OK };
                RowData frameRowData = new RowData() { dataStr = camEAStackProduct.showHalconWindow.m_nImageFrameID.ToString(), resultType = AllResultType };
                showRowDataList.Add(timeRowData);
                showRowDataList.Add(frameRowData);
                for (int i = 0; i < camEAStackProduct.sizeInfos.Count; i++)
                {
                    showRowDataList.Add(new RowData() { dataStr = camEAStackProduct.sizeInfos[i].resultType.ToString(), resultType = camEAStackProduct.sizeInfos[i].resultType });
                    showRowDataList.Add(new RowData() { dataStr = camEAStackProduct.sizeInfos[i].DataResult.ToString("F3"), resultType = ResultType.OK });
                }

                saveRowDataList = showRowDataList;
                NListView.Add(form.listView3, showRowDataList.ToArray(), true);

                FileOperationCSV.WriteData_CSV(camDetectionVariable.ProdDataPath, camDetectionVariable.DataTitlesSave, saveRowDataList.ToArray(), $"{cameraEnum}");

                //==============================================
                //【】图像显示、保存        
                PrimitiveTransition(cameraEnum, camEAStackProduct.showHalconWindow, resultTemp, camEAStackProduct.showHalconWindow.m_nImageFrameID, out Operation_ImageInfo operation_ImageInfo);
                HWC3.operation_Image.imageInfosShow.Enqueue(operation_ImageInfo);

                HOperatorSet.CountSeconds(out camEAProduct1.Image_DataProcessTime);
                HOperatorSet.CountSeconds(out camEAProduct1.Image_EndTime);
                //计算时间
                BIZ_General.CalculateProcessingTime(camEAProduct1);

                //==============================================
                //【】发送结果到PLC
                if (!Project.Instance.Is_simulation)
                {
                    if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A模切工位)
                    {
                        if (resultTemp)
                        {
                            Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_UnreelingCorrectReturnOffsetValue_ADie.Value = (float)camEAStackProduct.sizeInfos[0].DataResult;
                            Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_UnreelingCorrectReturnOffsetValue_ADie, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                        }


                        Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_UnreelingCorrectReturnResult_ADie.Value = (short)(resultTemp ? (short)1 : (short)2);
                        Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_UnreelingCorrectReturnResult_ADie, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    }
                    else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B模切工位)
                    {
                        if (resultTemp)
                        {
                            Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_UnreelingCorrectReturnOffsetValue_BDie.Value = (float)camEAStackProduct.sizeInfos[0].DataResult;
                            Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_UnreelingCorrectReturnOffsetValue_BDie, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                        }

                        Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_UnreelingCorrectReturnResult_BDie.Value = (short)(resultTemp ? (short)1 : (short)2);
                        Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_UnreelingCorrectReturnResult_BDie, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    }
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

                CamEAStackProduct camEAStackProduct = camEAProduct1 as CamEAStackProduct;
                CamDetectionVariable camDetectionVariable = Project.Instance.cameraManager[cameraEnum].m_CamDetectionVariable;
                //==============================================
                //【1】数据处理
                bool resultTemp = false;
                ResultType AllResultType = ResultType.NG;
                ResultType SizeAllResultType = ResultType.NG;
                ResultType DefectResultType = ResultType.NG;
                //============================================================================
                //【1.1】尺寸判断
                if (stackEACraft.DieCutFlagY)
                {
                    camEAStackProduct.sizeInfos[0].DataResult = -camEAStackProduct.sizeInfos[0].DataResult;
                }
                camEAStackProduct.sizeInfos[0].DataResult += stackEACraft.DieCutOffsetY;
                //if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A模切工位)
                //{

                //    if (stackEACraft.DieCutFlagX)
                //    {
                //        camEAStackProduct.sizeInfos[1].DataResult = -camEAStackProduct.sizeInfos[1].DataResult;
                //    }
                //    camEAStackProduct.sizeInfos[1].DataResult += stackEACraft.DieCutOffsetX;
                //}

                if (stackEACraft.DieCutFlagY_R)
                {
                    if (camEAStackProduct.sizeInfos.Count > 1)
                    {
                        camEAStackProduct.sizeInfos[1].DataResult = -camEAStackProduct.sizeInfos[1].DataResult;
                    }
                }

                BIZ_General.SizeOffsetCaftJudgment(cameraEnum, stackEACraft.ListSizeEdits[(int)cameraEnum], camEAStackProduct.sizeInfos, out SizeAllResultType);
                BIZ_General.AddMessageShowWindow(camEAStackProduct.sizeInfos, camEAStackProduct.showHalconWindow);
                //============================================================================
                //【1.2】总结果判断
                AllResultType = SizeAllResultType;
                resultTemp = AllResultType != ResultType.NG;
                bool YsizeResult = camEAStackProduct.sizeInfos[0].resultType != ResultType.NG;
                bool XsizeResult = true;//阴极只测Y，X默认发OK

                //添加曲线图数据               
                foreach (var item in camEAStackProduct.sizeInfos)
                {
                    string chartName = Project.Instance.cameraManager[cameraEnum].m_CamParam.CameraName + "_" + item.Description;
                    BIZ_General.DictionaryKeySafety(chartName, ref camDetectionVariable.sizeDetectionItemData);
                    camDetectionVariable.sizeDetectionItemData[chartName].lChartAllData.Add(new ChartPoint() { ChartDetectionData = item.DataResult }, 100);

                    ChartPoint chartTempPoint = new ChartPoint() { ChartDetectionData = item.DataResult };
                    camDetectionVariable.sizeDetectionItemData[chartName].lChartAllTempData.Add(chartTempPoint, 100);
                }

                //==============================================
                //【2】数据显示、保存              
                List<RowData> showRowDataList = new List<RowData>();
                List<RowData> saveRowDataList = new List<RowData>();
                RowData timeRowData = new RowData() { dataStr = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff"), resultType = ResultType.OK };
                RowData frameRowData = new RowData() { dataStr = camEAStackProduct.showHalconWindow.m_nImageFrameID.ToString(), resultType = AllResultType };
                showRowDataList.Add(timeRowData);
                showRowDataList.Add(frameRowData);
                for (int i = 0; i < camEAStackProduct.sizeInfos.Count; i++)
                {
                    showRowDataList.Add(new RowData() { dataStr = camEAStackProduct.sizeInfos[i].resultType.ToString(), resultType = camEAStackProduct.sizeInfos[i].resultType });
                    showRowDataList.Add(new RowData() { dataStr = camEAStackProduct.sizeInfos[i].DataResult.ToString("F3"), resultType = ResultType.OK });
                }

                saveRowDataList = showRowDataList;
                NListView.Add(form.listView4, showRowDataList.ToArray(), true);

                FileOperationCSV.WriteData_CSV(camDetectionVariable.ProdDataPath, camDetectionVariable.DataTitlesSave, saveRowDataList.ToArray(), $"{cameraEnum}");

                //==============================================
                //【】图像显示、保存        
                PrimitiveTransition(cameraEnum, camEAStackProduct.showHalconWindow, resultTemp, camEAStackProduct.showHalconWindow.m_nImageFrameID, out Operation_ImageInfo operation_ImageInfo);
                HWC4.operation_Image.imageInfosShow.Enqueue(operation_ImageInfo);

                HOperatorSet.CountSeconds(out camEAProduct1.Image_DataProcessTime);
                HOperatorSet.CountSeconds(out camEAProduct1.Image_EndTime);
                //计算时间
                BIZ_General.CalculateProcessingTime(camEAProduct1);

                //==============================================
                //【】发送结果到PLC
                if (!Project.Instance.Is_simulation)
                {
                    if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A模切工位)
                    {
                        if (XsizeResult)
                        {
                            Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_DieCutCorrectReturnOffsetValueX_ADie.Value = (float)camEAStackProduct.sizeInfos[1].DataResult;
                            Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_DieCutCorrectReturnOffsetValueX_ADie, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);


                        }

                        if (YsizeResult)
                        {
                            Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_DieCutCorrectReturnOffsetValueY_ADie.Value = (float)camEAStackProduct.sizeInfos[0].DataResult;
                            Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_DieCutCorrectReturnOffsetValueY_ADie, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                        }


                        Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_DieCutCorrectReturnResult_ADie.Value = (short)(YsizeResult ? (short)1 : (short)2);
                        Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_DieCutCorrectReturnResult_ADie, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                        Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_DieCutCorrectReturnResult2_ADie.Value = (short)(XsizeResult ? (short)1 : (short)2);
                        Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_DieCutCorrectReturnResult2_ADie, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    }
                    else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B模切工位)
                    {
                        if (YsizeResult)
                        {
                            //Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_DieCutCorrectReturnOffsetValueX_BDie.Value = (float)camEAStackProduct.sizeInfos[1].DataResult;
                            //Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_DieCutCorrectReturnOffsetValueX_BDie, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                            Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_DieCutCorrectReturnOffsetValueY_BDie.Value = (float)camEAStackProduct.sizeInfos[0].DataResult;
                            Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_DieCutCorrectReturnOffsetValueY_BDie, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                        }


                        Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_DieCutCorrectReturnResult_BDie.Value = (short)(YsizeResult ? (short)1 : (short)2);
                        Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_DieCutCorrectReturnResult_BDie, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                        Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_DieCutCorrectReturnResult2_BDie.Value = (short)(XsizeResult ? (short)1 : (short)2);
                        Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_DieCutCorrectReturnResult2_BDie, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                        if (camEAStackProduct.sizeInfos.Count > 1)
                        {
                            Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_DieCutCorrectReturnOffsetValueYR_BDie.Value = (float)camEAStackProduct.sizeInfos[1].DataResult;
                            Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_DieCutCorrectReturnOffsetValueYR_BDie, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }

        public void ProcessingData_Cam5(ProductBase camEAProduct1)
        {
            CameraEnum cameraEnum = CameraEnum.Cam5;

            try
            {
                HOperatorSet.CountSeconds(out camEAProduct1.Image_StartDataProcessTime);

                CamEAStackProduct camEAStackProduct = camEAProduct1 as CamEAStackProduct;
                CamDetectionVariable camDetectionVariable = Project.Instance.cameraManager[cameraEnum].m_CamDetectionVariable;
                //============================================================================
                //【1】工艺处理
                bool resultTemp = false;
                ResultType AllResultType = ResultType.NG;
                ResultType SizeAllResultType = ResultType.NG;
                ResultType DefectResultType = ResultType.NG;

                //============================================================================
                //【1.1】尺寸判断
                BIZ_General.SizeCraftJudgment_DieCutFolding(cameraEnum, stackEACraft.ListSizeEdits[(int)cameraEnum], camEAStackProduct.sizeInfos, ref camDetectionVariable.sizeDetectionItemData, out SizeAllResultType);
                BIZ_General.AddMessageShowWindow(camEAStackProduct.sizeInfos, camEAStackProduct.showHalconWindow);
                //============================================================================
                //【1.2】缺陷判断
                BIZ_General.DefectCraftProcessing_DieCutFolding(cameraEnum, stackEACraft.ListDefectInfos[(int)cameraEnum], camEAStackProduct.NgTypeInfos, ref camDetectionVariable.defectDetectionItemData,
                    ref DisplayWindowClass5, out bool isJD, out bool isDefect);
                BIZ_General.AddMessageShowWindow(camEAStackProduct.NgTypeInfos, camEAStackProduct.showHalconWindow);
                //============================================================================
                //【1.3】总结果判断
                if (isJD || isDefect)
                {
                    DefectResultType = ResultType.NG;
                }
                else
                {
                    DefectResultType = ResultType.OK;
                }

                if (SizeAllResultType == ResultType.NG ||
                    DefectResultType == ResultType.NG)
                {
                    AllResultType = ResultType.NG;
                }
                else
                {
                    AllResultType = ResultType.OK;
                }

                resultTemp = AllResultType != ResultType.NG;

                //添加曲线图数据               
                foreach (var item in camEAStackProduct.sizeInfos)
                {
                    string chartName = Project.Instance.cameraManager[cameraEnum].m_CamParam.CameraName + "_" + item.Description;
                    BIZ_General.DictionaryKeySafety(chartName, ref camDetectionVariable.sizeDetectionItemData);
                    camDetectionVariable.sizeDetectionItemData[chartName].lChartAllData.Add(new ChartPoint() { ChartDetectionData = item.DataResult }, 100);

                    ChartPoint chartTempPoint = new ChartPoint() { ChartDetectionData = item.DataResult };
                    camDetectionVariable.sizeDetectionItemData[chartName].lChartAllTempData.Add(chartTempPoint, 100);
                }

                //============================================================================
                //【1.2】数据显示、保存            
                List<RowData> showRowDataList = new List<RowData>();
                List<RowData> saveRowDataList = new List<RowData>();
                RowData timeRowData = new RowData() { dataStr = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff"), resultType = ResultType.OK };
                RowData frameRowData = new RowData() { dataStr = camEAStackProduct.showHalconWindow.m_nImageFrameID.ToString(), resultType = AllResultType };
                showRowDataList.Add(timeRowData);
                showRowDataList.Add(frameRowData);
                for (int i = 0; i < camEAStackProduct.sizeInfos.Count; i++)
                {
                    showRowDataList.Add(new RowData() { dataStr = camEAStackProduct.sizeInfos[i].DataResult.ToString("F3"), resultType = camEAStackProduct.sizeInfos[i].resultType });
                }
                showRowDataList.Add(new RowData() { dataStr = DefectResultType.ToString(), resultType = DefectResultType });


                saveRowDataList.Add(timeRowData);
                saveRowDataList.Add(frameRowData);
                for (int i = 0; i < camEAStackProduct.sizeInfos.Count; i++)
                {
                    saveRowDataList.Add(new RowData() { dataStr = camEAStackProduct.sizeInfos[i].DataResult.ToString("F3"), resultType = ResultType.OK });
                    saveRowDataList.Add(new RowData() { dataStr = camEAStackProduct.sizeInfos[i].resultType.ToString(), resultType = camEAStackProduct.sizeInfos[i].resultType });
                }
                saveRowDataList.Add(new RowData() { dataStr = DefectResultType.ToString(), resultType = DefectResultType });

                NListView.Add(form.listView5, showRowDataList.ToArray(), true);
                FileOperationCSV.WriteData_CSV(camDetectionVariable.ProdDataPath, camDetectionVariable.DataTitlesSave, saveRowDataList.ToArray(), $"{cameraEnum}");

                //生产统计
                StatisticalData statisticalData = camDetectionVariable.statisticalData;
                if (!resultTemp) statisticalData.NGCount++;
                BIZ_General.ProdStatisticsCalculate(statisticalData, camEAStackProduct.showHalconWindow.m_nImageFrameID, statisticalData.NGCount);
                NLabel.UpdateControls(form.lb_SumupCount_AArea, statisticalData.SumupCount.ToString(), true);
                NLabel.UpdateControls(form.lb_OKCount_AArea, statisticalData.OKCount.ToString(), true);
                NLabel.UpdateControls(form.lb_NGCount_AArea, statisticalData.NGCount.ToString(), true);
                NLabel.UpdateControls(form.lb_Yield_AArea, statisticalData.Yield.ToString("F2") + "%", true);

                //============================================================================
                //【2】图像显示
                PrimitiveTransition(cameraEnum, camEAStackProduct.showHalconWindow, resultTemp, camEAStackProduct.showHalconWindow.m_nImageFrameID, out Operation_ImageInfo operation_ImageInfo);
                HWC5.operation_Image.imageInfosShow.Enqueue(operation_ImageInfo);

                HOperatorSet.CountSeconds(out camEAProduct1.Image_DataProcessTime);
                HOperatorSet.CountSeconds(out camEAProduct1.Image_EndTime);
                //计算时间
                BIZ_General.CalculateProcessingTime(camEAProduct1);

                //==============================================
                //【】发送结果到PLC
                if (!Project.Instance.Is_simulation)
                {
                    if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A模切工位)
                    {
                        if (true/*resultTemp*/)
                        {
                            Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_UpTabneckCamReturnTabneckValue_ADie.Value = (float)camEAStackProduct.sizeInfos[0].DataResult;
                            Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_UpTabneckCamReturnTabneckValue_ADie, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                        }


                        Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_UpTabneckCamReturnResult_ADie.Value = (short)(resultTemp ? (short)1 : (short)2);
                        Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_UpTabneckCamReturnResult_ADie, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    }
                    else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B模切工位)
                    {
                        if (true/*resultTemp*/)
                        {
                            Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_UpTabneckCamReturnTabneckValue_BDie.Value = (float)camEAStackProduct.sizeInfos[0].DataResult;
                            Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_UpTabneckCamReturnTabneckValue_BDie, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                        }


                        Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_UpTabneckCamReturnResult_BDie.Value = (short)(resultTemp ? (short)1 : (short)2);
                        Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_UpTabneckCamReturnResult_BDie, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    }
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }

        public void ProcessingData_Cam6(ProductBase camEAProduct1)
        {
            CameraEnum cameraEnum = CameraEnum.Cam6;

            try
            {
                HOperatorSet.CountSeconds(out camEAProduct1.Image_StartDataProcessTime);

                CamEAStackProduct camEAStackProduct = camEAProduct1 as CamEAStackProduct;
                CamDetectionVariable camDetectionVariable = Project.Instance.cameraManager[cameraEnum].m_CamDetectionVariable;
                //============================================================================
                //【1】工艺处理            
                bool resultTemp = false;
                ResultType AllResultType = ResultType.NG;
                ResultType SizeAllResultType = ResultType.NG;
                ResultType DefectResultType = ResultType.NG;

                //============================================================================
                //【1.1】尺寸判断
                BIZ_General.SizeCraftJudgment_DieCutFolding(cameraEnum, stackEACraft.ListSizeEdits[(int)cameraEnum], camEAStackProduct.sizeInfos, ref camDetectionVariable.sizeDetectionItemData, out SizeAllResultType);
                BIZ_General.AddMessageShowWindow(camEAStackProduct.sizeInfos, camEAStackProduct.showHalconWindow);
                //============================================================================
                //【1.2】缺陷判断
                BIZ_General.DefectCraftProcessing_DieCutFolding(cameraEnum, stackEACraft.ListDefectInfos[(int)cameraEnum], camEAStackProduct.NgTypeInfos, ref camDetectionVariable.defectDetectionItemData,
                    ref DisplayWindowClass7, out bool isJD, out bool isDefect);
                BIZ_General.AddMessageShowWindow(camEAStackProduct.NgTypeInfos, camEAStackProduct.showHalconWindow);
                //============================================================================
                //【1.3】总结果判断
                if (isJD || isDefect)
                {
                    DefectResultType = ResultType.NG;
                }
                else
                {
                    DefectResultType = ResultType.OK;
                }

                if (SizeAllResultType == ResultType.NG ||
                    DefectResultType == ResultType.NG)
                {
                    AllResultType = ResultType.NG;
                }
                else
                {
                    AllResultType = ResultType.OK;
                }

                resultTemp = AllResultType != ResultType.NG;

                //添加曲线图数据               
                foreach (var item in camEAStackProduct.sizeInfos)
                {
                    string chartName = Project.Instance.cameraManager[cameraEnum].m_CamParam.CameraName + "_" + item.Description;
                    BIZ_General.DictionaryKeySafety(chartName, ref camDetectionVariable.sizeDetectionItemData);
                    camDetectionVariable.sizeDetectionItemData[chartName].lChartAllData.Add(new ChartPoint() { ChartDetectionData = item.DataResult }, 100);

                    ChartPoint chartTempPoint = new ChartPoint() { ChartDetectionData = item.DataResult };
                    camDetectionVariable.sizeDetectionItemData[chartName].lChartAllTempData.Add(chartTempPoint, 100);
                }

                //============================================================================
                //【1.2】数据显示、保存            
                List<RowData> showRowDataList = new List<RowData>();
                List<RowData> saveRowDataList = new List<RowData>();
                RowData timeRowData = new RowData() { dataStr = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff"), resultType = ResultType.OK };
                RowData frameRowData = new RowData() { dataStr = camEAStackProduct.showHalconWindow.m_nImageFrameID.ToString(), resultType = AllResultType };
                showRowDataList.Add(timeRowData);
                showRowDataList.Add(frameRowData);
                for (int i = 0; i < camEAStackProduct.sizeInfos.Count; i++)
                {
                    showRowDataList.Add(new RowData() { dataStr = camEAStackProduct.sizeInfos[i].DataResult.ToString("F3"), resultType = camEAStackProduct.sizeInfos[i].resultType });
                }
                showRowDataList.Add(new RowData() { dataStr = DefectResultType.ToString(), resultType = DefectResultType });


                saveRowDataList.Add(timeRowData);
                saveRowDataList.Add(frameRowData);
                for (int i = 0; i < camEAStackProduct.sizeInfos.Count; i++)
                {
                    saveRowDataList.Add(new RowData() { dataStr = camEAStackProduct.sizeInfos[i].DataResult.ToString("F3"), resultType = ResultType.OK });
                    saveRowDataList.Add(new RowData() { dataStr = camEAStackProduct.sizeInfos[i].resultType.ToString(), resultType = camEAStackProduct.sizeInfos[i].resultType });
                }
                saveRowDataList.Add(new RowData() { dataStr = DefectResultType.ToString(), resultType = DefectResultType });

                NListView.Add(form.listView6, showRowDataList.ToArray(), true);
                FileOperationCSV.WriteData_CSV(camDetectionVariable.ProdDataPath, camDetectionVariable.DataTitlesSave, saveRowDataList.ToArray(), $"{cameraEnum}");

                //生产统计
                StatisticalData statisticalData = camDetectionVariable.statisticalData;
                if (!resultTemp) statisticalData.NGCount++;
                BIZ_General.ProdStatisticsCalculate(statisticalData, camEAStackProduct.showHalconWindow.m_nImageFrameID, statisticalData.NGCount);
                NLabel.UpdateControls(form.lb_SumupCount_AArea, statisticalData.SumupCount.ToString(), true);
                NLabel.UpdateControls(form.lb_OKCount_AArea, statisticalData.OKCount.ToString(), true);
                NLabel.UpdateControls(form.lb_NGCount_AArea, statisticalData.NGCount.ToString(), true);
                NLabel.UpdateControls(form.lb_Yield_AArea, statisticalData.Yield.ToString("F2") + "%", true);

                //============================================================================
                //【2】图像显示
                PrimitiveTransition(cameraEnum, camEAStackProduct.showHalconWindow, resultTemp, camEAStackProduct.showHalconWindow.m_nImageFrameID, out Operation_ImageInfo operation_ImageInfo);
                HWC6.operation_Image.imageInfosShow.Enqueue(operation_ImageInfo);

                bIZ_Variable.Cam6_TabneckValue = camEAStackProduct.sizeInfos[0].DataResult;
                bIZ_Variable.Cam6_CurrentResult = resultTemp;
                bIZ_Variable.Cam6_Finish = true;

                HOperatorSet.CountSeconds(out camEAProduct1.Image_DataProcessTime);
                HOperatorSet.CountSeconds(out camEAProduct1.Image_EndTime);
                //计算时间
                BIZ_General.CalculateProcessingTime(camEAProduct1);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }

        public void ProcessingData_Cam7(ProductBase camEAProduct1)
        {
            CameraEnum cameraEnum = CameraEnum.Cam7;

            try
            {
                HOperatorSet.CountSeconds(out camEAProduct1.Image_StartDataProcessTime);

                CamEAStackProduct camEAStackProduct = camEAProduct1 as CamEAStackProduct;
                CamDetectionVariable camDetectionVariable = Project.Instance.cameraManager[cameraEnum].m_CamDetectionVariable;
                //============================================================================
                //【1】工艺处理            
                bool resultTemp = false;
                ResultType AllResultType = ResultType.NG;
                ResultType SizeAllResultType = ResultType.NG;
                ResultType DefectResultType = ResultType.NG;

                //============================================================================
                //【1.1】尺寸判断
                BIZ_General.SizeCraftJudgment_DieCutFolding(cameraEnum, stackEACraft.ListSizeEdits[(int)cameraEnum], camEAStackProduct.sizeInfos, ref camDetectionVariable.sizeDetectionItemData, out SizeAllResultType);
                BIZ_General.AddMessageShowWindow(camEAStackProduct.sizeInfos, camEAStackProduct.showHalconWindow);
                //============================================================================
                //【1.2】缺陷判断
                BIZ_General.DefectCraftProcessing_DieCutFolding(cameraEnum, stackEACraft.ListDefectInfos[(int)cameraEnum], camEAStackProduct.NgTypeInfos, ref camDetectionVariable.defectDetectionItemData,
                    ref DisplayWindowClass9, out bool isJD, out bool isDefect);
                BIZ_General.AddMessageShowWindow(camEAStackProduct.NgTypeInfos, camEAStackProduct.showHalconWindow);
                //============================================================================
                //【1.3】总结果判断
                if (isJD || isDefect)
                {
                    DefectResultType = ResultType.NG;
                }
                else
                {
                    DefectResultType = ResultType.OK;
                }

                if (SizeAllResultType == ResultType.NG ||
                    DefectResultType == ResultType.NG)
                {
                    AllResultType = ResultType.NG;
                }
                else
                {
                    AllResultType = ResultType.OK;
                }

                resultTemp = AllResultType != ResultType.NG;

                //添加曲线图数据               
                foreach (var item in camEAStackProduct.sizeInfos)
                {
                    string chartName = Project.Instance.cameraManager[cameraEnum].m_CamParam.CameraName + "_" + item.Description;
                    BIZ_General.DictionaryKeySafety(chartName, ref camDetectionVariable.sizeDetectionItemData);
                    camDetectionVariable.sizeDetectionItemData[chartName].lChartAllData.Add(new ChartPoint() { ChartDetectionData = item.DataResult }, 100);

                    ChartPoint chartTempPoint = new ChartPoint() { ChartDetectionData = item.DataResult };
                    camDetectionVariable.sizeDetectionItemData[chartName].lChartAllTempData.Add(chartTempPoint, 100);
                }

                //============================================================================
                //【1.2】数据显示、保存            
                List<RowData> showRowDataList = new List<RowData>();
                List<RowData> saveRowDataList = new List<RowData>();
                RowData timeRowData = new RowData() { dataStr = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff"), resultType = ResultType.OK };
                RowData frameRowData = new RowData() { dataStr = camEAStackProduct.showHalconWindow.m_nImageFrameID.ToString(), resultType = AllResultType };
                showRowDataList.Add(timeRowData);
                showRowDataList.Add(frameRowData);
                for (int i = 0; i < camEAStackProduct.sizeInfos.Count; i++)
                {
                    showRowDataList.Add(new RowData() { dataStr = camEAStackProduct.sizeInfos[i].DataResult.ToString("F3"), resultType = camEAStackProduct.sizeInfos[i].resultType });
                }
                showRowDataList.Add(new RowData() { dataStr = DefectResultType.ToString(), resultType = DefectResultType });


                saveRowDataList.Add(timeRowData);
                saveRowDataList.Add(frameRowData);
                for (int i = 0; i < camEAStackProduct.sizeInfos.Count; i++)
                {
                    saveRowDataList.Add(new RowData() { dataStr = camEAStackProduct.sizeInfos[i].DataResult.ToString("F3"), resultType = ResultType.OK });
                    saveRowDataList.Add(new RowData() { dataStr = camEAStackProduct.sizeInfos[i].resultType.ToString(), resultType = camEAStackProduct.sizeInfos[i].resultType });
                }
                saveRowDataList.Add(new RowData() { dataStr = DefectResultType.ToString(), resultType = DefectResultType });

                NListView.Add(form.listView7, showRowDataList.ToArray(), true);
                FileOperationCSV.WriteData_CSV(camDetectionVariable.ProdDataPath, camDetectionVariable.DataTitlesSave, saveRowDataList.ToArray(), $"{cameraEnum}");

                //生产统计
                StatisticalData statisticalData = camDetectionVariable.statisticalData;
                if (!resultTemp) statisticalData.NGCount++;
                BIZ_General.ProdStatisticsCalculate(statisticalData, camEAStackProduct.showHalconWindow.m_nImageFrameID, statisticalData.NGCount);
                NLabel.UpdateControls(form.lb_SumupCount_AArea, statisticalData.SumupCount.ToString(), true);
                NLabel.UpdateControls(form.lb_OKCount_AArea, statisticalData.OKCount.ToString(), true);
                NLabel.UpdateControls(form.lb_NGCount_AArea, statisticalData.NGCount.ToString(), true);
                NLabel.UpdateControls(form.lb_Yield_AArea, statisticalData.Yield.ToString("F2") + "%", true);

                //============================================================================
                //【2】图像显示
                PrimitiveTransition(cameraEnum, camEAStackProduct.showHalconWindow, resultTemp, camEAStackProduct.showHalconWindow.m_nImageFrameID, out Operation_ImageInfo operation_ImageInfo);
                HWC7.operation_Image.imageInfosShow.Enqueue(operation_ImageInfo);

                bIZ_Variable.Cam7_TabneckValue = camEAStackProduct.sizeInfos[0].DataResult;
                bIZ_Variable.Cam7_CurrentResult = resultTemp;
                bIZ_Variable.Cam7_Finish = true;

                HOperatorSet.CountSeconds(out camEAProduct1.Image_DataProcessTime);
                HOperatorSet.CountSeconds(out camEAProduct1.Image_EndTime);
                //计算时间
                BIZ_General.CalculateProcessingTime(camEAProduct1);
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
        private void PrimitiveTransition(CameraEnum cameraEnum, ShowHalconWindow showHalconWindow, bool result, long frame, out Operation_ImageInfo operation_ImageInfo)
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
                operation_ImageInfo = BIZ_General.ImageDataTypeTransition(showHalconWindow, Project.Instance.paramPathSet.ProductMojuanPath, imageSavePath, imageSaveName, saveImageFormat, imageSaveSelect, isShowImage,
                   HighDefinition_Screenshot, screenshotSaveSelect, Project.Instance.paramPathSet.ProductMojuanPath, screenshotSavePath, imageSaveName, screenshotSaveFormat, cameraEnum.ToString());
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(cameraEnum.ToString(), ex);
            }
        }

        /// <summary>
        /// 将产品图像转换为显示图像
        /// </summary>
        /// <param name="cameraEnum"></param>
        /// <param name="showHalconWindow"></param>
        /// <param name="result"></param>
        /// <param name="frame"></param>
        /// <param name="PackgeNum"></param>
        /// <param name="EAIndex"></param>
        /// <param name="operation_ImageInfo"></param>
        private void ProdImageTransition(CameraEnum cameraEnum, ShowHalconWindow showHalconWindow, bool result, long frame, long prodId, out Operation_ImageInfo operation_ImageInfo)
        {
            operation_ImageInfo = null;
            try
            {
                ImageSaveParamer in_imageSaveParamer = Project.Instance.cameraManager[cameraEnum].m_ImageSaveParamer;

                string imageSavePath = in_imageSaveParamer.ImageSave_Path_ProdImg;
                ImageSaveFormat saveImageFormat = (ImageSaveFormat)in_imageSaveParamer.SaveImageFormat_ProdImg;
                ImageSaveSelect imageSaveSelect = (ImageSaveSelect)in_imageSaveParamer.SaveImageSelect_ProdImg;
                bool isShowImage = true;
                bool HighDefinition_Screenshot = in_imageSaveParamer.HighDefinition_ProdScreenshot;
                ImageSaveSelect screenshotSaveSelect = (ImageSaveSelect)in_imageSaveParamer.SaveImageSelect_ProdScreenshot;
                string screenshotSavePath = in_imageSaveParamer.ImageSave_Path_ProdScreenshot;
                ImageSaveFormat screenshotSaveFormat = (ImageSaveFormat)in_imageSaveParamer.SaveImageFormat_ProdScreenshot;
                string imageSaveName = frame.ToString().PadLeft(5, '0') + $"_{prodId}";
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
        private void NGImageQueueTransition(CameraEnum cameraEnum, List<NgTypeInfo> ngTypeInfos, long frame, bool isDB, ref QueuePackage<Operation_ImageInfo> out_operationImageInfos, bool abnormal = false)
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
                    string imageSavePath = "";
                    if (abnormal) imageSavePath = in_imageSaveParamer.ImageSave_Path_Min + "\\" + "异常";
                    else imageSavePath = in_imageSaveParamer.ImageSave_Path_Min + "\\" + (isDB ? "打标" : "不打标");
                    imageSavePath += $"\\{ngTypeInfo.m_strNgTypeName}\\";
                    ImageSaveFormat saveImageFormat = (ImageSaveFormat)in_imageSaveParamer.SaveImageFormat_Min;
                    bool isSaveImage = in_imageSaveParamer.IsSaveImage_Min;
                    bool isShowImage = in_imageSaveParamer.IsShowImage_Min;
                    string imageSaveName = frame.ToString().PadLeft(5, '0');
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

        public ProcessExecutionResult GetProdResult()
        {
            try
            {
                //================================================================
                //【1】产品判断         
                if (bIZ_Variable.Cam6_Finish == true &&
                    bIZ_Variable.Cam7_Finish == true)
                {

                    return ProcessExecutionResult.Success;
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
            if (stackEACraft.ListSizeEdits[(int)cameraEnum].Where(x => x.Name == sizeName).Count() > 0)
            {
                sizeNameDescription = stackEACraft.ListSizeEdits[(int)cameraEnum].Find(p => p.Name == sizeName).Description;
                standard = stackEACraft.ListSizeEdits[(int)cameraEnum].Find(p => p.Name == sizeName).StDValue;
                upper = stackEACraft.ListSizeEdits[(int)cameraEnum].Find(p => p.Name == sizeName).StD_Up;
                lower = stackEACraft.ListSizeEdits[(int)cameraEnum].Find(p => p.Name == sizeName).StD_Down;
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
                cameraEnum = CameraEnum.Cam3;
                if (true)
                {
                    string sizeName = "基准距离Y";
                    GetSizeCarft(cameraEnum, sizeName, out string sizeNameDescription, out double upper, out double standard, out double lower);
                    string chartName = Project.Instance.cameraManager[cameraEnum].m_CamParam.CameraName + "_" + sizeNameDescription;

                    ParamsChart paramsChart = new ParamsChart();
                    paramsChart.f_UpLimit = standard + upper;
                    paramsChart.f_DownLimit = standard + lower;
                    paramsChart.f_StandarValue = standard;
                    paramsChart.points.Add(new PointClass() { CurName = "测量值", Point = new Point(0, 0), Color = Color.Blue });
                    chartUIParams[chartName] = new ChartUIParam(chartName, form.formsPlot_FJJPY);
                    chartUIParams[chartName].InitCurve(paramsChart);
                }

                cameraEnum = CameraEnum.Cam4;
                if (true)
                {
                    string sizeName = "基准距离Y";
                    GetSizeCarft(cameraEnum, sizeName, out string sizeNameDescription, out double upper, out double standard, out double lower);
                    string chartName = Project.Instance.cameraManager[cameraEnum].m_CamParam.CameraName + "_" + sizeNameDescription;

                    ParamsChart paramsChart = new ParamsChart();
                    paramsChart.f_UpLimit = standard + upper;
                    paramsChart.f_DownLimit = standard + lower;
                    paramsChart.f_StandarValue = standard;
                    paramsChart.points.Add(new PointClass() { CurName = "测量值", Point = new Point(0, 0), Color = Color.Blue });
                    chartUIParams[chartName] = new ChartUIParam(chartName, form.formsPlot_MQJPY);
                    chartUIParams[chartName].InitCurve(paramsChart);
                }

                if (true)
                {
                    string sizeName = "基准距离X";
                    GetSizeCarft(cameraEnum, sizeName, out string sizeNameDescription, out double upper, out double standard, out double lower);
                    string chartName = Project.Instance.cameraManager[cameraEnum].m_CamParam.CameraName + "_" + sizeNameDescription;

                    ParamsChart paramsChart = new ParamsChart();
                    paramsChart.f_UpLimit = standard + upper;
                    paramsChart.f_DownLimit = standard + lower;
                    paramsChart.f_StandarValue = standard;
                    paramsChart.points.Add(new PointClass() { CurName = "测量值", Point = new Point(0, 0), Color = Color.Blue });
                    chartUIParams[chartName] = new ChartUIParam(chartName, form.formsPlot_MQJPX);
                    chartUIParams[chartName].InitCurve(paramsChart);
                }

                cameraEnum = CameraEnum.Cam5;
                if (true)
                {
                    string sizeName = "TabNeckX";
                    GetSizeCarft(cameraEnum, sizeName, out string sizeNameDescription, out double upper, out double standard, out double lower);
                    string chartName = Project.Instance.cameraManager[cameraEnum].m_CamParam.CameraName + "_" + sizeNameDescription;

                    ParamsChart paramsChart = new ParamsChart();
                    paramsChart.f_UpLimit = standard + upper;
                    paramsChart.f_DownLimit = standard + lower;
                    paramsChart.f_StandarValue = standard;
                    paramsChart.points.Add(new PointClass() { CurName = "测量值", Point = new Point(0, 0), Color = Color.Blue });
                    chartUIParams[chartName] = new ChartUIParam(chartName, form.formsPlot_LUTabneck);
                    chartUIParams[chartName].InitCurve(paramsChart);
                }

                cameraEnum = CameraEnum.Cam6;
                if (true)
                {
                    string sizeName = "TabNeckX";
                    GetSizeCarft(cameraEnum, sizeName, out string sizeNameDescription, out double upper, out double standard, out double lower);
                    string chartName = Project.Instance.cameraManager[cameraEnum].m_CamParam.CameraName + "_" + sizeNameDescription;

                    ParamsChart paramsChart = new ParamsChart();
                    paramsChart.f_UpLimit = standard + upper;
                    paramsChart.f_DownLimit = standard + lower;
                    paramsChart.f_StandarValue = standard;
                    paramsChart.points.Add(new PointClass() { CurName = "测量值", Point = new Point(0, 0), Color = Color.Blue });
                    chartUIParams[chartName] = new ChartUIParam(chartName, form.formsPlot_LDTabneck);
                    chartUIParams[chartName].InitCurve(paramsChart);
                }

                cameraEnum = CameraEnum.Cam7;
                if (true)
                {
                    string sizeName = "TabNeckX";
                    GetSizeCarft(cameraEnum, sizeName, out string sizeNameDescription, out double upper, out double standard, out double lower);
                    string chartName = Project.Instance.cameraManager[cameraEnum].m_CamParam.CameraName + "_" + sizeNameDescription;

                    ParamsChart paramsChart = new ParamsChart();
                    paramsChart.f_UpLimit = standard + upper;
                    paramsChart.f_DownLimit = standard + lower;
                    paramsChart.f_StandarValue = standard;
                    paramsChart.points.Add(new PointClass() { CurName = "测量值", Point = new Point(0, 0), Color = Color.Blue });
                    chartUIParams[chartName] = new ChartUIParam(chartName, form.formsPlot_RDTabneck);
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
                foreach (var item in Project.Instance.cameraManager[cameraEnum].m_CamDetectionVariable.sizeDetectionItemData)
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
                if (Project.Instance.Is_Run)
                {
                    bool IsIntervalTime = intervalExecution.GetWhetherToReachIntervalTime(1000);
                    if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A模切工位)
                    {
                        //心跳信号
                        if (IsIntervalTime)
                        {
                            Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_CorrectHeart_CCD_ADie.Value = 1;
                            Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_CorrectHeart_CCD_ADie, true, Project.Instance.cutOverlayPLCManage.HeartbeatLog);

                            Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_LineHeart_CCD_ADie.Value = (short)1;
                            Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_LineHeart_CCD_ADie, true, Project.Instance.cutOverlayPLCManage.HeartbeatLog);
                        }

                        Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_LineCamDist_ADie, ReadLogSaveType.ReadDifferentState, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                        //记录PLC纠偏量
                        Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.InOut_DieCutCorrectReadOffsetTrigger_ADie, ReadLogSaveType.ReadDifferentState, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                        bool IsReadOffsetTrigger = Project.Instance.cutOverlayPLCManage.m_DevAddress.InOut_DieCutCorrectReadOffsetTrigger_ADie.Value;
                        if (IsReadOffsetTrigger)
                        {
                            Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_DieCutCorrectOffsetValueY_ADie, ReadLogSaveType.True, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                            Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_DieCutCorrectOffsetValueR_ADie, ReadLogSaveType.True, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                            double OffsetValueY = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_DieCutCorrectOffsetValueY_ADie.Value;
                            double OffsetValueR = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_DieCutCorrectOffsetValueR_ADie.Value;
                            //读完复位触发信号
                            Project.Instance.cutOverlayPLCManage.m_DevAddress.InOut_DieCutCorrectReadOffsetTrigger_ADie.Value = false;
                            Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.InOut_DieCutCorrectReadOffsetTrigger_ADie, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                            //读取纠偏量日志
                            string[] title = new string[]
                            {
                                        "时间",
                                        "读取纠偏量使能",
                                        "纠偏Y轴纠偏量",
                                        "纠偏R轴纠偏量",
                            };

                            string[] data = new string[]
                                  {
                                         $"{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff")}",
                                         $"{IsReadOffsetTrigger}",
                                         $"{OffsetValueY}",
                                         $"{OffsetValueR}",
                                  };

                            FileOperationCSV.WriteDataLock_CSV(Project.Instance.paramPathSet.DieCutCorrectOffsetValueLogPath,
                                title, data, CameraEnum.Cam1.ToString());
                        }
                        //PLC下发一键换卷
                        AutoChangeRoll();
                    }
                    else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B模切工位)
                    {
                        //心跳信号
                        if (IsIntervalTime)
                        {
                            Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_CorrectHeart_CCD_BDie.Value = 1;
                            Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_CorrectHeart_CCD_BDie, true, Project.Instance.cutOverlayPLCManage.HeartbeatLog);

                            Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_LineHeart_CCD_BDie.Value = (short)1;
                            Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_LineHeart_CCD_BDie, true, Project.Instance.cutOverlayPLCManage.HeartbeatLog);
                        }

                        Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_LineCamDist_BDie, ReadLogSaveType.ReadDifferentState, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                        //记录PLC纠偏量
                        Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.InOut_DieCutCorrectReadOffsetTrigger_BDie, ReadLogSaveType.ReadDifferentState, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                        bool IsReadOffsetTrigger = Project.Instance.cutOverlayPLCManage.m_DevAddress.InOut_DieCutCorrectReadOffsetTrigger_BDie.Value;
                        if (IsReadOffsetTrigger)
                        {
                            Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_DieCutCorrectOffsetValueY_BDie, ReadLogSaveType.True, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                            Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_DieCutCorrectOffsetValueR_BDie, ReadLogSaveType.True, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                            double OffsetValueY = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_DieCutCorrectOffsetValueY_BDie.Value;
                            double OffsetValueR = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_DieCutCorrectOffsetValueR_BDie.Value;
                            //读完复位触发信号
                            Project.Instance.cutOverlayPLCManage.m_DevAddress.InOut_DieCutCorrectReadOffsetTrigger_BDie.Value = false;
                            Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.InOut_DieCutCorrectReadOffsetTrigger_BDie, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                            //读取纠偏量日志
                            string[] title = new string[]
                            {
                                        "时间",
                                        "读取纠偏量使能",
                                        "纠偏Y轴纠偏量",
                                        "纠偏R轴纠偏量",
                            };

                            string[] data = new string[]
                                  {
                                         $"{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff")}",
                                         $"{IsReadOffsetTrigger}",
                                         $"{OffsetValueY}",
                                         $"{OffsetValueR}",
                                  };

                            FileOperationCSV.WriteDataLock_CSV(Project.Instance.paramPathSet.DieCutCorrectOffsetValueLogPath,
                                title, data, CameraEnum.Cam1.ToString());
                        }
                        //PLC下发一键换卷
                        AutoChangeRoll();
                    }
                    else
                    {
                        Log4netRecord.ErrorLog.Debug("工位选择错误");
                        TJBJInfo.AlarmWriteState(AlarmName.工位选择错误, StopAlarmStateEnum.停机, "");
                        return;
                    }
                }


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

                if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A模切工位)
                {
                    //记录PLC纠偏量
                    Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.InOut_ChangeRollTrigger_ADie, ReadLogSaveType.ReadDifferentState, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    bool IsChangeRollTrigger = Project.Instance.cutOverlayPLCManage.m_DevAddress.InOut_ChangeRollTrigger_ADie.Value;
                    if (IsChangeRollTrigger)
                    {
                        bool result = Form_Main.FormMain.MainDetection.AutoHJ(reelNumber);
                        if (result)
                        {
                            Project.Instance.cutOverlayPLCManage.m_DevAddress.InOut_ChangeRollResult_ADie.Value = true;
                        }
                        else
                        {
                            Project.Instance.cutOverlayPLCManage.m_DevAddress.InOut_ChangeRollResult_ADie.Value = false;
                        }
                        Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.InOut_ChangeRollResult_ADie, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    }
                }
                else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B模切工位)
                {
                    //记录PLC纠偏量
                    Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.InOut_ChangeRollTrigger_BDie, ReadLogSaveType.ReadDifferentState, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    bool IsChangeRollTrigger = Project.Instance.cutOverlayPLCManage.m_DevAddress.InOut_ChangeRollTrigger_BDie.Value;
                    if (IsChangeRollTrigger)
                    {
                        bool result = Form_Main.FormMain.MainDetection.AutoHJ(reelNumber);
                        if (result)
                        {
                            Project.Instance.cutOverlayPLCManage.m_DevAddress.InOut_ChangeRollResult_BDie.Value = true;
                        }
                        else
                        {
                            Project.Instance.cutOverlayPLCManage.m_DevAddress.InOut_ChangeRollResult_BDie.Value = false;
                        }
                        Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.InOut_ChangeRollResult_BDie, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
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
            //初始化数据显示界面
            NLabel.UpdateControls(form.LB_Formula, Project.Instance.systemInfo.CurrentCraft);
            NLabel.UpdateControls(form.LB_DetectionType, Project.Instance.craftBase.materialCrafEnum.ToString());


            if (true)
            {
                CameraEnum cameraEnum = CameraEnum.Cam1;
                CamDetectionVariable camDetectionVariable = Project.Instance.cameraManager[cameraEnum].m_CamDetectionVariable;

                NListView.Init(form.listView1, camDetectionVariable.DataTitlesShow);
                NLabel.UpdateControls(form.lb_SumupCount_ADefect, camDetectionVariable.statisticalData.SumupCount.ToString(), true);
                NLabel.UpdateControls(form.lb_OKCount_ADefect, camDetectionVariable.statisticalData.OKCount.ToString(), true);
                NLabel.UpdateControls(form.lb_NGCount_ADefect, camDetectionVariable.statisticalData.NGCount.ToString(), true);
                NLabel.UpdateControls(form.lb_Yield_ADefect, camDetectionVariable.statisticalData.Yield.ToString(), true);
            }

            if (true)
            {
                CameraEnum cameraEnum = CameraEnum.Cam2;
                CamDetectionVariable camDetectionVariable = Project.Instance.cameraManager[cameraEnum].m_CamDetectionVariable;

                NListView.Init(form.listView2, camDetectionVariable.DataTitlesShow);
                NLabel.UpdateControls(form.lb_SumupCount_BDefect, camDetectionVariable.statisticalData.SumupCount.ToString(), true);
                NLabel.UpdateControls(form.lb_OKCount_BDefect, camDetectionVariable.statisticalData.OKCount.ToString(), true);
                NLabel.UpdateControls(form.lb_NGCount_BDefect, camDetectionVariable.statisticalData.NGCount.ToString(), true);
                NLabel.UpdateControls(form.lb_Yield_BDefect, camDetectionVariable.statisticalData.Yield.ToString(), true);
            }


            if (true)
            {
                CameraEnum cameraEnum = CameraEnum.Cam3;
                CamDetectionVariable camDetectionVariable = Project.Instance.cameraManager[cameraEnum].m_CamDetectionVariable;

                NListView.Init(form.listView3, camDetectionVariable.DataTitlesShow);

            }

            if (true)
            {
                CameraEnum cameraEnum = CameraEnum.Cam4;
                CamDetectionVariable camDetectionVariable = Project.Instance.cameraManager[cameraEnum].m_CamDetectionVariable;

                NListView.Init(form.listView4, camDetectionVariable.DataTitlesShow);

            }

            if (true)
            {
                CameraEnum cameraEnum = CameraEnum.Cam5;
                CamDetectionVariable camDetectionVariable = Project.Instance.cameraManager[cameraEnum].m_CamDetectionVariable;

                NListView.Init(form.listView5, camDetectionVariable.DataTitlesShow);
                NLabel.UpdateControls(form.lb_SumupCount_AArea, camDetectionVariable.statisticalData.SumupCount.ToString(), true);
                NLabel.UpdateControls(form.lb_OKCount_AArea, camDetectionVariable.statisticalData.OKCount.ToString(), true);
                NLabel.UpdateControls(form.lb_NGCount_AArea, camDetectionVariable.statisticalData.NGCount.ToString(), true);
                NLabel.UpdateControls(form.lb_Yield_AArea, camDetectionVariable.statisticalData.Yield.ToString(), true);
            }

            if (true)
            {
                CameraEnum cameraEnum = CameraEnum.Cam6;
                CamDetectionVariable camDetectionVariable = Project.Instance.cameraManager[cameraEnum].m_CamDetectionVariable;

                NListView.Init(form.listView6, camDetectionVariable.DataTitlesShow);
                NLabel.UpdateControls(form.lb_SumupCount_B1Area, camDetectionVariable.statisticalData.SumupCount.ToString(), true);
                NLabel.UpdateControls(form.lb_OKCount_B1Area, camDetectionVariable.statisticalData.OKCount.ToString(), true);
                NLabel.UpdateControls(form.lb_NGCount_B1Area, camDetectionVariable.statisticalData.NGCount.ToString(), true);
                NLabel.UpdateControls(form.lb_Yield_B1Area, camDetectionVariable.statisticalData.Yield.ToString(), true);
            }

            if (true)
            {
                CameraEnum cameraEnum = CameraEnum.Cam7;
                CamDetectionVariable camDetectionVariable = Project.Instance.cameraManager[cameraEnum].m_CamDetectionVariable;

                NListView.Init(form.listView7, camDetectionVariable.DataTitlesShow);
                NLabel.UpdateControls(form.lb_SumupCount_B2Area, camDetectionVariable.statisticalData.SumupCount.ToString(), true);
                NLabel.UpdateControls(form.lb_OKCount_B2Area, camDetectionVariable.statisticalData.OKCount.ToString(), true);
                NLabel.UpdateControls(form.lb_NGCount_B2Area, camDetectionVariable.statisticalData.NGCount.ToString(), true);
                NLabel.UpdateControls(form.lb_Yield_B2Area, camDetectionVariable.statisticalData.Yield.ToString(), true);
            }

            NLabel.UpdateControls(form.LB_Process1, Project.Instance.cameraManager[CameraEnum.Cam1].m_CamParam.CameraName);
            NLabel.UpdateControls(form.LB_Process2, Project.Instance.cameraManager[CameraEnum.Cam2].m_CamParam.CameraName);
            NLabel.UpdateControls(form.LB_Process3, Project.Instance.cameraManager[CameraEnum.Cam3].m_CamParam.CameraName);
            NLabel.UpdateControls(form.LB_Process4, Project.Instance.cameraManager[CameraEnum.Cam4].m_CamParam.CameraName);
            NLabel.UpdateControls(form.LB_Process5, Project.Instance.cameraManager[CameraEnum.Cam5].m_CamParam.CameraName);
            NLabel.UpdateControls(form.LB_Process6, Project.Instance.cameraManager[CameraEnum.Cam6].m_CamParam.CameraName);
            NLabel.UpdateControls(form.LB_Process7, Project.Instance.cameraManager[CameraEnum.Cam7].m_CamParam.CameraName);

            NLabel.UpdateControls(form.lblv1, Project.Instance.cameraManager[CameraEnum.Cam1].m_CamParam.CameraName);
            NLabel.UpdateControls(form.lblv2, Project.Instance.cameraManager[CameraEnum.Cam2].m_CamParam.CameraName);
            NLabel.UpdateControls(form.lblv3, Project.Instance.cameraManager[CameraEnum.Cam3].m_CamParam.CameraName);
            NLabel.UpdateControls(form.lblv4, Project.Instance.cameraManager[CameraEnum.Cam4].m_CamParam.CameraName);
            NLabel.UpdateControls(form.lblv5, Project.Instance.cameraManager[CameraEnum.Cam5].m_CamParam.CameraName);
            NLabel.UpdateControls(form.lblv6, Project.Instance.cameraManager[CameraEnum.Cam6].m_CamParam.CameraName);
            NLabel.UpdateControls(form.lblv7, Project.Instance.cameraManager[CameraEnum.Cam7].m_CamParam.CameraName);

            InitChart();
        }

        public void ClearForm()
        {
            NLabel.UpdateControls(form.LB_Formula, Project.Instance.systemInfo.CurrentCraft);
            NLabel.UpdateControls(form.LB_DetectionType, Project.Instance.craftBase.materialCrafEnum.ToString());

            if (true)
            {
                CameraEnum cameraEnum = CameraEnum.Cam1;
                CamDetectionVariable camDetectionVariable = Project.Instance.cameraManager[cameraEnum].m_CamDetectionVariable;

                NListView.DeleteListAllItems(form.listView1);
                NLabel.UpdateControls(form.lb_SumupCount_ADefect, camDetectionVariable.statisticalData.SumupCount.ToString(), true);
                NLabel.UpdateControls(form.lb_OKCount_ADefect, camDetectionVariable.statisticalData.OKCount.ToString(), true);
                NLabel.UpdateControls(form.lb_NGCount_ADefect, camDetectionVariable.statisticalData.NGCount.ToString(), true);
                NLabel.UpdateControls(form.lb_Yield_ADefect, camDetectionVariable.statisticalData.Yield.ToString(), true);
            }
            if (true)
            {
                CameraEnum cameraEnum = CameraEnum.Cam2;
                CamDetectionVariable camDetectionVariable = Project.Instance.cameraManager[cameraEnum].m_CamDetectionVariable;

                NListView.DeleteListAllItems(form.listView2);
                NLabel.UpdateControls(form.lb_SumupCount_BDefect, camDetectionVariable.statisticalData.SumupCount.ToString(), true);
                NLabel.UpdateControls(form.lb_OKCount_BDefect, camDetectionVariable.statisticalData.OKCount.ToString(), true);
                NLabel.UpdateControls(form.lb_NGCount_BDefect, camDetectionVariable.statisticalData.NGCount.ToString(), true);
                NLabel.UpdateControls(form.lb_Yield_BDefect, camDetectionVariable.statisticalData.Yield.ToString(), true);
            }
            if (true)
            {
                CameraEnum cameraEnum = CameraEnum.Cam3;
                CamDetectionVariable camDetectionVariable = Project.Instance.cameraManager[cameraEnum].m_CamDetectionVariable;

                NListView.DeleteListAllItems(form.listView3);
            }
            if (true)
            {
                CameraEnum cameraEnum = CameraEnum.Cam3;
                CamDetectionVariable camDetectionVariable = Project.Instance.cameraManager[cameraEnum].m_CamDetectionVariable;

                NListView.DeleteListAllItems(form.listView4);
            }
            if (true)
            {
                CameraEnum cameraEnum = CameraEnum.Cam5;
                CamDetectionVariable camDetectionVariable = Project.Instance.cameraManager[cameraEnum].m_CamDetectionVariable;

                NListView.DeleteListAllItems(form.listView5);
                NLabel.UpdateControls(form.lb_SumupCount_AArea, camDetectionVariable.statisticalData.SumupCount.ToString(), true);
                NLabel.UpdateControls(form.lb_OKCount_AArea, camDetectionVariable.statisticalData.OKCount.ToString(), true);
                NLabel.UpdateControls(form.lb_NGCount_AArea, camDetectionVariable.statisticalData.NGCount.ToString(), true);
                NLabel.UpdateControls(form.lb_Yield_AArea, camDetectionVariable.statisticalData.Yield.ToString(), true);
            }

            if (true)
            {
                CameraEnum cameraEnum = CameraEnum.Cam6;
                CamDetectionVariable camDetectionVariable = Project.Instance.cameraManager[cameraEnum].m_CamDetectionVariable;

                NListView.DeleteListAllItems(form.listView6);
                NLabel.UpdateControls(form.lb_SumupCount_B1Area, camDetectionVariable.statisticalData.SumupCount.ToString(), true);
                NLabel.UpdateControls(form.lb_OKCount_B1Area, camDetectionVariable.statisticalData.OKCount.ToString(), true);
                NLabel.UpdateControls(form.lb_NGCount_B1Area, camDetectionVariable.statisticalData.NGCount.ToString(), true);
                NLabel.UpdateControls(form.lb_Yield_B1Area, camDetectionVariable.statisticalData.Yield.ToString(), true);
            }

            if (true)
            {
                CameraEnum cameraEnum = CameraEnum.Cam7;
                CamDetectionVariable camDetectionVariable = Project.Instance.cameraManager[cameraEnum].m_CamDetectionVariable;

                NListView.DeleteListAllItems(form.listView7);
                NLabel.UpdateControls(form.lb_SumupCount_B2Area, camDetectionVariable.statisticalData.SumupCount.ToString(), true);
                NLabel.UpdateControls(form.lb_OKCount_B2Area, camDetectionVariable.statisticalData.OKCount.ToString(), true);
                NLabel.UpdateControls(form.lb_NGCount_B2Area, camDetectionVariable.statisticalData.NGCount.ToString(), true);
                NLabel.UpdateControls(form.lb_Yield_B2Area, camDetectionVariable.statisticalData.Yield.ToString(), true);
            }
            InitChart();
        }

        public void UpdateForm()
        {
            try
            {
                //曲线图    
                CamChartDataShow(CameraEnum.Cam3);
                CamChartDataShow(CameraEnum.Cam4);
                CamChartDataShow(CameraEnum.Cam5);
                CamChartDataShow(CameraEnum.Cam6);
                CamChartDataShow(CameraEnum.Cam7);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }

        public void InitVariable()
        {
            List<string> dataShowTitles = new List<string>();
            List<string> dataSaveTitles = new List<string>();

            foreach (var item in Project.Instance.cameraManager)
            {
                string path = item.Value.m_CamDetectionVariable.ProdDataPath;
                item.Value.m_CamDetectionVariable = new CamDetectionVariable();
                item.Value.m_CamDetectionVariable.ProdDataPath = path;
            }
            if (true)
            {
                CameraEnum cameraEnum = CameraEnum.Cam1;
                CamDetectionVariable camDetectionVariable = Project.Instance.cameraManager[cameraEnum].m_CamDetectionVariable;

                //数据表头初始化
                dataShowTitles.AddRange(new string[] { "时间", "帧号", "瑕疵结果" });

                camDetectionVariable.DataTitlesShow = dataShowTitles.ToArray();
                camDetectionVariable.DataTitlesSave = dataShowTitles.ToArray();
                dataShowTitles.Clear();
                dataSaveTitles.Clear();
            }


            if (true)
            {
                CameraEnum cameraEnum = CameraEnum.Cam2;
                CamDetectionVariable camDetectionVariable = Project.Instance.cameraManager[cameraEnum].m_CamDetectionVariable;

                //数据表头初始化
                dataShowTitles.AddRange(new string[] { "时间", "帧号", "瑕疵结果" });

                camDetectionVariable.DataTitlesShow = dataShowTitles.ToArray();
                camDetectionVariable.DataTitlesSave = dataShowTitles.ToArray();
                dataShowTitles.Clear();
                dataSaveTitles.Clear();
            }

            if (true)
            {
                CameraEnum cameraEnum = CameraEnum.Cam3;
                CamDetectionVariable camDetectionVariable = Project.Instance.cameraManager[cameraEnum].m_CamDetectionVariable;

                //数据表头初始化
                dataShowTitles.AddRange(new string[] { "时间", "帧号" });
                string[] strings = stackEACraft.ListSizeEdits[(int)cameraEnum].SelectMany(p => new string[] { "结果", p.Description }).ToArray();
                dataShowTitles.AddRange(strings);

                camDetectionVariable.DataTitlesShow = dataShowTitles.ToArray();
                camDetectionVariable.DataTitlesSave = dataShowTitles.ToArray();
                dataShowTitles.Clear();
                dataSaveTitles.Clear();
            }

            if (true)
            {
                CameraEnum cameraEnum = CameraEnum.Cam4;
                CamDetectionVariable camDetectionVariable = Project.Instance.cameraManager[cameraEnum].m_CamDetectionVariable;

                //数据表头初始化
                //数据表头初始化
                dataShowTitles.AddRange(new string[] { "时间", "帧号" });
                string[] strings = stackEACraft.ListSizeEdits[(int)cameraEnum].SelectMany(p => new string[] { "结果", p.Description }).ToArray();
                dataShowTitles.AddRange(strings);

                camDetectionVariable.DataTitlesShow = dataShowTitles.ToArray();
                camDetectionVariable.DataTitlesSave = dataShowTitles.ToArray();
                dataShowTitles.Clear();
                dataSaveTitles.Clear();
            }

            if (true)
            {
                CameraEnum cameraEnum = CameraEnum.Cam5;
                CamDetectionVariable camDetectionVariable = Project.Instance.cameraManager[cameraEnum].m_CamDetectionVariable;

                //数据表头初始化
                string[] stringsShow = stackEACraft.ListSizeEdits[(int)cameraEnum].SelectMany(p => new string[] { p.Description }).ToArray();
                string[] stringsSave = stackEACraft.ListSizeEdits[(int)cameraEnum].SelectMany(p => new string[] { p.Description, "结果" }).ToArray();

                dataShowTitles.AddRange(new string[] { "时间", "帧号" });
                dataShowTitles.AddRange(stringsShow);
                dataShowTitles.Add("瑕疵结果");

                dataSaveTitles.AddRange(new string[] { "时间", "帧号" });
                dataSaveTitles.AddRange(stringsSave);
                dataSaveTitles.Add("瑕疵结果");

                camDetectionVariable.DataTitlesShow = dataShowTitles.ToArray();
                camDetectionVariable.DataTitlesSave = dataSaveTitles.ToArray();
                dataShowTitles.Clear();
                dataSaveTitles.Clear();
            }

            if (true)
            {
                CameraEnum cameraEnum = CameraEnum.Cam6;
                CamDetectionVariable camDetectionVariable = Project.Instance.cameraManager[cameraEnum].m_CamDetectionVariable;

                //数据表头初始化
                string[] stringsShow = stackEACraft.ListSizeEdits[(int)cameraEnum].SelectMany(p => new string[] { p.Description }).ToArray();
                string[] stringsSave = stackEACraft.ListSizeEdits[(int)cameraEnum].SelectMany(p => new string[] { p.Description, "结果" }).ToArray();

                dataShowTitles.AddRange(new string[] { "时间", "帧号" });
                dataShowTitles.AddRange(stringsShow);
                dataShowTitles.Add("瑕疵结果");

                dataSaveTitles.AddRange(new string[] { "时间", "帧号" });
                dataSaveTitles.AddRange(stringsSave);
                dataSaveTitles.Add("瑕疵结果");

                camDetectionVariable.DataTitlesShow = dataShowTitles.ToArray();
                camDetectionVariable.DataTitlesSave = dataSaveTitles.ToArray();
                dataShowTitles.Clear();
                dataSaveTitles.Clear();
            }

            if (true)
            {
                CameraEnum cameraEnum = CameraEnum.Cam7;
                CamDetectionVariable camDetectionVariable = Project.Instance.cameraManager[cameraEnum].m_CamDetectionVariable;

                //数据表头初始化
                string[] stringsShow = stackEACraft.ListSizeEdits[(int)cameraEnum].SelectMany(p => new string[] { p.Description }).ToArray();
                string[] stringsSave = stackEACraft.ListSizeEdits[(int)cameraEnum].SelectMany(p => new string[] { p.Description, "结果" }).ToArray();

                dataShowTitles.AddRange(new string[] { "时间", "帧号" });
                dataShowTitles.AddRange(stringsShow);
                dataShowTitles.Add("瑕疵结果");

                dataSaveTitles.AddRange(new string[] { "时间", "帧号" });
                dataSaveTitles.AddRange(stringsSave);
                dataSaveTitles.Add("瑕疵结果");

                camDetectionVariable.DataTitlesShow = dataShowTitles.ToArray();
                camDetectionVariable.DataTitlesSave = dataSaveTitles.ToArray();
                dataShowTitles.Clear();
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

                if (true)
                {
                    Operation_ImageProcessor processor = DisplayWindowClass1.operation_Image;
                    if (processor.imageInfosShow.Count > 0)
                    {
                        DisplayWindowClass displayWindowClass = CombinationHWindowControl_cam1.GetDisplayWindowClass();

                        if (processor != null && processor.imageInfosShow != null && processor.imageInfosShow.Count > 0)
                            processor.ShowImagesAsync(processor.imageInfosShow, displayWindowClass, processor.imageInfosSave, processor.imageInfosScreenshotSave);
                    }
                    BIZ_General.Image_Process(processor, disposeImageRetainLenght);
                }

                if (true)
                {
                    Operation_ImageProcessor processor = DisplayWindowClass3.operation_Image;
                    if (processor.imageInfosShow.Count > 0)
                    {
                        DisplayWindowClass displayWindowClass = CombinationHWindowControl_cam2.GetDisplayWindowClass();

                        if (processor != null && processor.imageInfosShow != null && processor.imageInfosShow.Count > 0)
                            processor.ShowImagesAsync(processor.imageInfosShow, displayWindowClass, processor.imageInfosSave, processor.imageInfosScreenshotSave);
                    }
                    BIZ_General.Image_Process(processor, disposeImageRetainLenght);
                }

                if (true)
                {
                    Operation_ImageProcessor processor = DisplayWindowClass5.operation_Image;
                    if (processor.imageInfosShow.Count > 0)
                    {
                        DisplayWindowClass displayWindowClass = CombinationHWindowControl_cam5.GetDisplayWindowClass();

                        if (processor != null && processor.imageInfosShow != null && processor.imageInfosShow.Count > 0)
                            processor.ShowImagesAsync(processor.imageInfosShow, displayWindowClass, processor.imageInfosSave, processor.imageInfosScreenshotSave);
                    }
                    BIZ_General.Image_Process(processor, disposeImageRetainLenght);
                }

                if (true)
                {
                    Operation_ImageProcessor processor = DisplayWindowClass7.operation_Image;
                    if (processor.imageInfosShow.Count > 0)
                    {
                        DisplayWindowClass displayWindowClass = CombinationHWindowControl_cam6.GetDisplayWindowClass();

                        if (processor != null && processor.imageInfosShow != null && processor.imageInfosShow.Count > 0)
                            processor.ShowImagesAsync(processor.imageInfosShow, displayWindowClass, processor.imageInfosSave, processor.imageInfosScreenshotSave);
                    }
                    BIZ_General.Image_Process(processor, disposeImageRetainLenght);
                }

                if (true)
                {
                    Operation_ImageProcessor processor = DisplayWindowClass9.operation_Image;
                    if (processor.imageInfosShow.Count > 0)
                    {
                        DisplayWindowClass displayWindowClass = CombinationHWindowControl_cam7.GetDisplayWindowClass();

                        if (processor != null && processor.imageInfosShow != null && processor.imageInfosShow.Count > 0)
                            processor.ShowImagesAsync(processor.imageInfosShow, displayWindowClass, processor.imageInfosSave, processor.imageInfosScreenshotSave);
                    }
                    BIZ_General.Image_Process(processor, disposeImageRetainLenght);
                }
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

            DisplayWindowClass1?.Dispose();
            DisplayWindowClass2?.Dispose();
            DisplayWindowClass3?.Dispose();
            DisplayWindowClass4?.Dispose();
            DisplayWindowClass5?.Dispose();
            DisplayWindowClass6?.Dispose();
            DisplayWindowClass7?.Dispose();
            DisplayWindowClass8?.Dispose();
            DisplayWindowClass9?.Dispose();


        }

        public void SimLoadImage(bool simAutoFlag, ref bool simRunFlag)
        {
            try
            {
                if (simRunFlag)
                {
                    Parallel.ForEach(Project.Instance.baseProDevice.m_DicCamOperaParam_Sim, (item) =>
                    {
                        bool flag = SimControl.LoadImage_Sim(Project.Instance.cameraManager[(CameraEnum)item.Key].m_ImageSimManage,
                                            ref item.Value.QueueGrabImage, $"{((CameraEnum)item.Key).ToString()}仿真加载图片");
                        if (flag)
                        {
                            if (item.Key < 2)
                            {
                                (item.Value as DieCutSliceParam).CallBackImgDataDeal();
                                //(item.Value as DieCutSliceParam).semaphoreSlimCam.Release();
                                string str = item.Key.ToString() + "信号量--" + (item.Value as DieCutSliceParam).semaphoreSlimCam.CurrentCount.ToString() + "图像数--" + item.Value.QueueGrabImage.Count.ToString();
                                //  Console.WriteLine(str);
                            }
                            else
                            {
                                (item.Value as AreaCamParam).CallBackImgDataDeal();
                                //(item.Value as AreaCamParam).semaphoreSlimCam.Release();
                                string str = item.Key.ToString() + "信号量--" + (item.Value as AreaCamParam).semaphoreSlimCam.CurrentCount.ToString() + "图像数--" + item.Value.QueueGrabImage.Count.ToString();
                                //  Console.WriteLine(str);

                            }
                        }
                    });


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

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }

        }

        public void UpdateCraft()
        {
            stackEACraft = Project.Instance.craftBase as StackEACraft;
        }
        #endregion
    }
}

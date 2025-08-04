using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using AntdUI.Svg;

using Basler.Pylon;

using Extensions;

using Frame_Core;

using HalconDotNet;

using LogTool;

using NPOI.SS.Formula.Functions;
using NPOI.XSSF.Streaming.Values;

using ScottPlot.Statistics.Interpolation;

using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace Mef_VisionFrame
{
    public class BIZ_DataWorker_Narrow : IBizWorkerBase
    {
        BIZ_ThreadManage_Narrow bIZ_ThreadManage = new BIZ_ThreadManage_Narrow();
        public BIZ_Variable_Narrow BIZ_Variable = new BIZ_Variable_Narrow();
        Form_Narrow form;
        PFTCleanCraft pFTCleanCraft;

        public DisplayWindowClass HWC1;
        public DisplayWindowClass HWC2;
        public DisplayWindowClass HWC3;
        public DisplayWindowClass HWC4;
        public DisplayWindowClass HWC5;
        public DisplayWindowClass HWC6;
        public DisplayWindowClass HWC7;
        public DisplayWindowClass HWC8;

        public DisplayWindowClass DisplayWindowClass1;
        public DisplayWindowClass DisplayWindowClass2;
        public DisplayWindowClass DisplayWindowClass3;
        public DisplayWindowClass DisplayWindowClass4;
        public DisplayWindowClass DisplayWindowClass5;
        public DisplayWindowClass DisplayWindowClass6;


        CombinationHWindowControl CombinationHWindowControl_cam1;
        CombinationHWindowControl CombinationHWindowControl_cam2;

        //曲线图
        Dictionary<string, ChartUIParam> chartUIParams = new Dictionary<string, ChartUIParam>();

        //曲线图
        Dictionary<string, ChartUIParam> chartUIPage = new Dictionary<string, ChartUIParam>();

        IntervalExecution intervalExecution = new IntervalExecution();
        IntervalExecution intervalExecutionStas = new IntervalExecution();
        public object Prod_Lock = new object();

        public BIZ_DataWorker_Narrow(Form_Narrow _form)
        {
            form = _form;
            UpdateCraft();
            HWC1 = new DisplayWindowClass(form.HWC1, form.TB_PositionAndGray1, form.PB_FitWin1, CameraEnum.Cam1, form.HWC1.Name);
            //HWC2 = new DisplayWindowClass(form.HWC2, form.TB_PositionAndGray2, form.PB_FitWin2, CameraEnum.Cam2, form.HWC2.Name);
            HWC3 = new DisplayWindowClass(form.HWC3, form.TB_PositionAndGray3, form.PB_FitWin3, CameraEnum.Cam3, form.HWC3.Name);
            // HWC4 = new DisplayWindowClass(form.HWC4, form.TB_PositionAndGray4, form.PB_FitWin4, CameraEnum.Cam4, form.HWC4.Name);
            HWC5 = new DisplayWindowClass(form.HWC5, form.TB_PositionAndGray5, form.PB_FitWin5, CameraEnum.Cam5, form.HWC5.Name);
            HWC6 = new DisplayWindowClass(form.HWC6, form.TB_PositionAndGray6, form.PB_FitWin6, CameraEnum.Cam5, form.HWC6.Name);
            HWC7 = new DisplayWindowClass(form.HWC7, form.TB_PositionAndGray7, form.PB_FitWin7, CameraEnum.Cam5, form.HWC7.Name);
            HWC8 = new DisplayWindowClass(form.HWC8, form.TB_PositionAndGray8, form.PB_FitWin8, CameraEnum.Cam5, form.HWC8.Name);

            DisplayWindowClass1 = new DisplayWindowClass(form.hWindowControl1, "Cam1_MinImage1");
            DisplayWindowClass2 = new DisplayWindowClass(form.hWindowControl2, "Cam1_MinImage2");
            DisplayWindowClass3 = new DisplayWindowClass(form.hWindowControl3, "Cam1_MinImage3");
            DisplayWindowClass4 = new DisplayWindowClass(form.hWindowControl4, "Cam2_MinImage1");
            DisplayWindowClass5 = new DisplayWindowClass(form.hWindowControl5, "Cam2_MinImage2");
            DisplayWindowClass6 = new DisplayWindowClass(form.hWindowControl6, "Cam2_MinImage3");

            CombinationHWindowControl_cam1 = new CombinationHWindowControl(CameraEnum.Cam1.ToString(), new List<DisplayWindowClass>() { DisplayWindowClass1, DisplayWindowClass2, DisplayWindowClass3 });
            CombinationHWindowControl_cam2 = new CombinationHWindowControl(CameraEnum.Cam2.ToString(), new List<DisplayWindowClass>() { DisplayWindowClass4, DisplayWindowClass5, DisplayWindowClass6 });

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

            foreach(var item in Project.Instance.thick3DManages)
            {
                item.SendData -= send3DValue;
                item.SendData += send3DValue;
            }
            

            //  var thd =BIZ_General.GetAllControlNames(form);


        }


        public void ProcessingData_FrameCam1(ProductBase camEAProduct)
        {

            CameraEnum cameraEnum = CameraEnum.Cam1;
            double Calipix_x = Project.Instance.cameraManager[cameraEnum].m_CamParam.CalibrationValue_X;
            double Calipix_y = Project.Instance.cameraManager[cameraEnum].m_CamParam.CalibrationValue_Y;
            ConcurrentDictionary<int, PFTCleanEAProductInfo> dic_EAproductsInfo_A = Project.Instance.Is_simulation ? BIZ_Variable.dic_EAproductsInfo_SimA : BIZ_Variable.dic_EAproductsInfo_A;
            try
            {
                HOperatorSet.CountSeconds(out camEAProduct.Image_StartDataProcessTime);

                PFTImageProduct pFTImageProduct = camEAProduct as PFTImageProduct;
                CamDetectionVariable camDetectionVariable = Project.Instance.cameraManager[cameraEnum].m_CamDetectionVariable;
                //============================================================================
                //【1】工艺处理             

                ResultType bIsFrameSizeNg = ResultType.NG;

                ResultType bIsFrameDefectNg = ResultType.NG;


                //点检处理
                try
                {
                    SizeCheckPointParam sizeCheckPointParam = Project.Instance.cameraManager[cameraEnum].m_SizeCheckPointManage.sizeCheckPointParam;
                    
                    long frame = pFTImageProduct.nCurImageID;
                    double speed = 0;
                    if (sizeCheckPointParam.SSDJ)
                    {
                        speed = GetSpeedValue();
                    }
                   
                    if (PointCheck.RealTimePointCheckResetStartDJJudge(frame, speed, sizeCheckPointParam))
                    {

                        PointCheck.DJProcess(cameraEnum, pFTImageProduct.showHalconWindow.m_hImage, Calipix_x, Calipix_y, sizeCheckPointParam, out HObject preproccessRegion, out List<RectangleInfo> rectangles);

                        //添加预处理区域到显示窗口
                        if (preproccessRegion != null && preproccessRegion.IsInitialized())
                        {
                            pFTImageProduct.showHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = preproccessRegion, m_stuGetColor = HalconColor.red });
                        }

                        //显示结果
                        if (rectangles != null && rectangles.Count > 0)
                        {
                            for (int i = 0; i < rectangles.Count; i++)
                            {
                                string[] data = new string[]
                                {
                                      rectangles[i].widthReality.ToString("F2"),
                                      rectangles[i].widthResult?"OK":"NG",
                                      rectangles[i].heightReality.ToString("F2"),
                                      rectangles[i].heightResult?"OK":"NG",
                                      sizeCheckPointParam.SizeDJResult?"OK":"NG"
                                };
                                string[] dataTitle = PointCheck.GetPointCheckTitles();
                                FileOperationCSV.WriteData_CSV(Project.Instance.cameraManager[cameraEnum].m_SizeCheckPointManage.SizeRealTimeCheckPointPath, dataTitle, data, cameraEnum.ToString());

                                //添加至显示窗口
                                pFTImageProduct.showHalconWindow.m_lineShowInfos.Add(new LineInfo() { RowBegin = rectangles[i].leftEdgeBeginRow, ColBegin = rectangles[i].leftEdgeBeginCol, RowEnd = rectangles[i].leftEdgeEndRow, ColEnd = rectangles[i].leftEdgeEndCol, Color = HalconColor.green });
                                pFTImageProduct.showHalconWindow.m_lineShowInfos.Add(new LineInfo() { RowBegin = rectangles[i].topEdgeBeginRow, ColBegin = rectangles[i].topEdgeBeginCol, RowEnd = rectangles[i].topEdgeEndRow, ColEnd = rectangles[i].topEdgeEndCol, Color = HalconColor.green });
                                pFTImageProduct.showHalconWindow.m_lineShowInfos.Add(new LineInfo() { RowBegin = rectangles[i].rightEdgeBeginRow, ColBegin = rectangles[i].rightEdgeBeginCol, RowEnd = rectangles[i].rightEdgeEndRow, ColEnd = rectangles[i].rightEdgeEndCol, Color = HalconColor.green });
                                pFTImageProduct.showHalconWindow.m_lineShowInfos.Add(new LineInfo() { RowBegin = rectangles[i].bottomEdgeBeginRow, ColBegin = rectangles[i].bottomEdgeBeginCol, RowEnd = rectangles[i].bottomEdgeEndRow, ColEnd = rectangles[i].bottomEdgeEndCol, Color = HalconColor.green });
                            }
                        }

                    }

                }
                catch (Exception ee)
                {
                     MessageNotify.AddErrorLog($"PointCheck异常：{ee.Message}");
                }


                //界面数据实时显示
                //尺寸数据判断
                if (pFTImageProduct.ListImageSizes.Count > 0)
                {
                    foreach (var item in pFTImageProduct.ListImageSizes)
                    {
                        pFTImageProduct.showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo { m_strMessage = $"{item.SizeName}__{item.DataResult}", m_stuGetColor = item.resultType == ResultType.NG ? HalconColor.red : item.resultType == ResultType.OK ? HalconColor.green : HalconColor.orange });
                    }
                }
                foreach (var item in pFTImageProduct.ListImageSizes)
                {
                    if (pFTImageProduct.nCurEAID == 0)
                    {
                        item.resultType = ResultType.OK;
                    }
                    List<RowData> rowDatas = new List<RowData>();
                    rowDatas.Add(new RowData() { dataStr = DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss_fff"), resultType = ResultType.OK });
                    rowDatas.Add(new RowData() { dataStr = $"{pFTImageProduct.nCurImageID}", resultType = ResultType.OK });
                    rowDatas.Add(new RowData() { dataStr = $"{pFTImageProduct.nCurEAID.ToString()}", resultType = ResultType.OK });
                    rowDatas.Add(new RowData() { dataStr = $"{item.Description.ToString()}", resultType = item.resultType });
                    rowDatas.Add(new RowData() { dataStr = $"{item.resultType.ToString()}", resultType = item.resultType });
                    rowDatas.Add(new RowData() { dataStr = $"{item.DataResult.ToString("0.00")}", resultType = item.resultType });


                    UIService.Instance.AddAction(() =>
                    {
                        NListView.Add(form.listView2, rowDatas.ToArray(), true);

                    });


                    bIZ_ThreadManage.limitThreadPool.RunAction(() =>
                    {
                        FileOperationCSV.WriteData_CSV(camDetectionVariable.ProdSizeDataPath, BIZ_Variable.SizeProcessTitles.ToArray(), rowDatas.ToArray(), cameraEnum.ToString());

                    });


                }

                if (pFTImageProduct.ListImageSizes.Find(x => x.IsNG == true) == null || pFTImageProduct.nCurEAID == 0)
                {
                    bIsFrameSizeNg = ResultType.OK;
                }
                else if(pFTImageProduct.ListImageSizes.Count>0)
                {
                    bIsFrameSizeNg= pFTImageProduct.ListImageSizes.Max(x => x.resultType);
                }


             


                //缺项数据判断//帧只做NG判断
                ProcessImageDefects(pFTImageProduct.ListImageDefects, cameraEnum);

                if (pFTImageProduct.ListImageDefects.Count() == 0 || pFTImageProduct.nCurEAID == 0)
                {
                    bIsFrameDefectNg = ResultType.OK;
                }
                else if (pFTImageProduct.ListImageDefects.Count() > 0)
                {
                    //查找ListImageDefects返回resultType最大值
                    bIsFrameDefectNg = pFTImageProduct.ListImageDefects.Max(x => x.resultType);
                }


                ResultType maxResultTypeAll = new[] { bIsFrameSizeNg, bIsFrameDefectNg }
               .Max(result => result);

                //界面NG显示及表格保存
                pFTImageProduct.showHalconWindow.m_strCamName = Project.Instance.cameraManager[cameraEnum].m_CamParam.CameraName;
                pFTImageProduct.showHalconWindow.m_bIsNG = maxResultTypeAll == ResultType.NG;
                pFTImageProduct.showHalconWindow.m_messageShowInfos.Insert(0, new MessageShowInfo() { m_strMessage = maxResultTypeAll.ToString(), m_stuGetColor = maxResultTypeAll == ResultType.NG ? HalconColor.red : maxResultTypeAll == ResultType.OK ? HalconColor.green : HalconColor.orange });
                foreach (var item in pFTImageProduct.ListImageDefects)
                {
                    if (pFTImageProduct.nCurEAID == 0)
                    {
                        item.resultType = ResultType.OK;
                    }


                    List<RowData> rowDeftDatas = new List<RowData>();
                    rowDeftDatas.Add(new RowData() { dataStr = DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss_fff"), resultType = ResultType.OK });
                    rowDeftDatas.Add(new RowData() { dataStr = $"{item.m_nImgIndex}", resultType = ResultType.OK });
                    rowDeftDatas.Add(new RowData() { dataStr = $"{item.m_iProductIndex.ToString()}", resultType = ResultType.OK });
                    rowDeftDatas.Add(new RowData() { dataStr = $"{item.m_strNgTypeName.ToString()}", resultType = item.resultType });
                    rowDeftDatas.Add(new RowData() { dataStr = $"{item.resultType.ToString()}", resultType = item.resultType });
                    rowDeftDatas.Add(new RowData() { dataStr = $"面积：{item.m_fNgTypArea.ToString()}_高度：{item.m_fNgTypHeight.ToString()}_宽度：{item.m_fNgTypWidth.ToString()}", resultType = item.resultType });
                    UIService.Instance.AddAction(() =>
                    {

                        NListView.Add(form.listView1, rowDeftDatas.ToArray(), true);

                    });


                    bIZ_ThreadManage.limitThreadPool.RunAction(() =>
                    {
                        FileOperationCSV.WriteData_CSV(camDetectionVariable.ProdDeftDataPath, BIZ_Variable.DeftProcessTitles.ToArray(), rowDeftDatas.ToArray(), cameraEnum.ToString());

                    });
                }

                //窗体NG显示
                BIZ_General.AddMessageShowWindow(pFTImageProduct.ListImageDefects, pFTImageProduct.showHalconWindow);


                //EA数据处理//判断EA数据
                if (pFTImageProduct.blSplitEA)
                {
                    ResultType bIsEASizeNg = ResultType.NG;

                    ResultType bIsEADefectNg = ResultType.NG;


                    bool bIsA = dic_EAproductsInfo_A.TryGetValue(pFTImageProduct.nCurEAID, out PFTCleanEAProductInfo pFTCleanEAProductInfo);
                    if (!bIsA)
                    {
                        MessageNotify.AddErrorLog($"{cameraEnum}_产品EA({pFTImageProduct.nCurEAID})未找到_A{bIsA}");
                    }
                    //EA尺寸总结果判断
                    //if (pFTCleanEAProductInfo.ListEASizes.Find(x => x.resultType == ResultType.NG) == null || pFTImageProduct.nCurEAID == 0)
                    //{
                    //    bIsEASizeNg = ResultType.OK;

                    //}

                    //EA尺寸判断
                    foreach (var item in pFTCleanEAProductInfo.ListEASizes)
                    {
                        var size = pFTCleanEAProductInfo.ListEASizes.Find(x => x.Name == item.Name);
                        var sizeitem = pFTCleanCraft.ListEASizes.Find(x => x.Name == item.Name&&x.camera==CameraEnum.Cam1);
                        if (sizeitem != null)
                        {
                            GetSizeJudgeInfo(sizeitem, ref size);
                        }
                        else
                        {
                            MessageNotify.AddErrorLog($"相机1工艺尺寸{item.Description}未找到");
                        }
                    }

                    bIsEASizeNg = EvaluateEASizeStatus(pFTCleanEAProductInfo);

                    //缺项数据判断//帧只做NG判断
                    ProcessImageDefects(pFTCleanEAProductInfo.ListEADefects, cameraEnum, true);

                    bIsEADefectNg = EvaluateEADefectStatus(pFTCleanEAProductInfo);


                    List<RowData> strData = new List<RowData>();
                    strData.Add(new RowData() { dataStr = DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss_fff"), resultType = ResultType.OK });
                    strData.Add(new RowData() { dataStr = $"{pFTCleanEAProductInfo.EAID}", resultType = ResultType.OK });

                    foreach (var item in pFTCleanEAProductInfo.ListEASizes)
                    {
                        if (pFTImageProduct.nCurEAID == 0)
                        {
                            item.resultType = ResultType.OK;
                        }
                        strData.Add(new RowData() { dataStr = item.DataResult.ToString(), resultType = item.resultType });
                        strData.Add(new RowData() { dataStr = item.resultType.ToString(), resultType = item.resultType });

                        UIService.Instance.AddAction(() =>
                        {
                            ChartDataShow(chartUIParams[item.Name.ToString()], item.DataResult);
                            ChartDataShow(chartUIPage[item.Name.ToString()], item.DataResult);

                        });



                    }


                    strData.Add(new RowData() { dataStr = bIsEADefectNg.ToString(), resultType = bIsEADefectNg });

                    ResultType resultTypeAll = new[] { bIsEASizeNg, bIsEADefectNg }
                    .Max(result => result);
                
                    strData.Add(new RowData() { dataStr = resultTypeAll.ToString(), resultType = resultTypeAll });

                    List<string> dftinfo = new List<string>();
                    foreach (var item in pFTCleanEAProductInfo.ListEADefects)
                    {
                        if (pFTImageProduct.nCurEAID == 0)
                        {
                            item.resultType = ResultType.OK;
                        }
                        dftinfo.Add(item.m_strNgTypeName.ToString());
                    }

                    string strInfo = string.Join("_", dftinfo);

                    strData.Add(new RowData() { dataStr = strInfo, resultType = bIsEADefectNg });

                    UIService.Instance.AddAction(() =>
                    {

                        NListView.Add(form.ListView_A, strData.ToArray(), true);
                        NListView.Add(form.ListView_AA, strData.ToArray(), true);

                    });

                    bIZ_ThreadManage.limitThreadPool.RunAction(() =>
                    {
                        FileOperationCSV.WriteData_CSV(camDetectionVariable.ProdEADataPath, BIZ_Variable.EATitles_A.ToArray(), strData.ToArray(), cameraEnum.ToString());

                    });
                    if (pFTImageProduct.nCurEAID != 0)
                    {
                        //尺寸报警停机判断
                        TJBJSizeInfo_WriteState(cameraEnum, pFTCleanEAProductInfo.ListEASizes, BIZ_Variable.ListEASizes, pFTCleanCraft.ListEASizes);

                        //缺项报警停机判断
                        TJBJDeftInfo_WriteState(cameraEnum, pFTCleanEAProductInfo.ListEADefects, BIZ_Variable.ListDefectInfos, pFTCleanCraft.ListDefectInfos);

                        ProductDataSizeStastic(pFTCleanEAProductInfo.ListEASizes, BIZ_Variable.sizeItemInfos_A);

                        ProductDataDefectStastic(pFTCleanEAProductInfo.ListEADefects, BIZ_Variable.defectItemInfos_A);

                    }

                }




                //【2】图像显示
                PrimitiveTransition(cameraEnum, pFTImageProduct.showHalconWindow, out Operation_ImageInfo operation_ImageInfo);
                HWC1.operation_Image.imageInfosShow.Enqueue(operation_ImageInfo);



                BIZ_Variable.queueImgDispose_A.Enqueue(pFTImageProduct);
                //if (BIZ_Variable.queueImgDispose_A.Count > 20)
                //{
                //    BIZ_Variable.queueImgDispose_A.Dequeue()?.Dispose();

                //}

                HOperatorSet.CountSeconds(out pFTImageProduct.Image_DataProcessTime);
                HOperatorSet.CountSeconds(out pFTImageProduct.Image_EndTime);
                //计算时间
                BIZ_General.CalculateProcessingTime(pFTImageProduct);



            }
            catch (Exception ex)
            {
                MessageNotify.AddErrorLog("相机1数据处理异常：" + ex.ToString());
            }
        }

        public void ProcessingData_FrameCam2(ProductBase camEAProduct)
        {

            CameraEnum cameraEnum = CameraEnum.Cam2;
            double Calipix_x = Project.Instance.cameraManager[cameraEnum].m_CamParam.CalibrationValue_X;
            double Calipix_y = Project.Instance.cameraManager[cameraEnum].m_CamParam.CalibrationValue_Y;
            ConcurrentDictionary<int, PFTCleanEAProductInfo> dic_EAproductsInfo_A = Project.Instance.Is_simulation ? BIZ_Variable.dic_EAproductsInfo_SimA : BIZ_Variable.dic_EAproductsInfo_A;
            ConcurrentDictionary<int, PFTCleanEAProductInfo> dic_EAproductsInfo_B = Project.Instance.Is_simulation ? BIZ_Variable.dic_EAproductsInfo_SimB : BIZ_Variable.dic_EAproductsInfo_B;


            try
            {
                HOperatorSet.CountSeconds(out camEAProduct.Image_StartDataProcessTime);

                PFTImageProduct pFTImageProduct = camEAProduct as PFTImageProduct;
                CamDetectionVariable camDetectionVariable = Project.Instance.cameraManager[cameraEnum].m_CamDetectionVariable;
                //============================================================================
                //【1】工艺处理             

                ResultType bIsFrameSizeNg = ResultType.NG;

                ResultType bIsFrameDefectNg = ResultType.NG;

                //点检处理
                try
                {
                    SizeCheckPointParam sizeCheckPointParam = Project.Instance.cameraManager[cameraEnum].m_SizeCheckPointManage.sizeCheckPointParam;
                    long frame = pFTImageProduct.nCurImageID;
                    double speed = 0;
                    if (sizeCheckPointParam.SSDJ)
                    {
                        speed = GetSpeedValue();
                    }
                    if (PointCheck.RealTimePointCheckResetStartDJJudge(frame, speed, sizeCheckPointParam))
                    {

                        PointCheck.DJProcess(cameraEnum, pFTImageProduct.showHalconWindow.m_hImage, Calipix_x, Calipix_y, sizeCheckPointParam, out HObject preproccessRegion, out List<RectangleInfo> rectangles);

                        //添加预处理区域到显示窗口
                        if (preproccessRegion != null && preproccessRegion.IsInitialized())
                        {
                            pFTImageProduct.showHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = preproccessRegion, m_stuGetColor = HalconColor.red });
                        }

                        //显示结果
                        if (rectangles != null && rectangles.Count > 0)
                        {
                            for (int i = 0; i < rectangles.Count; i++)
                            {
                                string[] data = new string[]
                                {
                                      rectangles[i].widthReality.ToString("F2"),
                                      rectangles[i].widthResult?"OK":"NG",
                                      rectangles[i].heightReality.ToString("F2"),
                                      rectangles[i].heightResult?"OK":"NG",
                                      sizeCheckPointParam.SizeDJResult?"OK":"NG"
                                };
                                string[] dataTitle = PointCheck.GetPointCheckTitles();
                                FileOperationCSV.WriteData_CSV(Project.Instance.cameraManager[cameraEnum].m_SizeCheckPointManage.SizeRealTimeCheckPointPath, dataTitle, data, cameraEnum.ToString());

                                //添加至显示窗口
                                pFTImageProduct.showHalconWindow.m_lineShowInfos.Add(new LineInfo() { RowBegin = rectangles[i].leftEdgeBeginRow, ColBegin = rectangles[i].leftEdgeBeginCol, RowEnd = rectangles[i].leftEdgeEndRow, ColEnd = rectangles[i].leftEdgeEndCol, Color = HalconColor.green });
                                pFTImageProduct.showHalconWindow.m_lineShowInfos.Add(new LineInfo() { RowBegin = rectangles[i].topEdgeBeginRow, ColBegin = rectangles[i].topEdgeBeginCol, RowEnd = rectangles[i].topEdgeEndRow, ColEnd = rectangles[i].topEdgeEndCol, Color = HalconColor.green });
                                pFTImageProduct.showHalconWindow.m_lineShowInfos.Add(new LineInfo() { RowBegin = rectangles[i].rightEdgeBeginRow, ColBegin = rectangles[i].rightEdgeBeginCol, RowEnd = rectangles[i].rightEdgeEndRow, ColEnd = rectangles[i].rightEdgeEndCol, Color = HalconColor.green });
                                pFTImageProduct.showHalconWindow.m_lineShowInfos.Add(new LineInfo() { RowBegin = rectangles[i].bottomEdgeBeginRow, ColBegin = rectangles[i].bottomEdgeBeginCol, RowEnd = rectangles[i].bottomEdgeEndRow, ColEnd = rectangles[i].bottomEdgeEndCol, Color = HalconColor.green });
                            }
                        }

                    }

                }
                catch (Exception ee)
                {
                    MessageNotify.AddErrorLog($"PointCheck异常：{ee.Message}");
                }

                //尺寸数据判断
                if (pFTImageProduct.ListImageSizes.Count > 0)
                {
                    foreach (var item in pFTImageProduct.ListImageSizes)
                    {
                        pFTImageProduct.showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo { m_strMessage = $"{item.SizeName}__{item.DataResult}", m_stuGetColor = item.resultType == ResultType.NG ? HalconColor.red : item.resultType == ResultType.OK ? HalconColor.green : HalconColor.orange });
                    }
                }

                foreach (var item in pFTImageProduct.ListImageSizes)
                {
                    if (pFTImageProduct.nCurEAID == 0)
                    {
                        item.resultType = ResultType.OK;
                    }
                    List<RowData> rowDatas = new List<RowData>();
                    rowDatas.Add(new RowData() { dataStr = DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss_fff"), resultType = ResultType.OK });
                    rowDatas.Add(new RowData() { dataStr = $"{pFTImageProduct.nCurImageID}", resultType = ResultType.OK });
                    rowDatas.Add(new RowData() { dataStr = $"{pFTImageProduct.nCurEAID.ToString()}", resultType = ResultType.OK });
                    rowDatas.Add(new RowData() { dataStr = $"{item.Description.ToString()}", resultType = item.resultType });
                    rowDatas.Add(new RowData() { dataStr = $"{item.resultType.ToString()}", resultType = item.resultType });
                    rowDatas.Add(new RowData() { dataStr = $"{item.DataResult.ToString("0.00")}", resultType = item.resultType });

                    UIService.Instance.AddAction(() =>
                    {
                        NListView.Add(form.listView4, rowDatas.ToArray(), true);

                    });


                    bIZ_ThreadManage.limitThreadPool.RunAction(() =>
                    {
                        FileOperationCSV.WriteData_CSV(camDetectionVariable.ProdSizeDataPath, BIZ_Variable.SizeProcessTitles.ToArray(), rowDatas.ToArray(), cameraEnum.ToString());

                    });


                }


                if (pFTImageProduct.ListImageSizes.Find(x => x.IsNG == true) == null || pFTImageProduct.nCurEAID == 0)
                {
                    bIsFrameSizeNg = ResultType.OK;
                }
                else if (pFTImageProduct.ListImageSizes.Count > 0)
                {
                    bIsFrameSizeNg = pFTImageProduct.ListImageSizes.Max(x => x.resultType);
                }


                //缺项数据判断//帧只做NG判断
                ProcessImageDefects(pFTImageProduct.ListImageDefects, cameraEnum);

                if (pFTImageProduct.ListImageDefects.Count() == 0 || pFTImageProduct.nCurEAID == 0)
                {
                    bIsFrameDefectNg = ResultType.OK;
                }
                else if(pFTImageProduct.ListImageDefects.Count() > 0)
                {
                    //查找ListImageDefects返回resultType最大值
                    bIsFrameDefectNg = pFTImageProduct.ListImageDefects.Max(x => x.resultType);
                }

                ResultType maxResultTypeAll = new[] { bIsFrameSizeNg, bIsFrameDefectNg }
               .Max(result => result);

                //界面NG显示及表格保存
                pFTImageProduct.showHalconWindow.m_strCamName = Project.Instance.cameraManager[cameraEnum].m_CamParam.CameraName;
                pFTImageProduct.showHalconWindow.m_bIsNG = maxResultTypeAll == ResultType.NG; ;
                pFTImageProduct.showHalconWindow.m_messageShowInfos.Insert(0, new MessageShowInfo() { m_strMessage = maxResultTypeAll.ToString(), m_stuGetColor = maxResultTypeAll == ResultType.NG ? HalconColor.red : maxResultTypeAll == ResultType.OK ? HalconColor.green : HalconColor.orange });

                foreach (var item in pFTImageProduct.ListImageDefects)
                {
                    if (pFTImageProduct.nCurEAID == 0)
                    {
                        item.resultType = ResultType.OK;
                    }

                    List<RowData> rowDeftDatas = new List<RowData>();
                    rowDeftDatas.Add(new RowData() { dataStr = DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss_fff"), resultType = ResultType.OK });
                    rowDeftDatas.Add(new RowData() { dataStr = $"{item.m_nImgIndex}", resultType = ResultType.OK });
                    rowDeftDatas.Add(new RowData() { dataStr = $"{item.m_iProductIndex.ToString()}", resultType = ResultType.OK });
                    rowDeftDatas.Add(new RowData() { dataStr = $"{item.m_strNgTypeName.ToString()}", resultType = item.resultType });
                    rowDeftDatas.Add(new RowData() { dataStr = $"{item.resultType.ToString()}", resultType = item.resultType });
                    rowDeftDatas.Add(new RowData() { dataStr = $"面积：{item.m_fNgTypArea.ToString()}_高度：{item.m_fNgTypHeight.ToString()}_宽度：{item.m_fNgTypWidth.ToString()}", resultType = item.resultType });
                    UIService.Instance.AddAction(() =>
                    {
                        NListView.Add(form.listView3, rowDeftDatas.ToArray(), true);

                    });


                    bIZ_ThreadManage.limitThreadPool.RunAction(() =>
                    {
                        FileOperationCSV.WriteData_CSV(camDetectionVariable.ProdDeftDataPath, BIZ_Variable.DeftProcessTitles.ToArray(), rowDeftDatas.ToArray(), cameraEnum.ToString());

                    });
                }




                //窗体NG显示
                BIZ_General.AddMessageShowWindow(pFTImageProduct.ListImageDefects, pFTImageProduct.showHalconWindow);


                //EA数据处理//判断EA数据
                if (pFTImageProduct.blSplitEA)
                {
                    ResultType bIsEASizeNg = ResultType.NG;

                    ResultType bIsEADefectNg = ResultType.NG;


                    bool bIsA = dic_EAproductsInfo_A.TryGetValue(pFTImageProduct.nCurEAID, out PFTCleanEAProductInfo pFTCleanEAProductInfo_A);
                    bool bIsB = dic_EAproductsInfo_B.TryGetValue(pFTImageProduct.nCurEAID, out PFTCleanEAProductInfo pFTCleanEAProductInfo_B);

                    if (!bIsA && !bIsB)
                    {
                        MessageNotify.AddErrorLog($"{cameraEnum}_产品EA({pFTImageProduct.nCurEAID})未找到_A{bIsA}_B{bIsB}");
                    }


                    //EA尺寸总结果判断
                    //if (pFTCleanEAProductInfo_B.ListEASizes.Find(x => x.resultType == ResultType.NG) == null || pFTImageProduct.nCurEAID == 0)
                    //{
                    //    bIsEASizeNg = ResultType.OK;
                    //}

                    //EA尺寸判断
                    foreach (var item in pFTCleanEAProductInfo_B.ListEASizes)
                    {
                        var size = pFTCleanEAProductInfo_B.ListEASizes.Find(x => x.Name == item.Name);
                        var sizeitem = pFTCleanCraft.ListEASizes.Find(x => x.Name == item.Name && x.camera == CameraEnum.Cam2);
                        if (sizeitem != null)
                        {
                            GetSizeJudgeInfo(sizeitem, ref size);
                        }
                        else
                        {
                            MessageNotify.AddErrorLog($"相机2工艺尺寸{item.Name}未找到");
                        }
                    }

                    bIsEASizeNg = EvaluateEASizeStatus(pFTCleanEAProductInfo_B);
                    //缺项数据判断//帧只做NG判断
                    ProcessImageDefects(pFTCleanEAProductInfo_B.ListEADefects, cameraEnum, true);

                    bIsEADefectNg = EvaluateEADefectStatus(pFTCleanEAProductInfo_B);


                    List<RowData> strData = new List<RowData>();
                    strData.Add(new RowData() { dataStr = DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss_fff"), resultType = ResultType.OK });
                    strData.Add(new RowData() { dataStr = $"{pFTCleanEAProductInfo_B.EAID}", resultType = ResultType.OK });

                    foreach (var item in pFTCleanEAProductInfo_B.ListEASizes)
                    {
                        if (pFTImageProduct.nCurEAID == 0)
                        {
                            item.resultType = ResultType.OK;
                        }
                        strData.Add(new RowData() { dataStr = item.DataResult.ToString(), resultType = item.resultType });
                        strData.Add(new RowData() { dataStr = item.resultType.ToString(), resultType = item.resultType });

                        UIService.Instance.AddAction(() =>
                        {
                            ChartDataShow(chartUIParams[item.Name.ToString()], item.DataResult);
                            ChartDataShow(chartUIPage[item.Name.ToString()], item.DataResult);
                        });

                    }

                    strData.Add(new RowData() { dataStr = bIsEADefectNg.ToString(), resultType = bIsEADefectNg });

                    ResultType resultTypeAll = new[] { bIsEASizeNg, bIsEADefectNg }
                    .Max(result => result); ;

                    strData.Add(new RowData() { dataStr = resultTypeAll.ToString(), resultType = resultTypeAll });

                    List<string> dftinfo = new List<string>();
                    foreach (var item in pFTCleanEAProductInfo_B.ListEADefects)
                    {
                        if (pFTImageProduct.nCurEAID == 0)
                        {
                            item.resultType = ResultType.OK;
                        }
                        dftinfo.Add(item.m_strNgTypeName.ToString());
                    }

                    string strInfo = string.Join("_", dftinfo);

                    strData.Add(new RowData() { dataStr = strInfo, resultType = bIsEADefectNg });

                    UIService.Instance.AddAction(() =>
                    {

                        NListView.Add(form.ListView_B, strData.ToArray(), true);
                        NListView.Add(form.ListView_BB, strData.ToArray(), true);

                    });

                    bIZ_ThreadManage.limitThreadPool.RunAction(() =>
                    {
                        FileOperationCSV.WriteData_CSV(camDetectionVariable.ProdEADataPath, BIZ_Variable.EATitles_B.ToArray(), strData.ToArray(), cameraEnum.ToString());

                    });


                    if (pFTImageProduct.nCurEAID != 0)
                    {
                        //尺寸报警停机判断
                        TJBJSizeInfo_WriteState(cameraEnum, pFTCleanEAProductInfo_B.ListEASizes, BIZ_Variable.ListEASizes, pFTCleanCraft.ListEASizes);

                        //缺项报警停机判断
                        TJBJDeftInfo_WriteState(cameraEnum, pFTCleanEAProductInfo_B.ListEADefects, BIZ_Variable.ListDefectInfos, pFTCleanCraft.ListDefectInfos);

                        ProductDataSizeStastic(pFTCleanEAProductInfo_B.ListEASizes, BIZ_Variable.sizeItemInfos_B);

                        ProductDataDefectStastic(pFTCleanEAProductInfo_B.ListEADefects, BIZ_Variable.defectItemInfos_B);
                    }

                    //综合AB结果判断，触发打标

                    bool bIsDB = false;

                    bIsDB = ((pFTCleanEAProductInfo_A.ListEASizes.Find(x => x.resultType == ResultType.NG) != null) ||
                        (pFTCleanEAProductInfo_A.ListEADefects.Find(x => x.resultType == ResultType.NG) != null) ||
                        (pFTCleanEAProductInfo_B.ListEASizes.Find(x => x.resultType == ResultType.NG) != null) ||
                        (pFTCleanEAProductInfo_B.ListEADefects.Find(x => x.resultType == ResultType.NG) != null)) && pFTImageProduct.nCurEAID != 0;

                    bool bIsDB_Deft = ((pFTCleanEAProductInfo_A.ListEADefects.Find(x => x.resultType == ResultType.NG) != null) ||
                                  (pFTCleanEAProductInfo_B.ListEADefects.Find(x => x.resultType == ResultType.NG) != null)) && pFTImageProduct.nCurEAID != 0;

                    ResultType resultTypeAll_AB_DB = bIsDB ? ResultType.NG : (((pFTCleanEAProductInfo_A.ListEASizes.Find(x => x.resultType == ResultType.Warn) != null) ||
                        (pFTCleanEAProductInfo_A.ListEADefects.Find(x => x.resultType == ResultType.Warn) != null) ||
                        (pFTCleanEAProductInfo_B.ListEASizes.Find(x => x.resultType == ResultType.Warn) != null) ||
                        (pFTCleanEAProductInfo_B.ListEADefects.Find(x => x.resultType == ResultType.Warn) != null)) && pFTImageProduct.nCurEAID != 0) ? ResultType.Warn : ResultType.OK;
                    ResultType resultTypeAll_AB_Deft = bIsDB_Deft ? ResultType.NG : ((pFTCleanEAProductInfo_A.ListEADefects.Find(x => x.resultType == ResultType.Warn) != null) || (pFTCleanEAProductInfo_B.ListEADefects.Find(x => x.resultType == ResultType.Warn) != null) && pFTImageProduct.nCurEAID != 0) ? ResultType.Warn : ResultType.OK;

                    if (bIsDB)
                    {
                        HOperatorSet.GetImageSize(pFTImageProduct.showHalconWindow.m_hImage, out HTuple w, out HTuple h);
                        int pos = Math.Abs(h.I - (int)pFTCleanEAProductInfo_A.EArange.StarImageRow);
                        List<string> posList = new List<string>();
                        posList.Add(pos.ToString("0.00"));
                        bIZ_ThreadManage.limitThreadPool.RunAction(() =>
                        {
                            Project.Instance.dbjCardManager.SendDBInfo(pFTCleanEAProductInfo_A.EArange.StartImageID, posList.ToArray());
                        });
                        // Project.Instance.dbjCardManager.SendDBInfo(pFTCleanEAProductInfo_A.EArange.StartImageID, posList.ToArray());
                        MessageNotify.AddCommLog($"产品EA({pFTImageProduct.nCurEAID})_帧号:{pFTCleanEAProductInfo_A.EArange.StartImageID}_位置:{pFTCleanEAProductInfo_A.EArange.StarImageRow}_NG,触发打标");
                    }
                    else
                    {

                        List<string> posList = new List<string>();
                        posList.Add("-111");
                        bIZ_ThreadManage.limitThreadPool.RunAction(() =>
                        {
                            Project.Instance.dbjCardManager.SendDBInfo(pFTCleanEAProductInfo_A.EArange.StartImageID, posList.ToArray());
                        });

                        MessageNotify.AddCommLog($"产品EA({pFTImageProduct.nCurEAID})_帧号:{pFTCleanEAProductInfo_A.EArange.StartImageID}_位置:{pFTCleanEAProductInfo_A.EArange.StarImageRow}_OK,发送打标位置信息");
                    }



                    //数据统计
                    StatisticalData statisticalData = camDetectionVariable.statisticalData;
                    if (bIsDB) statisticalData.NGCount++;

                    BIZ_General.ProdStatisticsCalculate(statisticalData, pFTImageProduct.nCurEAID + 1, statisticalData.NGCount);

                    UIService.Instance.AddAction(() =>
                    {
                        NLabel.UpdateControls(form.lb_SumupCount_Line, statisticalData.SumupCount.ToString(), true);
                        NLabel.UpdateControls(form.lb_OKCount_Line, statisticalData.OKCount.ToString(), true);
                        NLabel.UpdateControls(form.lb_NGCount_Line, statisticalData.NGCount.ToString(), true);
                        NLabel.UpdateControls(form.lb_DBCount_lineDect, statisticalData.NGCount.ToString());
                        NLabel.UpdateControls(form.lb_Yield_Line, statisticalData.Yield.ToString("0.00") + "%", true);

                    });



                    //EA数据保存

                    List<RowData> strEAData_AB = new List<RowData>();
                    strEAData_AB.Add(new RowData() { dataStr = DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss_fff"), resultType = ResultType.OK });
                    strEAData_AB.Add(new RowData() { dataStr = $"{pFTCleanEAProductInfo_B.EAID}", resultType = ResultType.OK });

                    List<SizeInfo> strEAData_AB_Size = new List<SizeInfo>();

                    strEAData_AB_Size.AddRange(pFTCleanEAProductInfo_A.ListEASizes);
                    strEAData_AB_Size.AddRange(pFTCleanEAProductInfo_B.ListEASizes);


                    foreach (var item in pFTCleanCraft.ListEASizes)
                    {
                        if (!item.Enable)
                        {
                            continue;
                        }
                        if((int)item.camera>1)
                        {
                            continue;
                        }
                        var sizeInfo = strEAData_AB_Size.Find(x => x.Name == item.Name);

                        if (sizeInfo == null)
                        {
                            sizeInfo = new SizeInfo() { Description = item.Description, resultType = ResultType.Warn };
                            //报警
                            MessageNotify.AddErrorLog($"产品EA({pFTImageProduct.nCurEAID})尺寸{item.Description}未从列表中找出");
                        }
                        strEAData_AB.Add(new RowData() { dataStr = sizeInfo.DataResult.ToString(), resultType = sizeInfo.resultType });
                        strEAData_AB.Add(new RowData() { dataStr = sizeInfo.resultType.ToString(), resultType = sizeInfo.resultType });

                    }
                    strEAData_AB.Add(new RowData() { dataStr = resultTypeAll_AB_Deft.ToString(), resultType = resultTypeAll_AB_Deft });
                    strEAData_AB.Add(new RowData() { dataStr = resultTypeAll_AB_DB.ToString(), resultType = resultTypeAll_AB_DB });
                    List<string> dftinfo_AB = new List<string>();
                    dftinfo_AB.Add("相机A：");
                    foreach (var item in pFTCleanEAProductInfo_A.ListEADefects)
                    {
                        if (pFTImageProduct.nCurEAID == 0)
                        {
                            item.resultType = ResultType.OK;
                        }
                        dftinfo_AB.Add(item.m_strNgTypeName.ToString());
                    }
                    dftinfo_AB.Add("相机B：");
                    foreach (var item in pFTCleanEAProductInfo_B.ListEADefects)
                    {
                        if (pFTImageProduct.nCurEAID == 0)
                        {
                            item.resultType = ResultType.OK;
                        }
                        dftinfo_AB.Add(item.m_strNgTypeName.ToString());
                    }

                    string strInfo_AB = string.Join("_", dftinfo_AB);

                    strEAData_AB.Add(new RowData() { dataStr = strInfo_AB, resultType = resultTypeAll_AB_Deft });

                    UIService.Instance.AddAction(() =>
                    {

                        NListView.Add(form.ListView_AB, strEAData_AB.ToArray(), true);
                        // NListView.Add(form.ListView_AABB, strEAData_AB.ToArray(), true);

                    });

                    bIZ_ThreadManage.limitThreadPool.RunAction(() =>
                    {
                        FileOperationCSV.WriteData_CSV(Project.Instance.paramPathSet.PFTProdEADataPath_AB, BIZ_Variable.EATitles_AB.ToArray(), strEAData_AB.ToArray(), cameraEnum.ToString());

                    });


                    //EA数据挪入释放队列

                    // //延迟2个EA再放入释放队列
                    // //获取字典中key的最大值

                    int maxKey = dic_EAproductsInfo_B.Keys.Max();
                    int minKey = dic_EAproductsInfo_B.Keys.Min();
                    if (Math.Abs(maxKey - minKey) >1)
                    {
                        dic_EAproductsInfo_A.TryRemove(minKey, out pFTCleanEAProductInfo_A);
                        dic_EAproductsInfo_B.TryRemove(minKey, out pFTCleanEAProductInfo_B);
                        BIZ_Variable.queueProductsDispose_A.Enqueue(pFTCleanEAProductInfo_A);
                        BIZ_Variable.queueProductsDispose_B.Enqueue(pFTCleanEAProductInfo_B);
                    }

                    //BIZ_Variable.queueProductsDispose_A.Enqueue(pFTCleanEAProductInfo_A);
                    //BIZ_Variable.queueProductsDispose_B.Enqueue(pFTCleanEAProductInfo_B);


                    //释放内存
                    if (BIZ_Variable.queueProductsDispose_A.Count > 0)
                    {
                        BIZ_Variable.queueProductsDispose_A.Dequeue()?.Dispose();

                    }

                    //释放内存
                    if (BIZ_Variable.queueProductsDispose_B.Count > 0)
                    {
                        BIZ_Variable.queueProductsDispose_B.Dequeue()?.Dispose();
                    }

                    //数据统计汇总表
                    StasticShowAndSave();
                }

                //============================================================================
                //【2】图像显示
                PrimitiveTransition(cameraEnum, pFTImageProduct.showHalconWindow, out Operation_ImageInfo operation_ImageInfo);
                HWC3.operation_Image.imageInfosShow.Enqueue(operation_ImageInfo);

                //BIZ_Variable.queueImgDispose_B.Enqueue(pFTImageProduct);

                //if (BIZ_Variable.queueImgDispose_B.Count > 20)
                //{
                //    BIZ_Variable.queueImgDispose_B.Dequeue()?.Dispose();

                //}


                HOperatorSet.CountSeconds(out pFTImageProduct.Image_DataProcessTime);
                HOperatorSet.CountSeconds(out pFTImageProduct.Image_EndTime);
                //计算时间
                BIZ_General.CalculateProcessingTime(pFTImageProduct);

            }
            catch (Exception ex)
            {
                MessageNotify.AddErrorLog("相机2数据处理异常：" + ex.ToString());
            }
        }

        public void ProcessingData_EACam1(ProductBase camEAProduct)
        {
            CameraEnum cameraEnum = CameraEnum.Cam1;
            try
            {
                HOperatorSet.CountSeconds(out camEAProduct.Image_StartDataProcessTime);

                PFTCleanEAProductInfo pFTCleanEAProductInfo = camEAProduct as PFTCleanEAProductInfo;
                CamDetectionVariable camDetectionVariable = Project.Instance.cameraManager[cameraEnum].m_CamDetectionVariable;
                //EA数据处理//判断EA数据

                ResultType bIsEASizeNg = ResultType.NG;

                ResultType bIsEADefectNg = ResultType.NG;

                if (pFTCleanEAProductInfo.ListEASizes.Count > 0)
                {
                    foreach (var item in pFTCleanEAProductInfo.ListEASizes)
                    {
                        camEAProduct.showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo { m_strMessage = $"{item.SizeName}__{item.DataResult}", m_stuGetColor = item.resultType == ResultType.NG ? HalconColor.red : item.resultType == ResultType.OK ? HalconColor.green : HalconColor.orange });
                    }
                }

                //EA尺寸总结果判断
                //if (pFTCleanEAProductInfo.ListEASizes.Find(x => x.resultType == ResultType.NG) == null || pFTCleanEAProductInfo.EAID == 0)
                //{
                //    bIsEASizeNg = ResultType.OK;
                //}
                bIsEASizeNg = EvaluateEASizeStatus(pFTCleanEAProductInfo);

                bIsEADefectNg = EvaluateEADefectStatus(pFTCleanEAProductInfo);

                ResultType maxResultTypeAll = new[] { bIsEASizeNg, bIsEADefectNg }
                .Max(result => result);

                BIZ_General.AddMessageShowWindow(pFTCleanEAProductInfo.ListEADefects, pFTCleanEAProductInfo.showHalconWindow);
                foreach (var item in pFTCleanEAProductInfo.ListEAlines)
                {
                    pFTCleanEAProductInfo.showHalconWindow.m_lineShowInfos.Add(item);
                }
               
            

                pFTCleanEAProductInfo.showHalconWindow.m_nMode = 1;
                pFTCleanEAProductInfo.showHalconWindow.m_strCamName = Project.Instance.cameraManager[cameraEnum].m_CamParam.CameraName;
                pFTCleanEAProductInfo.showHalconWindow.m_bIsNG = maxResultTypeAll == (ResultType.NG);
                pFTCleanEAProductInfo.showHalconWindow.m_messageShowInfos.Insert(0, new MessageShowInfo() { m_strMessage = maxResultTypeAll.ToString(), m_stuGetColor = maxResultTypeAll == ResultType.NG ? HalconColor.red : maxResultTypeAll == ResultType.OK ? HalconColor.green : HalconColor.orange });
                PrimitiveTransitionEA(cameraEnum, pFTCleanEAProductInfo.showHalconWindow, out Operation_ImageInfo operation_ImageInfo);
                HWC7.operation_Image.imageInfosShow.Enqueue(operation_ImageInfo);
                HOperatorSet.CountSeconds(out pFTCleanEAProductInfo.Image_DataProcessTime);
                HOperatorSet.CountSeconds(out pFTCleanEAProductInfo.Image_EndTime);
                //计算时间
                BIZ_General.CalculateProcessingTime(pFTCleanEAProductInfo);

            }
            catch (Exception ex)
            {
                MessageNotify.AddErrorLog("Cam1EA数据处理异常：" + ex.ToString());
            }
        }

        public void ProcessingData_EACam2(ProductBase camEAProduct)
        {

            CameraEnum cameraEnum = CameraEnum.Cam2;

            try
            {
                HOperatorSet.CountSeconds(out camEAProduct.Image_StartDataProcessTime);

                PFTCleanEAProductInfo pFTCleanEAProductInfo = camEAProduct as PFTCleanEAProductInfo;
                CamDetectionVariable camDetectionVariable = Project.Instance.cameraManager[cameraEnum].m_CamDetectionVariable;
                //EA数据处理//判断EA数据


                ResultType bIsEASizeNg = ResultType.NG;

                ResultType bIsEADefectNg = ResultType.NG;

                if (pFTCleanEAProductInfo.ListEASizes.Count > 0)
                {
                    foreach (var item in pFTCleanEAProductInfo.ListEASizes)
                    {
                        camEAProduct.showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo { m_strMessage = $"{item.SizeName}__{item.DataResult}", m_stuGetColor = item.resultType == ResultType.NG ? HalconColor.red : item.resultType == ResultType.OK ? HalconColor.green : HalconColor.orange });
                    }
                }

                //EA尺寸总结果判断
                //if (pFTCleanEAProductInfo.ListEASizes.Find(x => x.resultType == ResultType.NG) == null || pFTCleanEAProductInfo.EAID == 0)
                //{
                //    bIsEASizeNg = ResultType.OK;
                //}

                bIsEASizeNg = EvaluateEASizeStatus(pFTCleanEAProductInfo);

                //if (pFTCleanEAProductInfo.ListEADefects.Count() == 0 || pFTCleanEAProductInfo.EAID == 0)
                //{
                //    bIsEADefectNg = ResultType.OK;
                //}

                bIsEADefectNg = EvaluateEADefectStatus(pFTCleanEAProductInfo);



                ResultType maxResultTypeAll = new[] { bIsEASizeNg, bIsEADefectNg }
                     .Max(result => result);


                BIZ_General.AddMessageShowWindow(pFTCleanEAProductInfo.ListEADefects, pFTCleanEAProductInfo.showHalconWindow);
                foreach (var item in pFTCleanEAProductInfo.ListEAlines)
                {
                    pFTCleanEAProductInfo.showHalconWindow.m_lineShowInfos.Add(item);
                }




                pFTCleanEAProductInfo.showHalconWindow.m_nMode = 1;
                pFTCleanEAProductInfo.showHalconWindow.m_strCamName = Project.Instance.cameraManager[cameraEnum].m_CamParam.CameraName;
                pFTCleanEAProductInfo.showHalconWindow.m_bIsNG = maxResultTypeAll == (ResultType.NG);
                pFTCleanEAProductInfo.showHalconWindow.m_messageShowInfos.Insert(0, new MessageShowInfo() { m_strMessage = maxResultTypeAll.ToString(), m_stuGetColor = maxResultTypeAll == ResultType.NG ? HalconColor.red : maxResultTypeAll == ResultType.OK ? HalconColor.green : HalconColor.orange });


                PrimitiveTransitionEA(cameraEnum, pFTCleanEAProductInfo.showHalconWindow, out Operation_ImageInfo operation_ImageInfo);
                HWC8.operation_Image.imageInfosShow.Enqueue(operation_ImageInfo);
                HOperatorSet.CountSeconds(out pFTCleanEAProductInfo.Image_DataProcessTime);
                HOperatorSet.CountSeconds(out pFTCleanEAProductInfo.Image_EndTime);
                //计算时间
                BIZ_General.CalculateProcessingTime(pFTCleanEAProductInfo);

            }
            catch (Exception ex)
            {
                MessageNotify.AddErrorLog("Cam2EA数据处理异常：" + ex.ToString());
            }
        }


        public void ProcessingData3D_A(ProductBase camEAProduct)
        {
            CameraEnum cameraEnum = CameraEnum.Cam3;

            try
            {

                HOperatorSet.CountSeconds(out camEAProduct.Image_StartDataProcessTime);

                PFTImageProduct pFTImageProduct = camEAProduct as PFTImageProduct;
                CamDetectionVariable camDetectionVariable = Project.Instance.cameraManager[cameraEnum].m_CamDetectionVariable;
                ResultType bIsFrameSizeNg = ResultType.NG;


                if(Project.Instance.cameraManager[cameraEnum].param_MsaThick3D.bIsCheck)
                {
                    PointCheck.DJ3DProcess(cameraEnum, Project.Instance.cameraManager[cameraEnum].param_MsaThick3D, pFTImageProduct.showHalconWindow);
                }


                foreach (var item in pFTImageProduct.ListImageSizes)
                {
                    var sizeitems = pFTCleanCraft.ListEASizes.FindAll(x => x.camera == cameraEnum & x.Enable == true);
                    if (sizeitems != null)
                    {
                        var sizeitem = sizeitems.Find(x => x.Name ==item.Name);
                        if (sizeitem != null)
                        {
                            item.DataResult = (float)item.DataResult * sizeitem.StD_RealFixRito + sizeitem.StD_RealFixOffset;
                            OtherAlgorithm.Calibration(sizeitem.EnableTopModel, sizeitem.StDValue, sizeitem.StD_TopModelRito, sizeitem.StD_TopModelOffset, ref item.DataResult);
                            item.DataResult= Math.Round(item.DataResult, 2);
                            var index = pFTImageProduct.ListImageSizes.FindIndex(x => x.Name == item.Name);
                            SizeInfo sizeInfo = pFTImageProduct.ListImageSizes[index];
                            GetSizeJudgeInfo( sizeitem,ref sizeInfo);
                            UIService.Instance.AddAction(() =>
                            {
                                ChartDataShow(chartUIParams[item.Name.ToString()], item.DataResult);
                                ChartDataShow(chartUIPage[item.Name.ToString()], item.DataResult);
                            });
                          
                            pFTImageProduct.showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = item.Name + ":" + item.DataResult.ToString("0.00"), m_stuGetColor = item.resultType == ResultType.NG ? HalconColor.red : item.resultType == ResultType.OK ? HalconColor.green : HalconColor.orange });
                        }
                        else
                        {
                            MessageNotify.AddErrorLog($"3D相机A_帧号:{pFTImageProduct.nCurImageID}_尺寸未配置");
                        }
                    }
                    else
                    {
                        MessageNotify.AddErrorLog($"3D相机A_帧号:{pFTImageProduct.nCurImageID}_尺寸未配置");
                    }
              
                    List<RowData> rowDatas = new List<RowData>();
                    rowDatas.Add(new RowData() { dataStr = DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss_fff"), resultType = ResultType.OK });
                    rowDatas.Add(new RowData() { dataStr = $"{pFTImageProduct.nCurImageID}", resultType = ResultType.OK });
                    rowDatas.Add(new RowData() { dataStr = $"{item.DataResult.ToString("0.00")}", resultType = item.resultType });
                    rowDatas.Add(new RowData() { dataStr = $"{item.resultType.ToString()}", resultType = item.resultType });


                    UIService.Instance.AddAction(() =>
                    {
                        NListView.Add(form.listView5, rowDatas.ToArray(), true);

                    });


                    bIZ_ThreadManage.limitThreadPool.RunAction(() =>
                    {
                        FileOperationCSV.WriteData_CSV(camDetectionVariable.ProdSizeDataPath, BIZ_Variable.Size3DProcessTitles.ToArray(), rowDatas.ToArray(), cameraEnum.ToString());

                    });


                }

                if (pFTImageProduct.ListImageSizes.Find(x => x.resultType == ResultType.NG) == null && pFTImageProduct.ListImageSizes.Count > 0)
                {
                    //找出最大值
                    bIsFrameSizeNg = pFTImageProduct.ListImageSizes.Max(x => x.resultType);
                
                }
                if (pFTImageProduct.ListImageSizes.Count() == 0)
                {
                    MessageNotify.AddErrorLog($"3D相机A_帧号:{pFTImageProduct.nCurImageID}_尺寸未检测到");
                }
                else
                {
                    bIsFrameSizeNg = pFTImageProduct.ListImageSizes.FirstOrDefault().resultType;
                    double value = pFTImageProduct.ListImageSizes.FirstOrDefault().DataResult - Project.Instance.thick3DManages[0].correctParam.JP_StdValue;
                    bIZ_ThreadManage.limitThreadPool.RunAction(() =>
                    {
                        if (Project.Instance.thick3DManages[0].WriteCorrectValue(value, pFTImageProduct.nCurEAID, out value) && Project.Instance.thick3DManages[0].correctParam.ENAutoJP)
                        {
                            send3DValue(1, (float)value);
                        }

                        MessageNotify.AddCommLog($"3D相机A_帧号:{pFTImageProduct.nCurImageID}_尺寸:{value.ToString("0.00")}");
                    });

                }
                camEAProduct.showHalconWindow.m_nMode = 2;
                camEAProduct.showHalconWindow.m_strCamName = Project.Instance.cameraManager[cameraEnum].m_CamParam.CameraName;
                camEAProduct.showHalconWindow.bIs3D = true;
                pFTImageProduct.showHalconWindow.m_bIsNG = bIsFrameSizeNg == ResultType.NG;
                pFTImageProduct.showHalconWindow.m_messageShowInfos.Insert(0, new MessageShowInfo() { m_strMessage = bIsFrameSizeNg.ToString(), m_stuGetColor = bIsFrameSizeNg == ResultType.NG ? HalconColor.red : bIsFrameSizeNg == ResultType.OK ? HalconColor.green : HalconColor.orange });

                bool bIsDB = bIsFrameSizeNg == ResultType.NG;
               
                
           
                //数据统计
                StatisticalData statisticalData = camDetectionVariable.statisticalData;

                //尺寸报警停机判断
                TJBJSizeInfo_WriteState(cameraEnum, pFTImageProduct.ListImageSizes, BIZ_Variable.ListEASizes, pFTCleanCraft.ListEASizes);
                ProductDataSizeStastic(pFTImageProduct.ListImageSizes, BIZ_Variable.sizeItemInfos_C);


                if (bIsDB) statisticalData.NGCount++;

                BIZ_General.ProdStatisticsCalculate(statisticalData, pFTImageProduct.nCurImageID, statisticalData.NGCount);

                UIService.Instance.AddAction(() =>
                {
                    NLabel.UpdateControls(form.lb_SumupCount_3D1Defect, statisticalData.SumupCount.ToString(), true);
                    NLabel.UpdateControls(form.lb_OKCount_3D1Defect, statisticalData.OKCount.ToString(), true);
                    NLabel.UpdateControls(form.lb_NGCount_3D1Defect, statisticalData.NGCount.ToString(), true);
                    NLabel.UpdateControls(form.lb_DBCount_3D1Dect, statisticalData.NGCount.ToString());
                    NLabel.UpdateControls(form.lb_Yield_3D1Defect, statisticalData.Yield.ToString("0.00") + "%", true);

                });
                //【2】图像显示
                HOperatorSet.CountSeconds(out camEAProduct.Image_EndTime);
                HOperatorSet.CountSeconds(out camEAProduct.Image_DataProcessTime);
                BIZ_General.CalculateProcessingTime(camEAProduct);
                PrimitiveTransition(cameraEnum, camEAProduct.showHalconWindow, out Operation_ImageInfo operation_ImageInfo);
                HWC5.operation_Image.imageInfosShow.Enqueue(operation_ImageInfo);
            }
            catch (Exception ee)
            {
                MessageNotify.AddErrorLog("相机3数据处理异常：" + ee.ToString());
            }



        }

       


        public void ProcessingData3D_B(ProductBase camEAProduct)
        {
            CameraEnum cameraEnum = CameraEnum.Cam4;

            try
            {

                HOperatorSet.CountSeconds(out camEAProduct.Image_StartDataProcessTime);

                PFTImageProduct pFTImageProduct = camEAProduct as PFTImageProduct;
                CamDetectionVariable camDetectionVariable = Project.Instance.cameraManager[cameraEnum].m_CamDetectionVariable;
                ResultType bIsFrameSizeNg = ResultType.NG;

                if (Project.Instance.cameraManager[cameraEnum].param_MsaThick3D.bIsCheck)
                {
                    PointCheck.DJ3DProcess(cameraEnum, Project.Instance.cameraManager[cameraEnum].param_MsaThick3D, pFTImageProduct.showHalconWindow);
                }


                foreach (var item in pFTImageProduct.ListImageSizes)
                {
                    var sizeitems = pFTCleanCraft.ListEASizes.FindAll(x => x.camera == cameraEnum & x.Enable == true);
                    if (sizeitems != null)
                    {
                        var sizeitem = sizeitems.Find(x => x.Name == item.Name);
                        if (sizeitem != null)
                        {
                            item.DataResult = (float)item.DataResult * sizeitem.StD_RealFixRito + sizeitem.StD_RealFixOffset;
                              OtherAlgorithm.Calibration(sizeitem.EnableTopModel, sizeitem.StDValue, sizeitem.StD_TopModelRito, sizeitem.StD_TopModelOffset, ref item.DataResult);
                            item.DataResult = Math.Round(item.DataResult, 2);
                            var index = pFTImageProduct.ListImageSizes.FindIndex(x => x.Name == item.Name);
                            SizeInfo sizeInfo = pFTImageProduct.ListImageSizes[index];
                            GetSizeJudgeInfo(sizeitem, ref sizeInfo);
                            UIService.Instance.AddAction(() =>
                            {
                                ChartDataShow(chartUIParams[item.Name.ToString()], item.DataResult);
                                ChartDataShow(chartUIPage[item.Name.ToString()], item.DataResult);
                            });
                            pFTImageProduct.showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = item.Name + ":" + item.DataResult.ToString("0.00"), m_stuGetColor = item.resultType == ResultType.NG ? HalconColor.red : item.resultType == ResultType.OK ? HalconColor.green : HalconColor.orange });
                        }
                        else
                        {
                            MessageNotify.AddErrorLog($"3D相机B_帧号:{pFTImageProduct.nCurImageID}_尺寸未配置");
                        }
                    }
                    else
                    {
                        MessageNotify.AddErrorLog($"3D相机B_帧号:{pFTImageProduct.nCurImageID}_尺寸未配置");
                    }


               
                    List<RowData> rowDatas = new List<RowData>();
                    rowDatas.Add(new RowData() { dataStr = DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss_fff"), resultType = ResultType.OK });
                    rowDatas.Add(new RowData() { dataStr = $"{pFTImageProduct.nCurImageID}", resultType = ResultType.OK });
                    rowDatas.Add(new RowData() { dataStr = $"{item.DataResult.ToString("0.00")}", resultType = item.resultType });
                    rowDatas.Add(new RowData() { dataStr = $"{item.resultType.ToString()}", resultType = item.resultType });


                    UIService.Instance.AddAction(() =>
                    {
                        NListView.Add(form.listView6, rowDatas.ToArray(), true);

                    });


                    bIZ_ThreadManage.limitThreadPool.RunAction(() =>
                    {
                        FileOperationCSV.WriteData_CSV(camDetectionVariable.ProdSizeDataPath, BIZ_Variable.Size3DProcessTitles.ToArray(), rowDatas.ToArray(), cameraEnum.ToString());

                    });


                }

                if (pFTImageProduct.ListImageSizes.Find(x => x.resultType == ResultType.NG) == null && pFTImageProduct.ListImageSizes.Count > 0)
                {
                    bIsFrameSizeNg = pFTImageProduct.ListImageSizes.Max(x => x.resultType);
                }
                if (pFTImageProduct.ListImageSizes.Count() == 0)
                {
                    MessageNotify.AddErrorLog($"3D相机B_帧号:{pFTImageProduct.nCurImageID}_尺寸未检测到");
                }
                else
                {
                  
                    double   value = pFTImageProduct.ListImageSizes.FirstOrDefault().DataResult - Project.Instance.thick3DManages[1].correctParam.JP_StdValue;
                    bIZ_ThreadManage.limitThreadPool.RunAction(() =>
                    {
                        if (Project.Instance.thick3DManages[1].WriteCorrectValue(value, pFTImageProduct.nCurEAID, out value) && Project.Instance.thick3DManages[1].correctParam.ENAutoJP)
                        {
                            send3DValue(2, (float)value);
                        }

                        MessageNotify.AddCommLog($"3D相机B_帧号:{pFTImageProduct.nCurImageID}_尺寸:{value.ToString("0.00")}");
                    });


                }
                camEAProduct.showHalconWindow.m_strCamName = Project.Instance.cameraManager[cameraEnum].m_CamParam.CameraName;
                camEAProduct.showHalconWindow.m_nMode = 2;
                camEAProduct.showHalconWindow.bIs3D = true;
                pFTImageProduct.showHalconWindow.m_bIsNG = bIsFrameSizeNg == ResultType.NG;
                pFTImageProduct.showHalconWindow.m_messageShowInfos.Insert(0, new MessageShowInfo() { m_strMessage = bIsFrameSizeNg.ToString(), m_stuGetColor = bIsFrameSizeNg == ResultType.NG ? HalconColor.red : bIsFrameSizeNg == ResultType.OK ? HalconColor.green : HalconColor.orange });

                bool bIsDB = bIsFrameSizeNg == ResultType.NG;
            

                //数据统计
                StatisticalData statisticalData = camDetectionVariable.statisticalData;

                //尺寸报警停机判断
                TJBJSizeInfo_WriteState(cameraEnum, pFTImageProduct.ListImageSizes, BIZ_Variable.ListEASizes, pFTCleanCraft.ListEASizes);
                ProductDataSizeStastic(pFTImageProduct.ListImageSizes, BIZ_Variable.sizeItemInfos_D);

                if (bIsDB) statisticalData.NGCount++;

                BIZ_General.ProdStatisticsCalculate(statisticalData, pFTImageProduct.nCurImageID, statisticalData.NGCount);

                UIService.Instance.AddAction(() =>
                {
                    NLabel.UpdateControls(form.lb_SumupCount_3D2Defect, statisticalData.SumupCount.ToString(), true);
                    NLabel.UpdateControls(form.lb_OKCount_3D2efect, statisticalData.OKCount.ToString(), true);
                    NLabel.UpdateControls(form.lb_NGCount_3D2efect, statisticalData.NGCount.ToString(), true);
                    NLabel.UpdateControls(form.lb_DBCount_3D2Dect, statisticalData.NGCount.ToString());
                    NLabel.UpdateControls(form.lb_Yield_3D2efect, statisticalData.Yield.ToString("0.00") + "%", true);

                });

                //【2】图像显示
                HOperatorSet.CountSeconds(out camEAProduct.Image_EndTime);
                HOperatorSet.CountSeconds(out camEAProduct.Image_DataProcessTime);
                BIZ_General.CalculateProcessingTime(camEAProduct);
                PrimitiveTransition(cameraEnum, camEAProduct.showHalconWindow, out Operation_ImageInfo operation_ImageInfo);
                HWC6.operation_Image.imageInfosShow.Enqueue(operation_ImageInfo);
            }
            catch (Exception ee)
            {
                MessageNotify.AddErrorLog("相机4数据处理异常：" + ee.ToString());
            }
        }


        public void send3DValue(int index, float value)
        {
            try
            {
                if (index == 1)
                {
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_SendThickValue3D1_CCDPFT.Value = value;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_SendThickValue3D1_CCDPFT, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                }
                else
                {
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_SendThickValue3D2_CCDPFT.Value = value;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_SendThickValue3D2_CCDPFT, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }
            }
            catch (Exception ee)
            {
                MessageNotify.AddErrorLog("发送3D值异常：" + ee.ToString());
                //throw;
            }

        }


        public float GetSpeedValue()
        {
            float speedValue = 0;
            try
            {
                 Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_MachineSpeed_CCDPFT, ReadLogSaveType.True, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                 speedValue= Project.Instance.cutOverlayPLCManage.m_DevAddress.In_MachineSpeed_CCDPFT.Value;
            }
            catch (Exception ee)
            {
                MessageNotify.AddErrorLog("获取速度值异常：" + ee.ToString());
            }
            return speedValue;
        }
        public ResultType EvaluateEADefectStatus(PFTCleanEAProductInfo pFTCleanEAProductInfo)
        {
            // 如果没有缺陷或者当前EAID为0，则直接返回OK

            if (pFTCleanEAProductInfo.ListEADefects.Count() == 0 || pFTCleanEAProductInfo.EAID == 0)
            {
                return ResultType.OK;
            }

            // 检查是否存在NG的缺陷
            var ngIndex = pFTCleanEAProductInfo.ListEADefects.FindIndex(x => x.resultType == ResultType.NG);
            if (ngIndex != -1)
            {
                return ResultType.NG;
            }

            // 如果没有NG的缺陷，检查是否存在Warn的尺寸问题
            var warnIndex = pFTCleanEAProductInfo.ListEADefects.FindIndex(x => x.resultType == ResultType.Warn);
            if (warnIndex != -1)
            {
                return ResultType.Warn;
            }

            // 如果既没有NG的缺陷，也没有Warn的尺寸问题，则返回OK
            return ResultType.OK;
        }

        public ResultType EvaluateEASizeStatus(PFTCleanEAProductInfo pFTCleanEAProductInfo)
        {
            // 如果没有缺陷或者当前EAID为0，则直接返回OK

            if (pFTCleanEAProductInfo.ListEASizes.Count() == 0 || pFTCleanEAProductInfo.EAID == 0)
            {
                return ResultType.OK;
            }

            // 检查是否存在NG的缺陷
            var ngIndex = pFTCleanEAProductInfo.ListEASizes.FindIndex(x => x.resultType == ResultType.NG);
            if (ngIndex != -1)
            {
                return ResultType.NG;
            }

            // 如果没有NG的缺陷，检查是否存在Warn的尺寸问题
            var warnIndex = pFTCleanEAProductInfo.ListEASizes.FindIndex(x => x.resultType == ResultType.Warn);
            if (warnIndex != -1)
            {
                return ResultType.Warn;
            }

            // 如果既没有NG的缺陷，也没有Warn的尺寸问题，则返回OK
            return ResultType.OK;
        }

        /// <summary>
        /// 尺寸报警停机判断
        /// </summary>
        /// <param name="sizeInfos"></param>
        /// <param name="TJBJInfos"></param>
        /// <param name="CraftSizeInfos"></param>
        public void TJBJSizeInfo_WriteState(CameraEnum cameraEnum, List<SizeInfo> sizeInfos, List<SizeItemInfo> TJBJSizeInfos, List<SizeItemInfo> CraftSizeInfos)
        {
            foreach (var item in sizeInfos)
            {
                var sizeIndex = TJBJSizeInfos.FindIndex(x => x.Name == item.Name && x.camera == cameraEnum);
                var sizeCraftIndex = CraftSizeInfos.FindIndex(x => x.Name == item.Name && x.camera == cameraEnum);
                if (sizeIndex == -1)
                {
                    MessageNotify.AddErrorLog($"尺寸_BIZ_Variable.ListEASizes_结果{item.Name}未找到对应的尺寸信息");
                    continue;
                }

                if (sizeCraftIndex == -1)
                {
                    MessageNotify.AddErrorLog($"尺寸_pFTCleanCraft.ListEASizes_结果{item.Name}未找到对应的尺寸信息");
                    continue;
                }



                if (item.BJInfo.IsAlarm)
                {
                    TJBJSizeInfos[sizeIndex].CountNGBJ++;
                }

                if (TJBJSizeInfos[sizeIndex].CountNGBJ > CraftSizeInfos[sizeIndex].CountNGBJ && CraftSizeInfos[sizeIndex].BJ)
                {
                    TJBJInfo.AlarmWriteState(AlarmName.检测项报警停机, StopAlarmStateEnum.报警, cameraEnum.ToString() + "_" + item.Description);
                    TJBJSizeInfos[sizeIndex].CountNGBJ = 0;
                }

                if (item.TJInfo.IsAlarm)
                {
                    TJBJSizeInfos[sizeIndex].CountNGTJ++;
                }

                if (TJBJSizeInfos[sizeIndex].CountNGTJ > CraftSizeInfos[sizeIndex].CountNGTJ && CraftSizeInfos[sizeIndex].TJ)
                {
                    TJBJInfo.AlarmWriteState(AlarmName.检测项报警停机, StopAlarmStateEnum.停机, cameraEnum.ToString() + "_" + item.Description);
                    TJBJSizeInfos[sizeIndex].CountNGTJ = 0;
                }

            }

        }


        public void TJBJDeftInfo_WriteState(CameraEnum cameraEnum, List<NgTypeInfo> deftInfos, List<DefectItemInfo> TJBJDeftInfos, List<DefectItemInfo> CraftDeftInfos)
        {
            foreach (var item in deftInfos)
            {
                var deftIndex = TJBJDeftInfos.FindIndex(x => x.Name == item.m_strNgTypeName.ToString() && x.camera == cameraEnum);
                var deftCraftIndex = CraftDeftInfos.FindIndex(x => x.Name == item.m_strNgTypeName.ToString() && x.camera == cameraEnum);
                if (deftIndex == -1)
                {
                    MessageNotify.AddErrorLog($"缺项_BIZ_Variable.ListDefectInfos_结果{item.m_strNgTypeName.ToString()}未找到对应的缺项信息");
                    continue;
                }

                if (deftCraftIndex == -1)
                {
                    MessageNotify.AddErrorLog($"缺项_pFTCleanCraft.ListDefectInfos_结果{item.m_strNgTypeName.ToString()}未找到对应的缺项信息");
                    continue;
                }

                if (item.resultType == ResultType.NG)
                {
                    TJBJDeftInfos[deftIndex].CountNGBJ++;
                }
                if (TJBJDeftInfos[deftIndex].CountNGBJ > CraftDeftInfos[deftIndex].CountNGBJ && CraftDeftInfos[deftIndex].BJ)
                {
                    TJBJInfo.AlarmWriteState(AlarmName.检测项报警停机, StopAlarmStateEnum.报警, cameraEnum.ToString() + "_" + item.m_strNgTypeName.ToString());
                    TJBJDeftInfos[deftIndex].CountNGBJ = 0;
                }



                if (item.resultType == ResultType.NG)
                {
                    TJBJDeftInfos[deftIndex].CountNGTJ++;
                }

                if (TJBJDeftInfos[deftIndex].CountNGTJ > CraftDeftInfos[deftIndex].CountNGTJ && CraftDeftInfos[deftIndex].TJ)
                {
                    TJBJInfo.AlarmWriteState(AlarmName.检测项报警停机, StopAlarmStateEnum.停机, cameraEnum.ToString() + "_" + item.m_strNgTypeName.ToString());
                    TJBJDeftInfos[deftIndex].CountNGTJ = 0;
                }



            }


        }


        public void ProductDataSizeStastic(List<SizeInfo> sizeInfos, List<SizeItemInfo> SatisticalSizeInfos)
        {
            foreach (var item in SatisticalSizeInfos)
            {
                var sizeIndex = sizeInfos.FindIndex(x => x.Name == item.Name && x.DBInfo.IsAlarm == true);
                if (!(sizeIndex == -1))
                {
                    item.CountNGDB++;
                }
            }
        }

        public void ProductDataDefectStastic(List<NgTypeInfo> deftInfos, List<DefectItemInfo> SatisticalDeftInfos)
        {
            foreach (var item in SatisticalDeftInfos)
            {
                var deftIndex = deftInfos.FindIndex(x => x.m_strNgTypeName.ToString() == item.Name && x.resultType == ResultType.NG);
                if (!(deftIndex == -1))
                {
                    item.CountNGDB++;
                }
            }
        }


        public void StasticShowAndSave()
        {
            try
            {
                CamDetectionVariable camDetectionVariable1 = Project.Instance.cameraManager[CameraEnum.Cam1].m_CamDetectionVariable;
                CamDetectionVariable camDetectionVariable2 = Project.Instance.cameraManager[CameraEnum.Cam2].m_CamDetectionVariable;
                CamDetectionVariable camDetectionVariable3 = Project.Instance.cameraManager[CameraEnum.Cam3].m_CamDetectionVariable;
                CamDetectionVariable camDetectionVariable4 = Project.Instance.cameraManager[CameraEnum.Cam4].m_CamDetectionVariable;
                List<string[]> dataList = new List<string[]>();
                string[] title = { "工位", "产量", "NG数", "良率", "打标数" };
                dataList.Add(title);
                dataList.Add(new string[] { "线扫检测", camDetectionVariable2.statisticalData.SumupCount.ToString(), camDetectionVariable2.statisticalData.NGCount.ToString(), camDetectionVariable2.statisticalData.Yield.ToString("0.00") + "%", camDetectionVariable2.statisticalData.OKCount.ToString() });
                dataList.Add(new string[] { "A面#3D", camDetectionVariable3.statisticalData.SumupCount.ToString(), camDetectionVariable3.statisticalData.NGCount.ToString(), camDetectionVariable3.statisticalData.Yield.ToString("0.00") + "%", camDetectionVariable3.statisticalData.OKCount.ToString() });
                dataList.Add(new string[] { "B面#3D", camDetectionVariable4.statisticalData.SumupCount.ToString(), camDetectionVariable4.statisticalData.NGCount.ToString(), camDetectionVariable4.statisticalData.Yield.ToString("0.00") + "%", camDetectionVariable4.statisticalData.OKCount.ToString() });

                dataList.Add(new string[] { "———-------" });
                title = new string[] { "NG尺寸类型", "线扫A面NG统计" };
                dataList.Add(title);
                foreach (var item in BIZ_Variable.sizeItemInfos_A)
                {
                    dataList.Add(new string[] { item.Description, item.CountNGDB.ToString() });
                }

                dataList.Add(new string[] { "———-------" });
                title = new string[] { "NG尺寸类型", "线扫B面NG统计" };
                dataList.Add(title);
                foreach (var item in BIZ_Variable.sizeItemInfos_B)
                {
                    dataList.Add(new string[] { item.Description, item.CountNGDB.ToString() });
                }

                dataList.Add(new string[] { "———-------" });
                title = new string[] { "NG尺寸类型", "A面#3D_NG统计" };
                dataList.Add(title);
                foreach (var item in BIZ_Variable.sizeItemInfos_C)
                {
                    dataList.Add(new string[] { item.Description, item.CountNGDB.ToString() });
                }

                dataList.Add(new string[] { "———-------" });
                title = new string[] { "NG尺寸类型", "B面#3D_NG统计" };
                dataList.Add(title);
                foreach (var item in BIZ_Variable.sizeItemInfos_D)
                {
                    dataList.Add(new string[] { item.Description, item.CountNGDB.ToString() });
                }

                dataList.Add(new string[] { "———-------" });
                title = new string[] { "NG缺项类型", "线扫A面NG统计" };
                dataList.Add(title);
                foreach (var item in BIZ_Variable.defectItemInfos_A)
                {
                    dataList.Add(new string[] { item.Name, item.CountNGDB.ToString() });
                }

                dataList.Add(new string[] { "———-------" });
                title = new string[] { "NG缺项类型", "线扫B面NG统计" };
                dataList.Add(title);
                foreach (var item in BIZ_Variable.defectItemInfos_B)
                {
                    dataList.Add(new string[] { item.Name, item.CountNGDB.ToString() });
                }
                dataList.Add(new string[] { "———-------" });



                bIZ_ThreadManage.limitThreadPool.RunAction(() =>
                {
                    FileOperationCSV.WriteData_CSV(Project.Instance.paramPathSet.ProdStatisticsDataPath, dataList, "总计");
                    form.BeginInvoke(new Action(() =>
                    {
                        form.listNgInfo.Items.Clear();
                        foreach (var item in dataList)
                        {
                            NListView2.Add(form.listNgInfo, item, 100, ListViewShowType.Clear, false);
                        }

                    }
                ));
                });


                //UIService.Instance.AddAction(() =>
                //{

                //});
                //form.rtbNgInfo.Clear();
                //foreach (var item in dataList)
                //{
                // form.rtbNgInfo.AppendText(string.Join("|", item) + "\n");
                //}



            }
            catch (Exception ee)
            {

                throw;
            }
        }


        public void ProcessImageDefects(List<NgTypeInfo> imageDefects, CameraEnum cameraEnum, bool flag = false)
        {
            // 创建一个列表用于收集需要释放的对象
            List<NgTypeInfo> disposables = new List<NgTypeInfo>();

            DisplayWindowClass displayWindowClass;
            if (cameraEnum == CameraEnum.Cam1)
            {
                displayWindowClass = DisplayWindowClass1;
            }
            else
            {
                displayWindowClass = DisplayWindowClass4;
            }

            foreach (NgTypeInfo item in imageDefects)
            {
                // 查找对应的缺陷信息
                DefectItemInfo defectInfo = pFTCleanCraft.ListDefectInfos
                    .FirstOrDefault(x => x.Name == item.m_strNgTypeName.ToString() && x.camera == cameraEnum);

                if (defectInfo == null)
                {
                    // 记录未找到对应缺陷信息的错误
                    MessageNotify.AddErrorLog(
                        $"从缺项工艺ListDefectInfos中未找到对应的缺项信息{item.m_strNgTypeName}");
                    item.resultType = ResultType.NG;
                    if (!flag)
                    {
                        BIZ_General.NGImageQueueTransition(
                        cameraEnum,
                        new List<NgTypeInfo> { item },
                        item.m_nImgIndex,
                        true,
                        ref displayWindowClass.operation_Image.imageInfosShow);
                    }

                    continue;
                }
                else
                {
                    if (defectInfo.Enable)
                    {
                        item.resultType = ResultType.Warn;

                        // 检查缺陷面积是否超过阈值
                        if (item.m_fNgTypArea > defectInfo.Area)
                        {
                            if (defectInfo.DB)
                            {
                                item.resultType = ResultType.NG;
                            }

                            // 将缺陷信息添加到图像队列（启用报警）


                        }

                        if (defectInfo.HeightLogic)
                        {
                            if (item.m_fNgTypHeight > defectInfo.Height && item.resultType != ResultType.NG)
                            {
                                if (defectInfo.DB)
                                {
                                    item.resultType = ResultType.NG;
                                }



                            }
                        }


                        if (defectInfo.WidthLogic)
                        {
                            if (item.m_fNgTypWidth > defectInfo.Width && item.resultType != ResultType.NG)
                            {
                                if (defectInfo.DB)
                                {
                                    item.resultType = ResultType.NG;
                                }



                            }
                        }

                        // 如果结果类型为警告，将缺陷信息添加到图像队列（不启用报警）

                        if (!flag)
                        {
                            BIZ_General.NGImageQueueTransition(
                           cameraEnum,
                           new List<NgTypeInfo> { item },
                           item.m_nImgIndex,
                           item.resultType == ResultType.NG,
                           ref displayWindowClass.operation_Image.imageInfosShow);
                        }


                    }
                    else
                    {
                        // 如果缺陷未启用，将对象添加到 disposables 列表中
                        disposables.Add(item);

                    }
                }
            }

            // 在循环外部统一释放资源
            foreach (NgTypeInfo disposableItem in disposables)
            {
                disposableItem?.Dispose();
                MessageNotify.AddDebugLog($"未启用的缺项项目，释放缺项对象{disposableItem.m_strNgTypeName}");
            }

            // 从原始列表中移除已处理的项（可选）
            imageDefects.RemoveAll(item => disposables.Contains(item));
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
        private void PrimitiveTransition(CameraEnum cameraEnum, ShowHalconWindow showHalconWindow, out Operation_ImageInfo operation_ImageInfo)
        {
            operation_ImageInfo = null;
            try
            {
                ImageSaveParamer in_imageSaveParamer = Project.Instance.cameraManager[cameraEnum].m_ImageSaveParamer;
                string imageSavePath = in_imageSaveParamer.ImageSave_Path_Primitive;
                ImageSaveFormat saveImageFormat = (ImageSaveFormat)in_imageSaveParamer.SaveImageFormat_Primitive;
                ImageDeepthSaveFormat imageDeepthSaveFormat = (ImageDeepthSaveFormat)in_imageSaveParamer.SaveImageFormat_Deepth;
                ImageSaveSelect imageSaveSelect = (ImageSaveSelect)in_imageSaveParamer.SaveImageSelect_Primitive;
                bool isShowImage = in_imageSaveParamer.IsShowImage_Primitive;
                bool IsCompression_Deepth = in_imageSaveParamer.IsCompression_Deepth;
                bool HighDefinition_Screenshot = in_imageSaveParamer.HighDefinition_Screenshot;
                ImageSaveSelect screenshotSaveSelect = (ImageSaveSelect)in_imageSaveParamer.SaveImageSelect_Screenshot;
                string screenshotSavePath = in_imageSaveParamer.ImageSave_Path_Screenshot;
                ImageSaveFormat screenshotSaveFormat = (ImageSaveFormat)in_imageSaveParamer.SaveImageFormat_Screenshot;
                string imageSaveName = showHalconWindow.m_nImageFrameID.ToString().PadLeft(5, '0') + "_" + (showHalconWindow.m_nProductID).ToString();
                operation_ImageInfo = BIZ_General.ImageDataTypeTransition(showHalconWindow, Project.Instance.paramPathSet.ProductMojuanPath, imageSavePath, imageSaveName, saveImageFormat, imageSaveSelect, isShowImage,
                IsCompression_Deepth, HighDefinition_Screenshot, screenshotSaveSelect, Project.Instance.paramPathSet.ProductMojuanPath, screenshotSavePath, imageSaveName, screenshotSaveFormat, imageDeepthSaveFormat, cameraEnum.ToString());
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(cameraEnum.ToString(), ex);
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
        private void PrimitiveTransitionEA(CameraEnum cameraEnum, ShowHalconWindow showHalconWindow, out Operation_ImageInfo operation_ImageInfo)
        {
            operation_ImageInfo = null;
            try
            {
                ImageSaveParamer in_imageSaveParamer = Project.Instance.cameraManager[cameraEnum].m_ImageSaveParamer;

                string imageSavePath = in_imageSaveParamer.ImageSave_Path_ProdImg;
                ImageSaveFormat saveImageFormat = (ImageSaveFormat)in_imageSaveParamer.SaveImageFormat_ProdImg;
                ImageSaveSelect imageSaveSelect = (ImageSaveSelect)in_imageSaveParamer.SaveImageSelect_ProdImg;
                bool isShowImage = in_imageSaveParamer.IsShowImage_Primitive;
                bool HighDefinition_Screenshot = in_imageSaveParamer.HighDefinition_ProdScreenshot;
                ImageSaveSelect screenshotSaveSelect = (ImageSaveSelect)in_imageSaveParamer.SaveImageSelect_ProdScreenshot;
                string screenshotSavePath = in_imageSaveParamer.ImageSave_Path_ProdScreenshot;
                ImageSaveFormat screenshotSaveFormat = (ImageSaveFormat)in_imageSaveParamer.SaveImageFormat_ProdScreenshot;
                string imageSaveName = showHalconWindow.m_nImageFrameID.ToString().PadLeft(5, '0') + "_" + (showHalconWindow.m_nProductID).ToString();
                operation_ImageInfo = BIZ_General.ImageDataTypeTransition(showHalconWindow, Project.Instance.paramPathSet.ProductMojuanPath, imageSavePath, imageSaveName, saveImageFormat, imageSaveSelect, isShowImage,
                   HighDefinition_Screenshot, screenshotSaveSelect, Project.Instance.paramPathSet.ProductMojuanPath, screenshotSavePath, imageSaveName, screenshotSaveFormat, cameraEnum.ToString());
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(cameraEnum.ToString(), ex);
            }
        }

        public AThreadManageBase ThreadManage { get; set; }


        public void InitStastic()
        {


           
        }


        public void UpdateCraft()
        {
            pFTCleanCraft = Project.Instance.craftBase as PFTCleanCraft;
            BIZ_Variable.ListEASizes?.Clear();
            BIZ_Variable.ListEASizes = new List<SizeItemInfo>();
            //初始化尺寸数据
            foreach (var item in pFTCleanCraft.ListEASizes)
            {
                SizeItemInfo sizeItemInfo = new SizeItemInfo();
                sizeItemInfo.Description = item.Description;
                sizeItemInfo.Name = item.Name;
                sizeItemInfo.camera = item.camera;
                BIZ_Variable.ListEASizes.Add(sizeItemInfo);

            }


            BIZ_Variable.ListDefectInfos?.Clear();
            BIZ_Variable.ListDefectInfos = new List<DefectItemInfo>();


            //初始化缺陷数据
            foreach (var item in pFTCleanCraft.ListDefectInfos)
            {
                DefectItemInfo defectItemInfo = new DefectItemInfo();
                defectItemInfo.Description = item.Description;
                defectItemInfo.Name = item.Name;
                defectItemInfo.camera = item.camera;
                BIZ_Variable.ListDefectInfos.Add(defectItemInfo);

            }


        }

        public void AlarmReset()
        {
            Project.Instance.dbjCardManager.SendAlarmReset();
        }

        public void ClearForm()
        {
            try
            {
                NLabel.UpdateControls(form.LB_Formula, Project.Instance.systemInfo.CurrentCraft);
                NLabel.UpdateControls(form.LB_DetectionType, Project.Instance.craftBase.materialCrafEnum.ToString());
                NListView.DeleteListAllItems(form.listView1);
                NListView.DeleteListAllItems(form.listView2);
                NListView.DeleteListAllItems(form.listView3);
                NListView.DeleteListAllItems(form.listView4);
                NListView.DeleteListAllItems(form.listView5);
                NListView.DeleteListAllItems(form.listView6);

                NListView.DeleteListAllItems(form.ListView_A);
                NListView.DeleteListAllItems(form.ListView_AA);
                NListView.DeleteListAllItems(form.ListView_B);
                NListView.DeleteListAllItems(form.ListView_BB);
                NListView.DeleteListAllItems(form.ListView_AB);
                // NListView.DeleteListAllItems(form.ListView_AABB);

                //生产统计初始化
                CameraEnum cameraEnum = CameraEnum.Cam1;
                CamDetectionVariable camDetectionVariable = Project.Instance.cameraManager[cameraEnum].m_CamDetectionVariable;
                NLabel.UpdateControls(form.lb_SumupCount_Line, camDetectionVariable.statisticalData.SumupCount.ToString());
                NLabel.UpdateControls(form.lb_Yield_Line, camDetectionVariable.statisticalData.Yield.ToString("F2"));
                NLabel.UpdateControls(form.lb_NGCount_Line, camDetectionVariable.statisticalData.NGCount.ToString());
                NLabel.UpdateControls(form.lb_OKCount_Line, camDetectionVariable.statisticalData.OKCount.ToString());
                NLabel.UpdateControls(form.lb_DBCount_lineDect, camDetectionVariable.statisticalData.DBSendCount.ToString());
                NLabel.UpdateControls(form.lbTotallength, $"总长度：{camDetectionVariable.statisticalData.fTotolLength.ToString("F2")}");
                NLabel.UpdateControls(form.lbOklength, $"OK长度：{camDetectionVariable.statisticalData.fOkLength.ToString("F2")}");
                NLabel.UpdateControls(form.lbNglength, $"NG长度：{camDetectionVariable.statisticalData.fNgLength.ToString("F2")}");

                cameraEnum = CameraEnum.Cam3;
                camDetectionVariable = Project.Instance.cameraManager[cameraEnum].m_CamDetectionVariable;
                NLabel.UpdateControls(form.lb_SumupCount_3D1Defect, camDetectionVariable.statisticalData.SumupCount.ToString());
                NLabel.UpdateControls(form.lb_Yield_3D1Defect, camDetectionVariable.statisticalData.Yield.ToString("F2"));
                NLabel.UpdateControls(form.lb_NGCount_3D1Defect, camDetectionVariable.statisticalData.NGCount.ToString());
                NLabel.UpdateControls(form.lb_OKCount_3D1Defect, camDetectionVariable.statisticalData.OKCount.ToString());
                NLabel.UpdateControls(form.lb_DBCount_3D1Dect, camDetectionVariable.statisticalData.DBSendCount.ToString());

                cameraEnum = CameraEnum.Cam4;
                camDetectionVariable = Project.Instance.cameraManager[cameraEnum].m_CamDetectionVariable;
                NLabel.UpdateControls(form.lb_SumupCount_3D2Defect, camDetectionVariable.statisticalData.SumupCount.ToString());
                NLabel.UpdateControls(form.lb_Yield_3D2efect, camDetectionVariable.statisticalData.Yield.ToString("F2"));
                NLabel.UpdateControls(form.lb_NGCount_3D2efect, camDetectionVariable.statisticalData.NGCount.ToString());
                NLabel.UpdateControls(form.lb_OKCount_3D2efect, camDetectionVariable.statisticalData.OKCount.ToString());
                NLabel.UpdateControls(form.lb_DBCount_3D2Dect, camDetectionVariable.statisticalData.DBSendCount.ToString());
                form.BeginInvoke(new MethodInvoker(InitChart));
              
            }
            catch (Exception ee)
            {

                throw;
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


            DisplayWindowClass1?.Dispose();
            DisplayWindowClass2?.Dispose();
            DisplayWindowClass3?.Dispose();
            DisplayWindowClass4?.Dispose();
            DisplayWindowClass5?.Dispose();
            DisplayWindowClass6?.Dispose();

            BIZ_Variable?.Dispose();

        }

        public void ImageShowSave()
        {
            try
            {
                int disposeImageRetainLenght = 2;
                BIZ_General.Image_Process(HWC1.operation_Image, disposeImageRetainLenght, HWC1);
                // BIZ_General.Image_Process(HWC2.operation_Image, disposeImageRetainLenght, HWC2);
                BIZ_General.Image_Process(HWC3.operation_Image, disposeImageRetainLenght, HWC3);
                //  BIZ_General.Image_Process(HWC4.operation_Image, disposeImageRetainLenght, HWC4);
                BIZ_General.Image_Process(HWC5.operation_Image, disposeImageRetainLenght, HWC5);
                BIZ_General.Image_Process(HWC6.operation_Image, disposeImageRetainLenght, HWC6);
                BIZ_General.Image_Process(HWC7.operation_Image, disposeImageRetainLenght, HWC7);
                BIZ_General.Image_Process(HWC8.operation_Image, disposeImageRetainLenght, HWC8);

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
                    Operation_ImageProcessor processor = DisplayWindowClass4.operation_Image;
                    if (processor.imageInfosShow.Count > 0)
                    {
                        DisplayWindowClass displayWindowClass = CombinationHWindowControl_cam2.GetDisplayWindowClass();

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

        public void Communication()
        {
            try
            {
                bool IsIntervalTime = intervalExecution.GetWhetherToReachIntervalTime(1000);

                //=========================================================================
                //读取打标机信息
                if (Project.Instance.dbjCardManager != null && Project.Instance.dbjCardManager.initState == true)
                {
                    if (IsIntervalTime)
                    {
                        bIZ_ThreadManage.limitThreadPool.RunAction(() =>
                        {
                            Project.Instance.dbjCardManager.SendData();
                        });

                    }
                    Project.Instance.dbjCardManager.ReadData();

                }
            
                //设备数据发送与接收
                if (Project.Instance.cutOverlayPLCManage != null && Project.Instance.cutOverlayPLCManage.InitState == true)
                {
                   

                    AutoChangeRoll();

                }

                if (Project.Instance.cutOverlayPLCManage != null && Project.Instance.cutOverlayPLCManage.InitState == true)
                {
                    //心跳信号
                    if (IsIntervalTime)
                    {
                        Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_CorrectHeart_CCDXLA.Value = 1;
                        Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_CorrectHeart_CCDXLA, true, Project.Instance.cutOverlayPLCManage.HeartbeatLog);
                    }
                }


            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }

        public void AutoChangeRoll()
        {
            try
            {
                string reelNumber = DateTime.Now.ToString("yyyyMMddHHmm");
                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_ChangeRollTrigger_CCDPFT, ReadLogSaveType.ReadDifferentState, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                bool IsChangeRollTrigger = (int)Project.Instance.cutOverlayPLCManage.m_DevAddress.In_ChangeRollTrigger_CCDPFT.Value == 1;
                if (IsChangeRollTrigger)
                {
                    bool bIsReadSuccess = Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_ChangeRollNum_CCDPFT, 20, ReadLogSaveType.ReadDifferentState, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                    if (!bIsReadSuccess)
                    {
                        TJBJInfo.AlarmWriteState(AlarmName.自动换卷失败, StopAlarmStateEnum.停机, $"未读取到膜卷号");
                        return;
                    }
                    bool result = Form_Main.FormMain.MainDetection.AutoHJ(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_ChangeRollNum_CCDPFT.Value);
                    if (result)
                    {
                        Project.Instance.cutOverlayPLCManage.m_DevAddress.In_ChangeRollTrigger_CCDPFT.Value = 0;
                    }
                    else
                    {
                        TJBJInfo.AlarmWriteState(AlarmName.自动换卷失败, StopAlarmStateEnum.停机, $"未读取到膜卷号");
                        return;
                    }

                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_ChangeRollTrigger_CCDPFT, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }
            }
            catch (Exception ex)
            {
                MessageNotify.AddErrorLog("自动换卷报错：" + ex.ToString()); ;

            }
        }


        public void InitForm()
        {
            NLabel.UpdateControls(form.LB_Formula, Project.Instance.systemInfo.CurrentCraft);
            NLabel.UpdateControls(form.LB_DetectionType, Project.Instance.craftBase.materialCrafEnum.ToString());

            //生产统计初始化
            CameraEnum cameraEnum = CameraEnum.Cam1;
            CamDetectionVariable camDetectionVariable = Project.Instance.cameraManager[cameraEnum].m_CamDetectionVariable;
            NLabel.UpdateControls(form.lb_SumupCount_Line, camDetectionVariable.statisticalData.SumupCount.ToString());
            NLabel.UpdateControls(form.lb_Yield_Line, camDetectionVariable.statisticalData.Yield.ToString("F2"));
            NLabel.UpdateControls(form.lb_NGCount_Line, camDetectionVariable.statisticalData.NGCount.ToString());
            NLabel.UpdateControls(form.lb_OKCount_Line, camDetectionVariable.statisticalData.OKCount.ToString());
            NLabel.UpdateControls(form.lb_DBCount_lineDect, camDetectionVariable.statisticalData.DBSendCount.ToString());
            NLabel.UpdateControls(form.lbTotallength, $"总长度：{camDetectionVariable.statisticalData.fTotolLength.ToString("F2")}");
            NLabel.UpdateControls(form.lbOklength, $"OK长度：{camDetectionVariable.statisticalData.fOkLength.ToString("F2")}");
            NLabel.UpdateControls(form.lbNglength, $"NG长度：{camDetectionVariable.statisticalData.fNgLength.ToString("F2")}");

            cameraEnum = CameraEnum.Cam3;
            camDetectionVariable = Project.Instance.cameraManager[cameraEnum].m_CamDetectionVariable;
            NLabel.UpdateControls(form.lb_SumupCount_3D1Defect, camDetectionVariable.statisticalData.SumupCount.ToString());
            NLabel.UpdateControls(form.lb_Yield_3D1Defect, camDetectionVariable.statisticalData.Yield.ToString("F2"));
            NLabel.UpdateControls(form.lb_NGCount_3D1Defect, camDetectionVariable.statisticalData.NGCount.ToString());
            NLabel.UpdateControls(form.lb_OKCount_3D1Defect, camDetectionVariable.statisticalData.OKCount.ToString());
            NLabel.UpdateControls(form.lb_DBCount_3D1Dect, camDetectionVariable.statisticalData.DBSendCount.ToString());

            cameraEnum = CameraEnum.Cam4;
            camDetectionVariable = Project.Instance.cameraManager[cameraEnum].m_CamDetectionVariable;
            NLabel.UpdateControls(form.lb_SumupCount_3D2Defect, camDetectionVariable.statisticalData.SumupCount.ToString());
            NLabel.UpdateControls(form.lb_Yield_3D2efect, camDetectionVariable.statisticalData.Yield.ToString("F2"));
            NLabel.UpdateControls(form.lb_NGCount_3D2efect, camDetectionVariable.statisticalData.NGCount.ToString());
            NLabel.UpdateControls(form.lb_OKCount_3D2efect, camDetectionVariable.statisticalData.OKCount.ToString());
            NLabel.UpdateControls(form.lb_DBCount_3D2Dect, camDetectionVariable.statisticalData.DBSendCount.ToString());


            //初始化数据显示界面
            NListView.Init(form.listView1, BIZ_Variable.DeftProcessTitles.ToArray());
            NListView.Init(form.listView2, BIZ_Variable.SizeProcessTitles.ToArray());
            NListView.Init(form.listView3, BIZ_Variable.DeftProcessTitles.ToArray());
            NListView.Init(form.listView4, BIZ_Variable.SizeProcessTitles.ToArray());


            NListView.Init(form.listView5, BIZ_Variable.Size3DProcessTitles.ToArray());
            NListView.Init(form.listView6, BIZ_Variable.Size3DProcessTitles.ToArray());

            NListView.Init(form.ListView_A, BIZ_Variable.EATitles_A.ToArray());
            NListView.Init(form.ListView_AA, BIZ_Variable.EATitles_A.ToArray());

            NListView.Init(form.ListView_B, BIZ_Variable.EATitles_B.ToArray());
            NListView.Init(form.ListView_BB, BIZ_Variable.EATitles_B.ToArray());

            NListView.Init(form.ListView_AB, BIZ_Variable.EATitles_AB.ToArray());

            NListView.Init(form.listNgInfo, new string[5] { "default", "default", "default", "default", "default" });
            //NListView2.Init(form.listNgInfo, new TitlesInfo[5] { new TitlesInfo(), new TitlesInfo(), new TitlesInfo(), new TitlesInfo(), new TitlesInfo() });

            // NListView.Init(form.ListView_AABB, BIZ_Variable.EATitles_AB.ToArray());

            //  NListView.Init(form.listRunLogView, new string[] { "时间", "运行记录" });

            InitChart();

        }



        public void InitChart()
        {

            chartUIParams?.Clear();
            chartUIPage?.Clear();
            form.tabChartMain.Controls?.Clear();
            form.tabChartControl.Controls?.Clear();
            //初始化图表控件
            foreach (var item in pFTCleanCraft.ListEASizes)
            {
                chartUIParams[item.Name] = new ChartUIParam(item.Description, new ScottPlot.FormsPlot());
            }

            if (pFTCleanCraft.ListEASizes.Count > 0)
            {
                int chartCount = pFTCleanCraft.ListEASizes.Count;


                int rowCount = 2;
                int colCount = 4;

                //判断需要多少页,每页有8个图表
                int pageCount = (chartCount + 7) / 8;

                //tabcontrol先清空
                form.tabChartControl.TabPages.Clear();

                for (int i = 0; i < pageCount; i++)
                {
                    //再添加页
                    form.tabChartControl.TabPages.Add(new TabPage());
                    form.tabChartControl.TabPages[i].Text = $"第{i + 1}页";
                    form.tabChartControl.TabPages[i].BackColor = Color.White;
                    form.tabChartControl.TabPages[i].ForeColor = Color.Black;
                    form.tabChartControl.TabPages[i].Padding = new Padding(10);
                    form.tabChartControl.TabPages[i].AutoScroll = true;
                    form.tabChartControl.TabPages[i].AutoScrollMargin = new Size(10, 10);
                    form.tabChartControl.TabPages[i].AutoScrollMinSize = new Size(100, 100);
                    //form.tabChartControl.TabPages[i].UseWaitCursor = true;
                    form.tabChartControl.TabPages[i].Cursor = Cursors.Hand;
                    form.tabChartControl.TabPages[i].Dock = DockStyle.Fill;
                    //添加tableLayoutPanel,初始化控件rowCount，colCount
                    TableLayoutPanel tableLayoutPanel = new TableLayoutPanel();
                    tableLayoutPanel.RowCount = rowCount;
                    tableLayoutPanel.ColumnCount = colCount;
                    tableLayoutPanel.Dock = DockStyle.Fill;
                    //设置行样式为均分
                    tableLayoutPanel.RowStyles.Clear();
                    for (int j = 0; j < rowCount; j++)
                    {
                        tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F / rowCount));
                    }

                    //设置列样式为均分
                    tableLayoutPanel.ColumnStyles.Clear();
                    for (int j = 0; j < colCount; j++)
                    {
                        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F / colCount));
                    }

                    //添加图表控件到 TableLayoutPanel
                    for (int j = 0; j < 8; j++)
                    {
                        int col = j % colCount;
                        int row = j / colCount;
                        if (i * 8 + j < chartCount)
                        {
                            chartUIParams[pFTCleanCraft.ListEASizes[i * 8 + j].Name].plotView.Dock = DockStyle.Fill;
                            ParamsChart paramsChart = new ParamsChart();
                            paramsChart.f_UpLimit = pFTCleanCraft.ListEASizes[i * 8 + j].StDValue + pFTCleanCraft.ListEASizes[i * 8 + j].StD_Up;
                            paramsChart.f_DownLimit = pFTCleanCraft.ListEASizes[i * 8 + j].StDValue + pFTCleanCraft.ListEASizes[i * 8 + j].StD_Down;
                            paramsChart.f_StandarValue = pFTCleanCraft.ListEASizes[i * 8 + j].StDValue;
                            paramsChart.points.Add(new PointClass() { CurName = "测量值", Point = new Point(0, 0), Color = Color.Blue });
                            chartUIParams[pFTCleanCraft.ListEASizes[i * 8 + j].Name].InitCurve(paramsChart);
                            tableLayoutPanel.Controls.Add(chartUIParams[pFTCleanCraft.ListEASizes[i * 8 + j].Name].plotView, col, row);
                        }
                    }

                    //添加到tabcontrol
                    form.tabChartControl.TabPages[i].Controls.Add(tableLayoutPanel);
                }
            }

            //初始化图表控件
            foreach (var item in pFTCleanCraft.ListEASizes)
            {
                chartUIPage[item.Name] = new ChartUIParam(item.Description, new ScottPlot.FormsPlot());
            }

            if (pFTCleanCraft.ListEASizes.Count > 0)
            {
                int chartCount = pFTCleanCraft.ListEASizes.Count;


                int rowCount = 4;
                int colCount = 2;

                int pageCount = (chartCount + 7) / 8;
                //tabcontrol先清空
                form.tabChartMain.TabPages.Clear();

                for (int i = 0; i < pageCount; i++)
                {

                    //再添加页
                    form.tabChartMain.TabPages.Add(new TabPage());
                    form.tabChartMain.TabPages[i].Text = $"第{i + 1}页";
                    form.tabChartMain.TabPages[i].BackColor = Color.White;
                    form.tabChartMain.TabPages[i].ForeColor = Color.Black;
                    form.tabChartMain.TabPages[i].Padding = new Padding(10);
                    form.tabChartMain.TabPages[i].AutoScroll = true;
                    form.tabChartMain.TabPages[i].AutoScrollMargin = new Size(10, 10);
                    form.tabChartMain.TabPages[i].AutoScrollMinSize = new Size(100, 100);
                    //form.tabChartMain.TabPages[i].UseWaitCursor = true;
                    form.tabChartMain.TabPages[i].Cursor = Cursors.Hand;
                    form.tabChartMain.TabPages[i].Dock = DockStyle.Fill;
                    //添加tableLayoutPanel,初始化控件rowCount，colCount
                    TableLayoutPanel tableLayoutPanel = new TableLayoutPanel();
                    tableLayoutPanel.RowCount = rowCount;
                    tableLayoutPanel.ColumnCount = colCount;
                    tableLayoutPanel.Dock = DockStyle.Fill;
                    //设置行样式为均分
                    tableLayoutPanel.RowStyles.Clear();
                    for (int j = 0; j < rowCount; j++)
                    {
                        tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F / rowCount));
                    }

                    //设置列样式为均分
                    tableLayoutPanel.ColumnStyles.Clear();
                    for (int j = 0; j < colCount; j++)
                    {
                        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F / colCount));
                    }

                    //添加图表控件到 TableLayoutPanel
                    for (int j = 0; j < 8; j++)
                    {
                        int col = j % colCount;
                        int row = j / colCount;
                        if (i * 8 + j < chartCount)
                        {
                            chartUIPage[pFTCleanCraft.ListEASizes[i * 8 + j].Name].plotView.Dock = DockStyle.Fill;
                            ParamsChart paramsChart = new ParamsChart();
                            paramsChart.f_UpLimit = pFTCleanCraft.ListEASizes[i * 8 + j].StDValue + pFTCleanCraft.ListEASizes[i * 8 + j].StD_Up;
                            paramsChart.f_DownLimit = pFTCleanCraft.ListEASizes[i * 8 + j].StDValue + pFTCleanCraft.ListEASizes[i * 8 + j].StD_Down;
                            paramsChart.f_StandarValue = pFTCleanCraft.ListEASizes[i * 8 + j].StDValue;
                            paramsChart.points.Add(new PointClass() { CurName = "测量值", Point = new Point(0, 0), Color = Color.Blue });
                            chartUIPage[pFTCleanCraft.ListEASizes[i * 8 + j].Name].InitCurve(paramsChart);
                            tableLayoutPanel.Controls.Add(chartUIPage[pFTCleanCraft.ListEASizes[i * 8 + j].Name].plotView, col, row);
                        }
                    }

                    //添加到tabcontrol
                    form.tabChartMain.TabPages[i].Controls.Add(tableLayoutPanel);



                }


            }

        }


        public void UpdateChart()
        {

        }

        /// <summary>
        /// 曲线图数据显示
        /// </summary>
        /// <param name="chartUIParam"></param>
        /// <param name="data"></param>

        public void ChartDataShow(ChartUIParam chartUIParam, double data)
        {
            try
            {
                ParamsChart paramsChart = new ParamsChart();
                PointClass point = new PointClass();
                point.CurName = "测量值";
                point.pointInfo = new PointInfo() { Col = 0, Row = data };
                paramsChart.points.Add(point);

                chartUIParam.UpDataCurve(paramsChart);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(chartUIParam.strChartName, ex);
            }

        }

        public void InitVariable()
        {
            BIZ_Variable.ListEASizes?.Clear();
            BIZ_Variable.ListEASizes = new List<SizeItemInfo>();
            //初始化尺寸数据
            foreach (var item in pFTCleanCraft.ListEASizes)
            {
                SizeItemInfo sizeItemInfo = new SizeItemInfo();
                sizeItemInfo.Description = item.Description;
                sizeItemInfo.Name = item.Name;
                sizeItemInfo.camera = item.camera;
                BIZ_Variable.ListEASizes.Add(sizeItemInfo);

            }


            BIZ_Variable.ListDefectInfos?.Clear();
            BIZ_Variable.ListDefectInfos = new List<DefectItemInfo>();


            //初始化缺陷数据
            foreach (var item in pFTCleanCraft.ListDefectInfos)
            {
                DefectItemInfo defectItemInfo = new DefectItemInfo();
                defectItemInfo.Description = item.Description;
                defectItemInfo.Name = item.Name;
                defectItemInfo.camera = item.camera;
                BIZ_Variable.ListDefectInfos.Add(defectItemInfo);

            }

            foreach (var item in Project.Instance.cameraManager)
            {
                string ProdDataPath = item.Value.m_CamDetectionVariable.ProdDataPath;
                string ProdSizeDataPath = item.Value.m_CamDetectionVariable.ProdSizeDataPath;
                string ProdDeftDataPath = item.Value.m_CamDetectionVariable.ProdDeftDataPath;
                string ProdEADataPath = item.Value.m_CamDetectionVariable.ProdEADataPath;

                item.Value.m_CamDetectionVariable = new CamDetectionVariable();
                item.Value.m_CamDetectionVariable.ProdDataPath = ProdDataPath;


                item.Value.m_CamDetectionVariable.ProdSizeDataPath = ProdSizeDataPath;

                item.Value.m_CamDetectionVariable.ProdDeftDataPath = ProdDeftDataPath;


                item.Value.m_CamDetectionVariable.ProdEADataPath = ProdEADataPath;
            }


            BIZ_Variable.SizeProcessTitles = new List<string>
              {
                  "时间",
                  "帧号",
                  "EA号",
                  "尺寸项目",
                  "结果判断",
                  "当前尺寸数据"
              };




            BIZ_Variable.DeftProcessTitles = new List<string>
              {
                  "时间",
                  "帧号",
                  "EA号",
                  "缺陷类型",
                  "结果判断",
                  "缺陷信息",

              };


            BIZ_Variable.Size3DProcessTitles = new List<string>
              {
                  "时间",
                  "帧号",
                  "深度",
                  "结果判断"
              };


            //EA数据表格
            BIZ_Variable.EATitles_A = new List<string>() { "时间", "EA索引" };
            BIZ_Variable.EATitles_B = new List<string>() { "时间", "EA索引" };
            BIZ_Variable.sizeItemInfos_A = new List<SizeItemInfo>();
            BIZ_Variable.sizeItemInfos_B = new List<SizeItemInfo>();
            BIZ_Variable.sizeItemInfos_C = new List<SizeItemInfo>();
            BIZ_Variable.sizeItemInfos_D = new List<SizeItemInfo>();
            for (int i = 0; i < pFTCleanCraft.ListEASizes.Count; i++)
            {
                if (pFTCleanCraft.ListEASizes[i].camera == CameraEnum.Cam1 && pFTCleanCraft.ListEASizes[i].Enable)
                {
                    BIZ_Variable.EATitles_A.Add(pFTCleanCraft.ListEASizes[i].Description);
                    BIZ_Variable.EATitles_A.Add("结果");
                    SizeItemInfo sizeItemInfo = pFTCleanCraft.ListEASizes[i].DeepClone();
                    sizeItemInfo.CountNGDB = 0;
                    BIZ_Variable.sizeItemInfos_A.Add(sizeItemInfo);
                }
                if (pFTCleanCraft.ListEASizes[i].camera == CameraEnum.Cam2 && pFTCleanCraft.ListEASizes[i].Enable)
                {
                    BIZ_Variable.EATitles_B.Add(pFTCleanCraft.ListEASizes[i].Description);
                    BIZ_Variable.EATitles_B.Add("结果");
                    SizeItemInfo sizeItemInfo = pFTCleanCraft.ListEASizes[i].DeepClone();
                    sizeItemInfo.CountNGDB = 0;
                    BIZ_Variable.sizeItemInfos_B.Add(sizeItemInfo);
                }

                if (pFTCleanCraft.ListEASizes[i].camera == CameraEnum.Cam3 && pFTCleanCraft.ListEASizes[i].Enable)
                {
                    SizeItemInfo sizeItemInfo = pFTCleanCraft.ListEASizes[i].DeepClone();
                    sizeItemInfo.CountNGDB = 0;
                    BIZ_Variable.sizeItemInfos_C.Add(sizeItemInfo);
                }

                if (pFTCleanCraft.ListEASizes[i].camera == CameraEnum.Cam4 && pFTCleanCraft.ListEASizes[i].Enable)
                {
                    SizeItemInfo sizeItemInfo = pFTCleanCraft.ListEASizes[i].DeepClone();
                    sizeItemInfo.CountNGDB = 0;
                    BIZ_Variable.sizeItemInfos_D.Add(sizeItemInfo);
                }

            }

            BIZ_Variable.EATitles_A.Add("缺陷结果");
            BIZ_Variable.EATitles_A.Add("总结果");
            BIZ_Variable.EATitles_A.Add("缺陷信息汇总");
            BIZ_Variable.EATitles_B.Add("缺陷结果");
            BIZ_Variable.EATitles_B.Add("总结果");
            BIZ_Variable.EATitles_B.Add("缺陷信息汇总");


            BIZ_Variable.defectItemInfos_A = new List<DefectItemInfo>();
            BIZ_Variable.defectItemInfos_B = new List<DefectItemInfo>();
            for (int i = 0; i < pFTCleanCraft.ListDefectInfos.Count; i++)
            {
                if (pFTCleanCraft.ListDefectInfos[i].camera == CameraEnum.Cam1 && pFTCleanCraft.ListDefectInfos[i].Enable)
                {
                    var defectItemInfo = pFTCleanCraft.ListDefectInfos[i].DeepClone();
                    defectItemInfo.CountNGDB = 0;
                    BIZ_Variable.defectItemInfos_A.Add(defectItemInfo);

                }

                if (pFTCleanCraft.ListDefectInfos[i].camera == CameraEnum.Cam2 && pFTCleanCraft.ListDefectInfos[i].Enable)
                {
                    var defectItemInfo = pFTCleanCraft.ListDefectInfos[i].DeepClone();
                    defectItemInfo.CountNGDB = 0;
                    BIZ_Variable.defectItemInfos_B.Add(defectItemInfo);

                }
            }


            BIZ_Variable.EATitles_AB = new List<string>() { "时间", "EA索引" };

            for (int i = 0; i < pFTCleanCraft.ListEASizes.Count; i++)
            {
                if (pFTCleanCraft.ListEASizes[i].Enable)
                {
                    BIZ_Variable.EATitles_AB.Add(pFTCleanCraft.ListEASizes[i].Description);
                    BIZ_Variable.EATitles_AB.Add("结果");
                }


            }


            BIZ_Variable.EATitles_AB.Add("缺陷结果");
            BIZ_Variable.EATitles_AB.Add("总结果");
            BIZ_Variable.EATitles_AB.Add("缺陷信息汇总");

            //foreach (var item in stackEACraft.cameras)
        }

        public void SendAlarm(int index)
        {
            //if (Project.Instance.cutOverlayPLCManage != null && Project.Instance.cutOverlayPLCManage.InitState == true)
            //{
            //    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_CorrectAlarm_CCDPFT.Value = 1;
            //    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_CorrectAlarm_CCDPFT, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
            //}
            //throw new NotImplementedException();
        }

        public void SendStopAlarm(int index)
        {
            if (Project.Instance.cutOverlayPLCManage != null && Project.Instance.cutOverlayPLCManage.InitState == true)
            {
                Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_CorrectAlarm_CCDPFT.Value = 1;
                Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_CorrectAlarm_CCDPFT, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
            }
            // throw new NotImplementedException();
        }

        public void SimLoadImage(bool simAutoFlag, ref bool simRunFlag)
        {
            try
            {
                if (simRunFlag)
                {
                    Parallel.ForEach(Project.Instance.baseProDevice.m_DicCamOperaParam_Sim, (item) =>
                    {
                        CameraEnum cameraEnum = (CameraEnum)item.Key;
                        bool flag = false;

                        if (cameraEnum == CameraEnum.Cam1 || cameraEnum == CameraEnum.Cam2)
                        {

                            flag = SimControl.LoadImage_Sim(Project.Instance.cameraManager[cameraEnum].m_ImageSimManage,
                                      ref item.Value.QueueGrabImage, $"{(cameraEnum).ToString()}仿真加载图片");
                        }

                        if (flag)
                        {
                            (item.Value as PFTCleanProParams).CallBackImgDataDeal();
                        }

                    });
                    Parallel.ForEach(Project.Instance.baseProDevice.m_DicCamOperaParam_Sim, (item) =>
                    {
                        CameraEnum cameraEnum = (CameraEnum)item.Key;
                        bool flag = false;
                        if (cameraEnum == CameraEnum.Cam3 || cameraEnum == CameraEnum.Cam4)
                        {
                            flag = SimControl.LoadImage_Sim3D(Project.Instance.cameraManager[(CameraEnum)item.Key].m_ImageSimManage,
                                             ref item.Value.Queue3DImage, $"{((CameraEnum)item.Key).ToString()}仿真加载图片");

                        }
                        if (flag)
                        {
                            (item.Value as PFTCleanProParams).CallBackImgDataDeal();
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

        public void StartMojuan(string strMojuanPath)
        {
            //发送打标机软件新建膜卷信号
            if (Project.Instance.dbjCardManager != null)
            {
                Project.Instance.dbjCardManager.SendXJMQ(strMojuanPath);
            }

        }

        public void StartRun()
        {
           
            //打标机发送运行信号
            if (Project.Instance.dbjCardManager != null)
            {
                //发送打标机软件图像长度
                double imageLength = pFTCleanCraft.EASizeMD;
                Project.Instance.dbjCardManager.SendImageLength(imageLength);
                Project.Instance.dbjCardManager.SendRun(true);

            }

            if (Project.Instance.cutOverlayPLCManage != null && Project.Instance.cutOverlayPLCManage.InitState == true)
            {
                //运行发送使能信号
                Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_SendCCDEnable_CCDPFT.Value = 1;
                Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_SendCCDEnable_CCDPFT, true, Project.Instance.cutOverlayPLCManage.HeartbeatLog);
            }


            //throw new NotImplementedException();
        }

        public void StopMojuan()
        {
            // throw new NotImplementedException();
        }

        public void StopRun()
        {
            //打标机发送暂停信号
            if (Project.Instance.dbjCardManager != null) Project.Instance.dbjCardManager.SendRun(false);
            // throw new NotImplementedException();

            if (Project.Instance.cutOverlayPLCManage != null && Project.Instance.cutOverlayPLCManage.InitState == true)
            {
                //运行发送使能信号
                Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_SendCCDEnable_CCDPFT.Value = 0;
                Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_SendCCDEnable_CCDPFT, true, Project.Instance.cutOverlayPLCManage.HeartbeatLog);
            }
        }



        public void UpdateForm()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }



        /// <summary>
        /// 尺寸NG判断
        /// </summary>
        /// <param name="sizeItemInfo"></param>
        /// <param name="sizeInfo"></param>
        public void GetSizeJudgeInfo(SizeItemInfo sizeItemInfo, ref SizeInfo sizeInfo)
        {
            // 初始化结果类型和报警状态
            sizeInfo.IsNG = true;
            sizeInfo.resultType = ResultType.Warn;

            // 检查是否在标准范围内
            bool isInStandardRange = sizeInfo.DataResult.Between(sizeItemInfo.StDValue + sizeItemInfo.StD_Down, sizeItemInfo.StDValue + sizeItemInfo.StD_Up);

            if (isInStandardRange)
            {
                sizeInfo.resultType = ResultType.OK;
                sizeInfo.IsNG = false;
            }
            else
            {
                // 设置报警信息
                if (sizeItemInfo.DB)
                {
                    SetAlarmInfo(sizeItemInfo, sizeInfo);
                }
            }

            // 检查停机条件并设置相关信息
            if (sizeItemInfo.TJ)
            {
                CheckTJCondition(sizeItemInfo, sizeInfo);
            }

            // 检查报警条件并设置相关信息
            if (sizeItemInfo.BJ)
            {
                CheckBJCondition(sizeItemInfo, sizeInfo);
            }
        }


        private void SetAlarmInfo(SizeItemInfo sizeItemInfo, SizeInfo sizeInfo)
        {
            sizeInfo.DBInfo.IsAlarm = true;
            sizeInfo.DBInfo.UpTolerance = sizeItemInfo.StDValue + sizeItemInfo.StD_Up;
            sizeInfo.DBInfo.DownTolerance = sizeItemInfo.StDValue + sizeItemInfo.StD_Down;
            if (!sizeInfo.DataResult.Between(sizeInfo.DBInfo.DownTolerance, sizeInfo.DBInfo.UpTolerance))
            {
                sizeInfo.resultType = ResultType.NG;
            }
        }



        private void CheckBJCondition(SizeItemInfo sizeItemInfo, SizeInfo sizeInfo)
        {
            bool isInBJRange = sizeInfo.DataResult >= (sizeItemInfo.StDValue + sizeItemInfo.StD_BJ_Down) &&
                               sizeInfo.DataResult <= (sizeItemInfo.StDValue + sizeItemInfo.StD_BJ_Up);

            if (!isInBJRange)
            {
                sizeInfo.BJInfo.IsAlarm = true;
                sizeInfo.BJInfo.UpTolerance = sizeItemInfo.StDValue + sizeItemInfo.StD_BJ_Up;
                sizeInfo.BJInfo.DownTolerance = sizeItemInfo.StDValue + sizeItemInfo.StD_BJ_Down;
            }
        }

        private void CheckTJCondition(SizeItemInfo sizeItemInfo, SizeInfo sizeInfo)
        {
            bool isInTJRange = sizeInfo.DataResult >= (sizeItemInfo.StDValue + sizeItemInfo.StD_TJ_Down) &&
                               sizeInfo.DataResult <= (sizeItemInfo.StDValue + sizeItemInfo.StD_TJ_Up);

            if (!isInTJRange)
            {
                sizeInfo.TJInfo.IsAlarm = true;
                sizeInfo.TJInfo.UpTolerance = sizeItemInfo.StDValue + sizeItemInfo.StD_TJ_Up;
                sizeInfo.TJInfo.DownTolerance = sizeItemInfo.StDValue + sizeItemInfo.StD_TJ_Down;
            }
        }



    }
}

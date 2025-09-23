using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Markup;

using AntdUI;



using Extensions;

using Frame_Core;

using HalconDotNet;

using HslCommunication.Profinet.OpenProtocol;

using LogTool;

using Newtonsoft.Json;

using NPOI.SS.Formula.Functions;



namespace Mef_VisionFrame
{
    public partial class BIZ_DataWorker_WidthLaser : IBizWorkerBase
    {

        BIZ_ThreadManage_WidthLaser bIZ_ThreadManage = new BIZ_ThreadManage_WidthLaser();
        public AThreadManageBase ThreadManage { get; set; }

        public BIZ_Variable_WidthLaser BIZ_Variable = new BIZ_Variable_WidthLaser();
        Form_WidthLaserDetect form;
        PFTCleanCraft pFTCleanCraft;

        public DisplayWindowClass HWC_InitCam1;
        public DisplayWindowClass HWC_InitCam2;
        public DisplayWindowClass HWC_EACam1;
        public DisplayWindowClass HWC_EACam2;


        public DisplayWindowClass DisplayWindowClass1;
        public DisplayWindowClass DisplayWindowClass2;
        public DisplayWindowClass DisplayWindowClass3;
        public DisplayWindowClass DisplayWindowClass4;
        public DisplayWindowClass DisplayWindowClass5;
        public DisplayWindowClass DisplayWindowClass6;

        List<Dictionary<string, ChartUIParam>> ChartUIPage = new List<Dictionary<string, ChartUIParam>>();
        ConcurrentDictionary<int, ChartUIParam> ChartConfigPage = new ConcurrentDictionary<int, ChartUIParam>();
        ConcurrentDictionary<int, ChartUIParam> ChartGroovePage = new ConcurrentDictionary<int, ChartUIParam>();



        CombinationHWindowControl CombinationHWindowControl_cam1;
        CombinationHWindowControl CombinationHWindowControl_cam2;
        IntervalExecution intervalExecution = new IntervalExecution();
        IntervalExecution intervalExecutionStas = new IntervalExecution();

        public BIZ_DataWorker_WidthLaser(Form_WidthLaserDetect _form)
        {
            //先有工艺变量，再定内部变量，再初始化窗体
            form = _form;

            UpdateCraft();
            HWC_InitCam1 = new DisplayWindowClass(form.FrontInitHalconView1);
            HWC_InitCam2 = new DisplayWindowClass(form.FrontInitHalconView2);
            HWC_EACam1 = new DisplayWindowClass(form.FrontEAHalconView1);
            HWC_EACam2 = new DisplayWindowClass(form.FrontEAHalconView2);
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
        }


        public void ProcessingData_FrameCam1(ProductBase camEAProduct)
        {

            ExecuteMethod(() =>
            {
                CameraEnum cameraEnum = CameraEnum.Cam1;
                double Calipix_x = Project.Instance.cameraManager[cameraEnum].m_CamParam.CalibrationValue_X;
                double Calipix_y = Project.Instance.cameraManager[cameraEnum].m_CamParam.CalibrationValue_Y;

                PFTImageProduct pFTImageProduct = camEAProduct as PFTImageProduct;
                //原图处理
                FrameMethord(cameraEnum, pFTImageProduct, form.DpModelDetectProcessCam1, HWC_InitCam1, BIZ_Variable.strDeftInitCam1Title.ToArray(), BIZ_Variable.strDeftInitCam1Path);

                int count = pFTImageProduct.showHalconWindow.m_hImage.GetReferenceCount();
                MessageNotify.AddDebugLog($"Cam1_原图数据处理完成，当前图像ID{pFTImageProduct.nCurImageID}，引用计数为{count}");

            });



        }




        public void ProcessingData_FrameCam2(ProductBase camEAProduct)
        {
            ExecuteMethod(() =>
            {
                CameraEnum cameraEnum = CameraEnum.Cam2;
                ConcurrentDictionary<int, PFTCleanEAProductInfo> dic_EAproductsInfo_A = Project.Instance.Is_simulation ? BIZ_Variable.dic_EAproductsInfo_SimA : BIZ_Variable.dic_EAproductsInfo_A;
                ConcurrentDictionary<int, PFTCleanEAProductInfo> dic_EAproductsInfo_B = Project.Instance.Is_simulation ? BIZ_Variable.dic_EAproductsInfo_SimB : BIZ_Variable.dic_EAproductsInfo_B;
                double Calipix_x = Project.Instance.cameraManager[cameraEnum].m_CamParam.CalibrationValue_X;
                double Calipix_y = Project.Instance.cameraManager[cameraEnum].m_CamParam.CalibrationValue_Y;

                PFTImageProduct pFTImageProduct = camEAProduct as PFTImageProduct;


                //原图处理
                FrameMethord(cameraEnum, pFTImageProduct, form.DpModelDetectProcessCam2, HWC_InitCam2, BIZ_Variable.strDeftInitCam2Title.ToArray(), BIZ_Variable.strDeftInitCam2Path);
                // EA判断处理
                // 找到未完成EA处理且已完成切片的产品
                // 获取对应的产品信息
                BIZ_Variable.queueImgProductsDisposeAB.Enqueue(pFTImageProduct);

                //这里作为延迟一帧再做数据合并，避免数据不一致
                if (BIZ_Variable.queueImgProductsDisposeAB.Count >1)
                {
                    PFTImageProduct pFT = BIZ_Variable.queueImgProductsDisposeAB.Dequeue();
                    if (pFT.blSplitEA)
                    {


                        int keyA = pFT.nCurEAID;
                        if (dic_EAproductsInfo_A.TryGetValue(keyA, out PFTCleanEAProductInfo productInfoA) &&
                        dic_EAproductsInfo_B.TryGetValue(keyA, out PFTCleanEAProductInfo productInfoB))
                        {
                            bIZ_ThreadManage.limitThreadPool.RunAction(() =>
                            {
                                int InnerkeyA = productInfoA.EAID;
                                //数据来源分析
                                EADataAnalysis(cameraEnum, Calipix_x, Calipix_y, productInfoA, productInfoB, out List<string> OutDBChannel);
                                //发送打标数据
                                SendDBInfoIfNeeded(InnerkeyA, productInfoA.EArange.StartImageID, productInfoA.resultSliceAllResult, ((SlaveMachineParam)Project.Instance.slaveMachineManager.tcpClientParam).nCamDBIndex);
                                UIAndFileDataSave(InnerkeyA, OutDBChannel, productInfoA, productInfoB);
                                
                                if(Project.Instance.slaveMachineManager.initState)
                                {
                                    //转成josn发送
                                    List< StatisticalData > StatisticalData_EASlices = new List< StatisticalData >();
                                    for(int i=0;i< BIZ_Variable.StatisticalData_EASlices.Count;i++)
                                    {
                                        StatisticalData_EASlices.Add(BIZ_Variable.StatisticalData_EASlices[i].statisticalData);
                                    }
                                    string jsonData = JsonConvert.SerializeObject(StatisticalData_EASlices);
                                    Project.Instance.slaveMachineManager.SendDataSum(jsonData);
                                }

                            });
                          

                        }
                        else
                        {

                            // 报警：未找到对应的产品信息
                            MessageNotify.AddErrorLog("ProcessingData_FrameCam2()_未找到对应的产品信息");
                            MessageNotify.AddStopAlarm(new StopAlarmMessage() { time = DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss"), StopAlarmState = StopAlarmStateEnum.停机, name = "未找到对应的产品信息", describe = "未找到对应的产品信息" });
                        }


                    }
                }

            });
        }





        public void ProcessingData_EACam1(ProductBase camEAProduct)
        {
            ExecuteMethod(() =>
            {
                CameraEnum cameraEnum = CameraEnum.Cam1;
                ConcurrentDictionary<int, PFTCleanEAProductInfo> dic_EAproductsInfo_A = Project.Instance.Is_simulation ? BIZ_Variable.dic_EAproductsInfo_SimA : BIZ_Variable.dic_EAproductsInfo_A;
                HOperatorSet.CountSeconds(out camEAProduct.Image_StartDataProcessTime);
                PFTCleanEAProductInfo pFTCleanEAProductInfo = camEAProduct as PFTCleanEAProductInfo;

                MessageNotify.AddDebugLog($"Cam1_EA数据刚开始，EAID:{pFTCleanEAProductInfo.EAID}");
                EAFrameMethord(cameraEnum, pFTCleanEAProductInfo, HWC_EACam1);

                int EAID = pFTCleanEAProductInfo.EAID;
                if (dic_EAproductsInfo_A.TryRemove(pFTCleanEAProductInfo.EAID - 1, out pFTCleanEAProductInfo))
                {
                    //获取所有keys
                    var keys = dic_EAproductsInfo_A.Keys.ToList();
                    MessageNotify.AddDebugLog($"Cam1_EA数据处理完成，EAID:{pFTCleanEAProductInfo.EAID},将其移入释放队列,字典含有keys为：{string.Join(",",keys)}");
                    BIZ_Variable.queueProductsDispose_A.Enqueue(pFTCleanEAProductInfo);
                }
                else
                {
                    MessageNotify.AddDebugLog($"Cam1_EA数据未达到移入队列条件，EAID为{EAID}");

                }

                if (BIZ_Variable.queueProductsDispose_A?.Count > 0)
                {
                    PFTCleanEAProductInfo pFT = BIZ_Variable.queueProductsDispose_A.Dequeue();
                    MessageNotify.AddDebugLog($"释放队列A中达到释放条件，EAID:{pFT.EAID},将其释放");
                    pFT?.Dispose();

                }



            });
        }

        public void ProcessingData_EACam2(ProductBase camEAProduct)
        {
            ExecuteMethod(() =>
            {
                CameraEnum cameraEnum = CameraEnum.Cam2;
                ConcurrentDictionary<int, PFTCleanEAProductInfo> dic_EAproductsInfo_A = Project.Instance.Is_simulation ? BIZ_Variable.dic_EAproductsInfo_SimA : BIZ_Variable.dic_EAproductsInfo_A;
                ConcurrentDictionary<int, PFTCleanEAProductInfo> dic_EAproductsInfo_B = Project.Instance.Is_simulation ? BIZ_Variable.dic_EAproductsInfo_SimB : BIZ_Variable.dic_EAproductsInfo_B;
                HOperatorSet.CountSeconds(out camEAProduct.Image_StartDataProcessTime);
                PFTCleanEAProductInfo pFTCleanEAProductInfo = camEAProduct as PFTCleanEAProductInfo;
                MessageNotify.AddDebugLog($"Cam2_EA数据刚开始，EAID:{pFTCleanEAProductInfo.EAID}");
                EAFrameMethord(cameraEnum, pFTCleanEAProductInfo, HWC_EACam2);

                int EAID = pFTCleanEAProductInfo.EAID;
                if (dic_EAproductsInfo_B.TryRemove(pFTCleanEAProductInfo.EAID - 1, out pFTCleanEAProductInfo))
                {
                    //获取所有keys
                    var keys = dic_EAproductsInfo_B.Keys.ToList();
                    BIZ_Variable.queueProductsDispose_B.Enqueue(pFTCleanEAProductInfo);
                    MessageNotify.AddDebugLog($"Cam2_EA数据处理完成，EAID:{pFTCleanEAProductInfo.EAID},将其移入释放队列,字典含有keys为：{string.Join(",", keys)}");
                }
                else
                {
                    MessageNotify.AddDebugLog($"Cam2_EA数据未达到移入队列条件，EAID为{EAID}");

                }

                if (BIZ_Variable.queueProductsDispose_B?.Count > 0)
                {
                    PFTCleanEAProductInfo pFT = BIZ_Variable.queueProductsDispose_B.Dequeue();
                    pFT?.Dispose();
                    MessageNotify.AddDebugLog($"释放队列B中达到释放条件，EAID:{pFT.EAID},将其释放");

                }



            });
        }

        public void Communication()
        {
            try
            {

                bool IsIntervalTime = intervalExecution.GetWhetherToReachIntervalTime(30000);
                //=========================================================================
     
                //读取主机通讯信息
                if (Project.Instance.slaveMachineManager != null && Project.Instance.slaveMachineManager.initState == true)
                {
                    if (IsIntervalTime)
                    {
                        Project.Instance.slaveMachineManager.SendData();
                    }

                    Project.Instance.slaveMachineManager.ReadData();

                }


            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }

        public void AlarmReset()
        {
            // throw new NotImplementedException();
        }

        public void ClearForm()
        {
            InitForm();
            InitChart();
            InitGrooveChart();
        }



        public void DisposeImageAndClearImageQueue()
        {
            HWC_InitCam1?.Dispose();
            HWC_InitCam2?.Dispose();
            HWC_EACam1?.Dispose();
            HWC_EACam2?.Dispose();

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


                UIService.Instance.AddAction(() => { BIZ_General.Image_Process(HWC_InitCam1.operation_Image, disposeImageRetainLenght, HWC_InitCam1); });
                UIService.Instance.AddAction(() => { BIZ_General.Image_Process(HWC_InitCam2.operation_Image, disposeImageRetainLenght, HWC_InitCam2); });

                BIZ_General.Image_Process(HWC_EACam1.operation_Image, 1, HWC_EACam1);
                BIZ_General.Image_Process(HWC_EACam2.operation_Image, 1, HWC_EACam2);


                if (true)
                {
                    Operation_ImageProcessor processor = DisplayWindowClass1.operation_Image;
                    if (processor.imageInfosShow.Count > 0)
                    {
                        DisplayWindowClass displayWindowClass = CombinationHWindowControl_cam1.GetDisplayWindowClass();

                        if (processor != null && processor.imageInfosShow != null && processor.imageInfosShow.Count > 0)
                            processor.ShowImagesAsync(processor.imageInfosShow, displayWindowClass, processor.imageInfosSave, processor.imageInfosScreenshotSave);
                    }
                    BIZ_General.Image_Process(processor, 5);
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
                    BIZ_General.Image_Process(processor, 5);
                }

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }

        public void InitForm()
        {
            // 获取当前类名和方法名
            string className = "";
            string methodName = "";
            try
            {
                className = MethodBase.GetCurrentMethod().DeclaringType?.FullName ?? "UnknownClass";
                methodName = MethodBase.GetCurrentMethod().Name;
                form.CobSelectedIndexChangedAction -= CobSelectChange;
                form.CobSelectedIndexChangedAction += CobSelectChange;
                form.OutDataAct -= OutData;
                form.OutDataAct += OutData;
                form.SetGrooveChart(pFTCleanCraft.SliceCount, pFTCleanCraft.ListSliceSet?.Select(d => d.ShaveNum).ToArray());
                form.Invoke(new Action(() =>
                {
                    NDataGridView.InitGrid(form.Dgv_StatsEA, BIZ_Variable.DgvColumnheaders, BIZ_Variable.DgvRowheaders);
                    NLabel.UpdateControls(form.LB_Formula, Project.Instance.systemInfo.CurrentCraft);
                    NLabel.UpdateControls(form.LB_DetectionType, LanguageProject.GetLangeuageAttribute(Project.Instance.craftBase.materialCrafEnum, LanguageEnum.Chinese));

                    NDataPanelViewModel.Init(form.DpModelDetectProcessCam1, BIZ_Variable.strDeftInitCam1Title.ToArray(), 20, 20);
                    NDataPanelViewModel.Init(form.DpModelDetectProcessCam2, BIZ_Variable.strDeftInitCam2Title.ToArray(), 20, 20);
                    NDataPanelViewModel.Init(form.DpModelSizeConfig, BIZ_Variable.strEASizeConfigTitle.ToArray(), 20, 20);
                    NDataPanelViewModel.Init(form.DpModelEASize, BIZ_Variable.strEASizeTitle.ToArray(), 20, 20);
                    NDataPanelViewModel.Init(form.DpModelEADetect, BIZ_Variable.strEADeftTitle.ToArray(), 20, 20);
                    NDataPanelViewModel.Init(form.DpModelEADetectSize, BIZ_Variable.strEASizeDeftTitle.ToArray(), 20, 20);
                    string[] strings = new string[50];
                    for (int i = 0; i < 50; i++)
                    {
                        strings[i] = "default";
                    }
                    NListView.Init(form.listNgInfo, strings);

                    InitChart();
                    InitGrooveChart();

                }));
            }
            catch (Exception ex)
            {
                // 构造完整的异常信息，包含类名、方法名和异常详情
                string fullErrorMessage = $"[{className}.{methodName}] 发生异常: {ex.ToString()}";

                // 输出带调用堆栈的详细错误信息
                MessageNotify.AddErrorLog(fullErrorMessage);
            }



        }

        public void InitVariable()
        {

            ExecuteMethod(() =>
            {
                BIZ_Variable.DgvColumnheaders?.Clear();
                BIZ_Variable.DgvRowheaders?.Clear();
                BIZ_Variable.queueImgProductsDisposeAB?.Clear();
                BIZ_Variable.StatisticalData_EASlices?.Clear();
                BIZ_Variable.DgvColumnheaders = new List<string>();
                BIZ_Variable.DgvRowheaders = new List<string> { "产量", "良率", "OK数", "NG数", "DB数" };
                BIZ_Variable.DgvColumnheaders.Add("条数/统计");
                BIZ_Variable.strDeftInitCam1Title = new List<string>() { "时间", "帧号", "EA号", "条号", "NG类型", "宽度","高度","面积","是否保存","是否打标","路径"};
                BIZ_Variable.strDeftInitCam1Path = Project.Instance.paramPathSet.ProductMojuanPath + $"{Project.Instance.cameraManager[CameraEnum.Cam1].m_CamParam.CameraName}_原图缺项检测表.csv";
                BIZ_Variable.strDeftInitCam2Title = new List<string>() { "时间", "帧号", "EA号", "条号", "NG类型", "宽度", "高度", "面积", "是否保存", "是否打标", "路径" };
                BIZ_Variable.strDeftInitCam2Path = Project.Instance.paramPathSet.ProductMojuanPath + $"{Project.Instance.cameraManager[CameraEnum.Cam2].m_CamParam.CameraName}_原图缺项检测表.csv";
                BIZ_Variable.strEASizeConfigTitle = new List<string>() { "时间", "EA号" };
                BIZ_Variable.strEASizeConfigPath = Project.Instance.paramPathSet.ProductMojuanPath + $"配置文件总表.csv";

                BIZ_Variable.strEASizeTitle = new List<string>() { "时间", "EA号", "条号" };
                BIZ_Variable.strEASizePath = Project.Instance.paramPathSet.ProductMojuanPath + $"双相机槽位尺寸汇总表.csv";
                BIZ_Variable.strEADeftTitle = new List<string>() { "时间", "EA号" };
                BIZ_Variable.strEADeftPath = Project.Instance.paramPathSet.ProductMojuanPath + $"双相机缺项汇总表.csv";
                BIZ_Variable.strEASizeDeftTitle = new List<string>() { "时间", "EA号" };
                BIZ_Variable.strEASizeDeftPath = Project.Instance.paramPathSet.ProductMojuanPath + $"双相机尺寸缺项汇总表.csv";
                BIZ_Variable.strProductDataPath = Project.Instance.paramPathSet.ProductMojuanPath + $"生产统计文件.csv";

                ChartUIPage?.Clear();
                ChartUIPage = new List<Dictionary<string, ChartUIParam>>();
                for (int i = 0; i < pFTCleanCraft.SliceCount; i++)
                {
                    BIZ_Variable.DgvColumnheaders.Add("Slice" + (i + 1).ToString());
                }

                // 初始化统计数据结构
                BIZ_Variable.StatisticalData_EASlices = InitializeStatisticalData(
                    BIZ_Variable.StatisticalData_EASlices,
                    pFTCleanCraft
                );

                //EA尺寸标题

                string str = "配置尺寸总结果";

                if(pFTCleanCraft.ListSlotSizeLimits?.Count>0)
                {
                    for (int i = 0; i < pFTCleanCraft.ListSliceSet?.Select(d => d.ShaveNum).ToArray().Max(); i++)
                    {
                        // 获取测量槽位的枚举名称（优化性能）
                       // var slotNames = Enum.GetNames(typeof(MeasureSlotEnum));


                        foreach(var slot in pFTCleanCraft.ListSlotSizeLimits)
                        {
                            if(slot.Enable)
                            {
                                str = $"(孔区{i + 1})_{slot.Description}";
                                BIZ_Variable.strEASizeTitle.Add(str);
                                str = $"(孔区{i + 1})_{slot.Description}_结果";
                                BIZ_Variable.strEASizeTitle.Add(str);
                            }
                        }
                    }
                }


            

                str = "槽位总结果";
                BIZ_Variable.strEASizeTitle.Add(str);
                str = "配置尺寸总结果";
                BIZ_Variable.strEASizeTitle.Add(str);
                str = "总结果";
                BIZ_Variable.strEASizeTitle.Add(str);


                //EA缺陷标题
                for (int i = 0; i < BIZ_Variable.StatisticalData_EASlices.Count; i++)
                {
                    str = $"S{i + 1}_总结果";
                    BIZ_Variable.strEADeftTitle.Add(str);
                    //str = "当前缺陷信息";
                    //BIZ_Variable.strEADeftTitle.Add(str);
                }
                str = "总结果";
                //EA尺寸缺陷标题
                BIZ_Variable.strEADeftTitle.Add(str);


                for (int i = 0; i < BIZ_Variable.StatisticalData_EASlices.Count; i++)
                {
                    str = $"S{i + 1}_当前条尺寸结果";
                    BIZ_Variable.strEASizeDeftTitle.Add(str);
                    str = $"S{i + 1}_当前条缺陷结果";
                    BIZ_Variable.strEASizeDeftTitle.Add(str);
                    str = $"S{i + 1}_当前条总结果";
                    BIZ_Variable.strEASizeDeftTitle.Add(str);
                    str = $"S{i + 1}_当前条打标通道";
                    BIZ_Variable.strEASizeDeftTitle.Add(str);
                }


                //EA配置尺寸标题
                for (int i = 0; i < pFTCleanCraft.ListEASizes.Count; i++)
                {
                    str = pFTCleanCraft.ListEASizes[i].Description;
                    BIZ_Variable.strEASizeConfigTitle.Add(str);
                    str = "当前结果";
                    BIZ_Variable.strEASizeConfigTitle.Add(str);
                    str = "所属条号";
                    BIZ_Variable.strEASizeConfigTitle.Add(str);
                }




            });




        }

        public void SendAlarm(int index)
        {
            //throw new NotImplementedException();
        }

        public void SendStopAlarm(int index)
        {
            //throw new NotImplementedException();
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

                        flag = SimControl.LoadImage_Sim(Project.Instance.cameraManager[cameraEnum].m_ImageSimManage,
                                     ref item.Value.QueueGrabImage, $"{(cameraEnum).ToString()}仿真加载图片");

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
            //if (Project.Instance.dbjCardManager != null)
            //{
            //    Project.Instance.dbjCardManager.SendXJMQ(strMojuanPath);
            //}

        }

        public void StartRun()
        {

            //打标机发送运行信号
            //if (Project.Instance.dbjCardManager != null)
            //{
            //    //发送打标机软件图像长度
            //    double imageLength = pFTCleanCraft.EASizeMD;
            //    Project.Instance.dbjCardManager.SendImageLength(imageLength);
            //    Project.Instance.dbjCardManager.SendRun(true);

            //}

        }

        public void StopMojuan()
        {
            // throw new NotImplementedException();
        }

        public void StopRun()
        {
            //打标机发送暂停信号
           // if (Project.Instance.dbjCardManager != null) Project.Instance.dbjCardManager.SendRun(false);


        }


        public void UpdateCraft()
        {
            string className = "";
            string methodName = "";

            try
            {
                // 获取当前类名和方法名
                className = MethodBase.GetCurrentMethod().DeclaringType?.FullName ?? "UnknownClass";
                methodName = MethodBase.GetCurrentMethod().Name;
                pFTCleanCraft = Project.Instance.craftBase as PFTCleanCraft;
                List<StatisticalData_Slice> currentData = new List<StatisticalData_Slice>();
                if (BIZ_Variable.StatisticalData_EASlices?.Count > 0)
                {
                    currentData = BIZ_Variable.StatisticalData_EASlices.DeepClone();
                }
               
                QueuePackage<PFTImageProduct> queueImgProductsDisposeAB = new QueuePackage<PFTImageProduct>();
                while (BIZ_Variable.queueImgProductsDisposeAB?.Count > 0)
                {
                    queueImgProductsDisposeAB.Enqueue(BIZ_Variable.queueImgProductsDisposeAB.Dequeue());
                }
                
                InitVariable();
                BIZ_Variable.queueImgProductsDisposeAB=queueImgProductsDisposeAB;

                // 更新数据结构并保留现有计数
                if (currentData.Count == 0)
                {
                    currentData = BIZ_Variable.StatisticalData_EASlices.DeepClone();
                }
                BIZ_Variable.StatisticalData_EASlices = UpdateStatisticalData(
                    currentData, BIZ_Variable.StatisticalData_EASlices,
                    pFTCleanCraft
                );

                InitForm();
            }
            catch (Exception ee)
            {

                MessageNotify.AddErrorLog($"[{className}.{methodName}] 发生异常: {ee.ToString()}");
            }


        }



        public void UpdateForm()
        {
            //throw new NotImplementedException();
        }



        public void ChartDataGrooveShow(int i, int j, int k, double data)
        {

            ExecuteMethod(() =>
            {
                UIService.Instance.AddAction(() =>
                {
                    if (form.GetCob_SliceIndex() == -1 || form.GetCob_GrooveIndex() == -1)
                    {
                        return;
                    }
                    bool isShow = false;
                    if (i == form.GetCob_SliceIndex() && j == form.GetCob_GrooveIndex())
                    {
                        isShow = true;
                    }
                    else
                    {
                        isShow = false;
                    }

                    ParamsChart paramsChart = new ParamsChart();
                    PointClass point = new PointClass();
                    point.CurName = "测量值";
                    point.pointInfo = new PointInfo() { Col = 0, Row = data };
                    paramsChart.points.Add(point);
                    ChartGroovePage[k].UpDataCurve(paramsChart, i, j, k, isShow);

                });
           

            });



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
                UIService.Instance.AddAction(() => 
                {
                    chartUIParam.UpDataCurve(paramsChart);

                });

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(chartUIParam.strChartName, ex);
            }

        }

        public void SendStopAlarm(string msg)
        {
            if(Project.Instance.slaveMachineManager.initState)
            {
                Project.Instance.slaveMachineManager.SendAlarmStop(msg);
            }
        }

        public void SendAlarm(string msg)
        {
            if (Project.Instance.slaveMachineManager.initState)
            {
                Project.Instance.slaveMachineManager.SendAlarm(msg);
            }
        }

       public void OutData()
        {
            ExecuteMethod(() =>
            {
                var items = new List<(int, int, int)> { };
                for (int i = 0; i < BIZ_Variable.StatisticalData_EASlices.Count; i++)
                {
                    var data = (i + 1, (int)BIZ_Variable.StatisticalData_EASlices[i].statisticalData.OKCount, (int)BIZ_Variable.StatisticalData_EASlices[i].statisticalData.NGCount);
                    items.Add(data);
                }

                byte[] pdf = PdfReportHelper.BuildPdf(
                DateTime.Now,
                "default",
                Project.Instance.systemInfo.currentMojuanhao,
                (int)pFTCleanCraft.SliceCount,
                (int)BIZ_Variable.StatisticalData_EASlices[0].statisticalData.SumupCount,
                 Project.Instance.userManage.userData.CurrentUser.ToString(),
                 Project.Instance.userManage.userData.CurrentUserID,
                 items);

                System.IO.File.WriteAllBytes(Project.Instance.paramPathSet.ProductMojuanPath + "统计报告.pdf", pdf);

                MessageNotify.AddDebugLog("统计报告生成成功！");
                



             });

        }

    }
}

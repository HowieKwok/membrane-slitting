using Basler.Pylon;
using Frame_Core.pluginParams;
using HalconDotNet;
using LogTool;
using NPOI.OpenXmlFormats.Dml.Diagram;
using NPOI.SS.Formula.Functions;
using NPOI.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;
using static System.Net.WebRequestMethods;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Menu;

namespace Frame_Core
{
    /// <summary>
    /// 单面阳极流程，-单面阳极，铜箔，有槽位，
    /// </summary>
    public class SingleAnodeProDevice : BaseProDevice
    {
        private StackEACraft m_craft { get; set; } = new StackEACraft();

        /// <summary>
        /// 换卷产品绝对ID
        /// </summary>
        private long m_nProductID_Roll = 0;

        /// <summary>
        /// 换卷图像帧号
        /// </summary>
        private long m_nImageID_Roll = 0;

        public SingleAnodeProDevice()
        {
            m_nCamCount = 5;
            m_nProcessCount = 5;

            for (int i = 0; i < m_nCamCount; i++)
            {
                if (i < 2)
                {
                    m_DicCamOperaParam[i] = new DieCutSliceParam(((CameraEnum)i).ToString());
                    m_DicCamOperaParam_Sim[i] = new DieCutSliceParam(((CameraEnum)i).ToString());
                }
                else
                {
                    m_DicCamOperaParam[i] = new AreaCamParam(((CameraEnum)i).ToString());
                    m_DicCamOperaParam_Sim[i] = new AreaCamParam(((CameraEnum)i).ToString());
                }
            }
            Project.Instance.BindCamImageSource(m_DicCamOperaParam);
            INIPro();
            m_craft.CamNum = m_nCamCount;
            BuildProcessServiceList();
        }

        public override void INIPro()
        {
            m_craft = (StackEACraft)Project.Instance.craftBase;
        }
        public override bool INIProduct()
        {
            try
            {
                foreach (var item in m_DicCamOperaParam)
                {
                    item.Value?.Dispose();
                }
                InitProduct_Sim();
                return true;
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
                Log4netRecord.ErrorLog.Debug("初始化系统数据报错————\n" + ee.ToString());
                return false;
            }

        }
        public override bool InitProduct_Sim()
        {
            try
            {
                /*</清空仿真数据>*/

                foreach (var item in Project.Instance.cameraManager)
                {
                    //初始化仿真路径ID
                    item.Value.m_ImageSimManage.InitSimFlag();
                }

                foreach (var item in m_DicCamOperaParam_Sim)
                {
                    item.Value?.Dispose();
                }

                return true;
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
                Log4netRecord.ErrorLog.Debug("初始化仿真数据报错————\n" + ee.ToString());
                return false;
            }

        }
        public override void BuildProcessServiceList()
        {
            //int ID = Project.Instance.CreateProcess();
            //ProcessServices TempProcessBase = Project.Instance.GetProcessServiceById(ID);
            //TempProcessBase.process.processInfo.ProcessName = CameraEnum.Cam1.ToString() + "单阳涂布面预处理";
            //TempProcessBase.m_eventINIProcess += IniProLineCam;
            //TempProcessBase.m_eventRUNProcess += ProProcessCam1;
            //TempProcessBase.InitProcess();
            //TempProcessBase.OpeningThread();

            int ID = Project.Instance.CreateProcess();
            ProcessServices TempProcessBase = Project.Instance.GetProcessServiceById(ID);
            TempProcessBase.process.processInfo.ProcessName = Project.Instance.cameraManager[CameraEnum.Cam1].m_CamParam.CameraName;
            TempProcessBase.m_eventINIProcess += IniCraftCam1;
            TempProcessBase.m_eventRUNProcess += ProProcessCam1;
            TempProcessBase.InitProcess();
            TempProcessBase.OpeningThread();

            //ID = Project.Instance.CreateProcess();
            //TempProcessBase = Project.Instance.GetProcessServiceById(ID);
            //TempProcessBase.process.processInfo.ProcessName = CameraEnum.Cam2.ToString() + "单阳箔面预处理";
            //TempProcessBase.m_eventINIProcess += IniProLineCam;
            //TempProcessBase.m_eventRUNProcess += ProProcessCam2;
            //TempProcessBase.InitProcess();
            //TempProcessBase.OpeningThread();

            ID = Project.Instance.CreateProcess();
            TempProcessBase = Project.Instance.GetProcessServiceById(ID);
            TempProcessBase.process.processInfo.ProcessName = Project.Instance.cameraManager[CameraEnum.Cam2].m_CamParam.CameraName;
            TempProcessBase.m_eventINIProcess += IniCraftCam1;
            TempProcessBase.m_eventRUNProcess += ProProcessCam1;
            TempProcessBase.InitProcess();
            TempProcessBase.OpeningThread();

            ID = Project.Instance.CreateProcess();
            TempProcessBase = Project.Instance.GetProcessServiceById(ID);
            TempProcessBase.process.processInfo.ProcessName = Project.Instance.cameraManager[CameraEnum.Cam3].m_CamParam.CameraName;
            TempProcessBase.m_eventINIProcess += IniProRectifyCam;
            TempProcessBase.m_eventRUNProcess += ProProcessCam3;
            TempProcessBase.InitProcess();
            TempProcessBase.OpeningThread();

            ID = Project.Instance.CreateProcess();
            TempProcessBase = Project.Instance.GetProcessServiceById(ID);
            TempProcessBase.process.processInfo.ProcessName = Project.Instance.cameraManager[CameraEnum.Cam4].m_CamParam.CameraName;
            TempProcessBase.m_eventINIProcess += IniProRectifyCam;
            TempProcessBase.m_eventRUNProcess += ProProcessCam3;
            TempProcessBase.InitProcess();
            TempProcessBase.OpeningThread();

            ID = Project.Instance.CreateProcess();
            TempProcessBase = Project.Instance.GetProcessServiceById(ID);
            TempProcessBase.process.processInfo.ProcessName = Project.Instance.cameraManager[CameraEnum.Cam5].m_CamParam.CameraName;
            TempProcessBase.m_eventINIProcess += IniProSizeCam;
            TempProcessBase.m_eventRUNProcess += ProProcessCam5;
            TempProcessBase.InitProcess();
            TempProcessBase.OpeningThread();
        }

        //初始化流程
        /// <summary>
        /// 线扫EA处理流程cam1
        /// </summary>
        public void IniCraftCam1(Process process)
        {
            try
            {
                process.m_ModuleObjList.Clear();
                int ProcessID = process.processInfo.ProcessID;
                int camID = ProcessID;
                string pluginName;
                IPlugin Plugin;
                ParamsMeasureLine Params;
                foreach (var item in m_craft.ListLines[camID])
                {
                    //基准边，在overhang中指代隔膜边
                    pluginName = "直线抓边工具";
                    Plugin = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                    Plugin.moduleInfo.ModuleName = (item.StrLineName);
                    Plugin.moduleInfo.PluginName = pluginName;
                    Params = Plugin.PluginParams as ParamsMeasureLine; //里氏转换
                    Params.hv_RoiAngle = 90;
                    process.AddModuleObj(Plugin);
                }

                pluginName = "模切瑕疵检测工具";
                IPlugin PluginDefects = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                PluginDefects.moduleInfo.ModuleName = $"涂布瑕疵检测";
                PluginDefects.moduleInfo.PluginName = pluginName;
                ParamsMQDefects MQDefectsParams = PluginDefects.PluginParams as ParamsMQDefects; //里氏转换
                MQDefectsParams.com_Cathode = 0;
                MQDefectsParams.com_DetectionMethod = 4;
                process.AddModuleObj(PluginDefects);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        /// <summary>
        /// 线扫EA处理流程cam2
        /// </summary>
        public void IniCraftCam2(Process process)
        {
            try
            {
                process.m_ModuleObjList.Clear();
                int ProcessID = process.processInfo.ProcessID;
                int camID = ProcessID;
                string pluginName;
                IPlugin Plugin;

                ParamsMeasureLine Params;
                foreach (var item in m_craft.ListLines[camID])
                {
                    //基准边，在overhang中指代隔膜边
                    pluginName = "直线抓边工具";
                    Plugin = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                    Plugin.moduleInfo.ModuleName = (item.StrLineName);
                    Plugin.moduleInfo.PluginName = pluginName;
                    Params = Plugin.PluginParams as ParamsMeasureLine; //里氏转换
                    Params.hv_RoiAngle = 90;
                    process.AddModuleObj(Plugin);
                }

                pluginName = "模切瑕疵检测工具";
                IPlugin PluginDefects = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                PluginDefects.moduleInfo.ModuleName = $"箔材瑕疵检测";
                PluginDefects.moduleInfo.PluginName = pluginName;
                ParamsMQDefects MQDefectsParams = PluginDefects.PluginParams as ParamsMQDefects; //里氏转换
                MQDefectsParams.com_Cathode = 0;
                MQDefectsParams.com_DetectionMethod = 4;
                process.AddModuleObj(PluginDefects);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            {

            }
        }
        /// <summary>
        /// 纠偏流程
        /// </summary>
        /// <param name="process"></param>
        public void IniProRectifyCam(Process process)
        {
            try
            {
                process.m_ModuleObjList.Clear();
                int ProcessID = process.processInfo.ProcessID;
                int camID = ProcessID;
                IPlugin Plugin;
                string pluginName;
                ParamsMeasureLine Params;
                foreach (var item in m_craft.ListLines[camID])
                {
                    if (!item.IsStandLine) //基准边不用添加
                    {
                        //基准边，在overhang中指代隔膜边
                        pluginName = "直线抓边工具";
                        Plugin = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                        Plugin.moduleInfo.ModuleName = (item.StrLineName);
                        Plugin.moduleInfo.PluginName = pluginName;
                        Params = Plugin.PluginParams as ParamsMeasureLine; //里氏转换
                        Params.hv_RoiAngle = 90;
                        process.AddModuleObj(Plugin);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 面阵尺寸外观流程
        /// </summary>
        /// <param name="process"></param>
        public void IniProSizeCam(Process process)
        {
            try
            {
                process.m_ModuleObjList.Clear();
                int ProcessID = process.processInfo.ProcessID;
                int camID = ProcessID;
                IPlugin Plugin;
                string pluginName;

                //模板匹配工具
                ParamsFindModel ModelParams;
                pluginName = "模板匹配检测工具";
                Plugin = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                Plugin.moduleInfo.ModuleName = "模板查找";
                Plugin.moduleInfo.PluginName = pluginName;
                Plugin.moduleInfo.ProcessName = process.processInfo.ProcessName;
                Plugin.moduleInfo.ProcessID = process.processInfo.ProcessID;
                ModelParams = Plugin.PluginParams as ParamsFindModel; //里氏转换
                process.AddModuleObj(Plugin);

                //缺陷检测区域
                ParamsDetectDefect RegionParams;
                pluginName = "缺陷检测工具";
                Plugin = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                Plugin.moduleInfo.ModuleName = "缺陷检测区域";
                Plugin.moduleInfo.PluginName = pluginName;
                Plugin.moduleInfo.ProcessName = process.processInfo.ProcessName;
                Plugin.moduleInfo.ProcessID = process.processInfo.ProcessID;
                RegionParams = Plugin.PluginParams as ParamsDetectDefect; //里氏转换
                process.AddModuleObj(Plugin);

                // 模切瑕疵检测工具
                ParamsMQDefects DefectsParams;
                pluginName = "模切瑕疵检测工具";
                Plugin = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                Plugin.moduleInfo.ModuleName = "缺陷检测";
                Plugin.moduleInfo.PluginName = pluginName;
                Plugin.moduleInfo.ProcessName = process.processInfo.ProcessName;
                Plugin.moduleInfo.ProcessID = process.processInfo.ProcessID;
                DefectsParams = Plugin.PluginParams as ParamsMQDefects; //里氏转换
                process.AddModuleObj(Plugin);

                // 边缘检测检测工具
                pluginName = "模切瑕疵检测工具";
                Plugin = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                Plugin.moduleInfo.ModuleName = "边缘检测";
                Plugin.moduleInfo.PluginName = pluginName;
                Plugin.moduleInfo.ProcessName = process.processInfo.ProcessName;
                Plugin.moduleInfo.ProcessID = process.processInfo.ProcessID;
                DefectsParams = Plugin.PluginParams as ParamsMQDefects; //里氏转换
                process.AddModuleObj(Plugin);


                ParamsMeasureLine Params;
                foreach (var item in m_craft.ListLines[camID])
                {
                    //基准边，在overhang中指代隔膜边
                    pluginName = "直线抓边工具";
                    Plugin = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                    Plugin.moduleInfo.ModuleName = (item.StrLineName);
                    Plugin.moduleInfo.PluginName = pluginName;
                    Params = Plugin.PluginParams as ParamsMeasureLine; //里氏转换
                    Params.hv_RoiAngle = 90;
                    process.AddModuleObj(Plugin);
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //执行流程
        public void ProProcessCam1(Process process)
        {
            try
            {
                int ProcessID = process.processInfo.ProcessID;
                int camID = ProcessID;
                SetThreadAffinityMask(GetCurrentThread(), new UIntPtr(SetCpuID(ProcessID)));
                //瑕疵检测区域缩进
                double LeftShaft = m_craft.LeftShaft1;
                double UpShaft = m_craft.UpShaft1;
                double RightShaft = m_craft.RightShaft1;
                double DownShaft = m_craft.DownShaft1;
                if (camID == 1)
                {
                    LeftShaft = m_craft.LeftShaft2;
                    UpShaft = m_craft.UpShaft2;
                    RightShaft = m_craft.RightShaft2;
                    DownShaft = m_craft.DownShaft2;
                }

                //标定系数
                double Calipix_y = Project.Instance.cameraManager[(CameraEnum)camID].m_CamParam.CalibrationValue_Y;
                double Calipix_x = Project.Instance.cameraManager[(CameraEnum)camID].m_CamParam.CalibrationValue_X;
                double ABMis = Project.Instance.cameraManager[CameraEnum.Cam2].m_CamParam.ToMainCamDistance / Calipix_y;
                bool ExecuteSuccess = false;
                // 选择参数和队列基于是否模拟

                DieCutSliceParam DieCutSliceParamBase = Project.Instance.Is_simulation ? m_DicCamOperaParam_Sim[camID] as DieCutSliceParam : m_DicCamOperaParam[camID] as DieCutSliceParam;
                var imageQueue = DieCutSliceParamBase.alreadyDealGrabImage;

                DieCutSliceProduct dieCutSliceProduct;
                DieCutSliceParamBase.semaphoreSlimCam.Wait();

                if (imageQueue.Count > 0) // 检查主相机的图像队列是否有图像
                {
                    //主相机图像参数
                    ImageInfo InfoImgInfo = imageQueue.Dequeue();

                    List<MessageShowInfo> MesssgeShowInfos = new List<MessageShowInfo>();
                    List<RegionShowInfo> RegionShowInfos = new List<RegionShowInfo>();
                    List<LineInfo> LineShowInfos = new List<LineInfo>();

                    dieCutSliceProduct = new DieCutSliceProduct();
                    dieCutSliceProduct.bIsCathode = false;
                    dieCutSliceProduct.nCurFrameImgId = InfoImgInfo.image_ID;
                    dieCutSliceProduct.nRealativeFrameImgID = InfoImgInfo.image_ID;

                    string str = ((CameraEnum)camID).ToString() + "信号量--" + DieCutSliceParamBase.semaphoreSlimCam.CurrentCount.ToString() + "图像数--" + imageQueue.Count.ToString();
                    MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = str, m_stuGetColor = HalconColor.green });

                    MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = $"当前产品对应的绝对帧号：{dieCutSliceProduct.nCurFrameImgId}", m_stuGetColor = HalconColor.green });
                    MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = $"当前产品对应的相对帧号：{dieCutSliceProduct.nRealativeFrameImgID}", m_stuGetColor = HalconColor.green });

                    //计时            
                    HOperatorSet.CountSeconds(out dieCutSliceProduct.Image_StartTime);


                    double C1 = 1000; //矩形左边
                    double R1 = 1000; //矩形上边
                    double C2 = 2000; //矩形右边
                    double R2 = 2000; //矩形下边

                    //抓边，并把边结果添加到显示队列，
                    for (int i = 0; i < m_craft.ListLines[camID].Count(); i++)
                    {
                        //抓边
                        IPlugin PluginMline = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == m_craft.ListLines[camID][i].StrLineName);
                        ParamsMeasureLine MlineParams = PluginMline.PluginParams as ParamsMeasureLine; //里氏转换
                        MlineParams.ho_Image = InfoImgInfo.ho_image;

                        LineInfo line = new LineInfo();
                        bool isSuccess = PluginMline.ExecuteObj();
                        DetectEdgeValueAssign(MlineParams, isSuccess, line, m_craft.ListLines[camID][i].StrLineName, MesssgeShowInfos, LineShowInfos, RegionShowInfos);

                        //定位涂布区域左边
                        if (line.StrLineName == "涂布左基准" && isSuccess)
                        {
                            C1 = (line.ColBegin + line.ColEnd) / 2 + LeftShaft;
                        }
                        //定位涂布区域右边
                        if (line.StrLineName == "涂布右基准" && isSuccess)
                        {
                            C2 = (line.ColBegin + line.ColEnd) / 2 - RightShaft;
                        }
                    }
                    HOperatorSet.CountSeconds(out dieCutSliceProduct.Image_FindLineTime);
                    //生成矩形检测区域
                    R1 = UpShaft; //矩形上边
                    R2 = InfoImgInfo.imageHeight - DownShaft; //矩形下边
                    if (C1 < C2 && R1 < R2)
                    {
                        HOperatorSet.GenRectangle1(out HObject GroVRegion1, R1, C1, R2, C2);
                        RegionShowInfos.Add(new RegionShowInfo() { m_hRegion = GroVRegion1, m_stuGetColor = HalconColor.green });

                        //执行缺陷检测。
                        ParamsMQDefects _ParamsSlotefects = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == "涂布瑕疵检测").PluginParams as ParamsMQDefects;
                        _ParamsSlotefects.DetectionROI = GroVRegion1;
                        HOperatorSet.GenEmptyObj(out _ParamsSlotefects.ShieldROI);
                        _ParamsSlotefects.ho_Image = InfoImgInfo.ho_image;
                        _ParamsSlotefects.WidthCali = Calipix_x;
                        _ParamsSlotefects.HeightCali = Calipix_y;
                        process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == "涂布瑕疵检测").ExecuteObj();

                        //整合缺陷检测结果。
                        foreach (var item in _ParamsSlotefects.detectInfos)
                        {
                            dieCutSliceProduct.NgTypeInfos.Add(item);
                        }
                    }
                    else
                    {
                        HOperatorSet.GenRectangle1(out HObject GroVRegion1, 0, 0, 100, 100);
                        RegionShowInfos.Add(new RegionShowInfo() { m_hRegion = GroVRegion1, m_stuGetColor = HalconColor.green });
                        //未能生成检测区域，可返回，或者生成默认区域。
                        //return;
                    }

                    HOperatorSet.CountSeconds(out dieCutSliceProduct.Image_FindDefectTime);
                    HOperatorSet.CountSeconds(out dieCutSliceProduct.Image_ArithmeticProcessTime);
                    //MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = "处理时长：" + HtEnd.D.ToString("0.00") + "ms", m_stuGetColor = HalconColor.green });

                    dieCutSliceProduct.showHalconWindow.m_nProductID = InfoImgInfo.image_ID;
                    dieCutSliceProduct.showHalconWindow.m_hImage = InfoImgInfo.ho_image;
                    dieCutSliceProduct.showHalconWindow.m_lineShowInfos = LineShowInfos;
                    dieCutSliceProduct.showHalconWindow.m_regionShowInfos = RegionShowInfos;
                    dieCutSliceProduct.showHalconWindow.m_messageShowInfos = MesssgeShowInfos;

                    DieCutSliceParamBase.ProductCallBack(dieCutSliceProduct);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void ProProcessCam2(Process process)
        {
            try
            {
                int ProcessID = process.processInfo.ProcessID;
                int camID = ProcessID;
                SetThreadAffinityMask(GetCurrentThread(), new UIntPtr(SetCpuID(ProcessID)));


                //标定系数
                double Calipix_y = Project.Instance.cameraManager[(CameraEnum)camID].m_CamParam.CalibrationValue_Y;
                double Calipix_x = Project.Instance.cameraManager[(CameraEnum)camID].m_CamParam.CalibrationValue_X;
                double ABMis = Project.Instance.cameraManager[CameraEnum.Cam1].m_CamParam.ToMainCamDistance / Calipix_y;
                bool ExecuteSuccess = false;
                // 选择参数和队列基于是否模拟

                DieCutSliceParam DieCutSliceParamFrom = Project.Instance.Is_simulation ? m_DicCamOperaParam_Sim[camID] as DieCutSliceParam : m_DicCamOperaParam[camID] as DieCutSliceParam;
                var imageQueue = DieCutSliceParamFrom.alreadyDealGrabImage;

                DieCutSliceProduct dieCutSliceProduct;
                DieCutSliceParamFrom.semaphoreSlimCam.Wait();

                if (imageQueue.Count > 0) // 检查主相机的图像队列是否有图像
                {
                    //主相机图像参数
                    ImageInfo InfoImgInfo = imageQueue.Dequeue();
                    DieCutSliceParamFrom.imageInfos.Add(InfoImgInfo);
                    List<MessageShowInfo> MesssgeShowInfos = new List<MessageShowInfo>();
                    List<RegionShowInfo> RegionShowInfos = new List<RegionShowInfo>();
                    List<LineInfo> LineShowInfos = new List<LineInfo>();

                    dieCutSliceProduct = new DieCutSliceProduct();
                    dieCutSliceProduct.bIsCathode = false;

                    dieCutSliceProduct.nCurFrameImgId = InfoImgInfo.image_ID;
                    dieCutSliceProduct.nRealativeFrameImgID = InfoImgInfo.image_ID;

                    string str = ((CameraEnum)camID).ToString() + "信号量--" + DieCutSliceParamFrom.semaphoreSlimCam.CurrentCount.ToString() + "图像数--" + imageQueue.Count.ToString();
                    MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = str, m_stuGetColor = HalconColor.green });

                    MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = $"当前产品对应的绝对帧号：{dieCutSliceProduct.nCurFrameImgId}", m_stuGetColor = HalconColor.green });
                    MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = $"当前产品对应的相对帧号：{dieCutSliceProduct.nRealativeFrameImgID}", m_stuGetColor = HalconColor.green });


                    //计时            
                    HOperatorSet.CountSeconds(out dieCutSliceProduct.Image_StartTime);

                    double JR1X1 = 0; //槽1左边X
                    double JR1midY = 0; //槽1左边X

                    //抓边，并把边结果添加到显示队列，
                    for (int i = 0; i < m_craft.ListLines[camID].Count(); i++)
                    {
                        //抓边
                        IPlugin PluginMline = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == m_craft.ListLines[camID][i].StrLineName);
                        ParamsMeasureLine MlineParams = PluginMline.PluginParams as ParamsMeasureLine; //里氏转换
                        MlineParams.ho_Image = InfoImgInfo.ho_image;

                        LineInfo line = new LineInfo();
                        bool isSuccess = PluginMline.ExecuteObj();
                        DetectEdgeValueAssign(MlineParams, isSuccess, line, m_craft.ListLines[camID][i].StrLineName, MesssgeShowInfos, LineShowInfos, RegionShowInfos);

                        //精定位槽位中心
                        if (line.StrLineName == "涂布左基准" && isSuccess)
                        {
                            JR1X1 = (line.ColBegin + line.ColEnd) / 2;
                        }
                    }
                    //完成抓边后，检测缺陷，设定ROI。
                    HTuple htRowBegion = 0;
                    HTuple htRowEnd = 0;
                    HTuple htColBegion = 0;
                    HTuple htColEnd = 0;
                    HOperatorSet.GenEmptyObj(out HObject GroVRegion1);
                    if (JR1X1 > 0)
                    {
                        htRowBegion = 0;
                        htRowEnd = InfoImgInfo.imageHeight;
                        htColBegion = JR1X1 + m_craft.GrovED / Calipix_x - m_craft.EASizeCH2 / Calipix_x;
                        htColEnd = JR1X1 + m_craft.GrovED / Calipix_x + m_craft.JRH / Calipix_x;
                        //涂布ROI区域
                        HOperatorSet.GenRectangle1(out GroVRegion1, htRowBegion, htColBegion, htRowEnd, htColEnd);
                        RegionShowInfos.Add(new RegionShowInfo() { m_hRegion = GroVRegion1, m_stuGetColor = HalconColor.green });

                        //执行缺陷检测。
                        ParamsMQDefects _ParamsSlotefects = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == "箔材瑕疵检测").PluginParams as ParamsMQDefects;
                        _ParamsSlotefects.DetectionROI = GroVRegion1;
                        _ParamsSlotefects.ShieldROI = new HObject();
                        _ParamsSlotefects.ho_Image = InfoImgInfo.ho_image;
                        _ParamsSlotefects.WidthCali = Calipix_x;
                        _ParamsSlotefects.HeightCali = Calipix_y;
                        process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == "箔材瑕疵检测").ExecuteObj();

                        //整合缺陷检测结果。
                        foreach (var item in _ParamsSlotefects.detectInfos)
                        {
                            dieCutSliceProduct.NgTypeInfos.Add(item);
                        }
                    }

                    HOperatorSet.CountSeconds(out dieCutSliceProduct.Image_FindLineTime);
                    HOperatorSet.CountSeconds(out dieCutSliceProduct.Image_FindDefectTime);
                    HOperatorSet.CountSeconds(out dieCutSliceProduct.Image_ArithmeticProcessTime);
                    //MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = "处理时长：" + HtEnd.D.ToString("0.00") + "ms", m_stuGetColor = HalconColor.green });

                    dieCutSliceProduct.showHalconWindow.m_nProductID = InfoImgInfo.image_ID;
                    dieCutSliceProduct.showHalconWindow.m_hImage = InfoImgInfo.ho_image;
                    dieCutSliceProduct.showHalconWindow.m_lineShowInfos = LineShowInfos;
                    dieCutSliceProduct.showHalconWindow.m_regionShowInfos = RegionShowInfos;
                    dieCutSliceProduct.showHalconWindow.m_messageShowInfos = MesssgeShowInfos;

                    DieCutSliceParamFrom.ProductCallBack(dieCutSliceProduct);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 纠偏流程
        /// </summary>
        /// <param name="process"></param>
        public void ProProcessCam3(Process process)
        {
            try
            {
                int ProcessID = process.processInfo.ProcessID;
                int camID = ProcessID;
                double BaseLineY = m_craft.UnwindBaseLineY; //纠偏基准边Y
                if (camID == (int)CameraEnum.Cam4)
                {
                    //camID = (int)CameraEnum.Cam4;
                    BaseLineY = m_craft.DieCutBaseLineY;
                }
                double baseLineX = m_craft.DieCutBaseLineX;

                SetThreadAffinityMask(GetCurrentThread(), new UIntPtr(SetCpuID(ProcessID)));
                //主相机图像参数
                ImageInfo InfoImgInfo;

                //标定系数
                double Calipix_y = Project.Instance.cameraManager[(CameraEnum)camID].m_CamParam.CalibrationValue_Y;
                double Calipix_x = Project.Instance.cameraManager[(CameraEnum)camID].m_CamParam.CalibrationValue_X;


                bool ExecuteSuccess = false;
                //相机对应的流程参数
                AreaCamParam ACProParam = Project.Instance.Is_simulation ? m_DicCamOperaParam_Sim[camID] as AreaCamParam : m_DicCamOperaParam[camID] as AreaCamParam;

                var imageQueue = ACProParam.alreadyDealGrabImage;

                CamEAStackProduct camEAPD;
                ACProParam.semaphoreSlimCam.Wait();


                // 检查主相机的图像队列是否有图像
                if (imageQueue.Count > 0)
                {

                    //局部变量
                    List<MessageShowInfo> MesssgeShowInfos = new List<MessageShowInfo>();
                    List<RegionShowInfo> RegionShowInfos = new List<RegionShowInfo>();
                    List<LineInfo> LineShowInfos = new List<LineInfo>();

                    //图像出队
                    InfoImgInfo = imageQueue.Dequeue();
                    ACProParam.nEANum++;
                    //根据流程参数的 开始信号 生成单帧图像检测的PD。
                    if (ACProParam.bIsStartShoot == true)
                    {
                        ACProParam.bIsStartShoot = false;
                        ACProParam.nEAIndex = 1;
                        camEAPD = new CamEAStackProduct() { intCamEnum = camID, EAIndex = ACProParam.nEAIndex };
                    }
                    else
                    {
                        ACProParam.nEAIndex++;
                        camEAPD = new CamEAStackProduct() { intCamEnum = camID, EAIndex = ACProParam.nEAIndex };
                    }

                    //计时            
                    HOperatorSet.CountSeconds(out camEAPD.Image_StartTime);

                    string str = ((CameraEnum)camID).ToString() + "信号量--" + ACProParam.semaphoreSlimCam.CurrentCount.ToString() + "图像数--" + imageQueue.Count.ToString();
                    MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = str, m_stuGetColor = HalconColor.green });
                    MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = $"当前帧号：{InfoImgInfo.image_ID}", m_stuGetColor = HalconColor.green });
                    MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = $"产品编号：{camEAPD.EAIndex}", m_stuGetColor = HalconColor.green });

                    //抓边，并把边结果添加到显示队列，
                    for (int i = 0; i < m_craft.ListLines[camID].Count(); i++)
                    {
                        if (!m_craft.ListLines[camID][i].IsStandLine) //基准边判断。
                        {
                            //抓边检测
                            IPlugin PluginMline = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == m_craft.ListLines[camID][i].StrLineName);
                            ParamsMeasureLine MlineParams = PluginMline.PluginParams as ParamsMeasureLine; //里氏转换
                            MlineParams.ho_Image = InfoImgInfo.ho_image;
                            LineInfo line = new LineInfo();
                            bool isSuccess = PluginMline.ExecuteObj();
                            DetectEdgeValueAssign(MlineParams, isSuccess, line, m_craft.ListLines[camID][i].StrLineName, MesssgeShowInfos, LineShowInfos, RegionShowInfos);
                        }
                        else  //基准边边直接添加
                        {
                            if (m_craft.ListLines[camID][i].StrLineName.Contains("基准边Y"))
                            {
                                LineInfo line = new LineInfo()
                                {
                                    StrLineName = m_craft.ListLines[camID][i].StrLineName,
                                    RowBegin = BaseLineY,
                                    ColBegin = 0,
                                    RowEnd = BaseLineY,
                                    ColEnd = InfoImgInfo.imageWidth - 1,
                                    IsLineSuccessfull = true,
                                };
                                line.m_nLineWidth = 10;
                                line.Color = HalconColor.yellow;

                                LineShowInfos.Add(line);
                            }
                            else if (m_craft.ListLines[camID][i].StrLineName.Contains("基准边X"))
                            {
                                LineInfo line = new LineInfo()
                                {
                                    StrLineName = m_craft.ListLines[camID][i].StrLineName,
                                    RowBegin = 0,
                                    ColBegin = baseLineX,
                                    RowEnd = InfoImgInfo.imageHeight - 1,
                                    ColEnd = baseLineX,
                                    IsLineSuccessfull = true,
                                };
                                line.m_nLineWidth = 10;

                                line.Color = HalconColor.yellow;
                                LineShowInfos.Add(line);
                            }
                        }
                    }

                    //抓边完成后，计算尺寸。
                    for (int i = 0; i < m_craft.ListSizeEdits[camID].Count; i++)
                    {
                        SizeInfo size = new SizeInfo()
                        {
                            Name = m_craft.ListSizeEdits[camID][i].Name,
                            SizeName = m_craft.ListSizeEdits[camID][i].Name,
                            Description = m_craft.ListSizeEdits[camID][i].Description,
                        };
                        LineInfo line1 = LineShowInfos.Find(c => c.StrLineName == m_craft.ListSizeEdits[camID][i].Line1);
                        LineInfo line2 = LineShowInfos.Find(c => c.StrLineName == m_craft.ListSizeEdits[camID][i].Line2);
                        if (line1 != null && line2 != null)
                        {
                            if (line1.IsLineSuccessfull && line2.IsLineSuccessfull)
                            {
                                ///补充根据枚举类型计算的线线关系。
                                bool flag = CalculateLines((CameraEnum)camID, line1, line2, m_craft.ListSizeEdits[camID][i].EditorType, m_craft.ListSizeEdits[camID][i].StD_RealFixRito, m_craft.ListSizeEdits[camID][i].StD_RealFixOffset, out double val);
                                if (flag) //
                                {
                                    size.DataResult = val;
                                }
                            }
                        }
                        camEAPD.sizeInfos.Add(size);//添加尺寸
                    }
                    camEAPD.CamFinish = true;
                    HOperatorSet.CountSeconds(out camEAPD.Image_FindLineTime);
                    HOperatorSet.CountSeconds(out camEAPD.Image_FindDefectTime);
                    HOperatorSet.CountSeconds(out camEAPD.Image_ArithmeticProcessTime);
                    //MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = "处理时长：" + HtEnd.D.ToString("0.00") + "ms", m_stuGetColor = HalconColor.green });
                    //单相机，单次检测完成，返回本次结果。后续计算逻辑统一计算。
                    camEAPD.showHalconWindow.m_nProductID = InfoImgInfo.image_ID;
                    camEAPD.showHalconWindow.m_hImage = InfoImgInfo.ho_image;
                    camEAPD.showHalconWindow.m_nImageFrameID = InfoImgInfo.image_ID;
                    camEAPD.showHalconWindow.m_lineShowInfos = LineShowInfos;
                    camEAPD.showHalconWindow.m_regionShowInfos = RegionShowInfos;
                    camEAPD.showHalconWindow.m_messageShowInfos = MesssgeShowInfos;
                    ACProParam.ProductCallBack(camEAPD);
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }
        public void ProProcessCam5(Process process)
        {
            try
            {
                int ProcessID = process.processInfo.ProcessID;
                int camID = ProcessID;
                SetThreadAffinityMask(GetCurrentThread(), new UIntPtr(SetCpuID(ProcessID)));
                //主相机图像参数
                ImageInfo InfoImgInfo;

                //标定系数
                double Calipix_y = Project.Instance.cameraManager[(CameraEnum)camID].m_CamParam.CalibrationValue_Y;
                double Calipix_x = Project.Instance.cameraManager[(CameraEnum)camID].m_CamParam.CalibrationValue_X;

                bool ExecuteSuccess = false;
                //相机对应的流程参数
                AreaCamParam ACProParam = Project.Instance.Is_simulation ? m_DicCamOperaParam_Sim[camID] as AreaCamParam : m_DicCamOperaParam[camID] as AreaCamParam;

                var imageQueue = ACProParam.alreadyDealGrabImage;

                CamEAStackProduct camEAPD;
                ACProParam.semaphoreSlimCam.Wait();


                // 检查主相机的图像队列是否有图像
                if (imageQueue.Count > 0)
                {

                    //局部变量
                    List<MessageShowInfo> MesssgeShowInfos = new List<MessageShowInfo>();
                    List<RegionShowInfo> RegionShowInfos = new List<RegionShowInfo>();
                    List<LineInfo> LineShowInfos = new List<LineInfo>();

                    //图像出队
                    InfoImgInfo = imageQueue.Dequeue();
                    ACProParam.nEANum++;
                    //根据流程参数的 开始信号 生成单帧图像检测的PD。
                    if (ACProParam.bIsStartShoot == true)
                    {
                        ACProParam.bIsStartShoot = false;
                        ACProParam.nEAIndex = 1;
                        camEAPD = new CamEAStackProduct() { intCamEnum = camID, EAIndex = ACProParam.nEAIndex };
                    }
                    else
                    {
                        ACProParam.nEAIndex++;
                        camEAPD = new CamEAStackProduct() { intCamEnum = camID, EAIndex = ACProParam.nEAIndex };
                    }

                    //计时            
                    HOperatorSet.CountSeconds(out camEAPD.Image_StartTime);

                    string str = ((CameraEnum)camID).ToString() + "信号量--" + ACProParam.semaphoreSlimCam.CurrentCount.ToString() + "图像数--" + imageQueue.Count.ToString();
                    MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = str, m_stuGetColor = HalconColor.green });
                    MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = $"当前帧号：{InfoImgInfo.image_ID}", m_stuGetColor = HalconColor.green });
                    MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = $"产品编号：{camEAPD.EAIndex}", m_stuGetColor = HalconColor.green });



                    IPlugin pluginDetectRegion = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == "缺陷检测区域");
                    ParamsDetectDefect RegionParams = pluginDetectRegion.PluginParams as ParamsDetectDefect; //里氏转换
                    RegionParams.ho_Image = InfoImgInfo.ho_image;
                    HObject region = null;
                    HOperatorSet.GenEmptyObj(out region);
                    HTuple hv_RoiPhi;
                    HOperatorSet.TupleRad(RegionParams.hv_RoiAngle, out hv_RoiPhi);
                    HOperatorSet.GenRectangle2(out region, RegionParams.hv_RoiMidRow, RegionParams.hv_RoiMidColumn, -hv_RoiPhi, RegionParams.hv_RoiHeight / 2, RegionParams.hv_RoiWidth / 2);
                    if (System.IO.File.Exists(RegionParams.m_strModelGroupPath))
                    {
                        HOperatorSet.ReadRegion(out region, RegionParams.m_strModelGroupPath);
                    }
                    //if (region?.CountObj() == 0 || region?.CountObj() == null)
                    //{
                    //    //报警提示
                    //    return;
                    //}

                    IPlugin PluginModel = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == "模板查找");
                    ParamsFindModel ModelParams = PluginModel.PluginParams as ParamsFindModel; //里氏转换
                    ModelParams.ho_Image = InfoImgInfo.ho_image;
                    bool ismodelSuccess = PluginModel.ExecuteObj();
                    double offsetx = 0;
                    double offsety = 0;
                    double offsetRad = 0;

                    if (ismodelSuccess)
                    {
                        HOperatorSet.AffineTransRegion(region, out region, ModelParams.m_HtuMatID, "nearest_neighbor");
                        RegionShowInfos.Add(new RegionShowInfo() { m_hRegion = ModelParams.m_ho_OutRegion, m_stuGetColor = HalconColor.orange });
                        RegionShowInfos.Add(new RegionShowInfo() { m_hRegion = ModelParams.m_ho_OutCropRegion, m_stuGetColor = HalconColor.pink });
                        RegionShowInfos.Add(new RegionShowInfo() { m_hRegion = region, m_stuGetColor = HalconColor.gold });
                        //计算偏移量
                        offsetx = ModelParams.m_fOffsetX;
                        offsety = ModelParams.m_fOffsetY;
                        offsetRad = ModelParams.m_fOffsetR;
                    }


                    //开腐蚀运算
                    HObject regionErosion = null;
                    if (RegionParams.hv_DilationRadiusCof > 0)
                    {
                        HOperatorSet.ErosionCircle(region, out regionErosion, RegionParams.hv_ErosionRadiusCof);
                    }
                    else
                    {
                        regionErosion = region.Clone();
                    }

                    //膨胀运算

                    HObject regionDilation = null;
                    if (RegionParams.hv_ErosionRadiusCof > 0 && RegionParams.hv_DilationRadiusCof > 0)
                    {
                        HOperatorSet.DilationCircle(region, out regionDilation, RegionParams.hv_DilationRadiusCof);
                        HOperatorSet.Difference(regionDilation, regionErosion, out regionDilation);
                    }
                    else
                    {
                        regionDilation = region.Clone();
                    }


                    RegionShowInfos.Add(new RegionShowInfo() { m_hRegion = regionErosion, m_stuGetColor = HalconColor.pink });
                    RegionShowInfos.Add(new RegionShowInfo() { m_hRegion = regionDilation, m_stuGetColor = HalconColor.blue });
                    IPlugin PluginMQDefects = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == "缺陷检测");
                    ParamsMQDefects DefectsParams = PluginMQDefects.PluginParams as ParamsMQDefects;
                    DefectsParams.DetectionROI = regionErosion;
                    HOperatorSet.GenEmptyObj(out DefectsParams.ShieldROI);

                    DefectsParams.ho_Image = InfoImgInfo.ho_image;
                    DefectsParams.WidthCali = Calipix_x;
                    DefectsParams.HeightCali = Calipix_y;

                    if (!PluginMQDefects.ExecuteObj())
                    {
                        //直接默认缺陷NG，并报警打日志说明异常,默认输出NG类型为其它
                        DefectsParams.detectInfos.Dequeue()?.Dispose();
                        HOperatorSet.GenRectangle1(out HObject hObject, 30, 30, 50, 50);
                        HOperatorSet.ReduceDomain(InfoImgInfo.ho_image, hObject, out HObject imageReduced);
                        NgTypeInfo ngTypeInfo = new NgTypeInfo
                        {
                            m_strNgTypeName = Defct1Enum.其它,
                            m_nImgIndex = InfoImgInfo.image_ID,
                            m_hImage = imageReduced,
                            m_hRegion = hObject,
                            m_iProductIndex = InfoImgInfo.image_ID,
                            m_fNgTypArea = 9999,
                        };
                        camEAPD.NgTypeInfos.Add(ngTypeInfo);
                    }
                    else
                    {
                        //这里直接传给佳庆，现在先做显示用，后续合并这里只做数据传递，不需要再这里做显示
                        while (DefectsParams.detectInfos.Count > 0)
                        {
                            NgTypeInfo ngTypeInfo = DefectsParams.detectInfos.Dequeue();
                            ngTypeInfo.m_nImgIndex = InfoImgInfo.image_ID;
                            ngTypeInfo.m_iProductIndex = InfoImgInfo.image_ID;
                            camEAPD.NgTypeInfos.Add(ngTypeInfo);
                            //RegionShowInfos.Add(new RegionShowInfo() { m_hRegion = ngTypeInfo.m_hRegion, m_stuGetColor = HalconColor.red });
                            //MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = ngTypeInfo.m_strNgTypeName + ":" + "涂层_面积_" + ngTypeInfo.m_fNgTypArea.ToString("0.00"), m_stuGetColor = HalconColor.red });
                            //ngTypeInfo.m_hImage?.Dispose();
                        }
                    }


                    IPlugin PluginMQDefectsEdge = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == "边缘检测");
                    ParamsMQDefects DefectsParamsEdge = PluginMQDefectsEdge.PluginParams as ParamsMQDefects;
                    DefectsParamsEdge.DetectionROI = regionDilation;
                    HOperatorSet.GenEmptyObj(out DefectsParamsEdge.ShieldROI);
                    DefectsParamsEdge.ho_Image = InfoImgInfo.ho_image;
                    DefectsParamsEdge.WidthCali = Calipix_x;
                    DefectsParamsEdge.HeightCali = Calipix_y;


                    if (!PluginMQDefectsEdge.ExecuteObj())
                    {
                        //直接默认缺陷NG，并报警打日志说明异常,默认输出NG类型为其它
                        DefectsParamsEdge.detectInfos.Dequeue()?.Dispose();
                        HOperatorSet.GenRectangle1(out HObject hObject, 30, 30, 50, 50);
                        HOperatorSet.ReduceDomain(InfoImgInfo.ho_image, hObject, out HObject imageReduced);
                        NgTypeInfo ngTypeInfo = new NgTypeInfo
                        {
                            m_strNgTypeName = Defct1Enum.其它,
                            m_nImgIndex = InfoImgInfo.image_ID,
                            m_hImage = imageReduced,
                            m_hRegion = hObject,
                            m_iProductIndex = InfoImgInfo.image_ID,
                            m_fNgTypArea = 9999,
                        };
                        camEAPD.NgTypeInfos.Add(ngTypeInfo);
                    }
                    else
                    {
                        //这里直接传给佳庆，现在先做显示用，后续合并这里只做数据传递，不需要再这里做显示
                        while (DefectsParamsEdge.detectInfos.Count > 0)
                        {
                            NgTypeInfo ngTypeInfo = DefectsParamsEdge.detectInfos.Dequeue();
                            ngTypeInfo.m_nImgIndex = InfoImgInfo.image_ID;
                            ngTypeInfo.m_iProductIndex = InfoImgInfo.image_ID;
                            camEAPD.NgTypeInfos.Add(ngTypeInfo);
                            //RegionShowInfos.Add(new RegionShowInfo() { m_hRegion = ngTypeInfo.m_hRegion, m_stuGetColor = HalconColor.red });
                            //MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = ngTypeInfo.m_strNgTypeName + ":" + "涂层_面积_" + ngTypeInfo.m_fNgTypArea.ToString("0.00"), m_stuGetColor = HalconColor.red });
                            //ngTypeInfo.m_hImage?.Dispose();
                        }
                    }

                    //抓边，并把边结果添加到显示队列，
                    for (int i = 0; i < m_craft.ListLines[camID].Count(); i++)
                    {
                        if (!m_craft.ListLines[camID][i].IsStandLine) //基准边判断。
                        {
                            //抓边检测
                            IPlugin PluginMline = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == m_craft.ListLines[camID][i].StrLineName);
                            ParamsMeasureLine MlineParams = PluginMline.PluginParams as ParamsMeasureLine; //里氏转换
                            MlineParams.ho_Image = InfoImgInfo.ho_image;
                            MlineParams.hv_RoiMidRow += offsety;
                            MlineParams.hv_RoiMidColumn += offsetx;
                            HOperatorSet.TupleDeg(offsetRad, out HTuple rad);
                            MlineParams.hv_RoiAngle += rad;
                            LineInfo line = new LineInfo();
                            bool isSuccess = PluginMline.ExecuteObj();
                            DetectEdgeValueAssign(MlineParams, isSuccess, line, m_craft.ListLines[camID][i].StrLineName, MesssgeShowInfos, LineShowInfos, RegionShowInfos);
                            MlineParams.hv_RoiMidRow -= offsety;
                            MlineParams.hv_RoiMidColumn -= offsetx;
                            MlineParams.hv_RoiAngle -= rad;
                        }
                        else  //基准边边直接添加
                        {
                            LineInfo line = new LineInfo()
                            {
                                StrLineName = m_craft.ListLines[camID][i].StrLineName,
                                RowBegin = m_craft.ListLines[camID][i].RowBegin,
                                RowEnd = m_craft.ListLines[camID][i].RowEnd,
                                ColBegin = m_craft.ListLines[camID][i].ColBegin,
                                ColEnd = m_craft.ListLines[camID][i].ColEnd,
                            };
                            line.m_nLineWidth = 10;
                            line.Color = HalconColor.yellow;
                            LineShowInfos.Add(line);
                        }
                    }

                    //抓边完成后，计算尺寸。
                    for (int i = 0; i < m_craft.ListSizeEdits[camID].Count; i++)
                    {
                        SizeInfo size = new SizeInfo()
                        {
                            Name = m_craft.ListSizeEdits[camID][i].Name,
                            SizeName = m_craft.ListSizeEdits[camID][i].Name,
                            Description = m_craft.ListSizeEdits[camID][i].Description,
                        };
                        LineInfo line1 = LineShowInfos.Find(c => c.StrLineName == m_craft.ListSizeEdits[camID][i].Line1);
                        LineInfo line2 = LineShowInfos.Find(c => c.StrLineName == m_craft.ListSizeEdits[camID][i].Line2);
                        if (line1 != null && line2 != null)
                        {
                            if (line1.IsLineSuccessfull && line2.IsLineSuccessfull)
                            {
                                ///补充根据枚举类型计算的线线关系。
                                bool flag = CalculateLinesPl((CameraEnum)camID, line1, line2, m_craft.ListSizeEdits[camID][i].EditorType, m_craft.ListSizeEdits[camID][i].StD_RealFixRito, m_craft.ListSizeEdits[camID][i].StD_RealFixOffset, out double val);
                                if (flag) //
                                {
                                    size.DataResult = val;
                                }
                            }
                        }
                        camEAPD.sizeInfos.Add(size);//添加尺寸
                    }

                    //完成抓边后，检测缺陷，设定ROI。 
                    // 功能预留先不写


                    camEAPD.CamFinish = true;
                    HOperatorSet.CountSeconds(out camEAPD.Image_FindLineTime);
                    HOperatorSet.CountSeconds(out camEAPD.Image_FindDefectTime);
                    HOperatorSet.CountSeconds(out camEAPD.Image_ArithmeticProcessTime);
                    //MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = "处理时长：" + HtEnd.D.ToString("0.00") + "ms", m_stuGetColor = HalconColor.green });
                    //单相机，单次检测完成，返回本次结果。后续计算逻辑统一计算。
                    camEAPD.showHalconWindow.m_nProductID = InfoImgInfo.image_ID;
                    camEAPD.showHalconWindow.m_hImage = InfoImgInfo.ho_image;
                    camEAPD.showHalconWindow.m_nImageFrameID = InfoImgInfo.image_ID;
                    camEAPD.showHalconWindow.m_lineShowInfos = LineShowInfos;
                    camEAPD.showHalconWindow.m_regionShowInfos = RegionShowInfos;
                    camEAPD.showHalconWindow.m_messageShowInfos = MesssgeShowInfos;
                    ACProParam.ProductCallBack(camEAPD);

                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }



        public void GenerateROI(double JRX, double midY, double Calipix_x, double Calipix_y, out HObject ROI)
        {
            //完成抓边后，检测缺陷，设定ROI。
            HTuple htRowBegion = 0;
            HTuple htRowEnd = 0;
            HTuple htColBegion = 0;
            HTuple htColEnd = 0;

            HOperatorSet.GenEmptyObj(out HObject coatRegion);
            HOperatorSet.GenEmptyObj(out HObject coatShieldRegion);

            HOperatorSet.GenEmptyObj(out HObject RegionDifference);
            HOperatorSet.GenEmptyObj(out HObject RegionClosing);
            HOperatorSet.GenEmptyObj(out ROI);

            //生成槽位区域//槽位左边JR1X1，中心JR1midY。
            if (midY > 0 && JRX > 0)
            {
                htRowBegion = midY - m_craft.JRW / Calipix_y / 2 - m_craft.JRL / Calipix_y;
                htRowEnd = midY + m_craft.JRW / Calipix_y / 2 + m_craft.JRR / Calipix_y;
                htColBegion = JRX - m_craft.JRTagH / Calipix_x - m_craft.EASizeCH2 / Calipix_x;
                htColEnd = JRX - m_craft.JRTagH / Calipix_x;
                //涂布ROI区域
                HOperatorSet.GenRectangle1(out coatRegion, htRowBegion, htColBegion, htRowEnd, htColEnd);

                htRowBegion = midY - m_craft.JRW / Calipix_y / 2 - m_craft.JRL / Calipix_y;
                htRowEnd = midY + m_craft.JRW / Calipix_y / 2 + m_craft.JRR / Calipix_y - m_craft.EASizeCW / Calipix_y;
                htColBegion = JRX - m_craft.JRTagH - m_craft.EASizeCH2 / Calipix_x;
                htColEnd = JRX - m_craft.JRTagH - m_craft.EASizeCH / Calipix_x;
                //涂布ROI 屏蔽区域
                HOperatorSet.GenRectangle1(out coatShieldRegion, htRowBegion, htColBegion, htRowEnd, htColEnd);
                HOperatorSet.Difference(coatRegion, coatShieldRegion, out RegionDifference);
                HOperatorSet.ClosingCircle(RegionDifference, out RegionClosing, m_craft.colsingRadius);
                HOperatorSet.OpeningCircle(RegionClosing, out ROI, m_craft.openingRadius);
            }

            coatRegion.Dispose();
            coatShieldRegion.Dispose();
            RegionDifference.Dispose();
            RegionClosing.Dispose();

        }


        public void CalculatePonitToPointDist()
        {

        }
    }
}

using DefectRuleClassifier.Core.Models;
using Extensions;
using Frame_Core.pluginParams;
using Frame_Core.业务参数类;
using Frame_Core.流程汇总文件.产品类;
using FrameCommunity.Data;
using HalconDotNet;
using LogTool;

using NPOI.OpenXmlFormats.Vml;
using NPOI.SS.Formula.Functions;
using Org.BouncyCastle.Math.EC;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing.Text;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Markup;

namespace Frame_Core.业务类
{

    public static class ObservableCollectionExtensions
    {
        public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                collection.Add(item);
            }
        }
    }
    partial class DieCutProDevice : BaseProDevice
    {
        private CraftParam_DieCuttingCut m_craft => Project.Instance.craftBase as CraftParam_DieCuttingCut;
        public DieCutProDevice()
        {
            m_nCamCount = 6;
            m_nProcessCount = 8;
            for (int i = 0; i < m_nCamCount; i++)
            {
                //默认头两个相机是模切相机
                if (i <= 1)
                {
                    m_DicCamOperaParam[i] = new DieCutParam(((CameraEnum)i).ToString());
                    m_DicCamOperaParam_Sim[i] = new DieCutParam(((CameraEnum)i).ToString());
                }
                //默认第三个为裁切定位相机
                if (i == 2)
                {
                    m_DicCamOperaParam[i] = new CutCorrectParam(((CameraEnum)i).ToString());
                    m_DicCamOperaParam_Sim[i] = new CutCorrectParam(((CameraEnum)i).ToString());
                }
                //默认剩下的为尺寸相机
                if (i > 2)
                {
                    m_DicCamOperaParam[i] = new SizeDetectParam(((CameraEnum)i).ToString());
                    m_DicCamOperaParam_Sim[i] = new SizeDetectParam(((CameraEnum)i).ToString());
                }
            }
            Project.Instance.BindCamImageSource(m_DicCamOperaParam);
            HOperatorSet.SetSystem("clip_region", "false");
            INIPro();
            //  m_craft.CamNum = m_nCamCount;
            BuildProcessServiceList();
            limitThreadPool = new LimitThreadPool(100);

        }
        LimitThreadPool limitThreadPool;

        /// <summary>
        /// 参数类的绑定
        /// </summary>
        public override void INIPro()
        {

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

                System.Windows.MessageBox.Show(ee.ToString());
                Log4netRecord.ErrorLog.Debug("初始化系统数据报错————\n" + ee.ToString());
                return false;

            }
        }


        public override bool InitProduct_Sim()
        {
            try
            {
                foreach (var item in m_DicCamOperaParam_Sim)
                {
                    item.Value?.Dispose();
                }
                return true;
            }
            catch (Exception ee)
            {
                System.Windows.MessageBox.Show(ee.ToString());
                Log4netRecord.ErrorLog.Debug("初始化仿真系统数据报错————\n" + ee.ToString());
                return false;

            }
        }


        public override void BuildProcessServiceList()
        {

            int ID = Project.Instance.CreateProcess();
            int PoleMode = (int)m_craft.Electrode;
            int tabISLeft = m_craft.Get_GlobalVariableItem("极耳在左侧").Value.ToInt();
            bool isAT = m_craft.Get_GlobalVariableItem("带AT").Value.ToBool();
            ProcessServices TempProcessBase = Project.Instance.GetProcessServiceById(ID);
            TempProcessBase.process.processInfo.CameraID = 0;
            TempProcessBase.process.processInfo.ProcessName = Service_Camera.GetCam_Name((int)CameraEnum.Cam1);

            TempProcessBase.m_eventINIProcess += Init相机裁图流程;
            TempProcessBase.m_eventRUNProcess += ProProcess新相机裁图流程
                ;
            TempProcessBase.InitProcess();
            TempProcessBase.OpeningThread();

            ID = Project.Instance.CreateProcess();
            TempProcessBase = Project.Instance.GetProcessServiceById(ID);
            TempProcessBase.process.processInfo.CameraID = 1;
            TempProcessBase.process.processInfo.ProcessName = Service_Camera.GetCam_Name((int)CameraEnum.Cam2);
            TempProcessBase.m_eventINIProcess += Init相机裁图流程;
            TempProcessBase.m_eventRUNProcess += ProProcess新相机裁图流程;
            TempProcessBase.InitProcess();
            TempProcessBase.OpeningThread();

            ID = Project.Instance.CreateProcess();
            TempProcessBase = Project.Instance.GetProcessServiceById(ID);
            TempProcessBase.process.processInfo.CameraID = 0;
            TempProcessBase.process.processInfo.ProcessName = Service_Camera.GetCam_Name((int)CameraEnum.Cam1) + "_极片EA";
            TempProcessBase.m_eventINIProcess += Init极片EA流程;
            TempProcessBase.m_eventRUNProcess += ProProcess极片EA流程;
            TempProcessBase.InitProcess();
            TempProcessBase.OpeningThread();


            ID = Project.Instance.CreateProcess();
            TempProcessBase = Project.Instance.GetProcessServiceById(ID);
            TempProcessBase.process.processInfo.CameraID = 1;
            TempProcessBase.process.processInfo.ProcessName = Service_Camera.GetCam_Name((int)CameraEnum.Cam2) + "_极片EA";
            TempProcessBase.m_eventINIProcess += Init极片EA流程;
            TempProcessBase.m_eventRUNProcess += ProProcess极片EA流程;
            TempProcessBase.InitProcess();
            TempProcessBase.OpeningThread();

            ID = Project.Instance.CreateProcess();
            TempProcessBase = Project.Instance.GetProcessServiceById(ID);
            TempProcessBase.process.processInfo.CameraID = 2;
            TempProcessBase.process.processInfo.ProcessName = Service_Camera.GetCam_Name((int)CameraEnum.Cam3);
            TempProcessBase.m_eventINIProcess += Init纠偏检测流程;
            TempProcessBase.m_eventRUNProcess += ProProcess纠偏检测流程;
            TempProcessBase.InitProcess();
            TempProcessBase.OpeningThread();


            ID = Project.Instance.CreateProcess();
            TempProcessBase = Project.Instance.GetProcessServiceById(ID);
            TempProcessBase.process.processInfo.CameraID = 3;
            TempProcessBase.process.processInfo.ProcessName = Service_Camera.GetCam_Name((int)CameraEnum.Cam4);
            TempProcessBase.m_eventINIProcess += Init极片尺寸检测流程;
            TempProcessBase.m_eventRUNProcess += ProProcess极片尺寸检测流程;
            TempProcessBase.InitProcess();
            TempProcessBase.OpeningThread();

            if (tabISLeft==1)
            {
                ID = Project.Instance.CreateProcess();
                TempProcessBase = Project.Instance.GetProcessServiceById(ID);
                TempProcessBase.process.processInfo.CameraID = 4;
                TempProcessBase.process.processInfo.ProcessName = Service_Camera.GetCam_Name((int)CameraEnum.Cam5);
                TempProcessBase.m_eventINIProcess += Init极片尺寸带极耳检测流程;
                TempProcessBase.m_eventRUNProcess += ProProcess极片尺寸带极耳检测流程;
                TempProcessBase.InitProcess();
                TempProcessBase.OpeningThread();

                ID = Project.Instance.CreateProcess();
                TempProcessBase = Project.Instance.GetProcessServiceById(ID);
                TempProcessBase.process.processInfo.CameraID = 5;
                TempProcessBase.process.processInfo.ProcessName = Service_Camera.GetCam_Name((int)CameraEnum.Cam6);
                TempProcessBase.m_eventINIProcess += Init极片尺寸检测流程;
                TempProcessBase.m_eventRUNProcess += ProProcess极片尺寸检测流程;
                TempProcessBase.InitProcess();
                TempProcessBase.OpeningThread();
            }
            else
            {

                ID = Project.Instance.CreateProcess();
                TempProcessBase = Project.Instance.GetProcessServiceById(ID);
                TempProcessBase.process.processInfo.CameraID = 4;
                TempProcessBase.process.processInfo.ProcessName = Service_Camera.GetCam_Name((int)CameraEnum.Cam5);
                TempProcessBase.m_eventINIProcess += Init极片尺寸检测流程;
                TempProcessBase.m_eventRUNProcess += ProProcess极片尺寸检测流程;
                TempProcessBase.InitProcess();
                TempProcessBase.OpeningThread();

                ID = Project.Instance.CreateProcess();
                TempProcessBase = Project.Instance.GetProcessServiceById(ID);
                TempProcessBase.process.processInfo.CameraID = 5;
                TempProcessBase.process.processInfo.ProcessName = Service_Camera.GetCam_Name((int)CameraEnum.Cam6);
                TempProcessBase.m_eventINIProcess += Init极片尺寸带极耳检测流程;
                TempProcessBase.m_eventRUNProcess += ProProcess极片尺寸带极耳检测流程;
                TempProcessBase.InitProcess();
                TempProcessBase.OpeningThread();

             
            }






        }

        //初始化流程
        /// <summary>
        /// 线扫相机处理流程-裁拼图主相机
        /// </summary>
        /// <param name="process"></param>
        public void Init相机裁图流程(Process process)
        {
            process.m_ModuleObjList.Clear();
            int ProcessID = process.processInfo.ProcessID;
            int camID = (int)CameraEnum.Cam1;
            try
            {

                string pluginName;
                IPlugin Plugin;

                //基准边，用与涂层定位使用
                ParamsMeasureLine Params;
                pluginName = "直线抓边工具";
                Plugin = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                Plugin.moduleInfo.ModuleName = "涂层抓边定位";
                Plugin.moduleInfo.PluginName = pluginName;
                Params = Plugin.PluginParams as ParamsMeasureLine; //里氏转换
                process.AddModuleObj(Plugin);



                pluginName = "图像裁切工具(槽位，侧基准边)";
                IPlugin PluginCropLength = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                PluginCropLength.moduleInfo.ModuleName = "拼大图模块";
                PluginCropLength.moduleInfo.PluginName = pluginName;
                ParamsCropPart CropSideLengthParams = PluginCropLength.PluginParams as ParamsCropPart; //里氏转换
                CropSideLengthParams.CropShaftCol = 0;
                CropSideLengthParams.CropShaftRow = 0;
                process.AddModuleObj(PluginCropLength);

                pluginName = "图像裁切工具(槽位，侧基准边)";
                IPlugin PluginCropSmall = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                PluginCropSmall.moduleInfo.ModuleName = "拼小图模块";
                PluginCropSmall.moduleInfo.PluginName = pluginName;
                ParamsCropPart CropSideSmallParams = PluginCropLength.PluginParams as ParamsCropPart; //里氏转换
                CropSideSmallParams.CropShaftCol = 0;
                CropSideSmallParams.CropShaftRow = 0;
                process.AddModuleObj(PluginCropSmall);

                //pluginName = "模板匹配工具";
                //IPlugin PluginMatch = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                //PluginMatch.moduleInfo.ModuleName = "极耳&&Mark匹配";
                //PluginMatch.moduleInfo.PluginName = pluginName;
                //ParamsTemplateMatch paramsTemplateMatch = PluginMatch.PluginParams as ParamsTemplateMatch; //里氏转换
                //process.AddModuleObj(PluginMatch);



                pluginName = "两线交点工具";
                Plugin = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                Plugin.moduleInfo.ModuleName = "精定位交点模块";
                Plugin.moduleInfo.PluginName = pluginName;
                ParamsFindIntersection paramsFindIntersection = Plugin.PluginParams as ParamsFindIntersection;
                process.AddModuleObj(Plugin);

                //pluginName = "凸点测量检测";
                //IPlugin PluginReusableMeasure = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                //PluginReusableMeasure.moduleInfo.ModuleName = "精定位模块";
                //PluginReusableMeasure.moduleInfo.ModuleRemarks = "凸点测量检测插件&&精定位模块";
                //PluginReusableMeasure.moduleInfo.PluginName = pluginName;
                //ParamsReusableMeasureDetect paramsReusableMeasureDetect = PluginReusableMeasure.PluginParams as ParamsReusableMeasureDetect; //里氏转换
                //process.AddModuleObj(PluginReusableMeasure);



            }
            catch (Exception ee)
            {
                MessageNotify.AddErrorLog($"流程{ProcessID}__IniBaseCropProcess()方法异常\n+{ee.ToString()}");
                // throw;
            }
        }

        //初始化流程
        /// <summary>
        /// 线扫相机处理流程-裁拼图从相机
        /// </summary>
        /// <param name="process"></param>
        public void Init从相机裁图流程(Process process)
        {
            process.m_ModuleObjList.Clear();
            int ProcessID = process.processInfo.ProcessID;
            int camID = (int)CameraEnum.Cam2;
            try
            {
                string pluginName;

                pluginName = "图像裁切工具(槽位，侧基准边)";
                IPlugin PluginCropSide = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                PluginCropSide.moduleInfo.ModuleName = "裁拼图模块";
                PluginCropSide.moduleInfo.ModuleRemarks = "兼容拼图与裁图";
                PluginCropSide.moduleInfo.PluginName = pluginName;
                ParamsCropPart CropSideParams = PluginCropSide.PluginParams as ParamsCropPart; //里氏转换
                process.AddModuleObj(PluginCropSide);


            }
            catch (Exception ee)
            {
                MessageNotify.AddErrorLog($"流程{ProcessID}__IniFromCropProcess()方法异常\n+{ee.ToString()}");
                // throw;
            }
        }

        /// <summary>
        /// 线扫相机1&2的初始化流程
        /// </summary>
        /// <param name="process"></param>
        public void Init极片EA流程(Process process)
        {

            //接工艺，判断阴阳机型，0，阴极（带AT），1，阳极（不带AT）,统一右侧为极耳侧
            int PoleMode = (int)m_craft.Electrode;
            process.m_ModuleObjList.Clear();
            int ProcessID = process.processInfo.ProcessID;
            int camID = (int)CameraEnum.Cam1;
            int tabISLeft = m_craft.Get_GlobalVariableItem("极耳在左侧").Value.ToInt();
            bool isAT = m_craft.Get_GlobalVariableItem("带AT").Value.ToBool();

            try
            {

                string pluginName;
                IPlugin Plugin;

                //基准边，用与涂层定位使用
                ParamsMeasureLine Params;
                pluginName = "直线抓边工具";
                Plugin = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                Plugin.moduleInfo.ModuleName = "左侧涂层抓边定位";
                Plugin.moduleInfo.PluginName = pluginName;
                Params = Plugin.PluginParams as ParamsMeasureLine; //里氏转换
                process.AddModuleObj(Plugin);

                Plugin = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                Plugin.moduleInfo.ModuleName = "右侧涂层抓边定位";
                Plugin.moduleInfo.PluginName = pluginName;
                Params = Plugin.PluginParams as ParamsMeasureLine; //里氏转换
                process.AddModuleObj(Plugin);


                //Plugin = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                //Plugin.moduleInfo.ModuleName = "极耳抓边定位";
                //Plugin.moduleInfo.PluginName = pluginName;
                //Params = Plugin.PluginParams as ParamsMeasureLine; //里氏转换
                //process.AddModuleObj(Plugin);

                if(isAT)
                {
                    pluginName = "直线抓边工具";
                    Plugin = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                    Plugin.moduleInfo.ModuleName = "AT抓边定位";
                    Plugin.moduleInfo.PluginName = pluginName;
                    Params = Plugin.PluginParams as ParamsMeasureLine; //里氏转换
                    process.AddModuleObj(Plugin);

                    pluginName = "Blob分析工具";
                    Plugin = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                    Plugin.moduleInfo.ModuleName = "AT漏金属检测模块";
                    Plugin.moduleInfo.PluginName = pluginName;
                    ParamsBlobAnalysis paramsATBlobAnalysis = Plugin.PluginParams as ParamsBlobAnalysis; //里氏转换
                    process.AddModuleObj(Plugin);
                }

 


                pluginName = "Blob分析工具";
                Plugin = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                Plugin.moduleInfo.ModuleName = "边缘破损检测模块";
                Plugin.moduleInfo.PluginName = pluginName;
                ParamsBlobAnalysis paramsBlobAnalysis = Plugin.PluginParams as ParamsBlobAnalysis; //里氏转换
                process.AddModuleObj(Plugin);



                pluginName = "涂布缺陷检测";
                Plugin = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                Plugin.moduleInfo.ModuleName = "涂布检测";
                Plugin.moduleInfo.PluginName = pluginName;
                ParamsDefectCoat paramsDefectCoat = Plugin.PluginParams as ParamsDefectCoat; //里氏转换
                process.AddModuleObj(Plugin);


            }
            catch (Exception ee)
            {
                MessageNotify.AddErrorLog($"流程{ProcessID}__InitEAProcess()方法异常\n+{ee.ToString()}");
                // throw;
            }
        }
        /// <summary>
        /// 不带极耳侧
        /// </summary>
        /// <param name="process"></param>
        public void Init极片尺寸检测流程(Process process)
        {
            process.m_ModuleObjList.Clear();
            int ProcessID = process.processInfo.ProcessID;
            int camID = (int)CameraEnum.Cam1;
            string pluginName;
            IPlugin Plugin;
            try
            {
                pluginName = "CAC运行标定";
                Plugin = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                Plugin.moduleInfo.ModuleName = "棋盘格标定";
                Plugin.moduleInfo.PluginName = pluginName;
                ParamsCACRunCalibration paramsCACRunCalibration = Plugin.PluginParams as ParamsCACRunCalibration;
                process.AddModuleObj(Plugin);

                //粗定位交点工具
                pluginName = "两线交点工具";
                Plugin = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                Plugin.moduleInfo.ModuleName = "粗定位工具";
                Plugin.moduleInfo.PluginName = pluginName;
                ParamsFindIntersection paramsFindIntersection = Plugin.PluginParams as ParamsFindIntersection;
                process.AddModuleObj(Plugin);

                pluginName = "凸点测量检测";
                Plugin = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                Plugin.moduleInfo.ModuleName = "V角凸点1";
                Plugin.moduleInfo.PluginName = pluginName;
                ParamsReusableMeasureDetect paramsReusableMeasureDetect = Plugin.PluginParams as ParamsReusableMeasureDetect; //里氏转换
                process.AddModuleObj(Plugin);

                pluginName = "凸点测量检测";
                Plugin = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                Plugin.moduleInfo.ModuleName = "V角凸点2";
                Plugin.moduleInfo.PluginName = pluginName;
                paramsReusableMeasureDetect = Plugin.PluginParams as ParamsReusableMeasureDetect; //里氏转换
                process.AddModuleObj(Plugin);

                pluginName = "Blob分析工具";
                Plugin = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                Plugin.moduleInfo.ModuleName = "极片有无";
                Plugin.moduleInfo.PluginName = pluginName;
                ParamsBlobAnalysis paramsBlobAnalysis = Plugin.PluginParams as ParamsBlobAnalysis; //里氏转换
                process.AddModuleObj(Plugin);

                pluginName = "Blob分析工具";
                Plugin = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                Plugin.moduleInfo.ModuleName = "顶部V角检测有无";
                Plugin.moduleInfo.PluginName = pluginName;
                paramsBlobAnalysis = Plugin.PluginParams as ParamsBlobAnalysis; //里氏转换
                process.AddModuleObj(Plugin);

            }
            catch (Exception ee)
            {
                MessageNotify.AddErrorLog($"流程{ProcessID}__InitSizeProcess()方法异常\n+{ee.ToString()}");
                // throw;
            }






        }

        //带极耳侧
        public void Init极片尺寸带极耳检测流程(Process process)
        {
            //接工艺，判断阴阳机型，0，阴极（带AT），1，阳极（不带AT）,统一右侧为极耳侧
            int PoleMode = 0;
            process.m_ModuleObjList.Clear();
            int ProcessID = process.processInfo.ProcessID;
            int camID = (int)CameraEnum.Cam1;
            int tabISLeft = m_craft.Get_GlobalVariableItem("极耳在左侧").Value.ToInt();
            bool isAT = m_craft.Get_GlobalVariableItem("带AT").Value.ToBool();

            try
            {

                string pluginName;
                IPlugin Plugin;

                pluginName = "CAC运行标定";
                Plugin = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                Plugin.moduleInfo.ModuleName = "棋盘格标定";
                Plugin.moduleInfo.PluginName = pluginName;
                ParamsCACRunCalibration paramsCACRunCalibration = Plugin.PluginParams as ParamsCACRunCalibration;
                process.AddModuleObj(Plugin);


                //粗定位交点工具
                pluginName = "两线交点工具";
                Plugin = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                Plugin.moduleInfo.ModuleName = "粗定位工具";
                Plugin.moduleInfo.PluginName = pluginName;
                ParamsFindIntersection paramsFindIntersection = Plugin.PluginParams as ParamsFindIntersection;
                process.AddModuleObj(Plugin);

                //粗定位交点工具
                pluginName = "两线交点工具";
                Plugin = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                Plugin.moduleInfo.ModuleName = "极耳左侧交点";
                Plugin.moduleInfo.PluginName = pluginName;
                paramsFindIntersection = Plugin.PluginParams as ParamsFindIntersection;
                process.AddModuleObj(Plugin);

                pluginName = "直线抓边工具";
                Plugin = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                Plugin.moduleInfo.ModuleName = "极片右侧抓边";
                Plugin.moduleInfo.PluginName = pluginName;
                ParamsMeasureLine Params = Plugin.PluginParams as ParamsMeasureLine; //里氏转换
                process.AddModuleObj(Plugin);

                pluginName = "直线抓边工具";
                Plugin = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                Plugin.moduleInfo.ModuleName = "极耳右侧抓边";
                Plugin.moduleInfo.PluginName = pluginName;
                Params = Plugin.PluginParams as ParamsMeasureLine; //里氏转换
                process.AddModuleObj(Plugin);


                if(isAT||true)
                {
                    pluginName = "直线抓边工具";
                    Plugin = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                    Plugin.moduleInfo.ModuleName = "极片AT抓边";
                    Plugin.moduleInfo.PluginName = pluginName;
                    Params = Plugin.PluginParams as ParamsMeasureLine; //里氏转换
                    process.AddModuleObj(Plugin);

                }



                pluginName = "组合交点工具";
                Plugin = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                Plugin.moduleInfo.ModuleName = "极耳右侧组合交点";
                Plugin.moduleInfo.PluginName = pluginName;
                ParamsCombPoint paramsCombPoint = Plugin.PluginParams as ParamsCombPoint;
                process.AddModuleObj(Plugin);


                pluginName = "组合交点工具";
                Plugin = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                Plugin.moduleInfo.ModuleName = "极片右侧组合交点";
                Plugin.moduleInfo.PluginName = pluginName;
                paramsCombPoint = Plugin.PluginParams as ParamsCombPoint;
                process.AddModuleObj(Plugin);

                pluginName = "组合交点工具";
                Plugin = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                Plugin.moduleInfo.ModuleName = "极耳左根部组合交点";
                Plugin.moduleInfo.PluginName = pluginName;
                paramsCombPoint = Plugin.PluginParams as ParamsCombPoint;
                process.AddModuleObj(Plugin);


                pluginName = "组合交点工具";
                Plugin = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                Plugin.moduleInfo.ModuleName = "极耳右根部组合交点";
                Plugin.moduleInfo.PluginName = pluginName;
                paramsCombPoint = Plugin.PluginParams as ParamsCombPoint;
                process.AddModuleObj(Plugin);

                pluginName = "凸点测量检测";
                Plugin = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                Plugin.moduleInfo.ModuleName = "左侧V角凸点1";
                Plugin.moduleInfo.PluginName = pluginName;
                ParamsReusableMeasureDetect paramsReusableMeasureDetect = Plugin.PluginParams as ParamsReusableMeasureDetect; //里氏转换
                process.AddModuleObj(Plugin);

                pluginName = "凸点测量检测";
                Plugin = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                Plugin.moduleInfo.ModuleName = "左侧V角凸点2";
                Plugin.moduleInfo.PluginName = pluginName;
                paramsReusableMeasureDetect = Plugin.PluginParams as ParamsReusableMeasureDetect; //里氏转换
                process.AddModuleObj(Plugin);

                pluginName = "凸点测量检测";
                Plugin = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                Plugin.moduleInfo.ModuleName = "右侧V角凸点1";
                Plugin.moduleInfo.PluginName = pluginName;
                paramsReusableMeasureDetect = Plugin.PluginParams as ParamsReusableMeasureDetect; //里氏转换
                process.AddModuleObj(Plugin);

                pluginName = "凸点测量检测";
                Plugin = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                Plugin.moduleInfo.ModuleName = "右侧V角凸点2";
                Plugin.moduleInfo.PluginName = pluginName;
                paramsReusableMeasureDetect = Plugin.PluginParams as ParamsReusableMeasureDetect; //里氏转换
                process.AddModuleObj(Plugin);




                pluginName = "Blob分析工具";
                Plugin = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                Plugin.moduleInfo.ModuleName = "左底部V角检测有无";
                Plugin.moduleInfo.PluginName = pluginName;
                ParamsBlobAnalysis paramsBlobAnalysis = Plugin.PluginParams as ParamsBlobAnalysis; //里氏转换
                process.AddModuleObj(Plugin);

                pluginName = "Blob分析工具";
                Plugin = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                Plugin.moduleInfo.ModuleName = "右底部V角检测有无";
                Plugin.moduleInfo.PluginName = pluginName;
                paramsBlobAnalysis = Plugin.PluginParams as ParamsBlobAnalysis; //里氏转换
                process.AddModuleObj(Plugin);

                pluginName = "Blob分析工具";
                Plugin = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                Plugin.moduleInfo.ModuleName = "极片有无";
                Plugin.moduleInfo.PluginName = pluginName;
                paramsBlobAnalysis = Plugin.PluginParams as ParamsBlobAnalysis; //里氏转换
                process.AddModuleObj(Plugin);


                pluginName = "Blob分析工具";
                Plugin = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                Plugin.moduleInfo.ModuleName = "极耳翻折检测";
                Plugin.moduleInfo.PluginName = pluginName;
                paramsBlobAnalysis = Plugin.PluginParams as ParamsBlobAnalysis; //里氏转换
                process.AddModuleObj(Plugin);


                pluginName = "Blob分析工具";
                Plugin = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                Plugin.moduleInfo.ModuleName = "极耳余料检测";
                Plugin.moduleInfo.PluginName = pluginName;
                paramsBlobAnalysis = Plugin.PluginParams as ParamsBlobAnalysis; //里氏转换
                process.AddModuleObj(Plugin);

                pluginName = "Blob分析工具";
                Plugin = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                Plugin.moduleInfo.ModuleName = "极耳涂布内翻检测";
                Plugin.moduleInfo.PluginName = pluginName;
                paramsBlobAnalysis = Plugin.PluginParams as ParamsBlobAnalysis; //里氏转换
                process.AddModuleObj(Plugin);


            }
            catch (Exception ee)
            {
                MessageNotify.AddErrorLog($"流程{ProcessID}__InitSizePoleEarProcss()方法异常\n+{ee.ToString()}");
                //throw;
            }
        }


        /// <summary>
        /// 纠偏相机初始化
        /// </summary>
        /// <param name="process"></param>
        public void Init纠偏检测流程(Process process)
        {
            process.m_ModuleObjList.Clear();
            int ProcessID = process.processInfo.ProcessID;
            int camID = (int)CameraEnum.Cam1;
            string pluginName;
            IPlugin Plugin;
            try
            {
                //基准边，用于裁切纠偏定位使用
                ParamsMeasureLine Params;
                pluginName = "直线抓边工具";
                Plugin = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                Plugin.moduleInfo.ModuleName = "基准定位边";
                Plugin.moduleInfo.PluginName = pluginName;
                Params = Plugin.PluginParams as ParamsMeasureLine; //里氏转换
                process.AddModuleObj(Plugin);

                pluginName = "组合交点工具";
                Plugin = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                Plugin.moduleInfo.ModuleName = "基准定位交点";
                Plugin.moduleInfo.PluginName = pluginName;
                ParamsCombPoint paramsCombPoint = Plugin.PluginParams as ParamsCombPoint;
                process.AddModuleObj(Plugin);


                pluginName = "模板匹配工具";
                IPlugin PluginMatch = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                PluginMatch.moduleInfo.ModuleName = "极耳&&圆角匹配";
                PluginMatch.moduleInfo.PluginName = pluginName;
                ParamsTemplateMatch paramsTemplateMatch = PluginMatch.PluginParams as ParamsTemplateMatch; //里氏转换
                process.AddModuleObj(PluginMatch);


                pluginName = "两线交点工具";
                Plugin = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                Plugin.moduleInfo.ModuleName = "左极耳根部定位";
                Plugin.moduleInfo.PluginName = pluginName;
                ParamsFindIntersection paramsFindIntersection = Plugin.PluginParams as ParamsFindIntersection;
                process.AddModuleObj(Plugin);


                pluginName = "两线交点工具";
                Plugin = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                Plugin.moduleInfo.ModuleName = "右极耳根部定位";
                Plugin.moduleInfo.PluginName = pluginName;
                paramsFindIntersection = Plugin.PluginParams as ParamsFindIntersection;
                process.AddModuleObj(Plugin);
            }
            catch (Exception ee)
            {
                MessageNotify.AddErrorLog($"流程{ProcessID}__InitPosCorrect()方法异常\n+{ee.ToString()}");
                //throw;
            }
        }


        /// <summary>
        /// 线扫1预处理流程
        /// </summary>
        /// <param name="process"></param>
        public void ProProcess主相机裁图流程(Process process)
        {
            try
            {
                int ProcessID = process.processInfo.ProcessID;
                int camID = (int)CameraEnum.Cam1;
                int CurCamSide = camID;
                //设置当前流程绑定核
                SetThreadAffinityMask(GetCurrentThread(), new UIntPtr(SetCpuID(ProcessID)));
                //标定系数
                double Calipix_y = Service_Camera.GetCam_CalibrationValue_Y((int)CameraEnum.Cam1);
                double Calipix_x = Service_Camera.GetCam_CalibrationValue_X((int)CameraEnum.Cam1);
                //这里接工艺
                double ABMis = (m_craft.Get_GlobalVariableItem((int)CameraEnum.Cam1, "CamABGap")?.Value.ToDouble() ?? 0) / Calipix_y;
                //裁图高度比例系数
                double CropHeightScale = (m_craft.Get_GlobalVariableItem("CropHeightScale")?.Value.ToDouble() ?? 1);

                //工艺基准位，根据涂层抓边定位位置来
                PointInfo point基准 = new PointInfo() { Col = (m_craft.Get_GlobalVariableItem((int)CameraEnum.Cam1, "DefCamEALftBaseCol")?.Value.ToDouble() ?? 0), Row = 0 };
                //默认切刀位置
                double CutLine = (m_craft.Get_GlobalVariableItem((int)CameraEnum.Cam1, "DefCutPos")?.Value.ToDouble() ?? 0);
                //居中裁图偏移距离
                double midLine = (m_craft.Get_GlobalVariableItem("CropCenterOffset")?.Value.ToDouble() ?? 0);
                //极片高度
                double PieHeight = (m_craft.Get_GlobalVariableItem("PieHeight")?.Value.ToDouble() ?? 0) / Calipix_y;

                double midLineTemp =0;
                double topLineTemp = 0;
                double bottomLineTemp = 0;


                Thread CurrentThread = Thread.CurrentThread;
                CurrentThread.Priority = ThreadPriority.Highest;


                // 选择参数和队列基于是否模拟
                DieCutParam ProParam主相机模切业务 = Project.Instance.Is_simulation ? m_DicCamOperaParam_Sim[camID] as DieCutParam : m_DicCamOperaParam[camID] as DieCutParam;
                DieCutParam ProParam从相机模切业务 = Project.Instance.Is_simulation ? m_DicCamOperaParam_Sim[camID + 1] as DieCutParam : m_DicCamOperaParam[camID + 1] as DieCutParam;
                var imageQueue = ProParam主相机模切业务.alreadyDealGrabImage;
                //通过信号量把控线程
                ProParam主相机模切业务.semaphoreSlimCam.Wait(50);
                if (imageQueue.Count > 0) // 检查主相机的图像队列是否有图像
                {
                    DieCutImageProduct Product模切帧图 = new DieCutImageProduct();
                    HOperatorSet.CountSeconds(out Product模切帧图.detectionTime.Image_StartTime);

                    ImageInfo InfoImgInfo = imageQueue.Dequeue();
                    InfoImgInfo.nFirstLineEncoderCount = (long)((InfoImgInfo.image_ID - 1) * InfoImgInfo.imageHeight);
                    InfoImgInfo.nLastLineEncoderCount = (long)(InfoImgInfo.image_ID * InfoImgInfo.imageHeight);

                    //计算当前图片绝对位置
                    ProParam主相机模切业务.nFristAbsPosPixel = (long)((InfoImgInfo.image_ID - 1) * InfoImgInfo.imageHeight);
                    ProParam主相机模切业务.nLastAbsPosPixel = (long)(InfoImgInfo.image_ID * InfoImgInfo.imageHeight);



                    ProParam主相机模切业务.imageInfos.Add(InfoImgInfo);

                    if (ProParam主相机模切业务.imageInfos.Count > 2)
                    {
                        ProParam主相机模切业务.imageInfos.RemoveAt(0);
                    }



                    Product模切帧图.showHalconGroup = new System.Collections.ObjectModel.ObservableCollection<ShowHalconWindow>();
                    Product模切帧图.showHalconGroup.Add(new ShowHalconWindow()
                    {
                        WindowName = "原图",
                        m_nImageFrameID = InfoImgInfo.image_ID,

                    });
                    Product模切帧图.showHalconGroup.Add(new ShowHalconWindow()
                    {
                        WindowName = "拼接后",
                        m_nImageFrameID = InfoImgInfo.image_ID,

                    });
                    //用临时变量转接对象
                    ShowHalconWindow ShowHalconWindow1 = Product模切帧图.showHalconGroup[0];
                    ShowHalconWindow ShowHalconWindow2 = Product模切帧图.showHalconGroup[1];

                    //得有两张才能拼接大图
                    if (ProParam主相机模切业务.imageInfos.Count == 2)
                    {
                        //第一步,拼接大图显示
                        IPlugin CropPlugin = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == $"拼大图模块");
                        ParamsCropPart CropPartParams = CropPlugin.PluginParams as ParamsCropPart; //里氏转换
                        CropPartParams.list_ImageInfo = ProParam主相机模切业务.imageInfos;
                        ImageInfo image1 = ProParam主相机模切业务.imageInfos[0];
                        ImageInfo image2 = ProParam主相机模切业务.imageInfos[1];
                        //归一化
                        NormalizaLocation2((int)image1.image_ID, (image1.imageHeight + image2.imageHeight) / 2, image1.imageHeight, out int id, out double line);
                        CropPartParams.PartSizeRow = (image1.imageHeight + image2.imageHeight) * 0.95;
                        CropPartParams.PartSizeCol = image1.imageWidth;
                        CropPartParams.PartCenterID = id;
                        CropPartParams.PartCenterRow = line;
                        CropPartParams.PartCenterCol = image1.imageWidth / 2;
                        if (CropPlugin.ExecuteObj())
                        {
                            ShowHalconWindow1.m_hImage = InfoImgInfo.ho_image;
                            ShowHalconWindow1.m_nImageFrameID = InfoImgInfo.image_ID;

                            ShowHalconWindow2.m_hImage = CropPartParams.Image_part;
                            ShowHalconWindow2.m_nImageFrameID = InfoImgInfo.image_ID;

                            if (CropPartParams.hv_ImagePartBeginID != image1.image_ID)
                            {

                                MessageNotify.AddErrorLog("裁拼图模块执行失败:" + "裁剪后的图像开始ID与传入的图像ID不一致");
                                HOperatorSet.CountSeconds(out Product模切帧图.detectionTime.Image_EndTime);
                                ProParam主相机模切业务.ProductCallBack(Product模切帧图);
                                return;
                            }


                            IPlugin PluginLine = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == $"涂层抓边定位");
                            ParamsMeasureLine paramsMeasureLine = PluginLine.PluginParams as ParamsMeasureLine;
                            paramsMeasureLine.ho_Image = CropPartParams.Image_part;
                            LineInfo MeasureLineInfo = new LineInfo() { StrLineName = "涂层抓边定位", strMoudleName = "涂层抓边定位", nProcessID = ProcessID };


                            //抓边结果总结
                            DetectEdgeValueAssign(paramsMeasureLine, PluginLine.ExecuteObj(), MeasureLineInfo, ShowHalconWindow2.m_messageShowInfos,
                                       ShowHalconWindow2.m_lineShowInfos, ShowHalconWindow2.m_regionShowInfos);

                            Product模切帧图.LineInfosResult.Add(MeasureLineInfo);

                            IPlugin PluginMatch = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == $"极耳&&Mark匹配");
                            ParamsTemplateMatch paramsTemplateMatch = PluginMatch.PluginParams as ParamsTemplateMatch;
                            paramsTemplateMatch.ho_Image = CropPartParams.Image_part;
                            //这里的位置跟随默认启用//模块需定义好模块基准位置
                            paramsTemplateMatch.m_bUsePositionFollow = true;
                            paramsTemplateMatch.m_fFollowBaseCol = point基准.Col;
                            paramsTemplateMatch.m_fFollowCurrentRow = 0;
                            paramsTemplateMatch.m_fFollowCurrentCol = (MeasureLineInfo.ColBegin + MeasureLineInfo.ColEnd) / 2;
                            paramsTemplateMatch.m_fFollowCurrentRow = 0;
                            if (PluginMatch.ExecuteObj())
                            {

                                ShowHalconWindow2.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = paramsTemplateMatch.ho_OutMatchRoiRegion.CopyObj(1, -1), m_stuGetColor = HalconColor.orange });

                                if (paramsTemplateMatch.m_lMaskRoiParams.Count > 0 && paramsTemplateMatch.ho_OutMatchRoiRegion != null)
                                {
                                    // ShowHalconWindow2.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = paramsTemplateMatch.ho_OutMaskRoiRegion.CopyObj(1, -1), m_stuGetColor = HalconColor.orange });
                                }



                                //选择分数最好的那个
                                var bestResult = paramsTemplateMatch.Results.OrderByDescending(r => r.Score).FirstOrDefault();

                                // 生成十字交叉线并添加到显示窗口
                                HOperatorSet.GenCrossContourXld(out HObject cross, bestResult.Row /*+ bestResult.OffsetY*/, bestResult.Col/* + bestResult.OffsetX*/, 20, 45);
                                ShowHalconWindow2.m_regionShowInfos.Add(new RegionShowInfo()
                                {
                                    m_hRegion = cross,
                                    m_stuGetColor = HalconColor.gold
                                });

                                PointInfo point跟随 = new PointInfo() { Col = bestResult.Col + bestResult.OffsetX, Row = bestResult.Row + bestResult.OffsetY };
                                PointInfo pointMatch = new PointInfo() { Col = bestResult.Col, Row = bestResult.Row };
                                IPlugin Plugin精定位交点模块 = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == $"精定位交点模块");
                                ImageInfo imageInfo = new ImageInfo() { ho_image = CropPartParams.Image_part };
                                bool success = ProcessCoarseLocation(imageInfo, Plugin精定位交点模块, ProcessID, ShowHalconWindow2, out OutCrossGroup Cross精定位交点模块, pointMatch, point跟随, "精定位模块", true, true);
                                if (!success)
                                {
                                    CutLine = ProParam主相机模切业务.nLastCutLine > 0 ? ProParam主相机模切业务.nLastCutLine : CutLine;
                                    Log4netRecord.ErrorLog.Debug($"流程{Plugin精定位交点模块.moduleInfo.ProcessID}的{Plugin精定位交点模块.moduleInfo.ModuleName}执行失败");
                                }
                                else
                                {
                                    // 生成十字交叉线并添加到显示窗口
                                    HOperatorSet.GenCrossContourXld(out HObject cross1, Cross精定位交点模块.point.Row, Cross精定位交点模块.point.Col, 20, 45);
                                    ShowHalconWindow2.m_regionShowInfos.Add(new RegionShowInfo()
                                    {
                                        m_hRegion = cross1,
                                        m_stuGetColor = HalconColor.gold
                                    });
                                    CutLine = Cross精定位交点模块.point.Row;
                                    ProParam主相机模切业务.nLastCutLine = CutLine;
                                }

                            }
                            else
                            {
                                CutLine = ProParam主相机模切业务.nLastCutLine > 0 ? ProParam主相机模切业务.nLastCutLine : CutLine;
                            }

                            //这里进行位置推算
                            //第一步，判断位置是否满足裁图条件
                            HOperatorSet.GetImageSize(CropPartParams.Image_part, out HTuple width, out HTuple height);
                            midLineTemp = midLine + CutLine;
                            topLineTemp = midLineTemp - (PieHeight * CropHeightScale) / 2;
                            bottomLineTemp = midLineTemp + (PieHeight * CropHeightScale) / 2;

                            //第二步，判断三个位置是否满足裁图条件，<0,或者大于图像高度都不行,日志输出各自位置值，正常来说不应该出现这种情况，一定是参数存在问题

                            if (topLineTemp < 0 || topLineTemp > height || bottomLineTemp < 0 || bottomLineTemp > height || midLineTemp < 0 || midLineTemp > height)
                            {
                                ImageInfo EAImageInfo = new ImageInfo();
                                EAImageInfo.NgType = 1;
                                EAImageInfo.ho_image = image1.ho_image.CopyObj(1,-1);
                                EAImageInfo.EAID = ProParam主相机模切业务.nEAId;
                                EAImageInfo.nFirstLineEncoderCount = image1.nFirstLineEncoderCount;
                                EAImageInfo.nLastLineEncoderCount = image1.nLastLineEncoderCount;
                                EAImageInfo.imageWidth = InfoImgInfo.imageWidth;
                                EAImageInfo.imageHeight = EAImageInfo.nLastLineEncoderCount - EAImageInfo.nFirstLineEncoderCount;
                                EAImageInfo.ImageDescription = $"第{EAImageInfo.EAID}片EA";

                                ProParam主相机模切业务.imageEAInfos.Add(EAImageInfo);
                                ProParam主相机模切业务.semaphoreSlim.Release();

                                //推算裁断位
                                LineInfo lineInfo = new LineInfo();
                                lineInfo.RowBegin = EAImageInfo.nFirstLineEncoderCount + ABMis;
                                lineInfo.RowEnd = EAImageInfo.nLastLineEncoderCount + ABMis;
                                lineInfo.ColBegin = 0;
                                lineInfo.NgType = 1;
                                lineInfo.ColEnd = image1.imageWidth;
                                lineInfo.EAID = ProParam主相机模切业务.nEAId;
                                ProParam从相机模切业务.BufferSplitLines.Add(lineInfo);
                                ProParam主相机模切业务.nEAId++;

                                ShowHalconWindow2.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = ("裁剪后的图像不满足裁图条件\n" + "裁剪后的图像上边位置值:" + topLineTemp.ToString() + "\n裁剪后的图像下边位置值:" + bottomLineTemp.ToString() + "\n裁剪后的图像中间位置值:" + midLineTemp.ToString()), m_stuGetColor = HalconColor.red });

                                ShowHalconWindow1.m_nImageFrameID = InfoImgInfo.image_ID;
                                ShowHalconWindow1.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前帧号:{InfoImgInfo.image_ID.ToString()}" });

                                ShowHalconWindow2.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前帧号:{InfoImgInfo.image_ID.ToString()}" });
                                ShowHalconWindow2.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前EA号:{ProParam主相机模切业务.nEAId - 1}" });
                                ShowHalconWindow2.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前EA号:{ProParam主相机模切业务.nEAId - 1}_首行裁断位", m_bIsWindow = false, m_fCol = image1.imageWidth / 2, m_fRow = 1 });
                                ShowHalconWindow2.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前EA号:{ProParam主相机模切业务.nEAId - 1}_尾行裁断位", m_bIsWindow = false, m_fCol = image1.imageWidth / 2, m_fRow = image1.imageHeight });

                                ShowHalconWindow2.m_lineShowInfos.Add(new LineInfo() { RowBegin = 1, RowEnd = 1, ColBegin = 0, ColEnd = image1.imageWidth });
                                ShowHalconWindow2.m_lineShowInfos.Add(new LineInfo() { RowBegin = image1.imageHeight, RowEnd = image1.imageHeight, ColBegin = 0, ColEnd = image1.imageWidth });

                                MessageNotify.AddErrorLog("裁剪后的图像不满足裁图条件\n" + "裁剪后的图像上边位置值:" + topLineTemp.ToString() + "\n裁剪后的图像下边位置值:" + bottomLineTemp.ToString() + "\n裁剪后的图像中间位置值:" + midLineTemp.ToString());
                                HOperatorSet.CountSeconds(out Product模切帧图.detectionTime.Image_EndTime);
                               
                                ProParam主相机模切业务.ProductCallBack(Product模切帧图);
                                return;
                            }

                            //第三步裁图，用于完整EA使用
                            List<ImageInfo> imageInfos = new List<ImageInfo>();
                            imageInfos.Add(new ImageInfo() { ho_image = CropPartParams.Image_part, image_ID = 1 });
                            IPlugin EACropPlugin = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == $"拼小图模块");

                            ParamsCropPart EACropPartParams = EACropPlugin.PluginParams as ParamsCropPart; //里氏转换
                            EACropPartParams.list_ImageInfo = imageInfos;
                            EACropPartParams.PartSizeRow = (bottomLineTemp - topLineTemp);
                            EACropPartParams.PartSizeCol = width;
                            EACropPartParams.PartCenterID = 1;
                            EACropPartParams.PartCenterRow = midLineTemp;
                            EACropPartParams.PartCenterCol = width / 2;


                            if (EACropPlugin.ExecuteObj())
                            {

                                ImageInfo EAImageInfo = new ImageInfo();
                                EAImageInfo.ho_image = EACropPartParams.Image_part;
                                EAImageInfo.EAID = ProParam主相机模切业务.nEAId;
                                EAImageInfo.nFirstLineEncoderCount = image1.nFirstLineEncoderCount + ((int)EACropPartParams.hv_ImagePartBeginRow.D) + (int)((image1.imageHeight + image2.imageHeight) * 0.05 / 2);
                                EAImageInfo.nLastLineEncoderCount = EAImageInfo.nFirstLineEncoderCount + (int)(bottomLineTemp - topLineTemp);
                                EAImageInfo.imageWidth = InfoImgInfo.imageWidth;
                                EAImageInfo.imageHeight = EAImageInfo.nLastLineEncoderCount - EAImageInfo.nFirstLineEncoderCount;
                                EAImageInfo.ImageDescription = $"第{EAImageInfo.EAID}片EA";

                                ProParam主相机模切业务.imageEAInfos.Add(EAImageInfo);
                                ProParam主相机模切业务.semaphoreSlim.Release();


                                //推算裁断位
                                LineInfo lineInfo = new LineInfo();
                                lineInfo.RowBegin = EAImageInfo.nFirstLineEncoderCount + ABMis;
                                lineInfo.RowEnd = EAImageInfo.nLastLineEncoderCount + ABMis;
                                lineInfo.ColBegin = 0;
                                lineInfo.ColEnd = width;
                                lineInfo.EAID = ProParam主相机模切业务.nEAId;
                                ProParam从相机模切业务.BufferSplitLines.Add(lineInfo);
                              



                            }
                            else
                            {


                                ImageInfo EAImageInfo = new ImageInfo();
                                EAImageInfo.NgType = 1;
                                EAImageInfo.ho_image = image1.ho_image.CopyObj(1,-1);
                                EAImageInfo.EAID = ProParam主相机模切业务.nEAId;
                                EAImageInfo.nFirstLineEncoderCount = image1.nFirstLineEncoderCount;
                                EAImageInfo.nLastLineEncoderCount = image1.nLastLineEncoderCount;
                                EAImageInfo.imageWidth = InfoImgInfo.imageWidth;
                                EAImageInfo.imageHeight = EAImageInfo.nLastLineEncoderCount - EAImageInfo.nFirstLineEncoderCount;
                                EAImageInfo.ImageDescription = $"第{EAImageInfo.EAID}片EA";

                                ProParam主相机模切业务.imageEAInfos.Add(EAImageInfo);
                                ProParam主相机模切业务.semaphoreSlim.Release();

                                //推算裁断位
                                LineInfo lineInfo = new LineInfo();
                                lineInfo.NgType = 1;
                                lineInfo.RowBegin = EAImageInfo.nFirstLineEncoderCount + ABMis;
                                lineInfo.RowEnd = EAImageInfo.nLastLineEncoderCount + ABMis;
                                lineInfo.ColBegin = 0;
                                lineInfo.ColEnd = image1.imageWidth;
                                lineInfo.EAID = ProParam主相机模切业务.nEAId;
                                ProParam从相机模切业务.BufferSplitLines.Add(lineInfo);
                                ShowHalconWindow2.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = "拼小图模块失败", m_stuGetColor = HalconColor.red });
                                ShowHalconWindow1.m_nImageFrameID = InfoImgInfo.image_ID;
                                ShowHalconWindow1.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前帧号:{InfoImgInfo.image_ID.ToString()}" });
                                ShowHalconWindow2.m_nProductID = ProParam主相机模切业务.nEAId;
                                ShowHalconWindow2.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前帧号:{InfoImgInfo.image_ID.ToString()}" });
                                ShowHalconWindow2.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前EA号:{ProParam主相机模切业务.nEAId}" });
                                ShowHalconWindow2.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前EA号:{ProParam主相机模切业务.nEAId}_首行裁断位", m_bIsWindow = false, m_fCol = image1.imageWidth / 2, m_fRow = 1 });
                                ShowHalconWindow2.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前EA号:{ProParam主相机模切业务.nEAId}_尾行裁断位", m_bIsWindow = false, m_fCol = image1.imageWidth / 2, m_fRow = image1.imageHeight });

                                ShowHalconWindow2.m_lineShowInfos.Add(new LineInfo() { RowBegin = 1, RowEnd = 1, ColBegin = 0, ColEnd = image1.imageWidth });
                                ShowHalconWindow2.m_lineShowInfos.Add(new LineInfo() { RowBegin = image1.imageHeight, RowEnd = image1.imageHeight, ColBegin = 0, ColEnd = image1.imageWidth });
                                ProParam主相机模切业务.nEAId++;

                                //不应该失败，肯定跟参数设置有关
                                MessageNotify.AddErrorLog("拼小图模块执行失败:" + "不应该失败，推理的肯定跟参数设置有关，检查参数");
                                HOperatorSet.CountSeconds(out Product模切帧图.detectionTime.Image_EndTime);
                                ProParam主相机模切业务.ProductCallBack(Product模切帧图);
                                return;

                            }

                            ShowHalconWindow1.m_nImageFrameID = InfoImgInfo.image_ID;
                            ShowHalconWindow1.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前帧号:{InfoImgInfo.image_ID.ToString()}" });
                            ShowHalconWindow2.m_nImageFrameID = InfoImgInfo.image_ID;
                            ShowHalconWindow2.m_nProductID = ProParam主相机模切业务.nEAId ;

                            ShowHalconWindow2.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前帧号:{InfoImgInfo.image_ID.ToString()}" });
                            ShowHalconWindow2.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前EA号:{ProParam主相机模切业务.nEAId}" });
                            ShowHalconWindow2.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前EA号:{ProParam主相机模切业务.nEAId}_首行裁断位", m_bIsWindow = false, m_fCol = width / 2, m_fRow = EACropPartParams.hv_ImagePartBeginRow });
                            ShowHalconWindow2.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前EA号:{ProParam主相机模切业务.nEAId}_尾行裁断位", m_bIsWindow = false, m_fCol = width / 2, m_fRow = EACropPartParams.hv_ImagePartEndRow });

                            ShowHalconWindow2.m_lineShowInfos.Add(new LineInfo() { RowBegin = EACropPartParams.hv_ImagePartBeginRow, RowEnd = EACropPartParams.hv_ImagePartBeginRow, ColBegin = 0, ColEnd = width });
                            ShowHalconWindow2.m_lineShowInfos.Add(new LineInfo() { RowBegin = EACropPartParams.hv_ImagePartEndRow, RowEnd = EACropPartParams.hv_ImagePartEndRow, ColBegin = 0, ColEnd = width });
                            ProParam主相机模切业务.nEAId++;



                        }
                        else
                        {
                            ImageInfo EAImageInfo = new ImageInfo();
                            EAImageInfo.NgType = 1;
                            EAImageInfo.ho_image = image1.ho_image.CopyObj(1,-1);
                            EAImageInfo.EAID = ProParam主相机模切业务.nEAId;
                            EAImageInfo.nFirstLineEncoderCount = image1.nFirstLineEncoderCount;
                            EAImageInfo.nLastLineEncoderCount = image1.nLastLineEncoderCount;
                            EAImageInfo.imageWidth = InfoImgInfo.imageWidth;
                            EAImageInfo.imageHeight = EAImageInfo.nLastLineEncoderCount - EAImageInfo.nFirstLineEncoderCount;
                            EAImageInfo.ImageDescription = $"第{EAImageInfo.EAID}片EA";

                            ProParam主相机模切业务.imageEAInfos.Add(EAImageInfo);
                            ProParam主相机模切业务.semaphoreSlim.Release();

                            //推算裁断位
                            LineInfo lineInfo = new LineInfo();
                            lineInfo.NgType = 1;
                            lineInfo.RowBegin = EAImageInfo.nFirstLineEncoderCount + ABMis;
                            lineInfo.RowEnd = EAImageInfo.nLastLineEncoderCount + ABMis;
                            lineInfo.ColBegin = 0;
                            lineInfo.ColEnd = image1.imageWidth;
                            lineInfo.EAID = ProParam主相机模切业务.nEAId;
                            ProParam从相机模切业务.BufferSplitLines.Add(lineInfo);
                         

                            ShowHalconWindow2.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = "拼接大图失败", m_stuGetColor = HalconColor.red });
                            ShowHalconWindow1.m_hImage = InfoImgInfo.ho_image;
                            ShowHalconWindow1.m_nImageFrameID = InfoImgInfo.image_ID;
                            ShowHalconWindow1.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前帧号:{InfoImgInfo.image_ID.ToString()}" });
                            ShowHalconWindow2.m_hImage = InfoImgInfo.ho_image.CopyObj(1, -1);
                            ShowHalconWindow2.m_nImageFrameID = InfoImgInfo.image_ID;
                            ShowHalconWindow2.m_nProductID = ProParam主相机模切业务.nEAId;

                            ShowHalconWindow2.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前帧号:{InfoImgInfo.image_ID.ToString()}" });
                            ShowHalconWindow2.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前EA号:{ProParam主相机模切业务.nEAId}" });
                            ShowHalconWindow2.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前EA号:{ProParam主相机模切业务.nEAId}_首行裁断位", m_bIsWindow = false, m_fCol = image1.imageWidth / 2, m_fRow = 1 });
                            ShowHalconWindow2.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前EA号:{ProParam主相机模切业务.nEAId}_尾行裁断位", m_bIsWindow = false, m_fCol = image1.imageWidth / 2, m_fRow = image1.imageHeight });
                            ShowHalconWindow2.m_lineShowInfos.Add(new LineInfo() { RowBegin = 1, RowEnd = 1, ColBegin = 0, ColEnd = image1.imageWidth });
                            ShowHalconWindow2.m_lineShowInfos.Add(new LineInfo() { RowBegin = image1.imageHeight, RowEnd = image1.imageHeight, ColBegin = 0, ColEnd = image1.imageWidth });
                            ProParam主相机模切业务.nEAId++;
                            MessageNotify.AddErrorLog("裁拼图模块执行失败:" + "检查传入的参数是否正确");
                        }

                    }
                    //这里为开机的第一片，默认用第一片
                    else
                    {

                        InfoImgInfo.EAID = ProParam主相机模切业务.nEAId;
                        InfoImgInfo.NgType = 2;
                        InfoImgInfo.ImageDescription = "首片NG";

                        ProParam主相机模切业务.imageEAInfos.Add(InfoImgInfo);
                        ProParam主相机模切业务.semaphoreSlim.Release();

                        //推算裁断位
                        LineInfo lineInfo = new LineInfo();
                        lineInfo.EAID = ProParam主相机模切业务.nEAId;
                        lineInfo.RowBegin = ABMis;
                        lineInfo.RowEnd = (ABMis + InfoImgInfo.imageHeight);
                        lineInfo.ColBegin = 0;
                        lineInfo.ColEnd = InfoImgInfo.imageWidth;
                        ProParam从相机模切业务.BufferSplitLines.Add(lineInfo);


                        ShowHalconWindow1.m_hImage = InfoImgInfo.ho_image.CopyObj(1, -1);
                        ShowHalconWindow1.m_nImageFrameID = InfoImgInfo.image_ID;

                        ShowHalconWindow1.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前帧号:{InfoImgInfo.image_ID.ToString()}" });
                        ShowHalconWindow2.m_hImage = InfoImgInfo.ho_image.CopyObj(1, -1);
                        ShowHalconWindow2.m_nImageFrameID = InfoImgInfo.image_ID;
                        ShowHalconWindow2.m_nProductID = ProParam主相机模切业务.nEAId;
                        ShowHalconWindow2.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前帧号:{InfoImgInfo.image_ID.ToString()}" });
                        ShowHalconWindow2.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前EA号:{ProParam主相机模切业务.nEAId}" });

                        ProParam主相机模切业务.nEAId++;
                    }

                    HOperatorSet.CountSeconds(out Product模切帧图.detectionTime.Image_EndTime);
                    HTuple end = (Product模切帧图.detectionTime.Image_EndTime - Product模切帧图.detectionTime.Image_StartTime) * 1000;
                    ShowHalconWindow2.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前耗时:{end.D.ToString("0.00")}" });
                    ProParam主相机模切业务.ProductCallBack(Product模切帧图);


                }


            }
            catch (Exception ee)
            {
                MessageNotify.AddErrorLog("ProProcessCropBaseCam():" + ee.ToString());
                System.Windows.Forms.MessageBox.Show("ProProcessCropBaseCam():" + ee.ToString(), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //throw;
            }
        }


        public void ProProcess新相机裁图流程(Process process)
        {
            try
            {
                int ProcessID = process.processInfo.ProcessID;
                int camID = ProcessID==0?0:1;
                int CurCamSide = camID;
                //设置当前流程绑定核
                SetThreadAffinityMask(GetCurrentThread(), new UIntPtr(SetCpuID(ProcessID)));
                //标定系数
                double Calipix_y = Service_Camera.GetCam_CalibrationValue_Y(camID);
                double Calipix_x = Service_Camera.GetCam_CalibrationValue_X(camID);
                //这里接工艺
               // double ABMis = (m_craft.Get_GlobalVariableItem(camID, "CamABGap")?.Value.ToDouble() ?? 0) / Calipix_y;
                //裁图高度比例系数
                double CropHeightScale = (m_craft.Get_GlobalVariableItem("CropHeightScale")?.Value.ToDouble() ?? 1);

                //工艺基准位，根据涂层抓边定位位置来
                PointInfo point基准 = new PointInfo() { Col = (m_craft.Get_GlobalVariableItem((int)CameraEnum.Cam1, "DefCamEALftBaseCol")?.Value.ToDouble() ?? 0), Row = 0 };
                //默认切刀位置
                double CutLine = (m_craft.Get_GlobalVariableItem(camID, "DefCutPos")?.Value.ToDouble() ?? 0);
                //居中裁图偏移距离
                double midLine = (m_craft.Get_GlobalVariableItem("CropCenterOffset")?.Value.ToDouble() ?? 0);
                //极片高度
                double PieHeight = (m_craft.Get_GlobalVariableItem("PieHeight")?.Value.ToDouble() ?? 0) / Calipix_y;

                double midLineTemp = 0;
                double topLineTemp = 0;
                double bottomLineTemp = 0;

                Thread CurrentThread = Thread.CurrentThread;
                CurrentThread.Priority = ThreadPriority.Highest;

                // 选择参数和队列基于是否模拟
                DieCutParam ProParam相机模切业务 = Project.Instance.Is_simulation ? m_DicCamOperaParam_Sim[camID] as DieCutParam : m_DicCamOperaParam[camID] as DieCutParam;
                var imageQueue = ProParam相机模切业务.alreadyDealGrabImage;
                //通过信号量把控线程
                ProParam相机模切业务.semaphoreSlimCam.Wait(50);
                if (imageQueue.Count > 0) // 检查主相机的图像队列是否有图像
                {
                    DieCutImageProduct Product模切帧图 = new DieCutImageProduct();
                    HOperatorSet.CountSeconds(out Product模切帧图.detectionTime.Image_StartTime);

                    ImageInfo InfoImgInfo = imageQueue.Dequeue();
                    InfoImgInfo.nFirstLineEncoderCount = (long)((InfoImgInfo.image_ID - 1) * InfoImgInfo.imageHeight);
                    InfoImgInfo.nLastLineEncoderCount = (long)(InfoImgInfo.image_ID * InfoImgInfo.imageHeight);

                    //计算当前图片绝对位置
                    ProParam相机模切业务.nFristAbsPosPixel = (long)((InfoImgInfo.image_ID - 1) * InfoImgInfo.imageHeight);
                    ProParam相机模切业务.nLastAbsPosPixel = (long)(InfoImgInfo.image_ID * InfoImgInfo.imageHeight);



                    ProParam相机模切业务.imageInfos.Add(InfoImgInfo);

                    if (ProParam相机模切业务.imageInfos.Count > 2)
                    {
                        ProParam相机模切业务.imageInfos.RemoveAt(0);
                    }



                    Product模切帧图.showHalconGroup = new System.Collections.ObjectModel.ObservableCollection<ShowHalconWindow>();
                    Product模切帧图.showHalconGroup.Add(new ShowHalconWindow()
                    {
                        WindowName = "原图",
                        m_nImageFrameID = InfoImgInfo.image_ID,

                    });
                    Product模切帧图.showHalconGroup.Add(new ShowHalconWindow()
                    {
                        WindowName = "拼接后",
                        m_nImageFrameID = InfoImgInfo.image_ID,

                    });
                    //用临时变量转接对象
                    ShowHalconWindow ShowHalconWindow1 = Product模切帧图.showHalconGroup[0];
                    ShowHalconWindow ShowHalconWindow2 = Product模切帧图.showHalconGroup[1];

                    //得有两张才能拼接大图
                    if (ProParam相机模切业务.imageInfos.Count == 2)
                    {
                        //第一步,拼接大图显示
                        IPlugin CropPlugin = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == $"拼大图模块");
                        ParamsCropPart CropPartParams = CropPlugin.PluginParams as ParamsCropPart; //里氏转换
                        CropPartParams.list_ImageInfo = ProParam相机模切业务.imageInfos;
                        ImageInfo image1 = ProParam相机模切业务.imageInfos[0];
                        ImageInfo image2 = ProParam相机模切业务.imageInfos[1];
                        //归一化
                         NormalizaLocation2((int)image1.image_ID, (image1.imageHeight + image2.imageHeight) / 2, image1.imageHeight, out int id, out double line);
                   
                        CropPartParams.PartSizeRow =(int)((image1.imageHeight + image2.imageHeight));
                        CropPartParams.PartSizeCol = image1.imageWidth;
                        CropPartParams.PartCenterID = id;
                        CropPartParams.PartCenterRow = line;
                        CropPartParams.PartCenterCol = image1.imageWidth / 2;
                        bool flag = false;
                        HOperatorSet.CountSeconds(out HTuple start);
                        try
                        {
                            HOperatorSet.ConcatObj(image1.ho_image, image2.ho_image, out CropPartParams.Image_part);
                            HOperatorSet.TileImagesOffset(CropPartParams.Image_part, out CropPartParams.Image_part, (new HTuple(0)).TupleConcat(image1.imageHeight), (new HTuple(0)).TupleConcat(0),
                                                         (new HTuple(-1)).TupleConcat(-1), (new HTuple(-1)).TupleConcat(-1), (new HTuple(-1)).TupleConcat(-1), (new HTuple(-1)).TupleConcat(-1),
                                                          image1.imageWidth, image1.imageHeight + image2.imageHeight);
                            flag = true;
                        }
                        catch (Exception)
                        {
                            flag = false;
                            //throw;
                        }
                        finally
                        {

                        }
                        HOperatorSet.CountSeconds(out HTuple end1);

                        CropPartParams.m_fElapsedMs = (end1.D - start.D) * 1000.0;
                        CropPartParams.m_bRunSuccess = flag;

                        if (flag)
                        {
                            ShowHalconWindow1.m_hImage = InfoImgInfo.ho_image;
                            ShowHalconWindow1.m_nImageFrameID = InfoImgInfo.image_ID;

                            ShowHalconWindow2.m_hImage = CropPartParams.Image_part;
                            ShowHalconWindow2.m_nImageFrameID = InfoImgInfo.image_ID;

                            //if (CropPartParams.hv_ImagePartBeginID != image1.image_ID)
                            //{

                            //    MessageNotify.AddErrorLog("裁拼图模块执行失败:" + "裁剪后的图像开始ID与传入的图像ID不一致");
                            //    HOperatorSet.CountSeconds(out Product模切帧图.detectionTime.Image_EndTime);
                            //    ProParam相机模切业务.ProductCallBack(Product模切帧图);
                            //    return;
                            //}


                            IPlugin PluginLine = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == $"涂层抓边定位");
                            ParamsMeasureLine paramsMeasureLine = PluginLine.PluginParams as ParamsMeasureLine;
                            paramsMeasureLine.ho_Image = CropPartParams.Image_part;
                            LineInfo MeasureLineInfo = new LineInfo() { StrLineName = "涂层抓边定位", strMoudleName = "涂层抓边定位", nProcessID = ProcessID };


                            //抓边结果总结
                            DetectEdgeValueAssign(paramsMeasureLine, PluginLine.ExecuteObj(), MeasureLineInfo, ShowHalconWindow2.m_messageShowInfos,
                                       ShowHalconWindow2.m_lineShowInfos, ShowHalconWindow2.m_regionShowInfos);

                            Product模切帧图.LineInfosResult.Add(MeasureLineInfo);
                            PointInfo point跟随 = new PointInfo() { Col = (MeasureLineInfo.ColBegin + MeasureLineInfo.ColEnd) / 2, Row = 0 };
                            PointInfo pointMatch = new PointInfo() { Col = point基准.Col, Row = 0 };
                            IPlugin Plugin精定位交点模块 = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == $"精定位交点模块");
                            ImageInfo imageInfo = new ImageInfo() { ho_image = CropPartParams.Image_part };
                            bool success = ProcessCoarseLocation(imageInfo, Plugin精定位交点模块, ProcessID, ShowHalconWindow2, out OutCrossGroup Cross精定位交点模块, pointMatch, point跟随, "精定位模块", true, true);
                            if (!success)
                            {
                                CutLine = ProParam相机模切业务.nLastCutLine > 0 ? ProParam相机模切业务.nLastCutLine : CutLine;
                                Log4netRecord.ErrorLog.Debug($"流程{Plugin精定位交点模块.moduleInfo.ProcessID}的{Plugin精定位交点模块.moduleInfo.ModuleName}执行失败");
                            }
                            else
                            {
                                // 生成十字交叉线并添加到显示窗口
                                HOperatorSet.GenCrossContourXld(out HObject cross1, Cross精定位交点模块.point.Row, Cross精定位交点模块.point.Col, 20, 45);
                                ShowHalconWindow2.m_regionShowInfos.Add(new RegionShowInfo()
                                {
                                    m_hRegion = cross1,
                                    m_stuGetColor = HalconColor.gold
                                });
                                CutLine = Cross精定位交点模块.point.Row;
                                ProParam相机模切业务.nLastCutLine = CutLine;
                            }
                            CutLine = ProParam相机模切业务.nLastCutLine > 0 ? ProParam相机模切业务.nLastCutLine : CutLine;

                            //这里进行位置推算
                            //第一步，判断位置是否满足裁图条件
                            HOperatorSet.GetImageSize(CropPartParams.Image_part, out HTuple width, out HTuple height);
                            midLineTemp = midLine + CutLine;
                            topLineTemp = midLineTemp - (PieHeight * CropHeightScale) / 2;
                            bottomLineTemp = midLineTemp + (PieHeight * CropHeightScale) / 2;

                            //第二步，判断三个位置是否满足裁图条件，<0,或者大于图像高度都不行,日志输出各自位置值，正常来说不应该出现这种情况，一定是参数存在问题

                            if (topLineTemp < 0 || topLineTemp > height || bottomLineTemp < 0 || bottomLineTemp > height || midLineTemp < 0 || midLineTemp > height)
                            {
                                ImageInfo EAImageInfo = new ImageInfo();
                                EAImageInfo.NgType = 1;
                                EAImageInfo.ho_image = image1.ho_image.CopyObj(1, -1);
                                EAImageInfo.EAID = ProParam相机模切业务.nEAId;
                                EAImageInfo.nFirstLineEncoderCount = image1.nFirstLineEncoderCount;
                                EAImageInfo.nLastLineEncoderCount = image1.nLastLineEncoderCount;
                                EAImageInfo.imageWidth = InfoImgInfo.imageWidth;
                                EAImageInfo.imageHeight = EAImageInfo.nLastLineEncoderCount - EAImageInfo.nFirstLineEncoderCount;
                                EAImageInfo.ImageDescription = $"第{EAImageInfo.EAID}片EA";

                                ProParam相机模切业务.imageEAInfos.Add(EAImageInfo);
                                ProParam相机模切业务.semaphoreSlim.Release();

                                ProParam相机模切业务.nEAId++;

                                ShowHalconWindow2.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = ("裁剪后的图像不满足裁图条件\n" + "裁剪后的图像上边位置值:" + topLineTemp.ToString() + "\n裁剪后的图像下边位置值:" + bottomLineTemp.ToString() + "\n裁剪后的图像中间位置值:" + midLineTemp.ToString()), m_stuGetColor = HalconColor.red });

                                ShowHalconWindow1.m_nImageFrameID = InfoImgInfo.image_ID;
                                ShowHalconWindow1.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前帧号:{InfoImgInfo.image_ID.ToString()}" });

                                ShowHalconWindow2.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前帧号:{InfoImgInfo.image_ID.ToString()}" });
                                ShowHalconWindow2.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前EA号:{ProParam相机模切业务.nEAId - 1}" });
                                ShowHalconWindow2.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前EA号:{ProParam相机模切业务.nEAId - 1}_首行裁断位", m_bIsWindow = false, m_fCol = image1.imageWidth / 2, m_fRow = 1 });
                                ShowHalconWindow2.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前EA号:{ProParam相机模切业务.nEAId - 1}_尾行裁断位", m_bIsWindow = false, m_fCol = image1.imageWidth / 2, m_fRow = image1.imageHeight });

                                ShowHalconWindow2.m_lineShowInfos.Add(new LineInfo() { RowBegin = 1, RowEnd = 1, ColBegin = 0, ColEnd = image1.imageWidth });
                                ShowHalconWindow2.m_lineShowInfos.Add(new LineInfo() { RowBegin = image1.imageHeight, RowEnd = image1.imageHeight, ColBegin = 0, ColEnd = image1.imageWidth });

                                MessageNotify.AddErrorLog("裁剪后的图像不满足裁图条件\n" + "裁剪后的图像上边位置值:" + topLineTemp.ToString() + "\n裁剪后的图像下边位置值:" + bottomLineTemp.ToString() + "\n裁剪后的图像中间位置值:" + midLineTemp.ToString());
                                HOperatorSet.CountSeconds(out Product模切帧图.detectionTime.Image_EndTime);

                                ProParam相机模切业务.ProductCallBack(Product模切帧图);
                                return;
                            }

                            //第三步裁图，用于完整EA使用
                            List<ImageInfo> imageInfos = new List<ImageInfo>();
                            imageInfos.Add(new ImageInfo() { ho_image = CropPartParams.Image_part, image_ID = 1 });
                            IPlugin EACropPlugin = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == $"拼小图模块");

                            ParamsCropPart EACropPartParams = EACropPlugin.PluginParams as ParamsCropPart; //里氏转换
                            EACropPartParams.list_ImageInfo = imageInfos;
                            EACropPartParams.PartSizeRow = (bottomLineTemp - topLineTemp);
                            EACropPartParams.PartSizeCol = width;
                            EACropPartParams.PartCenterID = 1;
                            EACropPartParams.PartCenterRow = midLineTemp;
                            EACropPartParams.PartCenterCol = width / 2;


                            if (EACropPlugin.ExecuteObj())
                            {

                                ImageInfo EAImageInfo = new ImageInfo();
                                EAImageInfo.ho_image = EACropPartParams.Image_part;
                                EAImageInfo.EAID = ProParam相机模切业务.nEAId;
                                EAImageInfo.nFirstLineEncoderCount = image1.nFirstLineEncoderCount + ((int)EACropPartParams.hv_ImagePartBeginRow.D) + (int)((image1.imageHeight + image2.imageHeight) * 0.05 / 2);
                                EAImageInfo.nLastLineEncoderCount = EAImageInfo.nFirstLineEncoderCount + (int)(bottomLineTemp - topLineTemp);
                                EAImageInfo.imageWidth = InfoImgInfo.imageWidth;
                                EAImageInfo.imageHeight = EAImageInfo.nLastLineEncoderCount - EAImageInfo.nFirstLineEncoderCount;
                                EAImageInfo.ImageDescription = $"第{EAImageInfo.EAID}片EA";

                                ProParam相机模切业务.imageEAInfos.Add(EAImageInfo);
                                ProParam相机模切业务.semaphoreSlim.Release();

                            }
                            else
                            {

                                ImageInfo EAImageInfo = new ImageInfo();
                                EAImageInfo.NgType = 1;
                                EAImageInfo.ho_image = image1.ho_image.CopyObj(1, -1);
                                EAImageInfo.EAID = ProParam相机模切业务.nEAId;
                                EAImageInfo.nFirstLineEncoderCount = image1.nFirstLineEncoderCount;
                                EAImageInfo.nLastLineEncoderCount = image1.nLastLineEncoderCount;
                                EAImageInfo.imageWidth = InfoImgInfo.imageWidth;
                                EAImageInfo.imageHeight = EAImageInfo.nLastLineEncoderCount - EAImageInfo.nFirstLineEncoderCount;
                                EAImageInfo.ImageDescription = $"第{EAImageInfo.EAID}片EA";

                                ProParam相机模切业务.imageEAInfos.Add(EAImageInfo);
                                ProParam相机模切业务.semaphoreSlim.Release();

                                ShowHalconWindow2.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = "拼小图模块失败", m_stuGetColor = HalconColor.red });
                                ShowHalconWindow1.m_nImageFrameID = InfoImgInfo.image_ID;
                                ShowHalconWindow1.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前帧号:{InfoImgInfo.image_ID.ToString()}" });
                                ShowHalconWindow2.m_nProductID = ProParam相机模切业务.nEAId;
                                ShowHalconWindow2.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前帧号:{InfoImgInfo.image_ID.ToString()}" });
                                ShowHalconWindow2.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前EA号:{ProParam相机模切业务.nEAId}" });
                                ShowHalconWindow2.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前EA号:{ProParam相机模切业务.nEAId}_首行裁断位", m_bIsWindow = false, m_fCol = image1.imageWidth / 2, m_fRow = 1 });
                                ShowHalconWindow2.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前EA号:{ProParam相机模切业务.nEAId}_尾行裁断位", m_bIsWindow = false, m_fCol = image1.imageWidth / 2, m_fRow = image1.imageHeight });

                                ShowHalconWindow2.m_lineShowInfos.Add(new LineInfo() { RowBegin = 1, RowEnd = 1, ColBegin = 0, ColEnd = image1.imageWidth });
                                ShowHalconWindow2.m_lineShowInfos.Add(new LineInfo() { RowBegin = image1.imageHeight, RowEnd = image1.imageHeight, ColBegin = 0, ColEnd = image1.imageWidth });
                                ProParam相机模切业务.nEAId++;

                                //不应该失败，肯定跟参数设置有关
                                MessageNotify.AddErrorLog("拼小图模块执行失败:" + "不应该失败，推理的肯定跟参数设置有关，检查参数");
                                HOperatorSet.CountSeconds(out Product模切帧图.detectionTime.Image_EndTime);
                                ProParam相机模切业务.ProductCallBack(Product模切帧图);
                                return;

                            }

                            ShowHalconWindow1.m_nImageFrameID = InfoImgInfo.image_ID;
                            ShowHalconWindow1.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前帧号:{InfoImgInfo.image_ID.ToString()}" });
                            ShowHalconWindow2.m_nImageFrameID = InfoImgInfo.image_ID;
                            ShowHalconWindow2.m_nProductID = ProParam相机模切业务.nEAId;

                            ShowHalconWindow2.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前帧号:{InfoImgInfo.image_ID.ToString()}" });
                            ShowHalconWindow2.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前EA号:{ProParam相机模切业务.nEAId}" });
                            ShowHalconWindow2.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前EA号:{ProParam相机模切业务.nEAId}_首行裁断位", m_bIsWindow = false, m_fCol = width / 2, m_fRow = EACropPartParams.hv_ImagePartBeginRow });
                            ShowHalconWindow2.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前EA号:{ProParam相机模切业务.nEAId}_尾行裁断位", m_bIsWindow = false, m_fCol = width / 2, m_fRow = EACropPartParams.hv_ImagePartEndRow });

                            ShowHalconWindow2.m_lineShowInfos.Add(new LineInfo() { RowBegin = EACropPartParams.hv_ImagePartBeginRow, RowEnd = EACropPartParams.hv_ImagePartBeginRow, ColBegin = 0, ColEnd = width });
                            ShowHalconWindow2.m_lineShowInfos.Add(new LineInfo() { RowBegin = EACropPartParams.hv_ImagePartEndRow, RowEnd = EACropPartParams.hv_ImagePartEndRow, ColBegin = 0, ColEnd = width });
                            ProParam相机模切业务.nEAId++;



                        }
                        else
                        {
                            ImageInfo EAImageInfo = new ImageInfo();
                            EAImageInfo.NgType = 1;
                            EAImageInfo.ho_image = image1.ho_image.CopyObj(1, -1);
                            EAImageInfo.EAID = ProParam相机模切业务.nEAId;
                            EAImageInfo.nFirstLineEncoderCount = image1.nFirstLineEncoderCount;
                            EAImageInfo.nLastLineEncoderCount = image1.nLastLineEncoderCount;
                            EAImageInfo.imageWidth = InfoImgInfo.imageWidth;
                            EAImageInfo.imageHeight = EAImageInfo.nLastLineEncoderCount - EAImageInfo.nFirstLineEncoderCount;
                            EAImageInfo.ImageDescription = $"第{EAImageInfo.EAID}片EA";

                            ProParam相机模切业务.imageEAInfos.Add(EAImageInfo);
                            ProParam相机模切业务.semaphoreSlim.Release();



                            ShowHalconWindow2.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = "拼接大图失败", m_stuGetColor = HalconColor.red });
                            ShowHalconWindow1.m_hImage = InfoImgInfo.ho_image;
                            ShowHalconWindow1.m_nImageFrameID = InfoImgInfo.image_ID;
                            ShowHalconWindow1.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前帧号:{InfoImgInfo.image_ID.ToString()}" });
                            ShowHalconWindow2.m_hImage = InfoImgInfo.ho_image.CopyObj(1, -1);
                            ShowHalconWindow2.m_nImageFrameID = InfoImgInfo.image_ID;
                            ShowHalconWindow2.m_nProductID = ProParam相机模切业务.nEAId;

                            ShowHalconWindow2.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前帧号:{InfoImgInfo.image_ID.ToString()}" });
                            ShowHalconWindow2.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前EA号:{ProParam相机模切业务.nEAId}" });
                            ShowHalconWindow2.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前EA号:{ProParam相机模切业务.nEAId}_首行裁断位", m_bIsWindow = false, m_fCol = image1.imageWidth / 2, m_fRow = 1 });
                            ShowHalconWindow2.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前EA号:{ProParam相机模切业务.nEAId}_尾行裁断位", m_bIsWindow = false, m_fCol = image1.imageWidth / 2, m_fRow = image1.imageHeight });
                            ShowHalconWindow2.m_lineShowInfos.Add(new LineInfo() { RowBegin = 1, RowEnd = 1, ColBegin = 0, ColEnd = image1.imageWidth });
                            ShowHalconWindow2.m_lineShowInfos.Add(new LineInfo() { RowBegin = image1.imageHeight, RowEnd = image1.imageHeight, ColBegin = 0, ColEnd = image1.imageWidth });
                            ProParam相机模切业务.nEAId++;
                            MessageNotify.AddErrorLog("裁拼图模块执行失败:" + "检查传入的参数是否正确");
                        }

                    }
                    //这里为开机的第一片，默认用第一片
                    else
                    {

                        InfoImgInfo.EAID = ProParam相机模切业务.nEAId;
                        InfoImgInfo.NgType = 2;
                        InfoImgInfo.ImageDescription = "首片NG";

                        ProParam相机模切业务.imageEAInfos.Add(InfoImgInfo);
                        ProParam相机模切业务.semaphoreSlim.Release();


                        ShowHalconWindow1.m_hImage = InfoImgInfo.ho_image.CopyObj(1, -1);
                        ShowHalconWindow1.m_nImageFrameID = InfoImgInfo.image_ID;

                        ShowHalconWindow1.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前帧号:{InfoImgInfo.image_ID.ToString()}" });
                        ShowHalconWindow2.m_hImage = InfoImgInfo.ho_image.CopyObj(1, -1);
                        ShowHalconWindow2.m_nImageFrameID = InfoImgInfo.image_ID;
                        ShowHalconWindow2.m_nProductID = ProParam相机模切业务.nEAId;
                        ShowHalconWindow2.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前帧号:{InfoImgInfo.image_ID.ToString()}" });
                        ShowHalconWindow2.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前EA号:{ProParam相机模切业务.nEAId}" });

                        ProParam相机模切业务.nEAId++;
                    }

                    HOperatorSet.CountSeconds(out Product模切帧图.detectionTime.Image_EndTime);
                    HTuple end = (Product模切帧图.detectionTime.Image_EndTime - Product模切帧图.detectionTime.Image_StartTime) * 1000;
                    ShowHalconWindow2.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前耗时:{end.D.ToString("0.00")}" });
                    ProParam相机模切业务.ProductCallBack(Product模切帧图);


                }


            }
            catch (Exception ee)
            {
                MessageNotify.AddErrorLog("ProProcessCropBaseCam():" + ee.ToString());
                System.Windows.Forms.MessageBox.Show("ProProcessCropBaseCam():" + ee.ToString(), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //throw;
            }
        }

    

        /// <summary>
        /// 线扫2预处理流程
        /// </summary>
        /// <param name="process"></param>
        public void ProProcess从相机裁图流程(Process process)
        {
            try
            {

                int ProcessID = process.processInfo.ProcessID;
                int camID = 0;
                int CurCamSide = camID;
                //设置当前流程绑定核
                SetThreadAffinityMask(GetCurrentThread(), new UIntPtr(SetCpuID(ProcessID)));
                //标定系数
                double Calipix_y = Service_Camera.GetCam_CalibrationValue_Y((int)CameraEnum.Cam2);
                double Calipix_x = Service_Camera.GetCam_CalibrationValue_X((int)CameraEnum.Cam2);

                double ABMis = (m_craft.Get_GlobalVariableItem((int)CameraEnum.Cam1, "CamABGap")?.Value.ToDouble() ?? 0) / Calipix_y;

                //极片高度，这里接工艺
                double PieHeight = (m_craft.Get_GlobalVariableItem("PieHeight")?.Value.ToDouble() ?? 0) / Calipix_y; ;

                //默认切刀位置
                double CutLine = (m_craft.Get_GlobalVariableItem((int)CameraEnum.Cam2, "DefCutPos")?.Value.ToDouble() ?? 0);

                //居中裁图偏移距离
                double midLine = (m_craft.Get_GlobalVariableItem("CropCenterOffset")?.Value.ToDouble() ?? 0);
                //裁图高度比例系数
                double CropHeightScale = (m_craft.Get_GlobalVariableItem("CropHeightScale")?.Value.ToDouble() ?? 1);


                // 选择参数和队列基于是否模拟
                DieCutParam ProParam从相机模切业务 = Project.Instance.Is_simulation ? m_DicCamOperaParam_Sim[camID + 1] as DieCutParam : m_DicCamOperaParam[camID + 1] as DieCutParam;
                var imageQueue = ProParam从相机模切业务.alreadyDealGrabImage;
                //通过信号量把控线程
                ProParam从相机模切业务.semaphoreSlimCam.Wait(50);

                if (imageQueue.Count > 0) // 检查从相机的图像队列是否有图像
                {

                    ImageInfo InfoImgInfo = imageQueue.Dequeue();
                    DieCutImageProduct Product模切帧图 = new DieCutImageProduct();
                    HOperatorSet.CountSeconds(out Product模切帧图.detectionTime.Image_StartTime);
                    InfoImgInfo.nFirstLineEncoderCount = (long)((InfoImgInfo.image_ID - 1) * InfoImgInfo.imageHeight);
                    InfoImgInfo.nLastLineEncoderCount = (long)(InfoImgInfo.image_ID * InfoImgInfo.imageHeight);

                    //计算当前图片绝对位置
                    ProParam从相机模切业务.nFristAbsPosPixel = (long)((InfoImgInfo.image_ID - 1) * InfoImgInfo.imageHeight);
                    ProParam从相机模切业务.nLastAbsPosPixel = (long)(InfoImgInfo.image_ID * InfoImgInfo.imageHeight);
                    ProParam从相机模切业务.imageInfos.Add(InfoImgInfo);

                    if (ProParam从相机模切业务.imageInfos.Count > 2)
                    {
                        ProParam从相机模切业务.imageInfos.RemoveAt(0);
                    }



                    Product模切帧图.showHalconGroup = new System.Collections.ObjectModel.ObservableCollection<ShowHalconWindow>();
                    Product模切帧图.showHalconGroup.Add(new ShowHalconWindow()
                    {
                        WindowName = "原图",
                        m_nImageFrameID = InfoImgInfo.image_ID,
                       m_hImage= InfoImgInfo.ho_image

                    });
                    Product模切帧图.showHalconGroup.Add(new ShowHalconWindow()
                    {
                        WindowName = "拼接后",
                        m_nImageFrameID = InfoImgInfo.image_ID,
                        m_hImage = InfoImgInfo.ho_image.CopyObj(1, -1)

                    });

                    //用临时变量转接对象
                    ShowHalconWindow ShowHalconWindow1 = Product模切帧图.showHalconGroup[0];
                    ShowHalconWindow ShowHalconWindow2 = Product模切帧图.showHalconGroup[1];

                    //判断是否出问题了
                    if (ProParam从相机模切业务.nLastAbsPosPixel > (ABMis))
                    {
                        if (ProParam从相机模切业务.BufferSplitLines.Count == 0)
                        {
                            MessageNotify.AddErrorLog("已经过了设定距离，该相机为后相机，不应该出现无切刀位情况，只能是参数设置问题");
                            HOperatorSet.CountSeconds(out Product模切帧图.detectionTime.Image_EndTime);
                            
                            ShowHalconWindow2.m_messageShowInfos.Add(new MessageShowInfo()
                            {
                                m_strMessage = $"帧号{InfoImgInfo.image_ID}_已经过了设定距离，该相机为后相机，不应该出现无切刀位情况，只能是参数设置问题",
                                m_stuGetColor = HalconColor.red

                            });
                           
                            ShowHalconWindow1.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前帧号:{InfoImgInfo.image_ID.ToString()}" });
                            ShowHalconWindow2.m_nProductID = InfoImgInfo.image_ID;
                            ProParam从相机模切业务.ProductCallBack(Product模切帧图);
                            return;
                        }


                    }
                    else
                    {
                       
       
                        ShowHalconWindow1.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前帧号:{InfoImgInfo.image_ID.ToString()}" });
                        ShowHalconWindow2.m_nProductID = InfoImgInfo.image_ID;
                        ShowHalconWindow2.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前帧号:{InfoImgInfo.image_ID.ToString()}" });
                        ShowHalconWindow2.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前片未到位置,AB距离{ABMis}_当前帧尾部位置{InfoImgInfo.nLastLineEncoderCount}" });

                        HOperatorSet.CountSeconds(out Product模切帧图.detectionTime.Image_EndTime);
                        ProParam从相机模切业务.ProductCallBack(Product模切帧图);
                        return;
                    }

                    //判断切刀位是否到达
                    if (ProParam从相机模切业务.BufferSplitLines.Count > 0)
                    {
                        int i = 0;
                        //判定是否可进入裁图，直接拿尾行判断
                        for (int a = 0; a < ProParam从相机模切业务.BufferSplitLines.Count; a++)
                        {
                            LineInfo lineInfoBase = ProParam从相机模切业务.BufferSplitLines[a];
                            if (InfoImgInfo.nLastLineEncoderCount > lineInfoBase.RowEnd && InfoImgInfo.nFirstLineEncoderCount < lineInfoBase.RowEnd)
                            {
                             
                                //计算在图中的位置
                                double tempH = InfoImgInfo.nLastLineEncoderCount - lineInfoBase.RowEnd;
                                double bottomLineTemp = InfoImgInfo.imageHeight - tempH;
                                double topLineTemp = bottomLineTemp - PieHeight;
                                double midLineTemp = topLineTemp + PieHeight / 2;

                                IPlugin EACropPlugin = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == $"裁拼图模块");

                                ParamsCropPart EACropPartParams = EACropPlugin.PluginParams as ParamsCropPart; //里氏转换
                                EACropPartParams.list_ImageInfo = ProParam从相机模切业务.imageInfos;
                                EACropPartParams.PartSizeRow = (lineInfoBase.RowEnd - lineInfoBase.RowBegin);
                                EACropPartParams.PartSizeCol = InfoImgInfo.imageWidth;
                                EACropPartParams.PartCenterID = InfoImgInfo.image_ID;
                                EACropPartParams.PartCenterRow = midLineTemp;
                                EACropPartParams.PartCenterCol = InfoImgInfo.imageWidth / 2;

                                if (EACropPlugin.ExecuteObj())
                                {

                                    ImageInfo EAImageInfo = new ImageInfo();
                                    EAImageInfo.NgType = lineInfoBase.NgType;
                                    EAImageInfo.ho_image = EACropPartParams.Image_part;
                                    EAImageInfo.EAID = lineInfoBase.EAID;
                                    EAImageInfo.nFirstLineEncoderCount = (long)lineInfoBase.RowBegin;
                                    EAImageInfo.nLastLineEncoderCount = (long)lineInfoBase.RowEnd;
                                    EAImageInfo.imageWidth = InfoImgInfo.imageWidth;
                                    EAImageInfo.imageHeight = (long)lineInfoBase.RowEnd - (long)lineInfoBase.RowBegin;
                                    EAImageInfo.ImageDescription = $"第{InfoImgInfo.EAID}片EA";
                                    ProParam从相机模切业务.imageEAInfos.Add(EAImageInfo);
                                    ProParam从相机模切业务.semaphoreSlim.Release();
                                }
                                else
                                {
                                    //这里不应该出问题，要检查参数问题
                                    MessageNotify.AddErrorLog($"流程{ProcessID}裁图失败，这里不应改出现问题，请检查参数设置");
                                    HOperatorSet.CountSeconds(out Product模切帧图.detectionTime.Image_EndTime);
                                    ShowHalconWindow1.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前帧号:{InfoImgInfo.image_ID.ToString()}" });
                                    ShowHalconWindow2.m_nProductID = InfoImgInfo.image_ID;
                                    ShowHalconWindow2.m_messageShowInfos.Add(new MessageShowInfo()
                                    {
                                        m_strMessage = $"帧号{InfoImgInfo.image_ID}_流程{ProcessID}裁图失败，这里不应改出现问题，请检查参数设置",
                                        m_stuGetColor = HalconColor.red

                                    });
                                    ProParam从相机模切业务.ProductCallBack(Product模切帧图);
                                    return;
                                }
                                ShowHalconWindow1.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前帧号:{InfoImgInfo.image_ID.ToString()}" });
                                ShowHalconWindow2.m_nProductID = lineInfoBase.EAID;
                                ShowHalconWindow2.m_lineShowInfos.Add(new LineInfo() { ColBegin = 0, ColEnd = InfoImgInfo.imageWidth, RowBegin = EACropPartParams.hv_ImagePartEndRow, RowEnd = EACropPartParams.hv_ImagePartEndRow });
                                ShowHalconWindow2.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前帧号:{InfoImgInfo.image_ID.ToString()}" });
                                ShowHalconWindow2.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前EA号:{lineInfoBase.EAID}" });
                                ShowHalconWindow2.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前EA号:{lineInfoBase.EAID}_尾行裁断位", m_bIsWindow = false, m_fCol = InfoImgInfo.imageWidth / 2, m_fRow = EACropPartParams.hv_ImagePartEndRow });
                                i++;



                            }
                            else
                            {
                                //判断首行在不在当前图片位置，没有直接报警
                                if (!(InfoImgInfo.nLastLineEncoderCount > lineInfoBase.RowBegin && InfoImgInfo.nFirstLineEncoderCount < lineInfoBase.RowBegin))
                                {
                                    //报警
                                    MessageNotify.AddErrorLog($"当前图片位置不在裁断位范围内，该相机为后相机，不应该出现无切刀位情况，只能是参数设置问题\n当前图片起始位:{InfoImgInfo.nFirstLineEncoderCount.ToString()}\n当前图片结束位:{InfoImgInfo.nLastLineEncoderCount.ToString()}\n裁断位起始位:{lineInfoBase.RowBegin.ToString()}\n裁断位结束位:{lineInfoBase.RowEnd.ToString()}");
                                    HOperatorSet.CountSeconds(out Product模切帧图.detectionTime.Image_EndTime);
                                    ShowHalconWindow1.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前帧号:{InfoImgInfo.image_ID.ToString()}" });
                                    ShowHalconWindow2.m_nProductID = InfoImgInfo.image_ID;
                                    ShowHalconWindow2.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前图片位置不在裁断位范围内，该相机为后相机，不应该出现无切刀位情况，只能是参数设置问题\n当前图片起始位:{InfoImgInfo.nFirstLineEncoderCount.ToString()}\n当前图片结束位:{InfoImgInfo.nLastLineEncoderCount.ToString()}\n裁断位起始位:{lineInfoBase.RowBegin.ToString()}\n裁断位结束位:{lineInfoBase.RowEnd.ToString()}", m_stuGetColor = HalconColor.red });
                                    ProParam从相机模切业务.ProductCallBack(Product模切帧图);
                                    return;
                                }
                                else
                                {

                                    ShowHalconWindow1.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前帧号:{InfoImgInfo.image_ID.ToString()}" });
                                    ShowHalconWindow2.m_nProductID = InfoImgInfo.image_ID;
                                    ShowHalconWindow2.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前帧号:{InfoImgInfo.image_ID.ToString()}" });
                                    ShowHalconWindow2.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"第{lineInfoBase.EAID}个EA未到位置,AB距离{ABMis}_当前帧尾部位置{InfoImgInfo.nLastLineEncoderCount}_当前切刀位置{lineInfoBase.RowEnd}" });
                                }

                                break;
                            }

                        }

                        ProParam从相机模切业务.BufferSplitLines.RemoveRange(0, i);



                    }
                    else
                    {

                        ShowHalconWindow1.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前帧号:{InfoImgInfo.image_ID.ToString()}" });
                        ShowHalconWindow2.m_nProductID = InfoImgInfo.image_ID;
                        ShowHalconWindow2.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前帧号:{InfoImgInfo.image_ID.ToString()}" });
                        ShowHalconWindow2.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前片未到位置,AB距离{ABMis}_当前帧尾部位置{InfoImgInfo.nLastLineEncoderCount}" });

                    }

                    HOperatorSet.CountSeconds(out Product模切帧图.detectionTime.Image_EndTime);
                    HTuple end = (Product模切帧图.detectionTime.Image_EndTime - Product模切帧图.detectionTime.Image_StartTime) * 1000;
                    ShowHalconWindow2.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前耗时:{end.D.ToString("0.00")}" });
                    ProParam从相机模切业务.ProductCallBack(Product模切帧图);



                }





            }
            catch (Exception ee)
            {
                MessageNotify.AddErrorLog("ProProcessCropFromCam():" + ee.ToString());
                System.Windows.Forms.MessageBox.Show("ProProcessCropFromCam():" + ee.ToString(), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //throw;
            }

        }

        /// <summary>
        /// 线扫EA流程
        /// </summary>
        /// <param name="process"></param>
        public void ProProcess极片EA流程(Process process)
        {
            int ProcessID = process.processInfo.ProcessID;
            try
            {


                int camID = 0;
                int CurCamSide = camID;
                int CID = ((ProcessID % 2) == 0) ? 0 : 1;

                //设置当前流程绑定核
                SetThreadAffinityMask(GetCurrentThread(), new UIntPtr(SetCpuID(ProcessID)));
                //标定系数
                double Calipix_y = Service_Camera.GetCam_CalibrationValue_Y(CID);
                double Calipix_x = Service_Camera.GetCam_CalibrationValue_X(CID);

                int tabISLeft = m_craft.Get_GlobalVariableItem("极耳在左侧").Value.ToInt();
                bool isAT = m_craft.Get_GlobalVariableItem("带AT").Value.ToBool();

                //承接极性，方便切换
                int PoleMode = (int)m_craft.Electrode;
           

                //极片高度，这里接工艺
                double PieHeight = (m_craft.Get_GlobalVariableItem("PieHeight")?.Value.ToDouble() ?? 0) / Calipix_y;
                double PieWidth = (m_craft.Get_GlobalVariableItem("PieWidth")?.Value.ToDouble() ?? 0) / Calipix_x;
                double ATWidth = (m_craft.Get_GlobalVariableItem("AtWidth")?.Value.ToDouble() ?? 0) / Calipix_x;
                double BrokenWidth = (m_craft.Get_GlobalVariableItem(CID, "BreakAreaW")?.Value.ToDouble() ?? 0) / Calipix_x;

                //这里接左侧抓边的基准，基准要设置好
                PointInfo point基准 = new PointInfo() { Col = (m_craft.Get_GlobalVariableItem(CID, "DefCamEALftBaseCol")?.Value.ToDouble() ?? 0), Row = 0 };


                //可正可负
                //涂布左偏移//mm//pixel 
                double CoatLeftCol = (m_craft.Get_GlobalVariableItem(CID, "CoatLftCol1")?.Value.ToDouble() ?? 0) / Calipix_x;
                //涂布右偏移//mm
                double CoatRightCol = (m_craft.Get_GlobalVariableItem(CID, "CoatRgtCol2")?.Value.ToDouble() ?? 0) / Calipix_x;
                //涂布上偏移//mm
                double CoatTopRow = (m_craft.Get_GlobalVariableItem(CID, "CoatTopRow1")?.Value.ToDouble() ?? 0) / Calipix_y;
                //涂布下偏移//mm
                double CoatBottomRow = (m_craft.Get_GlobalVariableItem(CID, "CoatTopRow2")?.Value.ToDouble() ?? 0) / Calipix_y;

                //可正可负
                //破损左偏移//mm//pixel 
                double BrokenLeftCol = (m_craft.Get_GlobalVariableItem(CID, "BreakLftCol1")?.Value.ToDouble() ?? 0) / Calipix_x;
                //破损右偏移//mm
                double BrokenRightCol = (m_craft.Get_GlobalVariableItem(CID, "BreakRgtCol2")?.Value.ToDouble() ?? 0) / Calipix_x;
                //破损上偏移//mm
                double BrokenTopRow = (m_craft.Get_GlobalVariableItem(CID, "BreakTopRow1")?.Value.ToDouble() ?? 0) / Calipix_y;
                //下偏移//mm
                double BrokenBottomRow = (m_craft.Get_GlobalVariableItem(CID, "BreakBotRow2")?.Value.ToDouble() ?? 0) / Calipix_y;


                //AT左偏移//mm//pixel
                double ATLeftCol = (m_craft.Get_GlobalVariableItem(CID, "AtLftCol1")?.Value.ToDouble() ?? 0) / Calipix_x;
                //AT右偏移//mm
                double ATRightCol = (m_craft.Get_GlobalVariableItem(CID, "AtLftCol2")?.Value.ToDouble() ?? 0) / Calipix_x;
                //AT上偏移//mm
                double ATTopRow = (m_craft.Get_GlobalVariableItem(CID, "AtTopRow1")?.Value.ToDouble() ?? 0) / Calipix_y;
                //AT下偏移//mm
                double ATBottomRow = (m_craft.Get_GlobalVariableItem(CID, "AtBotRow2")?.Value.ToDouble() ?? 0) / Calipix_y;


                // 选择参数和队列基于是否模拟

                DieCutParam ProParm模切业务 = Project.Instance.Is_simulation ? m_DicCamOperaParam_Sim[CID] as DieCutParam : m_DicCamOperaParam[CID] as DieCutParam;
                //通过信号量把控线程
                ProParm模切业务.semaphoreSlim.Wait(30);

                if (ProParm模切业务.imageEAInfos.Count > 0)
                {
                    ImageInfo EAimageInfo = ProParm模切业务.imageEAInfos[0];
                    ProParm模切业务.imageEAInfos.RemoveAt(0);
                    DieCutEAProduct Product极片EA = new DieCutEAProduct();
                    HOperatorSet.CountSeconds(out Product极片EA.detectionTime.Image_StartTime);
                    Product极片EA.showHalconGroup = new System.Collections.ObjectModel.ObservableCollection<ShowHalconWindow>();
                    Product极片EA.showHalconGroup.Add(new ShowHalconWindow()
                    {
                        WindowName = "原图",
                        m_hImage = EAimageInfo.ho_image,
                        m_nImageFrameID = EAimageInfo.image_ID,
                        m_nProductID = EAimageInfo.EAID,

                    });

                    //用临时变量转接对象
                    ShowHalconWindow ShowHalconWindow = Product极片EA.showHalconGroup[0];

                    //不进入检测，默认NG
                    if(EAimageInfo.NgType>0)
                    {
                        HOperatorSet.GenEmptyObj(out HObject region);
                        HOperatorSet.CountSeconds(out Product极片EA.detectionTime.Image_EndTime);
                        HTuple end2 = (Product极片EA.detectionTime.Image_EndTime - Product极片EA.detectionTime.Image_StartTime) * 1000;
                        ShowHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前耗时:{end2.D.ToString("0.00")}" });
                       
                        NgDefectResult ngDefect = new NgDefectResult();
                        ngDefect.ClassName = "涂布不良";
                        ngDefect.Area =999999;
                        ngDefect.HeightPx = 999999;
                        ngDefect.WidthPx = 999999;
                        ngDefect.Height = 999999;
                        ngDefect.Width = 999999;
                        ngDefect.PixelToMmH = Calipix_x;
                        ngDefect.PixelToMmV = Calipix_y;
                        ngDefect.Region = region;
                        ngDefect.CropImage = EAimageInfo.ho_image;
                        Product极片EA.ngDefectResults.Add(ngDefect);
                        ProParm模切业务.ProductCallBackCraft(Product极片EA);
                        return;

                    }




                    #region 左侧涂层抓边定位
                    IPlugin Plugin左侧涂层抓边定位 = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == $"左侧涂层抓边定位");
                    ParamsMeasureLine params左侧涂层抓边定位 = Plugin左侧涂层抓边定位.PluginParams as ParamsMeasureLine;
                    params左侧涂层抓边定位.ho_Image = EAimageInfo.ho_image;
                    LineInfo Line左侧涂层抓边定位 = new LineInfo() { StrLineName = "左侧涂层抓边定位", strMoudleName = "左侧涂层抓边定位", nProcessID = ProcessID };
                    //抓边结果总结
                    DetectEdgeValueAssign(params左侧涂层抓边定位, Plugin左侧涂层抓边定位.ExecuteObj(), Line左侧涂层抓边定位, ShowHalconWindow.m_messageShowInfos,
                               ShowHalconWindow.m_lineShowInfos, ShowHalconWindow.m_regionShowInfos);
                    Product极片EA.LineInfosResult.Add(Line左侧涂层抓边定位);
                    #endregion

                    #region 右侧涂层抓边定位 
                    IPlugin Plugin右侧涂层抓边定位 = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == $"右侧涂层抓边定位");
                    ParamsMeasureLine params右侧涂层抓边定位 = Plugin右侧涂层抓边定位.PluginParams as ParamsMeasureLine;
                    params右侧涂层抓边定位.ho_Image = EAimageInfo.ho_image;
                    params右侧涂层抓边定位.hv_RoiMidColumn = (Line左侧涂层抓边定位.ColBegin + Line左侧涂层抓边定位.ColEnd) / 2 + PieWidth;
                    //params右侧涂层抓边定位.hv_RoiMidRow = (Line左侧涂层抓边定位.RowBegin + Line左侧涂层抓边定位.RowEnd) / 2;
                    LineInfo Line右侧涂层抓边定位 = new LineInfo() { StrLineName = "右侧涂层抓边定位", strMoudleName = "右侧涂层抓边定位", nProcessID = ProcessID };
                    DetectEdgeValueAssign(params右侧涂层抓边定位, Plugin右侧涂层抓边定位.ExecuteObj(), Line右侧涂层抓边定位, ShowHalconWindow);
                    Product极片EA.LineInfosResult.Add(Line右侧涂层抓边定位);
                    #endregion


                    LineInfo LineAT抓边定位 = new LineInfo() { StrLineName = "AT抓边定位", strMoudleName = "AT抓边定位", nProcessID = ProcessID };
                    #region AT抓边定位
                    if (isAT)
                    {
                        //增加AT抓边
                        IPlugin PluginAT抓边定位 = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == $"AT抓边定位");
                        ParamsMeasureLine paramsAT抓边定位 = PluginAT抓边定位.PluginParams as ParamsMeasureLine;
                        paramsAT抓边定位.ho_Image = EAimageInfo.ho_image;
                        paramsAT抓边定位.hv_RoiMidColumn = (Line左侧涂层抓边定位.ColBegin + Line左侧涂层抓边定位.ColEnd) / 2 + PieWidth + ATWidth; ;
                        //paramsAT抓边定位.hv_RoiMidRow = (Line左侧涂层抓边定位.RowBegin + Line左侧涂层抓边定位.RowEnd) / 2;
                        LineAT抓边定位 = new LineInfo() { StrLineName = "AT抓边定位", strMoudleName = "AT抓边定位", nProcessID = ProcessID };
                        DetectEdgeValueAssign(paramsAT抓边定位, PluginAT抓边定位.ExecuteObj(), LineAT抓边定位, ShowHalconWindow);
                        Product极片EA.LineInfosResult.Add(LineAT抓边定位);
                    }

                    #endregion
                    //抓边定位


                    ////检测区域信息

                    #region 涂布缺陷检测
                    //检测涂层//插件需设置好涂布范围//位置跟随拿抓边定位的
                    IPlugin Plugin涂布缺陷检测 = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == $"涂布检测");

                    ParamsDefectCoat params涂布缺陷检测 = Plugin涂布缺陷检测.PluginParams as ParamsDefectCoat;
                    //offsetX的意思是默认涂层不跟随
                    double offsetX = (Line左侧涂层抓边定位.ColBegin + Line左侧涂层抓边定位.ColEnd) / 2 - point基准.Col;
                    params涂布缺陷检测.ho_Image = EAimageInfo.ho_image;
                    params涂布缺陷检测.LocalRoi.Col1 = ((Line左侧涂层抓边定位.ColBegin + Line左侧涂层抓边定位.ColEnd) / 2) - offsetX;
                    params涂布缺陷检测.LocalRoi.Col2 = ((Line左侧涂层抓边定位.ColBegin + Line左侧涂层抓边定位.ColEnd) / 2) - offsetX + PieWidth;
                    params涂布缺陷检测.LocalRoi.Row1 = 1;
                    params涂布缺陷检测.LocalRoi.Row2 = EAimageInfo.imageHeight - 1;
                    params涂布缺陷检测.LocalRoi.TopOffsetRow = CoatTopRow;
                    params涂布缺陷检测.LocalRoi.BottomOffsetRow = CoatBottomRow;
                    params涂布缺陷检测.LocalRoi.LeftOffsetCol = CoatLeftCol;
                    params涂布缺陷检测.LocalRoi.RightOffsetCol = CoatRightCol;
                    params涂布缺陷检测.PixelToMmH = Calipix_x;
                    params涂布缺陷检测.PixelToMmV = Calipix_y;

                    //位置跟随
                    params涂布缺陷检测.BaseCol = point基准.Col;
                    params涂布缺陷检测.BaseRow = 0;
                    params涂布缺陷检测.BaseAngle = 0;
                    params涂布缺陷检测.CurrentCol = (Line左侧涂层抓边定位.ColBegin + Line左侧涂层抓边定位.ColEnd) / 2;
                    params涂布缺陷检测.CurrentRow = 0;
                    params涂布缺陷检测.CurrentAngle = 0;

                    params涂布缺陷检测.EnablePositionFollow = true;

                    if (Plugin涂布缺陷检测.ExecuteObj())
                    {
                        ShowHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = params涂布缺陷检测.ho_DetectRegion.CopyObj(1, -1), m_stuGetColor = HalconColor.orange });
                        if (params涂布缺陷检测.MaskRois.Count > 0 && params涂布缺陷检测.ho_MaskRoiRegion != null)
                        {
                            ShowHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = params涂布缺陷检测.ho_MaskRoiRegion.CopyObj(1, -1), m_stuGetColor = HalconColor.orange });
                        }


                        if (params涂布缺陷检测.NgResults.Count > 0)
                        {
                            Product极片EA.ngDefectResults = params涂布缺陷检测.NgResults;
                            params涂布缺陷检测.NgResults = new System.Collections.ObjectModel.ObservableCollection<DefectRuleClassifier.Core.Models.NgDefectResult>();

                        }


                    }
                    else
                    {
                        //涂布检测模块出问题，需排查原因，不能开机，停机处理
                        MessageNotify.AddStopAlarm(new StopAlarmMessage() { StopAlarmState = StopAlarmStateEnum.停机, name = "涂布检测模块出问题", describe = "涂布检测模块出问题，需排查原因，不能开机，停机处理" });

                        HOperatorSet.CountSeconds(out Product极片EA.detectionTime.Image_EndTime);
                        ProParm模切业务.ProductCallBackCraft(Product极片EA);

                        return;
                    }
                    #endregion

                    #region 边缘破损检测模块

                    //检测边缘破损//位置跟随一定要启用//用的就是左侧抓边跟随定位//检测区域用offsetX默认不跟随

                    LineInfo Line破损基准 = isAT? LineAT抓边定位 : Line右侧涂层抓边定位;

                    IPlugin Plugin边缘破损检测模块 = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == $"边缘破损检测模块");
                    ParamsBlobAnalysis params边缘破损检测模块 = Plugin边缘破损检测模块.PluginParams as ParamsBlobAnalysis;
                    params边缘破损检测模块.ho_Image = EAimageInfo.ho_image;
                    params边缘破损检测模块.m_fLocalRoiCol2 = (Line破损基准.ColBegin + Line破损基准.ColEnd) / 2 - offsetX;
                    params边缘破损检测模块.m_fLocalRoiCol1 = params边缘破损检测模块.m_fLocalRoiCol2 - BrokenWidth;
                    params边缘破损检测模块.m_fLocalRoiRow1 = 1;
                    params边缘破损检测模块.m_fLocalRoiRow2 = EAimageInfo.imageHeight - 1;

                    params边缘破损检测模块.TopOffsetRow = BrokenTopRow;
                    params边缘破损检测模块.BottomOffsetRow = BrokenBottomRow;
                    params边缘破损检测模块.LeftOffsetCol = BrokenLeftCol;
                    params边缘破损检测模块.RightOffsetCol = BrokenRightCol;

                    params边缘破损检测模块.m_fBaseCol = point基准.Col;
                    params边缘破损检测模块.m_fBaseRow = 0;
                    params边缘破损检测模块.m_fBaseAngle = 0;
                    params边缘破损检测模块.m_fCurrentCol = (Line左侧涂层抓边定位.ColBegin + Line左侧涂层抓边定位.ColEnd) / 2;
                    params边缘破损检测模块.m_fCurrentRow = 0;
                    params边缘破损检测模块.m_fCurrentAngle = 0;
                    params边缘破损检测模块.m_bUsePositionFollow = true;

                    if(params边缘破损检测模块.m_bIsEnable)
                    {
                        if (Plugin边缘破损检测模块.ExecuteObj())
                        {
                            ShowHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = params边缘破损检测模块.m_ho_OutLocalRoiRegion.CopyObj(1, -1), m_stuGetColor = HalconColor.orange });
                            if (params边缘破损检测模块.m_lMaskRoiParams.Count > 0 && params边缘破损检测模块.m_ho_OutMaskRoiRegion != null)
                            {
                                ShowHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = params边缘破损检测模块.m_ho_OutMaskRoiRegion.CopyObj(1, -1), m_stuGetColor = HalconColor.orange });
                            }

                            foreach (var item in params边缘破损检测模块.m_lOutRegionResults)
                            {
                                try
                                {
                                    //paramsBlobAnalysis.m_lOutRegionResults[item.m_nIndex].m_fCenterRow = (lineInfo.RowBegin + lineInfo.RowEnd) / 2;
                                    HOperatorSet.SelectObj(params边缘破损检测模块.m_ho_OutSelectedRegion, out HObject temp, item.m_nIndex);
                                    HOperatorSet.SmallestRectangle1(temp, out HTuple row1, out HTuple col1, out HTuple row2, out HTuple col2);
                                    HOperatorSet.CropPart(EAimageInfo.ho_image, out HObject imagePart, item.m_fCenterRow, item.m_fCenterCol, (col1 - col2).TupleAbs() > 68 ? (col1 - col2).TupleAbs() : (HTuple)68, (row1 - row2).TupleAbs() * 1.1 > 68 ? (row1 - row2).TupleAbs() * 1.1 : (HTuple)68);
                                    NgDefectResult ngDefect = new NgDefectResult();
                                    ngDefect.ClassName = "边缘破损";
                                    ngDefect.Area = item.m_fArea * Calipix_x * Calipix_y;
                                    ngDefect.HeightPx = (row1 - row2).TupleAbs();
                                    ngDefect.WidthPx = (col1 - col2).TupleAbs();
                                    ngDefect.Height = (row1 - row2).TupleAbs() * Calipix_y;
                                    ngDefect.Width = (col1 - col2).TupleAbs() * Calipix_x;
                                    ngDefect.PixelToMmH = Calipix_x;
                                    ngDefect.PixelToMmV = Calipix_y;
                                    ngDefect.Region = temp;
                                    ngDefect.CropImage = imagePart;
                                    Product极片EA.ngDefectResults.Add(ngDefect);
                                }
                                catch (Exception)
                                {
                                    //边缘破损模块输出问题，需排查原因，不能开机，停机处理
                                    MessageNotify.AddStopAlarm(new StopAlarmMessage() { StopAlarmState = StopAlarmStateEnum.停机, name = "边缘破损模块输出问题", describe = "边缘破损模块输出问题，需排查原因，不能开机，停机处理" });

                                    HOperatorSet.CountSeconds(out Product极片EA.detectionTime.Image_EndTime);
                                    ProParm模切业务.ProductCallBackCraft(Product极片EA);

                                    return;
                                    //throw;
                                }


                            }
                        }
                        else
                        {
                            //边缘破损检测模块出问题，需排查原因，不能开机，停机处理
                            MessageNotify.AddStopAlarm(new StopAlarmMessage() { StopAlarmState = StopAlarmStateEnum.停机, name = "边缘破损检测模块", describe = "边缘破损检测模块，需排查原因，不能开机，停机处理" });

                            HOperatorSet.CountSeconds(out Product极片EA.detectionTime.Image_EndTime);
                            ProParm模切业务.ProductCallBackCraft(Product极片EA);

                            return;
                        }
                    }

                  
                    #endregion

                    #region AT漏金属检测模块
                    if (isAT)
                    {
                        //检测边缘破损//位置跟随一定要启用//用的就是左侧抓边跟随定位//检测层不跟随
                        IPlugin PluginAT漏金属检测模块 = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == $"AT漏金属检测模块");
                        ParamsBlobAnalysis paramsAT漏金属检测模块 = PluginAT漏金属检测模块.PluginParams as ParamsBlobAnalysis;
                        paramsAT漏金属检测模块.ho_Image = EAimageInfo.ho_image;

                        paramsAT漏金属检测模块.m_fLocalRoiRow1 = 1;
                        paramsAT漏金属检测模块.m_fLocalRoiCol1 = (Line右侧涂层抓边定位.ColBegin + Line右侧涂层抓边定位.ColEnd) / 2 - offsetX;
                        paramsAT漏金属检测模块.m_fLocalRoiRow2 = EAimageInfo.imageHeight - 1;
                        paramsAT漏金属检测模块.m_fLocalRoiCol2 = (LineAT抓边定位.ColBegin + LineAT抓边定位.ColEnd) / 2 - offsetX;

                        paramsAT漏金属检测模块.TopOffsetRow = ATTopRow;
                        paramsAT漏金属检测模块.BottomOffsetRow = ATBottomRow;
                        paramsAT漏金属检测模块.LeftOffsetCol = ATLeftCol;
                        paramsAT漏金属检测模块.RightOffsetCol = ATRightCol;

                        paramsAT漏金属检测模块.m_fBaseCol = point基准.Col;
                        paramsAT漏金属检测模块.m_fBaseRow = 0;
                        paramsAT漏金属检测模块.m_fBaseAngle = 0;

                        paramsAT漏金属检测模块.m_fCurrentCol = (Line左侧涂层抓边定位.ColBegin + Line左侧涂层抓边定位.ColEnd) / 2;
                        paramsAT漏金属检测模块.m_fCurrentRow = 0;
                        paramsAT漏金属检测模块.m_fCurrentAngle = 0;
                        paramsAT漏金属检测模块.m_bUsePositionFollow = true;


                        if(paramsAT漏金属检测模块.m_bIsEnable)
                        {
                            if (PluginAT漏金属检测模块.ExecuteObj())
                            {
                                ShowHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = paramsAT漏金属检测模块.m_ho_OutLocalRoiRegion.CopyObj(1, -1), m_stuGetColor = HalconColor.orange });
                                if (paramsAT漏金属检测模块.m_lMaskRoiParams.Count > 0 && paramsAT漏金属检测模块.m_ho_OutMaskRoiRegion != null)
                                {
                                    ShowHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = paramsAT漏金属检测模块.m_ho_OutMaskRoiRegion.CopyObj(1, -1), m_stuGetColor = HalconColor.orange });
                                }
                                foreach (var item in paramsAT漏金属检测模块.m_lOutRegionResults)
                                {
                                    try
                                    {
                                        // paramsBlobMetal.m_lOutRegionResults[item.m_nIndex].m_fCenterRow = (lineInfo.RowBegin + lineInfo.RowEnd) / 2;
                                        HOperatorSet.SelectObj(paramsAT漏金属检测模块.m_ho_OutSelectedRegion, out HObject temp, item.m_nIndex);
                                        HOperatorSet.SmallestRectangle1(temp, out HTuple row1, out HTuple col1, out HTuple row2, out HTuple col2);
                                        HOperatorSet.CropPart(EAimageInfo.ho_image, out HObject imagePart, item.m_fCenterRow, item.m_fCenterCol, (col1 - col2).TupleAbs() > 68 ? (col1 - col2).TupleAbs() : (HTuple)68, (row1 - row2).TupleAbs() * 1.1 > 68 ? (row1 - row2).TupleAbs() * 1.1 : (HTuple)68);
                                        NgDefectResult ngDefect = new NgDefectResult();
                                        ngDefect.ClassName = "AT漏金属";
                                        ngDefect.Area = item.m_fArea * Calipix_x * Calipix_y;
                                        ngDefect.HeightPx = (row1 - row2).TupleAbs();
                                        ngDefect.WidthPx = (col1 - col2).TupleAbs();
                                        ngDefect.Height = (row1 - row2).TupleAbs() * Calipix_y;
                                        ngDefect.Width = (col1 - col2).TupleAbs() * Calipix_x;
                                        ngDefect.PixelToMmH = Calipix_x;
                                        ngDefect.PixelToMmV = Calipix_y;
                                        ngDefect.Region = temp;
                                        ngDefect.CropImage = imagePart;
                                        Product极片EA.ngDefectResults.Add(ngDefect);
                                    }
                                    catch (Exception)
                                    {
                                        //边缘破损模块输出问题，需排查原因，不能开机，停机处理
                                        MessageNotify.AddStopAlarm(new StopAlarmMessage() { StopAlarmState = StopAlarmStateEnum.停机, name = "AT漏金属检测模块", describe = "AT漏金属检测模块输出问题，需排查原因，不能开机，停机处理" });

                                        HOperatorSet.CountSeconds(out Product极片EA.detectionTime.Image_EndTime);
                                        ProParm模切业务.ProductCallBackCraft(Product极片EA);

                                        return;
                                        //throw;
                                    }


                                }
                            }
                            else
                            {
                                //涂布检测模块出问题，需排查原因，不能开机，停机处理
                                MessageNotify.AddStopAlarm(new StopAlarmMessage() { StopAlarmState = StopAlarmStateEnum.停机, name = "AT漏金属检测模块", describe = "AT漏金属检测模块出问题，需排查原因，不能开机，停机处理" });

                                HOperatorSet.CountSeconds(out Product极片EA.detectionTime.Image_EndTime);
                                ProParm模切业务.ProductCallBackCraft(Product极片EA);

                                return;
                            }
                        }

                      



                    }
                    #endregion


                    HOperatorSet.CountSeconds(out Product极片EA.detectionTime.Image_EndTime);
                    HTuple end = (Product极片EA.detectionTime.Image_EndTime - Product极片EA.detectionTime.Image_StartTime) * 1000;
                    ShowHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前耗时:{end.D.ToString("0.00")}" });

                    ProParm模切业务.ProductCallBackCraft(Product极片EA);


                }

            }
            catch (Exception ee)
            {
                MessageNotify.AddErrorLog($"流程{ProcessID}ProProcessEACam():" + ee.ToString());
                System.Windows.Forms.MessageBox.Show("ProProcessEACam():" + ee.ToString(), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // throw;
            }
        }

        /// <summary>
        /// 尺寸相机极耳侧检测
        /// </summary>
        /// <param name="process"></param>
        public void ProProcess极片尺寸带极耳检测流程(Process process)
        {


            ///底部是第8个相机吧
            int ProcessID = process.processInfo.ProcessID;
            int camID = process.processInfo.CameraID;
            int CurCamSide = camID;
            int PoleMode = 0;
            int tabISLeft = m_craft.Get_GlobalVariableItem("极耳在左侧").Value.ToInt();
            bool isAT = m_craft.Get_GlobalVariableItem("带AT").Value.ToBool();
            //HTuple end=0;
            try
            {

                //设置当前流程绑定核
                SetThreadAffinityMask(GetCurrentThread(), new UIntPtr(SetCpuID(ProcessID)));
                //标定系数
                double Calipix_y = Service_Camera.GetCam_CalibrationValue_Y(camID);
                double Calipix_x = Service_Camera.GetCam_CalibrationValue_X(camID);
                //工艺粗定位基准
                //初基准点/预留承接工艺

                PointInfo point基准 = new PointInfo()
                {
                    Col = (m_craft.Get_GlobalVariableItem(camID, "CamBaseCol")?.Value.ToDouble() ?? 0),
                    Row = (m_craft.Get_GlobalVariableItem(camID, "CamBaseRow")?.Value.ToDouble() ?? 0)
                };


                SizeDetectParam ProParam尺寸业务 = Project.Instance.Is_simulation ? m_DicCamOperaParam_Sim[camID] as SizeDetectParam : m_DicCamOperaParam[camID] as SizeDetectParam;
                var imageQueue = ProParam尺寸业务.alreadyDealGrabImage;
                ProParam尺寸业务.semaphoreSlimCam.Wait(45);
                if (imageQueue.Count > 0)
                {


                    ImageInfo InfoImgInfo = imageQueue.Dequeue();

                    SizeDectectProduct Product尺寸 = new SizeDectectProduct();

                    Product尺寸.showHalconGroup = new System.Collections.ObjectModel.ObservableCollection<ShowHalconWindow>();
                    Product尺寸.showHalconGroup.Add(new ShowHalconWindow()
                    {
                        WindowName = "原图",
                        m_nImageFrameID = InfoImgInfo.image_ID,
                        m_hImage = InfoImgInfo.ho_image,

                    });
                    ShowHalconWindow ShowHalconWindow = Product尺寸.showHalconGroup[0];
                    HOperatorSet.CountSeconds(out Product尺寸.detectionTime.Image_StartTime);
                    #region 交点模块执行
                    //初定位//当找不到基准点的时候使用位置跟随的基准点//模块需要提前设置好位置跟随的默认基准，但不要启用位置跟随//默认line2为水平方向
                    IPlugin PluginIntersection = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == $"粗定位工具");

                    bool success = ProcessCoarseLocation(InfoImgInfo, PluginIntersection, ProcessID, Product尺寸.CrossPointInfos, ShowHalconWindow, out OutCrossGroup Cross粗定位工具PointInfo, "粗定位工具");
                    if (!success)
                    {
                        Log4netRecord.ErrorLog.Debug($"流程{PluginIntersection.moduleInfo.ProcessID}的{PluginIntersection.moduleInfo.ModuleName}执行失败");
                    }


                    //极耳左侧交点//默认line2为水平方向

                    PluginIntersection = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == $"极耳左侧交点");
                    success = ProcessCoarseLocation(InfoImgInfo, PluginIntersection, ProcessID, Product尺寸.CrossPointInfos, ShowHalconWindow, out OutCrossGroup Cross极耳左侧交点Info, point基准, Cross粗定位工具PointInfo.point, "极耳左侧交点", true, true);
                    if (!success)
                    {
                        Log4netRecord.ErrorLog.Debug($"流程{PluginIntersection.moduleInfo.ProcessID}的{PluginIntersection.moduleInfo.ModuleName}执行失败");
                    }
                    #endregion

                    #region 抓边模块执行
                    IPlugin PluginMeasureLine = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == $"极片右侧抓边");
                    ParamsMeasureLine paramsMeasureLine = PluginMeasureLine.PluginParams as ParamsMeasureLine;
                    paramsMeasureLine.ho_Image = InfoImgInfo.ho_image;
                    paramsMeasureLine.m_bUsePositionFollow = true;
                    paramsMeasureLine.m_fBaseCol = point基准.Col;
                    paramsMeasureLine.m_fBaseRow = point基准.Row;
                    paramsMeasureLine.m_fBaseAngle = 0;
                    paramsMeasureLine.m_fCurrentCol = Cross粗定位工具PointInfo.point.Col;
                    paramsMeasureLine.m_fCurrentRow = Cross粗定位工具PointInfo.point.Row;
                    paramsMeasureLine.m_fCurrentAngle = 0;

                    LineInfo Line极片右侧抓边Info = new LineInfo() { StrLineName = "极片右侧抓边", strMoudleName = "极片右侧抓边", nProcessID = ProcessID };

                    //抓边结果总结
                    DetectEdgeValueAssign(paramsMeasureLine, PluginMeasureLine.ExecuteObj(), Line极片右侧抓边Info, ShowHalconWindow.m_messageShowInfos,
                               ShowHalconWindow.m_lineShowInfos, ShowHalconWindow.m_regionShowInfos);

                    Product尺寸.LineInfosResult.Add(Line极片右侧抓边Info);


                    PluginMeasureLine = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == $"极耳右侧抓边");
                    paramsMeasureLine = PluginMeasureLine.PluginParams as ParamsMeasureLine;
                    paramsMeasureLine.ho_Image = InfoImgInfo.ho_image;
                    paramsMeasureLine.m_bUsePositionFollow = true;
                    paramsMeasureLine.m_fBaseCol = point基准.Col;
                    paramsMeasureLine.m_fBaseRow = point基准.Row;
                    paramsMeasureLine.m_fBaseAngle = 0;
                    paramsMeasureLine.m_fCurrentCol = Cross粗定位工具PointInfo.point.Col;
                    paramsMeasureLine.m_fCurrentRow = Cross粗定位工具PointInfo.point.Row;
                    paramsMeasureLine.m_fCurrentAngle = 0;

                    LineInfo Line极耳右侧抓边Info = new LineInfo() { StrLineName = "极耳右侧抓边", strMoudleName = "极耳右侧抓边", nProcessID = ProcessID };

                    //抓边结果总结
                    DetectEdgeValueAssign(paramsMeasureLine, PluginMeasureLine.ExecuteObj(), Line极耳右侧抓边Info, ShowHalconWindow.m_messageShowInfos,
                               ShowHalconWindow.m_lineShowInfos, ShowHalconWindow.m_regionShowInfos);
                    Product尺寸.LineInfosResult.Add(Line极耳右侧抓边Info);
                    LineInfo Line极片AT抓边Info;
                    if (isAT||true)
                    {
                        PluginMeasureLine = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == $"极片AT抓边");
                        paramsMeasureLine = PluginMeasureLine.PluginParams as ParamsMeasureLine;
                        paramsMeasureLine.ho_Image = InfoImgInfo.ho_image;
                        paramsMeasureLine.m_bUsePositionFollow = true;
                        paramsMeasureLine.m_fBaseCol = point基准.Col;
                        paramsMeasureLine.m_fBaseRow = point基准.Row;
                        paramsMeasureLine.m_fBaseAngle = 0;
                        paramsMeasureLine.m_fCurrentCol = Cross粗定位工具PointInfo.point.Col;
                        paramsMeasureLine.m_fCurrentRow = Cross粗定位工具PointInfo.point.Row;
                        paramsMeasureLine.m_fCurrentAngle = 0;

                        Line极片AT抓边Info = new LineInfo() { StrLineName = "极片AT抓边", strMoudleName = "极片AT抓边", nProcessID = ProcessID };

                        //抓边结果总结
                        DetectEdgeValueAssign(paramsMeasureLine, PluginMeasureLine.ExecuteObj(), Line极片AT抓边Info, ShowHalconWindow.m_messageShowInfos,
                                   ShowHalconWindow.m_lineShowInfos, ShowHalconWindow.m_regionShowInfos);
                        Product尺寸.LineInfosResult.Add(Line极片AT抓边Info);
                    }
                    #endregion

                    #region 组合交点模块行

                    #region 极耳右侧组合交点
                    IPlugin Plugin极耳右侧组合交点 = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == $"极耳右侧组合交点");
                    ParamsCombPoint params极耳右侧组合交点 = Plugin极耳右侧组合交点.PluginParams as ParamsCombPoint;
                    params极耳右侧组合交点.ho_Image = InfoImgInfo.ho_image;
                    params极耳右侧组合交点.LineInfo1 = Line极耳右侧抓边Info.Copy();
                    params极耳右侧组合交点.LineInfo2 = Cross极耳左侧交点Info.LineInfo2.Copy();

                    PointInfo point极耳右侧组合交点 = new PointInfo();
                    if (Plugin极耳右侧组合交点.ExecuteObj())
                    {
                        point极耳右侧组合交点.Col = params极耳右侧组合交点.hv_IntersectionColumn;
                        point极耳右侧组合交点.Row = params极耳右侧组合交点.hv_IntersectionRow;
                        point极耳右侧组合交点.ModuleName = "极耳右侧组合交点";
                        point极耳右侧组合交点.nProcessID = ProcessID;
                        point极耳右侧组合交点.bIsSuccess = true;


                    }
                    else
                    {
                        Log4netRecord.ErrorLog.Debug($"流程{Plugin极耳右侧组合交点.moduleInfo.ProcessID}&&{Plugin极耳右侧组合交点.moduleInfo.ModuleName}---{params极耳右侧组合交点.m_strRunMessage}");
                    }

                    Product尺寸.PointResults.Add(point极耳右侧组合交点.Copy());
                    #endregion

                    #region 极片右侧组合交点
                    IPlugin 极片右侧组合交点 = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == $"极片右侧组合交点");
                    ParamsCombPoint params极片右侧组合交点 = 极片右侧组合交点.PluginParams as ParamsCombPoint;
                    params极片右侧组合交点.ho_Image = InfoImgInfo.ho_image;
                    params极片右侧组合交点.LineInfo1 = Line极片右侧抓边Info.Copy();
                    params极片右侧组合交点.LineInfo2 = Cross粗定位工具PointInfo.LineInfo2.Copy();

                    PointInfo point极片右侧组合交点 = new PointInfo();
                    if (极片右侧组合交点.ExecuteObj())
                    {
                        point极片右侧组合交点.Col = params极片右侧组合交点.hv_IntersectionColumn;
                        point极片右侧组合交点.Row = params极片右侧组合交点.hv_IntersectionRow;
                        point极片右侧组合交点.ModuleName = "极片右侧组合交点";
                        point极片右侧组合交点.nProcessID = ProcessID;
                        point极片右侧组合交点.bIsSuccess = true;

                    }
                    else
                    {
                        Log4netRecord.ErrorLog.Debug($"流程{极片右侧组合交点.moduleInfo.ProcessID}&&{极片右侧组合交点.moduleInfo.ModuleName}---{params极片右侧组合交点.m_strRunMessage}");
                    }

                    Product尺寸.PointResults.Add(point极片右侧组合交点.Copy());
                    #endregion

                    #region 极耳左根部组合交点
                    IPlugin Plugin极耳左根部组合交点 = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == $"极耳左根部组合交点");
                    ParamsCombPoint params极耳左根部组合交点 = Plugin极耳左根部组合交点.PluginParams as ParamsCombPoint;
                    params极耳左根部组合交点.ho_Image = InfoImgInfo.ho_image;
                    params极耳左根部组合交点.LineInfo1 = Cross粗定位工具PointInfo.LineInfo2.Copy();
                    params极耳左根部组合交点.LineInfo2 = Cross极耳左侧交点Info.LineInfo1.Copy();

                    PointInfo point极耳左根部组合交点 = new PointInfo();
                    if (Plugin极耳左根部组合交点.ExecuteObj())
                    {
                        point极耳左根部组合交点.Col = params极耳左根部组合交点.hv_IntersectionColumn;
                        point极耳左根部组合交点.Row = params极耳左根部组合交点.hv_IntersectionRow;
                        point极耳左根部组合交点.ModuleName = "极耳左根部组合交点";
                        point极耳左根部组合交点.nProcessID = ProcessID;
                        point极耳左根部组合交点.bIsSuccess = true;


                    }
                    else
                    {
                        Log4netRecord.ErrorLog.Debug($"流程{Plugin极耳左根部组合交点.moduleInfo.ProcessID}&&{Plugin极耳左根部组合交点.moduleInfo.ModuleName}---{params极耳左根部组合交点.m_strRunMessage}");
                    }

                    Product尺寸.PointResults.Add(point极耳左根部组合交点);
                    #endregion

                    #region 极耳右根部组合交点
                    IPlugin Plugin极耳右根部组合交点 = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == $"极耳右根部组合交点");
                    ParamsCombPoint params极耳右根部组合交点 = Plugin极耳右根部组合交点.PluginParams as ParamsCombPoint;
                    params极耳右根部组合交点.ho_Image = InfoImgInfo.ho_image;
                    params极耳右根部组合交点.LineInfo1 = Cross粗定位工具PointInfo.LineInfo2.Copy();
                    params极耳右根部组合交点.LineInfo2 = Line极耳右侧抓边Info.Copy();

                    PointInfo point极耳右根部组合交点 = new PointInfo();
                    if (Plugin极耳右根部组合交点.ExecuteObj())
                    {
                        point极耳右根部组合交点.Col = params极耳右根部组合交点.hv_IntersectionColumn;
                        point极耳右根部组合交点.Row = params极耳右根部组合交点.hv_IntersectionRow;
                        point极耳右根部组合交点.ModuleName = "极耳右根部组合交点";
                        point极耳右根部组合交点.nProcessID = ProcessID;
                        point极耳右根部组合交点.bIsSuccess = true;


                    }
                    else
                    {
                        Log4netRecord.ErrorLog.Debug($"流程{Plugin极耳右根部组合交点.moduleInfo.ProcessID}&&{Plugin极耳右根部组合交点.moduleInfo.ModuleName}---{params极耳右根部组合交点.m_strRunMessage}");
                    }

                    Product尺寸.PointResults.Add(point极耳右根部组合交点);
                    HOperatorSet.GenCrossContourXld(out HObject cross组合交点, point极耳右根部组合交点.Row, point极耳右根部组合交点.Col, 10, 45);
                    ShowHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = cross组合交点, m_stuGetColor = HalconColor.gold });
                    HOperatorSet.GenCrossContourXld(out cross组合交点, point极耳左根部组合交点.Row, point极耳左根部组合交点.Col, 10, 45);
                    ShowHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = cross组合交点, m_stuGetColor = HalconColor.gold });
                    HOperatorSet.GenCrossContourXld(out cross组合交点, point极片右侧组合交点.Row, point极片右侧组合交点.Col, 10, 45);
                    ShowHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = cross组合交点, m_stuGetColor = HalconColor.gold });

                    #endregion

                    #endregion

                    #region 凸点测量检测模块执行

                    PointInfo point左侧V角凸点1 = new PointInfo() { nProcessID = ProcessID, ModuleName = "左侧V角凸点1" };
                    PointInfo point左侧V角凸点2 = new PointInfo() { nProcessID = ProcessID, ModuleName = "左侧V角凸点2" };
                    PointInfo point右侧V角凸点1 = new PointInfo() { nProcessID = ProcessID, ModuleName = "右侧V角凸点1" };
                    PointInfo point右侧V角凸点2 = new PointInfo() { nProcessID = ProcessID, ModuleName = "右侧V角凸点2" };
                    HOperatorSet.GenCrossContourXld(out HObject cross, point左侧V角凸点1.Row, point左侧V角凸点1.Col, 10, 45);
                    ShowHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = cross, m_stuGetColor = HalconColor.gold });
                    HOperatorSet.GenCrossContourXld(out cross, point左侧V角凸点2.Row, point左侧V角凸点2.Col, 10, 45);
                    ShowHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = cross, m_stuGetColor = HalconColor.gold });
                    HOperatorSet.GenCrossContourXld(out cross, point右侧V角凸点1.Row, point右侧V角凸点1.Col, 10, 45);
                    ShowHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = cross, m_stuGetColor = HalconColor.gold });
                    HOperatorSet.GenCrossContourXld(out cross, point右侧V角凸点2.Row, point右侧V角凸点2.Col, 10, 45);
                    ShowHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = cross, m_stuGetColor = HalconColor.gold });


                    //开下并行//减少耗时
                    Parallel.Invoke(
                    #region point左侧V角凸点1
                    () =>

                    {
                        IPlugin Plugin左侧V角凸点1 = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == $"左侧V角凸点1");
                        ParamsReusableMeasureDetect Params左侧V角凸点1 = Plugin左侧V角凸点1.PluginParams as ParamsReusableMeasureDetect;
                        Params左侧V角凸点1.ho_Image = InfoImgInfo.ho_image;
                        Params左侧V角凸点1.EnableJumpDetection = true;
                        Params左侧V角凸点1.m_bUsePositionFollow = true;
                        Params左侧V角凸点1.m_bCurrentPoseValid = true;
                        Params左侧V角凸点1.m_fBaseCol = point基准.Col;
                        Params左侧V角凸点1.m_fBaseRow = point基准.Row;
                        Params左侧V角凸点1.m_fBaseAngle = 0;
                        Params左侧V角凸点1.m_fCurrentCol = Cross粗定位工具PointInfo.point.Col;
                        Params左侧V角凸点1.m_fCurrentRow = Cross粗定位工具PointInfo.point.Row;
                       // Params左侧V角凸点1.m_fCurrentRow = 0;
                        if (Plugin左侧V角凸点1.ExecuteObj())
                        {
                            if (Params左侧V角凸点1.EnableJumpDetection)
                            {
                                point左侧V角凸点1.Col = Params左侧V角凸点1.JumpOutput.IntersectionCol;
                                point左侧V角凸点1.Row = Params左侧V角凸点1.JumpOutput.IntersectionRow;
                                point左侧V角凸点1.ModuleName = "左侧V角凸点1";
                                point左侧V角凸点1.nProcessID = ProcessID;
                                point左侧V角凸点1.bIsSuccess = true;

                            }



                        }
                        else
                        {

                            Log4netRecord.ErrorLog.Debug($"流程{Plugin左侧V角凸点1.moduleInfo.ProcessID}&&{Plugin左侧V角凸点1.moduleInfo.ModuleName}---{Params左侧V角凸点1.m_strRunMessage}");
                        }
                    },
                    #endregion, 
                    #region point左侧V角凸点2
                    () =>

                    {

                        IPlugin Plugin左侧V角凸点2 = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == $"左侧V角凸点2");
                        ParamsReusableMeasureDetect Params左侧V角凸点2 = Plugin左侧V角凸点2.PluginParams as ParamsReusableMeasureDetect;
                        Params左侧V角凸点2.ho_Image = InfoImgInfo.ho_image;
                        Params左侧V角凸点2.EnableJumpDetection = true;
                        Params左侧V角凸点2.m_bUsePositionFollow = true;
                        Params左侧V角凸点2.m_bCurrentPoseValid = true;
                        Params左侧V角凸点2.m_fBaseCol = point基准.Col;
                        Params左侧V角凸点2.m_fBaseRow = point基准.Row;
                        Params左侧V角凸点2.m_fBaseAngle = 0;
                        Params左侧V角凸点2.m_fCurrentCol = Cross粗定位工具PointInfo.point.Col;
                        Params左侧V角凸点2.m_fCurrentRow = Cross粗定位工具PointInfo.point.Row;
                        //Params左侧V角凸点2.m_fCurrentRow = 0;
                        if (Plugin左侧V角凸点2.ExecuteObj())
                        {
                            if (Params左侧V角凸点2.EnableJumpDetection)
                            {
                                point左侧V角凸点2.Col = Params左侧V角凸点2.JumpOutput.IntersectionCol;
                                point左侧V角凸点2.Row = Params左侧V角凸点2.JumpOutput.IntersectionRow;
                                point左侧V角凸点2.ModuleName = "左侧V角凸点2";
                                point左侧V角凸点2.nProcessID = ProcessID;
                                point左侧V角凸点2.bIsSuccess = true;

                            }



                        }
                        else
                        {

                            Log4netRecord.ErrorLog.Debug($"流程{Plugin左侧V角凸点2.moduleInfo.ProcessID}&&{Plugin左侧V角凸点2.moduleInfo.ModuleName}---{Params左侧V角凸点2.m_strRunMessage}");
                        }
                    },
                    #endregion
                    #region point右侧V角凸点1
                    () =>
                    {
                        IPlugin Plugin右侧V角凸点1 = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == $"右侧V角凸点1");
                        ParamsReusableMeasureDetect Params右侧V角凸点1 = Plugin右侧V角凸点1.PluginParams as ParamsReusableMeasureDetect;
                        Params右侧V角凸点1.ho_Image = InfoImgInfo.ho_image;
                        Params右侧V角凸点1.EnableJumpDetection = true;
                        Params右侧V角凸点1.m_bUsePositionFollow = true;
                        Params右侧V角凸点1.m_bCurrentPoseValid = true;
                        Params右侧V角凸点1.m_fBaseCol = point基准.Col;
                        Params右侧V角凸点1.m_fBaseRow = point基准.Row;
                        Params右侧V角凸点1.m_fBaseAngle = 0;
                        Params右侧V角凸点1.m_fCurrentCol = Cross粗定位工具PointInfo.point.Col;
                        Params右侧V角凸点1.m_fCurrentRow = Cross粗定位工具PointInfo.point.Row;
                        //Params右侧V角凸点1.m_fCurrentRow = 0;
                        if (Plugin右侧V角凸点1.ExecuteObj())
                        {
                            if (Params右侧V角凸点1.EnableJumpDetection)
                            {
                                point右侧V角凸点1.Col = Params右侧V角凸点1.JumpOutput.IntersectionCol;
                                point右侧V角凸点1.Row = Params右侧V角凸点1.JumpOutput.IntersectionRow;
                                point右侧V角凸点1.ModuleName = "右侧V角凸点1";
                                point右侧V角凸点1.nProcessID = ProcessID;
                                point右侧V角凸点1.bIsSuccess = true;

                            }



                        }
                        else
                        {

                            Log4netRecord.ErrorLog.Debug($"流程{Plugin右侧V角凸点1.moduleInfo.ProcessID}&&{Plugin右侧V角凸点1.moduleInfo.ModuleName}---{Params右侧V角凸点1.m_strRunMessage}");
                        }
                    },
                    #endregion
                    #region point右侧V角凸点2
                    () =>
                    {
                        IPlugin Plugin右侧V角凸点2 = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == $"右侧V角凸点2");
                        ParamsReusableMeasureDetect Params右侧V角凸点2 = Plugin右侧V角凸点2.PluginParams as ParamsReusableMeasureDetect;
                        Params右侧V角凸点2.ho_Image = InfoImgInfo.ho_image;
                        Params右侧V角凸点2.EnableJumpDetection = true;
                        Params右侧V角凸点2.m_bUsePositionFollow = true;
                        Params右侧V角凸点2.m_bCurrentPoseValid = true;
                        Params右侧V角凸点2.m_fBaseCol = point基准.Col;
                        Params右侧V角凸点2.m_fBaseRow = point基准.Row;
                        Params右侧V角凸点2.m_fBaseAngle = 0;
                        Params右侧V角凸点2.m_fCurrentCol = Cross粗定位工具PointInfo.point.Col;
                        Params右侧V角凸点2.m_fCurrentRow = Cross粗定位工具PointInfo.point.Row;
                        //Params右侧V角凸点2.m_fCurrentRow = 0;
                        if (Plugin右侧V角凸点2.ExecuteObj())
                        {
                            if (Params右侧V角凸点2.EnableJumpDetection)
                            {
                                point右侧V角凸点2.Col = Params右侧V角凸点2.JumpOutput.IntersectionCol;
                                point右侧V角凸点2.Row = Params右侧V角凸点2.JumpOutput.IntersectionRow;
                                point右侧V角凸点2.ModuleName = "右侧V角凸点2";
                                point右侧V角凸点2.nProcessID = ProcessID;
                                point右侧V角凸点2.bIsSuccess = true;
                            }



                        }
                        else
                        {

                            Log4netRecord.ErrorLog.Debug($"流程{Plugin右侧V角凸点2.moduleInfo.ProcessID}&&{Plugin右侧V角凸点2.moduleInfo.ModuleName}---{Params右侧V角凸点2.m_strRunMessage}");
                        }
                    }
                    #endregion
                    );

                    Product尺寸.PointResults.Add(point左侧V角凸点1);
                    Product尺寸.PointResults.Add(point左侧V角凸点2);
                    Product尺寸.PointResults.Add(point右侧V角凸点1);
                    Product尺寸.PointResults.Add(point右侧V角凸点2);
                    HOperatorSet.GenCrossContourXld(out cross, point左侧V角凸点1.Row, point左侧V角凸点1.Col, 10, 45);
                    ShowHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = cross, m_stuGetColor = HalconColor.gold });
                    HOperatorSet.GenCrossContourXld(out cross, point左侧V角凸点2.Row, point左侧V角凸点2.Col, 10, 45);
                    ShowHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = cross, m_stuGetColor = HalconColor.gold });
                    HOperatorSet.GenCrossContourXld(out cross, point右侧V角凸点1.Row, point右侧V角凸点1.Col, 10, 45);
                    ShowHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = cross, m_stuGetColor = HalconColor.gold });
                    HOperatorSet.GenCrossContourXld(out cross, point右侧V角凸点2.Row, point右侧V角凸点2.Col, 10, 45);
                    ShowHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = cross, m_stuGetColor = HalconColor.gold });


                    #endregion

                    #region Blob模块执行
                    ObservableCollection<NgDefectResult> ngDefect左底部V角检测有无 = new ObservableCollection<NgDefectResult>();
                    ObservableCollection<NgDefectResult> ngDefect右底部V角检测有无 = new ObservableCollection<NgDefectResult>();
                    ObservableCollection<NgDefectResult> ngDefect极片有无 = new ObservableCollection<NgDefectResult>();
                    ObservableCollection<NgDefectResult> ngDefect极耳翻折检测 = new ObservableCollection<NgDefectResult>();
                    ObservableCollection<NgDefectResult> ngDefect极耳余料检测 = new ObservableCollection<NgDefectResult>();
                    ObservableCollection<NgDefectResult> ngDefect极耳涂布内翻检测 = new ObservableCollection<NgDefectResult>();
                    List<RegionShowInfo> RegionShow左底部V角检测有无 = new List<RegionShowInfo>();
                    List<RegionShowInfo> RegionShow右底部V角检测有无 = new List<RegionShowInfo>();
                    List<RegionShowInfo> RegionShow极片有无 = new List<RegionShowInfo>();
                    List<RegionShowInfo> RegionShow极耳翻折检测 = new List<RegionShowInfo>();
                    List<RegionShowInfo> RegionShow极耳余料检测 = new List<RegionShowInfo>();
                    List<RegionShowInfo> RegionShow极耳涂布内翻检测 = new List<RegionShowInfo>();

                    //开并行吧，毕竟也耗时间
                    Parallel.Invoke(
                    #region 左底部V角检测有无
                        () =>
                        {
                            IPlugin Plugin左底部V角检测有无 = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == $"左底部V角检测有无");
                            ParamsBlobAnalysis params左底部V角检测有无 = Plugin左底部V角检测有无.PluginParams as ParamsBlobAnalysis;
                            params左底部V角检测有无.ho_Image = InfoImgInfo.ho_image;
                            params左底部V角检测有无.m_fBaseCol = point基准.Col;
                            params左底部V角检测有无.m_fBaseRow = point基准.Row;
                            // paramsBlobAnalysis.m_fBaseAngle = 0;
                            params左底部V角检测有无.m_fCurrentCol = Cross粗定位工具PointInfo.point.Col;
                            params左底部V角检测有无.m_fCurrentRow = Cross粗定位工具PointInfo.point.Row;
                            // paramsBlobAnalysis.m_fCurrentAngle = 0;
                            params左底部V角检测有无.m_bUsePositionFollow = true;
                            if(params左底部V角检测有无.m_bIsEnable)
                            {
                                if (Plugin左底部V角检测有无.ExecuteObj())
                                {
                                    RegionShow左底部V角检测有无.Add(new RegionShowInfo() { m_hRegion = params左底部V角检测有无.m_ho_OutLocalRoiRegion.CopyObj(1, -1), m_stuGetColor = HalconColor.orange });
                                    if (params左底部V角检测有无.m_lMaskRoiParams.Count > 0 && params左底部V角检测有无.m_ho_OutMaskRoiRegion != null)
                                    {
                                        RegionShow左底部V角检测有无.Add(new RegionShowInfo() { m_hRegion = params左底部V角检测有无.m_ho_OutMaskRoiRegion.CopyObj(1, -1), m_stuGetColor = HalconColor.orange });
                                    }

                                    foreach (var item in params左底部V角检测有无.m_lOutRegionResults)
                                    {
                                        try
                                        {

                                            HOperatorSet.SelectObj(params左底部V角检测有无.m_ho_OutSelectedRegion, out HObject temp, item.m_nIndex);
                                            HOperatorSet.SmallestRectangle1(temp, out HTuple row1, out HTuple col1, out HTuple row2, out HTuple col2);
                                            HOperatorSet.CropPart(InfoImgInfo.ho_image, out HObject imagePart, item.m_fCenterRow, item.m_fCenterCol, (col1 - col2).TupleAbs() > 68 ? (col1 - col2).TupleAbs() : (HTuple)68, (row1 - row2).TupleAbs() * 1.1 > 68 ? (row1 - row2).TupleAbs() * 1.1 : (HTuple)68);
                                            NgDefectResult ngDefect = new NgDefectResult();
                                            ngDefect.ClassName = "V角检测有无";
                                            ngDefect.Area = item.m_fArea * Calipix_x * Calipix_y;
                                            ngDefect.HeightPx = (row1 - row2).TupleAbs();
                                            ngDefect.WidthPx = (col1 - col2).TupleAbs();
                                            ngDefect.Height = (row1 - row2).TupleAbs() * Calipix_y;
                                            ngDefect.Width = (col1 - col2).TupleAbs() * Calipix_x;
                                            ngDefect.PixelToMmH = Calipix_x;
                                            ngDefect.PixelToMmV = Calipix_y;
                                            ngDefect.Region = temp;
                                            ngDefect.CropImage = imagePart;
                                            ngDefect左底部V角检测有无.Add(ngDefect);


                                        }
                                        catch (Exception)
                                        {

                                            MessageNotify.AddStopAlarm(new StopAlarmMessage() { StopAlarmState = StopAlarmStateEnum.停机, name = $"结果__流程{Plugin左底部V角检测有无.moduleInfo.ProcessID}&&{Plugin左底部V角检测有无.moduleInfo.ModuleName}", describe = $"流程{Plugin左底部V角检测有无.moduleInfo.ProcessID}&&{Plugin左底部V角检测有无.moduleInfo.ModuleName}——出问题，需排查原因，不能开机，停机处理" });
                                            HTuple end1 = (Product尺寸.detectionTime.Image_EndTime - Product尺寸.detectionTime.Image_StartTime) * 1000;
                                            ShowHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前耗时：{end1.D.ToString("0.00")}" });
                                            HOperatorSet.CountSeconds(out Product尺寸.detectionTime.Image_EndTime);
                                            ProParam尺寸业务.ProductCallBack(Product尺寸);
                                            return;
                                            //throw;
                                        }


                                    }
                                }
                                else
                                {
                                    MessageNotify.AddStopAlarm(new StopAlarmMessage() { StopAlarmState = StopAlarmStateEnum.停机, name = $"流程{Plugin左底部V角检测有无.moduleInfo.ProcessID}&&{Plugin左底部V角检测有无.moduleInfo.ModuleName}", describe = $"流程{Plugin左底部V角检测有无.moduleInfo.ProcessID}&&{Plugin左底部V角检测有无.moduleInfo.ModuleName}——出问题，需排查原因，不能开机，停机处理" });
                                    HOperatorSet.CountSeconds(out Product尺寸.detectionTime.Image_EndTime);
                                    ProParam尺寸业务.ProductCallBack(Product尺寸);
                                    return;
                                }
                            }


                        
                        },
                    #endregion

                    #region 右底部V角检测有无
                            () =>
                            {
                                IPlugin Plugin右底部V角检测有无 = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == $"右底部V角检测有无");
                                ParamsBlobAnalysis params右底部V角检测有无 = Plugin右底部V角检测有无.PluginParams as ParamsBlobAnalysis;
                                params右底部V角检测有无.ho_Image = InfoImgInfo.ho_image;
                                params右底部V角检测有无.m_fBaseCol = point基准.Col;
                                params右底部V角检测有无.m_fBaseRow = point基准.Row;
                                // paramsBlobAnalysis.m_fBaseAngle = 0;
                                params右底部V角检测有无.m_fCurrentCol = Cross粗定位工具PointInfo.point.Col;
                                params右底部V角检测有无.m_fCurrentRow = Cross粗定位工具PointInfo.point.Row;
                                // paramsBlobAnalysis.m_fCurrentAngle = 0;
                                params右底部V角检测有无.m_bUsePositionFollow = true;

                                if(params右底部V角检测有无.m_bIsEnable)
                                {
                                    if (Plugin右底部V角检测有无.ExecuteObj())
                                    {

                                        RegionShow右底部V角检测有无.Add(new RegionShowInfo() { m_hRegion = params右底部V角检测有无.m_ho_OutLocalRoiRegion.CopyObj(1, -1), m_stuGetColor = HalconColor.orange });
                                        if (params右底部V角检测有无.m_lMaskRoiParams.Count > 0 && params右底部V角检测有无.m_ho_OutMaskRoiRegion != null)
                                        {
                                            RegionShow右底部V角检测有无.Add(new RegionShowInfo() { m_hRegion = params右底部V角检测有无.m_ho_OutMaskRoiRegion.CopyObj(1, -1), m_stuGetColor = HalconColor.orange });
                                        }

                                        foreach (var item in params右底部V角检测有无.m_lOutRegionResults)
                                        {
                                            try
                                            {

                                                HOperatorSet.SelectObj(params右底部V角检测有无.m_ho_OutSelectedRegion, out HObject temp, item.m_nIndex);
                                                HOperatorSet.SmallestRectangle1(temp, out HTuple row1, out HTuple col1, out HTuple row2, out HTuple col2);
                                                HOperatorSet.CropPart(InfoImgInfo.ho_image, out HObject imagePart, item.m_fCenterRow, item.m_fCenterCol, (col1 - col2).TupleAbs() > 68 ? (col1 - col2).TupleAbs() : (HTuple)68, (row1 - row2).TupleAbs() * 1.1 > 68 ? (row1 - row2).TupleAbs() * 1.1 : (HTuple)68);
                                                NgDefectResult ngDefect = new NgDefectResult();
                                                ngDefect.ClassName = "V角检测有无";
                                                ngDefect.Area = item.m_fArea * Calipix_x * Calipix_y;
                                                ngDefect.HeightPx = (row1 - row2).TupleAbs();
                                                ngDefect.WidthPx = (col1 - col2).TupleAbs();
                                                ngDefect.Height = (row1 - row2).TupleAbs() * Calipix_y;
                                                ngDefect.Width = (col1 - col2).TupleAbs() * Calipix_x;
                                                ngDefect.PixelToMmH = Calipix_x;
                                                ngDefect.PixelToMmV = Calipix_y;
                                                ngDefect.Region = temp;
                                                ngDefect.CropImage = imagePart;
                                                ngDefect右底部V角检测有无.Add(ngDefect);

                                            }
                                            catch (Exception)
                                            {
                                                MessageNotify.AddStopAlarm(new StopAlarmMessage() { StopAlarmState = StopAlarmStateEnum.停机, name = $"流程{Plugin右底部V角检测有无.moduleInfo.ProcessID}&&{Plugin右底部V角检测有无.moduleInfo.ModuleName}", describe = $"结果__流程{Plugin右底部V角检测有无.moduleInfo.ProcessID}&&{Plugin右底部V角检测有无.moduleInfo.ModuleName}——出问题，需排查原因，不能开机，停机处理" });
                                                HOperatorSet.CountSeconds(out Product尺寸.detectionTime.Image_EndTime);
                                                HTuple end2 = (Product尺寸.detectionTime.Image_EndTime - Product尺寸.detectionTime.Image_StartTime) * 1000;
                                                ShowHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前耗时：{end2.D.ToString("0.00")}" });
                                                ProParam尺寸业务.ProductCallBack(Product尺寸);
                                                return;
                                                //throw;
                                            }


                                        }
                                    }
                                    else
                                    {

                                        MessageNotify.AddStopAlarm(new StopAlarmMessage() { StopAlarmState = StopAlarmStateEnum.停机, name = $"流程{Plugin右底部V角检测有无.moduleInfo.ProcessID}&&{Plugin右底部V角检测有无.moduleInfo.ModuleName}", describe = $"流程{Plugin右底部V角检测有无.moduleInfo.ProcessID}&&{Plugin右底部V角检测有无.moduleInfo.ModuleName}——出问题，需排查原因，不能开机，停机处理" });
                                        HOperatorSet.CountSeconds(out Product尺寸.detectionTime.Image_EndTime);
                                        ProParam尺寸业务.ProductCallBack(Product尺寸);
                                        return;
                                    }
                                }
                              
                            },
                    #endregion

                    #region 极片有无
                    () =>
                    {
                        IPlugin Plugin极片有无 = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == $"极片有无");
                        ParamsBlobAnalysis params极片有无 = Plugin极片有无.PluginParams as ParamsBlobAnalysis;
                        params极片有无.ho_Image = InfoImgInfo.ho_image;
                        params极片有无.m_fBaseCol = point基准.Col;
                        params极片有无.m_fBaseRow = point基准.Row;
                        // paramsBlobAnalysis.m_fBaseAngle = 0;
                        params极片有无.m_fCurrentCol = Cross粗定位工具PointInfo.point.Col;
                        params极片有无.m_fCurrentRow = Cross粗定位工具PointInfo.point.Row;
                        // paramsBlobAnalysis.m_fCurrentAngle = 0;
                        params极片有无.m_bUsePositionFollow = true;

                        if(params极片有无.m_bIsEnable)
                        {
                            if (Plugin极片有无.ExecuteObj())
                            {
                                RegionShow极片有无.Add(new RegionShowInfo() { m_hRegion = params极片有无.m_ho_OutLocalRoiRegion.CopyObj(1, -1), m_stuGetColor = HalconColor.orange });
                                if (params极片有无.m_lMaskRoiParams.Count > 0 && params极片有无.m_ho_OutMaskRoiRegion != null)
                                {
                                    RegionShow极片有无.Add(new RegionShowInfo() { m_hRegion = params极片有无.m_ho_OutMaskRoiRegion.CopyObj(1, -1), m_stuGetColor = HalconColor.orange });
                                }

                                foreach (var item in params极片有无.m_lOutRegionResults)
                                {
                                    try
                                    {

                                        HOperatorSet.SelectObj(params极片有无.m_ho_OutSelectedRegion, out HObject temp, item.m_nIndex);
                                        HOperatorSet.SmallestRectangle1(temp, out HTuple row1, out HTuple col1, out HTuple row2, out HTuple col2);
                                        HOperatorSet.CropPart(InfoImgInfo.ho_image, out HObject imagePart, item.m_fCenterRow, item.m_fCenterCol, (col1 - col2).TupleAbs() > 68 ? (col1 - col2).TupleAbs() : (HTuple)68, (row1 - row2).TupleAbs() * 1.1 > 68 ? (row1 - row2).TupleAbs() * 1.1 : (HTuple)68);
                                        NgDefectResult ngDefect = new NgDefectResult();
                                        ngDefect.ClassName = "极片有无";
                                        ngDefect.Area = item.m_fArea * Calipix_x * Calipix_y;
                                        ngDefect.HeightPx = (row1 - row2).TupleAbs();
                                        ngDefect.WidthPx = (col1 - col2).TupleAbs();
                                        ngDefect.Height = (row1 - row2).TupleAbs() * Calipix_y;
                                        ngDefect.Width = (col1 - col2).TupleAbs() * Calipix_x;
                                        ngDefect.PixelToMmH = Calipix_x;
                                        ngDefect.PixelToMmV = Calipix_y;
                                        ngDefect.Region = temp;
                                        ngDefect.CropImage = imagePart;
                                        ngDefect极片有无.Add(ngDefect);

                                    }
                                    catch (Exception)
                                    {
                                        MessageNotify.AddStopAlarm(new StopAlarmMessage() { StopAlarmState = StopAlarmStateEnum.停机, name = $"流程{Plugin极片有无.moduleInfo.ProcessID}&&{Plugin极片有无.moduleInfo.ModuleName}", describe = $"结果__流程{Plugin极片有无.moduleInfo.ProcessID}&&{Plugin极片有无.moduleInfo.ModuleName}——出问题，需排查原因，不能开机，停机处理" });
                                        HOperatorSet.CountSeconds(out Product尺寸.detectionTime.Image_EndTime);
                                        HTuple end3 = (Product尺寸.detectionTime.Image_EndTime - Product尺寸.detectionTime.Image_StartTime) * 1000;
                                        ShowHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前耗时：{end3.D.ToString("0.00")}" });
                                        ProParam尺寸业务.ProductCallBack(Product尺寸);
                                        return;
                                        //throw;
                                    }


                                }
                            }
                            else
                            {

                                MessageNotify.AddStopAlarm(new StopAlarmMessage() { StopAlarmState = StopAlarmStateEnum.停机, name = $"流程{Plugin极片有无.moduleInfo.ProcessID}&&{Plugin极片有无.moduleInfo.ModuleName}", describe = $"流程{Plugin极片有无.moduleInfo.ProcessID}&&{Plugin极片有无.moduleInfo.ModuleName}——出问题，需排查原因，不能开机，停机处理" });
                                HOperatorSet.CountSeconds(out Product尺寸.detectionTime.Image_EndTime);
                                ProParam尺寸业务.ProductCallBack(Product尺寸);
                                return;
                            }
                        }

                       
                    },
                    #endregion ，

                    #region 极耳翻折检测
                              () =>
                              {
                                  IPlugin Plugin极耳翻折检测 = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == $"极耳翻折检测");
                                  ParamsBlobAnalysis params极耳翻折检测 = Plugin极耳翻折检测.PluginParams as ParamsBlobAnalysis;
                                  params极耳翻折检测.ho_Image = InfoImgInfo.ho_image;
                                  params极耳翻折检测.m_fBaseCol = point基准.Col;
                                  params极耳翻折检测.m_fBaseRow = point基准.Row;
                                  // paramsBlobAnalysis.m_fBaseAngle = 0;
                                  params极耳翻折检测.m_fCurrentCol = Cross粗定位工具PointInfo.point.Col;
                                  params极耳翻折检测.m_fCurrentRow = Cross粗定位工具PointInfo.point.Row;
                                  // paramsBlobAnalysis.m_fCurrentAngle = 0;
                                  params极耳翻折检测.m_bUsePositionFollow = true;

                                  if(params极耳翻折检测.m_bIsEnable)
                                  {
                                      if (Plugin极耳翻折检测.ExecuteObj())
                                      {
                                          RegionShow极耳翻折检测.Add(new RegionShowInfo() { m_hRegion = params极耳翻折检测.m_ho_OutLocalRoiRegion.CopyObj(1, -1), m_stuGetColor = HalconColor.orange });
                                          if (params极耳翻折检测.m_lMaskRoiParams.Count > 0 && params极耳翻折检测.m_ho_OutMaskRoiRegion != null)
                                          {
                                              RegionShow极耳翻折检测.Add(new RegionShowInfo() { m_hRegion = params极耳翻折检测.m_ho_OutMaskRoiRegion.CopyObj(1, -1), m_stuGetColor = HalconColor.orange });
                                          }

                                          foreach (var item in params极耳翻折检测.m_lOutRegionResults)
                                          {
                                              try
                                              {

                                                  HOperatorSet.SelectObj(params极耳翻折检测.m_ho_OutSelectedRegion, out HObject temp, item.m_nIndex);
                                                  HOperatorSet.SmallestRectangle1(temp, out HTuple row1, out HTuple col1, out HTuple row2, out HTuple col2);
                                                  HOperatorSet.CropPart(InfoImgInfo.ho_image, out HObject imagePart, item.m_fCenterRow, item.m_fCenterCol, (col1 - col2).TupleAbs() > 68 ? (col1 - col2).TupleAbs() : (HTuple)68, (row1 - row2).TupleAbs() * 1.1 > 68 ? (row1 - row2).TupleAbs() * 1.1 : (HTuple)68);
                                                  NgDefectResult ngDefect = new NgDefectResult();
                                                  ngDefect.ClassName = "极耳翻折检测";
                                                  ngDefect.Area = item.m_fArea * Calipix_x * Calipix_y;
                                                  ngDefect.HeightPx = (row1 - row2).TupleAbs();
                                                  ngDefect.WidthPx = (col1 - col2).TupleAbs();
                                                  ngDefect.Height = (row1 - row2).TupleAbs() * Calipix_y;
                                                  ngDefect.Width = (col1 - col2).TupleAbs() * Calipix_x;
                                                  ngDefect.PixelToMmH = Calipix_x;
                                                  ngDefect.PixelToMmV = Calipix_y;
                                                  ngDefect.Region = temp;
                                                  ngDefect.CropImage = imagePart;
                                                  ngDefect极耳翻折检测.Add(ngDefect);

                                              }
                                              catch (Exception)
                                              {
                                                  MessageNotify.AddStopAlarm(new StopAlarmMessage() { StopAlarmState = StopAlarmStateEnum.停机, name = $"流程{Plugin极耳翻折检测.moduleInfo.ProcessID}&&{Plugin极耳翻折检测.moduleInfo.ModuleName}", describe = $"结果__流程{Plugin极耳翻折检测.moduleInfo.ProcessID}&&{Plugin极耳翻折检测.moduleInfo.ModuleName}——出问题，需排查原因，不能开机，停机处理" });
                                                  HOperatorSet.CountSeconds(out Product尺寸.detectionTime.Image_EndTime);
                                                  HTuple end4 = (Product尺寸.detectionTime.Image_EndTime - Product尺寸.detectionTime.Image_StartTime) * 1000;
                                                  ShowHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前耗时：{end4.D.ToString("0.00")}" });
                                                  ProParam尺寸业务.ProductCallBack(Product尺寸);
                                                  return;
                                                  //throw;
                                              }


                                          }
                                      }
                                      else
                                      {

                                          MessageNotify.AddStopAlarm(new StopAlarmMessage() { StopAlarmState = StopAlarmStateEnum.停机, name = $"流程{Plugin极耳翻折检测.moduleInfo.ProcessID}&&{Plugin极耳翻折检测.moduleInfo.ModuleName}", describe = $"流程{Plugin极耳翻折检测.moduleInfo.ProcessID}&&{Plugin极耳翻折检测.moduleInfo.ModuleName}——出问题，需排查原因，不能开机，停机处理" });
                                          HOperatorSet.CountSeconds(out Product尺寸.detectionTime.Image_EndTime);
                                          ProParam尺寸业务.ProductCallBack(Product尺寸);
                                          return;
                                      }
                                  }

                                
                              },
                    #endregion

                    #region 极耳余料检测
                                    () =>
                                    {
                                        IPlugin Plugin极耳余料检测 = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == $"极耳余料检测");
                                        ParamsBlobAnalysis params极耳余料检测 = Plugin极耳余料检测.PluginParams as ParamsBlobAnalysis;
                                        params极耳余料检测.ho_Image = InfoImgInfo.ho_image;
                                        params极耳余料检测.m_fBaseCol = point基准.Col;
                                        params极耳余料检测.m_fBaseRow = point基准.Row;
                                        // paramsBlobAnalysis.m_fBaseAngle = 0;
                                        params极耳余料检测.m_fCurrentCol = Cross粗定位工具PointInfo.point.Col;
                                        params极耳余料检测.m_fCurrentRow = Cross粗定位工具PointInfo.point.Row;
                                        // paramsBlobAnalysis.m_fCurrentAngle = 0;
                                        params极耳余料检测.m_bUsePositionFollow = true;
                                        if(params极耳余料检测.m_bIsEnable)
                                        {
                                            if (Plugin极耳余料检测.ExecuteObj())
                                            {

                                                RegionShow极耳余料检测.Add(new RegionShowInfo() { m_hRegion = params极耳余料检测.m_ho_OutLocalRoiRegion.CopyObj(1, -1), m_stuGetColor = HalconColor.orange });
                                                if (params极耳余料检测.m_lMaskRoiParams.Count > 0 && params极耳余料检测.m_ho_OutMaskRoiRegion != null)
                                                {
                                                    RegionShow极耳余料检测.Add(new RegionShowInfo() { m_hRegion = params极耳余料检测.m_ho_OutMaskRoiRegion.CopyObj(1, -1), m_stuGetColor = HalconColor.orange });
                                                }

                                                foreach (var item in params极耳余料检测.m_lOutRegionResults)
                                                {
                                                    try
                                                    {

                                                        HOperatorSet.SelectObj(params极耳余料检测.m_ho_OutSelectedRegion, out HObject temp, item.m_nIndex);
                                                        HOperatorSet.SmallestRectangle1(temp, out HTuple row1, out HTuple col1, out HTuple row2, out HTuple col2);
                                                        HOperatorSet.CropPart(InfoImgInfo.ho_image, out HObject imagePart, item.m_fCenterRow, item.m_fCenterCol, (col1 - col2).TupleAbs() > 68 ? (col1 - col2).TupleAbs() : (HTuple)68, (row1 - row2).TupleAbs() * 1.1 > 68 ? (row1 - row2).TupleAbs() * 1.1 : (HTuple)68);
                                                        NgDefectResult ngDefect = new NgDefectResult();
                                                        ngDefect.ClassName = "极耳余料检测";
                                                        ngDefect.Area = item.m_fArea * Calipix_x * Calipix_y;
                                                        ngDefect.HeightPx = (row1 - row2).TupleAbs();
                                                        ngDefect.WidthPx = (col1 - col2).TupleAbs();
                                                        ngDefect.Height = (row1 - row2).TupleAbs() * Calipix_y;
                                                        ngDefect.Width = (col1 - col2).TupleAbs() * Calipix_x;
                                                        ngDefect.PixelToMmH = Calipix_x;
                                                        ngDefect.PixelToMmV = Calipix_y;
                                                        ngDefect.Region = temp;
                                                        ngDefect.CropImage = imagePart;
                                                        ngDefect极耳余料检测.Add(ngDefect);

                                                    }
                                                    catch (Exception)
                                                    {
                                                        MessageNotify.AddStopAlarm(new StopAlarmMessage() { StopAlarmState = StopAlarmStateEnum.停机, name = $"流程{Plugin极耳余料检测.moduleInfo.ProcessID}&&{Plugin极耳余料检测.moduleInfo.ModuleName}", describe = $"结果__流程{Plugin极耳余料检测.moduleInfo.ProcessID}&&{Plugin极耳余料检测.moduleInfo.ModuleName}——出问题，需排查原因，不能开机，停机处理" });
                                                        HOperatorSet.CountSeconds(out Product尺寸.detectionTime.Image_EndTime);
                                                        HTuple end5 = (Product尺寸.detectionTime.Image_EndTime - Product尺寸.detectionTime.Image_StartTime) * 1000;
                                                        ShowHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前耗时：{end5.D.ToString("0.00")}" });
                                                        ProParam尺寸业务.ProductCallBack(Product尺寸);
                                                        return;
                                                        //throw;
                                                    }


                                                }
                                            }
                                            else
                                            {

                                                MessageNotify.AddStopAlarm(new StopAlarmMessage() { StopAlarmState = StopAlarmStateEnum.停机, name = $"流程{Plugin极耳余料检测.moduleInfo.ProcessID}&&{Plugin极耳余料检测.moduleInfo.ModuleName}", describe = $"流程{Plugin极耳余料检测.moduleInfo.ProcessID}&&{Plugin极耳余料检测.moduleInfo.ModuleName}——出问题，需排查原因，不能开机，停机处理" });
                                                HOperatorSet.CountSeconds(out Product尺寸.detectionTime.Image_EndTime);
                                                HTuple end6 = (Product尺寸.detectionTime.Image_EndTime - Product尺寸.detectionTime.Image_StartTime) * 1000;
                                                ShowHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前耗时：{end6.D.ToString("0.00")}" });
                                                ProParam尺寸业务.ProductCallBack(Product尺寸);
                                                return;
                                            }
                                        }
                                      
                                    },
                    #endregion

                    #region 极耳涂布内翻检测
                                    () =>
                                    {


                                        IPlugin Plugin极耳涂布内翻检测 = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == $"极耳涂布内翻检测");
                                        ParamsBlobAnalysis params极耳涂布内翻检测 = Plugin极耳涂布内翻检测.PluginParams as ParamsBlobAnalysis;
                                        params极耳涂布内翻检测.ho_Image = InfoImgInfo.ho_image;
                                        params极耳涂布内翻检测.m_fBaseCol = point基准.Col;
                                        params极耳涂布内翻检测.m_fBaseRow = point基准.Row;
                                        // paramsBlobAnalysis.m_fBaseAngle = 0;
                                        params极耳涂布内翻检测.m_fCurrentCol = Cross粗定位工具PointInfo.point.Col;
                                        params极耳涂布内翻检测.m_fCurrentRow = Cross粗定位工具PointInfo.point.Row;
                                        // paramsBlobAnalysis.m_fCurrentAngle = 0;
                                        params极耳涂布内翻检测.m_bUsePositionFollow = true;
                                        if(params极耳涂布内翻检测.m_bIsEnable)
                                        {
                                            if (Plugin极耳涂布内翻检测.ExecuteObj())
                                            {
                                                RegionShow极耳涂布内翻检测.Add(new RegionShowInfo() { m_hRegion = params极耳涂布内翻检测.m_ho_OutLocalRoiRegion.CopyObj(1, -1), m_stuGetColor = HalconColor.orange });
                                                if (params极耳涂布内翻检测.m_lMaskRoiParams.Count > 0 && params极耳涂布内翻检测.m_ho_OutMaskRoiRegion != null)
                                                {
                                                    RegionShow极耳涂布内翻检测.Add(new RegionShowInfo() { m_hRegion = params极耳涂布内翻检测.m_ho_OutMaskRoiRegion.CopyObj(1, -1), m_stuGetColor = HalconColor.orange });
                                                }

                                                foreach (var item in params极耳涂布内翻检测.m_lOutRegionResults)
                                                {
                                                    try
                                                    {

                                                        HOperatorSet.SelectObj(params极耳涂布内翻检测.m_ho_OutSelectedRegion, out HObject temp, item.m_nIndex);
                                                        HOperatorSet.SmallestRectangle1(temp, out HTuple row1, out HTuple col1, out HTuple row2, out HTuple col2);
                                                        HOperatorSet.CropPart(InfoImgInfo.ho_image, out HObject imagePart, item.m_fCenterRow, item.m_fCenterCol, (col1 - col2).TupleAbs() > 68 ? (col1 - col2).TupleAbs() : (HTuple)68, (row1 - row2).TupleAbs() * 1.1 > 68 ? (row1 - row2).TupleAbs() * 1.1 : (HTuple)68);
                                                        NgDefectResult ngDefect = new NgDefectResult();
                                                        ngDefect.ClassName = "极耳涂布内翻检测";
                                                        ngDefect.Area = item.m_fArea * Calipix_x * Calipix_y;
                                                        ngDefect.HeightPx = (row1 - row2).TupleAbs();
                                                        ngDefect.WidthPx = (col1 - col2).TupleAbs();
                                                        ngDefect.Height = (row1 - row2).TupleAbs() * Calipix_y;
                                                        ngDefect.Width = (col1 - col2).TupleAbs() * Calipix_x;
                                                        ngDefect.PixelToMmH = Calipix_x;
                                                        ngDefect.PixelToMmV = Calipix_y;
                                                        ngDefect.Region = temp;
                                                        ngDefect.CropImage = imagePart;
                                                        ngDefect极耳涂布内翻检测.Add(ngDefect);

                                                    }
                                                    catch (Exception)
                                                    {
                                                        MessageNotify.AddStopAlarm(new StopAlarmMessage() { StopAlarmState = StopAlarmStateEnum.停机, name = $"流程{Plugin极耳涂布内翻检测.moduleInfo.ProcessID}&&{Plugin极耳涂布内翻检测.moduleInfo.ModuleName}", describe = $"结果__流程{Plugin极耳涂布内翻检测.moduleInfo.ProcessID}&&{Plugin极耳涂布内翻检测.moduleInfo.ModuleName}——出问题，需排查原因，不能开机，停机处理" });
                                                        HOperatorSet.CountSeconds(out Product尺寸.detectionTime.Image_EndTime);
                                                        HTuple end7 = (Product尺寸.detectionTime.Image_EndTime - Product尺寸.detectionTime.Image_StartTime) * 1000;
                                                        ShowHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前耗时：{end7.D.ToString("0.00")}" });
                                                        ProParam尺寸业务.ProductCallBack(Product尺寸);
                                                        return;
                                                        //throw;
                                                    }


                                                }
                                            }
                                            else
                                            {

                                                MessageNotify.AddStopAlarm(new StopAlarmMessage() { StopAlarmState = StopAlarmStateEnum.停机, name = $"流程{Plugin极耳涂布内翻检测.moduleInfo.ProcessID}&&{Plugin极耳涂布内翻检测.moduleInfo.ModuleName}", describe = $"流程{Plugin极耳涂布内翻检测.moduleInfo.ProcessID}&&{Plugin极耳涂布内翻检测.moduleInfo.ModuleName}——出问题，需排查原因，不能开机，停机处理" });
                                                HOperatorSet.CountSeconds(out Product尺寸.detectionTime.Image_EndTime);
                                                HTuple end8 = (Product尺寸.detectionTime.Image_EndTime - Product尺寸.detectionTime.Image_StartTime) * 1000;
                                                ShowHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前耗时：{end8.D.ToString("0.00")}" });

                                                ProParam尺寸业务.ProductCallBack(Product尺寸);
                                                return;
                                            }
                                        }


                                 
                                    }
                                    #endregion
                                    );

                    Product尺寸.ngDefectResults.AddRange(ngDefect左底部V角检测有无);
                    Product尺寸.ngDefectResults.AddRange(ngDefect右底部V角检测有无);
                    Product尺寸.ngDefectResults.AddRange(ngDefect极耳翻折检测);
                    Product尺寸.ngDefectResults.AddRange(ngDefect极耳余料检测);
                    Product尺寸.ngDefectResults.AddRange(ngDefect极耳涂布内翻检测);
                    Product尺寸.ngDefectResults.AddRange(ngDefect极片有无);

                    ShowHalconWindow.m_regionShowInfos.AddRange(RegionShow左底部V角检测有无);
                    ShowHalconWindow.m_regionShowInfos.AddRange(RegionShow右底部V角检测有无);
                    ShowHalconWindow.m_regionShowInfos.AddRange(RegionShow极耳翻折检测);
                    ShowHalconWindow.m_regionShowInfos.AddRange(RegionShow极耳余料检测);
                    ShowHalconWindow.m_regionShowInfos.AddRange(RegionShow极耳涂布内翻检测);
                    ShowHalconWindow.m_regionShowInfos.AddRange(RegionShow极片有无);
                    #endregion

                    #region 棋盘格坐标转换
                    IPlugin plugin棋盘格标定 = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == $"棋盘格标定");
                    ParamsCACRunCalibration params棋盘格标定 = plugin棋盘格标定.PluginParams as ParamsCACRunCalibration;
                    params棋盘格标定.ho_Image = InfoImgInfo.ho_image;
                    for (int i = 0; i < Product尺寸.LineInfosResult.Count; i++)
                    {
                        LineInfo lineInfo = Product尺寸.LineInfosResult[i].Copy();

                        ExecuteBoardCalibration(lineInfo.ColBegin, lineInfo.RowBegin,
                           plugin棋盘格标定, params棋盘格标定, out lineInfo.ColBegin, out lineInfo.RowBegin, out string errorMessage);

                        ExecuteBoardCalibration(lineInfo.ColEnd, lineInfo.RowEnd,
                           plugin棋盘格标定, params棋盘格标定, out lineInfo.ColEnd, out lineInfo.RowEnd, out errorMessage);
                        Product尺寸.LineInfosResult[i] = lineInfo;

                    }

                    for (int i = 0; i < Product尺寸.CrossPointInfos.Count; i++)
                    {
                        OutCrossGroup outCrossGroup = Product尺寸.CrossPointInfos[i].Copy();

                        ExecuteBoardCalibration(outCrossGroup.LineInfo1.ColBegin, outCrossGroup.LineInfo1.RowBegin,
                            plugin棋盘格标定, params棋盘格标定, out outCrossGroup.LineInfo1.ColBegin, out outCrossGroup.LineInfo1.RowBegin, out string errorMessage);
                        ExecuteBoardCalibration(outCrossGroup.LineInfo1.ColEnd, outCrossGroup.LineInfo1.RowEnd,
                        plugin棋盘格标定, params棋盘格标定, out outCrossGroup.LineInfo1.ColEnd, out outCrossGroup.LineInfo1.RowEnd, out errorMessage);
                        ExecuteBoardCalibration(outCrossGroup.LineInfo2.ColBegin, outCrossGroup.LineInfo2.RowBegin,
                                plugin棋盘格标定, params棋盘格标定, out outCrossGroup.LineInfo2.ColBegin, out outCrossGroup.LineInfo2.RowBegin, out errorMessage);
                        ExecuteBoardCalibration(outCrossGroup.LineInfo2.ColEnd, outCrossGroup.LineInfo2.RowEnd,
                               plugin棋盘格标定, params棋盘格标定, out outCrossGroup.LineInfo2.ColEnd, out outCrossGroup.LineInfo2.RowEnd, out errorMessage);
                        ExecuteBoardCalibration(outCrossGroup.point.Col, outCrossGroup.point.Row,
                              plugin棋盘格标定, params棋盘格标定, out outCrossGroup.point.Col, out outCrossGroup.point.Row, out errorMessage);
                        Product尺寸.CrossPointInfos[i] = outCrossGroup;
                    }

                    for (int i = 0; i < Product尺寸.PointResults.Count; i++)
                    {
                        PointInfo point = Product尺寸.PointResults[i].Copy();
                        ExecuteBoardCalibration(point.Col, point.Row,
                            plugin棋盘格标定, params棋盘格标定, out point.Col, out point.Row, out string errorMessage);

                        Product尺寸.PointResults[i] = point;
                    }

                    #endregion

                    //赋值
                    ShowHalconWindow.m_hImage = InfoImgInfo.ho_image;
                    ShowHalconWindow.m_nImageFrameID = InfoImgInfo.image_ID;
                    ShowHalconWindow.m_nProductID = InfoImgInfo.image_ID;

                    HOperatorSet.CountSeconds(out Product尺寸.detectionTime.Image_EndTime);
                    HTuple end = (Product尺寸.detectionTime.Image_EndTime - Product尺寸.detectionTime.Image_StartTime) * 1000;
                    ShowHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前耗时：{end.D.ToString("0.00")}" });
                    ProParam尺寸业务.ProductCallBack(Product尺寸);


                }

            }
            catch (Exception ee)
            {
                MessageNotify.AddErrorLog($"流程{ProcessID}ProProcess极片尺寸带极耳检测流程():" + ee.ToString());
                System.Windows.Forms.MessageBox.Show("ProProcess极片尺寸带极耳检测流程():" + ee.ToString(), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //throw;
            }
        }

        /// <summary>
        /// 不带极耳侧
        /// </summary>
        /// <param name="process"></param>
        public void ProProcess极片尺寸检测流程(Process process)
        {
            ///底部是第8个相机吧
            int ProcessID = process.processInfo.ProcessID;
            int camID = process.processInfo.CameraID;
            int CurCamSide = camID;
            int PoleMode = 0;
            int CID = process.processInfo.CameraID;
            camID = CID;
            try
            {


                //设置当前流程绑定核
                SetThreadAffinityMask(GetCurrentThread(), new UIntPtr(SetCpuID(ProcessID)));
                //标定系数
                double Calipix_y = Service_Camera.GetCam_CalibrationValue_Y(camID);
                double Calipix_x = Service_Camera.GetCam_CalibrationValue_X(camID);
                //工艺粗定位基准
                //初基准点/预留承接工艺

                PointInfo point基准 = new PointInfo()
                {
                    Col = (m_craft.Get_GlobalVariableItem(CID, "CamBaseCol")?.Value.ToDouble() ?? 0),
                    Row = (m_craft.Get_GlobalVariableItem(CID, "CamBaseRow")?.Value.ToDouble() ?? 0)
                };


                SizeDetectParam ProParam尺寸业务 = Project.Instance.Is_simulation ? m_DicCamOperaParam_Sim[CID] as SizeDetectParam : m_DicCamOperaParam[CID] as SizeDetectParam;
                var imageQueue = ProParam尺寸业务.alreadyDealGrabImage;
                ProParam尺寸业务.semaphoreSlimCam.Wait(45);
                if (imageQueue.Count > 0)
                {
                    ImageInfo InfoImgInfo = imageQueue.Dequeue();

                    SizeDectectProduct Product尺寸 = new SizeDectectProduct();
                    HOperatorSet.CountSeconds(out Product尺寸.detectionTime.Image_StartTime);
                    Product尺寸.showHalconGroup = new System.Collections.ObjectModel.ObservableCollection<ShowHalconWindow>();

                    Product尺寸.showHalconGroup.Add(new ShowHalconWindow()
                    {
                        WindowName = "原图",
                        m_nImageFrameID = InfoImgInfo.image_ID,
                        m_hImage = InfoImgInfo.ho_image,

                    });

                    ShowHalconWindow ShowHalconWindow = Product尺寸.showHalconGroup[0];
                    #region 交点模块执行
                    IPlugin PluginIntersection = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == $"粗定位工具");

                    bool success = ProcessCoarseLocation(InfoImgInfo, PluginIntersection, ProcessID, Product尺寸.CrossPointInfos, ShowHalconWindow, out OutCrossGroup Cross粗定位工具PointInfo, "粗定位工具");
                    if (!success)
                    {
                        Log4netRecord.ErrorLog.Debug($"流程{PluginIntersection.moduleInfo.ProcessID}的{PluginIntersection.moduleInfo.ModuleName}执行失败");
                    }

                    #endregion

                    #region 凸点测量检测模块执行
                    PointInfo pointV角凸点1 = new PointInfo() { nProcessID = ProcessID, ModuleName = "V角凸点1" };
                    PointInfo pointV角凸点2 = new PointInfo() { nProcessID = ProcessID, ModuleName = "V角凸点2" };
                    Parallel.Invoke
                        (
                    #region V角凸点1
                        () =>
                        {
                            IPlugin PluginV角凸点1 = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == $"V角凸点1");
                            ParamsReusableMeasureDetect ParamsV角凸点1 = PluginV角凸点1.PluginParams as ParamsReusableMeasureDetect;
                            ParamsV角凸点1.ho_Image = InfoImgInfo.ho_image;
                            ParamsV角凸点1.EnableJumpDetection = true;
                            ParamsV角凸点1.m_bUsePositionFollow = true;
                            ParamsV角凸点1.m_bCurrentPoseValid = true;
                            ParamsV角凸点1.m_fBaseCol = point基准.Col;
                            ParamsV角凸点1.m_fBaseRow = point基准.Row;
                            ParamsV角凸点1.m_fBaseAngle = 0;
                            ParamsV角凸点1.m_fCurrentCol = Cross粗定位工具PointInfo.point.Col;
                            ParamsV角凸点1.m_fCurrentRow = Cross粗定位工具PointInfo.point.Row;
                            //ParamsV角凸点1.m_fCurrentRow = 0;
                            if (PluginV角凸点1.ExecuteObj())
                            {
                                if (ParamsV角凸点1.EnableJumpDetection)
                                {
                                    pointV角凸点1.Col = ParamsV角凸点1.JumpOutput.IntersectionCol;
                                    pointV角凸点1.Row = ParamsV角凸点1.JumpOutput.IntersectionRow;
                                    pointV角凸点1.bIsSuccess = true;
                                }



                            }
                            else
                            {

                                Log4netRecord.ErrorLog.Debug($"流程{PluginV角凸点1.moduleInfo.ProcessID}&&{PluginV角凸点1.moduleInfo.ModuleName}---{ParamsV角凸点1.m_strRunMessage}");
                            }
                        },
                    #endregion

                    #region V角凸点2
                        () =>
                        {
                            IPlugin PluginV角凸点2 = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == $"V角凸点2");
                            ParamsReusableMeasureDetect ParamsV角凸点2 = PluginV角凸点2.PluginParams as ParamsReusableMeasureDetect;
                            ParamsV角凸点2.ho_Image = InfoImgInfo.ho_image;
                            ParamsV角凸点2.EnableJumpDetection = true;
                            ParamsV角凸点2.m_bUsePositionFollow = true;
                            ParamsV角凸点2.m_bCurrentPoseValid = true;
                            ParamsV角凸点2.m_fBaseCol = point基准.Col;
                            ParamsV角凸点2.m_fBaseRow = point基准.Row;
                            ParamsV角凸点2.m_fBaseAngle = 0;
                            ParamsV角凸点2.m_fCurrentCol = Cross粗定位工具PointInfo.point.Col;
                            ParamsV角凸点2.m_fCurrentRow = Cross粗定位工具PointInfo.point.Row;
                            //ParamsV角凸点2.m_fCurrentRow = 0;
                            if (PluginV角凸点2.ExecuteObj())
                            {
                                if (ParamsV角凸点2.EnableJumpDetection)
                                {
                                    pointV角凸点2.Col = ParamsV角凸点2.JumpOutput.IntersectionCol;
                                    pointV角凸点2.Row = ParamsV角凸点2.JumpOutput.IntersectionRow;
                                    pointV角凸点2.bIsSuccess = true;
                                }



                            }
                            else
                            {

                                Log4netRecord.ErrorLog.Debug($"流程{PluginV角凸点2.moduleInfo.ProcessID}&&{PluginV角凸点2.moduleInfo.ModuleName}---{ParamsV角凸点2.m_strRunMessage}");
                            }
                        }
                        #endregion
                        )
                      ;

                    Product尺寸.PointResults.Add(pointV角凸点1);
                    Product尺寸.PointResults.Add(pointV角凸点2);
                    HOperatorSet.GenCrossContourXld(out HObject cross, pointV角凸点1.Row, pointV角凸点1.Col, 10, 45);
                    ShowHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = cross, m_stuGetColor = HalconColor.gold });
                    HOperatorSet.GenCrossContourXld(out cross, pointV角凸点2.Row, pointV角凸点2.Col, 10, 45);
                    ShowHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = cross, m_stuGetColor = HalconColor.gold });

                    #endregion

                    #region Blob模块执行
                    ObservableCollection<NgDefectResult> ngDefect极片有无 = new ObservableCollection<NgDefectResult>();
                    ObservableCollection<NgDefectResult> ngDefect顶部V角检测有无 = new ObservableCollection<NgDefectResult>();
                    List<RegionShowInfo> RegionShow极片有无 = new List<RegionShowInfo>();
                    List<RegionShowInfo> RegionShow顶部V角检测有无 = new List<RegionShowInfo>();
                    Parallel.Invoke(

                    #region 极片有无
                        () =>
                        {
                            IPlugin Plugin极片有无 = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == $"极片有无");
                            ParamsBlobAnalysis params极片有无 = Plugin极片有无.PluginParams as ParamsBlobAnalysis;
                            params极片有无.ho_Image = InfoImgInfo.ho_image;
                            params极片有无.m_fBaseCol = point基准.Col;
                            params极片有无.m_fBaseRow = point基准.Row;
                            // paramsBlobAnalysis.m_fBaseAngle = 0;
                            params极片有无.m_fCurrentCol = Cross粗定位工具PointInfo.point.Col;
                            params极片有无.m_fCurrentRow = Cross粗定位工具PointInfo.point.Row;
                            // paramsBlobAnalysis.m_fCurrentAngle = 0;
                            params极片有无.m_bUsePositionFollow = true;
                            if(params极片有无.m_bIsEnable)
                            {
                                if (Plugin极片有无.ExecuteObj())
                                {
                                    RegionShow极片有无.Add(new RegionShowInfo() { m_hRegion = params极片有无.m_ho_OutLocalRoiRegion.CopyObj(1, -1), m_stuGetColor = HalconColor.orange });
                                    if (params极片有无.m_lMaskRoiParams.Count > 0 && params极片有无.m_ho_OutMaskRoiRegion != null)
                                    {
                                        RegionShow极片有无.Add(new RegionShowInfo() { m_hRegion = params极片有无.m_ho_OutMaskRoiRegion.CopyObj(1, -1), m_stuGetColor = HalconColor.orange });
                                    }


                                    foreach (var item in params极片有无.m_lOutRegionResults)
                                    {
                                        try
                                        {

                                            HOperatorSet.SelectObj(params极片有无.m_ho_OutSelectedRegion, out HObject temp, item.m_nIndex);
                                            HOperatorSet.SmallestRectangle1(temp, out HTuple row1, out HTuple col1, out HTuple row2, out HTuple col2);
                                            HOperatorSet.CropPart(InfoImgInfo.ho_image, out HObject imagePart, item.m_fCenterRow, item.m_fCenterCol, (col1 - col2).TupleAbs() > 68 ? (col1 - col2).TupleAbs() : (HTuple)68, (row1 - row2).TupleAbs() * 1.1 > 68 ? (row1 - row2).TupleAbs() * 1.1 : (HTuple)68);
                                            NgDefectResult ngDefect = new NgDefectResult();
                                            ngDefect.ClassName = "极片有无";
                                            ngDefect.Area = item.m_fArea * Calipix_x * Calipix_y;
                                            ngDefect.HeightPx = (row1 - row2).TupleAbs();
                                            ngDefect.WidthPx = (col1 - col2).TupleAbs();
                                            ngDefect.Height = (row1 - row2).TupleAbs() * Calipix_y;
                                            ngDefect.Width = (col1 - col2).TupleAbs() * Calipix_x;
                                            ngDefect.PixelToMmH = Calipix_x;
                                            ngDefect.PixelToMmV = Calipix_y;
                                            ngDefect.Region = temp;
                                            ngDefect.CropImage = imagePart;
                                            ngDefect极片有无.Add(ngDefect);

                                        }
                                        catch (Exception)
                                        {
                                            MessageNotify.AddStopAlarm(new StopAlarmMessage() { StopAlarmState = StopAlarmStateEnum.停机, name = $"流程{Plugin极片有无.moduleInfo.ProcessID}&&{Plugin极片有无.moduleInfo.ModuleName}", describe = $"结果__流程{Plugin极片有无.moduleInfo.ProcessID}&&{Plugin极片有无.moduleInfo.ModuleName}——出问题，需排查原因，不能开机，停机处理" });

                                            HOperatorSet.CountSeconds(out Product尺寸.detectionTime.Image_EndTime);
                                            ProParam尺寸业务.ProductCallBack(Product尺寸);
                                            return;
                                            //throw;
                                        }


                                    }
                                }
                                else
                                {

                                    MessageNotify.AddStopAlarm(new StopAlarmMessage() { StopAlarmState = StopAlarmStateEnum.停机, name = $"流程{Plugin极片有无.moduleInfo.ProcessID}&&{Plugin极片有无.moduleInfo.ModuleName}", describe = $"流程{Plugin极片有无.moduleInfo.ProcessID}&&{Plugin极片有无.moduleInfo.ModuleName}——出问题，需排查原因，不能开机，停机处理" });

                                    HOperatorSet.CountSeconds(out Product尺寸.detectionTime.Image_EndTime);
                                    ProParam尺寸业务.ProductCallBack(Product尺寸);
                                    return;
                                }
                            }
                          
                        },
                    #endregion

                    #region 顶部V角检测有无
                    () =>
                    {
                        IPlugin Plugin顶部V角检测有无 = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == $"顶部V角检测有无");
                        ParamsBlobAnalysis params顶部V角检测有无 = Plugin顶部V角检测有无.PluginParams as ParamsBlobAnalysis;
                        params顶部V角检测有无.ho_Image = InfoImgInfo.ho_image;
                        params顶部V角检测有无.m_fBaseCol = point基准.Col;
                        params顶部V角检测有无.m_fBaseRow = point基准.Row;
                        // paramsBlobAnalysis.m_fBaseAngle = 0;
                        params顶部V角检测有无.m_fCurrentCol = Cross粗定位工具PointInfo.point.Col;
                        params顶部V角检测有无.m_fCurrentRow = Cross粗定位工具PointInfo.point.Row;
                        // paramsBlobAnalysis.m_fCurrentAngle = 0;
                        params顶部V角检测有无.m_bUsePositionFollow = true;
                        if(params顶部V角检测有无.m_bIsEnable)
                        {
                            if (Plugin顶部V角检测有无.ExecuteObj())
                            {
                                RegionShow顶部V角检测有无.Add(new RegionShowInfo() { m_hRegion = params顶部V角检测有无.m_ho_OutLocalRoiRegion.CopyObj(1, -1), m_stuGetColor = HalconColor.orange });
                                if (params顶部V角检测有无.m_lMaskRoiParams.Count > 0 && params顶部V角检测有无.m_ho_OutMaskRoiRegion != null)
                                {
                                    RegionShow顶部V角检测有无.Add(new RegionShowInfo() { m_hRegion = params顶部V角检测有无.m_ho_OutMaskRoiRegion.CopyObj(1, -1), m_stuGetColor = HalconColor.orange });
                                }

                                foreach (var item in params顶部V角检测有无.m_lOutRegionResults)
                                {
                                    try
                                    {

                                        HOperatorSet.SelectObj(params顶部V角检测有无.m_ho_OutSelectedRegion, out HObject temp, item.m_nIndex);
                                        HOperatorSet.SmallestRectangle1(temp, out HTuple row1, out HTuple col1, out HTuple row2, out HTuple col2);
                                        HOperatorSet.CropPart(InfoImgInfo.ho_image, out HObject imagePart, item.m_fCenterRow, item.m_fCenterCol, (col1 - col2).TupleAbs() > 68 ? (col1 - col2).TupleAbs() : (HTuple)68, (row1 - row2).TupleAbs() * 1.1 > 68 ? (row1 - row2).TupleAbs() * 1.1 : (HTuple)68);
                                        NgDefectResult ngDefect = new NgDefectResult();
                                        ngDefect.ClassName = "V角检测有无";
                                        ngDefect.Area = item.m_fArea * Calipix_x * Calipix_y;
                                        ngDefect.HeightPx = (row1 - row2).TupleAbs();
                                        ngDefect.WidthPx = (col1 - col2).TupleAbs();
                                        ngDefect.Height = (row1 - row2).TupleAbs() * Calipix_y;
                                        ngDefect.Width = (col1 - col2).TupleAbs() * Calipix_x;
                                        ngDefect.PixelToMmH = Calipix_x;
                                        ngDefect.PixelToMmV = Calipix_y;
                                        ngDefect.Region = temp;
                                        ngDefect.CropImage = imagePart;
                                        ngDefect顶部V角检测有无.Add(ngDefect);

                                    }
                                    catch (Exception)
                                    {
                                        MessageNotify.AddStopAlarm(new StopAlarmMessage() { StopAlarmState = StopAlarmStateEnum.停机, name = $"流程{Plugin顶部V角检测有无.moduleInfo.ProcessID}&&{Plugin顶部V角检测有无.moduleInfo.ModuleName}", describe = $"结果__流程{Plugin顶部V角检测有无.moduleInfo.ProcessID}&&{Plugin顶部V角检测有无.moduleInfo.ModuleName}——出问题，需排查原因，不能开机，停机处理" });

                                        HOperatorSet.CountSeconds(out Product尺寸.detectionTime.Image_EndTime);
                                        ProParam尺寸业务.ProductCallBack(Product尺寸);
                                        return;
                                        //throw;
                                    }


                                }
                            }
                            else
                            {

                                MessageNotify.AddStopAlarm(new StopAlarmMessage() { StopAlarmState = StopAlarmStateEnum.停机, name = $"流程{Plugin顶部V角检测有无.moduleInfo.ProcessID}&&{Plugin顶部V角检测有无.moduleInfo.ModuleName}", describe = $"流程{Plugin顶部V角检测有无.moduleInfo.ProcessID}&&{Plugin顶部V角检测有无.moduleInfo.ModuleName}——出问题，需排查原因，不能开机，停机处理" });

                                HOperatorSet.CountSeconds(out Product尺寸.detectionTime.Image_EndTime);
                                ProParam尺寸业务.ProductCallBack(Product尺寸);
                                return;
                            }
                        }
                      
                        ShowHalconWindow.m_regionShowInfos.AddRange(RegionShow极片有无);
                        ShowHalconWindow.m_regionShowInfos.AddRange(RegionShow顶部V角检测有无);
                    }



                    #endregion

                      );
                    #endregion

                    Product尺寸.ngDefectResults.AddRange(ngDefect极片有无);
                    Product尺寸.ngDefectResults.AddRange(ngDefect顶部V角检测有无);
                    ShowHalconWindow.m_regionShowInfos.AddRange(RegionShow极片有无);
                    ShowHalconWindow.m_regionShowInfos.AddRange(RegionShow顶部V角检测有无);

                    #region 棋盘格坐标转换
                    IPlugin plugin棋盘格标定 = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == $"棋盘格标定");
                    ParamsCACRunCalibration params棋盘格标定 = plugin棋盘格标定.PluginParams as ParamsCACRunCalibration;
                    params棋盘格标定.ho_Image = InfoImgInfo.ho_image;

                    for (int i = 0; i < Product尺寸.LineInfosResult.Count; i++)
                    {
                        LineInfo lineInfo = Product尺寸.LineInfosResult[i].Copy();

                        ExecuteBoardCalibration(lineInfo.ColBegin, lineInfo.RowBegin,
                           plugin棋盘格标定, params棋盘格标定, out lineInfo.ColBegin, out lineInfo.RowBegin, out string errorMessage);

                        ExecuteBoardCalibration(lineInfo.ColEnd, lineInfo.RowEnd,
                           plugin棋盘格标定, params棋盘格标定, out lineInfo.ColEnd, out lineInfo.RowEnd, out errorMessage);
                        Product尺寸.LineInfosResult[i] = lineInfo;
                    }

                    for (int i = 0; i < Product尺寸.CrossPointInfos.Count; i++)
                    {
                        OutCrossGroup outCrossGroup = Product尺寸.CrossPointInfos[i].Copy();
                        ExecuteBoardCalibration(outCrossGroup.LineInfo1.ColBegin, outCrossGroup.LineInfo1.RowBegin,
                            plugin棋盘格标定, params棋盘格标定, out outCrossGroup.LineInfo1.ColBegin, out outCrossGroup.LineInfo1.RowBegin, out string errorMessage);
                        ExecuteBoardCalibration(outCrossGroup.LineInfo1.ColEnd, outCrossGroup.LineInfo1.RowEnd,
                        plugin棋盘格标定, params棋盘格标定, out outCrossGroup.LineInfo1.ColEnd, out outCrossGroup.LineInfo1.RowEnd, out errorMessage);
                        ExecuteBoardCalibration(outCrossGroup.LineInfo2.ColBegin, outCrossGroup.LineInfo2.RowBegin,
                                plugin棋盘格标定, params棋盘格标定, out outCrossGroup.LineInfo2.ColBegin, out outCrossGroup.LineInfo2.RowBegin, out errorMessage);
                        ExecuteBoardCalibration(outCrossGroup.LineInfo2.ColEnd, outCrossGroup.LineInfo2.RowEnd,
                               plugin棋盘格标定, params棋盘格标定, out outCrossGroup.LineInfo2.ColEnd, out outCrossGroup.LineInfo2.RowEnd, out errorMessage);
                        ExecuteBoardCalibration(outCrossGroup.point.Col, outCrossGroup.point.Row,
                              plugin棋盘格标定, params棋盘格标定, out outCrossGroup.point.Col, out outCrossGroup.point.Row, out errorMessage);
                        Product尺寸.CrossPointInfos[i] = outCrossGroup;
                    }

                    for (int i = 0; i < Product尺寸.PointResults.Count; i++)
                    {
                        PointInfo point = Product尺寸.PointResults[i].Copy();
                        ExecuteBoardCalibration(point.Col, point.Row,
                            plugin棋盘格标定, params棋盘格标定, out point.Col, out point.Row, out string errorMessage);
                        Product尺寸.PointResults[i] = point;

                    }

                    #endregion

                    //赋值
                    ShowHalconWindow.m_hImage = InfoImgInfo.ho_image;
                    ShowHalconWindow.m_nImageFrameID = InfoImgInfo.image_ID;
                    ShowHalconWindow.m_nProductID = InfoImgInfo.image_ID;
                    HOperatorSet.CountSeconds(out Product尺寸.detectionTime.Image_EndTime);
                    HTuple end = (Product尺寸.detectionTime.Image_EndTime - Product尺寸.detectionTime.Image_StartTime) * 1000;
                    ShowHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前耗时：{end.D.ToString("0.00")}" });
                    ProParam尺寸业务.ProductCallBack(Product尺寸);
                }
            }
            catch (Exception ee)
            {
                MessageNotify.AddErrorLog($"流程{ProcessID}ProProcess极片尺寸检测流程():" + ee.ToString());
                System.Windows.Forms.MessageBox.Show("ProProcess极片尺寸检测流程():" + ee.ToString(), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // throw;
            }
        }

        /// <summary>
        /// 纠偏相机
        /// </summary>
        /// <param name="process"></param>
        public void ProProcess纠偏检测流程(Process process)
        {
            //第三个相机
            int ProcessID = process.processInfo.ProcessID;
            int camID = (int)CameraEnum.Cam3;

            try
            {

                //设置当前流程绑定核
                SetThreadAffinityMask(GetCurrentThread(), new UIntPtr(SetCpuID(ProcessID)));
                //标定系数
                double Calipix_y = Service_Camera.GetCam_CalibrationValue_Y(camID);
                double Calipix_x = Service_Camera.GetCam_CalibrationValue_X(camID);
                double 基准点X = m_craft.Get_GlobalVariableItem(camID, "AlignCamBaseCol").Value.ToDouble();
                double 基准点Y = m_craft.Get_GlobalVariableItem(camID, "AlignCamBaseRow").Value.ToDouble();
                CutCorrectParam ProParam纠偏业务 = Project.Instance.Is_simulation ? m_DicCamOperaParam_Sim[camID] as CutCorrectParam : m_DicCamOperaParam[camID] as CutCorrectParam;

                //默认基准点
                PointInfo point基准 = new PointInfo()
                {
                    Col = (m_craft.Get_GlobalVariableItem(camID, "CamBaseCol")?.Value.ToDouble() ?? 0),
                    Row = (m_craft.Get_GlobalVariableItem(camID, "CamBaseRow")?.Value.ToDouble() ?? 0)
                };
                var imageQueue = ProParam纠偏业务.alreadyDealGrabImage;
                ProParam纠偏业务.semaphoreSlimCam.Wait(45);
                if (imageQueue.Count > 0)
                {
                    ImageInfo InfoImgInfo = imageQueue.Dequeue();
                    PosCorrectProduct Product纠偏 = new PosCorrectProduct();
                    HOperatorSet.CountSeconds(out Product纠偏.detectionTime.Image_StartTime);

                    Product纠偏.showHalconGroup = new System.Collections.ObjectModel.ObservableCollection<ShowHalconWindow>();

                    Product纠偏.showHalconGroup.Add(new ShowHalconWindow()
                    {
                        WindowName = "原图",
                        m_nImageFrameID = InfoImgInfo.image_ID,
                        m_hImage = InfoImgInfo.ho_image,

                    });

                    ShowHalconWindow ShowHalconWindow = Product纠偏.showHalconGroup[0];
                    ShowHalconWindow.m_lineShowInfos.Add(new LineInfo() {ColBegin= 基准点X ,ColEnd= 基准点X ,RowBegin=0,RowEnd=InfoImgInfo.imageHeight});
                    #region 抓边模块执行

                    IPlugin Plugin基准定位边 = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == $"基准定位边");
                    ParamsMeasureLine params基准定位边 = Plugin基准定位边.PluginParams as ParamsMeasureLine;
                    params基准定位边.ho_Image = InfoImgInfo.ho_image;
                    LineInfo Line基准定位边 = new LineInfo() { StrLineName = "基准定位边", strMoudleName = "基准定位边", nProcessID = ProcessID };
                    DetectEdgeValueAssign(params基准定位边, Plugin基准定位边.ExecuteObj(), Line基准定位边, ShowHalconWindow);
                    Product纠偏.LineInfosResult.Add(Line基准定位边);

                    if (!Line基准定位边.IsLineSuccessfull)
                    {
                        if (ProParam纠偏业务.LastLineInfo.RowBegin >= 0)
                        {
                            Line基准定位边 = ProParam纠偏业务.LastLineInfo;
                        }
                        else
                        {
                            Line基准定位边.RowBegin = point基准.Row;
                            Line基准定位边.RowEnd = point基准.Row;
                            Line基准定位边.ColBegin = 0;
                            Line基准定位边.ColEnd = InfoImgInfo.imageWidth;
                            Line基准定位边.IsLineSuccessfull = true;
                        }

                        ShowHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = "基准定位边检测失败，自动设置为基准点", m_stuGetColor = HalconColor.red });
                    }
                    else
                    {
                        ProParam纠偏业务.LastLineInfo = Line基准定位边.Copy();
                    }

                    #endregion

                    #region 组合交点模块
                    IPlugin Plugin基准定位交点 = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == $"基准定位交点");
                    ParamsCombPoint params基准定位交点 = Plugin基准定位交点.PluginParams as ParamsCombPoint;
                    params基准定位交点.ho_Image = InfoImgInfo.ho_image;
                    params基准定位交点.LineInfo1 = Line基准定位边;
                    params基准定位交点.LineInfo2 = new LineInfo() { ColBegin = 0, ColEnd = 0, RowBegin = 1, RowEnd = InfoImgInfo.imageHeight, IsLineSuccessfull = true };

                    PointInfo point基准定位交点 = new PointInfo();

                    if (Plugin基准定位交点.ExecuteObj())
                    {
                        point基准定位交点.Col = params基准定位交点.hv_IntersectionColumn;
                        point基准定位交点.Row = params基准定位交点.hv_IntersectionRow;
                        point基准定位交点.ModuleName = "基准定位交点";
                        point基准定位交点.nProcessID = ProcessID;
                        point基准定位交点.bIsSuccess = true;
                        HOperatorSet.GenCrossContourXld(out HObject cross, point基准定位交点.Row, point基准定位交点.Col, 10, 45);
                        ShowHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = cross, m_stuGetColor = HalconColor.gold });

                    }
                    else
                    {
                        Log4netRecord.ErrorLog.Debug($"流程{Plugin基准定位交点.moduleInfo.ProcessID}&&{Plugin基准定位交点.moduleInfo.ModuleName}---{params基准定位交点.m_strRunMessage}");
                    }

                    Product纠偏.PointResults.Add(point基准定位交点);
                    #endregion

                    #region 模板匹配
                    IPlugin Plugin极耳Mark匹配 = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == $"极耳&&圆角匹配");
                    ParamsTemplateMatch params极耳Mark匹配 = Plugin极耳Mark匹配.PluginParams as ParamsTemplateMatch;
                    params极耳Mark匹配.ho_Image = InfoImgInfo.ho_image;
                    //这里的位置跟随默认启用//模块需定义好模块基准位置
                    params极耳Mark匹配.m_bUsePositionFollow = true;
                    params极耳Mark匹配.m_fFollowBaseCol = point基准.Col;
                    params极耳Mark匹配.m_fFollowBaseRow = point基准.Row;

                    params极耳Mark匹配.m_fFollowCurrentRow = point基准.Row;
                    params极耳Mark匹配.m_fFollowCurrentCol = point基准定位交点.Col;

                    OutCrossGroup Cross左极耳根部定位 = new OutCrossGroup();
                    OutCrossGroup Cross右极耳根部定位 = new OutCrossGroup();

                    if (Plugin极耳Mark匹配.ExecuteObj())
                    {
                        ShowHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = params极耳Mark匹配.ho_OutMatchRoiRegion.CopyObj(1, -1), m_stuGetColor = HalconColor.orange });
                        if (params极耳Mark匹配.m_lMaskRoiParams.Count > 0 && params极耳Mark匹配.ho_OutMatchRoiRegion != null)
                        {
                            ShowHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = params极耳Mark匹配.ho_OutMaskRoiRegion.CopyObj(1, -1), m_stuGetColor = HalconColor.orange });
                        }

                        //选择分数最好的那个
                        var bestResult = params极耳Mark匹配.Results.OrderByDescending(r => r.Score).FirstOrDefault();
                        ShowHalconWindow showHalcon左极耳根部定位 = new ShowHalconWindow();
                        ShowHalconWindow showHalcon右极耳根部定位 = new ShowHalconWindow();
                        Parallel.Invoke(

                        #region 左极耳根部定位
                            () =>
                            {

                                IPlugin Plugin左极耳根部定位 = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == $"左极耳根部定位");

                                PointInfo point左极耳根部定位跟随 = new PointInfo() { Col = point基准.Col + bestResult.OffsetX, Row = point基准.Row+ bestResult.OffsetY, bIsSuccess = true };
                                bool success = ProcessCoarseLocation(InfoImgInfo, Plugin左极耳根部定位, ProcessID, showHalcon左极耳根部定位, out Cross左极耳根部定位, point基准, point左极耳根部定位跟随, "极耳左侧交点", true, true);
                                if (!success)
                                {
                                    Log4netRecord.ErrorLog.Debug($"流程{Plugin左极耳根部定位.moduleInfo.ProcessID}的{Plugin左极耳根部定位.moduleInfo.ModuleName}执行失败");
                                }
                            },
                        #endregion

                        #region 右极耳根部定位
                             () =>
                             {

                                 IPlugin Plugin右极耳根部定位 = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == $"右极耳根部定位");

                                 PointInfo point右极耳根部定位跟随 = new PointInfo() { Col = point基准.Col + bestResult.OffsetX, Row = point基准.Row + bestResult.OffsetY, bIsSuccess = true };
                                 bool success = ProcessCoarseLocation(InfoImgInfo, Plugin右极耳根部定位, ProcessID, showHalcon右极耳根部定位, out Cross右极耳根部定位, point基准, point右极耳根部定位跟随, "极耳左侧交点", true, true);
                                 if (!success)
                                 {
                                     Log4netRecord.ErrorLog.Debug($"流程{Plugin右极耳根部定位.moduleInfo.ProcessID}的{Plugin右极耳根部定位.moduleInfo.ModuleName}执行失败");
                                 }
                             }
                             #endregion

                              );

                        ShowHalconWindow.m_lineShowInfos.AddRange(showHalcon左极耳根部定位.m_lineShowInfos);
                        ShowHalconWindow.m_regionShowInfos.AddRange(showHalcon左极耳根部定位.m_regionShowInfos);
                        ShowHalconWindow.m_lineShowInfos.AddRange(showHalcon右极耳根部定位.m_lineShowInfos);
                        ShowHalconWindow.m_regionShowInfos.AddRange(showHalcon右极耳根部定位.m_regionShowInfos);
                    }
                    else
                    {
                        NgDefectResult ngDefectResult = new NgDefectResult()
                        {
                            ClassName = "无极耳",
                            Area = -999,
                            HeightPx = -999,
                            WidthPx = -999,
                            Height = -999,
                            Width = -999,
                            PixelToMmH = -999,
                            PixelToMmV = -999,
                        };

                        Product纠偏.ngDefectResults.Add(ngDefectResult);


                        Cross左极耳根部定位.nProcessID = ProcessID;
                        Cross左极耳根部定位.ModuleName = "左极耳根部定位";
                        Cross左极耳根部定位.LineInfo1 = new LineInfo();
                        Cross左极耳根部定位.LineInfo2 = new LineInfo();
                        Cross左极耳根部定位.point = new PointInfo();



                        Cross右极耳根部定位.nProcessID = ProcessID;
                        Cross右极耳根部定位.ModuleName = "右极耳根部定位";
                        Cross右极耳根部定位.LineInfo1 = new LineInfo();
                        Cross右极耳根部定位.LineInfo2 = new LineInfo();
                        Cross右极耳根部定位.point = new PointInfo();



                    }

                    PointInfo point中点 = new PointInfo() { nProcessID = ProcessID, ModuleName = "中点" };

                    if (Cross左极耳根部定位.bIsSuccess && Cross右极耳根部定位.bIsSuccess)
                    {
                        point中点.bIsSuccess = true;
                        point中点.Col = (Cross左极耳根部定位.point.Col + Cross右极耳根部定位.point.Col) / 2;
                        point中点.Row = (Cross左极耳根部定位.point.Row + Cross右极耳根部定位.point.Row) / 2;
                        LineInfo lineInfo = new LineInfo()
                        {
                            IsLineSuccessfull = true,
                            RowBegin = Cross左极耳根部定位.point.Row,
                            RowEnd = Cross右极耳根部定位.point.Row,
                            ColBegin = Cross左极耳根部定位.point.Col,
                            ColEnd = Cross右极耳根部定位.point.Col
                        };

                        HOperatorSet.GenCrossContourXld(out HObject cross, point中点.Row, point中点.Col, 10, 45);

                        ShowHalconWindow.m_regionShowInfos.Add(new RegionShowInfo()
                        {
                            m_hRegion = cross,
                            m_stuGetColor = HalconColor.gold
                        });

                        ShowHalconWindow.m_lineShowInfos.Add(lineInfo);
                    }
                    else
                    {
                        NgDefectResult ngDefectResult = new NgDefectResult()
                        {
                            ClassName = "无极耳",
                            Area = -999,
                            HeightPx = -999,
                            WidthPx = -999,
                            Height = -999,
                            Width = -999,
                            PixelToMmH = -999,
                            PixelToMmV = -999,
                        };

                        Product纠偏.ngDefectResults.Add(ngDefectResult);
                        ShowHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"左根部定位_{Cross左极耳根部定位.bIsSuccess}&&右根部定位_{Cross右极耳根部定位.bIsSuccess}", m_stuGetColor = HalconColor.red });
                    }



                    Product纠偏.CrossPointInfos.Add(Cross左极耳根部定位);
                    Product纠偏.CrossPointInfos.Add(Cross右极耳根部定位);
                    Product纠偏.PointResults.Add(point中点);

                    //赋值
                    ShowHalconWindow.m_hImage = InfoImgInfo.ho_image;
                    ShowHalconWindow.m_nImageFrameID = InfoImgInfo.image_ID;
                    ShowHalconWindow.m_nProductID = InfoImgInfo.image_ID;

                    HOperatorSet.CountSeconds(out Product纠偏.detectionTime.Image_EndTime);
                    HTuple end = (Product纠偏.detectionTime.Image_EndTime - Product纠偏.detectionTime.Image_StartTime) * 1000;
                    ShowHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"当前耗时：{end.D.ToString("0.00")}" });

                    ProParam纠偏业务.ProductCallBack(Product纠偏);
                    #endregion
                }
            }

            catch (Exception ee)
            {
                MessageNotify.AddErrorLog($"流程{ProcessID}ProProcess纠偏检测流程():" + ee.ToString());
                System.Windows.Forms.MessageBox.Show("ProProcess纠偏检测流程():" + ee.ToString(), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //throw;
            }
        }


    }
}

using HalconDotNet;

using LogTool;

using NPOI.SS.Formula.Functions;
using NPOI.SS.Formula.PTG;
using NPOI.SS.Util;

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;

using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace Frame_Core
{
    public class PFTCleanProDevice : BaseProDevice
    {

        private PFTCleanCraft m_craft { get; set; } = new PFTCleanCraft();

        private LimitThreadPool m_threadPool = new LimitThreadPool(10);

        //public Action m_eventProDeviceRunFinish { get; set; } = null;


        /// <summary>
        /// 换卷产品绝对ID
        /// </summary>
        private long m_nProductID_Roll = 0;

        /// <summary>
        /// 换卷图像帧号
        /// </summary>
        private long m_nImageID_Roll = 0;

        public PFTCleanProDevice()
        {
            m_nCamCount = 2;
            m_nProcessCount = 4;
            for (int i = 0; i < m_nCamCount; i++)
            {
                m_DicCamOperaParam[i] = new PFTCleanProParams(((CameraEnum)i).ToString());
                m_DicCamOperaParam_Sim[i] = new PFTCleanProParams(((CameraEnum)i).ToString());
            }
            Project.Instance.BindCamImageSource(m_DicCamOperaParam);

            HOperatorSet.SetSystem("clip_region", "false");
            INIPro();
            m_craft.CamNum = m_nCamCount;
            BuildProcessServiceList();
        }
        public override void INIPro()
        {
            m_craft = (PFTCleanCraft)Project.Instance.craftBase;
        }
        public override bool INIProduct()
        {
            try
            {
                foreach (var item in m_DicCamOperaParam)
                {
                    item.Value?.Dispose();
                }
                for (int i = 0; i < m_nCamCount; i++)
                {
                    double Calipix_y = Project.Instance.cameraManager[(CameraEnum)i].m_CamParam.CalibrationValue_Y;
                    double Calipix_x = Project.Instance.cameraManager[(CameraEnum)i].m_CamParam.CalibrationValue_X;
                    double ABMis = Project.Instance.cameraManager[(CameraEnum)i].m_CamParam.ToMainCamDistance / Calipix_y;

                    double imageHeight = Project.Instance.cameraManager[(CameraEnum)i].m_CamParam.ImageHeight;
                    //扩充EA产品-cam0
                    // PFTCleanProParams proms = new PFTCleanProParams(((CameraEnum)index).ToString());
                    PFTCleanProParams proms = (PFTCleanProParams)m_DicCamOperaParam[i];

                    proms.dic_EAproductsInfo[0] = ExPandHeadLinedic(i, 0, 1, ABMis, imageHeight, Calipix_y, Calipix_x);//new一个新产品信息
                                                                                                                       // m_DicCamOperaParam[index] = proms;
                }
                InitProduct_Sim();
                Project.Instance.BindCamImageSource(m_DicCamOperaParam);
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
                for (int i = 0; i < m_nCamCount; i++)
                {
                    double Calipix_y = Project.Instance.cameraManager[(CameraEnum)i].m_CamParam.CalibrationValue_Y;
                    double Calipix_x = Project.Instance.cameraManager[(CameraEnum)i].m_CamParam.CalibrationValue_X;
                    double ABMis = Project.Instance.cameraManager[(CameraEnum)i].m_CamParam.ToMainCamDistance / Calipix_y;
                    double imageHeight = Project.Instance.cameraManager[(CameraEnum)i].m_CamParam.ImageHeight;

                    //PFTCleanProParams promsim = new PFTCleanProParams(((CameraEnum)index).ToString());
                    PFTCleanProParams promsim = (PFTCleanProParams)m_DicCamOperaParam_Sim[i];
                    promsim.dic_EAproductsInfo[0] = ExPandHeadLinedic(i, 0, 1, ABMis, imageHeight, Calipix_y, Calipix_x);//new一个新产品信息
                                                                                                                         // m_DicCamOperaParam_Sim[index] = promsim;
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

            int ID = Project.Instance.CreateProcess();
            ProcessServices TempProcessBase = Project.Instance.GetProcessServiceById(ID);
            TempProcessBase.process.processInfo.ProcessName = Project.Instance.cameraManager[CameraEnum.Cam1].m_CamParam.CameraName;
            TempProcessBase.m_eventINIProcess += IniBaseCam;
            TempProcessBase.m_eventRUNProcess += ProProcessBaseCam;
            TempProcessBase.InitProcess();
            TempProcessBase.OpeningThread();

            ID = Project.Instance.CreateProcess();
            TempProcessBase = Project.Instance.GetProcessServiceById(ID);
            TempProcessBase.process.processInfo.ProcessName = "A面EA流程";
            TempProcessBase.m_eventINIProcess += IniBaseEA;
            TempProcessBase.m_eventRUNProcess += ProProcessBaseEA;
            TempProcessBase.InitProcess();
            TempProcessBase.OpeningThread();

            ID = Project.Instance.CreateProcess();
            TempProcessBase = Project.Instance.GetProcessServiceById(ID);
            TempProcessBase.process.processInfo.ProcessName = Project.Instance.cameraManager[CameraEnum.Cam2].m_CamParam.CameraName;
            TempProcessBase.m_eventINIProcess += IniFromCam;
            TempProcessBase.m_eventRUNProcess += ProProcessFromCam;
            TempProcessBase.InitProcess();
            TempProcessBase.OpeningThread();

            ID = Project.Instance.CreateProcess();
            TempProcessBase = Project.Instance.GetProcessServiceById(ID);
            TempProcessBase.process.processInfo.ProcessName = "B面EA流程";
            TempProcessBase.m_eventINIProcess += IniFromEA;
            TempProcessBase.m_eventRUNProcess += ProProcessFromEA;
            TempProcessBase.InitProcess();
            TempProcessBase.OpeningThread();


            //ID = Project.Instance.CreateProcess();
            //TempProcessBase = Project.Instance.GetProcessServiceById(ID);
            //TempProcessBase.process.processInfo.ProcessName = "A面3D流程";
            //TempProcessBase.m_eventINIProcess += Ini3DProcessA;
            //TempProcessBase.m_eventRUNProcess += ProProcess3D_A;
            //TempProcessBase.InitProcess();
            //TempProcessBase.OpeningThread();

            //ID = Project.Instance.CreateProcess();
            //TempProcessBase = Project.Instance.GetProcessServiceById(ID);
            //TempProcessBase.process.processInfo.ProcessName = "B面3D流程";
            //TempProcessBase.m_eventINIProcess += Ini3DProcessB;
            //TempProcessBase.m_eventRUNProcess += ProProcess3D_B;
            //TempProcessBase.InitProcess();
            //TempProcessBase.OpeningThread();
        }


        //初始化流程
        /// <summary>
        /// 线扫相机处理流程-主相机
        /// </summary>
        /// <param name="process"></param>
        public void IniBaseCam(Process process)
        {
            try
            {
                process.m_ModuleObjList.Clear();
                int ProcessID = process.processInfo.ProcessID;
                int camID = (int)CameraEnum.Cam1;
                string pluginName;
                IPlugin Plugin;

                //找料头插件
                pluginName = "查找料头工具";
                IPlugin PluginHead = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                PluginHead.moduleInfo.ModuleName = $"{(CameraEnum)camID}找料头";
                PluginHead.moduleInfo.PluginName = pluginName;
                ParamsFindHead findHeadParams = PluginHead.PluginParams as ParamsFindHead; //里氏转换
                findHeadParams.Is_ActiveFindHead = true;
                findHeadParams.hv_ColumnStart = 9000;
                findHeadParams.hv_ColumnEnd = 11000;
                findHeadParams.hv_RoiMidColumn = 1000;
                findHeadParams.hv_RoiHeight = 1400;
                process.AddModuleObj(PluginHead);

                pluginName = "图像裁切工具(槽位，侧基准边)";
                IPlugin PluginCropTile = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                PluginCropTile.moduleInfo.ModuleName = $"料尾裁切";
                PluginCropTile.moduleInfo.PluginName = pluginName;
                ParamsCropPart CropTileParams = PluginCropTile.PluginParams as ParamsCropPart; //里氏转换
                process.AddModuleObj(PluginCropTile);

                //pluginName = "图像裁切工具(槽位，侧基准边)";
                //IPlugin PluginCropSide = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                //PluginCropSide.moduleInfo.ModuleName = $"打孔区裁切";
                //PluginCropSide.moduleInfo.PluginName = pluginName;
                //ParamsCropPart CropSideParams = PluginCropSide.PluginParams as ParamsCropPart; //里氏转换
                //CropSideParams.CropShaftCol = 0;
                //process.AddModuleObj(PluginCropSide);

                //添加所有抓边
                ParamsMeasureLine Params;
                foreach (var item in m_craft.ListLines[camID])
                {
                    //基准边，在overhang中指代隔膜边
                    pluginName = "直线抓边工具";
                    Plugin = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                    Plugin.moduleInfo.ModuleName = (item.StrLineName);
                    Plugin.moduleInfo.PluginName = pluginName;
                    Params = Plugin.PluginParams as ParamsMeasureLine; //里氏转换
                    SetLineParams(item.StrLineName, Params);
                    process.AddModuleObj(Plugin);
                }

                for (int i = 0; i < m_craft.SliceCount; i++)
                {
                    if (m_craft.ListSliceSet[i].BelongCam == ((CameraEnum)camID).ToString())
                    {
                        pluginName = "图像裁切工具(槽位，侧基准边)";
                        IPlugin PluginCropSide = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                        PluginCropSide.moduleInfo.ModuleName = $"第{i + 1}条打孔区裁切";
                        PluginCropSide.moduleInfo.PluginName = pluginName;
                        ParamsCropPart CropSideParams = PluginCropSide.PluginParams as ParamsCropPart; //里氏转换
                        process.AddModuleObj(PluginCropSide);

                        //添加槽位{index}缺陷检测插件
                        pluginName = "模切瑕疵检测工具";
                        IPlugin PluginDefect = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                        PluginDefect.moduleInfo.ModuleName = $"第{i + 1}条打孔区缺陷检测";
                        PluginDefect.moduleInfo.PluginName = pluginName;
                        ParamsMQDefects DefectParams = PluginDefect.PluginParams as ParamsMQDefects; //里氏转换
                        DefectParams.com_DetectionMethod = 4;
                        process.AddModuleObj(PluginDefect);

                        //添加槽位{index}缺陷检测插件
                        pluginName = "模切瑕疵检测工具";
                        IPlugin PluginDefectCoat = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                        PluginDefectCoat.moduleInfo.ModuleName = $"第{i + 1}条涂布区缺陷检测";
                        PluginDefectCoat.moduleInfo.PluginName = pluginName;
                        ParamsMQDefects DefectParamsCoat = PluginDefectCoat.PluginParams as ParamsMQDefects; //里氏转换
                        DefectParams.com_DetectionMethod = 1;
                        process.AddModuleObj(PluginDefectCoat);
                    }
                }

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }

        /// <summary>
        /// EA处理流程-主相机
        /// </summary>
        /// <param name="process"></param>
        public void IniBaseEA(Process process)
        {
            try
            {
                process.m_ModuleObjList.Clear();
                int ProcessID = process.processInfo.ProcessID;
                int camID = ProcessID;
                string pluginName;
                IPlugin Plugin;
                ParamsMeasureLine Params;

                pluginName = "图像裁切工具(槽位，侧基准边)";
                IPlugin PluginCropTile = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                PluginCropTile.moduleInfo.ModuleName = $"图片裁切";
                PluginCropTile.moduleInfo.PluginName = pluginName;
                ParamsCropPart CropTileParams = PluginCropTile.PluginParams as ParamsCropPart; //里氏转换
                process.AddModuleObj(PluginCropTile);

                //foreach (var item in m_craft.ListLines[camID])
                //{
                //    //基准边，在overhang中指代隔膜边
                //    pluginName = "直线抓边工具";
                //    Plugin = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                //    Plugin.moduleInfo.ModuleName = (item.StrLineName);
                //    Plugin.moduleInfo.PluginName = pluginName;
                //    Params = Plugin.PluginParams as ParamsMeasureLine; //里氏转换
                //    Params.hv_RoiAngle = 90;
                //    process.AddModuleObj(Plugin);
                //}

                //pluginName = "模切瑕疵检测工具";
                //IPlugin PluginDefects = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                //PluginDefects.moduleInfo.ModuleName = $"涂布瑕疵检测";
                //PluginDefects.moduleInfo.PluginName = pluginName;
                //ParamsMQDefects MQDefectsParams = PluginDefects.PluginParams as ParamsMQDefects; //里氏转换
                //MQDefectsParams.com_Cathode = 0;
                //MQDefectsParams.com_DetectionMethod = 4;
                //process.AddModuleObj(PluginDefects);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }

        }
        /// <summary>
        /// 线扫相机处理流程-副相机
        /// </summary>
        /// <param name="process"></param>
        public void IniFromCam(Process process)
        {
            try
            {
                process.m_ModuleObjList.Clear();
                int ProcessID = process.processInfo.ProcessID;
                int camID = (int)CameraEnum.Cam2;
                string pluginName;
                IPlugin Plugin;


                pluginName = "图像裁切工具(槽位，侧基准边)";
                IPlugin PluginCropHead = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                PluginCropHead.moduleInfo.ModuleName = $"料头裁切";
                PluginCropHead.moduleInfo.PluginName = pluginName;
                ParamsCropPart CropHeadParams = PluginCropHead.PluginParams as ParamsCropPart; //里氏转换
                process.AddModuleObj(PluginCropHead);

                pluginName = "图像裁切工具(槽位，侧基准边)";
                IPlugin PluginCropTile = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                PluginCropTile.moduleInfo.ModuleName = $"料尾裁切";
                PluginCropTile.moduleInfo.PluginName = pluginName;
                ParamsCropPart CropTileParams = PluginCropTile.PluginParams as ParamsCropPart; //里氏转换
                process.AddModuleObj(PluginCropTile);

                //pluginName = "图像裁切工具(槽位，侧基准边)";
                //IPlugin PluginCropSide = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                //PluginCropSide.moduleInfo.ModuleName = $"打孔区裁切";
                //PluginCropSide.moduleInfo.PluginName = pluginName;
                //ParamsCropPart CropSideParams = PluginCropSide.PluginParams as ParamsCropPart; //里氏转换
                //CropSideParams.CropShaftCol = 0;
                //process.AddModuleObj(PluginCropSide);

                //添加所有抓边
                ParamsMeasureLine Params;
                foreach (var item in m_craft.ListLines[camID])
                {
                    //基准边，在overhang中指代隔膜边
                    pluginName = "直线抓边工具";
                    Plugin = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                    Plugin.moduleInfo.ModuleName = (item.StrLineName);
                    Plugin.moduleInfo.PluginName = pluginName;
                    Params = Plugin.PluginParams as ParamsMeasureLine; //里氏转换
                    SetLineParams(item.StrLineName, Params);
                    process.AddModuleObj(Plugin);
                }

                for (int i = 0; i < m_craft.SliceCount; i++)
                {
                    if (m_craft.ListSliceSet[i].BelongCam == ((CameraEnum)camID).ToString())
                    {
                        pluginName = "图像裁切工具(槽位，侧基准边)";
                        IPlugin PluginCropSide = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                        PluginCropSide.moduleInfo.ModuleName = $"第{i + 1}条打孔区裁切";
                        PluginCropSide.moduleInfo.PluginName = pluginName;
                        ParamsCropPart CropSideParams = PluginCropSide.PluginParams as ParamsCropPart; //里氏转换
                        process.AddModuleObj(PluginCropSide);

                        //添加槽位{index}缺陷检测插件
                        pluginName = "模切瑕疵检测工具";
                        IPlugin PluginDefect = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                        PluginDefect.moduleInfo.ModuleName = $"第{i + 1}条打孔区缺陷检测";
                        PluginDefect.moduleInfo.PluginName = pluginName;
                        ParamsMQDefects DefectParams = PluginDefect.PluginParams as ParamsMQDefects; //里氏转换
                        DefectParams.com_DetectionMethod = 4;
                        process.AddModuleObj(PluginDefect);

                        //添加槽位{index}缺陷检测插件
                        pluginName = "模切瑕疵检测工具";
                        IPlugin PluginDefectCoat = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                        PluginDefectCoat.moduleInfo.ModuleName = $"第{i + 1}条涂布区缺陷检测";
                        PluginDefectCoat.moduleInfo.PluginName = pluginName;
                        ParamsMQDefects DefectParamsCoat = PluginDefectCoat.PluginParams as ParamsMQDefects; //里氏转换
                        DefectParams.com_DetectionMethod = 1;
                        process.AddModuleObj(PluginDefectCoat);
                    }
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }

        }
        /// <summary>
        /// EA处理流程-副相机
        /// </summary>
        /// <param name="process"></param>
        public void IniFromEA(Process process)
        {
            try
            {
                process.m_ModuleObjList.Clear();
                int ProcessID = process.processInfo.ProcessID;
                int camID = ProcessID;
                string pluginName;
                IPlugin Plugin;
                ParamsMeasureLine Params;

                pluginName = "图像裁切工具(槽位，侧基准边)";
                IPlugin PluginCropTile = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                PluginCropTile.moduleInfo.ModuleName = $"图片裁切";
                PluginCropTile.moduleInfo.PluginName = pluginName;
                ParamsCropPart CropTileParams = PluginCropTile.PluginParams as ParamsCropPart; //里氏转换
                process.AddModuleObj(PluginCropTile);

                //foreach (var item in m_craft.ListLines[camID])
                //{
                //    //基准边，在overhang中指代隔膜边
                //    pluginName = "直线抓边工具";
                //    Plugin = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                //    Plugin.moduleInfo.ModuleName = (item.StrLineName);
                //    Plugin.moduleInfo.PluginName = pluginName;
                //    Params = Plugin.PluginParams as ParamsMeasureLine; //里氏转换
                //    Params.hv_RoiAngle = 90;
                //    process.AddModuleObj(Plugin);
                //}

                //pluginName = "模切瑕疵检测工具";
                //IPlugin PluginDefects = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                //PluginDefects.moduleInfo.ModuleName = $"涂布瑕疵检测";
                //PluginDefects.moduleInfo.PluginName = pluginName;
                //ParamsMQDefects MQDefectsParams = PluginDefects.PluginParams as ParamsMQDefects; //里氏转换
                //MQDefectsParams.com_Cathode = 0;
                //MQDefectsParams.com_DetectionMethod = 4;
                //process.AddModuleObj(PluginDefects);


                //pluginName = "模切瑕疵检测工具";
                //PluginDefects = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                //PluginDefects.moduleInfo.ModuleName = $"单片二瑕疵检测";
                //PluginDefects.moduleInfo.PluginName = pluginName;
                //MQDefectsParams = PluginDefects.PluginParams as ParamsMQDefects; //里氏转换
                //MQDefectsParams.com_Cathode = 0;
                //MQDefectsParams.com_DetectionMethod = 4;
                //process.AddModuleObj(PluginDefects);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }

        }

        public void Ini3DProcessA(Process process)
        {
            try
            {
                process.m_ModuleObjList.Clear();
                int ProcessID = process.processInfo.ProcessID;
                int camID = ProcessID;
                string pluginName;
                IPlugin Plugin;
                ParamsMeasureLine Params;

                pluginName = "直线抓边工具";
                Plugin = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                Plugin.moduleInfo.ModuleName = $"3D定位抓边";
                Plugin.moduleInfo.PluginName = pluginName;
                Params = Plugin.PluginParams as ParamsMeasureLine; //里氏转换
                Params.hv_RoiAngle = 90;
                process.AddModuleObj(Plugin);

                pluginName = "3D测深度";
                IPlugin PluginDepth = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                PluginDepth.moduleInfo.ModuleName = $"3D测深度";
                PluginDepth.moduleInfo.PluginName = pluginName;
                ParamsD3MeasureDepth CropTileParams = PluginDepth.PluginParams as ParamsD3MeasureDepth; //里氏转换
                process.AddModuleObj(PluginDepth);

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }

        }

        public void Ini3DProcessB(Process process)
        {
            try
            {
                process.m_ModuleObjList.Clear();
                int ProcessID = process.processInfo.ProcessID;
                int camID = ProcessID;
                string pluginName;
                IPlugin Plugin;
                ParamsMeasureLine Params;

                pluginName = "直线抓边工具";
                Plugin = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                Plugin.moduleInfo.ModuleName = $"3D定位抓边";
                Plugin.moduleInfo.PluginName = pluginName;
                Params = Plugin.PluginParams as ParamsMeasureLine; //里氏转换
                Params.hv_RoiAngle = 90;
                process.AddModuleObj(Plugin);


                pluginName = "3D测深度";
                IPlugin PluginDepth = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                PluginDepth.moduleInfo.ModuleName = $"3D测深度";
                PluginDepth.moduleInfo.PluginName = pluginName;
                ParamsD3MeasureDepth CropTileParams = PluginDepth.PluginParams as ParamsD3MeasureDepth; //里氏转换
                process.AddModuleObj(PluginDepth);

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }

        }

        public void SetLineParams(string pluginName, ParamsMeasureLine Params)
        {
            if (pluginName.Contains((DirectionEnum.左边).ToString()) || pluginName.Contains((DirectionEnum.右间隙边).ToString()))
            {
                Params.hv_RoiPointsNum = 60;
                Params.hv_RoiAngle = 90;
                Params.hv_MeasureThreshold = 8;
                Params.bl_ImageProcessIndex = true;
                Params.hv_ScaleMean = 25;
                Params.hv_MeasureMinScore = 0.2;
            }
            else if (pluginName.Contains((DirectionEnum.右边).ToString()) || pluginName.Contains((DirectionEnum.左间隙边).ToString()))
            {
                Params.hv_RoiPointsNum = 60;
                Params.hv_RoiAngle = -90;
                Params.hv_MeasureThreshold = 8;
                Params.bl_ImageProcessIndex = true;
                Params.hv_ScaleMean = 25;
                Params.hv_MeasureMinScore = 0.2;
            }
            else if (pluginName.Contains((DirectionEnum.上边).ToString()))
            {
                Params.hv_RoiPointsNum = 60;
                Params.hv_RoiAngle = 180;
                Params.hv_MeasureThreshold = 10;
                Params.bl_ImageProcessIndex = false;
                Params.hv_ScaleMean = 25;
                Params.hv_MeasureMinScore = 0.2;
            }
            else if (pluginName.Contains((DirectionEnum.下边).ToString()))
            {
                Params.hv_RoiPointsNum = 60;
                Params.hv_RoiAngle = 0;
                Params.hv_MeasureThreshold = 10;
                Params.bl_ImageProcessIndex = false;
                Params.hv_ScaleMean = 25;
                Params.hv_MeasureMinScore = 0.2;
            }
            else if (pluginName.Contains("基准边"))
            {
                Params.hv_RoiPointsNum = 60;
                Params.hv_RoiAngle = 90;
                Params.hv_MeasureThreshold = 30;
            }
        }


        //执行流程
        public void ProProcessBaseCam(Process process)
        {
            try
            {
                int ProcessID = process.processInfo.ProcessID;
                int camID = 0;
                SetThreadAffinityMask(GetCurrentThread(), new UIntPtr(SetCpuID(ProcessID)));

                //标定系数
                double Calipix_y = Project.Instance.cameraManager[(CameraEnum)camID].m_CamParam.CalibrationValue_Y;
                double Calipix_x = Project.Instance.cameraManager[(CameraEnum)camID].m_CamParam.CalibrationValue_X;
                double ABMis = Project.Instance.cameraManager[(CameraEnum)camID + 1].m_CamParam.ToMainCamDistance / Calipix_y;//到主相机像素距离

                double HeadMisAtoB = m_craft.HeadMis_AtoB / Calipix_y;//

                // 选择参数和队列基于是否模拟
                PFTCleanProParams PFTCleanParamBase = Project.Instance.Is_simulation ? m_DicCamOperaParam_Sim[camID] as PFTCleanProParams : m_DicCamOperaParam[camID] as PFTCleanProParams;
                PFTCleanProParams PFTCleanParamFrom = Project.Instance.Is_simulation ? m_DicCamOperaParam_Sim[camID + 1] as PFTCleanProParams : m_DicCamOperaParam[camID + 1] as PFTCleanProParams;
                var imageQueue = PFTCleanParamBase.alreadyDealGrabImage;
                PFTCleanParamBase.semaphoreSlimCam.Wait();

                if (imageQueue.Count > 0) // 检查主相机的图像队列是否有图像
                {
                    PFTCleanEAProductInfo currentEAProduct = PFTCleanParamBase.dic_EAproductsInfo[PFTCleanParamBase.nCurEAProId];
                    // PFTCleanEAProductInfo EAProductFrom = PFTCleanParamFrom.dic_EAproductsInfo[PFTCleanParamBase.nCurEAProId];
                    PFTImageProduct ImageProduct;
                    ImageInfo InfoImgInfo = imageQueue.Dequeue();
                    PFTCleanParamBase.imageInfos.Add(InfoImgInfo);
                    if (PFTCleanParamBase.imageInfos.Count > 8)
                    {
                        //PFTCleanParamBase.imageInfos.First().Dispose();
                        PFTCleanParamBase.imageInfos.RemoveAt(0);
                    }

                    List<MessageShowInfo> MesssgeShowInfos = new List<MessageShowInfo>();
                    List<RegionShowInfo> RegionShowInfos = new List<RegionShowInfo>();
                    List<LineInfo> LineShowInfos = new List<LineInfo>();

                    ImageProduct = new PFTImageProduct();
                    for (int i = 0; i < m_craft.SliceCount; i++)
                    {
                        ImageProduct.ListImageSlicePD.Add(new PFTSliceProduct());
                    }
                    ImageProduct.nCurImageID = InfoImgInfo.image_ID;
                    ImageProduct.nCurEAID = PFTCleanParamBase.nCurEAProId;
                    int count0 = InfoImgInfo.ho_image.GetReferenceCount();
                    string str = ((CameraEnum)camID).ToString() + "信号量--" + PFTCleanParamBase.semaphoreSlimCam.CurrentCount.ToString() + "图像数--" + imageQueue.Count.ToString();
                    MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = str, m_stuGetColor = HalconColor.green });
                    MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = $"当前产品编号{currentEAProduct.EAID}，当前图像帧号：{ImageProduct.nCurImageID}", m_stuGetColor = HalconColor.green });


                    //开始检测计时            
                    HOperatorSet.CountSeconds(out ImageProduct.Image_StartTime);

                    LineInfo EdgeLine = new LineInfo();
                    //确认基准边 L+R ,必须有的两条边--
                    // for (int i = 0; i < 2; i++)
                    {
                        string linepluginname = "涂布左基准边";
                        if (camID == 1) linepluginname = "涂布右基准边";
                        //抓边
                        IPlugin PluginMline = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == linepluginname);
                        ParamsMeasureLine MlineParams = PluginMline.PluginParams as ParamsMeasureLine; //里氏转换
                        MlineParams.ho_Image = InfoImgInfo.ho_image;
                        LineInfo line = new LineInfo();
                        bool isSuccess = PluginMline.ExecuteObj();
                        DetectEdgeValueAssign(MlineParams, isSuccess, line, linepluginname, MesssgeShowInfos, LineShowInfos, RegionShowInfos);

                        //把抓边添加到ImageProduct和EAProduct
                        LineInfo Normaleftline = NormalizeLine(line, (int)ImageProduct.nCurImageID, 0, 0, (int)InfoImgInfo.image_ID, InfoImgInfo.imageHeight);
                        ImageProduct.ListImagelines.Add(Normaleftline);
                        EdgeLine = EdgeLine.copyLine(Normaleftline);
                        LineInfo EAline = EAAddline(Normaleftline, currentEAProduct.CropEArange.StartImageID, currentEAProduct.CropEArange.StarImageRow, 0, InfoImgInfo.imageHeight);
                        currentEAProduct.ListEAlines.Add(EAline);

                        if (isSuccess)
                        {
                            if (camID == 0)
                            {
                                ImageProduct.EdgelineLeft = (MlineParams.hv_LineColumnBegin.D + MlineParams.hv_LineColumnEnd.D) / 2;
                                ImageProduct.EdgelineRight = InfoImgInfo.imageWidth;
                            }
                            else
                            {
                                ImageProduct.EdgelineRight = (MlineParams.hv_LineColumnBegin.D + MlineParams.hv_LineColumnEnd.D) / 2;
                                double EdgeShaft = ImageProduct.EdgelineRight - m_craft.Cam2BaseCol;
                                ImageProduct.EdgelineLeft = -(m_craft.EXCam1Col - m_craft.EXCam2Col - m_craft.Cam1BaseCol - EdgeShaft);

                            }
                        }
                    }

                    //找料头
                    IPlugin Plugin = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == $"{(CameraEnum)camID}找料头");
                    ParamsFindHead findHeadParams = Plugin.PluginParams as ParamsFindHead; //里氏转换
                    findHeadParams.list_ImageInfo = PFTCleanParamBase.imageInfos;
                    findHeadParams.ImageNowID = (int)InfoImgInfo.image_ID;
                    bool bl_findHeadSuccess = Plugin.ExecuteObj();
                    if (bl_findHeadSuccess)
                    {
                        if (findHeadParams.hv_ImageHeadBeginID == findHeadParams.ImageNowID)
                        {
                            HOperatorSet.GenRectangle1(out HObject GroVRegion1, findHeadParams.hv_ImageHeadBeginRow, findHeadParams.hv_ImageHeadBeginCol, findHeadParams.hv_ImageHeadEndRow, findHeadParams.hv_ImageHeadEndCol);
                            RegionShowInfos.Add(new RegionShowInfo() { m_hRegion = GroVRegion1, m_stuGetColor = HalconColor.green });
                        }
                        LineInfo HeadlineInfo = new LineInfo();
                        HeadlineInfo.StrLineName = "涂布料头边";
                        HeadlineInfo.IsLineSuccessfull = true;
                        HeadlineInfo.RowBeginID = (int)InfoImgInfo.image_ID;
                        HeadlineInfo.RowBegin = (findHeadParams.headLineID - HeadlineInfo.RowBeginID) * InfoImgInfo.imageHeight + findHeadParams.headLine;
                        HeadlineInfo.ColBegin = 100;
                        HeadlineInfo.RowEndID = (int)InfoImgInfo.image_ID;
                        HeadlineInfo.RowEnd = (findHeadParams.headLineID - HeadlineInfo.RowBeginID) * InfoImgInfo.imageHeight + findHeadParams.headLine;
                        HeadlineInfo.ColEnd = InfoImgInfo.imageWidth - 100;
                        HeadlineInfo.m_nLineWidth = 10;
                        HeadlineInfo.Color = HalconColor.pink;
                        LineShowInfos.Add(HeadlineInfo);
                        string str0 = $"{(CameraEnum)camID}找料头成功，产品编号：{PFTCleanParamBase.nCurEAProId + 1},料头位置：第{findHeadParams.headLineID}帧-第{findHeadParams.headLine}行";
                        MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = str0, m_stuGetColor = HalconColor.green });

                        ImageProduct.blSplitEA = true;
                        //扩充EA产品-cam0
                        PFTCleanEAProductInfo EAEAProductCam0 = ExPandHeadLinedic(camID, PFTCleanParamBase.nCurEAProId + 1, findHeadParams.headLineID, findHeadParams.headLine, InfoImgInfo.imageHeight, Calipix_y, Calipix_x);//new一个新产品信息
                        PFTCleanParamBase.dic_EAproductsInfo[PFTCleanParamBase.nCurEAProId + 1] = EAEAProductCam0;
                        //扩充EA产品-cam1
                        PFTCleanEAProductInfo EAEAProductCam1 = ExPandHeadLinedic(camID + 1, PFTCleanParamBase.nCurEAProId + 1, findHeadParams.headLineID, findHeadParams.headLine + ABMis + HeadMisAtoB, InfoImgInfo.imageHeight, Calipix_y, Calipix_x);//new一个新产品信息
                        PFTCleanParamFrom.dic_EAproductsInfo[PFTCleanParamBase.nCurEAProId + 1] = EAEAProductCam1;

                        //把抓边添加到ImageProduct和EAProduct
                        ImageProduct.ListImagelines.Add(HeadlineInfo);
                        LineInfo EAline = EAAddline(HeadlineInfo, EAEAProductCam0.CropEArange.StartImageID, EAEAProductCam0.CropEArange.StarImageRow, 0, InfoImgInfo.imageHeight);
                        EAEAProductCam0.ListEAlines.Add(EAline);
                    }
                    //超长EA，强制结束EA，
                    else if (InfoImgInfo.image_ID >= currentEAProduct.EArange.EndImageID + 2)
                    {
                        ImageProduct.blSplitEA = true;

                        //扩充EA产品-cam0
                        PFTCleanEAProductInfo EAEAProductCam0 = ExPandHeadLinedic(camID, PFTCleanParamBase.nCurEAProId + 1, currentEAProduct.EArange.EndImageID + 2, 0, InfoImgInfo.imageHeight, Calipix_y, Calipix_x);//new一个新产品信息
                        PFTCleanParamBase.dic_EAproductsInfo[PFTCleanParamBase.nCurEAProId + 1] = EAEAProductCam0;
                        //扩充EA产品-cam1
                        PFTCleanEAProductInfo EAEAProductCam1 = ExPandHeadLinedic(camID + 1, PFTCleanParamBase.nCurEAProId + 1, findHeadParams.headLineID, findHeadParams.headLine + ABMis + HeadMisAtoB, InfoImgInfo.imageHeight, Calipix_y, Calipix_x);//new一个新产品信息
                        PFTCleanParamFrom.dic_EAproductsInfo[PFTCleanParamBase.nCurEAProId + 1] = EAEAProductCam1;

                        string str0 = $"A相机EA产品超长，强制分EA，产品编号：{PFTCleanParamBase.nCurEAProId + 1},料头位置：第{currentEAProduct.EArange.EndImageID}帧-第{currentEAProduct.EArange.EndImageRow}行";
                        MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = str0, m_stuGetColor = HalconColor.red });
                    }

                    //同一排打孔区裁一条横向贯穿图
                    //裁削薄区打孔区
                    if (true)
                    {
                        //bool ToDoShaveCrop = true;
                        //while (ToDoShaveCrop && currentEAProduct.ArraycurShaveIndex[0] < m_craft.ListSliceSet[0].ShaveNum)
                        //{
                        //    //选第一条削薄区定位距来确定裁切位置，
                        //    int index = currentEAProduct.ArraycurShaveIndex[0];
                        //    SlotProduct Cropslot0 = currentEAProduct.list2EAShave[0][index];

                        //    string namn = $"打孔区裁切";
                        //    IPlugin CropPlugin = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == namn);
                        //    ParamsCropPart CropPartParams = CropPlugin.PluginParams as ParamsCropPart; //里氏转换
                        //    CropPartParams.list_ImageInfo = PFTCleanParamBase.imageInfos;

                        //    CropPartParams.PartSizeRow = Cropslot0.Croprange.Height;
                        //    CropPartParams.PartSizeCol = ImageProduct.EdgelineRight - ImageProduct.EdgelineLeft + 200;
                        //    CropPartParams.PartCenterID = Cropslot0.Croprange.MidImageID;
                        //    CropPartParams.PartCenterRow = Cropslot0.Croprange.MidImageRow;
                        //    CropPartParams.PartCenterCol = (ImageProduct.EdgelineRight + ImageProduct.EdgelineLeft) / 2 - 100;//槽和基准边同一边就不用+，在对侧，就用片宽减半槽宽。

                        //    //tempslot.Croprange.StartCol = ImageProduct.EdgelineLeft + tempslot.slotInfo.SlotTDPosition / Calipix_x - tempslot.slotInfo.OutwardShieldLeft / Calipix_x;
                        //    //tempslot.Croprange.EndCol = tempslot.Croprange.StartCol + CropPartParams.PartSizeCol;

                        //    bool bl_ExecuteSuccess = CropPlugin.ExecuteObj();
                        //    if (bl_ExecuteSuccess)//削薄区成功
                        //    {
                        //        if (true)
                        //        {
                        //            double rowMove = (Cropslot0.Croprange.MidImageID - ImageProduct.nCurImageID) * InfoImgInfo.imageHeight + CropPartParams.PartCenterRow;
                        //            HOperatorSet.GenRectangle2(out HObject GroVRegion1, rowMove, CropPartParams.PartCenterCol, 0, CropPartParams.PartSizeCol / 2 + CropPartParams.CropShaftCol, CropPartParams.PartSizeRow / 2 + CropPartParams.CropShaftRow);
                        //            RegionShowInfos.Add(new RegionShowInfo() { m_hRegion = GroVRegion1, m_stuGetColor = HalconColor.pink });

                        //            string str0 = $"第{index + 1}排打孔区裁切成功";
                        //            MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = str0, m_stuGetColor = HalconColor.orange });
                        //            currentEAProduct.ArraycurShaveIndex[0]++;

                        //            //tempslot.showHalconWindow.m_hImage = CropPartParams.Image_part;
                        //            //tempslot.showHalconWindow.m_nImageFrameID = index;
                        //            //tempslot.showHalconWindow.m_nProductID = PFTCleanParamBase.nCurEAProId;
                        //        }
                        //        //后续开并行执行
                        //        //Parallel.For(0, m_craft.SliceCount, i =>
                        //        for (int i = 0; i < m_craft.SliceCount; i++)
                        //        {
                        //            int SliceIndex = i;
                        //            if (m_craft.ListSliceSet[SliceIndex].BelongCam == ((CameraEnum)camID).ToString())
                        //            {
                        //                //削薄区
                        //                SlotProduct tempslot = currentEAProduct.list2EAShave[SliceIndex][index];
                        //                //找对应槽的四条边
                        //                double C1 = 1000; //矩形左边
                        //                double R1 = 1000; //矩形上边
                        //                double C2 = 2000; //矩形右边
                        //                double R2 = 2000; //矩形下边
                        //                int count = Enum.GetValues(typeof(DirectionEnum)).Length;
                        //                bool BlSlotLine = true;
                        //                for (int j = 0; j < count; j++)
                        //                {
                        //                    string LineName = $"第{SliceIndex + 1}条打孔区" + ((DirectionEnum)j).ToString();

                        //                    IPlugin PluginMline = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == LineName);
                        //                    ParamsMeasureLine MlineParams = PluginMline.PluginParams as ParamsMeasureLine; //里氏转换
                        //                    MlineParams.ho_Image = CropPartParams.Image_part;
                        //                    LineInfo line = new LineInfo();
                        //                    bool isSuccess = PluginMline.ExecuteObj();
                        //                    string ShaveName = $"第{SliceIndex + 1}条第{index + 1}个打孔区" + ((DirectionEnum)j).ToString();
                        //                    DetectEdgeValueAssign(MlineParams, isSuccess, line, ShaveName, MesssgeShowInfos, LineShowInfos, RegionShowInfos);

                        //                    tempslot.showHalconWindow.m_lineShowInfos = LineShowInfos;
                        //                    //把抓边添加到ImageProduct和EAProduct
                        //                    double Srow = CropPartParams.PartCenterRow - CropPartParams.PartSizeRow / 2 - CropPartParams.CropShaftRow;
                        //                    double Scol = CropPartParams.PartCenterCol - CropPartParams.PartSizeCol / 2 - CropPartParams.CropShaftCol;
                        //                    LineInfo Normaleftline = NormalizeLine(line, (int)CropPartParams.PartCenterID, Srow, Scol, (int)InfoImgInfo.image_ID, InfoImgInfo.imageHeight);
                        //                    //添加边到图像抓边队列
                        //                    ImageProduct.ListImagelines.Add(Normaleftline);
                        //                    //添加边到打孔区抓边队列
                        //                    tempslot.lineInfos.Add(Normaleftline);

                        //                    LineInfo EAline = EAAddline(Normaleftline, currentEAProduct.CropEArange.StartImageID, currentEAProduct.CropEArange.StarImageRow, 0, InfoImgInfo.imageHeight);
                        //                    // current3DProduct.ListEAlines.Add(EAline);

                        //                    BlSlotLine &= isSuccess;
                        //                    //CalcuEASize(camID, InfoImgInfo.imageHeight, Normaleftline, m_craft.ListEASizes, currentEAProduct, EAProductFrom, ImageProduct.ListImageSizes);
                        //                    if (isSuccess)
                        //                    {
                        //                        if (j == 0)
                        //                        {
                        //                            C1 = (MlineParams.hv_LineColumnBegin.D + MlineParams.hv_LineColumnEnd.D) / 2 + tempslot.slotInfo.InwardShieldLeft / Calipix_x;
                        //                            tempslot.SlotSizes[0].DataResult = ((Normaleftline.ColBegin - ImageProduct.EdgelineLeft) + (Normaleftline.ColEnd - ImageProduct.EdgelineLeft)) * 0.5 * Calipix_x;  //td定位距
                        //                        }
                        //                        else if (j == 1)
                        //                        {
                        //                            C2 = (MlineParams.hv_LineColumnBegin.D + MlineParams.hv_LineColumnEnd.D) / 2 - tempslot.slotInfo.InwardShieldRight / Calipix_x;
                        //                            tempslot.SlotSizes[2].DataResult = (C2 - C1) * Calipix_x;//td尺寸
                        //                        }
                        //                        else if (j == 2)
                        //                        {
                        //                            R1 = (MlineParams.hv_LineRowBegin.D + MlineParams.hv_LineRowEnd.D) / 2 + tempslot.slotInfo.InwardShieldUp / Calipix_y;

                        //                            double subRowS = (Normaleftline.RowBeginID - currentEAProduct.EArange.StartImageID) * InfoImgInfo.imageHeight + Normaleftline.RowBegin - currentEAProduct.EArange.StarImageRow;
                        //                            double subRowE = (Normaleftline.RowEndID - currentEAProduct.EArange.StartImageID) * InfoImgInfo.imageHeight + Normaleftline.RowEnd - currentEAProduct.EArange.StarImageRow;
                        //                            tempslot.SlotSizes[1].DataResult = (subRowS + subRowE) * 0.5 * Calipix_y;  //td定位距
                        //                        }
                        //                        else if (j == 3)
                        //                        {
                        //                            R2 = (MlineParams.hv_LineRowBegin.D + MlineParams.hv_LineRowEnd.D) / 2 - tempslot.slotInfo.InwardShieldDown / Calipix_y;
                        //                            tempslot.SlotSizes[3].DataResult = (R2 - R1) * Calipix_y;//Md尺寸
                        //                        }
                        //                    }
                        //                }

                        //                //检测对应削薄区的缺陷，
                        //                //执行缺陷检测。
                        //                IPlugin PluginSlotefects = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == $"第{SliceIndex + 1}条打孔区缺陷检测");
                        //                ParamsMQDefects _ParamsSlotefects = PluginSlotefects.PluginParams as ParamsMQDefects;
                        //                if (BlSlotLine && C1 < C2 && R1 < R2)
                        //                {
                        //                    //添加槽位检测缩进 current3DProduct.listEAShave[index]
                        //                    HOperatorSet.GenRectangle1(out HObject GroVRegion1, R1, C1, R2, C2);
                        //                    _ParamsSlotefects.DetectionROI = GroVRegion1;

                        //                    int STid = tempslot.Croprange.StartImageID;
                        //                    double STrow = tempslot.Croprange.StarImageRow - CropPartParams.CropShaftRow;
                        //                    double STcol = tempslot.Croprange.StartCol - CropPartParams.CropShaftCol;
                        //                    HObject GroVRegionMoved = NormalizeRegion(GroVRegion1, STid, STrow, STcol, (int)InfoImgInfo.image_ID, InfoImgInfo.imageHeight);
                        //                    RegionShowInfos.Add(new RegionShowInfo() { m_hRegion = GroVRegionMoved, m_stuGetColor = HalconColor.cyan });
                        //                }
                        //                else
                        //                {
                        //                    HOperatorSet.GenRectangle1(out HObject GroVRegion1, CropPartParams.CropShaftRow, CropPartParams.CropShaftCol, CropPartParams.PartSizeRow + CropPartParams.CropShaftRow, CropPartParams.PartSizeCol + CropPartParams.CropShaftCol);
                        //                    _ParamsSlotefects.DetectionROI = GroVRegion1;
                        //                    int STid = tempslot.Croprange.StartImageID;
                        //                    double STrow = tempslot.Croprange.StarImageRow - CropPartParams.CropShaftRow;
                        //                    double STcol = tempslot.Croprange.StartCol - CropPartParams.CropShaftCol;
                        //                    HObject GroVRegionMoved = NormalizeRegion(GroVRegion1, STid, STrow, STcol, (int)InfoImgInfo.image_ID, InfoImgInfo.imageHeight);
                        //                    RegionShowInfos.Add(new RegionShowInfo() { m_hRegion = GroVRegionMoved, m_stuGetColor = HalconColor.cyan });
                        //                }
                        //                HOperatorSet.GenEmptyObj(out _ParamsSlotefects.ShieldROI);
                        //                _ParamsSlotefects.ho_Image = CropPartParams.Image_part;
                        //                _ParamsSlotefects.WidthCali = Calipix_x;
                        //                _ParamsSlotefects.HeightCali = Calipix_y;
                        //                PluginSlotefects.ExecuteObj();

                        //                //整合缺陷检测结果。整合缺陷到EA。
                        //                while (_ParamsSlotefects.detectInfos.Count() > 0)
                        //                {
                        //                    NgTypeInfo item = _ParamsSlotefects.detectInfos?.Dequeue();
                        //                    item.m_nImgIndex = (int)InfoImgInfo.image_ID;
                        //                    item.m_iProductIndex = PFTCleanParamBase.nCurEAProId;
                        //                    item.m_iSliceIndex = SliceIndex;
                        //                    int STid = tempslot.Croprange.StartImageID;
                        //                    double STrow = tempslot.Croprange.StarImageRow - CropPartParams.CropShaftRow;
                        //                    double STcol = tempslot.Croprange.StartCol - CropPartParams.CropShaftCol;
                        //                    HObject GroVRegionMoved = NormalizeRegion(item.m_hRegion, STid, STrow, STcol, (int)InfoImgInfo.image_ID, InfoImgInfo.imageHeight);
                        //                    item.m_hRegion = GroVRegionMoved;
                        //                    ImageProduct.ListImageDefects.Add(item);

                        //                    NgTypeInfo EAngInfo = new NgTypeInfo();
                        //                    EAngInfo = item.Copy();
                        //                    HOperatorSet.CopyObj(GroVRegionMoved, out HObject region, 1, 1);
                        //                    int EASTid = currentEAProduct.CropEArange.StartImageID;
                        //                    double EASTrow = currentEAProduct.CropEArange.StarImageRow;
                        //                    HObject RegionMoved = EAAddRegion(region, EASTid, EASTrow, (int)InfoImgInfo.image_ID, InfoImgInfo.imageHeight);
                        //                    EAngInfo.m_hRegion = RegionMoved;
                        //                    currentEAProduct.ListEADefects.Add(EAngInfo);
                        //                }
                        //            }

                        //        }
                        //        // });
                        //    }
                        //    else //先做没裁图成功就跳出循环。后期再加帧号防呆。
                        //    {
                        //        ToDoShaveCrop = false;
                        //    }
                        //}

                    }

                    Parallel.For(0, m_craft.SliceCount, i =>
                    // for (int i = 0; i < m_craft.SliceCount; i++)
                    {
                        int SliceIndex = i;
                        ImageProduct.ListImageSlicePD[SliceIndex].showHalconWindow.m_messageShowInfos = new List<MessageShowInfo>();
                        ImageProduct.ListImageSlicePD[SliceIndex].showHalconWindow.m_regionShowInfos = new List<RegionShowInfo>();
                        ImageProduct.ListImageSlicePD[SliceIndex].showHalconWindow.m_lineShowInfos = new List<LineInfo>();
                        if (m_craft.ListSliceSet[SliceIndex].BelongCam == ((CameraEnum)camID).ToString())
                        {
                            if (true)
                            {
                                //裁削薄区打孔区
                                bool ToDoShaveCrop = true;
                                while (ToDoShaveCrop && currentEAProduct.ListEASlicePD[SliceIndex].curSlShaveIndex < m_craft.ListSliceSet[SliceIndex].ShaveNum)
                                {
                                    int index = currentEAProduct.ListEASlicePD[SliceIndex].curSlShaveIndex;
                                    //削薄区
                                    SlotProduct tempslot = currentEAProduct.ListEASlicePD[SliceIndex].listShave[index];

                                    string namn = $"第{SliceIndex + 1}条打孔区裁切";
                                    IPlugin CropPlugin = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == namn);
                                    ParamsCropPart CropPartParams = CropPlugin.PluginParams as ParamsCropPart; //里氏转换
                                    CropPartParams.list_ImageInfo = PFTCleanParamBase.imageInfos;

                                    CropPartParams.PartSizeRow = tempslot.Croprange.Height;
                                    CropPartParams.PartSizeCol = tempslot.Croprange.Width;
                                    CropPartParams.PartCenterID = tempslot.Croprange.MidImageID;
                                    CropPartParams.PartCenterRow = tempslot.Croprange.MidImageRow;
                                    tempslot.Croprange.StartCol = ImageProduct.EdgelineLeft + tempslot.slotInfo.SlotTDPosition / Calipix_x - tempslot.slotInfo.OutwardShieldLeft / Calipix_x;
                                    CropPartParams.PartCenterCol = tempslot.Croprange.StartCol + CropPartParams.PartSizeCol / 2;//槽和基准边同一边就不用+，在对侧，就用片宽减半槽宽。
                                    tempslot.Croprange.EndCol = tempslot.Croprange.StartCol + CropPartParams.PartSizeCol;

                                    bool bl_ExecuteSuccess = CropPlugin.ExecuteObj();
                                    if (bl_ExecuteSuccess)//削薄区成功
                                    {
                                        if (true)
                                        {
                                            double rowMove = (tempslot.Croprange.MidImageID - ImageProduct.nCurImageID) * InfoImgInfo.imageHeight + CropPartParams.PartCenterRow;
                                            HOperatorSet.GenRectangle2(out HObject GroVRegion1, rowMove, CropPartParams.PartCenterCol, 0, CropPartParams.PartSizeCol / 2 + CropPartParams.CropShaftCol, CropPartParams.PartSizeRow / 2 + CropPartParams.CropShaftRow);
                                            ImageProduct.ListImageSlicePD[SliceIndex].showHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = GroVRegion1, m_stuGetColor = HalconColor.pink });
                                        }

                                        string str0 = $"第{SliceIndex + 1}条第{index + 1}个打孔区裁切成功";
                                        ImageProduct.ListImageSlicePD[SliceIndex].showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo { m_strMessage = str0, m_stuGetColor = HalconColor.orange });

                                        currentEAProduct.ListEASlicePD[SliceIndex].curSlShaveIndex++;
                                        //tempslot.showHalconWindow.m_hImage = CropPartParams.Image_part;
                                        //tempslot.showHalconWindow.m_nImageFrameID = index;
                                        //tempslot.showHalconWindow.m_nProductID = PFTCleanParamBase.nCurEAProId;


                                        //添加首尾条的间隙边
                                        if (SliceIndex == 0)
                                        {
                                            //第一条左间隙边,
                                            int n = (int)(DirectionEnum.左间隙边);
                                            tempslot.Slotlines[n] = EdgeLine;
                                        }
                                        else if (SliceIndex == m_craft.SliceCount - 1)
                                        {
                                            //第一条左间隙边,
                                            int n = (int)(DirectionEnum.右间隙边);
                                            tempslot.Slotlines[n] = EdgeLine;
                                        }

                                        //找对应槽的四条边
                                        double C1 = 1000; //矩形左边
                                        double R1 = 1000; //矩形上边
                                        double C2 = 2000; //矩形右边
                                        double R2 = 2000; //矩形下边

                                        // int count = Enum.GetValues(typeof(DirectionEnum)).Length;
                                        int count = 4;
                                        bool BlSlotLine = true;
                                        for (int j = 0; j < count; j++)
                                        {
                                            string LineName = $"第{SliceIndex + 1}条打孔区" + ((DirectionEnum)j).ToString();

                                            IPlugin PluginMline = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == LineName);
                                            ParamsMeasureLine MlineParams = PluginMline.PluginParams as ParamsMeasureLine; //里氏转换
                                            MlineParams.ho_Image = CropPartParams.Image_part;
                                            LineInfo line = new LineInfo();
                                            bool isSuccess = PluginMline.ExecuteObj();
                                            string ShaveName = $"第{SliceIndex + 1}条第{index + 1}个打孔区" + ((DirectionEnum)j).ToString();
                                            DetectEdgeValueAssign(MlineParams, isSuccess, line, ShaveName, ImageProduct.ListImageSlicePD[SliceIndex].showHalconWindow.m_messageShowInfos,
                                                 ImageProduct.ListImageSlicePD[SliceIndex].showHalconWindow.m_lineShowInfos, ImageProduct.ListImageSlicePD[SliceIndex].showHalconWindow.m_regionShowInfos);

                                            // tempslot.showHalconWindow.m_lineShowInfos = LineShowInfos;
                                            //把抓边添加到ImageProduct和EAProduct
                                            double Srow = CropPartParams.PartCenterRow - CropPartParams.PartSizeRow / 2 - CropPartParams.CropShaftRow;
                                            double Scol = CropPartParams.PartCenterCol - CropPartParams.PartSizeCol / 2 - CropPartParams.CropShaftCol;
                                            LineInfo Normalline = NormalizeLine(line, (int)CropPartParams.PartCenterID, Srow, Scol, (int)InfoImgInfo.image_ID, InfoImgInfo.imageHeight);
                                            //添加边到打孔区抓边队列
                                            tempslot.Slotlines[j] = Normalline.copyLine(Normalline);
                                            //添加边到图像抓边队列
                                            ImageProduct.ListImageSlicePD[SliceIndex].ListImagelines.Add(Normalline); //开并行，不能往imageprod里面放，会冲突
                                            ///添加边到EA抓边队列
                                            LineInfo EAline = EAAddline(Normalline, currentEAProduct.CropEArange.StartImageID, currentEAProduct.CropEArange.StarImageRow, 0, InfoImgInfo.imageHeight);
                                            currentEAProduct.ListEASlicePD[SliceIndex].ListImagelines.Add(EAline);

                                            BlSlotLine &= isSuccess;
                                            //CalcuEASize(camID, InfoImgInfo.imageHeight, Normaleftline, m_craft.ListEASizes, currentEAProduct, EAProductFrom, ImageProduct.ListImageSizes);
                                            if (isSuccess)
                                            {
                                                if (j == 0) //左边,
                                                {
                                                    C1 = (MlineParams.hv_LineColumnBegin.D + MlineParams.hv_LineColumnEnd.D) / 2 + tempslot.slotInfo.InwardShieldLeft / Calipix_x;
                                                    tempslot.SlotSizes[0].DataResult = ((Normalline.ColBegin - ImageProduct.EdgelineLeft) + (Normalline.ColEnd - ImageProduct.EdgelineLeft)) * 0.5 * Calipix_x;  //td定位距

                                                    if (SliceIndex != 0) //把除第一条的左边，传递给上一条的右间隙边
                                                    {
                                                        //右间隙边,
                                                        int n = (int)(DirectionEnum.右间隙边);
                                                        currentEAProduct.ListEASlicePD[SliceIndex - 1].listShave[index].Slotlines[n] = Normalline.copyLine(Normalline);
                                                    }
                                                }
                                                else if (j == 1) //右边,
                                                {
                                                    C2 = (MlineParams.hv_LineColumnBegin.D + MlineParams.hv_LineColumnEnd.D) / 2 - tempslot.slotInfo.InwardShieldRight / Calipix_x;
                                                    tempslot.SlotSizes[2].DataResult = (C2 - C1) * Calipix_x;//td尺寸

                                                    if (SliceIndex != m_craft.SliceCount - 1)//把除第最后一条的右边，传递给上一条的左间隙边
                                                    {
                                                        //左间隙边,
                                                        int n = (int)(DirectionEnum.左间隙边);
                                                        currentEAProduct.ListEASlicePD[SliceIndex + 1].listShave[index].Slotlines[n] = Normalline.copyLine(Normalline);
                                                    }
                                                }
                                                else if (j == 2) //上边,
                                                {
                                                    R1 = (MlineParams.hv_LineRowBegin.D + MlineParams.hv_LineRowEnd.D) / 2 + tempslot.slotInfo.InwardShieldUp / Calipix_y;

                                                    double subRowS = (Normalline.RowBeginID - currentEAProduct.EArange.StartImageID) * InfoImgInfo.imageHeight + Normalline.RowBegin - currentEAProduct.EArange.StarImageRow;
                                                    double subRowE = (Normalline.RowEndID - currentEAProduct.EArange.StartImageID) * InfoImgInfo.imageHeight + Normalline.RowEnd - currentEAProduct.EArange.StarImageRow;
                                                    tempslot.SlotSizes[1].DataResult = (subRowS + subRowE) * 0.5 * Calipix_y;  //td定位距
                                                }
                                                else if (j == 3) //下边,
                                                {
                                                    R2 = (MlineParams.hv_LineRowBegin.D + MlineParams.hv_LineRowEnd.D) / 2 - tempslot.slotInfo.InwardShieldDown / Calipix_y;
                                                    tempslot.SlotSizes[3].DataResult = (R2 - R1) * Calipix_y;//Md尺寸
                                                }
                                            }
                                        }


                                        //检测对应削薄区的缺陷，
                                        //执行缺陷检测。
                                        IPlugin PluginSlotefects = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == $"第{SliceIndex + 1}条打孔区缺陷检测");
                                        ParamsMQDefects _ParamsSlotefects = PluginSlotefects.PluginParams as ParamsMQDefects;
                                        if (BlSlotLine && C1 < C2 && R1 < R2)
                                        {
                                            //添加槽位检测缩进 current3DProduct.listEAShave[index]
                                            HOperatorSet.GenRectangle1(out HObject GroVRegion1, R1, C1, R2, C2);
                                            _ParamsSlotefects.DetectionROI = GroVRegion1;

                                            int STid = tempslot.Croprange.StartImageID;
                                            double STrow = tempslot.Croprange.StarImageRow - CropPartParams.CropShaftRow;
                                            double STcol = tempslot.Croprange.StartCol - CropPartParams.CropShaftCol;
                                            HObject GroVRegionMoved = NormalizeRegion(GroVRegion1, STid, STrow, STcol, (int)InfoImgInfo.image_ID, InfoImgInfo.imageHeight);
                                            ImageProduct.ListImageSlicePD[SliceIndex].showHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = GroVRegionMoved, m_stuGetColor = HalconColor.cyan });
                                        }
                                        else
                                        {
                                            HOperatorSet.GenRectangle1(out HObject GroVRegion1, CropPartParams.CropShaftRow, CropPartParams.CropShaftCol, CropPartParams.PartSizeRow + CropPartParams.CropShaftRow, CropPartParams.PartSizeCol + CropPartParams.CropShaftCol);
                                            _ParamsSlotefects.DetectionROI = GroVRegion1;
                                            int STid = tempslot.Croprange.StartImageID;
                                            double STrow = tempslot.Croprange.StarImageRow - CropPartParams.CropShaftRow;
                                            double STcol = tempslot.Croprange.StartCol - CropPartParams.CropShaftCol;
                                            HObject GroVRegionMoved = NormalizeRegion(GroVRegion1, STid, STrow, STcol, (int)InfoImgInfo.image_ID, InfoImgInfo.imageHeight);
                                            ImageProduct.ListImageSlicePD[SliceIndex].showHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = GroVRegionMoved, m_stuGetColor = HalconColor.cyan });
                                        }
                                        HOperatorSet.GenEmptyObj(out _ParamsSlotefects.ShieldROI);
                                        _ParamsSlotefects.ho_Image = CropPartParams.Image_part;
                                        _ParamsSlotefects.WidthCali = Calipix_x;
                                        _ParamsSlotefects.HeightCali = Calipix_y;
                                        PluginSlotefects.ExecuteObj();

                                        //整合缺陷检测结果。整合缺陷到EA。
                                        while (_ParamsSlotefects.detectInfos.Count() > 0)
                                        {
                                            NgTypeInfo item = _ParamsSlotefects.detectInfos?.Dequeue();
                                            item.m_nImgIndex = (int)InfoImgInfo.image_ID;
                                            item.m_iProductIndex = PFTCleanParamBase.nCurEAProId;
                                            item.m_iSliceIndex = SliceIndex;
                                            int STid = tempslot.Croprange.StartImageID;
                                            double STrow = tempslot.Croprange.StarImageRow - CropPartParams.CropShaftRow;
                                            double STcol = tempslot.Croprange.StartCol - CropPartParams.CropShaftCol;
                                            HObject GroVRegionMoved = NormalizeRegion(item.m_hRegion, STid, STrow, STcol, (int)InfoImgInfo.image_ID, InfoImgInfo.imageHeight);
                                            item.m_hRegion = GroVRegionMoved;
                                            ImageProduct.ListImageSlicePD[SliceIndex].ListImageDefects.Add(item);

                                            NgTypeInfo EAngInfo = new NgTypeInfo();
                                            EAngInfo = item.Copy();
                                            HOperatorSet.CopyObj(GroVRegionMoved, out HObject region, 1, 1);
                                            int EASTid = currentEAProduct.CropEArange.StartImageID;
                                            double EASTrow = currentEAProduct.CropEArange.StarImageRow;
                                            HObject RegionMoved = EAAddRegion(region, EASTid, EASTrow, (int)InfoImgInfo.image_ID, InfoImgInfo.imageHeight);
                                            EAngInfo.m_hRegion = RegionMoved;
                                            currentEAProduct.ListEASlicePD[SliceIndex].ListImageDefects.Add(EAngInfo);
                                        }
                                    }
                                    else //先做没裁图成功就跳出循环。后期再加帧号防呆。
                                    {
                                        ToDoShaveCrop = false;
                                    }
                                }

                            }

                            //每条左右边均分。
                            double left = ImageProduct.EdgelineLeft + m_craft.ListSliceSet[SliceIndex].SliceStartPos / Calipix_x;
                            double right = ImageProduct.EdgelineLeft + m_craft.ListSliceSet[SliceIndex].SliceEndPos / Calipix_x;
                            double TDshaft = m_craft.ListSliceSet[SliceIndex].SliceStartPos / Calipix_x;
                            //检测下个EA的涂布缺陷。
                            if (ImageProduct.blSplitEA)
                            {
                                HObject GroVRegion1 = CreatCoatBlock(InfoImgInfo, PFTCleanParamBase.dic_EAproductsInfo[PFTCleanParamBase.nCurEAProId + 1], SliceIndex, left, right, TDshaft, Calipix_y, Calipix_x);
                                ImageProduct.ListImageSlicePD[SliceIndex].showHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = GroVRegion1, m_stuGetColor = HalconColor.green });
                                //执行缺陷检测。
                                ParamsMQDefects _ParamsSlotefects = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == $"第{SliceIndex + 1}条涂布区缺陷检测").PluginParams as ParamsMQDefects;
                                _ParamsSlotefects.DetectionROI = GroVRegion1;
                                HOperatorSet.GenEmptyObj(out _ParamsSlotefects.ShieldROI);
                                _ParamsSlotefects.ho_Image = InfoImgInfo.ho_image;
                                _ParamsSlotefects.WidthCali = Calipix_x;
                                _ParamsSlotefects.HeightCali = Calipix_y;
                                process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == $"第{SliceIndex + 1}条涂布区缺陷检测").ExecuteObj();
                                //整合缺陷检测结果。
                                while (_ParamsSlotefects.detectInfos.Count() > 0)
                                {
                                    NgTypeInfo item = _ParamsSlotefects.detectInfos?.Dequeue();
                                    item.m_nImgIndex = (int)InfoImgInfo.image_ID;
                                    item.m_iProductIndex = PFTCleanParamBase.nCurEAProId + 1;
                                    item.m_iSliceIndex = SliceIndex;
                                    ImageProduct.ListImageSlicePD[SliceIndex].ListImageDefects.Add(item);

                                    NgTypeInfo EAngInfo = new NgTypeInfo();
                                    EAngInfo = item.Copy();
                                    HOperatorSet.CopyObj(item.m_hRegion, out HObject region, 1, 1);
                                    int EASTid = PFTCleanParamBase.dic_EAproductsInfo[PFTCleanParamBase.nCurEAProId + 1].CropEArange.StartImageID;
                                    double EASTrow = PFTCleanParamBase.dic_EAproductsInfo[PFTCleanParamBase.nCurEAProId + 1].CropEArange.StarImageRow;
                                    HObject RegionMoved = EAAddRegion(region, EASTid, EASTrow, (int)InfoImgInfo.image_ID, InfoImgInfo.imageHeight);
                                    EAngInfo.m_hRegion = RegionMoved;
                                    PFTCleanParamBase.dic_EAproductsInfo[PFTCleanParamBase.nCurEAProId + 1].ListEASlicePD[SliceIndex].ListImageDefects.Add(EAngInfo);
                                }
                            }

                            //检测当前EA涂布缺陷
                            if (true)
                            {
                                //生成矩形检测区域
                                HObject GroVRegion1 = CreatCoatBlock(InfoImgInfo, currentEAProduct, SliceIndex, left, right, TDshaft, Calipix_y, Calipix_x);
                                ImageProduct.ListImageSlicePD[SliceIndex].showHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = GroVRegion1, m_stuGetColor = HalconColor.yellow });
                                //执行缺陷检测。
                                ParamsMQDefects _ParamsSlotefects = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == $"第{SliceIndex + 1}条涂布区缺陷检测").PluginParams as ParamsMQDefects;
                                _ParamsSlotefects.DetectionROI = GroVRegion1;
                                HOperatorSet.GenEmptyObj(out _ParamsSlotefects.ShieldROI);
                                _ParamsSlotefects.ho_Image = InfoImgInfo.ho_image;
                                _ParamsSlotefects.WidthCali = Calipix_x;
                                _ParamsSlotefects.HeightCali = Calipix_y;
                                process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == $"第{SliceIndex + 1}条涂布区缺陷检测").ExecuteObj();
                                //整合缺陷检测结果。
                                while (_ParamsSlotefects.detectInfos.Count() > 0)
                                {
                                    NgTypeInfo item = _ParamsSlotefects.detectInfos?.Dequeue();
                                    item.m_nImgIndex = (int)InfoImgInfo.image_ID;
                                    item.m_iProductIndex = PFTCleanParamBase.nCurEAProId;
                                    item.m_iSliceIndex = SliceIndex;
                                    ImageProduct.ListImageSlicePD[SliceIndex].ListImageDefects.Add(item);

                                    NgTypeInfo EAngInfo = new NgTypeInfo();
                                    EAngInfo = item.Copy();
                                    HOperatorSet.CopyObj(item.m_hRegion, out HObject region, 1, 1);
                                    HOperatorSet.AreaCenter(item.m_hRegion, out HTuple area, out HTuple row, out HTuple column);
                                    HOperatorSet.AreaCenter(region, out area, out row, out column);
                                    int EASTid = currentEAProduct.CropEArange.StartImageID;
                                    double EASTrow = currentEAProduct.CropEArange.StarImageRow;

                                    HObject RegionMoved = EAAddRegion(region, EASTid, EASTrow, (int)InfoImgInfo.image_ID, InfoImgInfo.imageHeight);
                                    HOperatorSet.AreaCenter(RegionMoved, out area, out row, out column);
                                    EAngInfo.m_hRegion = RegionMoved;
                                    currentEAProduct.ListEASlicePD[SliceIndex].ListImageDefects.Add(EAngInfo);
                                }
                            }
                        }
                        // }
                    });

                    //料尾裁图，抓边
                    if (!currentEAProduct.BlTileDone && InfoImgInfo.image_ID >= currentEAProduct.Coatrange.EndImageID) //图像大于等于料尾裁图的帧号
                    {
                        IPlugin CropPlugin = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == "料尾裁切");
                        ParamsCropPart CropPartParams = CropPlugin.PluginParams as ParamsCropPart; //里氏转换
                        CropPartParams.list_ImageInfo = PFTCleanParamBase.imageInfos;

                        CropPartParams.PartSizeRow = m_craft.EACropShiledMD;
                        CropPartParams.PartSizeCol = 1000;
                        CropPartParams.PartCenterID = currentEAProduct.Coatrange.EndImageID;
                        CropPartParams.PartCenterRow = currentEAProduct.Coatrange.EndImageRow;
                        CropPartParams.PartCenterCol = ImageProduct.EdgelineLeft + 1000;//槽和基准边同一边就不用+，在对侧，就用片宽减半槽宽。
                        bool bl_ExecuteSuccess = CropPlugin.ExecuteObj();
                        if (bl_ExecuteSuccess)
                        {
                            if (currentEAProduct.Coatrange.EndImageID == ImageProduct.nCurImageID)
                            {
                                HOperatorSet.GenRectangle2(out HObject GroVRegion1, CropPartParams.PartCenterRow, CropPartParams.PartCenterCol, 0, CropPartParams.PartSizeCol / 2 + CropPartParams.CropShaftCol, CropPartParams.PartSizeRow / 2 + CropPartParams.CropShaftRow);
                                RegionShowInfos.Add(new RegionShowInfo() { m_hRegion = GroVRegion1, m_stuGetColor = HalconColor.pink });
                            }
                            string str0 = $"产品{currentEAProduct.EAID}料尾裁切成功";
                            MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = str0, m_stuGetColor = HalconColor.orange });

                            currentEAProduct.BlTileDone = true;
                            IPlugin PluginMline = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == "涂布料尾边");
                            ParamsMeasureLine MlineParams = PluginMline.PluginParams as ParamsMeasureLine; //里氏转换
                            MlineParams.ho_Image = CropPartParams.Image_part;
                            LineInfo line = new LineInfo();
                            bool isSuccess = PluginMline.ExecuteObj();
                            DetectEdgeValueAssign(MlineParams, isSuccess, line, PluginMline.moduleInfo.ModuleName, MesssgeShowInfos, LineShowInfos, RegionShowInfos);

                            //把抓边添加到ImageProduct和EAProduct
                            double Srow = CropPartParams.PartCenterRow - CropPartParams.PartSizeRow / 2 - CropPartParams.CropShaftRow;
                            double Scol = CropPartParams.PartCenterCol - CropPartParams.PartSizeCol / 2 - CropPartParams.CropShaftCol;
                            LineInfo Normalline = NormalizeLine(line, (int)CropPartParams.PartCenterID, Srow, Scol, (int)InfoImgInfo.image_ID, InfoImgInfo.imageHeight);
                            ImageProduct.ListImagelines.Add(Normalline);
                            LineInfo EAline = EAAddline(Normalline, currentEAProduct.CropEArange.StartImageID, currentEAProduct.CropEArange.StarImageRow, 0, InfoImgInfo.imageHeight);
                            currentEAProduct.ListEAlines.Add(EAline);
                        }
                    }

                    ///复制图像给EA拼大图流程

                    HOperatorSet.CopyObj(InfoImgInfo.ho_image, out HObject CopyImage, 1, 1);
                    HOperatorSet.GetImageSize(InfoImgInfo.ho_image, out HTuple wi, out HTuple hi);
                    PFTCleanParamBase.PFTImageProdQueue.Enqueue(new ImageInfo() { image_ID = InfoImgInfo.image_ID, EAID = PFTCleanParamBase.nCurEAProId, ho_image = CopyImage, imageWidth = wi, imageHeight = hi });
                    PFTCleanParamBase.semaphoreSlim.Release();

                    if (ImageProduct.blSplitEA)
                    {
                        PFTCleanParamBase.nCurEAProId++;
                    }

                    ImageProduct.showHalconWindow.m_nProductID = ImageProduct.nCurEAID;
                    ImageProduct.showHalconWindow.m_nImageFrameID = InfoImgInfo.image_ID;
                    ImageProduct.showHalconWindow.m_hImage = InfoImgInfo.ho_image;
                    ImageProduct.showHalconWindow.m_lineShowInfos = ImageProduct.ListImagelines;
                    ImageProduct.showHalconWindow.m_regionShowInfos = RegionShowInfos;
                    ImageProduct.showHalconWindow.m_messageShowInfos = MesssgeShowInfos;
                    PFTCleanParamBase.ProductCallBack(ImageProduct);
                    MessageNotify.AddDebugLog($"Cam1帧图流程产品{ImageProduct.nCurEAID}图像{InfoImgInfo.image_ID}处理完成");
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }

        public void ProProcessBaseEA(Process process)
        {
            try
            {
                int ProcessID = process.processInfo.ProcessID;
                int camID = 0;
                SetThreadAffinityMask(GetCurrentThread(), new UIntPtr(SetCpuID(ProcessID)));

                //标定系数
                double Calipix_y = Project.Instance.cameraManager[(CameraEnum)camID].m_CamParam.CalibrationValue_Y;
                double Calipix_x = Project.Instance.cameraManager[(CameraEnum)camID].m_CamParam.CalibrationValue_X;
                double ABMis = Project.Instance.cameraManager[(CameraEnum)1].m_CamParam.ToMainCamDistance / Calipix_y;//到主相机像素距离
                                                                                                                      // bool ExecuteSuccess = false;
                                                                                                                      // 选择参数和队列基于是否模拟
                double ZoomFactorX = 0.5;
                double ZoomFactorY = 0.5;
                PFTCleanProParams CleanEAParamBase = Project.Instance.Is_simulation ? m_DicCamOperaParam_Sim[camID] as PFTCleanProParams : m_DicCamOperaParam[camID] as PFTCleanProParams;
                var PFTimageQueue = CleanEAParamBase.PFTImageProdQueue;
                CleanEAParamBase.semaphoreSlim.Wait();

                ///复制图像给EA拼大图流程
                if (PFTimageQueue.Count > 2) // 检查主相机的图像队列是否有图像
                {
                    //主相机图像参数
                    ImageInfo InfoImgInfo = PFTimageQueue.Dequeue();

                    CleanEAParamBase.EAlistimageInfos.Add(InfoImgInfo);
                    if (CleanEAParamBase.EAlistimageInfos.Count > 3)
                    {
                        long removeID = InfoImgInfo.image_ID - 3;
                        var tempIMfo = CleanEAParamBase.EAlistimageInfos.FindIndex(c => c.image_ID == removeID);
                        if (tempIMfo != -1)
                        {
                            CleanEAParamBase.EAlistimageInfos[tempIMfo]?.Dispose();
                            CleanEAParamBase.EAlistimageInfos.RemoveAt(tempIMfo);
                            //获取CleanEAParamBase.EAlistimageInfos中所有的image_ID
                            var key = CleanEAParamBase.EAlistimageInfos.Select(c => c.image_ID).ToList();
                            int count = InfoImgInfo.ho_image.GetReferenceCount();
                            MessageNotify.AddDebugLog($"Cam1EA流程产品{CleanEAParamBase.nCurShowEAProId}图像{InfoImgInfo.image_ID}移除图像ID{removeID}移除成功,队列中超过10帧图像,图像被引用次数{count}，剩余{string.Join(",", key)}");
                        }
                        else
                        {
                            MessageNotify.AddErrorLog($"Cam1EA流程产品{CleanEAParamBase.nCurShowEAProId}图像{InfoImgInfo.image_ID}图移除图像ID{removeID}处理失败,队列中没有前10帧图像");
                        }
                    }
                    //获取CleanEAParamBase.dic_EAproductsInfo所有的key值
                    List<int> keys = CleanEAParamBase.dic_EAproductsInfo.Keys.ToList();
                    MessageNotify.AddDebugLog($"Cam1EA流程产品{CleanEAParamBase.nCurShowEAProId}图像{InfoImgInfo.image_ID}图像EAID{InfoImgInfo.EAID}处理开始,当前key值列表:{string.Join(",", keys)}");
                    PFTCleanEAProductInfo currentEAProduct = CleanEAParamBase.dic_EAproductsInfo[CleanEAParamBase.nCurShowEAProId];

                    if (InfoImgInfo.EAID > CleanEAParamBase.nCurShowEAProId)
                    {
                        currentEAProduct.blEndTotalEAImage = true;
                    }


                    List<MessageShowInfo> MesssgeShowInfos = new List<MessageShowInfo>();
                    List<RegionShowInfo> RegionShowInfos = new List<RegionShowInfo>();
                    List<LineInfo> LineShowInfos = new List<LineInfo>();
                    //计时            
                    HOperatorSet.CountSeconds(out currentEAProduct.Image_StartTime);
                    try
                    {
                        int StartID = currentEAProduct.CropEArange.StartImageID > 1 ? currentEAProduct.CropEArange.StartImageID : 1;
                        //如果已经拼接了料头图，就跳过，如果未拼接料头图，就裁拼料头图
                        if (!currentEAProduct.blStartTotalEAImage)
                        {
                            //当前帧不是料头裁切帧，则裁切帧应该是当前帧上一帧，在上一帧裁出料头，再拼接当前帧整个，作为EA全图。
                            if (StartID < InfoImgInfo.image_ID)
                            {
                                ImageInfo InfoImgInfo0 = CleanEAParamBase.EAlistimageInfos.Find(c => c.image_ID == StartID);
                                try
                                {
                                    if (InfoImgInfo0 != null)
                                    {
                                        //HOperatorSet.CropRectangle1(InfoImgInfo0.ho_image, out currentEAProduct.TotalEAImage, currentEAProduct.CropEArange.StarImageRow - 1, 0, InfoImgInfo.imageHeight - 1, InfoImgInfo.imageWidth - 1);

                                        HOperatorSet.CropRectangle1(InfoImgInfo0.ho_image, out HObject ZooImage, currentEAProduct.CropEArange.StarImageRow - 1, 0, InfoImgInfo.imageHeight - 1, InfoImgInfo.imageWidth - 1);
                                        HOperatorSet.ZoomImageFactor(ZooImage, out currentEAProduct.EAImageCollection, ZoomFactorX, ZoomFactorY, "bilinear");
                                    }
                                    else
                                    {
                                        // currentEAProduct.TotalEAImage = InfoImgInfo.ho_image;
                                        HOperatorSet.ZoomImageFactor(InfoImgInfo.ho_image, out HObject ZooImage, ZoomFactorX, ZoomFactorY, "bilinear");
                                        currentEAProduct.EAImageCollection = ZooImage;
                                    }
                                    StartID++;
                                    while (StartID <= InfoImgInfo.image_ID)
                                    {
                                        ImageInfo InfoImgInfo1 = CleanEAParamBase.EAlistimageInfos.Find(c => c.image_ID == StartID);
                                        if (InfoImgInfo1 != null)
                                        {
                                            //TileImage(currentEAProduct.TotalEAImage, InfoImgInfo1.ho_image, out currentEAProduct.TotalEAImage);
                                            HOperatorSet.ZoomImageFactor(InfoImgInfo1.ho_image, out HObject ZooImage, ZoomFactorX, ZoomFactorY, "bilinear");
                                            HOperatorSet.ConcatObj(currentEAProduct.EAImageCollection, ZooImage, out currentEAProduct.EAImageCollection);
                                        }
                                        StartID++;
                                        //try
                                        //{

                                        //}
                                        //catch (Exception ee)
                                        //{
                                        //    int ccount = InfoImgInfo1.ho_image.GetReferenceCount();
                                        //    MessageNotify.AddDebugLog("InfoImgInfo1:" + ee.ToString());
                                        //}


                                    }
                                    currentEAProduct.blStartTotalEAImage = true;
                                }
                                catch (Exception ee)
                                {
                                    MessageNotify.AddDebugLog("InfoImgInfo0:" + ee.ToString());
                                    //throw;
                                }


                            }
                            else if (StartID == InfoImgInfo.image_ID)//当前帧就是料头裁切帧，则直接裁出部分作为EA全图，承接后续拼图
                            {
                                //  HOperatorSet.CropRectangle1(InfoImgInfo.ho_image, out currentEAProduct.TotalEAImage, currentEAProduct.CropEArange.StarImageRow - 1, 0, InfoImgInfo.imageHeight - 1, InfoImgInfo.imageWidth - 1);
                                HOperatorSet.CropRectangle1(InfoImgInfo.ho_image, out HObject ZooImage, currentEAProduct.CropEArange.StarImageRow - 1, 0, InfoImgInfo.imageHeight - 1, InfoImgInfo.imageWidth - 1);
                                HOperatorSet.ZoomImageFactor(ZooImage, out currentEAProduct.EAImageCollection, ZoomFactorX, ZoomFactorY, "bilinear");

                                currentEAProduct.blStartTotalEAImage = true;
                            }
                        }
                        //裁拼EA图像，
                        else if (InfoImgInfo.image_ID < currentEAProduct.CropEArange.EndImageID) //当前图像帧ID  小于裁图结束ID，则把EA全图拼上整个当前帧。
                        {
                            //  TileImage(currentEAProduct.TotalEAImage, InfoImgInfo.ho_image, out currentEAProduct.TotalEAImage);
                            HOperatorSet.ZoomImageFactor(InfoImgInfo.ho_image, out HObject ZooImage, ZoomFactorX, ZoomFactorY, "bilinear");
                            HOperatorSet.ConcatObj(currentEAProduct.EAImageCollection, ZooImage, out currentEAProduct.EAImageCollection);
                        }
                        else if (InfoImgInfo.image_ID == currentEAProduct.CropEArange.EndImageID)//当前图像帧ID  大于等于 裁图结束ID，则把EA全图拼上局部当前帧。当前EA就已经拼图完成。开始拼下一个EA的EA全图
                        {
                            HOperatorSet.CropRectangle1(InfoImgInfo.ho_image, out HObject Image_part, 0, 0, currentEAProduct.CropEArange.EndImageRow - 1, InfoImgInfo.imageWidth - 1);
                            // TileImage(currentEAProduct.TotalEAImage, Image_part, out currentEAProduct.TotalEAImage);
                            HOperatorSet.ZoomImageFactor(Image_part, out HObject ZooImage, ZoomFactorX, ZoomFactorY, "bilinear");
                            HOperatorSet.ConcatObj(currentEAProduct.EAImageCollection, ZooImage, out currentEAProduct.EAImageCollection);
                            Image_part.Dispose();
                            // currentEAProduct.blEndTotalEAImage = true;
                        }

                        if (currentEAProduct.blEndTotalEAImage)
                        {
                            TileImage2Offset(currentEAProduct.EAImageCollection, out currentEAProduct.TotalEAImage);
                            foreach (NgTypeInfo item in currentEAProduct.ListEADefects)
                            {
                                HOperatorSet.ZoomRegion(item.m_hRegion, out HObject defect, ZoomFactorX, ZoomFactorY);
                                item.m_hRegion = defect;
                            }
                            for (int i = 0; i < m_craft.SliceCount; i++)
                            {
                                foreach (NgTypeInfo item in currentEAProduct.ListEASlicePD[i].ListImageDefects)
                                {
                                    HOperatorSet.ZoomRegion(item.m_hRegion, out HObject defect, ZoomFactorX, ZoomFactorY);
                                    item.m_hRegion = defect;
                                }
                            }
                            CleanEAParamBase.nCurShowEAProId = InfoImgInfo.EAID;
                            currentEAProduct.EAImageCollection?.DisposeEx();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageNotify.AddErrorLog(ex.ToString());
                    }
                    string str = ((CameraEnum)camID).ToString() + "信号量--" + CleanEAParamBase.semaphoreSlimCam.CurrentCount.ToString() + "图像数--" + PFTimageQueue.Count.ToString();
                    MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = str, m_stuGetColor = HalconColor.green });
                    MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = $"当前产品对应的产品号：{currentEAProduct.EAID}", m_stuGetColor = HalconColor.green });

                    //计时            
                    HOperatorSet.CountSeconds(out currentEAProduct.Image_FindLineTime);
                    HOperatorSet.CountSeconds(out currentEAProduct.Image_FindDefectTime);
                    HOperatorSet.CountSeconds(out currentEAProduct.Image_ArithmeticProcessTime);
                    //MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = "处理时长：" + HtEnd.D.ToString("0.00") + "ms", m_stuGetColor = HalconColor.green });
                    double hTime5 = ((currentEAProduct.Image_ArithmeticProcessTime - currentEAProduct.Image_StartTime) * 1000).D;//总耗时
                    MessageNotify.AddDebugLog($"EA：{currentEAProduct.EAID}-" + $"ShowEA：{CleanEAParamBase.nCurShowEAProId}-" + $"图：{InfoImgInfo.EAID}-" + $"耗时：{hTime5}ms");

                    currentEAProduct.showHalconWindow.m_nProductID = currentEAProduct.EAID;
                    currentEAProduct.showHalconWindow.m_hImage = currentEAProduct.TotalEAImage;
                    currentEAProduct.showHalconWindow.m_lineShowInfos = LineShowInfos;
                    currentEAProduct.showHalconWindow.m_regionShowInfos = RegionShowInfos;
                    currentEAProduct.showHalconWindow.m_messageShowInfos = MesssgeShowInfos;

                    if (currentEAProduct.blEndTotalEAImage)
                    {
                        CleanEAParamBase.ProductCallBackCraft(currentEAProduct);
                        CleanEAParamBase.nCurShowEAProId = InfoImgInfo.EAID;
                        MessageNotify.AddDebugLog($"Cam1EA流程产品{currentEAProduct.EAID}nCurShowEAProId{CleanEAParamBase.nCurShowEAProId}图像{InfoImgInfo.image_ID}处理完成");
                    }
                }


            }
            catch (Exception ex)
            {
                MessageNotify.AddErrorLog("Cam1EA流程产品报错：" + ex.Message);

            }
        }

        public void ProProcessFromCam(Process process)
        {
            try
            {
                int ProcessID = process.processInfo.ProcessID;
                int camID = (int)CameraEnum.Cam2;
                SetThreadAffinityMask(GetCurrentThread(), new UIntPtr(SetCpuID(ProcessID)));

                //标定系数
                double Calipix_y = Project.Instance.cameraManager[(CameraEnum)camID].m_CamParam.CalibrationValue_Y;
                double Calipix_x = Project.Instance.cameraManager[(CameraEnum)camID].m_CamParam.CalibrationValue_X;
                double ABMis = Project.Instance.cameraManager[(CameraEnum)camID].m_CamParam.ToMainCamDistance / Calipix_y;//到主相机像素距离


                // 选择参数和队列基于是否模拟
                PFTCleanProParams PFTCleanParamBase = Project.Instance.Is_simulation ? m_DicCamOperaParam_Sim[camID - 1] as PFTCleanProParams : m_DicCamOperaParam[camID - 1] as PFTCleanProParams;
                PFTCleanProParams PFTCleanParamFrom = Project.Instance.Is_simulation ? m_DicCamOperaParam_Sim[camID] as PFTCleanProParams : m_DicCamOperaParam[camID] as PFTCleanProParams;
                var imageQueue = PFTCleanParamFrom.alreadyDealGrabImage;
                PFTCleanParamFrom.semaphoreSlimCam.Wait();

                if (imageQueue.Count > 0) // 检查主相机的图像队列是否有图像  
                {
                    // PFTCleanEAProductInfo EAProductBase = PFTCleanParamBase.dic_EAproductsInfo[PFTCleanParamFrom.nCurEAProId];
                    PFTCleanEAProductInfo currentEAProduct = PFTCleanParamFrom.dic_EAproductsInfo[PFTCleanParamFrom.nCurEAProId];

                    PFTImageProduct ImageProduct;
                    ImageInfo InfoImgInfo = imageQueue.Dequeue();
                    PFTCleanParamFrom.imageInfos.Add(InfoImgInfo);
                    if (PFTCleanParamFrom.imageInfos.Count > 8)
                    {
                        //PFTCleanParamFrom.imageInfos.First().Dispose();
                        PFTCleanParamFrom.imageInfos.RemoveAt(0);
                    }

                    List<MessageShowInfo> MesssgeShowInfos = new List<MessageShowInfo>();
                    List<RegionShowInfo> RegionShowInfos = new List<RegionShowInfo>();
                    List<LineInfo> LineShowInfos = new List<LineInfo>();

                    int count0 = InfoImgInfo.ho_image.GetReferenceCount();
                    ImageProduct = new PFTImageProduct();
                    for (int i = 0; i < m_craft.SliceCount; i++)
                    {
                        ImageProduct.ListImageSlicePD.Add(new PFTSliceProduct());
                    }
                    ImageProduct.nCurImageID = InfoImgInfo.image_ID;
                    ImageProduct.nCurEAID = PFTCleanParamFrom.nCurEAProId;
                    //InfoImgInfo.ho_image.CopyImage(out HObject dupImage);
                    //int count1 = dupImage.GetReferenceCount();
                    string str = ((CameraEnum)camID).ToString() + "信号量--" + PFTCleanParamFrom.semaphoreSlimCam.CurrentCount.ToString() + "图像数--" + imageQueue.Count.ToString();
                    MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = str, m_stuGetColor = HalconColor.green });
                    MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = $"当前产品编号{currentEAProduct.EAID}，当前图像帧号：{ImageProduct.nCurImageID}", m_stuGetColor = HalconColor.green });
                    //开始检测计时            
                    HOperatorSet.CountSeconds(out ImageProduct.Image_StartTime);

                    //确认基准边 L+R ,必须有的两条边--
                    LineInfo EdgeLine = new LineInfo();
                    if (true)
                    //for (int i = 0; i < 2; i++)
                    {
                        string linepluginname = "涂布左基准边";
                        if (camID == 1) linepluginname = "涂布右基准边";
                        //抓边
                        IPlugin PluginMline = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == linepluginname);
                        ParamsMeasureLine MlineParams = PluginMline.PluginParams as ParamsMeasureLine; //里氏转换
                        MlineParams.ho_Image = InfoImgInfo.ho_image;
                        LineInfo line = new LineInfo();
                        bool isSuccess = PluginMline.ExecuteObj();
                        DetectEdgeValueAssign(MlineParams, isSuccess, line, linepluginname, MesssgeShowInfos, LineShowInfos, RegionShowInfos);

                        //把抓边添加到ImageProduct和EAProduct
                        LineInfo Normalline = NormalizeLine(line, (int)ImageProduct.nCurImageID, 0, 0, (int)InfoImgInfo.image_ID, InfoImgInfo.imageHeight);
                        ImageProduct.ListImagelines.Add(Normalline);
                        EdgeLine = EdgeLine.copyLine(Normalline);
                        LineInfo EAline = EAAddline(Normalline, currentEAProduct.CropEArange.StartImageID, currentEAProduct.CropEArange.StarImageRow, 0, InfoImgInfo.imageHeight);
                        currentEAProduct.ListEAlines.Add(EAline);

                        if (isSuccess)
                        {
                            if (camID == 0)
                            {
                                ImageProduct.EdgelineLeft = (MlineParams.hv_LineColumnBegin.D + MlineParams.hv_LineColumnEnd.D) / 2;
                                ImageProduct.EdgelineRight = InfoImgInfo.imageWidth;
                            }
                            else
                            {
                                ImageProduct.EdgelineRight = (MlineParams.hv_LineColumnBegin.D + MlineParams.hv_LineColumnEnd.D) / 2;
                                double EdgeShaft = ImageProduct.EdgelineRight - m_craft.Cam2BaseCol;
                                ImageProduct.EdgelineLeft = -(m_craft.EXCam1Col - m_craft.EXCam2Col - m_craft.Cam1BaseCol - EdgeShaft);
                            }
                        }
                    }

                    if (true)
                    {
                        ////找料头
                        //IPlugin Plugin = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == $"{(CameraEnum)camID}找料头");
                        //ParamsFindHead findHeadParams = Plugin.PluginParams as ParamsFindHead; //里氏转换
                        //findHeadParams.list_ImageInfo = PFTCleanParamFrom.imageInfos;
                        //findHeadParams.ImageNowID = (int)InfoImgInfo.image_ID;
                        //bool bl_findHeadSuccess = Plugin.ExecuteObj();
                        //if (bl_findHeadSuccess)
                        //{
                        //    if (findHeadParams.hv_ImageHeadBeginID == findHeadParams.ImageNowID)
                        //    {
                        //        HOperatorSet.GenRectangle1(out HObject GroVRegion1, findHeadParams.hv_ImageHeadBeginRow, findHeadParams.hv_ImageHeadBeginCol, findHeadParams.hv_ImageHeadEndRow, findHeadParams.hv_ImageHeadEndCol);
                        //        RegionShowInfos.Add(new RegionShowInfo() { m_hRegion = GroVRegion1, m_stuGetColor = HalconColor.green });
                        //    }
                        //    LineInfo HeadlineInfo = new LineInfo();
                        //    HeadlineInfo.StrLineName = "涂布料头边";
                        //    HeadlineInfo.IsLineSuccessfull = true;
                        //    HeadlineInfo.RowBeginID = (int)InfoImgInfo.image_ID;
                        //    HeadlineInfo.RowBegin = (findHeadParams.headLineID - HeadlineInfo.RowBeginID) * InfoImgInfo.imageHeight + findHeadParams.headLine;
                        //    HeadlineInfo.ColBegin = 100;
                        //    HeadlineInfo.RowEndID = (int)InfoImgInfo.image_ID;
                        //    HeadlineInfo.RowEnd = (findHeadParams.headLineID - HeadlineInfo.RowBeginID) * InfoImgInfo.imageHeight + findHeadParams.headLine;
                        //    HeadlineInfo.ColEnd = InfoImgInfo.imageWidth - 100;
                        //    HeadlineInfo.m_nLineWidth = 10;
                        //    HeadlineInfo.Color = HalconColor.pink;
                        //    LineShowInfos.Add(HeadlineInfo);
                        //    string str0 = $"B相机找料头成功，产品编号：{PFTCleanParamFrom.nCurEAProId + 1},料头位置：第{findHeadParams.headLineID}帧-第{findHeadParams.headLine}行";
                        //    MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = str0, m_stuGetColor = HalconColor.green });

                        //    ImageProduct.blSplitEA = true;
                        //    //扩充EA产品-cam0
                        //    PFTCleanEAProductInfo EAEAProductCam0 = ExPandHeadLinedic(camID, PFTCleanParamFrom.nCurEAProId + 1, findHeadParams.headLineID, findHeadParams.headLine, InfoImgInfo.imageHeight, Calipix_y, Calipix_x);//new一个新产品信息
                        //    PFTCleanParamFrom.dic_EAproductsInfo[PFTCleanParamFrom.nCurEAProId + 1] = EAEAProductCam0;
                        //    //////扩充EA产品-cam1
                        //    //PFTCleanEAProductInfo EAEAProductCam1 = ExPandHeadLinedic(camID + 1, PFTCleanParamBase.nCurEAProId + 1, findHeadParams.headLineID, findHeadParams.headLine + ABMis, InfoImgInfo.imageHeight, Calipix_y, Calipix_x);//new一个新产品信息
                        //    //PFTCleanParamFrom.dic_EAproductsInfo[PFTCleanParamBase.nCurEAProId + 1] = EAEAProductCam1;

                        //    //把抓边添加到ImageProduct和EAProduct
                        //    // LineInfo Normaleftline = NormalizeLine(HeadlineInfo, (int)ImageProduct.nCurImageID, 0, 0, (int)InfoImgInfo.image_ID, InfoImgInfo.imageHeight);
                        //    ImageProduct.ListImagelines.Add(HeadlineInfo);
                        //    LineInfo EAline = EAAddline(HeadlineInfo, EAEAProductCam0.CropEArange.StartImageID, EAEAProductCam0.CropEArange.StarImageRow, 0, InfoImgInfo.imageHeight);
                        //    // EAEAProductCam0.ListEAlines.Add(EAline);
                        //    // CalcuEASize(camID, InfoImgInfo.imageHeight, HeadlineInfo, m_craft.ListEASizes, EAEAProductCam0, EAEAProductCam1, ImageProduct.ListImageSizes);
                        //}
                        ////超长EA，强制结束EA，
                        //else if (InfoImgInfo.image_ID >= currentEAProduct.EArange.EndImageID + 2)
                        //{
                        //    ImageProduct.blSplitEA = true;

                        //    //扩充EA产品-cam0
                        //    PFTCleanEAProductInfo EAEAProductCam0 = ExPandHeadLinedic(camID, PFTCleanParamFrom.nCurEAProId + 1, currentEAProduct.EArange.EndImageID + 2, 0, InfoImgInfo.imageHeight, Calipix_y, Calipix_x);//new一个新产品信息
                        //    PFTCleanParamFrom.dic_EAproductsInfo[PFTCleanParamFrom.nCurEAProId + 1] = EAEAProductCam0;
                        //    ////扩充EA产品-cam1
                        //    //PFTCleanEAProductInfo EAEAProductCam1 = ExPandHeadLinedic(camID + 1, PFTCleanParamBase.nCurEAProId + 1, currentEAProduct.EArange.EndImageID + 2, ABMis, InfoImgInfo.imageHeight, Calipix_y, Calipix_x);//new一个新产品信息
                        //    //PFTCleanParamFrom.dic_EAproductsInfo[PFTCleanParamBase.nCurEAProId + 1] = EAEAProductCam1;

                        //    string str0 = $"B相机EA产品超长，强制分EA，产品编号：{PFTCleanParamFrom.nCurEAProId + 1},料头位置：第{currentEAProduct.EArange.EndImageID}帧-第{currentEAProduct.EArange.EndImageRow}行";
                        //    MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = str0, m_stuGetColor = HalconColor.red });
                        //}

                    }

                    ///根据Cam1料头裁切找料头边，
                    //根据图像ID自动开始下个EA
                    if (PFTCleanParamFrom.dic_EAproductsInfo.ContainsKey(PFTCleanParamFrom.nCurEAProId + 1))
                    {
                        if (ImageProduct.nCurImageID >= PFTCleanParamFrom.dic_EAproductsInfo[PFTCleanParamFrom.nCurEAProId + 1].Coatrange.StartImageID)//从相机使用图像帧和当前料头ID比较，判断是否开启检测下一个产品
                        {
                            ImageProduct.blSplitEA = true;
                        }

                        if (ImageProduct.blSplitEA)
                        {
                            PFTCleanEAProductInfo NextEAProductBase = PFTCleanParamBase.dic_EAproductsInfo[PFTCleanParamFrom.nCurEAProId + 1];
                            PFTCleanEAProductInfo NextEAProduct = PFTCleanParamFrom.dic_EAproductsInfo[PFTCleanParamFrom.nCurEAProId + 1];
                            //尝试检测下个EA的料头（如果足够裁图，就能在当前帧测量，如果不能，就在下一帧测量）
                            IPlugin CropPlugin = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == "料头裁切");
                            ParamsCropPart CropPartParams = CropPlugin.PluginParams as ParamsCropPart; //里氏转换
                            CropPartParams.list_ImageInfo = PFTCleanParamFrom.imageInfos;

                            CropPartParams.PartSizeRow = m_craft.EACropShiledMD;
                            CropPartParams.PartSizeCol = 1000;
                            CropPartParams.PartCenterID = NextEAProduct.Coatrange.StartImageID;
                            CropPartParams.PartCenterRow = NextEAProduct.Coatrange.StarImageRow;
                            CropPartParams.PartCenterCol = ImageProduct.EdgelineRight - 1000;//槽和基准边同一边就不用+，在对侧，就用片宽减半槽宽。
                            bool bl_findHeadSuccess = CropPlugin.ExecuteObj();
                            if (bl_findHeadSuccess)
                            {
                                if (NextEAProduct.Coatrange.StartImageID == ImageProduct.nCurImageID)
                                {
                                    HOperatorSet.GenRectangle2(out HObject GroVRegion1, CropPartParams.PartCenterRow, CropPartParams.PartCenterCol, 0, CropPartParams.PartSizeCol / 2 + CropPartParams.CropShaftCol, CropPartParams.PartSizeRow / 2 + CropPartParams.CropShaftRow);
                                    RegionShowInfos.Add(new RegionShowInfo() { m_hRegion = GroVRegion1, m_stuGetColor = HalconColor.pink });
                                }
                                string str0 = $"产品{NextEAProduct.EAID}料头裁切成功";
                                MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = str0, m_stuGetColor = HalconColor.orange });

                                NextEAProduct.BlHeadDone = true;
                                IPlugin PluginMline = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == "涂布料头边");
                                ParamsMeasureLine MlineParams = PluginMline.PluginParams as ParamsMeasureLine; //里氏转换
                                MlineParams.ho_Image = CropPartParams.Image_part;
                                LineInfo line = new LineInfo();
                                bool isSuccess = PluginMline.ExecuteObj();
                                DetectEdgeValueAssign(MlineParams, isSuccess, line, PluginMline.moduleInfo.ModuleName, MesssgeShowInfos, LineShowInfos, RegionShowInfos);

                                //把抓边添加到ImageProduct和EAProduct
                                double Srow = CropPartParams.PartCenterRow - CropPartParams.PartSizeRow / 2 - CropPartParams.CropShaftRow;
                                double Scol = CropPartParams.PartCenterCol - CropPartParams.PartSizeCol / 2 - CropPartParams.CropShaftCol;
                                LineInfo Normalline = NormalizeLine(line, (int)CropPartParams.PartCenterID, Srow, Scol, (int)InfoImgInfo.image_ID, InfoImgInfo.imageHeight);
                                ImageProduct.ListImagelines.Add(Normalline);
                                LineInfo EAline = EAAddline(Normalline, NextEAProduct.CropEArange.StartImageID, NextEAProduct.CropEArange.StarImageRow, 0, InfoImgInfo.imageHeight);
                                NextEAProduct.ListEAlines.Add(EAline);
                                // CalcuEASize(camID, InfoImgInfo.imageHeight, Normaleftline, m_craft.ListEASizes, NextEAProductBase, NextEAProduct, ImageProduct.ListImageSizes);
                            }
                        }
                    }

                    ////料头裁图，抓边，只是找一下料头边，滞后一帧也没关系
                    if (!currentEAProduct.BlHeadDone && InfoImgInfo.image_ID >= currentEAProduct.Coatrange.StartImageID) //图像大于等于料尾裁图的帧号
                    {
                        IPlugin CropPlugin = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == "料头裁切");
                        ParamsCropPart CropPartParams = CropPlugin.PluginParams as ParamsCropPart; //里氏转换
                        CropPartParams.list_ImageInfo = PFTCleanParamFrom.imageInfos;

                        CropPartParams.PartSizeRow = m_craft.EACropShiledMD;
                        CropPartParams.PartSizeCol = 1000;
                        CropPartParams.PartCenterID = currentEAProduct.Coatrange.StartImageID;
                        CropPartParams.PartCenterRow = currentEAProduct.Coatrange.StarImageRow;
                        CropPartParams.PartCenterCol = ImageProduct.EdgelineLeft + 1000;//槽和基准边同一边就不用+，在对侧，就用片宽减半槽宽。
                        bool bl_findHeadSuccess = CropPlugin.ExecuteObj();
                        if (bl_findHeadSuccess)
                        {
                            if (currentEAProduct.Coatrange.StartImageID == ImageProduct.nCurImageID)
                            {
                                HOperatorSet.GenRectangle2(out HObject GroVRegion1, CropPartParams.PartCenterRow, CropPartParams.PartCenterCol, 0, CropPartParams.PartSizeCol / 2 + CropPartParams.CropShaftCol, CropPartParams.PartSizeRow / 2 + CropPartParams.CropShaftRow);
                                RegionShowInfos.Add(new RegionShowInfo() { m_hRegion = GroVRegion1, m_stuGetColor = HalconColor.pink });
                            }
                            string str0 = $"产品{currentEAProduct.EAID}料头裁切成功";
                            MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = str0, m_stuGetColor = HalconColor.orange });

                            currentEAProduct.BlHeadDone = true;
                            IPlugin PluginMline = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == "涂布料头边");
                            ParamsMeasureLine MlineParams = PluginMline.PluginParams as ParamsMeasureLine; //里氏转换
                            MlineParams.ho_Image = CropPartParams.Image_part;
                            LineInfo line = new LineInfo();
                            bool isSuccess = PluginMline.ExecuteObj();
                            DetectEdgeValueAssign(MlineParams, isSuccess, line, PluginMline.moduleInfo.ModuleName, MesssgeShowInfos, LineShowInfos, RegionShowInfos);

                            //把抓边添加到ImageProduct和EAProduct
                            double Srow = CropPartParams.PartCenterRow - CropPartParams.PartSizeRow / 2 - CropPartParams.CropShaftRow;
                            double Scol = CropPartParams.PartCenterCol - CropPartParams.PartSizeCol / 2 - CropPartParams.CropShaftCol;
                            LineInfo Normalline = NormalizeLine(line, (int)CropPartParams.PartCenterID, Srow, Scol, (int)InfoImgInfo.image_ID, InfoImgInfo.imageHeight);
                            ImageProduct.ListImagelines.Add(Normalline);
                            LineInfo EAline = EAAddline(Normalline, currentEAProduct.CropEArange.StartImageID, currentEAProduct.CropEArange.StarImageRow, 0, InfoImgInfo.imageHeight);
                            currentEAProduct.ListEAlines.Add(EAline);

                            //CalcuEASize(camID, InfoImgInfo.imageHeight, Normaleftline, m_craft.ListEASizes, EAProductBase, currentEAProduct, ImageProduct.ListImageSizes);
                        }
                    }


                    Parallel.For(0, m_craft.SliceCount, i =>
                    // for (int i = 0; i < m_craft.SliceCount; i++)
                    {
                        int SliceIndex = i;
                        ImageProduct.ListImageSlicePD[SliceIndex].showHalconWindow.m_messageShowInfos = new List<MessageShowInfo>();
                        ImageProduct.ListImageSlicePD[SliceIndex].showHalconWindow.m_regionShowInfos = new List<RegionShowInfo>();
                        ImageProduct.ListImageSlicePD[SliceIndex].showHalconWindow.m_lineShowInfos = new List<LineInfo>();

                        if (m_craft.ListSliceSet[SliceIndex].BelongCam == ((CameraEnum)camID).ToString())
                        {
                            //每条左右边。
                            double left = ImageProduct.EdgelineLeft + m_craft.ListSliceSet[SliceIndex].SliceStartPos / Calipix_x;
                            double right = ImageProduct.EdgelineLeft + m_craft.ListSliceSet[SliceIndex].SliceEndPos / Calipix_x;
                            double TDshaft = m_craft.ListSliceSet[SliceIndex].SliceStartPos / Calipix_x;
                            //检测下个EA的涂布缺陷。
                            if (ImageProduct.blSplitEA)
                            {
                                HObject GroVRegion1 = CreatCoatBlock(InfoImgInfo, PFTCleanParamFrom.dic_EAproductsInfo[PFTCleanParamFrom.nCurEAProId + 1], SliceIndex, left, right, TDshaft, Calipix_y, Calipix_x);
                                ImageProduct.ListImageSlicePD[SliceIndex].showHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = GroVRegion1, m_stuGetColor = HalconColor.green });
                                //执行缺陷检测。
                                ParamsMQDefects _ParamsSlotefects = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == $"第{SliceIndex + 1}条涂布区缺陷检测").PluginParams as ParamsMQDefects;
                                _ParamsSlotefects.DetectionROI = GroVRegion1;
                                HOperatorSet.GenEmptyObj(out _ParamsSlotefects.ShieldROI);
                                _ParamsSlotefects.ho_Image = InfoImgInfo.ho_image;
                                _ParamsSlotefects.WidthCali = Calipix_x;
                                _ParamsSlotefects.HeightCali = Calipix_y;
                                process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == $"第{SliceIndex + 1}条涂布区缺陷检测").ExecuteObj();
                                //整合缺陷检测结果。
                                while (_ParamsSlotefects.detectInfos.Count() > 0)
                                {
                                    NgTypeInfo item = _ParamsSlotefects.detectInfos?.Dequeue();
                                    item.m_nImgIndex = (int)InfoImgInfo.image_ID;
                                    item.m_iProductIndex = PFTCleanParamFrom.nCurEAProId + 1;
                                    item.m_iSliceIndex = SliceIndex;
                                    ImageProduct.ListImageSlicePD[SliceIndex].ListImageDefects.Add(item);

                                    NgTypeInfo EAngInfo = new NgTypeInfo();
                                    EAngInfo = item.Copy();
                                    HOperatorSet.CopyObj(item.m_hRegion, out HObject region, 1, 1);
                                    int EASTid = PFTCleanParamFrom.dic_EAproductsInfo[PFTCleanParamFrom.nCurEAProId + 1].CropEArange.StartImageID;
                                    double EASTrow = PFTCleanParamFrom.dic_EAproductsInfo[PFTCleanParamFrom.nCurEAProId + 1].CropEArange.StarImageRow;
                                    HObject RegionMoved = EAAddRegion(region, EASTid, EASTrow, (int)InfoImgInfo.image_ID, InfoImgInfo.imageHeight);
                                    EAngInfo.m_hRegion = RegionMoved;
                                    PFTCleanParamFrom.dic_EAproductsInfo[PFTCleanParamFrom.nCurEAProId + 1].ListEASlicePD[SliceIndex].ListImageDefects.Add(EAngInfo);
                                }
                            }


                            //裁削薄区打孔区
                            bool ToDoShaveCrop = true;
                            while (ToDoShaveCrop && currentEAProduct.ListEASlicePD[SliceIndex].curSlShaveIndex < m_craft.ListSliceSet[SliceIndex].ShaveNum)
                            {
                                int index = currentEAProduct.ListEASlicePD[SliceIndex].curSlShaveIndex;
                                //削薄区
                                SlotProduct tempslot = currentEAProduct.ListEASlicePD[SliceIndex].listShave[index];
                                string namn = $"第{SliceIndex + 1}条打孔区裁切";
                                IPlugin CropPlugin = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == namn);
                                ParamsCropPart CropPartParams = CropPlugin.PluginParams as ParamsCropPart; //里氏转换
                                CropPartParams.list_ImageInfo = PFTCleanParamFrom.imageInfos;

                                CropPartParams.PartSizeRow = tempslot.Croprange.Height;
                                CropPartParams.PartSizeCol = tempslot.Croprange.Width;
                                CropPartParams.PartCenterID = tempslot.Croprange.MidImageID;
                                CropPartParams.PartCenterRow = tempslot.Croprange.MidImageRow;
                                tempslot.Croprange.StartCol = ImageProduct.EdgelineLeft + tempslot.slotInfo.SlotTDPosition / Calipix_x - tempslot.slotInfo.OutwardShieldLeft / Calipix_x;
                                CropPartParams.PartCenterCol = tempslot.Croprange.StartCol + CropPartParams.PartSizeCol / 2;//槽和基准边同一边就不用+，在对侧，就用片宽减半槽宽。
                                tempslot.Croprange.EndCol = tempslot.Croprange.StartCol + CropPartParams.PartSizeCol;

                                bool bl_ExecuteSuccess = CropPlugin.ExecuteObj();
                                if (bl_ExecuteSuccess)//削薄区成功
                                {
                                    if (true)
                                    {
                                        double rowMove = (tempslot.Croprange.MidImageID - ImageProduct.nCurImageID) * InfoImgInfo.imageHeight + CropPartParams.PartCenterRow;
                                        HOperatorSet.GenRectangle2(out HObject GroVRegion1, rowMove, CropPartParams.PartCenterCol, 0, CropPartParams.PartSizeCol / 2 + CropPartParams.CropShaftCol, CropPartParams.PartSizeRow / 2 + CropPartParams.CropShaftRow);
                                        ImageProduct.ListImageSlicePD[SliceIndex].showHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = GroVRegion1, m_stuGetColor = HalconColor.pink });
                                    }

                                    string str0 = $"第{SliceIndex + 1}条第{index + 1}个打孔区裁切成功";
                                    ImageProduct.ListImageSlicePD[SliceIndex].showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo { m_strMessage = str0, m_stuGetColor = HalconColor.orange });

                                    currentEAProduct.ListEASlicePD[SliceIndex].curSlShaveIndex++;

                                    //tempslot.showHalconWindow.m_hImage = CropPartParams.Image_part;
                                    //tempslot.showHalconWindow.m_nImageFrameID = index;
                                    //tempslot.showHalconWindow.m_nProductID = PFTCleanParamFrom.nCurEAProId;

                                    //添加首尾条的间隙边
                                    if (SliceIndex == 0)
                                    {
                                        //第一条左间隙边,
                                        int n = (int)(DirectionEnum.左间隙边);
                                        tempslot.Slotlines[n] = EdgeLine;
                                        //tempslot.Slotlines[n] = (new LineInfo()).copyLine(ImageProduct.ListImagelines.Find(c => c.StrLineName == "涂布左基准边"));
                                    }
                                    else if (SliceIndex == m_craft.SliceCount - 1)
                                    {
                                        //第一条左间隙边,
                                        int n = (int)(DirectionEnum.右间隙边);
                                        tempslot.Slotlines[n] = EdgeLine;
                                        // tempslot.Slotlines[n] = (new LineInfo()).copyLine(ImageProduct.ListImagelines.Find(c => c.StrLineName == "涂布右基准边"));
                                    }

                                    //找对应槽的四条边
                                    double C1 = 1000; //矩形左边
                                    double R1 = 1000; //矩形上边
                                    double C2 = 2000; //矩形右边
                                    double R2 = 2000; //矩形下边
                                                      //int count = Enum.GetValues(typeof(DirectionEnum)).Length;
                                    int count = 4;
                                    bool BlSlotLine = true;
                                    for (int j = 0; j < count; j++)
                                    {
                                        string LineName = $"第{SliceIndex + 1}条打孔区" + ((DirectionEnum)j).ToString();

                                        IPlugin PluginMline = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == LineName);
                                        ParamsMeasureLine MlineParams = PluginMline.PluginParams as ParamsMeasureLine; //里氏转换
                                        MlineParams.ho_Image = CropPartParams.Image_part;
                                        LineInfo line = new LineInfo();
                                        bool isSuccess = PluginMline.ExecuteObj();
                                        string ShaveName = $"第{SliceIndex + 1}条第{index + 1}个打孔区" + ((DirectionEnum)j).ToString();
                                        DetectEdgeValueAssign(MlineParams, isSuccess, line, ShaveName, ImageProduct.ListImageSlicePD[SliceIndex].showHalconWindow.m_messageShowInfos,
                                                 ImageProduct.ListImageSlicePD[SliceIndex].showHalconWindow.m_lineShowInfos, ImageProduct.ListImageSlicePD[SliceIndex].showHalconWindow.m_regionShowInfos);

                                        //tempslot.showHalconWindow.m_lineShowInfos = LineShowInfos;
                                        //把抓边添加到ImageProduct和EAProduct
                                        double Srow = CropPartParams.PartCenterRow - CropPartParams.PartSizeRow / 2 - CropPartParams.CropShaftRow;
                                        double Scol = CropPartParams.PartCenterCol - CropPartParams.PartSizeCol / 2 - CropPartParams.CropShaftCol;
                                        LineInfo Normalline = NormalizeLine(line, (int)CropPartParams.PartCenterID, Srow, Scol, (int)InfoImgInfo.image_ID, InfoImgInfo.imageHeight);

                                        //添加边到打孔区抓边队列
                                        tempslot.Slotlines[j] = Normalline.copyLine(Normalline);
                                        //添加边到图像抓边队列
                                        ImageProduct.ListImageSlicePD[SliceIndex].ListImagelines.Add(Normalline);
                                        //添加边到EA抓边队列--------//
                                        LineInfo EAline = EAAddline(Normalline, currentEAProduct.CropEArange.StartImageID, currentEAProduct.CropEArange.StarImageRow, 0, InfoImgInfo.imageHeight);
                                        currentEAProduct.ListEASlicePD[SliceIndex].ListImagelines.Add(EAline);



                                        BlSlotLine &= isSuccess;
                                        if (isSuccess)
                                        {
                                            if (j == 0)
                                            {
                                                C1 = (MlineParams.hv_LineColumnBegin.D + MlineParams.hv_LineColumnEnd.D) / 2 + tempslot.slotInfo.InwardShieldLeft / Calipix_x;
                                                tempslot.SlotSizes[0].DataResult = ((Normalline.ColBegin - ImageProduct.EdgelineLeft) + (Normalline.ColEnd - ImageProduct.EdgelineLeft)) * 0.5 * Calipix_x;  //td定位距

                                                if (SliceIndex != 0) //把除第一条的左边，传递给上一条的右间隙边
                                                {
                                                    //右间隙边,
                                                    int n = (int)(DirectionEnum.右间隙边);
                                                    currentEAProduct.ListEASlicePD[SliceIndex - 1].listShave[index].Slotlines[n] = Normalline.copyLine(Normalline);
                                                }
                                            }
                                            else if (j == 1)
                                            {
                                                C2 = (MlineParams.hv_LineColumnBegin.D + MlineParams.hv_LineColumnEnd.D) / 2 - tempslot.slotInfo.InwardShieldRight / Calipix_x;
                                                tempslot.SlotSizes[2].DataResult = (C2 - C1) * Calipix_x;//td尺寸

                                                if (SliceIndex != m_craft.SliceCount - 1)//把除第最后一条的右边，传递给上一条的左间隙边
                                                {
                                                    //左间隙边,
                                                    int n = (int)(DirectionEnum.左间隙边);
                                                    currentEAProduct.ListEASlicePD[SliceIndex + 1].listShave[index].Slotlines[n] = Normalline.copyLine(Normalline);
                                                }
                                            }
                                            else if (j == 2)
                                            {
                                                R1 = (MlineParams.hv_LineRowBegin.D + MlineParams.hv_LineRowEnd.D) / 2 + tempslot.slotInfo.InwardShieldUp / Calipix_y;

                                                double subRowS = (Normalline.RowBeginID - currentEAProduct.EArange.StartImageID) * InfoImgInfo.imageHeight + Normalline.RowBegin - currentEAProduct.EArange.StarImageRow;
                                                double subRowE = (Normalline.RowEndID - currentEAProduct.EArange.StartImageID) * InfoImgInfo.imageHeight + Normalline.RowEnd - currentEAProduct.EArange.StarImageRow;
                                                tempslot.SlotSizes[1].DataResult = (subRowS + subRowE) * 0.5 * Calipix_y;  //td定位距
                                            }
                                            else if (j == 3)
                                            {
                                                R2 = (MlineParams.hv_LineRowBegin.D + MlineParams.hv_LineRowEnd.D) / 2 - tempslot.slotInfo.InwardShieldDown / Calipix_y;
                                                tempslot.SlotSizes[3].DataResult = (R2 - R1) * Calipix_y;//Md尺寸
                                            }

                                        }
                                    }

                                    //检测对应削薄区的缺陷，
                                    //执行缺陷检测。
                                    IPlugin PluginSlotefects = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == $"第{SliceIndex + 1}条打孔区缺陷检测");
                                    ParamsMQDefects _ParamsSlotefects = PluginSlotefects.PluginParams as ParamsMQDefects;
                                    if (BlSlotLine && C1 < C2 && R1 < R2)
                                    {
                                        //添加槽位检测缩进 current3DProduct.listEAShave[index]
                                        HOperatorSet.GenRectangle1(out HObject GroVRegion1, R1, C1, R2, C2);
                                        _ParamsSlotefects.DetectionROI = GroVRegion1;

                                        int STid = tempslot.Croprange.StartImageID;
                                        double STrow = tempslot.Croprange.StarImageRow - CropPartParams.CropShaftRow;
                                        double STcol = tempslot.Croprange.StartCol - CropPartParams.CropShaftCol;
                                        HObject GroVRegionMoved = NormalizeRegion(GroVRegion1, STid, STrow, STcol, (int)InfoImgInfo.image_ID, InfoImgInfo.imageHeight);
                                        ImageProduct.ListImageSlicePD[SliceIndex].showHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = GroVRegionMoved, m_stuGetColor = HalconColor.cyan });
                                    }
                                    else
                                    {
                                        HOperatorSet.GenRectangle1(out HObject GroVRegion1, CropPartParams.CropShaftRow, CropPartParams.CropShaftCol, CropPartParams.PartSizeRow + CropPartParams.CropShaftRow, CropPartParams.PartSizeCol + CropPartParams.CropShaftCol);
                                        _ParamsSlotefects.DetectionROI = GroVRegion1;
                                        int STid = tempslot.Croprange.StartImageID;
                                        double STrow = tempslot.Croprange.StarImageRow - CropPartParams.CropShaftRow;
                                        double STcol = tempslot.Croprange.StartCol - CropPartParams.CropShaftCol;
                                        HObject GroVRegionMoved = NormalizeRegion(GroVRegion1, STid, STrow, STcol, (int)InfoImgInfo.image_ID, InfoImgInfo.imageHeight);
                                        ImageProduct.ListImageSlicePD[SliceIndex].showHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = GroVRegionMoved, m_stuGetColor = HalconColor.cyan });
                                    }
                                    HOperatorSet.GenEmptyObj(out _ParamsSlotefects.ShieldROI);
                                    _ParamsSlotefects.ho_Image = CropPartParams.Image_part;
                                    _ParamsSlotefects.WidthCali = Calipix_x;
                                    _ParamsSlotefects.HeightCali = Calipix_y;
                                    PluginSlotefects.ExecuteObj();

                                    //整合缺陷检测结果。整合缺陷到EA。
                                    while (_ParamsSlotefects.detectInfos.Count() > 0)
                                    {
                                        NgTypeInfo item = _ParamsSlotefects.detectInfos?.Dequeue();
                                        item.m_nImgIndex = (int)InfoImgInfo.image_ID;
                                        item.m_iProductIndex = PFTCleanParamFrom.nCurEAProId;
                                        item.m_iSliceIndex = SliceIndex;
                                        int STid = tempslot.Croprange.StartImageID;
                                        double STrow = tempslot.Croprange.StarImageRow - CropPartParams.CropShaftRow;
                                        double STcol = tempslot.Croprange.StartCol - CropPartParams.CropShaftCol;
                                        HObject GroVRegionMoved = NormalizeRegion(item.m_hRegion, STid, STrow, STcol, (int)InfoImgInfo.image_ID, InfoImgInfo.imageHeight);
                                        item.m_hRegion = GroVRegionMoved;
                                        ImageProduct.ListImageSlicePD[SliceIndex].ListImageDefects.Add(item);

                                        NgTypeInfo EAngInfo = new NgTypeInfo();
                                        EAngInfo = item.Copy();
                                        HOperatorSet.CopyObj(GroVRegionMoved, out HObject region, 1, 1);
                                        int EASTid = currentEAProduct.CropEArange.StartImageID;
                                        double EASTrow = currentEAProduct.CropEArange.StarImageRow;
                                        HObject RegionMoved = EAAddRegion(region, EASTid, EASTrow, (int)InfoImgInfo.image_ID, InfoImgInfo.imageHeight);
                                        EAngInfo.m_hRegion = RegionMoved;
                                        currentEAProduct.ListEASlicePD[SliceIndex].ListImageDefects.Add(EAngInfo);
                                    }
                                }
                                else //先做没裁图成功就跳出循环。后期再加帧号防呆。
                                {
                                    ToDoShaveCrop = false;
                                }
                            }

                            //检测涂布缺陷
                            if (true)
                            {
                                //生成矩形检测区域
                                HObject GroVRegion1 = CreatCoatBlock(InfoImgInfo, currentEAProduct, SliceIndex, left, right, TDshaft, Calipix_y, Calipix_x);
                                ImageProduct.ListImageSlicePD[SliceIndex].showHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = GroVRegion1, m_stuGetColor = HalconColor.yellow });
                                //执行缺陷检测。
                                ParamsMQDefects _ParamsSlotefects = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == $"第{SliceIndex + 1}条涂布区缺陷检测").PluginParams as ParamsMQDefects;
                                _ParamsSlotefects.DetectionROI = GroVRegion1;
                                HOperatorSet.GenEmptyObj(out _ParamsSlotefects.ShieldROI);
                                _ParamsSlotefects.ho_Image = InfoImgInfo.ho_image;
                                _ParamsSlotefects.WidthCali = Calipix_x;
                                _ParamsSlotefects.HeightCali = Calipix_y;
                                process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == $"第{SliceIndex + 1}条涂布区缺陷检测").ExecuteObj();
                                //整合缺陷检测结果。
                                while (_ParamsSlotefects.detectInfos.Count() > 0)
                                {
                                    NgTypeInfo item = _ParamsSlotefects.detectInfos?.Dequeue();
                                    item.m_nImgIndex = (int)InfoImgInfo.image_ID;
                                    item.m_iProductIndex = PFTCleanParamFrom.nCurEAProId;
                                    item.m_iSliceIndex = SliceIndex;
                                    ImageProduct.ListImageSlicePD[SliceIndex].ListImageDefects.Add(item);

                                    NgTypeInfo EAngInfo = new NgTypeInfo();
                                    EAngInfo = item.Copy();
                                    HOperatorSet.CopyObj(item.m_hRegion, out HObject region, 1, 1);
                                    HOperatorSet.AreaCenter(item.m_hRegion, out HTuple area, out HTuple row, out HTuple column);
                                    HOperatorSet.AreaCenter(region, out area, out row, out column);
                                    int EASTid = currentEAProduct.CropEArange.StartImageID;
                                    double EASTrow = currentEAProduct.CropEArange.StarImageRow;

                                    HObject RegionMoved = EAAddRegion(region, EASTid, EASTrow, (int)InfoImgInfo.image_ID, InfoImgInfo.imageHeight);
                                    HOperatorSet.AreaCenter(RegionMoved, out area, out row, out column);
                                    EAngInfo.m_hRegion = RegionMoved;
                                    currentEAProduct.ListEASlicePD[SliceIndex].ListImageDefects.Add(EAngInfo);
                                }
                            }
                        }
                        //}
                    });
                    //料尾裁图，抓边
                    if (!currentEAProduct.BlTileDone && InfoImgInfo.image_ID >= currentEAProduct.Coatrange.EndImageID) //图像大于等于料尾裁图的帧号
                    {
                        IPlugin CropPlugin = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == "料尾裁切");
                        ParamsCropPart CropPartParams = CropPlugin.PluginParams as ParamsCropPart; //里氏转换
                        CropPartParams.list_ImageInfo = PFTCleanParamFrom.imageInfos;

                        CropPartParams.PartSizeRow = m_craft.EACropShiledMD;
                        CropPartParams.PartSizeCol = 1000;
                        CropPartParams.PartCenterID = currentEAProduct.Coatrange.EndImageID;
                        CropPartParams.PartCenterRow = currentEAProduct.Coatrange.EndImageRow;
                        CropPartParams.PartCenterCol = ImageProduct.EdgelineRight - 2000;//槽和基准边同一边就不用+，在对侧，就用片宽减半槽宽。
                        bool bl_ExecuteSuccess = CropPlugin.ExecuteObj();
                        if (bl_ExecuteSuccess)
                        {
                            if (currentEAProduct.Coatrange.EndImageID == ImageProduct.nCurImageID)
                            {
                                HOperatorSet.GenRectangle2(out HObject GroVRegion1, CropPartParams.PartCenterRow, CropPartParams.PartCenterCol, 0, CropPartParams.PartSizeCol / 2 + CropPartParams.CropShaftCol, CropPartParams.PartSizeRow / 2 + CropPartParams.CropShaftRow);
                                RegionShowInfos.Add(new RegionShowInfo() { m_hRegion = GroVRegion1, m_stuGetColor = HalconColor.pink });
                            }
                            string str0 = $"产品{currentEAProduct.EAID}料尾裁切成功";
                            MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = str0, m_stuGetColor = HalconColor.orange });

                            currentEAProduct.BlTileDone = true;
                            IPlugin PluginMline = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == "涂布料尾边");
                            ParamsMeasureLine MlineParams = PluginMline.PluginParams as ParamsMeasureLine; //里氏转换
                            MlineParams.ho_Image = CropPartParams.Image_part;
                            LineInfo line = new LineInfo();
                            bool isSuccess = PluginMline.ExecuteObj();
                            DetectEdgeValueAssign(MlineParams, isSuccess, line, PluginMline.moduleInfo.ModuleName, MesssgeShowInfos, LineShowInfos, RegionShowInfos);

                            //把抓边添加到ImageProduct和EAProduct
                            double Srow = CropPartParams.PartCenterRow - CropPartParams.PartSizeRow / 2 - CropPartParams.CropShaftRow;
                            double Scol = CropPartParams.PartCenterCol - CropPartParams.PartSizeCol / 2 - CropPartParams.CropShaftCol;
                            LineInfo Normalline = NormalizeLine(line, (int)CropPartParams.PartCenterID, Srow, Scol, (int)InfoImgInfo.image_ID, InfoImgInfo.imageHeight);
                            ImageProduct.ListImagelines.Add(Normalline);
                            LineInfo EAline = EAAddline(Normalline, currentEAProduct.CropEArange.StartImageID, currentEAProduct.CropEArange.StarImageRow, 0, InfoImgInfo.imageHeight);
                            currentEAProduct.ListEAlines.Add(EAline);
                            //current3DProduct.ListEAlines.Add(EAline);
                            if (isSuccess)
                            {
                            }
                            //CalcuEASize(camID, InfoImgInfo.imageHeight, Normaleftline, m_craft.ListEASizes, EAProductBase, currentEAProduct, ImageProduct.ListImageSizes);
                        }
                    }


                    ///复制图像给EA拼大图流程
                    // InfoImgInfo.ho_image.CopyImage(out HObject CopyImage);
                    //InfoImgInfo.ho_image.DisposeEx();
                    HOperatorSet.CopyObj(InfoImgInfo.ho_image, out HObject CopyImage, 1, 1);
                    HOperatorSet.GetImageSize(InfoImgInfo.ho_image, out HTuple w, out HTuple h);
                    PFTCleanParamFrom.PFTImageProdQueue.Enqueue(new ImageInfo() { image_ID = InfoImgInfo.image_ID, EAID = PFTCleanParamFrom.nCurEAProId, ho_image = CopyImage, imageWidth = w, imageHeight = h });
                    PFTCleanParamFrom.semaphoreSlim.Release();

                    if (ImageProduct.blSplitEA)
                    {
                        PFTCleanParamFrom.nCurEAProId++;
                    }

                    ImageProduct.showHalconWindow.m_nProductID = ImageProduct.nCurEAID;
                    ImageProduct.showHalconWindow.m_nImageFrameID = InfoImgInfo.image_ID;
                    ImageProduct.showHalconWindow.m_hImage = InfoImgInfo.ho_image;
                    ImageProduct.showHalconWindow.m_lineShowInfos = ImageProduct.ListImagelines;
                    ImageProduct.showHalconWindow.m_regionShowInfos = RegionShowInfos;
                    ImageProduct.showHalconWindow.m_messageShowInfos = MesssgeShowInfos;
                    PFTCleanParamFrom.ProductCallBack(ImageProduct);
                    MessageNotify.AddDebugLog($"Cam2帧图流程产品{ImageProduct.nCurEAID}图像{InfoImgInfo.image_ID}处理完成");
                }

            }
            catch (Exception ex)
            {
                MessageNotify.AddErrorLog(ex.ToString());
                Log4netRecord.ErrorLog.Error(ex);
            }
        }

        public void ProProcessFromEA(Process process)
        {
            try
            {
                int ProcessID = process.processInfo.ProcessID;
                int camID = 1;
                SetThreadAffinityMask(GetCurrentThread(), new UIntPtr(SetCpuID(ProcessID)));

                //标定系数
                double Calipix_y = Project.Instance.cameraManager[(CameraEnum)camID].m_CamParam.CalibrationValue_Y;
                double Calipix_x = Project.Instance.cameraManager[(CameraEnum)camID].m_CamParam.CalibrationValue_X;
                double ABMis = Project.Instance.cameraManager[(CameraEnum)camID].m_CamParam.ToMainCamDistance / Calipix_y;//到主相机像素距离
                                                                                                                          // bool ExecuteSuccess = false;
                                                                                                                          // 选择参数和队列基于是否模拟
                double ZoomFactorX = 0.5;
                double ZoomFactorY = 0.5;

                PFTCleanProParams PFTCleanParamFrom = Project.Instance.Is_simulation ? m_DicCamOperaParam_Sim[camID] as PFTCleanProParams : m_DicCamOperaParam[camID] as PFTCleanProParams;
                var PFTimageQueue = PFTCleanParamFrom.PFTImageProdQueue;
                PFTCleanParamFrom.semaphoreSlim.Wait();

                if (PFTimageQueue.Count > 2) // 检查主相机的图像队列是否有图像
                {
                    //主相机图像参数
                    ImageInfo InfoImgInfo = PFTimageQueue.Dequeue();
                    PFTCleanParamFrom.EAlistimageInfos.Add(InfoImgInfo);
                    if (PFTCleanParamFrom.EAlistimageInfos.Count > 6)
                    {
                        long removeID = InfoImgInfo.image_ID - 6;
                        var tempIMfo = PFTCleanParamFrom.EAlistimageInfos.FindIndex(c => c.image_ID == removeID);
                        if (tempIMfo != -1)
                        {
                            PFTCleanParamFrom.EAlistimageInfos[tempIMfo]?.Dispose();
                            PFTCleanParamFrom.EAlistimageInfos.RemoveAt(tempIMfo);
                            //获取 PFTCleanParamFrom.EAlistimageInfos所有的key值
                            var key = PFTCleanParamFrom.EAlistimageInfos.Select(c => c.image_ID).ToList();
                            MessageNotify.AddDebugLog($"Cam2EA流程产品{PFTCleanParamFrom.nCurShowEAProId}图像{InfoImgInfo.image_ID}移除图像ID{removeID}移除成功,队列中超过10帧图像，剩余{string.Join(",", key)}");
                        }
                        else
                        {
                            MessageNotify.AddErrorLog($"Cam2EA流程产品{PFTCleanParamFrom.nCurShowEAProId}图像{InfoImgInfo.image_ID}移除图像ID{removeID}处理失败,队列中没有前10帧图像");
                        }
                    }
                    //获取CleanEAParamBase.dic_EAproductsInfo所有的key值
                    List<int> keys = PFTCleanParamFrom.dic_EAproductsInfo.Keys.ToList();
                    MessageNotify.AddDebugLog($"Cam2EA流程产品{PFTCleanParamFrom.nCurShowEAProId}图像{InfoImgInfo.image_ID}图像EAID{InfoImgInfo.EAID}处理开始,当前key值列表:{string.Join(",", keys)}");

                    PFTCleanEAProductInfo currentEAProduct = PFTCleanParamFrom.dic_EAproductsInfo[PFTCleanParamFrom.nCurShowEAProId];

                    List<MessageShowInfo> MesssgeShowInfos = new List<MessageShowInfo>();
                    List<RegionShowInfo> RegionShowInfos = new List<RegionShowInfo>();
                    List<LineInfo> LineShowInfos = new List<LineInfo>();
                    try
                    {
                        if (InfoImgInfo.EAID > PFTCleanParamFrom.nCurShowEAProId)
                        {
                            currentEAProduct.blEndTotalEAImage = true;
                        }
                        //if (PFTCleanParamFrom.nCurEAProId > PFTCleanParamFrom.nCurShowEAProId)
                        //{
                        //    PFTCleanEAProductInfo NextEAProduct = PFTCleanParamFrom.dic_EAproductsInfo[PFTCleanParamFrom.nCurEAProId];
                        //    if (InfoImgInfo.image_ID > NextEAProduct.EArange.StartImageID)
                        //    {
                        //        currentEAProduct.blEndTotalEAImage = true;
                        //    }
                        //}

                        int StartID = currentEAProduct.CropEArange.StartImageID > 1 ? currentEAProduct.CropEArange.StartImageID : 1;
                        //如果已经拼接了料头图，就跳过，如果未拼接料头图，就裁拼料头图
                        if (!currentEAProduct.blStartTotalEAImage)
                        {
                            //当前帧不是料头裁切帧，则裁切帧应该是当前帧上一帧，在上一帧裁出料头，再拼接当前帧整个，作为EA全图。
                            if (StartID < InfoImgInfo.image_ID)
                            {
                                ImageInfo InfoImgInfo0 = PFTCleanParamFrom.EAlistimageInfos.Find(c => c.image_ID == StartID);
                                if (InfoImgInfo0 != null)
                                {
                                    // HOperatorSet.CropRectangle1(InfoImgInfo0.ho_image, out currentEAProduct.TotalEAImage, currentEAProduct.CropEArange.StarImageRow - 1, 0, InfoImgInfo.imageHeight - 1, InfoImgInfo.imageWidth - 1);
                                    HOperatorSet.CropRectangle1(InfoImgInfo0.ho_image, out HObject ZooImage, currentEAProduct.CropEArange.StarImageRow - 1, 0, InfoImgInfo.imageHeight - 1, InfoImgInfo.imageWidth - 1);
                                    HOperatorSet.ZoomImageFactor(ZooImage, out currentEAProduct.EAImageCollection, ZoomFactorX, ZoomFactorY, "bilinear");
                                }
                                else
                                {
                                    // currentEAProduct.TotalEAImage = InfoImgInfo.ho_image;
                                    HOperatorSet.ZoomImageFactor(InfoImgInfo.ho_image, out HObject ZooImage, ZoomFactorX, ZoomFactorY, "bilinear");
                                    currentEAProduct.EAImageCollection = ZooImage;
                                }
                                StartID++;
                                while (StartID <= InfoImgInfo.image_ID)
                                {
                                    ImageInfo InfoImgInfo1 = PFTCleanParamFrom.EAlistimageInfos.Find(c => c.image_ID == StartID);
                                    if (InfoImgInfo1 != null)
                                    {
                                        //TileImage(currentEAProduct.TotalEAImage, InfoImgInfo1.ho_image, out currentEAProduct.TotalEAImage);
                                        HOperatorSet.ZoomImageFactor(InfoImgInfo1.ho_image, out HObject ZooImage, ZoomFactorX, ZoomFactorY, "bilinear");
                                        HOperatorSet.ConcatObj(currentEAProduct.EAImageCollection, ZooImage, out currentEAProduct.EAImageCollection);
                                    }
                                    StartID++;
                                }
                                currentEAProduct.blStartTotalEAImage = true;
                            }
                            else if (StartID == InfoImgInfo.image_ID)//当前帧就是料头裁切帧，则直接裁出部分作为EA全图，承接后续拼图
                            {
                                // HOperatorSet.CropRectangle1(InfoImgInfo.ho_image, out currentEAProduct.TotalEAImage, currentEAProduct.CropEArange.StarImageRow - 1, 0, InfoImgInfo.imageHeight - 1, InfoImgInfo.imageWidth - 1);
                                HOperatorSet.CropRectangle1(InfoImgInfo.ho_image, out HObject ZooImage, currentEAProduct.CropEArange.StarImageRow - 1, 0, InfoImgInfo.imageHeight - 1, InfoImgInfo.imageWidth - 1);
                                HOperatorSet.ZoomImageFactor(ZooImage, out currentEAProduct.EAImageCollection, ZoomFactorX, ZoomFactorY, "bilinear");

                                currentEAProduct.blStartTotalEAImage = true;
                            }
                        }

                        //裁拼EA图像，
                        else if (InfoImgInfo.image_ID < currentEAProduct.CropEArange.EndImageID) //当前图像帧ID  小于裁图结束ID，则把EA全图拼上整个当前帧。
                        {

                            try
                            {
                                //HOperatorSet.GetImageSize(current3DProduct.TotalEAImage, out HTuple w, out HTuple h);
                                //  TileImage(currentEAProduct.TotalEAImage, InfoImgInfo.ho_image, out currentEAProduct.TotalEAImage);
                                HOperatorSet.ZoomImageFactor(InfoImgInfo.ho_image, out HObject ZooImage, ZoomFactorX, ZoomFactorY, "bilinear");
                                HOperatorSet.ConcatObj(currentEAProduct.EAImageCollection, ZooImage, out currentEAProduct.EAImageCollection);
                            }
                            catch (Exception ex)
                            {
                                MessageNotify.AddDebugLog("3:" + ex.ToString());
                            }
                        }
                        else if (InfoImgInfo.image_ID == currentEAProduct.CropEArange.EndImageID) //当前图像帧ID  大于等于 裁图结束ID，则把EA全图拼上局部当前帧。当前EA就已经拼图完成。开始拼下一个EA的EA全图
                        {
                            try
                            {
                                HOperatorSet.CropRectangle1(InfoImgInfo.ho_image, out HObject Image_part, 0, 0, currentEAProduct.CropEArange.EndImageRow - 1, InfoImgInfo.imageWidth - 1);
                                // TileImage(currentEAProduct.TotalEAImage, Image_part, out currentEAProduct.TotalEAImage);
                                HOperatorSet.ZoomImageFactor(Image_part, out HObject ZooImage, ZoomFactorX, ZoomFactorY, "bilinear");
                                Image_part.Dispose();
                                HOperatorSet.ConcatObj(currentEAProduct.EAImageCollection, ZooImage, out currentEAProduct.EAImageCollection);
                                // currentEAProduct.blEndTotalEAImage = true;
                            }
                            catch (Exception ex)
                            {
                                MessageNotify.AddDebugLog("4:" + ex.ToString());
                            }
                        }

                        if (currentEAProduct.blEndTotalEAImage)
                        {
                            TileImage2Offset(currentEAProduct.EAImageCollection, out currentEAProduct.TotalEAImage);
                            foreach (NgTypeInfo item in currentEAProduct.ListEADefects)
                            {
                                HOperatorSet.ZoomRegion(item.m_hRegion, out HObject defect, ZoomFactorX, ZoomFactorY);
                                item.m_hRegion = defect;
                            }
                            for (int i = 0; i < m_craft.SliceCount; i++)
                            {
                                foreach (NgTypeInfo item in currentEAProduct.ListEASlicePD[i].ListImageDefects)
                                {
                                    HOperatorSet.ZoomRegion(item.m_hRegion, out HObject defect, ZoomFactorX, ZoomFactorY);
                                    item.m_hRegion = defect;
                                }
                            }

                            PFTCleanParamFrom.nCurShowEAProId = InfoImgInfo.EAID;

                            currentEAProduct.EAImageCollection?.DisposeEx();

                        }
                    }
                    catch (Exception ex)
                    {
                        Log4netRecord.ErrorLog.Error(ex);
                    }
                    //计时            
                    HOperatorSet.CountSeconds(out currentEAProduct.Image_StartTime);
                    HOperatorSet.CountSeconds(out currentEAProduct.Image_FindDefectTime);
                    HOperatorSet.CountSeconds(out currentEAProduct.Image_ArithmeticProcessTime);

                    string str = ((CameraEnum)camID).ToString() + "信号量--" + PFTCleanParamFrom.semaphoreSlimCam.CurrentCount.ToString() + "图像数--" + PFTimageQueue.Count.ToString();
                    MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = str, m_stuGetColor = HalconColor.green });
                    MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = $"当前产品对应的产品号：{currentEAProduct.EAID}", m_stuGetColor = HalconColor.green });

                    ///todo待开发
                    ///
                    currentEAProduct.showHalconWindow.m_nProductID = currentEAProduct.EAID;
                    currentEAProduct.showHalconWindow.m_hImage = currentEAProduct.TotalEAImage;
                    currentEAProduct.showHalconWindow.m_lineShowInfos = LineShowInfos;
                    currentEAProduct.showHalconWindow.m_regionShowInfos = RegionShowInfos;
                    currentEAProduct.showHalconWindow.m_messageShowInfos = MesssgeShowInfos;

                    if (currentEAProduct.blEndTotalEAImage)
                    {
                        PFTCleanParamFrom.ProductCallBackCraft(currentEAProduct);
                        PFTCleanParamFrom.nCurShowEAProId = InfoImgInfo.EAID;
                        MessageNotify.AddDebugLog($"Cam2EA流程产品{currentEAProduct.EAID}图像nCurShowEAProId{PFTCleanParamFrom.nCurShowEAProId}{InfoImgInfo.image_ID}处理完成");
                    }



                    //InfoImgInfo?.Dispose();
                }
            }
            catch (Exception ex)
            {

                MessageNotify.AddErrorLog("Cam2EA流程产品报错：" + ex.Message);
            }
        }


        public void ProProcess3D_A(Process process)
        {
            try
            {
                int ProcessID = process.processInfo.ProcessID;
                int camID = 2;
                SetThreadAffinityMask(GetCurrentThread(), new UIntPtr(SetCpuID(ProcessID)));

                //标定系数
                double Calipix_y = Project.Instance.cameraManager[(CameraEnum)camID].m_CamParam.CalibrationValue_Y;
                double Calipix_x = Project.Instance.cameraManager[(CameraEnum)camID].m_CamParam.CalibrationValue_X;

                // bool ExecuteSuccess = false;
                // 选择参数和队列基于是否模拟
                PFTCleanProParams PFTCleanParamBase = Project.Instance.Is_simulation ? m_DicCamOperaParam_Sim[camID] as PFTCleanProParams : m_DicCamOperaParam[camID] as PFTCleanProParams;


                PFTCleanParamBase.semaphoreSlimCam.Wait();

                if (PFTCleanParamBase.alreadyDealGrabImage3D.Count > 0)
                {
                    Image3DClass Image3D = PFTCleanParamBase.alreadyDealGrabImage3D.Dequeue();

                    List<MessageShowInfo> MesssgeShowInfos = new List<MessageShowInfo>();
                    List<RegionShowInfo> RegionShowInfos = new List<RegionShowInfo>();
                    List<LineInfo> LineShowInfos = new List<LineInfo>();
                    MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = $"3D帧号：{Image3D.image_ID}_开始处理3D图像", m_stuGetColor = HalconColor.green });
                    //初始化3D相机单帧产品。
                    PFTImageProduct current3DProduct = new PFTImageProduct();
                    //初始化3D相机的尺寸。
                    for (int i = 0; i < m_craft.ListEASizes.Count; i++)
                    {
                        if (m_craft.ListEASizes[i].camera == (CameraEnum)camID && (m_craft.ListEASizes[i].Enable == true))
                        {
                            SizeInfo size = new SizeInfo()
                            {
                                Name = m_craft.ListEASizes[i].Name,
                                SizeName = m_craft.ListEASizes[i].Name,
                                Description = m_craft.ListEASizes[i].Description,
                                StandardValue = m_craft.ListEASizes[i].StDValue,
                                upperLimitValue = m_craft.ListEASizes[i].StD_BJ_Up,
                                lowerLimitValue = m_craft.ListEASizes[i].StD_BJ_Down,
                            };
                            current3DProduct.ListImageSizes.Add(size);
                        }
                    }
                    double m3DStartRow = 0;
                    //确认上基准边 
                    string linepluginname = "3D定位抓边";
                    //抓边
                    IPlugin PluginMline = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == linepluginname);
                    ParamsMeasureLine MlineParams = PluginMline.PluginParams as ParamsMeasureLine; //里氏转换
                    MlineParams.ho_Image = Image3D.Image_Light;
                    LineInfo line = new LineInfo();
                    bool LineisSuccess = PluginMline.ExecuteObj();
                    DetectEdgeValueAssign(MlineParams, LineisSuccess, line, linepluginname, MesssgeShowInfos, LineShowInfos, RegionShowInfos);
                    if (LineisSuccess)
                    {
                        m3DStartRow = (MlineParams.hv_LineRowBegin.D + MlineParams.hv_LineRowEnd.D) / 2;
                    }


                    //测深度插件
                    IPlugin PluginM3Depth = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == "3D测深度");
                    ParamsD3MeasureDepth M3DlineParams = PluginM3Depth.PluginParams as ParamsD3MeasureDepth; //里氏转换
                    M3DlineParams.ho_Image = Image3D.Image3D;
                    M3DlineParams.ho_ImageLight = Image3D.Image_Light;

                    if (LineisSuccess)
                    {
                        M3DlineParams.hv_RoiMidRow = m3DStartRow + M3DlineParams.hv_ShaveSizeRow;
                    }
                    HOperatorSet.TupleRad(-M3DlineParams.hv_RoiAngle, out HTuple rad);
                    HOperatorSet.GenRectangle2(out HObject rectangle, M3DlineParams.hv_RoiMidRow, M3DlineParams.hv_RoiMidColumn, rad, M3DlineParams.hv_RoiHeight / 2, M3DlineParams.hv_RoiWidth / 2);
                    RegionShowInfos.Add(new RegionShowInfo() { m_hRegion = rectangle, m_stuGetColor = HalconColor.blue });

                    bool isSuccess = PluginM3Depth.ExecuteObj();
                    SizeInfo SizeInfo = current3DProduct.ListImageSizes.Find(c => c.Name == ((CameraEnum)camID).ToString() + "3D深度");
                    if (SizeInfo != null)
                    {
                        if (isSuccess)
                        {
                            SizeInfo.DataResult = M3DlineParams.hv_DepthMean.D;

                        }
                        else
                        {
                            SizeInfo.DataResult = -999;
                        }
                        SizeInfo.resultType = ResultType.OK;
                        SizeInfo.IsNG = false;
                        //MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = $"测量结果：{isSuccess}_{SizeInfo.DataResult}", m_stuGetColor = HalconColor.green });
                    }
                    else
                    {
                        MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = $"空检测项", m_stuGetColor = HalconColor.green });
                    }



                    current3DProduct.nCurImageID = Image3D.image_ID;
                    current3DProduct.showHalconWindow.m_nProductID = Image3D.image_ID;
                    current3DProduct.showHalconWindow.m_nImageFrameID = Image3D.image_ID;
                    current3DProduct.showHalconWindow.Image3DClass = Image3D;
                    current3DProduct.showHalconWindow.m_lineShowInfos = LineShowInfos;
                    current3DProduct.showHalconWindow.m_regionShowInfos = RegionShowInfos;
                    current3DProduct.showHalconWindow.m_messageShowInfos = MesssgeShowInfos;
                    // if (current3DProduct.blEndTotalEAImage)
                    {
                        PFTCleanParamBase.ProductCallBack(current3DProduct);
                    }

                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }


        public void ProProcess3D_B(Process process)
        {
            try
            {
                int ProcessID = process.processInfo.ProcessID;
                int camID = 3;
                SetThreadAffinityMask(GetCurrentThread(), new UIntPtr(SetCpuID(ProcessID)));

                //标定系数
                double Calipix_y = Project.Instance.cameraManager[(CameraEnum)camID].m_CamParam.CalibrationValue_Y;
                double Calipix_x = Project.Instance.cameraManager[(CameraEnum)camID].m_CamParam.CalibrationValue_X;

                // bool ExecuteSuccess = false;
                // 选择参数和队列基于是否模拟
                PFTCleanProParams PFTCleanParamBase = Project.Instance.Is_simulation ? m_DicCamOperaParam_Sim[camID] as PFTCleanProParams : m_DicCamOperaParam[camID] as PFTCleanProParams;


                PFTCleanParamBase.semaphoreSlimCam.Wait();

                if (PFTCleanParamBase.alreadyDealGrabImage3D.Count > 0)
                {
                    Image3DClass Image3D = PFTCleanParamBase.alreadyDealGrabImage3D.Dequeue();

                    List<MessageShowInfo> MesssgeShowInfos = new List<MessageShowInfo>();
                    List<RegionShowInfo> RegionShowInfos = new List<RegionShowInfo>();
                    List<LineInfo> LineShowInfos = new List<LineInfo>();
                    MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = $"3D帧号：{Image3D.image_ID}_开始处理3D图像", m_stuGetColor = HalconColor.green });
                    //初始化3D相机单帧产品。
                    PFTImageProduct current3DProduct = new PFTImageProduct();
                    //初始化3D相机的尺寸。
                    for (int i = 0; i < m_craft.ListEASizes.Count; i++)
                    {
                        if (m_craft.ListEASizes[i].camera == (CameraEnum)camID && (m_craft.ListEASizes[i].Enable == true))
                        {
                            SizeInfo size = new SizeInfo()
                            {
                                Name = m_craft.ListEASizes[i].Name,
                                SizeName = m_craft.ListEASizes[i].Name,
                                Description = m_craft.ListEASizes[i].Description,
                                StandardValue = m_craft.ListEASizes[i].StDValue,
                                upperLimitValue = m_craft.ListEASizes[i].StD_BJ_Up,
                                lowerLimitValue = m_craft.ListEASizes[i].StD_BJ_Down,
                            };
                            current3DProduct.ListImageSizes.Add(size);
                        }
                    }
                    double m3DStartRow = 0;
                    //确认上基准边 
                    string linepluginname = "3D定位抓边";
                    //抓边
                    IPlugin PluginMline = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == linepluginname);
                    ParamsMeasureLine MlineParams = PluginMline.PluginParams as ParamsMeasureLine; //里氏转换
                    MlineParams.ho_Image = Image3D.Image_Light;
                    LineInfo line = new LineInfo();
                    bool LineisSuccess = PluginMline.ExecuteObj();
                    DetectEdgeValueAssign(MlineParams, LineisSuccess, line, linepluginname, MesssgeShowInfos, LineShowInfos, RegionShowInfos);
                    if (LineisSuccess)
                    {
                        m3DStartRow = (MlineParams.hv_LineRowBegin.D + MlineParams.hv_LineRowEnd.D) / 2;
                    }

                    //测深度插件
                    IPlugin PluginM3Depth = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == "3D测深度");
                    ParamsD3MeasureDepth M3DlineParams = PluginM3Depth.PluginParams as ParamsD3MeasureDepth; //里氏转换
                    M3DlineParams.ho_Image = Image3D.Image3D;
                    M3DlineParams.ho_ImageLight = Image3D.Image_Light;
                    if (LineisSuccess)
                    {
                        M3DlineParams.hv_RoiMidRow = m3DStartRow + M3DlineParams.hv_ShaveSizeRow;
                    }
                    HOperatorSet.TupleRad(-M3DlineParams.hv_RoiAngle, out HTuple rad);
                    HOperatorSet.GenRectangle2(out HObject rectangle, M3DlineParams.hv_RoiMidRow, M3DlineParams.hv_RoiMidColumn, rad, M3DlineParams.hv_RoiHeight / 2, M3DlineParams.hv_RoiWidth / 2);
                    RegionShowInfos.Add(new RegionShowInfo() { m_hRegion = rectangle, m_stuGetColor = HalconColor.blue });

                    bool isSuccess = PluginM3Depth.ExecuteObj();

                    SizeInfo SizeInfo = current3DProduct.ListImageSizes.Find(c => c.Name == ((CameraEnum)camID).ToString() + "3D深度");
                    if (SizeInfo != null)
                    {
                        if (isSuccess)
                        {
                            SizeInfo.DataResult = M3DlineParams.hv_DepthMean.D;
                        }
                        else
                        {
                            SizeInfo.DataResult = -999;
                        }
                        SizeInfo.resultType = ResultType.OK;
                        SizeInfo.IsNG = false;
                        // current3DProduct.ListImageSizes.Add(SizeInfo);
                        // MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = $"测量结果：{isSuccess}_{SizeInfo.DataResult}", m_stuGetColor = HalconColor.green });
                    }
                    else
                    {
                        MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = $"空检测项", m_stuGetColor = HalconColor.green });
                    }


                    current3DProduct.nCurImageID = Image3D.image_ID;
                    current3DProduct.showHalconWindow.m_nProductID = Image3D.image_ID;
                    current3DProduct.showHalconWindow.m_nImageFrameID = Image3D.image_ID;
                    current3DProduct.showHalconWindow.Image3DClass = Image3D;
                    current3DProduct.showHalconWindow.m_lineShowInfos = LineShowInfos;
                    current3DProduct.showHalconWindow.m_regionShowInfos = RegionShowInfos;
                    current3DProduct.showHalconWindow.m_messageShowInfos = MesssgeShowInfos;
                    // if (current3DProduct.blEndTotalEAImage)
                    {
                        PFTCleanParamBase.ProductCallBack(current3DProduct);
                    }

                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }



        // 扩展产品：往产品队列中新增一个产品，并赋初始值。
        public PFTCleanEAProductInfo ExPandHeadLinedic(int camID, int EAproductID, int HeadLineID, double HeadLineRow, double ImageHeight, double Calipix_y, double Calipix_x)
        {
            PFTCleanEAProductInfo EAEAProduct = new PFTCleanEAProductInfo();
            EAEAProduct.EAID = EAproductID;
            try
            {
                for (int i = 0; i < m_craft.SliceCount; i++) //分条，每条一个槽位，N个打孔区
                {
                    PFTSliceProduct SlicePD = new PFTSliceProduct();
                    EAEAProduct.ListEASlicePD.Add(SlicePD);

                    // EAEAProduct.list2EAShave.Add(new List<SlotProduct>());
                    //if (m_craft.ListSliceSet[i].BelongCam == ((CameraEnum)camID).ToString())
                    {
                        if (m_craft.MaterialType)
                        {
                            SlotProduct slot = new SlotProduct();
                            slot.slotInfo = m_craft.ListSlotInfos.Find(c => c.SliceID == i && c.ShaveID == 0);
                            slot.IsFinished = false;
                            slot.Slottype = SlottypeEnum.slot;

                            double MDdist0 = (slot.slotInfo.SlotMDPosition - slot.slotInfo.OutwardShieldUp) / Calipix_y; //槽位裁图起始---距离料头的行数
                            double MDdist1 = (slot.slotInfo.SlotMDPosition + slot.slotInfo.SlotMDSize + slot.slotInfo.OutwardShieldDown) / Calipix_y; //槽位裁图终止---距离料头的行数
                            slot.Croprange.Height = MDdist1 - MDdist0;
                            slot.Croprange.Width = (slot.slotInfo.SlotTDSize + slot.slotInfo.OutwardShieldLeft + slot.slotInfo.OutwardShieldRight) / Calipix_x;
                            NormalizaLocation(HeadLineID, HeadLineRow + MDdist0, ImageHeight, out slot.Croprange.StartImageID, out slot.Croprange.StarImageRow);//起
                            NormalizaLocation(HeadLineID, HeadLineRow + MDdist1, ImageHeight, out slot.Croprange.EndImageID, out slot.Croprange.EndImageRow);//止
                            NormalizaLocation(HeadLineID, HeadLineRow + (MDdist0 + MDdist1) / 2, ImageHeight, out slot.Croprange.MidImageID, out slot.Croprange.MidImageRow);//中
                            SlicePD.listSlot.Add(slot);
                        }
                        for (int k = 0; k < m_craft.ListSliceSet[i].ShaveNum; k++)
                        {
                            SlotProduct slot = new SlotProduct();
                            slot.slotInfo = m_craft.ListSlotInfos.Find(c => c.SliceID == i && c.ShaveID == k + 1);
                            slot.IsFinished = false;
                            slot.Slottype = SlottypeEnum.shave;
                            double MDdist0 = (slot.slotInfo.SlotMDPosition - slot.slotInfo.OutwardShieldUp) / Calipix_y; //槽位裁图起始---距离料头的行数
                            double MDdist1 = (slot.slotInfo.SlotMDPosition + slot.slotInfo.SlotMDSize + slot.slotInfo.OutwardShieldDown) / Calipix_y; //槽位裁图终止---距离料头的行数
                            slot.Croprange.Height = MDdist1 - MDdist0;
                            slot.Croprange.Width = (slot.slotInfo.SlotTDSize + slot.slotInfo.OutwardShieldLeft + slot.slotInfo.OutwardShieldRight) / Calipix_x;
                            NormalizaLocation(HeadLineID, HeadLineRow + MDdist0, ImageHeight, out slot.Croprange.StartImageID, out slot.Croprange.StarImageRow);//起
                            NormalizaLocation(HeadLineID, HeadLineRow + MDdist1, ImageHeight, out slot.Croprange.EndImageID, out slot.Croprange.EndImageRow);//止
                            NormalizaLocation(HeadLineID, HeadLineRow + (MDdist0 + MDdist1) / 2, ImageHeight, out slot.Croprange.MidImageID, out slot.Croprange.MidImageRow);//中

                            //初始槽位打孔区的抓边。上下左右边，0919增加左间隙边，右间隙边,这两个边不抓边，直接赋值。
                            for (int j = 0; j < Enum.GetValues(typeof(DirectionEnum)).Length; j++)
                            {
                                string EnumName = ((DirectionEnum)j).ToString();
                                LineInfo lineInfo = new LineInfo() { StrLineName = EnumName };
                                slot.Slotlines.Add(lineInfo);
                            }
                            ExPandSlotSize(slot); //每个打孔区添加四个尺寸
                            SlicePD.listShave.Add(slot);
                            // EAEAProduct.list2EAShave[i].Add(slot);
                        }
                    }
                }
                //EA像素长度
                double EAY = m_craft.EASizeMD / Calipix_y;
                //EA起始结束位置
                NormalizaLocation(HeadLineID, HeadLineRow, ImageHeight, out EAEAProduct.EArange.StartImageID, out EAEAProduct.EArange.StarImageRow);
                NormalizaLocation(HeadLineID, HeadLineRow + EAY, ImageHeight, out EAEAProduct.EArange.EndImageID, out EAEAProduct.EArange.EndImageRow);
                //EA裁图起始结束位置。
                NormalizaLocation(HeadLineID, HeadLineRow - m_craft.EACropShiledMD, ImageHeight, out EAEAProduct.CropEArange.StartImageID, out EAEAProduct.CropEArange.StarImageRow);
                NormalizaLocation(HeadLineID, HeadLineRow + EAY - m_craft.EACropShiledMD, ImageHeight, out EAEAProduct.CropEArange.EndImageID, out EAEAProduct.CropEArange.EndImageRow);
                //涂布长度
                double coatY = m_craft.CoatSizeMD / Calipix_y;
                //涂布起始结束位置
                NormalizaLocation(HeadLineID, HeadLineRow, ImageHeight, out EAEAProduct.Coatrange.StartImageID, out EAEAProduct.Coatrange.StarImageRow);
                NormalizaLocation(HeadLineID, HeadLineRow + coatY, ImageHeight, out EAEAProduct.Coatrange.EndImageID, out EAEAProduct.Coatrange.EndImageRow);

                //初始化相机产品的抓边。
                //for (int i = 0; i < m_craft.ListLines[camID].Count; i++)
                //{
                //    LineInfo lineInfo = new LineInfo() { StrLineName = m_craft.ListLines[camID][i].StrLineName };
                //    EAEAProduct.ListEAlines.Add(lineInfo);
                //}

                //初始化相机0的尺寸。
                for (int i = 0; i < m_craft.ListEASizes.Count; i++)
                {
                    if (m_craft.ListEASizes[i].camera == (CameraEnum)camID && (m_craft.ListEASizes[i].Enable == true))
                    {
                        SizeInfo size = new SizeInfo()
                        {
                            Index = m_craft.ListEASizes[i].Index,
                            Name = m_craft.ListEASizes[i].Name,
                            SizeName = m_craft.ListEASizes[i].Name,
                            Description = m_craft.ListEASizes[i].Description,
                            StandardValue = m_craft.ListEASizes[i].StDValue,
                            upperLimitValue = m_craft.ListEASizes[i].StD_BJ_Up,
                            lowerLimitValue = m_craft.ListEASizes[i].StD_BJ_Down,
                        };
                        EAEAProduct.ListEASizes.Add(size);
                    }
                }

            }
            catch (Exception ee)
            {
                Log4netRecord.ErrorLog.Error(ee);
            }

            return EAEAProduct;
        }

        /// 每个打孔区添加四个尺寸
        /// </summary>
        /// <param name="slot"></param>
        public void ExPandSlotSize(SlotProduct slot)
        {
            int count = Enum.GetValues(typeof(MeasureSlotEnum)).Length;
            //初始化相机0的尺寸。
            for (int i = 0; i < count; i++)
            {
                double StdValue = slot.slotInfo.AllValues[i];
                double UpLimitValue = m_craft.ListSlotSizeLimits[i].UpLimitValue;
                double lowLimitValue = m_craft.ListSlotSizeLimits[i].lowLimitValue;
                if (((MeasureSlotEnum)i) == MeasureSlotEnum.TD左间隙)
                {
                    UpLimitValue = m_craft.SlotGapTDisUp[i];
                    lowLimitValue = -m_craft.SlotGapTDisDown[i];
                }
                if (((MeasureSlotEnum)i) == MeasureSlotEnum.TD右间隙)
                {
                    UpLimitValue = m_craft.SlotGapTDisUp[i + 1]; ;
                    lowLimitValue = -m_craft.SlotGapTDisDown[i + 1];
                }
                SizeInfo size = new SizeInfo()
                {
                    Name = ((MeasureSlotEnum)i).ToString(),
                    SizeName = ((MeasureSlotEnum)i).ToString(),
                    Description = slot.slotInfo.InfoName + ((MeasureSlotEnum)i).ToString(),
                    StandardValue = StdValue,
                    upperLimitValue = StdValue + UpLimitValue,
                    lowerLimitValue = StdValue + lowLimitValue,
                };
                slot.SlotSizes.Add(size);
            }
        }


        //创建
        public HObject CreatCoatBlock(ImageInfo imageInfo, PFTCleanEAProductInfo PFTCleanEA, int SliceIndex, double col1, double col2, double TDShaft, double Calipix_y, double Calipix_x)
        {
            HObject ShieldRegion0;
            HOperatorSet.GenEmptyObj(out ShieldRegion0);
            HObject ShieldRegion;
            HOperatorSet.GenEmptyObj(out ShieldRegion);
            HObject CoatwRegion;
            HOperatorSet.GenEmptyObj(out CoatwRegion);

            double UpOffset = m_craft.CoatShieldMDHead / Calipix_y;
            double DownOffset = m_craft.CoatShieldMDTile / Calipix_y;

            double LeftOffset = m_craft.CoatShieldTDLeft / Calipix_x;
            double RightOffset = m_craft.CoatShieldTDRight / Calipix_x;

            double coatShield1 = col1 + LeftOffset;
            double coatShield2 = col2 - RightOffset;

            try
            {
                HOperatorSet.GenRectangle1(out CoatwRegion, 0, coatShield1, imageInfo.imageHeight - 1, coatShield2);

                //料头帧
                if (imageInfo.image_ID == PFTCleanEA.Coatrange.StartImageID)
                {
                    HOperatorSet.GenRectangle1(out ShieldRegion, 0, coatShield1, PFTCleanEA.Coatrange.StarImageRow + UpOffset, coatShield2);//
                    HOperatorSet.Union2(ShieldRegion0, ShieldRegion, out ShieldRegion0);
                }
                //槽位屏蔽
                if (m_craft.MaterialType)
                {
                    SlotProduct slot = PFTCleanEA.ListEASlicePD[SliceIndex].listSlot[0];
                    double tdpos = slot.slotInfo.SlotTDPosition / Calipix_x;
                    double col01 = col1 + tdpos - slot.slotInfo.OutwardShieldLeft / Calipix_x - TDShaft;
                    double col02 = col1 + tdpos + (slot.slotInfo.SlotTDSize + slot.slotInfo.OutwardShieldRight) / Calipix_x - TDShaft;
                    double row01 = slot.Croprange.StarImageRow;
                    double row02 = slot.Croprange.EndImageRow;
                    if (imageInfo.image_ID == slot.Croprange.StartImageID)
                    {
                        if (slot.Croprange.EndImageID > imageInfo.image_ID)
                        {
                            row02 = imageInfo.imageHeight;
                        }
                        HOperatorSet.GenRectangle1(out ShieldRegion, row01, col01, row02, col02);//
                        HOperatorSet.Union2(ShieldRegion0, ShieldRegion, out ShieldRegion0);
                    }
                    else if (imageInfo.image_ID == slot.Croprange.EndImageID)
                    {
                        HOperatorSet.GenRectangle1(out ShieldRegion, 0, col01, row02, col02);//
                        HOperatorSet.Union2(ShieldRegion0, ShieldRegion, out ShieldRegion0);
                    }
                }

                ////削薄区屏蔽
                for (int i = 0; i < PFTCleanEA.ListEASlicePD[SliceIndex].listShave.Count; i++)
                {
                    SlotProduct shave = PFTCleanEA.ListEASlicePD[SliceIndex].listShave[i];
                    double tdpos = shave.slotInfo.SlotTDPosition / Calipix_x;
                    double col01 = col1 + tdpos - shave.slotInfo.OutwardShieldLeft / Calipix_x - TDShaft;
                    double col02 = col1 + tdpos + (shave.slotInfo.SlotTDSize + shave.slotInfo.OutwardShieldRight) / Calipix_x - TDShaft;
                    double row01 = shave.Croprange.StarImageRow;
                    double row02 = shave.Croprange.EndImageRow;
                    if (imageInfo.image_ID == shave.Croprange.StartImageID)
                    {
                        if (shave.Croprange.EndImageID > imageInfo.image_ID)
                        {
                            row02 = imageInfo.imageHeight;
                        }
                        HOperatorSet.GenRectangle1(out ShieldRegion, row01, col01, row02, col02);//
                        HOperatorSet.Union2(ShieldRegion0, ShieldRegion, out ShieldRegion0);
                    }
                    else if (imageInfo.image_ID == shave.Croprange.EndImageID)
                    {
                        HOperatorSet.GenRectangle1(out ShieldRegion, 0, col01, row02, col02);//
                        HOperatorSet.Union2(ShieldRegion0, ShieldRegion, out ShieldRegion0);
                    }
                }


                //料尾帧
                if (imageInfo.image_ID == PFTCleanEA.Coatrange.EndImageID)
                {
                    HOperatorSet.GenRectangle1(out ShieldRegion, PFTCleanEA.Coatrange.EndImageRow - DownOffset, col1, imageInfo.imageHeight - 1, col2); //
                    HOperatorSet.ConcatObj(ShieldRegion0, ShieldRegion, out ShieldRegion0);
                }
                if (imageInfo.image_ID > PFTCleanEA.Coatrange.EndImageID)
                {
                    HOperatorSet.GenRectangle1(out ShieldRegion, 0, col1, imageInfo.imageHeight - 1, col2); //
                    HOperatorSet.ConcatObj(ShieldRegion0, ShieldRegion, out ShieldRegion0);
                }

                HOperatorSet.Difference(CoatwRegion, ShieldRegion0, out ShieldRegion0);
            }
            catch (Exception ex)
            {

                Log4netRecord.ErrorLog.Error(ex);
            }
            CoatwRegion.Dispose();
            ShieldRegion.Dispose();
            return ShieldRegion0;
        }


        /// <summary>
        /// 两张图图像拼接，要求：数量2，宽度相同。
        /// </summary>
        /// <param name="ho_SourceImage0"></param>
        /// <param name="ho_SourceImage1"></param>
        /// <param name=""></param>
        public void TileImage(HObject ho_SourceImage0, HObject ho_SourceImage1, out HObject ho_ImageTiled)
        {

            HObject ho_ObjectsConcat = null;
            HOperatorSet.GenEmptyObj(out ho_ObjectsConcat);
            HOperatorSet.GenEmptyObj(out ho_ImageTiled);

            try
            {
                HTuple hv_Width0, hv_Height0;  //获取图像宽高 
                HOperatorSet.GetImageSize(ho_SourceImage0, out hv_Width0, out hv_Height0);
                HTuple hv_Width1, hv_Height1;  //获取图像宽高 
                HOperatorSet.GetImageSize(ho_SourceImage1, out hv_Width1, out hv_Height1);
                if (hv_Width0.I != hv_Width1.I)
                {

                    ho_ObjectsConcat.Dispose();
                    return;
                }
                HOperatorSet.ConcatObj(ho_SourceImage0, ho_SourceImage1, out ho_ObjectsConcat);
                HOperatorSet.TileImagesOffset(ho_ObjectsConcat, out ho_ImageTiled, (new HTuple(0)).TupleConcat(hv_Height0), (new HTuple(0)).TupleConcat(0),
                                             (new HTuple(-1)).TupleConcat(-1), (new HTuple(-1)).TupleConcat(-1), (new HTuple(-1)).TupleConcat(-1), (new HTuple(-1)).TupleConcat(-1),
                                              hv_Width0, hv_Height0 + hv_Height1);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
            ho_ObjectsConcat.Dispose();
        }

        /// <summary>
        /// 拼接若干张等宽的图片
        /// </summary>
        /// <param name="inputImages">传入若干等宽图片图片</param>
        /// <param name="ResImage">输出拼接后的图片</param>
        public static void TileImage2Offset(HObject inputImages, out HObject ResImage)
        {
            HOperatorSet.GenEmptyObj(out ResImage);
            HTuple TotalH = new HTuple();
            HTuple InputRows = new HTuple();
            HTuple InputCols = new HTuple();
            HTuple InputR1 = new HTuple();
            HTuple InputR2 = new HTuple();
            HTuple InputC1 = new HTuple();
            HTuple InputC2 = new HTuple();
            try
            {
                HOperatorSet.CountObj(inputImages, out HTuple Num);
                //if(Num < 2)
                //{
                //    return;
                //}
                //if (Num >= 2)
                {
                    HOperatorSet.GetImageSize(inputImages, out HTuple w, out HTuple h);
                    InputRows = InputRows.TupleInsert(0, 0);
                    for (int i = 0; i < Num - 1; i++)
                    {
                        HTuple hTuple = h.TupleSelectRange(0, i);
                        hTuple = hTuple.TupleSum() - 1;
                        InputRows = InputRows.TupleConcat(hTuple);
                    }
                    TotalH = h.TupleSum() - Num;
                    for (int i = 0; i < Num; i++)
                    {
                        InputCols = InputCols.TupleConcat(0);
                        InputR1 = InputR1.TupleConcat(-1);
                        InputR2 = InputR2.TupleConcat(-1);
                        InputC1 = InputC1.TupleConcat(-1);
                        InputC2 = InputC2.TupleConcat(-1);
                    }
                    HOperatorSet.TileImagesOffset(inputImages, out ResImage, InputRows, InputCols,
                                                                            InputR1, InputC1,
                                                                            InputR2, InputC2,
                                                                            w[0], TotalH);
                }
            }
            catch (Exception ex)
            {
                // LogName.ErrorLog.Info(ex);
            }
        }
        //把原图或者裁图内的抓边，转换成ID和行的lineinfo
        public LineInfo NormalizeLine(LineInfo ImglineInfo, int ImagestartID, double ImageStartRow, double ImageStartCol, int ImageId, double ImageHeight)
        {
            LineInfo NormallineInfo = new LineInfo();
            if (true) //归一化到相机图像，ID+行
            {
                NormallineInfo.StrLineName = ImglineInfo.StrLineName;
                NormallineInfo.IsLineSuccessfull = ImglineInfo.IsLineSuccessfull;
                NormalizaLocation(ImagestartID, ImageStartRow + ImglineInfo.RowBegin, ImageHeight, out NormallineInfo.RowBeginID, out NormallineInfo.RowBegin);
                NormalizaLocation(ImagestartID, ImageStartRow + ImglineInfo.RowEnd, ImageHeight, out NormallineInfo.RowEndID, out NormallineInfo.RowEnd);
                //把抓边ID限制到当前帧ID，方便显示
                NormallineInfo.RowBegin = (NormallineInfo.RowBeginID - ImageId) * ImageHeight + NormallineInfo.RowBegin;
                NormallineInfo.RowBeginID = ImageId;
                //把抓边ID限制到当前帧ID，方便显示
                NormallineInfo.RowEnd = (NormallineInfo.RowEndID - ImageId) * ImageHeight + NormallineInfo.RowEnd;
                NormallineInfo.RowEndID = ImageId;

                NormallineInfo.ColBegin = ImageStartCol + ImglineInfo.ColBegin;
                NormallineInfo.ColEnd = ImageStartCol + ImglineInfo.ColEnd;
            }
            return NormallineInfo;
        }

        //把归一化完成的抓边，抓换成EA大图的边
        public LineInfo EAAddline(LineInfo ImglineInfo, int ImagestartID, double ImageStartRow, double ImageStartCol, double ImageHeight)
        {
            //ImglineInfo为已经归一化到相机图像的边
            LineInfo NormallineInfo = new LineInfo();
            if (true)//归一化到EA图像,同一大帧，只做行
            {
                NormallineInfo.StrLineName = ImglineInfo.StrLineName;
                NormallineInfo.IsLineSuccessfull = ImglineInfo.IsLineSuccessfull;
                NormallineInfo.RowBeginID = 0;
                NormallineInfo.RowBegin = (ImglineInfo.RowBeginID - ImagestartID) * ImageHeight + ImglineInfo.RowBegin - ImageStartRow;
                NormallineInfo.RowEndID = 0;
                NormallineInfo.RowEnd = (ImglineInfo.RowEndID - ImagestartID) * ImageHeight + ImglineInfo.RowEnd - ImageStartRow;
                NormallineInfo.ColBegin = ImglineInfo.ColBegin - ImageStartCol;
                NormallineInfo.ColEnd = ImglineInfo.ColEnd - ImageStartCol;
            }
            return NormallineInfo;
        }

        //把原图或者裁图内的抓边，转换成ID和行的Regioninfo
        public HObject NormalizeRegion(HObject PartImgRegion, int ImagestartID, double ImageStartRow, double ImageStartCol, int ImageId, double ImageHeight)
        {
            HObject RegionMoved0;
            HOperatorSet.GenEmptyObj(out RegionMoved0);
            if (true) //归一化到相机图像，ID+行
            {
                //把抓边ID限制到当前帧ID，方便显示
                double RowMove = (ImagestartID - ImageId) * ImageHeight + ImageStartRow;
                //把抓边ID限制到当前帧ID，方便显示
                double colMove = ImageStartCol;
                HOperatorSet.MoveRegion(PartImgRegion, out RegionMoved0, RowMove, colMove);
            }
            //PartImgRegion?.Dispose();
            return RegionMoved0;
        }
        //把原图的region添加到EA产品中
        public HObject EAAddRegion(HObject PartImgRegion, int EAstartID, double EAStartRow, int ImageId, double ImageHeight)
        {
            HObject RegionMoved0;
            HOperatorSet.GenEmptyObj(out RegionMoved0);
            if (true) //归一化到相机图像，ID+行
            {
                // HOperatorSet.SmallestRectangle1(PartImgRegion, out HTuple row1, out HTuple col1, out HTuple row2, out HTuple col2);
                //把抓边ID限制到当前帧ID，方便显示
                double RowMove = (ImageId - EAstartID) * ImageHeight - EAStartRow;
                HOperatorSet.MoveRegion(PartImgRegion, out RegionMoved0, RowMove, 0);
                //HOperatorSet.GenRectangle1(out RegionMoved0, row1 + RowMove, col1, row2 + RowMove, col2);
                HOperatorSet.AreaCenter(RegionMoved0, out HTuple area, out HTuple row, out HTuple column);
            }
            PartImgRegion?.DisposeEx();
            return RegionMoved0;
        }

        //#region 辅助函数
        public void CalcuEASize(int cameraID, double imageHeight, LineInfo line, List<SizeItemInfo> ListEASizes, PFTCleanEAProductInfo pFTCleanEACam0, PFTCleanEAProductInfo pFTCleanEACam1, List<SizeInfo> ImageSizeInfos)
        {
            MessageNotify.AddDebugLog($"当前方法：CalcuEASize中的传入的相机ID为{cameraID}，抓边信息为{line.StrLineName}，抓边是否成功为{line.IsLineSuccessfull}");

            //try
            //{

            //    //把传入的边转变成EA格式，替换到对应位置。
            //    if (cameraID == 0)
            //    {
            //        //根据抓边索引能计算的项目，
            //        int existingLineIndex = pFTCleanEACam0.ListEAlines.FindIndex(x => x.StrLineName == line.StrLineName);
            //        if (existingLineIndex == -1)
            //        {
            //            MessageNotify.AddErrorLog("cam1传入列表中不存在对应抓边，排查传入抓边是否正确");
            //            return;
            //        }
            //        else
            //        {
            //            //替换成实际抓边
            //            LineInfo EAline = EAAddline(line, pFTCleanEACam0.CropEArange.StartImageID, pFTCleanEACam0.CropEArange.StarImageRow, 0, imageHeight);
            //            EAline.IsAdd = true;
            //            pFTCleanEACam0.ListEAlines[existingLineIndex] = EAline;
            //        }
            //    }
            //    else
            //    {
            //        //根据抓边索引能计算的项目，
            //        int existingLineIndex = pFTCleanEACam1.ListEAlines.FindIndex(x => x.StrLineName == line.StrLineName);
            //        if (existingLineIndex == -1)
            //        {
            //            MessageNotify.AddErrorLog("cam2传入列表中不存在对应抓边，排查传入抓边是否正确");
            //            return;
            //        }
            //        else
            //        {
            //            //替换成实际抓边
            //            LineInfo EAline = EAAddline(line, pFTCleanEACam1.CropEArange.StartImageID, pFTCleanEACam1.CropEArange.StarImageRow, 0, imageHeight);
            //            EAline.IsAdd = true;
            //            pFTCleanEACam1.ListEAlines[existingLineIndex] = EAline;
            //        }
            //    }



            //    ////根据抓边索引能计算的项目，
            //    //var existingLineIndex0 = pFTCleanEACam0.ListEAlines.FindIndex(x => x.StrLineName == line.StrLineName);
            //    //var existingLineIndex1 = pFTCleanEACam1.ListEAlines.FindIndex(x => x.StrLineName == line.StrLineName);


            //    //if (existingLineIndex0 == -1 && existingLineIndex1 == -1)
            //    //{
            //    //    MessageNotify.AddErrorLog("传入列表中不存在对应抓边，排查传入抓边是否正确");
            //    //    return;
            //    //}


            //    ////判断existingLineIndex0和existingLineIndex1哪个有效，并替换成实际抓边
            //    //if (existingLineIndex0 != -1 && cameraID == 0)
            //    //{
            //    //    if (existingLineIndex0 == 6)
            //    //    {

            //    //    }
            //    //    //替换成实际抓边
            //    //    LineInfo EAline = EAAddline(line, pFTCleanEACam0.CropEArange.StartImageID, pFTCleanEACam0.CropEArange.StarImageRow, 0, imageHeight);
            //    //    EAline.IsAdd = true;
            //    //    pFTCleanEACam0.ListEAlines[existingLineIndex0] = EAline;
            //    //}
            //    //if (existingLineIndex1 != -1 && cameraID == 1)
            //    //{
            //    //    //替换成实际抓边
            //    //    LineInfo EAline = EAAddline(line, pFTCleanEACam1.CropEArange.StartImageID, pFTCleanEACam1.CropEArange.StarImageRow, 0, imageHeight);
            //    //    EAline.IsAdd = true;
            //    //    pFTCleanEACam1.ListEAlines[existingLineIndex1] = EAline;
            //    //}
            //    //提取符合条件的尺寸项目
            //    //List<SizeItemInfo> ListEASizesCanCalcu = new List<SizeItemInfo>();
            //    //List<List<LineInfo>> EAlinesTemp = new List<List<LineInfo>>();
            //    //EAlinesTemp.Add(pFTCleanEACam0.ListEAlines);
            //    //EAlinesTemp.Add(pFTCleanEACam1.ListEAlines);


            //    //EAlinesTemp，第一层list，对应相机，第二层list，对应抓边
            //    //从ListEASizesCanCalcu 中取出所有可以计算的尺寸项，计算尺寸，扩充AddSizeInfos，要先从EAlinesTemp中取出对应相机的抓边，再计算尺寸
            //    //提取需要计算的尺寸项

            //    //提取符合条件的尺寸项目
            //    List<SizeItemInfo> ListEASizesCanCalcu = new List<SizeItemInfo>();
            //    List<List<LineInfo>> EAlinesTemp = new List<List<LineInfo>>();
            //    EAlinesTemp.Add(pFTCleanEACam0.ListEAlines);
            //    EAlinesTemp.Add(pFTCleanEACam1.ListEAlines);
            //    foreach (SizeItemInfo sizeItem in ListEASizes)
            //    {
            //        if (sizeItem.Enable == false)
            //        {
            //            continue;
            //        }
            //        //将sizeItem.camera1转成CameraEnum枚举
            //        Enum.TryParse(sizeItem.camera1, out CameraEnum cameraEnum1);
            //        Enum.TryParse(sizeItem.camera2, out CameraEnum cameraEnum2);

            //        if ((int)sizeItem.camera > 1)
            //        {
            //            continue;
            //        }
            //        //检查列表的ListEASizes的相机索引是否超出范围
            //        if (!(cameraEnum1 == CameraEnum.Cam1 || cameraEnum2 == CameraEnum.Cam2))
            //        {
            //            MessageNotify.AddErrorLog($"尺寸计算，传入的相机参数有误，请检查_camera1_{cameraEnum1.ToString()}_camera2_{cameraEnum2.ToString()}");
            //            continue;
            //        }

            //        if (EAlinesTemp[(int)cameraEnum1].Find(x => x.StrLineName == sizeItem.Line1) == null)
            //        {
            //            continue;
            //        }
            //        if (EAlinesTemp[(int)cameraEnum2].Find(x => x.StrLineName == sizeItem.Line2) == null)
            //        {
            //            continue;
            //        }
            //        else
            //        {

            //            var sizeInfoIndex0 = pFTCleanEACam0.ListEASizes.FindIndex(x => x.Name == sizeItem.Name);
            //            var sizeInfoIndex1 = pFTCleanEACam1.ListEASizes.FindIndex(x => x.Name == sizeItem.Name);

            //            SizeInfo sizeInfo = new SizeInfo();
            //            if (sizeInfoIndex0 != -1 && sizeInfoIndex1 != -1)
            //            {
            //                MessageNotify.AddErrorLog($"请检查传入的尺寸名称是否重复，尺寸名称为{sizeItem.Name}，在两个产品尺寸列表同时存在");
            //                continue;
            //            }
            //            else if (sizeInfoIndex0 == -1 && sizeInfoIndex1 == -1)
            //            {
            //                MessageNotify.AddErrorLog($"请检查传入的尺寸名称是否存在，尺寸名称为{sizeItem.Name}，在两个产品尺寸列表都不存在");
            //                continue;
            //            }
            //            else if (sizeInfoIndex0 != -1)
            //            {

            //                sizeInfo = pFTCleanEACam0.ListEASizes[sizeInfoIndex0];
            //                if (sizeInfo.IsCalcued)
            //                {
            //                    continue;
            //                }
            //            }
            //            else if (sizeInfoIndex1 != -1)
            //            {
            //                sizeInfo = pFTCleanEACam1.ListEASizes[sizeInfoIndex1];
            //                if (sizeInfo.IsCalcued)
            //                {
            //                    continue;
            //                }
            //            }

            //            //开始计算尺寸
            //            // 使用字典加速 LineName 查找

            //            if ((int)cameraEnum1 > 2 || (int)cameraEnum2 > 2)
            //            {
            //                MessageNotify.AddErrorLog($"尺寸计算，传入的相机参数有误，请检查_camera1_{cameraEnum1.ToString()}_camera2_{cameraEnum2.ToString()}");
            //                continue;
            //            }
            //            var line1 = EAlinesTemp[(int)(cameraEnum1)].Find(x => x.StrLineName == sizeItem.Line1 && x.IsAdd == true);
            //            var line2 = EAlinesTemp[(int)(cameraEnum2)].Find(x => x.StrLineName == sizeItem.Line2 && x.IsAdd == true);

            //            if (line1 == null || line2 == null)
            //            {
            //                continue;
            //            }

            //            if (!line1.IsLineSuccessfull || !line2.IsLineSuccessfull)
            //            {
            //                sizeInfo.IsCalcued = true;
            //                sizeInfo.DataResult = sizeInfo.DataResult * sizeItem.StD_RealFixRito + sizeItem.StD_RealFixOffset;
            //                GetSizeJudgeInfo(sizeItem, ref sizeInfo);
            //                ImageSizeInfos.Add(sizeInfo);
            //                continue;
            //            }

            //            // Y方向比例
            //            double YScale = 0;
            //            double XScale = 0;
            //            double YMis = 0;
            //            if (sizeItem.EditorType == EditorTypeEnum.线距离Y.ToString())
            //            {

            //                if (sizeItem.camera1 == sizeItem.camera.ToString() || sizeItem.camera2 == sizeItem.camera.ToString())
            //                {

            //                    YScale = Project.Instance.cameraManager[sizeItem.camera].m_CamParam.CalibrationValue_Y;

            //                    //计算两个相机Y方向的尺寸
            //                    double size = 0;


            //                    if (cameraEnum1 != cameraEnum2)
            //                    {
            //                        size = (((line1.RowBegin + line1.RowEnd) / 2) - ((line2.RowBegin + line2.RowEnd) / 2)) * YScale;
            //                    }
            //                    else
            //                    {
            //                        size = Math.Abs(((line1.RowBegin + line1.RowEnd) / 2) - ((line2.RowBegin + line2.RowEnd) / 2)) * YScale;
            //                    }



            //                    //工艺判断尺寸NG项目
            //                    sizeInfo.DataResult = Math.Round(size, 2) * sizeItem.StD_RealFixRito + sizeItem.StD_RealFixOffset;
            //                    sizeInfo.IsCalcued = true;
            //                    GetSizeJudgeInfo(sizeItem, ref sizeInfo);
            //                    ImageSizeInfos.Add(sizeInfo);

            //                }
            //                else
            //                {
            //                    continue;
            //                }



            //            }
            //            if (sizeItem.EditorType == EditorTypeEnum.线距离X.ToString())
            //            {
            //                if (cameraEnum1 != cameraEnum2)
            //                {
            //                    //计算两个相机X方向的尺寸
            //                    //1.先找出当前相机的基准边，没有就跳过
            //                    LineInfo baseLine1 = EAlinesTemp[(int)cameraEnum1].Find(x => x.StrLineName == "涂布左基准边");
            //                    LineInfo baseLine2 = EAlinesTemp[(int)cameraEnum2].Find(x => x.StrLineName == "涂布左基准边");

            //                    if (baseLine1 == null || baseLine2 == null)
            //                    {
            //                        continue;
            //                    }
            //                    //2.计算当前相机到基准线的距离
            //                    XScale = Project.Instance.cameraManager[cameraEnum1].m_CamParam.CalibrationValue_X;
            //                    CalculateLinesX(baseLine1, line1, out double line1X);
            //                    line1X = line1X * XScale;
            //                    XScale = Project.Instance.cameraManager[cameraEnum2].m_CamParam.CalibrationValue_X;
            //                    CalculateLinesX(baseLine2, line2, out double line2X);
            //                    line2X = line2X * XScale;


            //                    //3.计算两个相机X方向的尺寸
            //                    double size = (line1X - line2X);

            //                    //工艺判断尺寸NG项目
            //                    sizeInfo.DataResult = Math.Round(size, 2) * sizeItem.StD_RealFixRito + sizeItem.StD_RealFixOffset;

            //                    sizeInfo.IsCalcued = true;
            //                    GetSizeJudgeInfo(sizeItem, ref sizeInfo);
            //                    ImageSizeInfos.Add(sizeInfo);

            //                }
            //                if (cameraEnum1 == cameraEnum2)
            //                {
            //                    XScale = Project.Instance.cameraManager[cameraEnum1].m_CamParam.CalibrationValue_X;
            //                    CalculateLinesX(line1, line2, out double size);
            //                    size = size * XScale;

            //                    //工艺判断尺寸NG项目
            //                    sizeInfo.DataResult = Math.Round(size, 2) * sizeItem.StD_RealFixRito + sizeItem.StD_RealFixOffset;
            //                    sizeInfo.IsCalcued = true;
            //                    GetSizeJudgeInfo(sizeItem, ref sizeInfo);
            //                    ImageSizeInfos.Add(sizeInfo);
            //                }

            //            }


            //            //抓边数据记录
            //            m_threadPool.RunAction(new Action(() =>
            //            {
            //                //表格标题
            //                List<string> tableTitle = new List<string>
            //                {
            //                    "时间",
            //                    "相机",
            //                    "尺寸名称",
            //                    "抓边1名称",
            //                    "是否成功",
            //                    "开始帧号",
            //                    "开始行",
            //                    "开始列",
            //                    "结束帧号",
            //                    "结束行",
            //                    "结束列",
            //                    "抓边2名称",
            //                    "是否成功",
            //                    "开始帧号",
            //                    "开始行",
            //                    "开始列",
            //                    "结束帧号",
            //                    "结束行",
            //                    "结束列",
            //                    "计算方式",
            //                    "距离",

            //                };

            //                List<string> data = new List<string>();
            //                data.Add(System.DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss"));
            //                data.Add(cameraEnum1.ToString());
            //                data.Add(sizeItem.Description);
            //                data.Add(line1.StrLineName);
            //                data.Add(line1.IsLineSuccessfull.ToString());
            //                data.Add(line1.RowBeginID.ToString());
            //                data.Add(line1.RowBegin.ToString());
            //                data.Add(line1.ColBegin.ToString());
            //                data.Add(line1.RowEndID.ToString());
            //                data.Add(line1.RowEnd.ToString());
            //                data.Add(line1.ColEnd.ToString());
            //                data.Add(line2.StrLineName);
            //                data.Add(line2.IsLineSuccessfull.ToString());
            //                data.Add(line2.RowBeginID.ToString());
            //                data.Add(line2.RowBegin.ToString());
            //                data.Add(line2.ColBegin.ToString());
            //                data.Add(line2.RowEndID.ToString());
            //                data.Add(line2.RowEnd.ToString());
            //                data.Add(line2.ColEnd.ToString());
            //                data.Add(sizeItem.EditorType);
            //                data.Add(sizeInfo.DataResult.ToString());

            //                FileOperationCSV.WriteDataLock_CSV(Project.Instance.paramPathSet.PFTProdSizeDataLinePath,
            //              tableTitle.ToArray(), data.ToArray(), cameraEnum1.ToString());
            //            }));

            //        }

            //    }
            //}
            //catch (Exception ee)
            //{
            //    MessageNotify.AddErrorLog($"计算EA尺寸失败，错误信息：{ee.ToString()}");
            //}





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

        /// <summary>
        /// 映射抓边位置
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="lineInfo"></param>
        /// <returns></returns>
        public LineInfo GetLineInfo(CameraEnum camera, LineInfo lineInfo, double imageHeight)
        {
            double row = Project.Instance.cameraManager[camera].m_CamParam.ToMainCamDistance;
            NormalizaLocation(ref lineInfo, row, imageHeight);
            return lineInfo;
        }



        #region 方法模板
        public void ExecuteMethod(Action action, [CallerMemberName] string methodName = "")
        {
            string className = MethodBase.GetCurrentMethod().DeclaringType?.FullName ?? "UnknownClass";
            try
            {
                // 执行传入的委托
                action();
            }
            catch (Exception ee)
            {
                // 构造完整的异常信息，包含类名、方法名和异常详情
                string fullErrorMessage = $"[{className}.{methodName}] 发生异常: {ee.ToString()}";
                // 输出带调用堆栈的详细错误信息
                MessageNotify.AddErrorLog(fullErrorMessage);
            }
        }
        #endregion

        /// <summary>
        /// 计算 TD 与 MD 方向槽位间距并写回 SlotProduct。
        /// </summary>
        /// <param name="cleanCraft">工艺参数对象。</param>
        /// <param name="slotMatrixA">第一组槽位矩阵。</param>
        /// <param name="slotMatrixB">第二组槽位矩阵。</param>
        /// <param name="pixelSizeX">X 方向像素物理尺寸（mm/px）。</param>
        /// <param name="pixelSizeY">Y 方向像素物理尺寸（mm/px）。</param>
        /// <param name="isSuccess">输出：计算是否完全成功。</param>
        //public void MeasureDataDistance(
        //    PFTCleanCraft cleanCraft,
        //    List<List<SlotProduct>> slotMatrixA,
        //    List<List<SlotProduct>> slotMatrixB,
        //    double pixelSizeX,
        //    double pixelSizeY,
        //    out bool isSuccess)
        //{
        //    isSuccess = ExecuteMethod(() =>
        //    {
        //        /* ====== 1. 参数合法性校验 ====== */
        //        if (cleanCraft?.ListSliceSet?[0] == null)
        //            return false;

        //        /* ====== 2. 扁平化所有槽位 ====== */
        //        var allSlots = slotMatrixA
        //                     .Concat(slotMatrixB)
        //                     .SelectMany(row => row)
        //                     .ToList();

        //        /* ====== 3. TD 间距计算 ====== */
        //        for (int shaveId = 0; shaveId < cleanCraft.ListSliceSet[0].ShaveNum; shaveId++)
        //        {
        //            for (int sliceId = 1; sliceId <= cleanCraft.SliceCount - 1; sliceId++)
        //            {
        //                var cur = allSlots.Find(s => s.slotInfo.SliceID == shaveId &&
        //                                              s.slotInfo.ShaveID == sliceId);
        //                var next = allSlots.Find(s => s.slotInfo.SliceID == shaveId &&
        //                                              s.slotInfo.ShaveID == sliceId + 1);
        //                if (cur == null || next == null)
        //                {
        //                    MessageNotify.AddErrorLog("TD slot data missing.");
        //                    return false;
        //                }

        //                double tdWidth = Math.Abs(
        //                    (next.lineInfos[(int)DirectionEnum.左边].ColBegin +
        //                     next.lineInfos[(int)DirectionEnum.左边].ColEnd -
        //                     cur.lineInfos[(int)DirectionEnum.左边].ColBegin -
        //                     cur.lineInfos[(int)DirectionEnum.左边].ColEnd) * 0.5 * pixelSizeX);

        //                if (cur.slotInfo.BelongCam != next.slotInfo.BelongCam)
        //                {
        //                    double exOffset = cleanCraft.EXCam1Col - cleanCraft.EXCam2Col - cleanCraft.Cam1BaseCol;
        //                    tdWidth += exOffset * pixelSizeX;
        //                }

        //                cur.sizeInfos[(int)MeasureSlotEnum.TD间距].DataResult = tdWidth;
        //                next.sizeInfos[(int)MeasureSlotEnum.TD间距].DataResult = tdWidth;
        //            }
        //        }

        //        /* ====== 4. MD 间距计算 ====== */
        //        for (int sliceId = 0; sliceId < cleanCraft.SliceCount; sliceId++)
        //        {
        //            for (int shaveId = 1; shaveId <= cleanCraft.ListSliceSet[0].ShaveNum - 1; shaveId++)
        //            {
        //                var cur = allSlots.Find(s => s.slotInfo.ShaveID == sliceId + 1 &&
        //                                              s.slotInfo.SliceID == shaveId);
        //                var next = allSlots.Find(s => s.slotInfo.ShaveID == sliceId + 1 &&
        //                                              s.slotInfo.SliceID == shaveId + 1);
        //                if (cur == null || next == null)
        //                {
        //                    MessageNotify.AddErrorLog("MD slot data missing.");
        //                    return false;
        //                }

        //                double mdWidth = Math.Abs(
        //                    (next.lineInfos[(int)DirectionEnum.上边].RowBegin +
        //                     next.lineInfos[(int)DirectionEnum.上边].RowEnd -
        //                     cur.lineInfos[(int)DirectionEnum.上边].RowBegin -
        //                     cur.lineInfos[(int)DirectionEnum.上边].RowEnd) * 0.5 * pixelSizeY);

        //                cur.sizeInfos[(int)MeasureSlotEnum.MD间距].DataResult = mdWidth;
        //                next.sizeInfos[(int)MeasureSlotEnum.MD间距].DataResult = mdWidth;
        //            }
        //        }

        //        return true; // 全部成功
        //    });
        //}


    }

}


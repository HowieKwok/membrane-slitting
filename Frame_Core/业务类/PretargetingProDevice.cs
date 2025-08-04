using HalconDotNet;
using LogTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Frame_Core
{
    public class PretargetingProDevice : BaseProDevice
    {
        private OverHangCraft m_craft { get; set; } = new OverHangCraft();

        public PretargetingProDevice()
        {
            m_nCamCount = 4;
            m_nProcessCount = 4;

            for (int i = 0; i < m_nCamCount; i++)
            {
                CameraEnum camEnum = (CameraEnum)i;
                m_DicCamOperaParam[i] = new AreaCamParam(((CameraEnum)i).ToString()) { cameraEnum = camEnum };
                m_DicCamOperaParam_Sim[i] = new AreaCamParam(((CameraEnum)i).ToString()) { cameraEnum = camEnum };

            }
            Project.Instance.BindCamImageSource(m_DicCamOperaParam);
            INIPro();
            m_craft.CamNum = m_nCamCount;
            BuildProcessServiceList();
        }

        public override void INIPro()
        {
            m_craft = (OverHangCraft)Project.Instance.craftBase;

        }
        public override bool INIProduct()
        {
            try
            {

                foreach (var item in m_DicCamOperaParam)
                {
                    item.Value?.Dispose();
                    //m_DicCamOperaParam[item.Key] = new DieCutBurrDetectParameter();
                }

                //m_DicCamOperaParam.Clear();
                //for (int i = 0; i < m_nCamCount; i++)
                //{
                //    m_DicCamOperaParam[i] = new AreaCamParam(((CameraEnum)i).ToString());
                //}
                //Project.Instance.RemoveBindCamImageSource();
                //Project.Instance.BindCamImageSource(m_DicCamOperaParam);
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

                //foreach (var item in Project.Instance.m_ProcessList)
                //{
                //    item.process?.Dispose();
                //}

                //m_DicCamOperaParam_Sim.Clear();

                //for (int i = 0; i < m_nCamCount; i++)
                //{
                //    m_DicCamOperaParam_Sim[i] = new AreaCamParam(((CameraEnum)i).ToString()); ;
                //}



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
            for (int i = 0; i < m_craft.CamNum; i++)
            {
                int ID = Project.Instance.CreateProcess();
                ProcessServices TempProcessBase = Project.Instance.GetProcessServiceById(ID);

                TempProcessBase.process.processInfo.ProcessName = Project.Instance.cameraManager[(CameraEnum)i].m_CamParam.CameraName;
                TempProcessBase.m_eventINIProcess += IniProcessBase;
                TempProcessBase.m_eventRUNProcess += ProcessingBase;
                TempProcessBase.InitProcess();
                TempProcessBase.OpeningThread();
            }
        }

        public void IniProcessBase(Process process)
        {

            try
            {
                process.m_ModuleObjList.Clear();
                int CamID = process.processInfo.ProcessID;
                IPlugin Plugin;
                string pluginName;
                ParamsMeasureLine Params;
                foreach (var item in m_craft.ListLines[CamID])
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

        public void ProcessingBase(Process process)
        {
            try
            {
                int CamID = process.processInfo.ProcessID;
                SetThreadAffinityMask(GetCurrentThread(), new UIntPtr(SetCpuID(CamID)));
                //主相机图像参数
                ImageInfo InfoImgInfo;
                //标定系数
                double Calipix_y = Project.Instance.cameraManager[(CameraEnum)CamID].m_CamParam.CalibrationValue_Y;
                double Calipix_x = Project.Instance.cameraManager[(CameraEnum)CamID].m_CamParam.CalibrationValue_X;

                bool ExecuteSuccess = false;
                // 选择参数和队列基于是否模拟
                AreaCamParam pretargetParam = Project.Instance.Is_simulation ? m_DicCamOperaParam_Sim[CamID] as AreaCamParam : m_DicCamOperaParam[CamID] as AreaCamParam;


                var imageQueue = pretargetParam.alreadyDealGrabImage;

                CamEACornerProduct camEAPD; ;
                pretargetParam.semaphoreSlimCam.Wait();

                string str = "流程--" + CamID.ToString() + "信号量--" + pretargetParam.semaphoreSlimCam.CurrentCount.ToString() + "图像数--" + imageQueue.Count.ToString();

                if (imageQueue.Count > 0) // 检查从相机的图像队列是否有图像
                {
                    //调试模式
                    if (!(Project.Instance.cameraManager[(CameraEnum)CamID].m_ImageSaveParamer.IsDebug == 1))
                    {
                        if (PretargetingProDevice.DevIsStop())
                        {
                            Log4netRecord.RunLog.Info("设备停止,释放图像资源");
                            imageQueue.Dequeue()?.Dispose();
                            return;
                        }
                    }

                    //区分首次拍照和二次拍照
                    if (pretargetParam.bIsStartShoot)
                    {
                        pretargetParam.bIsStartShoot = false;
                        pretargetParam.bIsEndShoot = true;
                        pretargetParam.nEAIndex = 1;
                    }
                    if (pretargetParam.bIsEndShoot)
                    {
                        pretargetParam.bIsStartShoot = true;
                        pretargetParam.bIsEndShoot = false;
                        pretargetParam.nEAIndex = 2;
                    }
                    //图像出队
                    InfoImgInfo = imageQueue.Dequeue();//出队
                    camEAPD = new CamEACornerProduct() { intCamEnum = CamID, EAIndex = pretargetParam.nEAIndex, CamFrame = InfoImgInfo.image_ID, };


                    //计时            
                    HOperatorSet.CountSeconds(out camEAPD.Image_StartTime);
                    //局部变量
                    List<MessageShowInfo> MesssgeShowInfos = new List<MessageShowInfo>();
                    List<RegionShowInfo> RegionShowInfos = new List<RegionShowInfo>();
                    List<LineInfo> LineShowInfos = new List<LineInfo>();
                   
                    pretargetParam.nEANum++;
                    MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = str, m_stuGetColor = HalconColor.green });
                    MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = "帧号：" + InfoImgInfo.image_ID.ToString(), m_stuGetColor = HalconColor.green });



                    //抓边，并把边结果添加到显示队列，
                    Project.Instance.cameraManager[(CameraEnum)CamID].param_PretargetingCalib.BaseLinesTemp = new List<LineInfo>();
                    for (int i = 0; i < m_craft.ListLines[CamID].Count(); i++)
                    {
                        //槽位检测：GrooveIndex，第几排槽
                        IPlugin PluginMline = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == m_craft.ListLines[CamID][i].StrLineName);
                        ParamsMeasureLine MlineParams = PluginMline.PluginParams as ParamsMeasureLine; //里氏转换
                        MlineParams.ho_Image = InfoImgInfo.ho_image;
                        bool isSuccess = PluginMline.ExecuteObj();
                        LineInfo line = new LineInfo();
                        DetectEdgeValueAssign(MlineParams, isSuccess, line, m_craft.ListLines[CamID][i].StrLineName, MesssgeShowInfos, LineShowInfos, RegionShowInfos);
                        if (isSuccess)
                        {                       
                            camEAPD.BaseLines.Add(line);                         
                            Project.Instance.cameraManager[(CameraEnum)CamID].param_PretargetingCalib.BaseLinesTemp.Add(line);

                        }
                    }
                    //抓边完成后，计算交点。
                    for (int i = 0; i < m_craft.ListSizeEdits[CamID].Count; i++)
                    {
                        CoorPoint point = new CoorPoint() { strCrossName = m_craft.ListSizeEdits[CamID][i].Name, CCamera = (CameraEnum)CamID, AffineCamera = m_craft.BaseCamera };

                        LineInfo line1 = LineShowInfos.Find(c => c.StrLineName == m_craft.ListSizeEdits[CamID][i].Line1);
                        LineInfo line2 = LineShowInfos.Find(c => c.StrLineName == m_craft.ListSizeEdits[CamID][i].Line2);
                        if (line1 != null && line2 != null)
                        {
                            if (line1.IsLineSuccessfull && line2.IsLineSuccessfull)
                            {
                                bool flag = IntersectionLines(line1, line2, out double pointY, out double pointX);
                                if (flag) //有交点才转换坐标
                                {
                                    point.CPointX = pointX;
                                    point.CPointY = pointY;
                                    point.CPointFinish = true;
                                    AffinePoint(point);
                                    //显示交点
                                    HOperatorSet.GenCrossContourXld(out HObject cross, point.CPointY, point.CPointX, 50, 90);
                                    RegionShowInfos.Add(new RegionShowInfo() { m_hRegion = cross, m_stuGetColor = HalconColor.orange });
                                    MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = "角点坐标：(" + point.CPointX.ToString("0.00") + " , " + point.CPointY.ToString("0.00") + ")", m_stuGetColor = HalconColor.green });
                                }
                            }
                        }
                        camEAPD.coorEaPoints.Add(point);//添加交点
                    }

                    //camEAPD.CamFinish = true;
                    HOperatorSet.CountSeconds(out camEAPD.Image_FindLineTime);
                    HOperatorSet.CountSeconds(out camEAPD.Image_FindDefectTime);
                    HOperatorSet.CountSeconds(out camEAPD.Image_ArithmeticProcessTime);

                    //MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = "处理时长：" + HtEnd.D.ToString("0.00") + "ms", m_stuGetColor = HalconColor.green });
                    //单相机，单次检测完成，返回本次结果。后续计算逻辑统一计算。
                    camEAPD.m_lineShowInfos = LineShowInfos;
                    camEAPD.showHalconWindow.m_nProductID = InfoImgInfo.image_ID;
                    camEAPD.showHalconWindow.m_hImage = InfoImgInfo.ho_image;
                    camEAPD.showHalconWindow.m_nImageFrameID = InfoImgInfo.image_ID;
                    camEAPD.showHalconWindow.m_lineShowInfos = LineShowInfos;
                    camEAPD.showHalconWindow.m_regionShowInfos = RegionShowInfos;
                    camEAPD.showHalconWindow.m_messageShowInfos = MesssgeShowInfos;
                    pretargetParam.ProductCallBack(camEAPD);
                }

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }

        /// <summary>
        /// 判断设备是否停止
        /// </summary>
        /// <returns></returns>
        public static bool DevIsStop()
        {
            float value = 0;
            if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面A极片纠偏工位 ||
                Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面B极片纠偏工位 ||
                Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面极片复检工位)
            {
                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_StartStopSignal_AOVHResult, ReadLogSaveType.ReadDifferentState, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                value = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_StartStopSignal_AOVHResult.Value;
            }
            else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面A极片纠偏工位 ||
                     Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面B极片纠偏工位 ||
                     Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面极片复检工位)
            {
                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_StartStopSignal_BOVHResult, ReadLogSaveType.ReadDifferentState, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                value = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_StartStopSignal_BOVHResult.Value;
            }

            if (value == 2)
            {
                return true;
            }
            return false;
        }
    }
}

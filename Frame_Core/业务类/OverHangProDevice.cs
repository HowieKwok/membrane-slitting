using Basler.Pylon;
using HalconDotNet;
using LogTool;
using NPOI.OpenXmlFormats.Vml;
using NPOI.OpenXmlFormats.Wordprocessing;
using NPOI.POIFS.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Frame_Core
{
    public class OverHangProDevice : BaseProDevice
    {
        private OverHangCraft m_craft { get; set; } = new OverHangCraft();

        public OverHangProDevice()
        {
            m_nCamCount = 8;
            m_nProcessCount = 8;


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
                }
                //m_DicCamOperaParam.Clear();

                //for (int i = 0; i < m_nCamCount; i++)
                //{
                //    m_DicCamOperaParam[i] = new AreaCamParam(((CameraEnum)i).ToString()); ;
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

                pluginName = "查找Mark工具";
                Plugin = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                Plugin.moduleInfo.ModuleName = ("查找Mark工具");
                Plugin.moduleInfo.PluginName = pluginName;
                ParamsFindMark ParamsMark = Plugin.PluginParams as ParamsFindMark; //里氏转换
                process.AddModuleObj(Plugin);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 单相机单次拍照流程
        /// </summary>
        /// <param name="process"></param>
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
                //相机对应的流程参数
                AreaCamParam OVHProParam = Project.Instance.Is_simulation ? m_DicCamOperaParam_Sim[CamID] as AreaCamParam : m_DicCamOperaParam[CamID] as AreaCamParam;

                var imageQueue = OVHProParam.alreadyDealGrabImage;
                CamEACornerProduct camEAPD;
                OVHProParam.semaphoreSlimCam.Wait();

                string str = "流程--" + CamID.ToString() + "信号量--" + OVHProParam.semaphoreSlimCam.CurrentCount.ToString() + "图像数--" + imageQueue.Count.ToString();
                //  Console.WriteLine(str);
                // 检查主相机的图像队列是否有图像
                if (imageQueue.Count > 0)
                {
                    if (!(Project.Instance.cameraManager[(CameraEnum)CamID].m_ImageSaveParamer.IsDebug == 1))
                    {
                        if (PretargetingProDevice.DevIsStop())
                        {
                            Log4netRecord.RunLog.Info("设备停止,释放图像资源");
                            imageQueue.Dequeue()?.Dispose();
                            return;
                        }
                    }


                    //图像出队
                    InfoImgInfo = imageQueue.Dequeue();
                    //OVHProParam.nEANum++;

                    //根据流程参数的 开始信号 生成单帧图像检测的PD。
                    OVHProParam.nEAIndex = 1;
                    camEAPD = new CamEACornerProduct()
                    {
                        CamFrame = InfoImgInfo.image_ID,
                        intCamEnum = CamID,                      
                        PackgeNum = InfoImgInfo.otherInfo.PackgeNum,
                        EAIndex = InfoImgInfo.otherInfo.CurrentLayer,
                        UpTabneckValue = InfoImgInfo.otherInfo.UpTabneckValue,
                        DownTabneckValue = InfoImgInfo.otherInfo.DownTabneckValue,
                        PoleWidth1 = InfoImgInfo.otherInfo.PoleWidth1,
                        PoleWidth2 = InfoImgInfo.otherInfo.PoleWidth2,
                        PoleHeight1 = InfoImgInfo.otherInfo.PoleHeight1,
                        PoleHeight2 = InfoImgInfo.otherInfo.PoleHeight2,
                        PoleEarW= InfoImgInfo.otherInfo.PoleEarW,
                        PoleEarH = InfoImgInfo.otherInfo.PoleEarH,
                        PoleEarD = InfoImgInfo.otherInfo.PoleEarD,
                        AffiliationStation = InfoImgInfo.otherInfo.AffiliationStation,
                    };

                    //计时
                    HOperatorSet.CountSeconds(out camEAPD.Image_StartTime);

                    //局部变量
                    List<MessageShowInfo> MesssgeShowInfos = new List<MessageShowInfo>();
                    List<RegionShowInfo> RegionShowInfos = new List<RegionShowInfo>();
                    List<LineInfo> LineShowInfos = new List<LineInfo>();



                    MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = str, m_stuGetColor = HalconColor.green });
                    MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = $"帧号：{InfoImgInfo.image_ID}", m_stuGetColor = HalconColor.green });
                    MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = $"电芯：{InfoImgInfo.otherInfo.PackgeNum}", m_stuGetColor = HalconColor.green });
                    MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = $"当前层数：{InfoImgInfo.otherInfo.CurrentLayer}", m_stuGetColor = HalconColor.green });

                    //mark圆
                    //if (Project.Instance.cameraManager[(CameraEnum)CamID].param_CalibrationPlate.bIsEnableMark)
                    //{
                    //    Param_CalibrationPlate param_CalibrationPlate = Project.Instance.cameraManager[(CameraEnum)CamID].param_CalibrationPlate;
                    //    bool flag = ImageAlgorithm.FindCirclePretreatmentTemp(InfoImgInfo.ho_image, param_CalibrationPlate, out HObject ho_Contours, out HObject rec, out HTuple row, out HTuple col);
                    //    if (flag)
                    //    {
                    //        double fRow = row.D;
                    //        double fCol = col.D;
                    //        RegionShowInfos.Add(new RegionShowInfo() { m_hRegion = ho_Contours, m_stuGetColor = HalconColor.green });
                    //        RegionShowInfos.Add(new RegionShowInfo() { m_hRegion = rec, m_stuGetColor = HalconColor.blue });
                    //        //ho_Contours?.Dispose();
                    //        MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = $"M({fCol.ToString("0.00")},{fRow.ToString("0.00")})", m_stuGetColor = HalconColor.green, m_bIsWindow = false, m_fRow = fRow, m_fCol = fCol + 100 });

                    //    }
                    //    else
                    //    {
                    //        rec?.Dispose();
                    //        ho_Contours?.Dispose();
                    //        double fRow = param_CalibrationPlate.BasePointY;
                    //        double fCol = param_CalibrationPlate.BasePointX;

                    //        MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = "未定位到Mark", m_stuGetColor = HalconColor.blue, m_bIsWindow = false, m_fRow = fRow, m_fCol = fCol + 100 });

                    //    }

                    //}

                    //插件找Mark孔
                    if (true)
                    {                      
                        Param_MarkSet param_MarkSet = Project.Instance.cameraManager[(CameraEnum)CamID].param_MarkSet;

                        //输出X、Y、R基准点
                        double currentBasePointX = param_MarkSet.BasePointX;
                        double currentBasePointY = param_MarkSet.BasePointY;
                        double currentBasePointR = param_MarkSet.BasePointR;

                        param_MarkSet.m_DetectionBaseCenterX = currentBasePointX;
                        param_MarkSet.m_DetectionBaseCenterY = currentBasePointY;
                        param_MarkSet.m_DetectionBaseCenterR = currentBasePointR;


                        IPlugin PluginMark = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == "查找Mark工具");
                        ParamsFindMark ParamsMark = PluginMark.PluginParams as ParamsFindMark; //里氏转换
                        ParamsMark.m_fPixelUnitX = Calipix_x;
                        ParamsMark.m_fPixelUnitY = Calipix_y;

                        if (ParamsMark.MarkType > 0)
                        {
                            ParamsMark.ho_Image = InfoImgInfo.ho_image;
                            ParamsMark.m_fBaseCenterX = currentBasePointX;
                            ParamsMark.m_fBaseCenterY = currentBasePointY;
                            bool isSuccess = PluginMark.ExecuteObj();

                            //识别失败报警停机判断
                            if (param_MarkSet.UnidentificationMarkBJTJ)
                            {
                                if (!isSuccess)
                                {
                                    param_MarkSet.UnidentificationMarkCount++;
                                    if (param_MarkSet.UnidentificationMarkCount >= param_MarkSet.UnidentificationMarkNum)
                                    {                                     
                                        TJBJInfo.AlarmWriteState(AlarmName.OVH连续N次未识别Mark, StopAlarmStateEnum.停机, $"相机：{((CameraEnum)CamID).ToString()}，识别失败次数：{param_MarkSet.UnidentificationMarkCount}");
                                        param_MarkSet.UnidentificationMarkCount = 0;
                                    }
                                }
                                else
                                {
                                    param_MarkSet.UnidentificationMarkCount = 0;
                                }                              
                            }

                            if (isSuccess)
                            {
                                //Mark孔位置波动监控
                                if (param_MarkSet.MarkPosRangeBJTJ)
                                {
                                    if (Math.Abs(ParamsMark.m_fGoalX - param_MarkSet.BasePointX) > param_MarkSet.MarkPosRange)
                                    {
                                        TJBJInfo.AlarmWriteState(AlarmName.OVH_Mark中心位置波动X超范围, StopAlarmStateEnum.停机, $"相机：{((CameraEnum)CamID).ToString()}，基准位置：{param_MarkSet.BasePointX}，实际位置：{ParamsMark.m_fGoalX}");
                                    }

                                    if (Math.Abs(ParamsMark.m_fGoalY - param_MarkSet.BasePointY) > param_MarkSet.MarkPosRange)
                                    {
                                        TJBJInfo.AlarmWriteState(AlarmName.OVH_Mark中心位置波动Y超范围, StopAlarmStateEnum.停机, $"相机：{((CameraEnum)CamID).ToString()}，基准位置：{param_MarkSet.BasePointY}，实际位置：{ParamsMark.m_fGoalY}");
                                    }
                                }
                                  


                                ParamsMark.m_fGoalX = Math.Round(ParamsMark.m_fGoalX, 3);
                                ParamsMark.m_fGoalY = Math.Round(ParamsMark.m_fGoalY, 3);
                                ParamsMark.m_fGoalR = Math.Round(ParamsMark.m_fGoalR, 3);
                               
                                param_MarkSet.m_DetectionBaseCenterX = ParamsMark.m_fGoalX;
                                param_MarkSet.m_DetectionBaseCenterY = ParamsMark.m_fGoalY;
                                param_MarkSet.m_DetectionBaseCenterR = ParamsMark.m_fGoalR;

                                camEAPD.MarkSizeW = ParamsMark.m_fMarkWidth;
                                camEAPD.MarkSizeH = ParamsMark.m_fMarkHeight;
                               

                                //Mark孔找矩形边成功
                                if (ParamsMark.bIsFindLineSucess)
                                {
                                    HOperatorSet.GenCrossContourXld(out HObject cross1, ParamsMark.leftTop_Row, ParamsMark.leftTop_Col, 100, 0);
                                    HOperatorSet.GenCrossContourXld(out HObject cross2, ParamsMark.rightTop_Row, ParamsMark.rightTop_Col, 100, 0);
                                    HOperatorSet.GenCrossContourXld(out HObject cross3, ParamsMark.leftBottom_Row, ParamsMark.leftBottom_Col, 100, 0);
                                    HOperatorSet.GenCrossContourXld(out HObject cross4, ParamsMark.rightBottom_Row, ParamsMark.rightBottom_Col, 100, 0);
                                    RegionShowInfos.Add(new RegionShowInfo() { m_hRegion = cross1, m_stuGetColor = HalconColor.green });
                                    RegionShowInfos.Add(new RegionShowInfo() { m_hRegion = cross2, m_stuGetColor = HalconColor.green });
                                    RegionShowInfos.Add(new RegionShowInfo() { m_hRegion = cross3, m_stuGetColor = HalconColor.green });
                                    RegionShowInfos.Add(new RegionShowInfo() { m_hRegion = cross4, m_stuGetColor = HalconColor.green });
                                }

                                if (ParamsMark.m_ho_OutCropRegion != null)
                                    RegionShowInfos.Add(new RegionShowInfo() { m_hRegion = ParamsMark.m_ho_OutCropRegion, m_stuGetColor = HalconColor.green });
                                if (ParamsMark.m_ho_OutRegion != null)
                                    RegionShowInfos.Add(new RegionShowInfo() { m_hRegion = ParamsMark.m_ho_OutRegion, m_stuGetColor = HalconColor.blue });
                                if (ParamsMark.m_ho_OutRecRegion != null)
                                    RegionShowInfos.Add(new RegionShowInfo() { m_hRegion = ParamsMark.m_ho_OutRecRegion, m_stuGetColor = HalconColor.blue });
                                // ParamsMark.m_ho_CropImg?.Dispose();
                                MesssgeShowInfos.Add(new MessageShowInfo
                                {
                                    m_strMessage = $"M({ParamsMark.m_fGoalX},{ParamsMark.m_fGoalY},{ParamsMark.m_fGoalR})",
                                    m_stuGetColor = HalconColor.blue,
                                    m_bIsWindow = false,
                                    m_fRow = param_MarkSet.m_DetectionBaseCenterY,
                                    m_fCol = param_MarkSet.m_DetectionBaseCenterX + 100
                                });
                                Log4netRecord.RunLog.Info($"找到Mark孔，坐标：X:{param_MarkSet.m_DetectionBaseCenterX},Y:{param_MarkSet.m_DetectionBaseCenterY},R:{param_MarkSet.m_DetectionBaseCenterR}");
                            }
                            else
                            {
                               
                                //ParamsMark.m_ho_CropImg?.Dispose();
                                if (ParamsMark.m_ho_OutCropRegion != null)
                                    RegionShowInfos.Add(new RegionShowInfo() { m_hRegion = ParamsMark.m_ho_OutCropRegion, m_stuGetColor = HalconColor.green });
                                if (ParamsMark.m_ho_OutRegion != null)
                                    RegionShowInfos.Add(new RegionShowInfo() { m_hRegion = ParamsMark.m_ho_OutRegion, m_stuGetColor = HalconColor.blue });
                                if (ParamsMark.m_ho_OutRecRegion != null)
                                    RegionShowInfos.Add(new RegionShowInfo() { m_hRegion = ParamsMark.m_ho_OutRecRegion, m_stuGetColor = HalconColor.blue });
                                MesssgeShowInfos.Add(new MessageShowInfo
                                {
                                    m_strMessage = "未定位到Mark",
                                    m_stuGetColor = HalconColor.red,
                                    m_bIsWindow = false,
                                    m_fRow = param_MarkSet.m_DetectionBaseCenterY,
                                    m_fCol = param_MarkSet.m_DetectionBaseCenterX + 100
                                });
                                Log4netRecord.RunLog.Info($"未定位Mark孔，坐标：X:{param_MarkSet.m_DetectionBaseCenterX},Y:{param_MarkSet.m_DetectionBaseCenterY},R:{param_MarkSet.m_DetectionBaseCenterR}");
                            }

                        }


                    }


                    //抓边，并把边结果添加到显示队列，
                    for (int i = 0; i < m_craft.ListLines[CamID].Count(); i++)
                    {
                        //槽位检测：GrooveIndex，第几排槽
                        IPlugin PluginMline = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == m_craft.ListLines[CamID][i].StrLineName);
                        ParamsMeasureLine MlineParams = PluginMline.PluginParams as ParamsMeasureLine; //里氏转换
                        MlineParams.ho_Image = InfoImgInfo.ho_image;

                        //最后一层单独使用抓边方向
                        if (camEAPD.EAIndex == m_craft.StakeCathodeCount)
                        {
                            if (!m_craft.ListLines[CamID][i].StrLineName.Contains("膜"))
                            {
                                MlineParams.hv_MeasureTransition = m_craft.LastFloorMeasureTransition;
                            }
                        }
                        else
                        {
                            MlineParams.hv_MeasureTransition = MlineParams.hv_HistoryMeasureTransition;
                        }
                        if (CamID == 3 || CamID == 7)
                        {

                        }

                        LineInfo line = new LineInfo();
                        bool isSuccess = PluginMline.ExecuteObj();
                        DetectEdgeValueAssign(MlineParams, isSuccess, line, m_craft.ListLines[CamID][i].StrLineName, MesssgeShowInfos, LineShowInfos, RegionShowInfos);

                        //最后一层使用完抓边方向复位
                        MlineParams.hv_MeasureTransition = MlineParams.hv_HistoryMeasureTransition;
                    }

                    ////抓边完成后，计算交点。
                    //for (int i = 0; i < m_craft.ListPointEdits[CamID].Count; i++)
                    //{
                    //    CoorPoint point = new CoorPoint() { strCrossName = m_craft.ListPointEdits[CamID][i].Name, CCamera = (CameraEnum)CamID, AffineCamera = m_craft.BaseCamera };

                    //    LineInfo line1 = LineShowInfos.Find(c => c.StrLineName == m_craft.ListPointEdits[CamID][i].Line1);
                    //    LineInfo line2 = LineShowInfos.Find(c => c.StrLineName == m_craft.ListPointEdits[CamID][i].Line2);
                    //    if (line1 != null && line2 != null)
                    //    {
                    //        if (line1.IsLineSuccessfull && line2.IsLineSuccessfull)
                    //        {
                    //            bool flag = IntersectionLines(line1, line2, out double pointY, out double pointX);
                    //            if (flag) //有交点才转换坐标
                    //            {
                    //                point.CPointX = pointX;
                    //                point.CPointY = pointY;
                    //                AffinePoint(point);
                    //                //显示交点
                    //                HOperatorSet.GenCrossContourXld(out HObject cross, point.CPointY, point.CPointX, 50, 90);
                    //                RegionShowInfos.Add(new RegionShowInfo() { m_hRegion = cross, m_stuGetColor = HalconColor.orange });
                    //                MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = "角点坐标：(" + point.CPointX.ToString("0.00") + " , " + point.CPointY.ToString("0.00") + ")", m_stuGetColor = HalconColor.green });
                    //            }
                    //        }
                    //    }
                    //    camEAPD.coorEaPoints.Add(point);//添加交点
                    //}

                    //camEAPD.CamFinish = true;
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

                    OVHProParam.ProductCallBack(camEAPD);

                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Fatal(ex);

            }
        }


    }
}

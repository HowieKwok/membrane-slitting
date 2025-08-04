using HalconDotNet;
using LogTool;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Frame_Core
{

    public class Slice1To2ProDevice : BaseProDevice
    {
        private Slice1to2Craft m_craft { get; set; } = new Slice1to2Craft();

        public ProductBase IProductProcess;
        public Slice1To2ProDevice()
        {
            m_nCamCount = 2;
            m_nProcessCount = 2;

            for (int i = 0; i < m_nCamCount; i++)
            {
                m_DicCamOperaParam[i] = new Slice1To2Paramter();
                m_DicCamOperaParam_Sim[i] = new Slice1To2Paramter();
            }
            Project.Instance.BindCamImageSource(m_DicCamOperaParam);
            m_craft = (Slice1to2Craft)Project.Instance.craftBase;
            BuildProcessServiceList();
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

                foreach (var item in Project.Instance.m_ProcessList)
                {
                    item.process?.Dispose();
                }

                m_DicCamOperaParam.Clear();
                m_DicCamOperaParam.Add((int)CameraEnum.Cam1, new Slice1To2Paramter());
                m_DicCamOperaParam.Add((int)CameraEnum.Cam2, new Slice1To2Paramter());
                Project.Instance.BindCamImageSource(m_DicCamOperaParam);
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
                    //m_DicCamOperaParam_Sim[item.Key] = new DieCutBurrDetectParameter();
                }


                foreach (var item in Project.Instance.m_ProcessList)
                {
                    item.process?.Dispose();
                }

                m_DicCamOperaParam_Sim.Clear();

                m_DicCamOperaParam_Sim.Add((int)CameraEnum.Cam1, new Slice1To2Paramter());
                m_DicCamOperaParam_Sim.Add((int)CameraEnum.Cam2, new Slice1To2Paramter());

                return true;
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
                Log4netRecord.ErrorLog.Debug("初始化仿真数据报错————\n" + ee.ToString());
                return false;
            }

        }
     
        /// <summary>
        /// 只在开软件执行一次。有更改相机个数的换型需要重启软件。
        /// </summary>
        public override void BuildProcessServiceList()
        {
            //Project.Instance.DeleteALLProcess(); //
            int ID = Project.Instance.CreateProcess();
            ProcessServices TempProcessBase = Project.Instance.GetProcessServiceById(ID);
            TempProcessBase.process.processInfo.ProcessName = CameraEnum.Cam1.ToString() + "分切检测";
            TempProcessBase.m_eventINIProcess += IniProcess1;
            TempProcessBase.m_eventRUNProcess += Processing1;
            TempProcessBase.InitProcess();
            TempProcessBase.OpeningThread();

            ID = Project.Instance.CreateProcess();
            TempProcessBase = Project.Instance.GetProcessServiceById(ID);
            TempProcessBase.process.processInfo.ProcessName = CameraEnum.Cam2.ToString() + "分切检测";
            TempProcessBase.m_eventINIProcess += IniProcess2;
            TempProcessBase.m_eventRUNProcess += Processing2;
            TempProcessBase.InitProcess();
            TempProcessBase.OpeningThread();

           


        }

        public override void INIPro()
        {
            m_craft = (Slice1to2Craft)Project.Instance.craftBase;
            //  m_stdCalibrationInfo = Project.Instance.calibrationInfo;//加载系统参数之后加载，修改标定参数后加载         
            //加载仿真图
            foreach (var item in Project.Instance.m_ProcessList)
            {
                item.process.ProductInfo=new SingleSliceProduct();
            }
        }

        public void IniProcess1(Process process)
        {
            if (m_craft.electrode == ElectrodeEnum.阴极)
            {
                process.m_ModuleObjList.Clear();
                string pluginName = "";
                IPlugin PluginFitLine;
                foreach (var item in m_craft.CamerCraft[CameraEnum.Cam1].ListLines)
                {
                    if(!item.IsStandLine)
                    {
                        pluginName = "直线抓边工具";
                        PluginFitLine = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                        PluginFitLine.moduleInfo.ModuleName = (item.StrLineName);
                        PluginFitLine.moduleInfo.PluginName = pluginName;
                        ParamsMeasureLine ParamsMeasureLine = PluginFitLine.PluginParams as ParamsMeasureLine; //里氏转换
                        ParamsMeasureLine.hv_RoiAngle = 90;
                        process.AddModuleObj(PluginFitLine);
                    }
                }

          


            }
            else
            {
                process.m_ModuleObjList.Clear();
                string pluginName = "";
                IPlugin PluginFitLine;
                foreach (var item in m_craft.CamerCraft[CameraEnum.Cam1].ListLines)
                {
                    if (!item.IsStandLine)
                    {
                        pluginName = "直线抓边工具";
                        PluginFitLine = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                        PluginFitLine.moduleInfo.ModuleName = (item.StrLineName);
                        PluginFitLine.moduleInfo.PluginName = pluginName;
                        ParamsMeasureLine ParamsMeasureLine = PluginFitLine.PluginParams as ParamsMeasureLine; //里氏转换
                        ParamsMeasureLine.hv_RoiAngle = 90;
                        process.AddModuleObj(PluginFitLine);
                    }
                }

           


            }

        }

        public void IniProcess2(Process process)
        {
            if (m_craft.electrode == ElectrodeEnum.阴极)
            {
                process.m_ModuleObjList.Clear();
                string pluginName = "";
                IPlugin PluginFitLine;
                foreach (var item in m_craft.CamerCraft[CameraEnum.Cam2].ListLines)
                {
                    if (!item.IsStandLine)
                    {
                        pluginName = "直线抓边工具";
                        PluginFitLine = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                        PluginFitLine.moduleInfo.ModuleName = (item.StrLineName);
                        PluginFitLine.moduleInfo.PluginName = pluginName;
                        ParamsMeasureLine ParamsMeasureLine = PluginFitLine.PluginParams as ParamsMeasureLine; //里氏转换
                        ParamsMeasureLine.hv_RoiAngle = 90;
                        process.AddModuleObj(PluginFitLine);
                    }
                }


            }
            else
            {
                process.m_ModuleObjList.Clear();
                string pluginName = "";
                IPlugin PluginFitLine;
                foreach (var item in m_craft.CamerCraft[CameraEnum.Cam2].ListLines)
                {
                    if (!item.IsStandLine)
                    {
                        pluginName = "直线抓边工具";
                        PluginFitLine = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
                        PluginFitLine.moduleInfo.ModuleName = (item.StrLineName);
                        PluginFitLine.moduleInfo.PluginName = pluginName;
                        ParamsMeasureLine ParamsMeasureLine = PluginFitLine.PluginParams as ParamsMeasureLine; //里氏转换
                        ParamsMeasureLine.hv_RoiAngle = 90;
                        process.AddModuleObj(PluginFitLine);
                    }
                }

            }

        }


        public void Processing1(Process process)
        {
            try
            {
                SetThreadAffinityMask(GetCurrentThread(), new UIntPtr(SetCpuID(0)));
                //A相机图像参数
                ImageInfo InfoImgInfo;

                //标定系数
                double Calipix_y = Project.Instance.cameraManager[CameraEnum.Cam1].m_CamParam.CalibrationValue_Y;
                double Calipix_x = Project.Instance.cameraManager[CameraEnum.Cam1].m_CamParam.CalibrationValue_X;
              
                bool ExecuteSuccess = false;

                // 选择参数和队列基于是否模拟
                Slice1To2Paramter ISlice1To2Paramter = Project.Instance.Is_simulation ? m_DicCamOperaParam_Sim[(int)CameraEnum.Cam1] as Slice1To2Paramter : m_DicCamOperaParam[(int)CameraEnum.Cam1] as Slice1To2Paramter;
                var imageQueue = ISlice1To2Paramter.QueueGrabImage;

                if (imageQueue.Count > 0) // 检查A相机的图像队列是否有图像
                {
                    // 从队列中取出一幅图像
                    imageQueue.Peek().image_ID = ISlice1To2Paramter.m_nCurProImgId;
                    InfoImgInfo = imageQueue.Dequeue();//出队


                }
                else
                {
                    return;
                }
                List<MessageShowInfo> MesssgeShowInfos = new List<MessageShowInfo>();
                List<RegionShowInfo> RegionShowInfos = new List<RegionShowInfo>();
                List<LineInfo> LineShowInfos = new List<LineInfo>();
                HTuple HtStart = 0;
                HTuple HtEnd = 0;
                MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = "图像帧号：" + ISlice1To2Paramter.m_nCurProImgId, m_stuGetColor = HalconColor.green , m_nFontSize = 20 });
                HOperatorSet.CountSeconds(out HtStart);
                Dictionary<string, LineInfo> lineInfosMap = new Dictionary<string, LineInfo>();
                Dictionary<string, double> sizeMap = new Dictionary<string, double>();
                Queue<SizeInfo> _NgSizeInfoQueue = new Queue<SizeInfo>();
                bool OutterIsOK = true;
                foreach (var item in m_craft.CamerCraft[CameraEnum.Cam1].ListLines)
                {
                    lineInfosMap.Add(item.StrLineName, new LineInfo());
                }
                ParamsMeasureLine MeasureLineParamsTile;
                foreach (var item in m_craft.CamerCraft[CameraEnum.Cam1].ListLines)
                {
                    if(!item.IsStandLine)
                    {
                        IPlugin PluginLine = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == item.StrLineName);
                        MeasureLineParamsTile = PluginLine.PluginParams as ParamsMeasureLine;
                        MeasureLineParamsTile.ho_Image = InfoImgInfo.ho_image;
                        DetectEdgeValueAssign(PluginLine, MeasureLineParamsTile, lineInfosMap, item.StrLineName, MesssgeShowInfos, LineShowInfos);

                    }
                   

                }
                double size = 0;
                foreach (var item in m_craft.CamerCraft[CameraEnum.Cam1].ListsizeItemEditors)
                {
                    if (lineInfosMap.ContainsKey(item.Line1) && lineInfosMap.ContainsKey(item.Line2))
                    {
                        if (lineInfosMap[item.Line1].IsLineSuccessfull && lineInfosMap[item.Line2].IsLineSuccessfull)
                        {
                            if (item.EditorType == EditorTypeEnum.线距离X.ToString())
                            {
                                size = Math.Abs((lineInfosMap[item.Line1].ColBegin + lineInfosMap[item.Line1].ColEnd) / 2 - (lineInfosMap[item.Line2].ColBegin + lineInfosMap[item.Line2].ColEnd) / 2);
                                size = size * Calipix_x;
                                sizeMap.Add(item.Name, size);
                            }
                            else if (item.EditorType == EditorTypeEnum.线距离Y.ToString())
                            {
                                size = Math.Abs((lineInfosMap[item.Line1].RowBegin + lineInfosMap[item.Line1].RowEnd) / 2 - (lineInfosMap[item.Line2].RowBegin + lineInfosMap[item.Line2].RowEnd) / 2);
                                size = size * Calipix_x;
                                sizeMap.Add(item.Name, size);
                            }
                        }
                        else
                        {
                            size = item.DefaultValue;
                            sizeMap.Add(item.Name, size);
                        }

                    }
                    else
                    {
                        size = item.DefaultValue;
                        sizeMap.Add(item.Name, size);
                    }
                }

                for (int i = 0; i < Enum.GetValues(typeof(SizeEnum)).Length; i++)
                {
                    bool InnerIsOK = true;
                    SizeInfo _NgSizeInfo = new SizeInfo();

                    if (sizeMap.ContainsKey(((SizeEnum)i).ToString()))
                    {
                        if (((Slice1to2Craft)m_craft).CamerCraft[CameraEnum.Cam1].SizeInfoList[i].Enable)
                        {
                            double adjustScaleValue = ((Slice1to2Craft)m_craft).CamerCraft[CameraEnum.Cam1].SizeInfoList[i].StD_RealFixRito;
                            size = sizeMap[((SizeEnum)i).ToString()]* adjustScaleValue;
                            _NgSizeInfo.sizeEnum = (SizeEnum)i;
                            _NgSizeInfo.DataResult = double.Parse(size.ToString("0.00"));
                            _NgSizeInfo.StandardValue = ((Slice1to2Craft)m_craft).CamerCraft[CameraEnum.Cam1].SizeInfoList[i].StDValue;

                            if (((Slice1to2Craft)m_craft).CamerCraft[CameraEnum.Cam1].SizeInfoList[i].DB)
                            {

                                if (size < (((Slice1to2Craft)m_craft).CamerCraft[CameraEnum.Cam1].SizeInfoList[i].StDValue + ((Slice1to2Craft)m_craft).CamerCraft[CameraEnum.Cam1].SizeInfoList[i].StD_Down) ||
                                 size > (((Slice1to2Craft)m_craft).CamerCraft[CameraEnum.Cam1].SizeInfoList[i].StDValue + ((Slice1to2Craft)m_craft).CamerCraft[CameraEnum.Cam1].SizeInfoList[i].StD_Up))
                                {

                                   //  MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = ((SizeEnum)i).ToString() + ":" + size.ToString("0.00"), m_stuGetColor = HalconColor.red, m_nFontSize = 20 });
                                    _NgSizeInfo.IsNG = true;
                                    _NgSizeInfo.DBInfo.UpTolerance = (((Slice1to2Craft)m_craft).CamerCraft[CameraEnum.Cam1].SizeInfoList[i].StD_Up);
                                    _NgSizeInfo.DBInfo.DownTolerance = (((Slice1to2Craft)m_craft).CamerCraft[CameraEnum.Cam1].SizeInfoList[i].StD_Down);
                                    _NgSizeInfo.DBInfo.IsAlarm = true;
                                    InnerIsOK = false;
                                    OutterIsOK = false;


                                }

                                if (!InnerIsOK)
                                {                                   
                                    MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = ((SizeEnum)i).ToString() + ":" + size.ToString("0.00"), m_stuGetColor = HalconColor.red, m_nFontSize = 20 });
                                }


                            }

                            if (((Slice1to2Craft)m_craft).CamerCraft[CameraEnum.Cam1].SizeInfoList[i].BJ)
                            {
                                if (size < (((Slice1to2Craft)m_craft).CamerCraft[CameraEnum.Cam1].SizeInfoList[i].StDValue + ((Slice1to2Craft)m_craft).CamerCraft[CameraEnum.Cam1].SizeInfoList[i].StD_BJ_Down) ||
                                 size > (((Slice1to2Craft)m_craft).CamerCraft[CameraEnum.Cam1].SizeInfoList[i].StDValue + ((Slice1to2Craft)m_craft).CamerCraft[CameraEnum.Cam1].SizeInfoList[i].StD_BJ_Up))
                                {
                                    if (InnerIsOK)
                                    {
                                        MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = ((SizeEnum)i).ToString() + ":" + size.ToString("0.00"), m_stuGetColor = HalconColor.orange, m_nFontSize = 20 });
                                    }
                                    _NgSizeInfo.IsNG = true;
                                    _NgSizeInfo.BJInfo.UpTolerance = (((Slice1to2Craft)m_craft).CamerCraft[CameraEnum.Cam1].SizeInfoList[i].StD_BJ_Up);
                                    _NgSizeInfo.BJInfo.DownTolerance = (((Slice1to2Craft)m_craft).CamerCraft[CameraEnum.Cam1].SizeInfoList[i].StD_BJ_Down);
                                    _NgSizeInfo.BJInfo.IsAlarm = true;
                                    InnerIsOK = false;
                                    //OutterIsOK = false;


                                }


                            }

                            if (((Slice1to2Craft)m_craft).CamerCraft[CameraEnum.Cam1].SizeInfoList[i].TJ)
                            {
                                if (size < (((Slice1to2Craft)m_craft).CamerCraft[CameraEnum.Cam1].SizeInfoList[i].StDValue + ((Slice1to2Craft)m_craft).CamerCraft[CameraEnum.Cam1].SizeInfoList[i].StD_BJ_Down) ||
                                 size > (((Slice1to2Craft)m_craft).CamerCraft[CameraEnum.Cam1].SizeInfoList[i].StDValue + ((Slice1to2Craft)m_craft).CamerCraft[CameraEnum.Cam1].SizeInfoList[i].StD_BJ_Up))
                                {
                                    if (InnerIsOK)
                                    {
                                        MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = ((SizeEnum)i).ToString() + ":" + size.ToString("0.00"), m_stuGetColor = HalconColor.pink, m_nFontSize = 20 });

                                    }
                                    _NgSizeInfo.IsNG = true;
                                    _NgSizeInfo.TJInfo.UpTolerance = (((Slice1to2Craft)m_craft).CamerCraft[CameraEnum.Cam1].SizeInfoList[i].StD_BJ_Up);
                                    _NgSizeInfo.TJInfo.DownTolerance = (((Slice1to2Craft)m_craft).CamerCraft[CameraEnum.Cam1].SizeInfoList[i].StD_BJ_Down);
                                    _NgSizeInfo.TJInfo.IsAlarm = true;
                                    InnerIsOK = false;
                                   // OutterIsOK = false;


                                }


                            }

                            if (InnerIsOK)
                            {
                                _NgSizeInfo.IsNG = false;
                                MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = ((SizeEnum)i).ToString() + ":" + _NgSizeInfo.DataResult.ToString("0.00"), m_stuGetColor = HalconColor.green, m_nFontSize = 20 });

                            }
                            _NgSizeInfoQueue.Enqueue(_NgSizeInfo);

                        }
                    }
                    else
                    {
                        if (((Slice1to2Craft)m_craft).CamerCraft[CameraEnum.Cam1].SizeInfoList[i].Enable)
                        {
                            _NgSizeInfo.sizeEnum = ((SizeEnum)i);
                            _NgSizeInfo.DataResult = double.Parse(0.ToString("0.00"));
                            _NgSizeInfo.StandardValue = ((Slice1to2Craft)m_craft).CamerCraft[CameraEnum.Cam1].SizeInfoList[i].StDValue;
                            _NgSizeInfo.IsNG = false;

                            if (InnerIsOK)
                            {
                                _NgSizeInfo.IsNG = false;
                                MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = ((SizeEnum)i).ToString() + ":" + _NgSizeInfo.DataResult.ToString("0.00"), m_stuGetColor = HalconColor.green, m_nFontSize = 20 });

                            }
                            _NgSizeInfoQueue.Enqueue(_NgSizeInfo);
                        }

                        // MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = ((SizeEnum)i).ToString() + ":" + 0, m_stuGetColor = HalconColor.green });
                    }


                }

             

                if (!OutterIsOK)
                {
                    MesssgeShowInfos.Insert(0, new MessageShowInfo { m_strMessage = "NG", m_stuGetColor = HalconColor.red ,m_nFontSize=60});
                }
                else
                {
                    MesssgeShowInfos.Insert(0, new MessageShowInfo { m_strMessage = "OK", m_stuGetColor = HalconColor.green, m_nFontSize = 60 });
                }

                HOperatorSet.CountSeconds(out HtEnd);
                HtEnd = (HtEnd - HtStart) * 1000;
                MesssgeShowInfos.Add(new MessageShowInfo() { m_strMessage = "检测时间:" + HtEnd.D.ToString("0") + "ms", m_stuGetColor = HalconColor.green , m_nFontSize = 20 });
                //数据上传

               
               ((SingleSliceProduct)process.ProductInfo).DataResults.Enqueue(new ImageDataResult()
                {
                    m_bIsNG = !OutterIsOK,
                    m_nImgIndex = InfoImgInfo.image_ID,
                    m_fExcuseTime = HtEnd,
                    m_qNgSizeInfos = _NgSizeInfoQueue,

                });
               

                (process).ShowHalconWindows.Enqueue(new ShowHalconWindow()
                {
                    m_hImage = InfoImgInfo.ho_image,
                    m_nImageFrameID = InfoImgInfo.image_ID,
                    m_nProductID = 0,
                    m_bIsNG = !OutterIsOK,
                    m_regionShowInfos = RegionShowInfos,
                    m_lineShowInfos = LineShowInfos,
                    m_messageShowInfos = MesssgeShowInfos,
                });

               
                ISlice1To2Paramter.m_nCurProImgId++;
            }
            catch (Exception ee)
            {
                System.Windows.Forms.MessageBox.Show(ee.ToString());
                Log4netRecord.ErrorLog.Debug(CameraEnum.Cam1.ToString() + "分切1流程报错————" + ee.ToString());
            }

            Thread.Sleep(3);

        }

        public void Processing2(Process process)
        {
            try
            {
                SetThreadAffinityMask(GetCurrentThread(), new UIntPtr(SetCpuID(1)));
                //A相机图像参数
                ImageInfo InfoImgInfo ;

                //标定系数
                double Calipix_y = Project.Instance.cameraManager[CameraEnum.Cam2].m_CamParam.CalibrationValue_Y;
                double Calipix_x = Project.Instance.cameraManager[CameraEnum.Cam2].m_CamParam.CalibrationValue_X;
               
                bool ExecuteSuccess = false;

                // 选择参数和队列基于是否模拟
                Slice1To2Paramter ISlice1To2Paramter = Project.Instance.Is_simulation ? m_DicCamOperaParam_Sim[(int)CameraEnum.Cam2] as Slice1To2Paramter : m_DicCamOperaParam[(int)CameraEnum.Cam2] as Slice1To2Paramter;
                var imageQueue = ISlice1To2Paramter.QueueGrabImage;

                if (imageQueue.Count > 0) // 检查A相机的图像队列是否有图像
                {
                    // 从队列中取出一幅图像
                    imageQueue.Peek().image_ID = ISlice1To2Paramter.m_nCurProImgId;
                    InfoImgInfo = imageQueue.Dequeue();//出队


                }
                else
                {
                    return;
                }
                List<MessageShowInfo> MesssgeShowInfos = new List<MessageShowInfo>();
                List<RegionShowInfo> RegionShowInfos = new List<RegionShowInfo>();
                List<LineInfo> LineShowInfos = new List<LineInfo>();
                HTuple HtStart = 0;
                HTuple HtEnd = 0;
                MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = "图像帧号：" + ISlice1To2Paramter.m_nCurProImgId, m_stuGetColor = HalconColor.green , m_nFontSize = 20 });
                HOperatorSet.CountSeconds(out HtStart);
                Dictionary<string, LineInfo> lineInfosMap = new Dictionary<string, LineInfo>();
                Dictionary<string, double> sizeMap = new Dictionary<string, double>();
                Queue<SizeInfo> _NgSizeInfoQueue = new Queue<SizeInfo>();
                bool OutterIsOK = true;
                foreach (var item in m_craft.CamerCraft[CameraEnum.Cam2].ListLines)
                {
                    lineInfosMap.Add(item.StrLineName, new LineInfo());
                }
                ParamsMeasureLine MeasureLineParamsTile;
                foreach (var item in m_craft.CamerCraft[CameraEnum.Cam2].ListLines)
                {
                    if (!item.IsStandLine)
                    {
                        IPlugin PluginLine = process.m_ModuleObjList.Find(c => c.moduleInfo.ModuleName == item.StrLineName);
                        MeasureLineParamsTile = PluginLine.PluginParams as ParamsMeasureLine;
                        MeasureLineParamsTile.ho_Image = InfoImgInfo.ho_image;
                        DetectEdgeValueAssign(PluginLine, MeasureLineParamsTile, lineInfosMap, item.StrLineName, MesssgeShowInfos, LineShowInfos);
                    }
                }
                double size = 0;
                foreach (var item in m_craft.CamerCraft[CameraEnum.Cam2].ListsizeItemEditors)
                {
                    if (lineInfosMap.ContainsKey(item.Line1) && lineInfosMap.ContainsKey(item.Line2))
                    {
                        if (lineInfosMap[item.Line1].IsLineSuccessfull && lineInfosMap[item.Line2].IsLineSuccessfull)
                        {
                            if (item.EditorType == EditorTypeEnum.线距离X.ToString())
                            {
                                size = Math.Abs((lineInfosMap[item.Line1].ColBegin + lineInfosMap[item.Line1].ColEnd) / 2 - (lineInfosMap[item.Line2].ColBegin + lineInfosMap[item.Line2].ColEnd) / 2);
                                size = size * Calipix_x;
                                sizeMap.Add(item.Name, size);
                            }
                            else if (item.EditorType == EditorTypeEnum.线距离Y.ToString())
                            {
                                size = Math.Abs((lineInfosMap[item.Line1].RowBegin + lineInfosMap[item.Line1].RowEnd) / 2 - (lineInfosMap[item.Line2].RowBegin + lineInfosMap[item.Line2].RowEnd) / 2);
                                size = size * Calipix_x;
                                sizeMap.Add(item.Name, size);
                            }

                        }
                        else
                        {
                            size = item.DefaultValue;
                            sizeMap.Add(item.Name, size);
                        }

                    }
                    else
                    {
                        size = item.DefaultValue;
                        sizeMap.Add(item.Name, size);
                    }
                }


                for (int i = 0; i < Enum.GetValues(typeof(SizeEnum)).Length; i++)
                {
                    bool InnerIsOK = true;
                    SizeInfo _NgSizeInfo = new SizeInfo();

                    if (sizeMap.ContainsKey(((SizeEnum)i).ToString()))
                    {
                        if (((Slice1to2Craft)m_craft).CamerCraft[CameraEnum.Cam2].SizeInfoList[i].Enable)
                        {
                            double adjustScaleValue = ((Slice1to2Craft)m_craft).CamerCraft[CameraEnum.Cam2].SizeInfoList[i].StD_RealFixRito;
                            size = sizeMap[((SizeEnum)i).ToString()]* adjustScaleValue;
                            _NgSizeInfo.sizeEnum = (SizeEnum)i;
                            _NgSizeInfo.DataResult = double.Parse(size.ToString("0.00"));
                            _NgSizeInfo.StandardValue = ((Slice1to2Craft)m_craft).CamerCraft[CameraEnum.Cam2].SizeInfoList[i].StDValue;

                            if (((Slice1to2Craft)m_craft).CamerCraft[CameraEnum.Cam2].SizeInfoList[i].DB)
                            {

                                if (size < (((Slice1to2Craft)m_craft).CamerCraft[CameraEnum.Cam2].SizeInfoList[i].StDValue + ((Slice1to2Craft)m_craft).CamerCraft[CameraEnum.Cam2].SizeInfoList[i].StD_Down) ||
                                 size > (((Slice1to2Craft)m_craft).CamerCraft[CameraEnum.Cam2].SizeInfoList[i].StDValue + ((Slice1to2Craft)m_craft).CamerCraft[CameraEnum.Cam2].SizeInfoList[i].StD_Up))
                                {
                                
                                  //  MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = ((SizeEnum)i).ToString() + ":" + size.ToString("0.00"), m_stuGetColor = HalconColor.red , m_nFontSize = 20 });
                                    _NgSizeInfo.IsNG = true;
                                    _NgSizeInfo.DBInfo.UpTolerance = (((Slice1to2Craft)m_craft).CamerCraft[CameraEnum.Cam2].SizeInfoList[i].StD_Up);
                                    _NgSizeInfo.DBInfo.DownTolerance = (((Slice1to2Craft)m_craft).CamerCraft[CameraEnum.Cam2].SizeInfoList[i].StD_Down);
                                    _NgSizeInfo.DBInfo.IsAlarm = true;
                                    InnerIsOK = false;
                                    OutterIsOK = false;

                                    if (!InnerIsOK)
                                    {
                                        MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = ((SizeEnum)i).ToString() + ":" + size.ToString("0.00"), m_stuGetColor = HalconColor.red , m_nFontSize = 20 });
                                    }

                                }


                            }

                            if (((Slice1to2Craft)m_craft).CamerCraft[CameraEnum.Cam2].SizeInfoList[i].BJ)
                            {
                                if (size < (((Slice1to2Craft)m_craft).CamerCraft[CameraEnum.Cam2].SizeInfoList[i].StDValue + ((Slice1to2Craft)m_craft).CamerCraft[CameraEnum.Cam2].SizeInfoList[i].StD_BJ_Down) ||
                                 size > (((Slice1to2Craft)m_craft).CamerCraft[CameraEnum.Cam2].SizeInfoList[i].StDValue + ((Slice1to2Craft)m_craft).CamerCraft[CameraEnum.Cam2].SizeInfoList[i].StD_BJ_Up))
                                {
                                    if (InnerIsOK)
                                    {
                                        MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = ((SizeEnum)i).ToString() + ":" + size.ToString("0.00"), m_stuGetColor = HalconColor.orange , m_nFontSize = 20 });

                                    }
                                    _NgSizeInfo.IsNG = true;
                                    _NgSizeInfo.BJInfo.UpTolerance = (((Slice1to2Craft)m_craft).CamerCraft[CameraEnum.Cam2].SizeInfoList[i].StD_BJ_Up);
                                    _NgSizeInfo.BJInfo.DownTolerance = (((Slice1to2Craft)m_craft).CamerCraft[CameraEnum.Cam2].SizeInfoList[i].StD_BJ_Down);
                                    _NgSizeInfo.BJInfo.IsAlarm = true;
                                    InnerIsOK = false;
                                  //  OutterIsOK = false;


                                }


                            }

                            if (((Slice1to2Craft)m_craft).CamerCraft[CameraEnum.Cam2].SizeInfoList[i].TJ)
                            {
                                if (size < (((Slice1to2Craft)m_craft).CamerCraft[CameraEnum.Cam2].SizeInfoList[i].StDValue + ((Slice1to2Craft)m_craft).CamerCraft[CameraEnum.Cam2].SizeInfoList[i].StD_BJ_Down) ||
                                 size > (((Slice1to2Craft)m_craft).CamerCraft[CameraEnum.Cam2].SizeInfoList[i].StDValue + ((Slice1to2Craft)m_craft).CamerCraft[CameraEnum.Cam2].SizeInfoList[i].StD_BJ_Up))
                                {
                                    if (InnerIsOK)
                                    {
                                        MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = ((SizeEnum)i).ToString() + ":" + size.ToString("0.00"), m_stuGetColor = HalconColor.pink, m_nFontSize = 20 });
                                    }
                                    _NgSizeInfo.IsNG = true;
                                    _NgSizeInfo.TJInfo.UpTolerance = (((Slice1to2Craft)m_craft).CamerCraft[CameraEnum.Cam2].SizeInfoList[i].StD_BJ_Up);
                                    _NgSizeInfo.TJInfo.DownTolerance = (((Slice1to2Craft)m_craft).CamerCraft[CameraEnum.Cam2].SizeInfoList[i].StD_BJ_Down);
                                    _NgSizeInfo.TJInfo.IsAlarm = true;
                                    InnerIsOK = false;
                                  //  OutterIsOK = false;


                                }


                            }

                            if (InnerIsOK)
                            {
                                _NgSizeInfo.IsNG = false;
                                MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = ((SizeEnum)i).ToString() + ":" + _NgSizeInfo.DataResult.ToString("0.00"), m_stuGetColor = HalconColor.green, m_nFontSize = 20 });

                            }
                            _NgSizeInfoQueue.Enqueue(_NgSizeInfo);

                        }
                    }
                    else
                    {
                        if (((Slice1to2Craft)m_craft).CamerCraft[CameraEnum.Cam2].SizeInfoList[i].Enable)
                        {
                            _NgSizeInfo.sizeEnum = ((SizeEnum)i);
                            _NgSizeInfo.DataResult = double.Parse(0.ToString("0.00"));
                            _NgSizeInfo.StandardValue = ((Slice1to2Craft)m_craft).CamerCraft[CameraEnum.Cam2].SizeInfoList[i].StDValue;
                            _NgSizeInfo.IsNG = false;

                            if (InnerIsOK)
                            {
                                _NgSizeInfo.IsNG = false;
                                MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = ((SizeEnum)i).ToString() + ":" + _NgSizeInfo.DataResult.ToString("0.00"), m_stuGetColor = HalconColor.green, m_nFontSize = 20 });

                            }
                            _NgSizeInfoQueue.Enqueue(_NgSizeInfo);
                        }
                           
                       // MesssgeShowInfos.Add(new MessageShowInfo { m_strMessage = ((SizeEnum)i).ToString() + ":" + 0, m_stuGetColor = HalconColor.green });
                    }

                  
                }

           
                if (!OutterIsOK)
                {
                    MesssgeShowInfos.Insert(0, new MessageShowInfo { m_strMessage = "NG", m_stuGetColor = HalconColor.red , m_nFontSize = 60 });
                }
                else
                {
                    MesssgeShowInfos.Insert(0, new MessageShowInfo { m_strMessage = "OK", m_stuGetColor = HalconColor.green , m_nFontSize = 60});
                }
                HOperatorSet.CountSeconds(out HtEnd);
                HtEnd = (HtEnd - HtStart) * 1000;
                MesssgeShowInfos.Add(new MessageShowInfo() { m_strMessage = "检测时间:" + HtEnd.D.ToString("0") + "ms", m_stuGetColor = HalconColor.green , m_nFontSize = 20 });
                //数据上传

                ((SingleSliceProduct)process.ProductInfo).DataResults.Enqueue(new ImageDataResult()
                {
                    m_bIsNG = !OutterIsOK,
                    m_nImgIndex = InfoImgInfo.image_ID,
                    m_fExcuseTime = HtEnd,
                    m_qNgSizeInfos = _NgSizeInfoQueue,

                });
               

                (process).ShowHalconWindows.Enqueue(new ShowHalconWindow()
                {
                    m_hImage = InfoImgInfo.ho_image,
                    m_nImageFrameID = InfoImgInfo.image_ID,
                    m_nProductID = 0,
                    m_bIsNG = !OutterIsOK,
                    m_regionShowInfos = RegionShowInfos,
                    m_lineShowInfos = LineShowInfos,
                    m_messageShowInfos = MesssgeShowInfos,
                });

                ISlice1To2Paramter.m_nCurProImgId++;
            }
            catch (Exception ee)
            {
                System.Windows.Forms.MessageBox.Show(ee.ToString());
                Log4netRecord.ErrorLog.Debug(CameraEnum.Cam1.ToString() + "分切2流程报错————" + ee.ToString());
            }

            Thread.Sleep(3);

        }

        public bool DetectEdgeValueAssign(IPlugin pluginEdgeLine, ParamsMeasureLine measureLineParamsTile, Dictionary<string, LineInfo> dicLineInfo, string lineName, List<MessageShowInfo> messageShowInfos = null, List<LineInfo> lineShowInfos = null)
        {
            HTuple starTime = 0, stopTime = 0;
            HOperatorSet.CountSeconds(out starTime);
            bool IsExecuteSuccess = false;
            IPlugin PluginSideLine = pluginEdgeLine;
            ParamsMeasureLine MeasureLineParamsTile;



            if (measureLineParamsTile != null)
            {
                MeasureLineParamsTile = measureLineParamsTile;
            }
            else
            {
                MeasureLineParamsTile = new ParamsMeasureLine();
                MeasureLineParamsTile = PluginSideLine.PluginParams as ParamsMeasureLine; //里氏转换
                                                                                          //  MeasureLineParamsTile.ho_Image = image;
            }
            IsExecuteSuccess = PluginSideLine.ExecuteObj();
            if (IsExecuteSuccess)
            {
                dicLineInfo[lineName].StrLineName = lineName;
                dicLineInfo[lineName].IsLineSuccessfull = true;
                dicLineInfo[lineName].RowBegin = MeasureLineParamsTile.hv_LineRowBegin;
                dicLineInfo[lineName].ColBegin = MeasureLineParamsTile.hv_LineColumnBegin;
                dicLineInfo[lineName].RowEnd = MeasureLineParamsTile.hv_LineRowEnd;
                dicLineInfo[lineName].ColEnd = MeasureLineParamsTile.hv_LineColumnEnd;
                dicLineInfo[lineName].Color = HalconColor.cyan;
                lineShowInfos.Add(dicLineInfo[lineName]);
            }
            else
            {

                dicLineInfo[lineName].StrLineName = lineName;
                dicLineInfo[lineName].IsLineSuccessfull = false;
                dicLineInfo[lineName].RowBegin = MeasureLineParamsTile.hv_RoiMidRow;
                dicLineInfo[lineName].ColBegin = MeasureLineParamsTile.hv_RoiMidColumn;
                dicLineInfo[lineName].RowEnd = MeasureLineParamsTile.hv_RoiMidRow + 2;
                dicLineInfo[lineName].ColEnd = MeasureLineParamsTile.hv_RoiMidColumn + 2;
                dicLineInfo[lineName].Color = HalconColor.red;
                if (messageShowInfos != null && lineShowInfos != null)
                {
                    messageShowInfos.Add(new MessageShowInfo { m_strMessage = lineName + ":抓边未成功", m_stuGetColor = HalconColor.red, m_nFontSize = 20 });
                    lineShowInfos.Add(dicLineInfo[lineName]);

                }

            }
            HOperatorSet.CountSeconds(out stopTime);
            if (Project.Instance.Is_bShowStationTiming)
            {
                messageShowInfos.Add(new MessageShowInfo { m_strMessage = lineName + ":抓边处理时长：" + ((stopTime - starTime) * 1000).D.ToString("0.00") + "ms", m_stuGetColor = HalconColor.green, m_nFontSize = 20 });

            }
            return IsExecuteSuccess;
        }

    }
}

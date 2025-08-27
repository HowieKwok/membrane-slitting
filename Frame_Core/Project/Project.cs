using LogTool;

using NPOI.SS.Formula.Functions;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media.Media3D;

using static NPOI.SS.Formula.PTG.ArrayPtg;

namespace Frame_Core
{

    public class Project
    {
        private Project() { }
        private static Project project = null;
        /// <summary>
        /// 项目类实例(单例模式)
        /// </summary>
        public static Project Instance
        {
            get
            {
                if (project == null)
                {
                    project = new Project();
                }
                return project;
            }
            set
            {
                project = value;
            }
        }


        /// <summary>
        /// 模块服务管理
        /// </summary>
        public ModelServiceManage modelServiceManage = new ModelServiceManage();

        /// <summary>
        /// 日志参数
        /// </summary>
        public LogParam logParam = new LogParam();


        /// <summary>
        /// 参数路径设置
        /// </summary>
        public ParamPathSet paramPathSet = new ParamPathSet();



        /// <summary>
        /// 用户管理
        /// </summary>
        public UserManage userManage = new UserManage();

        /// <summary>
        /// 权限管理参数
        /// </summary>
        public PurviewManage purviewManage = new PurviewManage();

        //当前活动窗体类型
        public object activeObject { get; set; }

        /// <summary>
        /// 相机管理
        /// </summary>
        public Dictionary<CameraEnum, CameraManager> cameraManager = new Dictionary<CameraEnum, CameraManager>();

        /// <summary>
        /// 光源管理
        /// </summary>
        public LightManager[] lightManager = new LightManager[0];

        /// <summary>
        /// 3D测厚纠偏管理
        /// </summary>
        public Thick3DManage[] thick3DManages = new Thick3DManage[0];

        /// <summary>
        /// 磁盘参数
        /// </summary>
        public DriveParameter driveParameter = new DriveParameter();


        /// <summary>
        /// 设备PLC管理
        /// </summary>
        public PFTPLCManage cutOverlayPLCManage = new PFTPLCManage();

        /// <summary>
        /// 打标机管理
        /// </summary>
        public DBJCardManager dbjCardManager = new DBJCardManager();

        /// <summary>
        ///  副机管理
        /// </summary>
        public SlaveMachineManager slaveMachineManager = new SlaveMachineManager();


        //状态
        public bool Is_MojuanStart = false;//膜卷开始标志
        public bool Is_simulation = false; //项目仿真运行标志位。

        public bool Is_bShowStationTiming = false;//是否显示工位时间

        /// <summary>
        /// 自动仿真标志
        /// </summary>
        public bool m_SimAutoFlag = false;

        /// <summary>
        /// 仿真运行标志
        /// </summary>
        public bool m_SimRunFlag = false;
        /// <summary>
        /// 启用产品图单步仿真开关
        /// </summary>
        public bool Is_bProductSwitch = false;
        public bool Is_SaveModelImage = false;//保存模板图
        public bool Is_Run = false;//软件运行标志


        /// <summary>
        /// 进入自动标定模式
        /// </summary>
        public bool IsAutoCalibrationModel = false;

        //系统参数
        public SystemInfo systemInfo = new SystemInfo();

        //工艺配方参数
        public CraftBase craftBase = new CraftBase();


        //流程类
        public BaseProDevice baseProDevice = new BaseProDevice();

        // 流程列表
        public List<ProcessServices> m_ProcessList = new List<ProcessServices>();
        private List<List<ModuleInfo>> instanceList = new List<List<ModuleInfo>>(); //用于保存整个Instance的结构，流程和模块。



        public void CloseAllDev()
        {
            try
            {
                foreach (var item in cameraManager.Values)
                {
                    if (item != null)
                    {
                        item.Close();
                    }

                }

                for (int i = 0; i < lightManager.Length; i++)
                {
                    if (lightManager[i] != null)
                    {
                        lightManager[i].Close();
                    }
                }

                if (cutOverlayPLCManage != null)
                {
                    cutOverlayPLCManage.Disconnect();
                }

                if (dbjCardManager != null)
                {
                    dbjCardManager.Close();
                }

                if (slaveMachineManager != null)
                {
                    slaveMachineManager.Close();
                }

                //if (correctManage != null)
                //{
                //    correctManage.Close();
                //}

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);

            }


        }

        public bool INIProject()
        {
            bool initParamResult = true;
            bool initObjectResult = true;
            string paramPath = "";

            int camNum = 2;
            int lightNum = 2;

            try
            {
                //初始化路径
                paramPathSet.InitPath();
                //加载系统参数              
                systemInfo.filePath = paramPathSet.SystemSetPathName;
                systemInfo = (SystemInfo)systemInfo.Read();
                if (systemInfo == null)
                {
                    Log4netRecord.ErrorLog.Fatal("系统配置加载失败");
                    systemInfo = new SystemInfo();
                }
                systemInfo.filePath = paramPathSet.SystemSetPathName;
                systemInfo.IsHostMachine = true;



                //初始化参数路径到生产类型目录下
                //paramPathSet.InitRelativeParamPath(systemInfo.CurrentMaterial.ToString());
                ComStationsanwahlEnum comStationsanwahlEnum = (ComStationsanwahlEnum)systemInfo.ComStationsanwahl;
                paramPathSet.InitRelativeParamPath(comStationsanwahlEnum.ToString());

                //加载日志参数
                LogParamManage.GetLogParam(paramPathSet.LogParamPath, ref logParam);
                //加载参数及初始化配置文件和对象
                Log4netRecord.InitLog4netConfigure(logParam);//初始化软件报错日志                


                //初始化配方、生产路径
                paramPathSet.UpdateCraftPath(systemInfo.CurrentCraft, systemInfo.currentMojuanhao, systemInfo.CurrentMaterial);

                //加载用户管理参数                       
                paramPath = paramPathSet.UserParamPathName;
                initParamResult &= userManage.InitUserParam(paramPath);

                //加载 配方参数 和 流程参数
                ChnageCraft(systemInfo.CurrentCraft, systemInfo.CurrentMaterial);


                //加载相机参数
                //加载相机参数、标定参数
                string[] sizeCoefficientName = new string[0];//尺寸系数名称

                foreach (SizeEnum item in Enum.GetValues(typeof(SizeEnum)))//相机
                {
                    Array.Resize(ref sizeCoefficientName, sizeCoefficientName.Length + 1);
                    sizeCoefficientName[sizeCoefficientName.Length - 1] = item.ToString();
                }

                //主机
                cameraManager.Clear();
               if (systemInfo.CurrentMaterial == MaterialCrafEnum.PFT清洗机)
                {
                    InitCam(2);
                   // Init3DThickManage(2);
                    baseProDevice = new PFTCleanProDevice();

                    lightNum = 1;
                }
                //注册流程相机图像队列
                //注册流程相机图像队列
                //if (systemInfo.CurrentMaterial == MaterialCrafEnum.PFT清洗机)
                //{
                //    for (int i = 0; i < baseProDevice.m_DicCamOperaParam.Count; i++)
                //    {
                //        if (i == 0 )
                //        {
                //            baseProDevice.m_DicCamOperaParam[i].QueueGrabImage = cameraManager[(CameraEnum)i].Camera.QueueGrabImage;
                //            cameraManager[(CameraEnum)i].Camera.CamCallbackEvent = baseProDevice.m_DicCamOperaParam[i].CallBackImgDataDeal;
                //        }
                //        else if (i == 1)
                //        {
                //            baseProDevice.m_DicCamOperaParam[i].QueueGrabImage = cameraManager[(CameraEnum)1].Camera.QueueGrabImage;
                //            cameraManager[(CameraEnum)i].Camera.CamCallbackEvent = baseProDevice.m_DicCamOperaParam[i].CallBackImgDataDeal;
                //        }
                //        //留给3D相机使用

                //    }
                //}
                //else
                {
                    foreach (var item in cameraManager.Keys)
                    {
                
                        //注册流程相机图像队列
                        if (!cameraManager[item].Camera.bIs3D)
                        {
                            baseProDevice.m_DicCamOperaParam[(int)item].QueueGrabImage = cameraManager[item].Camera.QueueGrabImage;
                        }
                        else
                        {
                            baseProDevice.m_DicCamOperaParam[(int)item].Queue3DImage = cameraManager[item].Camera.QueueGrab3DImage;
                        }

                        cameraManager[item].Camera.CamCallbackEvent = baseProDevice.m_DicCamOperaParam[(int)item].CallBackImgDataDeal;
                    }
                }


                //加载光源参数
                lightManager = new LightManager[lightNum];
                for (int i = 0; i < lightManager.Length; i++)
                {
                    lightManager[i] = new LightManager();
                    paramPath = paramPathSet.LightParamPathName + $"{i + 1}";
                    initParamResult &= lightManager[i].InitLightParam(paramPath);
                    initObjectResult &= lightManager[i].IniLightSystem();
                }
                

                ///加载磁盘参数
                if (driveParameter == null) driveParameter = new DriveParameter();
                driveParameter.filePath = paramPathSet.DiverParamPathName;
                driveParameter = (DriveParameter)driveParameter.Read();

                Project.Instance.paramPathSet.initProdDataPath = $"{driveParameter.driveName}\\生产数据\\";



                //初始化主设备参数
                InitModel_cutOverlayPLCManage();

                //加载打标机软件参数
                dbjCardManager = new DBJCardManager();
                paramPath = paramPathSet.DBJParamPathName;
                initParamResult &= dbjCardManager.InitParam(paramPath);
                initObjectResult &= dbjCardManager.InitPLC();


                //加载副机参数
                slaveMachineManager = new SlaveMachineManager();
                paramPath = paramPathSet.SlaveParamPathName;
                initParamResult &= slaveMachineManager.InitParam(paramPath);
                initObjectResult &= slaveMachineManager.InitPLC();


                //新建膜卷路径
                NewMujuanhao(systemInfo.CurrentCraft, systemInfo.currentMojuanhao, systemInfo.CurrentMaterial);


                ReadProject(); // 加载配方后，默认添加一个初始编号为0的产品。
                SaveProject();

                baseProDevice.INIPro();
                baseProDevice.INIProduct();



                //加载权限管理参数
                foreach (var ModelName in Enum.GetValues(typeof(ModelName)))
                {
                    modelServiceManage.AddModel(ModelName.ToString(), new ModelControl());
                }
                paramPath = paramPathSet.purviewParamPathName;
                initParamResult &= purviewManage.InitpurviewParam(paramPath);
                purviewManage.UpdateModelPurviewParam(modelServiceManage.m_dicModels, purviewManage.purviewParam.l_modelPrivilege);
                purviewManage.purviewParam.Save();



            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
                System.Windows.Forms.MessageBox.Show("初始化失败，请检查日志文件！");
                initParamResult = false;
            }
            return initParamResult;
        }

        /// <summary>
        /// 初始化相机参数
        /// </summary>
        /// <param name="camNum"></param>
        /// <returns></returns>
        public bool InitCam(int camNum)
        {
            for (int i = 0; i < camNum; i++)
            {
                CameraEnum cameraEnum = (CameraEnum)i;
                cameraManager.Add(cameraEnum, new CameraManager() { CameraNameEnum = cameraEnum });

                string paramPath = paramPathSet.CamParamPathName + $"{cameraEnum}";

              
                cameraManager[cameraEnum].InitCameraParam(paramPath);
                cameraManager[cameraEnum].IniCameraSystem(i);

                cameraManager[cameraEnum].InitCalibrationPlateParam(paramPathSet.CalibrationPlateParamPath + $"{cameraEnum}");
                cameraManager[cameraEnum].InitCheckerboardParam(paramPathSet.CheckerboardParamPath + $"{cameraEnum}");
                cameraManager[cameraEnum].InitPretargetingCalibParam(paramPathSet.PretargetingCalibParamPath + $"{cameraEnum}");
                cameraManager[cameraEnum].InitImageSaveParamer(paramPathSet.ImageSaveParamPath + $"{cameraEnum}");
                cameraManager[cameraEnum].InitOVHMarkParam(paramPathSet.CutOverlayMarkParamPath + $"{cameraEnum}");
                cameraManager[cameraEnum].InitMsaCircleParam(paramPathSet.MsaCircleParamPath + $"{cameraEnum}");
                cameraManager[cameraEnum].InitMsaLineWidthParam(paramPathSet.MsaLineWidthParamPath + $"{cameraEnum}");
                cameraManager[cameraEnum].InitMsa3DThickDetect(paramPathSet.Msa3DThickParamPath + $"{cameraEnum}");
                cameraManager[cameraEnum].m_SizeCheckPointManage.InitCamSizeCheckPointParam(paramPathSet.CheckDJParamPath + $"{cameraEnum}");

            }

            return true;
        }



        public bool Init3DThickManage(int thickNum)
        {
            bool result = true;
            try
            {

                thick3DManages = new Thick3DManage[thickNum];
                for(int i=0;i<thickNum;i++)
                {
                    thick3DManages[i] = new Thick3DManage();
                    thick3DManages[i].InitParam(paramPathSet.CamParamPathName+$"3D{i+1}");
                    result= true;
                }
            }
            catch (Exception ee)
            {

                result= false; ;
             
            }
            return result;
        
        }



        /// <summary>
        /// 初始化模块-切叠设备PLC
        /// </summary>
        public void InitModel_cutOverlayPLCManage()
        {
            ////加载主设备PLC参数  
            if (cutOverlayPLCManage == null) cutOverlayPLCManage = new PFTPLCManage();
            cutOverlayPLCManage.LoadParam(paramPathSet.CutOverlayPLCParamPath);
            cutOverlayPLCManage.Connect(cutOverlayPLCManage.m_DevParam.IsConnect, PLCTypeEnum.Omron, (CommunicationProtocolEnum)cutOverlayPLCManage.m_DevParam.ComType, cutOverlayPLCManage.m_DevParam.PLC_IP, cutOverlayPLCManage.m_DevParam.DevPort,(emDataFormat)cutOverlayPLCManage.m_DevParam.nByteTrans);
        }

        public void BindCamImageSource(Dictionary<int, BaseProParam> m_DicCamOperaParam)
        {
            baseProDevice.m_DicCamOperaParam = m_DicCamOperaParam;
            //if (systemInfo.CurrentMaterial == MaterialCrafEnum.PFT清洗机)
            //{
            //    for (int i = 0; i < m_DicCamOperaParam.Count; i++)
            //    {
            //        if (i == 0)
            //        {
            //            baseProDevice.m_DicCamOperaParam[i].QueueGrabImage = cameraManager[(CameraEnum)i].Camera.QueueGrabImage;
            //            cameraManager[(CameraEnum)i].Camera.CamCallbackEvent = baseProDevice.m_DicCamOperaParam[i].CallBackImgDataDeal;
            //        }
            //        else if (i == 2)
            //        {
            //            baseProDevice.m_DicCamOperaParam[i].QueueGrabImage = cameraManager[(CameraEnum)1].Camera.QueueGrabImage;
            //            cameraManager[(CameraEnum)i].Camera.CamCallbackEvent = baseProDevice.m_DicCamOperaParam[i].CallBackImgDataDeal;
            //        }
            //        //留给3D相机使用
            //    }
            //}
            //else
            {
                foreach (var item in cameraManager.Keys)
                {
                    //注册流程相机图像队列
                    if (!cameraManager[item].Camera.bIs3D)
                    {
                        baseProDevice.m_DicCamOperaParam[(int)item].QueueGrabImage = cameraManager[item].Camera.QueueGrabImage;
                    }
                    else
                    {
                        baseProDevice.m_DicCamOperaParam[(int)item].Queue3DImage = cameraManager[item].Camera.QueueGrab3DImage;
                    }

                    cameraManager[item].Camera.CamCallbackEvent = baseProDevice.m_DicCamOperaParam[(int)item].CallBackImgDataDeal;
                }
            }


        }
        public void RemoveBindCamImageSource()
        {
            foreach (var item in cameraManager.Keys)
            {
                //注册流程相机图像队列                  
                // baseProDevice.m_DicCamOperaParam[(int)item].QueueGrabImage = cameraManager[item].Camera.QueueGrabImage;
                cameraManager[item].Camera.CamCallbackEvent -= baseProDevice.m_DicCamOperaParam[(int)item].CallBackImgDataDeal;
                //cameraManager[item].Camera.CamCallbackEvent += baseProDevice.m_DicCamOperaParam[(int)item].CallBackImgDataDeal;
            }
        }

        /// <summary>
        /// 加载工艺 和 流程列表
        /// </summary>
        /// <param name="_CurrentCraft"></param>
        /// <param name="_Currentmaterial"></param>
        /// <returns></returns>
        public bool ChnageCraft(string _CurrentCraft, MaterialCrafEnum _Currentmaterial)
        {
            bool flag1 = true;
            try
            {
                //初始化配方流程路径
                craftBase.filePath = paramPathSet.CraftPath + _Currentmaterial.ToString() + "\\" + _CurrentCraft;
                //craftInfo.fileFullPath = paramPathSet.craftParamPath;
                switch (_Currentmaterial)
                {
                   
                    case MaterialCrafEnum.PFT清洗机:
                        craftBase = craftBase.Read<PFTCleanCraft>();
                        if (craftBase == null)
                        {
                            flag1 = false;
                            craftBase = new PFTCleanCraft();
                            craftBase.Init();
                        }
                        break;


                    default:
                        craftBase = new PFTCleanCraft();
                        break;
                }
                if (flag1 == false)
                {
                    Log4netRecord.ErrorLog.Fatal("工艺配方加载失败:" + craftBase.fileFullPath);
                    return false;
                }
                craftBase.fileFullPath = paramPathSet.CraftPath + _Currentmaterial.ToString() + "\\" + _CurrentCraft + ".json";
            }
            // craftBase.fileFullPath = paramPathSet.CraftPath + _Currentmaterial.ToString() + "\\" + _CurrentCraft + ".cfts";
            //加载配方的检测流程。
            //flag1 = ReadProject(); // 加载配方后，默认添加一个初始编号为0的产品。
            //SaveProject();

            catch (Exception ex)
            {
                flag1 = false;
                Log4netRecord.ErrorLog.Debug(ex);
            }
            return flag1;
        }
        object NewMujuanhao_lock = new object();
        public void NewMujuanhao(string _currentProduct, string _currentMojuanhao, MaterialCrafEnum _materialCrafEnum)
        {
            lock (NewMujuanhao_lock)
            {
                //初始化配方、生产路径
                //初始化配方、生产路径        
                paramPathSet.UpdateParamPath(_currentProduct, _currentMojuanhao, _materialCrafEnum, m_ProcessList, systemInfo.IsHostMachine);

                cutOverlayPLCManage.SetLogPath(paramPathSet.ProductMojuanPath);

                string strPath = paramPathSet.ProductMojuanPath;
                if (!Directory.Exists(strPath))
                    Directory.CreateDirectory(strPath);

                TJBJInfo.TJBJInfoPath = strPath + "CCD停机报警信息记录.ini";

                //重开膜卷初始化打标软件参数和信息路径
                dbjCardManager.ReadLogPath = strPath + "打标软件接收记录-接收MarkingSoftwareReceiveRecord.ini";
                dbjCardManager.SendLogPath = strPath + "打标软件发送记录-发送MarkingSoftwareSendRecord.ini";
                dbjCardManager.SendHeartPath = strPath + "打标软件发送记录-心跳MarkingSoftwareHeartbeatRecord.ini";
                dbjCardManager.SendDBPath = strPath + "打标软件发送记录-打标MarkingSoftwareMarkingRecord.ini";
                dbjCardManager.SendJPPath = strPath + "打标软件发送记录-纠偏MarkingSoftwareOffsetRecord.ini";

                ////重开膜卷初始化主副机软件参数和信息路径
                slaveMachineManager.ReadLogPath = strPath + "主副机接收记录-接收MarkingSoftwareReceiveRecord.ini";
                slaveMachineManager.SendLogPath = strPath + "主副机发送记录-发送MarkingSoftwareSendRecord.ini";
                slaveMachineManager.SendHeartPath = strPath + "主副机发送记录-心跳MarkingSoftwareHeartbeatRecord.ini";
            


            }
        }

        /// <summary>
        /// 获取光源状态
        /// </summary>
        /// <returns></returns>
        public bool GetLightState()
        {
            bool openOrClosing = true;
            try
            {
                if (lightManager != null)
                {
                    for (int i = 0; i < lightManager.Length; i++)
                    {
                        openOrClosing &= lightManager[i].openOrCloseFlag;
                    }
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
            return openOrClosing;
        }

        #region Process流程控制
        /// <summary>
        /// 创建流程
        /// </summary>
        public int CreateProcess()
        {
            ProcessServices processervice = new ProcessServices();
            //获取新不重复的id  如果已经存在  1,2,4   那么久获得的id 是 3
            bool flag = false;
            int id = 0;
            do
            {
                flag = true;

                foreach (ProcessServices procS in m_ProcessList)
                {
                    if (procS.process.processInfo.ProcessID == id)
                    {
                        id++;
                        flag = false;
                        break;
                    }
                }

                if (flag == true)
                {
                    break;
                }
            } while (true);
            processervice.process.processInfo.ProcessID = id;
            processervice.process.processInfo.ProcessName = "流程" + id.ToString();
            m_ProcessList.Add(processervice);
            return id;
        }
        public void CreateProcess(int ID, string Name)
        {
            ProcessServices processervice = new ProcessServices();
            processervice.process.processInfo.ProcessID = ID;
            processervice.process.processInfo.ProcessName = Name;
            m_ProcessList.Add(processervice);
        }
        /// <summary>
        /// 根据id获取对应的Process
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Process GetProcessById(int id)
        {
            return m_ProcessList.FirstOrDefault(c => c.process.processInfo.ProcessID == id).process;
        }
        /// <summary>
        /// 根据id获取对应的ProcessService
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ProcessServices GetProcessServiceById(int id)
        {
            return m_ProcessList.FirstOrDefault(c => c.process.processInfo.ProcessID == id);
        }
        /// <summary>
        /// 根据Name获取对应的Process
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Process GetProcessByName(string name)
        {
            return m_ProcessList.FirstOrDefault(c => c.process.processInfo.ProcessName == name).process;
        }
        /// <summary>
        /// 根据Name获取对应的ProcessService
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ProcessServices GetProcessServiceByName(string name)
        {
            return m_ProcessList.FirstOrDefault(c => c.process.processInfo.ProcessName == name);
        }
        /// <summary>
        /// 删除Process
        /// </summary>
        /// <param name="proc"></param>
        public void DeleteProcess(Process proc)
        {
            if (proc != null)
            {
                foreach (IPlugin Obj in proc.m_ModuleObjList)
                {
                    Obj.DisPoseObj();
                }
                proc.m_ModuleObjList.Clear();
                GC.Collect();
            }
        }
        /// <summary>
        /// 删除ProcessService
        /// </summary>
        /// <param name="procS"></param>
        public void DeleteProcessService(ProcessServices procS)
        {
            if (procS != null)
            {
                foreach (IPlugin Obj in procS.process.m_ModuleObjList)
                {
                    Obj.DisPoseObj();
                }
                m_ProcessList.Remove(procS);
                GC.Collect();
            }
        }
        /// <summary>
        /// 清空所有ProcessService和Process
        /// </summary>
        public void DeleteALLProcess()
        {
            foreach (ProcessServices processService in m_ProcessList)
            {
                foreach (IPlugin Obj in processService.process.m_ModuleObjList)
                {
                    Obj.DisPoseObj();
                }
            }
            m_ProcessList.Clear();
            GC.Collect();
        }

        /// <summary>
        /// 获取所有项目运行状态
        /// </summary>
        /// <returns></returns>
        public bool GetStates()
        {
            foreach (ProcessServices pro in m_ProcessList)
            {
                if (pro.GetThreadStatus() == true) return true;
            }
            return false;
        }

        /// <summary>
        /// 连续运行
        /// </summary>
        /// <param name="projectID"></param>
        public void StartRun(int processID)
        {
            Is_Run = true;
            m_ProcessList.FirstOrDefault(c => c.process.processInfo.ProcessID == processID).RunMode = RunMode.RunCycle;//循环
            m_ProcessList.FirstOrDefault(c => c.process.processInfo.ProcessID == processID).Start();
        }

        /// <summary>
        /// 执行一次
        /// </summary>
        /// <param name="projectID"></param>
        public void ExecuteOnce(int processID)
        {
            //循环
            m_ProcessList.FirstOrDefault(c => c.process.processInfo.ProcessID == processID).RunMode = RunMode.RunOnce;
            m_ProcessList.FirstOrDefault(c => c.process.processInfo.ProcessID == processID).Start();
        }

        /// <summary>
        /// 停止运行
        /// </summary>
        /// <param name="projectID"></param>
        public void StopRun(int processID)
        {
            Is_Run = false;
            m_ProcessList.FirstOrDefault(c => c.process.processInfo.ProcessID == processID).RunMode = RunMode.None;//停止
            m_ProcessList.FirstOrDefault(c => c.process.processInfo.ProcessID == processID).Stop();
        }

        public void SaveProject()
        {
            instanceList.Clear();
            foreach (ProcessServices item in m_ProcessList)
            {
                instanceList.Add(item.process.m_ModuleObjList.Select(s => s.moduleInfo).ToList());
                string ProcessPath = Path.Combine(paramPathSet.ProcessPath, item.process.processInfo.ProcessName);
                item.process.SaveProcess(ProcessPath);
            }
            bool flag1 = XmlSerialization.ObjectToXml(paramPathSet.InstanceParamPath, instanceList);
        }

        public bool ReadProject()
        {
            try
            {
                //开软件时更具相机型号开启流程，后续所有的切换，只切换process。相当于只切换插件参数
                // DeleteALLProcess();  
                if (File.Exists(paramPathSet.InstanceParamPath))
                {
                    instanceList = (List<List<ModuleInfo>>)XmlSerialization.XmlToObject(paramPathSet.InstanceParamPath, instanceList.GetType());
                }
                else
                {
                    instanceList = new List<List<ModuleInfo>>();
                }
                int Index = 0;
                foreach (List<ModuleInfo> item in instanceList)
                {
                    DeleteProcess(m_ProcessList[Index].process);
                    Process process = m_ProcessList[Index].process;
                    process.processInfo.ProcessID = item[0].ProcessID;
                    process.processInfo.ProcessName = item[0].ProcessName;

                    string ProcessPath00 = Path.Combine(paramPathSet.ProcessPath, item[0].ProcessName);

                    foreach (var Moduleinfoitem in item)
                    {
                        IPlugin PluginLine = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[Moduleinfoitem.PluginName].PluginsBuildName);
                        PluginLine.moduleInfo = Moduleinfoitem;

                        string ModuleinfoPath00 = Path.Combine(ProcessPath00, Moduleinfoitem.ModuleName);
                        PluginLine.PluginParams.filePath = ModuleinfoPath00;//Moduleinfoitem.filePath
                        PluginLine.PluginParams = (PluginParamsBase)PluginLine.PluginParams.Read();
                        PluginLine.PluginParams.filePath = ModuleinfoPath00;
                        process.AddModuleObj(PluginLine);
                    }
                    //给每个ProcessService绑定process

                    // m_ProcessList[Index].process = process;
                    Index++;
                }
                baseProDevice.INIPro();
                return true;
            }
            catch (Exception ee)
            {
                Log4netRecord.ErrorLog.Fatal("配方的检测流程加载失败:" + ee);
                return false;
            }


            //FileStream fs = new FileStream(@"D:\testCode\mef\Frame_Core\Frame_Core\bin\x64\Debug\ProjectParams\Record1.data", FileMode.Open);
            //BinaryFormatter bf = new BinaryFormatter();
            //m_ProcessInfoList = (List<ProcessInfo>)bf.Deserialize(fs);

        }

        #endregion

    }
}

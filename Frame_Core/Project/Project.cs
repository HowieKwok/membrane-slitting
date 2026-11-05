using ControlBoard.Model;
using ControlBoard.服务类;
using DevMgmt;
using DevMgmt.Cameras.Models;
using DevMgmt.Light.Models;
using Frame_Core.业务类;
using FrameCommunity.DataOperation;
using FrameCommunity.错误代码;
using LogTool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
       // public DBJCardManager dbjCardManager = new DBJCardManager();

        /// <summary>
        ///  副机管理
        /// </summary>
        public SlaveMachineManager slaveMachineManager = new SlaveMachineManager();


        //状态
        public bool Is_MojuanStart = false;//膜卷开始标志
        public bool Is_simulation = false; //项目仿真运行标志位。

        public bool Is_bShowStationTiming = false;//是否显示工位时间
        public bool LightOnOff = false;//光源状态开关
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

        //相机参数
        public CamParam M_CamParam = new CamParam();

        //图像保存参数
        public NewImageSaveParamer M_NewImageSaveParamer = new NewImageSaveParamer();

        //光源参数
        public LightControllerParam M_LightControllerParam = new LightControllerParam();

        //PLC参数
        public PLCDevParam M_PLCDevParam = new PLCDevParam();

        //PLC服务类
        public Server_DiecuttingCutPLC server_DiecuttingCutPLC = null;

        //控制卡参数
        public ParamControlCord M_ParamControlCord = new ParamControlCord();

        //点检参数
        public Param_PointInspection M_param_PointInspection = new Param_PointInspection();
        /// <summary>
        /// 相机相关
        /// </summary>
        public Dictionary<int, Tuple<CameraDev, CameraRelevancy>> M_CameraRelevancies = new Dictionary<int, Tuple<CameraDev, CameraRelevancy>>();

        //流程类
        public BaseProDevice baseProDevice = new BaseProDevice();
        /// <summary>
        /// 流程管理模块，集中管理流程列表、流程保存/读取、运行控制和线程释放。
        /// Project 保留旧接口作为兼容门面，避免调试界面和插件旧代码大面积修改。
        /// </summary>
        public ProcessManager Process流程插件管理者 = new ProcessManager();

        /// <summary>
        /// 流程列表旧入口。
        /// 为兼容调试界面、业务类和插件中已有的 Project.Instance.m_ProcessList 调用，保留该属性名；
        /// 实际数据由 Proc.List 统一维护，Project 不再单独保存 instanceList。
        /// </summary>
        public List<ProcessServices> m_ProcessList
        {
            get { return Process流程插件管理者.List; }
        }


        private List<List<ModuleInfo>> instanceList = new List<List<ModuleInfo>>(); //用于保存整个Instance的结构，流程和模块。


        public void Init_Dev()
        {
            try
            {
                M_CamParam.l_CameraDevs.ConnectAllCam();
                M_CamParam.l_CameraDevs.StartGrabImage();        

                M_LightControllerParam.l_LightControllerDevs.ConnectAllDev();
                M_LightControllerParam.l_LightControllerDevs.OpenAllLight();
                LightOnOff = M_LightControllerParam.l_LightControllerDevs.GetAllLightOpenState();

                server_DiecuttingCutPLC.Connect(M_PLCDevParam.IsConnect, PLCTypeEnum.Omron, (CommunicationProtocolEnum)M_PLCDevParam.ComType, M_PLCDevParam.PLC_IP, M_PLCDevParam.DevPort, (emDataFormat)M_PLCDevParam.nByteTrans);

                ServiceCardManage.Instance.InitServiceCard(M_ParamControlCord);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);

            }
        }
        public void Close_Dev()
        {
            try
            {
                M_CamParam.l_CameraDevs.CloseAllCam();
                M_LightControllerParam.l_LightControllerDevs.CloseAllDev();

                server_DiecuttingCutPLC?.Disconnect();

                ServiceCardManage.Instance.GetIControlCard()?.Board_Close();
                //if (cutOverlayPLCManage != null)
                //{
                //    cutOverlayPLCManage.Disconnect();
                //}

                //if (dbjCardManager != null)
                //{
                //    dbjCardManager.Close();
                //}

                //if (slaveMachineManager != null)
                //{
                //    slaveMachineManager.Close();
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

            try
            {
                ErrorWorks.RegisterErrorInfo();

                paramPathSet.InitRelativeParamPath();

                //加载日志参数
                LogParamManage.GetLogParam(paramPathSet.LogParamPath, ref logParam);
                //加载参数及初始化配置文件和对象
                Log4netRecord.InitLog4netConfigure(logParam);//初始化软件报错日志 

                //加载系统参数
                systemInfo = systemInfo.ReadExtension<SystemInfo>(paramPathSet.SystemPath, $"SystemParam");
                systemInfo.IsHostMachine = true;


                //初始化配方、生产路径
                paramPathSet.UpdateCraftPath(systemInfo.RecipeName, systemInfo.currentMojuanhao, systemInfo.CraftType);

                //加载用户管理参数                       
                paramPath = paramPathSet.UserParamPathName;
                initParamResult &= userManage.InitUserParam(paramPath);


                //【1】加载相机参数
                M_CamParam = M_CamParam.ReadExtension<CamParam>(paramPathSet.SystemPath, $"AllCamParam");

                //【2】加载光源参数
                M_LightControllerParam = M_LightControllerParam.ReadExtension<LightControllerParam>(paramPathSet.SystemPath, $"AllLightControllerParam");

                //【3】加载 配方参数
                craftBase = Craft_ModelService.GetCraft(paramPathSet.CraftPath, systemInfo.CraftType, systemInfo.RecipeName);

                //【4】加载图像保存参数
                M_NewImageSaveParamer = M_NewImageSaveParamer.ReadExtension<NewImageSaveParamer>(paramPathSet.SystemPath, $"ImageSaveParamer", M_CamParam.l_CameraDevs.Count);

                //【5】加载PLC参数
                M_PLCDevParam = M_PLCDevParam.ReadExtension<PLCDevParam>(paramPathSet.SystemPath, $"PLCDevParamr");
                server_DiecuttingCutPLC = new Server_DiecuttingCutPLC(M_PLCDevParam);

                //【6】加载点检参数
                M_param_PointInspection = M_param_PointInspection.ReadExtension<Param_PointInspection>(paramPathSet.SystemPath, $"PointInspectionParamer");

                //【7】加载控制卡参数
                M_ParamControlCord = M_ParamControlCord.ReadExtension<ParamControlCord>(paramPathSet.SystemPath, $"ControlCordParam");

                //初始化运行内存及CPU读取功能
                MyMetrics.GetPrivateWorkingSetMB();
                MyMetrics.GetCpuPercentLikeTaskManager();

                //【100】连接设备
                Init_Dev();

                //相机相关
                M_CameraRelevancies.Clear();
                for (int i = 0; i < M_CamParam.l_CameraDevs.Count; i++)
                {
                    CameraEnum cameraEnum = (CameraEnum)i;
                    CameraDev cameraDev = M_CamParam.l_CameraDevs[i];
                    CameraRelevancy cameraRelevancy = new CameraRelevancy();
                    M_CameraRelevancies.Add(i, new Tuple<CameraDev, CameraRelevancy>(cameraDev, cameraRelevancy));
                    InitCamRelevancies(i);
                }

                if (systemInfo.CraftType == MaterialCrafEnum.激光模切一体机)
                {

                    // Init3DThickManage(2);
                    baseProDevice = new DieCutProDevice();

                }


                //初始化点检功能
                PointInspectionWorks.InitPointInspectionFunction();



                ///加载磁盘参数
                if (driveParameter == null) driveParameter = new DriveParameter();
                driveParameter.filePath = paramPathSet.DiverParamPathName;
                driveParameter = (DriveParameter)driveParameter.Read();

                Project.Instance.paramPathSet.initProdDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "生产数据") + "\\";



                //初始化主设备参数
                //      InitModel_cutOverlayPLCManage();

                //加载打标机软件参数
                //dbjCardManager = new DBJCardManager();
                //paramPath = paramPathSet.DBJParamPathName;
                //initParamResult &= dbjCardManager.InitParam(paramPath);
                //initObjectResult &= dbjCardManager.InitPLC();


                //加载副机参数
                //slaveMachineManager = new SlaveMachineManager();
                //paramPath = paramPathSet.SlaveParamPathName;
                //initParamResult &= slaveMachineManager.InitParam(paramPath);
                //initObjectResult &= slaveMachineManager.InitPLC();


                //新建膜卷路径
                initParamResult &= NewMujuanhao(systemInfo.RecipeName, systemInfo.currentMojuanhao, systemInfo.CraftType);

                //插件管理器初始化
                Process流程插件管理者.SetPath(paramPathSet.ProcessPath, paramPathSet.InstanceParamPath);

                ReadProject(); // 加载配方后，默认添加一个初始编号为0的产品。
                SaveProject();


                baseProDevice.INIPro();
                baseProDevice.INIProduct();

                BindCamImageSource(baseProDevice.m_DicCamOperaParam);





                //加载权限管理参数
                foreach (var ModelName in Enum.GetValues(typeof(ModelName)))
                {
                    modelServiceManage.AddModel(ModelName.ToString(), new ModelControl());
                }
                paramPath = paramPathSet.purviewParamPathName;
                initParamResult &= purviewManage.InitpurviewParam(paramPath);
                purviewManage.UpdateModelPurviewParam(modelServiceManage.m_dicModels, ref purviewManage.purviewParam.l_modelPrivilege);
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
        public bool InitCamRelevancies(int index)
        {
            CameraRelevancy cameraRelevancy = Service_Camera.Get_CameraRelevancy(index);
            cameraRelevancy.InitCalibrationPlateParam(paramPathSet.CalibrationPlateParamPath + $"{index}");
            cameraRelevancy.InitCheckerboardParam(paramPathSet.CheckerboardParamPath + $"{index}");
            cameraRelevancy.InitPretargetingCalibParam(paramPathSet.PretargetingCalibParamPath + $"{index}");
            cameraRelevancy.InitImageSaveParamer(paramPathSet.ImageSaveParamPath + $"{index}");
            cameraRelevancy.InitOVHMarkParam(paramPathSet.CutOverlayMarkParamPath + $"{index}");
            cameraRelevancy.InitMsaCircleParam(paramPathSet.MsaCircleParamPath + $"{index}");
            cameraRelevancy.InitMsaLineWidthParam(paramPathSet.MsaLineWidthParamPath + $"{index}");


            cameraRelevancy.InitParam_GrayScaleAdaptationParameter(paramPathSet.GrayScaleAdaptationParamPath + $"{index}");
            cameraRelevancy.m_SizePointInspectionManage.InitParam(paramPathSet.CamSizePointInspectionParamPathName + $"{index}");
            cameraRelevancy.m_DefectPointInspectionManage.InitParam(paramPathSet.CamDefectPointInspectionParamPathName + $"{index}");
            cameraRelevancy.InitParam_PointInspectionStateParameter(paramPathSet.CamPointInspectionStateParamPathName + $"{index}");
            return true;
        }



        public bool Init3DThickManage(int thickNum)
        {
            bool result = true;
            try
            {

                thick3DManages = new Thick3DManage[thickNum];
                for (int i = 0; i < thickNum; i++)
                {
                    thick3DManages[i] = new Thick3DManage();
                    thick3DManages[i].InitParam(paramPathSet.CamParamPathName + $"3D{i + 1}");
                    result = true;
                }
            }
            catch (Exception)
            {

                result = false; ;

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
            cutOverlayPLCManage.Connect(cutOverlayPLCManage.m_DevParam.IsConnect, PLCTypeEnum.Omron, (CommunicationProtocolEnum)cutOverlayPLCManage.m_DevParam.ComType, cutOverlayPLCManage.m_DevParam.PLC_IP, cutOverlayPLCManage.m_DevParam.DevPort, (emDataFormat)cutOverlayPLCManage.m_DevParam.nByteTrans);
        }

        public void BindCamImageSource(Dictionary<int, BaseProParam> m_DicCamOperaParam)
        {
            baseProDevice.m_DicCamOperaParam = m_DicCamOperaParam;

            for (int i = 0; i < M_CamParam.l_CameraDevs.Count; i++)
            {
                //注册流程相机图像队列
                if (baseProDevice.m_DicCamOperaParam.Count > i)
                {
                    baseProDevice.m_DicCamOperaParam[i].QueueGrabImage = M_CamParam.l_CameraDevs[i].Camera.QueueGrabImage;

                    M_CamParam.l_CameraDevs[i].Camera.CamCallbackEndEvent = null;
                    M_CamParam.l_CameraDevs[i].Camera.CamCallbackEndEvent = baseProDevice.m_DicCamOperaParam[i].CallBackImgDataDeal;
                }
            }
        }
        public void RemoveBindCamImageSource()
        {
            foreach (var item in M_CameraRelevancies.Keys)
            {
                //注册流程相机图像队列                  
                // baseProDevice.m_DicCamOperaParam[(int)item].QueueGrabImage = M_CameraRelevancies[item].Camera.QueueGrabImage;
                //M_CameraRelevancies[item].Camera.CamCallbackEvent -= baseProDevice.m_DicCamOperaParam[(int)item].CallBackImgDataDeal;
                //M_CameraRelevancies[item].Camera.CamCallbackEvent += baseProDevice.m_DicCamOperaParam[(int)item].CallBackImgDataDeal;
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
                craftBase.filePath = paramPathSet.CraftPath + _Currentmaterial.GetDescription() + "\\" + _CurrentCraft;
                //craftInfo.fileFullPath = paramPathSet.craftParamPath;
                switch (_Currentmaterial)
                {

                    case MaterialCrafEnum.激光模切一体机:
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
                craftBase.fileFullPath = paramPathSet.CraftPath + _Currentmaterial.GetDescription() + "\\" + _CurrentCraft + ".json";
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
        public bool NewMujuanhao(string _currentProduct, string _currentMojuanhao, MaterialCrafEnum _materialCrafEnum)
        {
            lock (NewMujuanhao_lock)
            {
                try
                {
                    paramPathSet.UpdateParamPath(_currentProduct, _currentMojuanhao, _materialCrafEnum, m_ProcessList, systemInfo.IsHostMachine);

                    server_DiecuttingCutPLC?.SetLogPath(paramPathSet.ProductMojuanPath);
                    cutOverlayPLCManage?.SetLogPath(paramPathSet.ProductMojuanPath);

                    string strPath = paramPathSet.ProductMojuanPath;
                    Directory.CreateDirectory(strPath);

                    TJBJInfo.TJBJInfoPath = Path.Combine(strPath, "CCD停机报警信息记录.ini");
                    LogWorks.SetGrayScaleAdaptationLoggPath(Path.Combine(strPath, "GrayLog"));

                //重开膜卷初始化打标软件参数和信息路径
                //dbjCardManager.ReadLogPath = strPath + "打标软件接收记录-接收MarkingSoftwareReceiveRecord.ini";
                //dbjCardManager.SendLogPath = strPath + "打标软件发送记录-发送MarkingSoftwareSendRecord.ini";
                //dbjCardManager.SendHeartPath = strPath + "打标软件发送记录-心跳MarkingSoftwareHeartbeatRecord.ini";
                //dbjCardManager.SendDBPath = strPath + "打标软件发送记录-打标MarkingSoftwareMarkingRecord.ini";
                //dbjCardManager.SendJPPath = strPath + "打标软件发送记录-纠偏MarkingSoftwareOffsetRecord.ini";

                    ////重开膜卷初始化主副机软件参数和信息路径
                    if (slaveMachineManager != null)
                    {
                        slaveMachineManager.ReadLogPath = Path.Combine(strPath, "主副机接收记录-接收MarkingSoftwareReceiveRecord.ini");
                        slaveMachineManager.SendLogPath = Path.Combine(strPath, "主副机发送记录-发送MarkingSoftwareSendRecord.ini");
                        slaveMachineManager.SendHeartPath = Path.Combine(strPath, "主副机发送记录-心跳MarkingSoftwareHeartbeatRecord.ini");
                        slaveMachineManager.SendDBPath = Path.Combine(strPath, "主副机发送记录-打标MarkingSoftwareMarkingRecord.ini");
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    Log4netRecord.ErrorLog.Debug(ex);
                    return false;
                }
            }
        }



        #region Process流程控制
        /// <summary>
        /// 创建流程。
        /// 兼容旧的 Project.CreateProcess() 写法，内部交给 Proc 创建 ProcessServices 并生成不重复 ProcessID。
        /// </summary>
        /// <returns>新流程 ID。</returns>
        public int CreateProcess()
        {
            return Process流程插件管理者.Create();
        }
        /// <summary>
        /// 按指定 ID 和名称创建流程。
        /// 兼容旧代码入口，实际创建逻辑由 Proc 管理。
        /// </summary>
        /// <param name="ID">流程 ID。</param>
        /// <param name="Name">流程名称。</param>
        public void CreateProcess(int ID, string Name)
        {
            Process流程插件管理者.Create(ID, Name);
        }
        /// <summary>
        /// 根据 ID 获取对应的 Process。
        /// 插件中 GetResultParams 等旧代码仍可通过该入口把结果写回流程输出表。
        /// </summary>
        /// <param name="id">流程 ID。</param>
        /// <returns>对应 Process；未找到返回 null。</returns>
        public Process GetProcessById(int id)
        {
            return Process流程插件管理者.GetProcess(id);
        }
        /// <summary>
        /// 根据 ID 获取对应的 ProcessServices。
        /// BuildProcessServiceList 创建流程后会通过该入口取回服务对象并绑定初始化/运行事件。
        /// </summary>
        /// <param name="id">流程 ID。</param>
        /// <returns>对应 ProcessServices；未找到返回 null。</returns>
        public ProcessServices GetProcessServiceById(int id)
        {
            return Process流程插件管理者.GetService(id);
        }
        /// <summary>
        /// 根据流程名称获取对应的 Process。
        /// 调试设置界面按流程名定位当前流程时使用该旧入口。
        /// </summary>
        /// <param name="name">流程名称。</param>
        /// <returns>对应 Process；未找到返回 null。</returns>
        public Process GetProcessByName(string name)
        {
            return Process流程插件管理者.GetProcess(name);
        }
        /// <summary>
        /// 根据流程名称获取对应的 ProcessServices。
        /// </summary>
        /// <param name="name">流程名称。</param>
        /// <returns>对应 ProcessServices；未找到返回 null。</returns>
        public ProcessServices GetProcessServiceByName(string name)
        {
            return Process流程插件管理者.GetService(name);
        }
        /// <summary>
        /// 清空指定 Process 下的模块对象。
        /// 为兼容旧语义，该入口不从流程列表移除 ProcessServices。
        /// </summary>
        /// <param name="proc">需要清空模块的流程对象。</param>
        public void DeleteProcess(Process proc)
        {
            Process流程插件管理者.Delete(proc);
        }
        /// <summary>
        /// 删除指定 ProcessServices。
        /// 内部会先请求流程线程安全退出，再释放模块并从 Proc.List 移除。
        /// </summary>
        /// <param name="procS">需要删除的流程服务。</param>
        public void DeleteProcessService(ProcessServices procS)
        {
            Process流程插件管理者.DeleteService(procS);
        }
        /// <summary>
        /// 清空所有 ProcessServices 和 Process。
        /// 主要在重新加载流程、切换流程结构前使用；实际线程关闭和模块释放由 Proc.DeleteAll 完成。
        /// </summary>
        public void DeleteALLProcess()
        {
            Process流程插件管理者.DeleteAll();
        }

        /// <summary>
        /// 获取所有流程运行状态。
        /// </summary>
        /// <returns>任一流程正在运行返回 true。</returns>
        public bool GetStates()
        {
            return Process流程插件管理者.GetStates();
        }

        /// <summary>
        /// 启动指定流程连续运行。
        /// </summary>
        /// <param name="processID">流程 ID。</param>
        public void StartRun(int processID)
        {
            Is_Run = true;
            Process流程插件管理者.Start(processID);
        }

        /// <summary>
        /// 指定流程执行一次。
        /// </summary>
        /// <param name="processID">流程 ID。</param>
        public void ExecuteOnce(int processID)
        {
            Process流程插件管理者.ExecuteOnce(processID);
        }

        /// <summary>
        /// 停止指定流程运行。
        /// </summary>
        /// <param name="processID">流程 ID。</param>
        public void StopRun(int processID)
        {
            Is_Run = false;
            Process流程插件管理者.Stop(processID);
        }

        /// <summary>
        /// 保存当前配方流程。
        /// Project 层只负责同步当前路径并判断 Proc.Save 返回值；具体模块参数和 Instance.xml 保存由 Proc 统一处理。
        /// </summary>
        public void SaveProject()
        {
            Process流程插件管理者.SetPath(paramPathSet.ProcessPath, paramPathSet.InstanceParamPath);
            if (!Process流程插件管理者.Save())
            {
                Log4netRecord.ErrorLog.Fatal("配方的检测流程保存失败:" + Process流程插件管理者.LastError);
            }
        }

        /// <summary>
        /// 读取当前配方流程。
        /// 先初始化业务设备基础数据，再把 baseProDevice.BuildProcessServiceList 作为标准流程构建动作传给 Proc.Load；
        /// Proc 内部完成清理旧流程、构建标准流程、读取 Instance.xml、自检合并、加载插件参数和读后自动保存。
        /// </summary>
        /// <returns>流程加载成功返回 true；失败时写 Fatal 并返回 false。</returns>
        public bool ReadProject()
        {
            try
            {
                baseProDevice.INIPro();

                bool ok = Process流程插件管理者.Load(paramPathSet.ProcessPath, paramPathSet.InstanceParamPath, baseProDevice.BuildProcessServiceList);
                if (!ok)
                {
                    Log4netRecord.ErrorLog.Fatal("配方的检测流程加载失败:" + Process流程插件管理者.LastError);
                }

                return ok;
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

using LogTool;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    public class ParamPathSet
    {
        #region 基础路径
        /// <summary>
        /// 软件工作路径
        /// </summary>
        public readonly string softwareWorkPath = $"{System.Environment.CurrentDirectory}\\";
        /// <summary>
        /// //系统配置文件勿动,存放硬件配置参数，系统设置参数。
        /// </summary>
        public readonly string SystemPath = $"{System.Environment.CurrentDirectory}\\ZSystemConfigurationFile\\";
        /// <summary>
        /// /配方文件,存放配方工艺，配方流程，配方模板图。
        /// </summary>
        public readonly string CraftPath = System.Environment.CurrentDirectory + "\\ZFormulaFile\\";

        //点检记录
        public readonly string pointCheckRootPath = "E:\\点检数据PointInspectionData\\";

        public readonly string SizeCheckPointDataName = "尺寸点检数据SizeCheckPointData.csv";
       
        public readonly string CheckPointRecordPath = "E:\\点检数据PointInspectionData\\CheckPoint.ini";      //存放点检数据


        /// <summary>
        /// 初始生产数据路径
        /// </summary>
        public string initProdDataPath = $"E:\\生产数据\\";

        #endregion

        #region 运行路径
        //系统参数
        public string SystemSetPathName;//系统参数
        public string purviewParamPathName;//权限参数
        public string UserParamPathName;//用户管理参数  
        public string CamParamPathName;//相机配置文件保存路径
        public string CamCalibParamPathName;//相机标定参数
        public string CamSizeCheckPointParamPathName;//相机尺寸点检参数
        public string CamDistParamPathName;//相机距离参数      
        public string LightParamPathName;//光源参数
        public string DiverParamPathName;//磁盘参数
        public string DevtParamPathName;//主设备参数
        public string DevtParamPathName_Pocket;//主设备参数
        public string DBJParamPathName;//打标机参数
        public string JPParamPathName;//纠偏参数
        public string BaseSetParamPathName;//基础设置参数保存路径名称
        public string EmulationParamPath;//仿真参数路径
        public string SystemSetParamPath;//系统设置参数路径
        public string LogParamPath;//日志参数路径
        public string MainSlaveMachineParamePath;//日主副机参数路径
        public string AutoDeleteFileManageParamePath;//自动删除文件路径
        public string MESParamPath;//Mes参数路径
        public string ControlCardParamPath;//控制卡参数路径
        public string CalibrationPlateParamPath;//标定板参数路径
        public string CheckerboardParamPath;//棋盘格参数路径
        public string PretargetingCalibParamPath;//预定位标定参数路径
        public string ImageSaveParamPath;//图像保存参数路径
        public string CutOverlayPLCParamPath;//切叠主设备参数
        public string CutOverlayMarkParamPath;//切叠Mark参数

        public string MsaCircleParamPath;//Msa参数路径
        public string MsaLineWidthParamPath;//Msa线宽参数路径
        public string MsaLineCheckBoradParamPath;//Msa棋盘格线宽参数路径
        public string Msa3DThickParamPath;//Msa3D测厚参数路径
        public string CheckDJParamPath;//点检参数路径
        public string CutOverlayProjectBasePath;//切叠项目基础路径
        //配方参数
        public string ProcessPath;       //运行流程插件参数路径  
        public string InstanceParamPath; //运行流程结构参数路径
        public string craftParamPath; //工艺参数路径


        //生产数据
        public string ProdStatisticsDataPath = "E:\\生产数据\\生产统计数据";
        public string ProdSizeDataPath = "E:\\生产数据\\生产数据";
        public string ProdSizeDataPath_ZBMCCam1 = "E:\\生产数据\\生产数据";
        public string ProdSizeDataPath_JRMCCam1 = "E:\\生产数据\\生产数据";
        public string ProdSizeDataPath_ZBMCCam2 = "E:\\生产数据\\生产数据";
        public string ProdSizeDataPath_JRMCCam2 = "E:\\生产数据\\生产数据";
        public string ProdMeterDataPath = "E:\\生产数据\\生产米数";
        public string ProdSizeDataPath_DuanMCam1 = "E:\\生产数据\\生产数据";
        public string ProdSizeDataPath_DaMCam1 = "E:\\生产数据\\生产数据";
        public string ProdSizeDataPath_Statistics = "E:\\生产数据\\生产统计";

        public string ProdSizeDataPath_Cam1 = "E:\\生产数据\\生产数据";
        public string ProdSizeDataPath_Cam2 = "E:\\生产数据\\生产数据";
        public string ProdSizeDataPath_Cam3 = "E:\\生产数据\\生产数据";
        public string ProdSizeDataPath_Cam4 = "E:\\生产数据\\生产数据";

        public string DBDataPath = "E:\\打标数据统计\\";

        public string CamCallbackPathName = "E:\\生产数据\\海康相机记录日志.ini";  //运行调试日志路径名称
        public string RunLogDebugPathName = "E:\\生产数据\\RunDebugLog.ini";  //运行调试日志路径名称      
        public string ProductRootPath = "E:\\生产数据\\"; //存放膜卷号数据路径
        public string ProductMojuanPath = "E:\\生产数据\\"; //生产膜卷路径
        public string ProductMojuanMESPath = "E:\\生产数据\\"; //生产膜卷路径
        public string MesRootPath = "E:\\MES文件\\";      //存放MES文件路径。界面可修改
        public string ProdInfoPath = "E:\\生产数据\\产品信息";
        public string AreaDataPath = "E:\\生产数据\\面阵相机检测数据";
        public string Cam1_DetectionTimePath = "E:\\生产数据\\图像处理耗时相机1";
        public string Cam2_DetectionTimePath = "E:\\生产数据\\图像处理耗时相机2";
        public string Cam3_DetectionTimePath = "E:\\生产数据\\图像处理耗时相机3";
        public string Cam4_DetectionTimePath = "E:\\生产数据\\图像处理耗时相机4";

        public string LogPath_DB1 = "E:\\生产数据\\打标日志记录1.ini";
        public string LogPath_DB2 = "E:\\生产数据\\打标日志记录2.ini";
        public string LogPath_DB3 = "E:\\生产数据\\打标日志记录3.ini";
        public string LogPath_DB4 = "E:\\生产数据\\打标日志记录4.ini";

        public string LogPath_Correct = "E:\\生产数据\\纠偏日志记录.ini";

        //点检数据
        public string CamSizeCheckPointPath = $"E:\\点检数据\\{DateTime.Now.ToString("yyyy.MM.dd")}\\";      //存放点检数据

        //赞不使用
        public string LogRootPath = "E:\\日志文件\\";     //存放系统日志，修改记录日志，软件报错日志路径。界面可修改


        #region 切叠项目路径
        public string OVHDataScreenshotRelativePath = "E:\\生产数据\\生产数据";

        public string OVHProdDataPath_Cam1 = "E:\\生产数据\\生产数据";
        public string OVHProdDataPath_Cam2 = "E:\\生产数据\\生产数据";
        public string OVHProdDataPath_Cam3 = "E:\\生产数据\\生产数据";
        public string OVHProdDataPath_Cam4 = "E:\\生产数据\\生产数据";
        public string OVHProdDataPath_Cam5 = "E:\\生产数据\\生产数据";
        public string OVHProdDataPath_Cam6 = "E:\\生产数据\\生产数据";
        public string OVHProdDataPath_Cam7 = "E:\\生产数据\\生产数据";
        public string OVHProdDataPath_Cam8 = "E:\\生产数据\\生产数据";

        public string OVHProdDataSizeDiaphragmLogPath = "E:\\生产数据\\尺寸日志";
        public string OVHProdDataSizeLogPath = "E:\\生产数据\\尺寸日志";
        public string OVHProdDataSizeMonitorLogPath = "E:\\生产数据\\尺寸日志";
        public string OVHProdDataPath_AllACCam = "E:\\生产数据\\AC生产数据";
        public string OVHProdDataPath_AllCACam = "E:\\生产数据\\CA生产数据";
        public string OVHProdDataPath_AllAACam = "E:\\生产数据\\AA生产数据";
        public string OVHProdDataPath_AllCCCam = "E:\\生产数据\\CC生产数据";
        public string OVHProdDataPath_AllCam = "E:\\生产数据\\CA生产数据";
        public string OVHProdDataPath_PieSize = "E:\\生产数据\\CA生产数据";
        public string OVHProdNGDataPath_AllCam = "E:\\生产数据\\CA生产数据";

        public string PretargetingProdDataPath = "E:\\生产数据\\生产数据";
        public string PretargetingProdSizeDataPath = "E:\\生产数据\\生产数据";

        //模切工位读取PLC纠偏记录日志
        public string DieCutCorrectOffsetValueLogPath = "E:\\生产数据\\生产数据";

        public string PretargetingCalibrationPath = "E:\\标定\\";



        #endregion


        #region 窄幅清洗机路径
        public string PFTProdSizeDataPath_Cam1 = "E:\\生产数据\\生产尺寸数据";
        public string PFTProdSizeDataPath_Cam2 = "E:\\生产数据\\生产尺寸数据";
        public string PFTProdSizeDataPath_Cam3 = "E:\\生产数据\\生产尺寸数据";
        public string PFTProdSizeDataPath_Cam4 = "E:\\生产数据\\生产尺寸数据";

        public string PFTProdSizeDataLinePath = "E:\\生产数据\\线扫尺寸日志";


        public string PFTProdDeftDataPath_Cam1 = "E:\\生产数据\\生产缺项数据";
        public string PFTProdDeftDataPath_Cam2 = "E:\\生产数据\\生产缺项数据";
        public string PFTProdDeftDataPath_Cam3 = "E:\\生产数据\\生产缺项数据";
        public string PFTProdDeftDataPath_Cam4 = "E:\\生产数据\\生产缺陷数据";


        public string PFTProdEADataPath_A = "E:\\生产数据\\PFT_A面EA生产数据";
        public string PFTProdEADataPath_B = "E:\\生产数据\\PFT_B面EA生产数据";
        public string PFTProdEADataPath_AB = "E:\\生产数据\\PFT_AB面EA生产数据";

        #endregion

        #endregion

        /// <summary>
        /// 初始化系统文件路径
        /// </summary>
        /// <param name="camNum"></param>
        /// <param name="lightNum"></param>
        /// <param name="uniteCalibNum"></param>
        public void InitPath()
        {
            //系统配置文件路径
            SystemSetPathName = SystemPath + "systemparams";

        }
        public void InitRelativeParamPath(string relativePath)
        {
            CutOverlayProjectBasePath = SystemPath + $"\\{relativePath}";

            //系统配置文件路径    
            purviewParamPathName = SystemPath + $"\\{relativePath}" + "\\PurviewParam";
            UserParamPathName = SystemPath + $"\\{relativePath}" + "\\UserParam";

            //日志参数路径
            LogParamPath = SystemPath + $"\\{relativePath}" + "\\LogParam";

            //相机配置参数路径
            CamParamPathName = SystemPath + $"\\{relativePath}" + "\\CameraParam";

            //相机标定参数路径
            CamCalibParamPathName = SystemPath + $"\\{relativePath}" + "\\CameraCalibParam";

            //相机尺寸点检参数路径
            CamSizeCheckPointParamPathName = SystemPath + $"\\{relativePath}" + "\\CamSizeCheckPointParam";

            //相机距离参数路径
            CamDistParamPathName = SystemPath + $"\\{relativePath}" + "\\CameraDistParam";

            //基础参数保存路径
            BaseSetParamPathName = SystemPath + $"\\{relativePath}" + "\\BaseSetParam";

            //仿真参数保存路径
            EmulationParamPath = SystemPath + $"\\{relativePath}" + "\\EmulationParam";

            //系统设置参数
            SystemSetParamPath = SystemPath + $"\\{relativePath}" + "\\SystemSetParam";

            //光源参数保存路径名称
            LightParamPathName = SystemPath + $"\\{relativePath}" + "\\LightParam";

            //磁盘参数保存路径名称
            DiverParamPathName = SystemPath + $"\\{relativePath}" + "\\DiverParam";

            //主副机参数保存路径名称
            MainSlaveMachineParamePath = SystemPath + $"\\{relativePath}" + "\\MainSlaveMachineParame";

            //自动删除文件参数路径
            AutoDeleteFileManageParamePath = SystemPath + $"\\{relativePath}" + "\\AutoDeleteFileManageParame";

            //MES参数路径
            MESParamPath = SystemPath + $"\\{relativePath}" + "\\MESParam";

            //控制卡参数路径
            ControlCardParamPath = SystemPath + $"\\{relativePath}" + "\\ControlCardParam";

            DevtParamPathName = SystemPath + $"\\{relativePath}" + "\\mDevPLCParam";
            DevtParamPathName_Pocket = SystemPath + $"\\{relativePath}" + "\\mDevPLCParamPocket";
            DBJParamPathName = SystemPath + $"\\{relativePath}" + "\\DBJPLCParam";
            JPParamPathName = SystemPath + $"\\{relativePath}" + "\\JPParam";

            //图像保存参数路径
            ImageSaveParamPath = SystemPath + $"\\{relativePath}" + "\\ImageSaveParam";

            //切叠主设备参数保存路径
            CutOverlayPLCParamPath = SystemPath + $"\\{relativePath}" + "\\CutOverlayPLCParam";

            //切叠Mark参数保存路径
            CutOverlayMarkParamPath = SystemPath + $"\\{relativePath}" + "\\CutOverlayMarkParam";

            CalibrationPlateParamPath = SystemPath + $"\\{relativePath}" + "\\CalibrationPlateParam";
            CheckerboardParamPath = SystemPath + $"\\{relativePath}" + "\\CheckerboardParam";
            PretargetingCalibParamPath = SystemPath + $"\\{relativePath}" + "\\PretargetingCalibParam";


            MsaCircleParamPath = SystemPath + $"\\{relativePath}" + "\\MsaCircleParamPath";
            MsaLineWidthParamPath = SystemPath + $"\\{relativePath}" + "\\MsaLineWidthParamPath";
            MsaLineCheckBoradParamPath = SystemPath + $"\\{relativePath}" + "\\MsaLineCheckBoradParamPath";
            Msa3DThickParamPath = SystemPath + $"\\{relativePath}" + "\\Msa3DThickParamPath";
            CheckDJParamPath = SystemPath + $"\\{relativePath}" + "\\CheckDJParamPath";


        }
        public void UpdateCraftPath(string _currentCraft, string _currentMojuanhao, MaterialCrafEnum _CurrentMaterial)
        {
            //配方路径
            ProcessPath = CraftPath + _CurrentMaterial.ToString() + "\\" + _currentCraft + "\\Process"; ;
            InstanceParamPath = ProcessPath + "\\Instance.xml";
            craftParamPath = CraftPath + _CurrentMaterial.ToString() + "\\" + _currentCraft + ".json";
            //craftParamPath = CraftPath + _CurrentMaterial.ToString() + "\\" + _currentCraft + ".cfts";
            if (!Directory.Exists(ProcessPath))
                Directory.CreateDirectory(ProcessPath);
        }
        /// <summary>
        /// 刷新生产数据路径
        /// </summary>
        /// <param name="_currentProduct"></param>
        /// <param name="_currentMojuanhao"></param>
        public void UpdateParamPath(string _currentProduct, string _currentMojuanhao, MaterialCrafEnum _CurrentMaterial, List<ProcessServices> mListprocessServices, bool isMainMachine)
        {
            /// <summary>
            /// 生产日期
            /// </summary>
            string productionDate = DateTime.Now.ToString("yyyy.MM.dd");

            //配方路径
            UpdateCraftPath(_currentProduct, _currentMojuanhao, _CurrentMaterial);

            //生产数据路径
            string mainSlaveMachineName = "MainMachine";
            if (!isMainMachine) mainSlaveMachineName = "SlaveMachine";

            string proDataPathStr = initProdDataPath + $"{productionDate}{mainSlaveMachineName}" + "\\" + _CurrentMaterial.ToString() + "\\" + _currentProduct + "\\";
            ProductRootPath = proDataPathStr;
            ProductMojuanPath = proDataPathStr + _currentMojuanhao + "\\";
            ProductMojuanMESPath = proDataPathStr + _currentMojuanhao;

            CamCallbackPathName = ProductMojuanPath + $"{_currentMojuanhao}-海康相机记录日志.ini";
            RunLogDebugPathName = ProductMojuanPath + $"{_currentMojuanhao}-RunDebugLog.ini";

            ProdStatisticsDataPath = ProductMojuanPath + $"{_currentMojuanhao}-生产统计数据.csv";
            ProdSizeDataPath = ProductMojuanPath + $"{_currentMojuanhao}-生产数据.csv";
            ProdSizeDataPath_ZBMCCam1 = ProductMojuanPath + $"{_currentMojuanhao}-直边毛刺1-生产数据.csv";
            ProdSizeDataPath_JRMCCam1 = ProductMojuanPath + $"{_currentMojuanhao}-极耳毛刺1-生产数据.csv";
            ProdSizeDataPath_ZBMCCam2 = ProductMojuanPath + $"{_currentMojuanhao}-直边毛刺2-生产数据.csv";
            ProdSizeDataPath_JRMCCam2 = ProductMojuanPath + $"{_currentMojuanhao}-极耳毛刺2-生产数据.csv";
            ProdSizeDataPath_DuanMCam1 = ProductMojuanPath + $"{_currentMojuanhao}-端面-生产数据.csv";
            ProdSizeDataPath_DaMCam1 = ProductMojuanPath + $"{_currentMojuanhao}-大面-生产数据.csv";
            ProdSizeDataPath_Statistics = ProductMojuanPath + $"{_currentMojuanhao}-生产统计.csv";

            if (Project.Instance.cameraManager != null)
            {
                if (Project.Instance.cameraManager.ContainsKey(CameraEnum.Cam1))
                {
                    string camName = Project.Instance.cameraManager[CameraEnum.Cam1].m_CamParam.CameraName;
                    ProdSizeDataPath_Cam1 = ProductMojuanPath + $"{_currentMojuanhao}-{camName}-生产数据.csv";
                    LogPath_DB1 = ProductMojuanPath + $"{camName}-打标日志记录.ini";
                }
                if (Project.Instance.cameraManager.ContainsKey(CameraEnum.Cam2))
                {
                    string camName = Project.Instance.cameraManager[CameraEnum.Cam2].m_CamParam.CameraName;
                    ProdSizeDataPath_Cam2 = ProductMojuanPath + $"{_currentMojuanhao}-{camName}-生产数据.csv";
                    LogPath_DB2 = ProductMojuanPath + $"{camName}-打标日志记录.ini";
                }
                if (Project.Instance.cameraManager.ContainsKey(CameraEnum.Cam3))
                {
                    string camName = Project.Instance.cameraManager[CameraEnum.Cam3].m_CamParam.CameraName;
                    ProdSizeDataPath_Cam3 = ProductMojuanPath + $"{_currentMojuanhao}-{camName}-生产数据.csv";
                    LogPath_DB3 = ProductMojuanPath + $"{camName}-打标日志记录.ini";
                }
                if (Project.Instance.cameraManager.ContainsKey(CameraEnum.Cam4))
                {
                    string camName = Project.Instance.cameraManager[CameraEnum.Cam4].m_CamParam.CameraName;
                    ProdSizeDataPath_Cam4 = ProductMojuanPath + $"{_currentMojuanhao}-{camName}-生产数据.csv";
                    LogPath_DB4 = ProductMojuanPath + $"{camName}-打标日志记录.ini";
                }

                //生产数据图片保存路径
                foreach (var item in Project.Instance.cameraManager)
                {
                    item.Value.m_ImageSaveParamer.ImageSave_Path_Primitive = $"\\{item.Value.m_CamParam.CameraName}\\" + "原图";
                    item.Value.m_ImageSaveParamer.ImageSave_Path_Screenshot = $"\\{item.Value.m_CamParam.CameraName}\\" + "截图";
                    item.Value.m_ImageSaveParamer.ImageSave_Path_Min = $"\\{item.Value.m_CamParam.CameraName}\\" + "NG小图";
                    item.Value.m_ImageSaveParamer.ImageSave_Path_ProdImg = $"\\{item.Value.m_CamParam.CameraName}\\" + "产品图";
                    item.Value.m_ImageSaveParamer.ImageSave_Path_ProdScreenshot = $"\\{item.Value.m_CamParam.CameraName}\\" + "产品截图";

                    item.Value.m_CamDetectionVariable.ProdDataPath = ProductMojuanPath + $"{_currentMojuanhao}-{item.Value.m_CamParam.CameraName}-生产数据.csv";

                    item.Value.m_CamDetectionVariable.ProdSizeDataPath = ProductMojuanPath + $"{_currentMojuanhao}-{item.Value.m_CamParam.CameraName}-尺寸过程数据.csv";
                    item.Value.m_CamDetectionVariable.ProdDeftDataPath = ProductMojuanPath + $"{_currentMojuanhao}-{item.Value.m_CamParam.CameraName}-缺项过程数据.csv";
                    item.Value.m_CamDetectionVariable.ProdEADataPath = ProductMojuanPath + $"{_currentMojuanhao}-{item.Value.m_CamParam.CameraName}-EA过程数据.csv";
                }
            }


            OVHDataScreenshotRelativePath = "\\OVH数据NG截图\\";

            OVHProdDataPath_Cam1 = ProductMojuanPath + $"{_currentMojuanhao}-主相机1-生产数据.csv";
            OVHProdDataPath_Cam2 = ProductMojuanPath + $"{_currentMojuanhao}-主相机2-生产数据.csv";
            OVHProdDataPath_Cam3 = ProductMojuanPath + $"{_currentMojuanhao}-主相机3-生产数据.csv";
            OVHProdDataPath_Cam4 = ProductMojuanPath + $"{_currentMojuanhao}-主相机4-生产数据.csv";
            OVHProdDataPath_Cam5 = ProductMojuanPath + $"{_currentMojuanhao}-从相机5-生产数据.csv";
            OVHProdDataPath_Cam6 = ProductMojuanPath + $"{_currentMojuanhao}-从相机6-生产数据.csv";
            OVHProdDataPath_Cam7 = ProductMojuanPath + $"{_currentMojuanhao}-从相机7-生产数据.csv";
            OVHProdDataPath_Cam8 = ProductMojuanPath + $"{_currentMojuanhao}-从相机8-生产数据.csv";
            OVHProdDataPath_AllACCam = ProductMojuanPath + $"{_currentMojuanhao}-汇总相机数据AC-生产数据.csv";
            OVHProdDataPath_AllCACam = ProductMojuanPath + $"{_currentMojuanhao}-汇总相机数据CA-生产数据.csv";
            OVHProdDataPath_AllAACam = ProductMojuanPath + $"{_currentMojuanhao}-汇总相机数据AA-生产数据.csv";
            OVHProdDataPath_AllCCCam = ProductMojuanPath + $"{_currentMojuanhao}-汇总相机数据CC-生产数据.csv";
            OVHProdDataPath_AllCam = ProductMojuanPath + $"{_currentMojuanhao}-Overhang电芯数据.csv";
            OVHProdDataPath_PieSize = ProductMojuanPath + $"{_currentMojuanhao}-Overhang电芯尺寸数据.csv";
            OVHProdNGDataPath_AllCam = ProductMojuanPath + $"{_currentMojuanhao}-Overhang电芯NG数据.csv";
            OVHProdDataSizeDiaphragmLogPath = ProductMojuanPath + $"{_currentMojuanhao}-隔膜尺寸日志.csv";
            OVHProdDataSizeLogPath = ProductMojuanPath + $"{_currentMojuanhao}-AC和CA尺寸日志.csv";
            OVHProdDataSizeMonitorLogPath = ProductMojuanPath + $"{_currentMojuanhao}-AA和CC尺寸日志.csv";

            PretargetingProdDataPath = ProductMojuanPath + $"{_currentMojuanhao}-预定位-生产数据.csv";
            PretargetingProdSizeDataPath = ProductMojuanPath + $"{_currentMojuanhao}-预定位-尺寸数据.csv";


            PFTProdSizeDataPath_Cam1 = ProductMojuanPath + $"{_currentMojuanhao}-相机1-PFT_生产过程尺寸数据.csv" ;
            PFTProdSizeDataPath_Cam2 = ProductMojuanPath + $"{_currentMojuanhao}-相机2-PFT_生产过程尺寸数据.csv";
            PFTProdSizeDataPath_Cam3 = ProductMojuanPath + $"{_currentMojuanhao}-相机3-PFT_生产过程尺寸数据.csv";
            PFTProdSizeDataPath_Cam4 = ProductMojuanPath + $"{_currentMojuanhao}-相机4-PFT_生产过程尺寸数据.csv";

            PFTProdSizeDataLinePath = ProductMojuanPath + $"{_currentMojuanhao}-PFT_生产过程尺寸日志.csv";

            PFTProdDeftDataPath_Cam1 = ProductMojuanPath + $"{_currentMojuanhao}-相机1-PFT_生产缺项数据.csv";
            PFTProdDeftDataPath_Cam2 = ProductMojuanPath + $"{_currentMojuanhao}-相机2-PFT_生产缺项数据.csv";
            PFTProdDeftDataPath_Cam3 = ProductMojuanPath + $"{_currentMojuanhao}-相机3-PFT_生产缺项数据.csv";
            PFTProdDeftDataPath_Cam4 = ProductMojuanPath + $"{_currentMojuanhao}-相机4-PFT_生产缺项数据.csv";


            PFTProdEADataPath_A = ProductMojuanPath + $"{_currentMojuanhao}-PFT_A面EA生产数据.csv";
            PFTProdEADataPath_B = ProductMojuanPath + $"{_currentMojuanhao}-PFT_B面EA生产数据.csv";
            PFTProdEADataPath_AB = ProductMojuanPath + $"{_currentMojuanhao}-PFT_AB面EA生产数据.csv";


            //模切工位读取PLC纠偏记录日志
            DieCutCorrectOffsetValueLogPath = ProductMojuanPath + $"{_currentMojuanhao}-读取PLC纠偏量.csv";


            ProdMeterDataPath = ProductMojuanPath + $"{_currentMojuanhao}-生产米数.csv";
            DBDataPath = ProductMojuanPath + $"{_currentMojuanhao}-打标信息统计.csv";
            ProdInfoPath = ProductMojuanPath + $"{_currentMojuanhao}-产品信息.ini";
            AreaDataPath = ProductMojuanPath + $"{_currentMojuanhao}-面阵相机检测数据.csv";
            Cam1_DetectionTimePath = ProductMojuanPath + $"{_currentMojuanhao}-图像处理耗时相机1.csv";
            Cam2_DetectionTimePath = ProductMojuanPath + $"{_currentMojuanhao}-图像处理耗时相机2.csv";
            Cam3_DetectionTimePath = ProductMojuanPath + $"{_currentMojuanhao}-图像处理耗时相机3.csv";
            Cam4_DetectionTimePath = ProductMojuanPath + $"{_currentMojuanhao}-图像处理耗时相机4.csv";
            LogPath_Correct = ProductMojuanPath + $"纠偏日志记录.ini";

            if (!Directory.Exists(PretargetingCalibrationPath))
                Directory.CreateDirectory(PretargetingCalibrationPath);

            if (!Directory.Exists(ProductMojuanPath))
                Directory.CreateDirectory(ProductMojuanPath);



            //点检数据路径
            CamSizeCheckPointPath = "E:\\点检数据\\" + productionDate + "\\";
            if (!Directory.Exists(CamSizeCheckPointPath))
                Directory.CreateDirectory(CamSizeCheckPointPath);

            Log4netRecord.ChangeProdLogPath(ProductMojuanPath);//初始化相机回调日志路径


        }
    }
}

using Basler.Pylon;

using EHolly;

using Frame_Core;

using HalconDotNet;

using LogTool;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Frame_Core
{
    public class CameraManager
    {

        public CameraManager()
        {

        }
        /// <summary>
        /// 当前图像
        /// </summary>
        public HObject currentImage = null;

        /// <summary>
        /// 相机枚举
        /// </summary>
        public CameraEnum CameraNameEnum = CameraEnum.Cam1;

        /// <summary>
        /// 相机临时数据
        /// </summary>
        public TempData m_TempData = new TempData();

        ///// <summary>
        ///// 图片保存参数
        ///// </summary>
        //public ImageSaveParamer m_ImageSaveParamer = new ImageSaveParamer();

        /// <summary>
        /// 保存图片控制类
        /// </summary>
        public SaveImageControl m_SaveImageControl = new SaveImageControl();

        /// <summary>
        /// 图片仿真路径数据
        /// </summary>
        public ImageSimManage m_ImageSimManage = new ImageSimManage();

        /// <summary>
        /// 相机标定板参数
        /// </summary>
        public Param_CalibrationPlate param_CalibrationPlate = new Param_CalibrationPlate();


        /// <summary>
        /// Mark设置参数
        /// </summary>
        public Param_MarkSet param_MarkSet = new Param_MarkSet();

        /// <summary>
        /// 棋盘格参数
        /// </summary>
        public Param_Checkerboard param_Checkerboard = new Param_Checkerboard();

        /// <summary>
        /// 预定位标定参数（只标定相机到平台坐标系） 
        /// </summary>
        public Param_PretargetingCalib param_PretargetingCalib = new Param_PretargetingCalib();

        /// <summary>
        /// Msa圆参数
        /// </summary>
        public Param_MsaCircle param_MsaCircle = new Param_MsaCircle();


        /// <summary>
        /// Msa3D测厚检测
        /// </summary>
        public Param_MsaThick3D  param_MsaThick3D = new Param_MsaThick3D();

        /// <summary>
        /// Msa线宽参数
        /// </summary>
        public Param_MsaLineWidth param_MsaLineWidth = new Param_MsaLineWidth();

        /// <summary>
        /// 灰度自适应参数
        /// </summary>
        public GrayScaleAdaptationParameter m_GrayScaleAdaptationParameter = new GrayScaleAdaptationParameter();


        /// <summary>
        /// 尺寸点检管理
        /// </summary>
        public SizePointInspectionManage m_SizePointInspectionManage = new SizePointInspectionManage();

        /// <summary>
        /// 缺陷检测管理
        /// </summary>
        public DefectPointInspectionManage m_DefectPointInspectionManage = new DefectPointInspectionManage();

        /// <summary>
        /// 点检状态参数
        /// </summary>
        public PointInspectionStateParam m_PointInspectionStateParam = new PointInspectionStateParam();


        /// <summary>
        /// 尺寸点检算法
        /// </summary>
        public SizeCheckPointAlgorithm sizeCheckPointAlgorithm = new SizeCheckPointAlgorithm();

        /// <summary>
        /// 相机检测变量
        /// </summary>
        public CamDetectionVariable m_CamDetectionVariable = new CamDetectionVariable();

        public CamDetectionVariable m_CamDetectionACAllVariable = new CamDetectionVariable();
        public CamDetectionVariable m_CamDetectionCAAllVariable = new CamDetectionVariable();



        /// <summary>
        /// 尺寸点检数据保存路径
        /// </summary>
        public string SizeDJDataPathName = "";

        /// <summary>
        /// 相机参数
        /// </summary>
        public CameraParameter m_CamParam;
        /// <summary>
        /// 相机对象
        /// </summary>
        public CameraObj Camera;
        public HWindow hWindow;
        public FileSaveImage SaveImage = new FileSaveImage();//保存相机图片
        public FileOperationCSV SaveCSV = new FileOperationCSV();//保存相机检测数据
        /// <summary>
        /// 初始化状态
        /// </summary>
        public bool initState;

        /// <summary>
        /// 初始化相机参数
        /// </summary>
        /// <param name="path"></param>
        /// <param name="isCopy"></param>
        /// <returns></returns>
        public bool InitCameraParam(string path, Boolean isCopy = false)
        {
            try
            {
                if (!isCopy)
                {
                    if (m_CamParam == null) m_CamParam = new CameraParameter();
                    m_CamParam.filePath = path;
                    m_CamParam.Init();
                    m_CamParam = (CameraParameter)m_CamParam.Read();
                    m_CamParam.filePath = path;

                    //读取图片仿真路径数据
                    m_ImageSimManage.ReadImageSimPath(m_CamParam.EmulationPath);
                }
                else
                {
                    if (m_CamParam == null) m_CamParam = new CameraParameter();
                    m_CamParam.filePath = path;
                    m_CamParam.Save();
                }
            }
            catch (Exception ex)
            {

                Log4netRecord.ErrorLog.Debug(ex);
                return false;
            }
            return true;

        }
        /// <summary>
        /// 初始化相机
        /// </summary>
        /// <returns></returns>
        public bool IniCameraSystem(int camId)
        {
            bool bool_CamIni = false;
            try
            {

                if (initState) Close();

                initState = false;
                Thread.Sleep(200);
                Camera = GetCamera(m_CamParam.CameraType, m_CamParam.CameraSerial, m_CamParam.PCIP, m_CamParam.CameraIP);
                Camera.CamID = camId;
                bool_CamIni = Camera.InitCam();
                bool_CamIni = Camera.OpenCam() && bool_CamIni;

                initState = bool_CamIni;
                //amera.StartGrabImage();
            }
            catch (Exception ex)
            {

                initState = false;
                Log4netRecord.ErrorLog.Debug(ex);
            }

            return initState;
        }
        /// <summary>
        /// 关闭相机
        /// </summary>
        /// <returns></returns>
        public bool Close()
        {
            try
            {
                if (Camera != null)
                {
                    Camera.DestroyObjects2();
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
                return false;
            }
            return true;
        }


        ///// <summary>
        ///// 初始化图片保存参数
        ///// </summary>
        ///// <returns></returns>
        //public bool InitImageSaveParamer(string path)
        //{
        //    try
        //    {
        //        if (m_ImageSaveParamer == null) m_ImageSaveParamer = new ImageSaveParamer();
        //        m_ImageSaveParamer.filePath = path;
        //        m_ImageSaveParamer = (ImageSaveParamer)m_ImageSaveParamer.Read();
        //        m_ImageSaveParamer.filePath = path;


        //    }
        //    catch (Exception ex)
        //    {
        //        Log4netRecord.ErrorLog.Debug(ex);
        //        return false;
        //    }
        //    return true;
        //}

        /// <summary>
        /// 根据输入类型返回对应相应相机类
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private CameraObj GetCamera(string mane, string CameraSerial, string PC_IP, string Camera_IP)
        {
            if (mane == "灰点相机")
                return new CameraFly(CameraSerial);
            else if (mane == "海康相机")
                return new CameraHK(CameraSerial, PC_IP, Camera_IP);
            else if (mane == "海康彩色相机")
                return new CameraHKRGB(CameraSerial, PC_IP, Camera_IP);
            else if (mane == "DALSCameraLink相机")
                return new CameraDalsaCameraLink(CameraSerial, Camera_IP, PC_IP);
            else if (mane == "DALSCameraLink2相机")
                return new CameraDalsaCameraLink2(CameraSerial, Camera_IP, PC_IP);
            else if (mane == "DALSGigEVision相机")
                return new CameraDalsaGigEVision(CameraSerial, Camera_IP, PC_IP);
            else if (mane == "埃科(采集卡或黑白相机)")
                return new IKBoardSDK(CameraSerial, Camera_IP, PC_IP, false);
            else if (mane == "埃科(采集卡或彩色相机)")
                return new IKBoardSDK(CameraSerial, Camera_IP, PC_IP, true);
            else if (mane == "Basler(面阵)")
                return new CameraBasler(CameraSerial);
            else if (mane.Contains("埃科CXP黑白相机"))
                return new IKCXPBoard(CameraSerial, Camera_IP, false);
            else if (mane.Contains("埃科CXP彩色相机"))
                return new IKCXPBoard(CameraSerial, Camera_IP, true);
            else if (mane.Contains("海康U口黑白相机"))
                return new CameraHIK(CameraSerial, CameraType.USB3Mono);
            else if (mane.Contains("海康U口彩色相机"))
                return new CameraHIK(CameraSerial, CameraType.USB3Color);
            else if (mane.Contains("LMI"))
                return new CameraLMI(CameraSerial, PC_IP, Camera_IP);

            else
                System.Windows.Forms.MessageBox.Show(string.Format("没找到{0}相机", mane));
            return null;
        }

        /// <summary>
        /// 初始化标定板参数
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool InitCalibrationPlateParam(string path)
        {
            try
            {
                if (param_CalibrationPlate == null) param_CalibrationPlate = new Param_CalibrationPlate();
                param_CalibrationPlate.filePath = path;

                param_CalibrationPlate = (Param_CalibrationPlate)param_CalibrationPlate.Read();
                param_CalibrationPlate.filePath = path;
                param_CalibrationPlate.cameraParampath = Project.Instance.paramPathSet.CutOverlayProjectBasePath + $"\\{CameraNameEnum}hv_CamParam_data.tup";
                param_CalibrationPlate.worldPosepath = Project.Instance.paramPathSet.CutOverlayProjectBasePath + $"\\{CameraNameEnum}hv_CamPose_data.tup";

                param_CalibrationPlate.ReadCalibParam();


            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
                return false;
            }
            return true;


        }

        /// <summary>
        /// 初始化棋盘格参数
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool InitCheckerboardParam(string path)
        {
            try
            {
                if (param_Checkerboard == null) param_Checkerboard = new Param_Checkerboard();
                param_Checkerboard.filePath = path;

                param_Checkerboard = (Param_Checkerboard)param_Checkerboard.Read();
                param_Checkerboard.filePath = path;
                param_Checkerboard.HomMat2D_PathName = Project.Instance.paramPathSet.CutOverlayProjectBasePath + $"\\{CameraNameEnum}HomMat2D.mat";
                param_Checkerboard.HomMat2DInvert_PathName = Project.Instance.paramPathSet.CutOverlayProjectBasePath + $"\\{CameraNameEnum}HomMat2DInvert.mat";

                param_Checkerboard.ReadCalibParam();


            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
                return false;
            }
            return true;


        }

        /// <summary>
        /// 初始预定位标定参数
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool InitPretargetingCalibParam(string path)
        {
            try
            {
                if (param_PretargetingCalib == null) param_PretargetingCalib = new Param_PretargetingCalib();
                param_PretargetingCalib.filePath = path;

                param_PretargetingCalib = (Param_PretargetingCalib)param_PretargetingCalib.Read();
                param_PretargetingCalib.filePath = path;

                param_PretargetingCalib.HomMat2DPlatform_PathName = Project.Instance.paramPathSet.CutOverlayProjectBasePath + $"\\PlatformHomMat2D.mat";
                param_PretargetingCalib.HomMat2D_PathName = Project.Instance.paramPathSet.CutOverlayProjectBasePath + $"\\{CameraNameEnum}PretargetingHomMat2D.mat";
                param_PretargetingCalib.HomMat2DInvert_PathName = Project.Instance.paramPathSet.CutOverlayProjectBasePath + $"\\{CameraNameEnum}PretargetingHomMat2DInvert.mat";

                param_PretargetingCalib.Model_handle_PathName = Project.Instance.paramPathSet.CutOverlayProjectBasePath + $"\\{CameraNameEnum}ModelHandle.shm";
                param_PretargetingCalib.ModelImage_handle_PathName = Project.Instance.paramPathSet.CutOverlayProjectBasePath + $"\\{CameraNameEnum}ModelImage.bmp";


                param_PretargetingCalib.ReadCalibParam();


            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
                return false;
            }
            return true;


        }

        /// <summary>
        /// 初始MSA圆点参数
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool InitMsaCircleParam(string path)
        {
            try
            {
                if (param_MsaCircle == null) param_MsaCircle = new Param_MsaCircle();
                param_MsaCircle.filePath = path;

                param_MsaCircle = (Param_MsaCircle)param_MsaCircle.Read();
                param_MsaCircle.filePath = path;



            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
                return false;
            }
            return true;


        }

        /// <summary>
        /// 初始MSA线宽参数
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool InitMsaLineWidthParam(string path)
        {
            try
            {
                if (param_MsaLineWidth == null) param_MsaLineWidth = new Param_MsaLineWidth();
                param_MsaLineWidth.filePath = path;

                param_MsaLineWidth = (Param_MsaLineWidth)param_MsaLineWidth.Read();
                param_MsaLineWidth.filePath = path;



            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
                return false;
            }
            return true;


        }

        /// <summary>
        /// 初始化参数
        /// </summary>
        /// <returns></returns>
        public bool InitParam_GrayScaleAdaptationParameter(string path)
        {
            try
            {
                if (m_GrayScaleAdaptationParameter == null) m_GrayScaleAdaptationParameter = new GrayScaleAdaptationParameter();
                m_GrayScaleAdaptationParameter.filePath = path;
                m_GrayScaleAdaptationParameter = (GrayScaleAdaptationParameter)m_GrayScaleAdaptationParameter.Read();
                m_GrayScaleAdaptationParameter.filePath = path;

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
                return false;
            }
            return true;
        }
        /// <summary>
        /// 初始化参数
        /// </summary>
        /// <returns></returns>
        public bool InitParam_PointInspectionStateParameter(string path)
        {
            try
            {
                if (m_PointInspectionStateParam == null) m_PointInspectionStateParam = new PointInspectionStateParam();
                m_PointInspectionStateParam.filePath = path;
                m_PointInspectionStateParam = (PointInspectionStateParam)m_PointInspectionStateParam.Read();
                m_PointInspectionStateParam.filePath = path;

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
                return false;
            }
            return true;
        }
        /// <summary>
        /// 初始MSA3D测厚参数
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool InitMsa3DThickDetect(string path)
        {
            try
            {
                if (param_MsaThick3D == null) param_MsaThick3D = new Param_MsaThick3D();
                param_MsaThick3D.filePath = path;

                param_MsaThick3D = (Param_MsaThick3D)param_MsaThick3D.Read();
                param_MsaThick3D.filePath = path;

                param_MsaThick3D.LtROIParamsPath=path+"LtRoiParams.xml";
                param_MsaThick3D.LtDictionaryPath = path + "LtDictionary.xml";

                param_MsaThick3D.LoadRoiParam();


            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
                return false;
            }
            return true;


        }


        /// <summary>
        /// 初始OVH Mark标定参数
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool InitOVHMarkParam(string path)
        {
            try
            {
                if (param_MarkSet == null) param_MarkSet = new Param_MarkSet();
                param_MarkSet.filePath = path;

                param_MarkSet = (Param_MarkSet)param_MarkSet.Read();
                param_MarkSet.filePath = path;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
                return false;
            }
            return true;


        }

        /// <summary>
        /// 图片保存参数
        /// </summary>

        public ImageSaveParamer m_ImageSaveParamer = new ImageSaveParamer();
        public bool InitImageSaveParamer(string path)
        {
            try
            {
                if (m_ImageSaveParamer == null) m_ImageSaveParamer = new ImageSaveParamer();
                m_ImageSaveParamer.filePath = path;
                m_ImageSaveParamer = (ImageSaveParamer)m_ImageSaveParamer.Read();
                m_ImageSaveParamer.filePath = path;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
                return false;
            }
            return true;
        }
    }
}

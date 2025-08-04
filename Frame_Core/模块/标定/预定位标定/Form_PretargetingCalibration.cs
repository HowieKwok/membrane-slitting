using HalconDotNet;
using LogTool;
using NPOI.OpenXmlFormats.Dml.Diagram;
using NPOI.OpenXmlFormats.Vml;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Markup;
using YYSD.UICore.Control;
using YYSD.UICore;
using HslCommunication;
using NLog.Fluent;

namespace Frame_Core
{
    public partial class Form_PretargetingCalibration : Form
    {
        static Dictionary<CameraEnum, CameraManager> cameraManager;
        CameraEnum currentCameraEnum = CameraEnum.Cam1;
        CalibrationAlgorithmEnum currentCalibAlgorithmEnum = CalibrationAlgorithmEnum.棋盘格;
        Param_PretargetingCalib CurrentParam_PretargetingCalib = new Param_PretargetingCalib();

        /// <summary>
        /// 标定图片路径
        /// </summary>
        static string CalibrationPath = "";

        static Dictionary<int, string> dicCobInputXorY = new Dictionary<int, string>();
        static Dictionary<int, string> dicCobDirection = new Dictionary<int, string>();
        static Dictionary<int, string> dicCobinputTransition = new Dictionary<int, string>();
        static Dictionary<int, string> dicCobinputPosition = new Dictionary<int, string>();

        Thd_WorkClass thd_WorkClass = new Thd_WorkClass();
        DisplayWindowClass displayWindow_Cam1;
        DisplayWindowClass displayWindow_Cam2;
        DisplayWindowClass displayWindow_Cam3;
        DisplayWindowClass displayWindow_Cam4;

        public HTuple TempCheckerboardHomMat2D_Cam1 = new HTuple();//棋盘格矩阵
        public HTuple TempCheckerboardHomMat2D_Cam2 = new HTuple();
        public HTuple TempCheckerboardHomMat2D_Cam3 = new HTuple();
        public HTuple TempCheckerboardHomMat2D_Cam4 = new HTuple();

        public HTuple TempCheckerboardModel_handle_Cam1 = new HTuple();//模板矩阵
        public HTuple TempCheckerboardModel_handle_Cam2 = new HTuple();
        public HTuple TempCheckerboardModel_handle_Cam3 = new HTuple();
        public HTuple TempCheckerboardModel_handle_Cam4 = new HTuple();

        public HTuple TempPlatformHomMat2D = new HTuple();//平台矩阵
        double TempCenterOfRotation_X = 0;//旋转中心X
        double TempCenterOfRotation_Y = 0;//旋转中心Y

        /// <summary>
        /// 相机回调执行标定算法完成标志
        /// </summary>
        bool CamCalibrationFinish_Cam1 = false;

        /// <summary>
        /// 相机回调执行标定算法完成标志
        /// </summary>
        bool CamCalibrationFinish_Cam2 = false;

        /// <summary>
        /// 相机回调执行标定算法完成标志
        /// </summary>
        bool CamCalibrationFinish_Cam3 = false;

        /// <summary>
        /// 相机回调执行标定算法完成标志
        /// </summary>
        bool CamCalibrationFinish_Cam4 = false;

        /// <summary>
        /// 图片仿真路径数据
        /// </summary>
        public ImageSimManage m_ImageSimManage_Cam1 = new ImageSimManage();
        /// <summary>
        /// 图片仿真路径数据
        /// </summary>
        public ImageSimManage m_ImageSimManage_Cam2 = new ImageSimManage();
        /// <summary>
        /// 图片仿真路径数据
        /// </summary>
        public ImageSimManage m_ImageSimManage_Cam3 = new ImageSimManage();
        /// <summary>
        /// 图片仿真路径数据
        /// </summary>
        public ImageSimManage m_ImageSimManage_Cam4 = new ImageSimManage();
        public Form_PretargetingCalibration(Dictionary<CameraEnum, CameraManager> _cameraManager)
        {
            InitializeComponent();
            cameraManager = _cameraManager;
        }
        private void Form_PretargetingCalibration_Load(object sender, EventArgs e)
        {
            try
            {
                CalibrationPath = Project.Instance.paramPathSet.PretargetingCalibrationPath + $"{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff")}\\";
                Project.Instance.IsAutoCalibrationModel = true;

                dicCobInputXorY.Clear();
                dicCobInputXorY.Add(0, "X");
                dicCobInputXorY.Add(1, "Y");

                dicCobDirection.Clear();
                dicCobDirection.Add(0, "L2R_New");
                dicCobDirection.Add(1, "U2B_New");

                dicCobinputTransition.Clear();
                dicCobinputTransition.Add(0, "negative");
                dicCobinputTransition.Add(1, "positive");
                dicCobinputTransition.Add(2, "all");

                dicCobinputPosition.Clear();
                dicCobinputPosition.Add(0, "first");
                dicCobinputPosition.Add(1, "last");
                dicCobinputPosition.Add(2, "all");

                iCobInputXorY.Items.Clear();
                cob_SelectLine.Items.Clear();

                cob_SelectLine.Items.Add("Line1");
                cob_SelectLine.Items.Add("Line2");

                iCobInputXorY.DataSource = dicCobInputXorY.Values.ToArray();

                iCobDirection.Items.Clear();
                iCobDirection.DataSource = dicCobDirection.Values.ToArray();

                iCobinputTransition.Items.Clear();
                iCobinputTransition.DataSource = dicCobinputTransition.Values.ToArray();

                iCobinputPosition.Items.Clear();
                iCobinputPosition.DataSource = dicCobinputPosition.Values.ToArray();



                LB_Process1.Text = cameraManager[CameraEnum.Cam1].m_CamParam.CameraName;
                displayWindow_Cam1 = new DisplayWindowClass(HWC1, TB_PositionAndGray1, PB_FitWin1, "PretargetingCalibrationHWC1");
                displayWindow_Cam1.operation_ImageInfo.showHalconWindow = new ShowHalconWindow();
                displayWindow_Cam1.operation_ImageInfo.showHalconWindow.m_messageShowInfos = new List<MessageShowInfo>();
                displayWindow_Cam1.operation_ImageInfo.showHalconWindow.m_regionShowInfos = new List<RegionShowInfo>();
                displayWindow_Cam1.operation_ImageInfo.showHalconWindow.m_lineShowInfos = new List<LineInfo>();

                LB_Process2.Text = cameraManager[CameraEnum.Cam2].m_CamParam.CameraName;
                displayWindow_Cam2 = new DisplayWindowClass(HWC2, TB_PositionAndGray2, PB_FitWin2, "PretargetingCalibrationHWC2");
                displayWindow_Cam2.operation_ImageInfo.showHalconWindow = new ShowHalconWindow();
                displayWindow_Cam2.operation_ImageInfo.showHalconWindow.m_messageShowInfos = new List<MessageShowInfo>();
                displayWindow_Cam2.operation_ImageInfo.showHalconWindow.m_regionShowInfos = new List<RegionShowInfo>();
                displayWindow_Cam2.operation_ImageInfo.showHalconWindow.m_lineShowInfos = new List<LineInfo>();

                LB_Process3.Text = cameraManager[CameraEnum.Cam3].m_CamParam.CameraName;
                displayWindow_Cam3 = new DisplayWindowClass(HWC3, TB_PositionAndGray3, PB_FitWin3, "PretargetingCalibrationHWC3");
                displayWindow_Cam3.operation_ImageInfo.showHalconWindow = new ShowHalconWindow();
                displayWindow_Cam3.operation_ImageInfo.showHalconWindow.m_messageShowInfos = new List<MessageShowInfo>();
                displayWindow_Cam3.operation_ImageInfo.showHalconWindow.m_regionShowInfos = new List<RegionShowInfo>();
                displayWindow_Cam3.operation_ImageInfo.showHalconWindow.m_lineShowInfos = new List<LineInfo>();

                LB_Process4.Text = cameraManager[CameraEnum.Cam4].m_CamParam.CameraName;
                displayWindow_Cam4 = new DisplayWindowClass(HWC4, TB_PositionAndGray4, PB_FitWin4, "PretargetingCalibrationHWC4");
                displayWindow_Cam4.operation_ImageInfo.showHalconWindow = new ShowHalconWindow();
                displayWindow_Cam4.operation_ImageInfo.showHalconWindow.m_messageShowInfos = new List<MessageShowInfo>();
                displayWindow_Cam4.operation_ImageInfo.showHalconWindow.m_regionShowInfos = new List<RegionShowInfo>();
                displayWindow_Cam4.operation_ImageInfo.showHalconWindow.m_lineShowInfos = new List<LineInfo>();

                Project.Instance.baseProDevice.m_DicCamOperaParam[0].AutoCalibrationDelegate = null;
                Project.Instance.baseProDevice.m_DicCamOperaParam[0].AutoCalibrationDelegate += CalibrationProcess_Cam1;
                Project.Instance.baseProDevice.m_DicCamOperaParam[1].AutoCalibrationDelegate = null;
                Project.Instance.baseProDevice.m_DicCamOperaParam[1].AutoCalibrationDelegate += CalibrationProcess_Cam2;
                Project.Instance.baseProDevice.m_DicCamOperaParam[2].AutoCalibrationDelegate = null;
                Project.Instance.baseProDevice.m_DicCamOperaParam[2].AutoCalibrationDelegate += CalibrationProcess_Cam3;
                Project.Instance.baseProDevice.m_DicCamOperaParam[3].AutoCalibrationDelegate = null;
                Project.Instance.baseProDevice.m_DicCamOperaParam[3].AutoCalibrationDelegate += CalibrationProcess_Cam4;

                Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[0].AutoCalibrationDelegate = null;
                Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[0].AutoCalibrationDelegate += CalibrationProcess_Cam1;
                Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[1].AutoCalibrationDelegate = null;
                Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[1].AutoCalibrationDelegate += CalibrationProcess_Cam2;
                Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[2].AutoCalibrationDelegate = null;
                Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[2].AutoCalibrationDelegate += CalibrationProcess_Cam3;
                Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[3].AutoCalibrationDelegate = null;
                Project.Instance.baseProDevice.m_DicCamOperaParam_Sim[3].AutoCalibrationDelegate += CalibrationProcess_Cam4;


                thd_WorkClass.Start(100, "ThdPretargetingCalibration");
                thd_WorkClass.PerformWork -= ThdPretargetingCalibration;
                thd_WorkClass.PerformWork += ThdPretargetingCalibration;

                cbx_MeasureTransition.DataSource = Enum.GetNames(typeof(MeasureTransitionEnum));
                cbx_MeasureSelect.DataSource = Enum.GetNames(typeof(MeasureSelectEnum));

                cbx_CamSelect.Items.Clear();
                foreach (var item in cameraManager)
                {
                    cbx_CamSelect.Items.Add(item.Value.m_CamParam.CameraName.ToString());
                }
                cbx_CamSelect.SelectedIndex = (int)Project.Instance.craftBase.BaseCamera;

                if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面A极片纠偏工位 ||
                    Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面B极片纠偏工位)

                {
                    pictureBox2.Image = Frame_Core.Properties.Resources.A面模板图;
                }
                else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面A极片纠偏工位 ||
                    Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面B极片纠偏工位)
                {
                    pictureBox2.Image = Frame_Core.Properties.Resources.B面模板图;
                }

                TempCheckerboardHomMat2D_Cam1 = cameraManager[CameraEnum.Cam1].param_PretargetingCalib.HomMat2D;
                TempCheckerboardHomMat2D_Cam2 = cameraManager[CameraEnum.Cam2].param_PretargetingCalib.HomMat2D;
                TempCheckerboardHomMat2D_Cam3 = cameraManager[CameraEnum.Cam3].param_PretargetingCalib.HomMat2D;
                TempCheckerboardHomMat2D_Cam4 = cameraManager[CameraEnum.Cam4].param_PretargetingCalib.HomMat2D;

                TempCheckerboardModel_handle_Cam1 = cameraManager[CameraEnum.Cam1].param_PretargetingCalib.Model_handle;
                TempCheckerboardModel_handle_Cam2 = cameraManager[CameraEnum.Cam2].param_PretargetingCalib.Model_handle;
                TempCheckerboardModel_handle_Cam3 = cameraManager[CameraEnum.Cam3].param_PretargetingCalib.Model_handle;
                TempCheckerboardModel_handle_Cam4 = cameraManager[CameraEnum.Cam4].param_PretargetingCalib.Model_handle;

                TempPlatformHomMat2D = cameraManager[CameraEnum.Cam3].param_PretargetingCalib.HomMat2DPlatform;
                TempCenterOfRotation_X = cameraManager[CameraEnum.Cam3].param_PretargetingCalib.CenterOfRotationX;
                TempCenterOfRotation_Y = cameraManager[CameraEnum.Cam3].param_PretargetingCalib.CenterOfRotationY;
                ///读取相机模板图像
                foreach (var item in cameraManager)
                {
                    item.Value.param_PretargetingCalib.ReadModelHandle();
                }

                LoadCalibrationImgPath(tb_CalibImgPath.Text, currentCalibAlgorithmEnum);

                cbx_CalibAlgorithmSelection.Items.Clear();
                cbx_CalibAlgorithmSelection.Items.AddRange(Enum.GetNames(typeof(CalibrationAlgorithmEnum)));
                cbx_CalibAlgorithmSelection.SelectedIndex = 0;
                UpdateForm();

                //修改记录备份
                CurrentParam_PretargetingCalib.backup(CurrentParam_PretargetingCalib);
                YControlAutobind.Loading<Param_PretargetingCalib, IYCustomAttribute>(this, CurrentParam_PretargetingCalib);//测试存修改记录
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }
        private void Form_PretargetingCalibration_FormClosing(object sender, FormClosingEventArgs e)
        {

            try
            {
                Project.Instance.IsAutoCalibrationModel = false;

                thd_WorkClass.Stop();
                displayWindow_Cam1?.Dispose();
                displayWindow_Cam1?.operation_ImageInfo?.showHalconWindow?.Dispose();
                displayWindow_Cam2?.Dispose();
                displayWindow_Cam2?.operation_ImageInfo?.showHalconWindow?.Dispose();
                displayWindow_Cam3?.Dispose();
                displayWindow_Cam3?.operation_ImageInfo?.showHalconWindow?.Dispose();
                displayWindow_Cam4?.Dispose();
                displayWindow_Cam4?.operation_ImageInfo?.showHalconWindow?.Dispose();
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }

        private void cbx_CamSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                currentCameraEnum = (CameraEnum)cbx_CamSelect.SelectedIndex;
                CurrentParam_PretargetingCalib = cameraManager[(CameraEnum)cbx_CamSelect.SelectedIndex].param_PretargetingCalib;
                LoadParam(ref CurrentParam_PretargetingCalib);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }
        private void cbx_CalibAlgorithmSelection_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                currentCalibAlgorithmEnum = (CalibrationAlgorithmEnum)cbx_CalibAlgorithmSelection.SelectedIndex;
                LoadCalibrationImgPath(tb_CalibImgPath.Text, cbx_CalibAlgorithmSelection.Text, currentCalibAlgorithmEnum);

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                MessageBox.Show(ex.Message);
            }

        }

        /// <summary>
        /// 加载标定图像路径
        /// </summary>
        /// <param name="calibrationImgPath"></param>
        /// <param name="currentCalibAlgorithmEnum"></param>
        public void LoadCalibrationImgPath(string calibrationImgPath, string typeName, CalibrationAlgorithmEnum currentCalibAlgorithmEnum)
        {
            try
            {
                if (calibrationImgPath == "" || calibrationImgPath == null)
                {
                    return;
                }
                string parentDirectory = Directory.GetParent(calibrationImgPath).FullName;
                // 新文件夹路径
                string fullPath = Path.Combine(parentDirectory, typeName);

                if (true)
                {
                    CameraEnum cameraEnum = CameraEnum.Cam1;
                    m_ImageSimManage_Cam1.InitSimFlag();
                    // 获取父目录路径

                    //判断是否存在
                    if (!Directory.Exists(fullPath))
                    {
                        MessageBox.Show("标定图像路径不存在，请重新选择");

                    }
                    m_ImageSimManage_Cam1.ReadImageSimPath(fullPath + $"\\{cameraEnum}");

                }

                if (true)
                {
                    CameraEnum cameraEnum = CameraEnum.Cam2;
                    m_ImageSimManage_Cam2.InitSimFlag();
                    //判断是否存在
                    if (!Directory.Exists(fullPath))
                    {
                        MessageBox.Show("标定图像路径不存在，请重新选择");

                    }
                    m_ImageSimManage_Cam2.ReadImageSimPath(fullPath + $"\\{cameraEnum}");
                }
                if (true)
                {
                    CameraEnum cameraEnum = CameraEnum.Cam3;
                    m_ImageSimManage_Cam3.InitSimFlag();
                    //判断是否存在
                    if (!Directory.Exists(fullPath))
                    {
                        MessageBox.Show("标定图像路径不存在，请重新选择");

                    }
                    m_ImageSimManage_Cam3.ReadImageSimPath(fullPath + $"\\{cameraEnum}");
                }
                if (true)
                {
                    CameraEnum cameraEnum = CameraEnum.Cam4;
                    m_ImageSimManage_Cam4.InitSimFlag();
                    //判断是否存在
                    if (!Directory.Exists(fullPath))
                    {
                        MessageBox.Show("标定图像路径不存在，请重新选择");

                    }
                    m_ImageSimManage_Cam4.ReadImageSimPath(fullPath + $"\\{cameraEnum}");
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                MessageBox.Show(ex.ToString());
            }

        }



        private void btn_Save_Click(object sender, EventArgs e)
        {
            try
            {
                SaveParam();
                LoadCalibrationImgPath(tb_CalibImgPath.Text, currentCalibAlgorithmEnum);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                MessageBox.Show(ex.Message);
            }

        }

        private void lklb_CalibImgPath_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                // 获取文件夹绝对路径    显示在 txtbox 控件里
                using (FolderBrowserDialog folder = new FolderBrowserDialog())
                {
                    if (Project.Instance.paramPathSet.PretargetingCalibrationPath != null && System.IO.Directory.Exists(Project.Instance.paramPathSet.PretargetingCalibrationPath))
                    {
                        folder.SelectedPath = Project.Instance.paramPathSet.PretargetingCalibrationPath;

                    }
                    if (folder.ShowDialog() == DialogResult.OK && folder.SelectedPath != "")
                    {
                        tb_CalibImgPath.Text = folder.SelectedPath;

                        LoadCalibrationImgPath(tb_CalibImgPath.Text, currentCalibAlgorithmEnum);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btn_RunCurrentImg_Click(object sender, EventArgs e)
        {
            try
            {
                if (displayWindow_Cam1.operation_ImageInfo?.showHalconWindow?.m_hImage != null && displayWindow_Cam1.operation_ImageInfo?.showHalconWindow?.m_hImage?.CountObj() > 0)
                {
                    HOperatorSet.CopyObj(displayWindow_Cam1.operation_ImageInfo.showHalconWindow.m_hImage, out HObject ho_Image, 1, 1);
                    CalibrationProcess_Cam1(CameraEnum.Cam1, ho_Image);
                }

                if (displayWindow_Cam2.operation_ImageInfo?.showHalconWindow?.m_hImage != null && displayWindow_Cam2.operation_ImageInfo?.showHalconWindow?.m_hImage?.CountObj() > 0)
                {
                    HOperatorSet.CopyObj(displayWindow_Cam2.operation_ImageInfo.showHalconWindow.m_hImage, out HObject ho_Image, 1, 1);
                    CalibrationProcess_Cam2(CameraEnum.Cam2, ho_Image);
                }

                if (displayWindow_Cam3.operation_ImageInfo?.showHalconWindow?.m_hImage != null && displayWindow_Cam3.operation_ImageInfo?.showHalconWindow?.m_hImage?.CountObj() > 0)
                {
                    HOperatorSet.CopyObj(displayWindow_Cam3.operation_ImageInfo.showHalconWindow.m_hImage, out HObject ho_Image, 1, 1);
                    CalibrationProcess_Cam3(CameraEnum.Cam3, ho_Image);
                }

                if (displayWindow_Cam4.operation_ImageInfo?.showHalconWindow?.m_hImage != null && displayWindow_Cam4.operation_ImageInfo?.showHalconWindow?.m_hImage?.CountObj() > 0)
                {
                    HOperatorSet.CopyObj(displayWindow_Cam4.operation_ImageInfo.showHalconWindow.m_hImage, out HObject ho_Image, 1, 1);
                    CalibrationProcess_Cam4(CameraEnum.Cam4, ho_Image);
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                MessageBox.Show(ex.ToString());
            }
        }


        Task m_taskTranslation = null;
        private void Btn_offlineTranslation_Click(object sender, EventArgs e)
        {
            try
            {
                if (!(m_ImageSimManage_Cam1.ImageEmulationPaths.Length > 0 &&
                   m_ImageSimManage_Cam1.ImageEmulationPaths.Length == dgv_PlatformTranslationCalibPlatformCoord.RowCount))
                {
                    MessageBox.Show($"Cam1平移仿真的图像数{m_ImageSimManage_Cam1.ImageEmulationPaths.Length}与设置的平台坐标数{dgv_PlatformTranslationCalibPlatformCoord.RowCount}不匹配，请重新选择平移仿真图像或平台坐标");
                    return;
                }

                if (!(m_ImageSimManage_Cam2.ImageEmulationPaths.Length > 0 &&
                    m_ImageSimManage_Cam2.ImageEmulationPaths.Length == dgv_PlatformTranslationCalibPlatformCoord.RowCount))
                {
                    MessageBox.Show($"Cam2平移仿真的图像数{m_ImageSimManage_Cam2.ImageEmulationPaths.Length}与设置的平台坐标数{dgv_PlatformTranslationCalibPlatformCoord.RowCount}不匹配，请重新选择平移仿真图像或平台坐标");
                    return;
                }

                if (!(m_ImageSimManage_Cam3.ImageEmulationPaths.Length > 0 &&
                  m_ImageSimManage_Cam3.ImageEmulationPaths.Length == dgv_PlatformTranslationCalibPlatformCoord.RowCount))
                {
                    MessageBox.Show($"Cam3平移仿真的图像数{m_ImageSimManage_Cam2.ImageEmulationPaths.Length}与设置的平台坐标数{dgv_PlatformTranslationCalibPlatformCoord.RowCount}不匹配，请重新选择平移仿真图像或平台坐标");
                    return;
                }

                if (!(m_ImageSimManage_Cam4.ImageEmulationPaths.Length > 0 &&
                  m_ImageSimManage_Cam4.ImageEmulationPaths.Length == dgv_PlatformTranslationCalibPlatformCoord.RowCount))
                {
                    MessageBox.Show($"Cam4平移仿真的图像数{m_ImageSimManage_Cam4.ImageEmulationPaths.Length}与设置的平台坐标数{dgv_PlatformTranslationCalibPlatformCoord.RowCount}不匹配，请重新选择平移仿真图像或平台坐标");
                    return;
                }

                //如果m_taskTranslation没有结束，不允许再次开始
                if (m_taskTranslation != null && !m_taskTranslation.IsCompleted)
                {
                    MessageBox.Show("平移仿真任务正在进行中，请等待任务结束后再开始新的任务");
                    return;
                }


                NDataGridView.ClearRows(dataGridView_Cam1);
                NDataGridView.ClearRows(dataGridView_Cam2);
                NDataGridView.ClearRows(dataGridView_Cam3);
                NDataGridView.ClearRows(dataGridView_Cam4);



                m_taskTranslation = new Task(new Action(() =>
                {
                    for (int i = 0; i < dgv_PlatformTranslationCalibPlatformCoord.RowCount; i++)
                    {
                        if (i == 0)
                        {

                            Parallel.Invoke(

                                  () =>
                                  {
                                      m_ImageSimManage_Cam1.currentImageIndex = 0;
                                      m_ImageSimManage_Cam1.GetCurrentImage(out HObject emulationImage1);
                                      if (emulationImage1 != null && emulationImage1?.CountObj() > 0)
                                      {
                                          CalibrationProcess_Cam1(CameraEnum.Cam1, emulationImage1);
                                      }
                                  }
                                ,
                                () =>
                                {
                                    m_ImageSimManage_Cam2.currentImageIndex = 0;
                                    m_ImageSimManage_Cam2.GetCurrentImage(out HObject emulationImage2);
                                    if (emulationImage2 != null && emulationImage2?.CountObj() > 0)
                                    {
                                        CalibrationProcess_Cam2(CameraEnum.Cam2, emulationImage2);
                                    }
                                }
                            , () =>
                            {
                                m_ImageSimManage_Cam3.currentImageIndex = 0;
                                m_ImageSimManage_Cam3.GetCurrentImage(out HObject emulationImage3);
                                if (emulationImage3 != null && emulationImage3?.CountObj() > 0)
                                {
                                    CalibrationProcess_Cam3(CameraEnum.Cam3, emulationImage3);
                                }


                            }

                            ,
                                () =>
                                {
                                    m_ImageSimManage_Cam4.currentImageIndex = 0;
                                    m_ImageSimManage_Cam4.GetCurrentImage(out HObject emulationImage4);
                                    if (emulationImage4 != null && emulationImage4?.CountObj() > 0)
                                    {
                                        CalibrationProcess_Cam4(CameraEnum.Cam1, emulationImage4);
                                    }
                                });

                        }
                        else
                        {
                            Parallel.Invoke(
                                () =>
                                {
                                    m_ImageSimManage_Cam1.GetNextImage(out HObject emulationImage1);
                                    if (emulationImage1 != null && emulationImage1?.CountObj() > 0)
                                    {
                                        CalibrationProcess_Cam1(CameraEnum.Cam1, emulationImage1);
                                    }
                                },

                                    () =>
                                    {
                                        m_ImageSimManage_Cam2.GetNextImage(out HObject emulationImage2);
                                        if (emulationImage2 != null && emulationImage2?.CountObj() > 0)
                                        {
                                            CalibrationProcess_Cam2(CameraEnum.Cam2, emulationImage2);
                                        }
                                    }, () =>
                                    {
                                        m_ImageSimManage_Cam3.GetNextImage(out HObject emulationImage3);
                                        if (emulationImage3 != null && emulationImage3?.CountObj() > 0)
                                        {
                                            CalibrationProcess_Cam3(CameraEnum.Cam3, emulationImage3);
                                        }
                                    }
                                  ,
                                    () =>
                                    {
                                        m_ImageSimManage_Cam4.GetNextImage(out HObject emulationImage4);
                                        if (emulationImage4 != null && emulationImage4?.CountObj() > 0)
                                        {
                                            CalibrationProcess_Cam4(CameraEnum.Cam4, emulationImage4);
                                        }
                                    });



                        }
                        Thread.Sleep(1000);
                    }

                    this.Invoke(new Action(() =>
                    {
                        btn_Calib_Click(null, null);
                    }));
                    //输出弹窗，显示是否保存平移矩阵

                    MessageBox.Show("平移仿真任务完成，是否保存平移矩阵？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (MessageBox.Show("是否保存平移矩阵？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        this.Invoke(new Action(() =>
                        {
                            btn_SavePlatformCalibrationChecker_Click(null, null);
                        }));

                        MessageBox.Show("平移矩阵已保存");
                    }
                }));
                m_taskTranslation.Start();





            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                MessageBox.Show(ex.ToString());
            }

        }


        Task m_taskRotate = null;
        private void Btn_offlineRotate_Click(object sender, EventArgs e)
        {
            try
            {
                if (!(m_ImageSimManage_Cam1.ImageEmulationPaths.Length > 0 &&
                  m_ImageSimManage_Cam1.ImageEmulationPaths.Length == dgv_PlatformRotateCalibPlatformCoord.RowCount))
                {
                    MessageBox.Show($"Cam1旋转仿真的图像数{m_ImageSimManage_Cam1.ImageEmulationPaths.Length}与设置的平台坐标数{dgv_PlatformTranslationCalibPlatformCoord.RowCount}不匹配，请重新选择平移仿真图像或平台坐标");
                    return;
                }

                if (!(m_ImageSimManage_Cam2.ImageEmulationPaths.Length > 0 &&
                    m_ImageSimManage_Cam2.ImageEmulationPaths.Length == dgv_PlatformRotateCalibPlatformCoord.RowCount))
                {
                    MessageBox.Show($"Cam2旋转仿真的图像数{m_ImageSimManage_Cam2.ImageEmulationPaths.Length}与设置的平台坐标数{dgv_PlatformTranslationCalibPlatformCoord.RowCount}不匹配，请重新选择平移仿真图像或平台坐标");
                    return;
                }

                if (!(m_ImageSimManage_Cam3.ImageEmulationPaths.Length > 0 &&
                  m_ImageSimManage_Cam3.ImageEmulationPaths.Length == dgv_PlatformRotateCalibPlatformCoord.RowCount))
                {
                    MessageBox.Show($"Cam3旋转仿真的图像数{m_ImageSimManage_Cam2.ImageEmulationPaths.Length}与设置的平台坐标数{dgv_PlatformTranslationCalibPlatformCoord.RowCount}不匹配，请重新选择平移仿真图像或平台坐标");
                    return;
                }

                if (!(m_ImageSimManage_Cam4.ImageEmulationPaths.Length > 0 &&
                 m_ImageSimManage_Cam4.ImageEmulationPaths.Length == dgv_PlatformRotateCalibPlatformCoord.RowCount))
                {
                    MessageBox.Show($"Cam4旋转仿真的图像数{m_ImageSimManage_Cam4.ImageEmulationPaths.Length}与设置的平台坐标数{dgv_PlatformTranslationCalibPlatformCoord.RowCount}不匹配，请重新选择平移仿真图像或平台坐标");
                    return;
                }


                //如果m_taskTranslation没有结束，不允许再次开始
                if (m_taskTranslation != null && !m_taskTranslation.IsCompleted)
                {
                    MessageBox.Show("旋转仿真任务正在进行中，请等待任务结束后再开始新的任务");
                    return;
                }


                NDataGridView.ClearRows(dataGridView_Cam1);
                NDataGridView.ClearRows(dataGridView_Cam2);
                NDataGridView.ClearRows(dataGridView_Cam3);
                NDataGridView.ClearRows(dataGridView_Cam4);



                m_taskRotate = new Task(new Action(() =>
                {
                    for (int i = 0; i < dgv_PlatformRotateCalibPlatformCoord.RowCount; i++)
                    {
                        if (i == 0)
                        {
                            Parallel.Invoke(
                                () =>
                                {
                                    m_ImageSimManage_Cam1.currentImageIndex = 0;
                                    m_ImageSimManage_Cam1.GetCurrentImage(out HObject emulationImage1);
                                    if (emulationImage1 != null && emulationImage1?.CountObj() > 0)
                                    {
                                        CalibrationProcess_Cam1(CameraEnum.Cam1, emulationImage1);
                                    }
                                },
                                () =>
                                {
                                    m_ImageSimManage_Cam2.currentImageIndex = 0;
                                    m_ImageSimManage_Cam2.GetCurrentImage(out HObject emulationImage2);
                                    if (emulationImage2 != null && emulationImage2?.CountObj() > 0)
                                    {
                                        CalibrationProcess_Cam2(CameraEnum.Cam2, emulationImage2);
                                    }
                                }, () =>
                                {
                                    m_ImageSimManage_Cam3.currentImageIndex = 0;
                                    m_ImageSimManage_Cam3.GetCurrentImage(out HObject emulationImage3);
                                    if (emulationImage3 != null && emulationImage3?.CountObj() > 0)
                                    {
                                        CalibrationProcess_Cam3(CameraEnum.Cam3, emulationImage3);
                                    }
                                },
                                () =>
                                {
                                    m_ImageSimManage_Cam4.currentImageIndex = 0;
                                    m_ImageSimManage_Cam4.GetCurrentImage(out HObject emulationImage4);
                                    if (emulationImage4 != null && emulationImage4?.CountObj() > 0)
                                    {
                                        CalibrationProcess_Cam4(CameraEnum.Cam4, emulationImage4);
                                    }
                                }


                                );




                        }
                        else
                        {

                            Parallel.Invoke(
                                () =>
                                {
                                    m_ImageSimManage_Cam1.GetNextImage(out HObject emulationImage1);
                                    if (emulationImage1 != null && emulationImage1?.CountObj() > 0)
                                    {
                                        CalibrationProcess_Cam1(CameraEnum.Cam1, emulationImage1);
                                    }
                                },
                                () =>
                                {
                                    m_ImageSimManage_Cam2.GetNextImage(out HObject emulationImage2);
                                    if (emulationImage2 != null && emulationImage2?.CountObj() > 0)
                                    {
                                        CalibrationProcess_Cam2(CameraEnum.Cam2, emulationImage2);
                                    }
                                }, () =>
                                {
                                    m_ImageSimManage_Cam3.GetNextImage(out HObject emulationImage3);
                                    if (emulationImage3 != null && emulationImage3?.CountObj() > 0)
                                    {
                                        CalibrationProcess_Cam3(CameraEnum.Cam3, emulationImage3);
                                    }
                                },
                                () =>
                                {
                                    m_ImageSimManage_Cam4.GetNextImage(out HObject emulationImage4);
                                    if (emulationImage4 != null && emulationImage4?.CountObj() > 0)
                                    {
                                        CalibrationProcess_Cam4(CameraEnum.Cam4, emulationImage4);
                                    }
                                }

                                   );


                        }
                        Thread.Sleep(1000);
                    }

                    this.Invoke(new Action(() =>
                    {
                        btn_Calib_Click(null, null);
                    }));


                }));
                m_taskRotate.Start();





            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                MessageBox.Show(ex.ToString());
            }
        }


        private void btn_Preview_Click(object sender, EventArgs e)
        {
            try
            {
                m_ImageSimManage_Cam1.GetPreviewImage(out HObject emulationImage1);
                if (emulationImage1 != null && emulationImage1?.CountObj() > 0)
                {
                    CalibrationProcess_Cam1(CameraEnum.Cam1, emulationImage1);
                }

                m_ImageSimManage_Cam2.GetPreviewImage(out HObject emulationImage2);
                if (emulationImage2 != null && emulationImage2?.CountObj() > 0)
                {
                    CalibrationProcess_Cam2(CameraEnum.Cam2, emulationImage2);
                }

                m_ImageSimManage_Cam3.GetPreviewImage(out HObject emulationImage3);
                if (emulationImage3 != null && emulationImage3?.CountObj() > 0)
                {
                    CalibrationProcess_Cam3(CameraEnum.Cam3, emulationImage3);
                }

                m_ImageSimManage_Cam4.GetPreviewImage(out HObject emulationImage4);
                if (emulationImage4 != null && emulationImage4?.CountObj() > 0)
                {
                    CalibrationProcess_Cam4(CameraEnum.Cam4, emulationImage4);
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                MessageBox.Show(ex.ToString());
            }
        }

        private void btn_Run_Click(object sender, EventArgs e)
        {
            try
            {
                m_ImageSimManage_Cam1.GetCurrentImage(out HObject emulationImage1);
                if (emulationImage1 != null && emulationImage1?.CountObj() > 0)
                {
                    CalibrationProcess_Cam1(CameraEnum.Cam1, emulationImage1);
                }

                m_ImageSimManage_Cam2.GetCurrentImage(out HObject emulationImage2);
                if (emulationImage2 != null && emulationImage2?.CountObj() > 0)
                {
                    CalibrationProcess_Cam2(CameraEnum.Cam2, emulationImage2);
                }

                m_ImageSimManage_Cam3.GetCurrentImage(out HObject emulationImage3);
                if (emulationImage3 != null && emulationImage3?.CountObj() > 0)
                {
                    CalibrationProcess_Cam3(CameraEnum.Cam3, emulationImage3);
                }

                m_ImageSimManage_Cam4.GetCurrentImage(out HObject emulationImage4);
                if (emulationImage4 != null && emulationImage4?.CountObj() > 0)
                {
                    CalibrationProcess_Cam4(CameraEnum.Cam4, emulationImage4);
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                MessageBox.Show(ex.ToString());
            }
        }

        private void btn_Next_Click(object sender, EventArgs e)
        {
            try
            {
                m_ImageSimManage_Cam1.GetNextImage(out HObject emulationImage1);
                if (emulationImage1 != null && emulationImage1?.CountObj() > 0)
                {
                    CalibrationProcess_Cam1(CameraEnum.Cam1, emulationImage1);
                }

                m_ImageSimManage_Cam2.GetNextImage(out HObject emulationImage2);
                if (emulationImage2 != null && emulationImage2?.CountObj() > 0)
                {
                    CalibrationProcess_Cam2(CameraEnum.Cam2, emulationImage2);
                }

                m_ImageSimManage_Cam3.GetNextImage(out HObject emulationImage3);
                if (emulationImage3 != null && emulationImage3?.CountObj() > 0)
                {
                    CalibrationProcess_Cam3(CameraEnum.Cam3, emulationImage3);
                }

                m_ImageSimManage_Cam4.GetNextImage(out HObject emulationImage4);
                if (emulationImage4 != null && emulationImage4?.CountObj() > 0)
                {
                    CalibrationProcess_Cam4(CameraEnum.Cam4, emulationImage4);
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                MessageBox.Show(ex.ToString());
            }
        }





        private void btn_ClearDetectionDatagridview_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("是否清空当前检测数据", "询问", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    NDataGridView.ClearRows(dataGridView_Cam1);
                    NDataGridView.ClearRows(dataGridView_Cam2);
                    NDataGridView.ClearRows(dataGridView_Cam3);
                    NDataGridView.ClearRows(dataGridView_Cam4);
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                MessageBox.Show(ex.ToString());
            }
        }

        public void UpdateForm()
        {
            TLPanal_CamMain.Invoke(new Action(() =>
            {
                if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面A极片纠偏工位 ||
                  Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面B极片纠偏工位)
                {
                    TLPanal_CamMain.SetRow(tableLayoutPanel_Cam4, 0);
                    TLPanal_CamMain.SetColumn(tableLayoutPanel_Cam4, 0);
                    TLPanal_CamMain.SetRowSpan(tableLayoutPanel_Cam4, 1);
                    TLPanal_CamMain.SetColumnSpan(tableLayoutPanel_Cam4, 1);

                    TLPanal_CamMain.SetRow(tableLayoutPanel_Cam3, 0);
                    TLPanal_CamMain.SetColumn(tableLayoutPanel_Cam3, 1);
                    TLPanal_CamMain.SetRowSpan(tableLayoutPanel_Cam3, 1);
                    TLPanal_CamMain.SetColumnSpan(tableLayoutPanel_Cam3, 1);

                    TLPanal_CamMain.SetRow(tableLayoutPanel_Cam2, 1);
                    TLPanal_CamMain.SetColumn(tableLayoutPanel_Cam2, 0);
                    TLPanal_CamMain.SetRowSpan(tableLayoutPanel_Cam2, 1);
                    TLPanal_CamMain.SetColumnSpan(tableLayoutPanel_Cam2, 1);

                    TLPanal_CamMain.SetRow(tableLayoutPanel_Cam1, 1);
                    TLPanal_CamMain.SetColumn(tableLayoutPanel_Cam1, 1);
                    TLPanal_CamMain.SetRowSpan(tableLayoutPanel_Cam1, 1);
                    TLPanal_CamMain.SetColumnSpan(tableLayoutPanel_Cam1, 1);
                }
                else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面A极片纠偏工位 ||
                         Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面B极片纠偏工位)

                {

                    TLPanal_CamMain.SetRow(tableLayoutPanel_Cam1, 0);
                    TLPanal_CamMain.SetColumn(tableLayoutPanel_Cam1, 0);
                    TLPanal_CamMain.SetRowSpan(tableLayoutPanel_Cam1, 1);
                    TLPanal_CamMain.SetColumnSpan(tableLayoutPanel_Cam1, 1);

                    TLPanal_CamMain.SetRow(tableLayoutPanel_Cam2, 0);
                    TLPanal_CamMain.SetColumn(tableLayoutPanel_Cam2, 1);
                    TLPanal_CamMain.SetRowSpan(tableLayoutPanel_Cam2, 1);
                    TLPanal_CamMain.SetColumnSpan(tableLayoutPanel_Cam2, 1);

                    TLPanal_CamMain.SetRow(tableLayoutPanel_Cam3, 1);
                    TLPanal_CamMain.SetColumn(tableLayoutPanel_Cam3, 0);
                    TLPanal_CamMain.SetRowSpan(tableLayoutPanel_Cam3, 1);
                    TLPanal_CamMain.SetColumnSpan(tableLayoutPanel_Cam3, 1);

                    TLPanal_CamMain.SetRow(tableLayoutPanel_Cam4, 1);
                    TLPanal_CamMain.SetColumn(tableLayoutPanel_Cam4, 1);
                    TLPanal_CamMain.SetRowSpan(tableLayoutPanel_Cam4, 1);
                    TLPanal_CamMain.SetColumnSpan(tableLayoutPanel_Cam4, 1);

                }
            }));
        }

        #region 参数加载与显示
        /// <summary>
        /// 加载参数
        /// </summary>
        /// <param name="param_Calib"></param>
        private void LoadParam(ref Param_PretargetingCalib param_Calib)
        {
            try
            {
                //（所有相机都保存）
                ControlTool.SetParamShow(tb_CalibImgPath, ref param_Calib.CalibImgPath);

                //基准和补偿参数（所有相机都保存）
                ControlTool.SetParamShow(nud_MachineHandBCValueX, ref param_Calib.MachineHandBCValue.Col);
                ControlTool.SetParamShow(nud_MachineHandBCValueY, ref param_Calib.MachineHandBCValue.Row);
                ControlTool.SetParamShow(nud_MachineHandBCValueR, ref param_Calib.MachineHandBCValue.Angle);
                ControlTool.SetParamShow(cbx_NotBCValueX, ref param_Calib.NotBCValueX);
                ControlTool.SetParamShow(cbx_NotBCValueY, ref param_Calib.NotBCValueY);
                ControlTool.SetParamShow(cbx_NotBCValueR, ref param_Calib.NotBCValueR);

                ControlTool.SetParamShow(nud_BCDownOffectValueX, ref param_Calib.MachineDownBCOffectValue.Col);
                ControlTool.SetParamShow(nud_BCDownOffectValueY, ref param_Calib.MachineDownBCOffectValue.Row);
                ControlTool.SetParamShow(nud_BCDownOffectValueR, ref param_Calib.MachineDownBCOffectValue.Angle);
                ControlTool.SetParamShow(nud_BCUpOffectValueX, ref param_Calib.MachineUpBCOffectValue.Col);
                ControlTool.SetParamShow(nud_BCUpOffectValueY, ref param_Calib.MachineUpBCOffectValue.Row);
                ControlTool.SetParamShow(nud_BCUpOffectValueR, ref param_Calib.MachineUpBCOffectValue.Angle);

                ControlTool.SetParamShow(nud_BCHandOffectValueX, ref param_Calib.MachineHandBCOffectValue.Col);
                ControlTool.SetParamShow(nud_BCHandOffectValueY, ref param_Calib.MachineHandBCOffectValue.Row);
                ControlTool.SetParamShow(nud_BCHandOffectValueR, ref param_Calib.MachineHandBCOffectValue.Angle);

                ControlTool.SetParamShow(nud_BCDownOffectValueX_Up, ref param_Calib.MachineDownBCOffectValue_Up.Col);
                ControlTool.SetParamShow(nud_BCDownOffectValueY_Up, ref param_Calib.MachineDownBCOffectValue_Up.Row);
                ControlTool.SetParamShow(nud_BCDownOffectValueR_Up, ref param_Calib.MachineDownBCOffectValue_Up.Angle);
                ControlTool.SetParamShow(nud_BCUpOffectValueX_Up, ref param_Calib.MachineUpBCOffectValue_Up.Col);
                ControlTool.SetParamShow(nud_BCUpOffectValueY_Up, ref param_Calib.MachineUpBCOffectValue_Up.Row);
                ControlTool.SetParamShow(nud_BCUpOffectValueR_Up, ref param_Calib.MachineUpBCOffectValue_Up.Angle);

                //基准设置（所有相机都保存）
                ControlTool.SetParamShow(nud_BasePointX, ref param_Calib.BasePointX);
                ControlTool.SetParamShow(nud_BasePointY, ref param_Calib.BasePointY);
                ControlTool.SetParamShow(nud_BaseLineX1, ref param_Calib.BaseLineBeginCol);
                ControlTool.SetParamShow(nud_BaseLineY1, ref param_Calib.BaseLineBeginRow);
                ControlTool.SetParamShow(nud_BaseLineX2, ref param_Calib.BaseLineEndCol);
                ControlTool.SetParamShow(nud_BaseLineY2, ref param_Calib.BaseLineEndRow);

                ControlTool.SetParamShow(nud_CenterOfRotationX, ref param_Calib.CenterOfRotationX);
                ControlTool.SetParamShow(nud_CenterOfRotationY, ref param_Calib.CenterOfRotationY);


                //棋盘格标定(所有相机都保存)            
                ControlTool.SetParamShow(nud_CheckerboardCalibPointNum, ref param_Calib.CheckerboardCalibPointNum);

                //棋盘格标定(相机单独保存)              
                ControlTool.SetParamShow(nud_CheckerboardBoxMinThd, ref param_Calib.CheckerboardMinThd);
                ControlTool.SetParamShow(nud_CheckerboardBoxMaxThd, ref param_Calib.CheckerboardMaxThd);
                ControlTool.SetParamShow(nud_CheckerboardBoxSideLengthPix_X, ref param_Calib.CheckerboardBoxSideLengthPix_X);
                ControlTool.SetParamShow(nud_CheckerboardBoxSideLengthPix_Y, ref param_Calib.CheckerboardBoxSideLengthPix_Y);
                ControlTool.SetParamShow(cbx_MeasureTransition, ref param_Calib.LineModelParam.MeasureTransition);
                ControlTool.SetParamShow(cbx_MeasureSelect, ref param_Calib.LineModelParam.MeasureSelect);
                ControlTool.SetParamShow(nud_MesureThreshold, ref param_Calib.LineModelParam.MesureThreshold);
                ControlTool.SetParamShow(nud_CheckerboardfRoiStartRow, ref param_Calib.CheckerboardfRoiStartRow);
                ControlTool.SetParamShow(nud_CheckerboardfRoiStartCol, ref param_Calib.CheckerboardfRoiStartCol);
                ControlTool.SetParamShow(nud_CheckerboardfRoiEndRow, ref param_Calib.CheckerboardfRoiEndRow);
                ControlTool.SetParamShow(nud_CheckerboardfRoiEndCol, ref param_Calib.CheckerboardfRoiEndCol);
                ShowCheckerboardDatagridview(ref param_Calib);


                //平台标定(所有相机都保存)             
                ControlTool.SetParamShow(nud_PlatformTranslationCalibPointNum, ref param_Calib.PlatformTranslationCalibPointNum);
                ControlTool.SetParamShow(nud_PlatformRotateCalibPointNum, ref param_Calib.PlatformRotateCalibPointNum);
                ShowPlatformTranslationDatagridview(ref param_Calib);
                ShowPlatformRotateDatagridview(ref param_Calib);
                //平台标定参数设置(相机单独保存)                
                LoadLineParam(ref param_Calib);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }

        private void LoadLineParam(ref Param_PretargetingCalib param_Calib)
        {
            int currentFindILinendex = cob_SelectLine.SelectedIndex;
            if (currentFindILinendex < 0)
            {
                cob_SelectLine.SelectedIndex = 0;
                currentFindILinendex = 0;
            }
            ControlTool.SetParamShow(fRoiStartRow, ref param_Calib.findRectangleLineParams[currentFindILinendex].fRoiStartRow);
            ControlTool.SetParamShow(fRoiStartCol, ref param_Calib.findRectangleLineParams[currentFindILinendex].fRoiStartCol);
            ControlTool.SetParamShow(fRoiEndRow, ref param_Calib.findRectangleLineParams[currentFindILinendex].fRoiEndRow);
            ControlTool.SetParamShow(fRoiEndCol, ref param_Calib.findRectangleLineParams[currentFindILinendex].fRoiEndCol);
            ControlTool.SetParamShow(iCobDirection, ref param_Calib.findRectangleLineParams[currentFindILinendex].iCobDirection);
            ControlTool.SetParamShow(iCobinputPosition, ref param_Calib.findRectangleLineParams[currentFindILinendex].iCobinputPosition);
            ControlTool.SetParamShow(iCobinputTransition, ref param_Calib.findRectangleLineParams[currentFindILinendex].iCobinputTransition);
            ControlTool.SetParamShow(iCobInputXorY, ref param_Calib.findRectangleLineParams[currentFindILinendex].iCobInputXorY);
            ControlTool.SetParamShow(iInputAmplitudeThreshold, ref param_Calib.findRectangleLineParams[currentFindILinendex].iInputAmplitudeThreshold);
            ControlTool.SetParamShow(fInputSigma, ref param_Calib.findRectangleLineParams[currentFindILinendex].fInputSigma);
            ControlTool.SetParamShow(fInputRoiWidthLen, ref param_Calib.findRectangleLineParams[currentFindILinendex].fInputRoiWidthLen);
            ControlTool.SetParamShow(iInputInterval, ref param_Calib.findRectangleLineParams[currentFindILinendex].iInputInterval);

            ControlTool.SetParamShow(fModelRoiStartRow, ref param_Calib.fModelRoiStartRow);
            ControlTool.SetParamShow(fModelRoiStartCol, ref param_Calib.fModelRoiStartCol);
            ControlTool.SetParamShow(fModelRoiEndRow, ref param_Calib.fModelRoiEndRow);
            ControlTool.SetParamShow(fModelRoiEndCol, ref param_Calib.fModelRoiEndCol);
        }
        private void SaveParam()
        {
            try
            {
                //（所有相机都保存）
                foreach (var item in cameraManager)
                {
                    GetFormParamToAllCam(ref item.Value.param_PretargetingCalib);
                    item.Value.param_PretargetingCalib.Save();
                }
                //(相机单独保存)
                GetFormParamToOneCam(ref CurrentParam_PretargetingCalib);
                CurrentParam_PretargetingCalib.Save();

                //修改记录设置
                CurrentParam_PretargetingCalib.FilePath = (Project.Instance.logParam.LogSavePath + "\\ParamsChangeLog\\");
                string error = YControlAutobind.Saveing<Param_PretargetingCalib, IYCustomAttribute>(this, CurrentParam_PretargetingCalib);
                // 保存修改记录，保存后备份
                CurrentParam_PretargetingCalib.saveChangeLog(CurrentParam_PretargetingCalib, String.Format("预定位标定设置:"));
                CurrentParam_PretargetingCalib.backup(CurrentParam_PretargetingCalib);
                if (error != "") System.Windows.Forms.MessageBox.Show(error);

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(PopUpWindowsShow.ReplaceStringIgnoreCaseHalcon(ex.Message + ex.StackTrace));
                PopUpWindowsShow.MessageShowNotHalcon(ex.Message + ex.StackTrace);
            }

        }

        /// <summary>
        /// 获取界面参数至所有相机
        /// </summary>
        /// <param name="param_Calib"></param>
        private void GetFormParamToAllCam(ref Param_PretargetingCalib param_Calib)
        {
            try
            {
                //（所有相机都保存）
                ControlTool.GetParamSave(tb_CalibImgPath, ref param_Calib.CalibImgPath);

                //基准和补偿参数（所有相机都保存）
                ControlTool.GetParamSave(nud_MachineHandBCValueX, ref param_Calib.MachineHandBCValue.Col);
                ControlTool.GetParamSave(nud_MachineHandBCValueY, ref param_Calib.MachineHandBCValue.Row);
                ControlTool.GetParamSave(nud_MachineHandBCValueR, ref param_Calib.MachineHandBCValue.Angle);
                ControlTool.GetParamSave(cbx_NotBCValueX, ref param_Calib.NotBCValueX);
                ControlTool.GetParamSave(cbx_NotBCValueY, ref param_Calib.NotBCValueY);
                ControlTool.GetParamSave(cbx_NotBCValueR, ref param_Calib.NotBCValueR);

                ControlTool.GetParamSave(nud_BCDownOffectValueX, ref param_Calib.MachineDownBCOffectValue.Col);
                ControlTool.GetParamSave(nud_BCDownOffectValueY, ref param_Calib.MachineDownBCOffectValue.Row);
                ControlTool.GetParamSave(nud_BCDownOffectValueR, ref param_Calib.MachineDownBCOffectValue.Angle);
                ControlTool.GetParamSave(nud_BCUpOffectValueX, ref param_Calib.MachineUpBCOffectValue.Col);
                ControlTool.GetParamSave(nud_BCUpOffectValueY, ref param_Calib.MachineUpBCOffectValue.Row);
                ControlTool.GetParamSave(nud_BCUpOffectValueR, ref param_Calib.MachineUpBCOffectValue.Angle);

                ControlTool.GetParamSave(nud_BCHandOffectValueX, ref param_Calib.MachineHandBCOffectValue.Col);
                ControlTool.GetParamSave(nud_BCHandOffectValueY, ref param_Calib.MachineHandBCOffectValue.Row);
                ControlTool.GetParamSave(nud_BCHandOffectValueR, ref param_Calib.MachineHandBCOffectValue.Angle);

                ControlTool.GetParamSave(nud_BCDownOffectValueX_Up, ref param_Calib.MachineDownBCOffectValue_Up.Col);
                ControlTool.GetParamSave(nud_BCDownOffectValueY_Up, ref param_Calib.MachineDownBCOffectValue_Up.Row);
                ControlTool.GetParamSave(nud_BCDownOffectValueR_Up, ref param_Calib.MachineDownBCOffectValue_Up.Angle);
                ControlTool.GetParamSave(nud_BCUpOffectValueX_Up, ref param_Calib.MachineUpBCOffectValue_Up.Col);
                ControlTool.GetParamSave(nud_BCUpOffectValueY_Up, ref param_Calib.MachineUpBCOffectValue_Up.Row);
                ControlTool.GetParamSave(nud_BCUpOffectValueR_Up, ref param_Calib.MachineUpBCOffectValue_Up.Angle);

                //基准设置（所有相机都保存）
                ControlTool.GetParamSave(nud_BasePointX, ref param_Calib.BasePointX);
                ControlTool.GetParamSave(nud_BasePointY, ref param_Calib.BasePointY);
                ControlTool.GetParamSave(nud_BaseLineX1, ref param_Calib.BaseLineBeginCol);
                ControlTool.GetParamSave(nud_BaseLineY1, ref param_Calib.BaseLineBeginRow);
                ControlTool.GetParamSave(nud_BaseLineX2, ref param_Calib.BaseLineEndCol);
                ControlTool.GetParamSave(nud_BaseLineY2, ref param_Calib.BaseLineEndRow);

                ControlTool.GetParamSave(nud_CenterOfRotationX, ref param_Calib.CenterOfRotationX);
                ControlTool.GetParamSave(nud_CenterOfRotationY, ref param_Calib.CenterOfRotationY);


                //棋盘格标定(所有相机都保存)             
                ControlTool.GetParamSave(nud_CheckerboardCalibPointNum, ref param_Calib.CheckerboardCalibPointNum);



                //平台标定(所有相机都保存)              
                ControlTool.GetParamSave(nud_PlatformTranslationCalibPointNum, ref param_Calib.PlatformTranslationCalibPointNum);
                ControlTool.GetParamSave(nud_PlatformRotateCalibPointNum, ref param_Calib.PlatformRotateCalibPointNum);
                SavePlatformTranslationDatagridview(ref param_Calib);
                SavePlatformRotateDatagridview(ref param_Calib);

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// 获取界面参数至单个相机
        /// </summary>
        /// <param name="param_Calib"></param>
        private void GetFormParamToOneCam(ref Param_PretargetingCalib param_Calib)
        {
            try
            {
                //棋盘格标定(相机单独保存)              
                ControlTool.GetParamSave(nud_CheckerboardBoxMinThd, ref param_Calib.CheckerboardMinThd);
                ControlTool.GetParamSave(nud_CheckerboardBoxMaxThd, ref param_Calib.CheckerboardMaxThd);
                ControlTool.GetParamSave(nud_CheckerboardBoxSideLengthPix_X, ref param_Calib.CheckerboardBoxSideLengthPix_X);
                ControlTool.GetParamSave(nud_CheckerboardBoxSideLengthPix_Y, ref param_Calib.CheckerboardBoxSideLengthPix_Y);
                ControlTool.GetParamSave(cbx_MeasureTransition, ref param_Calib.LineModelParam.MeasureTransition);
                ControlTool.GetParamSave(cbx_MeasureSelect, ref param_Calib.LineModelParam.MeasureSelect);
                ControlTool.GetParamSave(nud_MesureThreshold, ref param_Calib.LineModelParam.MesureThreshold);
                ControlTool.GetParamSave(nud_CheckerboardfRoiStartRow, ref param_Calib.CheckerboardfRoiStartRow);
                ControlTool.GetParamSave(nud_CheckerboardfRoiStartCol, ref param_Calib.CheckerboardfRoiStartCol);
                ControlTool.GetParamSave(nud_CheckerboardfRoiEndRow, ref param_Calib.CheckerboardfRoiEndRow);
                ControlTool.GetParamSave(nud_CheckerboardfRoiEndCol, ref param_Calib.CheckerboardfRoiEndCol);
                SaveCheckerboardDatagridview(ref param_Calib);

                //平台标定参数设置(相机单独保存)
                int currentFindILinendex = cob_SelectLine.SelectedIndex;
                if (currentFindILinendex < 0)
                {
                    cob_SelectLine.SelectedIndex = 0;
                    currentFindILinendex = 0;
                }
                ControlTool.GetParamSave(fRoiStartRow, ref param_Calib.findRectangleLineParams[currentFindILinendex].fRoiStartRow);
                ControlTool.GetParamSave(fRoiStartCol, ref param_Calib.findRectangleLineParams[currentFindILinendex].fRoiStartCol);
                ControlTool.GetParamSave(fRoiEndRow, ref param_Calib.findRectangleLineParams[currentFindILinendex].fRoiEndRow);
                ControlTool.GetParamSave(fRoiEndCol, ref param_Calib.findRectangleLineParams[currentFindILinendex].fRoiEndCol);
                ControlTool.GetParamSave(iCobDirection, ref param_Calib.findRectangleLineParams[currentFindILinendex].iCobDirection);
                ControlTool.GetParamSave(iCobinputPosition, ref param_Calib.findRectangleLineParams[currentFindILinendex].iCobinputPosition);
                ControlTool.GetParamSave(iCobinputTransition, ref param_Calib.findRectangleLineParams[currentFindILinendex].iCobinputTransition);
                ControlTool.GetParamSave(iCobInputXorY, ref param_Calib.findRectangleLineParams[currentFindILinendex].iCobInputXorY);
                ControlTool.GetParamSave(iInputAmplitudeThreshold, ref param_Calib.findRectangleLineParams[currentFindILinendex].iInputAmplitudeThreshold);
                ControlTool.GetParamSave(fInputSigma, ref param_Calib.findRectangleLineParams[currentFindILinendex].fInputSigma);
                ControlTool.GetParamSave(fInputRoiWidthLen, ref param_Calib.findRectangleLineParams[currentFindILinendex].fInputRoiWidthLen);
                ControlTool.GetParamSave(iInputInterval, ref param_Calib.findRectangleLineParams[currentFindILinendex].iInputInterval);

                ControlTool.GetParamSave(fModelRoiStartRow, ref param_Calib.fModelRoiStartRow);
                ControlTool.GetParamSave(fModelRoiStartCol, ref param_Calib.fModelRoiStartCol);
                ControlTool.GetParamSave(fModelRoiEndRow, ref param_Calib.fModelRoiEndRow);
                ControlTool.GetParamSave(fModelRoiEndCol, ref param_Calib.fModelRoiEndCol);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }

        #endregion

        #region 基准和参数补偿
        private void cbx_NotBCValueX_Click(object sender, EventArgs e)
        {
            nud_MachineHandBCValueX.Value = -nud_MachineHandBCValueX.Value;
        }

        private void cbx_NotBCValueY_Click(object sender, EventArgs e)
        {
            nud_MachineHandBCValueY.Value = -nud_MachineHandBCValueY.Value;
        }

        private void cbx_NotBCValueR_Click(object sender, EventArgs e)
        {
            nud_MachineHandBCValueR.Value = -nud_MachineHandBCValueR.Value;
        }

        #endregion

        #region 基准设置
        private void btn_SetBaseStandard_Click(object sender, EventArgs e)
        {
            DialogResult result = System.Windows.Forms.MessageBox.Show($"是否更新当前拍照位置为基准", "询问", MessageBoxButtons.YesNo);
            if (result == DialogResult.No)
            {
                return;
            }


            ControlTool.SetParamShow(nud_BasePointX, ref CurrentParam_PretargetingCalib.BasePointXTemp);
            ControlTool.SetParamShow(nud_BasePointY, ref CurrentParam_PretargetingCalib.BasePointYTemp);
            ControlTool.SetParamShow(nud_BaseLineX1, ref CurrentParam_PretargetingCalib.BaseLineBeginColTemp);
            ControlTool.SetParamShow(nud_BaseLineY1, ref CurrentParam_PretargetingCalib.BaseLineBeginRowTemp);
            ControlTool.SetParamShow(nud_BaseLineX2, ref CurrentParam_PretargetingCalib.BaseLineEndColTemp);
            ControlTool.SetParamShow(nud_BaseLineY2, ref CurrentParam_PretargetingCalib.BaseLineEndRowTemp);
        }

        private void btn_SetBaseLine_Click(object sender, EventArgs e)
        {
            DialogResult result = System.Windows.Forms.MessageBox.Show($"是否更新基准线显示", "询问", MessageBoxButtons.YesNo);
            if (result == DialogResult.No)
            {
                return;
            }

            foreach (var item in cameraManager)
            {
                item.Value.param_PretargetingCalib.BaseLines = item.Value.param_PretargetingCalib.BaseLinesTemp.ToList();
            }
        }

        private void btn_OffsetXalibrationX_Click(object sender, EventArgs e)
        {
            DialogResult result = System.Windows.Forms.MessageBox.Show($"X偏移校准旋转中心Y", "询问", MessageBoxButtons.YesNo);
            if (result == DialogResult.No)
            {
                return;
            }


            double CenterOfRotationX = CurrentParam_PretargetingCalib.CenterOfRotationX;
            double CenterOfRotationY = CurrentParam_PretargetingCalib.CenterOfRotationY;
            double X = (double)nud_CurrentOffsetX.Value;
            double Y = (double)nud_CurrentOffsetY.Value;
            double R = (double)nud_CurrentOffsetR.Value;

            if (X == 0 || R == 0) return;
            HOperatorSet.TupleRad(R, out HTuple rad);
            CenterOfRotationY = CenterOfRotationY + (X / (Math.Sin(-rad.D)));

            ControlTool.SetParamShow(nud_CenterOfRotationY, ref CenterOfRotationY);
        }

        private void btn_OffsetXalibrationY_Click(object sender, EventArgs e)
        {
            DialogResult result = System.Windows.Forms.MessageBox.Show($"Y偏移校准旋转中心X", "询问", MessageBoxButtons.YesNo);
            if (result == DialogResult.No)
            {
                return;
            }
            double CenterOfRotationX = CurrentParam_PretargetingCalib.CenterOfRotationX;
            double CenterOfRotationY = CurrentParam_PretargetingCalib.CenterOfRotationY;
            double X = (double)nud_CurrentOffsetX.Value;
            double Y = (double)nud_CurrentOffsetY.Value;
            double R = (double)nud_CurrentOffsetR.Value;

            if (Y == 0 || R == 0) return;
            HOperatorSet.TupleRad(R, out HTuple rad);
            CenterOfRotationX = CenterOfRotationX + (Y / (Math.Sin(rad.D)));

            ControlTool.SetParamShow(nud_CenterOfRotationX, ref CenterOfRotationX);
        }

        #endregion

        #region 棋盘格标定
        /// <summary>
        /// 显示棋盘格标定坐标数据Datagridview
        /// </summary>
        public void ShowCheckerboardDatagridview(ref Param_PretargetingCalib param_Calib)
        {
            try
            {
                //显示棋盘格标定棋盘格坐标
                if (param_Calib.CheckerboardCalibrationCheckerboardCoord == null)
                    param_Calib.CheckerboardCalibrationCheckerboardCoord = new List<PointRotateInfo>();
                dgv_CheckerboardCalibrationCheckerboardCoord.Rows.Clear();
                for (int i = 0; i < param_Calib.CheckerboardCalibrationCheckerboardCoord.Count; i++)
                {
                    dgv_CheckerboardCalibrationCheckerboardCoord.Rows.Add(
                        param_Calib.CheckerboardCalibrationCheckerboardCoord[i].Row,
                        param_Calib.CheckerboardCalibrationCheckerboardCoord[i].Col);
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                MessageBox.Show(ex.ToString());
            }
        }
        /// <summary>
        /// 保存棋盘格标定坐标数据Datagridview
        /// </summary>
        public void SaveCheckerboardDatagridview(ref Param_PretargetingCalib param_Calib)
        {
            try
            {
                //保存棋盘格标定棋盘格坐标
                List<PointRotateInfo> CheckerboardCalibrationCheckerboardCoord = new List<PointRotateInfo>();
                for (int i = 0; i < dgv_CheckerboardCalibrationCheckerboardCoord.Rows.Count; i++)
                {
                    CheckerboardCalibrationCheckerboardCoord.Add(new PointRotateInfo()
                    {
                        Row = double.Parse(dgv_CheckerboardCalibrationCheckerboardCoord.Rows[i].Cells[0].Value.ToString()),
                        Col = double.Parse(dgv_CheckerboardCalibrationCheckerboardCoord.Rows[i].Cells[1].Value.ToString()),
                        Angle = 0,
                        IsRotate = false
                    });
                }
                param_Calib.CheckerboardCalibrationCheckerboardCoord = CheckerboardCalibrationCheckerboardCoord;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                MessageBox.Show("棋盘格标定坐标输入有误");
            }
        }
        private void nud_CheckerboardCalibPointNum_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                NDataGridView.ChangeDataGridviewRowNum(dgv_CheckerboardCalibrationCheckerboardCoord, (int)nud_CheckerboardCalibPointNum.Value);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                MessageBox.Show(ex.ToString());
            }
        }
        private void btn_CheckerboardCalibTransition_Click(object sender, EventArgs e)
        {
            try
            {

                HOperatorSet.AffineTransPoint2d(CurrentParam_PretargetingCalib.HomMat2D, (double)nud_CamCoordX.Value, (double)nud_CamCoordY.Value, out HTuple hrptxs, out HTuple hrptys);

                nud_WorldCoordX.Value = (decimal)hrptxs.D;
                nud_WorldCoordY.Value = (decimal)hrptys.D;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(PopUpWindowsShow.ReplaceStringIgnoreCaseHalcon(ex.Message + ex.StackTrace));
                PopUpWindowsShow.MessageShowNotHalcon(ex.Message + ex.StackTrace);
            }
        }
        #endregion

        #region 平台标定
        object currentCalibrationState_lockObj = new object();
        /// <summary>
        /// 当前标定状态
        /// </summary>
        CurrentCalibrationState _currentCalibrationState = CurrentCalibrationState.None;
        CurrentCalibrationState currentCalibrationState
        {
            get
            {
                return _currentCalibrationState;
            }
            set
            {
                lock (currentCalibrationState_lockObj)
                {
                    _currentCalibrationState = value;
                }
            }
        }

        public PointRotateInfo CurrentMachineHandBaseOffsetPoint = new PointRotateInfo();
        /// <summary>
        /// 当前发送平台平移偏移点索引
        /// </summary>
        int currentTranslationCalibrationPointIndex = 0;

        /// <summary>
        /// 当前发送平台旋转偏移点索引
        /// </summary>
        int currentRotateCalibrationPointIndex = 0;

        /// <summary>
        /// 当前相机的偏移点
        /// </summary>
        PointRotateInfo currentCamOffsetPoint = new PointRotateInfo();
        bool ExecuteOne = false;

        /// <summary>
        /// 显示平台平移标定坐标数据Datagridview
        /// </summary>
        public void ShowPlatformTranslationDatagridview(ref Param_PretargetingCalib param_Calib)
        {
            try
            {
                //显示平台标定平台坐标
                if (param_Calib.PlatformTranslationCalibPlatformCoord == null)
                    param_Calib.PlatformTranslationCalibPlatformCoord = new List<PointRotateInfo>();
                dgv_PlatformTranslationCalibPlatformCoord.Rows.Clear();
                for (int i = 0; i < param_Calib.PlatformTranslationCalibPlatformCoord.Count; i++)
                {
                    dgv_PlatformTranslationCalibPlatformCoord.Rows.Add(
                        param_Calib.PlatformTranslationCalibPlatformCoord[i].Row,
                        param_Calib.PlatformTranslationCalibPlatformCoord[i].Col);
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// 保存平台平移标定坐标数据Datagridview
        /// </summary>
        public void SavePlatformTranslationDatagridview(ref Param_PretargetingCalib param_Calib)
        {
            try
            {
                //保存棋盘格标定棋盘格坐标
                List<PointRotateInfo> PlatformCalibrationPlatformCoord = new List<PointRotateInfo>();
                for (int i = 0; i < dgv_PlatformTranslationCalibPlatformCoord.Rows.Count; i++)
                {
                    PlatformCalibrationPlatformCoord.Add(new PointRotateInfo()
                    {
                        Row = double.Parse(dgv_PlatformTranslationCalibPlatformCoord.Rows[i].Cells[0].Value.ToString()),
                        Col = double.Parse(dgv_PlatformTranslationCalibPlatformCoord.Rows[i].Cells[1].Value.ToString()),
                    });
                }
                param_Calib.PlatformTranslationCalibPlatformCoord = PlatformCalibrationPlatformCoord;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                MessageBox.Show("平台标定平移坐标输入有误");
            }
        }

        /// <summary>
        /// 显示平台旋转标定坐标数据Datagridview
        /// </summary>
        public void ShowPlatformRotateDatagridview(ref Param_PretargetingCalib param_Calib)
        {
            try
            {

                //显示平台标定平台坐标
                if (param_Calib.PlatformRotateCalibPlatformCoord == null)
                    param_Calib.PlatformRotateCalibPlatformCoord = new List<PointRotateInfo>();
                dgv_PlatformRotateCalibPlatformCoord.Rows.Clear();
                for (int i = 0; i < param_Calib.PlatformRotateCalibPlatformCoord.Count; i++)
                {
                    dgv_PlatformRotateCalibPlatformCoord.Rows.Add(
                        param_Calib.PlatformRotateCalibPlatformCoord[i].Row,
                        param_Calib.PlatformRotateCalibPlatformCoord[i].Col,
                        param_Calib.PlatformRotateCalibPlatformCoord[i].Angle);
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// 保存平台旋转标定坐标数据Datagridview
        /// </summary>
        public void SavePlatformRotateDatagridview(ref Param_PretargetingCalib param_Calib)
        {
            try
            {
                //保存棋盘格标定棋盘格坐标
                List<PointRotateInfo> PlatformCalibrationPlatformCoord = new List<PointRotateInfo>();
                for (int i = 0; i < dgv_PlatformRotateCalibPlatformCoord.Rows.Count; i++)
                {
                    PlatformCalibrationPlatformCoord.Add(new PointRotateInfo()
                    {
                        Row = double.Parse(dgv_PlatformRotateCalibPlatformCoord.Rows[i].Cells[0].Value.ToString()),
                        Col = double.Parse(dgv_PlatformRotateCalibPlatformCoord.Rows[i].Cells[1].Value.ToString()),
                        Angle = double.Parse(dgv_PlatformRotateCalibPlatformCoord.Rows[i].Cells[2].Value.ToString()),
                    });
                }
                param_Calib.PlatformRotateCalibPlatformCoord = PlatformCalibrationPlatformCoord;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                MessageBox.Show("平台标定旋转坐标输入有误");
            }
        }

        private void nud_PlatformTranslationCalibPointNum_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                NDataGridView.ChangeDataGridviewRowNum(dgv_PlatformTranslationCalibPlatformCoord, (int)nud_PlatformTranslationCalibPointNum.Value);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                MessageBox.Show(ex.ToString());
            }
        }

        private void nud_PlatformRotateCalibPointNum_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                NDataGridView.ChangeDataGridviewRowNum(dgv_PlatformRotateCalibPlatformCoord, (int)nud_PlatformRotateCalibPointNum.Value);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                MessageBox.Show(ex.ToString());
            }
        }
        private void btn_ExecuteOne_Click(object sender, EventArgs e)
        {
            ExecuteOne = true;
        }

        private void btn_ExitCalib_Click(object sender, EventArgs e)
        {
            cbx_CamSelect.Invoke(new Action(() =>
            {
                cbx_CamSelect.Enabled = true;
                cbx_CalibAlgorithmSelection.Enabled = true;
                btn_RunCurrentImg.Enabled = true;
                btn_Preview.Enabled = true;
                btn_Run.Enabled = true;
                btn_Next.Enabled = true;
                btn_ClearDetectionDatagridview.Enabled = true;
                nud_PlatformTranslationCalibPointNum.Enabled = true;
                nud_PlatformRotateCalibPointNum.Enabled = true;
            }));

            currentCalibrationState = CurrentCalibrationState.退出标定;
        }

        #endregion

        #region 平台标定参数
        private void cob_SelectLine_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                LoadLineParam(ref CurrentParam_PretargetingCalib);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                MessageBox.Show(ex.ToString());
            }
        }
        private void ROISET_Click(object sender, EventArgs e)
        {
            try
            {

                DisplayWindowClass displayWindow = GetCamHwindow();
                if (displayWindow.operation_ImageInfo?.showHalconWindow?.m_hImage == null) { MessageBox.Show("请先加载图像！"); return; };
                //获取图像信息
                HOperatorSet.CopyObj(displayWindow.operation_ImageInfo.showHalconWindow.m_hImage, out HObject ho_Image, 1, 1);
                ShowHalconWindow showHalconWindow = new ShowHalconWindow();
                showHalconWindow.m_hImage = ho_Image;
                showHalconWindow.m_messageShowInfos = new List<MessageShowInfo>();
                showHalconWindow.m_regionShowInfos = new List<RegionShowInfo>();
                showHalconWindow.m_lineShowInfos = new List<LineInfo>();
                displayWindow.operation_ImageInfo?.Dispose();

                HOperatorSet.SetColor(displayWindow.m_hWindowControl_Cam.HalconWindow, "blue");
                HOperatorSet.DrawRectangle1(displayWindow.m_hWindowControl_Cam.HalconWindow, out HTuple rowStart, out HTuple columnStart, out HTuple rowEnd, out HTuple columnEnd);
                HOperatorSet.GenRectangle1(out HObject rectangle, rowStart, columnStart, rowEnd, columnEnd);

                showHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = rectangle, m_stuGetColor = HalconColor.green });

                if (currentCalibAlgorithmEnum == CalibrationAlgorithmEnum.棋盘格)
                {
                    nud_CheckerboardfRoiStartRow.Value = Convert.ToDecimal(rowStart.D);
                    nud_CheckerboardfRoiStartCol.Value = Convert.ToDecimal(columnStart.D);
                    nud_CheckerboardfRoiEndRow.Value = Convert.ToDecimal(rowEnd.D);
                    nud_CheckerboardfRoiEndCol.Value = Convert.ToDecimal(columnEnd.D);
                }
                else
                {
                    fRoiStartRow.Value = Convert.ToDecimal(rowStart.D);
                    fRoiStartCol.Value = Convert.ToDecimal(columnStart.D);
                    fRoiEndRow.Value = Convert.ToDecimal(rowEnd.D);
                    fRoiEndCol.Value = Convert.ToDecimal(columnEnd.D);
                }


                displayWindow.RepaintCam(new Operation_ImageInfo() { showHalconWindow = showHalconWindow }, out Operation_ImageInfo screenshotImage);
                screenshotImage?.Dispose();


            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                MessageBox.Show("未生成ROI");
            }
        }

        private void Btn_RoiSet_Click(object sender, EventArgs e)
        {
            try
            {
                DisplayWindowClass displayWindow = GetCamHwindow();
                if (displayWindow.operation_ImageInfo?.showHalconWindow?.m_hImage == null) { MessageBox.Show("请先加载图像！"); return; };

                //获取图像信息
                HOperatorSet.CopyObj(displayWindow.operation_ImageInfo.showHalconWindow.m_hImage, out HObject ho_Image, 1, 1);
                ShowHalconWindow showHalconWindow = new ShowHalconWindow();
                showHalconWindow.m_hImage = ho_Image;
                showHalconWindow.m_messageShowInfos = new List<MessageShowInfo>();
                showHalconWindow.m_regionShowInfos = new List<RegionShowInfo>();
                showHalconWindow.m_lineShowInfos = new List<LineInfo>();
                displayWindow.operation_ImageInfo?.Dispose();

                HOperatorSet.SetColor(displayWindow.m_hWindowControl_Cam.HalconWindow, "blue");
                HOperatorSet.DrawRectangle1(displayWindow.m_hWindowControl_Cam.HalconWindow, out HTuple rowStart, out HTuple columnStart, out HTuple rowEnd, out HTuple columnEnd);
                HOperatorSet.GenRectangle1(out HObject rectangle, rowStart, columnStart, rowEnd, columnEnd);

                showHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = rectangle, m_stuGetColor = HalconColor.green });
                fModelRoiStartRow.Value = Convert.ToDecimal(rowStart.D);
                fModelRoiStartCol.Value = Convert.ToDecimal(columnStart.D);
                fModelRoiEndRow.Value = Convert.ToDecimal(rowEnd.D);
                fModelRoiEndCol.Value = Convert.ToDecimal(columnEnd.D);

                displayWindow.RepaintCam(new Operation_ImageInfo() { showHalconWindow = showHalconWindow }, out Operation_ImageInfo screenshotImage);
                screenshotImage?.Dispose();

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                MessageBox.Show("未生成ROI");
            }
        }

        private void btn_CreatMode_Click(object sender, EventArgs e)
        {
            try
            {
                DisplayWindowClass displayWindow = GetCamHwindow();
                if (displayWindow.operation_ImageInfo?.showHalconWindow?.m_hImage == null) { MessageBox.Show("请先加载图像！"); return; };

                //获取图像信息
                HObject ho_Image = displayWindow.operation_ImageInfo.showHalconWindow.m_hImage;

                HOperatorSet.GenRectangle1(out HObject rectangle, CurrentParam_PretargetingCalib.fModelRoiStartRow, CurrentParam_PretargetingCalib.fModelRoiStartCol, CurrentParam_PretargetingCalib.fModelRoiEndRow, CurrentParam_PretargetingCalib.fModelRoiEndCol);
                HOperatorSet.ReduceDomain(ho_Image, rectangle, out HObject model);

                HOperatorSet.CreateShapeModel(model, "auto", 0, (new HTuple(360)).TupleRad()
                 , "auto", "auto", "use_polarity", "auto", "auto", out HTuple model_handle);

                //创建模板成功，刷新模板图
                if (currentCameraEnum == CameraEnum.Cam1)
                {
                    TempCheckerboardModel_handle_Cam1 = model_handle;
                }
                else if (currentCameraEnum == CameraEnum.Cam2)
                {
                    TempCheckerboardModel_handle_Cam2 = model_handle;
                }
                else if (currentCameraEnum == CameraEnum.Cam3)
                {
                    TempCheckerboardModel_handle_Cam3 = model_handle;
                }
                else if (currentCameraEnum == CameraEnum.Cam4)
                {
                    TempCheckerboardModel_handle_Cam4 = model_handle;
                }
                CurrentParam_PretargetingCalib.Model_Image = model;
            }
            catch (Exception ex)
            {

                MessageBox.Show("模型创建失败！\n" + ex.Message);
            }
        }

        private void btn_SaveMode_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentCameraEnum == CameraEnum.Cam1)
                {
                    CurrentParam_PretargetingCalib.Model_handle = TempCheckerboardModel_handle_Cam1;
                }
                else if (currentCameraEnum == CameraEnum.Cam2)
                {
                    CurrentParam_PretargetingCalib.Model_handle = TempCheckerboardModel_handle_Cam2;
                }
                else if (currentCameraEnum == CameraEnum.Cam3)
                {
                    CurrentParam_PretargetingCalib.Model_handle = TempCheckerboardModel_handle_Cam3;
                }
                else if (currentCameraEnum == CameraEnum.Cam4)
                {
                    CurrentParam_PretargetingCalib.Model_handle = TempCheckerboardModel_handle_Cam4;
                }

                //保存模板图
                CurrentParam_PretargetingCalib.SaveModelHandle();

                //重新加载模板
                CurrentParam_PretargetingCalib.ReadModelHandle();

                MessageBox.Show("保存模板成功");
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                MessageBox.Show(ex.ToString());
            }
        }
        #endregion

        private void btn_Calib_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = System.Windows.Forms.MessageBox.Show($"是否标定", "询问", MessageBoxButtons.YesNo);
                if (result == DialogResult.No)
                {
                    return;
                }

                if (currentCalibAlgorithmEnum == CalibrationAlgorithmEnum.棋盘格)
                {

                    if (currentCameraEnum == CameraEnum.Cam1)
                    {
                        CalculateCheckerboardCalibrationData(cameraManager[CameraEnum.Cam1].m_CamParam.CameraName,
                          dataGridView_Cam1, dgv_CheckerboardCalibrationCheckerboardCoord, out TempCheckerboardHomMat2D_Cam1);
                    }
                    else if (currentCameraEnum == CameraEnum.Cam2)
                    {
                        CalculateCheckerboardCalibrationData(cameraManager[CameraEnum.Cam2].m_CamParam.CameraName,
                         dataGridView_Cam2, dgv_CheckerboardCalibrationCheckerboardCoord, out TempCheckerboardHomMat2D_Cam2);
                    }
                    else if (currentCameraEnum == CameraEnum.Cam3)
                    {
                        CalculateCheckerboardCalibrationData(cameraManager[CameraEnum.Cam3].m_CamParam.CameraName,
                         dataGridView_Cam3, dgv_CheckerboardCalibrationCheckerboardCoord, out TempCheckerboardHomMat2D_Cam3);
                    }
                    else if (currentCameraEnum == CameraEnum.Cam4)
                    {
                        CalculateCheckerboardCalibrationData(cameraManager[CameraEnum.Cam4].m_CamParam.CameraName,
                         dataGridView_Cam4, dgv_CheckerboardCalibrationCheckerboardCoord, out TempCheckerboardHomMat2D_Cam4);
                    }
                }
                else if (currentCalibAlgorithmEnum == CalibrationAlgorithmEnum.平移)
                {
                    CalculatePlatformTranslationCalibrationData(dataGridView_Cam2, cameraManager[CameraEnum.Cam2].param_PretargetingCalib.HomMat2D,
                                                                dataGridView_Cam3, cameraManager[CameraEnum.Cam3].param_PretargetingCalib.HomMat2D,
                                                                dgv_PlatformTranslationCalibPlatformCoord, out TempPlatformHomMat2D);

                }
                else if (currentCalibAlgorithmEnum == CalibrationAlgorithmEnum.旋转)
                {
                    CalculatePlatformRotateCalibrationData(dataGridView_Cam2, cameraManager[CameraEnum.Cam2].param_PretargetingCalib.HomMat2D,
                                                           dataGridView_Cam3, cameraManager[CameraEnum.Cam3].param_PretargetingCalib.HomMat2D,
                                                           dgv_PlatformRotateCalibPlatformCoord, out TempCenterOfRotation_X, out TempCenterOfRotation_Y, out double RadiusC);
                    ControlTool.SetParamShow(nud_CenterOfRotationX, ref TempCenterOfRotation_X);
                    ControlTool.SetParamShow(nud_CenterOfRotationY, ref TempCenterOfRotation_Y);
                }
                MessageBox.Show($"{currentCalibAlgorithmEnum}标定成功");
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                MessageBox.Show("标定失败");
            }
        }

        private void btn_SaveCheckerboardCalibrationChecker_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentCameraEnum == CameraEnum.Cam1)
                {
                    cameraManager[currentCameraEnum].param_PretargetingCalib.HomMat2D = TempCheckerboardHomMat2D_Cam1;
                    cameraManager[currentCameraEnum].param_PretargetingCalib.SaveCheckerboardCalibParam();
                }
                else if (currentCameraEnum == CameraEnum.Cam2)
                {
                    cameraManager[currentCameraEnum].param_PretargetingCalib.HomMat2D = TempCheckerboardHomMat2D_Cam2;
                    cameraManager[currentCameraEnum].param_PretargetingCalib.SaveCheckerboardCalibParam();
                }
                else if (currentCameraEnum == CameraEnum.Cam3)
                {
                    cameraManager[currentCameraEnum].param_PretargetingCalib.HomMat2D = TempCheckerboardHomMat2D_Cam3;
                    cameraManager[currentCameraEnum].param_PretargetingCalib.SaveCheckerboardCalibParam();
                }
                else if (currentCameraEnum == CameraEnum.Cam4)
                {
                    cameraManager[currentCameraEnum].param_PretargetingCalib.HomMat2D = TempCheckerboardHomMat2D_Cam4;
                    cameraManager[currentCameraEnum].param_PretargetingCalib.SaveCheckerboardCalibParam();
                }
                MessageBox.Show("保存成功");
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                MessageBox.Show("保存失败");
            }
        }

        private void btn_SavePlatformCalibrationChecker_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (var item in cameraManager)
                {
                    item.Value.param_PretargetingCalib.HomMat2DPlatform = TempPlatformHomMat2D;
                    item.Value.param_PretargetingCalib.SavePlatformCalibParam();
                }
                MessageBox.Show("保存成功");
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                MessageBox.Show("保存失败");
            }
        }

        /// <summary>
        /// 计算棋盘格标定数据
        /// </summary>
        /// <param name="camName"></param>
        /// <param name="dgv_Original"></param>
        /// <param name="dgv_Target"></param>
        /// <param name="homMat2D"></param>
        /// <returns></returns>
        public bool CalculateCheckerboardCalibrationData(string camName, DataGridView dgv_Original, DataGridView dgv_Target, out HTuple homMat2D)
        {
            homMat2D = 0;
            try
            {
                if (dgv_Original.Rows.Count != dgv_Target.Rows.Count)
                {
                    MessageBox.Show($"{camName}-检测数据表格和棋盘格标定坐标表格行数不一致！");
                    return false;
                }
                List<double> ptxs = new List<double>();
                List<double> ptys = new List<double>();
                List<double> ptxd = new List<double>();
                List<double> ptyd = new List<double>();
                for (int i = 0; i < dgv_Original.Rows.Count; i++)
                {
                    ptxs.Add(double.Parse(dgv_Original.Rows[i].Cells[1].Value.ToString()));
                    ptys.Add(double.Parse(dgv_Original.Rows[i].Cells[0].Value.ToString()));
                    ptxd.Add(double.Parse(dgv_Target.Rows[i].Cells[1].Value.ToString()));
                    ptyd.Add(double.Parse(dgv_Target.Rows[i].Cells[0].Value.ToString()));
                }
                //计算平移坐标系
                HOperatorSet.VectorToHomMat2d(ptxs.ToArray(), ptys.ToArray(), ptxd.ToArray(), ptyd.ToArray(), out homMat2D);
                HOperatorSet.HomMat2dInvert(homMat2D, out HTuple homMat2DInvert);

                MessageBox.Show($"{camName}-标定成功");
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                MessageBox.Show($"{camName}-标定失败");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 计算平台平移标定数据
        /// </summary>
        /// <param name="dgv_Original1"></param>
        /// <param name="dgv_Original2"></param>
        /// <param name="dgv_Target"></param>
        /// <param name="homMat2D"></param>
        /// <returns></returns>
        public bool CalculatePlatformTranslationCalibrationData(DataGridView dgv_Original1, HTuple HomMat2D1, DataGridView dgv_Original2, HTuple HomMat2D2, DataGridView dgv_Target, out HTuple homMat2D)
        {
            homMat2D = 0;
            try
            {
                if (dgv_Original1.Rows.Count != dgv_Target.Rows.Count || dgv_Original2.Rows.Count != dgv_Target.Rows.Count)
                {
                    MessageBox.Show($"检测数据表格和平台平移标定坐标表格行数不一致！");
                    return false;
                }
                List<double> ptxs = new List<double>();
                List<double> ptys = new List<double>();
                List<double> ptxd = new List<double>();
                List<double> ptyd = new List<double>();
                for (int i = 0; i < dgv_Original1.Rows.Count; i++)
                {
                    double x1 = double.Parse(dgv_Original1.Rows[i].Cells[1].Value.ToString());
                    double y1 = double.Parse(dgv_Original1.Rows[i].Cells[0].Value.ToString());
                    double x2 = double.Parse(dgv_Original2.Rows[i].Cells[1].Value.ToString());
                    double y2 = double.Parse(dgv_Original2.Rows[i].Cells[0].Value.ToString());
                    if (!ImageAlgorithm.GetAffinePoints2d(HomMat2D1, x1, y1, out double out_currentX1, out double out_currentY1)) { MessageBox.Show($"第一个矩阵转换失败"); return false; };
                    if (!ImageAlgorithm.GetAffinePoints2d(HomMat2D1, x2, y2, out double out_currentX2, out double out_currentY2)) { MessageBox.Show($"第二个矩阵转换失败"); return false; };
                    double x = (out_currentX1 + out_currentX2) / 2;
                    double y = (out_currentY1 + out_currentY2) / 2;

                    ptxs.Add(x);
                    ptys.Add(y);
                    ptxd.Add(double.Parse(dgv_Target.Rows[i].Cells[1].Value.ToString()));
                    ptyd.Add(double.Parse(dgv_Target.Rows[i].Cells[0].Value.ToString()));
                }
                //计算平移坐标系
                HOperatorSet.VectorToHomMat2d(ptxs.ToArray(), ptys.ToArray(), ptxd.ToArray(), ptyd.ToArray(), out homMat2D);
                HOperatorSet.HomMat2dInvert(homMat2D, out HTuple homMat2DInvert);


                MessageBox.Show($"标定成功");
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                MessageBox.Show($"标定失败");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 计算平台旋转标定数据
        /// </summary>
        /// <param name="dgv_Original1"></param>
        /// <param name="dgv_Original2"></param>
        /// <param name="dgv_Target"></param>
        /// <param name="homMat2D"></param>
        /// <returns></returns>
        public bool CalculatePlatformRotateCalibrationData(DataGridView dgv_Original1, HTuple HomMat2D1, DataGridView dgv_Original2, HTuple HomMat2D2, DataGridView dgv_Target, out double centerOfRotation_X, out double centerOfRotation_Y, out double RadiusC)
        {
            centerOfRotation_X = 0;
            centerOfRotation_Y = 0;
            RadiusC = 0;
            HObject hObject1 = null;
            HObject hObject2 = null;
            try
            {
                if (dgv_Original1.Rows.Count != dgv_Target.Rows.Count || dgv_Original2.Rows.Count != dgv_Target.Rows.Count)
                {
                    MessageBox.Show($"检测数据表格和平台旋转标定坐标表格行数不一致！");
                    return false;
                }
                HTuple ptxs1 = new HTuple();
                HTuple ptys1 = new HTuple();
                HTuple ptxs2 = new HTuple();
                HTuple ptys2 = new HTuple();

                for (int i = 0; i < dgv_Original1.Rows.Count; i++)
                {
                    double x1 = double.Parse(dgv_Original1.Rows[i].Cells[1].Value.ToString());
                    double y1 = double.Parse(dgv_Original1.Rows[i].Cells[0].Value.ToString());
                    double x2 = double.Parse(dgv_Original2.Rows[i].Cells[1].Value.ToString());
                    double y2 = double.Parse(dgv_Original2.Rows[i].Cells[0].Value.ToString());
                    if (!ImageAlgorithm.GetAffinePoints2d(HomMat2D1, x1, y1, out double out_currentX1, out double out_currentY1)) { MessageBox.Show($"第一个矩阵转换失败"); return false; };
                    if (!ImageAlgorithm.GetAffinePoints2d(HomMat2D1, x2, y2, out double out_currentX2, out double out_currentY2)) { MessageBox.Show($"第二个矩阵转换失败"); return false; };

                    ptxs1 = ptxs1.TupleConcat(out_currentX1);
                    ptys1 = ptys1.TupleConcat(out_currentY1);
                    ptxs2 = ptxs2.TupleConcat(out_currentX2);
                    ptys2 = ptys2.TupleConcat(out_currentY2);
                }
                HOperatorSet.GenContourPolygonXld(out hObject1, ptys1, ptxs1);
                HOperatorSet.FitCircleContourXld(hObject1, "algebraic", -1, 0, 0, 3, 2, out HTuple row1, out HTuple column1, out HTuple radius1, out HTuple startPhi1, out HTuple endPhi1, out HTuple pointOrder1);

                HOperatorSet.GenContourPolygonXld(out hObject2, ptys2, ptxs2);
                HOperatorSet.FitCircleContourXld(hObject2, "algebraic", -1, 0, 0, 3, 2, out HTuple row2, out HTuple column2, out HTuple radius2, out HTuple startPhi2, out HTuple endPhi2, out HTuple pointOrder2);

                centerOfRotation_X = (column1 + column2) / 2;
                centerOfRotation_Y = (row1 + row2) / 2;
                RadiusC = radius1;
                MessageBox.Show($"标定成功");
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                MessageBox.Show($"标定失败");
                return false;
            }
            finally
            {
                hObject1?.Dispose();
                hObject2?.Dispose();
            }

            return true;
        }
        #region 标定流程

        /// <summary>
        /// 标定线程
        /// </summary>
        public void ThdPretargetingCalibration()
        {
            try
            {

                //读取, //判断是否首次进入标定流程
                Project.Instance.cutOverlayPLCManage.Pretargeting_ReadCalibrationModel(out bool isCalibrationModel, out bool isReadDifferentState);
                if (isCalibrationModel && isReadDifferentState)
                {
                    DialogResult result = System.Windows.Forms.MessageBox.Show($"是否进入自动标定模式", "询问", MessageBoxButtons.YesNo);
                    if (result == DialogResult.No)
                    {
                        return;
                    }

                    currentCalibrationState = CurrentCalibrationState.标定中;
                    NLabel.UpdateControls(label5, "标定中...");

                    //读取拍照信号
                    Project.Instance.cutOverlayPLCManage.Pretargeting_ReadPhotoSignal(out bool isPhotoSignal);
                    if (isPhotoSignal)
                    {
                        //获取平台基准位置拍照结果
                        ProcessExecutionResult processExecutionResult = IntervalExecution.DelayExecute(DateTime.Now, 5000, GetResult);
                        if (processExecutionResult == ProcessExecutionResult.Success)
                        {
                            Project.Instance.cutOverlayPLCManage.CommunicationLog.Info("自动标定基准位置拍照完成");
                        }
                        //获取平台位置
                        Project.Instance.cutOverlayPLCManage.Pretargeting_ReadPlatformPos(ReadLogSaveType.True, out double PlatformX, out double PlatformY, out double PlatformR);
                        CurrentMachineHandBaseOffsetPoint.Col = PlatformX;
                        CurrentMachineHandBaseOffsetPoint.Row = PlatformY;
                        CurrentMachineHandBaseOffsetPoint.Angle = PlatformR;

                        //初始化                 
                        currentTranslationCalibrationPointIndex = 0;
                        currentRotateCalibrationPointIndex = 0;
                        CamCalibrationFinish_Cam1 = false;
                        CamCalibrationFinish_Cam2 = false;
                        CamCalibrationFinish_Cam3 = false;
                        CamCalibrationFinish_Cam4 = false;
                        NDataGridView.ClearRows(dataGridView_Cam1);
                        NDataGridView.ClearRows(dataGridView_Cam2);
                        NDataGridView.ClearRows(dataGridView_Cam3);
                        NDataGridView.ClearRows(dataGridView_Cam4);

                        cbx_CamSelect.Invoke(new Action(() =>
                        {
                            cbx_CamSelect.Enabled = false;
                            cbx_CalibAlgorithmSelection.Enabled = false;
                            btn_RunCurrentImg.Enabled = false;
                            btn_Preview.Enabled = false;
                            btn_Run.Enabled = false;
                            btn_Next.Enabled = false;
                            btn_ClearDetectionDatagridview.Enabled = false;
                            nud_PlatformTranslationCalibPointNum.Enabled = false;
                            nud_PlatformRotateCalibPointNum.Enabled = false;
                        }));

                        if (processExecutionResult == ProcessExecutionResult.Success)
                        {
                            //走平移坐标
                            if (currentCalibAlgorithmEnum == CalibrationAlgorithmEnum.平移)
                            {
                                if (dgv_PlatformTranslationCalibPlatformCoord.Rows.Count > currentTranslationCalibrationPointIndex)
                                {
                                    //获取平移点坐标
                                    double row = double.Parse(dgv_PlatformTranslationCalibPlatformCoord.Rows[currentTranslationCalibrationPointIndex].Cells[0].Value.ToString());
                                    double col = double.Parse(dgv_PlatformTranslationCalibPlatformCoord.Rows[currentTranslationCalibrationPointIndex].Cells[1].Value.ToString());

                                    //发送位置给PLC
                                    Project.Instance.cutOverlayPLCManage.Pretargeting_SendOffsetLocation(col, row, 0);
                                    currentTranslationCalibrationPointIndex++;

                                    string str = $"正在进行第平移{currentTranslationCalibrationPointIndex}个点的标定...";
                                    NLabel.UpdateControls(label5, str);
                                    Project.Instance.cutOverlayPLCManage.CommunicationLog.Info(str);

                                }
                            }
                            //走旋转坐标
                            else if (currentCalibAlgorithmEnum == CalibrationAlgorithmEnum.旋转)
                            {
                                //获取平移点坐标
                                double row = double.Parse(dgv_PlatformRotateCalibPlatformCoord.Rows[currentRotateCalibrationPointIndex].Cells[0].Value.ToString());
                                double col = double.Parse(dgv_PlatformRotateCalibPlatformCoord.Rows[currentRotateCalibrationPointIndex].Cells[1].Value.ToString());
                                double angle = double.Parse(dgv_PlatformRotateCalibPlatformCoord.Rows[currentRotateCalibrationPointIndex].Cells[2].Value.ToString());

                                //发送位置给PLC
                                Project.Instance.cutOverlayPLCManage.Pretargeting_SendOffsetLocation(col, row, angle);
                                currentRotateCalibrationPointIndex++;

                                string str = $"正在进行第旋转{currentRotateCalibrationPointIndex}个点的标定...";
                                NLabel.UpdateControls(label5, str);
                                Project.Instance.cutOverlayPLCManage.CommunicationLog.Info(str);
                            }

                            //发送标定结果OK
                            Project.Instance.cutOverlayPLCManage.Pretargeting_SendCalibrationResult(1);
                        }
                        else
                        {
                            currentCalibrationState = CurrentCalibrationState.标定失败;
                            Project.Instance.cutOverlayPLCManage.CommunicationLog.Info("获取图像结果超时");
                            Project.Instance.cutOverlayPLCManage.Pretargeting_SendCalibrationResult(2);
                        }
                    }

                }

                switch (currentCalibrationState)
                {
                    case CurrentCalibrationState.None:
                        break;
                    case CurrentCalibrationState.开始标定:
                        NLabel.UpdateControls(label5, "开始标定...");


                        break;
                    case CurrentCalibrationState.标定中:

                        //单步执行
                        if (cbx_ExecuteOne.Checked)
                        {
                            if (ExecuteOne)
                            {
                                ExecuteOne = false;
                            }
                            else
                            {
                                break;
                            }
                        }

                        //读取拍照信号
                        Project.Instance.cutOverlayPLCManage.Pretargeting_ReadPhotoSignal(out bool isPhotoSignal);
                        if (isPhotoSignal)
                        {
                            //获取平台位置                                   
                            Project.Instance.cutOverlayPLCManage.Pretargeting_ReadPlatformPos(ReadLogSaveType.True, out double PlatformX, out double PlatformY, out double PlatformR);

                            //获取图像结果
                            ProcessExecutionResult processExecutionResult = IntervalExecution.DelayExecute(DateTime.Now, 5000, GetResult);

                            if (processExecutionResult == ProcessExecutionResult.Success)
                            {

                                //走平移坐标
                                if (currentCalibAlgorithmEnum == CalibrationAlgorithmEnum.平移)
                                {
                                    if (dgv_PlatformTranslationCalibPlatformCoord.Rows.Count > currentTranslationCalibrationPointIndex)
                                    {
                                        //获取平移点坐标
                                        double row = double.Parse(dgv_PlatformTranslationCalibPlatformCoord.Rows[currentTranslationCalibrationPointIndex].Cells[0].Value.ToString());
                                        double col = double.Parse(dgv_PlatformTranslationCalibPlatformCoord.Rows[currentTranslationCalibrationPointIndex].Cells[1].Value.ToString());

                                        //发送位置给PLC
                                        Project.Instance.cutOverlayPLCManage.Pretargeting_SendOffsetLocation(col, row, 0);
                                        currentTranslationCalibrationPointIndex++;

                                        string str = $"正在进行第平移{currentTranslationCalibrationPointIndex}个点的标定...";
                                        NLabel.UpdateControls(label5, str);
                                        Project.Instance.cutOverlayPLCManage.CommunicationLog.Info(str);

                                        //发送标定结果OK
                                        Project.Instance.cutOverlayPLCManage.Pretargeting_SendCalibrationResult(1);

                                    }
                                    //标定完成
                                    else
                                    {
                                        currentCalibrationState = CurrentCalibrationState.标定数据获取完成;
                                        Project.Instance.cutOverlayPLCManage.CommunicationLog.Info("标定数据获取完成");
                                        Project.Instance.cutOverlayPLCManage.Pretargeting_SendCalibrationResult(3);
                                    }
                                }
                                //走旋转坐标
                                else if (currentCalibAlgorithmEnum == CalibrationAlgorithmEnum.旋转)
                                {

                                    if (dgv_PlatformRotateCalibPlatformCoord.Rows.Count > currentRotateCalibrationPointIndex)
                                    {
                                        //获取平移点坐标
                                        double row = double.Parse(dgv_PlatformRotateCalibPlatformCoord.Rows[currentRotateCalibrationPointIndex].Cells[0].Value.ToString());
                                        double col = double.Parse(dgv_PlatformRotateCalibPlatformCoord.Rows[currentRotateCalibrationPointIndex].Cells[1].Value.ToString());
                                        double angle = double.Parse(dgv_PlatformRotateCalibPlatformCoord.Rows[currentRotateCalibrationPointIndex].Cells[2].Value.ToString());

                                        //发送位置给PLC
                                        Project.Instance.cutOverlayPLCManage.Pretargeting_SendOffsetLocation(col, row, angle);
                                        currentRotateCalibrationPointIndex++;

                                        string str = $"正在进行第旋转{currentRotateCalibrationPointIndex}个点的标定...";
                                        NLabel.UpdateControls(label5, str);
                                        Project.Instance.cutOverlayPLCManage.CommunicationLog.Info(str);

                                        //发送标定结果OK
                                        Project.Instance.cutOverlayPLCManage.Pretargeting_SendCalibrationResult(1);
                                    }
                                    //标定完成
                                    else
                                    {
                                        currentCalibrationState = CurrentCalibrationState.标定数据获取完成;
                                        Project.Instance.cutOverlayPLCManage.CommunicationLog.Info("标定数据获取完成");
                                        Project.Instance.cutOverlayPLCManage.Pretargeting_SendCalibrationResult(3);
                                    }
                                }

                            }
                            else
                            {
                                currentCalibrationState = CurrentCalibrationState.标定失败;
                                Project.Instance.cutOverlayPLCManage.CommunicationLog.Info("获取图像结果超时");
                                Project.Instance.cutOverlayPLCManage.Pretargeting_SendCalibrationResult(2);

                            }
                        }
                        break;
                    case CurrentCalibrationState.标定数据获取完成:
                        string str1 = "标定数据获取完成，请进行标定...";
                        NLabel.UpdateControls(label5, str1);
                        break;
                    case CurrentCalibrationState.标定完成:
                        NLabel.UpdateControls(label5, "标定完成...");
                        break;
                    case CurrentCalibrationState.标定失败:
                        NLabel.UpdateControls(label5, "标定失败...");
                        break;
                    case CurrentCalibrationState.退出标定:
                        NLabel.UpdateControls(label5, "已退出标定...");
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                currentCalibrationState = CurrentCalibrationState.标定失败;
                Log4netRecord.ErrorLog.Error(ex);
            }
        }

        /// <summary>
        /// 获取图像结果
        /// </summary>
        /// <returns></returns>
        public ProcessExecutionResult GetResult()
        {
            try
            {
                //================================================================
                //【1】平台标定只需要标对角相机坐标即可
                if (CamCalibrationFinish_Cam2 == true &&
                    CamCalibrationFinish_Cam3 == true)
                {
                    CamCalibrationFinish_Cam2 = false;
                    CamCalibrationFinish_Cam3 = false;
                    return ProcessExecutionResult.Success;
                }

                return ProcessExecutionResult.ContinueExecute;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                return ProcessExecutionResult.Exception;
            }
        }

        public void CalibrationProcess_Cam1(CameraEnum camName, HObject ho_image)
        {
            try
            {
                HTuple Model_handle = cameraManager[CameraEnum.Cam1].param_PretargetingCalib.Model_handle;
                CalibrationProcess(camName, ho_image, true, currentCalibAlgorithmEnum, Model_handle, displayWindow_Cam1, dataGridView_Cam1);
                CamCalibrationFinish_Cam1 = true;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                MessageBox.Show(ex.ToString());
            }
        }

        public void CalibrationProcess_Cam2(CameraEnum camName, HObject ho_image)
        {
            try
            {
                HTuple Model_handle = cameraManager[CameraEnum.Cam2].param_PretargetingCalib.Model_handle;
                Log4netRecord.RunLog.Error("Cam2已启动");
                CalibrationProcess(camName, ho_image, true, currentCalibAlgorithmEnum, Model_handle, displayWindow_Cam2, dataGridView_Cam2);
                Log4netRecord.RunLog.Error("Cam2已结束");
                CamCalibrationFinish_Cam2 = true;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                MessageBox.Show(ex.ToString());
            }
        }

        public void CalibrationProcess_Cam3(CameraEnum camName, HObject ho_image)
        {
            try
            {
                HTuple Model_handle = cameraManager[CameraEnum.Cam3].param_PretargetingCalib.Model_handle;
                Log4netRecord.RunLog.Error("Cam3已启动");
                CalibrationProcess(camName, ho_image, true, currentCalibAlgorithmEnum, Model_handle, displayWindow_Cam3, dataGridView_Cam3);
                Log4netRecord.RunLog.Error("Cam3已结束");
                CamCalibrationFinish_Cam3 = true;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                MessageBox.Show(ex.ToString());
            }
        }

        public void CalibrationProcess_Cam4(CameraEnum camName, HObject ho_image)
        {
            try
            {
                HTuple Model_handle = cameraManager[CameraEnum.Cam4].param_PretargetingCalib.Model_handle;
                CalibrationProcess(camName, ho_image, true, currentCalibAlgorithmEnum, Model_handle, displayWindow_Cam4, dataGridView_Cam4);
                CamCalibrationFinish_Cam4 = true;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// 标定流程
        /// </summary>
        /// <param name="camName"></param>
        /// <param name="ho_image"></param>
        /// <param name="isSaveImage"></param>
        /// <param name="calibAlgorithmEnum"></param>
        /// <param name="displayWindow"></param>
        /// <param name="dataGridView"></param>
        private static void CalibrationProcess(CameraEnum camName, HObject ho_image, bool isSaveImage, CalibrationAlgorithmEnum calibAlgorithmEnum, HTuple model_handle, DisplayWindowClass displayWindow, DataGridView dataGridView)
        {
            ShowHalconWindow showHalconWindow = null;
            try
            {
                if (ho_image == null) return;

                //释放之前的图像资源
                displayWindow.operation_ImageInfo?.Dispose();
                Param_PretargetingCalib pretargetingCalibParam = cameraManager[camName].param_PretargetingCalib;
                List<PointRotateInfo> detectionImageCoord = new List<PointRotateInfo>();
                //string camNameStr = $"{camName}";

                //==============================================================
                //标定算法选择
                //棋盘格标定
                if (calibAlgorithmEnum == CalibrationAlgorithmEnum.棋盘格)
                {
                    //===============================
                    //【1】检测                
                    bool isSuccess = CheckerboardCalibrationArithmetic(camName, ho_image, pretargetingCalibParam, out showHalconWindow,
                          out double leftTop_RowCoord, out double leftTop_ColCoord, out double rightTop_RowCoord, out double rightTop_ColCoord,
                          out double leftBottom_RowCoord, out double leftBottom_ColCoord, out double rightBottom_RowCoord, out double rightBottom_ColCoord);

                    //===============================
                    //【2】显示数据
                    if (isSuccess)
                    {
                        detectionImageCoord.Add(new PointRotateInfo() { Row = leftBottom_RowCoord, Col = leftBottom_ColCoord });
                        detectionImageCoord.Add(new PointRotateInfo() { Row = rightBottom_RowCoord, Col = rightBottom_ColCoord });
                        detectionImageCoord.Add(new PointRotateInfo() { Row = rightTop_RowCoord, Col = rightTop_ColCoord });
                        detectionImageCoord.Add(new PointRotateInfo() { Row = leftTop_RowCoord, Col = leftTop_ColCoord });

                        //存放检测到得棋盘格标定坐标
                        pretargetingCalibParam.CheckerboardCalibrationImageCoord = detectionImageCoord;

                        //显示棋盘格标定图像坐标
                        NDataGridView.ClearRows(dataGridView);
                        for (int i = 0; i < detectionImageCoord.Count; i++)
                        {
                            NDataGridView.AddRow(dataGridView, new string[] { detectionImageCoord[i].Row.ToString("F3"), detectionImageCoord[i].Col.ToString("F3"), }, 100, false);
                        }
                    }

                    //===============================
                    //【3】保存标定图片
                    if (isSaveImage)
                    {
                        string path = CalibrationPath + $"{calibAlgorithmEnum}\\{camName}\\";
                        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                        string fileName = $"1_{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff")}" + SaveImageTool.GetImageFormat(ImageSaveFormat.BMP);
                        SaveImageTool.SaveImage(ho_image, ImageSaveFormat.BMP, path, fileName);
                    }
                }
                //平台平移标定
                //平台旋转标定
                else if (calibAlgorithmEnum == CalibrationAlgorithmEnum.平移 ||
                         calibAlgorithmEnum == CalibrationAlgorithmEnum.旋转)
                {
                    if (camName == CameraEnum.Cam2 || camName == CameraEnum.Cam3)
                    {
                        //===============================
                        //【1】检测
                        bool isSuccess = FindModelMethod(camName, ho_image, pretargetingCalibParam, model_handle, out showHalconWindow, out double row, out double col);

                        //===============================
                        //【2】显示数据
                        if (isSuccess)
                        {
                            detectionImageCoord.Add(new PointRotateInfo() { Row = row, Col = col });

                            //存放检测到得标定平移坐标
                            pretargetingCalibParam.PlatformTranslationCalibImageCoord = detectionImageCoord;

                            //显示标定平移图像坐标
                            for (int i = 0; i < detectionImageCoord.Count; i++)
                            {
                                NDataGridView.AddRow(dataGridView, new string[] { detectionImageCoord[i].Row.ToString("F3"), detectionImageCoord[i].Col.ToString("F3"), }, 100, false);
                            }
                        }
                    }
                    else
                    {
                        showHalconWindow = new ShowHalconWindow();
                        showHalconWindow.m_hImage = ho_image;
                    }


                    //===============================
                    //【3】保存标定图片
                    //获取平台位置
                    if (isSaveImage)
                    {
                        Project.Instance.cutOverlayPLCManage.Pretargeting_ReadPlatformPos(ReadLogSaveType.True, out double PlatformX, out double PlatformY, out double PlatformR);
                        string path = CalibrationPath + $"{calibAlgorithmEnum}\\{camName}\\";
                        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                        string fileName = $"X{PlatformX.ToString("F1")}_Y{PlatformY.ToString("F1")}_R{PlatformR.ToString("F1")}_{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff")}" + SaveImageTool.GetImageFormat(ImageSaveFormat.BMP);
                        SaveImageTool.SaveImage(ho_image, ImageSaveFormat.BMP, path, fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(PopUpWindowsShow.ReplaceStringIgnoreCaseHalcon(ex.Message));
                PopUpWindowsShow.MessageShowNotHalcon(ex.Message + ex.StackTrace);
            }

            try
            {
                //显示图像
                if (showHalconWindow != null)
                {
                    displayWindow.RepaintCam(new Operation_ImageInfo() { showHalconWindow = showHalconWindow }, out Operation_ImageInfo screenshotImage);
                    screenshotImage?.Dispose();
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(PopUpWindowsShow.ReplaceStringIgnoreCaseHalcon(ex.Message));
                PopUpWindowsShow.MessageShowNotHalcon(ex.Message + ex.StackTrace);
            }
        }

        #endregion

        #region 标定算法
        /// <summary>
        /// 棋盘格标定算法
        /// </summary>
        public static bool CheckerboardCalibrationArithmetic(CameraEnum camName, HObject ho_image, Param_PretargetingCalib CalibParam, out ShowHalconWindow showHalconWindow,
              out double leftTop_RowCoord, out double leftTop_ColCoord, out double rightTop_RowCoord, out double rightTop_ColCoord,
              out double leftBottom_RowCoord, out double leftBottom_ColCoord, out double rightBottom_RowCoord, out double rightBottom_ColCoord)
        {
            leftTop_RowCoord = 0;
            leftTop_ColCoord = 0;
            rightTop_RowCoord = 0;
            rightTop_ColCoord = 0;
            leftBottom_RowCoord = 0;
            leftBottom_ColCoord = 0;
            rightBottom_RowCoord = 0;
            rightBottom_ColCoord = 0;
            HObject rectangle = null;
            showHalconWindow = new ShowHalconWindow();
            try
            {
                showHalconWindow.m_hImage = ho_image;
                showHalconWindow.m_messageShowInfos = new List<MessageShowInfo>();
                showHalconWindow.m_regionShowInfos = new List<RegionShowInfo>();
                showHalconWindow.m_lineShowInfos = new List<LineInfo>();

                HOperatorSet.GetImageSize(ho_image, out HTuple width, out HTuple height);
                showHalconWindow.m_lineShowInfos.Add(new LineInfo() { RowBegin = 0, ColBegin = width / 2, RowEnd = height - 1, ColEnd = width / 2, Color = HalconColor.green });
                showHalconWindow.m_lineShowInfos.Add(new LineInfo() { RowBegin = height / 2, ColBegin = 0, RowEnd = height / 2, ColEnd = width - 1, Color = HalconColor.green });

                //预处理找棋盘格
                HOperatorSet.GenRectangle1(out rectangle, CalibParam.CheckerboardfRoiStartRow, CalibParam.CheckerboardfRoiStartCol, CalibParam.CheckerboardfRoiEndRow, CalibParam.CheckerboardfRoiEndCol);
                HOperatorSet.ReduceDomain(ho_image, rectangle, out HObject ho_image_pre);
                ImageAlgorithm.FindCheckerboardPretreatment(ho_image_pre, CalibParam.CheckerboardBoxSideLengthPix_X, CalibParam.CheckerboardBoxSideLengthPix_Y, CalibParam.CheckerboardMinThd, CalibParam.CheckerboardMaxThd, out HObject ho_Region);
                HOperatorSet.SmallestRectangle1(ho_Region, out HTuple row1, out HTuple col1, out HTuple row2, out HTuple col2);
                showHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = ho_Region, m_stuGetColor = HalconColor.red });

                //找边线和角点
                List<PointInfo> imageCoordinatePointInfos = new List<PointInfo>();
                List<PointInfo> worldCoordinatePointInfos = new List<PointInfo>();
                for (int i = 0; i < ho_Region.CountObj(); i++)
                {
                    HObject ho_RegionTemp = ho_Region.SelectObj(i + 1);
                    ImageAlgorithm.FindRectangleEdge(ho_image, ho_RegionTemp, MeasureTransitionEnum.不区分明暗,
                    out double leftEdgeBeginRow, out double leftEdgeBeginCol, out double leftEdgeEndRow, out double leftEdgeEndCol,
                    out double topEdgeBeginRow, out double topEdgeBeginCol, out double topEdgeEndRow, out double topEdgeEndCol,
                    out double rightEdgeBeginRow, out double rightEdgeBeginCol, out double rightEdgeEndRow, out double rightEdgeEndCol,
                    out double bottomEdgeBeginRow, out double bottomEdgeBeginCol, out double bottomEdgeEndRow, out double bottomEdgeEndCol,
                    out double leftTop_Row, out double leftTop_Col, out double rightTop_Row, out double rightTop_Col,
                    out double leftBottom_Row, out double leftBottom_Col, out double rightBottom_Row, out double rightBottom_Col);

                    if (i == 0)
                    {

                        HOperatorSet.IntersectionLines(leftTop_Row, leftTop_Col, rightBottom_Row, rightBottom_Col,
                                                       rightTop_Row, rightTop_Col, leftBottom_Row, leftBottom_Col, out HTuple row, out HTuple col, out HTuple isOverlapping);

                        //输出第一个棋盘格方格四个角点坐标
                        leftTop_RowCoord = leftTop_Row;
                        leftTop_ColCoord = leftTop_Col;
                        rightTop_RowCoord = rightTop_Row;
                        rightTop_ColCoord = rightTop_Col;
                        leftBottom_RowCoord = leftBottom_Row;
                        leftBottom_ColCoord = leftBottom_Col;
                        rightBottom_RowCoord = rightBottom_Row;
                        rightBottom_ColCoord = rightBottom_Col;


                        showHalconWindow.m_lineShowInfos.Add(new LineInfo() { RowBegin = leftTop_Row, ColBegin = leftTop_Col, RowEnd = rightBottom_Row, ColEnd = rightBottom_Col, Color = HalconColor.green });
                        showHalconWindow.m_lineShowInfos.Add(new LineInfo() { RowBegin = rightTop_Row, ColBegin = rightTop_Col, RowEnd = leftBottom_Row, ColEnd = leftBottom_Col, Color = HalconColor.green });
                    }


                    //添加点坐标
                    imageCoordinatePointInfos.Add(new PointInfo() { Row = leftTop_Row, Col = leftTop_Col });
                    imageCoordinatePointInfos.Add(new PointInfo() { Row = rightTop_Row, Col = rightTop_Col });
                    imageCoordinatePointInfos.Add(new PointInfo() { Row = leftBottom_Row, Col = leftBottom_Col });
                    imageCoordinatePointInfos.Add(new PointInfo() { Row = rightBottom_Row, Col = rightBottom_Col });

                    showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"R:{leftTop_Row.ToString("F3")}_C:{leftTop_Col.ToString("F3")}", m_fRow = leftTop_Row, m_fCol = leftTop_Col, m_bIsWindow = false, m_stuGetColor = HalconColor.blue });
                    showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"R:{rightTop_Row.ToString("F3")}_C:{rightTop_Col.ToString("F3")}", m_fRow = rightTop_Row, m_fCol = rightTop_Col, m_bIsWindow = false, m_stuGetColor = HalconColor.blue });
                    showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"R:{leftBottom_Row.ToString("F3")}_C:{leftBottom_Col.ToString("F3")}", m_fRow = leftBottom_Row - 3, m_fCol = leftBottom_Col, m_bIsWindow = false, m_stuGetColor = HalconColor.blue });
                    showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"R:{rightBottom_Row.ToString("F3")}_C:{rightBottom_Col.ToString("F3")}", m_fRow = rightBottom_Row - 3, m_fCol = rightBottom_Col, m_bIsWindow = false, m_stuGetColor = HalconColor.blue });

                    //显示角点
                    HOperatorSet.GenCrossContourXld(out HObject ho_Cross, leftTop_Row, leftTop_Col, 6, 45);
                    showHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = ho_Cross, m_stuGetColor = HalconColor.red });
                    HOperatorSet.GenCrossContourXld(out ho_Cross, rightTop_Row, rightTop_Col, 6, 45);
                    showHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = ho_Cross, m_stuGetColor = HalconColor.red });
                    HOperatorSet.GenCrossContourXld(out ho_Cross, leftBottom_Row, leftBottom_Col, 6, 45);
                    showHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = ho_Cross, m_stuGetColor = HalconColor.red });
                    HOperatorSet.GenCrossContourXld(out ho_Cross, rightBottom_Row, rightBottom_Col, 6, 45);
                    showHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = ho_Cross, m_stuGetColor = HalconColor.red });

                    //显示边线
                    showHalconWindow.m_lineShowInfos.Add(new LineInfo() { RowBegin = leftEdgeBeginRow, ColBegin = leftEdgeBeginCol, RowEnd = leftEdgeEndRow, ColEnd = leftEdgeEndCol, Color = HalconColor.green });
                    showHalconWindow.m_lineShowInfos.Add(new LineInfo() { RowBegin = topEdgeBeginRow, ColBegin = topEdgeBeginCol, RowEnd = topEdgeEndRow, ColEnd = topEdgeEndCol, Color = HalconColor.green });
                    showHalconWindow.m_lineShowInfos.Add(new LineInfo() { RowBegin = rightEdgeBeginRow, ColBegin = rightEdgeBeginCol, RowEnd = rightEdgeEndRow, ColEnd = rightEdgeEndCol, Color = HalconColor.green });
                    showHalconWindow.m_lineShowInfos.Add(new LineInfo() { RowBegin = bottomEdgeBeginRow, ColBegin = bottomEdgeBeginCol, RowEnd = bottomEdgeEndRow, ColEnd = bottomEdgeEndCol, Color = HalconColor.green });

                }
            }
            catch (Exception ex)
            {

                Log4netRecord.ErrorLog.Error(camName.ToString(), ex);
                return false;
            }
            finally
            {
                rectangle?.Dispose();
            }

            return true;
        }


        /// <summary>
        /// 模板匹配找极片角点
        /// </summary>
        /// <param name="hoImage"></param>
        /// <param name="param_PretargetingCalib"></param>
        /// <param name="model_handle"></param>
        /// <param name="showHalconWindow"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public static bool FindModelMethod(CameraEnum camName, HObject hoImage, Param_PretargetingCalib param_PretargetingCalib, HTuple model_handle, out ShowHalconWindow showHalconWindow, out double row, out double col)
        {
            showHalconWindow = new ShowHalconWindow();
            showHalconWindow.m_hImage = hoImage;
            showHalconWindow.m_messageShowInfos = new List<MessageShowInfo>();
            showHalconWindow.m_regionShowInfos = new List<RegionShowInfo>();
            showHalconWindow.m_lineShowInfos = new List<LineInfo>();
            try
            {
                HOperatorSet.FindShapeModel(hoImage, model_handle, 0, (new HTuple(360)).TupleRad()
                   , 0.4, 1, 0.5, "least_squares", 0, 0.9, out HTuple hv_Row, out HTuple hv_Column, out HTuple hv_Angle,
                 out HTuple hv_Score);
                HOperatorSet.GenRectangle1(out HObject rectangle, param_PretargetingCalib.fModelRoiStartRow, param_PretargetingCalib.fModelRoiStartCol, param_PretargetingCalib.fModelRoiEndRow, param_PretargetingCalib.fModelRoiEndCol);
                // showHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = rectangle, m_stuGetColor = HalconColor.green });
                HOperatorSet.AreaCenter(rectangle, out HTuple hv_Area2, out HTuple hv_Row2, out HTuple hv_Column2);

                HOperatorSet.VectorAngleToRigid(hv_Row2, hv_Column2, 0, hv_Row, hv_Column, hv_Angle, out HTuple hv_HomMat2D);

                HOperatorSet.GenRectangle1(out HObject rectangleX, param_PretargetingCalib.findRectangleLineParams[0].fRoiStartRow, param_PretargetingCalib.findRectangleLineParams[0].fRoiStartCol,
                      param_PretargetingCalib.findRectangleLineParams[0].fRoiEndRow, param_PretargetingCalib.findRectangleLineParams[0].fRoiEndCol);
                HOperatorSet.GenRectangle1(out HObject rectangleY, param_PretargetingCalib.findRectangleLineParams[1].fRoiStartRow, param_PretargetingCalib.findRectangleLineParams[1].fRoiStartCol,
                      param_PretargetingCalib.findRectangleLineParams[1].fRoiEndRow, param_PretargetingCalib.findRectangleLineParams[1].fRoiEndCol);

                HOperatorSet.AreaCenter(rectangleX, out HTuple hv_AreaX, out HTuple hv_RowX, out HTuple hv_ColumnX);
                HOperatorSet.AreaCenter(rectangleY, out HTuple hv_AreaY, out HTuple hv_RowY, out HTuple hv_ColumnY);
                // HOperatorSet.VectorAngleToRigid(hv_RowX, hv_ColumnX, 0, hv_Row,
                //hv_Column, hv_Angle, out HTuple hv_HomMat2DX);

                // HOperatorSet.VectorAngleToRigid(hv_RowY, hv_ColumnY, 0, hv_Row,
                //             hv_Column, hv_Angle, out HTuple hv_HomMat2DY);
                HOperatorSet.AffineTransRegion(rectangle, out rectangle, hv_HomMat2D,
                    "nearest_neighbor");
                HOperatorSet.AffineTransRegion(rectangleX, out rectangleX, hv_HomMat2D,
                    "nearest_neighbor");
                HOperatorSet.AffineTransRegion(rectangleY, out rectangleY, hv_HomMat2D,
                   "nearest_neighbor");
                showHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = rectangle, m_stuGetColor = HalconColor.yellow });
                showHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = rectangleX, m_stuGetColor = HalconColor.green });
                showHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = rectangleY, m_stuGetColor = HalconColor.green });

                Find_Line_ForRectangle(hoImage, rectangleX, out HObject ho_outputContourX, dicCobInputXorY[param_PretargetingCalib.findRectangleLineParams[0].iCobInputXorY],
                    param_PretargetingCalib.findRectangleLineParams[0].iInputInterval, dicCobDirection[param_PretargetingCalib.findRectangleLineParams[0].iCobDirection],
                     param_PretargetingCalib.findRectangleLineParams[0].fInputRoiWidthLen, param_PretargetingCalib.findRectangleLineParams[0].fInputSigma,
                      param_PretargetingCalib.findRectangleLineParams[0].iInputAmplitudeThreshold,
                      dicCobinputTransition[param_PretargetingCalib.findRectangleLineParams[0].iCobinputTransition],
                      dicCobinputPosition[param_PretargetingCalib.findRectangleLineParams[0].iCobinputPosition], out HTuple hv_OutPointRowX,
                   out HTuple hv_OutPointColX);
                showHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = ho_outputContourX, m_stuGetColor = HalconColor.orange });

                HOperatorSet.GenCrossContourXld(out HObject ho_CrossX, hv_OutPointRowX, hv_OutPointColX, 100, (new HTuple(45)).TupleRad());
                showHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = ho_CrossX, m_stuGetColor = HalconColor.blue });

                HOperatorSet.FitLineContourXld(ho_outputContourX, "tukey", -1, 0, 5, 2, out HTuple hv_RowBeginX,
                out HTuple hv_ColBeginX, out HTuple hv_RowEndX, out HTuple hv_ColEndX, out HTuple hv_NrX, out HTuple hv_NcX,
                out HTuple hv_DistX);

                Find_Line_ForRectangle(hoImage, rectangleY, out HObject ho_outputContourY, dicCobInputXorY[param_PretargetingCalib.findRectangleLineParams[1].iCobInputXorY],
                  param_PretargetingCalib.findRectangleLineParams[1].iInputInterval, dicCobDirection[param_PretargetingCalib.findRectangleLineParams[1].iCobDirection],
                   param_PretargetingCalib.findRectangleLineParams[1].fInputRoiWidthLen, param_PretargetingCalib.findRectangleLineParams[0].fInputSigma,
                    param_PretargetingCalib.findRectangleLineParams[1].iInputAmplitudeThreshold,
                    dicCobinputTransition[param_PretargetingCalib.findRectangleLineParams[1].iCobinputTransition],
                    dicCobinputPosition[param_PretargetingCalib.findRectangleLineParams[1].iCobinputPosition], out HTuple hv_OutPointRowY,
                 out HTuple hv_OutPointColY);
                showHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = ho_outputContourY, m_stuGetColor = HalconColor.orange });

                HOperatorSet.GenCrossContourXld(out HObject ho_CrossY, hv_OutPointRowY, hv_OutPointColY, 100, (new HTuple(45)).TupleRad());
                showHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = ho_CrossY, m_stuGetColor = HalconColor.blue });

                HOperatorSet.FitLineContourXld(ho_outputContourY, "tukey", -1, 0, 5, 2, out HTuple hv_RowBeginY,
                out HTuple hv_ColBeginY, out HTuple hv_RowEndY, out HTuple hv_ColEndY, out HTuple hv_NrY, out HTuple hv_NcY,
                out HTuple hv_DistY);

                HOperatorSet.IntersectionLl(hv_RowBeginX, hv_ColBeginX, hv_RowEndX, hv_ColEndX, hv_RowBeginY, hv_ColBeginY, hv_RowEndY, hv_ColEndY, out HTuple hv_ouputY,
                    out HTuple hv_ouputX, out HTuple hv_IsParallel);

                HOperatorSet.GenCrossContourXld(out HObject ho_Cross, hv_ouputY, hv_ouputX, 100, (new HTuple(45)).TupleRad());
                showHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = ho_Cross, m_stuGetColor = HalconColor.orange });
                row = hv_ouputY.D;
                col = hv_ouputX.D;

                return true;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(camName.ToString(), ex);
                MessageBox.Show("模型定位失败");
                row = 0;
                col = 0;
                return false;
            }

        }

        public static void Find_Line_ForRectangle(HObject ho_inputImage, HObject ho_inputRegion,
                       out HObject ho_outputContour, HTuple hv_inputXorY, HTuple hv_inputInterval,
                       HTuple hv_inputDirection, HTuple hv_inputRoiWidthLen, HTuple hv_inputSigma,
                       HTuple hv_inputAmplitudeThreshold, HTuple hv_inputTransition, HTuple hv_inputPosition,
                       out HTuple hv_OutPointRow, out HTuple hv_OutPointCol)
        {




            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_ProcessContour, ho_Rectangle1 = null;
            HObject ho_Cross1 = null, ho_Cross3 = null, ho_Cross2 = null;
            HObject ho_ResultContour = null, ho_Cross = null;

            // Local control variables 

            HTuple hv_Row11 = new HTuple(), hv_Column1 = new HTuple();
            HTuple hv_Row21 = new HTuple(), hv_Column2 = new HTuple();
            HTuple hv_Length1 = new HTuple(), hv_Length2 = new HTuple();
            HTuple hv_Row = new HTuple(), hv_Column = new HTuple();
            HTuple hv_r2 = new HTuple(), hv_c2 = new HTuple(), hv_r4 = new HTuple();
            HTuple hv_c4 = new HTuple(), hv_Row1 = new HTuple(), hv_Col1 = new HTuple();
            HTuple hv_Row2 = new HTuple(), hv_Col2 = new HTuple();
            HTuple hv_LineRowStart_Measure_01_0 = new HTuple(), hv_LineColumnStart_Measure_01_0 = new HTuple();
            HTuple hv_LineRowEnd_Measure_01_0 = new HTuple(), hv_LineColumnEnd_Measure_01_0 = new HTuple();
            HTuple hv_MeasureLength1 = new HTuple(), hv_r1 = new HTuple();
            HTuple hv_c1 = new HTuple(), hv_r3 = new HTuple(), hv_c3 = new HTuple();
            HTuple hv_Width = new HTuple(), hv_Height = new HTuple();
            HTuple hv_MetrologyHandle = new HTuple(), hv_Index1 = new HTuple();
            HTuple hv_LineParameter = new HTuple(), hv_DebugMode = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_outputContour);
            HOperatorSet.GenEmptyObj(out ho_ProcessContour);
            HOperatorSet.GenEmptyObj(out ho_Rectangle1);
            HOperatorSet.GenEmptyObj(out ho_Cross1);
            HOperatorSet.GenEmptyObj(out ho_Cross3);
            HOperatorSet.GenEmptyObj(out ho_Cross2);
            HOperatorSet.GenEmptyObj(out ho_ResultContour);
            HOperatorSet.GenEmptyObj(out ho_Cross);
            hv_OutPointRow = new HTuple();
            hv_OutPointCol = new HTuple();
            try
            {

                //inputXorY:沿Y轴搜索,或者X轴
                //inputInterval:间隔
                //inputDirection:方向 L2R_New从左到右,U2B_New从上到下
                //inputRoiWidthLen:卡尺ROI长度
                //inputSigma:平滑系数
                //inputAmplitudeThreshold:阈值
                //inputTransition:所以’negative‘从亮到暗，‘positive’从暗到亮，‘all’。
                //inputPosition:'first'第一点,'last'最后一个,'all' 所有的边缘


                hv_OutPointRow.Dispose();
                hv_OutPointRow = new HTuple();
                hv_OutPointCol.Dispose();
                hv_OutPointCol = new HTuple();
                ho_ProcessContour.Dispose();
                HOperatorSet.GenEmptyObj(out ho_ProcessContour);

                //*****************************************二维卡尺方法**************************
                if ((int)((new HTuple((new HTuple((new HTuple(hv_inputDirection.TupleEqual(
                    "L2R_New"))).TupleOr(new HTuple(hv_inputDirection.TupleEqual("R2L_New"))))).TupleOr(
                    new HTuple(hv_inputDirection.TupleEqual("U2B_New"))))).TupleOr(new HTuple(hv_inputDirection.TupleEqual(
                    "B2U_New")))) != 0)
                {
                    hv_Row11.Dispose(); hv_Column1.Dispose(); hv_Row21.Dispose(); hv_Column2.Dispose();
                    HOperatorSet.SmallestRectangle1(ho_inputRegion, out hv_Row11, out hv_Column1,
                        out hv_Row21, out hv_Column2);
                    hv_Length1.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Length1 = (((hv_Column2 - hv_Column1)).TupleAbs()
                            ) / 2;
                    }
                    hv_Length2.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Length2 = (((hv_Row21 - hv_Row11)).TupleAbs()
                            ) / 2;
                    }
                    hv_Row.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Row = (hv_Row21 + hv_Row11) / 2;
                    }
                    hv_Column.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Column = (hv_Column2 + hv_Column1) / 2;
                    }
                    ho_Rectangle1.Dispose();
                    HOperatorSet.GenRectangle2(out ho_Rectangle1, hv_Row, hv_Column, 0, hv_Length1,
                        hv_Length2);
                    //an := rad(Phi)
                    if ((int)(new HTuple(hv_inputXorY.TupleEqual("Y"))) != 0)
                    {
                        hv_r2.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_r2 = hv_Row - hv_Length2;
                        }
                        hv_c2.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_c2 = hv_Column - hv_Length1;
                        }
                        hv_r4.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_r4 = hv_Row + hv_Length2;
                        }
                        hv_c4.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_c4 = hv_Column + hv_Length1;
                        }
                        ho_Cross1.Dispose();
                        HOperatorSet.GenCrossContourXld(out ho_Cross1, hv_r2, hv_c2, 100, 0);
                        ho_Cross3.Dispose();
                        HOperatorSet.GenCrossContourXld(out ho_Cross3, hv_r4, hv_c4, 100, 0);
                        hv_Row1.Dispose();
                        hv_Row1 = new HTuple(hv_r2);
                        hv_Col1.Dispose();
                        hv_Col1 = new HTuple(hv_c2);
                        hv_Row2.Dispose();
                        hv_Row2 = new HTuple(hv_r4);
                        hv_Col2.Dispose();
                        hv_Col2 = new HTuple(hv_c4);

                        if ((int)(new HTuple(hv_inputDirection.TupleEqual("L2R_New"))) != 0)
                        {
                            hv_LineRowStart_Measure_01_0.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_LineRowStart_Measure_01_0 = hv_Row2 + (hv_inputRoiWidthLen / 2);
                            }
                            hv_LineColumnStart_Measure_01_0.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_LineColumnStart_Measure_01_0 = (hv_Col1 + hv_Col2) / 2;
                            }
                            hv_LineRowEnd_Measure_01_0.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_LineRowEnd_Measure_01_0 = hv_Row1 - (hv_inputRoiWidthLen / 2);
                            }
                            hv_LineColumnEnd_Measure_01_0.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_LineColumnEnd_Measure_01_0 = (hv_Col1 + hv_Col2) / 2;
                            }
                        }
                        else if ((int)(new HTuple(hv_inputDirection.TupleEqual("R2L_New"))) != 0)
                        {
                            hv_LineRowStart_Measure_01_0.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_LineRowStart_Measure_01_0 = hv_Row1 - (hv_inputRoiWidthLen / 2);
                            }
                            hv_LineColumnStart_Measure_01_0.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_LineColumnStart_Measure_01_0 = (hv_Col1 + hv_Col2) / 2;
                            }
                            hv_LineRowEnd_Measure_01_0.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_LineRowEnd_Measure_01_0 = hv_Row2 + (hv_inputRoiWidthLen / 2);
                            }
                            hv_LineColumnEnd_Measure_01_0.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_LineColumnEnd_Measure_01_0 = (hv_Col1 + hv_Col2) / 2;
                            }
                        }
                        hv_MeasureLength1.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_MeasureLength1 = ((hv_Col2 - hv_Col1) / 2) - 2;
                        }
                    }
                    else if ((int)(new HTuple(hv_inputXorY.TupleEqual("X"))) != 0)
                    {
                        hv_r1.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_r1 = hv_Row - hv_Length2;
                        }
                        hv_c1.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_c1 = hv_Column - hv_Length1;
                        }
                        ho_Cross2.Dispose();
                        HOperatorSet.GenCrossContourXld(out ho_Cross2, hv_r1, hv_c1, 100, 0);
                        hv_r3.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_r3 = hv_Row + hv_Length2;
                        }
                        hv_c3.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_c3 = hv_Column + hv_Length1;
                        }
                        ho_Cross1.Dispose();
                        HOperatorSet.GenCrossContourXld(out ho_Cross1, hv_r1, hv_c1, 100, 0);
                        ho_Cross3.Dispose();
                        HOperatorSet.GenCrossContourXld(out ho_Cross3, hv_r3, hv_c3, 100, 0);
                        //an := deg(Phi)
                        hv_Row1.Dispose();
                        hv_Row1 = new HTuple(hv_r1);
                        hv_Col1.Dispose();
                        hv_Col1 = new HTuple(hv_c1);
                        hv_Row2.Dispose();
                        hv_Row2 = new HTuple(hv_r3);
                        hv_Col2.Dispose();
                        hv_Col2 = new HTuple(hv_c3);
                        //*         endif
                        if ((int)(new HTuple(hv_inputDirection.TupleEqual("U2B_New"))) != 0)
                        {
                            hv_LineRowStart_Measure_01_0.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_LineRowStart_Measure_01_0 = (hv_Row1 + hv_Row2) / 2;
                            }
                            hv_LineColumnStart_Measure_01_0.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_LineColumnStart_Measure_01_0 = hv_Col1 - (hv_inputRoiWidthLen / 2);
                            }
                            hv_LineRowEnd_Measure_01_0.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_LineRowEnd_Measure_01_0 = (hv_Row1 + hv_Row2) / 2;
                            }
                            hv_LineColumnEnd_Measure_01_0.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_LineColumnEnd_Measure_01_0 = hv_Col2 + (hv_inputRoiWidthLen / 2);
                            }
                        }
                        else if ((int)(new HTuple(hv_inputDirection.TupleEqual("B2U_New"))) != 0)
                        {
                            hv_LineRowStart_Measure_01_0.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_LineRowStart_Measure_01_0 = (hv_Row1 + hv_Row2) / 2;
                            }
                            hv_LineColumnStart_Measure_01_0.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_LineColumnStart_Measure_01_0 = hv_Col2 + (hv_inputRoiWidthLen / 2);
                            }
                            hv_LineRowEnd_Measure_01_0.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_LineRowEnd_Measure_01_0 = (hv_Row1 + hv_Row2) / 2;
                            }
                            hv_LineColumnEnd_Measure_01_0.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_LineColumnEnd_Measure_01_0 = hv_Col1 - (hv_inputRoiWidthLen / 2);
                            }
                        }
                        hv_MeasureLength1.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_MeasureLength1 = (hv_Row2 - hv_Row1) / 2;
                        }
                    }


                    hv_Width.Dispose(); hv_Height.Dispose();
                    HOperatorSet.GetImageSize(ho_inputImage, out hv_Width, out hv_Height);
                    //Create the metrology model data structure
                    hv_MetrologyHandle.Dispose();
                    HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
                    //The image size is set in advance to speed up the
                    //first call of apply_metrology_model.
                    HOperatorSet.SetMetrologyModelImageSize(hv_MetrologyHandle, hv_Width, hv_Height);
                    //Define the parameters of the metrology line objects
                    //Create One metrology line objects and set parameters
                    //gen_contour_polygon_xld (Contour1, [LineRowStart_Measure_01_0,LineRowEnd_Measure_01_0], [LineColumnStart_Measure_01_0, LineColumnEnd_Measure_01_0])
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Index1.Dispose();
                        HOperatorSet.AddMetrologyObjectLineMeasure(hv_MetrologyHandle, hv_LineRowStart_Measure_01_0,
                            hv_LineColumnStart_Measure_01_0, hv_LineRowEnd_Measure_01_0, hv_LineColumnEnd_Measure_01_0,
                            hv_MeasureLength1, hv_inputRoiWidthLen / 2, hv_inputSigma, hv_inputAmplitudeThreshold,
                            (((new HTuple("measure_distance")).TupleConcat("measure_transition")).TupleConcat(
                            "measure_select")).TupleConcat("min_score"), ((((hv_inputInterval.TupleConcat(
                            hv_inputTransition))).TupleConcat(hv_inputPosition))).TupleConcat(0.3),
                            out hv_Index1);
                    }

                    //set_metrology_object_param (MetrologyHandle, 'all', 'min_score', 0.3)

                    //Create region of interest for the alignment
                    HOperatorSet.ApplyMetrologyModel(ho_inputImage, hv_MetrologyHandle);
                    hv_LineParameter.Dispose();
                    HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, "all", "all", "result_type",
                        "all_param", out hv_LineParameter);
                    ho_outputContour.Dispose();
                    HOperatorSet.GetMetrologyObjectResultContour(out ho_outputContour, hv_MetrologyHandle,
                        "all", "all", 1.5);
                    hv_DebugMode.Dispose();
                    hv_DebugMode = 1;
                    if ((int)(hv_DebugMode) != 0)
                    {
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ConcatObj(ho_outputContour, ho_ProcessContour, out ExpTmpOutVar_0
                                );
                            ho_ProcessContour.Dispose();
                            ho_ProcessContour = ExpTmpOutVar_0;
                        }
                        ho_ResultContour.Dispose(); hv_OutPointRow.Dispose(); hv_OutPointCol.Dispose();
                        HOperatorSet.GetMetrologyObjectMeasures(out ho_ResultContour, hv_MetrologyHandle,
                            "all", "all", out hv_OutPointRow, out hv_OutPointCol);
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ConcatObj(ho_ResultContour, ho_ProcessContour, out ExpTmpOutVar_0
                                );
                            ho_ProcessContour.Dispose();
                            ho_ProcessContour = ExpTmpOutVar_0;
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            ho_Cross.Dispose();
                            HOperatorSet.GenCrossContourXld(out ho_Cross, hv_OutPointRow, hv_OutPointCol,
                                5, (new HTuple(45)).TupleRad());
                        }
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ConcatObj(ho_Cross, ho_ProcessContour, out ExpTmpOutVar_0);
                            ho_ProcessContour.Dispose();
                            ho_ProcessContour = ExpTmpOutVar_0;
                        }
                        if (HDevWindowStack.IsOpen())
                        {
                            HOperatorSet.DispObj(ho_ProcessContour, HDevWindowStack.GetActive());
                        }
                    }

                    HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle);
                    ho_ProcessContour.Dispose();
                    ho_Rectangle1.Dispose();
                    ho_Cross1.Dispose();
                    ho_Cross3.Dispose();
                    ho_Cross2.Dispose();
                    ho_ResultContour.Dispose();
                    ho_Cross.Dispose();

                    hv_Row11.Dispose();
                    hv_Column1.Dispose();
                    hv_Row21.Dispose();
                    hv_Column2.Dispose();
                    hv_Length1.Dispose();
                    hv_Length2.Dispose();
                    hv_Row.Dispose();
                    hv_Column.Dispose();
                    hv_r2.Dispose();
                    hv_c2.Dispose();
                    hv_r4.Dispose();
                    hv_c4.Dispose();
                    hv_Row1.Dispose();
                    hv_Col1.Dispose();
                    hv_Row2.Dispose();
                    hv_Col2.Dispose();
                    hv_LineRowStart_Measure_01_0.Dispose();
                    hv_LineColumnStart_Measure_01_0.Dispose();
                    hv_LineRowEnd_Measure_01_0.Dispose();
                    hv_LineColumnEnd_Measure_01_0.Dispose();
                    hv_MeasureLength1.Dispose();
                    hv_r1.Dispose();
                    hv_c1.Dispose();
                    hv_r3.Dispose();
                    hv_c3.Dispose();
                    hv_Width.Dispose();
                    hv_Height.Dispose();
                    hv_MetrologyHandle.Dispose();
                    hv_Index1.Dispose();
                    hv_LineParameter.Dispose();
                    hv_DebugMode.Dispose();

                    return;
                }
                ho_ProcessContour.Dispose();
                ho_Rectangle1.Dispose();
                ho_Cross1.Dispose();
                ho_Cross3.Dispose();
                ho_Cross2.Dispose();
                ho_ResultContour.Dispose();
                ho_Cross.Dispose();

                hv_Row11.Dispose();
                hv_Column1.Dispose();
                hv_Row21.Dispose();
                hv_Column2.Dispose();
                hv_Length1.Dispose();
                hv_Length2.Dispose();
                hv_Row.Dispose();
                hv_Column.Dispose();
                hv_r2.Dispose();
                hv_c2.Dispose();
                hv_r4.Dispose();
                hv_c4.Dispose();
                hv_Row1.Dispose();
                hv_Col1.Dispose();
                hv_Row2.Dispose();
                hv_Col2.Dispose();
                hv_LineRowStart_Measure_01_0.Dispose();
                hv_LineColumnStart_Measure_01_0.Dispose();
                hv_LineRowEnd_Measure_01_0.Dispose();
                hv_LineColumnEnd_Measure_01_0.Dispose();
                hv_MeasureLength1.Dispose();
                hv_r1.Dispose();
                hv_c1.Dispose();
                hv_r3.Dispose();
                hv_c3.Dispose();
                hv_Width.Dispose();
                hv_Height.Dispose();
                hv_MetrologyHandle.Dispose();
                hv_Index1.Dispose();
                hv_LineParameter.Dispose();
                hv_DebugMode.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_ProcessContour.Dispose();
                ho_Rectangle1.Dispose();
                ho_Cross1.Dispose();
                ho_Cross3.Dispose();
                ho_Cross2.Dispose();
                ho_ResultContour.Dispose();
                ho_Cross.Dispose();

                hv_Row11.Dispose();
                hv_Column1.Dispose();
                hv_Row21.Dispose();
                hv_Column2.Dispose();
                hv_Length1.Dispose();
                hv_Length2.Dispose();
                hv_Row.Dispose();
                hv_Column.Dispose();
                hv_r2.Dispose();
                hv_c2.Dispose();
                hv_r4.Dispose();
                hv_c4.Dispose();
                hv_Row1.Dispose();
                hv_Col1.Dispose();
                hv_Row2.Dispose();
                hv_Col2.Dispose();
                hv_LineRowStart_Measure_01_0.Dispose();
                hv_LineColumnStart_Measure_01_0.Dispose();
                hv_LineRowEnd_Measure_01_0.Dispose();
                hv_LineColumnEnd_Measure_01_0.Dispose();
                hv_MeasureLength1.Dispose();
                hv_r1.Dispose();
                hv_c1.Dispose();
                hv_r3.Dispose();
                hv_c3.Dispose();
                hv_Width.Dispose();
                hv_Height.Dispose();
                hv_MetrologyHandle.Dispose();
                hv_Index1.Dispose();
                hv_LineParameter.Dispose();
                hv_DebugMode.Dispose();

                throw HDevExpDefaultException;
            }
        }

        public DisplayWindowClass GetCamHwindow()
        {
            DisplayWindowClass displayWindow = null;
            if (currentCameraEnum == CameraEnum.Cam1)
            {
                displayWindow = displayWindow_Cam1;
            }
            else if (currentCameraEnum == CameraEnum.Cam2)
            {
                displayWindow = displayWindow_Cam2;
            }
            else if (currentCameraEnum == CameraEnum.Cam3)
            {
                displayWindow = displayWindow_Cam3;
            }
            else if (currentCameraEnum == CameraEnum.Cam4)
            {
                displayWindow = displayWindow_Cam4;
            }
            return displayWindow;
        }
        #endregion

        /// <summary>
        /// 加载标定图像路径
        /// </summary>
        /// <param name="calibrationImgPath"></param>
        /// <param name="currentCalibAlgorithmEnum"></param>
        public void LoadCalibrationImgPath(string calibrationImgPath, CalibrationAlgorithmEnum currentCalibAlgorithmEnum)
        {
            try
            {
                if (true)
                {
                    CameraEnum cameraEnum = CameraEnum.Cam1;
                    m_ImageSimManage_Cam1.InitSimFlag();
                    m_ImageSimManage_Cam1.ReadImageSimPath(calibrationImgPath + $"\\{cameraEnum}");
                }

                if (true)
                {
                    CameraEnum cameraEnum = CameraEnum.Cam2;
                    m_ImageSimManage_Cam2.InitSimFlag();
                    m_ImageSimManage_Cam2.ReadImageSimPath(calibrationImgPath + $"\\{cameraEnum}");
                }
                if (true)
                {
                    CameraEnum cameraEnum = CameraEnum.Cam3;
                    m_ImageSimManage_Cam3.InitSimFlag();
                    m_ImageSimManage_Cam3.ReadImageSimPath(calibrationImgPath + $"\\{cameraEnum}");
                }
                if (true)
                {
                    CameraEnum cameraEnum = CameraEnum.Cam4;
                    m_ImageSimManage_Cam4.InitSimFlag();
                    m_ImageSimManage_Cam4.ReadImageSimPath(calibrationImgPath + $"\\{cameraEnum}");
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                MessageBox.Show(ex.ToString());
            }

        }

        private void btn_edgeShow_Click(object sender, EventArgs e)
        {
            try
            {
                //*从亮到暗，从下到上

                ShowHalconWindow currentShowHalconWindow = null;
                ImageSimManage m_ImageSimManage = null;
                DisplayWindowClass displayWindowClass = null;
                switch (currentCameraEnum)
                {
                    case CameraEnum.Cam1:
                        currentShowHalconWindow = displayWindow_Cam1.operation_ImageInfo.showHalconWindow;
                        m_ImageSimManage = m_ImageSimManage_Cam1;
                        displayWindowClass = displayWindow_Cam1;
                        break;
                    case CameraEnum.Cam2:
                        currentShowHalconWindow = displayWindow_Cam2.operation_ImageInfo.showHalconWindow;
                        m_ImageSimManage = m_ImageSimManage_Cam2;
                        displayWindowClass = displayWindow_Cam2;
                        break;
                    case CameraEnum.Cam3:
                        currentShowHalconWindow = displayWindow_Cam3.operation_ImageInfo.showHalconWindow;
                        m_ImageSimManage = m_ImageSimManage_Cam3;
                        displayWindowClass = displayWindow_Cam3;
                        break;
                    case CameraEnum.Cam4:
                        currentShowHalconWindow = displayWindow_Cam4.operation_ImageInfo.showHalconWindow;
                        m_ImageSimManage = m_ImageSimManage_Cam4;
                        displayWindowClass = displayWindow_Cam4;
                        break;
                }
                if (m_ImageSimManage.ImageEmulationPaths == null || m_ImageSimManage.ImageEmulationPaths.Length == 0) { MessageBox.Show("请先加载图像！"); return; };
                m_ImageSimManage.GetCurrentImage(out HObject Load_Image);


                // 释放之前的数据              
                currentShowHalconWindow.Dispose();
                currentShowHalconWindow.m_hImage = Load_Image;
                int currentFindILinendex = cob_SelectLine.SelectedIndex;
                if (currentFindILinendex < 0)
                {
                    cob_SelectLine.SelectedIndex = 0;
                    currentFindILinendex = 0;
                }
                HOperatorSet.GenRectangle1(out HObject rectangle, CurrentParam_PretargetingCalib.findRectangleLineParams[currentFindILinendex].fRoiStartRow, CurrentParam_PretargetingCalib.findRectangleLineParams[currentFindILinendex].fRoiStartCol,
                     CurrentParam_PretargetingCalib.findRectangleLineParams[currentFindILinendex].fRoiEndRow, CurrentParam_PretargetingCalib.findRectangleLineParams[currentFindILinendex].fRoiEndCol);
                currentShowHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = rectangle, m_stuGetColor = HalconColor.green });

                Find_Line_ForRectangle(Load_Image, rectangle, out HObject ho_outputContour1, dicCobInputXorY[CurrentParam_PretargetingCalib.findRectangleLineParams[currentFindILinendex].iCobInputXorY],
                     CurrentParam_PretargetingCalib.findRectangleLineParams[currentFindILinendex].iInputInterval, dicCobDirection[CurrentParam_PretargetingCalib.findRectangleLineParams[currentFindILinendex].iCobDirection],
                      CurrentParam_PretargetingCalib.findRectangleLineParams[currentFindILinendex].fInputRoiWidthLen, CurrentParam_PretargetingCalib.findRectangleLineParams[currentFindILinendex].fInputSigma,
                       CurrentParam_PretargetingCalib.findRectangleLineParams[currentFindILinendex].iInputAmplitudeThreshold,
                       dicCobinputTransition[CurrentParam_PretargetingCalib.findRectangleLineParams[currentFindILinendex].iCobinputTransition],
                       dicCobinputPosition[CurrentParam_PretargetingCalib.findRectangleLineParams[currentFindILinendex].iCobinputPosition], out HTuple hv_OutPointRow1,
                    out HTuple hv_OutPointCol1);
                currentShowHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = ho_outputContour1, m_stuGetColor = HalconColor.yellow });
                HOperatorSet.GenCrossContourXld(out HObject ho_Cross1, hv_OutPointRow1, hv_OutPointCol1, 5, (new HTuple(45)).TupleRad());
                currentShowHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = ho_Cross1, m_stuGetColor = HalconColor.blue });

                displayWindowClass.RepaintCam(new Operation_ImageInfo() { showHalconWindow = currentShowHalconWindow }, out Operation_ImageInfo screenshotImage);
                screenshotImage?.Dispose();

            }
            catch (Exception ee)
            {

                MessageBox.Show("未找到边缘！" + ee.Message);
            }

        }

        /// <summary>
        /// 修正旋转中心坐标
        /// </summary>
        /// <param name="currentCenterX">当前设置的旋转中心 X 坐标</param>
        /// <param name="currentCenterY">当前设置的旋转中心 Y 坐标</param>
        /// <param name="deltaX">观察到的 X 偏移量</param>
        /// <param name="deltaY">观察到的 Y 偏移量</param>
        /// <param name="theta">旋转角度（单位：度）</param>
        /// <returns>修正后的旋转中心坐标 (X, Y)</returns>
        public (double CorrectedX, double CorrectedY) CorrectRotationCenter(
            double currentCenterX, double currentCenterY, double deltaX, double deltaY, double theta)
        {
            // 将角度转换为弧度
            double thetaRad = theta * Math.PI / 180.0;

            // 计算三角函数值
            double cosTheta = Math.Cos(thetaRad);
            double sinTheta = Math.Sin(thetaRad);
            double cosThetaMinus1 = cosTheta - 1;

            // 计算修正量 Cx 和 Cy
            double denominator = cosThetaMinus1 * cosThetaMinus1 + sinTheta * sinTheta;
            double Cx = (cosThetaMinus1 * deltaX + sinTheta * deltaY) / denominator;
            double Cy = (-sinTheta * deltaX + cosThetaMinus1 * deltaY) / denominator;

            // 修正旋转中心坐标
            double correctedX = currentCenterX + Cx;
            double correctedY = currentCenterY + Cy;

            return (correctedX, correctedY);
        }

        private void Btn_secCalib_Click(object sender, EventArgs e)
        {
            try
            {
                // 计算修正后的旋转中心
                (double CorrectedX, double CorrectedY) correctedCenter = CorrectRotationCenter((double)nud_CenterOfRotationX.Value, (double)nud_CenterOfRotationY.Value,
                    (double)nud_CurrentOffsetX.Value, (double)nud_CurrentOffsetY.Value, (double)nud_CurrentOffsetR.Value);

                nud_rotateX.Value = (decimal)correctedCenter.CorrectedX;
                nud_rotateY.Value = (decimal)correctedCenter.CorrectedY;
            }
            catch (Exception ex)
            {
                MessageBox.Show("输入得值有误)\n" + ex.Message);
            }

        }
    }

    public enum CurrentCalibrationState
    {
        None,
        开始标定,
        标定中,
        标定数据获取完成,
        标定完成,
        标定失败,
        退出标定
    }
    public enum CalibrationAlgorithmEnum
    {
        棋盘格,
        平移,
        旋转
    }
}

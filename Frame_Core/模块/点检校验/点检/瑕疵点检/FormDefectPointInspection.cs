using HalconDotNet;
using LogTool;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ViewROI2;

namespace Frame_Core
{
    public partial class FormDefectPointInspection : Form
    {
        #region 窗口变量
        bool bl_RoiExist = false;
        static DisplayWindowClass displayWindow;
        /// <summary>
        /// 图片仿真路径数据
        /// </summary>
        public ImageSimManage m_ImageSimManage = new ImageSimManage();
        /// <summary>
        /// 当前显示的结果数据（包含图像、Region）
        /// </summary>
        ShowHalconWindow currentShowHalconWindow = new ShowHalconWindow();

        public void InitWindow()
        {
            displayWindow = new DisplayWindowClass(HWC_Display, TB_PositionAndGray, RestoreWindow, CameraEnum.Cam1, "Form_PointInspection", true);
            displayWindow.m_hWndCtrl_Display_Cam.ImageProcessingEvent += new HWndCtrl.ImageProcessingDelegate(ImageProcessingCallback);   //更改测量矩形，执行直线测量方法
            currentShowHalconWindow.m_messageShowInfos = new List<MessageShowInfo>();
            currentShowHalconWindow.m_regionShowInfos = new List<RegionShowInfo>();
            currentShowHalconWindow.m_lineShowInfos = new List<LineInfo>();
        }
        public void ImageProcessingCallback()
        {
            GetRoiParams(displayWindow.m_roiRectangle2_Cam, out int RoiPointsNum, out double RoiMidRow, out double RoiMidColumn, out double RoiAngle, out double RoiLength1, out double RoiLength2);

        }

        /// <summary>
        /// 获取ROI参数并显示
        /// </summary>
        /// <param name="roiRectangle2"></param>
        /// <param name="measLineParams"></param>
        public void GetRoiParams(ROIRectangle2 roiRectangle2, out int RoiPointsNum, out double RoiMidRow, out double RoiMidColumn, out double RoiAngle, out double RoiLength1, out double RoiLength2)
        {
            roiRectangle2.ROIName = "";
            HTuple hv_Angle;
            double d_Angle;
            double w;
            double h;
            //measLineParams.ho_Image = roiRectangle2.
            //HOperatorSet.TupleRad(lineParam.RoiAngle, out hv_Phi);
            HOperatorSet.TupleDeg(roiRectangle2.phi, out hv_Angle);
            d_Angle = hv_Angle;
            w = roiRectangle2.length1 * 2;
            h = roiRectangle2.length2 * 2;

            RoiPointsNum = roiRectangle2.PointNum;
            RoiMidRow = roiRectangle2.midR;
            RoiMidColumn = roiRectangle2.midC;
            RoiAngle = d_Angle;
            RoiLength1 = w;
            RoiLength2 = h;
        }

        /// <summary>
        /// ViewRoi函数：加载ROI
        /// </summary>
        /// <param name="measLineParams">抓边工作类</param>
        /// <param name="roiRectangle2"></param>
        /// <param name="bl_RoiExist">ROI是否存在</param>
        /// <param name="roiController">Roi控制类</param>
        /// <param name="hWndCtrl">Roi显示类</param>
        private void SetRoiParams(HObject ho_Image, int RoiPointsNum, double RoiMidRow, double RoiMidColumn, double RoiAngle, double RoiLength1, double RoiLength2, ROIRectangle2 roiRectangle2, ROIController roiController)
        {
            HTuple hv_Phi;
            HTuple hv_Width;
            HTuple hv_Height;
            roiRectangle2.PointNum = RoiPointsNum;
            roiRectangle2.ROIName = "";

            roiRectangle2.midR = RoiMidRow;
            roiRectangle2.midC = RoiMidColumn;
            HOperatorSet.TupleRad(RoiAngle, out hv_Phi);
            roiRectangle2.phi = hv_Phi.D;
            roiRectangle2.length2 = RoiLength2 / 2;
            roiRectangle2.length1 = RoiLength1 / 2;

            if (ho_Image != null)
            {
                if (ho_Image.CountObj() != 0)
                {
                    HOperatorSet.GetImageSize(ho_Image, out hv_Width, out hv_Height);
                    if (hv_Width < 1000)
                    {
                        roiController.size = roiRectangle2.size = 1;
                    }
                    else
                    {
                        roiController.size = roiRectangle2.size = hv_Width / 1000 + 1;
                    }
                    if (bl_RoiExist == false)
                    {
                        bl_RoiExist = true;
                        roiController.setROIShape(roiRectangle2);
                        roiController.roiMode.createROI(roiRectangle2.midC, roiRectangle2.midR);
                        roiController.ROIList.Add(roiController.roiMode);
                    }

                    roiController.activeROIidx = -1;
                    roiRectangle2.updateHandlePos();
                }
            }
        }

        #endregion

      
        CameraEnum currentCamera = CameraEnum.Cam1;
        CameraRelevancy currentCameraManager;    
        public FormDefectPointInspection(CameraEnum camera, CameraRelevancy _M_CameraRelevancies)
        {
            InitializeComponent();
            currentCamera = camera;
            currentCameraManager = _M_CameraRelevancies;
        }

        private void Form_DefectPointInspection_Load(object sender, EventArgs e)
        {
            try
            {
                InitWindow();

                //DefectPointInspectionManage.CreationDefectFolder(currentCameraManager.m_DefectPointInspectionManage.defectPointInspectionParam.PointCheckImgPath, currentCamera, Project.Instance.craftInfo);
               
                ShowFormParam(currentCameraManager.m_DefectPointInspectionManage.defectPointInspectionParam);
                m_ImageSimManage.InitSimFlag();
                DirectoryHelper.GetImagePathsFromSubfolders(currentCameraManager.m_DefectPointInspectionManage.defectPointInspectionParam.PointCheckImgPath, out List<string> imagePaths);
                m_ImageSimManage.ImageEmulationPaths = imagePaths.ToArray();
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
                MessageBox.Show(ex.ToString());
            }
        }

        private void Form_DefectPointInspection_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                //窗体关闭清理非托管内存
                currentShowHalconWindow.Dispose();
                displayWindow.Dispose();
              
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
                MessageBox.Show(ex.ToString());
            }
        }


        #region 加载图片仿真
        private void LoadImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog FileDialog = new OpenFileDialog();
            FileDialog.Multiselect = true;
            //FileDialog.InitialDirectory = Project.Instance.standImagePath;
            if (FileDialog.ShowDialog() == DialogResult.OK)
            {
                m_ImageSimManage.InitSimFlag();
                m_ImageSimManage.ImageEmulationPaths = FileDialog.FileNames;
            }
        }

        private void btn_Preview_Click(object sender, EventArgs e)
        {
            if (m_ImageSimManage.ImageEmulationPaths == null || m_ImageSimManage.ImageEmulationPaths.Length == 0) return;
            m_ImageSimManage.GetPreviewImage(out HObject Load_Image, out string imgPath);
            Execute(Load_Image, imgPath);
        }

        private void btn_Run_Click(object sender, EventArgs e)
        {
            if (m_ImageSimManage.ImageEmulationPaths == null || m_ImageSimManage.ImageEmulationPaths.Length == 0) return;
            m_ImageSimManage.GetCurrentImage(out HObject Load_Image, out string imgPath);
            Execute(Load_Image, imgPath);
        }

        private void btn_Next_Click(object sender, EventArgs e)
        {
            if (m_ImageSimManage.ImageEmulationPaths == null || m_ImageSimManage.ImageEmulationPaths.Length == 0) return;
            m_ImageSimManage.GetNextImage(out HObject Load_Image, out string imgPath);
            Execute(Load_Image, imgPath);
        }
        #endregion

        #region 一键点检
        private void btn_StartPointInspection_Click(object sender, EventArgs e)
        {
            //string str = "缺陷点检";
            //string filePath = Project.Instance.paramPathSet.pointCheckRootPath + DateTime.Now.ToString("yyyy.MM.dd_HH-mm-ss-fff") + str;
            //if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);

            //ExecuteResult executeResult = DefectPointInspectionWorks.OnekeyPointInspectionFlow(currentCamera, filePath+ $"\\{str}_{currentCamera}", out bool isPass, out List<RowData[]> datass);
            //NListView2.ClearItems(listView1);
            //NListView2.Add(listView1, datass, 10000, ListViewShowType.RemoveAtStartRowAndEndVisible, false);

            //UpdateTime(executeResult.Time);

        }

        public bool DirectoriesNoImage_CallBack(string path)
        {
            string str = "请先在软件根目录存放NG大图路径如下";
            if (MessageBox.Show($"{str}\n{path}", "", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
            {
                return false;
            }
            System.Diagnostics.Process.Start(path);
            return true;
        }
        #endregion

        public void ShowFormParam(DefectPointInspectionParam defectPointInspectionParam)
        {
            ControlTool.SetParamShow(tbx_PointCheckImgPath, ref defectPointInspectionParam.PointCheckImgPath);
        }

        public void GetFormParam(ref DefectPointInspectionParam defectPointInspectionParam)
        {
            try
            {
                ControlTool.GetParamSave(tbx_PointCheckImgPath, ref defectPointInspectionParam.PointCheckImgPath);
            }
            catch (Exception ex)
            {

                Log4netRecord.ErrorLog.Debug(ex);
                MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// 数据查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(Project.Instance.paramPathSet.pointCheckRootPath);
            }
            catch (Exception)
            {

            }
        }
        private void btn_Save_Click(object sender, EventArgs e)
        {
            try
            {
                SaveParam();

                //DefectPointInspectionManage.CreationDefectFolder(currentCameraManager.m_DefectPointInspectionManage.defectPointInspectionParam.PointCheckImgPath, currentCamera, Project.Instance.craftInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        public void SaveParam()
        {

            try
            {

                GetFormParam(ref currentCameraManager.m_DefectPointInspectionManage.defectPointInspectionParam);

                currentCameraManager.m_DefectPointInspectionManage.defectPointInspectionParam.Save();
            }
            catch (Exception ex)
            {

                Log4netRecord.ErrorLog.Debug(ex);
                MessageBox.Show(ex.ToString());
            }
        }


        private void linkLabel_ImgPath_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                string path = tbx_PointCheckImgPath.Text;
                // 获取文件夹绝对路径    显示在 txtbox 控件里
                using (FolderBrowserDialog folder = new FolderBrowserDialog())
                {
                    if (path != null && System.IO.Directory.Exists(path))
                    {
                        folder.SelectedPath = path;

                    }
                    if (folder.ShowDialog() == DialogResult.OK && folder.SelectedPath != "")
                    {
                        tbx_PointCheckImgPath.Text = folder.SelectedPath;
                        m_ImageSimManage.InitSimFlag();
                        DirectoryHelper.GetImagePathsFromSubfolders(tbx_PointCheckImgPath.Text, out List<string> imagePaths);
                        m_ImageSimManage.ImageEmulationPaths = imagePaths.ToArray();
                    }
                }

            }
            catch (Exception)
            {

            }
        }
        private void tbx_PointCheckImgPath_TextChanged(object sender, EventArgs e)
        {
            m_ImageSimManage.InitSimFlag();
            DirectoryHelper.GetImagePathsFromSubfolders(tbx_PointCheckImgPath.Text, out List<string> imagePaths);
            m_ImageSimManage.ImageEmulationPaths = imagePaths.ToArray();
        }
        public void Execute(HObject ho_Image, string imgPath)
        {
            HTuple start = null, end = null;
            HOperatorSet.CountSeconds(out start);
            double time = 0;
            try
            {
                // 释放之前的数据
                currentShowHalconWindow.Dispose();
                currentShowHalconWindow.m_hImage = ho_Image;
              

                displayWindow.m_hWndCtrl_Display_Cam.bl_RoiOperateFlag = false;//不显示测量矩形ROI

                //DefectPointInspectionWorks.DefectMeasureFlow(currentCamera, ho_Image, imgPath, ref currentShowHalconWindow, ref dieCutLaminationParameter);

                displayWindow.RepaintCam(new Operation_ImageInfo() { showHalconWindow = currentShowHalconWindow }, out _);
              
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                MessageBox.Show(ex.Message);
            }
            finally
            {
                HOperatorSet.CountSeconds(out end);
                time = ((end - start) * 1000).D;
                time = Math.Round(time, 2);
                lb_Time.Text = $"Time：{time}ms";
            }
        }

        private void UpdateTime(double time)
        {
            lb_Time.Text = $"Time：{time}ms";
        }
    }
}

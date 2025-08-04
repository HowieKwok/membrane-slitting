using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Frame_Core;
using ViewROI2;
using HalconDotNet;
using System.IO;
using NPOI.OpenXmlFormats.Wordprocessing;
using System.Diagnostics;
using LogTool;
using NPOI.OpenXmlFormats.Vml;
using System.Diagnostics.Contracts;
using static System.Net.Mime.MediaTypeNames;
using DALSA.SaperaLT.SapClassBasic;
using System.Threading;

namespace Mef_VisionFrame
{
    public partial class CheckPointForm : Form
    {

        DisplayWindowClass displayWindow;
        /// <summary>
        /// 当前显示的结果数据（包含图像、Region）
        /// </summary>
        ShowHalconWindow currentShowHalconWindow = new ShowHalconWindow();

        /// <summary>
        /// 图片仿真路径数据
        /// </summary>
        public ImageSimManage m_ImageSimManage = new ImageSimManage();

        Dictionary<CameraEnum, CameraManager> cameraManager;
        CameraManager currentCameraManager;
        public CheckPointForm(Dictionary<CameraEnum, CameraManager> _cameraManager)
        {
            InitializeComponent();
            cameraManager = _cameraManager;

        }

        private void CheckPointForm_Load(object sender, EventArgs e)
        {
            try
            {
                displayWindow = new DisplayWindowClass(HWC_Display, TB_PositionAndGray, FullScrean, CameraEnum.Cam1, "MultiCamCalibrationModelForm");
                currentShowHalconWindow.m_messageShowInfos = new List<MessageShowInfo>();
                currentShowHalconWindow.m_regionShowInfos = new List<RegionShowInfo>();
                currentShowHalconWindow.m_lineShowInfos = new List<LineInfo>();

                //=========================选择项控件
                //选择项控件
                this.Tv_Select.HideSelection = false;
                //自已绘制  
                this.Tv_Select.DrawMode = TreeViewDrawMode.OwnerDrawText;


                //初始化相机节点
                Tv_Select.Nodes.Clear();
                foreach (var item in cameraManager)
                {
                    Tv_Select.Nodes.Add(item.Key.ToString());
                }

                //展开节点
                Tv_Select.ExpandAll();
                //初始节点
                Tv_Select.SelectedNode = Tv_Select.Nodes[0];

            }
            catch (Exception ex)
            {

                Log4netRecord.ErrorLog.Debug(ex);
            }
        }

        private void CheckPointForm_FormClosing(object sender, FormClosingEventArgs e)
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

            }
        }


        private void LoadImage_Click(object sender, EventArgs e)
        {

            try
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
            catch (Exception ex)
            {

                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }

        }


        private void btn_previewSize_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_ImageSimManage.ImageEmulationPaths == null || m_ImageSimManage.ImageEmulationPaths.Length == 0) return;
                m_ImageSimManage.GetPreviewImage(out HObject Load_Image);

                HandExecutePointCheck(Load_Image);
            }
            catch (Exception)
            {

            }
        }

        private void btn_detectionSize_Click(object sender, EventArgs e)
        {
            try
            {

                if (m_ImageSimManage.ImageEmulationPaths == null || m_ImageSimManage.ImageEmulationPaths.Length == 0) return;
                m_ImageSimManage.GetCurrentImage(out HObject Load_Image);

                HandExecutePointCheck(Load_Image);
            }
            catch (Exception)
            {

            }

        }

        private void btn_nextSize_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_ImageSimManage.ImageEmulationPaths == null || m_ImageSimManage.ImageEmulationPaths.Length == 0) return;
                m_ImageSimManage.GetNextImage(out HObject Load_Image);

                HandExecutePointCheck(Load_Image);
            }
            catch (Exception)
            {


            }
        }

     

        #region 选择项控件
        /// 绘制颜色
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tv_Select_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            //e.DrawDefault = true; //我这里用默认颜色即可，只需要在TreeView失去焦点时选中节点仍然突显  
            //return;

            //or  自定义颜色  
            if ((e.State & TreeNodeStates.Selected) != 0)
            {
                //演示为绿底白字  
                e.Graphics.FillRectangle(Brushes.DodgerBlue, e.Node.Bounds);

                Font nodeFont = e.Node.NodeFont;
                if (nodeFont == null) nodeFont = ((TreeView)sender).Font;
                e.Graphics.DrawString(e.Node.Text, nodeFont, Brushes.White, Rectangle.Inflate(e.Bounds, 2, 0));
            }
            else
            {
                e.DrawDefault = true;
            }

            if ((e.State & TreeNodeStates.Focused) != 0)
            {
                using (Pen focusPen = new Pen(Color.Black))
                {
                    focusPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                    Rectangle focusBounds = e.Node.Bounds;
                    focusBounds.Size = new Size(focusBounds.Width - 1,
                    focusBounds.Height - 1);
                    e.Graphics.DrawRectangle(focusPen, focusBounds);
                }
            }

        }

        private void Tv_Select_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {

                foreach (var item in cameraManager)
                {
                    if (Tv_Select.SelectedNode.Text == item.Key.ToString())
                    {
                        currentCameraManager = item.Value;
                        break;
                    }
                }

                NListView2.ClearItems(listView1);
                if (currentCameraManager.m_SizeCheckPointManage.sizeCheckPointParam != null)
                {
                    ControlTool.SetParamShow(tbx_PointCheckImgPath, ref currentCameraManager.m_SizeCheckPointManage.sizeCheckPointParam.PointCheckImgPath);
                    ControlTool.SetParamShow(cbx_SSDJ, ref currentCameraManager.m_SizeCheckPointManage.sizeCheckPointParam.SSDJ);
                    ControlTool.SetParamShow(nud_PointCheckSpeedRestrict, ref currentCameraManager.m_SizeCheckPointManage.sizeCheckPointParam.PointCheckSpeedRestrict);
                    ControlTool.SetParamShow(nud_CheckStartFrame, ref currentCameraManager.m_SizeCheckPointManage.sizeCheckPointParam.CheckStartFrame);
                    ControlTool.SetParamShow(nud_CheckNumber, ref currentCameraManager.m_SizeCheckPointManage.sizeCheckPointParam.CheckNumber);
                    ControlTool.SetParamShow(nud_FailAccumulativeNum, ref currentCameraManager.m_SizeCheckPointManage.sizeCheckPointParam.FailAccumulativeNum);
                    ControlTool.SetParamShow(nud_AlarmAccumulativeNum, ref currentCameraManager.m_SizeCheckPointManage.sizeCheckPointParam.AlarmAccumulativeNum);
                    ControlTool.SetParamShow(nud_StopAccumulativeNum, ref currentCameraManager.m_SizeCheckPointManage.sizeCheckPointParam.StopAccumulativeNum);
                    ControlTool.SetParamShow(cbx_Alarm, ref currentCameraManager.m_SizeCheckPointManage.sizeCheckPointParam.Alarm);
                    ControlTool.SetParamShow(cbx_Stop, ref currentCameraManager.m_SizeCheckPointManage.sizeCheckPointParam.Stop);
                    ControlTool.SetParamShow(nud_CalibBlockWidth, ref currentCameraManager.m_SizeCheckPointManage.sizeCheckPointParam.CalibBlockWidth);
                    ControlTool.SetParamShow(nud_CalibBlockWidthTolerance, ref currentCameraManager.m_SizeCheckPointManage.sizeCheckPointParam.CalibBlockWidthTolerance);
                    ControlTool.SetParamShow(nud_CalibBlockHeiht, ref currentCameraManager.m_SizeCheckPointManage.sizeCheckPointParam.CalibBlockHeiht);
                    ControlTool.SetParamShow(nud_CalibBlockHeihtTolerance, ref currentCameraManager.m_SizeCheckPointManage.sizeCheckPointParam.CalibBlockHeihtTolerance);
                    ControlTool.SetParamShow(nud_SizeX1, ref currentCameraManager.m_SizeCheckPointManage.sizeCheckPointParam.EdgeX1);
                    ControlTool.SetParamShow(nud_SizeX2, ref currentCameraManager.m_SizeCheckPointManage.sizeCheckPointParam.EdgeX2);
                    ControlTool.SetParamShow(num_CalibBlockMinThreshold, ref currentCameraManager.m_SizeCheckPointManage.sizeCheckPointParam.CalibBlockMinThreshold);
                    ControlTool.SetParamShow(num_CalibBlockMaxThreshold, ref currentCameraManager.m_SizeCheckPointManage.sizeCheckPointParam.CalibBlockMaxThreshold);
                    ControlTool.SetParamShow(nud_SideLengthPixX, ref currentCameraManager.m_SizeCheckPointManage.sizeCheckPointParam.SideLengthPixX);
                    ControlTool.SetParamShow(nud_SideLengthPixY, ref currentCameraManager.m_SizeCheckPointManage.sizeCheckPointParam.SideLengthPixY);

                    ControlTool.SetParamShow(nud_MesureThreshold, ref currentCameraManager.m_SizeCheckPointManage.sizeCheckPointParam.LineModelParam.MesureThreshold);

                }

                m_ImageSimManage.ReadImageSimPath(currentCameraManager.m_SizeCheckPointManage.sizeCheckPointParam.PointCheckImgPath);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
            }
        }

        #endregion
        #region 保存参数
        public void SaveParam()
        {
            try
            {
                ControlTool.GetParamSave(tbx_PointCheckImgPath, ref currentCameraManager.m_SizeCheckPointManage.sizeCheckPointParam.PointCheckImgPath);
                ControlTool.GetParamSave(cbx_SSDJ, ref currentCameraManager.m_SizeCheckPointManage.sizeCheckPointParam.SSDJ);
                ControlTool.GetParamSave(nud_PointCheckSpeedRestrict, ref currentCameraManager.m_SizeCheckPointManage.sizeCheckPointParam.PointCheckSpeedRestrict);
                ControlTool.GetParamSave(nud_CheckStartFrame, ref currentCameraManager.m_SizeCheckPointManage.sizeCheckPointParam.CheckStartFrame);
                ControlTool.GetParamSave(nud_CheckNumber, ref currentCameraManager.m_SizeCheckPointManage.sizeCheckPointParam.CheckNumber);
                ControlTool.GetParamSave(nud_FailAccumulativeNum, ref currentCameraManager.m_SizeCheckPointManage.sizeCheckPointParam.FailAccumulativeNum);
                ControlTool.GetParamSave(nud_AlarmAccumulativeNum, ref currentCameraManager.m_SizeCheckPointManage.sizeCheckPointParam.AlarmAccumulativeNum);
                ControlTool.GetParamSave(nud_StopAccumulativeNum, ref currentCameraManager.m_SizeCheckPointManage.sizeCheckPointParam.StopAccumulativeNum);
                ControlTool.GetParamSave(cbx_Alarm, ref currentCameraManager.m_SizeCheckPointManage.sizeCheckPointParam.Alarm);
                ControlTool.GetParamSave(cbx_Stop, ref currentCameraManager.m_SizeCheckPointManage.sizeCheckPointParam.Stop);
                ControlTool.GetParamSave(nud_CalibBlockWidth, ref currentCameraManager.m_SizeCheckPointManage.sizeCheckPointParam.CalibBlockWidth);
                ControlTool.GetParamSave(nud_CalibBlockWidthTolerance, ref currentCameraManager.m_SizeCheckPointManage.sizeCheckPointParam.CalibBlockWidthTolerance);
                ControlTool.GetParamSave(nud_CalibBlockHeiht, ref currentCameraManager.m_SizeCheckPointManage.sizeCheckPointParam.CalibBlockHeiht);
                ControlTool.GetParamSave(nud_CalibBlockHeihtTolerance, ref currentCameraManager.m_SizeCheckPointManage.sizeCheckPointParam.CalibBlockHeihtTolerance);
                ControlTool.GetParamSave(nud_SizeX1, ref currentCameraManager.m_SizeCheckPointManage.sizeCheckPointParam.EdgeX1);
                ControlTool.GetParamSave(nud_SizeX2, ref currentCameraManager.m_SizeCheckPointManage.sizeCheckPointParam.EdgeX2);
                ControlTool.GetParamSave(num_CalibBlockMinThreshold, ref currentCameraManager.m_SizeCheckPointManage.sizeCheckPointParam.CalibBlockMinThreshold);
                ControlTool.GetParamSave(num_CalibBlockMaxThreshold, ref currentCameraManager.m_SizeCheckPointManage.sizeCheckPointParam.CalibBlockMaxThreshold);
                ControlTool.GetParamSave(nud_SideLengthPixX, ref currentCameraManager.m_SizeCheckPointManage.sizeCheckPointParam.SideLengthPixX);
                ControlTool.GetParamSave(nud_SideLengthPixY, ref currentCameraManager.m_SizeCheckPointManage.sizeCheckPointParam.SideLengthPixY);

                ControlTool.GetParamSave(nud_MesureThreshold, ref currentCameraManager.m_SizeCheckPointManage.sizeCheckPointParam.LineModelParam.MesureThreshold);
                currentCameraManager.m_SizeCheckPointManage.sizeCheckPointParam.Save();
            }
            catch (Exception ex)
            {

                Log4netRecord.ErrorLog.Debug(ex);
            }

        }

        private void btn_SaveParam_Click(object sender, EventArgs e)
        {
            try
            {
                SaveParam();
            }
            catch (Exception ex)
            {

                Log4netRecord.ErrorLog.Debug(ex);
            }

        }

        #endregion

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(Project.Instance.paramPathSet.pointCheckRootPath);
            }
            catch (Exception ex)
            {

            }

        }

        /// <summary>
        /// 消除尺寸波动
        /// </summary>
        /// <param name="measureValue"></param>
        /// <param name=""></param>
        public void EliminateSizeFluctuate(double standardValue, double measureValue, out double eliminateFluctuateValue)
        {
            //对测量值进行顶级运算         
            eliminateFluctuateValue = (2 / 3.0 * standardValue) + (measureValue / 3);
        }

        bool isStartPointCheckFlag = false;
        private void btn_StartPointCheck_Click(object sender, EventArgs e)
        {
            try
            {
                if (isStartPointCheckFlag) return;
                isStartPointCheckFlag = true;

                AutoExecutePointCheck();

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
                MessageBox.Show(ex.ToString());
            }
            isStartPointCheckFlag = false;
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                // 获取文件夹绝对路径    显示在 txtbox 控件里
                using (FolderBrowserDialog folder = new FolderBrowserDialog())
                {
                    if (Project.Instance.paramPathSet.ProductRootPath != null && System.IO.Directory.Exists(Project.Instance.paramPathSet.ProductRootPath))
                    {
                        folder.SelectedPath = Project.Instance.paramPathSet.ProductRootPath;

                    }
                    if (folder.ShowDialog() == DialogResult.OK && folder.SelectedPath != "")
                    {
                        tbx_PointCheckImgPath.Text = folder.SelectedPath;
                        currentCameraManager.m_SizeCheckPointManage.sizeCheckPointParam.PointCheckImgPath = folder.SelectedPath;
                    }
                }

            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// 手动执行点检
        /// </summary>
        /// <param name="Load_Image"></param>
        public void HandExecutePointCheck(HObject Load_Image)
        {
            try
            {
                if (Load_Image == null || !Load_Image.IsInitialized()) return;

                ExecutePointCheck(Load_Image, currentCameraManager, currentCameraManager.m_SizeCheckPointManage.sizeCheckPointParam, out List<RectangleInfo> rectangles);

                string filePath = Project.Instance.paramPathSet.pointCheckRootPath + DateTime.Now.ToString("yyyy.MM.dd");
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                filePath += "\\" + $"{currentCameraManager.CameraNameEnum}{Project.Instance.paramPathSet.SizeCheckPointDataName}";
                //数据保存和显示                
                DataShowAndSave(currentCameraManager.CameraNameEnum, filePath, rectangles, currentCameraManager.m_SizeCheckPointManage.sizeCheckPointParam.SizeDJResult);

                //显示图像
                displayWindow.RepaintCam(new Operation_ImageInfo() { showHalconWindow = currentShowHalconWindow }, out Operation_ImageInfo screenshotImage);
                screenshotImage?.Dispose();
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }

        }

        /// <summary>
        /// 自动执行点检
        /// </summary>
        public void AutoExecutePointCheck()
        {
            try
            {
                string strResult = "";
                bool pointCheckResult = true;

                string filePath = Project.Instance.paramPathSet.pointCheckRootPath + DateTime.Now.ToString("yyyy.MM.dd");
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                filePath += "\\" + DateTime.Now.ToString("HH-mm-ss");
                foreach (var item in cameraManager)
                {                 
                    string fileSavePath = filePath+ $"{item.Key}{Project.Instance.paramPathSet.SizeCheckPointDataName}";

                    //只有相机1和相机2需要进行点检
                    if (item.Key == CameraEnum.Cam1 || item.Key == CameraEnum.Cam2)
                    {
                        //设置当前相机
                        Tv_Select.SelectedNode = Tv_Select.Nodes[(int)item.Key];

                        //加载图像路径
                        m_ImageSimManage.InitSimFlag();
                        m_ImageSimManage.ReadImageSimPath(item.Value.m_SizeCheckPointManage.sizeCheckPointParam.PointCheckImgPath);

                        //开始点检                        
                        SizeCheckPointParam sizeCheckPointParam = item.Value.m_SizeCheckPointManage.sizeCheckPointParam.Clone();//复制一份参数，避免影响实时点检
                        sizeCheckPointParam.ResetStartDJ();

                        int imgNum = m_ImageSimManage.ImageEmulationPaths.Length > sizeCheckPointParam.CheckNumber ? sizeCheckPointParam.CheckNumber : m_ImageSimManage.ImageEmulationPaths.Length;
                        for (int i = 0; i < imgNum; i++)
                        {
                            m_ImageSimManage.GetZeroIndexStartNextImage(out HObject Load_Image);
                            ExecutePointCheck(Load_Image, item.Value, sizeCheckPointParam, out List<RectangleInfo> rectangles);

                            //数据保存和显示
                           
                            DataShowAndSave(item.Key, fileSavePath, rectangles, sizeCheckPointParam.SizeDJResult);
                        }

                        //结果
                        string pointCheckResultStr = "尺寸点检结果";
                        string pointCheckResultStr1 = "通过";
                        string pointCheckResultStr2 = "不通过";
                        strResult += item.Key.ToString() + pointCheckResultStr + "：" + (sizeCheckPointParam.SizeDJResult ? pointCheckResultStr1 : pointCheckResultStr2) + "\r\n";

                        pointCheckResult &= sizeCheckPointParam.SizeDJResult;

                    }
                }
                lb_PointCheckResult.Text = strResult;

                if (pointCheckResult)
                {
                    Project.Instance.systemInfo.djFinish = true;
                    Project.Instance.systemInfo.djLastOKTime = DateTime.Now;
                    Project.Instance.systemInfo.Save();
                    // 写入点检日志
                    PointCheck.FileOperationINI.Write_SendLog_ini(Project.Instance.paramPathSet.CheckPointRecordPath, strResult);
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }

        public void ExecutePointCheck(HObject Load_Image, CameraManager camManager, SizeCheckPointParam sizeCheckPointParam, out List<RectangleInfo> RectangleInfos)
        {
            RectangleInfos = null;
            try
            {
                if (Load_Image == null || !Load_Image.IsInitialized()) return;

                // 释放之前的数据              
                currentShowHalconWindow.Dispose();
                currentShowHalconWindow.m_hImage = Load_Image;

                //点检              
                PointCheck.DJProcess(camManager.CameraNameEnum, Load_Image,
                    camManager.m_CamParam.CalibrationValue_X, camManager.m_CamParam.CalibrationValue_Y,
                    sizeCheckPointParam, out HObject preproccessRegion, out List<RectangleInfo> rectangles);

                //添加至显示窗口
                currentShowHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = preproccessRegion, m_stuGetColor = HalconColor.red });
                RectangleInfos = rectangles;

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
            }

        }

        /// <summary>
        /// 显示结果并保存数据
        /// </summary>
        /// <param name="cameraEnum"></param>
        /// <param name="fileSavePath"></param>
        /// <param name="rectangles"></param>
        /// <param name="result"></param>
        public void DataShowAndSave(CameraEnum cameraEnum, string fileSavePath, List<RectangleInfo> rectangles, bool result)
        {

            if (rectangles != null)
            {
                //显示结果
                for (int i = 0; i < rectangles.Count; i++)
                {
                    string[] data = new string[]
                    {
                                         rectangles[i].widthReality.ToString("F2"),
                                         rectangles[i].widthResult?"OK":"NG",
                                         rectangles[i].heightReality.ToString("F2"),
                                         rectangles[i].heightResult?"OK":"NG",
                                         result?"OK":"NG"
                    };
                    NListView2.Add(listView1, data, 1000, ListViewShowType.RemoveAtStartRow, true);

                    // 写入点检数据
                    string[] pointCheckDataTitle = PointCheck.GetPointCheckTitles();

                    FileOperationCSV.WriteData_CSV(fileSavePath, pointCheckDataTitle, data, cameraEnum.ToString());

                    //添加至显示窗口
                    currentShowHalconWindow.m_lineShowInfos.Add(new LineInfo() { RowBegin = rectangles[i].leftEdgeBeginRow, ColBegin = rectangles[i].leftEdgeBeginCol, RowEnd = rectangles[i].leftEdgeEndRow, ColEnd = rectangles[i].leftEdgeEndCol, Color = HalconColor.green });
                    currentShowHalconWindow.m_lineShowInfos.Add(new LineInfo() { RowBegin = rectangles[i].topEdgeBeginRow, ColBegin = rectangles[i].topEdgeBeginCol, RowEnd = rectangles[i].topEdgeEndRow, ColEnd = rectangles[i].topEdgeEndCol, Color = HalconColor.green });
                    currentShowHalconWindow.m_lineShowInfos.Add(new LineInfo() { RowBegin = rectangles[i].rightEdgeBeginRow, ColBegin = rectangles[i].rightEdgeBeginCol, RowEnd = rectangles[i].rightEdgeEndRow, ColEnd = rectangles[i].rightEdgeEndCol, Color = HalconColor.green });
                    currentShowHalconWindow.m_lineShowInfos.Add(new LineInfo() { RowBegin = rectangles[i].bottomEdgeBeginRow, ColBegin = rectangles[i].bottomEdgeBeginCol, RowEnd = rectangles[i].bottomEdgeEndRow, ColEnd = rectangles[i].bottomEdgeEndCol, Color = HalconColor.green });
                }
            }
        }
    }

}

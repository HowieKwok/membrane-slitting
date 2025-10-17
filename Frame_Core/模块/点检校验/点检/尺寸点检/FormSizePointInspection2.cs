using Basler.Pylon;
using Frame_Cor;
using HalconDotNet;
using LogTool;
using NPOI.SS.Formula.Functions;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace Frame_Core
{
    public partial class FormSizePointInspection2 : Form
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
            ControlTool.SetParamShow(NUD_RoiNum, ref RoiPointsNum);
            ControlTool.SetParamShow(NUD_RoiWidth, ref RoiLength1);
            ControlTool.SetParamShow(NUD_RoiHeight, ref RoiLength2);
            ControlTool.SetParamShow(NUD_RoiAngle, ref RoiAngle);
            ControlTool.SetParamShow(NUD_RoiMidRow, ref RoiMidRow);
            ControlTool.SetParamShow(NUD_RoiMidColumn, ref RoiMidColumn);

            if (CurrentLineParam != null)
            {
                //设置ROISize
                displayWindow.m_roiRectangle2_Cam.size = (double)nud_ROISize.Value;

                //设置基准点坐标和偏移量
                ControlTool.SetParamShow(nud_BasePointX, ref CurrentLineParam.BasePointX);
                ControlTool.SetParamShow(nud_BasePointY, ref CurrentLineParam.BasePointY);
                int BaseOffsetX = (int)(RoiMidColumn - CurrentLineParam.BasePointX);
                int BaseOffsetY = (int)(RoiMidRow - CurrentLineParam.BasePointY);
                ControlTool.SetParamShow(nud_BaseOffsetX, ref BaseOffsetX);
                ControlTool.SetParamShow(nud_BaseOffsetY, ref BaseOffsetY);
            }

        }

        /// <summary>
        /// 获取ROI参数并显示
        /// </summary>
        /// <param name="roiRectangle2"></param>
        /// <param name="measLineParams"></param>
        public void GetRoiParams(ROIRectangle2 roiRectangle2, out int RoiPointsNum, out double RoiMidRow, out double RoiMidColumn, out double RoiAngle, out double RoiLength1, out double RoiLength2)
        {
            roiRectangle2.ROIName = "";
            HTuple hv_Phi;
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
        CameraManager currentCameraManager;
        SizePointInspectionParam CurrentSizePointInspectionParam;
        Param_Line CurrentLineParam = null;

        public FormSizePointInspection2(CameraEnum camera, CameraManager _cameraManager)
        {
            InitializeComponent();
            currentCamera = camera;
            currentCameraManager = _cameraManager;
            CurrentSizePointInspectionParam = currentCameraManager.m_SizePointInspectionManage.sizePointInspectionParam;
        }

        private void FormSizePointInspection2_Load(object sender, EventArgs e)
        {
            try
            {

                CB_MeasureTransition.DataSource = Enum.GetNames(typeof(MeasureTransition)).Select(x => LanguageProject.GetLangeuageAttribute((MeasureTransition)Enum.Parse(typeof(MeasureTransition), x), Project.Instance.systemInfo.m_CurrentLanguageType)).ToArray();
                CB_MeasureSelect.DataSource = Enum.GetNames(typeof(MeasureLine)).Select(x => LanguageProject.GetLangeuageAttribute((MeasureLine)Enum.Parse(typeof(MeasureLine), x), Project.Instance.systemInfo.m_CurrentLanguageType)).ToArray();
                CB_MeasureIORegions.DataSource = Enum.GetNames(typeof(MeasureIORegions)).Select(x => LanguageProject.GetLangeuageAttribute((MeasureIORegions)Enum.Parse(typeof(MeasureIORegions), x), Project.Instance.systemInfo.m_CurrentLanguageType)).ToArray();
                CB_MeasureInterpolation.DataSource = Enum.GetNames(typeof(MeasureInterpolation)).Select(x => LanguageProject.GetLangeuageAttribute((MeasureInterpolation)Enum.Parse(typeof(MeasureInterpolation), x), Project.Instance.systemInfo.m_CurrentLanguageType)).ToArray();
                CB_LineSortType.DataSource = Enum.GetNames(typeof(SortEnum)).Select(x => LanguageProject.GetLangeuageAttribute((SortEnum)Enum.Parse(typeof(SortEnum), x), Project.Instance.systemInfo.m_CurrentLanguageType)).ToArray();
                cb_SizePointInspectionArithmetic.DataSource = Enum.GetNames(typeof(SizePointInspectionArithmeticEnum)).Select(x => LanguageProject.GetLangeuageAttribute((SizePointInspectionArithmeticEnum)Enum.Parse(typeof(SizePointInspectionArithmeticEnum), x), Project.Instance.systemInfo.m_CurrentLanguageType)).ToArray();


                InitWindow();

                ShowFormParam(CurrentSizePointInspectionParam);

                m_ImageSimManage.ReadImageSimPath(CurrentSizePointInspectionParam.PointCheckImgPath);

                NListView2.Init(listView1, SizePointInspectionWorks.GetSizePointInspectionTitle(CurrentSizePointInspectionParam.SizeLogicParams_UserDefineds).ToArray());
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
                MessageBox.Show(ex.ToString());
            }
        }

        private void FormSizePointInspection2_FormClosing(object sender, FormClosingEventArgs e)
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

        private void btn_Save_Click(object sender, EventArgs e)
        {
            try
            {
                SaveParam();
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

                GetFormParam(ref CurrentSizePointInspectionParam);

                currentCameraManager.m_SizePointInspectionManage.sizePointInspectionParam.Save();
            }
            catch (Exception ex)
            {

                Log4netRecord.ErrorLog.Debug(ex);
                MessageBox.Show(ex.ToString());
            }
        }

        public void ShowFormParam(SizePointInspectionParam sizePointInspectionParam)
        {
            try
            {

                ControlTool.SetParamShow(tbx_PointCheckImgPath, ref sizePointInspectionParam.PointCheckImgPath);
                ControlTool.SetParamShow(cb_SizePointInspectionArithmetic, ref sizePointInspectionParam.SizePointInspectionArithmetic);

                ControlTool.SetParamShow(cbx_SSDJ, ref sizePointInspectionParam.sizePointInspectionWorkParam.SSDJ);
                ControlTool.SetParamShow(nud_PointCheckSpeedRestrict, ref sizePointInspectionParam.sizePointInspectionWorkParam.PointCheckSpeedRestrict);
                ControlTool.SetParamShow(nud_CheckIntervalFrame, ref sizePointInspectionParam.sizePointInspectionWorkParam.CheckIntervalFrame);
                ControlTool.SetParamShow(nud_CheckNumber, ref sizePointInspectionParam.sizePointInspectionWorkParam.CheckNumber);
                ControlTool.SetParamShow(nud_FailAccumulativeNum, ref sizePointInspectionParam.sizePointInspectionWorkParam.FailAccumulativeNum);
                ControlTool.SetParamShow(cbx_Alarm, ref sizePointInspectionParam.sizePointInspectionWorkParam.Alarm);
                ControlTool.SetParamShow(cbx_Stop, ref sizePointInspectionParam.sizePointInspectionWorkParam.Stop);

                ShowFormParam(ref sizePointInspectionParam.p_RectangleFilm);
                ShowFormParam(ref sizePointInspectionParam.p_LineWidthFilm);

                Refresh_dgv_SizeLogic_UserDefineds(CurrentSizePointInspectionParam.SizeLogicParams_UserDefineds);

                Refresh_lbx_LineItems(CurrentSizePointInspectionParam.LineParams, 0);
            }
            catch (Exception ex)
            {

                Log4netRecord.ErrorLog.Debug(ex);
                MessageBox.Show(ex.ToString());
            }
        }

        public void GetFormParam(ref SizePointInspectionParam sizePointInspectionParam)
        {
            try
            {

                ControlTool.GetParamSave(tbx_PointCheckImgPath, ref sizePointInspectionParam.PointCheckImgPath);
                ControlTool.GetParamSave(cb_SizePointInspectionArithmetic, ref sizePointInspectionParam.SizePointInspectionArithmetic);

                ControlTool.GetParamSave(cbx_SSDJ, ref sizePointInspectionParam.sizePointInspectionWorkParam.SSDJ);
                ControlTool.GetParamSave(nud_PointCheckSpeedRestrict, ref sizePointInspectionParam.sizePointInspectionWorkParam.PointCheckSpeedRestrict);
                ControlTool.GetParamSave(nud_CheckIntervalFrame, ref sizePointInspectionParam.sizePointInspectionWorkParam.CheckIntervalFrame);
                ControlTool.GetParamSave(nud_CheckNumber, ref sizePointInspectionParam.sizePointInspectionWorkParam.CheckNumber);
                ControlTool.GetParamSave(nud_FailAccumulativeNum, ref sizePointInspectionParam.sizePointInspectionWorkParam.FailAccumulativeNum);
                ControlTool.GetParamSave(cbx_Alarm, ref sizePointInspectionParam.sizePointInspectionWorkParam.Alarm);
                ControlTool.GetParamSave(cbx_Stop, ref sizePointInspectionParam.sizePointInspectionWorkParam.Stop);


                GetFormParam(ref sizePointInspectionParam.p_RectangleFilm);
                GetFormParam(ref sizePointInspectionParam.p_LineWidthFilm);

                Get_dgv_SizeLogic_UserDefineds(ref CurrentSizePointInspectionParam.SizeLogicParams_UserDefineds);

                int index = lbx_LineItems.SelectedIndex;
                if (index >= 0 && index < CurrentSizePointInspectionParam.LineParams.Count)
                {
                    Param_Line lineParam = CurrentSizePointInspectionParam.LineParams[index];
                    GetLineParam(ref lineParam);
                }
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
                        m_ImageSimManage.InitSimFlag();
                        m_ImageSimManage.ReadImageSimPath(tbx_PointCheckImgPath.Text);
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
            m_ImageSimManage.ReadImageSimPath(tbx_PointCheckImgPath.Text);
        }
        public void Execute(HObject ho_Image, string imgPath)
        {
            try
            {
                // 释放之前的数据
                currentShowHalconWindow.Dispose();
                currentShowHalconWindow.m_hImage = ho_Image;

                displayWindow.m_hWndCtrl_Display_Cam.bl_RoiOperateFlag = false;//不显示测量矩形ROI
                string str2 = "图像路径";
                currentShowHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = str2 + ":" + imgPath, m_stuGetColor = HalconColor.green });

                double time = 0;
                //获取界面参数
                SizePointInspectionParam TempSizePointInspectionParam = CurrentSizePointInspectionParam;

                if (TempSizePointInspectionParam.SizePointInspectionArithmetic == (int)SizePointInspectionArithmeticEnum.矩形菲林片)
                {
                    //
                    ExecuteResult executeResult = SizePointInspectionWorks.SizeMeasureFlow_FilmRectangle(currentCamera, ho_Image, CurrentSizePointInspectionParam, ref currentShowHalconWindow, out List<List<SizeInfo_P>> sizeInfos);
                    time = executeResult.Time;
                    //点检              
                    HOperatorSet.GetImageSize(ho_Image, out HTuple imageWidth, out HTuple imageHeight);
                    currentShowHalconWindow.m_lineShowInfos.Add(new LineInfo() { RowBegin = 0, ColBegin = TempSizePointInspectionParam.p_RectangleFilm.EdgeX1, RowEnd = imageHeight - 1, ColEnd = TempSizePointInspectionParam.p_RectangleFilm.EdgeX1, Color = HalconColor.green });
                    currentShowHalconWindow.m_lineShowInfos.Add(new LineInfo() { RowBegin = 0, ColBegin = TempSizePointInspectionParam.p_RectangleFilm.EdgeX2, RowEnd = imageHeight - 1, ColEnd = TempSizePointInspectionParam.p_RectangleFilm.EdgeX2, Color = HalconColor.green });
                }
                else
                {
                    ExecuteResult executeResult = SizePointInspectionWorks.SizeMeasureFlow_FilmLineWidth(currentCamera, ho_Image, CurrentSizePointInspectionParam, ref currentShowHalconWindow, out List<List<SizeInfo_P>> sizeInfos);
                    time = executeResult.Time;

                    //点检              
                    HOperatorSet.GetImageSize(ho_Image, out HTuple imageWidth, out HTuple imageHeight);
                    currentShowHalconWindow.m_lineShowInfos.Add(new LineInfo() { RowBegin = 0, ColBegin = TempSizePointInspectionParam.p_LineWidthFilm.EdgeX1, RowEnd = imageHeight - 1, ColEnd = TempSizePointInspectionParam.p_LineWidthFilm.EdgeX1, Color = HalconColor.green });
                    currentShowHalconWindow.m_lineShowInfos.Add(new LineInfo() { RowBegin = 0, ColBegin = TempSizePointInspectionParam.p_LineWidthFilm.EdgeX2, RowEnd = imageHeight - 1, ColEnd = TempSizePointInspectionParam.p_LineWidthFilm.EdgeX2, Color = HalconColor.green });
                }


                displayWindow.RepaintCam(new Operation_ImageInfo() { showHalconWindow = currentShowHalconWindow }, out _);
                UpdateTime(time);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }



        #region 矩形菲林片
        public void ShowFormParam(ref RectangleFilmParam rectangleFilmParam)
        {

            ControlTool.SetParamShow(nud_SizeX1, ref rectangleFilmParam.EdgeX1);
            ControlTool.SetParamShow(nud_SizeX2, ref rectangleFilmParam.EdgeX2);
            ControlTool.SetParamShow(num_CalibBlockMinThreshold, ref rectangleFilmParam.CalibBlockMinThreshold);
            ControlTool.SetParamShow(num_CalibBlockMaxThreshold, ref rectangleFilmParam.CalibBlockMaxThreshold);
            ControlTool.SetParamShow(nud_SideLengthPixX, ref rectangleFilmParam.SideLengthPixX);
            ControlTool.SetParamShow(nud_SideLengthPixY, ref rectangleFilmParam.SideLengthPixY);

        }

        public void GetFormParam(ref RectangleFilmParam rectangleFilmParam)
        {


            ControlTool.GetParamSave(nud_SizeX1, ref rectangleFilmParam.EdgeX1);
            ControlTool.GetParamSave(nud_SizeX2, ref rectangleFilmParam.EdgeX2);
            ControlTool.GetParamSave(num_CalibBlockMinThreshold, ref rectangleFilmParam.CalibBlockMinThreshold);
            ControlTool.GetParamSave(num_CalibBlockMaxThreshold, ref rectangleFilmParam.CalibBlockMaxThreshold);
            ControlTool.GetParamSave(nud_SideLengthPixX, ref rectangleFilmParam.SideLengthPixX);
            ControlTool.GetParamSave(nud_SideLengthPixY, ref rectangleFilmParam.SideLengthPixY);

        }

        #endregion

        #region 线宽菲林片
        public void ShowFormParam(ref LineWidthFilmParam lineWidthFilmParam)
        {

            ControlTool.SetParamShow(nud_SizeX1_LineWidth, ref lineWidthFilmParam.EdgeX1);
            ControlTool.SetParamShow(nud_SizeX2_LineWidth, ref lineWidthFilmParam.EdgeX2);
            ControlTool.SetParamShow(nud_FilmMinHeight_LineWidth, ref lineWidthFilmParam.FilmMinHeight);
            ControlTool.SetParamShow(nud_FilmMaxHeight_LineWidth, ref lineWidthFilmParam.FilmMaxHeight);
            ControlTool.SetParamShow(nud_CalibBlockMinThreshold_LineW, ref lineWidthFilmParam.CalibBlockMinThreshold);
            ControlTool.SetParamShow(nud_CalibBlockMaxThreshold_LineW, ref lineWidthFilmParam.CalibBlockMaxThreshold);
            ControlTool.SetParamShow(nud_DetectionStartRowToCentreDistance, ref lineWidthFilmParam.DetectionStartRowToCentreDistance);

        }

        public void GetFormParam(ref LineWidthFilmParam lineWidthFilmParam)
        {


            ControlTool.GetParamSave(nud_SizeX1_LineWidth, ref lineWidthFilmParam.EdgeX1);
            ControlTool.GetParamSave(nud_SizeX2_LineWidth, ref lineWidthFilmParam.EdgeX2);
            ControlTool.GetParamSave(nud_FilmMinHeight_LineWidth, ref lineWidthFilmParam.FilmMinHeight);
            ControlTool.GetParamSave(nud_FilmMaxHeight_LineWidth, ref lineWidthFilmParam.FilmMaxHeight);
            ControlTool.GetParamSave(nud_CalibBlockMinThreshold_LineW, ref lineWidthFilmParam.CalibBlockMinThreshold);
            ControlTool.GetParamSave(nud_CalibBlockMaxThreshold_LineW, ref lineWidthFilmParam.CalibBlockMaxThreshold);
            ControlTool.GetParamSave(nud_DetectionStartRowToCentreDistance, ref lineWidthFilmParam.DetectionStartRowToCentreDistance);


        }


        #endregion

        #region 抓边参数
        private void ShowLineParam(Param_Line lineParam)
        {
            if (lineParam == null) return;
            ControlTool.SetParamShow(NUD_MeasureThreshold, ref lineParam.MesureThreshold);
            ControlTool.SetParamShow(NUD_MeasureSigma, ref lineParam.MesureSigma);
            ControlTool.SetParamShow(CB_MeasureTransition, ref lineParam.MeasureTransition_);
            ControlTool.SetParamShow(CB_MeasureSelect, ref lineParam.MeasureSelect);
            ControlTool.SetParamShow(NUD_MeasureMinScore, ref lineParam.MeasureMinScore);
            ControlTool.SetParamShow(NUD_MeasureNumInstances, ref lineParam.MeasureNumInstances);
            ControlTool.SetParamShow(NUD_MeasureDistThreshold, ref lineParam.MeasureDistThreshold);
            ControlTool.SetParamShow(NUD_MaxNumIterations, ref lineParam.MaxNumIterations);
            ControlTool.SetParamShow(CB_MeasureIORegions, ref lineParam.MeasureIORegions_);
            ControlTool.SetParamShow(CB_MeasureInterpolation, ref lineParam.MeasureInterpolation_);

            ControlTool.SetParamShow(CB_LineSortType, ref lineParam.LineSortType);

            ControlTool.SetParamShow(NUD_RoiNum, ref lineParam.RoiPointsNum);
            ControlTool.SetParamShow(NUD_RoiWidth, ref lineParam.RoiLength1);
            ControlTool.SetParamShow(NUD_RoiHeight, ref lineParam.RoiLength2);
            ControlTool.SetParamShow(NUD_RoiAngle, ref lineParam.RoiAngle);
            ControlTool.SetParamShow(NUD_RoiMidRow, ref lineParam.RoiMidRow);
            ControlTool.SetParamShow(NUD_RoiMidColumn, ref lineParam.RoiMidColumn);

            ControlTool.SetParamShow(nud_ROISize, ref lineParam.ROISize);

            ControlTool.SetParamShow(cb_IsROIFollow, ref lineParam.IsROIFollow);
            ControlTool.SetParamShow(nud_BasePointX, ref lineParam.BasePointX);
            ControlTool.SetParamShow(nud_BasePointY, ref lineParam.BasePointY);
            ControlTool.SetParamShow(nud_BaseOffsetX, ref lineParam.BaseOffsetX);
            ControlTool.SetParamShow(nud_BaseOffsetY, ref lineParam.BaseOffsetY);
        }
        private void GetLineParam(ref Param_Line lineParam)
        {

            if (lineParam == null) return;

            ControlTool.GetParamSave(NUD_MeasureThreshold, ref lineParam.MesureThreshold);
            ControlTool.GetParamSave(NUD_MeasureSigma, ref lineParam.MesureSigma);
            ControlTool.GetParamSave(CB_MeasureTransition, ref lineParam.MeasureTransition_);
            ControlTool.GetParamSave(CB_MeasureSelect, ref lineParam.MeasureSelect);
            ControlTool.GetParamSave(NUD_MeasureMinScore, ref lineParam.MeasureMinScore);
            ControlTool.GetParamSave(NUD_MeasureNumInstances, ref lineParam.MeasureNumInstances);
            ControlTool.GetParamSave(NUD_MeasureDistThreshold, ref lineParam.MeasureDistThreshold);
            ControlTool.GetParamSave(NUD_MaxNumIterations, ref lineParam.MaxNumIterations);
            ControlTool.GetParamSave(CB_MeasureIORegions, ref lineParam.MeasureIORegions_);
            ControlTool.GetParamSave(CB_MeasureInterpolation, ref lineParam.MeasureInterpolation_);

            ControlTool.GetParamSave(CB_LineSortType, ref lineParam.LineSortType);

            ControlTool.GetParamSave(NUD_RoiNum, ref lineParam.RoiPointsNum);
            ControlTool.GetParamSave(NUD_RoiWidth, ref lineParam.RoiLength1);
            ControlTool.GetParamSave(NUD_RoiHeight, ref lineParam.RoiLength2);
            ControlTool.GetParamSave(NUD_RoiAngle, ref lineParam.RoiAngle);
            ControlTool.GetParamSave(NUD_RoiMidRow, ref lineParam.RoiMidRow);
            ControlTool.GetParamSave(NUD_RoiMidColumn, ref lineParam.RoiMidColumn);

            ControlTool.GetParamSave(nud_ROISize, ref lineParam.ROISize);

            ControlTool.GetParamSave(cb_IsROIFollow, ref lineParam.IsROIFollow);
            ControlTool.GetParamSave(nud_BasePointX, ref lineParam.BasePointX);
            ControlTool.GetParamSave(nud_BasePointY, ref lineParam.BasePointY);
            ControlTool.GetParamSave(nud_BaseOffsetX, ref lineParam.BaseOffsetX);
            ControlTool.GetParamSave(nud_BaseOffsetY, ref lineParam.BaseOffsetY);
        }
        private void Refresh_lbx_LineItems(List<Param_Line> lineParams, int index)
        {
            lbx_LineItems.Items.Clear();
            for (int i = 0; i < lineParams.Count; i++)
            {
                lbx_LineItems.Items.Add(lineParams[i].LineName);
            }
            if (index >= 0 && index < lbx_LineItems.Items.Count) lbx_LineItems.SelectedIndex = index;
        }
        private void lbx_LineItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = lbx_LineItems.SelectedIndex;
            if (index >= 0 && index < CurrentSizePointInspectionParam.LineParams.Count)
            {
                CurrentLineParam = CurrentSizePointInspectionParam.LineParams[index];
                ShowLineParam(CurrentLineParam);
                displayWindow.m_hWndCtrl_Display_Cam.bl_RoiOperateFlag = true;//显示测量矩形ROI
                SetRoiParams(currentShowHalconWindow.m_hImage,
                    CurrentLineParam.RoiPointsNum, CurrentLineParam.RoiMidRow, CurrentLineParam.RoiMidColumn, CurrentLineParam.RoiAngle, CurrentLineParam.RoiLength1, CurrentLineParam.RoiLength2,
                    displayWindow.m_roiRectangle2_Cam, displayWindow.m_roiController_Cam);
                displayWindow.m_hWndCtrl_Display_Cam.repaint();
            }
        }


        private void 添加ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("", "询问", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            CurrentSizePointInspectionParam.LineParams.Add(new Param_Line() { LineName = $"Line{CurrentSizePointInspectionParam.LineParams.Count + 1}" });
            Refresh_lbx_LineItems(CurrentSizePointInspectionParam.LineParams, CurrentSizePointInspectionParam.LineParams.Count - 1);
        }
        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("", "询问", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            int rowIndex = lbx_LineItems.SelectedIndex;
            if (rowIndex < 0) return;    
            CurrentSizePointInspectionParam.LineParams.RemoveAt(CurrentSizePointInspectionParam.LineParams.Count - 1);
            Refresh_lbx_LineItems(CurrentSizePointInspectionParam.LineParams, CurrentSizePointInspectionParam.LineParams.Count - 1);
        }
        private void 删除所有ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("", "询问", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            CurrentSizePointInspectionParam.LineParams.Clear();
            Refresh_lbx_LineItems(CurrentSizePointInspectionParam.LineParams, -1);
        }
        #endregion

        #region 检测项
        private void Refresh_dgv_SizeLogic_UserDefineds(List<Param_SizeLogic_UserDefined> SizeLogic_UserDefinedsParam)
        {

            DataGridViewComboBoxColumn computationalLogic = dgv_SizeLogic_UserDefined.Columns[3] as DataGridViewComboBoxColumn;
            if (computationalLogic == null) return;
            else computationalLogic.DataSource = Enum.GetNames(typeof(SizeComputationalLogicEnum));

            dgv_SizeLogic_UserDefined.Rows.Clear();
            for (int i = 0; i < SizeLogic_UserDefinedsParam.Count; i++)
            {
                dgv_SizeLogic_UserDefined.Rows.Add();
                dgv_SizeLogic_UserDefined.Rows[i].Cells[0].Value = SizeLogic_UserDefinedsParam[i].Name;
                dgv_SizeLogic_UserDefined.Rows[i].Cells[1].Value = SizeLogic_UserDefinedsParam[i].Line1Name;
                dgv_SizeLogic_UserDefined.Rows[i].Cells[2].Value = SizeLogic_UserDefinedsParam[i].Line2Name;
                dgv_SizeLogic_UserDefined.Rows[i].Cells[3].Value = computationalLogic.Items[SizeLogic_UserDefinedsParam[i].SizeCalculateLogic];
                dgv_SizeLogic_UserDefined.Rows[i].Cells[4].Value = SizeLogic_UserDefinedsParam[i].StandardValue;
                dgv_SizeLogic_UserDefined.Rows[i].Cells[5].Value = SizeLogic_UserDefinedsParam[i].ToleranceValue;
                dgv_SizeLogic_UserDefined.Rows[i].Cells[6].Value = SizeLogic_UserDefinedsParam[i].CoefficientValue;
            }
        }

        private void Get_dgv_SizeLogic_UserDefineds(ref List<Param_SizeLogic_UserDefined> SizeLogic_UserDefinedsParam)
        {
            DataGridViewComboBoxColumn computationalLogic = dgv_SizeLogic_UserDefined.Columns[3] as DataGridViewComboBoxColumn;
            if (computationalLogic == null) return;

            for (int i = 0; i < dgv_SizeLogic_UserDefined.Rows.Count; i++)
            {
                SizeLogic_UserDefinedsParam[i].Name = Convert.ToString(dgv_SizeLogic_UserDefined.Rows[i].Cells[0].Value);
                SizeLogic_UserDefinedsParam[i].Line1Name = Convert.ToString(dgv_SizeLogic_UserDefined.Rows[i].Cells[1].Value);
                SizeLogic_UserDefinedsParam[i].Line2Name = Convert.ToString(dgv_SizeLogic_UserDefined.Rows[i].Cells[2].Value);
                SizeLogic_UserDefinedsParam[i].SizeCalculateLogic = computationalLogic.Items.IndexOf(dgv_SizeLogic_UserDefined.Rows[i].Cells[3].Value);
                SizeLogic_UserDefinedsParam[i].StandardValue = Convert.ToDouble(dgv_SizeLogic_UserDefined.Rows[i].Cells[4].Value);
                SizeLogic_UserDefinedsParam[i].ToleranceValue = Convert.ToDouble(dgv_SizeLogic_UserDefined.Rows[i].Cells[5].Value);
                SizeLogic_UserDefinedsParam[i].CoefficientValue = Convert.ToDouble(dgv_SizeLogic_UserDefined.Rows[i].Cells[6].Value);
            }
        }

        private void 添加ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("", "询问", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            CurrentSizePointInspectionParam.SizeLogicParams_UserDefineds.Add(new Param_SizeLogic_UserDefined());
            Refresh_dgv_SizeLogic_UserDefineds(CurrentSizePointInspectionParam.SizeLogicParams_UserDefineds);
        }
        private void 删除ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("", "询问", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            if (dgv_SizeLogic_UserDefined.RowCount <= 0) return;
            int rowIndex = dgv_SizeLogic_UserDefined.CurrentRow.Index;

            CurrentSizePointInspectionParam.SizeLogicParams_UserDefineds.RemoveAt(rowIndex);
            Refresh_dgv_SizeLogic_UserDefineds(CurrentSizePointInspectionParam.SizeLogicParams_UserDefineds);

        }
        private void 删除所有ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("", "询问", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            CurrentSizePointInspectionParam.SizeLogicParams_UserDefineds.Clear();
            Refresh_dgv_SizeLogic_UserDefineds(CurrentSizePointInspectionParam.SizeLogicParams_UserDefineds);
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

        private void btn_StartSizePointInspection_Click(object sender, EventArgs e)
        {
            string str = "尺寸点检";
            string filePath = Project.Instance.paramPathSet.pointCheckRootPath + DateTime.Now.ToString("yyyy.MM.dd_HH-mm-ss-fff") + str;
            if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);

            NListView2.Init(listView1, SizePointInspectionWorks.GetSizePointInspectionTitle(CurrentSizePointInspectionParam.SizeLogicParams_UserDefineds).ToArray());
            ExecuteResult executeResult = SizePointInspectionWorks.OnekeyPointInspectionFlow(currentCamera, filePath + $"\\{str}_{currentCamera}", out bool isPass, out List<RowData[]> sizeDatas);
            NListView2.ClearItems(listView1);
            NListView2.Add(listView1, sizeDatas, 1000, ListViewShowType.RemoveAtStartRowAndEndVisible, false);

            UpdateTime(executeResult.Time);
        }

        private void UpdateTime(double time)
        {
            lb_Time.Text = $"Time：{time}ms";
        }




    }
}

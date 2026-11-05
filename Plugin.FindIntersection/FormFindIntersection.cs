using Frame_Core;
using HalconDotNet;
using System;
using System.Drawing;
using System.Windows.Forms;
using ViewROI;

using static log4net.Appender.ColoredConsoleAppender;

namespace Plugin.FindIntersection
{
    
    public partial class FormFindIntersection : FormPluginBase
    {
        private FindIntersectionObj pluginsObj { get; set; }
        public ParamsFindIntersection PluginParams { get; set; }

        bool crosshairDragActive = false;   // 右键是否按下

        public HObject ho_Image1 = null;

        // ==================== Dual ROI ====================
        bool bl_RoiExist_Line1 = false;
        bool bl_RoiExist_Line2 = false;

        bool bl_RefreshObj = true;
        bool bl_ShowObj = false;

        // Prevent feedback loop when updating NUD from ROI drag
        bool bl_UpdatingFromROI = false;

        // Track ROI drag state for auto re-execution after mouseUp
        int lastActiveROIidx = -1;

        // Crosshair & gray profile
        private double crossRow = -1, crossCol = -1;
        private double[] hProfileData = null;
        private double[] vProfileData = null;
        private int imgWidth = 0, imgHeight = 0;
        private System.DateTime lastProfileUpdate = System.DateTime.MinValue;
        private bool crosshairEnabled = false;

        public HWndCtrl hWndCtrl_Display;
        public ROIController roiController = new ROIController();
        public ROIRectangle2 roiRectangle2_Line1 = new ROIRectangle2();
        public ROIRectangle2 roiRectangle2_Line2 = new ROIRectangle2();

        #region Anti-flicker Win32
        [System.Runtime.InteropServices.DllImport("user32")]
        private static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, IntPtr lParam);
        private const int WM_SETREDRAW = 0xB;
        #endregion

        public FormFindIntersection()
        {
            InitializeComponent();

            this.TB_PositionAndGray.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;   // 加边框
            this.TB_PositionAndGray.BackColor = System.Drawing.Color.FromArgb(50, 50, 55);      // 稍亮背景
            this.TB_PositionAndGray.Text = "Row: ---   Col: ---   Gray: ---";                    // 显示占位文本

            // Enable double buffering for chart panels
            typeof(System.Windows.Forms.Panel).InvokeMember("DoubleBuffered",
                System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic,
                null, panelHProfile, new object[] { true });
            typeof(System.Windows.Forms.Panel).InvokeMember("DoubleBuffered",
                System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic,
                null, panelVProfile, new object[] { true });

            hWndCtrl_Display = new HWndCtrl(HWC_Display);
            hWndCtrl_Display.useROIController(roiController);
            hWndCtrl_Display.setViewState(HWndCtrl.MODE_VIEW_ZOOM);
            hWndCtrl_Display.setViewState(HWndCtrl.MODE_VIEW_MOVE);

            // Register ViewROI callbacks
            hWndCtrl_Display.ImageProcessingEvent += new HWndCtrl.ImageProcessingDelegate(RunFunction);
            hWndCtrl_Display.ReDrawEvent += new HWndCtrl.ReDrawDelegate(ShowResult);

            HWC_Display.HMouseDown += HWC_Display_HMouseDown;
            HWC_Display.HMouseUp += HWC_Display_HMouseUp;

            // Line 1 ComboBox items
            CB_L1_Transition.Items.AddRange(new object[] { "\u4ece\u660e\u5230\u6697", "\u4ece\u6697\u5230\u660e", "\u4e0d\u533a\u5206\u660e\u6697" });
            CB_L1_Select.Items.AddRange(new object[] { "\u7b2c\u4e00\u6761\u8fb9", "\u6700\u540e\u4e00\u6761\u8fb9", "\u6240\u6709\u8fb9" });
            CB_L1_Interpolation.Items.AddRange(new object[] { "nearest_neighbor", "bilinear", "bicubic" });
            CB_L1_IORegions.Items.AddRange(new object[] { "true", "false" });

            // Line 2 ComboBox items
            CB_L2_Transition.Items.AddRange(new object[] { "\u4ece\u660e\u5230\u6697", "\u4ece\u6697\u5230\u660e", "\u4e0d\u533a\u5206\u660e\u6697" });
            CB_L2_Select.Items.AddRange(new object[] { "\u7b2c\u4e00\u6761\u8fb9", "\u6700\u540e\u4e00\u6761\u8fb9", "\u6240\u6709\u8fb9" });
            CB_L2_Interpolation.Items.AddRange(new object[] { "nearest_neighbor", "bilinear", "bicubic" });
            CB_L2_IORegions.Items.AddRange(new object[] { "true", "false" });
            tabControl1.BackColor = Color.FromArgb(35, 35, 39);
        }


        private void HWC_Display_HMouseDown(object sender, HMouseEventArgs e)
        {
            // 仅右键按下且总开关开启时激活拖动
            if (e.Button == MouseButtons.Right && crosshairEnabled)
            {
                crosshairDragActive = true;

                // 立即定位到点击位置
                if (hWndCtrl_Display.ho_Image_Changed != null)
                {
                    try
                    {
                        HTuple hvRow, hvCol;
                        HOperatorSet.ConvertCoordinatesWindowToImage(
                            HWC_Display.HalconWindow, e.Y, e.X, out hvRow, out hvCol);
                        crossRow = hvRow.D;
                        crossCol = hvCol.D;
                        ExtractGrayProfiles(crossRow, crossCol);
                        panelHProfile.Invalidate();
                        panelVProfile.Invalidate();
                        hWndCtrl_Display.repaint();
                    }
                    catch { }
                }
            }
        }

        private void HWC_Display_HMouseUp(object sender, HMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                hWndCtrl_Display.repaint();
                crosshairDragActive = false;

                // 清空曲线数据，隐藏十字线
                //crossRow = -1;
                //crossCol = -1;
                //hProfileData = null;
                //vProfileData = null;
                //panelHProfile.Invalidate();
                //panelVProfile.Invalidate();
               
            }
        }
        // ==================== Overrides from FormPluginBase ====================

        public override void Init()
        {
            pluginsObj = (FindIntersectionObj)pluginsObjBase;
            PluginParams = (ParamsFindIntersection)pluginsObj.PluginParams;
            this.Text = pluginsObjBase.moduleInfo.ModuleName;
            LoadStandImage();
        }

        public override void Btn_Save_Click(object sender, EventArgs e)
        {
            SaveUIToParams();
            base.Btn_Save_Click(sender, e);
        }

        public override void Btn_RUN_Click(object sender, EventArgs e)
        {
            try
            {
                SaveUIToParams();
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                //  sw.Start();

                base.Btn_RUN_Click(sender, e);
                //sw.Stop();
                //Tb_CostTime.Text = String.Format("\u8017\u65f6\uff1a{0}ms", sw.ElapsedMilliseconds);
                hWndCtrl_Display.repaint();
                UpdateResultDisplay();
            }
            catch (Exception ex)
            {
                MessageBox.Show("\u6267\u884c\u5931\u8d25: " + ex.Message);
            }
        }

        // ==================== Form Load ====================

        private void FormFindIntersection_Load(object sender, EventArgs e)
        {
            ShowFitLineParams(PluginParams);
        }

        // ==================== Image Loading ====================

        private void LoadStandImage()
        {
            try
            {
                if (ho_Image1 != null)
                {
                    if (!pluginsObj.isExistModel)
                    {
                        PluginParams.ho_Image = ho_Image1;
                    }
                }
                if (PluginParams.ho_Image != null && PluginParams.ho_Image.CountObj() != 0)
                {
                    HImage hImage = new HImage();
                    HobjectToHimage(PluginParams.ho_Image, ref hImage);
                    hWndCtrl_Display.addIconicVar(hImage);
                }
            }
            catch { }

            SetRoiParams_Line1(PluginParams, roiRectangle2_Line1, roiController);
            SetRoiParams_Line2(PluginParams, roiRectangle2_Line2, roiController);
            hWndCtrl_Display.resetAll();
            hWndCtrl_Display.repaint();
        }

        private void HobjectToHimage(HObject hobject, ref HImage image)
        {
            HTuple pointer, type, width, height;
            HOperatorSet.GetImagePointer1(hobject, out pointer, out type, out width, out height);
            image.GenImage1(type, width, height, pointer);
        }

        private void LoadImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "Image Files|*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff|All Files|*.*";
            fd.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            if (fd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    PluginParams.ho_Image = new HImage(fd.FileName);
                    ho_Image1 = PluginParams.ho_Image;
                    HImage hImage = new HImage();
                    HobjectToHimage(PluginParams.ho_Image, ref hImage);
                    hWndCtrl_Display.addIconicVar(hImage);

                    SetRoiParams_Line1(PluginParams, roiRectangle2_Line1, roiController);
                    SetRoiParams_Line2(PluginParams, roiRectangle2_Line2, roiController);
                    hWndCtrl_Display.resetAll();
                    hWndCtrl_Display.repaint();
                }
                catch { }
            }
        }

        // ==================== Show UI Params ====================

        private void ShowFitLineParams(ParamsFindIntersection p)
        {
            try
            {
                bl_UpdatingFromROI = true;

                // Line 1 ROI
                NUD_RoiNum_L1.Value = (decimal)p.hv_Line1_RoiPointsNum;
                NUD_RoiWidth_L1.Value = (decimal)p.hv_Line1_RoiWidth;
                NUD_RoiHeight_L1.Value = (decimal)p.hv_Line1_RoiHeight;
                NUD_RoiAngle_L1.Value = (decimal)p.hv_Line1_RoiAngle;
                NUD_RoiMidRow_L1.Value = (decimal)p.hv_Line1_RoiMidRow;
                NUD_RoiMidColumn_L1.Value = (decimal)p.hv_Line1_RoiMidColumn;

                // Line 2 ROI
                NUD_RoiNum_L2.Value = (decimal)p.hv_Line2_RoiPointsNum;
                NUD_RoiWidth_L2.Value = (decimal)p.hv_Line2_RoiWidth;
                NUD_RoiHeight_L2.Value = (decimal)p.hv_Line2_RoiHeight;
                NUD_RoiAngle_L2.Value = (decimal)p.hv_Line2_RoiAngle;
                NUD_RoiMidRow_L2.Value = (decimal)p.hv_Line2_RoiMidRow;
                NUD_RoiMidColumn_L2.Value = (decimal)p.hv_Line2_RoiMidColumn;

                // Line 1 Measure
                NUD_L1_Sigma.Value = (decimal)p.hv_Line1_MeasureSigma;
                NUD_L1_Threshold.Value = (decimal)p.hv_Line1_MeasureThreshold;
                SetTransitionCombo(CB_L1_Transition, p.hv_Line1_MeasureTransition);
                SetSelectCombo(CB_L1_Select, p.hv_Line1_MeasureSelect);
                NUD_L1_MinScore.Value = (decimal)p.hv_Line1_MeasureMinScore;
                NUD_L1_NumInstances.Value = (decimal)p.hv_Line1_MeasureNumInstances;
                NUD_L1_DistThreshold.Value = (decimal)p.hv_Line1_MeasureDistThreshold;
                NUD_L1_MaxIterations.Value = (decimal)p.hv_Line1_MaxNumIterations;
                SetInterpolationCombo(CB_L1_Interpolation, p.hv_Line1_MeasureInterpolation);
                SetIORegionsCombo(CB_L1_IORegions, p.hv_Line1_MeasureIORegions);

                // Line 2 Measure
                NUD_L2_Sigma.Value = (decimal)p.hv_Line2_MeasureSigma;
                NUD_L2_Threshold.Value = (decimal)p.hv_Line2_MeasureThreshold;
                SetTransitionCombo(CB_L2_Transition, p.hv_Line2_MeasureTransition);
                SetSelectCombo(CB_L2_Select, p.hv_Line2_MeasureSelect);
                NUD_L2_MinScore.Value = (decimal)p.hv_Line2_MeasureMinScore;
                NUD_L2_NumInstances.Value = (decimal)p.hv_Line2_MeasureNumInstances;
                NUD_L2_DistThreshold.Value = (decimal)p.hv_Line2_MeasureDistThreshold;
                NUD_L2_MaxIterations.Value = (decimal)p.hv_Line2_MaxNumIterations;
                SetInterpolationCombo(CB_L2_Interpolation, p.hv_Line2_MeasureInterpolation);
                SetIORegionsCombo(CB_L2_IORegions, p.hv_Line2_MeasureIORegions);

                bl_UpdatingFromROI = false;
            }
            catch { bl_UpdatingFromROI = false; }
        }

        // ==================== ComboBox Helpers ====================

        private void SetTransitionCombo(ComboBox cb, string value)
        {
            switch (value)
            {
                case "negative": cb.SelectedIndex = 0; break;
                case "positive": cb.SelectedIndex = 1; break;
                case "all": cb.SelectedIndex = 2; break;
                default: cb.SelectedIndex = 2; break;
            }
        }

        private void SetSelectCombo(ComboBox cb, string value)
        {
            switch (value)
            {
                case "first": cb.SelectedIndex = 0; break;
                case "last": cb.SelectedIndex = 1; break;
                case "all": cb.SelectedIndex = 2; break;
                default: cb.SelectedIndex = 0; break;
            }
        }

        private void SetInterpolationCombo(ComboBox cb, string value)
        {
            switch (value)
            {
                case "nearest_neighbor": cb.SelectedIndex = 0; break;
                case "bilinear": cb.SelectedIndex = 1; break;
                case "bicubic": cb.SelectedIndex = 2; break;
                default: cb.SelectedIndex = 0; break;
            }
        }

        private void SetIORegionsCombo(ComboBox cb, string value)
        {
            switch (value)
            {
                case "true": cb.SelectedIndex = 0; break;
                case "false": cb.SelectedIndex = 1; break;
                default: cb.SelectedIndex = 0; break;
            }
        }

        private string GetTransitionValue(ComboBox cb)
        {
            switch (cb.SelectedIndex)
            {
                case 0: return "negative";
                case 1: return "positive";
                case 2: return "all";
                default: return "all";
            }
        }

        private string GetSelectValue(ComboBox cb)
        {
            switch (cb.SelectedIndex)
            {
                case 0: return "first";
                case 1: return "last";
                case 2: return "all";
                default: return "first";
            }
        }

        private string GetInterpolationValue(ComboBox cb)
        {
            switch (cb.SelectedIndex)
            {
                case 0: return "nearest_neighbor";
                case 1: return "bilinear";
                case 2: return "bicubic";
                default: return "nearest_neighbor";
            }
        }

        private string GetIORegionsValue(ComboBox cb)
        {
            switch (cb.SelectedIndex)
            {
                case 0: return "true";
                case 1: return "false";
                default: return "true";
            }
        }

        // ==================== Save UI -> Params ====================

        private void SaveUIToParams()
        {
            // Line 1 ROI
            GetRoiParams_Line1(roiRectangle2_Line1, PluginParams);
            // Line 2 ROI
            GetRoiParams_Line2(roiRectangle2_Line2, PluginParams);

            // Line 1 Measure
            PluginParams.hv_Line1_MeasureSigma = (double)NUD_L1_Sigma.Value;
            PluginParams.hv_Line1_MeasureThreshold = (int)NUD_L1_Threshold.Value;
            PluginParams.hv_Line1_MeasureTransition = GetTransitionValue(CB_L1_Transition);
            PluginParams.hv_Line1_MeasureSelect = GetSelectValue(CB_L1_Select);
            PluginParams.hv_Line1_MeasureMinScore = (double)NUD_L1_MinScore.Value;
            PluginParams.hv_Line1_MeasureNumInstances = (int)NUD_L1_NumInstances.Value;
            PluginParams.hv_Line1_MeasureDistThreshold = (double)NUD_L1_DistThreshold.Value;
            PluginParams.hv_Line1_MaxNumIterations = (int)NUD_L1_MaxIterations.Value;
            PluginParams.hv_Line1_MeasureInterpolation = GetInterpolationValue(CB_L1_Interpolation);
            PluginParams.hv_Line1_MeasureIORegions = GetIORegionsValue(CB_L1_IORegions);

            // Line 2 Measure
            PluginParams.hv_Line2_MeasureSigma = (double)NUD_L2_Sigma.Value;
            PluginParams.hv_Line2_MeasureThreshold = (int)NUD_L2_Threshold.Value;
            PluginParams.hv_Line2_MeasureTransition = GetTransitionValue(CB_L2_Transition);
            PluginParams.hv_Line2_MeasureSelect = GetSelectValue(CB_L2_Select);
            PluginParams.hv_Line2_MeasureMinScore = (double)NUD_L2_MinScore.Value;
            PluginParams.hv_Line2_MeasureNumInstances = (int)NUD_L2_NumInstances.Value;
            PluginParams.hv_Line2_MeasureDistThreshold = (double)NUD_L2_DistThreshold.Value;
            PluginParams.hv_Line2_MaxNumIterations = (int)NUD_L2_MaxIterations.Value;
            PluginParams.hv_Line2_MeasureInterpolation = GetInterpolationValue(CB_L2_Interpolation);
            PluginParams.hv_Line2_MeasureIORegions = GetIORegionsValue(CB_L2_IORegions);
        }

        // ==================== ROI <-> Params Binding ====================

        /// <summary>
        /// Params -> ROI (Line 1)
        /// </summary>
        private void SetRoiParams_Line1(ParamsFindIntersection p, ROIRectangle2 roi, ROIController ctrl)
        {
            HTuple hv_Phi;
            roi.PointNum = p.hv_Line1_RoiPointsNum;
            roi.ROIName = "Line1";
            roi.midR = p.hv_Line1_RoiMidRow;
            roi.midC = p.hv_Line1_RoiMidColumn;
            HOperatorSet.TupleRad(p.hv_Line1_RoiAngle, out hv_Phi);
            roi.phi = hv_Phi.D;
            roi.length2 = p.hv_Line1_RoiWidth / 2;
            roi.length1 = p.hv_Line1_RoiHeight / 2;

            if (p.ho_Image != null && p.ho_Image.CountObj() != 0)
            {
                HTuple hv_Width, hv_Height;
                HOperatorSet.GetImageSize(p.ho_Image, out hv_Width, out hv_Height);
                if (hv_Width < 1000)
                    ctrl.size = roi.size = 1;
                else
                    ctrl.size = roi.size = hv_Width / 1000 + 1;

                if (!bl_RoiExist_Line1)
                {
                    bl_RoiExist_Line1 = true;
                    ctrl.setROIShape(roi);
                    ctrl.roiMode.createROI(roi.midC, roi.midR);
                    ctrl.ROIList.Add(ctrl.roiMode);
                }
                ctrl.activeROIidx = -1;
                roi.updateHandlePos();
                bl_ShowObj = true;
            }
        }

        /// <summary>
        /// Params -> ROI (Line 2)
        /// </summary>
        private void SetRoiParams_Line2(ParamsFindIntersection p, ROIRectangle2 roi, ROIController ctrl)
        {
            HTuple hv_Phi;
            roi.PointNum = p.hv_Line2_RoiPointsNum;
            roi.ROIName = "Line2";
            roi.midR = p.hv_Line2_RoiMidRow;
            roi.midC = p.hv_Line2_RoiMidColumn;
            HOperatorSet.TupleRad(p.hv_Line2_RoiAngle, out hv_Phi);
            roi.phi = hv_Phi.D;
            roi.length2 = p.hv_Line2_RoiWidth / 2;
            roi.length1 = p.hv_Line2_RoiHeight / 2;

            if (p.ho_Image != null && p.ho_Image.CountObj() != 0)
            {
                HTuple hv_Width, hv_Height;
                HOperatorSet.GetImageSize(p.ho_Image, out hv_Width, out hv_Height);
                if (hv_Width < 1000)
                    ctrl.size = roi.size = 1;
                else
                    ctrl.size = roi.size = hv_Width / 1000 + 1;

                if (!bl_RoiExist_Line2)
                {
                    bl_RoiExist_Line2 = true;
                    ctrl.setROIShape(roi);
                    ctrl.roiMode.createROI(roi.midC, roi.midR);
                    ctrl.ROIList.Add(ctrl.roiMode);
                }
                ctrl.activeROIidx = -1;
                roi.updateHandlePos();
                bl_ShowObj = true;
            }
        }

        /// <summary>
        /// ROI -> Params (Line 1) - after drag
        /// </summary>
        public void GetRoiParams_Line1(ROIRectangle2 roi, ParamsFindIntersection p)
        {
            HTuple hv_Angle;
            p.hv_Line1_RoiPointsNum = roi.PointNum;
            p.hv_Line1_RoiMidRow = roi.midR;
            p.hv_Line1_RoiMidColumn = roi.midC;
            HOperatorSet.TupleDeg(roi.phi, out hv_Angle);
            p.hv_Line1_RoiAngle = hv_Angle;
            p.hv_Line1_RoiWidth = roi.length2 * 2;
            p.hv_Line1_RoiHeight = roi.length1 * 2;
        }

        /// <summary>
        /// ROI -> Params (Line 2) - after drag
        /// </summary>
        public void GetRoiParams_Line2(ROIRectangle2 roi, ParamsFindIntersection p)
        {
            HTuple hv_Angle;
            p.hv_Line2_RoiPointsNum = roi.PointNum;
            p.hv_Line2_RoiMidRow = roi.midR;
            p.hv_Line2_RoiMidColumn = roi.midC;
            HOperatorSet.TupleDeg(roi.phi, out hv_Angle);
            p.hv_Line2_RoiAngle = hv_Angle;
            p.hv_Line2_RoiWidth = roi.length2 * 2;
            p.hv_Line2_RoiHeight = roi.length1 * 2;
        }

        /// <summary>
        /// ROI -> NUD controls (real-time during drag)
        /// </summary>
        private void RefreshRoiParams()
        {
            bl_UpdatingFromROI = true;
            try
            {
                // Line 1
                if (roiRectangle2_Line1 != null)
                {
                    HTuple hv_Deg1;
                    HOperatorSet.TupleDeg(roiRectangle2_Line1.phi, out hv_Deg1);
                    NUD_RoiWidth_L1.Value = (decimal)Convert.ToInt32(roiRectangle2_Line1.length2 * 2);
                    NUD_RoiHeight_L1.Value = (decimal)Convert.ToInt32(roiRectangle2_Line1.length1 * 2);
                    NUD_RoiAngle_L1.Value = (decimal)Convert.ToInt32(hv_Deg1.D);
                    NUD_RoiMidRow_L1.Value = (decimal)Convert.ToInt32(roiRectangle2_Line1.midR);
                    NUD_RoiMidColumn_L1.Value = (decimal)Convert.ToInt32(roiRectangle2_Line1.midC);
                }

                // Line 2
                if (roiRectangle2_Line2 != null)
                {
                    HTuple hv_Deg2;
                    HOperatorSet.TupleDeg(roiRectangle2_Line2.phi, out hv_Deg2);
                    NUD_RoiWidth_L2.Value = (decimal)Convert.ToInt32(roiRectangle2_Line2.length2 * 2);
                    NUD_RoiHeight_L2.Value = (decimal)Convert.ToInt32(roiRectangle2_Line2.length1 * 2);
                    NUD_RoiAngle_L2.Value = (decimal)Convert.ToInt32(hv_Deg2.D);
                    NUD_RoiMidRow_L2.Value = (decimal)Convert.ToInt32(roiRectangle2_Line2.midR);
                    NUD_RoiMidColumn_L2.Value = (decimal)Convert.ToInt32(roiRectangle2_Line2.midC);
                }
            }
            catch { }
            bl_UpdatingFromROI = false;
        }

        // ==================== ViewROI Callbacks ====================

        /// <summary>
        /// ImageProcessingEvent callback - sets image for display
        /// </summary>
        private void RunFunction()
        {
            try
            {
                bl_RefreshObj = false;
                if (roiController.activeROIidx == -1)
                {
                    if (PluginParams.ho_Image != null && PluginParams.ho_Image.CountObj() != 0)
                    {
                        hWndCtrl_Display.ho_Image_Changed = PluginParams.ho_Image;
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// ReDrawEvent callback - shows measurement results
        /// </summary>
        private void ShowResult()
        {
            // Always refresh ROI params (real-time during drag)
            RefreshRoiParams();

            // Auto re-execute after ROI drag release (mouseUp sets activeROIidx to -1)
            int currentIdx = roiController.activeROIidx;
            if (lastActiveROIidx != -1 && currentIdx == -1)
            {
                bl_ShowObj = true;
            }
            lastActiveROIidx = currentIdx;

            ShowLineToHWD(PluginParams, HWC_Display.HalconWindow,
                roiRectangle2_Line1.size);
            bl_RefreshObj = true;
        }

        /// <summary>
        /// Draw measurement results onto the Halcon window (with anti-flicker)
        /// </summary>
        private void ShowLineToHWD(ParamsFindIntersection p, HTuple hv_HWindowID, HTuple hv_size)
        {
            SendMessage(tableLayoutPanel3.Handle, WM_SETREDRAW, 0, IntPtr.Zero);
            try
            {
                if (roiController.activeROIidx == -1)
                {
                    HObject ho_Cross1, ho_Cross2, ho_RegionLine1, ho_RegionLine2, ho_CrossIntersection;
                    HOperatorSet.GenEmptyObj(out ho_Cross1);
                    HOperatorSet.GenEmptyObj(out ho_Cross2);
                    HOperatorSet.GenEmptyObj(out ho_RegionLine1);
                    HOperatorSet.GenEmptyObj(out ho_RegionLine2);
                    HOperatorSet.GenEmptyObj(out ho_CrossIntersection);

                    // ---- Line 1 edge points (green cross) ----
                    if (p.hv_Line1_CrossRow != null && hWndCtrl_Display.bl_RoiOperateFlag && !bl_ShowObj)
                    {
                        HOperatorSet.GenCrossContourXld(out ho_Cross1,
                            p.hv_Line1_CrossRow, p.hv_Line1_CrossColumn,
                            hv_size * 3, (new HTuple(45)).TupleRad());
                        HOperatorSet.SetDraw(hv_HWindowID, "fill");
                        HOperatorSet.SetColor(hv_HWindowID, "green");
                        HOperatorSet.SetLineWidth(hv_HWindowID, 2);
                        HOperatorSet.DispObj(ho_Cross1, hv_HWindowID);
                    }

                    // ---- Line 1 fitted line (yellow) ----
                    if (p.hv_Line1_RowBegin != null && hWndCtrl_Display.bl_RoiOperateFlag && !bl_ShowObj)
                    {
                        HOperatorSet.GenRegionLine(out ho_RegionLine1,
                            p.hv_Line1_RowBegin, p.hv_Line1_ColumnBegin,
                            p.hv_Line1_RowEnd, p.hv_Line1_ColumnEnd);
                        HOperatorSet.SetLineWidth(hv_HWindowID, 3);
                        HOperatorSet.SetColor(hv_HWindowID, "yellow");
                        HOperatorSet.DispObj(ho_RegionLine1, hv_HWindowID);
                    }

                    // ---- Line 2 edge points (cyan cross) ----
                    if (p.hv_Line2_CrossRow != null && hWndCtrl_Display.bl_RoiOperateFlag && !bl_ShowObj)
                    {
                        HOperatorSet.GenCrossContourXld(out ho_Cross2,
                            p.hv_Line2_CrossRow, p.hv_Line2_CrossColumn,
                            hv_size * 3, (new HTuple(45)).TupleRad());
                        HOperatorSet.SetDraw(hv_HWindowID, "fill");
                        HOperatorSet.SetColor(hv_HWindowID, "cyan");
                        HOperatorSet.SetLineWidth(hv_HWindowID, 2);
                        HOperatorSet.DispObj(ho_Cross2, hv_HWindowID);
                    }

                    // ---- Line 2 fitted line (magenta) ----
                    if (p.hv_Line2_RowBegin != null && hWndCtrl_Display.bl_RoiOperateFlag && !bl_ShowObj)
                    {
                        HOperatorSet.GenRegionLine(out ho_RegionLine2,
                            p.hv_Line2_RowBegin, p.hv_Line2_ColumnBegin,
                            p.hv_Line2_RowEnd, p.hv_Line2_ColumnEnd);
                        HOperatorSet.SetLineWidth(hv_HWindowID, 3);
                        HOperatorSet.SetColor(hv_HWindowID, "magenta");
                        HOperatorSet.DispObj(ho_RegionLine2, hv_HWindowID);
                    }

                    // ---- Intersection point (red large cross) ----
                    if (p.hv_IntersectionRow != null && p.hv_IsParallel != null
                        && p.hv_IsParallel.I == 0
                        && hWndCtrl_Display.bl_RoiOperateFlag && !bl_ShowObj)
                    {
                        HOperatorSet.GenCrossContourXld(out ho_CrossIntersection,
                            p.hv_IntersectionRow, p.hv_IntersectionColumn,
                            hv_size * 8, (new HTuple(45)).TupleRad());
                        HOperatorSet.SetColor(hv_HWindowID, "red");
                        HOperatorSet.SetLineWidth(hv_HWindowID, 3);
                        HOperatorSet.DispObj(ho_CrossIntersection, hv_HWindowID);

                        // Display intersection coordinate text
                        HOperatorSet.SetColor(hv_HWindowID, "red");
                        HOperatorSet.SetTposition(hv_HWindowID,
                            p.hv_IntersectionRow.D + hv_size * 5,
                            p.hv_IntersectionColumn.D + hv_size * 5);
                        HOperatorSet.WriteString(hv_HWindowID,
                            String.Format("({0:F1}, {1:F1})",
                            p.hv_IntersectionRow.D, p.hv_IntersectionColumn.D));
                    }

                    ho_Cross1.Dispose();
                    ho_Cross2.Dispose();
                    ho_RegionLine1.Dispose();
                    ho_RegionLine2.Dispose();
                    ho_CrossIntersection.Dispose();
                }

                // Draw crosshair lines at mouse position
                if (crosshairEnabled && crosshairDragActive && crossRow >= 0 && crossCol >= 0 && p.ho_Image != null && p.ho_Image.CountObj() != 0)
                
                    {
                    HTuple cw, ch;
                    HOperatorSet.GetImageSize(p.ho_Image, out cw, out ch);
                    HOperatorSet.SetColor(hv_HWindowID, "yellow");
                    HOperatorSet.SetLineWidth(hv_HWindowID, 1);
                    HOperatorSet.DispLine(hv_HWindowID, crossRow, 0, crossRow, cw - 1);
                    HOperatorSet.DispLine(hv_HWindowID, 0, crossCol, ch - 1, crossCol);
                }
            }
            catch { }
            SendMessage(tableLayoutPanel3.Handle, WM_SETREDRAW, 1, IntPtr.Zero);
            tableLayoutPanel3.Refresh();
        }

        // ==================== Timer ====================

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (bl_RefreshObj & bl_ShowObj)
            {
                bl_ShowObj = false;
                if (roiRectangle2_Line1 != null && roiRectangle2_Line2 != null
                    && PluginParams.ho_Image != null && PluginParams.ho_Image.CountObj() != 0)
                {
                    // Sync ROI state to params before executing
                    GetRoiParams_Line1(roiRectangle2_Line1, PluginParams);
                    GetRoiParams_Line2(roiRectangle2_Line2, PluginParams);
                    SaveUIToParams();
                    pluginsObj.ExecuteObj();
                }

                Invoke(new MethodInvoker(delegate
                {
                    hWndCtrl_Display.repaint();
                    UpdateResultDisplay();
                }));
            }
        }

        // ==================== NUD Value Changed (Line 1 ROI) ====================

        private void NUD_Roi_L1_ValueChanged(object sender, EventArgs e)
        {
            if (bl_UpdatingFromROI || PluginParams == null) return;

            GetRoiParams_Line1(roiRectangle2_Line1, PluginParams);
            PluginParams.hv_Line1_RoiPointsNum = (int)NUD_RoiNum_L1.Value;
            PluginParams.hv_Line1_RoiWidth = (double)NUD_RoiWidth_L1.Value;
            PluginParams.hv_Line1_RoiHeight = (double)NUD_RoiHeight_L1.Value;
            PluginParams.hv_Line1_RoiAngle = (double)NUD_RoiAngle_L1.Value;
            PluginParams.hv_Line1_RoiMidRow = (double)NUD_RoiMidRow_L1.Value;
            PluginParams.hv_Line1_RoiMidColumn = (double)NUD_RoiMidColumn_L1.Value;
            SetRoiParams_Line1(PluginParams, roiRectangle2_Line1, roiController);
        }

        // ==================== NUD Value Changed (Line 2 ROI) ====================

        private void NUD_Roi_L2_ValueChanged(object sender, EventArgs e)
        {
            if (bl_UpdatingFromROI || PluginParams == null) return;

            GetRoiParams_Line2(roiRectangle2_Line2, PluginParams);
            PluginParams.hv_Line2_RoiPointsNum = (int)NUD_RoiNum_L2.Value;
            PluginParams.hv_Line2_RoiWidth = (double)NUD_RoiWidth_L2.Value;
            PluginParams.hv_Line2_RoiHeight = (double)NUD_RoiHeight_L2.Value;
            PluginParams.hv_Line2_RoiAngle = (double)NUD_RoiAngle_L2.Value;
            PluginParams.hv_Line2_RoiMidRow = (double)NUD_RoiMidRow_L2.Value;
            PluginParams.hv_Line2_RoiMidColumn = (double)NUD_RoiMidColumn_L2.Value;
            SetRoiParams_Line2(PluginParams, roiRectangle2_Line2, roiController);
        }

        // ==================== Measure Params Changed ====================

        private void NUD_Measure_ValueChanged(object sender, EventArgs e)
        {
            if (bl_UpdatingFromROI || PluginParams == null) return;
            bl_ShowObj = true;
        }

        private void CB_Measure_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (bl_UpdatingFromROI || PluginParams == null) return;
            bl_ShowObj = true;
        }

        // ==================== Display Helpers ====================

        //private void HWC_Display_HMouseMove(object sender, HMouseEventArgs e)
        //{
        //    // Position and gray value display

        //    if (hWndCtrl_Display.ho_Image_Changed != null && roiController.activeROIidx == -1)
        //    {
        //        try
        //        {
        //            TB_PositionAndGray.Text = hWndCtrl_Display.Get_Mouseposition_and_Gray(
        //                HWC_Display.HalconWindow, hWndCtrl_Display.ho_Image_Changed, e);
        //            LB_IMageSize.Text = ShowIMageSize(hWndCtrl_Display.ho_Image_Changed);
        //        }
        //        catch { }
        //    }

        //    // Crosshair & gray profile (independent of ROI drag state)
        //    if (crosshairEnabled && hWndCtrl_Display.ho_Image_Changed != null)
        //    {
        //        try
        //        {
        //            HTuple hvImgRow, hvImgCol;
        //            HOperatorSet.ConvertCoordinatesWindowToImage(
        //                HWC_Display.HalconWindow, (HTuple)e.Y, (HTuple)e.X,
        //                out hvImgRow, out hvImgCol);
        //            crossRow = hvImgRow.D;
        //            crossCol = hvImgCol.D;

        //            if ((System.DateTime.Now - lastProfileUpdate).TotalMilliseconds > 50)
        //            {
        //                lastProfileUpdate = System.DateTime.Now;
        //                ExtractGrayProfiles(crossRow, crossCol);
        //                panelHProfile.Invalidate();
        //                panelVProfile.Invalidate();
        //                hWndCtrl_Display.repaint();
        //            }
        //        }
        //        catch { }
        //    }
        //}

        private void HWC_Display_HMouseMove(object sender, HMouseEventArgs e)
        {
            // 显示位置和灰度（与原逻辑一致）
            if (hWndCtrl_Display.ho_Image_Changed != null && roiController.activeROIidx == -1)
            {
                try
                {
                    LB_IMageSize.Text = hWndCtrl_Display.Get_Mouseposition_and_Gray(
                    HWC_Display.HalconWindow, hWndCtrl_Display.ho_Image_Changed, e);
                    TB_PositionAndGray.Text = ShowIMageSize(hWndCtrl_Display.ho_Image_Changed);
            

                }
                catch { }
            }

            // 右键拖动状态下更新十字线
            if (crosshairEnabled && crosshairDragActive && hWndCtrl_Display.ho_Image_Changed != null)
            {
                try
                {
                    HTuple hvImgRow, hvImgCol;
                    HOperatorSet.ConvertCoordinatesWindowToImage(
                        HWC_Display.HalconWindow, e.Y, e.X, out hvImgRow, out hvImgCol);
                    crossRow = hvImgRow.D;
                    crossCol = hvImgCol.D;

                    if ((System.DateTime.Now - lastProfileUpdate).TotalMilliseconds > 50)
                    {
                        lastProfileUpdate = System.DateTime.Now;
                        ExtractGrayProfiles(crossRow, crossCol);
                        panelHProfile.Invalidate();
                        panelVProfile.Invalidate();
                        hWndCtrl_Display.repaint();
                    }
                }
                catch { }
            }
        }

        private string ShowIMageSize(HObject image)
        {
            try
            {
                HTuple w, h;
                HOperatorSet.GetImageSize(image, out w, out h);
                return String.Format("Size  R:{0}   C:{1}", h, w);
            }
            catch { return "Size  R:--   C:--"; }
        }

        private void FullScrean_Click(object sender, EventArgs e)
        {
            hWndCtrl_Display.resetAll();
            hWndCtrl_Display.repaint();
        }

        private void HWC_Display_HMouseDoubleClick(object sender, HMouseEventArgs e)
        {
            hWndCtrl_Display.resetAll();
            hWndCtrl_Display.repaint();
        }

        // ==================== Crosshair Toggle ====================

        private void btnCrosshair_Click(object sender, EventArgs e)
        {
            crosshairEnabled = !crosshairEnabled;
            btnCrosshair.BackColor = crosshairEnabled ? _clrAccent : _clrPanel;
            if (!crosshairEnabled)
            {
                crossRow = -1;
                crossCol = -1;
                hProfileData = null;
                vProfileData = null;
                panelHProfile.Invalidate();
                panelVProfile.Invalidate();
                hWndCtrl_Display.repaint();
            }
        }

        private void btnCrosshair_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            var btn = (System.Windows.Forms.Button)sender;
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            float cx = btn.Width / 2f, cy = btn.Height / 2f;
            Color clr = crosshairEnabled ? Color.White : _clrTextDim;

            // Draw eye shape using Bezier curves (almond shape)
            PointF pL = new PointF(cx - 10, cy);
            PointF pR = new PointF(cx + 10, cy);

            using (var path = new System.Drawing.Drawing2D.GraphicsPath())
            {
                path.AddBezier(pL, new PointF(cx - 5, cy - 7), new PointF(cx + 5, cy - 7), pR);
                path.AddBezier(pR, new PointF(cx + 5, cy + 7), new PointF(cx - 5, cy + 7), pL);
                path.CloseFigure();
                using (var pen = new Pen(clr, 1.4f))
                    g.DrawPath(pen, path);
            }

            // Iris circle
            using (var pen = new Pen(clr, 1.2f))
                g.DrawEllipse(pen, cx - 4, cy - 4, 8, 8);

            // Pupil dot
            using (var brush = new SolidBrush(clr))
                g.FillEllipse(brush, cx - 2f, cy - 2f, 4, 4);

            // Strike-through line when crosshair is disabled
            if (!crosshairEnabled)
            {
                using (var pen = new Pen(clr, 1.5f))
                    g.DrawLine(pen, cx - 10, cy + 7, cx + 10, cy - 7);
            }
        }

        private void UpdateResultDisplay()
        {
            try
            {
                ParamsFindIntersection p = PluginParams;

                // Line 1 fit status
                LB_Line1Success.Text = p.bl_Line1_FitSuccess ? "线1拟合: 成功" : "线1拟合: 失败";
                LB_Line1Success.ForeColor = p.bl_Line1_FitSuccess ? Color.Green : Color.Red;

                // Line 2 fit status
                LB_Line2Success.Text = p.bl_Line2_FitSuccess ? "线2拟合: 成功" : "线2拟合: 失败";
                LB_Line2Success.ForeColor = p.bl_Line2_FitSuccess ? Color.Green : Color.Red;

                // Line 1 detail
                if (p.bl_Line1_FitSuccess && p.hv_Line1_RowBegin != null)
                    LB_Line1Info.Text = String.Format("线1: ({0:F1},{1:F1})->({2:F1},{3:F1})",
                        p.hv_Line1_RowBegin.D, p.hv_Line1_ColumnBegin.D,
                        p.hv_Line1_RowEnd.D, p.hv_Line1_ColumnEnd.D);
                else
                    LB_Line1Info.Text = "线1: 未检测到";

                // Line 2 detail
                if (p.bl_Line2_FitSuccess && p.hv_Line2_RowBegin != null)
                    LB_Line2Info.Text = String.Format("线2: ({0:F1},{1:F1})->({2:F1},{3:F1})",
                        p.hv_Line2_RowBegin.D, p.hv_Line2_ColumnBegin.D,
                        p.hv_Line2_RowEnd.D, p.hv_Line2_ColumnEnd.D);
                else
                    LB_Line2Info.Text = "线2: 未检测到";

                // Intersection
                if (p.bl_IntersectionSuccess && p.hv_IntersectionRow != null && p.hv_IsParallel != null)
                {
                    if (p.hv_IsParallel.I == 0)
                    {
                        LB_ResultRow.Text = String.Format("交点行: {0:F2}", p.hv_IntersectionRow.D);
                        LB_ResultCol.Text = String.Format("交点列: {0:F2}", p.hv_IntersectionColumn.D);
                        LB_ResultAngle.Text = String.Format("角度: {0:F2}", p.hv_IntersectionAngle.D);
                        LB_IsParallel.Text = "是否平行: 否 (有交点)";
                    }
                    else
                    {
                        LB_ResultRow.Text = "交点行: --";
                        LB_ResultCol.Text = "交点列: --";
                        LB_IsParallel.Text = "是否平行: 是 (无交点)";
                    }
                }
                else
                {
                    LB_ResultRow.Text = "交点行: --";
                    LB_ResultCol.Text = "交点列: --";
                    LB_IsParallel.Text = "是否平行: --";
                }
            }
            catch
            {
                LB_ResultRow.Text = "交点行: --";
                LB_ResultCol.Text = "交点列: --";
                LB_IsParallel.Text = "是否平行: --";
            }
        }

        // ==================== Gray Profile Extraction ====================

        private void ExtractGrayProfiles(double row, double col)
        {
            try
            {
                HObject img = PluginParams.ho_Image;
                if (img == null || img.CountObj() == 0) return;

                HTuple w, h;
                HOperatorSet.GetImageSize(img, out w, out h);
                imgWidth = w.I;
                imgHeight = h.I;

                int iRow = (int)Math.Round(row);
                int iCol = (int)Math.Round(col);
                if (iRow < 0 || iRow >= imgHeight || iCol < 0 || iCol >= imgWidth) return;

                // Horizontal profile (all columns at row iRow)
                double[] rArr = new double[imgWidth];
                double[] cArr = new double[imgWidth];
                for (int i = 0; i < imgWidth; i++) { rArr[i] = iRow; cArr[i] = i; }
                HTuple gH;
                HOperatorSet.GetGrayval(img, new HTuple(rArr), new HTuple(cArr), out gH);
                hProfileData = gH.ToDArr();

                // Vertical profile (all rows at column iCol)
                double[] rArr2 = new double[imgHeight];
                double[] cArr2 = new double[imgHeight];
                for (int i = 0; i < imgHeight; i++) { rArr2[i] = i; cArr2[i] = iCol; }
                HTuple gV;
                HOperatorSet.GetGrayval(img, new HTuple(rArr2), new HTuple(cArr2), out gV);
                vProfileData = gV.ToDArr();
            }
            catch { hProfileData = null; vProfileData = null; }
        }

        // ==================== Chart Painting ====================

        private void PanelHProfile_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            DrawGrayProfile(e.Graphics, (System.Windows.Forms.Panel)sender,
                hProfileData, crossCol, "H \u7070\u5ea6\u66f2\u7ebf (Row=" + ((int)crossRow).ToString() + ")");
        }

        private void PanelVProfile_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            DrawGrayProfile(e.Graphics, (System.Windows.Forms.Panel)sender,
                vProfileData, crossRow, "V \u7070\u5ea6\u66f2\u7ebf (Col=" + ((int)crossCol).ToString() + ")");
        }

        private void DrawGrayProfile(System.Drawing.Graphics g, System.Windows.Forms.Panel panel,
            double[] data, double markerPos, string title)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            int padL = 32, padR = 6, padT = 20, padB = 4;
            int pw = panel.Width, ph = panel.Height;
            System.Drawing.Rectangle chart = new System.Drawing.Rectangle(
                padL, padT, pw - padL - padR, ph - padT - padB);

            // Background
            using (System.Drawing.SolidBrush bg = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(28, 28, 31)))
                g.FillRectangle(bg, panel.ClientRectangle);

            if (chart.Width < 4 || chart.Height < 4) return;

            // Chart area
            using (System.Drawing.SolidBrush cbg = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(20, 20, 23)))
                g.FillRectangle(cbg, chart);
            using (System.Drawing.Pen bp = new System.Drawing.Pen(System.Drawing.Color.FromArgb(55, 55, 60)))
                g.DrawRectangle(bp, chart);

            // Title
            using (System.Drawing.SolidBrush tb = new System.Drawing.SolidBrush(_clrAccent))
            using (System.Drawing.Font tf = new System.Drawing.Font("Microsoft YaHei UI", 8F, System.Drawing.FontStyle.Bold))
                g.DrawString(title, tf, tb, 3, 2);

            // Y-axis grid & labels
            using (System.Drawing.Pen gp = new System.Drawing.Pen(System.Drawing.Color.FromArgb(40, 40, 44)))
            using (System.Drawing.SolidBrush lb = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(110, 110, 114)))
            using (System.Drawing.Font lf = new System.Drawing.Font("Consolas", 7F))
            {
                for (int v = 0; v <= 255; v += 64)
                {
                    float y = chart.Bottom - (float)v / 255f * chart.Height;
                    g.DrawLine(gp, chart.Left, y, chart.Right, y);
                    g.DrawString(v.ToString(), lf, lb, 1, y - 6);
                }
            }

            if (data == null || data.Length < 2) return;

            // Profile curve
            int step = Math.Max(1, data.Length / Math.Max(1, chart.Width));
            int count = Math.Max(2, data.Length / step);
            System.Drawing.PointF[] pts = new System.Drawing.PointF[count];
            for (int i = 0; i < count; i++)
            {
                int idx = Math.Min(i * step, data.Length - 1);
                float x = chart.Left + (float)i / (count - 1) * chart.Width;
                float y = chart.Bottom - (float)(data[idx] / 255.0) * chart.Height;
                y = Math.Max(chart.Top, Math.Min(chart.Bottom, y));
                pts[i] = new System.Drawing.PointF(x, y);
            }
            using (System.Drawing.Pen cp = new System.Drawing.Pen(System.Drawing.Color.FromArgb(0, 200, 120), 1.2f))
                g.DrawLines(cp, pts);

            // Position marker
            if (markerPos >= 0 && markerPos < data.Length)
            {
                float mx = chart.Left + (float)markerPos / (data.Length - 1) * chart.Width;
                using (System.Drawing.Pen mp = new System.Drawing.Pen(System.Drawing.Color.FromArgb(220, 80, 80), 1f))
                    g.DrawLine(mp, mx, chart.Top, mx, chart.Bottom);

                int mIdx = (int)Math.Round(markerPos);
                if (mIdx >= 0 && mIdx < data.Length)
                {
                    string txt = String.Format("{0:F0}", data[mIdx]);
                    using (System.Drawing.SolidBrush vtb = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(255, 200, 200)))
                    using (System.Drawing.Font vf = new System.Drawing.Font("Consolas", 7.5F))
                    {
                        float tx = mx + 3;
                        if (tx + 25 > chart.Right) tx = mx - 28;
                        g.DrawString(txt, vf, vtb, tx, chart.Top + 2);
                    }
                }
            }
        }

        // ==================== Tab Owner-Draw ====================

        private void tabControl1_DrawItem(object sender, System.Windows.Forms.DrawItemEventArgs e)
        {
            System.Windows.Forms.TabControl tc = (System.Windows.Forms.TabControl)sender;
            System.Windows.Forms.TabPage tp = tc.TabPages[e.Index];
            System.Drawing.Rectangle r = tc.GetTabRect(e.Index);
            bool sel = (tc.SelectedIndex == e.Index);
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            if (sel)
            {
                // Selected tab: gradient fill with accent indicator
                using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                    r, System.Drawing.Color.FromArgb(0, 100, 180), _clrAccent,
                    System.Drawing.Drawing2D.LinearGradientMode.Horizontal))
                    g.FillRectangle(brush, r);

                // Right-edge accent bar
                using (var pen = new Pen(System.Drawing.Color.FromArgb(60, 200, 255), 3f))
                    g.DrawLine(pen, r.Right - 1, r.Top + 2, r.Right - 1, r.Bottom - 2);
            }
            else
            {
                // Unselected tab: subtle dark background
                using (var bg = new SolidBrush(System.Drawing.Color.FromArgb(35, 35, 39)))
                    g.FillRectangle(bg, r);

                // Bottom separator line
                using (var pen = new Pen(System.Drawing.Color.FromArgb(50, 50, 55)))
                    g.DrawLine(pen, r.Left + 4, r.Bottom - 1, r.Right - 4, r.Bottom - 1);
            }

            // Draw text
            System.Drawing.Color textColor = sel ? System.Drawing.Color.White : _clrTextDim;
            using (System.Drawing.SolidBrush fg = new System.Drawing.SolidBrush(textColor))
            {
                System.Drawing.StringFormat sf = new System.Drawing.StringFormat();
                sf.Alignment = System.Drawing.StringAlignment.Center;
                sf.LineAlignment = System.Drawing.StringAlignment.Center;
                var font = sel ? new Font(tp.Font, FontStyle.Bold) : tp.Font;
                g.DrawString(tp.Text.Trim(), font, fg, r, sf);
                if (sel) font.Dispose();
            }
        }
    }
}
  
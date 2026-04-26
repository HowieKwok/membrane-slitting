using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Frame_Core;
using HalconDotNet;
using ViewROI;

namespace Plugin.FindIntersectionAngle
{
    public partial class FormFindIntersectionAngle : FormPluginBase
    {
        private FindIntersectionAngleObj pluginObj;
        private ParamsFindIntersectionAngle p;
        private readonly List<int> hProfile = new List<int>();
        private readonly List<int> vProfile = new List<int>();
        private readonly ROIController roiController = new ROIController();
        private readonly Timer refreshTimer = new Timer();
        private HWndCtrl hWndCtrl;
        private bool crossVisible = true;
        private bool refreshPending;
        private bool refreshing;
        private int cursorRow = -1;
        private int cursorCol = -1;
        private int cursorGray = -1;

        public FormFindIntersectionAngle()
            : this(new FindIntersectionAngleObj())
        {
        }

        public FormFindIntersectionAngle(FindIntersectionAngleObj obj)
        {
            InitializeComponent();
            BuildLinePage(this.line1Panel, true);
            BuildLinePage(this.line2Panel, false);
            pluginObj = obj ?? new FindIntersectionAngleObj();
            pluginObj.IniObj();
            p = pluginObj.PluginParams as ParamsFindIntersectionAngle ?? new ParamsFindIntersectionAngle();
            pluginObj.PluginParams = p;
            pluginsObjBase = pluginObj;
            pluginsObjParams_Back = pluginObj.PluginParams;

            InitViewRoi();
            EnsurePluginContext();
            BindParamsToUi();
            WireRefreshEvents();
            DisplayAll(true);

            refreshTimer.Interval = 200;
            refreshTimer.Tick += RefreshTimer_Tick;
            refreshTimer.Start();
        }

        private ParamsFindIntersectionAngle EnsurePluginContext()
        {
            FindIntersectionAngleObj frameworkObj = pluginsObjBase as FindIntersectionAngleObj;
            if (frameworkObj != null && !object.ReferenceEquals(frameworkObj, pluginObj))
            {
                pluginObj = frameworkObj;
            }

            if (pluginObj == null)
            {
                pluginObj = new FindIntersectionAngleObj();
                pluginsObjBase = pluginObj;
            }

            if (pluginObj.PluginParams == null)
            {
                pluginObj.PluginParams = new ParamsFindIntersectionAngle();
            }

            p = pluginObj.PluginParams as ParamsFindIntersectionAngle;
            if (p == null)
            {
                p = new ParamsFindIntersectionAngle();
                pluginObj.PluginParams = p;
            }

            pluginsObjBase = pluginObj;
            pluginsObjParams_Back = pluginObj.PluginParams;
            return p;
        }

        public override void Init()
        {
            EnsurePluginContext();
            if (pluginsObjBase != null && pluginsObjBase.moduleInfo != null)
            {
                Text = pluginsObjBase.moduleInfo.ModuleName;
            }
            BindParamsToUi();
            DisplayAll(true);
        }

        private void InitViewRoi()
        {
            hWndCtrl = new HWndCtrl(hWindow);
            hWndCtrl.useROIController(roiController);
            hWndCtrl.setViewState(HWndCtrl.MODE_VIEW_ZOOM);
            hWndCtrl.setViewState(HWndCtrl.MODE_VIEW_MOVE);
            hWndCtrl.ImageProcessingEvent += RunFunction;
            hWndCtrl.ReDrawEvent += ShowResult;
            roiController.NotifyRCObserver = OnRoiChanged;
        }

        private void WireRefreshEvents()
        {
            foreach (TextBox box in new[]
            {
                txtL1Row, txtL1Col, txtL1Angle, txtL1Height, txtL1Width, txtL1Points, txtL1Sigma, txtL1Threshold,
                txtL1NumInstances, txtL1DistThreshold, txtL1MinScore, txtL1Iterations,
                txtL2Row, txtL2Col, txtL2Angle, txtL2Height, txtL2Width, txtL2Points, txtL2Sigma, txtL2Threshold,
                txtL2NumInstances, txtL2DistThreshold, txtL2MinScore, txtL2Iterations
            })
            {
                box.TextChanged += MarkRefresh;
            }

            foreach (ComboBox combo in new[] { cmbL1Transition, cmbL1Select, cmbL1Interpolation, cmbL2Transition, cmbL2Select, cmbL2Interpolation })
            {
                combo.SelectedIndexChanged += MarkRefresh;
            }
        }

        private void BindParamsToUi()
        {
            EnsurePluginContext();
            SetText(txtL1Row, p.hv_Line1_RoiMidRow); SetText(txtL1Col, p.hv_Line1_RoiMidColumn); SetText(txtL1Angle, p.hv_Line1_RoiAngle);
            SetText(txtL1Height, p.hv_Line1_RoiHeight); SetText(txtL1Width, p.hv_Line1_RoiWidth); SetText(txtL1Points, p.hv_Line1_RoiPointsNum);
            SetText(txtL1Sigma, p.hv_Line1_MeasureSigma); SetText(txtL1Threshold, p.hv_Line1_MeasureThreshold); SetText(txtL1NumInstances, p.hv_Line1_MeasureNumInstances);
            SetText(txtL1DistThreshold, p.hv_Line1_MeasureDistThreshold); SetText(txtL1MinScore, p.hv_Line1_MeasureMinScore); SetText(txtL1Iterations, p.hv_Line1_MaxNumIterations);
            SelectCombo(cmbL1Transition, p.hv_Line1_MeasureTransition); SelectCombo(cmbL1Select, p.hv_Line1_MeasureSelect); SelectCombo(cmbL1Interpolation, p.hv_Line1_MeasureInterpolation);

            SetText(txtL2Row, p.hv_Line2_RoiMidRow); SetText(txtL2Col, p.hv_Line2_RoiMidColumn); SetText(txtL2Angle, p.hv_Line2_RoiAngle);
            SetText(txtL2Height, p.hv_Line2_RoiHeight); SetText(txtL2Width, p.hv_Line2_RoiWidth); SetText(txtL2Points, p.hv_Line2_RoiPointsNum);
            SetText(txtL2Sigma, p.hv_Line2_MeasureSigma); SetText(txtL2Threshold, p.hv_Line2_MeasureThreshold); SetText(txtL2NumInstances, p.hv_Line2_MeasureNumInstances);
            SetText(txtL2DistThreshold, p.hv_Line2_MeasureDistThreshold); SetText(txtL2MinScore, p.hv_Line2_MeasureMinScore); SetText(txtL2Iterations, p.hv_Line2_MaxNumIterations);
            SelectCombo(cmbL2Transition, p.hv_Line2_MeasureTransition); SelectCombo(cmbL2Select, p.hv_Line2_MeasureSelect); SelectCombo(cmbL2Interpolation, p.hv_Line2_MeasureInterpolation);
        }

        private void SaveUiToParams()
        {
            EnsurePluginContext();
            p.hv_Line1_RoiMidRow = D(txtL1Row); p.hv_Line1_RoiMidColumn = D(txtL1Col); p.hv_Line1_RoiAngle = D(txtL1Angle);
            p.hv_Line1_RoiHeight = D(txtL1Height); p.hv_Line1_RoiWidth = D(txtL1Width); p.hv_Line1_RoiPointsNum = I(txtL1Points);
            p.hv_Line1_MeasureSigma = D(txtL1Sigma); p.hv_Line1_MeasureThreshold = I(txtL1Threshold); p.hv_Line1_MeasureNumInstances = I(txtL1NumInstances);
            p.hv_Line1_MeasureDistThreshold = D(txtL1DistThreshold); p.hv_Line1_MeasureMinScore = D(txtL1MinScore); p.hv_Line1_MaxNumIterations = I(txtL1Iterations);
            p.hv_Line1_MeasureTransition = C(cmbL1Transition, "all"); p.hv_Line1_MeasureSelect = C(cmbL1Select, "first"); p.hv_Line1_MeasureInterpolation = C(cmbL1Interpolation, "nearest_neighbor");

            p.hv_Line2_RoiMidRow = D(txtL2Row); p.hv_Line2_RoiMidColumn = D(txtL2Col); p.hv_Line2_RoiAngle = D(txtL2Angle);
            p.hv_Line2_RoiHeight = D(txtL2Height); p.hv_Line2_RoiWidth = D(txtL2Width); p.hv_Line2_RoiPointsNum = I(txtL2Points);
            p.hv_Line2_MeasureSigma = D(txtL2Sigma); p.hv_Line2_MeasureThreshold = I(txtL2Threshold); p.hv_Line2_MeasureNumInstances = I(txtL2NumInstances);
            p.hv_Line2_MeasureDistThreshold = D(txtL2DistThreshold); p.hv_Line2_MeasureMinScore = D(txtL2MinScore); p.hv_Line2_MaxNumIterations = I(txtL2Iterations);
            p.hv_Line2_MeasureTransition = C(cmbL2Transition, "all"); p.hv_Line2_MeasureSelect = C(cmbL2Select, "first"); p.hv_Line2_MeasureInterpolation = C(cmbL2Interpolation, "nearest_neighbor");
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            EnsurePluginContext();
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "图像文件|*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff|所有文件|*.*";
                if (dialog.ShowDialog() != DialogResult.OK) return;
                p.ho_Image = new HImage(dialog.FileName);
                DisplayAll(true);
            }
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            DisplayAll(true);
        }

        private void BtnCross_Click(object sender, EventArgs e)
        {
            crossVisible = !crossVisible;
            btnCross.BackColor = crossVisible ? Color.FromArgb(0, 120, 165) : Color.FromArgb(42, 42, 45);
            lblStatus.Text = crossVisible ? "十字光标：显示" : "十字光标：隐藏";
            RepaintView();
        }

        private void BtnLine1Roi_Click(object sender, EventArgs e)
        {
            ActivateRoi(0);
        }

        private void BtnLine2Roi_Click(object sender, EventArgs e)
        {
            ActivateRoi(1);
        }

        private void ActivateRoi(int index)
        {
            SaveUiToParams();
            SyncParamsToRois();
            roiController.setActiveROIIdx(index);
            refreshPending = true;
            RepaintView();
        }

        private void BtnMeasure_Click(object sender, EventArgs e)
        {
            RunFunction();
            RepaintView();
            tabParams.SelectedTab = tabResult;
        }

        private void HWindow_HMouseMove(object sender, HMouseEventArgs e)
        {
            UpdateCursor((int)Math.Round(e.Y), (int)Math.Round(e.X), false);
        }

        private void HWindow_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int row;
            int col;
            GetImagePoint(e.X, e.Y, out row, out col);
            UpdateCursor(row, col, true);
        }

        private void UpdateCursor(int row, int col, bool lockCross)
        {
            if (p.ho_Image == null || !p.ho_Image.IsInitialized()) return;
            try
            {
                HTuple width, height;
                HOperatorSet.GetImageSize(p.ho_Image, out width, out height);
                if (row < 0 || col < 0 || row >= height.I || col >= width.I) return;
                HTuple gray;
                HOperatorSet.GetGrayval(p.ho_Image, row, col, out gray);
                lblMouse.Text = string.Format("图像：{0} x {1}    鼠标：R{2} C{3} 灰度{4}", width.I, height.I, row, col, gray.I);

                if (!lockCross) return;
                cursorRow = row;
                cursorCol = col;
                cursorGray = gray.I;
                BuildProfiles(row, col);
                RepaintView();
            }
            catch
            {
            }
        }

        private void GetImagePoint(int x, int y, out int row, out int col)
        {
            Rectangle part = hWindow.ImagePart;
            if (part.Width <= 0 || part.Height <= 0 || hWindow.Width <= 0 || hWindow.Height <= 0)
            {
                row = y;
                col = x;
                return;
            }
            col = part.Left + (int)Math.Round(x * part.Width / (double)hWindow.Width);
            row = part.Top + (int)Math.Round(y * part.Height / (double)hWindow.Height);
        }

        private void BuildProfiles(int row, int col)
        {
            hProfile.Clear();
            vProfile.Clear();
            try
            {
                HTuple width, height;
                HOperatorSet.GetImageSize(p.ho_Image, out width, out height);
                int stepH = Math.Max(1, width.I / 500);
                int stepV = Math.Max(1, height.I / 500);
                for (int c = 0; c < width.I; c += stepH)
                {
                    HTuple g;
                    HOperatorSet.GetGrayval(p.ho_Image, row, c, out g);
                    hProfile.Add(ClampGray(g.I));
                }
                for (int r = 0; r < height.I; r += stepV)
                {
                    HTuple g;
                    HOperatorSet.GetGrayval(p.ho_Image, r, col, out g);
                    vProfile.Add(ClampGray(g.I));
                }
                lblHProfile.Text = string.Format("H 灰度曲线 (Row: {0})", row);
                lblVProfile.Text = string.Format("V 灰度曲线 (Col: {0})", col);
                panelHProfile.Invalidate();
                panelVProfile.Invalidate();
            }
            catch
            {
            }
        }

        private void DisplayAll(bool resetView)
        {
            EnsurePluginContext();
            try
            {
                if (resetView && hWndCtrl != null) hWndCtrl.resetAll();
                if (p.ho_Image != null && p.ho_Image.IsInitialized() && hWndCtrl != null)
                {
                    hWndCtrl.addIconicVar(p.ho_Image);
                    SyncParamsToRois();
                    RepaintView();
                }
                RefreshResult();
            }
            catch
            {
            }
        }

        private void SyncParamsToRois()
        {
            int active = roiController.getActiveROIIdx();
            roiController.ROIList.Clear();
            AddRoiFromParams(p.hv_Line1_RoiMidRow, p.hv_Line1_RoiMidColumn, p.hv_Line1_RoiAngle, p.hv_Line1_RoiHeight, p.hv_Line1_RoiWidth, p.hv_Line1_RoiPointsNum);
            AddRoiFromParams(p.hv_Line2_RoiMidRow, p.hv_Line2_RoiMidColumn, p.hv_Line2_RoiAngle, p.hv_Line2_RoiHeight, p.hv_Line2_RoiWidth, p.hv_Line2_RoiPointsNum);
            roiController.setActiveROIIdx(active >= 0 && active < roiController.ROIList.Count ? active : 0);
        }

        private void AddRoiFromParams(double row, double col, double angleDeg, double height, double width, int points)
        {
            ROIRectangle2 roi = new ROIRectangle2();
            roi.midR = row;
            roi.midC = col;
            roiController.setROIShape(roi);
            roiController.roiMode.createROI(roi.midC, roi.midR);
            roiController.ROIList.Add(roiController.roiMode);

            ROIRectangle2 roiMode = roiController.roiMode as ROIRectangle2;
            if (roiMode == null) return;
            roiMode.phi = angleDeg * Math.PI / 180.0;
            roiMode.length1 = height / 2.0;
            roiMode.length2 = width / 2.0;
            roiMode.PointNum = points;
            roiMode.updateHandlePos();
        }

        private void SyncRoisToParams()
        {
            if (roiController.ROIList.Count > 0) ApplyRoiToLine1((ROIRectangle2)roiController.ROIList[0]);
            if (roiController.ROIList.Count > 1) ApplyRoiToLine2((ROIRectangle2)roiController.ROIList[1]);
            BindParamsToUi();
        }

        private void ApplyRoiToLine1(ROIRectangle2 roi)
        {
            p.hv_Line1_RoiMidRow = roi.midR;
            p.hv_Line1_RoiMidColumn = roi.midC;
            p.hv_Line1_RoiAngle = roi.phi * 180.0 / Math.PI;
            p.hv_Line1_RoiHeight = roi.length1 * 2.0;
            p.hv_Line1_RoiWidth = roi.length2 * 2.0;
            p.hv_Line1_RoiPointsNum = roi.PointNum;
        }

        private void ApplyRoiToLine2(ROIRectangle2 roi)
        {
            p.hv_Line2_RoiMidRow = roi.midR;
            p.hv_Line2_RoiMidColumn = roi.midC;
            p.hv_Line2_RoiAngle = roi.phi * 180.0 / Math.PI;
            p.hv_Line2_RoiHeight = roi.length1 * 2.0;
            p.hv_Line2_RoiWidth = roi.length2 * 2.0;
            p.hv_Line2_RoiPointsNum = roi.PointNum;
        }

        private void RunFunction()
        {
            if (refreshing) return;
            refreshing = true;
            try
            {
                EnsurePluginContext();
                SaveUiToParams();
                SyncRoisToParams();
                pluginObj.ExecuteObj();
                RefreshResult();
            }
            catch
            {
            }
            finally
            {
                refreshing = false;
            }
        }

        private void ShowResult()
        {
            EnsurePluginContext();
            DrawMeasurePoints(p.hv_Line1_CrossRow, p.hv_Line1_CrossColumn, "blue");
            DrawMeasurePoints(p.hv_Line2_CrossRow, p.hv_Line2_CrossColumn, "green");
            DrawFitResults();
            DrawCrosshair();
        }

        private void OnRoiChanged(int eventId)
        {
            refreshPending = true;
        }

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            if (!refreshPending || p.ho_Image == null || !p.ho_Image.IsInitialized()) return;
            refreshPending = false;
            RunFunction();
            RepaintView();
        }

        private void RepaintView()
        {
            try
            {
                if (hWndCtrl != null) hWndCtrl.repaint();
            }
            catch
            {
            }
        }

        private void DrawMeasurePoints(HTuple rows, HTuple cols, string color)
        {
            if (!HasTuple(rows) || !HasTuple(cols)) return;
            hWindow.HalconWindow.SetColor(color);
            for (int i = 0; i < rows.Length; i++)
            {
                hWindow.HalconWindow.DispCross(rows[i].D, cols[i].D, 10.0, 0.0);
            }
        }

        private void DrawFitResults()
        {
            if (HasTuple(p.hv_Line1_RowBegin))
            {
                hWindow.HalconWindow.SetColor("cyan");
                hWindow.HalconWindow.DispLine(p.hv_Line1_RowBegin, p.hv_Line1_ColumnBegin, p.hv_Line1_RowEnd, p.hv_Line1_ColumnEnd);
            }
            if (HasTuple(p.hv_Line2_RowBegin))
            {
                hWindow.HalconWindow.SetColor("green");
                hWindow.HalconWindow.DispLine(p.hv_Line2_RowBegin, p.hv_Line2_ColumnBegin, p.hv_Line2_RowEnd, p.hv_Line2_ColumnEnd);
            }
            if (HasTuple(p.hv_IntersectionRow))
            {
                hWindow.HalconWindow.SetColor("red");
                hWindow.HalconWindow.DispCross(p.hv_IntersectionRow.D, p.hv_IntersectionColumn.D, 24.0, 0.0);
            }
        }

        private void DrawCrosshair()
        {
            if (!crossVisible || cursorRow < 0 || cursorCol < 0) return;
            hWindow.HalconWindow.SetColor("cyan");
            hWindow.HalconWindow.DispCross((double)cursorRow, (double)cursorCol, 40.0, 0.0);
            hWindow.HalconWindow.SetTposition(cursorRow + 10, cursorCol + 10);
            hWindow.HalconWindow.WriteString(string.Format("R:{0} C:{1} Gray:{2}", cursorRow, cursorCol, cursorGray));
        }

        private void RefreshResult()
        {
            lblStatus.Text = p.bl_IntersectionSuccess ? "状态：检测完成" : "状态：等待执行";
            lblTopAngle.Text = HasTuple(p.hv_LineAngle) ? string.Format("当前夹角：{0:F3} 度", p.hv_LineAngle.D) : "当前夹角：-- 度";
            lblResultAngle.Text = HasTuple(p.hv_LineAngle) ? string.Format("两线夹角：{0:F3} 度", p.hv_LineAngle.D) : "两线夹角：--";
            lblResultCross.Text = HasTuple(p.hv_IntersectionRow) ? string.Format("交点：R={0:F2}  C={1:F2}", p.hv_IntersectionRow.D, p.hv_IntersectionColumn.D) : "交点：--";
            lblResultLine1.Text = p.bl_Line1_FitSuccess ? "线1拟合：成功" : "线1拟合：未完成";
            lblResultLine2.Text = p.bl_Line2_FitSuccess ? "线2拟合：成功" : "线2拟合：未完成";
            lblResultParallel.Text = HasTuple(p.hv_IsParallel) ? (p.hv_IsParallel.I == 0 ? "是否平行：否" : "是否平行：是") : "是否平行：--";
        }

        private void ProfilePaint(object sender, PaintEventArgs e)
        {
            Panel panel = sender as Panel;
            List<int> values = panel == panelHProfile ? hProfile : vProfile;
            e.Graphics.Clear(Color.FromArgb(18, 20, 24));
            using (Pen grid = new Pen(Color.FromArgb(45, 50, 58)))
            {
                for (int i = 1; i < 4; i++) e.Graphics.DrawLine(grid, 0, panel.Height * i / 4, panel.Width, panel.Height * i / 4);
            }
            if (values.Count < 2) return;
            using (Pen pen = new Pen(Color.DeepSkyBlue, 2))
            {
                PointF[] points = new PointF[values.Count];
                for (int i = 0; i < values.Count; i++)
                {
                    float x = i * (panel.Width - 1f) / (values.Count - 1);
                    float y = panel.Height - 1f - values[i] * (panel.Height - 2f) / 255f;
                    points[i] = new PointF(x, y);
                }
                e.Graphics.DrawLines(pen, points);
            }
        }

        private void MarkRefresh(object sender, EventArgs e) { refreshPending = true; }
        private int ClampGray(int value) { return Math.Max(0, Math.Min(255, value)); }
        private bool HasTuple(HTuple tuple) { return tuple != null && tuple.Length > 0; }
        private void SetText(TextBox box, object value) { box.Text = Convert.ToString(value, CultureInfo.InvariantCulture); }
        private void SelectCombo(ComboBox combo, string value) { combo.SelectedItem = value; if (combo.SelectedIndex < 0 && combo.Items.Count > 0) combo.SelectedIndex = 0; }
        private double D(TextBox box) { double value; return double.TryParse(box.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value) ? value : 0.0; }
        private int I(TextBox box) { int value; return int.TryParse(box.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value) ? value : 0; }
        private string C(ComboBox combo, string fallback) { return combo.SelectedItem == null ? fallback : combo.SelectedItem.ToString(); }
    }
}

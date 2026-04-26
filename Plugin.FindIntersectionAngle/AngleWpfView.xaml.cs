using Frame_Core;
using HalconDotNet;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Plugin.FindIntersectionAngle
{
    public partial class AngleWpfView : UserControl
    {
        private readonly FindIntersectionAngleObj pluginObj;
        private readonly ParamsFindIntersectionAngle p;
        private readonly HWindowControl halconWindow;
        private readonly List<int> hProfile = new List<int>();
        private readonly List<int> vProfile = new List<int>();
        private readonly Stopwatch profileWatch = Stopwatch.StartNew();
        private bool crosshairEnabled = true;
        private bool loadingUi;
        private int activeRoi;

        public AngleWpfView(FindIntersectionAngleObj obj)
        {
            InitializeComponent();
            pluginObj = obj ?? new FindIntersectionAngleObj();
            pluginObj.IniObj();
            p = pluginObj.PluginParams as ParamsFindIntersectionAngle ?? new ParamsFindIntersectionAngle();

            halconWindow = new HWindowControl();
            halconWindow.BackColor = System.Drawing.Color.Black;
            halconWindow.Dock = System.Windows.Forms.DockStyle.Fill;
            halconWindow.HMouseMove += HalconWindow_HMouseMove;
            halconWindow.HMouseDown += HalconWindow_HMouseDown;
            halconWindow.HMouseUp += HalconWindow_HMouseUp;
            HalconHost.Child = halconWindow;

            InitComboItems();
            LoadParamsToUi();
            RefreshResultDisplay();
            DisplayImageAndResults();
        }

        private void InitComboItems()
        {
            FillCombo(L1Transition, new[] { "all", "negative", "positive", "uniform" });
            FillCombo(L2Transition, new[] { "all", "negative", "positive", "uniform" });
            FillCombo(L1Select, new[] { "first", "last", "all" });
            FillCombo(L2Select, new[] { "first", "last", "all" });
            FillCombo(L1Interpolation, new[] { "nearest_neighbor", "bilinear", "bicubic" });
            FillCombo(L2Interpolation, new[] { "nearest_neighbor", "bilinear", "bicubic" });
            FillCombo(L1IORegions, new[] { "true", "false" });
            FillCombo(L2IORegions, new[] { "true", "false" });
        }

        private void FillCombo(ComboBox combo, string[] values)
        {
            combo.Items.Clear();
            foreach (string value in values) combo.Items.Add(value);
        }

        private void LoadParamsToUi()
        {
            loadingUi = true;
            SetText(L1Row, p.hv_Line1_RoiMidRow); SetText(L1Col, p.hv_Line1_RoiMidColumn); SetText(L1Angle, p.hv_Line1_RoiAngle);
            SetText(L1Height, p.hv_Line1_RoiHeight); SetText(L1Width, p.hv_Line1_RoiWidth); SetText(L1Points, p.hv_Line1_RoiPointsNum);
            SetText(L1Sigma, p.hv_Line1_MeasureSigma); SetText(L1Threshold, p.hv_Line1_MeasureThreshold); SetText(L1NumInstances, p.hv_Line1_MeasureNumInstances);
            SetText(L1DistThreshold, p.hv_Line1_MeasureDistThreshold); SetText(L1MinScore, p.hv_Line1_MeasureMinScore); SetText(L1Iterations, p.hv_Line1_MaxNumIterations);
            SelectCombo(L1Transition, p.hv_Line1_MeasureTransition); SelectCombo(L1Select, p.hv_Line1_MeasureSelect); SelectCombo(L1Interpolation, p.hv_Line1_MeasureInterpolation); SelectCombo(L1IORegions, p.hv_Line1_MeasureIORegions);

            SetText(L2Row, p.hv_Line2_RoiMidRow); SetText(L2Col, p.hv_Line2_RoiMidColumn); SetText(L2Angle, p.hv_Line2_RoiAngle);
            SetText(L2Height, p.hv_Line2_RoiHeight); SetText(L2Width, p.hv_Line2_RoiWidth); SetText(L2Points, p.hv_Line2_RoiPointsNum);
            SetText(L2Sigma, p.hv_Line2_MeasureSigma); SetText(L2Threshold, p.hv_Line2_MeasureThreshold); SetText(L2NumInstances, p.hv_Line2_MeasureNumInstances);
            SetText(L2DistThreshold, p.hv_Line2_MeasureDistThreshold); SetText(L2MinScore, p.hv_Line2_MeasureMinScore); SetText(L2Iterations, p.hv_Line2_MaxNumIterations);
            SelectCombo(L2Transition, p.hv_Line2_MeasureTransition); SelectCombo(L2Select, p.hv_Line2_MeasureSelect); SelectCombo(L2Interpolation, p.hv_Line2_MeasureInterpolation); SelectCombo(L2IORegions, p.hv_Line2_MeasureIORegions);
            loadingUi = false;
        }

        private void SaveUiToParams()
        {
            p.hv_Line1_RoiMidRow = D(L1Row); p.hv_Line1_RoiMidColumn = D(L1Col); p.hv_Line1_RoiAngle = D(L1Angle);
            p.hv_Line1_RoiHeight = D(L1Height); p.hv_Line1_RoiWidth = D(L1Width); p.hv_Line1_RoiPointsNum = I(L1Points);
            p.hv_Line1_MeasureSigma = D(L1Sigma); p.hv_Line1_MeasureThreshold = I(L1Threshold); p.hv_Line1_MeasureNumInstances = I(L1NumInstances);
            p.hv_Line1_MeasureDistThreshold = D(L1DistThreshold); p.hv_Line1_MeasureMinScore = D(L1MinScore); p.hv_Line1_MaxNumIterations = I(L1Iterations);
            p.hv_Line1_MeasureTransition = C(L1Transition, "all"); p.hv_Line1_MeasureSelect = C(L1Select, "first"); p.hv_Line1_MeasureInterpolation = C(L1Interpolation, "nearest_neighbor"); p.hv_Line1_MeasureIORegions = C(L1IORegions, "true");

            p.hv_Line2_RoiMidRow = D(L2Row); p.hv_Line2_RoiMidColumn = D(L2Col); p.hv_Line2_RoiAngle = D(L2Angle);
            p.hv_Line2_RoiHeight = D(L2Height); p.hv_Line2_RoiWidth = D(L2Width); p.hv_Line2_RoiPointsNum = I(L2Points);
            p.hv_Line2_MeasureSigma = D(L2Sigma); p.hv_Line2_MeasureThreshold = I(L2Threshold); p.hv_Line2_MeasureNumInstances = I(L2NumInstances);
            p.hv_Line2_MeasureDistThreshold = D(L2DistThreshold); p.hv_Line2_MeasureMinScore = D(L2MinScore); p.hv_Line2_MaxNumIterations = I(L2Iterations);
            p.hv_Line2_MeasureTransition = C(L2Transition, "all"); p.hv_Line2_MeasureSelect = C(L2Select, "first"); p.hv_Line2_MeasureInterpolation = C(L2Interpolation, "nearest_neighbor"); p.hv_Line2_MeasureIORegions = C(L2IORegions, "true");
        }

        private void BtnLoadImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "打开图像";
            dialog.Filter = "图像文件|*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff|所有文件|*.*";
            if (dialog.ShowDialog() != true) return;

            HImage image = new HImage();
            image.ReadImage(dialog.FileName);
            p.ho_Image = image;
            DisplayImageAndResults();
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            hProfile.Clear();
            vProfile.Clear();
            DrawProfile(CanvasHProfile, hProfile, "横向灰度曲线");
            DrawProfile(CanvasVProfile, vProfile, "纵向灰度曲线");
            DisplayImageAndResults();
        }

        private void BtnCrosshair_Click(object sender, RoutedEventArgs e)
        {
            crosshairEnabled = !crosshairEnabled;
            BtnCrosshair.Background = new SolidColorBrush(crosshairEnabled ? Color.FromRgb(0, 120, 150) : Color.FromRgb(25, 40, 58));
        }

        private void BtnRun_Click(object sender, RoutedEventArgs e)
        {
            SaveUiToParams();
            pluginObj.ExecuteObj();
            RefreshResultDisplay();
            DisplayImageAndResults();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveUiToParams();
            pluginObj.saveParams();
        }

        private void Param_LostFocus(object sender, RoutedEventArgs e)
        {
            if (loadingUi) return;
            SaveUiToParams();
            DisplayImageAndResults();
        }

        private void HalconWindow_HMouseDown(object sender, HMouseEventArgs e)
        {
            activeRoi = PickRoi(e.Y, e.X);
        }

        private void HalconWindow_HMouseUp(object sender, HMouseEventArgs e)
        {
            activeRoi = 0;
        }

        private void HalconWindow_HMouseMove(object sender, HMouseEventArgs e)
        {
            if (activeRoi > 0)
            {
                MoveActiveRoi(e.Y, e.X);
                return;
            }

            if (!crosshairEnabled || p.ho_Image == null || !p.ho_Image.IsInitialized() || profileWatch.ElapsedMilliseconds < 45) return;
            profileWatch.Restart();

            try
            {
                int row = (int)Math.Round(e.Y);
                int col = (int)Math.Round(e.X);
                HTuple width, height;
                HOperatorSet.GetImageSize(p.ho_Image, out width, out height);
                if (row < 0 || col < 0 || row >= height.I || col >= width.I) return;

                HTuple gray;
                HOperatorSet.GetGrayval(p.ho_Image, row, col, out gray);
                TxtGray.Text = string.Format("行：{0}  列：{1}  灰度：{2}", row, col, gray.I);
                AddProfileValue(hProfile, gray.I);
                AddProfileValue(vProfile, gray.I);
                DrawProfile(CanvasHProfile, hProfile, "横向灰度曲线");
                DrawProfile(CanvasVProfile, vProfile, "纵向灰度曲线");
            }
            catch
            {
            }
        }

        private int PickRoi(double row, double col)
        {
            double d1 = Distance(row, col, p.hv_Line1_RoiMidRow, p.hv_Line1_RoiMidColumn);
            double d2 = Distance(row, col, p.hv_Line2_RoiMidRow, p.hv_Line2_RoiMidColumn);
            double hit1 = Math.Max(30, Math.Max(p.hv_Line1_RoiHeight, p.hv_Line1_RoiWidth) * 0.65);
            double hit2 = Math.Max(30, Math.Max(p.hv_Line2_RoiHeight, p.hv_Line2_RoiWidth) * 0.65);
            if (d1 <= hit1 && d1 <= d2) return 1;
            if (d2 <= hit2) return 2;
            return 0;
        }

        private void MoveActiveRoi(double row, double col)
        {
            if (activeRoi == 1)
            {
                p.hv_Line1_RoiMidRow = row;
                p.hv_Line1_RoiMidColumn = col;
                loadingUi = true;
                SetText(L1Row, row);
                SetText(L1Col, col);
                loadingUi = false;
            }
            else if (activeRoi == 2)
            {
                p.hv_Line2_RoiMidRow = row;
                p.hv_Line2_RoiMidColumn = col;
                loadingUi = true;
                SetText(L2Row, row);
                SetText(L2Col, col);
                loadingUi = false;
            }

            DisplayImageAndResults();
        }

        private double Distance(double row1, double col1, double row2, double col2)
        {
            double dr = row1 - row2;
            double dc = col1 - col2;
            return Math.Sqrt(dr * dr + dc * dc);
        }

        private void DisplayImageAndResults()
        {
            try
            {
                halconWindow.HalconWindow.ClearWindow();
                if (p.ho_Image == null || !p.ho_Image.IsInitialized()) return;

                HTuple width, height;
                HOperatorSet.GetImageSize(p.ho_Image, out width, out height);
                halconWindow.HalconWindow.SetPart(0, 0, height.I - 1, width.I - 1);
                halconWindow.HalconWindow.DispObj(p.ho_Image);
                TxtImageSize.Text = string.Format("图像：{0} x {1}", width.I, height.I);

                DrawMeasureRoi(p.hv_Line1_RoiMidRow, p.hv_Line1_RoiMidColumn, p.hv_Line1_RoiAngle, p.hv_Line1_RoiHeight, p.hv_Line1_RoiWidth, "cyan");
                DrawMeasureRoi(p.hv_Line2_RoiMidRow, p.hv_Line2_RoiMidColumn, p.hv_Line2_RoiAngle, p.hv_Line2_RoiHeight, p.hv_Line2_RoiWidth, "green");
                DrawResultsOverlay();
            }
            catch
            {
            }
        }

        private void DrawMeasureRoi(double row, double col, double angleDeg, double height, double width, string color)
        {
            HTuple phi;
            HOperatorSet.TupleRad(angleDeg, out phi);
            HObject rect;
            HOperatorSet.GenRectangle2(out rect, row, col, phi, height / 2.0, width / 2.0);
            halconWindow.HalconWindow.SetDraw("margin");
            halconWindow.HalconWindow.SetLineWidth(2);
            halconWindow.HalconWindow.SetColor(color);
            halconWindow.HalconWindow.DispObj(rect);
            halconWindow.HalconWindow.DispCross(row, col, 18.0, 0.0);
            rect.Dispose();
        }

        private void DrawResultsOverlay()
        {
            if (HasTuple(p.hv_Line1_CrossRow) && HasTuple(p.hv_Line1_CrossColumn))
            {
                halconWindow.HalconWindow.SetColor("yellow");
                for (int i = 0; i < p.hv_Line1_CrossRow.Length; i++)
                    halconWindow.HalconWindow.DispCross(p.hv_Line1_CrossRow[i].D, p.hv_Line1_CrossColumn[i].D, 12.0, 0.0);
            }
            if (HasTuple(p.hv_Line2_CrossRow) && HasTuple(p.hv_Line2_CrossColumn))
            {
                halconWindow.HalconWindow.SetColor("orange");
                for (int i = 0; i < p.hv_Line2_CrossRow.Length; i++)
                    halconWindow.HalconWindow.DispCross(p.hv_Line2_CrossRow[i].D, p.hv_Line2_CrossColumn[i].D, 12.0, 0.0);
            }
            if (HasTuple(p.hv_Line1_RowBegin) && HasTuple(p.hv_Line1_RowEnd))
            {
                halconWindow.HalconWindow.SetColor("cyan");
                halconWindow.HalconWindow.DispLine(p.hv_Line1_RowBegin, p.hv_Line1_ColumnBegin, p.hv_Line1_RowEnd, p.hv_Line1_ColumnEnd);
            }
            if (HasTuple(p.hv_Line2_RowBegin) && HasTuple(p.hv_Line2_RowEnd))
            {
                halconWindow.HalconWindow.SetColor("green");
                halconWindow.HalconWindow.DispLine(p.hv_Line2_RowBegin, p.hv_Line2_ColumnBegin, p.hv_Line2_RowEnd, p.hv_Line2_ColumnEnd);
            }
            if (HasTuple(p.hv_IntersectionRow) && HasTuple(p.hv_IntersectionColumn))
            {
                halconWindow.HalconWindow.SetColor("red");
                halconWindow.HalconWindow.DispCross(p.hv_IntersectionRow.D, p.hv_IntersectionColumn.D, 36.0, 0.0);
            }
        }

        private void RefreshResultDisplay()
        {
            TxtLine1Success.Text = p.bl_Line1_FitSuccess ? "线1拟合：成功" : "线1拟合：未完成";
            TxtLine2Success.Text = p.bl_Line2_FitSuccess ? "线2拟合：成功" : "线2拟合：未完成";
            TxtResultRow.Text = HasTuple(p.hv_IntersectionRow) ? string.Format("交点行：{0:F2}", p.hv_IntersectionRow.D) : "交点行：--";
            TxtResultCol.Text = HasTuple(p.hv_IntersectionColumn) ? string.Format("交点列：{0:F2}", p.hv_IntersectionColumn.D) : "交点列：--";
            string angleText = HasTuple(p.hv_LineAngle) ? string.Format("夹角：{0:F3} 度", p.hv_LineAngle.D) : "夹角：-";
            TxtLineAngle.Text = angleText;
            TxtLineAngleStatus.Text = angleText;
            TxtParallel.Text = HasTuple(p.hv_IsParallel) ? (p.hv_IsParallel.I == 0 ? "是否平行：否" : "是否平行：是") : "是否平行：-";
            TxtRuntime.Text = p.bl_IntersectionSuccess ? "已输出结果" : "等待执行";
        }

        private void DrawProfile(Canvas canvas, List<int> values, string title)
        {
            canvas.Children.Clear();
            canvas.Children.Add(new TextBlock { Text = title, Foreground = new SolidColorBrush(Color.FromRgb(143, 185, 201)), Margin = new Thickness(8, 6, 0, 0) });
            if (values.Count < 2) return;

            double width = Math.Max(1, canvas.ActualWidth);
            double height = Math.Max(1, canvas.ActualHeight);
            Polyline polyline = new Polyline { Stroke = new SolidColorBrush(Color.FromRgb(0, 216, 255)), StrokeThickness = 2 };
            for (int i = 0; i < values.Count; i++)
            {
                double x = i * width / Math.Max(1, values.Count - 1);
                double y = height - values[i] * height / 255.0;
                polyline.Points.Add(new Point(x, y));
            }
            canvas.Children.Add(polyline);
        }

        private void AddProfileValue(List<int> values, int value)
        {
            values.Add(Math.Max(0, Math.Min(255, value)));
            while (values.Count > 120) values.RemoveAt(0);
        }

        private bool HasTuple(HTuple tuple)
        {
            return tuple != null && tuple.Length > 0;
        }

        private void SetText(TextBox box, object value)
        {
            box.Text = Convert.ToString(value, CultureInfo.InvariantCulture);
        }

        private void SelectCombo(ComboBox combo, string value)
        {
            combo.SelectedItem = value;
            if (combo.SelectedIndex < 0 && combo.Items.Count > 0) combo.SelectedIndex = 0;
        }

        private double D(TextBox box)
        {
            double value;
            return double.TryParse(box.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out value) ? value : 0.0;
        }

        private int I(TextBox box)
        {
            int value;
            return int.TryParse(box.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value) ? value : 0;
        }

        private string C(ComboBox combo, string fallback)
        {
            return combo.SelectedItem == null ? fallback : combo.SelectedItem.ToString();
        }
    }
}

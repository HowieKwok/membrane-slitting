using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using HalconDotNet;
using Microsoft.Win32;
using ViewROI;
using Forms = System.Windows.Forms;

namespace Plugin.FindIntersection
{
    public partial class FindIntersectionWpfView : UserControl
    {
        private readonly FindIntersectionObj pluginsObj = new FindIntersectionObj();
        private ParamsFindIntersection pluginParams;
        private HWndCtrl hWndCtrl;
        private readonly ROIController roiController = new ROIController();
        private readonly ROIRectangle2 roiRectangle2Line1 = new ROIRectangle2();
        private readonly ROIRectangle2 roiRectangle2Line2 = new ROIRectangle2();
        private bool syncing;
        private bool blRoiExistLine1;
        private bool blRoiExistLine2;
        private bool crosshairEnabled;
        private bool crosshairDragActive;
        private bool fitRefreshPending;
        private bool fitRefreshRunning;
        private double crossRow = -1;
        private double crossCol = -1;
        private double[] hProfileData;
        private double[] vProfileData;
        private readonly DispatcherTimer fitRefreshTimer = new DispatcherTimer();

        public FindIntersectionWpfView()
        {
            InitializeComponent();
            pluginParams = EnsurePluginParams();
            DataContextChanged += FindIntersectionWpfView_DataContextChanged;
            HProfileCanvas.SizeChanged += delegate { DrawProfile(HProfileCanvas, hProfileData, true); };
            VProfileCanvas.SizeChanged += delegate { DrawProfile(VProfileCanvas, vProfileData, false); };
            fitRefreshTimer.Interval = TimeSpan.FromMilliseconds(250);
            fitRefreshTimer.Tick += FitRefreshTimer_Tick;
            fitRefreshTimer.Start();
            Unloaded += FindIntersectionWpfView_Unloaded;
        }

        private FindIntersectionDemoViewModel ViewModel
        {
            get { return DataContext as FindIntersectionDemoViewModel; }
        }

        private ParamsFindIntersection EnsurePluginParams()
        {
            if (pluginsObj.PluginParams == null)
            {
                pluginsObj.PluginParams = new ParamsFindIntersection();
            }

            pluginParams = pluginsObj.PluginParams as ParamsFindIntersection;
            if (pluginParams == null)
            {
                pluginParams = new ParamsFindIntersection();
                pluginsObj.PluginParams = pluginParams;
            }

            return pluginParams;
        }

        private bool EnsureHalconView()
        {
            var vm = ViewModel;
            if (hWndCtrl != null) return true;

            try
            {
                if (vm != null) vm.StatusText = "正在初始化 Halcon / ViewROI";

                var halconWindow = new HWindowControl();
                halconWindow.Dock = Forms.DockStyle.Fill;
                HalconElementHost.Child = halconWindow;
                HalconElementHost.SizeChanged += HalconElementHost_SizeChanged;

                hWndCtrl = new HWndCtrl(halconWindow);
                hWndCtrl.useROIController(roiController);
                hWndCtrl.setViewState(HWndCtrl.MODE_VIEW_ZOOM);
                hWndCtrl.setViewState(HWndCtrl.MODE_VIEW_MOVE);
                hWndCtrl.ImageProcessingEvent += RunFunction;
                hWndCtrl.ReDrawEvent += ShowResult;

                halconWindow.HMouseMove += HwcDisplayHMouseMove;
                halconWindow.HMouseDown += HwcDisplayHMouseDown;
                halconWindow.HMouseUp += HwcDisplayHMouseUp;
                roiController.NotifyRCObserver += RoiControllerNotifyRCObserver;

                if (vm != null) vm.StatusText = "ViewROI 初始化完成";
                return true;
            }
            catch (Exception ex)
            {
                if (vm != null) vm.StatusText = "Halcon 初始化失败：" + ex.Message;
                MessageBox.Show("Halcon 初始化失败：" + ex.Message, "FindIntersection", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
        }

        private void HalconElementHost_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var child = HalconElementHost.Child as HWindowControl;
            if (child == null) return;
            child.Size = new System.Drawing.Size(Math.Max(1, (int)HalconElementHost.ActualWidth), Math.Max(1, (int)HalconElementHost.ActualHeight));
            try
            {
                if (hWndCtrl != null)
                {
                    hWndCtrl.resetAll();
                    hWndCtrl.repaint();
                }
            }
            catch
            {
            }
        }

        private void LoadImage_Click(object sender, RoutedEventArgs e)
        {
            EnsurePluginParams();
            var vm = ViewModel;
            try
            {
                var dialog = new OpenFileDialog();
                dialog.Filter = "图像文件|*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff|所有文件|*.*";
                if (dialog.ShowDialog() != true) return;
                if (!EnsureHalconView()) return;

                pluginParams.ho_Image = new HImage(dialog.FileName);
                hWndCtrl.addIconicVar(pluginParams.ho_Image);
                PullParamsToViewModel();
                SetRoiParamsLine1(pluginParams, roiRectangle2Line1, roiController);
                SetRoiParamsLine2(pluginParams, roiRectangle2Line2, roiController);
                hWndCtrl.resetAll();
                hWndCtrl.repaint();

                EmptyHint.Visibility = Visibility.Collapsed;
                RequestFitRefresh();
                if (vm != null)
                {
                    vm.ResetResult();
                    vm.StatusText = "图片已加载：" + System.IO.Path.GetFileName(dialog.FileName);
                }
            }
            catch (Exception ex)
            {
                if (vm != null) vm.StatusText = "加载图片失败：" + ex.Message;
                MessageBox.Show("加载图片失败：" + ex.Message, "FindIntersection", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CrossCursor_Click(object sender, RoutedEventArgs e)
        {
            crosshairEnabled = !crosshairEnabled;
            if (ViewModel != null) ViewModel.StatusText = crosshairEnabled ? "十字光标已开启，按住右键查看灰度曲线" : "十字光标已关闭";
            if (hWndCtrl != null) hWndCtrl.repaint();
        }

        private void FitWindow_Click(object sender, RoutedEventArgs e)
        {
            if (!EnsureHalconView()) return;
            hWndCtrl.resetAll();
            hWndCtrl.repaint();
            if (ViewModel != null) ViewModel.StatusText = "已适应窗口";
        }

        private void KeepRoi_Click(object sender, RoutedEventArgs e)
        {
            if (!EnsureHalconView()) return;
            SaveViewModelToParams();
            SetRoiParamsLine1(pluginParams, roiRectangle2Line1, roiController);
            SetRoiParamsLine2(pluginParams, roiRectangle2Line2, roiController);
            hWndCtrl.repaint();
            RequestFitRefresh();
            if (ViewModel != null) ViewModel.StatusText = "ROI 已保持";
        }

        private void RunPreview_Click(object sender, RoutedEventArgs e)
        {
            EnsurePluginParams();
            var vm = ViewModel;
            try
            {
                if (!EnsureHalconView()) return;
                SaveViewModelToParams();
                pluginsObj.ExecuteObj();
                hWndCtrl.repaint();
                UpdateResultDisplay();
                fitRefreshPending = false;
                if (vm != null) vm.StatusText = "运行完成，结果已刷新";
            }
            catch (Exception ex)
            {
                if (vm != null) vm.StatusText = "运行失败：" + ex.Message;
                MessageBox.Show("运行失败：" + ex.Message, "FindIntersection", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void FindIntersectionWpfView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var oldVm = e.OldValue as FindIntersectionDemoViewModel;
            if (oldVm != null) oldVm.RoiParameterChanged -= ViewModelRoiParameterChanged;

            var newVm = e.NewValue as FindIntersectionDemoViewModel;
            if (newVm != null)
            {
                PullParamsToViewModel();
                newVm.RoiParameterChanged += ViewModelRoiParameterChanged;
                newVm.StatusText = "界面已加载，点击加载图片";
            }
        }

        private void ViewModelRoiParameterChanged(object sender, EventArgs e)
        {
            if (syncing || hWndCtrl == null) return;
            SaveViewModelToParams();
            SetRoiParamsLine1(pluginParams, roiRectangle2Line1, roiController);
            SetRoiParamsLine2(pluginParams, roiRectangle2Line2, roiController);
            hWndCtrl.repaint();
            RequestFitRefresh();
        }

        private void FindIntersectionWpfView_Unloaded(object sender, RoutedEventArgs e)
        {
            fitRefreshTimer.Stop();
        }

        private void RequestFitRefresh()
        {
            fitRefreshPending = true;
        }

        private void FitRefreshTimer_Tick(object sender, EventArgs e)
        {
            if (!fitRefreshPending || fitRefreshRunning || syncing || hWndCtrl == null) return;
            if (pluginParams == null || pluginParams.ho_Image == null || !pluginParams.ho_Image.IsInitialized()) return;

            fitRefreshPending = false;
            fitRefreshRunning = true;
            try
            {
                EnsurePluginParams();
                SaveViewModelToParams();
                pluginsObj.ExecuteObj();
                hWndCtrl.repaint();
                UpdateResultDisplay();
            }
            catch
            {
                if (ViewModel != null) ViewModel.StatusText = "实时刷新失败，请检查 ROI 或参数";
            }
            finally
            {
                fitRefreshRunning = false;
            }
        }

        private void RunFunction()
        {
            if (hWndCtrl != null) hWndCtrl.ho_Image_Changed = pluginParams.ho_Image;
        }

        private void ShowResult()
        {
            RefreshRoiParams();
            var child = HalconElementHost.Child as HWindowControl;
            if (child != null)
            {
                ShowLineToHwd(pluginParams, child.HalconWindow, roiRectangle2Line1.size);
            }
        }

        private void SaveViewModelToParams()
        {
            EnsurePluginParams();
            var vm = ViewModel;
            if (vm == null || pluginParams == null) return;

            GetRoiParamsLine1(roiRectangle2Line1, pluginParams);
            GetRoiParamsLine2(roiRectangle2Line2, pluginParams);

            pluginParams.hv_Line1_RoiMidRow = vm.Line1CenterRow;
            pluginParams.hv_Line1_RoiMidColumn = vm.Line1CenterColumn;
            pluginParams.hv_Line1_RoiAngle = vm.Line1Angle;
            pluginParams.hv_Line1_RoiHeight = vm.Line1Height;
            pluginParams.hv_Line1_RoiWidth = vm.Line1Width;
            pluginParams.hv_Line1_RoiPointsNum = vm.Line1PointCount;
            pluginParams.hv_Line1_MeasureSigma = vm.Line1Sigma;
            pluginParams.hv_Line1_MeasureThreshold = vm.Line1Threshold;
            pluginParams.hv_Line1_MeasureNumInstances = vm.Line1NumInstances;
            pluginParams.hv_Line1_MeasureDistThreshold = vm.Line1DistThreshold;
            pluginParams.hv_Line1_MeasureMinScore = vm.Line1MinScore;
            pluginParams.hv_Line1_MaxNumIterations = vm.Line1MaxIterations;
            pluginParams.hv_Line1_MeasureTransition = vm.Line1Transition;
            pluginParams.hv_Line1_MeasureSelect = vm.Line1Select;

            pluginParams.hv_Line2_RoiMidRow = vm.Line2CenterRow;
            pluginParams.hv_Line2_RoiMidColumn = vm.Line2CenterColumn;
            pluginParams.hv_Line2_RoiAngle = vm.Line2Angle;
            pluginParams.hv_Line2_RoiHeight = vm.Line2Height;
            pluginParams.hv_Line2_RoiWidth = vm.Line2Width;
            pluginParams.hv_Line2_RoiPointsNum = vm.Line2PointCount;
            pluginParams.hv_Line2_MeasureSigma = vm.Line2Sigma;
            pluginParams.hv_Line2_MeasureThreshold = vm.Line2Threshold;
            pluginParams.hv_Line2_MeasureNumInstances = vm.Line2NumInstances;
            pluginParams.hv_Line2_MeasureDistThreshold = vm.Line2DistThreshold;
            pluginParams.hv_Line2_MeasureMinScore = vm.Line2MinScore;
            pluginParams.hv_Line2_MaxNumIterations = vm.Line2MaxIterations;
            pluginParams.hv_Line2_MeasureTransition = vm.Line2Transition;
            pluginParams.hv_Line2_MeasureSelect = vm.Line2Select;
        }

        private void PullParamsToViewModel()
        {
            EnsurePluginParams();
            var vm = ViewModel;
            if (vm == null || pluginParams == null) return;
            syncing = true;
            try
            {
                vm.UpdateFromRoi(1, pluginParams.hv_Line1_RoiMidRow, pluginParams.hv_Line1_RoiMidColumn,
                    pluginParams.hv_Line1_RoiAngle, pluginParams.hv_Line1_RoiHeight,
                    pluginParams.hv_Line1_RoiWidth, pluginParams.hv_Line1_RoiPointsNum);
                vm.Line1Sigma = pluginParams.hv_Line1_MeasureSigma;
                vm.Line1Threshold = pluginParams.hv_Line1_MeasureThreshold;
                vm.Line1NumInstances = pluginParams.hv_Line1_MeasureNumInstances;
                vm.Line1DistThreshold = pluginParams.hv_Line1_MeasureDistThreshold;
                vm.Line1MinScore = pluginParams.hv_Line1_MeasureMinScore;
                vm.Line1MaxIterations = pluginParams.hv_Line1_MaxNumIterations;
                vm.Line1Transition = pluginParams.hv_Line1_MeasureTransition;
                vm.Line1Select = pluginParams.hv_Line1_MeasureSelect;

                vm.UpdateFromRoi(2, pluginParams.hv_Line2_RoiMidRow, pluginParams.hv_Line2_RoiMidColumn,
                    pluginParams.hv_Line2_RoiAngle, pluginParams.hv_Line2_RoiHeight,
                    pluginParams.hv_Line2_RoiWidth, pluginParams.hv_Line2_RoiPointsNum);
                vm.Line2Sigma = pluginParams.hv_Line2_MeasureSigma;
                vm.Line2Threshold = pluginParams.hv_Line2_MeasureThreshold;
                vm.Line2NumInstances = pluginParams.hv_Line2_MeasureNumInstances;
                vm.Line2DistThreshold = pluginParams.hv_Line2_MeasureDistThreshold;
                vm.Line2MinScore = pluginParams.hv_Line2_MeasureMinScore;
                vm.Line2MaxIterations = pluginParams.hv_Line2_MaxNumIterations;
                vm.Line2Transition = pluginParams.hv_Line2_MeasureTransition;
                vm.Line2Select = pluginParams.hv_Line2_MeasureSelect;
            }
            finally
            {
                syncing = false;
            }
        }

        private void SetRoiParamsLine1(ParamsFindIntersection p, ROIRectangle2 roi, ROIController ctrl)
        {
            if (p == null || roi == null || ctrl == null) return;

            roi.PointNum = p.hv_Line1_RoiPointsNum;
            roi.ROIName = "Line1";
            roi.midR = p.hv_Line1_RoiMidRow;
            roi.midC = p.hv_Line1_RoiMidColumn;
            HTuple hvPhi;
            HOperatorSet.TupleRad(p.hv_Line1_RoiAngle, out hvPhi);
            roi.phi = hvPhi.D;
            roi.length2 = p.hv_Line1_RoiWidth / 2.0;
            roi.length1 = p.hv_Line1_RoiHeight / 2.0;

            if (!blRoiExistLine1)
            {
                blRoiExistLine1 = true;
                ctrl.setROIShape(roi);
                ctrl.roiMode.createROI(roi.midC, roi.midR);
                ctrl.ROIList.Add(ctrl.roiMode);
            }

            ctrl.activeROIidx = -1;
            roi.updateHandlePos();
        }

        private void SetRoiParamsLine2(ParamsFindIntersection p, ROIRectangle2 roi, ROIController ctrl)
        {
            if (p == null || roi == null || ctrl == null) return;

            roi.PointNum = p.hv_Line2_RoiPointsNum;
            roi.ROIName = "Line2";
            roi.midR = p.hv_Line2_RoiMidRow;
            roi.midC = p.hv_Line2_RoiMidColumn;
            HTuple hvPhi;
            HOperatorSet.TupleRad(p.hv_Line2_RoiAngle, out hvPhi);
            roi.phi = hvPhi.D;
            roi.length2 = p.hv_Line2_RoiWidth / 2.0;
            roi.length1 = p.hv_Line2_RoiHeight / 2.0;

            if (!blRoiExistLine2)
            {
                blRoiExistLine2 = true;
                ctrl.setROIShape(roi);
                ctrl.roiMode.createROI(roi.midC, roi.midR);
                ctrl.ROIList.Add(ctrl.roiMode);
            }

            ctrl.activeROIidx = -1;
            roi.updateHandlePos();
        }

        private void GetRoiParamsLine1(ROIRectangle2 roi, ParamsFindIntersection p)
        {
            if (roi == null) return;
            p.hv_Line1_RoiPointsNum = roi.PointNum;
            p.hv_Line1_RoiMidRow = roi.midR;
            p.hv_Line1_RoiMidColumn = roi.midC;
            HTuple hvAngle;
            HOperatorSet.TupleDeg(roi.phi, out hvAngle);
            p.hv_Line1_RoiAngle = hvAngle;
            p.hv_Line1_RoiWidth = roi.length2 * 2.0;
            p.hv_Line1_RoiHeight = roi.length1 * 2.0;
        }

        private void GetRoiParamsLine2(ROIRectangle2 roi, ParamsFindIntersection p)
        {
            if (roi == null) return;
            p.hv_Line2_RoiPointsNum = roi.PointNum;
            p.hv_Line2_RoiMidRow = roi.midR;
            p.hv_Line2_RoiMidColumn = roi.midC;
            HTuple hvAngle;
            HOperatorSet.TupleDeg(roi.phi, out hvAngle);
            p.hv_Line2_RoiAngle = hvAngle;
            p.hv_Line2_RoiWidth = roi.length2 * 2.0;
            p.hv_Line2_RoiHeight = roi.length1 * 2.0;
        }

        private void RefreshRoiParams()
        {
            syncing = true;
            try
            {
                GetRoiParamsLine1(roiRectangle2Line1, pluginParams);
                GetRoiParamsLine2(roiRectangle2Line2, pluginParams);
                if (ViewModel != null)
                {
                    ViewModel.UpdateFromRoi(1, roiRectangle2Line1.midR, roiRectangle2Line1.midC,
                        roiRectangle2Line1.phi * 180.0 / Math.PI, roiRectangle2Line1.length1 * 2.0,
                        roiRectangle2Line1.length2 * 2.0, roiRectangle2Line1.PointNum);
                    ViewModel.UpdateFromRoi(2, roiRectangle2Line2.midR, roiRectangle2Line2.midC,
                        roiRectangle2Line2.phi * 180.0 / Math.PI, roiRectangle2Line2.length1 * 2.0,
                        roiRectangle2Line2.length2 * 2.0, roiRectangle2Line2.PointNum);
                }
            }
            finally
            {
                syncing = false;
            }
        }

        private void ShowLineToHwd(ParamsFindIntersection p, HTuple hvWindowId, HTuple hvSize)
        {
            if (p == null || hvWindowId == null) return;

            try
            {
                if (p.hv_Line1_CrossRow != null && hWndCtrl.bl_RoiOperateFlag)
                {
                    HObject hoCross1;
                    HOperatorSet.GenCrossContourXld(out hoCross1, p.hv_Line1_CrossRow, p.hv_Line1_CrossColumn, hvSize, 0);
                    HOperatorSet.SetColor(hvWindowId, "green");
                    HOperatorSet.DispObj(hoCross1, hvWindowId);
                    hoCross1.Dispose();
                }

                if (p.hv_Line1_RowBegin != null && hWndCtrl.bl_RoiOperateFlag)
                {
                    HObject hoLine1;
                    HOperatorSet.GenRegionLine(out hoLine1, p.hv_Line1_RowBegin, p.hv_Line1_ColumnBegin, p.hv_Line1_RowEnd, p.hv_Line1_ColumnEnd);
                    HOperatorSet.SetColor(hvWindowId, "yellow");
                    HOperatorSet.DispObj(hoLine1, hvWindowId);
                    hoLine1.Dispose();
                }

                if (p.hv_Line2_CrossRow != null && hWndCtrl.bl_RoiOperateFlag)
                {
                    HObject hoCross2;
                    HOperatorSet.GenCrossContourXld(out hoCross2, p.hv_Line2_CrossRow, p.hv_Line2_CrossColumn, hvSize, 0);
                    HOperatorSet.SetColor(hvWindowId, "cyan");
                    HOperatorSet.DispObj(hoCross2, hvWindowId);
                    hoCross2.Dispose();
                }

                if (p.hv_Line2_RowBegin != null && hWndCtrl.bl_RoiOperateFlag)
                {
                    HObject hoLine2;
                    HOperatorSet.GenRegionLine(out hoLine2, p.hv_Line2_RowBegin, p.hv_Line2_ColumnBegin, p.hv_Line2_RowEnd, p.hv_Line2_ColumnEnd);
                    HOperatorSet.SetColor(hvWindowId, "magenta");
                    HOperatorSet.DispObj(hoLine2, hvWindowId);
                    hoLine2.Dispose();
                }

                if (p.hv_IntersectionRow != null && p.hv_IsParallel != null && p.hv_IsParallel.I == 0)
                {
                    HObject hoCrossIntersection;
                    HOperatorSet.GenCrossContourXld(out hoCrossIntersection, p.hv_IntersectionRow, p.hv_IntersectionColumn, 30, 0);
                    HOperatorSet.SetColor(hvWindowId, "red");
                    HOperatorSet.DispObj(hoCrossIntersection, hvWindowId);
                    HOperatorSet.SetTposition(hvWindowId, p.hv_IntersectionRow + 16, p.hv_IntersectionColumn + 16);
                    HOperatorSet.WriteString(hvWindowId, "Intersection");
                    hoCrossIntersection.Dispose();
                }

                if (crosshairEnabled && crossRow >= 0 && crossCol >= 0)
                {
                    HOperatorSet.SetColor(hvWindowId, "orange red");
                    HOperatorSet.SetDraw(hvWindowId, "margin");
                    HOperatorSet.DispLine(hvWindowId, crossRow, 0, crossRow, 99999);
                    HOperatorSet.DispLine(hvWindowId, 0, crossCol, 99999, crossCol);
                }
            }
            catch
            {
            }
        }

        private void RoiControllerNotifyRCObserver(int eventCode)
        {
            if (syncing) return;
            if (eventCode != ROIController.EVENT_UPDATE_ROI &&
                eventCode != ROIController.EVENT_MOVING_ROI &&
                eventCode != ROIController.EVENT_ACTIVATED_ROI)
            {
                return;
            }

            RefreshRoiParams();
            RequestFitRefresh();
        }

        private void HwcDisplayHMouseDown(object sender, HMouseEventArgs e)
        {
            if (!crosshairEnabled || e.Button != Forms.MouseButtons.Right) return;
            crosshairDragActive = true;
            UpdateCrosshair(e);
        }

        private void HwcDisplayHMouseUp(object sender, HMouseEventArgs e)
        {
            if (e.Button == Forms.MouseButtons.Right) crosshairDragActive = false;
        }

        private void HwcDisplayHMouseMove(object sender, HMouseEventArgs e)
        {
            var vm = ViewModel;
            try
            {
                if (vm != null && hWndCtrl != null)
                {
                    vm.MouseInfo = hWndCtrl.Get_Mouseposition_and_Gray(((HWindowControl)sender).HalconWindow, pluginParams.ho_Image, e);
                }
            }
            catch
            {
            }

            if (crosshairEnabled && crosshairDragActive)
            {
                UpdateCrosshair(e);
            }
        }

        private void UpdateCrosshair(HMouseEventArgs e)
        {
            crossRow = e.Y;
            crossCol = e.X;
            ExtractGrayProfiles(crossRow, crossCol);
            DrawProfile(HProfileCanvas, hProfileData, true);
            DrawProfile(VProfileCanvas, vProfileData, false);
            if (hWndCtrl != null) hWndCtrl.repaint();
        }

        private void ExtractGrayProfiles(double row, double col)
        {
            try
            {
                if (pluginParams.ho_Image == null || !pluginParams.ho_Image.IsInitialized()) return;

                HTuple width;
                HTuple height;
                HOperatorSet.GetImageSize(pluginParams.ho_Image, out width, out height);
                int w = Math.Max(0, width.I);
                int h = Math.Max(0, height.I);
                int r = Math.Max(0, Math.Min(h - 1, (int)Math.Round(row)));
                int c = Math.Max(0, Math.Min(w - 1, (int)Math.Round(col)));

                var hValues = new List<double>(w);
                for (int x = 0; x < w; x++)
                {
                    HTuple gray;
                    HOperatorSet.GetGrayval(pluginParams.ho_Image, r, x, out gray);
                    hValues.Add(gray.D);
                }

                var vValues = new List<double>(h);
                for (int y = 0; y < h; y++)
                {
                    HTuple gray;
                    HOperatorSet.GetGrayval(pluginParams.ho_Image, y, c, out gray);
                    vValues.Add(gray.D);
                }

                hProfileData = hValues.ToArray();
                vProfileData = vValues.ToArray();
            }
            catch
            {
            }
        }

        private static void DrawProfile(Canvas canvas, double[] data, bool horizontal)
        {
            if (canvas == null) return;
            canvas.Children.Clear();
            if (data == null || data.Length < 2) return;

            double width = Math.Max(1, canvas.ActualWidth);
            double height = Math.Max(1, canvas.ActualHeight);
            double min = double.MaxValue;
            double max = double.MinValue;
            foreach (double v in data)
            {
                if (v < min) min = v;
                if (v > max) max = v;
            }

            if (Math.Abs(max - min) < 0.0001) max = min + 1;

            var polyline = new Polyline
            {
                Stroke = horizontal ? System.Windows.Media.Brushes.Cyan : System.Windows.Media.Brushes.Lime,
                StrokeThickness = 1.3
            };

            int step = Math.Max(1, data.Length / 900);
            for (int i = 0; i < data.Length; i += step)
            {
                double x = width * i / Math.Max(1, data.Length - 1);
                double y = height - (data[i] - min) / (max - min) * height;
                polyline.Points.Add(new System.Windows.Point(x, y));
            }

            canvas.Children.Add(polyline);
        }

        private void UpdateResultDisplay()
        {
            var vm = ViewModel;
            if (vm == null) return;

            try
            {
                vm.Line1Status = pluginParams.bl_Line1_FitSuccess ? "线1拟合: 成功" : "线1拟合: 失败";
                vm.Line2Status = pluginParams.bl_Line2_FitSuccess ? "线2拟合: 成功" : "线2拟合: 失败";

                vm.Line1Info = pluginParams.bl_Line1_FitSuccess && pluginParams.hv_Line1_RowBegin != null
                    ? string.Format("线1: ({0:F1},{1:F1})->({2:F1},{3:F1})",
                        pluginParams.hv_Line1_RowBegin.D, pluginParams.hv_Line1_ColumnBegin.D,
                        pluginParams.hv_Line1_RowEnd.D, pluginParams.hv_Line1_ColumnEnd.D)
                    : "线1: 未检测到";

                vm.Line2Info = pluginParams.bl_Line2_FitSuccess && pluginParams.hv_Line2_RowBegin != null
                    ? string.Format("线2: ({0:F1},{1:F1})->({2:F1},{3:F1})",
                        pluginParams.hv_Line2_RowBegin.D, pluginParams.hv_Line2_ColumnBegin.D,
                        pluginParams.hv_Line2_RowEnd.D, pluginParams.hv_Line2_ColumnEnd.D)
                    : "线2: 未检测到";

                if (pluginParams.bl_IntersectionSuccess && pluginParams.hv_IntersectionRow != null && pluginParams.hv_IsParallel != null)
                {
                    if (pluginParams.hv_IsParallel.I == 0)
                    {
                        vm.IntersectionRow = pluginParams.hv_IntersectionRow.D;
                        vm.IntersectionColumn = pluginParams.hv_IntersectionColumn.D;
                        vm.Angle = pluginParams.hv_IntersectionAngle == null ? 0 : pluginParams.hv_IntersectionAngle.D;
                        vm.IntersectionText = string.Format("交点：R={0:F2}  C={1:F2}", vm.IntersectionRow, vm.IntersectionColumn);
                        vm.ParallelText = "是否平行: 否 (有交点)";
                    }
                    else
                    {
                        vm.IntersectionText = "交点：R=--  C=--";
                        vm.ParallelText = "是否平行: 是 (无交点)";
                    }
                }
                else
                {
                    vm.IntersectionText = "交点：R=--  C=--";
                    vm.ParallelText = "是否平行: --";
                }
            }
            catch
            {
                vm.ResetResult();
            }
        }
    }
}

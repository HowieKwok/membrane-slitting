using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using Frame_Core;
using Frame_Core.pluginParams;
using HalconDotNet;
using Microsoft.Win32;
using Plugin.FindIntersection.Controls;
using Plugin.FindIntersection.Services;
using Plugin.FindIntersection.ViewModels;
using ViewROI;
using Forms = System.Windows.Forms;

namespace Plugin.FindIntersection
{
    public partial class FindIntersectionWpfView : UserControl, IPluginWpfView, IToolActions
    {
        private readonly ToolVm vm;
        private readonly FindIntersectionOverlayPainter overlayPainter = new FindIntersectionOverlayPainter();
        private FindIntersectionObj pluginObject;
        private ParamsFindIntersection pluginParams;
        private HalconViewRoiAdapter mainDisplay;
        private FindIntersectionRoiBinder roiBinder;
        private HWindowControl halconWindow;
        private bool syncing;
        private bool crosshairEnabled;
        private bool crosshairDragActive;
        private double crossRow = -1;
        private double crossCol = -1;
        private double menuRow;
        private double menuCol;
        private double[] hProfileData;
        private double[] vProfileData;

        public FindIntersectionWpfView()
        {
            InitializeComponent();
            vm = new ToolVm(this);
            DataContext = vm;
            pluginParams = EnsurePluginParams();
            vm.RoiParameterChanged += OnViewModelRoiParameterChanged;
            HProfileCanvas.SizeChanged += delegate { DrawProfile(HProfileCanvas, hProfileData, true); };
            VProfileCanvas.SizeChanged += delegate { DrawProfile(VProfileCanvas, vProfileData, false); };
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        public FindIntersectionObj PluginObject { get { return pluginObject; } }
        public PluginParamsBase PluginParams { get { return pluginParams; } }
        IPlugin IPluginWpfView.PluginObject { get { return pluginObject; } }
        PluginParamsBase IPluginWpfView.PluginParams { get { return pluginParams; } }

        public void BindPlugin(IPlugin plugin)
        {
            pluginObject = plugin as FindIntersectionObj ?? new FindIntersectionObj();
            pluginParams = EnsurePluginParams();
            PullParamsToViewModel();
            ShowFrameImage(true);
            QueueShowFrameImage();
        }

        public void RefreshBindings()
        {
            DataContext = null;
            DataContext = vm;
        }

        public PluginViewActionResult RunFromHost()
        {
            Stopwatch watch = Stopwatch.StartNew();
            bool success = false;
            try
            {
                BeforeRun();
                success = pluginObject != null && pluginObject.ExecuteObj();
                watch.Stop();
                AfterRun(success, watch.ElapsedMilliseconds);
                string message = success ? "两线交点工具运行完成" : "两线交点工具运行失败：" + GetRunMessage();
                return new PluginViewActionResult { Success = success, Message = message, ElapsedMs = watch.ElapsedMilliseconds };
            }
            catch (Exception ex)
            {
                if (watch.IsRunning) watch.Stop();
                if (pluginParams != null)
                {
                    pluginParams.m_bRunSuccess = false;
                    pluginParams.m_strRunMessage = ex.Message;
                    pluginParams.m_fElapsedMs = watch.ElapsedMilliseconds;
                }

                vm.StatusText = "运行异常：" + ex.Message;
                if (pluginParams != null)
                {
                    pluginParams.m_bRunSuccess = false;
                    pluginParams.m_fElapsedMs = watch.ElapsedMilliseconds;
                }
                return PluginViewActionResult.Fail("两线交点工具运行异常：" + ex.Message, watch.ElapsedMilliseconds);
            }
        }

        public PluginViewActionResult SaveFromHost(string filePath)
        {
            Stopwatch watch = Stopwatch.StartNew();
            bool success = false;
            try
            {
                BeforeSave();
                if (pluginObject != null)
                {
                    pluginObject.moduleInfo.filePath = filePath;
                    if (pluginObject.PluginParams != null) pluginObject.PluginParams.filePath = filePath;
                    success = pluginObject.saveParams();
                }
                else if (pluginParams != null)
                {
                    pluginParams.filePath = filePath;
                    success = pluginParams.Save();
                }

                watch.Stop();
                AfterSave(success);
                return success ? PluginViewActionResult.Ok("两线交点参数已保存", watch.ElapsedMilliseconds) : PluginViewActionResult.Fail("两线交点参数保存失败", watch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                if (watch.IsRunning) watch.Stop();
                vm.StatusText = "保存异常：" + ex.Message;
                return PluginViewActionResult.Fail("两线交点参数保存异常：" + ex.Message, watch.ElapsedMilliseconds);
            }
        }

        public void CloseFromHost()
        {
            BeforeClose();
        }

        public void BeforeRun()
        {
            CommitViewToParams();
        }

        public void AfterRun(bool success, long elapsedMs)
        {
            UpdateResultDisplay();
            vm.StatusText = success ? "运行完成" : "运行失败：" + GetRunMessage();
            mainDisplay?.Repaint();
        }

        public void BeforeSave()
        {
            CommitViewToParams();
        }

        public void AfterSave(bool success)
        {
            vm.StatusText = success ? "参数已保存" : "参数保存失败";
        }

        public void BeforeClose()
        {
            BeforeSave();
            DisposeView();
        }

        public void LoadImage()
        {
            EnsurePluginParams();
            try
            {
                OpenFileDialog dialog = new OpenFileDialog { Filter = "图像文件|*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff|所有文件|*.*" };
                if (dialog.ShowDialog(Window.GetWindow(this)) != true) return;

                pluginParams.ho_Image = new HImage(dialog.FileName);
                PullParamsToViewModel();
                ShowFrameImage(true);
                vm.ResetResult();
                vm.StatusText = "图片已加载：" + System.IO.Path.GetFileName(dialog.FileName);
            }
            catch (Exception ex)
            {
                vm.StatusText = "加载图片失败：" + ex.Message;
                MessageBox.Show("加载图片失败：" + ex.Message, "两线交点工具", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void ToggleCrosshair()
        {
            crosshairEnabled = !crosshairEnabled;
            vm.StatusText = crosshairEnabled ? "十字光标已开启，按住右键查看灰度曲线" : "十字光标已关闭";
            mainDisplay?.Repaint();
        }

        public void FitImage()
        {
            if (!EnsureHalconView()) return;
            mainDisplay.FitImage();
            vm.StatusText = "已适应窗口";
        }

        public void KeepRoi()
        {
            if (!EnsureHalconView()) return;
            CommitViewToParams();
            ApplyParamsToViewRoi();
            mainDisplay?.Repaint();
            vm.StatusText = "ROI 已保存，点击运行刷新结果";
        }

        public void Run()
        {
            if (!EnsureHalconView()) return;
            RunFromHost();
        }

        public void SetBasePose()
        {
            SetFollowPoint(true);
        }

        public void SetCurrentPose()
        {
            SetFollowPoint(false);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            EnsureHalconView();
            ShowFrameImage(true);
            QueueShowFrameImage();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            DisposeView();
        }

        private ParamsFindIntersection EnsurePluginParams()
        {
            if (pluginObject == null) pluginObject = new FindIntersectionObj();
            if (pluginObject.PluginParams == null) pluginObject.PluginParams = new ParamsFindIntersection();

            pluginParams = pluginObject.PluginParams as ParamsFindIntersection;
            if (pluginParams == null)
            {
                pluginParams = new ParamsFindIntersection();
                pluginObject.PluginParams = pluginParams;
            }

            return pluginParams;
        }

        private bool EnsureHalconView()
        {
            if (mainDisplay != null) return true;

            try
            {
                vm.StatusText = "正在初始化 Halcon / ViewROI";
                halconWindow = new HWindowControl { Dock = Forms.DockStyle.Fill, BackColor = System.Drawing.Color.FromArgb(1, 3, 8) };
                HalconElementHost.SizeChanged += OnHalconHostSizeChanged;
                HalconElementHost.Child = halconWindow;

                mainDisplay = new HalconViewRoiAdapter(halconWindow, true);
                roiBinder = new FindIntersectionRoiBinder(mainDisplay.RoiController);
                mainDisplay.Redraw += DrawOverlay;
                mainDisplay.RoiChanged += OnViewRoiChanged;

                halconWindow.DoubleClick += OnHalconDoubleClick;
                halconWindow.HMouseMove += OnHalconMouseMove;
                halconWindow.HMouseDown += OnHalconMouseDown;
                halconWindow.HMouseUp += OnHalconMouseUp;

                vm.StatusText = "ViewROI 初始化完成";
                return true;
            }
            catch (Exception ex)
            {
                vm.StatusText = "Halcon 初始化失败：" + ex.Message;
                MessageBox.Show("Halcon 初始化失败：" + ex.Message, "两线交点工具", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
        }

        private void ShowFrameImage(bool fit)
        {
            if (!HasValidImage(pluginParams == null ? null : pluginParams.ho_Image)) return;
            if (!EnsureHalconView()) return;

            mainDisplay.ShowImage(pluginParams.ho_Image, fit);
            ApplyParamsToViewRoi();
            mainDisplay.Repaint();
            EmptyHint.Visibility = Visibility.Collapsed;
        }

        private void QueueShowFrameImage()
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                if (mainDisplay == null) return;
                ShowFrameImage(true);
            }), System.Windows.Threading.DispatcherPriority.ContextIdle);
        }

        private void OnHalconHostSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width < 10 || e.NewSize.Height < 10) return;
            ShowFrameImage(true);
        }

        private static bool HasValidImage(HObject image)
        {
            if (image == null || !image.IsInitialized()) return false;
            try
            {
                HOperatorSet.CountObj(image, out HTuple count);
                if (count.I <= 0) return false;
                HOperatorSet.GetObjClass(image, out HTuple objClass);
                return string.Equals(objClass.S, "image", StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }
        private void ApplyParamsToViewRoi()
        {
            if (roiBinder == null || pluginParams == null) return;
            syncing = true;
            try { roiBinder.ApplyParamsToViewRoi(pluginParams); }
            finally { syncing = false; }
        }

        private void CommitViewToParams()
        {
            EnsurePluginParams();
            CommitBindings();
            roiBinder?.SaveViewRoiToParams(pluginParams);
            vm.CommitToFrameParams();
        }

        private void PullParamsToViewModel()
        {
            EnsurePluginParams();
            syncing = true;
            try { vm.BindFrameParams(pluginParams); }
            finally { syncing = false; }
        }

        private void SaveViewRoiToViewModel()
        {
            if (roiBinder == null || pluginParams == null) return;
            syncing = true;
            try
            {
                roiBinder.SaveViewRoiToParams(pluginParams);
                roiBinder.SaveViewRoiToViewModel(vm);
            }
            finally { syncing = false; }
        }

        private void OnViewModelRoiParameterChanged(object sender, EventArgs e)
        {
            if (syncing || mainDisplay == null) return;
            vm.CommitToFrameParams();
            ApplyParamsToViewRoi();
            mainDisplay.Repaint();
        }

        private void OnViewRoiChanged(int eventCode)
        {
            if (syncing) return;
            if (eventCode != ROIController.EVENT_UPDATE_ROI && eventCode != ROIController.EVENT_MOVING_ROI && eventCode != ROIController.EVENT_ACTIVATED_ROI) return;

            SaveViewRoiToViewModel();
            mainDisplay?.Repaint();
            if (eventCode == ROIController.EVENT_UPDATE_ROI) vm.StatusText = "ROI 已更新，点击运行刷新结果";
        }

        private void DrawOverlay()
        {
            if (halconWindow == null || pluginParams == null) return;
            pluginObject.ExecuteObj();
            overlayPainter.Draw(halconWindow.HalconWindow, pluginParams, crossRow, crossCol, crosshairEnabled, 8);
        }

        private void OnHalconDoubleClick(object sender, EventArgs e)
        {
            mainDisplay?.FitImage();
        }

        private void OnHalconMouseDown(object sender, HMouseEventArgs e)
        {
            if (e.Button != Forms.MouseButtons.Right) return;

            menuRow = e.Y;
            menuCol = e.X;
            if (crosshairEnabled)
            {
                crosshairDragActive = true;
                UpdateCrosshair(e);
            }

            FollowMenu.IsOpen = true;
        }

        private void OnHalconMouseUp(object sender, HMouseEventArgs e)
        {
            if (e.Button == Forms.MouseButtons.Right) crosshairDragActive = false;
        }

        private void OnHalconMouseMove(object sender, HMouseEventArgs e)
        {
            if (mainDisplay != null && pluginParams != null) vm.MouseInfo = mainDisplay.GetMouseText(e);
            if (crosshairEnabled && crosshairDragActive) UpdateCrosshair(e);
        }

        private void SetFollowPoint(bool isBase)
        {
            EnsurePluginParams();
            double angle = roiBinder == null ? 0 : roiBinder.GetActiveAngleDegree();

            if (isBase)
            {
                vm.Params.BaseRow = menuRow;
                vm.Params.BaseColumn = menuCol;
                vm.Params.BaseAngle = angle;
                vm.StatusText = string.Format("已设置基坐标基准：R={0:F2}  C={1:F2}  A={2:F3}", menuRow, menuCol, angle);
            }
            else
            {
                vm.Params.CurrentRow = menuRow;
                vm.Params.CurrentColumn = menuCol;
                vm.Params.CurrentAngle = angle;
                vm.Params.CurrentPoseValid = true;
                vm.Params.EnablePositionFollow = true;
                vm.StatusText = string.Format("已设置当前输入坐标：R={0:F2}  C={1:F2}  A={2:F3}", menuRow, menuCol, angle);
            }

            FollowMenu.IsOpen = false;
            CommitViewToParams();
            mainDisplay?.Repaint();
        }

        private void UpdateCrosshair(HMouseEventArgs e)
        {
            crossRow = e.Y;
            crossCol = e.X;
            ExtractGrayProfiles(crossRow, crossCol);
            DrawProfile(HProfileCanvas, hProfileData, true);
            DrawProfile(VProfileCanvas, vProfileData, false);
            mainDisplay?.Repaint();
        }

        private void ExtractGrayProfiles(double row, double col)
        {
            try
            {
                if (pluginParams == null || pluginParams.ho_Image == null || !pluginParams.ho_Image.IsInitialized()) return;

                HObject profileImage = pluginParams.ho_Image;
                HOperatorSet.CountChannels(profileImage, out HTuple channels);
                if (channels.I > 1) HOperatorSet.AccessChannel(profileImage, out profileImage, 1);

                try
                {
                    HOperatorSet.GetImageSize(profileImage, out HTuple width, out HTuple height);
                    int w = Math.Max(0, width.I);
                    int h = Math.Max(0, height.I);
                    int r = Math.Max(0, Math.Min(h - 1, (int)Math.Round(row)));
                    int c = Math.Max(0, Math.Min(w - 1, (int)Math.Round(col)));

                    List<double> hValues = new List<double>(w);
                    for (int x = 0; x < w; x++)
                    {
                        HOperatorSet.GetGrayval(profileImage, r, x, out HTuple gray);
                        hValues.Add(gray.D);
                    }

                    List<double> vValues = new List<double>(h);
                    for (int y = 0; y < h; y++)
                    {
                        HOperatorSet.GetGrayval(profileImage, y, c, out HTuple gray);
                        vValues.Add(gray.D);
                    }

                    hProfileData = hValues.ToArray();
                    vProfileData = vValues.ToArray();
                }
                finally
                {
                    if (!ReferenceEquals(profileImage, pluginParams.ho_Image)) profileImage.Dispose();
                }
            }
            catch { }
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
            foreach (double value in data)
            {
                if (value < min) min = value;
                if (value > max) max = value;
            }

            if (Math.Abs(max - min) < 0.0001) max = min + 1;

            Polyline line = new Polyline { Stroke = horizontal ? Brushes.Cyan : Brushes.Lime, StrokeThickness = 1.3 };
            int step = Math.Max(1, data.Length / 900);
            for (int i = 0; i < data.Length; i += step)
            {
                double x = width * i / Math.Max(1, data.Length - 1);
                double y = height - (data[i] - min) / (max - min) * height;
                line.Points.Add(new Point(x, y));
            }

            canvas.Children.Add(line);
        }

        private void UpdateResultDisplay()
        {
            try { vm.LoadResultsFromParams(pluginParams); }
            catch { vm.ResetResult(); }
        }

        private string GetRunMessage()
        {
            return pluginParams == null ? string.Empty : pluginParams.m_strRunMessage;
        }

        private void CommitBindings()
        {
            System.Windows.Input.FocusManager.SetFocusedElement(this, this);
            UpdateBindingSources(this);
        }

        private void UpdateBindingSources(DependencyObject parent)
        {
            if (parent == null) return;

            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);

                NumericBox numericBox = child as NumericBox;
                if (numericBox != null) numericBox.GetBindingExpression(NumericBox.ValueProperty)?.UpdateSource();

                CheckBox checkBox = child as CheckBox;
                if (checkBox != null) checkBox.GetBindingExpression(CheckBox.IsCheckedProperty)?.UpdateSource();

                ComboBox comboBox = child as ComboBox;
                if (comboBox != null)
                {
                    comboBox.GetBindingExpression(ComboBox.SelectedValueProperty)?.UpdateSource();
                    comboBox.GetBindingExpression(ComboBox.TextProperty)?.UpdateSource();
                }

                UpdateBindingSources(child);
            }
        }

        private void DisposeView()
        {
            if (mainDisplay != null)
            {
                mainDisplay.Redraw -= DrawOverlay;
                mainDisplay.RoiChanged -= OnViewRoiChanged;
                mainDisplay.Dispose();
                mainDisplay = null;
            }

            if (halconWindow != null)
            {
                HalconElementHost.SizeChanged -= OnHalconHostSizeChanged;
                halconWindow.DoubleClick -= OnHalconDoubleClick;
                halconWindow.HMouseMove -= OnHalconMouseMove;
                halconWindow.HMouseDown -= OnHalconMouseDown;
                halconWindow.HMouseUp -= OnHalconMouseUp;
                halconWindow.Dispose();
                halconWindow = null;
            }

            roiBinder = null;
            HalconElementHost.Child = null;
        }
    }
}



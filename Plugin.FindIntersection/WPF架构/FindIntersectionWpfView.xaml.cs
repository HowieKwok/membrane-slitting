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
    /// <summary>
    /// 两线交点工具的 WPF 视图。
    /// 负责显示图像、管理 Halcon 窗口、处理鼠标交互、绘制 ROI 和运行结果。
    /// 实现 IPluginWpfView 和 IToolActions 以接入框架的生命周期。
    /// </summary>
    public partial class FindIntersectionWpfView : UserControl, IPluginWpfView, IToolActions
    {
        // --------------------------- 私有字段 ---------------------------

        /// <summary>视图模型，负责业务数据和命令</summary>
        private readonly ToolVm vm;

        /// <summary>覆盖层绘制器，用于在 Halcon 窗口上高亮交点、轮廓等</summary>
        private readonly FindIntersectionOverlayPainter overlayPainter = new FindIntersectionOverlayPainter();

        /// <summary>核心插件对象，包含算法执行逻辑</summary>
        private FindIntersectionObj pluginObject;

        /// <summary>插件参数，与视图模型双向绑定</summary>
        private ParamsFindIntersection pluginParams;

        /// <summary>封装了 HWindowControl 和 ROI 控制器的适配器</summary>
        private HalconViewRoiAdapter mainDisplay;

        /// <summary>负责同步界面 ROI 与参数中的 ROI 数据</summary>
        private FindIntersectionRoiBinder roiBinder;

        /// <summary>Halcon 图像显示控件（实际托管在 WindowsFormsHost 中）</summary>
        private HWindowControl halconWindow;

        /// <summary>防止循环同步的标志</summary>
        private bool syncing;

        /// <summary>十字光标是否启用（右键拖动显示灰度曲线）</summary>
        private bool crosshairEnabled;

        /// <summary>十字光标拖拽是否处于活动状态（右键按下且移动）</summary>
        private bool crosshairDragActive;

        /// <summary>十字光标当前的行坐标（图像坐标）</summary>
        private double crossRow = -1;

        /// <summary>十字光标当前的列坐标（图像坐标）</summary>
        private double crossCol = -1;

        /// <summary>右键菜单弹出时鼠标的行坐标（图像坐标）</summary>
        private double menuRow;

        /// <summary>右键菜单弹出时鼠标的列坐标（图像坐标）</summary>
        private double menuCol;

        /// <summary>水平方向灰度曲线数据（当前十字光标所在行的像素灰度值）</summary>
        private double[] hProfileData;

        /// <summary>垂直方向灰度曲线数据（当前十字光标所在列的像素灰度值）</summary>
        private double[] vProfileData;

        // --------------------------- 构造函数 ---------------------------

        public FindIntersectionWpfView()
        {
            InitializeComponent();
            vm = new ToolVm(this);
            DataContext = vm;
            pluginParams = EnsurePluginParams();

            // 当视图模型中的 ROI 参数发生变化时，同步到界面 ROI
            vm.RoiParameterChanged += OnViewModelRoiParameterChanged;

            // 当绘制曲线的画布大小改变时，重新绘制曲线
            HProfileCanvas.SizeChanged += delegate { DrawProfile(HProfileCanvas, hProfileData, true); };
            VProfileCanvas.SizeChanged += delegate { DrawProfile(VProfileCanvas, vProfileData, false); };

            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        // --------------------------- 公开属性 ---------------------------

        /// <summary>插件对象（强类型）</summary>
        public FindIntersectionObj PluginObject => pluginObject;

        /// <summary>插件参数（基类型）</summary>
        public PluginParamsBase PluginParams => pluginParams;

        /// <summary>插件对象（接口类型）</summary>
        IPlugin IPluginWpfView.PluginObject => pluginObject;

        // --------------------------- IPluginWpfView 实现 ---------------------------

        /// <summary>将外部传入的插件对象绑定到视图</summary>
        public void BindPlugin(IPlugin plugin)
        {
            pluginObject = plugin as FindIntersectionObj ?? new FindIntersectionObj();
            pluginParams = EnsurePluginParams();
            PullParamsToViewModel();          // 将参数加载到视图模型
            ShowFrameImage(true);             // 显示图像
            QueueShowFrameImage();            // 延迟刷新一次，确保窗口完全初始化
        }

        /// <summary>刷新数据绑定（用于解决某些情况下绑定失效的问题）</summary>
        public void RefreshBindings()
        {
            DataContext = null;
            DataContext = vm;
        }

        /// <summary>从宿主环境执行工具（运行算法）</summary>
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
                return PluginViewActionResult.Fail("两线交点工具运行异常：" + ex.Message, watch.ElapsedMilliseconds);
            }
        }

        /// <summary>保存参数到文件</summary>
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
                return success ? PluginViewActionResult.Ok("两线交点参数已保存", watch.ElapsedMilliseconds)
                               : PluginViewActionResult.Fail("两线交点参数保存失败", watch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                if (watch.IsRunning) watch.Stop();
                vm.StatusText = "保存异常：" + ex.Message;
                return PluginViewActionResult.Fail("两线交点参数保存异常：" + ex.Message, watch.ElapsedMilliseconds);
            }
        }

        /// <summary>关闭视图前释放资源</summary>
        public void CloseFromHost()
        {
            BeforeClose();
        }

        // --------------------------- IToolActions 实现 ---------------------------

        /// <summary>运行前的准备工作（提交界面参数到内部对象）</summary>
        public void BeforeRun()
        {
            CommitViewToParams();
        }

        /// <summary>运行后的处理（更新结果显示，刷新状态）</summary>
        public void AfterRun(bool success, long elapsedMs)
        {
            UpdateResultDisplay();
            vm.StatusText = success ? "运行完成" : "运行失败：" + GetRunMessage();
            mainDisplay?.Repaint();
        }

        /// <summary>保存前的准备工作</summary>
        public void BeforeSave()
        {
            CommitViewToParams();
        }

        /// <summary>保存后的反馈</summary>
        public void AfterSave(bool success)
        {
            vm.StatusText = success ? "参数已保存" : "参数保存失败";
        }

        /// <summary>关闭前的清理</summary>
        public void BeforeClose()
        {
            BeforeSave();
            DisposeView();
        }

        // --------------------------- 工具栏命令实现 ---------------------------

        /// <summary>加载本地图像文件</summary>
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

        /// <summary>切换十字光标模式（右键拖动显示灰度曲线）</summary>
        public void ToggleCrosshair()
        {
            crosshairEnabled = !crosshairEnabled;
            vm.StatusText = crosshairEnabled ? "十字光标已开启，按住右键查看灰度曲线" : "十字光标已关闭";
            mainDisplay?.Repaint();
        }

        /// <summary>将图像适应窗口大小</summary>
        public void FitImage()
        {
            if (!EnsureHalconView()) return;
            mainDisplay.FitImage();
            vm.StatusText = "已适应窗口";
        }

        /// <summary>保存当前 ROI 到参数中</summary>
        public void KeepRoi()
        {
            if (!EnsureHalconView()) return;
            CommitViewToParams();
            ApplyParamsToViewRoi();
            mainDisplay?.Repaint();
            vm.StatusText = "ROI 已保存，点击运行刷新结果";
        }

        /// <summary>运行工具</summary>
        public void Run()
        {
            if (!EnsureHalconView()) return;
            RunFromHost();
        }

        /// <summary>将当前右键菜单位置设为基坐标基准</summary>
        public void SetBasePose()
        {
            SetFollowPoint(true);
        }

        /// <summary>将当前右键菜单位置设为当前跟随坐标</summary>
        public void SetCurrentPose()
        {
            SetFollowPoint(false);
        }

        // --------------------------- 生命周期事件 ---------------------------

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            EnsureHalconView();
            ShowFrameImage(true);
            QueueShowFrameImage(); // 确保图像最终显示
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            DisposeView();
        }

        // --------------------------- 初始化与参数管理 ---------------------------

        /// <summary>确保插件参数对象存在并返回</summary>
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

        /// <summary>确保 Halcon 窗口及 ROI 控制器已初始化</summary>
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

                // 订阅 Halcon 控件的事件
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

        /// <summary>显示图像（可选择是否适应窗口）</summary>
        private void ShowFrameImage(bool fit)
        {
            if (!HasValidImage(pluginParams?.ho_Image)) return;
            if (!EnsureHalconView()) return;

            mainDisplay.ShowImage(pluginParams.ho_Image, fit);
            ApplyParamsToViewRoi();   // 将参数中的 ROI 显示到界面上
            mainDisplay.Repaint();
            EmptyHint.Visibility = Visibility.Collapsed;
        }

        /// <summary>延迟调用 ShowFrameImage，用于界面完全加载后刷新</summary>
        private void QueueShowFrameImage()
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                if (mainDisplay == null) return;
                ShowFrameImage(true);
            }), System.Windows.Threading.DispatcherPriority.ContextIdle);
        }

        /// <summary>Halcon 控件所在容器尺寸改变时，重新适应图像</summary>
        private void OnHalconHostSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width < 10 || e.NewSize.Height < 10) return;
            ShowFrameImage(true);
        }

        /// <summary>判断 HObject 是否为有效的 Halcon 图像对象</summary>
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

        // --------------------------- ROI 同步逻辑 ---------------------------

        /// <summary>将参数中的 ROI 数据应用到界面 ROI 控制器</summary>
        private void ApplyParamsToViewRoi()
        {
            if (roiBinder == null || pluginParams == null) return;
            syncing = true;
            try { roiBinder.ApplyParamsToViewRoi(pluginParams); }
            finally { syncing = false; }
        }

        /// <summary>将界面 ROI 数据保存到参数对象</summary>
        private void CommitViewToParams()
        {
            EnsurePluginParams();
            CommitBindings();               // 强制更新所有绑定源
            roiBinder?.SaveViewRoiToParams(pluginParams);
            vm.CommitToFrameParams();       // 视图模型中的其他参数也同步到参数对象
        }

        /// <summary>将插件参数加载到视图模型</summary>
        private void PullParamsToViewModel()
        {
            EnsurePluginParams();
            syncing = true;
            try { vm.BindFrameParams(pluginParams); }
            finally { syncing = false; }
        }

        /// <summary>将界面 ROI 数据同步到视图模型</summary>
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

        /// <summary>视图模型中 ROI 参数变化时，同步到界面 ROI</summary>
        private void OnViewModelRoiParameterChanged(object sender, EventArgs e)
        {
            if (syncing || mainDisplay == null) return;
            vm.CommitToFrameParams();
            ApplyParamsToViewRoi();
            mainDisplay.Repaint();
        }

        /// <summary>界面 ROI 发生变化时，保存回视图模型</summary>
        private void OnViewRoiChanged(int eventCode)
        {
            if (syncing) return;
            if (eventCode != ROIController.EVENT_UPDATE_ROI && eventCode != ROIController.EVENT_MOVING_ROI && eventCode != ROIController.EVENT_ACTIVATED_ROI) return;

            SaveViewRoiToViewModel();
            mainDisplay?.Repaint();
            if (eventCode == ROIController.EVENT_UPDATE_ROI)
                vm.StatusText = "ROI 已更新，点击运行刷新结果";
        }

        // --------------------------- 绘制覆盖层 ---------------------------

        /// <summary>在 Halcon 窗口上绘制覆盖层（交点、直线、十字光标等）</summary>
        private void DrawOverlay()
        {
            if (halconWindow == null || pluginParams == null) return;
            // 每次重绘前先执行一次算法，确保结果是最新的（注意：频繁执行可能影响性能，此处为简化实现）
            pluginObject.ExecuteObj();
            overlayPainter.Draw(halconWindow.HalconWindow, pluginParams, crossRow, crossCol, crosshairEnabled, 8);
        }

        // --------------------------- Halcon 控件事件处理 ---------------------------

        private void OnHalconDoubleClick(object sender, EventArgs e)
        {
            mainDisplay?.FitImage();
        }

        private void OnHalconMouseDown(object sender, HMouseEventArgs e)
        {
            if (e.Button != Forms.MouseButtons.Right) return;

            // 记录右键菜单弹出时的图像坐标
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
            if (e.Button == Forms.MouseButtons.Right)
                crosshairDragActive = false;
        }

        private void OnHalconMouseMove(object sender, HMouseEventArgs e)
        {
            if (mainDisplay != null && pluginParams != null)
                vm.MouseInfo = mainDisplay.GetMouseText(e);   // 更新鼠标位置信息

            if (crosshairEnabled && crosshairDragActive)
                UpdateCrosshair(e);
        }

        // --------------------------- 十字光标与灰度曲线 ---------------------------

        /// <summary>设置跟随点（基坐标或当前位姿）</summary>
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

        /// <summary>更新十字光标位置并提取灰度曲线</summary>
        private void UpdateCrosshair(HMouseEventArgs e)
        {
            crossRow = e.Y;
            crossCol = e.X;
            ExtractGrayProfiles(crossRow, crossCol);
            DrawProfile(HProfileCanvas, hProfileData, true);
            DrawProfile(VProfileCanvas, vProfileData, false);
            mainDisplay?.Repaint();
        }

        /// <summary>从图像中提取指定像素行和列的灰度值曲线</summary>
        private void ExtractGrayProfiles(double row, double col)
        {
            try
            {
                if (pluginParams == null || pluginParams.ho_Image == null || !pluginParams.ho_Image.IsInitialized())
                    return;

                HObject profileImage = pluginParams.ho_Image;
                HOperatorSet.CountChannels(profileImage, out HTuple channels);
                if (channels.I > 1)
                    HOperatorSet.AccessChannel(profileImage, out profileImage, 1);  // 取单通道

                try
                {
                    HOperatorSet.GetImageSize(profileImage, out HTuple width, out HTuple height);
                    int w = Math.Max(0, width.I);
                    int h = Math.Max(0, height.I);
                    int r = Math.Max(0, Math.Min(h - 1, (int)Math.Round(row)));
                    int c = Math.Max(0, Math.Min(w - 1, (int)Math.Round(col)));

                    // 提取水平方向灰度值
                    List<double> hValues = new List<double>(w);
                    for (int x = 0; x < w; x++)
                    {
                        HOperatorSet.GetGrayval(profileImage, r, x, out HTuple gray);
                        hValues.Add(gray.D);
                    }

                    // 提取垂直方向灰度值
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
                    if (!ReferenceEquals(profileImage, pluginParams.ho_Image))
                        profileImage.Dispose();   // 释放临时图像对象
                }
            }
            catch { /* 静默失败，保持原有数据不变 */ }
        }

        /// <summary>在 Canvas 上绘制灰度曲线</summary>
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

            if (Math.Abs(max - min) < 0.0001)
                max = min + 1;   // 避免除零

            Polyline line = new Polyline
            {
                Stroke = horizontal ? Brushes.Cyan : Brushes.Lime,
                StrokeThickness = 1.3
            };
            int step = Math.Max(1, data.Length / 900);   // 采样步长，防止点过多
            for (int i = 0; i < data.Length; i += step)
            {
                double x = width * i / Math.Max(1, data.Length - 1);
                double y = height - (data[i] - min) / (max - min) * height;
                line.Points.Add(new Point(x, y));
            }
            canvas.Children.Add(line);
        }

        // --------------------------- 结果显示辅助 ---------------------------

        /// <summary>从参数中更新视图模型的结果显示</summary>
        private void UpdateResultDisplay()
        {
            try { vm.LoadResultsFromParams(pluginParams); }
            catch { vm.ResetResult(); }
        }

        /// <summary>获取最后一次运行的错误消息</summary>
        private string GetRunMessage()
        {
            return pluginParams == null ? string.Empty : pluginParams.m_strRunMessage;
        }

        // --------------------------- 绑定强制刷新 ---------------------------

        /// <summary>强制提交所有绑定的输入控件（如 NumericBox、CheckBox）的值到绑定源</summary>
        private void CommitBindings()
        {
            System.Windows.Input.FocusManager.SetFocusedElement(this, this);
            UpdateBindingSources(this);
        }

        /// <summary>递归更新所有子控件的绑定源</summary>
        private void UpdateBindingSources(DependencyObject parent)
        {
            if (parent == null) return;

            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);

                if (child is NumericBox numericBox)
                    numericBox.GetBindingExpression(NumericBox.ValueProperty)?.UpdateSource();

                if (child is CheckBox checkBox)
                    checkBox.GetBindingExpression(CheckBox.IsCheckedProperty)?.UpdateSource();

                if (child is ComboBox comboBox)
                {
                    comboBox.GetBindingExpression(ComboBox.SelectedValueProperty)?.UpdateSource();
                    comboBox.GetBindingExpression(ComboBox.TextProperty)?.UpdateSource();
                }

                UpdateBindingSources(child);
            }
        }

        // --------------------------- 资源释放 ---------------------------

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
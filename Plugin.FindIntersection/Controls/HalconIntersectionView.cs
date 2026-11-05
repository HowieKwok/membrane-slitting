using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using Frame_Core.pluginParams;
using HalconDotNet;
using Plugin.FindIntersection.Services;
using ViewROI;
using Forms = System.Windows.Forms;

namespace Plugin.FindIntersection.Controls
{
    /// <summary>
    /// Halcon + ViewROI 显示控件（纯 View 层）。
    /// 
    /// 设计目的：
    /// 将所有与第三方控件（HWindowControl、WindowsFormsHost、ROI 控制器）相关的
    /// 底层细节封装在此 UserControl 内，不泄漏给 ViewModel。
    /// 
    /// 与 ViewModel 的通讯：
    /// - 输入（VM → View）：通过依赖属性绑定（图像、ROI 参数、十字光标、命令等）。
    /// - 输出（View → VM）：通过 ICommand 属性（鼠标事件、ROI 变更事件）执行命令。
    /// 
    /// 主要功能：
    /// 1. 显示 Halcon 图像，并支持适应窗口。
    /// 2. 管理两条可拖拽的旋转矩形 ROI（直线提取区域）。
    /// 3. 绘制叠加层（直线、交点、十字光标等）。
    /// 4. 将用户交互（鼠标移动、右键点击、ROI 拖放结束）转换为命令回调。
    /// </summary>
    public sealed class HalconIntersectionView : UserControl, IDisposable
    {
        // ==================== 依赖属性定义 ====================
        // 这些属性让外界（主要是 ViewModel）只需通过数据绑定就能控制控件，
        // 无需直接引用控件实例。

        /// <summary>当前显示的 Halcon 图像对象。</summary>
        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register(nameof(Image), typeof(HObject), typeof(HalconIntersectionView),
                new PropertyMetadata(null, OnDisplayPropertyChanged));

        /// <summary>叠加层绘制所需的参数（交点、直线等）。</summary>
        public static readonly DependencyProperty OverlayParamsProperty =
            DependencyProperty.Register(nameof(OverlayParams), typeof(ParamsFindIntersection), typeof(HalconIntersectionView),
                new PropertyMetadata(null, OnDisplayPropertyChanged));

        /// <summary>
        /// 显示刷新计数器。
        /// 由于 OverlayParams 是引用类型，其内部值改变时依赖属性不会自动触发重绘。
        /// ViewModel 每次需要强制刷新叠加层时，只需对该属性递增 1。
        /// </summary>
        public static readonly DependencyProperty DisplayRevisionProperty =
            DependencyProperty.Register(nameof(DisplayRevision), typeof(int), typeof(HalconIntersectionView),
                new PropertyMetadata(0, OnDisplayPropertyChanged));

        /// <summary>下一次显示图像时是否需要适应窗口大小。</summary>
        public static readonly DependencyProperty FitOnNextDisplayProperty =
            DependencyProperty.Register(nameof(FitOnNextDisplay), typeof(bool), typeof(HalconIntersectionView),
                new PropertyMetadata(false));

        /// <summary>是否显示十字光标。</summary>
        public static readonly DependencyProperty CrosshairEnabledProperty =
            DependencyProperty.Register(nameof(CrosshairEnabled), typeof(bool), typeof(HalconIntersectionView),
                new PropertyMetadata(false, OnDisplayPropertyChanged));

        /// <summary>十字光标行坐标（图像坐标）。</summary>
        public static readonly DependencyProperty CrossRowProperty =
            DependencyProperty.Register(nameof(CrossRow), typeof(double), typeof(HalconIntersectionView),
                new PropertyMetadata(-1.0, OnDisplayPropertyChanged));

        /// <summary>十字光标列坐标（图像坐标）。</summary>
        public static readonly DependencyProperty CrossColProperty =
            DependencyProperty.Register(nameof(CrossCol), typeof(double), typeof(HalconIntersectionView),
                new PropertyMetadata(-1.0, OnDisplayPropertyChanged));

        // 以下 12 个属性为两条 ROI 线的几何参数，使用双向绑定。
        // 当 ViewModel 修改这些值时，会自动更新界面上 ROI 的形状；
        // 当用户在界面上拖动 ROI 后，这些值也会自动写回 ViewModel。
        public static readonly DependencyProperty Line1CenterRowProperty = RegisterRoiProperty(nameof(Line1CenterRow));
        public static readonly DependencyProperty Line1CenterColumnProperty = RegisterRoiProperty(nameof(Line1CenterColumn));
        public static readonly DependencyProperty Line1AngleProperty = RegisterRoiProperty(nameof(Line1Angle));
        public static readonly DependencyProperty Line1HeightProperty = RegisterRoiProperty(nameof(Line1Height), 1.0);
        public static readonly DependencyProperty Line1WidthProperty = RegisterRoiProperty(nameof(Line1Width), 1.0);
        public static readonly DependencyProperty Line1PointCountProperty = RegisterRoiProperty(nameof(Line1PointCount), 2.0);
        public static readonly DependencyProperty Line2CenterRowProperty = RegisterRoiProperty(nameof(Line2CenterRow));
        public static readonly DependencyProperty Line2CenterColumnProperty = RegisterRoiProperty(nameof(Line2CenterColumn));
        public static readonly DependencyProperty Line2AngleProperty = RegisterRoiProperty(nameof(Line2Angle));
        public static readonly DependencyProperty Line2HeightProperty = RegisterRoiProperty(nameof(Line2Height), 1.0);
        public static readonly DependencyProperty Line2WidthProperty = RegisterRoiProperty(nameof(Line2Width), 1.0);
        public static readonly DependencyProperty Line2PointCountProperty = RegisterRoiProperty(nameof(Line2PointCount), 2.0);

        // ==================== 命令依赖属性 ====================
        // View 不执行业务逻辑，所有交互通过命令转发给 ViewModel。

        /// <summary>ROI 拖放结束时的回调命令。参数为 <see cref="FindIntersectionRoiChangedArgs"/>。</summary>
        public static readonly DependencyProperty RoiChangedCommandProperty =
            DependencyProperty.Register(nameof(RoiChangedCommand), typeof(ICommand), typeof(HalconIntersectionView),
                new PropertyMetadata(null));

        /// <summary>鼠标在 Halcon 窗口移动时的回调命令。参数为 <see cref="HalconMouseInfoArgs"/>。</summary>
        public static readonly DependencyProperty MouseMoveCommandProperty =
            DependencyProperty.Register(nameof(MouseMoveCommand), typeof(ICommand), typeof(HalconIntersectionView),
                new PropertyMetadata(null));

        /// <summary>右键按下时的回调命令。参数为 <see cref="HalconMouseInfoArgs"/>。</summary>
        public static readonly DependencyProperty RightMouseDownCommandProperty =
            DependencyProperty.Register(nameof(RightMouseDownCommand), typeof(ICommand), typeof(HalconIntersectionView),
                new PropertyMetadata(null));

        /// <summary>右键松开时的回调命令。参数为 <see cref="HalconMouseInfoArgs"/>。</summary>
        public static readonly DependencyProperty RightMouseUpCommandProperty =
            DependencyProperty.Register(nameof(RightMouseUpCommand), typeof(ICommand), typeof(HalconIntersectionView),
                new PropertyMetadata(null));

        // ==================== 内部字段 ====================
        private readonly FindIntersectionOverlayPainter overlayPainter = new FindIntersectionOverlayPainter(); // 叠加层绘制器
        private HWindowControl halconWindow;               // Halcon 原生图像控件
        private HalconViewRoiAdapter display;              // 封装了 HWndCtrl 和 ROIController 的适配器
        private ROIRectangle2 line1Roi;                    // 第一条直线 ROI 对象
        private ROIRectangle2 line2Roi;                    // 第二条直线 ROI 对象
        private bool line1Created;                         // 第一条 ROI 是否已添加到控制器
        private bool line2Created;                         // 第二条 ROI 是否已添加到控制器
        private bool syncingRoi;                           // 防止 ROI 同步循环（程序改 ROI 时忽略用户事件）
        private bool disposed;                             // 是否已释放资源
        private WindowsFormsHost host;                     // 承载 Halcon WinForms 控件的宿主

        // ==================== 构造函数 ====================
        public HalconIntersectionView()
        {
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        // ==================== 属性包装器 ====================
        public HObject Image { get { return (HObject)GetValue(ImageProperty); } set { SetValue(ImageProperty, value); } }
        public ParamsFindIntersection OverlayParams { get { return (ParamsFindIntersection)GetValue(OverlayParamsProperty); } set { SetValue(OverlayParamsProperty, value); } }
        public int DisplayRevision { get { return (int)GetValue(DisplayRevisionProperty); } set { SetValue(DisplayRevisionProperty, value); } }
        public bool FitOnNextDisplay { get { return (bool)GetValue(FitOnNextDisplayProperty); } set { SetValue(FitOnNextDisplayProperty, value); } }
        public bool CrosshairEnabled { get { return (bool)GetValue(CrosshairEnabledProperty); } set { SetValue(CrosshairEnabledProperty, value); } }
        public double CrossRow { get { return (double)GetValue(CrossRowProperty); } set { SetValue(CrossRowProperty, value); } }
        public double CrossCol { get { return (double)GetValue(CrossColProperty); } set { SetValue(CrossColProperty, value); } }

        public double Line1CenterRow { get { return (double)GetValue(Line1CenterRowProperty); } set { SetValue(Line1CenterRowProperty, value); } }
        public double Line1CenterColumn { get { return (double)GetValue(Line1CenterColumnProperty); } set { SetValue(Line1CenterColumnProperty, value); } }
        public double Line1Angle { get { return (double)GetValue(Line1AngleProperty); } set { SetValue(Line1AngleProperty, value); } }
        public double Line1Height { get { return (double)GetValue(Line1HeightProperty); } set { SetValue(Line1HeightProperty, value); } }
        public double Line1Width { get { return (double)GetValue(Line1WidthProperty); } set { SetValue(Line1WidthProperty, value); } }
        public double Line1PointCount { get { return (double)GetValue(Line1PointCountProperty); } set { SetValue(Line1PointCountProperty, value); } }
        public double Line2CenterRow { get { return (double)GetValue(Line2CenterRowProperty); } set { SetValue(Line2CenterRowProperty, value); } }
        public double Line2CenterColumn { get { return (double)GetValue(Line2CenterColumnProperty); } set { SetValue(Line2CenterColumnProperty, value); } }
        public double Line2Angle { get { return (double)GetValue(Line2AngleProperty); } set { SetValue(Line2AngleProperty, value); } }
        public double Line2Height { get { return (double)GetValue(Line2HeightProperty); } set { SetValue(Line2HeightProperty, value); } }
        public double Line2Width { get { return (double)GetValue(Line2WidthProperty); } set { SetValue(Line2WidthProperty, value); } }
        public double Line2PointCount { get { return (double)GetValue(Line2PointCountProperty); } set { SetValue(Line2PointCountProperty, value); } }

        public ICommand RoiChangedCommand { get { return (ICommand)GetValue(RoiChangedCommandProperty); } set { SetValue(RoiChangedCommandProperty, value); } }
        public ICommand MouseMoveCommand { get { return (ICommand)GetValue(MouseMoveCommandProperty); } set { SetValue(MouseMoveCommandProperty, value); } }
        public ICommand RightMouseDownCommand { get { return (ICommand)GetValue(RightMouseDownCommandProperty); } set { SetValue(RightMouseDownCommandProperty, value); } }
        public ICommand RightMouseUpCommand { get { return (ICommand)GetValue(RightMouseUpCommandProperty); } set { SetValue(RightMouseUpCommandProperty, value); } }

        // ==================== IDisposable 实现 ====================
        public void Dispose()
        {
            DisposeDisplay();
            disposed = true;
        }

        // ==================== 依赖属性注册辅助 ====================

        /// <summary>
        /// 注册 ROI 双向依赖属性，默认值为 0，变更回调为 <see cref="OnRoiPropertyChanged"/>。
        /// </summary>
        private static DependencyProperty RegisterRoiProperty(string propertyName, double defaultValue = 0.0)
        {
            return DependencyProperty.Register(
                propertyName,
                typeof(double),
                typeof(HalconIntersectionView),
                new FrameworkPropertyMetadata(defaultValue, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnRoiPropertyChanged));
        }

        /// <summary>
        /// 显示相关属性变化时，刷新整个显示（图像、ROI、叠加层）。
        /// </summary>
        private static void OnDisplayPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((HalconIntersectionView)d).RefreshDisplay();
        }

        /// <summary>
        /// 任意 ROI 几何参数变化时，将属性值应用到内部 ROI 对象并重绘。
        /// </summary>
        private static void OnRoiPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((HalconIntersectionView)d).ApplyBoundValuesToRoi();
        }

        // ==================== 生命周期事件 ====================
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            disposed = false;
            EnsureDisplay();
            RefreshDisplay();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            DisposeDisplay();
        }

        // ==================== 初始化与显示 ====================

        /// <summary>
        /// 创建 Halcon 控件、ViewROI 适配器以及两条 ROI 对象，
        /// 仅当尚未创建且未释放时执行。
        /// </summary>
        private void EnsureDisplay()
        {
            if (display != null || disposed) return;

            // 创建 Halcon 原生控件并嵌入 WindowsFormsHost
            halconWindow = new HWindowControl { Dock = Forms.DockStyle.Fill, BackColor = System.Drawing.Color.FromArgb(1, 3, 8) };
            host = new WindowsFormsHost { Child = halconWindow };
            Content = host;

            // 初始化 ViewROI 适配器（封装 HWndCtrl + ROIController）
            display = new HalconViewRoiAdapter(halconWindow, true);
            display.Redraw += DrawOverlay;           // 每次重绘时绘制叠加层
            display.RoiChanged += OnViewRoiChanged; // 监听 ROI 变更

            // 订阅 Halcon 原生鼠标事件
            halconWindow.DoubleClick += OnHalconDoubleClick;
            halconWindow.HMouseMove += OnHalconMouseMove;
            halconWindow.HMouseDown += OnHalconMouseDown;
            halconWindow.HMouseUp += OnHalconMouseUp;

            // 创建两条直线 ROI 对象
            line1Roi = new ROIRectangle2 { ROIName = "Line1" };
            line2Roi = new ROIRectangle2 { ROIName = "Line2" };
        }

        /// <summary>
        /// 刷新显示：显示当前图像，应用 ROI 参数，并重绘叠加层。
        /// </summary>
        private void RefreshDisplay()
        {
            if (display == null || disposed) return;
            if (!HasValidImage(Image)) return;

            display.ShowImage(Image, FitOnNextDisplay);
            ApplyBoundValuesToRoi();
            display.Repaint();
        }

        /// <summary>
        /// 将依赖属性中的 ROI 几何参数写入内部的 ROIRectangle2 对象，
        /// 并首次创建时添加到 ROI 控制器。
        /// </summary>
        private void ApplyBoundValuesToRoi()
        {
            if (display == null || syncingRoi || disposed) return;

            syncingRoi = true;
            try
            {
                ApplyLineRoi(line1Roi, ref line1Created, Line1CenterRow, Line1CenterColumn, Line1Angle, Line1Height, Line1Width, Line1PointCount);
                ApplyLineRoi(line2Roi, ref line2Created, Line2CenterRow, Line2CenterColumn, Line2Angle, Line2Height, Line2Width, Line2PointCount);
                display.RoiController.activeROIidx = -1; // 清除激活，避免高亮
                display.Repaint();
            }
            finally
            {
                syncingRoi = false;
            }
        }

        /// <summary>
        /// 将参数值设置到指定的 ROIRectangle2 对象中。
        /// 若该 ROI 尚未添加到控制器，则在此完成初始创建。
        /// </summary>
        private void ApplyLineRoi(ROIRectangle2 roi, ref bool created, double row, double column, double angleDegree, double height, double width, double pointCount)
        {
            if (roi == null) return;

            roi.PointNum = Math.Max(2, (int)Math.Round(pointCount));
            roi.midR = row;
            roi.midC = column;
            HOperatorSet.TupleRad(angleDegree, out HTuple phi);
            roi.phi = phi.D;
            roi.length1 = Math.Max(1, height) / 2.0;   // length1 是高度的一半
            roi.length2 = Math.Max(1, width) / 2.0;    // length2 是宽度的一半

            // 首次创建时需要将 ROI 对象通过 setROIShape 注册并添加到列表
            if (!created)
            {
                created = true;
                display.RoiController.setROIShape(roi);
                display.RoiController.roiMode.createROI(roi.midC, roi.midR);
                display.RoiController.ROIList.Add(display.RoiController.roiMode);
                display.RoiController.resetROI();
            }

            roi.updateHandlePos(); // 更新操作手柄位置
        }

        // ==================== ROI 交互事件 ====================

        /// <summary>
        /// 当用户拖拽 ROI 手柄或结束拖拽时，ViewROI 触发此回调。
        /// 仅在拖拽结束（EVENT_UPDATE_ROI）时才将 ROI 值推回依赖属性，
        /// 并执行 RoiChangedCommand 通知 ViewModel。
        /// </summary>
        private void OnViewRoiChanged(int eventCode)
        {
            if (syncingRoi) return;

            // 拖动过程中不处理，避免反复重绘打断交互
            if (eventCode != ROIController.EVENT_UPDATE_ROI)
                return;

            syncingRoi = true;
            try
            {
                // 将内部 ROI 对象的最新值写入依赖属性（自动同步到 ViewModel）
                PushRoiValuesToBindings();
            }
            finally
            {
                syncingRoi = false;
            }

            // 构造参数并执行命令
            FindIntersectionRoiChangedArgs args = CreateRoiArgs(eventCode);
            ExecuteCommand(RoiChangedCommand, args);
            display?.Repaint();
        }

        /// <summary>
        /// 读取内部 ROI 对象的几何参数并更新对应的依赖属性。
        /// 由于依赖属性为双向绑定，这些新值会立即反映到 ViewModel。
        /// </summary>
        private void PushRoiValuesToBindings()
        {
            if (line1Roi != null)
            {
                SetCurrentValue(Line1CenterRowProperty, line1Roi.midR);
                SetCurrentValue(Line1CenterColumnProperty, line1Roi.midC);
                SetCurrentValue(Line1AngleProperty, ToDegree(line1Roi.phi));
                SetCurrentValue(Line1HeightProperty, line1Roi.length1 * 2.0);
                SetCurrentValue(Line1WidthProperty, line1Roi.length2 * 2.0);
                SetCurrentValue(Line1PointCountProperty, (double)line1Roi.PointNum);
            }

            if (line2Roi != null)
            {
                SetCurrentValue(Line2CenterRowProperty, line2Roi.midR);
                SetCurrentValue(Line2CenterColumnProperty, line2Roi.midC);
                SetCurrentValue(Line2AngleProperty, ToDegree(line2Roi.phi));
                SetCurrentValue(Line2HeightProperty, line2Roi.length1 * 2.0);
                SetCurrentValue(Line2WidthProperty, line2Roi.length2 * 2.0);
                SetCurrentValue(Line2PointCountProperty, (double)line2Roi.PointNum);
            }
        }

        /// <summary>
        /// 封装 ROI 变更事件的参数，包含两条线的当前状态和激活角度。
        /// </summary>
        private FindIntersectionRoiChangedArgs CreateRoiArgs(int eventCode)
        {
            return new FindIntersectionRoiChangedArgs
            {
                EventCode = eventCode,
                Line1 = CreateLineArgs(line1Roi),
                Line2 = CreateLineArgs(line2Roi),
                ActiveAngleDegree = GetActiveAngleDegree()
            };
        }

        private static FindIntersectionLineRoiArgs CreateLineArgs(ROIRectangle2 roi)
        {
            if (roi == null) return new FindIntersectionLineRoiArgs();
            return new FindIntersectionLineRoiArgs
            {
                Row = roi.midR,
                Column = roi.midC,
                AngleDegree = ToDegree(roi.phi),
                Height = roi.length1 * 2.0,
                Width = roi.length2 * 2.0,
                PointCount = roi.PointNum
            };
        }

        /// <summary>
        /// 获取当前激活 ROI 的角度（度），默认取第一条线。
        /// </summary>
        private double GetActiveAngleDegree()
        {
            ROIRectangle2 roi = display != null && display.RoiController.activeROIidx == 1 ? line2Roi : line1Roi;
            return roi == null ? 0 : ToDegree(roi.phi);
        }

        private static double ToDegree(double rad)
        {
            HOperatorSet.TupleDeg(rad, out HTuple deg);
            return deg.D;
        }

        // ==================== 叠加层绘制 ====================

        /// <summary>
     /// 在 Halcon 窗口上绘制交点、直线、十字光标等叠加层。
        /// </summary>
        private void DrawOverlay()
        {
            if (halconWindow == null) return;
            overlayPainter.Draw(halconWindow.HalconWindow, OverlayParams, CrossRow, CrossCol, CrosshairEnabled, 8);
        }

        // ==================== 鼠标事件转发 ====================

        private void OnHalconDoubleClick(object sender, EventArgs e)
        {
            display?.FitImage();
        }

        private void OnHalconMouseMove(object sender, HMouseEventArgs e)
        {
            string text = display == null ? "Row:-  Col:-  Gray:-" : display.GetMouseText(e);
            ExecuteCommand(MouseMoveCommand, new HalconMouseInfoArgs(e.Y, e.X, text));
        }

        private void OnHalconMouseDown(object sender, HMouseEventArgs e)
        {
            if (e.Button != Forms.MouseButtons.Right) return;
            ExecuteCommand(RightMouseDownCommand, new HalconMouseInfoArgs(e.Y, e.X, display == null ? string.Empty : display.GetMouseText(e)));
        }

        private void OnHalconMouseUp(object sender, HMouseEventArgs e)
        {
            if (e.Button != Forms.MouseButtons.Right) return;
            ExecuteCommand(RightMouseUpCommand, new HalconMouseInfoArgs(e.Y, e.X, display == null ? string.Empty : display.GetMouseText(e)));
        }

        /// <summary>
        /// 安全执行指定的 ICommand，如果命令不可用则跳过。
        /// </summary>
        private static void ExecuteCommand(ICommand command, object parameter)
        {
            if (command != null && command.CanExecute(parameter))
                command.Execute(parameter);
        }

        // ==================== 工具方法 ====================

        /// <summary>
        /// 判断 Halcon 图像对象是否有效（已初始化、非空、为 image 类型）。
        /// </summary>
        private static bool HasValidImage(HObject image)
        {
            if (image == null || !image.IsInitialized()) return false;

            try
            {
                HOperatorSet.CountObj(image, out HTuple count);
                if (count == null || count.Length <= 0 || count.I <= 0) return false;

                HOperatorSet.GetObjClass(image, out HTuple objClass);
                if (objClass == null || objClass.Length <= 0) return false;

                return string.Equals(objClass.S, "image", StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        // ==================== 资源释放 ====================

        /// <summary>
        /// 释放所有 Halcon/ViewROI 资源，取消事件订阅。
        /// </summary>
        private void DisposeDisplay()
        {
            if (display == null && halconWindow == null) return;

            if (display != null)
            {
                display.Redraw -= DrawOverlay;
                display.RoiChanged -= OnViewRoiChanged;
                display.Dispose();
                display = null;
            }

            if (halconWindow != null)
            {
                halconWindow.DoubleClick -= OnHalconDoubleClick;
                halconWindow.HMouseMove -= OnHalconMouseMove;
                halconWindow.HMouseDown -= OnHalconMouseDown;
                halconWindow.HMouseUp -= OnHalconMouseUp;
                halconWindow.Dispose();
                halconWindow = null;
            }

            if (host != null)
            {
                host.Child = null;
                host.Dispose();
                host = null;
            }

            // 清除依赖属性值的引用，辅助 GC 回收
            ClearValue(ImageProperty);
            ClearValue(OverlayParamsProperty);
            ClearValue(RoiChangedCommandProperty);
            ClearValue(MouseMoveCommandProperty);
            ClearValue(RightMouseDownCommandProperty);
            ClearValue(RightMouseUpCommandProperty);
            Content = null;
            line1Roi = null;
            line2Roi = null;
            line1Created = false;
            line2Created = false;
        }
    }

    // ==================== 命令参数类（简单的数据载体） ====================

    /// <summary>鼠标事件信息（行、列、格式化文本）。</summary>
    public sealed class HalconMouseInfoArgs
    {
        public HalconMouseInfoArgs(double row, double column, string text)
        {
            Row = row;
            Column = column;
            Text = text ?? string.Empty;
        }

        public double Row { get; private set; }
        public double Column { get; private set; }
        public string Text { get; private set; }
    }

    /// <summary>ROI 变更事件的参数。</summary>
    public sealed class FindIntersectionRoiChangedArgs
    {
        public int EventCode { get; set; }
        public FindIntersectionLineRoiArgs Line1 { get; set; }
        public FindIntersectionLineRoiArgs Line2 { get; set; }
        public double ActiveAngleDegree { get; set; }
    }

    /// <summary>单条直线 ROI 的几何参数。</summary>
    public sealed class FindIntersectionLineRoiArgs
    {
        public double Row { get; set; }
        public double Column { get; set; }
        public double AngleDegree { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }
        public int PointCount { get; set; }
    }
}
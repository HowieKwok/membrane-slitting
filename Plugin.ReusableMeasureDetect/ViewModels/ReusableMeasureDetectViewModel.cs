using Plugin.ReusableMeasureDetect.PointFilters.ViewModels;
using Plugin.ReusableMeasureDetect.Core;
using Plugin.ReusableMeasureDetect.Services;
using ViewROI;
using Frame_Core;
using Frame_Core.pluginParams;
using HalconDotNet;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;

namespace Plugin.ReusableMeasureDetect.ViewModels
{
    /// <summary>
    /// 可复用测量检测插件主 ViewModel。
    /// 负责 MVVM 参数绑定、命令处理、HALCON/ROI 适配、图片仿真加载、位置跟随交互、运行结果表格和叠加绘制。
    /// </summary>
    public sealed class ReusableMeasureDetectViewModel : BindableBase, IPluginWpfView
    {
        private IPlugin pluginObject;
        private ParamsReusableMeasureDetect pluginParams;
        private readonly HWindowControl halconWindow = new HWindowControl();
        private HalconViewRoiAdapter roiAdapter;
        private bool closed;
        private string statusText = "等待运行 / Ready";
        private string statusBarText = "耗时=-- ms | 鼠标 Row=-- Col=-- Gray=-- | 图像=-- | 原始=0 滤波=0 | 状态=--";
        private double mouseRow = double.NaN;
        private double mouseCol = double.NaN;
        private string mouseGray = "--";
        private int imageWidth;
        private int imageHeight;
        private double menuRow = double.NaN;
        private double menuCol = double.NaN;
        private double activeRoiAngle;
        private bool isFollowMenuOpen;
        private string followOffsetText = "跟随偏差：X=--  Y=--  R=--";

        /// <summary>
        /// 初始化命令和滤波编辑器事件。
        /// </summary>
        public ReusableMeasureDetectViewModel()
        {
            FilterEditor.DisplayInvalidated += RenderOverlay;
            LoadImageCommand = new RelayCommand<object>(_ => LoadImage());
            RunCommand = new RelayCommand<object>(_ => RunFromHost());
            DemoPointsCommand = new RelayCommand<object>(_ => GenerateDemoPointsAndRun());
            FitWindowCommand = new RelayCommand<object>(_ => FitWindow());
            ResetRoiFromPointsCommand = new RelayCommand<object>(_ => ResetRoiFromPoints());
            SetBasePoseCommand = new RelayCommand<object>(_ => SetBasePose());
            SetCurrentPoseCommand = new RelayCommand<object>(_ => SetCurrentPose());
        }

        /// <summary>
        /// 点集滤波编辑器 ViewModel，负责滤波 Pipeline 的增删改和参数绑定。
        /// </summary>
        public PointFilterEditorViewModel FilterEditor { get; } = new PointFilterEditorViewModel();
        public ICommand LoadImageCommand { get; }
        public ICommand RunCommand { get; }
        public ICommand DemoPointsCommand { get; }
        public ICommand FitWindowCommand { get; }
        public ICommand ResetRoiFromPointsCommand { get; }
        public ICommand SetBasePoseCommand { get; }
        public ICommand SetCurrentPoseCommand { get; }
        /// <summary>
        /// 当前绑定的插件对象。
        /// </summary>
        public IPlugin PluginObject { get { return pluginObject; } }

        /// <summary>
        /// 当前插件参数对象。
        /// </summary>
        public PluginParamsBase PluginParams { get { return pluginParams; } }

        /// <summary>
        /// 主 HALCON 显示窗口，由 WPF 视图放入 WindowsFormsHost。
        /// </summary>
        public HWindowControl MainHalconWindow { get { return halconWindow; } }

        /// <summary>
        /// 原始点、滤波点、凸点结果、突变结果和汇总结果表格数据源。
        /// </summary>
        public ObservableCollection<PointResultRow> RawPointRows { get; } = new ObservableCollection<PointResultRow>();
        public ObservableCollection<PointResultRow> FilteredPointRows { get; } = new ObservableCollection<PointResultRow>();
        public ObservableCollection<KeyValueResultRow> ConvexResultRows { get; } = new ObservableCollection<KeyValueResultRow>();
        public ObservableCollection<KeyValueResultRow> JumpResultRows { get; } = new ObservableCollection<KeyValueResultRow>();
        public ObservableCollection<KeyValueResultRow> SummaryRows { get; } = new ObservableCollection<KeyValueResultRow>();

        public string StatusText
        {
            get { return statusText; }
            private set { SetProperty(ref statusText, value); }
        }

        public string StatusBarText
        {
            get { return statusBarText; }
            private set { SetProperty(ref statusBarText, value); }
        }

        public string FollowOffsetText
        {
            get { return followOffsetText; }
            private set { SetProperty(ref followOffsetText, value); }
        }

        public bool IsFollowMenuOpen
        {
            get { return isFollowMenuOpen; }
            set { SetProperty(ref isFollowMenuOpen, value); }
        }

        // 基础运行和 MeasureLine 抓边参数：全部通过 GetParamValue/SetParamValue 直接绑定到 ParamsReusableMeasureDetect。
        public bool EnablePlugin
        {
            get { return GetParamValue(parameters => parameters.m_bIsEnable, true); }
            set { SetParamValue((parameters, newValue) => parameters.m_bIsEnable = newValue, value, nameof(EnablePlugin)); }
        }

        public bool EnableMeasureGrab
        {
            get { return GetParamValue(parameters => parameters.EnableMeasureGrab, true); }
            set { SetParamValue((parameters, newValue) => parameters.EnableMeasureGrab = newValue, value, nameof(EnableMeasureGrab)); }
        }

        public double MeasureSigma
        {
            get { return GetParamValue(parameters => parameters.MeasureSigma, 2.0); }
            set { SetParamValue((parameters, newValue) => parameters.MeasureSigma = Math.Max(0.1, newValue), value, nameof(MeasureSigma)); }
        }

        public double MeasureThreshold
        {
            get { return GetParamValue(parameters => parameters.MeasureThreshold, 10.0); }
            set { SetParamValue((parameters, newValue) => parameters.MeasureThreshold = Math.Max(0.0, newValue), value, nameof(MeasureThreshold)); }
        }

        public int MeasureNumInstances
        {
            get { return GetParamValue(parameters => parameters.MeasureNumInstances, 1); }
            set { SetParamValue((parameters, newValue) => parameters.MeasureNumInstances = Math.Max(1, newValue), value, nameof(MeasureNumInstances)); }
        }

        public double MeasureDistThreshold
        {
            get { return GetParamValue(parameters => parameters.MeasureDistThreshold, 3.5); }
            set { SetParamValue((parameters, newValue) => parameters.MeasureDistThreshold = Math.Max(0.0, newValue), value, nameof(MeasureDistThreshold)); }
        }

        public string MeasureTransition
        {
            get { return GetParamValue(parameters => parameters.MeasureTransition, "all"); }
            set { SetParamValue((parameters, newValue) => parameters.MeasureTransition = NormalizeTransition(newValue), value, nameof(MeasureTransition)); }
        }

        public string MeasureSelect
        {
            get { return GetParamValue(parameters => parameters.MeasureSelect, "all"); }
            set { SetParamValue((parameters, newValue) => parameters.MeasureSelect = NormalizeSelect(newValue), value, nameof(MeasureSelect)); }
        }

        public double MeasureMinScore
        {
            get { return GetParamValue(parameters => parameters.MeasureMinScore, 0.7); }
            set { SetParamValue((parameters, newValue) => parameters.MeasureMinScore = Clamp01(newValue), value, nameof(MeasureMinScore)); }
        }

        public int MaxNumIterations
        {
            get { return GetParamValue(parameters => parameters.MaxNumIterations, -1); }
            set { SetParamValue((parameters, newValue) => parameters.MaxNumIterations = newValue, value, nameof(MaxNumIterations)); }
        }

        public string MeasureInterpolation
        {
            get { return GetParamValue(parameters => parameters.MeasureInterpolation, "bilinear"); }
            set { SetParamValue((parameters, newValue) => parameters.MeasureInterpolation = NormalizeInterpolation(newValue), value, nameof(MeasureInterpolation)); }
        }

        public string MeasureIORegions
        {
            get { return GetParamValue(parameters => parameters.MeasureIORegions, "false"); }
            set { SetParamValue((parameters, newValue) => parameters.MeasureIORegions = NormalizeBoolString(newValue), value, nameof(MeasureIORegions)); }
        }

        public double ScaleMean
        {
            get { return GetParamValue(parameters => parameters.ScaleMean, 0.0); }
            set { SetParamValue((parameters, newValue) => parameters.ScaleMean = Math.Max(0.0, newValue), value, nameof(ScaleMean)); }
        }

        public bool EnablePointMedianReject
        {
            get { return GetParamValue(parameters => parameters.EnablePointMedianReject, true); }
            set { SetParamValue((parameters, newValue) => parameters.EnablePointMedianReject = newValue, value, nameof(EnablePointMedianReject)); }
        }

        public double PointMedianRangeFactor
        {
            get { return GetParamValue(parameters => parameters.PointMedianRangeFactor, 2.0); }
            set { SetParamValue((parameters, newValue) => parameters.PointMedianRangeFactor = Math.Max(0.1, newValue), value, nameof(PointMedianRangeFactor)); }
        }

        // 突变检测参数：控制是否检测、点集排序方向、主副边拟合点数、角度阈值和拟合误差阈值。
        public bool EnableJumpDetection
        {
            get { return GetParamValue(parameters => parameters.EnableJumpDetection, true); }
            set { SetParamValue((parameters, newValue) => parameters.EnableJumpDetection = newValue, value, nameof(EnableJumpDetection)); RenderOverlay(); }
        }

        public bool EnableJumpDebugDisplay
        {
            get { return GetParamValue(parameters => parameters.EnableJumpDebugDisplay, true); }
            set { SetParamValue((parameters, newValue) => parameters.EnableJumpDebugDisplay = newValue, value, nameof(EnableJumpDebugDisplay)); RenderOverlay(); }
        }

        public bool EnableConvexPeakDetection
        {
            get { return GetParamValue(parameters => parameters.EnableConvexPeakDetection, true); }
            set { SetParamValue((parameters, newValue) => parameters.EnableConvexPeakDetection = newValue, value, nameof(EnableConvexPeakDetection)); RenderOverlay(); }
        }

        public bool EnableCornerPeakDetection
        {
            get { return GetParamValue(parameters => parameters.EnableCornerPeakDetection, true); }
            set { SetParamValue((parameters, newValue) => parameters.EnableCornerPeakDetection = newValue, value, nameof(EnableCornerPeakDetection)); RenderOverlay(); }
        }

        public string ConvexDetectionMethod
        {
            get { return EnableCornerPeakDetection ? "CornerPeak" : "None"; }
            set
            {
                EnableCornerPeakDetection = string.Equals(value, "CornerPeak", StringComparison.OrdinalIgnoreCase);
                OnPropertyChanged(nameof(ConvexDetectionMethod));
            }
        }

        public string JumpMode
        {
            get { return GetParamValue(parameters => parameters.JumpMode, "Row"); }
            set { SetParamValue((parameters, newValue) => parameters.JumpMode = NormalizeJumpMode(newValue), value, nameof(JumpMode)); }
        }

        public double RowJumpThreshold
        {
            get { return GetParamValue(parameters => parameters.RowJumpThreshold, 5.0); }
            set { SetParamValue((parameters, newValue) => parameters.RowJumpThreshold = Math.Max(0.0, newValue), value, nameof(RowJumpThreshold)); }
        }

        public double ColJumpThreshold
        {
            get { return GetParamValue(parameters => parameters.ColJumpThreshold, 5.0); }
            set { SetParamValue((parameters, newValue) => parameters.ColJumpThreshold = Math.Max(0.0, newValue), value, nameof(ColJumpThreshold)); }
        }

        public int MainMaxFitPoints
        {
            get { return GetParamValue(parameters => parameters.MainMaxFitPoints, 20); }
            set { SetParamValue((parameters, newValue) => parameters.MainMaxFitPoints = Math.Max(2, newValue), value, nameof(MainMaxFitPoints)); }
        }

        public int SecondaryMaxFitPoints
        {
            get { return GetParamValue(parameters => parameters.SecondaryMaxFitPoints, 20); }
            set { SetParamValue((parameters, newValue) => parameters.SecondaryMaxFitPoints = Math.Max(2, newValue), value, nameof(SecondaryMaxFitPoints)); }
        }

        public int MinFitPoints
        {
            get { return GetParamValue(parameters => parameters.MinFitPoints, 3); }
            set { SetParamValue((parameters, newValue) => parameters.MinFitPoints = Math.Max(2, newValue), value, nameof(MinFitPoints)); }
        }

        public double MaxFitError
        {
            get { return GetParamValue(parameters => parameters.MaxFitError, 25.0); }
            set { SetParamValue((parameters, newValue) => parameters.MaxFitError = Math.Max(0.0, newValue), value, nameof(MaxFitError)); }
        }

        public double MinTurnAngleDeg
        {
            get { return GetParamValue(parameters => parameters.MinTurnAngleDeg, 30.0); }
            set { SetParamValue((parameters, newValue) => parameters.MinTurnAngleDeg = Clamp0To90(newValue), value, nameof(MinTurnAngleDeg)); }
        }

        public double RowMainAngleMinDeg
        {
            get { return GetParamValue(parameters => parameters.RowMainAngleMinDeg, 0.0); }
            set { SetParamValue((parameters, newValue) => parameters.RowMainAngleMinDeg = Clamp0To90(newValue), value, nameof(RowMainAngleMinDeg)); }
        }

        public double RowMainAngleMaxDeg
        {
            get { return GetParamValue(parameters => parameters.RowMainAngleMaxDeg, 10.0); }
            set { SetParamValue((parameters, newValue) => parameters.RowMainAngleMaxDeg = Clamp0To90(newValue), value, nameof(RowMainAngleMaxDeg)); }
        }

        public double ColMainAngleMinDeg
        {
            get { return GetParamValue(parameters => parameters.ColMainAngleMinDeg, 70.0); }
            set { SetParamValue((parameters, newValue) => parameters.ColMainAngleMinDeg = Clamp0To90(newValue), value, nameof(ColMainAngleMinDeg)); }
        }

        public double ColMainAngleMaxDeg
        {
            get { return GetParamValue(parameters => parameters.ColMainAngleMaxDeg, 90.0); }
            set { SetParamValue((parameters, newValue) => parameters.ColMainAngleMaxDeg = Clamp0To90(newValue), value, nameof(ColMainAngleMaxDeg)); }
        }

        // 凸点检测参数：传统角点凸点通过平滑窗口、支撑窗口、突出度、坡度差和综合得分筛选候选。
        public int ConvexSmoothWindow
        {
            get { return GetParamValue(parameters => parameters.ConvexSmoothWindow, 3); }
            set { SetParamValue((parameters, newValue) => parameters.ConvexSmoothWindow = Math.Max(1, newValue), value, nameof(ConvexSmoothWindow)); }
        }

        public int ConvexSupportWindow
        {
            get { return GetParamValue(parameters => parameters.ConvexSupportWindow, 4); }
            set { SetParamValue((parameters, newValue) => parameters.ConvexSupportWindow = Math.Max(2, newValue), value, nameof(ConvexSupportWindow)); }
        }

        public double ConvexMinPeakScore
        {
            get { return GetParamValue(parameters => parameters.ConvexMinPeakScore, 0.35); }
            set { SetParamValue((parameters, newValue) => parameters.ConvexMinPeakScore = Math.Max(0.0, newValue), value, nameof(ConvexMinPeakScore)); }
        }

        public double ConvexMinProminence
        {
            get { return GetParamValue(parameters => parameters.ConvexMinProminence, 0.25); }
            set { SetParamValue((parameters, newValue) => parameters.ConvexMinProminence = Math.Max(0.001, newValue), value, nameof(ConvexMinProminence)); }
        }

        public double ConvexMinTurnAngleDeg
        {
            get { return GetParamValue(parameters => parameters.ConvexMinTurnAngleDeg, 18.0); }
            set { SetParamValue((parameters, newValue) => parameters.ConvexMinTurnAngleDeg = Clamp0To90(newValue), value, nameof(ConvexMinTurnAngleDeg)); }
        }

        public double ConvexMinSlopeDelta
        {
            get { return GetParamValue(parameters => parameters.ConvexMinSlopeDelta, 0.35); }
            set { SetParamValue((parameters, newValue) => parameters.ConvexMinSlopeDelta = Math.Max(0.0, newValue), value, nameof(ConvexMinSlopeDelta)); }
        }

        public double ConvexOutlierRadius
        {
            get { return GetParamValue(parameters => parameters.ConvexOutlierRadius, 12.0); }
            set { SetParamValue((parameters, newValue) => parameters.ConvexOutlierRadius = Math.Max(0.0, newValue), value, nameof(ConvexOutlierRadius)); }
        }

        public int ConvexOutlierMinNeighbors
        {
            get { return GetParamValue(parameters => parameters.ConvexOutlierMinNeighbors, 2); }
            set { SetParamValue((parameters, newValue) => parameters.ConvexOutlierMinNeighbors = Math.Max(0, newValue), value, nameof(ConvexOutlierMinNeighbors)); }
        }

        public double ConvexMinPeakDistance
        {
            get { return GetParamValue(parameters => parameters.ConvexMinPeakDistance, 12.0); }
            set { SetParamValue((parameters, newValue) => parameters.ConvexMinPeakDistance = Math.Max(0.0, newValue), value, nameof(ConvexMinPeakDistance)); }
        }

        public double ConvexMaxGapRatio
        {
            get { return GetParamValue(parameters => parameters.ConvexMaxGapRatio, 2.4); }
            set { SetParamValue((parameters, newValue) => parameters.ConvexMaxGapRatio = Math.Max(1.0, newValue), value, nameof(ConvexMaxGapRatio)); }
        }

        public double ConvexMaxGapExtra
        {
            get { return GetParamValue(parameters => parameters.ConvexMaxGapExtra, 6.0); }
            set { SetParamValue((parameters, newValue) => parameters.ConvexMaxGapExtra = Math.Max(0.0, newValue), value, nameof(ConvexMaxGapExtra)); }
        }

        public int ConvexMinChainPoints
        {
            get { return GetParamValue(parameters => parameters.ConvexMinChainPoints, 6); }
            set { SetParamValue((parameters, newValue) => parameters.ConvexMinChainPoints = Math.Max(2, newValue), value, nameof(ConvexMinChainPoints)); }
        }


        // ROI 和位置跟随参数：界面 ROI、右键菜单位姿和 MeasureLine 抓边 ROI 共用这一组参数。
        public bool EnableRoi
        {
            get { return GetParamValue(parameters => parameters.EnableRoi, false); }
            set { SetParamValue((parameters, newValue) => parameters.EnableRoi = newValue, value, nameof(EnableRoi)); RenderOverlay(); }
        }

        public double RoiCenterRow
        {
            get { return GetParamValue(parameters => parameters.RoiCenterRow, 300.0); }
            set { SetParamValue((parameters, newValue) => parameters.RoiCenterRow = newValue, value, nameof(RoiCenterRow)); RenderOverlay(); }
        }

        public double RoiCenterCol
        {
            get { return GetParamValue(parameters => parameters.RoiCenterCol, 300.0); }
            set { SetParamValue((parameters, newValue) => parameters.RoiCenterCol = newValue, value, nameof(RoiCenterCol)); RenderOverlay(); }
        }

        public double RoiAngleDeg
        {
            get { return GetParamValue(parameters => parameters.RoiAngleDeg, 0.0); }
            set { SetParamValue((parameters, newValue) => parameters.RoiAngleDeg = ClampAngleMinus180To180(newValue), value, nameof(RoiAngleDeg)); RenderOverlay(); }
        }

        public double RoiHalfLength1
        {
            get { return GetParamValue(parameters => parameters.RoiHalfLength1, 180.0); }
            set
            {
                SetParamValue((parameters, newValue) => parameters.RoiHalfLength1 = Math.Max(1.0, newValue), value, nameof(RoiHalfLength1));
                OnPropertyChanged(nameof(RoiHeight));
                RenderOverlay();
            }
        }

        public double RoiHalfLength2
        {
            get { return GetParamValue(parameters => parameters.RoiHalfLength2, 80.0); }
            set
            {
                SetParamValue((parameters, newValue) => parameters.RoiHalfLength2 = Math.Max(1.0, newValue), value, nameof(RoiHalfLength2));
                OnPropertyChanged(nameof(RoiWidth));
                RenderOverlay();
            }
        }

        public double RoiHeight
        {
            get { return RoiHalfLength1 * 2.0; }
            set { RoiHalfLength1 = Math.Max(1.0, value * 0.5); }
        }

        public double RoiWidth
        {
            get { return RoiHalfLength2 * 2.0; }
            set { RoiHalfLength2 = Math.Max(1.0, value * 0.5); }
        }

        public int RoiPointNum
        {
            get { return GetParamValue(parameters => parameters.RoiPointNum, 20); }
            set { SetParamValue((parameters, newValue) => parameters.RoiPointNum = Math.Max(2, newValue), value, nameof(RoiPointNum)); RenderOverlay(); }
        }

        public bool EnablePositionFollow
        {
            get { return GetParamValue(parameters => parameters.m_bUsePositionFollow, false); }
            set
            {
                SetParamValue((parameters, newValue) => parameters.m_bUsePositionFollow = newValue, value, nameof(EnablePositionFollow));
                RefreshFollowOffsetText();
                RenderOverlay();
            }
        }

        public bool CurrentPoseValid
        {
            get { return GetParamValue(parameters => parameters.m_bCurrentPoseValid, false); }
            set { SetParamValue((parameters, newValue) => parameters.m_bCurrentPoseValid = newValue, value, nameof(CurrentPoseValid)); }
        }

        public double BaseRow
        {
            get { return GetParamValue(parameters => parameters.m_fBaseRow, 0.0); }
            set { SetParamValue((parameters, newValue) => parameters.m_fBaseRow = newValue, value, nameof(BaseRow)); RefreshFollowOffsetText(); RenderOverlay(); }
        }

        public double BaseCol
        {
            get { return GetParamValue(parameters => parameters.m_fBaseCol, 0.0); }
            set { SetParamValue((parameters, newValue) => parameters.m_fBaseCol = newValue, value, nameof(BaseCol)); RefreshFollowOffsetText(); RenderOverlay(); }
        }

        public double BaseAngle
        {
            get { return GetParamValue(parameters => parameters.m_fBaseAngle, 0.0); }
            set { SetParamValue((parameters, newValue) => parameters.m_fBaseAngle = ClampAngleMinus180To180(newValue), value, nameof(BaseAngle)); RefreshFollowOffsetText(); RenderOverlay(); }
        }

        public double CurrentRow
        {
            get { return GetParamValue(parameters => parameters.m_fCurrentRow, 0.0); }
            set { SetParamValue((parameters, newValue) => parameters.m_fCurrentRow = newValue, value, nameof(CurrentRow)); RefreshFollowOffsetText(); RenderOverlay(); }
        }

        public double CurrentCol
        {
            get { return GetParamValue(parameters => parameters.m_fCurrentCol, 0.0); }
            set { SetParamValue((parameters, newValue) => parameters.m_fCurrentCol = newValue, value, nameof(CurrentCol)); RefreshFollowOffsetText(); RenderOverlay(); }
        }

        public double CurrentAngle
        {
            get { return GetParamValue(parameters => parameters.m_fCurrentAngle, 0.0); }
            set { SetParamValue((parameters, newValue) => parameters.m_fCurrentAngle = ClampAngleMinus180To180(newValue), value, nameof(CurrentAngle)); RefreshFollowOffsetText(); RenderOverlay(); }
        }

        /// <summary>
        /// 绑定框架插件对象和参数对象，初始化滤波编辑器、结果表格和叠加显示。
        /// </summary>
        public void BindPlugin(IPlugin plugin)
        {
            pluginObject = plugin;
            pluginParams = plugin == null ? null : plugin.PluginParams as ParamsReusableMeasureDetect;
            if (pluginParams == null && plugin != null)
            {
                pluginParams = new ParamsReusableMeasureDetect();
                plugin.PluginParams = pluginParams;
            }
            EnsureParams();
            FilterEditor.Attach(pluginParams.FilterOptions, pluginParams.FilterOutput);
            RefreshBindings();
            RenderOverlay();
        }

        /// <summary>
        /// 从当前参数刷新所有界面绑定、状态文本、跟随偏差和结果表格。
        /// </summary>
        public void RefreshBindings()
        {
            EnsureParams();
            FilterEditor.Attach(pluginParams.FilterOptions, pluginParams.FilterOutput);
            StatusText = string.IsNullOrEmpty(pluginParams.m_strRunMessage) ? "等待运行 / Ready" : pluginParams.m_strRunMessage;
            RefreshFollowOffsetText();
            RefreshResultRows();
            UpdateStatusBarText();
            OnPropertyChanged(string.Empty);
        }

        /// <summary>
        /// 界面或框架触发运行时调用：先提交界面参数，再执行插件，然后刷新滤波步骤、结果和叠加层。
        /// </summary>
        public PluginViewActionResult RunFromHost()
        {
            if (pluginObject == null) return PluginViewActionResult.Fail("插件未绑定");
            CommitToParams();
            bool success = pluginObject.ExecuteObj();
            FilterEditor.RefreshFromPipelineOutput();
            StatusText = BuildStatusText();
            RefreshFollowOffsetText();
            RefreshResultRows();
            UpdateStatusBarText();
            RenderOverlay();
            long elapsedMs = pluginParams == null ? 0 : (long)pluginParams.m_fElapsedMs;
            string message = pluginParams == null ? string.Empty : pluginParams.m_strRunMessage;
            return success ? PluginViewActionResult.Ok(message, elapsedMs) : PluginViewActionResult.Fail(message, elapsedMs);
        }

        /// <summary>
        /// 保存当前参数到指定文件路径。
        /// </summary>
        public PluginViewActionResult SaveFromHost(string filePath)
        {
            if (pluginObject == null) return PluginViewActionResult.Fail("插件未绑定");
            CommitToParams();
            if (pluginObject.PluginParams != null) pluginObject.PluginParams.filePath = filePath;
            bool success = pluginObject.saveParams();
            StatusText = success ? "保存成功 / Saved" : "保存失败 / Save failed";
            UpdateStatusBarText();
            return success ? PluginViewActionResult.Ok("保存成功") : PluginViewActionResult.Fail("保存失败");
        }

        /// <summary>
        /// 打开文件对话框加载本地图像，用于插件界面仿真；加载后启用抓边并刷新显示。
        /// </summary>
        public void LoadImage()
        {
            EnsureParams();
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "图像文件|*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff|所有文件|*.*"
            };
            if (dialog.ShowDialog() != true) return;

            if (pluginParams.ho_Image != null) pluginParams.ho_Image.Dispose();
            HOperatorSet.ReadImage(out pluginParams.ho_Image, dialog.FileName);
            pluginParams.ImageSourceIndex = 0;
            pluginParams.EnableMeasureGrab = true;
            pluginParams.ReleaseOutputs();
            InitRoiAdapter();
            roiAdapter.RefreshFromParams(pluginParams, true);
            UpdateImageSizeFromParams();
            RefreshResultRows();
            UpdateStatusBarText();
            StatusText = "图片已加载：" + System.IO.Path.GetFileName(dialog.FileName);
            OnPropertyChanged(nameof(EnableMeasureGrab));
        }

        /// <summary>
        /// 关闭界面时释放 ROI 适配器和 HALCON 窗口。
        /// </summary>
        public void CloseFromHost()
        {
            if (closed) return;
            closed = true;
            if (roiAdapter != null)
            {
                roiAdapter.Dispose();
                roiAdapter = null;
            }
            halconWindow.Dispose();
        }

        /// <summary>
        /// 框架运行前回调，提交 ROI 和滤波参数。
        /// </summary>
        public void BeforeRun() { CommitToParams(); }

        /// <summary>
        /// 框架保存前回调，提交 ROI 和滤波参数。
        /// </summary>
        public void BeforeSave() { CommitToParams(); }

        /// <summary>
        /// 框架关闭前回调，释放界面资源。
        /// </summary>
        public void BeforeClose() { CloseFromHost(); }

        /// <summary>
        /// 框架保存后回调，更新保存状态文本。
        /// </summary>
        public void AfterSave(bool success) { StatusText = success ? "保存成功 / Saved" : "保存失败 / Save failed"; }

        /// <summary>
        /// 框架运行后回调，刷新结果表格、状态栏和图像叠加。
        /// </summary>
        public void AfterRun(bool success, long elapsedMs)
        {
            FilterEditor.RefreshFromPipelineOutput();
            StatusText = BuildStatusText();
            RefreshFollowOffsetText();
            RefreshResultRows();
            UpdateStatusBarText();
            RenderOverlay();
        }

        /// <summary>
        /// 插件被流程外部运行完成后回调，用外部执行后的参数刷新本界面。
        /// </summary>
        public void OnPluginExecutedFromOutside(PluginExecuteCompletedEventArgs e)
        {
            if (e != null && e.Param is ParamsReusableMeasureDetect executedParams)
                pluginParams = executedParams;
            EnsureParams();
            FilterEditor.Attach(pluginParams.FilterOptions, pluginParams.FilterOutput);
            StatusText = BuildStatusText();
            RefreshResultRows();
            UpdateStatusBarText();
            RenderOverlay();
        }

        /// <summary>
        /// 将界面状态提交回参数：ROI 从 ViewROI 同步，滤波 Pipeline 从 FilterEditor 同步。
        /// </summary>
        private void CommitToParams()
        {
            EnsureParams();
            if (roiAdapter != null && pluginParams.EnableRoi && roiAdapter.UpdateParamsFromActiveRoi(pluginParams))
                NotifyRoiProperties();
            FilterEditor.CommitToOptions();
            RefreshFollowOffsetText();
        }


        /// <summary>
        /// 延迟创建 HALCON/ViewROI 适配器，并绑定重绘、ROI、鼠标事件。
        /// </summary>
        private void InitRoiAdapter()
        {
            if (roiAdapter != null) return;
            roiAdapter = new HalconViewRoiAdapter(halconWindow);
            roiAdapter.DrawOverlayRequested += DrawOverlayObjects;
            roiAdapter.RoiChanged += OnRoiChanged;
            roiAdapter.MouseInfoChanged += OnMouseInfoChanged;
            roiAdapter.RightMouseDown += OnRightMouseDown;
            roiAdapter.RightMouseUp += OnRightMouseUp;
        }

        /// <summary>
        /// 鼠标移动时更新底部状态栏中的 Row、Col、Gray 和图像尺寸。
        /// </summary>
        private void OnMouseInfoChanged(ReusableMeasureMouseInfo info)
        {
            if (info == null) return;
            mouseRow = info.Row;
            mouseCol = info.Col;
            mouseGray = info.Gray;
            imageWidth = info.ImageWidth;
            imageHeight = info.ImageHeight;
            UpdateStatusBarText();
        }

        /// <summary>
        /// 右键按下时记录当前菜单坐标和 ROI 角度，用于设置基准/当前位姿。
        /// </summary>
        private void OnRightMouseDown(ReusableMeasureMouseInfo info)
        {
            if (info == null) return;
            menuRow = info.Row;
            menuCol = info.Col;
            activeRoiAngle = RoiAngleDeg;
            if (roiAdapter != null)
            {
                ROIRectangle2 roi = roiAdapter.GetActiveRectangleRoi();
                if (roi != null)
                {
                    HOperatorSet.TupleDeg(roi.phi, out HTuple deg);
                    activeRoiAngle = deg.D;
                }
            }
            IsFollowMenuOpen = true;
        }

        /// <summary>
        /// 右键抬起预留入口，当前不执行动作。
        /// </summary>
        private void OnRightMouseUp(ReusableMeasureMouseInfo info)
        {
        }

        /// <summary>
        /// 适应显示当前图像或点集范围。
        /// </summary>
        private void FitWindow()
        {
            EnsureParams();
            InitRoiAdapter();
            roiAdapter.RefreshFromParams(pluginParams, true);
            StatusText = "已适应显示 / Fit view";
        }

        /// <summary>
        /// 将当前右键坐标和 ROI 角度保存为位置跟随基准位姿。
        /// </summary>
        private void SetBasePose()
        {
            EnsureParams();
            UpdateMenuPoseFromRoiIfNeeded();
            pluginParams.m_fBaseRow = menuRow;
            pluginParams.m_fBaseCol = menuCol;
            pluginParams.m_fBaseAngle = activeRoiAngle;
            IsFollowMenuOpen = false;
            NotifyFollowProperties();
            RefreshFollowOffsetText();
            RenderOverlay();
        }

        /// <summary>
        /// 将当前右键坐标和 ROI 角度保存为位置跟随当前位姿，并立即重新运行检测。
        /// </summary>
        private void SetCurrentPose()
        {
            EnsureParams();
            UpdateMenuPoseFromRoiIfNeeded();
            pluginParams.m_fCurrentRow = menuRow;
            pluginParams.m_fCurrentCol = menuCol;
            pluginParams.m_fCurrentAngle = activeRoiAngle;
            pluginParams.m_bCurrentPoseValid = true;
            pluginParams.m_bUsePositionFollow = true;
            IsFollowMenuOpen = false;
            NotifyFollowProperties();
            RefreshFollowOffsetText();
            RunFromHost();
        }

        /// <summary>
        /// 如果右键坐标无效，则使用当前 ROI 中心作为菜单位姿坐标，并同步 ROI 角度。
        /// </summary>
        private void UpdateMenuPoseFromRoiIfNeeded()
        {
            if (roiAdapter == null) return;
            ROIRectangle2 roi = roiAdapter.GetActiveRectangleRoi();
            if (roi == null) return;
            if (double.IsNaN(menuRow) || double.IsNaN(menuCol))
            {
                menuRow = roi.midR;
                menuCol = roi.midC;
            }
            HOperatorSet.TupleDeg(roi.phi, out HTuple deg);
            activeRoiAngle = deg.D;
        }

        /// <summary>
        /// 根据当前点集包围盒重置 ROI，便于无图仿真或点集输入时快速得到有效抓边区域。
        /// </summary>
        private void ResetRoiFromPoints()
        {
            EnsureParams();
            List<MeasurePointParam> points = pluginParams.FilterOutput != null && pluginParams.FilterOutput.SelectedPoints != null && pluginParams.FilterOutput.SelectedPoints.Count > 0
                ? pluginParams.FilterOutput.SelectedPoints
                : pluginParams.InputPoints;
            if (points == null || points.Count == 0)
                points = Plugin.ReusableMeasureDetect.PointFilters.Core.PointFilterPipelineCore.BuildInputOrDemoPoints(pluginParams);

            double minRow = double.MaxValue;
            double minCol = double.MaxValue;
            double maxRow = double.MinValue;
            double maxCol = double.MinValue;
            for (int index = 0; index < points.Count; index++)
            {
                minRow = Math.Min(minRow, points[index].Row);
                minCol = Math.Min(minCol, points[index].Col);
                maxRow = Math.Max(maxRow, points[index].Row);
                maxCol = Math.Max(maxCol, points[index].Col);
            }

            if (minRow == double.MaxValue) return;
            pluginParams.RoiCenterRow = (minRow + maxRow) * 0.5;
            pluginParams.RoiCenterCol = (minCol + maxCol) * 0.5;
            pluginParams.RoiAngleDeg = 0.0;
            pluginParams.RoiHalfLength1 = Math.Max(20.0, (maxCol - minCol) * 0.5 + 20.0);
            pluginParams.RoiHalfLength2 = Math.Max(20.0, (maxRow - minRow) * 0.5 + 20.0);
            pluginParams.EnableRoi = true;
            NotifyRoiProperties();
            RenderOverlay();
        }

        /// <summary>
        /// ViewROI 发生变化时同步参数；拖动更新完成时自动运行以实现 ROI 位置跟随显示。
        /// </summary>
        private void OnRoiChanged(int eventCode)
        {
            if (pluginParams == null || roiAdapter == null) return;
            if (!roiAdapter.UpdateParamsFromActiveRoi(pluginParams)) return;
            NotifyRoiProperties();
            if (eventCode == ROIController.EVENT_UPDATE_ROI && pluginParams.EnableRoi && pluginObject != null)
            {
                RunFromHost();
                return;
            }
            RenderOverlay();
        }

        /// <summary>
        /// 通知所有 ROI 相关属性刷新，保证文本框、数值控件和显示同步。
        /// </summary>
        private void NotifyRoiProperties()
        {
            OnPropertyChanged(nameof(EnableRoi));
            OnPropertyChanged(nameof(RoiCenterRow));
            OnPropertyChanged(nameof(RoiCenterCol));
            OnPropertyChanged(nameof(RoiAngleDeg));
            OnPropertyChanged(nameof(RoiHalfLength1));
            OnPropertyChanged(nameof(RoiHalfLength2));
            OnPropertyChanged(nameof(RoiHeight));
            OnPropertyChanged(nameof(RoiWidth));
            OnPropertyChanged(nameof(RoiPointNum));
        }

        /// <summary>
        /// 通知所有位置跟随相关属性刷新。
        /// </summary>
        private void NotifyFollowProperties()
        {
            OnPropertyChanged(nameof(EnablePositionFollow));
            OnPropertyChanged(nameof(CurrentPoseValid));
            OnPropertyChanged(nameof(BaseRow));
            OnPropertyChanged(nameof(BaseCol));
            OnPropertyChanged(nameof(BaseAngle));
            OnPropertyChanged(nameof(CurrentRow));
            OnPropertyChanged(nameof(CurrentCol));
            OnPropertyChanged(nameof(CurrentAngle));
        }

        /// <summary>
        /// 根据基准位姿和当前位姿刷新跟随偏差文本，并把偏差写回参数。
        /// </summary>
        private void RefreshFollowOffsetText()
        {
            if (pluginParams == null)
            {
                FollowOffsetText = "跟随偏差：X=--  Y=--  R=--";
                return;
            }

            if (!pluginParams.m_bUsePositionFollow)
            {
                pluginParams.m_fFollowOffsetX = 0;
                pluginParams.m_fFollowOffsetY = 0;
                pluginParams.m_fFollowOffsetR = 0;
            }
            else
            {
                pluginParams.m_fFollowOffsetY = pluginParams.m_fCurrentRow - pluginParams.m_fBaseRow;
                pluginParams.m_fFollowOffsetX = pluginParams.m_fCurrentCol - pluginParams.m_fBaseCol;
                pluginParams.m_fFollowOffsetR = pluginParams.m_fCurrentAngle - pluginParams.m_fBaseAngle;
            }

            FollowOffsetText = string.Format(
                CultureInfo.InvariantCulture,
                "跟随偏差：X={0:F3}  Y={1:F3}  R={2:F3}°",
                pluginParams.m_fFollowOffsetX,
                pluginParams.m_fFollowOffsetY,
                pluginParams.m_fFollowOffsetR);
        }

        /// <summary>
        /// 从参数图像读取宽高，用于底部状态栏显示。
        /// </summary>
        private void UpdateImageSizeFromParams()
        {
            imageWidth = 0;
            imageHeight = 0;
            if (pluginParams == null || pluginParams.ho_Image == null || !pluginParams.ho_Image.IsInitialized()) return;
            try
            {
                HOperatorSet.GetImageSize(pluginParams.ho_Image, out HTuple width, out HTuple height);
                imageWidth = width.I;
                imageHeight = height.I;
            }
            catch
            {
            }
        }
        /// <summary>
        /// 生成演示直角点集并运行检测，用于无图情况下验证滤波、突变和凸点输出。
        /// </summary>
        private void GenerateDemoPointsAndRun()
        {
            EnsureParams();
            pluginParams.EnableMeasureGrab = false;
            pluginParams.InputPoints.Clear();
            for (int index = 0; index < 24; index++)
                pluginParams.InputPoints.Add(new MeasurePointParam(300, 120 + index * 16));
            pluginParams.InputPoints.Add(new MeasurePointParam(300, 500));
            for (int index = 1; index < 24; index++)
                pluginParams.InputPoints.Add(new MeasurePointParam(300 + index * 16, 500));
            StatusText = "已生成演示直角点：水平主边 + 垂直副边 / Demo corner points generated";
            OnPropertyChanged(nameof(EnableMeasureGrab));
            RunFromHost();
        }

        /// <summary>
        /// 确保插件参数、输出对象和滤波参数对象存在，并在必要时写回插件对象。
        /// </summary>
        private void EnsureParams()
        {
            if (pluginParams == null)
                pluginParams = new ParamsReusableMeasureDetect();
            pluginParams.EnsureOutputs();
            if (pluginParams.FilterOptions == null)
                pluginParams.FilterOptions = new PointFilterOptionsParam();
            if (pluginObject != null && pluginObject.PluginParams == null)
                pluginObject.PluginParams = pluginParams;
        }

        /// <summary>
        /// 读取参数值；参数对象为空时返回界面默认值。
        /// </summary>
        private T GetParamValue<T>(Func<ParamsReusableMeasureDetect, T> getter, T fallback)
        {
            return pluginParams == null ? fallback : getter(pluginParams);
        }

        /// <summary>
        /// 写入参数值并通知指定绑定属性刷新。
        /// </summary>
        private void SetParamValue<T>(Action<ParamsReusableMeasureDetect, T> setter, T value, string propertyName)
        {
            EnsureParams();
            setter(pluginParams, value);
            OnPropertyChanged(propertyName);
        }

        /// <summary>
        /// 刷新 HALCON 显示窗口、ROI 和叠加结果。
        /// </summary>
        private void RenderOverlay()
        {
            if (closed || pluginParams == null) return;
            try
            {
                InitRoiAdapter();
                roiAdapter.RefreshFromParams(pluginParams, false);
                StatusText = BuildStatusText();
                UpdateStatusBarText();
            }
            catch (Exception ex)
            {
                StatusText = "显示失败 / Display failed: " + ex.Message;
                UpdateStatusBarText();
            }
        }

        /// <summary>
        /// 在 ViewROI 重绘后绘制原始点、当前点集、突变主副边、凸点边和结果十字。
        /// </summary>
        private void DrawOverlayObjects(HWindow window)
        {
            if (window == null || pluginParams == null) return;
            pluginParams.EnsureOutputs();
            DrawFollowRoi(window);
            if (pluginParams.GrabOutput.ho_RawPointsCross != null && pluginParams.GrabOutput.ho_RawPointsCross.IsInitialized())
            {
                DispObject(window, pluginParams.GrabOutput.ho_RawPointsCross, "green", "原始抓边点 Raw", 20, 20);
            }
            else if (pluginParams.InputPoints != null && pluginParams.InputPoints.Count > 0)
            {
                HObject rawCross = null;
                try
                {
                    rawCross = Plugin.ReusableMeasureDetect.PointFilters.Core.PointFilterPipelineCore.BuildPointCross(pluginParams.InputPoints, 8.0);
                    DispObject(window, rawCross, "green", "原始输入点 Raw", 20, 20);
                }
                finally
                {
                    if (rawCross != null) rawCross.Dispose();
                }
            }

            if (pluginParams.FilterOptions.ShowSelectedPointSet)
            {
                double pointLabelRow;
                double pointLabelCol;
                GetPointCenter(pluginParams.FilterOutput.SelectedPoints, 20, 160, out pointLabelRow, out pointLabelCol);
                string pointLabel = string.IsNullOrEmpty(pluginParams.FilterOutput.SelectedStepName)
                    ? "当前点集 Selected PointSet"
                    : "当前点集 " + pluginParams.FilterOutput.SelectedStepName;
                DispObject(window, pluginParams.FilterOutput.ho_SelectedPointsCross, "cyan", pointLabel, pointLabelRow, pointLabelCol);
            }

            if (pluginParams.EnableJumpDetection && pluginParams.EnableJumpDebugDisplay)
            {
                double labelRow;
                double labelCol;
                GetEdgeLabelPosition(pluginParams.JumpOutput.MainEdge, 20, 20, out labelRow, out labelCol);
                DispObject(window, pluginParams.JumpOutput.ho_MainEdgeXld, "blue", "突变主边 Jump Main", labelRow, labelCol);
                GetEdgeLabelPosition(pluginParams.JumpOutput.SecondaryEdge, 45, 20, out labelRow, out labelCol);
                DispObject(window, pluginParams.JumpOutput.ho_SecondaryEdgeXld, "magenta", "突变副边 Jump Secondary", labelRow, labelCol);
                DispObject(window, pluginParams.JumpOutput.ho_IntersectionCross, "red", "突变交点 Jump Intersection", pluginParams.JumpOutput.IntersectionRow + 12, pluginParams.JumpOutput.IntersectionCol + 12);
            }

            if (!pluginParams.EnableJumpDetection && pluginParams.EnableConvexPeakDetection)
            {
                double labelRow;
                double labelCol;
                GetEdgeLabelPosition(pluginParams.ConvexOutput.MainEdge, 20, 20, out labelRow, out labelCol);
                DispObject(window, pluginParams.ConvexOutput.ho_MainEdgeXld, "blue", "凸点主边 Convex Main", labelRow, labelCol);
                GetEdgeLabelPosition(pluginParams.ConvexOutput.SecondaryEdge, 45, 20, out labelRow, out labelCol);
                DispObject(window, pluginParams.ConvexOutput.ho_SecondaryEdgeXld, "magenta", "凸点副边 Convex Secondary", labelRow, labelCol);
            }

            if (pluginParams.EnableConvexPeakDetection)
                DispObject(window, pluginParams.ConvexOutput.ho_PeakCross, "yellow", "凸点 Convex Peak", pluginParams.ConvexOutput.PeakRow + 12, pluginParams.ConvexOutput.PeakCol + 12);
        }

        /// <summary>
        /// 位置跟随启用时绘制绿色跟随后的运行 ROI，和可编辑原始 ROI 区分显示。
        /// </summary>
        private void DrawFollowRoi(HWindow window)
        {
            if (pluginParams == null || !pluginParams.m_bUsePositionFollow || !pluginParams.m_bCurrentPoseValid) return;
            ReusableMeasureRoiRect2 followed = new ReusableMeasureRoiFollower(pluginParams, false).Transform(
                pluginParams.RoiCenterRow,
                pluginParams.RoiCenterCol,
                pluginParams.RoiAngleDeg,
                pluginParams.RoiHalfLength1,
                pluginParams.RoiHalfLength2,
                pluginParams.RoiPointNum);
            window.SetDraw("margin");
            window.SetLineWidth(2);
            window.SetColor("green");
            window.DispRectangle2(followed.Row, followed.Col, -ToRad(followed.AngleDeg), followed.HalfLength1, followed.HalfLength2);
            window.SetTposition((int)Math.Round(followed.Row + 10), (int)Math.Round(followed.Col + 10));
            window.WriteString("跟随ROI Follow ROI");
        }
        /// <summary>
        /// 按指定颜色绘制 HALCON 对象，并在给定位置写入标签。
        /// </summary>
        private void DispObject(HWindow window, HObject displayObject, string color, string label, double labelRow, double labelCol)
        {
            if (displayObject == null || !displayObject.IsInitialized()) return;
            window.SetColor(color);
            window.SetLineWidth(3);
            window.DispObj(displayObject);
            window.SetTposition((int)Math.Round(labelRow), (int)Math.Round(labelCol));
            window.WriteString(label);
        }

        /// <summary>
        /// 把参数输出转换为界面结果表格，包括原始点、滤波点、凸点、突变和运行汇总。
        /// </summary>
        private void RefreshResultRows()
        {
            RawPointRows.Clear();
            FilteredPointRows.Clear();
            ConvexResultRows.Clear();
            JumpResultRows.Clear();
            SummaryRows.Clear();

            if (pluginParams == null) return;
            pluginParams.EnsureOutputs();

            IList<MeasurePointParam> rawPoints = pluginParams.GrabOutput.RawPoints != null && pluginParams.GrabOutput.RawPoints.Count > 0
                ? pluginParams.GrabOutput.RawPoints
                : pluginParams.InputPoints;
            AddPointRows(RawPointRows, rawPoints);
            AddPointRows(FilteredPointRows, pluginParams.FilterOutput.SelectedPoints);

            AddResult(ConvexResultRows, "启用", pluginParams.EnableConvexPeakDetection ? "是" : "否");
            AddResult(ConvexResultRows, "检测方法", pluginParams.EnableConvexPeakDetection ? "角点凸点" : "--");
            AddResult(ConvexResultRows, "状态", pluginParams.ConvexOutput.Success ? "OK" : "NG");
            AddResult(ConvexResultRows, "原因", string.IsNullOrEmpty(pluginParams.ConvexOutput.RejectReason) ? "--" : pluginParams.ConvexOutput.RejectReason);
            AddResult(ConvexResultRows, "方法", string.IsNullOrEmpty(pluginParams.ConvexOutput.Method) ? "--" : pluginParams.ConvexOutput.Method);
            AddResult(ConvexResultRows, "Peak Row", FormatDouble(pluginParams.ConvexOutput.PeakRow));
            AddResult(ConvexResultRows, "Peak Col", FormatDouble(pluginParams.ConvexOutput.PeakCol));
            AddResult(ConvexResultRows, "Score", FormatDouble(pluginParams.ConvexOutput.Score, "F3"));
            AddResult(ConvexResultRows, "Prominence", FormatDouble(pluginParams.ConvexOutput.Prominence));
            AddResult(ConvexResultRows, "SlopeDelta", FormatDouble(pluginParams.ConvexOutput.SlopeDelta));
            AddResult(ConvexResultRows, "TurnAngle", FormatDouble(pluginParams.ConvexOutput.TurnAngleDeg));
            AddResult(ConvexResultRows, "FitError", FormatDouble(pluginParams.ConvexOutput.FitError));

            AddResult(JumpResultRows, "启用", pluginParams.EnableJumpDetection ? "是" : "否");
            AddResult(JumpResultRows, "状态", pluginParams.JumpOutput.Success ? "OK" : "NG");
            AddResult(JumpResultRows, "原因", string.IsNullOrEmpty(pluginParams.JumpOutput.RejectReason) ? "--" : pluginParams.JumpOutput.RejectReason);
            AddResult(JumpResultRows, "模式", pluginParams.JumpOutput.Mode);
            AddResult(JumpResultRows, "交点 Row", FormatDouble(pluginParams.JumpOutput.IntersectionRow));
            AddResult(JumpResultRows, "交点 Col", FormatDouble(pluginParams.JumpOutput.IntersectionCol));
            AddResult(JumpResultRows, "夹角", FormatDouble(pluginParams.JumpOutput.TurnAngleDeg));
            AddResult(JumpResultRows, "主边角度", FormatDouble(pluginParams.JumpOutput.MainAngleDeg));
            AddResult(JumpResultRows, "主边点数", CountOf(pluginParams.JumpOutput.MainEdge.Points).ToString(CultureInfo.InvariantCulture));
            AddResult(JumpResultRows, "副边点数", CountOf(pluginParams.JumpOutput.SecondaryEdge.Points).ToString(CultureInfo.InvariantCulture));
            AddResult(JumpResultRows, "主边误差", FormatDouble(pluginParams.JumpOutput.MainEdge.FitError));
            AddResult(JumpResultRows, "副边误差", FormatDouble(pluginParams.JumpOutput.SecondaryEdge.FitError));

            AddResult(SummaryRows, "运行状态", pluginParams.m_bRunSuccess ? "OK" : "NG");
            AddResult(SummaryRows, "耗时", FormatDouble(pluginParams.m_fElapsedMs) + " ms");
            AddResult(SummaryRows, "消息", string.IsNullOrEmpty(pluginParams.m_strRunMessage) ? "--" : pluginParams.m_strRunMessage);
            AddResult(SummaryRows, "抓边状态", pluginParams.GrabOutput.Success ? "OK" : "NG");
            AddResult(SummaryRows, "抓边点数", pluginParams.GrabOutput.RawPointCount.ToString(CultureInfo.InvariantCulture));
            AddResult(SummaryRows, "中值剔除点数", pluginParams.GrabOutput.RemovedPointCount.ToString(CultureInfo.InvariantCulture));
            AddResult(SummaryRows, "位置跟随", pluginParams.m_bUsePositionFollow ? "启用" : "关闭");
            AddResult(SummaryRows, "当前位姿有效", pluginParams.m_bCurrentPoseValid ? "是" : "否");
            AddResult(SummaryRows, "跟随偏差", string.Format(CultureInfo.InvariantCulture, "X={0:F3}, Y={1:F3}, R={2:F3}", pluginParams.m_fFollowOffsetX, pluginParams.m_fFollowOffsetY, pluginParams.m_fFollowOffsetR));
            AddResult(SummaryRows, "滤波步骤", string.IsNullOrEmpty(pluginParams.FilterOutput.SelectedStepName) ? "--" : pluginParams.FilterOutput.SelectedStepName);
            AddResult(SummaryRows, "检测点数", pluginParams.FilterOutput.PointCount.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// 刷新底部状态栏，显示耗时、鼠标坐标、灰度、图像大小、点数和运行状态。
        /// </summary>
        private void UpdateStatusBarText()
        {
            int rawCount = 0;
            int filterCount = 0;
            double elapsed = 0;
            string state = "--";
            if (pluginParams != null)
            {
                pluginParams.EnsureOutputs();
                rawCount = pluginParams.GrabOutput.RawPointCount > 0
                    ? pluginParams.GrabOutput.RawPointCount
                    : CountOf(pluginParams.InputPoints);
                filterCount = pluginParams.FilterOutput.PointCount;
                elapsed = pluginParams.m_fElapsedMs;
                state = pluginParams.m_bRunSuccess ? "OK" : "NG";
            }

            StatusBarText = string.Format(
                CultureInfo.InvariantCulture,
                "耗时={0:F2} ms | 鼠标 Row={1} Col={2} Gray={3} | 图像={4} | 原始={5} 滤波={6} | 状态={7}",
                elapsed,
                FormatMouse(mouseRow),
                FormatMouse(mouseCol),
                string.IsNullOrEmpty(mouseGray) ? "--" : mouseGray,
                imageWidth > 0 && imageHeight > 0 ? imageWidth + "x" + imageHeight : "--",
                rawCount,
                filterCount,
                state);
        }

        /// <summary>
        /// 将点集写入表格行集合。
        /// </summary>
        private static void AddPointRows(ObservableCollection<PointResultRow> target, IList<MeasurePointParam> points)
        {
            if (target == null || points == null) return;
            for (int index = 0; index < points.Count; index++)
            {
                MeasurePointParam point = points[index];
                target.Add(new PointResultRow(index + 1, point.Row, point.Col));
            }
        }

        /// <summary>
        /// 向键值结果表格追加一行。
        /// </summary>
        private static void AddResult(ObservableCollection<KeyValueResultRow> target, string name, string value)
        {
            if (target == null) return;
            target.Add(new KeyValueResultRow(name, value));
        }

        /// <summary>
        /// 空安全地统计列表数量。
        /// </summary>
        private static int CountOf<T>(IList<T> list)
        {
            return list == null ? 0 : list.Count;
        }

        /// <summary>
        /// 使用 F2 格式输出 double，非法值显示为 --。
        /// </summary>
        private static string FormatDouble(double value)
        {
            return FormatDouble(value, "F2");
        }

        /// <summary>
        /// 使用指定格式输出 double，非法值显示为 --。
        /// </summary>
        private static string FormatDouble(double value, string format)
        {
            if (double.IsNaN(value) || double.IsInfinity(value)) return "--";
            return value.ToString(format, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// 格式化鼠标坐标，非法值显示为 --。
        /// </summary>
        private static string FormatMouse(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value)) return "--";
            return value.ToString("F1", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// 构建顶部状态详情文本，集中显示 ROI、跟随、滤波、突变和凸点关键结果。
        /// </summary>
        private string BuildStatusText()
        {
            if (pluginParams == null) return "等待运行 / Ready";
            pluginParams.EnsureOutputs();
            return string.Format(
                CultureInfo.InvariantCulture,
                "{0}\nROI：启用={1} Center=({2:F2},{3:F2}) Angle={4:F2}° Half=({5:F2},{6:F2})\n跟随 Follow：启用={7} 当前有效={8} Offset(X,Y,R)=({9:F3},{10:F3},{11:F3})\n滤波 Filter：启用={12} 步骤={13} 总步骤={14} 当前点数={15}\n突变 Jump：主边点={16} 副边点={17} 交点=({18:F2},{19:F2}) 夹角={20:F2}° 误差=({21:F2},{22:F2})\n凸点 Convex：点=({23:F2},{24:F2}) Score={25:F3} 夹角={26:F2}° 突起度={27:F2} 误差={28:F2}",
                string.IsNullOrEmpty(pluginParams.m_strRunMessage) ? "等待运行 / Ready" : pluginParams.m_strRunMessage,
                pluginParams.EnableRoi,
                pluginParams.RoiCenterRow,
                pluginParams.RoiCenterCol,
                pluginParams.RoiAngleDeg,
                pluginParams.RoiHalfLength1,
                pluginParams.RoiHalfLength2,
                pluginParams.m_bUsePositionFollow,
                pluginParams.m_bCurrentPoseValid,
                pluginParams.m_fFollowOffsetX,
                pluginParams.m_fFollowOffsetY,
                pluginParams.m_fFollowOffsetR,
                pluginParams.FilterOutput.FilterEnabled,
                string.IsNullOrEmpty(pluginParams.FilterOutput.SelectedStepName) ? "未运行" : pluginParams.FilterOutput.SelectedStepName,
                pluginParams.FilterOutput.Steps == null ? 0 : pluginParams.FilterOutput.Steps.Count,
                pluginParams.FilterOutput.PointCount,
                pluginParams.JumpOutput.MainEdge.Points == null ? 0 : pluginParams.JumpOutput.MainEdge.Points.Count,
                pluginParams.JumpOutput.SecondaryEdge.Points == null ? 0 : pluginParams.JumpOutput.SecondaryEdge.Points.Count,
                pluginParams.JumpOutput.IntersectionRow,
                pluginParams.JumpOutput.IntersectionCol,
                pluginParams.JumpOutput.TurnAngleDeg,
                pluginParams.JumpOutput.MainEdge.FitError,
                pluginParams.JumpOutput.SecondaryEdge.FitError,
                pluginParams.ConvexOutput.PeakRow,
                pluginParams.ConvexOutput.PeakCol,
                pluginParams.ConvexOutput.Score,
                pluginParams.ConvexOutput.TurnAngleDeg,
                pluginParams.ConvexOutput.Prominence,
                pluginParams.ConvexOutput.FitError);
        }
        /// <summary>
        /// 计算拟合边标签位置；有有效线段时取线段中点，否则取边点中心或备用坐标。
        /// </summary>
        private static void GetEdgeLabelPosition(FittedEdgeOutputParam edge, double fallbackRow, double fallbackCol, out double row, out double col)
        {
            if (edge != null && Math.Abs(edge.StartRow - edge.EndRow) + Math.Abs(edge.StartCol - edge.EndCol) > 0.000001)
            {
                row = (edge.StartRow + edge.EndRow) * 0.5;
                col = (edge.StartCol + edge.EndCol) * 0.5;
                return;
            }
            GetPointCenter(edge == null ? null : edge.Points, fallbackRow, fallbackCol, out row, out col);
        }

        /// <summary>
        /// 计算点集中心，点集为空时返回备用坐标。
        /// </summary>
        private static void GetPointCenter(IList<MeasurePointParam> points, double fallbackRow, double fallbackCol, out double row, out double col)
        {
            if (points == null || points.Count == 0)
            {
                row = fallbackRow;
                col = fallbackCol;
                return;
            }

            double rowSum = 0;
            double colSum = 0;
            for (int index = 0; index < points.Count; index++)
            {
                rowSum += points[index].Row;
                colSum += points[index].Col;
            }
            row = rowSum / points.Count;
            col = colSum / points.Count;
        }

        /// <summary>
        /// 规范化突变模式，只允许 Row 或 Col。
        /// </summary>
        private static string NormalizeJumpMode(string mode)
        {
            return string.Equals(mode, "Col", StringComparison.OrdinalIgnoreCase) ? "Col" : "Row";
        }

        /// <summary>
        /// 使用 HALCON TupleRad 将角度从度转换为弧度。
        /// </summary>
        private static double ToRad(double angleDeg)
        {
            HOperatorSet.TupleRad(angleDeg, out HTuple rad);
            return rad.D;
        }

        /// <summary>
        /// 规范化 HALCON measure_transition 参数。
        /// </summary>
        private static string NormalizeTransition(string value)
        {
            if (string.Equals(value, "negative", StringComparison.OrdinalIgnoreCase)) return "negative";
            if (string.Equals(value, "positive", StringComparison.OrdinalIgnoreCase)) return "positive";
            if (string.Equals(value, "uniform", StringComparison.OrdinalIgnoreCase)) return "uniform";
            return "all";
        }

        /// <summary>
        /// 规范化 HALCON measure_select 参数。
        /// </summary>
        private static string NormalizeSelect(string value)
        {
            if (string.Equals(value, "first", StringComparison.OrdinalIgnoreCase)) return "first";
            if (string.Equals(value, "last", StringComparison.OrdinalIgnoreCase)) return "last";
            return "all";
        }

        /// <summary>
        /// 规范化 HALCON measure_interpolation 参数。
        /// </summary>
        private static string NormalizeInterpolation(string value)
        {
            if (string.Equals(value, "nearest_neighbor", StringComparison.OrdinalIgnoreCase)) return "nearest_neighbor";
            if (string.Equals(value, "bicubic", StringComparison.OrdinalIgnoreCase)) return "bicubic";
            return "bilinear";
        }

        /// <summary>
        /// 规范化字符串布尔值，非 true 统一视为 false。
        /// </summary>
        private static string NormalizeBoolString(string value)
        {
            return string.Equals(value, "true", StringComparison.OrdinalIgnoreCase) ? "true" : "false";
        }

        /// <summary>
        /// 将数值限制到 0 到 1。
        /// </summary>
        private static double Clamp01(double value)
        {
            if (value < 0) return 0;
            if (value > 1) return 1;
            return value;
        }

        /// <summary>
        /// 将角度限制到 0 到 90 度。
        /// </summary>
        private static double Clamp0To90(double value)
        {
            if (value < 0) return 0;
            if (value > 90) return 90;
            return value;
        }

        /// <summary>
        /// 将角度归一化到 -180 到 180 度。
        /// </summary>
        private static double ClampAngleMinus180To180(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
                return 0;

            value %= 360;
            if (value > 180) return value - 360;
            if (value < -180) return value + 360;
            return value;
        }
    }

    /// <summary>
    /// 点集结果表格行，显示点序号和 Row/Col 坐标。
    /// </summary>
    public sealed class PointResultRow
    {
        public int Index { get; private set; }
        public double Row { get; private set; }
        public double Col { get; private set; }

        /// <summary>
        /// 创建点结果行。
        /// </summary>
        public PointResultRow(int index, double row, double col)
        {
            Index = index;
            Row = row;
            Col = col;
        }
    }

    /// <summary>
    /// 键值结果表格行，用于凸点、突变和汇总结果页。
    /// </summary>
    public sealed class KeyValueResultRow
    {
        public string Name { get; private set; }
        public string Value { get; private set; }

        /// <summary>
        /// 创建键值结果行。
        /// </summary>
        public KeyValueResultRow(string name, string value)
        {
            Name = name ?? string.Empty;
            Value = value ?? string.Empty;
        }
    }
}





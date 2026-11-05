using Frame_Core;
using Frame_Core.pluginParams;
using FrameCommunity.MaskRoiTool;
using HalconDotNet;
using Microsoft.Win32;
using Plugin.TemplateMatch.Core;
using Plugin.TemplateMatch.Core.Matching;
using Plugin.TemplateMatch.Services;
using Plugin.TemplateMatch.WPF架构;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using ViewROI;
using CommunityToolkit.Mvvm.Input;

namespace Plugin.TemplateMatch.ViewModels
{
    /// <summary>
    /// 模板匹配插件的视图模型，实现 IPluginWpfView 接口，负责与视图交互、管理模板参数、执行匹配算法以及显示结果。
    /// </summary>
    public sealed class TemplateMatchViewModel : BindableBase, IPluginWpfView
    {
        // --------------------------- 私有字段 ---------------------------
        private readonly TemplateStorageService storageService = new TemplateStorageService();       // 模板文件存储服务
        private readonly TemplateMatcherFactory matcherFactory = new TemplateMatcherFactory();       // 模板匹配器工厂（根据模式创建不同匹配器）
        private readonly TemplateCreateParameterAdvisor createParameterAdvisor = new TemplateCreateParameterAdvisor(); // 模板创建参数建议器

        private readonly HWindowControl mainWindow = new HWindowControl();     // 主图像显示控件
        private readonly HWindowControl templateWindow = new HWindowControl(); // 模板预览图像显示控件
        private readonly HWindowControl matchWindow = new HWindowControl();    // 匹配结果显示控件

        private TemplateMatchObj pluginObject;           // 插件核心对象（包含执行逻辑）
        private ParamsTemplateMatch pluginParams;        // 插件参数对象（序列化存储）

        private HalconViewRoiAdapter mainDisplay;        // 主显示区域的 ROI 适配器
        private HalconViewRoiAdapter templateDisplay;    // 模板显示区域的 ROI 适配器
        private HalconViewRoiAdapter matchDisplay;       // 匹配结果显示区域的 ROI 适配器

        private TemplateModelItemVm selectedTemplate;    // 当前选中的模板视图模型
        private TemplateMatchResultParam selectedResult; // 当前选中的匹配结果
        private MaskRoiParam selectedMaskRoiParam;       // 当前选中的屏蔽 ROI 参数，供现有 ViewROI 联动
        private MaskRoiOptions maskRoiOptions = new MaskRoiOptions();
        private MaskRoiOptions subscribedMaskRoiOptions;

        private bool isSyncingRoi;                       // 标志位，避免 ROI 同步时的循环触发
        private bool isInitializingView;                  // 标志位，避免界面初始化时多次刷新 Halcon 控件
        private bool isClosed;                            // 标志位，避免关闭释放重复执行

        // 界面绑定的文本属性
        private string statusText = "等待加载";
        private string mouseInfo = "行:-  列:-  灰度:-";
        private string imageSizeText = "图像大小: -";
        private string elapsedText = "耗时: -";
        private string successText = "是否成功: -";

        // --------------------------- 公开属性（用于数据绑定）---------------------------
        public ObservableCollection<TemplateModelItemVm> Templates { get; } = new ObservableCollection<TemplateModelItemVm>();   // 模板列表
        public ObservableCollection<TemplateMatchResultParam> Results { get; } = new ObservableCollection<TemplateMatchResultParam>(); // 匹配结果列表
        public MaskRoiOptions MaskRoiOptions { get { return maskRoiOptions; } }

        // 枚举数据源
        public Array MatchModes { get { return Enum.GetValues(typeof(TemplateMatchMode)); } }
        public Array RoiShapeTypes { get { return new object[] { 10, 11, 12, 13, 14 }; } }   // ROI 形状类型（Halcon 矩形、旋转矩形、圆等）
        public Array MaskRoiShapeTypes { get { return Enum.GetValues(typeof(MaskRoiShapeType)); } }
        public string[] OptimizationOptions { get { return new[] { "auto", "none", "point_reduction_low", "point_reduction_medium", "point_reduction_high", "pregeneration" }; } }
        public string[] MetricOptions { get { return new[] { "use_polarity", "ignore_global_polarity", "ignore_local_polarity", "ignore_color_polarity" }; } }
        public string[] SubPixelOptions { get { return new[] { "least_squares", "least_squares_high", "least_squares_very_high", "interpolation", "none" }; } }

        // 根据选中的模板获取创建参数和查找参数
        public TemplateCreateOptions CreateOptions => SelectedTemplate != null ? SelectedTemplate.CreateOptions : EnsureParams().CreateOptions;
        public TemplateFindOptions FindOptions => SelectedTemplate != null ? SelectedTemplate.FindOptions : EnsureParams().FindOptions;

        // 界面命令
        public ICommand AddTemplateCommand { get; }
        public ICommand DeleteTemplateCommand { get; }
        public ICommand ApplyRoiCommand { get; }
        public ICommand SetBaseByRoiCommand { get; }
        public ICommand LoadImageCommand { get; }
        public ICommand CreateTemplateCommand { get; }
        public ICommand RunCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand FitMainCommand { get; }
        public ICommand FitTemplateCommand { get; }
        public ICommand FitMatchCommand { get; }
    

        // 插件接口成员
        public IPlugin PluginObject => pluginObject;
        public PluginParamsBase PluginParams => pluginParams;
        public HWindowControl MainHalconWindow => mainWindow;
        public HWindowControl TemplateHalconWindow => templateWindow;
        public HWindowControl MatchHalconWindow => matchWindow;

        // 界面绑定的状态文本
        public string StatusText { get => statusText; private set => SetProperty(ref statusText, value); }
        public string MouseInfo { get => mouseInfo; private set => SetProperty(ref mouseInfo, value); }
        public string ImageSizeText { get => imageSizeText; private set => SetProperty(ref imageSizeText, value); }
        public string ElapsedText { get => elapsedText; private set => SetProperty(ref elapsedText, value); }
        public string SuccessText { get => successText; private set => SetProperty(ref successText, value); }

        // 各种布尔/数值参数，直接读写 pluginParams 并触发属性变化通知
        public bool UseLocalRoi { get => EnsureParams().m_bUseLocalRoi; set { var p = EnsureParams(); if (p.m_bUseLocalRoi == value) return; p.m_bUseLocalRoi = value; if (value) p.m_bShowLocalRoi = true; OnPropertyChanged(); OnPropertyChanged(nameof(ShowLocalRoi)); SyncMainRois(); ShowMainImage(false); } }
        public bool ShowViewRoi { get => EnsureParams().m_bShowViewRoi; set { var p = EnsureParams(); if (p.m_bShowViewRoi == value) return; p.m_bShowViewRoi = value; OnPropertyChanged(); SyncMainRois(); ShowMainImage(false); } }
        public bool ShowTemplateRoi { get => EnsureParams().m_bShowTemplateRoi; set { EnsureParams().m_bShowTemplateRoi = value; OnPropertyChanged(); SyncMainRois(); ShowMainImage(false); } }
        public bool ShowLocalRoi { get => EnsureParams().m_bShowLocalRoi; set { EnsureParams().m_bShowLocalRoi = value; OnPropertyChanged(); SyncMainRois(); ShowMainImage(false); } }
        public bool EditTemplateRoi { get => EnsureParams().m_bEditTemplateRoi; set { EnsureParams().m_bEditTemplateRoi = value; OnPropertyChanged(); SyncMainRois(); } }
        public bool EditLocalRoi { get => EnsureParams().m_bEditLocalRoi; set { EnsureParams().m_bEditLocalRoi = value; OnPropertyChanged(); SyncMainRois(); } }
        public bool UseMaskRoi { get => MaskRoiOptions.EnableMaskRoi; set { if (MaskRoiOptions.EnableMaskRoi == value) return; MaskRoiOptions.EnableMaskRoi = value; OnPropertyChanged(); } }
        public bool ShowMaskRoi { get => MaskRoiOptions.ShowMaskRoi; set { if (MaskRoiOptions.ShowMaskRoi == value) return; MaskRoiOptions.ShowMaskRoi = value; OnPropertyChanged(); } }
        public bool EditMaskRoi { get => MaskRoiOptions.EditMaskRoi; set { if (MaskRoiOptions.EditMaskRoi == value) return; MaskRoiOptions.EditMaskRoi = value; OnPropertyChanged(); } }
        public int TemplateRoiShapeType { get => EnsureParams().m_nTemplateRoiShapeType; set { EnsureParams().m_nTemplateRoiShapeType = value; OnPropertyChanged(); SyncMainRois(); } }
        public int LocalRoiShapeType { get => EnsureParams().m_nLocalRoiShapeType; set { EnsureParams().m_nLocalRoiShapeType = value; OnPropertyChanged(); SyncMainRois(); } }
        public bool RoiFollowMatch { get => EnsureParams().m_bRoiFollowMatch; set { var p = EnsureParams(); if (p.m_bRoiFollowMatch == value) return; p.m_bRoiFollowMatch = value; OnPropertyChanged(); ShowMainImage(false); ShowMatchResult(false); } }
        public bool UsePositionFollow { get => EnsureParams().m_bUsePositionFollow; set { var p = EnsureParams(); if (p.m_bUsePositionFollow == value) return; p.m_bUsePositionFollow = value; OnPropertyChanged(); ShowMainImage(false); ShowMatchResult(false); } }
        public double FollowBaseRow { get => EnsureParams().m_fFollowBaseRow; set { EnsureParams().m_fFollowBaseRow = value; OnPropertyChanged(); ShowMainImage(false); } }
        public double FollowBaseCol { get => EnsureParams().m_fFollowBaseCol; set { EnsureParams().m_fFollowBaseCol = value; OnPropertyChanged(); ShowMainImage(false); } }
        public double FollowBaseAngleDeg { get => EnsureParams().m_fFollowBaseAngleDeg; set { EnsureParams().m_fFollowBaseAngleDeg = value; OnPropertyChanged(); ShowMainImage(false); } }
        public double FollowCurrentRow { get => EnsureParams().m_fFollowCurrentRow; set { EnsureParams().m_fFollowCurrentRow = value; OnPropertyChanged(); ShowMainImage(false); } }
        public double FollowCurrentCol { get => EnsureParams().m_fFollowCurrentCol; set { EnsureParams().m_fFollowCurrentCol = value; OnPropertyChanged(); ShowMainImage(false); } }
        public double FollowCurrentAngleDeg { get => EnsureParams().m_fFollowCurrentAngleDeg; set { EnsureParams().m_fFollowCurrentAngleDeg = value; OnPropertyChanged(); ShowMainImage(false); } }

        // 局部 ROI 坐标（矩形区域）
        public double LocalRoiRow1 { get => EnsureParams().m_fLocalRoiRow1; set { EnsureParams().m_fLocalRoiRow1 = value; OnPropertyChanged(); SyncMainRois(); } }
        public double LocalRoiCol1 { get => EnsureParams().m_fLocalRoiCol1; set { EnsureParams().m_fLocalRoiCol1 = value; OnPropertyChanged(); SyncMainRois(); } }
        public double LocalRoiRow2 { get => EnsureParams().m_fLocalRoiRow2; set { EnsureParams().m_fLocalRoiRow2 = value; OnPropertyChanged(); SyncMainRois(); } }
        public double LocalRoiCol2 { get => EnsureParams().m_fLocalRoiCol2; set { EnsureParams().m_fLocalRoiCol2 = value; OnPropertyChanged(); SyncMainRois(); } }

        public MaskRoiParam SelectedMaskRoiParam
        {
            get { return selectedMaskRoiParam; }
            set
            {
                if (ReferenceEquals(selectedMaskRoiParam, value)) return;
                if (selectedMaskRoiParam != null) selectedMaskRoiParam.PropertyChanged -= OnSelectedMaskRoiParamPropertyChanged;
                selectedMaskRoiParam = value;
                if (selectedMaskRoiParam != null) selectedMaskRoiParam.PropertyChanged += OnSelectedMaskRoiParamPropertyChanged;
                UpdateMaskRoiSelectedIndex();
                OnPropertyChanged();
                if (!isInitializingView)
                {
                    SyncMainRois();
                    ShowMainImage(false);
                }
            }
        }

        // 当前选中的模板和结果
        public TemplateModelItemVm SelectedTemplate
        {
            get => selectedTemplate;
            set
            {
                var oldTemplate = selectedTemplate;
                if (SetProperty(ref selectedTemplate, value))
                {
                    if (oldTemplate != null) oldTemplate.PropertyChanged -= OnSelectedTemplatePropertyChanged;
                    if (selectedTemplate != null) selectedTemplate.PropertyChanged += OnSelectedTemplatePropertyChanged;
                    OnPropertyChanged(nameof(CreateOptions));
                    OnPropertyChanged(nameof(FindOptions));
                    if (!isInitializingView)
                    {
                        SyncMainRois();
                        ShowMainImage(false);
                        ShowTemplatePreview(true);
                    }
                }
            }
        }

        public TemplateMatchResultParam SelectedResult
        {
            get => selectedResult;
            set => SetProperty(ref selectedResult, value);
        }

        // --------------------------- 构造函数 ---------------------------
        public TemplateMatchViewModel()
        {
            // 初始化所有命令，绑定到对应的方法
            AddTemplateCommand = new RelayCommand(()=> AddTemplate());
            DeleteTemplateCommand = new RelayCommand(() => DeleteTemplate());
            ApplyRoiCommand = new RelayCommand(() => ApplyRoiToSelected());
            SetBaseByRoiCommand = new RelayCommand(() => SetBaseByRoiCenter());
            LoadImageCommand = new RelayCommand(() => LoadImage());
            CreateTemplateCommand = new RelayCommand(() => CreateTemplate());
            RunCommand = new RelayCommand(() => RunFromHost());
            SaveCommand = new RelayCommand(() => SaveFromHost(pluginObject == null ? string.Empty : pluginObject.moduleInfo.filePath));
            FitMainCommand = new RelayCommand(() => ShowMainImage(true));
            FitTemplateCommand = new RelayCommand(() => ShowTemplatePreview(true));
            FitMatchCommand = new RelayCommand(() => ShowMatchResult(true));
        }

        // --------------------------- IPluginWpfView 接口实现 ---------------------------
        /// <summary>
        /// 绑定插件对象，初始化 Halcon 显示控件，并加载已有参数。
        /// </summary>
        public void BindPlugin(IPlugin plugin)
        {
            isClosed = false;
            pluginObject = plugin as TemplateMatchObj ?? new TemplateMatchObj();
            EnsureParams();
            pluginObject.IniObj();
            InitHalconWindows();
            try
            {
                isInitializingView = true;
                PullFromParams();               // 将参数同步到 ViewModel 属性
                RefreshImageSizeText();
            }
            finally
            {
                isInitializingView = false;
            }
            ShowMainImage(true);
            ShowTemplatePreview(true);
            if (pluginParams != null && pluginParams.m_bRunSuccess)
                ShowMatchResult(true);
            StatusText = "模板已预加载";
        }

        /// <summary>
        /// 刷新所有绑定属性（通常用于外部参数变更后更新界面）。
        /// </summary>
        public void RefreshBindings()
        {
            try
            {
                isInitializingView = true;
                PullFromParams();
                OnPropertyChanged(string.Empty);
            }
            finally
            {
                isInitializingView = false;
            }
            SyncMainRois();
        }

        /// <summary>
        /// 执行模板匹配（由宿主调用）。
        /// </summary>
        public PluginViewActionResult RunFromHost()
        {
            Stopwatch watch = Stopwatch.StartNew();
            try
            {
                BeforeRun();           // 运行前提交参数
                bool ok = pluginObject != null && pluginObject.ExecuteObj();
                GC.Collect();
                watch.Stop();
                AfterRun(ok, watch.ElapsedMilliseconds);
                return ok ? PluginViewActionResult.Ok("模板匹配完成", watch.ElapsedMilliseconds)
                          : PluginViewActionResult.Fail(GetMessage(), watch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                if (watch.IsRunning) watch.Stop();
                StatusText = "运行异常: " + ex.Message;
                return PluginViewActionResult.Fail(StatusText, watch.ElapsedMilliseconds);
            }
        }

        /// <summary>
        /// 保存参数到文件（由宿主调用）。
        /// </summary>
        public PluginViewActionResult SaveFromHost(string filePath)
        {
            try
            {
                BeforeSave();
                if (pluginObject != null && !string.IsNullOrWhiteSpace(filePath))
                    pluginObject.moduleInfo.filePath = filePath;
                bool ok = pluginObject != null && pluginObject.saveParams();
                AfterSave(ok);
                return ok ? PluginViewActionResult.Ok("参数已保存") : PluginViewActionResult.Fail("参数保存失败");
            }
            catch (Exception ex)
            {
                StatusText = "保存异常: " + ex.Message;
                return PluginViewActionResult.Fail(StatusText);
            }
        }

        /// <summary>
        /// 关闭插件，释放资源。
        /// </summary>
        public void CloseFromHost()
        {
            if (isClosed) return;
            isClosed = true;
            BeforeClose();
            DisposeDisplay(ref mainDisplay);
            DisposeDisplay(ref templateDisplay);
            DisposeDisplay(ref matchDisplay);
            ReleaseViewReferences();
        }

        /// <summary>
        /// 运行前的准备工作：提交参数到插件对象。
        /// </summary>
        public void BeforeRun()
        {
            CommitToParams();
        }

        /// <summary>
        /// 运行后的处理：同步模板状态、提取结果、刷新显示。
        /// </summary>
        public void AfterRun(bool success, long elapsedMs)
        {
            SyncTemplatesFromParams();      // 将运行后的模板状态（如是否已创建）同步到界面
            PullResultsFromParams();        // 获取匹配结果列表
            ElapsedText = "耗时: " + elapsedMs + " ms";
            SuccessText = "是否成功: " + (success ? "是" : "否");
            ShowMainImage(false);
            ShowMatchResult(true);
            StatusText = success ? "运行完成" : "运行失败: " + GetMessage();
        }

        /// <summary>
        /// 保存前的参数提交。
        /// </summary>
        public void BeforeSave()
        {
            CommitToParams();
        }

        /// <summary>
        /// 保存后的状态更新。
        /// </summary>
        public void AfterSave(bool success)
        {
            StatusText = success ? "参数已保存" : "参数保存失败";
        }

        /// <summary>
        /// 关闭前的参数提交。
        /// </summary>
        public void BeforeClose()
        {
            CommitToParams();
            DetachTemplateEvents();
        }

        /// <summary>
        /// 当插件从外部执行完成时调用（例如通过流程引擎调用），更新界面显示。
        /// </summary>
        public void OnPluginExecutedFromOutside(PluginExecuteCompletedEventArgs e)
        {
            if (e == null || pluginParams == null || !ReferenceEquals(e.Param, pluginParams)) return;
            AfterRun(e.Success, e.ElapsedMs);
        }

        // --------------------------- 私有方法（业务逻辑）---------------------------
        /// <summary>
        /// 打开文件对话框，加载图像到插件参数中。
        /// </summary>
        private void LoadImage()
        {
            EnsureParams();
            var dialog = new Microsoft.Win32.OpenFileDialog { Filter = "图像文件|*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff|所有文件|*.*" };
            if (dialog.ShowDialog() != true) return;

            try
            {
                isSyncingRoi = true;
                pluginParams.ho_Image?.Dispose();
                pluginParams.ho_Image = null;
                HObject loadedImage = null;
                HObject grayImage = null;
                try
                {
                    HOperatorSet.ReadImage(out loadedImage, dialog.FileName);
                    HOperatorSet.Rgb1ToGray(loadedImage, out grayImage);
                    pluginParams.ho_Image = grayImage;
                    grayImage = null;
                }
                finally
                {
                    HalconObjectGuard.DisposeObject(ref loadedImage);
                    HalconObjectGuard.DisposeObject(ref grayImage);
                }
                RefreshImageSizeText();
                ShowMainImage(true);
                ShowTemplatePreview(true);
                ShowMatchResult(true);
            }
            finally
            {
                isSyncingRoi = false;
            }
            StatusText = "图像已加载: " + Path.GetFileName(dialog.FileName);
        }

        /// <summary>
        /// 添加一个新模板（初始为未创建状态，仅设置默认 ROI 区域）。
        /// </summary>
        private void AddTemplate()
        {
            ParamsTemplateMatch p = EnsureParams();
            // 基于当前选中的模板或默认值创建新模板参数
            TemplateModelParam source = SelectedTemplate != null ? SelectedTemplate.CopyToNewParam() : new TemplateModelParam();
            source.Name = "Template" + (Templates.Count + 1);
            source.IsEnabled = true;
            source.IsCreated = false;
            source.MatchMode = source.CreateOptions == null ? source.MatchMode : source.CreateOptions.MatchMode;
            if (source.CreateOptions != null) source.CreateOptions.MatchMode = source.MatchMode;
            // 默认 ROI 大小 200x200
            source.RoiRow1 = 100;
            source.RoiCol1 = 100;
            source.RoiRow2 = 300;
            source.RoiCol2 = 300;
            source.RoiShapeType = p.m_nTemplateRoiShapeType;
            source.BaseRow = 200;
            source.BaseCol = 200;
            source.ModelId = null;
            source.CreateContours = null;

            TemplateModelItemVm item = new TemplateModelItemVm(source);
            Templates.Add(item);
            SelectedTemplate = item;
            CommitToParams();   // 立即同步到参数对象
            ShowMainImage(false);
            StatusText = "已新增模板: " + item.Name;
        }

        /// <summary>
        /// 删除当前选中的模板，并从磁盘删除对应的模板文件和图片。
        /// </summary>
        private void DeleteTemplate()
        {
            if (SelectedTemplate == null) return;
            TemplateModelParam target = SelectedTemplate.CopyToNewParam();
            storageService.DeleteTemplate(target, EnsureParams().TemplateRootDir);
            Templates.Remove(SelectedTemplate);
            CommitToParams();
            SelectedTemplate = Templates.FirstOrDefault();
            ShowTemplatePreview(true);
            StatusText = "模板文件夹和模板图片已删除";
        }

        private static int GetMaskRoiSelectedIndex(IList<MaskRoiParam> rois, MaskRoiParam selected)
        {
            if (rois == null || rois.Count == 0) return -1;
            if (selected == null) return 0;

            int index = rois.IndexOf(selected);
            return index >= 0 ? index : 0;
        }

        private void BindMaskRoiOptions(MaskRoiOptions options)
        {
            if (subscribedMaskRoiOptions != null)
                subscribedMaskRoiOptions.PropertyChanged -= OnMaskRoiOptionsPropertyChanged;

            maskRoiOptions = options ?? new MaskRoiOptions();
            maskRoiOptions.EnsureList();
            subscribedMaskRoiOptions = maskRoiOptions;
            subscribedMaskRoiOptions.PropertyChanged += OnMaskRoiOptionsPropertyChanged;
            OnPropertyChanged(nameof(MaskRoiOptions));
        }

        private void OnMaskRoiOptionsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ParamsTemplateMatch p = EnsureParams();
            maskRoiOptions.EnsureList();
            p.m_bUseMaskRoi = maskRoiOptions.EnableMaskRoi;
            p.m_bShowMaskRoi = maskRoiOptions.ShowMaskRoi;
            p.m_bEditMaskRoi = maskRoiOptions.EditMaskRoi;
            p.m_lMaskRoiParams = maskRoiOptions.MaskRois;

            if (selectedMaskRoiParam != null && !p.m_lMaskRoiParams.Contains(selectedMaskRoiParam))
                SelectedMaskRoiParam = null;

            OnPropertyChanged(nameof(UseMaskRoi));
            OnPropertyChanged(nameof(ShowMaskRoi));
            OnPropertyChanged(nameof(EditMaskRoi));

            if (isInitializingView) return;
            CommitToParams();
            SyncMainRois();
            ShowMainImage(false);
            ShowMatchResult(false);
        }

        private void OnSelectedMaskRoiParamPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (isInitializingView || isSyncingRoi) return;
            CommitToParams();
            SyncMainRois();
            ShowMainImage(false);
            ShowMatchResult(false);
        }

        private void UpdateMaskRoiSelectedIndex()
        {
            if (maskRoiOptions == null) return;
            maskRoiOptions.SelectedIndex = GetMaskRoiSelectedIndex(maskRoiOptions.MaskRois, selectedMaskRoiParam);
        }

        /// <summary>
        /// 创建模板：从当前图像上提取 ROI 区域的图像特征，生成模板模型并保存。
        /// </summary>
        private void CreateTemplate()
        {
            ParamsTemplateMatch p = EnsureParams();
            TemplateModelItemVm templateVm = SelectedTemplate;
            if (templateVm == null || !HasValidImage(p.ho_Image)) return;
            TemplateModelParam template = templateVm.ToParam();

            HObject region = null;
            HObject reduceImage = null;
            HObject preview = null;
            HObject templateImage = null;
            HTuple modelId = null;
            try
            {
                // 确保创建参数存在且匹配模式一致
                if (template.CreateOptions == null)
                    template.CreateOptions = new TemplateCreateOptions();
                template.MatchMode = template.CreateOptions == null ? template.MatchMode : template.CreateOptions.MatchMode;
                template.CreateOptions.MatchMode = template.MatchMode;

                // 根据模板 ROI 生成区域并削减图像域
                HOperatorSet.GenRectangle1(out region, template.RoiRow1, template.RoiCol1, template.RoiRow2, template.RoiCol2);
                HOperatorSet.ReduceDomain(p.ho_Image, region, out reduceImage);

                // 建议模板创建参数，并弹出对话框让用户调整
                TemplateCreateParameterSuggestion suggestion = createParameterAdvisor.Determine(reduceImage, template.CreateOptions);
                var dialogVm = new TemplateCreateParameterDialogVm(template.CreateOptions, suggestion);
                var dialog = new TemplateCreateParameterDialog { DataContext = dialogVm };
                if (dialog.ShowDialog() != true)
                {
                    StatusText = "已取消创建模板";
                    return;
                }
                dialogVm.ApplyTo(template.CreateOptions);
                templateVm.CreateOptions = template.CreateOptions;

                // 调用匹配器创建模板
                ITemplateMatcher matcher = matcherFactory.Resolve(template.MatchMode);
                matcher.Create(p.ho_Image, reduceImage, region, template, template.CreateOptions, out modelId, out preview);

                // 释放旧的模板对象，保存新模型
                template.Dispose();
                template.ModelId = modelId;
                modelId = null;          // 已经转移所有权，避免重复释放
                template.CreateContours = preview;
                preview = null;
                template.IsCreated = true;
                template.IsEnabled = true;

                // 保存模板模型到磁盘，同时保存模板图像
                storageService.SaveTemplate(template, p.TemplateRootDir);
                templateVm.CopyFrom(template);

                string imagePath = storageService.GetTemplateImagePath(p.TemplateRootDir, template);
                Directory.CreateDirectory(Path.GetDirectoryName(imagePath));
                HOperatorSet.CropDomain(reduceImage, out templateImage);
                HOperatorSet.WriteImage(templateImage, "png", 0, imagePath);

                CommitToParams();
                ShowMainImage(false);
                ShowTemplatePreview(true);
                ShowMatchResult(true);
                StatusText = "模板已创建";
            }
            catch(Exception ee)
            {
                StatusText = "模板创建失败";
                System.Windows.MessageBox.Show("模板创建失败!!!\n"+ee.ToString(), "报警", MessageBoxButton.OK, MessageBoxImage.Error);

            }
            finally
            {
                // 清理临时 Halcon 对象
                if (modelId != null && modelId.Length > 0)
                    HalconObjectGuard.ClearTemplateModel(template == null ? TemplateMatchMode.Shape : template.MatchMode, ref modelId);
                HalconObjectGuard.DisposeObject(ref region);
                HalconObjectGuard.DisposeObject(ref reduceImage);
                HalconObjectGuard.DisposeObject(ref preview);
                HalconObjectGuard.DisposeObject(ref templateImage);
            }
        }

        /// <summary>
        /// 将主视图上当前激活的 ROI 应用到选中的模板或局部 ROI 参数上。
        /// </summary>
        private void ApplyRoiToSelected()
        {
            if (SelectedTemplate == null || mainDisplay == null) return;
            if (!mainDisplay.TryGetActiveRoiBounds(out double row1, out double col1, out double row2, out double col2)) return;

            if (mainDisplay.ActiveRoiRole == ParamsTemplateMatch.RoiRoleLocal)
            {
                // 应用到局部 ROI
                LocalRoiRow1 = row1;
                LocalRoiCol1 = col1;
                LocalRoiRow2 = row2;
                LocalRoiCol2 = col2;
            }
            else if (mainDisplay.ActiveRoiRole == ParamsTemplateMatch.RoiRoleMask && SelectedMaskRoiParam != null)
            {
                UpdateSelectedMaskRoiBounds(row1, col1, row2, col2);
            }
            else
            {
                // 应用到模板 ROI
                SelectedTemplate.SetRoi(row1, col1, row2, col2);
            }
            CommitToParams();
            ShowMainImage(false);
            ShowTemplatePreview(true);
            ShowMatchResult(true);
        }

        /// <summary>
        /// 将当前激活 ROI 的中心点设置为选中模板的基准点（用于位置跟随的参考）。
        /// </summary>
        private void SetBaseByRoiCenter()
        {
            if (SelectedTemplate == null || mainDisplay == null) return;
            if (!mainDisplay.TryGetActiveRoiBounds(out double row1, out double col1, out double row2, out double col2)) return;
            SelectedTemplate.BaseRow = (row1 + row2) / 2.0;
            SelectedTemplate.BaseCol = (col1 + col2) / 2.0;
            SelectedTemplate.BaseAngleDeg = 0;
            CommitToParams();
            ShowMainImage(false);
            ShowTemplatePreview(true);
            ShowMatchResult(true);
        }

        /// <summary>
        /// 初始化三个 Halcon 显示窗口的适配器，并订阅事件。
        /// </summary>
        private void InitHalconWindows()
        {
            if (mainDisplay != null) return;
            mainDisplay = new HalconViewRoiAdapter(mainWindow, ShowViewRoi);
            templateDisplay = new HalconViewRoiAdapter(templateWindow, false);
            matchDisplay = new HalconViewRoiAdapter(matchWindow, false);

            mainDisplay.RoiChanged += OnMainRoiChanged;
            mainDisplay.MouseMoved += OnMainMouseMoved;
            mainDisplay.RightMouseDown += OnMainRightMouseDown;
            mainDisplay.MiddleMouseDown += OnMainMiddleMouseDown;
            mainDisplay.Redraw += DrawMainOverlay;
            templateDisplay.Redraw += DrawTemplateOverlay;
            matchDisplay.Redraw += DrawMatchOverlay;
        }

        /// <summary>
        /// 在主显示区域显示插件当前的图像，并可选择适应窗口大小。
        /// </summary>
        private void ShowMainImage(bool fit)
        {
            EnsureParams();
            if (mainDisplay == null) return;
            bool wasSyncing = isSyncingRoi;
            try
            {
                mainDisplay.SetOverlayState(pluginParams, SelectedTemplate?.ToParam());
                SyncMainRois();
                isSyncingRoi = true;
                mainDisplay.SetRoiOperate(EditTemplateRoi || EditLocalRoi || EditMaskRoi);
                mainDisplay.ShowImage(pluginParams.ho_Image, fit);
                DrawMainOverlay();
                FocusWindow(mainWindow);
            }
            finally
            {
                isSyncingRoi = wasSyncing;
            }
            OnPropertyChanged(nameof(SelectedMaskRoiParam));
        }

        /// <summary>
        /// 在模板预览窗口显示图像并绘制模板轮廓和基准点。
        /// </summary>
        private void ShowTemplatePreview(bool fit)
        {
            EnsureParams();
            if (templateDisplay == null) return;
            templateDisplay.ShowImage(pluginParams.ho_Image, fit);
            DrawTemplateOverlay();
        }

        /// <summary>
        /// 在匹配结果显示窗口显示图像并绘制匹配结果（轮廓、十字标记等）。
        /// </summary>
        private void ShowMatchResult(bool fit)
        {
            EnsureParams();
            if (matchDisplay == null) return;
            matchDisplay.ShowImage(pluginParams.ho_Image, fit);
            DrawMatchOverlay();
        }

        // --------------------------- 事件处理 ---------------------------
        /// <summary>
        /// 主显示区域 ROI 发生变化时的回调，用于同步更新 ViewModel 中的 ROI 参数。
        /// </summary>
        private void OnMainRoiChanged(int eventCode)
        {
            if (isSyncingRoi) return;
            if (mainDisplay == null) return;
            if (eventCode != ROIController.EVENT_UPDATE_ROI && eventCode != ROIController.EVENT_CREATED_ROI) return;

            if (!mainDisplay.TryGetActiveRoiBounds(out double row1, out double col1, out double row2, out double col2)) return;
            if (mainDisplay.ActiveRoiRole == ParamsTemplateMatch.RoiRoleLocal)
            {
                if (!EditLocalRoi) return;
                pluginParams.m_fLocalRoiRow1 = row1;
                pluginParams.m_fLocalRoiCol1 = col1;
                pluginParams.m_fLocalRoiRow2 = row2;
                pluginParams.m_fLocalRoiCol2 = col2;
                OnPropertyChanged(nameof(LocalRoiRow1));
                OnPropertyChanged(nameof(LocalRoiCol1));
                OnPropertyChanged(nameof(LocalRoiRow2));
                OnPropertyChanged(nameof(LocalRoiCol2));
            }
            else if (mainDisplay.ActiveRoiRole == ParamsTemplateMatch.RoiRoleMask)
            {
                if (!EditMaskRoi || SelectedMaskRoiParam == null) return;
                UpdateSelectedMaskRoiBounds(row1, col1, row2, col2);
            }
            else if (SelectedTemplate != null)
            {
                if (!EditTemplateRoi) return;
                SelectedTemplate.RoiRow1 = Math.Min(row1, row2);
                SelectedTemplate.RoiCol1 = Math.Min(col1, col2);
                SelectedTemplate.RoiRow2 = Math.Max(row1, row2);
                SelectedTemplate.RoiCol2 = Math.Max(col1, col2);
            }
            CommitToParams();
            SyncMainRois();
        }

        /// <summary>
        /// 当选中模板的属性发生变化时，重新同步主显示区域的 ROI。
        /// </summary>
        private void OnSelectedTemplatePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (isInitializingView) return;
            if (selectedTemplate == null) return;
            if (e == null) return;
            if (e.PropertyName == nameof(TemplateModelItemVm.RoiRow1) ||
                e.PropertyName == nameof(TemplateModelItemVm.RoiCol1) ||
                e.PropertyName == nameof(TemplateModelItemVm.RoiRow2) ||
                e.PropertyName == nameof(TemplateModelItemVm.RoiCol2) ||
                e.PropertyName == nameof(TemplateModelItemVm.RoiShapeType) ||
                e.PropertyName == nameof(TemplateModelItemVm.BaseRow) ||
                e.PropertyName == nameof(TemplateModelItemVm.BaseCol))
            {
                SyncMainRois();
            }
        }

        private void UpdateSelectedMaskRoiBounds(double row1, double col1, double row2, double col2)
        {
            if (SelectedMaskRoiParam == null) return;
            bool wasSyncing = isSyncingRoi;
            try
            {
                isSyncingRoi = true;
                SelectedMaskRoiParam.Row1 = Math.Min(row1, row2);
                SelectedMaskRoiParam.Col1 = Math.Min(col1, col2);
                SelectedMaskRoiParam.Row2 = Math.Max(row1, row2);
                SelectedMaskRoiParam.Col2 = Math.Max(col1, col2);
                SelectedMaskRoiParam.NormalizeFromRectangle1();
            }
            finally
            {
                isSyncingRoi = wasSyncing;
            }
        }

        /// <summary>
        /// 鼠标移动时更新鼠标信息文本（坐标、灰度值）。
        /// </summary>
        private void OnMainMouseMoved(HalconMouseInfoArgs args)
        {
            if (args != null) MouseInfo = args.Text;
        }

        /// <summary>
        /// 右键点击主图像时，询问是否将当前点设置为模板的基准点。
        /// </summary>
        private void OnMainRightMouseDown(HalconMouseInfoArgs args)
        {
            if (args == null || SelectedTemplate == null) return;
            if (System.Windows.Forms.MessageBox.Show("是否将当前点设置为基准点？", "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;
            SelectedTemplate.BaseRow = args.Row;
            SelectedTemplate.BaseCol = args.Column;
            SelectedTemplate.BaseAngleDeg = 0;
            CommitToParams();
            ShowMainImage(false);
            ShowTemplatePreview(false);
        }

        /// <summary>
        /// 中键点击主图像时，弹出上下文菜单，用于设置位置跟随的当前点和基准点。
        /// </summary>
        private void OnMainMiddleMouseDown(HalconMouseInfoArgs args)
        {
            if (args == null || mainWindow == null) return;

            var menu = new ContextMenuStrip();
            menu.Items.Add("设置当前点", null, delegate { SetFollowCurrentPoint(args.Row, args.Column); });
            menu.Items.Add("设置基准点", null, delegate { SetFollowBasePoint(args.Row, args.Column); });
            menu.Items.Add("手动输入位置跟随", null, delegate { ShowPositionFollowInputDialog(); });
            menu.Show(mainWindow, mainWindow.PointToClient(Control.MousePosition));
        }

        /// <summary>
        /// 设置位置跟随的当前匹配点（即模板匹配后找到的实例位置）。
        /// </summary>
        private void SetFollowCurrentPoint(double row, double col)
        {
            FollowCurrentRow = row;
            FollowCurrentCol = col;
            CommitToParams();
            StatusText = $"已设置当前点: 行 {row:0.###}, 列 {col:0.###}";
            ShowMainImage(false);
        }

        /// <summary>
        /// 设置位置跟随的基准点（待匹配图像上的参考位置）。
        /// </summary>
        private void SetFollowBasePoint(double row, double col)
        {
            FollowBaseRow = row;
            FollowBaseCol = col;
            CommitToParams();
            StatusText = $"已设置基准点: 行 {row:0.###}, 列 {col:0.###}";
            ShowMainImage(false);
        }

        /// <summary>
        /// 弹出一个对话框，让用户手动输入位置跟随的基准点和当前点参数。
        /// </summary>
        private void ShowPositionFollowInputDialog()
        {
            using (Form form = new Form())
            {
                // 窗体样式设置
                form.Text = "位置跟随参数";
                form.Width = 360;
                form.Height = 290;
                form.StartPosition = FormStartPosition.CenterParent;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.MaximizeBox = false;
                form.MinimizeBox = false;
                form.BackColor = System.Drawing.Color.FromArgb(8, 16, 24);
                form.ForeColor = System.Drawing.Color.White;

                var table = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    Padding = new Padding(16),
                    ColumnCount = 2,
                    RowCount = 8
                };
                table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
                table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
                form.Controls.Add(table);

                // 创建输入框
                TextBox baseRow = AddFollowInput(table, 0, "基准行", FollowBaseRow);
                TextBox baseCol = AddFollowInput(table, 1, "基准列", FollowBaseCol);
                TextBox baseAngle = AddFollowInput(table, 2, "基准角度", FollowBaseAngleDeg);
                TextBox currentRow = AddFollowInput(table, 3, "当前行", FollowCurrentRow);
                TextBox currentCol = AddFollowInput(table, 4, "当前列", FollowCurrentCol);
                TextBox currentAngle = AddFollowInput(table, 5, "当前角度", FollowCurrentAngleDeg);

                // 按钮区域
                var footer = new FlowLayoutPanel { FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft, Dock = DockStyle.Fill };
                Button ok = new Button { Text = "确定", DialogResult = DialogResult.OK, Width = 82, Height = 28 };
                Button cancel = new Button { Text = "取消", DialogResult = DialogResult.Cancel, Width = 82, Height = 28 };
                footer.Controls.Add(ok);
                footer.Controls.Add(cancel);
                table.Controls.Add(footer, 0, 6);
                table.SetColumnSpan(footer, 2);
                form.AcceptButton = ok;
                form.CancelButton = cancel;

                if (form.ShowDialog(mainWindow) != DialogResult.OK) return;
                if (!TryReadFollowInput(baseRow, out double br) ||
                    !TryReadFollowInput(baseCol, out double bc) ||
                    !TryReadFollowInput(baseAngle, out double ba) ||
                    !TryReadFollowInput(currentRow, out double cr) ||
                    !TryReadFollowInput(currentCol, out double cc) ||
                    !TryReadFollowInput(currentAngle, out double ca))
                {
                    StatusText = "位置跟随参数输入错误";
                    return;
                }

                FollowBaseRow = br;
                FollowBaseCol = bc;
                FollowBaseAngleDeg = ba;
                FollowCurrentRow = cr;
                FollowCurrentCol = cc;
                FollowCurrentAngleDeg = ca;
                CommitToParams();
                StatusText = "位置跟随参数已更新";
                ShowMainImage(false);
            }
        }

        /// <summary>
        /// 辅助方法：在表格中添加一行带标签和文本框的输入控件。
        /// </summary>
        private static TextBox AddFollowInput(TableLayoutPanel table, int row, string name, double value)
        {
            var label = new Label
            {
                Text = name,
                Dock = DockStyle.Fill,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                ForeColor = System.Drawing.Color.FromArgb(190, 230, 244)
            };
            var text = new TextBox
            {
                Text = value.ToString("0.###"),
                Dock = DockStyle.Fill,
                BackColor = System.Drawing.Color.FromArgb(7, 17, 28),
                ForeColor = System.Drawing.Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
            table.Controls.Add(label, 0, row);
            table.Controls.Add(text, 1, row);
            return text;
        }

        private static bool TryReadFollowInput(TextBox textBox, out double value) => double.TryParse(textBox.Text, out value);

        // --------------------------- 绘图辅助方法 ---------------------------
        /// <summary>
        /// 在主显示区域绘制覆盖层：基准点、位置跟随 ROI 等。
        /// </summary>
        private void DrawMainOverlay()
        {
            if (mainDisplay == null || pluginParams == null) return;
            try
            {
                DrawBasePoint(mainDisplay, SelectedTemplate);
                DrawMatchFollowRois(mainDisplay);
                DrawPositionFollowLocalRois(mainDisplay);
            }
            catch { }
        }

        /// <summary>
        /// 在模板预览区域绘制模板轮廓和基准点。
        /// </summary>
        private void DrawTemplateOverlay()
        {
            if (templateDisplay == null || SelectedTemplate == null) return;
            try
            {
                if (SelectedTemplate.CreateContours != null && SelectedTemplate.CreateContours.IsInitialized())
                {
                    templateDisplay.HalconWindow.SetColor("cyan");
                    templateDisplay.HalconWindow.SetLineWidth(1);
                    templateDisplay.HalconWindow.DispObj(SelectedTemplate.CreateContours);
                }
                DrawBasePoint(templateDisplay, SelectedTemplate);
            }
            catch { }
        }

        /// <summary>
        /// 在匹配结果显示区域绘制匹配到的轮廓、十字标记、位置跟随 ROI 等。
        /// </summary>
        private void DrawMatchOverlay()
        {
            if (matchDisplay == null || pluginParams == null) return;
            try
            {
                if (!pluginParams.m_bRunSuccess)
                {
                    matchDisplay.HalconWindow.SetColor("red");
                    matchDisplay.HalconWindow.SetLineWidth(3);
                    matchDisplay.HalconWindow.SetTposition(36, 36);
                    matchDisplay.HalconWindow.WriteString("NG: " + GetMessage());
                    return;
                }

                // 绘制匹配轮廓（绿色）
                if (pluginParams.ho_OutContours != null && pluginParams.ho_OutContours.IsInitialized())
                {
                    matchDisplay.HalconWindow.SetColor("lime green");
                    matchDisplay.HalconWindow.SetLineWidth(1);
                    matchDisplay.HalconWindow.DispObj(pluginParams.ho_OutContours);
                }

                // 绘制每个匹配结果的十字和名称
                foreach (var result in Results)
                {
                    if (result == null) continue;
                    matchDisplay.HalconWindow.SetColor("yellow");
                    matchDisplay.HalconWindow.SetLineWidth(2);
                    matchDisplay.HalconWindow.DispCross(result.Row, result.Col, 80, 0);
                    matchDisplay.HalconWindow.SetColor("white");
                    matchDisplay.HalconWindow.SetTposition((int)(result.Row + 26), (int)(result.Col + 26));
                    matchDisplay.HalconWindow.WriteString(result.TemplateName);
                }

                DrawMatchFollowRois(matchDisplay);
                DrawPositionFollowLocalRois(matchDisplay);
            }
            catch { }
        }

        /// <summary>
        /// 绘制基准点（品红色十字 + 文字标签）。
        /// </summary>
        private static void DrawBasePoint(HalconViewRoiAdapter display, TemplateModelItemVm template)
        {
            if (display == null || template == null) return;
            display.HalconWindow.SetColor("magenta");
            display.HalconWindow.SetLineWidth(2);
            display.HalconWindow.DispCross(template.BaseRow, template.BaseCol, 90, 0);
            display.HalconWindow.SetColor("white");
            display.HalconWindow.SetTposition((int)(template.BaseRow + 18), (int)(template.BaseCol + 18));
            display.HalconWindow.WriteString("基准点");
        }

        /// <summary>
        /// 绘制匹配后跟随的 ROI（如果启用了 RoiFollowMatch）。
        /// </summary>
        private void DrawMatchFollowRois(HalconViewRoiAdapter display)
        {
            if (display == null || pluginParams == null || !pluginParams.m_bRunSuccess || !RoiFollowMatch) return;
            if (pluginParams.ho_OutMatchRoiRegion == null || !pluginParams.ho_OutMatchRoiRegion.IsInitialized()) return;
            display.HalconWindow.SetRgba(255, 165, 0, 128);
            display.HalconWindow.SetLineWidth(2);
            display.HalconWindow.DispObj(pluginParams.ho_OutMatchRoiRegion);
        }

        /// <summary>
        /// 绘制位置跟随的局部 ROI（如果启用了 UsePositionFollow 且匹配成功）。
        /// </summary>
        private void DrawPositionFollowLocalRois(HalconViewRoiAdapter display)
        {
            if (display == null || pluginParams == null || !pluginParams.m_bRunSuccess || !UsePositionFollow) return;
            foreach (var result in Results)
            {
                if (!result.IsSuccess || !result.HasFollowRoi) continue;
                display.HalconWindow.SetColor("cyan");
                display.HalconWindow.SetDraw("margin");
                display.HalconWindow.SetLineWidth(1);
                display.HalconWindow.DispRectangle1(result.FollowRow1, result.FollowCol1, result.FollowRow2, result.FollowCol2);
                display.HalconWindow.SetTposition((int)(result.FollowRow1 + 8), (int)(result.FollowCol1 + 8));
                display.HalconWindow.WriteString("位置跟随ROI");
            }
        }

        // --------------------------- 参数同步辅助方法 ---------------------------
        /// <summary>
        /// 确保 pluginParams 不为空，并返回有效的参数对象。
        /// </summary>
        private ParamsTemplateMatch EnsureParams()
        {
            if (pluginObject == null) pluginObject = new TemplateMatchObj();
            if (pluginObject.PluginParams == null) pluginObject.PluginParams = new ParamsTemplateMatch();
            pluginParams = pluginObject.PluginParams as ParamsTemplateMatch ?? new ParamsTemplateMatch();
            pluginObject.PluginParams = pluginParams;
            return pluginParams;
        }

        /// <summary>
        /// 从 pluginParams 中读取数据，更新 ViewModel 中的集合和属性。
        /// </summary>
        private void PullFromParams()
        {
            ParamsTemplateMatch p = EnsureParams();
            Templates.Clear();
            if (p.Templates != null)
            {
                foreach (var template in p.Templates)
                    Templates.Add(new TemplateModelItemVm(template));
            }
            if (Templates.Count == 0)
                Templates.Add(new TemplateModelItemVm { Name = "Template1", IsEnabled = true, IsCreated = false, RoiShapeType = p.m_nTemplateRoiShapeType });

            if (p.m_lMaskRoiParams == null) p.m_lMaskRoiParams = new List<MaskRoiParam>();
            BindMaskRoiOptions(new MaskRoiOptions
            {
                EnableMaskRoi = p.m_bUseMaskRoi,
                ShowMaskRoi = p.m_bShowMaskRoi,
                EditMaskRoi = p.m_bEditMaskRoi,
                SelectedIndex = GetMaskRoiSelectedIndex(p.m_lMaskRoiParams, selectedMaskRoiParam),
                MaskRois = p.m_lMaskRoiParams
            });
            SelectedMaskRoiParam = maskRoiOptions.MaskRois.Count > 0 ? maskRoiOptions.MaskRois[0] : null;

            SelectedTemplate = Templates.FirstOrDefault();
            PullResultsFromParams();
            // 触发所有相关属性的刷新
            OnPropertyChanged(nameof(CreateOptions));
            OnPropertyChanged(nameof(FindOptions));
            OnPropertyChanged(nameof(UseLocalRoi));
            OnPropertyChanged(nameof(ShowViewRoi));
            OnPropertyChanged(nameof(ShowTemplateRoi));
            OnPropertyChanged(nameof(ShowLocalRoi));
            OnPropertyChanged(nameof(EditTemplateRoi));
            OnPropertyChanged(nameof(EditLocalRoi));
            OnPropertyChanged(nameof(UseMaskRoi));
            OnPropertyChanged(nameof(ShowMaskRoi));
            OnPropertyChanged(nameof(EditMaskRoi));
            OnPropertyChanged(nameof(MaskRoiOptions));
            OnPropertyChanged(nameof(SelectedMaskRoiParam));
            OnPropertyChanged(nameof(TemplateRoiShapeType));
            OnPropertyChanged(nameof(LocalRoiShapeType));
            OnPropertyChanged(nameof(RoiFollowMatch));
            OnPropertyChanged(nameof(UsePositionFollow));
            OnPropertyChanged(nameof(FollowBaseRow));
            OnPropertyChanged(nameof(FollowBaseCol));
            OnPropertyChanged(nameof(FollowBaseAngleDeg));
            OnPropertyChanged(nameof(FollowCurrentRow));
            OnPropertyChanged(nameof(FollowCurrentCol));
            OnPropertyChanged(nameof(FollowCurrentAngleDeg));
            OnPropertyChanged(nameof(LocalRoiRow1));
            OnPropertyChanged(nameof(LocalRoiCol1));
            OnPropertyChanged(nameof(LocalRoiRow2));
            OnPropertyChanged(nameof(LocalRoiCol2));
            if (!isInitializingView) SyncMainRois();
        }

        /// <summary>
        /// 从 pluginParams.Results 中读取匹配结果到 Results 集合。
        /// </summary>
        private void PullResultsFromParams()
        {
            Results.Clear();
            if (pluginParams != null && pluginParams.Results != null)
            {
                foreach (var result in pluginParams.Results)
                    Results.Add(result);
            }
            SelectedResult = Results.FirstOrDefault();
        }

        /// <summary>
        /// 将运行后可能变化的模板状态（如模型 ID、是否已创建）从 pluginParams 同步到 ViewModel 中的模板项。
        /// </summary>
        private void SyncTemplatesFromParams()
        {
            if (pluginParams == null || pluginParams.Templates == null) return;
            foreach (var param in pluginParams.Templates)
            {
                var item = Templates.FirstOrDefault(t => ReferenceEquals(t.RuntimeParam, param) || string.Equals(t.Name, param.Name, StringComparison.OrdinalIgnoreCase));
                item?.CopyFrom(param);
            }
        }

        /// <summary>
        /// 将 ViewModel 中的模板列表和选中模板的参数提交回 pluginParams，以便保存或执行。
        /// </summary>
        private void CommitToParams()
        {
            ParamsTemplateMatch p = EnsureParams();
            p.Templates = Templates.Select(t => t.ToParam()).ToList();
            if (SelectedTemplate != null)
            {
                p.CreateOptions = SelectedTemplate.CreateOptions;
                p.FindOptions = SelectedTemplate.FindOptions;
            }
            if (p.m_lMaskRoiParams == null) p.m_lMaskRoiParams = new List<MaskRoiParam>();
            p.m_bUseMaskRoi = UseMaskRoi;
            p.m_bShowMaskRoi = ShowMaskRoi;
            p.m_bEditMaskRoi = EditMaskRoi;
        }

        /// <summary>
        /// 同步主显示区域中的 ROI 对象（模板 ROI、局部 ROI）到 Halcon 控件。
        /// </summary>
        private void SyncMainRois()
        {
            if (mainDisplay == null || pluginParams == null) return;
            if (isSyncingRoi) return;
            try
            {
                isSyncingRoi = true;
                mainDisplay.SyncTemplateAndLocalRois(pluginParams, SelectedTemplate?.ToParam(), SelectedMaskRoiParam);
            }
            finally
            {
                isSyncingRoi = false;
            }
        }

        /// <summary>
        /// 刷新当前图像的尺寸显示文本。
        /// </summary>
        private void RefreshImageSizeText()
        {
            if (!HasValidImage(pluginParams?.ho_Image))
            {
                ImageSizeText = "图像大小: -";
                return;
            }
            HOperatorSet.GetImageSize(pluginParams.ho_Image, out HTuple width, out HTuple height);
            ImageSizeText = $"图像大小: {width.I} x {height.I}";
        }

        /// <summary>
        /// 获取运行时的错误消息。
        /// </summary>
        private string GetMessage() => pluginParams == null ? string.Empty : pluginParams.m_strRunMessage;

        /// <summary>
        /// 安全释放 HalconViewRoiAdapter 对象。
        /// </summary>
        private void DisposeDisplay(ref HalconViewRoiAdapter display)
        {
            if (display == null) return;
            DetachDisplayEvents(display);
            display.Dispose();
            display = null;
        }

        private void DetachDisplayEvents(HalconViewRoiAdapter display)
        {
            if (display == null) return;
            if (ReferenceEquals(display, mainDisplay))
            {
                display.RoiChanged -= OnMainRoiChanged;
                display.MouseMoved -= OnMainMouseMoved;
                display.RightMouseDown -= OnMainRightMouseDown;
                display.MiddleMouseDown -= OnMainMiddleMouseDown;
                display.Redraw -= DrawMainOverlay;
            }
            if (ReferenceEquals(display, templateDisplay))
            {
                display.Redraw -= DrawTemplateOverlay;
            }
            if (ReferenceEquals(display, matchDisplay))
            {
                display.Redraw -= DrawMatchOverlay;
            }
        }

        private void DetachTemplateEvents()
        {
            foreach (TemplateModelItemVm template in Templates)
            {
                if (template != null) template.PropertyChanged -= OnSelectedTemplatePropertyChanged;
            }
        }

        private void ReleaseViewReferences()
        {
            DetachTemplateEvents();
            if (subscribedMaskRoiOptions != null)
            {
                subscribedMaskRoiOptions.PropertyChanged -= OnMaskRoiOptionsPropertyChanged;
                subscribedMaskRoiOptions = null;
            }
            if (selectedMaskRoiParam != null)
            {
                selectedMaskRoiParam.PropertyChanged -= OnSelectedMaskRoiParamPropertyChanged;
                selectedMaskRoiParam = null;
            }

            selectedTemplate = null;
            selectedResult = null;
            maskRoiOptions = new MaskRoiOptions();

            Templates.Clear();
            Results.Clear();

            pluginParams = null;
            pluginObject = null;

            OnPropertyChanged(nameof(SelectedTemplate));
            OnPropertyChanged(nameof(SelectedResult));
            OnPropertyChanged(nameof(SelectedMaskRoiParam));
            OnPropertyChanged(nameof(MaskRoiOptions));
            OnPropertyChanged(nameof(Templates));
            OnPropertyChanged(nameof(Results));
        }

        /// <summary>
        /// 尝试将焦点设置到 Halcon 窗口控件。
        /// </summary>
        private static void FocusWindow(HWindowControl window)
        {
            try { window.Focus(); } catch { }
        }

        /// <summary>
        /// 判断 HObject 是否为有效的图像对象。
        /// </summary>
        private static bool HasValidImage(HObject source)
        {
            if (source == null || !source.IsInitialized()) return false;
            try
            {
                HOperatorSet.CountObj(source, out HTuple count);
                if (count.I <= 0) return false;
                HOperatorSet.GetObjClass(source, out HTuple objClass);
                return string.Equals(objClass.S, "image", StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }
    }
}

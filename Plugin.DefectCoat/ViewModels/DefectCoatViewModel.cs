using DefectRuleClassifier.Core.Models;
using DefectRuleClassifier.Core.Classifiers;
using DefectRuleClassifier.Core.Features;
using Plugin.DefectCoat.Models;
using Plugin.DefectCoat.Services;
using Plugin.DefectCoat.Views;
using DefectRuleClassifier.Views;
using DefectRuleClassifier.ViewModels;
using Frame_Core;
using HalconDotNet;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using WinForms = System.Windows.Forms;
using Frame_Core.pluginParams;

using FrameCommunity.公共类;

namespace Plugin.DefectCoat.ViewModels
{
    /// <summary>
    /// 涂布缺陷检测插件页面的 ViewModel。
    /// 主界面只负责参数绑定、ROI 同步、结果显示和宿主生命周期转发；
    /// 检测算法、分类调度和输出对象回写统一交给 DefectCoatObj/DefectDetectPipeline。
    /// </summary>
    public sealed class DefectCoatViewModel : BindableBase, IPluginWpfView
    {
        /// <summary>当前页面绑定的插件对象。独立调试时默认创建；接入框架时由 BindPlugin 替换为宿主传入对象。</summary>
        private DefectCoatObj plugin = new DefectCoatObj();
        /// <summary>主图 HALCON/ViewROI 适配器，只负责显示、拖拽 ROI 和读取 Rectangle1 四坐标，不承载检测算法。</summary>
        private HalconViewRoiAdapter halconView;
        /// <summary>NG Region 预览窗口的 HALCON 控件引用，生命周期由 WPF 页面创建，ViewModel 只做显示适配。</summary>
        private HWindowControl regionWindow;
        /// <summary>NG 小图预览窗口的 HALCON 控件引用，用于显示当前选中缺陷的裁剪图。</summary>
        private HWindowControl smallWindow;
        /// <summary>预处理/阈值叠加预览窗口的 HALCON 控件引用，用于查看灰度归一化和亮暗区域结果。</summary>
        private HWindowControl preprocessWindow;
        /// <summary>NG Region 预览适配器；内部会复制显示用 HObject，Dispose 时只释放自己的显示副本。</summary>
        private HalconPreviewAdapter regionPreview;
        /// <summary>NG 小图预览适配器；显示当前缺陷 CropImage 的副本，不释放业务结果对象。</summary>
        private HalconPreviewAdapter smallPreview;
        /// <summary>预处理图预览适配器；用于显示 ho_PreprocessImage 和分类叠加区域。</summary>
        private HalconPreviewAdapter preprocessPreview;
        /// <summary>主界面状态栏文本，仅用于界面提示，不参与参数保存。</summary>
        private string statusText = "等待导入图像";
        /// <summary>主图鼠标所在位置、灰度等信息文本，由 HalconViewRoiAdapter 鼠标事件刷新。</summary>
        private string mouseInfo = "图像:-  行:--  列:--  灰度:-";
        /// <summary>预处理预览窗口鼠标信息文本，由 preprocessPreview 鼠标事件刷新。</summary>
        private string preprocessMouseInfo = "预处理图:-  行:--  列:--  灰度:-";
        /// <summary>当前输入图像尺寸显示文本，只用于界面显示，不保存到 XML。</summary>
        private string imageInfo = "图像大小(W*H): --";
        /// <summary>当前手动导入图像的文件名；正式宿主运行时可为空或保持上次调试值。</summary>
        private string imageName = "--";
        /// <summary>当前正在编辑的局部 ROI 或屏蔽 ROI 的 Rectangle1 四坐标摘要。</summary>
        private string roiInfo = "ROI: --";
        /// <summary>分类规则页当前选中的 NG 类别，影响特征条件编辑和命令可用性。</summary>
        private NgTypeRule selectedNgType;
        /// <summary>分类规则页当前选中的特征规则，切换时同步特征类型下拉框。</summary>
        private FeatureRule selectedFeature;
        /// <summary>检测结果表当前选中的缺陷结果，驱动 Region/小图/特征明细预览。</summary>
        private NgDefectResult selectedResult;
        /// <summary>特征摘要表当前选中的行，与 SelectedResult 按缺陷 Index 双向联动。</summary>
        private DefectFeatureSummaryRow selectedFeatureRow;
        /// <summary>特征规则编辑区当前选中的特征类型选项，用于把 UI 选择写回 SelectedFeature.FeatureType。</summary>
        private FeatureTypeOption selectedFeatureOption;
        /// <summary>屏蔽 ROI 列表当前选中项；EditRole 为 Mask 时它就是参数面板和 ViewROI 的编辑目标。</summary>
        private CoatRoiParamViewModel selectedMaskRoi;
        /// <summary>分类规则设置窗口实例缓存，避免重复打开多个规则窗口。</summary>
        private RuleSettingsWindow ruleSettingsWindow;
        /// <summary>分类结果窗口实例缓存，重复点击时复用窗口并刷新 DataContext。</summary>
        private ClassifyResultWindow classifyResultWindow;
        /// <summary>界面调试用的最近一次手动加载图像路径，不写入 ParamsDefectCoat，正式插件运行只依赖 Params.ho_Image。</summary>
        private string loadedImagePath;
        /// <summary>最近一次检测耗时显示文本，只用于界面状态栏，不写入参数文件。</summary>
        private string runElapsedText = "耗时：-- ms";

        /// <summary>
        /// 构造界面 ViewModel 的默认参数、绑定集合和命令。
        /// 注意：这里创建的 plugin 只用于独立调试；接入宿主后必须通过 BindPlugin 绑定真实插件对象。
        /// </summary>
        public DefectCoatViewModel()
        {
            Params = plugin.Params;
            Results = new ObservableCollection<NgDefectResult>();
            ClassifyResults = new ObservableCollection<NgDefectResult>();
            MaskRoiItems = new ObservableCollection<CoatRoiParamViewModel>();
            PartitionItems = new ObservableCollection<CoatPartitionParamViewModel>();
            FeatureRows = new ObservableCollection<DefectFeatureSummaryRow>();
            FeatureDetailRows = new ObservableCollection<FeatureValueRow>();
            DebugLogs = new ObservableCollection<string>();
            PartitionDirections = new ObservableCollection<PartitionDirectionOption>
            {
                new PartitionDirectionOption { Value = PartitionSplitDirection.LongSide, DisplayName = "长向分段" },
                new PartitionDirectionOption { Value = PartitionSplitDirection.WideSide, DisplayName = "宽向分段" }
            };
            FeatureTypes = new ObservableCollection<FeatureTypeOption>(FeatureCatalog.All);
            JudgeConditionModes = new ObservableCollection<JudgeConditionModeOption>
            {
                new JudgeConditionModeOption { Value = JudgeConditionMode.AllConditionAnd, DisplayName = "所有条件且", Remark = "长度、宽度、面积全部通过" },
                new JudgeConditionModeOption { Value = JudgeConditionMode.AllConditionOr, DisplayName = "所有条件或", Remark = "长度、宽度、面积任意一项通过" },
                new JudgeConditionModeOption { Value = JudgeConditionMode.LengthAndWidthOrArea, DisplayName = "(长&&宽)||面积", Remark = "长宽同时通过，或者面积通过" },
                new JudgeConditionModeOption { Value = JudgeConditionMode.LengthOrWidthAndArea, DisplayName = "(长||宽)&&面积", Remark = "长度或宽度任意一项通过，并且面积通过" }
            };
      
            LoadImageCommand = new RelayCommand(LoadImage);
            DrawRoiCommand = new RelayCommand(DrawRoi);
            GetRoiDataCommand = new RelayCommand(GetRoiData);
            DetectCommand = new RelayCommand(Detect);
            ShowClassifyCommand = new RelayCommand(ShowClassifyResult);
            ShowRuleSettingsCommand = new RelayCommand(ShowRuleSettings);
            AddNgTypeCommand = new RelayCommand(AddNgType);
            DeleteNgTypeCommand = new RelayCommand(DeleteNgType, () => SelectedNgType != null);
            MoveNgUpCommand = new RelayCommand(() => MoveNgType(-1), () => SelectedNgType != null);
            MoveNgDownCommand = new RelayCommand(() => MoveNgType(1), () => SelectedNgType != null);
            AddFeatureCommand = new RelayCommand(AddFeature, () => SelectedNgType != null);
            DeleteFeatureCommand = new RelayCommand(DeleteFeature, () => SelectedFeature != null);
            AddJudgeConditionCommand = new RelayCommand(AddJudgeCondition, () => SelectedNgType != null);
            DeleteJudgeConditionCommand = new RelayCommand(DeleteJudgeCondition, () => SelectedNgType != null);
            SortFeatureRowsCommand = new RelayCommand(SortFeatureRows);
            GetAllFeaturesCommand = new RelayCommand(GetAllFeaturesForSelected, () => SelectedResult != null);
            ClearResultsCommand = new RelayCommand(ClearResults);
            SaveParamsCommand = new RelayCommand(SaveParams);
            LoadParamsCommand = new RelayCommand(LoadParams);
            SelectFeatureRulePathCommand = new RelayCommand(SelectFeatureRulePath);
            AddMaskRoiCommand = new RelayCommand(AddMaskRoi);
            DeleteMaskRoiCommand = new RelayCommand(DeleteMaskRoi, () => SelectedMaskRoi != null);
            RebuildPartitionsCommand = new RelayCommand(RebuildPartitions);
            NormalizePartitionsCommand = new RelayCommand(NormalizePartitions);
            AiPlaceholderCommand = new RelayCommand(() =>
            {
                Params.EnableAiClassify = true;
                plugin.InitObj();
                AddDebug("AI分类接口已预留：InitObj已初始化占位对象，真实模型路径后续放入AI独立参数类。");
                StatusText = "AI分类接口已预留：当前占位对象不执行真实推理。";
            });

            plugin.InitObj();
            OnPropertyChanged(nameof(Params));
            selectedNgType = FeatureRuleConfig.NgTypes.FirstOrDefault();
            selectedFeature = selectedNgType == null ? null : selectedNgType.Features.FirstOrDefault();
            selectedFeatureOption = selectedFeature == null ? FeatureTypes.FirstOrDefault() : FeatureCatalog.Get(selectedFeature.FeatureType);
            UpdateRoiInfo();
            AddDebug("模块初始化完成。");
        }

        /// <summary>当前界面绑定的强类型参数。BindPlugin 或读取参数后会保持与 plugin.PluginParams 同步。</summary>
        public ParamsDefectCoat Params { get; private set; }
        /// <summary>提供给宿主框架读取的插件对象引用，始终返回当前绑定的 DefectCoatObj。</summary>
        public IPlugin PluginObject => plugin;
        /// <summary>提供给宿主框架读取的参数基类引用，实际类型应为 ParamsDefectCoat。</summary>
        public PluginParamsBase PluginParams => plugin.PluginParams;
        /// <summary>主检测结果表数据源。集合项持有 HObject 结果引用，清空或释放前必须先解除选中引用。</summary>
        public ObservableCollection<NgDefectResult> Results { get; }
        /// <summary>分类结果窗口数据源。与 Params.NgResults 同步，不单独拥有结果对象释放权。</summary>
        public ObservableCollection<NgDefectResult> ClassifyResults { get; }
        /// <summary>屏蔽 ROI 界面适配集合；每一项包装 Params.MaskRois 中的一个 CoatRoiParam。</summary>
        public ObservableCollection<CoatRoiParamViewModel> MaskRoiItems { get; }
        /// <summary>分区参数界面适配集合；每一项包装 Params.Partitions 中的一个 CoatPartitionParam。</summary>
        public ObservableCollection<CoatPartitionParamViewModel> PartitionItems { get; }
        /// <summary>局部检测 ROI 的界面适配对象，包装 Params.LocalRoi。</summary>
        public CoatRoiParamViewModel LocalRoiItem { get; private set; }
        /// <summary>结果表的常用特征摘要集合，优先展示面积、长宽、灰度等常查字段。</summary>
        public ObservableCollection<DefectFeatureSummaryRow> FeatureRows { get; }
        /// <summary>当前选中缺陷的完整特征明细集合，点击“获取全部特征”后会补齐更多特征。</summary>
        public ObservableCollection<FeatureValueRow> FeatureDetailRows { get; }
        /// <summary>界面调试日志集合，只用于排查流程，不写入参数文件。</summary>
        public ObservableCollection<string> DebugLogs { get; }
        /// <summary>分区方向候选项，决定按局部 ROI 长向还是宽向切分。</summary>
        public ObservableCollection<PartitionDirectionOption> PartitionDirections { get; }
        /// <summary>可选特征类型目录，由分类规则模块 FeatureCatalog 提供。</summary>
        public ObservableCollection<FeatureTypeOption> FeatureTypes { get; }
        /// <summary>长宽面积判定条件模式候选项，用于规则分类的二次判定。</summary>
        public ObservableCollection<JudgeConditionModeOption> JudgeConditionModes { get; }
        /// <summary>特征值规则分类配置对象，由插件对象持有并负责保存。</summary>
        public FeatureRuleClassifierConfig FeatureRuleConfig => plugin.FeatureRuleConfig;
        /// <summary>传统模型分类配置对象，由插件对象持有并负责保存。</summary>
        public ModelClassifierConfig ModelClassifierConfig => plugin.ModelClassifierConfig;
        /// <summary>手动导入测试图像命令；正式宿主流程通常直接写入 Params.ho_Image。</summary>
        public ICommand LoadImageCommand { set; get; }
        /// <summary>刷新/创建当前编辑 ROI 的命令，不做检测，只同步 ViewROI 显示。</summary>
        public ICommand DrawRoiCommand { set; get; }
        /// <summary>从 ViewROI 拖拽框回读 Rectangle1 四坐标到当前编辑 ROI 的命令。</summary>
        public ICommand GetRoiDataCommand { set; get; }
        /// <summary>界面手动执行检测命令，内部仍通过 DefectCoatObj.ExecuteObj 进入主逻辑。</summary>
        public ICommand DetectCommand { set; get; }
        /// <summary>打开分类结果窗口命令，按当前 LastRunResult 重新执行分类结果刷新。</summary>
        public ICommand ShowClassifyCommand { set; get; }
        /// <summary>打开规则配置窗口命令，规则保存后会重新初始化分类对象。</summary>
        public ICommand ShowRuleSettingsCommand { set; get; }
        /// <summary>新增 NG 类型命令，操作的是 FeatureRuleConfig.NgTypes。</summary>
        public ICommand AddNgTypeCommand { set; get; }
        /// <summary>删除当前 NG 类型命令，仅在 SelectedNgType 非空时可用。</summary>
        public ICommand DeleteNgTypeCommand { set; get; }
        /// <summary>上移当前 NG 类型优先级命令，列表越靠前匹配优先级越高。</summary>
        public ICommand MoveNgUpCommand { set; get; }
        /// <summary>下移当前 NG 类型优先级命令，列表越靠后匹配优先级越低。</summary>
        public ICommand MoveNgDownCommand { set; get; }
        /// <summary>给当前 NG 类型新增特征条件命令。</summary>
        public ICommand AddFeatureCommand { set; get; }
        /// <summary>删除当前特征条件命令，仅在 SelectedFeature 非空时可用。</summary>
        public ICommand DeleteFeatureCommand { set; get; }
        /// <summary>启用当前 NG 类型长宽面积判定条件命令。</summary>
        public ICommand AddJudgeConditionCommand { set; get; }
        /// <summary>关闭当前 NG 类型长宽面积判定条件命令。</summary>
        public ICommand DeleteJudgeConditionCommand { set; get; }
        /// <summary>按面积降序重排特征摘要显示命令，不改变 Params.NgResults 原始顺序。</summary>
        public ICommand SortFeatureRowsCommand { set; get; }
        /// <summary>为当前选中缺陷补算完整特征命令，用于规则选型调试。</summary>
        public ICommand GetAllFeaturesCommand { set; get; }
        /// <summary>清空界面结果和插件运行输出命令，会释放插件拥有的输出 HObject。</summary>
        public ICommand ClearResultsCommand { set; get; }
        /// <summary>手动保存参数命令，会先同步 ROI/分区适配集合到 Params。</summary>
        public ICommand SaveParamsCommand { set; get; }
        /// <summary>手动读取参数命令，只替换算法参数，不随意释放宿主输入图像。</summary>
        public ICommand LoadParamsCommand { set; get; }
        /// <summary>选择特征规则文件路径命令，分类器参数路径会随规则路径自动维护。</summary>
        public ICommand SelectFeatureRulePathCommand { set; get; }
        /// <summary>AI 分类占位命令，用于验证配置入口，不执行真实 AI 推理。</summary>
        public ICommand AiPlaceholderCommand { set; get; }
        /// <summary>新增屏蔽 ROI 命令，新 ROI 默认基于局部 ROI 中心生成 Rectangle1。</summary>
        public ICommand AddMaskRoiCommand { set; get; }
        /// <summary>删除当前屏蔽 ROI 命令，删除后会重排 Index/Name 并刷新 ViewROI。</summary>
        public ICommand DeleteMaskRoiCommand { set; get; }
        /// <summary>按当前局部 ROI 和分区数量重建分区命令。</summary>
        public ICommand RebuildPartitionsCommand { set; get; }
        /// <summary>归一化分区长度命令，负责像素/mm 换算和固定长度分配。</summary>
        public ICommand NormalizePartitionsCommand { set; get; }

        /// <summary>状态栏显示文本，所有长流程入口都会更新它，便于现场快速判断当前步骤。</summary>
        public string StatusText { get => statusText; set => SetProperty(ref statusText, value); }
        /// <summary>主图鼠标信息文本，来自主 HALCON 窗口坐标和灰度读取。</summary>
        public string MouseInfo { get => mouseInfo; set => SetProperty(ref mouseInfo, value); }
        /// <summary>预处理预览窗口鼠标信息文本，文字前缀与主图区分开。</summary>
        public string PreprocessMouseInfo { get => preprocessMouseInfo; set => SetProperty(ref preprocessMouseInfo, value); }
        /// <summary>图像尺寸文本，单位为像素，来源于 HALCON GetImageSize。</summary>
        public string ImageInfo { get => imageInfo; set => SetProperty(ref imageInfo, value); }
        /// <summary>当前调试图像文件名。宿主运行传图时不一定有文件名。</summary>
        public string ImageName { get => imageName; set => SetProperty(ref imageName, value); }
        /// <summary>当前编辑 ROI 的四坐标摘要，跟随 EditRole 在局部 ROI 和屏蔽 ROI 之间切换。</summary>
        public string RoiInfo { get => roiInfo; set => SetProperty(ref roiInfo, value); }
        /// <summary>本次手动执行或宿主执行耗时显示文本，不写入参数。</summary>
        public string RunElapsedText { get => runElapsedText; set => SetProperty(ref runElapsedText, value); }
        /// <summary>
        /// 当前 ROI 编辑角色。
        /// Local 表示参数面板和 ViewROI 操作 Params.LocalRoi；Mask 表示操作 SelectedMaskRoi.Model。
        /// </summary>
        public RoiEditRole EditRole
        {
            get => Params.EditRole;
            set
            {
                if (Params.EditRole == value) return;
                Params.EditRole = value;
                if (Params.EditRole == RoiEditRole.Mask && SelectedMaskRoi == null && MaskRoiItems.Count > 0)
                    SelectedMaskRoi = MaskRoiItems[0];
                SyncRectangle1FieldsFromEditRoi();
                SyncViewFromEditRoi();
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// 是否显示主图 ViewROI 控制框。
        /// 只影响界面上的 ROI 框显示和拖拽，不清空已经保存的 Rectangle1 四坐标。
        /// </summary>
        public bool ShowViewRoi
        {
            get => Params.ShowViewRoi;
            set
            {
                if (Params.ShowViewRoi == value) return;
                Params.ShowViewRoi = value;
                OnPropertyChanged();

                if (value)
                {
                    halconView?.RefreshFromParams(Params, false);
                    SyncViewFromEditRoi();
                }
                else
                {
                    halconView?.SetViewRoiVisible(false);
                }
            }
        }
        /// <summary>当前编辑 Rectangle1 的 Row1。它不是独立字段，读写会直接落到局部 ROI 或选中屏蔽 ROI。</summary>
        public double EditRoiRow1 { get => GetEditRoiValue(item => item.Row1); set => SetEditRoiValue(value, (item, newValue) => item.Row1 = newValue); }
        /// <summary>当前编辑 Rectangle1 的 Col1。它不是独立字段，读写会直接落到局部 ROI 或选中屏蔽 ROI。</summary>
        public double EditRoiCol1 { get => GetEditRoiValue(item => item.Col1); set => SetEditRoiValue(value, (item, newValue) => item.Col1 = newValue); }
        /// <summary>当前编辑 Rectangle1 的 Row2。它不是独立字段，读写会直接落到局部 ROI 或选中屏蔽 ROI。</summary>
        public double EditRoiRow2 { get => GetEditRoiValue(item => item.Row2); set => SetEditRoiValue(value, (item, newValue) => item.Row2 = newValue); }
        /// <summary>当前编辑 Rectangle1 的 Col2。它不是独立字段，读写会直接落到局部 ROI 或选中屏蔽 ROI。</summary>
        public double EditRoiCol2 { get => GetEditRoiValue(item => item.Col2); set => SetEditRoiValue(value, (item, newValue) => item.Col2 = newValue); }

        /// <summary>当前编辑 Rectangle1 的 TopOffsetRow。它不是独立字段，读写会直接落到局部 ROI 或选中屏蔽 ROI。</summary>
        public double EditTopOffsetRow { get => GetEditRoiValue(item => item.TopOffsetRow); set => SetEditRoiValue(value, (item, newValue) => item.TopOffsetRow = newValue); }


        /// <summary>当前编辑 Rectangle1 的 BottomOffsetRow。它不是独立字段，读写会直接落到局部 ROI 或选中屏蔽 ROI。</summary>
        public double EditBottomOffsetRow { get => GetEditRoiValue(item => item.BottomOffsetRow); set => SetEditRoiValue(value, (item, newValue) => item.BottomOffsetRow = newValue); }

        /// <summary>当前编辑 Rectangle1 的 LeftOffsetCol。它不是独立字段，读写会直接落到局部 ROI 或选中屏蔽 ROI。</summary>
        public double EditLeftOffsetCol { get => GetEditRoiValue(item => item.LeftOffsetCol); set => SetEditRoiValue(value, (item, newValue) => item.LeftOffsetCol = newValue); }


        /// <summary>当前编辑 Rectangle1 的 LeftOffsetCol。它不是独立字段，读写会直接落到局部 ROI 或选中屏蔽 ROI。</summary>
        public double EditRightOffsetCol { get => GetEditRoiValue(item => item.RightOffsetCol); set => SetEditRoiValue(value, (item, newValue) => item.RightOffsetCol = newValue); }



        /// <summary>分区方向上的局部 ROI 总长度，同时显示像素和毫米换算值。</summary>
        public string PartitionTotalLengthText => string.Format("{0:F2}px / {1:F4}mm", GetPartitionTotalLength(), GetPartitionTotalLength() * GetPartitionPixelUnit());
        /// <summary>分区固定边长度显示文本；长向切分时固定宽度，宽向切分时固定高度。</summary>
        public string PartitionFixedWidthText
        {
            get
            {
                CoatRoiParam roi = Params.LocalRoi;
                double widthPx = Params.PartitionDirection == PartitionSplitDirection.WideSide ? GetRoiHeight(roi) : GetRoiWidth(roi);
                double unit = Params.PartitionDirection == PartitionSplitDirection.WideSide
                    ? (Params.PixelToMmV <= 0 ? 1 : Params.PixelToMmV)
                    : (Params.PixelToMmH <= 0 ? 1 : Params.PixelToMmH);
                return string.Format("{0:F2}px / {1:F4}mm", Math.Max(1, widthPx), Math.Max(1, widthPx) * unit);
            }
        }
        /// <summary>NG Region 预览标题，随 SelectedResult 改变。</summary>
        public string RegionPreviewTitle => SelectedResult == null ? "NG Region：未选择缺陷" : string.Format("NG Region：#{0} {1} {2}", SelectedResult.Index, SelectedResult.ClassName, SelectedResult.Polarity);
        /// <summary>NG 小图预览标题，随 SelectedResult 改变。</summary>
        public string SmallPreviewTitle => SelectedResult == null ? "NG小图：未选择缺陷" : string.Format("NG小图：#{0} {1}", SelectedResult.Index, SelectedResult.ClassName);
        /// <summary>预处理预览固定标题，说明叠加颜色含义。</summary>
        public string PreprocessPreviewTitle => "预处理/阈值图：缩放图 + 暗区(蓝) + 亮区(黄) + NG(红)";
        /// <summary>
        /// 当前选中的屏蔽 ROI。
        /// 切换时会刷新参数面板四坐标，并在 Mask 编辑模式下同步主图 ViewROI。
        /// </summary>
        public CoatRoiParamViewModel SelectedMaskRoi
        {
            get => selectedMaskRoi;
            set
            {
                if (SetProperty(ref selectedMaskRoi, value))
                {
                    (DeleteMaskRoiCommand as RelayCommand)?.RaiseCanExecuteChanged();
                    if (value != null && Params.EditRole == RoiEditRole.Mask)
                    {
                        NormalizeRoiDerivedFields(value.Model);
                        UpdateRoiInfo();
                    }
                    RefreshEditRoiBindings();
                    if (value != null && Params.EditRole == RoiEditRole.Mask)
                        SyncViewFromEditRoi();
                }
            }
        }
        /// <summary>当前选中的 NG 类型。改变后会刷新特征规则选中项和命令可用状态。</summary>
        public NgTypeRule SelectedNgType
        {
            get => selectedNgType;
            set
            {
                if (SetProperty(ref selectedNgType, value))
                {
                    SelectedFeature = selectedNgType == null ? null : selectedNgType.Features.FirstOrDefault();
                    RaiseRuleCommands();
                }
            }
        }

        /// <summary>当前选中的特征规则。改变后同步特征类型下拉选项。</summary>
        public FeatureRule SelectedFeature
        {
            get => selectedFeature;
            set
            {
                if (SetProperty(ref selectedFeature, value))
                {
                    SelectedFeatureOption = selectedFeature == null ? FeatureTypes.FirstOrDefault() : FeatureCatalog.Get(selectedFeature.FeatureType);
                    RaiseRuleCommands();
                }
            }
        }

        /// <summary>当前特征类型选项。界面选择变化会写回 SelectedFeature.FeatureType。</summary>
        public FeatureTypeOption SelectedFeatureOption
        {
            get => selectedFeatureOption;
            set
            {
                if (SetProperty(ref selectedFeatureOption, value) && value != null && SelectedFeature != null)
                    SelectedFeature.FeatureType = value.Value;
            }
        }

        /// <summary>当前选中的缺陷结果。改变后刷新预览图、特征明细和“获取全部特征”命令状态。</summary>
        public NgDefectResult SelectedResult
        {
            get => selectedResult;
            set
            {
                if (SetProperty(ref selectedResult, value))
                {
                    if (value != null && (selectedFeatureRow == null || selectedFeatureRow.DefectIndex != value.Index))
                        SelectedFeatureRow = FeatureRows.FirstOrDefault(item => item.DefectIndex == value.Index);
                    OnPropertyChanged(nameof(RegionPreviewTitle));
                    OnPropertyChanged(nameof(SmallPreviewTitle));
                    RefreshFeatureDetailRows();
                    RefreshSelectedPreview();
                    (GetAllFeaturesCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        /// <summary>当前选中的特征摘要行。表格选行时按 DefectIndex 反向定位 SelectedResult。</summary>
        public DefectFeatureSummaryRow SelectedFeatureRow
        {
            get => selectedFeatureRow;
            set
            {
                if (SetProperty(ref selectedFeatureRow, value) && value != null)
                    SelectedResult = Results.FirstOrDefault(item => item.Index == value.DefectIndex);
            }
        }

        /// <summary>
        /// 框架把 MEF 创建的插件对象绑定到页面时调用。
        /// 页面不重新创建参数，只引用宿主插件已有的 PluginParams，保证保存、运行、输出都落在同一个对象上。
        /// </summary>
        public void BindPlugin(IPlugin pluginObject)
        {
            DefectCoatObj typed = pluginObject as DefectCoatObj;
            if (typed == null)
            {
                StatusText = "绑定插件失败：传入对象不是 DefectCoatObj。";
                AddDebug(StatusText);
                return;
            }

            if (!ReferenceEquals(plugin, typed))
                plugin.ReleaseOutputs();

            plugin = typed;
            if (!(plugin.PluginParams is ParamsDefectCoat))
                plugin.SetParams(new ParamsDefectCoat());

            Params = plugin.Params;
            plugin.InitObj();
            RebuildUiAdaptersFromParams();
            RefreshBindings();
            AddDebug("已绑定宿主插件对象。");
        }

        /// <summary>
        /// 从插件参数刷新界面绑定和 ROI 显示。
        /// 宿主加载 XML 参数、外部流程运行后都会调用该入口。
        /// </summary>
        public void RefreshBindings()
        {
            OnPropertyChanged(nameof(Params));
            OnPropertyChanged(nameof(PluginParams));
            OnPropertyChanged(nameof(FeatureRuleConfig));
            OnPropertyChanged(nameof(ModelClassifierConfig));
            OnPropertyChanged(nameof(ShowViewRoi));
            SyncRectangle1FieldsFromEditRoi();
            UpdateRoiInfo();
            halconView?.RefreshFromParams(Params, Params.ho_Image != null && Params.ho_Image.IsInitialized());
            SyncViewFromEditRoi();
            RefreshResultCollectionsFromParams();
        }

        /// <summary>
        /// 宿主点击“执行”入口。
        /// 先同步当前 ROI 参数，再调用插件对象 ExecuteObj，最后刷新结果表和 HALCON 叠加显示。
        /// </summary>
        public PluginViewActionResult RunFromHost()
        {
            Stopwatch watch = Stopwatch.StartNew();
            try
            {
                Detect();
                watch.Stop();
                return Params.m_bRunSuccess
                    ? PluginViewActionResult.Ok(Params.m_strRunMessage, watch.ElapsedMilliseconds)
                    : PluginViewActionResult.Fail(Params.m_strRunMessage, watch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                watch.Stop();
                StatusText = "宿主执行失败：" + ex.Message;
                AddDebug(StatusText);
                return PluginViewActionResult.Fail(StatusText, watch.ElapsedMilliseconds);
            }
        }

        /// <summary>
        /// 宿主点击“保存”入口。
        /// 保存顺序固定为：分类规则独立参数 -> 主检测参数，避免主参数保存成功但规则 XML 未更新。
        /// </summary>
        public PluginViewActionResult SaveFromHost(string filePath)
        {
            Stopwatch watch = Stopwatch.StartNew();
            try
            {
                BeforeSave();
                //if (string.IsNullOrWhiteSpace(filePath))
                //    filePath = Path.Combine(DefaultConfigPathService.GetModuleConfigDirectory(), "ParamsDefectCoat.xml");

                //if (!filePath.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                //    filePath += ".xml";

                EnsureRuleConfigPathForParamFile(filePath);
                SaveCurrentRuleConfig();
                plugin.moduleInfo.filePath = filePath;
                plugin.Params.filePath = filePath;/*Path.ChangeExtension(filePath, null);*/
                bool success = plugin.saveParams();
                AfterSave(success);
                watch.Stop();
                return success
                    ? PluginViewActionResult.Ok("涂布缺陷检测参数保存成功。", watch.ElapsedMilliseconds)
                    : PluginViewActionResult.Fail("涂布缺陷检测参数保存失败。", watch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                watch.Stop();
                AfterSave(false);
                StatusText = "宿主保存失败：" + ex.Message;
                AddDebug(StatusText);
                return PluginViewActionResult.Fail(StatusText, watch.ElapsedMilliseconds);
            }
        }

        /// <summary>
        /// 宿主关闭插件可视化页面入口。
        /// 这里只释放页面显示资源，不释放检测输出；检测输出由下次 ExecuteObj 前或插件对象 DisPoseObj 时释放。
        /// </summary>
        public void CloseFromHost()
        {
            Close();
            GC.Collect();
        }

        /// <summary>宿主执行前通知入口，只更新界面状态，不修改检测参数。</summary>
        public void BeforeRun()
        {
            StatusText = "准备执行涂布缺陷检测。";
        }

        /// <summary>宿主执行后通知入口，刷新耗时和成功/失败状态文本。</summary>
        public void AfterRun(bool success, long elapsedMs)
        {
            RunElapsedText = string.Format("耗时：{0} ms", elapsedMs);
            StatusText = success ? "宿主执行完成。" : Params.m_strRunMessage;
        }

        /// <summary>宿主保存前通知入口，先回读当前 ViewROI 四坐标到参数。</summary>
        public void BeforeSave()
        {
            GetRoiData();
            StatusText = "准备保存涂布缺陷检测参数。";
        }

        /// <summary>宿主保存后通知入口，更新界面状态和调试日志。</summary>
        public void AfterSave(bool success)
        {
            StatusText = success ? "参数保存成功，分类规则已同步保存。" : "参数保存失败。";
            AddDebug(StatusText);
        }

        /// <summary>宿主关闭前通知入口，只更新状态；实际释放由 CloseFromHost/Close 完成。</summary>
        public void BeforeClose()
        {
            StatusText = "插件页面准备关闭。";
        }

        /// <summary>
        /// 插件对象在页面外部被宿主流程调用后，页面从 PluginParams 回读输出并刷新显示。
        /// </summary>
        public void OnPluginExecutedFromOutside(PluginExecuteCompletedEventArgs e)
        {
            RefreshResultCollectionsFromParams();
            halconView?.ShowRunResult(Params.ho_DetectRegion, Params.ho_MaskRoiRegion, Params.ho_ResultRegion);
            ShowPreprocessPreview(true);
            RunElapsedText = string.Format("耗时：{0} ms", e == null ? Params.m_fElapsedMs : e.ElapsedMs);
            StatusText = e == null ? Params.m_strRunMessage : e.Message;
        }

        /// <summary>
        /// 绑定主图 HALCON 显示适配器。
        /// 适配器负责 ViewROI 拖拽事件，ViewModel 在 OnRoiChanged 中把四坐标回写到当前编辑 ROI。
        /// </summary>
        public void AttachHalconView(HalconViewRoiAdapter view)
        {
            halconView = view;
            halconView.RoiChanged += OnRoiChanged;
            halconView.MouseMoved += e => MouseInfo = e.Text;
            halconView.RefreshFromParams(Params, true);
            AddDebug("HALCON显示控件已初始化：WindowsFormsHost + HWindowControl + ViewROI。");
        }

        /// <summary>
        /// 绑定三个预览 HALCON 窗口。
        /// 这里仅创建显示适配器，不接管 Params 或 NgDefectResult 中业务 HObject 的所有权。
        /// </summary>
        public void AttachPreviewWindows(HWindowControl region, HWindowControl small, HWindowControl preprocess)
        {
            regionWindow = region;
            smallWindow = small;
            preprocessWindow = preprocess;
            regionPreview = new HalconPreviewAdapter(regionWindow);
            smallPreview = new HalconPreviewAdapter(smallWindow);
            preprocessPreview = new HalconPreviewAdapter(preprocessWindow);
            if (preprocessPreview != null)
                preprocessPreview.MouseMoved += OnPreprocessMouseMove;
            AddDebug("NG Region、NG小图、预处理阈值图显示控件已初始化。");
        }

        /// <summary>
        /// 释放页面显示资源。
        /// 只关闭适配器和显示副本，不释放宿主输入图像，也不直接释放插件输出结果。
        /// </summary>
        public void Close()
        {
            if (halconView != null)
            {
                halconView.RoiChanged -= OnRoiChanged;
                halconView.Dispose();
                halconView = null;
            }
            if (preprocessPreview != null)
                preprocessPreview.MouseMoved -= OnPreprocessMouseMove;
            LoadImageCommand = null;
            DrawRoiCommand = null;
            GetRoiDataCommand = null;
            DetectCommand = null;
            ShowClassifyCommand = null;
            ShowRuleSettingsCommand = null;
            AddNgTypeCommand = null;
            DeleteNgTypeCommand = null;
            MoveNgUpCommand = null;
            MoveNgDownCommand = null;
            AddFeatureCommand = null;
            DeleteFeatureCommand = null;
            AddJudgeConditionCommand = null;
            DeleteJudgeConditionCommand = null;
            SortFeatureRowsCommand = null;
            GetAllFeaturesCommand = null;
            ClearResultsCommand = null;
            SaveParamsCommand = null;
            LoadParamsCommand = null;
            SelectFeatureRulePathCommand = null;
            AiPlaceholderCommand = null;
            AddMaskRoiCommand = null;
            DeleteMaskRoiCommand = null;
            RebuildPartitionsCommand = null;
            NormalizePartitionsCommand = null;



            regionPreview?.Dispose();
            smallPreview?.Dispose();
            preprocessPreview?.Dispose();
            regionPreview = null;
            smallPreview = null;
            preprocessPreview = null;

            Results.Clear();
            ClassifyResults.Clear();
            MaskRoiItems.Clear();
            PartitionItems.Clear();
            Results.Clear();
            ClassifyResults.Clear();
            FeatureRows.Clear();
            FeatureDetailRows.Clear();
            DebugLogs.Clear();
            PartitionDirections.Clear();
            FeatureTypes.Clear();
            JudgeConditionModes.Clear();
           



        }

        /// <summary>主图和当前预览图全部自适应显示，不改变任何参数。</summary>
        public void FitAllViews()
        {
            halconView?.ShowImage(Params.ho_Image, true);
            RefreshSelectedPreview();
        }

        /// <summary>NG Region 预览窗口自适应显示，不改变 SelectedResult。</summary>
        public void FitRegionView()
        {
            ShowRegionPreview(true);
        }

        /// <summary>NG 小图预览窗口自适应显示，不改变 SelectedResult。</summary>
        public void FitSmallView()
        {
            ShowSmallPreview(true);
        }

        /// <summary>预处理/阈值预览窗口自适应显示，不重新执行检测。</summary>
        public void FitPreprocessView()
        {
            ShowPreprocessPreview(true);
        }

        /// <summary>
        /// 文本框手动输入 ROI 后的同步入口。
        /// 会把当前编辑 Rectangle1 四坐标归一化后同步到 Params 和 ViewROI。
        /// </summary>
        public void SyncRoiFromText()
        {
            CommitEditRoiFieldsToModel();
            SyncViewFromEditRoi();
            UpdateRoiInfo();
        }

        /// <summary>
        /// Demo 手动读图入口。
        /// 自己读入的图像由 Params.OwnsInputImage 标记为插件拥有，后续 Clear/Release 时才允许释放。
        /// </summary>
        private void LoadImage()
        {
            OpenFileDialog dialog = new OpenFileDialog { Filter = "图像文件|*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff|所有文件|*.*" };
            if (dialog.ShowDialog() != true) return;
            ClearResults();
            Params.ReleaseOwnedInputImage();
            HOperatorSet.ReadImage(out Params.ho_Image, dialog.FileName);
            Params.OwnsInputImage = true;
            loadedImagePath = dialog.FileName;
            ImageName = Path.GetFileName(dialog.FileName);
            HOperatorSet.GetImageSize(Params.ho_Image, out HTuple width, out HTuple height);
            ImageInfo = string.Format("图像大小(W*H): {0}*{1}", width, height);
        
            halconView?.RefreshFromParams(Params, true);
            regionPreview?.ShowImage(Params.ho_Image, true);
            smallPreview?.ShowImage(Params.ho_Image, true);
            preprocessPreview?.ShowImage(Params.ho_Image, true);
          

            AddDebug("导入图像：" + dialog.FileName);
            StatusText = "已导入图像：" + ImageName;
        }

        /// <summary>
        /// 刷新当前编辑 ROI 到主图 ViewROI。
        /// 只更新 ROI 显示，不执行检测，也不修改检测算法参数以外的状态。
        /// </summary>
        private void DrawRoi()
        {
            SyncParamsFromAdapterItems();
            CommitEditRoiFieldsToModel();
            halconView?.RefreshFromParams(Params, false);
            SyncViewFromEditRoi();
            AddDebug("刷新矩形ROI，可在图像窗口拖动调整。");
            StatusText = "已创建/刷新矩形ROI，可在图像窗口拖动调整。";
        }

        /// <summary>
        /// 从 ViewROI 回读当前 Rectangle1 四坐标，并同步到局部 ROI 或选中屏蔽 ROI。
        /// 这是拖拽后让界面参数跟着变化的关键入口。
        /// </summary>
        private void GetRoiData()
        {
            SyncParamsFromAdapterItems();
            PullViewRoiToEditModel();
            OnPropertyChanged(nameof(Params));
            RefreshEditRoiBindings();
            UpdateRoiInfo();
            AddDebug("ROI数据已同步：" + RoiInfo);
            StatusText = "ROI数据已同步到参数。";
        }

        /// <summary>
        /// 手动检测入口。
        /// 先回读 ROI，再调用插件主逻辑 ExecuteObj；界面层只刷新结果，不在这里改检测算法。
        /// </summary>
        private void Detect()
        {
            GetRoiData();
            ClearUiResultReferences();
            Results.Clear();
            ClassifyResults.Clear();
            FeatureRows.Clear();
            FeatureDetailRows.Clear();
            DebugLogs.Clear();
            AddDebug("开始检测。");
            AddDebug(string.Format(
                "参数：ReferenceGray={0}, UpOffset={1}, DownOffset={2}, Open={3}, Close={4}, MinArea={5}",
                Params.ReferenceGray, Params.UpGrayOffset, Params.DownGrayOffset, Params.OpenRadius, Params.CloseRadius, Params.MinCandidateArea));

            Stopwatch watch = Stopwatch.StartNew();
            bool success = plugin.ExecuteObj();
            watch.Stop();
            RunElapsedText = string.Format("耗时：{0} ms", watch.ElapsedMilliseconds);
            if (plugin.LastRunResult != null && plugin.LastRunResult.DetectResult != null)
            {
                halconView?.ShowRunResult(Params.ho_DetectRegion, Params.ho_MaskRoiRegion, Params.ho_ResultRegion);
                ShowPreprocessPreview(true);
                OnPropertyChanged(nameof(Params));
                OnPropertyChanged(nameof(PreprocessPreviewTitle));

                foreach (string log in plugin.LastRunResult.DetectResult.DebugLogs)
                    AddDebug(log);

                RefreshResultCollectionsFromParams();
            }
            AddDebug("Elapsed=" + RunElapsedText);
            AddDebug("检测结束：Success=" + success + "，候选数=" + Results.Count + "，外层表已同步规则分类结果。");
            StatusText = success ? string.Format("检测完成，候选缺陷数：{0}。分类特征按规则按需计算，全部特征请点按钮获取。", Results.Count) : Params.m_strRunMessage;
            // 强制回收 Gen0、Gen1、Gen2（完整回收）
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
        }

        /// <summary>
        /// 打开分类规则设置窗口。
        /// 规则窗口编辑的是分类模块独立配置，保存后再回写到插件持有的配置对象。
        /// </summary>
        private void ShowRuleSettings()
        {
            plugin.EnsureDefaultConfigPaths();
            OnPropertyChanged(nameof(Params));
            if (ruleSettingsWindow != null)
            {
                ruleSettingsWindow.Activate();
                return;
            }

            ClearRuleCollectionEditState();
            var ruleVm = new RuleSettingsViewModel(
                plugin.FeatureRuleConfig,
                Params.RuleConfigPath,
                plugin.ModelClassifierConfig,
                plugin.AiClassifierConfig);
            ruleVm.Saved += OnRuleSettingsSaved;
            ruleSettingsWindow = new RuleSettingsWindow
            {
                DataContext = ruleVm
            };
            SetSafeOwner(ruleSettingsWindow);
            ruleSettingsWindow.Closed += (s, e) =>
            {
                ruleVm.Saved -= OnRuleSettingsSaved;
                ruleSettingsWindow = null;
                AddDebug("分类规则设置窗口已关闭。");
            };
            ruleSettingsWindow.Show();
        }

        /// <summary>
        /// 插件运行在框架宿主中时，宿主不一定由标准 WPF Application 启动。
        /// 因此 Application.Current 或 MainWindow 可能为空，打开子窗口前必须防空；
        /// 有宿主主窗口时设置 Owner，便于窗口跟随宿主；没有时直接独立显示，避免空引用。
        /// </summary>
        private static void SetSafeOwner(Window window)
        {
            if (window == null)
                return;

            Application app = Application.Current;
            if (app == null || app.MainWindow == null || ReferenceEquals(app.MainWindow, window))
                return;

            window.Owner = app.MainWindow;
        }

        /// <summary>
        /// 分类规则窗口保存事件回调。
        /// 这里只同步规则配置并重新 InitObj，不改检测参数、ROI 参数或当前图像。
        /// </summary>
        private void OnRuleSettingsSaved(object sender, EventArgs e)
        {
            RuleSettingsViewModel vm = sender as RuleSettingsViewModel;
            if (vm == null) return;

            Params.RuleConfigPath = vm.RuleConfigPath;
            CopyFeatureRuleConfig(vm.FeatureRuleConfig, plugin.FeatureRuleConfig);
            plugin.CopyClassifierConfigs(vm.ModelRuleConfig, vm.AiRuleConfig);
            plugin.InitObj();
            SelectedNgType = plugin.FeatureRuleConfig.NgTypes.FirstOrDefault();
            SelectedFeature = SelectedNgType == null ? null : SelectedNgType.Features.FirstOrDefault();
            StatusText = "分类规则已保存并重新加载。";
            AddDebug("分类规则保存事件触发：已执行 InitObj() 重新加载分类对象。");
            OnPropertyChanged(nameof(Params));
            OnPropertyChanged(nameof(FeatureRuleConfig));
            OnPropertyChanged(nameof(ModelClassifierConfig));
        }

        /// <summary>
        /// 清理规则窗口中 DataGrid 使用的集合视图编辑状态。
        /// 避免关闭窗口时 DataGrid 仍在编辑单元格，下次 Show 重新刷新视图时报 DeferRefresh 异常。
        /// </summary>
        private void ClearRuleCollectionEditState()
        {
            ClearEditableView(FeatureRuleConfig.NgTypes);
            ClearEditableView(FeatureRuleConfig.DarkNgTypes);
            ClearEditableView(ClassifyResults);
            ClearEditableView(JudgeConditionModes);
            if (SelectedNgType != null)
                ClearEditableView(SelectedNgType.Features);
        }

        /// <summary>
        /// 尝试提交或取消集合视图中的 DataGrid 编辑事务。
        /// 这是窗口复用前的界面状态清理，不影响业务参数内容。
        /// </summary>
        private static void ClearEditableView(System.Collections.IEnumerable source)
        {
            if (source == null) return;
            IEditableCollectionView view = CollectionViewSource.GetDefaultView(source) as IEditableCollectionView;
            if (view == null) return;

            try
            {
                if (view.IsAddingNew)
                    view.CommitNew();
                if (view.IsEditingItem)
                    view.CommitEdit();
            }
            catch
            {
                try
                {
                    if (view.IsAddingNew)
                        view.CancelNew();
                    if (view.IsEditingItem)
                        view.CancelEdit();
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// 显示按规则重新分类后的结果窗口。
        /// 该入口复用 LastRunResult 的检测结果，不重新跑阈值检测。
        /// </summary>
        private void ShowClassifyResult()
        {
            if (plugin.LastRunResult == null || plugin.LastRunResult.DetectResult == null)
            {
                StatusText = "请先执行检测，再Show分类结果。";
                MessageBox.Show("请先执行检测，再Show分类结果。", "分类结果", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var refreshed = plugin.ClassifyLastResult();
            ClearUiResultReferences();
            Results.Clear();
            ClassifyResults.Clear();
            FeatureRows.Clear();
            FeatureDetailRows.Clear();
            foreach (NgDefectResult item in Params.NgResults)
                item.Dispose();
            Params.NgResults.Clear();
            foreach (NgDefectResult item in refreshed)
                Params.NgResults.Add(item);

            foreach (NgDefectResult item in Params.NgResults)
            {
                ClassifyResults.Add(item);
                Results.Add(item);
                FeatureRows.Add(new DefectFeatureSummaryRow
                {
                    DefectIndex = item.Index,
                    Area = item.Features.Get("area"),
                    Length = item.Features.Get("length"),
                    Width = item.Features.Get("width"),
                    Height = item.Features.Get("height"),
                    Row = item.Features.Get("row"),
                    Column = item.Features.Get("column"),
                    MeanGray = item.Features.Get("mean_gray"),
                    Contrast = item.Features.Get(FeatureCatalog.KeyOf(PicFeatureType.BrightDarkDiff), double.NaN),
                    Roundness = item.Features.Get(FeatureCatalog.KeyOf(PicFeatureType.DefectRoundness), double.NaN),
                    WidthHeightRate = item.Features.Get(FeatureCatalog.KeyOf(PicFeatureType.WidthHeightRate), double.NaN),
                    BrightArea = item.Features.Get(FeatureCatalog.KeyOf(PicFeatureType.BrightArea), double.NaN),
                    DarkArea = item.Features.Get(FeatureCatalog.KeyOf(PicFeatureType.DarkArea), double.NaN),
                    Polarity = item.Polarity
                });
            }

            SelectedResult = Results.FirstOrDefault();
            SelectedFeatureRow = FeatureRows.FirstOrDefault();

            foreach (NgDefectResult item in ClassifyResults)
                AddDebug(string.Format("分类#{0}：{1}，来源={2}，规则={3}", item.Index, item.ClassName, item.Source, item.HitRule));

            if (classifyResultWindow != null)
            {
                classifyResultWindow.DataContext = ClassifyResults;
                classifyResultWindow.Activate();
            }
            else
            {
                string modelPath = plugin.ModelClassifierConfig?.ModelPath;
                string runsDir = string.IsNullOrWhiteSpace(modelPath)
                    ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "OpenCV分类Demo", "runs")
                    : Path.GetDirectoryName(modelPath);
                classifyResultWindow = new ClassifyResultWindow
                {
                    DataContext = ClassifyResults,
                    TrainingDataRoot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "测试"),
                    TrainingRunsDir = runsDir,
                    TrainingCompleted = () => plugin.InitObj()
                };
                SetSafeOwner(classifyResultWindow);
                classifyResultWindow.Closed += (s, e) => classifyResultWindow = null;
                classifyResultWindow.Show();
            }
            StatusText = "分类结果窗口已显示。NG列表顺序即优先级，靠前优先。";
        }

        /// <summary>新增一个 NG 类型规则，并立即选中，便于继续编辑特征条件。</summary>
        private void AddNgType()
        {
            NgTypeRule rule = new NgTypeRule { Name = "新类别" };
            FeatureRuleConfig.NgTypes.Add(rule);
            SelectedNgType = rule;
            AddDebug("新增NG类型：" + rule.Name);
        }

        /// <summary>删除当前选中的 NG 类型规则，并把选中项切到列表第一个。</summary>
        private void DeleteNgType()
        {
            if (SelectedNgType == null) return;
            AddDebug("删除NG类型：" + SelectedNgType.Name);
            FeatureRuleConfig.NgTypes.Remove(SelectedNgType);
            SelectedNgType = FeatureRuleConfig.NgTypes.FirstOrDefault();
        }

        /// <summary>
        /// 调整 NG 类型规则优先级。
        /// 列表顺序就是特征值分类匹配顺序，越靠前越先命中。
        /// </summary>
        private void MoveNgType(int direction)
        {
            if (SelectedNgType == null) return;
            int oldIndex = FeatureRuleConfig.NgTypes.IndexOf(SelectedNgType);
            int newIndex = oldIndex + direction;
            if (oldIndex < 0 || newIndex < 0 || newIndex >= FeatureRuleConfig.NgTypes.Count) return;
            FeatureRuleConfig.NgTypes.Move(oldIndex, newIndex);
            AddDebug(string.Format("调整NG优先级：{0} 从{1}到{2}", SelectedNgType.Name, oldIndex + 1, newIndex + 1));
        }

        /// <summary>给当前 NG 类型增加一个默认特征范围条件，后续由用户调整类型和阈值。</summary>
        private void AddFeature()
        {
            if (SelectedNgType == null) return;
            FeatureRule rule = new FeatureRule { FeatureType = PicFeatureType.DefectGray, MinValue = 0, MaxValue = 999999999 };
            SelectedNgType.Features.Add(rule);
            SelectedFeature = rule;
            AddDebug("新增特征条件：" + FeatureCatalog.Get(rule.FeatureType).DisplayName);
        }

        /// <summary>删除当前选中的特征条件，并将选中项切回当前类别第一条条件。</summary>
        private void DeleteFeature()
        {
            if (SelectedNgType == null || SelectedFeature == null) return;
            AddDebug("删除特征条件：" + FeatureCatalog.Get(SelectedFeature.FeatureType).DisplayName);
            SelectedNgType.Features.Remove(SelectedFeature);
            SelectedFeature = SelectedNgType.Features.FirstOrDefault();
        }

        /// <summary>启用当前类别的长宽面积组合判定条件，默认使用全部条件与逻辑。</summary>
        private void AddJudgeCondition()
        {
            if (SelectedNgType != null)
            {
                SelectedNgType.EnableJudgeCondition = true;
                SelectedNgType.JudgeCondition = JudgeConditionMode.AllConditionAnd;
            }
            AddDebug("已启用当前类别判定条件：所有条件且。");
            RaiseRuleCommands();
        }

        /// <summary>关闭当前类别的长宽面积组合判定条件，只保留特征范围判断。</summary>
        private void DeleteJudgeCondition()
        {
            if (SelectedNgType == null) return;
            SelectedNgType.EnableJudgeCondition = false;
            AddDebug("已关闭当前类别判定条件：" + SelectedNgType.Name);
            RaiseRuleCommands();
        }

        /// <summary>仅重排界面摘要表显示顺序，不修改检测结果集合和分类结果集合。</summary>
        private void SortFeatureRows()
        {
            var sorted = FeatureRows.OrderByDescending(item => item.Area).ToList();
            FeatureRows.Clear();
            foreach (var item in sorted) FeatureRows.Add(item);
            AddDebug("特征输出已按面积降序排序。");
        }

        /// <summary>
        /// 新增屏蔽 ROI。
        /// 默认在局部 ROI 中心附近生成 Rectangle1，并切换到 Mask 编辑角色，方便立即拖动。
        /// </summary>
        private void AddMaskRoi()
        {
            int index = Params.MaskRois.Count + 1;
            CoatRoiParam roi = new CoatRoiParam
            {
                Index = index,
                Enabled = true,
                Name = "屏蔽ROI-" + index,
                Row1 = GetRoiCenterRow(Params.LocalRoi) - GetRoiHeight(Params.LocalRoi) / 4.0,
                Row2 = GetRoiCenterRow(Params.LocalRoi) + GetRoiHeight(Params.LocalRoi) / 4.0,
                Col1 = GetRoiCenterCol(Params.LocalRoi) - GetRoiWidth(Params.LocalRoi) / 4.0,
                Col2 = GetRoiCenterCol(Params.LocalRoi) + GetRoiWidth(Params.LocalRoi) / 4.0
            };
            NormalizeRoiDerivedFields(roi);
            Params.MaskRois.Add(roi);
            CoatRoiParamViewModel roiItem = CreateMaskRoiItem(roi);
            MaskRoiItems.Add(roiItem);
            SelectedMaskRoi = roiItem;
            EditRole = RoiEditRole.Mask;
            SyncRectangle1FieldsFromEditRoi();
            halconView?.RefreshFromParams(Params, false);
            SyncViewFromEditRoi();
            AddDebug("新增屏蔽ROI：" + roi.Name);
        }

        /// <summary>
        /// 删除当前屏蔽 ROI。
        /// 删除后重新编号并重建 UI 适配项，避免界面集合与 Params.MaskRois 不一致。
        /// </summary>
        private void DeleteMaskRoi()
        {
            if (SelectedMaskRoi == null) return;
            Params.MaskRois.Remove(SelectedMaskRoi.Model);
            MaskRoiItems.Remove(SelectedMaskRoi);
            for (int i = 0; i < Params.MaskRois.Count; i++)
            {
                Params.MaskRois[i].Index = i + 1;
                Params.MaskRois[i].Name = "屏蔽ROI-" + (i + 1);
            }
            RebuildMaskRoiItemsFromParams();
            SelectedMaskRoi = MaskRoiItems.FirstOrDefault();
            if (SelectedMaskRoi == null) Params.EditRole = RoiEditRole.Local;
            SyncRectangle1FieldsFromEditRoi();
            halconView?.RefreshFromParams(Params, false);
            SyncViewFromEditRoi();
            AddDebug("删除屏蔽ROI。");
        }

        /// <summary>
        /// 清空当前界面结果并释放插件拥有的运行输出。
        /// 注意：手动导入图像是否释放由 Params.OwnsInputImage 决定，宿主图像不在这里无条件释放。
        /// </summary>
        private void ClearResults()
        {
            ClearUiResultReferences();
            halconView?.ClearRunResult();
            Results.Clear();
            ClassifyResults.Clear();
            FeatureRows.Clear();
            FeatureDetailRows.Clear();
            DebugLogs.Clear();
            plugin.ReleaseOutputs();
            regionPreview?.Clear();
            smallPreview?.Clear();
            preprocessPreview?.Clear();
            AddDebug("已清空检测结果、分类结果和特征列表。");
        }

        /// <summary>
        /// 手动保存参数。
        /// 保存前先把 ROI/分区 UI 适配集合写回 Params，再保存主 XML 和分类规则配置。
        /// </summary>
        private void SaveParams()
        {
            SyncParamsFromAdapterItems();
            SaveFileDialog dialog = new SaveFileDialog { Filter = "XML参数|*.xml", FileName = "DefectCoatParams.xml" };
            if (dialog.ShowDialog() != true) return;

            EnsureRuleConfigPathForParamFile(dialog.FileName);
            SaveCurrentRuleConfig();
            StatusText = plugin.SaveParams(dialog.FileName) ? "参数保存成功" : "参数保存失败";
            AddDebug(StatusText + "：" + dialog.FileName);
        }

        /// <summary>当主参数文件没有指定规则路径时，给它补一个默认分类规则路径。</summary>
        private void EnsureRuleConfigPathForParamFile(string paramFilePath)
        {
            if (!string.IsNullOrWhiteSpace(Params.RuleConfigPath))
                return;

            Params.RuleConfigPath = DefaultConfigPathService.GetDefaultFeatureRulePath();
            OnPropertyChanged(nameof(Params));
        }

        /// <summary>
        /// 保存当前分类规则及其关联的传统分类器、AI 分类器配置。
        /// 失败只写入调试日志，不中断主参数保存流程。
        /// </summary>
        private void SaveCurrentRuleConfig()
        {
            plugin.EnsureDefaultConfigPaths();
            OnPropertyChanged(nameof(Params));

            try
            {
                EnsureClassifierConfigPathsForRuleFile();
                plugin.ModelClassifierConfig.Save(plugin.FeatureRuleConfig.ModelClassifierConfigPath);
                plugin.AiClassifierConfig.Save(plugin.FeatureRuleConfig.AiClassifierConfigPath);
                plugin.FeatureRuleConfig.Save(Params.RuleConfigPath);
                AddDebug("分类规则参数已保存：" + Params.RuleConfigPath);
            }
            catch (Exception ex)
            {
                AddDebug("分类规则参数保存失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 主界面只让用户维护特征规则入口路径。
        /// 保存模块参数时，分类器和 AI 的独立参数文件跟随规则文件自动生成，路径写回特征规则 XML。
        /// </summary>
        private void EnsureClassifierConfigPathsForRuleFile()
        {
            string directory = Path.GetDirectoryName(Params.RuleConfigPath);
            if (string.IsNullOrWhiteSpace(directory))
                directory = DefaultConfigPathService.GetModuleConfigDirectory();

            string baseName = Path.GetFileNameWithoutExtension(Params.RuleConfigPath);
            if (string.IsNullOrWhiteSpace(plugin.FeatureRuleConfig.ModelClassifierConfigPath))
                plugin.FeatureRuleConfig.ModelClassifierConfigPath = DefaultConfigPathService.GetDefaultModelClassifierPath(Params.RuleConfigPath);
            if (string.IsNullOrWhiteSpace(plugin.FeatureRuleConfig.AiClassifierConfigPath))
                plugin.FeatureRuleConfig.AiClassifierConfigPath = DefaultConfigPathService.GetDefaultAiClassifierPath(Params.RuleConfigPath);

            plugin.ModelClassifierConfig.ConfigPath = plugin.FeatureRuleConfig.ModelClassifierConfigPath;
            plugin.AiClassifierConfig.ConfigPath = plugin.FeatureRuleConfig.AiClassifierConfigPath;
        }

        /// <summary>
        /// 选择特征规则保存路径。
        /// 路径改变后立即保存规则和分类器配置，并重新初始化分类对象。
        /// </summary>
        private void SelectFeatureRulePath()
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = "分类规则参数|*.json",
                FileName = Path.GetFileName(DefaultConfigPathService.EnsureFeatureRulePath(Params.RuleConfigPath)),
                InitialDirectory = Path.GetDirectoryName(DefaultConfigPathService.EnsureFeatureRulePath(Params.RuleConfigPath)),
                Title = "选择分类规则保存路径"
            };
            if (dialog.ShowDialog() != true) return;

            try
            {
                Params.RuleConfigPath = dialog.FileName;
                OnPropertyChanged(nameof(Params));
                EnsureClassifierConfigPathsForRuleFile();
                plugin.ModelClassifierConfig.Save(plugin.FeatureRuleConfig.ModelClassifierConfigPath);
                plugin.AiClassifierConfig.Save(plugin.FeatureRuleConfig.AiClassifierConfigPath);
                plugin.FeatureRuleConfig.Save(Params.RuleConfigPath);
                StatusText = "分类规则保存路径已设置。";
                AddDebug("已设置分类规则保存路径：" + dialog.FileName);
                plugin.InitObj();
                OnPropertyChanged(nameof(Params));
                OnPropertyChanged(nameof(FeatureRuleConfig));
            }
            catch (Exception ex)
            {
                StatusText = "分类规则路径设置失败：" + ex.Message;
                AddDebug(StatusText);
            }
        }

        /// <summary>
        /// 手动读取 XML 参数。
        /// 只复制可保存的算法参数和 ROI/分区集合，不替换当前 DefectCoatObj，也不无条件释放输入图像。
        /// </summary>
        private void LoadParams()
        {
            OpenFileDialog dialog = new OpenFileDialog { Filter = "XML参数|*.xml" };
            if (dialog.ShowDialog() != true) return;
            try
            {
                ParamsDefectCoat loaded = DefectCoatObj.LoadParams(dialog.FileName);
                // 加载参数只更新算法配置，不释放也不替换输入图像。
                // 正式插件运行时图像由宿主写入 Params.ho_Image，Demo 手动打开图像时由 LoadImage 管理。
                ClearResults();

                CopyLoadedParams(loaded);
                plugin.InitObj();
                SelectedNgType = FeatureRuleConfig.NgTypes.FirstOrDefault();
                SelectedFeature = SelectedNgType == null ? null : SelectedNgType.Features.FirstOrDefault();
                if (Params.ho_Image != null)
                {
                    halconView?.RefreshFromParams(Params, true);
                    regionPreview?.ShowImage(Params.ho_Image, true);
                    smallPreview?.ShowImage(Params.ho_Image, true);
                    preprocessPreview?.ShowImage(Params.ho_Image, true);
                }
                else
                {
                    halconView?.ClearImage();
                    regionPreview?.Clear();
                    smallPreview?.Clear();
                    preprocessPreview?.Clear();
                }

                SyncRectangle1FieldsFromEditRoi();
                SyncViewFromEditRoi();
                OnPropertyChanged(nameof(Params));
                AddDebug("读取参数：" + dialog.FileName);
                StatusText = Params.ho_Image != null ? "参数读取成功，保留当前图像并刷新参数和ROI。" : "参数读取成功，当前未传入图像，仅刷新参数和ROI。";
            }
            catch (Exception ex)
            {
                StatusText = "参数读取失败：" + ex.Message;
                AddDebug(StatusText);
            }
        }

        /// <summary>
        /// 把规则窗口中的特征分类配置复制回插件持有对象。
        /// 这里不直接替换 target 引用，避免已有绑定对象失效。
        /// </summary>
        private static void CopyFeatureRuleConfig(FeatureRuleClassifierConfig source, FeatureRuleClassifierConfig target)
        {
            if (source == null || target == null) return;
            target.ConfigPath = source.ConfigPath;
            target.ModelClassifierConfigPath = source.ModelClassifierConfigPath;
            target.AiClassifierConfigPath = source.AiClassifierConfigPath;
            target.EnableCandidateCountLimit = source.EnableCandidateCountLimit;
            target.MaxCandidateCount = source.MaxCandidateCount;
            target.EnableMaxEnterClassifyLimit = source.EnableMaxEnterClassifyLimit;
            target.MaxEnterClassifyCount = source.MaxEnterClassifyCount;
            target.OverflowClassName = source.OverflowClassName;

            target.JudgeConditionTemplates.Clear();
            if (source.JudgeConditionTemplates != null)
                foreach (JudgeConditionTemplate item in source.JudgeConditionTemplates) target.JudgeConditionTemplates.Add(item);

            target.NgTypes.Clear();
            if (source.NgTypes != null)
                foreach (NgTypeRule item in source.NgTypes) target.NgTypes.Add(item);

            target.DarkNgTypes.Clear();
            if (source.DarkNgTypes != null)
                foreach (NgTypeRule item in source.DarkNgTypes) target.DarkNgTypes.Add(item);
        }

        /// <summary>
        /// 把从 XML 加载的参数复制到当前 Params。
        /// 复制范围只限检测、ROI、分区、分类路径等配置项；运行态图像和窗口引用不从文件导入。
        /// </summary>
        private void CopyLoadedParams(ParamsDefectCoat loaded)
        {
            Params.EnableFeatureRuleClassify = loaded.EnableFeatureRuleClassify;
            Params.EnableModelClassify = loaded.EnableModelClassify;
            Params.EnableAiClassify = loaded.EnableAiClassify;
            Params.RuleConfigPath = loaded.RuleConfigPath;
            if (string.IsNullOrWhiteSpace(Params.RuleConfigPath))
                Params.RuleConfigPath = DefaultConfigPathService.GetDefaultFeatureRulePath();
            Params.PartitionCount = Math.Max(1, loaded.PartitionCount);
            Params.PartitionDirection = loaded.PartitionDirection;
            Params.PartitionLengthUnit = loaded.PartitionLengthUnit;
            Params.ReferenceGray = loaded.ReferenceGray;
            Params.UpGrayOffset = loaded.UpGrayOffset;
            Params.DownGrayOffset = loaded.DownGrayOffset;
            Params.OpenRadius = loaded.OpenRadius;
            Params.CloseRadius = loaded.CloseRadius;
            Params.MinCandidateArea = loaded.MinCandidateArea;
            Params.EnableBackgroundGrayScale = loaded.EnableBackgroundGrayScale;
            Params.EnableSmallRegionMerge = loaded.EnableSmallRegionMerge;
            Params.SmallRegionMergeMaxArea = loaded.SmallRegionMergeMaxArea;
            Params.SmallRegionMergeWidth = loaded.SmallRegionMergeWidth;
            Params.SmallRegionMergeHeight = loaded.SmallRegionMergeHeight;
            Params.MaxGrayScaleAbs = loaded.MaxGrayScaleAbs;
            Params.PixelToMmH = loaded.PixelToMmH;
            Params.PixelToMmV = loaded.PixelToMmV;
            Params.DetectMode = loaded.DetectMode;
            Params.EditRole = loaded.EditRole;
            Params.EnableLocalRoi = loaded.EnableLocalRoi;
            Params.EnableMaskRoi = loaded.EnableMaskRoi;
            Params.ShowViewRoi = loaded.ShowViewRoi;
            Params.EnablePositionFollow = loaded.EnablePositionFollow;
            Params.LocalRoi = loaded.LocalRoi ?? Params.LocalRoi;
            Params.BaseRow = loaded.BaseRow;
            Params.BaseCol = loaded.BaseCol;
            Params.BaseAngle = loaded.BaseAngle;
            Params.CurrentRow = loaded.CurrentRow;
            Params.CurrentCol = loaded.CurrentCol;
            Params.CurrentAngle = loaded.CurrentAngle;
            Params.MaskRois.Clear();
            if (loaded.MaskRois != null)
                foreach (CoatRoiParam item in loaded.MaskRois) Params.MaskRois.Add(item);
            Params.Partitions.Clear();
            if (loaded.Partitions != null)
                foreach (CoatPartitionParam item in loaded.Partitions) Params.Partitions.Add(item);
            NormalizePartitions(false);
            RebuildMaskRoiItemsFromParams();
            SelectedMaskRoi = MaskRoiItems.FirstOrDefault();
            SelectedNgType = plugin.FeatureRuleConfig.NgTypes.FirstOrDefault();
        }

        /// <summary>创建屏蔽 ROI 的 ViewModel 适配项，并挂接统一的 ROI 参数变化回调。</summary>
        private CoatRoiParamViewModel CreateMaskRoiItem(CoatRoiParam model)
        {
            return new CoatRoiParamViewModel(model, OnRoiAdapterChanged);
        }

        /// <summary>创建分区参数的 ViewModel 适配项，并挂接统一的分区参数变化回调。</summary>
        private CoatPartitionParamViewModel CreatePartitionItem(CoatPartitionParam model)
        {
            return new CoatPartitionParamViewModel(model, OnPartitionAdapterChanged);
        }

        /// <summary>
        /// 根据当前 Params 重建所有 UI 适配对象。
        /// BindPlugin 或加载参数后调用，确保界面集合包装的是当前参数对象。
        /// </summary>
        private void RebuildUiAdaptersFromParams()
        {
            LocalRoiItem = new CoatRoiParamViewModel(Params.LocalRoi, OnRoiAdapterChanged);
            RebuildMaskRoiItemsFromParams();
            RebuildPartitionItemsFromParams();
            OnPropertyChanged(nameof(LocalRoiItem));
        }

        /// <summary>用 Params.MaskRois 重建屏蔽 ROI 界面集合，保持集合顺序和参数顺序一致。</summary>
        private void RebuildMaskRoiItemsFromParams()
        {
            MaskRoiItems.Clear();
            if (Params.MaskRois != null)
            {
                foreach (CoatRoiParam item in Params.MaskRois)
                    MaskRoiItems.Add(CreateMaskRoiItem(item));
            }
            OnPropertyChanged(nameof(MaskRoiItems));
        }

        /// <summary>用 Params.Partitions 重建分区界面集合，保持分区编号和参数集合一致。</summary>
        private void RebuildPartitionItemsFromParams()
        {
            PartitionItems.Clear();
            if (Params.Partitions != null)
            {
                foreach (CoatPartitionParam item in Params.Partitions)
                    PartitionItems.Add(CreatePartitionItem(item));
            }
            OnPropertyChanged(nameof(PartitionItems));
        }

        /// <summary>刷新分区适配项的属性通知，用于单位换算或总长度变化后的界面更新。</summary>
        private void RefreshPartitionItems()
        {
            foreach (CoatPartitionParamViewModel item in PartitionItems)
                item.RefreshAll();
            OnPropertyChanged(nameof(PartitionItems));
        }

        /// <summary>
        /// 把 UI 适配集合写回 Params。
        /// 保存、绘制、检测和分区归一化前都要调用，避免界面改动停留在适配层。
        /// </summary>
        private void SyncParamsFromAdapterItems()
        {
            if (LocalRoiItem != null && LocalRoiItem.Model != null)
                Params.LocalRoi = LocalRoiItem.Model;

            Params.MaskRois.Clear();
            foreach (CoatRoiParamViewModel item in MaskRoiItems)
                if (item.Model != null) Params.MaskRois.Add(item.Model);

            Params.Partitions.Clear();
            foreach (CoatPartitionParamViewModel item in PartitionItems)
                if (item.Model != null) Params.Partitions.Add(item.Model);
        }

        /// <summary>
        /// ROI 适配项属性变化回调。
        /// 根据当前 EditRole 把局部 ROI 或屏蔽 ROI 的 Rectangle1 四坐标同步到界面和 ViewROI。
        /// </summary>
        private void OnRoiAdapterChanged()
        {
            CoatRoiParam roi = GetCurrentEditRoi();
            NormalizeRoiDerivedFields(roi);
            RefreshCurrentRoiItem();
            RefreshEditRoiBindings();
            SyncViewFromEditRoi();
            UpdateRoiInfo();
        }

        /// <summary>分区适配项属性变化回调，只刷新分区总长度和固定边显示。</summary>
        private void OnPartitionAdapterChanged()
        {
            OnPropertyChanged(nameof(PartitionTotalLengthText));
            OnPropertyChanged(nameof(PartitionFixedWidthText));
        }

        /// <summary>
        /// 按当前局部 ROI 总长度和分区数量重建分区列表。
        /// 新分区继承当前全局检测阈值参数，长度默认均分。
        /// </summary>
        private void RebuildPartitions()
        {
            SyncParamsFromAdapterItems();
            int count = Math.Max(1, Params.PartitionCount);
            CommitEditRoiFieldsToModel();
            Params.Partitions.Clear();
            double length = GetPartitionTotalLength();
            double each = length / count;
            for (int i = 0; i < count; i++)
                Params.Partitions.Add(CreatePartitionFromGlobal(i + 1, each, false));

            NormalizePartitions(false);
            AddDebug(string.Format("分区参数已重建：总长度={0:F2}px，分区数={1}", length, count));
            StatusText = "分区参数已重建。";
        }

        /// <summary>使用当前界面集合归一化分区长度。</summary>
        private void NormalizePartitions()
        {
            NormalizePartitions(true);
        }

        /// <summary>
        /// 归一化分区集合。
        /// 会补齐/裁剪分区数量、处理像素/mm 换算，并把未固定长度分区按剩余长度均分。
        /// </summary>
        private void NormalizePartitions(bool syncFromUi)
        {
            if (syncFromUi)
                SyncParamsFromAdapterItems();
            int count = Math.Max(1, Params.PartitionCount);
            CommitEditRoiFieldsToModel();
            double totalLength = GetPartitionTotalLength();
            while (Params.Partitions.Count < count)
                Params.Partitions.Add(CreatePartitionFromGlobal(Params.Partitions.Count + 1, totalLength / count, false));

            while (Params.Partitions.Count > count)
                Params.Partitions.RemoveAt(Params.Partitions.Count - 1);

            double unit = GetPartitionPixelUnit();
            double fixedSum = 0;
            int autoCount = 0;
            for (int i = 0; i < Params.Partitions.Count; i++)
            {
                CoatPartitionParam item = Params.Partitions[i];
                item.Index = i + 1;
                item.Name = string.IsNullOrWhiteSpace(item.Name) ? "分区" + item.Index : item.Name;

                if (Params.PartitionLengthUnit == PartitionLengthUnit.Millimeter)
                    item.SetLengthFromSystem(Math.Max(1, item.LengthMm / unit));
                else
                {
                    if (item.Length <= 0)
                        item.SetLengthFromSystem(totalLength / count);
                    item.SetLengthMmFromSystem(item.Length * unit);
                }

                if (item.IsLengthFixed)
                    fixedSum += Math.Max(0, item.Length);
                else
                    autoCount++;
            }

            if (autoCount > 0)
            {
                double autoLength = Math.Max(1, (totalLength - fixedSum) / autoCount);
                foreach (CoatPartitionParam item in Params.Partitions.Where(item => !item.IsLengthFixed))
                    item.SetLengthFromSystem(autoLength);
            }

            foreach (CoatPartitionParam item in Params.Partitions)
                item.SetLengthMmFromSystem(item.Length * unit);

            RebuildPartitionItemsFromParams();
            OnPropertyChanged(nameof(Params));
            OnPropertyChanged(nameof(PartitionItems));
            OnPropertyChanged(nameof(PartitionTotalLengthText));
            OnPropertyChanged(nameof(PartitionFixedWidthText));
            AddDebug(string.Format(
                "分区长度已换算：总长度={0:F2}px，总长度={1:F4}mm，单位={2}，固定分区={3}，自动分区={4}",
                totalLength,
                totalLength * unit,
                Params.PartitionLengthUnit == PartitionLengthUnit.Millimeter ? "mm" : "px",
                Params.Partitions.Count(item => item.IsLengthFixed),
                Params.Partitions.Count(item => !item.IsLengthFixed)));
        }

        /// <summary>
        /// 获取分区方向上的局部 ROI 总长度，单位为像素。
        /// Rectangle1 以四坐标为准，宽度来自 Col2-Col1，高度来自 Row2-Row1。
        /// </summary>
        private double GetPartitionTotalLength()
        {
            CoatRoiParam roi = Params.LocalRoi;
            if (roi != null) NormalizeRoiDerivedFields(roi);
            double width = GetRoiWidth(roi);
            double height = GetRoiHeight(roi);
            return Params.PartitionDirection == PartitionSplitDirection.WideSide
                ? Math.Max(1, width)
                : Math.Max(1, height);
        }

        /// <summary>获取当前分区方向使用的像素到毫米比例，非法值按 1 处理避免除零。</summary>
        private double GetPartitionPixelUnit()
        {
            double unit = Params.PartitionDirection == PartitionSplitDirection.WideSide
                ? Params.PixelToMmH
                : Params.PixelToMmV;
            return unit <= 0 ? 1 : unit;
        }

        /// <summary>
        /// 从当前全局检测参数创建一个分区参数。
        /// 分区默认继承全局阈值/形态学/小区域合并参数，长度由调用方传入。
        /// </summary>
        private CoatPartitionParam CreatePartitionFromGlobal(int index, double lengthPx, bool fixedLength)
        {
            CoatPartitionParam item = new CoatPartitionParam
            {
                Index = index,
                Name = "分区" + index,
                ReferenceGray = Params.ReferenceGray,
                UpGrayOffset = Params.UpGrayOffset,
                DownGrayOffset = Params.DownGrayOffset,
                OpenRadius = Params.OpenRadius,
                CloseRadius = Params.CloseRadius,
                MinCandidateArea = Params.MinCandidateArea,
                EnableBackgroundGrayScale = Params.EnableBackgroundGrayScale,
                EnableSmallRegionMerge = Params.EnableSmallRegionMerge,
                SmallRegionMergeMaxArea = Params.SmallRegionMergeMaxArea,
                SmallRegionMergeWidth = Params.SmallRegionMergeWidth,
                SmallRegionMergeHeight = Params.SmallRegionMergeHeight,
                MaxGrayScaleAbs = Params.MaxGrayScaleAbs,
                IsLengthFixed = fixedLength
            };
            item.SetLengthFromSystem(Math.Max(1, lengthPx));
            item.SetLengthMmFromSystem(item.Length * GetPartitionPixelUnit());
            return item;
        }

        /// <summary>
        /// 把当前编辑 ROI 归一化并刷新四坐标绑定。
        /// 当前参数只保留 Rectangle1 四坐标，不再维护中心点、角度或半宽半高兼容字段。
        /// </summary>
        private void SyncRectangle1FieldsFromEditRoi()
        {
            CoatRoiParam roi = Params.EditRole == RoiEditRole.Mask ? (SelectedMaskRoi == null ? null : SelectedMaskRoi.Model) : Params.LocalRoi;
            if (roi == null) roi = Params.LocalRoi;
            NormalizeRoiDerivedFields(roi);
            UpdateRoiInfo();
            RefreshEditRoiBindings();
        }

        /// <summary>
        /// 获取当前正在编辑的 ROI 对象。
        /// Local 编辑 Params.LocalRoi；Mask 编辑 SelectedMaskRoi.Model，未选中时兜底取第一个屏蔽 ROI。
        /// </summary>
        private CoatRoiParam GetCurrentEditRoi()
        {
            // 当前界面只允许编辑 Rectangle1：局部 ROI 或选中的屏蔽 ROI。
            if (Params.EditRole == RoiEditRole.Mask)
            {
                if (SelectedMaskRoi != null && SelectedMaskRoi.Model != null)
                    return SelectedMaskRoi.Model;

                if (Params.MaskRois != null && Params.MaskRois.Count > 0)
                    return Params.MaskRois[0];
            }

            return Params.LocalRoi;
        }

        /// <summary>读取当前编辑 ROI 的某个四坐标字段；无 ROI 时返回 0 供界面显示。</summary>
        private double GetEditRoiValue(Func<CoatRoiParam, double> getter)
        {
            CoatRoiParam roi = GetCurrentEditRoi();
            return roi == null ? 0 : getter(roi);
        }

        /// <summary>
        /// 写入当前编辑 ROI 的某个四坐标字段。
        /// 写入后立即归一化、刷新文本绑定并同步 ViewROI，保证界面和图上框一致。
        /// </summary>
        private void SetEditRoiValue(double value, Action<CoatRoiParam, double> setter)
        {
            CoatRoiParam roi = GetCurrentEditRoi();
            if (roi == null) return;

            setter(roi, value);
            CommitEditRoiFieldsToModel();
            RefreshEditRoiBindings();
            SyncViewFromEditRoi();
        }

        /// <summary>
        /// 提交当前编辑 ROI 的文本坐标。
        /// 这里只归一化 Rectangle1 四坐标，不新增 ROI、不改检测流程，也不恢复中心点/角度/半宽半高等旧参数。
        /// </summary>
        private void CommitEditRoiFieldsToModel()
        {
            CoatRoiParam roi = GetCurrentEditRoi();
            if (roi == null) return;

            NormalizeRoiDerivedFields(roi);
            UpdateRoiInfo();
            RefreshCurrentRoiItem();
        }

        /// <summary>
        /// 从主图 ViewROI 读取当前 Rectangle1 四坐标并写回当前编辑 ROI。
        /// ViewROI 读取失败时退回文本坐标提交，避免拖拽框为空导致参数丢失。
        /// </summary>
        private void PullViewRoiToEditModel()
        {
            CoatRoiParam roi = GetCurrentEditRoi();
            if (roi == null) return;

            if (halconView != null && halconView.TryGetActiveRectangle1(out double row1, out double col1, out double row2, out double col2))
            {
                roi.Row1 = row1;
                roi.Col1 = col1;
                roi.Row2 = row2;
                roi.Col2 = col2;
                NormalizeRoiDerivedFields(roi);
                UpdateRoiInfo();
                RefreshCurrentRoiItem();
                RefreshEditRoiBindings();
                return;
            }

            CommitEditRoiFieldsToModel();
        }

        /// <summary>
        /// 把当前编辑 ROI 的四坐标推送到主图 ViewROI。
        /// 只同步显示框，不重新生成检测结果。
        /// </summary>
        private void SyncViewFromEditRoi()
        {
            CoatRoiParam roi = GetCurrentEditRoi();
            if (roi == null) return;

            NormalizeRoiDerivedFields(roi);
            halconView?.SyncActiveRectangle1(roi.Row1, roi.Col1, roi.Row2, roi.Col2);
        }

        /// <summary>刷新当前编辑 ROI 对应的适配项属性通知。</summary>
        private void RefreshCurrentRoiItem()
        {
            if (Params.EditRole == RoiEditRole.Mask && SelectedMaskRoi != null)
                SelectedMaskRoi.RefreshAll();
            else
                LocalRoiItem?.RefreshAll();
        }

        /// <summary>
        /// 刷新 ROI 文本框和分区长度显示。
        /// 四坐标代理属性没有自己的 backing field，必须显式通知界面重读当前编辑 ROI。
        /// </summary>
        private void RefreshEditRoiBindings()
        {
            OnPropertyChanged(nameof(EditRoiRow1));
            OnPropertyChanged(nameof(EditRoiCol1));
            OnPropertyChanged(nameof(EditRoiRow2));
            OnPropertyChanged(nameof(EditRoiCol2));
            OnPropertyChanged(nameof(PartitionTotalLengthText));
            OnPropertyChanged(nameof(PartitionFixedWidthText));
        }

        /// <summary>
        /// 归一化 Rectangle1 四坐标。
        /// Row/Col 四坐标是唯一 ROI 参数，中心、角度、半宽半高都不再保存。
        /// </summary>
        private static void NormalizeRoiDerivedFields(CoatRoiParam roi)
        {
            if (roi == null) return;
            double row1 = Math.Min(roi.Row1, roi.Row2);
            double row2 = Math.Max(roi.Row1, roi.Row2);
            double col1 = Math.Min(roi.Col1, roi.Col2);
            double col2 = Math.Max(roi.Col1, roi.Col2);
            roi.Row1 = row1;
            roi.Row2 = row2;
            roi.Col1 = col1;
            roi.Col2 = col2;
        }

        /// <summary>
        /// 计算 Rectangle1 的像素宽度。
        /// 宽度只从 Col1/Col2 临时计算，不再保存 Width、Length1 等派生参数，避免界面显示和真实 ROI 坐标脱节。
        /// </summary>
        private static double GetRoiWidth(CoatRoiParam roi)
        {
            return roi == null ? 1 : Math.Max(1, Math.Abs(roi.Col2 - roi.Col1));
        }

        /// <summary>
        /// 计算 Rectangle1 的像素高度。
        /// 高度只从 Row1/Row2 临时计算，不再保存 Height、Length2 等派生参数。
        /// </summary>
        private static double GetRoiHeight(CoatRoiParam roi)
        {
            return roi == null ? 1 : Math.Max(1, Math.Abs(roi.Row2 - roi.Row1));
        }

        /// <summary>
        /// 计算 Rectangle1 中心行坐标。
        /// 该值只用于新增屏蔽 ROI 时给一个合理默认框，不写入参数文件，也不作为可编辑 ROI 参数显示。
        /// </summary>
        private static double GetRoiCenterRow(CoatRoiParam roi)
        {
            return roi == null ? 0 : (Math.Min(roi.Row1, roi.Row2) + Math.Max(roi.Row1, roi.Row2)) / 2.0;
        }

        /// <summary>
        /// 计算 Rectangle1 中心列坐标。
        /// 该值只用于界面辅助生成默认屏蔽 ROI，不恢复旧的 CenterCol 参数。
        /// </summary>
        private static double GetRoiCenterCol(CoatRoiParam roi)
        {
            return roi == null ? 0 : (Math.Min(roi.Col1, roi.Col2) + Math.Max(roi.Col1, roi.Col2)) / 2.0;
        }

        /// <summary>主图 ViewROI 拖拽完成后的回调，立即把图上框坐标回写到当前编辑 ROI。</summary>
        private void OnRoiChanged()
        {
            PullViewRoiToEditModel();
            UpdateRoiInfo();
        }

        /// <summary>刷新规则编辑相关命令的 CanExecute 状态。</summary>
        private void RaiseRuleCommands()
        {
            (DeleteNgTypeCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (MoveNgUpCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (MoveNgDownCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (AddFeatureCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (DeleteFeatureCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (DeleteJudgeConditionCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }

        /// <summary>更新当前编辑 ROI 的四坐标摘要文本。</summary>
        private void UpdateRoiInfo()
        {
            CoatRoiParam roi = GetCurrentEditRoi();
            if (roi == null)
                RoiInfo = "ROI: --";
            else
                RoiInfo = string.Format("ROI: Row1={0:F2}, Col1={1:F2}, Row2={2:F2}, Col2={3:F2}",
                    roi.Row1, roi.Col1, roi.Row2, roi.Col2);
            OnPropertyChanged(nameof(RoiInfo));
        }

        /// <summary>向界面调试日志追加一行带毫秒时间戳的文本。</summary>
        private void AddDebug(string text)
        {
            DebugLogs.Add(DateTime.Now.ToString("HH:mm:ss.fff") + "  " + text);
        }

        /// <summary>
        /// 从 Params.NgResults 刷新主结果表、分类结果表和默认特征摘要表。
        /// 注意：这里只读取插件输出，不重新计算检测或分类，保证界面层不承载算法逻辑。
        /// </summary>
        private void RefreshResultCollectionsFromParams()
        {
            ClearUiResultReferences();
            Results.Clear();
            ClassifyResults.Clear();
            FeatureRows.Clear();
            FeatureDetailRows.Clear();

            if (Params.NgResults != null)
            {
                foreach (NgDefectResult item in Params.NgResults)
                {
                    Results.Add(item);
                    ClassifyResults.Add(item);
                    FeatureRows.Add(new DefectFeatureSummaryRow
                    {
                        DefectIndex = item.Index,
                        Area = item.Features.Get("area"),
                        Length = item.Features.Get("length"),
                        Width = item.Features.Get("width"),
                        Height = item.Features.Get("height"),
                        Row = item.Features.Get("row"),
                        Column = item.Features.Get("column"),
                        MeanGray = item.Features.Get("mean_gray"),
                        Contrast = item.Features.Get(FeatureCatalog.KeyOf(PicFeatureType.BrightDarkDiff), double.NaN),
                        Roundness = item.Features.Get(FeatureCatalog.KeyOf(PicFeatureType.DefectRoundness), double.NaN),
                        WidthHeightRate = item.Features.Get(FeatureCatalog.KeyOf(PicFeatureType.WidthHeightRate), double.NaN),
                        BrightArea = item.Features.Get(FeatureCatalog.KeyOf(PicFeatureType.BrightArea), double.NaN),
                        DarkArea = item.Features.Get(FeatureCatalog.KeyOf(PicFeatureType.DarkArea), double.NaN),
                        Polarity = item.Polarity
                    });
                }
            }

            SelectedResult = Results.FirstOrDefault();
            SelectedFeatureRow = FeatureRows.FirstOrDefault();
        }

        /// <summary>
        /// 解除界面对旧结果对象的引用。
        /// 释放 Params.NgResults 或 LastRunResult 前必须先调用，避免 SelectedResult 继续指向已释放的 Region/小图。
        /// </summary>
        private void ClearUiResultReferences()
        {
            SelectedResult = null;
            SelectedFeatureRow = null;
            FeatureDetailRows.Clear();
            OnPropertyChanged(nameof(RegionPreviewTitle));
            OnPropertyChanged(nameof(SmallPreviewTitle));
            (GetAllFeaturesCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }

        /// <summary>刷新当前选中缺陷对应的三个预览窗口，不改变检测结果集合。</summary>
        private void RefreshSelectedPreview()
        {
            ShowRegionPreview(true);
            ShowSmallPreview(true);
            ShowPreprocessPreview(false);
        }

        /// <summary>
        /// 为当前缺陷补算完整特征。
        /// 检测阶段只写入基础特征，完整特征按需计算，避免每次检测都付出全部特征开销。
        /// </summary>
        private void GetAllFeaturesForSelected()
        {
            if (SelectedResult == null || SelectedResult.Features == null)
            {
                StatusText = "请先选择一个缺陷。";
                return;
            }

            DefectFeatureExtractor extractor = new DefectFeatureExtractor();
            extractor.ExtractAll(new DefectFeatureContext
            {
                Image = Params.ho_PreprocessImage ?? Params.ho_Image,
                DefectRegion = SelectedResult.Region,
                DarkRegion = SelectedResult.DarkRegion,
                LightRegion = SelectedResult.LightRegion,
                InvalidRegion = SelectedResult.InvalidRegion,
                WidthPx = SelectedResult.WidthPx,
                HeightPx = SelectedResult.HeightPx,
                PixelToMmH = SelectedResult.PixelToMmH,
                PixelToMmV = SelectedResult.PixelToMmV,
                ReferenceGray = SelectedResult.ReferenceGray
            }, SelectedResult.Features);

            RefreshFeatureDetailRows();
            RefreshSelectedFeatureSummary();
            AddDebug(string.Format("已获取缺陷#{0}全部分类特征={1}项，可用于选择分类特征和设定规则范围。", SelectedResult.Index, FeatureCatalog.All.Count));
            StatusText = string.Format("已获取缺陷#{0}的全部分类特征，可用于分类规则选型和范围设定。", SelectedResult.Index);
        }

        /// <summary>把当前选中缺陷中新补算的特征同步回摘要表对应行。</summary>
        private void RefreshSelectedFeatureSummary()
        {
            if (SelectedResult == null) return;
            DefectFeatureSummaryRow row = FeatureRows.FirstOrDefault(item => item.DefectIndex == SelectedResult.Index);
            if (row == null) return;

            row.Contrast = SelectedResult.Features.Get(FeatureCatalog.KeyOf(PicFeatureType.BrightDarkDiff), double.NaN);
            row.Roundness = SelectedResult.Features.Get(FeatureCatalog.KeyOf(PicFeatureType.DefectRoundness), double.NaN);
            row.WidthHeightRate = SelectedResult.Features.Get(FeatureCatalog.KeyOf(PicFeatureType.WidthHeightRate), double.NaN);
            row.BrightArea = SelectedResult.Features.Get(FeatureCatalog.KeyOf(PicFeatureType.BrightArea), double.NaN);
            row.DarkArea = SelectedResult.Features.Get(FeatureCatalog.KeyOf(PicFeatureType.DarkArea), double.NaN);
        }

        /// <summary>刷新当前选中缺陷的特征明细表，先按 FeatureCatalog 顺序显示，再补充未登记的扩展特征。</summary>
        private void RefreshFeatureDetailRows()
        {
            FeatureDetailRows.Clear();
            if (SelectedResult == null || SelectedResult.Features == null) return;

            var featureOptions = FeatureCatalog.All.ToList();
            var catalogKeys = featureOptions.Select(item => item.FeatureKey).ToList();
            foreach (FeatureTypeOption option in featureOptions)
            {
                double value;
                if (!SelectedResult.Features.TryGet(option.FeatureKey, out value)) continue;
                FeatureDetailRows.Add(new FeatureValueRow
                {
                    DefectIndex = SelectedResult.Index,
                    FeatureName = option.FeatureKey,
                    DisplayName = option.DisplayName,
                    Value = value
                });
            }

            foreach (var item in SelectedResult.Features.Values.OrderBy(item => item.Key))
            {
                if (catalogKeys.Any(key => string.Equals(key, item.Key, StringComparison.OrdinalIgnoreCase))) continue;

                FeatureDetailRows.Add(new FeatureValueRow
                {
                    DefectIndex = SelectedResult.Index,
                    FeatureName = item.Key,
                    DisplayName = FeatureCatalog.GetDisplayNameByKey(item.Key),
                    Value = item.Value
                });
            }

            AddDebug(string.Format("缺陷#{0} 当前已缓存特征项={1}", SelectedResult.Index, FeatureDetailRows.Count));
        }

        /// <summary>
        /// 显示当前选中缺陷的 Region 叠加预览。
        /// GetSelectedRegion 返回的是结果 Region 的副本，本方法 finally 中负责释放该副本。
        /// </summary>
        private void ShowRegionPreview(bool fit)
        {
            if (regionPreview == null) return;
            if (Params.ho_Image == null || !Params.ho_Image.IsInitialized())
            {
                regionPreview.Clear();
                return;
            }

            HObject region = GetSelectedRegion();
            try
            {
                regionPreview.ShowImage(Params.ho_Image, fit);
                HObject red = IsSelectedClassified() ? region : null;
                HObject green = IsSelectedUnclassified() ? region : null;
                HObject orange = IsSelectedNotMeet() ? region : null;
                regionPreview.ShowOverlay(null, null, red, green, orange);
            }
            finally
            {
                region?.Dispose();
            }
        }

        /// <summary>
        /// 显示当前选中缺陷的小图预览。
        /// CropImage 使用副本显示，显示结束后释放副本，避免释放结果对象持有的原始小图。
        /// </summary>
        private void ShowSmallPreview(bool fit)
        {
            if (smallPreview == null) return;
            HObject crop = GetSelectedCropImage();
            if (crop == null)
            {
                smallPreview.ShowImage(Params.ho_Image, fit);
                return;
            }

            try
            {
                smallPreview.ShowImage(crop, fit);
                smallPreview.ShowOverlay(null, null, null, null, null);
            }
            finally
            {
                crop.Dispose();
            }
        }

        /// <summary>
        /// 显示预处理图和分类叠加区域。
        /// 这里直接把 Params 输出对象交给预览适配器显示，适配器不接管业务 HObject 所有权。
        /// </summary>
        private void ShowPreprocessPreview(bool fit)
        {
            if (preprocessPreview == null) return;
            HObject image = Params.ho_PreprocessImage ?? Params.ho_Image;
            if (image == null || !image.IsInitialized())
            {
                preprocessPreview.Clear();
                return;
            }

            preprocessPreview.ShowImage(image, fit);
            preprocessPreview.ShowOverlay(Params.ho_DarkRegion, Params.ho_LightRegion, Params.ho_ClassifiedRegion, Params.ho_UnclassifiedRegion, Params.ho_NotMeetRegion);
        }

        /// <summary>判断当前结果是否属于“未达到设定要求”的橙色预览类别。</summary>
        private bool IsSelectedNotMeet()
        {
            return SelectedResult != null && string.Equals(SelectedResult.Source, "未达到设定要求", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>判断当前结果是否属于未分类候选，用于绿色预览叠加。</summary>
        private bool IsSelectedUnclassified()
        {
            return SelectedResult != null
                && !IsSelectedNotMeet()
                && (string.Equals(SelectedResult.ClassName, "未分类", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(SelectedResult.Source, "未分类", StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>判断当前结果是否已被某个分类规则或分类器命中，用于红色预览叠加。</summary>
        private bool IsSelectedClassified()
        {
            return SelectedResult != null && !IsSelectedNotMeet() && !IsSelectedUnclassified();
        }

        /// <summary>复制当前选中缺陷的 Region，调用方负责 Dispose 返回值。</summary>
        private HObject GetSelectedRegion()
        {
            if (SelectedResult == null || SelectedResult.Region == null || !SelectedResult.Region.IsInitialized()) return null;
            return SelectedResult.Region.CopyObj(1, -1);
        }

        /// <summary>复制当前选中缺陷的小图，调用方负责 Dispose 返回值。</summary>
        private HObject GetSelectedCropImage()
        {
            if (SelectedResult == null || SelectedResult.CropImage == null || !SelectedResult.CropImage.IsInitialized()) return null;
            return SelectedResult.CropImage.CopyObj(1, -1);
        }

        /// <summary>预处理预览窗口鼠标移动回调，只替换显示前缀，不改变坐标值。</summary>
        private void OnPreprocessMouseMove(HalconMouseInfoArgs e)
        {
            PreprocessMouseInfo = e.Text.Replace("图像", "预处理图");
        }
    }
}



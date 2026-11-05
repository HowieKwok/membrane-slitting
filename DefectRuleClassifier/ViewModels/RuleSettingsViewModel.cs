using CommunityToolkit.Mvvm.Input;
using DefectRuleClassifier.Core.Classifiers;
using DefectRuleClassifier.Core.Models;
using DefectRuleClassifier.Services;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace DefectRuleClassifier.ViewModels
{
    /// <summary>
    /// 分类规则独立编辑视图模型。
    /// 规则窗口只负责编辑、加载、保存和向主界面发出保存事件，不直接参与检测流程。
    /// </summary>
    public partial class RuleSettingsViewModel : BindableBase
    {
        /// <summary>规则窗口左侧 NG 类型表格当前选中的类别。</summary>
        private NgTypeRule selectedNgType;

        /// <summary>当前类别下方特征表格选中的单条特征范围规则。</summary>
        private FeatureRule selectedFeature;

        /// <summary>当前特征规则在特征字典中的显示对象，用于 ComboBox 显示中文名称、说明和范围。</summary>
        private FeatureTypeOption selectedFeatureOption;

        /// <summary>特征值分类规则 XML 路径。该文件是分类规则主入口。</summary>
        private string ruleConfigPath;

        /// <summary>传统分类器参数 XML 路径。真实模型路径和数量限制保存在该文件中。</summary>
        private string modelConfigPath;

        /// <summary>AI 分类参数 XML 路径。真实 AI 模型路径和说明保存在该文件中。</summary>
        private string aiConfigPath;

        /// <summary>规则窗口顶部保存状态提示。</summary>
        private string saveStatusText = "尚未保存";

        /// <summary>
        /// 创建分类规则编辑窗口的 ViewModel。
        /// 这里会把主流程当前正在使用的规则复制到窗口自己的编辑对象中，
        /// 用户在弹窗里编辑时不会立即影响正在运行的插件对象，只有点击保存并触发 Saved 事件后，主界面才重新 InitObj。
        /// </summary>
        /// <param name="sourceConfig">主流程当前使用的特征值分类规则。</param>
        /// <param name="rulePath">特征值分类规则文件路径。</param>
        /// <param name="modelConfig">主流程当前加载的传统分类器参数。</param>
        /// <param name="aiConfig">主流程当前加载的 AI 分类参数。</param>
        public RuleSettingsViewModel(
            FeatureRuleClassifierConfig sourceConfig,
            string rulePath,
            ModelClassifierConfig modelConfig,
            AiClassifierConfig aiConfig)
        {
            // 特征值分类规则是规则窗口的主体。
            // 复制一份而不是直接引用 sourceConfig，是为了避免用户编辑到一半关闭窗口时污染主流程参数。
            FeatureRuleConfig = new FeatureRuleClassifierConfig();
            CopyConfig(sourceConfig, FeatureRuleConfig);
            RuleConfigPath = DefaultConfigPathService.EnsureFeatureRulePath(rulePath);

            // 传统分类器和 AI 参数是独立 XML。
            // 规则主文件只保存这两个 XML 的路径，后续插件 InitObj 时再按路径分别加载。
            ModelRuleConfig = new ModelClassifierConfig();
            AiRuleConfig = new AiClassifierConfig();
            CopyModelConfig(modelConfig, ModelRuleConfig);
            CopyAiConfig(aiConfig, AiRuleConfig);
            ModelConfigPath = string.IsNullOrWhiteSpace(ModelRuleConfig.ConfigPath)
                ? DefaultConfigPathService.GetDefaultModelClassifierPath(RuleConfigPath)
                : ModelRuleConfig.ConfigPath;
            AiConfigPath = string.IsNullOrWhiteSpace(AiRuleConfig.ConfigPath)
                ? DefaultConfigPathService.GetDefaultAiClassifierPath(RuleConfigPath)
                : AiRuleConfig.ConfigPath;

            FeatureTypes = new ObservableCollection<FeatureTypeOption>(FeatureCatalog.All);
            JudgeConditionModes = new ObservableCollection<JudgeConditionModeOption>
            {
                new JudgeConditionModeOption { Value = JudgeConditionMode.AllConditionAnd, DisplayName = "所有条件且", Remark = "长度、宽度、面积全部满足才通过" },
                new JudgeConditionModeOption { Value = JudgeConditionMode.AllConditionOr, DisplayName = "所有条件或", Remark = "长度、宽度、面积任意一项满足就通过" },
                new JudgeConditionModeOption { Value = JudgeConditionMode.LengthAndWidthOrArea, DisplayName = "(长&&宽)||面积", Remark = "长宽同时满足，或面积满足" },
                new JudgeConditionModeOption { Value = JudgeConditionMode.LengthOrWidthAndArea, DisplayName = "(长||宽)&&面积", Remark = "长度或宽度任意一项满足，并且面积满足" }
            };

            AddNgTypeCommand = new RelayCommand(AddNgType);
            DeleteNgTypeCommand = new RelayCommand(DeleteNgType, () => SelectedNgType != null);
            MoveNgUpCommand = new RelayCommand(() => MoveNgType(-1), () => SelectedNgType != null);
            MoveNgDownCommand = new RelayCommand(() => MoveNgType(1), () => SelectedNgType != null);
            AddFeatureCommand = new RelayCommand(AddFeature, () => SelectedNgType != null);
            DeleteFeatureCommand = new RelayCommand(DeleteFeature, () => SelectedFeature != null);
            AddJudgeConditionCommand = new RelayCommand(AddJudgeCondition, () => SelectedNgType != null);
            DeleteJudgeConditionCommand = new RelayCommand(DeleteJudgeCondition, () => SelectedNgType != null);
            SelectPathCommand = new RelayCommand(SelectPath);
            LoadCommand = new RelayCommand(LoadFromCurrentPath);
            SaveCommand = new RelayCommand(SaveToCurrentPath);
            ImportCommand = new RelayCommand(ImportRules);
            SelectModelConfigPathCommand = new RelayCommand(SelectModelConfigPath);
            SelectModelFilePathCommand = new RelayCommand(SelectModelFilePath);
            SelectLabelsPathCommand = new RelayCommand(SelectLabelsPath);
            LoadModelConfigCommand = new RelayCommand(LoadModelConfigFromPath);
            SelectAiConfigPathCommand = new RelayCommand(SelectAiConfigPath);
            SelectAiModelPathCommand = new RelayCommand(SelectAiModelPath);
            LoadAiConfigCommand = new RelayCommand(LoadAiConfigFromPath);

            // 默认选中第一类和第一条特征，让右侧表格/按钮状态能立即绑定。
            SelectedNgType = FeatureRuleConfig.NgTypes.FirstOrDefault();
            SelectedFeature = SelectedNgType == null ? null : SelectedNgType.Features.FirstOrDefault();
            SelectedFeatureOption = SelectedFeature == null ? FeatureTypes.FirstOrDefault() : FeatureCatalog.Get(SelectedFeature.FeatureType);
            UpdateSaveStatus("规则已就绪");
        }

        /// <summary>
        /// 规则保存完成事件。
        /// 主界面打开规则窗口时注册该事件；窗口保存后主界面复制规则并调用 InitObj，窗口关闭或重开时解除注册，降低耦合。
        /// </summary>
        public event EventHandler Saved;

        /// <summary>触发“已保存”事件（分类器工具保存绑定后调用，通知插件重新加载配置）。</summary>
        public void RaiseSaved()
        {
            Saved?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>窗口正在编辑的特征值分类规则副本。</summary>
        public FeatureRuleClassifierConfig FeatureRuleConfig { get; }

        /// <summary>窗口正在编辑的传统分类器参数副本。</summary>
        public ModelClassifierConfig ModelRuleConfig { get; }

        /// <summary>窗口正在编辑的 AI 分类参数副本。</summary>
        public AiClassifierConfig AiRuleConfig { get; }

        /// <summary>特征类型下拉框数据源，来自 FeatureCatalog，全中文显示。</summary>
        public ObservableCollection<FeatureTypeOption> FeatureTypes { get; }

        /// <summary>尺寸判定条件下拉框数据源。</summary>
        public ObservableCollection<JudgeConditionModeOption> JudgeConditionModes { get; }

        /// <summary>新增一个 NG 类型规则。</summary>
        public ICommand AddNgTypeCommand { get; }

        /// <summary>删除当前选中的 NG 类型规则。</summary>
        public ICommand DeleteNgTypeCommand { get; }

        /// <summary>提升当前 NG 类型优先级。NgTypes 顺序越靠前，分类时越先匹配。</summary>
        public ICommand MoveNgUpCommand { get; }

        /// <summary>降低当前 NG 类型优先级。</summary>
        public ICommand MoveNgDownCommand { get; }

        /// <summary>给当前 NG 类型新增一条特征范围条件。</summary>
        public ICommand AddFeatureCommand { get; }

        /// <summary>删除当前选中的特征范围条件。</summary>
        public ICommand DeleteFeatureCommand { get; }

        /// <summary>启用当前 NG 类型的尺寸判定。</summary>
        public ICommand AddJudgeConditionCommand { get; }

        /// <summary>关闭当前 NG 类型的尺寸判定。</summary>
        public ICommand DeleteJudgeConditionCommand { get; }

        /// <summary>选择特征值分类规则 XML 保存路径。</summary>
        public ICommand SelectPathCommand { get; }

        /// <summary>从当前规则路径加载特征值分类规则。</summary>
        public ICommand LoadCommand { get; }

        /// <summary>保存特征值分类规则、传统分类器参数和 AI 参数。</summary>
        public ICommand SaveCommand { get; }

        /// <summary>从外部 XML 导入一套特征值分类规则。</summary>
        public ICommand ImportCommand { get; }

        /// <summary>选择传统分类器参数 XML 保存路径。</summary>
        public ICommand SelectModelConfigPathCommand { get; }

        /// <summary>选择传统分类器模型文件路径。</summary>
        public ICommand SelectModelFilePathCommand { get; }
        public ICommand SelectLabelsPathCommand { get; }

        /// <summary>从当前传统分类器参数路径加载参数。</summary>
        public ICommand LoadModelConfigCommand { get; }

        /// <summary>选择 AI 参数 XML 保存路径。</summary>
        public ICommand SelectAiConfigPathCommand { get; }

        /// <summary>选择 AI 模型文件路径。</summary>
        public ICommand SelectAiModelPathCommand { get; }

        /// <summary>从当前 AI 参数路径加载参数。</summary>
        public ICommand LoadAiConfigCommand { get; }

        public string SaveStatusText
        {
            get => saveStatusText;
            set => SetProperty(ref saveStatusText, value);
        }

        public string RuleConfigPath
        {
            get => ruleConfigPath;
            set
            {
                // 路径显示和实际配置对象保持一致，保存时会写入该路径。
                if (SetProperty(ref ruleConfigPath, value))
                    FeatureRuleConfig.ConfigPath = value;
            }
        }

        public string ModelConfigPath
        {
            get => modelConfigPath;
            set
            {
                // 传统分类器参数路径既显示在界面，也写入 ModelRuleConfig.ConfigPath。
                if (SetProperty(ref modelConfigPath, value))
                    ModelRuleConfig.ConfigPath = value;
            }
        }

        public string AiConfigPath
        {
            get => aiConfigPath;
            set
            {
                // AI 参数路径既显示在界面，也写入 AiRuleConfig.ConfigPath。
                if (SetProperty(ref aiConfigPath, value))
                    AiRuleConfig.ConfigPath = value;
            }
        }

        public NgTypeRule SelectedNgType
        {
            get => selectedNgType;
            set
            {
                if (SetProperty(ref selectedNgType, value))
                {
                    // 切换类别时，右侧特征表自动切到该类别的第一条特征规则。
                    SelectedFeature = selectedNgType == null ? null : selectedNgType.Features.FirstOrDefault();
                    RaiseRuleCommands();
                }
            }
        }

        public FeatureRule SelectedFeature
        {
            get => selectedFeature;
            set
            {
                if (SetProperty(ref selectedFeature, value))
                {
                    // 选中特征规则后，同步特征下拉框显示对象，便于界面显示中文名称和说明。
                    SelectedFeatureOption = selectedFeature == null ? FeatureTypes.FirstOrDefault() : FeatureCatalog.Get(selectedFeature.FeatureType);
                    RaiseRuleCommands();
                }
            }
        }

        public FeatureTypeOption SelectedFeatureOption
        {
            get => selectedFeatureOption;
            set
            {
                // 用户在 ComboBox 切换特征类型时，真正保存的是 FeatureRule.FeatureType。
                if (SetProperty(ref selectedFeatureOption, value) && value != null && SelectedFeature != null)
                    SelectedFeature.FeatureType = value.Value;
            }
        }

        private void SelectPath()
        {
            // 这里只选择特征值分类规则主文件路径。
            // 传统分类器参数和 AI 参数有各自路径，不直接混在主参数里。
            SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = "分类规则参数|*.json",
                FileName = Path.GetFileName(DefaultConfigPathService.EnsureFeatureRulePath(RuleConfigPath)),
                InitialDirectory = Path.GetDirectoryName(DefaultConfigPathService.EnsureFeatureRulePath(RuleConfigPath)),
                Title = "选择分类规则保存路径"
            };
            if (dialog.ShowDialog() != true) return;
            RuleConfigPath = dialog.FileName;
            UpdateSaveStatus("规则路径已设置，点击保存后写入完整分类参数");
        }

        private void LoadFromCurrentPath()
        {
            // 防呆：路径为空或文件不存在时不清空当前编辑内容，只提示状态。
            if (string.IsNullOrWhiteSpace(RuleConfigPath) || !File.Exists(RuleConfigPath))
            {
                UpdateSaveStatus("规则文件不存在");
                return;
            }

            LoadFromFile(RuleConfigPath);
        }

        private void SaveToCurrentPath()
        {
            // 路径为空时使用默认路径，避免保存时生成空路径异常。
            if (string.IsNullOrWhiteSpace(RuleConfigPath))
            {
                RuleConfigPath = DefaultConfigPathService.GetDefaultFeatureRulePath();
            }

            // 分类器参数和 AI 参数分别落盘，特征规则文件只记录它们的参数路径。
            // 这样多个检测模块可以复用同一套分类规则入口，也能单独替换分类器或 AI 参数。
            string modelPath = string.IsNullOrWhiteSpace(ModelConfigPath)
                ? DefaultConfigPathService.GetDefaultModelClassifierPath(RuleConfigPath)
                : ModelConfigPath;
            string aiPath = string.IsNullOrWhiteSpace(AiConfigPath)
                ? DefaultConfigPathService.GetDefaultAiClassifierPath(RuleConfigPath)
                : AiConfigPath;

            ModelRuleConfig.Save(modelPath);
            AiRuleConfig.Save(aiPath);
            ModelConfigPath = modelPath;
            AiConfigPath = aiPath;

            // 特征规则文件是主入口，里面只记录分类器和 AI 的参数文件路径。
            // 真正的模型路径、数量限制等仍然保存在各自独立 XML 中。
            FeatureRuleConfig.ModelClassifierConfigPath = ModelConfigPath;
            FeatureRuleConfig.AiClassifierConfigPath = AiConfigPath;
            FeatureRuleConfig.Save(RuleConfigPath);

            UpdateSaveStatus("规则保存成功");
            Saved?.Invoke(this, EventArgs.Empty);
        }

        private void ImportRules()
        {
            // 导入外部规则时，会把窗口当前编辑副本替换为导入文件内容。
            // 保存前不会自动影响主流程；保存成功后通过 Saved 事件通知主界面重新加载。
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "分类规则或模块参数|*.xml|所有文件|*.*",
                Title = "选择分类规则文件"
            };
            if (dialog.ShowDialog() != true) return;

            RuleConfigPath = dialog.FileName;
            LoadFromFile(dialog.FileName);
            UpdateSaveStatus("规则已导入");
        }

        private void SelectModelConfigPath()
        {
            // 选择传统分类器参数 XML 的保存路径，不是选择模型文件。
            SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = "分类规则参数|*.json",
                FileName = string.IsNullOrWhiteSpace(ModelConfigPath) ? "ModelClassifierConfig.xml" : Path.GetFileName(ModelConfigPath),
                Title = "选择分类器参数保存路径"
            };
            if (dialog.ShowDialog() != true) return;
            ModelConfigPath = dialog.FileName;
            UpdateSaveStatus("分类器参数路径已设置");
        }

        private void SelectModelFilePath()
        {
            // 选择真实分类器模型文件路径。当前 ModelClassifierPlaceholder 只保存路径，不执行真实分类。
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "ONNX 模型|*.onnx|分类器模型|*.svm;*.mlp;*.model;*.xml;*.dat|所有文件|*.*",
                Title = "选择分类器模型文件"
            };
            if (dialog.ShowDialog() != true) return;
            ModelRuleConfig.ModelPath = dialog.FileName;
            OnPropertyChanged(nameof(ModelRuleConfig));
        }


        private void SelectLabelsPath()
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "标签文件|*.txt|所有文件|*.*",
                Title = "选择模型标签文件（labels.txt，顺序即标签序号，0 起）"
            };
            if (dialog.ShowDialog() != true) return;
            ModelRuleConfig.LabelsPath = dialog.FileName;
            OnPropertyChanged(nameof(ModelRuleConfig));
        }

        private void LoadModelConfigFromPath()
        {
            // 单独加载分类器参数，便于多个模块共用同一个分类器配置。
            if (string.IsNullOrWhiteSpace(ModelConfigPath) || !File.Exists(ModelConfigPath))
            {
                UpdateSaveStatus("未找到分类器参数文件");
                return;
            }

            ModelClassifierConfig loaded = ModelClassifierConfig.LoadOrDefault(ModelConfigPath);
            CopyModelConfig(loaded, ModelRuleConfig);
            ModelConfigPath = string.IsNullOrWhiteSpace(ModelRuleConfig.ConfigPath) ? ModelConfigPath : ModelRuleConfig.ConfigPath;
            OnPropertyChanged(nameof(ModelRuleConfig));
            UpdateSaveStatus("分类器参数已加载");
        }

        private void SelectAiConfigPath()
        {
            // 选择 AI 参数 XML 的保存路径，不是选择 AI 模型文件。
            SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = "分类规则参数|*.json",
                FileName = string.IsNullOrWhiteSpace(AiConfigPath) ? "AiClassifierConfig.xml" : Path.GetFileName(AiConfigPath),
                Title = "选择AI参数保存路径"
            };
            if (dialog.ShowDialog() != true) return;
            AiConfigPath = dialog.FileName;
            UpdateSaveStatus("AI参数路径已设置");
        }

        private void SelectAiModelPath()
        {
            // 选择真实 AI 模型路径。当前 AiClassifierPlaceholder 只保存路径，不执行真实推理。
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "AI模型|*.onnx;*.pt;*.pth;*.engine;*.model;*.xml|所有文件|*.*",
                Title = "选择AI模型文件"
            };
            if (dialog.ShowDialog() != true) return;
            AiRuleConfig.ModelPath = dialog.FileName;
            OnPropertyChanged(nameof(AiRuleConfig));
        }

        private void LoadAiConfigFromPath()
        {
            // 单独加载 AI 参数，便于多个模块共用同一个 AI 配置。
            if (string.IsNullOrWhiteSpace(AiConfigPath) || !File.Exists(AiConfigPath))
            {
                UpdateSaveStatus("未找到AI参数文件");
                return;
            }

            AiClassifierConfig loaded = AiClassifierConfig.LoadOrDefault(AiConfigPath);
            CopyAiConfig(loaded, AiRuleConfig);
            AiConfigPath = string.IsNullOrWhiteSpace(AiRuleConfig.ConfigPath) ? AiConfigPath : AiRuleConfig.ConfigPath;
            OnPropertyChanged(nameof(AiRuleConfig));
            UpdateSaveStatus("AI参数已加载");
        }

        private void LoadFromFile(string fileName)
        {
            // 加载特征规则主文件后，再根据主文件内记录的路径去加载分类器参数和 AI 参数。
            FeatureRuleClassifierConfig loaded = FeatureRuleClassifierConfig.LoadOrDefault(fileName);
            CopyConfig(loaded, FeatureRuleConfig);
            RuleConfigPath = DefaultConfigPathService.EnsureFeatureRulePath(fileName);
            ModelConfigPath = string.IsNullOrWhiteSpace(FeatureRuleConfig.ModelClassifierConfigPath)
                ? DefaultConfigPathService.GetDefaultModelClassifierPath(RuleConfigPath)
                : FeatureRuleConfig.ModelClassifierConfigPath;
            AiConfigPath = string.IsNullOrWhiteSpace(FeatureRuleConfig.AiClassifierConfigPath)
                ? DefaultConfigPathService.GetDefaultAiClassifierPath(RuleConfigPath)
                : FeatureRuleConfig.AiClassifierConfigPath;
            if (!string.IsNullOrWhiteSpace(ModelConfigPath) && File.Exists(ModelConfigPath))
                CopyModelConfig(ModelClassifierConfig.LoadOrDefault(ModelConfigPath), ModelRuleConfig);
            if (!string.IsNullOrWhiteSpace(AiConfigPath) && File.Exists(AiConfigPath))
                CopyAiConfig(AiClassifierConfig.LoadOrDefault(AiConfigPath), AiRuleConfig);
            SelectedNgType = FeatureRuleConfig.NgTypes.FirstOrDefault();
            SelectedFeature = SelectedNgType == null ? null : SelectedNgType.Features.FirstOrDefault();
            OnPropertyChanged(nameof(FeatureRuleConfig));
            OnPropertyChanged(nameof(ModelRuleConfig));
            OnPropertyChanged(nameof(AiRuleConfig));
            UpdateSaveStatus("规则已加载");
        }

        private void AddNgType()
        {
            // 新增类别默认给一个放开的尺寸范围，避免刚创建就因为尺寸判定全部不通过。
            // 具体特征范围由用户继续添加 FeatureRule。
            FeatureRuleConfig.NgTypes.Add(new NgTypeRule
            {
                Name = "新类别",
                EnableJudgeCondition = false,
                JudgeCondition = JudgeConditionMode.AllConditionAnd,
                MinLength = 0,
                MaxLength = 999999999,
                MinWidth = 0,
                MaxWidth = 999999999,
                MinArea = 0,
                MaxArea = 999999999
            });
            SelectedNgType = FeatureRuleConfig.NgTypes.LastOrDefault();
        }

        private void DeleteNgType()
        {
            // 删除类别后重新选中第一类，保证界面 SelectedNgType 不指向已经移除的对象。
            if (SelectedNgType == null) return;
            FeatureRuleConfig.NgTypes.Remove(SelectedNgType);
            SelectedNgType = FeatureRuleConfig.NgTypes.FirstOrDefault();
        }

        private void MoveNgType(int step)
        {
            // NgTypes 的顺序就是特征分类优先级，向上移动表示更早匹配。
            if (SelectedNgType == null) return;
            int oldIndex = FeatureRuleConfig.NgTypes.IndexOf(SelectedNgType);
            int newIndex = oldIndex + step;
            if (oldIndex < 0 || newIndex < 0 || newIndex >= FeatureRuleConfig.NgTypes.Count) return;
            FeatureRuleConfig.NgTypes.Move(oldIndex, newIndex);
        }

        private void AddFeature()
        {
            // 新增特征规则默认使用“缺陷灰度”，范围放开。
            // 用户应在表格中选择实际需要的特征类型，并设置最小/最大值。
            if (SelectedNgType == null) return;
            FeatureRule rule = new FeatureRule
            {
                FeatureType = PicFeatureType.DefectGray,
                MinValue = 0,
                MaxValue = 999999999
            };
            SelectedNgType.Features.Add(rule);
            SelectedFeature = rule;
        }

        private void DeleteFeature()
        {
            // 仅删除当前类别下选中的特征范围条件，不影响其他类别。
            if (SelectedNgType == null || SelectedFeature == null) return;
            SelectedNgType.Features.Remove(SelectedFeature);
            SelectedFeature = SelectedNgType.Features.FirstOrDefault();
        }

        private void AddJudgeCondition()
        {
            // 尺寸判定是类别级条件：先判断该类别的长/宽/面积要求，再判断特征范围。
            if (SelectedNgType == null) return;
            SelectedNgType.EnableJudgeCondition = true;
            SelectedNgType.JudgeCondition = JudgeConditionMode.AllConditionAnd;
        }

        private void DeleteJudgeCondition()
        {
            // 关闭尺寸判定后，该类别只按特征范围条件判断。
            if (SelectedNgType == null) return;
            SelectedNgType.EnableJudgeCondition = false;
        }

        private void RaiseRuleCommands()
        {
            // 选中项变化后刷新按钮可用状态，避免无选中对象时仍可点击删除/移动。
            (DeleteNgTypeCommand as RelayCommand)?.NotifyCanExecuteChanged();
            (MoveNgUpCommand as RelayCommand)?.NotifyCanExecuteChanged();
            (MoveNgDownCommand as RelayCommand)?.NotifyCanExecuteChanged();
            (AddFeatureCommand as RelayCommand)?.NotifyCanExecuteChanged();
            (DeleteFeatureCommand as RelayCommand)?.NotifyCanExecuteChanged();
            (DeleteJudgeConditionCommand as RelayCommand)?.NotifyCanExecuteChanged();
            (AddJudgeConditionCommand as RelayCommand)?.NotifyCanExecuteChanged();
        }

        private void UpdateSaveStatus(string text)
        {
            // 状态文本统一从这里更新，避免界面出现空状态。
            SaveStatusText = string.IsNullOrWhiteSpace(text) ? "尚未保存" : text;
        }

        private static void CopyConfig(FeatureRuleClassifierConfig source, FeatureRuleClassifierConfig target)
        {
            // 深拷贝分类规则，避免规则窗口编辑副本和主流程运行对象共用同一个集合实例。
            // 如果这里只做引用赋值，用户在弹窗未保存时也会直接修改主流程参数。
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
                foreach (JudgeConditionTemplate item in source.JudgeConditionTemplates)
                    target.JudgeConditionTemplates.Add(CloneTemplate(item));

            target.NgTypes.Clear();
            if (source.NgTypes != null)
                foreach (NgTypeRule item in source.NgTypes)
                    target.NgTypes.Add(CloneNgType(item));

            // DarkNgTypes 当前不参与运行分类，只为兼容旧 XML 保留并原样复制。
            target.DarkNgTypes.Clear();
            if (source.DarkNgTypes != null)
                foreach (NgTypeRule item in source.DarkNgTypes)
                    target.DarkNgTypes.Add(CloneNgType(item));
        }

        private static NgTypeRule CloneNgType(NgTypeRule source)
        {
            // 克隆一个 NG 类型：包含类别名、尺寸判定和全部特征范围条件。
            if (source == null) return null;

            NgTypeRule target = new NgTypeRule
            {
                Name = source.Name,
                EnableJudgeCondition = source.EnableJudgeCondition,
                JudgeCondition = source.JudgeCondition,
                MinLength = source.MinLength,
                MaxLength = source.MaxLength,
                MinWidth = source.MinWidth,
                MaxWidth = source.MaxWidth,
                MinArea = source.MinArea,
                MaxArea = source.MaxArea
            };

            if (source.Features != null)
                foreach (FeatureRule feature in source.Features)
                    target.Features.Add(CloneFeatureRule(feature));

            return target;
        }

        private static FeatureRule CloneFeatureRule(FeatureRule source)
        {
            // 克隆单条特征规则：特征类型 + 最小值 + 最大值。
            if (source == null) return null;
            return new FeatureRule
            {
                FeatureType = source.FeatureType,
                MinValue = source.MinValue,
                MaxValue = source.MaxValue
            };
        }

        private static JudgeConditionTemplate CloneTemplate(JudgeConditionTemplate source)
        {
            // 判定条件模板目前主要用于兼容旧参数和说明显示。
            if (source == null) return null;
            return new JudgeConditionTemplate
            {
                Id = source.Id,
                Name = source.Name,
                Expression = source.Expression,
                Remark = source.Remark
            };
        }

        private static void CopyModelConfig(ModelClassifierConfig source, ModelClassifierConfig target)
        {
            // 深拷贝传统分类器参数。模型对象本身不在这里加载，只保存模型路径和数量限制。
            if (source == null || target == null) return;
            target.ConfigPath = source.ConfigPath;
            target.ModelPath = source.ModelPath;
            target.LabelsPath = source.LabelsPath;
            target.IndexToFeatureLabel.Clear();
            if (source.IndexToFeatureLabel != null)
                foreach (ModelLabelMapItem item in source.IndexToFeatureLabel)
                    target.IndexToFeatureLabel.Add(new ModelLabelMapItem { Index = item.Index, FeatureLabel = item.FeatureLabel });
            target.EnableCandidateCountLimit = source.EnableCandidateCountLimit;
            target.MaxCandidateCount = source.MaxCandidateCount;
            target.EnableMaxEnterClassifierLimit = source.EnableMaxEnterClassifierLimit;
            target.MaxEnterClassifierCount = source.MaxEnterClassifierCount;
            target.OverflowClassName = source.OverflowClassName;
        }

        private static void CopyAiConfig(AiClassifierConfig source, AiClassifierConfig target)
        {
            // 深拷贝 AI 参数。真实 AI 推理对象由后续 InitObj 根据路径初始化。
            if (source == null || target == null) return;
            target.ConfigPath = source.ConfigPath;
            target.ModelPath = source.ModelPath;
            target.Remark = source.Remark;
        }
    }
}

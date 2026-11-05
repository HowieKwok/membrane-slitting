using Plugin.ReusableMeasureDetect.PointFilters.Abstractions;
using Plugin.ReusableMeasureDetect.PointFilters.Core;
using Frame_Core.pluginParams;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Input;

namespace Plugin.ReusableMeasureDetect.PointFilters.ViewModels
{
    /// <summary>
    /// 点集滤波编辑器 ViewModel。
    /// 负责维护“已添加滤波器、可添加滤波器、滤波步骤、当前滤波参数字段”的 MVVM 状态，并把界面操作提交到 PointFilterOptionsParam。
    /// </summary>
    public sealed class PointFilterEditorViewModel : BindableBase, IPointFilterEditor
    {
        private PointFilterOptionsParam options;
        private PointFilterOutputParam output;
        private bool loadingFromOptions;
        private FilterItemViewModel selectedFilter;
        private FilterItemViewModel selectedAvailableFilter;
        private FilterStepItemViewModel selectedFilterStep;

        /// <summary>
        /// 初始化滤波编辑命令和默认列表。
        /// </summary>
        public PointFilterEditorViewModel()
        {
            AddFilterCommand = new RelayCommand<object>(_ => AddSelectedFilter());
            DeleteFilterCommand = new RelayCommand<object>(_ => DeleteSelectedFilter());
            MoveFilterUpCommand = new RelayCommand<object>(_ => MoveSelectedFilter(-1));
            MoveFilterDownCommand = new RelayCommand<object>(_ => MoveSelectedFilter(1));
            RefreshAvailableFilters();
            RefreshSteps();
        }

        /// <summary>
        /// 当滤波步骤或显示点集变化时通知宿主重新绘制叠加层。
        /// </summary>
        public event Action DisplayInvalidated;
        public PointFilterOptionsParam Options { get { EnsureOptions(); return options; } }
        public PointFilterOutputParam Output { get { EnsureOutput(); return output; } }
        public ObservableCollection<FilterItemViewModel> FilterPipeline { get; } = new ObservableCollection<FilterItemViewModel>();
        public ObservableCollection<FilterItemViewModel> AvailableFilters { get; } = new ObservableCollection<FilterItemViewModel>();
        public ObservableCollection<FilterStepItemViewModel> FilterStepItems { get; } = new ObservableCollection<FilterStepItemViewModel>();
        public ObservableCollection<FilterParameterFieldViewModel> SelectedFilterParameterFields { get; } = new ObservableCollection<FilterParameterFieldViewModel>();
        public ICommand AddFilterCommand { get; }
        public ICommand DeleteFilterCommand { get; }
        public ICommand MoveFilterUpCommand { get; }
        public ICommand MoveFilterDownCommand { get; }

        public FilterItemViewModel SelectedFilter
        {
            get { return selectedFilter; }
            set
            {
                if (!SetProperty(ref selectedFilter, value)) return;
                NotifySelectedFilterProperties();
            }
        }

        public FilterItemViewModel SelectedAvailableFilter
        {
            get { return selectedAvailableFilter; }
            set { SetProperty(ref selectedAvailableFilter, value); }
        }

        public FilterStepItemViewModel SelectedFilterStep
        {
            get { return selectedFilterStep; }
            set
            {
                if (!SetProperty(ref selectedFilterStep, value)) return;
                if (loadingFromOptions) return;
                EnsureOptions();
                options.SelectedStepIndex = value == null ? -1 : value.StepIndex;
                PointFilterPipelineCore.SelectStepToOutput(options, Output, options.SelectedStepIndex);
                RaiseDisplayInvalidated();
            }
        }

        public string SelectedFilterParamTitle
        {
            get
            {
                return SelectedFilter == null
                    ? "当前滤波参数 / Selected Filter Params：请先添加并选择滤波"
                    : "当前滤波参数 / Selected Filter Params：" + SelectedFilter.DisplayName;
            }
        }

        public bool IsMeanSelected { get { return IsSelectedFilter("Mean"); } }
        public bool IsGaussianSelected { get { return IsSelectedFilter("Gaussian"); } }
        public bool IsMedianSelected { get { return IsSelectedFilter("Median"); } }
        public bool IsBilateralSelected { get { return IsSelectedFilter("Bilateral"); } }
        public bool IsGuidedSelected { get { return IsSelectedFilter("Guided"); } }
        public bool IsCurvatureFlowSelected { get { return IsSelectedFilter("CurvatureFlow"); } }
        public bool IsSobelSelected { get { return IsSelectedFilter("Sobel"); } }
        public bool IsLaplacianSelected { get { return IsSelectedFilter("Laplacian"); } }
        public bool IsGaborSelected { get { return IsSelectedFilter("Gabor"); } }
        public bool HasSelectedFilter { get { return SelectedFilter != null; } }
        public bool HasNoSelectedFilter { get { return SelectedFilter == null; } }

        public bool EnableFilter
        {
            get { return Options.Enable; }
            set { Options.Enable = value; OnPropertyChanged(nameof(EnableFilter)); }
        }

        public bool ShowSelectedPointSet
        {
            get { return Options.ShowSelectedPointSet; }
            set { Options.ShowSelectedPointSet = value; OnPropertyChanged(nameof(ShowSelectedPointSet)); RaiseDisplayInvalidated(); }
        }

        public string FilterPreSortMode
        {
            get { return Options.PreSortMode; }
            set { Options.PreSortMode = NormalizeSortMode(value); OnPropertyChanged(nameof(FilterPreSortMode)); }
        }

        public double FilterCrossSize
        {
            get { return Options.DisplayCrossSize; }
            set
            {
                Options.DisplayCrossSize = Math.Max(2.0, value);
                OnPropertyChanged(nameof(FilterCrossSize));
                PointFilterPipelineCore.SelectStepToOutput(Options, Output, Options.SelectedStepIndex);
                RaiseDisplayInvalidated();
            }
        }

        public int MeanRadius
        {
            get { return Options.MeanRadius; }
            set { Options.MeanRadius = Math.Max(1, value); OnPropertyChanged(nameof(MeanRadius)); }
        }

        public int GaussianRadius
        {
            get { return Options.GaussianRadius; }
            set { Options.GaussianRadius = Math.Max(1, value); OnPropertyChanged(nameof(GaussianRadius)); }
        }

        public double GaussianSigma
        {
            get { return Options.GaussianSigma; }
            set { Options.GaussianSigma = Math.Max(0.1, value); OnPropertyChanged(nameof(GaussianSigma)); }
        }

        public int MedianRadius
        {
            get { return Options.MedianRadius; }
            set { Options.MedianRadius = Math.Max(1, value); OnPropertyChanged(nameof(MedianRadius)); }
        }

        public int BilateralRadius
        {
            get { return Options.BilateralRadius; }
            set { Options.BilateralRadius = Math.Max(1, value); OnPropertyChanged(nameof(BilateralRadius)); }
        }

        public double BilateralSpatialSigma
        {
            get { return Options.BilateralSpatialSigma; }
            set { Options.BilateralSpatialSigma = Math.Max(0.1, value); OnPropertyChanged(nameof(BilateralSpatialSigma)); }
        }

        public double BilateralRangeSigma
        {
            get { return Options.BilateralRangeSigma; }
            set { Options.BilateralRangeSigma = Math.Max(0.1, value); OnPropertyChanged(nameof(BilateralRangeSigma)); }
        }

        public int GuidedRadius
        {
            get { return Options.GuidedRadius; }
            set { Options.GuidedRadius = Math.Max(1, value); OnPropertyChanged(nameof(GuidedRadius)); }
        }

        public double GuidedEpsilon
        {
            get { return Options.GuidedEpsilon; }
            set { Options.GuidedEpsilon = Math.Max(0.000001, value); OnPropertyChanged(nameof(GuidedEpsilon)); }
        }

        public int CurvatureFlowIterations
        {
            get { return Options.CurvatureFlowIterations; }
            set { Options.CurvatureFlowIterations = Math.Max(1, value); OnPropertyChanged(nameof(CurvatureFlowIterations)); }
        }

        public double CurvatureFlowStep
        {
            get { return Options.CurvatureFlowStep; }
            set { Options.CurvatureFlowStep = Math.Max(0.001, value); OnPropertyChanged(nameof(CurvatureFlowStep)); }
        }

        public int SobelRadius
        {
            get { return Options.SobelRadius; }
            set { Options.SobelRadius = Math.Max(1, value); OnPropertyChanged(nameof(SobelRadius)); }
        }

        public double SobelAmount
        {
            get { return Options.SobelAmount; }
            set { Options.SobelAmount = Math.Max(0.0, value); OnPropertyChanged(nameof(SobelAmount)); }
        }

        public int LaplacianRadius
        {
            get { return Options.LaplacianRadius; }
            set { Options.LaplacianRadius = Math.Max(1, value); OnPropertyChanged(nameof(LaplacianRadius)); }
        }

        public double LaplacianAmount
        {
            get { return Options.LaplacianAmount; }
            set { Options.LaplacianAmount = Math.Max(0.0, value); OnPropertyChanged(nameof(LaplacianAmount)); }
        }

        public int GaborRadius
        {
            get { return Options.GaborRadius; }
            set { Options.GaborRadius = Math.Max(1, value); OnPropertyChanged(nameof(GaborRadius)); }
        }

        public double GaborSigma
        {
            get { return Options.GaborSigma; }
            set { Options.GaborSigma = Math.Max(0.1, value); OnPropertyChanged(nameof(GaborSigma)); }
        }

        /// <summary>
        /// 挂载宿主参数和输出对象，并把保存的滤波顺序、选中步骤加载到界面集合。
        /// </summary>
        public void Attach(PointFilterOptionsParam pointFilterOptions, PointFilterOutputParam pointFilterOutput)
        {
            options = pointFilterOptions ?? new PointFilterOptionsParam();
            output = pointFilterOutput ?? new PointFilterOutputParam();
            LoadFromOptions();
        }

        /// <summary>
        /// 将界面当前 Pipeline 顺序和选中步骤写回 Options，运行前和保存前必须调用。
        /// </summary>
        public void CommitToOptions()
        {
            EnsureOptions();
            SetAllFilterEnabled(options, false);
            List<string> ids = new List<string>();
            for (int index = 0; index < FilterPipeline.Count; index++)
            {
                FilterItemViewModel item = FilterPipeline[index];
                if (item == null || ContainsId(ids, item.Id)) continue;
                ids.Add(item.Id);
                SetFilterEnabled(options, item.Id, true);
            }
            options.FilterOrderCsv = string.Join(",", ids.ToArray());
            options.SelectedStepIndex = SelectedFilterStep == null ? -1 : SelectedFilterStep.StepIndex;
        }

        /// <summary>
        /// 根据 Output.Steps 刷新“输出步骤”下拉列表，保留自动最后一步选项。
        /// </summary>
        public void RefreshSteps()
        {
            FilterStepItems.Clear();
            FilterStepItems.Add(new FilterStepItemViewModel(-1, "自动最后一步 / Auto Last"));
            if (output != null && output.Steps != null)
            {
                for (int index = 0; index < output.Steps.Count; index++)
                {
                    PointFilterStepOutputParam step = output.Steps[index];
                    int pointCount = step.Points == null ? 0 : step.Points.Count;
                    FilterStepItems.Add(new FilterStepItemViewModel(index, string.Format(CultureInfo.InvariantCulture, "{0}: {1} ({2}点)", index, step.Name, pointCount)));
                }
            }
            SelectStepFromOptions();
        }

        /// <summary>
        /// 返回当前滤波输出选中的点集。
        /// </summary>
        public IReadOnlyList<MeasurePointParam> GetSelectedPoints()
        {
            return Output.SelectedPoints;
        }

        /// <summary>
        /// Pipeline 重新运行后刷新步骤列表并通知宿主重绘。
        /// </summary>
        public void RefreshFromPipelineOutput()
        {
            RefreshSteps();
            RaiseDisplayInvalidated();
        }

        /// <summary>
        /// 从 Options 加载界面状态；loadingFromOptions 防止加载过程中触发反向写回。
        /// </summary>
        private void LoadFromOptions()
        {
            loadingFromOptions = true;
            try
            {
                LoadFilterPipelineFromOptions();
                RefreshSteps();
                OnPropertyChanged(string.Empty);
                NotifySelectedFilterProperties();
            }
            finally
            {
                loadingFromOptions = false;
            }
        }

        /// <summary>
        /// 按保存的 FilterOrderCsv 和启用开关重建已添加滤波器列表。
        /// </summary>
        private void LoadFilterPipelineFromOptions()
        {
            FilterPipeline.Clear();
            List<string> addedIds = new List<string>();
            List<string> orderedIds = ParseFilterOrder(Options.FilterOrderCsv);
            for (int index = 0; index < orderedIds.Count; index++)
                AddFilterFromOptions(orderedIds[index], addedIds);

            List<PointFilterToolDescriptor> descriptors = PointFilterPipelineCore.GetToolDescriptors();
            for (int index = 0; index < descriptors.Count; index++)
                AddFilterFromOptions(descriptors[index].Id, addedIds);

            SelectedFilter = FilterPipeline.Count > 0 ? FilterPipeline[0] : null;
            RefreshAvailableFilters();
        }

        /// <summary>
        /// 如果指定滤波器在 Options 中启用且尚未添加，则追加到 Pipeline。
        /// </summary>
        private void AddFilterFromOptions(string id, List<string> addedIds)
        {
            if (!IsFilterEnabled(Options, id) || ContainsId(addedIds, id)) return;
            PointFilterToolDescriptor descriptor = FindFilterDescriptor(id);
            if (descriptor == null) return;
            FilterPipeline.Add(CreateFilterItem(descriptor));
            addedIds.Add(descriptor.Id);
        }

        /// <summary>
        /// 从可添加列表把当前选中滤波器加入 Pipeline。
        /// </summary>
        private void AddSelectedFilter()
        {
            if (SelectedAvailableFilter == null) return;
            if (PipelineContains(SelectedAvailableFilter.Id)) return;
            FilterItemViewModel newItem = CreateFilterItem(SelectedAvailableFilter.Id, SelectedAvailableFilter.DisplayName);
            FilterPipeline.Add(newItem);
            SelectedFilter = newItem;
            CommitToOptions();
            RefreshAvailableFilters();
        }

        /// <summary>
        /// 从 Pipeline 删除当前选中滤波器，并自动选择相邻项。
        /// </summary>
        private void DeleteSelectedFilter()
        {
            if (SelectedFilter == null) return;
            int oldIndex = FilterPipeline.IndexOf(SelectedFilter);
            if (oldIndex < 0) return;
            FilterPipeline.RemoveAt(oldIndex);
            SelectedFilter = FilterPipeline.Count == 0 ? null : FilterPipeline[Math.Min(oldIndex, FilterPipeline.Count - 1)];
            CommitToOptions();
            RefreshAvailableFilters();
        }

        /// <summary>
        /// 上移或下移当前选中滤波器，direction=-1 表示上移，direction=1 表示下移。
        /// </summary>
        private void MoveSelectedFilter(int direction)
        {
            if (SelectedFilter == null) return;
            int oldIndex = FilterPipeline.IndexOf(SelectedFilter);
            int newIndex = oldIndex + direction;
            if (oldIndex < 0 || newIndex < 0 || newIndex >= FilterPipeline.Count) return;
            FilterPipeline.Move(oldIndex, newIndex);
            SelectedFilter = FilterPipeline[newIndex];
            CommitToOptions();
            RefreshAvailableFilters();
        }

        /// <summary>
        /// 根据已添加 Pipeline 刷新可添加列表，避免同一滤波器重复添加。
        /// </summary>
        private void RefreshAvailableFilters()
        {
            string oldSelectedId = SelectedAvailableFilter == null ? string.Empty : SelectedAvailableFilter.Id;
            AvailableFilters.Clear();
            List<PointFilterToolDescriptor> descriptors = PointFilterPipelineCore.GetToolDescriptors();
            for (int index = 0; index < descriptors.Count; index++)
            {
                PointFilterToolDescriptor descriptor = descriptors[index];
                if (PipelineContains(descriptor.Id)) continue;
                AvailableFilters.Add(CreateFilterItem(descriptor));
            }
            SelectedAvailableFilter = AvailableFilters.FirstOrDefault(item => string.Equals(item.Id, oldSelectedId, StringComparison.OrdinalIgnoreCase));
            if (SelectedAvailableFilter == null && AvailableFilters.Count > 0) SelectedAvailableFilter = AvailableFilters[0];
        }

        /// <summary>
        /// 根据 Options.SelectedStepIndex 在步骤列表中恢复选中项。
        /// </summary>
        private void SelectStepFromOptions()
        {
            int selectedStepIndex = options == null ? -1 : options.SelectedStepIndex;
            FilterStepItemViewModel target = FilterStepItems.FirstOrDefault(item => item.StepIndex == selectedStepIndex);
            SelectedFilterStep = target ?? FilterStepItems.FirstOrDefault();
        }

        /// <summary>
        /// 判断 Pipeline 中是否已存在指定滤波器。
        /// </summary>
        private bool PipelineContains(string id)
        {
            return FilterPipeline.Any(item => string.Equals(item.Id, id, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 判断当前选中的滤波器是否为指定 ID，用于 DataTemplate 或显隐绑定。
        /// </summary>
        private bool IsSelectedFilter(string id)
        {
            return SelectedFilter != null && string.Equals(SelectedFilter.Id, id, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 刷新当前选中滤波器相关的标题、显隐和参数字段绑定。
        /// </summary>
        private void NotifySelectedFilterProperties()
        {
            RefreshSelectedParameterFields();
            OnPropertyChanged(nameof(SelectedFilterParamTitle));
            OnPropertyChanged(nameof(IsMeanSelected));
            OnPropertyChanged(nameof(IsGaussianSelected));
            OnPropertyChanged(nameof(IsMedianSelected));
            OnPropertyChanged(nameof(IsBilateralSelected));
            OnPropertyChanged(nameof(IsGuidedSelected));
            OnPropertyChanged(nameof(IsCurvatureFlowSelected));
            OnPropertyChanged(nameof(IsSobelSelected));
            OnPropertyChanged(nameof(IsLaplacianSelected));
            OnPropertyChanged(nameof(IsGaborSelected));
            OnPropertyChanged(nameof(HasSelectedFilter));
            OnPropertyChanged(nameof(HasNoSelectedFilter));
        }

        /// <summary>
        /// 根据滤波器描述动态生成参数字段集合，界面通过 ItemsControl 展示。
        /// </summary>
        private void RefreshSelectedParameterFields()
        {
            SelectedFilterParameterFields.Clear();
            if (SelectedFilter == null || SelectedFilter.ParameterFields == null) return;
            for (int index = 0; index < SelectedFilter.ParameterFields.Count; index++)
            {
                PointFilterParameterDescriptor field = SelectedFilter.ParameterFields[index];
                if (field == null || string.IsNullOrWhiteSpace(field.BindingPath)) continue;
                SelectedFilterParameterFields.Add(new FilterParameterFieldViewModel(this, field));
            }
        }

        /// <summary>
        /// 确保滤波参数对象存在。
        /// </summary>
        private void EnsureOptions()
        {
            if (options == null) options = new PointFilterOptionsParam();
        }

        /// <summary>
        /// 确保滤波输出对象存在。
        /// </summary>
        private void EnsureOutput()
        {
            if (output == null) output = new PointFilterOutputParam();
        }

        /// <summary>
        /// 安全触发重绘通知。
        /// </summary>
        private void RaiseDisplayInvalidated()
        {
            Action handler = DisplayInvalidated;
            if (handler != null) handler();
        }


        /// <summary>
        /// 根据滤波器描述创建列表项 ViewModel。
        /// </summary>
        private static FilterItemViewModel CreateFilterItem(PointFilterToolDescriptor descriptor)
        {
            return descriptor == null ? null : CreateFilterItem(descriptor.Id, descriptor.DisplayName);
        }

        /// <summary>
        /// 根据滤波器 ID 创建具体子类型，供 WPF DataTemplate 按类型选择独立参数界面。
        /// </summary>
        private static FilterItemViewModel CreateFilterItem(string id, string displayName)
        {
            PointFilterToolDescriptor descriptor = FindFilterDescriptor(id);
            IReadOnlyList<PointFilterParameterDescriptor> parameterFields = descriptor == null
                ? new List<PointFilterParameterDescriptor>()
                : descriptor.Parameters;
            if (string.Equals(id, "Mean", StringComparison.OrdinalIgnoreCase)) return new MeanFilterItemViewModel(id, displayName, parameterFields);
            if (string.Equals(id, "Gaussian", StringComparison.OrdinalIgnoreCase)) return new GaussianFilterItemViewModel(id, displayName, parameterFields);
            if (string.Equals(id, "Median", StringComparison.OrdinalIgnoreCase)) return new MedianFilterItemViewModel(id, displayName, parameterFields);
            if (string.Equals(id, "Bilateral", StringComparison.OrdinalIgnoreCase)) return new BilateralFilterItemViewModel(id, displayName, parameterFields);
            if (string.Equals(id, "Guided", StringComparison.OrdinalIgnoreCase)) return new GuidedFilterItemViewModel(id, displayName, parameterFields);
            if (string.Equals(id, "CurvatureFlow", StringComparison.OrdinalIgnoreCase)) return new CurvatureFlowFilterItemViewModel(id, displayName, parameterFields);
            if (string.Equals(id, "Sobel", StringComparison.OrdinalIgnoreCase)) return new SobelFilterItemViewModel(id, displayName, parameterFields);
            if (string.Equals(id, "Laplacian", StringComparison.OrdinalIgnoreCase)) return new LaplacianFilterItemViewModel(id, displayName, parameterFields);
            if (string.Equals(id, "Gabor", StringComparison.OrdinalIgnoreCase)) return new GaborFilterItemViewModel(id, displayName, parameterFields);
            return new FilterItemViewModel(id, displayName, parameterFields);
        }
        /// <summary>
        /// 解析保存的滤波顺序 CSV，自动去重并忽略空值。
        /// </summary>
        private static List<string> ParseFilterOrder(string csv)
        {
            List<string> ids = new List<string>();
            if (string.IsNullOrWhiteSpace(csv)) return ids;
            string[] parts = csv.Split(new[] { ',', ';', '，', '；' }, StringSplitOptions.RemoveEmptyEntries);
            for (int index = 0; index < parts.Length; index++)
            {
                string id = parts[index].Trim();
                if (!string.IsNullOrEmpty(id) && !ContainsId(ids, id)) ids.Add(id);
            }
            return ids;
        }

        /// <summary>
        /// 忽略大小写判断 ID 列表是否包含指定 ID。
        /// </summary>
        private static bool ContainsId(List<string> ids, string id)
        {
            for (int index = 0; index < ids.Count; index++)
                if (string.Equals(ids[index], id, StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        }

        /// <summary>
        /// 从 Pipeline 注册表中查找指定滤波器描述。
        /// </summary>
        private static PointFilterToolDescriptor FindFilterDescriptor(string id)
        {
            List<PointFilterToolDescriptor> descriptors = PointFilterPipelineCore.GetToolDescriptors();
            for (int index = 0; index < descriptors.Count; index++)
                if (string.Equals(descriptors[index].Id, id, StringComparison.OrdinalIgnoreCase)) return descriptors[index];
            return null;
        }

        /// <summary>
        /// 根据滤波器 ID 读取 Options 中对应的启用开关。
        /// </summary>
        private static bool IsFilterEnabled(PointFilterOptionsParam filterOptions, string id)
        {
            if (string.Equals(id, "Mean", StringComparison.OrdinalIgnoreCase)) return filterOptions.Mean;
            if (string.Equals(id, "Gaussian", StringComparison.OrdinalIgnoreCase)) return filterOptions.Gaussian;
            if (string.Equals(id, "Median", StringComparison.OrdinalIgnoreCase)) return filterOptions.Median;
            if (string.Equals(id, "Bilateral", StringComparison.OrdinalIgnoreCase)) return filterOptions.Bilateral;
            if (string.Equals(id, "Guided", StringComparison.OrdinalIgnoreCase)) return filterOptions.Guided;
            if (string.Equals(id, "CurvatureFlow", StringComparison.OrdinalIgnoreCase)) return filterOptions.CurvatureFlow;
            if (string.Equals(id, "Sobel", StringComparison.OrdinalIgnoreCase)) return filterOptions.Sobel;
            if (string.Equals(id, "Laplacian", StringComparison.OrdinalIgnoreCase)) return filterOptions.Laplacian;
            if (string.Equals(id, "Gabor", StringComparison.OrdinalIgnoreCase)) return filterOptions.Gabor;
            return false;
        }

        /// <summary>
        /// 批量设置所有滤波器启用开关，用于 Commit 前清空旧状态。
        /// </summary>
        private static void SetAllFilterEnabled(PointFilterOptionsParam filterOptions, bool enabled)
        {
            filterOptions.Mean = enabled;
            filterOptions.Gaussian = enabled;
            filterOptions.Median = enabled;
            filterOptions.Bilateral = enabled;
            filterOptions.Guided = enabled;
            filterOptions.CurvatureFlow = enabled;
            filterOptions.Sobel = enabled;
            filterOptions.Laplacian = enabled;
            filterOptions.Gabor = enabled;
        }

        /// <summary>
        /// 根据滤波器 ID 写入 Options 中对应的启用开关。
        /// </summary>
        private static void SetFilterEnabled(PointFilterOptionsParam filterOptions, string id, bool enabled)
        {
            if (string.Equals(id, "Mean", StringComparison.OrdinalIgnoreCase)) filterOptions.Mean = enabled;
            else if (string.Equals(id, "Gaussian", StringComparison.OrdinalIgnoreCase)) filterOptions.Gaussian = enabled;
            else if (string.Equals(id, "Median", StringComparison.OrdinalIgnoreCase)) filterOptions.Median = enabled;
            else if (string.Equals(id, "Bilateral", StringComparison.OrdinalIgnoreCase)) filterOptions.Bilateral = enabled;
            else if (string.Equals(id, "Guided", StringComparison.OrdinalIgnoreCase)) filterOptions.Guided = enabled;
            else if (string.Equals(id, "CurvatureFlow", StringComparison.OrdinalIgnoreCase)) filterOptions.CurvatureFlow = enabled;
            else if (string.Equals(id, "Sobel", StringComparison.OrdinalIgnoreCase)) filterOptions.Sobel = enabled;
            else if (string.Equals(id, "Laplacian", StringComparison.OrdinalIgnoreCase)) filterOptions.Laplacian = enabled;
            else if (string.Equals(id, "Gabor", StringComparison.OrdinalIgnoreCase)) filterOptions.Gabor = enabled;
        }

        /// <summary>
        /// 规范化预排序模式，非法值回退为 None。
        /// </summary>
        private static string NormalizeSortMode(string mode)
        {
            if (string.Equals(mode, "Row", StringComparison.OrdinalIgnoreCase)) return "Row";
            if (string.Equals(mode, "Col", StringComparison.OrdinalIgnoreCase)) return "Col";
            return "None";
        }
    }

    /// <summary>
    /// 滤波器列表项基类，保存 ID、显示名和该滤波器的参数字段描述。
    /// </summary>
    public class FilterItemViewModel
    {
        public string Id { get; private set; }
        public string DisplayName { get; private set; }
        public IReadOnlyList<PointFilterParameterDescriptor> ParameterFields { get; private set; }

        /// <summary>
        /// 创建滤波器列表项。
        /// </summary>
        public FilterItemViewModel(string id, string displayName, IReadOnlyList<PointFilterParameterDescriptor> parameterFields)
        {
            Id = id;
            DisplayName = displayName;
            ParameterFields = parameterFields ?? new List<PointFilterParameterDescriptor>();
        }
    }

    /// <summary>
    /// 均值滤波列表项类型，供 WPF 模板按类型绑定。
    /// </summary>
    public sealed class MeanFilterItemViewModel : FilterItemViewModel { public MeanFilterItemViewModel(string id, string displayName, IReadOnlyList<PointFilterParameterDescriptor> parameterFields) : base(id, displayName, parameterFields) { } }
    /// <summary>
    /// 高斯滤波列表项类型，供 WPF 模板按类型绑定。
    /// </summary>
    public sealed class GaussianFilterItemViewModel : FilterItemViewModel { public GaussianFilterItemViewModel(string id, string displayName, IReadOnlyList<PointFilterParameterDescriptor> parameterFields) : base(id, displayName, parameterFields) { } }
    /// <summary>
    /// 中值滤波列表项类型，供 WPF 模板按类型绑定。
    /// </summary>
    public sealed class MedianFilterItemViewModel : FilterItemViewModel { public MedianFilterItemViewModel(string id, string displayName, IReadOnlyList<PointFilterParameterDescriptor> parameterFields) : base(id, displayName, parameterFields) { } }
    /// <summary>
    /// 双边滤波列表项类型，供 WPF 模板按类型绑定。
    /// </summary>
    public sealed class BilateralFilterItemViewModel : FilterItemViewModel { public BilateralFilterItemViewModel(string id, string displayName, IReadOnlyList<PointFilterParameterDescriptor> parameterFields) : base(id, displayName, parameterFields) { } }
    /// <summary>
    /// 引导滤波列表项类型，供 WPF 模板按类型绑定。
    /// </summary>
    public sealed class GuidedFilterItemViewModel : FilterItemViewModel { public GuidedFilterItemViewModel(string id, string displayName, IReadOnlyList<PointFilterParameterDescriptor> parameterFields) : base(id, displayName, parameterFields) { } }
    /// <summary>
    /// 曲率流滤波列表项类型，供 WPF 模板按类型绑定。
    /// </summary>
    public sealed class CurvatureFlowFilterItemViewModel : FilterItemViewModel { public CurvatureFlowFilterItemViewModel(string id, string displayName, IReadOnlyList<PointFilterParameterDescriptor> parameterFields) : base(id, displayName, parameterFields) { } }
    /// <summary>
    /// Sobel 增强列表项类型，供 WPF 模板按类型绑定。
    /// </summary>
    public sealed class SobelFilterItemViewModel : FilterItemViewModel { public SobelFilterItemViewModel(string id, string displayName, IReadOnlyList<PointFilterParameterDescriptor> parameterFields) : base(id, displayName, parameterFields) { } }
    /// <summary>
    /// Laplacian 增强列表项类型，供 WPF 模板按类型绑定。
    /// </summary>
    public sealed class LaplacianFilterItemViewModel : FilterItemViewModel { public LaplacianFilterItemViewModel(string id, string displayName, IReadOnlyList<PointFilterParameterDescriptor> parameterFields) : base(id, displayName, parameterFields) { } }
    /// <summary>
    /// Gabor-like 滤波列表项类型，供 WPF 模板按类型绑定。
    /// </summary>
    public sealed class GaborFilterItemViewModel : FilterItemViewModel { public GaborFilterItemViewModel(string id, string displayName, IReadOnlyList<PointFilterParameterDescriptor> parameterFields) : base(id, displayName, parameterFields) { } }

    /// <summary>
    /// 滤波输出步骤下拉框列表项，StepIndex=-1 表示自动最后一步。
    /// </summary>
    public sealed class FilterStepItemViewModel
    {
        public int StepIndex { get; private set; }
        public string DisplayName { get; private set; }

        /// <summary>
        /// 创建步骤项。
        /// </summary>
        public FilterStepItemViewModel(int stepIndex, string displayName)
        {
            StepIndex = stepIndex;
            DisplayName = displayName;
        }
    }

    /// <summary>
    /// 单个滤波参数输入项 ViewModel。
    /// 通过 BindingPath 反射读写 PointFilterEditorViewModel 上的对应属性，使通用参数面板不依赖具体滤波器。
    /// </summary>
    public sealed class FilterParameterFieldViewModel : BindableBase
    {
        private readonly PointFilterEditorViewModel owner;
        private readonly PointFilterParameterDescriptor descriptor;

        /// <summary>
        /// 创建参数字段 ViewModel。
        /// </summary>
        public FilterParameterFieldViewModel(PointFilterEditorViewModel owner, PointFilterParameterDescriptor descriptor)
        {
            this.owner = owner;
            this.descriptor = descriptor;
        }

        public string Label { get { return descriptor == null ? string.Empty : descriptor.Label; } }
        public string BindingPath { get { return descriptor == null ? string.Empty : descriptor.BindingPath; } }
        public int DecimalPlaces { get { return GetPropertyType() == typeof(int) ? 0 : 3; } }
        public double Increment { get { return GetPropertyType() == typeof(int) ? 1.0 : 0.1; } }

        public double NumericValue
        {
            get
            {
                object value = GetRawValue();
                if (value == null) return 0.0;
                return Convert.ToDouble(value, CultureInfo.InvariantCulture);
            }
            set
            {
                Type propertyType = GetPropertyType();
                SetRawValue(propertyType == typeof(int) ? (object)(int)Math.Round(value) : value);
                OnPropertyChanged(nameof(NumericValue));
                OnPropertyChanged(nameof(Value));
            }
        }

        public string Value
        {
            get
            {
                object value = GetRawValue();
                IFormattable formattable = value as IFormattable;
                return formattable == null
                    ? Convert.ToString(value, CultureInfo.InvariantCulture)
                    : formattable.ToString(null, CultureInfo.InvariantCulture);
            }
            set
            {
                SetRawValue(ConvertValue(value, GetPropertyType()));
                OnPropertyChanged(nameof(Value));
                OnPropertyChanged(nameof(NumericValue));
            }
        }

        /// <summary>
        /// 获取 BindingPath 指向属性的类型；找不到时按 double 处理。
        /// </summary>
        private Type GetPropertyType()
        {
            System.ComponentModel.PropertyDescriptor property = GetProperty();
            return property == null ? typeof(double) : property.PropertyType;
        }

        /// <summary>
        /// 通过反射读取宿主 ViewModel 上的原始属性值。
        /// </summary>
        private object GetRawValue()
        {
            System.ComponentModel.PropertyDescriptor property = GetProperty();
            return property == null ? null : property.GetValue(owner);
        }

        /// <summary>
        /// 通过反射写入宿主 ViewModel 上的原始属性值。
        /// </summary>
        private void SetRawValue(object value)
        {
            System.ComponentModel.PropertyDescriptor property = GetProperty();
            if (property == null || property.IsReadOnly) return;
            property.SetValue(owner, value);
        }

        /// <summary>
        /// 按 BindingPath 查找宿主 ViewModel 属性描述。
        /// </summary>
        private System.ComponentModel.PropertyDescriptor GetProperty()
        {
            if (owner == null || string.IsNullOrWhiteSpace(BindingPath)) return null;
            return System.ComponentModel.TypeDescriptor.GetProperties(owner)[BindingPath];
        }

        /// <summary>
        /// 将文本转换为目标属性类型，转换失败时使用对应类型默认值。
        /// </summary>
        private static object ConvertValue(string text, Type targetType)
        {
            if (targetType == typeof(string)) return text ?? string.Empty;
            if (targetType == typeof(int))
            {
                int value;
                return int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value) ? value : 0;
            }
            if (targetType == typeof(double))
            {
                double value;
                return double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value) ? value : 0.0;
            }
            if (targetType == typeof(bool))
            {
                bool value;
                return bool.TryParse(text, out value) && value;
            }
            return Convert.ChangeType(text, targetType, CultureInfo.InvariantCulture);
        }
    }
}



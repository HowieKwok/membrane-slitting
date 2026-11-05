using CommunityToolkit.Mvvm.Input;
using Frame_Core.pluginParams;
using FrameCommunity.公共类;
using HalconDotNet;
using Plugin.BlobAnalysis.Core;
using Plugin.BlobAnalysis.Models;
using Plugin.BlobAnalysis.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;



namespace Plugin.BlobAnalysis.ViewModels
{
    /// <summary>
    /// Blob 分析界面 ViewModel。
    /// 重要约定：界面层的唯一参数源是 Frame_Core.pluginParams.ParamsBlobAnalysis。
    /// BlobAnalysisParams 只作为算法层运行时 DTO 使用，不负责 XML 保存/读取。
    /// 图像统一使用 frameParams.ho_Image，界面层不释放该输入图像，避免释放上游流程仍在使用的 HObject。
    /// </summary>
    public partial class BlobAnalysisViewModel : BindableBase
    {
        private readonly IBlobAnalysisEngine engine;
        private ParamsBlobAnalysis frameParams;
        private BlobAnalysisParams parameters;
        private BlobAnalysisResult lastResult;
        private RoiParamViewModel selectedMaskRoi;
        private BlobRegionFeature selectedRegion;
        private FilterRuleParamViewModel selectedFilter;
        private string statusText = "等待加载图像";
        private string imageInfo = "图像大小: -";
        private string mouseInfo = "Row:- Col:- Gray:-";
        private string runSummary = "检测成功: -    Blob数量: -    耗时: -";
        private double lastRightRow;
        private double lastRightCol;
        private bool hasRightPoint;

        public BlobAnalysisViewModel() : this(new BlobAnalysisEngine()) { }

        public BlobAnalysisViewModel(IBlobAnalysisEngine engine)
        {
            this.engine = engine;
            parameters = new BlobAnalysisParams();
            AddMaskRoiCommand = new RelayCommand(AddMaskRoi);
            DeleteMaskRoiCommand = new RelayCommand(DeleteSelectedMaskRoi, () => SelectedMaskRoi != null);
            SetBaseFromMouseCommand = new RelayCommand(SetBaseFromMouse, () => hasRightPoint);
            SetCurrentFromMouseCommand = new RelayCommand(SetCurrentFromMouse, () => hasRightPoint);
            AddFilterCommand = new RelayCommand(()=> AddFilterRequested?.Invoke());
            DeleteFilterCommand = new RelayCommand(DeleteSelectedFilter, () => SelectedFilter != null);
            ReloadCollections();
        }



        public ParamsBlobAnalysis FrameParams { get { return frameParams; } }

        /// <summary>
        /// 当前图像统一从 ParamsBlobAnalysis.ho_Image 取得。
        /// 不在 ViewModel 内维护第二份图像引用，也不在界面层 Dispose 输入图像。
        /// </summary>
        public HObject Image { get { return frameParams == null ? null : frameParams.ho_Image; } }

        public BlobAnalysisParams Parameters { get { return parameters; } }
        public ObservableCollection<RoiParamViewModel> MaskRois { get; } = new ObservableCollection<RoiParamViewModel>();
        public ObservableCollection<FilterRuleParamViewModel> Filters { get; } = new ObservableCollection<FilterRuleParamViewModel>();
        public ObservableCollection<BlobRegionFeature> Results { get; } = new ObservableCollection<BlobRegionFeature>();

        public IReadOnlyList<KeyValuePair<DetectRangeMode, string>> DetectModeItems { get; } = new List<KeyValuePair<DetectRangeMode, string>> { new KeyValuePair<DetectRangeMode, string>(DetectRangeMode.Global, "全局检测"), new KeyValuePair<DetectRangeMode, string>(DetectRangeMode.LocalRoi, "局部ROI检测") };
        public IReadOnlyList<KeyValuePair<RoiEditRole, string>> RoiRoleItems { get; } = new List<KeyValuePair<RoiEditRole, string>> { new KeyValuePair<RoiEditRole, string>(RoiEditRole.Local, "局部检测ROI"), new KeyValuePair<RoiEditRole, string>(RoiEditRole.Mask, "屏蔽ROI") };
        public IReadOnlyList<KeyValuePair<MorphMode, string>> MorphModeItems { get; } = new List<KeyValuePair<MorphMode, string>> { new KeyValuePair<MorphMode, string>(MorphMode.None, "不处理"), new KeyValuePair<MorphMode, string>(MorphMode.Open, "开运算"), new KeyValuePair<MorphMode, string>(MorphMode.Close, "闭运算"), new KeyValuePair<MorphMode, string>(MorphMode.FillUp, "填充孔洞"), new KeyValuePair<MorphMode, string>(MorphMode.OpenClose, "先开后闭"), new KeyValuePair<MorphMode, string>(MorphMode.CloseOpen, "先闭后开") };
        public IReadOnlyList<KeyValuePair<SortFeature, string>> SortFeatureItems { get; } = new List<KeyValuePair<SortFeature, string>> { new KeyValuePair<SortFeature, string>(SortFeature.Area, "面积"), new KeyValuePair<SortFeature, string>(SortFeature.Row, "中心Row"), new KeyValuePair<SortFeature, string>(SortFeature.Column, "中心Col") };
        public IReadOnlyList<KeyValuePair<SortDirection, string>> SortDirectionItems { get; } = new List<KeyValuePair<SortDirection, string>> { new KeyValuePair<SortDirection, string>(SortDirection.Asc, "升序"), new KeyValuePair<SortDirection, string>(SortDirection.Desc, "降序") };
        public IReadOnlyList<KeyValuePair<string, string>> FilterFeatureItems { get; } = new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("area", "面积 area"), new KeyValuePair<string, string>("row", "质心Row row"), new KeyValuePair<string, string>("column", "质心Col column"), new KeyValuePair<string, string>("width", "宽度 width"), new KeyValuePair<string, string>("height", "高度 height"), new KeyValuePair<string, string>("circularity", "圆度 circularity"), new KeyValuePair<string, string>("rectangularity", "矩形度 rectangularity"), new KeyValuePair<string, string>("convexity", "凸性 convexity"), new KeyValuePair<string, string>("compactness", "紧凑度 compactness"), new KeyValuePair<string, string>("holes_num", "孔洞数 holes_num"), new KeyValuePair<string, string>("orientation", "方向 orientation"), new KeyValuePair<string, string>("ra", "长轴 ra"), new KeyValuePair<string, string>("rb", "短轴 rb"), new KeyValuePair<string, string>("anisometry", "长宽比 anisometry") };
        public IReadOnlyList<KeyValuePair<string, string>> FilterOperationItems { get; } = new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("and", "并且 and"), new KeyValuePair<string, string>("or", "或者 or") };

        public RelayCommand AddMaskRoiCommand { get; set; }
        public RelayCommand DeleteMaskRoiCommand { get; set; }
        public RelayCommand AddFilterCommand { get; set; }
        public RelayCommand DeleteFilterCommand { get; set; }
        public RelayCommand SetBaseFromMouseCommand { get; set; }
        public RelayCommand SetCurrentFromMouseCommand { get; set; }

        public RoiParamViewModel SelectedMaskRoi { get { return selectedMaskRoi; } set { SetProperty(ref selectedMaskRoi, value); DeleteMaskRoiCommand.NotifyCanExecuteChanged(); } }
        public BlobRegionFeature SelectedRegion { get { return selectedRegion; } set { SetProperty(ref selectedRegion, value); } }
        public FilterRuleParamViewModel SelectedFilter { get { return selectedFilter; } set { SetProperty(ref selectedFilter, value); DeleteFilterCommand.NotifyCanExecuteChanged(); } }
        public string StatusText { get { return statusText; } set { SetProperty(ref statusText, value); } }
        public string ImageInfo { get { return imageInfo; } set { SetProperty(ref imageInfo, value); } }
        public string MouseInfo { get { return mouseInfo; } set { SetProperty(ref mouseInfo, value); } }
        public string RunSummary { get { return runSummary; } set { SetProperty(ref runSummary, value); } }
        public BlobAnalysisResult LastResult { get { return lastResult; } }
        public Action AddFilterRequested { get; set; }

        public DetectRangeMode DetectMode { get { return parameters.DetectMode; } set { parameters.DetectMode = value; OnPropertyChanged(); } }
        public RoiEditRole EditRole { get { return parameters.EditRole; } set { parameters.EditRole = value; OnPropertyChanged(); } }

        public bool m_bIsEnable { get { return parameters.m_bIsEnable; } set { parameters.m_bIsEnable = value; OnPropertyChanged(); } }
        public bool EnableLocalRoi { get { return parameters.EnableLocalRoi; } set { parameters.EnableLocalRoi = value; OnPropertyChanged(); } }
        public bool EnableMaskRoi { get { return parameters.EnableMaskRoi; } set { parameters.EnableMaskRoi = value; OnPropertyChanged(); } }
        public bool ShowViewRoi { get { return parameters.ShowViewRoi; } set { parameters.ShowViewRoi = value; OnPropertyChanged(); } }
        public bool EnablePositionFollow { get { return parameters.EnablePositionFollow; } set { parameters.EnablePositionFollow = value; OnPropertyChanged(); } }
        public double ThresholdMin { get { return parameters.ThresholdMin; } set { parameters.ThresholdMin = value; OnPropertyChanged(); } }
        public double ThresholdMax { get { return parameters.ThresholdMax; } set { parameters.ThresholdMax = value; OnPropertyChanged(); } }
        public MorphMode MorphMode { get { return parameters.MorphMode; } set { parameters.MorphMode = value; OnPropertyChanged(); } }
        public double MorphRadius { get { return parameters.MorphRadius; } set { parameters.MorphRadius = value; OnPropertyChanged(); } }
        public SortFeature SortFeature { get { return parameters.SortFeature; } set { parameters.SortFeature = value; OnPropertyChanged(); } }
        public SortDirection SortDirection { get { return parameters.SortDirection; } set { parameters.SortDirection = value; OnPropertyChanged(); } }
        public double BaseRow { get { return parameters.BaseRow; } set { parameters.BaseRow = value; OnPropertyChanged(); } }
        public double BaseCol { get { return parameters.BaseCol; } set { parameters.BaseCol = value; OnPropertyChanged(); } }
        public double BaseAngle { get { return parameters.BaseAngle; } set { parameters.BaseAngle = value; OnPropertyChanged(); } }
        public double CurrentRow { get { return parameters.CurrentRow; } set { parameters.CurrentRow = value; OnPropertyChanged(); } }
        public double CurrentCol { get { return parameters.CurrentCol; } set { parameters.CurrentCol = value; OnPropertyChanged(); } }
        public double CurrentAngle { get { return parameters.CurrentAngle; } set { parameters.CurrentAngle = value; OnPropertyChanged(); } }
        public double LocalRow1 { get { return parameters.LocalRoi.Row1; } set { parameters.LocalRoi.Row1 = value; OnPropertyChanged(); } }
        public double LocalCol1 { get { return parameters.LocalRoi.Col1; } set { parameters.LocalRoi.Col1 = value; OnPropertyChanged(); } }
        public double LocalRow2 { get { return parameters.LocalRoi.Row2; } set { parameters.LocalRoi.Row2 = value; OnPropertyChanged(); } }
        public double LocalCol2 { get { return parameters.LocalRoi.Col2; } set { parameters.LocalRoi.Col2 = value; OnPropertyChanged(); } }

        public double TopOffsetRow 
        { 
            get 
            { 
                return parameters.LocalRoi.TopOffsetRow;
            } 
            set 
            {
                parameters.LocalRoi.TopOffsetRow = value;
                OnPropertyChanged();
            } 
        }
        public double BottomOffsetRow { get { return parameters.LocalRoi.BottomOffsetRow; } set { parameters.LocalRoi.BottomOffsetRow = value; OnPropertyChanged(); } }
        public double LeftOffsetCol { get { return parameters.LocalRoi.LeftOffsetCol; } set { parameters.LocalRoi.LeftOffsetCol = value; OnPropertyChanged(); } }
        public double RightOffsetCol { get { return parameters.LocalRoi.RightOffsetCol; } set { parameters.LocalRoi.RightOffsetCol = value; OnPropertyChanged(); } }

        /// <summary>
        /// 绑定流程参数类。界面显示和保存都以 ParamsBlobAnalysis 为准。
        /// </summary>
        public void BindFrameParams(ParamsBlobAnalysis source)
        {
            frameParams = source;
            parameters = BlobParamsMapper.ToRuntime(frameParams);
            UpdateImageInfo("流程输入图像");
            ReloadCollections();
            OnPropertyChanged(string.Empty);
           
        }

        public void clearMethod()
        {
            try
            {
                DetectModeItems.ToList().Clear();
                RoiRoleItems.ToList().Clear();
                MorphModeItems.ToList().Clear();
                SortFeatureItems.ToList().Clear();
                SortDirectionItems.ToList().Clear();
                FilterFeatureItems.ToList().Clear();
                FilterOperationItems.ToList().Clear();
                MaskRois.Clear();
                Filters.Clear();
                Results.Clear();
                AddMaskRoiCommand = null;
                DeleteMaskRoiCommand = null;
                AddFilterCommand = null; 
                SetBaseFromMouseCommand = null;
                 SetCurrentFromMouseCommand = null;



                 }
     
            catch (Exception)
            {

                //throw;
            }
        }

        /// <summary>
        /// 将界面修改回写到 ParamsBlobAnalysis。保存和运行前必须调用。
        /// </summary>
        public void CommitToFrameParams()
        {
            if (frameParams == null) return;
            BlobParamsMapper.ApplyToFrame(parameters, frameParams);
        }

        /// <summary>
        /// 加载图像到 ParamsBlobAnalysis.ho_Image。
        /// 注意：这里不释放旧 ho_Image，旧图像生命周期由流程框架统一管理。
        /// </summary>
        public void LoadImageToFrameParams(string path)
        {
            if (frameParams == null) return;
            HOperatorSet.ReadImage(out HObject image, path);
            frameParams.ho_Image?.Dispose();
            frameParams.ho_Image = image;
            UpdateImageInfo(Path.GetFileName(path));
            OnPropertyChanged(nameof(Image));
        }

        public void RunLocalOnly()
        {
            lastResult?.Dispose();
            CommitToFrameParams();
            lastResult = engine.Run(Image, parameters);
            Results.Clear();
            foreach (BlobRegionFeature item in lastResult.Regions) Results.Add(item);
            SelectedRegion = Results.Count > 0 ? Results[0] : null;
            RunSummary = string.Format(CultureInfo.InvariantCulture, "检测成功: {0}    Blob数量: {1}    耗时: {2} ms", lastResult.Success, Results.Count, lastResult.ElapsedMs);
            StatusText = lastResult.Success ? "Blob 分析完成" : "Blob 分析失败: " + lastResult.Message;
            OnPropertyChanged(nameof(LastResult));
        }

        public void LoadResultsFromParams(ParamsBlobAnalysis source)
        {
            Results.Clear();
            if (source != null && source.m_lOutRegionResults != null)
            {
                foreach (BlobRegionResultParam item in source.m_lOutRegionResults)
                {
                    Results.Add(new BlobRegionFeature
                    {
                        Index = item.m_nIndex,
                        Area = item.m_fArea,
                        Row = item.m_fCenterRow,
                        Col = item.m_fCenterCol,
                        Rect2Row = item.m_fRect2Row,
                        Rect2Col = item.m_fRect2Col,
                        Ra = item.m_fRa,
                        Rb = item.m_fRb,
                        Ratio = item.m_fAspectRatio,
                        Phi = item.m_fPhi,
                        AngleDeg = item.m_fAngleDeg,
                        Circularity = item.m_fCircularity,
                        Rectangularity = item.m_fRectangularity,
                        MeanGray = item.m_fMeanGray,
                        DeviationGray = item.m_fDeviationGray,
                        MinGray = item.m_fMinGray,
                        MaxGray = item.m_fMaxGray,
                        Length1 = item.m_fLength1,
                        Length2 = item.m_fLength2
                    });
                }
            }
            SelectedRegion = Results.Count > 0 ? Results[0] : null;
            RunSummary = source == null ? "检测成功: -    Blob数量: -    耗时: -" : string.Format(CultureInfo.InvariantCulture, "检测成功: {0}    Blob数量: {1}    耗时: {2:0.###} ms", source.m_bRunSuccess, source.m_nRegionCount, source.m_fElapsedMs);
            StatusText = source == null ? "未执行" : source.m_strRunMessage;
            OnPropertyChanged(nameof(LastResult));
        }

        public void UpdateMouse(double row, double col, string gray)
        {
            MouseInfo = string.Format(CultureInfo.InvariantCulture, "Row:{0:0} Col:{1:0} Gray:{2}", row, col, gray);
        }

        public void SetLastRightPoint(double row, double col)
        {
            lastRightRow = row;
            lastRightCol = col;
            hasRightPoint = true;
            StatusText = string.Format(CultureInfo.InvariantCulture, "右键点 Row:{0:0.0} Col:{1:0.0}", row, col);
            SetBaseFromMouseCommand.NotifyCanExecuteChanged();
            SetCurrentFromMouseCommand.NotifyCanExecuteChanged();
        }

        /// <summary>
        /// 释放界面私有资源。这里只释放算法临时结果，不释放 frameParams.ho_Image。
        /// </summary>
        public void ReleaseViewResources()
        {
            lastResult?.Dispose();
            lastResult = null;
        }

        private void SetBaseFromMouse()
        {
            BaseRow = lastRightRow;
            BaseCol = lastRightCol;
        }

        private void SetCurrentFromMouse()
        {
            CurrentRow = lastRightRow;
            CurrentCol = lastRightCol;
        }

        private void AddMaskRoi()
        {
            int index = MaskRois.Count + 1;
            RoiParam roi = new RoiParam { Index = index, Enabled = true, Name = "屏蔽ROI-" + index, Shape = RoiShapeType.Rectangle1, Row1 = LocalRow1, Col1 = LocalCol1, Row2 = LocalRow1 + 200, Col2 = LocalCol1 + 300, CenterRow = LocalRow1 + 100, CenterCol = LocalCol1 + 150, Length1 = 150, Length2 = 100, Radius = 80 };
            parameters.MaskRois.Add(roi);
            RoiParamViewModel item = new RoiParamViewModel(roi);
            MaskRois.Add(item);
            SelectedMaskRoi = item;
            EditRole = RoiEditRole.Mask;
        }

        private void DeleteSelectedMaskRoi()
        {
            if (SelectedMaskRoi == null) return;
            parameters.MaskRois.Remove(SelectedMaskRoi.Model);
            MaskRois.Remove(SelectedMaskRoi);
            for (int i = 0; i < MaskRois.Count; i++)
            {
                MaskRois[i].Index = i + 1;
                MaskRois[i].Name = "屏蔽ROI-" + (i + 1).ToString(CultureInfo.InvariantCulture);
            }
            SelectedMaskRoi = MaskRois.Count > 0 ? MaskRois[0] : null;
        }

        public void AddFilter(FilterRuleParam rule)
        {
            if (rule == null) return;
            parameters.Filters.Add(rule);
            FilterRuleParamViewModel item = new FilterRuleParamViewModel(rule);
            Filters.Add(item);
            SelectedFilter = item;
        }

        private void DeleteSelectedFilter()
        {
            if (SelectedFilter == null) return;
            parameters.Filters.Remove(SelectedFilter.Model);
            Filters.Remove(SelectedFilter);
            SelectedFilter = Filters.Count > 0 ? Filters[0] : null;
        }

        private void ReloadCollections()
        {
            MaskRois.Clear();
            if (parameters.MaskRois != null)
            {
                foreach (RoiParam roi in parameters.MaskRois) MaskRois.Add(new RoiParamViewModel(roi));
            }
            Filters.Clear();
            if (parameters.Filters != null)
            {
                foreach (FilterRuleParam filter in parameters.Filters) Filters.Add(new FilterRuleParamViewModel(filter));
            }
            SelectedMaskRoi = MaskRois.Count > 0 ? MaskRois[0] : null;
            SelectedFilter = Filters.Count > 0 ? Filters[0] : null;
        }

        private void UpdateImageInfo(string name)
        {
            if (Image != null && Image.IsInitialized())
            {
                HOperatorSet.GetImageSize(Image, out HTuple width, out HTuple height);
                ImageInfo = string.Format(CultureInfo.InvariantCulture, "图像大小: {0} x {1}", width.I, height.I);
                StatusText = "图像已绑定: " + name;
            }
            else
            {
                ImageInfo = "图像大小: -";
            }
        }
    }
}
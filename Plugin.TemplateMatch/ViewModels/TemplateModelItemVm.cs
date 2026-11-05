using Frame_Core.pluginParams;
using HalconDotNet;
using System;

namespace Plugin.TemplateMatch.ViewModels
{
    public sealed class TemplateModelItemVm : BindableBase
    {
        private bool isEnabled;
        private bool isCreated;
        private string name;
        private TemplateMatchMode matchMode;
        private TemplateCreateOptions createOptions;
        private TemplateFindOptions findOptions;
        private double roiRow1;
        private double roiCol1;
        private double roiRow2;
        private double roiCol2;
        private int roiShapeType;
        private double baseRow;
        private double baseCol;
        private double baseAngleDeg;
        private double lastScore;
        private string modelFileName;

        public TemplateModelItemVm()
        {
            isEnabled = true;
            name = "Template1";
            matchMode = TemplateMatchMode.Shape;
            createOptions = new TemplateCreateOptions();
            findOptions = new TemplateFindOptions();
            roiRow1 = 0;
            roiCol1 = 0;
            roiRow2 = 100;
            roiCol2 = 100;
            roiShapeType = ViewROI.ROI.ROI_TYPE_RECTANCLE1;
            baseRow = 0;
            baseCol = 0;
            baseAngleDeg = 0;
            modelFileName = string.Empty;
        }

        public TemplateModelItemVm(TemplateModelParam source) : this()
        {
            if (source == null) return;
            RuntimeParam = source;
            isEnabled = source.IsEnabled;
            isCreated = source.IsCreated;
            name = source.Name;
            matchMode = source.MatchMode;
            createOptions = CloneCreateOptions(source.CreateOptions);
            findOptions = CloneFindOptions(source.FindOptions);
            roiRow1 = source.RoiRow1;
            roiCol1 = source.RoiCol1;
            roiRow2 = source.RoiRow2;
            roiCol2 = source.RoiCol2;
            roiShapeType = source.RoiShapeType;
            baseRow = source.BaseRow;
            baseCol = source.BaseCol;
            baseAngleDeg = source.BaseAngleDeg;
            lastScore = source.LastScore;
            modelFileName = source.ModelFileName;
            ModelId = source.ModelId;
            CreateContours = source.CreateContours;
        }

        public bool IsEnabled { get { return isEnabled; } set { SetProperty(ref isEnabled, value); } }
        public bool IsCreated { get { return isCreated; } set { SetProperty(ref isCreated, value); } }
        public string Name { get { return name; } set { SetProperty(ref name, string.IsNullOrWhiteSpace(value) ? "Template1" : value); } }
        public TemplateMatchMode MatchMode
        {
            get { return matchMode; }
            set
            {
                if (SetProperty(ref matchMode, value) && CreateOptions != null)
                    CreateOptions.MatchMode = value;
            }
        }
        public TemplateCreateOptions CreateOptions { get { return createOptions; } set { SetProperty(ref createOptions, value ?? new TemplateCreateOptions()); } }
        public TemplateFindOptions FindOptions { get { return findOptions; } set { SetProperty(ref findOptions, value ?? new TemplateFindOptions()); } }
        public double RoiRow1 { get { return roiRow1; } set { SetProperty(ref roiRow1, value); } }
        public double RoiCol1 { get { return roiCol1; } set { SetProperty(ref roiCol1, value); } }
        public double RoiRow2 { get { return roiRow2; } set { SetProperty(ref roiRow2, value); } }
        public double RoiCol2 { get { return roiCol2; } set { SetProperty(ref roiCol2, value); } }
        public int RoiShapeType { get { return roiShapeType; } set { SetProperty(ref roiShapeType, value); } }
        public double BaseRow { get { return baseRow; } set { SetProperty(ref baseRow, value); } }
        public double BaseCol { get { return baseCol; } set { SetProperty(ref baseCol, value); } }
        public double BaseAngleDeg { get { return baseAngleDeg; } set { SetProperty(ref baseAngleDeg, value); } }
        public double LastScore { get { return lastScore; } set { SetProperty(ref lastScore, value); } }
        public string ModelFileName { get { return modelFileName; } set { SetProperty(ref modelFileName, value ?? string.Empty); } }

        internal TemplateModelParam RuntimeParam { get; private set; }
        public HTuple ModelId { get; set; }
        public HObject CreateContours { get; set; }

        public TemplateModelParam ToParam()
        {
            return CopyTo(RuntimeParam ?? new TemplateModelParam());
        }

        public TemplateModelParam ToNewParam()
        {
            return CopyToNewParam();
        }

        public TemplateModelParam CopyTo(TemplateModelParam target)
        {
            if (target == null) target = new TemplateModelParam();
            target.IsEnabled = IsEnabled;
            target.IsCreated = IsCreated;
            target.Name = Name;
            target.MatchMode = MatchMode;
            target.CreateOptions = CloneCreateOptions(CreateOptions);
            target.CreateOptions.MatchMode = MatchMode;
            target.FindOptions = CloneFindOptions(FindOptions);
            target.RoiRow1 = RoiRow1;
            target.RoiCol1 = RoiCol1;
            target.RoiRow2 = RoiRow2;
            target.RoiCol2 = RoiCol2;
            target.RoiShapeType = RoiShapeType;
            target.BaseRow = BaseRow;
            target.BaseCol = BaseCol;
            target.BaseAngleDeg = BaseAngleDeg;
            target.LastScore = LastScore;
            target.ModelFileName = ModelFileName;
            target.ModelId = ModelId;
            target.CreateContours = CreateContours;
            RuntimeParam = target;
            return target;
        }

        public TemplateModelParam CopyToNewParam()
        {
            TemplateModelParam target = new TemplateModelParam();
            target.IsEnabled = IsEnabled;
            target.IsCreated = IsCreated;
            target.Name = Name;
            target.MatchMode = MatchMode;
            target.CreateOptions = CloneCreateOptions(CreateOptions);
            target.CreateOptions.MatchMode = MatchMode;
            target.FindOptions = CloneFindOptions(FindOptions);
            target.RoiRow1 = RoiRow1;
            target.RoiCol1 = RoiCol1;
            target.RoiRow2 = RoiRow2;
            target.RoiCol2 = RoiCol2;
            target.RoiShapeType = RoiShapeType;
            target.BaseRow = BaseRow;
            target.BaseCol = BaseCol;
            target.BaseAngleDeg = BaseAngleDeg;
            target.LastScore = LastScore;
            target.ModelFileName = ModelFileName;
            target.ModelId = ModelId;
            target.CreateContours = CreateContours;
            return target;
        }

        public void CopyFrom(TemplateModelParam source)
        {
            if (source == null) return;
            RuntimeParam = source;
            IsEnabled = source.IsEnabled;
            IsCreated = source.IsCreated;
            Name = source.Name;
            MatchMode = source.MatchMode;
            CreateOptions = CloneCreateOptions(source.CreateOptions);
            CreateOptions.MatchMode = source.MatchMode;
            FindOptions = CloneFindOptions(source.FindOptions);
            RoiRow1 = source.RoiRow1;
            RoiCol1 = source.RoiCol1;
            RoiRow2 = source.RoiRow2;
            RoiCol2 = source.RoiCol2;
            RoiShapeType = source.RoiShapeType;
            BaseRow = source.BaseRow;
            BaseCol = source.BaseCol;
            BaseAngleDeg = source.BaseAngleDeg;
            LastScore = source.LastScore;
            ModelFileName = source.ModelFileName;
            ModelId = source.ModelId;
            CreateContours = source.CreateContours;
        }

        public void SetRoi(double row1, double col1, double row2, double col2)
        {
            RoiRow1 = Math.Min(row1, row2);
            RoiCol1 = Math.Min(col1, col2);
            RoiRow2 = Math.Max(row1, row2);
            RoiCol2 = Math.Max(col1, col2);
        }

        private static TemplateCreateOptions CloneCreateOptions(TemplateCreateOptions source)
        {
            source = source ?? new TemplateCreateOptions();
            return new TemplateCreateOptions
            {
                MatchMode = source.MatchMode,
                ModelCreatingMethod = source.ModelCreatingMethod,
                NumLevels = source.NumLevels,
                AngleStartDeg = source.AngleStartDeg,
                AngleExtentDeg = source.AngleExtentDeg,
                AngleStepDeg = source.AngleStepDeg,
                Optimization = source.Optimization,
                Metric = source.Metric,
                Contrast = source.Contrast,
                MinContrast = source.MinContrast
            };
        }

        private static TemplateFindOptions CloneFindOptions(TemplateFindOptions source)
        {
            source = source ?? new TemplateFindOptions();
            return new TemplateFindOptions
            {
                AngleStartDeg = source.AngleStartDeg,
                AngleExtentDeg = source.AngleExtentDeg,
                MinScore = source.MinScore,
                NumMatches = source.NumMatches,
                MaxOverlap = source.MaxOverlap,
                SubPixel = source.SubPixel,
                NumLevels = source.NumLevels,
                Greediness = source.Greediness,
                TimeoutMs = source.TimeoutMs
            };
        }
    }
}

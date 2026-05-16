using System;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Frame_Core.pluginParams;
using HalconDotNet;

namespace Plugin.FindIntersection.ViewModels
{
    public interface IToolActions
    {
        void LoadImage();
        void ToggleCrosshair();
        void FitImage();
        void KeepRoi();
        void Run();
        void SetBasePose();
        void SetCurrentPose();
    }

    public sealed class ToolVm : BindableBase
    {
        private readonly IToolActions actions;
        private string statusText = "等待加载图片";
        private string mouseInfo = "图像：--  行：--  列：--  灰度：--";
        private string line1Status = "抓边1拟合：--";
        private string line2Status = "抓边2拟合：--";
        private string line1Info = "抓边1：未检测到";
        private string line2Info = "抓边2：未检测到";
        private string parallelText = "是否平行：--";
        private string intersectionText = "交点：R=--  C=--";
        private string followOffsetText = "跟随偏差：X=--  Y=--  R=--";
        private double intersectionRow;
        private double intersectionColumn;
        private double angle;

        public ToolVm(IToolActions actions)
        {
            this.actions = actions ?? throw new ArgumentNullException(nameof(actions));
            Params.ParameterChanged += delegate { RoiParameterChanged?.Invoke(this, EventArgs.Empty); };

            LoadImageCommand = new RelayCommand(actions.LoadImage);
            ToggleCrosshairCommand = new RelayCommand(actions.ToggleCrosshair);
            FitWindowCommand = new RelayCommand(actions.FitImage);
            KeepRoiCommand = new RelayCommand(actions.KeepRoi);
            RunCommand = new RelayCommand(actions.Run);
            SetBasePoseCommand = new RelayCommand(actions.SetBasePose);
            SetCurrentPoseCommand = new RelayCommand(actions.SetCurrentPose);
        }

        public event EventHandler RoiParameterChanged;

        public ToolParams Params { get; } = new ToolParams();
        public ICommand LoadImageCommand { get; }
        public ICommand ToggleCrosshairCommand { get; }
        public ICommand FitWindowCommand { get; }
        public ICommand KeepRoiCommand { get; }
        public ICommand RunCommand { get; }
        public ICommand SetBasePoseCommand { get; }
        public ICommand SetCurrentPoseCommand { get; }

        public double IntersectionRow { get { return intersectionRow; } set { SetProperty(ref intersectionRow, value); } }
        public double IntersectionColumn { get { return intersectionColumn; } set { SetProperty(ref intersectionColumn, value); } }
        public double Angle { get { return angle; } set { SetProperty(ref angle, value); } }
        public string StatusText { get { return statusText; } set { SetProperty(ref statusText, value); } }
        public string MouseInfo { get { return mouseInfo; } set { SetProperty(ref mouseInfo, value); } }
        public string Line1Status { get { return line1Status; } set { SetProperty(ref line1Status, value); } }
        public string Line2Status { get { return line2Status; } set { SetProperty(ref line2Status, value); } }
        public string Line1Info { get { return line1Info; } set { SetProperty(ref line1Info, value); } }
        public string Line2Info { get { return line2Info; } set { SetProperty(ref line2Info, value); } }
        public string ParallelText { get { return parallelText; } set { SetProperty(ref parallelText, value); } }
        public string IntersectionText { get { return intersectionText; } set { SetProperty(ref intersectionText, value); } }
        public string FollowOffsetText { get { return followOffsetText; } set { SetProperty(ref followOffsetText, value); } }

        public void BindFrameParams(ParamsFindIntersection parameters)
        {
            Params.Bind(parameters ?? new ParamsFindIntersection());
        }

        public void CommitToFrameParams()
        {
            Params.Apply();
        }

        public void LoadResultsFromParams(ParamsFindIntersection source)
        {
            if (source == null)
            {
                ResetResult();
                return;
            }

            FollowOffsetText = string.Format("跟随偏差：X={0:F3}  Y={1:F3}  R={2:F3}°", source.m_fFollowOffsetX, source.m_fFollowOffsetY, source.m_fFollowOffsetR);
            Line1Status = source.bl_Line1_FitSuccess ? "抓边1拟合：成功" : "抓边1拟合：失败";
            Line2Status = source.bl_Line2_FitSuccess ? "抓边2拟合：成功" : "抓边2拟合：失败";
            Line1Info = FormatLine("抓边1", source.bl_Line1_FitSuccess, source.hv_Line1_RowBegin, source.hv_Line1_ColumnBegin, source.hv_Line1_RowEnd, source.hv_Line1_ColumnEnd);
            Line2Info = FormatLine("抓边2", source.bl_Line2_FitSuccess, source.hv_Line2_RowBegin, source.hv_Line2_ColumnBegin, source.hv_Line2_RowEnd, source.hv_Line2_ColumnEnd);

            if (source.bl_IntersectionSuccess &&
                source.hv_IntersectionRow != null &&
                source.hv_IntersectionColumn != null &&
                source.hv_IsParallel != null &&
                source.hv_IsParallel.I == 0)
            {
                IntersectionRow = source.hv_IntersectionRow.D;
                IntersectionColumn = source.hv_IntersectionColumn.D;
                Angle = source.hv_IntersectionAngle == null ? 0 : source.hv_IntersectionAngle.D;
                IntersectionText = string.Format("交点：R={0:F2}  C={1:F2}", IntersectionRow, IntersectionColumn);
                ParallelText = "是否平行：否";
                return;
            }

            IntersectionText = "交点：R=--  C=--";
            ParallelText = source.hv_IsParallel != null && source.hv_IsParallel.I != 0 ? "是否平行：是" : "是否平行：--";
        }

        public void ResetResult()
        {
            Line1Status = "抓边1拟合：--";
            Line2Status = "抓边2拟合：--";
            Line1Info = "抓边1：未检测到";
            Line2Info = "抓边2：未检测到";
            ParallelText = "是否平行：--";
            IntersectionText = "交点：R=--  C=--";
            IntersectionRow = 0;
            IntersectionColumn = 0;
            Angle = 0;
            FollowOffsetText = "跟随偏差：X=--  Y=--  R=--";
        }

        private static string FormatLine(string name, bool success, HTuple rowBegin, HTuple columnBegin, HTuple rowEnd, HTuple columnEnd)
        {
            if (!success || rowBegin == null || columnBegin == null || rowEnd == null || columnEnd == null)
            {
                return name + "：未检测到";
            }

            return string.Format("{0}：({1:F1},{2:F1}) -> ({3:F1},{4:F1})", name, rowBegin.D, columnBegin.D, rowEnd.D, columnEnd.D);
        }
    }

    public sealed class ToolParams : BindableBase
    {
        private ParamsFindIntersection frameParams;
        private bool syncing;
        private double line1CenterRow;
        private double line1CenterColumn;
        private double line1Angle;
        private double line1Height;
        private double line1Width;
        private double line1PointCount;
        private double line1Sigma;
        private double line1Threshold;
        private double line1NumInstances;
        private double line1DistThreshold;
        private double line1MinScore;
        private double line1MaxIterations;
        private string line1Transition;
        private string line1Select;
        private double line2CenterRow;
        private double line2CenterColumn;
        private double line2Angle;
        private double line2Height;
        private double line2Width;
        private double line2PointCount;
        private double line2Sigma;
        private double line2Threshold;
        private double line2NumInstances;
        private double line2DistThreshold;
        private double line2MinScore;
        private double line2MaxIterations;
        private string line2Transition;
        private string line2Select;
        private bool enablePositionFollow;
        private double baseRow;
        private double baseColumn;
        private double baseAngle;
        private double currentRow;
        private double currentColumn;
        private double currentAngle;
        private bool currentPoseValid;

        public event EventHandler ParameterChanged;

        public void Bind(ParamsFindIntersection source)
        {
            frameParams = source ?? new ParamsFindIntersection();
            syncing = true;
            try
            {
                Line1CenterRow = frameParams.hv_Line1_RoiMidRow;
                Line1CenterColumn = frameParams.hv_Line1_RoiMidColumn;
                Line1Angle = frameParams.hv_Line1_RoiAngle;
                Line1Height = frameParams.hv_Line1_RoiHeight;
                Line1Width = frameParams.hv_Line1_RoiWidth;
                Line1PointCount = frameParams.hv_Line1_RoiPointsNum;
                Line1Sigma = frameParams.hv_Line1_MeasureSigma;
                Line1Threshold = frameParams.hv_Line1_MeasureThreshold;
                Line1NumInstances = frameParams.hv_Line1_MeasureNumInstances;
                Line1DistThreshold = frameParams.hv_Line1_MeasureDistThreshold;
                Line1MinScore = frameParams.hv_Line1_MeasureMinScore;
                Line1MaxIterations = frameParams.hv_Line1_MaxNumIterations;
                Line1Transition = frameParams.hv_Line1_MeasureTransition;
                Line1Select = frameParams.hv_Line1_MeasureSelect;

                Line2CenterRow = frameParams.hv_Line2_RoiMidRow;
                Line2CenterColumn = frameParams.hv_Line2_RoiMidColumn;
                Line2Angle = frameParams.hv_Line2_RoiAngle;
                Line2Height = frameParams.hv_Line2_RoiHeight;
                Line2Width = frameParams.hv_Line2_RoiWidth;
                Line2PointCount = frameParams.hv_Line2_RoiPointsNum;
                Line2Sigma = frameParams.hv_Line2_MeasureSigma;
                Line2Threshold = frameParams.hv_Line2_MeasureThreshold;
                Line2NumInstances = frameParams.hv_Line2_MeasureNumInstances;
                Line2DistThreshold = frameParams.hv_Line2_MeasureDistThreshold;
                Line2MinScore = frameParams.hv_Line2_MeasureMinScore;
                Line2MaxIterations = frameParams.hv_Line2_MaxNumIterations;
                Line2Transition = frameParams.hv_Line2_MeasureTransition;
                Line2Select = frameParams.hv_Line2_MeasureSelect;

                EnablePositionFollow = frameParams.m_bUsePositionFollow;
                BaseRow = frameParams.m_fBaseRow;
                BaseColumn = frameParams.m_fBaseCol;
                BaseAngle = frameParams.m_fBaseAngle;
                CurrentRow = frameParams.m_fCurrentRow;
                CurrentColumn = frameParams.m_fCurrentCol;
                CurrentAngle = frameParams.m_fCurrentAngle;
                CurrentPoseValid = frameParams.m_bCurrentPoseValid;
            }
            finally
            {
                syncing = false;
            }
        }

        public void Apply()
        {
            if (frameParams == null) return;

            frameParams.hv_Line1_RoiMidRow = Line1CenterRow;
            frameParams.hv_Line1_RoiMidColumn = Line1CenterColumn;
            frameParams.hv_Line1_RoiAngle = Line1Angle;
            frameParams.hv_Line1_RoiHeight = Line1Height;
            frameParams.hv_Line1_RoiWidth = Line1Width;
            frameParams.hv_Line1_RoiPointsNum = ToInt(Line1PointCount);
            frameParams.hv_Line1_MeasureSigma = Line1Sigma;
            frameParams.hv_Line1_MeasureThreshold = ToInt(Line1Threshold);
            frameParams.hv_Line1_MeasureNumInstances = ToInt(Line1NumInstances);
            frameParams.hv_Line1_MeasureDistThreshold = Line1DistThreshold;
            frameParams.hv_Line1_MeasureMinScore = Line1MinScore;
            frameParams.hv_Line1_MaxNumIterations = ToInt(Line1MaxIterations);
            frameParams.hv_Line1_MeasureTransition = Line1Transition;
            frameParams.hv_Line1_MeasureSelect = Line1Select;

            frameParams.hv_Line2_RoiMidRow = Line2CenterRow;
            frameParams.hv_Line2_RoiMidColumn = Line2CenterColumn;
            frameParams.hv_Line2_RoiAngle = Line2Angle;
            frameParams.hv_Line2_RoiHeight = Line2Height;
            frameParams.hv_Line2_RoiWidth = Line2Width;
            frameParams.hv_Line2_RoiPointsNum = ToInt(Line2PointCount);
            frameParams.hv_Line2_MeasureSigma = Line2Sigma;
            frameParams.hv_Line2_MeasureThreshold = ToInt(Line2Threshold);
            frameParams.hv_Line2_MeasureNumInstances = ToInt(Line2NumInstances);
            frameParams.hv_Line2_MeasureDistThreshold = Line2DistThreshold;
            frameParams.hv_Line2_MeasureMinScore = Line2MinScore;
            frameParams.hv_Line2_MaxNumIterations = ToInt(Line2MaxIterations);
            frameParams.hv_Line2_MeasureTransition = Line2Transition;
            frameParams.hv_Line2_MeasureSelect = Line2Select;

            frameParams.m_bUsePositionFollow = EnablePositionFollow;
            frameParams.m_fBaseRow = BaseRow;
            frameParams.m_fBaseCol = BaseColumn;
            frameParams.m_fBaseAngle = BaseAngle;
            frameParams.m_fCurrentRow = CurrentRow;
            frameParams.m_fCurrentCol = CurrentColumn;
            frameParams.m_fCurrentAngle = CurrentAngle;
            frameParams.m_bCurrentPoseValid = CurrentPoseValid;
        }

        public void UpdateFromRoi(int index, double row, double column, double angleDegree, double height, double width, int pointCount)
        {
            syncing = true;
            try
            {
                if (index == 1)
                {
                    Line1CenterRow = row;
                    Line1CenterColumn = column;
                    Line1Angle = angleDegree;
                    Line1Height = height;
                    Line1Width = width;
                    Line1PointCount = pointCount;
                }
                else
                {
                    Line2CenterRow = row;
                    Line2CenterColumn = column;
                    Line2Angle = angleDegree;
                    Line2Height = height;
                    Line2Width = width;
                    Line2PointCount = pointCount;
                }
            }
            finally
            {
                syncing = false;
            }
        }

        public double Line1CenterRow { get { return line1CenterRow; } set { SetValue(ref line1CenterRow, value); } }
        public double Line1CenterColumn { get { return line1CenterColumn; } set { SetValue(ref line1CenterColumn, value); } }
        public double Line1Angle { get { return line1Angle; } set { SetValue(ref line1Angle, value); } }
        public double Line1Height { get { return line1Height; } set { SetValue(ref line1Height, value); } }
        public double Line1Width { get { return line1Width; } set { SetValue(ref line1Width, value); } }
        public double Line1PointCount { get { return line1PointCount; } set { SetValue(ref line1PointCount, value); } }
        public double Line1Sigma { get { return line1Sigma; } set { SetValue(ref line1Sigma, value); } }
        public double Line1Threshold { get { return line1Threshold; } set { SetValue(ref line1Threshold, value); } }
        public double Line1NumInstances { get { return line1NumInstances; } set { SetValue(ref line1NumInstances, value); } }
        public double Line1DistThreshold { get { return line1DistThreshold; } set { SetValue(ref line1DistThreshold, value); } }
        public double Line1MinScore { get { return line1MinScore; } set { SetValue(ref line1MinScore, value); } }
        public double Line1MaxIterations { get { return line1MaxIterations; } set { SetValue(ref line1MaxIterations, value); } }
        public string Line1Transition { get { return line1Transition; } set { SetValue(ref line1Transition, value); } }
        public string Line1Select { get { return line1Select; } set { SetValue(ref line1Select, value); } }

        public double Line2CenterRow { get { return line2CenterRow; } set { SetValue(ref line2CenterRow, value); } }
        public double Line2CenterColumn { get { return line2CenterColumn; } set { SetValue(ref line2CenterColumn, value); } }
        public double Line2Angle { get { return line2Angle; } set { SetValue(ref line2Angle, value); } }
        public double Line2Height { get { return line2Height; } set { SetValue(ref line2Height, value); } }
        public double Line2Width { get { return line2Width; } set { SetValue(ref line2Width, value); } }
        public double Line2PointCount { get { return line2PointCount; } set { SetValue(ref line2PointCount, value); } }
        public double Line2Sigma { get { return line2Sigma; } set { SetValue(ref line2Sigma, value); } }
        public double Line2Threshold { get { return line2Threshold; } set { SetValue(ref line2Threshold, value); } }
        public double Line2NumInstances { get { return line2NumInstances; } set { SetValue(ref line2NumInstances, value); } }
        public double Line2DistThreshold { get { return line2DistThreshold; } set { SetValue(ref line2DistThreshold, value); } }
        public double Line2MinScore { get { return line2MinScore; } set { SetValue(ref line2MinScore, value); } }
        public double Line2MaxIterations { get { return line2MaxIterations; } set { SetValue(ref line2MaxIterations, value); } }
        public string Line2Transition { get { return line2Transition; } set { SetValue(ref line2Transition, value); } }
        public string Line2Select { get { return line2Select; } set { SetValue(ref line2Select, value); } }

        public bool EnablePositionFollow { get { return enablePositionFollow; } set { SetValue(ref enablePositionFollow, value); } }
        public double BaseRow { get { return baseRow; } set { SetValue(ref baseRow, value); } }
        public double BaseColumn { get { return baseColumn; } set { SetValue(ref baseColumn, value); } }
        public double BaseAngle { get { return baseAngle; } set { SetValue(ref baseAngle, value); } }
        public double CurrentRow { get { return currentRow; } set { SetValue(ref currentRow, value); } }
        public double CurrentColumn { get { return currentColumn; } set { SetValue(ref currentColumn, value); } }
        public double CurrentAngle { get { return currentAngle; } set { SetValue(ref currentAngle, value); } }
        public bool CurrentPoseValid { get { return currentPoseValid; } set { SetValue(ref currentPoseValid, value); } }

        private void SetValue<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (!SetProperty(ref storage, value, propertyName)) return;
            if (!syncing) ParameterChanged?.Invoke(this, EventArgs.Empty);
        }

        private static int ToInt(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value)) return 0;
            return (int)Math.Round(value, MidpointRounding.AwayFromZero);
        }
    }
}

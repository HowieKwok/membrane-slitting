using System;
using System.Runtime.CompilerServices;
using Frame_Core;
using Plugin.MeasureLine.ViewModels;

namespace Plugin.MeasureLine.Models
{
    public sealed class ModelParams : BindableBase
    {
        private ParamsMeasureLine frameParams;
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
        private string measureIORegions;
        private string measureInterpolation;
        private bool imageProcessEnabled;
        private double scaleMean;
        private double scaleFactor;
        private double sharpenIterations;
        private double sharpenSigma;
        private string peakValleySelect;
        private bool defaultLine;
        private string roiMoveIndex;
        private double roiStandardRow;
        private double roiStandardColumn;
        private double roiStandardOffsetRow;
        private double roiStandardOffsetColumn;
        private double roiIncreaseNum;
        private double roiIncreaseRow;
        private double roiIncreaseColumn;
        private bool enablePositionFollow;
        private double baseRow;
        private double baseColumn;
        private double baseAngle;
        private double currentRow;
        private double currentColumn;
        private double currentAngle;
        private bool currentPoseValid;

        public event EventHandler ParameterChanged;

        public void Bind(ParamsMeasureLine source)
        {
            frameParams = source ?? new ParamsMeasureLine();
            syncing = true;
            try
            {
                Line1CenterRow = frameParams.hv_RoiMidRow;
                Line1CenterColumn = frameParams.hv_RoiMidColumn;
                Line1Angle = frameParams.hv_RoiAngle;
                Line1Height = frameParams.hv_RoiHeight;
                Line1Width = frameParams.hv_RoiWidth;
                Line1PointCount = frameParams.hv_RoiPointsNum;
                Line1Sigma = frameParams.hv_MeasureSigma;
                Line1Threshold = frameParams.hv_MeasureThreshold;
                Line1NumInstances = frameParams.hv_MeasureNumInstances;
                Line1DistThreshold = frameParams.hv_MeasureDistThreshold;
                Line1MinScore = frameParams.hv_MeasureMinScore;
                Line1MaxIterations = frameParams.hv_MaxNumIterations;
                Line1Transition = frameParams.hv_MeasureTransition;
                Line1Select = frameParams.hv_MeasureSelect;
                MeasureIORegions = frameParams.hv_MeasureIORegions;
                MeasureInterpolation = frameParams.hv_MeasureInterpolation;
                ImageProcessEnabled = frameParams.bl_ImageProcessIndex;
                ScaleMean = frameParams.hv_ScaleMean;
                ScaleFactor = frameParams.hv_ScaleFactor;
                SharpenIterations = frameParams.hv_SharpenIterations;
                SharpenSigma = frameParams.hv_SharpenSigma;
                PeakValleySelect = frameParams.hv_PeakValleySelect;
                DefaultLine = frameParams.bl_DefaultLine;
                RoiMoveIndex = frameParams.str_RoiMoveIndex;
                RoiStandardRow = frameParams.hv_RoiStandardRow;
                RoiStandardColumn = frameParams.hv_RoiStandardColumn;
                RoiStandardOffsetRow = frameParams.hv_RoiStandardOffsetRow;
                RoiStandardOffsetColumn = frameParams.hv_RoiStandardOffsetColumn;
                RoiIncreaseNum = frameParams.hv_RoiIncreaseNum;
                RoiIncreaseRow = frameParams.hv_RoiIncreaseRow;
                RoiIncreaseColumn = frameParams.hv_RoiIncreaseColumn;
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

            frameParams.hv_RoiMidRow = Line1CenterRow;
            frameParams.hv_RoiMidColumn = Line1CenterColumn;
            frameParams.hv_RoiAngle = Line1Angle;
            frameParams.hv_RoiHeight = Line1Height;
            frameParams.hv_RoiWidth = Line1Width;
            frameParams.hv_RoiPointsNum = ToInt(Line1PointCount);
            frameParams.hv_MeasureSigma = Line1Sigma;
            frameParams.hv_MeasureThreshold = ToInt(Line1Threshold);
            frameParams.hv_MeasureNumInstances = ToInt(Line1NumInstances);
            frameParams.hv_MeasureDistThreshold = Line1DistThreshold;
            frameParams.hv_MeasureMinScore = Line1MinScore;
            frameParams.hv_MaxNumIterations = ToInt(Line1MaxIterations);
            frameParams.hv_MeasureTransition = Line1Transition;
            frameParams.hv_MeasureSelect = Line1Select;
            frameParams.hv_MeasureIORegions = MeasureIORegions;
            frameParams.hv_MeasureInterpolation = MeasureInterpolation;
            frameParams.bl_ImageProcessIndex = ImageProcessEnabled;
            frameParams.hv_ScaleMean = ToInt(ScaleMean);
            frameParams.hv_ScaleFactor = ScaleFactor;
            frameParams.hv_SharpenIterations = ToInt(SharpenIterations);
            frameParams.hv_SharpenSigma = SharpenSigma;
            frameParams.hv_PeakValleySelect = PeakValleySelect;
            frameParams.bl_DefaultLine = DefaultLine;
            frameParams.str_RoiMoveIndex = RoiMoveIndex;
            frameParams.hv_RoiStandardRow = RoiStandardRow;
            frameParams.hv_RoiStandardColumn = RoiStandardColumn;
            frameParams.hv_RoiStandardOffsetRow = RoiStandardOffsetRow;
            frameParams.hv_RoiStandardOffsetColumn = RoiStandardOffsetColumn;
            frameParams.hv_RoiIncreaseNum = ToInt(RoiIncreaseNum);
            frameParams.hv_RoiIncreaseRow = ToInt(RoiIncreaseRow);
            frameParams.hv_RoiIncreaseColumn = ToInt(RoiIncreaseColumn);
            frameParams.m_bUsePositionFollow = EnablePositionFollow;
            frameParams.m_fBaseRow = BaseRow;
            frameParams.m_fBaseCol = BaseColumn;
            frameParams.m_fBaseAngle = BaseAngle;
            frameParams.m_fCurrentRow = CurrentRow;
            frameParams.m_fCurrentCol = CurrentColumn;
            frameParams.m_fCurrentAngle = CurrentAngle;
            frameParams.m_bCurrentPoseValid = CurrentPoseValid;
        }

        public void UpdateFromRoi(double row, double column, double angleDegree, double height, double width, int pointCount)
        {
            syncing = true;
            try
            {
                Line1CenterRow = row;
                Line1CenterColumn = column;
                Line1Angle = angleDegree;
                Line1Height = height;
                Line1Width = width;
                Line1PointCount = pointCount;
                RoiStandardOffsetRow = row - RoiStandardRow;
                RoiStandardOffsetColumn = column - RoiStandardColumn;
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
        public string MeasureIORegions { get { return measureIORegions; } set { SetValue(ref measureIORegions, value); } }
        public string MeasureInterpolation { get { return measureInterpolation; } set { SetValue(ref measureInterpolation, value); } }
        public bool ImageProcessEnabled { get { return imageProcessEnabled; } set { SetValue(ref imageProcessEnabled, value); } }
        public double ScaleMean { get { return scaleMean; } set { SetValue(ref scaleMean, value); } }
        public double ScaleFactor { get { return scaleFactor; } set { SetValue(ref scaleFactor, value); } }
        public double SharpenIterations { get { return sharpenIterations; } set { SetValue(ref sharpenIterations, value); } }
        public double SharpenSigma { get { return sharpenSigma; } set { SetValue(ref sharpenSigma, value); } }
        public string PeakValleySelect { get { return peakValleySelect; } set { SetValue(ref peakValleySelect, value); } }
        public bool DefaultLine { get { return defaultLine; } set { SetValue(ref defaultLine, value); } }
        public string RoiMoveIndex { get { return roiMoveIndex; } set { SetValue(ref roiMoveIndex, value); } }
        public double RoiStandardRow { get { return roiStandardRow; } set { SetValue(ref roiStandardRow, value); } }
        public double RoiStandardColumn { get { return roiStandardColumn; } set { SetValue(ref roiStandardColumn, value); } }
        public double RoiStandardOffsetRow { get { return roiStandardOffsetRow; } set { SetValue(ref roiStandardOffsetRow, value); } }
        public double RoiStandardOffsetColumn { get { return roiStandardOffsetColumn; } set { SetValue(ref roiStandardOffsetColumn, value); } }
        public double RoiIncreaseNum { get { return roiIncreaseNum; } set { SetValue(ref roiIncreaseNum, value); } }
        public double RoiIncreaseRow { get { return roiIncreaseRow; } set { SetValue(ref roiIncreaseRow, value); } }
        public double RoiIncreaseColumn { get { return roiIncreaseColumn; } set { SetValue(ref roiIncreaseColumn, value); } }
        public bool EnablePositionFollow { get { return enablePositionFollow; } set { SetValue(ref enablePositionFollow, value); } }
        public double BaseRow { get { return baseRow; } set { SetValue(ref baseRow, value); } }
        public double BaseColumn { get { return baseColumn; } set { SetValue(ref baseColumn, value); } }
        public double BaseAngle { get { return baseAngle; } set { SetValue(ref baseAngle, value); } }
        public double CurrentRow { get { return currentRow; } set { SetCurrentValue(ref currentRow, value); } }
        public double CurrentColumn { get { return currentColumn; } set { SetCurrentValue(ref currentColumn, value); } }
        public double CurrentAngle { get { return currentAngle; } set { SetCurrentValue(ref currentAngle, value); } }
        public bool CurrentPoseValid { get { return currentPoseValid; } set { SetValue(ref currentPoseValid, value); } }

        private void SetValue<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (!SetProperty(ref storage, value, propertyName)) return;
            if (!syncing) ParameterChanged?.Invoke(this, EventArgs.Empty);
        }

        private void SetCurrentValue(ref double storage, double value, [CallerMemberName] string propertyName = null)
        {
            if (!SetProperty(ref storage, value, propertyName)) return;
            if (!syncing && !CurrentPoseValid)
            {
                CurrentPoseValid = true;
                return;
            }
            if (!syncing) ParameterChanged?.Invoke(this, EventArgs.Empty);
        }

        private static int ToInt(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value)) return 0;
            return (int)Math.Round(value, MidpointRounding.AwayFromZero);
        }
    }
}

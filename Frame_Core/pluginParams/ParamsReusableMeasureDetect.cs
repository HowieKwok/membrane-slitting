using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Frame_Core.pluginParams
{
    [Serializable]
    public class ParamsReusableMeasureDetect : PluginParamsBase, IDisposable
    {
        [NonSerialized]
        [XmlIgnore]
        public HObject ho_Image;

        [NonSerialized]
        [XmlIgnore]
        public HObject ho_ImageProcessed;

        public int ImageSourceIndex = 0;
        public string ImageSourceModule = "default";
        public string ImageSourceName = "default";

        public bool m_bIsEnable = true;
        public bool EnableMeasureGrab = true;

        public bool EnableRoi = true;
        public double RoiCenterRow = 700.0;
        public double RoiCenterCol = 700.0;
        public double RoiAngleDeg = -90.0;
        public double RoiHalfLength1 = 100.0;
        public double RoiHalfLength2 = 15.0;
        public int RoiPointNum = 20;

        public bool m_bUsePositionFollow = false;
        public double m_fBaseRow = 0;
        public double m_fBaseCol = 0;
        public double m_fBaseAngle = 0;
        public double m_fCurrentRow = 0;
        public double m_fCurrentCol = 0;
        public double m_fCurrentAngle = 0;
        public bool m_bCurrentPoseValid = false;
        public double m_fFollowOffsetY = 0;
        public double m_fFollowOffsetX = 0;
        public double m_fFollowOffsetR = 0;

        public double MeasureSigma = 2.0;
        public double MeasureThreshold = 10.0;
        public int MeasureNumInstances = 1;
        public double MeasureDistThreshold = 3.5;
        public string MeasureTransition = "all";
        public string MeasureSelect = "all";
        public double MeasureMinScore = 0.7;
        public int MaxNumIterations = -1;
        public string MeasureInterpolation = "bilinear";
        public string MeasureIORegions = "false";
        public double ScaleMean = 0.0;
        public bool EnablePointMedianReject = true;
        public double PointMedianRangeFactor = 2.0;

        public bool EnableJumpDetection = true;
        public bool EnableJumpDebugDisplay = true;
        public string JumpMode = "Row";
        public double RowJumpThreshold = 5.0;
        public double ColJumpThreshold = 5.0;
        public int MainMaxFitPoints = 8;
        public int SecondaryMaxFitPoints = 8;
        public int MinFitPoints = 3;
        public double MaxFitError = 25.0;
        public double MinTurnAngleDeg = 18.0;
        public double RowMainAngleMinDeg = 0.0;
        public double RowMainAngleMaxDeg = 10.0;
        public double ColMainAngleMinDeg = 70.0;
        public double ColMainAngleMaxDeg = 90.0;

        public bool EnableConvexPeakDetection = true;
        public bool EnableCornerPeakDetection = true;
        public int ConvexSmoothWindow = 3;
        public int ConvexSupportWindow = 4;
        public double ConvexMinPeakScore = 0.35;
        public double ConvexMinProminence = 0.25;
        public double ConvexMinTurnAngleDeg = 18.0;
        public double ConvexMinSlopeDelta = 0.35;
        public double ConvexOutlierRadius = 12.0;
        public int ConvexOutlierMinNeighbors = 2;
        public double ConvexMinPeakDistance = 12.0;
        public double ConvexMaxGapRatio = 2.4;
        public double ConvexMaxGapExtra = 6.0;
        public int ConvexMinChainPoints = 6;

        public PointFilterOptionsParam FilterOptions = new PointFilterOptionsParam();
        public List<MeasurePointParam> InputPoints = new List<MeasurePointParam>();

        [NonSerialized]
        [XmlIgnore]
        public List<MeasurePointParam> RuntimeDetectionPoints;

        public MeasureGrabOutputParam GrabOutput = new MeasureGrabOutputParam();
        public PointFilterOutputParam FilterOutput = new PointFilterOutputParam();
        public JumpDetectionOutputParam JumpOutput = new JumpDetectionOutputParam();
        public ConvexPeakOutputParam ConvexOutput = new ConvexPeakOutputParam();

        public ParamsReusableMeasureDetect()
        {
            HOperatorSet.GenEmptyObj(out ho_Image);
            HOperatorSet.GenEmptyObj(out ho_ImageProcessed);
            EnsureOutputs();
        }

        public void EnsureOutputs()
        {
            if (FilterOptions == null) FilterOptions = new PointFilterOptionsParam();
            if (InputPoints == null) InputPoints = new List<MeasurePointParam>();
            if (GrabOutput == null) GrabOutput = new MeasureGrabOutputParam();
            if (FilterOutput == null) FilterOutput = new PointFilterOutputParam();
            if (JumpOutput == null) JumpOutput = new JumpDetectionOutputParam();
            if (ConvexOutput == null) ConvexOutput = new ConvexPeakOutputParam();
            GrabOutput.EnsureHalconObjects();
            FilterOutput.EnsureHalconObjects();
            JumpOutput.EnsureHalconObjects();
            ConvexOutput.EnsureHalconObjects();
        }

        public void ReleaseOutputs()
        {
            EnsureOutputs();
            GrabOutput.Clear();
            FilterOutput.Clear();
            JumpOutput.Clear();
            ConvexOutput.Clear();
            RuntimeDetectionPoints = null;
        }

        public void Dispose()
        {
            if (GrabOutput != null) GrabOutput.Dispose();
            if (FilterOutput != null) FilterOutput.Dispose();
            if (JumpOutput != null) JumpOutput.Dispose();
            if (ConvexOutput != null) ConvexOutput.Dispose();
            if (ho_ImageProcessed != null) { ho_ImageProcessed.Dispose(); ho_ImageProcessed = null; }
            if (ho_Image != null) { ho_Image.Dispose(); ho_Image = null; }
        }
    }

    [Serializable]
    public sealed class MeasureGrabOutputParam : IDisposable
    {
        [NonSerialized]
        [XmlIgnore]
        public HObject ho_RawPointsCross;

        public bool Success = false;
        public string Message = string.Empty;
        public int RawPointCount = 0;
        public int RemovedPointCount = 0;
        public List<MeasurePointParam> RawPoints = new List<MeasurePointParam>();

        public void EnsureHalconObjects()
        {
            if (ho_RawPointsCross == null) HOperatorSet.GenEmptyObj(out ho_RawPointsCross);
        }

        public void Clear()
        {
            DisposeHalconObjects();
            HOperatorSet.GenEmptyObj(out ho_RawPointsCross);
            Success = false;
            Message = string.Empty;
            RawPointCount = 0;
            RemovedPointCount = 0;
            RawPoints = new List<MeasurePointParam>();
        }

        public void Dispose()
        {
            DisposeHalconObjects();
        }

        private void DisposeHalconObjects()
        {
            if (ho_RawPointsCross != null) { ho_RawPointsCross.Dispose(); ho_RawPointsCross = null; }
        }
    }

    [Serializable]
    public sealed class PointFilterOptionsParam
    {
        public bool Enable = false;
        public string PreSortMode = "None";
        public string FilterOrderCsv = "Mean,Gaussian,Median,Bilateral,Guided,CurvatureFlow,Sobel,Laplacian,Gabor";
        public int SelectedStepIndex = -1;
        public double DisplayCrossSize = 8.0;
        public bool ShowSelectedPointSet = true;

        public bool Mean = false;
        public bool Gaussian = false;
        public bool Median = false;
        public bool Bilateral = false;
        public bool Guided = false;
        public bool CurvatureFlow = false;
        public bool Sobel = false;
        public bool Laplacian = false;
        public bool Gabor = false;

        public int MeanRadius = 3;
        public int GaussianRadius = 3;
        public double GaussianSigma = 5;
        public int MedianRadius = 3;
        public int BilateralRadius = 3;
        public double BilateralSpatialSigma = 5;
        public double BilateralRangeSigma = 6;
        public int GuidedRadius = 3;
        public double GuidedEpsilon = 0.01;
        public int CurvatureFlowIterations = 6;
        public double CurvatureFlowStep = 0.22;
        public int SobelRadius = 3;
        public double SobelAmount = 0.35;
        public int LaplacianRadius = 3;
        public double LaplacianAmount = 0.25;
        public int GaborRadius = 3;
        public double GaborSigma = 5;
    }

    [Serializable]
    public sealed class PointFilterStepOutputParam
    {
        public string Name = string.Empty;
        public string ToolId = string.Empty;
        public List<MeasurePointParam> Points = new List<MeasurePointParam>();
    }

    [Serializable]
    public sealed class PointFilterOutputParam : IDisposable
    {
        [NonSerialized]
        [XmlIgnore]
        public HObject ho_SelectedPointsCross;

        public List<MeasurePointParam> RawPoints = new List<MeasurePointParam>();
        public List<PointFilterStepOutputParam> Steps = new List<PointFilterStepOutputParam>();
        public List<MeasurePointParam> SelectedPoints = new List<MeasurePointParam>();
        public bool FilterEnabled = false;
        public int SelectedStepIndex = -1;
        public string SelectedStepName = string.Empty;
        public string SelectedToolId = string.Empty;
        public int AppliedFilterCount = 0;
        public int PointCount = 0;

        public void EnsureHalconObjects()
        {
            if (ho_SelectedPointsCross == null) HOperatorSet.GenEmptyObj(out ho_SelectedPointsCross);
        }

        public void Clear()
        {
            DisposeHalconObjects();
            HOperatorSet.GenEmptyObj(out ho_SelectedPointsCross);
            RawPoints = new List<MeasurePointParam>();
            Steps = new List<PointFilterStepOutputParam>();
            SelectedPoints = new List<MeasurePointParam>();
            FilterEnabled = false;
            SelectedStepIndex = -1;
            SelectedStepName = string.Empty;
            SelectedToolId = string.Empty;
            AppliedFilterCount = 0;
            PointCount = 0;
        }

        public void Dispose()
        {
            DisposeHalconObjects();
        }

        private void DisposeHalconObjects()
        {
            if (ho_SelectedPointsCross != null) { ho_SelectedPointsCross.Dispose(); ho_SelectedPointsCross = null; }
        }
    }

    [Serializable]
    public sealed class FittedEdgeOutputParam
    {
        public List<MeasurePointParam> Points = new List<MeasurePointParam>();
        public double StartRow = 0;
        public double StartCol = 0;
        public double EndRow = 0;
        public double EndCol = 0;
        public double FitError = 0;
        public double AngleDeg = 0;

        public void Clear()
        {
            Points = new List<MeasurePointParam>();
            StartRow = 0;
            StartCol = 0;
            EndRow = 0;
            EndCol = 0;
            FitError = 0;
            AngleDeg = 0;
        }
    }

    [Serializable]
    public sealed class JumpDetectionOutputParam : IDisposable
    {
        [NonSerialized]
        [XmlIgnore]
        public HObject ho_MainEdgeXld;

        [NonSerialized]
        [XmlIgnore]
        public HObject ho_SecondaryEdgeXld;

        [NonSerialized]
        [XmlIgnore]
        public HObject ho_IntersectionCross;

        public bool Success = false;
        public string RejectReason = string.Empty;
        public string Mode = "Row";
        public FittedEdgeOutputParam MainEdge = new FittedEdgeOutputParam();
        public FittedEdgeOutputParam SecondaryEdge = new FittedEdgeOutputParam();
        public double IntersectionRow = 0;
        public double IntersectionCol = 0;
        public double TurnAngleDeg = 0;
        public double MainAngleDeg = 0;
        public double MainMean = 0;
        public double SecondaryMean = 0;

        public void EnsureHalconObjects()
        {
            if (ho_MainEdgeXld == null) HOperatorSet.GenEmptyObj(out ho_MainEdgeXld);
            if (ho_SecondaryEdgeXld == null) HOperatorSet.GenEmptyObj(out ho_SecondaryEdgeXld);
            if (ho_IntersectionCross == null) HOperatorSet.GenEmptyObj(out ho_IntersectionCross);
        }

        public void Clear()
        {
            DisposeHalconObjects();
            HOperatorSet.GenEmptyObj(out ho_MainEdgeXld);
            HOperatorSet.GenEmptyObj(out ho_SecondaryEdgeXld);
            HOperatorSet.GenEmptyObj(out ho_IntersectionCross);
            Success = false;
            RejectReason = string.Empty;
            Mode = "Row";
            MainEdge.Clear();
            SecondaryEdge.Clear();
            IntersectionRow = 0;
            IntersectionCol = 0;
            TurnAngleDeg = 0;
            MainAngleDeg = 0;
            MainMean = 0;
            SecondaryMean = 0;
        }

        public void Dispose()
        {
            DisposeHalconObjects();
        }

        private void DisposeHalconObjects()
        {
            if (ho_MainEdgeXld != null) { ho_MainEdgeXld.Dispose(); ho_MainEdgeXld = null; }
            if (ho_SecondaryEdgeXld != null) { ho_SecondaryEdgeXld.Dispose(); ho_SecondaryEdgeXld = null; }
            if (ho_IntersectionCross != null) { ho_IntersectionCross.Dispose(); ho_IntersectionCross = null; }
        }
    }

    [Serializable]
    public sealed class ConvexPeakOutputParam : IDisposable
    {
        [NonSerialized]
        [XmlIgnore]
        public HObject ho_MainEdgeXld;

        [NonSerialized]
        [XmlIgnore]
        public HObject ho_SecondaryEdgeXld;

        [NonSerialized]
        [XmlIgnore]
        public HObject ho_PeakCross;

        public bool Success = false;
        public string RejectReason = string.Empty;
        public FittedEdgeOutputParam MainEdge = new FittedEdgeOutputParam();
        public FittedEdgeOutputParam SecondaryEdge = new FittedEdgeOutputParam();
        public List<MeasurePointParam> PeakPoints = new List<MeasurePointParam>();
        public double PeakRow = 0;
        public double PeakCol = 0;
        public double Score = 0;
        public double TurnAngleDeg = 0;
        public double Prominence = 0;
        public double SlopeDelta = 0;
        public double FitError = 0;
        public string Method = string.Empty;

        public void EnsureHalconObjects()
        {
            if (ho_MainEdgeXld == null) HOperatorSet.GenEmptyObj(out ho_MainEdgeXld);
            if (ho_SecondaryEdgeXld == null) HOperatorSet.GenEmptyObj(out ho_SecondaryEdgeXld);
            if (ho_PeakCross == null) HOperatorSet.GenEmptyObj(out ho_PeakCross);
        }

        public void Clear()
        {
            DisposeHalconObjects();
            HOperatorSet.GenEmptyObj(out ho_MainEdgeXld);
            HOperatorSet.GenEmptyObj(out ho_SecondaryEdgeXld);
            HOperatorSet.GenEmptyObj(out ho_PeakCross);
            Success = false;
            RejectReason = string.Empty;
            MainEdge.Clear();
            SecondaryEdge.Clear();
            PeakPoints = new List<MeasurePointParam>();
            PeakRow = 0;
            PeakCol = 0;
            Score = 0;
            TurnAngleDeg = 0;
            Prominence = 0;
            SlopeDelta = 0;
            FitError = 0;
            Method = string.Empty;
        }

        public void Dispose()
        {
            DisposeHalconObjects();
        }

        private void DisposeHalconObjects()
        {
            if (ho_MainEdgeXld != null) { ho_MainEdgeXld.Dispose(); ho_MainEdgeXld = null; }
            if (ho_SecondaryEdgeXld != null) { ho_SecondaryEdgeXld.Dispose(); ho_SecondaryEdgeXld = null; }
            if (ho_PeakCross != null) { ho_PeakCross.Dispose(); ho_PeakCross = null; }
        }
    }

    [Serializable]
    public sealed class MeasurePointParam
    {
        public double Row { get; set; }
        public double Col { get; set; }

        public MeasurePointParam() { }
        public MeasurePointParam(double row, double col)
        {
            Row = row;
            Col = col;
        }
    }
}

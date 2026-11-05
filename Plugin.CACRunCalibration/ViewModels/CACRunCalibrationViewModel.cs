using Frame_Core.pluginParams;
using Plugin.CACRunCalibration.Models;
using Plugin.CACRunCalibration.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;

namespace Plugin.CACRunCalibration.ViewModels
{
    public sealed class CACRunCalibrationViewModel : BindableBase, IDisposable
    {
        private readonly ParamsCACRunCalibration param;
        private bool disposed;

        public CACRunCalibrationModel Model { get; private set; }

        public IReadOnlyList<string> QrPolarityItems { get; private set; }
        public IReadOnlyList<string> QrDefaultParamItems { get; private set; }
        public IReadOnlyList<string> CenterSourceItems { get; private set; }
        public IReadOnlyList<string> MappingModelItems { get; private set; }
        public ObservableCollection<CalibrationPointRow> PointRows { get { return Model.PointRows; } }

        public ICommand GenerateMatrixCommand { get; private set; }
        public ICommand CheckMatrixCommand { get; private set; }
        public ICommand TransformPointCommand { get; private set; }
        public ICommand ReadRoiCommand { get; private set; }

        public event EventHandler GenerateMatrixRequested;
        public event EventHandler CheckMatrixRequested;
        public event EventHandler TransformPointRequested;
        public event EventHandler ReadRoiRequested;

        public CACRunCalibrationViewModel(ParamsCACRunCalibration param)
        {
            if (param == null) throw new ArgumentNullException("param");
            this.param = param;
            Model = new CACRunCalibrationModel();
            QrPolarityItems = new[] { "dark_on_light", "light_on_dark", "any" };
            QrDefaultParamItems = new[] { "standard_recognition", "enhanced_recognition", "maximum_recognition" };
            CenterSourceItems = new[] { "SmallestRectangleCenter", "DataCodeCenter" };
            MappingModelItems = new[] { "Affine" };

            GenerateMatrixCommand = new RelayCommand(_ => Raise(GenerateMatrixRequested));
            CheckMatrixCommand = new RelayCommand(_ => Raise(CheckMatrixRequested));
            TransformPointCommand = new RelayCommand(_ => Raise(TransformPointRequested));
            ReadRoiCommand = new RelayCommand(_ => Raise(ReadRoiRequested));
            LoadFromParams();
        }

        public void LoadFromParams()
        {
            Model.BoardRows = param.m_nBoardRows;
            Model.BoardColumns = param.m_nBoardColumns;
            Model.SpacingX = param.m_fSpacingX;
            Model.SpacingY = param.m_fSpacingY;
            Model.OriginX = param.m_fOriginX;
            Model.OriginY = param.m_fOriginY;
            Model.QrPhysicalWidth = param.m_fQrPhysicalWidth;
            Model.QrPhysicalHeight = param.m_fQrPhysicalHeight;
            Model.SerialBaseIndex = param.m_nSerialBaseIndex;
            Model.QrPerRow = param.m_nQrPerRow;
            Model.QrColumnStep = param.m_nQrColumnStep;
            Model.QrRowStep = param.m_nQrRowStep;
            Model.QrOddRowColumnOffset = param.m_nQrOddRowColumnOffset;
            Model.InvertRowAxis = param.m_bInvertRowAxis;
            Model.InvertColumnAxis = param.m_bInvertColumnAxis;
            Model.QrPolarity = param.m_strQrPolarity;
            Model.QrDefaultParameters = param.m_strQrDefaultParameters;
            Model.CenterSource = param.m_strCenterSource;
            Model.MappingModel = param.m_strMappingModel;
            Model.StopAfterResultNum = param.m_nStopAfterResultNum;
            Model.MaxAllowedErrorMm = param.m_fMaxAllowedErrorMm;
            Model.EnableRoi = param.m_bEnableRoi;
            Model.ShowViewRoi = param.m_bShowViewRoi;
            Model.RoiRow1 = param.m_fRoiRow1;
            Model.RoiCol1 = param.m_fRoiCol1;
            Model.RoiRow2 = param.m_fRoiRow2;
            Model.RoiCol2 = param.m_fRoiCol2;
            Model.InputPixelRow = param.m_fInputPixelRow;
            Model.InputPixelCol = param.m_fInputPixelCol;
            Model.OutputBoardX = param.m_fOutputBoardX;
            Model.OutputBoardY = param.m_fOutputBoardY;
            Model.TransformSuccess = param.m_bTransformSuccess;
            Model.HasValidMatrix = param.m_bHasValidMatrix;
            Model.MatrixStateText = param.m_strMatrixStateText;
            Model.MatrixCheckMessage = param.m_strMatrixCheckMessage;
            Model.RunMessage = param.m_strRunMessage;
            Model.ImageStateText = param.ho_Image != null && param.ho_Image.IsInitialized() ? "已加载图像" : "未加载图像";
            Model.LoadedImagePath = param.ho_Image != null && param.ho_Image.IsInitialized() ? Model.LoadedImagePath : string.Empty;
            Model.LastSaveTime = param.m_strLastSaveTime;
            Model.DetectedCount = param.m_nDetectedCount;
            Model.UsedCount = param.m_nUsedCount;
            Model.AvgErrorMm = param.m_fAvgErrorMm;
            Model.RmsErrorMm = param.m_fRmsErrorMm;
            Model.MaxErrorMm = param.m_fMaxErrorMm;
            Model.ResultStateText = param.m_bHasValidMatrix ? "成功" : "未完成";
        }

        public void CommitToParams()
        {
            param.m_nBoardRows = Model.BoardRows;
            param.m_nBoardColumns = Model.BoardColumns;
            param.m_fSpacingX = Model.SpacingX;
            param.m_fSpacingY = Model.SpacingY;
            param.m_fOriginX = Model.OriginX;
            param.m_fOriginY = Model.OriginY;
            param.m_fQrPhysicalWidth = Model.QrPhysicalWidth;
            param.m_fQrPhysicalHeight = Model.QrPhysicalHeight;
            param.m_nSerialBaseIndex = Model.SerialBaseIndex;
            param.m_nQrPerRow = Model.QrPerRow;
            param.m_nQrColumnStep = Model.QrColumnStep;
            param.m_nQrRowStep = Model.QrRowStep;
            param.m_nQrOddRowColumnOffset = Model.QrOddRowColumnOffset;
            param.m_bInvertRowAxis = Model.InvertRowAxis;
            param.m_bInvertColumnAxis = Model.InvertColumnAxis;
            param.m_strQrPolarity = Model.QrPolarity;
            param.m_strQrDefaultParameters = Model.QrDefaultParameters;
            param.m_strCenterSource = Model.CenterSource;
            param.m_strMappingModel = Model.MappingModel;
            param.m_nStopAfterResultNum = Model.StopAfterResultNum;
            param.m_fMaxAllowedErrorMm = Model.MaxAllowedErrorMm;
            param.m_bEnableRoi = Model.EnableRoi;
            param.m_bShowViewRoi = Model.ShowViewRoi;
            param.m_fRoiRow1 = Model.RoiRow1;
            param.m_fRoiCol1 = Model.RoiCol1;
            param.m_fRoiRow2 = Model.RoiRow2;
            param.m_fRoiCol2 = Model.RoiCol2;
            param.m_fInputPixelRow = Model.InputPixelRow;
            param.m_fInputPixelCol = Model.InputPixelCol;
            param.m_fOutputBoardX = Model.OutputBoardX;
            param.m_fOutputBoardY = Model.OutputBoardY;
            param.m_bTransformSuccess = Model.TransformSuccess;
            param.m_bHasValidMatrix = Model.HasValidMatrix;
            param.m_strMatrixStateText = Model.MatrixStateText;
            param.m_strMatrixCheckMessage = Model.MatrixCheckMessage;
            param.m_strRunMessage = Model.RunMessage;
            param.m_strLastSaveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            Model.LastSaveTime = param.m_strLastSaveTime;
        }

        public void UpdateMatrixState(bool valid, string message)
        {
            Model.HasValidMatrix = valid;
            Model.MatrixStateText = valid ? "矩阵有效" : "无有效矩阵";
            Model.MatrixCheckMessage = message ?? string.Empty;
            Model.ResultStateText = valid ? "成功" : "失败";
        }

        public void UpdateRunMessage(bool success, string message)
        {
            Model.RunMessage = message ?? string.Empty;
            Model.ResultStateText = success ? "成功" : "失败";
        }

        public void UpdateImageState(bool loaded, string path)
        {
            Model.ImageStateText = loaded ? "已加载图像" : "未加载图像";
            Model.LoadedImagePath = path ?? string.Empty;
        }

        public void UpdatePointRows(IEnumerable<CalibrationPointRow> rows)
        {
            Model.PointRows.Clear();
            if (rows == null) return;
            foreach (CalibrationPointRow row in rows) Model.PointRows.Add(row);
        }

        public void Dispose()
        {
            if (disposed) return;
            disposed = true;
            GenerateMatrixRequested = null;
            CheckMatrixRequested = null;
            TransformPointRequested = null;
            ReadRoiRequested = null;
        }

        private void Raise(EventHandler handler)
        {
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}

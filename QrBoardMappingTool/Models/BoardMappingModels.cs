using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace QrBoardMappingTool.Models
{
    public enum BoardMappingModelKind
    {
        Affine,
        Projective
    }

    public enum QrIndexOrder
    {
        RowColumn,
        ColumnRow
    }

    public enum QrCenterSource
    {
        SmallestRectangleCenter,
        DataCodeCenter
    }

    public enum FitPixelSource
    {
        NominalGridCenter,
        DetectedQrCenter
    }

    public enum ViewerRoiKind
    {
        Rectangle,
        Circle,
        Polygon
    }

    public sealed class BoardMappingSettings : NotifyObject
    {
        private string imagePath = string.Empty;
        private int boardRows = 10;
        private int boardColumns = 10;
        private double spacingX = 10;
        private double spacingY = 10;
        private double originX = 0;
        private double originY = 0;
        private double pixelCellWidth = 100;
        private double pixelCellHeight = 100;
        private double pixelOriginColumn = 0;
        private double pixelOriginRow = 0;
        private double qrPhysicalWidth = 10;
        private double qrPhysicalHeight = 10;
        private double qrPixelWidth = 100;
        private double qrPixelHeight = 100;
        private int serialBaseIndex = 1;
        private int qrPerRow = 5;
        private int qrColumnStep = 2;
        private int qrRowStep = 1;
        private int qrOddRowColumnOffset = 1;
        private bool invertRowAxis;
        private bool invertColumnAxis;
        private QrIndexOrder indexOrder = QrIndexOrder.RowColumn;
        private BoardMappingModelKind mappingModel = BoardMappingModelKind.Affine;
        private FitPixelSource fitPixelSource = FitPixelSource.DetectedQrCenter;
        private string qrSeparator = "-";
        private string qrPolarity = "dark_on_light";
        private string qrDefaultParameters = "maximum_recognition";
        private int stopAfterResultNum = 97;
        private double maxAllowedErrorMm = 0.2;
        private QrCenterSource centerSource = QrCenterSource.SmallestRectangleCenter;

        public string ImagePath { get => imagePath; set => Set(ref imagePath, value); }
        public int BoardRows { get => boardRows; set => Set(ref boardRows, Math.Max(1, value)); }
        public int BoardColumns { get => boardColumns; set => Set(ref boardColumns, Math.Max(1, value)); }
        public double SpacingX { get => spacingX; set => Set(ref spacingX, value); }
        public double SpacingY { get => spacingY; set => Set(ref spacingY, value); }
        public double OriginX { get => originX; set => Set(ref originX, value); }
        public double OriginY { get => originY; set => Set(ref originY, value); }
        public double PixelCellWidth { get => pixelCellWidth; set => Set(ref pixelCellWidth, value); }
        public double PixelCellHeight { get => pixelCellHeight; set => Set(ref pixelCellHeight, value); }
        public double PixelOriginColumn { get => pixelOriginColumn; set => Set(ref pixelOriginColumn, value); }
        public double PixelOriginRow { get => pixelOriginRow; set => Set(ref pixelOriginRow, value); }
        public double QrPhysicalWidth { get => qrPhysicalWidth; set => Set(ref qrPhysicalWidth, value); }
        public double QrPhysicalHeight { get => qrPhysicalHeight; set => Set(ref qrPhysicalHeight, value); }
        public double QrPixelWidth { get => qrPixelWidth; set => Set(ref qrPixelWidth, value); }
        public double QrPixelHeight { get => qrPixelHeight; set => Set(ref qrPixelHeight, value); }
        public int SerialBaseIndex { get => serialBaseIndex; set => Set(ref serialBaseIndex, value); }
        public int QrPerRow { get => qrPerRow; set => Set(ref qrPerRow, Math.Max(1, value)); }
        public int QrColumnStep { get => qrColumnStep; set => Set(ref qrColumnStep, Math.Max(1, value)); }
        public int QrRowStep { get => qrRowStep; set => Set(ref qrRowStep, Math.Max(1, value)); }
        public int QrOddRowColumnOffset { get => qrOddRowColumnOffset; set => Set(ref qrOddRowColumnOffset, value); }
        public bool InvertRowAxis { get => invertRowAxis; set => Set(ref invertRowAxis, value); }
        public bool InvertColumnAxis { get => invertColumnAxis; set => Set(ref invertColumnAxis, value); }
        public QrIndexOrder IndexOrder { get => indexOrder; set => Set(ref indexOrder, value); }
        public BoardMappingModelKind MappingModel { get => mappingModel; set => Set(ref mappingModel, value); }
        public FitPixelSource FitPixelSource { get => fitPixelSource; set => Set(ref fitPixelSource, value); }
        public string QrSeparator { get => qrSeparator; set => Set(ref qrSeparator, value ?? string.Empty); }
        public string QrPolarity { get => qrPolarity; set => Set(ref qrPolarity, value ?? string.Empty); }
        public string QrDefaultParameters { get => qrDefaultParameters; set => Set(ref qrDefaultParameters, value ?? string.Empty); }
        public int StopAfterResultNum { get => stopAfterResultNum; set => Set(ref stopAfterResultNum, Math.Max(1, value)); }
        public double MaxAllowedErrorMm { get => maxAllowedErrorMm; set => Set(ref maxAllowedErrorMm, value); }
        public QrCenterSource CenterSource { get => centerSource; set => Set(ref centerSource, value); }

        public double PhysicalXFromIndex(int rowIndex, int columnIndex)
        {
            int col = InvertColumnAxis ? BoardColumns - 1 - columnIndex : columnIndex;
            return OriginX + col * SpacingX + QrPhysicalWidth / 2.0;
        }

        public double PhysicalYFromIndex(int rowIndex, int columnIndex)
        {
            int row = InvertRowAxis ? BoardRows - 1 - rowIndex : rowIndex;
            return OriginY + row * SpacingY + QrPhysicalHeight / 2.0;
        }

        public double PixelColumnFromIndex(int rowIndex, int columnIndex)
        {
            int col = InvertColumnAxis ? BoardColumns - 1 - columnIndex : columnIndex;
            return PixelOriginColumn + col * PixelCellWidth + QrPixelWidth / 2.0;
        }

        public double PixelRowFromIndex(int rowIndex, int columnIndex)
        {
            int row = InvertRowAxis ? BoardRows - 1 - rowIndex : rowIndex;
            return PixelOriginRow + row * PixelCellHeight + QrPixelHeight / 2.0;
        }

        public bool TrySerialToBoardIndex(int serial, out int rowIndex, out int columnIndex)
        {
            int zeroBased = serial - SerialBaseIndex;
            if (zeroBased < 0)
            {
                rowIndex = -1;
                columnIndex = -1;
                return false;
            }

            int qrRow = zeroBased / QrPerRow;
            int qrColumnInRow = zeroBased % QrPerRow;
            rowIndex = qrRow * QrRowStep;
            columnIndex = qrColumnInRow * QrColumnStep;
            if ((qrRow & 1) == 1)
            {
                columnIndex += QrOddRowColumnOffset;
            }

            return true;
        }
    }

    public sealed class QrBoardPoint : NotifyObject
    {
        private string decoded = string.Empty;
        private int serialNumber;
        private int rowIndex;
        private int columnIndex;
        private double imageRow;
        private double imageColumn;
        private double detectedImageRow;
        private double detectedImageColumn;
        private double nominalImageRow;
        private double nominalImageColumn;
        private double pixelError;
        private double phiRad;
        private double worldX;
        private double worldY;
        private double fittedX;
        private double fittedY;
        private double errorMm;
        private bool used = true;

        public string Decoded { get => decoded; set => Set(ref decoded, value); }
        public int SerialNumber { get => serialNumber; set => Set(ref serialNumber, value); }
        public int RowIndex { get => rowIndex; set => Set(ref rowIndex, value); }
        public int ColumnIndex { get => columnIndex; set => Set(ref columnIndex, value); }
        public double ImageRow { get => imageRow; set => Set(ref imageRow, value); }
        public double ImageColumn { get => imageColumn; set => Set(ref imageColumn, value); }
        public double DetectedImageRow { get => detectedImageRow; set => Set(ref detectedImageRow, value); }
        public double DetectedImageColumn { get => detectedImageColumn; set => Set(ref detectedImageColumn, value); }
        public double NominalImageRow { get => nominalImageRow; set => Set(ref nominalImageRow, value); }
        public double NominalImageColumn { get => nominalImageColumn; set => Set(ref nominalImageColumn, value); }
        public double PixelError { get => pixelError; set => Set(ref pixelError, value); }
        public double PhiRad { get => phiRad; set => Set(ref phiRad, value); }
        public double PhiDeg => PhiRad * 180.0 / Math.PI;
        public double WorldX { get => worldX; set => Set(ref worldX, value); }
        public double WorldY { get => worldY; set => Set(ref worldY, value); }
        public double FittedX { get => fittedX; set => Set(ref fittedX, value); }
        public double FittedY { get => fittedY; set => Set(ref fittedY, value); }
        public double ErrorMm { get => errorMm; set => Set(ref errorMm, value); }
        public bool Used { get => used; set => Set(ref used, value); }
    }

    public sealed class BoardMappingResult : NotifyObject
    {
        private bool success;
        private string message = string.Empty;
        private int detectedCount;
        private int usedCount;
        private double rmsErrorMm;
        private double maxErrorMm;
        private double avgErrorMm;

        public ObservableCollection<QrBoardPoint> Points { get; } = new ObservableCollection<QrBoardPoint>();

        public bool Success { get => success; set => Set(ref success, value); }
        public string Message { get => message; set => Set(ref message, value); }
        public int DetectedCount { get => detectedCount; set => Set(ref detectedCount, value); }
        public int UsedCount { get => usedCount; set => Set(ref usedCount, value); }
        public double RmsErrorMm { get => rmsErrorMm; set => Set(ref rmsErrorMm, value); }
        public double MaxErrorMm { get => maxErrorMm; set => Set(ref maxErrorMm, value); }
        public double AvgErrorMm { get => avgErrorMm; set => Set(ref avgErrorMm, value); }
    }

    public abstract class NotifyObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected bool Set<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value))
            {
                return false;
            }

            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }
    }
}

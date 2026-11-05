using Plugin.CACRunCalibration.ViewModels;
using System.Collections.ObjectModel;

namespace Plugin.CACRunCalibration.Models
{
    /// <summary>
    /// CAC运行标定插件的界面模型，继承自 BindableBase，实现属性变更通知。
    /// 包含标定板参数、二维码检测设置、ROI、转换输入输出、运行状态等所有界面绑定数据。
    /// </summary>
    public sealed class CACRunCalibrationModel : BindableBase
    {
        // ==================== 标定板物理参数 ====================
        private int boardRows;                  // 标定板行数（棋盘格或二维码阵列的行数）
        private int boardColumns;               // 标定板列数
        private double spacingX;                // 相邻点水平间距（毫米）
        private double spacingY;                // 相邻点垂直间距（毫米）
        private double originX;                 // 标定板原点 X 坐标（毫米）
        private double originY;                 // 标定板原点 Y 坐标（毫米）
        private double qrPhysicalWidth;         // 单个二维码物理宽度（毫米）
        private double qrPhysicalHeight;        // 单个二维码物理高度（毫米）

        // ==================== 二维码序列映射参数 ====================
        private int serialBaseIndex;            // 二维码序列号的起始值（通常为1）
        private int qrPerRow;                   // 每行二维码个数
        private int qrColumnStep;               // 列步长（序列号递增时列索引的增量）
        private int qrRowStep;                  // 行步长（序列号递增时行索引的增量）
        private int qrOddRowColumnOffset;       // 奇数行额外列偏移（用于蛇形布局）
        private bool invertRowAxis;             // 是否反转行轴
        private bool invertColumnAxis;          // 是否反转列轴

        // ==================== 二维码识别参数 ====================
        private string qrPolarity;              // 二维码极性（如 "dark_on_light"）
        private string qrDefaultParameters;      // 二维码默认参数（Halcon 模型参数）
        private string centerSource;            // 中心点来源："DataCodeCenter" 或其他
        private string mappingModel;            // 映射模型（目前未使用，预留扩展）
        private int stopAfterResultNum;         // 最多识别的二维码数量

        // ==================== 误差与 ROI 控制 ====================
        private double maxAllowedErrorMm;       // 允许的最大单点拟合误差（毫米，可用于报警）
        private bool enableRoi;                 // 是否启用 ROI
        private bool showViewRoi;               // 是否在图像上显示 ROI 矩形
        private double roiRow1;                 // ROI 上边缘行坐标
        private double roiCol1;                 // ROI 左边缘列坐标
        private double roiRow2;                 // ROI 下边缘行坐标
        private double roiCol2;                 // ROI 右边缘列坐标

        // ==================== 像素坐标转换测试 ====================
        private double inputPixelRow;           // 输入的像素行坐标（用于测试转换）
        private double inputPixelCol;           // 输入的像素列坐标
        private double outputBoardX;            // 转换输出的物理 X 坐标
        private double outputBoardY;            // 转换输出的物理 Y 坐标
        private bool transformSuccess;          // 最后一次转换是否成功

        // ==================== 矩阵状态与消息 ====================
        private bool hasValidMatrix;            // 是否存在有效矩阵
        private string matrixStateText;         // 矩阵状态文本（如“矩阵有效”、“无有效矩阵”）
        private string matrixCheckMessage;      // 矩阵校验消息（详细描述）
        private string runMessage;              // 当前运行/操作消息（用于状态栏显示）
        private string imageStateText;          // 图像状态文本（如“已加载图像”、“未加载”）
        private string loadedImagePath;         // 已加载图像的完整路径
        private string lastSaveTime;            // 上次保存矩阵的时间文本

        // ==================== 标定结果统计 ====================
        private int detectedCount;              // 检测到的二维码总数
        private int usedCount;                  // 实际参与拟合的点数（被勾选的）
        private double avgErrorMm;              // 平均拟合误差（毫米）
        private double rmsErrorMm;              // 均方根拟合误差（毫米）
        private double maxErrorMm;              // 最大拟合误差（毫米）
        private string resultStateText;         // 最近一次操作的结果状态（如“成功”、“失败”）

        // ==================== UI 状态 ====================
        private bool isBusy;                    // 是否正在执行耗时操作（用于禁用按钮、显示进度）

        // ==================== 点表集合 ====================
        /// <summary>
        /// 标定点列表，用于 DataGrid 绑定，支持动态增删和界面更新。
        /// </summary>
        public ObservableCollection<CalibrationPointRow> PointRows { get; } = new ObservableCollection<CalibrationPointRow>();

        // ==================== 属性访问器 ====================

        /// <summary>标定板行数</summary>
        public int BoardRows { get { return boardRows; } set { SetProperty(ref boardRows, value); } }

        /// <summary>标定板列数</summary>
        public int BoardColumns { get { return boardColumns; } set { SetProperty(ref boardColumns, value); } }

        /// <summary>相邻点水平间距（毫米）</summary>
        public double SpacingX { get { return spacingX; } set { SetProperty(ref spacingX, value); } }

        /// <summary>相邻点垂直间距（毫米）</summary>
        public double SpacingY { get { return spacingY; } set { SetProperty(ref spacingY, value); } }

        /// <summary>标定板原点 X 坐标（毫米）</summary>
        public double OriginX { get { return originX; } set { SetProperty(ref originX, value); } }

        /// <summary>标定板原点 Y 坐标（毫米）</summary>
        public double OriginY { get { return originY; } set { SetProperty(ref originY, value); } }

        /// <summary>单个二维码物理宽度（毫米）</summary>
        public double QrPhysicalWidth { get { return qrPhysicalWidth; } set { SetProperty(ref qrPhysicalWidth, value); } }

        /// <summary>单个二维码物理高度（毫米）</summary>
        public double QrPhysicalHeight { get { return qrPhysicalHeight; } set { SetProperty(ref qrPhysicalHeight, value); } }

        /// <summary>二维码序列号的起始值</summary>
        public int SerialBaseIndex { get { return serialBaseIndex; } set { SetProperty(ref serialBaseIndex, value); } }

        /// <summary>每行二维码个数</summary>
        public int QrPerRow { get { return qrPerRow; } set { SetProperty(ref qrPerRow, value); } }

        /// <summary>列步长</summary>
        public int QrColumnStep { get { return qrColumnStep; } set { SetProperty(ref qrColumnStep, value); } }

        /// <summary>行步长</summary>
        public int QrRowStep { get { return qrRowStep; } set { SetProperty(ref qrRowStep, value); } }

        /// <summary>奇数行额外列偏移</summary>
        public int QrOddRowColumnOffset { get { return qrOddRowColumnOffset; } set { SetProperty(ref qrOddRowColumnOffset, value); } }

        /// <summary>是否反转行轴</summary>
        public bool InvertRowAxis { get { return invertRowAxis; } set { SetProperty(ref invertRowAxis, value); } }

        /// <summary>是否反转列轴</summary>
        public bool InvertColumnAxis { get { return invertColumnAxis; } set { SetProperty(ref invertColumnAxis, value); } }

        /// <summary>二维码极性</summary>
        public string QrPolarity { get { return qrPolarity; } set { SetProperty(ref qrPolarity, value); } }

        /// <summary>二维码默认参数</summary>
        public string QrDefaultParameters { get { return qrDefaultParameters; } set { SetProperty(ref qrDefaultParameters, value); } }

        /// <summary>中心点来源（"DataCodeCenter" 或其它）</summary>
        public string CenterSource { get { return centerSource; } set { SetProperty(ref centerSource, value); } }

        /// <summary>映射模型</summary>
        public string MappingModel { get { return mappingModel; } set { SetProperty(ref mappingModel, value); } }

        /// <summary>最多识别二维码数</summary>
        public int StopAfterResultNum { get { return stopAfterResultNum; } set { SetProperty(ref stopAfterResultNum, value); } }

        /// <summary>允许的最大单点拟合误差（毫米）</summary>
        public double MaxAllowedErrorMm { get { return maxAllowedErrorMm; } set { SetProperty(ref maxAllowedErrorMm, value); } }

        /// <summary>是否启用 ROI</summary>
        public bool EnableRoi { get { return enableRoi; } set { SetProperty(ref enableRoi, value); } }

        /// <summary>是否在图像上显示 ROI 矩形</summary>
        public bool ShowViewRoi { get { return showViewRoi; } set { SetProperty(ref showViewRoi, value); } }

        /// <summary>ROI 上边缘行坐标</summary>
        public double RoiRow1 { get { return roiRow1; } set { SetProperty(ref roiRow1, value); } }

        /// <summary>ROI 左边缘列坐标</summary>
        public double RoiCol1 { get { return roiCol1; } set { SetProperty(ref roiCol1, value); } }

        /// <summary>ROI 下边缘行坐标</summary>
        public double RoiRow2 { get { return roiRow2; } set { SetProperty(ref roiRow2, value); } }

        /// <summary>ROI 右边缘列坐标</summary>
        public double RoiCol2 { get { return roiCol2; } set { SetProperty(ref roiCol2, value); } }

        /// <summary>输入的像素行坐标（测试转换）</summary>
        public double InputPixelRow { get { return inputPixelRow; } set { SetProperty(ref inputPixelRow, value); } }

        /// <summary>输入的像素列坐标</summary>
        public double InputPixelCol { get { return inputPixelCol; } set { SetProperty(ref inputPixelCol, value); } }

        /// <summary>转换输出的物理 X 坐标</summary>
        public double OutputBoardX { get { return outputBoardX; } set { SetProperty(ref outputBoardX, value); } }

        /// <summary>转换输出的物理 Y 坐标</summary>
        public double OutputBoardY { get { return outputBoardY; } set { SetProperty(ref outputBoardY, value); } }

        /// <summary>最后一次转换是否成功</summary>
        public bool TransformSuccess { get { return transformSuccess; } set { SetProperty(ref transformSuccess, value); } }

        /// <summary>是否存在有效矩阵</summary>
        public bool HasValidMatrix { get { return hasValidMatrix; } set { SetProperty(ref hasValidMatrix, value); } }

        /// <summary>矩阵状态文本</summary>
        public string MatrixStateText { get { return matrixStateText; } set { SetProperty(ref matrixStateText, value); } }

        /// <summary>矩阵校验消息</summary>
        public string MatrixCheckMessage { get { return matrixCheckMessage; } set { SetProperty(ref matrixCheckMessage, value); } }

        /// <summary>运行/操作消息</summary>
        public string RunMessage { get { return runMessage; } set { SetProperty(ref runMessage, value); } }

        /// <summary>图像状态文本</summary>
        public string ImageStateText { get { return imageStateText; } set { SetProperty(ref imageStateText, value); } }

        /// <summary>已加载图像的完整路径</summary>
        public string LoadedImagePath { get { return loadedImagePath; } set { SetProperty(ref loadedImagePath, value); } }

        /// <summary>上次保存矩阵的时间文本</summary>
        public string LastSaveTime { get { return lastSaveTime; } set { SetProperty(ref lastSaveTime, value); } }

        /// <summary>检测到的二维码总数</summary>
        public int DetectedCount { get { return detectedCount; } set { SetProperty(ref detectedCount, value); } }

        /// <summary>实际参与拟合的点数</summary>
        public int UsedCount { get { return usedCount; } set { SetProperty(ref usedCount, value); } }

        /// <summary>平均拟合误差（毫米）</summary>
        public double AvgErrorMm { get { return avgErrorMm; } set { SetProperty(ref avgErrorMm, value); } }

        /// <summary>均方根拟合误差（毫米）</summary>
        public double RmsErrorMm { get { return rmsErrorMm; } set { SetProperty(ref rmsErrorMm, value); } }

        /// <summary>最大拟合误差（毫米）</summary>
        public double MaxErrorMm { get { return maxErrorMm; } set { SetProperty(ref maxErrorMm, value); } }

        /// <summary>最近操作的结果状态文本（如“成功”、“失败”）</summary>
        public string ResultStateText { get { return resultStateText; } set { SetProperty(ref resultStateText, value); } }

        /// <summary>是否正在执行耗时操作</summary>
        public bool IsBusy { get { return isBusy; } set { SetProperty(ref isBusy, value); } }
    }
}
using Plugin.CACRunCalibration.ViewModels;
using System;

namespace Plugin.CACRunCalibration.Models
{
    /// <summary>
    /// 标定板上的一个识别点（二维码）的完整数据模型。
    /// 继承自 BindableBase，支持属性变更通知，用于界面表格绑定和实时刷新。
    /// </summary>
    public sealed class CalibrationPointRow : BindableBase
    {
        // 是否参与矩阵拟合（界面勾选状态）
        private bool used = true;
        // 二维码解码后的原始字符串
        private string decoded = string.Empty;
        // 从解码字符串中提取的序列号
        private int serialNumber;
        // 标定板上对应的行索引
        private int boardRow;
        // 标定板上对应的列索引
        private int boardColumn;
        // 图像上的行坐标（由最小外接矩形中心或二维码内部算法给出）
        private double imageRow;
        // 图像上的列坐标
        private double imageColumn;
        // 检测到的原始行坐标（与 ImageRow 可能相同，用于记录初始检测值）
        private double detectedRow;
        // 检测到的原始列坐标
        private double detectedColumn;
        // 名义行坐标（基于板参数计算，未考虑矩阵）
        private double nominalRow;
        // 名义列坐标
        private double nominalColumn;
        // 像素误差：图像检测坐标与名义坐标的欧氏距离
        private double pixelError;
        // 二维码符号的旋转角度（弧度）
        private double phiRad;
        // 物理世界 X 坐标（基于板参数和行列索引计算）
        private double worldX;
        // 物理世界 Y 坐标
        private double worldY;
        // 拟合后的 X 坐标（将图像坐标通过当前矩阵反算得到）
        private double fittedX;
        // 拟合后的 Y 坐标
        private double fittedY;
        // 拟合误差（毫米）：拟合坐标与理论世界坐标的欧氏距离
        private double errorMm;

        // ---- 属性定义 ----

        /// <summary>
        /// 是否参与矩阵拟合。true 表示该点在计算变换矩阵时会被使用。
        /// </summary>
        public bool Used { get { return used; } set { SetProperty(ref used, value); } }

        /// <summary>
        /// 二维码解码后的原始字符串。
        /// </summary>
        public string Decoded { get { return decoded; } set { SetProperty(ref decoded, value); } }

        /// <summary>
        /// 从解码字符串中提取的数字序列号，用于映射标定板位置。
        /// </summary>
        public int SerialNumber { get { return serialNumber; } set { SetProperty(ref serialNumber, value); } }

        /// <summary>
        /// 该点在标定板上的行索引（从0开始，由上到下）。
        /// </summary>
        public int BoardRow { get { return boardRow; } set { SetProperty(ref boardRow, value); } }

        /// <summary>
        /// 该点在标定板上的列索引（从0开始，由左到右）。
        /// </summary>
        public int BoardColumn { get { return boardColumn; } set { SetProperty(ref boardColumn, value); } }

        /// <summary>
        /// 图像中的行坐标（像素）。来源于二维码中心或最小外接矩形中心。
        /// </summary>
        public double ImageRow { get { return imageRow; } set { SetProperty(ref imageRow, value); } }

        /// <summary>
        /// 图像中的列坐标（像素）。
        /// </summary>
        public double ImageColumn { get { return imageColumn; } set { SetProperty(ref imageColumn, value); } }

        /// <summary>
        /// 检测到的原始行坐标，通常与 ImageRow 一致，用于保持原始检测结果。
        /// </summary>
        public double DetectedRow { get { return detectedRow; } set { SetProperty(ref detectedRow, value); } }

        /// <summary>
        /// 检测到的原始列坐标。
        /// </summary>
        public double DetectedColumn { get { return detectedColumn; } set { SetProperty(ref detectedColumn, value); } }

        /// <summary>
        /// 名义行坐标，由标定板物理参数直接计算得到（不考虑矩阵变换）。
        /// </summary>
        public double NominalRow { get { return nominalRow; } set { SetProperty(ref nominalRow, value); } }

        /// <summary>
        /// 名义列坐标。
        /// </summary>
        public double NominalColumn { get { return nominalColumn; } set { SetProperty(ref nominalColumn, value); } }

        /// <summary>
        /// 像素误差：检测到的图像坐标与名义坐标之间的欧氏距离（单位：像素）。
        /// 用于衡量检测位置的偏差，通常不用于最终拟合误差评估。
        /// </summary>
        public double PixelError { get { return pixelError; } set { SetProperty(ref pixelError, value); } }

        /// <summary>
        /// 二维码符号的旋转角度（弧度制）。
        /// </summary>
        public double PhiRad
        {
            get { return phiRad; }
            set
            {
                SetProperty(ref phiRad, value);
                // 通知角度制属性也发生变化，便于界面显示
                OnPropertyChanged(nameof(PhiDeg));
            }
        }

        /// <summary>
        /// 二维码符号的旋转角度（角度制，只读）。
        /// </summary>
        public double PhiDeg { get { return PhiRad * 180.0 / Math.PI; } }

        /// <summary>
        /// 标定板上的理论物理世界 X 坐标（毫米），基于板参数和行列位置计算。
        /// </summary>
        public double WorldX { get { return worldX; } set { SetProperty(ref worldX, value); } }

        /// <summary>
        /// 标定板上的理论物理世界 Y 坐标（毫米）。
        /// </summary>
        public double WorldY { get { return worldY; } set { SetProperty(ref worldY, value); } }

        /// <summary>
        /// 拟合后的物理 X 坐标。通过当前变换矩阵将图像坐标转换得到。
        /// </summary>
        public double FittedX { get { return fittedX; } set { SetProperty(ref fittedX, value); } }

        /// <summary>
        /// 拟合后的物理 Y 坐标。
        /// </summary>
        public double FittedY { get { return fittedY; } set { SetProperty(ref fittedY, value); } }

        /// <summary>
        /// 拟合误差（单位：毫米）。计算方式为 FittedX/Y 与 WorldX/Y 的欧氏距离。
        /// 此值反映该点在当前矩阵下的残差大小。
        /// </summary>
        public double ErrorMm { get { return errorMm; } set { SetProperty(ref errorMm, value); } }
    }
}
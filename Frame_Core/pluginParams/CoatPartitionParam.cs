using System;
using System.ComponentModel;

namespace Frame_Core.pluginParams
{
    /// <summary>
    /// 涂布缺陷检测分区参数。该类后续迁移到 Frame_Core\pluginParams，必须保持纯参数类，不继承任何插件 MVVM 基类。
    /// </summary>
    [Serializable]
    public class CoatPartitionParam
    {
        /// <summary>分区序号，从 1 开始。界面、日志和结果追踪使用。</summary>
        [Description("分区序号，从1开始")]
        public int Index { get; set; }

        /// <summary>分区名称。默认“分区1/分区2”，可在界面显示。</summary>
        [Description("分区名称")]
        public string Name { get; set; } = "分区";

        /// <summary>是否启用该分区。关闭后该分区不执行检测，但仍占用长度位置。</summary>
        [Description("是否启用该分区")]
        public bool Enabled { get; set; } = true;

        /// <summary>长度是否由用户固定。true 时自动换算不会覆盖该分区长度。</summary>
        [Description("长度是否由用户固定")]
        public bool IsLengthFixed { get; set; }

        /// <summary>分区长度，单位 px。</summary>
        [Description("分区长度(px)")]
        public double Length { get; set; } = 100;

        /// <summary>分区长度，单位 mm。通过公共像素单量和 Length 互相换算，分区内不保存像素单量。</summary>
        [Description("分区长度(mm)")]
        public double LengthMm { get; set; } = 100;

        /// <summary>参考灰度。暗/亮阈值都以该值为中心。</summary>
        [Description("参考灰度")]
        public int ReferenceGray { get; set; } = 128;

        /// <summary>亮缺陷阈值上偏差。亮阈值下限 = ReferenceGray + UpGrayOffset。</summary>
        [Description("亮缺陷阈值上偏差")]
        public int UpGrayOffset { get; set; } = 35;

        /// <summary>暗缺陷阈值下偏差。暗阈值上限 = ReferenceGray - DownGrayOffset。</summary>
        [Description("暗缺陷阈值下偏差")]
        public int DownGrayOffset { get; set; } = 35;

        /// <summary>开运算半径，单位 px。用于去除小噪声。</summary>
        [Description("开运算半径(px)")]
        public int OpenRadius { get; set; } = 1;

        /// <summary>闭运算半径，单位 px。用于连接邻近区域。</summary>
        [Description("闭运算半径(px)")]
        public int CloseRadius { get; set; } = 2;

        /// <summary>最小候选面积，单位 mm²。运行时根据 PixelToMmH/PixelToMmV 换算成 px² 做 SelectShape。</summary>
        [Description("最小候选面积(mm²)")]
        public double MinCandidateArea { get; set; } = 10;

        /// <summary>是否启用背景灰度归一化，对应源程序 PretreatmentImg 中的 ScaleImage 逻辑。</summary>
        [Description("启用背景灰度归一化")]
        public bool EnableBackgroundGrayScale { get; set; }

        /// <summary>是否启用小区域合并。只对最大面积小于 SmallRegionMergeMaxArea 的区域执行 ClosingRectangle1。</summary>
        [Description("启用小区域合并")]
        public bool EnableSmallRegionMerge { get; set; } = true;

        /// <summary>小区域合并最大面积阈值，单位 px²。默认 1000，对齐源程序逻辑。</summary>
        [Description("小区域合并最大面积(px²)")]
        public double SmallRegionMergeMaxArea { get; set; } = 1000;

        /// <summary>小区域合并矩形宽度，单位 px。</summary>
        [Description("小区域合并矩形宽度(px)")]
        public int SmallRegionMergeWidth { get; set; } = 3;

        /// <summary>小区域合并矩形高度，单位 px。</summary>
        [Description("小区域合并矩形高度(px)")]
        public int SmallRegionMergeHeight { get; set; } = 3;

        /// <summary>ScaleImage 最大倍率绝对值。超过该值时回退为 1，避免 HALCON 参数越界。</summary>
        [Description("灰度归一化最大倍率绝对值")]
        public double MaxGrayScaleAbs { get; set; } = 1024;

        /// <summary>系统自动设置像素长度，不改变 IsLengthFixed，用于换算和自动分配剩余长度。</summary>
        public void SetLengthFromSystem(double value)
        {
            Length = value;
        }

        /// <summary>系统自动设置毫米长度，不改变 IsLengthFixed，用于公共像素单量变化后的反向显示。</summary>
        public void SetLengthMmFromSystem(double value)
        {
            LengthMm = value;
        }

        /// <summary>浅拷贝分区参数。运行时复制一份，避免 NormalizePartitions 修改界面绑定对象。</summary>
        public CoatPartitionParam Clone()
        {
            return (CoatPartitionParam)MemberwiseClone();
        }
    }
}
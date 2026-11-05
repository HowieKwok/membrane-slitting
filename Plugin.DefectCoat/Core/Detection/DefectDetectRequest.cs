using Frame_Core.pluginParams;
using FrameCommunity.公共类;
using HalconDotNet;
using Plugin.DefectCoat.Models;
using System.Collections.Generic;

namespace Plugin.DefectCoat.Core.Detection
{
    /// <summary>
    /// 检测器输入请求对象。
    /// DefectDetectPipeline.Run 会把 ParamsDefectCoat 中的持久化参数拷贝到这里；
    /// HalconThresholdDefectDetector 只依赖该对象，不直接依赖界面 ViewModel。
    /// </summary>
    public sealed class DefectDetectRequest
    {
        /// <summary>输入图像。必须是已初始化的 HObject，检测器内部会转成灰度图。</summary>
        public HObject Image { get; set; }

        /// <summary>分区数量。1 表示单分区检测，多于 1 表示按 Partitions 逐区检测。</summary>
        public int PartitionCount { get; set; }

        /// <summary>分区方向。LongSide 切 Row 方向，WideSide 切 Col 方向。</summary>
        public PartitionSplitDirection PartitionDirection { get; set; }

        /// <summary>分区长度显示单位。检测器最终使用像素长度，界面负责和毫米互相换算。</summary>
        public PartitionLengthUnit PartitionLengthUnit { get; set; }

        /// <summary>默认参考灰度。新建分区时使用；实际检测以分区参数为准。</summary>
        public int ReferenceGray { get; set; }

        /// <summary>默认亮缺陷灰度上偏差。</summary>
        public int UpGrayOffset { get; set; }

        /// <summary>默认暗缺陷灰度下偏差。</summary>
        public int DownGrayOffset { get; set; }

        /// <summary>默认开运算半径，单位 px。</summary>
        public int OpenRadius { get; set; }

        /// <summary>默认闭运算半径，单位 px。</summary>
        public int CloseRadius { get; set; }

        /// <summary>默认最小候选面积，单位 mm²。</summary>
        public double MinCandidateArea { get; set; }

        /// <summary>默认是否启用背景灰度归一化。</summary>
        public bool EnableBackgroundGrayScale { get; set; }

        /// <summary>默认是否启用小区域 ClosingRectangle1 合并。</summary>
        public bool EnableSmallRegionMerge { get; set; }

        /// <summary>默认小区域合并最大面积阈值，单位 px²。</summary>
        public double SmallRegionMergeMaxArea { get; set; }

        /// <summary>默认小区域合并矩形宽度，单位 px。</summary>
        public int SmallRegionMergeWidth { get; set; }

        /// <summary>默认小区域合并矩形高度，单位 px。</summary>
        public int SmallRegionMergeHeight { get; set; }

        /// <summary>ScaleImage 最大倍率绝对值，避免 HALCON 参数越界。</summary>
        public double MaxGrayScaleAbs { get; set; }

        /// <summary>横向像素单量，单位 mm/pixel。</summary>
        public double PixelToMmH { get; set; }

        /// <summary>纵向像素单量，单位 mm/pixel。</summary>
        public double PixelToMmV { get; set; }
        /// <summary>
        /// 是否启用检测阶段候选数量硬上限。
        /// 启用后，检测算法在 SelectShape 后先统计候选数量；
        /// 若数量超过 MaxCandidateCount，则不生成单个 DefectCandidate，避免逐个裁图、求交和 CopyObj。
        /// </summary>
        public bool EnableCandidateCountLimit { get; set; }

        /// <summary>
        /// 检测阶段允许生成单个候选的最大数量。
        /// 只有候选总数小于等于该值时，才会执行 AppendCandidates。
        /// </summary>
        public int MaxCandidateCount { get; set; }

        /// <summary>
        /// 检测候选数量超限时输出的类别名称，例如“涂布不良”。
        /// 该值来自分类规则配置，但检测层只保存名称，不直接依赖分类规则对象。
        /// </summary>
        public string OverflowClassName { get; set; }
        /// <summary>检测范围模式。LocalRoi 时使用 LocalRoi 生成检测 Region。</summary>
        public DetectRangeMode DetectMode { get; set; }

        /// <summary>是否启用检测 ROI。</summary>
        public bool EnableLocalRoi { get; set; }

        /// <summary>是否启用屏蔽 ROI。</summary>
        public bool EnableMaskRoi { get; set; }

        /// <summary>是否启用位置跟随。启用后 ROI Region 使用 HALCON 刚性仿射旋转/平移。</summary>
        public bool EnablePositionFollow { get; set; }

        /// <summary>主检测 ROI。当前要求 Rectangle1。</summary>
        public CoatRoiParam LocalRoi { get; set; }

        /// <summary>屏蔽 ROI 列表。每个 ROI 会生成 Region 后 Union，再从缺陷候选中扣除。</summary>
        public IList<CoatRoiParam> MaskRois { get; set; }

        /// <summary>分区参数列表。每个分区拥有独立阈值、过滤、形态学参数。</summary>
        public IList<CoatPartitionParam> Partitions { get; set; }

        /// <summary>位置跟随基准行坐标，单位 px。</summary>
        public double BaseRow { get; set; }

        /// <summary>位置跟随基准列坐标，单位 px。</summary>
        public double BaseCol { get; set; }

        /// <summary>位置跟随基准角度，单位度。</summary>
        public double BaseAngle { get; set; }

        /// <summary>当前产品定位行坐标，单位 px。</summary>
        public double CurrentRow { get; set; }

        /// <summary>当前产品定位列坐标，单位 px。</summary>
        public double CurrentCol { get; set; }

        /// <summary>当前产品定位角度，单位度。</summary>
        public double CurrentAngle { get; set; }
    }
}



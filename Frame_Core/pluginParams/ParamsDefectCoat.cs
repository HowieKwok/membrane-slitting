using DefectRuleClassifier.Core.Classifiers;
using DefectRuleClassifier.Core.Models;
using Frame_Core;
using FrameCommunity.公共类;
using HalconDotNet;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml.Serialization;


namespace Frame_Core.pluginParams
{


    /// <summary>
    /// 缺陷检测模块参数，必须继承 Frame_Core.PluginParamsBase。
    /// 框架只认识 IPlugin.PluginParams 这一条参数入口，因此检测输入、可保存参数、运行输出结果都必须能从本类找到。
    /// 分类规则明细不放在这里，本类只保存 RuleConfigPath，让 InitObj 按路径加载独立分类参数，便于多个模块共用同一套分类规则。
    /// HALCON 对象和运行结果仅作运行期传递，不写入 XML。
    /// </summary>
    [Serializable]
    public sealed class ParamsDefectCoat : PluginParamsBase
    {
        #region 框架运行输入：必须放在 PluginParamsBase 派生类中

        /// <summary>
        /// 输入图像。框架插件运行时由上游模块或宿主写入 ParamsDefectCoat.ho_Image，ExecuteObj 从这里取图检测。
        /// 这是运行输入，必须放在 PluginParamsBase 派生类中，不能只放在 ViewModel 或检测器私有字段里。
        /// 运行态对象不写入 XML。
        /// </summary>
        [NonSerialized]
        [XmlIgnore]
        [Description("输入图像（运行时）")]
        public HObject ho_Image;

        /// <summary>
        /// 输入图像是否由本模块创建并拥有释放权。
        /// 插件宿主传入的 ho_Image 默认不归本模块释放；Demo 自己加载图像时才置为 true。
        /// </summary>
        [NonSerialized]
        [XmlIgnore]
        [Description("输入图像是否由本模块创建并拥有释放权")]
        public bool OwnsInputImage;

        #endregion

        #region 框架运行输出：必须放在 PluginParamsBase 派生类中

        /// <summary>
        /// 实际参与检测的检测区域 ROI。运行后写回 PluginParams，供宿主显示、后续模块读取；运行态对象，不写入 XML。
        /// </summary>
        [NonSerialized]
        [XmlIgnore]
        [Description("实际参与检测的检测区域ROI")]
        public HObject ho_DetectRegion;

        /// <summary>
        /// 全部候选缺陷合并区域。用于主窗口总览显示；运行态对象，不写入 XML。
        /// </summary>
        [NonSerialized]
        [XmlIgnore]
        [Description("全部候选缺陷合并区域")]
        public HObject ho_ResultRegion;

        /// <summary>
        /// 已分类缺陷区域。显示颜色约定为红色；运行态对象，不写入 XML。
        /// </summary>
        [NonSerialized]
        [XmlIgnore]
        [Description("已分类缺陷区域")]
        public HObject ho_ClassifiedRegion;

        /// <summary>
        /// 未分类缺陷区域。显示颜色约定为绿色；运行态对象，不写入 XML。
        /// </summary>
        [NonSerialized]
        [XmlIgnore]
        [Description("未分类缺陷区域")]
        public HObject ho_UnclassifiedRegion;

        /// <summary>
        /// 已命中类别但尺寸判定未达要求的区域。显示颜色约定为橙色；运行态对象，不写入 XML。
        /// </summary>
        [NonSerialized]
        [XmlIgnore]
        [Description("已命中类别但尺寸判定未达要求的区域")]
        public HObject ho_NotMeetRegion;

        /// <summary>
        /// 屏蔽 ROI 合并区域。检测时从缺陷区域中扣除，同时输出给界面显示；运行态对象，不写入 XML。
        /// </summary>
        [NonSerialized]
        [XmlIgnore]
        [Description("屏蔽ROI合并区域")]
        public HObject ho_MaskRoiRegion;

        /// <summary>
        /// 预处理后的灰度图。用于调试背景灰度归一化、阈值结果；运行态对象，不写入 XML。
        /// </summary>
        [NonSerialized]
        [XmlIgnore]
        [Description("预处理后的灰度图")]
        public HObject ho_PreprocessImage;

        /// <summary>
        /// 暗缺陷阈值区域。用于算法调试显示；运行态对象，不写入 XML。
        /// </summary>
        [NonSerialized]
        [XmlIgnore]
        [Description("暗缺陷阈值区域")]
        public HObject ho_DarkRegion;

        /// <summary>
        /// 亮缺陷阈值区域。用于算法调试显示；运行态对象，不写入 XML。
        /// </summary>
        [NonSerialized]
        [XmlIgnore]
        [Description("亮缺陷阈值区域")]
        public HObject ho_LightRegion;

        /// <summary>
        /// 最终缺陷结果列表。每个结果内部保存小图、NG Region 和特征值。
        /// 这是最重要的框架输出，必须放在 PluginParamsBase 派生类中，方便后续插件通过变量名读取。
        /// 运行态集合不写入 XML。
        /// </summary>
        [XmlIgnore]
        [Description("最终缺陷结果列表")]
        public ObservableCollection<NgDefectResult> NgResults { get; set; } = new ObservableCollection<NgDefectResult>();

        #endregion

        #region 模块入口与分类开关

        /// <summary>是否启用特征值分类。框架保存的主参数开关；启用后按独立分类规则文件、优先级和特征范围分类。</summary>
        [Description("是否启用特征值分类")]
        public bool EnableFeatureRuleClassify { get; set; } = true;

        /// <summary>是否启用传统分类器分类。框架保存的主参数开关；接口和对象已预留，模型参数由分类规则配置关联路径加载。</summary>
        [Description("是否启用传统分类器分类")]
        public bool EnableModelClassify { get; set; }

        /// <summary>是否启用 AI 整图分类。框架保存的主参数开关；当前为占位接口，后续接入模型后由 InitObj 根据路径初始化。</summary>
        [Description("是否启用AI整图分类")]
        public bool EnableAiClassify { get; set; }

        /// <summary>
        /// 分类规则文件路径。主参数只保存这一个入口。
        /// 具体的 FeatureRuleClassifierConfig、ModelClassifierConfig、AiClassifierConfig 不放在本类中，避免每个工具重复一份分类规则。
        /// </summary>
        [Description("分类规则文件路径")]
        public string RuleConfigPath { get; set; }

        /// <summary>ROI 类型显示文本。当前统一使用 Rectangle1，仅保留给界面绑定和旧参数兼容。</summary>
        [Description("ROI类型显示文本")]
        public string RoiType { get; set; } = "矩形ROI";

        /// <summary>默认缺陷名称。规则未命中或调试时用于界面显示，不代替分类规则。</summary>
        [Description("默认缺陷名称")]
        public string SelectedDefectName { get; set; } = "默认缺陷";

        /// <summary>模块说明文本。用于 Demo 界面展示，不参与算法计算。</summary>
        [Description("模块说明文本")]
        public string LabelDescription { get; set; } = "AI / 分类器接口已预留，当前 Demo 使用图像处理与规则分类主流程。";

        /// <summary>预处理调试信息。运行后写入灰度归一化倍率、阈值等信息，不作为输入参数。</summary>
        [Description("预处理调试信息")]
        public string PreprocessInfo { get; set; } = "预处理：--";

        #endregion

        #region ROI 与分区参数

        /// <summary>当前编辑 ROI 的中心行坐标，单位 px；由 ViewROI 拖动或参数面板同步。</summary>
        [Description("当前编辑ROI的中心行坐标(px)")]
        public double RoiCenterRow { get; set; } = 300;

        /// <summary>当前编辑 ROI 的中心列坐标，单位 px；由 ViewROI 拖动或参数面板同步。</summary>
        [Description("当前编辑ROI的中心列坐标(px)")]
        public double RoiCenterColumn { get; set; } = 300;

        /// <summary>ROI 角度。Rectangle1 当前不使用角度，保留给旧接口兼容。</summary>
        [Description("ROI角度")]
        public double RoiAngle { get; set; }

        /// <summary>当前编辑 ROI 高度，单位 px。</summary>
        [Description("当前编辑ROI高度(px)")]
        public double RoiHeight { get; set; } = 320;

        /// <summary>当前编辑 ROI 宽度，单位 px。</summary>
        [Description("当前编辑ROI宽度(px)")]
        public double RoiWidth { get; set; } = 420;

        /// <summary>检测分区数量。1 表示单区域检测，多于 1 时按分段方向切分 Rectangle1 检测区。</summary>
        [Description("检测分区数量")]
        public int PartitionCount { get; set; } = 1;

        /// <summary>分区切分方向。长向分段切高度，宽向分段切宽度。</summary>
        [Description("分区切分方向")]
        public PartitionSplitDirection PartitionDirection { get; set; } = PartitionSplitDirection.LongSide;

        /// <summary>分区长度输入单位。选择毫米时使用公共像素单量换算为像素后运行。</summary>
        [Description("分区长度输入单位")]
        public PartitionLengthUnit PartitionLengthUnit { get; set; } = PartitionLengthUnit.Pixel;

        /// <summary>检测范围模式。当前 Demo 主要使用局部 ROI 检测，全局检测保留给调试。</summary>
        [Description("检测范围模式")]
        public DetectRangeMode DetectMode { get; set; } = DetectRangeMode.LocalRoi;

        /// <summary>当前 ViewROI 编辑对象：检测 ROI 或屏蔽 ROI。</summary>
        [Description("当前ViewROI编辑对象")]
        public RoiEditRole EditRole { get; set; } = RoiEditRole.Local;

        /// <summary>是否启用检测区域 ROI。关闭时可退化为全图检测。</summary>
        [Description("是否启用检测区域ROI")]
        public bool EnableLocalRoi { get; set; } = true;

        /// <summary>是否启用屏蔽 ROI。启用后屏蔽区域从候选缺陷区域中扣除。</summary>
        [Description("是否启用屏蔽ROI")]
        public bool EnableMaskRoi { get; set; } = true;

        /// <summary>是否在主图显示 ViewROI 控制框。只影响界面显示，不影响检测区域数据。</summary>
        [Description("是否在主图显示ViewROI控制框")]
        public bool ShowViewRoi { get; set; } = true;

        /// <summary>是否启用位置跟随。启用后根据基准点、当前点和角度差转换检测 ROI 与屏蔽 ROI。</summary>
        [Description("是否启用位置跟随")]
        public bool EnablePositionFollow { get; set; }

        /// <summary>检测区域 ROI。统一使用 Rectangle1；分区检测会在该矩形内切分。</summary>
        [Description("检测区域ROI")]
        public CoatRoiParam LocalRoi { get; set; } = new CoatRoiParam
        {
            Index = 1,
            Name = "检测区域ROI",
            Shape = RoiShapeType.Rectangle1,
            Row1 = 140,
            Col1 = 90,
            Row2 = 460,
            Col2 = 510,
            CenterRow = 300,
            CenterCol = 300,
            Angle = 0,
            Length1 = 160,
            Length2 = 210
        };

        /// <summary>屏蔽 ROI 列表。每个 ROI 统一由 ViewROI 编辑，运行时合并为 ho_MaskRoiRegion。</summary>
        [XmlArrayItem("CoatRoiParam", typeof(CoatRoiParam))]
        [Description("屏蔽ROI列表")]
        public ObservableCollection<CoatRoiParam> MaskRois { get; set; } = new ObservableCollection<CoatRoiParam>();

        /// <summary>分区检测参数列表。每个分区保存自己的阈值、过滤、合并参数和分区长度。</summary>
        [Description("分区检测参数列表")]
        public ObservableCollection<CoatPartitionParam> Partitions { get; set; } = new ObservableCollection<CoatPartitionParam>();

        /// <summary>位置跟随基准行坐标。</summary>
        [Description("位置跟随基准行坐标")]
        public double BaseRow { get; set; }

        /// <summary>位置跟随基准列坐标。</summary>
        [Description("位置跟随基准列坐标")]
        public double BaseCol { get; set; }

        /// <summary>位置跟随基准角度，单位度。运行时与当前角度相减得到旋转角度差。</summary>
        [Description("位置跟随基准角度(度)")]
        public double BaseAngle { get; set; }

        /// <summary>当前产品定位行坐标。</summary>
        [Description("当前产品定位行坐标")]
        public double CurrentRow { get; set; }

        /// <summary>当前产品定位列坐标。</summary>
        [Description("当前产品定位列坐标")]
        public double CurrentCol { get; set; }

        /// <summary>当前产品定位角度，单位度。运行时与基准角度相减得到旋转角度差。</summary>
        [Description("当前产品定位角度(度)")]
        public double CurrentAngle { get; set; }

        #endregion

        #region 默认检测参数与单位

        /// <summary>默认参考灰度。生成新分区时拷贝到分区参数；实际运行以分区参数为准。</summary>
        [Description("默认参考灰度")]
        public int ReferenceGray { get; set; } = 128;

        /// <summary>默认亮缺陷灰度上偏差。生成新分区时拷贝到分区参数。</summary>
        [Description("默认亮缺陷灰度上偏差")]
        public int UpGrayOffset { get; set; } = 35;

        /// <summary>默认暗缺陷灰度下偏差。生成新分区时拷贝到分区参数。</summary>
        [Description("默认暗缺陷灰度下偏差")]
        public int DownGrayOffset { get; set; } = 35;

        /// <summary>默认开运算半径，单位 px。生成新分区时拷贝到分区参数。</summary>
        [Description("默认开运算半径(px)")]
        public int OpenRadius { get; set; } = 1;

        /// <summary>默认闭运算半径，单位 px。生成新分区时拷贝到分区参数。</summary>
        [Description("默认闭运算半径(px)")]
        public int CloseRadius { get; set; } = 2;

        /// <summary>默认最小候选面积，单位 mm²。生成新分区时拷贝到分区参数。</summary>
        [Description("默认最小候选面积(mm²)")]
        public double MinCandidateArea { get; set; } = 10;

        /// <summary>默认是否启用背景灰度归一化。生成新分区时拷贝到分区参数。</summary>
        [Description("默认启用背景灰度归一化")]
        public bool EnableBackgroundGrayScale { get; set; }

        /// <summary>默认是否启用小区域合并。生成新分区时拷贝到分区参数。</summary>
        [Description("默认启用小区域合并")]
        public bool EnableSmallRegionMerge { get; set; } = true;

        /// <summary>默认小区域合并最大面积，单位 px²。生成新分区时拷贝到分区参数。</summary>
        [Description("默认小区域合并最大面积(px²)")]
        public double SmallRegionMergeMaxArea { get; set; } = 1000;

        /// <summary>默认小区域矩形闭运算宽度，单位 px。生成新分区时拷贝到分区参数。</summary>
        [Description("默认小区域矩形闭运算宽度(px)")]
        public int SmallRegionMergeWidth { get; set; } = 3;

        /// <summary>默认小区域矩形闭运算高度，单位 px。生成新分区时拷贝到分区参数。</summary>
        [Description("默认小区域矩形闭运算高度(px)")]
        public int SmallRegionMergeHeight { get; set; } = 3;

        /// <summary>灰度归一化最大倍率绝对值，超过该值时回退为 1.0，避免 HALCON ScaleImage 参数越界。</summary>
        [Description("灰度归一化最大倍率绝对值")]
        public double MaxGrayScaleAbs { get; set; } = 1024;

        /// <summary>横向像素单量，单位 mm/pixel。公共单位参数，分区不再单独保存。</summary>
        [Description("横向像素单量(mm/pixel)")]
        public double PixelToMmH { get; set; } = 1;

        /// <summary>纵向像素单量，单位 mm/pixel。公共单位参数，分区不再单独保存。</summary>
        [Description("纵向像素单量(mm/pixel)")]
        public double PixelToMmV { get; set; } = 1;

        #endregion

        /// <summary>
        /// 释放一次运行产生的 HALCON 输出对象和结果列表，不释放输入图像。
        /// </summary>
        public void ReleaseOutputHObjects()
        {
            ho_DetectRegion?.Dispose();
            ho_DetectRegion = null;
            ho_ResultRegion?.Dispose();
            ho_ResultRegion = null;
            ho_ClassifiedRegion?.Dispose();
            ho_ClassifiedRegion = null;
            ho_UnclassifiedRegion?.Dispose();
            ho_UnclassifiedRegion = null;
            ho_NotMeetRegion?.Dispose();
            ho_NotMeetRegion = null;
            ho_MaskRoiRegion?.Dispose();
            ho_MaskRoiRegion = null;
            ho_PreprocessImage?.Dispose();
            ho_PreprocessImage = null;
            ho_DarkRegion?.Dispose();
            ho_DarkRegion = null;
            ho_LightRegion?.Dispose();
            ho_LightRegion = null;
            if (NgResults != null)
            {
                foreach (NgDefectResult item in NgResults)
                    item?.Dispose();
                NgResults.Clear();
            }
        }

        /// <summary>
        /// 仅在输入图像由本模块加载/创建时释放输入图像。
        /// 宿主作为插件传入的图像不应在本模块关闭时释放，否则会破坏宿主的图像生命周期。
        /// </summary>
        public void ReleaseOwnedInputImage()
        {
            if (!OwnsInputImage)
                return;

            ho_Image?.Dispose();
            ho_Image = null;
            OwnsInputImage = false;
        }

        /// <summary>
        /// 释放运行输出，并且只释放本模块拥有的输入图像。
        /// 保留给 Demo 换图、重新加载参数等明确需要清理本模块自有图像的场景。
        /// </summary>
        public void ReleaseOwnedRuntimeHObjects()
        {
            ReleaseOutputHObjects();
            ReleaseOwnedInputImage();
        }
    }
}
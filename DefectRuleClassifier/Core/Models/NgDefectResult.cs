using DefectRuleClassifier.ViewModels;
using HalconDotNet;
using System;
using System.Xml.Serialization;

namespace DefectRuleClassifier.Core.Models
{
    /// <summary>
    /// 单个缺陷的最终输出结果。
    /// 数值和文字字段用于界面绑定、结果列表显示、后续模块读取。
    /// HObject 字段只在运行期使用，不参与 XML/JSON 持久化，必须由 Dispose 统一释放。
    /// </summary>
    public sealed class NgDefectResult : BindableBase
    {
        private int index;
        private string className;
        private string source;
        private string hitRule;
        private double area;
        private double length;
        private double width;
        private double height;
        private double row;
        private double column;
        private double meanGray;
        private string polarity;
        private int classIndex = -1;
        private double confidence;
        private DefectFeatureBag features = new DefectFeatureBag();

        /// <summary>缺陷序号，用于列表显示和结果定位。</summary>
        public int Index { get => index; set => SetProperty(ref index, value); }

        /// <summary>分类后的缺陷类别，例如黑点、白点、涂布不良、未分类。</summary>
        public string ClassName { get => className; set => SetProperty(ref className, value); }

        /// <summary>结果来源，例如特征值分类、分类器分类、AI分类、数量超限、未达到设定要求。</summary>
        public string Source { get => source; set => SetProperty(ref source, value); }

        /// <summary>命中的规则说明或判定说明，用于追溯为什么输出该类别。</summary>
        public string HitRule { get => hitRule; set => SetProperty(ref hitRule, value); }

        /// <summary>缺陷面积，通常为 mm²。</summary>
        public double Area { get => area; set => SetProperty(ref area, value); }

        /// <summary>缺陷长边尺寸，通常为 mm。</summary>
        public double Length { get => length; set => SetProperty(ref length, value); }

        /// <summary>缺陷短边或宽度尺寸，通常为 mm。</summary>
        public double Width { get => width; set => SetProperty(ref width, value); }

        /// <summary>缺陷外接矩形高度，通常为 mm。</summary>
        public double Height { get => height; set => SetProperty(ref height, value); }

        /// <summary>缺陷中心行坐标，单位像素。</summary>
        public double Row { get => row; set => SetProperty(ref row, value); }

        /// <summary>缺陷中心列坐标，单位像素。</summary>
        public double Column { get => column; set => SetProperty(ref column, value); }

        /// <summary>缺陷区域平均灰度，用于结果列表快速查看。</summary>
        public double MeanGray { get => meanGray; set => SetProperty(ref meanGray, value); }

        /// <summary>缺陷极性或来源区域，例如亮缺陷、暗缺陷、多区域合并。</summary>
        public string Polarity { get => polarity; set => SetProperty(ref polarity, value); }

        /// <summary>模型标签序号（传统分类器输出，0 起）；非模型分类时为 -1。用于界面显示与下游 switch(序号) 匹配。</summary>
        public int ClassIndex { get => classIndex; set => SetProperty(ref classIndex, value); }

        /// <summary>模型分类置信度，范围 0..1；非模型分类时为 0。</summary>
        public double Confidence { get => confidence; set => SetProperty(ref confidence, value); }

        /// <summary>分类特征集合。规则分类按需读取，手动获取全部特征时会补齐更多特征。</summary>
        public DefectFeatureBag Features { get => features; set => SetProperty(ref features, value); }

        /// <summary>缺陷 NG Region。运行期 HALCON 对象，不序列化，由 Dispose 释放。</summary>
        [NonSerialized]
        [XmlIgnore]
        public HObject Region;

        /// <summary>单个缺陷裁剪小图。传统分类器分类时使用它作为输入，不保存到参数文件。</summary>
        [NonSerialized]
        [XmlIgnore]
        public HObject CropImage;

        /// <summary>当前缺陷与暗阈值区域的交集，用于暗区灰度、暗区面积等特征计算。</summary>
        [NonSerialized]
        [XmlIgnore]
        public HObject DarkRegion;

        /// <summary>当前缺陷与亮阈值区域的交集，用于亮区灰度、亮区面积等特征计算。</summary>
        [NonSerialized]
        [XmlIgnore]
        public HObject LightRegion;

        /// <summary>屏蔽 ROI 区域或无效区域，用于部分特征计算时扣除干扰区域。</summary>
        [NonSerialized]
        [XmlIgnore]
        public HObject InvalidRegion;

        /// <summary>缺陷外接矩形宽度，单位像素；运行期辅助字段，不写入参数文件。</summary>
        [XmlIgnore]
        public double WidthPx;

        /// <summary>缺陷外接矩形高度，单位像素；运行期辅助字段，不写入参数文件。</summary>
        [XmlIgnore]
        public double HeightPx;

        /// <summary>横向像素单量，单位 mm/pixel。</summary>
        [XmlIgnore]
        public double PixelToMmH = 1;

        /// <summary>纵向像素单量，单位 mm/pixel。</summary>
        [XmlIgnore]
        public double PixelToMmV = 1;

        /// <summary>检测参考灰度，部分灰度差、亮暗特征需要用它作为基准。</summary>
        [XmlIgnore]
        public int ReferenceGray;



        [XmlIgnore]
        public long T_nImgIndex { get; set; } = 0;

        [XmlIgnore]
        public long T_iProductIndex { get; set; } = 0;

        /// <summary>是否超规格</summary>
        [XmlIgnore]
        public bool T_IsOverspecification { get; set; } = true;


        /// <summary>是否超规格</summary>
        [XmlIgnore]
        public bool T_IsSevere { get; set; } = true;

        /// <summary>结果</summary>
        [XmlIgnore]
        public bool T_IsNG { get; set; } = true;

        /// <summary>保存</summary>
        [XmlIgnore]
        public bool T_IsSave { get; set; } = false;

        /// <summary>缺陷等级</summary>
        [XmlIgnore]
        public int T_DefectLevel { get; set; } = 10000;

        [XmlIgnore]
        public string T_Path { get; set; } = "";

        [XmlIgnore]
        public string T_FileName { get; set; } = "";

        [XmlIgnore]
        public string T_FileFormat { get; set; } = "";
        
        /// <summary>
        /// 释放该缺陷结果持有的所有 HALCON 运行期对象。
        /// 只释放本结果内部 CopyObj 出来的对象，不释放插件输入图 Params.ho_Image。
        /// 特征计算用的预处理整图由外层 DetectResult.PreprocessImage 或 Params.ho_PreprocessImage 统一持有，
        /// 单个缺陷结果不再复制整图，避免候选数量多时按缺陷数放大内存。
        /// </summary>
        public void Dispose()
        {
            Region?.Dispose();
            Region = null;
            CropImage?.Dispose();
            CropImage = null;
            DarkRegion?.Dispose();
            DarkRegion = null;
            LightRegion?.Dispose();
            LightRegion = null;
            InvalidRegion?.Dispose();
            InvalidRegion = null;
        }

        /// <summary>
        /// 释放该缺陷结果持有的所有 HALCON 运行期对象。
        /// 只释放本结果内部 CopyObj 出来的对象，不释放插件输入图 Params.ho_Image。
        /// 特征计算用的预处理整图由外层 DetectResult.PreprocessImage 或 Params.ho_PreprocessImage 统一持有，
        /// 单个缺陷结果不再复制整图，避免候选数量多时按缺陷数放大内存。
        /// </summary>
        public void DisposePortion()
        {
            Region = null;
            CropImage = null;
            DarkRegion?.Dispose();
            DarkRegion = null;
            LightRegion?.Dispose();
            LightRegion = null;
            InvalidRegion?.Dispose();
            InvalidRegion = null;
        }
    }
}

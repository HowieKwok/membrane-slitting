using FrameCommunity.公共类;
using System;
using System.ComponentModel;
using System.Xml.Serialization;


namespace Frame_Core.pluginParams
{


    /// <summary>
    /// 涂布缺陷 ROI 参数。该类后续迁移到 Frame_Core\pluginParams，必须保持纯参数类，不继承任何插件 MVVM 基类。
    /// </summary>
    [Serializable]
    [XmlType("CoatRoiParam")]
    public class CoatRoiParam
    {
        /// <summary>是否启用该 ROI。屏蔽 ROI 可单独关闭；检测 ROI 通常由 ParamsDefectCoat.EnableLocalRoi 控制。</summary>
        [Description("是否启用该ROI")]
        public bool Enabled { get; set; } = true;

        /// <summary>ROI 序号。屏蔽 ROI 列表显示和保存时使用。</summary>
        [Description("ROI序号")]
        public int Index { get; set; }

        /// <summary>ROI 名称。用于界面列表和调试日志。</summary>
        [Description("ROI名称")]
        public string Name { get; set; } = "屏蔽ROI";

        /// <summary>ROI 形状。检测主 ROI 当前固定为 Rectangle1。</summary>
        [Description("ROI形状")]
        public RoiShapeType Shape { get; set; } = RoiShapeType.Rectangle1;



        private double _row1 = 0;
        /// <summary>Rectangle1 左上/右下行坐标之一，单位 px。运行前会归一化为 min(Row1,Row2)。</summary>
        [Description("左上/右下行坐标(px)")]
        public double Row1
        {
            get { 
                return _row1 ; 
                 }
            set
            {
                _row1 = value + TopOffsetRow;
            }
        }
        private double _col1 = 0;
        /// <summary>Rectangle1 左上/右下列坐标之一，单位 px。运行前会归一化为 min(Col1,Col2)。</summary>
        [Description("左上/右下列坐标(px)")]
        public double Col1
        {
            get
            {
                return _col1;
            }
            set
            {
                _col1 = value + LeftOffsetCol;
            }
        }

        private double _row2 = 0;
        /// <summary>Rectangle1 左上/右下行坐标之一，单位 px。运行前会归一化为 max(Row1,Row2)。</summary>
        [Description("右下/左上行坐标(px)")]
        public double Row2
        {
            get
            {
                return _row2;
            }
            set
            {
                _row2 = value + BottomOffsetRow;
            }
        }
        private double _col2 = 0;
        /// <summary>Rectangle1 左上/右下列坐标之一，单位 px。运行前会归一化为 max(Col1,Col2)。</summary>
        [Description("右下/左上列坐标(px)")]
        public double Col2
        {
            get
            {
                return _col2;
            }
            set
            {
                _col2 = value + RightOffsetCol;
            }
        }

        /// <summary>
        /// 上偏移(Row)
        /// </summary>
        [Description("上偏移(Row)")]
        public double TopOffsetRow { get; set; } = 0;
        /// <summary>
        /// 下偏移(Row)
        /// </summary>
        [Description("下偏移(Row)")]
        public double BottomOffsetRow { get; set; } = 0;
        /// <summary>
        /// 左偏移(Col)
        /// </summary>
        [Description("左偏移(Col)")]
        public double LeftOffsetCol { get; set; } = 0;
        /// <summary>
        /// 右偏移(Col)
        /// </summary>
        [Description("右偏移(Col)")]
        public double RightOffsetCol { get; set; } = 0;


        /// <summary>ROI 中心行坐标，单位 px。由 Row1/Row2 或 ViewROI 同步得到。</summary>
        [Description("ROI中心行坐标(px)")]
        public double CenterRow { get; set; }

        /// <summary>ROI 中心列坐标，单位 px。由 Col1/Col2 或 ViewROI 同步得到。</summary>
        [Description("ROI中心列坐标(px)")]
        public double CenterCol { get; set; }

        /// <summary>ROI 角度。Rectangle2 使用弧度；Rectangle1 不直接使用该值。</summary>
        [Description("ROI角度(弧度)")]
        public double Angle { get; set; }

        /// <summary>Rectangle2 半高或 Rectangle1 派生半高，单位 px。</summary>
        [Description("ROI半高/半长(px)")]
        public double Length1 { get; set; } = 50;

        /// <summary>Rectangle2 半宽或 Rectangle1 派生半宽，单位 px。</summary>
        [Description("ROI半宽(px)")]
        public double Length2 { get; set; } = 20;

        /// <summary>圆形 ROI 半径，单位 px。当前检测主流程暂不使用。</summary>
        [Description("圆形ROI半径(px)")]
        public double Radius { get; set; } = 30;

        /// <summary>界面显示角度。Rectangle2 从弧度转度；Rectangle1 直接显示保存值。</summary>
        [Description("界面显示角度(度)")]
        [XmlIgnore]
        public double AngleDeg => Shape == RoiShapeType.Rectangle2 ? Angle * 180.0 / Math.PI : Angle;

        /// <summary>ROI 宽度，单位 px。</summary>
        [Description("ROI宽度(px)")]
        [XmlIgnore]
        public double Width => Length2 * 2.0;

        /// <summary>ROI 高度，单位 px。</summary>
        [Description("ROI高度(px)")]
        [XmlIgnore]
        public double Height => Length1 * 2.0;

        /// <summary>浅拷贝 ROI 参数。只复制数值字段，不复制运行态 HALCON 对象。</summary>
        public CoatRoiParam Clone()
        {
            return (CoatRoiParam)MemberwiseClone();
        }
    }
}
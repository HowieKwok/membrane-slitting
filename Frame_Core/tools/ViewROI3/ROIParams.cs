using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ViewROI3
{
    [Serializable]
    public class ROIParams
    {
        [Description("中心行坐标")]
        public double midR { get; set; } = 50;
        [Description("中心列坐标")]
        public double midC { get; set; } = 50;
        [Description("半径")]
        public double radius { get; set; } = 10;
        [Description("行坐标R1")]
        public double row1 { get; set; } = 25;
        [Description("列坐标C1")]
        public double col1 { get; set; } = 25;
        [Description("行坐标R2")]
        public double row2 { get; set; } = 75;
        [Description("列坐标C2")]
        public double col2 { get; set; } = 75;

        [Description("Length1")]
        public double length1 { get; set; } = 75;

        [Description("Length2")]
        public double length2 { get; set; } = 75;

        [Description("矩形2角度")]
        public double phi { get; set; } = -Math.PI / 2;

        [Description("矩形2扫描点数")]
        public int pointnum { get; set; } = 10;
        [Description("ROI类型")]

        public ROIType rOItype = ROIType.圆;

        [Description("ROI测量使能")]

        public bool MeasureEnable = false;

        [Description("ROI索引")]
        public int roiIndex = -1;

        //[NonSerialized]
        //[Description("控件字典")]
        //public Dictionary<string, object> ControlValues = new Dictionary<string, object>();

    }
}

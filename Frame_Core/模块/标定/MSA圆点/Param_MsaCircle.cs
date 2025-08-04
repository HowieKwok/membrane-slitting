using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YYSD.UICore.Control;

namespace Frame_Core
{
    [Serializable]
    public  class Param_MsaCircle : XmlSerializationOperation
    {

        /// <summary>
        /// ROI_Y中心X//输入变量
        /// </summary>
        [ShowTextAttribute("ROI_Y中心X")]
        [Description("ROI_Y中心X")]
        public double fROI_YCenterX { set; get; } = 0.00;

        /// <summary>
        /// ROI_Y中心Y//输入变量
        /// </summary>
        [ShowTextAttribute("ROI_Y中心Y")]
        [Description("ROI_Y中心Y")]
        public double fROI_YCenterY { set; get; } = 0.00;

        /// <summary>
        /// ROI_Y中心R//输入变量
        /// </summary>
        [ShowTextAttribute("ROI_Y中心R")]
        [Description("ROI_Y中心R")]
        public double fROI_YCenterR { set; get; } = 0.00;


        /// <summary>
        /// ROI_Length1//输入变量
        /// </summary>
        [ShowTextAttribute("ROI_YLength1")]
        [Description("ROI_YLength1")]
        public double fROI_YLenght1 { set; get; } = 0.00;

        /// <summary>
        /// ROI_Length2//输入变量
        /// </summary>
        [ShowTextAttribute("ROI_YLength2")]
        [Description("ROI_YLength2")]
        public double fROI_YLenght2 { set; get; } = 0.00;


        /// <summary>
        /// ROI_X中心X//输入变量
        /// </summary>
        [ShowTextAttribute("ROI_X中心X")]
        [Description("ROI_X中心X")]
        public double fROI_XCenterX { set; get; } = 0.00;

        /// <summary>
        /// ROI_Y中心Y//输入变量
        /// </summary>
        [ShowTextAttribute("ROI_X中心Y")]
        [Description("ROI_X中心Y")]
        public double fROI_XCenterY { set; get; } = 0.00;

        /// <summary>
        /// ROI_Y中心R//输入变量
        /// </summary>
        [ShowTextAttribute("ROI_X中心R")]
        [Description("ROI_X中心R")]
        public double fROI_XCenterR { set; get; } = 0.00;


        /// <summary>
        /// ROI_Length1//输入变量
        /// </summary>
        [ShowTextAttribute("ROI_XLength1")]
        [Description("ROI_XLength1")]
        public double fROI_XLenght1 { set; get; } = 0.00;

        /// <summary>
        /// ROI_Length2//输入变量
        /// </summary>
        [ShowTextAttribute("ROI_XLength2")]
        [Description("ROI_XLength2")]
        public double fROI_XLenght2 { set; get; } = 0.00;

        /// <summary>
        /// 阈值范围最小值//输入变量
        /// </summary>
        [ShowTextAttribute("阈值范围最小值")]
        [Description("阈值范围最小值")]
        public int Ithreoldmin { set; get; } = 0;

        /// <summary>
        /// 阈值范围最大值//输入变量
        /// </summary>
        [ShowTextAttribute("阈值范围最大值")]
        [Description("阈值范围最大值")]
        public int Ithreoldmax { set; get; } = 80;


        /// <summary>
        /// 宽度范围最小值//输入变量
        /// </summary>
        [ShowTextAttribute("宽度范围最小值")]
        [Description("宽度范围最小值")]
        public double fwidthmin { set; get; } = 15.00;

        /// <summary>
        /// 宽度范围最大值//输入变量
        /// </summary>
        [ShowTextAttribute("宽度范围最大值")]
        [Description("宽度范围最大值")]
        public double fwidthmax { set; get; } = 300.00;


        /// <summary>
        /// 高度范围最小值//输入变量
        /// </summary>
        [ShowTextAttribute("高度范围最小值")]
        [Description("高度范围最小值")]
        public double fHeightMin { set; get; } = 15.00;


        /// <summary>
        /// 高度范围最大值//输入变量
        /// </summary>
        [ShowTextAttribute("高度范围最大值")]
        [Description("高度范围最大值")]
        public double fHeightMax { set; get; } = 400.00;

        /// <summary>
        /// 面积范围最大值//输入变量
        /// </summary>
        [ShowTextAttribute("面积范围最小值")]
        [Description("面积范围最小值")]
        public double fAreaMin{ set; get; } = 2000.00;

        /// <summary>
        /// 面积范围最大值//输入变量
        /// </summary>
        [ShowTextAttribute("面积范围最大值")]
        [Description("面积范围最大值")]
        public double fAreaMax { set; get; } = 9999.00;


        /// <summary>
        /// Y方向个数//输入变量
        /// </summary>
        [ShowTextAttribute("Y方向个数")]
        [Description("Y方向个数")]
        public int IYDectectNum { set; get; } = 1;

        /// <summary>
        /// X方向个数//输入变量
        /// </summary>
        [ShowTextAttribute("X方向个数")]
        [Description("X方向个数")]
        public int IXDectectNum { set; get; } = 1;

        /// <summary>
        /// 启用模板//输入变量
        /// </summary>
        [ShowTextAttribute("启用模板")]
        [Description("启用模板")]
        public bool bIsEnabelMobel { set; get; } = false;

        /// <summary>
        /// Y方向圆点坐标//输入变量
        /// </summary>
        [ShowTextAttribute("Y方向圆点坐标")]
        [Description("Y方向圆点坐标")]
        public List<PointRotateInfo> pointYRotateInfos   { set; get; } 

        /// <summary>
        /// X方向圆点坐标//输入变量
        /// </summary>
        [ShowTextAttribute("X方向圆点坐标")]
        [Description("X方向圆点坐标")]
        public List<PointRotateInfo> pointXRotateInfos { set; get; } 


    }
}

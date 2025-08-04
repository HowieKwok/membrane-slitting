using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frame_Core;
using HalconDotNet;
using YYSD.UICore.Control;

namespace Plugin.MeasureEar
{
    [Serializable]
    public class ParamsMeasureEar : PluginParamsBase
    {
        //输入变量
        /// <summary>
        /// 输入变量：EA原图
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject ho_Image;

        /// <summary>
        /// 输入变量：抓边参数队列-预设9条边
        /// </summary>
        [Description("抓边类数组")]
        public ParamsMeasureLine[] array_MeasureLineParam;

        /// <summary>
        /// 输入变量：是否为背光图像
        /// </summary>
        // public bool IsBackImage = false;
        [Description("是否为背光")]
        public bool IsBackImage  = false;

        /// <summary>
        /// 输入变量：极耳中心坐标Col_X
        /// </summary>
        [ShowTextAttribute("极耳中心位置列坐标")]
        [Description("极耳中心位置列")]
        public double JRCenterCol  = 6000;


        /// <summary>
        /// 输入变量：极耳边缘选择
        /// </summary>
        [Description("极耳边缘选择")]
        public JRLineEnum JRLine;


        /// <summary>
        /// 输入变量：是否为阴极
        /// </summary>
        [Description("极耳边缘选择")]
        public bool IsCathode = false;

        /// <summary>
        /// 输入变量：是否单线调试模式
        /// </summary>
        [ShowTextAttribute("是否单线调试模式1")]
        [Description("是否单线调试模式")]
        public bool IsFitOneLine = false;

        ///// <summary>
        ///// 输入变量：是否为左侧极耳
        ///// </summary>
        //public bool IsLeftJR = false;


        /// <summary>
        /// 输入变量：横X像素当量
        /// </summary>
        [Description("横X像素当量")]
        public double Pix_X = 0.03;
        /// <summary>
        /// 输入变量：纵Y像素当量
        /// </summary>
        [Description("纵Y像素当量")]
        public double Pix_Y = 0.04;

        /// <summary>
        /// 输入变量：极耳中心坐标Row_Y
        /// </summary>
        [Description("极耳中心坐标Row_Y")]
        public double JRCenterRow = 100;


        /// <summary>
        /// 输入变量：极耳裁切左偏移Col_X
        /// </summary>
        [Description("极耳裁切左偏移Col_X")]
        public double JRShaft_Left = 1000;
        /// <summary>
        /// 输入变量：极耳裁切右偏移Col_X
        /// </summary>
        [Description("极耳裁切右偏移Col_X")]
        public double JRShaft_Right = 1000;

        /// <summary>
        /// 输入变量：R角检测偏移X
        /// </summary>
        [Description("R角检测偏移X")]
        public double AngleShaftX = 50;
        /// <summary>
        /// 输入变量：R角检测偏移Y
        /// </summary>
        [Description("R角检测偏移Y")]
        public double AngleShaftY = 50;

        //Mark检测参数
        /// <summary>
        /// 上Mark检测启用
        /// </summary>
        [Description("上Mark检测启用")]
        public bool Mark1Enable = true;
        /// <summary>
        /// 下Mark检测启用
        /// </summary>
        [Description("下Mark检测启用")]
        public bool Mark2Enable = true;

        /// <summary>
        /// 上Mark距离极耳中心行偏移
        /// </summary>
        [Description("上Mark行偏移")]
        public double Mark1ShaftRow = 200;
        /// <summary>
        /// 下Mark距离极耳中心行偏移
        /// </summary>
        [Description("上Mark行偏移")]
        public double Mark2ShaftRow = 200;

        /// <summary>
        /// 识别Mark灰度min
        /// </summary>
        [Description("识别Mark灰度min")]
        public int MarkGrayMin = 0;
        /// <summary>
        /// 识别Mark灰度max
        /// </summary>
        [Description("识别Mark灰度max")]
        public int MarkGrayMax = 100;

        /// <summary>
        /// Mark测量区域X
        /// </summary>
        [Description("Mark测量区域X")]
        public int MarkRegionX = 100;
        /// <summary>
        /// Mark测量区域Y
        /// </summary>
        [Description("Mark测量区域Y")]
        public int MarkRegionY = 100;




        //输出变量
        /// <summary>
        /// 输出变量：Crop极耳图
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject ho_JRImage;
        /// <summary>
        /// 输出变量：Zoom极耳图
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject ho_ZoomJRImage;

        /// <summary>
        /// 输出变量：极耳R1角XLD
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject ho_R1_XLD;
        /// <summary>
        /// 输出变量：极耳R1角XLD
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject ho_R2_XLD;


        /// <summary>
        /// 输出变量：上mark1的测量框Region
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject ho_MarkregionRect1;
        /// <summary>
        /// 输出变量：下mark2的测量框Region
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject ho_MarkregionRect2;
        /// <summary>
        /// 输出变量：上mark1的Region
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject ho_Markregion1;
        /// <summary>
        /// 输出变量：下mark2的Region
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject ho_Markregion2;


        /// <summary>
        /// 输出变量：极耳R角半径
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple R1Radius;

        /// <summary>
        /// 输出变量：极耳R1角拟合圆心Row
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple R1Row;
        /// <summary>
        /// 输出变量：极耳R1角拟合圆心Col
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple R1Col;

        /// <summary>
        /// 输出变量：极耳R2角半径
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple R2Radius;
        /// <summary>
        /// 输出变量：极耳R2角拟合圆心Row
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple R2Row;
        /// <summary>
        /// 输出变量：极耳R2角拟合圆心Col
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple R2Col;

        /// <summary>
        /// 尺寸测量词典
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public Dictionary<SizeEnum, double> dic_SizeItem = new Dictionary<SizeEnum, double>();

        /// <summary>
        /// 输出变量：极耳底边上切点row
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple hv_PointRow0 = null;

        /// <summary>
        /// 输出变量：极耳底边上切点col
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple hv_PointCol0 = null;

        /// <summary>
        /// 输出变量：极耳底边下切点row
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple hv_PointRow1 = null;

        /// <summary>
        /// 输出变量：极耳底边下切点col
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple hv_PointCol1 = null;
        /// <summary>
        /// 输出变量：极耳顶边上切点row
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple hv_PointRow2 = null;

        /// <summary>
        /// 输出变量：极耳顶边上切点col
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple hv_PointCol2 = null;
        /// <summary>
        /// 输出变量：极耳顶边下切点row
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple hv_PointRow3 = null;

        /// <summary>
        /// 输出变量：极耳顶边下切点col
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple hv_PointCol3 = null;

        /// <summary>
        /// 输出变量：极耳中心偏差，（精准值-传入值） = 偏差
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple hv_MidOffset = 0;

       

        public ParamsMeasureEar()
        {
            HOperatorSet.GenEmptyObj(out ho_Image);
            HOperatorSet.GenEmptyObj(out ho_JRImage);
            HOperatorSet.GenEmptyObj(out ho_ZoomJRImage);
            HOperatorSet.GenEmptyObj(out ho_R1_XLD);
            HOperatorSet.GenEmptyObj(out ho_R2_XLD);
            HOperatorSet.GenEmptyObj(out ho_MarkregionRect1);
            HOperatorSet.GenEmptyObj(out ho_MarkregionRect2);
            HOperatorSet.GenEmptyObj(out ho_Markregion1);
            HOperatorSet.GenEmptyObj(out ho_Markregion2);


            array_MeasureLineParam = new ParamsMeasureLine[Enum.GetNames(typeof(JRLineEnum)).Length];
            foreach (JRLineEnum item in Enum.GetValues(typeof(JRLineEnum)))
            {
                array_MeasureLineParam[(int)item] = new ParamsMeasureLine();
                array_MeasureLineParam[(int)item].str_ModelName = item.ToString();
            }


            foreach (SizeEnum item in Enum.GetValues(typeof(SizeEnum)))
            {
                if (item == SizeEnum.极耳顶宽Y || item == SizeEnum.极耳底宽Y || item == SizeEnum.极耳高度 || item == SizeEnum.极耳台阶高 ||
                    item == SizeEnum.极耳上升沿角度 || item == SizeEnum.极耳下降沿角度 || item == SizeEnum.顶R1半径 || item == SizeEnum.底R2半径 ||
                    item == SizeEnum.上V角宽度 || item == SizeEnum.上V角深度 || item == SizeEnum.下V角宽度 || item == SizeEnum.下V角深度)  
                {
                    dic_SizeItem.Add(item, -999);
                }
            }
        }
        public void INiInputParams()
        {
            if (array_MeasureLineParam == null)
            {
                foreach (JRLineEnum item in Enum.GetValues(typeof(JRLineEnum)))
                {
                    ParamsMeasureLine pm = new ParamsMeasureLine();
                    pm.str_ModelName = item.ToString();
                    // array_MeasureLineParam.Add(pm);
                }
            }
        }


        /// <summary>
        /// 初始化输出变量
        /// </summary>
        public void INiOutputParams()
        {
            ho_JRImage.Dispose();
            ho_ZoomJRImage.Dispose();
            ho_R1_XLD.Dispose();
            ho_R2_XLD.Dispose();
            ho_MarkregionRect1.Dispose();
            ho_MarkregionRect2.Dispose();
            ho_Markregion1.Dispose();
            ho_Markregion2.Dispose();

            HOperatorSet.GenEmptyObj(out ho_MarkregionRect1);
            HOperatorSet.GenEmptyObj(out ho_MarkregionRect2);
            HOperatorSet.GenEmptyObj(out ho_Markregion1);
            HOperatorSet.GenEmptyObj(out ho_Markregion2);
            HOperatorSet.GenEmptyObj(out ho_R1_XLD);
            HOperatorSet.GenEmptyObj(out ho_R2_XLD);
            HOperatorSet.GenEmptyObj(out ho_JRImage);
            HOperatorSet.GenEmptyObj(out ho_ZoomJRImage);

            R1Radius = 0;
            R1Row = 0;
            R1Col = 0;
            R2Radius = 0;
            R2Row = 0;
            R2Col = 0;


            foreach (SizeEnum item in Enum.GetValues(typeof(SizeEnum)))
            {
                if (item == SizeEnum.极耳顶宽Y || item == SizeEnum.极耳底宽Y || item == SizeEnum.极耳高度 || item == SizeEnum.极耳台阶高 ||
                    item == SizeEnum.极耳上升沿角度 || item == SizeEnum.极耳下降沿角度 || item == SizeEnum.顶R1半径 || item == SizeEnum.底R2半径 ||
                    item == SizeEnum.上V角宽度 || item == SizeEnum.上V角深度 || item == SizeEnum.下V角宽度 || item == SizeEnum.下V角深度)
                {
                    dic_SizeItem[item] = -999;
                }
            }
            hv_PointRow0 = 0;
            hv_PointCol0 = 0;

            hv_PointRow1 = 0;
            hv_PointCol1 = 0;

            hv_PointRow2 = 0;
            hv_PointCol2 = 0;

            hv_PointRow3 = 0;
            hv_PointCol3 = 0;
        }
    }
}

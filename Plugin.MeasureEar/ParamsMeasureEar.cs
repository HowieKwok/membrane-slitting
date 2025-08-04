using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frame_Core;
using HalconDotNet;

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
        /// 输入变量：抓边参数队列-预设7条边
        /// </summary>
        //public ParamsMeasureLine MeasureLineParam0 = new ParamsMeasureLine();    //极耳底边1,
        //public ParamsMeasureLine MeasureLineParam1 = new ParamsMeasureLine();    //极耳底边2,
        //public ParamsMeasureLine MeasureLineParam2 = new ParamsMeasureLine();    //极耳斜边1,
        //public ParamsMeasureLine MeasureLineParam3 = new ParamsMeasureLine();    //极耳斜边2,
        //public ParamsMeasureLine MeasureLineParam4 = new ParamsMeasureLine();    //极耳顶边,
        //public ParamsMeasureLine MeasureLineParam5 = new ParamsMeasureLine();    //台阶边,
        //public ParamsMeasureLine MeasureLineParam6 = new ParamsMeasureLine();    //AT边,
        public ParamsMeasureLine[] array_MeasureLineParam ;  

        /// <summary>
        /// 输入变量：极耳边缘选择
        /// </summary>
        public JRLineEnum JRLine;

        /// <summary>
        /// 输入变量：是否为背光图像
        /// </summary>
        public bool IsBackImage = false;

        /// <summary>
        /// 输入变量：是否为阴极
        /// </summary>
        public bool IsCathode = false;

        /// <summary>
        /// 输入变量：是否单线调试模式
        /// </summary>
        public bool IsFitOneLine = false;

        ///// <summary>
        ///// 输入变量：是否为左侧极耳
        ///// </summary>
        //public bool IsLeftJR = false;


        /// <summary>
        /// 输入变量：横X像素当量
        /// </summary>
        public double Pix_X = 0.03;
        /// <summary>
        /// 输入变量：纵Y像素当量
        /// </summary>
        public double Pix_Y = 0.04;

        /// <summary>
        /// 输入变量：极耳中心坐标Row_Y
        /// </summary>
        public double JRCenterRow = 100;
        /// <summary>
        /// 输入变量：极耳中心坐标Col_X
        /// </summary>
        public double JRCenterCol = 6000;

        /// <summary>
        /// 输入变量：极耳裁切左偏移Col_X
        /// </summary>
        public double JRShaft_Left = 1000;
        /// <summary>
        /// 输入变量：极耳裁切右偏移Col_X
        /// </summary>
        public double JRShaft_Right = 1000;

        /// <summary>
        /// 输入变量：R角检测偏移X
        /// </summary>
        public double AngleShaftX = 50;
        /// <summary>
        /// 输入变量：R角检测偏移Y
        /// </summary>
        public double AngleShaftY = 50;



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


        public ParamsMeasureEar()
        {
            HOperatorSet.GenEmptyObj(out ho_Image);
            HOperatorSet.GenEmptyObj(out ho_JRImage);
            HOperatorSet.GenEmptyObj(out ho_ZoomJRImage);
            HOperatorSet.GenEmptyObj(out ho_R1_XLD);
            HOperatorSet.GenEmptyObj(out ho_R2_XLD);

           
            array_MeasureLineParam = new ParamsMeasureLine[Enum.GetNames(typeof(JRLineEnum)).Length];
            foreach (JRLineEnum item in Enum.GetValues(typeof(JRLineEnum)))
            {
                array_MeasureLineParam[(int)item] = new ParamsMeasureLine();
                array_MeasureLineParam[(int)item].str_ModelName = item.ToString();
            }
               

            foreach (SizeEnum item in Enum.GetValues(typeof(SizeEnum)))
            {
                if (item == SizeEnum.极耳顶宽Y || item == SizeEnum.极耳底宽Y || item == SizeEnum.极耳高度  || item == SizeEnum.极耳台阶高 ||
                    item == SizeEnum.极耳上升沿角度 || item == SizeEnum.极耳下降沿角度 || item == SizeEnum.顶R1半径 || item == SizeEnum.底R2半径)
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
                    item == SizeEnum.极耳上升沿角度 || item == SizeEnum.极耳下降沿角度 || item == SizeEnum.顶R1半径 || item == SizeEnum.底R2半径)
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

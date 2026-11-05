using Frame_Core;
using FrameCommunity;
using FrameCommunity.Data;
using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    /// <summary>
    /// 大面毛刺插件参数
    /// </summary>
    public class ParamsDaMianBurr : PluginParamsBase
    {
        #region 1. 输入参数
        /// <summary>
        /// 输入变量：原图
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public ImageInfo ImageInfo;
        /// <summary>
        /// 输入变量：屏蔽区域
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject ho_ShieldRegion;


        /// <summary>
        /// 输入变量：图像X向像素当量
        /// </summary>
        public double Pix_X = 0.002;
        /// <summary>
        /// 输入变量：图像Y向像素当量
        /// </summary>
        public double Pix_Y = 0.002;



        /// <summary>
        /// 初步定位涂布区域的最小阈值
        /// </summary>
        public int inputMinThreshold = 0;
        /// <summary>
        /// 初步定位涂布区域的最大阈值
        /// </summary>
        public int inputMaxThreshold = 100;
        /// <summary>
        /// 卡尺找大面边线的矩形区域半高
        /// </summary>
        public double inputFindLineHeight = 100;
        /// <summary>
        /// 卡尺找大面边线的矩形区域半宽
        /// </summary>
        public double inputFindLineWidth = 500;
        /// <summary>
        /// 找大面边缘毛刺第一次平移值
        /// </summary>
        public double inputFindMaoCiUP1 = 10;
        /// <summary>
        /// 找大面边缘毛刺第二次平移值
        /// </summary>
        public double inputFindMaoCiUP2 = 200;
        /// <summary>
        /// 毛刺最小阈值
        /// </summary>
        public int inputMaoCiMinThreshold = 0;
        /// <summary>
        /// 毛刺最大阈值
        /// </summary>
        public int inputMaoCiMaxThreshold = 50;

        /// <summary>
        /// 毛刺最小面积
        /// </summary>
        public double inputMinArea_DaMian = 100;
        /// <summary>
        /// 毛刺最大面积
        /// </summary>
        public double inputMaxArea = 10e+10;

        /// <summary>
        /// 毛刺最小高度
        /// </summary>
        public double DaMianMaoCiMinHeight = 0.01;

        /// <summary>
        /// 大面涂布边基准
        /// </summary>
        public double DaMianOriginValue = 1000;


        #endregion


        #region 2.输出变量

        /// <summary>
        /// 输出变量：缺陷List
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public List<NgTypeInfo> List_Defect = new List<NgTypeInfo>();


        /// <summary>
        /// 输出变量：缺陷数量
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public int DefectCount = 0;

        /// <summary>
        /// 输出变量：模块运行成功标志
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public bool bl_ModelRunFlag = false;

        /// <summary>
        /// 输出变量：图像处理后的图像
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject ho_ImageProcessed;



        /// <summary>
        /// 输出大面毛刺区域
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject outputDaMianMaoCiRegions;

        /// <summary>
        /// 输出大面边线
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject outputDaMianLine;


        /// <summary>
        /// 输出大面毛刺检测区域
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject outputDaMianMaoCiDetectionRectangle;

        /// <summary>
        /// 输出大面涂布边Y坐标值
        /// </summary>
        public double outputDaMianTuBuPointY=0;

        /// <summary>
        /// 大面毛刺高度
        /// </summary>
        public double DaMianMaoCiHeight = 0;

        /// <summary>
        /// 输出纠偏值
        /// </summary>
        public double outputCorrectionValue = 0;


        #endregion

        public ParamsDaMianBurr()
        {
            ImageInfo =new ImageInfo();
            HOperatorSet.GenEmptyObj(out ho_ShieldRegion);
            HOperatorSet.GenEmptyObj(out ho_ImageProcessed);


            HOperatorSet.GenEmptyObj(out outputDaMianMaoCiRegions);
            HOperatorSet.GenEmptyObj(out outputDaMianLine);
            HOperatorSet.GenEmptyObj(out outputDaMianMaoCiDetectionRectangle);
        }
    }
}

using Frame_Core;
using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{

    /// <summary>
    /// 端面毛刺插件参数
    /// </summary>
    public class ParamsEndBurr : PluginParamsBase
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
        /// 找线最小阈值
        /// </summary>
        public int inputFitLineMinGray = 60;
        /// <summary>
        /// 最小轮廓长度
        /// </summary>
        public double inputMinXldLength = 10;
        /// <summary>
        /// 缺陷最小过滤面积
        /// </summary>
        public double inputMinArea = 0.0001;
        /// <summary>
        /// 找缺陷最小阈值
        /// </summary>
        public int inputFitDefectMinGray = 110;
        /// <summary>
        /// 分切屏蔽线区域高度
        /// </summary>
        public double inputHeight = 0.03;

        /// <summary>
        /// 把金属箔材区域放大，用于判断B类毛刺是否与箔材连接
        /// </summary>
        public int BoCaiexpansionValue = 6;


        /// <summary>
        /// 涂层最小阈值
        /// </summary>
        public int inputTuCengMinGray = 30;

        /// <summary>
        /// 涂层最大阈值
        /// </summary>
        public int inputTuCengMaxGray = 150;

        /// <summary>
        /// B类 最小毛刺高度比例
        /// </summary>
        public double MaoCi_H = 0.45;


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
        /// 输出分切毛刺区域
        /// </summary>

        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject outputFenQieMaoCiRegions;

        /// <summary>
        /// 输出分切箔材区域
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject outputFenQieBoCaiRegions;

        /// <summary>
        /// 输出涂层区域
        /// </summary>

        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject outputTuCengRegion;




        #endregion

        public ParamsEndBurr()
        {
            ImageInfo = new ImageInfo();
            HOperatorSet.GenEmptyObj(out ho_ShieldRegion);
            HOperatorSet.GenEmptyObj(out ho_ImageProcessed);


            HOperatorSet.GenEmptyObj(out outputFenQieMaoCiRegions);
            HOperatorSet.GenEmptyObj(out outputFenQieBoCaiRegions);
            HOperatorSet.GenEmptyObj(out outputTuCengRegion);
        }
    }
}

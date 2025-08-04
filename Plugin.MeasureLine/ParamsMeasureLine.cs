using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frame_Core;
using HalconDotNet;

namespace Plugin.MeasureLine
{
    /// <summary>
    /// 直线测量参数类
    /// </summary>
    [Serializable]
    public class SSS_ParamsMeasureLine :PluginParamsBase
    {
        #region 1. 输入参数
        /// <summary>
        /// 输入变量：原图
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject ho_Image;

        public int ImageSourceIndex = 0;
        /// <summary>
        /// 输入变量：源数据导入模块
        /// </summary>
        public string ImageSourceModule = "default";

        public string  ImageSourceName = "default";
        /// <summary>
        /// 输入变量：直线名称
        /// </summary>
        public string str_ModelName = "未命名";

        /// <summary>
        /// 输入变量：Roi密度：
        /// </summary>
        public int hv_RoiPointsNum = 20;

        /// <summary>
        /// 输入变量：抓边ROI中点行坐标
        /// </summary>
        public double hv_RoiMidRow = 300;

        /// <summary>
        /// 输入变量：抓边ROI中点列坐标
        /// </summary>
        public double hv_RoiMidColumn = 300;

        /// <summary>
        /// 输入变量：抓边ROI旋转角度
        /// </summary>
        public double hv_RoiAngle = -90;

        /// <summary>
        /// 输入变量：抓边ROI高度
        /// </summary>
        public double hv_RoiHeight = 200;

        /// <summary>
        /// 输入变量：抓边ROI宽度
        /// </summary>
        public double hv_RoiWidth = 200;


        /// <summary>
        /// 输入变量：ROI运动方式
        /// </summary>
        public string str_RoiMoveIndex = "StaticROI";

        /// <summary>
        /// 输入变量：ROI基准行坐标
        /// </summary>
        public double hv_RoiStandardRow = 0;

        /// <summary>
        /// 输入变量：ROI基准列坐标
        /// </summary>
        public double hv_RoiStandardColumn = 0;

        /// <summary>
        /// 输入变量：ROI基准行偏移量
        /// </summary>
        public double hv_RoiStandardOffsetRow = 0;

        /// <summary>
        /// 输入变量：ROI基准列偏移量
        /// </summary>
        public double hv_RoiStandardOffsetColumn = 0;

        /// <summary>
        /// 输入变量：ROI递增次数
        /// </summary>
        public int hv_RoiIncreaseNum = 0;

        /// <summary>
        /// 输入变量：ROI行增量
        /// </summary>
        public int hv_RoiIncreaseRow = 0;

        /// <summary>
        /// 输入变量：ROI列增量
        /// </summary>
        public int hv_RoiIncreaseColumn = 0;


        /// <summary>
        /// 输入变量：是否进行图像预处理
        /// </summary>
        public bool bl_ImageProcessIndex = false;

        /// <summary>
        /// 输入变量：对比度增强-底色选取
        /// </summary>
        public int hv_ScaleMean = 255;

        /// <summary>
        /// 输入变量：对比度增强-增强因子
        /// </summary>
        public double hv_ScaleFactor = 0.0;

        /// <summary>
        /// 输入变量：图像锐化迭代次数
        /// </summary>
        public int hv_SharpenIterations = 0;

        /// <summary>
        /// 输入变量：图像锐化平滑系数
        /// </summary>
        public double hv_SharpenSigma = 0.5;


        /// <summary>
        /// 输入变量：抓边检测直线模型平滑系数
        /// </summary>
        public double hv_MeasureSigma = 3.0;

        /// <summary>
        /// 输入变量：抓边检测直线模型灰度阈值
        /// </summary>
        public int hv_MeasureThreshold = 20;

        /// <summary>
        /// 输入变量：直线拟合数量：默认值： 1 值的列表： 1，2，3，4
        /// </summary>
        public int hv_MeasureNumInstances = 1;

        /// <summary>
        /// 输入变量：距离剔除：默认值：3.5  值的列表： 0，1.0，2.0， 3.5，5.0
        /// </summary>
        public double hv_MeasureDistThreshold = 4;

        /// <summary>
        /// 输入变量：明暗方向：值列表： 'all'，'negative'， 'positive'，'uniform'
        /// </summary>
        public string hv_MeasureTransition = "positive";

        /// <summary>
        /// 输入变量：边缘选取：'all'，'first'，'last'
        /// </summary>
        public string hv_MeasureSelect = "first";

        /// <summary>
        /// 输入变量：最小得分：'min_score'
        /// </summary>
        public double hv_MeasureMinScore = 0.5;

        /// <summary>
        /// 输入变量：迭代次数: 默认值：-1  值的列表： 10，100，1000
        /// </summary>
        public int hv_MaxNumIterations = -1;

        /// <summary>
        /// 输入变量：插值类型：nearest_neighbor'灰度值是从最近像素的灰度值获得；'bilinear'，使用双线性插值 ；'bicubic'，则使用双三次插值。
        /// </summary>
        public string hv_MeasureInterpolation = "nearest_neighbor";

        /// <summary>
        /// 输入变量：指定对测量结果的验证：默认值： “ false”  值列表： 'true'，'false'
        /// </summary>
        public string hv_MeasureIORegions = "true";
        #endregion


        #region 2.输出变量
        /// <summary>
        /// 输出变量：处理后的图像
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject ho_ImageProcessed;

        /// <summary>
        /// 输出变量：抓边检测直线模型
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple hv_MetrologyHandle = null;

        /// <summary>
        /// 输出变量：抓边检测直线模型索引
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple hv_Index = null;

        /// <summary>
        /// 输出变量：抓边检测拟合点行坐标
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple hv_CrossRow = null;

        /// <summary>
        /// 输出变量：抓边检测拟合点列坐标
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple hv_CrossColumn = null;

        /// <summary>
        /// 输出变量：抓边检测直线起始行坐标
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple hv_LineRowBegin = null;

        /// <summary>
        /// 输出变量：抓边检测直线起始列坐标
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple hv_LineColumnBegin = null;

        /// <summary>
        /// 输出变量：抓边检测直线终止行坐标
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple hv_LineRowEnd = null;

        /// <summary>
        /// 输出变量：抓边检测直线终止列坐标
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple hv_LineColumnEnd = null;

        /// <summary>
        /// 输出变量：模块运行成功标志
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public bool bl_FitSuccess = false;
        #endregion
        public SSS_ParamsMeasureLine()
        {
            HOperatorSet.GenEmptyObj(out ho_Image);
            HOperatorSet.GenEmptyObj(out ho_ImageProcessed);
        }

    }

}

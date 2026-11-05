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
    /// 直线测量参数类
    /// </summary>
    [Serializable]
    public class ParamsFindHead : PluginParamsBase
    {

        #region 1. 输入参数

        /// <summary>
        /// 找料头还是找料尾 true：找料头，false：找料尾
        /// </summary>
        public bool Is_FindHeadOrTrail = true;

        /// <summary>
        /// 输入变量：相机图像队列，图片与ID绑定
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public List<ImageInfo> list_ImageInfo = new List<ImageInfo>();
        /// <summary>
        /// 输入变量：检测图像的ID，
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public int ImageNowID = 1;
        /// <summary>
        /// 是否主动找料头，true：主动查图找料头，false：接收传入料头值
        /// </summary>
        public bool Is_ActiveFindHead = true;
        /// <summary>
        /// 是否裁切料头小图
        /// </summary>
        public bool Is_CropHeadimg = false;

        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        /// <summary>
        /// 测试料头
        /// </summary>
        public bool Is_TestFindHead = false;

        #region 抓料头边
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
        public double hv_RoiMidRow = 200;

        /// <summary>
        /// 输入变量：抓边ROI中点列坐标
        /// </summary>
        public double hv_RoiMidColumn = 200;

        /// <summary>
        /// 输入变量：抓边ROI旋转角度
        /// </summary>
        public double hv_RoiAngle = 0;

        /// <summary>
        /// 输入变量：抓边ROI高度
        /// </summary>
        public double hv_RoiHeight = 200;

        /// <summary>
        /// 输入变量：抓边ROI宽度
        /// </summary>
        public double hv_RoiWidth = 200;


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
        public string hv_MeasureTransition = "negative";

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
        #region 区域法找料头区域
        /// <summary>
        /// 找料头灰度阈值
        /// </summary>
        public int hv_RegionThreshold = 200;

        /// <summary>
        /// 找料头ROI个数，个数越多，查找越细致 ,必须大于等于1
        /// </summary>
        public int hv_RegionNum = 20;

        /// <summary>
        /// 找料头ROI起始列，只用在涂布区查找
        /// </summary>
        public double hv_ColumnStart = 3000;

        /// <summary>
        /// 找料头ROI起始列，只用在涂布区查找
        /// </summary>
        public double hv_ColumnEnd = 6000;

        /// <summary>
        /// 输出变量：料头被图像裁断的图像ID缓存//开辟缓存图像队列，用于料头在边缘的拼图。。
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public int Cache_imageID =-1;


        #endregion

        #region 料头预处理参数
        /// <summary>
        /// 料头预处理
        /// </summary>
        public bool EnStubBar = false;
        /// <summary>
        /// 料头边留白区灰度
        /// </summary>
        public int WhiteRegionGray = 150;
        /// <summary>
        /// 料头边涂膜区灰度
        /// </summary>
        public int CoatingRegionGary = 70;
        #endregion

        #endregion


        #region 2.输出变量

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
        public bool bl_ModelRunFlag = false;

        /// <summary>
        /// 输出变量：留白检测结果：0是料区，1是留白
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple hv_GrayResult = null;

        /// <summary>
        /// 输出变量：留白检测灰度均值 
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple hv_GrayMean = null;


        /// <summary>
        /// 输出变量：料头行数
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public double headLine;

        /// <summary>
        /// 输出变量：料头图片ID
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public int headLineID;

        /// <summary>
        /// 输出变量：抓边图像
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject ho_ImageMeasureLine;

        /// <summary>
        /// 输出变量：料头图像
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject ho_ImageHead;    
        /// <summary>
        /// 输出变量：抓料头图像裁图起点坐标ID
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple hv_ImageHeadBeginID = 0;

        /// <summary>
        /// 输出变量：抓料头图像裁图起点坐标行
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple hv_ImageHeadBeginRow = 0;

        /// <summary>
        /// 输出变量：抓料头图像裁图起点坐标列
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple hv_ImageHeadBeginCol = 0;

        /// <summary>
        /// 输出变量：抓料头图像裁图终点坐标ID
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple hv_ImageHeadEndID = 0;

        /// <summary>
        /// 输出变量：抓料头图像裁图终点坐标行
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple hv_ImageHeadEndRow = 0;

        /// <summary>
        /// 输出变量：抓料头图像裁图终点坐标列
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple hv_ImageHeadEndCol = 0;

        /// <summary>
        /// 输出变量：图片中是否包含料头。
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public bool bl_ContainHeadLine;
        #endregion
        public ParamsFindHead()
        {
            headLine = -1;
            headLineID = -1;
            HOperatorSet.GenEmptyObj(out ho_ImageHead);         
            HOperatorSet.GenEmptyObj(out ho_ImageMeasureLine);
            //headLine = 0;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frame_Core;
using HalconDotNet;

namespace Frame_Core
{ 
    /// <summary>
    /// 两条抓边找交点参数类
    /// </summary>
    [Serializable]
    public class ParamsFindIntersectionAngle : PluginParamsBase
    {
        #region 1. 输入参数 - 公共部分

        /// <summary>
        /// 输入变量：原图
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject ho_Image;

        /// <summary>
        /// 图像来源索引：0=默认 1=指定模块
        /// </summary>
        public int ImageSourceIndex = 0;

        /// <summary>
        /// 输入变量：源数据导入模块
        /// </summary>
        public string ImageSourceModule = "default";

        /// <summary>
        /// 输入变量：源数据导入名称
        /// </summary>
        public string ImageSourceName = "default";

        /// <summary>
        /// 输入变量：工具名称
        /// </summary>
        public string str_ModelName = "未命名";

        #endregion

        #region 2. 输入参数 - 直线1 ROI

        /// <summary>
        /// 直线1：ROI中点行坐标
        /// </summary>
        public double hv_Line1_RoiMidRow = 200;

        /// <summary>
        /// 直线1：ROI中点列坐标
        /// </summary>
        public double hv_Line1_RoiMidColumn = 300;

        /// <summary>
        /// 直线1：ROI旋转角度(度)
        /// </summary>
        public double hv_Line1_RoiAngle = 0;

        /// <summary>
        /// 直线1：ROI高度
        /// </summary>
        public double hv_Line1_RoiHeight = 200;

        /// <summary>
        /// 直线1：ROI宽度
        /// </summary>
        public double hv_Line1_RoiWidth = 100;

        /// <summary>
        /// 直线1：ROI采样密度
        /// </summary>
        public int hv_Line1_RoiPointsNum = 20;

        /// <summary>
        /// 直线1：平滑系数
        /// </summary>
        public double hv_Line1_MeasureSigma = 3.0;

        /// <summary>
        /// 直线1：灰度阈值
        /// </summary>
        public int hv_Line1_MeasureThreshold = 20;

        /// <summary>
        /// 直线1：拟合数量
        /// </summary>
        public int hv_Line1_MeasureNumInstances = 1;

        /// <summary>
        /// 直线1：距离剔除阈值
        /// </summary>
        public double hv_Line1_MeasureDistThreshold = 3.5;

        /// <summary>
        /// 直线1：明暗方向 'all','negative','positive','uniform'
        /// </summary>
        public string hv_Line1_MeasureTransition = "all";

        /// <summary>
        /// 直线1：边缘选取 'all','first','last'
        /// </summary>
        public string hv_Line1_MeasureSelect = "first";

        /// <summary>
        /// 直线1：最小得分
        /// </summary>
        public double hv_Line1_MeasureMinScore = 0.5;

        /// <summary>
        /// 直线1：迭代次数
        /// </summary>
        public int hv_Line1_MaxNumIterations = -1;

        /// <summary>
        /// 直线1：插值类型
        /// </summary>
        public string hv_Line1_MeasureInterpolation = "nearest_neighbor";

        /// <summary>
        /// 直线1：测量结果验证
        /// </summary>
        public string hv_Line1_MeasureIORegions = "true";

        #endregion

        #region 3. 输入参数 - 直线2 ROI

        /// <summary>
        /// 直线2：ROI中点行坐标
        /// </summary>
        public double hv_Line2_RoiMidRow = 300;

        /// <summary>
        /// 直线2：ROI中点列坐标
        /// </summary>
        public double hv_Line2_RoiMidColumn = 400;

        /// <summary>
        /// 直线2：ROI旋转角度(度)
        /// </summary>
        public double hv_Line2_RoiAngle = -90;

        /// <summary>
        /// 直线2：ROI高度
        /// </summary>
        public double hv_Line2_RoiHeight = 200;

        /// <summary>
        /// 直线2：ROI宽度
        /// </summary>
        public double hv_Line2_RoiWidth = 100;

        /// <summary>
        /// 直线2：ROI采样密度
        /// </summary>
        public int hv_Line2_RoiPointsNum = 20;

        /// <summary>
        /// 直线2：平滑系数
        /// </summary>
        public double hv_Line2_MeasureSigma = 3.0;

        /// <summary>
        /// 直线2：灰度阈值
        /// </summary>
        public int hv_Line2_MeasureThreshold = 20;

        /// <summary>
        /// 直线2：拟合数量
        /// </summary>
        public int hv_Line2_MeasureNumInstances = 1;

        /// <summary>
        /// 直线2：距离剔除阈值
        /// </summary>
        public double hv_Line2_MeasureDistThreshold = 3.5;

        /// <summary>
        /// 直线2：明暗方向
        /// </summary>
        public string hv_Line2_MeasureTransition = "all";

        /// <summary>
        /// 直线2：边缘选取
        /// </summary>
        public string hv_Line2_MeasureSelect = "first";

        /// <summary>
        /// 直线2：最小得分
        /// </summary>
        public double hv_Line2_MeasureMinScore = 0.5;

        /// <summary>
        /// 直线2：迭代次数
        /// </summary>
        public int hv_Line2_MaxNumIterations = -1;

        /// <summary>
        /// 直线2：插值类型
        /// </summary>
        public string hv_Line2_MeasureInterpolation = "nearest_neighbor";

        /// <summary>
        /// 直线2：测量结果验证
        /// </summary>
        public string hv_Line2_MeasureIORegions = "true";

        #endregion

        #region 4. 输出变量

        /// <summary>
        /// 直线1：拟合点行坐标
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple hv_Line1_CrossRow = null;

        /// <summary>
        /// 直线1：拟合点列坐标
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple hv_Line1_CrossColumn = null;

        /// <summary>
        /// 直线1：起始行
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple hv_Line1_RowBegin = null;

        /// <summary>
        /// 直线1：起始列
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple hv_Line1_ColumnBegin = null;

        /// <summary>
        /// 直线1：终止行
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple hv_Line1_RowEnd = null;

        /// <summary>
        /// 直线1：终止列
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple hv_Line1_ColumnEnd = null;

        /// <summary>
        /// 直线2：拟合点行坐标
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple hv_Line2_CrossRow = null;

        /// <summary>
        /// 直线2：拟合点列坐标
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple hv_Line2_CrossColumn = null;

        /// <summary>
        /// 直线2：起始行
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple hv_Line2_RowBegin = null;

        /// <summary>
        /// 直线2：起始列
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple hv_Line2_ColumnBegin = null;

        /// <summary>
        /// 直线2：终止行
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple hv_Line2_RowEnd = null;

        /// <summary>
        /// 直线2：终止列
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple hv_Line2_ColumnEnd = null;

        /// <summary>
        /// 交点行坐标
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple hv_IntersectionRow = null;

        /// <summary>
        /// 交点列坐标
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple hv_IntersectionColumn = null;

        /// <summary>
        /// 两条直线夹角(度)
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple hv_LineAngle = null;

        /// <summary>
        /// 两直线是否平行(平行则无交点)
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple hv_IsParallel = null;

        /// <summary>
        /// 直线1拟合成功标志
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public bool bl_Line1_FitSuccess = false;

        /// <summary>
        /// 直线2拟合成功标志
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public bool bl_Line2_FitSuccess = false;

        /// <summary>
        /// 交点计算成功标志
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public bool bl_IntersectionSuccess = false;

        #endregion

        public ParamsFindIntersectionAngle()
        {
            HOperatorSet.GenEmptyObj(out ho_Image);
        }
    }
}

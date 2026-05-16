using HalconDotNet;
using System;
using System.ComponentModel;
using System.Xml.Serialization;
using YYSD.UICore.Control;

namespace Frame_Core.pluginParams
{
    /// <summary>
    /// 两条抓边找交点参数类。
    /// </summary>
    [Serializable]
    public class ParamsFindIntersection : PluginParamsBase
    {
        #region 输入参数

        /// <summary>
        /// 输入图像，由流程框架或上游模块传入。
        /// </summary>
        [NonSerialized]
        [XmlIgnore]
        public HObject ho_Image;

        [ShowTextAttribute("图像来源索引")]
        [Description("0=当前输入图像，1=指定模块输出图像")]
        public int ImageSourceIndex = 0;

        [ShowTextAttribute("图像来源模块")]
        [Description("图像来源模块名称")]
        public string ImageSourceModule = "default";

        [ShowTextAttribute("图像来源名称")]
        [Description("图像来源输出参数名称")]
        public string ImageSourceName = "default";

        [ShowTextAttribute("工具名称")]
        [Description("当前找交点工具名称")]
        public string str_ModelName = "未命名";

        #endregion

        #region 抓边1 Rectangle2 ROI

        [ShowTextAttribute("抓边1中心行")]
        [Description("抓边1 Rectangle2 ROI 中心 Row")]
        public double hv_Line1_RoiMidRow = 200;

        [ShowTextAttribute("抓边1中心列")]
        [Description("抓边1 Rectangle2 ROI 中心 Column")]
        public double hv_Line1_RoiMidColumn = 300;

        [ShowTextAttribute("抓边1角度")]
        [Description("抓边1 Rectangle2 ROI 角度，单位度")]
        public double hv_Line1_RoiAngle = 0;

        [ShowTextAttribute("抓边1高度")]
        [Description("抓边1 Rectangle2 ROI 高度")]
        public double hv_Line1_RoiHeight = 200;

        [ShowTextAttribute("抓边1宽度")]
        [Description("抓边1 Rectangle2 ROI 宽度")]
        public double hv_Line1_RoiWidth = 100;

        [ShowTextAttribute("抓边1采样点数")]
        [Description("抓边1测量采样点数量")]
        public int hv_Line1_RoiPointsNum = 20;

        [ShowTextAttribute("抓边1平滑系数")]
        [Description("抓边1边缘测量 Sigma")]
        public double hv_Line1_MeasureSigma = 3.0;

        [ShowTextAttribute("抓边1灰度阈值")]
        [Description("抓边1边缘测量阈值")]
        public int hv_Line1_MeasureThreshold = 20;

        [ShowTextAttribute("抓边1拟合数量")]
        [Description("抓边1测量拟合实例数量")]
        public int hv_Line1_MeasureNumInstances = 1;

        [ShowTextAttribute("抓边1距离剔除阈值")]
        [Description("抓边1拟合距离剔除阈值")]
        public double hv_Line1_MeasureDistThreshold = 3.5;

        [ShowTextAttribute("抓边1明暗方向")]
        [Description("抓边1明暗方向，可选 all、negative、positive、uniform")]
        public string hv_Line1_MeasureTransition = "all";

        [ShowTextAttribute("抓边1边缘选取")]
        [Description("抓边1边缘选取方式，可选 first、last、all")]
        public string hv_Line1_MeasureSelect = "first";

        [ShowTextAttribute("抓边1最小得分")]
        [Description("抓边1拟合最小得分")]
        public double hv_Line1_MeasureMinScore = 0.5;

        [ShowTextAttribute("抓边1迭代次数")]
        [Description("抓边1最大迭代次数，-1表示使用默认值")]
        public int hv_Line1_MaxNumIterations = -1;

        [ShowTextAttribute("抓边1插值类型")]
        [Description("抓边1测量插值类型")]
        public string hv_Line1_MeasureInterpolation = "nearest_neighbor";

        [ShowTextAttribute("抓边1显示测量区域")]
        [Description("抓边1是否输出测量区域")]
        public string hv_Line1_MeasureIORegions = "true";

        #endregion

        #region 抓边2 Rectangle2 ROI

        [ShowTextAttribute("抓边2中心行")]
        [Description("抓边2 Rectangle2 ROI 中心 Row")]
        public double hv_Line2_RoiMidRow = 300;

        [ShowTextAttribute("抓边2中心列")]
        [Description("抓边2 Rectangle2 ROI 中心 Column")]
        public double hv_Line2_RoiMidColumn = 400;

        [ShowTextAttribute("抓边2角度")]
        [Description("抓边2 Rectangle2 ROI 角度，单位度")]
        public double hv_Line2_RoiAngle = -90;

        [ShowTextAttribute("抓边2高度")]
        [Description("抓边2 Rectangle2 ROI 高度")]
        public double hv_Line2_RoiHeight = 200;

        [ShowTextAttribute("抓边2宽度")]
        [Description("抓边2 Rectangle2 ROI 宽度")]
        public double hv_Line2_RoiWidth = 100;

        [ShowTextAttribute("抓边2采样点数")]
        [Description("抓边2测量采样点数量")]
        public int hv_Line2_RoiPointsNum = 20;

        [ShowTextAttribute("抓边2平滑系数")]
        [Description("抓边2边缘测量 Sigma")]
        public double hv_Line2_MeasureSigma = 3.0;

        [ShowTextAttribute("抓边2灰度阈值")]
        [Description("抓边2边缘测量阈值")]
        public int hv_Line2_MeasureThreshold = 20;

        [ShowTextAttribute("抓边2拟合数量")]
        [Description("抓边2测量拟合实例数量")]
        public int hv_Line2_MeasureNumInstances = 1;

        [ShowTextAttribute("抓边2距离剔除阈值")]
        [Description("抓边2拟合距离剔除阈值")]
        public double hv_Line2_MeasureDistThreshold = 3.5;

        [ShowTextAttribute("抓边2明暗方向")]
        [Description("抓边2明暗方向，可选 all、negative、positive、uniform")]
        public string hv_Line2_MeasureTransition = "all";

        [ShowTextAttribute("抓边2边缘选取")]
        [Description("抓边2边缘选取方式，可选 first、last、all")]
        public string hv_Line2_MeasureSelect = "first";

        [ShowTextAttribute("抓边2最小得分")]
        [Description("抓边2拟合最小得分")]
        public double hv_Line2_MeasureMinScore = 0.5;

        [ShowTextAttribute("抓边2迭代次数")]
        [Description("抓边2最大迭代次数，-1表示使用默认值")]
        public int hv_Line2_MaxNumIterations = -1;

        [ShowTextAttribute("抓边2插值类型")]
        [Description("抓边2测量插值类型")]
        public string hv_Line2_MeasureInterpolation = "nearest_neighbor";

        [ShowTextAttribute("抓边2显示测量区域")]
        [Description("抓边2是否输出测量区域")]
        public string hv_Line2_MeasureIORegions = "true";

        #endregion

        #region 位置跟随

        [ShowTextAttribute("启用位置跟随")]
        [Description("启用后按基准位置到当前位置的平移和旋转修正两个抓边 ROI")]
        public bool m_bUsePositionFollow = false;

        [ShowTextAttribute("基准行")]
        [Description("位置跟随基准 Row")]
        public double m_fBaseRow = 0;

        [ShowTextAttribute("基准列")]
        [Description("位置跟随基准 Column")]
        public double m_fBaseCol = 0;

        [ShowTextAttribute("基准角度")]
        [Description("位置跟随基准角度，单位度")]
        public double m_fBaseAngle = 0;

        [ShowTextAttribute("当前行")]
        [Description("位置跟随当前 Row")]
        public double m_fCurrentRow = 0;

        [ShowTextAttribute("当前列")]
        [Description("位置跟随当前 Column")]
        public double m_fCurrentCol = 0;

        [ShowTextAttribute("当前角度")]
        [Description("位置跟随当前角度，单位度")]
        public double m_fCurrentAngle = 0;


        [ShowTextAttribute("当前坐标有效")]
        [Description("当前位置由上游流程或界面右键设置后置为 true；启用跟随时必须有效")]
        public bool m_bCurrentPoseValid = false;
        #endregion

        #region 输出变量

        [ShowTextAttribute("跟随偏差Y")]
        [Description("当前位置 Row - 基准 Row，界面按 Y 显示")]
        public double m_fFollowOffsetY = 0;

        [ShowTextAttribute("跟随偏差X")]
        [Description("当前位置 Column - 基准 Column，界面按 X 显示")]
        public double m_fFollowOffsetX = 0;

        [ShowTextAttribute("跟随偏差R")]
        [Description("当前角度 - 基准角度，单位度")]
        public double m_fFollowOffsetR = 0;

        [NonSerialized]
        [XmlIgnore]
        public HTuple hv_Line1_CrossRow = null;

        [NonSerialized]
        [XmlIgnore]
        public HTuple hv_Line1_CrossColumn = null;

        [NonSerialized]
        [XmlIgnore]
        public HTuple hv_Line1_RowBegin = null;

        [NonSerialized]
        [XmlIgnore]
        public HTuple hv_Line1_ColumnBegin = null;

        [NonSerialized]
        [XmlIgnore]
        public HTuple hv_Line1_RowEnd = null;

        [NonSerialized]
        [XmlIgnore]
        public HTuple hv_Line1_ColumnEnd = null;

        [NonSerialized]
        [XmlIgnore]
        public HTuple hv_Line2_CrossRow = null;

        [NonSerialized]
        [XmlIgnore]
        public HTuple hv_Line2_CrossColumn = null;

        [NonSerialized]
        [XmlIgnore]
        public HTuple hv_Line2_RowBegin = null;

        [NonSerialized]
        [XmlIgnore]
        public HTuple hv_Line2_ColumnBegin = null;

        [NonSerialized]
        [XmlIgnore]
        public HTuple hv_Line2_RowEnd = null;

        [NonSerialized]
        [XmlIgnore]
        public HTuple hv_Line2_ColumnEnd = null;

        [NonSerialized]
        [XmlIgnore]
        public HTuple hv_IntersectionRow = null;

        [NonSerialized]
        [XmlIgnore]
        public HTuple hv_IntersectionColumn = null;

        [NonSerialized]
        [XmlIgnore]
        public HTuple hv_IntersectionAngle = null;

        [NonSerialized]
        [XmlIgnore]
        public HTuple hv_LineAngle = null;

        [NonSerialized]
        [XmlIgnore]
        public HTuple hv_IsParallel = null;

        [NonSerialized]
        [XmlIgnore]
        public bool bl_Line1_FitSuccess = false;

        [NonSerialized]
        [XmlIgnore]
        public bool bl_Line2_FitSuccess = false;

        [NonSerialized]
        [XmlIgnore]
        public bool bl_IntersectionSuccess = false;

        #endregion

        public ParamsFindIntersection()
        {
            HOperatorSet.GenEmptyObj(out ho_Image);
        }
    }
}

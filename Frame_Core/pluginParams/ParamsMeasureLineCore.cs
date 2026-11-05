using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frame_Core;
using HalconDotNet;
using YYSD.UICore.Control;

namespace Frame_Core
{
    /// <summary>
    /// 直线测量参数类
    /// </summary>
    [Serializable]
    public class ParamsMeasureLine : PluginParamsBase
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

        public string ImageSourceName = "default";
        /// <summary>
        /// 输入变量：直线名称
        /// </summary>
        [Description("直线名称")]
        public string str_ModelName = "未命名";

        /// <summary>
        /// 输入变量：Roi密度：
        /// </summary>
        [Description("Roi密度")]
        public int hv_RoiPointsNum = 20;

        /// <summary>
        /// 输入变量：抓边ROI中点行坐标
        /// </summary>
        [Description("ROI中心坐标")]
        public double hv_RoiMidRow = 300;

        /// <summary>
        /// 输入变量：抓边ROI中点列坐标
        /// </summary>
        [Description("抓边ROI中点列坐标")]
        public double hv_RoiMidColumn = 300;

        /// <summary>
        /// 输入变量：抓边ROI旋转角度
        /// </summary>
        [Description("抓边ROI旋转角度")]
        public double hv_RoiAngle = -90;

        /// <summary>
        /// 输入变量：抓边ROI高度
        /// </summary>
        [Description("抓边ROI高度")]
        public double hv_RoiHeight = 200;

        /// <summary>
        /// 输入变量：抓边ROI宽度
        /// </summary>
        [Description("抓边ROI宽度")]
        public double hv_RoiWidth = 200;


        /// <summary>
        /// 输入变量：ROI运动方式
        /// </summary>
        [Description("ROI运动方式")]
        public string str_RoiMoveIndex = "StaticROI";

        /// <summary>
        /// 输入变量：ROI基准行坐标
        /// </summary>
        [Description("ROI基准行坐标")]
        public double hv_RoiStandardRow = 0;

        /// <summary>
        /// 输入变量：ROI基准列坐标
        /// </summary>
        [Description("ROI基准列坐标")]
        public double hv_RoiStandardColumn = 0;

        /// <summary>
        /// 输入变量：ROI基准行偏移量
        /// </summary>
        [Description("ROI基准行偏移量")]
        public double hv_RoiStandardOffsetRow = 0;

        /// <summary>
        /// 输入变量：ROI基准列偏移量
        /// </summary>
        [Description("ROI基准列偏移量")]
        public double hv_RoiStandardOffsetColumn = 0;

        /// <summary>
        /// 输入变量：ROI递增次数
        /// </summary>
        [Description("ROI递增次数")]
        public int hv_RoiIncreaseNum = 0;

        /// <summary>
        /// 输入变量：ROI行增量
        /// </summary>
        [Description("ROI行增量")]
        public int hv_RoiIncreaseRow = 0;

        /// <summary>
        /// 输入变量：ROI列增量
        /// </summary>
        [Description("ROI列增量")]
        public int hv_RoiIncreaseColumn = 0;


        /// <summary>
        /// 输入变量：是否进行图像预处理
        /// </summary>
        [Description("是否进行图像预处理")]
        public bool bl_ImageProcessIndex = false;

        /// <summary>
        /// 输入变量：对比度增强-底色选取
        /// </summary>
        [Description("对比度增强-底色选取")]
        public int hv_ScaleMean = 255;

        /// <summary>
        /// 输入变量：对比度增强-增强因子
        /// </summary>
        [Description("对比度增强-增强因子")]
        public double hv_ScaleFactor = 0.0;

        /// <summary>
        /// 输入变量：图像锐化迭代次数
        /// </summary>
        [Description("图像锐化迭代次数")]
        public int hv_SharpenIterations = 0;

        /// <summary>
        /// 输入变量：图像锐化平滑系数
        /// </summary>
        [Description("图像锐化平滑系数")]
        public double hv_SharpenSigma = 0.5;


        /// <summary>
        /// 输入变量：抓边检测直线模型平滑系数
        /// </summary>
        [Description("平滑系数")]
        public double hv_MeasureSigma = 3.0;

        /// <summary>
        /// 输入变量：抓边检测直线模型灰度阈值
        /// </summary>
        [Description("灰度阈值")]
        public int hv_MeasureThreshold = 20;

        /// <summary>
        /// 输入变量：直线拟合数量：默认值： 1 值的列表： 1，2，3，4
        /// </summary>
        [Description("直线拟合数量")]
        public int hv_MeasureNumInstances = 1;

        /// <summary>
        /// 输入变量：距离剔除：默认值：3.5  值的列表： 0，1.0，2.0， 3.5，5.0
        /// </summary>
        [Description("距离剔除")]
        public double hv_MeasureDistThreshold = 4;

        /// <summary>
        /// 输入变量：明暗方向：值列表： 'all'，'negative'， 'positive'，'uniform'
        /// </summary>
        [Description("明暗方向")]
        public string hv_MeasureTransition = "positive";


        /// <summary>
        /// 输入变量：明暗方向：值列表： 'all'，'negative'， 'positive'，'uniform'
        /// </summary>
        [Description("历史抓边明暗方向")]
        public string hv_HistoryMeasureTransition = "positive";

        /// <summary>
        /// 输入变量：边缘选取：'all'，'first'，'last'
        /// </summary>
        [Description("边缘选取")]
        public string hv_MeasureSelect = "first";


        /// <summary>
        /// 输入变量：峰谷值选取：'None'，'peak'，'valley'
        /// </summary>
        [Description("峰谷值选取")]
        public string hv_PeakValleySelect = "None";

        /// <summary>
        /// 输入变量：最小得分：'min_score'
        /// </summary>
        [Description("最小得分")]
        public double hv_MeasureMinScore = 0.5;

        /// <summary>
        /// 输入变量：迭代次数: 默认值：-1  值的列表： 10，100，1000
        /// </summary>
        [Description("迭代次数")]
        public int hv_MaxNumIterations = -1;

        /// <summary>
        /// 输入变量：插值类型：nearest_neighbor'灰度值是从最近像素的灰度值获得；'bilinear'，使用双线性插值 ；'bicubic'，则使用双三次插值。
        /// </summary>
        [Description("插值类型")]
        public string hv_MeasureInterpolation = "nearest_neighbor";

        /// <summary>
        /// 输入变量：指定对测量结果的验证：默认值： “ false”  值列表： 'true'，'false'
        /// </summary>
        [Description("测量结果验证")]
        public string hv_MeasureIORegions = "true";

        /// <summary>
        /// 输入变量：指定对测量结果的验证：默认值： “ false”  值列表： 'true'，'false'
        /// </summary>
        [Description("测量结果验证")]
        public bool bl_DefaultLine = false;

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

        /// <summary>
        /// 输出变量：抓边检测直线终止行坐标
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple hv_PeakpointRow = null;

        /// <summary>
        /// 输出变量：抓边检测直线终止列坐标
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple hv_PeakpointColumn = null;

        [ShowTextAttribute("跟随偏差Y")]
        [Description("当前位置 Row - 基准 Row，界面按 Y 显示")]
        public double m_fFollowOffsetY = 0;

        [ShowTextAttribute("跟随偏差X")]
        [Description("当前位置 Column - 基准 Column，界面按 X 显示")]
        public double m_fFollowOffsetX = 0;

        [ShowTextAttribute("跟随偏差R")]
        [Description("当前角度 - 基准角度，单位度")]
        public double m_fFollowOffsetR = 0;

        #endregion
        public ParamsMeasureLine()
        {
            HOperatorSet.GenEmptyObj(out ho_Image);
            HOperatorSet.GenEmptyObj(out ho_ImageProcessed);
        }

    }

}


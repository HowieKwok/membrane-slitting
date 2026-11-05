using HalconDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using YYSD.UICore.Control;

namespace Frame_Core.pluginParams
{
    [Serializable]
    public class ParamsFindModel : PluginParamsBase
    {


        /// <summary>
        /// 输入变量：原图
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject ho_Image;

        [ShowTextAttribute("创建模板参数组")]
        [Description("创建模板参数组")]
        public List<CreateModelParam> m_lCreateParams = new List<CreateModelParam>();



        [ShowTextAttribute("查找模板参数组")]
        [Description("查找模板参数组")]
        public List<FindModelParam> m_lFindParams = new List<FindModelParam>();

        /// <summary>
        /// 模板图片路径//输入变量
        /// </summary>
        [ShowTextAttribute("模板图片路径")]
        [Description("模板图片路径")]
        public string m_strModelPath = AppDomain.CurrentDomain.BaseDirectory;


  

        /// <summary>
        /// 模板图片路径//输入变量
        /// </summary>
        [ShowTextAttribute("模板图片组路径")]
        [Description("模板图片组路径")]
        public string m_strModelGroupPath = AppDomain.CurrentDomain.BaseDirectory;

        /// <summary>
        /// 超时时间//输入变量
        /// </summary>
        [ShowTextAttribute("超时时间")]
        [Description("超时时间")]
        public int m_nOverTime = 30;

        /// <summary>
        /// 边缘找点//输入变量
        /// </summary>
        [ShowTextAttribute("边缘开始位置找点")]
        [Description("边缘开始位置找点")]
        public double m_fEdgeFindCol = 1000.0;

        /// <summary>
        /// 边缘找点//输入变量
        /// </summary>
        [ShowTextAttribute("边缘结束位置找点")]
        [Description("边缘结束位置找点")]
        public double m_fEdgeFindEndCol = 4000;

        /// <summary>
        /// 边缘找点//输入变量
        /// </summary>
        [ShowTextAttribute("边缘开始位置找点Rol")]
        [Description("边缘开始位置找点Rol")]
        public double m_fEdgeFindRow = 1000;


        /// <summary>
        /// 边缘找点//输入变量
        /// </summary>
        [ShowTextAttribute("边缘结束位置找点Rol")]
        [Description("边缘结束位置找点Rol")]
        public double m_fEdgeFindEndRow = 3000.0;


        /// <summary>
        /// 输入图片压缩Width系数//输入变量
        /// </summary>
        [ShowTextAttribute("输入图片压缩Width系数")]
        [Description("输入图片压缩Width系数")]
        public double m_scaleWidth = 1.0;

        /// <summary>
        /// 输入图片压缩Height系数//输入变量
        /// </summary>
        [ShowTextAttribute("输入图片压缩Height系数")]
        [Description("输入图片压缩Height系数")]
        public double m_scaleHeight = 1.0;
        /// <summary>
        /// 模板Region中心Row//输入变量
        /// </summary>
        [ShowTextAttribute("模板Region中心Row")]
        [Description("模板Region中心Row")]
        public double m_fCenterRow = 0.0;

        /// <summary>
        /// 模板Region中心Col//输入变量
        /// </summary>
        [ShowTextAttribute("模板Region中心Col")]
        [Description("模板Region中心Col")]
        public double m_fCenterCol = 0.0;

        /// <summary>
        /// 模板Region中心Length_X`//输入变量
        /// </summary>
        [ShowTextAttribute("模板Region中心Length_X")]
        [Description("模板Region中心Length_X")]
        public double m_fCenterLength_X = 10.0;

        /// <summary>
        /// 模板Region中心Length_Y`//输入变量
        /// </summary>
        [ShowTextAttribute("模板Region中心Length_Y")]
        [Description("模板Region中心Length_Y")]
        public double m_fCenterLength_Y = 10.0;

        /// <summary>
        /// 模板Region中心Col//输入变量
        /// </summary>
        [ShowTextAttribute("模板Region中心Angle")]
        [Description("模板Region中心Angle")]
        public double m_fCenterAngle = 0.0;




  



        /// <summary>
        /// 偏差值X是否取反//输入变量
        /// </summary>
        [ShowTextAttribute("偏差值X是否取反")]
        [Description("偏差值X是否取反")]
        public bool  m_bNotOffsetX = false;


        /// <summary>
        /// 偏差值Y是否取反//输入变量
        /// </summary>
        [ShowTextAttribute("偏差值Y是否取反")]
        [Description("偏差值Y是否取反")]
        public bool m_bNotOffsetY = false;



        /// <summary>
        /// 偏差值R是否取反//输入变量
        /// </summary>
        [ShowTextAttribute("偏差值R是否取反")]
        [Description("偏差值R是否取反")]
        public bool m_bNotOffsetR = false;


        /// <summary>
        /// 判断是否启用//输入变量
        /// </summary>
        [ShowTextAttribute("判断是否启用")]
        [Description("判断是否启用")]
        public bool m_bIsEnable = false;

        /// <summary>
        /// 输出变量：缩小图片
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject m_ho_OutImgShow;


        /// <summary>
        /// 输出变量：裁图Region
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject m_ho_OutCropRegion;


        /// <summary>
        /// 输出变量：匹配Region
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject m_ho_OutRegion;

        /// <summary>
        /// 输出变量：匹配圆度
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject m_ho_OutCircleRegion;


        /// <summary>
        /// 输出变量：匹配分数
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public double m_fScoreMatch = 0;


        /// <summary>
        /// 输出变量：匹配圆度分数
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public double m_fScoreCircularity = 0;


        /// <summary>
        /// 输出变量：偏差X
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public double m_fOffsetX = 0.0;

        /// <summary>
        /// 输出变量：偏差Y
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public double m_fOffsetY = 0.0;

        /// <summary>
        /// 输出变量：偏差R
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public double m_fOffsetR = 0.0;


        /// <summary>
        /// 输出变量：目标位置X
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public double m_fGoalX = 0.0;

        /// <summary>
        /// 输出变量：目标位置Y
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public double m_fGoalY = 0.0;

        /// <summary>
        /// 输出变量：目标位置R
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public double m_fGoaR = 0.0;

        /// <summary>
        /// 输出变量：是否超时
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public bool m_bOverTime;
        /// <summary>
        /// 输出变量：当前识别到的目标模板名称
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public string m_strGoalName = "None";

        /// <summary>
        /// 输出变量：输出矩阵
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple  m_HtuMatID;


        


    }

    [Serializable]
    public class CreateModelParam : XmlSerializationOperation
    {

        [ShowTextAttribute("模板名称")]
        [Description("模板名称")]
        public string m_strModelName = "";

        /// <summary>
        /// 输入变量，金字塔层数
        /// </summary>
        [ShowTextAttribute("金字塔层数")]
        [Description("金字塔层数")]
        public int m_nNumLevels_Create = 3;




        /// <summary>
        /// 输入变量，起始角度Create
        /// </summary>
        [ShowTextAttribute("起始角度Create")]
        [Description("起始角度Create")]
        public int m_nAngleStart_Create = 0;
        /// <summary>
        /// 输入变量，角度范围Create
        /// </summary>
        [ShowTextAttribute("角度范围Create")]
        [Description("角度范围Create")]
        public int m_nAngleExtent_Create = 360;

        /// <summary>
        /// 输入变量，步长Create
        /// </summary>
        [ShowTextAttribute("步长Create")]
        [Description("步长Create")]
        public double m_nAngleStep_Create = 0;

        /// <summary>
        /// 输入变量，设置模板优化和模板创建方法Create
        /// </summary>
        [ShowTextAttribute("设置模板优化和模板创建方法Create")]
        [Description("设置模板优化和模板创建方法Create")]
        public int m_nOptimization_Create = 0;

        /// <summary>
        /// 输入变量，设置匹配方法Create
        /// </summary>
        [ShowTextAttribute("设置匹配方法Create")]
        [Description("设置匹配方法Create")]
        public int m_nMetric_Create = 0;

        /// <summary>
        /// 模板Region中心Row//输入变量
        /// </summary>
        [ShowTextAttribute("基准模板Region中心Row")]
        [Description("基准模板Region中心Row")]
        public double m_fCenterRowBase = 0.0;

        /// <summary>
        /// 模板Region中心Col//输入变量
        /// </summary>
        [ShowTextAttribute("基准保存用模板Region中心Col")]
        [Description("基准保存用模板Region中心Col")]
        public double m_fCenterColBase = 0.0;


        /// <summary>
        /// 模板Region中心Length_X`//输入变量
        /// </summary>
        [ShowTextAttribute("基准保存用模板Region中心Length_X")]
        [Description("基准保存用模板Region中心Length_X")]
        public double m_fCenterLength_XBase = 10;

        /// <summary>
        /// 模板Region中心Length_Y`//输入变量
        /// </summary>
        [ShowTextAttribute("基准保存用模板Region中心Length_Y")]
        [Description("基准保存用模板Region中心Length_Y")]
        public double m_fCenterLength_YBase = 10;

        /// <summary>
        /// 模板Region中心Col//输入变量
        /// </summary>
        [ShowTextAttribute("基准保存用模板Region中心Angle")]
        [Description("基准保存用模板Region中心Angle")]
        public double m_fCenterAngleBase = 0;
    }

    [Serializable]
    public class FindModelParam : XmlSerializationOperation
    {


        [ShowTextAttribute("模板名称")]
        [Description("模板名称")]
        public string m_strModelName = "";

        /// <summary>
        /// 输入变量，起始角度Find
        /// </summary>
        [ShowTextAttribute("起始角度Find")]
        [Description("起始角度Find")]
        public int m_nAngleStart_Find = 0;

        /// <summary>
        /// 输入变量，角度范围Find
        /// </summary>
        [ShowTextAttribute("角度范围Find")]
        [Description("角度范围Find")]
        public int m_nAngleExtent_Find = 360;

        /// <summary>
        /// 输入变量，输入最低的匹配分数Find
        /// </summary>
        [ShowTextAttribute("输入最低的匹配分数Find")]
        [Description("输入最低的匹配分数Find")]
        public double m_fMinScore_Find = 0.5;

        /// <summary>
        /// 输入变量，在图像上找到模板的最大个数Find
        /// </summary>
        [ShowTextAttribute("在图像上找到模板的最大个数Find")]
        [Description("在图像上找到模板的最大个数Find")]
        public int m_nNumMatches_Find = 1;

        /// <summary>
        /// 输入变量，定义了找到的两个目标区域最多重叠的系数Find
        /// </summary>
        [ShowTextAttribute("定义了找到的两个目标区域最多重叠的系数Find")]
        [Description("定义了找到的两个目标区域最多重叠的系数Find")]
        public double m_fMaxOverlap_Find = 0.5;

        /// <summary>
        /// 输入变量，确定找到的目标是否使用亚像素精度提取Find
        /// </summary>
        [ShowTextAttribute("在图像上是否启用亚像素Find")]
        [Description("在图像上是否启用亚像素Find")]
        public int m_nSubPixel_Find = 1;

        /// <summary>
        /// 输入变量，搜索时使用的金字塔层数Find
        /// </summary>
        [ShowTextAttribute("搜索时使用的金字塔层数Find")]
        [Description("搜索时使用的金字塔层数Find")]
        public int m_nNumLevels_Find = 3;

        /// <summary>
        /// 输入变量，搜索时的“贪婪程度”Find
        /// </summary>
        [ShowTextAttribute("搜索时的“贪婪程度”Find")]
        [Description("搜索时的“贪婪程度”Find")]
        public double m_fGreediness_Find = 0.9;

        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple ht_ModelID = 0;
    }


    //public class CreateModelParam : XmlSerializationOperation
    //{

    //    public string m_strModelName = "";

    //    public int m_nNumLevels_Create = 1;

    //    public int m_nAngleStart_Create = 0;

    //    public int m_nAngleExtent_Create = 360;


    //    public double m_nAngleStep_Create = 0;


    //    public int m_nOptimization_Create = 0;


    //    public int m_nMetric_Create = 0;
    //}

    //[Serializable]
    //public class FindModelParam
    //{



    //    public string m_strModelName = "";


    //    public int m_nAngleStart_Find = 0;


    //    public int m_nAngleExtent_Find = 360;

    //    public double m_fMinScore_Find = 0.5;

    //    public int m_nNumMatches_Find = 1;


    //    public double m_fMaxOverlap_Find = 0.5;


    //    public int m_nSubPixel_Find = 1;


    //    public int m_nNumLevels_Find = 1;


    //    public double m_fGreediness_Find = 0.9;

    //    [NonSerialized]
    //    [System.Xml.Serialization.XmlIgnore]
    //    public HTuple ht_ModelID = 0;
    //}
    //[Serializable]
    //public class ParamsFindModel : PluginParamsBase
    //{


    //    /// <summary>
    //    /// 输入变量：原图
    //    /// </summary>
    //    [NonSerialized]
    //    [System.Xml.Serialization.XmlIgnore]
    //    public HObject ho_Image;


    //    [ShowTextAttribute("创建模板参数组")]
    //    [Description("创建模板参数组")]
    //    public List<CreateModelParam> m_lCreateParams = new List<CreateModelParam>();



    //    [ShowTextAttribute("查找模板参数组")]
    //    [Description("查找模板参数组")]
    //    public List<FindModelParam> m_lFindParams = new List<FindModelParam>();



    //    /// <summary>
    //    /// 模板图片路径//输入变量
    //    /// </summary>
    //    [ShowTextAttribute("模板图片路径")]
    //    [Description("模板图片路径")]
    //    public string m_strModelPath = "E:\\";

    //    /// <summary>
    //    /// 模板图片路径//输入变量
    //    /// </summary>
    //    [ShowTextAttribute("模板图片组路径")]
    //    [Description("模板图片组路径")]
    //    public string m_strModelGroupPath = "E:\\";


    //    /// <summary>
    //    /// 超时时间//输入变量
    //    /// </summary>
    //    [ShowTextAttribute("超时时间")]
    //    [Description("超时时间")]
    //    public int m_nOverTime = 30;

    //    /// <summary>
    //    /// 边缘找点//输入变量
    //    /// </summary>
    //    [ShowTextAttribute("边缘开始位置找点")]
    //    [Description("边缘开始位置找点")]
    //    public double m_fEdgeFindCol = 10;

    //    /// <summary>
    //    /// 边缘找点//输入变量
    //    /// </summary>
    //    [ShowTextAttribute("边缘结束位置找点")]
    //    [Description("边缘结束位置找点")]
    //    public double m_fEdgeFindEndCol = 10;


    //    /// <summary>
    //    /// 输入图片压缩Width系数//输入变量
    //    /// </summary>
    //    [ShowTextAttribute("输入图片压缩Width系数")]
    //    [Description("输入图片压缩Width系数")]
    //    public double m_scaleWidth = 1;

    //    /// <summary>
    //    /// 输入图片压缩Height系数//输入变量
    //    /// </summary>
    //    [ShowTextAttribute("输入图片压缩Height系数")]
    //    [Description("输入图片压缩Height系数")]
    //    public double m_scaleHeight = 1;
    //    /// <summary>
    //    /// 模板Region中心Row//输入变量
    //    /// </summary>
    //    [ShowTextAttribute("模板Region中心Row")]
    //    [Description("模板Region中心Row")]
    //    public double m_fCenterRow = 0;

    //    /// <summary>
    //    /// 模板Region中心Col//输入变量
    //    /// </summary>
    //    [ShowTextAttribute("模板Region中心Col")]
    //    [Description("模板Region中心Col")]
    //    public double m_fCenterCol = 0;

    //    /// <summary>
    //    /// 模板Region中心Length_X`//输入变量
    //    /// </summary>
    //    [ShowTextAttribute("模板Region中心Length_X")]
    //    [Description("模板Region中心Length_X")]
    //    public double m_fCenterLength_X = 10;

    //    /// <summary>
    //    /// 模板Region中心Length_Y`//输入变量
    //    /// </summary>
    //    [ShowTextAttribute("模板Region中心Length_Y")]
    //    [Description("模板Region中心Length_Y")]
    //    public double m_fCenterLength_Y = 10;

    //    /// <summary>
    //    /// 模板Region中心Col//输入变量
    //    /// </summary>
    //    [ShowTextAttribute("模板Region中心Angle")]
    //    [Description("模板Region中心Angle")]
    //    public double m_fCenterAngle = 0;

    //    #region

    //    /// <summary>
    //    /// 输入变量，创建模板参数自动
    //    /// </summary>
    //    [ShowTextAttribute("创建模板参数自动")]
    //    [Description("创建模板参数自动")]
    //    public bool m_bIsAutoCreateModel = true;

    //    /// <summary>
    //    /// 输入变量，匹配模板参数自动
    //    /// </summary>
    //    [ShowTextAttribute("匹配模板参数自动")]
    //    [Description("匹配模板参数自动")]
    //    public bool m_bIsAutoFindModel = true;

    //    /// <summary>
    //    /// 输入变量，匹配模板参数自动
    //    /// </summary>
    //    [ShowTextAttribute("启用灰度匹配")]
    //    [Description("启用灰度匹配")]
    //    public bool m_bIsNccMatchStart = false;

    //    /// <summary>
    //    /// 输入变量，是否启用圆度检测
    //    /// </summary>
    //    [ShowTextAttribute("是否启用圆度检测")]
    //    [Description("是否启用圆度检测")]
    //    public bool m_bIsCircleDectect = false;

    //    /// <summary>
    //    /// 输入变量，金字塔层数
    //    /// </summary>
    //    [ShowTextAttribute("金字塔层数")]
    //    [Description("金字塔层数")]
    //    public int m_nNumLevels_Create = 1;

    //    /// <summary>
    //    /// 输入变量，起始角度Create
    //    /// </summary>
    //    [ShowTextAttribute("起始角度Create")]
    //    [Description("起始角度Create")]
    //    public int m_nAngleStart_Create = 0;
    //    /// <summary>
    //    /// 输入变量，角度范围Create
    //    /// </summary>
    //    [ShowTextAttribute("角度范围Create")]
    //    [Description("角度范围Create")]
    //    public int m_nAngleExtent_Create = 360;

    //    /// <summary>
    //    /// 输入变量，步长Create
    //    /// </summary>
    //    [ShowTextAttribute("步长Create")]
    //    [Description("步长Create")]
    //    public double m_nAngleStep_Create = 0;

    //    /// <summary>
    //    /// 输入变量，设置模板优化和模板创建方法Create
    //    /// </summary>
    //    [ShowTextAttribute("设置模板优化和模板创建方法Create")]
    //    [Description("设置模板优化和模板创建方法Create")]
    //    public int m_nOptimization_Create = 0;

    //    /// <summary>
    //    /// 输入变量，设置匹配方法Create
    //    /// </summary>
    //    [ShowTextAttribute("设置匹配方法Create")]
    //    [Description("设置匹配方法Create")]
    //    public int m_nMetric_Create = 0;

    //    /// <summary>
    //    /// 输入变量，起始角度Find
    //    /// </summary>
    //    [ShowTextAttribute("起始角度Find")]
    //    [Description("起始角度Find")]
    //    public int m_nAngleStart_Find = 0;

    //    /// <summary>
    //    /// 输入变量，角度范围Find
    //    /// </summary>
    //    [ShowTextAttribute("角度范围Find")]
    //    [Description("角度范围Find")]
    //    public int m_nAngleExtent_Find = 0;

    //    /// <summary>
    //    /// 输入变量，输入最低的匹配分数Find
    //    /// </summary>
    //    [ShowTextAttribute("输入最低的匹配分数Find")]
    //    [Description("输入最低的匹配分数Find")]
    //    public double m_fMinScore_Find = 0.5;

    //    /// <summary>
    //    /// 输入变量，在图像上找到模板的最大个数Find
    //    /// </summary>
    //    [ShowTextAttribute("在图像上找到模板的最大个数Find")]
    //    [Description("在图像上找到模板的最大个数Find")]
    //    public int m_nNumMatches_Find = 1;

    //    /// <summary>
    //    /// 输入变量，定义了找到的两个目标区域最多重叠的系数Find
    //    /// </summary>
    //    [ShowTextAttribute("定义了找到的两个目标区域最多重叠的系数Find")]
    //    [Description("定义了找到的两个目标区域最多重叠的系数Find")]
    //    public double m_fMaxOverlap_Find = 0.5;

    //    /// <summary>
    //    /// 输入变量，确定找到的目标是否使用亚像素精度提取Find
    //    /// </summary>
    //    [ShowTextAttribute("在图像上是否启用亚像素Find")]
    //    [Description("在图像上是否启用亚像素Find")]
    //    public int m_nSubPixel_Find = 1;

    //    /// <summary>
    //    /// 输入变量，搜索时使用的金字塔层数Find
    //    /// </summary>
    //    [ShowTextAttribute("搜索时使用的金字塔层数Find")]
    //    [Description("搜索时使用的金字塔层数Find")]
    //    public int m_nNumLevels_Find = 1;

    //    /// <summary>
    //    /// 输入变量，搜索时的“贪婪程度”Find
    //    /// </summary>
    //    [ShowTextAttribute("搜索时的“贪婪程度”Find")]
    //    [Description("搜索时的“贪婪程度”Find")]
    //    public double m_fGreediness_Find = 0.9;

    //    /// <summary>
    //    /// 输入变量，设置边缘处理滤波器
    //    /// </summary>
    //    [ShowTextAttribute("设置边缘处理滤波器")]
    //    [Description("设置边缘处理滤波器")]
    //    public int m_nFilter_Circle = 0;

    //    /// <summary>
    //    /// 输入变量，设置特征参数
    //    /// </summary>
    //    [ShowTextAttribute("设置特征参数")]
    //    [Description("设置特征参数")]
    //    public int m_nFeature_Circle = 0;
    //    /// <summary>
    //    /// 输入变量，设置平滑程度
    //    /// </summary>
    //    [ShowTextAttribute("设置平滑程度")]
    //    [Description("设置平滑程度")]
    //    public double m_fAlpha = 1.0;

    //    /// <summary>
    //    /// 输入变量，设置边缘振幅区间Low
    //    /// </summary>
    //    [ShowTextAttribute("设置平滑程度Low")]
    //    [Description("设置平滑程度Low")]
    //    public int m_nAmpLow = 1;

    //    /// <summary>
    //    /// 输入变量，设置边缘振幅区间High
    //    /// </summary>
    //    [ShowTextAttribute("设置平滑程度High")]
    //    [Description("设置平滑程度High")]
    //    public int m_nAmpHigh = 1;

    //    /// <summary>
    //    /// 输入变量，设置下限阈值1
    //    /// </summary>
    //    [ShowTextAttribute("设置下限阈值1")]
    //    [Description("设置下限阈值1")]
    //    public double m_fGrayMin1 = 0;

    //    /// <summary>
    //    /// 输入变量，设置上限阈值1
    //    /// </summary>
    //    [ShowTextAttribute("设置上限阈值1")]
    //    [Description("设置上限阈值1")]
    //    public double m_fGrayMax1 = 0;

    //    /// <summary>
    //    /// 输入变量，设置下限阈值2
    //    /// </summary>
    //    [ShowTextAttribute("设置下限阈值2")]
    //    [Description("设置下限阈值2")]
    //    public double m_fGrayMin2 = 0;

    //    /// <summary>
    //    /// 输入变量，设置上限阈值2
    //    /// </summary>
    //    [ShowTextAttribute("设置上限阈值2")]
    //    [Description("设置上限阈值2")]
    //    public double m_fGrayMax2 = 0;

    //    /// <summary>
    //    /// 输入变量，设置匹配圆度
    //    /// </summary>
    //    [ShowTextAttribute("设置匹配圆度")]
    //    [Description("设置匹配圆度")]
    //    public double m_fMatchCircle = 1;


    //    #endregion

    //    /// <summary>
    //    /// 输出变量：缩小图片
    //    /// </summary>
    //    [NonSerialized]
    //    [System.Xml.Serialization.XmlIgnore]
    //    public HObject m_ho_OutImgShow;


    //    /// <summary>
    //    /// 输出变量：裁图Region
    //    /// </summary>
    //    [NonSerialized]
    //    [System.Xml.Serialization.XmlIgnore]
    //    public HObject m_ho_OutCropRegion;


    //    /// <summary>
    //    /// 输出变量：匹配Region
    //    /// </summary>
    //    [NonSerialized]
    //    [System.Xml.Serialization.XmlIgnore]
    //    public HObject m_ho_OutRegion;

    //    /// <summary>
    //    /// 输出变量：匹配圆度
    //    /// </summary>
    //    [NonSerialized]
    //    [System.Xml.Serialization.XmlIgnore]
    //    public HObject m_ho_OutCircleRegion;


    //    /// <summary>
    //    /// 输出变量：匹配分数
    //    /// </summary>
    //    [NonSerialized]
    //    [System.Xml.Serialization.XmlIgnore]
    //    public double m_fScoreMatch = 0;


    //    /// <summary>
    //    /// 输出变量：匹配圆度分数
    //    /// </summary>
    //    [NonSerialized]
    //    [System.Xml.Serialization.XmlIgnore]
    //    public double m_fScoreCircularity = 0;

    //    /// <summary>
    //    /// 输出变量：是否超时
    //    /// </summary>
    //    [NonSerialized]
    //    [System.Xml.Serialization.XmlIgnore]
    //    public bool m_bOverTime;

    //}

    //[Serializable]
    //public class CreateModelParam : XmlSerializationOperation
    //{

    //    [ShowTextAttribute("模板名称")]
    //    [Description("模板名称")]
    //    public string m_strModelName = "";

    //    /// <summary>
    //    /// 输入变量，金字塔层数
    //    /// </summary>
    //    [ShowTextAttribute("金字塔层数")]
    //    [Description("金字塔层数")]
    //    public int m_nNumLevels_Create = 1;




    //    /// <summary>
    //    /// 输入变量，起始角度Create
    //    /// </summary>
    //    [ShowTextAttribute("起始角度Create")]
    //    [Description("起始角度Create")]
    //    public int m_nAngleStart_Create = 0;
    //    /// <summary>
    //    /// 输入变量，角度范围Create
    //    /// </summary>
    //    [ShowTextAttribute("角度范围Create")]
    //    [Description("角度范围Create")]
    //    public int m_nAngleExtent_Create = 360;

    //    /// <summary>
    //    /// 输入变量，步长Create
    //    /// </summary>
    //    [ShowTextAttribute("步长Create")]
    //    [Description("步长Create")]
    //    public double m_nAngleStep_Create =0 ;

    //    /// <summary>
    //    /// 输入变量，设置模板优化和模板创建方法Create
    //    /// </summary>
    //    [ShowTextAttribute("设置模板优化和模板创建方法Create")]
    //    [Description("设置模板优化和模板创建方法Create")]
    //    public int m_nOptimization_Create = 0;

    //    /// <summary>
    //    /// 输入变量，设置匹配方法Create
    //    /// </summary>
    //    [ShowTextAttribute("设置匹配方法Create")]
    //    [Description("设置匹配方法Create")]
    //    public int m_nMetric_Create = 0;
    //}

    //[Serializable]
    //public class FindModelParam : XmlSerializationOperation
    //{


    //    [ShowTextAttribute("模板名称")]
    //    [Description("模板名称")]
    //    public string m_strModelName = "";

    //    /// <summary>
    //    /// 输入变量，起始角度Find
    //    /// </summary>
    //    [ShowTextAttribute("起始角度Find")]
    //    [Description("起始角度Find")]
    //    public int m_nAngleStart_Find = 0;

    //    /// <summary>
    //    /// 输入变量，角度范围Find
    //    /// </summary>
    //    [ShowTextAttribute("角度范围Find")]
    //    [Description("角度范围Find")]
    //    public int m_nAngleExtent_Find = 360;

    //    /// <summary>
    //    /// 输入变量，输入最低的匹配分数Find
    //    /// </summary>
    //    [ShowTextAttribute("输入最低的匹配分数Find")]
    //    [Description("输入最低的匹配分数Find")]
    //    public double m_fMinScore_Find = 0.5;

    //    /// <summary>
    //    /// 输入变量，在图像上找到模板的最大个数Find
    //    /// </summary>
    //    [ShowTextAttribute("在图像上找到模板的最大个数Find")]
    //    [Description("在图像上找到模板的最大个数Find")]
    //    public int m_nNumMatches_Find = 1;

    //    /// <summary>
    //    /// 输入变量，定义了找到的两个目标区域最多重叠的系数Find
    //    /// </summary>
    //    [ShowTextAttribute("定义了找到的两个目标区域最多重叠的系数Find")]
    //    [Description("定义了找到的两个目标区域最多重叠的系数Find")]
    //    public double m_fMaxOverlap_Find = 0.5;

    //    /// <summary>
    //    /// 输入变量，确定找到的目标是否使用亚像素精度提取Find
    //    /// </summary>
    //    [ShowTextAttribute("在图像上是否启用亚像素Find")]
    //    [Description("在图像上是否启用亚像素Find")]
    //    public int m_nSubPixel_Find = 1;

    //    /// <summary>
    //    /// 输入变量，搜索时使用的金字塔层数Find
    //    /// </summary>
    //    [ShowTextAttribute("搜索时使用的金字塔层数Find")]
    //    [Description("搜索时使用的金字塔层数Find")]
    //    public int m_nNumLevels_Find = 1;

    //    /// <summary>
    //    /// 输入变量，搜索时的“贪婪程度”Find
    //    /// </summary>
    //    [ShowTextAttribute("搜索时的“贪婪程度”Find")]
    //    [Description("搜索时的“贪婪程度”Find")]
    //    public double m_fGreediness_Find = 0.9;

    //    [NonSerialized]
    //    [System.Xml.Serialization.XmlIgnore]
    //    public HTuple ht_ModelID = 0;
    //}

}

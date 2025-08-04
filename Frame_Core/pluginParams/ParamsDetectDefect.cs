using HalconDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YYSD.UICore.Control;

namespace Frame_Core
{
    /// <summary>
    /// 缺陷检测参数类
    /// </summary>
    [Serializable]
    public class ParamsDetectDefect : PluginParamsBase
    {
        #region 1. 输入参数
        /// <summary>
        /// 输入变量：原图
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject ho_Image;
        /// <summary>
        /// 输入变量：屏蔽区域
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject ho_ShieldRegion;

        /// <summary>
        /// 输入变量：破损检测名称
        /// </summary>
        public string str_ModelName = "未命名";

        /// <summary>
        /// 输入变量：Roi密度：
        /// </summary>
        public int hv_RoiPointsNum = 1;

        /// <summary>
        /// 输入变量：ROI中点行坐标
        /// </summary>
        public double hv_RoiMidRow = 300;

        /// <summary>
        /// 输入变量：ROI中点列坐标
        /// </summary>
        public double hv_RoiMidColumn = 300;

        /// <summary>
        /// 输入变量：ROI旋转角度
        /// </summary>
        public double hv_RoiAngle = -90;

        /// <summary>
        /// 输入变量：ROI高度
        /// </summary>
        public double hv_RoiHeight = 200;

        /// <summary>
        /// 输入变量：ROI宽度
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
        public int hv_ScaleValueR = 255;

        /// <summary>
        /// 输入变量：对比度增强-底色选取
        /// </summary>
        public int hv_ScaleValueG = 255;

        /// <summary>
        /// 输入变量：对比度增强-底色选取
        /// </summary>
        public int hv_ScaleValueB = 255;

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
        /// 输入变量：灰度阈值上限
        /// </summary>
        public int hv_SelectThresholdUp = 210;

        /// <summary>
        /// 输入变量：灰度阈值下限
        /// </summary>
        public int hv_SelectThresholdDown = 0;

        /// <summary>
        /// 输入变量：区域筛选方法
        /// </summary>
        public string hv_RegionSelectIndex = "outer_radius";

        /// <summary>
        /// 输入变量：残留最大值
        /// </summary>
        public double hv_ResidueMaxValue = 999999999;

        /// <summary>
        /// 输入变量：残留最小值
        /// </summary>
        public double hv_ResidueMinValue = 3;

        //输出区域预处理
        
        /// <summary>
        /// 输入变量：是否启用输出区域处理算法
        /// </summary>
        public bool IsRegionOperate = false;

        /// <summary>
        /// 输入变量：开运算半径
        /// </summary>
        public int hv_OpenRadius = 20;

        /// <summary>
        /// 输入变量：开运算半径
        /// </summary>
        public int hv_ErosionRadius = 15;

        /// <summary>
        /// 输入变量：膨胀半径确认
        /// </summary>
        public int hv_ErosionRadiusCof = 15;


        /// <summary>
        /// 输入变量：开运算半径
        /// </summary>
        public int hv_DilationRadius = 15;


        /// <summary>
        /// 输入变量：腐蚀半径确认
        /// </summary>
        public int hv_DilationRadiusCof = 15;


        /// <summary>
        /// 模板图片路径//输入变量
        /// </summary>
        [ShowTextAttribute("模板region路径")]
        [Description("模板region路径")]
        public string m_strModelGroupPath = "E:\\";


        /// <summary>
        /// 输出变量：模块运行成功标志
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public bool bl_ModelRunFlag = false;
        #endregion


        #region 2.输出变量
        /// <summary>
        /// 输出变量：对比度增强后的图像
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject ho_ImageScale;

        /// <summary>
        /// 输出变量：锐化处理后的图像
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject ho_ImageSharpen;

        /// <summary>
        /// 输出变量：图像处理后的图像
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject ho_ImageProcessed;

        /// <summary>
        /// 输出变量：筛选区域
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject ho_SelectedRegion;

        /// <summary>
        ///  输出变量：残留区域
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject ho_ResidueRegion;
        /// <summary>
        ///  输出变量：残留区域1
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject ho_ResidueRegion1;
        /// <summary>
        ///  输出变量：残留区域2
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject ho_ResidueRegion2;

        /// <summary>
        /// 输出变量：残留外接圆
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject ho_ResidueCircle;

        /// <summary>
        /// 输出变量：破损数目-总
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple  hv_ResidueNum = null;
        /// <summary>
        /// 输出变量：破损数目1
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple hv_ResidueNum1 = null;
        /// <summary>
        /// 输出变量：破损数目2
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple hv_ResidueNum2 = null;
        /// <summary>
        /// 输出变量：缺陷总像素个数
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple hv_totalAreaPix = null;
        #endregion

        public ParamsDetectDefect()
        {
            HOperatorSet.GenEmptyObj(out ho_Image);
            HOperatorSet.GenEmptyObj(out ho_ShieldRegion);
            
            HOperatorSet.GenEmptyObj(out ho_ImageScale);
            HOperatorSet.GenEmptyObj(out ho_ImageSharpen);
            HOperatorSet.GenEmptyObj(out ho_ImageProcessed);

            HOperatorSet.GenEmptyObj(out ho_SelectedRegion);
            HOperatorSet.GenEmptyObj(out ho_ResidueRegion);
            HOperatorSet.GenEmptyObj(out ho_ResidueCircle);
        }
    }
}

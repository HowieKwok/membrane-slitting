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
    public class ParamsD3MeasureDepth : PluginParamsBase
    {
        #region 1. 输入参数
        /// <summary>
        /// 输入变量：原图深度图
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject ho_Image;

        /// <summary>
        /// 输入变量：原图亮度图
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject ho_ImageLight;


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
        /// 输入变量：
        /// 中点行坐标
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
        /// 输出变量：模块运行成功标志
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public bool bl_ModelRunFlag = false;


        /// <summary>
        /// 输入变量：削薄区尺寸行数
        /// </summary>
        public double hv_ShaveSizeRow = 100;



        /// <summary>
        /// 输入变量：平移区域行
        /// </summary>
        public double MoveRegionRow = 100;

        /// <summary>
        /// 输入变量：平移区域列
        /// </summary>
        public double MoveRegionColumn = 0;

        /// <summary>
        /// 输入变量：拟合算法 /* Default value: 'regression' * List of values: 'huber', 'regression', 'tukey'
        /// </summary>
        public string hv_Algorithm = "regression";

        /// <summary>
        /// 输入变量：最大迭代次数
        /// </summary>
        public int hv_Iterations = 55;

        /// <summary>
        /// 输出值选择
        /// </summary>
        public int nOutputValueIndex = 0;
  

        /// <summary>
        /// 输入变量：用于消除异常值的裁剪因子
        /// </summary>
        public int hv_ClippingFactor = 15;

        /// <summary>
        /// 输入变量：校正因子
        /// </summary>
        public double hv_Mult = 1;

        /// <summary>
        /// 输入变量：修正值
        /// </summary>
        public double hv_Add = 0;

        /// <summary>
        /// 输入变量：像素类型 /* Default value: 'byte'  // * List of values: 'byte', 'real', 'uint2'
        /// </summary>
        public string hv_ImageType = "real";


        ///滤波算子参数
        /// <summary>
        /// 输入变量：拟合算法 /* Default value: 'regression' * List of values: 'huber', 'regression', 'tukey'
        /// </summary>
        public string DataFilter = "无滤波处理";

        /// <summary>
        /// 输入变量：最大迭代次数
        /// </summary>
        public int Averagefactor = 10;
        /// <summary>
        /// 输入变量：最大迭代次数
        /// </summary>
        public int Movefactor = 20;
        /// <summary>
        /// 输入变量：最大迭代次数
        /// </summary>
        public double rateHighRemove = 0.1;
        /// <summary>
        /// 输入变量：最大迭代次数
        /// </summary>
        public double rateLowRemove = 0.1;

        /// <summary>
        /// 输入变量：最大迭代次数
        /// </summary>
        public bool InvertValue = false;
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
        /// 输出变量：联合区域
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject ho_UnionPlaneRegion;

        /// <summary>
        /// 输出变量：深度平均值
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple hv_DepthMean = null;

        /// <summary>
        /// 输出变量：深度最大值
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple hv_DepthMax = null;
        /// <summary>
        /// 输出变量：深度最小值
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple hv_DepthMin = null;


        /// <summary>
        /// 输出变量：深度值
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple hv_DepthValue = null;

        #endregion

        public ParamsD3MeasureDepth()
        {
            HOperatorSet.GenEmptyObj(out ho_Image);
            HOperatorSet.GenEmptyObj(out ho_ImageLight);
            HOperatorSet.GenEmptyObj(out ho_ShieldRegion);

            HOperatorSet.GenEmptyObj(out ho_ImageScale);
            HOperatorSet.GenEmptyObj(out ho_ImageSharpen);
            HOperatorSet.GenEmptyObj(out ho_ImageProcessed);

            HOperatorSet.GenEmptyObj(out ho_UnionPlaneRegion);

        }
    }
}

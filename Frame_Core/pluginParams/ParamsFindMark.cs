using HalconDotNet;
using S7.Net.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using YYSD.UICore.Control;

namespace Frame_Core
{
    [Serializable]
    public class ParamsFindMark : PluginParamsBase
    {


        /// <summary>
        /// 输入变量：原图
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject ho_Image;


        /// <summary>
        /// Roi密度//输入变量
        /// </summary>
        [ShowTextAttribute("Roi密度")]
        [Description("Roi密度")]
        public int hv_RoiPointsNum = 1;

        /// <summary>
        /// ROI中点行坐标//输入变量
        /// </summary>
        [ShowTextAttribute("ROI中点行坐标")]
        [Description("ROI中点行坐标")]
        public double hv_RoiMidRow = 300;



        /// <summary>
        /// ROI中点列坐标//输入变量
        /// </summary>
        [ShowTextAttribute("ROI中点列坐标")]
        [Description("ROI中点列坐标")]
        public double hv_RoiMidColumn = 300;

        /// <summary>
        /// ROI旋转角度//输入变量
        /// </summary>
        [ShowTextAttribute("ROI旋转角度")]
        [Description("ROI旋转角度")]
        public double hv_RoiAngle = -90;


        /// <summary>
        /// 像素单元X//输入变量
        /// </summary>
        [ShowTextAttribute("像素单元X")]
        [Description("像素单元X")]
        public double m_fPixelUnitX = 0.0096712;


        /// <summary>
        /// 像素单元Y//输入变量
        /// </summary>
        [ShowTextAttribute("像素单元Y")]
        [Description("像素单元Y")]
        public double m_fPixelUnitY = 0.0096712;


        /// <summary>
        /// ROI高度//输入变量
        /// </summary>
        [ShowTextAttribute("ROI高度")]
        [Description("ROI高度")]
        public double hv_RoiHeight = 200;


        /// <summary>
        /// ROI宽度//输入变量
        /// </summary>
        [ShowTextAttribute("ROI宽度")]
        [Description("ROI宽度")]

        public double hv_RoiWidth = 200;



        /// <summary>
        /// 裁图高度//输入变量
        /// </summary>
        [ShowTextAttribute("裁图高度")]
        [Description("裁图高度")]
        public double hv_CropHeight = 200;


        /// <summary>
        /// ROI宽度//输入变量
        /// </summary>
        [ShowTextAttribute("ROI宽度")]
        [Description("ROI宽度")]

        public double hv_CropWidth = 200;

        /// <summary>
        /// StaticROI//输入变量
        /// </summary>
        [ShowTextAttribute("StaticROI")]
        [Description("StaticROI")]

        public string str_RoiMoveIndex = "StaticROI";

        /// <summary>
        /// 输入变量：ROI基准行坐标
        /// </summary>
        [ShowTextAttribute("ROI基准行坐标")]
        [Description("ROI基准行坐标")]
        public double hv_RoiStandardRow = 0;

        /// <summary>
        /// 输入变量：ROI基准列坐标
        /// </summary>
        [ShowTextAttribute("ROI基准列坐标")]
        [Description("ROI基准列坐标")]
        public double hv_RoiStandardColumn = 0;

        /// <summary>
        /// 输入变量：ROI基准行偏移量
        /// </summary>
        [ShowTextAttribute("ROI基准行偏移量")]
        [Description("ROI基准行偏移量")]
        public double hv_RoiStandardOffsetRow = 0;

        /// <summary>
        /// 输入变量：ROI基准列偏移量
        /// </summary>
        [ShowTextAttribute("ROI基准列偏移量")]
        [Description("ROI基准列偏移量")]
        public double hv_RoiStandardOffsetColumn = 0;

        /// <summary>
        /// 输入变量：ROI递增次数
        /// </summary>
        [ShowTextAttribute("ROI递增次数")]
        [Description("ROI递增次数")]
        public int hv_RoiIncreaseNum = 0;

        /// <summary>
        /// 输入变量：ROI行增量
        /// </summary>
        [ShowTextAttribute("ROI行增量")]
        [Description("ROI行增量")]
        public int hv_RoiIncreaseRow = 0;

        /// <summary>
        /// 输入变量：ROI列增量
        /// </summary>
        [ShowTextAttribute("ROI列增量")]
        [Description("ROI列增量")]
        public int hv_RoiIncreaseColumn = 0;


        /// <summary>
        /// 圆形最小阈值//输入变量
        /// </summary>
        [ShowTextAttribute("圆形最小阈值")]
        [Description("圆形最小阈值")]
        public int m_nCircleThreshMin = 150;

        /// <summary>
        /// 圆形最大阈值//输入变量
        /// </summary>
        [ShowTextAttribute("圆形最大阈值")]
        [Description("圆形最大阈值")]
        public int m_nCircleThreshMax = 255;


        /// <summary>
        /// 矩形最小阈值//输入变量
        /// </summary>
        [ShowTextAttribute("矩形最小阈值")]
        [Description("矩形最小阈值")]
        public int m_nRecThreshMin = 150;

        /// <summary>
        /// 矩形最大阈值//输入变量
        /// </summary>
        [ShowTextAttribute("矩形最大阈值")]
        [Description("矩形最大阈值")]
        public int m_nRecThreshMax = 255;

        /// <summary>
        /// 圆形最小宽度//输入变量
        /// </summary>
        [ShowTextAttribute("圆形最小宽度")]
        [Description("圆形最小宽度")]
        public int m_nCircleWidthMin = 250;

        /// <summary>
        /// Mark最大宽度//输入变量
        /// </summary>
        [ShowTextAttribute("圆形最大宽度")]
        [Description("圆形最大宽度")]
        public int m_nCircleWidthMax = 1000;


        /// <summary>
        /// 矩形最小宽度//输入变量
        /// </summary>
        [ShowTextAttribute("矩形最小宽度")]
        [Description("矩形最小宽度")]
        public int m_nRecWidthMin = 150;

        /// <summary>
        /// 矩形最大宽度//输入变量
        /// </summary>
        [ShowTextAttribute("矩形最大宽度")]
        [Description("矩形最大宽度")]
        public int m_nRecWidthMax = 300;


        /// <summary>
        /// Mark最小圆度//输入变量
        /// </summary>
        [ShowTextAttribute("Mark最小圆度")]
        [Description("Mark最小圆度")]
        public double m_fCircularityMin = 0.85;

        /// <summary>
        /// Mark最大圆度//输入变量
        /// </summary>
        [ShowTextAttribute("Mark最大圆度")]
        [Description("Mark最大圆度")]
        public double m_fCircularityMax = 1.00;


        /// <summary>
        /// Mark基准点X//输入变量
        /// </summary>
        [ShowTextAttribute("Mark基准点X")]
        [Description("Mark基准点X")]
        public double m_fBaseCenterX = 1.00;

        /// <summary>
        /// Mark基准点Y//输入变量
        /// </summary>
        [ShowTextAttribute("Mark基准点Y")]
        [Description("Mark基准点Y")]
        public double m_fBaseCenterY = 1.00;

        /// <summary>
        /// Mark基准点R//输入变量
        /// </summary>
        [ShowTextAttribute("Mark基准点R")]
        [Description("Mark基准点R")]
        public double m_fBaseCenterR = 0;

        /// <summary>
        /// 圆形平滑//输入变量
        /// </summary>
        [ShowTextAttribute("圆形平滑")]
        [Description("圆形平滑")]
        public int CircleOpenParam = 5;

        /// <summary>
        /// 矩形平滑//输入变量
        /// </summary>
        [ShowTextAttribute("矩形平滑")]
        [Description("矩形平滑")]
        public int RectangleOpenParam = 5;


        /// <summary>
        /// 矩形平滑//输入变量（0 不启用，1 找圆，2 找矩形）
        /// </summary>
        [ShowTextAttribute("mark类型")]
        [Description("mark类型")]
        public int MarkType = 1;



        /// <summary>
        /// 启用矩形识别//输入变量
        /// </summary>
        [ShowTextAttribute("启用矩形识别")]
        [Description("启用矩形识别")]
        public bool bIsEnRec = false;

        /// <summary>
        /// 启用圆识别//输入变量
        /// </summary>
        [ShowTextAttribute("启用圆识别")]
        [Description("启用圆识别")]
        public bool bIsEnCircleRecognize = true;


        /// <summary>
        /// 启用圆识别//输入变量
        /// </summary>
        [ShowTextAttribute("启用找边识别")]
        [Description("启用找边识别")]
        public bool bIsEnFindLine = true;



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
        /// 输出变量：矩形Region
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject m_ho_OutRecRegion;

        /// <summary>
        /// 输出变量：裁图
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject m_ho_CropImg;

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
        public double m_fGoalR = 0.0;

        /// <summary>
        /// 输出变量：左顶点Row
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public double leftTop_Row = 0;
        /// <summary>
        /// 输出变量：左顶点Col
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public double leftTop_Col = 0;
        /// <summary>
        /// 输出变量：右顶点Row
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public double rightTop_Row = 0;
        /// <summary>
        /// 输出变量：右顶点Col
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public double rightTop_Col = 0;
        /// <summary>
        /// 输出变量：左下角Row
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public double leftBottom_Row = 0;
        /// <summary>
        /// 输出变量：左下角Col
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public double leftBottom_Col = 0;

        /// <summary>
        /// 输出变量：右下角Row
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public double rightBottom_Row = 0;

        /// <summary>
        /// 输出变量：右下角Col
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public double rightBottom_Col = 0;



        /// <summary>
        /// 输出变量：Mark高度
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public double m_fMarkHeight = 0;



        /// <summary>
        /// 输出变量：Mark宽度
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public double m_fMarkWidth = 0;




        /// <summary>
        /// 输出变量：找边是否成功
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public bool bIsFindLineSucess = false;


    }





}

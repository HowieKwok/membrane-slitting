using Frame_Core;
using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.EdgeBurr
{
    /// <summary>
    /// 直边毛刺插件参数
    /// </summary>
    public class ParamsEdgeBurr : PluginParamsBase
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
        /// 输入变量：图像X向像素当量
        /// </summary>
        public double Pix_X = 0.01;
        /// <summary>
        /// 输入变量：图像Y向像素当量
        /// </summary>
        public double Pix_Y = 0.01;

        /// <summary>
        /// 输入变量：边缘灰度阈值
        /// </summary>
        public int hv_EdgeThreshold = 128;

        /// <summary>
        /// 输入变量：边缘平滑系数，只能是奇数。
        /// </summary>
        public int hv_EdgeSmooth = 25;

        /// <summary>
        /// 输入变量：亮毛刺（true）/暗毛刺（false）。
        /// </summary>
        public bool Is_WhiteBurr = true;


        /// <summary>
        /// 输入变量：宽度尺寸筛选下限
        /// </summary>
        public double hv_SelectWidth = 0.03;

        /// <summary>
        /// 输入变量：高度尺寸筛选下限
        /// </summary>
        public double hv_SelectHeight = 0.03;

        /// <summary>
        /// 输入变量：区域面积筛选下限
        /// </summary>
        public double hv_SelectAreaMin = 0.05;
        /// <summary>
        /// 输入变量：区域面积筛选上限
        /// </summary>
        public double hv_SelectAreaMax = 5;

        /// <summary>
        /// 输入变量：圆度筛选下限（扩大100倍）
        /// </summary>
        public int hv_SelectCirMin = 1;
        /// <summary>
        /// 输入变量：圆度筛选上限（扩大100倍）
        /// </summary>
        public int hv_SelectCirMax = 100;


        #endregion


        #region 2.输出变量

        /// <summary>
        /// 输出变量：模块运行成功标志
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public List<NgTypeInfo> List_Defect = new List<NgTypeInfo>();


        /// <summary>
        /// 输出变量：模块运行成功标志
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

        #endregion

        public ParamsEdgeBurr()
        {
            ImageInfo = new ImageInfo();
            HOperatorSet.GenEmptyObj(out ho_ShieldRegion);
            HOperatorSet.GenEmptyObj(out ho_ImageProcessed);

        }
    }
}

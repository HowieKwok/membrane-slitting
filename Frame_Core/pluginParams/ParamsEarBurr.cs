using Frame_Core;
using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    /// <summary>
    /// 直边毛刺插件参数
    /// </summary>
    public class ParamsEarBurr : PluginParamsBase
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
        /// 输入变量：R角缺陷最小面积
        /// </summary>
        public double hv_RminArea = 0.0001;
        /// <summary>
        /// 输入变量：R角缺陷最小宽度
        /// </summary>
        public double hv_RminWidth = 0.01;
        /// <summary>
        /// 输入变量：R角缺陷最小高度
        /// </summary>
        public double hv_RminHeigth = 0.01;

        /// <summary>
        /// 输入变量：顶部缺陷最小面积
        /// </summary>
        public double hv_HminArea = 0.0001;
        /// <summary>
        /// 输入变量：顶部缺陷最小宽度
        /// </summary>
        public double hv_HminWidth = 0.01;
        /// <summary>
        /// 输入变量：顶部缺陷最小高度
        /// </summary>
        public double hv_HminHeigth = 0.01;

        /// <summary>
        /// 输入变量：斜边缺陷最小面积
        /// </summary>
        public double hv_HyminArea = 0.0001;
        /// <summary>
        /// 输入变量：斜边缺陷最小宽度
        /// </summary>
        public double hv_HyminWidth = 0.01;
        /// <summary>
        /// 输入变量：斜边缺陷最小高度
        /// </summary>
        public double hv_HyminHeigth = 0.01;

        /// <summary>
        /// 输入变量：是否是半极耳图，只检测极耳底部R角
        /// </summary>
        public bool Bl_IsHalfEar = false;
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

        public ParamsEarBurr()
        {
            ImageInfo = new ImageInfo();
            HOperatorSet.GenEmptyObj(out ho_ShieldRegion);
            HOperatorSet.GenEmptyObj(out ho_ImageProcessed);

        }
    }
}

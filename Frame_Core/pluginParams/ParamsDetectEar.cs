using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrameCommunity;
using FrameCommunity.Data;
using HalconDotNet;

namespace Frame_Core
{
    [Serializable]
    public class ParamsDetectEar : PluginParamsBase
    {
        //输入变量
        /// <summary>
        /// 输入变量：相机图像队列，图片与ID绑定
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public ImageInfo ImageInfo = new ImageInfo();
        /// <summary>
        /// 输入变量：极耳队列-输入
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public List<PoleEarsInfo> ProPoleList_In = new List<PoleEarsInfo>();


        /// <summary>
        /// 输入变量：标准极耳宽_Y
        /// </summary>
        [Description("标准极耳宽_Y")]
        public double JRSizeY = 200;

        /// <summary>
        /// 输入变量：标准极耳高_X
        /// </summary>
        [Description("标准极耳高_X")]
        public double JRSizeX = 200;

        /// <summary>
        /// 输入变量：标准极耳间距_Y
        /// </summary>
        [Description("标准极耳间距_Y")]
        public double JRSize = 200;

        /// <summary>
        /// 输入变量：极耳间距_等间距
        /// </summary>
        [Description("极耳间距_等间距")]
        public double JRInterval = 300;

        /// <summary>
        /// 输入变量：检测区域起始列1
        /// </summary>
        [Description("检测区域起始列1")]
        public double Column1 = 200;
        /// <summary>
        /// 输入变量：检测区域结束列2
        /// </summary>
        [Description("检测区域结束列2")]
        public double Column2 = 400;


        /// <summary>
        /// 输入变量：找极耳阈值min值
        /// </summary>
        [Description("找极耳阈值min值")]
        public double threshold_Min = 128;

        /// <summary>
        /// 输入变量：找极耳阈值max值
        /// </summary>
        [Description("找极耳阈值max值")]
        public double threshold_Max = 255;


        /// <summary>
        /// 输入变量：极耳宽Y方向比例下限，建议0.3-0.8
        /// </summary>
        [Description("极耳宽Y方向比例下限")]
        public double JRSizeY_PercentMin = 0.7;

        /// <summary>
        /// 输入变量：极耳宽Y方向比例上限，建议1.1-1.3
        /// </summary>
        [Description("极耳宽Y方向比例上限")]
        public double JRSizeY_PercentMax = 1.2;
        /// <summary>
        /// 输入变量：极耳高X方向比例下限，建议0.3-0.8
        /// </summary>
        [Description("极耳高X方向比例下限")]
        public double JRSizeX_PercentMin = 0.7;

        /// <summary>
        /// 输入变量：极耳高X方向比例上限，建议1.1-1.3
        /// </summary>
        [Description("极耳高X方向比例上限")]
        public double JRSizeX_PercentMax = 1.3;


        //输出变量
        /// <summary>
        /// 输出变量：查找极耳的输出,极耳上边坐标
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple JR_RowUp;

        /// <summary>
        /// 输出变量：查找极耳的输出,极耳下边坐标
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple JR_RowDown;

        /// <summary>
        /// 输出变量：查找极耳的输出,极耳下边坐标
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject JR_Region;

        /// <summary>
        /// 输出变量：新加入极耳队列的计数
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public int JRcount_Matched;

        /// <summary>
        /// 极耳队列
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public List<PoleEarsInfo> ProPoleList_Out = new List<PoleEarsInfo>();
        /// <summary>
        /// 输出变量：模块运行成功标志
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public bool bl_ModelRunFlag = false;

        public ParamsDetectEar()
        {
            //HOperatorSet.GenEmptyObj(out ho_Image);
            HOperatorSet.GenEmptyObj(out JR_Region);

        }
    }
}

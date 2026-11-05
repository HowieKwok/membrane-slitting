using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frame_Core;
using FrameCommunity;
using FrameCommunity.Data;
using HalconDotNet;

namespace Frame_Core
{ 
    public class ParamsRectDetectEar : PluginParamsBase
    {
        //输入变量
        /// <summary>
        /// 输入变量：相机图像队列，图片与ID绑定
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public ImageInfo ImageInfo = new ImageInfo();

        /// <summary>
        /// 上一帧图像最后一个矩形灰度阈值结果。
        /// </summary>
        public bool bl_GrayLevel = false;

        /// <summary>
        /// 找料头ROI个数，个数越多，查找越细致 ,必须大于等于5
        /// </summary>
        public int hv_RegionNum = 20;


        /// <summary>
        /// 屏蔽找料头ROI个数，定位料头后预留R角区域
        /// </summary>
        public int hv_RegionShieldNum = 0;
        

        /// <summary>
        /// 输入变量：检测区域起始列1
        /// </summary>
        public int Column1 = 200;
        /// <summary>
        /// 输入变量：检测区域结束列2
        /// </summary>
        public int Column2 = 400;

        /// <summary>
        /// 输入变量：找极耳阈值min值
        /// </summary>
        public int threshold_Min = 128;

        /// <summary>
        /// 输入变量：找极耳阈值max值
        /// </summary>
        public int threshold_Max = 255;


        /// <summary>
        /// 输出变量：留白检测结果：0是直边区，1是极耳区
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public double PreRectHeight = 500;

        /// <summary>
        /// 输出变量：留白检测结果：0是直边区，1是极耳区
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple hv_GrayResult = null;

        /// <summary>
        /// 输出变量：留白检测灰度均值 
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple hv_GrayMean = null;

        /// <summary>
        /// 输出变量：留白检测结果：0是直边区，1是极耳区
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public List<int> listGrayResult_Out = new List<int>();

        /// <summary>
        /// 输出变量：输出粗定位极耳的位置队列
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public List<PoleEarsInfo> ProPoleList_Out = new List<PoleEarsInfo>();

        /// <summary>
        /// 输出变量：输出粗定位极耳的位置队列
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public List<PoleEarsInfo> ProEdgeList_Out = new List<PoleEarsInfo>();

        /// <summary>
        /// 输出变量：模块运行成功标志
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public bool bl_ModelRunFlag = false;

        public ParamsRectDetectEar()
        {
           // HOperatorSet.GenEmptyObj(out ho_DifferRegion);
            //HOperatorSet.GenEmptyObj(out ImageInfo.);
            //HOperatorSet.GenEmptyObj(out JR_Region);

        }
    }
}

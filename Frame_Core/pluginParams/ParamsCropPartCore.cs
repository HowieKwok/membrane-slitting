using Frame_Core;
using HalconDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    [Serializable]
    public class ParamsCropPart : PluginParamsBase
    {
        /// <summary>
        /// 输入变量：相机图像队列，图片与ID绑定
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public List<ImageInfo> list_ImageInfo = new List<ImageInfo>();
        /// <summary>
        /// 输入变量：检测图像的ID，
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public long ImageNowID = 0;

        /// <summary>
        /// 输入变量：区域中心-图像ID：
        /// </summary>
        [Description("区域中心-图像ID")]
        public long PartCenterID = 10;
        /// <summary>
        /// 输入变量：区域中心-行：
        /// </summary>
        [Description("区域中心-行")]
        public double PartCenterRow = 1000;

        /// <summary>
        /// 输入变量：区域中心-列：
        /// </summary>
        [Description("区域中心-列")]
        public double PartCenterCol = 1000;

        /// <summary>
        /// 输入变量：区域图像尺寸-行：
        /// </summary>
        [Description("区域图像尺寸-行")]
        public double PartSizeRow = 300;
        /// <summary>
        /// 输入变量：区域图像尺寸-列：
        /// </summary>
        [Description("区域图像尺寸-列")]
        public double PartSizeCol = 300;

        /// <summary>
        /// 输入变量：图像裁切偏移尺寸-行：
        /// </summary>
        [Description("图像裁切偏移尺寸-行")]
        public double CropShaftRow = 300;
        /// <summary>
        /// 输入变量：图像裁切偏移尺寸-列：
        /// </summary>
        [Description("图像裁切偏移尺寸-列")]
        public double CropShaftCol = 300;

        /// <summary>
        /// 输出变量：裁切图像
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject Image_part;
        /// <summary>
        /// 输出变量：抓料头图像裁图起点坐标ID
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple hv_ImagePartBeginID = 0;

        /// <summary>
        /// 输出变量：抓料头图像裁图起点坐标行
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple hv_ImagePartBeginRow = 0;

        /// <summary>
        /// 输出变量：抓料头图像裁图起点坐标列
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple hv_ImagePartBeginCol = 0;
        /// <summary>
        /// 输出变量：抓料头图像裁图起点坐标ID
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple hv_ImagePartEndID = 0;

        /// <summary>
        /// 输出变量：抓料头图像裁图起点坐标行
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple hv_ImagePartEndRow = 0;

        /// <summary>
        /// 输出变量：抓料头图像裁图起点坐标列
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple hv_ImagePartEndCol = 0;

        public ParamsCropPart()
        {
            HOperatorSet.GenEmptyObj(out Image_part);
            //headLine = 0;
        }
    }
}

using Frame_Core;
using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.CropPart
{
    [Serializable]
    public class SSS_ParamsCropPart : PluginParamsBase
    {
        /// <summary>
        /// 输入变量：相机图像队列，图片与ID绑定
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public List<ImageInfo> list_ImageInfo = new List<ImageInfo>();

        /// <summary>
        /// 输入变量：裁切图名称
        /// </summary>
        public string str_PartName = "未命名";
        /// <summary>
        /// 输入变量：裁切图名称
        /// </summary>
        public long int_PartID = 0;

        /// <summary>
        /// 输入变量：区域中心-图像ID：
        /// </summary>
        public long PartCenterID = 10;
        /// <summary>
        /// 输入变量：区域中心-行：：
        /// </summary>
        public double PartCenterRow = 1000;

        /// <summary>
        /// 输入变量：区域中心-列：
        /// </summary>
        public double PartCenterCol = 1000;

        /// <summary>
        /// 输入变量：区域图像尺寸-行：
        /// </summary>
        public double PartSizeRow = 300;
        /// <summary>
        /// 输入变量：区域图像尺寸-列：
        /// </summary>
        public double PartSizeCol = 300;

        /// <summary>
        /// 输入变量：图像裁切偏移尺寸-行：
        /// </summary>
        public double CropShaftRow = 300;
        /// <summary>
        /// 输入变量：图像裁切偏移尺寸-列：
        /// </summary>
        public double CropShaftCol = 300;

        /// <summary>
        /// 输入变量：是否保存裁切图：
        /// </summary>
        public bool Is_SavePart = true;
        /// <summary>
        /// 输出变量：裁切图像
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject Image_part;
        public SSS_ParamsCropPart()
        {
            HOperatorSet.GenEmptyObj(out Image_part);
            //headLine = 0;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frame_Core;
using HalconDotNet;
using YYSD.UICore.Control;

namespace Frame_Core
{ 
    public class ParamsAIConvertImg : PluginParamsBase
    {
        //  private ImageInfo _imageInfo = new ImageInfo();

        //输入变量
        /// <summary>
        /// 输入变量：相机图像队列，图片与ID绑定
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public ImageInfo ImageInfo=new ImageInfo();
       

        /// <summary>
        /// 输入变量，模型路径
        /// </summary>
        [ShowTextAttribute("输入变量，模型路径")]
        [Description("输入变量，模型路径")]
        public string  tbStrModelPath { get; set; } = $"{System.Environment.CurrentDirectory}\\AIFile11";
     
        /// <summary>
        ///输入变量，模型类型
        /// </summary>
        [ShowTextAttribute("输入变量，模型类型")]
        [Description("输入变量，模型类型")]
        public string tbStrModelType { get; set; } = "Segment";

        /// <summary>
        ///输入变量，模型名字
        /// </summary>
        [ShowTextAttribute("输入变量，模型名字")]
        [Description("输入变量，模型名字")]
        public string tbStrModelName { get; set; } = "dundai";
        /// <summary>
        /// 输入变量，模型数量
        /// </summary>
        [ShowTextAttribute("输入变量，模型数量")]
        [Description("输入变量，模型数量")]
        public int nudIntModelNum { get; set; } = 1;

        /// <summary>
        /// 输入变量：Gpu最大占用率
        /// </summary>
        [ShowTextAttribute("输入变量，Gpu最大占用率")]
        [Description("输入变量，Gpu最大占用率")]
        public double nudFGpuMaxRate { get; set; } = 0.6;

        /// <summary>
        /// 输入变量：图像通道数
        /// </summary>
        [ShowTextAttribute("输入变量，图像通道数")]
        [Description("输入变量，图像通道数")]
        public int nudIntImgChannel { get; set; } = 3;

        /// <summary>
        /// 输入变量：最小阈值
        /// </summary>
        [ShowTextAttribute("输入变量，最小阈值")]
        [Description("输入变量，最小阈值")]
        public int nudIntThresholMin { get; set; } = 3;

        /// <summary>
        /// 输入变量：最大阈值
        /// </summary>
        [ShowTextAttribute("输入变量，最大阈值")]
        [Description("输入变量，最大阈值")]
        public int nudIntThresholMax { get; set; } = 255;

        /// <summary>
        /// 输入变量：最大阈值
        /// </summary>
        [ShowTextAttribute("输入变量，掩膜颜色通道1")]
        [Description("输入变量，掩膜颜色通道1")]
        public int nudIntColorR { get; set; } = 255;


        /// <summary>
        /// 输入变量：最大阈值
        /// </summary>
        [ShowTextAttribute("输入变量，掩膜颜色通道2")]
        [Description("输入变量，掩膜颜色通道2")]
        public int nudIntColorG { get; set; } = 255;

        /// <summary>
        /// 输入变量：最大阈值
        /// </summary>
        [ShowTextAttribute("输入变量，掩膜颜色通道3")]
        [Description("输入变量，掩膜颜色通道3")]
        public int nudIntColorB { get; set; } = 255;
        /// <summary>
        /// 输出变量：已转换的图像
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public ImageInfo outImageInfo = new ImageInfo();


        /// <summary>
        /// 输出变量：已转换的Region
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject outConvertRegion ;

        public ParamsAIConvertImg()
        {
           // HOperatorSet.GenEmptyObj(out ho_DifferRegion);
            //HOperatorSet.GenEmptyObj(out ImageInfo.);
            //HOperatorSet.GenEmptyObj(out JR_Region);

        }
    }
}

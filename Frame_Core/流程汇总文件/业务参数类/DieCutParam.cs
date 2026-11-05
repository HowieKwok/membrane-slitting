using Extensions;
using Frame_Core.流程汇总文件.产品类;
using FrameCommunity.Data;
using HalconDotNet;
using LogTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core.业务参数类
{
    /// <summary>
    /// 模切业务参数
    /// </summary>
    public class DieCutParam :BaseProParam
    {
        public DieCutParam(string cameraName):base(cameraName)
        {
           
        }

        /// <summary>
        /// 像素头部绝对位置
        /// </summary>
        public long nFristAbsPosPixel = 0;

        /// <summary>
        /// 像素尾部绝对位置
        /// </summary>
        public long nLastAbsPosPixel = 0;


        public List<DieCutEAProduct> EAProducts { get; set; } = new List<DieCutEAProduct>();

        public double nLastCutLine = -1;

        /// <summary>
        /// 相机图像队列，用于传入插件，list
        /// </summary>
        public List<ImageInfo> imageInfos { get; set; } = new List<ImageInfo>();

        /// <summary>
        /// 相机EA图像队列，用于EA检测
        /// </summary>
        public List<ImageInfo> imageEAInfos { get; set; } = new List<ImageInfo>();


        /// <summary>
        /// 输出裁图分界线
        /// </summary>
        public List<LineInfo> BufferSplitLines = new List<LineInfo>();

        /// <summary>
        /// EA计数索引
        /// </summary>
        public long nEAIndex = 1;

        public override void Dispose()
        {
            foreach(var item in imageInfos)
            {
                item?.Dispose();
            }
            imageInfos.Clear();
            foreach (var item in imageEAInfos)
            {
                item?.Dispose();
            }
            imageEAInfos.Clear();
            BufferSplitLines?.Clear();

            base.Dispose();
        }


       
    }
}

using FrameCommunity.Halocn二次封装;
using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameCommunity.Data
{
    public class ImageInfo : IDisposable
    {
        public HObject ho_image = null;
        public long image_ID = -1;
        public string cameraName = "default Camera";
        public double imageWidth = 16384;//原图高
        public double imageHeight = 10000;//原图宽

        /// <summary>
        /// 0:正常 1：EA失败 2:首片NG
        /// </summary>
        public int NgType = 0;
        /// <summary>
        /// 图片信息描述
        /// </summary>
        public string ImageDescription = "";

        /// <summary>
        /// 首行脉冲计数
        /// </summary>
        public long nFirstLineEncoderCount = 0;
        /// <summary>
        /// 尾行脉冲计数
        /// </summary>
        public long nLastLineEncoderCount = 0;

        public long EAID = 0;



        public void Dispose()
        {
            ho_image?.DisposeEx();
            ho_image = null;
        }
    }

    public abstract class ImageOtherInfoBase
    {

    }
}

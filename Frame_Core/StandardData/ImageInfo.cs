using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    public class ImageInfo : IDisposable
    {
        public HObject ho_image = null;
        public long image_ID = -1;
        public string cameraName = "default Camera";
        public double imageWidth = 16384;//原图高
        public double imageHeight = 10000;//原图宽

        public int EAID = 0;

        /// <summary>
        /// 其他信息
        /// </summary>
        public OtherInfo otherInfo = new OtherInfo();

        public void Dispose()
        {
            ho_image?.DisposeEx();
            ho_image = null;
        }
    }

    public class OtherInfo
    {

        /// <summary>
        /// 第几个电芯包
        /// </summary>
        public int PackgeNum = 0;

        /// <summary>
        /// 电芯包编号CCD自己的编号
        /// </summary>
        public int PackgeNum_FromCCD = 0;

        /// <summary>
        /// 当前层数
        /// </summary>
        public int CurrentLayer = 0;

        /// <summary>
        /// 当前层数
        /// </summary>
        public int CurrentLayer_FromCCD = 0;

        /// <summary>
        /// 归属工位  1为A叠台  2为B叠台
        /// </summary>
        public int AffiliationStation = 0;

        /// <summary>
        /// 上Tabneck值
        /// </summary>
        public double UpTabneckValue = 0;

        /// <summary>
        /// 下Tabneck值
        /// </summary>
        public double DownTabneckValue = 0;

        public double PoleWidth1 = -999;
        public double PoleWidth2 = -999;
        public double PoleHeight1 = -999;
        public double PoleHeight2 = -999;
        public double PoleEarW = -999;
        public double PoleEarH = -999;
        public double PoleEarD = -999;


        /// <summary>
        /// 开始信号
        /// </summary>
        public bool startFlag = false;

        /// <summary>
        /// 结束信号
        /// </summary>
        public bool endFlag = false;
    }

    public class CropImageInfo : IDisposable
    {
        public HObject ho_image = null;

        public long image_ID = -1;
        public string ImageName = "default Camera";
        public double imageWidth = 16384;//原图高
        public double imageHeight = 10000;//原图宽

        public int StartImageID = 0;
        public int StarImageRow = 0;

        public int EndImageID_ = 0;
        public int EndIImageRow = 0;

        public void Dispose()
        {
            ho_image?.Dispose();
            ho_image = null;

        }
    }

}

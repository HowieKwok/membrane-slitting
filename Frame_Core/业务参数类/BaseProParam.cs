using HalconDotNet;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    public class BaseProParam : IDisposable
    {
        /// <summary>
        /// 相机枚举
        /// </summary>
        public CameraEnum cameraEnum = CameraEnum.Cam1;
        /// <summary>
        /// 相机图像队列
        /// </summary>
        public List<ImageInfo> m_listImageInfo_Cam = new List<ImageInfo>();
        public QueuePackage<ImageInfo> QueueGrabImage = new QueuePackage<ImageInfo>();
        public bool bIs3D = false;
        /// <summary>
        /// 用于3D图像队列
        /// </summary>
        public QueuePackage<Image3DClass> Queue3DImage = new QueuePackage<Image3DClass>();
        /// <summary>
        /// 已经处理的图像队列
        /// </summary>
        public QueuePackage<ImageInfo> alreadyDealGrabImage = new QueuePackage<ImageInfo>();
        public QueuePackage<Image3DClass> alreadyDealGrabImage3D = new QueuePackage<Image3DClass>();


        public Action<ProductBase> ProductCallBack; //原相机图像单帧回调 -预处理
        public Action<ProductBase> ProductCallBackCraft; //EA图像单帧回调 - 工艺流程


        public delegate void AutoCalibration(CameraEnum camName, HObject ho_image); //自动标定委托
        public AutoCalibration AutoCalibrationDelegate;//自动标定委托
        public object m_lImgInfolock = new object();
        public virtual void Dispose()
        {
            while (QueueGrabImage.Count > 0)
            {
                QueueGrabImage.Dequeue()?.Dispose();
            }

            while (alreadyDealGrabImage.Count > 0)
            {
                alreadyDealGrabImage.Dequeue()?.Dispose();
            }
            if (m_listImageInfo_Cam.Count() > 0)
            {
                foreach (var item in m_listImageInfo_Cam)
                {
                    item?.Dispose();
                }
            }
            m_listImageInfo_Cam.Clear();

            while (Queue3DImage.Count > 0)
            {
                Queue3DImage.Dequeue().Dispose();
            }
            // throw new NotImplementedException();
        }

        /// <summary>
        /// 回调数据处理
        /// </summary>
        public virtual void CallBackImgDataDeal() { }

    }
}

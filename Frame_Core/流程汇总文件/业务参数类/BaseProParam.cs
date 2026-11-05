using FrameCommunity;
using FrameCommunity.Data;
using HalconDotNet;
using LogTool;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Frame_Core
{
    public class BaseProParam : IDisposable
    {

        private static readonly ConcurrentDictionary<int, SemaphoreSlim> camSemaphoreMap = new ConcurrentDictionary<int, SemaphoreSlim>();
        /// <summary>
        /// 原始图像信号
        /// </summary>
        public SemaphoreSlim semaphoreSlimCam => camSemaphoreMap.GetOrAdd(_camID, _ => new SemaphoreSlim(0, 100));

        public readonly int _camID;


        /// <summary>
        /// 相机的图像ID，默认第一片为1，预处理使用
        /// </summary>
        public int nCurProImgId = 1;
        /// <summary>
        /// EA计数索引
        /// </summary>
        public int nEAId = 1;

        /// <summary>
        /// 相机名字
        /// </summary>
        public string strCamreaName { get; }

        private static readonly ConcurrentDictionary<int, SemaphoreSlim> camSemaphoreMapSlim = new ConcurrentDictionary<int, SemaphoreSlim>();
        

        /// <summary>
        /// 裁图EA信号
        /// </summary>
        public SemaphoreSlim semaphoreSlim => camSemaphoreMapSlim.GetOrAdd(_camID, _ => new SemaphoreSlim(0, 100));




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

        /// <summary>
        /// 销毁的图像数
        /// </summary>
        public int disposeImageCount = 0;

        public BaseProParam(string cameraName)
        {
            strCamreaName = cameraName;
            Enum.TryParse(cameraName, out CameraEnum camEnum);
            _camID = (int)camEnum;
        }


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

            nCurProImgId = 1;
            nEAId = 1;
            // throw new NotImplementedException();
        }

        /// <summary>
        /// 回调数据处理
        /// </summary>
        public virtual void CallBackImgDataDeal()
        {


            if (QueueGrabImage.Count > 0)
            {
                if (!Project.Instance.GetStates() && !Project.Instance.Is_simulation)
                {
                    ImageInfo imageInfo = QueueGrabImage.Dequeue();
                    disposeImageCount++;
                    Log4netRecord.RunLog.Debug(strCamreaName + "销毁回调的图像，图像帧号为:" + imageInfo.image_ID + "\n" + "销毁的图像数：" + disposeImageCount);
                    imageInfo.Dispose();
                }
                else
                {
                    disposeImageCount = 0;
                    ImageInfo InfoImgInfo = QueueGrabImage.Dequeue();
                    try
                    {
                        if (InfoImgInfo != null)
                        {
                            if (InfoImgInfo.image_ID != 0)
                            {
                                InfoImgInfo.image_ID = nCurProImgId;
                                alreadyDealGrabImage.Enqueue(InfoImgInfo);
                                nCurProImgId++;
                                semaphoreSlimCam.Release();
                            }
                        }
                        else
                        {
                            MessageBox.Show(strCamreaName + "图像来源为空");
                        }

                    }
                    catch (Exception ee)
                    {

                        MessageBox.Show(strCamreaName + "图像来源为空:" + ee.ToString());
                    }



                }

            }




        }

    }
}

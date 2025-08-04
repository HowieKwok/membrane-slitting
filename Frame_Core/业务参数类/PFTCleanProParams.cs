using DotNetDetour;

using Extensions;

using HalconDotNet;

using HslCommunication.Core.Net;
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
    public class PFTCleanProParams : BaseProParam
    {
        /// <summary>
        /// 相机图片检测信号量
        /// </summary>
        public SemaphoreSlim semaphoreSlimCam = new SemaphoreSlim(0, 100);
        /// <summary>
        /// 局部裁切图片检测信号量。
        /// </summary>
        public SemaphoreSlim semaphoreSlim = new SemaphoreSlim(0, 100);

        /// <summary>
        /// 相机图像队列，用于传入插件，list
        /// </summary>
        public List<ImageInfo> imageInfos = new List<ImageInfo>();

        /// <summary>
        /// 相机图像队列，用于传入插件，list
        /// </summary>
        public List<ImageInfo> EAlistimageInfos = new List<ImageInfo>();

        /// <summary>
        /// 相机名字
        /// </summary>
        private string strCamreaName { get; }

        /// <summary>
        /// 相机的图像ID，默认第一片为1，预处理使用
        /// </summary>
        public int nCurProImgId = 1;

        /// <summary>
        /// 当前产品EAID
        /// </summary>
        public int nCurEAProId = 0;

        /// <summary>
        /// 当前产品EAID
        /// </summary>
        public int nCurShowEAProId = 0;

        //产品参数
        public ConcurrentDictionary<int, PFTCleanEAProductInfo> dic_EAproductsInfo = new ConcurrentDictionary<int, PFTCleanEAProductInfo>();

        /// <summary>
        /// 遍历槽位用到的队列偏移量
        /// </summary>
        public int SlotCountBias = 0;
        /// <summary>
        /// 遍历削薄区用到的队列偏移量
        /// </summary>
        public int ShaveCountBias = 0;

        /// <summary>
        /// 销毁的图像数
        /// </summary>
        int disposeImageCount = 0;

        /// <summary>
        /// EA拼大图流程。每帧之后传递图像和图像结果，用于EA拼图
        /// </summary>
        public QueuePackage<ImageInfo> PFTImageProdQueue = new QueuePackage<ImageInfo>();

        /// <summary>
        /// 相机图像队列，用于传入裁拼图插件，list
        /// </summary>
        public List<ImageInfo> EAimageInfos = new List<ImageInfo>();

        public PFTCleanProParams(string cameraName)
        {
            strCamreaName = cameraName;
        }
        public override void CallBackImgDataDeal()
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


            if (Queue3DImage.Count > 0)
            {
                if (!Project.Instance.GetStates() && !Project.Instance.Is_simulation)
                {
                    Image3DClass image3DClass = Queue3DImage.Dequeue();
                    disposeImageCount++;
                    Log4netRecord.RunLog.Debug(strCamreaName + "销毁回调的图像，图像帧号为:" + image3DClass.image_ID + "\n" + "销毁的图像数：" + disposeImageCount);
                    image3DClass?.Dispose();
                }
                else
                {
                    disposeImageCount = 0;
                    Image3DClass InfoImgInfo = Queue3DImage.Dequeue();
                    Image3DClass image3DClass = new Image3DClass();
                    image3DClass.image_ID = InfoImgInfo.image_ID;
                    InfoImgInfo.Image_Light.CopyImage(out image3DClass.Image_Light);
                    InfoImgInfo.Image3D.CopyImage(out image3DClass.Image3D);
                    InfoImgInfo.Image2D.CopyImage(out image3DClass.Image2D);
                    image3DClass.Info3D = InfoImgInfo.Info3D.DeepClone();
                    image3DClass.Info= InfoImgInfo.Info.DeepClone();
                    InfoImgInfo.Dispose();
                    try
                    {
                        if (image3DClass != null)
                        {
                            if (image3DClass.image_ID != 0)
                            {
                                image3DClass.image_ID = nCurProImgId;
                                alreadyDealGrabImage3D.Enqueue(image3DClass);
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

        public override void Dispose()
        {
            nCurProImgId = 1;
            disposeImageCount = 0;
            nCurEAProId = 0;
            nCurShowEAProId = 0;
            base.Dispose();
            int ct = semaphoreSlimCam.CurrentCount;
            for (int i = 0; i < ct; i++)
            {
                semaphoreSlimCam.Wait();
            }
            int ct1 = semaphoreSlim.CurrentCount;
            for (int i = 0; i < ct1; i++)
            {
                semaphoreSlimCam.Wait();
            }

            foreach (var item in imageInfos)
            {
                item?.Dispose();
            }
            imageInfos.Clear();
            foreach (var item in dic_EAproductsInfo)
            {
                item.Value.Dispose();
            }
            dic_EAproductsInfo.Clear();
        }

    }
}

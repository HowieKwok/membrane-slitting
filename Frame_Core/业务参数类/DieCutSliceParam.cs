using LogTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Frame_Core
{
    public class DieCutSliceParam : BaseProParam
    {
        public SemaphoreSlim semaphoreSlimCam = new SemaphoreSlim(0, 100);

        /// <summary>
        /// 相机图像队列，用于传入插件，list
        /// </summary>
        public List<ImageInfo> imageInfos = new List<ImageInfo>();

        /// <summary>
        /// EA条码
        /// </summary>
        public string strEaCode = "";

        /// <summary>
        /// 相机名字
        /// </summary>
        private string strCamreaName { get; }

        /// <summary>
        /// 相机的图像ID，默认第一片为1，预处理使用
        /// </summary>
        public long nCurProImgId = 1;

        /// <summary>
        /// 销毁的图像数
        /// </summary>
        int disposeImageCount = 0;

        public int CurJRID = 0;

        /// <summary>
        /// 过程变量：用于维护 用于定义相机裁图基准位置，方便裁图防呆
        /// </summary>
        public ListPackage<PoleEarsInfo> m_poleEarsInfos = new ListPackage<PoleEarsInfo>();

        /// <summary>
        /// 输入变量：极耳模块查找极耳使用，预处理找极耳分EA 。
        /// </summary>
        public List<PoleEarsInfo> m_poleEarsInfos_In = new List<PoleEarsInfo>();

        /// <summary>
        /// 输出变量：接收查找极耳插件输出的极耳信息。
        /// </summary>
        public List<PoleEarsInfo> PoleEarsInfos_Out = new List<PoleEarsInfo>();


        /// <summary>
        /// 当前相机流程，已经裁切图像的ID。
        /// </summary>
        public int CurCropJRID = 0;
        /// <summary>
        /// 裁切单片EA图像队列。
        /// </summary>
        public QueuePackage<CropImgParam> cropImgParamQueue = new QueuePackage<CropImgParam>();
        /// <summary>
        /// 裁切单片EA检测信号量。
        /// </summary>
        public SemaphoreSlim semaphoreSlim = new SemaphoreSlim(0, 100);



        /// <summary>
        /// 相机预处理数据，用于查找裁片使用，不满足裁片条件的数据先缓存等待下次使用
        /// </summary>
        // public List<PoleEarsInfo> m_bufferPoleEarsInfos = new List<PoleEarsInfo>();


        public PoleEarsInfo bufferCropPos = new PoleEarsInfo();


        //public long nProducrtID = 0;



        public DieCutSliceParam(string cameraName)
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

        }


        public override void Dispose()
        {
            nCurProImgId = 1;
            disposeImageCount = 0;
            CurJRID = 0;
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

            foreach (var item in m_poleEarsInfos)
            {
                item.Image_JRpart?.Dispose();
            }
            m_poleEarsInfos?.Clear();

            foreach (var item in m_poleEarsInfos_In)
            {
                item.Image_JRpart?.Dispose();
            }
            m_poleEarsInfos_In?.Clear();
            foreach (var item in PoleEarsInfos_Out)
            {
                item.Image_JRpart?.Dispose();
            }
            PoleEarsInfos_Out?.Clear();



            while (cropImgParamQueue.Count > 0)
            {
                cropImgParamQueue.Dequeue()?.Dispose();
            }
            bufferCropPos.Image_JRpart?.Dispose();
        }


    }

    public class CropImgParam : IDisposable
    {
        public string strCamreaName = "";
        public long nCurFrameImgId { get; set; } = 0;
        public long nProducrtID { get; set; } = 0;
        public long nRealativeProductID = 0;
        public long nRealativeFrameImgID = 0;
        public ImageInfo imageInfo { get; set; } = new ImageInfo();
        public void Dispose()
        {
            imageInfo?.Dispose();
        }
    }
}

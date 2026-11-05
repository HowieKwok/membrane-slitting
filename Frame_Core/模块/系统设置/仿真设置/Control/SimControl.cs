using FrameCommunity;
using FrameCommunity.Data;
using HalconDotNet;

using LogTool;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    public class SimControl
    {
        public static void SimRun(RunMode runMode, ImageSimManage imageSimManage, ref QueuePackage<ImageInfo> queueGrabImage_Sim, string sourc)
        {
            switch (runMode)
            {
                case RunMode.None:

                    break;
                case RunMode.RunOnce:
                    LoadImage_Sim(imageSimManage, ref queueGrabImage_Sim, sourc);
                    break;
                case RunMode.RunCycle:
                    LoadImage_Sim(imageSimManage, ref queueGrabImage_Sim, sourc);
                    break;
            }
        }

        public static bool LoadPrevImage_Sim(ImageSimManage imageSimManage, ref QueuePackage<ImageInfo> queueGrabImage_Sim, string source)
        {
            bool readRsult = false;
            try
            {
                ImageInfo image = new ImageInfo();
                readRsult = imageSimManage.GetPreviewImage(out HObject tempImage, out string imgPath);          
                if (readRsult && image != null && tempImage != null && tempImage.IsInitialized() && tempImage.CountObj() > 0)
                {
                    image.ho_image = tempImage;
                    HOperatorSet.GetImageSize(image.ho_image, out HTuple width, out HTuple height);
                    image.imageWidth = width.I;
                    image.imageHeight = height.I;
                    image.image_ID = ++imageSimManage.currentImageID;
                    imageSimManage.CamCallbackStartEvent?.Invoke(image.image_ID);
                    queueGrabImage_Sim.Enqueue(image);
                
                    readRsult = true;
                }
                else
                {
                    if (image != null) image.Dispose();
                    readRsult = false;
                }
            }
            catch (Exception ex)
            {
                readRsult = false;
                Log4netRecord.ErrorLog.Error("加载仿真图像报错" + $"来源：{source}", ex);
            }
            return readRsult;
        }

        public static bool LoadNextImage_Sim(ImageSimManage imageSimManage, ref QueuePackage<ImageInfo> queueGrabImage_Sim, string source)
        {
            bool readRsult = false;
            try
            {
                ImageInfo image = new ImageInfo();
                readRsult = imageSimManage.GetNextImage(out HObject tempImage, out string imgPath);
                if (readRsult && image != null && tempImage != null && tempImage.IsInitialized() && tempImage.CountObj() > 0)
                {
                    image.ho_image = tempImage;
                    HOperatorSet.GetImageSize(image.ho_image, out HTuple width, out HTuple height);
                    image.imageWidth = width.I;
                    image.imageHeight = height.I;
                    image.image_ID = ++imageSimManage.currentImageID;
                    imageSimManage.CamCallbackStartEvent?.Invoke(image.image_ID);
                    queueGrabImage_Sim.Enqueue(image);
                
                    readRsult = true;
                }
                else
                {
                    if (image != null) image.Dispose();
                    readRsult = false;
                }
            }
            catch (Exception ex)
            {
                readRsult = false;
                Log4netRecord.ErrorLog.Error("加载仿真图像报错" + $"来源：{source}", ex);
            }
            return readRsult;
        }

        public static bool LoadImage_Sim(ImageSimManage imageSimManage, ref QueuePackage<ImageInfo> queueGrabImage_Sim, string source)
        {
            bool readRsult = false;
            try
            {
                ImageInfo image = new ImageInfo();
                readRsult = imageSimManage.GetZeroIndexStartNextImage(out HObject tempImage,out string imgPath);
                if (readRsult && image != null && tempImage != null && tempImage.IsInitialized() && tempImage.CountObj() > 0)
                {
                    image.ho_image = tempImage;
                    HOperatorSet.GetImageSize(image.ho_image, out HTuple width, out HTuple height);
                    image.imageWidth = width.I;
                    image.imageHeight = height.I;
                    image.image_ID = ++imageSimManage.currentImageID;
                    imageSimManage.CamCallbackStartEvent?.Invoke(image.image_ID);
                    queueGrabImage_Sim.Enqueue(image);

                    readRsult = true;
                }
                else
                {
                    if (image != null) image.Dispose();
                    readRsult = false;
                }
            }
            catch (Exception ex)
            {
                readRsult = false;
                Log4netRecord.ErrorLog.Error("加载仿真图像报错" + $"来源：{source}", ex);
            }
            return readRsult;
        }


        public static bool LoadImage_Sim3D(ImageSimManage imageSimManage, ref QueuePackage<Image3DClass> queueGrabImage_Sim, string source)
        {
            Image3DClass image3DClass = null;
            bool readRsult = false;
            try
            {

                readRsult = imageSimManage.GetZeroIndexStartNextImage3D(out image3DClass);
                if (readRsult && image3DClass != null)
                {
                    image3DClass.image_ID = ++imageSimManage.currentImageID;
                    queueGrabImage_Sim.Enqueue(image3DClass);
                    readRsult = true;
                }
                else
                {
                    if (image3DClass != null) image3DClass.Dispose();
                    readRsult = false;
                }
            }
            catch (Exception ex)
            {
                readRsult = false;
                Log4netRecord.ErrorLog.Error("加载仿真图像报错" + $"来源：{source}", ex);
            }

            return readRsult;
        }

        public static bool LoadImage_Sim(ImageSimManage imageSimManage, out ImageInfo imageInfo, string source)
        {
            imageInfo = null;
            bool readRsult = false;
            try
            {
                ImageInfo image = new ImageInfo();
                readRsult = imageSimManage.GetZeroIndexStartNextImage(out HObject tempImage, out string imgPath);
                if (readRsult && image != null && tempImage != null && tempImage.IsInitialized() && tempImage.CountObj() > 0)
                {
                    image.ho_image = tempImage;
                    HOperatorSet.GetImageSize(image.ho_image, out HTuple width, out HTuple height);
                    image.imageWidth = width.I;
                    image.imageHeight = height.I;
                    image.image_ID = ++imageSimManage.currentImageID;
                    imageInfo = image;

                    readRsult = true;
                }
                else
                {
                    if (image != null) image.Dispose();
                    readRsult = false;
                }
            }
            catch (Exception ex)
            {
                readRsult = false;
                Log4netRecord.ErrorLog.Error("加载仿真图像报错" + $"来源：{source}", ex);
            }
            return readRsult;
        }
    }
}

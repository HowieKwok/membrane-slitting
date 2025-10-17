using HalconDotNet;
using LogTool;
using NPOI.OpenXmlFormats.Vml;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EHolly.ImageTool;
using BitMiracle.LibTiff.Classic;
using Basler.Pylon;


namespace Frame_Core
{
    /// <summary>
    /// 保存图片执行类
    /// </summary>
    public class SaveImageTool
    {
        /// <summary>
        /// 保存图像
        /// </summary>
        /// <param name="image"></param>
        /// <param name="imageSaveFormat"></param>
        /// <param name="saveDirectoryPath"></param>
        /// <param name="saveFileName"></param>
        /// <param name="isDisposeImage"></param>
        /// <returns></returns>
        public static ExecuteResult SaveImage(HObject image, ImageSaveFormat imageSaveFormat, string saveDirectoryPath, string saveFileName, bool isDisposeImage = false)
        {
            ExecuteResult executeResult = new ExecuteResult();
            try
            {
                if (!Directory.Exists(saveDirectoryPath))
                    Directory.CreateDirectory(saveDirectoryPath);

                string savePath = saveDirectoryPath + "\\" + saveFileName;

                if (image == null)
                {
                    executeResult.Message = "图像对象为空";
                    return executeResult;
                }

                if (!image.IsInitialized())
                {
                    executeResult.Message = "图像未初始化";
                    return executeResult;
                }

                if (image?.CountObj() == 0 || image?.CountObj() == null)
                {
                    executeResult.Message = "图像对象数量为0";
                    return executeResult;
                }

                switch (imageSaveFormat)
                {
                    case ImageSaveFormat.BMP:
                        EHollyImageTool.SaveImage(image, "bmp", 0, savePath, FileSaveKind.BigImage, DisponseHObject);
                       // HOperatorSet.WriteImage(image, "bmp", 0, savePath);

                        break;
                    case ImageSaveFormat.JPEG20:
                        EHollyImageTool.SaveImage(image, "jpeg 20", 0, savePath, FileSaveKind.BigImage, DisponseHObject);
                        //HOperatorSet.WriteImage(image, "jpeg 20", 0, savePath);
                        break;
                    case ImageSaveFormat.JPEG40:
                        EHollyImageTool.SaveImage(image, "jpeg 40", 0, savePath, FileSaveKind.BigImage, DisponseHObject);
                        // HOperatorSet.WriteImage(image, "jpeg 40", 0, savePath);
                        break;

                    case ImageSaveFormat.TIF:
                        EHollyImageTool.SaveImage(image, "tiff", 0, savePath, FileSaveKind.BigImage, DisponseHObject);
                        //HOperatorSet.WriteImage(image, "tiff", 0, savePath);
                        break;

                    case ImageSaveFormat.TIFCompress:
                        EHollyImageTool.SaveImage(image, "tiff deflate 1", 0, savePath, FileSaveKind.BigImage, DisponseHObject);
                        // HOperatorSet.WriteImage(image, "tiff deflate 1", 0, savePath);
                        break;
                    //case ImageSaveFormat.JPEG60:
                    //    HOperatorSet.WriteImage(image, "jpeg 60", 0, savePath + ".jpeg");
                    //    break;
                    //case ImageSaveFormat.JPEG80:
                    //    HOperatorSet.WriteImage(image, "jpeg 80", 0, savePath + ".jpeg");
                    //    break;
                    //case ImageSaveFormat.JPEG100:
                    //    HOperatorSet.WriteImage(image, "jpeg 100", 0, savePath + ".jpeg");
                    //    break;
                    default:
                        EHollyImageTool.SaveImage(image, "jpeg 20", 0, savePath, FileSaveKind.BigImage, DisponseHObject);
                        //HOperatorSet.WriteImage(image, "jpeg 20", 0, savePath);
                        break;
                }
                executeResult.IsSuccess = true;
            }
            catch (Exception ex)
            {
                executeResult.Message = ex.ToString();
                executeResult.IsSuccess = false;
            }
            finally
            {
                if (isDisposeImage)
                {
                    image?.Dispose();
                }
            }
            return executeResult;

        }

        /// <summary>
        /// 保存图像
        /// </summary>
        /// <param name="image"></param>
        /// <param name="imageSaveFormat"></param>
        /// <param name="saveDirectoryPath"></param>
        /// <param name="saveFileName"></param>
        /// <param name="isDisposeImage"></param>
        /// <returns></returns>
        public static ExecuteResult SaveImageQue(HObject image,  Operation_ImageInfo imageInfo, QueuePackage<Operation_ImageInfo> target_DisposeQueue, ImageSaveFormat imageSaveFormat, string saveDirectoryPath, string saveFileName, bool isDisposeImage = false)
        {
            ExecuteResult executeResult = new ExecuteResult();
            Action<HObject> emptyAction = _ => target_DisposeQueue.Enqueue(imageInfo);
            try
            {
                if (!Directory.Exists(saveDirectoryPath))
                    Directory.CreateDirectory(saveDirectoryPath);

                string savePath = saveDirectoryPath + "\\" + saveFileName;

                if (image == null)
                {
                    executeResult.Message = "图像对象为空";
                    return executeResult;
                }

                if (!image.IsInitialized())
                {
                    executeResult.Message = "图像未初始化";
                    return executeResult;
                }

                if (image?.CountObj() == 0 || image?.CountObj() == null)
                {
                    executeResult.Message = "图像对象数量为0";
                    return executeResult;
                }

                switch (imageSaveFormat)
                {
                    case ImageSaveFormat.BMP:
                       
                        EHollyImageTool.SaveImage(image, "bmp", 0, savePath, FileSaveKind.BigImage, emptyAction);
                        // HOperatorSet.WriteImage(image, "bmp", 0, savePath);

                        break;
                    case ImageSaveFormat.JPEG20:
                      
                        EHollyImageTool.SaveImage(image, "jpeg 20", 0, savePath, FileSaveKind.BigImage, emptyAction);
                        //HOperatorSet.WriteImage(image, "jpeg 20", 0, savePath);
                        break;
                    case ImageSaveFormat.JPEG40:
                       
                        EHollyImageTool.SaveImage(image, "jpeg 40", 0, savePath, FileSaveKind.BigImage, emptyAction);
                        // HOperatorSet.WriteImage(image, "jpeg 40", 0, savePath);
                        break;

                    case ImageSaveFormat.TIF:
                       
                        EHollyImageTool.SaveImage(image, "tiff", 0, savePath, FileSaveKind.BigImage, emptyAction);
                        //HOperatorSet.WriteImage(image, "tiff", 0, savePath);
                        break;

                    case ImageSaveFormat.TIFCompress:
                       
                        EHollyImageTool.SaveImage(image, "tiff deflate 1", 0, savePath, FileSaveKind.BigImage, emptyAction);
                        // HOperatorSet.WriteImage(image, "tiff deflate 1", 0, savePath);
                        break;
                    //case ImageSaveFormat.JPEG60:
                    //    HOperatorSet.WriteImage(image, "jpeg 60", 0, savePath + ".jpeg");
                    //    break;
                    //case ImageSaveFormat.JPEG80:
                    //    HOperatorSet.WriteImage(image, "jpeg 80", 0, savePath + ".jpeg");
                    //    break;
                    //case ImageSaveFormat.JPEG100:
                    //    HOperatorSet.WriteImage(image, "jpeg 100", 0, savePath + ".jpeg");
                    //    break;
                    default:
                      
                        EHollyImageTool.SaveImage(image, "jpeg 20", 0, savePath, FileSaveKind.BigImage, emptyAction);
                        //HOperatorSet.WriteImage(image, "jpeg 20", 0, savePath);
                        break;
                }
                executeResult.IsSuccess = true;
            }
            catch (Exception ex)
            {
                executeResult.Message = ex.ToString();
                executeResult.IsSuccess = false;
            }
            finally
            {
                if (isDisposeImage)
                {
                    image?.Dispose();
                }
            }
            return executeResult;

        }






        /// <summary>
        /// 保存图像
        /// </summary>
        /// <param name="image"></param>
        /// <param name="imageSaveFormat"></param>
        /// <param name="saveDirectoryPath"></param>
        /// <param name="saveFileName"></param>
        /// <param name="isDisposeImage"></param>
        /// <returns></returns>
        public static ExecuteResult SaveImageInit(HObject image, ImageSaveFormat imageSaveFormat, string saveDirectoryPath, string saveFileName, bool isDisposeImage = false)
        {
            ExecuteResult executeResult = new ExecuteResult();
            try
            {
                if (!Directory.Exists(saveDirectoryPath))
                    Directory.CreateDirectory(saveDirectoryPath);

                string savePath = saveDirectoryPath + "\\" + saveFileName;

                if (image == null)
                {
                    executeResult.Message = "图像对象为空";
                    return executeResult;
                }

                if (!image.IsInitialized())
                {
                    executeResult.Message = "图像未初始化";
                    return executeResult;
                }

                if (image?.CountObj() == 0 || image?.CountObj() == null)
                {
                    executeResult.Message = "图像对象数量为0";
                    return executeResult;
                }

                switch (imageSaveFormat)
                {
                    case ImageSaveFormat.BMP:
                        //EHollyImageTool.SaveImage(image, "bmp", 0, savePath, FileSaveKind.BigImage, DisponseHObject);
                         HOperatorSet.WriteImage(image, "bmp", 0, savePath);

                        break;
                    case ImageSaveFormat.JPEG20:
                        //EHollyImageTool.SaveImage(image, "jpeg 20", 0, savePath, FileSaveKind.BigImage, DisponseHObject);
                        HOperatorSet.WriteImage(image, "jpeg 20", 0, savePath);
                        break;
                    case ImageSaveFormat.JPEG40:
                       // EHollyImageTool.SaveImage(image, "jpeg 40", 0, savePath, FileSaveKind.BigImage, DisponseHObject);
                         HOperatorSet.WriteImage(image, "jpeg 40", 0, savePath);
                        break;

                    case ImageSaveFormat.TIF:
                        //EHollyImageTool.SaveImage(image, "tiff", 0, savePath, FileSaveKind.BigImage, DisponseHObject);
                        HOperatorSet.WriteImage(image, "tiff", 0, savePath);
                        break;

                    case ImageSaveFormat.TIFCompress:
                        //EHollyImageTool.SaveImage(image, "tiff deflate 1", 0, savePath, FileSaveKind.BigImage, DisponseHObject);
                         HOperatorSet.WriteImage(image, "tiff deflate 1", 0, savePath);
                        break;
                    //case ImageSaveFormat.JPEG60:
                    //    HOperatorSet.WriteImage(image, "jpeg 60", 0, savePath + ".jpeg");
                    //    break;
                    //case ImageSaveFormat.JPEG80:
                    //    HOperatorSet.WriteImage(image, "jpeg 80", 0, savePath + ".jpeg");
                    //    break;
                    //case ImageSaveFormat.JPEG100:
                    //    HOperatorSet.WriteImage(image, "jpeg 100", 0, savePath + ".jpeg");
                    //    break;
                    default:
                        //EHollyImageTool.SaveImage(image, "jpeg 20", 0, savePath, FileSaveKind.BigImage, DisponseHObject);
                        HOperatorSet.WriteImage(image, "jpeg 20", 0, savePath);
                        break;
                }
                executeResult.IsSuccess = true;
            }
            catch (Exception ex)
            {
                executeResult.Message = ex.ToString();
                executeResult.IsSuccess = false;
            }
            finally
            {
                if (isDisposeImage)
                {
                    image?.Dispose();
                }
            }
            return executeResult;

        }



        public static ExecuteResult SaveDeepthImage(Image3DClass image3DClass, ImageDeepthSaveFormat imageSaveFormat, string saveDirectoryPath, string saveFileName, bool bIsCompression = true, bool isDisposeImage = false)
        {
            ExecuteResult executeResult = new ExecuteResult();
            try
            {
                if (!Directory.Exists(saveDirectoryPath))
                    Directory.CreateDirectory(saveDirectoryPath);

                string savePath = Path.GetFileNameWithoutExtension(saveFileName);
                savePath = saveDirectoryPath + "\\" + savePath;
                // savePath = Path.GetFileNameWithoutExtension(savePath);
                if (image3DClass == null)
                {
                    executeResult.Message = "图像对象为空";
                    return executeResult;
                }

                if (image3DClass.Image3D == null)
                {
                    executeResult.Message = "图像对象为空";
                    return executeResult;
                }

                if (!image3DClass.Image3D.IsInitialized())
                {
                    executeResult.Message = "图像未初始化";
                    return executeResult;
                }

                if (image3DClass.Image3D?.CountObj() == 0 || image3DClass.Image3D?.CountObj() == null)
                {
                    executeResult.Message = "图像对象数量为0";
                    return executeResult;
                }

                switch (imageSaveFormat)
                {
                    case ImageDeepthSaveFormat.TIF:

                        if (bIsCompression)
                        {
                            //压缩后
                            EHollyImageTool.SaveImage(image3DClass.Image3D, "tiff deflate 1", 0, savePath, FileSaveKind.BigImage, DisponseHObject);
                           // HOperatorSet.WriteImage(image3DClass.Image3D, "tiff deflate 1", 0, savePath);

                        }
                        else
                        {
                            //不压缩
                            EHollyImageTool.SaveImage(image3DClass.Image3D, "tiff", 0, savePath, FileSaveKind.BigImage, DisponseHObject);
                            //HOperatorSet.WriteImage(image3DClass.Image3D, "tiff", 0, savePath);

                        }

                        break;
                    case ImageDeepthSaveFormat.TIF_DAT:
                        string path = savePath + ".dat";
                        SaveAndLoad_Data.Save(image3DClass, path);
                        if (bIsCompression)
                        {
                            //压缩后
                            EHollyImageTool.SaveImage(image3DClass.Image3D, "tiff deflate 1", 0, savePath, FileSaveKind.BigImage, DisponseHObject);
                        }
                        else
                        {
                            //不压缩
                            EHollyImageTool.SaveImage(image3DClass.Image3D, "tiff", 0, savePath, FileSaveKind.BigImage, DisponseHObject);
                        }
                      
                        break;
                    case ImageDeepthSaveFormat.DAT:
                        string path1 = savePath + ".dat";
                        SaveAndLoad_Data.Save(image3DClass, path1);
                        break;

                    default:
                        //压缩后
                        EHollyImageTool.SaveImage(image3DClass.Image3D, "tiff deflate 1", 0, savePath, FileSaveKind.BigImage, DisponseHObject);
                        break;
                }
                executeResult.IsSuccess = true;
            }
            catch (Exception ex)
            {
                executeResult.Message = ex.ToString();
                executeResult.IsSuccess = false;
            }
            finally
            {
                if (isDisposeImage)
                {
                    image3DClass?.Dispose();
                }
            }
            return executeResult;

        }


        /// <summary>
        /// 获取图像格式
        /// </summary>
        /// <param name="imageSaveFormat"></param>
        /// <returns></returns>
        public static string GetImageFormat(ImageSaveFormat imageSaveFormat)
        {
            string format = ".jpeg";
            switch (imageSaveFormat)
            {
                case ImageSaveFormat.BMP:
                    format = ".bmp";
                    break;
                case ImageSaveFormat.JPEG20:
                    format = ".jpeg";
                    break;
                case ImageSaveFormat.JPEG40:
                    format = ".jpeg";
                    break;
                case ImageSaveFormat.TIF:
                    format = ".tif";
                    break;
                case ImageSaveFormat.TIFCompress:
                    format = ".tif";
                    break;
                //case ImageSaveFormat.JPEG60:
                //    HOperatorSet.WriteImage(image, "jpeg 60", 0, savePath + ".jpeg");
                //    break;
                //case ImageSaveFormat.JPEG80:
                //    HOperatorSet.WriteImage(image, "jpeg 80", 0, savePath + ".jpeg");
                //    break;
                //case ImageSaveFormat.JPEG100:
                //    HOperatorSet.WriteImage(image, "jpeg 100", 0, savePath + ".jpeg");
                //    break;
                default:
                    format = ".jpeg";
                    break;
            }
            return format;
        }

        /// <summary>
        /// 保存窗口截图
        /// </summary>
        /// <param name="userControl"></param>
        /// <param name="saveDirectoryPath"></param>
        /// <param name="saveFileName"></param>
        public static void SaveWindowScreenshot(UserControl userControl, string saveDirectoryPath, string saveFileName)
        {
            try
            {
                string savePath = saveDirectoryPath + "\\" + saveFileName + "_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff");

                HTuple time1;
                HTuple time2;
                //计时            
                HOperatorSet.CountSeconds(out time1);

                Bitmap bit = new Bitmap(userControl.Width, userControl.Height);//实例化一个和窗体一样大的bitmap
                Graphics g = Graphics.FromImage(bit);
                g.CompositingQuality = CompositingQuality.HighQuality;//质量设为最高
                g.CopyFromScreen(userControl.Left, userControl.Top, 0, 0, new Size(userControl.Width, userControl.Height));//保存整个窗体为图片
                g.CopyFromScreen(userControl.PointToScreen(Point.Empty), Point.Empty, userControl.Size);//只保存某个控件（这里是panel游戏区）
                bit.Save($"{savePath}.png");//默认保存格式为PNG，保存成jpg格式质量不是很好
                                            //计时            
                HOperatorSet.CountSeconds(out time2);
                double hTime3 = ((time2 - time1) * 1000).D;//算法总耗时
                //Console.WriteLine("截图耗时：" + hTime3.ToString());
            }
            catch (Exception ee)
            {

            }
        }



        /// <summary>
        /// 自己的资源释放函数
        /// </summary>
        /// <param name="image"></param>
        private static void DisponseHObject(HObject image)
        {
            image.DisposeEx();
        }
    }
}

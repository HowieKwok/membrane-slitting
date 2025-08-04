using HalconDotNet;
using LogTool;
using NPOI.OpenXmlFormats.Vml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    /// <summary>
    /// 保存图片执行类
    /// </summary>
    public class SaveImageControl
    {
        ///// <summary>
        ///// 保存原图
        ///// </summary>
        ///// <param name="cameraEnum"></param>
        ///// <param name="CamImage"></param>
        ///// <param name="savePath"></param>
        //public void WritePrimitiveImage(CameraManager cameraManager, HObject CamImage, string saveDirectoryPath, string saveFileName)
        //{
        //    try
        //    {
        //        ImageSaveParamer imageSaveParamer = cameraManager.m_ImageSaveParamer;

        //        if (!Directory.Exists(saveDirectoryPath))
        //            Directory.CreateDirectory(saveDirectoryPath);

        //        if (imageSaveParamer.IsSavePrimitiveImage)
        //        {
        //            switch (imageSaveParamer.imageSaveBMP)
        //            {
        //                case ImageSaveFormat.JPEG:
        //                    HOperatorSet.WriteImage(CamImage, "jpeg 20", 0, saveDirectoryPath + "\\" + saveFileName + ".jpeg");
        //                    break;
        //                case ImageSaveFormat.BMP:
        //                    HOperatorSet.WriteImage(CamImage, "bmp", 0, saveDirectoryPath + "\\" + saveFileName + ".bmp");
        //                    break;
        //                default:
        //                    HOperatorSet.WriteImage(CamImage, "jpeg 20", 0, saveDirectoryPath + "\\" + saveFileName + ".jpeg");
        //                    break;
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //         Log4netRecord.ErrorLog.Debug(ex + $"{cameraManager.m_CamParam.CameraName}");
        //    }
        //    finally
        //    {

        //    }
        //}

        ///// <summary>
        ///// 保存产品图
        ///// </summary>
        //public void WriteProductImage(CameraManager cameraManager, HObject CamImage, bool imageResult, string saveDirectoryPath, string saveFileName)
        //{
        //    try
        //    {
        //        ImageSaveParamer imageSaveParamer = cameraManager.m_ImageSaveParamer;

        //        if (!Directory.Exists(saveDirectoryPath))
        //            Directory.CreateDirectory(saveDirectoryPath);


        //        //switch (imageSaveParamer.imageSave)
        //        //{
        //        //    case ImageSaveSelect.保存所有:
        //        //        if (imageSaveParamer.SaveProdImageFormat== ImageSaveFormat.BMP)
        //        //        {
        //        //            HOperatorSet.WriteImage(CamImage, "bmp", 0, saveDirectoryPath + "\\" + saveFileName + ".bmp");
        //        //        }
        //        //        else
        //        //        {
        //        //            HOperatorSet.WriteImage(CamImage, "jpeg 20", 0, saveDirectoryPath + "\\" + saveFileName + ".jpeg");
        //        //        }                     
        //        //        break;
        //        //    case ImageSaveSelect.保存OK:
        //        //        if (imageResult)
        //        //        {
        //        //            if (imageSaveParamer.SaveProdImageFormat == ImageSaveFormat.BMP)
        //        //            {
        //        //                HOperatorSet.WriteImage(CamImage, "bmp", 0, saveDirectoryPath + "\\" + saveFileName + ".bmp");
        //        //            }
        //        //            else
        //        //            {
        //        //                HOperatorSet.WriteImage(CamImage, "jpeg 20", 0, saveDirectoryPath + "\\" + saveFileName + ".jpeg");
        //        //            }
        //        //        }

        //        //        break;
        //        //    case ImageSaveSelect.保存NG:
        //        //        if (!imageResult)
        //        //        {
        //        //            if (imageSaveParamer.SaveProdImageFormat == ImageSaveFormat.BMP)
        //        //            {
        //        //                HOperatorSet.WriteImage(CamImage, "bmp", 0, saveDirectoryPath + "\\" + saveFileName + ".bmp");
        //        //            }
        //        //            else
        //        //            {
        //        //                HOperatorSet.WriteImage(CamImage, "jpeg 20", 0, saveDirectoryPath + "\\" + saveFileName + ".jpeg");
        //        //            }
        //        //        }
        //        //        break;
        //        //    case ImageSaveSelect.不保存:

        //        //        break;
        //        //    default:

        //        //        break;
        //        //}

        //    }
        //    catch (Exception ex)
        //    {
        //         Log4netRecord.ErrorLog.Debug(ex + $"{cameraManager.m_CamParam.CameraName}");
        //    }
        //    finally
        //    {

        //    }
        //}

        /////// <summary>
        /////// 保存NG产品截图
        /////// </summary>
        /////// <param name="cameraManager"></param>
        /////// <param name="HalconWinHandle"></param>
        /////// <param name="imageResult"></param>
        /////// <param name="savePath"></param>
        ////public void WriteNGProductScreenshot(CameraManager cameraManager, HWindow HalconWinHandle, string saveDirectoryPath, string saveFileName)
        ////{
        ////    HObject Copy_Win_Image;
        ////    HOperatorSet.GenEmptyObj(out Copy_Win_Image);
        ////    try
        ////    {
        ////        ImageSaveParamer imageSaveParamer = cameraManager.m_ImageSaveParamer;

        ////        if (!Directory.Exists(saveDirectoryPath))
        ////            Directory.CreateDirectory(saveDirectoryPath);


        ////        HOperatorSet.DumpWindowImage(out Copy_Win_Image, HalconWinHandle);
        ////        HOperatorSet.WriteImage(Copy_Win_Image, "jpeg 20", 0, saveDirectoryPath + "\\" + saveFileName + ".jpeg");

        ////    }
        ////    catch (Exception ex)
        ////    {
        ////         Log4netRecord.ErrorLog.Debug(ex + $"{cameraManager.m_CamParam.CameraName}");
        ////    }
        ////    finally
        ////    {
        ////        Copy_Win_Image.Dispose();
        ////    }
        ////}

        /////// <summary>
        /////// 保存NG截图
        /////// </summary>
        /////// <param name="cameraManager"></param>
        /////// <param name="CamImage"></param>
        /////// <param name="saveDirectoryPath"></param>
        /////// <param name="saveFileName"></param>
        ////public void WriteNGCropImage(CameraManager cameraManager, HObject CamImage, string saveDirectoryPath, string saveFileName)
        ////{
         
        ////    try
        ////    {
        ////        ImageSaveParamer imageSaveParamer = cameraManager.m_ImageSaveParamer;

        ////        if (!Directory.Exists(saveDirectoryPath))
        ////            Directory.CreateDirectory(saveDirectoryPath);
         
        ////        HOperatorSet.WriteImage(CamImage, "jpeg 20", 0, saveDirectoryPath + "\\" + saveFileName + ".jpeg");
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        Log4netRecord.ErrorLog.Debug(ex + $"{cameraManager.m_CamParam.CameraName}");
        ////    }
        ////    finally
        ////    {
               
        ////    }
        ////}

        ///// <summary>
        ///// 保存NG小图
        ///// </summary>
        ///// <param name="cameraManager"></param>
        ///// <param name="ngCropImageInfo"></param>
        ///// <param name="savePath"></param>
        //public void WriteNGSmallImage(CameraManager cameraManager, HObject ngImage, string saveDirectoryPath, string saveFileName)
        //{
        //    try
        //    {
        //        ImageSaveParamer imageSaveParamer = cameraManager.m_ImageSaveParamer;

        //        if (!Directory.Exists(saveDirectoryPath))
        //            Directory.CreateDirectory(saveDirectoryPath);

        //        //if (imageSaveParamer.SaveNgMinImageFormat == ImageSaveFormat.BMP)
        //        //{
        //        //    HOperatorSet.WriteImage(ngImage, "bmp", 0, saveDirectoryPath + "\\" + saveFileName + ".bmp");
        //        //}
        //        //else
        //        //{
        //        //    HOperatorSet.WriteImage(ngImage, "jpeg 20", 0, saveDirectoryPath + "\\" + saveFileName + ".jpeg");
        //        //}
              
        //    }
        //    catch (Exception ex)
        //    {
        //         Log4netRecord.ErrorLog.Debug(ex + $"{cameraManager.m_CamParam.CameraName}");
        //    }
        //    finally
        //    {

        //    }
        //}
    }

    /// <summary>
    /// NG裁图图片信息
    /// </summary>
    public class NGCropImageInfo
    {
        /// <summary>
        /// NG小图
        /// </summary>
        public HObject ho_NGCropImage;

        /// <summary>
        /// NG小图所在的原图开始帧号
        /// </summary>
        public int primitiveImgBegFrame = 0;

        /// <summary>
        /// NG小图所在的原图开始行
        /// </summary>
        public int primitiveImgBegRow1 = 0;

        /// <summary>
        /// NG小图所在的原图结束帧号
        /// </summary>
        public int primitiveImgEndFrame = 0;

        /// <summary>
        /// NG小图所在的原图结束行
        /// </summary>
        public int primitiveImgEndRow1 = 0;

        /// <summary>
        /// NG小图所在的产品数
        /// </summary>
        public int EANum = 0;

        /// <summary>
        /// NG所在的产品图row1位置
        /// </summary>
        public double EARow1 = 0;
        /// <summary>
        /// NG所在的产品图coll位置
        /// </summary>
        public double EAColl = 0;
        /// <summary>
        /// NG所在的产品图row2位置
        /// </summary>
        public double EARow2 = 0;
        /// <summary>
        /// NG所在的产品图col2位置
        /// </summary>
        public double EACol2 = 0;

        /// <summary>
        /// NG类型
        /// </summary>
        public string NGType = "";

        /// <summary>
        /// NG面积
        /// </summary>
        public double area = 0;

        /// <summary>
        /// NG宽度
        /// </summary>
        public double width = 0;

        /// <summary>
        /// NG高度
        /// </summary>
        public double height = 0;

        /// <summary>
        /// NG灰度值
        /// </summary>
        public int iNG_Gray = 0;


        /// <summary>
        /// 缺陷附加信息
        /// </summary>
        public string sAttachInfo = "";

        /// <summary>
        /// 是否打标
        /// </summary>
        public bool IsMarking = false;
    }
}

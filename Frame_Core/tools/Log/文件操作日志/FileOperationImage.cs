using HalconDotNet;
using LogTool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    public class FileSaveImage
    {
        public enum SaveImageType
        {
            jpeg20,
            bmp
        }
   
        /// <summary>
        /// 保存原图
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileName"></param>
        /// <param name="camImage"></param>
        /// <param name="saveImageType"></param>
        public void SaveImage_Original(string filePath, string fileName, HObject camImage, SaveImageType saveImageType)
        {
            try
            {
                //if (!Directory.Exists(filePath))
                //    Directory.CreateDirectory(filePath);
                switch (saveImageType)
                {
                    case SaveImageType.jpeg20:
                        HOperatorSet.WriteImage(camImage, "jpeg 20", 0, filePath + fileName + ".jpeg");
                        break;
                    case SaveImageType.bmp:
                        HOperatorSet.WriteImage(camImage, "bmp", 0, filePath + fileName + ".bmp");
                        break;
                    default:
                        HOperatorSet.WriteImage(camImage, "jpeg 20", 0, filePath + fileName + ".jpeg");
                        break;
                }
                camImage.Dispose();
            }
            catch (Exception ex)
            {
                 Log4netRecord.ErrorLog.Debug(ex);
            }
        }


        /// <summary>
        /// 保存截图
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileName"></param>
        /// <param name="HalconWinHandle"></param>
        public void SaveImage_Dump(string filePath, string fileName, HWindow HalconWinHandle)
        {
            try
            {
                HObject Copy_Win_Image = null;
                HOperatorSet.DumpWindowImage(out Copy_Win_Image, HalconWinHandle);
                HOperatorSet.WriteImage(Copy_Win_Image, "jpeg 20", 0, filePath + fileName + ".jpeg");
                Copy_Win_Image.Dispose();
            }
            catch (Exception ex)
            {
                 Log4netRecord.ErrorLog.Debug(ex);
            }

        }

        /// <summary>
        /// 保存小图
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileName"></param>
        /// <param name="HalconWinHandle"></param>
        public void SaveImage_Small(string filePath, string fileName, HObject camImage)
        {
            try
            {              
                HOperatorSet.WriteImage(camImage, "jpeg", 0, filePath + fileName + ".jpeg");
                //camImage.Dispose();
            }
            catch (Exception ex)
            {
                 Log4netRecord.ErrorLog.Debug(ex);
            }

        }

    }
}

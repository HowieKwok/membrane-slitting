using BitMiracle.LibTiff.Classic;
using HalconDotNet;

using NPOI.OpenXmlFormats.Wordprocessing;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace Frame_Core
{
    /// <summary>
    /// 图片仿真管理
    /// </summary>
    public class ImageSimManage
    {
        public delegate void delegateCallBack();
        public delegate void delegateDeviceEventGrabed(long currentFrame);
        /// <summary>
        /// 相机回调结束事件
        /// </summary>
        public delegateCallBack CamCallbackEndEvent;

        /// <summary>
        /// 相机回调开始事件
        /// </summary>
        public delegateDeviceEventGrabed CamCallbackStartEvent;

        /// <summary>
        /// 当前图像ID
        /// </summary>
        public int currentImageID = 0;

        /// <summary>
        /// 图像开始加载完成标志
        /// </summary>
        public bool ImageStartLoadFinishFlag = false;

        /// <summary>
        /// 当前图像索引
        /// </summary>
        public int currentImageIndex = 0;

        /// <summary>
        /// 从索引0开始获取图像
        /// </summary>
        private bool ZeroIndexStart = true;

        public void InitSimFlag()
        {
            currentImageID = 0;
            currentImageIndex = 0;
            ImageStartLoadFinishFlag = false;
            ZeroIndexStart = true;
        }

        /// <summary>
        /// 图片仿真路径
        /// </summary>
        public string[] ImageEmulationPaths;

        /// <summary>
        /// 读取图片仿真路径
        /// </summary>
        /// <returns></returns>
        public void ReadImageSimPath(string EmulationPath)
        {
            try
            {
                //ImageEmulationPaths = new string[0];
                //currentImageIndex = 0;
                //if (EmulationPath == null || EmulationPath == "") return;

                ////bmp
                //string[] files = System.IO.Directory.GetFiles(EmulationPath, "*.bmp");               
                //files = files.OrderBy(s => int.Parse(System.Text.RegularExpressions.Regex.Match(s, @"\d+").Value)).ToArray();
                //if (files != null)
                //{
                //    for (int j = 0; j < files.Length; j++)
                //    {
                //        Array.Resize(ref ImageEmulationPaths, ImageEmulationPaths.Length + 1);
                //        ImageEmulationPaths[ImageEmulationPaths.Length - 1] = files[j];
                //    }
                //}
                ////jpg
                //string[] jpgfiles = System.IO.Directory.GetFiles(EmulationPath, "*.jpg");
                //jpgfiles = jpgfiles.OrderBy(s => int.Parse(System.Text.RegularExpressions.Regex.Match(s, @"\d+").Value)).ToArray();
                //if (jpgfiles != null)
                //{
                //    for (int j = 0; j < jpgfiles.Length; j++)
                //    {
                //        Array.Resize(ref ImageEmulationPaths, ImageEmulationPaths.Length + 1);
                //        ImageEmulationPaths[ImageEmulationPaths.Length - 1] = jpgfiles[j];
                //    }
                //}

                ////jpeg
                //string[] jpegfiles = System.IO.Directory.GetFiles(EmulationPath, "*.jpeg");
                //jpegfiles = jpegfiles.OrderBy(s => int.Parse(System.Text.RegularExpressions.Regex.Match(s, @"\d+").Value)).ToArray();
                //if (jpegfiles != null)
                //{
                //    for (int j = 0; j < jpegfiles.Length; j++)
                //    {
                //        Array.Resize(ref ImageEmulationPaths, ImageEmulationPaths.Length + 1);
                //        ImageEmulationPaths[ImageEmulationPaths.Length - 1] = jpegfiles[j];
                //    }
                //}

                ////Tiff
                //string[] tifffiles = System.IO.Directory.GetFiles(EmulationPath, "*.tiff");
                //tifffiles = tifffiles.OrderBy(s => int.Parse(System.Text.RegularExpressions.Regex.Match(s, @"\d+").Value)).ToArray();
                //if (tifffiles != null)
                //{
                //    for (int j = 0; j < tifffiles.Length; j++)
                //    {
                //        Array.Resize(ref ImageEmulationPaths, ImageEmulationPaths.Length + 1);
                //        ImageEmulationPaths[ImageEmulationPaths.Length - 1] = tifffiles[j];
                //    }
                //}

                ImageEmulationPaths = new string[0];
                currentImageIndex = 0;
                if (string.IsNullOrEmpty(EmulationPath)) return;
                if (!Directory.Exists(EmulationPath)) return;

                string[] extensions = { "*.bmp", "*.jpg", "*.jpeg", "*.tiff", "*.dat" };

                //==========================
                //【】通过帧号排序
                //foreach (var extension in extensions)
                //{
                //    string[] files = System.IO.Directory.GetFiles(EmulationPath, extension);
                //    //files = files.OrderBy(s => int.Parse(System.Text.RegularExpressions.Regex.Match(s, @"\d+").Value)).ToArray();
                //    files = files.OrderBy(GetStartValueFromFile).ToArray();

                //    if (files != null)
                //    {
                //        foreach (var file in files)
                //        {
                //            Array.Resize(ref ImageEmulationPaths, ImageEmulationPaths.Length + 1);
                //            ImageEmulationPaths[ImageEmulationPaths.Length - 1] = file;
                //        }
                //    }
                //}

                //==========================
                //【】通过时间排序
                foreach (var extension in extensions)
                {
                    string[] files = System.IO.Directory.GetFiles(EmulationPath, extension);
                    files = files.OrderBy(f => new FileInfo(f).LastWriteTime).ToArray();

                    if (files != null)
                    {
                        foreach (var file in files)
                        {
                            Array.Resize(ref ImageEmulationPaths, ImageEmulationPaths.Length + 1);
                            ImageEmulationPaths[ImageEmulationPaths.Length - 1] = file;
                        }
                    }
                }
            }
            catch (Exception)
            {

            }

        }
        public int GetStartValueFromFile(string filePath)
        {
            string fileName = System.IO.Path.GetFileNameWithoutExtension(filePath);
            string numberString = Regex.Match(fileName, @"\d+").Value;
            int number = 0;
            if (int.TryParse(numberString, out number))
            {
                return number;
            }
            return 0; // Default value if parsing fails
        }

        /// <summary>
        /// 获取上一张图像
        /// </summary>
        public bool GetPreviewImage(out HObject EmulationImage, out string imgPath)
        {
            bool result = false;
            imgPath = "";
            HOperatorSet.GenEmptyObj(out EmulationImage);
            try
            {
                if (ImageEmulationPaths != null && ImageEmulationPaths.Length > 0)
                {
                    if (currentImageIndex == 0)
                    {
                        if (GetZeroIndexStartNext())
                        {
                            result = GetImage(out EmulationImage, out imgPath);
                            return result;
                        }

                        currentImageIndex = ImageEmulationPaths.Length - 1;
                    }
                    else
                    {
                        currentImageIndex--;
                    }
                    result = GetImage(out EmulationImage, out imgPath);
                }
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 是否获取第0张
        /// </summary>
        /// <returns></returns>
        public bool GetZeroIndexStartNext()
        {    
            if (ZeroIndexStart)
            {
                ZeroIndexStart = false;
                return true;
            }
            return false;
        }
        /// <summary>
        /// 获取当前图像
        /// </summary>
        public bool GetCurrentImage(out HObject EmulationImage, out string imgPath)
        {
            bool result = false;
            imgPath = "";
            HOperatorSet.GenEmptyObj(out EmulationImage);
            try
            {
                if (ImageEmulationPaths != null && ImageEmulationPaths.Length > 0)
                {
                    GetZeroIndexStartNext();
                    result = GetImage(out EmulationImage, out imgPath);
                }
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 获取下一张图像
        /// </summary>
        public bool GetNextImage(out HObject EmulationImage, out string imgPath)
        {
            bool result = false;
            imgPath = "";
            HOperatorSet.GenEmptyObj(out EmulationImage);
            try
            {
                if (ImageEmulationPaths != null && ImageEmulationPaths.Length > 0)
                {
                    if (currentImageIndex == 0)
                    {
                        if (GetZeroIndexStartNext())
                        {
                            result = GetImage(out EmulationImage, out imgPath);
                            return result;
                        }        
                    }

                    if (currentImageIndex == ImageEmulationPaths.Length - 1)
                    {
                        currentImageIndex = 0;
                    }
                    else
                    {
                        currentImageIndex++;
                    }
                    result = GetImage(out EmulationImage, out imgPath);
                }
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }


        /// <summary>
        /// 获取当前图像
        /// </summary>
        public bool GetCurrent3DImage(out Image3DClass image3DClass)
        {
            bool result = false;
            // HOperatorSet.GenEmptyObj(out EmulationImage);
            image3DClass = new Image3DClass();
            try
            {
                if (ImageEmulationPaths != null && ImageEmulationPaths.Length > 0)
                {
                    result = GetImage3D(out image3DClass);
                }
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }
        /// <summary>
        /// 获取下一张图像
        /// </summary>
        public bool GetNext3DImage(out Image3DClass image3DClass)
        {
            bool result = false;
            image3DClass = new Image3DClass();
            try
            {
                if (ImageEmulationPaths != null && ImageEmulationPaths.Length > 0)
                {
                    if (currentImageIndex == ImageEmulationPaths.Length - 1)
                    {
                        currentImageIndex = 0;
                    }
                    else
                    {
                        currentImageIndex++;
                    }
                    result = GetImage3D(out image3DClass);
                }
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }


        /// <summary>
        /// 获取图片
        /// </summary>
        private bool GetImage(out HObject EmulationImage, out string imgPath)
        {
            bool result = false;
            imgPath = "";
            HOperatorSet.GenEmptyObj(out EmulationImage);
            try
            {
                if (ImageEmulationPaths.Length > 0 && ImageEmulationPaths.Length > currentImageIndex)
                {
                    imgPath = ImageEmulationPaths[currentImageIndex];
                    if (!File.Exists(imgPath)) return false;
                    HOperatorSet.ReadImage(out EmulationImage, imgPath);
                    if (EmulationImage == null || !EmulationImage.IsInitialized() || EmulationImage.CountObj() == 0)
                    {
                        EmulationImage.Dispose();
                        return false;
                    }
                    result = true;
                }
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }


        private bool GetImage3D(out Image3DClass EmulationImage3D)
        {
            bool result = false;
            Image3DClass image3DClass = null;
            try
            {
                if (ImageEmulationPaths.Length > 0 && ImageEmulationPaths.Length > currentImageIndex)
                {
                    image3DClass = (Image3DClass)SaveAndLoad_Data.Load(ImageEmulationPaths[currentImageIndex]);
                    if (image3DClass != null)
                    {

                        result = true;
                    }
                    else
                    {

                        result = false;
                    }

                }

            }
            catch (Exception)
            {
                result = false;
            }
            EmulationImage3D = image3DClass;
            return result;
        }

        /// <summary>
        /// 获取从索引零开始的下一张图像
        /// </summary>
        /// <returns></returns>
        public bool GetZeroIndexStartNextImage(out HObject EmulationImage, out string imgPath)
        {
            bool result = false;
            imgPath = "";
            HOperatorSet.GenEmptyObj(out EmulationImage);
            try
            {
                if (ImageEmulationPaths != null && ImageEmulationPaths.Length > 0)
                {
                    result = GetImage(out EmulationImage, out imgPath);
                    if (currentImageIndex == ImageEmulationPaths.Length - 1)
                    {
                        currentImageIndex = 0;
                    }
                    else
                    {
                        currentImageIndex++;
                    }
                }
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 获取从索引零开始的下一张图像
        /// </summary>
        /// <returns></returns>
        public bool GetZeroIndexStartNextImage3D(out Image3DClass EmulationImage3D)
        {
            bool result = false;
            Image3DClass image3DClass = null;
            try
            {
                if (ImageEmulationPaths != null && ImageEmulationPaths.Length > 0)
                {

                    result = GetImage3D(out image3DClass);

                    if (currentImageIndex == ImageEmulationPaths.Length - 1)
                    {
                        currentImageIndex = 0;
                    }
                    else
                    {
                        currentImageIndex++;
                    }
                }
            }
            catch (Exception)
            {

                result = false;
            }
            EmulationImage3D = image3DClass;
            return result;
        }

        /// <summary>
        /// 获取所有图像
        /// </summary>
        /// <param name="EmulationImages"></param>
        /// <returns></returns>
        public bool GetAllImage(out HObject[] EmulationImages)
        {
            bool result = false;
            EmulationImages = new HObject[0];
            try
            {
                if (ImageEmulationPaths != null && ImageEmulationPaths.Length > 0)
                {
                    EmulationImages = new HObject[ImageEmulationPaths.Length];
                    for (int i = 0; i < ImageEmulationPaths.Length; i++)
                    {
                        HOperatorSet.ReadImage(out EmulationImages[i], ImageEmulationPaths[i]);
                    }
                    result = true;
                }
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }
    }
}

using System;
using System.Collections.Generic;
using HalconDotNet;
using EHAICBase;
using LogTool;
using static System.Windows.Forms.AxHost;

namespace ClassifyCPU_onnx_test
{
    public class ClassificationClient
    {
        private EHAIBase client ;

        private string modelPath = "model.onnx";

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="modelPath"></param>
        public int init(string modelPath)
        {
            client = new EHAIBase();
            int initreturn = client.initializerModel(modelPath);
            if (initreturn != 0)
            {
                Log4netRecord.ErrorLog.Error($"模型建立失败！{initreturn}");
            }
            this.modelPath = modelPath;

            return initreturn;
        }
        /// <summary>
        /// 分类
        /// </summary>
        /// <param name="ho_Images"></param>
        /// <param name="classDatas"></param>
        /// <param name="Times"></param>
        /// <returns></returns>
        public int Classification_channel(List<HObject> ho_Images, out int[] classDatas, out double Times)
        {
            Times = 0;//分类时间
            classDatas = null;
            DateTime dt1 = DateTime.Now;
            try
            {
                if (ho_Images == null)
                    return -1;  //ho_Images中没有图像
                    
                unsafe
                {
                    int imageNum = ho_Images.Count;
                    classDatas = new int[imageNum];
                    byte*[] pImages = new byte*[imageNum];
                    int[] imageWidths = new int[imageNum];
                    int[] imageHeights = new int[imageNum];
                    HTuple hv_Pointer, hv_Type, wImage = null, hImage = null;
                    for (int imgIdx = 0; imgIdx < imageNum; imgIdx++)
                    {
                        HOperatorSet.GetImagePointer1(ho_Images[imgIdx], out hv_Pointer, out hv_Type, out wImage, out hImage);
                        byte* pImage = (byte*)hv_Pointer[0].L;
                        pImages[imgIdx] = pImage;
                        imageWidths[imgIdx] = wImage;
                        imageHeights[imgIdx] = hImage;
                    }

                    fixed (byte** ptrImages = pImages)
                    {
                        fixed (int* ptrw = imageWidths)
                        {
                            fixed (int* ptrh = imageHeights)
                            {
                                allBody classifyResult = new allBody();
                                int inferReturn = client.inferModel("Classify", ptrImages, 1, ptrw, ptrh, imageNum, ref classifyResult);
                                if (inferReturn < 0)
                                {
                                    return inferReturn;
                                   
                                }
                                List<ValueType> Classes = new List<ValueType>();//接收解析结果
                                for (int ImageInd = 0; ImageInd < classifyResult.ImagesBody.Length; ImageInd++)
                                {
                                    if (classifyResult.ImagesBody[ImageInd].ImageBody.Length < 1)
                                        continue;
                                    for (int ployInd = 0; ployInd < classifyResult.ImagesBody[ImageInd].ImageBody.Length; ployInd++)
                                    {
                                        var ploy = classifyResult.ImagesBody[ImageInd].ImageBody[ployInd];
                                        Classes.Add(ploy.category);
                                    }
                                    classDatas[ImageInd] = (int)Classes[ImageInd];

                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
               //LogHelper.WriteError(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + ex);
            }
            finally
            {
                DateTime dt2 = DateTime.Now;
                TimeSpan ts = DateDiff2(dt1, dt2);
                Times = ts.TotalMilliseconds;
            }
            return 0;
           
        }

        /// <summary>
        /// 多个相机多个线程加锁进入分类器服务器
        /// </summary>
        public static readonly object _lockClify = new object();
        /// <summary>
        /// 压缩图片，返回图片前5个像素值
        /// </summary>
        /// <param name="ho_ImagePart">原图</param>
        /// <param name="ho_ImagePartCache">压缩后的图片</param>
        /// <returns></returns>
        public string Imagecompression(HObject ho_ImagePart, out HObject ho_ImagePartCache)
        {
            lock (_lockClify)
            {
                HTuple partWidth, partHeight;
                HOperatorSet.GenEmptyObj(out ho_ImagePartCache);
                HObject ho_ImagePartClassifyCache;
                HOperatorSet.GenEmptyObj(out ho_ImagePartClassifyCache);
                HOperatorSet.GetImageSize(ho_ImagePart, out partWidth, out partHeight);
                if (partWidth * partHeight < 100000 && partWidth < 1000 && partHeight < 1000)//判断图像的尺寸，判断宽度跟高度是否超出1000
                {
                    ho_ImagePartCache.Dispose();
                    HOperatorSet.CopyImage(ho_ImagePart, out ho_ImagePartCache);//开辟图像内存
                }
                else
                {
                    HTuple ZoomScale = 100000.0 / partWidth / partHeight;//等比例
                    HOperatorSet.TupleSqrt(ZoomScale, out ZoomScale);//求开方
                    HOperatorSet.ZoomImageFactor(ho_ImagePart, out ho_ImagePartClassifyCache, ZoomScale, ZoomScale, "nearest_neighbor");//等比例缩放

                    //尺寸防呆，若尺寸超过5位数服务器会挂掉
                    HOperatorSet.GetImageSize(ho_ImagePartClassifyCache, out partWidth, out partHeight);
                    if (partWidth > 10000)
                    {
                        HOperatorSet.ZoomImageSize(ho_ImagePartClassifyCache, out ho_ImagePartClassifyCache, 9999, partHeight, "nearest_neighbor");
                    }
                    if (partHeight > 10000)
                    {
                        HOperatorSet.ZoomImageSize(ho_ImagePartClassifyCache, out ho_ImagePartClassifyCache, partWidth, 9999, "nearest_neighbor");
                    }
                    ho_ImagePartCache.Dispose();
                    HOperatorSet.CopyImage(ho_ImagePartClassifyCache, out ho_ImagePartCache);//开辟新内存
                }
                //记录进入分类器图片的前5个像素值
                HTuple InputGray;
                string GrayValue = "";
                for (int i = 0; i < 5; i++)
                {
                    HOperatorSet.GetGrayval(ho_ImagePartCache, 0, i, out InputGray);
                    GrayValue += InputGray.ToString() + " ";
                }
                return GrayValue;
            }
        }
        static TimeSpan DateDiff2(DateTime DateTime1, DateTime DateTime2)
        {
            TimeSpan ts1 = new TimeSpan(DateTime1.Ticks);
            TimeSpan ts2 = new TimeSpan(DateTime2.Ticks);
            TimeSpan ts = ts1.Subtract(ts2).Duration();
            return ts;
        }   //此函数作用是计算程序运行时间的  

    }
}

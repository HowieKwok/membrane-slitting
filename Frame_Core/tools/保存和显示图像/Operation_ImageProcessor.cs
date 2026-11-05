using Frame_Core;
using FrameCommunity;
using HalconDotNet;


using LogTool;

using NPOI.OpenXmlFormats.Wordprocessing;
using NPOI.SS.Formula.Functions;
using NPOI.Util;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using static System.Net.Mime.MediaTypeNames;

namespace Frame_Core
{
    /// <summary>
    /// 操作类：图像处理
    /// </summary>
    public class Operation_ImageProcessor : IDisposable
    {

        private string Source { get; }
        private readonly object lockObject = new object();
        DisplayWindowClass displayWindow;
        public QueuePackage<Operation_ImageInfo> imageInfosShow = new QueuePackage<Operation_ImageInfo>();
        public QueuePackage<Operation_ImageInfo> imageInfosSave = new QueuePackage<Operation_ImageInfo>();
        public QueuePackage<Operation_ImageInfo> imageInfosScreenshotSave = new QueuePackage<Operation_ImageInfo>();
        public QueuePackage<Operation_ImageInfo> imageInfosHightScreenshotSave = new QueuePackage<Operation_ImageInfo>();
        public QueuePackage<Operation_ImageInfo> imageInfosDispose = new QueuePackage<Operation_ImageInfo>();

        private bool isShowOperationInProgress = false;
        private bool isSaveOperationInProgress = false;
        private bool isSaveScreenshotOperationInProgress = false;

        public delegate ExecuteResult ImageShowDelegate(Operation_ImageInfo imageInfo, DisplayWindowClass displayWindowClass, bool bIsScreenshot, out Operation_ImageInfo screenshotImage);
        public delegate ExecuteResult ImageSaveDelegate(Operation_ImageInfo imageInfo);
        //public event ImageShowDelegate ShowImageEvent;
        //public event ImageSaveDelegate SaveImageEvent;
        //public event ImageSaveDelegate SaveScreenshotImageEvent;


        Thread threadHighScreenshot = null;
        AutoResetEvent autoEventHighScreenshot = new AutoResetEvent(false);

        public Operation_ImageProcessor(string source)
        {
            Source = source;

        }
        public Operation_ImageProcessor(string source, DisplayWindowClass _displayWindowClass)
        {
            Source = source;
            displayWindow = _displayWindowClass;
            threadHighScreenshot=new Thread(HighScreenshotMethod);
            threadHighScreenshot.IsBackground = true;
            threadHighScreenshot.Start();
        }


        

        public void HighScreenshotMethod()
        {
           while(true)
            {
                autoEventHighScreenshot.WaitOne(1000);
                string text = "触发高清截图";
                QueueLengthLimit(imageInfosHightScreenshotSave, 20, text);
                if (imageInfosHightScreenshotSave.Count > 0 && imageInfosHightScreenshotSave.Peek().bIsSaveScreenshot)
                {
                    if (TryDequeue(imageInfosHightScreenshotSave, out Operation_ImageInfo imageInfo, 0, text))
                    {
                        displayWindow.HightDefinition_Screenshot(imageInfo,out Operation_ImageInfo screenshotImage);
                        if(screenshotImage!=null)
                        {
                            screenshotImage.HighDefinition_Screenshot = false;
                            imageInfosScreenshotSave.Enqueue(screenshotImage);
                            MessageNotify.AddDebugLog($"当前相机：{imageInfo.showHalconWindow.m_strCamName}_模式：{imageInfo.showHalconWindow.m_nMode}_触发高清截图队列个数：{imageInfosHightScreenshotSave.Count}");


                        }

                    }
                }
                else
                {
                    if (TryDequeue(imageInfosHightScreenshotSave, out Operation_ImageInfo imageInfo, 0, text))
                    {
                        imageInfo.Dispose();
                        MessageNotify.AddDebugLog($"当前相机：{imageInfo.showHalconWindow.m_strCamName}_模式：{imageInfo.showHalconWindow.m_nMode}_触发高清截图队列个数：{imageInfosHightScreenshotSave.Count}");
                    }
                }
            }
        }



        public async Task ShowImagesAsync(QueuePackage<Operation_ImageInfo> source_ShowQueue, DisplayWindowClass displayWindowClass, QueuePackage<Operation_ImageInfo> target_SaveQueue, QueuePackage<Operation_ImageInfo> target_SaveScreenshotQueue)
        {
            string text = "显示";
            try
            {
                QueueLengthLimit(source_ShowQueue, 20, text);
                if (source_ShowQueue.Count > 0 && source_ShowQueue.Peek().bIsShowImage)
                {
                    if (!isShowOperationInProgress)
                    {
                        if (TryDequeue(source_ShowQueue, out Operation_ImageInfo imageInfo, 0, text))
                        {
                          
                            isShowOperationInProgress = true;
                            await Task.Run(() =>
                            {
                                _ = ExecuteImageOperationShowAsync(imageInfo, displayWindowClass, target_SaveQueue, target_SaveScreenshotQueue, text);
                                isShowOperationInProgress = false;
                            });

                        }
                    }
                }
                else
                {
                    if (TryDequeue(source_ShowQueue, out Operation_ImageInfo imageInfo, 0, text))
                    {
                        target_SaveQueue.Enqueue(imageInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error("异步显示图像错误" + $"来源：{Source}", ex);
            }
        }

        public async Task SaveImagesAsync(QueuePackage<Operation_ImageInfo> source_SaveQueue, QueuePackage<Operation_ImageInfo> target_DisposeQueue)
        {
            string text = "保存";
            try
            {
                QueueLengthLimit(source_SaveQueue, 20, text);
                if (source_SaveQueue.Count > 0 && source_SaveQueue.Peek().bIsSaveImage)
                {
                    if (!isSaveOperationInProgress)
                    {
                        if (TryDequeue(source_SaveQueue, out Operation_ImageInfo imageInfo, 0, text))
                        {
                            isSaveOperationInProgress = true;
                            await Task.Run(() =>
                            {
                                if (imageInfo.showHalconWindow.m_nMode == 1)
                                {

                                }
                                _ = ExecuteImageOperationSaveAsync(imageInfo, target_DisposeQueue, text);
                                isSaveOperationInProgress = false;
                              //  MessageNotify.AddDebugLog($"当前相机：{imageInfo.showHalconWindow.m_strCamName}_模式：{imageInfo.showHalconWindow.m_nMode}_释放队列个数：{target_DisposeQueue.Count}");
                            });
                        }
                    }
                }
                else
                {
                    if (TryDequeue(source_SaveQueue, out Operation_ImageInfo imageInfo, 0, text))
                    {
                        target_DisposeQueue.Enqueue(imageInfo);
                        MessageNotify.AddDebugLog($"当前相机：{imageInfo.showHalconWindow.m_strCamName}_模式：{imageInfo.showHalconWindow.m_nMode}_释放队列个数：{target_DisposeQueue.Count}");
                    }
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug("异步保存图像错误" + $"来源：{Source}-{text}", ex);
            }
        }

        public async Task DisposeImagesAsync(QueuePackage<Operation_ImageInfo> source_DisposeQueue, int N)
        {
            string text = "释放";
            Operation_ImageInfo imageInfo = null;
            try
            {
                TryDequeue(source_DisposeQueue, out imageInfo, N, text);
                if(imageInfo!=null)
                {
                    int count = imageInfo.showHalconWindow.m_hImage.GetReferenceCount();
                    MessageNotify.AddDebugLog($"当前相机：{imageInfo.showHalconWindow.m_strCamName}当前ID:{imageInfo.showHalconWindow.m_nImageFrameID}图像内部剩余数：{count}");
                }
         

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug("异步释放图像错误" + $"来源：{Source}-{text}", ex);
            }
           
            imageInfo?.Dispose();


        }

        public async Task SaveDisposeScreenshotImagesAsync(QueuePackage<Operation_ImageInfo> source_SaveScreenshotQueue)
        {
            string text = "保存截图";
            try
            {
                QueueLengthLimit(source_SaveScreenshotQueue, 20, text);
                if (source_SaveScreenshotQueue.Count > 0 && source_SaveScreenshotQueue.Peek().bIsSaveScreenshot)
                {
                    if (!isSaveScreenshotOperationInProgress)
                    {
                        if (TryDequeue(source_SaveScreenshotQueue, out Operation_ImageInfo imageInfo, 0, text))
                        {
                            isSaveScreenshotOperationInProgress = true;
                            await Task.Run(() =>
                            {
                                _ = ExecuteScreenshotImageOperationSaveDisposeAsync(imageInfo, text);
                                isSaveScreenshotOperationInProgress = false;
                                MessageNotify.AddDebugLog($"当前相机：{imageInfo.showHalconWindow.m_strCamName}_模式：{imageInfo.showHalconWindow.m_nMode}_截图队列个数：{source_SaveScreenshotQueue.Count}");
                            });

                        }
                    }
                }
                else
                {
                    if (TryDequeue(source_SaveScreenshotQueue, out Operation_ImageInfo imageInfo, 0, text))
                    {
                        imageInfo.Dispose();
                        MessageNotify.AddDebugLog($"当前相机：{imageInfo.showHalconWindow.m_strCamName}_模式：{imageInfo.showHalconWindow.m_nMode}_截图队列个数：{source_SaveScreenshotQueue.Count}");
                    }
                }

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug("异步保存截图错误" + $"来源：{Source}-{text}", ex);
            }
        }

        public Thread thread =null;
      

        private async Task ExecuteImageOperationShowAsync(Operation_ImageInfo imageInfo, DisplayWindowClass displayWindowClass, QueuePackage<Operation_ImageInfo> target_SaveQueue, QueuePackage<Operation_ImageInfo> target_SaveScreenshotQueue, string operation)
        {
            try
            {
                Operation_ImageInfo screenshotImage = null;
                //UIService.Instance.AddAction(() => { });
                displayWindowClass?.RepaintCam(imageInfo, out screenshotImage);
                if (imageInfo != null)
                {
                    int count = imageInfo.showHalconWindow.m_hImage.GetReferenceCount();
                    MessageNotify.AddDebugLog($"显示方法_当前相机：{imageInfo.showHalconWindow.m_strCamName}当前ID:{imageInfo.showHalconWindow.m_nImageFrameID}图像内部剩余数：{count}");
                }
                target_SaveQueue.Enqueue(imageInfo);
                if (screenshotImage != null)
                {
                   
                    if(screenshotImage.HighDefinition_Screenshot)
                    {
                        imageInfosHightScreenshotSave.Enqueue(screenshotImage);
                        autoEventHighScreenshot.Set();
                    }
                    else
                    {
                        target_SaveScreenshotQueue.Enqueue(screenshotImage);
                    }
                   
                }

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error("显示图像错误" + $"来源：{Source}-{operation}", ex);
            }
        }

        private async Task ExecuteImageOperationSaveAsync(Operation_ImageInfo imageInfo, QueuePackage<Operation_ImageInfo> target_DisposeQueue, string operation)
        {
            HObject hoImage = null;
            try
            {

                ExecuteResult executeResult;
                if (!imageInfo.showHalconWindow.bIs3D)
                {
                    if (imageInfo.showHalconWindow.m_nMode == 1)
                    {


                        HOperatorSet.CountSeconds(out HTuple secondstart);
                        HOperatorSet.GetImageSize(imageInfo.showHalconWindow.m_hImage, out HTuple width, out HTuple height);

                        if (height > 32000)
                        {
                            HOperatorSet.HomMat2dIdentity(out HTuple HomMat2dIdentity);
                            double scaleX = 1;
                            double scaleY = 1;
                            double a = (width / height);
                            a = a < 0.25 ? 0.25 : a;
                            a = a * 0.75;
                            scaleX = a;
                            scaleY = a;
                            HOperatorSet.HomMat2dScale(HomMat2dIdentity, scaleX, scaleY, 0, 0, out HTuple HomMat2dScale);
                            HOperatorSet.AffineTransImage(imageInfo.showHalconWindow.m_hImage, out hoImage, HomMat2dScale, "nearest_neighbor", "true");
                            //HOperatorSet.ZoomImageSize(imageInfo.showHalconWindow.m_hImage, out hoImage, width, 32000, "constant");
                             executeResult = SaveImageTool.SaveImage(hoImage, imageInfo.imageSaveFormat, imageInfo.str_ImageSaveBasePath + imageInfo.str_ImageSaveRelativePath, imageInfo.str_ImageSaveName);
                            HomMat2dIdentity?.Dispose();
                            HomMat2dScale?.Dispose();
                            target_DisposeQueue.Enqueue(imageInfo);
                        }
                        else
                        {
                            executeResult = SaveImageTool.SaveImageQue(imageInfo.showHalconWindow.m_hImage, imageInfo, target_DisposeQueue, imageInfo.imageSaveFormat, imageInfo.str_ImageSaveBasePath + imageInfo.str_ImageSaveRelativePath, imageInfo.str_ImageSaveName);
                        }
                        HOperatorSet.CountSeconds(out HTuple secondend);
                        double time = (double)(secondend - secondstart) * 1000;
                        MessageNotify.AddDebugLog("产品图片保存耗时：" + time.ToString());

                    }
              
                    else 
                    {

                        executeResult = SaveImageTool.SaveImageQue(imageInfo.showHalconWindow.m_hImage,imageInfo,target_DisposeQueue, imageInfo.imageSaveFormat, imageInfo.str_ImageSaveBasePath + imageInfo.str_ImageSaveRelativePath, imageInfo.str_ImageSaveName);
          
                    }

                }
                else
                {
                    Image3DClass image3DClass = imageInfo.showHalconWindow.Image3DClass.Copy();
                    string RelativePath = imageInfo.str_ImageSaveRelativePath + "深度图" + "\\";
                    executeResult = SaveImageTool.SaveDeepthImage(image3DClass, imageInfo.imageDeepthSaveFormat, imageInfo.str_ImageSaveBasePath + RelativePath, imageInfo.str_ImageSaveName, imageInfo.bIsCompression);
                     RelativePath = imageInfo.str_ImageSaveRelativePath + "亮度图" + "\\";
                    executeResult = SaveImageTool.SaveImage(image3DClass.Image_Light, imageInfo.imageSaveFormat, imageInfo.str_ImageSaveBasePath + RelativePath, imageInfo.str_ImageSaveName);
                    RelativePath = imageInfo.str_ImageSaveRelativePath + "彩虹图" + "\\";
                    executeResult = SaveImageTool.SaveImage(image3DClass.Image2D, imageInfo.imageSaveFormat, imageInfo.str_ImageSaveBasePath + RelativePath, imageInfo.str_ImageSaveName);
                    target_DisposeQueue.Enqueue(imageInfo);
                }

                if (!executeResult.IsSuccess)
                {
                    Log4netRecord.ErrorLog.Info("保存图像异常" + $"来源：{Source}-{operation}" + "\r\n" + executeResult.Message);
                }
       
             
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error("保存图像错误" + $"来源：{Source}-{operation}", ex);
            }
           


        }

        private async Task ExecuteScreenshotImageOperationSaveDisposeAsync(Operation_ImageInfo imageInfo, string operation)
        {
            try
            {
                ExecuteResult executeResult = SaveImageTool.SaveImage(imageInfo.showHalconWindow.m_hImage, imageInfo.screenshotSaveFormat, imageInfo.str_ScreenshotSaveBasePath + imageInfo.str_ScreenshotSaveRelativePath, imageInfo.str_ScreenshotSaveName);
                if (!executeResult.IsSuccess)
                {
                    Log4netRecord.ErrorLog.Info("截图保存异常" + $"来源：{Source}-{operation}" + "\r\n" + executeResult.Message);
                }

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error("截图保存释放错误" + $"来源：{Source}-{operation}", ex);
            }
            //imageInfo?.Dispose();
        }

        private bool TryDequeue(QueuePackage<Operation_ImageInfo> queue, out Operation_ImageInfo imageInfo, int N, string operation)
        {
            try
            {
           

                if (queue.Count > N)
                {
                    imageInfo = queue.Dequeue();
                    return true;
                }

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error("队列移出错误" + $"来源：{Source}-{operation}", ex);
            }
            imageInfo = null;
            return false;
        }

        private void QueueLengthLimit(QueuePackage<Operation_ImageInfo> queue, int limit, string operation)
        {
            try
            {
                if (queue.Count > limit)
                {
                    while (queue.Count > 5)
                    {
                        queue.Dequeue()?.Dispose();
                    }
                    Log4netRecord.ErrorLog.Info($"队列长度超过设定值{limit}已自动释放" + $"来源：{Source}-{operation}");
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error("队列长度限制错误" + $"来源：{Source}-{operation}", ex);
            }
        }

        public void Dispose()
        {
            try
            {
                if (imageInfosShow != null)
                {
                    while (imageInfosShow.Count > 0)
                    {
                        imageInfosShow.Dequeue()?.Dispose();
                    }
                }
                if (imageInfosSave != null)
                {
                    while (imageInfosSave.Count > 0)
                    {
                        imageInfosSave.Dequeue()?.Dispose();
                    }
                }

                if (imageInfosScreenshotSave != null)
                {
                    while (imageInfosScreenshotSave.Count > 0)
                    {
                        imageInfosScreenshotSave.Dequeue()?.Dispose();
                    }
                }



                if (imageInfosDispose != null)
                {
                    while (imageInfosDispose.Count > 0)
                    {
                        imageInfosDispose.Dequeue()?.Dispose();
                    }
                }


                if(imageInfosHightScreenshotSave != null)
                {
                    while (imageInfosHightScreenshotSave.Count > 0)
                    {
                        imageInfosHightScreenshotSave.Dequeue()?.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error("清空队列错误" + $"来源：{Source}", ex);
            }
        }
    }
    /// <summary>
    /// 工作类-图像信息
    /// </summary>
    public class Operation_ImageInfo : IDisposable
    {
        /// <summary>
        /// 显示图像信息
        /// </summary>
        public ShowHalconWindow showHalconWindow;

        //原图
        public string str_ImageSaveBasePath;
        public string str_ImageSaveRelativePath;
        public string str_ImageSaveName;
        public ImageSaveFormat imageSaveFormat;
        public ImageDeepthSaveFormat imageDeepthSaveFormat;
        public bool bIsSaveImage;
        public bool bIsShowImage;

        //截图
        public bool bIsSaveScreenshot;
        public bool HighDefinition_Screenshot = false;// 高清晰度-截图
        public bool bIsCompression = true;
        public string str_ScreenshotSaveBasePath;
        public string str_ScreenshotSaveRelativePath;
        public string str_ScreenshotSaveName;

        public ImageSaveFormat screenshotSaveFormat;

        /// <summary>
        /// 图像帧ID
        /// </summary>
        public long m_nImageFrameID = 0;

        public Operation_ImageInfo Copy()
        {
            Operation_ImageInfo info = new Operation_ImageInfo();
            info.str_ImageSaveBasePath = str_ImageSaveBasePath;
            info.str_ImageSaveRelativePath = str_ImageSaveRelativePath;
            info.str_ImageSaveName = str_ImageSaveName;
            info.imageSaveFormat = imageSaveFormat;
            info.imageDeepthSaveFormat = imageDeepthSaveFormat;

            info.bIsSaveImage = bIsSaveImage;
            info.bIsShowImage = bIsShowImage;
            info.bIsSaveScreenshot = bIsSaveScreenshot;
            info.HighDefinition_Screenshot = HighDefinition_Screenshot;
            info.bIsCompression = bIsCompression;

            info.str_ScreenshotSaveBasePath = str_ScreenshotSaveBasePath;
            info.str_ScreenshotSaveRelativePath = str_ScreenshotSaveRelativePath;
            info.str_ScreenshotSaveName = str_ScreenshotSaveName;
            info.screenshotSaveFormat = screenshotSaveFormat;
            info.showHalconWindow = showHalconWindow?.Copy();

            return info;
        }


        public void Dispose()
        {
            showHalconWindow?.Dispose();
        }
    }
}

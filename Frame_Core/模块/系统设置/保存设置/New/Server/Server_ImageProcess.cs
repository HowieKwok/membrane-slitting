using FrameCommunity;
using FrameCommunity.Data;
using LogTool;
using NPOI.OpenXmlFormats.Dml.Diagram;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    public delegate void ImageShowDelegate(ShowHalconWindow showHalconWindow);  
    public delegate void HighScreenshotDelegate(ShowHalconWindow showHalconWindow);
    public delegate void ImageSaveDelegate(ShowHalconWindow showHalconWindow);

    public static class Server_ImageProcess
    {
        public static async Task ShowImagesAsync(ImageShowDelegate delegateEvent, ImageQueueData source_ShowQueue, ImageQueueData target_SaveQueue)
        {
            try
            {
                if (source_ShowQueue.DataQueue.Count > source_ShowQueue.QueuelimitCount)
                {
                    while (source_ShowQueue.DataQueue.Count > source_ShowQueue.QueueReserveCount)
                    {
                        source_ShowQueue.DataQueue.Dequeue()?.Dispose();
                    }
                    MessageNotify.AddDebugLog($"【{source_ShowQueue.Name}】-【显示】队列长度超过设定值{source_ShowQueue.QueuelimitCount}已自动释放");
                }

                if (source_ShowQueue.DataQueue.Count > 0)
                {
                    ShowHalconWindow showHalconWindow = source_ShowQueue.DataQueue.Dequeue();
                    if (showHalconWindow != null)
                    {
                        if (showHalconWindow.m_imageSavePathInfo.IsShow)
                        {
                            if (!source_ShowQueue.State)
                            {
                                source_ShowQueue.State = true;
                                await Task.Run(() =>
                                {
                                    try
                                    {
                                        delegateEvent?.Invoke(showHalconWindow);
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageNotify.AddErrorLog($"【{source_ShowQueue.Name}】-【显示】异步图像错误", ex);
                                    }
                                    try
                                    {
                                        target_SaveQueue.DataQueue.Enqueue(showHalconWindow);
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageNotify.AddErrorLog($"【{source_ShowQueue.Name}】-【显示】异步图像错误", ex);
                                    }
                                    source_ShowQueue.State = false;
                                });
                            }
                        }
                        else
                        {
                            target_SaveQueue.DataQueue.Enqueue(showHalconWindow);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageNotify.AddErrorLog($"【{source_ShowQueue.Name}】-【显示】异步图像错误", ex);
            }
        }
    
        public static async Task SaveImagesAsync(ImageSaveDelegate delegateEvent, ImageQueueData target_SaveQueue, ImageQueueData target_DisposeQueue)
        {
            try
            {
                //保存
                if (target_SaveQueue.DataQueue.Count > target_SaveQueue.QueuelimitCount)
                {
                    while (target_SaveQueue.DataQueue.Count > target_SaveQueue.QueueReserveCount)
                    {
                        target_SaveQueue.DataQueue.Dequeue()?.Dispose();
                    }
                    MessageNotify.AddDebugLog($"【{target_SaveQueue.Name}】-【保存】队列长度超过设定值{target_SaveQueue.QueuelimitCount}已自动释放");
                }

                if (target_SaveQueue.DataQueue.Count > 0)
                {
                    ShowHalconWindow showHalconWindow = target_SaveQueue.DataQueue.Dequeue();
                    if (showHalconWindow != null)
                    {
                        if (showHalconWindow.m_imageSavePathInfo.IsSave)
                        {
                            if (!target_SaveQueue.State)
                            {
                                target_SaveQueue.State = true;
                                await Task.Run(() =>
                                {
                                    try
                                    {
                                        showHalconWindow.m_strCamName = target_SaveQueue.Name;
                                        delegateEvent?.Invoke(showHalconWindow);               
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageNotify.AddErrorLog($"【{target_SaveQueue.Name}】-【保存】异步图像错误", ex);
                                    }
                                    try
                                    {
                                        target_DisposeQueue.DataQueue.Enqueue(showHalconWindow);
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageNotify.AddErrorLog($"【{target_SaveQueue.Name}】-【保存】异步图像错误", ex);
                                    }
                                    target_SaveQueue.State = false;
                                });
                            }
                        }
                        else
                        {
                            target_DisposeQueue.DataQueue.Enqueue(showHalconWindow);
                        }
                    }

                }


                //释放
                if (target_DisposeQueue.DataQueue.Count > target_DisposeQueue.QueueReserveCount)
                {
                    target_DisposeQueue.DataQueue.Dequeue()?.Dispose();
                }
            }
            catch (Exception ex)
            {
                MessageNotify.AddErrorLog($"【{target_SaveQueue.Name}】-【保存】异步图像错误", ex);
            }
        }

        public static async Task HighScreenshotAsync(HighScreenshotDelegate delegateEvent, ImageQueueData target_HighScreenshotQueue)
        {
            try
            {
                if (target_HighScreenshotQueue.DataQueue.Count > target_HighScreenshotQueue.QueuelimitCount)
                {
                    while (target_HighScreenshotQueue.DataQueue.Count > target_HighScreenshotQueue.QueueReserveCount)
                    {
                        target_HighScreenshotQueue.DataQueue.Dequeue()?.Dispose();
                    }
                    MessageNotify.AddDebugLog($"【{target_HighScreenshotQueue.Name}】-【高清截图】队列长度超过设定值{target_HighScreenshotQueue.QueuelimitCount}已自动释放");
                }

                if (target_HighScreenshotQueue.DataQueue.Count > 0)
                {
                    ShowHalconWindow showHalconWindow = target_HighScreenshotQueue.DataQueue.Dequeue();
                    if (showHalconWindow != null)
                    {
                        if (!target_HighScreenshotQueue.State)
                        {
                            target_HighScreenshotQueue.State = true;
                            await Task.Run(() =>
                            {
                                try
                                {
                                    delegateEvent?.Invoke(showHalconWindow);                                  
                                }
                                catch (Exception ex)
                                {
                                    MessageNotify.AddErrorLog($"【{target_HighScreenshotQueue.Name}】-【高清截图】异步图像错误", ex);
                                }
                                try
                                {
                                    //showHalconWindow.Dispose();
                                }
                                catch (Exception ex)
                                {
                                    MessageNotify.AddErrorLog($"【{target_HighScreenshotQueue.Name}】-【高清截图】异步图像错误", ex);
                                }
                            
                                target_HighScreenshotQueue.State = false;
                            });
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                MessageNotify.AddErrorLog($"【{target_HighScreenshotQueue.Name}】-【高清截图】异步图像错误", ex);
            }
        }

        public static async Task SaveScreenshotAsync(ImageSaveDelegate delegateEvent, ImageQueueData target_SaveScreenshotQueue)
        {
            try
            {
                //保存
                if (target_SaveScreenshotQueue.DataQueue.Count > target_SaveScreenshotQueue.QueuelimitCount)
                {
                    while (target_SaveScreenshotQueue.DataQueue.Count > target_SaveScreenshotQueue.QueueReserveCount)
                    {
                        target_SaveScreenshotQueue.DataQueue.Dequeue()?.Dispose();
                    }
                    MessageNotify.AddDebugLog($"【{target_SaveScreenshotQueue.Name}】-【截图保存】队列长度超过设定值{target_SaveScreenshotQueue.QueuelimitCount}已自动释放");
                }

                if (target_SaveScreenshotQueue.DataQueue.Count > 0)
                {
                    ShowHalconWindow showHalconWindow = target_SaveScreenshotQueue.DataQueue.Dequeue();
                    if (showHalconWindow != null)
                    {
                        if (!target_SaveScreenshotQueue.State)
                        {
                            target_SaveScreenshotQueue.State = true;
                            await Task.Run(() =>
                            {
                                try
                                {
                                    showHalconWindow.m_strCamName = target_SaveScreenshotQueue.Name;
                                    delegateEvent?.Invoke(showHalconWindow);            
                                }
                                catch (Exception ex)
                                {
                                    MessageNotify.AddErrorLog($"【{target_SaveScreenshotQueue.Name}】-【截图保存】异步图像错误", ex);
                                }
                                try
                                {
                                    //释放
                                    showHalconWindow?.Dispose();
                                }
                                catch (Exception ex)
                                {
                                    MessageNotify.AddErrorLog($"【{target_SaveScreenshotQueue.Name}】-【截图保存】异步图像错误", ex);
                                }
                             
                                target_SaveScreenshotQueue.State = false;
                            });
                        }
                    }

                }


            }
            catch (Exception ex)
            {
                MessageNotify.AddErrorLog($"【{target_SaveScreenshotQueue.Name}】-【截图保存】异步图像错误", ex);
            }
        }


        public static void ImageShowCallback(NewDisplayWindowClass newDisplayWindowClass, ShowHalconWindow showHalconWindow)
        {
            newDisplayWindowClass.RepaintCam(showHalconWindow);        
        }

        public static void ImageShowCallback(NewDisplayWindowClass newDisplayWindowClass, ShowHalconWindow showHalconWindow, ImageQueueData target_SaveScreenshotQueue, ImageQueueData target_HighScreenshotQueue)
        {
            newDisplayWindowClass.RepaintCam(showHalconWindow);
            if (showHalconWindow.m_imageSavePathInfo.IsScreenshot)
            {
                if (showHalconWindow.m_imageSavePathInfo.IsHighDefinitionCapture)
                {
                    ShowHalconWindow showHalconWindow1 = showHalconWindow.Copy();
                    if (showHalconWindow1 != null)
                    {
                        target_HighScreenshotQueue.DataQueue.Enqueue(showHalconWindow1);
                    }
                }
                else
                {
                    newDisplayWindowClass.ScreenshotImage(out ShowHalconWindow screenshotImage);
                    if (screenshotImage != null)
                    {                 
                        target_SaveScreenshotQueue.DataQueue.Enqueue(screenshotImage);
                    }
                }
            }
        }

        public static void HighScreenshotCallback(NewDisplayWindowClass newDisplayWindowClass, ShowHalconWindow showHalconWindow, ImageQueueData target_SaveScreenshotQueue)
        {
            if (newDisplayWindowClass?.hightScreenshot == null) return;
            newDisplayWindowClass.hightScreenshot.HightDefinition_Screenshot(showHalconWindow, out ShowHalconWindow screenshotImage);
            if (screenshotImage != null)
            {
                target_SaveScreenshotQueue.DataQueue.Enqueue(screenshotImage);
            }

        }

        public static void ImageSaveCallback(ShowHalconWindow showHalconWindow)
        {
            ExecuteResult executeResult = SaveImageTool.SaveImage(showHalconWindow.m_hImage, showHalconWindow.m_imageSavePathInfo.FileFormat, showHalconWindow.m_imageSavePathInfo.Path, showHalconWindow.m_imageSavePathInfo.FileName);

            if (!executeResult.IsSuccess)
            {
                MessageNotify.AddErrorLog($"【{showHalconWindow.m_strCamName}】-【保存】异步图像错误" + "\r\n" + executeResult.Message);
            }
        }

        public static void ScreenshotSaveCallback(ShowHalconWindow showHalconWindow)
        {
            ExecuteResult executeResult = SaveImageTool.SaveImage(showHalconWindow.m_hImage, showHalconWindow.m_imageSavePathInfo.ScreenshotFileFormat, showHalconWindow.m_imageSavePathInfo.ScreenshotPath, showHalconWindow.m_imageSavePathInfo.ScreenshotFileName);

            if (!executeResult.IsSuccess)
            {
                MessageNotify.AddErrorLog($"【{showHalconWindow.m_strCamName}】-【截图保存】异步图像错误" + "\r\n" + executeResult.Message);
            }
        }
    }  
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ViewROI2;
using System.Threading.Tasks;
using HalconDotNet;
using LogTool;
using System.Windows.Forms;
using IKapBoardClassLibrary;
using Frame_Core;

namespace Frame_Core
{
    public class DisplayWindowClass : IDisposable
    {

        /// <summary>
        /// 检测结果图像显示窗体对象
        /// </summary>
        private HWndCtrl m_hWndCtrl_Display_Process_Cam;
        private ROIController m_roiController_Process_Cam = new ROIController();
        private ROIRectangle2 m_roiRectangle2_Process_Cam = new ROIRectangle2();


        /// <summary>
        /// 检测结果图像显示窗体对象
        /// </summary>
        public HWndCtrl m_hWndCtrl_Display_Cam;
        private ROIController m_roiController_Cam = new ROIController();
        private ROIRectangle2 m_roiRectangle2_Cam = new ROIRectangle2();
        //private ShowHalconWindow m_ShowHalconWindow = new ShowHalconWindow();
        public HWindowControl m_hWindowControl_Cam;

        private HTuple hv_dumpWindowHandle;//保存高清截图窗口

        private bool m_bIsWindowShow = true;
        private int m_nLineWidth = 2;


        public Operation_ImageProcessor operation_Image;
        public Operation_ImageInfo operation_ImageInfo = new Operation_ImageInfo();

        public string source;
        TextBox textBox = null;
        PictureBox pictureBox = null;

        public DisplayWindowClass(HWindowControl hWindowControl, string _source)
        {

            source = _source;
            if (operation_Image == null) operation_Image = new Operation_ImageProcessor(source);
            if (hWindowControl != null)
            {
                m_hWindowControl_Cam = hWindowControl;
                m_hWndCtrl_Display_Cam = new HWndCtrl(m_hWindowControl_Cam);
                m_roiController_Cam = new ROIController();
                m_hWndCtrl_Display_Cam.setViewState(HWndCtrl.MODE_VIEW_ZOOM);
                m_hWndCtrl_Display_Cam.setViewState(HWndCtrl.MODE_VIEW_MOVE);
                m_hWndCtrl_Display_Cam.ReDrawEvent += new HWndCtrl.ReDrawDelegate(ShowRoiResult);     //更改测量矩形，重绘测量结果

                //Resize window
                HOperatorSet.SetWindowAttr("background_color", "black");
                HOperatorSet.OpenWindow(0, 0, 512, 512, 0, "buffer", "", out hv_dumpWindowHandle);
            }

        }

        public DisplayWindowClass(HWindowControl hWindowControl, TextBox _textBox, PictureBox _pictureBox, CameraEnum cameraEnum, string _source)
        {
            textBox = _textBox;
            pictureBox = _pictureBox;
            pictureBox.Click -= btn_Click;
            pictureBox.Click += btn_Click;

            source = _source;
            if (operation_Image == null) operation_Image = new Operation_ImageProcessor(source);
            if (hWindowControl != null)
            {
                m_hWindowControl_Cam = hWindowControl;
                m_hWndCtrl_Display_Cam = new HWndCtrl(m_hWindowControl_Cam);
                m_roiController_Cam = new ROIController();
                m_hWndCtrl_Display_Cam.setViewState(HWndCtrl.MODE_VIEW_ZOOM);
                m_hWndCtrl_Display_Cam.setViewState(HWndCtrl.MODE_VIEW_MOVE);
                m_hWndCtrl_Display_Cam.ReDrawEvent += new HWndCtrl.ReDrawDelegate(ShowRoiResult);     //更改测量矩形，重绘测量结果
                m_hWndCtrl_Display_Cam.viewPort.HMouseMove += HWC_HMouseMove;

                //Resize window
                HOperatorSet.SetWindowAttr("background_color", "black");
                HOperatorSet.OpenWindow(0, 0, 512, 512, 0, "buffer", "", out hv_dumpWindowHandle);
            }


        }

        public DisplayWindowClass(HWindowControl hWindowControl, TextBox _textBox, PictureBox _pictureBox, string _source)
        {
            textBox = _textBox;
            pictureBox = _pictureBox;
            pictureBox.Click -= btn_Click;
            pictureBox.Click += btn_Click;

            source = _source;
            if (operation_Image == null) operation_Image = new Operation_ImageProcessor(source);
            if (hWindowControl != null)
            {
                m_hWindowControl_Cam = hWindowControl;
                m_hWndCtrl_Display_Cam = new HWndCtrl(m_hWindowControl_Cam);
                m_roiController_Cam = new ROIController();
                m_hWndCtrl_Display_Cam.setViewState(HWndCtrl.MODE_VIEW_ZOOM);
                m_hWndCtrl_Display_Cam.setViewState(HWndCtrl.MODE_VIEW_MOVE);
                m_hWndCtrl_Display_Cam.ReDrawEvent += new HWndCtrl.ReDrawDelegate(ShowRoiResult);     //更改测量矩形，重绘测量结果
                m_hWndCtrl_Display_Cam.viewPort.HMouseMove += HWC_HMouseMove;

                //Resize window
                HOperatorSet.SetWindowAttr("background_color", "black");
                HOperatorSet.OpenWindow(0, 0, 512, 512, 0, "buffer", "", out hv_dumpWindowHandle);
            }


        }

        /// <summary>
        /// 显示图像
        /// </summary>
        /// <param name="image"></param>
        /// <param name="showHalconWindow"></param>
        /// <param name="isScreenshot">是否截图</param>
        /// <param name="screenshot">返回截图duil</param>
        /// <param name="bIsShowWindow">是否显示图片</param>
        public ExecuteResult RepaintCam(Operation_ImageInfo imageInfo, out Operation_ImageInfo screenshotImage)
        {
            ExecuteResult executeResult = new ExecuteResult();
            screenshotImage = null;
            Operation_ImageInfo screenshotImageTemp = null;

            try
            {
                m_hWindowControl_Cam.Invoke(new Action(() =>
                {
                    try
                    {


                        int FontLine = 0;

                        object Locker = new object();
                        lock (Locker)
                        {
                            operation_ImageInfo = imageInfo;
                            if (!operation_ImageInfo.showHalconWindow.bIs3D)
                            {
                                if (operation_ImageInfo.showHalconWindow.m_hImage != null)
                                {
                                    m_hWndCtrl_Display_Cam.addIconicVar(operation_ImageInfo.showHalconWindow.m_hImage);

                                }
                            }
                            else
                            {
                                if (operation_ImageInfo.showHalconWindow.Image3DClass.Image2D != null)
                                {
                                    m_hWndCtrl_Display_Cam.addIconicVar(operation_ImageInfo.showHalconWindow.Image3DClass.Image2D);
                                }
                            }


                            //显示图片区域文本信息
                            RePaint();


                            if (imageInfo.bIsSaveScreenshot)
                            {
                                HObject Copy_Win_Image = null;
                                try
                                {
                                    //高清截图
                                    if (imageInfo.HighDefinition_Screenshot)
                                    {
                                        //计时            
                                        //HOperatorSet.CountSeconds(out HTuple StartTime);
                                        //HTuple hv_Height = 0; HTuple hv_Width = 0;
                                        //if (!operation_ImageInfo.showHalconWindow.bIs3D)
                                        //{
                                        //    HOperatorSet.GetImageSize(operation_ImageInfo.showHalconWindow.m_hImage, out hv_Height, out hv_Width);

                                        //}
                                        //else
                                        //{
                                        //    HOperatorSet.GetImageSize(operation_ImageInfo.showHalconWindow.Image3DClass.Image2D, out hv_Height, out hv_Width);
                                        //}

                                        //HOperatorSet.SetWindowExtents(hv_dumpWindowHandle, 0, 0, hv_Width, hv_Height);
                                        //HOperatorSet.SetPart(hv_dumpWindowHandle, 0, 0, hv_Width - 1, hv_Height - 1);
                                        //HOperatorSet.CountSeconds(out HTuple EndTime);
                                        //double hTime1 = ((EndTime - StartTime) * 1000).D;//抓边算法耗时

                                        //MessageNotify.AddDebugLog("开高清窗口耗时：" + hTime1.ToString());
                                        ////Console.WriteLine("开窗口耗时：" + hTime1.ToString());
                                        //HOperatorSet.DispObj(operation_ImageInfo.showHalconWindow.m_hImage, hv_dumpWindowHandle);
                                        //ShowRoiResult(hv_dumpWindowHandle);
                                        //HOperatorSet.DumpWindowImage(out Copy_Win_Image, hv_dumpWindowHandle);
                                        //HOperatorSet.CountSeconds(out EndTime);
                                        //hTime1 = ((EndTime - StartTime) * 1000).D;//抓边算法耗时
                                        //MessageNotify.AddDebugLog("开高清窗口总耗时：" + hTime1.ToString());
                                        screenshotImageTemp = new Operation_ImageInfo();
                                        //HightDefinition_Screenshot(imageInfo, out screenshotImageTemp);
                                        screenshotImageTemp = operation_ImageInfo.Copy();
                                        //screenshotImageTemp.showHalconWindow = new ShowHalconWindow();
                                        //screenshotImageTemp.showHalconWindow.m_hImage = Copy_Win_Image;
                                        //screenshotImageTemp.bIsSaveScreenshot = imageInfo.bIsSaveScreenshot;
                                        //screenshotImageTemp.str_ScreenshotSaveBasePath = imageInfo.str_ScreenshotSaveBasePath;
                                        //screenshotImageTemp.str_ScreenshotSaveRelativePath = imageInfo.str_ScreenshotSaveRelativePath;
                                        //screenshotImageTemp.str_ScreenshotSaveName = imageInfo.str_ScreenshotSaveName;
                                        //screenshotImageTemp.screenshotSaveFormat = imageInfo.screenshotSaveFormat;
                                        //hv_Width.Dispose();
                                        //hv_Height.Dispose();

                                        ////计时            
                                        //HOperatorSet.CountSeconds(out EndTime);
                                        //hTime1 = ((EndTime - StartTime) * 1000).D;//抓边算法耗时

                                    }
                                    //直接截图
                                    else
                                    {
                                        HOperatorSet.DumpWindowImage(out Copy_Win_Image, m_hWindowControl_Cam.HalconWindow);
                                        screenshotImageTemp = new Operation_ImageInfo();
                                        screenshotImageTemp.showHalconWindow = new ShowHalconWindow();
                                        screenshotImageTemp.showHalconWindow.m_hImage = Copy_Win_Image;
                                        screenshotImageTemp.showHalconWindow.bIs3D = imageInfo.showHalconWindow.bIs3D;
                                        screenshotImageTemp.showHalconWindow.m_nMode = imageInfo.showHalconWindow.m_nMode;
                                        screenshotImageTemp.showHalconWindow.m_strCamName = imageInfo.showHalconWindow.m_strCamName;
                                        screenshotImageTemp.bIsSaveScreenshot = imageInfo.bIsSaveScreenshot;
                                        screenshotImageTemp.str_ScreenshotSaveBasePath = imageInfo.str_ScreenshotSaveBasePath;
                                        screenshotImageTemp.str_ScreenshotSaveRelativePath = imageInfo.str_ScreenshotSaveRelativePath;
                                        screenshotImageTemp.str_ScreenshotSaveName = imageInfo.str_ScreenshotSaveName;
                                        screenshotImageTemp.screenshotSaveFormat = imageInfo.screenshotSaveFormat;
                                    }

                                }
                                catch (Exception ex)
                                {
                                    Log4netRecord.ErrorLog.Error(source, ex);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log4netRecord.ErrorLog.Error(source, ex);
                    }

                }));


                screenshotImage = screenshotImageTemp;
                executeResult.IsSuccess = true;
            }
            catch (Exception ex)
            {
                executeResult.Message = ex.ToString();
                executeResult.IsSuccess = false;
                Log4netRecord.ErrorLog.Error(source, ex);
            }
            return executeResult;
        }
        public ExecuteResult HightDefinition_Screenshot(Operation_ImageInfo imageInfo, out Operation_ImageInfo screenshotImage)
        {
            ExecuteResult executeResult = new ExecuteResult();
            screenshotImage = null;
            Operation_ImageInfo screenshotImageTemp = null;
            double scaleX = 1;
            double scaleY = 1.0;
            HObject m_hImage = null;
            HTuple HomMat2dScale=null;
            try
            {
                if (imageInfo.bIsSaveScreenshot)
                {
                    HObject Copy_Win_Image = null;
                    try
                    {
                        //计时            
                        HOperatorSet.CountSeconds(out HTuple StartTime);
                        HTuple hv_Height = 0; HTuple hv_Width = 0;
                        HOperatorSet.HomMat2dIdentity(out HTuple HomMat2dIdentity);
                        if (imageInfo.showHalconWindow.m_nMode ==1)
                        {
                            HOperatorSet.GetImageSize(imageInfo.showHalconWindow.m_hImage, out hv_Width, out hv_Height);
                            if ((hv_Height / hv_Width) > 1.25)
                            {
                                double a = (hv_Width / hv_Height);
                                a=a<0.25?0.25:a;
                                a = a * 0.75;
                                scaleX = a;
                                scaleY = a;
                            }
                            HOperatorSet.HomMat2dScale(HomMat2dIdentity, scaleX, scaleY, 0, 0, out  HomMat2dScale);

                            HOperatorSet.AffineTransImage(imageInfo.showHalconWindow.m_hImage, out m_hImage, HomMat2dScale, "nearest_neighbor", "true");
                            HOperatorSet.CountSeconds(out HTuple EndTime1);
                            double hTime2 = ((EndTime1 - StartTime) * 1000).D;//抓边算法耗时

                            MessageNotify.AddDebugLog("压缩产品高清截图耗时：" + hTime2.ToString());
                        }

                     
                        // HOperatorSet.ZoomImageFactor(operation_ImageInfo.showHalconWindow.m_hImage, out operation_ImageInfo.showHalconWindow.m_hImage, 0.1, 0.1, "constant");
                        if (!imageInfo.showHalconWindow.bIs3D)
                        {
                            if(imageInfo.showHalconWindow.m_nMode==1)
                            {
                                HOperatorSet.GetImageSize(m_hImage, out hv_Width, out hv_Height);
                            }
                            else
                            {
                                HOperatorSet.GetImageSize(imageInfo.showHalconWindow.m_hImage, out hv_Width, out hv_Height);
                            }
                           

                        }
                        else
                        {
                            HOperatorSet.GetImageSize(imageInfo.showHalconWindow.Image3DClass.Image2D, out hv_Width, out hv_Height);
                        }
                        HomMat2dIdentity?.Dispose();
                        HomMat2dScale?.Dispose();

                        HOperatorSet.SetWindowExtents(hv_dumpWindowHandle, 0, 0, hv_Width, hv_Height);
                        HOperatorSet.SetPart(hv_dumpWindowHandle, 0, 0, (hv_Height - 1), hv_Width - 1);
                        HOperatorSet.CountSeconds(out HTuple EndTime);
                        double hTime1 = ((EndTime - StartTime) * 1000).D;//抓边算法耗时

                        MessageNotify.AddDebugLog("开高清窗口耗时：" + hTime1.ToString());
                        //Console.WriteLine("开窗口耗时：" + hTime1.ToString());
                        if (!imageInfo.showHalconWindow.bIs3D)
                        {
                            if(imageInfo.showHalconWindow.m_nMode == 1)
                            {
                                HOperatorSet.DispObj(m_hImage, hv_dumpWindowHandle);
                            }
                            else
                            {
                                HOperatorSet.DispObj(imageInfo.showHalconWindow.m_hImage, hv_dumpWindowHandle);
                            }
                           
                        }
                        else
                        {
                            HOperatorSet.DispObj(imageInfo.showHalconWindow.Image3DClass.Image2D, hv_dumpWindowHandle);
                        }
                        if(imageInfo.showHalconWindow.m_nMode == 1)
                        {
                            ShowRoiResult(hv_dumpWindowHandle, scaleX, scaleY);
                        }
                        else
                        {
                            ShowRoiResult(hv_dumpWindowHandle);
                        }
                      
                        HOperatorSet.DumpWindowImage(out Copy_Win_Image, hv_dumpWindowHandle);
                        HOperatorSet.CountSeconds(out EndTime);
                        hTime1 = ((EndTime - StartTime) * 1000).D;//抓边算法耗时
                        MessageNotify.AddDebugLog("开高清窗口总耗时：" + hTime1.ToString());
                        screenshotImageTemp = new Operation_ImageInfo();
                        screenshotImageTemp.showHalconWindow = new ShowHalconWindow();
                        screenshotImageTemp.showHalconWindow.m_strCamName=imageInfo.showHalconWindow.m_strCamName;
                        screenshotImageTemp.showHalconWindow.m_nMode = imageInfo.showHalconWindow.m_nMode;
                        screenshotImageTemp.showHalconWindow.bIs3D = imageInfo.showHalconWindow.bIs3D;
                        screenshotImageTemp.showHalconWindow.m_nProductID = imageInfo.showHalconWindow.m_nProductID;
                        screenshotImageTemp.showHalconWindow.m_nImageFrameID = imageInfo.showHalconWindow.m_nImageFrameID;
                        screenshotImageTemp.showHalconWindow.m_hImage = Copy_Win_Image;
                        screenshotImageTemp.bIsSaveScreenshot = imageInfo.bIsSaveScreenshot;
                        screenshotImageTemp.str_ScreenshotSaveBasePath = imageInfo.str_ScreenshotSaveBasePath;
                        screenshotImageTemp.str_ScreenshotSaveRelativePath = imageInfo.str_ScreenshotSaveRelativePath;
                        screenshotImageTemp.str_ScreenshotSaveName = imageInfo.str_ScreenshotSaveName;
                        screenshotImageTemp.screenshotSaveFormat = imageInfo.screenshotSaveFormat;
                        hv_Width.Dispose();
                        hv_Height.Dispose();

                        //计时            
                        HOperatorSet.CountSeconds(out EndTime);
                        hTime1 = ((EndTime - StartTime) * 1000).D;//抓边算法耗时


                    }
                    catch (Exception ex)
                    {
                        Log4netRecord.ErrorLog.Error(source, ex);
                        executeResult.IsSuccess = false;
                    }

                    screenshotImage = screenshotImageTemp;
                    //executeResult.IsSuccess = true;
                }
            }
            catch (Exception ee)
            {
                executeResult.Message = ee.ToString();
                executeResult.IsSuccess = false;
                Log4netRecord.ErrorLog.Error(source, ee);
            }

            imageInfo?.Dispose();
            m_hImage?.Dispose();
            return executeResult;

        }

        //public ExecuteResult HightDefinition_Screenshot(Operation_ImageInfo imageInfo, out Operation_ImageInfo screenshotImage)
        //{
        //    ExecuteResult executeResult = new ExecuteResult();
        //    screenshotImage = null;
        //    Operation_ImageInfo screenshotImageTemp = null;
        //    try
        //    {
        //        if (imageInfo.bIsSaveScreenshot)
        //        {
        //            HObject Copy_Win_Image = null;
        //            try
        //            {
        //                //计时            
        //                HOperatorSet.CountSeconds(out HTuple StartTime);
        //                HTuple hv_Height = 0; HTuple hv_Width = 0;
        //                if (!operation_ImageInfo.showHalconWindow.bIs3D)
        //                {
        //                    HOperatorSet.GetImageSize(operation_ImageInfo.showHalconWindow.m_hImage, out hv_Height, out hv_Width);

        //                }
        //                else
        //                {
        //                    HOperatorSet.GetImageSize(operation_ImageInfo.showHalconWindow.Image3DClass.Image2D, out hv_Height, out hv_Width);
        //                }

        //                HOperatorSet.SetWindowExtents(hv_dumpWindowHandle, 0, 0, hv_Height, hv_Width);
        //                HOperatorSet.SetPart(hv_dumpWindowHandle, 0, 0, hv_Width - 1, hv_Height - 1);
        //                HOperatorSet.CountSeconds(out HTuple EndTime);
        //                double hTime1 = ((EndTime - StartTime) * 1000).D;//抓边算法耗时

        //                MessageNotify.AddDebugLog("开高清窗口耗时：" + hTime1.ToString());
        //                //Console.WriteLine("开窗口耗时：" + hTime1.ToString());
        //                if (!operation_ImageInfo.showHalconWindow.bIs3D)
        //                {
        //                    HOperatorSet.DispObj(operation_ImageInfo.showHalconWindow.m_hImage, hv_dumpWindowHandle);
        //                }
        //                else
        //                {
        //                    HOperatorSet.DispObj(operation_ImageInfo.showHalconWindow.Image3DClass.Image2D, hv_dumpWindowHandle);
        //                }

        //                ShowRoiResult(hv_dumpWindowHandle);
        //                HOperatorSet.DumpWindowImage(out Copy_Win_Image, hv_dumpWindowHandle);
        //                HOperatorSet.CountSeconds(out EndTime);
        //                hTime1 = ((EndTime - StartTime) * 1000).D;//抓边算法耗时
        //                MessageNotify.AddDebugLog("开高清窗口总耗时：" + hTime1.ToString());
        //                screenshotImageTemp = new Operation_ImageInfo();
        //                screenshotImageTemp.showHalconWindow = new ShowHalconWindow();
        //                screenshotImageTemp.showHalconWindow.m_hImage = Copy_Win_Image;
        //                screenshotImageTemp.bIsSaveScreenshot = imageInfo.bIsSaveScreenshot;
        //                screenshotImageTemp.str_ScreenshotSaveBasePath = imageInfo.str_ScreenshotSaveBasePath;
        //                screenshotImageTemp.str_ScreenshotSaveRelativePath = imageInfo.str_ScreenshotSaveRelativePath;
        //                screenshotImageTemp.str_ScreenshotSaveName = imageInfo.str_ScreenshotSaveName;
        //                screenshotImageTemp.screenshotSaveFormat = imageInfo.screenshotSaveFormat;
        //                hv_Width.Dispose();
        //                hv_Height.Dispose();

        //                //计时            
        //                HOperatorSet.CountSeconds(out EndTime);
        //                hTime1 = ((EndTime - StartTime) * 1000).D;//抓边算法耗时


        //            }
        //            catch (Exception ex)
        //            {
        //                Log4netRecord.ErrorLog.Error(source, ex);
        //                executeResult.IsSuccess = false;
        //            }

        //            screenshotImage = screenshotImageTemp;
        //            executeResult.IsSuccess = true;
        //        }
        //    }
        //    catch (Exception ee)
        //    {
        //        executeResult.Message = ee.ToString();
        //        executeResult.IsSuccess = false;
        //        Log4netRecord.ErrorLog.Error(source, ee);
        //    }
        //    imageInfo?.Dispose();
        //    return executeResult;
        //}


        private void ShowRoiResult()
        {
            try
            {
                HWindowControl HWC = m_hWindowControl_Cam;
                HWC.Invoke(new Action(() =>
                {
                    try
                    {
                        object Locker = new object();
                        HOperatorSet.GetWindowExtents(HWC.HalconWindow, out HTuple Row, out HTuple Col, out HTuple width, out HTuple height);
                        int count = 0;
                        int FontLine = 0;
                        double colPos = 0;
                        if (m_bIsWindowShow)
                        {
                            if (operation_ImageInfo.showHalconWindow.m_messageShowInfos != null)
                                foreach (var item in operation_ImageInfo.showHalconWindow.m_messageShowInfos)
                                {
                                    if (item.m_bIsWindow)
                                    {
                                        if (count > height / 20)
                                        {
                                            count = 0;
                                            colPos = width / 2;
                                            FontLine = 0;
                                        }
                                        HOperatorSet.SetColor(HWC.HalconWindow, item.m_stuGetColor.ToString());
                                        set_display_font(HWC.HalconWindow, item.m_nFontSize, "mono", "true", "false");
                                        HOperatorSet.DispText(HWC.HalconWindow, item.m_strMessage, "window", FontLine, colPos, item.m_stuGetColor.ToString(), "box", "false");

                                        FontLine += item.m_nFontSize + 10;
                                        count++;
                                    }
                                    else
                                    {
                                        HOperatorSet.SetColor(HWC.HalconWindow, item.m_stuGetColor.ToString());
                                        set_display_font(HWC.HalconWindow, item.m_nFontSize, "mono", "true", "false");
                                        HOperatorSet.DispText(HWC.HalconWindow, item.m_strMessage, "image", item.m_fRow, item.m_fCol, item.m_stuGetColor.ToString(), "box", "false");
                                    }

                                }
                            if (operation_ImageInfo.showHalconWindow.m_regionShowInfos != null)
                                foreach (var item in operation_ImageInfo.showHalconWindow.m_regionShowInfos)
                                {
                                    try
                                    {

                                        if ((item.m_hRegion?.CountObj() != null && item.m_hRegion?.CountObj() != 0))
                                        {
                                            HOperatorSet.SetColor(HWC.HalconWindow, item.m_stuGetColor.ToString());
                                            HOperatorSet.SetDraw(HWC.HalconWindow, "margin");
                                            HOperatorSet.DispObj(item.m_hRegion, HWC.HalconWindow);
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        int index = operation_ImageInfo.showHalconWindow.m_regionShowInfos.IndexOf(item);
                                    }

                                }

                            if (operation_ImageInfo.showHalconWindow.m_lineShowInfos != null)
                                foreach (var item in operation_ImageInfo.showHalconWindow.m_lineShowInfos)
                                {
                                    HOperatorSet.SetColor(HWC.HalconWindow, item.Color.ToString());
                                    //箭头显示
                                    HOperatorSet.DispArrow(HWC.HalconWindow, item.RowBegin, item.ColBegin, item.RowEnd, item.ColEnd, item.m_nLineWidth * 0.6);
                                    HOperatorSet.DispArrow(HWC.HalconWindow, item.RowEnd, item.ColEnd, item.RowBegin, item.ColBegin, item.m_nLineWidth * 0.6);
                                }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log4netRecord.ErrorLog.Error(source, ex);
                    }

                }));
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(source, ex);
            }



        }

        private void ShowRoiResult(HTuple HalconWindow)
        {
            try
            {
                try
                {
                    object Locker = new object();
                    HOperatorSet.GetWindowExtents(HalconWindow, out HTuple Row, out HTuple Col, out HTuple width, out HTuple height);
                    int count = 0;
                    int FontLine = 0;
                    int fontsize = 100;
                    double colPos = 0;
                    if (m_bIsWindowShow)
                    {

                        if (operation_ImageInfo.showHalconWindow.m_messageShowInfos != null)
                            foreach (var item in operation_ImageInfo.showHalconWindow.m_messageShowInfos)
                            {
                                if (item.m_bIsWindow)
                                {
                                    if (count > height / 20)
                                    {
                                        count = 0;
                                        colPos = width / 2;
                                        FontLine = 0;
                                    }
                                    HOperatorSet.SetColor(HalconWindow, item.m_stuGetColor.ToString());
                                    set_display_font(HalconWindow, fontsize, "mono", "true", "false");
                                    HOperatorSet.DispText(HalconWindow, item.m_strMessage, "window", FontLine, colPos, item.m_stuGetColor.ToString(), "box", "false");

                                    FontLine += fontsize + 10;
                                    count++;
                                }
                                else
                                {
                                    HOperatorSet.SetColor(HalconWindow, item.m_stuGetColor.ToString());
                                    set_display_font(HalconWindow, fontsize, "mono", "true", "false");
                                    HOperatorSet.DispText(HalconWindow, item.m_strMessage, "image", item.m_fRow, item.m_fCol, item.m_stuGetColor.ToString(), "box", "false");
                                }

                            }

                        HOperatorSet.GetLineWidth(HalconWindow, out HTuple lineWidth);
                        HOperatorSet.SetLineWidth(HalconWindow, 1);
                        if (operation_ImageInfo.showHalconWindow.m_regionShowInfos != null)
                            foreach (var item in operation_ImageInfo.showHalconWindow.m_regionShowInfos)
                            {
                                if ((item.m_hRegion?.CountObj() != null && item.m_hRegion?.CountObj() != 0))
                                {
                                    HOperatorSet.SetColor(HalconWindow, item.m_stuGetColor.ToString());
                                    HOperatorSet.SetDraw(HalconWindow, "margin");
                                    HOperatorSet.DispObj(item.m_hRegion, HalconWindow);
                                }

                            }

                        if (operation_ImageInfo.showHalconWindow.m_lineShowInfos != null)
                            foreach (var item in operation_ImageInfo.showHalconWindow.m_lineShowInfos)
                            {
                                HOperatorSet.SetColor(HalconWindow, item.Color.ToString());
                                //箭头显示
                                HOperatorSet.DispArrow(HalconWindow, item.RowBegin, item.ColBegin, item.RowEnd, item.ColEnd, item.m_nLineWidth * 0.6);
                                HOperatorSet.DispArrow(HalconWindow, item.RowEnd, item.ColEnd, item.RowBegin, item.ColBegin, item.m_nLineWidth * 0.6);
                            }
                    }
                }
                catch (Exception ex)
                {
                    Log4netRecord.ErrorLog.Error(source, ex);
                }


            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(source, ex);
            }



        }


        List<HObject> hObjects = new List<HObject>();
        private void ShowRoiResult(HTuple HalconWindow, double scaleX = 1, double scaleY = 1)
        {
            try
            {
                try
                {
                    object Locker = new object();
                    HOperatorSet.GetWindowExtents(HalconWindow, out HTuple Row, out HTuple Col, out HTuple width, out HTuple height);
                    int count = 0;
                    double FontLine = 0;
                    double fontsize = 100 * scaleY;
                    double colPos = 0;
                    hObjects.Clear();
                    if (m_bIsWindowShow)
                    {

                        if (operation_ImageInfo.showHalconWindow.m_messageShowInfos != null)
                            foreach (var item in operation_ImageInfo.showHalconWindow.m_messageShowInfos)
                            {
                                if (item.m_bIsWindow)
                                {
                                    if (count > height / 20)
                                    {
                                        count = 0;
                                        colPos = width / 2;
                                        FontLine = 0;
                                    }
                                    HOperatorSet.SetColor(HalconWindow, item.m_stuGetColor.ToString());
                                    set_display_font(HalconWindow, fontsize, "mono", "true", "false");
                                    HOperatorSet.DispText(HalconWindow, item.m_strMessage, "window", FontLine, colPos, item.m_stuGetColor.ToString(), "box", "false");

                                    FontLine += fontsize + 10;
                                    count++;
                                }
                                else
                                {
                                    HOperatorSet.SetColor(HalconWindow, item.m_stuGetColor.ToString());
                                    set_display_font(HalconWindow, fontsize, "mono", "true", "false");
                                    HOperatorSet.DispText(HalconWindow, item.m_strMessage, "image", item.m_fRow, item.m_fCol, item.m_stuGetColor.ToString(), "box", "false");
                                }

                            }

                        HOperatorSet.GetLineWidth(HalconWindow, out HTuple lineWidth);
                        HOperatorSet.SetLineWidth(HalconWindow, 1);
                        HOperatorSet.HomMat2dIdentity(out HTuple HomMat2dIdentity);
                        HOperatorSet.HomMat2dScale(HomMat2dIdentity, scaleX, scaleY, 0, 0, out HTuple HomMat2dScale);
                        foreach (var item in hObjects)
                        {
                            item?.Dispose();
                        }


                        if (operation_ImageInfo.showHalconWindow.m_regionShowInfos != null)
                            foreach (var item in operation_ImageInfo.showHalconWindow.m_regionShowInfos)
                            {
                                if ((item.m_hRegion?.CountObj() != null && item.m_hRegion?.CountObj() != 0))
                                {

                                    HOperatorSet.SetColor(HalconWindow, item.m_stuGetColor.ToString());
                                    HOperatorSet.SetDraw(HalconWindow, "margin");

                                    HOperatorSet.AffineTransRegion(item.m_hRegion, out HObject hObject, HomMat2dScale, "false");
                                    //HOperatorSet.DispObj(hObject, HalconWindow);
                                    hObjects.Add(hObject);

                                }

                            }


                        foreach (var item in hObjects)
                        {
                            if (item?.CountObj() != null && item?.CountObj() != 0)
                            {
                                HOperatorSet.DispObj(item, HalconWindow);
                            }
                        }

                        if (operation_ImageInfo.showHalconWindow.m_lineShowInfos != null)
                            foreach (var item in operation_ImageInfo.showHalconWindow.m_lineShowInfos)
                            {
                                try
                                {
                                    HOperatorSet.SetColor(HalconWindow, item.Color.ToString());
                                    //箭头显示

                                    HOperatorSet.AffineTransPoint2d(HomMat2dScale, item.ColBegin, item.RowBegin, out HTuple ColBegin, out HTuple RowBegin);
                                    HOperatorSet.AffineTransPoint2d(HomMat2dScale, item.ColEnd, item.RowEnd, out HTuple ColEnd, out HTuple RowEnd);

                                    //HOperatorSet.GenContourRegionXld(lineRegion, out lineRegion, "center");
                                    // HOperatorSet.GetLinesXld(lineRegion, out HTuple RowBegin, out HTuple ColBegin, out HTuple RowEnd, out HTuple ColEnd,out HTuple Length, out HTuple Angle);
                                    HOperatorSet.DispArrow(HalconWindow, RowBegin, ColBegin, RowEnd, ColEnd, m_nLineWidth);
                                    HOperatorSet.DispArrow(HalconWindow, RowEnd, ColEnd, RowBegin, ColBegin, m_nLineWidth);

                                    //HOperatorSet.DispArrow(HalconWindow, item.RowBegin, item.ColBegin , item.RowEnd , item.ColEnd , item.m_nLineWidth );
                                    //HOperatorSet.DispArrow(HalconWindow, item.RowEnd , item.ColEnd , item.RowBegin , item.ColBegin , item.m_nLineWidth );
                                }
                                catch (Exception)
                                {
                                    HOperatorSet.DispArrow(HalconWindow, item.RowBegin, item.ColBegin, item.RowEnd, item.ColEnd, item.m_nLineWidth);
                                    HOperatorSet.DispArrow(HalconWindow, item.RowEnd, item.ColEnd, item.RowBegin, item.ColBegin, item.m_nLineWidth);
                                    //throw;
                                }

                            }



                        HomMat2dIdentity?.Dispose();
                        HomMat2dScale?.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    Log4netRecord.ErrorLog.Error(source, ex);
                }


            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(source, ex);
            }



        }
        public void RePaint()
        {
            try
            {
                m_hWndCtrl_Display_Cam.resetAll();
                m_hWndCtrl_Display_Cam.repaint();

            }
            catch (Exception e)
            {
                Console.WriteLine(source.ToString() + "图像刷新方法::" + e.Message);
            }
        }

        public void set_display_font(HTuple hv_WindowHandle, HTuple hv_Size, HTuple hv_Font,
                            HTuple hv_Bold, HTuple hv_Slant)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_OS = new HTuple(), hv_Fonts = new HTuple();
            HTuple hv_Style = new HTuple(), hv_Exception = new HTuple();
            HTuple hv_AvailableFonts = new HTuple(), hv_Fdx = new HTuple();
            HTuple hv_Indices = new HTuple();
            HTuple hv_Font_COPY_INP_TMP = new HTuple(hv_Font);
            HTuple hv_Size_COPY_INP_TMP = new HTuple(hv_Size);

            // Initialize local and output iconic variables 
            try
            {
                //This procedure sets the text font of the current window with
                //the specified attributes.
                //
                //Input parameters:
                //WindowHandle: The graphics window for which the font will be set
                //Size: The font size. If Size=-1, the default of 16 is used.
                //Bold: If set to 'true', a bold font is used
                //Slant: If set to 'true', a slanted font is used
                //
                hv_OS.Dispose();
                HOperatorSet.GetSystem("operating_system", out hv_OS);
                if ((int)((new HTuple(hv_Size_COPY_INP_TMP.TupleEqual(new HTuple()))).TupleOr(
                    new HTuple(hv_Size_COPY_INP_TMP.TupleEqual(-1)))) != 0)
                {
                    hv_Size_COPY_INP_TMP.Dispose();
                    hv_Size_COPY_INP_TMP = 16;
                }
                if ((int)(new HTuple(((hv_OS.TupleSubstr(0, 2))).TupleEqual("Win"))) != 0)
                {
                    //Restore previous behaviour
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_Size = ((1.13677 * hv_Size_COPY_INP_TMP)).TupleInt()
                                ;
                            hv_Size_COPY_INP_TMP.Dispose();
                            hv_Size_COPY_INP_TMP = ExpTmpLocalVar_Size;
                        }
                    }
                }
                else
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_Size = hv_Size_COPY_INP_TMP.TupleInt()
                                ;
                            hv_Size_COPY_INP_TMP.Dispose();
                            hv_Size_COPY_INP_TMP = ExpTmpLocalVar_Size;
                        }
                    }
                }
                if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("Courier"))) != 0)
                {
                    hv_Fonts.Dispose();
                    hv_Fonts = new HTuple();
                    hv_Fonts[0] = "Courier";
                    hv_Fonts[1] = "Courier 10 Pitch";
                    hv_Fonts[2] = "Courier New";
                    hv_Fonts[3] = "CourierNew";
                    hv_Fonts[4] = "Liberation Mono";
                }
                else if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("mono"))) != 0)
                {
                    hv_Fonts.Dispose();
                    hv_Fonts = new HTuple();
                    hv_Fonts[0] = "Consolas";
                    hv_Fonts[1] = "Menlo";
                    hv_Fonts[2] = "Courier";
                    hv_Fonts[3] = "Courier 10 Pitch";
                    hv_Fonts[4] = "FreeMono";
                    hv_Fonts[5] = "Liberation Mono";
                }
                else if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("sans"))) != 0)
                {
                    hv_Fonts.Dispose();
                    hv_Fonts = new HTuple();
                    hv_Fonts[0] = "Luxi Sans";
                    hv_Fonts[1] = "DejaVu Sans";
                    hv_Fonts[2] = "FreeSans";
                    hv_Fonts[3] = "Arial";
                    hv_Fonts[4] = "Liberation Sans";
                }
                else if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("serif"))) != 0)
                {
                    hv_Fonts.Dispose();
                    hv_Fonts = new HTuple();
                    hv_Fonts[0] = "Times New Roman";
                    hv_Fonts[1] = "Luxi Serif";
                    hv_Fonts[2] = "DejaVu Serif";
                    hv_Fonts[3] = "FreeSerif";
                    hv_Fonts[4] = "Utopia";
                    hv_Fonts[5] = "Liberation Serif";
                }
                else
                {
                    hv_Fonts.Dispose();
                    hv_Fonts = new HTuple(hv_Font_COPY_INP_TMP);
                }
                hv_Style.Dispose();
                hv_Style = "";
                if ((int)(new HTuple(hv_Bold.TupleEqual("true"))) != 0)
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_Style = hv_Style + "Bold";
                            hv_Style.Dispose();
                            hv_Style = ExpTmpLocalVar_Style;
                        }
                    }
                }
                else if ((int)(new HTuple(hv_Bold.TupleNotEqual("false"))) != 0)
                {
                    hv_Exception.Dispose();
                    hv_Exception = "Wrong value of control parameter Bold";
                    throw new HalconException(hv_Exception);
                }
                if ((int)(new HTuple(hv_Slant.TupleEqual("true"))) != 0)
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_Style = hv_Style + "Italic";
                            hv_Style.Dispose();
                            hv_Style = ExpTmpLocalVar_Style;
                        }
                    }
                }
                else if ((int)(new HTuple(hv_Slant.TupleNotEqual("false"))) != 0)
                {
                    hv_Exception.Dispose();
                    hv_Exception = "Wrong value of control parameter Slant";
                    throw new HalconException(hv_Exception);
                }
                if ((int)(new HTuple(hv_Style.TupleEqual(""))) != 0)
                {
                    hv_Style.Dispose();
                    hv_Style = "Normal";
                }
                hv_AvailableFonts.Dispose();
                HOperatorSet.QueryFont(hv_WindowHandle, out hv_AvailableFonts);
                hv_Font_COPY_INP_TMP.Dispose();
                hv_Font_COPY_INP_TMP = "";
                for (hv_Fdx = 0; (int)hv_Fdx <= (int)((new HTuple(hv_Fonts.TupleLength())) - 1); hv_Fdx = (int)hv_Fdx + 1)
                {
                    hv_Indices.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Indices = hv_AvailableFonts.TupleFind(
                            hv_Fonts.TupleSelect(hv_Fdx));
                    }
                    if ((int)(new HTuple((new HTuple(hv_Indices.TupleLength())).TupleGreater(
                        0))) != 0)
                    {
                        if ((int)(new HTuple(((hv_Indices.TupleSelect(0))).TupleGreaterEqual(0))) != 0)
                        {
                            hv_Font_COPY_INP_TMP.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Font_COPY_INP_TMP = hv_Fonts.TupleSelect(
                                    hv_Fdx);
                            }
                            break;
                        }
                    }
                }
                if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual(""))) != 0)
                {
                    throw new HalconException("Wrong value of control parameter Font");
                }
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    {
                        HTuple
                          ExpTmpLocalVar_Font = (((hv_Font_COPY_INP_TMP + "-") + hv_Style) + "-") + hv_Size_COPY_INP_TMP;
                        hv_Font_COPY_INP_TMP.Dispose();
                        hv_Font_COPY_INP_TMP = ExpTmpLocalVar_Font;
                    }
                }
                HOperatorSet.SetFont(hv_WindowHandle, hv_Font_COPY_INP_TMP);

                hv_Font_COPY_INP_TMP.Dispose();
                hv_Size_COPY_INP_TMP.Dispose();
                hv_OS.Dispose();
                hv_Fonts.Dispose();
                hv_Style.Dispose();
                hv_Exception.Dispose();
                hv_AvailableFonts.Dispose();
                hv_Fdx.Dispose();
                hv_Indices.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_Font_COPY_INP_TMP.Dispose();
                hv_Size_COPY_INP_TMP.Dispose();
                hv_OS.Dispose();
                hv_Fonts.Dispose();
                hv_Style.Dispose();
                hv_Exception.Dispose();
                hv_AvailableFonts.Dispose();
                hv_Fdx.Dispose();
                hv_Indices.Dispose();

                throw HDevExpDefaultException;
            }
        }


        private void GeneratePerpendicularSegments(double x1, double y1, double x2, double y2, double length, out HObject lineRegion1, out HObject lineregion2)
        {

            // 计算直线的角度
            HTuple angle;
            HOperatorSet.AngleLl(y1, x1, y2, x2, y1, x1, y1 + 10, x1, out angle);

            // 计算垂直线的角度
            HTuple perpendicularAngle = angle + new HTuple(90).TupleRad();

            // 根据直线的两个端点计算垂直线的端点坐标
            HTuple rowStart1, colStart1, rowEnd1, colEnd1;
            HTuple rowStart2, colStart2, rowEnd2, colEnd2;

            // 第一个端点的垂直线
            rowStart1 = y1 + (length / 2) * perpendicularAngle.TupleSin();
            colStart1 = x1 - (length / 2) * perpendicularAngle.TupleCos();
            rowEnd1 = y1 - (length / 2) * perpendicularAngle.TupleSin();
            colEnd1 = x1 + (length / 2) * perpendicularAngle.TupleCos();

            // 第二个端点的垂直线
            rowStart2 = y2 + (length / 2) * perpendicularAngle.TupleSin();
            colStart2 = x2 - (length / 2) * perpendicularAngle.TupleCos();
            rowEnd2 = y2 - (length / 2) * perpendicularAngle.TupleSin();
            colEnd2 = x2 + (length / 2) * perpendicularAngle.TupleCos();


            // 在端点1处生成垂直线段

            HOperatorSet.GenContourPolygonXld(out HObject perpendicularLine1, new HTuple(rowStart1, rowEnd1), new HTuple(colStart1, colEnd1));
            // 在端点2处生成垂直线段
            lineRegion1 = perpendicularLine1;
            HOperatorSet.GenContourPolygonXld(out HObject perpendicularLine2, new HTuple(rowStart2, rowEnd2), new HTuple(colStart2, colEnd2));
            lineregion2 = perpendicularLine2;



        }

        private void HWC_HMouseMove(object sender, HalconDotNet.HMouseEventArgs e)
        {
            try
            {
                if (m_hWndCtrl_Display_Cam.ho_Image_Changed != null)
                {
                    string strName = m_hWndCtrl_Display_Cam.Get_Mouseposition_and_Gray(m_hWindowControl_Cam.HalconWindow, m_hWndCtrl_Display_Cam.ho_Image_Changed, e);
                    SetShowText(strName);
                }

            }
            catch (Exception)
            {


            }

        }
        public void SetShowText(string strName)
        {
            if (textBox == null) return;

            if (textBox.InvokeRequired)
            {
                textBox.Invoke(new MethodInvoker(() =>
                {
                    textBox.Text = strName;
                }));
            }
            else
            {
                textBox.Text = strName;
            }
        }

        private void btn_Click(object sender, EventArgs e)
        {
            try
            {
                // 重置显示画面
                RePaint();

            }
            catch (Exception)
            {
            }
        }

        public void Dispose()
        {
            operation_Image?.Dispose();
        }
    }
}

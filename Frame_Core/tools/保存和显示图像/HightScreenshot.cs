using HalconDotNet;
using LogTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    public class HightScreenshot
    {
        string source = "";
        private HTuple hv_dumpWindowHandle;//保存高清截图窗口
        private bool m_bIsWindowShow = true;
        private int m_nLineWidth = 2;

        public HightScreenshot(string _source)
        {
            source = _source;
            //Resize window
            HOperatorSet.SetWindowAttr("background_color", "black");
            HOperatorSet.OpenWindow(0, 0, 512, 512, 0, "buffer", "", out hv_dumpWindowHandle);
            HOperatorSet.SetWindowParam(hv_dumpWindowHandle, "flush", "true");
            //HOperatorSet.SetDraw(hv_dumpWindowHandle, "margin");
        }

        public ExecuteResult HightDefinition_Screenshot(ShowHalconWindow imageInfo,  out ShowHalconWindow screenshotImage)
        {
            // lock(obj)
            {
                ExecuteResult executeResult = new ExecuteResult();
                screenshotImage = null;
                ShowHalconWindow screenshotImageTemp = null;
                double scaleX = 1;
                double scaleY = 1.0;
                HObject m_hImage = null;
                HTuple HomMat2dScale = null;

                HOperatorSet.FlushBuffer(hv_dumpWindowHandle);
                HOperatorSet.ClearWindow(hv_dumpWindowHandle);

                try
                {
                    if (true)
                    {
                        HObject Copy_Win_Image = null;
                        try
                        {
                            //计时            
                            HOperatorSet.CountSeconds(out HTuple StartTime);
                            HTuple hv_Height = 0; HTuple hv_Width = 0;
                            HOperatorSet.HomMat2dIdentity(out HTuple HomMat2dIdentity);
                            if (imageInfo.m_nMode == 1)
                            {
                                HOperatorSet.GetImageSize(imageInfo.m_hImage, out hv_Width, out hv_Height);
                                if ((hv_Height / hv_Width) > 1.25)
                                {
                                    double a = (hv_Width / hv_Height);
                                    a = a < 0.25 ? 0.25 : a;
                                    a = a * 0.75;
                                    scaleX = a;
                                    scaleY = a;
                                }
                                HOperatorSet.HomMat2dScale(HomMat2dIdentity, scaleX, scaleY, 0, 0, out HomMat2dScale);

                                HOperatorSet.AffineTransImage(imageInfo.m_hImage, out m_hImage, HomMat2dScale, "nearest_neighbor", "true");
                                HOperatorSet.CountSeconds(out HTuple EndTime1);
                                double hTime2 = ((EndTime1 - StartTime) * 1000).D;//抓边算法耗时

                                MessageNotify.AddDebugLog("压缩产品高清截图耗时：" + hTime2.ToString());
                            }
                            else if (imageInfo.m_nMode == 0 && !imageInfo.bIs3D)
                            {
                                scaleX = 0.5;
                                scaleY = 0.5;
                                HOperatorSet.HomMat2dScale(HomMat2dIdentity, scaleX, scaleY, 0, 0, out HomMat2dScale);

                                HOperatorSet.AffineTransImage(imageInfo.m_hImage, out m_hImage, HomMat2dScale, "nearest_neighbor", "true");
                                HOperatorSet.CountSeconds(out HTuple EndTime1);
                            }


                            // HOperatorSet.ZoomImageFactor(ShowWindowInfo.m_hImage, out ShowWindowInfo.m_hImage, 0.1, 0.1, "constant");
                            if (!imageInfo.bIs3D)
                            {
                                HOperatorSet.GetImageSize(m_hImage, out hv_Width, out hv_Height);


                            }
                            else
                            {
                                HOperatorSet.GetImageSize(imageInfo.Image3DClass.Image2D, out hv_Width, out hv_Height);
                            }
                            HomMat2dIdentity?.Dispose();
                            HomMat2dScale?.Dispose();

                            HOperatorSet.SetWindowExtents(hv_dumpWindowHandle, 0, 0, hv_Width, hv_Height);
                            HOperatorSet.SetPart(hv_dumpWindowHandle, 0, 0, (hv_Height - 1), hv_Width - 1);
                            HOperatorSet.CountSeconds(out HTuple EndTime);
                            double hTime1 = ((EndTime - StartTime) * 1000).D;//抓边算法耗时

                            MessageNotify.AddDebugLog("开高清窗口耗时：" + hTime1.ToString());
                            //Console.WriteLine("开窗口耗时：" + hTime1.ToString());
                            if (!imageInfo.bIs3D)
                            {
                                HOperatorSet.DispObj(m_hImage, hv_dumpWindowHandle);

                            }
                            else
                            {
                                HOperatorSet.DispObj(imageInfo.Image3DClass.Image2D, hv_dumpWindowHandle);
                            }
                            if (imageInfo.m_nMode == 1)
                            {
                                ShowRoiResult(hv_dumpWindowHandle, imageInfo, scaleX, scaleY);
                            }
                            else if (imageInfo.m_nMode == 0 && !imageInfo.bIs3D)
                            {
                                ShowRoiResult(hv_dumpWindowHandle, imageInfo, scaleX, scaleY);

                            }
                            else
                            {
                                ShowRoiResult(hv_dumpWindowHandle, imageInfo);
                            }

                            HOperatorSet.DumpWindowImage(out Copy_Win_Image, hv_dumpWindowHandle);
                            HOperatorSet.CountSeconds(out EndTime);
                            hTime1 = ((EndTime - StartTime) * 1000).D;//抓边算法耗时
                            MessageNotify.AddDebugLog("开高清窗口总耗时：" + hTime1.ToString());
                            screenshotImageTemp = new ShowHalconWindow();
                            screenshotImageTemp.m_strCamName = imageInfo.m_strCamName;
                            screenshotImageTemp.m_nMode = imageInfo.m_nMode;
                            screenshotImageTemp.bIs3D = imageInfo.bIs3D;
                            screenshotImageTemp.m_nProductID = imageInfo.m_nProductID;
                            screenshotImageTemp.m_nImageFrameID = imageInfo.m_nImageFrameID;
                            screenshotImageTemp.m_hImage = Copy_Win_Image;
                            screenshotImageTemp.m_imageSavePathInfo = imageInfo.m_imageSavePathInfo;
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
        //                if (!ShowWindowInfo.bIs3D)
        //                {
        //                    HOperatorSet.GetImageSize(ShowWindowInfo.m_hImage, out hv_Height, out hv_Width);

        //                }
        //                else
        //                {
        //                    HOperatorSet.GetImageSize(ShowWindowInfo.Image3DClass.Image2D, out hv_Height, out hv_Width);
        //                }

        //                HOperatorSet.SetWindowExtents(hv_dumpWindowHandle, 0, 0, hv_Height, hv_Width);
        //                HOperatorSet.SetPart(hv_dumpWindowHandle, 0, 0, hv_Width - 1, hv_Height - 1);
        //                HOperatorSet.CountSeconds(out HTuple EndTime);
        //                double hTime1 = ((EndTime - StartTime) * 1000).D;//抓边算法耗时

        //                MessageNotify.AddDebugLog("开高清窗口耗时：" + hTime1.ToString());
        //                //Console.WriteLine("开窗口耗时：" + hTime1.ToString());
        //                if (!ShowWindowInfo.bIs3D)
        //                {
        //                    HOperatorSet.DispObj(ShowWindowInfo.m_hImage, hv_dumpWindowHandle);
        //                }
        //                else
        //                {
        //                    HOperatorSet.DispObj(ShowWindowInfo.Image3DClass.Image2D, hv_dumpWindowHandle);
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

        private void ShowRoiResult(HTuple HalconWindow, ShowHalconWindow imageInfo)
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

                        if (imageInfo.m_messageShowInfos != null)
                            foreach (var item in imageInfo.m_messageShowInfos)
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
                        if (imageInfo.m_regionShowInfos != null)
                            foreach (var item in imageInfo.m_regionShowInfos)
                            {
                                if ((item.m_hRegion?.CountObj() != null && item.m_hRegion?.CountObj() != 0))
                                {
                                    HOperatorSet.SetColor(HalconWindow, item.m_stuGetColor.ToString());
                                    HOperatorSet.SetDraw(HalconWindow, "margin");
                                    HOperatorSet.DispObj(item.m_hRegion, HalconWindow);
                                }

                            }

                        if (imageInfo.m_lineShowInfos != null)
                            foreach (var item in imageInfo.m_lineShowInfos)
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
        private void ShowRoiResult(HTuple HalconWindow, ShowHalconWindow imageInfo, double scaleX = 1, double scaleY = 1)
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

                        if (imageInfo.m_messageShowInfos != null)
                            foreach (var item in imageInfo.m_messageShowInfos)
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


                        if (imageInfo.m_regionShowInfos != null)
                            foreach (var item in imageInfo.m_regionShowInfos)
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

                        if (imageInfo.m_lineShowInfos != null)
                            foreach (var item in imageInfo.m_lineShowInfos)
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
    }
}

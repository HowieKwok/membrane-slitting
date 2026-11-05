using Frame_Core;
using FrameCommunity.Data;
using HalconDotNet;
using LogTool;
using NPOI.XSSF.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ViewROI2;

namespace Frame_Core
{
    public class NewDisplayWindowClass : IDisposable
    {
        /// <summary>
        /// 检测结果图像显示窗体对象
        /// </summary>
        private ROIController m_roiController_Process_Cam = new ROIController();
        private ROIRectangle2 m_roiRectangle2_Process_Cam = new ROIRectangle2();


        /// <summary>
        /// 检测结果图像显示窗体对象
        /// </summary>
        public HWndCtrl m_hWndCtrl_Display_Cam;
        public ROIController m_roiController_Cam = new ROIController();
        public ROIRectangle2 m_roiRectangle2_Cam = new ROIRectangle2();
        //private ShowHalconWindow m_ShowHalconWindow = new ShowHalconWindow();
        public HWindowControl m_hWindowControl_Cam;

        public HightScreenshot hightScreenshot;

        private bool m_bIsWindowShow = true;
        private int m_nLineWidth = 2;


        public ShowHalconWindow ShowWindowInfo = new ShowHalconWindow();

        public string source;
        TextBox textBox = null;
        PictureBox pictureBox = null;

        public NewDisplayWindowClass(HWindowControl hWindowControl)
        {

            source = hWindowControl.Name;
            if (hWindowControl != null)
            {
                m_hWindowControl_Cam = hWindowControl;
                m_hWndCtrl_Display_Cam = new HWndCtrl(m_hWindowControl_Cam);
                m_roiController_Cam = new ROIController();
                m_hWndCtrl_Display_Cam.setViewState(HWndCtrl.MODE_VIEW_ZOOM);
                m_hWndCtrl_Display_Cam.setViewState(HWndCtrl.MODE_VIEW_MOVE);
                m_hWndCtrl_Display_Cam.ReDrawEvent += new HWndCtrl.ReDrawDelegate(ShowRoiResult);     //更改测量矩形，重绘测量结果

            }

        }

        public NewDisplayWindowClass(UC_HalconWindow hWindowControl)
        {
            textBox = hWindowControl.TB_GrayInfo;
            pictureBox = hWindowControl.PB_FitWinOutter;
            pictureBox.Click -= btn_Click;
            pictureBox.Click += btn_Click;

            source = hWindowControl.Name;
            if (hWindowControl != null)
            {
                m_hWindowControl_Cam = hWindowControl.hWindow;
                m_hWndCtrl_Display_Cam = new HWndCtrl(m_hWindowControl_Cam);
                m_roiController_Cam = new ROIController();
                m_hWndCtrl_Display_Cam.setViewState(HWndCtrl.MODE_VIEW_ZOOM);
                m_hWndCtrl_Display_Cam.setViewState(HWndCtrl.MODE_VIEW_MOVE);
                m_hWndCtrl_Display_Cam.ReDrawEvent += new HWndCtrl.ReDrawDelegate(ShowRoiResult);     //更改测量矩形，重绘测量结果
                m_hWndCtrl_Display_Cam.viewPort.HMouseMove += HWC_HMouseMove;


                hightScreenshot = new HightScreenshot(source);
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
        public ExecuteResult RepaintCam(ShowHalconWindow imageInfo)
        {
            ExecuteResult executeResult = new ExecuteResult();

            try
            {
                m_hWindowControl_Cam.Invoke(new Action(() =>
                {
                    try
                    {
                        ShowWindowInfo = imageInfo;
                        if (ShowWindowInfo.m_hImage != null)
                        {
                            m_hWndCtrl_Display_Cam.addIconicVar(ShowWindowInfo.m_hImage);
                        }

                        //显示图片区域文本信息
                        RePaint();
                    }
                    catch (Exception ex)
                    {
                        Log4netRecord.ErrorLog.Error(source, ex);
                    }

                }));

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

        public ExecuteResult ScreenshotImage(out ShowHalconWindow screenshotImage)
        {
            ExecuteResult executeResult = new ExecuteResult();
            screenshotImage = null;
            ShowHalconWindow screenshotImageTemp = null;

            try
            {
                m_hWindowControl_Cam.Invoke(new Action(() =>
                {
                    try
                    {

                        HObject Copy_Win_Image = null;
                        HOperatorSet.DumpWindowImage(out Copy_Win_Image, m_hWindowControl_Cam.HalconWindow);
                        screenshotImageTemp = new ShowHalconWindow();
                        screenshotImageTemp.m_hImage = Copy_Win_Image;
                        screenshotImageTemp.m_nImageFrameID = ShowWindowInfo.m_nImageFrameID;
                        screenshotImageTemp.m_nProductID = ShowWindowInfo.m_nProductID;
                        screenshotImageTemp.m_imageSavePathInfo = ShowWindowInfo.m_imageSavePathInfo;
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

        public object obj = new object();

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
                            if (ShowWindowInfo.m_messageShowInfos != null)
                                foreach (var item in ShowWindowInfo.m_messageShowInfos)
                                {
                                    if (item.m_bIsWindow)
                                    {
                                        if (FontLine > height - item.m_nFontSize/*count > (height / 20)-2*/)
                                        {
                                            count = 0;
                                            colPos = width - 200;
                                            FontLine = 0;
                                        }



                                        HOperatorSet.SetColor(HWC.HalconWindow, item.m_stuGetColor.ToString());
                                        set_display_font(HWC.HalconWindow, item.m_nFontSize, "mono", "true", "false");
                                        HOperatorSet.DispText(HWC.HalconWindow, item.m_strMessage, "window", FontLine, colPos, item.m_stuGetColor.ToString(), "box", "false");

                                        int count1 = item.m_strMessage.Count(c => c == '\n');
                                        FontLine += item.m_nFontSize + 10 + (count1 * (item.m_nFontSize + 5));
                                        //count++;
                                    }
                                    else
                                    {
                                        HOperatorSet.SetColor(HWC.HalconWindow, item.m_stuGetColor.ToString());
                                        set_display_font(HWC.HalconWindow, item.m_nFontSize, "mono", "true", "false");
                                        HOperatorSet.DispText(HWC.HalconWindow, item.m_strMessage, "image", item.m_fRow, item.m_fCol, item.m_stuGetColor.ToString(), "box", "false");
                                    }

                                }

                            if (ShowWindowInfo.m_regionShowInfos != null && ShowWindowInfo.m_nMode < 3)
                                foreach (var item in ShowWindowInfo.m_regionShowInfos)
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
                                    catch (Exception)
                                    {
                                        int index = ShowWindowInfo.m_regionShowInfos.IndexOf(item);
                                    }

                                }

                            if (ShowWindowInfo.m_lineShowInfos != null)
                                foreach (var item in ShowWindowInfo.m_lineShowInfos)
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
            ShowWindowInfo?.Dispose();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;

namespace HalconAlgorithm
{
    class PaintWindows
    {
        /// <summary>
        /// XLD绘制：生成圆角矩形
        /// </summary>
        /// <param name="hv_Row">中心点行坐标</param>
        /// <param name="hv_Column">中心点列坐标</param>
        /// <param name="hv_Angle">旋转角度</param>
        /// <param name="hv_Length1">半长</param>
        /// <param name="hv_Length2">半宽</param>
        /// <param name="hv_Radius">倒角半径</param>
        /// <param name="ho_Contours">输出圆角矩形XLD轮廓</param>
        public static void PaintRoundRectangle(HTuple hv_Row, HTuple hv_Column, HTuple hv_Angle, HTuple hv_Length1, HTuple hv_Length2, HTuple hv_Radius, out HObject ho_Contours)
        {
            HObject ho_Rectangle;
            HObject ho_RegionOpening;
            HOperatorSet.GenRectangle2(out  ho_Rectangle, hv_Row, hv_Column, hv_Angle, hv_Length1, hv_Length2);
            HOperatorSet.OpeningCircle(ho_Rectangle, out  ho_RegionOpening, hv_Radius);
            HOperatorSet.GenContourRegionXld(ho_RegionOpening, out ho_Contours, "border");

            ho_Rectangle.Dispose();
            ho_RegionOpening.Dispose();
        }


        /// <summary>
        /// 图像显示：适应窗口，等比显示图像
        /// </summary>
        /// <param name="ho_Image">输入图像</param>
        /// <param name="hWindowControl">输入Halcon窗口</param>
        /// <returns>显示结果</returns>
        public static bool DisplayImage(HObject ho_Image, HWindowControl hWindowControl)
        {
            HTuple hv_StartX, hv_StartY, hv_ZoomFactor;
            HOperatorSet.ClearWindow(hWindowControl.HalconWindow);
            //获取图像大小及纵横比
            HTuple hv_Width = new HTuple(), hv_Height = new HTuple();
            HOperatorSet.GetImageSize(ho_Image, out hv_Width, out hv_Height);
            int im_width = int.Parse(hv_Width.ToString());
            int im_height = int.Parse(hv_Height.ToString());
            double im_AspectRatio = (double)(im_width) / (double)(im_height);
            double halconWindowHeight = 0;
            double halconWindowWidth = 0;

            halconWindowHeight = hWindowControl.WindowSize.Height;
            halconWindowWidth = hWindowControl.WindowSize.Width;

            double w_AspectRatio = halconWindowWidth / halconWindowHeight;
            HOperatorSet.SetSystem("int_zooming", "false");//图像缩放之前最好将此参数设置为false.

            HTuple para = new HTuple("constant");
            HObject ho_zoomImage;
            HOperatorSet.GenEmptyObj(out ho_zoomImage);

            ho_zoomImage.Dispose();
            HOperatorSet.ClearWindow(hWindowControl.HalconWindow);

            if (halconWindowWidth < im_width && im_AspectRatio > w_AspectRatio)
            {
                //超宽图像               
                HOperatorSet.ZoomImageSize(ho_Image, out ho_zoomImage, halconWindowWidth, halconWindowWidth / im_AspectRatio, para);
                hv_ZoomFactor = halconWindowWidth / im_width;
                hv_StartX = 0 - (halconWindowHeight - halconWindowWidth / im_AspectRatio) / 2;
                hv_StartY = 0;
            }
            else
            {
                //超高图像                
                HOperatorSet.ZoomImageSize(ho_Image, out ho_zoomImage, halconWindowHeight * im_AspectRatio, halconWindowHeight, para);
                hv_ZoomFactor = halconWindowHeight / im_height;
                hv_StartX = 0;
                hv_StartY = 0 - (halconWindowWidth - halconWindowHeight * im_AspectRatio) / 2;
            }

            try
            {
                HOperatorSet.SetPart(hWindowControl.HalconWindow, hv_StartX, hv_StartY, halconWindowHeight - 1 + hv_StartX,
                    halconWindowWidth - 1 + hv_StartY);//设置居中显示
                HOperatorSet.DispObj(ho_zoomImage, hWindowControl.HalconWindow);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// 轮廓显示：适应窗口，等比显示轮廓
        /// </summary>
        /// <param name="ho_Contours">输入XLD轮廓</param>
        /// <param name="ho_Image">输入图像</param>
        /// <param name="hWindowControl">输入Halcon窗口</param>
        /// <returns>显示结果</returns>
        public static bool DisplayXLD(HObject ho_Contours  , HObject ho_Image, HWindowControl hWindowControl)
        {
            HTuple hv_StartX, hv_StartY, hv_ZoomFactor;
            //获取图像大小及纵横比
            HTuple hv_Width = new HTuple(), hv_Height = new HTuple();
            HOperatorSet.GetImageSize(ho_Image, out hv_Width, out hv_Height);
            int im_width = int.Parse(hv_Width.ToString());
            int im_height = int.Parse(hv_Height.ToString());
            double im_AspectRatio = (double)(im_width) / (double)(im_height);
            double halconWindowHeight = 0;
            double halconWindowWidth = 0;

            halconWindowHeight = hWindowControl.WindowSize.Height;
            halconWindowWidth = hWindowControl.WindowSize.Width;

            double w_AspectRatio = halconWindowWidth / halconWindowHeight;
            HOperatorSet.SetSystem("int_zooming", "false");//图像缩放之前最好将此参数设置为false.
            HTuple para = new HTuple("constant");
            HTuple hv_HomMat2D1;
            HOperatorSet.HomMat2dIdentity(out  hv_HomMat2D1);


            if (halconWindowWidth < im_width && im_AspectRatio > w_AspectRatio)
            {
                //超宽图像               
                hv_ZoomFactor = halconWindowWidth / im_width;
                hv_StartX = 0 - (halconWindowHeight - halconWindowWidth / im_AspectRatio) / 2;
                hv_StartY = 0;

                HOperatorSet.HomMat2dScale(hv_HomMat2D1, hv_ZoomFactor, hv_ZoomFactor, 0, 0, out hv_HomMat2D1);
            }
            else
            {
                //超高图像                
                hv_ZoomFactor = halconWindowHeight / im_height;
                hv_StartX = 0;
                hv_StartY = 0 - (halconWindowWidth - halconWindowHeight * im_AspectRatio) / 2;
                HOperatorSet.HomMat2dScale(hv_HomMat2D1, hv_ZoomFactor, hv_ZoomFactor, 0, 0, out hv_HomMat2D1);
            }

            try
            {
                HObject ho_ModelContours;
                HOperatorSet.AffineTransContourXld(ho_Contours, out  ho_ModelContours, hv_HomMat2D1);
                HOperatorSet.SetPart(hWindowControl.HalconID, hv_StartX, hv_StartY, halconWindowHeight - 1 + hv_StartX,
                    halconWindowWidth - 1 + hv_StartY);//设置居中显示
                HOperatorSet.DispObj(ho_ModelContours, hWindowControl.HalconID);

                ho_ModelContours.Dispose();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// 轮廓显示：适应窗口，等比显示区域
        /// </summary>
        /// <param name="ho_Region">输入区域</param>
        /// <param name="ho_Image">输入图像</param>
        /// <param name="hWindowControl">输入Halcon窗口</param>
        /// <returns>显示结果</returns>
        public static bool DisplayRegion(HObject ho_Region, HObject ho_Image, HWindowControl hWindowControl)
        {
            HTuple hv_StartX, hv_StartY, hv_ZoomFactor;
            //获取图像大小及纵横比
            HTuple hv_Width = new HTuple(), hv_Height = new HTuple();
            HOperatorSet.GetImageSize(ho_Image, out hv_Width, out hv_Height);
            int im_width = int.Parse(hv_Width.ToString());
            int im_height = int.Parse(hv_Height.ToString());
            double im_AspectRatio = (double)(im_width) / (double)(im_height);
            double halconWindowHeight = 0;
            double halconWindowWidth = 0;

            halconWindowHeight = hWindowControl.WindowSize.Height;
            halconWindowWidth = hWindowControl.WindowSize.Width;

            double w_AspectRatio = halconWindowWidth / halconWindowHeight;
            HOperatorSet.SetSystem("int_zooming", "false");//图像缩放之前最好将此参数设置为false.
            HTuple para = new HTuple("constant");
            HTuple hv_HomMat2D1;
            HOperatorSet.HomMat2dIdentity(out hv_HomMat2D1);


            if (halconWindowWidth < im_width && im_AspectRatio > w_AspectRatio)
            {
                //超宽图像               
                hv_ZoomFactor = halconWindowWidth / im_width;
                hv_StartX = 0 - (halconWindowHeight - halconWindowWidth / im_AspectRatio) / 2;
                hv_StartY = 0;

                HOperatorSet.HomMat2dScale(hv_HomMat2D1, hv_ZoomFactor, hv_ZoomFactor, 0, 0, out hv_HomMat2D1);
            }
            else
            {
                //超高图像                
                hv_ZoomFactor = halconWindowHeight / im_height;
                hv_StartX = 0;
                hv_StartY = 0 - (halconWindowWidth - halconWindowHeight * im_AspectRatio) / 2;
                HOperatorSet.HomMat2dScale(hv_HomMat2D1, hv_ZoomFactor, hv_ZoomFactor, 0, 0, out hv_HomMat2D1);
            }

            try
            {
                HObject ho_Modelho_Region;
                HOperatorSet.AffineTransRegion(ho_Region, out  ho_Modelho_Region, hv_HomMat2D1, "nearest_neighbor");
                HOperatorSet.SetPart(hWindowControl.HalconID, hv_StartX, hv_StartY, halconWindowHeight - 1 + hv_StartX,
                    halconWindowWidth - 1 + hv_StartY);//设置居中显示
                HOperatorSet.DispObj(ho_Modelho_Region, hWindowControl.HalconID);

                ho_Modelho_Region.Dispose();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// 文字绘制：在HWindow中写文字
        /// </summary>
        /// <param name="hv_ImageWindow">HWindow窗体ID</param>
        /// <param name="WriteString">绘制文字</param>
        /// <param name="FontSize">字体大小</param>
        /// <param name="FontColor">字体颜色</param>
        /// <param name="Row">起始点行坐标</param>
        /// <param name="Column">起始点列坐标</param>
        public static void WriteStringInHwindown(HTuple hv_ImageWindow, HTuple WriteString, HTuple FontSize, HTuple FontColor, HTuple Row, HTuple Column)
        {
            set_display_font(hv_ImageWindow, FontSize, "mono", "true", "false");
            HOperatorSet.SetColor(hv_ImageWindow, FontColor);
            HOperatorSet.SetTposition(hv_ImageWindow, Row, Column);
            HOperatorSet.WriteString(hv_ImageWindow, WriteString);
        }


        /// <summary>
        /// 文字绘制：设置字体
        /// </summary>
        /// <param name="hv_WindowHandle"></param>
        /// <param name="hv_Size"></param>
        /// <param name="hv_Font"></param>
        /// <param name="hv_Bold"></param>
        /// <param name="hv_Slant"></param>
        public static void set_display_font(HTuple hv_WindowHandle, HTuple hv_Size, HTuple hv_Font, HTuple hv_Bold, HTuple hv_Slant)
        {
            // Local iconic variables 

            // Local control variables 

            HTuple hv_OS = null, hv_Fonts = new HTuple();
            HTuple hv_Style = null, hv_Exception = new HTuple(), hv_AvailableFonts = null;
            HTuple hv_Fdx = null, hv_Indices = new HTuple();
            HTuple hv_Font_COPY_INP_TMP = hv_Font.Clone();
            HTuple hv_Size_COPY_INP_TMP = hv_Size.Clone();

            // Initialize local and output iconic variables 
            //This procedure sets the text font of the current window with
            //the specified attributes.
            //
            //Input parameters:
            //WindowHandle: The graphics window for which the font will be set
            //Size: The font size. If Size=-1, the default of 16 is used.
            //Bold: If set to 'true', a bold font is used
            //Slant: If set to 'true', a slanted font is used
            //
            HOperatorSet.GetSystem("operating_system", out hv_OS);
            // dev_get_preferences(...); only in hdevelop
            // dev_set_preferences(...); only in hdevelop
            if ((int)((new HTuple(hv_Size_COPY_INP_TMP.TupleEqual(new HTuple()))).TupleOr(
                new HTuple(hv_Size_COPY_INP_TMP.TupleEqual(-1)))) != 0)
            {
                hv_Size_COPY_INP_TMP = 16;
            }
            if ((int)(new HTuple(((hv_OS.TupleSubstr(0, 2))).TupleEqual("Win"))) != 0)
            {
                //Restore previous behaviour
                hv_Size_COPY_INP_TMP = ((1.13677 * hv_Size_COPY_INP_TMP)).TupleInt();
            }
            else
            {
                hv_Size_COPY_INP_TMP = hv_Size_COPY_INP_TMP.TupleInt();
            }
            if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("Courier"))) != 0)
            {
                hv_Fonts = new HTuple();
                hv_Fonts[0] = "Courier";
                hv_Fonts[1] = "Courier 10 Pitch";
                hv_Fonts[2] = "Courier New";
                hv_Fonts[3] = "CourierNew";
                hv_Fonts[4] = "Liberation Mono";
            }
            else if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("mono"))) != 0)
            {
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
                hv_Fonts = new HTuple();
                hv_Fonts[0] = "Luxi Sans";
                hv_Fonts[1] = "DejaVu Sans";
                hv_Fonts[2] = "FreeSans";
                hv_Fonts[3] = "Arial";
                hv_Fonts[4] = "Liberation Sans";
            }
            else if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("serif"))) != 0)
            {
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
                hv_Fonts = hv_Font_COPY_INP_TMP.Clone();
            }
            hv_Style = "";
            if ((int)(new HTuple(hv_Bold.TupleEqual("true"))) != 0)
            {
                hv_Style = hv_Style + "Bold";
            }
            else if ((int)(new HTuple(hv_Bold.TupleNotEqual("false"))) != 0)
            {
                hv_Exception = "Wrong value of control parameter Bold";
                throw new HalconException(hv_Exception);
            }
            if ((int)(new HTuple(hv_Slant.TupleEqual("true"))) != 0)
            {
                hv_Style = hv_Style + "Italic";
            }
            else if ((int)(new HTuple(hv_Slant.TupleNotEqual("false"))) != 0)
            {
                hv_Exception = "Wrong value of control parameter Slant";
                throw new HalconException(hv_Exception);
            }
            if ((int)(new HTuple(hv_Style.TupleEqual(""))) != 0)
            {
                hv_Style = "Normal";
            }
            HOperatorSet.QueryFont(hv_WindowHandle, out hv_AvailableFonts);
            hv_Font_COPY_INP_TMP = "";
            for (hv_Fdx = 0; (int)hv_Fdx <= (int)((new HTuple(hv_Fonts.TupleLength())) - 1); hv_Fdx = (int)hv_Fdx + 1)
            {
                hv_Indices = hv_AvailableFonts.TupleFind(hv_Fonts.TupleSelect(hv_Fdx));
                if ((int)(new HTuple((new HTuple(hv_Indices.TupleLength())).TupleGreater(0))) != 0)
                {
                    if ((int)(new HTuple(((hv_Indices.TupleSelect(0))).TupleGreaterEqual(0))) != 0)
                    {
                        hv_Font_COPY_INP_TMP = hv_Fonts.TupleSelect(hv_Fdx);
                        break;
                    }
                }
            }
            if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual(""))) != 0)
            {
                throw new HalconException("Wrong value of control parameter Font");
            }
            hv_Font_COPY_INP_TMP = (((hv_Font_COPY_INP_TMP + "-") + hv_Style) + "-") + hv_Size_COPY_INP_TMP;
            HOperatorSet.SetFont(hv_WindowHandle, hv_Font_COPY_INP_TMP);
            // dev_set_preferences(...); only in hdevelop

            return;
        }

    }

}

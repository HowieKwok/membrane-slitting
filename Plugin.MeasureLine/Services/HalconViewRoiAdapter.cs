using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using Frame_Core;
using HalconDotNet;
using Plugin.MeasureLine.Core;
using ViewROI;

namespace Plugin.MeasureLine.Services
{
    public sealed class HalconViewRoiAdapter : IDisposable
    {
        private readonly HWindowControl window;
        private readonly HWndCtrl viewCtrl;
        private readonly ROIController roiController;
        private readonly HWndCtrl.ReDrawDelegate redrawHandler;
        private readonly IconicDelegate roiChangedHandler;
        private readonly HMouseEventHandler mouseMoveHandler;
        private readonly HMouseEventHandler mouseDownHandler;
        private readonly HMouseEventHandler mouseUpHandler;

        private HObject image;
        private HObject displayImage;
        private ParamsMeasureLine overlayParams;
        private bool crosshairEnabled;
        private double crossRow = -1;
        private double crossCol = -1;
        private bool disposed;

        public HalconViewRoiAdapter(HWindowControl windowControl, bool enableRoi)
        {
            window = windowControl ?? throw new ArgumentNullException(nameof(windowControl));
            roiController = new ROIController();
            viewCtrl = new HWndCtrl(window);
            viewCtrl.useROIController(roiController);
            viewCtrl.setDispLevel(HWndCtrl.MODE_INCLUDE_ROI);
            viewCtrl.setViewState(HWndCtrl.MODE_VIEW_MOVE);
            viewCtrl.bl_RoiOperateFlag = enableRoi;
            viewCtrl.ho_Image?.Dispose();
            viewCtrl.ho_Image_Changed?.Dispose();
            viewCtrl.ho_Image = null;
            viewCtrl.ho_Image_Changed = null;
            roiController.setDrawColor("yellow", "red", "gray");

            redrawHandler = OnViewRoiRedraw;
            roiChangedHandler = OnViewRoiChanged;
            mouseMoveHandler = OnMouseMove;
            mouseDownHandler = OnMouseDown;
            mouseUpHandler = OnMouseUp;

            viewCtrl.ReDrawEvent += redrawHandler;
            roiController.NotifyRCObserver += roiChangedHandler;
            window.HMouseMove += mouseMoveHandler;
            window.HMouseDown += mouseDownHandler;
            window.HMouseUp += mouseUpHandler;
        }

        public event Action<int> RoiChanged;
        public event Action<HalconMouseInfoArgs> MouseMoved;
        public event Action<HalconMouseInfoArgs> RightMouseDown;
        public event Action<HalconMouseInfoArgs> RightMouseUp;
        public event Action<double[], double[]> GrayProfileChanged;

        public ROIController RoiController => roiController;
        public HWindow HalconWindow => window.HalconWindow;

        public void SetOverlayState(ParamsMeasureLine parameters, bool showCrosshair, double row, double col)
        {
            overlayParams = parameters;
            crosshairEnabled = showCrosshair;
            crossRow = row;
            crossCol = col;
        }

        public void SetRoiOperate(bool enabled)
        {
            if (disposed) return;
            viewCtrl.bl_RoiOperateFlag = enabled;
            Repaint();
        }

        public void ShowImage(HObject hoImage, bool fit)
        {
            if (disposed) return;
            image = hoImage;
            UpdateRoiHandleSize();

            if (fit)
            {
                ResetAndFitImage();
                return;
            }

            RefreshDisplayImage();
            if (displayImage == null || !displayImage.IsInitialized()) return;

            viewCtrl.addIconicVar(displayImage);
            viewCtrl.resetflag = false;
            viewCtrl.repaint();
        }

        public void FitImage()
        {
            if (disposed) return;
            if (HasValidImage(image))
            {
                ResetAndFitImage();
                return;
            }

            viewCtrl.resetAll();
            viewCtrl.repaint();
        }

        public void RefreshFromParams(ParamsMeasureLine parameters, bool fit)
        {
            if (disposed) return;
            overlayParams = parameters;
            if (parameters != null)
            {
                EnsureRectangleRoi(parameters.hv_RoiMidColumn, parameters.hv_RoiMidRow, ToRad(parameters.hv_RoiAngle), parameters.hv_RoiHeight / 2.0, parameters.hv_RoiWidth / 2.0, parameters.hv_RoiPointsNum);
                image = parameters.ho_Image;
                UpdateRoiHandleSize();
                if (HasValidImage(image))
                {
                    ShowImage(image, fit);
                    return;
                }
            }

            viewCtrl.resetAll();
            viewCtrl.repaint();
        }

        public void SyncActiveRoi(double row, double column, double angleDeg, double height, double width, int pointCount)
        {
            if (disposed) return;

            ROIRectangle2 roi = GetActiveRectangleRoi();
            if (roi == null)
            {
                if(roiController.ROIList?.Count <= 0)
                {
                    EnsureRectangleRoi(column, row, ToRad(angleDeg), height / 2.0, width / 2.0, pointCount);
                    return;
                }
                else
                {
                    roi = (ROIRectangle2)roiController.ROIList[0];
                }
              
            }

            //roiController.setROIShape(roi);
            roi.midR = row;
            roi.midC = column;
            roi.phi = ToRad(angleDeg);
            roi.length1 = height / 2.0;
            roi.length2 = width / 2.0;
            roi.PointNum = pointCount;
            //UpdateRoiHandleSize();
            Repaint();
        }

        public ROIRectangle2 GetActiveRectangleRoi()
        {
            return roiController.getActiveROI() as ROIRectangle2;
        }

        public void Repaint()
        {
            if (disposed) return;
            viewCtrl.repaint();
        }

        public string GetMouseText(HMouseEventArgs e)
        {
            if (!HasValidImage(image)) return "图像:-  行:--  列:--  灰度:-";

            try
            {
                int row = (int)Math.Round(e.Y);
                int col = (int)Math.Round(e.X);
                HOperatorSet.GetImageSize(image, out HTuple width, out HTuple height);
                if (row < 0 || col < 0 || row >= height.I || col >= width.I)
                    return "图像:-  行:--  列:--  灰度:-";

                HObject readImage = image;
                HObject channel = null;
                try
                {
                    HOperatorSet.CountChannels(image, out HTuple channels);
                    if (channels.I > 1)
                    {
                        HOperatorSet.AccessChannel(image, out channel, 1);
                        readImage = channel;
                    }

                    HOperatorSet.GetGrayval(readImage, row, col, out HTuple gray);
                    return string.Format(CultureInfo.InvariantCulture, "图像:-  行:{0}  列:{1}  灰度:{2}", row, col, gray.ToString());
                }
                finally
                {
                    channel?.Dispose();
                }
            }
            catch
            {
                return "图像:-  行:--  列:--  灰度:-";
            }
        }

        public void Dispose()
        {
            if (disposed) return;
            disposed = true;

            try
            {
                //viewCtrl.ho_Image?.Dispose();
                //viewCtrl.ho_Image_Changed?.Dispose();
                viewCtrl.ho_Image = null;
                viewCtrl.ho_Image_Changed = null;
                viewCtrl.ReDrawEvent -= redrawHandler;
                roiController.NotifyRCObserver -= roiChangedHandler;
                window.HMouseMove -= mouseMoveHandler;
                window.HMouseDown -= mouseDownHandler;
                window.HMouseUp -= mouseUpHandler;
                window?.Dispose();
            }
            catch
            {
            }

            try
            {
                roiController.reset();
            }
            catch
            {
            }

            displayImage?.Dispose();
            
            displayImage = null;
            image = null;
            overlayParams = null;
        }

        private void EnsureRectangleRoi(double midC, double midR, double phi, double length1, double length2, int pointCount)
        {
            ROIRectangle2 roi = GetActiveRectangleRoi();
            if (roi == null)
            {
                roiController.reset();
                roi = new ROIRectangle2();
                roi.createROI(midC, midR);
            
                roiController.setROIShape(roi);
                roiController.ROIList.Add(roi);
                roiController.activeROIidx = 0;
               // roiController.resetROI();
            }
            else
            {
                roiController.setROIShape(roi);
                roiController.resetROI();
            }
            
            roi.midC = midC;
            roi.midR = midR;
            roi.phi = phi;
            roi.length1 = length1;
            roi.length2 = length2;
            roi.PointNum = pointCount;
            roi.updateHandlePos();
        }

        private void UpdateRoiHandleSize()
        {
            if (!HasValidImage(image)) return;

            try
            {
                HOperatorSet.GetImageSize(image, out HTuple width, out HTuple _);
                double size = width.I < 1000 ? 1 : width.I / 1000.0 + 1.0;
                roiController.size = size;
                ROIRectangle2 roi = GetActiveRectangleRoi();
                //if (roi == null)
                //{
                //    if(roiController.ROIList.Count==0)
                //    {
                //        roi.size = size;
                //    }
                   
                //}
             
            }
            catch
            {
            }
        }

        private void ResetAndFitImage()
        {
            RefreshDisplayImage();
            if (displayImage == null || !displayImage.IsInitialized()) return;

            viewCtrl.addIconicVar(displayImage);
            viewCtrl.resetAll();
            viewCtrl.resetflag = true;
            viewCtrl.repaint();
            viewCtrl.resetflag = false;
        }

        private void RefreshDisplayImage()
        {
            displayImage?.Dispose();
            displayImage = null;
            if (!HasValidImage(image)) return;

            HObject displaySource = image;
            HObject grayImage = null;
            try
            {
                HOperatorSet.CountChannels(image, out HTuple channels);
                if (channels.I > 1)
                {
                    HOperatorSet.Rgb1ToGray(image, out grayImage);
                    displaySource = grayImage;
                }

                displayImage = displaySource.CopyObj(1, 1);
            }
            finally
            {
                if (grayImage != null && !ReferenceEquals(grayImage, displaySource))
                {
                    grayImage.Dispose();
                }
            }
        }

        private void OnViewRoiRedraw()
        {
            DrawOverlay();
        }

        private void OnViewRoiChanged(int eventCode)
        {
            RoiChanged?.Invoke(eventCode);
        }

        private void OnMouseMove(object sender, HMouseEventArgs e)
        {
            MouseMoved?.Invoke(new HalconMouseInfoArgs(e.Y, e.X, GetMouseText(e)));
            HalconMouseInfoArgs args = new HalconMouseInfoArgs(e.Y, e.X, GetMouseText(e));
           
            if (crosshairEnabled)
            {
                EmitGrayProfiles(args.Row, args.Column);
            }
          
        
        }

        private void OnMouseDown(object sender, HMouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            HalconMouseInfoArgs args = new HalconMouseInfoArgs(e.Y, e.X, GetMouseText(e));
           
            RightMouseDown?.Invoke(args);
        }

        private void OnMouseUp(object sender, HMouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            RightMouseUp?.Invoke(new HalconMouseInfoArgs(e.Y, e.X, GetMouseText(e)));
        }

        private void DrawOverlay()
        {
            if (overlayParams == null || disposed) return;

            try
            {
                DrawFitResult(HalconWindow, overlayParams, 8);
                DrawFollowRoi(HalconWindow, overlayParams);
                DrawCrosshair(HalconWindow, crossRow, crossCol, crosshairEnabled);
            }
            catch
            {
            }
        }

        private void DrawFitResult(HWindow target, ParamsMeasureLine parameters, double crossSize)
        {
            if (parameters == null) return;

            if (parameters.hv_CrossRow != null && parameters.hv_CrossColumn != null)
            {
                DrawCross(target, parameters.hv_CrossRow, parameters.hv_CrossColumn, crossSize, "green");
            }

            if (parameters.hv_LineRowBegin != null &&
                parameters.hv_LineColumnBegin != null &&
                parameters.hv_LineRowEnd != null &&
                parameters.hv_LineColumnEnd != null)
            {
                DrawLine(target, parameters.hv_LineRowBegin, parameters.hv_LineColumnBegin, parameters.hv_LineRowEnd, parameters.hv_LineColumnEnd, "yellow");
            }
        }

        private void DrawFollowRoi(HWindow target, ParamsMeasureLine parameters)
        {
            if (parameters == null) return;

            RoiFollower transformer = new RoiFollower(parameters, false);
            RoiRect2 line = transformer.Transform(parameters.hv_RoiMidRow, parameters.hv_RoiMidColumn, -parameters.hv_RoiAngle, parameters.hv_RoiHeight, parameters.hv_RoiWidth, parameters.hv_RoiPointsNum);

            target.SetDraw("margin");
            target.SetLineWidth(2);
            DrawRect2(target, line, "green", "抓边");
        }

        private static void DrawRect2(HWindow target, RoiRect2 roi, string color, string text)
        {
            target.SetColor(color);
            target.SetLineWidth(2);
            target.DispRectangle2(roi.Row, roi.Col, ToRad(roi.AngleDeg), roi.Height / 2.0, roi.Width / 2.0);
            target.SetTposition((int)Math.Round(roi.Row + 10), (int)Math.Round(roi.Col + 10));
            target.WriteString(text);
        }

        private static void DrawLine(HWindow target, HTuple row1, HTuple col1, HTuple row2, HTuple col2, string color)
        {
            HObject line = null;
            try
            {
                HOperatorSet.GenRegionLine(out line, row1, col1, row2, col2);
                target.SetColor(color);
                target.DispObj(line);
            }
            finally
            {
                line?.Dispose();
            }
        }

        private static void DrawCross(HWindow target, HTuple row, HTuple col, double size, string color)
        {
            HObject cross = null;
            try
            {
                HOperatorSet.GenCrossContourXld(out cross, row, col, size, 0);
                target.SetColor(color);
                target.DispObj(cross);
            }
            finally
            {
                cross?.Dispose();
            }
        }

        private static void DrawCrosshair(HWindow target, double row, double col, bool enabled)
        {
            if (!enabled || row < 0 || col < 0) return;
            target.SetColor("orange red");
            target.SetDraw("margin");
            target.DispLine(row, 0, row, 99999);
            target.DispLine(0, col, 99999, col);
        }

        private void EmitGrayProfiles(double row, double col)
        {
            if (!HasValidImage(image)) return;

            try
            {
                HObject profileImage = image;
                HObject channelImage = null;
                try
                {
                    HOperatorSet.CountChannels(profileImage, out HTuple channels);
                    if (channels.I > 1)
                    {
                        HOperatorSet.AccessChannel(profileImage, out channelImage, 1);
                        profileImage = channelImage;
                    }

                    HOperatorSet.GetImageSize(profileImage, out HTuple width, out HTuple height);
                    int w = Math.Max(0, width.I);
                    int h = Math.Max(0, height.I);
                    if (w <= 0 || h <= 0) return;

                    int r = Math.Max(0, Math.Min(h - 1, (int)Math.Round(row)));
                    int c = Math.Max(0, Math.Min(w - 1, (int)Math.Round(col)));

                    List<double> horizontal = new List<double>(w);
                    for (int x = 0; x < w; x++)
                    {
                        HOperatorSet.GetGrayval(profileImage, r, x, out HTuple gray);
                        horizontal.Add(gray.D);
                    }

                    List<double> vertical = new List<double>(h);
                    for (int y = 0; y < h; y++)
                    {
                        HOperatorSet.GetGrayval(profileImage, y, c, out HTuple gray);
                        vertical.Add(gray.D);
                    }

                    GrayProfileChanged?.Invoke(horizontal.ToArray(), vertical.ToArray());
                }
                finally
                {
                    channelImage?.Dispose();
                }
            }
            catch
            {
            }
        }

        private static double ToRad(double angleDeg)
        {
            HOperatorSet.TupleRad(angleDeg, out HTuple rad);
            return rad.D;
        }

        private static void SetPrivateDouble(object target, string fieldName, double value)
        {
            System.Reflection.FieldInfo field = target.GetType().GetField(fieldName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (field != null) field.SetValue(target, value);
        }

        private static bool HasValidImage(HObject source)
        {
            if (source == null || !source.IsInitialized()) return false;
            try
            {
                HOperatorSet.CountObj(source, out HTuple count);
                if (count.I <= 0) return false;
                HOperatorSet.GetObjClass(source, out HTuple objClass);
                return string.Equals(objClass.S, "image", StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }
    }

    public sealed class HalconMouseInfoArgs
    {
        public HalconMouseInfoArgs(double row, double column, string text)
        {
            Row = row;
            Column = column;
            Text = text ?? string.Empty;
        }

        public double Row { get; private set; }
        public double Column { get; private set; }
        public string Text { get; private set; }
    }
}

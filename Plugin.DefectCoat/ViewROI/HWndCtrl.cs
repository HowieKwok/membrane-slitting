using System;
using System.Collections;
using HalconDotNet;
using System.Windows.Forms;
namespace ViewROI
{
    /// <summary>HALCON 图像/ROI 状态事件通知委托。</summary>
    public delegate void IconicDelegate(int val);
    /// <summary>无参数回调委托，用于重绘后叠加自定义显示。</summary>
    public delegate void FuncDelegate();



    /// <summary>
    /// HALCON 窗口交互控制器。
    /// 封装 HWindowControl 的图像显示、缩放、平移、ROI 绘制和鼠标灰度读取。
    /// 本插件通过 HalconViewRoiAdapter 使用该类，保持检测 ROI 与 ViewModel 参数同步。
	/// </summary>
	public class HWndCtrl
    {

        /// <summary>窗口重绘完成后的回调委托。</summary>
        public delegate void ReDrawDelegate(); // 窗口重绘完成后的回调委托。
        /// <summary>窗口重绘完成事件，外部可在事件中叠加自定义对象。</summary>
        public event ReDrawDelegate ReDrawEvent; // 外部可在重绘完成后叠加自定义对象。

        /// <summary>图像处理触发委托。</summary>
        public delegate void ImageProcessingDelegate(); // 图像处理触发委托。
        /// <summary>图像刷新事件，供外部接入自定义处理。</summary>
        public event ImageProcessingDelegate ImageProcessingEvent; // 图像刷新时通知外部处理。

        /// <summary>是否允许 ROI 响应鼠标操作。为 false 时只显示 ROI，不允许拖动。</summary>
        public bool bl_RoiOperateFlag = true;



        /// <summary>鼠标不执行任何窗口操作。</summary>
        public const int MODE_VIEW_NONE = 10;

        /// <summary>鼠标滚轮或框选缩放图像。</summary>
        public const int MODE_VIEW_ZOOM = 11;

        /// <summary>鼠标拖拽平移图像。</summary>
        public const int MODE_VIEW_MOVE = 12;

        /// <summary>鼠标框选局部放大。</summary>
        public const int MODE_VIEW_ZOOMWINDOW = 13;

        /// <summary>显示并允许操作 ROI。</summary>
        public const int MODE_INCLUDE_ROI = 1;

        /// <summary>隐藏或排除 ROI 操作。</summary>
        public const int MODE_EXCLUDE_ROI = 2;


        /// <summary>
        /// 图像更新事件编号。
        /// </summary>
        public const int EVENT_UPDATE_IMAGE = 31;
        /// <summary>
        /// 读取图像失败事件编号。
        /// </summary>
        public const int ERR_READING_IMG = 32;
        /// <summary> 
        /// 定义图形上下文失败事件编号。
        /// </summary>
        public const int ERR_DEFINING_GC = 33;

        /// <summary> 
        /// 图形栈最大对象数量。当前插件主显示只保留一张主图，避免旧对象堆积。
        /// </summary>
        private const int MAXNUMOBJLIST = 1;


        /// <summary>当前窗口交互模式，取 MODE_VIEW_*。</summary>
        private int stateView;

        /// <summary>鼠标左键是否处于按下状态。</summary>
        public bool mousePressed = false;

        /// <summary>鼠标拖动起点坐标，单位为窗口坐标。</summary>
        private double startX, startY;

        /// <summary>框选放大时的窗口矩形。</summary>
        public System.Drawing.Rectangle rect;

        /// <summary>是否需要重置显示区域。</summary>
        public bool resetflag = true;

        /// <summary>当前绑定的 HALCON WindowsForms 控件。</summary>
        public HWindowControl viewPort;

        /// <summary>
        /// ROI 控制器实例，负责 ROI 创建、选中、拖拽和模型区域生成。
        /// </summary>
        private ROIController roiManager;

        /// <summary>控制 ROI 是否绘制以及是否响应鼠标事件，取 MODE_INCLUDE_ROI 或 MODE_EXCLUDE_ROI。</summary>
        private int dispROI;


        // 当前窗口尺寸和图像尺寸，用于缩放、平移和坐标换算。
        /// <summary>当前 HALCON 控件宽度，单位 px。</summary>
        private int windowWidth;
        /// <summary>当前 HALCON 控件高度，单位 px。</summary>
        private int windowHeight;
        /// <summary>当前图像宽度，单位 px。</summary>
        private int imageWidth;
        /// <summary>当前图像高度，单位 px。</summary>
        private int imageHeight;
        /// <summary>鼠标位置灰度读取时使用的临时字符串。</summary>
        String str;
        /// <summary>当前显示图像列方向范围。</summary>
        private int[] CompRangeX;
        /// <summary>当前显示图像行方向范围。</summary>
        private int[] CompRangeY;

        /// <summary>框选缩放时记录的起止图像坐标。</summary>
        double hv_StartX, hv_StartY, hv_EndX, hv_EndY;

        /// <summary>上一次窗口坐标，用于平移计算。</summary>
        private int prevCompX, prevCompY;
        /// <summary>窗口像素到图像坐标的换算步长。</summary>
        private double stepSizeX, stepSizeY;


        /// <summary>当前 HALCON 窗口显示的图像区域行列范围。</summary>
        private double ImgRow1, ImgCol1, ImgRow2, ImgCol2;

        /// <summary>最近一次 HALCON 窗口或图形上下文异常信息。</summary>
        public string exceptionText = "";


        // Delegates to send notification messages to other classes
        /// <summary>窗口绘制完成后由外部追加文字、十字线或其它提示的回调。</summary>
        public FuncDelegate addInfoDelegate;

        /// <summary>窗口控制器异常或状态变化通知回调。</summary>
        public IconicDelegate NotifyIconObserver;


        /// <summary>放大镜窗口对象。</summary>
        private HWindow ZoomWindow;
        /// <summary>放大镜窗口显示倍率。</summary>
        public double zoomWndFactor;
        /// <summary>鼠标滚轮缩放倍率。</summary>
        public double zoomfactor;
        /// <summary>缩放增量基准值。</summary>
        private double zoomAddOn;
        /// <summary>放大镜窗口尺寸。</summary>
        private int zoomWndSize;


        /// <summary>当前显示的 HALCON 图像或区域对象。</summary>
        public HObject ho_Image = new HObject();
        /// <summary>显示变更后的图像对象，供外部处理或刷新时使用。</summary>
        public HObject ho_Image_Changed = new HObject();
        /// <summary>HALCON 图形上下文对象，保存颜色、线宽、绘制模式等显示状态。</summary>
        private GraphicsContext mGC;


        /// <summary> 
        /// Initializes the image dimension, mouse delegation, and the 
        /// graphical context setup of the instance.
        /// </summary>
        /// <param name="view"> HALCON window </param>
        public HWndCtrl(HWindowControl view)
        {
            viewPort = view;
            stateView = MODE_VIEW_NONE;
            windowWidth = viewPort.Size.Width;
            windowHeight = viewPort.Size.Height;

            zoomWndFactor = (double)imageWidth / viewPort.Width;
            zoomAddOn = Math.Pow(0.9, 5);
            zoomWndSize = 150;

            /*default*/
            CompRangeX = new int[] { 0, 100 };
            CompRangeY = new int[] { 0, 100 };

            prevCompX = prevCompY = 0;

            dispROI = MODE_INCLUDE_ROI;//1;

            viewPort.HMouseUp += new HalconDotNet.HMouseEventHandler(this.mouseUp);
            viewPort.HMouseDown += new HalconDotNet.HMouseEventHandler(this.mouseDown);
            viewPort.HMouseMove += new HalconDotNet.HMouseEventHandler(this.mouseMoved);
            viewPort.HMouseWheel += new HalconDotNet.HMouseEventHandler(this.mouseWheeled);


            addInfoDelegate = new FuncDelegate(dummyV);
            NotifyIconObserver = new IconicDelegate(dummy);

            // graphical stack 
            mGC = new GraphicsContext();
            mGC.gcNotification = new GCDelegate(exceptionGC);
        }


        /// <summary>
        /// 绑定 ROIController。
        /// HWndCtrl 负责鼠标坐标和重绘，ROIController 负责 ROI 创建、选中和拖拽。
        /// </summary>
        /// <param name="rC"> 
        /// Controller that manages interactive ROIs for the HALCON window 
        /// </param>
        public void useROIController(ROIController rC)
        {
            roiManager = rC;
            rC.setViewController(this);
        }


        /// <summary>读取 HALCON 图像尺寸，并据此初始化窗口显示范围。</summary>
        /// <param name="image">HALCON image</param>
        private void setImagePart(HImage image)
        {
            string s;
            int w, h;

            image.GetImagePointer1(out s, out w, out h);
            setImagePart(0, 0, h, w);
        }


        /// <summary>
        /// 根据给定左上/右下图像坐标设置窗口显示范围。
        /// resetflag 为 true 时才重置，避免每次重绘都覆盖用户缩放和平移。
        /// </summary>
        /// <param name="r1">y coordinate of left upper corner</param>
        /// <param name="c1">x coordinate of left upper corner</param>
        /// <param name="r2">y coordinate of right lower corner</param>
        /// <param name="c2">x coordinate of right lower corner</param>
        private void setImagePart(int r1, int c1, int r2, int c2)
        {
            if (resetflag)
            {
                ImgRow1 = r1;
                ImgCol1 = c1;
                ImgRow2 = imageHeight = r2;
                ImgCol2 = imageWidth = c2;
                rect.X = (int)ImgCol1;
                rect.Y = (int)ImgRow1;
                rect.Height = (int)imageHeight;
                rect.Width = (int)imageWidth;
                resetflag = false;
            }

        }


        /// <summary>设置 HALCON 窗口鼠标交互模式，例如平移、缩放、放大镜或无操作。</summary>
        /// <param name="mode">One of the MODE_VIEW_* constants</param>
        public void setViewState(int mode)
        {
            stateView = mode;
            if (roiManager != null)
                roiManager.resetROI();
        }

        /// <summary>默认空状态通知回调。</summary>
        private void dummy(int val)
        {
        }

        /// <summary>默认空重绘后处理回调。</summary>
        private void dummyV()
        {
        }

        /// <summary>图形上下文异常回调，记录异常并通知观察者。</summary>
        private void exceptionGC(string message)
        {
            exceptionText = message;
            NotifyIconObserver(ERR_DEFINING_GC);
        }

        /// <summary>设置是否显示并允许操作 ROI。</summary>
        public void setDispLevel(int mode)
        {
            dispROI = mode;
        }
        /// <summary>
        /// 按鼠标位置缩放当前显示图像区域。
        /// x/y 为图像坐标，scale 小于 1 放大，大于 1 缩小。
        /// </summary>
        private void zoomImage(double x, double y, double scale)
        {
            double lengthC, lengthR;
            double percentC, percentR;
            int lenC, lenR;

            if (viewPort.ImagePart.Width < 15 || viewPort.ImagePart.Height < 15)
            {
                if (scale <= 1)
                    return;
            }

            percentC = (x - ImgCol1) / viewPort.ImagePart.Width;
            percentR = (y - ImgRow1) / viewPort.ImagePart.Height;

            lengthC = viewPort.ImagePart.Width * scale;
            lengthR = viewPort.ImagePart.Height * scale;

            ImgCol1 = x - lengthC * percentC;
            ImgRow1 = y - lengthR * percentR;

            lenC = (int)Math.Round(lengthC);
            lenR = (int)Math.Round(lengthR);


            rect.X = (int)Math.Round(ImgCol1);
            rect.Y = (int)Math.Round(ImgRow1);
            rect.Width = (lenC > 0) ? lenC : 1;
            rect.Height = (lenR > 0) ? lenR : 1;

            zoomfactor = Convert.ToDouble(imageHeight) / Convert.ToDouble(rect.Height);
            if (zoomfactor>0)
            {
                repaint();
            }
    

        }

        /// <summary>按窗口拖拽位移平移当前显示图像区域。</summary>
        private void moveImage(double motionX, double motionY)
        {
            ImgRow1 += -motionY;
            ImgRow2 += -motionY;

            ImgCol1 += -motionX;
            ImgCol2 += -motionX;


            rect.X = (int)Math.Round(ImgCol1);
            rect.Y = (int)Math.Round(ImgRow1);
            repaint();
        }


        /// <summary>重置 HALCON 窗口显示范围到初始图像范围。</summary>
        public void resetAll()
        {
            resetflag = true;
            setImagePart((int)hv_StartX, (int)hv_StartY, (int)hv_EndX, (int)hv_EndY);

        }


        /// <summary>鼠标滚轮事件，根据滚轮方向缩放图像。</summary>
        private void mouseWheeled(object sender, HalconDotNet.HMouseEventArgs e)
        {

            double scale;
            if (e.Delta < 0)
                scale = 1.25;
            else if (e.Delta > 0)
                scale = 0.8;
            else
                scale = 1;

            zoomImage(e.X, e.Y, scale);
        }

        /// <summary>鼠标按下事件，优先交给 ROIController 创建/选中 ROI，否则进入平移或放大镜模式。</summary>
        private void mouseDown(object sender, HalconDotNet.HMouseEventArgs e)
        {

            mousePressed = true;
            int activeROIidx = -1;

            if (roiManager != null && (dispROI == MODE_INCLUDE_ROI)& bl_RoiOperateFlag)
            {
                activeROIidx = roiManager.mouseDownAction(e.X, e.Y);
            }

            if (activeROIidx == -1)
            {
                switch (stateView)
                {
                    case MODE_VIEW_MOVE:
                        startX = e.X;
                        startY = e.Y;
                        break;
                    case MODE_VIEW_NONE:
                        break;
                    case MODE_VIEW_ZOOMWINDOW:
                        activateZoomWindow((int)e.X, (int)e.Y);
                        break;
                    default:
                        break;
                }

            }
            //end of if
        }

        /// <summary>打开局部放大窗口，并以当前鼠标位置为中心显示放大区域。</summary>
        private void activateZoomWindow(int X, int Y)
        {
            double posX, posY;
            int zoomZone;

            if (ZoomWindow != null)
                ZoomWindow.Dispose();

            HOperatorSet.SetSystem("border_width", 10);
            ZoomWindow = new HWindow();

            posX = ((X - ImgCol1) / (ImgCol2 - ImgCol1)) * viewPort.Width;
            posY = ((Y - ImgRow1) / (ImgRow2 - ImgRow1)) * viewPort.Height;

            zoomZone = (int)((zoomWndSize / 2) * zoomWndFactor * zoomAddOn);
            ZoomWindow.OpenWindow((int)posY - (zoomWndSize / 2), (int)posX - (zoomWndSize / 2),
                                   zoomWndSize, zoomWndSize,
                                   viewPort.HalconID, "visible", "");
            ZoomWindow.SetPart(Y - zoomZone, X - zoomZone, Y + zoomZone, X + zoomZone);
            repaint(ZoomWindow);
            ZoomWindow.SetColor("black");
        }

        /// <summary>鼠标抬起事件，结束 ROI 拖动、通知 ROI 更新或关闭放大镜窗口。</summary>
        private void mouseUp(object sender, HalconDotNet.HMouseEventArgs e)
        {
            mousePressed = false;

            if (roiManager != null
                && (roiManager.activeROIidx != -1)
                && (dispROI == MODE_INCLUDE_ROI)& bl_RoiOperateFlag)
            {
                roiManager.NotifyRCObserver(ROIController.EVENT_UPDATE_ROI);
                roiManager.activeROIidx = -1;
                repaint(viewPort.HalconWindow);
            }
            else if (stateView == MODE_VIEW_ZOOMWINDOW)
            {
                ZoomWindow.Dispose();
            }
        }

        /// <summary>鼠标移动事件，根据当前模式拖动 ROI、平移图像或刷新放大镜。</summary>
        private void mouseMoved(object sender, HalconDotNet.HMouseEventArgs e)
        {
            double motionX, motionY;
            double posX, posY;
            double zoomZone;

            if (!mousePressed)
                return;
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (roiManager != null && (roiManager.activeROIidx != -1) && (dispROI == MODE_INCLUDE_ROI)& bl_RoiOperateFlag)
                {
                    roiManager.mouseMoveAction(e.X, e.Y);
                }
                else if (stateView == MODE_VIEW_MOVE)
                {
                    motionX = ((e.X - startX));
                    motionY = ((e.Y - startY));

                    if (((int)motionX != 0) || ((int)motionY != 0))
                    {
                        moveImage(motionX, motionY);
                        startX = e.X - motionX;
                        startY = e.Y - motionY;
                    }
                }
                else if (stateView == MODE_VIEW_ZOOMWINDOW)
                {
                    HSystem.SetSystem("flush_graphic", "false");
                    ZoomWindow.ClearWindow();


                    posX = ((e.X - ImgCol1) / (ImgCol2 - ImgCol1)) * viewPort.Width;
                    posY = ((e.Y - ImgRow1) / (ImgRow2 - ImgRow1)) * viewPort.Height;
                    zoomZone = (zoomWndSize / 2) * zoomWndFactor * zoomAddOn;

                    ZoomWindow.SetWindowExtents((int)posY - (zoomWndSize / 2),
                                                (int)posX - (zoomWndSize / 2),
                                                zoomWndSize, zoomWndSize);
                    ZoomWindow.SetPart((int)(e.Y - zoomZone), (int)(e.X - zoomZone),
                                       (int)(e.Y + zoomZone), (int)(e.X + zoomZone));
                    repaint(ZoomWindow);

                    HSystem.SetSystem("flush_graphic", "true");
                    ZoomWindow.DispLine(-100.0, -100.0, -100.0, -100.0);
                }
            }
        }

        /// <summary>触发绑定的主 HALCON 窗口重绘。</summary>
        public void repaint()
        {
            try
            {
                viewPort.ImagePart = rect;
                repaint(viewPort.HalconWindow);
            }
            catch (Exception)
            { }

        }

        /// <summary>
        /// 重绘指定 HALCON 窗口。
        /// 先显示当前图像，再绘制 ROI，最后触发外部 ReDrawEvent 叠加运行结果。
        /// </summary>
        public void repaint(HalconDotNet.HWindow window)
        {
            HSystem.SetSystem("flush_graphic", "false");
            window.ClearWindow();
            if (ImageProcessingEvent != null)
                ImageProcessingEvent();

            if (ho_Image_Changed != null)
            {
                //HOperatorSet.GetImageSize(ho_Image_Changed, out HTuple Width, out HTuple Height);
                //HOperatorSet.SetPart(window, 0, 0, Height, Width);
                 window.DispObj(ho_Image_Changed);
            }

            if (roiManager != null & (dispROI == MODE_INCLUDE_ROI)&bl_RoiOperateFlag)
                roiManager.paintData(window);

            HSystem.SetSystem("flush_graphic", "true");

            window.SetColor("black");
            window.DispLine(-100.0, -100.0, -101.0, -101.0);

            if (ReDrawEvent != null)
                ReDrawEvent();

            
            //mGC.stateOfSettings.Clear();
        }

        /********************************************************************/
        /*                      GRAPHICSSTACK                               */
        /********************************************************************/

        ///// <summary>
        /// 旧版图像加入显示栈方法，已保留注释供对比。
        ///// </summary>
        ///// <param name="obj">Iconic object</param>
        //public void addIconicVar(HObject obj)
        //{
        //    HObjectEntry entry;

        //    if (obj == null)
        //        return;

        //    if (obj is HImage)
        //    {
        //        double win_Width, win_Height;
        //        string s;
        //        win_Width = viewPort.WindowSize.Width;
        //        win_Height = viewPort.WindowSize.Height;
        //        ((HImage)obj).GetImagePointer1(out s, out imageWidth, out imageHeight);
        //        //HOperatorSet.GetImageSize(obj, out HTuple _imageWidth, out HTuple _imageHeight);
        //        //imageWidth=_imageWidth.I; imageHeight=_imageHeight.I;
        //        double imgAspectRatio = Convert.ToDouble(imageWidth) / Convert.ToDouble(imageHeight);
        //        double winAspectRatio = Convert.ToDouble(win_Width) / Convert.ToDouble(win_Height);

        //        try
        //        {
        //            hv_StartX = hv_StartY = hv_EndX = hv_EndY = 0;

        //            if (imgAspectRatio > winAspectRatio)
        //            {
        // 图像比窗口更宽时，按宽度适应并在行方向居中。
        //                hv_StartY = 0;
        //                zoomWndFactor = imageWidth / win_Width;
        //                hv_StartX = -(win_Height - imageHeight / zoomWndFactor) / 2 * zoomWndFactor;
        //            }
        //            if (imgAspectRatio <= winAspectRatio)
        //            {
        // 图像比窗口更高时，按高度适应并在列方向居中。
        //                hv_StartX = 0;
        //                zoomWndFactor = imageHeight / win_Height;
        //                hv_StartY = -(win_Width - imageWidth / zoomWndFactor) / 2 * zoomWndFactor;
        //            }
        //            hv_EndX = imageHeight - hv_StartX * 2;
        //            hv_EndY = imageWidth - hv_StartY * 2;
        //            setImagePart((int)hv_StartX, (int)hv_StartY, (int)hv_EndX, (int)hv_EndY);

        //        }
        //        catch (Exception)
        //        {
        //        }

        //    }

        //    entry = new HObjectEntry(obj, mGC.copyContextList());

        //    ho_Image_Changed = ho_Image = entry.HObj;

        //}

        /// <summary>
        /// 将 HALCON 图像或 Region 加入显示栈，并根据窗口比例计算初始显示范围。
        /// </summary>
        /// <param name="obj">待显示的 HALCON 图像或区域对象。</param>
        public void addIconicVar(HObject obj)
        {
            HObjectEntry entry;

            if (obj == null)
                return;

            if (obj is HObject)
            {
                try
                {
                    double win_Width, win_Height;
                    string s;
                    win_Width = viewPort.WindowSize.Width;
                    win_Height = viewPort.WindowSize.Height;
                    HTuple W, H;
                    HOperatorSet.GetImageSize(obj, out W, out H);
                    imageWidth = Convert.ToInt32(W.D);
                    imageHeight = Convert.ToInt32(H.D);


                    double imgAspectRatio = Convert.ToDouble(imageWidth) / Convert.ToDouble(imageHeight);
                    double winAspectRatio = Convert.ToDouble(win_Width) / Convert.ToDouble(win_Height);

                    hv_StartX = hv_StartY = hv_EndX = hv_EndY = 0;

                    if (imgAspectRatio > winAspectRatio)
                    {
                        // 图像比窗口更宽时，按宽度适应并在行方向居中。
                        hv_StartY = 0;
                        zoomWndFactor = imageWidth / win_Width;
                        hv_StartX = -(win_Height - imageHeight / zoomWndFactor) / 2 * zoomWndFactor;
                    }
                    if (imgAspectRatio <= winAspectRatio)
                    {
                        // 图像比窗口更高时，按高度适应并在列方向居中。
                        hv_StartX = 0;
                        zoomWndFactor = imageHeight / win_Height;
                        hv_StartY = -(win_Width - imageWidth / zoomWndFactor) / 2 * zoomWndFactor;
                    }
                    hv_EndX = imageHeight - hv_StartX * 2;
                    hv_EndY = imageWidth - hv_StartY * 2;
                    setImagePart((int)hv_StartX, (int)hv_StartY, (int)hv_EndX, (int)hv_EndY);

                }
                catch (Exception)
                {
                }

            }

            entry = new HObjectEntry(obj, mGC.copyContextList());

            ho_Image_Changed = ho_Image = entry.HObj;

        }

        /// <summary>
        /// 读取鼠标当前位置的图像行、列和灰度值。
        /// 鼠标超出图像范围或读取失败时返回占位文本。
        /// </summary>
        public string  Get_Mouseposition_and_Gray(HTuple hv_Window, HObject Image, HMouseEventArgs e)
        {
            HTuple ptC, ptR, hv_Button;
            HTuple row, col, grayval;
            HTuple Image_Width, Image_Height;
            try
            {
                HOperatorSet.GetImageSize(Image, out Image_Width, out Image_Height);
                HOperatorSet.GetMposition(hv_Window, out ptR, out ptC, out hv_Button);
                row = (ptR).TupleInt();
                col = (ptC).TupleInt();
                if (Image != null && (row >= 0 && row <= Image_Height) && (col >= 0 && col <= Image_Width)) // 鼠标坐标落在图像范围内才读取灰度。
                {
                    HOperatorSet.GetGrayval(Image, row, col, out grayval); // 读取当前像素灰度值。
                    str = String.Format("R:{0}  C:{1}  Value:{2}",  row, col, grayval); // 返回界面状态栏显示文本。

                }
                else
                {
                    str = "R: --  C: --  Value: -- ";
                }
            }
            catch (Exception)
            {

                str = "R: --  C: --  Value: -- ";
            }

            return str;
        }





    }//end of class
}//end of namespace

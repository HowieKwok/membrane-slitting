using Frame_Core.pluginParams;
using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using ViewROI;

namespace Plugin.ReusableMeasureDetect.Services
{
    /// <summary>
    /// HALCON 显示窗口和 ViewROI 控件的适配层。
    /// ViewModel 不直接操作 ROIController/HWndCtrl，而是通过本类刷新图像、同步 ROI、获取鼠标图像坐标和灰度、触发叠加绘制。
    /// </summary>
    public sealed class HalconViewRoiAdapter : IDisposable
    {
        private readonly HWindowControl window;
        private readonly HWndCtrl viewController;
        private readonly ROIController roiController;
        private readonly HWndCtrl.ReDrawDelegate redrawHandler;
        private readonly IconicDelegate roiChangedHandler;
        private HObject image;
        private HObject displayImage;
        private ParamsReusableMeasureDetect parameters;
        private bool displayImageOwned;
        private bool disposed;

        /// <summary>
        /// 创建适配器并完成 ViewROI 事件绑定。
        /// </summary>
        /// <param name="windowControl">界面宿主中的 HALCON 窗口控件。</param>
        public HalconViewRoiAdapter(HWindowControl windowControl)
        {
            window = windowControl ?? throw new ArgumentNullException(nameof(windowControl));
            roiController = new ROIController();
            viewController = new HWndCtrl(window);
            viewController.useROIController(roiController);
            viewController.setDispLevel(HWndCtrl.MODE_INCLUDE_ROI);
            viewController.setViewState(HWndCtrl.MODE_VIEW_MOVE);
            viewController.bl_RoiOperateFlag = false;
            viewController.ho_Image?.Dispose();
            viewController.ho_Image_Changed?.Dispose();
            viewController.ho_Image = null;
            viewController.ho_Image_Changed = null;
            roiController.setDrawColor("yellow", "red", "gray");

            redrawHandler = OnViewRedraw;
            roiChangedHandler = OnRoiChanged;
            viewController.ReDrawEvent += redrawHandler;
            roiController.NotifyRCObserver += roiChangedHandler;
            window.HMouseMove += OnMouseMove;
            window.HMouseDown += OnMouseDown;
            window.HMouseUp += OnMouseUp;
        }

        /// <summary>
        /// ViewROI 重绘后请求 ViewModel 叠加绘制检测点、拟合线和结果标注。
        /// </summary>
        public event Action<HWindow> DrawOverlayRequested;

        /// <summary>
        /// ROI 被创建、移动或更新时触发，参数为 ViewROI 的事件码。
        /// </summary>
        public event Action<int> RoiChanged;

        /// <summary>
        /// 鼠标移动时回传图像坐标、灰度和图像尺寸，供底部状态栏显示。
        /// </summary>
        public event Action<ReusableMeasureMouseInfo> MouseInfoChanged;

        /// <summary>
        /// 右键按下时回传当前坐标，用于打开位置跟随快捷菜单。
        /// </summary>
        public event Action<ReusableMeasureMouseInfo> RightMouseDown;

        /// <summary>
        /// 右键抬起事件，目前保留给后续扩展。
        /// </summary>
        public event Action<ReusableMeasureMouseInfo> RightMouseUp;

        /// <summary>
        /// 暴露底层 HALCON 窗口，供少量必须直接绘制的调用使用。
        /// </summary>
        public HWindow HalconWindow { get { return window.HalconWindow; } }

        /// <summary>
        /// 从参数刷新图像、ROI 和显示区域。
        /// fitImage=true 时重置视野适应图像；false 时只重绘当前视野。
        /// </summary>
        public void RefreshFromParams(ParamsReusableMeasureDetect source, bool fitImage)
        {
            if (disposed) return;
            parameters = source;
            image = source == null ? null : source.ho_Image;
            bool enableRoi = source != null && source.EnableRoi;
            viewController.bl_RoiOperateFlag = enableRoi;
            if (enableRoi) EnsureRectangleRoi(source);

            if (HasValidImage(image))
            {
                RefreshDisplayImage();
                if (displayImage != null && displayImage.IsInitialized())
                    viewController.addIconicVar(displayImage);
                if (fitImage) viewController.resetAll();
                viewController.resetflag = fitImage;
                viewController.repaint();
                viewController.resetflag = false;
                return;
            }

            RepaintWithoutImage();
        }

        /// <summary>
        /// 重新绘制当前图像和叠加层，不修改参数和 ROI。
        /// </summary>
        public void Repaint()
        {
            if (disposed) return;
            if (HasValidImage(image)) viewController.repaint();
            else RepaintWithoutImage();
        }

        /// <summary>
        /// 获取当前激活的旋转矩形 ROI；没有激活项时尝试返回 ROI 列表第一个矩形。
        /// </summary>
        public ROIRectangle2 GetActiveRectangleRoi()
        {
            ROIRectangle2 active = roiController.getActiveROI() as ROIRectangle2;
            if (active != null) return active;
            if (roiController.ROIList != null && roiController.ROIList.Count > 0)
                return roiController.ROIList[0] as ROIRectangle2;
            return null;
        }

        /// <summary>
        /// 把 ViewROI 中的矩形位置、角度、宽高和采样点数同步回插件参数。
        /// </summary>
        /// <returns>成功同步返回 true；没有 ROI 或参数为空返回 false。</returns>
        public bool UpdateParamsFromActiveRoi(ParamsReusableMeasureDetect target)
        {
            if (target == null) return false;
            ROIRectangle2 roi = GetActiveRectangleRoi();
            if (roi == null) return false;
            HOperatorSet.TupleDeg(roi.phi, out HTuple angleDeg);
            target.RoiCenterRow = roi.midR;
            target.RoiCenterCol = roi.midC;
            target.RoiAngleDeg = angleDeg.D;
            target.RoiHalfLength1 = Math.Max(0.001, roi.length1);
            target.RoiHalfLength2 = Math.Max(0.001, roi.length2);
            target.RoiPointNum = Math.Max(2, roi.PointNum);
            return true;
        }

        /// <summary>
        /// 解除事件绑定并释放本适配器创建的 HALCON 显示图像，防止窗口关闭后仍持有引用。
        /// </summary>
        public void Dispose()
        {
            if (disposed) return;
            disposed = true;
            try
            {
                viewController.ReDrawEvent -= redrawHandler;
                roiController.NotifyRCObserver -= roiChangedHandler;
                window.HMouseMove -= OnMouseMove;
                window.HMouseDown -= OnMouseDown;
                window.HMouseUp -= OnMouseUp;
                roiController.reset();
            }
            catch
            {
            }
            DisposeDisplayImage();
            image = null;
            parameters = null;
        }

        /// <summary>
        /// 确保 ViewROI 中存在一个旋转矩形 ROI，并用参数值刷新其中心、角度、半长、半宽和点数。
        /// </summary>
        private void EnsureRectangleRoi(ParamsReusableMeasureDetect source)
        {
            ROIRectangle2 roi = GetActiveRectangleRoi();
            if (roi == null)
            {
                roiController.ROIList.Clear();
                roiController.activeROIidx = -1;
                roi = new ROIRectangle2();
                roi.createROI(source.RoiCenterCol, source.RoiCenterRow);
                roiController.setROIShape(roi);
                roiController.ROIList.Add(roi);
                roiController.activeROIidx = 0;
            }

            HOperatorSet.TupleRad(source.RoiAngleDeg, out HTuple angleRad);
            roi.midR = source.RoiCenterRow;
            roi.midC = source.RoiCenterCol;
            roi.phi = angleRad.D;
            roi.length1 = Math.Max(0.001, Math.Abs(source.RoiHalfLength1));
            roi.length2 = Math.Max(0.001, Math.Abs(source.RoiHalfLength2));
            roi.PointNum = Math.Max(2, source.RoiPointNum);
            roi.updateHandlePos();
        }

        /// <summary>
        /// 准备用于显示的图像。
        /// 彩色图会转换为灰度副本并由本类释放；灰度图直接引用原对象，不在本类释放。
        /// </summary>
        private void RefreshDisplayImage()
        {
            DisposeDisplayImage();
            if (!HasValidImage(image)) return;

            try
            {
                HOperatorSet.CountChannels(image, out HTuple channels);
                if (channels.I > 1)
                {
                    HOperatorSet.Rgb1ToGray(image, out displayImage);
                    displayImageOwned = true;
                    return;
                }

                displayImage = image;
                displayImageOwned = false;
            }
            catch
            {
                DisposeDisplayImage();
            }
        }

        /// <summary>
        /// 只释放本类拥有的显示图像副本，避免误释放参数中的原始输入图像。
        /// </summary>
        private void DisposeDisplayImage()
        {
            if (displayImageOwned && displayImage != null)
                displayImage.Dispose();
            //viewController.ho_Image?.Dispose();
            //viewController.ho_Image_Changed?.Dispose();
            viewController.ho_Image = null;
            viewController.ho_Image_Changed = null;
            displayImage = null;
            displayImageOwned = false;
        }

        /// <summary>
        /// 无有效图像时按点集和 ROI 自动设置显示范围，然后绘制 ROI 与叠加结果。
        /// </summary>
        private void RepaintWithoutImage()
        {
            HWindow target = window.HalconWindow;
            try
            {
                SetPartFromData(target);
                target.ClearWindow();
                if (parameters != null && parameters.EnableRoi)
                    roiController.paintData(target);
                RaiseOverlay(target);
            }
            catch
            {
            }
        }

        /// <summary>
        /// 根据输入点、滤波点和 ROI 计算显示窗口 SetPart 范围，用于无图仿真和点集调试。
        /// </summary>
        private void SetPartFromData(HWindow target)
        {
            double rowMin = 0;
            double colMin = 0;
            double rowMax = 700;
            double colMax = 700;
            bool hasData = false;

            if (parameters != null)
            {
                AddPointBounds(parameters.InputPoints, ref rowMin, ref colMin, ref rowMax, ref colMax, ref hasData);
                if (parameters.FilterOutput != null)
                    AddPointBounds(parameters.FilterOutput.SelectedPoints, ref rowMin, ref colMin, ref rowMax, ref colMax, ref hasData);
                if (parameters.EnableRoi)
                {
                    AddBounds(parameters.RoiCenterRow - parameters.RoiHalfLength1 - parameters.RoiHalfLength2,
                        parameters.RoiCenterCol - parameters.RoiHalfLength1 - parameters.RoiHalfLength2,
                        parameters.RoiCenterRow + parameters.RoiHalfLength1 + parameters.RoiHalfLength2,
                        parameters.RoiCenterCol + parameters.RoiHalfLength1 + parameters.RoiHalfLength2,
                        ref rowMin, ref colMin, ref rowMax, ref colMax, ref hasData);
                }
            }

            double margin = Math.Max(20.0, Math.Max(rowMax - rowMin, colMax - colMin) * 0.08);
            target.SetPart((int)Math.Floor(rowMin - margin), (int)Math.Floor(colMin - margin), (int)Math.Ceiling(rowMax + margin), (int)Math.Ceiling(colMax + margin));
        }

        /// <summary>
        /// 将点集范围合并到显示包围盒。
        /// </summary>
        private static void AddPointBounds(IList<MeasurePointParam> points, ref double rowMin, ref double colMin, ref double rowMax, ref double colMax, ref bool hasData)
        {
            if (points == null) return;
            for (int index = 0; index < points.Count; index++)
            {
                MeasurePointParam point = points[index];
                AddBounds(point.Row, point.Col, point.Row, point.Col, ref rowMin, ref colMin, ref rowMax, ref colMax, ref hasData);
            }
        }

        /// <summary>
        /// 将一个矩形范围合并到显示包围盒。
        /// </summary>
        private static void AddBounds(double minRow, double minCol, double maxRow, double maxCol, ref double rowMin, ref double colMin, ref double rowMax, ref double colMax, ref bool hasData)
        {
            if (!hasData)
            {
                rowMin = minRow;
                colMin = minCol;
                rowMax = maxRow;
                colMax = maxCol;
                hasData = true;
                return;
            }
            rowMin = Math.Min(rowMin, minRow);
            colMin = Math.Min(colMin, minCol);
            rowMax = Math.Max(rowMax, maxRow);
            colMax = Math.Max(colMax, maxCol);
        }

        /// <summary>
        /// ViewROI 重绘回调，通知 ViewModel 补充绘制算法叠加对象。
        /// </summary>
        private void OnViewRedraw()
        {
            RaiseOverlay(window.HalconWindow);
        }

        /// <summary>
        /// 安全触发叠加绘制事件。
        /// </summary>
        private void RaiseOverlay(HWindow target)
        {
            Action<HWindow> handler = DrawOverlayRequested;
            if (handler != null) handler(target);
        }

        /// <summary>
        /// ViewROI ROI 变化回调，向 ViewModel 传递原始事件码。
        /// </summary>
        private void OnRoiChanged(int eventCode)
        {
            Action<int> handler = RoiChanged;
            if (handler != null) handler(eventCode);
        }

        /// <summary>
        /// 鼠标移动回调，更新状态栏坐标和灰度。
        /// </summary>
        private void OnMouseMove(object sender, HMouseEventArgs e)
        {
            Action<ReusableMeasureMouseInfo> handler = MouseInfoChanged;
            if (handler == null) return;
            handler(BuildMouseInfo(e));
        }

        /// <summary>
        /// 鼠标按下回调；仅右键用于位置跟随菜单。
        /// </summary>
        private void OnMouseDown(object sender, HMouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            Action<ReusableMeasureMouseInfo> handler = RightMouseDown;
            if (handler != null) handler(BuildMouseInfo(e));
        }

        /// <summary>
        /// 鼠标抬起回调；仅右键事件向外传递，供后续交互扩展。
        /// </summary>
        private void OnMouseUp(object sender, HMouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            Action<ReusableMeasureMouseInfo> handler = RightMouseUp;
            if (handler != null) handler(BuildMouseInfo(e));
        }

        /// <summary>
        /// 按 ViewROI 的窗口状态获取图像坐标，并在坐标落在图像内时读取灰度值。
        /// </summary>
        private ReusableMeasureMouseInfo BuildMouseInfo(HMouseEventArgs e)
        {
            double row = double.NaN;
            double col = double.NaN;
            string gray = "--";
            int imageWidth = 0;
            int imageHeight = 0;

            try
            {
                GetMousePositionFromViewRoi(e, out row, out col);

                if (HasValidImage(image))
                {
                    HOperatorSet.GetImageSize(image, out HTuple widthTuple, out HTuple heightTuple);
                    imageWidth = widthTuple.I;
                    imageHeight = heightTuple.I;
                    int rowIndex = (int)Math.Round(row);
                    int colIndex = (int)Math.Round(col);
                    if (rowIndex >= 0 && colIndex >= 0 && rowIndex < imageHeight && colIndex < imageWidth)
                    {
                        HObject grayReadImage = image;
                        HObject channelImage = null;
                        try
                        {
                            HOperatorSet.CountChannels(image, out HTuple channels);
                            if (channels.I > 1)
                            {
                                HOperatorSet.AccessChannel(image, out channelImage, 1);
                                grayReadImage = channelImage;
                            }
                            HOperatorSet.GetGrayval(grayReadImage, rowIndex, colIndex, out HTuple grayTuple);
                            gray = FormatGrayValue(grayTuple);
                        }
                        finally
                        {
                            if (channelImage != null) channelImage.Dispose();
                        }
                    }
                }
            }
            catch
            {
            }

            return new ReusableMeasureMouseInfo(row, col, gray, imageWidth, imageHeight);
        }

        /// <summary>
        /// 使用 HALCON GetMposition 获取 ViewROI 当前图像坐标；异常时回退到鼠标事件坐标。
        /// </summary>
        private void GetMousePositionFromViewRoi(HMouseEventArgs e, out double row, out double col)
        {
            try
            {
                HOperatorSet.GetMposition(window.HalconWindow, out HTuple rowTuple, out HTuple colTuple, out HTuple _);
                row = rowTuple.TupleInt().D;
                col = colTuple.TupleInt().D;
                return;
            }
            catch
            {
                row = e.Y;
                col = e.X;
            }
        }

        /// <summary>
        /// 将 HALCON 灰度 HTuple 转换为状态栏字符串，多通道值用逗号拼接。
        /// </summary>
        private static string FormatGrayValue(HTuple grayTuple)
        {
            if (grayTuple == null || grayTuple.Length <= 0) return "--";
            if (grayTuple.Length == 1) return grayTuple.D.ToString("F0", CultureInfo.InvariantCulture);

            List<string> values = new List<string>();
            for (int index = 0; index < grayTuple.Length; index++)
                values.Add(grayTuple[index].D.ToString("F0", CultureInfo.InvariantCulture));
            return string.Join(",", values.ToArray());
        }

        /// <summary>
        /// 校验 HObject 是否是已初始化的 HALCON 图像对象。
        /// </summary>
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

    /// <summary>
    /// 鼠标图像信息快照，供 ViewModel 更新底部状态栏和位置跟随菜单。
    /// </summary>
    public sealed class ReusableMeasureMouseInfo
    {
        public readonly double Row;
        public readonly double Col;
        public readonly string Gray;
        public readonly int ImageWidth;
        public readonly int ImageHeight;

        /// <summary>
        /// 创建鼠标信息，Row/Col 为图像坐标，Gray 为当前像素灰度显示文本。
        /// </summary>
        public ReusableMeasureMouseInfo(double row, double col, string gray, int imageWidth, int imageHeight)
        {
            Row = row;
            Col = col;
            Gray = gray ?? "--";
            ImageWidth = imageWidth;
            ImageHeight = imageHeight;
        }
    }
}


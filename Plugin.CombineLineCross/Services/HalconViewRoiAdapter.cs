using System;
using Frame_Core;
using Frame_Core.pluginParams;
using HalconDotNet;
using ViewROI;

namespace Plugin.CombineLineCross.Services
{
    /// <summary>
    /// ViewROI 适配器。
    /// 负责参数图像显示、本地图像加载、两条 ROILine、缩放/平移、事件转发和资源释放。
    /// 图像只使用 ParamsCombPoint.ho_Image，不在适配器内重复保存图像引用。
    /// </summary>
    public sealed class HalconViewRoiAdapter : IDisposable
    {
        public readonly HWindowControl window;

        private readonly HWndCtrl viewCtrl;
        private readonly ROIController roiController;
        private readonly IconicDelegate roiChangedHandler;
        private readonly HWndCtrl.ReDrawDelegate redrawHandler;
        private readonly HMouseEventHandler mouseMoveHandler;

        private ParamsCombPoint parameters;
        private bool synchronizing;
        private bool disposed;

        public ROILine ROILine1;
        public ROILine ROILine2;

        /// <summary>ROI 拖动、选择、松开时通知 ViewModel。</summary>
        public event Action<int> RoiChanged;

        /// <summary>鼠标坐标使用图像坐标：Row、Column。</summary>
        public event Action<double, double> MouseMoved;

        public HalconViewRoiAdapter(HWindowControl windowControl, bool enableRoi)
        {
            window = windowControl ?? throw new ArgumentNullException(nameof(windowControl));
            roiController = new ROIController();
            viewCtrl = new HWndCtrl(window);
            // HWndCtrl 可能携带旧的显示缓存，初始化时先彻底清空。
            viewCtrl.ho_Image?.Dispose();
            viewCtrl.ho_Image_Changed?.Dispose();
            viewCtrl.ho_Image_Changed = null;
            viewCtrl.ho_Image = null;
            viewCtrl.useROIController(roiController);
            viewCtrl.setDispLevel(HWndCtrl.MODE_INCLUDE_ROI);
            viewCtrl.setViewState(HWndCtrl.MODE_VIEW_MOVE);
            viewCtrl.bl_RoiOperateFlag = enableRoi;
            roiController.setDrawColor("yellow", "red", "gray");

            roiChangedHandler = OnViewRoiChanged;
            redrawHandler = OnViewRedraw;
            mouseMoveHandler = OnMouseMove;
            roiController.NotifyRCObserver += roiChangedHandler;
            viewCtrl.ReDrawEvent += redrawHandler;
            window.HMouseMove += mouseMoveHandler;
        }

        public bool IsDisposed { get { return disposed; } }

        /// <summary>
        /// 绑定插件参数。同步两条线，并直接显示参数对象持有的图像。
        /// </summary>
        public void SetBinding(ParamsCombPoint input)
        {
            if (disposed || !HasValidParameters(input)) return;
            parameters = input;
            FitImage();
            SynchronizeRoisFromParameters();
        }

        /// <summary>
        /// 供“加载本地图像”按钮调用。新图直接替换 ParamsCombPoint.ho_Image。
        /// </summary>
        public bool OpenLocalImage()
        {
            if (disposed) return false;
            using (System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog())
            {
                dialog.Filter = "图像文件|*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff|所有文件|*.*";
                if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) return false;
                return LoadLocalImage(dialog.FileName);
            }
        }

        /// <summary>
        /// 加载指定文件。新图校验通过后，释放旧参数图像并替换为新图。
        /// </summary>
        public bool LoadLocalImage(string filePath)
        {
            if (disposed || parameters == null || string.IsNullOrWhiteSpace(filePath)) return false;

            HImage loaded = null;
            try
            {
                loaded = new HImage(filePath);
                if (!IsValidImage(loaded))
                {
                    loaded.Dispose();
                    return false;
                }

                if (parameters.ho_Image != null) parameters.ho_Image.Dispose();
                parameters.ho_Image = loaded;
                loaded = null;
                FitImage();
                return true;
            }
            catch
            {
                if (loaded != null) loaded.Dispose();
                return false;
            }
        }

        /// <summary>让参数对象中的当前图像按窗口大小重新适应显示。</summary>
        public void FitImage()
        {
            if (disposed || parameters == null || !IsValidImage(parameters.ho_Image) ||
                window.Width <= 0 || window.Height <= 0) return;

            viewCtrl.addIconicVar(parameters.ho_Image);
            viewCtrl.resetAll();
            viewCtrl.repaint();
        }

        public void Refresh()
        {
            if (disposed) return;
            viewCtrl.repaint();
        }

        /// <summary>开启/关闭拖动，但关闭时仍显示两条线。</summary>
        public void SetRoiEditing(bool enabled)
        {
            if (disposed) return;
            viewCtrl.bl_RoiOperateFlag = enabled;
            Refresh();
        }

        public void SetRoiVisible(bool visible)
        {
            if (disposed) return;
            viewCtrl.setDispLevel(visible ? HWndCtrl.MODE_INCLUDE_ROI : HWndCtrl.MODE_EXCLUDE_ROI);
            Refresh();
        }

        public void SetViewState(int mode)
        {
            if (disposed) return;
            viewCtrl.setViewState(mode);
            Refresh();
        }

        /// <summary>用户拖动 ROI 后调用，回写既有 LineInfo，不创建任何参数对象。</summary>
        public void SynchronizeParametersFromRois()
        {
            if (disposed) return;
            if (parameters == null || ROILine1 == null || ROILine2 == null) return;

            CopyLine(ROILine1, parameters.LineInfo1);
            CopyLine(ROILine2, parameters.LineInfo2);
        }

        private void SynchronizeRoisFromParameters()
        {
            synchronizing = true;
            try
            {
                EnsureTwoLines();
                ROILine1.SetEndpoints(parameters.LineInfo1.RowBegin, parameters.LineInfo1.ColBegin,
                                      parameters.LineInfo1.RowEnd, parameters.LineInfo1.ColEnd);
                ROILine2.SetEndpoints(parameters.LineInfo2.RowBegin, parameters.LineInfo2.ColBegin,
                                      parameters.LineInfo2.RowEnd, parameters.LineInfo2.ColEnd);
                roiController.setActiveROIIdx(-1);
            }
            finally
            {
                synchronizing = false;
            }
            Refresh();
        }

        private void EnsureTwoLines()
        {
            if (ROILine1 != null && ROILine2 != null && roiController.ROIList.Count == 2 &&
                ReferenceEquals(roiController.ROIList[0], ROILine1) &&
                ReferenceEquals(roiController.ROIList[1], ROILine2)) return;

            DisposeLines();
            DisposeModelRegion();
            roiController.reset();
            ROILine1 = new ROILine();
            ROILine2 = new ROILine();
            roiController.ROIList.Add(ROILine1);
            roiController.ROIList.Add(ROILine2);
        }

        private void OnViewRoiChanged(int eventCode)
        {
            if (disposed || synchronizing) return;

            if (eventCode == ROIController.EVENT_ACTIVATED_ROI ||
                eventCode == ROIController.EVENT_MOVING_ROI ||
                eventCode == ROIController.EVENT_UPDATE_ROI)
            {
                SynchronizeParametersFromRois();
            }

            Action<int> handler = RoiChanged;
            if (handler != null) handler(eventCode);
        }

        /// <summary>
        /// ViewROI 每次完成底图和 ROI 绘制后，在此补画已有计算结果。
        /// 回调中只绘制，不调用 repaint 或 Refresh，避免绘制重入。
        /// </summary>
        private void OnViewRedraw()
        {
            if (disposed) return;
            DrawIntersection();
        }

        /// <summary>只绘制参数中已有的交点结果，不修改参数、不触发刷新。</summary>
        private void DrawIntersection()
        {
            if (disposed || parameters == null || !parameters.m_bRunSuccess ||
                parameters.hv_IsParallel == null || parameters.hv_IsParallel.Length == 0 ||
                parameters.hv_IsParallel[0].I != 0 ||
                parameters.hv_IntersectionRow == null || parameters.hv_IntersectionRow.Length == 0 ||
                parameters.hv_IntersectionColumn == null || parameters.hv_IntersectionColumn.Length == 0)
            {
                return;
            }

            HWindow halconWindow = window.HalconWindow;
            halconWindow.SetColor("red");
            halconWindow.SetLineWidth(2);
            halconWindow.DispCross(parameters.hv_IntersectionRow[0].D,
                                            parameters.hv_IntersectionColumn[0].D,
                                            30.0,
                                            0.0);
            HOperatorSet.GenRegionLine(out HObject line1, parameters.LineInfo1.RowBegin, parameters.LineInfo1.ColBegin, parameters.LineInfo1.RowEnd, parameters.LineInfo1.ColEnd);
            HOperatorSet.GenRegionLine(out HObject line2, parameters.LineInfo2.RowBegin, parameters.LineInfo2.ColBegin, parameters.LineInfo2.RowEnd, parameters.LineInfo2.ColEnd);
            halconWindow.SetLineWidth(2);
            halconWindow.SetColor("yellow");
            halconWindow.DispObj(line1);
            halconWindow.SetLineWidth(2);
            halconWindow.SetColor("orange");
            halconWindow.DispObj(line2);
            line1?.Dispose();
            line2?.Dispose();
            halconWindow.SetLineWidth(1);

        }

        private void OnMouseMove(object sender, HMouseEventArgs e)
        {
            if (disposed) return;
            Action<double, double> handler = MouseMoved;
            if (handler != null) handler(e.Y, e.X);
        }

        private static void CopyLine(ROILine roi, LineInfo target)
        {
            if (roi == null || target == null) return;

            HTuple data = roi.getModelData();
            try
            {
                target.RowBegin = data[0].D;
                target.ColBegin = data[1].D;
                target.RowEnd = data[2].D;
                target.ColEnd = data[3].D;
            }
            finally
            {
                data.Dispose();
            }
        }

        private static bool IsValidImage(HObject image)
        {
            if (image == null) return false;
            HTuple width = null;
            HTuple height = null;
            try
            {
                HOperatorSet.GetImageSize(image, out width, out height);
                return width.Length > 0 && height.Length > 0 && width[0].D > 0 && height[0].D > 0;
            }
            catch
            {
                return false;
            }
            finally
            {
                if (width != null) width.Dispose();
                if (height != null) height.Dispose();
            }
        }

        private static bool HasValidParameters(ParamsCombPoint input)
        {
            return input != null && input.LineInfo1 != null && input.LineInfo2 != null;
        }

        private void DisposeLines()
        {
            if (ROILine1 != null) ROILine1.Dispose();
            if (ROILine2 != null && !ReferenceEquals(ROILine1, ROILine2)) ROILine2.Dispose();
        }

        private void DisposeModelRegion()
        {
            if (roiController.ModelROI != null)
            {
                roiController.ModelROI.Dispose();
                roiController.ModelROI = null;
            }
        }

        public void Dispose()
        {
            if (disposed) return;
            disposed = true;

            // 对称解绑构造函数中注册的所有事件。
            roiController.NotifyRCObserver -= roiChangedHandler;
            viewCtrl.ReDrawEvent -= redrawHandler;
            window.HMouseMove -= mouseMoveHandler;

            DisposeLines();
            DisposeModelRegion();
            roiController.ROIList.Clear();
            roiController.resetROI();
            viewCtrl.Dispose();

            ROILine1 = null;
            ROILine2 = null;
            parameters = null;
            RoiChanged = null;
            MouseMoved = null;
        }
    }
}

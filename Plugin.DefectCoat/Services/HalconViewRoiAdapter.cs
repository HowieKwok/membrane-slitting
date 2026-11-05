using Plugin.DefectCoat.Models;
using HalconDotNet;
using System;
using System.Reflection;
using ViewROI;
using Frame_Core.pluginParams;

namespace Plugin.DefectCoat.Services
{
    /// <summary>
    /// 主 HALCON 图像窗口和 ViewROI 的适配器。
    /// 本类负责图像显示、Rectangle1 ROI 读写、运行结果叠加和鼠标灰度信息；
    /// 传入的图像、ROI Region、结果 Region 都只做显示引用，不在本类中 Dispose。
    /// </summary>
    public sealed class HalconViewRoiAdapter : IDisposable
    {
        /// <summary>主图 HALCON 控件，由 WPF WindowsFormsHost 承载。</summary>
        private readonly HWindowControl window;

        /// <summary>ViewROI 窗口控制器，负责缩放、平移、ROI 绘制和重绘事件。</summary>
        private readonly HWndCtrl viewCtrl;

        /// <summary>ViewROI ROI 控制器，内部维护 ROIList 和当前活动 ROI。</summary>
        private readonly ROIController roiController;

        /// <summary>重绘事件委托，保存字段用于 Dispose 时退订。</summary>
        private readonly HWndCtrl.ReDrawDelegate redrawHandler;

        /// <summary>ROI 变化事件委托，保存字段用于 Dispose 时退订。</summary>
        private readonly IconicDelegate roiChangedHandler;

        /// <summary>鼠标移动事件委托，保存字段用于 Dispose 时退订。</summary>
        private readonly HMouseEventHandler mouseMoveHandler;

        /// <summary>当前主图引用。该对象来自 Params.ho_Image，本类不拥有释放权。</summary>
        private HObject image;

        /// <summary>实际检测 ROI Region 引用，用于蓝色叠加显示；释放权属于 Params/检测结果。</summary>
        private HObject roiRegion;

        /// <summary>屏蔽 ROI 合并 Region 引用，用于橙色叠加显示；释放权属于 Params/检测结果。</summary>
        private HObject maskRegion;

        /// <summary>候选结果 Region 引用，用于红色叠加显示；释放权属于 Params/检测结果。</summary>
        private HObject resultRegion;

        /// <summary>当前检测 ROI 扣除屏蔽区后的灰度均值，仅用于鼠标状态栏显示。</summary>
        private HTuple grayMean=0;

        /// <summary>释放标记，防止重复退订事件。</summary>
        private bool disposed;

        /// <summary>
        /// 创建主图窗口适配器并启用 ViewROI 操作。
        /// 构造时只创建控制器和事件绑定，不读取图像，也不改变业务参数。
        /// </summary>
        public HalconViewRoiAdapter(HWindowControl windowControl)
        {
            window = windowControl ?? throw new ArgumentNullException(nameof(windowControl));
            roiController = new ROIController();
            viewCtrl = new HWndCtrl(window);
            viewCtrl.ho_Image?.Dispose();
            viewCtrl.ho_Image_Changed?.Dispose();
            viewCtrl.ho_Image = null;
            viewCtrl.ho_Image_Changed = null;
            viewCtrl.useROIController(roiController);
            viewCtrl.setDispLevel(HWndCtrl.MODE_INCLUDE_ROI);
            viewCtrl.setViewState(HWndCtrl.MODE_VIEW_MOVE);
            viewCtrl.bl_RoiOperateFlag = true;
            roiController.setDrawColor("yellow", "red", "gray");

            redrawHandler = OnRedraw;
            roiChangedHandler = OnRoiChanged;
            mouseMoveHandler = OnMouseMove;
            viewCtrl.ReDrawEvent += redrawHandler;
            roiController.NotifyRCObserver += roiChangedHandler;
            window.HMouseMove += mouseMoveHandler;
        }

        /// <summary>ROI 被创建、拖动或更新时触发，ViewModel 负责把 Rectangle1 四坐标写回参数。</summary>
        public event Action RoiChanged;

        /// <summary>鼠标移动时触发，向 ViewModel 输出 Row/Col/Gray 和灰度均值文本。</summary>
        public event Action<HalconMouseInfoArgs> MouseMoved;

        /// <summary>暴露底层 HALCON 控件，供极少数外层需要访问控件对象的场景使用。</summary>
        public HWindowControl Window => window;

        /// <summary>
        /// 根据参数刷新主图和当前 ViewROI。
        /// 当前实现优先使用 Params.LocalRoi 的 Row1/Col1/Row2/Col2 创建 Rectangle1。
        /// </summary>
        public void RefreshFromParams(ParamsDefectCoat parameters, bool fit)
        {
            if (disposed || parameters == null) return;
            SetViewRoiVisible(parameters.ShowViewRoi);
            if (parameters.ShowViewRoi)
                EnsureRectangle(parameters);
            ShowImage(parameters.ho_Image, fit);
        }

        /// <summary>显示主图。传入图像只保存引用，不 CopyObj，不 Dispose。</summary>
        public void ShowImage(HObject hoImage, bool fit)
        {
            if (disposed) return;
            image = hoImage;
            if (HasObject(image))
            {
              
                viewCtrl.addIconicVar(image);
                viewCtrl.resetflag = fit;
                viewCtrl.resetAll();
                viewCtrl.repaint();
                return;
            }

            ClearImage();
        }

        /// <summary>清空显示引用和窗口内容，不释放外部图像或结果 Region。</summary>
        public void ClearImage()
        {
            if (disposed) return;
            image = null;
            roiRegion = null;
            maskRegion = null;
            resultRegion = null;
            viewCtrl.ho_Image = null;
            viewCtrl.ho_Image_Changed = null;
            try
            {
                window.HalconWindow.ClearWindow();
            }
            catch
            {
            }
        }

        /// <summary>兼容旧调用：只显示检测 ROI 和结果 Region，不显示屏蔽 ROI。</summary>
        public void ShowRunResult(HObject roi, HObject result)
        {
            ShowRunResult(roi, null, result);
        }

        /// <summary>设置运行后叠加显示的检测 ROI、屏蔽 ROI 和结果 Region。只保存显示引用。</summary>
        public void ShowRunResult(HObject roi, HObject mask, HObject result)
        {
            roiRegion = roi;
            maskRegion = mask;
            resultRegion = result;
            Repaint();
        }

        /// <summary>清空运行结果叠加显示，不释放外部 HObject。</summary>
        public void ClearRunResult()
        {
            roiRegion = null;
            maskRegion = null;
            resultRegion = null;
            Repaint();
        }

        /// <summary>
        /// 把当前活动 ViewROI 同步为 Rectangle1 四坐标。
        /// row/col 坐标单位为像素；如果当前没有 ROI，则创建一个 ROIRectangle1。
        /// </summary>
        public void SyncActiveRectangle1(double row1, double col1, double row2, double col2)
        {
            if (disposed) return;
            ROIRectangle1 roi = GetActiveRectangle();
            if (roi == null)
            {
                roiController.reset();
                roi = new ROIRectangle1();
                roi.createROI((col1 + col2) / 2.0, (row1 + row2) / 2.0);
                roiController.setROIShape(roi);
                roiController.ROIList.Add(roi);
                roiController.activeROIidx = 0;
            }

            SetRectangle1Data(roi, row1, col1, row2, col2);
            Repaint();
        }

        /// <summary>
        /// 读取当前活动 ROIRectangle1 的四坐标。
        /// 返回 false 表示当前没有可用 Rectangle1；不会修改任何参数对象。
        /// </summary>
        public bool TryGetActiveRectangle1(out double row1, out double col1, out double row2, out double col2)
        {
            row1 = col1 = row2 = col2 = 0;
            ROIRectangle1 roi = GetActiveRectangle();
            if (roi == null) return false;

            HTuple data = roi.getModelData();
            if (data == null || data.Length < 4) return false;

            row1 = data[0].D;
            col1 = data[1].D;
            row2 = data[2].D;
            col2 = data[3].D;
            return true;
        }

        /// <summary>设置 ViewROI 框是否显示和响应拖拽；不影响已经保存的 ROI 四坐标。</summary>
        public void SetViewRoiVisible(bool visible)
        {
            if (disposed) return;
            viewCtrl.bl_RoiOperateFlag = visible;
            viewCtrl.setDispLevel(visible ? HWndCtrl.MODE_INCLUDE_ROI : HWndCtrl.MODE_EXCLUDE_ROI);
            Repaint();
        }

        /// <summary>请求 ViewROI 控制器重绘主图、ROI 和业务叠加区域。</summary>
        public void Repaint()
        {
            if (disposed) return;
            viewCtrl.repaint();
        }

        /// <summary>退订 ViewROI 和鼠标事件。本类没有外部 HObject 所有权，因此不释放 image/region 字段。</summary>
        public void Dispose()
        {
            if (disposed) return;
            disposed = true;
            try
            {
                viewCtrl.ReDrawEvent -= redrawHandler;
                roiController.NotifyRCObserver -= roiChangedHandler;
                window.HMouseMove -= mouseMoveHandler;
            }
            catch
            {
            }
        }

        /// <summary>确保主窗口存在一个 Rectangle1 ROI，并用参数中的四坐标刷新它。</summary>
        private void EnsureRectangle(ParamsDefectCoat parameters)
        {
            CoatRoiParam localRoi = parameters.LocalRoi ?? new CoatRoiParam { Row1 = 140, Col1 = 90, Row2 = 460, Col2 = 510 };
            double row1 = localRoi.Row1;
            double col1 = localRoi.Col1;
            double row2 = localRoi.Row2;
            double col2 = localRoi.Col2;

            ROIRectangle1 roi = GetActiveRectangle();
            if (roi == null)
            {
                roiController.reset();
                roi = new ROIRectangle1();
                roi.createROI((col1 + col2) / 2.0, (row1 + row2) / 2.0);
                roiController.setROIShape(roi);
                roiController.ROIList.Add(roi);
                roiController.activeROIidx = 0;
            }

            SetRectangle1Data(roi, row1, col1, row2, col2);
        }

        /// <summary>获取当前活动 Rectangle1；若活动 ROI 不是 Rectangle1，则回退到 ROIList 第一个 Rectangle1。</summary>
        private ROIRectangle1 GetActiveRectangle()
        {
            if (roiController.ROIList == null || roiController.ROIList.Count == 0) return null;
            ROIRectangle1 active = roiController.getActiveROI() as ROIRectangle1;
            return active ?? roiController.ROIList[0] as ROIRectangle1;
        }

        /// <summary>
        /// ViewROI 通知 ROI 变化时触发。
        /// 这里顺便计算检测 ROI 扣除屏蔽 ROI 后的灰度均值，真正的参数回写交给 ViewModel 处理。
        /// </summary>
        private void OnRoiChanged(int value)
        {
            if (HasObject(image))
            {
                HOperatorSet.GenEmptyObj(out HObject tempRegion);
                if(maskRegion!=null)
                {
                    tempRegion = maskRegion.CopyObj(1,-1);

                }
                if(roiRegion!=null)
                {
                    HOperatorSet.Difference(roiRegion, tempRegion, out HObject regionDifference);
                    HOperatorSet.Intensity(regionDifference, image, out grayMean, out HTuple deviation);
                    regionDifference?.Dispose();
                }

                tempRegion?.Dispose();
            }
            RoiChanged?.Invoke();
        }

        /// <summary>ViewROI 重绘后补画检测 ROI、屏蔽 ROI 和结果 Region。</summary>
        private void OnRedraw()
        {
            try
            {
                if (HasObject(roiRegion))
                {
                    window.HalconWindow.SetColor("blue");
                    window.HalconWindow.SetDraw("margin");
                    window.HalconWindow.DispObj(roiRegion);
                }
                if (HasObject(maskRegion))
                {
                    window.HalconWindow.SetColor("orange");
                    window.HalconWindow.SetDraw("margin");
                    window.HalconWindow.DispObj(maskRegion);
                }
                if (HasObject(resultRegion))
                {
                    window.HalconWindow.SetColor("red");
                    window.HalconWindow.SetDraw("margin");
                    window.HalconWindow.DispObj(resultRegion);
                }
            }
            catch
            {
            }
        }

        /// <summary>鼠标移动时输出坐标、灰度和当前 ROI 灰度均值。</summary>
        private void OnMouseMove(object sender, HMouseEventArgs e)
        {
            MouseMoved?.Invoke(new HalconMouseInfoArgs
            {
                Row = e.Y,
                Column = e.X,
                Text = BuildMouseText(e.Y, e.X) + string.Format("_灰度均值:{0}", grayMean.D.ToString("0.0"))
            });
        }

        /// <summary>构造主图窗口状态栏文本。多通道图像只读取第 1 通道灰度。</summary>
        private string BuildMouseText(double rowValue, double colValue)
        {
            if (!HasObject(image)) return "图像:-  行:--  列:--  灰度:-";
            try
            {
                int row = (int)Math.Round(rowValue);
                int col = (int)Math.Round(colValue);
                HOperatorSet.GetImageSize(image, out HTuple width, out HTuple height);
                if (row < 0 || col < 0 || row >= height.I || col >= width.I)
                    return "图像:-  行:--  列:--  灰度:-";

                HObject read = image;
                HObject channel = null;
                try
                {
                    HOperatorSet.CountChannels(image, out HTuple channels);
                    if (channels.I > 1)
                    {
                        HOperatorSet.AccessChannel(image, out channel, 1);
                        read = channel;
                    }
                    HOperatorSet.GetGrayval(read, row, col, out HTuple gray);
                    return string.Format("图像:-  行:{0}  列:{1}  灰度:{2}", row, col, gray);
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

        /// <summary>判断 HALCON 对象是否已初始化且包含对象，避免空对象显示异常。</summary>
        private static bool HasObject(HObject obj)
        {
            if (obj == null || !obj.IsInitialized()) return false;
            try
            {
                HOperatorSet.CountObj(obj, out HTuple count);
                return count.I > 0;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 通过反射写入 ROIRectangle1 的私有坐标字段。
        /// ViewROI 原类未暴露 setter，因此适配层统一在这里写入并保持 row/col 顺序归一化。
        /// </summary>
        private static void SetRectangle1Data(ROIRectangle1 roi, double row1, double col1, double row2, double col2)
        {
            if (roi == null) return;
            double minRow = Math.Min(row1, row2);
            double maxRow = Math.Max(row1, row2);
            double minCol = Math.Min(col1, col2);
            double maxCol = Math.Max(col1, col2);
            SetPrivateDouble(roi, "row1", minRow);
            SetPrivateDouble(roi, "col1", minCol);
            SetPrivateDouble(roi, "row2", maxRow);
            SetPrivateDouble(roi, "col2", maxCol);
            SetPrivateDouble(roi, "midR", (minRow + maxRow) / 2.0);
            SetPrivateDouble(roi, "midC", (minCol + maxCol) / 2.0);
        }

        /// <summary>反射设置 ViewROI 对象的 private double 字段；字段不存在时静默跳过。</summary>
        private static void SetPrivateDouble(object target, string fieldName, double value)
        {
            FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            if (field != null)
                field.SetValue(target, value);
        }
    }
}

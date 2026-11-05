using HalconDotNet;
using System;
using System.Globalization;
using ViewROI;

namespace Plugin.CACRunCalibration.Services
{
    /// <summary>
    /// Halcon 图像窗口与 ROI 交互适配器。
    /// 封装 HWindowControl、视图控制器和 ROI 控制器，提供图像显示、ROI 创建/编辑/读取、
    /// 以及鼠标灰度值读取等功能。适用于标定插件的图像预览和区域选择。
    /// </summary>
    public sealed class HalconViewRoiAdapter : IDisposable
    {
        // ---------- 底层 Halcon 与 ROI 组件 ----------
        private readonly HWindowControl window;          // Halcon 原生的 Windows Forms 图像窗口控件
        private readonly HWndCtrl viewCtrl;              // 视图控制器，管理图像显示、平移、缩放、ROI 叠加
        private readonly ROIController roiController;    // ROI 控制器，管理多个 ROI 对象的创建、编辑、选择

        // ---------- 图像缓存 ----------
        private HObject image;                           // 最近一次 ShowImage 传入的原始图像（可能多通道）
        private HImage displayImage;                     // 实际用于显示的图像（单通道，通过指针快速获取）

        // ---------- 事件订阅句柄（用于取消订阅） ----------
        private HWndCtrl.ReDrawDelegate redrawHandler;   // 视图重绘时的回调
        private IconicDelegate roiChangedHandler;        // ROI 变化（移动/尺寸等）时的回调

        // ---------- 释放标志 ----------
        private bool disposed;

        /// <summary>
        /// ROI 发生变化时触发（参数含义由 ROIController 约定）。
        /// </summary>
        public event Action<int> RoiChanged;

        /// <summary>
        /// 视图重绘时触发（无参数，通知外部更新叠加层等）。
        /// </summary>
        public event Action Redraw;

        /// <summary>
        /// 初始化适配器，绑定窗口、视图控制器和 ROI 控制器。
        /// </summary>
        /// <param name="windowControl">Halcon 的图像显示窗口控件</param>
        /// <param name="enableRoi">是否允许 ROI 交互操作</param>
        public HalconViewRoiAdapter(HWindowControl windowControl, bool enableRoi)
        {
            if (windowControl == null) throw new ArgumentNullException("windowControl");

            window = windowControl;
            roiController = new ROIController();
            viewCtrl = new HWndCtrl(window);

            // 将 ROI 控制器关联到视图控制器，并设置显示模式
            viewCtrl.useROIController(roiController);
            viewCtrl.setDispLevel(HWndCtrl.MODE_INCLUDE_ROI);   // 显示图像及 ROI
            viewCtrl.setViewState(HWndCtrl.MODE_VIEW_MOVE);     // 默认视图模式为移动（可平移/缩放）
            
            viewCtrl.ho_Image?.Dispose();
            viewCtrl.ho_Image_Changed?.Dispose();
            viewCtrl.ho_Image = null;
            viewCtrl.ho_Image_Changed = null;

            viewCtrl.bl_RoiOperateFlag = enableRoi;             // 控制 ROI 是否可被鼠标操作

            // 设置 ROI 绘制颜色：黄色（选中）、红色（一般）、灰色（参考线）
            roiController.setDrawColor("yellow", "red", "gray");

            // 订阅内部事件，并转发到外部公开事件
            redrawHandler = OnViewRoiRedraw;
            roiChangedHandler = OnViewRoiChanged;
            viewCtrl.ReDrawEvent += redrawHandler;
            roiController.NotifyRCObserver += roiChangedHandler;
        }

        // ---------- 公开属性 ----------
        /// <summary>
        /// 获取 Halcon 窗口句柄，用于直接调用 Halcon 绘图操作。
        /// </summary>
        public HWindow HalconWindow { get { return window.HalconWindow; } }

        /// <summary>
        /// 获取视图控制器，用于高级视图操作（如设置显示模式）。
        /// </summary>
        public HWndCtrl ViewCtrl { get { return viewCtrl; } }

        /// <summary>
        /// 获取 ROI 控制器，用于直接操作 ROI 列表。
        /// </summary>
        public ROIController RoiController { get { return roiController; } }

        // ---------- ROI 与图像操作 ----------

        /// <summary>
        /// 启用/禁用 ROI 的鼠标交互（移动、缩放、创建等）。
        /// </summary>
        public void SetRoiOperate(bool enabled)
        {
            if (disposed) return;
            viewCtrl.bl_RoiOperateFlag = enabled;
        }

        /// <summary>
        /// 显示图像，可选择是否自适应窗口大小。
        /// </summary>
        /// <param name="hoImage">待显示的 Halcon 图像</param>
        /// <param name="fit">true 重置视图并适应窗口；false 保持当前视图参数并仅刷新显示图像</param>
        public void ShowImage(HObject hoImage, bool fit)
        {
            if (disposed) return;
            image = hoImage;

            if (fit)
            {
                ResetAndFitImage();   // 重置所有视图状态并自适应
                return;
            }

            // 非自适应模式：仅更新显示图像，保留当前 ROI 和视图状态
            RefreshDisplayImage();
            if (displayImage == null || !displayImage.IsInitialized()) return;
            viewCtrl.addIconicVar(displayImage);
            viewCtrl.repaint();
        }

        /// <summary>
        /// 自适应窗口：若图像已加载则重置视图以完整显示图像；否则仅刷新窗口并重置视图。
        /// </summary>
        public void FitImage()
        {
            if (disposed) return;
            if (image != null && image.IsInitialized())
            {
                ResetAndFitImage();
                return;
            }

            ForceWindowSizeRefresh();
            viewCtrl.resetAll();
            viewCtrl.repaint();
        }

        /// <summary>
        /// 清除所有 ROI 对象，并重绘窗口。
        /// </summary>
        public void ClearRoi()
        {
            if (disposed) return;
            roiController.reset();
            viewCtrl.repaint();
        }

        /// <summary>
        /// 确保窗口中存在一个矩形 ROI，若不存在则创建。
        /// 矩形坐标由 row1, col1, row2, col2 定义（内部会调整为左上/右下顺序）。
        /// 此方法使用反射直接设置 ROIRectangle1 的内部字段，以绕过其 createROI 的参数限制。
        /// </summary>
        /// <param name="row1">矩形上边缘行坐标</param>
        /// <param name="col1">矩形左边缘列坐标</param>
        /// <param name="row2">矩形下边缘行坐标</param>
        /// <param name="col2">矩形右边缘列坐标</param>
        public void EnsureRectangleRoi(double row1, double col1, double row2, double col2)
        {
            if (disposed) return;

            // 清空已有 ROI，重新创建
            roiController.reset();
            ROIRectangle1 roi = new ROIRectangle1();
            // createROI 的参数通常为 (col, row)，这里取中心点创建初始矩形
            roi.createROI((col1 + col2) / 2.0, (row1 + row2) / 2.0);

            // 通过反射设置矩形的实际边界，确保精确匹配传入坐标
            SetPrivateDouble(roi, "row1", Math.Min(row1, row2));
            SetPrivateDouble(roi, "col1", Math.Min(col1, col2));
            SetPrivateDouble(roi, "row2", Math.Max(row1, row2));
            SetPrivateDouble(roi, "col2", Math.Max(col1, col2));
            SetPrivateDouble(roi, "midR", (row1 + row2) / 2.0);
            SetPrivateDouble(roi, "midC", (col1 + col2) / 2.0);

            roiController.ROIList.Add(roi);
            roiController.activeROIidx = 0;   // 设为新添加的 ROI 为当前活跃
            viewCtrl.repaint();
        }

        /// <summary>
        /// 尝试获取当前活跃矩形 ROI 的坐标。
        /// 若存在矩形 ROI 则输出 (row1, col1, row2, col2)，否则返回 false。
        /// </summary>
        public bool TryGetRectangleRoi(out double row1, out double col1, out double row2, out double col2)
        {
            row1 = 0; col1 = 0; row2 = 0; col2 = 0;

            ROI roi = roiController.getActiveROI();
            if (roi == null && roiController.ROIList.Count > 0) roi = roiController.ROIList[0] as ROI;
            if (roi == null) return false;

            HTuple data = roi.getModelData();
            if (data == null || data.Length < 4) return false;

            // ROIRectangle1 的模型数据顺序：[row1, col1, row2, col2]
            row1 = data[0].D;
            col1 = data[1].D;
            row2 = data[2].D;
            col2 = data[3].D;
            return true;
        }

        /// <summary>
        /// 根据鼠标事件获取当前位置的行、列和灰度值（用于状态栏提示）。
        /// 若图像无效或鼠标超出图像范围，返回占位文本。
        /// 支持多通道图像：自动取第一通道读取灰度。
        /// </summary>
        public string GetMouseText(HMouseEventArgs e)
        {
            if (image == null || !image.IsInitialized()) return "Row:-  Col:-  Gray:-";
            try
            {
                int row = (int)Math.Round(e.Y);
                int col = (int)Math.Round(e.X);
                HOperatorSet.GetImageSize(image, out HTuple width, out HTuple height);
                if (row < 0 || col < 0 || row >= height.I || col >= width.I) return "Row:-  Col:-  Gray:-";

                HObject readImage = image;
                HObject channel = null;
                try
                {
                    HOperatorSet.CountChannels(image, out HTuple channels);
                    if (channels.I > 1)
                    {
                        // 多通道图像，提取第一个通道用于读取灰度
                        HOperatorSet.AccessChannel(image, out channel, 1);
                        readImage = channel;
                    }

                    HOperatorSet.GetGrayval(readImage, row, col, out HTuple gray);
                    return string.Format(CultureInfo.InvariantCulture, "Row:{0}  Col:{1}  Gray:{2}", row, col, gray.ToString());
                }
                finally
                {
                    if (channel != null) channel.Dispose();
                }
            }
            catch
            {
                return "Row:-  Col:-  Gray:-";
            }
        }

        /// <summary>
        /// 强制重绘窗口。
        /// </summary>
        public void Repaint()
        {
            if (disposed) return;
            viewCtrl.repaint();
        }

        // ---------- IDisposable 实现 ----------
        public void Dispose()
        {
            if (disposed) return;
            disposed = true;

            try
            {
                if (redrawHandler != null) viewCtrl.ReDrawEvent -= redrawHandler;
                if (roiChangedHandler != null) roiController.NotifyRCObserver -= roiChangedHandler;
            }
            catch { }

            try { roiController.reset(); } catch { }
            try
            {
                viewCtrl.ho_Image = null;
                viewCtrl.ho_Image_Changed = null;
            }
            catch { }

            if (displayImage != null) displayImage.Dispose();

            displayImage = null;
            image = null;
            redrawHandler = null;
            roiChangedHandler = null;
            RoiChanged = null;
            Redraw = null;
        }

        // ---------- 内部辅助方法 ----------

        /// <summary>
        /// 重置视图并自适应图像：强制窗口刷新、重建显示图像、重置视图状态、重新添加图像并重绘。
        /// </summary>
        private void ResetAndFitImage()
        {
            ForceWindowSizeRefresh();
            RefreshDisplayImage();
            if (displayImage == null || !displayImage.IsInitialized()) return;

            viewCtrl.resetAll();
            viewCtrl.resetflag = true;
            viewCtrl.addIconicVar(displayImage);
            viewCtrl.repaint();
        }

        /// <summary>
        /// 重新生成用于显示的灰度图像（单通道，通过指针高效共享内存）。
        /// 若原图为多通道，先转换为灰度图。
        /// 生成的 displayImage 类型为 HImage，便于 Halcon 内部渲染。
        /// </summary>
        private void RefreshDisplayImage()
        {
            if (displayImage != null) displayImage.Dispose();
            displayImage = null;
            if (image == null || !image.IsInitialized()) return;

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

                // 获取图像指针，以零拷贝方式创建 HImage 对象（高效）
                HOperatorSet.GetImagePointer1(displaySource, out HTuple pointer, out HTuple type, out HTuple width, out HTuple height);
                displayImage = new HImage();
                displayImage.GenImage1(type, width, height, pointer);
            }
            finally
            {
                if (grayImage != null) grayImage.Dispose();
            }
        }

        /// <summary>
        /// 强制刷新窗口及其父容器布局，确保绘制尺寸与实际一致。
        /// </summary>
        private void ForceWindowSizeRefresh()
        {
            if (window.Parent != null) window.Parent.Refresh();
            window.Refresh();
        }

        // ---------- 内部事件转发 ----------

        /// <summary>
        /// 视图重绘事件处理，转发到外部 Redraw 事件。
        /// </summary>
        private void OnViewRoiRedraw()
        {
            Action handler = Redraw;
            if (handler != null) handler();
        }

        /// <summary>
        /// ROI 变化事件处理，转发到外部 RoiChanged 事件，参数为 ROI 索引。
        /// </summary>
        private void OnViewRoiChanged(int value)
        {
            Action<int> handler = RoiChanged;
            if (handler != null) handler(value);
        }

        /// <summary>
        /// 使用反射设置对象的私有实例字段（用于 ROIRectangle1 内部字段修改）。
        /// </summary>
        private static void SetPrivateDouble(object target, string fieldName, double value)
        {
            System.Reflection.FieldInfo field = target.GetType().GetField(fieldName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (field != null) field.SetValue(target, value);
        }
    }
}
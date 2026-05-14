using HalconDotNet;
using System;
using System.Globalization;
using System.Windows.Forms;
using ViewROI;

namespace Plugin.BlobAnalysis.Services
{
    /// <summary>
    /// ViewROI 显示适配器：只封装使用方式，不改 ViewROI 源码。
    /// 关键点：ViewROI.addIconicVar 只有收到 HImage 时才会计算图像宽高和自适应范围。
    /// </summary>
    public sealed class HalconViewRoiAdapter
    {
        private readonly HWindowControl window;
        private readonly HWndCtrl viewCtrl;
        private readonly ROIController roiController;
        private HObject image;
        private HImage displayImage;

        public event Action<int> RoiChanged;
        public event Action Redraw;

        public HalconViewRoiAdapter(HWindowControl windowControl, bool enableRoi)
        {
            window = windowControl ?? throw new ArgumentNullException(nameof(windowControl));
            roiController = new ROIController();
            viewCtrl = new HWndCtrl(window);
            // 把 ROIController 交给 HWndCtrl 管理，ViewROI 内部会负责鼠标拖动、缩放、ROI 激活等事件。
            viewCtrl.useROIController(roiController);
            // MODE_INCLUDE_ROI：重绘时同时显示图像和 ROI。否则只重绘图像，ROI 可能不显示。
            viewCtrl.setDispLevel(HWndCtrl.MODE_INCLUDE_ROI);
            // MODE_VIEW_MOVE：默认允许图像平移。ViewROI 内部仍会处理滚轮缩放。
            viewCtrl.setViewState(HWndCtrl.MODE_VIEW_MOVE);
            // bl_RoiOperateFlag 控制是否允许操作 ROI。false 时避免误拖 ROI，只保留图像浏览。
            viewCtrl.bl_RoiOperateFlag = enableRoi;
            roiController.setDrawColor("yellow", "red", "gray");
            // ReDrawEvent：ViewROI 自己重绘图像后通知外层补画业务图形，如基准点、跟随 ROI、文字。
            viewCtrl.ReDrawEvent += delegate { Redraw?.Invoke(); };
            roiController.NotifyRCObserver += delegate(int value)
            {
                RoiChanged?.Invoke(value);
            };
        }

        public HWindow HalconWindow => window.HalconWindow;
        public HWndCtrl ViewCtrl => viewCtrl;
        public ROIController RoiController => roiController;

        public void SetRoiOperate(bool enabled)
        {
            viewCtrl.bl_RoiOperateFlag = enabled;
            Repaint();
        }

        /// <summary>
        /// 显示图像。注意必须把 HObject 转成 HImage 再 addIconicVar，ViewROI 才能正确计算宽高、自适应和缩放范围。
        /// </summary>
        public void ShowImage(HObject hoImage, bool fit)
        {
            image = hoImage;
            RefreshDisplayImage();
            if (fit) ResetAndFitImage();
            else
            {
                viewCtrl.addIconicVar(displayImage);
                viewCtrl.repaint();
            }
        }

        public void FitImage()
        {
            if (image != null && image.IsInitialized()) ResetAndFitImage();
            else
            {
                ForceWindowSizeRefresh();
                viewCtrl.resetAll();
                viewCtrl.repaint();
            }
        }

        private void ResetAndFitImage()
        {
            ForceWindowSizeRefresh();
            RefreshDisplayImage();
            viewCtrl.resetAll();
            viewCtrl.resetflag = true;
            viewCtrl.addIconicVar(displayImage);
            viewCtrl.repaint();
        }

        private void RefreshDisplayImage()
        {
            displayImage?.Dispose();
            displayImage = null;
            if (image == null || !image.IsInitialized()) return;

            HOperatorSet.GetImagePointer1(image, out HTuple pointer, out HTuple type, out HTuple width, out HTuple height);
            displayImage = new HImage();
            displayImage.GenImage1(type, width, height, pointer);
        }

        private void ForceWindowSizeRefresh()
        {
            window.Parent?.Refresh();
            window.Refresh();
            Application.DoEvents();
        }

        public void ClearRoi()
        {
            roiController.reset();
            viewCtrl.repaint();
        }

        /// <summary>
        /// 创建或刷新一个矩形 ROI。
        /// 本 Demo 只使用一个 ROI，它在模板 ROI 和局部检测 ROI 两种角色之间切换。
        /// </summary>
        public void EnsureRectangleRoi(double row1, double col1, double row2, double col2)
        {
            roiController.reset();
            ROIRectangle1 roi = new ROIRectangle1();
            roi.createROI((col1 + col2) / 2.0, (row1 + row2) / 2.0);

            // 与 TemplateMatchIntegratedExe 保持一致：使用 ViewROI 原生 ROIRectangle1，只通过反射写入初始坐标。
            SetPrivateDouble(roi, "row1", Math.Min(row1, row2));
            SetPrivateDouble(roi, "col1", Math.Min(col1, col2));
            SetPrivateDouble(roi, "row2", Math.Max(row1, row2));
            SetPrivateDouble(roi, "col2", Math.Max(col1, col2));
            SetPrivateDouble(roi, "midR", (row1 + row2) / 2.0);
            SetPrivateDouble(roi, "midC", (col1 + col2) / 2.0);

            roiController.ROIList.Add(roi);
            roiController.activeROIidx = 0;
            viewCtrl.repaint();
        }

        /// <summary>
        /// 读取当前激活矩形 ROI 的 Row1/Col1/Row2/Col2，用于双向绑定回参数类。
        /// </summary>
        public bool TryGetRectangleRoi(out double row1, out double col1, out double row2, out double col2)
        {
            row1 = col1 = row2 = col2 = 0;
            ROI roi = roiController.getActiveROI();
            if (roi == null && roiController.ROIList.Count > 0) roi = roiController.ROIList[0] as ROI;
            if (roi == null) return false;

            HTuple data = roi.getModelData();
            if (data == null || data.Length < 4) return false;
            row1 = data[0].D;
            col1 = data[1].D;
            row2 = data[2].D;
            col2 = data[3].D;
            return true;
        }

        /// <summary>
        /// 根据 HALCON 鼠标事件读取当前图像坐标和灰度值，用于底部状态栏实时显示。
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

                HOperatorSet.GetGrayval(image, row, col, out HTuple gray);
                return string.Format(CultureInfo.InvariantCulture, "Row:{0}  Col:{1}  Gray:{2}", row, col, gray.ToString());
            }
            catch
            {
                return "Row:-  Col:-  Gray:-";
            }
        }

        public void Repaint()
        {
            viewCtrl.repaint();
        }

        private static void SetPrivateDouble(object target, string fieldName, double value)
        {
            System.Reflection.FieldInfo field = target.GetType().GetField(fieldName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (field != null) field.SetValue(target, value);
        }
    }
}








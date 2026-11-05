using Frame_Core.pluginParams;
using FrameCommunity.MaskRoiTool;
using HalconDotNet;
using Plugin.TemplateMatch.Core;
using System;
using System.Globalization;
using System.Windows.Forms;
using ViewROI;

namespace Plugin.TemplateMatch.Services
{
    /// <summary>
    /// Halcon 视图 ROI 适配器。封装了 HWindowControl、ROIController 和 HWndCtrl，
    /// 负责显示图像、管理模板 ROI 和局部 ROI 的交互（绘制、编辑、同步），
    /// 并将用户的 ROI 操作通过事件通知上层 ViewModel。
    /// </summary>
    public sealed class HalconViewRoiAdapter : IDisposable
    {
        // ---------- 核心控件与控制器 ----------
        private readonly HWindowControl window;          // Halcon 显示控件
        private readonly HWndCtrl viewCtrl;              // 视图控制器（负责缩放、平移、ROI 绘制）
        private readonly ROIController roiController;    // ROI 控制器（管理 ROI 列表、激活状态）

        // ---------- 事件句柄（用于取消订阅）----------
        private HWndCtrl.ReDrawDelegate redrawHandler;
        private IconicDelegate roiChangedHandler;
        private HMouseEventHandler mouseMoveHandler;
        private HMouseEventHandler mouseDownHandler;
        private HMouseEventHandler mouseUpHandler;

        // ---------- 图像与参数 ----------
        private HObject image;               // 原始图像（外部传入，不负责释放）
        private HObject displayImage;        // 用于显示的图像副本（灰度图，本类负责释放）
        private ParamsTemplateMatch overlayParams;  // 插件参数，用于绘制覆盖层（匹配结果等）
        private TemplateModelParam selectedTemplate; // 当前选中的模板参数

        // ---------- ROI 对象 ----------
        private ROI templateViewRoi;          // 模板 ROI（与 selectedTemplate 关联）
        private ROI localViewRoi;             // 局部 ROI（与 overlayParams 中的局部 ROI 关联）
        private ROI maskViewRoi;              // 当前选中屏蔽 ROI

        // ---------- 当前激活的 ROI 信息 ----------
        private int activeRoiRole;            // 当前激活的 ROI 角色：模板 ROI 或局部 ROI（见 ParamsTemplateMatch.RoiRoleXxx）
        private int activeRoiShapeType;       // 当前激活的 ROI 形状类型（矩形、旋转矩形等）

        // ---------- 缓存的状态，用于快速重建 ROI ----------
        private int templateViewRoiShapeType;
        private int localViewRoiShapeType;
        private int maskViewRoiShapeType;
        private bool showTemplateViewRoi;     // 是否显示模板 ROI
        private bool showLocalViewRoi;        // 是否显示局部 ROI
        private bool showMaskViewRoi;         // 是否显示当前屏蔽 ROI
        private bool editTemplateViewRoi;     // 是否允许编辑模板 ROI
        private bool editLocalViewRoi;        // 是否允许编辑局部 ROI
        private bool editMaskViewRoi;         // 是否允许编辑屏蔽 ROI

        private bool disposed;                // 资源释放标志

        /// <summary>
        /// 构造 Halcon 视图 ROI 适配器。
        /// </summary>
        /// <param name="windowControl">Halcon 窗口控件</param>
        /// <param name="enableRoi">是否启用 ROI 操作（绘制/编辑）</param>
        public HalconViewRoiAdapter(HWindowControl windowControl, bool enableRoi)
        {
            window = windowControl ?? throw new ArgumentNullException(nameof(windowControl));
            roiController = new ROIController();
            viewCtrl = new HWndCtrl(window);
            viewCtrl.ho_Image?.Dispose();
            viewCtrl.ho_Image_Changed?.Dispose();
            viewCtrl.ho_Image = null;
            viewCtrl.ho_Image_Changed = null;
            viewCtrl.useROIController(roiController);
            viewCtrl.setDispLevel(HWndCtrl.MODE_INCLUDE_ROI);  // 显示 ROI
            viewCtrl.setViewState(HWndCtrl.MODE_VIEW_MOVE);    // 默认视图模式为移动/缩放
            viewCtrl.bl_RoiOperateFlag = enableRoi;            // 是否允许 ROI 操作
            roiController.setDrawColor("yellow", "red", "gray"); // 设置 ROI 绘制颜色（未激活、激活、固定）

            // 创建委托实例，用于事件订阅和取消订阅
            redrawHandler = OnViewRoiRedraw;
            roiChangedHandler = OnViewRoiChanged;
            mouseMoveHandler = OnMouseMove;
            mouseDownHandler = OnMouseDown;
            mouseUpHandler = OnMouseUp;

            // 订阅事件
            viewCtrl.ReDrawEvent += redrawHandler;
            roiController.NotifyRCObserver += roiChangedHandler;
            window.HMouseMove += mouseMoveHandler;
            window.HMouseDown += mouseDownHandler;
            window.HMouseUp += mouseUpHandler;
        }

        // ---------- 公开事件 ----------
        /// <summary> ROI 发生变化时触发（创建、更新、激活等）。参数为 ROIController 的事件代码。 </summary>
        public event Action<int> RoiChanged;
        /// <summary> 鼠标移动时触发，传递坐标和灰度值信息。 </summary>
        public event Action<HalconMouseInfoArgs> MouseMoved;
        /// <summary> 鼠标右键按下时触发。 </summary>
        public event Action<HalconMouseInfoArgs> RightMouseDown;
        /// <summary> 鼠标右键弹起时触发。 </summary>
        public event Action<HalconMouseInfoArgs> RightMouseUp;
        /// <summary> 鼠标中键按下时触发。 </summary>
        public event Action<HalconMouseInfoArgs> MiddleMouseDown;
        /// <summary> 重绘完成后触发，用于上层绘制额外的覆盖层（基准点、匹配结果等）。 </summary>
        public event Action Redraw;

        // ---------- 公开属性 ----------
        public HWindow HalconWindow => window.HalconWindow;
        public ROIController RoiController => roiController;
        public int ActiveRoiRole => activeRoiRole;
        public int ActiveRoiShapeType => activeRoiShapeType;

        /// <summary>
        /// 设置覆盖层所需的数据（插件参数和选中的模板）。
        /// 这些数据将在绘制覆盖层时使用。
        /// </summary>
        public void SetOverlayState(ParamsTemplateMatch parameters, TemplateModelParam template)
        {
            overlayParams = parameters;
            selectedTemplate = template;
        }

        /// <summary>
        /// 启用或禁用 ROI 编辑功能（即是否可以拖动、新建 ROI）。
        /// </summary>
        public void SetRoiOperate(bool enabled)
        {
            if (disposed) return;
            viewCtrl.bl_RoiOperateFlag = enabled;
            Repaint();
        }

        /// <summary>
        /// 根据插件参数和选中的模板，同步模板 ROI 和局部 ROI 到 ROIController 中。
        /// 该方法会重建 ROI 对象，并根据显示/编辑标志设置 ROI 列表的可见性和可编辑性。
        /// </summary>
        public void SyncTemplateAndLocalRois(ParamsTemplateMatch parameters, TemplateModelParam template, MaskRoiParam selectedMaskRoi = null)
        {
            if (disposed) return;

            overlayParams = parameters;
            selectedTemplate = template;

            // 根据参数决定是否显示模板 ROI 和局部 ROI
            showTemplateViewRoi = parameters != null && parameters.m_bShowViewRoi && parameters.m_bShowTemplateRoi && template != null;
            showLocalViewRoi = parameters != null && parameters.m_bShowViewRoi && parameters.m_bUseLocalRoi && parameters.m_bShowLocalRoi;
            showMaskViewRoi = parameters != null && parameters.m_bShowViewRoi && parameters.m_bShowMaskRoi && selectedMaskRoi != null;
            editTemplateViewRoi = showTemplateViewRoi && parameters.m_bEditTemplateRoi;
            editLocalViewRoi = showLocalViewRoi && parameters.m_bEditLocalRoi;
            editMaskViewRoi = showMaskViewRoi && parameters.m_bEditMaskRoi;

            // 更新或创建模板 ROI 对象
            if (showTemplateViewRoi)
            {
                templateViewRoi = UpdateConfiguredRoi(
                    templateViewRoi,
                    ref templateViewRoiShapeType,
                    template.RoiShapeType,
                    template.RoiRow1,
                    template.RoiCol1,
                    template.RoiRow2,
                    template.RoiCol2);
            }

            // 更新或创建局部 ROI 对象
            if (showLocalViewRoi)
            {
                localViewRoi = UpdateConfiguredRoi(
                    localViewRoi,
                    ref localViewRoiShapeType,
                    parameters.m_nLocalRoiShapeType,
                    parameters.m_fLocalRoiRow1,
                    parameters.m_fLocalRoiCol1,
                    parameters.m_fLocalRoiRow2,
                    parameters.m_fLocalRoiCol2);
            }

            // 屏蔽 ROI 支持多 ROI 列表，交互编辑的是当前选中项。
            if (showMaskViewRoi)
            {
                maskViewRoi = UpdateConfiguredRoi(
                    maskViewRoi,
                    ref maskViewRoiShapeType,
                    ConvertMaskShapeType(selectedMaskRoi.Shape),
                    selectedMaskRoi.Row1,
                    selectedMaskRoi.Col1,
                    selectedMaskRoi.Row2,
                    selectedMaskRoi.Col2);
            }

            // ROIController 依靠 ROIList 来管理显示和交互。这里只保留需要显示的 ROI 对象。
            // 注意：保留相同的 ROI 实例可以保持用户拖拽时的状态连续性。
            roiController.ROIList.Clear();
            if (showTemplateViewRoi) roiController.ROIList.Add(templateViewRoi);
            if (showLocalViewRoi) roiController.ROIList.Add(localViewRoi);
            if (showMaskViewRoi) roiController.ROIList.Add(maskViewRoi);

            // 编程同步 ROI 后，必须清除激活索引，否则下次鼠标弹起可能会被误认为是用户编辑，
            // 导致将当前显示坐标写回参数（而实际并未做编辑）。
            roiController.activeROIidx = -1;
            viewCtrl.bl_RoiOperateFlag = editTemplateViewRoi || editLocalViewRoi || editMaskViewRoi;
            Repaint();
        }

        /// <summary>
        /// 清除所有交互式 ROI（清空 ROIController 中的所有 ROI）。
        /// </summary>
        public void ClearInteractiveRoi()
        {
            if (disposed) return;
            roiController.reset();
            Repaint();
        }

        /// <summary>
        /// 根据参数刷新显示（用于外部参数变更后重新显示 ROI）。
        /// </summary>
        public void RefreshFromParams(ParamsTemplateMatch parameters, TemplateModelParam template, bool fit)
        {
            if (disposed) return;
            overlayParams = parameters;
            selectedTemplate = template;

            if (template != null)
                EnsureRoi(ParamsTemplateMatch.RoiRoleTemplate, template.RoiShapeType, template.RoiRow1, template.RoiCol1, template.RoiRow2, template.RoiCol2);

            image = parameters?.ho_Image;
            ShowImage(image, fit);
        }

        /// <summary>
        /// 显示图像，并可选择适应窗口大小。
        /// </summary>
        public void ShowImage(HObject hoImage, bool fit)
        {
            if (disposed) return;
            image = hoImage;
            UpdateRoiHandleSize();   // 根据图像尺寸更新 ROI 手柄大小

            if (fit)
            {
                ResetAndFitImage();
                return;
            }

            RefreshDisplayImage();   // 生成用于显示的灰度图像副本
            if (displayImage == null || !displayImage.IsInitialized()) return;

            viewCtrl.addIconicVar(displayImage);
            viewCtrl.resetflag = false;
            viewCtrl.repaint();
        }

        /// <summary>
        /// 适应窗口大小显示图像。
        /// </summary>
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

        /// <summary>
        /// 确保存在一个指定角色和形状的 ROI，并设置其坐标。
        /// 如果已有 ROI 则更新，否则创建。
        /// </summary>
        public void EnsureRoi(int role, int shapeType, double row1, double col1, double row2, double col2)
        {
            if (disposed) return;
            activeRoiRole = role;
            activeRoiShapeType = NormalizeShapeType(shapeType);

            ROI roi = null;
            if (roiController.ROIList.Count > 0)
                roi = roiController.ROIList[0] as ROI;

            if (roi == null || GetRoiType(roi) != activeRoiShapeType)
            {
                roiController.reset();
                roi = CreateRoi(activeRoiShapeType);
                roi.createROI((col1 + col2) / 2.0, (row1 + row2) / 2.0);
                roiController.ROIList.Add(roi);
            }

            SetRoiBounds(roi, Math.Min(row1, row2), Math.Min(col1, col2), Math.Max(row1, row2), Math.Max(col1, col2));
            roiController.activeROIidx = 0;
            Repaint();
        }

        /// <summary>
        /// 尝试获取当前激活的 ROI 的边界矩形（标准轴对齐矩形）。
        /// </summary>
        /// <returns>成功获取返回 true，否则 false。</returns>
        public bool TryGetActiveRoiBounds(out double row1, out double col1, out double row2, out double col2)
        {
            row1 = col1 = row2 = col2 = 0;
            ROI roi = roiController.getActiveROI();
            if (roi == null && roiController.ROIList.Count > 0) roi = roiController.ROIList[0] as ROI;
            return TryGetRoiBounds(roi, out row1, out col1, out row2, out col2);
        }

        /// <summary>
        /// 强制重绘视图。
        /// </summary>
        public void Repaint()
        {
            if (disposed) return;
            viewCtrl.repaint();
        }

        /// <summary>
        /// 根据鼠标位置和当前图像，生成显示文本（行、列、灰度值）。
        /// </summary>
        public string GetMouseText(HMouseEventArgs e)
        {
            if (!HasValidImage(image)) return "Row-  Col-  灰度:-";
            try
            {
                int row = (int)Math.Round(e.Y);
                int col = (int)Math.Round(e.X);
                HOperatorSet.GetImageSize(image, out HTuple width, out HTuple height);
                if (row < 0 || col < 0 || row >= height.I || col >= width.I) return "Row-  Col-  灰度:-";

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
                    return string.Format(CultureInfo.InvariantCulture, "Row：{0}  Col：{1}  灰度:{2}", row, col, gray.ToString());
                }
                finally
                {
                    channel?.Dispose();
                }
            }
            catch
            {
                // 历史遗留乱码字符串，保留原样
                return "琛?-  鍒?-  鐏板害:-";
            }
        }

        /// <summary>
        /// 释放资源，取消事件订阅，清空 ROI 和图像副本。
        /// </summary>
        public void Dispose()
        {
            if (disposed) return;
            disposed = true;

            // 取消事件订阅
            try
            {
                if (redrawHandler != null) viewCtrl.ReDrawEvent -= redrawHandler;
                if (roiChangedHandler != null) roiController.NotifyRCObserver -= roiChangedHandler;
                if (mouseMoveHandler != null) window.HMouseMove -= mouseMoveHandler;
                if (mouseDownHandler != null) window.HMouseDown -= mouseDownHandler;
                if (mouseUpHandler != null) window.HMouseUp -= mouseUpHandler;
            }
            catch { }

            try { roiController.reset(); } catch { }
            templateViewRoi = null;
            localViewRoi = null;
            maskViewRoi = null;

            try
            {
                viewCtrl.ho_Image = null;
                viewCtrl.ho_Image_Changed = null;
            }
            catch { }

            // 释放本类拥有的显示图像副本
            displayImage?.Dispose();
            displayImage = null;
            image = null;
            overlayParams = null;
            selectedTemplate = null;
            // 清空事件委托，避免内存泄漏
            RoiChanged = null;
            MouseMoved = null;
            RightMouseDown = null;
            RightMouseUp = null;
            MiddleMouseDown = null;
            Redraw = null;
        }

        // ---------- 私有辅助方法 ----------

        /// <summary>
        /// 重置视图并适应图像大小。
        /// </summary>
        private void ResetAndFitImage()
        {
            RefreshDisplayImage();
            if (displayImage == null || !displayImage.IsInitialized()) return;

            viewCtrl.resetAll();
            viewCtrl.resetflag = true;
            viewCtrl.addIconicVar(displayImage);
            viewCtrl.repaint();
            viewCtrl.resetflag = false;
        }

        /// <summary>
        /// 根据图像尺寸更新 ROI 手柄的相对大小。
        /// </summary>
        private void UpdateRoiHandleSize()
        {
            if (!HasValidImage(image)) return;

            try
            {
                HOperatorSet.GetImageSize(image, out HTuple width, out HTuple height);
                double longSide = Math.Max(width.D, height.D);
                roiController.size = longSide < 1000 ? 1.5 : longSide / 650.0;
            }
            catch
            {
            }
        }

        /// <summary>
        /// 生成用于显示的图像副本（如果是彩色图则转换为灰度图，并复制一份）。
        /// </summary>
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
                // 复制一份图像，因为 displayImage 需要独立管理生命周期
                displayImage = displaySource.CopyObj(1, 1);
            }
            finally
            {
                grayImage?.Dispose();
            }
        }

        // ---------- 事件回调 ----------
        private void OnViewRoiRedraw()
        {
            Redraw?.Invoke();           // 通知上层重绘覆盖层
            DrawOverlay();              // 绘制本类管理的覆盖层（模板 ROI 边界、局部 ROI 边界、匹配结果等）
        }

        private void OnViewRoiChanged(int eventCode)
        {
            activeRoiRole = ResolveActiveRoiRole();
            activeRoiShapeType = GetActiveRoiShapeType();

            // 如果当前激活的 ROI 不允许编辑（比如模板 ROI 未开启编辑标志），则立即取消激活
            if (eventCode == ROIController.EVENT_ACTIVATED_ROI && !IsActiveRoleEditable())
            {
                roiController.activeROIidx = -1;
                Repaint();
                return;
            }

            RoiChanged?.Invoke(eventCode);
        }

        private void OnMouseMove(object sender, HMouseEventArgs e)
        {
            MouseMoved?.Invoke(new HalconMouseInfoArgs(e.Y, e.X, GetMouseText(e)));
        }

        private void OnMouseDown(object sender, HMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                RightMouseDown?.Invoke(new HalconMouseInfoArgs(e.Y, e.X, GetMouseText(e)));
            else if (e.Button == MouseButtons.Middle)
                MiddleMouseDown?.Invoke(new HalconMouseInfoArgs(e.Y, e.X, GetMouseText(e)));
        }

        private void OnMouseUp(object sender, HMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                RightMouseUp?.Invoke(new HalconMouseInfoArgs(e.Y, e.X, GetMouseText(e)));
        }

        /// <summary>
        /// 绘制覆盖层：模板 ROI、局部 ROI、匹配结果轮廓等。
        /// </summary>
        private void DrawOverlay()
        {
            if (disposed || overlayParams == null) return;

            try
            {
                DrawTemplateRoi(HalconWindow, overlayParams, selectedTemplate);
                DrawLocalRoi(HalconWindow, overlayParams);
                DrawMaskRois(HalconWindow, overlayParams);
                DrawResults(HalconWindow, overlayParams);
            }
            catch
            {
            }
        }

        /// <summary>
        /// 根据 ROI 列表中的顺序和显示标志，确定当前激活的 ROI 角色（模板 or 局部）。
        /// </summary>
        private int ResolveActiveRoiRole()
        {
            int activeIndex = roiController.activeROIidx;
            if (activeIndex < 0) return activeRoiRole;

            int index = 0;
            if (showTemplateViewRoi && activeIndex == index)
                return ParamsTemplateMatch.RoiRoleTemplate;

            if (showTemplateViewRoi) index++;

            if (showLocalViewRoi && activeIndex == index)
                return ParamsTemplateMatch.RoiRoleLocal;

            if (showLocalViewRoi) index++;

            if (showMaskViewRoi && activeIndex == index)
                return ParamsTemplateMatch.RoiRoleMask;

            return activeRoiRole;
        }

        /// <summary>
        /// 根据角色获取其在 ROIList 中的索引。
        /// </summary>
        private int ResolveIndexForRole(int role)
        {
            if (role == ParamsTemplateMatch.RoiRoleTemplate && showTemplateViewRoi)
                return 0;

            if (role == ParamsTemplateMatch.RoiRoleLocal && showLocalViewRoi)
                return showTemplateViewRoi ? 1 : 0;

            if (role == ParamsTemplateMatch.RoiRoleMask && showMaskViewRoi)
            {
                int index = 0;
                if (showTemplateViewRoi) index++;
                if (showLocalViewRoi) index++;
                return index;
            }

            return -1;
        }

        /// <summary>
        /// 获取当前激活 ROI 的形状类型。
        /// </summary>
        private int GetActiveRoiShapeType()
        {
            ROI roi = roiController.getActiveROI();
            if (roi == null) return activeRoiShapeType;
            return GetRoiType(roi);
        }

        /// <summary>
        /// 判断当前激活的 ROI 是否允许编辑。
        /// </summary>
        private bool IsActiveRoleEditable()
        {
            if (activeRoiRole == ParamsTemplateMatch.RoiRoleTemplate)
                return editTemplateViewRoi;

            if (activeRoiRole == ParamsTemplateMatch.RoiRoleLocal)
                return editLocalViewRoi;

            if (activeRoiRole == ParamsTemplateMatch.RoiRoleMask)
                return editMaskViewRoi;

            return false;
        }

        // ---------- 静态绘制辅助方法 ----------
        private static void DrawLocalRoi(HWindow target, ParamsTemplateMatch parameters)
        {
            if (parameters == null || !parameters.m_bUseLocalRoi || !parameters.m_bShowLocalRoi) return;
            target.SetDraw("margin");
            target.SetLineWidth(2);
            target.SetColor("cyan");
            target.DispRectangle1(parameters.m_fLocalRoiRow1, parameters.m_fLocalRoiCol1, parameters.m_fLocalRoiRow2, parameters.m_fLocalRoiCol2);
            DrawLabel(target, parameters.m_fLocalRoiRow1, parameters.m_fLocalRoiCol1, "局部ROI", "cyan");
        }

        private static void DrawMaskRois(HWindow target, ParamsTemplateMatch parameters)
        {
            if (parameters == null || !parameters.m_bShowMaskRoi || parameters.m_lMaskRoiParams == null) return;

            target.SetDraw("margin");
            target.SetLineWidth(2);
            foreach (MaskRoiParam roi in parameters.m_lMaskRoiParams)
            {
                if (roi == null) continue;
                target.SetColor(roi.Enabled ? "red" : "gray");
                switch (roi.Shape)
                {
                    case MaskRoiShapeType.Rectangle2:
                        target.DispRectangle2(roi.CenterRow, roi.CenterCol, roi.Angle, roi.Length1, roi.Length2);
                        break;
                    case MaskRoiShapeType.Circle:
                        target.DispCircle(roi.CenterRow, roi.CenterCol, roi.Radius);
                        break;
                    default:
                        target.DispRectangle1(roi.Row1, roi.Col1, roi.Row2, roi.Col2);
                        break;
                }
                DrawLabel(target, roi.Row1, roi.Col1, roi.Enabled ? roi.Name : roi.Name + "(禁用)", roi.Enabled ? "red" : "gray");
            }

            if (parameters.ho_OutMaskRoiRegion != null && parameters.ho_OutMaskRoiRegion.IsInitialized())
            {
                target.SetColor("orange red");
                target.SetLineWidth(1);
                target.DispObj(parameters.ho_OutMaskRoiRegion);
            }
        }

        private static void DrawTemplateRoi(HWindow target, ParamsTemplateMatch parameters, TemplateModelParam template)
        {
            if (template == null || parameters == null || !parameters.m_bShowTemplateRoi) return;

            target.SetDraw("margin");
            target.SetLineWidth(2);
            target.SetColor("yellow");
            target.DispRectangle1(template.RoiRow1, template.RoiCol1, template.RoiRow2, template.RoiCol2);
            DrawLabel(target, template.RoiRow1, template.RoiCol1, "模板原始ROI", "yellow");

            if (template.CreateContours != null && template.CreateContours.IsInitialized())
            {
                target.SetColor("orange red");
                target.DispObj(template.CreateContours);
            }

            DrawCross(target, template.BaseRow, template.BaseCol, 12, "green");
        }

        private static void DrawResults(HWindow target, ParamsTemplateMatch parameters)
        {
            if (parameters == null) return;

            if (parameters.ho_OutContours != null && parameters.ho_OutContours.IsInitialized())
            {
                target.SetColor("green");
                target.SetLineWidth(2);
                target.DispObj(parameters.ho_OutContours);
            }

            if (parameters.Results == null) return;
            foreach (TemplateMatchResultParam result in parameters.Results)
            {
                if (result == null || !result.IsSuccess) continue;
                DrawMatchFollowRoi(target, parameters, result);
                DrawCross(target, result.Row, result.Col, 16, "red");
                target.SetColor("red");
                target.SetTposition((int)Math.Round(result.Row + 10), (int)Math.Round(result.Col + 10));
                target.WriteString(string.Format(CultureInfo.InvariantCulture, "{0}:{1:0.000}", result.TemplateName, result.Score));
            }
        }

        private static void DrawMatchFollowRoi(HWindow target, ParamsTemplateMatch parameters, TemplateMatchResultParam result)
        {
            if (parameters == null || !parameters.m_bRoiFollowMatch) return;

            TemplateModelParam template = FindTemplate(parameters, result.TemplateName);
            if (template != null)
            {
                DrawTemplateRoiAtMatch(target, template, result);
                return;
            }

            if (parameters.ho_OutMatchRoiRegion == null || !parameters.ho_OutMatchRoiRegion.IsInitialized()) return;
            target.SetDraw("margin");
            target.SetLineWidth(2);
            target.SetColor("orange");
            target.DispObj(parameters.ho_OutMatchRoiRegion);
            DrawLabel(target, result.Row, result.Col, "匹配ROI", "orange");
        }

        private static TemplateModelParam FindTemplate(ParamsTemplateMatch parameters, string templateName)
        {
            if (parameters?.Templates == null) return null;
            foreach (TemplateModelParam template in parameters.Templates)
            {
                if (template == null) continue;
                if (string.Equals(template.Name, templateName, StringComparison.OrdinalIgnoreCase))
                    return template;
            }
            return null;
        }

        /// <summary>
        /// 根据匹配结果将模板 ROI 变换到匹配位置并绘制。
        /// </summary>
        private static void DrawTemplateRoiAtMatch(HWindow target, TemplateModelParam template, TemplateMatchResultParam result)
        {
            HObject contour = null;
            try
            {
                TransformRectCorners(
                    template.RoiRow1,
                    template.RoiCol1,
                    template.RoiRow2,
                    template.RoiCol2,
                    template.BaseRow,
                    template.BaseCol,
                    template.BaseAngleDeg,
                    result.Row,
                    result.Col,
                    result.AngleDeg,
                    out HTuple rows,
                    out HTuple cols);

                HOperatorSet.GenContourPolygonXld(out contour, rows, cols);
                target.SetDraw("margin");
                target.SetLineWidth(3);
                target.SetColor("orange");
                target.DispObj(contour);
                DrawLabel(target, rows.TupleMin().D, cols.TupleMin().D, "匹配ROI", "orange");
            }
            finally
            {
                contour?.Dispose();
            }
        }

        /// <summary>
        /// 将矩形角点从模板基准坐标系变换到匹配结果坐标系（考虑平移和旋转）。
        /// </summary>
        private static void TransformRectCorners(double row1, double col1, double row2, double col2,
            double baseRow, double baseCol, double baseAngleDeg,
            double currentRow, double currentCol, double currentAngleDeg,
            out HTuple rows, out HTuple cols)
        {
            double[] srcRows = { row1, row1, row2, row2, row1 };
            double[] srcCols = { col1, col2, col2, col1, col1 };
            double[] dstRows = new double[srcRows.Length];
            double[] dstCols = new double[srcCols.Length];
            double deltaAngleRad = (currentAngleDeg - baseAngleDeg) * Math.PI / 180.0;
            double cos = Math.Cos(deltaAngleRad);
            double sin = Math.Sin(deltaAngleRad);

            for (int i = 0; i < srcRows.Length; i++)
            {
                double dr = srcRows[i] - baseRow;
                double dc = srcCols[i] - baseCol;
                dstRows[i] = currentRow + dr * cos - dc * sin;
                dstCols[i] = currentCol + dr * sin + dc * cos;
            }

            rows = new HTuple(dstRows);
            cols = new HTuple(dstCols);
        }

        private static void DrawLabel(HWindow target, double row, double col, string text, string color)
        {
            target.SetColor(color);
            target.SetTposition((int)Math.Round(row + 8), (int)Math.Round(col + 8));
            target.WriteString(text);
        }

        private static void DrawCross(HWindow target, double row, double col, double size, string color)
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

        // ---------- ROI 创建与操作辅助方法 ----------
        private static ROI CreateRoi(int shapeType)
        {
            switch (NormalizeShapeType(shapeType))
            {
                case ROI.ROI_TYPE_LINE:
                    return new ROILine();
                case ROI.ROI_TYPE_CIRCLE:
                    return new ROICircle();
                case ROI.ROI_TYPE_CIRCLEARC:
                    return new ROICircularArc();
                case ROI.ROI_TYPE_RECTANGLE2:
                    return new ROIRectangle2();
                case ROI.ROI_TYPE_RECTANCLE1:
                default:
                    return new ROIRectangle1();
            }
        }

        private static ROI CreateConfiguredRoi(int shapeType, double row1, double col1, double row2, double col2)
        {
            int normalizedType = NormalizeShapeType(shapeType);
            ROI roi = CreateRoi(normalizedType);
            roi.createROI((col1 + col2) / 2.0, (row1 + row2) / 2.0);
            SetRoiBounds(roi, Math.Min(row1, row2), Math.Min(col1, col2), Math.Max(row1, row2), Math.Max(col1, col2));
            return roi;
        }

        private static ROI UpdateConfiguredRoi(ROI current, ref int currentShapeType, int shapeType, double row1, double col1, double row2, double col2)
        {
            int normalizedType = NormalizeShapeType(shapeType);
            if (current == null || currentShapeType != normalizedType || GetRoiType(current) != normalizedType)
            {
                current = CreateRoi(normalizedType);
                current.createROI((col1 + col2) / 2.0, (row1 + row2) / 2.0);
                currentShapeType = normalizedType;
            }

            SetRoiBounds(current, Math.Min(row1, row2), Math.Min(col1, col2), Math.Max(row1, row2), Math.Max(col1, col2));
            return current;
        }

        private static int NormalizeShapeType(int shapeType)
        {
            switch (shapeType)
            {
                case ROI.ROI_TYPE_LINE:
                case ROI.ROI_TYPE_CIRCLE:
                case ROI.ROI_TYPE_CIRCLEARC:
                case ROI.ROI_TYPE_RECTANCLE1:
                case ROI.ROI_TYPE_RECTANGLE2:
                    return shapeType;
                default:
                    return ROI.ROI_TYPE_RECTANCLE1;
            }
        }

        private static int ConvertMaskShapeType(MaskRoiShapeType shapeType)
        {
            switch (shapeType)
            {
                case MaskRoiShapeType.Rectangle2:
                    return ROI.ROI_TYPE_RECTANGLE2;
                case MaskRoiShapeType.Circle:
                    return ROI.ROI_TYPE_CIRCLE;
                case MaskRoiShapeType.Rectangle1:
                default:
                    return ROI.ROI_TYPE_RECTANCLE1;
            }
        }

        private static int GetRoiType(ROI roi)
        {
            if (roi is ROILine) return ROI.ROI_TYPE_LINE;
            if (roi is ROICircle) return ROI.ROI_TYPE_CIRCLE;
            if (roi is ROICircularArc) return ROI.ROI_TYPE_CIRCLEARC;
            if (roi is ROIRectangle2) return ROI.ROI_TYPE_RECTANGLE2;
            return ROI.ROI_TYPE_RECTANCLE1;
        }

        /// <summary>
        /// 设置 ROI 的边界（支持矩形、旋转矩形、线、圆、圆弧等类型）。
        /// 使用反射设置私有字段，因为 ViewROI 库未提供公开的 Set 方法。
        /// </summary>
        private static void SetRoiBounds(ROI roi, double row1, double col1, double row2, double col2)
        {
            double midR = (row1 + row2) / 2.0;
            double midC = (col1 + col2) / 2.0;
            double width = Math.Max(1.0, Math.Abs(col2 - col1));
            double height = Math.Max(1.0, Math.Abs(row2 - row1));

            if (roi is ROIRectangle1)
            {
                SetPrivateDouble(roi, "row1", row1);
                SetPrivateDouble(roi, "col1", col1);
                SetPrivateDouble(roi, "row2", row2);
                SetPrivateDouble(roi, "col2", col2);
                SetPrivateDouble(roi, "midR", midR);
                SetPrivateDouble(roi, "midC", midC);
                return;
            }

            if (roi is ROIRectangle2)
            {
                SetPublicDouble(roi, "midR", midR);
                SetPublicDouble(roi, "midC", midC);
                SetPublicDouble(roi, "length1", height / 2.0);
                SetPublicDouble(roi, "length2", width / 2.0);
                CallNoArg(roi, "updateHandlePos");
                return;
            }

            if (roi is ROILine)
            {
                SetPrivateDouble(roi, "row1", midR);
                SetPrivateDouble(roi, "col1", col1);
                SetPrivateDouble(roi, "row2", midR);
                SetPrivateDouble(roi, "col2", col2);
                SetPrivateDouble(roi, "midR", midR);
                SetPrivateDouble(roi, "midC", midC);
                CallNoArg(roi, "updateArrowHandle");
                return;
            }

            if (roi is ROICircle)
            {
                double radius = Math.Max(width, height) / 2.0;
                SetPrivateDouble(roi, "midR", midR);
                SetPrivateDouble(roi, "midC", midC);
                SetPrivateDouble(roi, "radius", radius);
                SetPrivateDouble(roi, "row1", midR);
                SetPrivateDouble(roi, "col1", midC + radius);
                return;
            }

            if (roi is ROICircularArc)
            {
                double radius = Math.Max(width, height) / 2.0;
                SetPrivateDouble(roi, "midR", midR);
                SetPrivateDouble(roi, "midC", midC);
                SetPrivateDouble(roi, "radius", radius);
                SetPrivateDouble(roi, "sizeR", midR);
                SetPrivateDouble(roi, "sizeC", midC - radius);
                SetPrivateDouble(roi, "startPhi", Math.PI * 0.25);
                SetPrivateDouble(roi, "extentPhi", Math.PI * 1.5);
                SetPrivateString(roi, "circDir", "positive");
                CallNoArg(roi, "determineArcHandles");
                CallNoArg(roi, "updateArrowHandle");
            }
        }

        private static bool TryGetRoiBounds(ROI roi, out double row1, out double col1, out double row2, out double col2)
        {
            row1 = col1 = row2 = col2 = 0;
            if (roi == null) return false;

            // 对于矩形和线，可以直接从 modelData 获取坐标
            if (roi is ROIRectangle1 || roi is ROILine)
            {
                HTuple data = roi.getModelData();
                if (data != null && data.Length >= 4)
                {
                    row1 = Math.Min(data[0].D, data[2].D);
                    col1 = Math.Min(data[1].D, data[3].D);
                    row2 = Math.Max(data[0].D, data[2].D);
                    col2 = Math.Max(data[1].D, data[3].D);
                    return true;
                }
            }

            // 对于其他 ROI，通过生成 Region 然后求最小外接矩形
            HRegion region = null;
            try
            {
                region = roi.getRegion();
                if (region == null || !region.IsInitialized()) return false;
                HOperatorSet.SmallestRectangle1(region, out HTuple r1, out HTuple c1, out HTuple r2, out HTuple c2);
                row1 = r1.D;
                col1 = c1.D;
                row2 = r2.D;
                col2 = c2.D;
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                region?.Dispose();
            }
        }

        // 反射辅助方法：设置私有字段、公有字段、调用方法
        private static void SetPrivateDouble(object target, string fieldName, double value)
        {
            var field = target.GetType().GetField(fieldName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (field != null) field.SetValue(target, value);
        }

        private static void SetPublicDouble(object target, string fieldName, double value)
        {
            var field = target.GetType().GetField(fieldName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            if (field != null) field.SetValue(target, value);
        }

        private static void SetPrivateString(object target, string fieldName, string value)
        {
            var field = target.GetType().GetField(fieldName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (field != null) field.SetValue(target, value);
        }

        private static void CallNoArg(object target, string methodName)
        {
            var method = target.GetType().GetMethod(methodName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            if (method != null) method.Invoke(target, null);
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

    /// <summary>
    /// Halcon 鼠标事件参数，包含行、列坐标及描述文本。
    /// </summary>
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

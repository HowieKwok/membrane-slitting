using Frame_Core;
using Frame_Core.pluginParams;
using HalconDotNet;

using Plugin.CACRunCalibration.Core;
using Plugin.CACRunCalibration.Models;
using Plugin.CACRunCalibration.Services;
using Plugin.CACRunCalibration.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using WinForms = System.Windows.Forms;

namespace Plugin.CACRunCalibration.WPF架构
{
    /// <summary>
    /// CAC运行标定插件的 WPF 用户控件视图。
    /// 实现 IPluginWpfView，提供图像预览、ROI 操作、参数绑定和运行/保存等交互。
    /// </summary>
    public partial class WPFRunCalibration : UserControl, IPluginWpfView
    {
        // ----- 核心组件 -----
        private readonly CACRunCalibrationEngine engine = new CACRunCalibrationEngine();   // 标定引擎（矩阵生成、转换）
        private CACRunCalibrationViewModel viewModel;      // 视图模型，绑定所有界面数据与命令
        private HalconViewRoiAdapter roiAdapter;           // Halcon 图像显示与 ROI 适配器
        private HWindowControl halconControl;              // Halcon 原生的图像显示控件（Windows Forms 宿主）
        private HObject ownedLoadedImage;                  // 手动加载的图像（需手动释放）
        private bool displayFitRequired = true;            // 下次显示时是否需要自适应窗口
        private bool disposed;                             // 是否已释放
        private bool closingFromHost;                      // 是否通过框架 CloseFromHost 退出
        private bool syncingRoi;                           // 防止 ROI 变更时的循环同步

        // ----- 接口要求的属性 -----
        public IPlugin PluginObject { get; private set; }           // 所属的插件对象
        public PluginParamsBase PluginParams { get; private set; }  // 插件参数对象

        /// <summary>
        /// 构造函数，仅初始化 XAML 布局并绑定一个默认参数对象。
        /// 实际参数绑定在 BindPlugin 中完成。
        /// </summary>
        public WPFRunCalibration()
        {
            InitializeComponent();
            BindViewModel(new ParamsCACRunCalibration());
        }

        /// <summary>
        /// 框架调用，将插件对象与视图关联。
        /// 初始化视图模型、Halcon 窗口、刷新显示和矩阵状态。
        /// </summary>
        public void BindPlugin(IPlugin plugin)
        {
            PluginObject = plugin;
            PluginParams = plugin == null ? null : plugin.PluginParams;
            ParamsCACRunCalibration param = PluginParams as ParamsCACRunCalibration;
            if (param == null) return;

            disposed = false;
            BindViewModel(param);             // 创建 ViewModel 并绑定事件
            InitHalconView();                 // 创建 Halcon 显示控件和 ROI 适配器
            displayFitRequired = true;
            RefreshDisplay();                 // 显示图像（自适应）
            RefreshMatrixState();             // 更新矩阵状态文本
        }

        /// <summary>
        /// 强制刷新所有数据绑定（将 DataContext 切换以触发重新绑定）。
        /// </summary>
        public void RefreshBindings()
        {
            if (viewModel == null) return;
            DataContext = null;
            DataContext = viewModel;
        }

        /// <summary>
        /// 框架的“运行”入口，调用插件的 ExecuteObj 并返回结构化结果。
        /// </summary>
        public PluginViewActionResult RunFromHost()
        {
            Stopwatch watch = Stopwatch.StartNew();
            bool success = false;
            try
            {
                BeforeRun();                                    // 提交绑定、保存 ROI、同步参数
                success = PluginObject != null && PluginObject.ExecuteObj();
                watch.Stop();
                AfterRun(success, watch.ElapsedMilliseconds);   // 同步模型、更新界面状态
                return success
                    ? PluginViewActionResult.Ok(GetParamMessage("执行完成"), watch.ElapsedMilliseconds)
                    : PluginViewActionResult.Fail(GetParamMessage("执行失败"), watch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                if (watch.IsRunning) watch.Stop();
                return PluginViewActionResult.Fail("执行异常：" + ex.Message, watch.ElapsedMilliseconds);
            }
        }

        /// <summary>
        /// 框架的“保存”入口，调用插件的 saveParams 并返回结构化结果。
        /// </summary>
        public PluginViewActionResult SaveFromHost(string filePath)
        {
            Stopwatch watch = Stopwatch.StartNew();
            bool success = false;
            try
            {
                BeforeSave();
                if (PluginObject != null)
                {
                    PluginObject.moduleInfo.filePath = filePath;
                    if (PluginObject.PluginParams != null) PluginObject.PluginParams.filePath = filePath;
                    success = PluginObject.saveParams();
                }
                else if (PluginParams != null)
                {
                    PluginParams.filePath = filePath;
                    success = PluginParams.Save();
                }

                watch.Stop();
                AfterSave(success);
                return success
                    ? PluginViewActionResult.Ok("参数已保存，矩阵跟随参数XML保存", watch.ElapsedMilliseconds)
                    : PluginViewActionResult.Fail("参数保存失败", watch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                if (watch.IsRunning) watch.Stop();
                return PluginViewActionResult.Fail("保存异常：" + ex.Message, watch.ElapsedMilliseconds);
            }
        }

        /// <summary>
        /// 框架关闭此视图时调用，标记为 Host 触发的关闭并释放资源。
        /// </summary>
        public void CloseFromHost()
        {
            closingFromHost = true;
            BeforeClose();
            DisposeView();
        }

        // ==================== 框架生命周期钩子 ====================
        public void BeforeRun()
        {
            CommitBindings();          // 强制刷新所有绑定，确保编辑内容传入 ViewModel
            SaveCurrentViewRoi();      // 保存当前 ROI 矩形坐标
            SyncModelToParam();        // 将 ViewModel 中修改后的值写回参数对象
            RefreshMatrixState();      // 刷新矩阵状态（保证参数最新）
        }

        public void AfterRun(bool success, long elapsedMs)
        {
            SyncParamToModel();        // 将参数对象的结果写回 ViewModel（如转换结果、消息）
            if (viewModel != null) viewModel.Model.ResultStateText = success ? "成功" : "失败";
            RefreshDisplay(false);     // 重绘图像（保留视图位置）
        }

        public void BeforeSave()
        {
            CommitBindings();
            SaveCurrentViewRoi();
            SyncModelToParam();
        }

        public void AfterSave(bool success)
        {
            SyncParamToModel();
            if (viewModel != null) viewModel.Model.ResultStateText = success ? "保存成功" : "保存失败";
            RefreshBindings();         // 完全刷新绑定，确保保存后的状态反映在 UI
        }

        public void BeforeClose()
        {
            CommitBindings();
            SaveCurrentViewRoi();
            SyncModelToParam();
        }

        // 按钮点击：提交绑定
        public void Btn_RUN_Click(object sender, EventArgs e) { CommitBindings(); }
        public void Btn_Save_Click(object sender, EventArgs e) { CommitBindings(); }
        public void Btn_Close_Click(object sender, EventArgs e) { CommitBindings(); }

        // ==================== 初始化与资源管理 ====================

        /// <summary>
        /// 创建或替换 ViewModel，并订阅其交互事件。
        /// </summary>
        private void BindViewModel(ParamsCACRunCalibration param)
        {
            // 清理旧 ViewModel 的事件
            if (viewModel != null)
            {
                viewModel.GenerateMatrixRequested -= OnGenerateMatrixRequested;
                viewModel.CheckMatrixRequested -= OnCheckMatrixRequested;
                viewModel.TransformPointRequested -= OnTransformPointRequested;
                viewModel.ReadRoiRequested -= OnReadRoiRequested;
                viewModel.Dispose();
            }

            viewModel = new CACRunCalibrationViewModel(param);
            viewModel.GenerateMatrixRequested += OnGenerateMatrixRequested;
            viewModel.CheckMatrixRequested += OnCheckMatrixRequested;
            viewModel.TransformPointRequested += OnTransformPointRequested;
            viewModel.ReadRoiRequested += OnReadRoiRequested;
            DataContext = viewModel;
        }

        /// <summary>
        /// 控件加载时，初始化 Halcon 视图并刷新。
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            disposed = false;
            InitHalconView();
            displayFitRequired = true;
            RefreshDisplay();
            RefreshMatrixState();
        }

        /// <summary>
        /// 控件卸载时，若由 Host 关闭则释放所有资源。
        /// </summary>
        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            if (!closingFromHost) return;
            DisposeView();
        }

        /// <summary>
        /// 创建 Halcon 控件和 ROI 适配器，订阅重绘、鼠标事件。
        /// </summary>
        private void InitHalconView()
        {
            if (roiAdapter != null) return;

            halconControl = new HWindowControl { Dock = WinForms.DockStyle.Fill, BackColor = System.Drawing.Color.FromArgb(2, 6, 11) };
            MainHost.Child = halconControl;
            roiAdapter = new HalconViewRoiAdapter(halconControl, viewModel == null || viewModel.Model.ShowViewRoi);
            roiAdapter.Redraw += DrawOverlay;                         // 每次重绘时叠加二维码轮廓、点标号等
            roiAdapter.RoiChanged += OnViewRoiChanged;                // ROI 变动同步回 ViewModel
            halconControl.HMouseMove += OnMainMouseMove;              // 鼠标移动显示灰度值
            halconControl.MouseDoubleClick += new WinForms.MouseEventHandler((sender, e) => { roiAdapter.FitImage(); }); // 双击自适应窗口
        }

        // ==================== 图像加载 ====================

        /// <summary>
        /// 打开文件对话框加载图像，更新到参数对象并刷新显示。
        /// </summary>
        private void OnLoadImageClick(object sender, RoutedEventArgs e)
        {
            WinForms.OpenFileDialog dialog = new WinForms.OpenFileDialog();
            dialog.Title = "选择图像";
            dialog.Filter = "Image Files|*.bmp;*.png;*.jpg;*.jpeg;*.tif;*.tiff|All Files|*.*";
            dialog.Multiselect = false;
            if (dialog.ShowDialog() != WinForms.DialogResult.OK) return;

            ParamsCACRunCalibration param = PluginParams as ParamsCACRunCalibration;
            if (param == null) return;

            HObject newImage = null;
            try
            {
                HOperatorSet.ReadImage(out newImage, dialog.FileName);
                if (ownedLoadedImage != null)
                {
                    ownedLoadedImage.Dispose();
                    ownedLoadedImage = null;
                }
                param.ho_Image?.Dispose();
                ownedLoadedImage = newImage;
                param.ho_Image = newImage;
                viewModel.UpdateImageState(true, dialog.FileName);
                viewModel.UpdateRunMessage(true, "图像已加载：" + Path.GetFileName(dialog.FileName));
                displayFitRequired = true;
                RefreshDisplay();
            }
            catch (Exception ex)
            {
                if (newImage != null) newImage.Dispose();
                viewModel.UpdateImageState(false, string.Empty);
                viewModel.UpdateRunMessage(false, "图像加载失败：" + ex.Message);
            }
        }

        // ==================== 显示与刷新 ====================

        /// <summary>
        /// 使用当前 fit 要求刷新图像显示。
        /// </summary>
        private void RefreshDisplay()
        {
            RefreshDisplay(displayFitRequired);
        }

        /// <summary>
        /// 显示图像，若 fit 为 true 则自适应窗口，同时应用 ROI 并重绘。
        /// </summary>
        private void RefreshDisplay(bool fit)
        {
            ParamsCACRunCalibration param = PluginParams as ParamsCACRunCalibration;
            if (param == null || roiAdapter == null) return;
            if (param.ho_Image == null || !param.ho_Image.IsInitialized())
            {
                viewModel.UpdateImageState(false, string.Empty);
                return;
            }

            roiAdapter.SetRoiOperate(viewModel.Model.ShowViewRoi);   // 是否允许 ROI 交互
            roiAdapter.ShowImage(param.ho_Image, fit);
            displayFitRequired = false;
            ApplyRoiToView();          // 将 ViewModel 中的 ROI 坐标设置到视图
            roiAdapter.Repaint();      // 触发重绘，进而调用 DrawOverlay
        }

        /// <summary>
        /// 根据 ViewModel 的 ShowViewRoi 状态，显示或清除视图中的矩形 ROI。
        /// </summary>
        private void ApplyRoiToView()
        {
            if (roiAdapter == null || viewModel == null) return;

            if (viewModel.Model.ShowViewRoi)
            {
                roiAdapter.EnsureRectangleRoi(viewModel.Model.RoiRow1, viewModel.Model.RoiCol1,
                                              viewModel.Model.RoiRow2, viewModel.Model.RoiCol2);
            }
            else
            {
                roiAdapter.ClearRoi();
            }
        }

        /// <summary>
        /// 检查矩阵有效性并更新 ViewModel 中的矩阵状态与点表。
        /// </summary>
        private void RefreshMatrixState()
        {
            ParamsCACRunCalibration param = PluginParams as ParamsCACRunCalibration;
            if (param == null || viewModel == null) return;

            bool ok = engine.HasValidMatrix(param, out string message);
            viewModel.UpdateMatrixState(ok, message);
            viewModel.LoadFromParams();   // 将参数中的点表、矩阵等同步到 ViewModel
        }

        // ==================== 绑定提交与参数同步 ====================

        /// <summary>
        /// 提交所有绑定：先移走焦点触发 LostFocus，再清除键盘焦点确保所有输入完成。
        /// </summary>
        private void CommitBindings()
        {
            FocusManager.SetFocusedElement(this, this);
            Keyboard.ClearFocus();
        }

        /// <summary>
        /// 将 ViewModel 的当前值写入参数对象（通常用于运行、保存前）。
        /// </summary>
        private void SyncModelToParam()
        {
            if (PluginParams is ParamsCACRunCalibration && viewModel != null) viewModel.CommitToParams();
        }

        /// <summary>
        /// 将参数对象的最新结果同步回 ViewModel（运行、保存后更新界面）。
        /// </summary>
        private void SyncParamToModel()
        {
            if (PluginParams is ParamsCACRunCalibration && viewModel != null) viewModel.LoadFromParams();
        }

        // ==================== 标定操作 ====================

        /// <summary>
        /// 异步生成矩阵：在后台线程中检测二维码并拟合矩阵，完成后更新界面。
        /// </summary>
        private async Task GenerateMatrixAsync()
        {
            ParamsCACRunCalibration param = PluginParams as ParamsCACRunCalibration;
            if (param == null || viewModel == null || viewModel.Model.IsBusy) return;

            CommitBindings();
            SaveCurrentViewRoi();
            SyncModelToParam();
            viewModel.Model.IsBusy = true;
            viewModel.UpdateRunMessage(false, "正在识别二维码并生成矩阵...");

            List<CalibrationPointRow> rows = new List<CalibrationPointRow>();
            bool ok = false;
            string message = string.Empty;
            try
            {
                ok = await Task.Run(() => engine.GenerateMatrix(param, rows, out message));
            }
            catch (Exception ex)
            {
                ok = false;
                message = "矩阵报错：" + ex.Message;
            }
            finally
            {
                viewModel.Model.IsBusy = false;
            }

            viewModel.UpdatePointRows(rows);                // 更新点表
            viewModel.UpdateMatrixState(ok, message);
            viewModel.LoadFromParams();                     // 将矩阵数据、状态同步到 ViewModel
            viewModel.UpdateRunMessage(ok, message);
            RefreshDisplay(false);                          // 重绘以显示二维码轮廓
        }

        /// <summary>
        /// 根据当前勾选点重新拟合矩阵（不重新检测二维码）。
        /// </summary>
        private void OnRefitSelectedClick(object sender, RoutedEventArgs e)
        {
            ParamsCACRunCalibration param = PluginParams as ParamsCACRunCalibration;
            if (param == null || viewModel == null) return;

            CommitBindings();
            SyncModelToParam();
            bool ok = engine.RefitMatrixFromRows(param, viewModel.PointRows.ToList(), out string message);
            viewModel.UpdateMatrixState(ok, message);
            viewModel.LoadFromParams();
            viewModel.UpdateRunMessage(ok, message);
            RefreshDisplay(false);
        }

        /// <summary>
        /// 手动触发矩阵有效性检查。
        /// </summary>
        private void CheckMatrix()
        {
            RefreshMatrixState();
            RefreshDisplay(false);
        }

        /// <summary>
        /// 外部执行完成后的界面刷新入口。
        /// 
        /// 调用来源：
        /// IPlugin.ExecuteObj() -> IPlugin.ExecuteCompleted -> WPFFormPluginBase.OnPluginExecuteCompleted()
        /// -> IPluginWpfView.OnPluginExecutedFromOutside()
        /// 
        /// 刷新内容：
        /// 1. 从 ParamsCACRunCalibration 重新读取执行输出、矩阵状态、转换结果。
        /// 2. 刷新界面绑定属性。
        /// 3. 重新显示当前参数里的 ho_Image。
        /// 4. 重绘二维码 XLD、标定点、ROI、输入像素点等叠加层。
        /// </summary>
        public void OnPluginExecutedFromOutside(PluginExecuteCompletedEventArgs e)
        {
            if (e == null) return;
            if (PluginParams == null) return;
            //先判断参数是否一致
            if (!ReferenceEquals(e.Param, PluginParams)) return;

            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new Action(() => OnPluginExecutedFromOutside(e)));
                return;
            }

            if (viewModel == null) return;

            RefreshFromPluginParams();

            if (viewModel.Model != null)
            {
                viewModel.Model.ResultStateText = e.Success ? "成功" : "失败";
                if (!string.IsNullOrWhiteSpace(e.Message))
                {
                    viewModel.Model.RunMessage = e.Message;
                }
            }

            RefreshBindings();
        }


        public void RefreshFromPluginParams()
        {
            if (viewModel == null) return;

            viewModel.LoadFromParams();
            RefreshMatrixState();
            RefreshDisplay(true);
        }


        /// <summary>
        /// 执行一次像素到物理坐标的转换（用于界面上的测试转换按钮）。
        /// </summary>
        private void TransformPoint()
        {
            ParamsCACRunCalibration param = PluginParams as ParamsCACRunCalibration;
            if (param == null || viewModel == null) return;

            CommitBindings();
            SyncModelToParam();
            bool ok = engine.TryPixelToBoard(param, viewModel.Model.InputPixelRow, viewModel.Model.InputPixelCol, out double x, out double y, out string message);
            param.m_fOutputBoardX = x;
            param.m_fOutputBoardY = y;
            param.m_bTransformSuccess = ok;
            param.m_bRunSuccess = ok;
            param.m_strRunMessage = ok ? message : "转换失败：" + message;
            SyncParamToModel();
            viewModel.UpdateRunMessage(ok, param.m_strRunMessage);
            RefreshDisplay(false);
        }

        /// <summary>
        /// 从视图获取当前矩形 ROI 的坐标并写入 ViewModel 和参数。
        /// </summary>
        private void ReadRoiFromView()
        {
            if (roiAdapter == null || viewModel == null) return;
            if (!roiAdapter.TryGetRectangleRoi(out double row1, out double col1, out double row2, out double col2)) return;

            viewModel.Model.RoiRow1 = row1;
            viewModel.Model.RoiCol1 = col1;
            viewModel.Model.RoiRow2 = row2;
            viewModel.Model.RoiCol2 = col2;
            SyncModelToParam();
            viewModel.UpdateRunMessage(true, "已读取ViewROI矩形坐标");
            RefreshDisplay(false);
        }

        /// <summary>
        /// 保存当前视图中的 ROI 坐标到 ViewModel。
        /// </summary>
        private void SaveCurrentViewRoi()
        {
            if (roiAdapter == null || viewModel == null || !viewModel.Model.ShowViewRoi) return;
            if (!roiAdapter.TryGetRectangleRoi(out double row1, out double col1, out double row2, out double col2)) return;

            viewModel.Model.RoiRow1 = row1;
            viewModel.Model.RoiCol1 = col1;
            viewModel.Model.RoiRow2 = row2;
            viewModel.Model.RoiCol2 = col2;
        }

        // ==================== 事件转发 ====================

        private void OnGenerateMatrixRequested(object sender, EventArgs e) { _ = GenerateMatrixAsync(); }
        private void OnCheckMatrixRequested(object sender, EventArgs e) { CheckMatrix(); }
        private void OnTransformPointRequested(object sender, EventArgs e) { TransformPoint(); }
        private void OnReadRoiRequested(object sender, EventArgs e) { ReadRoiFromView(); }
        private void OnGenerateMatrixClick(object sender, RoutedEventArgs e) { _ = GenerateMatrixAsync(); }
        private void OnCheckMatrixClick(object sender, RoutedEventArgs e) { CheckMatrix(); }
        private void OnTransformClick(object sender, RoutedEventArgs e) { TransformPoint(); }
        private void OnReadRoiClick(object sender, RoutedEventArgs e) { ReadRoiFromView(); }

        /// <summary>
        /// ROI 应用按钮：将界面参数中的 ROI 坐标应用到 Halcon 视图。
        /// </summary>
        private void OnApplyRoiClick(object sender, RoutedEventArgs e)
        {
            CommitBindings();
            SyncModelToParam();
            ApplyRoiToView();
            RefreshDisplay(false);
        }

        /// <summary>
        /// 显示/隐藏 ROI 的复选框变化时，刷新视图。
        /// </summary>
        private void OnShowRoiChanged(object sender, RoutedEventArgs e)
        {
            if (viewModel == null) return;
            CommitBindings();
            SyncModelToParam();
            RefreshDisplay(false);
        }

        /// <summary>
        /// ROI 参数（坐标）变化时，提交绑定并同步参数。
        /// </summary>
        private void OnRoiParameterChanged(object sender, RoutedEventArgs e)
        {
            if (viewModel == null) return;
            CommitBindings();
            SyncModelToParam();
        }

        /// <summary>
        /// 当用户在视图中拖动 ROI 时，将新坐标保存到 ViewModel。
        /// 使用 syncingRoi 标志避免循环更新。
        /// </summary>
        private void OnViewRoiChanged(int value)
        {
            if (syncingRoi) return;
            try
            {
                syncingRoi = true;
                SaveCurrentViewRoi();
                SyncModelToParam();
            }
            finally
            {
                syncingRoi = false;
            }
        }

        /// <summary>
        /// Halcon 窗口重绘时的叠加绘制：显示二维码轮廓、点十字线及编号、转换成功时的高亮十字。
        /// </summary>
        private void DrawOverlay()
        {
            ParamsCACRunCalibration param = PluginParams as ParamsCACRunCalibration;
            if (param == null || roiAdapter == null) return;

            HWindow window = roiAdapter.HalconWindow;
            try
            {
                window.SetDraw("margin");
                window.SetLineWidth(2);

                // 绘制二维码符号轮廓
                if (param.m_ho_OutQrXlds != null && param.m_ho_OutQrXlds.IsInitialized())
                {
                    window.SetColor("cyan");
                    window.DispObj(param.m_ho_OutQrXlds);
                }

                // 绘制每个标定点（绿色表示参与拟合，红色表示未使用）
                if (viewModel != null)
                {
                    foreach (CalibrationPointRow point in viewModel.PointRows)
                    {
                        window.SetColor(point.Used ? "green" : "red");
                        window.DispCross(point.ImageRow, point.ImageColumn, 18, 0.785398);
                        window.SetTposition((int)Math.Round(point.ImageRow + 12), (int)Math.Round(point.ImageColumn + 12));
                        window.WriteString(point.SerialNumber.ToString(CultureInfo.InvariantCulture));
                    }
                }

                // 如果转换成功，绘制输入像素坐标的高亮十字
                if (param.m_bTransformSuccess)
                {
                    window.SetColor("yellow");
                    window.DispCross(param.m_fInputPixelRow, param.m_fInputPixelCol, 28, 0.785398);
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// 鼠标在图像上移动时，显示行/列/灰度值。
        /// </summary>
        private void OnMainMouseMove(object sender, HMouseEventArgs e)
        {
            if (roiAdapter == null || viewModel == null) return;
            string text = roiAdapter.GetMouseText(e);
            if (!string.IsNullOrWhiteSpace(text)) viewModel.Model.RunMessage = text;
        }

        /// <summary>
        /// 获取参数中的运行消息，若为空则使用回退字符串。
        /// </summary>
        private string GetParamMessage(string fallback)
        {
            ParamsCACRunCalibration param = PluginParams as ParamsCACRunCalibration;
            return param == null || string.IsNullOrWhiteSpace(param.m_strRunMessage) ? fallback : param.m_strRunMessage;
        }

        /// <summary>
        /// 递归遍历可视树，对所有输入控件强制更新绑定源（已废弃，保留仅为兼容）。
        /// </summary>
        private static void UpdateBindingSources(DependencyObject parent)
        {
            // ... 代码略
        }

        /// <summary>
        /// 释放所有资源：取消事件订阅、释放 Halcon 控件和 ROI 适配器、清除图像引用。
        /// </summary>
        private void DisposeView()
        {
            if (disposed) return;
            disposed = true;

            try
            {
                if (halconControl != null) halconControl.HMouseMove -= OnMainMouseMove;
                if (roiAdapter != null)
                {
                    roiAdapter.Redraw -= DrawOverlay;
                    roiAdapter.RoiChanged -= OnViewRoiChanged;
                    roiAdapter.Dispose();
                }
            }
            catch { }

            roiAdapter = null;
            if (halconControl != null) halconControl.Dispose();
            halconControl = null;
            MainHost.Child = null;

            if (viewModel != null)
            {
                viewModel.GenerateMatrixRequested -= OnGenerateMatrixRequested;
                viewModel.CheckMatrixRequested -= OnCheckMatrixRequested;
                viewModel.TransformPointRequested -= OnTransformPointRequested;
                viewModel.ReadRoiRequested -= OnReadRoiRequested;
            }

            ownedLoadedImage = null;
        }
    }
}
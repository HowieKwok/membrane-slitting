using System;
using System.Diagnostics;
using System.Windows.Input;
using Frame_Core;
using Frame_Core.pluginParams;
using Plugin.CombineLineCross.Services;
using ViewROI;

namespace Plugin.CombineLineCross.ViewModels
{
    /// <summary>
    /// 页面业务层：只保存状态、订阅适配器事件、调用插件执行。
    /// 不持有 WPF 控件，不创建 HWindowControl，关闭时负责解除自己的订阅。
    /// </summary>
    public sealed class ToolVm : BindableBase, IPluginWpfView, IDisposable
    {
        private CombineLineCrossObj pluginObject;
        private ParamsCombPoint pluginParams;
        private HalconViewRoiAdapter display;
        private string statusText = "等待输入线段";
        private string intersectionText = "交点：--";
        private string angleText = "夹角：--";
        private string line1Text = "线段 1：--";
        private string line2Text = "线段 2：--";
        private string mouseText = "行：--  列：--";
        private bool isEditing = true;
        private bool isSynchronizing;
        private bool isDisposed;

        public ToolVm()
        {
            RefreshCommand = new RelayCommand<object>(_ => RefreshFromParameters(true));
            LoadLocalImageCommand = new RelayCommand<object>(_ => LoadLocalImage());
            FitImageCommand = new RelayCommand<object>(_ => FitImage());
            ToggleEditCommand = new RelayCommand<object>(_ => IsEditing = !IsEditing);
            RunCommand = new RelayCommand<object>(_ => RunFromHost());
        }

        public ICommand RefreshCommand { get; private set; }
        public ICommand LoadLocalImageCommand { get; private set; }
        public ICommand FitImageCommand { get; private set; }
        public ICommand ToggleEditCommand { get; private set; }
        public ICommand RunCommand { get; private set; }

        public IPlugin PluginObject { get { return pluginObject; } }
        public PluginParamsBase PluginParams { get { return pluginParams; } }

        public string StatusText { get { return statusText; } private set { SetProperty(ref statusText, value); } }
        public string IntersectionText { get { return intersectionText; } private set { SetProperty(ref intersectionText, value); } }
        public string AngleText { get { return angleText; } private set { SetProperty(ref angleText, value); } }
        public string Line1Text { get { return line1Text; } private set { SetProperty(ref line1Text, value); } }
        public string Line2Text { get { return line2Text; } private set { SetProperty(ref line2Text, value); } }
        public string MouseText { get { return mouseText; } private set { SetProperty(ref mouseText, value); } }

        /// <summary>直接映射参数对象的启用开关，不另存界面状态。</summary>
        public bool IsPluginEnabled
        {
            get { return pluginParams == null || pluginParams.m_bIsEnable; }
            set
            {
                if (isDisposed || pluginParams == null || pluginParams.m_bIsEnable == value) return;
                pluginParams.m_bIsEnable = value;
                OnPropertyChanged();
            }
        }

        // 端点数值直接映射到既有 LineInfo，避免页面再维护一份重复参数。
        public double Line1RowBegin { get { return GetLineValue(1, 0); } set { SetLineValue(1, 0, value); } }
        public double Line1ColBegin { get { return GetLineValue(1, 1); } set { SetLineValue(1, 1, value); } }
        public double Line1RowEnd { get { return GetLineValue(1, 2); } set { SetLineValue(1, 2, value); } }
        public double Line1ColEnd { get { return GetLineValue(1, 3); } set { SetLineValue(1, 3, value); } }
        public double Line2RowBegin { get { return GetLineValue(2, 0); } set { SetLineValue(2, 0, value); } }
        public double Line2ColBegin { get { return GetLineValue(2, 1); } set { SetLineValue(2, 1, value); } }
        public double Line2RowEnd { get { return GetLineValue(2, 2); } set { SetLineValue(2, 2, value); } }
        public double Line2ColEnd { get { return GetLineValue(2, 3); } set { SetLineValue(2, 3, value); } }

        public bool IsEditing
        {
            get { return isEditing; }
            set
            {
                if (!SetProperty(ref isEditing, value))
                {
                    return;
                }
                if (display != null && !display.IsDisposed)
                {
                    display.SetRoiEditing(value);
                }
            }
        }

        /// <summary>由页面在 HWindowControl 创建后注入适配器。</summary>
        public void AttachAdapter(HalconViewRoiAdapter adapter)
        {
            if (adapter == null || isDisposed)
            {
                return;
            }

            DetachDisplay();
            display = adapter;
            display.RoiChanged += OnRoiChanged;
            display.MouseMoved += OnMouseMoved;
            display.SetRoiEditing(IsEditing);
            RefreshFromParameters(false);
        }

        public void BindPlugin(IPlugin plugin)
        {
            if (isDisposed) return;
            pluginObject = plugin as CombineLineCrossObj ?? new CombineLineCrossObj();
            pluginParams = pluginObject.PluginParams as ParamsCombPoint;
            if (pluginParams == null)
            {
                pluginParams = new ParamsCombPoint();
                pluginObject.PluginParams = pluginParams;
            }

            RefreshFromParameters(false);
        }

        public void RefreshBindings()
        {
            OnPropertyChanged(string.Empty);
        }

        public PluginViewActionResult RunFromHost()
        {
            if (isDisposed) return PluginViewActionResult.Fail("页面已关闭");
            EnsureParameters();
            if (display != null && !display.IsDisposed) display.SynchronizeParametersFromRois();

            Stopwatch watch = Stopwatch.StartNew();
            try
            {
                pluginObject.ExecuteObj();
                watch.Stop();
                // 运行状态只以参数对象内的 m_bRunSuccess 为准。
                bool success = pluginParams.m_bRunSuccess;
                UpdateOutputText();
                StatusText = success ? "运行完成" : pluginParams.m_strRunMessage;
                // 当前页面执行后立即重绘；不依赖 ExecuteCompleted 回调。
                if (display != null && !display.IsDisposed) display.Refresh();
                return success
                    ? PluginViewActionResult.Ok(StatusText, watch.ElapsedMilliseconds)
                    : PluginViewActionResult.Fail(StatusText, watch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                if (watch.IsRunning) watch.Stop();
                StatusText = "运行异常：" + ex.Message;
                return PluginViewActionResult.Fail(StatusText, watch.ElapsedMilliseconds);
            }
        }

        public PluginViewActionResult SaveFromHost(string filePath)
        {
            if (isDisposed) return PluginViewActionResult.Fail("页面已关闭");
            EnsureParameters();
            try
            {
                pluginObject.moduleInfo.filePath = filePath;
                pluginParams.filePath = filePath;
                bool success = pluginObject.saveParams();
                StatusText = success ? "参数已保存" : "参数保存失败";
                return success ? PluginViewActionResult.Ok(StatusText) : PluginViewActionResult.Fail(StatusText);
            }
            catch (Exception ex)
            {
                StatusText = "保存异常：" + ex.Message;
                return PluginViewActionResult.Fail(StatusText);
            }
        }

        public void CloseFromHost()
        {
            Dispose();
        }

        public void BeforeRun()
        {
            if (display != null && !display.IsDisposed) display.SynchronizeParametersFromRois();
        }

        public void AfterRun(bool success, long elapsedMs)
        {
            UpdateOutputText();
            StatusText = pluginParams != null && pluginParams.m_bRunSuccess
                ? "运行完成"
                : (pluginParams == null ? "运行失败" : pluginParams.m_strRunMessage);
         
        }

        public void BeforeSave()
        {
            if (display != null && !display.IsDisposed) display.SynchronizeParametersFromRois();
        }

        public void AfterSave(bool success)
        {
            StatusText = success ? "参数已保存" : "参数保存失败";
        }

        public void BeforeClose()
        {
            Dispose();
        }

        public void OnPluginExecutedFromOutside(PluginExecuteCompletedEventArgs e)
        {
            if (e == null || pluginParams == null) return;
            if (!ReferenceEquals(e.Param, pluginParams)) return;
            RefreshFromParameters(false);
            AfterRun(e.Success, e.ElapsedMs);
        }

        /// <summary>
        /// 同步参数对象中的图像和两条线段。
        /// </summary>
        private void RefreshFromParameters(bool invalidateRunResult)
        {
            if (isDisposed || pluginParams == null)
            {
                return;
            }

            UpdateLineText();
            OnPropertyChanged(nameof(IsPluginEnabled));
            RefreshLineInputBindings();
            UpdateOutputText();
            if (display == null || display.IsDisposed)
            {
                return;
            }

            isSynchronizing = true;
            try
            {
                if (invalidateRunResult)
                {
                    // 新的外部线段尚未重新计算，旧交点和夹角不得继续显示。
                    pluginParams.m_bRunSuccess = false;
                    pluginParams.m_strRunMessage = "外部线段已刷新，请重新运行";
                }
                display.SetBinding(pluginParams);
            }
            finally
            {
                isSynchronizing = false;
            }
            UpdateOutputText();
            StatusText = pluginParams.m_strRunMessage;
        }

        private void FitImage()
        {
            if (display != null && !display.IsDisposed)
            {
                display.FitImage();
                StatusText = "图像已适应窗口";
            }
        }

        private void LoadLocalImage()
        {
            if (display == null || display.IsDisposed)
            {
                StatusText = "图像窗口尚未初始化";
                return;
            }

            StatusText = display.OpenLocalImage() ? "本地图像已加载" : "未加载本地图像";
        }

        private void OnRoiChanged(int eventCode)
        {
            if (isDisposed || isSynchronizing || pluginParams == null || display == null)
            {
                return;
            }

            display.SynchronizeParametersFromRois();
            UpdateLineText();
            RefreshLineInputBindings();
            if (eventCode == ROIController.EVENT_UPDATE_ROI)
            {
                // 线段已经变更，上一轮运行结果不能再用于显示或发布。
                pluginParams.m_bRunSuccess = false;
                pluginParams.m_strRunMessage = "线段已调整，请重新运行";
                UpdateOutputText();
                StatusText = pluginParams.m_strRunMessage;
                display.Refresh();
            }
        }

        private void OnMouseMoved(double row, double col)
        {
            if (!isDisposed)
            {
                MouseText = string.Format("行：{0:F1}  列：{1:F1}", row, col);
            }
        }

        private void UpdateLineText()
        {
            if (pluginParams == null || pluginParams.LineInfo1 == null || pluginParams.LineInfo2 == null)
            {
                return;
            }

            Line1Text = FormatLine("线段 1", pluginParams.LineInfo1);
            Line2Text = FormatLine("线段 2", pluginParams.LineInfo2);
        }

        private void UpdateOutputText()
        {
            if (pluginParams == null)
            {
                return;
            }

            AngleText = !pluginParams.m_bRunSuccess || pluginParams.hv_LineAngle == null || pluginParams.hv_LineAngle.Length == 0
                ? "夹角：--"
                : string.Format("夹角：{0:F3} 度", pluginParams.hv_LineAngle[0].D);
            IntersectionText = pluginParams.m_bRunSuccess &&
                               pluginParams.hv_IntersectionRow != null && pluginParams.hv_IntersectionRow.Length > 0 &&
                               pluginParams.hv_IntersectionColumn != null && pluginParams.hv_IntersectionColumn.Length > 0
                ? string.Format("交点：R={0:F3}  C={1:F3}", pluginParams.hv_IntersectionRow[0].D, pluginParams.hv_IntersectionColumn[0].D)
                : "交点：无唯一交点";
        }

        private static string FormatLine(string name, LineInfo line)
        {
            return string.Format("{0}：({1:F1}, {2:F1}) -> ({3:F1}, {4:F1})",
                                 name, line.RowBegin, line.ColBegin, line.RowEnd, line.ColEnd);
        }

        private double GetLineValue(int lineIndex, int pointIndex)
        {
            LineInfo line = GetLineInfo(lineIndex);
            if (line == null) return 0;

            switch (pointIndex)
            {
                case 0: return line.RowBegin;
                case 1: return line.ColBegin;
                case 2: return line.RowEnd;
                default: return line.ColEnd;
            }
        }

        /// <summary>数值控件修改端点后，同步既有参数和对应的 ViewROI 线段。</summary>
        private void SetLineValue(int lineIndex, int pointIndex, double value)
        {
            if (isDisposed) return;

            EnsureParameters();
            LineInfo line = GetLineInfo(lineIndex);
            if (line == null || GetLineValue(lineIndex, pointIndex) == value) return;

            switch (pointIndex)
            {
                case 0: line.RowBegin = value; break;
                case 1: line.ColBegin = value; break;
                case 2: line.RowEnd = value; break;
                default: line.ColEnd = value; break;
            }

            // 手工调整后的交点和夹角已过期，运行成功标志必须清除。
            pluginParams.m_bRunSuccess = false;
            pluginParams.m_strRunMessage = "线段参数已调整，请重新运行";
            isSynchronizing = true;
            try
            {
                if (display != null && !display.IsDisposed) display.SetBinding(pluginParams);
            }
            finally
            {
                isSynchronizing = false;
            }

            UpdateLineText();
            UpdateOutputText();
            StatusText = pluginParams.m_strRunMessage;
        }

        private LineInfo GetLineInfo(int lineIndex)
        {
            if (pluginParams == null) return null;
            return lineIndex == 1 ? pluginParams.LineInfo1 : pluginParams.LineInfo2;
        }

        private void RefreshLineInputBindings()
        {
            OnPropertyChanged(nameof(Line1RowBegin));
            OnPropertyChanged(nameof(Line1ColBegin));
            OnPropertyChanged(nameof(Line1RowEnd));
            OnPropertyChanged(nameof(Line1ColEnd));
            OnPropertyChanged(nameof(Line2RowBegin));
            OnPropertyChanged(nameof(Line2ColBegin));
            OnPropertyChanged(nameof(Line2RowEnd));
            OnPropertyChanged(nameof(Line2ColEnd));
        }

        private void EnsureParameters()
        {
            if (pluginObject == null)
            {
                pluginObject = new CombineLineCrossObj();
            }
            if (pluginParams == null)
            {
                pluginParams = pluginObject.PluginParams as ParamsCombPoint ?? new ParamsCombPoint();
                pluginObject.PluginParams = pluginParams;
            }
        }

        private void DetachDisplay()
        {
            if (display == null)
            {
                return;
            }

            display.RoiChanged -= OnRoiChanged;
            display.MouseMoved -= OnMouseMoved;
            display = null;
        }

        public void Dispose()
        {
            if (isDisposed)
            {
                return;
            }

            isDisposed = true;
            DetachDisplay();
            RefreshCommand = null;
            LoadLocalImageCommand = null;
            FitImageCommand = null;
            ToggleEditCommand = null;
            RunCommand = null;
            pluginObject = null;
            pluginParams = null;
            StatusText = string.Empty;
            IntersectionText = string.Empty;
            AngleText = string.Empty;
            Line1Text = string.Empty;
            Line2Text = string.Empty;
            MouseText = string.Empty;
            ClearPropertyChangedSubscribers();
        }
    }
}

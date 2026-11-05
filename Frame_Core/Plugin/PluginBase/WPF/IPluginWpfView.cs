using System;

namespace Frame_Core
{
    /// <summary>
    /// WPF 插件页面动作执行结果。
    /// 宿主窗口只读取这里的状态和耗时，用于统一刷新底部状态栏。
    /// </summary>
    public sealed class PluginViewActionResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public long ElapsedMs { get; set; }

        public static PluginViewActionResult Ok(string message, long elapsedMs = 0)
        {
            return new PluginViewActionResult { Success = true, Message = message, ElapsedMs = elapsedMs };
        }

        public static PluginViewActionResult Fail(string message, long elapsedMs = 0)
        {
            return new PluginViewActionResult { Success = false, Message = message, ElapsedMs = elapsedMs };
        }
    }

    /// <summary>
    /// WPF插件页面统一扩展接口。
    /// WPFFormPluginBase 只做按钮转发、耗时和状态显示；具体运行、保存、关闭释放由插件页面实现。
    /// </summary>
    public interface IPluginWpfView
    {
        IPlugin PluginObject { get; }
        PluginParamsBase PluginParams { get; }

        void BindPlugin(IPlugin plugin);
        void RefreshBindings();

        /// <summary>
        /// 宿主点击“执行”时调用。插件页面内部负责提交界面参数、调用插件执行、刷新结果显示并返回状态。
        /// </summary>
        PluginViewActionResult RunFromHost();

        /// <summary>
        /// 宿主点击“保存”时调用。filePath 由框架计算后传入，插件页面内部负责回写界面参数并调用 saveParams。
        /// </summary>
        PluginViewActionResult SaveFromHost(string filePath);

        /// <summary>
        /// 宿主关闭窗口时调用。只做界面局部资源释放，是否保存由显式保存按钮决定。
        /// </summary>
        void CloseFromHost();

        // 旧生命周期保留一段时间，便于已有 WPF 插件平滑迁移。
        void BeforeRun();
        void AfterRun(bool success, long elapsedMs);
        void BeforeSave();
        void AfterSave(bool success);
        void BeforeClose();


        /// <summary>
        /// 插件对象在窗体外部执行完成后，由 WPFFormPluginBase 调用。
        /// 页面内部应从 PluginParams 重新读取输出并刷新显示。
        /// 典型动作包括：
        /// 1. 刷新运行状态文本。
        /// 2. 刷新矩阵是否有效。
        /// 3. 刷新输出坐标。
        /// 4. 重新绘制图像叠加层和 ROI。
        /// </summary>
        void OnPluginExecutedFromOutside(PluginExecuteCompletedEventArgs e);
    
}
   
}
using Frame_Core;
using Plugin.ReusableMeasureDetect.ViewModels;
using System.Windows;
using System.Windows.Controls;
using WinForms = System.Windows.Forms;

namespace Plugin.ReusableMeasureDetect.Views
{
    /// <summary>
    /// 插件 WPF 视图壳。
    /// 该类只负责初始化 XAML、创建 ViewModel、挂载 HALCON 窗口到 WindowsFormsHost，并把框架生命周期调用转发给 ViewModel。
    /// </summary>
    public partial class ReusableMeasureDetectUser : UserControl, IPluginWpfView
    {
        private readonly ReusableMeasureDetectViewModel vm;

        /// <summary>
        /// 初始化插件界面，建立 DataContext，并把 HWindowControl 放入界面宿主。
        /// </summary>
        public ReusableMeasureDetectUser()
        {
            InitializeComponent();
            vm = new ReusableMeasureDetectViewModel();
            DataContext = vm;
            vm.MainHalconWindow.Dock = WinForms.DockStyle.Fill;
            MainHost.Child = vm.MainHalconWindow;
        }

        /// <summary>
        /// 当前绑定的插件对象，供框架读取。
        /// </summary>
        public IPlugin PluginObject { get { return vm.PluginObject; } }

        /// <summary>
        /// 当前插件参数对象，供框架读取和保存。
        /// </summary>
        public PluginParamsBase PluginParams { get { return vm.PluginParams; } }

        /// <summary>
        /// 框架绑定插件实例时调用，内部交给 ViewModel 完成参数挂载。
        /// </summary>
        public void BindPlugin(IPlugin plugin) { vm.BindPlugin(plugin); }

        /// <summary>
        /// 框架要求刷新界面绑定时调用。
        /// </summary>
        public void RefreshBindings() { vm.RefreshBindings(); }

        /// <summary>
        /// 框架或界面点击运行时调用。
        /// </summary>
        public PluginViewActionResult RunFromHost() { return vm.RunFromHost(); }

        /// <summary>
        /// 框架保存参数时调用。
        /// </summary>
        public PluginViewActionResult SaveFromHost(string filePath) { return vm.SaveFromHost(filePath); }

        /// <summary>
        /// 框架关闭界面时调用。
        /// </summary>
        public void CloseFromHost() { vm.CloseFromHost(); }

        /// <summary>
        /// 框架运行前回调，提交界面参数到插件参数。
        /// </summary>
        public void BeforeRun() { vm.BeforeRun(); }

        /// <summary>
        /// 框架运行后回调，刷新结果和叠加显示。
        /// </summary>
        public void AfterRun(bool success, long elapsedMs) { vm.AfterRun(success, elapsedMs); }

        /// <summary>
        /// 框架保存前回调，提交界面参数到插件参数。
        /// </summary>
        public void BeforeSave() { vm.BeforeSave(); }

        /// <summary>
        /// 框架保存后回调，更新界面状态。
        /// </summary>
        public void AfterSave(bool success) { vm.AfterSave(success); }

        /// <summary>
        /// 框架关闭前回调，释放窗口和 ROI 资源。
        /// </summary>
        public void BeforeClose() { vm.BeforeClose(); }

        /// <summary>
        /// 插件被外部流程运行完成后回调，用最新参数刷新界面。
        /// </summary>
        public void OnPluginExecutedFromOutside(PluginExecuteCompletedEventArgs e) { vm.OnPluginExecutedFromOutside(e); }

        /// <summary>
        /// WPF 控件卸载时释放 HALCON 窗口并解除宿主关系，避免界面关闭后仍持有窗口句柄。
        /// </summary>
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            vm.CloseFromHost();
            MainHost.Child = null;
        }
    }
}

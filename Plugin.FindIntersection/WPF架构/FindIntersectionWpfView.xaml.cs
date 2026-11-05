using System.Windows.Controls;
using Frame_Core;
using Frame_Core.pluginParams;
using Plugin.FindIntersection.ViewModels;

namespace Plugin.FindIntersection
{
    /// <summary>
    /// 两线交点 WPF 页面。
    /// 基类不改的前提下，页面必须实现 IPluginWpfView，供宿主识别。
    /// 这里不写业务逻辑，只把宿主生命周期转发给 ToolVm。
    /// </summary>
    public partial class FindIntersectionWpfView : UserControl, IPluginWpfView
    {
        private readonly ToolVm vm;

        public FindIntersectionWpfView()
        {
            InitializeComponent();
            vm = new ToolVm();
            DataContext = vm;
        }

        public IPlugin PluginObject { get { return vm.PluginObject; } }
        public PluginParamsBase PluginParams { get { return vm.PluginParams; } }

        public void BindPlugin(IPlugin plugin) { vm.BindPlugin(plugin); }
        public void RefreshBindings() { vm.RefreshBindings(); }
        public PluginViewActionResult RunFromHost() { return vm.RunFromHost(); }
        public PluginViewActionResult SaveFromHost(string filePath) { return vm.SaveFromHost(filePath); }
        public void CloseFromHost() { vm.CloseFromHost(); }
        public void BeforeRun() { vm.BeforeRun(); }
        public void AfterRun(bool success, long elapsedMs) { vm.AfterRun(success, elapsedMs); }
        public void BeforeSave() { vm.BeforeSave(); }
        public void AfterSave(bool success) { vm.AfterSave(success); }
        public void BeforeClose() { vm.BeforeClose(); }
        public void OnPluginExecutedFromOutside(PluginExecuteCompletedEventArgs e) { vm.OnPluginExecutedFromOutside(e); }
    }
}

/*
改动说明：
1. 不再把 View 传给 ToolVm。
2. 不再把 XAML 控件开放给 ToolVm。
3. 这个文件只剩宿主接口转发，这是“不改基类”前提下的最薄兼容层。
4. 真正的运行、保存、参数同步、ROI 处理、外部执行刷新都在 ToolVm。
*/

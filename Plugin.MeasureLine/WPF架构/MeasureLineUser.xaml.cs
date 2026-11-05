using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using Frame_Core;
using Plugin.MeasureLine.ViewModels;

namespace Plugin.MeasureLine.WPF架构
{
    /// <summary>
    /// MeasureLineUser.xaml 的交互逻辑
    /// </summary>
    public partial class MeasureLineUser : System.Windows.Controls.UserControl, IPluginWpfView
    {
        private readonly MeasurelineViewModel vm;

        public MeasureLineUser()
        {
            InitializeComponent();
            vm = new MeasurelineViewModel();
            DataContext = vm;
            vm.MainHalconWindow.Dock =System.Windows.Forms.DockStyle.Fill;
            HalconHost.Child = vm.MainHalconWindow;
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

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            vm.CloseFromHost();
            HalconHost.Child = null;
        }
    }
}
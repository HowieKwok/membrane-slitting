using System;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using Frame_Core;
using Frame_Core.pluginParams;
using HalconDotNet;
using Plugin.CombineLineCross.Services;
using Plugin.CombineLineCross.ViewModels;
using Forms = System.Windows.Forms;

namespace Plugin.CombineLineCross
{
    /// <summary>
    /// 页面只负责承载 HWindowControl 并转发生命周期。
    /// 参数同步、ROI 交互和计算均由 ToolVm 与 HalconViewRoiAdapter 处理。
    /// </summary>
    public partial class CombineLineCrossWpfView : UserControl, IPluginWpfView, IDisposable
    {
        private readonly ToolVm vm;
        private HWindowControl halconWindow;
        private HalconViewRoiAdapter adapter;
        private bool disposed;

        public CombineLineCrossWpfView()
        {
            InitializeComponent();
            vm = new ToolVm();
            DataContext = vm;
            Loaded += OnLoaded;
        }

        public IPlugin PluginObject { get { return vm.PluginObject; } }
        public PluginParamsBase PluginParams { get { return vm.PluginParams; } }

        private void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!disposed && adapter == null)
            {
                // 页面只创建和承载原生窗口；所有 ViewROI 行为在 HalconViewRoiAdapter。
                halconWindow = new HWindowControl();
                halconWindow.Dock = Forms.DockStyle.Fill;
                HalconHost.Child = halconWindow;
                adapter = new HalconViewRoiAdapter(halconWindow, true);
                vm.AttachAdapter(adapter);
            }
        }

        public void BindPlugin(IPlugin plugin) { vm.BindPlugin(plugin); }
        public void RefreshBindings() { vm.RefreshBindings(); }
        public PluginViewActionResult RunFromHost() { return vm.RunFromHost(); }
        public PluginViewActionResult SaveFromHost(string filePath) { return vm.SaveFromHost(filePath); }
        public void CloseFromHost() { Dispose(); }
        public void BeforeRun() { vm.BeforeRun(); }
        public void AfterRun(bool success, long elapsedMs) { vm.AfterRun(success, elapsedMs); }
        public void BeforeSave() { vm.BeforeSave(); }
        public void AfterSave(bool success) { vm.AfterSave(success); }
        public void BeforeClose() { Dispose(); }
        public void OnPluginExecutedFromOutside(PluginExecuteCompletedEventArgs e) { vm.OnPluginExecutedFromOutside(e); }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            Loaded -= OnLoaded;
            vm.Dispose();

            if (adapter != null)
            {
                adapter.Dispose();
                adapter = null;
            }
            if (HalconHost != null)
            {
                HalconHost.Child = null;
            }
            if (halconWindow != null)
            {
                halconWindow.Dispose();
                halconWindow = null;
            }
            DataContext = null;
        }
    }
}

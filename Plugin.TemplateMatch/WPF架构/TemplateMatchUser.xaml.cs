using Frame_Core;
using Plugin.TemplateMatch.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using WinForms = System.Windows.Forms;

namespace Plugin.TemplateMatch.Views
{
    public partial class TemplateMatchUser : UserControl, IPluginWpfView
    {
        private TemplateMatchViewModel vm;
        private WindowsFormsHost mainHost;
        private WindowsFormsHost templateHost;
        private WindowsFormsHost matchHost;
        private bool isClosed;

        public TemplateMatchUser()
        {
            Application.LoadComponent(this, new Uri("/Plugin.TemplateMatch;component/WPF架构/TemplateMatchUser.xaml", UriKind.Relative));
            vm = new TemplateMatchViewModel();
            DataContext = vm;
            AttachHalconHosts();
        }

        public IPlugin PluginObject { get { return vm == null ? null : vm.PluginObject; } }
        public PluginParamsBase PluginParams { get { return vm == null ? null : vm.PluginParams; } }

        public void BindPlugin(IPlugin plugin)
        {
            if (vm == null) return;
            vm.BindPlugin(plugin);
        }

        public void RefreshBindings() { if (vm != null) vm.RefreshBindings(); }
        public PluginViewActionResult RunFromHost() { return vm == null ? PluginViewActionResult.Fail("模板匹配界面已关闭") : vm.RunFromHost(); }
        public PluginViewActionResult SaveFromHost(string filePath) { return vm == null ? PluginViewActionResult.Fail("模板匹配界面已关闭") : vm.SaveFromHost(filePath); }
        public void CloseFromHost() { CloseViewResources(); }
        public void BeforeRun() { if (vm != null) vm.BeforeRun(); }
        public void AfterRun(bool success, long elapsedMs) { if (vm != null) vm.AfterRun(success, elapsedMs); }
        public void BeforeSave() { if (vm != null) vm.BeforeSave(); }
        public void AfterSave(bool success) { if (vm != null) vm.AfterSave(success); }
        public void BeforeClose() { if (vm != null) vm.BeforeClose(); }
        public void OnPluginExecutedFromOutside(PluginExecuteCompletedEventArgs e) { if (vm != null) vm.OnPluginExecutedFromOutside(e); }

        private void AttachHalconHosts()
        {
            mainHost = FindName("MainHost") as WindowsFormsHost;
            templateHost = FindName("TemplateHost") as WindowsFormsHost;
            matchHost = FindName("MatchHost") as WindowsFormsHost;

            vm.MainHalconWindow.Dock = WinForms.DockStyle.Fill;
            vm.TemplateHalconWindow.Dock = WinForms.DockStyle.Fill;
            vm.MatchHalconWindow.Dock = WinForms.DockStyle.Fill;

            if (mainHost != null) mainHost.Child = vm.MainHalconWindow;
            if (templateHost != null) templateHost.Child = vm.TemplateHalconWindow;
            if (matchHost != null) matchHost.Child = vm.MatchHalconWindow;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            // Unloaded may be raised by tab switching or visual-tree reattachment, so do not destroy the plugin VM here.
        }

        private void CloseViewResources()
        {
            if (isClosed) return;
            isClosed = true;

            if (MaskRoiEditor != null)
                MaskRoiEditor.Release();

            if (mainHost != null) mainHost.Child = null;
            if (templateHost != null) templateHost.Child = null;
            if (matchHost != null) matchHost.Child = null;

            DataContext = null;
            if (vm != null)
            {
                vm.CloseFromHost();
                vm = null;
            }

            mainHost = null;
            templateHost = null;
            matchHost = null;
        }
    }
}

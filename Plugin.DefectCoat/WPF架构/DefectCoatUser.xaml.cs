using DefectRuleClassifier.Core.Models;
using Frame_Core;
using HalconDotNet;
using Plugin.DefectCoat.Services;
using Plugin.DefectCoat.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WinForms = System.Windows.Forms;

namespace Plugin.DefectCoat.WPF架构
{
    /// <summary>
    /// 涂布缺陷检测插件的 WPF 页面。
    /// 这里只做界面容器初始化和框架生命周期转发，不写检测、分类、保存等业务逻辑。
    /// </summary>
    public partial class DefectCoatUser : UserControl, IPluginWpfView
    {
        /// <summary>页面唯一 ViewModel，业务绑定和宿主生命周期都转发给它处理。</summary>
        private  DefectCoatViewModel vm;
        /// <summary>主图 HALCON 窗口，用于显示输入图、ViewROI 和检测叠加。</summary>
        private  HWindowControl mainHalconWindow;
        /// <summary>NG Region 预览 HALCON 窗口。</summary>
        private  HWindowControl regionHalconWindow;
        /// <summary>NG 小图预览 HALCON 窗口。</summary>
        private  HWindowControl smallHalconWindow;
        /// <summary>预处理/阈值图预览 HALCON 窗口。</summary>
        private  HWindowControl preprocessHalconWindow;
        /// <summary>本地资源是否已经关闭，防止 Unloaded 和宿主 Close 重复 Dispose。</summary>
        private bool closed;

        /// <summary>
        /// 初始化插件页面、四个 HALCON 窗口和 ViewModel 适配器。
        /// 构造函数只做界面装配，不加载参数、不执行检测。
        /// </summary>
        public DefectCoatUser()
        {
            InitializeComponent();

            vm = new DefectCoatViewModel();
            DataContext = vm;

            mainHalconWindow = CreateHalconControl();
            regionHalconWindow = CreateHalconControl();
            smallHalconWindow = CreateHalconControl();
            preprocessHalconWindow = CreateHalconControl();

            MainHost.Child = mainHalconWindow;
            RegionHost.Child = regionHalconWindow;
            SmallHost.Child = smallHalconWindow;
            PreprocessHost.Child = preprocessHalconWindow;

            vm.AttachHalconView(new HalconViewRoiAdapter(mainHalconWindow));
            vm.AttachPreviewWindows(regionHalconWindow, smallHalconWindow, preprocessHalconWindow);
        }

        /// <summary>宿主读取插件对象入口，直接转发给 ViewModel。</summary>
        public IPlugin PluginObject => vm.PluginObject;

        /// <summary>宿主读取插件参数入口，直接转发给 ViewModel。</summary>
        public PluginParamsBase PluginParams => vm.PluginParams;

        /// <summary>宿主绑定 MEF 插件对象入口。</summary>
        public void BindPlugin(IPlugin plugin) { vm.BindPlugin(plugin); }

        /// <summary>宿主要求刷新绑定入口。</summary>
        public void RefreshBindings() { vm.RefreshBindings(); }

        /// <summary>宿主执行插件入口，页面层只转发给 ViewModel。</summary>
        public PluginViewActionResult RunFromHost() { return vm.RunFromHost(); }

        /// <summary>宿主保存插件参数入口，页面层只转发给 ViewModel。</summary>
        public PluginViewActionResult SaveFromHost(string filePath) { return vm.SaveFromHost(filePath); }

        /// <summary>宿主关闭页面入口，释放本页面创建的 HALCON 显示控件。</summary>
        public void CloseFromHost() { CloseLocalResources(); }

        /// <summary>宿主运行前通知入口。</summary>
        public void BeforeRun() { vm.BeforeRun(); }

        /// <summary>宿主运行后通知入口。</summary>
        public void AfterRun(bool success, long elapsedMs) { vm.AfterRun(success, elapsedMs); }

        /// <summary>宿主保存前通知入口。</summary>
        public void BeforeSave() { vm.BeforeSave(); }

        /// <summary>宿主保存后通知入口。</summary>
        public void AfterSave(bool success) { vm.AfterSave(success); }

        /// <summary>宿主关闭前通知入口。</summary>
        public void BeforeClose() { vm.BeforeClose(); }

        /// <summary>插件在页面外部被执行后的结果刷新入口。</summary>
        public void OnPluginExecutedFromOutside(PluginExecuteCompletedEventArgs e) { vm.OnPluginExecutedFromOutside(e); }

        /// <summary>创建统一样式的 HALCON WindowsForms 控件。</summary>
        private static HWindowControl CreateHalconControl()
        {
            return new HWindowControl { Dock = WinForms.DockStyle.Fill, BackColor = System.Drawing.Color.Black };
        }


        /// <summary>
        /// 释放页面本地资源。
        /// 这里只释放页面创建的 HALCON 控件和 ViewModel 显示适配器，不释放宿主仍可能持有的输入图像。
        /// </summary>
        private void CloseLocalResources()
        {
            if (closed)
                return;
            closed = true;

            vm.CloseFromHost();
            vm = null;

           
            MainHost?.Dispose();
            RegionHost?.Dispose();
            SmallHost?.Dispose();
            PreprocessHost?.Dispose();

            mainHalconWindow.Dispose();
            regionHalconWindow.Dispose();
            smallHalconWindow.Dispose();
            preprocessHalconWindow.Dispose();

            mainHalconWindow = null;
            regionHalconWindow = null;
            smallHalconWindow = null;
            preprocessHalconWindow = null;

            MainHost.Child = null;
            RegionHost.Child = null;
            SmallHost.Child = null;
            PreprocessHost.Child = null;

            this.DataContext = null;


        }

       

        /// <summary>主图自适应显示按钮事件。</summary>
        private void OnFitImageClick(object sender, RoutedEventArgs e)
        {
            vm.FitAllViews();
        }

        /// <summary>NG Region 预览自适应显示按钮事件。</summary>
        private void OnFitRegionClick(object sender, RoutedEventArgs e)
        {
            vm.FitRegionView();
        }

        /// <summary>NG 小图预览自适应显示按钮事件。</summary>
        private void OnFitSmallClick(object sender, RoutedEventArgs e)
        {
            vm.FitSmallView();
        }

        /// <summary>预处理图预览自适应显示按钮事件。</summary>
        private void OnFitPreprocessClick(object sender, RoutedEventArgs e)
        {
            vm.FitPreprocessView();
        }
    }
}

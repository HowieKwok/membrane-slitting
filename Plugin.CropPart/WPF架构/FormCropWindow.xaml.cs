using System.Windows;
using System.Windows.Controls;
using Frame_Core;
using HalconDotNet;
using Plugin.CropPart.Services;
using Plugin.CropPart.ViewModels;

namespace Plugin.CropPart.WPF架构
{
    /// <summary>
    /// FormCropWindow.xaml 的交互逻辑
    /// </summary>
    public partial class FormCropWindow : UserControl, IPluginWpfView
    {
        private readonly CropPartViewModel vm;
        private readonly CropPartHalconViewAdapter mainView;
        private readonly CropPartHalconViewAdapter cropView;

        public FormCropWindow()
        {
            InitializeComponent();

            vm = new CropPartViewModel();
            DataContext = vm;

            mainView = new CropPartHalconViewAdapter(new HWindowControl());
            cropView = new CropPartHalconViewAdapter(new HWindowControl());
            mainHost.Child = mainView.WindowControl;
            CropHost.Child = cropView.WindowControl;
            vm.AttachViews(mainView, cropView);
        }

        public IPlugin PluginObject
        {
            get { return vm.PluginObject; }
        }

        public PluginParamsBase PluginParams
        {
            get { return vm.PluginParams; }
        }

        public void BindPlugin(IPlugin plugin)
        {
            vm.BindPlugin(plugin);
            UpdateWindowTitle();
        }

        public void RefreshBindings()
        {
            vm.RefreshBindings();
        }

        public PluginViewActionResult RunFromHost()
        {
            return vm.RunFromHost();
        }

        public PluginViewActionResult SaveFromHost(string filePath)
        {
            return vm.SaveFromHost(filePath);
        }

        public void CloseFromHost()
        {
            vm.CloseFromHost();
            mainView.Dispose();
            cropView.Dispose();
            mainHost.Child = null;
            CropHost.Child = null;
           
        }

        public void BeforeRun()
        {
            vm.BeforeRun();
        }

        public void AfterRun(bool success, long elapsedMs)
        {
            vm.AfterRun(success, elapsedMs);
        }

        public void BeforeSave()
        {
            vm.BeforeSave();
        }

        public void AfterSave(bool success)
        {
            vm.AfterSave(success);
        }

        public void BeforeClose()
        {
            vm.BeforeClose();
        }

        public void OnPluginExecutedFromOutside(PluginExecuteCompletedEventArgs e)
        {
            vm.OnPluginExecutedFromOutside(e);
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            CloseFromHost();
 
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateWindowTitle();
        }

        private void UpdateWindowTitle()
        {
            Window window = Window.GetWindow(this);
            if (window != null) window.Title = vm.TitleText;
        }
    }
}

using System.Windows;
using Frame_Core;

namespace Plugin.FindIntersection
{
    public class FindIntersectionDemoDialog : WPFFormPluginBase
    {
        private readonly FindIntersectionDemoViewModel viewModel = new FindIntersectionDemoViewModel();

        public FindIntersectionDemoDialog(DemoPluginInfo pluginInfo)
        {
            Title = pluginInfo == null ? "FindIntersection" : pluginInfo.DisplayName;
            Width = 1220;
            Height = 760;
            MinWidth = 1100;
            MinHeight = 680;

            viewModel.SelectedPlugin = pluginInfo;
            viewModel.ResetResult();

            Content = new FindIntersectionWpfView { DataContext = viewModel };
        }
    }
}

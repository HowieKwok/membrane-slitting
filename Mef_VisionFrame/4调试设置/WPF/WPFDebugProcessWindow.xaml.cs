using Mef_VisionFrame._4调试设置.WPF.ViewModel;
using System.Windows;

namespace Mef_VisionFrame.调试设置.WPF
{
    public partial class WPFDebugProcessWindow : Window
    {
        // 公开VM，外部也可以访问
        public WPFDebugProcessViewModel ViewModel { get; }
        public WPFDebugProcessWindow()
        {
            InitializeComponent();
            // 绑定ViewModel
            ViewModel = new WPFDebugProcessViewModel(this);
            // ✅ 绑定到内部根Grid，不是Window！隔绝父窗体DataContext污染
            RootGrid.DataContext = ViewModel;
            //// 双击标题栏切换最大化
            this.MouseLeftButtonDown += (sender, e) =>
            {
                if (e.ClickCount == 2 && DataContext is WPFDebugProcessViewModel vm)
                {
                    vm.ToggleMaxCommand.Execute(null);
                }
            };
        }
    }
}

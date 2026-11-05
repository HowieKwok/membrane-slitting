using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore.Defaults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Mef_VisionFrame._4调试设置.WPF.ViewModel
{
   

    public partial class WPFDebugProcessViewModel: ObservableObject
    {
        private readonly WeakReference<Window> _windowRef;

        [ObservableProperty]
        private string _windowTitle = "调试流程界面";

  
        public string WindowTitle
        {
            get => _windowTitle;
            set => SetProperty(ref _windowTitle, value);
        }
        private string _maxButtonText = "□";
        public string MaxButtonText
        {
            get => _maxButtonText;
            set => SetProperty(ref _maxButtonText, value);
        }

        public WPFDebugProcessViewModel(Window window)
        {
            _windowRef = new WeakReference<Window>(window);

            // 老写法：手动创建命令
            MinimizeCommand = new RelayCommand(Minimize);
            ToggleMaxCommand = new RelayCommand(ToggleMax);
            HideWindowCommand = new RelayCommand(HideWindow);
            CloseWindowCommand = new RelayCommand(CloseWindow);
        }

        private Window GetTargetWindow()
        {
            _windowRef.TryGetTarget(out var win);
            return win;
        }

        public ICommand MinimizeCommand { get; }
        private void Minimize()
        {
            var win = GetTargetWindow();
            if (win == null) return;
            win.WindowState = WindowState.Minimized;
        }

        public ICommand ToggleMaxCommand { get; }
        private void ToggleMax()
        {
            var win = GetTargetWindow();
            if (win == null) return;

            if (win.WindowState == WindowState.Maximized)
            {
                win.WindowState = WindowState.Normal;
                MaxButtonText = "□";
            }
            else
            {
                win.WindowState = WindowState.Maximized;
                MaxButtonText = "▢";
            }
        }

        public ICommand HideWindowCommand { get; }
        private void HideWindow()
        {
            var win = GetTargetWindow();
            if (win == null) return;
            win.Hide();
        }

        public ICommand CloseWindowCommand { get; }
        private void CloseWindow()
        {
            var win = GetTargetWindow();
            if (win == null) return;
            win.Close();
        }
    }
}

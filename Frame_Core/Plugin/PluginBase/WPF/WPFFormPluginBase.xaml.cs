using BitMiracle.LibTiff.Classic;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;

namespace Frame_Core
{
    /// <summary>
    /// WPF 插件窗体基类。
    /// 
    /// 只保留一条执行完成通知链：
    /// IPlugin.ExecuteCompleted -> WPFFormPluginBase -> IPluginWpfView.OnPluginExecutedFromOutside。
    /// 
    /// 不再实现 IPluginActionNotifier，也不再保留 ActionCompleted / RaiseActionCompleted，
    /// 避免窗口按钮执行和后台执行走两套刷新机制。
    /// </summary>
    [ContentProperty("PluginContent")]
    public partial class WPFFormPluginBase : Window, INotifyPropertyChanged
    {
        private string costTimeText = "耗时：00ms";
        private string statusText = "等待操作";
        private bool beforeCloseCalled;
        private bool executeCompletedSubscribed;

        public LanguageEnum FormLanguage;
        public IPlugin pluginsObjBase { get; set; }
        public PluginParamsBase pluginsObjParams_Back { get; set; }

        public ICommand RunCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand CloseCommand { get; private set; }

        public object PluginContent
        {
            get { return GetValue(PluginContentProperty); }
            set { SetValue(PluginContentProperty, value); }
        }

        public static readonly DependencyProperty PluginContentProperty =
            DependencyProperty.Register("PluginContent", typeof(object), typeof(WPFFormPluginBase), new PropertyMetadata(null));

        public string CostTimeText
        {
            get { return costTimeText; }
            set
            {
                if (costTimeText == value) return;
                costTimeText = value;
                OnPropertyChanged();
            }
        }

        public string StatusText
        {
            get { return statusText; }
            set
            {
                if (statusText == value) return;
                statusText = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private IPluginWpfView PluginView
        {
            get { return PluginContent as IPluginWpfView; }
        }

        public WPFFormPluginBase()
        {
            RunCommand = new RelayCommand(delegate { Btn_RUN_Click(this, EventArgs.Empty); });
            SaveCommand = new RelayCommand(delegate { Btn_Save_Click(this, EventArgs.Empty); });
            CloseCommand = new RelayCommand(delegate { Btn_Close_Click(this, EventArgs.Empty); });

            InitializeComponent();
            Loaded += WPFFormPluginBase_Loaded;
            Closed += WPFFormPluginBase_Closed;
        }

        public virtual void Init()
        {
            if (pluginsObjBase == null || pluginsObjBase.moduleInfo == null) return;

            Title = pluginsObjBase.moduleInfo.ModuleName;
            StatusText = "模块：" + pluginsObjBase.moduleInfo.ModuleName;
            SubscribeExecuteCompleted();
            CommandManager.InvalidateRequerySuggested();
        }

        public virtual void Btn_RUN_Click(object sender, EventArgs e)
        {
            IPluginWpfView view = PluginView;
            if (view == null)
            {
                ApplyActionResult(PluginViewActionResult.Fail("执行失败：插件页面未实现 WPF 接口"), "执行失败");
                return;
            }

            try
            {
                pluginsObjBase.ExecuteObj();
                // PluginViewActionResult result = view.RunFromHost();
                PluginViewActionResult result = new PluginViewActionResult();
                result.Success = pluginsObjBase.PluginParams.m_bRunSuccess;
                result.Message = pluginsObjBase.PluginParams.m_strRunMessage;
                result.ElapsedMs = (long)pluginsObjBase.PluginParams.m_fElapsedMs;
                ApplyActionResult(result, result != null && result.Success ? "执行完成" : "执行失败");
            }
            catch (Exception ex)
            {
                ApplyActionResult(PluginViewActionResult.Fail("执行异常：" + ex.Message), "执行异常");
            }
        }

        public virtual void Btn_Save_Click(object sender, EventArgs e)
        {
            if (pluginsObjBase == null || pluginsObjBase.moduleInfo == null)
            {
                StatusText = "保存失败：插件对象为空";
                return;
            }

            IPluginWpfView view = PluginView;
            if (view == null)
            {
                StatusText = "保存失败：插件页面未实现 WPF 接口";
                return;
            }

            try
            {
                string filePath = PreparePluginSavePath();
                PluginViewActionResult result = view.SaveFromHost(filePath);
                ApplyActionResult(result, result != null && result.Success ? "参数已保存" : "参数保存失败");
            }
            catch (Exception ex)
            {
                StatusText = "保存异常：" + ex.Message;
            }
        }

        public virtual void Btn_Close_Click(object sender, EventArgs e)
        {
            Close();
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            if (!beforeCloseCalled)
            {
                beforeCloseCalled = true;
                try
                {
                    IPluginWpfView view = PluginView;
                    if (view != null) view.CloseFromHost();
                }
                catch (Exception ex)
                {
                    StatusText = "关闭异常：" + ex.Message;
                }
            }

            // ===== 修复：摘掉并释放所有 WindowsFormsHost，切断 WinForms 控件 → 窗口 的引用链 =====
            try
            {
                DetachWindowsFormsHosts(this);
            }
            catch
            {
            }
            PluginContent = null;

            base.OnClosing(e);
        }


        private static void DetachWindowsFormsHosts(DependencyObject current)
        {
            if (current == null) return;

            System.Windows.Forms.Integration.WindowsFormsHost host =
                current as System.Windows.Forms.Integration.WindowsFormsHost;
            if (host != null)
            {
                host.Child = null;
                host.Dispose();
                return;
            }

            int count = System.Windows.Media.VisualTreeHelper.GetChildrenCount(current);
            for (int i = 0; i < count; i++)
            {
                DetachWindowsFormsHosts(System.Windows.Media.VisualTreeHelper.GetChild(current, i));
            }
        }

        private string PreparePluginSavePath()
        {
            pluginsObjParams_Back = pluginsObjBase.PluginParams;
            string processPath = Path.Combine(Project.Instance.paramPathSet.ProcessPath, pluginsObjBase.moduleInfo.ProcessName);
            pluginsObjBase.moduleInfo.filePath = Path.Combine(processPath, pluginsObjBase.moduleInfo.ModuleName);
            if (pluginsObjBase.PluginParams != null) pluginsObjBase.PluginParams.filePath = pluginsObjBase.moduleInfo.filePath;
            return pluginsObjBase.moduleInfo.filePath;
        }

        private void ApplyActionResult(PluginViewActionResult result, string defaultMessage)
        {
            if (result == null)
            {
                CostTimeText = "耗时：00ms";
                StatusText = defaultMessage;
                return;
            }

            CostTimeText = "耗时：" + result.ElapsedMs.ToString("0.0") + "ms";
            StatusText = string.IsNullOrWhiteSpace(result.Message) ? defaultMessage : result.Message;
        }

        protected virtual void WPFFormPluginBase_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
        }

        private void WPFFormPluginBase_Closed(object sender, EventArgs e)
        {
            UnsubscribeExecuteCompleted();
            BindingOperations.ClearAllBindings(this);  // 清除所有子元素的绑定
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        private void SubscribeExecuteCompleted()
        {
            if (pluginsObjBase == null || executeCompletedSubscribed) return;

            pluginsObjBase.ExecuteCompleted += OnPluginExecuteCompleted;
            executeCompletedSubscribed = true;
        }

        private void UnsubscribeExecuteCompleted()
        {
            if (pluginsObjBase == null || !executeCompletedSubscribed) return;

            try
            {
                pluginsObjBase.ExecuteCompleted -= OnPluginExecuteCompleted;
            }
            catch
            {
            }

            executeCompletedSubscribed = false;
        }

        private void OnPluginExecuteCompleted(object sender, PluginExecuteCompletedEventArgs e)
        {
            if (!ReferenceEquals(sender, pluginsObjBase)) return;

            Dispatcher.BeginInvoke(new Action(() =>
            {
                CostTimeText = "耗时：" + e.ElapsedMs.ToString("0.0") + "ms";
                StatusText = string.IsNullOrWhiteSpace(e.Message) ? (e.Success ? "执行完成" : "执行失败") : e.Message;

                IPluginWpfView view = PluginView;
                if (view != null)
                {
                    view.OnPluginExecutedFromOutside(e);
                }
            }));
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private class RelayCommand : ICommand
        {
            private readonly Action<object> execute;
            private readonly Func<bool> canExecute;

            public RelayCommand(Action<object> execute, Func<bool> canExecute = null)
            {
                this.execute = execute;
                this.canExecute = canExecute;
            }

            public bool CanExecute(object parameter)
            {
                return canExecute == null || canExecute();
            }

            public void Execute(object parameter)
            {
                execute(parameter);
            }

            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }
        }
    }
}


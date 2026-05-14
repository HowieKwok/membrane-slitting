using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;

namespace Frame_Core
{
    [ContentProperty("PluginContent")]
    public partial class WPFFormPluginBase : Window, INotifyPropertyChanged, IPluginActionNotifier
    {
        private string costTimeText = "耗时：000ms";
        private string statusText = "等待操作";
        private bool beforeCloseCalled;

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
        public event EventHandler<PluginViewActionResult> ActionCompleted;

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
        }

        public virtual void Init()
        {
            if (pluginsObjBase == null || pluginsObjBase.moduleInfo == null) return;

            Title = pluginsObjBase.moduleInfo.ModuleName;
            StatusText = "模块：" + pluginsObjBase.moduleInfo.ModuleName;
            CommandManager.InvalidateRequerySuggested();
        }

        public virtual void Btn_RUN_Click(object sender, EventArgs e)
        {
            PluginViewActionResult result = null;
            IPluginWpfView view = PluginView;
            if (view == null)
            {
                result = PluginViewActionResult.Fail("执行失败：插件页面未实现 WPF 接口");
                StatusText = result.Message;
                RaiseActionCompleted(result);
                return;
            }

            try
            {
                result = view.RunFromHost();
                ApplyActionResult(result, result != null && result.Success ? "执行完成" : "执行失败");
            }
            catch (Exception ex)
            {
                result = PluginViewActionResult.Fail("执行异常：" + ex.Message);
                StatusText = result.Message;
            }
            finally
            {
                // 执行结果由插件写入 PluginParams。基类只通知外层重新读取参数刷新显示。
                RaiseActionCompleted(result);
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

            base.OnClosing(e);
        }

        private string PreparePluginSavePath()
        {
            pluginsObjParams_Back = pluginsObjBase.PluginParams;
            string processPath = Path.Combine(Project.Instance.paramPathSet.ProcessPath, pluginsObjBase.moduleInfo.ProcessName);
            pluginsObjBase.moduleInfo.filePath = Path.Combine(processPath, pluginsObjBase.moduleInfo.ModuleName);
            if (pluginsObjBase.PluginParams != null) pluginsObjBase.PluginParams.filePath = pluginsObjBase.moduleInfo.filePath;
            return pluginsObjBase.moduleInfo.filePath;
        }

        protected virtual void RaiseActionCompleted(PluginViewActionResult result)
        {
            EventHandler<PluginViewActionResult> handler = ActionCompleted;
            if (handler != null) handler(this, result);
        }
        private void ApplyActionResult(PluginViewActionResult result, string defaultMessage)
        {
            if (result == null)
            {
                CostTimeText = "耗时：000ms";
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

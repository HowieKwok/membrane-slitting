using System;
using System.Windows.Input;

namespace Plugin.DefectCoat.ViewModels
{
    /// <summary>
    /// WPF MVVM 通用命令包装类。
    /// 只保存执行委托和可执行判断委托，不包含业务逻辑；具体业务仍写在 ViewModel 方法中。
    /// </summary>
    public sealed class RelayCommand : ICommand
    {
        /// <summary>命令执行委托。按钮、菜单或快捷键触发时调用。</summary>
        private readonly Action execute;

        /// <summary>命令是否可执行的判断委托。为空时表示始终可执行。</summary>
        private readonly Func<bool> canExecute;

        /// <summary>
        /// 创建一个无参数命令。
        /// execute 不允许为空；canExecute 可为空，表示界面按钮不需要禁用控制。
        /// </summary>
        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecute = canExecute;
        }

        /// <summary>通知 WPF 重新查询 CanExecute，用于选择项变化后刷新按钮启用状态。</summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>返回命令当前是否可执行。parameter 未使用，保留 ICommand 标准签名。</summary>
        public bool CanExecute(object parameter)
        {
            return canExecute == null || canExecute();
        }

        /// <summary>执行命令。parameter 未使用，所有输入都由 ViewModel 当前状态提供。</summary>
        public void Execute(object parameter)
        {
            execute();
        }

        /// <summary>主动触发按钮可用状态刷新，例如 SelectedResult 或 SelectedMaskRoi 变化后调用。</summary>
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}

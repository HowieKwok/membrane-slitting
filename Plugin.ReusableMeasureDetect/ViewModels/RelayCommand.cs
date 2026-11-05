using System;
using System.Windows.Input;

namespace Plugin.ReusableMeasureDetect.ViewModels
{
    /// <summary>
    /// 主界面通用命令实现，将按钮点击等 ICommand 调用转发到 ViewModel 委托。
    /// </summary>
    public sealed class RelayCommand<T> : ICommand
    {
        private readonly Action<T> execute;
        private readonly Func<T, bool> canExecute;

        /// <summary>
        /// 创建命令，execute 为必填执行逻辑，canExecute 为可选启用判断。
        /// </summary>
        public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null)
        {
            if (execute == null) throw new ArgumentNullException(nameof(execute));
            this.execute = execute;
            this.canExecute = canExecute;
        }

        /// <summary>
        /// WPF 命令系统监听的可执行状态变化事件。
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// 判断当前命令是否允许执行。
        /// </summary>
        public bool CanExecute(object parameter)
        {
            if (canExecute == null) return true;
            if (parameter == null && typeof(T).IsValueType) return canExecute(default(T));
            return canExecute((T)parameter);
        }

        /// <summary>
        /// 执行命令，将 object 参数转换为泛型参数后调用 execute。
        /// </summary>
        public void Execute(object parameter)
        {
            T value = default(T);
            if (parameter != null) value = (T)parameter;
            execute(value);
        }

        /// <summary>
        /// 主动通知 WPF 重新查询 CanExecute。
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            EventHandler handler = CanExecuteChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}

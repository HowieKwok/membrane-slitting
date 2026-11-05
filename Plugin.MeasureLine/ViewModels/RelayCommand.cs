using System;
using System.Windows.Input;

namespace Plugin.MeasureLine.ViewModels
{
    public sealed class RelayCommand<T> : ICommand
    {
        private readonly Action<T> execute;
        private readonly Func<T, bool> canExecute;

        public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null)
        {
            if (execute == null) throw new ArgumentNullException(nameof(execute));
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            if (canExecute == null) return true;
            if (parameter == null && typeof(T).IsValueType) return canExecute(default(T));
            return canExecute((T)parameter);
        }

        public void Execute(object parameter)
        {
            T value = default(T);
            if (parameter != null) value = (T)parameter;
            execute(value);
        }

        public void RaiseCanExecuteChanged()
        {
            EventHandler handler = CanExecuteChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}

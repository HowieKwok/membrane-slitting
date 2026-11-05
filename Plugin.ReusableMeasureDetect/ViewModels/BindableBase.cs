using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Plugin.ReusableMeasureDetect.ViewModels
{
    /// <summary>
    /// 主界面 MVVM 基类，封装 INotifyPropertyChanged 和 SetProperty，供界面绑定实时刷新。
    /// </summary>
    public abstract class BindableBase : INotifyPropertyChanged
    {
        /// <summary>
        /// WPF 绑定监听的属性变化事件。
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 设置字段并在值变化时触发属性通知。
        /// </summary>
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value)) return false;
            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// 手动触发属性通知；propertyName 为空字符串时刷新全部绑定。
        /// </summary>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

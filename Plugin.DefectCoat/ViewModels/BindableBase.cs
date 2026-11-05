using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Plugin.DefectCoat.ViewModels
{
    /// <summary>
    /// WPF 界面绑定基类，只允许 ViewModel 或界面行对象继承。
    /// 参数类后续要迁移到 Frame_Core\pluginParams，不能继承该类，避免参数层耦合插件界面层。
    /// </summary>
    public abstract class BindableBase : INotifyPropertyChanged
    {
        /// <summary>WPF 绑定属性变化事件，ViewModel 属性更新后通过它通知界面刷新。</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 设置字段并触发属性变化通知。
        /// 新旧值相等时不通知，避免界面无意义刷新和命令状态反复计算。
        /// </summary>
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>手动触发属性变化通知，默认使用调用方属性名。</summary>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DefectRuleClassifier.ViewModels
{
    /// <summary>
    /// MVVM 绑定通知基类。
    /// 所有需要参与界面绑定的 ViewModel、参数行、结果行都应继承该类。
    /// 属性 setter 统一调用 SetProperty，值变化后触发 PropertyChanged，界面绑定才会刷新。
    /// </summary>
    public abstract class BindableBase : ObservableObject
    {
        //public event PropertyChangedEventHandler PropertyChanged;

        //protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        //{
        //    if (EqualityComparer<T>.Default.Equals(field, value))
        //        return false;

        //    field = value;
        //    OnPropertyChanged(propertyName);
        //    return true;
        //}

        //protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}
    }
}

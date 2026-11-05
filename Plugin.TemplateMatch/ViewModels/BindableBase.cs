using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Plugin.TemplateMatch.ViewModels
{
    public abstract class BindableBase : ObservableObject
    {
        //public event PropertyChangedEventHandler PropertyChanged;

        //protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        //{
        //    if (Equals(field, value)) return false;
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
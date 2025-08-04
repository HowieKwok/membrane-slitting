using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Frame_Core;

namespace LightControl
{
    public class LightModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public LightManager[] LightManager { get; set; }


        public LightModel(LightManager[]  lightManagers)
        {
            LightManager = lightManagers;
        }
        
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

       


        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name)); // 通知 Name 属性发生了变化
                }
            }
        }



        
        public void Save()
        {

        }

    }
}

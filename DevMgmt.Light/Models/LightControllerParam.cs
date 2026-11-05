
using DevMgmt.Light.SDK;
using FrameCommunity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DevMgmt.Light.Models
{
    [Serializable]
    public class LightControllerParam : BindableBase
    {
        public ObservableCollection<LightControllerDev> l_LightControllerDevs { get; set; } = new ObservableCollection<LightControllerDev>();
     
    }


    [Serializable]
    public class LightControllerDev : BindableBase  // 👈 这里必须加！
    {
        [XmlIgnore]
        [JsonIgnore]
        public LightControl LightController { get; set; }

        private LightControllerParameterBase _LightControllerParameter;
        public LightControllerParameterBase LightControllerParameter
        {
            get => _LightControllerParameter;
            set => SetProperty(ref _LightControllerParameter, value); // 👈 绑定用
        }


        // 核心布尔状态字段
        private bool _connStatus;
        [XmlIgnore]
        [JsonIgnore]
        public bool ConnStatus
        {
            get => _connStatus;
            set => SetProperty(ref _connStatus, value);
        }
    }
}

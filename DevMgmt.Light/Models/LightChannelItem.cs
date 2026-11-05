using FrameCommunity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevMgmt.Light.Models
{
    public class LightChannelItem : BindableBase
    {
        private string _DisplayName = "";
        /// <summary> 界面标签文本 </summary>
        public string DisplayName
        {
            get => _DisplayName;
            set => SetProperty(ref _DisplayName, value);
        }

        private int _brightness = 0;
        /// <summary>通道亮度 0~255</summary>
        public int Brightness
        {
            get => _brightness;
            set => SetProperty(ref _brightness, value);
        }

        private int _portNo = 0;
        /// <summary>绑定的硬件端口号</summary>
        public int PortNo
        {
            get => _portNo;
            set => SetProperty(ref _portNo, value);
        }

        private bool _SwitchState = false;
        /// <summary>打开关闭状态</summary>
        public bool SwitchState
        {
            get => _SwitchState;
            set => SetProperty(ref _SwitchState, value);
        }
    }
}

using CommunityToolkit.Mvvm.ComponentModel;
using FrameCommunity.公共类;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ControlBoard.Model
{
    public class ParamControlCord:FrameCommunity.DataOperation.JsonSerializationOperation
    {

        /// <summary>
        /// 卡号
        /// </summary>
        [Description("卡号")]
        public ushort CardNo = 0;





        /// <summary>
        /// 端口号
        /// </summary>
        [Description("端口号")]
        public ushort m_PortNo= 0;


        /// <summary>
        /// 控制卡类型
        /// </summary>
         [Description("控制卡类型")]  //枚举类型
        public EnumControlType  ControlType  = EnumControlType.IO0640;

        /// <summary>
        /// 设置滤波时间
        /// </summary>
        public double FilteTime = 20000;


    }

    public class IoPointStatus : ObservableObject
    {
        private bool _isOn;
        public int Index { get; set; }
        public string PointLabel => $"IO{Index:D2}";

        public bool IsOn
        {
            get => _isOn;
            set => SetProperty(ref _isOn, value);
        }
    }
}

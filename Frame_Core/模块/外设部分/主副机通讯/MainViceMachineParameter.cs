using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    /// <summary>
    /// 主副机通讯参数
    /// </summary>
    public class MainSlaveMachineParameter : XmlSerializationOperation
    {
        [Description("启用通讯")]
        public bool IsConnect { set; get; }

        [Description("是服务器")]
        public bool isServer { set; get; }

        [Description("主机服务器IP")]
        public string Server_IP { set; get; }

        [Description("客户端IP")]
        public string Client_IP { set; get; }

        public void Init()
        {
            IsConnect = true;
            Server_IP = "192.168.6.11";
            Client_IP = "192.168.6.100";
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    public class PLCDevParam : BusinessParamBase
    {
        [Description("通讯选择类型")]
        public int ComType { get; set; } = 0;

        [Description("启用通讯")]
        public bool IsConnect { get; set; } = true;

        [Description("电脑端IP")]
        public string LocalPC_IP { get; set; } = "127.0.0.1";

        [Description("设备IP")]
        public string PLC_IP { get; set; } = "127.0.0.1";

        [Description("设备端口号")]
        public int DevPort { get; set; } = 2000;


        [Description("字节转换")]//单位：ms
        public int nByteTrans { get; set; } = (int)emDataFormat.CDAB;
    }
}

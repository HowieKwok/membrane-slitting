using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    public class MainPLCParam : XmlSerializationOperation
    {
        [Description("通讯选择类型")]
        public int ComType = 0;

        [Description("启用通讯")]
        public bool IsConnect = true;

        [Description("电脑端IP")]
        public string LocalPC_IP = "127.0.0.1";

        [Description("设备IP")]
        public string PLC_IP = "127.0.0.1";

        [Description("设备端口号")]
        public int DevPort = 2000;
    

        [Description("超时时间")]//单位：ms
        public int TimeOutTime = 1000;

        [Description("字节转换")]//单位：ms
        public int nByteTrans =2;
    }

    public enum ComStationsanwahlEnum
    {
        上单面工位,
        下单面工位,
        A模切工位,
        B模切工位,
        A面A极片纠偏工位,
        A面B极片纠偏工位,
        B面A极片纠偏工位,
        B面B极片纠偏工位,
        A面极片复检工位,           
        B面极片复检工位,
        PFT,
    }
}

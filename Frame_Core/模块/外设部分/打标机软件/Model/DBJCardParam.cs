using Frame_Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    /// <summary>
    /// 打标机板卡参数类
    /// </summary>
    public class DBJCardParam : XmlSerializationOperation
    {
        [Description("通讯选择类型")]
        public int ComType { set; get; }

        [Description("启用通讯")]
        public bool IsConnect { set; get; }

        [Description("电脑端IP")]
        public string LocalPC_IP { set; get; }

        [Description("设备IP")]
        public string PLC_IP { set; get; }

        [Description("设备端口号")]
        public int DevPort { set; get; }

        [Description("自动打标")]
        public bool AutoDB { set; get; }


        public DBJCardParam()
        {
            //Init();
        }
        public void Init()
        {
            ComType = 0;
            IsConnect = true;
            LocalPC_IP = "127.0.0.1";
            PLC_IP = "127.0.0.1";
            DevPort = 2000;
            AutoDB = false;
        }

    }

}

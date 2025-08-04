using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    public class MDevParam_Slice1to2 : XmlSerializationOperation
    {
        [Description("通讯选择类型")]
        public int ComType  = 0;

        [Description("启用通讯")]
        public bool IsConnect = true;

        [Description("电脑端IP")]
        public string LocalPC_IP  = "127.0.0.1";

        [Description("设备IP")]
        public string PLC_IP  = "127.0.0.1";

        [Description("设备端口号")]
        public int DevPort  = 2000;

        [Description("编码器直径(单位：mm)")]
        public int EncoderDiameter  = 120;

        [Description("编码器脉冲")]
        public int EncoderPulse  = 5000;

        [Description("纠偏速度")]
        public int CorrentSleep = 10;

        /// <summary>
        /// PLC纠偏参数
        /// </summary>
        public CorrectParam correctParam = new CorrectParam();
       
    }
}

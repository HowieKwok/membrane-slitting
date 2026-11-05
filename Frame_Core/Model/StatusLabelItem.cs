using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    public class StatusLabelItem
    {
        public string Key { get; set; }
        public string Text { get; set; }
        public StatusLevel Status { get; set; } = StatusLevel.Normal;
    }
    public enum StatusLevel
    {
        Normal,   // 正常
        Warning,  // 警告
        Alarm     // 报警
    }
}

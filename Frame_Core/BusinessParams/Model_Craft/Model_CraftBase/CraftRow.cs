using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    public class CraftRow
    {
        public string Material { get; set; }
        public string CraftName { get; set; }
        public bool IsCurrent { get; set; }
        public DateTime LastWriteTime { get; set; }
        public string Status { get { return IsCurrent ? "生产中" : "可编辑"; } }
        public string ActionText { get { return "..."; } }
        public string LastWriteText { get { return LastWriteTime.ToString("yyyy-MM-dd HH:mm"); } }
    }
}

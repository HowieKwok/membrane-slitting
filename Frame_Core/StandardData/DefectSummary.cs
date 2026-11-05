using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    public class DefectSummary
    {
        public bool T_IsNG { get; set; } = true;
        public bool T_IsSevere { get; set; } = true;
        public int T_DefectLevel { get; set; } = 10000;

        public int T_TotalCount { get; set; } = 0;
        public int T_NgCount { get; set; } = 0;

        public string T_Name { get; set; } = "";
    }

}

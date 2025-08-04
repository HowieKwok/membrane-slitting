using Frame_Core;
using LogTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mef_VisionFrame
{
    public class BIZ_ThreadManage_OverHang : AThreadManageBase
    {
        public Thd_WorkClass thd_Communication = new Thd_WorkClass();

        public Thd_WorkClass thd_ProcessA = new Thd_WorkClass();

        public Thd_WorkClass thd_ProcessB = new Thd_WorkClass();
    }
}

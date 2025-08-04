using Frame_Core;
using LogTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mef_VisionFrame
{
    public class BIZ_ThreadManage_DoubleCathode : AThreadManageBase
    {
        public Thd_WorkClass thd_Communication = new Thd_WorkClass();

        public Thd_WorkClass thd_Process = new Thd_WorkClass();
    }
}

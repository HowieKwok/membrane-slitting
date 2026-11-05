using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Frame_Core;

namespace Mef_VisionFrame
{
    public  class BIZ_ThreadManage_WidthLaser : AThreadManageBase
    {


        /// <summary>
        /// 用于任务处理，不影响主流程时序
        /// </summary>
        public LimitThreadPool limitThreadPool = new LimitThreadPool(1, 1);
    }
}

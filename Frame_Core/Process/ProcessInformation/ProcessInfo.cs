using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    [Serializable]
   public  class ProcessInfo
    {
        public int ProcessID { get; set; }           //流程ID
        public string ProcessName { get; set; }       //流程  名称
      
    }
    public enum RunMode
    {
        None = 0,
        RunOnce = 1,//运行一次
        RunCycle = 2,//循环运行
    }
}

using Frame_Core;
using LogTool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mef_VisionFrame
{
    public class VM_DieCuttingCut : VMBase
    {
        public Model_DieCuttingCut Data;
        public Server_DieCuttingCut Server;
        public VM_DieCuttingCut(Model_DieCuttingCut _Data, Server_DieCuttingCut _Server) : base(_Data, _Server)
        {
            Data = _Data;
            Server = _Server;
        }
    }

}

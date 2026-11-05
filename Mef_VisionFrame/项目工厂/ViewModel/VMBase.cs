using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Mef_VisionFrame
{
    public abstract class VMBase
    {
        public ModelBase BaseData { get;  }
        public ServerBase BaseServer { get;  }
        public VMBase(ModelBase modelBase, ServerBase serverBase) { BaseData = modelBase; BaseServer = serverBase; }

    }
}

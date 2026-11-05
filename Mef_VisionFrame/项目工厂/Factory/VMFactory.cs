using Frame_Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mef_VisionFrame
{
    public class VMFactory : IViewModel
    {
        public VMBase WorkVMBase; 
        public void Load(ModelFactory modelFactory, ServerFactory serverFactory)
        {
            MaterialCrafEnum crafEnum = Project.Instance.systemInfo.CraftType;
            if (crafEnum == MaterialCrafEnum.激光模切一体机)
            {
                WorkVMBase = new VM_DieCuttingCut(modelFactory.WorkModelBase as Model_DieCuttingCut, serverFactory.WorkServerBase as Server_DieCuttingCut);
            }
        }
    }
}

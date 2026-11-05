using Frame_Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mef_VisionFrame
{
    public class ModelFactory : IModelFactory
    {
        public ModelBase WorkModelBase;
        public void Load()
        {
            MaterialCrafEnum crafEnum = Project.Instance.systemInfo.CraftType;
            if (crafEnum == MaterialCrafEnum.激光模切一体机)
            {
                WorkModelBase = new Model_DieCuttingCut();             
            }
        }

    
    }
}

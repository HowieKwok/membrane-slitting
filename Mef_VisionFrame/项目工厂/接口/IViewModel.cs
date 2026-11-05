using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mef_VisionFrame
{
    public interface IViewModel
    {
        void Load(ModelFactory modelFactory, ServerFactory  serverFactory);
    }
}

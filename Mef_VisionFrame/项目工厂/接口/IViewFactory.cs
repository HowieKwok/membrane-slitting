using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mef_VisionFrame
{
    public interface IViewFactory
    {
        void Load(VMFactory vMFactory);
    }
}

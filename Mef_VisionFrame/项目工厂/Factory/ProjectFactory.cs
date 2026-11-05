using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Mef_VisionFrame
{
    public static class ProjectFactory
    {
        public static ModelFactory F_Model = new ModelFactory();     
        public static ServerFactory F_Server = new ServerFactory();
        public static VMFactory F_VM = new VMFactory();
        public static ViewFactory F_View = new ViewFactory();
        public static void Load()
        {
            F_Model.Load();

            F_Server.Load(F_Model);

            F_VM.Load(F_Model, F_Server);

            F_View.Load(F_VM);
        }
    }
}

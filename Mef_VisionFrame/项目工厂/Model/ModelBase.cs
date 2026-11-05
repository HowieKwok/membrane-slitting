using Frame_Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mef_VisionFrame
{
    public abstract class ModelBase
    {
        public CraftBase M_CraftBase => Project.Instance.craftBase;

        private SimRunCommand _M_SimRunCommand = SimRunCommand.Stop;

        public SimRunCommand M_SimRunCommand
        {
            get => _M_SimRunCommand;
            set
            {
                if (_M_SimRunCommand == SimRunCommand.Stop)
                {
                    _M_SimRunCommand = value;
                }
                else
                {
                    if (value == SimRunCommand.Stop)
                        _M_SimRunCommand = value;
                }
            }
        }

        public abstract void Init();

        public abstract void InitFilePath();
    }

    public enum SimRunCommand
    {
        Stop,
        SimPrev,
        SimRun,
        AutoRun,
        SimNext
    }
}

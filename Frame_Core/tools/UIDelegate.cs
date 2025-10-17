using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    public delegate void LightChnaggeEventHandler();
    public static class UIDelegate
    {
        public static event LightChnaggeEventHandler LightChange;

        public static void LightParamChange()
        {
            LightChange?.Invoke();
        }
    }
}

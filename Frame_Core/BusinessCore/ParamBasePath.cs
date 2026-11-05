using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core.BusinessCore
{
    public static class ParamBasePath
    {
        public static readonly string SoftwareWorkPath = $"{System.Environment.CurrentDirectory}\\";

        public static readonly string SystemParamBasePath = $"{System.Environment.CurrentDirectory}\\Param\\System\\";

        public static readonly string SoftwareParamBasePath = $"{System.Environment.CurrentDirectory}\\Param\\Software\\";

        public static readonly string CraftParamBasePath = $"{System.Environment.CurrentDirectory}\\Param\\Craft\\";
    }
}

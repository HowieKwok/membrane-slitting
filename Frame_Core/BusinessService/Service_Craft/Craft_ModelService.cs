using DevMgmt.Cameras.Models;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Frame_Core
{
    public class Craft_ModelService : IBusinessService
    {
        public static CraftBase GetCraft(string CraftBasePath, MaterialCrafEnum craftType, string recipeName)
        {
            CraftBase mc = new CraftBase();
            int length = Project.Instance.M_CamParam?.l_CameraDevs?.Count ?? 10;
            switch (craftType)
            {
                case MaterialCrafEnum.激光模切一体机:
                    mc = mc.ReadCraft<CraftParam_DieCuttingCut>(CraftBasePath, craftType, recipeName, length);


                    break;
                default:
                    mc = mc.ReadCraft<CraftParam_DieCuttingCut>(CraftBasePath, craftType, recipeName, length);


                    break;
            }
            return mc;
        }

    }
}

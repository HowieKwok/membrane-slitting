using Frame_Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mef_VisionFrame
{
    public class Craft_ViewService: IBusinessService
    {
        /// <summary>
        /// 获取配方对应工艺参数和对应窗体
        /// </summary>
        /// <param name="craftType"></param>
        /// <param name="recipeName"></param>
        /// <param name="isGetView"></param>
        public static FormCraftBase GetView( FormCraftBase formCraftBase, MaterialCrafEnum craftType)
        {
            switch (craftType)
            {
                case MaterialCrafEnum.激光模切一体机:                  
                    if (formCraftBase != null)
                    {
                        formCraftBase.Close();
                        formCraftBase.Dispose();
                    }
                    formCraftBase = new Form_DieCuttingCut();
                    break;
                default:

                    if (formCraftBase != null)
                    {
                        formCraftBase.Close();
                        formCraftBase.Dispose();
                    }
                    formCraftBase = new Form_DieCuttingCut();
                    break;
            }

            formCraftBase.FormBorderStyle = FormBorderStyle.None;
            formCraftBase.Dock = DockStyle.Fill;
            formCraftBase.TopLevel = false;
            return formCraftBase;
        }

    }
}

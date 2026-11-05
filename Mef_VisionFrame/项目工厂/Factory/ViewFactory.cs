using Frame_Core;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mef_VisionFrame
{
    public class ViewFactory: IViewFactory
    {
        public UC_ToolView ToolView;
        public UCViewBase ProjectView;

        public void Load(VMFactory vMFactory)
        {
            MaterialCrafEnum crafEnum = Project.Instance.systemInfo.CraftType;
            if(crafEnum == MaterialCrafEnum.激光模切一体机)
            {
                ProjectView = new UC_DieCuttingCut(vMFactory.WorkVMBase as VM_DieCuttingCut) { Dock = DockStyle.Fill };
                ProjectView.Show();

                ToolView = new UC_ToolView(vMFactory.WorkVMBase) { Dock = DockStyle.Fill };
                ToolView.panel_DataShow.Controls.Clear();
                ToolView.panel_DataShow.Controls.Add(ProjectView);
                ToolView.Show();             
            }        
        }
    }
}

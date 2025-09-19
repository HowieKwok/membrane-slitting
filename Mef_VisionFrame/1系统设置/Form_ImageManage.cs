using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using EHolly.ImageTool;

using Frame_Core;

namespace Mef_VisionFrame
{
    public partial class Form_ImageManage : Form
    {
        public Form_ImageManage()
        {
            InitializeComponent();
            imageToolPanel1.Dock=DockStyle.Fill;  
        }

        private void Form_ImageManage_Load(object sender, EventArgs e)
        {
            if(Project.Instance.userManage.userData!=null)
            {
                if(Project.Instance.userManage.userData.CurrentUser== UserType.Admin)
                {
                    EHollyImageTool.EnableProSetting=true;
                }
            }
            imageToolPanel1.BackColor = Color.AntiqueWhite;
            this.BackColor = imageToolPanel1.BackColor;
        }
    }
}

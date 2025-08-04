using Frame_Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mef_VisionFrame
{
    public partial class Form_Pretargeting : Form
    {
        public Form_Pretargeting()
        {
            InitializeComponent();
        }

        private void Form_Pretargeting_Load(object sender, EventArgs e)
        {

            UpdateForm();


        }

        public void UpdateForm()
        {
            TLPanal_CamMain.Invoke(new Action(() =>
            {
                if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面A极片纠偏工位 ||
                  Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面B极片纠偏工位)
                {
                    TLPanal_CamMain.SetRow(TLPanal_Cam4, 0);
                    TLPanal_CamMain.SetColumn(TLPanal_Cam4, 0);
                    TLPanal_CamMain.SetRowSpan(TLPanal_Cam4, 1);
                    TLPanal_CamMain.SetColumnSpan(TLPanal_Cam4, 1);

                    TLPanal_CamMain.SetRow(TLPanal_Cam3, 0);
                    TLPanal_CamMain.SetColumn(TLPanal_Cam3, 1);
                    TLPanal_CamMain.SetRowSpan(TLPanal_Cam3, 1);
                    TLPanal_CamMain.SetColumnSpan(TLPanal_Cam3, 1);

                    TLPanal_CamMain.SetRow(TLPanal_Cam2, 1);
                    TLPanal_CamMain.SetColumn(TLPanal_Cam2, 0);
                    TLPanal_CamMain.SetRowSpan(TLPanal_Cam2, 1);
                    TLPanal_CamMain.SetColumnSpan(TLPanal_Cam2, 1);

                    TLPanal_CamMain.SetRow(TLPanal_Cam1, 1);
                    TLPanal_CamMain.SetColumn(TLPanal_Cam1, 1);
                    TLPanal_CamMain.SetRowSpan(TLPanal_Cam1, 1);
                    TLPanal_CamMain.SetColumnSpan(TLPanal_Cam1, 1);
                }
                else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面A极片纠偏工位 ||
                         Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面B极片纠偏工位)

                {

                    TLPanal_CamMain.SetRow(TLPanal_Cam1, 0);
                    TLPanal_CamMain.SetColumn(TLPanal_Cam1, 0);
                    TLPanal_CamMain.SetRowSpan(TLPanal_Cam1, 1);
                    TLPanal_CamMain.SetColumnSpan(TLPanal_Cam1, 1);

                    TLPanal_CamMain.SetRow(TLPanal_Cam2, 0);
                    TLPanal_CamMain.SetColumn(TLPanal_Cam2, 1);
                    TLPanal_CamMain.SetRowSpan(TLPanal_Cam2, 1);
                    TLPanal_CamMain.SetColumnSpan(TLPanal_Cam2, 1);

                    TLPanal_CamMain.SetRow(TLPanal_Cam3, 1);
                    TLPanal_CamMain.SetColumn(TLPanal_Cam3, 0);
                    TLPanal_CamMain.SetRowSpan(TLPanal_Cam3, 1);
                    TLPanal_CamMain.SetColumnSpan(TLPanal_Cam3, 1);

                    TLPanal_CamMain.SetRow(TLPanal_Cam4, 1);
                    TLPanal_CamMain.SetColumn(TLPanal_Cam4, 1);
                    TLPanal_CamMain.SetRowSpan(TLPanal_Cam4, 1);
                    TLPanal_CamMain.SetColumnSpan(TLPanal_Cam4, 1);

                }
            }));
        }
    }
}

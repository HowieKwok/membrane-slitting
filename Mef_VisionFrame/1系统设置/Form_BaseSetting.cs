using Frame_Core;
using LogTool;
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
    public partial class Form_BaseSetting : Form
    {
        SystemInfo systemInfo;
        public Form_BaseSetting(SystemInfo _systemInfo)
        {
            InitializeComponent();
            systemInfo = _systemInfo;
        }

        private void Form_BaseSetting_Load(object sender, EventArgs e)
        {
            try
            {
           
                dtp_dtCaliTimeBaiban.Value = systemInfo.djCaliTimeBaiban;
                dtp_dtCaliTimeYeBan.Value = systemInfo.djCaliTimeYeBan;
                cbx_djEnabled.Checked = systemInfo.djEnabled;




                lb_LastDJTime.Text= systemInfo.djLastOKTime.ToString("yyyy-MM-dd HH:mm:ss");
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }

        private void btn_SaveParam_Click(object sender, EventArgs e)
        {
            try
            {
     
                systemInfo.djCaliTimeBaiban = dtp_dtCaliTimeBaiban.Value;
                systemInfo.djCaliTimeYeBan = dtp_dtCaliTimeYeBan.Value;
                systemInfo.djEnabled = cbx_djEnabled.Checked;
                systemInfo.Save();
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }
    }
}

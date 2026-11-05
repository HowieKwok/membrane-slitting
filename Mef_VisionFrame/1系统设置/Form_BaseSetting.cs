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
        Param_PointInspection param;
        public Form_BaseSetting(Param_PointInspection _param)
        {
            InitializeComponent();
            param = _param;
        }

        private void Form_BaseSetting_Load(object sender, EventArgs e)
        {
            try
            {
           
                dtp_dtCaliTimeBaiban.Value = param.djCaliTimeBaiban;
                dtp_dtCaliTimeYeBan.Value = param.djCaliTimeYeBan;
                cbx_djEnabled.Checked = param.djEnabled;




                lb_LastDJTime.Text= param.djLastOKTime.ToString("yyyy-MM-dd HH:mm:ss");
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
     
                param.djCaliTimeBaiban = dtp_dtCaliTimeBaiban.Value;
                param.djCaliTimeYeBan = dtp_dtCaliTimeYeBan.Value;
                param.djEnabled = cbx_djEnabled.Checked;
                param.Save();
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }
    }
}

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

namespace Frame_Core
{
    public partial class CorrectForm : Form
    {
        CorrectManage correctManage;
        public CorrectForm(CorrectManage _correctManage)
        {
            correctManage = _correctManage;
            InitializeComponent();
        }

        private void CorrectForm_Load(object sender, EventArgs e)
        {
            try
            {
                cbx_JPType.Items.Clear();
                cbx_JPType.Items.Add("UDP");
                cbx_JPType.SelectedIndex = correctManage.correctParam.JPConnect_Type;
                tbxELMachineIP.Text = correctManage.correctParam.strELMachineIP;
                tbxELMachinePort.Value = correctManage.correctParam.strELMachinePort;
                tbxLocalMachineIP.Text = correctManage.correctParam.strLocalMachineIP;
                tbxLocalMachinePort.Value = correctManage.correctParam.strLocalMachinePort;

                cbx_ENAutoJP.Checked = correctManage.correctParam.ENAutoJP;
                cbx_JP_Sign.Checked = correctManage.correctParam.JP_Sign;
                num_TJDistance.Value = correctManage.correctParam.Correct_distance;
                num_TJJP_Pro.Value = (decimal)correctManage.correctParam.JP_Pro;

                num_maxTJJPnum.Value = (decimal)correctManage.correctParam.JP_MaxValue;
                num_minTJJPnum.Value = (decimal)correctManage.correctParam.JP_MinValue;
                num_maxBJTJNum.Value = correctManage.correctParam.JP_Max_errNUM;
                num_TJJPSign_LX_num.Value = correctManage.correctParam.Sign_LX_Num;

                num_Correctdata.Value = (decimal)correctManage.correctParam.Manu_JPValue;

                num_JP_HJLength.Value = (decimal)correctManage.correctParam.JP_HJLength;
                cb_EnHJJP.Checked = correctManage.correctParam.EnHJJP;

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            try
            {

                correctManage.correctParam.JPConnect_Type = cbx_JPType.SelectedIndex;
                correctManage.correctParam.strELMachineIP = tbxELMachineIP.Text.Replace(" ", "");
                correctManage.correctParam.strELMachinePort = (int)tbxELMachinePort.Value;
                correctManage.correctParam.strLocalMachineIP = tbxLocalMachineIP.Text.Replace(" ", "");
                correctManage.correctParam.strLocalMachinePort = (int)tbxLocalMachinePort.Value;

                correctManage.correctParam.ENAutoJP = cbx_ENAutoJP.Checked;
                correctManage.correctParam.JP_Sign = cbx_JP_Sign.Checked;
                correctManage.correctParam.Correct_distance = (int)num_TJDistance.Value;
                correctManage.correctParam.JP_Pro = (double)num_TJJP_Pro.Value;

                correctManage.correctParam.JP_MaxValue = (double)num_maxTJJPnum.Value;
                correctManage.correctParam.JP_MinValue = (double)num_minTJJPnum.Value;
                correctManage.correctParam.JP_Max_errNUM = (int)num_maxBJTJNum.Value;
                correctManage.correctParam.Sign_LX_Num = (int)num_TJJPSign_LX_num.Value;

                correctManage.correctParam.Manu_JPValue = (double)num_Correctdata.Value;

                correctManage.correctParam.JP_HJLength = (double)num_JP_HJLength.Value;
                correctManage.correctParam.EnHJJP = cb_EnHJJP.Checked;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        private void CorrectForm_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void btnELConnect_Click(object sender, EventArgs e)
        {
            correctManage.InitEL();
        }

        private void btnManuJP_Click(object sender, EventArgs e)
        {
            correctManage.SendOffsetData((double)num_Correctdata.Value, true);
        }

        private void btnHZJP_Click(object sender, EventArgs e)
        {
            correctManage.SendHomeMiddle();

        }
    }
}

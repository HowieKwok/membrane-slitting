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
    public partial class SlaveMachineForm : TCPClientForm
    {
        public SlaveMachineForm()
        {
            InitializeComponent();

        }


        public SlaveMachineForm(SlaveMachineManager _slaveMachineManager)
        {

            InitializeComponent();
            tcpCientManager = (SlaveMachineManager)_slaveMachineManager;

        }



        private void MDevPLCForm_FormClosing(object sender, FormClosingEventArgs e)
        {


        }
        protected override void btn_Save_Click(object sender, EventArgs e)
        {
            try
            {
                SlaveMachineParam   slaveMachineParam = (SlaveMachineParam)tcpCientManager.tcpClientParam;
                SlaveMachineManager  slaveMachineManager = (SlaveMachineManager)tcpCientManager;
                slaveMachineParam.ComType = cbx_ComType.SelectedIndex;
                slaveMachineParam.IsConnect = cbx_IsConnect.Checked;
                slaveMachineParam.LocalPC_IP = tbx_Local_IP.Text.Replace(" ", "");
                slaveMachineParam.PLC_IP = tbx_Dev_IP.Text.Replace(" ", "");
                slaveMachineParam.DevPort = (int)num_DevPort.Value;
                slaveMachineParam.nCamDBIndex = (int)num_CamDBIndex.Value;

                slaveMachineParam.Save();

            }
            catch (Exception ex)
            {

                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }
        //private string GetComTypeName(int index)
        //{
        //    return cbx_ComType.Items[index].ToString();
        //}

        protected override void btn_connectPLC_Click(object sender, EventArgs e)
        {
            SlaveMachineManager dBJCardManager = (SlaveMachineManager)tcpCientManager;
            dBJCardManager.InitPLC();
        }



        protected override void TCPClientForm_Load(object sender, EventArgs e)
        {
            try
            {


                tcpCientManager.tcpClientParam = (SlaveMachineParam)tcpCientManager.tcpClientParam.Read();
                string pathTemp = tcpCientManager.tcpClientParam.filePath;


                cbx_ComType.Items.Clear();
                cbx_ComType.Items.Add("TCP");

                cbx_ComType.SelectedIndex = tcpCientManager.tcpClientParam.ComType;
                cbx_IsConnect.Checked = tcpCientManager.tcpClientParam.IsConnect;
                tbx_Local_IP.Text = tcpCientManager.tcpClientParam.LocalPC_IP;
                tbx_Dev_IP.Text = tcpCientManager.tcpClientParam.PLC_IP;
                num_DevPort.Value = tcpCientManager.tcpClientParam.DevPort;
                num_CamDBIndex.Value = ((SlaveMachineParam)(tcpCientManager.tcpClientParam)).nCamDBIndex;

            }
            catch (Exception ex)
            {

                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SlaveMachineManager dBJCardManager = (SlaveMachineManager)tcpCientManager;
            using (DBJChanelForm dBJCardForm = new DBJChanelForm(dBJCardManager,1))
            {
                dBJCardForm.StartPosition = FormStartPosition.CenterScreen;
                dBJCardForm.ShowDialog();
            }

        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace Frame_Core
{
    public partial class DBJCardForm : TCPClientForm
    {

        public DBJCardForm()
        {
            InitializeComponent();
            
        }


        public DBJCardForm(DBJCardManager _DBJCardManager)
        {

            InitializeComponent();
            tcpCientManager = (DBJCardManager)_DBJCardManager;
           
        }



        private void MDevPLCForm_FormClosing(object sender, FormClosingEventArgs e)
        {


        }
        protected override void btn_Save_Click(object sender, EventArgs e)
        {
            try
            {
                DBJCardParam dBJCardParam = (DBJCardParam)tcpCientManager.tcpClientParam;
                DBJCardManager dBJCardManager = (DBJCardManager)tcpCientManager;
                dBJCardParam.ComType = cbx_ComType.SelectedIndex;
                dBJCardParam.IsConnect = cbx_IsConnect.Checked;
                dBJCardParam.LocalPC_IP = tbx_Local_IP.Text.Replace(" ", "");
                dBJCardParam.PLC_IP = tbx_Dev_IP.Text.Replace(" ", "");
                dBJCardParam.DevPort = (int)num_DevPort.Value;
                dBJCardParam.nCamDBIndex = (int)num_CamDBIndex.Value;
                dBJCardParam.AutoDB = cbx_AutoDB.Checked;
                dBJCardParam.Save();

                dBJCardManager.SendAutoDB(dBJCardParam.AutoDB);
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
            DBJCardManager dBJCardManager = (DBJCardManager)tcpCientManager;
            dBJCardManager.InitPLC();
        }

    

        protected override void TCPClientForm_Load(object sender, EventArgs e)
        {
            try
            {

             
                tcpCientManager.tcpClientParam = (DBJCardParam)tcpCientManager.tcpClientParam.Read();
                string pathTemp = tcpCientManager.tcpClientParam.filePath;
               

                cbx_ComType.Items.Clear();
                cbx_ComType.Items.Add("TCP");

                cbx_ComType.SelectedIndex = tcpCientManager.tcpClientParam.ComType;
                cbx_IsConnect.Checked = tcpCientManager.tcpClientParam.IsConnect;
                tbx_Local_IP.Text = tcpCientManager.tcpClientParam.LocalPC_IP;
                tbx_Dev_IP.Text = tcpCientManager.tcpClientParam.PLC_IP;
                num_DevPort.Value = tcpCientManager.tcpClientParam.DevPort;
                num_CamDBIndex.Value = ((DBJCardParam)(tcpCientManager.tcpClientParam)).nCamDBIndex;
                cbx_AutoDB.Checked = ((DBJCardParam)(tcpCientManager.tcpClientParam)).AutoDB;


            }
            catch (Exception ex)
            {

                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DBJCardManager dBJCardManager = (DBJCardManager)tcpCientManager;
            using (DBJChanelForm dBJCardForm = new DBJChanelForm(dBJCardManager))
            {
                dBJCardForm.StartPosition = FormStartPosition.CenterScreen;
                dBJCardForm.ShowDialog();
            }


        }
    }
}

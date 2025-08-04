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
    public partial class DBJCardForm : Form
    {
        DBJCardManager dBJCardManager;


        public DBJCardForm(DBJCardManager _DBJCardManager)
        {
            dBJCardManager = _DBJCardManager;
            InitializeComponent();
        }

        private void MDevPLCForm_Load(object sender, EventArgs e)
        {
            try
            {
                string pathTemp = dBJCardManager.dBJCardParam.filePath;
                dBJCardManager.dBJCardParam = (DBJCardParam)dBJCardManager.dBJCardParam.Read();
                dBJCardManager.dBJCardParam.filePath = pathTemp;
                cbx_ComType.Items.Clear();
                cbx_ComType.Items.Add("TCP");

                cbx_ComType.SelectedIndex = dBJCardManager.dBJCardParam.ComType;
                cbx_IsConnect.Checked = dBJCardManager.dBJCardParam.IsConnect;
                tbx_Local_IP.Text = dBJCardManager.dBJCardParam.LocalPC_IP;
                tbx_Dev_IP.Text = dBJCardManager.dBJCardParam.PLC_IP;
                num_DevPort.Value = dBJCardManager.dBJCardParam.DevPort;

                cbx_AutoDB.Checked = dBJCardManager.dBJCardParam.AutoDB;

            }
            catch (Exception ex)
            {

                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }

        }

        private void MDevPLCForm_FormClosing(object sender, FormClosingEventArgs e)
        {


        }
        private void btn_Save_Click(object sender, EventArgs e)
        {
            try
            {
                dBJCardManager.dBJCardParam.ComType = cbx_ComType.SelectedIndex;
                dBJCardManager.dBJCardParam.IsConnect = cbx_IsConnect.Checked;
                dBJCardManager.dBJCardParam.LocalPC_IP = tbx_Local_IP.Text.Replace(" ", "");
                dBJCardManager.dBJCardParam.PLC_IP = tbx_Dev_IP.Text.Replace(" ", "");
                dBJCardManager.dBJCardParam.DevPort = (int)num_DevPort.Value;

                dBJCardManager.dBJCardParam.AutoDB = cbx_AutoDB.Checked;
                dBJCardManager.dBJCardParam.Save();

                dBJCardManager.SendAutoDB(dBJCardManager.dBJCardParam.AutoDB);
            }
            catch (Exception ex)
            {

                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }
        private string GetComTypeName(int index)
        {
            return cbx_ComType.Items[index].ToString();
        }

        private void btn_connectPLC_Click(object sender, EventArgs e)
        {
            dBJCardManager.InitPLC();
        }


    }
}

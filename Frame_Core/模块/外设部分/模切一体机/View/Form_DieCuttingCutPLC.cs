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
    public partial class Form_DieCuttingCutPLC : Form
    {
        Server_DiecuttingCutPLC Server;
        public Form_DieCuttingCutPLC(Server_DiecuttingCutPLC server_DiecuttingCutPLC)
        {
            InitializeComponent();
            Server = server_DiecuttingCutPLC;
        }

        private void Form_DieCuttingCutPLC_Load(object sender, EventArgs e)
        {
            ParamToView();
        }

        public void ParamToView()
        {
            try
            {
                string[] comType = {
                  "Fins",
                   "CIP" ,
                    "S7"  ,
                     "ModbusTCP"  ,
                      "TCP"   ,
                        "UDP",};
                cbx_ComType.Items.Clear();
                cbx_ComType.DataSource = comType;

                cbx_ByteSelect.Items.Clear();
                cbx_ByteSelect.DataSource = Enum.GetValues(typeof(emDataFormat));


                cbx_ComType.SelectedIndex = Server.m_pLCDevParam.ComType;
                cbx_IsConnect.Checked = Server.m_pLCDevParam.IsConnect;
                cbx_ByteSelect.SelectedIndex = Server.m_pLCDevParam.nByteTrans;
                tbx_Local_IP.Text = Server.m_pLCDevParam.LocalPC_IP;
                tbx_Dev_IP.Text = Server.m_pLCDevParam.PLC_IP;
                num_DevPort.Value = Server.m_pLCDevParam.DevPort;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        public void ViewToParam()
        {
            try
            {

                Server.m_pLCDevParam.ComType = cbx_ComType.SelectedIndex;
                Server.m_pLCDevParam.IsConnect = cbx_IsConnect.Checked;
                Server.m_pLCDevParam.nByteTrans = cbx_ByteSelect.SelectedIndex;
                Server.m_pLCDevParam.LocalPC_IP = tbx_Local_IP.Text.Replace(" ", "");
                Server.m_pLCDevParam.PLC_IP = tbx_Dev_IP.Text.Replace(" ", "");
                Server.m_pLCDevParam.DevPort = (int)num_DevPort.Value;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            ViewToParam();
            Server.m_pLCDevParam.Save();
        }

  
        private void btn_connectPLC_Click(object sender, EventArgs e)
        {
            Server.Connect(Server.m_pLCDevParam.IsConnect, PLCTypeEnum.Omron, (CommunicationProtocolEnum)Server.m_pLCDevParam.ComType,
                     Server.m_pLCDevParam.PLC_IP,  Server.m_pLCDevParam.DevPort, (emDataFormat) Server.m_pLCDevParam.nByteTrans);
        }
  
        private void btn_closePLC_Click(object sender, EventArgs e)
        {
            Server.Disconnect();
        }
    }
}

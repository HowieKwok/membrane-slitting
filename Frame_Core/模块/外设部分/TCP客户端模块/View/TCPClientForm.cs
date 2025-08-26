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
    public partial class TCPClientForm : Form
    {
        protected TCPCientManager tcpCientManager;
        public TCPClientForm()
        {
            InitializeComponent();
            //tcpCientManager = new TCPCientManager();
            //tcpCientManager.tcpClientParam = new TCPClientParam();


        }
        public TCPClientForm(TCPCientManager  _tcpCientManager)
        {
            //tcpCientManager = new TCPCientManager();
            //tcpCientManager.tcpClientParam = new TCPClientParam();
            tcpCientManager = _tcpCientManager;
           
            InitializeComponent();
          
        }

        protected virtual  void TCPClientForm_Load(object sender, EventArgs e)
        {
            try
            {
                try
                {
              
                    if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                        return; // 设计器模式下不执行逻辑


                    tcpCientManager.tcpClientParam = (TCPClientParam)tcpCientManager.tcpClientParam.Read();
                    string pathTemp = tcpCientManager.tcpClientParam.filePath;
              
                    //tcpCientManager.tcpClientParam.filePath = pathTemp;
                    cbx_ComType.Items.Clear();
                    cbx_ComType.Items.Add("TCP");

                    cbx_ComType.SelectedIndex = tcpCientManager.tcpClientParam.ComType;
                    cbx_IsConnect.Checked = tcpCientManager.tcpClientParam.IsConnect;
                    tbx_Local_IP.Text = tcpCientManager.tcpClientParam.LocalPC_IP;
                    tbx_Dev_IP.Text = tcpCientManager.tcpClientParam.PLC_IP;
                    num_DevPort.Value = tcpCientManager.tcpClientParam.DevPort;



                }
                catch (Exception ex)
                {

                    System.Windows.Forms.MessageBox.Show(ex.ToString());
                }


            }
            catch (Exception ex)
            {

                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        //protected virtual void Form_Load(object sender, EventArgs e)
        //{
           
        //}

        protected virtual void btn_Save_Click(object sender, EventArgs e)
        {
            try
            {
                tcpCientManager.tcpClientParam.ComType = cbx_ComType.SelectedIndex;
                tcpCientManager.tcpClientParam.IsConnect = cbx_IsConnect.Checked;
                tcpCientManager.tcpClientParam.LocalPC_IP = tbx_Local_IP.Text.Replace(" ", "");
                tcpCientManager.tcpClientParam.PLC_IP = tbx_Dev_IP.Text.Replace(" ", "");
                tcpCientManager.tcpClientParam.DevPort = (int)num_DevPort.Value;

                tcpCientManager.tcpClientParam.Save();

              
            }
            catch (Exception ex)
            {

                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        protected  virtual void btn_connectPLC_Click(object sender, EventArgs e)
        {
            tcpCientManager.InitPLC();
        }
    }
}

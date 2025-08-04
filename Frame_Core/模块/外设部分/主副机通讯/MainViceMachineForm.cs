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
    public partial class MainSlaveMachineForm : Form
    {
        MainSlaveMachineParameter mainSlaveMachineParame;

        public MainSlaveMachineForm()
        {
            InitializeComponent();
            //mainSlaveMachineParame = Project.Instance.m_SlaveMachine.mainSlaveMachineParame;
        }

        private void MainSlaveMachineForm_Load(object sender, EventArgs e)
        {
            if (mainSlaveMachineParame == null) return;

            tbx_Server_IP.Text = mainSlaveMachineParame.Server_IP;
            tbx_Client_IP.Text = mainSlaveMachineParame.Client_IP;       
            cbx_IsConnect.Checked = mainSlaveMachineParame.IsConnect;
        }

        private void MainSlaveMachineForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (mainSlaveMachineParame == null) return;

            mainSlaveMachineParame.Server_IP = tbx_Server_IP.Text.Trim().Replace(" ", "");
            mainSlaveMachineParame.Client_IP = tbx_Client_IP.Text.Trim().Replace(" ", "");          
            mainSlaveMachineParame.IsConnect = cbx_IsConnect.Checked;
            mainSlaveMachineParame.Save();
        }
    }
}

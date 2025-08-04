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
    public partial class Form_MainPLC : Form
    {
        MainPLCManage mDevPLCManage;
        public Form_MainPLC(MainPLCManage _mDevPLCManage)
        {
            InitializeComponent();
            mDevPLCManage = _mDevPLCManage;
        }

        private void Form_PocketPLC_Load(object sender, EventArgs e)
        {
            LoadParam();
        }

        public void LoadParam()
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
                cbx_ComType.DataSource=comType;
                cbx_ComStationsanwahl.Items.Clear();
                cbx_ComStationsanwahl.DataSource = Enum.GetValues(typeof(ComStationsanwahlEnum));
                cbx_ByteSelect.Items.Clear();
                cbx_ByteSelect.DataSource = Enum.GetValues(typeof(emDataFormat));
                ControlTool.SetParamShow(cbx_ComType, ref mDevPLCManage.m_DevParam.ComType);
                ControlTool.SetParamShow(cbx_IsConnect, ref mDevPLCManage.m_DevParam.IsConnect);
                ControlTool.SetParamShow(cbx_ByteSelect, ref mDevPLCManage.m_DevParam.nByteTrans);
                ControlTool.SetParamShow(tbx_Local_IP, ref mDevPLCManage.m_DevParam.LocalPC_IP);
                ControlTool.SetParamShow(tbx_Dev_IP, ref mDevPLCManage.m_DevParam.PLC_IP);
                ControlTool.SetParamShow(num_DevPort, ref mDevPLCManage.m_DevParam.DevPort);

                ControlTool.SetParamShow(nud_TimeOutTime, ref mDevPLCManage.m_DevParam.TimeOutTime);

                ControlTool.SetParamShow(cbx_ComStationsanwahl, ref Project.Instance.systemInfo.ComStationsanwahl);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        public void SaveParam()
        {
            try
            {
                ControlTool.GetParamSave(cbx_ComType, ref mDevPLCManage.m_DevParam.ComType);
                ControlTool.GetParamSave(cbx_IsConnect, ref mDevPLCManage.m_DevParam.IsConnect);
                ControlTool.GetParamSave(tbx_Local_IP, ref mDevPLCManage.m_DevParam.LocalPC_IP);
                ControlTool.GetParamSave(tbx_Dev_IP, ref mDevPLCManage.m_DevParam.PLC_IP);
                ControlTool.GetParamSave(num_DevPort, ref mDevPLCManage.m_DevParam.DevPort);
                ControlTool.GetParamSave(cbx_ByteSelect, ref mDevPLCManage.m_DevParam.nByteTrans);

                ControlTool.GetParamSave(nud_TimeOutTime, ref mDevPLCManage.m_DevParam.TimeOutTime);
                mDevPLCManage.m_DevParam.Save();

                if (cbx_ComStationsanwahl.SelectedIndex != Project.Instance.systemInfo.ComStationsanwahl)
                {
                    ComStationsanwahlEnum comStationsanwahlEnum = (ComStationsanwahlEnum)cbx_ComStationsanwahl.SelectedIndex;
                    if (System.Windows.Forms.MessageBox.Show($"是否切换到{comStationsanwahlEnum}", "请停机状态切换,切换后软件自动关闭", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        ControlTool.GetParamSave(cbx_ComStationsanwahl, ref Project.Instance.systemInfo.ComStationsanwahl);
                        Project.Instance.systemInfo.Save();
                        Environment.Exit(0); //关闭整个工程
                    }
                       
                }              
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btn_connectPLC_Click(object sender, EventArgs e)
        {
            mDevPLCManage.Connect(mDevPLCManage.m_DevParam.IsConnect, PLCTypeEnum.Omron, CommunicationProtocolEnum.CIP,
                    mDevPLCManage.m_DevParam.PLC_IP, mDevPLCManage.m_DevParam.DevPort, (emDataFormat)mDevPLCManage.m_DevParam.nByteTrans);
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            SaveParam();
        }

        private void btn_closePLC_Click(object sender, EventArgs e)
        {
            mDevPLCManage.Disconnect();
        }
    }
}

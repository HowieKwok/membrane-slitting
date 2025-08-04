using log4net.Util;

using NPOI.SS.Formula.Functions;

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
    public partial class UC_Message : UserControl
    {
        private int _HeaderHeight = 30;
        [Category("自定义属性"), Description("TextFalse")]
        public int HeaderHeight
        {
            get { return _HeaderHeight; }
            set { _HeaderHeight = value; }
        }

        public UC_Message()
        {
            InitializeComponent();
        }

        private void UC_Message_Load(object sender, EventArgs e)
        {
            dgv_Alarm.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(dgv_Alarm, true);
            rtbx_Debug.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(rtbx_Debug, true);
            rtbx_Error.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(rtbx_Error, true);
            rtbx_Comm.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(rtbx_Comm, true);
        }
        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            timerUpdate.Enabled = false;
            try
            {
                if (MessageNotify.StopAlarmQueue.Count > 0)
                {
                    List<StopAlarmMessage> messages = MessageNotify.StopAlarmQueue.DequeueAll();
                    List<MessageShow> rows = new List<MessageShow>();
                    foreach (StopAlarmMessage message in messages)
                    {
                        string stopAlarmState = LanguageProject.GetLangeuageAttribute(message.StopAlarmState, Project.Instance.systemInfo.m_CurrentLanguageType);
                        List<string> row = new List<string>();
                        row.Add(message.time);
                        row.Add(stopAlarmState);
                        if (message.currentLanguageName != "") row.Add(message.currentLanguageName);
                        else row.Add(message.name);

                        MessageShow messageShow = new MessageShow();
                        messageShow.ShowRowMessage = row;

                        if (message.StopAlarmState == StopAlarmStateEnum.正常)
                        {
                            messageShow.color = Color.White;
                        }
                        else if (message.StopAlarmState == StopAlarmStateEnum.报警)
                        {
                            messageShow.color = Color.Yellow;
                        }
                        else if (message.StopAlarmState == StopAlarmStateEnum.停机)
                        {
                            messageShow.color = Color.Red;
                        }

                        rows.Add(messageShow);

                    }
                    UIService.Instance.AddAction(() =>
                    {
                        NDataGridView.AddRow(dgv_Alarm, rows, MessageNotify.ShowCount, true);
                    });

                }

                int index = tabControl1.SelectedIndex;
                int rowCount = 0;
                if (index == 0)
                {
                    rowCount = dgv_Alarm.Rows.Count;
                }
                else if (index == 1)
                {
                    if (MessageNotify.DebugLogs.Count > 0 && MessageNotify.DebugLogEnable)
                    {
                        MessageNotify.DebugLogEnable = false;
                        List<string> strings = MessageNotify.DebugLogs.ToArray().ToList();
                        UIService.Instance.AddAction(() =>
                        {
                            rtbx_Debug.Lines = strings.ToArray();
                            // 将 RichTextBox 滚动到最后一行
                            rtbx_Debug.SelectionStart = rtbx_Debug.Text.Length;
                            rtbx_Debug.ScrollToCaret();
                        });

                    }

                    rowCount = rtbx_Debug.Lines.Length;
                }
                else if (index == 2)
                {
                    if (MessageNotify.ErrorLogs.Count > 0 && MessageNotify.ErrorLogEnable)
                    {
                        MessageNotify.ErrorLogEnable = false;
                        List<string> strings = MessageNotify.ErrorLogs.ToArray().ToList();

                        UIService.Instance.AddAction(() =>
                        {
                            rtbx_Error.Lines = strings.ToArray();

                            // 将 RichTextBox 滚动到最后一行
                            rtbx_Error.SelectionStart = rtbx_Error.Text.Length;
                            rtbx_Error.ScrollToCaret();
                        });

                    }

                    rowCount = rtbx_Error.Lines.Length;
                }
                else if (index == 3)
                {
                    if (MessageNotify.CommLogs.Count > 0 && MessageNotify.CommLogEnable)
                    {
                        MessageNotify.CommLogEnable = false;
                        List<string> strings = MessageNotify.CommLogs.ToArray().ToList();

                        UIService.Instance.AddAction(() =>
                        {
                            rtbx_Comm.Lines = strings.ToArray();

                            // 将 RichTextBox 滚动到最后一行
                            rtbx_Comm.SelectionStart = rtbx_Comm.Text.Length;
                            rtbx_Comm.ScrollToCaret();
                        });

                    }

                    rowCount = rtbx_Comm.Lines.Length;
                }
                lb_LogRow.Text = $"{LB_Log.Text}({rowCount})";

            }
            catch { }
            timerUpdate.Enabled = true;
        }

        private void 清除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int index = tabControl1.SelectedIndex;
            if (index == 0)
            {
                MessageNotify.StopAlarmQueue.Clear();
                dgv_Alarm.Rows.Clear();
            }
            else if (index == 1)
            {
                MessageNotify.DebugLogs.Clear();
                rtbx_Debug.Clear();
            }
            else if (index == 2)
            {
                MessageNotify.ErrorLogs.Clear();
                rtbx_Error.Clear();
            }
            else if (index == 3)
            {
                MessageNotify.CommLogs.Clear();
                rtbx_Comm.Clear();
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

    }
}

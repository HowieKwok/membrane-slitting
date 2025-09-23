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
        /// <summary>
        /// 外界订阅：通知当前 SelectedIndex 变了
        /// </summary>
        public event Action<int> TabIndexChanged;

        private int _HeaderHeight = 30;
        #region ① 公开静态文本快照（给弹窗用）
        public static string DebugText = "";
        public static string ErrorText = "";
        public static string CommText = "";
        //public static int AlarmRowCount = 0;
        //public static string LastAlarmTime = string.Empty;
        #endregion

        #region ② 主从开关
        /// <summary>
        /// true 表示这个实例是弹窗里的“从”UC，只读镜像，不消费队列
        /// </summary>
        public bool IsSlave = false;
        #endregion


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
            /*---------- 从模式：用主窗体里的长度记忆 ----------*/
            if (IsSlave)
            {
                var host = this.Parent as FloatingMessageForm; // 拿到弹窗实例
                if (host == null) return;

                UIService.Instance.AddAction(() =>
                {
                    //// 1. 报警：拿业务快照，逐行追加
                    //var msgs = MessageNotify.StopAlarmQueue.DequeueAll();
                    //foreach (var m in msgs)
                    //    AddAlarmRow(m);   // 跟主窗体同一套逐行追加方法
                    //                      // 快照：把当前行数/最新时间留给弹窗
                    //UC_Message.AlarmRowCount = dgv_Alarm.RowCount;
                    //UC_Message.LastAlarmTime = msgs.LastOrDefault()?.time ?? string.Empty;
                    host.RefreshRichBox(rtbx_Debug, ref host.LastDebugLen, DebugText);
                    host.RefreshRichBox(rtbx_Error, ref host.LastErrorLen, ErrorText);
                    host.RefreshRichBox(rtbx_Comm, ref host.LastCommLen, CommText);
                });
              
                timerUpdate.Enabled = true;
                return;
            }
            /*――――――――――― 主模式：消费队列并显示 ――――――――――*/
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

                    NDataGridView.AddRow(dgv_Alarm, rows, MessageNotify.ShowCount, true);

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
                        rtbx_Debug.Lines = strings.ToArray();
                        // 将 RichTextBox 滚动到最后一行
                        rtbx_Debug.SelectionStart = rtbx_Debug.Text.Length;
                        rtbx_Debug.ScrollToCaret();

                    }

                    rowCount = rtbx_Debug.Lines.Length;
                }
                else if (index == 2)
                {
                    if (MessageNotify.ErrorLogs.Count > 0 && MessageNotify.ErrorLogEnable)
                    {
                        MessageNotify.ErrorLogEnable = false;
                        List<string> strings = MessageNotify.ErrorLogs.ToArray().ToList();

                        rtbx_Error.Lines = strings.ToArray();

                        // 将 RichTextBox 滚动到最后一行
                        rtbx_Error.SelectionStart = rtbx_Error.Text.Length;
                        rtbx_Error.ScrollToCaret();

                    }

                    rowCount = rtbx_Error.Lines.Length;
                }
                else if (index == 3)
                {
                    if (MessageNotify.CommLogs.Count > 0 && MessageNotify.CommLogEnable)
                    {
                        MessageNotify.CommLogEnable = false;
                        List<string> strings = MessageNotify.CommLogs.ToArray().ToList();

                        rtbx_Comm.Lines = strings.ToArray();

                        // 将 RichTextBox 滚动到最后一行
                        rtbx_Comm.SelectionStart = rtbx_Comm.Text.Length;
                        rtbx_Comm.ScrollToCaret();

                    }

                    rowCount = rtbx_Comm.Lines.Length;
                }

                lb_LogRow.Text = $"{LB_Log.Text}({rowCount})";

            }
            catch { }
            timerUpdate.Enabled = true;

            // 把当前文本拍个快照，供弹窗读取
            DebugText = rtbx_Debug.Text;
            ErrorText = rtbx_Error.Text;
            CommText = rtbx_Comm.Text;

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
            TabIndexChanged?.Invoke(tabControl1.SelectedIndex);
        }

        private void tableLayoutPanel_Main_DoubleClick(object sender, EventArgs e)
        {
            // 不再 new，用单例
            UIService.Instance.AddAction(() => ShowFloatingWindow(this));

        }



        private static FloatingMessageForm _floatForm;   // 单例

        /// <summary>
        /// 显示唯一弹窗（单例）
        /// </summary>
        public static void ShowFloatingWindow(UC_Message mainUC)
        {
            if (_floatForm == null || _floatForm.IsDisposed)
            {
                _floatForm = new FloatingMessageForm(mainUC);
                _floatForm.FormClosed += (_, __) => _floatForm = null;
                _floatForm.Show();
            }
            else
            {
                _floatForm.Activate();   // 已存在则前置
            }
        }

        /// <summary>
        /// 外部调用：静默切换页签，不触发事件循环
        /// </summary>
        public void SetTabIndexSilent(int idx)
        {
            if (tabControl1.SelectedIndex != idx)
                tabControl1.SelectedIndex = idx;
        }

        public void AddAlarmRow(StopAlarmMessage m)
        {
            string state = LanguageProject.GetLangeuageAttribute(
                               m.StopAlarmState,
                               Project.Instance.systemInfo.m_CurrentLanguageType);
            string name = string.IsNullOrEmpty(m.currentLanguageName)
                           ? m.name : m.currentLanguageName;

            int idx = dgv_Alarm.Rows.Add();
            var row = dgv_Alarm.Rows[idx];
            row.Cells[0].Value = m.time;
            row.Cells[1].Value = state;
            row.Cells[2].Value = name;

            switch (m.StopAlarmState)
            {
                case StopAlarmStateEnum.报警: row.DefaultCellStyle.BackColor = Color.Yellow; break;
                case StopAlarmStateEnum.停机: row.DefaultCellStyle.BackColor = Color.Red; break;
                default: row.DefaultCellStyle.BackColor = Color.White; break;
            }

            // 底部追新
            if (dgv_Alarm.RowCount > 0 &&
                dgv_Alarm.FirstDisplayedScrollingRowIndex +
                dgv_Alarm.DisplayedRowCount(true) >= dgv_Alarm.RowCount)
                dgv_Alarm.FirstDisplayedScrollingRowIndex = dgv_Alarm.RowCount - 1;
        }


    }
}

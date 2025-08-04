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
    public partial class Form_StopAlarm : Form
    {
        StopAlarmMessage stopAlarmessage;
        DateTime startTJ = DateTime.Now;
        DateTime EndTJ = DateTime.Now;

        public Form_StopAlarm(StopAlarmMessage _stopAlarmessage)
        {
            InitializeComponent();
            stopAlarmessage = _stopAlarmessage;
        }
        private void Form_StopAlarm_Load(object sender, EventArgs e)
        {
            ShowMessage(stopAlarmessage);
        }


        private void btn_Close_Click(object sender, EventArgs e)
        {
            CloseForm();
        }

        private void ShowMessage(StopAlarmMessage _stopAlarmessage)
        {
            try
            {
                startTJ = DateTime.Now;
                this.Text = stopAlarmessage.StopAlarmState.ToString();
                NTextBox.ClearLog(tbx_Name);
                NTextBox.ClearLog(tbx_Message);
                NTextBox.MessageShow(tbx_Name, new List<string> { stopAlarmessage.name, stopAlarmessage.currentLanguageName });

                NTextBox.MessageShow(tbx_Message, new List<string> { stopAlarmessage.describe, stopAlarmessage.solution });
             
            }
            catch (Exception)
            {

            }

        }
        private void CloseForm()
        {
            try
            {
                EndTJ = DateTime.Now;
                TJBJInfo.fileOperationINI.Write_SendLog_ini(TJBJInfo.TJBJInfoPath,
                    $"[{stopAlarmessage.time}-{stopAlarmessage.StopAlarmState}]-{stopAlarmessage.name}--{stopAlarmessage.currentLanguageName}]"
                     + "\r\n" + $"[{stopAlarmessage.describe}]"
                     + "\r\n开始弹框时间：" + startTJ.ToString("yyyy-MM-dd HH:mm:ss:fff")
                     + "\r\n结束弹框时间：" + EndTJ.ToString("yyyy-MM-dd HH:mm:ss:fff")
                     + "\r\n中间间隔时间：" + EndTJ.Subtract(startTJ).TotalMinutes.ToString("f2") + "min", "\r\n");

                GC.Collect();
                GC.WaitForPendingFinalizers();

                this.Close();
                this.Close();
            }
            catch (Exception)
            {

            }

        }


    }
}

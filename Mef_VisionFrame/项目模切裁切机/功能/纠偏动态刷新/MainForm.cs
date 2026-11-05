using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Mef_VisionFrame
{
    public partial class MainForm : Form
    {
        private readonly CorrectPosDeal _deal = new CorrectPosDeal(0,100);
        private int _frame;
        private MonitorForm _monitorForm;
        public MainForm() { InitializeComponent(); }
        private void OpenMonitor_Click(object sender, EventArgs e)
        {
            // 监控窗体全程只保留一个实例，重复点击只置前显示。
            if (_monitorForm == null || _monitorForm.IsDisposed)
            {
                _monitorForm = new MonitorForm(_deal, "1号裁切工位");
                _monitorForm.FormClosed += MonitorForm_FormClosed;
                _monitorForm.Show(this);
            }
            else
            {
                if (_monitorForm.WindowState == FormWindowState.Minimized)
                    _monitorForm.WindowState = FormWindowState.Normal;
                _monitorForm.Activate();
            }
        }

        private void MonitorForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // 窗体真正关闭后释放引用，下一次 Show 才创建新的实例。
            _monitorForm = null;
        }
        private void Update_Click(object sender, EventArgs e) { _deal.Update(++_frame, (double)Correction.Value); }
    }
}

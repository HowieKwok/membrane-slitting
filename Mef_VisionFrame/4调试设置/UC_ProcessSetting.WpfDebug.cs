using Mef_VisionFrame.调试设置.WPF;
using System;
using System.Windows.Forms;

namespace Mef_VisionFrame
{
    public partial class UC_ProcessSetting
    {
        private bool wpfDebugEntryAdded;
        //WPFDebugProcessWindow window;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // window = new WPFDebugProcessWindow();
            AddWpfDebugEntry();
        }

        private void AddWpfDebugEntry()
        {
            if (wpfDebugEntryAdded) return;
            wpfDebugEntryAdded = true;

            ToolStripButton btnWpfDebug = new ToolStripButton("WPF调试");
            btnWpfDebug.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnWpfDebug.Click += ToolStripBtn_WpfDebug_Click;

            toolStrip1.Items.Add(new ToolStripSeparator());
            toolStrip1.Items.Add(btnWpfDebug);
        }
       
        private void ToolStripBtn_WpfDebug_Click(object sender, EventArgs e)
        {
            WPFDebugProcessWindow window = new WPFDebugProcessWindow();
            window.ShowDialog();
        }
    }
}

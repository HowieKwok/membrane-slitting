using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Mef_VisionFrame
{
partial class MainForm
{
    private NumericUpDown Correction = null; private Button OpenMonitor = null; private Button UpdateButton = null;
    private void InitializeComponent()
    {
        Correction = new NumericUpDown { DecimalPlaces=3, Minimum=-10000, Maximum=10000, Width=120, Value=0.5M };
        OpenMonitor = new Button { Text="打开监控弹窗", AutoSize=true }; OpenMonitor.Click += OpenMonitor_Click;
        UpdateButton = new Button { Text="外部 Update", AutoSize=true }; UpdateButton.Click += Update_Click;
        Text="外部数据源模拟器"; Width=900; Height=190; BackColor=Color.FromArgb(7,16,29); ForeColor=Color.White;
        var title = new Label { Text="外部数据源模拟器", ForeColor=Color.FromArgb(112,217,255), Font=new Font("Microsoft YaHei",26,FontStyle.Bold), AutoSize=true, Location=new Point(24,18) };
        var label = new Label { Text="外部纠偏值", ForeColor=Color.FromArgb(139,170,194), AutoSize=true, Location=new Point(28,90) }; Correction.Location=new Point(110,84); OpenMonitor.Location=new Point(250,82); UpdateButton.Location=new Point(390,82);
        Controls.AddRange(new Control[]{title,label,Correction,OpenMonitor,UpdateButton});
    }
}
}

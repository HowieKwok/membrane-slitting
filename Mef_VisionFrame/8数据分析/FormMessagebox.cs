using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace Mef_VisionFrame
{
    public partial class FormMessagebox : Form
    {
        public FormMessagebox(string message, string caption)
        {
            InitializeComponent();
            this.Text = caption;
            this.Size = new System.Drawing.Size(300, 150);

            Label label = new Label { Text = message, AutoSize = true, Location = new System.Drawing.Point(10, 10) };
            Button button1 = new Button { Text = "截图", Location = new System.Drawing.Point(10, 70) };
            Button button2 = new Button { Text = "原图", Location = new System.Drawing.Point(150, 70) };

            button1.Click += (sender, e) => { DialogResult = DialogResult.OK; this.Close(); };
            button2.Click += (sender, e) => { DialogResult = DialogResult.Cancel; this.Close(); };

            this.Controls.Add(label);
            this.Controls.Add(button1);
            this.Controls.Add(button2);
        }

        public new DialogResult ShowDialog()
        {
            base.ShowDialog();
            return this.DialogResult;
        }
    }
}

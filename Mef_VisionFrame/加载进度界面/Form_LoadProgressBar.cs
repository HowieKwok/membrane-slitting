using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mef_VisionFrame
{
    public partial class Form_LoadProgressBar : Form
    {
        public bool closingFlag = false;
        private Action<Form_LoadProgressBar> action = null;

        public Form_LoadProgressBar()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (closingFlag)
            {
                closingFlag = false;
                this.Invoke(new Action(() => { this.Close(); this.Dispose(); }));
            }
        }
    }
}

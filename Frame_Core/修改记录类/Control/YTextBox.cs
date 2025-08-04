using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YYSD.UICore.Control
{
    public partial class YTextBox : UserControl, IYCustomControl
    {
        public YTextBox()
        {
            InitializeComponent();
        }
        public System.Windows.Forms.TextBox ValueObj { get { return this.txtObj; } }
        public System.Windows.Forms.Label NameObj { get { return this.lblObj; } }

        public object value
        {
            set
            {
                this.txtObj.Text = (string)value;
            }
            get
            {
                return this.txtObj.Text;
            }
        }
        public string name
        {
            set
            {
                if (value != null)
                {
                    this.lblObj.Text = (string)value;
                }
            }
            get
            {
                return this.lblObj.Text;
            }
        }
    }
}

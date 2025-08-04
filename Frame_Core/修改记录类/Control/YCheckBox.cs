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
    public partial class YCheckBox : UserControl, IYCustomControl
    {
        public YCheckBox()
        {
            InitializeComponent();
        }
        public System.Windows.Forms.CheckBox ValueObj { get { return this.cbObj; } }
        public System.Windows.Forms.Label NameObj { get { return this.lblObj; } }

        public object value
        {
            set
            {
                this.cbObj.Checked = Convert.ToBoolean(value);
            }
            get
            {
                return this.cbObj.Checked;
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

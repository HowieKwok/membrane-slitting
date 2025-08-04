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
    public partial class YComboBox : UserControl, IYCustomControl
    {
        public YComboBox()
        {
            InitializeComponent();
        }
        public System.Windows.Forms.ComboBox ValueObj { get { return this.comObj; } }
        public System.Windows.Forms.Label NameObj { get { return this.lblObj; } }

        public object value
        {
            set
            {
                int index = Convert.ToInt32(value);
                if (0 <= index && index < this.comObj.Items.Count)
                    this.comObj.SelectedIndex = index;
            }
            get
            {
                return this.comObj.SelectedIndex;
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

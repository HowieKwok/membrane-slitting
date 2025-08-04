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
    public partial class YNumericUpDown : UserControl, IYCustomControl
    {
        public YNumericUpDown()
        {
            InitializeComponent();
        }
        public System.Windows.Forms.NumericUpDown ValueObj { get { return this.numObj; } }
        public System.Windows.Forms.Label NameObj { get { return this.lblObj; } }

        public object value
        {
            set
            {
                int ivalue = Convert.ToInt32(value);
                double dvalue = Convert.ToDouble(value);
                if (ivalue != dvalue && ValueObj.DecimalPlaces == 0)
                    ValueObj.DecimalPlaces = 2;
                this.numObj.Value = Convert.ToDecimal(value);
            }
            get
            {
                return this.numObj.Value;
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

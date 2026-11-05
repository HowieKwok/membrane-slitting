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
    public partial class UCViewBase : UserControl
    {
        public UCViewBase()
        {
            InitializeComponent();         
        }

        public virtual void RefreshView()
        {

        }

        public virtual void InitView()
        {

        }
    }
}

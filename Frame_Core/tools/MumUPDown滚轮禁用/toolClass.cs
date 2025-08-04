using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Frame_Core
{
    public static class toolClass
    {
        #region 禁用鼠标滚轮更改NumericUpDown的参数
        public static void AllNumericUpDownControlSubscribeMouseWheelEvent(Control FatherControl)
        {
            foreach (Control control in FatherControl.Controls)
            {
                if (control.Controls != null)
                {
                    AllNumericUpDownControlSubscribeMouseWheelEvent(control);
                }
                if (control is NumericUpDown)
                {
                    var numericUpDown = control as NumericUpDown;
                    numericUpDown.MouseWheel += (object sender, MouseEventArgs e) =>
                    {
                        HandledMouseEventArgs h = e as HandledMouseEventArgs;
                        if (h != null)
                        {
                            h.Handled = true;
                        }
                    };
                }
            }
        }
        #endregion
    }
}

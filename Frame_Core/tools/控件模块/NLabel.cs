using LogTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Frame_Core
{
    public class NLabel
    {
        public static void UpdateControls(Label label, string text, bool isBeginInvoke = false)
        {
            try
            {
                if (label == null) return;
                if (text == null) return;
                if (label.Text == text) return;

                Action updateAction = () =>
                {
                    try
                    {
                        label.Text = text;
                    }
                    catch (Exception)
                    {
                        // Handle exception
                    }
                };
                Update(label, updateAction, isBeginInvoke);

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }

        private static void Update(Label label, Action updateAction, bool isBeginInvoke)
        {
            if (label == null || updateAction == null)
            {
                return;
            }

            if (isBeginInvoke)
            {
                label.BeginInvoke(new MethodInvoker(updateAction));
            }
            else
            {
                if (label.InvokeRequired)
                {
                    label.Invoke(new MethodInvoker(updateAction));
                }
                else
                {
                    updateAction();
                }
            }
        }
    }
}

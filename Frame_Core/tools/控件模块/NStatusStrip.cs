using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Frame_Core
{
    public class NStatusStrip
    {
        static bool InitFinsh = false;
        private static void IniStatStrip(StatusStrip statusStrip, SoftwareState[] softwareState)
        {
            try
            {
                statusStrip.BeginInvoke((EventHandler)delegate
                {
                    try
                    {
                        statusStrip.Items.Clear();
                        for (int i = 0; i < softwareState.Length; i++)
                        {
                            statusStrip.Items.Add(new ToolStripStatusLabel() { Name = $"toolStripStatusLabel{i}" });
                            statusStrip.Items[statusStrip.Items.Count - 1].Margin = new Padding { Right = 5 };
                        }
                        InitFinsh = true;
                    }
                    catch (Exception)
                    {


                    }

                });
            }
            catch (Exception)
            {

            }



        }
        public static void UpdateStatStrip(StatusStrip statusStrip, SoftwareState[] softwareState)
        {
            try
            {
                if (!InitFinsh)
                {
                    IniStatStrip(statusStrip, softwareState);
                }
                statusStrip.BeginInvoke((EventHandler)delegate
                {
                    try
                    {
                        for (int i = 0; i < softwareState.Length; i++)
                        {
                            if (statusStrip.Items[i].Text != softwareState[i].name)
                            {
                                statusStrip.Items[i].Text = softwareState[i].name;
                            }
                            Color currentColor;
                            if (softwareState[i].state == 1)
                            {
                                currentColor = Color.Green;
                            }
                            else if (softwareState[i].state == 0)
                            {
                                currentColor = Color.Red;
                            }
                            else
                            {
                                currentColor = Color.Yellow;
                            }
                            if (statusStrip.Items[i].BackColor != currentColor)
                            {
                                statusStrip.Items[i].BackColor = currentColor;
                            }

                        }
                    }
                    catch (Exception)
                    {

                    }

                });
            }
            catch (Exception)
            {


            }

        }

        public static void AddSoftwareState(string name, int state, ref SoftwareState[] softwareState)
        {
            Array.Resize<SoftwareState>(ref softwareState, softwareState.Length + 1);
            softwareState[softwareState.Length - 1] = new SoftwareState();
            softwareState[softwareState.Length - 1].name = name;
            softwareState[softwareState.Length - 1].state = state;
        }
    }

    public class SoftwareState
    {
        public string name = "";

        /// <summary>
        /// //1代表LightGreen，0代表Red，2代表Yellow
        /// </summary>
        public int state = StatusStripStruct.Normal;
    }

  
    public struct StatusStripStruct
    {
        /// <summary>
        /// 异常的
        /// </summary>
        public const int Abnormal = 0;

        /// <summary>
        /// 正常的
        /// </summary>
        public const int Normal = 1;
      
    }
  
}

using LogTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Frame_Core
{
    public class NTextBox
    {

        /// <summary>
        /// 添加Log显示
        /// </summary>
        /// <param name="runLog"></param>
        /// <param name="color"></param>
        public static void AddLogShow(TextBox textBox, List<string> runLog, int showLineCount, bool isBeginInvoke = false)
        {
            if (runLog == null) return;

            if (isBeginInvoke)
            {
                textBox.BeginInvoke((EventHandler)delegate
                {
                    ShowData(textBox, runLog, showLineCount);
                });
            }
            else
            {
                if (textBox.InvokeRequired)
                {
                    textBox.Invoke((EventHandler)delegate
                    {
                        ShowData(textBox, runLog, showLineCount);
                    });
                }
                else
                {
                    ShowData(textBox, runLog, showLineCount);
                }
            }
        }

        /// <summary>
        /// 添加Log显示
        /// </summary>
        /// <param name="runLog"></param>
        /// <param name="color"></param>
        public static void AddLogShow(TextBox textBox, string runLog, int showLineCount, bool isBeginInvoke = false)
        {
            if (runLog == null) return;

            List<string> runLogTemp = new List<string>();
            runLogTemp.Add(runLog);
            if (isBeginInvoke)
            {
                textBox.BeginInvoke((EventHandler)delegate
                {
                    ShowData(textBox, runLogTemp, showLineCount);
                });
            }
            else
            {
                if (textBox.InvokeRequired)
                {
                    textBox.Invoke((EventHandler)delegate
                    {
                        ShowData(textBox, runLogTemp, showLineCount);
                    });
                }
                else
                {
                    ShowData(textBox, runLogTemp, showLineCount);
                }
            }
        }

        private static void ShowData(TextBox textBox, List<string> runLog, int showLineCount)
        {
            try
            {
                string[] strLines = textBox.Lines;
                List<string> runLogTemp = strLines.ToList();

                for (int i = 0; i < runLog.Count; i++)
                {
                    runLogTemp.Add(runLog[i]);
                    runLogTemp.Add("\t");

                }

                while (runLogTemp.Count > showLineCount)
                {
                    runLogTemp.RemoveAt(0);
                }

                textBox.Lines = runLogTemp.ToArray();

                // 自动滚动到最后一行
                textBox.SelectionStart = textBox.Text.Length;
                textBox.ScrollToCaret();
            }
            catch (Exception ex)
            {

                Log4netRecord.ErrorLog.Debug(ex);
            }
        }

      
        /// <summary>
        /// 消息显示
        /// </summary>
        /// <param name="runLog"></param>
        /// <param name="color"></param>
        public static void MessageShow(TextBox textBox, List<string> runLog, bool isBeginInvoke = false)
        {
            if (runLog == null) return;

            if (isBeginInvoke)
            {
                textBox.BeginInvoke((EventHandler)delegate
                {
                    ShowData(textBox, runLog);
                });
            }
            else
            {
                if (textBox.InvokeRequired)
                {
                    textBox.Invoke((EventHandler)delegate
                    {
                        ShowData(textBox, runLog);
                    });
                }
                else
                {
                    ShowData(textBox, runLog);
                }
            }
        }

        /// <summary>
        /// 消息显示
        /// </summary>
        /// <param name="runLog"></param>
        /// <param name="color"></param>
        public static void MessageShow(TextBox textBox, string runLog, bool isBeginInvoke = false)
        {
            if (runLog == null) return;

            List<string> runLogTemp = new List<string>();
            runLogTemp.Add(runLog);
            if (isBeginInvoke)
            {
                textBox.BeginInvoke((EventHandler)delegate
                {
                    ShowData(textBox, runLogTemp);
                });
            }
            else
            {
                if (textBox.InvokeRequired)
                {
                    textBox.Invoke((EventHandler)delegate
                    {
                        ShowData(textBox, runLogTemp);
                    });
                }
                else
                {
                    ShowData(textBox, runLogTemp);
                }
            }
        }

        private static void ShowData(TextBox textBox, List<string> runLog)
        {
            try
            {
                if (runLog == null || runLog.Count == 0) return;
                textBox.Lines = runLog.ToArray();

                // 自动滚动到最后一行
                textBox.SelectionStart = textBox.Text.Length;
                textBox.ScrollToCaret();
            }
            catch (Exception ex)
            {

                Log4netRecord.ErrorLog.Debug(ex);
            }
        }

        /// <summary>
        /// 清除生产数据窗体
        /// </summary>
        public static void ClearLog(TextBox textBox)
        {
            if (true)
            {
                textBox.Invoke((EventHandler)delegate
                {
                    try
                    {
                        textBox.Text = "";

                    }
                    catch (Exception ex)
                    {

                        Log4netRecord.ErrorLog.Debug(ex);
                    }
                });
            }
        }
    }
}

using LogTool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Frame_Core
{
    public class ParamChangeLog
    {
        public static string ParamChangePath = "";
        private readonly static object ChangeLog = new object();
        public static void ParamChangeLog_Inspect(string _old, string _new, string userType, string userID, string menuItem, string ParamName, Boolean isRecord = false)
        {
            lock (ChangeLog)
            {
                try
                {
                    if (!isRecord)
                    {
                        if (_old != _new)
                        {
                            string filePath = ParamChangePath;

                            System.IO.StreamWriter writer = new System.IO.StreamWriter(filePath, true, Encoding.UTF8);
                            writer.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "  " + userType + "-" + userID + " 将 " + menuItem + "  " + ParamName + ":  " + _old + " 改为:  " + _new);
                            writer.Flush();
                            writer.Close();
                            writer.Dispose();
                        }
                    }
                    else
                    {
                        string filePath = ParamChangePath;

                        System.IO.StreamWriter writer = new System.IO.StreamWriter(filePath, true, Encoding.UTF8);
                        writer.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "  " + userType + "-" + userID + " 将 " + menuItem + "  " + ParamName);
                        writer.Flush();
                        writer.Close();
                        writer.Dispose();
                    }


                }
                catch (Exception ex)
                {
                     Log4netRecord.ErrorLog.Fatal(ex);
                    //  System.Windows.Forms.MessageBox.Show(ex.Message + ex.StackTrace);
                }
            }
        }


        public static string CailbChangePath = "";
        private readonly static object CalibChangeLog = new object();
        public static void CalibrationLog_Inspect(string _old, string _new, string userType, string userID, string menuItem, string ParamName, Boolean isRecord = false)
        {
            lock (CalibChangeLog)
            {
                try
                {
                    if (!isRecord)
                    {
                        if (_old != _new)
                        {
                            string filePath = CailbChangePath;

                            System.IO.StreamWriter writer = new System.IO.StreamWriter(filePath, true, Encoding.UTF8);
                            writer.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "  " + userType + "-" + userID + " 将 " + menuItem + "  " + ParamName + ":  " + _old + " 改为:  " + _new);
                            writer.Flush();
                            writer.Close();
                            writer.Dispose();
                        }
                    }

                }
                catch (Exception ex)
                {
                     Log4netRecord.ErrorLog.Fatal(ex);
                    //  System.Windows.Forms.MessageBox.Show(ex.Message + ex.StackTrace);
                }
            }
        }
    }
}

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

    public class FileOperationINI
    {
        private readonly object SendLog = new object();
        private readonly object ReadLog = new object();

        public static void WriteLog_ini(string filePath, string message)
        {
            try
            {
                //if (!Directory.Exists(filePath))
                //    Directory.CreateDirectory(filePath);
                System.IO.StreamWriter writer = new System.IO.StreamWriter(filePath, true, Encoding.UTF8);
                writer.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "     " + message);

                writer.Flush();
                writer.Close();
                writer.Dispose();
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug($"FilePath:{filePath}\r\nMessage:{message}", ex);
            }
        }
        public void Write_SendLog_ini(string filePath, string message, string track)
        {
            lock (SendLog)
            {

                try
                {
                    //if (!Directory.Exists(filePath))
                    //    Directory.CreateDirectory(filePath);
                    System.IO.StreamWriter writer = new System.IO.StreamWriter(filePath, true, Encoding.UTF8);
                    writer.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "     " + message + "\t" + track);

                    writer.Flush();
                    writer.Close();
                    writer.Dispose();
                }
                catch (Exception ex)
                {
                    Log4netRecord.ErrorLog.Debug(ex + "\n发送路径：" + filePath + "\n信息：\n" + message + "  " + track);
                }

            }
        }

        public void Write_SendLog_ini(string filePath, string message)
        {
            lock (SendLog)
            {

                try
                {
                    //if (!Directory.Exists(filePath))
                    //    Directory.CreateDirectory(filePath);
                    System.IO.StreamWriter writer = new System.IO.StreamWriter(filePath, true, Encoding.UTF8);
                    writer.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " " + message);

                    writer.Flush();
                    writer.Close();
                    writer.Dispose();
                }
                catch (Exception ex)
                {
                    Log4netRecord.ErrorLog.Debug(ex + "\n发送路径：" + filePath + "\n信息：\n" + message);
                }

            }
        }


        public void Write_SendLog_ini(string filePath, List<string> message, string track)
        {
            lock (SendLog)
            {

                try
                {
                    //if (!Directory.Exists(filePath))
                    //    Directory.CreateDirectory(filePath);
                    System.IO.StreamWriter writer = new System.IO.StreamWriter(filePath, true, Encoding.UTF8);
                    for (int i = 0; i < message.Count; i++)
                    {
                        writer.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "     " + message[i] + "\t" + track);
                    }


                    writer.Flush();
                    writer.Close();
                    writer.Dispose();
                }
                catch (Exception ex)
                {
                    Log4netRecord.ErrorLog.Debug(ex + "\n发送路径：" + filePath + "\n信息：\n" + message + "  " + track);
                }

            }
        }

        public void Write_ReadLog_ini(string filePath, string message, string track)
        {
            lock (ReadLog)
            {

                try
                {
                    //if (!Directory.Exists(filePath))
                    //    Directory.CreateDirectory(filePath);
                    System.IO.StreamWriter writer = new System.IO.StreamWriter(filePath, true, Encoding.UTF8);
                    writer.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "   " + message + "\t" + track);

                    writer.Flush();
                    writer.Close();
                    writer.Dispose();

                }
                catch (Exception ex)
                {
                    Log4netRecord.ErrorLog.Debug(ex + "\n读取路径：" + filePath + "\n信息：\n" + message + "  " + track);
                }

            }
        }

    }
}



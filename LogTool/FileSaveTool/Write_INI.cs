using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogTool
{
    internal class Write_INI
    {
        private readonly object SendLog = new object();

        public void Write_SendLog_ini(string filePath, string source, string message)
        {
            lock (SendLog)
            {
                //if (!Directory.Exists(filePath))
                //    Directory.CreateDirectory(filePath);
                System.IO.StreamWriter writer = new System.IO.StreamWriter(filePath, true, Encoding.UTF8);
                writer.WriteLine(DateTime.Now.ToString("yyyy-MM-dd  HH-mm-ss-fff") + "--" + message);

                writer.Flush();
                writer.Close();
                writer.Dispose();

            }
        }
    }
}

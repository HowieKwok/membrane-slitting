using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogTool
{
    /// <summary>
    /// 日志参数
    /// </summary>
    public class LogParam : XmlSerializationOperation
    {
        /// <summary>
        /// 日志保存路径
        /// </summary>
        public string LogSavePath = "E:\\Log";

        /// <summary>
        /// 保存文件大小(M)
        /// </summary>
        public int SaveFileSize = 2;

        /// <summary>
        /// 日志保存天数
        /// </summary>
        public int LogSaveDay = 30;
    }
}

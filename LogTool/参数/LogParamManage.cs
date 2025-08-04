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
    public static class LogParamManage
    {

        /// <summary>
        /// 获取日志参数，获取异常回返回Null值
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static void GetLogParam(string path, ref LogParam logParam)
        {
            try
            {
                logParam.filePath = path;
                logParam = (LogParam)logParam.Read();
                logParam.filePath = path;

            }
            catch (Exception)
            {


            }
        }
        /// <summary>
        /// 保存日志参数
        /// </summary>
        /// <param name="path"></param>
        /// <param name="logParam"></param>
        public static void SaveLogParam(string path, LogParam logParam)
        {
            try
            {
                logParam.filePath = path;
                logParam.Save();
            }
            catch (Exception)
            {


            }

        }
    }
}

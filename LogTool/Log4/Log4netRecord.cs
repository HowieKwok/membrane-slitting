using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Layout;
using log4net.Repository;
using log4net.Repository.Hierarchy;
using static log4net.Appender.RollingFileAppender;


namespace LogTool
{
    /// <summary>
    ///日志记录工具
    /// </summary>
    public static class Log4netRecord
    {
        //启用配置方式
        //[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config", ConfigFileExtension = "config", Watch = true)]
        //显式代码加载1.使用assembly，2.加载配置文件到info并执行Configure(info)
        //日志级别：高到底分别为：OFF > FATAL > ERROR > WARN > INFO > DEBUG  > ALL

        /// <summary>
        /// 报错日志
        /// </summary>
        public static log4net.ILog ErrorLog = log4net.LogManager.GetLogger(LogTypeEnum.ErrorLog.ToString());//记录日志   

        /// <summary>
        /// 运行日志
        /// </summary>
        public static log4net.ILog RunLog = log4net.LogManager.GetLogger(LogTypeEnum.RunLog.ToString());//记录日志

        /// <summary>
        /// 设置日志
        /// </summary>
        public static log4net.ILog SetLog = log4net.LogManager.GetLogger(LogTypeEnum.SetLog.ToString());//记录日志

        /// <summary>
        /// 相机日志
        /// </summary>
        public static log4net.ILog CamLog = log4net.LogManager.GetLogger(LogTypeEnum.CamLog.ToString());//记录日志

        /// <summary>
        /// 通讯日志
        /// </summary>
        public static log4net.ILog CommLog = log4net.LogManager.GetLogger(LogTypeEnum.CommLog.ToString());//记录日志




        /// <summary>
        /// 初始化Log4net
        /// </summary>
        /// <param name="_logParam"></param>
        public static void InitLog4netConfigure(LogParam _logParam)
        {
            try
            {
                string pattern = "%n异常时间：%d [%t] %n异常级别：%-5p %n异常位置：[%thread] (%file:%line) %n消息描述：%message%n异常：%exception%n%n异常类名：%class%n ";
                ErrorLog = LogObject.SetLog(_logParam.LogSavePath, LogTypeEnum.ErrorLog.ToString(), 50, 100, RollingMode.Composite, pattern);
                SetLog = LogObject.SetLog(_logParam.LogSavePath, LogTypeEnum.SetLog.ToString(), 50, 100, RollingMode.Composite);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        static bool InitProdFinish = false;
        /// <summary>
        /// 初始化生产数据日志
        /// </summary>
        /// <param name="filePath"></param>
        private static void InitProdLog(string filePath)
        {
            try
            {
                if (!InitProdFinish)
                {
                    RunLog = LogObject.SetLog(filePath, LogTypeEnum.RunLog.ToString(), 2, 100, RollingMode.Composite);
                    CamLog = LogObject.SetLog(filePath, LogTypeEnum.CamLog.ToString(), 2, 100, RollingMode.Composite);
                    CommLog = LogObject.SetLog(filePath, LogTypeEnum.CommLog.ToString(), 2, 100, RollingMode.Composite);
                    InitProdFinish = true;
                }

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }

        }

        /// <summary>
        /// 切换生产数据日志路径
        /// </summary>
        /// <param name="filePath"></param>
        public static void ChangeProdLogPath(string filePath)
        {
            try
            {
                InitProdLog(filePath);
                LogObject.ChangeLogPath(filePath, LogTypeEnum.RunLog.ToString());
                LogObject.ChangeLogPath(filePath, LogTypeEnum.CamLog.ToString());
                LogObject.ChangeLogPath(filePath, LogTypeEnum.CommLog.ToString());
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }
    }
}
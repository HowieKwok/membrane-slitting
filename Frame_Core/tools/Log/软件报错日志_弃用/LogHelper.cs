using NLog;
using NLog.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    public  class LogHelper
    {
        public static Logger logger = LogManager.GetCurrentClassLogger();
        public static void InitNlog(string path)
        {
            //初始化配置日志
            string config_file = Path.Combine(System.Environment.CurrentDirectory, "Nlog2", "NLog2.config");//Nlog1这个配置文件有点问题写不进数据
            if (File.Exists(config_file))
                NLog.LogManager.Configuration = new NLog.Config.XmlLoggingConfiguration(config_file);

            Logger logger1 = LogManager.GetCurrentClassLogger();//获取的是rules-name为*
            Logger logger2 = LogManager.GetLogger("PLCSendLog");//获取的是rules-name为PLCSendLog
        }


        /// <summary>
        /// 日志等级
        /// </summary>
        public enum LogLevel
        {
            /// <summary>
            /// 最常见的记录信息，一般用于普通输出
            /// </summary>
            Trace = 0,
            /// <summary>
            /// 记录信息，，一般用来调试程序
            /// </summary>
            Debug,
            /// <summary>
            /// 信息类型的消息
            /// </summary>
            Info,
            /// <summary>
            /// 警告信息
            /// </summary>
            Warn,
            /// <summary>
            /// 错误信息
            /// </summary>
            Error,
            /// <summary>
            /// 致命异常信息
            /// </summary>
            Fatal
        }
        /// <summary>
        /// 添加日志
        /// </summary>
        /// <param name="message">日志信息</param>
        /// <param name="logLevel">日志等级</param>
        public static void AddLogger(string message, LogLevel logLevel = LogLevel.Debug)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                    logger.Trace(message);
                    break;
                case LogLevel.Debug:
                    logger.Debug(message);
                    break;
                case LogLevel.Info:
                    logger.Info(message);
                    break;
                case LogLevel.Warn:
                    logger.Warn(message);
                    break;
                case LogLevel.Error:
                    logger.Error(message);
                    break;
                case LogLevel.Fatal:
                    logger.Fatal(message);
                    break;
                default:
                    logger.Debug(message);
                    break;
            }
        }
        /// <summary>
        /// 添加日志（catch到的Exception建议使用此方法）
        /// </summary>
        /// <param name="exception">错误信息</param>
        /// <param name="logLevel">日志等级</param>
        public static void AddLogger(Exception exception, LogLevel logLevel = LogLevel.Error)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                    logger.Trace(exception);
                    break;
                case LogLevel.Debug:
                    logger.Debug(exception);
                    break;
                case LogLevel.Info:
                    logger.Info(exception);
                    break;
                case LogLevel.Warn:
                    logger.Warn(exception);
                    break;
                case LogLevel.Error:
                    logger.Error(exception);
                    break;
                case LogLevel.Fatal:
                    logger.Fatal(exception);
                    break;
                default:
                    logger.Error(exception);
                    break;
            }
        }
        /// <summary>
        /// 添加日志（catch到的Exception建议使用此方法）
        /// </summary>
        /// <param name="exception">错误信息</param>
        /// <param name="message">错误描述</param>
        /// <param name="logLevel">日志等级</param>
        public static void AddLogger(Exception exception, string message, LogLevel logLevel = LogLevel.Error)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                    logger.Trace(exception, message);
                    break;
                case LogLevel.Debug:
                    logger.Debug(exception, message);
                    break;
                case LogLevel.Info:
                    logger.Info(exception, message);
                    break;
                case LogLevel.Warn:
                    logger.Warn(exception, message);
                    break;
                case LogLevel.Error:
                    logger.Error(exception, message);
                    break;
                case LogLevel.Fatal:
                    logger.Fatal(exception, message);
                    break;
                default:
                    logger.Error(exception, message);
                    break;
            }
        }
        /// <summary>
        /// 释放日志资源
        /// </summary>
        public static void Dispose()
        {
            NLog.LogManager.Shutdown();
        }
    }
}


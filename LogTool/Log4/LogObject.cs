using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static log4net.Appender.RollingFileAppender;

namespace LogTool
{
    /// <summary>
    /// 日志对象类
    /// </summary>
    public static class LogObject
    {
        static Dictionary<string, ILog> dicLog = new Dictionary<string, ILog>();

        public static ILog SetLog(string filePath, string loggerName, uint maxFileSize, int maxSizeRollBackups, RollingMode rollingMode)
        {
            if (!dicLog.ContainsKey(loggerName) && !LogIsExist(loggerName))/// 判断日志文件是否存在，不存在则创建
            {
                // 创建一个与调用这个方法的程序集相关联的日志仓库
                ILoggerRepository repository = LogManager.CreateRepository(Assembly.GetCallingAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));

                // 创建一个RollingFileAppender实例并配置其属性
                RollingFileAppender runLogAppender = new RollingFileAppender
                {
                    Name = $"{loggerName}Appender", // Appender的名称
                    File = filePath + "\\" + loggerName.ToString() + "\\", // 日志文件的基础路径
                    AppendToFile = true, // 设置为true表示追加到现有文件
                    MaxSizeRollBackups = maxSizeRollBackups, // 最多保留100个备份文件
                    MaxFileSize = maxFileSize * 1024 * 1024, // 单个日志文件的最大大小
                    Encoding = System.Text.Encoding.UTF8, // 文件的编码格式
                    PreserveLogFileNameExtension = true, // 在滚动时保持文件扩展名
                    StaticLogFileName = false, // 文件名将根据滚动策略变化
                    DatePattern = "yyyyMMdd\".log\"", // 定义文件名中日期的格式，注意检查路径格式是否正确
                    RollingStyle = rollingMode, //Date、 Composite使用基于时间和文件大小的复合滚动
                    Layout = new PatternLayout("%d [%t] %message%n ") // 定义日志的输出格式
                };
                runLogAppender.ActivateOptions(); // 激活配置选项，这一步是必须的

                // 用于将appender添加到logger，并设置日志级别        
                ConfigureLogger(repository, loggerName, Level.All, runLogAppender);

                // 获取指定名称的日志器（ILog实例）
                dicLog[loggerName] = log4net.LogManager.GetLogger(loggerName.ToString());
            }
            else
            {
                ChangeLogPath(filePath, loggerName);
            }

            ILog LogTemp = dicLog[loggerName];

            // 返回日志器实例，可以用于进行日志记录
            return LogTemp;
        }
        public static ILog SetLog(string filePath, string loggerName, uint maxFileSize, int maxSizeRollBackups, RollingMode rollingMode, string pattern)
        {
            if (!dicLog.ContainsKey(loggerName) && !LogIsExist(loggerName))/// 判断日志文件是否存在，不存在则创建
            {
                // 创建一个与调用这个方法的程序集相关联的日志仓库
                ILoggerRepository repository = LogManager.CreateRepository(Assembly.GetCallingAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));

                // 创建一个RollingFileAppender实例并配置其属性
                RollingFileAppender runLogAppender = new RollingFileAppender
                {
                    Name = $"{loggerName}Appender", // Appender的名称
                    File = filePath + "\\" + loggerName.ToString() + "\\", // 日志文件的基础路径
                    AppendToFile = true, // 设置为true表示追加到现有文件
                    MaxSizeRollBackups = maxSizeRollBackups, // 最多保留100个备份文件
                    MaxFileSize = maxFileSize * 1024 * 1024, // 单个日志文件的最大大小
                    Encoding = System.Text.Encoding.UTF8, // 文件的编码格式
                    PreserveLogFileNameExtension = true, // 在滚动时保持文件扩展名
                    StaticLogFileName = false, // 文件名将根据滚动策略变化
                    DatePattern = "yyyyMMdd\".log\"", // 定义文件名中日期的格式，注意检查路径格式是否正确
                    RollingStyle = rollingMode, //Date、 Composite使用基于时间和文件大小的复合滚动
                    Layout = new PatternLayout(pattern) // 定义日志的输出格式{"%n异常时间：%d [%t] %n异常级别：%-5p %n异常位置：[%thread] (%file:%line) %n消息描述：%message%n异常：%exception%n%n异常类名：%class%n "}
                };
                runLogAppender.ActivateOptions(); // 激活配置选项，这一步是必须的

                // 用于将appender添加到logger，并设置日志级别        
                ConfigureLogger(repository, loggerName, Level.All, runLogAppender);

                // 获取指定名称的日志器（ILog实例）
                dicLog[loggerName] = log4net.LogManager.GetLogger(loggerName.ToString());
            }
            else
            {
                ChangeLogPath(filePath, loggerName);
            }

            ILog LogTemp = dicLog[loggerName];

            // 返回日志器实例，可以用于进行日志记录
            return LogTemp;
        }

        /// <summary>
        /// 修改日志文件路径
        /// </summary>
        /// <param name="loggerName"></param>
        /// <param name="filePath"></param>
        public static void ChangeLogPath(string filePath, string loggerName)
        {
            var repository = log4net.LogManager.GetRepository();
            var appenders = repository.GetAppenders();

            var appenders1 = repository.GetLogger(loggerName.ToString()).Repository.GetAppenders();
            foreach (var appender in appenders1)
            {
                if (appender.Name.Contains(loggerName.ToString()))
                {
                    RollingFileAppender rollingFileAppender = (RollingFileAppender)appender;

                    rollingFileAppender.File = filePath + "\\" + loggerName.ToString() + "\\";

                    // 激活appender的选项  
                    rollingFileAppender.ActivateOptions();
                }

            }
        }

        /// <summary>
        /// 日志是否存在
        /// </summary>
        /// <param name="loggerName"></param>
        /// <returns></returns>
        public static bool LogIsExist(string loggerName)
        {
            bool IsExist = false;
            var repository = log4net.LogManager.GetRepository();
            var appenders = repository.GetAppenders();

            var appenders1 = repository.GetLogger(loggerName.ToString()).Repository.GetAppenders();
            foreach (var appender in appenders1)
            {
                if (appender.Name.Contains(loggerName.ToString()))
                {
                    IsExist = true;
                }

            }
            return IsExist;
        }
        /// <summary>
        /// 配置日志器
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="loggerName"></param>
        /// <param name="level"></param>
        /// <param name="appender"></param>
        private static void ConfigureLogger(ILoggerRepository repository, string loggerName, Level level, IAppender appender)
        {
            var logger = (log4net.Repository.Hierarchy.Logger)repository.GetLogger(loggerName.ToString());
            logger.Level = level; // 设置日志器的日志级别。日志级别控制哪些级别的日志消息将被记录。
            logger.AddAppender(appender);
            repository.Configured = true;  // 将日志仓库的配置状态设置为true，表示已经完成了配置。
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using NLog.Targets;
using System.IO;

namespace Frame_Core
{

    public class NlogRecord
    {
        public static  Logger logger;

        public static void InitNlog()
        {
            //创建一个配置文件对象
            var config = new NLog.Config.LoggingConfiguration();
            //创建日志写入目的地

            var logfile = new NLog.Targets.FileTarget("logfile")
            {
                FileName = $"logs/{DateTime.Now.ToString("yyyy-MM-dd")}.txt",

                Layout = "日志时间：${longdate}${newline}日志级别：${level:uppercase=true} ${newline}代码位置：[${threadid}] (${callsite-filename}:${callsite-linenumber}) ${newline}消息描述：${message}${newline}日志类名：${logger}${newline}附加信息：${event-properties:Extend:objectpath=OperateName}  ${event-properties:Extend:objectpath=OperateId}${newline}异常：${onexception:Exception}${exception:format=toString}${newline}",

                ArchiveFileName = "${basedir}/软件日志/错误日志/${shortdate}/${date:format=yyyyMMdd}-{#####}.log",

                ArchiveAboveSize = 104857600,

                ArchiveNumbering = ArchiveNumberingMode.Sequence,

                MaxArchiveFiles = 1000,

                ConcurrentWrites = false,

                KeepFileOpen = true,

                Encoding = Encoding.UTF8,

                OpenFileCacheTimeout = 30
            };

            //添加日志路由规则           
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile, "logfile");
      
            //配置文件生效
            LogManager.Configuration = config;

            //logger = LogManager.GetCurrentClassLogger();
            logger = LogManager.GetLogger("logfile");
        }
    
    }
}

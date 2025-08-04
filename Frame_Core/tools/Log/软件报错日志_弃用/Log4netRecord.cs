using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;


namespace Frame_Core
{
    //public class Log4netRecordTest
    //{
    //    //启用配置方式
    //    //[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config", ConfigFileExtension = "config", Watch = true)]
    //    //显式代码加载1.使用assembly，2.加载配置文件到info并执行Configure(info)
    //    //日志级别：高到底分别为：OFF > FATAL > ERROR > WARN > INFO > DEBUG  > ALL

    //    public static log4net.ILog LogDebug = log4net.LogManager.GetLogger("Debug");//记录日志  

    //    //public static log4net.ILog LogDebugTest1 = log4net.LogManager.GetLogger("DebugTest1");//调试日志  

    //    //public static log4net.ILog LogDebugTest2 = log4net.LogManager.GetLogger("DebugTest2");//调试日志  
    //    public static void InitLog4net()
    //    {
    //        string config_file = Path.Combine(System.Environment.CurrentDirectory, "系统配置文件勿动", "Log4net.config");
    //        if (File.Exists(config_file))              
    //              log4net.Config.XmlConfigurator.Configure(new FileInfo(config_file));
        
    //        bool settingFlag = log4net.LogManager.GetRepository().Configured;//检测配置文件读取状态
    //        var repository = log4net.LogManager.GetRepository();
    //        var appenders = repository.GetAppenders();
    //    }
    //}
}

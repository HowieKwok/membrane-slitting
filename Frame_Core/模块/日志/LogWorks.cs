using LogTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static log4net.Appender.RollingFileAppender;

namespace Frame_Core
{
    public static class LogWorks
    {
        public static log4net.ILog GrayScaleAdaptationLog = log4net.LogManager.GetLogger("GrayScaleAdaptationLog");//记录日志

        public static void SetGrayScaleAdaptationLoggPath(string logBasePath)
        {
            GrayScaleAdaptationLog = LogObject.SetLog(logBasePath, "GrayScaleAdaptationLog-灰度自适应", 50, 100, RollingMode.Date);
        }
    }
}

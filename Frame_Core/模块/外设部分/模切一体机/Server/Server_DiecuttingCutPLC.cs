using LogTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static log4net.Appender.RollingFileAppender;

namespace Frame_Core
{
    public class Server_DiecuttingCutPLC : PLCCommunicationBase
    {
     
        public Server_DiecuttingCutPLC(PLCDevParam pLCDevParam)
        {
            m_pLCDevParam = pLCDevParam;
        }
        public  void SetLogPath(string logPath)
        {
            HeartbeatLog = LogObject.SetLog(logPath, "CutOverlayPlcHeartbeatLog", 5, 100, RollingMode.Composite);

            CommunicationLog = LogObject.SetLog(logPath, "CutOverlayPlcCommunicationLog", 5, 100, RollingMode.Composite);
        }

        public log4net.ILog HeartbeatLog = log4net.LogManager.GetLogger("CutOverlayPlcHeartbeatLog");//记录日志

        public log4net.ILog CommunicationLog = log4net.LogManager.GetLogger("CutOverlayPlcCommunicationLog");//记录日志

        #region 参数
        /// <summary>
        /// PLC地址
        /// </summary>
        public PLCAddress_DieCuttingCut m_DevAddress = new PLCAddress_DieCuttingCut();

        /// <summary>
        /// PLC参数
        /// </summary>
        public PLCDevParam  m_pLCDevParam = null;

        #endregion
    }
}

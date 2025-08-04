using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using LogTool;
using static log4net.Appender.RollingFileAppender;

namespace Frame_Core
{
    public class PFTPLCManage:MainPLCManage
    {
     
        public override void SetLogPath(string logPath)
        {
            HeartbeatLog = LogObject.SetLog(logPath, "PFTPlcHeartbeatLog", 5, 100, RollingMode.Composite);

            CommunicationLog = LogObject.SetLog(logPath, "PFTPlcCommunicationLog", 5, 100, RollingMode.Composite);
        }

        public override bool LoadParam(string path)
        {
            bool result = false;
            try
            {
                if (m_DevParam == null) m_DevParam = new MainPLCParam();
                m_DevParam.filePath = path;
                m_DevParam = (MainPLCParam)m_DevParam.Read();
                m_DevParam.filePath = path;
                result = true;
                m_DevAddress = new PFTFinsAddress();
                m_DevAddress.filePath = $"{System.Environment.CurrentDirectory}\\PFTPlcAddress";
                m_DevAddress.Save();
                Other.WriteTag($"{System.Environment.CurrentDirectory}\\PFTPlcAddress.csv", m_DevAddress);
               
            }
            catch (Exception ex)
            {
                result = false;
                MessageBox.Show(ex.ToString());
            }
            return result;
        }


        public override int ReadMachineRunningState()
        {
            int machineRunningState=0;
            if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.PFT)
            {
                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_MachineRunningState_CCDPFT, ReadLogSaveType.True, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                machineRunningState = (int)Project.Instance.cutOverlayPLCManage.m_DevAddress.In_MachineRunningState_BOVHResult.Value;
            }
            return machineRunningState;
        }
    }
    
}

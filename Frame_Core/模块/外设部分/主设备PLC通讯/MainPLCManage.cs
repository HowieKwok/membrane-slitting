using LogTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static log4net.Appender.RollingFileAppender;

namespace Frame_Core
{
    public class MainPLCManage : PLCCommunicationBase
    {
        public virtual void SetLogPath(string logPath)
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
        public PFTFinsAddress m_DevAddress = new PFTFinsAddress();

        /// <summary>
        /// PLC参数
        /// </summary>
        public MainPLCParam m_DevParam = null;

        /// <summary>
        /// 加载模块参数
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public virtual bool LoadParam(string path)
        {
            bool result = false;
            try
            {
                if (m_DevParam == null) m_DevParam = new MainPLCParam();
                m_DevParam.filePath = path;
                m_DevParam = (MainPLCParam)m_DevParam.Read();
                m_DevParam.filePath = path;
                result = true;

                m_DevAddress.filePath = $"{System.Environment.CurrentDirectory}\\CutOverlayPlcAddress";
                m_DevAddress.Save();
                Other.WriteTag($"{System.Environment.CurrentDirectory}\\CutOverlayPlcAddress.csv", m_DevAddress);
            }
            catch (Exception ex)
            {
                result = false;
                MessageBox.Show(ex.ToString());
            }
            return result;
        }

        #endregion

        #region OVH业务
        /// <summary>
        /// 读取当前层数A叠台
        /// </summary>
        /// <returns></returns>
        public int OVH_ReadCurrentLayerA()
        {
            int CurrentLayer = 0;
            if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面极片复检工位)
            {
                Read(m_DevAddress.In_CurrentLayer_AAOVH, ReadLogSaveType.True, CommunicationLog);
                CurrentLayer = (int)m_DevAddress.In_CurrentLayer_AAOVH.Value;
            }
            else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面极片复检工位)
            {
                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_CurrentLayer_BAOVH, ReadLogSaveType.True, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                CurrentLayer = (int)Project.Instance.cutOverlayPLCManage.m_DevAddress.In_CurrentLayer_BAOVH.Value;
            }
            return CurrentLayer;
        }

        /// <summary>
        /// 读取当前层数B叠台
        /// </summary>
        /// <returns></returns>
        public int OVH_ReadCurrentLayerB()
        {
            int CurrentLayer = 0;
            if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面极片复检工位)
            {
                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_CurrentLayer_ABOVH, ReadLogSaveType.True, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                CurrentLayer = (int)Project.Instance.cutOverlayPLCManage.m_DevAddress.In_CurrentLayer_ABOVH.Value;
            }
            else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面极片复检工位)
            {
                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_CurrentLayer_BBOVH, ReadLogSaveType.True, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                CurrentLayer = (int)Project.Instance.cutOverlayPLCManage.m_DevAddress.In_CurrentLayer_BBOVH.Value;
            }
            return CurrentLayer;
        }

        /// <summary>
        /// 读取上电芯或下电芯
        /// </summary>
        /// <returns>1：下电芯 2：上电芯</returns>
        public int OVH_ReadUpOrDownCell()
        {
            int UpOrDownCell = 0;
            if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面极片复检工位)
            {
                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_UpOrDownCell_AOVHResult, ReadLogSaveType.True, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                UpOrDownCell = (int)Project.Instance.cutOverlayPLCManage.m_DevAddress.In_UpOrDownCell_AOVHResult.Value;
            }
            else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面极片复检工位)
            {
                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_UpOrDownCell_BOVHResult, ReadLogSaveType.True, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                UpOrDownCell = (int)Project.Instance.cutOverlayPLCManage.m_DevAddress.In_UpOrDownCell_BOVHResult.Value;
            }
            return UpOrDownCell;
        }

        /// <summary>
        /// 读取开始信号
        /// </summary>
        /// <returns>1开始</returns>
        public int OVH_ReadStartSignal()
        {
            int startEndSignal = 0;
            if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面极片复检工位)
            {
                Project.Instance.cutOverlayPLCManage.ReadAndWriteZero(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_BeginSignal_AOVHResult, Project.Instance.cutOverlayPLCManage.CommunicationLog, out float readValue);
                startEndSignal = (int)readValue;
            }
            else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面极片复检工位)
            {
                Project.Instance.cutOverlayPLCManage.ReadAndWriteZero(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_BeginSignal_BOVHResult, Project.Instance.cutOverlayPLCManage.CommunicationLog, out float readValue);
                startEndSignal = (int)readValue;
            }
            return startEndSignal;
        }

        /// <summary>
        /// 读取结束信号
        /// </summary>
        /// <returns> 1为单层，2为双层（单层往上电芯写结果）</returns>
        public int OVH_ReadEndSignal()
        {
            int startEndSignal = 0;
            if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面极片复检工位)
            {
                Project.Instance.cutOverlayPLCManage.ReadAndWriteZero(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_EndSignal_AOVHResult, Project.Instance.cutOverlayPLCManage.CommunicationLog, out short readValue);
                startEndSignal = (int)readValue;
            }
            else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面极片复检工位)
            {
                Project.Instance.cutOverlayPLCManage.ReadAndWriteZero(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_EndSignal_BOVHResult, Project.Instance.cutOverlayPLCManage.CommunicationLog, out short readValue);
                startEndSignal = (int)readValue;
            }
            return startEndSignal;
        }

        /// <summary>
        /// 写入开始信号反馈
        /// </summary>
        /// <returns>1开始</returns>
        public void OVH_WriteStartSignalFeedback(int startEndSignal)
        {
            if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面极片复检工位)
            {
                Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_BeginSignalFeedback_AOVHResult.Value = startEndSignal;
                Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_BeginSignalFeedback_AOVHResult, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
            }
            else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面极片复检工位)
            {
                Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_BeginSignalFeedback_BOVHResult.Value = startEndSignal;
                Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_BeginSignalFeedback_BOVHResult, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
            }

        }

        /// <summary>
        /// 读取上和下Tabneck值-A平台
        /// </summary>
        public void OVH_ReadUpAndDownTabneckValue_APlatform(out double upTabneckValue, out double downTabneckValue)
        {
            upTabneckValue = 0;
            downTabneckValue = 0;
            if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面极片复检工位)
            {
                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_UpTabneckValue_AAOVH, ReadLogSaveType.True, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                upTabneckValue = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_UpTabneckValue_AAOVH.Value;

                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_DownTabneckValue_AAOVH, ReadLogSaveType.True, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                downTabneckValue = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_DownTabneckValue_AAOVH.Value;
            }
            else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面极片复检工位)
            {
                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_UpTabneckValue_BAOVH, ReadLogSaveType.True, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                upTabneckValue = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_UpTabneckValue_BAOVH.Value;

                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_DownTabneckValue_BAOVH, ReadLogSaveType.True, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                downTabneckValue = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_DownTabneckValue_BAOVH.Value;
            }
        }


        public void OVHX_A_ReadYDWSizeValue(out double PoleWidth1, out double PoleWidth2, out double PoleHeight1, out double PoleHeight2,
                                            out double PoleEarW, out double PoleEarH, out double PoleEarD)
        {
            PoleWidth1 = -999;
            PoleWidth2 = -999;
            PoleHeight1 = -999;
            PoleHeight2 = -999;
            PoleEarW = -999;
            PoleEarH = -999;
            PoleEarD = -999;
            if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面极片复检工位)
            {
                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PoleWidth1_AAOVH, ReadLogSaveType.True, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                PoleWidth1 = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PoleWidth1_AAOVH.Value;

                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PoleWidth2_AAOVH, ReadLogSaveType.True, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                PoleWidth2 = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PoleWidth2_AAOVH.Value;

                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PoleHeight1_AAOVH, ReadLogSaveType.True, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                PoleHeight1 = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PoleHeight1_AAOVH.Value;

                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PoleHeight2_AAOVH, ReadLogSaveType.True, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                PoleHeight2 = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PoleHeight2_AAOVH.Value;

                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PoleEarW_AAOVH, ReadLogSaveType.True, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                PoleEarW = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PoleEarW_AAOVH.Value;

                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PoleEarH_AAOVH, ReadLogSaveType.True, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                PoleEarH = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PoleEarH_AAOVH.Value;

                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PoleEarD_AAOVH, ReadLogSaveType.True, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                PoleEarD = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PoleEarD_AAOVH.Value;

            }
            else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面极片复检工位)
            {
                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PoleWidth1_BAOVH, ReadLogSaveType.True, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                PoleWidth1 = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PoleWidth1_BAOVH.Value;

                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PoleWidth2_BAOVH, ReadLogSaveType.True, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                PoleWidth2 = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PoleWidth2_BAOVH.Value;

                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PoleHeight1_BAOVH, ReadLogSaveType.True, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                PoleHeight1 = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PoleHeight1_BAOVH.Value;

                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PoleHeight2_BAOVH, ReadLogSaveType.True, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                PoleHeight2 = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PoleHeight2_BAOVH.Value;

                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PoleEarW_BAOVH, ReadLogSaveType.True, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                PoleEarW = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PoleEarW_BAOVH.Value;

                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PoleEarH_BAOVH, ReadLogSaveType.True, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                PoleEarH = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PoleEarH_BAOVH.Value;

                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PoleEarD_BAOVH, ReadLogSaveType.True, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                PoleEarD = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PoleEarD_BAOVH.Value;
            }

        }



        public void OVHX_B_ReadYDWSizeValue(out double PoleWidth1, out double PoleWidth2, out double PoleHeight1, out double PoleHeight2,
                                           out double PoleEarW, out double PoleEarH, out double PoleEarD)
        {
            PoleWidth1 = -999;
            PoleWidth2 = -999;
            PoleHeight1 = -999;
            PoleHeight2 = -999;
            PoleEarW = -999;
            PoleEarH = -999;
            PoleEarD = -999;
            if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面极片复检工位)
            {
                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PoleWidth1_ABOVH, ReadLogSaveType.True, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                PoleWidth1 = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PoleWidth1_ABOVH.Value;

                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PoleWidth2_ABOVH, ReadLogSaveType.True, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                PoleWidth2 = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PoleWidth2_ABOVH.Value;

                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PoleHeight1_ABOVH, ReadLogSaveType.True, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                PoleHeight1 = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PoleHeight1_ABOVH.Value;

                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PoleHeight2_ABOVH, ReadLogSaveType.True, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                PoleHeight2 = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PoleHeight2_ABOVH.Value;

                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PoleEarW_ABOVH, ReadLogSaveType.True, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                PoleEarW = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PoleEarW_ABOVH.Value;

                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PoleEarH_ABOVH, ReadLogSaveType.True, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                PoleEarH = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PoleEarH_ABOVH.Value;

                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PoleEarD_ABOVH, ReadLogSaveType.True, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                PoleEarD = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PoleEarD_ABOVH.Value;

            }
            else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面极片复检工位)
            {
                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PoleWidth1_BBOVH, ReadLogSaveType.True, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                PoleWidth1 = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PoleWidth1_BBOVH.Value;

                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PoleWidth2_BBOVH, ReadLogSaveType.True, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                PoleWidth2 = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PoleWidth2_BBOVH.Value;

                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PoleHeight1_BBOVH, ReadLogSaveType.True, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                PoleHeight1 = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PoleHeight1_BBOVH.Value;

                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PoleHeight2_BBOVH, ReadLogSaveType.True, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                PoleHeight2 = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PoleHeight2_BBOVH.Value;

                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PoleEarW_BBOVH, ReadLogSaveType.True, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                PoleEarW = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PoleEarW_BBOVH.Value;

                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PoleEarH_BBOVH, ReadLogSaveType.True, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                PoleEarH = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PoleEarH_BBOVH.Value;

                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PoleEarD_BBOVH, ReadLogSaveType.True, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                PoleEarD = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PoleEarD_BBOVH.Value;
            }

        }




        /// <summary>
        /// 读取上和下Tabneck值-B平台
        /// </summary>
        public void OVH_ReadUpAndDownTabneckValue_BPlatform(out double upTabneckValue, out double downTabneckValue)
        {
            upTabneckValue = 0;
            downTabneckValue = 0;
            if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面极片复检工位)
            {
                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_UpTabneckValue_ABOVH, ReadLogSaveType.True, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                upTabneckValue = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_UpTabneckValue_ABOVH.Value;

                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_DownTabneckValue_ABOVH, ReadLogSaveType.True, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                downTabneckValue = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_DownTabneckValue_ABOVH.Value;
            }
            else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面极片复检工位)
            {
                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_UpTabneckValue_BBOVH, ReadLogSaveType.True, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                upTabneckValue = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_UpTabneckValue_BBOVH.Value;

                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_DownTabneckValue_BBOVH, ReadLogSaveType.True, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                downTabneckValue = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_DownTabneckValue_BBOVH.Value;
            }
        }

        /// <summary>
        /// 读取二维码
        /// </summary>
        /// <returns></returns>
        public string OVH_ReadQRCode()
        {
            string qrCode = "";
            if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面极片复检工位)
            {
                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_QRCode_AOVHResult, 30, ReadLogSaveType.True, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                qrCode = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_QRCode_AOVHResult.Value;
            }
            else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面极片复检工位)
            {
                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_QRCode_BOVHResult, 30, ReadLogSaveType.True, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                qrCode = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_QRCode_BOVHResult.Value;
            }
            return qrCode;
        }

        #endregion

        #region 预定位业务

        /// <summary>
        /// 读取是否进入标定模式
        /// </summary>
        /// <param name="isCalibrationModel">是否进入标定模式</param>
        /// <param name="isReadDifferentState">是否是读取不同状态</param>
        public void Pretargeting_ReadCalibrationModel(out bool isCalibrationModel, out bool isReadDifferentState)
        {
            isCalibrationModel = false;
            isReadDifferentState = false;
            //TODO: 读取平台位置
            if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面A极片纠偏工位)
            {
                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_StratCaliSignal_AACali, ReadLogSaveType.ReadDifferentState, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                if (Project.Instance.cutOverlayPLCManage.m_DevAddress.In_StratCaliSignal_AACali.Value == 10)
                {
                    isCalibrationModel = true;
                    isReadDifferentState = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_StratCaliSignal_AACali.ReadDifferentStateFlag;
                }
            }
            else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面B极片纠偏工位)
            {
                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_StratCaliSignal_ABCali, ReadLogSaveType.ReadDifferentState, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                if (Project.Instance.cutOverlayPLCManage.m_DevAddress.In_StratCaliSignal_ABCali.Value == 10)
                {
                    isCalibrationModel = true;
                    isReadDifferentState = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_StratCaliSignal_ABCali.ReadDifferentStateFlag;
                }
            }
            else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面A极片纠偏工位)
            {
                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_StratCaliSignal_BACali, ReadLogSaveType.ReadDifferentState, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                if (Project.Instance.cutOverlayPLCManage.m_DevAddress.In_StratCaliSignal_BACali.Value == 10)
                {
                    isCalibrationModel = true;
                    isReadDifferentState = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_StratCaliSignal_BACali.ReadDifferentStateFlag;
                }
            }
            else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面B极片纠偏工位)
            {
                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_StratCaliSignal_BBCali, ReadLogSaveType.ReadDifferentState, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                if (Project.Instance.cutOverlayPLCManage.m_DevAddress.In_StratCaliSignal_BBCali.Value == 10)
                {
                    isCalibrationModel = true;
                    isReadDifferentState = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_StratCaliSignal_BBCali.ReadDifferentStateFlag;
                }
            }
        }

        /// <summary>
        /// 读取平台位置
        /// </summary>
        /// <param name="PlatformX"></param>
        /// <param name="PlatformY"></param>
        /// <param name="PlatformR"></param>
        public void Pretargeting_ReadPlatformPos(ReadLogSaveType readLogSaveType, out double PlatformX, out double PlatformY, out double PlatformR)
        {
            PlatformX = 0;
            PlatformY = 0;
            PlatformR = 0;
            //TODO: 读取平台位置
            if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面A极片纠偏工位)
            {
                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PLCCurrentXAxisCoord_AACali, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PLCCurrentYAxisCoord_AACali, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PLCCurrentRAxisCoord_AACali, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                PlatformX = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PLCCurrentXAxisCoord_AACali.Value;
                PlatformY = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PLCCurrentYAxisCoord_AACali.Value;
                PlatformR = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PLCCurrentRAxisCoord_AACali.Value;

            }
            else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面B极片纠偏工位)
            {
                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PLCCurrentXAxisCoord_ABCali, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PLCCurrentYAxisCoord_ABCali, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PLCCurrentRAxisCoord_ABCali, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                PlatformX = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PLCCurrentXAxisCoord_ABCali.Value;
                PlatformY = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PLCCurrentYAxisCoord_ABCali.Value;
                PlatformR = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PLCCurrentRAxisCoord_ABCali.Value;
            }
            else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面A极片纠偏工位)
            {
                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PLCCurrentXAxisCoord_BACali, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PLCCurrentYAxisCoord_BACali, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PLCCurrentRAxisCoord_BACali, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                PlatformX = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PLCCurrentXAxisCoord_BACali.Value;
                PlatformY = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PLCCurrentYAxisCoord_BACali.Value;
                PlatformR = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PLCCurrentRAxisCoord_BACali.Value;
            }
            else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面B极片纠偏工位)
            {
                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PLCCurrentXAxisCoord_BBCali, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PLCCurrentYAxisCoord_BBCali, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PLCCurrentRAxisCoord_BBCali, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                PlatformX = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PLCCurrentXAxisCoord_BBCali.Value;
                PlatformY = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PLCCurrentYAxisCoord_BBCali.Value;
                PlatformR = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PLCCurrentRAxisCoord_BBCali.Value;
            }

        }


        public void Pretargeting_WriteSize(List<SizeInfo> sizeInfos, ReadLogSaveType readLogSaveType)
        {
            if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面A极片纠偏工位)
            {
                if (sizeInfos.Count > 0)
                {

                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_PoleWidth1_AAYDW.Value = (float)sizeInfos[0].DataResult;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_PoleWidth1_AAYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                }
                if (sizeInfos.Count > 1)
                {
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_PoleWidth2_AAYDW.Value = (float)sizeInfos[1].DataResult;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_PoleWidth2_AAYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }
                if (sizeInfos.Count > 2)
                {
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_PoleHeight1_AAYDW.Value = (float)sizeInfos[2].DataResult;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_PoleHeight1_AAYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }
                if (sizeInfos.Count > 3)
                {
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_PoleHeight2_AAYDW.Value = (float)sizeInfos[3].DataResult;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_PoleHeight2_AAYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }
                if (sizeInfos.Count > 4)
                {
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_PoleEarW_AAYDW.Value = (float)sizeInfos[4].DataResult;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_PoleEarW_AAYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }
                if (sizeInfos.Count > 5)
                {
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_PoleEarH_AAYDW.Value = (float)sizeInfos[5].DataResult;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_PoleEarH_AAYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }
                if (sizeInfos.Count > 6)
                {
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_PoleEarD_AAYDW.Value = (float)sizeInfos[6].DataResult;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_PoleEarD_AAYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }

            }
            else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面B极片纠偏工位)
            {
                if (sizeInfos.Count > 0)
                {

                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_PoleWidth1_ABYDW.Value = (float)sizeInfos[0].DataResult;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_PoleWidth1_ABYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                }
                if (sizeInfos.Count > 1)
                {
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_PoleWidth2_ABYDW.Value = (float)sizeInfos[1].DataResult;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_PoleWidth2_ABYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }
                if (sizeInfos.Count > 2)
                {
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_PoleHeight1_ABYDW.Value = (float)sizeInfos[2].DataResult;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_PoleHeight1_ABYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }
                if (sizeInfos.Count > 3)
                {
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_PoleHeight2_ABYDW.Value = (float)sizeInfos[3].DataResult;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_PoleHeight2_ABYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }
                if (sizeInfos.Count > 4)
                {
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_PoleEarW_ABYDW.Value = (float)sizeInfos[4].DataResult;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_PoleEarW_ABYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }
                if (sizeInfos.Count > 5)
                {
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_PoleEarH_ABYDW.Value = (float)sizeInfos[5].DataResult;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_PoleEarH_ABYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }
                if (sizeInfos.Count > 6)
                {
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_PoleEarD_ABYDW.Value = (float)sizeInfos[6].DataResult;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_PoleEarD_ABYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }

            }
            else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面A极片纠偏工位)
            {
                if (sizeInfos.Count > 0)
                {

                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_PoleWidth1_BAYDW.Value = (float)sizeInfos[0].DataResult;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_PoleWidth1_BAYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                }
                if (sizeInfos.Count > 1)
                {
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_PoleWidth2_BAYDW.Value = (float)sizeInfos[1].DataResult;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_PoleWidth2_BAYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }
                if (sizeInfos.Count > 2)
                {
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_PoleHeight1_BAYDW.Value = (float)sizeInfos[2].DataResult;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_PoleHeight1_BAYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }
                if (sizeInfos.Count > 3)
                {
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_PoleHeight2_BAYDW.Value = (float)sizeInfos[3].DataResult;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_PoleHeight2_BAYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }
                if (sizeInfos.Count > 4)
                {
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_PoleEarW_BAYDW.Value = (float)sizeInfos[4].DataResult;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_PoleEarW_BAYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }
                if (sizeInfos.Count > 5)
                {
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_PoleEarH_BAYDW.Value = (float)sizeInfos[5].DataResult;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_PoleEarH_BAYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }
                if (sizeInfos.Count > 6)
                {
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_PoleEarD_BAYDW.Value = (float)sizeInfos[6].DataResult;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_PoleEarD_BAYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }
            }
            else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面B极片纠偏工位)
            {
                if (sizeInfos.Count > 0)
                {

                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_PoleWidth1_BBYDW.Value = (float)sizeInfos[0].DataResult;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_PoleWidth1_BBYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                }
                if (sizeInfos.Count > 1)
                {
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_PoleWidth2_BBYDW.Value = (float)sizeInfos[1].DataResult;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_PoleWidth2_BBYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }
                if (sizeInfos.Count > 2)
                {
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_PoleHeight1_BBYDW.Value = (float)sizeInfos[2].DataResult;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_PoleHeight1_BBYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }
                if (sizeInfos.Count > 3)
                {
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_PoleHeight2_BBYDW.Value = (float)sizeInfos[3].DataResult;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_PoleHeight2_BBYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }
                if (sizeInfos.Count > 4)
                {
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_PoleEarW_BBYDW.Value = (float)sizeInfos[4].DataResult;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_PoleEarW_BBYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }
                if (sizeInfos.Count > 5)
                {
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_PoleEarH_BBYDW.Value = (float)sizeInfos[5].DataResult;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_PoleEarH_BBYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }
                if (sizeInfos.Count > 6)
                {
                    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_PoleEarD_BBYDW.Value = (float)sizeInfos[6].DataResult;
                    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_PoleEarD_BBYDW, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                }
            }
        }



        /// <summary>
        /// 读取拍照运行平台位置
        /// </summary>
        /// <param name="readLogSaveType"></param>
        /// <param name="PlatformX"></param>
        /// <param name="PlatformY"></param>
        /// <param name="PlatformR"></param>
        public void Pretargeting_ReadPhotoPlatformPos(ReadLogSaveType readLogSaveType, out double PlatformX, out double PlatformY, out double PlatformR)
        {
            PlatformX = 0;
            PlatformY = 0;
            PlatformR = 0;
            //TODO: 读取平台位置
            if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面A极片纠偏工位)
            {
                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_CorrAxisXPos_AAYDW, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_CorrAxisYPos_AAYDW, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_CorrAxisRPos_AAYDW, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                PlatformX = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_CorrAxisXPos_AAYDW.Value;
                PlatformY = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_CorrAxisYPos_AAYDW.Value;
                PlatformR = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_CorrAxisRPos_AAYDW.Value;

            }
            else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面B极片纠偏工位)
            {
                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_CorrAxisXPos_ABYDW, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_CorrAxisYPos_ABYDW, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_CorrAxisRPos_ABYDW, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                PlatformX = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_CorrAxisXPos_ABYDW.Value;
                PlatformY = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_CorrAxisYPos_ABYDW.Value;
                PlatformR = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_CorrAxisRPos_ABYDW.Value;
            }
            else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面A极片纠偏工位)
            {
                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_CorrAxisXPos_BAYDW, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_CorrAxisYPos_BAYDW, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_CorrAxisRPos_BAYDW, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                PlatformX = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_CorrAxisXPos_BAYDW.Value;
                PlatformY = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_CorrAxisYPos_BAYDW.Value;
                PlatformR = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_CorrAxisRPos_BAYDW.Value;
            }
            else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面B极片纠偏工位)
            {
                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_CorrAxisXPos_BBYDW, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_CorrAxisYPos_BBYDW, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_CorrAxisRPos_BBYDW, readLogSaveType, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                PlatformX = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_CorrAxisXPos_BBYDW.Value;
                PlatformY = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_CorrAxisYPos_BBYDW.Value;
                PlatformR = Project.Instance.cutOverlayPLCManage.m_DevAddress.In_CorrAxisRPos_BBYDW.Value;
            }

        }


        /// <summary>
        /// 读取设备运行状态  启动写1，停止写2
        /// </summary>
        /// <returns></returns>
        public virtual int ReadMachineRunningState()
        {
            int machineRunningState = 0;

            if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.上单面工位)
            {
                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_MachineRunningState_TopPlane, ReadLogSaveType.True, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                machineRunningState = (int)Project.Instance.cutOverlayPLCManage.m_DevAddress.In_MachineRunningState_TopPlane.Value;
            }
            else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.下单面工位)
            {
                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_MachineRunningState_BottomPlane, ReadLogSaveType.True, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                machineRunningState = (int)Project.Instance.cutOverlayPLCManage.m_DevAddress.In_MachineRunningState_BottomPlane.Value;
            }
            else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A模切工位)
            {
                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_MachineRunningState_ADie, ReadLogSaveType.True, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                machineRunningState = (int)Project.Instance.cutOverlayPLCManage.m_DevAddress.In_MachineRunningState_ADie.Value;
            }
            else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B模切工位)
            {
                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_MachineRunningState_BDie, ReadLogSaveType.True, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                machineRunningState = (int)Project.Instance.cutOverlayPLCManage.m_DevAddress.In_MachineRunningState_BDie.Value;
            }
            else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面A极片纠偏工位 || Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面B极片纠偏工位 ||
                Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面极片复检工位)
            {
                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_MachineRunningState_AOVHResult, ReadLogSaveType.True, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                machineRunningState = (int)Project.Instance.cutOverlayPLCManage.m_DevAddress.In_MachineRunningState_AOVHResult.Value;
            }
            else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面A极片纠偏工位 || Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面B极片纠偏工位 ||
                Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面极片复检工位)
            {
                Project.Instance.cutOverlayPLCManage.Read(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_MachineRunningState_BOVHResult, ReadLogSaveType.True, Project.Instance.cutOverlayPLCManage.CommunicationLog);
                machineRunningState = (int)Project.Instance.cutOverlayPLCManage.m_DevAddress.In_MachineRunningState_BOVHResult.Value;
            }

            return machineRunningState;
        }



        /// <summary>
        /// 读取拍照信号
        /// </summary>
        /// <param name="isPhotoSignal"></param>
        public void Pretargeting_ReadPhotoSignal(out bool isPhotoSignal)
        {
            isPhotoSignal = false;
            //TODO: 读取平台位置
            if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面A极片纠偏工位)
            {
                Project.Instance.cutOverlayPLCManage.ReadAndWriteZero(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PhotoSignal_AACali, Project.Instance.cutOverlayPLCManage.CommunicationLog, out short readValue);

                if (Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PhotoSignal_AACali.Value == 1)
                {
                    isPhotoSignal = true;
                }


            }
            else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面B极片纠偏工位)
            {
                Project.Instance.cutOverlayPLCManage.ReadAndWriteZero(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PhotoSignal_ABCali, Project.Instance.cutOverlayPLCManage.CommunicationLog, out short readValue);

                if (Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PhotoSignal_ABCali.Value == 1)
                {
                    isPhotoSignal = true;
                }
            }
            else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面A极片纠偏工位)
            {
                Project.Instance.cutOverlayPLCManage.ReadAndWriteZero(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PhotoSignal_BACali, Project.Instance.cutOverlayPLCManage.CommunicationLog, out short readValue);

                if (Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PhotoSignal_BACali.Value == 1)
                {
                    isPhotoSignal = true;
                }
            }
            else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面B极片纠偏工位)
            {
                Project.Instance.cutOverlayPLCManage.ReadAndWriteZero(Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PhotoSignal_BBCali, Project.Instance.cutOverlayPLCManage.CommunicationLog, out short readValue);

                if (Project.Instance.cutOverlayPLCManage.m_DevAddress.In_PhotoSignal_BBCali.Value == 1)
                {
                    isPhotoSignal = true;
                }
            }
        }

        /// <summary>
        /// 发送标定结果
        /// </summary>
        /// <param name="calibrationResult">1 是ok 2是NG 3是标定完成</param>
        public void Pretargeting_SendCalibrationResult(short calibrationResult)
        {
            if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面A极片纠偏工位)
            {
                Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult_AACali.Value = calibrationResult;
                Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult_AACali, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);


            }
            else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面B极片纠偏工位)
            {
                Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult_ABCali.Value = calibrationResult;
                Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult_ABCali, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);

            }
            else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面A极片纠偏工位)
            {
                Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult_BACali.Value = calibrationResult;
                Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult_BACali, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);

            }
            else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面B极片纠偏工位)
            {
                Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult_BBCali.Value = calibrationResult;
                Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult_BBCali, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);

            }
        }

        /// <summary>
        /// 发送偏移量
        /// </summary>
        /// <param name="offsetX"></param>
        /// <param name="offsetY"></param>
        /// <param name="offsetR"></param>
        public void Pretargeting_SendOffsetLocation(double offsetX, double offsetY, double offsetR)
        {
            if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面A极片纠偏工位)
            {
                Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnXAxisCoord_AACali.Value = (float)offsetX;
                Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnXAxisCoord_AACali, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnYAxisCoord_AACali.Value = (float)offsetY;
                Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnYAxisCoord_AACali, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnRAxisCoord_AACali.Value = (float)offsetR;
                Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnRAxisCoord_AACali, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
            }
            else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面B极片纠偏工位)
            {
                Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnXAxisCoord_ABCali.Value = (float)offsetX;
                Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnXAxisCoord_ABCali, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnYAxisCoord_ABCali.Value = (float)offsetY;
                Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnYAxisCoord_ABCali, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnRAxisCoord_ABCali.Value = (float)offsetR;
                Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnRAxisCoord_ABCali, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
            }
            else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面A极片纠偏工位)
            {
                Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnXAxisCoord_BACali.Value = (float)offsetX;
                Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnXAxisCoord_BACali, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnYAxisCoord_BACali.Value = (float)offsetY;
                Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnYAxisCoord_BACali, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnRAxisCoord_BACali.Value = (float)offsetR;
                Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnRAxisCoord_BACali, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
            }
            else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面B极片纠偏工位)
            {
                Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnXAxisCoord_BBCali.Value = (float)offsetX;
                Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnXAxisCoord_BBCali, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnYAxisCoord_BBCali.Value = (float)offsetY;
                Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnYAxisCoord_BBCali, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);

                Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnRAxisCoord_BBCali.Value = (float)offsetR;
                Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnRAxisCoord_BBCali, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);
            }
        }

        /// <summary>
        /// 发送W1
        /// </summary>
        /// <param name="value"></param>
        public void Pretargeting_SendW1(double value)
        {
            //if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面A极片纠偏工位)
            //{
            //    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult_AACali.Value = value;
            //    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult_AACali, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);


            //}
            //else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面B极片纠偏工位)
            //{
            //    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult_ABCali.Value = value;
            //    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult_ABCali, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);

            //}
            //else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面A极片纠偏工位)
            //{
            //    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult_BACali.Value = value;
            //    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult_BACali, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);

            //}
            //else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面B极片纠偏工位)
            //{
            //    Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult_BBCali.Value = value;
            //    Project.Instance.cutOverlayPLCManage.Write(Project.Instance.cutOverlayPLCManage.m_DevAddress.Out_ReturnResult_BBCali, true, Project.Instance.cutOverlayPLCManage.CommunicationLog);

            //}
        }
        #endregion
    }
}

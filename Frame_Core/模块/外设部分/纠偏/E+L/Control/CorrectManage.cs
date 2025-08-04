using LogTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Frame_Core
{
    public class CorrectManage
    {
        /// <summary>
        /// E+L消息日志路径
        /// </summary>
        public string ELMsgLog_path = $"E:\\检测数据\\NewProduct\\E+L纠偏器收发信息记录.ini";

        /// <summary>
        /// 初始化状态
        /// </summary>
        public bool initState;


        private int WriteFailNum = 0;//写入失败次数
        private int ReadFailNum = 10;//读取失败次数
        private int WriteFailCount = 0;//写入失败计数
        private int ReadFailCount = 0;//读取失败计数

        public CorrectParam correctParam = new CorrectParam();

        public CorrectTool correctTool = new CorrectTool();

        UDPClient DeviationELUDP = new UDPClient();

        EL el = new EL();

        public Thread thd_Send = null;
        public Thread thd_Read = null;
        public bool bFlag_Thd_PLC = false;



        /// <summary>
        /// 初始化参数
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool InitParam(string path)
        {
            try
            {
                if (correctParam == null) correctParam = new CorrectParam();
                correctParam.filePath = path;
                correctParam = (CorrectParam)correctParam.Read();
                correctParam.filePath = path;

            }
            catch (Exception ex)
            {

                Log4netRecord.ErrorLog.Debug(ex);
                return false;
            }

            return true;

        }


        public bool InitEL()
        {
            try
            {
                if (initState) Close();

                initState = false;
                WriteFailCount = 0;
                ReadFailCount = 0;
                switch (correctParam.JPConnect_Type)
                {
                    case 0:
                        initState = DeviationELUDP.IniUdpClient(correctParam.strELMachineIP, correctParam.strELMachinePort, correctParam.strLocalMachineIP, correctParam.strLocalMachinePort);
                        break;
                    case 1:

                        break;

                }
                if (initState)
                {
                    //InitSendReadThread();
                }

            }
            catch (Exception ex)
            {
                initState = false;
                Log4netRecord.ErrorLog.Debug(ex);
            }
            return initState;

        }

        public bool Close()
        {
            try
            {
                initState = false;
                WriteFailCount = 0;
                ReadFailCount = 0;
                bFlag_Thd_PLC = false;

                if (DeviationELUDP != null)
                {
                    DeviationELUDP.CloseClient();
                }

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
                return false;
            }
            return true;

        }


        #region 信息交互

        /// <summary>
        /// 读取纠偏器当前数值
        /// </summary>
        /// <returns></returns>
        private bool ReadData(out double source0ffsetValue, out double real0ffsetValue)
        {
            source0ffsetValue = 0;
            real0ffsetValue = 0;
            try
            {
                byte[] backByte = new byte[48];

                string DeviationMode = ""; //读取纠偏器当前模式
                if (DeviationELUDP.ReadData(ELMsgLog_path, out backByte))
                {
                    switch (el.ModeBack(backByte))
                    {
                        case BackNum.AutomaticBackNum:
                            DeviationMode = "自动模式";
                            break;
                        case BackNum.CenterPositionBackNum:
                            DeviationMode = "回中模式";
                            break;
                        case BackNum.ManualBackNum:
                            DeviationMode = "手动模式";
                            break;
                        case BackNum.MoveLeftBackNum:
                            DeviationMode = "手动左移";
                            break;
                        case BackNum.MoveRigthBackNum:
                            DeviationMode = "手动右移";
                            break;
                        default:
                            DeviationMode = "";
                            break;
                    }
                    source0ffsetValue = (short)el.ReadOffsetValue(backByte);
                    real0ffsetValue = source0ffsetValue * 0.1;

                    string str = $"纠偏数据读取成功,纠偏模式:{DeviationMode},纠偏器当前数值:{source0ffsetValue},实际数值:{real0ffsetValue}";
                    DeviationELUDP.ReadLog(ELMsgLog_path, str, "");
                    return true;
                }
                else
                {
                    ReadFailCount++;
                }


                ReadDataAlarmMonitoring();
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
            }
            return false;
        }


        public void SendOffsetData(double send_Correctdata, bool isManual = false)
        {

            try
            {
                int res = 0;
                double source0ffsetValue = 0;
                double real0ffsetValue = 0;
                if (ReadData(out source0ffsetValue, out real0ffsetValue))
                {
                    string str = "";
                    if (isManual) str = "手动";
                    res = DeviationELUDP.SendData(el.OffSet((int)(source0ffsetValue + (send_Correctdata * 10))));
                    str += $"纠偏数据发送成功,纠偏器当前数值:{source0ffsetValue},实际数值:{real0ffsetValue},软件发送数值:{(int)(source0ffsetValue + (send_Correctdata * 10))}";
                    DeviationELUDP.ReadLog(ELMsgLog_path, str, "");
                    Heart = DateTime.Now;
                }

                if (res == -2) WriteFailCount++;
                SendDataAlarmMonitoring();
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);

            }

        }

        DateTime Heart = DateTime.Now;
        public void SendHeartbeat()
        {
            try
            {

                DateTime date1 = DateTime.Now;
                if (date1.Subtract(Heart).TotalMilliseconds > 100)
                {
                    Heart = date1;
                    int res = DeviationELUDP.SendData(el.Empty(0));
                    if (res == -2) WriteFailCount++;
                    SendDataAlarmMonitoring();
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
            }
        }

        public void SendHomeMiddle()
        {
            try
            {
                byte[] send_data = el.Center();
                byte[] send_data2 = new byte[send_data.Length];
                for (int i = 0; i < send_data.Length; i++)
                {
                    send_data2[i] = send_data[i];
                }
                int res = DeviationELUDP.SendData(send_data2);
                if (res == -2) WriteFailCount++;
                string str = string.Format("纠偏回中");
                DeviationELUDP.ReadLog(ELMsgLog_path, str, "");

                SendDataAlarmMonitoring();
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
            }
        }
        #endregion

        /// <summary>
        /// 读取失败警告监控
        /// </summary>
        public void ReadDataAlarmMonitoring()
        {
            if (ReadFailCount > ReadFailNum)
            {
                Close();
                TJBJInfo.AlarmWriteState(AlarmName.纠偏器连接失败, new StopAlarmMessage() { StopAlarmState = StopAlarmStateEnum.停机, describe = "读取数据失败次数过多" });
            }
        }

        /// <summary>
        /// 发送失败警告监控
        /// </summary>
        public void SendDataAlarmMonitoring()
        {
            if (WriteFailCount > WriteFailNum)
            {
                Close();
                TJBJInfo.AlarmWriteState(AlarmName.纠偏器连接失败, new StopAlarmMessage() { StopAlarmState = StopAlarmStateEnum.停机, describe = "发送数据失败次数过多" });
            }
        }
    }
}

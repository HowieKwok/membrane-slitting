using LogTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    public class CorrectBTS_Control
    {

        /// <summary>
        /// 消息日志路径
        /// </summary>
        public string MsgLog_path = $"{AppDomain.CurrentDomain.BaseDirectory}检测数据\\NewProduct\\纠偏器收发信息记录.ini";

        /// <summary>
        /// 初始化状态
        /// </summary>
        public bool initState;


        private int WriteFailNum = 0;//写入失败次数
        private int ReadFailNum = 3;//读取失败次数
        private int WriteFailCount = 0;//写入失败计数
        private int ReadFailCount = 0;//读取失败计数

        public CorrectParam correctParam = new CorrectParam();

        public CorrectTool correctTool = new CorrectTool();

        UDPClient DeviationELUDP = new UDPClient();

        public BTS bTS = new BTS();

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


        public bool Init()
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



        /// <summary>
        /// 读取纠偏器当前数值
        /// </summary>
        /// <returns></returns>
        public bool ReadData(bool devRun, out double source0ffsetValue)
        {
            source0ffsetValue = 0;

            try
            {
                if (!initState) return false;
                byte[] backByte = new byte[96];
                backByte = bTS.GetSendHeartbeat();
                if (DeviationELUDP.ReadData(MsgLog_path, out backByte))
                {
                    int correctionMode = bTS.GetReadCorrectionMode(backByte);
                    bTS.GetReadOffsetValue(backByte, correctionMode, out source0ffsetValue, out double real0ffsetValue);

                    if (En_ReadData.value != real0ffsetValue)
                    {
                        En_ReadData.value = real0ffsetValue;
                        string str = $"纠偏数据读取成功,纠偏模式:{correctionMode},纠偏器当前数值:{source0ffsetValue},实际数值:{real0ffsetValue}";
                        DeviationELUDP.ReadLog(MsgLog_path, str, "");
                    }

                    DeviationELUDP.ReadLog(MsgLog_path, GetMessage(backByte), ""); 

                    if (devRun && bTS.GetReadWorkMode(backByte) != 2)
                    {
                        TJBJInfo.AlarmWriteState(AlarmName.纠偏器工作模式异常, new StopAlarmMessage() { StopAlarmState = StopAlarmStateEnum.停机, describe = "请检查纠偏器工作模式是否为“自动”" });                     
                        return false;
                    }
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

        object SendDataEL_Flag = new object();
        public void SendOffsetData(double readSource0ffsetValue, double send_Correctdata, bool isManual = false)
        {
            try
            {
                if (!initState) return;
                lock (SendDataEL_Flag)
                {
                    int res = 0;

                    string str = "";
                    if (isManual) str = "手动";
                    double readReal0ffsetValue = bTS.ReadOffsetValueConversion(readSource0ffsetValue);
                    double sendOffset = /*readReal0ffsetValue + */send_Correctdata;
                    byte[] sendData = bTS.GetSendValue(BTSSendCorrectModeStruct.Center1And2, sendOffset);
                    res = DeviationELUDP.SendData(sendData);
                    str += $"纠偏数据发送成功,纠偏器当前数值:{readSource0ffsetValue},实际数值:{readReal0ffsetValue},软件发送数值:{sendOffset}";
                    DeviationELUDP.ReadLog(MsgLog_path, str, "");
                    Heart = DateTime.Now;
                   
                    DeviationELUDP.ReadLog(MsgLog_path, GetMessage(sendData), "");

                    if (res == -2) WriteFailCount++;
                    SendDataAlarmMonitoring();
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
            }
        }
        public string GetMessage(byte[] bytes)
        {
            string str1 = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                str1 += bytes[i].ToString("x2");
            }
            return str1;
        }

        DateTime Heart = DateTime.Now;
        public void SendHeartbeat()
        {
            try
            {
                if (!initState) return;
                DateTime date1 = DateTime.Now;
                if (date1.Subtract(Heart).TotalMilliseconds > 100)
                {
                    Heart = date1;
                    int res = DeviationELUDP.SendData(bTS.GetSendHeartbeat());
                    if (res == -2) WriteFailCount++;
                    SendDataAlarmMonitoring();
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
            }
        }

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


        public DoubleData En_ReadData = new DoubleData();

        public DoubleData En_Heartbeat = new DoubleData();

        public DoubleData En_AtuoOffset = new DoubleData();

        public DoubleData En_ManualOffset = new DoubleData();
    }


    public enum CorrectLogicEnum
    {
        分切相机1膜宽减标准值,
        分切相机2膜宽减标准值,
        分切相机1极宽减标准值,
        分切相机2极宽减标准值,
        分切相机1膜宽减分切相机2膜宽,
        分切相机1极宽减分切相机2极宽,
    }
}

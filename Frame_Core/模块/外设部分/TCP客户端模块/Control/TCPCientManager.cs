using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using HalconDotNet;

using LogTool;

using static GTN.mc;

namespace Frame_Core
{
    public class TCPCientManager
    {
        /// <summary>
        /// 读取信息路径
        /// </summary>
        public string ReadLogPath = $"{AppDomain.CurrentDomain.BaseDirectory}检测数据\\NewProduct\\打标机软件日志记录-接收.ini";

        /// <summary>
        /// 发送信息路径
        /// </summary>
        public string SendLogPath = $"{AppDomain.CurrentDomain.BaseDirectory}检测数据\\NewProduct\\打标机软件日志记录-发送.ini";
        /// <summary>
        /// 发送心跳路径
        /// </summary>
        public string SendHeartPath = $"{AppDomain.CurrentDomain.BaseDirectory}检测数据\\NewProduct\\打标机软件日志记录-心跳.ini";

        public FinsTcp FinsTCP = new FinsTcp();              //调用FinsTcp协议的类
        public TCP TCPIP = new TCP();               //调用TCPClient里面现有的方法（TCP/IP)通讯     
        public EH_Modbus ModbusTcp = new EH_Modbus();


        /// <summary>
        /// 初始化状态
        /// </summary>
        public bool initState;

        /// <summary>
        /// 主设备运行状态
        /// </summary>
        public bool MDevRun_Flag = false;

        protected int WriteFailNum = 1;//写入失败次数
        protected int ReadFailNum = 1;//读取失败次数
        protected int WriteFailCount = 0;//写入失败计数
        protected int ReadFailCount = 0;//读取失败计数

        public Thread thd_Send = null;
        public Thread thd_Read = null;
        public bool bFlag_Thd_PLC = false;
        public TCPClientParam  tcpClientParam;

        public virtual bool InitPLC()
        {
            if (!tcpClientParam.IsConnect) return false;

            try
            {
                if (initState) Close();

                initState = false;
                ModbusTcp.Connected = false;
                WriteFailCount = 0;
                ReadFailCount = 0;
                switch (tcpClientParam.ComType)
                {
                    case 0:
                        initState = TCPIP.Connect_Plc(tcpClientParam.PLC_IP, tcpClientParam.DevPort);     //tcp连接   
                        break;
                    case 1:

                        break;

                }
                if (initState)
                {
                   
                    Thread.Sleep(3);
             
                }

            }
            catch (Exception ex)
            {
                initState = false;
                Log4netRecord.ErrorLog.Debug(ex);
            }
            return initState;
        }

        public virtual bool Close()
        {
            try
            {
                initState = false;
                bFlag_Thd_PLC = false;
                WriteFailCount = 0;
                ReadFailCount = 0;


                if (FinsTCP != null)
                {
                    FinsTCP.disconn();
                }
                if (TCPIP != null)
                {
                    TCPIP.Shutdown_PLC();
                }
                if (ModbusTcp != null)
                {
                    ModbusTcp.MODBUS_disconnect();
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
                return false;
            }
            return true;
        }

        public virtual bool InitParam(string path, Boolean isCopy = false)
        {
            try
            {
                if (!isCopy)
                {
                    if (tcpClientParam == null) tcpClientParam = new TCPClientParam();
                    tcpClientParam.filePath = path;
                    tcpClientParam.Init();
                    tcpClientParam = (TCPClientParam)tcpClientParam.Read();
                    tcpClientParam.filePath = path;
                }
                else
                {
                    if (tcpClientParam == null) tcpClientParam = new TCPClientParam();
                    tcpClientParam.filePath = path;
                    tcpClientParam.Save();
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

        #region 发送
       protected  object sendLock = new object();
        protected DateTime HeartTimer = DateTime.Now;


        public virtual void SendData()
        {

        }



        public virtual void ReadData()
        {

        }
        /// <summary>
        /// 发送心跳
        /// </summary>
        protected virtual void SendHeart()
        {
            if (!initState) return;
            try
            {
                lock (sendLock)
                {
                    DateTime HeartTemp = DateTime.Now;
                    if (HeartTemp.Subtract(HeartTimer).TotalMilliseconds > 1000)
                    {
                        HeartTimer = HeartTemp;
                      
                        bool F = TCPIP.TCP_Send("XSTX000000000000CLXH", SendHeartPath);
                        if (!F) WriteFailCount++;
                    }
                }
            }
            catch (Exception ex)
            {

                Log4netRecord.ErrorLog.Debug(ex);
            }



        }
  

        #endregion

        #region 接收
        /// <summary>
        /// 打标机接收信息解析
        /// </summary>
        protected virtual void DBJReadResponse(string RedPlcData_str)
        {
   


        }

        protected DateTime HeartFeedbark_CCD1 = DateTime.Now;
        /// <summary>
        /// 读取 心跳反馈
        /// </summary>
        /// <returns></returns>
        protected virtual void ReadPLC_Hart(bool isHeartFeedbark)
        {
            DateTime date1 = DateTime.Now;
            if (isHeartFeedbark) HeartFeedbark_CCD1 = date1;

            if (date1.Subtract(HeartFeedbark_CCD1).TotalMilliseconds > 3000)
            {
                //Close();
                //TJBJInfo.AlarmWriteState(AlarmName.打标机软件连接失败, true, "打标机PLC心跳反馈超时");
            }
        }
        #endregion
        #endregion


    }
}

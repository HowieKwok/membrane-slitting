using Frame_Core;

using HalconDotNet;

using LogTool;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace Frame_Core
{
    /// <summary>
    /// 打标机设备管理
    /// </summary>
    public class DBJCardManager: TCPCientManager
    {
       
        /// <summary>
        /// 发送打标信息路径
        /// </summary>
        public string SendDBPath = $"E:\\检测数据\\NewProduct\\打标机软件日志记录-打标.ini";

        /// <summary>
        /// 发送纠偏信息路径
        /// </summary>
        public string SendJPPath = $"E:\\检测数据\\NewProduct\\打标机软件日志记录-纠偏.ini";

        public int ZoneNumber { get; set; } = 8;




        public BindingList<CoatingZone> Zones()
        {
            DBJCardParam dBJCardParam = tcpClientParam as DBJCardParam;
            return dBJCardParam.Zones;
        }


        public void SetZoneCount(int newCount)
        {
           DBJCardParam dBJCardParam=tcpClientParam as DBJCardParam;
            int oldCount =dBJCardParam.Zones.Count;

            // 1. 需要新增
            for (int i = oldCount + 1; i <= newCount; i++)
                dBJCardParam.Zones.Add(new CoatingZone { ZoneNumber = i });

            // 2. 需要删除
            while (dBJCardParam.Zones.Count > newCount)
                dBJCardParam.Zones.RemoveAt(dBJCardParam.Zones.Count - 1);
        }



        public override bool InitParam(string path, Boolean isCopy = false)
        {
            try
            {
                DBJCardParam dBJCardParam = tcpClientParam as DBJCardParam;
                if (!isCopy)
                {
                    if (dBJCardParam == null) dBJCardParam = new DBJCardParam();
                    dBJCardParam.filePath = path;
                    dBJCardParam.Init();
                    dBJCardParam = (DBJCardParam)dBJCardParam.Read();
                    dBJCardParam.filePath = path;
                    if(dBJCardParam!=null)
                    {
                        dBJCardParam.ZonesInit();
                    }
                    tcpClientParam = dBJCardParam;
                
                }
                else
                {
                    if (dBJCardParam == null) dBJCardParam = new DBJCardParam();
                    dBJCardParam.filePath = path;
                    dBJCardParam.Init();
                    dBJCardParam.ZonesInit();
                    dBJCardParam.Save();
                    tcpClientParam =dBJCardParam;
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
        /// 打标机软件发送数量
        /// </summary>
        public int DbSendNum = 0;
        /// <summary>
        /// 打标机软件接收数量
        /// </summary>
        public int DbReadNum = 0;
        /// <summary>
        /// 打标机软件出标完成数量
        /// </summary>
        public int DbFinishNum = 0;
        /// <summary>
        /// 出标反馈产品ID
        /// </summary>
        public QueuePackage<long> MarkFeedbackProdID = new QueuePackage<long>();
        public override  bool InitPLC()
        {
            DBJCardParam dBJCardParam = tcpClientParam as DBJCardParam;
            if (!dBJCardParam.IsConnect) return false;

            bool T = false;
            try
            {
                if (initState) Close();

                initState = false;
                ModbusTcp.Connected = false;
                WriteFailCount = 0;
                ReadFailCount = 0;
                switch (dBJCardParam.ComType)
                {
                    case 0:
                        initState = TCPIP.Connect_Plc(dBJCardParam.PLC_IP, dBJCardParam.DevPort);     //tcp连接   
                        break;
                    case 1:

                        break;

                }
                if (initState)
                {
                    SendAutoDB(dBJCardParam.AutoDB);
                    Thread.Sleep(3);
                    SendCCDREBOOTSignal();

                    InitSendReadThread();
                }

            }
            catch (Exception ex)
            {
                initState = false;
                Log4netRecord.ErrorLog.Debug(ex);
            }
            return initState;
        }

        public override bool Close()
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


        #region 发送读取线程

        private void InitSendReadThread()
        {
            ////thd_Send = null;
            //thd_Read = null;
            //bFlag_Thd_PLC = false;
            //Thread.Sleep(250);
            ////thd_Send = new Thread(SendData);
            //thd_Read = new Thread(ReadData);
            ////thd_Send.Start();
            //thd_Read.Start();
        }

        public override void SendData()
        {
            if (!initState) return;
            try
            {
                //发送心跳
                SendHeart();

                string message = "";
                if (WriteFailCount > WriteFailNum)
                {
                    Close();
                    message = "写入数据失败次数过多";
                    TJBJInfo.AlarmWriteState(AlarmName.打标机软件连接失败, StopAlarmStateEnum.停机, "");

                }

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
                Close();
                TJBJInfo.AlarmWriteState(AlarmName.打标机软件连接失败, StopAlarmStateEnum.停机, "");

            }
        }

        public override void ReadData()
        {
            if (!initState) return;
            bool RedPlcData_Bool;
            string RedPlcData_str;
            try
            {
                string message = "";

                bool F = TCPIP.RedPlcData(ReadLogPath, out RedPlcData_Bool, out RedPlcData_str);

                //通讯状态
                if (!F)
                {
                    ReadFailCount++;
                }

                if (ReadFailCount > ReadFailNum)
                {
                    Close();
                    message = "读取数据失败次数过多";
                    TJBJInfo.AlarmWriteState(AlarmName.打标机软件连接失败, StopAlarmStateEnum.停机, "");

                }


                //解析读取到的字符串
                DBJReadResponse(RedPlcData_str);

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
                Close();
                TJBJInfo.AlarmWriteState(AlarmName.打标机软件连接失败, StopAlarmStateEnum.停机, "");

            }
        }


        private void test()
        {

        }

        #endregion



        #region 信息交互

        #region 发送
     
        /// <summary>
        /// 发送心跳
        /// </summary>
        protected override void SendHeart()
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
                        //Task.Run(() =>
                        //{
                        //    bool F = TCPIP.TCP_Send("XSTX000000000000CLXH", SendHeartPath);
                        //    if (!F) WriteFailCount++;
                        //});
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
        /// <summary>
        /// 发送CCD运行暂停信号
        /// </summary>
        public void SendRun(bool isRUN)
        {
            if (!initState) return;
            try
            {
                lock (sendLock)
                {
                    if (isRUN)
                    {
                        bool F = TCPIP.TCP_Send("PZ00000000000000000Y", SendLogPath);
                        if (!F) WriteFailCount++;
                    }
                    else
                    {
                        bool F = TCPIP.TCP_Send("PZ00000000000000000N", SendLogPath);
                        if (!F) WriteFailCount++;
                    }
                    SendBMLXY(isRUN);
                }
            }
            catch (Exception ex)
            {

                Log4netRecord.ErrorLog.Debug(ex);
            }


        }

        /// <summary>
        /// 发送编码轮压下或抬起
        /// </summary>
        private void SendBMLXY(bool isRUN)
        {
            if (!initState) return;
            try
            {
                lock (sendLock)
                {
                    if (isRUN)
                    {
                        bool F = TCPIP.TCP_Send("BMQYL000000000000DK", SendLogPath);
                        if (!F) WriteFailCount++;
                    }
                    else
                    {
                        bool F = TCPIP.TCP_Send("BMQYL000000000000GB", SendLogPath);
                        if (!F) WriteFailCount++;
                    }
                }
            }
            catch (Exception ex)
            {

                Log4netRecord.ErrorLog.Debug(ex);
            }


        }

        /// <summary>
        /// 发送主设备启动停止信号
        /// </summary>
        public void SendDevRun(bool devIsRUN)
        {
            if (!initState) return;
            try
            {
                lock (sendLock)
                {
                    if (devIsRUN)
                    {
                        bool F = TCPIP.TCP_Send("HMX000000000000000QD", SendLogPath);
                        if (!F) WriteFailCount++;
                    }
                    else
                    {
                        bool F = TCPIP.TCP_Send("HMX000000000000000TZ", SendLogPath);
                        if (!F) WriteFailCount++;
                    }
                }
            }
            catch (Exception ex)
            {

                Log4netRecord.ErrorLog.Debug(ex);
            }


        }

        /// <summary>
        /// 发送新建膜卷
        /// </summary>
        public void SendXJMQ(string mojuanhao)
        {
            if (!initState) return;
            try
            {
                lock (sendLock)
                {
                    bool F = TCPIP.TCP_Send("MJX0000000000000000J", SendLogPath);
                    if (!F) WriteFailCount++;
                }

                SendMojuanhao(mojuanhao);
            }
            catch (Exception ex)
            {

                Log4netRecord.ErrorLog.Debug(ex);
            }



        }

        /// <summary>
        /// 发送膜卷号
        /// </summary>
        private void SendMojuanhao(string mojuanhao)
        {
            if (!initState) return;
            try
            {
                lock (sendLock)
                {
                    bool F = TCPIP.TCP_Send($"NEWM{mojuanhao}", SendLogPath);
                    if (!F) WriteFailCount++;
                }
            }
            catch (Exception ex)
            {

                Log4netRecord.ErrorLog.Debug(ex);
            }



        }

        /// <summary>
        /// 发送自动打标使能
        /// </summary>
        public void SendAutoDB(bool isAutoDB)
        {
            if (!initState) return;
            try
            {
                lock (sendLock)
                {
                    if (isAutoDB)
                    {
                        bool F = TCPIP.TCP_Send("ZDDB00000000000000QD", SendLogPath);
                        if (!F) WriteFailCount++;
                    }
                    else
                    {
                        bool F = TCPIP.TCP_Send("ZDDB00000000000000GB", SendLogPath);
                        if (!F) WriteFailCount++;
                    }
                }
            }
            catch (Exception ex)
            {

                Log4netRecord.ErrorLog.Debug(ex);
            }
        }

        /// <summary>
        /// 发送报警复位
        /// </summary>
        public void SendAlarmReset()
        {
            if (!initState) return;
            try
            {
                lock (sendLock)
                {
                    bool F = TCPIP.TCP_Send("J0000000000000000FW", SendLogPath);
                    if (!F) WriteFailCount++;
                }
            }
            catch (Exception ex)
            {

                Log4netRecord.ErrorLog.Debug(ex);
            }



        }

        /// <summary>
        /// 发送停机信号
        /// </summary>
        public void SendTJ()
        {
            if (!initState) return;
            try
            {
                lock (sendLock)
                {
                    bool F = TCPIP.TCP_Send("HMX000000000000000TJ", SendLogPath);
                    if (!F) WriteFailCount++;
                }
            }
            catch (Exception ex)
            {

                Log4netRecord.ErrorLog.Debug(ex);
            }



        }


        /// <summary>
        /// 发送打标信息
        /// </summary>
        /// <param name="frame">帧号</param>
        /// <param name="imageBotDist">缺陷坐标到图像底部距离</param>
        public void SendDBInfo(long frame, string[] imageBotDists,int mode=0)
        {
            if (!initState) return;
            try
            {
                lock (sendLock)
                {
                    DbSendNum = DbSendNum + imageBotDists.Length;
                    string imageBotDist = String.Join(",", imageBotDists);
                    bool F = false;
                    HOperatorSet.CountSeconds(out HTuple secStart);
                    switch (mode)
                    {
                        case 0:
                            F = TCPIP.TCP_Send($"ZMXS,{frame},{imageBotDist}", SendDBPath);
                            break;
                        case 1:
                            F = TCPIP.TCP_Send($"FMXS,{frame},{imageBotDist}", SendDBPath);
                            break;
                        case 2:
                            F = TCPIP.TCP_Send($"ZMMD,{frame},{imageBotDist}", SendDBPath);
                            break;
                        case 3:
                            F = TCPIP.TCP_Send($"FMMD,{frame},{imageBotDist}", SendDBPath);
                            break;
                    }
                    HOperatorSet.CountSeconds(out HTuple secEnd);
                    //毫秒
                    double ms = (secEnd - secStart) * 1000;
                    MessageNotify.AddCommLog($"ZMXS,{frame},{imageBotDist}__发送耗时{ms}ms");
                    if (!F) WriteFailCount++;
                }
            }
            catch (Exception ex)
            {

                Log4netRecord.ErrorLog.Debug(ex);
            }



        }


        public void SendDBInfo(long frame, (int, string)[] DBChannalArray, int CameraNum,out string strSend)
        {

            string str = "DB12";
            if (!initState)
            {
                 strSend = "";
                 return;
            }
               
            try
            {
                lock (sendLock)
                {

                  
                    for (int i = 0; i < DBChannalArray.Count(); i++)
                    {
                        str += DBChannalArray[i].Item2;
                    }
                    str += $"{FormatChannelCode((int)frame)}{FormatChannelCode((int)CameraNum)}";
                    HOperatorSet.CountSeconds(out HTuple start1);
                    HOperatorSet.CountSeconds(out HTuple start);
                    bool F = TCPIP.TCP_Send(str, SendDBPath);
                    HOperatorSet.CountSeconds(out HTuple end);
                    double hTime4 = ((end - start) * 1000).D;//数据处理总耗时
                    MessageNotify.AddDebugLog("发送打标耗时：" + hTime4.ToString());
                    MessageNotify.AddCommLog("输出打标字符串：" + str);
                    if (!F) WriteFailCount++;

                }


            }
            catch (Exception)
            {

                throw;
            }

            strSend = str;


        }

        private string FormatChannelCode(int channel)
          => channel.ToString().PadLeft(4, '0'); // 生成"0001"格式

        /// <summary>
        /// 发送面阵相机检测结果
        /// </summary>
        /// <param name="isLabel">是否有标签</param>     
        public void SendDBMZResult(bool isLabel)
        {
            if (!initState) return;
            try
            {
                lock (sendLock)
                {
                    int isOK = isLabel ? 1 : 0;
                    bool F = TCPIP.TCP_Send($"DBMZ,{isOK}", SendDBPath);
                    if (!F) WriteFailCount++;
                }
            }
            catch (Exception ex)
            {

                Log4netRecord.ErrorLog.Debug(ex);
            }



        }

        /// <summary>
        /// 发送图片长度
        /// </summary>
        /// <param name="FrameLenght">帧长度</param>     
        public void SendFrameLenght(double FrameLenght)
        {
            if (!initState) return;
            try
            {
                lock (sendLock)
                {
                    string frameLenght = FrameLenght.ToString("0.000").PadRight(8, '0');
                    bool F = TCPIP.TCP_Send($"MARF{frameLenght}000000DZ", SendDBPath);
                    if (!F) WriteFailCount++;
                }
            }
            catch (Exception ex)
            {

                Log4netRecord.ErrorLog.Debug(ex);
            }



        }


        /// <summary>
        /// 发送纠偏信息
        /// </summary>      
        public void SendJPInfo(string JPValue)
        {
            if (!initState) return;
            try
            {
                lock (sendLock)
                {
                    bool F = TCPIP.TCP_Send($"JP,{JPValue}", SendJPPath);
                    if (!F) WriteFailCount++;
                }
            }
            catch (Exception ex)
            {

                Log4netRecord.ErrorLog.Debug(ex);
            }



        }

        /// <summary>
        /// 发送图片长度
        /// </summary>      
        public void SendImageLength(double imageLength)
        {
            if (!initState) return;
            try
            {
                lock (sendLock)
                {
                    //MARFXXXXXXXX000000DZ
                    // 如果需要确保字符串长度为8，可能需要截断或填充
                    string str = imageLength.ToString(CultureInfo.InvariantCulture);
                    if (str.Length > 8)
                    {
                        // 截断字符串，这可能会丢失数据
                        str = str.Substring(0, 8);
                    }
                    else if (str.Length < 8)
                    {
                        // 用0填充字符串直到长度为8
                        str = str.PadLeft(8, '0');
                    }

                    bool F = TCPIP.TCP_Send($"MARF{str}000000DZ", SendJPPath);
                    if (!F) WriteFailCount++;
                }
            }
            catch (Exception ex)
            {

                Log4netRecord.ErrorLog.Debug(ex);
            }
        }
        /// <summary>
        /// 发送CCD重启信号
        /// </summary>
        public void SendCCDREBOOTSignal()
        {
            if (!initState) return;
            try
            {
                lock (sendLock)
                {
                    bool F = TCPIP.TCP_Send($"REBOOT", SendJPPath);
                    if (!F) WriteFailCount++;
                }
                Thread.Sleep(100);
                SendRun(false);
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
        protected override void DBJReadResponse(string RedPlcData_str)
        {
            try
            {
                bool isHeartFeedbark = false;
                string[] strArr = RedPlcData_str.Split(',');
                if (strArr == null || string.IsNullOrEmpty(RedPlcData_str)) return;

                string str = "";
                if (strArr.Length > 0)
                {
                    if (strArr[0].Contains("RJTXDSTJ"))
                    {
                        TJBJInfo.AlarmWriteState(AlarmName.采图不正常停机报警, StopAlarmStateEnum.停机, "");
                      
                    }
                    if (strArr[0].Contains("SBQDXHAM"))
                    {
                     
                        TJBJInfo.AlarmWriteState(AlarmName.打标机未收到设备启动信号报警, StopAlarmStateEnum.停机, "");
                    }
                    if (strArr[0].Contains("BMCYCAM"))
                    {

                        TJBJInfo.AlarmWriteState(AlarmName.PLC编码器无脉冲信号报警, StopAlarmStateEnum.停机, "");

                    }
                    if (strArr[0].Contains("JBCWYAM"))
                    {
                        TJBJInfo.AlarmWriteState(AlarmName.编码器压轮未压到位磁性开关报警, StopAlarmStateEnum.停机, "");

                    }
                    if (strArr[0].Contains("NEANG"))
                    {
                        DbReadNum++;
                        if (strArr.Length > 1)
                        {
                            str = strArr[1];
                        }
                        TCPIP.SendPLC_iniLog(SendDBPath, $"打标机反馈-已接收到NG信号{str}[{DbReadNum}]", "");
                    }
                    if (strArr[0].Contains("NEADB"))
                    {
                        DbFinishNum++;
                        if (strArr.Length > 1)
                        {
                            str = strArr[1];
                            try
                            {
                                MarkFeedbackProdID.Enqueue(Convert.ToInt64(str), 1000);

                            }
                            catch (Exception)
                            {
                            }
                        }
                        TCPIP.SendPLC_iniLog(SendDBPath, $"打标机反馈-已完成出标{str}[{DbFinishNum}]", "");
                    }
                    if (strArr[0].Contains("NCSDCBFK"))
                    {
                        //正面打标机手动打标
                        //TODO
                    }
                    if (strArr[0].Contains("XSTX000000000000CLXH"))
                    {
                        isHeartFeedbark = true;
                        TCPIP.RedPLCLog(SendHeartPath, "心跳信号反馈", "");
                    }
                    if (strArr[0].Contains("NDBBQZAM"))
                    {
                        TJBJInfo.AlarmWriteState(AlarmName.打标机标签纸故障报警, StopAlarmStateEnum.停机, "");

                       // TJBJInfo.AlarmWriteState(AlarmName.打标机标签纸故障报警, true, "");
                    }
                    if (strArr[0].Contains("NDBCBAM"))
                    {
                        TJBJInfo.AlarmWriteState(AlarmName.打标机出标故障报警, StopAlarmStateEnum.停机, "");

                        //TJBJInfo.AlarmWriteState(AlarmName.打标机出标故障报警, true, "");
                    }
                    if (strArr[0].Contains("NDBSFAM"))
                    {
                        TJBJInfo.AlarmWriteState(AlarmName.打标机伺服故障报警, StopAlarmStateEnum.停机, "");

                        //TJBJInfo.AlarmWriteState(AlarmName.打标机伺服故障报警, true, "");
                    }
                    if (strArr[0].Contains("NDBWDWAM"))
                    {
                        TJBJInfo.AlarmWriteState(AlarmName.打标机未到位报警, StopAlarmStateEnum.停机, "");

                       // TJBJInfo.AlarmWriteState(AlarmName.打标机未到位报警, true, "");
                    }
                    if (strArr[0].Contains("NDBMLAM"))
                    {
                        TJBJInfo.AlarmWriteState(AlarmName.打标机标签纸末料故障报警, StopAlarmStateEnum.停机, "");

                      //  TJBJInfo.AlarmWriteState(AlarmName.打标机标签纸末料故障报警, true, "");
                    }
                    if (strArr[0].Contains("NDBJBQDD"))
                    {
                        TJBJInfo.AlarmWriteState(AlarmName.打标机标签纸断带故障报警, StopAlarmStateEnum.停机, "");

                       // TJBJInfo.AlarmWriteState(AlarmName.打标机标签纸断带故障报警, true, "");
                    }
                    if (strArr[0].Contains("NXGYAM"))
                    {
                        TJBJInfo.AlarmWriteState(AlarmName.持续感应出标异常报警, StopAlarmStateEnum.停机, "");

                       // TJBJInfo.AlarmWriteState(AlarmName.持续感应出标异常报警, true, "");
                    }
                    if (strArr[0].Contains("LABELERR"))
                    {
                        TJBJInfo.AlarmWriteState(AlarmName.激光出标异常, StopAlarmStateEnum.停机, "");

                       // TJBJInfo.AlarmWriteState(AlarmName.激光出标异常, true, "");
                    }
                    if (strArr[0].Contains("NGADDAM"))
                    {
                        TJBJInfo.AlarmWriteState(AlarmName.NG信号不在接收范围, StopAlarmStateEnum.停机, "");

                       // TJBJInfo.AlarmWriteState(AlarmName.NG信号不在接收范围, true, "");
                    }
                    if (strArr[0].Contains("NCHBGDAM"))
                    {
                        TJBJInfo.AlarmWriteState(AlarmName.内侧打标过多, StopAlarmStateEnum.停机, "");

                        //TJBJInfo.AlarmWriteState(AlarmName.内侧打标过多, true, "");
                    }
                    if (strArr[0].Contains("STOP"))
                    {
                        if (strArr.Length > 1)
                        {
                            string dist = strArr[1];
                            TJBJInfo.AlarmWriteState(AlarmName.NG异常停机距离, StopAlarmStateEnum.停机, "");

                           // TJBJInfo.AlarmWriteState(AlarmName.NG异常停机距离, true, $"补标位置距离背光相机[{dist}(mm)]");
                        }
                    }

         
                }

                ReadPLC_Hart(isHeartFeedbark);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);

            }


        }

       
        /// <summary>
        /// 读取 心跳反馈
        /// </summary>
        /// <returns></returns>
        protected override void ReadPLC_Hart(bool isHeartFeedbark)
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

    [Serializable]
    public class CoatingZone
    {
        public int ZoneNumber { get; set; }
        public BindingList<MarkerChannel> Channels { get; } = new BindingList<MarkerChannel>();
    }

    [Serializable]
    public class MarkerChannel
    {
        public int Id { get; set; } = 1;
        public string Name { get; set; } = "CH1";
        public override string ToString() => Name;
    }

}

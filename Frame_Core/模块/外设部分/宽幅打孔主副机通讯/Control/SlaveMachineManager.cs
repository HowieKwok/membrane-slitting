using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FrameCommunity;
using HalconDotNet;

using LogTool;

namespace Frame_Core
{
    public class SlaveMachineManager : TCPCientManager
    {
        /// <summary>
        /// 发送打标信息路径
        /// </summary>
        public string SendDBPath = $"{AppDomain.CurrentDomain.BaseDirectory}检测数据\\NewProduct\\打标机软件日志记录-打标.ini";


        public BindingList<CoatingZone> Zones()
        {
            SlaveMachineParam dBJCardParam = tcpClientParam as SlaveMachineParam;
            return dBJCardParam.Zones;
        }

        public void SetZoneCount(int newCount)
        {
            SlaveMachineParam dBJCardParam = tcpClientParam as SlaveMachineParam;
            int oldCount = dBJCardParam.Zones.Count;

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
                SlaveMachineParam slaveMachineParam = tcpClientParam as SlaveMachineParam;
                if (!isCopy)
                {
                    if (slaveMachineParam == null) slaveMachineParam = new SlaveMachineParam();
                    slaveMachineParam.filePath = path;
                    slaveMachineParam.Init();
                    slaveMachineParam = (SlaveMachineParam)slaveMachineParam.Read();
                    slaveMachineParam.filePath = path;
                    if (slaveMachineParam != null)
                    {
                        slaveMachineParam.ZonesInit();
                    }
                    tcpClientParam = slaveMachineParam;

                }
                else
                {
                    if (slaveMachineParam == null) slaveMachineParam = new SlaveMachineParam();
                    slaveMachineParam.filePath = path;
                    slaveMachineParam.Init();
                    slaveMachineParam.ZonesInit();
                    slaveMachineParam.Save();
                    tcpClientParam = slaveMachineParam;
                }
            }
            catch (Exception ex)
            {

                Log4netRecord.ErrorLog.Debug(ex);
                return false;
            }

            return true;

        }


        public QueuePackage<long> MarkFeedbackProdID = new QueuePackage<long>();
        public override bool InitPLC()
        {
            SlaveMachineParam dBJCardParam = tcpClientParam as SlaveMachineParam;
            if (!dBJCardParam.IsConnect) return false;

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

                }

            }
            catch (Exception ex)
            {
                initState = false;
                Log4netRecord.ErrorLog.Debug(ex);
            }
            return initState;
        }




        public delegate bool MJStartDelegate(string message);
        public delegate bool BeginDelegate();
        public event MJStartDelegate StartMJEvent;
        public event BeginDelegate StopMJEvent;
        public event BeginDelegate StartRunEvent;
        public event BeginDelegate StopRunEvent;


        #region 接收

        public string GetProduceCode()
        {
            return (tcpClientParam as SlaveMachineParam).ProduceCode;
        }


        public string GetRollCode()
        {
            return (tcpClientParam as SlaveMachineParam).RollCode;
        }


        public int GetMacheineSpeed()
        {
            return (int)(tcpClientParam as SlaveMachineParam).fMacheineSpeed;
        }



        protected override void DBJReadResponse(string RedPlcData_str)
        {
            try
            {
                bool isHeartFeedbark = false;
                string[] strArr = RedPlcData_str.Split(',');
                SlaveMachineParam slaveMachineParam = tcpClientParam as SlaveMachineParam;

                if (RedPlcData_str.Length > 2)
                {

                }
                if (strArr == null) return;


                if (strArr.Length > 0)
                {
                    UserManage userManage = Project.Instance.userManage;
                    switch (strArr[0])
                    {

                        #region >>>RUN_MJ 开始膜卷
                        case "RUN_MJ":
                            if (true)
                            {
                                slaveMachineParam.ProduceCode = strArr[1];
                                slaveMachineParam.RollCode = strArr[2];

                                //==============================================================================================================================
                                //=======【】停止运行
                                if (Project.Instance.Is_Run)
                                {
                                    StopRunEvent?.Invoke();

                                }
                                //==============================================================================================================================
                                //=======【】结束膜卷
                                if (Project.Instance.Is_MojuanStart)
                                {
                                    StopMJEvent?.Invoke();

                                }
                                //==============================================================================================================================
                                //=======【】开始运行
                                bool? result = StartMJEvent?.Invoke(slaveMachineParam.RollCode);
                                if (result == true)
                                {

                                    bool F = TCPIP.TCP_Send("FJ_Start_MJ", SendLogPath);
                                    if (!F) WriteFailCount++;
                                    MessageNotify.AddCommLog("接收主软件：FJ_Start_MJ");
                                }
                                else
                                {
                                    SendAlarmStop("开始膜卷失败");
                                    // MessageNotify.AddStopAlarm(new StopAlarmMessage() { time = DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss"), StopAlarmState = StopAlarmStateEnum.停机, name = "开始膜卷失败", describe = "开始膜卷失败" });

                                }
                            }

                            break;
                        #endregion

                        case "SPEED":
                            if (strArr.Length > 1)
                            {
                                slaveMachineParam.fMacheineSpeed = Convert.ToDouble(strArr[1]); ;

                            }

                            break;

                        //======【】STOP_MJ 结束膜卷
                        #region >>> STOP_MJ 结束膜卷
                        case "STOP_MJ":
                            if (true)
                            {
                                //==============================================================================================================================
                                //=======【】停止运行
                                if (Project.Instance.Is_Run)
                                {
                                    StopRunEvent?.Invoke();
                                }
                                //==============================================================================================================================
                                //=======【】结束膜卷
                                bool? result = StopMJEvent?.Invoke();
                                if (result == true)
                                {

                                    bool F = TCPIP.TCP_Send("FJ_Stop_MJ", SendLogPath);
                                    if (!F) WriteFailCount++;
                                    MessageNotify.AddCommLog("接收主软件：FJ_Stop_MJ");
                                }
                                else
                                {
                                    SendAlarmStop("结束膜卷失败");
                                    // MessageNotify.AddStopAlarm(new StopAlarmMessage() { time = DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss"), StopAlarmState = StopAlarmStateEnum.停机, name = "结束膜卷失败", describe = "结束膜卷失败" });
                                }
                            }
                            break;
                        #endregion

                        //======【】RUN 运行
                        #region >>>RUN 运行
                        case "RUN":
                            if (true)
                            {
                                slaveMachineParam.RunningFlag = 1;
                                bool? result = StartRunEvent?.Invoke();
                                if (result == true)
                                {
                                    bool F = TCPIP.TCP_Send("FJ_RUN", SendLogPath);
                                    if (!F) WriteFailCount++;
                                    MessageNotify.AddCommLog("接收主软件：FJ_RUN");
                                }
                                else
                                {
                                    SendAlarmStop("开始运行失败");
                                    // MessageNotify.AddStopAlarm(new StopAlarmMessage() { time = DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss"), StopAlarmState = StopAlarmStateEnum.停机, name = "开始运行失败", describe = "开始运行失败" });
                                }
                            }
                            break;
                        #endregion

                        //======【】STOP 暂停
                        #region >>>STOP 暂停
                        case "STOP":
                            if (true)
                            {
                                slaveMachineParam.RunningFlag = 0;
                                bool? result = StopRunEvent?.Invoke();
                                if (result == true)
                                {
                                    bool F = TCPIP.TCP_Send("FJ_STOP", SendLogPath);
                                    if (!F) WriteFailCount++;
                                    MessageNotify.AddCommLog("接收主软件：FJ_STOP");
                                }
                                else
                                {
                                    SendAlarmStop("停止运行失败");
                                    //  MessageNotify.AddStopAlarm(new StopAlarmMessage() { time = DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss"), StopAlarmState = StopAlarmStateEnum.停机, name = "停止运行失败", describe = "停止运行失败" });
                                }
                            }
                            break;
                        #endregion

                        //======【】EA长度
                        #region >>>EA长度
                        case "EA_Length":
                            ////if (strArr.Length > 1)
                            ////{
                            ////    double length = double.Parse(strArr[1]);
                            ////    dBJCardManager.SendImageLength(length);
                            ////    MessageNotify.AddCommLog($"接收主软件：EA_Length_{length}");
                            ////}
                            break;
                        #endregion

                        #region  >>>设备联动登陆
                        case "MQAccountLogin":
                            string userType = strArr[1];
                            string userId = strArr[2];
                            string Password = strArr[3];

                            UserType user = userManage.ToUserType(userType);
                            UserCtrlStatus result1 = userManage.UserLogin(user, userId, Password);

                            if (result1 == UserCtrlStatus.UserNameWrong)
                            {

                                userManage.AddUser(user, userId, Password);
                                userManage.userData.Save();
                                result1 = userManage.UserLogin(user, userId, Password);
                                MessageNotify.AddErrorLog($"本地用户不存在，自动创建用户,userType_{userType}_userId_{userId}_Password_{Password}");


                            }
                            if (result1 == UserCtrlStatus.PasswordRight)
                            {
                                MessageNotify.AddDebugLog($"用户{userId}登陆成功");
                            }
                            else
                            {
                                MessageNotify.AddErrorLog($"用户{userId}登陆失败");
                            }
                            MessageNotify.AddCommLog("接收主软件：MQAccountLogin");
                            break;
                        #endregion
                        //=============================================================================================================
                        //=======【】MQAccount_LogOff 设备联动退出登陆
                        #region  >>>设备联动退出登陆
                        case "FQAccountLogOff":

                            userManage.UserLogin(UserType.Null, "用户未登录", "");
                            MessageNotify.AddCommLog("接收主软件：FQAccountLogOff");
                            break;
                            #endregion
                    }

                }

            }
            catch (Exception ee)
            {

                MessageNotify.AddErrorLog(ee.ToString());
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

        #region 发送

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

                    TJBJInfo.AlarmWriteState(AlarmName.副机有硬件连接异常, new StopAlarmMessage() { StopAlarmState = StopAlarmStateEnum.停机, describe = "副机连接异常" });

                }

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
                Close();
                TJBJInfo.AlarmWriteState(AlarmName.副机有硬件连接异常, new StopAlarmMessage() { StopAlarmState = StopAlarmStateEnum.停机, describe = "副机连接异常" });


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

                    TJBJInfo.AlarmWriteState(AlarmName.副机有硬件连接异常, new StopAlarmMessage() { StopAlarmState = StopAlarmStateEnum.停机, describe = "副机连接异常" });

                }


                //解析读取到的字符串
                DBJReadResponse(RedPlcData_str);

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
                Close();
                //TJBJInfo.AlarmWriteState(AlarmName.打标机软件连接失败, StopAlarmStateEnum.停机, "");
                TJBJInfo.AlarmWriteState(AlarmName.副机有硬件连接异常, new StopAlarmMessage() { StopAlarmState = StopAlarmStateEnum.停机, describe = "副机连接异常" });

            }
        }


        public void SendDataSum(string strArr)
        {
            try
            {
                //将strArr转成用逗号分隔的字符串

                string str = $"Sum,{strArr}";
                if (strArr.Length > 0)
                {
                    bool F = TCPIP.TCP_Send(str, SendLogPath);
                    if (!F) WriteFailCount++;
                }
                else
                {
                    Log4netRecord.ErrorLog.Debug("打标机传入的数值有误");
                }

            }
            catch (Exception ee)
            {
                MessageNotify.AddErrorLog(ee.ToString());
                //throw;
            }
        }


        public void SendDBInfo(long frame, (int, string)[] DBChannalArray, int CameraNum, out string strSend)
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

        public void SendOwnDB(string strArr)
        {
            try
            {
                //将strArr转成用逗号分隔的字符串


                string str = $"ZBDB,{strArr}";
                if (strArr.Length > 0)
                {
                    bool F = TCPIP.TCP_Send(str, SendLogPath);
                    if (!F) WriteFailCount++;
                }
                else
                {
                    Log4netRecord.ErrorLog.Debug("打标机传入的数值有误");
                }

            }
            catch (Exception ee)
            {
                MessageNotify.AddErrorLog(ee.ToString());
                //throw;
            }
        }

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
                        bool F = TCPIP.TCP_Send("FJ_Heart", SendHeartPath);
                        if (!F) WriteFailCount++;
                    }
                }
            }
            catch (Exception ex)
            {

                MessageNotify.AddErrorLog(ex.ToString());
            }
        }



        public void SendAlarm(string alarmName)
        {
            try
            {
                bool F = TCPIP.TCP_Send("MC_Alarm," + alarmName, SendLogPath);
                if (!F) WriteFailCount++;
            }
            catch (Exception ee)
            {
                MessageNotify.AddErrorLog(ee.ToString());
                //throw;
            }
        }

        public void SendAlarmStop(string alarmName)
        {
            try
            {
                bool F = TCPIP.TCP_Send("MC_AlarmStop," + alarmName, SendLogPath);
                if (!F) WriteFailCount++;
            }
            catch (Exception ee)
            {
                MessageNotify.AddErrorLog(ee.ToString());
                //throw;
            }
        }
        #endregion

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
    }
}

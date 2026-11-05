using LogTool;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Frame_Core
{
    /// <summary>
    /// Remoting副机
    /// </summary>
    public class SlaveMachine
    {
        /// <summary>
        /// 主副机通讯参数
        /// </summary>
        public MainSlaveMachineParameter mainSlaveMachineParame = new MainSlaveMachineParameter();

        public RemotingCom Remo = new RemotingCom();//Remoting通讯

        public bool InitParam(string path, Boolean isCopy = false)
        {
            try
            {
                if (!isCopy)
                {
                    if (mainSlaveMachineParame == null) mainSlaveMachineParame = new MainSlaveMachineParameter();
                    mainSlaveMachineParame.filePath = path;
                    mainSlaveMachineParame.Init();
                    mainSlaveMachineParame = (MainSlaveMachineParameter)mainSlaveMachineParame.Read();
                    mainSlaveMachineParame.filePath = path;

                }
                else
                {
                }
            }
            catch (Exception ex)
            {

                Log4netRecord.ErrorLog.Debug(ex);
                return false;
            }

            return true;

        }

        public string logPath = "";

        /// <summary>
        /// 初始化Remoting
        /// </summary>
        public void ThrIniRemoting()
        {
            Thread RemoteThread = new Thread(Ini_Remoting);
            RemoteThread.Priority = ThreadPriority.Lowest;
            RemoteThread.IsBackground = true;
            RemoteThread.Start();
        }

        /// <summary>
        /// 初始化Remoting
        /// </summary>
        private void Ini_Remoting()
        {
            if (!mainSlaveMachineParame.IsConnect) return;
            Remo.ReEvent -= DataCommonEvent_Imp;

            int SelectServer = mainSlaveMachineParame.isServer ? 1 : 0;
            string IP = mainSlaveMachineParame.Server_IP;
            string LocalIP = mainSlaveMachineParame.Client_IP;
            if (mainSlaveMachineParame.isServer)
            {
                Remo.connect(
                        SelectServer,//1服务器，0客户端
                        0,//本地端口
                        889,//远程端口
                        IP,//服务器IP
                        IP//本地端口
                        , 0);
            }
            else
            {
                Remo.connect(
                        SelectServer,//1服务器，0客户端
                        0,//本地端口
                        889,//远程端口
                        IP,//服务器IP
                        LocalIP//本地端口
                        , 0);
            }
            if (Remo.linkStatus > 0)
            {
                Remo.ReEvent += DataCommonEvent_Imp;
                isServerOpen = true;

            }
            else
            {
                isServerOpen = false;
            }

            //=========================================================================================================
            //======【】
            //if (!RemotingHreart)
            //    ServerAlive();
        }


        #region 副机
        /// <summary>
        ///服务器打开
        /// </summary>
        public bool isServerOpen;

        /// <summary>
        ///客户端状态
        /// </summary>
        public bool clientState;


        bool RemotingHreart = false;
        DateTime Tick2 = DateTime.Now;
        private int RemotingHreart_ErrorCount = 0;
        /// <summary>
        /// 客户端检测服务器是否在线线程
        /// </summary>
        private void ServerAlive()
        {
            RemotingHreart = true;
            Thread threadHeartbeat = new Thread(new ThreadStart(delegate ()
            {
                while (RemotingHreart)
                {
                    try
                    {
                        //=========================================================================================================================
                        //=====【】
                        if (DateTime.Now.Subtract(Tick2).TotalMilliseconds > 3000)//3秒检测一次
                        {
                            if (Remo.Isconnected && Remo.linkStatus <= 0)//如果能获取到服务器状态，并且当前的状态是断开的就开启断线重连机制
                            {
                                Ini_Remoting();//断线重连 
                            }
                            else if (Remo.linkStatus <= 0)//服务器没开
                            {
                                clientState = false;
                            }
                            Tick2 = DateTime.Now;
                        }
                        //=========================================================================================================================
                        //=====【】分切机台断开连接
                        if (Remo.linkStatus <= 0)
                        {
                            RemotingHreart_ErrorCount++;
                            if (clientState)
                            {
                                if (RemotingHreart_ErrorCount <= 9)
                                {
                                    Remo.Write_SendLog_ini(logPath, "分切机台异常断开连接,模切自动重连..." + RemotingHreart_ErrorCount.ToString(), "");
                                }
                                else
                                {
                                    RemotingHreart_ErrorCount = 0;
                                    Remo.Write_SendLog_ini(logPath, "分切机台断开连接,模切自动重连...", "");

                                }
                            }
                            else
                            {
                                RemotingHreart_ErrorCount = 0;
                            }
                            clientState = false;
                        }
                        else
                        {
                            RemotingHreart_ErrorCount = 0;
                        }

                    }
                    catch (Exception ex)
                    {
                        Log4netRecord.ErrorLog.Debug(ex);
                    }
                    Thread.Sleep(1000);
                }
                RemotingHreart = false;
            }));
            threadHeartbeat.Priority = ThreadPriority.Lowest;
            threadHeartbeat.Name = "Heartbeat_Remoting_Client";
            threadHeartbeat.IsBackground = true;
            threadHeartbeat.Start();
        }

        public delegate bool BeginDelegate(string message, string parameter = "");
        public event BeginDelegate BeginEvent;
        /// <summary>
        /// Remoting接收远程主机数据
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="parametersArray"></param>
        private void DataCommonEvent_Imp(string eventName, object parametersArray)
        {
            try
            {
                string[] strs = new string[1];
                if (!parametersArray.GetType().IsArray)
                {
                    strs[0] = parametersArray.ToString();
                }
                else
                {
                    strs = (string[])parametersArray;
                }
                if ((null == strs) || (strs.Length <= 0))
                {
                    return;
                }

                string[] DataArray = new string[1];

                //============================================================================================================
                //======【】解析信号数据
                switch (eventName)
                {

                    //============================================================================================================
                    //======【】 心跳信号
                    #region >>> 心跳信号
                    case "ZJ_Heart":
                        Remo.SendMsg("FJ_Heart", "副机心跳信号");
                        break;
                    #endregion

                    //============================================================================================================
                    //======【】开始膜卷
                    #region >>>RUN_MJ 开始膜卷
                    case "ZJ_Start_MJ":
                        if (true)
                        {
                            bool? result = BeginEvent?.Invoke("ZJ_Start_MJ", strs[0]);                        
                        }

                        break;
                    #endregion
                    //============================================================================================================
                    //======【】STOP_MJ 结束膜卷
                    #region >>> STOP_MJ 结束膜卷
                    case "ZJ_Stop_MJ":
                        if (true)
                        {
                            bool? result = BeginEvent?.Invoke("ZJ_Stop_MJ");
                        
                        }
                        break;
                    #endregion
                    //============================================================================================================
                    //======【】RUN 运行
                    #region >>>RUN 运行
                    case "ZJ_RUN":
                        if (true)
                        {
                            bool? result = BeginEvent?.Invoke("ZJ_RUN");
                       
                        }
                        break;
                    #endregion
                    //============================================================================================================
                    //======【】STOP 暂停
                    #region >>>STOP 暂停
                    case "ZJ_STOP":
                        if (true)
                        {
                            bool? result = BeginEvent?.Invoke("ZJ_STOP");
                      
                        }
                        break;
                    #endregion

                    //============================================================================================================
                    //======【】ERROR
                    #region >>> ERROR
                    case "ZJ_ERROR":

                        break;
                    #endregion

                    //============================================================================================================
                    //======【】硬件异常
                    #region >>> 硬件异常
                    case "ZJ_HardwareException":

                        break;
                    #endregion

                    //============================================================================================================
                    //======【】报警信息
                    #region >>> FJAlarm 副机报警信息
                    case "ZJ_Alarm":

                        break;
                    #endregion


                    default: break;
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Fatal(ex);
            }
        }
        #endregion
    }
}

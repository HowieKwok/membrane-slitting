using System;
using System.Threading;
using System.Collections.Generic;
using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels.Http;
using System.Runtime.Serialization.Formatters;
using System.Reflection;
using remoteObj;
using LogTool;

namespace Frame_Core
{
    public class RemotingCom : FileOperationINI
    {

        private RemotingObject marshal_obj;//Remoting类
        Swap wrapper = new Swap();//事件交互类
        bool ISserver = false;//是否为服务器
        int _Port = 0;//本地端口
        int _Server_connectTimes = 0;//服务器重新连接次数
        int _Client_connectTimes = 0;//客户端重新连接次数
        IChannel channel;
        ObjRef objRef = new ObjRef();
        /// <summary>
        /// 0 offline连接失败  1 online正常连接.
        /// </summary>
        public int linkStatus { set; get; }

        public delegate void ReceiveEventHandler(string eventName, object parametersArray);
        public event ReceiveEventHandler ReEvent;//主界面触发使用
        /// <summary>
        /// 初始化连接
        /// </summary>
        /// <param name="SelectServer">0客户端，1服务器</param>
        /// <param name="LocalPort">本地端口</param>
        /// <param name="ServerPort">服务器端口</param>
        /// <param name="IP">服务器IP</param>
        /// <param name="SelectModel">连接模式：0 TCP连接，1 HTTP连接</param>
        /// <returns></returns>
        public bool connect(int SelectServer, int LocalPort, int ServerPort, string IP, string LocalIP, int SelectModel)
        {
            BinaryServerFormatterSinkProvider serverProvider = new
            BinaryServerFormatterSinkProvider();
            BinaryClientFormatterSinkProvider clientProvider = new
            BinaryClientFormatterSinkProvider();
            serverProvider.TypeFilterLevel = TypeFilterLevel.Full;
            RemotingConfiguration.Configure(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile, false);
            IDictionary props;
            try
            {
                ISserver = false;
                Disconnect();
                if (SelectServer == 0)//客户端
                {
                    props = new Hashtable();
                    props["port"] = LocalPort;
                    props["bindto"] = LocalIP;
                    if (_Client_connectTimes == 0)
                    {
                        if (SelectModel == 0)
                            channel = new TcpChannel(props, clientProvider, serverProvider);
                        else
                            channel = new HttpChannel(props, clientProvider, serverProvider);

                        try
                        {
                            IChannel[] ichannel = ChannelServices.RegisteredChannels;
                            if (ichannel.Length > 0)
                            {
                                foreach (TcpChannel chan in ichannel)
                                {
                                    ChannelServices.UnregisterChannel(chan);//释放掉已经注册的通道
                                }
                            }
                            ChannelServices.RegisterChannel(channel, false);
                        }
                        catch (Exception)
                        {
                            //LogName.ErrorLog.Error(ex);
                            ChannelServices.RegisterChannel(channel, false);
                        }

                    }
                    //获取代理
                    string url = "tcp://127.0.0.1:8888/url";
                    if (SelectModel == 0)
                        url = "tcp://" + IP + ":" + ServerPort.ToString() + "/url";
                    else
                        url = "http://" + IP + ":" + ServerPort.ToString() + "/url";

                    marshal_obj = (RemotingObject)Activator.GetObject(typeof(RemotingObject), url);

                }
                else//服务端
                {
                    ISserver = true;
                    props = new Dictionary<string, int>();
                    LocalPort = ServerPort;
                    props["port"] = ServerPort;

                    if (_Server_connectTimes == 0)
                    {
                        if (SelectModel == 0)
                            channel = new TcpChannel(props, clientProvider, serverProvider);
                        else
                            channel = new HttpChannel(props, clientProvider, serverProvider);

                        ChannelServices.RegisterChannel(channel, false);
                    }
                    else//服务器不可重新连接，否则客户端会出问题
                    {
                        return false;
                        //RemotingServices.Unmarshal(objRef);
                        //RemotingServices.Disconnect(marshal_obj);
                    }
                    //服务器获取远程对象
                    marshal_obj = new RemotingObject();
                    objRef = RemotingServices.Marshal(marshal_obj, "url");
                }
                _Port = LocalPort;
                if (!Isconnected)
                {
                    linkStatus = 0;
                    return false;
                }
                BrindConnect();//绑定事件
                _Server_connectTimes++;
                _Client_connectTimes++;
            }
            catch (Exception ex)
            {
                try
                {
                    ChannelServices.UnregisterChannel(channel);//释放掉已经注册的通道
                }
                catch
                {
                }
                Log4netRecord.ErrorLog.Error(ex);
                linkStatus = 0;
                return false;
            }
            linkStatus = 1;
            return true;
        }
        //private static readonly Object LockReceive = new object();
        /// <summary>
        /// 触发事件
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="eventName"></param>
        /// <param name="parametersArray"></param>
        void DataCommonEvent_Imp(int ID, string eventName, object[] parametersArray)
        {
            //lock (LockReceive)//如果不是自己发送的消息
            {
                if (ReEvent != null)
                {
                    ReEvent.BeginInvoke(eventName, parametersArray[0], null, null);
                }
            }
        }
        static readonly object SendLock = new object();
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="parametersArray"></param>
        public void SendMsg(string eventName, object parametersArray)
        {
            lock (SendLock)
            {
                try
                {
                    if (marshal_obj != null && linkStatus == 1)
                    {
                        if (ISserver)
                            marshal_obj.ServerSend(_Port, eventName, parametersArray);//遍历所有的客户端发送消息 
                        else
                            marshal_obj.ServerReceive(_Port, eventName, parametersArray);//向服务器发送消息
                    }
                }
                catch (Exception)
                {
                    //LogName.ErrorLog.Error(ex);
                }
            }


        }
        /// <summary>
        /// 绑定连接
        /// </summary>
        private void BrindConnect()
        {
            if (ISserver)
            {
                marshal_obj.DataCommonEvent -= DataCommonEvent_Imp;
                marshal_obj.DataCommonEvent += DataCommonEvent_Imp;
            }
            else
            {
                marshal_obj.CommonEvent -= wrapper.OnSwapCommonEvent;
                marshal_obj.CommonEvent += wrapper.OnSwapCommonEvent;//绑定SwapCommonEvent
                wrapper.SwapCommonEvent -= DataCommonEvent_Imp;
                wrapper.SwapCommonEvent += DataCommonEvent_Imp;//绑定事件
            }

        }
        /// <summary>
        /// 关闭连接，清除资源
        /// </summary>
        public void Disconnect()
        {
            try
            {
                if (!ISserver && linkStatus == 1 && Isconnected)
                {
                    if (marshal_obj != null)
                        marshal_obj.CommonEvent -= wrapper.OnSwapCommonEvent;
                }
                else if (ISserver && linkStatus == 1)
                {
                    if (marshal_obj != null)
                    {
                        marshal_obj.DataCommonEvent -= DataCommonEvent_Imp;
                        RemotingServices.Unmarshal(objRef);
                        RemotingServices.Disconnect(marshal_obj);
                    }
                }
            }
            catch (Exception)
            {
                //LogName.ErrorLog.Error(ex);
            }

            linkStatus = 0;
        }
        /// <summary>
        /// 获取当前服务器的连接状态
        /// </summary>
        public bool Isconnected
        {
            get
            {
                try
                {
                    if (marshal_obj.getAlive() == 80)
                        return true;
                    else
                    {
                        linkStatus = 0;//不是当前连接的服务器
                        return false;
                    }
                }
                catch
                {
                    linkStatus = 0;//服务器离线
                    return false;
                }

            }
        }
        /// <summary>
        /// 将类型T转换为V(将对应T类的属性赋值给V类的属性)
        /// </summary>
        public V Conversion<T, V>(T anyObject)
        {
            //数据不能为null，否则传输时会出现异常无法传输

            V TheTergetObject = System.Activator.CreateInstance<V>();

            //反射公共方法以及属性
            PropertyInfo[] AnyObject_PDs = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            PropertyInfo[] TheTergetObject_PDs = typeof(V).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            for (int j = 0; j < AnyObject_PDs.Length; j++)
            {
                PropertyInfo AnyObject_PD = AnyObject_PDs[j];
                PropertyInfo TheTergetObject_PD = TheTergetObject_PDs[j];

                TheTergetObject_PD.SetValue(TheTergetObject, AnyObject_PD.GetValue(anyObject, null));
            }
            return TheTergetObject;
        }
    }
}

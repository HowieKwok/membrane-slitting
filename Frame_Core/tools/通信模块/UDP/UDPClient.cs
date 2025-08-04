using LogTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    public class UDPClient : FileOperationINI
    {
        public string UdpServeIp = "127.0.0.1";//服务器IP
        public string UdpClientIp = "127.0.0.1";//服务器IP
        public int UdpServePort = 2000;//服务器端口
        public int UdpClientPort = 2001;//客户端端口
        static Socket UdpClient1; //连接句柄 
        static UdpClient udpclient;
        public object sendTcpLock = new object();//锁存，线程互斥，保证线程完整执行
        public object sendLock = new object();//锁存，线程互斥，保证线程完整执行
        public static readonly object LogLock = new object();//锁存，线程互斥，保证线程完整执行
        EndPoint UdpSendPoint;//用来保存发送服务器的ip和端口号
        IPEndPoint UdpSendPosPoint;
        EndPoint UdpReadPoint;//用来保存接收服务器的ip和端口号
        string strTempRead = "";//接收缓存
        string strTempSend = "";//发送缓存

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="ServeIP">服务器IP</param>
        /// <param name="ServePort">服务器端口</param>
        /// <param name="ClientPort">客户端端口</param>
        public UDPClient()
        {
          
        }
        /// <summary>
        /// 创建连接句柄
        /// </summary>
        public bool IniUdpClient(string ServeIP, int ServePort, string ClientIP, int ClientPort)
        {
            UdpServeIp = ServeIP;
            UdpClientIp = ClientIP;
            UdpServePort = ServePort;
            UdpClientPort = ClientPort;
            bool bIsConnect = false;
            try
            {
                UdpClient1 = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                UdpClient1.Bind(new IPEndPoint(IPAddress.Parse(UdpClientIp), UdpClientPort));
                UdpSendPoint = new IPEndPoint(IPAddress.Parse(UdpServeIp), UdpServePort);
                UdpReadPoint = new IPEndPoint(IPAddress.Any, 0);
                UdpClient1.ReceiveTimeout = 5000;

                //udpclient = new UdpClient(new IPEndPoint(IPAddress.Parse(UdpClientIp), 5000));
                //udpclient.EnableBroadcast = true;
                //udpclient.JoinMulticastGroup(IPAddress.Parse("224.3.0.0"));
                //UdpSendPosPoint = new IPEndPoint(IPAddress.Parse("224.3.0.0"), 57007);
                bIsConnect = true;
            }
            catch (Exception ex)
            {
                 Log4netRecord.ErrorLog.Info(ex.Message + ex.StackTrace);
                bIsConnect = false;
            }
            return bIsConnect;
        }
        /// <summary>
        /// 关闭连接句柄
        /// </summary>
        public void CloseClient()
        {
            if (UdpClient1 != null)
            {
                UdpClient1.Close();
            }
        }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="Sendmsg">数据</param>
        /// <param name="LogPath">日志</param>
        /// <returns></returns>
        public int SendData(Byte[] sendBytes)
        {
            try
            {
                int iSendSuc = -1;
                lock (sendTcpLock)
                {
                    try
                    {
                        int iSend = UdpClient1.SendTo(sendBytes, UdpSendPoint);
                        iSendSuc = (iSend == sendBytes.Length) ? 0 : -4;//0为发送成功，-4为发送数据长度不对                       
                        System.Threading.Thread.Sleep(2);
                    }
                    catch
                    {
                        iSendSuc = -2;//发送失败
                    }
                }
                string str = "";
                for (int i = 0; i < sendBytes.Length; i++)
                {
                    str += sendBytes[i].ToString("x2");
                }
                if (str.Substring(2, str.Length - 2) != strTempSend)
                {
                    strTempSend = str.Substring(2, str.Length - 2);
                    //  SendLog(LogPath, "发送数据：", str);
                }
                return iSendSuc;
            }
            catch (Exception ex)
            {
                 Log4netRecord.ErrorLog.Info(ex.Message + ex.StackTrace);
                return -2;
            }
        }
        public int SendPosData(Byte[] sendBytes, string LogPath)
        {
            try
            {
                int iSendSuc = -1;
                lock (sendLock)
                {
                    try
                    {
                        int iSend = udpclient.Send(sendBytes, sendBytes.Length, UdpSendPosPoint);
                        System.Threading.Thread.Sleep(1);
                        if (iSend != 43)
                             Log4netRecord.ErrorLog.Info("电眼数据包发送异常 iSend:" + iSend.ToString());
                    }
                    catch
                    {
                        //iSendSuc = -2;//发送失败

                    }
                }
                string str = "";
                for (int i = 0; i < sendBytes.Length; i++)
                {
                    str += sendBytes[i].ToString("x2");
                }
                if (str.Substring(2, str.Length - 2) != strTempSend)
                {
                    strTempSend = str.Substring(2, str.Length - 2);
                    //  SendLog(LogPath, "发送数据：", str);
                }
                return iSendSuc;
            }
            catch (Exception ex)
            {
                 Log4netRecord.ErrorLog.Info(ex.Message + ex.StackTrace);
                return -2;
            }
        }
        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="LogPath">日志</param>
        /// <returns></returns>
        public bool ReadData(string LogPath, out byte[] buffer)
        {
            buffer = new byte[1024];//存放接收缓存数据
            int length = 0;
            try
            {
               
                length = UdpClient1.ReceiveFrom(buffer, ref UdpReadPoint); //接收数据报

                string str = "";
                for (int i = 0; i < length; i++)
                {
                    str += buffer[i].ToString("x2");
                }
                if (str.Substring(2, str.Length - 2) != strTempRead)
                {
                    strTempRead = str.Substring(2, str.Length - 2);
                    //ReadLog(LogPath, "接收数据：", str);
                }
                return true;
            }
            catch (Exception ex)
            {
                 Log4netRecord.ErrorLog.Info(ex.Message + ex.StackTrace);
                //Console.WriteLine("接收失败：" + ex.Message + ex.StackTrace);
                return false;
            }


        }
        /// <summary>
        /// 发送日志记录
        /// </summary>
        /// <param name="filePath">路径</param>
        /// <param name="message"></param>
        /// <param name="track"></param>
        public void SendLog(string filePath, string message, string track)
        {
            lock (LogLock)
            {
                try
                {
                    System.IO.StreamWriter writer = new System.IO.StreamWriter(filePath, true, Encoding.UTF8);
                    writer.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "   " + message + "\t" + track);
                    writer.Flush();
                    writer.Close();
                    writer.Dispose();
                }
                catch (Exception ex)
                {
                     Log4netRecord.ErrorLog.Info(ex.Message + ex.StackTrace);
                }
            }
        }
        /// <summary>
        /// 接收日志记录
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="message"></param>
        /// <param name="track"></param>
        public void ReadLog(string filePath, string message, string track)
        {
            lock (LogLock)
            {
                try
                {
                    System.IO.StreamWriter writer = new System.IO.StreamWriter(filePath, true, Encoding.UTF8);
                    writer.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "   " + message + "\t" + track);
                    writer.Flush();
                    writer.Close();
                    writer.Dispose();
                }
                catch (Exception ex)
                {
                     Log4netRecord.ErrorLog.Info(ex.Message + ex.StackTrace);
                }
            }
        }
    }
}

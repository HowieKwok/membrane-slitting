using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DevMgmt.Light.Tool
{
    public class RS232
    {
        SerialPort serialPort = new SerialPort();//串行端口
        public bool Connected { set; get; }
        public string acceptmsg { set; get; }
        public int ReceiveLength { set; get; }

        /// <summary>初始化连接串口
        /// 
        /// </summary>
        /// <param name="PortName">端口号</param>
        /// <param name="BaudRate">波特率</param>
        /// <param name="StopBit">停止位：None = 0,One = 1,Two = 2,OnePointFive = 3</param>
        /// <param name="Parity">校验位：None = 0,Odd = 1,Even = 2,Mark = 3,Space = 4</param>
        /// <param name="DataBit">标准数据长度</param>
        /// <param name="ReceiveBufLength">接收数据长度，0为不接收</param>
        /// <returns></returns>
        public bool Connect(string PortName, int BaudRate, int StopBit, int Parity, int DataBit, int ReceiveBufLength)
        {
            try
            {
                if (serialPort.IsOpen)
                {
                    serialPort.Close();
                }
                ReceiveLength = ReceiveBufLength;
                serialPort.PortName = PortName;
                serialPort.BaudRate = BaudRate;
                serialPort.StopBits = (StopBits)StopBit;
                serialPort.Parity = (Parity)Parity;
                serialPort.DataBits = DataBit;
                serialPort.Open();
                serialPort.DiscardInBuffer();
                serialPort.DiscardOutBuffer();
                serialPort.RtsEnable = true;
                serialPort.DtrEnable = true;
                if (ReceiveLength > 0)
                {
                    serialPort.DataReceived += serialPort_DataReceived;
                }
            }
            catch (Exception)
            {
                //LogName.ErrorLog.Fatal(ex);
                Connected = false;
                return Connected;
            }
            Connected = true;
            return Connected;
        }

        public int Connect(string PortName)
        {
            try
            {
                if (serialPort.IsOpen)
                {
                    serialPort.Close();
                }
                serialPort.Open();
                serialPort.DiscardInBuffer();
                serialPort.DiscardOutBuffer();
                serialPort.RtsEnable = true;
                serialPort.DtrEnable = true;
                if (ReceiveLength > 0)
                {
                    serialPort.DataReceived += serialPort_DataReceived;
                }
            }
            catch
            {
                //LogName.ErrorLog.Fatal(ex);
                Connected = false;
                return -1;
            }
            Connected = true;
            return 0;
        }
        public bool Connect(int BaudRate)
        {
            try
            {
                serialPort.BaudRate = BaudRate;
                if (serialPort.IsOpen)
                {
                    serialPort.Close();
                }
                serialPort.Open();
                serialPort.DiscardInBuffer();
                serialPort.DiscardOutBuffer();
                serialPort.RtsEnable = true;
                serialPort.DtrEnable = true;
            }
            catch
            {
                //LogName.ErrorLog.Fatal(ex);
                Connected = false;
                return Connected;
            }
            Connected = true;
            return Connected;
        }

        /// <summary>接收串口数据回调函数
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                int length = serialPort.BytesToRead;
                if (length >= ReceiveLength)
                {
                    byte[] buf = new byte[ReceiveLength];//固定发送接收64位数据
                    int DataLength = serialPort.Read(buf, 0, ReceiveLength);
                    acceptmsg = Encoding.UTF8.GetString(buf, 0, DataLength);
                    serialPort.DiscardInBuffer();//清除缓存区数据，可能会丢数据；不清楚缓存区数据，可能会接收到错乱的数据
                }
            }
            catch
            {
                //LogName.ErrorLog.Fatal(ex);
            }
        }

        public int GetBaudRate()
        {
            return serialPort.BaudRate;
        }

        private object sendLock = new object();
        public void Send(string sendData)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(sendMessage), sendData);
            //Thread thr_send = new Thread(sendMessage);
            //thr_send.Start(sendData);
        }
        public void Send(byte[] sendData)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(sendBytes), sendData);
            //Thread thr_send = new Thread(sendBytes);
            //thr_send.Start(sendData);
        }
        public void sendBytes(object sendData)
        {
            lock (sendLock)
            {
                try
                {
                    byte[] Buf = (byte[])sendData;
                    if (serialPort.IsOpen)
                    {
                        serialPort.Write(Buf, 0, Buf.Length);
                    }
                    Thread.Sleep(30);
                }
                catch (Exception)
                {
                    //LogName.ErrorLog.Fatal(ex);
                }
            }

        }
        public void sendMessage(object sendData)
        {
            lock (sendLock)
            {
                try
                {
                    string msg = sendData.ToString();
                    if (serialPort.IsOpen)
                    {
                        serialPort.Write(msg);
                    }
                    Thread.Sleep(10);
                }
                catch
                {
                    //LogName.ErrorLog.Fatal(ex);
                }
            }
        }
        public string AStoSTR(int channel, int Value)
        {
            lock (sendLock)
            {
                string output = "";
                string str_value = "";
                int cc;
                output = "$3" + channel.ToString() + Value.ToString("X3");
                byte[] array = new byte[output.Length];
                array = Encoding.ASCII.GetBytes(output);
                cc = array[0] ^ array[1] ^ array[2] ^ array[3] ^ array[4] ^ array[5];
                str_value = cc.ToString("X2");

                output = output + str_value;
                return output;
            }
        }
        public string AStoSTR3(int channel, int Value)
        {
            lock (sendLock)
            {
                string output = "";
                string str_value = "";
                int cc;
                output = "#3" + channel.ToString() + Value.ToString("X3");
                byte[] array = new byte[output.Length];
                array = Encoding.ASCII.GetBytes(output);
                cc = array[0] ^ array[1] ^ array[2] ^ array[3] ^ array[4] ^ array[5];
                str_value = cc.ToString("X2");

                output = output + str_value;
                return output;
            }
        }
        public byte[] AStoSTRByte(int channel, int Value)
        {
            string output = "";
            byte[] k = null;
            try
            {
                lock (sendLock)
                {
                    output = "FF 01 0" + channel.ToString() + " 00 " + Value.ToString("X2") + " 00";
                    output = output.Replace(" ", "");//去除16进制数据中所有空格
                    output = output.Replace("\r", "");//去除16进制数据中所有换行
                    output = output.Replace("\n", "");//去除16进制数据中所有换行
                    List<string> sendData16 = new List<string>();//将发送的数据，2个合为1个，然后放在该缓存里 如：123456→12,34,56
                    for (int i = 0; i < output.Length; i += 2)
                    {
                        sendData16.Add(output.Substring(i, 2));
                    }
                    k = new byte[sendData16.Count];//sendBuffer的长度设置为：发送的数据2合1后的字节数
                    for (int i = 0; i < sendData16.Count; i++)
                    {
                        k[i] = (byte)(Convert.ToInt32(sendData16[i], 16));//发送数据改为16进制
                    }
                }
            }
            catch
            {
                //LogName.ErrorLog.Fatal(ex);
            }
            return k;
        }
        private string ASCIIToHex(string str)
        {
            try
            {
                byte[] a = Encoding.UTF8.GetBytes(str.Trim());
                string mystr1 = "";
                for (int i = 0; i < a.Length; i++)
                {
                    mystr1 += a[i].ToString("X2") + " ";
                }
                return mystr1;
            }
            catch
            {
                //LogName.ErrorLog.Fatal(ex);
                return str;
            }
        }
        private string HexToASCII(string str)
        {
            try
            {
                string[] mystr1 = str.Trim().Split(' ');
                byte[] t = new byte[mystr1.Length];
                for (int i = 0; i < t.Length; i++)
                {
                    t[i] = Convert.ToByte(mystr1[i], 16);
                }
                return Encoding.UTF8.GetString(t);
            }
            catch
            {
                //LogName.ErrorLog.Fatal(ex);
                return str;
            }
        }
        public void Close()
        {
            if (serialPort.IsOpen)
            {
                serialPort.Close();
                serialPort.Dispose();
            }
        }



    }
}

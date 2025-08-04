using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.Threading;
using System.ComponentModel;
using System.Reflection;
using Frame_Core;
using System.Text.RegularExpressions;
using LogTool;

namespace Frame_Core
{

    public class FinsTcp : FileOperationINI
    {
        DigitalTrans digitalTrans = new DigitalTrans();
        public FinsTcp()
        {

        }

        public string IP = "127.0.0.1";//服务器地址;
        public int Port = 9600;//服务器端口
        public byte Fra = 0x64;//本地IP地址的第四个节点
        public bool connected = false;

        private Socket msock = null;
        private TcpClient msender = null;
        private object sendTcpLock = new object();//锁存，线程互斥，保证线程完整执行
        private string ServerIP_LastOne = "";
        private string ClientIP_LastOne = "";
        private byte[] Client = new byte[4];//本地工控机IP
        private byte[] Server = new byte[4];//PLC服务器IP
        private string[] StrServer_Array = { "127", "0", "0", "1" };
        private string[] StrClient_Array = { "127", "0", "0", "1" };


        #region >>>连接
        /// <summary>
        /// FinsTcp连接
        /// </summary>
        /// <param name="Hostname">服务 端IP</param>
        /// <param name="Port">服务端 端口</param>
        /// <param name="LocalIP">本地IP</param>
        /// <returns></returns>
        public bool Connect(string Hostname, int Port, string LocalIP)
        {
            if (!connected)
            {
                try
                {
                    msender = new TcpClient(Hostname, Port);
                    msock = msender.Client;

                    string[] sClient_Split = LocalIP.Split('.');//分割PC端的IP地址，获取最后一个节点的数据

                    StrClient_Array = LocalIP.Split('.');
                    StrClient_Array = LocalIP.Split('.');
                    int jj = int.Parse(sClient_Split[3]);
                    ClientIP_LastOne = jj.ToString("X").PadLeft(2, '0');
                    Client[3] = Convert.ToByte(ClientIP_LastOne, 16);



                    byte[] Handshake = new byte[20];
                    Handshake[0] = 0x46;//F
                    Handshake[1] = 0x49;//I
                    Handshake[2] = 0x4e;//N
                    Handshake[3] = 0x53;//S

                    Handshake[4] = 0;
                    Handshake[5] = 0;
                    Handshake[6] = 0;
                    Handshake[7] = 0x0c;//Length长度

                    Handshake[8] = 0;
                    Handshake[9] = 0;
                    Handshake[10] = 0;
                    Handshake[11] = 0;//Command

                    Handshake[12] = 0;
                    Handshake[13] = 0;
                    Handshake[14] = 0;
                    Handshake[15] = 0;//Error Code

                    Handshake[16] = 0;
                    Handshake[17] = 0;

                    Handshake[18] = 0;
                    Handshake[19] = Client[3];//FINS Frame (本机节点)

                    msock.Send(Handshake);
                    byte[] buffer = new byte[24];
                    msock.ReceiveTimeout = 3000;
                    msock.Receive(buffer, SocketFlags.None);
                    byte[] buf = buffer;
                    buf[0] = 0x46;//F
                    buf[1] = 0x49;//I
                    buf[2] = 0x4e;//N
                    buf[3] = 0x53;//S
                    buf[4] = 0;
                    buf[5] = 0;
                    buf[6] = 0;
                    buf[7] = 10;
                    buf[8] = 0;
                    buf[9] = 0;
                    buf[10] = 0;
                    buf[11] = 1;
                    buf[12] = 0;
                    buf[13] = 0;
                    buf[14] = 0;
                    buf[15] = 0;
                    if (buf == buffer)
                    {
                        Client[0] = buffer[16];
                        Client[1] = buffer[17];
                        Client[2] = buffer[18];
                        Client[3] = buffer[19];

                        Server[0] = buffer[20];
                        Server[1] = buffer[21];
                        Server[2] = buffer[22];
                        Server[3] = buffer[23];

                        int kk = int.Parse(Server[3].ToString());//获取PLC反馈回来的节点
                        ServerIP_LastOne = kk.ToString("X").PadLeft(2, '0');

                        connected = true;
                    }
                }
                catch (Exception ex)
                {

                    connected = false;
                }
            }
            return connected;
        }
        #endregion

        #region >>>断开连接
        /// <summary>
        /// 断开连接
        /// </summary>
        public bool disconn()
        {
            try
            {
                if (connected)
                {
                    msender.Close();
                    connected = false;
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;

            }
        }
        #endregion



        /// <summary>
        /// 读取数据：批量读取单寄存器
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="location"></param>
        /// <param name="sum"></param>
        /// <param name="bReadSuccess"></param>
        /// <param name="readdata_arr"></param>
        /// <returns></returns>
        public bool Read_Ints(string filePath, int location, int sum, bool bFeedBack, out bool bReadSuccess, out int[] readdata_arr, Boolean isSaveLog = true)
        {
            bReadSuccess = false;
            readdata_arr = new int[0];
            bool bErrorResult = true;
            string readdata = "";
            string feedback = "";
            string writeMsg = "";
            try
            {
                string[] read_arr = new string[0];
                string Location = location.ToString("X").PadLeft(4, '0');
                int iErrorCode = Read_Data(Location, sum, out readdata, out read_arr, out writeMsg, out feedback);
                if (iErrorCode == 0)
                {
                    readdata_arr = digitalTrans._16to10Int(read_arr);
                    bReadSuccess = true;
                }
                else
                {
                    bErrorResult = false;
                }
                readdata = string.Join(",", readdata_arr);//合并字符数据
                int data = 0;
                if (readdata_arr.Length > 0) data = readdata_arr.Max();
                if (data != 0 || bErrorResult == false)
                {
                    string state = bReadSuccess ? "成功" : "失败";
                    if (isSaveLog)
                    {
                        ReadLog(filePath, "读地址：D" + location + " 数量：" + sum.ToString().ToString().PadLeft(2, ' ') + " 数值：" + readdata + " 状态：" + state, writeMsg, feedback);
                    }

                }
                //================================================================
                //======【】成功读取非“0”的数据时，复位数据写入“0”
                if (bFeedBack)
                {
                    Dictionary<int, int> dict = new Dictionary<int, int>();
                    List<int> senddatas = new List<int>();
                    for (int index = 0; index < readdata_arr.Length; index++)
                    {
                        int address = location + index;
                        dict.Clear();
                        senddatas.Clear();
                        for (int i = index; i < readdata_arr.Length; i++)
                        {
                            int item = readdata_arr[i];
                            if (item == 0) break;
                            if (dict.Count == 0)
                            {
                                dict.Add(i, item);
                                senddatas.Add(item);
                                index++;
                            }
                            else
                            {
                                int itemPrevious = 0;
                                if (dict.TryGetValue(i - 1, out itemPrevious))
                                {
                                    dict.Add(i, item);
                                    senddatas.Add(item);
                                    index++;
                                }
                            }
                        }
                        if (senddatas.Count > 0)
                        {
                            int[] senddata_arr = new int[senddatas.Count];
                            for (int i = 0; i < senddata_arr.Length; i++) senddata_arr[i] = 0;
                            Write_Ints(filePath, address, senddata_arr);
                        }
                    }
                }
                ////成功读取非“0”的数据时，复位数据写入“0”
                //if (bFeedBack && data != 0)
                //{
                //    int[] senddata_arr = new int[readdata_arr.Length];
                //    for (int i = 0; i < senddata_arr.Length; i++) senddata_arr[i] = 0;
                //    Write_Ints(filePath, location, senddata_arr);
                //}
            }
            catch (Exception ex)
            {
                bErrorResult = false;
                 Log4netRecord.ErrorLog.Error(ex + "\nFins反馈信息：" + feedback);
            }
            return bErrorResult;
        }
        public bool Read_Int16(string filePath, int location, bool bFeedBack, out bool bReadSuccess, out int data, Boolean isSaveLog = true)
        {
            bReadSuccess = false;
            data = 0;

            bool bErrorResult = true;
            string[] read_arr;
            string readdata = "";
            string feedback = "";
            string writeMsg = "";
            int sum = 1;
            try
            {
                string Location = location.ToString("X").PadLeft(4, '0');
                int iErrorCode = Read_Data(Location, sum, out readdata, out read_arr, out writeMsg, out feedback);
                if (iErrorCode == 0)
                {
                    if (readdata != null && readdata != "")
                    {
                        data = Convert.ToInt32(readdata, 16);
                    }

                    bReadSuccess = true;
                }
                else
                {
                    bErrorResult = false;
                }
                readdata = data.ToString().PadRight(16, ' ');
                if ((data != 0 || bErrorResult == false) && isSaveLog)
                {
                    string state = bReadSuccess ? "成功" : "失败";
                    ReadLog(filePath, "读地址：D" + location + " 数量：" + sum.ToString().ToString().PadLeft(2, ' ') + " 数值：" + readdata + " 状态：" + state, writeMsg, feedback);
                }
                //成功读取非“0”的数据时，复位数据写入“0”
                if (bFeedBack && data != 0)
                {
                    Write_Int16(filePath, "复位寄存器数值为0", location, 0);
                }
            }
            catch (Exception ex)
            {
                bErrorResult = false;
                 Log4netRecord.ErrorLog.Error(ex + "\nFins反馈信息：" + feedback);
            }
            return bErrorResult;
        }
        public bool Read_Int32(string filePath, int location, bool bFeedBack, out bool bReadSuccess, out int data, Boolean isSaveLog = true)
        {
            bReadSuccess = false;
            data = 0;
            bool bErrorResult = true;
            string[] read_arr;
            string readdata = "";
            string feedback = "";
            string writeMsg = "";
            try
            {
                string Location = location.ToString("X").PadLeft(4, '0');
                int sum = 2;
                int iErrorCode = Read_Data(Location, sum, out readdata, out read_arr, out writeMsg, out feedback);
                if (iErrorCode == 0)
                {
                    data = digitalTrans._16to10Int32(readdata);
                    bReadSuccess = true;
                }
                else
                {
                    bErrorResult = false;
                }
                readdata = data.ToString().PadRight(16, ' ');
                if ((data != 0 || bErrorResult == false) && isSaveLog)
                {
                    string state = bReadSuccess ? "成功" : "失败";
                    ReadLog(filePath, "读地址：D" + location + " 数量：" + sum.ToString().ToString().PadLeft(2, ' ') + " 数值：" + readdata + " 状态：" + state, writeMsg, feedback);
                }
                //成功读取非“0”的数据时，复位数据写入“0”
                if (bFeedBack && data != 0)
                {
                    Write_Int32(filePath, "复位寄存器数值为0", location, 0);
                }
            }
            catch (Exception ex)
            {
                bErrorResult = false;
                 Log4netRecord.ErrorLog.Error(ex + "\nFins反馈信息：" + feedback);
            }
            return bReadSuccess;
        }
        public bool Read_Float(string filePath, int location, bool bFeedBack, out bool bReadSuccess, out float data, Boolean isSaveLog = true)
        {
            bReadSuccess = false;
            data = 0;

            bool bErrorResult = true;
            string[] read_arr;
            string readdata = "";
            string feedback = "";
            string writeMsg = "";
            int sum = 2;
            string Location = location.ToString("X").PadLeft(4, '0');
            int iErrorCode = Read_Data(Location, sum, out readdata, out read_arr, out writeMsg, out feedback);
            if (iErrorCode == 0)
            {
                data = (float)digitalTrans._16to10double(readdata);
                bReadSuccess = true;
            }
            else
            {
                bErrorResult = false;
            }
            readdata = data.ToString().PadRight(16, ' ');
            if (data != 0 || bErrorResult == false)
            {
                if (isSaveLog)
                {
                    string state = bReadSuccess ? "成功" : "失败";
                    ReadLog(filePath, "读地址：D" + location + " 数量：" + sum.ToString().ToString().PadLeft(2, ' ') + " 数值：" + readdata + " 状态：" + state, writeMsg, feedback);
                }

            }
            //成功读取非“0”的数据时，复位数据写入“0”
            if (bFeedBack && data != 0)
            {
                float fSendData = data - (float)0.15;
                fSendData = 0;
                Write_Float(filePath, location, fSendData);
            }
            return bReadSuccess;
        }
        public bool Read_Double(string filePath, int location, bool bFeedBack, out bool bReadSuccess, out double data, Boolean isSaveLog = true)
        {
            bReadSuccess = false;
            data = 0;

            bool bErrorResult = true;
            string[] read_arr;
            string readdata = "";
            string feedback = "";
            string writeMsg = "";
            int sum = 4;
            string Location = location.ToString("X").PadLeft(4, '0');
            try
            {
                int iErrorCode = Read_Data(Location, sum, out readdata, out read_arr, out writeMsg, out feedback);
                if (iErrorCode == 0)
                {
                    data = (double)digitalTrans._16to10double64(readdata);
                    bReadSuccess = true;
                }
                else
                {
                    bErrorResult = false;
                }
                readdata = data.ToString().PadRight(16, ' ');
                if (data != 0 || bErrorResult == false)
                {
                    if (isSaveLog)
                    {
                        string state = bReadSuccess ? "成功" : "失败";
                        ReadLog(filePath, "读地址：D" + location + " 数量：" + sum.ToString().ToString().PadLeft(2, ' ') + " 数值：" + readdata + " 状态：" + state, writeMsg, feedback);
                    }

                }
                //成功读取非“0”的数据时，复位数据写入“0”
                if (bFeedBack && data != 0)
                {
                    double fSendData = data - (float)0.15;
                    fSendData = 0;
                    //Write_Float(filePath, location, fSendData);
                }
            }
            catch (Exception ex)
            {
                bErrorResult = false;
                 Log4netRecord.ErrorLog.Debug(ex + "\nFins反馈信息：" + feedback);
            }
            return bReadSuccess;
        }

        string ReadStr1 = "";
        public bool Read_String(string filePath, int location, bool bFeedBack, int num, int format, out bool bReadSuccess, out string data)
        {
            bReadSuccess = false;
            data = "";
            bool bErrorResult = true;
            string readdata = "";
            string feedback = "";
            string writeMsg = "";
            try
            {
                string[] read_arr = new string[0];
                int sum = num;
                string Location = location.ToString("X").PadLeft(4, '0');
                int iErrorCode = Read_Data(Location, sum, out readdata, out read_arr, out writeMsg, out feedback);
                if (iErrorCode == 0)
                {
                    if (format == 0)
                        readdata = conver_str(readdata);
                    data = digitalTrans.HexStringToString(readdata, Encoding.GetEncoding("GB2312"));
                    data = data.Trim();
                    if (data.Contains("\0"))
                    {
                        data = data.Replace('\0', ' ');
                        data = Regex.Replace(data, @"\s", "");
                    }
                }
                else
                {
                    bErrorResult = false;
                }//&& ReadStr != readdata.Trim()
                readdata = data.ToString().PadRight(16, ' ');

                if ((data.Length != 0 || bErrorResult == false) && readdata.Trim().Length > 1 && ReadStr1 != readdata.Trim())
                {
                    bReadSuccess = true;
                    string state = bReadSuccess ? "成功" : "失败";
                    ReadLog(filePath, "读地址：D" + location + " 数量：" + sum.ToString().ToString().PadLeft(2, ' ') + " 数值：" + readdata + " 状态：" + state, writeMsg, feedback);

                    ReadStr1 = readdata.Trim();
                    //ReadStr = Regex.Replace(ReadStr, @"\s", "");
                    //成功读取非“0”的数据时，复位数据写入“0”
                    if (bFeedBack)
                    {
                        string FwStr = "";
                        for (int i = 0; i < data.Length; i++)
                        {
                            FwStr += "";
                        }
                        Write_string(location, data.Length, FwStr, "", filePath);
                    }
                }

            }
            catch (Exception ex)
            {
                bErrorResult = false;
                 Log4netRecord.ErrorLog.Error(ex + "\nFins反馈信息：" + feedback);
            }
            return bErrorResult;
        }
        /// <summary>
        /// 写入数据：批量写入单寄存器
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="location"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool Write_Ints(string filePath, int location, int[] data)
        {
            string Location = location.ToString("X").PadLeft(4, '0');
            string strdata = "";
            for (int i = 0; i < data.Length; i++) strdata += data[i].ToString() + ",";
            bool bWriteStatus = false;

            int sum = data.Length;
            string state = "成功";
            string[] split_arr = new string[sum];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = Math.Abs(data[i]);
                split_arr[i] = data[i].ToString("X").PadLeft(4, '0');
            }

            //===============================================================================================================
            //=======【】写入数据
            string feedback = "";
            string writeMsg = "";
            bWriteStatus = Write_Data(Location, split_arr, out writeMsg, out feedback);
            state = bWriteStatus ? "成功" : "失败";
            SendLog(filePath, "写地址：D" + location + " 数量：" + sum.ToString().PadLeft(2, ' ') + " 数值：" + strdata + " 状态：" + state, writeMsg, feedback);
            return bWriteStatus;
        }
        public bool Write_Int16(string filePath, string strDescribe, int location, int data)
        {
            string Location = location.ToString("X").PadLeft(4, '0');
            string strdata = data.ToString().PadRight(16, ' ');
            bool bWriteStatus = false;

            int sum = 1;
            string state = "成功";
            string[] split_arr = new string[sum];
            data = Math.Abs(data);
            split_arr[0] = data.ToString("X").PadLeft(4, '0');
            //===============================================================================================================
            //=======【】写入数据
            string feedback = "";
            string writeMsg = "";
            bWriteStatus = Write_Data(Location, split_arr, out writeMsg, out feedback);
            state = bWriteStatus ? "成功" : "失败";
            SendLog(filePath, "写地址：D" + location + " 数量：" + sum.ToString().PadLeft(2, ' ') + " 数值：" + strdata + "描述:" + strDescribe + "\t\n状态：" + state, writeMsg, feedback);
            return bWriteStatus;
        }
        public bool Write_Int32(string filePath, string strDescribe, int location, int data)
        {
            string Location = location.ToString("X").PadLeft(4, '0');
            string strdata = data.ToString().PadRight(16, ' ');
            bool bWriteStatus = false;

            int sum = 2;
            string state = "成功";
            string[] split_arr = new string[sum];
            string sendData = data.ToString("X").PadLeft(8, '0');
            split_arr[0] = sendData.Substring(4, 4);//低地址存高位
            split_arr[1] = sendData.Substring(0, 4);
            //===============================================================================================================
            //=======【】写入数据
            string feedback = "";
            string writeMsg = "";
            bWriteStatus = Write_Data(Location, split_arr, out writeMsg, out feedback);
            state = bWriteStatus ? "成功" : "失败";
            SendLog(filePath, "写地址：D" + location + " 数量：" + sum.ToString().PadLeft(2, ' ') + " 数值：" + strdata + "描述:" + strDescribe + "\t状态：" + state, writeMsg, feedback);
            return bWriteStatus;
        }
        public bool Write_Float(string filePath, int location, float data)
        {
            string Location = location.ToString("X").PadLeft(4, '0');
            string strdata = data.ToString().PadLeft(16, ' ');
            bool bWriteStatus = false;

            int sum = 2;
            string state = "成功";
            string[] split_arr = new string[sum];
            byte[] b = BitConverter.GetBytes(data);
            string sendData = "";
            foreach (int temp in b)
            {
                sendData += Convert.ToString(temp, 16).PadLeft(2, '0');
            }
            string high = sendData.Substring(2, 2) + sendData.Substring(0, 2);
            string low = sendData.Substring(6, 2) + sendData.Substring(4, 2);

            split_arr[0] = high;//低地址存高位
            split_arr[1] = low;
            //===============================================================================================================
            //=======【】写入数据
            string feedback = "";
            string writeMsg = "";
            bWriteStatus = Write_Data(Location, split_arr, out writeMsg, out feedback);
            state = bWriteStatus ? "成功" : "失败";
            SendLog(filePath, "写地址：D" + location + " 数量：" + sum.ToString().PadLeft(2, ' ') + " 数值：" + strdata + "\t状态：" + state, writeMsg, feedback);
            return bWriteStatus;
        }
        public bool Write_Double(string filePath, int location, double data)
        {
            string Location = location.ToString("X").PadLeft(4, '0');
            string strdata = data.ToString().PadLeft(16, ' ');
            bool bWriteStatus = false;

            int sum = 4;
            string state = "成功";
            string[] split_arr = new string[sum];
            byte[] b = BitConverter.GetBytes(data);
            string sendData = "";
            foreach (int temp in b)
            {
                sendData += Convert.ToString(temp, 16).PadLeft(2, '0');
            }
            string high = sendData.Substring(2, 2) + sendData.Substring(0, 2);
            string low = sendData.Substring(6, 2) + sendData.Substring(4, 2);
            string low1 = sendData.Substring(10, 2) + sendData.Substring(8, 2);
            string low2 = sendData.Substring(14, 2) + sendData.Substring(12, 2);

            split_arr[0] = high;//低地址存高位
            split_arr[1] = low;
            split_arr[2] = low1;
            split_arr[3] = low2;
            //===============================================================================================================
            //=======【】写入数据
            string feedback = "";
            string writeMsg = "";
            bWriteStatus = Write_Data(Location, split_arr, out writeMsg, out feedback);
            state = bWriteStatus ? "成功" : "失败";
            SendLog(filePath, "写地址：D" + location + " 数量：" + sum.ToString().PadLeft(2, ' ') + " 数值：" + strdata + "\t状态：" + state, writeMsg, feedback);
            return bWriteStatus;
        }

        public bool Write(string filePath, int location, int data)
        {
            string Location = location.ToString("X").PadLeft(4, '0');
            string strdata = data.ToString().PadRight(16, ' ');
            bool bWriteStatus = false;

            int sum = 2;
            string state = "成功";
            string[] split_arr = new string[sum];
            string sendData = data.ToString("X").PadLeft(8, '0');
            split_arr[0] = sendData.Substring(4, 4);//低地址存高位
            split_arr[1] = sendData.Substring(0, 4);
            //===============================================================================================================
            //=======【】写入数据
            string feedback = "";
            string writeMsg = "";
            bWriteStatus = Write_Data(Location, split_arr, out writeMsg, out feedback);
            state = bWriteStatus ? "成功" : "失败";
            SendLog(filePath, "写地址：D" + location + " 数量：" + sum.ToString().PadLeft(2, ' ') + " 数值：" + strdata + "\t状态：" + state, writeMsg, feedback);
            return bWriteStatus;
        }
        public bool Write(string filePath, int location, float data)
        {
            string Location = location.ToString("X").PadLeft(4, '0');
            string strdata = data.ToString().PadLeft(16, ' ');
            bool bWriteStatus = false;

            int sum = 2;
            string state = "成功";
            string[] split_arr = new string[sum];
            byte[] b = BitConverter.GetBytes(data);
            string sendData = "";
            foreach (int temp in b)
            {
                sendData += Convert.ToString(temp, 16).PadLeft(2, '0');
            }
            string high = sendData.Substring(2, 2) + sendData.Substring(0, 2);
            string low = sendData.Substring(6, 2) + sendData.Substring(4, 2);

            split_arr[0] = high;//低地址存高位
            split_arr[1] = low;
            //===============================================================================================================
            //=======【】写入数据
            string feedback = "";
            string writeMsg = "";
            bWriteStatus = Write_Data(Location, split_arr, out writeMsg, out feedback);
            state = bWriteStatus ? "成功" : "失败";
            SendLog(filePath, "写地址：D" + location + " 数量：" + sum.ToString().PadLeft(2, ' ') + " 数值：" + strdata + "\t状态：" + state, writeMsg, feedback);
            return bWriteStatus;
        }
        public bool Write_Bool(string filePath, int location, bool data)
        {
            string Location = location.ToString("X").PadLeft(4, '0');
            string strdata = data.ToString();
            bool bWriteStatus = false;

            int sum = 1;
            string state = "成功";
            string[] split_arr = new string[sum];
            string sendData = data ? "01" : "00";
            split_arr[0] = sendData;//低地址存高位

            //===============================================================================================================
            //=======【】写入数据
            string feedback = "";
            string writeMsg = "";
            bWriteStatus = Write_Data(Location, split_arr, out writeMsg, out feedback);
            state = bWriteStatus ? "成功" : "失败";
            SendLog(filePath, "写地址：D" + location + " 数量：" + sum.ToString().PadLeft(2, ' ') + " 数值：" + strdata + "\t状态：" + state, writeMsg, feedback);
            return bWriteStatus;
        }
        public bool Write_string(int location, int sum, string data, string strDescribe, string strLogPath)
        {
            string Location = location.ToString("X").PadLeft(4, '0');
            string Data;
            strDescribe = "(" + strDescribe + ")";
            bool bWriteStatus = false;
            Data = digitalTrans.StringToHexString(sum, data);

            string strsum = sum.ToString("X").PadLeft(4, '0');//0001 0002
            bWriteStatus = Write_Data(Location, strsum, Data);
            string strWriteStatus = bWriteStatus == true ? "OK" : "NG";
            SendLog(strLogPath, "写地址：D" + location + " 数量：" + sum.ToString().PadLeft(2, ' ') + " 数值：" + data + " 状态：" + strWriteStatus +
                strDescribe, "", "");
            return bWriteStatus;
        }
        public bool Write_Data(string location, string sum, string data)
        {
            bool iSendSuc = false;
            lock (sendTcpLock)
            {
                try
                {
                    string[] strData = new string[15];
                    strData[0] = "46494E53"; //HEADER（数据头，FINS的ACILL码）
                    // strData[1] = "0000001E";//数据长度
                    strData[2] = "00000002";//命令代码
                    strData[3] = "00000000";//错误代码
                    strData[4] = "80000200";//消 息 头
                    strData[5] = ServerIP_LastOne;//PLC的地址：0A（192.168.1.10，OA表示IP的最后一位10的16进制）
                    strData[6] = "0000";//默认
                    strData[7] = ClientIP_LastOne;//计算机的地址：89（192.168.1.137，89表示IP的最后一位137的16进制）
                    strData[8] = "0000";//0000
                    strData[9] = "0102";//命令码：0101（0101表示读，0102表示写）
                    strData[10] = "82"; //读取的区域：82（表示读取DM区）
                    strData[11] = location;//DM区（寄存器）的地址：23F0（地址9200）
                    strData[12] = "00";//默认：00
                    strData[13] = sum;//读取的数据的数量：0001
                    strData[14] = data;//写入的数据
                    int DataLength = 0; ;
                    for (int i = 2; i < strData.Length; i++)
                    {
                        DataLength += strData[i].Length;
                    }
                    strData[1] = (DataLength / 2).ToString("X").PadLeft(8, '0');
                    string message = string.Join("", strData);//合并字符数据
                    // message = "46494E530000001E000000020000000080000200"+ strServerLastJiedian + "0000"+ strClientLastJiedian + "0000010282" + location + "000002" + data;
                    string PLC_mesage;
                    write_data(message, out PLC_mesage);
                    if (PLC_mesage.Length >= 60)
                    {
                        string err;
                        err = PLC_mesage.Substring(56, 4);
                        #region 返回代码
                        switch (err.Substring(0, 2))
                        {
                            case "00":
                                switch (err.Substring(2, 2))
                                {
                                    case "00": iSendSuc = true; break;
                                    case "01": iSendSuc = false; break;
                                }
                                break;
                            default:
                                iSendSuc = false;
                                break;
                        }
                        #endregion
                    }
                }
                catch (Exception ex)
                {
                     Log4netRecord.ErrorLog.Error(ex);
                    return iSendSuc;

                }
                return iSendSuc;
            }
        }

        /// <summary>
        /// 读指对地址数据
        /// </summary>
        /// <param name="location">开始地址十六进制</param>
        /// <param name="sum">地址总数十六进制</param>
        /// <returns></returns>
        private int Read_Data(string location, int sum, out string readData, out string[] read_arr, out string writeMsg, out string feedback)
        {
            feedback = "";
            readData = "";
            writeMsg = "";
            read_arr = new string[0];
            int reulst = 0;
            lock (sendTcpLock)
            {
                try
                {
                    // O存储区域是指通过指令的操作作数可以进入的区域。
                    //它由I /O继电器区（CIO）
                    //内部辅助继电器区（WR）
                    //保持继电器区（HR）
                    //特殊辅助继电器区（AR）
                    //暂存区（TR）
                    //数据存储区（DM）
                    //定时器（TIM）
                    //计数器（CNT）状态标志、时钟脉冲、任务标志（TK）
                    //变址寄存器（IR）
                    //数据寄存器（DR）等组成，主要是用来存储输入、输出数据和中间变量，提供定时器等的系统状态和信息。
                    //CIO“BO”；WR“B1”；DM“82”；HR“B2”；TIM“89”；AR“B3”；CNT“89”

                    //=================================================================================================================================
                    //========【】整编辑发送报文
                    string[] strData = new string[9];
                    strData[0] = "46494E53"; //HEADER（数据头，FINS的ACILL码）
                    strData[1] = "0000001E";//数据长度  计算方式从“命令代码”开始且包含其本身开始计算
                    strData[2] = "00000002";//命令代码
                    strData[3] = "00000000";//错误代码
                    strData[4] = "80000200";//消 息 头

                    //【1】ServerIP_LastOne“XX”表示服务端IP最后一位（例“192.168.1.10”IP最后一位“10”的转16进制即是“OA”）;
                    //【2】"0000"默认;
                    //【3】ClientIP_LastOne“XX”客户端IP最后一位：（例“192.168.1.137”IP最后一位“137”的转16进制即是“89”）
                    strData[5] = ServerIP_LastOne + "0000" + ClientIP_LastOne;
                    strData[6] = "0000" + "0101";//【1】“0000”默认；【2】命令码：0101（0101表示读，0102表示写）
                    strData[7] = "82" + location + "00"; //【1】.读取区域“82”表示读取DM区；【2】.读取地址“location”表示地址(例：16进制地址23F0,即9200)；【3】."00"：默认 ；
                    strData[8] = sum.ToString("X").PadLeft(4, '0'); ;//读取的数据的数量：0001


                    //=================================================================================================================================
                    //========【】计算发送数据长度
                    int iDataLength = 0;
                    for (int i = 2; i < strData.Length; i++) iDataLength += strData[i].Length;
                    strData[1] = (iDataLength / 2).ToString("X").PadLeft(8, '0');

                    string message = string.Join("", strData);//合并字符数据
                    writeMsg = message;
                    bool bError = read_data(message, out feedback);
                    //=================================================================================================================================
                    //========【】
                    if (!bError)
                    {
                        return -2;
                    }
                    //=================================================================================================================================
                    //========【】
                    if (feedback.Length >= 60)
                    {
                        string err = feedback.Substring(56, 4);
                        if (feedback.Length > 60)
                        {
                            readData = feedback.Substring(60, feedback.Length - 60);

                            int imod = readData.Length % 4 == 0 ? 0 : 1;
                            int iSum = readData.Length / 4 + imod;
                            read_arr = new string[iSum];
                            for (int i = 0; i < iSum; i++)
                            {
                                int length = i * 4 + 4 <= readData.Length ? 4 : readData.Length % 4;
                                read_arr[i] = readData.Substring(i * 4, length);
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                     Log4netRecord.ErrorLog.Error(ex);
                    reulst = -1;
                }
                return reulst;
            }
        }
        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="location">地址16进制</param>
        /// <param name="data">数据16进制</param>
        /// <returns></returns>
        private bool Write_Data(string location, string[] send_arr, out string writeMsg, out string feedback)
        {
            bool iSendSuc = false;
            feedback = "";
            writeMsg = "";
            lock (sendTcpLock)
            {
                try
                {
                    int sum = send_arr.Length;
                    string strSendData = "";
                    for (int i = 0; i < sum; i++) strSendData += send_arr[i];

                    //=================================================================================================================================
                    //========【】整编辑发送报文
                    string[] strData = new string[10];
                    strData[0] = "46494E53"; //HEADER（数据头，FINS的ACILL码）
                    strData[1] = "0000001E";//数据长度  计算方式从“命令代码”开始且包含其本身开始计算
                    strData[2] = "00000002";//命令代码
                    strData[3] = "00000000";//错误代码
                    strData[4] = "80000200";//消 息 头

                    //【1】ServerIP_LastOne“XX”表示服务端IP最后一位（例“192.168.1.10”IP最后一位“10”的转16进制即是“OA”）;
                    //【2】"0000"默认;
                    //【3】ClientIP_LastOne“XX”客户端IP最后一位：（例“192.168.1.137”IP最后一位“137”的转16进制即是“89”）
                    strData[5] = ServerIP_LastOne + "0000" + ClientIP_LastOne;
                    strData[6] = "0000" + "0102";//【1】“0000”默认；【2】命令码：0101（0101表示读，0102表示写）
                    strData[7] = "82" + location + "00"; //【1】.读取区域“82”表示读取DM区；【2】.读取地址“location”表示地址(例：16进制地址23F0,即9200)；【3】."00"：默认
                    strData[8] = sum.ToString("X").PadLeft(4, '0'); ;//读取的数据的数量：0001
                    strData[9] = strSendData;//写入的数据

                    //=================================================================================================================================
                    //========【】计算发送数据长度
                    int DataLength = 0;
                    for (int i = 2; i < strData.Length; i++) DataLength += strData[i].Length;
                    strData[1] = (DataLength / 2).ToString("X").PadLeft(8, '0');

                    string message = string.Join("", strData);//合并字符数据
                    writeMsg = message;
                    write_data(message, out feedback);
                    if (feedback.Length >= 60)
                    {
                        string err = feedback.Substring(56, 4);
                        //=================================================================================================================================
                        //========【】返回代码
                        switch (err.Substring(0, 2))
                        {
                            case "00":
                                switch (err.Substring(2, 2))
                                {
                                    case "00": iSendSuc = true; break;
                                    case "40": iSendSuc = true; break;//特殊
                                    case "01": iSendSuc = false; break;
                                }
                                break;
                            default:
                                iSendSuc = false;
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    return iSendSuc;
                     Log4netRecord.ErrorLog.Debug(ex);
                }
                Thread.Sleep(2);

                return iSendSuc;
            }
        }
        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="Send_Message">发送数据</param>
        /// <param name="Return_Message">返回数据</param>
        private bool read_data(string Send_Message, out string Return_Message)
        {
            lock (sendTcpLock)
            {
                try
                {
                    Return_Message = "";
                    byte[] arrClientSendMsg = new byte[Send_Message.Length / 2];
                    for (int i = 0; i < Send_Message.Length / 2; i++)
                    {
                        arrClientSendMsg[i] = Convert.ToByte(Send_Message.Substring(i * 2, 2), 16);
                    }
                    //调用客户端套接字发送字节数组     
                    if (msock != null)
                    {
                        msock.Send(arrClientSendMsg);
                        byte[] buffer = new byte[1024 * 1024];

                        //将客户端套接字接收到的数据存入内存缓冲区，并获取长度  
                        msock.ReceiveTimeout = 100;
                        int length = msock.Receive(buffer);
                        string hexString = "";

                        if (buffer != null)
                        {
                            for (int i = 0; i < length; i++)
                            {
                                hexString += (buffer[i].ToString("X2"));
                            }
                        }
                        Return_Message = hexString;
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Return_Message = ex.ToString();
                     Log4netRecord.ErrorLog.Error(ex);
                    return false;
                }
            }
        }
        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="Write_Mesage">数据</param>
        /// <param name="Return_Message">返回数据</param>
        private void write_data(string Write_Mesage, out string Return_Message)
        {
            lock (sendTcpLock)
            {
                try
                {
                    Return_Message = "";
                    byte[] arrClientSendMsg = new byte[Write_Mesage.Length / 2];
                    for (int i = 0; i < Write_Mesage.Length / 2; i++)
                    {
                        arrClientSendMsg[i] = Convert.ToByte(Write_Mesage.Substring(i * 2, 2), 16);
                    }
                    //调用客户端套接字发送字节数组    
                    if (msock != null)
                    {
                        msock.Send(arrClientSendMsg);
                        byte[] buffer = new byte[1024 * 1024];
                        //将客户端套接字接收到的数据存入内存缓冲区，并获取长度 
                        msock.ReceiveTimeout = 100;
                        int length = msock.Receive(buffer);
                        string hexString = null;
                        if (buffer != null)
                        {
                            for (int i = 0; i < length; i++)
                            {
                                hexString += (buffer[i].ToString("X2"));
                            }
                        }
                        Return_Message = hexString;
                    }
                }
                catch (Exception ex)
                {
                    Return_Message = "";
                     Log4netRecord.ErrorLog.Error(ex);
                }
            }
        }

        public string conver_str(string str_value)
        {
            string return_value = "";
            string temp_str1 = "";
            string temp_str2 = "";
            int j = 0;
            for (int i = 0; i < str_value.Length; i++)
            {
                if (i % 2 == 0)
                {
                    j++;
                    if (j % 2 != 0)
                    {
                        //if(str_value.Substring(i, 2)!="00")
                        temp_str1 = str_value.Substring(i, 2);
                    }
                    else
                    {
                        temp_str2 = str_value.Substring(i, 2);
                        return_value += temp_str2 + temp_str1;
                    }
                }

            }

            return return_value;
        }

        #region >>>日志
        private object SendLogLock = new object();//锁存，线程互斥，保证线程完整执行
        private object ReadLogLock = new object();//锁存，线程互斥，保证线程完整执行
        public void SendLog(string filePath, string message, string writeMsg, string feedback)
        {
            lock (SendLogLock)
            {
                //filePath = SendLogPath;
                try
                {
                    Write_SendLog_ini(filePath, message + " 【WriteMsg：" + writeMsg + " Feedback：" + feedback + " 】", "");
                    //System.IO.StreamWriter writer = new System.IO.StreamWriter(filePath, true, Encoding.UTF8);
                    ////writer.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " " + message + " 【反馈信息：" + feedback + " 】");
                    //writer.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " " + message + " 【WriteMsg：" + writeMsg + " Feedback：" + feedback + " 】");
                    //writer.Flush();
                    //writer.Close();
                    //writer.Dispose();
                }
                catch (Exception ex)
                {                
                     Log4netRecord.ErrorLog.Fatal(ex + "\n路径：" + filePath + "\n信息：\n" + message + "  " + feedback);
                }
            }
        }
        public void ReadLog(string filePath, string message, string writeMsg, string feedback)
        {
            lock (ReadLogLock)
            {
                //filePath = ReadLogPath;
                try
                {
                    Write_ReadLog_ini(filePath, message + " 【Feedback：" + feedback + " 】", "");
                    //System.IO.StreamWriter writer = new System.IO.StreamWriter(filePath, true, Encoding.UTF8);
                    //writer.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " " + message + " 【Feedback：" + feedback + " 】");
                    ////writer.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " " + message + " 【WriteMsg：" + writeMsg + " Feedback：" + feedback + " 】");
                    //writer.Flush();
                    //writer.Close();
                    //writer.Dispose();
                }
                catch (Exception ex)
                {                   
                     Log4netRecord.ErrorLog.Fatal(ex + "\n路径：" + filePath + "\n信息：\n" + message + "  " + feedback);
                }
            }
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using Frame_Core;
using LogTool;

namespace Frame_Core
{
    public class TCP : FileOperationINI
    {

        public object sendTcpLock = new object();
        public TcpClient client;
        Byte[] buffer = new byte[1024];//
        NetworkStream ReadStream;
        public bool bool_Connect_Plc = false;
        string tmp_Str = null;
        string tmp_Str1 = null;
        public string str_PLC_num = "";
        /// <summary>
        /// 判断是否为打标机，打标机延时间隔不能太长
        /// </summary>
        public bool Is_Dbj { set; get; }//判断是否为打标机，打标机延时间隔不能太长
        /// <summary>
        /// 
        /// </summary>
        /// <param name="PLC_IP">PLC IP地址</param>
        /// <param name="port">port：端口号</param>
        /// <returns></returns>
        public bool Connect_Plc(string PLC_IP, int port)
        {

            try
            {
                //初始化对象
                client = new TcpClient(PLC_IP, port);
                if (client.Connected)
                {
                    bool_Connect_Plc = true;
                    ReadStream = client.GetStream();
                }
                else
                {
                    bool_Connect_Plc = false;
                }
            }
            catch (Exception ex)
            {
                bool_Connect_Plc = false;
                log4net.ILog LogDebug = log4net.LogManager.GetLogger("Debug");
                log4net.Config.XmlConfigurator.Configure();
                LogDebug.Error(ex);
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
            return bool_Connect_Plc;
        }

        #region 辊压通信

        #region 功能码
        //软件发送


        public string str_mojuan = "MJXJ0000";//清空上一卷膜卷记录数据用
        public string str_run = "PZY00000";//软件使能给设备，使设备双重防呆
        public string str_stop = "PZN00000";//用于取消软件使能
        public string str_JP_QD = "JPSFSNQD";//使能启动才能让纠偏伺服动作
        public string str_JP_TZ = "JPSFSNTZ";//关闭分切纠偏使能
        public string str_BML_dowm = "BMQYLDK0";//编码压轮会相应的执行动作
        public string str_BML_up = "BMQYLGB0";//编码压轮会相应的执行动作	
        public string str_SF_FW = "SFFW0000";//让打标机及纠偏伺服复位（点击报警复位会一起发过来）		
        public string str_BJ_FW = "BJFW0000";//所有PLC的软报警都复位一次
        public string str_JP_HZ = "JBSFHZ00";//分切纠偏回中（回原点）
        public string str_JP_ZD = "JPZDDK00";//纠偏自动（在设备运行时软件检测到往内或外纠）
        public string str_JP_SD = "JPZDGB00";//自动纠偏的信号给屏蔽了，只接收手动给的纠偏信号
        public string str_Heart = "XSTXCLXH";//用于判断软件是否与PLC断连
        public string str_JC_YC = "KDJCYCTJ";//软件检测到异常则使设备报警停机
        public string str_JP_Speed = "JBFYSD00";//纠偏电机动作的速度，数据量为1，数据已浮点型写到数据区
        public string str_BML_D = "BMZJSD00";//编码器所在位置的辊轮直径，用于PLC内部计算，数据量为1，数据已浮点型写到数据区
        public string str_BML_MC = "BMMCSD00";//编码器的分辨率设定，用于PLC内部计算，数据量为1，数据已浮点型写到数据区
        public string str_JP_AUTO = "JPYDL000";//分切纠偏在自动纠偏下向移动，数据量为1，数据已浮点型写到数据区，内移为负值
        public string str_JP_Hand = "SDYDL000";//分切纠偏在手动纠偏下向移动，数据量为1，数据已浮点型写到数据区，内移为负值
        public string str_JP_SGJD = "SGJDSD00";//纠偏电机所带动的丝杆的精度(即伺服转一圈丝杆所走的距离)，数据量为1，数据已浮点型写到数据区	
        public string str_MZ_PZ = "PZJLJG00";//用于面阵拍照用，拍完一次后得间隔该参数的大小
        public string str_JP_PB = "JPPBSD00";//设备启动后在这段距离内不执行纠偏动作,数据量为1，数据已浮点型写到数据区

        //接收
        //JPBHSFAM 分切纠编伺服故障报警
        //BMCYCAM0 编码器异常报警
        //JBCWYAM0 编码器压轮未压到位(磁性开关)
        //NCYDJXAM 分切纠偏内侧极限位置报警
        //WCYDJXAM 分切纠偏外侧极限位置报警
        //FQJPYSN0 分切纠偏伺服使能反馈
        //RJTXDSTJ 软件心跳异常报警
        //ZJSD0000 主机速度
        //BMZJSD00 反馈编码轮直径设定   反馈编码轮直径设定 反馈编码轮直径设定
        //BMMCSD00 反馈编码器脉冲设定   反馈编码器脉冲设定 反馈编码器脉冲设定
        #endregion
        public bool TCP_Send(string SendmStr, float SendData, string filePath)
        {
            try
            {
                F = Send(SendmStr, SendData);
                if (F < 0)
                {
                    SendPLC_iniLog(filePath, "向PLC发送据失败：", SendmStr + " " + SendData);
                    bool_Connect_Plc = false;
                }
                else
                {
                    SendPLC_iniLog(filePath, "向PLC发送成功：", SendmStr + " " + SendData);
                    bool_Connect_Plc = true;

                }

                //=========================================================================================================
                //======【】
                Thread.Sleep(50);//120
            }
            catch (Exception ex)
            {
                SendPLC_iniLog(filePath, "向PLC发送失败：", ex.Message + ex.StackTrace);
                bool_Connect_Plc = false;
            }
            return bool_Connect_Plc;
        }
        public bool TCP_Send(string SendmStr, double SendData, string filePath)
        {
            try
            {
                F = Send(SendmStr, float.Parse(SendData.ToString()));
                if (F < 0)
                {
                    SendPLC_iniLog(filePath, "向PLC发送据失败：", SendmStr + " " + SendData);
                    bool_Connect_Plc = false;
                }
                else
                {
                    SendPLC_iniLog(filePath, "向PLC发送成功：", SendmStr + " " + SendData);
                    bool_Connect_Plc = true;

                }

                //=========================================================================================================
                //======【】
                Thread.Sleep(50);//120
            }
            catch (Exception ex)
            {
                SendPLC_iniLog(filePath, "向PLC发送失败：", ex.Message + ex.StackTrace);
                bool_Connect_Plc = false;
            }
            return bool_Connect_Plc;
        }
        public int Send(string SendmStr, float SendData)
        {
            int iSendSuc = -1;
            try
            {
                lock (sendTcpLock)
                {
                    Socket socket = client.Client;
                    if (null == socket)
                    {
                        return -2;
                    }

                    if (null == SendmStr)
                    {
                        return -3;
                    }
                    Byte[] sendStrBytes = Encoding.UTF8.GetBytes(SendmStr);
                    byte[] SendDataBytes = BitConverter.GetBytes(SendData);
                    //Array.Reverse(SendDataBytes); // 反转

                    byte[] sendBytes = new byte[20];
                    sendStrBytes.CopyTo(sendBytes, 0);
                    SendDataBytes.CopyTo(sendBytes, sendStrBytes.Length);

                    int iSend = socket.Send(sendBytes);
                    iSendSuc = (iSend == sendBytes.Length) ? 0 : -4;
                    if (Is_Dbj)//打标机延时间隔不能太长避免打标位置偏差过大
                    {
                        Thread.Sleep(20);
                    }
                    else
                    {
                        Thread.Sleep(20);
                    }
                    //测试是不是两个发送线程“同时”发送导致发送语句太接近PLC断开
                    Thread.Sleep(80);
                }
            }
            catch
            {
                iSendSuc = -5;
            }
            return iSendSuc;

        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Sendmsg"></param>
        /// <returns></returns>
        public int Send(string Sendmsg)
        {
            int iSendSuc = -1;
            try
            {
                lock (sendTcpLock)
                {
                    Socket socket = client.Client;
                    if (null == socket)
                    {
                        return -2;
                    }

                    if (null == Sendmsg)
                    {
                        return -3;
                    }

                    Byte[] sendBytes = Encoding.UTF8.GetBytes(Sendmsg);
                    int iSend = socket.Send(sendBytes);
                    iSendSuc = (iSend == sendBytes.Length) ? 0 : -4;
                    if (Is_Dbj)//打标机延时间隔不能太长避免打标位置偏差过大
                    {
                        Thread.Sleep(20);
                    }
                    else
                    {
                        Thread.Sleep(20);
                    }
                    //测试是不是两个发送线程“同时”发送导致发送语句太接近PLC断开
                    Thread.Sleep(80);
                }
            }
            catch
            {
                iSendSuc = -5;
            }
            return iSendSuc;

        }
        int F = 0;
        /// <summary>
        /// 发送PLC信息
        /// </summary>
        /// <param name="Send_PLC_Data">发送给PLC的内容</param>
        /// <param name="filePath">保存数据文档路径</param>
        /// <returns></returns>
        public bool TCP_Send(string Send_PLC_Data, string filePath)
        {
            try
            {
                F = Send(Send_PLC_Data);
                if (F < 0)
                {
                    SendPLC_iniLog(filePath, "向PLC发送据失败：", Send_PLC_Data);
                    bool_Connect_Plc = false;
                }
                else
                {
                    SendPLC_iniLog(filePath, "向PLC发送成功：", Send_PLC_Data);
                    bool_Connect_Plc = true;
                }


                Thread.Sleep(50);//120
            }
            catch (Exception ex)
            {

                SendPLC_iniLog(filePath, "向PLC发送失败：", ex.Message + ex.StackTrace);
                bool_Connect_Plc = false;
            }
            return bool_Connect_Plc;
        }
        //public bool TCP_Send(bool isLeft, string Send_PLC_Data, string filePath,out string StrLog_out)
        //{
        //    StrLog_out = "";
        //    try
        //    {
        //        string sTime = "    [STime：" + DateTime.Now.ToString("HH:mm:ss:fff");
        //        F = Send(Send_PLC_Data);
        //        if (F < 0)
        //        {
        //            StrLog_out = "向PLC发送据失败：" + Send_PLC_Data + sTime;

        //            if (isLeft)
        //                LogName.SendPLCLog_N(filePath, "向PLC发送据失败：", Send_PLC_Data + sTime);
        //            else
        //                LogName.SendPLCLog_W(filePath, "向PLC发送据失败：", Send_PLC_Data + sTime);
        //            bool_Connect_Plc = false;
        //        }
        //        else
        //        {
        //            sTime += "-ETime：" + DateTime.Now.ToString("HH:mm:ss:fff") + "]";
        //            StrLog_out = "向PLC发送成功：" + Send_PLC_Data + sTime;

        //            if (isLeft)
        //                LogName.SendPLCLog_N(filePath, "向PLC发送成功：", Send_PLC_Data + sTime);
        //            else
        //                LogName.SendPLCLog_W(filePath, "向PLC发送成功：", Send_PLC_Data + sTime);
        //            bool_Connect_Plc = true;
        //            Form1.bo_DBJClose = false;
        //        }

        //        //=========================================================================================================
        //        //======【】
        //        if (Send_PLC_Data.Contains("NG"))
        //        {
        //            ;
        //        }
        //        else
        //        {
        //            Thread.Sleep(50);//50
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        StrLog_out = "向PLC发送失败：" + Send_PLC_Data + "  Error：" + ex.Message + ex.StackTrace;
        //        SendPLC_iniLog(filePath, "向PLC发送失败：", ex.Message + ex.StackTrace);
        //        bool_Connect_Plc = false;
        //    }
        //    return bool_Connect_Plc;
        //}
        /// <summary>
        /// 软件CCD接收打标机信息
        /// </summary>
        /// <param name="filePath">路径</param>
        /// <param name="RedPlcData_Bool">接收PLC报警信息标志</param>
        /// <param name="RedPlcData_str">接收到的数据内容</param>
        /// <returns></returns>
        public bool RedPlcData(string filePath, out bool RedPlcData_Bool, out string RedPlcData_str)//接收PLC报警信息
        {

            RedPlcData_Bool = false;
            RedPlcData_str = "";
            string PLC_num = "", tempStr = "";
            int i = 0;
            bool ReadfloatStr = false;
            try
            {
                //====================================================================================================================
                //========【】
                if (client.Available > 0)
                {
                    i = ReadStream.Read(buffer, 0, buffer.Length);
                }
                else
                {

                }
                //====================================================================================================================
                //========【】
                if (i > 0)
                {
                    CvStrFloat(buffer, 8, out ReadfloatStr, out tempStr);
                    if (ReadfloatStr)
                    {
                        tmp_Str1 = tempStr;
                    }
                    else
                    {
                        tmp_Str = Encoding.ASCII.GetString(buffer, 0, i);
                        tmp_Str = tmp_Str.Replace(" ", "");
                        tmp_Str1 += tmp_Str;
                    }

                    Array.Clear(buffer, 0, buffer.Length);

                    if (tmp_Str1.Length > 3)
                    {
                        //====================================================================================================================
                        //========【】
                        RedPLCLog(filePath, "接收PLC数据：", tmp_Str1);
                        getValidCode(tmp_Str1, out RedPlcData_str, out PLC_num);
                        //====================================================================================================================
                        //========【】
                        if (/*UIRecipe.CCDconfig.Machine_type == 0*/false)
                        {
                            if (PLC_num != "" && PLC_num.Length < 10)
                                str_PLC_num = Convert.ToInt64(PLC_num).ToString();
                        }
                        else
                        {
                            if (PLC_num != "" && PLC_num.Length <= 20)
                            {
                                str_PLC_num = Convert.ToInt64(PLC_num).ToString();
                                str_PLC_num = str_PLC_num.Substring(0, 8);
                            }
                        }
                        //====================================================================================================================
                        //========【】
                        RedPlcData_str = RedPlcData_str.Trim();
                        RedPlcData_Bool = true;
                        tmp_Str1 = null;

                    }
                    bool_Connect_Plc = true;
                }

            }
            catch (Exception ex)
            {
                //System.Windows.Forms.MessageBox.Show("PLC网络中断~！请检测网络重新连接PLC");
                LogDebug.Error(ex);
                RedPLCLog(filePath, "接收PLC数据：", ex.Message + ex.StackTrace);
                bool_Connect_Plc = false;
            }
            return bool_Connect_Plc;
        }
        /// <summary>
        /// 解析string + float的数据编码，字节数组是小端编码
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="RedPlcData_Bool"></param>
        /// <param name="RedPlcData_str"></param>
        /// <returns></returns>

        public static log4net.ILog LogDebug = log4net.LogManager.GetLogger("Debug");//记录日志 
        public static readonly object PLCLog = new object();
        public static readonly object PLCReadLog_Lock = new object();
        /// <summary>
        /// 软件CCD接收打标机信息记录log
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="message"></param>
        /// <param name="track"></param>
        public void RedPLCLog(string filePath, string message, string track)
        {
            lock (PLCReadLog_Lock)
            {
                //string filePath = Application.StartupPath + "\\record\\RedPLCLog.ini";
                try
                {
                    Write_ReadLog_ini(filePath, message + "\t" + track, "");
                    //System.IO.StreamWriter writer = new System.IO.StreamWriter(filePath, true, Encoding.UTF8);
                    //writer.WriteLine(DateTime.Now.ToString("yyyy.MM.dd HH.mm.ss.fff") + "   " + message + "\t" + track);

                    //writer.Flush();
                    //writer.Close();
                    //writer.Dispose();
                }
                catch (Exception ex)
                {
                     Log4netRecord.ErrorLog.Fatal(ex + "\n路径：" + filePath + "\n信息：\n" + message + "  " + track);
                }

            }
        }
        /// <summary>
        /// 软件CCD发送打标机信息记录log
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="message"></param>
        /// <param name="track"></param>
        public void SendPLC_iniLog(string filePath, string message, string track)
        {
            lock (PLCLog)
            {
                try
                {
                    Write_SendLog_ini(filePath, message + "\t" + track, "");
                    ////string filePath = Application.StartupPath + "\\record\\PLC通讯收集.ini";
                    //System.IO.StreamWriter writer = new System.IO.StreamWriter(filePath, true, Encoding.UTF8);
                    //writer.WriteLine(DateTime.Now.ToString("yyyy.MM.dd HH.mm.ss.fff") + "   " + message + "\t" + track);

                    //writer.Flush();
                    //writer.Close();
                    //writer.Dispose();

                }
                catch (Exception ex)
                {
                     Log4netRecord.ErrorLog.Fatal(ex + "\n路径：" + filePath + "\n信息：\n" + message + "  " + track);
                }

            }
        }
        /// <summary>
        /// 获取打标机连接状态，是否连接上
        /// </summary>
        /// <returns></returns>
        public bool Get_PLcstatue()
        {
            try
            {

                if (client.Connected)
                {
                    //bool_Connect_Plc = true;
                    return true;
                }
                else
                {
                    bool_Connect_Plc = false;
                    return false;

                }

            }
            catch (Exception)
            {

                return false;
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Shutdown_PLC()
        {
            try
            {
                if (bool_Connect_Plc)
                {
                    client.Close();
                    bool_Connect_Plc = false;
                }

            }
            catch (Exception)
            {

                return false;
            }
            return true;
        }


        /// <summary>
        /// 筛选有效字符，lbl，2022年3月30日
        /// </summary>
        /// <param name="str"></param>
        /// <param name="outStr"></param>
        /// <param name="plcNum"></param>
        public void getValidCode(string str, out string outStr, out string plcNum)
        {
            outStr = "";
            plcNum = "";
            try
            {
                string tmp_Str;
                tmp_Str = str.Replace(" ", "");

                if (tmp_Str1.Length > 3)
                {
                    //====================================================================================================================
                    //========【】
                    for (int j = 0; j < tmp_Str1.Length; j++)
                    {
                        char ch = tmp_Str1[j];
                        if ((ch >= 48 && ch <= 57) || (ch >= 65 && ch <= 90) || ((ch >= 97 && ch <= 122)) || ch == 46 || ch == 44 || ch == 124)//44[,]
                        {
                            outStr += ch;
                        }

                        if (Char.IsLower(ch) || Char.IsUpper(ch) || Char.IsNumber(ch))
                        {
                            //RedPlcData_str += ch;
                            if (Char.IsNumber(ch) && tmp_Str1.Substring(0, 2) == "JL")
                            {
                                plcNum += ch;
                            }
                        }

                    }


                }

            }
            catch (Exception ex)
            {
                outStr = "";
                plcNum = "";
            }
        }

        public void CvStrFloat(byte[] buffer, int strLen, out bool RedPlcData_Bool, out string RedPlcData_str)
        {
            RedPlcData_Bool = false;
            RedPlcData_str = "";
            string tempStr = "", tempStrF = "";
            int str_Float = 12, floatLen = 4;
            int counts = 0;
            try
            {
                //====================================================================================================================
                //========【】
                tempStr = Encoding.ASCII.GetString(buffer, 0, strLen);

                for (int j = 0; j < 8; j++)
                {
                    //索引起始为0，检查buffer第12到20位字节ASCII码是否皆为0，为0则表示功能码+float
                    if (buffer[str_Float + j] != 0)
                        counts++;
                }

                if (true/*counts == 0*/)  //仅4个byte表示一个float值
                {
                    float[] floats = new float[floatLen / 4];
                    Buffer.BlockCopy(buffer, strLen, floats, 0, floatLen);
                    for (int k = 0; k < floats.Length; k++)
                    {
                        tempStrF += floats[k].ToString("0.000");
                    }
                    RedPlcData_Bool = true;
                    RedPlcData_str = tempStr/* + tempStrF*/;
                }

            }
            catch (Exception ex)
            {
                LogDebug.Error(ex);
            }
        }
    }
}

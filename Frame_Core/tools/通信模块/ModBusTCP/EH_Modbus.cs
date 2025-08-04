using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modbus.Data;
using Modbus.Device;
using Modbus.Utility;
using Modbus.IO;
using System.Net;
using System.Net.Sockets;
using Frame_Core;
using LogTool;

namespace Frame_Core
{
    public class EH_Modbus : FileOperationINI
    {
        TcpClient tcpclient = null;
        ModbusIpMaster master;
        public bool Connected = false;

        public static bool HighLow = true;//false为ABCD，true为CDAB

        //连接modbus
        public bool MODBUS_connect(string ip, int port)
        {
            try
            {
                tcpclient = new TcpClient();
                tcpclient.Connect(IPAddress.Parse(ip), port);
                master = ModbusIpMaster.CreateIp(tcpclient);
                if (tcpclient.Connected)
                {
                    Connected = true;
                }
                else
                {
                    Connected = false;
                }
            }
            catch (Exception ex)
            {
                Connected = false;
                 Log4netRecord.ErrorLog.Fatal(ex.Message + ex.StackTrace);
            }
            return Connected;
        }
        //断开连接modbus
        public void MODBUS_disconnect()
        {
            try
            {
                if (tcpclient != null)
                {
                    tcpclient.Close();
                }
                Connected = false;
            }
            catch (Exception ex)
            {
                Connected = false;
                 Log4netRecord.ErrorLog.Fatal(ex.Message + ex.StackTrace);
            }

        }
        //读取int16型数据
        private short Read_Int16(string location)
        {
            ushort[] mes = master.ReadHoldingRegisters(Convert.ToUInt16(location), Convert.ToUInt16(1));
            short data = GetShort(mes, 0);
            return data;

        }
        public short Read_int16(int location, string strDescribe, string strLogPath, Boolean isSaveLog = true)
        {
            short iReadData = 0;
            try
            {
                iReadData = Read_Int16(location.ToString());
                if (iReadData > 0 && isSaveLog)//xjh20210515 大于0才记录寄存器数据
                    RedPLCLog(strLogPath, "读取寄存器D" + location, "数值:" + iReadData + "读取状态:成功----" + strDescribe);
                return iReadData;
            }
            catch (Exception ex)
            {
                RedPLCLog(strLogPath, "读取PLC失败", "地址：" + location + "(" + strDescribe + ")");
                 Log4netRecord.ErrorLog.Error(ex.Message + ex.StackTrace);
                return -1;
            }
        }

        private int Read_Int32(string location)
        {
            ushort[] mes = master.ReadHoldingRegisters(Convert.ToUInt16(location), Convert.ToUInt16(2));
            int data = GetInt(mes, 0);
            return data;

        }
        public int Read_int32(int location, string strDescribe, string strLogPath, Boolean isSaveLog = true)
        {
            int iReadData = 0;
            try
            {
                iReadData = Read_Int32(location.ToString());
                if (iReadData > 0 && isSaveLog)
                    RedPLCLog(strLogPath, "读取寄存器D" + location, "数值:" + iReadData + "读取状态:成功----" + strDescribe);
                return iReadData;
            }
            catch (Exception ex)
            {
                RedPLCLog(strLogPath, "读取PLC失败", "地址：" + location + "(" + strDescribe + ")");
                 Log4netRecord.ErrorLog.Error(ex.Message + ex.StackTrace);
                return -1;
            }
        }

        //读取浮点型float数据
        private float Read_Float(string location)
        {
            ushort[] mes = master.ReadHoldingRegisters(Convert.ToUInt16(location), Convert.ToUInt16(2));
            float data = GetReal(mes, 0);
            return data;
        }

        public float Read_float(int location, string strDescribe, string strLogPath)
        {
            float iReadData = 0;
            try
            {
                iReadData = Read_Float(location.ToString());
                if (iReadData > 0)
                    RedPLCLog(strLogPath, "读取寄存器D" + location, "数值:" + iReadData + "读取状态:成功----" + strDescribe);

                return iReadData;
            }
            catch (Exception ex)
            {
                RedPLCLog(strLogPath, "读取PLC失败", "地址：" + location + "(" + strDescribe + ")");
                 Log4netRecord.ErrorLog.Error(ex.Message + ex.StackTrace);
                return -1;
            }
        }

        public double Read_double(int location, string strDescribe, string strLogPath, Boolean isSaveLog = true)
        {
            double iReadData = 0;
            try
            {
                iReadData = Read_Double(location.ToString());
                if (iReadData > 0 && isSaveLog)//xjh20210515 大于0才记录寄存器数据
                    RedPLCLog(strLogPath, "读取寄存器D" + location, "数值:" + iReadData + "读取状态:成功----" + strDescribe);

                return iReadData;
            }
            catch (Exception ex)
            {
                RedPLCLog(strLogPath, "读取PLC失败", "地址：" + location + "(" + strDescribe + ")");
                 Log4netRecord.ErrorLog.Error(ex.Message + ex.StackTrace);
                return -1;
            }
        }

        //读取double数据
        private double Read_Double(string location)
        {
            ushort[] mes = master.ReadHoldingRegisters(Convert.ToUInt16(location), Convert.ToUInt16(4));
            double data = GetDouble(mes, 0);
            return data;

        }

        public double[] Read_doubles(int location, int sum, string strDescribe, string strLogPath)
        {
            double[] iReadData = new double[0];
            try
            {
                iReadData = Read_Doubles(location.ToString(), sum);
                string readdata = string.Join(",", iReadData);//合并字符数据
                double data = 0;
                if (iReadData.Length > 0) data = iReadData.Max();
                if (data > 0)
                    RedPLCLog(strLogPath, "读取寄存器D" + location, "数量:" + sum + "数值:" + readdata + "读取状态:成功----" + strDescribe);

                return iReadData;
            }
            catch (Exception ex)
            {
                RedPLCLog(strLogPath, "读取PLC失败", "地址：" + location + "数量:" + sum + "(" + strDescribe + ")");
                 Log4netRecord.ErrorLog.Error(ex.Message + ex.StackTrace);
                return iReadData;
            }
        }

        //读取doubles数据
        private double[] Read_Doubles(string location, int sum)
        {
            ushort[] mes = master.ReadHoldingRegisters(Convert.ToUInt16(location), Convert.ToUInt16(sum * 4));//读取的长度需在1-125之间
            double[] data = GetDoubles(mes, 0);
            return data;

        }

        //读取字符串数据
        private string Read_String(string location, int sum)
        {
            ushort[] mes = master.ReadHoldingRegisters(Convert.ToUInt16(location), Convert.ToUInt16(sum));
            string str = GetString(mes, 0, sum);
            return str;

        }
        public string Read_string(int location, int sum, string strDescribe, string strLogPath)
        {
            string DM_value = "";
            try
            {
                DM_value = Read_String(location.ToString(), sum);
                if (DM_value != "")//xjh20210515 大于0才记录寄存器数据
                    RedPLCLog(strLogPath, "读取PLC成功", "地址：" + location + "(" + strDescribe + ")" + "    数值： " + DM_value);

                return DM_value;
            }
            catch (Exception ex)
            {
                RedPLCLog(strLogPath, "读取PLC失败", "地址：" + location + "(" + strDescribe + ")");
                 Log4netRecord.ErrorLog.Error(ex.Message + ex.StackTrace);
                return "-1";
            }
        }
        //写入int16数据
        private void Write_Int16(string location, short data)
        {
            ushort[] buff = new ushort[1];
           
            SetShort(buff, 0, data);
           
            master.WriteMultipleRegisters(Convert.ToUInt16(location), buff);
        }
        public bool Write_int16(int location, short data, string strDescribe, string strLogPath)
        {
            bool IsSuccess = false;
            try
            {
                Write_Int16(location.ToString(), (short)data);
                IsSuccess = true;
                SendPLC_iniLog(strLogPath, "写入寄存器地址D" + location,
                    "数值:" + data +
                    "写入状态:" + IsSuccess +
                    strDescribe);
            }
            catch (Exception ex)
            {
                IsSuccess = false;
                SendPLC_iniLog(strLogPath, "写入寄存器地址D" + location,
                   "数值:" + data +
                   "写入状态:" + IsSuccess +
                   strDescribe);
                 Log4netRecord.ErrorLog.Fatal(ex.Message + ex.StackTrace);
            }
            return IsSuccess;
        }
        //写入int32数据
        private void Write_Int32(string location, int data)
        {
            ushort[] buff = new ushort[2];
            SetInt(buff, 0, data);
            if (HighLow)
            {
                Array.Reverse(buff);//CDAB才要转换，如果是ABCD可屏蔽
            }
            master.WriteMultipleRegisters(Convert.ToUInt16(location), buff);
        }
        public bool Write_int32(int location, int data, string strDescribe, string strLogPath)
        {
            bool IsSuccess = false;
            try
            {
                Write_Int32(location.ToString(), data);
                IsSuccess = true;
                SendPLC_iniLog(strLogPath, "写入寄存器地址D" + location,
                    "数值:" + data +
                    "写入状态:" + IsSuccess +
                    strDescribe);
            }
            catch (Exception ex)
            {
                IsSuccess = false;
                SendPLC_iniLog(strLogPath, "写入寄存器地址D" + location,
                   "数值:" + data +
                   "写入状态:" + IsSuccess +
                   strDescribe);
                 Log4netRecord.ErrorLog.Fatal(ex.Message + ex.StackTrace);
            }
            return IsSuccess;
        }

        public object sendTcpLock = new object();//锁存，线程互斥，保证线程完整执行
        public object readTcpLock = new object();//锁存，线程互斥，保证线程完整执行
        public void SendPLC_iniLog(string filePath, string message, string track)
        {
            lock (sendTcpLock)
            {
                try
                {
                    Write_SendLog_ini(filePath, message, track);

                    //System.IO.StreamWriter writer = new System.IO.StreamWriter(filePath, true, Encoding.UTF8);
                    //writer.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "   " + message + "\t" + track);
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
        public void RedPLCLog(string filePath, string message, string track)
        {
            lock (readTcpLock)
            {
                try
                {
                    Write_ReadLog_ini(filePath, message, track);
                    //System.IO.StreamWriter writer = new System.IO.StreamWriter(filePath, true, Encoding.UTF8);
                    //writer.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + "   " + message + "\t" + track);

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
        //写入float数据
        private void Write_Float(string location, float data)
        {
            ushort[] buff = new ushort[2];
            SetReal(buff, 0, data);
            if (HighLow)
            {
                Array.Reverse(buff);//CDAB才要转换，如果是ABCD可屏蔽
            }
            master.WriteMultipleRegisters(Convert.ToUInt16(location), buff);
        }
        public bool Write_float(int location, float data, string strDescribe, string strLogPath)
        {
            bool IsSuccess = false;
            try
            {
                Write_Float(location.ToString(), data);
                IsSuccess = true;
                SendPLC_iniLog(strLogPath, "写入寄存器地址D" + location,
                    "数值:" + data +
                    "写入状态:" + IsSuccess +
                    strDescribe);
            }
            catch (Exception ex)
            {
                IsSuccess = false;
                SendPLC_iniLog(strLogPath, "写入寄存器地址D" + location,
                   "数值:" + data +
                   "写入状态:" + IsSuccess +
                   strDescribe);
                 Log4netRecord.ErrorLog.Fatal(ex.Message + ex.StackTrace);
            }
            return IsSuccess;

        }

        //写入double数据
        private void Write_Double(string location, double data)
        {
            ushort[] buff = new ushort[4];
            SetDouble(buff, 0, data);
            if (HighLow)
            {
                Array.Reverse(buff);//CDAB才要转换，如果是ABCD可屏蔽
            }
            master.WriteMultipleRegisters(Convert.ToUInt16(location), buff);
        }
        public bool Write_double(int location, double data, string strDescribe, string strLogPath)
        {
            bool IsSuccess = false;
            try
            {
                Write_Double(location.ToString(), data);
                IsSuccess = true;
                SendPLC_iniLog(strLogPath, "写入寄存器地址D" + location,
                    "数值:" + data +
                    "写入状态:" + IsSuccess +
                    strDescribe);
            }
            catch (Exception ex)
            {
                IsSuccess = false;
                SendPLC_iniLog(strLogPath, "写入寄存器地址D" + location,
                   "数值:" + data +
                   "写入状态:" + IsSuccess +
                   strDescribe);
                 Log4netRecord.ErrorLog.Fatal(ex.Message + ex.StackTrace);
            }
            return IsSuccess;

        }

        //写入doubles数据
        private void Write_Doubles(string location, double[] data, int sum)
        {
            ushort[] buff = new ushort[sum * 4];
            SetDoubles(buff, 0, data);
            if (HighLow)
            {
                Array.Reverse(buff);//CDAB才要转换，如果是ABCD可屏蔽
            }
            master.WriteMultipleRegisters(Convert.ToUInt16(location), buff);//写入的长度需在1-123之间
        }
        public bool Write_doubles(int location, int sum, double[] data, string strDescribe, string strLogPath)
        {
            bool IsSuccess = false;
            string readdata = "";
            try
            {
                Write_Doubles(location.ToString(), data, sum);
                readdata = string.Join(",", data);//合并字符数据
                IsSuccess = true;
                SendPLC_iniLog(strLogPath, "写入寄存器地址D" + location +
                    "寄存器数量:" + sum,
                    "数值:" + readdata +
                    "写入状态:" + IsSuccess +
                    strDescribe);
            }
            catch (Exception ex)
            {
                IsSuccess = false;
                SendPLC_iniLog(strLogPath, "写入寄存器地址D" + location +
                   "寄存器数量:" + sum,
                   "数值:" + readdata +
                   "写入状态:" + IsSuccess +
                   strDescribe);
                 Log4netRecord.ErrorLog.Fatal(ex.Message + ex.StackTrace);
            }
            return IsSuccess;

        }
        //写入字符串数据
        public void Write_String(string location, string data, int sum)
        {
            ushort[] buff = new ushort[sum];
            SetString(buff, 0, data);
           
            master.WriteMultipleRegisters(Convert.ToUInt16(location), buff);
        }
        public static float GetFloat(ushort P1, ushort P2)
        {
            int intSign, intSignRest, intExponent, intExponentRest;
            float faResult, faDigit;
            intSign = P1 / 32768;
            intSignRest = P1 % 32768;
            intExponent = intSignRest / 128;
            intExponentRest = intSignRest % 128;
            faDigit = (float)(intExponentRest * 65536 + P2) / 8388608;
            faResult = (float)Math.Pow(-1, intSign) * (float)Math.Pow(2, intExponent - 127) * (faDigit + 1);
            return faResult;
        }
        /// <summary>
        /// 赋值string
        /// </summary>
        /// <param name="src"></param>
        /// <param name="start"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static void SetString(ushort[] src, int start, string value)
        {
            byte[] bytesTemp = Encoding.UTF8.GetBytes(value);
            ushort[] dest = Bytes2Ushorts(bytesTemp, true);
            dest.CopyTo(src, start);
        }
        /// <summary>
        /// 获取string
        /// </summary>
        /// <param name="src"></param>
        /// <param name="start"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string GetString(ushort[] src, int start, int len)
        {
            ushort[] temp = new ushort[len];
            for (int i = 0; i < len; i++)
            {
                temp[i] = src[i + start];
            }
          
            byte[] bytesTemp = Ushorts2Bytes(temp, true);
            string res = Encoding.UTF8.GetString(bytesTemp).Trim(new char[] { '\0' });
            return res;
        }

        /// <summary>
        /// 赋值Real类型数据
        /// </summary>
        /// <param name="src"></param>
        /// <param name="start"></param>
        /// <param name="value"></param>
        public static void SetReal(ushort[] src, int start, float value)
        {
            byte[] bytes = BitConverter.GetBytes(value);

            bytes = Reverse(bytes);

            ushort[] dest = Bytes2Ushorts(bytes, true);
            dest.CopyTo(src, start);
        }
        /// <summary>
        /// 赋值double类型数据
        /// </summary>
        /// <param name="src"></param>
        /// <param name="start"></param>
        /// <param name="value"></param>
        public static void SetDouble(ushort[] src, int start, double value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            bytes = Reverse(bytes);
            ushort[] dest = Bytes2Ushorts(bytes, true);
            dest.CopyTo(src, start);
        }

        /// <summary>
        /// 赋值double类型数据
        /// </summary>
        /// <param name="src"></param>
        /// <param name="start"></param>
        /// <param name="value"></param>
        public static void SetDoubles(ushort[] src, int start, double[] value)
        {
            int ressIndex = 0;
            for (int j = 0; j < src.Length; j += 4)
            {

                byte[] bytes = BitConverter.GetBytes(value[ressIndex]);
                bytes = Reverse(bytes);
                ushort[] dest = Bytes2Ushorts(bytes, true);
                dest.CopyTo(src, start + j);
                ressIndex++;
            }

        }

        /// <summary>
        /// 获取float类型数据
        /// </summary>
        /// <param name="src"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public static float GetReal(ushort[] src, int start)
        {
            ushort[] temp = new ushort[2];
            for (int i = 0; i < 2; i++)
            {
                temp[i] = src[i + start];
            }
            if (HighLow)
            {
                Array.Reverse(temp);//CDAB才要转换，如果是ABCD可屏蔽
            }

            byte[] bytesTemp = Ushorts2Bytes(temp, true);
            bytesTemp = Reverse(bytesTemp);
            float res = BitConverter.ToSingle(bytesTemp, 0);
            return res;
        }

        /// <summary>
        /// 获取double类型数据
        /// </summary>
        /// <param name="src"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public static double GetDouble(ushort[] src, int start)
        {
            ushort[] temp = new ushort[4];
            for (int i = 0; i < 4; i++)
            {
                temp[i] = src[i + start];
            }
            if (HighLow)
            {
                Array.Reverse(temp);//CDAB才要转换，如果是ABCD可屏蔽
            }

            byte[] bytesTemp = Ushorts2Bytes(temp, true);
            bytesTemp = Reverse(bytesTemp);
            double res = BitConverter.ToDouble(bytesTemp, 0);
            return res;
        }

        /// <summary>
        /// 获取double类型数据
        /// </summary>
        /// <param name="src"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public static double[] GetDoubles(ushort[] src, int start)
        {
            double[] ress = new double[src.Length / 4];
            int ressIndex = 0;
            for (int j = 0; j < src.Length; j += 4)
            {

                ushort[] temp = new ushort[4];
                for (int i = 0; i < 4; i++)
                {
                    temp[i] = src[j + i + start];
                }
                if (HighLow)
                {
                    Array.Reverse(temp);//CDAB才要转换，如果是ABCD可屏蔽
                }
               
                byte[] bytesTemp = Ushorts2Bytes(temp, true);
                bytesTemp = Reverse(bytesTemp);
                ress[ressIndex] = BitConverter.ToDouble(bytesTemp, 0);

                ressIndex++;
            }
            return ress;
        }

        /// <summary>
        /// 赋值Short类型数据
        /// </summary>
        /// <param name="src"></param>
        /// <param name="start"></param>
        /// <param name="value"></param>
        public static void SetShort(ushort[] src, int start, short value)
        {
            byte[] bytes = BitConverter.GetBytes(value);

            ushort[] dest = Bytes2Ushorts(bytes);

            dest.CopyTo(src, start);
        }
        /// <summary>
        /// 赋值int类型数据
        /// </summary>
        /// <param name="src"></param>
        /// <param name="start"></param>
        /// <param name="value"></param>
        public static void SetInt(ushort[] src, int start, int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            bytes = Reverse(bytes);

            ushort[] dest = Bytes2Ushorts(bytes, true);

            dest.CopyTo(src, start);
        }
        /// <summary>
        /// 获取short类型数据
        /// </summary>
        /// <param name="src"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public static short GetShort(ushort[] src, int start)
        {
            ushort[] temp = new ushort[1];
            temp[0] = src[start];

            if (HighLow)
            {
                Array.Reverse(temp);//CDAB才要转换，如果是ABCD可屏蔽
            }
            byte[] bytesTemp = Ushorts2Bytes(temp);
            short res = BitConverter.ToInt16(bytesTemp, 0);
            return res;
        }

        /// <summary>
        /// 获取int类型数据
        /// </summary>
        /// <param name="src"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public static int GetInt(ushort[] src, int start)
        {
            ushort[] temp = new ushort[2];
            for (int i = 0; i < 2; i++)
            {
                temp[i] = src[i + start];
            }
            if (HighLow)
            {
                Array.Reverse(temp);//CDAB才要转换，如果是ABCD可屏蔽
            }

            byte[] bytesTemp = Ushorts2Bytes(temp, true);
            bytesTemp = Reverse(bytesTemp);
            short res = BitConverter.ToInt16(bytesTemp, 0);
            return res;
        }

        public static bool[] GetBools(ushort[] src, int start, int num)
        {
            ushort[] temp = new ushort[num];
            for (int i = start; i < start + num; i++)
            {
                temp[i] = src[i + start];
            }
          
            byte[] bytes = Ushorts2Bytes(temp);

            bool[] res = Bytes2Bools(bytes);

            return res;
        }
        private static bool[] Bytes2Bools(byte[] b)
        {
            bool[] array = new bool[8 * b.Length];

            for (int i = 0; i < b.Length; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    array[i * 8 + j] = (b[i] & 1) == 1;//判定byte的最后一位是否为1，若为1，则是true；否则是false
                    b[i] = (byte)(b[i] >> 1);//将byte右移一位
                }
            }
            return array;
        }
        private static byte Bools2Byte(bool[] array)
        {
            if (array != null && array.Length > 0)
            {
                byte b = 0;
                for (int i = 0; i < 8; i++)
                {
                    if (array[i])
                    {
                        byte nn = (byte)(1 << i);//左移一位，相当于×2
                        b += nn;
                    }
                }
                return b;
            }
            return 0;
        }
        private static ushort[] Bytes2Ushorts(byte[] src, bool reverse = false)
        {
            int len = src.Length;

            byte[] srcPlus = new byte[len + 1];
            src.CopyTo(srcPlus, 0);
            int count = len >> 1;

            if (len % 2 != 0)
            {
                count += 1;
            }

            ushort[] dest = new ushort[count];
            if (reverse)
            {
                for (int i = 0; i < count; i++)
                {
                    dest[i] = (ushort)(srcPlus[i * 2] << 8 | srcPlus[2 * i + 1] & 0xff);
                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    dest[i] = (ushort)(srcPlus[i * 2] & 0xff | srcPlus[2 * i + 1] << 8);
                }
            }

            return dest;
        }
        private static byte[] Ushorts2Bytes(ushort[] src, bool reverse = false)
        {

            int count = src.Length;
            byte[] dest = new byte[count << 1];
            if (reverse)
            {
                for (int i = 0; i < count; i++)
                {
                    dest[i * 2] = (byte)(src[i] >> 8);
                    dest[i * 2 + 1] = (byte)(src[i] >> 0);
                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    dest[i * 2] = (byte)(src[i] >> 0);
                    dest[i * 2 + 1] = (byte)(src[i] >> 8);
                }
            }
            return dest;
        }


        public static Byte[] Reverse(Byte[] data)
        {
            Array.Reverse(data);
            return data;
        }

    }
}

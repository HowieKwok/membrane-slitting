using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlyCapture2Managed;
using LogTool;
using S7.Net;
using S7.Net.Types;

namespace Frame_Core
{
    public class S7Net : FileOperationINI
    {
        Plc PLC = null;
        public bool connected = false;

        public bool Connect(CpuType xmzMoedl, string ip, int port)
        {

            try
            {
                CloseConnect();
                PLC = new Plc(xmzMoedl, ip, port, 0, 1);
                PLC.Open();
                if (PLC.IsConnected) connected = true;

            }
            catch (Exception ex)
            {
                connected = false;
                Log4netRecord.ErrorLog.Debug(ex);
            }
            return connected;
        }

        public void CloseConnect()
        {
            try
            {
                if (PLC == null) return;
                if (PLC.IsConnected) PLC.Close();
                if (!PLC.IsConnected) connected = false;

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
            }

        }
        object readDataLock = new object();
        object sendDataLock = new object();
        #region 读取
        /// <summary>
        /// 读取Double (   //DB12.DBX16.0()12是db，16是startByteAdr，0是bitAdr
        /// </summary>
        /// <param name="strLogPath">日志路径</param>
        /// <param name="strDescribe">信息描述</param>
        /// <param name="db">DB值</param>
        /// <param name="startByteAdr">DBX值（字）</param>
        /// <param name="varCount">读取数量</param>
        /// <param name="data">读取数据</param>
        /// <param name="bitAdr">DBX值（位）</param>
        /// <param name="isSaveLog">是否保存日志</param>
        /// <returns></returns>
        public bool Read_Doubles(string strLogPath, ref DoubleData doubleData, out double data, bool isSaveLog = true)
        {
            lock (readDataLock)
            {
                data = 0;
                string strDescribe = doubleData.name;//信息描述
                string dbName = doubleData.dbName;//DB名称
                string startByteAdrName = doubleData.startByteAdrName;//DBX值名称

                int db = doubleData.db;//DB值
                int startByteAdr = doubleData.startByteAdr;//DBX值（字）
                try
                {


                    data = (double)PLC.Read(DataType.DataBlock, db, startByteAdr, VarType.LReal, 1);
                    string readdata = string.Join(",", data);//合并字符数据
                    if (data != 0 && doubleData.OldValue != data && isSaveLog)
                    {
                        doubleData.OldValue = data;
                        RedPLCLog(strLogPath, $"读取寄存器{dbName}{db}.{startByteAdrName}{startByteAdr}", "数值:" + readdata + "读取状态:成功----" + strDescribe);
                    }
                    else
                    {

                    }

                    return true;


                }
                catch (Exception ex)
                {
                    RedPLCLog(strLogPath, "读取PLC失败", "地址：" + $"{dbName}{db}.{startByteAdrName}{startByteAdr}" + "(" + strDescribe + ")");
                    Log4netRecord.ErrorLog.Debug(ex);
                    return false;
                }
            }
        }

        public bool Read_Floats(string strLogPath, FloatData floatData, out float data, bool isSaveLog = true)
        {
            lock (readDataLock)
            {
                data = 0;
                string strDescribe = floatData.name;//信息描述
                string dbName = floatData.dbName;//DB名称
                string startByteAdrName = floatData.startByteAdrName;//DBX值名称

                int db = floatData.db;//DB值
                int startByteAdr = floatData.startByteAdr;//DBX值（字）
                try
                {

                    data = (float)PLC.Read(DataType.DataBlock, db, startByteAdr, VarType.Real, 1);
                    string readdata = string.Join(",", data);//合并字符数据
                    if (data != 0 && isSaveLog)
                    {

                        RedPLCLog(strLogPath, $"读取寄存器{dbName}{db}.{startByteAdrName}{startByteAdr}", "数值:" + readdata + "读取状态:成功----" + strDescribe);
                    }
                    else
                    {

                    }

                    return true;
                }
                catch (Exception ex)
                {
                    RedPLCLog(strLogPath, "读取PLC失败", "地址：" + $"{dbName}{db}.{startByteAdrName}{startByteAdr}" + "(" + strDescribe + ")");
                    Log4netRecord.ErrorLog.Debug(ex);
                    return false;
                }
            }
        }

        public bool Read_MESFloats(string strLogPath, FloatData floatData, out float data, bool isSaveLog = true)
        {
            lock (readDataLock)
            {
                data = 0;
                string strDescribe = floatData.name;//信息描述
                string dbName = floatData.dbName;//DB名称
                string startByteAdrName = floatData.startByteAdrName;//DBX值名称

                int db = floatData.db;//DB值
                int startByteAdr = floatData.startByteAdr;//DBX值（字）
                try
                {

                    data = (float)PLC.Read(DataType.DataBlock, db, startByteAdr, VarType.Real, 1);
                    string readdata = string.Join(",", data);//合并字符数据
                    if (floatData.OldValue != data)
                    {
                        floatData.OldValue = data;
                        floatData.readDifferentStateFlag = true;
                    }
                    else
                    {

                    }
                    if (isSaveLog)
                    {
                        RedPLCLog(strLogPath, $"读取寄存器{dbName}{db}.{startByteAdrName}{startByteAdr}", "数值:" + readdata + "读取状态:成功----" + strDescribe);
                    }


                    return true;
                }
                catch (Exception ex)
                {
                    RedPLCLog(strLogPath, "读取PLC失败", "地址：" + $"{dbName}{db}.{startByteAdrName}{startByteAdr}" + "(" + strDescribe + ")");
                    Log4netRecord.ErrorLog.Debug(ex);
                    return false;
                }
            }
        }

        public bool Read_Shorts(string strLogPath, ShortData shortData, out short data, bool isSaveLog = true)
        {
            lock (readDataLock)
            {
                data = 0;
                string strDescribe = shortData.name;//信息描述
                string dbName = shortData.dbName;//DB名称
                string startByteAdrName = shortData.startByteAdrName;//DBX值名称

                int db = shortData.db;//DB值
                int startByteAdr = shortData.startByteAdr;//DBX值（字）
                try
                {

                    data = (short)PLC.Read(DataType.DataBlock, db, startByteAdr, VarType.Int, 1);
                    string readdata = string.Join(",", data);//合并字符数据
                    if (data != 0 && isSaveLog)
                    {

                        RedPLCLog(strLogPath, $"读取寄存器{dbName}{db}.{startByteAdrName}{startByteAdr}", "数值:" + readdata + "读取状态:成功----" + strDescribe);
                    }
                    else
                    {

                    }

                    return true;
                }
                catch (Exception ex)
                {
                    RedPLCLog(strLogPath, "读取PLC失败", "地址：" + $"{dbName}{db}.{startByteAdrName}{startByteAdr}" + "(" + strDescribe + ")");
                    Log4netRecord.ErrorLog.Debug(ex);
                    return false;
                }
            }
        }
        public bool Read_ShortTemp(string strLogPath, ShortData shortData, out short data, out bool result, bool isSaveLog = true)
        {
            lock (readDataLock)
            {
                result = false;
                data = 0;
                string strDescribe = shortData.name;//信息描述
                string dbName = shortData.dbName;//DB名称
                string startByteAdrName = shortData.startByteAdrName;//DBX值名称

                int db = shortData.db;//DB值
                int startByteAdr = shortData.startByteAdr;//DBX值（字）
                try
                {

                    data = (short)PLC.Read(DataType.DataBlock, db, startByteAdr, VarType.Int, 1);
                    string readdata = string.Join(",", data);//合并字符数据              
                    if (shortData.OldValue != data && isSaveLog)
                    {
                        result = true;
                        RedPLCLog(strLogPath, $"读取寄存器{dbName}{db}.{startByteAdrName}{startByteAdr}", "数值:" + readdata + "读取状态:成功----" + strDescribe);
                    }
                    else
                    {

                    }

                    return true;
                }
                catch (Exception ex)
                {
                    RedPLCLog(strLogPath, "读取PLC失败", "地址：" + $"{dbName}{db}.{startByteAdrName}{startByteAdr}" + "(" + strDescribe + ")");
                    Log4netRecord.ErrorLog.Debug(ex);
                    return false;
                }
            }
        }

        public bool Read_Ints(string strLogPath, IntData intData, out int data, bool isSaveLog = true)
        {
            lock (readDataLock)
            {
                data = 0;
                string strDescribe = intData.name;//信息描述
                string dbName = intData.dbName;//DB名称
                string startByteAdrName = intData.startByteAdrName;//DBX值名称

                int db = intData.db;//DB值
                int startByteAdr = intData.startByteAdr;//DBX值（字）
                try
                {

                    data = (int)PLC.Read(DataType.DataBlock, db, startByteAdr, VarType.DInt, 1);
                    string readdata = string.Join(",", data);//合并字符数据
                    if (data != 0 && isSaveLog)
                    {

                        RedPLCLog(strLogPath, $"读取寄存器{dbName}{db}.{startByteAdrName}{startByteAdr}", "数值:" + readdata + "读取状态:成功----" + strDescribe);
                    }
                    else
                    {

                    }

                    return true;
                }
                catch (Exception ex)
                {
                    RedPLCLog(strLogPath, "读取PLC失败", "地址：" + $"{dbName}{db}.{startByteAdrName}{startByteAdr}" + "(" + strDescribe + ")");
                    Log4netRecord.ErrorLog.Debug(ex);
                    return false;
                }
            }
        }

        public bool Read_UInts(string strLogPath, UIntData uIntData, out uint data, bool isSaveLog = true)
        {
            lock (readDataLock)
            {
                data = 0;
                string strDescribe = uIntData.name;//信息描述
                string dbName = uIntData.dbName;//DB名称
                string startByteAdrName = uIntData.startByteAdrName;//DBX值名称

                int db = uIntData.db;//DB值
                int startByteAdr = uIntData.startByteAdr;//DBX值（字）
                try
                {

                    data = (uint)PLC.Read(DataType.DataBlock, db, startByteAdr, VarType.DWord, 1);
                    string readdata = string.Join(",", data);//合并字符数据
                    if (data != 0 && isSaveLog)
                    {

                        RedPLCLog(strLogPath, $"读取寄存器{dbName}{db}.{startByteAdrName}{startByteAdr}", "数值:" + readdata + "读取状态:成功----" + strDescribe);
                    }
                    else
                    {

                    }

                    return true;
                }
                catch (Exception ex)
                {
                    RedPLCLog(strLogPath, "读取PLC失败", "地址：" + $"{dbName}{db}.{startByteAdrName}{startByteAdr}" + "(" + strDescribe + ")");
                    Log4netRecord.ErrorLog.Debug(ex);
                    return false;
                }
            }
        }

        public bool Read_String(string strLogPath, StringData stringData, int varCount, out string data, bool isSaveLog = true)
        {
            lock (readDataLock)
            {
                data = "";
                string strDescribe = stringData.name;//信息描述
                string dbName = stringData.dbName;//DB名称
                string startByteAdrName = stringData.startByteAdrName;//DBX值名称

                int db = stringData.db;//DB值
                int startByteAdr = stringData.startByteAdr;//DBX值（字）
                try
                {
                    data = (string)PLC.Read(DataType.DataBlock, db, startByteAdr, VarType.String, varCount);
                    data = data.Trim(new char[] { '\0' });
                    data = data.Trim(new char[] { ' ' });

                    //string str= Encoding.ASCII.GetString(sdg);
                    //Encoding.ASCII.GetBytes(sdg);
                    string readdata = data;
                    if (data != "" && isSaveLog)// 大于0才记录寄存器数据
                        RedPLCLog(strLogPath, $"读取寄存器{dbName}{db}.{startByteAdrName}{startByteAdr}", "数值:" + readdata + "读取状态:成功----" + strDescribe);
                    return true;
                }
                catch (Exception ex)
                {
                    RedPLCLog(strLogPath, "读取PLC失败", "地址：" + $"{dbName}{db}.{startByteAdrName}{startByteAdr}" + "(" + strDescribe + ")");
                    Log4netRecord.ErrorLog.Debug(ex);
                    return false;
                }
            }
        }

        public bool Read_Bool(string strLogPath, BoolData boolData, out bool data, bool isSaveLog = true)
        {
            lock (readDataLock)
            {
                data = true;
                string strDescribe = boolData.name;//信息描述
                string dbName = boolData.dbName;//DB名称
                string startByteAdrName = boolData.startByteAdrName;//DBX值名称

                int db = boolData.db;//DB值
                int startByteAdr = boolData.startByteAdr;//DBX值（字）
                try
                {

                    data = (bool)PLC.Read(DataType.DataBlock, db, startByteAdr, VarType.Bit, 1);
                    string readdata = string.Join(",", data);//合并字符数据
                    if (boolData.OldValue != data && isSaveLog)
                    {
                        boolData.OldValue = data;
                        boolData.readDifferentStateFlag = true;
                        RedPLCLog(strLogPath, $"读取寄存器{dbName}{db}.{startByteAdrName}{startByteAdr}", "数值:" + readdata + "读取状态:成功----" + strDescribe);
                    }
                    else
                    {

                    }

                    return true;
                }
                catch (Exception ex)
                {
                    RedPLCLog(strLogPath, "读取PLC失败", "地址：" + $"{dbName}{db}.{startByteAdrName}{startByteAdr}" + "(" + strDescribe + ")");
                    Log4netRecord.ErrorLog.Debug(ex);
                    return false;
                }
            }
        }
        #endregion

        #region 写入
        public bool Write_Doubles(string strLogPath, DoubleData doubleData, double[] data, byte bitAdr = 0)
        {
            lock (sendDataLock)
            {
                string strDescribe = doubleData.name;//信息描述
                string dbName = doubleData.dbName;//DB名称
                string startByteAdrName = doubleData.startByteAdrName;//DBX值名称

                int db = doubleData.db;//DB值
                int startByteAdr = doubleData.startByteAdr;//DBX值（字）
                try
                {
                    DataItem dataItem = new DataItem();
                    dataItem.DataType = DataType.DataBlock;
                    dataItem.DB = db;
                    dataItem.StartByteAdr = startByteAdr;
                    dataItem.VarType = VarType.LReal;
                    dataItem.Value = data;
                    dataItem.BitAdr = bitAdr;

                    PLC.Write(dataItem);
                    string readdata = string.Join(",", data);//合并字符数据
                    SendPLCLog(strLogPath, $"写入寄存器{dbName}{db}.{startByteAdrName}{startByteAdr}", "数值:" + readdata + "读取状态:成功----" + strDescribe);
                    return true;
                }
                catch (Exception ex)
                {
                    SendPLCLog(strLogPath, "写入PLC失败", "地址：" + $"{dbName}{db}.{startByteAdrName}{startByteAdr}" + "(" + strDescribe + ")");
                    Log4netRecord.ErrorLog.Debug(ex);
                    return false;
                }
            }
        }

        public bool Write_Floats(string strLogPath, FloatData floatData, float[] data, byte bitAdr = 0)
        {
            lock (sendDataLock)
            {
                string strDescribe = floatData.name;//信息描述
                string dbName = floatData.dbName;//DB名称
                string startByteAdrName = floatData.startByteAdrName;//DBX值名称

                int db = floatData.db;//DB值
                int startByteAdr = floatData.startByteAdr;//DBX值（字）
                try
                {
                    DataItem dataItem = new DataItem();
                    dataItem.DataType = DataType.DataBlock;
                    dataItem.DB = db;
                    dataItem.StartByteAdr = startByteAdr;
                    dataItem.VarType = VarType.Real;
                    dataItem.Value = data;
                    dataItem.BitAdr = bitAdr;

                    PLC.Write(dataItem);
                    string readdata = string.Join(",", data);//合并字符数据
                    SendPLCLog(strLogPath, $"写入寄存器{dbName}{db}.{startByteAdrName}{startByteAdr}", "数值:" + readdata + "读取状态:成功----" + strDescribe);
                    return true;
                }
                catch (Exception ex)
                {
                    SendPLCLog(strLogPath, "写入PLC失败", "地址：" + $"{dbName}{db}.{startByteAdrName}{startByteAdr}" + "(" + strDescribe + ")");
                    Log4netRecord.ErrorLog.Debug(ex);
                    return false;
                }
            }
        }

        public bool Write_Shorts(string strLogPath, ShortData shortData, short[] data, byte bitAdr = 0)
        {
            lock (sendDataLock)
            {               
                string strDescribe = shortData.name;//信息描述
                string dbName = shortData.dbName;//DB名称
                string startByteAdrName = shortData.startByteAdrName;//DBX值名称

                int db = shortData.db;//DB值
                int startByteAdr = shortData.startByteAdr;//DBX值（字）
                try
                {                   
                    DataItem dataItem = new DataItem();
                    dataItem.DataType = DataType.DataBlock;
                    dataItem.DB = db;
                    dataItem.StartByteAdr = startByteAdr;
                    dataItem.VarType = VarType.Int;
                    dataItem.Value = data;
                    dataItem.BitAdr = bitAdr;

                    PLC.Write(dataItem);
                    string readdata = string.Join(",", data);//合并字符数据
                    SendPLCLog(strLogPath, $"写入寄存器{dbName}{db}.{startByteAdrName}{startByteAdr}", "数值:" + readdata + "读取状态:成功----" + strDescribe);
                    return true;
                }
                catch (Exception ex)
                {
                    SendPLCLog(strLogPath, "写入PLC失败", "地址：" + $"{dbName}{db}.{startByteAdrName}{startByteAdr}" + "(" + strDescribe + ")");
                    Log4netRecord.ErrorLog.Debug(ex);
                    return false;
                }
            }
        }

        public bool Write_Ints(string strLogPath, IntData intData, int[] data, byte bitAdr = 0)
        {
            lock (sendDataLock)
            {
                string strDescribe = intData.name;//信息描述
                string dbName = intData.dbName;//DB名称
                string startByteAdrName = intData.startByteAdrName;//DBX值名称

                int db = intData.db;//DB值
                int startByteAdr = intData.startByteAdr;//DBX值（字）
                try
                {
                    DataItem dataItem = new DataItem();
                    dataItem.DataType = DataType.DataBlock;
                    dataItem.DB = db;
                    dataItem.StartByteAdr = startByteAdr;
                    dataItem.VarType = VarType.DInt;
                    dataItem.Value = data;
                    dataItem.BitAdr = bitAdr;

                    PLC.Write(dataItem);
                    string readdata = string.Join(",", data);//合并字符数据
                    SendPLCLog(strLogPath, $"写入寄存器{dbName}{db}.{startByteAdrName}{startByteAdr}", "数值:" + readdata + "读取状态:成功----" + strDescribe);
                    return true;
                }
                catch (Exception ex)
                {
                    SendPLCLog(strLogPath, "写入PLC失败", "地址：" + $"{dbName}{db}.{startByteAdrName}{startByteAdr}" + "(" + strDescribe + ")");
                    Log4netRecord.ErrorLog.Debug(ex);
                    return false;
                }
            }
        }

        public bool Write_UInts(string strLogPath, UIntData uIntData, uint[] data, byte bitAdr = 0)
        {
            lock (sendDataLock)
            {
                string strDescribe = uIntData.name;//信息描述
                string dbName = uIntData.dbName;//DB名称
                string startByteAdrName = uIntData.startByteAdrName;//DBX值名称

                int db = uIntData.db;//DB值
                int startByteAdr = uIntData.startByteAdr;//DBX值（字）
                try
                {
                    DataItem dataItem = new DataItem();
                    dataItem.DataType = DataType.DataBlock;
                    dataItem.DB = db;
                    dataItem.StartByteAdr = startByteAdr;
                    dataItem.VarType = VarType.DWord;
                    dataItem.Value = data;
                    dataItem.BitAdr = bitAdr;

                    PLC.Write(dataItem);
                    string readdata = string.Join(",", data);//合并字符数据
                    SendPLCLog(strLogPath, $"写入寄存器{dbName}{db}.{startByteAdrName}{startByteAdr}", "数值:" + readdata + "读取状态:成功----" + strDescribe);
                    return true;
                }
                catch (Exception ex)
                {
                    SendPLCLog(strLogPath, "写入PLC失败", "地址：" + $"{dbName}{db}.{startByteAdrName}{startByteAdr}" + "(" + strDescribe + ")");
                    Log4netRecord.ErrorLog.Debug(ex);
                    return false;
                }
            }
        }

        public bool Write_String(string strLogPath, StringData stringData, int varCount, string data, byte bitAdr = 0)
        {
            lock (sendDataLock)
            {
                string strDescribe = stringData.name;//信息描述
                string dbName = stringData.dbName;//DB名称
                string startByteAdrName = stringData.startByteAdrName;//DBX值名称

                int db = stringData.db;//DB值
                int startByteAdr = stringData.startByteAdr;//DBX值（字）
                try
                {
                    DataItem dataItem = new DataItem();
                    dataItem.DataType = DataType.DataBlock;
                    dataItem.DB = db;
                    dataItem.StartByteAdr = startByteAdr;
                    dataItem.VarType = VarType.String;
                    dataItem.Value = data;
                    dataItem.BitAdr = bitAdr;
                    dataItem.Count = varCount;

                    PLC.Write(dataItem);
                    string readdata = data;//合并字符数据
                    SendPLCLog(strLogPath, $"写入寄存器{dbName}{db}.{startByteAdrName}{startByteAdr}", "数值:" + readdata + "读取状态:成功----" + strDescribe);
                    return true;
                }
                catch (Exception ex)
                {
                    SendPLCLog(strLogPath, "写入PLC失败", "地址：" + $"{dbName}{db}.{startByteAdrName}{startByteAdr}" + "(" + strDescribe + ")");
                    Log4netRecord.ErrorLog.Debug(ex);
                    return false;
                }
            }
        }
        #endregion



        object readLock = new object();
        object sendLock = new object();
        public void RedPLCLog(string filePath, string message, string track)
        {
            lock (readLock)
            {
                try
                {
                    Write_ReadLog_ini(filePath, message, track);

                }
                catch (Exception ex)
                {
                    Log4netRecord.ErrorLog.Fatal(ex + "\n路径：" + filePath + "\n信息：\n" + message + "  " + track);
                }
            }
        }

        public void SendPLCLog(string filePath, string message, string track)
        {
            lock (sendLock)
            {
                try
                {
                    Write_SendLog_ini(filePath, message, track);

                }
                catch (Exception ex)
                {
                    Log4netRecord.ErrorLog.Fatal(ex + "\n路径：" + filePath + "\n信息：\n" + message + "  " + track);

                }
            }
        }


    }
}

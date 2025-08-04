using HslCommunication.Core.Net;
using HslCommunication.Profinet.Omron;
using HslCommunication;
using LogTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HslCommunication.Robot.EFORT;
//using NPOI.SS.Formula.Functions;
using HslCommunication.Profinet.Siemens;
using HslCommunication.ModBus;
using NPOI.SS.Formula.Functions;
using static log4net.Appender.RollingFileAppender;
using System.Threading;
using HslCommunication.Core.Device;

namespace Frame_Core
{
    public class PLCCommunicationBase
    {
        public bool InitState = false;
        /// <summary>
        /// 写入失败数量
        /// </summary>
        public int WriteFailNum = 2;
        /// <summary>
        /// 读取失败数量
        /// </summary>
        public int ReadFailNum = 2;
        /// <summary>
        /// 写入失败计数
        /// </summary>
        public int WriteFailCount = 0;
        /// <summary>
        /// 读取失败计数
        /// </summary>
        public int ReadFailCount = 0;

        DeviceTcpNet networkDeviceBase = null;
        /// <summary>
        /// 读取失败警告监控
        /// </summary>
        public void ReadDataAlarmMonitoring()
        {
            if (ReadFailCount > ReadFailNum)
            {
                Disconnect();
                TJBJInfo.AlarmWriteState(AlarmName.主设备连接失败, new StopAlarmMessage() { StopAlarmState = StopAlarmStateEnum.停机, describe = "读取数据失败次数过多" });

            }
        }

        /// <summary>
        /// 读取失败警告监控处理
        /// </summary>
        private void ReadDataAlarmMonitoringProcessing(bool isSuccess)
        {
            if (!isSuccess) ReadFailCount++;
            ReadDataAlarmMonitoring();
        }

        /// <summary>
        /// 发送失败警告监控
        /// </summary>
        public void SendDataAlarmMonitoring()
        {
            if (WriteFailCount > WriteFailNum)
            {
                Disconnect();
                TJBJInfo.AlarmWriteState(AlarmName.主设备连接失败, new StopAlarmMessage() { StopAlarmState = StopAlarmStateEnum.停机, describe = "发送数据失败次数过多" });

            }
        }

        /// <summary>
        /// 发送失败警告监控处理
        /// </summary>
        private void SendDataAlarmMonitoringProcessing(bool isSuccess)
        {
            if (!isSuccess) WriteFailCount++;
            SendDataAlarmMonitoring();
        }

        /// <summary>
        /// 连接PLC
        /// </summary>
        /// <param name="plcType"></param>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public bool Connect(bool isConnect, PLCTypeEnum pLCTypeEnum, CommunicationProtocolEnum protocolEnum, string ip, int port,emDataFormat dataFormat=emDataFormat.ABCD)
        {
            try
            {
                if (!isConnect) return false;

                Disconnect();
                Thread.Sleep(200);

                if (networkDeviceBase == null)
                {
                    switch (protocolEnum)
                    {
                        case CommunicationProtocolEnum.Fins:
                            networkDeviceBase = new OmronFinsNet();
                            networkDeviceBase.ByteTransform.DataFormat = (HslCommunication.Core.DataFormat)dataFormat;
                            break;
                        case CommunicationProtocolEnum.CIP:
                            networkDeviceBase = new OmronCipNet();
                            networkDeviceBase.ByteTransform.DataFormat = (HslCommunication.Core.DataFormat)dataFormat;
                            break;
                        case CommunicationProtocolEnum.S7:
                            if (pLCTypeEnum == PLCTypeEnum.Siemens1200)
                            {
                                networkDeviceBase = new SiemensS7Net(SiemensPLCS.S1200);
                                networkDeviceBase.ByteTransform.DataFormat = (HslCommunication.Core.DataFormat)dataFormat;
                            }
                            else if (pLCTypeEnum == PLCTypeEnum.Siemens1500)
                            {
                                networkDeviceBase = new SiemensS7Net(SiemensPLCS.S1500);
                                networkDeviceBase.ByteTransform.DataFormat = (HslCommunication.Core.DataFormat)dataFormat;
                            }

                            break;
                        case CommunicationProtocolEnum.ModbusTCP:
                            networkDeviceBase = new ModbusTcpNet();
                            networkDeviceBase.ByteTransform.DataFormat = (HslCommunication.Core.DataFormat)dataFormat;
                            break;
                        case CommunicationProtocolEnum.TCP:
                            break;
                        case CommunicationProtocolEnum.UDP:
                            break;
                        default:
                            break;
                    }
                }
                

                if (networkDeviceBase == null) return false;

                networkDeviceBase.IpAddress = ip;
                networkDeviceBase.Port = port;
                networkDeviceBase.ByteTransform.DataFormat = (HslCommunication.Core.DataFormat)dataFormat;
                OperateResult connect = networkDeviceBase.ConnectServer();
                if (connect.IsSuccess) InitState = true;
                else InitState = false;

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
            }
            return InitState;
        }

        /// <summary>
        /// 断开PLC
        /// </summary>
        public void Disconnect()
        {
            try
            {
                WriteFailCount = 0;
                ReadFailCount = 0;
                InitState = false;

                networkDeviceBase?.ConnectClose();
                networkDeviceBase?.Dispose();
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
            }
        }

        private void ReadResultProcessing<T>(CommunicationData<T> data, bool isSuccess, T value)
        {
            //保存旧值
            data.ReadDifferentStateFlag = false;
            if (isSuccess)
            {
                if (IsContentDifferent(value, data.Value))
                {
                    data.ReadDifferentStateFlag = true;
                    data.OldReadValue = data.Value;
                }
                data.Value = value;
            }
        }
        private bool IsContentDifferent<T>(T value1, T value2)
        {
            if (typeof(T).IsArray)
            {
                // 如果是数组类型，使用 SequenceEqual 方法比较内容
                Array array1 = value1 as Array;
                Array array2 = value2 as Array;
                return !array1.Cast<object>().SequenceEqual(array2.Cast<object>());
            }
            else
            {
                // 对于其他类型，使用 Equals 方法比较内容
                return !value1.Equals(value2);
            }
        }
        private void SaveWriteLog<T>(CommunicationData<T> data, OperateResult operateResult, log4net.ILog log)
        {
            //保存日志        
            string resultStr = "";
            string message = "";
            if (operateResult.IsSuccess) resultStr += "写入成功";
            else resultStr += "写入失败";
            message = $"{resultStr}-名称：{data.Name}-地址：[{data.Address}]-值：[{string.Join(",", data.Value)}]";

            log.Debug(message + "_" + operateResult.Message);
        }
        private void SaveReadLog<T>(CommunicationData<T> data, bool isSuccess, string resultMessage, log4net.ILog log)
        {
            //保存日志        
            string resultStr = "";
            string message = "";
            if (isSuccess) resultStr += "读取成功";
            else resultStr += "读取失败";
            message = $"{resultStr}-名称：{data.Name}-地址：[{data.Address}]-值：[{string.Join(",", data.Value)}]";

            log.Debug(message + "_" + resultMessage);
        }
        private void SaveReadLog<T>(CommunicationData<T>[] data, bool isSuccess, string resultMessage, log4net.ILog log)
        {
            //保存日志        
            string resultStr = "";
            string message = "";
            if (isSuccess) resultStr += "读取成功";
            else resultStr += "读取失败";
            message = $"{resultStr}-名称：{data[0].Name}-地址：[{data[0].Address}]-值：[{string.Join(",", data.Select(d => d.Value))}]";

            log.Debug(message + "_" + resultMessage);
        }
        private bool GetIsSaveReadLog(ReadLogSaveType readLogSaveType, bool isReadDifferentStateFlag)
        {
            bool isSaveLog = false;
            if (readLogSaveType == ReadLogSaveType.ReadDifferentState && isReadDifferentStateFlag) isSaveLog = true;
            else if (readLogSaveType == ReadLogSaveType.True) isSaveLog = true;
            return isSaveLog;
        }

        public void Write(CommunicationData<bool> data, bool isSaveLog, log4net.ILog log)
        {
            if (!InitState) return;
            string message = data.Name;
            try
            {
                OperateResult operateResult = networkDeviceBase.Write(data.Address, data.Value);

                SendDataAlarmMonitoringProcessing(operateResult.IsSuccess);

                if (isSaveLog) SaveWriteLog(data, operateResult, log);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(message, ex);
            }
        }

        public void Write(CommunicationData<bool[]> data, bool isSaveLog, log4net.ILog log)
        {
            if (!InitState) return;
            string message = data.Name;
            try
            {
                OperateResult operateResult = networkDeviceBase.Write(data.Address, data.Value);

                SendDataAlarmMonitoringProcessing(operateResult.IsSuccess);

                if (isSaveLog) SaveWriteLog(data, operateResult, log);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(message, ex);
            }
        }

        public void Write(CommunicationData<short> data, bool isSaveLog, log4net.ILog log)
        {
            if (!InitState) return;
            string message = data.Name;
            try
            {
                OperateResult operateResult = networkDeviceBase.Write(data.Address, data.Value);

                SendDataAlarmMonitoringProcessing(operateResult.IsSuccess);

                if (isSaveLog) SaveWriteLog(data, operateResult, log);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(message, ex);
            }
        }
        public void Write(CommunicationData<short[]> data, bool isSaveLog, log4net.ILog log)
        {
            if (!InitState) return;
            string message = data.Name;
            try
            {
                OperateResult operateResult = networkDeviceBase.Write(data.Address, data.Value);

                SendDataAlarmMonitoringProcessing(operateResult.IsSuccess);

                if (isSaveLog) SaveWriteLog(data, operateResult, log);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(message, ex);
            }
        }

        public void Write(CommunicationData<int> data, bool isSaveLog, log4net.ILog log)
        {
            if (!InitState) return;
            string message = data.Name;
            try
            {
                OperateResult operateResult = networkDeviceBase.Write(data.Address, data.Value);

                SendDataAlarmMonitoringProcessing(operateResult.IsSuccess);

                if (isSaveLog) SaveWriteLog(data, operateResult, log);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(message, ex);
            }
        }
        public void Write(CommunicationData<int[]> data, bool isSaveLog, log4net.ILog log)
        {
            if (!InitState) return;
            string message = data.Name;
            try
            {
                OperateResult operateResult = networkDeviceBase.Write(data.Address, data.Value);

                SendDataAlarmMonitoringProcessing(operateResult.IsSuccess);

                if (isSaveLog) SaveWriteLog(data, operateResult, log);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(message, ex);
            }
        }

        public void Write(CommunicationData<long> data, bool isSaveLog, log4net.ILog log)
        {
            if (!InitState) return;
            string message = data.Name;
            try
            {
                OperateResult operateResult = networkDeviceBase.Write(data.Address, data.Value);

                SendDataAlarmMonitoringProcessing(operateResult.IsSuccess);

                if (isSaveLog) SaveWriteLog(data, operateResult, log);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(message, ex);
            }
        }
        public void Write(CommunicationData<long[]> data, bool isSaveLog, log4net.ILog log)
        {
            if (!InitState) return;
            string message = data.Name;
            try
            {
                OperateResult operateResult = networkDeviceBase.Write(data.Address, data.Value);

                SendDataAlarmMonitoringProcessing(operateResult.IsSuccess);

                if (isSaveLog) SaveWriteLog(data, operateResult, log);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(message, ex);
            }
        }

        public void Write(CommunicationData<ushort> data, bool isSaveLog, log4net.ILog log)
        {
            if (!InitState) return;
            string message = data.Name;
            try
            {
                OperateResult operateResult = networkDeviceBase.Write(data.Address, data.Value);

                SendDataAlarmMonitoringProcessing(operateResult.IsSuccess);

                if (isSaveLog) SaveWriteLog(data, operateResult, log);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(message, ex);
            }
        }
        public void Write(CommunicationData<ushort[]> data, bool isSaveLog, log4net.ILog log)
        {
            if (!InitState) return;
            string message = data.Name;
            try
            {
                OperateResult operateResult = networkDeviceBase.Write(data.Address, data.Value);

                SendDataAlarmMonitoringProcessing(operateResult.IsSuccess);

                if (isSaveLog) SaveWriteLog(data, operateResult, log);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(message, ex);
            }
        }

        public void Write(CommunicationData<uint> data, bool isSaveLog, log4net.ILog log)
        {
            if (!InitState) return;
            string message = data.Name;
            try
            {
                OperateResult operateResult = networkDeviceBase.Write(data.Address, data.Value);

                SendDataAlarmMonitoringProcessing(operateResult.IsSuccess);

                if (isSaveLog) SaveWriteLog(data, operateResult, log);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(message, ex);
            }
        }
        public void Write(CommunicationData<uint[]> data, bool isSaveLog, log4net.ILog log)
        {
            if (!InitState) return;
            string message = data.Name;
            try
            {
                OperateResult operateResult = networkDeviceBase.Write(data.Address, data.Value);

                SendDataAlarmMonitoringProcessing(operateResult.IsSuccess);

                if (isSaveLog) SaveWriteLog(data, operateResult, log);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(message, ex);
            }
        }

        public void Write(CommunicationData<ulong> data, bool isSaveLog, log4net.ILog log)
        {
            if (!InitState) return;
            string message = data.Name;
            try
            {
                OperateResult operateResult = networkDeviceBase.Write(data.Address, data.Value);

                SendDataAlarmMonitoringProcessing(operateResult.IsSuccess);

                if (isSaveLog) SaveWriteLog(data, operateResult, log);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(message, ex);
            }
        }
        public void Write(CommunicationData<ulong[]> data, bool isSaveLog, log4net.ILog log)
        {
            if (!InitState) return;
            string message = data.Name;
            try
            {
                OperateResult operateResult = networkDeviceBase.Write(data.Address, data.Value);

                SendDataAlarmMonitoringProcessing(operateResult.IsSuccess);

                if (isSaveLog) SaveWriteLog(data, operateResult, log);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(message, ex);
            }
        }

        public void Write(CommunicationData<float> data, bool isSaveLog, log4net.ILog log)
        {
            if (!InitState) return;
            string message = data.Name;
            try
            {
                OperateResult operateResult = networkDeviceBase.Write(data.Address, data.Value);

                SendDataAlarmMonitoringProcessing(operateResult.IsSuccess);

                if (isSaveLog) SaveWriteLog(data, operateResult, log);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(message, ex);
            }
        }
        public void Write(CommunicationData<float[]> data, bool isSaveLog, log4net.ILog log)
        {
            if (!InitState) return;
            string message = data.Name;
            try
            {
                OperateResult operateResult = networkDeviceBase.Write(data.Address, data.Value);

                SendDataAlarmMonitoringProcessing(operateResult.IsSuccess);

                if (isSaveLog) SaveWriteLog(data, operateResult, log);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(message, ex);
            }
        }

        public void Write(CommunicationData<double> data, bool isSaveLog, log4net.ILog log)
        {
            if (!InitState) return;
            string message = data.Name;
            try
            {
                OperateResult operateResult = networkDeviceBase.Write(data.Address, data.Value);

                SendDataAlarmMonitoringProcessing(operateResult.IsSuccess);

                if (isSaveLog) SaveWriteLog(data, operateResult, log);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(message, ex);
            }
        }
        public void Write(CommunicationData<double[]> data, bool isSaveLog, log4net.ILog log)
        {
            if (!InitState) return;
            string message = data.Name;
            try
            {
                OperateResult operateResult = networkDeviceBase.Write(data.Address, data.Value);

                SendDataAlarmMonitoringProcessing(operateResult.IsSuccess);

                if (isSaveLog) SaveWriteLog(data, operateResult, log);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(message, ex);
            }
        }

        public void Write(CommunicationData<byte[]> data, bool isSaveLog, log4net.ILog log)
        {
            if (!InitState) return;
            string message = data.Name;
            try
            {
                OperateResult operateResult = networkDeviceBase.Write(data.Address, data.Value);

                SendDataAlarmMonitoringProcessing(operateResult.IsSuccess);

                if (isSaveLog) SaveWriteLog(data, operateResult, log);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(message, ex);
            }
        }

        public void Write(CommunicationData<string> data, int length, bool isSaveLog, log4net.ILog log)
        {
            if (!InitState) return;
            string message = data.Name;
            try
            {
                OperateResult operateResult = networkDeviceBase.Write(data.Address, data.Value, length);

                SendDataAlarmMonitoringProcessing(operateResult.IsSuccess);

                if (isSaveLog) SaveWriteLog(data, operateResult, log);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(message, ex);
            }
        }



        public bool Read(CommunicationData<bool> data, ReadLogSaveType readLogSaveType, log4net.ILog log)
        {
            if (!InitState) return false;
            string message = data.Name;
            bool result = false;
            try
            {
                OperateResult<bool> operateResult = networkDeviceBase.ReadBool(data.Address);

                ReadDataAlarmMonitoringProcessing(operateResult.IsSuccess);

                ReadResultProcessing(data, operateResult.IsSuccess, operateResult.Content);

                if (GetIsSaveReadLog(readLogSaveType, data.ReadDifferentStateFlag))
                {
                    SaveReadLog(data, operateResult.IsSuccess, operateResult.Message, log);
                }

                result = operateResult.IsSuccess;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(message, ex);
            }
            return result;
        }

        public bool Read(CommunicationData<bool[]> data, ushort length, ReadLogSaveType readLogSaveType, log4net.ILog log)
        {
            if (!InitState) return false;
            string message = data.Name;
            bool result = false;
            try
            {
                OperateResult<bool[]> operateResult = networkDeviceBase.ReadBool(data.Address, length);

                ReadDataAlarmMonitoringProcessing(operateResult.IsSuccess);

                ReadResultProcessing(data, operateResult.IsSuccess, operateResult.Content);

                if (GetIsSaveReadLog(readLogSaveType, data.ReadDifferentStateFlag))
                {
                    SaveReadLog(data, operateResult.IsSuccess, operateResult.Message, log);
                }

                result = operateResult.IsSuccess;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(message, ex);
            }
            return result;
        }

        public bool Read(CommunicationData<short> data, ReadLogSaveType readLogSaveType, log4net.ILog log)
        {
            if (!InitState) return false;
            string message = data.Name;
            bool result = false;
            try
            {
                OperateResult<short> operateResult = networkDeviceBase.ReadInt16(data.Address);

                ReadDataAlarmMonitoringProcessing(operateResult.IsSuccess);

                ReadResultProcessing(data, operateResult.IsSuccess, operateResult.Content);

                if (GetIsSaveReadLog(readLogSaveType, data.ReadDifferentStateFlag))
                {
                    SaveReadLog(data, operateResult.IsSuccess, operateResult.Message, log);
                }

                result = operateResult.IsSuccess;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(message, ex);
            }
            return result;
        }

        public bool Read(CommunicationData<short[]> data, ushort length, ReadLogSaveType readLogSaveType, log4net.ILog log)
        {
            if (!InitState) return false;
            string message = data.Name;
            bool result = false;
            try
            {
                OperateResult<short[]> operateResult = networkDeviceBase.ReadInt16(data.Address, length);

                ReadDataAlarmMonitoringProcessing(operateResult.IsSuccess);

                ReadResultProcessing(data, operateResult.IsSuccess, operateResult.Content);

                if (GetIsSaveReadLog(readLogSaveType, data.ReadDifferentStateFlag))
                {
                    SaveReadLog(data, operateResult.IsSuccess, operateResult.Message, log);
                }

                result = operateResult.IsSuccess;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(message, ex);
            }
            return result;
        }

        public bool Read(CommunicationData<int> data, ReadLogSaveType readLogSaveType, log4net.ILog log)
        {
            if (!InitState) return false;
            string message = data.Name;
            bool result = false;
            try
            {
                OperateResult<int> operateResult = networkDeviceBase.ReadInt32(data.Address);

                ReadDataAlarmMonitoringProcessing(operateResult.IsSuccess);

                ReadResultProcessing(data, operateResult.IsSuccess, operateResult.Content);

                if (GetIsSaveReadLog(readLogSaveType, data.ReadDifferentStateFlag))
                {
                    SaveReadLog(data, operateResult.IsSuccess, operateResult.Message, log);
                }

                result = operateResult.IsSuccess;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(message, ex);
            }
            return result;
        }

        public bool Read(CommunicationData<int[]> data, ushort length, ReadLogSaveType readLogSaveType, log4net.ILog log)
        {
            if (!InitState) return false;
            string message = data.Name;
            bool result = false;
            try
            {
                OperateResult<int[]> operateResult = networkDeviceBase.ReadInt32(data.Address, length);

                ReadDataAlarmMonitoringProcessing(operateResult.IsSuccess);

                ReadResultProcessing(data, operateResult.IsSuccess, operateResult.Content);

                if (GetIsSaveReadLog(readLogSaveType, data.ReadDifferentStateFlag))
                {
                    SaveReadLog(data, operateResult.IsSuccess, operateResult.Message, log);
                }

                result = operateResult.IsSuccess;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(message, ex);
            }
            return result;
        }

        public bool Read(CommunicationData<long> data, ReadLogSaveType readLogSaveType, log4net.ILog log)
        {
            if (!InitState) return false;
            string message = data.Name;
            bool result = false;
            try
            {
                OperateResult<long> operateResult = networkDeviceBase.ReadInt64(data.Address);

                ReadDataAlarmMonitoringProcessing(operateResult.IsSuccess);

                ReadResultProcessing(data, operateResult.IsSuccess, operateResult.Content);

                if (GetIsSaveReadLog(readLogSaveType, data.ReadDifferentStateFlag))
                {
                    SaveReadLog(data, operateResult.IsSuccess, operateResult.Message, log);
                }

                result = operateResult.IsSuccess;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(message, ex);
            }
            return result;
        }

        public bool Read(CommunicationData<long[]> data, ushort length, ReadLogSaveType readLogSaveType, log4net.ILog log)
        {
            if (!InitState) return false;
            string message = data.Name;
            bool result = false;
            try
            {
                OperateResult<long[]> operateResult = networkDeviceBase.ReadInt64(data.Address, length);

                ReadDataAlarmMonitoringProcessing(operateResult.IsSuccess);

                ReadResultProcessing(data, operateResult.IsSuccess, operateResult.Content);

                if (GetIsSaveReadLog(readLogSaveType, data.ReadDifferentStateFlag))
                {
                    SaveReadLog(data, operateResult.IsSuccess, operateResult.Message, log);
                }

                result = operateResult.IsSuccess;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(message, ex);
            }
            return result;
        }

        public bool Read(CommunicationData<ushort> data, ReadLogSaveType readLogSaveType, log4net.ILog log)
        {
            if (!InitState) return false;
            string message = data.Name;
            bool result = false;
            try
            {
                OperateResult<ushort> operateResult = networkDeviceBase.ReadUInt16(data.Address);

                ReadDataAlarmMonitoringProcessing(operateResult.IsSuccess);

                ReadResultProcessing(data, operateResult.IsSuccess, operateResult.Content);

                if (GetIsSaveReadLog(readLogSaveType, data.ReadDifferentStateFlag))
                {
                    SaveReadLog(data, operateResult.IsSuccess, operateResult.Message, log);
                }

                result = operateResult.IsSuccess;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(message, ex);
            }
            return result;
        }

        public bool Read(CommunicationData<ushort[]> data, ushort length, ReadLogSaveType readLogSaveType, log4net.ILog log)
        {
            if (!InitState) return false;
            string message = data.Name;
            bool result = false;
            try
            {
                OperateResult<ushort[]> operateResult = networkDeviceBase.ReadUInt16(data.Address, length);

                ReadDataAlarmMonitoringProcessing(operateResult.IsSuccess);

                ReadResultProcessing(data, operateResult.IsSuccess, operateResult.Content);

                if (GetIsSaveReadLog(readLogSaveType, data.ReadDifferentStateFlag))
                {
                    SaveReadLog(data, operateResult.IsSuccess, operateResult.Message, log);
                }

                result = operateResult.IsSuccess;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(message, ex);
            }
            return result;
        }

        public bool Read(CommunicationData<uint> data, ReadLogSaveType readLogSaveType, log4net.ILog log)
        {
            if (!InitState) return false;
            string message = data.Name;
            bool result = false;
            try
            {
                OperateResult<uint> operateResult = networkDeviceBase.ReadUInt32(data.Address);

                ReadDataAlarmMonitoringProcessing(operateResult.IsSuccess);

                ReadResultProcessing(data, operateResult.IsSuccess, operateResult.Content);

                if (GetIsSaveReadLog(readLogSaveType, data.ReadDifferentStateFlag))
                {
                    SaveReadLog(data, operateResult.IsSuccess, operateResult.Message, log);
                }

                result = operateResult.IsSuccess;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(message, ex);
            }
            return result;
        }

        public bool Read(CommunicationData<uint[]> data, ushort length, ReadLogSaveType readLogSaveType, log4net.ILog log)
        {
            if (!InitState) return false;
            string message = data.Name;
            bool result = false;
            try
            {
                OperateResult<uint[]> operateResult = networkDeviceBase.ReadUInt32(data.Address, length);

                ReadDataAlarmMonitoringProcessing(operateResult.IsSuccess);

                ReadResultProcessing(data, operateResult.IsSuccess, operateResult.Content);

                if (GetIsSaveReadLog(readLogSaveType, data.ReadDifferentStateFlag))
                {
                    SaveReadLog(data, operateResult.IsSuccess, operateResult.Message, log);
                }

                result = operateResult.IsSuccess;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(message, ex);
            }
            return result;
        }

        public bool Read(CommunicationData<ulong> data, ReadLogSaveType readLogSaveType, log4net.ILog log)
        {
            if (!InitState) return false;
            string message = data.Name;
            bool result = false;
            try
            {
                OperateResult<ulong> operateResult = networkDeviceBase.ReadUInt64(data.Address);

                ReadDataAlarmMonitoringProcessing(operateResult.IsSuccess);

                ReadResultProcessing(data, operateResult.IsSuccess, operateResult.Content);

                if (GetIsSaveReadLog(readLogSaveType, data.ReadDifferentStateFlag))
                {
                    SaveReadLog(data, operateResult.IsSuccess, operateResult.Message, log);
                }

                result = operateResult.IsSuccess;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(message, ex);
            }
            return result;
        }

        public bool Read(CommunicationData<ulong[]> data, ushort length, ReadLogSaveType readLogSaveType, log4net.ILog log)
        {
            if (!InitState) return false;
            string message = data.Name;
            bool result = false;
            try
            {
                OperateResult<ulong[]> operateResult = networkDeviceBase.ReadUInt64(data.Address, length);

                ReadDataAlarmMonitoringProcessing(operateResult.IsSuccess);

                ReadResultProcessing(data, operateResult.IsSuccess, operateResult.Content);

                if (GetIsSaveReadLog(readLogSaveType, data.ReadDifferentStateFlag))
                {
                    SaveReadLog(data, operateResult.IsSuccess, operateResult.Message, log);
                }

                result = operateResult.IsSuccess;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(message, ex);
            }
            return result;
        }

        public bool Read(CommunicationData<float> data, ReadLogSaveType readLogSaveType, log4net.ILog log)
        {
            if (!InitState) return false;
            string message = data.Name;
            bool result = false;
            try
            {
                OperateResult<float> operateResult = networkDeviceBase.ReadFloat(data.Address);

                ReadDataAlarmMonitoringProcessing(operateResult.IsSuccess);

                ReadResultProcessing(data, operateResult.IsSuccess, operateResult.Content);

                if (GetIsSaveReadLog(readLogSaveType, data.ReadDifferentStateFlag))
                {
                    SaveReadLog(data, operateResult.IsSuccess, operateResult.Message, log);
                }

                result = operateResult.IsSuccess;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(message, ex);
            }
            return result;
        }

        public bool Read(CommunicationData<float[]> data, ushort length, ReadLogSaveType readLogSaveType, log4net.ILog log)
        {
            if (!InitState) return false;
            string message = data.Name;
            bool result = false;
            try
            {
                OperateResult<float[]> operateResult = networkDeviceBase.ReadFloat(data.Address, length);

                ReadDataAlarmMonitoringProcessing(operateResult.IsSuccess);

                ReadResultProcessing(data, operateResult.IsSuccess, operateResult.Content);

                if (GetIsSaveReadLog(readLogSaveType, data.ReadDifferentStateFlag))
                {
                    SaveReadLog(data, operateResult.IsSuccess, operateResult.Message, log);
                }

                result = operateResult.IsSuccess;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(message, ex);
            }
            return result;
        }

        public bool Read(CommunicationData<double> data, ReadLogSaveType readLogSaveType, log4net.ILog log)
        {
            if (!InitState) return false;
            string message = data.Name;
            bool result = false;
            try
            {
                OperateResult<double> operateResult = networkDeviceBase.ReadDouble(data.Address);

                ReadDataAlarmMonitoringProcessing(operateResult.IsSuccess);

                ReadResultProcessing(data, operateResult.IsSuccess, operateResult.Content);

                if (GetIsSaveReadLog(readLogSaveType, data.ReadDifferentStateFlag))
                {
                    SaveReadLog(data, operateResult.IsSuccess, operateResult.Message, log);
                }

                result = operateResult.IsSuccess;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(message, ex);
            }
            return result;
        }

        public bool Read(CommunicationData<double[]> data, ushort length, ReadLogSaveType readLogSaveType, log4net.ILog log)
        {
            if (!InitState) return false;
            string message = data.Name;
            bool result = false;
            try
            {
                OperateResult<double[]> operateResult = networkDeviceBase.ReadDouble(data.Address, length);

                ReadDataAlarmMonitoringProcessing(operateResult.IsSuccess);

                ReadResultProcessing(data, operateResult.IsSuccess, operateResult.Content);

                if (GetIsSaveReadLog(readLogSaveType, data.ReadDifferentStateFlag))
                {
                    SaveReadLog(data, operateResult.IsSuccess, operateResult.Message, log);
                }

                result = operateResult.IsSuccess;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(message, ex);
            }
            return result;
        }

        public bool Read(CommunicationData<string> data, ushort length, ReadLogSaveType readLogSaveType, log4net.ILog log)
        {
            if (!InitState) return false;
            string message = data.Name;
            bool result = false;
            try
            {
                OperateResult<string> operateResult = networkDeviceBase.ReadString(data.Address, length);

                ReadDataAlarmMonitoringProcessing(operateResult.IsSuccess);

                operateResult.Content = operateResult.Content.Trim(new char[] { '\0' });
                operateResult.Content = operateResult.Content.Trim(new char[] { ' ' });

                ReadResultProcessing(data, operateResult.IsSuccess, operateResult.Content);

              

                if (GetIsSaveReadLog(readLogSaveType, data.ReadDifferentStateFlag))
                {
                    SaveReadLog(data, operateResult.IsSuccess, operateResult.Message, log);
                }

                result = operateResult.IsSuccess;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(message, ex);
            }
            return result;
        }


        /// <summary>
        /// 读取非零值并写入零值
        /// </summary>
        /// <param name="data"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        public bool ReadAndWriteZero(CommunicationData<short> data, log4net.ILog log, out short readValue)
        {
            readValue = 0;
            if (!InitState) return false;
            string message = data.Name;
            bool result = false;
            try
            {
                OperateResult<short> operateResult = networkDeviceBase.ReadInt16(data.Address);
                readValue = operateResult.Content;
                ReadDataAlarmMonitoringProcessing(operateResult.IsSuccess);

                ReadResultProcessing(data, operateResult.IsSuccess, operateResult.Content);

                if (operateResult.IsSuccess && operateResult.Content != 0)
                {
                    //保存日志
                    SaveReadLog(data, operateResult.IsSuccess, operateResult.Message, log);

                    // 写入零值
                    data.Value = 0;
                    Write(data, true, log);
                    data.Value = operateResult.Content;//返回读取到的原值
                }

                result = operateResult.IsSuccess;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(message, ex);
            }
            return result;
        }
        /// <summary>
        /// 读取非零值并写入零值
        /// </summary>
        /// <param name="data"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        public bool ReadAndWriteZero(CommunicationData<short> data, log4net.ILog log, bool isSaveLog, out short readValue)
        {
            readValue = 0;
            if (!InitState) return false;
            string message = data.Name;
            bool result = false;
            try
            {
                OperateResult<short> operateResult = networkDeviceBase.ReadInt16(data.Address);
                readValue = operateResult.Content;
                ReadDataAlarmMonitoringProcessing(operateResult.IsSuccess);

                ReadResultProcessing(data, operateResult.IsSuccess, operateResult.Content);

                if ((operateResult.IsSuccess && operateResult.Content != 0)||
                    operateResult.IsSuccess && isSaveLog)
                {
                    //保存日志
                    SaveReadLog(data, operateResult.IsSuccess, operateResult.Message, log);

                    // 写入零值
                    data.Value = 0;
                    Write(data, true, log);
                    data.Value = operateResult.Content;//返回读取到的原值
                }

                result = operateResult.IsSuccess;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(message, ex);
            }
            return result;
        }

        /// <summary>
        /// 读取非零值并写入零值
        /// </summary>
        /// <param name="data"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        public bool ReadAndWriteZero(CommunicationData<float> data, log4net.ILog log, out float readValue)
        {
            readValue = 0;
            if (!InitState) return false;
            string message = data.Name;
            bool result = false;
            try
            {
                OperateResult<float> operateResult = networkDeviceBase.ReadFloat(data.Address);
                readValue = operateResult.Content;
                ReadDataAlarmMonitoringProcessing(operateResult.IsSuccess);

                ReadResultProcessing(data, operateResult.IsSuccess, operateResult.Content);

                if (operateResult.IsSuccess && operateResult.Content != 0)
                {
                    //保存日志
                    SaveReadLog(data, operateResult.IsSuccess, operateResult.Message, log);

                    // 写入零值
                    data.Value = 0;
                    Write(data, true, log);
                    data.Value = operateResult.Content;//返回读取到的原值
                }

                result = operateResult.IsSuccess;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(message, ex);
            }
            return result;
        }

        /// <summary>
        /// 读取非零值并写入零值
        /// </summary>
        /// <param name="data"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        public bool ReadAndWriteZero(CommunicationData<short>[] data, log4net.ILog log, out short[] shorts)
        {
            shorts = new short[data.Length];
            if (!InitState) return false;
            string message = "";
            bool result = false;
            try
            {
                message = data[0].Name;
                OperateResult<short[]> operateResult = networkDeviceBase.ReadInt16(data[0].Address, (ushort)data.Length);
                shorts = operateResult.Content;
                ReadDataAlarmMonitoringProcessing(operateResult.IsSuccess);

                for (int i = 0; i < data.Length; i++)
                {
                    ReadResultProcessing(data[i], i == 0 ? operateResult.IsSuccess : true, operateResult.Content[i]);
                }

                if (operateResult.IsSuccess)
                {
                    if (data.Where(t => t.Value != 0).Count() > 0)
                    {
                        //保存日志
                        SaveReadLog(data, operateResult.IsSuccess, operateResult.Message, log);

                        // 写入零值
                        for (int i = 0; i < data.Length; i++)
                        {
                            if (data[i].Value != 0)
                            {
                                data[i].Value = 0;
                                Write(data[i], true, log);
                                data[i].Value = operateResult.Content[i];//返回读取到的原值
                            }
                        }
                    }
                }

                result = operateResult.IsSuccess;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(message, ex);
            }
            return result;
        }
    }
}

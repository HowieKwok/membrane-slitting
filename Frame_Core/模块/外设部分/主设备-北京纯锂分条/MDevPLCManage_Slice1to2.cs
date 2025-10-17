using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HslCommunication;
using HslCommunication.Core.Device;
using HslCommunication.Core.Net;
using HslCommunication.Profinet.Omron;
using HslCommunication.Robot.EFORT;
using LogTool;
using NPOI.SS.Formula.Functions;
using NPOI.XSSF.Streaming.Values;

namespace Frame_Core
{
    /// <summary>
    /// 主设备PLC管理类(通讯类请参考吨袋通讯)
    /// </summary>
    public class MDevPLCManage_Slice1to2
    {
        FileOperationINI log = new FileOperationINI();
        /// <summary>
        /// 读取信息路径
        /// </summary>
        public string ReadLogPath = $"E:\\检测数据\\NewProduct\\主设备PLC日志记录-接收.ini";

        /// <summary>
        /// 发送信息路径
        /// </summary>
        public string SendLogPath = $"E:\\检测数据\\NewProduct\\主设备PLC日志记录-发送.ini";
        /// <summary>
        /// 发送心跳路径工控机1
        /// </summary>
        public string SendHeartPath1 = $"E:\\检测数据\\NewProduct\\主设备PLC日志记录-心跳1.ini";

        /// <summary>
        /// 发送纠偏数据路径
        /// </summary>
        public string SendCorrevtDataPath = $"E:\\检测数据\\NewProduct\\主设备PLC日志记录-纠偏.ini";

        public CorrectTool correctTool = new CorrectTool();
        #region 参数
        /// <summary>
        /// PLC地址
        /// </summary>
        public MDevAddress_Slice1to2 m_DevAddress = new MDevAddress_Slice1to2();

        /// <summary>
        /// PLC参数
        /// </summary>
        public MDevParam_Slice1to2 m_DevParam = null;

        /// <summary>
        /// 加载模块参数
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool LoadParam(string path)
        {
            bool result = false;
            try
            {
                if (m_DevParam == null) m_DevParam = new MDevParam_Slice1to2();
                m_DevParam.filePath = path;
                m_DevParam = (MDevParam_Slice1to2)m_DevParam.Read();
                m_DevParam.filePath = path;
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                MessageBox.Show(ex.ToString());
            }
            return result;
        }


        #endregion


        #region PLC控制

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
        private void ReadDataAlarmMonitoring()
        {
            if (ReadFailCount > ReadFailNum)
            {
                Disconnect();
                TJBJInfo.AlarmWriteState(AlarmName.主设备连接失败, new StopAlarmMessage() { StopAlarmState = StopAlarmStateEnum.停机, describe = "读取数据失败次数过多" });
            }
        }

        /// <summary>
        /// 发送失败警告监控
        /// </summary>
        private void SendDataAlarmMonitoring()
        {
            if (WriteFailCount > WriteFailNum)
            {
                Disconnect();
                TJBJInfo.AlarmWriteState(AlarmName.主设备连接失败, new StopAlarmMessage() { StopAlarmState = StopAlarmStateEnum.停机, describe = "发送数据失败次数过多" });
            }
        }
        public bool Connect()
        {
            try
            {
                if (!m_DevParam.IsConnect) return false;
                
                uint plcType = (uint)m_DevParam.ComType;
                string ip = m_DevParam.PLC_IP;
                int port = m_DevParam.DevPort;

                Disconnect();
                networkDeviceBase = new OmronFinsNet(ip, port);
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

        DateTime Heart_CCD1 = DateTime.Now;
        /// <summary>
        /// 写入心跳数据
        /// </summary>
        public void Write_Heart()
        {
            try
            {
                if (!InitState) return;
                DateTime date1 = DateTime.Now;
                if (date1.Subtract(Heart_CCD1).TotalMilliseconds > 1000)
                {
                    Heart_CCD1 = date1;
                    ShortData shortData = m_DevAddress.Heart_CCD1;
                    shortData.value = 1;
                    OperateResult operateResult = networkDeviceBase.Write(shortData.address, shortData.value);

                    string message = shortData.name;
                    if (operateResult.IsSuccess)
                    {
                        message += $"写入成功-地址：[{shortData.address}]-值：[{shortData.value}]";
                    }
                    else
                    {
                        WriteFailCount++;
                        message += $"写入失败-地址：[{shortData.address}]-值：[{shortData.value}]";
                    }

                    SendDataAlarmMonitoring();
                    log.Write_SendLog_ini(SendHeartPath1, message, operateResult.Message);

                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
            }
           
        }

        /// <summary>
        /// 写入NG报警信号
        /// </summary>
        public void Write_NGAlarm()
        {
            try
            {
                if (!InitState) return;
                ShortData shortData = m_DevAddress.NGAlarm;
      
                shortData.value = 1;
                OperateResult operateResult = networkDeviceBase.Write(shortData.address, shortData.value);

                string message = shortData.name;
                if (operateResult.IsSuccess)
                {
                    message += $"写入成功-地址：[{shortData.address}]-值：[{shortData.value}]";
                }
                else
                {
                    WriteFailCount++;
                    message += $"写入失败-地址：[{shortData.address}]-值：[{shortData.value}]";
                }

                SendDataAlarmMonitoring();
                log.Write_SendLog_ini(SendLogPath, message, operateResult.Message);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
            }
          

           
        }

        /// <summary>
        /// 写入NG停机信号
        /// </summary>
        public void Write_NGHalt()
        {
            try
            {
                if (!InitState) return;
                ShortData shortData = m_DevAddress.NGHalt;
     
                shortData.value = 1;
                OperateResult operateResult = networkDeviceBase.Write(shortData.address, shortData.value);

                string message = shortData.name;
                if (operateResult.IsSuccess)
                {
                    message += $"写入成功-地址：[{shortData.address}]-值：[{shortData.value}]";
                }
                else
                {
                    WriteFailCount++;
                    message += $"写入失败-地址：[{shortData.address}]-值：[{shortData.value}]";
                }

                SendDataAlarmMonitoring();
                log.Write_SendLog_ini(SendLogPath, message, operateResult.Message);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
            }
          

          
        }

        /// <summary>
        /// 写入编码轮压下或抬起信号（抬起信号为1，下压信号为2）
        /// </summary>
        public void Write_CodingWheelCylinder(bool signal)
        {
            try
            {
                if (!InitState) return;
                ShortData shortData = m_DevAddress.CodingWheelCylinder;

                shortData.value = signal ? (short)2 : (short)1;
                OperateResult operateResult = networkDeviceBase.Write(shortData.address, shortData.value);

                string message = shortData.name;
                if (operateResult.IsSuccess)
                {
                    message += $"写入成功-地址：[{shortData.address}]-值：[{shortData.value}]";
                }
                else
                {
                    WriteFailCount++;
                    message += $"写入失败-地址：[{shortData.address}]-值：[{shortData.value}]";
                }

                SendDataAlarmMonitoring();
                log.Write_SendLog_ini(SendLogPath, message, operateResult.Message);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
            }
         

         
        }

        /// <summary>
        /// 写入报警复位
        /// </summary>
        public void Write_ResetAlarm()
        {
            try
            {
                if (!InitState) return;
                ShortData shortData = m_DevAddress.ResetAlarm;


                shortData.value = 1;
                OperateResult operateResult = networkDeviceBase.Write(shortData.address, shortData.value);

                string message = shortData.name;
                if (operateResult.IsSuccess)
                {
                    message += $"写入成功-地址：[{shortData.address}]-值：[{shortData.value}]";
                }
                else
                {
                    WriteFailCount++;
                    message += $"写入失败-地址：[{shortData.address}]-值：[{shortData.value}]";
                }

                SendDataAlarmMonitoring();
                log.Write_SendLog_ini(SendLogPath, message, operateResult.Message);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
            }
          

         
        }

        /// <summary>
        /// 写入新建膜卷信号
        /// </summary>
        public void Write_NewFilmRoll()
        {
            try
            {
                if (!InitState) return;
                ShortData shortData = m_DevAddress.NewFilmRoll;
                shortData.value = 1;
                OperateResult operateResult = networkDeviceBase.Write(shortData.address, shortData.value);

                string message = shortData.name;
                if (operateResult.IsSuccess)
                {
                    message += $"写入成功-地址：[{shortData.address}]-值：[{shortData.value}]";
                }
                else
                {
                    WriteFailCount++;
                    message += $"写入失败-地址：[{shortData.address}]-值：[{shortData.value}]";
                }

                SendDataAlarmMonitoring();
                log.Write_SendLog_ini(SendLogPath, message, operateResult.Message);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
            }
          

          
        }

        /// <summary>
        /// 写入运行/暂停信号
        /// </summary>
        public void Write_RunOrPause(bool isRun)
        {
            try
            {
                if (!InitState) return;
                ShortData shortData = m_DevAddress.RunOrPause;
                shortData.value = isRun ? (short)1 : (short)0;
                OperateResult operateResult = networkDeviceBase.Write(shortData.address, shortData.value);

                string message = shortData.name;
                if (operateResult.IsSuccess)
                {
                    message += $"写入成功-地址：[{shortData.address}]-值：[{shortData.value}]";
                }
                else
                {
                    WriteFailCount++;
                    message += $"写入失败-地址：[{shortData.address}]-值：[{shortData.value}]";
                }

                SendDataAlarmMonitoring();
                log.Write_SendLog_ini(SendLogPath, message, operateResult.Message);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
            }
          

          
        }

        /// <summary>
        /// 写入编码轮直径
        /// </summary>
        public void Write_EncoderDiameter(short value)
        {
            try
            {
                if (!InitState) return;
                ShortData shortData = m_DevAddress.EncoderDiameter;
                shortData.value = value;
                OperateResult operateResult = networkDeviceBase.Write(shortData.address, shortData.value);

                string message = shortData.name;
                if (operateResult.IsSuccess)
                {
                    message += $"写入成功-地址：[{shortData.address}]-值：[{shortData.value}]";
                }
                else
                {
                    WriteFailCount++;
                    message += $"写入失败-地址：[{shortData.address}]-值：[{shortData.value}]";
                }

                SendDataAlarmMonitoring();
                log.Write_SendLog_ini(SendLogPath, message, operateResult.Message);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
            }
         

           
        }

        /// <summary>
        /// 写入编码轮脉冲
        /// </summary>
        public void Write_EncoderPulse(short value)
        {
            try
            {
                if (!InitState) return;
                ShortData shortData = m_DevAddress.EncoderPulse;
                shortData.value = value;
                OperateResult operateResult = networkDeviceBase.Write(shortData.address, shortData.value);

                string message = shortData.name;
                if (operateResult.IsSuccess)
                {
                    message += $"写入成功-地址：[{shortData.address}]-值：[{shortData.value}]";
                }
                else
                {
                    WriteFailCount++;
                    message += $"写入失败-地址：[{shortData.address}]-值：[{shortData.value}]";
                }

                SendDataAlarmMonitoring();
                log.Write_SendLog_ini(SendLogPath, message, operateResult.Message);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
            }
          

           
        }

        /// <summary>
        /// 写入纠偏使能
        /// </summary>
        public void Write_CorrectEn(bool value)
        {
            try
            {
                if (!InitState) return;
                ShortData shortData = m_DevAddress.CorrectEn;
                shortData.value = value ? (short)1 : (short)0;
                OperateResult operateResult = networkDeviceBase.Write(shortData.address, shortData.value);

                string message = shortData.name;
                if (operateResult.IsSuccess)
                {
                    message += $"写入成功-地址：[{shortData.address}]-值：[{shortData.value}]";
                }
                else
                {
                    WriteFailCount++;
                    message += $"写入失败-地址：[{shortData.address}]-值：[{shortData.value}]";
                }

                SendDataAlarmMonitoring();
                log.Write_SendLog_ini(SendLogPath, message, operateResult.Message);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
            }
          

          
        }

        /// <summary>
        /// 写入手自动纠偏切换,手动写(SD)写0，自动(ZD)写1
        /// </summary>
        public void Write_CorrectManualAuto(bool value)
        {
            try
            {
                if (!InitState) return;
                ShortData shortData = m_DevAddress.CorrectManualAuto;
                shortData.value = value ? (short)1 : (short)0;
                OperateResult operateResult = networkDeviceBase.Write(shortData.address, shortData.value);

                string message = shortData.name;
                if (operateResult.IsSuccess)
                {
                    message += $"写入成功-地址：[{shortData.address}]-值：[{shortData.value}]";
                }
                else
                {
                    WriteFailCount++;
                    message += $"写入失败-地址：[{shortData.address}]-值：[{shortData.value}]";
                }

                SendDataAlarmMonitoring();
                log.Write_SendLog_ini(SendLogPath, message, operateResult.Message);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
            }
         

          
        }

        /// <summary>
        /// 写入纠偏速度
        /// </summary>
        public void Write_CorrectSleep(short value)
        {
            try
            {
                if (!InitState) return;
                ShortData shortData = m_DevAddress.CorrectSleep;
                shortData.value = value;
                OperateResult operateResult = networkDeviceBase.Write(shortData.address, shortData.value);

                string message = shortData.name;
                if (operateResult.IsSuccess)
                {
                    message += $"写入成功-地址：[{shortData.address}]-值：[{shortData.value}]";
                }
                else
                {
                    WriteFailCount++;
                    message += $"写入失败-地址：[{shortData.address}]-值：[{shortData.value}]";
                }

                SendDataAlarmMonitoring();
                log.Write_SendLog_ini(SendLogPath, message, operateResult.Message);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
            }
           

         
        }

        /// <summary>
        /// 写入纠偏回中
        /// </summary>
        public void Write_CorrectToHome()
        {
            try
            {
                if (!InitState) return;
                ShortData shortData = m_DevAddress.CorrectToHome;
                shortData.value = 1;
                OperateResult operateResult = networkDeviceBase.Write(shortData.address, shortData.value);

                string message = shortData.name;
                if (operateResult.IsSuccess)
                {
                    message += $"写入成功-地址：[{shortData.address}]-值：[{shortData.value}]";
                }
                else
                {
                    WriteFailCount++;
                    message += $"写入失败-地址：[{shortData.address}]-值：[{shortData.value}]";
                }

                SendDataAlarmMonitoring();
                log.Write_SendLog_ini(SendLogPath, message, operateResult.Message);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
            }
            

           
        }

        /// <summary>
        /// 写入手动纠偏量
        /// </summary>
        public void Write_CorrectManualValue(float value)
        {
            try
            {
                if (!InitState) return;
                Write_CorrectManualAuto(false);
                FloatData shortData = m_DevAddress.CorrectManualValue;
                shortData.value = value;
                OperateResult operateResult = networkDeviceBase.Write(shortData.address, shortData.value);

                string message = shortData.name;
                if (operateResult.IsSuccess)
                {
                    message += $"写入成功-地址：[{shortData.address}]-值：[{shortData.value}]";
                }
                else
                {
                    WriteFailCount++;
                    message += $"写入失败-地址：[{shortData.address}]-值：[{shortData.value}]";
                }

                SendDataAlarmMonitoring();
                log.Write_SendLog_ini(SendLogPath, message, operateResult.Message);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
            }
          

          
        }

        /// <summary>
        /// 写入自动纠偏量
        /// </summary>
        public void Write_CorrectAutoValue()
        {
            try
            {
                if (!InitState) return;
                          
                FloatData shortData = m_DevAddress.CorrectAutoValue;
              
                if (!shortData.execute) return;
                shortData.execute = false;
                Write_CorrectManualAuto(true);
                //shortData.value = value;
                OperateResult operateResult = networkDeviceBase.Write(shortData.address, shortData.value);

                string message = shortData.name;
                if (operateResult.IsSuccess)
                {
                    message += $"写入成功-地址：[{shortData.address}]-值：[{shortData.value}]";
                }
                else
                {
                    WriteFailCount++;
                    message += $"写入失败-地址：[{shortData.address}]-值：[{shortData.value}]";
                }

                SendDataAlarmMonitoring();
                log.Write_SendLog_ini(SendLogPath, message, operateResult.Message);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
            }
        

          
        }
     

        /// <summary>
        /// 读取采图不正常报警停机
        /// </summary>
        public void Read_AcquisitionImageException()
        {
            try
            {
                if (!InitState) return;
                ShortData shortData = m_DevAddress.AcquisitionImageException;
                OperateResult<short> operateResult = networkDeviceBase.ReadInt16(shortData.address);
                shortData.value = operateResult.Content;


                string message = shortData.name;
                if (operateResult.IsSuccess)
                {
                    message += $"读取成功-地址：[{shortData.address}]-值：[{shortData.value}]";
                }
                else
                {
                    ReadFailCount++;
                    message += $"读取失败-地址：[{shortData.address}]-值：[{shortData.value}]";
                }


                if (operateResult.Content == 1)
                {
                    ReadDataAlarmMonitoring();
                    log.Write_ReadLog_ini(ReadLogPath, message, operateResult.Message);

                    TJBJInfo.AlarmWriteState(AlarmName.采图不正常停机报警, new StopAlarmMessage() { StopAlarmState = StopAlarmStateEnum.停机 });
                    OperateResult operateResult1 = networkDeviceBase.Write(shortData.address, (short)0);
                    if (operateResult1.IsSuccess)
                    {
                        message += $"写入成功-地址：[{shortData.address}]-值：[{shortData.value}]";
                    }
                    else
                    {
                        WriteFailCount++;
                        message += $"写入失败-地址：[{shortData.address}]-值：[{shortData.value}]";
                    }
                    SendDataAlarmMonitoring();
                    log.Write_ReadLog_ini(ReadLogPath, message, operateResult1.Message);
                }

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
            }
         
        

        }

        /// <summary>
        /// 读取PLC编码器无脉冲信号报警
        /// </summary>
        public void Read_PLCNotPulseSignal()
        {
            try
            {
                if (!InitState) return;
                ShortData shortData = m_DevAddress.PLCNotPulseSignal;
                OperateResult<short> operateResult = networkDeviceBase.ReadInt16(shortData.address);
                shortData.value = operateResult.Content;


                string message = shortData.name;
                if (operateResult.IsSuccess)
                {
                    message += $"读取成功-地址：[{shortData.address}]-值：[{shortData.value}]";
                }
                else
                {
                    ReadFailCount++;
                    message += $"读取失败-地址：[{shortData.address}]-值：[{shortData.value}]";
                }


                if (operateResult.Content == 1)
                {
                    ReadDataAlarmMonitoring();
                    log.Write_ReadLog_ini(ReadLogPath, message, operateResult.Message);

                    TJBJInfo.AlarmWriteState(AlarmName.PLC编码器无脉冲信号报警, new StopAlarmMessage() { StopAlarmState = StopAlarmStateEnum.停机 });
                    OperateResult operateResult1 = networkDeviceBase.Write(shortData.address, (short)0);
                    if (operateResult1.IsSuccess)
                    {
                        message += $"写入成功-地址：[{shortData.address}]-值：[{shortData.value}]";
                    }
                    else
                    {
                        WriteFailCount++;
                        message += $"写入失败-地址：[{shortData.address}]-值：[{shortData.value}]";
                    }
                    SendDataAlarmMonitoring();
                    log.Write_ReadLog_ini(ReadLogPath, message, operateResult1.Message);
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
            }
          

           
        }

        /// <summary>
        /// 读取纠偏伺服报警
        /// </summary>
        public void Read_CorrectServoAlarm()
        {
            try
            {
                if (!InitState) return;
                ShortData shortData = m_DevAddress.CorrectServoAlarm;
                OperateResult<short> operateResult = networkDeviceBase.ReadInt16(shortData.address);
                shortData.value = operateResult.Content;



                string message = shortData.name;
                if (operateResult.IsSuccess)
                {
                    message += $"读取成功-地址：[{shortData.address}]-值：[{shortData.value}]";
                }
                else
                {
                    ReadFailCount++;
                    message += $"读取失败-地址：[{shortData.address}]-值：[{shortData.value}]";
                }


                if (operateResult.Content == 1)
                {
                    ReadDataAlarmMonitoring();
                    log.Write_ReadLog_ini(ReadLogPath, message, operateResult.Message);

                    TJBJInfo.AlarmWriteState(AlarmName.纠编闭环伺服故障报警, new StopAlarmMessage() { StopAlarmState = StopAlarmStateEnum.停机 });
                    OperateResult operateResult1 = networkDeviceBase.Write(shortData.address, (short)0);
                    if (operateResult1.IsSuccess)
                    {
                        message += $"写入成功-地址：[{shortData.address}]-值：[{shortData.value}]";
                    }
                    else
                    {
                        WriteFailCount++;
                        message += $"写入失败-地址：[{shortData.address}]-值：[{shortData.value}]";
                    }
                    SendDataAlarmMonitoring();
                    log.Write_ReadLog_ini(ReadLogPath, message, operateResult1.Message);
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
            }
           

         
        }

        /// <summary>
        /// 读取编码轮未压到位报警
        /// </summary>
        public void Read_CodingWheelAlarm()
        {
            try
            {
                if (!InitState) return;
                ShortData shortData = m_DevAddress.CodingWheelAlarm;
                OperateResult<short> operateResult = networkDeviceBase.ReadInt16(shortData.address);
                shortData.value = operateResult.Content;



                string message = shortData.name;
                if (operateResult.IsSuccess)
                {
                    message += $"读取成功-地址：[{shortData.address}]-值：[{shortData.value}]";
                }
                else
                {
                    ReadFailCount++;
                    message += $"读取失败-地址：[{shortData.address}]-值：[{shortData.value}]";
                }


                if (operateResult.Content == 1)
                {
                    ReadDataAlarmMonitoring();
                    log.Write_ReadLog_ini(ReadLogPath, message, operateResult.Message);

                    TJBJInfo.AlarmWriteState(AlarmName.编码轮未压到位报警, new StopAlarmMessage() { StopAlarmState = StopAlarmStateEnum.停机 });
                    OperateResult operateResult1 = networkDeviceBase.Write(shortData.address, (short)0);
                    if (operateResult1.IsSuccess)
                    {
                        message += $"写入成功-地址：[{shortData.address}]-值：[{shortData.value}]";
                    }
                    else
                    {
                        WriteFailCount++;
                        message += $"写入失败-地址：[{shortData.address}]-值：[{shortData.value}]";
                    }
                    SendDataAlarmMonitoring();
                    log.Write_ReadLog_ini(ReadLogPath, message, operateResult1.Message);
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
            }
           

          
        }

        /// <summary>
        /// 读取纠偏极限位置报警
        /// </summary>
        public void Read_CorrectLimitingPositionAlarm()
        {
            try
            {
                if (!InitState) return;
                ShortData shortData = m_DevAddress.CorrectLimitingPositionAlarm;
                OperateResult<short> operateResult = networkDeviceBase.ReadInt16(shortData.address);
                shortData.value = operateResult.Content;



                string message = shortData.name;
                if (operateResult.IsSuccess)
                {
                    message += $"读取成功-地址：[{shortData.address}]-值：[{shortData.value}]";
                }
                else
                {
                    ReadFailCount++;
                    message += $"读取失败-地址：[{shortData.address}]-值：[{shortData.value}]";
                }




                if (operateResult.Content == 1)
                {
                    ReadDataAlarmMonitoring();
                    ReadDataAlarmMonitoring();
                    log.Write_ReadLog_ini(ReadLogPath, message, operateResult.Message);

                    TJBJInfo.AlarmWriteState(AlarmName.纠偏极限位置报警, new StopAlarmMessage() { StopAlarmState = StopAlarmStateEnum.停机 });
                    OperateResult operateResult1 = networkDeviceBase.Write(shortData.address, (short)0);
                    if (operateResult1.IsSuccess)
                    {
                        message += $"写入成功-地址：[{shortData.address}]-值：[{shortData.value}]";
                    }
                    else
                    {
                        WriteFailCount++;
                        message += $"写入失败-地址：[{shortData.address}]-值：[{shortData.value}]";
                    }
                    SendDataAlarmMonitoring();
                    log.Write_ReadLog_ini(ReadLogPath, message, operateResult1.Message);
                }

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
            }
           
         
        }
        #endregion
    }
}

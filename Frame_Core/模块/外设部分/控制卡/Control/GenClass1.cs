using Extensions;
using LogTool;
using Frame_Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EHolly
{
    public class GenClass1
    {
        public delegate void ReceiveEventHandler(string eventName, int Str, bool ObjparametersArray);
        public event ReceiveEventHandler ReEvent;//主界面触发使用
        short sRtn, nCore = 1, nAxis = 1;
        //public double K = 1.0 / 5000.0;

        /// <summary>
        /// 轴状态
        /// </summary>
        public Axis_State axis_State = new Axis_State();
        void LogMessage(string eventName, int parametersArray, bool ObjparametersArray)
        {
            if (ReEvent != null)
                ReEvent.BeginInvoke(eventName, parametersArray, ObjparametersArray, null, null);
        }
        /// <summary>
        /// 初始化板卡
        /// </summary>
        public bool Init_Control()
        {
            short sEcatSts;
            bool init = false;
            try
            {
                // 打开运动控制器
                sRtn = GTN.mc.GTN_Open(5, 2);
                LogMessage("error:GTN_Open", sRtn, true);

                // 中断EtherCAT通讯
                sRtn = GTN.mc.GTN_TerminateEcatComm(nCore);
                //LogMessage("error:GTN_TerminateEcatComm", sRtn, true);

                // 初始化EtherCAT总线
                if (sRtn != -6)
                    sRtn = GTN.mc.GTN_InitEcatComm(nCore);
                LogMessage("error: " + sRtn + "   GTN_InitEcatComm: -8 配置错误，请检查 eni 配置文件和实际连接从站是否匹配； -9未找到 eni 配置文件，请将 eni 文件放到可执行程序的相同目录； -10 未找到从站，请检查从站接线是否稳固", sRtn, false);
                // 等待EtherCAT总线初始化成功
                //LogMessage("EtherCat Is Connecting...", 1, false);
                int count = 0;
                while (true)
                {
                    if (sRtn == -10) { break; }
                    // 读取EtherCAT总线状态
                    sRtn = GTN.mc.GTN_IsEcatReady(nCore, out sEcatSts);
                    LogMessage("EtherCat Is Connecting...", 1, false);
                    Thread.Sleep(100);

                    count++;
                    if (sEcatSts == 1)
                    {
                        init = true;
                        LogMessage("EtherCat Connect Success!", 1, false);
                        // 启动EtherCAT通讯
                        sRtn = GTN.mc.GTN_StartEcatComm(nCore);
                        LogMessage("error:GTN_StartEcatComm", sRtn, true);
                        break;
                    }
                    else if (count > 10 * 30)//30秒
                    {
                        LogMessage("EtherCat Connect Fail!", 1, false);
                        break;
                    }

                }
                // 系统复位
                sRtn = GTN.mc.GTN_Reset(nCore);
                LogMessage("error:GTN_Reset", sRtn, true);
                // 系统配置
                sRtn = GTN.mc.GTN_LoadConfig(nCore, "gtn_core1.cfg");
                LogMessage("error:GTN_LoadConfig", sRtn, true);
                // 清除状态
                sRtn = GTN.mc.GTN_ClrSts(nCore, 1, 32);
                LogMessage("error:GTN_ClrSts", sRtn, true);
            }
            catch (Exception ex)
            {
                init = false;
                Log4netRecord.ErrorLog.Error(ex);
            }
            axis_State.IsConnected = init;
            // 初始化扩展模块
            return init;
        }

        /// <summary>
        /// 回原点
        /// </summary>
        public void GoToOOrigin(short axis, double velocity, double acceleration, short smoothTime, double gearRatio)
        {
            if (!axis_State.IsConnected) return;
            if (!axis_State.AxisEnable) return;
            if (axis_State.IsBusy) return;
            axis_State.IsInitFinish = false;
            axis_State.IsBusy = true;
            try
            {
                bool isok = false;
                while (!isok)
                {
                    int DA = Convert.ToInt32(0.05 / gearRatio);
                    MoveStart(axis, -DA, velocity, acceleration, smoothTime, false);
                    int[] Value = GetEcatAxisDI();

                    if (Value != null && Value[0] > 0)
                    {
                        //到达负限位,
                        Thread.Sleep(1000);
                        for (int j = 0; j < 100000000; j++)
                        {
                            //往原点方向移动
                            MoveStart(axis, DA, velocity, acceleration, smoothTime, false);
                            Value = GetEcatAxisDI();

                            if (Value != null && Value[2] > 0)
                            {

                                isok = true;
                                break;
                            }

                            Thread.Sleep(1);
                        }

                    }
                    if (isok)
                    {
                        break;
                    }
                    Thread.Sleep(1);
                }

                Thread.Sleep(100);
                //到达原点  所有位置清零。
                ZeroPos(axis);
                ClrSts(axis);
                //到达原点  所有位置清零。
                //Gen.ZeroPos(axis);
                //Gen.ClrSts(axis);
                GetPrfPos(axis);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error("轴回原点异常", ex);
            }

            axis_State.IsInitFinish = true;
            axis_State.IsBusy = false;
        }

        /// <summary>
        /// 读取外部模块位置
        /// </summary>  
        public int PDO(short slaveID, int GetPdo, int SubIdx, ushort datasize)
        {
            Int16 PDO_Value = 0;
            //short rtn = 0;
            //ushort offset = Convert.ToUInt16(txtOffset.Text);
            //ushort size = Convert.ToUInt16(txtSize.Text);
            //byte[] PDOValue = new byte[size];
            //rtn = GTN.mc.GTN_GetEcatRawData(nCore, offset, size, out PDOValue[0]);
            //PDO_Value = BitConverter.ToInt16(PDOValue, 0);
            //if(PDO_Value>10000|| PDO_Value<0)
            //   PDO_Value=32675
            //txtValueR.Text = (BitConverter.ToInt16(PDOValue, 0)).ToString();
            short rtn = 0;
            byte[] data = new byte[4];
            ushort index = ushort.Parse(GetPdo.ToString(), System.Globalization.NumberStyles.HexNumber);////将界面输入的16进制数据转为10进制
            byte subIdx = Convert.ToByte(SubIdx.ToString());
            rtn = GTN.mc.GTN_GetEcatSlavePdo(nCore, slaveID, index, subIdx, ref data[0], datasize);
            //LogMessage("error:GTN_GetEcatSlavePdo", rtn, true);
            PDO_Value = BitConverter.ToInt16(data, 0);

            return PDO_Value;

        }
        /// <summary>
        /// 轴使能
        /// </summary>  
        public void Servo1Enable()
        {
            if (!axis_State.IsConnected) return;
            short axis = 1;
            try
            {
                if (GetAxisStatus(nCore, axis, AxisStatusType.IsServoOn))
                {
                    GTN.mc.GTN_AxisOff(nCore, axis);
                    LogMessage("使能关闭", axis, true);
                    axis_State.AxisEnable = false;
                }
                else
                {
                    GTN.mc.GTN_AxisOn(nCore, axis);
                    LogMessage("使能打开", axis, true);
                    axis_State.AxisEnable = true;
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
                Log4netRecord.ErrorLog.Error("轴使能", ex);
            }
        }
        public bool GetAxisStatus(short core, short axis, AxisStatusType axisStatusType)
        {
            short sRtn;
            bool sts = false;
            int axisStatus;
            uint pClk;
            sRtn = GTN.mc.GTN_GetSts(core, axis, out axisStatus, 1, out pClk);
            switch (axisStatusType)

            {
                //驱动报警
                case AxisStatusType.Alarm:
                    if ((axisStatus & 0x2) != 0)
                        sts = true;
                    else
                        sts = false;
                    break;
                //跟随误差越限
                case AxisStatusType.MError:
                    if ((axisStatus & 0x10) != 0)
                        sts = true;
                    else
                        sts = false;
                    break;
                // 正向限位
                case AxisStatusType.PosLimit:
                    if ((axisStatus & 0x20) != 0)
                        sts = true;
                    else
                        sts = false;
                    break;
                // 负向限位
                case AxisStatusType.NegLimit:
                    if ((axisStatus & 0x40) != 0)
                        sts = true;
                    else
                        sts = false;
                    break;
                // 平滑停止
                case AxisStatusType.SmoothStop:
                    if ((axisStatus & 0x80) != 0)
                        sts = true;
                    else
                        sts = false;
                    break;
                // 急停标志
                case AxisStatusType.AbruptStop:
                    if ((axisStatus & 0x100) != 0)
                        sts = true;
                    else
                        sts = false;
                    break;
                // 伺服使能
                case AxisStatusType.IsServoOn:
                    if ((axisStatus & 0x200) != 0)
                        sts = true;
                    else
                        sts = false;
                    break;
                // 规划器正在运动
                case AxisStatusType.Motion:
                    if ((axisStatus & 0x400) != 0)
                        //sts = true;
                        sts = false;
                    else
                        //sts =false ;
                        sts = true;
                    break;
            }
            return sts;
        }
        public enum AxisStatusType
        {
            Alarm,
            MError,
            PosLimit,
            NegLimit,
            SmoothStop,
            AbruptStop,
            IsServoOn,
            Motion,
        }
        /// <summary>
        /// 轴运动暂停
        /// </summary>  
        public void MoveStop(short trapAxis)
        {
            try
            {
                GTN.mc.GTN_Stop(1, 1 << trapAxis, 0);
                axis_State.IsBusy = false;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }

        }
        /// <summary>
        /// 轴运动
        /// </summary>  
        public void MoveStart(short axis, int position, double velocity, double acceleration, short smoothTime, bool rbtnAbsChecked)
        {
            //GTN.mc.GTN_SynchAxisPos(1, 0x1 << (axis - 1));
            // rtn = GTN.mc.GTN_SetEcatAxisMode(1, axis, 8);
            //sRtn = GTN.mc.GTN_SetHomingMode(nCore, nAxis, 8); // 切换到位置控制模式
            //LogMessage("error:GTN_SetEcatAxisMode", rtn, true);

            if (!axis_State.IsConnected) return;
            if (!axis_State.AxisEnable) return;
            try
            {
                GetEcatAxisDI();
                if ((axis_State.NegativeLimit && position > 0) || (axis_State.PositiveLimit && position < 0) || (!axis_State.NegativeLimit && !axis_State.PositiveLimit))
                {
                    if (rbtnAbsChecked)
                    {
                        if (axis_State.IsInitFinish)//是否初始化完成
                        {
                            MoveAbs0(1, axis, position, velocity, acceleration, acceleration, smoothTime, 0);
                            LogMessage("绝对移动距离", position, true);
                        }
                    }
                    else
                    {

                        MoveRel(1, axis, position, velocity, acceleration, acceleration, smoothTime, 0);
                        LogMessage("相对移动距离", position, true);
                    }
                }

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error("轴运动异常", ex);
            }

        }

        private short MoveAbs0(short core, short axis, int position, double velocity, double acceleration, double deceleration, short smoothTime, double velStart)
        {
            short sRtn = 0;
            GTN.mc.TTrapPrm trap;
            sRtn += GTN.mc.GTN_PrfTrap(core, axis);
            sRtn += GTN.mc.GTN_GetTrapPrm(core, axis, out trap);
            trap.acc = acceleration;
            trap.dec = deceleration;
            trap.smoothTime = smoothTime;
            trap.velStart = velStart;
            sRtn += GTN.mc.GTN_SetTrapPrm(core, axis, ref trap);//设置点位运动参数
            sRtn += GTN.mc.GTN_SetVel(core, axis, velocity);//设置目标速度
            sRtn += GTN.mc.GTN_SetPos(core, axis, position);//设置目标位置
            sRtn += GTN.mc.GTN_Update(core, 1 << (axis - 1));//更新轴运动          
            return sRtn;
        }

        private short MoveRel(short core, short axis, int position, double velocity, double acceleration, double deceleration, short smoothTime, double velStart)
        {
            short sRtn = 0;
            double prfpos;
            uint clk;
            GTN.mc.TTrapPrm trap;
            sRtn += GTN.mc.GTN_PrfTrap(core, axis);
            sRtn += GTN.mc.GTN_GetTrapPrm(core, axis, out trap);
            trap.acc = acceleration;
            trap.dec = deceleration;
            trap.smoothTime = smoothTime;
            trap.velStart = velStart;
            sRtn += GTN.mc.GTN_SetTrapPrm(core, axis, ref trap);//设置点位运动参数
            sRtn += GTN.mc.GTN_SetVel(core, axis, velocity);//设置目标速度
            sRtn += GTN.mc.GTN_GetPrfPos(core, axis, out prfpos, 1, out clk);//设置目标速度
            sRtn += GTN.mc.GTN_SetPos(core, axis, (int)prfpos + position);//设置目标位置
            sRtn += GTN.mc.GTN_Update(core, 1 << (axis - 1));//更新轴运动          
            return sRtn;
        }
        /// <summary>
        /// 读取轴S输出
        /// </summary>  
        public void GetEcatAxisDO(out uint PdoValue)
        {
            short rtn = GTN.mc.GTN_GetEcatAxisDO(nCore, nAxis, out PdoValue);
        }
        /// <summary>
        /// 读取轴S输入
        /// </summary>  
        public int[] GetEcatAxisDI()
        {
            if (!axis_State.IsConnected) return null;
            if (!axis_State.AxisEnable) return null;

            uint PdiValue;
            short rtn = GTN.mc.GTN_GetEcatAxisDI(nCore, nAxis, out PdiValue);
            int[] Value = new int[5];
            for (int i = 0; i < 5; i++)     //比如10进制的8              1000
            {                             //移位然后按位比较 i=0时，   0000         按位&后等于0000即为0
                if ((PdiValue & (1 << i)) != 0)   //移位然后按位比较 i=3时，   1000         按位&后等于1000即为8
                    Value[i] = 1; //0是负限位/1 正限位 、2原点,3,轴报警，4轴异常
                else
                    Value[i] = 0;
            }
            if (axis_State != null)
            {
                axis_State.NegativeLimit = Value[0] > 0;
                axis_State.PositiveLimit = Value[1] > 0;
                axis_State.Home = Value[2] > 0;
                axis_State.Alarm = Value[3] > 0;
                axis_State.Abnormal = Value[4] > 0;
            }
            return Value;
        }
        /// <summary>
        /// 读取轴位置清零
        /// </summary>  
        public void ZeroPos(short axis)
        {
            if (!axis_State.IsConnected) return ;
            if (!axis_State.AxisEnable) return ;
            short rtn = GTN.mc.GTN_ZeroPos(nCore, axis, 1);
            GetPrfPos(axis);
        }
        /// <summary>
        /// 读取轴ERR清零
        /// </summary>  
        public void ClrSts(short axis)
        {
            if (!axis_State.IsConnected) return;
            if (!axis_State.AxisEnable) return;
            short rtn = GTN.mc.GTN_ClrSts(nCore, axis, 1);
        }
        /// <summary>
        /// 读取轴实际位置
        /// </summary>  
        public double GetPrfPos(short axis)
        {
            if (!axis_State.IsConnected) return 0;
            if (!axis_State.AxisEnable) return 0;
            double prfPos;
            uint pClock = 0;
            GTN.mc.GTN_GetPrfPos(nCore, axis, out prfPos, 1, out pClock);
            if (axis_State != null)
            {
                axis_State.CurrentPosition_Pulse = Convert.ToInt32(prfPos);
            }
            return prfPos;
        }
        /// <summary>
        /// 读取发送轴位置
        /// </summary>  
        public double GetPrfVel(short axis)
        {
            if (!axis_State.IsConnected) return 0;
            if (!axis_State.AxisEnable) return 0;
            double prfVel;
            uint pClock = 0;
            GTN.mc.GTN_GetPrfVel(nCore, axis, out prfVel, 1, out pClock);
            if (axis_State != null)
            {
                axis_State.AxisTargetPosition = Convert.ToInt32(prfVel);
            }
            return prfVel;
        }
        #region/////////di检测
        private void timerExdi(uint PdiValue)
        {
            for (int i = 0; i < 12; i++)                                        //比如10进制的8              1000
            {                                                                   //移位然后按位比较 i=0时，   0000         按位&后等于0000即为0
                //if ((PdiValue & (1 << (i + 18))) != 0)                                        //移位然后按位比较 i=3时，   1000         按位&后等于1000即为8
                //{
                //    SCANI[i].BackColor = Color.LightGreen;
                //    SCANI[i].ForeColor = Color.Black;
                //}
                //else
                //{
                //    SCANI[i].BackColor = Color.LightGray;
                //    SCANI[i].ForeColor = Color.Black;
                //}
            }
        }
        #endregion
        #region/////////6轴HOME、limit
        private int timeralarm(uint PdiValue)
        {
            int Alarm = -1;
            for (int i = 0; i < 18; i++)                                        //比如10进制的8              1000
            {                                                                   //移位然后按位比较 i=0时，   0000         按位&后等于0000即为0
                if ((PdiValue & (1 << i)) != 0)                                        //移位然后按位比较 i=3时，   1000         按位&后等于1000即为8
                {
                    Alarm = i;
                    //Alarm[i].BackColor = Color.LightGreen;
                    //Alarm[i].ForeColor = Color.Black;
                }
                //else
                //{
                //    Alarm[i].BackColor = Color.LightGray;
                //    Alarm[i].ForeColor = Color.Black;
                //}
            }
            return Alarm;
        }
        #endregion

        #region/////////6轴DO
        private void timerDO()
        {
            //short rStn; uint PdoValue;
            //rStn = GTN.mc.GTN_GetEcatAxisDO(nCore, nAxis, out PdoValue);
            //for (int i = 0; i < 8; i++)
            //{
            //    if ((PdoValue & (1 << i)) != 0)
            //    {
            //        outputdo[i].BackColor = Color.LightGray;
            //    }
            //    else
            //    {
            //        outputdo[i].BackColor = Color.LightGreen;
            //    }
            //}
        }
        #endregion
    }
}

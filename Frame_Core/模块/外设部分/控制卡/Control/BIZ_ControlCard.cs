using EHolly;
using Frame_Core;
using FrameCommunity;
using HalconDotNet;
using LogTool;
using NPOI.Util;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Frame_Core
{
    public class BIZ_ControlCard
    {
        #region 板卡加入
        public GenClass1 Gen = new GenClass1();

        bool isInit = false;
        /// <summary>
        /// 参数
        /// </summary>
        public Param_ControlCard param_ControlCard = new Param_ControlCard();

        public bool InitParam(string path)
        {
            try
            {
                if (param_ControlCard == null) param_ControlCard = new Param_ControlCard();
                param_ControlCard.filePath = path;
                param_ControlCard.Init();
                param_ControlCard = (Param_ControlCard)param_ControlCard.Read();
                param_ControlCard.filePath = path;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
                return false;
            }
            return true;
        }


        private void ThrInit_Control()
        {
            Gen.ReEvent -= LogMessage;
            Gen.ReEvent += LogMessage;
        }

        public void LogMessage(string cotent, int rtn, bool flg)
        {

            //this.panel4.Invoke(
            //    (Action)delegate
            //    {
            //        if (flg == true & rtn != 0)
            //        {
            //            string TotalLogContent = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss   ") + cotent + "   " + "- - -    " + rtn;
            //            ShowErrorInfo(1, TotalLogContent);
            //        }
            //        else if (flg == false & rtn != 0)
            //        {
            //            string TotalLogContent = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss   ") + cotent;
            //            ShowErrorInfo(1, TotalLogContent);
            //        }

            //    }
            //    );
        }



        /// <summary>
        /// 自动对焦
        /// </summary>
        /// <param name="doubles"></param>
        public void AutoFocusing(QueuePackage<double> doubles)
        {
            try
            {
                double Data = 0;
                if (doubles.Count > 4)
                {
                    double value = 0;
                    double Number = 0;
                    while (doubles.Count > 0)
                    {
                        double tempValue = doubles.Dequeue();
                        value += tempValue;
                        Number++;
                    }

                    Data = value / Number;

                }

                if (param_ControlCard.duanM_IsAutoFocusing)
                {
                    if (Data != 0)
                    {
                        int offsetValuePluse = MMToPulse(Data)- (Gen.axis_State.CurrentPosition_Pulse - (int)param_ControlCard.duanM_WorkPosition);
                     
                        if (param_ControlCard.duanM_Sign) offsetValuePluse = -offsetValuePluse;

                        double upper = param_ControlCard.duanM_WorkPosition + param_ControlCard.duanM_WorkTolerances;
                        double lower = param_ControlCard.duanM_WorkPosition - param_ControlCard.duanM_WorkTolerances;

                        if (Gen.axis_State.IsConnected && Gen.axis_State.AxisEnable)
                            if (Gen.axis_State.CurrentPosition_Pulse + offsetValuePluse < upper &&
                                Gen.axis_State.CurrentPosition_Pulse + offsetValuePluse > lower)
                            {
                                Gen.MoveStart(param_ControlCard.axis_Param.AxisID, offsetValuePluse, param_ControlCard.axis_Param.Velocity, param_ControlCard.axis_Param.Acceleration, (short)param_ControlCard.axis_Param.SmoothTime, false);
                                FileOperationINI.WriteLog_ini(Project.Instance.paramPathSet.LogPath_Correct, $"当前值：{Gen.axis_State.CurrentPosition_Pulse} 目标值：{Gen.axis_State.CurrentPosition_Pulse + offsetValuePluse} 公差：{param_ControlCard.duanM_WorkTolerances} 毫米：{Data}");
                            }
                    }
                }

                // 读取轴状态
                ReadAxisState();
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }

        }

        /// <summary>
        /// 读取轴状态
        /// </summary>
        public void ReadAxisState()
        {
            Gen.GetEcatAxisDI();
            Gen.GetPrfPos(param_ControlCard.axis_Param.AxisID);
            Gen.GetPrfVel(param_ControlCard.axis_Param.AxisID);
        }

        /// <summary>
        /// 初始化控制卡
        /// </summary>
        /// <returns></returns>
        public bool InitControlCard()
        {
            bool isok = false;
            try
            {
                //初始化控制卡
                isok = Gen.Init_Control();

                //轴使能
                Gen.Servo1Enable();
            }
            catch (Exception ex)
            {
                isok = false;
                Log4netRecord.ErrorLog.Error(ex);
            }
            return isok;
        }

        /// <summary>
        /// 复位轴
        /// </summary>
        public void ResetAxis()
        {
            //回原点
            Gen.GoToOOrigin(param_ControlCard.axis_Param.AxisID, param_ControlCard.axis_Param.Velocity, param_ControlCard.axis_Param.Acceleration, (short)param_ControlCard.axis_Param.SmoothTime, GetGearRatio());

            //去工作位
            int workPosition = Convert.ToInt32(param_ControlCard.duanM_WorkPosition);
            Gen.MoveStart(param_ControlCard.axis_Param.AxisID, workPosition, param_ControlCard.axis_Param.Velocity, param_ControlCard.axis_Param.Acceleration, (short)param_ControlCard.axis_Param.SmoothTime, true);
        }

        /// <summary>
        /// 毫米转脉冲
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        public int MMToPulse(double distance)
        {
            return Convert.ToInt32(distance / GetGearRatio());
        }

        /// <summary>
        /// 脉冲转毫米
        /// </summary>
        /// <param name="pulse"></param>
        /// <returns></returns>
        public double PulseToMM(double pulse)
        {
            return Convert.ToInt32(pulse * GetGearRatio());
        }

        /// <summary>
        /// 获取齿轮比例
        /// </summary>
        /// <returns></returns>
        public double GetGearRatio()
        {
            if (param_ControlCard.axis_Param.GearRatioMolecule <= 0 || param_ControlCard.axis_Param.GearRatioDenominator <= 0)
            {
                return 1.0 / 5000;
            }

            return (param_ControlCard.axis_Param.GearRatioMolecule / param_ControlCard.axis_Param.GearRatioDenominator);
        }
        #endregion

    }
}

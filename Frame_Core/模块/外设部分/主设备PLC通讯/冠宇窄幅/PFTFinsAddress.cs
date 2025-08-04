using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using S7.Net.Types;

namespace Frame_Core
{
    public  class PFTFinsAddress: CutOverlayPLCAddress
    {
        
        /// <summary>
        /// <para>线扫相机A心跳信号 1运行 0停止</para>
        /// </summary>
        [Description("线扫相机A心跳信号")]
        public CommunicationData<short> Out_CorrectHeart_CCDXLA = new CommunicationData<short>();


        /// <summary>
        ///  <para>触发设备报警信号 1报警 0正常</para>
        /// </summary>
        [Description("触发设备报警信号")]
        public CommunicationData<short> Out_CorrectAlarm_CCDPFT = new CommunicationData<short>();

        /// <summary>
        ///  <para>PLC下发一键换卷触发信号 1触发 0换卷完成</para>
        /// </summary>
        [Description("PLC下发一键换卷触发信号")]
        public CommunicationData<short> In_ChangeRollTrigger_CCDPFT = new CommunicationData<short>();

        /// <summary>
        ///  <para>CCD读取上位机卷号</para>
        /// </summary>
        [Description("CCD读取上位机卷号")]
        public CommunicationData<string> In_ChangeRollNum_CCDPFT = new CommunicationData<string>();



        /// <summary>
        ///  <para>设备运行速度</para>
        /// </summary>
        [Description("设备运行速度")]
        public CommunicationData<float> In_MachineSpeed_CCDPFT = new CommunicationData<float>();

        /// <summary>
        ///  <para> 机器运行状态1:正常运行 0：停机</para>
        /// </summary>
        [Description("机器运行状态")]
        public CommunicationData<short> In_MachineRunningState_CCDPFT = new CommunicationData<short>();


        /// <summary>
        ///  <para>点检触发 1提醒 0不提醒</para>
        /// </summary>
        [Description("点检提醒")]
        public CommunicationData<short> Out_SendCheck_CCDPFT = new CommunicationData<short>();


        /// <summary>
        ///  <para>点检触发 1提醒 0不提醒</para>
        /// </summary>
        [Description("CCD使能检测")]
        public CommunicationData<int> Out_SendCCDEnable_CCDPFT = new CommunicationData<int>();


        /// <summary>
        ///  <para>3D相机1测厚值</para>
        /// </summary>
        [Description("3D相机1测厚值")]
        public CommunicationData<float> Out_SendThickValue3D1_CCDPFT = new CommunicationData<float>();

        /// <summary>
        ///  <para>3D相机1测厚值</para>
        /// </summary>
        [Description("3D相机2测厚值")]
        public CommunicationData<float> Out_SendThickValue3D2_CCDPFT = new CommunicationData<float>();



        ///// <summary>
        /////  <para> 过胶带状态 1:正常 0：不是</para>
        ///// </summary>
        //[Description("过胶带状态")]
        //public CommunicationData<long> In_MachineRunningTape_CCDPFT = new CommunicationData<long>();



        ///// <summary>
        /////  <para> 叫减速状态 1:正常 0：不是</para>
        ///// </summary>
        //[Description("加减速状态")]
        //public CommunicationData<long> In_MachineRunningSlownDown_CCDPFT = new CommunicationData<long>();




        ///// <summary>
        /////  <para>漏标提醒 1提醒 0不提醒</para>
        ///// </summary>
        //[Description("漏标提醒")]
        //public CommunicationData<double> Out_SendLabelLose_CCDPFT = new CommunicationData<double>();


        public PFTFinsAddress()
        {
            Init();
        }

        public override void Init()
        {
            Out_CorrectHeart_CCDXLA.Address = "D25004";
            Out_CorrectHeart_CCDXLA.Name = "线扫相机A心跳信号";

            Out_CorrectAlarm_CCDPFT.Address = "D25008";
            Out_CorrectAlarm_CCDPFT.Name = "触发设备报警信号";

            In_ChangeRollTrigger_CCDPFT.Address = "D25012";
            In_ChangeRollTrigger_CCDPFT.Name = "PLC下发一键换卷触发信号";

            In_ChangeRollNum_CCDPFT.Address = "D25020";
            In_ChangeRollNum_CCDPFT.Name = "CCD读取上位机卷号";

            In_MachineRunningState_CCDPFT.Address = "D25060";
            In_MachineRunningState_CCDPFT.Name = "机器运行状态";

            Out_SendCheck_CCDPFT.Address = "D25094";
            Out_SendCheck_CCDPFT.Name = "点检提醒";

            Out_SendCCDEnable_CCDPFT.Address = "D25074";
            Out_SendCCDEnable_CCDPFT.Name = "CCD使能检测";

            In_MachineSpeed_CCDPFT.Address = "D25000";
            In_MachineSpeed_CCDPFT.Name = "设备运行速度";

            Out_SendThickValue3D1_CCDPFT.Address = "D25098";
            Out_SendThickValue3D1_CCDPFT.Name = "3D相机1测厚值";

            Out_SendThickValue3D2_CCDPFT.Address = "D25102";
            Out_SendThickValue3D2_CCDPFT.Name = "3D相机2测厚值";

            //Out_SendLabelLose_CCDPFT.Address = "D20100";
            //Out_SendLabelLose_CCDPFT.Name = "漏标提醒";

        }

    }
}

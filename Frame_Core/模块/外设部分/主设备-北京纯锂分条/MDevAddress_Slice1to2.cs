using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    public class MDevAddress_Slice1to2
    {
        #region 发送 
        [Description("心跳信号")]
        public ShortData Heart_CCD1 = new ShortData();//间隔1s写1

        [Description("NG报警")]
        public ShortData NGAlarm = new ShortData();

        [Description("NG停机")]
        public ShortData NGHalt = new ShortData();

        [Description("编码轮压下或抬起")]
        public ShortData CodingWheelCylinder = new ShortData();

        [Description("报警复位")]
        public ShortData ResetAlarm = new ShortData();

        [Description("新建膜卷")]
        public ShortData NewFilmRoll = new ShortData();

        [Description("运行/暂停")]
        public ShortData RunOrPause = new ShortData();

        [Description("编码轮直径")]
        public ShortData EncoderDiameter = new ShortData();

        [Description("编码轮脉冲")]
        public ShortData EncoderPulse = new ShortData();

        [Description("纠偏使能")]
        public ShortData CorrectEn = new ShortData();

        [Description("手自动纠偏切换")]
        public ShortData CorrectManualAuto = new ShortData();

        [Description("纠偏速度")]
        public ShortData CorrectSleep = new ShortData();

        [Description("纠偏回中")]
        public ShortData CorrectToHome = new ShortData();

        [Description("手动纠偏量")]
        public FloatData CorrectManualValue = new FloatData();

        [Description("自动纠偏量")]
        public FloatData CorrectAutoValue = new FloatData();

        #endregion

        #region 接收

        [Description("采图不正常报警停机")]
        public ShortData AcquisitionImageException = new ShortData();

        [Description("PLC编码器无脉冲信号报警")]
        public ShortData PLCNotPulseSignal = new ShortData();

        [Description("纠偏伺服报警")]
        public ShortData CorrectServoAlarm = new ShortData();

        [Description("编码轮未压到位报警")]
        public ShortData CodingWheelAlarm = new ShortData();

        [Description("纠偏极限位置报警")]
        public ShortData CorrectLimitingPositionAlarm = new ShortData();
        #endregion
        public MDevAddress_Slice1to2()
        {
            Init();
        }
        public void Init()
        {
            Heart_CCD1.address = "D8700";
            Heart_CCD1.name = "心跳信号";

            NGAlarm.address = "D8701";
            NGAlarm.name = "NG报警";

            NGHalt.address = "D8702";
            NGHalt.name = "NG停机";

            CodingWheelCylinder.address = "D8703";
            CodingWheelCylinder.name = "编码轮压下或抬起";

            ResetAlarm.address = "D8704";
            ResetAlarm.name = "报警复位";

            NewFilmRoll.address = "D8705";
            NewFilmRoll.name = "新建膜卷";

            RunOrPause.address = "D8706";
            RunOrPause.name = "运行/暂停";

            EncoderDiameter.address = "D8707";
            EncoderDiameter.name = "编码轮直径";

            EncoderPulse.address = "D8708";
            EncoderPulse.name = "编码轮脉冲";

            CorrectEn.address = "D8709";
            CorrectEn.name = "纠偏使能";

            CorrectManualAuto.address = "D8710";
            CorrectManualAuto.name = "手自动纠偏切换";

            CorrectSleep.address = "D8711";
            CorrectSleep.name = "纠偏速度";

            CorrectToHome.address = "D8712";
            CorrectToHome.name = "纠偏回中";

            CorrectManualValue.address = "D8713";
            CorrectManualValue.name = "手动纠偏量";

            CorrectAutoValue.address = "D8715";
            CorrectAutoValue.name = "自动纠偏量";

            AcquisitionImageException.address = "D5700";
            AcquisitionImageException.name = "采图不正常报警停机";

            PLCNotPulseSignal.address = "D5701";
            PLCNotPulseSignal.name = "PLC编码器无脉冲信号报警";

            CorrectServoAlarm.address = "D5702";
            CorrectServoAlarm.name = "纠偏伺服报警";

            CodingWheelAlarm.address = "D5703";
            CodingWheelAlarm.name = "编码轮未压到位报警";

            CorrectLimitingPositionAlarm.address = "D5704";
            CorrectLimitingPositionAlarm.name = "纠偏极限位置报警";
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    public class EL
    {
        bool HighLow = false;//false为ABCD，true为CDAB
        #region 变量
        public static Byte[] Value = new Byte[48];//L6分条58字节，B2分条48字节


        private const int Mode = 4;//发送模式
        private const int ModeBackNum = 5;//返回模式
        private const int Control = 8;
        private const int Offset = 12;
        private const int TargetPos = 14;
        private const int OffsetBack = 22;
        private const int TargetPosBack = 24;
        public const int AutomaticBackNum = 10001;
        public const int CenterPositionBackNum = 10002;
        public const int ManualBackNum = 10003;
        public const int MoveRigthBackNum = 10004;
        public const int MoveLeftBackNum = 10005;
        public const int AutomaticStopBackNum = 10006;

        private const string SetWebOffset = "0000000000000001";
        private const string SetTargetPos = "0000000000000010";
        private const string Automatic = "0000000000001001";
        private const string CenterPosition = "0000000000001010";
        private const string Manual = "0000000000001100";
        private const string MoveRigth = "0000000001010000";
        private const string MoveLeft = "0000000001100000";
        private const string AutomaticStop = "0000000010000000";

        private const string AutomaticBack = "0000000000000001";
        private const string CenterPositionBack = "0000000000000010";
        private const string ManualBack = "0000000000000100";
        private const string MoveRigthBack = "0000000000010100";
        private const string MoveLeftBack = "0000000000100100";
        private const string AutomaticStopBack = "0000000010000000";
        #endregion

        #region 方法
        public EL()
        {
            Init();
        }
        /// <summary>
        /// 报文初始化
        /// </summary>
        private void Init()
        {
            for (int i = 0; i < Value.Length; i++)
            {
                Value[i] = 0;
            }
        }

        /// <summary>
        /// 返回空报文
        /// </summary>
        /// <param name="offsetvalue">偏移值</param>
        /// <returns></returns>
        public byte[] Empty(double offsetvalue)
        {
            Init();
            return SetValue(SetValue(Value, Control, 2, StringToByte("0000000000000000")), Offset, 2, DoubleToByte(offsetvalue));
        }

        /// <summary>
        /// 返回偏移报文
        /// </summary>
        /// <param name="offsetvalue">偏移值</param>
        /// <returns></returns>
        public byte[] OffSet(double offsetvalue)
        {
            Init();
            return SetValue(SetValue(Value, Control, 2, StringToByte(SetWebOffset)), Offset, 2, DoubleToByte(offsetvalue));
        }
        /// <summary>
        /// 返回绝对位置报文
        /// </summary>
        /// <param name="offsetvalue">偏移值</param>
        /// <returns></returns>
        public byte[] Targetpos(double targetPosvalue)
        {
            Init();
            return SetValue(SetValue(Value, Control, 2, StringToByte(SetTargetPos)), TargetPos, 2, DoubleToByte(targetPosvalue));
        }
        /// <summary>
        /// 返回自动模式报文
        /// </summary>
        /// <returns></returns>
        public byte[] Auto()
        {
            Init();
            //return SetValue(Value, Mode, 2, StringToByte(Automatic));
            return SetValue(Value, Mode, 2, StringToByte(Automatic));
        }
        /// <summary>
        /// 选择电眼
        /// </summary>
        /// <returns></returns>
        public byte[] SelectSensor()
        {
            Init();
            return SetValue(Value, Mode, 2, StringToByte("0100010000000000"));
        }
        public byte[] SelectSensorManu()
        {
            Init();
            return SetValue(Value, Mode, 2, StringToByte("0000010000001100"));
        }

        public byte[] SelectSensorAuto()
        {
            Init();
            return SetValue(Value, Mode, 2, StringToByte("0000010000001001"));
        }
        /// <summary>
        /// 返回回中报文
        /// </summary>
        /// <returns></returns>
        public byte[] Center()
        {
            Init();
            return SetValue(Value, Mode, 2, StringToByte(CenterPosition));
        }
        /// <summary>
        /// 返回手动报文
        /// </summary>
        /// <returns></returns>
        public byte[] Manu()
        {
            Init();
            return SetValue(Value, Mode, 2, StringToByte(Manual));
        }
        /// <summary>
        /// 返回手动右移报文
        /// </summary>
        /// <returns></returns>
        public byte[] Right()
        {
            Init();
            return SetValue(Value, Mode, 2, StringToByte(MoveRigth));
        }
        /// <summary>
        /// 返回手动左移报文
        /// </summary>
        /// <returns></returns>
        public byte[] Left()
        {
            Init();
            return SetValue(Value, Mode, 2, StringToByte(MoveLeft));
        }
        /// <summary>
        /// 返回自动停止报文
        /// </summary>
        /// <returns></returns>
        public byte[] Stop()
        {
            Init();
            return SetValue(Value, Mode, 2, StringToByte(AutomaticStop));
        }
        /// <summary>
        /// 替换数组中的值
        /// </summary>
        /// <param name="Value">输入原数组</param>
        /// <param name="Index">索引</param>
        /// <param name="Num">长度</param>
        /// <param name="ByteValue">提供值数组</param>
        /// <returns></returns>
        private byte[] SetValue(Byte[] Value, int Index, int Num, byte[] ByteValue)
        {
            try
            {
                for (int i = 0; i < Num; i++)
                {
                    if (HighLow)
                    {
                        Value[Index + i] = ByteValue[ByteValue.Length - 1 - i];
                    }
                    else
                    {
                        Value[Index + i] = ByteValue[i];
                    }

                  
                }
                return Value;
            }
            catch
            {
                return Value;
            }
        }
        public byte[] SetValue1(Byte[] Value, int Index, int Num, byte[] ByteValue)
        {
            try
            {
                for (int i = 0; i < Num; i++)
                {
                    Value[Index + i] = ByteValue[i];
                    //   Value[Index + i] = ByteValue[i];
                }
                return Value;
            }
            catch
            {
                return Value;
            }
        }
        /// <summary>
        /// 字符串转字节
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private byte[] StringToByte(string str)
        {
            try
            {
                Int16 IntStr = Convert.ToInt16(str, 2);
                return BitConverter.GetBytes(IntStr);
            }
            catch
            {
                Int16 IntStr = Convert.ToInt16("0000000000000000", 2);
                return BitConverter.GetBytes(IntStr);
            }
        }
        /// <summary>
        /// 浮点转字节
        /// </summary>
        /// <param name="dle"></param>
        /// <returns></returns>
        public byte[] DoubleToByte(double D)
        {
            try
            {
                return BitConverter.GetBytes((Int16)(D));
            }
            catch
            {
                return BitConverter.GetBytes((Int16)(0));
            }
        }
        /// <summary>
        /// 浮点转4字节
        /// </summary>
        /// <param name="dle"></param>
        /// <returns></returns>
        public byte[] DoubleToByte1(double D)
        {
            try
            {
                return BitConverter.GetBytes((Int32)(D));
            }
            catch
            {
                return BitConverter.GetBytes((Int32)(0));
            }
        }
        /// <summary>
        /// 整数转字节
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        private byte[] IntToByte(int i)
        {
            return BitConverter.GetBytes((Int16)(i));
        }
        /// <summary>
        /// 获取纠偏偏移值
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public short ReadOffsetValue(Byte[] Value)
        {
            try
            {
                byte temp = 0;
                temp = Value[OffsetBack];
                if (HighLow)
                {
                    Value[OffsetBack] = Value[OffsetBack + 1];
                    Value[OffsetBack + 1] = temp;
                }

                short i = BitConverter.ToInt16(Value, OffsetBack);
                return i;
            }
            catch
            {
                return 0;
            }

        }

        /// <summary>
        /// 获取位置值
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public short ReadTargetPosValue(Byte[] Value)
        {
            try
            {
                short i = BitConverter.ToInt16(Value, TargetPosBack);
                return i;
            }
            catch
            {
                return 0;
            }

        }
        /// <summary>
        /// 返回模式代码
        /// </summary>
        /// <param name="bytevalue"></param>
        /// <returns></returns>
        public int ModeBack(byte[] bytevalue)
        {
            try
            {
                byte tempbyte;
                tempbyte = bytevalue[ModeBackNum];
                if (bytevalue[ModeBackNum] == StringToByte(AutomaticBack)[0])
                    return BackNum.AutomaticBackNum;
                if (bytevalue[ModeBackNum] == StringToByte(CenterPositionBack)[0])
                    return BackNum.CenterPositionBackNum;
                if (bytevalue[ModeBackNum] == StringToByte(ManualBack)[0])
                    return BackNum.ManualBackNum;
                if (bytevalue[ModeBackNum] == StringToByte(MoveRigthBack)[0])
                    return BackNum.MoveRigthBackNum;
                if (bytevalue[ModeBackNum] == StringToByte(MoveLeftBack)[0])
                    return BackNum.MoveLeftBackNum;
                if (bytevalue[ModeBackNum] == StringToByte(AutomaticStopBack)[0])
                    return BackNum.AutomaticStopBackNum;
                return -1;
            }
            catch
            {
                return -1;
            }
        }
        #endregion
    }
    public struct BackNum
    {
        public const int AutomaticBackNum = 10001;
        public const int CenterPositionBackNum = 10002;
        public const int ManualBackNum = 10003;
        public const int MoveRigthBackNum = 10004;
        public const int MoveLeftBackNum = 10005;
        public const int AutomaticStopBackNum = 10006;
    }
}

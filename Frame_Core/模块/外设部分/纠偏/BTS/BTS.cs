using NPOI.SS.Formula.Functions;
using NPOI.XSSF.Streaming.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    /// <summary>
    /// BTS纠偏（反馈数据是固定频率的，默认是 40ms）
    /// </summary>
    public class BTS
    {
        /// <summary>
        /// BTS发送协议头长度
        /// </summary>
        public const int BTS_SEND_VALUE_LEN = 96;

        /// <summary>
        /// 是否为小端序
        /// </summary>
        private bool isLittleEndian = true;


        /// <summary>
        /// 获取发送值报文
        /// </summary>
        /// <param name="correctionMode">纠偏模式</param>
        /// <param name="offsetValue">偏移值（mm）</param>
        /// <returns></returns>
        public Byte[] GetSendValue(int correctionMode, double offsetValue)
        {
            Byte[] bytesValue = new Byte[BTS_SEND_VALUE_LEN];

            // 设置协议为 0x2041
            SetValue(bytesValue, BTSSendByteIndexStruct.Protocol, new byte[] { 0x41, 0x20 });

            // 获取偏移值（纠偏器单位是um）
            byte[] bytesOffset = IntToByte((int)(SendOffsetValueConversion(offsetValue)));

            // 设置纠偏模式和偏移值
            byte correctionByte = 0x02; // 默认值
            int offsetIndex = BTSSendByteIndexStruct.OffsetEdge1; // 默认偏移索引

            if (correctionMode == BTSSendCorrectModeStruct.Edge2)
            {
                correctionByte = 0x04;
                offsetIndex = BTSSendByteIndexStruct.OffsetEdge2;
            }
            else if (correctionMode == BTSSendCorrectModeStruct.Center1And2)
            {
                correctionByte = 0x08;
                offsetIndex = BTSSendByteIndexStruct.OffsetCenter1And2;
            }
            SetValue(bytesValue, BTSSendByteIndexStruct.Enabled, new byte[] { correctionByte });
            SetValue(bytesValue, offsetIndex, bytesOffset);

            return bytesValue;
        }

        /// <summary>
        /// 获取发送心跳包（建议 100ms）
        /// </summary>
        /// <returns></returns>
        public Byte[] GetSendHeartbeat()
        {
            Byte[] bytesValue = new Byte[BTS_SEND_VALUE_LEN];

            // 设置协议为 0x2041
            SetValue(bytesValue, BTSSendByteIndexStruct.Protocol, new byte[] { 0x41, 0x20 });

            return bytesValue;
        }

        /// <summary>
        /// 获取读取纠偏模式
        /// </summary>
        /// <param name="bytevalue"></param>
        /// <returns></returns>
        public int GetReadCorrectionMode(byte[] bytevalue)
        {
            try
            {
                byte tempbyte;
                int byteIndex = BTSReadByteIndexStruct.CorrectionMode;
                tempbyte = bytevalue[byteIndex];
                if (bytevalue[byteIndex] == 0x01)
                    return BTSReadCorrectModeStruct.Edge1;
                if (bytevalue[byteIndex] == 0x02)
                    return BTSReadCorrectModeStruct.Edge2;
                if (bytevalue[byteIndex] == 0x03)
                    return BTSReadCorrectModeStruct.Edge3;
                if (bytevalue[byteIndex] == 0x05)
                    return BTSReadCorrectModeStruct.Center1And2;
                if (bytevalue[byteIndex] == 0x06)
                    return BTSReadCorrectModeStruct.WatchdogEdge1;
                if (bytevalue[byteIndex] == 0x07)
                    return BTSReadCorrectModeStruct.WatchdogEdge2;
                return -1;
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>
        /// 获取读取工作模式([1手动模式,2自动模式,3居中模式])
        /// </summary>
        /// <param name="bytevalue"></param>
        /// <returns></returns>
        public int GetReadWorkMode(byte[] bytevalue)
        {
            try
            {
                byte tempbyte;
                int byteIndex = BTSReadByteIndexStruct.WorkMode;
                tempbyte = bytevalue[byteIndex];
                if (bytevalue[byteIndex] == 0x31)
                    return 1;
                if (bytevalue[byteIndex] == 0x41)
                    return 2;
                if (bytevalue[byteIndex] == 0x51)
                    return 3;
                return -1;
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>
        /// 获取读取偏移值
        /// </summary>
        /// <param name="bytevalue"></param>
        /// <param name="readCorrectMode"></param>
        /// <param name="source0ffsetValue"></param>
        /// <param name="real0ffsetValue"></param>
        public void GetReadOffsetValue(byte[] bytevalue, int readCorrectMode, out double source0ffsetValue, out double real0ffsetValue)
        {
            source0ffsetValue = 0;
            real0ffsetValue = 0;
            try
            {
                if (readCorrectMode == -1) return;

                int offsetValueIndex = BTSReadByteIndexStruct.OffsetEdge1; // 默认偏移索引
                if (readCorrectMode == BTSReadCorrectModeStruct.Edge1)
                {
                    offsetValueIndex = BTSReadByteIndexStruct.OffsetEdge1;
                }
                else if (readCorrectMode == BTSReadCorrectModeStruct.Edge2)
                {
                    offsetValueIndex = BTSReadByteIndexStruct.OffsetEdge2;
                }
                else if (readCorrectMode == BTSReadCorrectModeStruct.Center1And2)
                {
                    offsetValueIndex = BTSReadByteIndexStruct.OffsetCenter1And2;
                }

                byte[] bytes = GetValue(bytevalue, offsetValueIndex, 4);
                int offserValue = BitConverter.ToInt32(bytes, 0);

                source0ffsetValue = offserValue;
                real0ffsetValue = ReadOffsetValueConversion(offserValue);;
            }
            catch
            {

            }
        }

        /// <summary>
        /// 读取偏移值单位换算
        /// </summary>
        /// <param name="offserValue">偏移值（单位：um）</param>
        /// <returns>返回单位mm</returns>
        public double ReadOffsetValueConversion(double offserValue)
        {
            return offserValue / 1000;
        }

        /// <summary>
        /// 发送偏移值单位换算
        /// </summary>
        /// <param name="offserValue">偏移值（单位：mm）</param>
        /// <returns>返回单位um</returns>
        public double SendOffsetValueConversion(double offserValue)
        {
            return offserValue * 1000;
        }


        /// <summary>
        /// 替换数组中的值
        /// </summary>
        /// <param name="Value">输入原数组</param>
        /// <param name="Index">索引</param>      
        /// <param name="ByteValue">提供值数组</param>    
        /// <returns></returns>
        public byte[] SetValue(byte[] Value, int Index, byte[] ByteValue)
        {
            try
            {
                //byte[] result = new byte[ByteValue.Length];
                //Array.Copy(ByteValue, result, ByteValue.Length);
                if (!isLittleEndian) { Array.Reverse(ByteValue); }
                for (int i = 0; i < ByteValue.Length; i++)
                {
                    Value[Index + i] = ByteValue[i];
                }
                return Value;
            }
            catch
            {
                return Value;
            }
        }

        public byte[] GetValue(byte[] Value, int Index, int Num)
        {
            if (Value == null || Index < 0 || Num <= 0 || Index + Num > Value.Length)
            {
                return null; // 返回空数组表示参数无效
            }

            byte[] result = new byte[Num];
            Array.Copy(Value, Index, result, 0, Num);

            if (!isLittleEndian) { Array.Reverse(result); }
            return result;
        }


        /// <summary>
        /// 字节反转AB转换为BA
        /// </summary>
        /// <param name="value">反转字节</param>
        /// <param name="startIndex">反转开始索引</param>
        public void ByteInvert_ToBA(ref byte[] value, int startIndex)
        {
            if (value == null || value.Length < startIndex + 2)
            {
                return;
            }

            byte temp = value[startIndex];
            value[startIndex] = value[startIndex + 1];
            value[startIndex + 1] = temp;
        }

        /// <summary>
        /// 字节反转ABCD转换为CDAB
        /// </summary>
        /// <param name="value">反转字节</param>
        /// <param name="startIndex">反转开始索引</param>
        public void ByteInvert_ToCDAB(ref byte[] value, int startIndex)
        {
            if (value == null || value.Length < startIndex + 4)
            {
                return;
            }

            byte temp = value[startIndex];
            value[startIndex] = value[startIndex + 2];
            value[startIndex + 2] = temp;

            temp = value[startIndex + 1];
            value[startIndex + 1] = value[startIndex + 3];
            value[startIndex + 3] = temp;
        }

        /// <summary>
        /// 字节反转ABCD转换为BADC
        /// </summary>
        /// <param name="value">反转字节</param>
        /// <param name="startIndex">反转开始索引</param>
        public void ByteInvert_ToBADC(ref byte[] value, int startIndex)
        {
            if (value == null || value.Length < startIndex + 4)
            {
                return;
            }

            byte temp = value[startIndex];
            value[startIndex] = value[startIndex + 1];
            value[startIndex + 1] = temp;

            temp = value[startIndex + 2];
            value[startIndex + 2] = value[startIndex + 3];
            value[startIndex + 3] = temp;
        }

        /// <summary>
        /// 字节反转ABCD转换为DCBA
        /// </summary>
        /// <param name="value">反转字节</param>
        /// <param name="startIndex">反转开始索引</param>
        public void ByteInvert_ToDCBA(ref byte[] value, int startIndex)
        {
            if (value == null || value.Length < startIndex + 4)
            {
                return;
            }

            byte temp = value[startIndex];
            value[startIndex] = value[startIndex + 3];
            value[startIndex + 3] = temp;

            temp = value[startIndex + 1];
            value[startIndex + 1] = value[startIndex + 2];
            value[startIndex + 2] = temp;
        }


        /// <summary>
        /// 字符串转字节
        /// </summary>
        /// <param name="value">只限于二进制数据字符</param>
        /// <returns></returns>
        public byte[] StringToByte(string value)
        {
            try
            {
                Int16 IntStr = Convert.ToInt16(value, 2);
                return BitConverter.GetBytes(IntStr);
            }
            catch
            {
                Int16 IntStr = Convert.ToInt16("0000000000000000", 2);//转换为二进制int
                return BitConverter.GetBytes(IntStr);
            }
        }

        /// <summary>
        /// 浮点转8字节
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public byte[] DoubleToByte(double value)
        {
            return BitConverter.GetBytes(value);
        }
        /// <summary>
        /// 浮点转4字节
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public byte[] DoubleToByte(float value)
        {
            return BitConverter.GetBytes(value);
        }

        /// <summary>
        /// 整数转4字节
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public byte[] IntToByte(int value)
        {
            return BitConverter.GetBytes(value);
        }


    }

    /// <summary>
    /// BTS发送纠偏模式
    /// </summary>
    public struct BTSSendCorrectModeStruct
    {
        public const int Edge1 = 1;

        public const int Edge2 = 2;

        public const int Center1And2 = 3;
    }

    /// <summary>
    /// BTS读取纠偏模式
    /// </summary>
    public struct BTSReadCorrectModeStruct
    {
        public const int Edge1 = 1;
        public const int Edge2 = 2;
        public const int Edge3 = 3;

        public const int Center1And2 = 5;
        public const int WatchdogEdge1 = 6;
        public const int WatchdogEdge2 = 7;
    }
    /// <summary>
    ///  BTS发送报文字节索引信息
    /// </summary>
    public struct BTSSendByteIndexStruct
    {
        /// <summary>
        /// 0x20, 0x41,指定协议为 2041 协议
        /// </summary>
        public const int Protocol = 20;

        /// <summary>
        /// 使能
        /// </summary>
        public const int Enabled = 28;

        /// <summary>
        /// 边缘1偏移（偏移值为有符号，单位：微米）
        /// </summary>
        public const int OffsetEdge1 = 36;

        /// <summary>
        /// 边缘2偏移（偏移值为有符号，单位：微米）
        /// </summary>
        public const int OffsetEdge2 = 40;//


        /// <summary>
        /// 中心1和2偏移（偏移值为有符号，单位：微米）
        /// </summary>
        public const int OffsetCenter1And2 = 44;//



    }
    /// <summary>
    /// BTS读取报文字节索引信息
    /// </summary>
    public struct BTSReadByteIndexStruct
    {
        /// <summary>
        /// 工作模式
        /// </summary>
        public const int WorkMode = 34;

        /// <summary>
        /// 纠偏模式
        /// </summary>
        public const int CorrectionMode = 35;

        /// <summary>
        /// 边缘1偏移（偏移值为有符号，单位：微米）
        /// </summary>
        public const int OffsetEdge1 = 36;

        /// <summary>
        /// 边缘2偏移（偏移值为有符号，单位：微米）
        /// </summary>
        public const int OffsetEdge2 = 40;//


        /// <summary>
        /// 中心1和2偏移（偏移值为有符号，单位：微米）
        /// </summary>
        public const int OffsetCenter1And2 = 44;//
    }
}

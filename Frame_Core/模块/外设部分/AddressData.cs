using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    public class CommunicationData<T>
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name = "";

        /// <summary>
        /// 地址
        /// </summary>
        public string Address = "";

        /// <summary>
        /// 是否执行
        /// </summary>
        public bool Execute = false;

        /// <summary>
        /// 值
        /// </summary>
        public T Value;

        /// <summary>
        /// 旧的读取值
        /// </summary>
        public T OldReadValue;

        /// <summary>
        /// 读取不同状态标志
        /// </summary>
        public bool ReadDifferentStateFlag = false;
    }

    public enum ReadLogSaveType
    {
        /// <summary>
        /// 读取不同状态
        /// </summary>
        ReadDifferentState,
        /// <summary>
        /// 全部保存
        /// </summary>
        True,
        /// <summary>
        /// 全部不保存
        /// </summary>
        Fasle
    }

    public enum WriteLogSaveType
    {
        /// <summary>
        /// 全部保存
        /// </summary>
        True,
        /// <summary>
        /// 全部不保存
        /// </summary>
        Fasle
    }

    public static class EdgeDetector
    {
        private static Dictionary<string, bool> _previousStates = new Dictionary<string, bool>();

        /// <summary>
        /// 检测上升沿
        /// </summary>
        /// <param name="signalName">信号名称</param>
        /// <param name="input">输入信号</param>
        /// <returns>如果上升沿被检测到，则返回true；否则返回false</returns>
        public static bool RisingEdge(string signalName, bool input)
        {
            if (!_previousStates.ContainsKey(signalName))
            {
                _previousStates[signalName] = false;
            }

            bool result = input && !_previousStates[signalName];
            _previousStates[signalName] = input;
            return result;
        }

        /// <summary>
        /// 检测下降沿
        /// </summary>
        /// <param name="signalName">信号名称</param>
        /// <param name="input">输入信号</param>
        /// <returns>如果下降沿被检测到，则返回true；否则返回false</returns>
        public static bool FallingEdge(string signalName, bool input)
        {
            if (!_previousStates.ContainsKey(signalName))
            {
                _previousStates[signalName] = false;
            }

            bool result = !input && _previousStates[signalName];
            _previousStates[signalName] = input;
            return result;
        }
    }


    public class BoolData
    {
        /// <summary>
        /// 读取不同状态标志
        /// </summary>
        public bool readDifferentStateFlag;

        /// <summary>
        ///旧的值
        /// </summary>
        public bool OldValue;

        //S7
        public string dbName;
        public int db;
        public string startByteAdrName;
        public int startByteAdr;

        //Fins、Modbus
        public string address;//寄存器地址


        public bool value;//寄存器值
        public bool execute;//执行访问寄存器（读写）
        public string name;//寄存器所代表的名称
    }


    public class StringData
    {
        /// <summary>
        /// 读取不同状态标志
        /// </summary>
        public bool readDifferentStateFlag;

        /// <summary>
        ///旧的值
        /// </summary>
        public string OldValue;

        //S7
        public string dbName;
        public int db;
        public string startByteAdrName;
        public int startByteAdr;

        public string address;

        public string value;
        public bool execute;
        public string name;
    }

    public class CharData
    {
        /// <summary>
        /// 读取不同状态标志
        /// </summary>
        public bool readDifferentStateFlag;

        /// <summary>
        ///旧的值
        /// </summary>
        public char OldValue;

        //S7
        public string dbName;
        public int db;
        public string startByteAdrName;
        public int startByteAdr;

        public string address;

        public char value;
        public bool execute;
        public string name;
    }

    public class ShortData
    {
        /// <summary>
        /// 读取不同状态标志
        /// </summary>
        public bool readDifferentStateFlag;

        /// <summary>
        ///旧的值
        /// </summary>
        public short OldValue;

        //S7
        public string dbName;
        public int db;
        public string startByteAdrName;
        public int startByteAdr;

        public string address;

        public short value;
        public bool execute;
        public string name;
    }

    public class IntData
    {
        /// <summary>
        /// 读取不同状态标志
        /// </summary>
        public bool readDifferentStateFlag;

        /// <summary>
        ///旧的值
        /// </summary>
        public int OldValue;

        //S7
        public string dbName;
        public int db;
        public string startByteAdrName;
        public int startByteAdr;

        public string address;

        public int value;
        public bool execute;
        public string name;
    }

    public class LongData
    {
        /// <summary>
        /// 读取不同状态标志
        /// </summary>
        public bool readDifferentStateFlag;

        /// <summary>
        ///旧的值
        /// </summary>
        public long OldValue;

        //S7
        public string dbName;
        public int db;
        public string startByteAdrName;
        public int startByteAdr;

        public string address;

        public long value;
        public bool execute;
        public string name;
    }

    public class UShortData
    {
        /// <summary>
        /// 读取不同状态标志
        /// </summary>
        public bool readDifferentStateFlag;

        /// <summary>
        ///旧的值
        /// </summary>
        public ushort OldValue;

        //S7
        public string dbName;
        public int db;
        public string startByteAdrName;
        public int startByteAdr;

        public string address;

        public ushort value;
        public bool execute;
        public string name;
    }

    public class UIntData
    {
        /// <summary>
        /// 读取不同状态标志
        /// </summary>
        public bool readDifferentStateFlag;

        /// <summary>
        ///旧的值
        /// </summary>
        public uint OldValue;

        //S7
        public string dbName;
        public int db;
        public string startByteAdrName;
        public int startByteAdr;

        public string address;

        public uint value;
        public bool execute;
        public string name;
    }

    public class ULongData
    {
        /// <summary>
        /// 读取不同状态标志
        /// </summary>
        public bool readDifferentStateFlag;

        /// <summary>
        ///旧的值
        /// </summary>
        public ulong OldValue;

        //S7
        public string dbName;
        public int db;
        public string startByteAdrName;
        public int startByteAdr;

        public string address;

        public ulong value;
        public bool execute;
        public string name;
    }

    public class FloatData
    {
        /// <summary>
        /// 读取不同状态标志
        /// </summary>
        public bool readDifferentStateFlag;

        /// <summary>
        ///旧的值
        /// </summary>
        public float OldValue;

        //S7
        public string dbName;
        public int db;
        public string startByteAdrName;
        public int startByteAdr;

        public string address;

        public float value;
        public bool execute;
        public string name;
    }

    public class DoubleData
    {
        /// <summary>
        /// 读取不同状态标志
        /// </summary>
        public bool readDifferentStateFlag;

        /// <summary>
        ///旧的值
        /// </summary>
        public double OldValue;

        //S7
        public string dbName;
        public int db;
        public string startByteAdrName;
        public int startByteAdr;

        public string address;

        public double value;
        public bool execute;
        public string name;
    }
}

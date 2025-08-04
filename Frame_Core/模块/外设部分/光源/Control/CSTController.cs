using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;

namespace Frame_Core
{
    /// <summary>
    /// 康视达光源控制器
    /// Time：2021.01.15
    /// User：Xuyy
    /// </summary>
    class CSTController : LightControl
    {
        [DllImport("CSTControllerDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CST_EthernetConnectIP(string IP, ref Int64 Handle);

        [DllImport("CSTControllerDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CST_EthernetConnectStop(ref Int64 Handle);

        [DllImport("CSTControllerDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt16 CST_EthernetGetDigitalValue(int CH, ref Int64 Handle);

        [DllImport("CSTControllerDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt16 CST_EthernetSetLightState(int CH, ref Int64 Handle);

        [DllImport("CSTControllerDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt16 CST_EthernetSetStrobeValue(int CH, int Light, ref Int64 Handle);

        [DllImport("CSTControllerDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt16 CST_EthernetSetDigitalValue(int CH, int Light, ref Int64 Handle);

        [DllImport("CSTControllerDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt16 CST_CreateSerialPort(int ComNum, int Baud, ref Int64 Handle);

        [DllImport("CSTControllerDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt16 CST_ReleaseSerialPort(ref Int64 Handle);

        [DllImport("CSTControllerDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt16 CST_SerialPortSetDigitalValue(int CH, int Light, ref Int64 Handle);


        [DllImport("CSTControllerDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt16 CST_EthernetSetMulDigitalValue(MulDigitalValue[] mMulDigitalValue, int length, ref Int64 Handle);
        public struct MulDigitalValue
        {
            public int channelIndex;
            public int DigitalValue;
        }

        [DllImport("CSTControllerDll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CST_CST_GetAdapter(IntPtr mAdapterPrmPtr);
        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct Adapter_prm
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 132)]
            public char[] cSn;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public char[] cIp;
        }


        Int64 mHandle = 0;


        //实现继承
        public override bool Connect(string PortName, int BaudRate, int StopBit, int Parity, int DataBit, int ReceiveBufLength)
        {
            string[] str = PortName.Split('M');
            int ComNum = int.Parse(str[1]);
           //int ComNum = Convert.ToInt32(PortName); //获取串口号
            int RT3 = CST_CreateSerialPort(ComNum, BaudRate, ref mHandle);  //创建串口
            if (RT3 == 10000)
            {
                return true;
            }
            return false;
        }
        public override int Connect(string SerialPortName)
        {
            return 0;
        }
        public override void ChangeBaudRate(int BaudRate)
        {
            //无
        }
        public override void ChangeValue(int channel, int Value)
        {
            int RT4 = CST_SerialPortSetDigitalValue(channel, Value, ref mHandle);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetBaudRate()
        {
            return 0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        public override void Open(int channel)
        {
            int RT4 = CST_SerialPortSetDigitalValue(channel, 150, ref mHandle);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        public override void Shutdown(int channel)
        {
            int RT4 = CST_SerialPortSetDigitalValue(channel, 0, ref mHandle);
        }
        /// <summary>
        /// 
        /// </summary>
        public override void Dispose()
        {
            int RT3 = CST_ReleaseSerialPort(ref mHandle);  //断开串口

        }
        public override void Setlnitial(char c)
        {
            
        }
    }
}

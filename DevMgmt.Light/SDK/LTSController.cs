using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace DevMgmt.Light.SDK
{
    //using LTSControllerHandleType = Int32;
    public class LTSControllerApi : LightControl
    {
        const string LTSControler_DLL = "LTSController.dll";
        //private LTSControllerHandleType ControllerHandle = 0;

        [StructLayout(LayoutKind.Sequential)]
        public struct IntensityItem
        {
            public int channel;
            public int intensity;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct TriggerWidthItem
        {
            public int channel;
            public int triggerWidth;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct HBTriggerWidthItem
        {
            public int channel;
            public int HBTriggerWidth;
        };

        [DllImport(LTSControler_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe
        int
        LTSController_InitSerialPort(String SerialPortName);

        [DllImport(LTSControler_DLL)]
        public static extern unsafe
        int
        LTSController_ReleaseSerialPort();

        [DllImport(LTSControler_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe
        int
        LTSController_InitNetwork(String IP, int nPort);

        [DllImport(LTSControler_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe
        int
        LTSController_ReleaseNetwork();

        [DllImport(LTSControler_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe
        int
        LTSController_TurnOnChannelLength(int Channel, int intensity);

        [DllImport(LTSControler_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe
        int
        LTSController_TurnOffChannelLength(int Channel, int intensity);

        [DllImport(LTSControler_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe
        int
        LTSController_TurnOnMultiChannelLength();

        [DllImport(LTSControler_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe
        int
        LTSController_TurnOffMultiChannelLength();

        [DllImport(LTSControler_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe
        int
        LTSController_ReadIntensityLength(int Channel);

        [DllImport(LTSControler_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe
        int
        LTSController_SetIntensityLength(int channelIdx, int intensity);

        [DllImport(LTSControler_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe
       void
       LTSController_SetInitial(char c);


        //实现继承
        public override bool Connect(string PortName, int BaudRate, int StopBit, int Parity, int DataBit, int ReceiveBufLength)
        {
            return true;
        }
        public override int Connect(string SerialPortName)
        {

           return LTSController_InitSerialPort(SerialPortName);
        }
        public override void ChangeBaudRate(int BaudRate)
        {
            //无
        }
        public override void ChangeValue(int channel, int Value)
        {
            LTSController_SetIntensityLength(channel, Value);
        }
        public override int GetBaudRate()
        {
            return 0;
        }
        public override void Open(int channel)
        {
            LTSController_TurnOnChannelLength(channel, 1);
        }
        public override void Shutdown(int channel)
        {
            LTSController_TurnOffChannelLength(channel, 0);
        }
        public override void Dispose()
        {
            LTSController_ReleaseSerialPort();
        }
        public override void Setlnitial(char c)
        {
            LTSController_SetInitial(c);
        }

    }
}

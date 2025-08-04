using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Frame_Core
{
    public class OPT2:LightControl
    {
        RS232 OPT = new RS232();
        public override bool Connect(string PortName, int BaudRate, int StopBit, int Parity, int DataBit, int ReceiveBufLength)
        {
            return OPT.Connect(PortName,  BaudRate,  StopBit,  Parity,  DataBit,  ReceiveBufLength);
        }
        public override int Connect(string SerialPortName)
        {
            return OPT.Connect(SerialPortName);
        }
        public override void ChangeBaudRate(int BaudRate)
        {
            byte[] ss = new byte[6];
            switch (BaudRate)
            {
                case 9600:
                    ss[0] = 0xFF;
                    ss[1] = 0x42;
                    ss[2] = 0x01;
                    ss[3] = 0x00;
                    ss[4] = 0x00;
                    ss[5] = 0xBC;
                    break;
                case 19200:
                    ss[0] = 0xFF;
                    ss[1] = 0x42;
                    ss[2] = 0x01;
                    ss[3] = 0x00;
                    ss[4] = 0x01;
                    ss[5] = 0xBD;
                    break;
                case 115200:
                    ss[0] = 0xFF;
                    ss[1] = 0x42;
                    ss[2] = 0x01;
                    ss[3] = 0x00;
                    ss[4] = 0x02;
                    ss[5] = 0xBE;
                    break;
                case 57600:
                    ss[0] = 0xFF;
                    ss[1] = 0x42;
                    ss[2] = 0x01;
                    ss[3] = 0x00;
                    ss[4] = 0x03;
                    ss[5] = 0xBF;
                    break;
                case 56000:
                    ss[0] = 0xFF;
                    ss[1] = 0x42;
                    ss[2] = 0x01;
                    ss[3] = 0x00;
                    ss[4] = 0x04;
                    ss[5] = 0xB8;
                    break;
                case 38400:
                    ss[0] = 0xFF;
                    ss[1] = 0x42;
                    ss[2] = 0x01;
                    ss[3] = 0x00;
                    ss[4] = 0x05;
                    ss[5] = 0xB9;
                    break;
                case 28800:
                    ss[0] = 0xFF;
                    ss[1] = 0x42;
                    ss[2] = 0x01;
                    ss[3] = 0x00;
                    ss[4] = 0x06;
                    ss[5] = 0xBA;
                    break;
                case 14400:
                    ss[0] = 0xFF;
                    ss[1] = 0x42;
                    ss[2] = 0x01;
                    ss[3] = 0x00;
                    ss[4] = 0x07;
                    ss[5] = 0xBB;
                    break;
                case 76800:
                    //ss[0] = 0xFF;
                    //ss[1] = 0x42;
                    //ss[2] = 0x01;
                    //ss[3] = 0x00;
                    //ss[4] = 0x08;
                    //ss[5] = 0xB4;
                    break;
            }
            OPT.Send(ss);
            Thread.Sleep(2000);
            OPT.Connect(BaudRate);
        }
        public override void ChangeValue(int channel,int Value)
        {
            byte[] sendvalue = null;
            sendvalue = OPT.AStoSTRByte(channel, Value);
            OPT.Send(sendvalue);
            Thread.Sleep(5);
        }
        public override int GetBaudRate()
        {
            return OPT.GetBaudRate();
        }
        public override void Open(int channel)
        {
            byte[] sendvalue = null;
            sendvalue = OPT.AStoSTRByte(channel, 10);
            OPT.Send(sendvalue);
        }
        public override void Shutdown(int channel)
        {
            byte[] sendvalue = null;
            sendvalue = OPT.AStoSTRByte(channel, 0);
            OPT.Send(sendvalue);
        }
        public override void Dispose()
        {
            OPT.Close();
        }

        public override void Setlnitial(char c)
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Frame_Core
{
    public class OPT1:LightControl
    {
        RS232 OPT = new RS232();
        byte[] led_data1 = new byte[8] { 0X40, 0X05, 0X03, 0X00, 0X1A, 0X00, 0X00, 0X00 };
        byte[] led_data2 = new byte[8] { 0X40, 0X05, 0X03, 0X00, 0X1A, 0X00, 0X00, 0X00 };
        public override bool Connect(string PortName, int BaudRate, int StopBit, int Parity, int DataBit, int ReceiveBufLength)
        {
            return OPT.Connect(PortName, BaudRate, StopBit, Parity, DataBit, ReceiveBufLength);
        }
        public override int Connect(string SerialPortName)
        {
            return OPT.Connect(SerialPortName);
        }
        public override void ChangeBaudRate(int BaudRate)
        {
            
        }
        public override void ChangeValue(int channel, int Value)
        {
            string sendvalue = "";
            sendvalue = OPT.AStoSTR(channel, Value);//发送字符串
            OPT.Send(sendvalue);
            Thread.Sleep(10);//
        }
        public override int GetBaudRate()
        {
            return OPT.GetBaudRate();
        }
        public override void Open(int channel)
        {
            string sendvalue = "";
            sendvalue = OPT.AStoSTR(channel, 10);//发送字符串
            OPT.Send(sendvalue);
            Thread.Sleep(20);
        }
        public override void Shutdown(int channel)
        {
            string sendvalue = "";
            sendvalue = OPT.AStoSTR(channel, 0);//发送字符串
            OPT.Send(sendvalue);
            Thread.Sleep(20);
        }
        public override void Dispose()
        {
            OPT.Close();
        }

        public void WDP1(int X)
        {
            led_data1[6] = Convert.ToByte(X & 0XFF);
            led_data1[7] = Convert.ToByte((98 + X) & 0XFF);

            OPT.Send(led_data1);

        }
        public void WDP2(int X)
        {
            led_data2[6] = Convert.ToByte(X & 0XFF);
            led_data2[7] = Convert.ToByte((98 + X) & 0XFF);
            OPT.Send(led_data2);
        }
        public override void Setlnitial(char c)
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
namespace Frame_Core
{
    class WDPController : LightControl
    {
        int productModel = 1;//产品型号
        public WDPController(int _productModel)//1:PSC3线光系列，2:PD5-4通道系列
        {
            productModel = _productModel;
        }

        SerialPort sp1 = new SerialPort();
        private bool sendData(byte[] cmd)
        {
            bool isOpen = sp1.IsOpen;
            if (isOpen)
            {
                sp1.Write(cmd, 0, cmd.Length);
            }
            //else
            //    System.Windows.Forms.MessageBox.Show("请先打开串口");
            return isOpen;
        }
        private void LightControl(int chanel, string light)
        {
            byte[] ss = new byte[8];
            byte sum = 0;
            ss[0] = 0x40;
            ss[1] = 0x05;
            if (productModel == 1)
            {
                ss[2] = 0x03;
            }
            else
            {
                ss[2] = 0x01;
            }



            ss[3] = 0x00;
            ss[4] = 0x1A;




            if (chanel == 1)
            {
                ss[5] = 0x00;
            }
            else if (chanel == 2)
                ss[5] = 0x01;
            else if (chanel == 3)
                ss[5] = 0x02;
            else if (chanel ==4)
                ss[5] = 0x03;
            else if (chanel ==5)
                ss[5] = 0x04;
            else if (chanel ==6)
                ss[5] = 0x05;
            else if (chanel ==7)
                ss[5] = 0x06;
            else if (chanel ==8)
                ss[5] = 0x07;
            ss[6] = (byte)Convert.ToInt32(light, 10);
            for (int i = 0; i < ss.Length - 1; i++)
            {
                sum += ss[i];
            }
            ss[ss.Length - 1] = sum;
            //ss[7] = 0x61;
            if (!sendData(ss)) { }
             //   System.Windows.Forms.MessageBox.Show("请先打开串口");

        }


        bool IsOpen = true;
        //========================================================================================================================
        //======【】实现继承
        public override bool Connect(string PortName, int BaudRate, int StopBit, int parity, int DataBit, int ReceiveBufLength)
        {
            if (!sp1.IsOpen)
            {
                try
                {
                    sp1.PortName = PortName;        //设置串口号
                    sp1.BaudRate = BaudRate;     //波特率
                    sp1.DataBits = DataBit;     //数据位

                    //校验位
                    switch (parity)
                    {
                        case 0:
                            sp1.Parity = Parity.None;       //无校验位
                            break;
                        case 1:
                            sp1.Parity = Parity.Odd;        //奇校验
                            break;
                        case 2:
                            sp1.Parity = Parity.Even;       //偶校验
                            break;
                        default:
                            System.Windows.Forms.MessageBox.Show("Error：参数不正确!", "Error");
                            break;
                    }

                    //停止位
                    switch (StopBit)
                    {
                        case 0:
                            sp1.StopBits = StopBits.None;
                            break;
                        case 1:
                            sp1.StopBits = StopBits.One;
                            break;
                        case 2:
                            sp1.StopBits = StopBits.OnePointFive;
                            break;
                        case 3:
                            sp1.StopBits = StopBits.Two;
                            break;
                        default:
                            System.Windows.Forms.MessageBox.Show("Error：参数不正确!", "Error");
                            break;
                    }
                    if (sp1.IsOpen == true)//如果打开状态，则先关闭一下 
                    {
                        sp1.Close();
                    }
                    sp1.Open();
                }
                catch (Exception ex)
                {
                    //System.Windows.Forms.MessageBox.Show("Error:" + ex.Message, "Error");
                    return false;
                }
            }
            else
            {
                sp1.Close();
                return false;
            }
            return true;


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
            LightControl(channel, Value.ToString());

        }
        public override int GetBaudRate()
        {
            return 0;
        }
        public override void Open(int channel)
        {
            byte[] ss = new byte[8];
            byte sum = 0;
            ss[0] = 0x40;
            ss[1] = 0x05;
            if (productModel == 1)
            {
                ss[2] = 0x03;
            }
            else
            {
                ss[2] = 0x01;
            }
            ss[3] = 0x00;
            ss[4] = 0x2A;
            if (channel == 1)
                ss[5] = 0x00;
            else
                ss[5] = 0x01;

            ss[6] = 0x01;


            for (int i = 0; i < ss.Length - 1; i++)
            {
                sum += ss[i];
            }
            ss[ss.Length - 1] = sum;
            if (!sendData(ss)) { }
              //  System.Windows.Forms.MessageBox.Show("请先打开串口");

        }
        public override void Shutdown(int channel)
        {
            byte[] ss = new byte[8];
            byte sum = 0;
            ss[0] = 0x40;
            ss[1] = 0x05;
            if (productModel == 1)
            {
                ss[2] = 0x03;
            }
            else
            {
                ss[2] = 0x01;
            }
            ss[3] = 0x00;
            ss[4] = 0x2A;
            if (channel == 1)
                ss[5] = 0x00;
            else
                ss[5] = 0x01;

            ss[6] = 0x00;


            for (int i = 0; i < ss.Length - 1; i++)
            {
                sum += ss[i];
            }
            ss[ss.Length - 1] = sum;
            sendData(ss);
        }
        public override void Dispose()
        {
            if (sp1.IsOpen == true)
            {
                sp1.Close();
            }

        }
        public override void Setlnitial(char c)
        {

        }
    }
}

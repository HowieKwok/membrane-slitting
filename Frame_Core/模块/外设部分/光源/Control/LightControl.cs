using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    public abstract class LightControl
    {
        /// <summary>初始化连接串口,端口号,波特率,停止位,校验位,标准数据长度,(接收数据长度0为不接收)
        /// 
        /// </summary>
        /// <param name="PortName">端口号</param>
        /// <param name="BaudRate">波特率</param>
        /// <param name="StopBit">停止位：None = 0,One = 1,Two = 2,OnePointFive = 3</param>
        /// <param name="Parity">校验位：None = 0,Odd = 1,Even = 2,Mark = 3,Space = 4</param>
        /// <param name="DataBit">标准数据长度</param>
        /// <param name="ReceiveBufLength">接收数据长度，0为不接收</param>
        /// <returns></returns>
        public abstract bool Connect(string PortName, int BaudRate, int StopBit, int Parity, int DataBit, int ReceiveBufLength);
        public abstract int Connect(string SerialPortName);
        public abstract void ChangeBaudRate(int BaudRate);
        public abstract void ChangeValue(int channel, int Value);
        public abstract int GetBaudRate();
        public abstract void Open(int channel);
        public abstract void Shutdown(int channel);
        public abstract void Dispose();
        public abstract void Setlnitial(char c);
    }
}

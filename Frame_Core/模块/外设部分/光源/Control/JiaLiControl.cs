using System;
using System.Text;
using System.IO.Ports;
using System.Threading;

namespace Frame_Core
{
    /// <summary>
    /// 嘉励光源控制器模块
    /// </summary>
    public class JiaLiControl : LightControl
    {
        #region >>member
        private SerialPort serialPort_1 = new SerialPort();
        private StringBuilder strRead = new StringBuilder();
        private StringBuilder strRend = new StringBuilder();
        #endregion

        #region >>共有重载
        /// <summary>
        /// 连接1
        /// </summary>
        public override bool Connect(string PortName, int BaudRate, int StopBit, int iParity, int DataBit, int ReceiveBufLength)
        {
            try
            {
                string[] str = PortName.Split('M');
                int ComNum = int.Parse(str[1]);

                serialPort_1.BaudRate = BaudRate;
                serialPort_1.PortName = PortName;
                serialPort_1.DataBits = DataBit;
                serialPort_1.Parity = Parity.None;

                serialPort_1.Open();
                //serialPort_1.DataReceived += new SerialDataReceivedEventHandler(PortDataReceive);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 连接2
        /// </summary>
        public override int Connect(string SerialPortName)
        {
            return 0;
        }
        /// <summary>
        /// 设置比特率
        /// </summary>
        public override void ChangeBaudRate(int BaudRate)
        {
            return;
        }
        /// <summary>
        /// 获取波特率
        /// </summary>
        /// <returns></returns>
        public override int GetBaudRate()
        {
            return 0;
        }
        /// <summary>
        /// 设置光源通道数值
        /// </summary>
        public override void ChangeValue(int channel, int Value)
        {
            try
            {
                if (channel==2) return;

                string value_str = Convert.ToInt32(Value).ToString("X").PadLeft(3, '0');
                string msg = "$3" + channel.ToString() + value_str;
                msg = msg + GetXorResualt(msg);
                serialPort_1.Write(msg);
                Thread.Sleep(30);
                strRend = new StringBuilder(msg);
                return;
            }
            catch (Exception)
            {

            }

        }
        /// <summary>
        /// 打开光源【某通道】
        /// </summary>
        public override void Open(int channel)
        {
            ChangeValue(channel, 150);
        }
        /// <summary>
        /// 关闭光源某通道】
        /// </summary>
        /// <param name="channel"></param>
        public override void Shutdown(int channel)
        {
            ChangeValue(channel, 0);
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public override void Dispose()
        {
            serialPort_1.Close();
        }
        public override void Setlnitial(char c)
        {
            return;
        }
        #endregion

        #region >>>私有
        //获取对应通道亮度值
        public string GetIntensity(string ch)
        {
            try
            {
                string dataRec = "$4" + ch + "000";
                dataRec = dataRec + GetXorResualt(dataRec);
                serialPort_1.Write(dataRec);
                Thread.Sleep(100);
                dataRec = strRead.ToString();
                dataRec = dataRec.Substring(3, 3);
                int convertTodec = Convert.ToInt32(dataRec, 16);
                return convertTodec.ToString();
            }
            catch (Exception)
            {
                return "0";
            }
        }
        /// <summary>
        /// 接收回调函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PortDataReceive(object sender, SerialDataReceivedEventArgs e)
        {

            Thread.Sleep(20);
            try
            {
                int Readlen = serialPort_1.BytesToRead;
                if (Readlen == 1)
                {   //设置亮度时，控制器接收到指令后返回的指令为$或&，因此为1个字节
                    byte[] DataRec = new byte[1];
                    serialPort_1.Read(DataRec, 0, 1);
                    string RecDataBuffer = Encoding.ASCII.GetString(DataRec);
                    strRead.Clear();
                    strRead = new StringBuilder(RecDataBuffer);

                }
                if (Readlen == 8)
                {
                    //获取亮度时，新款控制器接收到指令后返回的指令为8个字节
                    byte[] DataRec = new byte[8];
                    serialPort_1.Read(DataRec, 0, 8);
                    string RecDataBuffer = Encoding.ASCII.GetString(DataRec);
                    strRead.Clear();
                    strRead = new StringBuilder(RecDataBuffer);
                }
                if (Readlen == 9)
                {
                    //获取亮度时，部分老款控制器接收到指令后返回的指令为9个字节，前置多一个"$"，因此删除
                    byte[] DataRec = new byte[9];
                    serialPort_1.Read(DataRec, 0, 9);
                    string RecDataBuffer = Encoding.ASCII.GetString(DataRec);
                    strRead.Clear();
                    RecDataBuffer = RecDataBuffer.Replace("$$", "$");
                    strRead = new StringBuilder(RecDataBuffer);
                }

                serialPort_1.DiscardInBuffer();

            }
            catch (Exception)
            {
                ;
            }
        }
        /// <summary>
        /// 校验码【求xor校验注意】
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        private string GetXorResualt(string msg)
        {
            //获取字节数组
            byte[] b = Encoding.ASCII.GetBytes(msg);
            // xorResult 存放校验结注意：初值首元素值
            byte xorResult = b[0];
            // 求xor校验注意：XOR运算第二元素始
            for (int i = 1; i < b.Length; i++)
            {
                xorResult ^= b[i];
            }
            // 运算xorResultXOR校验结，^=为异或符号
            // System.Windows.Forms.MessageBox.Show();
            return xorResult.ToString("X");
        }
        #endregion
    }
}

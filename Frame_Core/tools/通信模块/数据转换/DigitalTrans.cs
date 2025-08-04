using Frame_Core;
using LogTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    public class DigitalTrans
    {

        /// <summary>
        /// 二进制转10进制小数
        /// </summary>
        /// <param name="origin"></param>
        /// <returns></returns>
        public int[] _16to10Int(string[] arr)
        {
            int[] data = new int[arr.Length];
            try
            {
                for (int i = 0; i < arr.Length; i++) data[i] = -1;
                for (int i = 0; i < arr.Length; i++)
                {
                    data[i] = Convert.ToInt32(arr[i], 16);
                }
            }
            catch (Exception ex)
            {
                 Log4netRecord.ErrorLog.Debug(ex);
            }
            return data;
        }
        /// <summary>
        /// 16进制转10进制64位小数
        /// </summary>
        /// <param name="origin"></param>
        /// <returns></returns>
        public double _16to10double64(string origin)
        {
            //提取高位数值
            string high = origin.Substring(12, 4);
            string high1 = origin.Substring(8, 4);
            //提取地位数值
            string low = origin.Substring(4, 4);
            string low1 = origin.Substring(0, 4);
            //16进制转10进制
            long origin_to_10 = Convert.ToInt64(high + high1 + low + low1, 16);
            //10进制转2进制
            string origin_to_2 = Convert.ToString(origin_to_10, 2).PadLeft(64, '0');
            //第一位0正1负
            string sign = origin_to_2.Substring(0, 1);
            //1-11位指数
            string Exponent = origin_to_2.Substring(1, 11);
            //17-12位小数
            string Mantissa = origin_to_2.Substring(12, origin_to_2.Length - 12);
            //指数2进制转10进制
            int Exponent_to_10 = Convert.ToInt16(Exponent, 2);
            //小数2进制转10进制
            double Mantissa_to_double = _2to10double(Mantissa);
            //指数2的N次幂
            double Exponent_to_read = Math.Pow(2, Exponent_to_10 - 1023);
            //实际值=（1+小数）*指数
            double read = Exponent_to_read * (1 + Mantissa_to_double);
            //0正1负
            if (sign == "0")
                return read;
            else
                return -read;
        }

        /// <summary>
        /// 二进制转10进制小数
        /// </summary>
        /// <param name="origin"></param>
        /// <returns></returns>
        public double _2to10double(string origin)
        {
            int Length = origin.Length;
            double data = 0;
            for (int i = Length - 1; i >= 0; i--)
            {
                //二进制转10进制小数，从最后一位开始是1就加1，0不加，然后除以2
                string _d = origin.Substring(i, 1);
                int _int_d = Convert.ToInt16(_d);
                if (_int_d == 1)
                {
                    data += 1;
                }
                data = data / 2;
            }
            return data;
        }
        /// <summary>
        /// 16进制转10进制小数
        /// </summary>
        /// <param name="origin"></param>
        /// <returns></returns>
        public double _16to10double(string origin)
        {
            //提取高位数值
            string high = origin.Substring(4, 4);
            //提取地位数值
            string low = origin.Substring(0, 4);
            //16进制转10进制
            int origin_to_10 = Convert.ToInt32(high + low, 16);
            //10进制转2进制
            string origin_to_2 = Convert.ToString(origin_to_10, 2).PadLeft(32, '0');
            //第一位0正1负
            string sign = origin_to_2.Substring(0, 1);
            //1-9位指数
            string Exponent = origin_to_2.Substring(1, 8);
            //10-32位小数
            string Mantissa = origin_to_2.Substring(9, origin_to_2.Length - 9);
            //指数2进制转10进制
            int Exponent_to_10 = Convert.ToInt16(Exponent, 2);
            //小数2进制转10进制
            double Mantissa_to_double = _2to10double(Mantissa);
            //指数2的N次幂
            double Exponent_to_read = Math.Pow(2, Exponent_to_10 - 127);
            //实际值=（1+小数）*指数
            double read = Exponent_to_read * (1 + Mantissa_to_double);
            //0正1负
            if (Math.Abs(read - 0) < 0.000000001) read = 0;
            if (sign == "0")
                return read;
            else
                return -read;
        }

        /// <summary>
        /// 16进制转10进制32位整型
        /// </summary>
        /// <param name="origin"></param>
        /// <returns></returns>
        public int _16to10Int32(string origin)
        {
            if (origin != "")
            {
                string high = origin.Substring(4, 4);//提取高位数值
                string low = origin.Substring(0, 4);//提取地位数值
                string strdata = high + low;
                int data = Convert.ToInt32(strdata, 16);
                return data;
            }
            else
            {
                return 0;
            }
           
        }
        /// <summary>
        /// 16进制字符串转字符串
        /// </summary>
        /// <param name="hs"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        public string HexStringToString(string hs, Encoding encode)
        {
            byte[] b = new byte[hs.Length / 2];
            //逐个字符变为16进制字节数据
            for (int i = 0; i < hs.Length; i += 2)
            {
                b[i / 2] = Convert.ToByte(hs.Substring(i, 2), 16);
            }
            //按照指定编码将字节数组变为字符串          
            return encode.GetString(b).Trim(new char[] { '\0' });
            //return encode.GetString(b);
        }
        /// <summary>
        /// 字符串转16进制字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string StringToHexString(int sum, string str)
        {
            if (str == null)
            {
                str = string.Empty;
            }
            //将字符串填充满length个，不足时用 空字符'\0'填充，相当于清空所有字符后填充
            //防止出现 上一次写 【ABCD】，本次写 【12】， 读取字符串 结果是【12CD】 的问题
            str = str.PadRight(sum, '\0');

            string rtn_str = null;
            byte[] b = Encoding.UTF8.GetBytes(str);
            for (int i = 0; i < b.Length; i++)
            {
                rtn_str += b[i].ToString("X2");
            }

            return rtn_str;
        }
    }
}

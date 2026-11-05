using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    public class CheckInputData
    {
        /// <summary>
        /// 校验文本框输入是否匹配指定DataType
        /// </summary>
        /// <param name="inputText">TextBox输入内容</param>
        /// <param name="dataType">目标数据类型</param>
        /// <returns>true=格式合法；false=非法，禁止输入</returns>
        public static bool CheckInputDataType(string inputText, DataTypeEnum dataType)
        {
            // 存在空格直接判定非法
            if (inputText.Contains(" "))
                return false;

            // 空内容放行（清空输入框场景）
            if (dataType == DataTypeEnum.String)
                return true;

            // 数值类型统一用TryParse校验
            if (dataType == DataTypeEnum.Short)
                return short.TryParse(inputText, out _);
            if (dataType == DataTypeEnum.Int)
                return int.TryParse(inputText, out _);
            if (dataType == DataTypeEnum.Long)
                return long.TryParse(inputText, out _);
            if (dataType == DataTypeEnum.Float)
                return float.TryParse(inputText, out _);
            if (dataType == DataTypeEnum.Double)
                return double.TryParse(inputText, out _);
            if (dataType == DataTypeEnum.Bool)
                return bool.TryParse(inputText, out _);    

            // 不支持的类型直接返回非法
            return true;
        }

        /// <summary>
        /// 校验文本框输入是否匹配指定DataType
        /// </summary>
        /// <param name="inputText">TextBox输入内容</param>
        /// <param name="dataType">目标数据类型</param>
        /// <returns>true=格式合法；false=非法，禁止输入</returns>
        public static bool CheckInputDataType(string inputText, Type dataType)
        {
            // 存在空格直接判定非法
            if (inputText.Contains(" "))
                return false;

            // 空内容放行（清空输入框场景）
            if (dataType == typeof(string))
                return true;

            // 数值类型统一用TryParse校验
            if (dataType == typeof(short))
                return short.TryParse(inputText, out _);
            if (dataType == typeof(int))
                return int.TryParse(inputText, out _);
            if (dataType == typeof(long))
                return long.TryParse(inputText, out _);
            if (dataType == typeof(float))
                return float.TryParse(inputText, out _);
            if (dataType == typeof(double))
                return double.TryParse(inputText, out _);        
            if (dataType == typeof(bool))
                return bool.TryParse(inputText, out _);

            // 不支持的类型直接返回非法
            return true;
        }

        /// <summary>
        /// 判断当前类型是否为数字值类型（short/int/long/float/double）
        /// </summary>
        public static bool IsNumericType(Type type)
        {
            return type == typeof(short)
                || type == typeof(int)
                || type == typeof(long)
                || type == typeof(float)
                || type == typeof(double);
        }

        /// <summary>
        /// 数值类型仅允许输入：数字、负号-、小数点.
        /// </summary>
        public static bool IsAllowedNumericChar(char inputChar)
        {
            return char.IsDigit(inputChar) || inputChar == '-' || inputChar == '.';
        }
    }
}

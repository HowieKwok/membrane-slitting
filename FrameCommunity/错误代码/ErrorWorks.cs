using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FrameCommunity.错误代码
{
    public static class ErrorWorks
    {
        private static Dictionary<uint, ErrorInfo> dic_ErrorInfo = new Dictionary<uint, ErrorInfo>();

        /// <summary>
        /// 注册错误信息，将ErrorInfo枚举中的所有值注册到字典中
        /// </summary>
        public static void RegisterErrorInfo()
        {
            // 方法1：使用Enum.GetValues获取所有枚举值
            var allErrorInfos = Enum.GetValues(typeof(ErrorCodeEnum));

            foreach (ErrorCodeEnum error in allErrorInfos)
            {
                // 将枚举值转换为uint作为Key
                uint errorCode = Convert.ToUInt32(error);

                // 如果字典中已存在该错误代码，可以选择跳过或覆盖
                if (!dic_ErrorInfo.ContainsKey(errorCode))
                {
                    dic_ErrorInfo.Add(errorCode, new ErrorInfo() { ErrorCode= errorCode, Message= error .ToString()});
                }
            }
        }

        /// <summary>
        /// 获取错误信息
        /// </summary>
        /// <param name="errorCode">错误代码</param>
        /// <returns>错误信息枚举</returns>
        public static ErrorInfo GetErrorInfo(uint errorCode)
        {
            if (dic_ErrorInfo.TryGetValue(errorCode, out ErrorInfo errorInfo))
            {
                return errorInfo;
            }
            return default(ErrorInfo);
        }

        /// <summary>
        /// 获取错误描述（如果ErrorInfo枚举有Description特性）
        /// </summary>
        public static string GetErrorMessage(uint errorCode)
        {
            if (dic_ErrorInfo.TryGetValue(errorCode, out ErrorInfo errorInfo))
            {
                // 获取枚举成员的Description特性
                var fieldInfo = errorInfo.GetType().GetField(errorInfo.ToString());
                var descriptionAttribute = fieldInfo?.GetCustomAttribute<System.ComponentModel.DescriptionAttribute>();

                if (descriptionAttribute != null)
                {
                    return descriptionAttribute.Description;
                }

                // 如果没有Description特性，返回枚举名称
                return errorInfo.ToString();
            }
            return "未知错误";
        }

        /// <summary>
        /// 检查错误代码是否已注册
        /// </summary>
        public static bool ContainsErrorCode(uint errorCode)
        {
            return dic_ErrorInfo.ContainsKey(errorCode);
        }

        /// <summary>
        /// 清空所有注册的错误信息
        /// </summary>
        private static void Clear()
        {
            dic_ErrorInfo.Clear();
        }

        /// <summary>
        /// 错误信息显示
        /// </summary>
        /// <param name="errorInfo"></param>
        public static void ErrorMessageShow(string source, ErrorInfo errorInfo)
        {
            if (errorInfo == null)
            {
                MessageBox.Show("错误信息对象为空", "错误");
                return;
            }

            string message = $"错误代码: {errorInfo.ErrorCode}\r\n错误信息: {source}-{errorInfo.Message}";
            MessageBox.Show(message, "错误");
        }
    }
}

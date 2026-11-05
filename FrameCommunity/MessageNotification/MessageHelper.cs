using FrameCommunity.错误代码;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameCommunity.MessageNotification
{
    public static class MessageHelper
    {
        /// <summary>
        /// 为指定名称生成带序号的不重复字符串
        /// </summary>
        /// <param name="names">已有的名称列表</param>
        /// <param name="name">基础名称</param>
        /// <returns>返回 name + 最小可用序号，如 "张三1"</returns>
        public static string GenerateUniqueName(List<string> names, string name)
        {
            // 如果列表为空或不存在当前name的任何变体，直接返回 name + "1"
            if (names == null || !names.Any(n => n.StartsWith(name)))
            {
                return $"{name}1";
            }

            // 找出所有以 name 开头的项，提取序号部分
            var existingNumbers = names
                .Where(n => n.StartsWith(name))
                .Select(n =>
                {
                    // 提取序号部分（去掉前缀name）
                    string numberPart = n.Substring(name.Length);
                    // 尝试解析为整数，解析失败则返回0
                    if (int.TryParse(numberPart, out int number))
                    {
                        return number;
                    }
                    return 0;
                })
                .Where(num => num > 0) // 只保留有效的正数序号
                .ToList();

            // 如果没有找到有效的序号，从1开始
            if (!existingNumbers.Any())
            {
                return $"{name}1";
            }

            // 找到最小的缺失序号
            int minAvailable = 1;
            while (existingNumbers.Contains(minAvailable))
            {
                minAvailable++;
            }

            return $"{name}{minAvailable}";
        }

        /// <summary>
        /// 重命名操作，检查新名称是否与列表中已有名称重复
        /// </summary>
        /// <param name="names">已有的名称列表</param>
        /// <param name="newName">要设置的新名称</param>
        /// <param name="resultName">输出参数，返回最终可用的名称（如果不重复则返回newName，重复则返回原值）</param>
        /// <returns>成功返回true，名称重复返回false</returns>
        public static bool TryRename(List<string> names, string newName, out string resultName)
        {
            // 参数验证
            if (names == null)
            {
                ErrorWorks.ErrorMessageShow("names", ErrorWorks.GetErrorInfo((uint)ErrorCodeEnum.名称为空));
                resultName = string.Empty;
                return false;
            }

            if (string.IsNullOrWhiteSpace(newName))
            {
                resultName = string.Empty;
                ErrorWorks.ErrorMessageShow("newName", ErrorWorks.GetErrorInfo((uint)ErrorCodeEnum.名称为空));
                return false;
            }

            // 检查新名称是否已存在（忽略大小写，可根据需求调整）
            if (names.Contains(newName))
            {
                resultName = newName; // 返回重复的名称                            
                return false;
            }

            // 名称可用
            resultName = newName;
            return true;
        }

        /// <summary>
        /// 返回队列中的最小未使用ID编号（从1开始）
        /// </summary>
        /// <param name="queue">包含ID的队列</param>
        /// <returns>最小的未使用ID</returns>
        public static int FindMinimumId(List<int> queue)
        {
            // 将队列转换为HashSet以便O(1)查找
            HashSet<int> existingIds = new HashSet<int>(queue);

            // 从1开始查找最小的未使用ID
            int minId = 1;
            while (existingIds.Contains(minId))
            {
                minId++;
            }

            return minId;
        }
    }
}

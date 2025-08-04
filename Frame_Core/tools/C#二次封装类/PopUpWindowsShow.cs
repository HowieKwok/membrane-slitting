using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    public class PopUpWindowsShow
    {
        /// <summary>
        /// 消息显示
        /// </summary>
        /// <param name="showMessage"></param>
        public static void MessageShowNotHalcon(string showMessage)
        {
            try
            {
                showMessage = ReplaceStringIgnoreCaseHalcon(showMessage);
            }
            catch (Exception)
            {
            }
            System.Windows.Forms.MessageBox.Show(showMessage);
        }

        /// <summary>
        /// 替换字符串，忽略大小写
        /// </summary>
        /// <param name="original"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public static string ReplaceStringIgnoreCaseHalcon(string original)
        {
            string replaced = original;
            try
            {
                replaced = ReplaceStringIgnoreCase(original, "HalconDotNet", "HNet");
                replaced = ReplaceStringIgnoreCase(replaced, "Halcon", "Hc");
                replaced = ReplaceStringIgnoreCase(replaced, "HTuple ", "Ht");
                replaced = ReplaceStringIgnoreCase(replaced, "HObject ", "HObj");
            }
            catch (Exception)
            {

            }
            return replaced;
        }
        /// <summary>
        /// 替换字符串，忽略大小写
        /// </summary>
        /// <param name="original"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public static string ReplaceStringIgnoreCase(string original, string oldValue, string newValue)
        {
            string replaced = original;
            try
            {
                // 将原始字符串转换为小写
                string lowerCaseOriginal = original.ToLower();
                string lowerCaseOldValue = oldValue.ToLower();

                // 使用Replace方法进行替换
                replaced = lowerCaseOriginal.Replace(lowerCaseOldValue, newValue);

                // 将结果转换回原来的大小写
                for (int i = 0; i < original.Length; i++)
                {
                    if (char.IsUpper(original[i]))
                    {
                        replaced = replaced.Replace(newValue, newValue);
                        break;
                    }
                }
            }
            catch (Exception)
            {

            }
            return replaced;
        }
    }
}

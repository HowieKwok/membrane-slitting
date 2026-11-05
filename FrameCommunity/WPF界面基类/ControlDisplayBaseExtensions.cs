using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameCommunity
{
    public static class ControlDisplayBaseExtensions
    {
        /// <summary>
        /// 内部辅助方法：设置单个字段显示状态
        /// </summary>
        /// <param name="param">参数实体</param>
        /// <param name="propName">属性名</param>
        /// <param name="isShow">是否显示</param>
        public static ControlDisplayBase GetControlDisplayBase(this Dictionary<string, ControlDisplayBase> ControlDisplay, string propName)
        {
            if (ControlDisplay.TryGetValue(propName, out var displayConfig))
            {
                return displayConfig;
            }
            else
            {
                return null;
            }
        }

        /// <summary>安全设置IsShow，空对象直接跳过</summary>
        public static void SetShow(this ControlDisplayBase display, bool isShow)
        {
            if (display != null)
                display.IsShow = isShow;
        }
    }
}

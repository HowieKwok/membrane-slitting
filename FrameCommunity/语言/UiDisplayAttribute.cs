using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameCommunity.语言
{
    // 仅允许标记枚举字段
    [AttributeUsage(AttributeTargets.Field)]
    public class UiDisplayAttribute : Attribute
    {
        /// <summary>后台唯一键（英文标识，逻辑、序列化使用）</summary>
        public string Key { get; }    

        public UiDisplayAttribute(string key)
        {
            Key = key;        
        }
    }
}

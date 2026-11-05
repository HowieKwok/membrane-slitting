using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace FrameCommunity.语言
{
    public static class LanguageManager
    {
        public static LanguageEnum CurrentLanguage = LanguageEnum.Chinese;
        public static string Translate(string key)
        {
            if (CurrentLanguage == LanguageEnum.Chinese)
            {
                //根据语言库获取对应语言
                return key; 
            }

            return key; // 默认中文
        }
    }
}


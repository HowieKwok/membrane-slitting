using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    /// <summary>
    /// 插件信息 
    /// </summary>
    [Serializable]
    public class PluginsInfo
    {
        public Type PluginsObjType { get; set; }//处理类
        public Type PluginsFormType { get; set; }//窗体类
        public Type PluginsParamType { get; set; }//c类
        public string PluginsBuildName { get; set; }//插件生成名称（输出名）
        public string PluginsCategory { get; set; }//插件分类
        public string PluginsDisplayName { get; set; }//插件显示名称（列表显示）
    }
    /// <summary>
    /// 模块信息
    /// </summary>
    [Serializable]
    public class ModuleInfo : XmlSerializationOperation
    {
        public string ModuleName { get; set; } = "";            //插件名称 +  模块编号 。         可以重命名
        public string ModuleRemarks { get; set; } = "";         //模块注释  。                   可以改动
        public int ModuleEncode { get; set; } = 0;              //模块编号  ，                    不可改动
        public string PluginName { get; set; } = "";            //插件名称  每一个插件都是唯一	，   不可改动
        public bool IsUse { get; set; } = true;                 //是否启用                        可以改动
        public int CostTime { get; set; } = -1;                 //模块运行时间ms                  不可改动
        public int ProcessID { get; set; } = -1;                //所属的流程id                    不可改动
        public string ProcessName { get; set; } = "";			//所属的流程Name                  不可改动
    }
}

using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Frame_Core
{
    
    public interface IPlugin
    {
        /// <summary> 模块信息参数 </summary>
        ModuleInfo moduleInfo { get; set; }        
        PluginParamsBase PluginParams { get; set; }
        /// <summary> 执行 </summary>
        bool ExecuteObj();
        /// <summary>初始化</summary>  
       // void Initialize(PluginParamsBase obj);
        bool saveParams();
        /// <summary>
        /// 模块赋值
        /// </summary>
        /// <param name="obj"></param>
        void SetParams(PluginParamsBase param);
        /// <summary>
        /// 获取模块参数
        /// </summary>
        /// <returns></returns>
        void GetResultParams(string varName, object obj);
        /// <summary>
        /// 初始化模块
        /// </summary>
        void IniObj();
        /// <summary>
        /// 释放模块
        /// </summary>
        void DisPoseObj();
    } 


}

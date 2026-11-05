using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Frame_Core
{
    /// <summary>
    /// 插件执行完成事件参数。
    /// 
    /// 说明：
    /// 1. Param 指向执行结束后的参数对象。
    /// 2. Success 表示本次 ExecuteObj() 是否成功。
    /// 3. Message 是执行后写回的运行信息。
    /// 4. ElapsedMs 是执行耗时。
    /// </summary>
    public sealed class PluginExecuteCompletedEventArgs : EventArgs
    {
        

        public PluginExecuteCompletedEventArgs(PluginParamsBase param, bool success, string message, long elapsedMs)
        {
            Param = param;
            Success = success;
            Message = message ?? string.Empty;
            ElapsedMs = elapsedMs;
        }

        public PluginParamsBase Param { get; private set; }
        public bool Success { get; private set; }
        public string Message { get; private set; }
        public long ElapsedMs { get; private set; }
    }




    public interface IPlugin
    {
        event EventHandler<PluginExecuteCompletedEventArgs> ExecuteCompleted;

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

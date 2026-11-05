using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Frame_Core
{

    public class Process :IDisposable
    {
        //项目信息
        public ProcessInfo processInfo { set; get; } = new ProcessInfo();//项目信息

        public List<IPlugin> m_ModuleObjList = new List<IPlugin>();//模块列表


        //输出容器 每个模块 都把自己的输出放到该容器中
        public Dictionary<string, Dictionary<string, object>> m_OutPutMap = new Dictionary<string, Dictionary<string, object>>();

        #region 模块操作
        /// <summary>
        /// 添加模块到流程中，以编号区分。
        /// </summary>
        /// <param name="moduleObjBase"></param>
        public void AddModuleObj(IPlugin moduleObjBase)
        {
            //确定新模块的不重名名称
            bool flag = false;
            int encode = 0;
            do
            {
                flag = true;
                foreach (IPlugin moduleObj in m_ModuleObjList)
                {
                    ModuleInfo moduleParam = moduleObj.moduleInfo;
                    //插件名称相同 编号不重复
                    if (moduleObjBase.moduleInfo.PluginName == moduleParam.PluginName && moduleParam.ModuleEncode == encode)
                    {
                        encode++;
                        flag = false;
                        break;
                    }
                }

                if (flag == true)
                {
                    break;
                }
            } while (true);

            moduleObjBase.moduleInfo.ModuleEncode = encode;
            if (moduleObjBase.moduleInfo.ModuleName == "")
            {
                moduleObjBase.moduleInfo.ModuleName = moduleObjBase.moduleInfo.PluginName + "_" + encode;
            }
            moduleObjBase.moduleInfo.ProcessID = this.processInfo.ProcessID;
            moduleObjBase.moduleInfo.ProcessName = this.processInfo.ProcessName;
            moduleObjBase.IniObj();
            m_ModuleObjList.Add(moduleObjBase);
        }
        /// <summary>
        /// 从流程中删除模块
        /// </summary>
        /// <param name="moduleObjBase"></param>
        public void DeleteModuleObj(IPlugin moduleObjBase)
        {
            //确定新模块的不重名名称
            if (m_ModuleObjList.Contains(moduleObjBase))
            {
                moduleObjBase.DisPoseObj();
                m_ModuleObjList.Remove(moduleObjBase);
            }
        }

        /// <summary>
        /// 从流程中删除模块
        /// </summary>
        /// <param name="moduleObjBase"></param>
        public void DeleteAllModuleObj()
        {
            //确定新模块的不重名名称
            foreach (IPlugin item in m_ModuleObjList)
            {
                item.DisPoseObj();
            }
            m_ModuleObjList.Clear();
        }
        /// <summary>
        /// 获取所有模块名称
        /// </summary>
        /// <returns></returns>
        public List<string> GetModuleNameList()
        {
            return m_ModuleObjList.Select(c => c.moduleInfo.ModuleName).ToList();
        }

        /// <summary>
        /// 根据名称获取模块
        /// </summary>
        /// <returns></returns>
        public IPlugin GetModuleByName(string moduelName)
        {
            return m_ModuleObjList.FirstOrDefault(c => c.moduleInfo.ModuleName == moduelName);
        }

        /// <summary>
        /// 还原模块数据
        /// </summary>
        /// <param name="backModuleObjBase"></param>
        public void RecoverModuleObj(IPlugin backModuleObjBase)
        {
            int index = m_ModuleObjList.FindIndex(c => c.moduleInfo.ModuleName == backModuleObjBase.moduleInfo.ModuleName);
            m_ModuleObjList[index] = backModuleObjBase;
        }

        #endregion
        #region 模块输出操作
        /// <summary>
        /// 添加输出
        /// </summary>
        /// <param name="moduleName"></param>
        /// <param name="varName"></param>
        /// <param name="obj"></param>
        public void AddOutPutParam(string moduleName, string varName, object obj)
        {
            if (!m_OutPutMap.ContainsKey(moduleName))
            {
                m_OutPutMap[moduleName] = new Dictionary<string, object>();
            }
            Dictionary<string, object> dic = m_OutPutMap[moduleName];
            dic[varName] = obj;
            
        }
        /// <summary>
        /// 根据名称获取对应的模块的输出
        /// </summary>
        /// <param name="moduleName"></param>
        /// <param name="varName"></param>
        /// <returns></returns>
        public object GetOutPutParamByName(string linkName)
        {
            if (string.IsNullOrEmpty(linkName)) return null;
            string[] arr = linkName.Split('.');
            if (arr.Length == 2)
            {
                string moduleName = arr[0];
                string varName = arr[1];

                if (m_OutPutMap.ContainsKey(moduleName))
                {
                    Dictionary<string, object> dic = m_OutPutMap[moduleName];

                    if (dic.ContainsKey(varName))
                    {
                        return dic[varName];
                    }
                }

            }
            return null;
        }
        public object GetOutPutParamByName(string moduleName, string varName)
        {
            if (string.IsNullOrEmpty(moduleName)) return null;
            if (string.IsNullOrEmpty(varName)) return null;
            try
            {
                if (m_OutPutMap.ContainsKey(moduleName))
                {
                    Dictionary<string, object> dic = m_OutPutMap[moduleName];

                    if (dic.ContainsKey(varName))
                    {
                        return dic[varName];
                    }

                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }


        }

        #endregion

        public void SaveProcess(string ProcessPath)
        {
            // string ProcessPath = Path.Combine(Project.Instance.paramPathSet.ProcessPath, processInfo.ProcessName);
            foreach (IPlugin item in m_ModuleObjList)
            {
                item.moduleInfo.filePath = Path.Combine(ProcessPath, item.moduleInfo.ModuleName);
                item.saveParams();
            }
        }

        public void Dispose()
        {
            
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    /// <summary>
    /// 模块服务管理
    /// </summary>
    public class ModelServiceManage
    {
        /// <summary>
        /// 模块字典
        /// </summary>
        public Dictionary<string, ModelControl> m_dicModels = new Dictionary<string, ModelControl>();

        /// <summary>
        /// 添加模块
        /// </summary>
        /// <param name="modelName"></param>
        /// <param name="model"></param>
        public void AddModel(string modelName, ModelControl model)
        {
            m_dicModels[modelName] = model;
            m_dicModels[modelName].modelPrivilege.ModelName = modelName;
        }

        /// <summary>
        /// 获取模块
        /// </summary>
        /// <param name="modelName"></param>
        /// <returns></returns>
        public ModelControl GetModel(string modelName)
        {
            bool result = m_dicModels.TryGetValue(modelName, out ModelControl model);
            if (result) return model;
            else return null;
        }
    }
}

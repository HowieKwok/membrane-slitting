using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frame_Core;
namespace Frame_Core
{
    /// <summary>
    /// 权限参数
    /// </summary>
    public class PurviewParam : XmlSerializationOperation
    {
        /// <summary>
        /// 模块权限参数
        /// </summary>
        public List<ModelPrivilege> l_modelPrivilege { get; set; }

        public PurviewParam()
        {
            //init();
        }
        public void init()
        {
            l_modelPrivilege = new List<ModelPrivilege>();
        }
    }
}

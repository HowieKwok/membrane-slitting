using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    /// <summary>
    /// 界面权限
    /// </summary>
    public class ModelPrivilege
    {
        public string ModelName;//模块名称

        public bool OperatorPurview = false;//操作员
        public bool TechnicianPurview = false;//技术员
        public bool EngineerPurview = true;//工程师
        public bool AdminPurview = true;//管理员
        public bool SupervisorPurview = true;//超级管理员
    }

}

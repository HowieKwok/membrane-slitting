using Frame_Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    public class UserData : XmlSerializationOperation
    {
        //当前用户      
        [System.Xml.Serialization.XmlIgnore]
        public UserType CurrentUser { get; set; }

        //当前用户ID


        [System.Xml.Serialization.XmlIgnore]     
        public string CurrentUserID { get; set; }

        /// <summary>
        /// 登录过期时间(单位秒 最小值300秒)
        /// </summary>
        public ushort LoginOutTime { get; set; }

        //加密启用
        public bool isEncrypt { get; set; }//加密功能切换暂时不添加，默认不加密


        public List<UserModel> OperatorUser { get; set; }
        public List<UserModel> TechnicianUser { get; set; }
        public List<UserModel> EngineerUser { get; set; }
        public List<UserModel> AdminUser { get; set; }
        public List<UserModel> SupervisorUser { get; set; }

        public UserData()
        {
            //init();
        
        }
        public void init()
        {
            CurrentUser = UserType.Null;
            CurrentUserID = "用户未登录";
            LoginOutTime = 300;
            isEncrypt = false;
            OperatorUser = new List<UserModel>() { new UserModel() { userType = UserType.Operator, Id = "001", password = "001" } };
            TechnicianUser = new List<UserModel>() { new UserModel() { userType = UserType.Technician, Id = "002", password = "002" } };
            EngineerUser = new List<UserModel>() { new UserModel() { userType = UserType.Engineer, Id = "003", password = "003" } };
            AdminUser = new List<UserModel>() { new UserModel() { userType = UserType.Admin, Id = "004", password = "004" } };
            SupervisorUser = new List<UserModel>() { new UserModel() { userType = UserType.Supervisor, Id = "005", password = "005" } };
        }
    }
    public class UserModel
    {
        /// <summary>
        /// 用户类型
        /// </summary>
        public UserType userType { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 用户密码
        /// </summary>
        public string password { get; set; }

        public UserModel()
        {
            init();
        }
        public void init()
        {
            userType = UserType.Operator;
            Id = "001";
            password = "001";
        }
    }


  
}

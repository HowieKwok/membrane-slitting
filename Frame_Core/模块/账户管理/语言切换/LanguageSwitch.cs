using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    /// <summary>
    /// 语言切换
    /// </summary>
    public partial class LanguageSwitch
    {
        /// <summary>
        /// 获取用户语言
        /// </summary>
        public static string ToUserLanguageg(UserType userType, UserLanguageg Operator)
        {
            string userTypeCh = string.Empty;
            switch (userType)
            {
                case UserType.Operator:
                    if (Operator == UserLanguageg.中文) userTypeCh = "操作员";
                    if (Operator == UserLanguageg.英文) userTypeCh = "Operator";
                    break;
                case UserType.Technician:
                    if (Operator == UserLanguageg.中文) userTypeCh = "技术员";
                    if (Operator == UserLanguageg.英文) userTypeCh = "Technician";
                    break;
                case UserType.Engineer:
                    if (Operator == UserLanguageg.中文) userTypeCh = "工程师";
                    if (Operator == UserLanguageg.英文) userTypeCh = "Engineer";
                    break;
                case UserType.Admin:
                    if (Operator == UserLanguageg.中文) userTypeCh = "管理员";
                    if (Operator == UserLanguageg.英文) userTypeCh = "Admin";
                    break;
                case UserType.Supervisor:
                    if (Operator == UserLanguageg.中文) userTypeCh = "超级管理员";
                    if (Operator == UserLanguageg.英文) userTypeCh = "Supervisor";
                    break;

            }
            return userTypeCh;
        }
        /// <summary>
        ///  获取用户类型
        /// </summary>
        /// <param name="userType"></param>
        /// <param name="Operator"></param>
        /// <returns></returns>
        public static UserType ToUserType(string userType)
        {
            if ((userType == "操作员") || (userType == "Operator"))
            {
                return UserType.Operator;
            }
            if ((userType == "技术员") || (userType == "Technician"))
            {
                return UserType.Technician;
            }
            if ((userType == "工程师") || (userType == "Engineer"))
            {
                return UserType.Engineer;
            }
            if ((userType == "管理员") || (userType == "Admin"))
            {
                return UserType.Admin;
            }
            if ((userType == "超级管理员") || (userType == "Supervisor"))
            {
                return UserType.Supervisor;
            }
            return UserType.Null;
        }
    }
}

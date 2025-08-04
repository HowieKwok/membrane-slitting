using Frame_Core;
using LogTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    public class UserManage
    {
      
        public UserData userData;

     
        public bool InitUserParam(string path, Boolean isCopy = false)
        {
            try
            {
                isCopy = false;
                if (!isCopy)
                {
                    if (userData == null) userData = new UserData();
                    userData.filePath = path;
                    userData.init();
                    userData = (UserData)userData.Read();
                    userData.filePath = path;
                    UserLogin(UserType.Null, "用户未登录", "");
                }
                else
                {
                    if (userData == null) userData = new UserData();
                    userData.filePath = path;
                    userData.Save();
                }
            }
            catch (Exception ex)
            {

                 Log4netRecord.ErrorLog.Debug(ex);
                return false;
            }
            return true;
        }

        #region 用户数据操作      
        object loginLock = new object();
        /// <summary>
        /// 用户登录
        /// </summary>
        public UserCtrlStatus UserLogin(UserType userType, string userId, string pwd)
        {
            lock (loginLock)
            {
                try
                {
                    if (userType != UserType.Null)
                    {
                        int index = 0;
                        List<UserModel> tempUser = null;
                        switch (userType)
                        {
                            case UserType.Operator:
                                tempUser = userData.OperatorUser;
                                break;
                            case UserType.Technician:
                                tempUser = userData.TechnicianUser;
                                break;
                            case UserType.Engineer:
                                tempUser = userData.EngineerUser;
                                break;
                            case UserType.Admin:
                                tempUser = userData.AdminUser;
                                break;
                            case UserType.Supervisor:
                                tempUser = userData.SupervisorUser;
                                break;
                        }
                        if (tempUser != null)
                        {
                            index = tempUser.FindIndex(p => p.Id == userId);
                            if (index < 0) return UserCtrlStatus.UserNameWrong;


                            if (index >= 0)
                            {
                                if (tempUser[index].password != pwd) return UserCtrlStatus.PasswordWrong;
                            }
                        }
                        else return UserCtrlStatus.OtherError;
                    }
     
                    userData.CurrentUser = userType;
                    userData.CurrentUserID = userId;
                    //登录成功更改界面显示事件
                    OnLoginChange(userType);
                }
                catch (Exception ex)
                {
                    return UserCtrlStatus.OtherError;

                }

                return UserCtrlStatus.PasswordRight;
            }
           
        }

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="userType">用户类型</param>
        /// <param name="userId">用户ID</param>
        /// <param name="pwd">密码</param>
        /// <returns>返回结果</returns>
        public UserCtrlStatus AddUser(UserType userType, string userId, string pwd, Boolean isencrypt = false)
        {

            try
            {
                bool isExist = false;
                int count = 0;
                count = userData.OperatorUser.Where(p => p.Id == userId).Count();
                if (count > 0) isExist = true;
                count = userData.TechnicianUser.Where(p => p.Id == userId).Count();
                if (count > 0) isExist = true;
                count = userData.EngineerUser.Where(p => p.Id == userId).Count();
                if (count > 0) isExist = true;
                count = userData.AdminUser.Where(p => p.Id == userId).Count();
                if (count > 0) isExist = true;
                count = userData.SupervisorUser.Where(p => p.Id == userId).Count();
                if (count > 0) isExist = true;

                if (isExist)
                {
                    return UserCtrlStatus.UserNameExist;
                }

                if (isencrypt)
                {
                    //密码需加密
                    userId = SecurityHelper.Encrypt(userId);
                    pwd = SecurityHelper.Encrypt(pwd);
                }

                switch (userType)
                {
                    case UserType.Operator:
                        userData.OperatorUser.Add(new UserModel() { userType = userType, Id = userId, password = pwd });
                        break;
                    case UserType.Technician:
                        userData.TechnicianUser.Add(new UserModel() { userType = userType, Id = userId, password = pwd });
                        break;
                    case UserType.Engineer:
                        userData.EngineerUser.Add(new UserModel() { userType = userType, Id = userId, password = pwd });
                        break;
                    case UserType.Admin:
                        userData.AdminUser.Add(new UserModel() { userType = userType, Id = userId, password = pwd });
                        break;
                    case UserType.Supervisor:
                        userData.SupervisorUser.Add(new UserModel() { userType = userType, Id = userId, password = pwd });
                        break;
                    default:
                        break;
                }

            }
            catch (Exception)
            {
                return UserCtrlStatus.AddUserError;
            }
            return UserCtrlStatus.AddUserSucceed;
        }
        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="userType">用户类型</param>
        /// <param name="userId">用户ID</param>
        /// <returns>返回结果</returns>
        public UserCtrlStatus DelUser(UserType userType, string userId)
        {
            try
            {
                bool isNoExist = false;
                int index = 0;
                switch (userType)
                {
                    case UserType.Operator:
                        index = userData.OperatorUser.FindIndex(p => p.Id == userId);
                        if (index >= 0) userData.OperatorUser.RemoveAt(index); else { isNoExist = true; }
                        break;
                    case UserType.Technician:
                        index = userData.TechnicianUser.FindIndex(p => p.Id == userId);
                        if (index >= 0) userData.TechnicianUser.RemoveAt(index); else { isNoExist = true; }
                        break;
                    case UserType.Engineer:
                        index = userData.EngineerUser.FindIndex(p => p.Id == userId);
                        if (index >= 0) userData.EngineerUser.RemoveAt(index); else { isNoExist = true; }
                        break;
                    case UserType.Admin:
                        index = userData.AdminUser.FindIndex(p => p.Id == userId);
                        if (index >= 0) userData.AdminUser.RemoveAt(index); else { isNoExist = true; }
                        break;
                    case UserType.Supervisor:
                        index = userData.SupervisorUser.FindIndex(p => p.Id == userId);
                        if (index >= 0) userData.SupervisorUser.RemoveAt(index); else { isNoExist = true; }
                        break;
                    default:
                        break;
                }

                if (isNoExist)
                {
                    return UserCtrlStatus.UserNameWrong;
                }
            }
            catch (Exception)
            {
                return UserCtrlStatus.DelUserError;
            }
            return UserCtrlStatus.DelUserSucceed;
        }
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="userType">用户类型</param>
        /// <param name="userId">用户ID</param>
        /// <param name="oldPwd">原密码</param>
        /// <param name="newPwd">新密码</param>
        /// <returns>返回结果</returns>
        public UserCtrlStatus ChangePwd(UserType userType, string userId, string oldPwd, string newPwd, Boolean isencrypt = false)
        {
            try
            {
                bool isNoExist = false;
                int index = 0;
                if (isencrypt)
                {
                    //新密码需要加密
                    userId = SecurityHelper.Encrypt(userId);
                    oldPwd = SecurityHelper.Encrypt(oldPwd);
                    newPwd = SecurityHelper.Encrypt(newPwd);
                }
                List<UserModel> tempUser = null;
                switch (userType)
                {
                    case UserType.Operator:
                        tempUser = userData.OperatorUser;
                        break;
                    case UserType.Technician:
                        tempUser = userData.TechnicianUser;
                        break;
                    case UserType.Engineer:
                        tempUser = userData.EngineerUser;
                        break;
                    case UserType.Admin:
                        tempUser = userData.AdminUser;
                        break;
                    case UserType.Supervisor:
                        tempUser = userData.SupervisorUser;
                        break;
                }
                if (tempUser != null)
                {
                    index = tempUser.FindIndex(p => p.Id == userId);
                    if (index >= 0)
                    {
                        if (tempUser[index].password != oldPwd) return UserCtrlStatus.PasswordWrong;
                        tempUser[index].password = newPwd;
                    }
                    else return UserCtrlStatus.UserNameWrong;
                }
                else return UserCtrlStatus.ModifyPwdError;

            }
            catch (Exception)
            {
                return UserCtrlStatus.ModifyPwdError;
            }
            return UserCtrlStatus.ModifyPwdSucceed;
        }

        #endregion


        #region
        /// <summary>
        ///  获取用户类型
        /// </summary>
        /// <param name="userType"></param>
        /// <param name="Operator"></param>
        /// <returns></returns>
        public  UserType ToUserType(string userType)
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

        /// <summary>
        /// 登陆状态发生改变时触发
        /// </summary>
        public delegate void LoginDelegate(UserType userType);
        public event LoginDelegate LoginEvent;
        readonly object loginChange_flag = new object();
        private void OnLoginChange(UserType type)
        {
            lock (loginChange_flag)
            {
                lastLoginTime = DateTime.Now;
                LoginEvent?.Invoke(type);
            }

        }


        DateTime lastLoginTime;
        /// <summary>
        /// 登录过期(长时间未操作)
        /// </summary>
        public void MonitorLogonOutTime()
        {
            try
            {
                System.TimeSpan ts = DateTime.Now - lastLoginTime;
                if (ts.TotalSeconds > (userData.LoginOutTime == 0 ? 300 : userData.LoginOutTime))
                {
                    OnLoginChange(UserType.Operator);//长时间未操作界面刷新为操作员权限                  
                    UserLogin(UserType.Null, "用户未登录", "");
                }
            }
            catch (Exception ex)
            {
                 Log4netRecord.ErrorLog.Debug(ex);
            }
        }

        #endregion

    }
    public enum UserLanguageg
    {
        中文,
        英文

    }
    public enum UserCtrlStatus
    {
        /// <summary>
        /// 用户名不存在
        /// </summary>
        UserNameWrong = 0,
        /// <summary>
        /// 用户名已存在
        /// </summary>
        UserNameExist = 1,
        /// <summary>
        /// 密码错误
        /// </summary>
        PasswordWrong = 2,
        /// <summary>
        /// 密码正确
        /// </summary>
        PasswordRight = 3,
        /// <summary>
        /// 添加用户失败
        /// </summary>
        AddUserError = 4,
        /// <summary>
        /// 添加用户成功
        /// </summary>
        AddUserSucceed = 5,
        /// <summary>
        /// 删除用户失败
        /// </summary>
        DelUserError = 6,
        /// <summary>
        /// 删除用户成功
        /// </summary>
        DelUserSucceed = 7,
        /// <summary>
        /// 修改密码失败
        /// </summary>
        ModifyPwdError = 8,
        /// <summary>
        /// 修改密码成功
        /// </summary>
        ModifyPwdSucceed = 9,
        /// <summary>
        /// 其他错误
        /// </summary>
        OtherError = 10,
    }
}

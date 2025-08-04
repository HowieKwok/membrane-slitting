using Frame_Core;
using LogTool;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Frame_Core
{
    /// <summary>
    /// 添加用户的窗体程序
    /// </summary>
    public partial class AddUserForm : Form
    {

        /// <summary>
        /// 注册用户方式:
        /// 【0】手动注册；
        /// 【1】刷卡注册;
        /// </summary>
        private int TypeRegistration = 0;
        UserManage userManage;
        public AddUserForm(UserManage _UserManage)
        {
            userManage = _UserManage;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {


                string userTypeStr = cmbUserType.Text;
                string userId = txtUserID.Text;
                string pwd = txtPwd.Text;
                string pwdCheck = txtPwdCheck.Text;

                string currentUser = LanguageSwitch.ToUserLanguageg(userManage.userData.CurrentUser, UserLanguageg.中文);
                string currentUserID = userManage.userData.CurrentUserID;
                UserType userType = LanguageSwitch.ToUserType(userTypeStr);

                if (string.IsNullOrEmpty(userTypeStr))
                {
                    System.Windows.Forms.MessageBox.Show("请选择用户类型！");
                    return;
                }
                if (string.IsNullOrEmpty(userId))
                {
                    System.Windows.Forms.MessageBox.Show("请输入用户ID！");
                    return;
                }
                if (string.IsNullOrEmpty(pwd))
                {
                    if (TypeRegistration == 0)//手动注册
                        System.Windows.Forms.MessageBox.Show("请输入登录密码！");
                    else
                        System.Windows.Forms.MessageBox.Show("请刷卡输入IC卡号！");
                    return;
                }
                if (TypeRegistration == 0)//手动注册
                {
                    if (string.IsNullOrEmpty(pwdCheck))
                    {
                        System.Windows.Forms.MessageBox.Show("请再次输入登录密码！");
                        return;
                    }
                    if (pwd != pwdCheck)
                    {
                        System.Windows.Forms.MessageBox.Show("两次输入的密码不一致，请确认！");
                        return;
                    }
                }
                else
                {
                    //pwd = pwd.Replace("\n", "");
                    //string oUserTypeEN = "", oUserId = "", oUserPwd = "";
                    //bool bo_return = Mainform.FindUsers("", "", pwd, out oUserTypeEN, out oUserId, out oUserPwd);
                    //if (bo_return)
                    //{
                    //    System.Windows.Forms.MessageBox.Show("本地IC卡号：" + oUserPwd + ",本地用户ID:" + oUserId + "\n已存在,请确认！");
                    //    return;
                    //}
                }


                UserCtrlStatus result = userManage.AddUser(userType, userId, pwd);

                if (result == UserCtrlStatus.UserNameExist)
                {
                    System.Windows.Forms.MessageBox.Show("用户ID已存在，请确认！");
                }
                else if (result == UserCtrlStatus.AddUserError)
                {
                    System.Windows.Forms.MessageBox.Show("添加失败！");
                }
                else if (result == UserCtrlStatus.AddUserSucceed)
                {
                    userManage.userData.Save();                 
                    string message = string.Format("添加账户  {0} {1}", userTypeStr, userId);
                    ParamChangeLog.ParamChangeLog_Inspect("", "", currentUser, currentUserID, "账户管理-用户管理", message, true);
                    System.Windows.Forms.MessageBox.Show("添加成功！");
                }
                else
                {
                    //log
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ckbDispPwd_CheckedChanged(object sender, EventArgs e)
        {
            if (ckbDispPwd.Checked)
            {
                txtPwd.PasswordChar = '\0';
                txtPwdCheck.PasswordChar = '\0';
            }
            else
            {
                txtPwd.PasswordChar = '*';
                txtPwdCheck.PasswordChar = '*';
            }
        }

        private void AddAccount_Load(object sender, EventArgs e)
        {
            try
            {
                string strOperator = LanguageSwitch.ToUserLanguageg(UserType.Operator, UserLanguageg.中文);
                string strTechnician = LanguageSwitch.ToUserLanguageg(UserType.Technician, UserLanguageg.中文);
                string strEngineer = LanguageSwitch.ToUserLanguageg(UserType.Engineer, UserLanguageg.中文);
                string strAdmin = LanguageSwitch.ToUserLanguageg(UserType.Admin, UserLanguageg.中文);
                string strSupervisor = LanguageSwitch.ToUserLanguageg(UserType.Supervisor, UserLanguageg.中文);
                //==================================================================================================================
                //=====【】高级可以添加低级账户
                #region >>>高级可以添加低级账户
                cmbUserType.Items.Clear();
                if (userManage.userData.CurrentUser == UserType.Operator)//目前没有应用，为了后续需求
                {

                }
                else if (userManage.userData.CurrentUser == UserType.Technician)
                {
                    cmbUserType.Items.Add(strOperator);
                    //cmbUserType.Items.Add(userManage.strTechnician);
                }
                else if (userManage.userData.CurrentUser == UserType.Engineer)
                {
                    cmbUserType.Items.Add(strOperator);
                    cmbUserType.Items.Add(strTechnician);
                    //cmbUserType.Items.Add(UserName.EngineerCN);
                }
                else if (userManage.userData.CurrentUser == UserType.Admin)
                {
                    cmbUserType.Items.Add(strOperator);
                    cmbUserType.Items.Add(strTechnician);
                    cmbUserType.Items.Add(strEngineer);
                    //cmbUserType.Items.Add(UserName.AdminCN);
                }
                else
                {
                    cmbUserType.Items.Add(strOperator);
                    cmbUserType.Items.Add(strTechnician);
                    cmbUserType.Items.Add(strEngineer);
                    cmbUserType.Items.Add(strAdmin);
                    cmbUserType.Items.Add(strSupervisor);
                }
                if (cmbUserType.Items.Count > 0)
                {
                    cmbUserType.SelectedIndex = 0;
                }
              
                #endregion
            }
            catch (Exception ex)
            {

                 Log4netRecord.ErrorLog.Debug(ex);
            }

        }

        private void rbtn_Type_SD_CheckedChanged(object sender, EventArgs e)
        {
            TypeRegistration = 0;
            if (TypeRegistration > 0)
            {
                label4.Visible = false;
                txtPwdCheck.Visible = false;
                ckbDispPwd.Checked = true;
                ckbDispPwd.Visible = false;
                label3.Text = "IC 卡号:";
            }
            else
            {
                label4.Visible = true;
                txtPwdCheck.Visible = true;
                ckbDispPwd.Visible = true;
                label3.Text = "登录密码:";
            }
        }

        private void rbtn_Type_SwingCard_CheckedChanged(object sender, EventArgs e)
        {
            TypeRegistration = 1;
            if (TypeRegistration > 0)
            {
                label4.Visible = false;
                txtPwdCheck.Visible = false;
                ckbDispPwd.Checked = true;
                ckbDispPwd.Visible = false;
                label3.Text = "IC 卡号:";
            }
            else
            {
                label4.Visible = true;
                txtPwdCheck.Visible = true;
                ckbDispPwd.Visible = true;
                label3.Text = "登录密码:";
            }
        }
    }
}

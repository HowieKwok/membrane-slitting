using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Frame_Core
{
    public partial class UserLogionForm : Form
    {
        UserManage userManage;
        public UserLogionForm(UserManage _UserManage)
        {
            userManage = _UserManage;
            InitializeComponent();
        }

        private void UserSetting_Load(object sender, EventArgs e)
        {
            try
            {
                cmbUserType.Items.Clear();
                cmbUserType.Items.Add(LanguageSwitch.ToUserLanguageg(UserType.Operator,UserLanguageg.中文));//操作员
                cmbUserType.Items.Add(LanguageSwitch.ToUserLanguageg(UserType.Technician, UserLanguageg.中文));//技术员
                cmbUserType.Items.Add(LanguageSwitch.ToUserLanguageg(UserType.Engineer, UserLanguageg.中文));//工程师
                cmbUserType.Items.Add(LanguageSwitch.ToUserLanguageg(UserType.Admin, UserLanguageg.中文));//管理员
                //cmbUserType.Items.Add(LanguageSwitch.ToUserLanguageg(UserType.Supervisor, UserLanguageg.中文));//超级管理员
                cmbUserType.SelectedIndex = 0;
            }
            catch (Exception)
            {

            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_login_Click(object sender, EventArgs e)
        {
            Login();
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ckbDispPwd_CheckedChanged(object sender, EventArgs e)
        {
            if (ckbDispPwd.Checked)
            {
                txtPwd.PasswordChar = '\0';
            }
            else
            {
                txtPwd.PasswordChar = '*';
            }
        }

        private void txtPwd_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                Login();
            }
        }

        private void txtPwd_TextChanged(object sender, EventArgs e)
        {

        }

        private void Login()
        {
            try
            {
                int seleIndex = cmbUserType.SelectedIndex;

                string userTypeStr = cmbUserType.Text;
                string userId = txtUserID.Text;
                string pwd = txtPwd.Text;

                if (string.IsNullOrEmpty(userTypeStr))
                {
                    System.Windows.Forms.MessageBox.Show("请选择用户类型！");
                    return;
                }

                UserType userType = UserType.Operator;
                var userTypeEnum = LanguageProject.MatchStringToEnum<UserType>(cmbUserType.SelectedItem.ToString());
                if (userTypeEnum != null)
                {
                    userType = userTypeEnum ?? UserType.Operator;
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("用户类型转换错误！");
                    return;
                }

                UserCtrlStatus userCtrlStatus = userManage.UserLogin(userType, userId, pwd);


                switch (userCtrlStatus)
                {
                    case UserCtrlStatus.UserNameWrong:
                        System.Windows.Forms.MessageBox.Show("用户不存在，请确认！");                     
                        break;
                    case UserCtrlStatus.UserNameExist:                      
                        break;
                    case UserCtrlStatus.PasswordWrong:
                        System.Windows.Forms.MessageBox.Show("密码错误，请确认！");                     
                        break;
                    case UserCtrlStatus.PasswordRight:
                        this.Close();
                        break;
                    case UserCtrlStatus.AddUserError:
                        System.Windows.Forms.MessageBox.Show("添加用户失败，请确认！");                    
                        break;
                    case UserCtrlStatus.AddUserSucceed:
                        break;
                    case UserCtrlStatus.DelUserError:
                        System.Windows.Forms.MessageBox.Show("删除用户失败，请确认！");                     
                        break;
                    case UserCtrlStatus.DelUserSucceed:
                        break;
                    case UserCtrlStatus.ModifyPwdError:
                        System.Windows.Forms.MessageBox.Show("修改密码失败，请确认！");                       
                        break;
                    case UserCtrlStatus.ModifyPwdSucceed:
                        break;
                    case UserCtrlStatus.OtherError:
                        System.Windows.Forms.MessageBox.Show("登录失败，请确认！");
                        break;
                    default:
                        break;
                }

            }
            catch (Exception)
            {

            }
           
        }

        private void Btn_Logout_Click(object sender, EventArgs e)
        {
            userManage.UserLogin(UserType.Null, "用户未登录", "");
        }
    }
}

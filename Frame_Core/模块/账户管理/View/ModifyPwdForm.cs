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
    /// 进行密码修改的窗体程序
    /// </summary>
    public partial class ModifyPwdForm : Form
    {
        UserManage userManage;
        /// <summary>
        /// 构造函数，注入当前用户
        /// </summary>
        /// <param name="currentUser">当前使用的用户</param>
        public ModifyPwdForm(UserManage _UserManage)
        {
            userManage = _UserManage;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string oldPwd = txtOldPwd.Text;
                string newPwd = txtNewPwd.Text;
                string newPwdCheck = txtNewPwdCheck.Text;

                string currentUser = LanguageSwitch.ToUserLanguageg(userManage.userData.CurrentUser, UserLanguageg.中文);
                string currentUserID = userManage.userData.CurrentUserID;
                if (oldPwd == null || oldPwd == "")
                {
                    System.Windows.Forms.MessageBox.Show("请输入原密码！");
                    return;
                }
                if (newPwd == null || newPwd == "")
                {
                    System.Windows.Forms.MessageBox.Show("请输入新密码！");
                    return;
                }
                if (newPwdCheck == null || newPwdCheck == "")
                {
                    System.Windows.Forms.MessageBox.Show("请再次输入新密码！");
                    return;
                }
                if (newPwd != newPwdCheck)
                {
                    System.Windows.Forms.MessageBox.Show("两次输入的密码不一致，请确认！");
                    return;
                }

                UserCtrlStatus modifyResult = userManage.ChangePwd(userManage.userData.CurrentUser, userManage.userData.CurrentUserID, oldPwd, newPwd);
                if (modifyResult == UserCtrlStatus.PasswordWrong)
                {
                    System.Windows.Forms.MessageBox.Show("原密码不正确，请确认！");
                    return;
                }
                if ((modifyResult == UserCtrlStatus.ModifyPwdError) || (modifyResult == UserCtrlStatus.UserNameWrong))
                {
                    System.Windows.Forms.MessageBox.Show("修改失败！");
                }
                else if (modifyResult == UserCtrlStatus.ModifyPwdSucceed)
                {

                    userManage.userData.Save();
                    ParamChangeLog.ParamChangeLog_Inspect(oldPwd, newPwd, currentUser, currentUserID, "账户管理-用户管理", "当前用户密码");
                    System.Windows.Forms.MessageBox.Show("修改成功！");

                    this.Close();
                }
            }
            catch (Exception ex)
            {

                 Log4netRecord.ErrorLog.Debug(ex);
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
                txtOldPwd.PasswordChar = '\0';
                txtNewPwd.PasswordChar = '\0';
                txtNewPwdCheck.PasswordChar = '\0';
            }
            else
            {
                txtOldPwd.PasswordChar = '*';
                txtNewPwd.PasswordChar = '*';
                txtNewPwdCheck.PasswordChar = '*';
            }
        }
    }
}

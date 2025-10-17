using Frame_Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Frame_Core
{
    /// <summary>
    /// 进行用户管理的窗体程序
    /// </summary>
    public partial class UserManagerForm : Form
    {
        UserManage userManage;
        public UserManagerForm(UserManage _UserManage)
        {
            userManage = _UserManage;
            InitializeComponent();
        }

        private void btnModifyPwd_Click(object sender, EventArgs e)
        {
            ModifyPwdForm frm = new ModifyPwdForm(userManage);
            frm.ShowDialog();
        }

        private void btnAddAccount_Click(object sender, EventArgs e)
        {
            AddUserForm frm = new AddUserForm(userManage);
            frm.ShowDialog();
            AllUserShow();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                ////userManage.userData = (UserData)userManage.userData.Read();//读取会刷新当前用户和ID会导致报错
                //string CurrentUserStr = LanguageSwitch.ToUserLanguageg(userManage.userData.CurrentUser, UserLanguageg.中文);
                //label1.Text = "当前用户：\n  " + CurrentUserStr + "\n  " + userManage.userData.CurrentUserID;


                string userType = LanguageProject.GetLangeuageAttribute(StatusStripEnum.当前用户类型, Project.Instance.systemInfo.m_CurrentLanguageType) +
           LanguageProject.GetLangeuageAttribute(Project.Instance.userManage.userData.CurrentUser, Project.Instance.systemInfo.m_CurrentLanguageType);
                string userID = LanguageProject.GetLangeuageAttribute(StatusStripEnum.当前用户ID, Project.Instance.systemInfo.m_CurrentLanguageType) + Project.Instance.userManage.userData.CurrentUserID;
                label1.Text = userType + "\r\n" + userID;

                AllUserShow();
                //==================================================================================================================
                //=====【】
                btnModifyPwd.Enabled = false;
                btnAddAccount.Enabled = false;
                btnDelAccount.Enabled = false;
                btnControlAccount.Enabled = false;
                cb_PwdFile_Enable.Enabled = false;
                txt_PwdFile_Path.Visible = false;
                lbl_path.Visible = false;
                if (userManage.userData.CurrentUser == UserType.Admin)
                {
                    btnControlAccount.Enabled = true;

                }
                if (userManage.userData.CurrentUser != UserType.Null)
                {
                    btnModifyPwd.Enabled = true;
                    if (userManage.userData.CurrentUser != UserType.Operator)
                    {
                        btnAddAccount.Enabled = true;
                        btnDelAccount.Enabled = true;
                    }
                }

            }
            catch (Exception ex)
            {

                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }

        }
        private void AllUserShow()
        {

            try
            {
                this.lvwUserList.Items.Clear();
                int num = 1;
                for (int i = 0; i < userManage.userData.OperatorUser.Count; i++)
                {
                    ListViewItem userItem = new ListViewItem((num++).ToString());
                    string UserTypeStr = LanguageSwitch.ToUserLanguageg(userManage.userData.OperatorUser[i].userType, UserLanguageg.中文);
                    string UseIDStr = userManage.userData.OperatorUser[i].Id;
                    userItem.SubItems.Add(UserTypeStr);//用户类型
                    userItem.SubItems.Add(UseIDStr);//用户ID
                    this.lvwUserList.Items.Add(userItem);
                }
                for (int i = 0; i < userManage.userData.TechnicianUser.Count; i++)
                {
                    ListViewItem userItem = new ListViewItem((num++).ToString());
                    string UserTypeStr = LanguageSwitch.ToUserLanguageg(userManage.userData.TechnicianUser[i].userType, UserLanguageg.中文);
                    string UseIDStr = userManage.userData.TechnicianUser[i].Id;
                    userItem.SubItems.Add(UserTypeStr);//用户类型
                    userItem.SubItems.Add(UseIDStr);//用户ID
                    this.lvwUserList.Items.Add(userItem);
                }

                for (int i = 0; i < userManage.userData.EngineerUser.Count; i++)
                {
                    ListViewItem userItem = new ListViewItem((num++).ToString());
                    string UserTypeStr = LanguageSwitch.ToUserLanguageg(userManage.userData.EngineerUser[i].userType, UserLanguageg.中文);
                    string UseIDStr = userManage.userData.EngineerUser[i].Id;
                    userItem.SubItems.Add(UserTypeStr);//用户类型
                    userItem.SubItems.Add(UseIDStr);//用户ID
                    this.lvwUserList.Items.Add(userItem);
                }

                for (int i = 0; i < userManage.userData.AdminUser.Count; i++)
                {
                    ListViewItem userItem = new ListViewItem((num++).ToString());
                    string UserTypeStr = LanguageSwitch.ToUserLanguageg(userManage.userData.AdminUser[i].userType, UserLanguageg.中文);
                    string UseIDStr = userManage.userData.AdminUser[i].Id;
                    userItem.SubItems.Add(UserTypeStr);//用户类型
                    userItem.SubItems.Add(UseIDStr);//用户ID
                    this.lvwUserList.Items.Add(userItem);
                }

                for (int i = 0; i < userManage.userData.SupervisorUser.Count; i++)
                {
                    ListViewItem userItem = new ListViewItem((num++).ToString());
                    string UserTypeStr = LanguageSwitch.ToUserLanguageg(userManage.userData.SupervisorUser[i].userType, UserLanguageg.中文);
                    string UseIDStr = userManage.userData.SupervisorUser[i].Id;
                    userItem.SubItems.Add(UserTypeStr);//用户类型
                    userItem.SubItems.Add(UseIDStr);//用户ID
                    this.lvwUserList.Items.Add(userItem);
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void btnDelAccount_Click(object sender, EventArgs e)
        {

            try
            {
                string currentUser = LanguageSwitch.ToUserLanguageg(userManage.userData.CurrentUser, UserLanguageg.中文);
                string currentUserID = userManage.userData.CurrentUserID;
                string selectUserTypeCN = "";
                string selectUserId = "";
                UserType userTypeTemp = UserType.Null;
                foreach (int i in lvwUserList.SelectedIndices)
                {
                    selectUserTypeCN = lvwUserList.Items[i].SubItems[1].Text;
                    selectUserId = lvwUserList.Items[i].SubItems[2].Text;
                    //userTypeTemp = LanguageSwitch.ToUserType(selectUserTypeCN);

                    var userTypeEnum = LanguageProject.MatchStringToEnum<UserType>(selectUserTypeCN);
                    if (userTypeEnum != null)
                    {
                        userTypeTemp = userTypeEnum ?? UserType.Operator;
                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show("用户类型转换错误！");
                        return;
                    }

                    if (userManage.userData.CurrentUser == userTypeTemp && userManage.userData.CurrentUserID == selectUserId)
                    {
                        System.Windows.Forms.MessageBox.Show("禁止删除当前用户！");
                        return;
                    }
                    if (userManage.userData.CurrentUser != UserType.Admin)
                    {
                        if (userManage.userData.CurrentUser <= userTypeTemp)
                        {
                            System.Windows.Forms.MessageBox.Show("当前用户无权删除高等级用户！如需要操作请联系管理员.");
                            return;
                        }
                    }


                }

                if (lvwUserList.SelectedIndices.Count > 0)
                {
                    string message = string.Format("删除账户 {0} {1}", selectUserTypeCN, selectUserId);
                    DialogResult result = System.Windows.Forms.MessageBox.Show(message, "询问", MessageBoxButtons.YesNo);
                    if (result.Equals(DialogResult.Yes))
                    {
                        UserCtrlStatus delResult = userManage.DelUser(userTypeTemp, selectUserId);
                        if (delResult == UserCtrlStatus.DelUserSucceed)
                        {

                            userManage.userData.Save();
                            ParamChangeLog.ParamChangeLog_Inspect("", "", currentUser, currentUserID, "账户管理-用户管理", message, true);
                            System.Windows.Forms.MessageBox.Show("删除成功！");
                        }
                        else
                        {
                            System.Windows.Forms.MessageBox.Show("删除失败！");
                            //log
                        }
                        AllUserShow();
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void btnControlAccount_Click(object sender, EventArgs e)
        {
            if (Project.Instance.purviewManage == null) return;

            PurviewForm frm = new PurviewForm(Project.Instance.purviewManage.purviewParam);
            frm.ShowDialog();
        }

        private void AccountManager_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                bool isSuccessfulOperation = false;
                isSuccessfulOperation = userManage.userData.Save();
                if (!isSuccessfulOperation) System.Windows.Forms.MessageBox.Show("权限分配参数存储失敗", "ERROR", MessageBoxButtons.OK);

            }
            catch (Exception ex)
            {

                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }


        }

        private void btn_PwdFile_Refresh_Click(object sender, EventArgs e)
        {

        }

        private void cb_PwdFile_Enable_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}

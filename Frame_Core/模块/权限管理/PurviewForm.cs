using Frame_Core;
using LogTool;
using System;
using System.Windows.Forms;

namespace Frame_Core
{
    public partial class PurviewForm : Form
    {
        PurviewParam purviewParam;
        public PurviewForm(PurviewParam _purviewParam)
        {
            purviewParam = _purviewParam;
            InitializeComponent();
        }
        private void Purview_Load(object sender, EventArgs e)
        {
            LoadConfig();
        }
        private void Purview_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveConfig();


        }
        private void LoadConfig()
        {
            try
            {
                //string pathTemp = purviewParam.filePath;
                //purviewParam = (PurviewParam)purviewParam.Read();
                //purviewParam.filePath = pathTemp;
                dGV_Account.Rows.Clear();

                dGV_Account.Columns[1].HeaderText = LanguageSwitch.ToUserLanguageg(UserType.Admin, UserLanguageg.中文);
                dGV_Account.Columns[2].HeaderText = LanguageSwitch.ToUserLanguageg(UserType.Engineer, UserLanguageg.中文); ;
                dGV_Account.Columns[3].HeaderText = LanguageSwitch.ToUserLanguageg(UserType.Technician, UserLanguageg.中文); ;
                dGV_Account.Columns[4].HeaderText = LanguageSwitch.ToUserLanguageg(UserType.Operator, UserLanguageg.中文); ;


                for (int i = 0; i < purviewParam.l_modelPrivilege.Count; i++)
                {
                    dGV_Account.Rows.Add();
                    dGV_Account.Rows[i].Cells[0].Value = purviewParam.l_modelPrivilege[i].ModelName;
                    dGV_Account.Rows[i].Cells[1].Value = purviewParam.l_modelPrivilege[i].AdminPurview;
                    dGV_Account.Rows[i].Cells[2].Value = purviewParam.l_modelPrivilege[i].EngineerPurview;
                    dGV_Account.Rows[i].Cells[3].Value = purviewParam.l_modelPrivilege[i].TechnicianPurview;
                    dGV_Account.Rows[i].Cells[4].Value = purviewParam.l_modelPrivilege[i].OperatorPurview;
                }



                for (int i = 0; i < dGV_Account.Rows.Count; i++)
                {
                    dGV_Account.Rows[i].Cells[0].ReadOnly = true;
                }
                for (int j = 0; j < dGV_Account.Columns.Count; j++)
                {
                    dGV_Account.Columns[j].SortMode = DataGridViewColumnSortMode.NotSortable;
                }
                //==============================================================================================================================
                //======【】
                //for (int iRow = 0; iRow < dGV_Account.Rows.Count; iRow++)
                //{
                //    for (int iCol = 0; iCol < dGV_Account.Columns.Count - 1; iCol++)
                //    {
                //        if ((bool)dGV_Account.Rows[iRow].Cells[iCol + 1].Value)
                //            this.dGV_Account.Rows[iRow].Cells[iCol + 1].Style.BackColor = System.Drawing.Color.Green;
                //    }
                //}
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
                 Log4netRecord.ErrorLog.Error(ex);
            }
        }
        private void SaveConfig()
        {
            try
            {
                dGV_Account.EndEdit();
                for (int i = 0; i < purviewParam.l_modelPrivilege.Count; i++)
                {
                    purviewParam.l_modelPrivilege[i].AdminPurview = (bool)dGV_Account.Rows[i].Cells[1].Value;
                    purviewParam.l_modelPrivilege[i].EngineerPurview = (bool)dGV_Account.Rows[i].Cells[2].Value;
                    purviewParam.l_modelPrivilege[i].TechnicianPurview = (bool)dGV_Account.Rows[i].Cells[3].Value;
                    purviewParam.l_modelPrivilege[i].OperatorPurview = (bool)dGV_Account.Rows[i].Cells[4].Value;
                  
                }
                purviewParam.Save();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
                 Log4netRecord.ErrorLog.Error(ex);
            }

        }
    }
}

namespace Frame_Core
{
    partial class UserManagerForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.btnModifyPwd = new System.Windows.Forms.Button();
            this.btnAddAccount = new System.Windows.Forms.Button();
            this.btnDelAccount = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnControlAccount = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lvwUserList = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox_Pwd = new System.Windows.Forms.GroupBox();
            this.btn_PwdFile_Refresh = new System.Windows.Forms.Button();
            this.cb_PwdFile_Enable = new System.Windows.Forms.CheckBox();
            this.txt_PwdFile_Path = new System.Windows.Forms.TextBox();
            this.lbl_path = new System.Windows.Forms.Label();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox_Pwd.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnModifyPwd
            // 
            this.btnModifyPwd.Location = new System.Drawing.Point(51, 154);
            this.btnModifyPwd.Name = "btnModifyPwd";
            this.btnModifyPwd.Size = new System.Drawing.Size(98, 37);
            this.btnModifyPwd.TabIndex = 0;
            this.btnModifyPwd.Text = "修改密码";
            this.btnModifyPwd.UseVisualStyleBackColor = true;
            this.btnModifyPwd.Click += new System.EventHandler(this.btnModifyPwd_Click);
            // 
            // btnAddAccount
            // 
            this.btnAddAccount.Location = new System.Drawing.Point(51, 214);
            this.btnAddAccount.Name = "btnAddAccount";
            this.btnAddAccount.Size = new System.Drawing.Size(98, 37);
            this.btnAddAccount.TabIndex = 1;
            this.btnAddAccount.Text = "添加账户";
            this.btnAddAccount.UseVisualStyleBackColor = true;
            this.btnAddAccount.Click += new System.EventHandler(this.btnAddAccount_Click);
            // 
            // btnDelAccount
            // 
            this.btnDelAccount.Location = new System.Drawing.Point(51, 274);
            this.btnDelAccount.Name = "btnDelAccount";
            this.btnDelAccount.Size = new System.Drawing.Size(98, 37);
            this.btnDelAccount.TabIndex = 2;
            this.btnDelAccount.Text = "删除账户";
            this.btnDelAccount.UseVisualStyleBackColor = true;
            this.btnDelAccount.Click += new System.EventHandler(this.btnDelAccount_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnControlAccount);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.btnModifyPwd);
            this.groupBox2.Controls.Add(this.btnAddAccount);
            this.groupBox2.Controls.Add(this.btnDelAccount);
            this.groupBox2.Location = new System.Drawing.Point(562, 13);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(229, 398);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "用户操作";
            // 
            // btnControlAccount
            // 
            this.btnControlAccount.Location = new System.Drawing.Point(51, 339);
            this.btnControlAccount.Name = "btnControlAccount";
            this.btnControlAccount.Size = new System.Drawing.Size(98, 35);
            this.btnControlAccount.TabIndex = 5;
            this.btnControlAccount.Text = "权限分配";
            this.btnControlAccount.UseVisualStyleBackColor = true;
            this.btnControlAccount.Click += new System.EventHandler(this.btnControlAccount_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Yellow;
            this.label2.Location = new System.Drawing.Point(7, 108);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(136, 32);
            this.label2.TabIndex = 4;
            this.label2.Text = "密码修改\r\n仅可修改当前用户";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(7, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 16);
            this.label1.TabIndex = 3;
            this.label1.Text = "当前用户";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lvwUserList);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(542, 398);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "用户列表";
            // 
            // lvwUserList
            // 
            this.lvwUserList.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.lvwUserList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.lvwUserList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvwUserList.FullRowSelect = true;
            this.lvwUserList.GridLines = true;
            this.lvwUserList.Location = new System.Drawing.Point(4, 23);
            this.lvwUserList.Margin = new System.Windows.Forms.Padding(4);
            this.lvwUserList.Name = "lvwUserList";
            this.lvwUserList.Size = new System.Drawing.Size(534, 371);
            this.lvwUserList.TabIndex = 0;
            this.lvwUserList.UseCompatibleStateImageBehavior = false;
            this.lvwUserList.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "序号";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "用户类型";
            this.columnHeader2.Width = 180;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "用户ID";
            this.columnHeader3.Width = 400;
            // 
            // groupBox_Pwd
            // 
            this.groupBox_Pwd.Controls.Add(this.btn_PwdFile_Refresh);
            this.groupBox_Pwd.Controls.Add(this.cb_PwdFile_Enable);
            this.groupBox_Pwd.Controls.Add(this.txt_PwdFile_Path);
            this.groupBox_Pwd.Controls.Add(this.lbl_path);
            this.groupBox_Pwd.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox_Pwd.Location = new System.Drawing.Point(9, 419);
            this.groupBox_Pwd.Name = "groupBox_Pwd";
            this.groupBox_Pwd.Size = new System.Drawing.Size(782, 70);
            this.groupBox_Pwd.TabIndex = 15;
            this.groupBox_Pwd.TabStop = false;
            this.groupBox_Pwd.Text = "账户信息与权限管控数据库文件刷新";
            this.groupBox_Pwd.Visible = false;
            // 
            // btn_PwdFile_Refresh
            // 
            this.btn_PwdFile_Refresh.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_PwdFile_Refresh.Location = new System.Drawing.Point(649, 17);
            this.btn_PwdFile_Refresh.Name = "btn_PwdFile_Refresh";
            this.btn_PwdFile_Refresh.Size = new System.Drawing.Size(127, 37);
            this.btn_PwdFile_Refresh.TabIndex = 17;
            this.btn_PwdFile_Refresh.Text = "确认刷新";
            this.btn_PwdFile_Refresh.UseVisualStyleBackColor = true;
            this.btn_PwdFile_Refresh.Click += new System.EventHandler(this.btn_PwdFile_Refresh_Click);
            // 
            // cb_PwdFile_Enable
            // 
            this.cb_PwdFile_Enable.AutoSize = true;
            this.cb_PwdFile_Enable.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cb_PwdFile_Enable.Location = new System.Drawing.Point(9, 30);
            this.cb_PwdFile_Enable.Name = "cb_PwdFile_Enable";
            this.cb_PwdFile_Enable.Size = new System.Drawing.Size(203, 20);
            this.cb_PwdFile_Enable.TabIndex = 16;
            this.cb_PwdFile_Enable.Text = "启用远程账户信息数据库";
            this.cb_PwdFile_Enable.UseVisualStyleBackColor = true;
            this.cb_PwdFile_Enable.CheckedChanged += new System.EventHandler(this.cb_PwdFile_Enable_CheckedChanged);
            // 
            // txt_PwdFile_Path
            // 
            this.txt_PwdFile_Path.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txt_PwdFile_Path.Location = new System.Drawing.Point(290, 24);
            this.txt_PwdFile_Path.Name = "txt_PwdFile_Path";
            this.txt_PwdFile_Path.Size = new System.Drawing.Size(346, 26);
            this.txt_PwdFile_Path.TabIndex = 14;
            // 
            // lbl_path
            // 
            this.lbl_path.AutoSize = true;
            this.lbl_path.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbl_path.Location = new System.Drawing.Point(216, 31);
            this.lbl_path.Name = "lbl_path";
            this.lbl_path.Size = new System.Drawing.Size(88, 16);
            this.lbl_path.TabIndex = 15;
            this.lbl_path.Text = "远程路径：";
            // 
            // AccountManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(803, 500);
            this.Controls.Add(this.groupBox_Pwd);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Font = new System.Drawing.Font("宋体", 12F);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AccountManager";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "用户管理";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AccountManager_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox_Pwd.ResumeLayout(false);
            this.groupBox_Pwd.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnModifyPwd;
        private System.Windows.Forms.Button btnAddAccount;
        private System.Windows.Forms.Button btnDelAccount;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListView lvwUserList;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnControlAccount;
        private System.Windows.Forms.GroupBox groupBox_Pwd;
        private System.Windows.Forms.Button btn_PwdFile_Refresh;
        private System.Windows.Forms.CheckBox cb_PwdFile_Enable;
        private System.Windows.Forms.TextBox txt_PwdFile_Path;
        private System.Windows.Forms.Label lbl_path;
    }
}


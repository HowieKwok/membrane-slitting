namespace Frame_Core
{
    partial class AddUserForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cmbUserType = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtUserID = new System.Windows.Forms.TextBox();
            this.txtPwd = new System.Windows.Forms.TextBox();
            this.txtPwdCheck = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.ckbDispPwd = new System.Windows.Forms.CheckBox();
            this.rbtn_Type_SD = new System.Windows.Forms.RadioButton();
            this.rbtn_Type_SwingCard = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // cmbUserType
            // 
            this.cmbUserType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbUserType.FormattingEnabled = true;
            this.cmbUserType.Location = new System.Drawing.Point(182, 73);
            this.cmbUserType.Margin = new System.Windows.Forms.Padding(4);
            this.cmbUserType.Name = "cmbUserType";
            this.cmbUserType.Size = new System.Drawing.Size(166, 24);
            this.cmbUserType.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(58, 77);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "用户类型:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(58, 127);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "用户 ID:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(58, 177);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 16);
            this.label3.TabIndex = 3;
            this.label3.Text = "登录密码:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(58, 227);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(112, 16);
            this.label4.TabIndex = 4;
            this.label4.Text = "再次确认密码:";
            // 
            // txtUserID
            // 
            this.txtUserID.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtUserID.Location = new System.Drawing.Point(182, 122);
            this.txtUserID.Name = "txtUserID";
            this.txtUserID.Size = new System.Drawing.Size(166, 26);
            this.txtUserID.TabIndex = 5;
            // 
            // txtPwd
            // 
            this.txtPwd.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtPwd.Location = new System.Drawing.Point(182, 172);
            this.txtPwd.Name = "txtPwd";
            this.txtPwd.PasswordChar = '*';
            this.txtPwd.Size = new System.Drawing.Size(166, 26);
            this.txtPwd.TabIndex = 6;
            // 
            // txtPwdCheck
            // 
            this.txtPwdCheck.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtPwdCheck.Location = new System.Drawing.Point(182, 222);
            this.txtPwdCheck.Name = "txtPwdCheck";
            this.txtPwdCheck.PasswordChar = '*';
            this.txtPwdCheck.Size = new System.Drawing.Size(166, 26);
            this.txtPwdCheck.TabIndex = 7;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(258, 324);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(90, 33);
            this.button1.TabIndex = 8;
            this.button1.Text = "添加";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(61, 324);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(90, 33);
            this.button2.TabIndex = 9;
            this.button2.Text = "取消";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // ckbDispPwd
            // 
            this.ckbDispPwd.AutoSize = true;
            this.ckbDispPwd.Location = new System.Drawing.Point(61, 274);
            this.ckbDispPwd.Name = "ckbDispPwd";
            this.ckbDispPwd.Size = new System.Drawing.Size(91, 20);
            this.ckbDispPwd.TabIndex = 10;
            this.ckbDispPwd.Text = "显示密码";
            this.ckbDispPwd.UseVisualStyleBackColor = true;
            this.ckbDispPwd.CheckedChanged += new System.EventHandler(this.ckbDispPwd_CheckedChanged);
            // 
            // rbtn_Type_SD
            // 
            this.rbtn_Type_SD.AutoSize = true;
            this.rbtn_Type_SD.Checked = true;
            this.rbtn_Type_SD.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rbtn_Type_SD.ForeColor = System.Drawing.Color.Black;
            this.rbtn_Type_SD.Location = new System.Drawing.Point(61, 12);
            this.rbtn_Type_SD.Name = "rbtn_Type_SD";
            this.rbtn_Type_SD.Size = new System.Drawing.Size(107, 23);
            this.rbtn_Type_SD.TabIndex = 12;
            this.rbtn_Type_SD.TabStop = true;
            this.rbtn_Type_SD.Text = "手动注册";
            this.rbtn_Type_SD.UseVisualStyleBackColor = true;
            this.rbtn_Type_SD.Visible = false;
            this.rbtn_Type_SD.CheckedChanged += new System.EventHandler(this.rbtn_Type_SD_CheckedChanged);
            // 
            // rbtn_Type_SwingCard
            // 
            this.rbtn_Type_SwingCard.AutoSize = true;
            this.rbtn_Type_SwingCard.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rbtn_Type_SwingCard.ForeColor = System.Drawing.Color.Black;
            this.rbtn_Type_SwingCard.Location = new System.Drawing.Point(210, 12);
            this.rbtn_Type_SwingCard.Name = "rbtn_Type_SwingCard";
            this.rbtn_Type_SwingCard.Size = new System.Drawing.Size(107, 23);
            this.rbtn_Type_SwingCard.TabIndex = 13;
            this.rbtn_Type_SwingCard.TabStop = true;
            this.rbtn_Type_SwingCard.Text = "刷卡注册";
            this.rbtn_Type_SwingCard.UseVisualStyleBackColor = true;
            this.rbtn_Type_SwingCard.Visible = false;
            this.rbtn_Type_SwingCard.CheckedChanged += new System.EventHandler(this.rbtn_Type_SwingCard_CheckedChanged);
            // 
            // AddAccount
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(447, 399);
            this.Controls.Add(this.rbtn_Type_SwingCard);
            this.Controls.Add(this.rbtn_Type_SD);
            this.Controls.Add(this.ckbDispPwd);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txtPwdCheck);
            this.Controls.Add(this.txtPwd);
            this.Controls.Add(this.txtUserID);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbUserType);
            this.Font = new System.Drawing.Font("宋体", 12F);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddAccount";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "添加账户";
            this.Load += new System.EventHandler(this.AddAccount_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbUserType;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtUserID;
        private System.Windows.Forms.TextBox txtPwd;
        private System.Windows.Forms.TextBox txtPwdCheck;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.CheckBox ckbDispPwd;
        private System.Windows.Forms.RadioButton rbtn_Type_SD;
        private System.Windows.Forms.RadioButton rbtn_Type_SwingCard;
    }
}
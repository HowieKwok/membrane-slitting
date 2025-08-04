namespace Frame_Core
{
    partial class ModifyPwdForm
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
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtOldPwd = new System.Windows.Forms.TextBox();
            this.txtNewPwd = new System.Windows.Forms.TextBox();
            this.txtNewPwdCheck = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.ckbDispPwd = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(322, 200);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(99, 34);
            this.button1.TabIndex = 0;
            this.button1.Text = "确认修改";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "原密码:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 69);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "新密码:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 113);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(96, 16);
            this.label3.TabIndex = 3;
            this.label3.Text = "新密码确认:";
            // 
            // txtOldPwd
            // 
            this.txtOldPwd.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtOldPwd.Location = new System.Drawing.Point(114, 22);
            this.txtOldPwd.Name = "txtOldPwd";
            this.txtOldPwd.PasswordChar = '*';
            this.txtOldPwd.Size = new System.Drawing.Size(300, 26);
            this.txtOldPwd.TabIndex = 4;
            // 
            // txtNewPwd
            // 
            this.txtNewPwd.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtNewPwd.Location = new System.Drawing.Point(114, 66);
            this.txtNewPwd.Name = "txtNewPwd";
            this.txtNewPwd.PasswordChar = '*';
            this.txtNewPwd.Size = new System.Drawing.Size(300, 26);
            this.txtNewPwd.TabIndex = 5;
            // 
            // txtNewPwdCheck
            // 
            this.txtNewPwdCheck.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtNewPwdCheck.Location = new System.Drawing.Point(114, 110);
            this.txtNewPwdCheck.Name = "txtNewPwdCheck";
            this.txtNewPwdCheck.PasswordChar = '*';
            this.txtNewPwdCheck.Size = new System.Drawing.Size(300, 26);
            this.txtNewPwdCheck.TabIndex = 6;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(58, 200);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(99, 34);
            this.button2.TabIndex = 7;
            this.button2.Text = "取消";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // ckbDispPwd
            // 
            this.ckbDispPwd.AutoSize = true;
            this.ckbDispPwd.Location = new System.Drawing.Point(15, 157);
            this.ckbDispPwd.Name = "ckbDispPwd";
            this.ckbDispPwd.Size = new System.Drawing.Size(91, 20);
            this.ckbDispPwd.TabIndex = 11;
            this.ckbDispPwd.Text = "显示密码";
            this.ckbDispPwd.UseVisualStyleBackColor = true;
            this.ckbDispPwd.CheckedChanged += new System.EventHandler(this.ckbDispPwd_CheckedChanged);
            // 
            // ModifyPwd
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(479, 261);
            this.Controls.Add(this.ckbDispPwd);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.txtNewPwdCheck);
            this.Controls.Add(this.txtNewPwd);
            this.Controls.Add(this.txtOldPwd);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Font = new System.Drawing.Font("宋体", 12F);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ModifyPwd";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "修改密码";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtOldPwd;
        private System.Windows.Forms.TextBox txtNewPwd;
        private System.Windows.Forms.TextBox txtNewPwdCheck;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.CheckBox ckbDispPwd;
    }
}
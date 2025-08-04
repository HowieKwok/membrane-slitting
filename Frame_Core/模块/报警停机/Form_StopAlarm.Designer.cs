namespace Frame_Core
{
    partial class Form_StopAlarm
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
            this.btn_Close = new System.Windows.Forms.Button();
            this.tableLayoutPanel_Mian = new System.Windows.Forms.TableLayoutPanel();
            this.tbx_Message = new System.Windows.Forms.TextBox();
            this.tbx_Name = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel_Mian.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_Close
            // 
            this.btn_Close.Location = new System.Drawing.Point(659, 16);
            this.btn_Close.Margin = new System.Windows.Forms.Padding(0);
            this.btn_Close.Name = "btn_Close";
            this.btn_Close.Size = new System.Drawing.Size(167, 54);
            this.btn_Close.TabIndex = 5;
            this.btn_Close.Text = "关闭";
            this.btn_Close.UseVisualStyleBackColor = true;
            this.btn_Close.Click += new System.EventHandler(this.btn_Close_Click);
            // 
            // tableLayoutPanel_Mian
            // 
            this.tableLayoutPanel_Mian.ColumnCount = 1;
            this.tableLayoutPanel_Mian.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel_Mian.Controls.Add(this.tbx_Message, 0, 1);
            this.tableLayoutPanel_Mian.Controls.Add(this.tbx_Name, 0, 0);
            this.tableLayoutPanel_Mian.Controls.Add(this.panel1, 0, 2);
            this.tableLayoutPanel_Mian.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel_Mian.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel_Mian.Name = "tableLayoutPanel_Mian";
            this.tableLayoutPanel_Mian.RowCount = 3;
            this.tableLayoutPanel_Mian.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 19.81352F));
            this.tableLayoutPanel_Mian.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 80.18648F));
            this.tableLayoutPanel_Mian.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 84F));
            this.tableLayoutPanel_Mian.Size = new System.Drawing.Size(883, 630);
            this.tableLayoutPanel_Mian.TabIndex = 9;
            // 
            // tbx_Message
            // 
            this.tbx_Message.BackColor = System.Drawing.Color.Black;
            this.tbx_Message.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbx_Message.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbx_Message.Font = new System.Drawing.Font("宋体", 20F);
            this.tbx_Message.ForeColor = System.Drawing.Color.Red;
            this.tbx_Message.Location = new System.Drawing.Point(50, 111);
            this.tbx_Message.Margin = new System.Windows.Forms.Padding(50, 3, 3, 3);
            this.tbx_Message.Multiline = true;
            this.tbx_Message.Name = "tbx_Message";
            this.tbx_Message.ReadOnly = true;
            this.tbx_Message.Size = new System.Drawing.Size(830, 431);
            this.tbx_Message.TabIndex = 6;
            this.tbx_Message.Text = "描述：\r\n解决方案：\r\n";
            // 
            // tbx_Name
            // 
            this.tbx_Name.BackColor = System.Drawing.Color.Black;
            this.tbx_Name.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbx_Name.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbx_Name.Font = new System.Drawing.Font("宋体", 25F);
            this.tbx_Name.ForeColor = System.Drawing.Color.Red;
            this.tbx_Name.Location = new System.Drawing.Point(10, 10);
            this.tbx_Name.Margin = new System.Windows.Forms.Padding(10, 10, 3, 3);
            this.tbx_Name.Multiline = true;
            this.tbx_Name.Name = "tbx_Name";
            this.tbx_Name.ReadOnly = true;
            this.tbx_Name.Size = new System.Drawing.Size(870, 95);
            this.tbx_Name.TabIndex = 6;
            this.tbx_Name.Text = "报警名称";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btn_Close);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 548);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(877, 79);
            this.panel1.TabIndex = 8;
            // 
            // Form_StopAlarm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(883, 630);
            this.ControlBox = false;
            this.Controls.Add(this.tableLayoutPanel_Mian);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form_StopAlarm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Alarm";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Form_StopAlarm_Load);
            this.tableLayoutPanel_Mian.ResumeLayout(false);
            this.tableLayoutPanel_Mian.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btn_Close;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_Mian;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox tbx_Name;
        private System.Windows.Forms.TextBox tbx_Message;
    }
}
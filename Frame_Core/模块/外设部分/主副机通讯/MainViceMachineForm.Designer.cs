namespace Frame_Core
{
    partial class MainSlaveMachineForm
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
            this.groupBox_Remoting = new System.Windows.Forms.GroupBox();
            this.cbx_IsConnect = new System.Windows.Forms.CheckBox();
            this.tbx_Client_IP = new System.Windows.Forms.TextBox();
            this.tbx_Server_IP = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBox_Remoting.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox_Remoting
            // 
            this.groupBox_Remoting.Controls.Add(this.cbx_IsConnect);
            this.groupBox_Remoting.Controls.Add(this.tbx_Client_IP);
            this.groupBox_Remoting.Controls.Add(this.tbx_Server_IP);
            this.groupBox_Remoting.Controls.Add(this.label4);
            this.groupBox_Remoting.Controls.Add(this.label8);
            this.groupBox_Remoting.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold);
            this.groupBox_Remoting.Location = new System.Drawing.Point(17, 36);
            this.groupBox_Remoting.Name = "groupBox_Remoting";
            this.groupBox_Remoting.Size = new System.Drawing.Size(487, 138);
            this.groupBox_Remoting.TabIndex = 91;
            this.groupBox_Remoting.TabStop = false;
            this.groupBox_Remoting.Text = "PC主/副机通讯-(Remoting)";
            // 
            // cbx_IsConnect
            // 
            this.cbx_IsConnect.AutoSize = true;
            this.cbx_IsConnect.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbx_IsConnect.Location = new System.Drawing.Point(9, 102);
            this.cbx_IsConnect.Name = "cbx_IsConnect";
            this.cbx_IsConnect.Size = new System.Drawing.Size(82, 18);
            this.cbx_IsConnect.TabIndex = 112;
            this.cbx_IsConnect.Text = "启用通讯";
            this.cbx_IsConnect.UseVisualStyleBackColor = true;
            // 
            // tbx_Client_IP
            // 
            this.tbx_Client_IP.Font = new System.Drawing.Font("宋体", 10.5F);
            this.tbx_Client_IP.Location = new System.Drawing.Point(131, 58);
            this.tbx_Client_IP.Margin = new System.Windows.Forms.Padding(5);
            this.tbx_Client_IP.Name = "tbx_Client_IP";
            this.tbx_Client_IP.Size = new System.Drawing.Size(149, 23);
            this.tbx_Client_IP.TabIndex = 111;
            this.tbx_Client_IP.Text = "192.168.6.100";
            // 
            // tbx_Server_IP
            // 
            this.tbx_Server_IP.Font = new System.Drawing.Font("宋体", 10.5F);
            this.tbx_Server_IP.Location = new System.Drawing.Point(131, 30);
            this.tbx_Server_IP.Margin = new System.Windows.Forms.Padding(5);
            this.tbx_Server_IP.Name = "tbx_Server_IP";
            this.tbx_Server_IP.Size = new System.Drawing.Size(149, 23);
            this.tbx_Server_IP.TabIndex = 110;
            this.tbx_Server_IP.Text = "192.168.6.11";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 10.5F);
            this.label4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label4.Location = new System.Drawing.Point(6, 62);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(91, 14);
            this.label4.TabIndex = 9;
            this.label4.Text = "主机客户端IP";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("宋体", 10.5F);
            this.label8.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label8.Location = new System.Drawing.Point(6, 34);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(91, 14);
            this.label8.TabIndex = 6;
            this.label8.Text = "副机服务端IP";
            // 
            // MainSlaveMachineForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkSeaGreen;
            this.ClientSize = new System.Drawing.Size(600, 281);
            this.Controls.Add(this.groupBox_Remoting);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "MainSlaveMachineForm";
            this.Text = "MainSlaveMachineForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainSlaveMachineForm_FormClosing);
            this.Load += new System.EventHandler(this.MainSlaveMachineForm_Load);
            this.groupBox_Remoting.ResumeLayout(false);
            this.groupBox_Remoting.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox_Remoting;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tbx_Client_IP;
        private System.Windows.Forms.TextBox tbx_Server_IP;
        private System.Windows.Forms.CheckBox cbx_IsConnect;
    }
}
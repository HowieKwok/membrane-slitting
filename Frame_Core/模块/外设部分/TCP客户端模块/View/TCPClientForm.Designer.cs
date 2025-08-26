namespace Frame_Core
{
    partial class TCPClientForm
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.num_DevPort = new System.Windows.Forms.NumericUpDown();
            this.label58 = new System.Windows.Forms.Label();
            this.TCPIP = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tbx_Dev_IP = new System.Windows.Forms.TextBox();
            this.tbx_Local_IP = new System.Windows.Forms.TextBox();
            this.btn_connectPLC = new System.Windows.Forms.Button();
            this.cbx_IsConnect = new System.Windows.Forms.CheckBox();
            this.cbx_ComType = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.btn_Save = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_DevPort)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.num_DevPort);
            this.groupBox1.Controls.Add(this.label58);
            this.groupBox1.Controls.Add(this.TCPIP);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.tbx_Dev_IP);
            this.groupBox1.Controls.Add(this.tbx_Local_IP);
            this.groupBox1.Controls.Add(this.btn_connectPLC);
            this.groupBox1.Controls.Add(this.cbx_IsConnect);
            this.groupBox1.Controls.Add(this.cbx_ComType);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Location = new System.Drawing.Point(12, 22);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(830, 133);
            this.groupBox1.TabIndex = 115;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "连接设置";
            // 
            // num_DevPort
            // 
            this.num_DevPort.Location = new System.Drawing.Point(721, 78);
            this.num_DevPort.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.num_DevPort.Name = "num_DevPort";
            this.num_DevPort.Size = new System.Drawing.Size(87, 21);
            this.num_DevPort.TabIndex = 112;
            // 
            // label58
            // 
            this.label58.AutoSize = true;
            this.label58.Font = new System.Drawing.Font("宋体", 10.5F);
            this.label58.Location = new System.Drawing.Point(623, 82);
            this.label58.Name = "label58";
            this.label58.Size = new System.Drawing.Size(98, 14);
            this.label58.TabIndex = 111;
            this.label58.Text = "TCPIP端口号：";
            // 
            // TCPIP
            // 
            this.TCPIP.AutoSize = true;
            this.TCPIP.Font = new System.Drawing.Font("宋体", 10.5F);
            this.TCPIP.Location = new System.Drawing.Point(333, 82);
            this.TCPIP.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.TCPIP.Name = "TCPIP";
            this.TCPIP.Size = new System.Drawing.Size(56, 14);
            this.TCPIP.TabIndex = 106;
            this.TCPIP.Text = "TCPIP：";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("宋体", 10.5F);
            this.label5.Location = new System.Drawing.Point(45, 82);
            this.label5.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 14);
            this.label5.TabIndex = 110;
            this.label5.Text = "电脑端IP：";
            // 
            // tbx_Dev_IP
            // 
            this.tbx_Dev_IP.Location = new System.Drawing.Point(413, 78);
            this.tbx_Dev_IP.Margin = new System.Windows.Forms.Padding(5);
            this.tbx_Dev_IP.Name = "tbx_Dev_IP";
            this.tbx_Dev_IP.Size = new System.Drawing.Size(147, 21);
            this.tbx_Dev_IP.TabIndex = 107;
            this.tbx_Dev_IP.Text = "127.0.0.2";
            // 
            // tbx_Local_IP
            // 
            this.tbx_Local_IP.Location = new System.Drawing.Point(128, 78);
            this.tbx_Local_IP.Margin = new System.Windows.Forms.Padding(5);
            this.tbx_Local_IP.Name = "tbx_Local_IP";
            this.tbx_Local_IP.Size = new System.Drawing.Size(149, 21);
            this.tbx_Local_IP.TabIndex = 109;
            this.tbx_Local_IP.Text = "192.168.5.100";
            // 
            // btn_connectPLC
            // 
            this.btn_connectPLC.Location = new System.Drawing.Point(426, 22);
            this.btn_connectPLC.Margin = new System.Windows.Forms.Padding(5);
            this.btn_connectPLC.Name = "btn_connectPLC";
            this.btn_connectPLC.Size = new System.Drawing.Size(140, 39);
            this.btn_connectPLC.TabIndex = 108;
            this.btn_connectPLC.Text = "连接PLC";
            this.btn_connectPLC.UseVisualStyleBackColor = true;
            this.btn_connectPLC.Click += new System.EventHandler(this.btn_connectPLC_Click);
            // 
            // cbx_IsConnect
            // 
            this.cbx_IsConnect.AutoSize = true;
            this.cbx_IsConnect.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbx_IsConnect.Location = new System.Drawing.Point(336, 39);
            this.cbx_IsConnect.Name = "cbx_IsConnect";
            this.cbx_IsConnect.Size = new System.Drawing.Size(82, 18);
            this.cbx_IsConnect.TabIndex = 105;
            this.cbx_IsConnect.Text = "启用通讯";
            this.cbx_IsConnect.UseVisualStyleBackColor = true;
            this.cbx_IsConnect.Visible = false;
            // 
            // cbx_ComType
            // 
            this.cbx_ComType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbx_ComType.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbx_ComType.FormattingEnabled = true;
            this.cbx_ComType.Location = new System.Drawing.Point(128, 35);
            this.cbx_ComType.Name = "cbx_ComType";
            this.cbx_ComType.Size = new System.Drawing.Size(151, 22);
            this.cbx_ComType.TabIndex = 104;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("宋体", 10.5F);
            this.label9.Location = new System.Drawing.Point(45, 39);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(77, 14);
            this.label9.TabIndex = 103;
            this.label9.Text = "通讯选择：";
            // 
            // btn_Save
            // 
            this.btn_Save.Location = new System.Drawing.Point(878, 36);
            this.btn_Save.Margin = new System.Windows.Forms.Padding(5);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(140, 82);
            this.btn_Save.TabIndex = 116;
            this.btn_Save.Text = "保存参数";
            this.btn_Save.UseVisualStyleBackColor = true;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // TCPClientForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1045, 308);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btn_Save);
            this.Name = "TCPClientForm";
            this.Text = "TCPClientForm";
            this.Load += new System.EventHandler(this.TCPClientForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_DevPort)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label58;
        private System.Windows.Forms.Label TCPIP;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btn_Save;
        protected System.Windows.Forms.NumericUpDown num_DevPort;
        protected System.Windows.Forms.TextBox tbx_Dev_IP;
        protected System.Windows.Forms.TextBox tbx_Local_IP;
        protected System.Windows.Forms.Button btn_connectPLC;
        protected System.Windows.Forms.CheckBox cbx_IsConnect;
        protected System.Windows.Forms.ComboBox cbx_ComType;
    }
}
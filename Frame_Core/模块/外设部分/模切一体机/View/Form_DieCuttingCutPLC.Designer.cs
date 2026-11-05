namespace Frame_Core
{
    partial class Form_DieCuttingCutPLC
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
            this.btn_closePLC = new System.Windows.Forms.Button();
            this.cbx_ByteSelect = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.num_DevPort = new System.Windows.Forms.NumericUpDown();
            this.label58 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tbx_Dev_IP = new System.Windows.Forms.TextBox();
            this.tbx_Local_IP = new System.Windows.Forms.TextBox();
            this.btn_connectPLC = new System.Windows.Forms.Button();
            this.cbx_IsConnect = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbx_ComType = new System.Windows.Forms.ComboBox();
            this.btn_Save = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.num_DevPort)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_closePLC
            // 
            this.btn_closePLC.Location = new System.Drawing.Point(339, 22);
            this.btn_closePLC.Margin = new System.Windows.Forms.Padding(5);
            this.btn_closePLC.Name = "btn_closePLC";
            this.btn_closePLC.Size = new System.Drawing.Size(140, 39);
            this.btn_closePLC.TabIndex = 115;
            this.btn_closePLC.Text = "断开PLC";
            this.btn_closePLC.UseVisualStyleBackColor = true;
            this.btn_closePLC.Click += new System.EventHandler(this.btn_closePLC_Click);
            // 
            // cbx_ByteSelect
            // 
            this.cbx_ByteSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbx_ByteSelect.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbx_ByteSelect.FormattingEnabled = true;
            this.cbx_ByteSelect.Location = new System.Drawing.Point(171, 117);
            this.cbx_ByteSelect.Name = "cbx_ByteSelect";
            this.cbx_ByteSelect.Size = new System.Drawing.Size(151, 22);
            this.cbx_ByteSelect.TabIndex = 114;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 10.5F);
            this.label4.Location = new System.Drawing.Point(36, 122);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 14);
            this.label4.TabIndex = 113;
            this.label4.Text = "字节选择：";
            // 
            // num_DevPort
            // 
            this.num_DevPort.Location = new System.Drawing.Point(171, 240);
            this.num_DevPort.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.num_DevPort.Name = "num_DevPort";
            this.num_DevPort.Size = new System.Drawing.Size(151, 21);
            this.num_DevPort.TabIndex = 112;
            // 
            // label58
            // 
            this.label58.AutoSize = true;
            this.label58.Font = new System.Drawing.Font("宋体", 10.5F);
            this.label58.Location = new System.Drawing.Point(36, 243);
            this.label58.Name = "label58";
            this.label58.Size = new System.Drawing.Size(91, 14);
            this.label58.TabIndex = 111;
            this.label58.Text = "设备端口号：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 10.5F);
            this.label1.Location = new System.Drawing.Point(36, 199);
            this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 14);
            this.label1.TabIndex = 106;
            this.label1.Text = "设备IP：";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("宋体", 10.5F);
            this.label5.Location = new System.Drawing.Point(36, 159);
            this.label5.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 14);
            this.label5.TabIndex = 110;
            this.label5.Text = "电脑端IP：";
            // 
            // tbx_Dev_IP
            // 
            this.tbx_Dev_IP.Location = new System.Drawing.Point(171, 196);
            this.tbx_Dev_IP.Margin = new System.Windows.Forms.Padding(5);
            this.tbx_Dev_IP.Name = "tbx_Dev_IP";
            this.tbx_Dev_IP.Size = new System.Drawing.Size(151, 21);
            this.tbx_Dev_IP.TabIndex = 107;
            this.tbx_Dev_IP.Text = "127.0.0.2";
            // 
            // tbx_Local_IP
            // 
            this.tbx_Local_IP.Location = new System.Drawing.Point(171, 155);
            this.tbx_Local_IP.Margin = new System.Windows.Forms.Padding(5);
            this.tbx_Local_IP.Name = "tbx_Local_IP";
            this.tbx_Local_IP.Size = new System.Drawing.Size(151, 21);
            this.tbx_Local_IP.TabIndex = 109;
            this.tbx_Local_IP.Text = "192.168.5.100";
            // 
            // btn_connectPLC
            // 
            this.btn_connectPLC.Location = new System.Drawing.Point(171, 22);
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
            this.cbx_IsConnect.Location = new System.Drawing.Point(39, 35);
            this.cbx_IsConnect.Name = "cbx_IsConnect";
            this.cbx_IsConnect.Size = new System.Drawing.Size(82, 18);
            this.cbx_IsConnect.TabIndex = 105;
            this.cbx_IsConnect.Text = "启用通讯";
            this.cbx_IsConnect.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("宋体", 10.5F);
            this.label9.Location = new System.Drawing.Point(36, 84);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(77, 14);
            this.label9.TabIndex = 103;
            this.label9.Text = "通讯选择：";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btn_closePLC);
            this.groupBox1.Controls.Add(this.cbx_ByteSelect);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.num_DevPort);
            this.groupBox1.Controls.Add(this.label58);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.tbx_Dev_IP);
            this.groupBox1.Controls.Add(this.tbx_Local_IP);
            this.groupBox1.Controls.Add(this.btn_connectPLC);
            this.groupBox1.Controls.Add(this.cbx_IsConnect);
            this.groupBox1.Controls.Add(this.cbx_ComType);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Location = new System.Drawing.Point(12, 27);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(574, 308);
            this.groupBox1.TabIndex = 121;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "连接设置";
            // 
            // cbx_ComType
            // 
            this.cbx_ComType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbx_ComType.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbx_ComType.FormattingEnabled = true;
            this.cbx_ComType.Location = new System.Drawing.Point(171, 78);
            this.cbx_ComType.Name = "cbx_ComType";
            this.cbx_ComType.Size = new System.Drawing.Size(151, 22);
            this.cbx_ComType.TabIndex = 104;
            // 
            // btn_Save
            // 
            this.btn_Save.Location = new System.Drawing.Point(476, 368);
            this.btn_Save.Margin = new System.Windows.Forms.Padding(5);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(110, 35);
            this.btn_Save.TabIndex = 122;
            this.btn_Save.Text = "保存参数";
            this.btn_Save.UseVisualStyleBackColor = true;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // Form_DieCuttingCutPLC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(610, 419);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btn_Save);
            this.Name = "Form_DieCuttingCutPLC";
            this.Text = "Form_DieCuttingCutPLC";
            this.Load += new System.EventHandler(this.Form_DieCuttingCutPLC_Load);
            ((System.ComponentModel.ISupportInitialize)(this.num_DevPort)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_closePLC;
        private System.Windows.Forms.ComboBox cbx_ByteSelect;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown num_DevPort;
        private System.Windows.Forms.Label label58;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbx_Dev_IP;
        private System.Windows.Forms.TextBox tbx_Local_IP;
        private System.Windows.Forms.Button btn_connectPLC;
        private System.Windows.Forms.CheckBox cbx_IsConnect;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cbx_ComType;
        private System.Windows.Forms.Button btn_Save;
    }
}
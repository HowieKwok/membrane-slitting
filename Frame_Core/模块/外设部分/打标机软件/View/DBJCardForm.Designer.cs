namespace Frame_Core
{
    partial class DBJCardForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tbx_Dev_IP = new System.Windows.Forms.TextBox();
            this.tbx_Local_IP = new System.Windows.Forms.TextBox();
            this.btn_connectPLC = new System.Windows.Forms.Button();
            this.cbx_IsConnect = new System.Windows.Forms.CheckBox();
            this.cbx_ComType = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.btn_Save = new System.Windows.Forms.Button();
            this.cbx_AutoDB = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_DevPort)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
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
            this.groupBox1.Location = new System.Drawing.Point(16, 18);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(1107, 166);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "连接设置";
            // 
            // num_DevPort
            // 
            this.num_DevPort.Location = new System.Drawing.Point(961, 98);
            this.num_DevPort.Margin = new System.Windows.Forms.Padding(4);
            this.num_DevPort.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.num_DevPort.Name = "num_DevPort";
            this.num_DevPort.Size = new System.Drawing.Size(116, 25);
            this.num_DevPort.TabIndex = 112;
            // 
            // label58
            // 
            this.label58.AutoSize = true;
            this.label58.Font = new System.Drawing.Font("宋体", 10.5F);
            this.label58.Location = new System.Drawing.Point(831, 102);
            this.label58.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label58.Name = "label58";
            this.label58.Size = new System.Drawing.Size(134, 18);
            this.label58.TabIndex = 111;
            this.label58.Text = "打标机端口号：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 10.5F);
            this.label1.Location = new System.Drawing.Point(444, 102);
            this.label1.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(98, 18);
            this.label1.TabIndex = 106;
            this.label1.Text = "打标机IP：";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("宋体", 10.5F);
            this.label5.Location = new System.Drawing.Point(60, 102);
            this.label5.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(98, 18);
            this.label5.TabIndex = 110;
            this.label5.Text = "电脑端IP：";
            // 
            // tbx_Dev_IP
            // 
            this.tbx_Dev_IP.Location = new System.Drawing.Point(551, 98);
            this.tbx_Dev_IP.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.tbx_Dev_IP.Name = "tbx_Dev_IP";
            this.tbx_Dev_IP.Size = new System.Drawing.Size(195, 25);
            this.tbx_Dev_IP.TabIndex = 107;
            this.tbx_Dev_IP.Text = "127.0.0.2";
            // 
            // tbx_Local_IP
            // 
            this.tbx_Local_IP.Location = new System.Drawing.Point(171, 98);
            this.tbx_Local_IP.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.tbx_Local_IP.Name = "tbx_Local_IP";
            this.tbx_Local_IP.Size = new System.Drawing.Size(197, 25);
            this.tbx_Local_IP.TabIndex = 109;
            this.tbx_Local_IP.Text = "192.168.5.100";
            // 
            // btn_connectPLC
            // 
            this.btn_connectPLC.Location = new System.Drawing.Point(568, 28);
            this.btn_connectPLC.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.btn_connectPLC.Name = "btn_connectPLC";
            this.btn_connectPLC.Size = new System.Drawing.Size(187, 49);
            this.btn_connectPLC.TabIndex = 108;
            this.btn_connectPLC.Text = "连接PLC";
            this.btn_connectPLC.UseVisualStyleBackColor = true;
            this.btn_connectPLC.Click += new System.EventHandler(this.btn_connectPLC_Click);
            // 
            // cbx_IsConnect
            // 
            this.cbx_IsConnect.AutoSize = true;
            this.cbx_IsConnect.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbx_IsConnect.Location = new System.Drawing.Point(448, 49);
            this.cbx_IsConnect.Margin = new System.Windows.Forms.Padding(4);
            this.cbx_IsConnect.Name = "cbx_IsConnect";
            this.cbx_IsConnect.Size = new System.Drawing.Size(102, 22);
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
            this.cbx_ComType.Location = new System.Drawing.Point(171, 44);
            this.cbx_ComType.Margin = new System.Windows.Forms.Padding(4);
            this.cbx_ComType.Name = "cbx_ComType";
            this.cbx_ComType.Size = new System.Drawing.Size(200, 25);
            this.cbx_ComType.TabIndex = 104;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("宋体", 10.5F);
            this.label9.Location = new System.Drawing.Point(60, 49);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(98, 18);
            this.label9.TabIndex = 103;
            this.label9.Text = "通讯选择：";
            // 
            // btn_Save
            // 
            this.btn_Save.Location = new System.Drawing.Point(1171, 35);
            this.btn_Save.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(187, 102);
            this.btn_Save.TabIndex = 113;
            this.btn_Save.Text = "保存参数";
            this.btn_Save.UseVisualStyleBackColor = true;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // cbx_AutoDB
            // 
            this.cbx_AutoDB.AutoSize = true;
            this.cbx_AutoDB.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbx_AutoDB.Location = new System.Drawing.Point(56, 49);
            this.cbx_AutoDB.Margin = new System.Windows.Forms.Padding(4);
            this.cbx_AutoDB.Name = "cbx_AutoDB";
            this.cbx_AutoDB.Size = new System.Drawing.Size(102, 22);
            this.cbx_AutoDB.TabIndex = 114;
            this.cbx_AutoDB.Text = "自动打标";
            this.cbx_AutoDB.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cbx_AutoDB);
            this.groupBox2.Location = new System.Drawing.Point(16, 220);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(1107, 119);
            this.groupBox2.TabIndex = 114;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "打标参数";
            // 
            // DBJCardForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkSeaGreen;
            this.ClientSize = new System.Drawing.Size(1393, 385);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btn_Save);
            this.Controls.Add(this.groupBox1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "DBJCardForm";
            this.Text = "打标机";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MDevPLCForm_FormClosing);
            this.Load += new System.EventHandler(this.MDevPLCForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_DevPort)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown num_DevPort;
        private System.Windows.Forms.Label label58;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbx_Dev_IP;
        private System.Windows.Forms.TextBox tbx_Local_IP;
        private System.Windows.Forms.Button btn_connectPLC;
        private System.Windows.Forms.CheckBox cbx_IsConnect;
        private System.Windows.Forms.ComboBox cbx_ComType;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btn_Save;
        private System.Windows.Forms.CheckBox cbx_AutoDB;
        private System.Windows.Forms.GroupBox groupBox2;
    }
}
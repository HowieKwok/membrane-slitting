namespace Frame_Core
{
    partial class FormPointInspection
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
            this.components = new System.ComponentModel.Container();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.btn_StartSizePointInspection = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.ucCameraCard2 = new Frame_Core.UCCameraCard();
            this.ucCameraCard1 = new Frame_Core.UCCameraCard();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.label40 = new System.Windows.Forms.Label();
            this.label42 = new System.Windows.Forms.Label();
            this.dtp_dtCaliTimeBaiban = new System.Windows.Forms.DateTimePicker();
            this.dtp_dtCaliTimeYeBan = new System.Windows.Forms.DateTimePicker();
            this.cbx_djEnabled = new System.Windows.Forms.CheckBox();
            this.btn_SaveParam = new System.Windows.Forms.Button();
            this.btn_StartDefectPointInspection = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Font = new System.Drawing.Font("宋体", 12F);
            this.linkLabel1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.linkLabel1.Location = new System.Drawing.Point(933, 307);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(71, 16);
            this.linkLabel1.TabIndex = 121;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "点检数据";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // btn_StartSizePointInspection
            // 
            this.btn_StartSizePointInspection.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_StartSizePointInspection.Location = new System.Drawing.Point(1030, 239);
            this.btn_StartSizePointInspection.Margin = new System.Windows.Forms.Padding(2);
            this.btn_StartSizePointInspection.Name = "btn_StartSizePointInspection";
            this.btn_StartSizePointInspection.Size = new System.Drawing.Size(105, 36);
            this.btn_StartSizePointInspection.TabIndex = 122;
            this.btn_StartSizePointInspection.Text = "一键尺寸点检";
            this.btn_StartSizePointInspection.UseVisualStyleBackColor = true;
            this.btn_StartSizePointInspection.Click += new System.EventHandler(this.btn_StartSizePointInspection_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // ucCameraCard2
            // 
            this.ucCameraCard2.Location = new System.Drawing.Point(612, 22);
            this.ucCameraCard2.Name = "ucCameraCard2";
            this.ucCameraCard2.Size = new System.Drawing.Size(523, 175);
            this.ucCameraCard2.TabIndex = 1;
            // 
            // ucCameraCard1
            // 
            this.ucCameraCard1.Location = new System.Drawing.Point(42, 22);
            this.ucCameraCard1.Name = "ucCameraCard1";
            this.ucCameraCard1.Size = new System.Drawing.Size(523, 175);
            this.ucCameraCard1.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label40);
            this.panel1.Controls.Add(this.label42);
            this.panel1.Controls.Add(this.dtp_dtCaliTimeBaiban);
            this.panel1.Controls.Add(this.dtp_dtCaliTimeYeBan);
            this.panel1.Location = new System.Drawing.Point(42, 295);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(479, 36);
            this.panel1.TabIndex = 230;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 9F);
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(131, 12);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 231;
            this.label3.Text = "早班";
            // 
            // label40
            // 
            this.label40.AutoSize = true;
            this.label40.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label40.Location = new System.Drawing.Point(317, 12);
            this.label40.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(29, 12);
            this.label40.TabIndex = 224;
            this.label40.Text = "晚班";
            // 
            // label42
            // 
            this.label42.AutoSize = true;
            this.label42.Font = new System.Drawing.Font("黑体", 10.5F);
            this.label42.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label42.Location = new System.Drawing.Point(3, 11);
            this.label42.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(105, 14);
            this.label42.TabIndex = 223;
            this.label42.Text = "点检时间设置：";
            // 
            // dtp_dtCaliTimeBaiban
            // 
            this.dtp_dtCaliTimeBaiban.CustomFormat = "HH:mm";
            this.dtp_dtCaliTimeBaiban.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtp_dtCaliTimeBaiban.Location = new System.Drawing.Point(164, 7);
            this.dtp_dtCaliTimeBaiban.Name = "dtp_dtCaliTimeBaiban";
            this.dtp_dtCaliTimeBaiban.ShowUpDown = true;
            this.dtp_dtCaliTimeBaiban.Size = new System.Drawing.Size(121, 21);
            this.dtp_dtCaliTimeBaiban.TabIndex = 221;
            // 
            // dtp_dtCaliTimeYeBan
            // 
            this.dtp_dtCaliTimeYeBan.CustomFormat = "HH:mm";
            this.dtp_dtCaliTimeYeBan.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtp_dtCaliTimeYeBan.Location = new System.Drawing.Point(351, 7);
            this.dtp_dtCaliTimeYeBan.Name = "dtp_dtCaliTimeYeBan";
            this.dtp_dtCaliTimeYeBan.ShowUpDown = true;
            this.dtp_dtCaliTimeYeBan.Size = new System.Drawing.Size(121, 21);
            this.dtp_dtCaliTimeYeBan.TabIndex = 222;
            // 
            // cbx_djEnabled
            // 
            this.cbx_djEnabled.AutoSize = true;
            this.cbx_djEnabled.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cbx_djEnabled.Location = new System.Drawing.Point(545, 307);
            this.cbx_djEnabled.Name = "cbx_djEnabled";
            this.cbx_djEnabled.Size = new System.Drawing.Size(48, 16);
            this.cbx_djEnabled.TabIndex = 229;
            this.cbx_djEnabled.Text = "启用";
            this.cbx_djEnabled.UseVisualStyleBackColor = true;
            // 
            // btn_SaveParam
            // 
            this.btn_SaveParam.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_SaveParam.Location = new System.Drawing.Point(1030, 295);
            this.btn_SaveParam.Margin = new System.Windows.Forms.Padding(2);
            this.btn_SaveParam.Name = "btn_SaveParam";
            this.btn_SaveParam.Size = new System.Drawing.Size(105, 36);
            this.btn_SaveParam.TabIndex = 231;
            this.btn_SaveParam.Text = "保存参数";
            this.btn_SaveParam.UseVisualStyleBackColor = true;
            this.btn_SaveParam.Click += new System.EventHandler(this.btn_SaveParam_Click);
            // 
            // btn_StartDefectPointInspection
            // 
            this.btn_StartDefectPointInspection.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_StartDefectPointInspection.Location = new System.Drawing.Point(899, 239);
            this.btn_StartDefectPointInspection.Margin = new System.Windows.Forms.Padding(2);
            this.btn_StartDefectPointInspection.Name = "btn_StartDefectPointInspection";
            this.btn_StartDefectPointInspection.Size = new System.Drawing.Size(105, 36);
            this.btn_StartDefectPointInspection.TabIndex = 232;
            this.btn_StartDefectPointInspection.Text = "一键缺陷点检";
            this.btn_StartDefectPointInspection.UseVisualStyleBackColor = true;
            this.btn_StartDefectPointInspection.Visible = false;
            this.btn_StartDefectPointInspection.Click += new System.EventHandler(this.btn_StartDefectPointInspection_Click);
            // 
            // FormPointInspection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1160, 353);
            this.Controls.Add(this.btn_StartDefectPointInspection);
            this.Controls.Add(this.btn_SaveParam);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.cbx_djEnabled);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.btn_StartSizePointInspection);
            this.Controls.Add(this.ucCameraCard2);
            this.Controls.Add(this.ucCameraCard1);
            this.Name = "FormPointInspection";
            this.Text = "FormPointInspection";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormPointInspection_FormClosing);
            this.Load += new System.EventHandler(this.FormPointInspection_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private UCCameraCard ucCameraCard1;
        private UCCameraCard ucCameraCard2;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Button btn_StartSizePointInspection;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label40;
        private System.Windows.Forms.Label label42;
        private System.Windows.Forms.DateTimePicker dtp_dtCaliTimeBaiban;
        private System.Windows.Forms.DateTimePicker dtp_dtCaliTimeYeBan;
        private System.Windows.Forms.CheckBox cbx_djEnabled;
        private System.Windows.Forms.Button btn_SaveParam;
        private System.Windows.Forms.Button btn_StartDefectPointInspection;
    }
}
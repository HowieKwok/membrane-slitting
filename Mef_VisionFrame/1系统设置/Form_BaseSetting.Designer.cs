namespace Mef_VisionFrame
{
    partial class Form_BaseSetting
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_BaseSetting));
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lb_LastDJTime = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.label40 = new System.Windows.Forms.Label();
            this.label42 = new System.Windows.Forms.Label();
            this.dtp_dtCaliTimeBaiban = new System.Windows.Forms.DateTimePicker();
            this.dtp_dtCaliTimeYeBan = new System.Windows.Forms.DateTimePicker();
            this.cbx_djEnabled = new System.Windows.Forms.CheckBox();
            this.btn_SaveParam = new System.Windows.Forms.Button();
            this.groupBox2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lb_LastDJTime);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.panel1);
            this.groupBox2.Controls.Add(this.cbx_djEnabled);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // lb_LastDJTime
            // 
            resources.ApplyResources(this.lb_LastDJTime, "lb_LastDJTime");
            this.lb_LastDJTime.Name = "lb_LastDJTime";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label40);
            this.panel1.Controls.Add(this.label42);
            this.panel1.Controls.Add(this.dtp_dtCaliTimeBaiban);
            this.panel1.Controls.Add(this.dtp_dtCaliTimeYeBan);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label40
            // 
            resources.ApplyResources(this.label40, "label40");
            this.label40.Name = "label40";
            // 
            // label42
            // 
            resources.ApplyResources(this.label42, "label42");
            this.label42.Name = "label42";
            // 
            // dtp_dtCaliTimeBaiban
            // 
            resources.ApplyResources(this.dtp_dtCaliTimeBaiban, "dtp_dtCaliTimeBaiban");
            this.dtp_dtCaliTimeBaiban.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtp_dtCaliTimeBaiban.Name = "dtp_dtCaliTimeBaiban";
            this.dtp_dtCaliTimeBaiban.ShowUpDown = true;
            // 
            // dtp_dtCaliTimeYeBan
            // 
            resources.ApplyResources(this.dtp_dtCaliTimeYeBan, "dtp_dtCaliTimeYeBan");
            this.dtp_dtCaliTimeYeBan.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtp_dtCaliTimeYeBan.Name = "dtp_dtCaliTimeYeBan";
            this.dtp_dtCaliTimeYeBan.ShowUpDown = true;
            // 
            // cbx_djEnabled
            // 
            resources.ApplyResources(this.cbx_djEnabled, "cbx_djEnabled");
            this.cbx_djEnabled.Name = "cbx_djEnabled";
            this.cbx_djEnabled.UseVisualStyleBackColor = true;
            // 
            // btn_SaveParam
            // 
            resources.ApplyResources(this.btn_SaveParam, "btn_SaveParam");
            this.btn_SaveParam.Name = "btn_SaveParam";
            this.btn_SaveParam.UseVisualStyleBackColor = true;
            this.btn_SaveParam.Click += new System.EventHandler(this.btn_SaveParam_Click);
            // 
            // Form_BaseSetting
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btn_SaveParam);
            this.Controls.Add(this.groupBox2);
            this.Name = "Form_BaseSetting";
            this.Load += new System.EventHandler(this.Form_BaseSetting_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox cbx_djEnabled;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label40;
        private System.Windows.Forms.Label label42;
        private System.Windows.Forms.DateTimePicker dtp_dtCaliTimeBaiban;
        private System.Windows.Forms.DateTimePicker dtp_dtCaliTimeYeBan;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lb_LastDJTime;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btn_SaveParam;
    }
}
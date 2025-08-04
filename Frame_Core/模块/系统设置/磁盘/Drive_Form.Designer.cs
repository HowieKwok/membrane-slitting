namespace Frame_Core
{
    partial class Drive_Form
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
            this.label1 = new System.Windows.Forms.Label();
            this.DriveCapacityLimit = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.IsAlarm = new System.Windows.Forms.CheckBox();
            this.diverRichText = new System.Windows.Forms.RichTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.comDiverName = new System.Windows.Forms.ComboBox();
            this.Btn_Divercfg = new System.Windows.Forms.Button();
            this.Btn_Save = new System.Windows.Forms.Button();
            this.richTextAlarm = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.DriveCapacityLimit)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 332);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "磁盘容量上限值：";
            // 
            // DriveCapacityLimit
            // 
            this.DriveCapacityLimit.Location = new System.Drawing.Point(130, 327);
            this.DriveCapacityLimit.Maximum = new decimal(new int[] {
            95,
            0,
            0,
            0});
            this.DriveCapacityLimit.Name = "DriveCapacityLimit";
            this.DriveCapacityLimit.Size = new System.Drawing.Size(87, 21);
            this.DriveCapacityLimit.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(227, 336);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(11, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "%";
            // 
            // IsAlarm
            // 
            this.IsAlarm.AutoSize = true;
            this.IsAlarm.Location = new System.Drawing.Point(25, 364);
            this.IsAlarm.Name = "IsAlarm";
            this.IsAlarm.Size = new System.Drawing.Size(72, 16);
            this.IsAlarm.TabIndex = 4;
            this.IsAlarm.Text = "是否报警";
            this.IsAlarm.UseVisualStyleBackColor = true;
            // 
            // diverRichText
            // 
            this.diverRichText.BackColor = System.Drawing.SystemColors.Menu;
            this.diverRichText.Location = new System.Drawing.Point(25, 122);
            this.diverRichText.Name = "diverRichText";
            this.diverRichText.Size = new System.Drawing.Size(432, 147);
            this.diverRichText.TabIndex = 5;
            this.diverRichText.Text = "";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(23, 88);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "磁盘信息";
            // 
            // comDiverName
            // 
            this.comDiverName.FormattingEnabled = true;
            this.comDiverName.Location = new System.Drawing.Point(25, 292);
            this.comDiverName.Name = "comDiverName";
            this.comDiverName.Size = new System.Drawing.Size(121, 20);
            this.comDiverName.TabIndex = 7;
            this.comDiverName.SelectedIndexChanged += new System.EventHandler(this.comDiverName_SelectedIndexChanged);
            // 
            // Btn_Divercfg
            // 
            this.Btn_Divercfg.Location = new System.Drawing.Point(229, 405);
            this.Btn_Divercfg.Name = "Btn_Divercfg";
            this.Btn_Divercfg.Size = new System.Drawing.Size(109, 23);
            this.Btn_Divercfg.TabIndex = 9;
            this.Btn_Divercfg.Text = "磁盘更换确认";
            this.Btn_Divercfg.UseVisualStyleBackColor = true;
            this.Btn_Divercfg.Click += new System.EventHandler(this.Btn_Divercfg_Click);
            // 
            // Btn_Save
            // 
            this.Btn_Save.Location = new System.Drawing.Point(390, 405);
            this.Btn_Save.Name = "Btn_Save";
            this.Btn_Save.Size = new System.Drawing.Size(75, 23);
            this.Btn_Save.TabIndex = 10;
            this.Btn_Save.Text = "保存参数";
            this.Btn_Save.UseVisualStyleBackColor = true;
            this.Btn_Save.Click += new System.EventHandler(this.Btn_Save_Click);
            // 
            // richTextAlarm
            // 
            this.richTextAlarm.Font = new System.Drawing.Font("思源黑体 CN Regular", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.richTextAlarm.Location = new System.Drawing.Point(25, 12);
            this.richTextAlarm.Name = "richTextAlarm";
            this.richTextAlarm.Size = new System.Drawing.Size(440, 47);
            this.richTextAlarm.TabIndex = 11;
            this.richTextAlarm.Text = "";
            // 
            // Drive_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(478, 457);
            this.Controls.Add(this.richTextAlarm);
            this.Controls.Add(this.Btn_Save);
            this.Controls.Add(this.Btn_Divercfg);
            this.Controls.Add(this.comDiverName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.diverRichText);
            this.Controls.Add(this.IsAlarm);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.DriveCapacityLimit);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(494, 496);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(494, 496);
            this.Name = "Drive_Form";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Drive_Form";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Drive_Form_FormClosed);
            this.Load += new System.EventHandler(this.Drive_Form_Load);
            ((System.ComponentModel.ISupportInitialize)(this.DriveCapacityLimit)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown DriveCapacityLimit;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox IsAlarm;
        private System.Windows.Forms.RichTextBox diverRichText;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comDiverName;
        private System.Windows.Forms.Button Btn_Divercfg;
        private System.Windows.Forms.Button Btn_Save;
        private System.Windows.Forms.RichTextBox richTextAlarm;
    }
}
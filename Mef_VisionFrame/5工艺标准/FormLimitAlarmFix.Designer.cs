namespace Mef_VisionFrame._5工艺标准
{
    partial class FormLimitAlarmFix
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.TextBox_Desp = new System.Windows.Forms.TextBox();
            this.linkLabel_SizeItem = new System.Windows.Forms.LinkLabel();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dataGridView1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox1.Location = new System.Drawing.Point(0, 117);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(5);
            this.groupBox1.Size = new System.Drawing.Size(864, 390);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "槽位标准工艺补偿系数";
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(5, 27);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(5);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(854, 358);
            this.dataGridView1.TabIndex = 88;
            // 
            // TextBox_Desp
            // 
            this.TextBox_Desp.Location = new System.Drawing.Point(262, 31);
            this.TextBox_Desp.Name = "TextBox_Desp";
            this.TextBox_Desp.Size = new System.Drawing.Size(183, 29);
            this.TextBox_Desp.TabIndex = 81;
            // 
            // linkLabel_SizeItem
            // 
            this.linkLabel_SizeItem.AutoSize = true;
            this.linkLabel_SizeItem.Location = new System.Drawing.Point(127, 32);
            this.linkLabel_SizeItem.Name = "linkLabel_SizeItem";
            this.linkLabel_SizeItem.Size = new System.Drawing.Size(58, 21);
            this.linkLabel_SizeItem.TabIndex = 80;
            this.linkLabel_SizeItem.TabStop = true;
            this.linkLabel_SizeItem.Text = "尺寸项";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label9.Location = new System.Drawing.Point(11, 32);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(58, 21);
            this.label9.TabIndex = 79;
            this.label9.Text = "项选择";
            // 
            // FormLimitAlarmFix
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(864, 507);
            this.Controls.Add(this.TextBox_Desp);
            this.Controls.Add(this.linkLabel_SizeItem);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "FormLimitAlarmFix";
            this.Text = "FormSlotFix";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormSlotFix_FormClosing);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TextBox TextBox_Desp;
        private System.Windows.Forms.LinkLabel linkLabel_SizeItem;
        private System.Windows.Forms.Label label9;
    }
}
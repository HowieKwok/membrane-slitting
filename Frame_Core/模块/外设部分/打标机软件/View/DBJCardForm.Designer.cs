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
            this.cbx_AutoDB = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.num_CamDBIndex = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.num_DevPort)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_CamDBIndex)).BeginInit();
            this.SuspendLayout();
            // 
            // cbx_AutoDB
            // 
            this.cbx_AutoDB.AutoSize = true;
            this.cbx_AutoDB.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbx_AutoDB.Location = new System.Drawing.Point(42, 39);
            this.cbx_AutoDB.Name = "cbx_AutoDB";
            this.cbx_AutoDB.Size = new System.Drawing.Size(82, 18);
            this.cbx_AutoDB.TabIndex = 114;
            this.cbx_AutoDB.Text = "自动打标";
            this.cbx_AutoDB.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.num_CamDBIndex);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.cbx_AutoDB);
            this.groupBox2.Location = new System.Drawing.Point(12, 176);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox2.Size = new System.Drawing.Size(830, 95);
            this.groupBox2.TabIndex = 114;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "打标参数";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(878, 126);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(140, 35);
            this.button1.TabIndex = 115;
            this.button1.Text = "打标通道管理";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(184, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 115;
            this.label1.Text = "编号选择：";
            // 
            // num_CamDBIndex
            // 
            this.num_CamDBIndex.Location = new System.Drawing.Point(256, 35);
            this.num_CamDBIndex.Maximum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.num_CamDBIndex.Name = "num_CamDBIndex";
            this.num_CamDBIndex.Size = new System.Drawing.Size(120, 21);
            this.num_CamDBIndex.TabIndex = 116;
            // 
            // DBJCardForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkSeaGreen;
            this.ClientSize = new System.Drawing.Size(1045, 308);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox2);
            this.MaximizeBox = false;
            this.Name = "DBJCardForm";
            this.Text = "打标机";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MDevPLCForm_FormClosing);
            this.Load += new System.EventHandler(this.TCPClientForm_Load);
            this.Controls.SetChildIndex(this.groupBox2, 0);
            this.Controls.SetChildIndex(this.button1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.num_DevPort)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_CamDBIndex)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.CheckBox cbx_AutoDB;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.NumericUpDown num_CamDBIndex;
        private System.Windows.Forms.Label label1;
    }
}
namespace Frame_Core
{
    partial class FormPluginBase
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
            this.Btn_Cancel = new AntdUI.Button();
            this.Btn_Save = new AntdUI.Button();
            this.Btn_RUN = new AntdUI.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.Tb_CostTime = new AntdUI.Tag();
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // Btn_Cancel
            // 
            this.Btn_Cancel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Btn_Cancel.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Btn_Cancel.Location = new System.Drawing.Point(906, 3);
            this.Btn_Cancel.Name = "Btn_Cancel";
            this.Btn_Cancel.Size = new System.Drawing.Size(99, 34);
            this.Btn_Cancel.TabIndex = 10;
            this.Btn_Cancel.Text = "取消";
            this.Btn_Cancel.Click += new System.EventHandler(this.Btn_Cancel_Click);
            // 
            // Btn_Save
            // 
            this.Btn_Save.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Btn_Save.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Btn_Save.Location = new System.Drawing.Point(806, 3);
            this.Btn_Save.Name = "Btn_Save";
            this.Btn_Save.Size = new System.Drawing.Size(94, 34);
            this.Btn_Save.TabIndex = 9;
            this.Btn_Save.Text = "保存";
            this.Btn_Save.Click += new System.EventHandler(this.Btn_Save_Click);
            // 
            // Btn_RUN
            // 
            this.Btn_RUN.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Btn_RUN.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Btn_RUN.Location = new System.Drawing.Point(706, 3);
            this.Btn_RUN.Name = "Btn_RUN";
            this.Btn_RUN.Size = new System.Drawing.Size(94, 34);
            this.Btn_RUN.TabIndex = 8;
            this.Btn_RUN.Text = "执行";
            this.Btn_RUN.Click += new System.EventHandler(this.Btn_RUN_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 8;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 104F));
            this.tableLayoutPanel1.Controls.Add(this.Btn_RUN, 5, 0);
            this.tableLayoutPanel1.Controls.Add(this.Btn_Save, 6, 0);
            this.tableLayoutPanel1.Controls.Add(this.Btn_Cancel, 7, 0);
            this.tableLayoutPanel1.Controls.Add(this.Tb_CostTime, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 771);
            this.tableLayoutPanel1.MaximumSize = new System.Drawing.Size(0, 40);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1008, 40);
            this.tableLayoutPanel1.TabIndex = 14;
            // 
            // Tb_CostTime
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.Tb_CostTime, 2);
            this.Tb_CostTime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Tb_CostTime.Location = new System.Drawing.Point(3, 3);
            this.Tb_CostTime.Name = "Tb_CostTime";
            this.Tb_CostTime.Size = new System.Drawing.Size(154, 34);
            this.Tb_CostTime.TabIndex = 15;
            this.Tb_CostTime.Text = "耗时：000ms";
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.Location = new System.Drawing.Point(463, 377);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.Size = new System.Drawing.Size(8, 4);
            this.checkedListBox1.TabIndex = 15;
            // 
            // FormPluginBase
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1008, 811);
            this.Controls.Add(this.checkedListBox1);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "FormPluginBase";
            this.Text = "PluginFormBase";
            this.Load += new System.EventHandler(this.FormPluginBase_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private AntdUI.Button Btn_RUN;
        private AntdUI.Button Btn_Save;
        private AntdUI.Button Btn_Cancel;
        private AntdUI.Tag Tb_CostTime;
        private System.Windows.Forms.CheckedListBox checkedListBox1;
    }
}
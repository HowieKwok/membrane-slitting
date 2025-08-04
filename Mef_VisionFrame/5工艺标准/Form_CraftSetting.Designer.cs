namespace Mef_VisionFrame
{
    partial class Form_CraftSetting
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.comb_materialCraf = new System.Windows.Forms.ComboBox();
            this.btn_SaveCurrentCraft = new System.Windows.Forms.Button();
            this.gB_Config = new System.Windows.Forms.GroupBox();
            this.tB_ConfigName = new System.Windows.Forms.TextBox();
            this.lV_ProjectList = new System.Windows.Forms.ListView();
            this.cMS_Config = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tSMI_CreateConfig = new System.Windows.Forms.ToolStripMenuItem();
            this.tSMI_CopyConfig = new System.Windows.Forms.ToolStripMenuItem();
            this.tSMI_RemoveConfig = new System.Windows.Forms.ToolStripMenuItem();
            this.tSMI_ReNameConfig = new System.Windows.Forms.ToolStripMenuItem();
            this.tSMI_OpenConfigFile = new System.Windows.Forms.ToolStripMenuItem();
            this.panel_Main = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1.SuspendLayout();
            this.gB_Config.SuspendLayout();
            this.cMS_Config.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 12;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.13059F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.13059F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.13059F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.14812F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10.93777F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.13059F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.13059F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.13059F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.13059F));
            this.tableLayoutPanel1.Controls.Add(this.comb_materialCraf, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btn_SaveCurrentCraft, 0, 11);
            this.tableLayoutPanel1.Controls.Add(this.gB_Config, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel_Main, 3, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 12;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.057583F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.546371F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.546371F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.546371F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.546371F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.546371F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.546371F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.546371F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.546371F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.546371F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.826073F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.199003F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1362, 825);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // comb_materialCraf
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.comb_materialCraf, 3);
            this.comb_materialCraf.Dock = System.Windows.Forms.DockStyle.Fill;
            this.comb_materialCraf.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comb_materialCraf.Font = new System.Drawing.Font("微软雅黑", 14.25F);
            this.comb_materialCraf.FormattingEnabled = true;
            this.comb_materialCraf.Location = new System.Drawing.Point(4, 4);
            this.comb_materialCraf.Margin = new System.Windows.Forms.Padding(4);
            this.comb_materialCraf.Name = "comb_materialCraf";
            this.comb_materialCraf.Size = new System.Drawing.Size(292, 33);
            this.comb_materialCraf.TabIndex = 261;
            this.comb_materialCraf.SelectedIndexChanged += new System.EventHandler(this.comb_materialCraf_SelectedIndexChanged);
            // 
            // btn_SaveCurrentCraft
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.btn_SaveCurrentCraft, 3);
            this.btn_SaveCurrentCraft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_SaveCurrentCraft.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_SaveCurrentCraft.Location = new System.Drawing.Point(3, 746);
            this.btn_SaveCurrentCraft.Name = "btn_SaveCurrentCraft";
            this.btn_SaveCurrentCraft.Size = new System.Drawing.Size(294, 76);
            this.btn_SaveCurrentCraft.TabIndex = 257;
            this.btn_SaveCurrentCraft.Text = "保存配方";
            this.btn_SaveCurrentCraft.UseVisualStyleBackColor = true;
            this.btn_SaveCurrentCraft.Click += new System.EventHandler(this.btn_SaveCurrentCraft_Click);
            // 
            // gB_Config
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.gB_Config, 3);
            this.gB_Config.Controls.Add(this.tB_ConfigName);
            this.gB_Config.Controls.Add(this.lV_ProjectList);
            this.gB_Config.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gB_Config.Location = new System.Drawing.Point(3, 44);
            this.gB_Config.Name = "gB_Config";
            this.tableLayoutPanel1.SetRowSpan(this.gB_Config, 10);
            this.gB_Config.Size = new System.Drawing.Size(294, 696);
            this.gB_Config.TabIndex = 0;
            this.gB_Config.TabStop = false;
            this.gB_Config.Text = "配方列表";
            // 
            // tB_ConfigName
            // 
            this.tB_ConfigName.BackColor = System.Drawing.Color.White;
            this.tB_ConfigName.Location = new System.Drawing.Point(9, 532);
            this.tB_ConfigName.Name = "tB_ConfigName";
            this.tB_ConfigName.Size = new System.Drawing.Size(113, 26);
            this.tB_ConfigName.TabIndex = 2;
            this.tB_ConfigName.Visible = false;
            this.tB_ConfigName.TextChanged += new System.EventHandler(this.tB_ConfigName_TextChanged);
            this.tB_ConfigName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tB_ConfigName_KeyDown);
            // 
            // lV_ProjectList
            // 
            this.lV_ProjectList.ContextMenuStrip = this.cMS_Config;
            this.lV_ProjectList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lV_ProjectList.GridLines = true;
            this.lV_ProjectList.HideSelection = false;
            this.lV_ProjectList.Location = new System.Drawing.Point(3, 22);
            this.lV_ProjectList.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lV_ProjectList.MultiSelect = false;
            this.lV_ProjectList.Name = "lV_ProjectList";
            this.lV_ProjectList.Size = new System.Drawing.Size(288, 671);
            this.lV_ProjectList.TabIndex = 1;
            this.lV_ProjectList.UseCompatibleStateImageBehavior = false;
            this.lV_ProjectList.View = System.Windows.Forms.View.List;
            this.lV_ProjectList.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lV_ProjectList_MouseClick);
            this.lV_ProjectList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lV_ProjectList_MouseDoubleClick);
            // 
            // cMS_Config
            // 
            this.cMS_Config.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.cMS_Config.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tSMI_CreateConfig,
            this.tSMI_CopyConfig,
            this.tSMI_RemoveConfig,
            this.tSMI_ReNameConfig,
            this.tSMI_OpenConfigFile});
            this.cMS_Config.Name = "cMS_Config";
            this.cMS_Config.Size = new System.Drawing.Size(173, 114);
            this.cMS_Config.Text = "配方";
            // 
            // tSMI_CreateConfig
            // 
            this.tSMI_CreateConfig.Name = "tSMI_CreateConfig";
            this.tSMI_CreateConfig.Size = new System.Drawing.Size(172, 22);
            this.tSMI_CreateConfig.Text = "新建配方";
            this.tSMI_CreateConfig.Click += new System.EventHandler(this.tSMI_CreateConfig_Click);
            // 
            // tSMI_CopyConfig
            // 
            this.tSMI_CopyConfig.Name = "tSMI_CopyConfig";
            this.tSMI_CopyConfig.Size = new System.Drawing.Size(172, 22);
            this.tSMI_CopyConfig.Text = "复制配方";
            this.tSMI_CopyConfig.Click += new System.EventHandler(this.tSMI_CopyConfig_Click);
            // 
            // tSMI_RemoveConfig
            // 
            this.tSMI_RemoveConfig.ForeColor = System.Drawing.Color.Red;
            this.tSMI_RemoveConfig.Name = "tSMI_RemoveConfig";
            this.tSMI_RemoveConfig.Size = new System.Drawing.Size(172, 22);
            this.tSMI_RemoveConfig.Text = "移除配方";
            this.tSMI_RemoveConfig.Click += new System.EventHandler(this.tSMI_RemoveConfig_Click);
            // 
            // tSMI_ReNameConfig
            // 
            this.tSMI_ReNameConfig.Name = "tSMI_ReNameConfig";
            this.tSMI_ReNameConfig.Size = new System.Drawing.Size(172, 22);
            this.tSMI_ReNameConfig.Text = "重命名配方";
            this.tSMI_ReNameConfig.Click += new System.EventHandler(this.tSMI_ReNameConfig_Click);
            // 
            // tSMI_OpenConfigFile
            // 
            this.tSMI_OpenConfigFile.ForeColor = System.Drawing.Color.Blue;
            this.tSMI_OpenConfigFile.Name = "tSMI_OpenConfigFile";
            this.tSMI_OpenConfigFile.Size = new System.Drawing.Size(172, 22);
            this.tSMI_OpenConfigFile.Text = "打开配方文件位置";
            this.tSMI_OpenConfigFile.Click += new System.EventHandler(this.tSMI_OpenConfigFile_Click);
            // 
            // panel_Main
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.panel_Main, 9);
            this.panel_Main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_Main.Location = new System.Drawing.Point(303, 3);
            this.panel_Main.Name = "panel_Main";
            this.tableLayoutPanel1.SetRowSpan(this.panel_Main, 12);
            this.panel_Main.Size = new System.Drawing.Size(1056, 819);
            this.panel_Main.TabIndex = 260;
            // 
            // Form_CraftSetting
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(1362, 825);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Name = "Form_CraftSetting";
            this.Text = "Form_ProductSetting";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.gB_Config.ResumeLayout(false);
            this.gB_Config.PerformLayout();
            this.cMS_Config.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox gB_Config;
        private System.Windows.Forms.ListView lV_ProjectList;
        private System.Windows.Forms.ContextMenuStrip cMS_Config;
        private System.Windows.Forms.ToolStripMenuItem tSMI_CreateConfig;
        private System.Windows.Forms.ToolStripMenuItem tSMI_CopyConfig;
        private System.Windows.Forms.ToolStripMenuItem tSMI_RemoveConfig;
        private System.Windows.Forms.ToolStripMenuItem tSMI_ReNameConfig;
        private System.Windows.Forms.Button btn_SaveCurrentCraft;
        private System.Windows.Forms.TextBox tB_ConfigName;
        private System.Windows.Forms.Panel panel_Main;
        private System.Windows.Forms.ComboBox comb_materialCraf;
        private System.Windows.Forms.ToolStripMenuItem tSMI_OpenConfigFile;
    }
}
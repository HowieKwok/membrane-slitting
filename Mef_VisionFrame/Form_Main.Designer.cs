namespace Mef_VisionFrame
{
    partial class Form_Main
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Main));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel_Main = new System.Windows.Forms.Panel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.用户设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.用户登录ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.账户管理ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.注销账户ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.系统设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.保存设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.磁盘设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.基础设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.图片管理ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.调试设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.工艺标准ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripMenuItem();
            this.点检校验ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.相机标定ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.尺寸点检ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.灰度自适应ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.数据分析ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.生产统计ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.数据查询ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.语言ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chineseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.englishToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.帮助ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timer_StatusBarUpdate = new System.Windows.Forms.Timer(this.components);
            this.timer_SimLoadImage = new System.Windows.Forms.Timer(this.components);
            this.timer_DJ = new System.Windows.Forms.Timer(this.components);
            this.timer_CraftBack = new System.Windows.Forms.Timer(this.components);
            this.uC_StateView1 = new Mef_VisionFrame.UC_StateView();
            this.tableLayoutPanel1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 5;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.Controls.Add(this.panel_Main, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.uC_StateView1, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.menuStrip1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1284, 661);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // panel_Main
            // 
            this.panel_Main.BackColor = System.Drawing.Color.White;
            this.tableLayoutPanel1.SetColumnSpan(this.panel_Main, 5);
            this.panel_Main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_Main.Location = new System.Drawing.Point(0, 30);
            this.panel_Main.Margin = new System.Windows.Forms.Padding(0);
            this.panel_Main.Name = "panel_Main";
            this.tableLayoutPanel1.SetRowSpan(this.panel_Main, 3);
            this.panel_Main.Size = new System.Drawing.Size(1284, 600);
            this.panel_Main.TabIndex = 2;
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.Control;
            this.tableLayoutPanel1.SetColumnSpan(this.menuStrip1, 5);
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.用户设置ToolStripMenuItem,
            this.系统设置ToolStripMenuItem,
            this.调试设置ToolStripMenuItem,
            this.工艺标准ToolStripMenuItem,
            this.toolStripMenuItem3,
            this.点检校验ToolStripMenuItem,
            this.数据分析ToolStripMenuItem,
            this.语言ToolStripMenuItem,
            this.帮助ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(1284, 30);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.menuStrip1_ItemClicked);
            // 
            // 用户设置ToolStripMenuItem
            // 
            this.用户设置ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.用户登录ToolStripMenuItem,
            this.账户管理ToolStripMenuItem,
            this.注销账户ToolStripMenuItem});
            this.用户设置ToolStripMenuItem.ForeColor = System.Drawing.Color.Black;
            this.用户设置ToolStripMenuItem.Name = "用户设置ToolStripMenuItem";
            this.用户设置ToolStripMenuItem.Size = new System.Drawing.Size(68, 26);
            this.用户设置ToolStripMenuItem.Text = "用户设置";
            // 
            // 用户登录ToolStripMenuItem
            // 
            this.用户登录ToolStripMenuItem.Name = "用户登录ToolStripMenuItem";
            this.用户登录ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.用户登录ToolStripMenuItem.Text = "用户登录";
            this.用户登录ToolStripMenuItem.Click += new System.EventHandler(this.用户登录ToolStripMenuItem_Click);
            // 
            // 账户管理ToolStripMenuItem
            // 
            this.账户管理ToolStripMenuItem.Name = "账户管理ToolStripMenuItem";
            this.账户管理ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.账户管理ToolStripMenuItem.Text = "账户管理";
            this.账户管理ToolStripMenuItem.Click += new System.EventHandler(this.账户管理ToolStripMenuItem_Click);
            // 
            // 注销账户ToolStripMenuItem
            // 
            this.注销账户ToolStripMenuItem.Name = "注销账户ToolStripMenuItem";
            this.注销账户ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.注销账户ToolStripMenuItem.Text = "注销账户";
            this.注销账户ToolStripMenuItem.Click += new System.EventHandler(this.注销账户ToolStripMenuItem_Click);
            // 
            // 系统设置ToolStripMenuItem
            // 
            this.系统设置ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.保存设置ToolStripMenuItem,
            this.磁盘设置ToolStripMenuItem,
            this.基础设置ToolStripMenuItem,
            this.图片管理ToolStripMenuItem});
            this.系统设置ToolStripMenuItem.ForeColor = System.Drawing.Color.Black;
            this.系统设置ToolStripMenuItem.Name = "系统设置ToolStripMenuItem";
            this.系统设置ToolStripMenuItem.Size = new System.Drawing.Size(68, 26);
            this.系统设置ToolStripMenuItem.Text = "系统设置";
            this.系统设置ToolStripMenuItem.Click += new System.EventHandler(this.系统设置ToolStripMenuItem_Click);
            // 
            // 保存设置ToolStripMenuItem
            // 
            this.保存设置ToolStripMenuItem.Name = "保存设置ToolStripMenuItem";
            this.保存设置ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.保存设置ToolStripMenuItem.Text = "保存设置";
            this.保存设置ToolStripMenuItem.Click += new System.EventHandler(this.保存设置ToolStripMenuItem_Click);
            // 
            // 磁盘设置ToolStripMenuItem
            // 
            this.磁盘设置ToolStripMenuItem.Name = "磁盘设置ToolStripMenuItem";
            this.磁盘设置ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.磁盘设置ToolStripMenuItem.Text = "磁盘设置";
            this.磁盘设置ToolStripMenuItem.Click += new System.EventHandler(this.磁盘设置ToolStripMenuItem_Click);
            // 
            // 基础设置ToolStripMenuItem
            // 
            this.基础设置ToolStripMenuItem.Name = "基础设置ToolStripMenuItem";
            this.基础设置ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.基础设置ToolStripMenuItem.Text = "基础设置";
            this.基础设置ToolStripMenuItem.Click += new System.EventHandler(this.基础设置ToolStripMenuItem_Click);
            // 
            // 图片管理ToolStripMenuItem
            // 
            this.图片管理ToolStripMenuItem.Name = "图片管理ToolStripMenuItem";
            this.图片管理ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.图片管理ToolStripMenuItem.Text = "图片管理";
            this.图片管理ToolStripMenuItem.Click += new System.EventHandler(this.图片管理ToolStripMenuItem_Click);
            // 
            // 调试设置ToolStripMenuItem
            // 
            this.调试设置ToolStripMenuItem.ForeColor = System.Drawing.Color.Black;
            this.调试设置ToolStripMenuItem.Name = "调试设置ToolStripMenuItem";
            this.调试设置ToolStripMenuItem.Size = new System.Drawing.Size(68, 26);
            this.调试设置ToolStripMenuItem.Text = "调试设置";
            this.调试设置ToolStripMenuItem.Click += new System.EventHandler(this.调试设置ToolStripMenuItem_Click);
            // 
            // 工艺标准ToolStripMenuItem
            // 
            this.工艺标准ToolStripMenuItem.ForeColor = System.Drawing.Color.Black;
            this.工艺标准ToolStripMenuItem.Name = "工艺标准ToolStripMenuItem";
            this.工艺标准ToolStripMenuItem.Size = new System.Drawing.Size(68, 26);
            this.工艺标准ToolStripMenuItem.Text = "工艺标准";
            this.工艺标准ToolStripMenuItem.Click += new System.EventHandler(this.工艺标准ToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem4,
            this.toolStripMenuItem5,
            this.toolStripMenuItem6,
            this.toolStripMenuItem7});
            this.toolStripMenuItem3.ForeColor = System.Drawing.Color.Black;
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(68, 26);
            this.toolStripMenuItem3.Text = "外联设置";
            this.toolStripMenuItem3.Click += new System.EventHandler(this.外联设置ToolStripMenuItem_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(136, 22);
            this.toolStripMenuItem4.Text = "相机设置";
            this.toolStripMenuItem4.Click += new System.EventHandler(this.相机toolStripMenuItem1_Click);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(136, 22);
            this.toolStripMenuItem5.Text = "光源设置";
            this.toolStripMenuItem5.Click += new System.EventHandler(this.光源ToolStripMenuItem_Click);
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(136, 22);
            this.toolStripMenuItem6.Text = "主设备通讯";
            this.toolStripMenuItem6.Click += new System.EventHandler(this.pLC通讯ToolStripMenuItem_Click);
            // 
            // toolStripMenuItem7
            // 
            this.toolStripMenuItem7.Name = "toolStripMenuItem7";
            this.toolStripMenuItem7.Size = new System.Drawing.Size(136, 22);
            this.toolStripMenuItem7.Text = "控制卡通讯";
            this.toolStripMenuItem7.Click += new System.EventHandler(this.打标机软件通讯ToolStripMenuItem_Click);
            // 
            // 点检校验ToolStripMenuItem
            // 
            this.点检校验ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.相机标定ToolStripMenuItem,
            this.尺寸点检ToolStripMenuItem,
            this.灰度自适应ToolStripMenuItem});
            this.点检校验ToolStripMenuItem.ForeColor = System.Drawing.Color.Black;
            this.点检校验ToolStripMenuItem.Name = "点检校验ToolStripMenuItem";
            this.点检校验ToolStripMenuItem.Size = new System.Drawing.Size(68, 26);
            this.点检校验ToolStripMenuItem.Text = "点检校验";
            this.点检校验ToolStripMenuItem.Visible = false;
            this.点检校验ToolStripMenuItem.Click += new System.EventHandler(this.点检校验ToolStripMenuItem_Click);
            // 
            // 相机标定ToolStripMenuItem
            // 
            this.相机标定ToolStripMenuItem.Name = "相机标定ToolStripMenuItem";
            this.相机标定ToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.相机标定ToolStripMenuItem.Text = "相机标定";
            this.相机标定ToolStripMenuItem.Click += new System.EventHandler(this.相机标定ToolStripMenuItem_Click);
            // 
            // 尺寸点检ToolStripMenuItem
            // 
            this.尺寸点检ToolStripMenuItem.Name = "尺寸点检ToolStripMenuItem";
            this.尺寸点检ToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.尺寸点检ToolStripMenuItem.Text = "点检";
            this.尺寸点检ToolStripMenuItem.Visible = false;
            this.尺寸点检ToolStripMenuItem.Click += new System.EventHandler(this.尺寸点检ToolStripMenuItem_Click);
            // 
            // 灰度自适应ToolStripMenuItem
            // 
            this.灰度自适应ToolStripMenuItem.Name = "灰度自适应ToolStripMenuItem";
            this.灰度自适应ToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.灰度自适应ToolStripMenuItem.Text = "灰度自适应";
            this.灰度自适应ToolStripMenuItem.Click += new System.EventHandler(this.灰度自适应ToolStripMenuItem_Click);
            // 
            // 数据分析ToolStripMenuItem
            // 
            this.数据分析ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.生产统计ToolStripMenuItem,
            this.数据查询ToolStripMenuItem});
            this.数据分析ToolStripMenuItem.ForeColor = System.Drawing.Color.Black;
            this.数据分析ToolStripMenuItem.Name = "数据分析ToolStripMenuItem";
            this.数据分析ToolStripMenuItem.Size = new System.Drawing.Size(68, 26);
            this.数据分析ToolStripMenuItem.Text = "数据分析";
            this.数据分析ToolStripMenuItem.Click += new System.EventHandler(this.数据分析ToolStripMenuItem_Click);
            // 
            // 生产统计ToolStripMenuItem
            // 
            this.生产统计ToolStripMenuItem.Name = "生产统计ToolStripMenuItem";
            this.生产统计ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.生产统计ToolStripMenuItem.Text = "生产统计";
            this.生产统计ToolStripMenuItem.Click += new System.EventHandler(this.生产统计ToolStripMenuItem_Click);
            // 
            // 数据查询ToolStripMenuItem
            // 
            this.数据查询ToolStripMenuItem.Name = "数据查询ToolStripMenuItem";
            this.数据查询ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.数据查询ToolStripMenuItem.Text = "数据查询";
            this.数据查询ToolStripMenuItem.Click += new System.EventHandler(this.数据查询ToolStripMenuItem_Click);
            // 
            // 语言ToolStripMenuItem
            // 
            this.语言ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.chineseToolStripMenuItem,
            this.englishToolStripMenuItem});
            this.语言ToolStripMenuItem.ForeColor = System.Drawing.Color.Black;
            this.语言ToolStripMenuItem.Name = "语言ToolStripMenuItem";
            this.语言ToolStripMenuItem.Size = new System.Drawing.Size(44, 26);
            this.语言ToolStripMenuItem.Text = "语言";
            this.语言ToolStripMenuItem.Visible = false;
            // 
            // chineseToolStripMenuItem
            // 
            this.chineseToolStripMenuItem.Name = "chineseToolStripMenuItem";
            this.chineseToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
            this.chineseToolStripMenuItem.Text = "Chinese";
            this.chineseToolStripMenuItem.Click += new System.EventHandler(this.chineseToolStripMenuItem_Click);
            // 
            // englishToolStripMenuItem
            // 
            this.englishToolStripMenuItem.Name = "englishToolStripMenuItem";
            this.englishToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
            this.englishToolStripMenuItem.Text = "English";
            this.englishToolStripMenuItem.Click += new System.EventHandler(this.englishToolStripMenuItem_Click);
            // 
            // 帮助ToolStripMenuItem
            // 
            this.帮助ToolStripMenuItem.Name = "帮助ToolStripMenuItem";
            this.帮助ToolStripMenuItem.Size = new System.Drawing.Size(44, 26);
            this.帮助ToolStripMenuItem.Text = "帮助";
            this.帮助ToolStripMenuItem.Visible = false;
            // 
            // timer_StatusBarUpdate
            // 
            this.timer_StatusBarUpdate.Enabled = true;
            this.timer_StatusBarUpdate.Interval = 500;
            this.timer_StatusBarUpdate.Tick += new System.EventHandler(this.timer_StatusBarUpdate_Tick);
            // 
            // timer_SimLoadImage
            // 
            this.timer_SimLoadImage.Enabled = true;
            this.timer_SimLoadImage.Interval = 500;
            this.timer_SimLoadImage.Tick += new System.EventHandler(this.timer_SimLoadImage_Tick);
            // 
            // timer_DJ
            // 
            this.timer_DJ.Enabled = true;
            this.timer_DJ.Interval = 60000;
            this.timer_DJ.Tick += new System.EventHandler(this.timer_DJ_Tick);
            // 
            // timer_CraftBack
            // 
            this.timer_CraftBack.Enabled = true;
            this.timer_CraftBack.Interval = 7200000;
            this.timer_CraftBack.Tick += new System.EventHandler(this.timer_CraftBack_Tick);
            // 
            // uC_StateView1
            // 
            this.uC_StateView1.BackColor = System.Drawing.Color.White;
            this.tableLayoutPanel1.SetColumnSpan(this.uC_StateView1, 5);
            this.uC_StateView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uC_StateView1.Location = new System.Drawing.Point(0, 630);
            this.uC_StateView1.Margin = new System.Windows.Forms.Padding(0);
            this.uC_StateView1.Name = "uC_StateView1";
            this.uC_StateView1.Size = new System.Drawing.Size(1284, 31);
            this.uC_StateView1.TabIndex = 3;
            // 
            // Form_Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(6)))), ((int)(((byte)(16)))), ((int)(((byte)(24)))));
            this.ClientSize = new System.Drawing.Size(1284, 661);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "Form_Main";
            this.Text = "SlaveMachine";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_Main_FormClosing);
            this.Load += new System.EventHandler(this.Form_Main_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ToolStripMenuItem 调试设置ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 工艺标准ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 数据分析ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 生产统计ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 系统设置ToolStripMenuItem;
        private System.Windows.Forms.Panel panel_Main;
        private System.Windows.Forms.ToolStripMenuItem 帮助ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 用户设置ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 点检校验ToolStripMenuItem;
        private System.Windows.Forms.Timer timer_StatusBarUpdate;
        private System.Windows.Forms.ToolStripMenuItem 用户登录ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 账户管理ToolStripMenuItem;
        public System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 保存设置ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 语言ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem chineseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem englishToolStripMenuItem;
        private System.Windows.Forms.Timer timer_SimLoadImage;
        public System.Windows.Forms.ToolStripMenuItem 相机标定ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 磁盘设置ToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem 尺寸点检ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 基础设置ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 图片管理ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 数据查询ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 灰度自适应ToolStripMenuItem;
        private System.Windows.Forms.Timer timer_DJ;
        private System.Windows.Forms.ToolStripMenuItem 注销账户ToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        public System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
        public System.Windows.Forms.ToolStripMenuItem toolStripMenuItem5;
        public System.Windows.Forms.ToolStripMenuItem toolStripMenuItem6;
        public System.Windows.Forms.ToolStripMenuItem toolStripMenuItem7;
        private UC_StateView uC_StateView1;
        private System.Windows.Forms.Timer timer_CraftBack;
    }
}


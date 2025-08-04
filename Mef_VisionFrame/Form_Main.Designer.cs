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
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel4 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel5 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel6 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel7 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel8 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel9 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel10 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel11 = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.系统设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.保存设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.仿真设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.磁盘设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.用户设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.用户登录ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.账户管理ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.主界面ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.调试设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.工艺标准ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.外联设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.相机toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.光源ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pLC通讯ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.打标机软件通讯ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.纠偏设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pC通讯设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.控制卡ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.点检校验ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.相机标定ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.预定位平台标定ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.尺寸点检ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mark设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mSA圆点设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mSA线宽设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mSA棋盘格线宽设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mSA3D测厚ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.数据分析ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.生产统计ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.语言ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chineseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.englishToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.帮助ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel_Main = new System.Windows.Forms.Panel();
            this.timer_StatusBarUpdate = new System.Windows.Forms.Timer(this.components);
            this.timer_DataShow = new System.Windows.Forms.Timer(this.components);
            this.timer_SimLoadImage = new System.Windows.Forms.Timer(this.components);
            this.基础设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanel1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
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
            this.tableLayoutPanel1.Controls.Add(this.statusStrip1, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.menuStrip1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel_Main, 0, 1);
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
            // statusStrip1
            // 
            this.statusStrip1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.tableLayoutPanel1.SetColumnSpan(this.statusStrip1, 5);
            this.statusStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2,
            this.toolStripStatusLabel3,
            this.toolStripStatusLabel4,
            this.toolStripStatusLabel5,
            this.toolStripStatusLabel6,
            this.toolStripStatusLabel7,
            this.toolStripStatusLabel8,
            this.toolStripStatusLabel9,
            this.toolStripStatusLabel10,
            this.toolStripStatusLabel11});
            this.statusStrip1.Location = new System.Drawing.Point(0, 630);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1284, 31);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(92, 26);
            this.toolStripStatusLabel1.Text = "当前用户类型：";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(81, 26);
            this.toolStripStatusLabel2.Text = "当前用户ID：";
            // 
            // toolStripStatusLabel3
            // 
            this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            this.toolStripStatusLabel3.Size = new System.Drawing.Size(39, 26);
            this.toolStripStatusLabel3.Text = "相机1";
            // 
            // toolStripStatusLabel4
            // 
            this.toolStripStatusLabel4.Name = "toolStripStatusLabel4";
            this.toolStripStatusLabel4.Size = new System.Drawing.Size(39, 26);
            this.toolStripStatusLabel4.Text = "相机2";
            // 
            // toolStripStatusLabel5
            // 
            this.toolStripStatusLabel5.Name = "toolStripStatusLabel5";
            this.toolStripStatusLabel5.Size = new System.Drawing.Size(39, 26);
            this.toolStripStatusLabel5.Text = "相机3";
            // 
            // toolStripStatusLabel6
            // 
            this.toolStripStatusLabel6.Name = "toolStripStatusLabel6";
            this.toolStripStatusLabel6.Size = new System.Drawing.Size(39, 26);
            this.toolStripStatusLabel6.Text = "相机4";
            // 
            // toolStripStatusLabel7
            // 
            this.toolStripStatusLabel7.Name = "toolStripStatusLabel7";
            this.toolStripStatusLabel7.Size = new System.Drawing.Size(39, 26);
            this.toolStripStatusLabel7.Text = "相机5";
            // 
            // toolStripStatusLabel8
            // 
            this.toolStripStatusLabel8.Name = "toolStripStatusLabel8";
            this.toolStripStatusLabel8.Size = new System.Drawing.Size(39, 26);
            this.toolStripStatusLabel8.Text = "光源1";
            // 
            // toolStripStatusLabel9
            // 
            this.toolStripStatusLabel9.Name = "toolStripStatusLabel9";
            this.toolStripStatusLabel9.Size = new System.Drawing.Size(39, 26);
            this.toolStripStatusLabel9.Text = "光源2";
            // 
            // toolStripStatusLabel10
            // 
            this.toolStripStatusLabel10.Name = "toolStripStatusLabel10";
            this.toolStripStatusLabel10.Size = new System.Drawing.Size(39, 26);
            this.toolStripStatusLabel10.Text = "光源3";
            // 
            // toolStripStatusLabel11
            // 
            this.toolStripStatusLabel11.BackColor = System.Drawing.SystemColors.Control;
            this.toolStripStatusLabel11.Name = "toolStripStatusLabel11";
            this.toolStripStatusLabel11.Size = new System.Drawing.Size(65, 26);
            this.toolStripStatusLabel11.Text = "打标机PLC";
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.tableLayoutPanel1.SetColumnSpan(this.menuStrip1, 5);
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.系统设置ToolStripMenuItem,
            this.用户设置ToolStripMenuItem,
            this.主界面ToolStripMenuItem,
            this.调试设置ToolStripMenuItem,
            this.工艺标准ToolStripMenuItem,
            this.外联设置ToolStripMenuItem,
            this.点检校验ToolStripMenuItem,
            this.数据分析ToolStripMenuItem,
            this.语言ToolStripMenuItem,
            this.帮助ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(1284, 25);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.menuStrip1_ItemClicked);
            // 
            // 系统设置ToolStripMenuItem
            // 
            this.系统设置ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.保存设置ToolStripMenuItem,
            this.仿真设置ToolStripMenuItem,
            this.磁盘设置ToolStripMenuItem,
            this.基础设置ToolStripMenuItem});
            this.系统设置ToolStripMenuItem.Name = "系统设置ToolStripMenuItem";
            this.系统设置ToolStripMenuItem.Size = new System.Drawing.Size(68, 21);
            this.系统设置ToolStripMenuItem.Text = "系统设置";
            this.系统设置ToolStripMenuItem.Click += new System.EventHandler(this.系统设置ToolStripMenuItem_Click);
            // 
            // 保存设置ToolStripMenuItem
            // 
            this.保存设置ToolStripMenuItem.Name = "保存设置ToolStripMenuItem";
            this.保存设置ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.保存设置ToolStripMenuItem.Text = "保存设置";
            this.保存设置ToolStripMenuItem.Click += new System.EventHandler(this.保存设置ToolStripMenuItem_Click);
            // 
            // 仿真设置ToolStripMenuItem
            // 
            this.仿真设置ToolStripMenuItem.Name = "仿真设置ToolStripMenuItem";
            this.仿真设置ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.仿真设置ToolStripMenuItem.Text = "仿真设置";
            this.仿真设置ToolStripMenuItem.Click += new System.EventHandler(this.仿真设置ToolStripMenuItem_Click);
            // 
            // 磁盘设置ToolStripMenuItem
            // 
            this.磁盘设置ToolStripMenuItem.Name = "磁盘设置ToolStripMenuItem";
            this.磁盘设置ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.磁盘设置ToolStripMenuItem.Text = "磁盘设置";
            this.磁盘设置ToolStripMenuItem.Click += new System.EventHandler(this.磁盘设置ToolStripMenuItem_Click);
            // 
            // 用户设置ToolStripMenuItem
            // 
            this.用户设置ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.用户登录ToolStripMenuItem,
            this.账户管理ToolStripMenuItem});
            this.用户设置ToolStripMenuItem.Name = "用户设置ToolStripMenuItem";
            this.用户设置ToolStripMenuItem.Size = new System.Drawing.Size(68, 21);
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
            // 主界面ToolStripMenuItem
            // 
            this.主界面ToolStripMenuItem.Name = "主界面ToolStripMenuItem";
            this.主界面ToolStripMenuItem.Size = new System.Drawing.Size(56, 21);
            this.主界面ToolStripMenuItem.Text = "主界面";
            this.主界面ToolStripMenuItem.Click += new System.EventHandler(this.主界面ToolStripMenuItem_Click);
            // 
            // 调试设置ToolStripMenuItem
            // 
            this.调试设置ToolStripMenuItem.Name = "调试设置ToolStripMenuItem";
            this.调试设置ToolStripMenuItem.Size = new System.Drawing.Size(68, 21);
            this.调试设置ToolStripMenuItem.Text = "调试设置";
            this.调试设置ToolStripMenuItem.Click += new System.EventHandler(this.调试设置ToolStripMenuItem_Click);
            // 
            // 工艺标准ToolStripMenuItem
            // 
            this.工艺标准ToolStripMenuItem.Name = "工艺标准ToolStripMenuItem";
            this.工艺标准ToolStripMenuItem.Size = new System.Drawing.Size(68, 21);
            this.工艺标准ToolStripMenuItem.Text = "工艺标准";
            this.工艺标准ToolStripMenuItem.Click += new System.EventHandler(this.工艺标准ToolStripMenuItem_Click);
            // 
            // 外联设置ToolStripMenuItem
            // 
            this.外联设置ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.相机toolStripMenuItem1,
            this.光源ToolStripMenuItem,
            this.pLC通讯ToolStripMenuItem,
            this.打标机软件通讯ToolStripMenuItem,
            this.纠偏设置ToolStripMenuItem,
            this.pC通讯设置ToolStripMenuItem,
            this.控制卡ToolStripMenuItem});
            this.外联设置ToolStripMenuItem.Name = "外联设置ToolStripMenuItem";
            this.外联设置ToolStripMenuItem.Size = new System.Drawing.Size(68, 21);
            this.外联设置ToolStripMenuItem.Text = "外联设置";
            this.外联设置ToolStripMenuItem.Click += new System.EventHandler(this.外联设置ToolStripMenuItem_Click);
            // 
            // 相机toolStripMenuItem1
            // 
            this.相机toolStripMenuItem1.Name = "相机toolStripMenuItem1";
            this.相机toolStripMenuItem1.Size = new System.Drawing.Size(160, 22);
            this.相机toolStripMenuItem1.Text = "相机设置";
            this.相机toolStripMenuItem1.Click += new System.EventHandler(this.相机toolStripMenuItem1_Click);
            // 
            // 光源ToolStripMenuItem
            // 
            this.光源ToolStripMenuItem.Name = "光源ToolStripMenuItem";
            this.光源ToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.光源ToolStripMenuItem.Text = "光源设置";
            this.光源ToolStripMenuItem.Click += new System.EventHandler(this.光源ToolStripMenuItem_Click);
            // 
            // pLC通讯ToolStripMenuItem
            // 
            this.pLC通讯ToolStripMenuItem.Name = "pLC通讯ToolStripMenuItem";
            this.pLC通讯ToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.pLC通讯ToolStripMenuItem.Text = "主设备通讯";
            this.pLC通讯ToolStripMenuItem.Click += new System.EventHandler(this.pLC通讯ToolStripMenuItem_Click);
            // 
            // 打标机软件通讯ToolStripMenuItem
            // 
            this.打标机软件通讯ToolStripMenuItem.Name = "打标机软件通讯ToolStripMenuItem";
            this.打标机软件通讯ToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.打标机软件通讯ToolStripMenuItem.Text = "打标机软件通讯";
            this.打标机软件通讯ToolStripMenuItem.Visible = false;
            this.打标机软件通讯ToolStripMenuItem.Click += new System.EventHandler(this.打标机软件通讯ToolStripMenuItem_Click);
            // 
            // 纠偏设置ToolStripMenuItem
            // 
            this.纠偏设置ToolStripMenuItem.Name = "纠偏设置ToolStripMenuItem";
            this.纠偏设置ToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.纠偏设置ToolStripMenuItem.Text = "纠偏设置";
            this.纠偏设置ToolStripMenuItem.Visible = false;
            this.纠偏设置ToolStripMenuItem.Click += new System.EventHandler(this.纠偏设置ToolStripMenuItem_Click);
            // 
            // pC通讯设置ToolStripMenuItem
            // 
            this.pC通讯设置ToolStripMenuItem.Name = "pC通讯设置ToolStripMenuItem";
            this.pC通讯设置ToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.pC通讯设置ToolStripMenuItem.Text = "PC通讯设置";
            this.pC通讯设置ToolStripMenuItem.Visible = false;
            this.pC通讯设置ToolStripMenuItem.Click += new System.EventHandler(this.pC通讯设置ToolStripMenuItem_Click);
            // 
            // 控制卡ToolStripMenuItem
            // 
            this.控制卡ToolStripMenuItem.Name = "控制卡ToolStripMenuItem";
            this.控制卡ToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.控制卡ToolStripMenuItem.Text = "控制卡";
            this.控制卡ToolStripMenuItem.Visible = false;
            this.控制卡ToolStripMenuItem.Click += new System.EventHandler(this.控制卡ToolStripMenuItem_Click);
            // 
            // 点检校验ToolStripMenuItem
            // 
            this.点检校验ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.相机标定ToolStripMenuItem,
            this.预定位平台标定ToolStripMenuItem,
            this.尺寸点检ToolStripMenuItem,
            this.mark设置ToolStripMenuItem,
            this.mSA圆点设置ToolStripMenuItem,
            this.mSA线宽设置ToolStripMenuItem,
            this.mSA棋盘格线宽设置ToolStripMenuItem,
            this.mSA3D测厚ToolStripMenuItem});
            this.点检校验ToolStripMenuItem.Name = "点检校验ToolStripMenuItem";
            this.点检校验ToolStripMenuItem.Size = new System.Drawing.Size(68, 21);
            this.点检校验ToolStripMenuItem.Text = "点检校验";
            this.点检校验ToolStripMenuItem.Click += new System.EventHandler(this.点检校验ToolStripMenuItem_Click);
            // 
            // 相机标定ToolStripMenuItem
            // 
            this.相机标定ToolStripMenuItem.Name = "相机标定ToolStripMenuItem";
            this.相机标定ToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.相机标定ToolStripMenuItem.Text = "相机标定";
            this.相机标定ToolStripMenuItem.Click += new System.EventHandler(this.相机标定ToolStripMenuItem_Click);
            // 
            // 预定位平台标定ToolStripMenuItem
            // 
            this.预定位平台标定ToolStripMenuItem.Name = "预定位平台标定ToolStripMenuItem";
            this.预定位平台标定ToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.预定位平台标定ToolStripMenuItem.Text = "预定位平台标定";
            this.预定位平台标定ToolStripMenuItem.Click += new System.EventHandler(this.预定位平台标定ToolStripMenuItem_Click);
            // 
            // 尺寸点检ToolStripMenuItem
            // 
            this.尺寸点检ToolStripMenuItem.Name = "尺寸点检ToolStripMenuItem";
            this.尺寸点检ToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.尺寸点检ToolStripMenuItem.Text = "尺寸点检";
            this.尺寸点检ToolStripMenuItem.Visible = false;
            this.尺寸点检ToolStripMenuItem.Click += new System.EventHandler(this.尺寸点检ToolStripMenuItem_Click);
            // 
            // mark设置ToolStripMenuItem
            // 
            this.mark设置ToolStripMenuItem.Name = "mark设置ToolStripMenuItem";
            this.mark设置ToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.mark设置ToolStripMenuItem.Text = "Mark设置";
            this.mark设置ToolStripMenuItem.Click += new System.EventHandler(this.mark设置ToolStripMenuItem_Click);
            // 
            // mSA圆点设置ToolStripMenuItem
            // 
            this.mSA圆点设置ToolStripMenuItem.Name = "mSA圆点设置ToolStripMenuItem";
            this.mSA圆点设置ToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.mSA圆点设置ToolStripMenuItem.Text = "MSA圆点设置";
            this.mSA圆点设置ToolStripMenuItem.Click += new System.EventHandler(this.mSA圆点设置ToolStripMenuItem_Click);
            // 
            // mSA线宽设置ToolStripMenuItem
            // 
            this.mSA线宽设置ToolStripMenuItem.Name = "mSA线宽设置ToolStripMenuItem";
            this.mSA线宽设置ToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.mSA线宽设置ToolStripMenuItem.Text = "MSA线宽设置";
            this.mSA线宽设置ToolStripMenuItem.Click += new System.EventHandler(this.mSA线宽设置ToolStripMenuItem_Click);
            // 
            // mSA棋盘格线宽设置ToolStripMenuItem
            // 
            this.mSA棋盘格线宽设置ToolStripMenuItem.Name = "mSA棋盘格线宽设置ToolStripMenuItem";
            this.mSA棋盘格线宽设置ToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.mSA棋盘格线宽设置ToolStripMenuItem.Text = "MSA棋盘格线宽设置";
            this.mSA棋盘格线宽设置ToolStripMenuItem.Click += new System.EventHandler(this.mSA棋盘格线宽设置ToolStripMenuItem_Click);
            // 
            // mSA3D测厚ToolStripMenuItem
            // 
            this.mSA3D测厚ToolStripMenuItem.Name = "mSA3D测厚ToolStripMenuItem";
            this.mSA3D测厚ToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.mSA3D测厚ToolStripMenuItem.Text = "MSA3D测厚";
            this.mSA3D测厚ToolStripMenuItem.Click += new System.EventHandler(this.mSA3D测厚ToolStripMenuItem_Click);
            // 
            // 数据分析ToolStripMenuItem
            // 
            this.数据分析ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.生产统计ToolStripMenuItem});
            this.数据分析ToolStripMenuItem.Name = "数据分析ToolStripMenuItem";
            this.数据分析ToolStripMenuItem.Size = new System.Drawing.Size(68, 21);
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
            // 语言ToolStripMenuItem
            // 
            this.语言ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.chineseToolStripMenuItem,
            this.englishToolStripMenuItem});
            this.语言ToolStripMenuItem.Name = "语言ToolStripMenuItem";
            this.语言ToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.语言ToolStripMenuItem.Text = "语言";
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
            this.帮助ToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.帮助ToolStripMenuItem.Text = "帮助";
            this.帮助ToolStripMenuItem.Visible = false;
            // 
            // panel_Main
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.panel_Main, 5);
            this.panel_Main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_Main.Location = new System.Drawing.Point(0, 30);
            this.panel_Main.Margin = new System.Windows.Forms.Padding(0);
            this.panel_Main.Name = "panel_Main";
            this.tableLayoutPanel1.SetRowSpan(this.panel_Main, 3);
            this.panel_Main.Size = new System.Drawing.Size(1284, 600);
            this.panel_Main.TabIndex = 2;
            // 
            // timer_StatusBarUpdate
            // 
            this.timer_StatusBarUpdate.Enabled = true;
            this.timer_StatusBarUpdate.Interval = 200;
            this.timer_StatusBarUpdate.Tick += new System.EventHandler(this.timer_StatusBarUpdate_Tick);
            // 
            // timer_DataShow
            // 
            this.timer_DataShow.Enabled = true;
            this.timer_DataShow.Interval = 200;
            this.timer_DataShow.Tick += new System.EventHandler(this.timer_DataShow_Tick);
            // 
            // timer_SimLoadImage
            // 
            this.timer_SimLoadImage.Enabled = true;
            this.timer_SimLoadImage.Interval = 500;
            this.timer_SimLoadImage.Tick += new System.EventHandler(this.timer_SimLoadImage_Tick);
            // 
            // 基础设置ToolStripMenuItem
            // 
            this.基础设置ToolStripMenuItem.Name = "基础设置ToolStripMenuItem";
            this.基础设置ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.基础设置ToolStripMenuItem.Text = "基础设置";
            this.基础设置ToolStripMenuItem.Click += new System.EventHandler(this.基础设置ToolStripMenuItem_Click);
            // 
            // Form_Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
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
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
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
        private System.Windows.Forms.ToolStripMenuItem 主界面ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 帮助ToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel3;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel4;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel5;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel6;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel7;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel8;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel9;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel10;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel11;
        private System.Windows.Forms.ToolStripMenuItem 用户设置ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 点检校验ToolStripMenuItem;
        private System.Windows.Forms.Timer timer_StatusBarUpdate;
        private System.Windows.Forms.ToolStripMenuItem 用户登录ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 账户管理ToolStripMenuItem;
        public System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 保存设置ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 仿真设置ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 语言ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem chineseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem englishToolStripMenuItem;
        private System.Windows.Forms.Timer timer_DataShow;
        private System.Windows.Forms.Timer timer_SimLoadImage;
        public System.Windows.Forms.ToolStripMenuItem 外联设置ToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem pLC通讯ToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem 相机toolStripMenuItem1;
        public System.Windows.Forms.ToolStripMenuItem 光源ToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem 打标机软件通讯ToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem pC通讯设置ToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem 纠偏设置ToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem 控制卡ToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem 预定位平台标定ToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem 相机标定ToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem mark设置ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mSA圆点设置ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mSA线宽设置ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 磁盘设置ToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem 尺寸点检ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mSA棋盘格线宽设置ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mSA3D测厚ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 基础设置ToolStripMenuItem;
    }
}


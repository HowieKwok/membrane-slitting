namespace Frame_Core
{
    partial class Form_MsaMeasureThick3D
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_MsaMeasureThick3D));
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.Btn_CreateData = new System.Windows.Forms.Button();
            this.lb1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.groupbox = new System.Windows.Forms.GroupBox();
            this.tb_ProductFile = new System.Windows.Forms.TextBox();
            this.tb_CheckFile = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cbx_CheckEnable = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.LineMethodSelect = new System.Windows.Forms.ComboBox();
            this.btn_Preview = new System.Windows.Forms.Button();
            this.btn_Next = new System.Windows.Forms.Button();
            this.btn_Run = new System.Windows.Forms.Button();
            this.cbx_CamSelect = new System.Windows.Forms.ComboBox();
            this.测量ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.工具ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.清除所有区域ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.删除指定区域ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.画线ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.画矩形1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.画圆ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.添加检测区域ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.画矩形2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.LB_IMageSize = new System.Windows.Forms.Label();
            this.LB_ModleName = new System.Windows.Forms.Label();
            this.TB_PositionAndGray = new System.Windows.Forms.TextBox();
            this.HWC_Display = new HalconDotNet.HWindowControl();
            this.TLPanel_BGM1 = new System.Windows.Forms.TableLayoutPanel();
            this.TB_PositionAndGray3D = new System.Windows.Forms.TextBox();
            this.ImageReset = new System.Windows.Forms.PictureBox();
            this.LoadImage = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupbox.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.TLPanel_BGM1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ImageReset)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LoadImage)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(27, 77);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(65, 12);
            this.linkLabel1.TabIndex = 42;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "linkLabel1";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // Btn_CreateData
            // 
            this.Btn_CreateData.Location = new System.Drawing.Point(481, 210);
            this.Btn_CreateData.Margin = new System.Windows.Forms.Padding(2);
            this.Btn_CreateData.Name = "Btn_CreateData";
            this.Btn_CreateData.Size = new System.Drawing.Size(73, 31);
            this.Btn_CreateData.TabIndex = 41;
            this.Btn_CreateData.Text = "生成数据";
            this.Btn_CreateData.UseVisualStyleBackColor = true;
            this.Btn_CreateData.Click += new System.EventHandler(this.Btn_CreateData_Click);
            // 
            // lb1
            // 
            this.lb1.AutoSize = true;
            this.lb1.Location = new System.Drawing.Point(27, 41);
            this.lb1.Name = "lb1";
            this.lb1.Size = new System.Drawing.Size(53, 12);
            this.lb1.TabIndex = 40;
            this.lb1.Text = "相机选择";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(394, 210);
            this.button1.Margin = new System.Windows.Forms.Padding(2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(73, 31);
            this.button1.TabIndex = 38;
            this.button1.Text = "保存设置";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupbox
            // 
            this.groupbox.Controls.Add(this.tb_ProductFile);
            this.groupbox.Controls.Add(this.tb_CheckFile);
            this.groupbox.Controls.Add(this.label3);
            this.groupbox.Controls.Add(this.label2);
            this.groupbox.Controls.Add(this.cbx_CheckEnable);
            this.groupbox.Controls.Add(this.label1);
            this.groupbox.Controls.Add(this.LineMethodSelect);
            this.groupbox.Controls.Add(this.btn_Preview);
            this.groupbox.Controls.Add(this.btn_Next);
            this.groupbox.Controls.Add(this.btn_Run);
            this.groupbox.Controls.Add(this.linkLabel1);
            this.groupbox.Controls.Add(this.Btn_CreateData);
            this.groupbox.Controls.Add(this.lb1);
            this.groupbox.Controls.Add(this.cbx_CamSelect);
            this.groupbox.Controls.Add(this.button1);
            this.groupbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupbox.Location = new System.Drawing.Point(3, 267);
            this.groupbox.Name = "groupbox";
            this.groupbox.Size = new System.Drawing.Size(566, 258);
            this.groupbox.TabIndex = 4;
            this.groupbox.TabStop = false;
            this.groupbox.Text = "参数设置";
            // 
            // tb_ProductFile
            // 
            this.tb_ProductFile.Location = new System.Drawing.Point(113, 181);
            this.tb_ProductFile.Name = "tb_ProductFile";
            this.tb_ProductFile.Size = new System.Drawing.Size(100, 21);
            this.tb_ProductFile.TabIndex = 52;
            // 
            // tb_CheckFile
            // 
            this.tb_CheckFile.Location = new System.Drawing.Point(113, 146);
            this.tb_CheckFile.Name = "tb_CheckFile";
            this.tb_CheckFile.Size = new System.Drawing.Size(100, 21);
            this.tb_CheckFile.TabIndex = 51;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(27, 185);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 12);
            this.label3.TabIndex = 50;
            this.label3.Text = "生产工程文件";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(27, 149);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 12);
            this.label2.TabIndex = 49;
            this.label2.Text = "点检工程文件";
            // 
            // cbx_CheckEnable
            // 
            this.cbx_CheckEnable.AutoSize = true;
            this.cbx_CheckEnable.Location = new System.Drawing.Point(27, 221);
            this.cbx_CheckEnable.Name = "cbx_CheckEnable";
            this.cbx_CheckEnable.Size = new System.Drawing.Size(72, 16);
            this.cbx_CheckEnable.TabIndex = 48;
            this.cbx_CheckEnable.Text = "启用点检";
            this.cbx_CheckEnable.UseVisualStyleBackColor = true;
            this.cbx_CheckEnable.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 113);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 12);
            this.label1.TabIndex = 47;
            this.label1.Text = "线型算法选择";
            // 
            // LineMethodSelect
            // 
            this.LineMethodSelect.FormattingEnabled = true;
            this.LineMethodSelect.Items.AddRange(new object[] {
            "找点算法",
            "灰度直方图"});
            this.LineMethodSelect.Location = new System.Drawing.Point(113, 105);
            this.LineMethodSelect.Name = "LineMethodSelect";
            this.LineMethodSelect.Size = new System.Drawing.Size(121, 20);
            this.LineMethodSelect.TabIndex = 46;
            // 
            // btn_Preview
            // 
            this.btn_Preview.BackColor = System.Drawing.SystemColors.Control;
            this.btn_Preview.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn_Preview.BackgroundImage")));
            this.btn_Preview.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btn_Preview.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold);
            this.btn_Preview.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Preview.Location = new System.Drawing.Point(394, 30);
            this.btn_Preview.Name = "btn_Preview";
            this.btn_Preview.Size = new System.Drawing.Size(35, 30);
            this.btn_Preview.TabIndex = 43;
            this.btn_Preview.UseVisualStyleBackColor = false;
            this.btn_Preview.Visible = false;
            // 
            // btn_Next
            // 
            this.btn_Next.BackColor = System.Drawing.SystemColors.Control;
            this.btn_Next.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn_Next.BackgroundImage")));
            this.btn_Next.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btn_Next.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold);
            this.btn_Next.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Next.Location = new System.Drawing.Point(500, 30);
            this.btn_Next.Name = "btn_Next";
            this.btn_Next.Size = new System.Drawing.Size(35, 30);
            this.btn_Next.TabIndex = 45;
            this.btn_Next.UseVisualStyleBackColor = false;
            this.btn_Next.Click += new System.EventHandler(this.btn_Next_Click);
            // 
            // btn_Run
            // 
            this.btn_Run.BackColor = System.Drawing.SystemColors.Control;
            this.btn_Run.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn_Run.BackgroundImage")));
            this.btn_Run.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btn_Run.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold);
            this.btn_Run.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Run.Location = new System.Drawing.Point(447, 30);
            this.btn_Run.Name = "btn_Run";
            this.btn_Run.Size = new System.Drawing.Size(35, 30);
            this.btn_Run.TabIndex = 44;
            this.btn_Run.UseVisualStyleBackColor = false;
            this.btn_Run.Click += new System.EventHandler(this.btn_Run_Click);
            // 
            // cbx_CamSelect
            // 
            this.cbx_CamSelect.FormattingEnabled = true;
            this.cbx_CamSelect.Location = new System.Drawing.Point(113, 38);
            this.cbx_CamSelect.Name = "cbx_CamSelect";
            this.cbx_CamSelect.Size = new System.Drawing.Size(121, 20);
            this.cbx_CamSelect.TabIndex = 39;
            this.cbx_CamSelect.SelectedIndexChanged += new System.EventHandler(this.cbx_CamSelect_SelectedIndexChanged);
            // 
            // 测量ToolStripMenuItem
            // 
            this.测量ToolStripMenuItem.Name = "测量ToolStripMenuItem";
            this.测量ToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.测量ToolStripMenuItem.Text = "测量";
            // 
            // 工具ToolStripMenuItem
            // 
            this.工具ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.测量ToolStripMenuItem});
            this.工具ToolStripMenuItem.Name = "工具ToolStripMenuItem";
            this.工具ToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.工具ToolStripMenuItem.Text = "工具";
            // 
            // 清除所有区域ToolStripMenuItem
            // 
            this.清除所有区域ToolStripMenuItem.Name = "清除所有区域ToolStripMenuItem";
            this.清除所有区域ToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.清除所有区域ToolStripMenuItem.Text = "清除所有区域";
            this.清除所有区域ToolStripMenuItem.Click += new System.EventHandler(this.清除所有区域ToolStripMenuItem_Click);
            // 
            // 删除指定区域ToolStripMenuItem
            // 
            this.删除指定区域ToolStripMenuItem.Name = "删除指定区域ToolStripMenuItem";
            this.删除指定区域ToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.删除指定区域ToolStripMenuItem.Text = "删除指定区域";
            this.删除指定区域ToolStripMenuItem.Click += new System.EventHandler(this.删除指定区域ToolStripMenuItem_Click);
            // 
            // 画线ToolStripMenuItem
            // 
            this.画线ToolStripMenuItem.Name = "画线ToolStripMenuItem";
            this.画线ToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
            this.画线ToolStripMenuItem.Text = "画线";
            this.画线ToolStripMenuItem.Click += new System.EventHandler(this.画线ToolStripMenuItem_Click);
            // 
            // 画矩形1ToolStripMenuItem
            // 
            this.画矩形1ToolStripMenuItem.Name = "画矩形1ToolStripMenuItem";
            this.画矩形1ToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
            this.画矩形1ToolStripMenuItem.Text = "画矩形1";
            this.画矩形1ToolStripMenuItem.Click += new System.EventHandler(this.画矩形1ToolStripMenuItem_Click);
            // 
            // 画圆ToolStripMenuItem
            // 
            this.画圆ToolStripMenuItem.Name = "画圆ToolStripMenuItem";
            this.画圆ToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
            this.画圆ToolStripMenuItem.Text = "画圆";
            this.画圆ToolStripMenuItem.Click += new System.EventHandler(this.画圆ToolStripMenuItem_Click);
            // 
            // 添加检测区域ToolStripMenuItem
            // 
            this.添加检测区域ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.画圆ToolStripMenuItem,
            this.画矩形1ToolStripMenuItem,
            this.画矩形2ToolStripMenuItem,
            this.画线ToolStripMenuItem});
            this.添加检测区域ToolStripMenuItem.Name = "添加检测区域ToolStripMenuItem";
            this.添加检测区域ToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.添加检测区域ToolStripMenuItem.Text = "添加检测区域";
            // 
            // 画矩形2ToolStripMenuItem
            // 
            this.画矩形2ToolStripMenuItem.Name = "画矩形2ToolStripMenuItem";
            this.画矩形2ToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
            this.画矩形2ToolStripMenuItem.Text = "画矩形2";
            this.画矩形2ToolStripMenuItem.Click += new System.EventHandler(this.画矩形2ToolStripMenuItem_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.添加检测区域ToolStripMenuItem,
            this.删除指定区域ToolStripMenuItem,
            this.清除所有区域ToolStripMenuItem,
            this.工具ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(149, 92);
            // 
            // LB_IMageSize
            // 
            this.LB_IMageSize.AutoSize = true;
            this.LB_IMageSize.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.TLPanel_BGM1.SetColumnSpan(this.LB_IMageSize, 3);
            this.LB_IMageSize.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LB_IMageSize.Font = new System.Drawing.Font("黑体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LB_IMageSize.ForeColor = System.Drawing.Color.White;
            this.LB_IMageSize.Location = new System.Drawing.Point(312, 5);
            this.LB_IMageSize.Margin = new System.Windows.Forms.Padding(13, 5, 3, 3);
            this.LB_IMageSize.Name = "LB_IMageSize";
            this.LB_IMageSize.Size = new System.Drawing.Size(207, 27);
            this.LB_IMageSize.TabIndex = 15;
            this.LB_IMageSize.Text = "Size  R:—   C:—";
            this.LB_IMageSize.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LB_ModleName
            // 
            this.LB_ModleName.AutoSize = true;
            this.LB_ModleName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.TLPanel_BGM1.SetColumnSpan(this.LB_ModleName, 4);
            this.LB_ModleName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LB_ModleName.Font = new System.Drawing.Font("黑体", 12.75F);
            this.LB_ModleName.ForeColor = System.Drawing.Color.White;
            this.LB_ModleName.Location = new System.Drawing.Point(13, 5);
            this.LB_ModleName.Margin = new System.Windows.Forms.Padding(13, 5, 3, 3);
            this.LB_ModleName.Name = "LB_ModleName";
            this.LB_ModleName.Size = new System.Drawing.Size(154, 27);
            this.LB_ModleName.TabIndex = 2;
            this.LB_ModleName.Text = "区域检测：";
            this.LB_ModleName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // TB_PositionAndGray
            // 
            this.TB_PositionAndGray.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.TB_PositionAndGray.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TLPanel_BGM1.SetColumnSpan(this.TB_PositionAndGray, 4);
            this.TB_PositionAndGray.Cursor = System.Windows.Forms.Cursors.Default;
            this.TB_PositionAndGray.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TB_PositionAndGray.Font = new System.Drawing.Font("黑体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.TB_PositionAndGray.ForeColor = System.Drawing.Color.White;
            this.TB_PositionAndGray.Location = new System.Drawing.Point(223, 235);
            this.TB_PositionAndGray.Margin = new System.Windows.Forms.Padding(5, 8, 5, 6);
            this.TB_PositionAndGray.Name = "TB_PositionAndGray";
            this.TB_PositionAndGray.ShortcutsEnabled = false;
            this.TB_PositionAndGray.Size = new System.Drawing.Size(294, 19);
            this.TB_PositionAndGray.TabIndex = 8;
            this.TB_PositionAndGray.TabStop = false;
            this.TB_PositionAndGray.Text = "R: —  C: —  Value: — ";
            // 
            // HWC_Display
            // 
            this.HWC_Display.BackColor = System.Drawing.Color.White;
            this.HWC_Display.BorderColor = System.Drawing.Color.White;
            this.TLPanel_BGM1.SetColumnSpan(this.HWC_Display, 10);
            this.HWC_Display.ContextMenuStrip = this.contextMenuStrip1;
            this.HWC_Display.Dock = System.Windows.Forms.DockStyle.Fill;
            this.HWC_Display.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.HWC_Display.Location = new System.Drawing.Point(3, 38);
            this.HWC_Display.Name = "HWC_Display";
            this.TLPanel_BGM1.SetRowSpan(this.HWC_Display, 8);
            this.HWC_Display.Size = new System.Drawing.Size(566, 186);
            this.HWC_Display.TabIndex = 9;
            this.HWC_Display.WindowSize = new System.Drawing.Size(566, 186);
            this.HWC_Display.HMouseMove += new HalconDotNet.HMouseEventHandler(this.HWC_Display_HMouseMove);
            // 
            // TLPanel_BGM1
            // 
            this.TLPanel_BGM1.AutoSize = true;
            this.TLPanel_BGM1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.TLPanel_BGM1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.TLPanel_BGM1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.TLPanel_BGM1.ColumnCount = 10;
            this.TLPanel_BGM1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.TLPanel_BGM1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.TLPanel_BGM1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.TLPanel_BGM1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.TLPanel_BGM1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TLPanel_BGM1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 81F));
            this.TLPanel_BGM1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.TLPanel_BGM1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 66F));
            this.TLPanel_BGM1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 87F));
            this.TLPanel_BGM1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.TLPanel_BGM1.Controls.Add(this.TB_PositionAndGray3D, 1, 9);
            this.TLPanel_BGM1.Controls.Add(this.LB_IMageSize, 6, 0);
            this.TLPanel_BGM1.Controls.Add(this.ImageReset, 9, 9);
            this.TLPanel_BGM1.Controls.Add(this.LoadImage, 0, 9);
            this.TLPanel_BGM1.Controls.Add(this.LB_ModleName, 0, 0);
            this.TLPanel_BGM1.Controls.Add(this.TB_PositionAndGray, 5, 9);
            this.TLPanel_BGM1.Controls.Add(this.HWC_Display, 0, 1);
            this.TLPanel_BGM1.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.TLPanel_BGM1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TLPanel_BGM1.Location = new System.Drawing.Point(0, 0);
            this.TLPanel_BGM1.Margin = new System.Windows.Forms.Padding(0);
            this.TLPanel_BGM1.Name = "TLPanel_BGM1";
            this.TLPanel_BGM1.RowCount = 10;
            this.TLPanel_BGM1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.TLPanel_BGM1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.TLPanel_BGM1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.TLPanel_BGM1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.TLPanel_BGM1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.TLPanel_BGM1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.TLPanel_BGM1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.TLPanel_BGM1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.TLPanel_BGM1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.TLPanel_BGM1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.TLPanel_BGM1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.TLPanel_BGM1.Size = new System.Drawing.Size(572, 264);
            this.TLPanel_BGM1.TabIndex = 3;
            // 
            // TB_PositionAndGray3D
            // 
            this.TB_PositionAndGray3D.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.TB_PositionAndGray3D.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TLPanel_BGM1.SetColumnSpan(this.TB_PositionAndGray3D, 4);
            this.TB_PositionAndGray3D.Cursor = System.Windows.Forms.Cursors.Default;
            this.TB_PositionAndGray3D.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TB_PositionAndGray3D.Font = new System.Drawing.Font("黑体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.TB_PositionAndGray3D.ForeColor = System.Drawing.Color.White;
            this.TB_PositionAndGray3D.Location = new System.Drawing.Point(55, 235);
            this.TB_PositionAndGray3D.Margin = new System.Windows.Forms.Padding(5, 8, 5, 6);
            this.TB_PositionAndGray3D.Name = "TB_PositionAndGray3D";
            this.TB_PositionAndGray3D.ShortcutsEnabled = false;
            this.TB_PositionAndGray3D.Size = new System.Drawing.Size(158, 19);
            this.TB_PositionAndGray3D.TabIndex = 16;
            this.TB_PositionAndGray3D.TabStop = false;
            this.TB_PositionAndGray3D.Text = "R: —  C: —  Value: — ";
            // 
            // ImageReset
            // 
            this.ImageReset.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.ImageReset.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("ImageReset.BackgroundImage")));
            this.ImageReset.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ImageReset.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ImageReset.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ImageReset.Location = new System.Drawing.Point(528, 231);
            this.ImageReset.Margin = new System.Windows.Forms.Padding(6, 4, 15, 8);
            this.ImageReset.Name = "ImageReset";
            this.ImageReset.Size = new System.Drawing.Size(29, 25);
            this.ImageReset.TabIndex = 7;
            this.ImageReset.TabStop = false;
            // 
            // LoadImage
            // 
            this.LoadImage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.LoadImage.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("LoadImage.BackgroundImage")));
            this.LoadImage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.LoadImage.Cursor = System.Windows.Forms.Cursors.Hand;
            this.LoadImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LoadImage.Location = new System.Drawing.Point(15, 231);
            this.LoadImage.Margin = new System.Windows.Forms.Padding(15, 4, 5, 7);
            this.LoadImage.Name = "LoadImage";
            this.LoadImage.Size = new System.Drawing.Size(30, 26);
            this.LoadImage.TabIndex = 4;
            this.LoadImage.TabStop = false;
            this.LoadImage.Click += new System.EventHandler(this.LoadImage_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.TLPanel_BGM1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupbox, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(572, 528);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // Form_MsaMeasureThick3D
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(572, 528);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "Form_MsaMeasureThick3D";
            this.Text = "Form_MsaMeasureThick3D";
            this.groupbox.ResumeLayout(false);
            this.groupbox.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.TLPanel_BGM1.ResumeLayout(false);
            this.TLPanel_BGM1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ImageReset)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LoadImage)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_Preview;
        private System.Windows.Forms.Button btn_Next;
        private System.Windows.Forms.Button btn_Run;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Button Btn_CreateData;
        private System.Windows.Forms.Label lb1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupbox;
        private System.Windows.Forms.ComboBox cbx_CamSelect;
        private System.Windows.Forms.ToolStripMenuItem 测量ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 工具ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 清除所有区域ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 删除指定区域ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 画线ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 画矩形1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 画圆ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 添加检测区域ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 画矩形2ToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        public System.Windows.Forms.Label LB_IMageSize;
        private System.Windows.Forms.TableLayoutPanel TLPanel_BGM1;
        public System.Windows.Forms.PictureBox ImageReset;
        public System.Windows.Forms.PictureBox LoadImage;
        public System.Windows.Forms.Label LB_ModleName;
        private System.Windows.Forms.TextBox TB_PositionAndGray;
        private HalconDotNet.HWindowControl HWC_Display;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox LineMethodSelect;
        private System.Windows.Forms.TextBox TB_PositionAndGray3D;
        private System.Windows.Forms.CheckBox cbx_CheckEnable;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tb_ProductFile;
        private System.Windows.Forms.TextBox tb_CheckFile;
        private System.Windows.Forms.Label label3;
    }
}
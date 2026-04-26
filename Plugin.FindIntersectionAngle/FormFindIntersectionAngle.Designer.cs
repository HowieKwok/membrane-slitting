namespace Plugin.FindIntersectionAngle
{
    partial class FormFindIntersectionAngle
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.mainPanel = new System.Windows.Forms.Panel();
            this.contentPanel = new System.Windows.Forms.Panel();
            this.imagePanel = new System.Windows.Forms.Panel();
            this.hWindow = new HalconDotNet.HWindowControl();
            this.profilePanel = new System.Windows.Forms.TableLayoutPanel();
            this.hProfileBox = new System.Windows.Forms.Panel();
            this.lblHProfile = new System.Windows.Forms.Label();
            this.panelHProfile = new System.Windows.Forms.Panel();
            this.vProfileBox = new System.Windows.Forms.Panel();
            this.lblVProfile = new System.Windows.Forms.Label();
            this.panelVProfile = new System.Windows.Forms.Panel();
            this.rightPanel = new System.Windows.Forms.Panel();
            this.tabParams = new System.Windows.Forms.TabControl();
            this.tabLine1 = new System.Windows.Forms.TabPage();
            this.line1Panel = new System.Windows.Forms.TableLayoutPanel();
            this.tabLine2 = new System.Windows.Forms.TabPage();
            this.line2Panel = new System.Windows.Forms.TableLayoutPanel();
            this.tabResult = new System.Windows.Forms.TabPage();
            this.resultPanel = new System.Windows.Forms.Panel();
            this.lblResultParallel = new System.Windows.Forms.Label();
            this.lblResultLine2 = new System.Windows.Forms.Label();
            this.lblResultLine1 = new System.Windows.Forms.Label();
            this.lblResultLine2Title = new System.Windows.Forms.Label();
            this.lblResultLine1Title = new System.Windows.Forms.Label();
            this.lblResultCross = new System.Windows.Forms.Label();
            this.lblResultAngle = new System.Windows.Forms.Label();
            this.topPanel = new System.Windows.Forms.Panel();
            this.lblMouse = new System.Windows.Forms.Label();
            this.btnCross = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblTopAngle = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.mainPanel.SuspendLayout();
            this.contentPanel.SuspendLayout();
            this.imagePanel.SuspendLayout();
            this.profilePanel.SuspendLayout();
            this.hProfileBox.SuspendLayout();
            this.vProfileBox.SuspendLayout();
            this.rightPanel.SuspendLayout();
            this.tabParams.SuspendLayout();
            this.tabLine1.SuspendLayout();
            this.tabLine2.SuspendLayout();
            this.tabResult.SuspendLayout();
            this.resultPanel.SuspendLayout();
            this.topPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainPanel
            // 
            this.mainPanel.BackColor = System.Drawing.Color.FromArgb(18, 22, 28);
            this.mainPanel.Controls.Add(this.contentPanel);
            this.mainPanel.Controls.Add(this.topPanel);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(0, 0);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Padding = new System.Windows.Forms.Padding(10);
            this.mainPanel.Size = new System.Drawing.Size(1180, 720);
            this.mainPanel.TabIndex = 0;
            // 
            // contentPanel
            // 
            this.contentPanel.Controls.Add(this.imagePanel);
            this.contentPanel.Controls.Add(this.rightPanel);
            this.contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentPanel.Location = new System.Drawing.Point(10, 62);
            this.contentPanel.Name = "contentPanel";
            this.contentPanel.Padding = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.contentPanel.Size = new System.Drawing.Size(1160, 648);
            this.contentPanel.TabIndex = 1;
            // 
            // imagePanel
            // 
            this.imagePanel.Controls.Add(this.hWindow);
            this.imagePanel.Controls.Add(this.profilePanel);
            this.imagePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imagePanel.Location = new System.Drawing.Point(0, 10);
            this.imagePanel.Name = "imagePanel";
            this.imagePanel.Padding = new System.Windows.Forms.Padding(0, 0, 10, 0);
            this.imagePanel.Size = new System.Drawing.Size(790, 638);
            this.imagePanel.TabIndex = 0;
            // 
            // hWindow
            // 
            this.hWindow.BackColor = System.Drawing.Color.Black;
            this.hWindow.BorderColor = System.Drawing.Color.FromArgb(0, 174, 239);
            this.hWindow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hWindow.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.hWindow.Location = new System.Drawing.Point(0, 0);
            this.hWindow.Name = "hWindow";
            this.hWindow.Size = new System.Drawing.Size(780, 418);
            this.hWindow.TabIndex = 0;
            this.hWindow.WindowSize = new System.Drawing.Size(780, 418);
            this.hWindow.HMouseMove += new HalconDotNet.HMouseEventHandler(this.HWindow_HMouseMove);
            this.hWindow.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.HWindow_MouseDoubleClick);
            // 
            // profilePanel
            // 
            this.profilePanel.ColumnCount = 1;
            this.profilePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.profilePanel.Controls.Add(this.hProfileBox, 0, 0);
            this.profilePanel.Controls.Add(this.vProfileBox, 0, 1);
            this.profilePanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.profilePanel.Location = new System.Drawing.Point(0, 418);
            this.profilePanel.Name = "profilePanel";
            this.profilePanel.RowCount = 2;
            this.profilePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.profilePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.profilePanel.Size = new System.Drawing.Size(780, 220);
            this.profilePanel.TabIndex = 1;
            // 
            // hProfileBox
            // 
            this.hProfileBox.BackColor = System.Drawing.Color.FromArgb(24, 29, 36);
            this.hProfileBox.Controls.Add(this.panelHProfile);
            this.hProfileBox.Controls.Add(this.lblHProfile);
            this.hProfileBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hProfileBox.Location = new System.Drawing.Point(0, 0);
            this.hProfileBox.Margin = new System.Windows.Forms.Padding(0, 0, 0, 6);
            this.hProfileBox.Name = "hProfileBox";
            this.hProfileBox.Padding = new System.Windows.Forms.Padding(8, 6, 8, 8);
            this.hProfileBox.Size = new System.Drawing.Size(780, 104);
            this.hProfileBox.TabIndex = 0;
            // 
            // lblHProfile
            // 
            this.lblHProfile.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblHProfile.ForeColor = System.Drawing.Color.FromArgb(126, 230, 255);
            this.lblHProfile.Location = new System.Drawing.Point(8, 6);
            this.lblHProfile.Name = "lblHProfile";
            this.lblHProfile.Size = new System.Drawing.Size(764, 20);
            this.lblHProfile.TabIndex = 0;
            this.lblHProfile.Text = "H 灰度曲线";
            // 
            // panelHProfile
            // 
            this.panelHProfile.BackColor = System.Drawing.Color.FromArgb(15, 18, 22);
            this.panelHProfile.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelHProfile.Location = new System.Drawing.Point(8, 26);
            this.panelHProfile.Name = "panelHProfile";
            this.panelHProfile.Size = new System.Drawing.Size(764, 70);
            this.panelHProfile.TabIndex = 1;
            this.panelHProfile.Paint += new System.Windows.Forms.PaintEventHandler(this.ProfilePaint);
            // 
            // vProfileBox
            // 
            this.vProfileBox.BackColor = System.Drawing.Color.FromArgb(24, 29, 36);
            this.vProfileBox.Controls.Add(this.panelVProfile);
            this.vProfileBox.Controls.Add(this.lblVProfile);
            this.vProfileBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.vProfileBox.Location = new System.Drawing.Point(0, 110);
            this.vProfileBox.Margin = new System.Windows.Forms.Padding(0);
            this.vProfileBox.Name = "vProfileBox";
            this.vProfileBox.Padding = new System.Windows.Forms.Padding(8, 6, 8, 8);
            this.vProfileBox.Size = new System.Drawing.Size(780, 110);
            this.vProfileBox.TabIndex = 1;
            // 
            // lblVProfile
            // 
            this.lblVProfile.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblVProfile.ForeColor = System.Drawing.Color.FromArgb(126, 230, 255);
            this.lblVProfile.Location = new System.Drawing.Point(8, 6);
            this.lblVProfile.Name = "lblVProfile";
            this.lblVProfile.Size = new System.Drawing.Size(764, 20);
            this.lblVProfile.TabIndex = 0;
            this.lblVProfile.Text = "V 灰度曲线";
            // 
            // panelVProfile
            // 
            this.panelVProfile.BackColor = System.Drawing.Color.FromArgb(15, 18, 22);
            this.panelVProfile.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelVProfile.Location = new System.Drawing.Point(8, 26);
            this.panelVProfile.Name = "panelVProfile";
            this.panelVProfile.Size = new System.Drawing.Size(764, 76);
            this.panelVProfile.TabIndex = 1;
            this.panelVProfile.Paint += new System.Windows.Forms.PaintEventHandler(this.ProfilePaint);
            // 
            // rightPanel
            // 
            this.rightPanel.BackColor = System.Drawing.Color.FromArgb(21, 25, 31);
            this.rightPanel.Controls.Add(this.tabParams);
            this.rightPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.rightPanel.Location = new System.Drawing.Point(790, 10);
            this.rightPanel.Name = "rightPanel";
            this.rightPanel.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.rightPanel.Size = new System.Drawing.Size(370, 638);
            this.rightPanel.TabIndex = 1;
            // 
            // tabParams
            // 
            this.tabParams.Controls.Add(this.tabLine1);
            this.tabParams.Controls.Add(this.tabLine2);
            this.tabParams.Controls.Add(this.tabResult);
            this.tabParams.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabParams.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Bold);
            this.tabParams.Location = new System.Drawing.Point(10, 0);
            this.tabParams.Name = "tabParams";
            this.tabParams.SelectedIndex = 0;
            this.tabParams.Size = new System.Drawing.Size(360, 638);
            this.tabParams.TabIndex = 0;
            // 
            // tabLine1
            // 
            this.tabLine1.BackColor = System.Drawing.Color.FromArgb(24, 29, 36);
            this.tabLine1.Controls.Add(this.line1Panel);
            this.tabLine1.ForeColor = System.Drawing.Color.White;
            this.tabLine1.Location = new System.Drawing.Point(4, 28);
            this.tabLine1.Name = "tabLine1";
            this.tabLine1.Padding = new System.Windows.Forms.Padding(12);
            this.tabLine1.Size = new System.Drawing.Size(352, 606);
            this.tabLine1.TabIndex = 0;
            this.tabLine1.Text = "线1参数";
            // 
            // line1Panel
            // 
            this.line1Panel.ColumnCount = 2;
            this.line1Panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 126F));
            this.line1Panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.line1Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.line1Panel.Location = new System.Drawing.Point(12, 12);
            this.line1Panel.Name = "line1Panel";
            this.line1Panel.RowCount = 1;
            this.line1Panel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.line1Panel.Size = new System.Drawing.Size(328, 582);
            this.line1Panel.TabIndex = 0;
            // 
            // tabLine2
            // 
            this.tabLine2.BackColor = System.Drawing.Color.FromArgb(24, 29, 36);
            this.tabLine2.Controls.Add(this.line2Panel);
            this.tabLine2.ForeColor = System.Drawing.Color.White;
            this.tabLine2.Location = new System.Drawing.Point(4, 28);
            this.tabLine2.Name = "tabLine2";
            this.tabLine2.Padding = new System.Windows.Forms.Padding(12);
            this.tabLine2.Size = new System.Drawing.Size(352, 606);
            this.tabLine2.TabIndex = 1;
            this.tabLine2.Text = "线2参数";
            // 
            // line2Panel
            // 
            this.line2Panel.ColumnCount = 2;
            this.line2Panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 126F));
            this.line2Panel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.line2Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.line2Panel.Location = new System.Drawing.Point(12, 12);
            this.line2Panel.Name = "line2Panel";
            this.line2Panel.RowCount = 1;
            this.line2Panel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.line2Panel.Size = new System.Drawing.Size(328, 582);
            this.line2Panel.TabIndex = 0;
            // 
            // tabResult
            // 
            this.tabResult.BackColor = System.Drawing.Color.FromArgb(24, 29, 36);
            this.tabResult.Controls.Add(this.resultPanel);
            this.tabResult.ForeColor = System.Drawing.Color.White;
            this.tabResult.Location = new System.Drawing.Point(4, 28);
            this.tabResult.Name = "tabResult";
            this.tabResult.Padding = new System.Windows.Forms.Padding(12);
            this.tabResult.Size = new System.Drawing.Size(352, 606);
            this.tabResult.TabIndex = 2;
            this.tabResult.Text = "结果输出";
            // 
            // resultPanel
            // 
            this.resultPanel.BackColor = System.Drawing.Color.FromArgb(18, 22, 28);
            this.resultPanel.Controls.Add(this.lblResultParallel);
            this.resultPanel.Controls.Add(this.lblResultLine2);
            this.resultPanel.Controls.Add(this.lblResultLine1);
            this.resultPanel.Controls.Add(this.lblResultLine2Title);
            this.resultPanel.Controls.Add(this.lblResultLine1Title);
            this.resultPanel.Controls.Add(this.lblResultCross);
            this.resultPanel.Controls.Add(this.lblResultAngle);
            this.resultPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.resultPanel.Location = new System.Drawing.Point(12, 12);
            this.resultPanel.Name = "resultPanel";
            this.resultPanel.Padding = new System.Windows.Forms.Padding(18);
            this.resultPanel.Size = new System.Drawing.Size(328, 582);
            this.resultPanel.TabIndex = 0;
            // 
            // result labels
            // 
            this.lblResultAngle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblResultAngle.Font = new System.Drawing.Font("Microsoft YaHei UI", 17F, System.Drawing.FontStyle.Bold);
            this.lblResultAngle.ForeColor = System.Drawing.Color.FromArgb(0, 220, 255);
            this.lblResultAngle.Location = new System.Drawing.Point(18, 18);
            this.lblResultAngle.Name = "lblResultAngle";
            this.lblResultAngle.Size = new System.Drawing.Size(292, 58);
            this.lblResultAngle.TabIndex = 0;
            this.lblResultAngle.Text = "两线夹角：--";
            this.lblResultCross.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblResultCross.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F);
            this.lblResultCross.ForeColor = System.Drawing.Color.White;
            this.lblResultCross.Location = new System.Drawing.Point(18, 76);
            this.lblResultCross.Name = "lblResultCross";
            this.lblResultCross.Size = new System.Drawing.Size(292, 42);
            this.lblResultCross.TabIndex = 1;
            this.lblResultCross.Text = "交点：--";
            this.lblResultLine1Title.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblResultLine1Title.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblResultLine1Title.ForeColor = System.Drawing.Color.FromArgb(126, 230, 255);
            this.lblResultLine1Title.Location = new System.Drawing.Point(18, 118);
            this.lblResultLine1Title.Name = "lblResultLine1Title";
            this.lblResultLine1Title.Size = new System.Drawing.Size(292, 32);
            this.lblResultLine1Title.TabIndex = 2;
            this.lblResultLine1Title.Text = "线1";
            this.lblResultLine1.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblResultLine1.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F);
            this.lblResultLine1.ForeColor = System.Drawing.Color.Gainsboro;
            this.lblResultLine1.Location = new System.Drawing.Point(18, 150);
            this.lblResultLine1.Name = "lblResultLine1";
            this.lblResultLine1.Size = new System.Drawing.Size(292, 42);
            this.lblResultLine1.TabIndex = 3;
            this.lblResultLine1.Text = "线1拟合：未完成";
            this.lblResultLine2Title.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblResultLine2Title.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblResultLine2Title.ForeColor = System.Drawing.Color.FromArgb(126, 230, 255);
            this.lblResultLine2Title.Location = new System.Drawing.Point(18, 192);
            this.lblResultLine2Title.Name = "lblResultLine2Title";
            this.lblResultLine2Title.Size = new System.Drawing.Size(292, 32);
            this.lblResultLine2Title.TabIndex = 4;
            this.lblResultLine2Title.Text = "线2";
            this.lblResultLine2.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblResultLine2.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F);
            this.lblResultLine2.ForeColor = System.Drawing.Color.Gainsboro;
            this.lblResultLine2.Location = new System.Drawing.Point(18, 224);
            this.lblResultLine2.Name = "lblResultLine2";
            this.lblResultLine2.Size = new System.Drawing.Size(292, 42);
            this.lblResultLine2.TabIndex = 5;
            this.lblResultLine2.Text = "线2拟合：未完成";
            this.lblResultParallel.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblResultParallel.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F);
            this.lblResultParallel.ForeColor = System.Drawing.Color.Gainsboro;
            this.lblResultParallel.Location = new System.Drawing.Point(18, 266);
            this.lblResultParallel.Name = "lblResultParallel";
            this.lblResultParallel.Size = new System.Drawing.Size(292, 42);
            this.lblResultParallel.TabIndex = 6;
            this.lblResultParallel.Text = "是否平行：--";
            // 
            // topPanel
            // 
            this.topPanel.BackColor = System.Drawing.Color.FromArgb(15, 18, 24);
            this.topPanel.Controls.Add(this.lblMouse);
            this.topPanel.Controls.Add(this.btnCross);
            this.topPanel.Controls.Add(this.btnReset);
            this.topPanel.Controls.Add(this.btnLoad);
            this.topPanel.Controls.Add(this.lblStatus);
            this.topPanel.Controls.Add(this.lblTopAngle);
            this.topPanel.Controls.Add(this.lblTitle);
            this.topPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.topPanel.Location = new System.Drawing.Point(10, 10);
            this.topPanel.Name = "topPanel";
            this.topPanel.Size = new System.Drawing.Size(1160, 52);
            this.topPanel.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft YaHei UI", 13F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(0, 220, 255);
            this.lblTitle.Location = new System.Drawing.Point(0, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Padding = new System.Windows.Forms.Padding(12, 0, 0, 0);
            this.lblTitle.Size = new System.Drawing.Size(190, 52);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "交点角度";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblTopAngle
            // 
            this.lblTopAngle.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblTopAngle.Font = new System.Drawing.Font("Microsoft YaHei UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblTopAngle.ForeColor = System.Drawing.Color.White;
            this.lblTopAngle.Location = new System.Drawing.Point(190, 0);
            this.lblTopAngle.Name = "lblTopAngle";
            this.lblTopAngle.Size = new System.Drawing.Size(210, 52);
            this.lblTopAngle.TabIndex = 1;
            this.lblTopAngle.Text = "当前夹角：-- 度";
            this.lblTopAngle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblStatus
            // 
            this.lblStatus.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblStatus.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.lblStatus.ForeColor = System.Drawing.Color.Gainsboro;
            this.lblStatus.Location = new System.Drawing.Point(400, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(150, 52);
            this.lblStatus.TabIndex = 2;
            this.lblStatus.Text = "状态：等待执行";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnLoad
            // 
            this.btnLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLoad.BackColor = System.Drawing.Color.FromArgb(31, 40, 52);
            this.btnLoad.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(0, 174, 239);
            this.btnLoad.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLoad.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnLoad.ForeColor = System.Drawing.Color.White;
            this.btnLoad.Location = new System.Drawing.Point(1000, 8);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(42, 36);
            this.btnLoad.TabIndex = 3;
            this.btnLoad.Text = "↥";
            this.btnLoad.UseVisualStyleBackColor = false;
            this.btnLoad.Click += new System.EventHandler(this.BtnLoad_Click);
            // 
            // btnReset
            // 
            this.btnReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReset.BackColor = System.Drawing.Color.FromArgb(31, 40, 52);
            this.btnReset.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(0, 174, 239);
            this.btnReset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReset.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnReset.ForeColor = System.Drawing.Color.White;
            this.btnReset.Location = new System.Drawing.Point(1050, 8);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(42, 36);
            this.btnReset.TabIndex = 4;
            this.btnReset.Text = "⟳";
            this.btnReset.UseVisualStyleBackColor = false;
            this.btnReset.Click += new System.EventHandler(this.BtnReset_Click);
            // 
            // btnCross
            // 
            this.btnCross.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCross.BackColor = System.Drawing.Color.FromArgb(0, 120, 165);
            this.btnCross.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(126, 230, 255);
            this.btnCross.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCross.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnCross.ForeColor = System.Drawing.Color.White;
            this.btnCross.Location = new System.Drawing.Point(1100, 8);
            this.btnCross.Name = "btnCross";
            this.btnCross.Size = new System.Drawing.Size(42, 36);
            this.btnCross.TabIndex = 5;
            this.btnCross.Text = "＋";
            this.btnCross.UseVisualStyleBackColor = false;
            this.btnCross.Click += new System.EventHandler(this.BtnCross_Click);
            // 
            // lblMouse
            // 
            this.lblMouse.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMouse.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.lblMouse.ForeColor = System.Drawing.Color.FromArgb(126, 230, 255);
            this.lblMouse.Location = new System.Drawing.Point(555, 0);
            this.lblMouse.Name = "lblMouse";
            this.lblMouse.Size = new System.Drawing.Size(420, 52);
            this.lblMouse.TabIndex = 6;
            this.lblMouse.Text = "图像：--    鼠标：R-- C-- 灰度--";
            this.lblMouse.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // FormFindIntersectionAngle
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(18, 22, 28);
            this.ClientSize = new System.Drawing.Size(1180, 720);
            this.Controls.Add(this.mainPanel);
            this.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.MinimumSize = new System.Drawing.Size(980, 680);
            this.Name = "FormFindIntersectionAngle";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "交点角度";
            this.mainPanel.ResumeLayout(false);
            this.contentPanel.ResumeLayout(false);
            this.imagePanel.ResumeLayout(false);
            this.profilePanel.ResumeLayout(false);
            this.hProfileBox.ResumeLayout(false);
            this.vProfileBox.ResumeLayout(false);
            this.rightPanel.ResumeLayout(false);
            this.tabParams.ResumeLayout(false);
            this.tabLine1.ResumeLayout(false);
            this.tabLine2.ResumeLayout(false);
            this.tabResult.ResumeLayout(false);
            this.resultPanel.ResumeLayout(false);
            this.topPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private void BuildLinePage(System.Windows.Forms.TableLayoutPanel panel, bool line1)
        {
            panel.SuspendLayout();
            panel.Controls.Clear();
            panel.RowStyles.Clear();
            panel.RowCount = 0;

            AddActionButton(panel, line1 ? "绘制线1 ROI" : "绘制线2 ROI", line1 ? new System.EventHandler(this.BtnLine1Roi_Click) : new System.EventHandler(this.BtnLine2Roi_Click));
            AddSection(panel, "ROI 参数");
            if (line1)
            {
                txtL1Row = AddText(panel, "中心 R");
                txtL1Col = AddText(panel, "中心 C");
                txtL1Angle = AddText(panel, "角度");
                txtL1Height = AddText(panel, "高度");
                txtL1Width = AddText(panel, "宽度");
                txtL1Points = AddText(panel, "采样点数");
                AddSection(panel, "抓边参数");
                txtL1Sigma = AddText(panel, "Sigma");
                txtL1Threshold = AddText(panel, "阈值");
                txtL1NumInstances = AddText(panel, "边数量");
                txtL1DistThreshold = AddText(panel, "距离阈值");
                txtL1MinScore = AddText(panel, "最小得分");
                txtL1Iterations = AddText(panel, "迭代次数");
                cmbL1Transition = AddCombo(panel, "边缘极性", new object[] { "all", "positive", "negative", "uniform" });
                cmbL1Select = AddCombo(panel, "选择方式", new object[] { "first", "last", "all" });
                cmbL1Interpolation = AddCombo(panel, "插值方式", new object[] { "nearest_neighbor", "bilinear", "bicubic" });
            }
            else
            {
                txtL2Row = AddText(panel, "中心 R");
                txtL2Col = AddText(panel, "中心 C");
                txtL2Angle = AddText(panel, "角度");
                txtL2Height = AddText(panel, "高度");
                txtL2Width = AddText(panel, "宽度");
                txtL2Points = AddText(panel, "采样点数");
                AddSection(panel, "抓边参数");
                txtL2Sigma = AddText(panel, "Sigma");
                txtL2Threshold = AddText(panel, "阈值");
                txtL2NumInstances = AddText(panel, "边数量");
                txtL2DistThreshold = AddText(panel, "距离阈值");
                txtL2MinScore = AddText(panel, "最小得分");
                txtL2Iterations = AddText(panel, "迭代次数");
                cmbL2Transition = AddCombo(panel, "边缘极性", new object[] { "all", "positive", "negative", "uniform" });
                cmbL2Select = AddCombo(panel, "选择方式", new object[] { "first", "last", "all" });
                cmbL2Interpolation = AddCombo(panel, "插值方式", new object[] { "nearest_neighbor", "bilinear", "bicubic" });
            }

            panel.RowCount++;
            panel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            System.Windows.Forms.Panel filler = new System.Windows.Forms.Panel();
            filler.Dock = System.Windows.Forms.DockStyle.Fill;
            panel.Controls.Add(filler, 0, panel.RowCount - 1);
            panel.SetColumnSpan(filler, 2);
            panel.ResumeLayout(false);
        }

        private void AddActionButton(System.Windows.Forms.TableLayoutPanel panel, string text, System.EventHandler handler)
        {
            panel.RowCount++;
            panel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            System.Windows.Forms.Button button = new System.Windows.Forms.Button();
            button.BackColor = System.Drawing.Color.FromArgb(0, 120, 165);
            button.Dock = System.Windows.Forms.DockStyle.Fill;
            button.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(126, 230, 255);
            button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            button.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Bold);
            button.ForeColor = System.Drawing.Color.White;
            button.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);
            button.Text = text;
            button.UseVisualStyleBackColor = false;
            button.Click += handler;
            panel.Controls.Add(button, 0, panel.RowCount - 1);
            panel.SetColumnSpan(button, 2);
        }

        private void AddSection(System.Windows.Forms.TableLayoutPanel panel, string text)
        {
            panel.RowCount++;
            panel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            System.Windows.Forms.Label label = new System.Windows.Forms.Label();
            label.Dock = System.Windows.Forms.DockStyle.Fill;
            label.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Bold);
            label.ForeColor = System.Drawing.Color.FromArgb(0, 220, 255);
            label.Text = text;
            label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            panel.Controls.Add(label, 0, panel.RowCount - 1);
            panel.SetColumnSpan(label, 2);
        }

        private System.Windows.Forms.TextBox AddText(System.Windows.Forms.TableLayoutPanel panel, string labelText)
        {
            panel.RowCount++;
            panel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            System.Windows.Forms.Label label = BuildFieldLabel(labelText);
            System.Windows.Forms.TextBox textBox = new System.Windows.Forms.TextBox();
            textBox.BackColor = System.Drawing.Color.FromArgb(12, 15, 20);
            textBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            textBox.Dock = System.Windows.Forms.DockStyle.Fill;
            textBox.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            textBox.ForeColor = System.Drawing.Color.White;
            textBox.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            panel.Controls.Add(label, 0, panel.RowCount - 1);
            panel.Controls.Add(textBox, 1, panel.RowCount - 1);
            return textBox;
        }

        private System.Windows.Forms.ComboBox AddCombo(System.Windows.Forms.TableLayoutPanel panel, string labelText, object[] items)
        {
            panel.RowCount++;
            panel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            System.Windows.Forms.Label label = BuildFieldLabel(labelText);
            System.Windows.Forms.ComboBox comboBox = new System.Windows.Forms.ComboBox();
            comboBox.BackColor = System.Drawing.Color.FromArgb(12, 15, 20);
            comboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            comboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            comboBox.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            comboBox.ForeColor = System.Drawing.Color.White;
            comboBox.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            comboBox.Items.AddRange(items);
            panel.Controls.Add(label, 0, panel.RowCount - 1);
            panel.Controls.Add(comboBox, 1, panel.RowCount - 1);
            return comboBox;
        }

        private System.Windows.Forms.Label BuildFieldLabel(string text)
        {
            System.Windows.Forms.Label label = new System.Windows.Forms.Label();
            label.Dock = System.Windows.Forms.DockStyle.Fill;
            label.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            label.ForeColor = System.Drawing.Color.Gainsboro;
            label.Margin = new System.Windows.Forms.Padding(0);
            label.Text = text;
            label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            return label;
        }

        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.Panel contentPanel;
        private System.Windows.Forms.Panel imagePanel;
        private HalconDotNet.HWindowControl hWindow;
        private System.Windows.Forms.TableLayoutPanel profilePanel;
        private System.Windows.Forms.Panel hProfileBox;
        private System.Windows.Forms.Label lblHProfile;
        private System.Windows.Forms.Panel panelHProfile;
        private System.Windows.Forms.Panel vProfileBox;
        private System.Windows.Forms.Label lblVProfile;
        private System.Windows.Forms.Panel panelVProfile;
        private System.Windows.Forms.Panel rightPanel;
        private System.Windows.Forms.TabControl tabParams;
        private System.Windows.Forms.TabPage tabLine1;
        private System.Windows.Forms.TabPage tabLine2;
        private System.Windows.Forms.TabPage tabResult;
        private System.Windows.Forms.TableLayoutPanel line1Panel;
        private System.Windows.Forms.TableLayoutPanel line2Panel;
        private System.Windows.Forms.Panel resultPanel;
        private System.Windows.Forms.Label lblResultAngle;
        private System.Windows.Forms.Label lblResultCross;
        private System.Windows.Forms.Label lblResultLine1Title;
        private System.Windows.Forms.Label lblResultLine1;
        private System.Windows.Forms.Label lblResultLine2Title;
        private System.Windows.Forms.Label lblResultLine2;
        private System.Windows.Forms.Label lblResultParallel;
        private System.Windows.Forms.Panel topPanel;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblTopAngle;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnCross;
        private System.Windows.Forms.Label lblMouse;
        private System.Windows.Forms.TextBox txtL1Row;
        private System.Windows.Forms.TextBox txtL1Col;
        private System.Windows.Forms.TextBox txtL1Angle;
        private System.Windows.Forms.TextBox txtL1Height;
        private System.Windows.Forms.TextBox txtL1Width;
        private System.Windows.Forms.TextBox txtL1Points;
        private System.Windows.Forms.TextBox txtL1Sigma;
        private System.Windows.Forms.TextBox txtL1Threshold;
        private System.Windows.Forms.TextBox txtL1NumInstances;
        private System.Windows.Forms.TextBox txtL1DistThreshold;
        private System.Windows.Forms.TextBox txtL1MinScore;
        private System.Windows.Forms.TextBox txtL1Iterations;
        private System.Windows.Forms.TextBox txtL2Row;
        private System.Windows.Forms.TextBox txtL2Col;
        private System.Windows.Forms.TextBox txtL2Angle;
        private System.Windows.Forms.TextBox txtL2Height;
        private System.Windows.Forms.TextBox txtL2Width;
        private System.Windows.Forms.TextBox txtL2Points;
        private System.Windows.Forms.TextBox txtL2Sigma;
        private System.Windows.Forms.TextBox txtL2Threshold;
        private System.Windows.Forms.TextBox txtL2NumInstances;
        private System.Windows.Forms.TextBox txtL2DistThreshold;
        private System.Windows.Forms.TextBox txtL2MinScore;
        private System.Windows.Forms.TextBox txtL2Iterations;
        private System.Windows.Forms.ComboBox cmbL1Transition;
        private System.Windows.Forms.ComboBox cmbL1Select;
        private System.Windows.Forms.ComboBox cmbL1Interpolation;
        private System.Windows.Forms.ComboBox cmbL2Transition;
        private System.Windows.Forms.ComboBox cmbL2Select;
        private System.Windows.Forms.ComboBox cmbL2Interpolation;
    }
}

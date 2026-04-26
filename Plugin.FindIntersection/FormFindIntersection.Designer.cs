namespace Plugin.FindIntersection
{
    partial class FormFindIntersection
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

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            // ===== Color Palette =====
            System.Drawing.Color clrBg = System.Drawing.Color.FromArgb(30, 30, 30);
            System.Drawing.Color clrPanel = System.Drawing.Color.FromArgb(45, 45, 48);
            System.Drawing.Color clrCard = System.Drawing.Color.FromArgb(55, 55, 58);
            System.Drawing.Color clrAccent = System.Drawing.Color.FromArgb(0, 122, 204);
            System.Drawing.Color clrText = System.Drawing.Color.FromArgb(220, 220, 220);
            System.Drawing.Color clrTextDim = System.Drawing.Color.FromArgb(160, 160, 160);
            System.Drawing.Color clrBorder = System.Drawing.Color.FromArgb(70, 70, 74);
            System.Drawing.Color clrCtrlBg = System.Drawing.Color.FromArgb(60, 60, 64);

            _clrBg = clrBg; _clrPanel = clrPanel; _clrAccent = clrAccent;
            _clrText = clrText; _clrTextDim = clrTextDim; _clrBorder = clrBorder;

            // ===== Fonts =====
            System.Drawing.Font fontTitle = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Bold);
            System.Drawing.Font fontNormal = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            System.Drawing.Font fontSmall = new System.Drawing.Font("Microsoft YaHei UI", 8.5F);
            System.Drawing.Font fontMono = new System.Drawing.Font("Consolas", 9F);
            System.Drawing.Font fontLabel = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            System.Drawing.Font fontNUD = new System.Drawing.Font("Microsoft YaHei UI", 9.5F);

            // ===== Instantiate controls =====
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.LB_ModleName = new System.Windows.Forms.Label();
            this.LB_IMageSize = new System.Windows.Forms.Label();
            this.HWC_Display = new HalconDotNet.HWindowControl();
            this.LoadImage = new System.Windows.Forms.PictureBox();
            this.TB_PositionAndGray = new System.Windows.Forms.TextBox();
            this.FullScrean = new System.Windows.Forms.PictureBox();
            //this.Tb_CostTime = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.panelChartContainer = new System.Windows.Forms.TableLayoutPanel();
            this.panelHProfile = new System.Windows.Forms.Panel();
            this.panelVProfile = new System.Windows.Forms.Panel();
            this.btnCrosshair = new System.Windows.Forms.Button();

            this.tabPage_ROI1 = new System.Windows.Forms.TabPage();
            this.tlp_ROI1 = new System.Windows.Forms.TableLayoutPanel();
            this.lbl_Num_L1 = new System.Windows.Forms.Label();
            this.lbl_Width_L1 = new System.Windows.Forms.Label();
            this.lbl_Height_L1 = new System.Windows.Forms.Label();
            this.lbl_Angle_L1 = new System.Windows.Forms.Label();
            this.lbl_MidRow_L1 = new System.Windows.Forms.Label();
            this.lbl_MidCol_L1 = new System.Windows.Forms.Label();
            this.NUD_RoiNum_L1 = new System.Windows.Forms.NumericUpDown();
            this.NUD_RoiWidth_L1 = new System.Windows.Forms.NumericUpDown();
            this.NUD_RoiHeight_L1 = new System.Windows.Forms.NumericUpDown();
            this.NUD_RoiAngle_L1 = new System.Windows.Forms.NumericUpDown();
            this.NUD_RoiMidRow_L1 = new System.Windows.Forms.NumericUpDown();
            this.NUD_RoiMidColumn_L1 = new System.Windows.Forms.NumericUpDown();

            this.tabPage_ROI2 = new System.Windows.Forms.TabPage();
            this.tlp_ROI2 = new System.Windows.Forms.TableLayoutPanel();
            this.lbl_Num_L2 = new System.Windows.Forms.Label();
            this.lbl_Width_L2 = new System.Windows.Forms.Label();
            this.lbl_Height_L2 = new System.Windows.Forms.Label();
            this.lbl_Angle_L2 = new System.Windows.Forms.Label();
            this.lbl_MidRow_L2 = new System.Windows.Forms.Label();
            this.lbl_MidCol_L2 = new System.Windows.Forms.Label();
            this.NUD_RoiNum_L2 = new System.Windows.Forms.NumericUpDown();
            this.NUD_RoiWidth_L2 = new System.Windows.Forms.NumericUpDown();
            this.NUD_RoiHeight_L2 = new System.Windows.Forms.NumericUpDown();
            this.NUD_RoiAngle_L2 = new System.Windows.Forms.NumericUpDown();
            this.NUD_RoiMidRow_L2 = new System.Windows.Forms.NumericUpDown();
            this.NUD_RoiMidColumn_L2 = new System.Windows.Forms.NumericUpDown();

            this.tabPage_Measure1 = new System.Windows.Forms.TabPage();
            this.panel_Measure1 = new System.Windows.Forms.Panel();
            this.lbl_L1_Sigma = new System.Windows.Forms.Label();
            this.lbl_L1_Threshold = new System.Windows.Forms.Label();
            this.lbl_L1_Transition = new System.Windows.Forms.Label();
            this.lbl_L1_Select = new System.Windows.Forms.Label();
            this.lbl_L1_MinScore = new System.Windows.Forms.Label();
            this.lbl_L1_NumInst = new System.Windows.Forms.Label();
            this.lbl_L1_DistThresh = new System.Windows.Forms.Label();
            this.lbl_L1_MaxIter = new System.Windows.Forms.Label();
            this.lbl_L1_Interpolation = new System.Windows.Forms.Label();
            this.lbl_L1_IORegions = new System.Windows.Forms.Label();
            this.NUD_L1_Sigma = new System.Windows.Forms.NumericUpDown();
            this.NUD_L1_Threshold = new System.Windows.Forms.NumericUpDown();
            this.CB_L1_Transition = new System.Windows.Forms.ComboBox();
            this.CB_L1_Select = new System.Windows.Forms.ComboBox();
            this.NUD_L1_MinScore = new System.Windows.Forms.NumericUpDown();
            this.NUD_L1_NumInstances = new System.Windows.Forms.NumericUpDown();
            this.NUD_L1_DistThreshold = new System.Windows.Forms.NumericUpDown();
            this.NUD_L1_MaxIterations = new System.Windows.Forms.NumericUpDown();
            this.CB_L1_Interpolation = new System.Windows.Forms.ComboBox();
            this.CB_L1_IORegions = new System.Windows.Forms.ComboBox();

            this.tabPage_Measure2 = new System.Windows.Forms.TabPage();
            this.panel_Measure2 = new System.Windows.Forms.Panel();
            this.lbl_L2_Sigma = new System.Windows.Forms.Label();
            this.lbl_L2_Threshold = new System.Windows.Forms.Label();
            this.lbl_L2_Transition = new System.Windows.Forms.Label();
            this.lbl_L2_Select = new System.Windows.Forms.Label();
            this.lbl_L2_MinScore = new System.Windows.Forms.Label();
            this.lbl_L2_NumInst = new System.Windows.Forms.Label();
            this.lbl_L2_DistThresh = new System.Windows.Forms.Label();
            this.lbl_L2_MaxIter = new System.Windows.Forms.Label();
            this.lbl_L2_Interpolation = new System.Windows.Forms.Label();
            this.lbl_L2_IORegions = new System.Windows.Forms.Label();
            this.NUD_L2_Sigma = new System.Windows.Forms.NumericUpDown();
            this.NUD_L2_Threshold = new System.Windows.Forms.NumericUpDown();
            this.CB_L2_Transition = new System.Windows.Forms.ComboBox();
            this.CB_L2_Select = new System.Windows.Forms.ComboBox();
            this.NUD_L2_MinScore = new System.Windows.Forms.NumericUpDown();
            this.NUD_L2_NumInstances = new System.Windows.Forms.NumericUpDown();
            this.NUD_L2_DistThreshold = new System.Windows.Forms.NumericUpDown();
            this.NUD_L2_MaxIterations = new System.Windows.Forms.NumericUpDown();
            this.CB_L2_Interpolation = new System.Windows.Forms.ComboBox();
            this.CB_L2_IORegions = new System.Windows.Forms.ComboBox();

            this.tabPage_Result = new System.Windows.Forms.TabPage();
            this.panel_Result = new System.Windows.Forms.Panel();
            this.lbl_ResultTitle = new System.Windows.Forms.Label();
            this.LB_Line1Success = new System.Windows.Forms.Label();
            this.LB_Line2Success = new System.Windows.Forms.Label();
            this.LB_ResultRow = new System.Windows.Forms.Label();
            this.LB_ResultCol = new System.Windows.Forms.Label();
            this.LB_ResultAngle = new System.Windows.Forms.Label();
            this.LB_IsParallel = new System.Windows.Forms.Label();
            this.LB_Line1Info = new System.Windows.Forms.Label();
            this.LB_Line2Info = new System.Windows.Forms.Label();

            this.timer1 = new System.Windows.Forms.Timer(this.components);

            // ===== SuspendLayout =====
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.panelChartContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LoadImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.FullScrean)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage_ROI1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_RoiNum_L1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_RoiWidth_L1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_RoiHeight_L1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_RoiAngle_L1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_RoiMidRow_L1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_RoiMidColumn_L1)).BeginInit();
            this.tabPage_ROI2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_RoiNum_L2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_RoiWidth_L2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_RoiHeight_L2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_RoiAngle_L2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_RoiMidRow_L2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_RoiMidColumn_L2)).BeginInit();
            this.tabPage_Measure1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_L1_Sigma)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_L1_Threshold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_L1_MinScore)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_L1_NumInstances)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_L1_DistThreshold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_L1_MaxIterations)).BeginInit();
            this.tabPage_Measure2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_L2_Sigma)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_L2_Threshold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_L2_MinScore)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_L2_NumInstances)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_L2_DistThreshold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_L2_MaxIterations)).BeginInit();
            this.tabPage_Result.SuspendLayout();
            this.SuspendLayout();

            // ===== tableLayoutPanel2 (root: 65% display | 35% params) =====
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 65F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35F));
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.BackColor = clrBg;
            this.tableLayoutPanel2.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.None;
            this.tableLayoutPanel2.Padding = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.groupBox2, 1, 0);

            // ===== tableLayoutPanel3 (display: 8 cols x 4 rows) =====
            this.tableLayoutPanel3.ColumnCount = 8;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 6F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 0.01F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 7F));
            this.tableLayoutPanel3.RowCount = 4;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 300F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.BackColor = clrBg;
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel3.Padding = new System.Windows.Forms.Padding(4);

            this.tableLayoutPanel3.Controls.Add(this.LB_ModleName, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.LB_IMageSize, 2, 0);
            this.tableLayoutPanel3.Controls.Add(this.LoadImage, 3, 0);
            this.tableLayoutPanel3.Controls.Add(this.FullScrean, 4, 0);
            this.tableLayoutPanel3.Controls.Add(this.btnCrosshair, 6, 0);
            this.tableLayoutPanel3.Controls.Add(this.HWC_Display, 0, 1);
            this.tableLayoutPanel3.SetColumnSpan(this.HWC_Display, 8);
            this.tableLayoutPanel3.Controls.Add(this.panelChartContainer, 0, 2);
            this.tableLayoutPanel3.SetColumnSpan(this.panelChartContainer, 8);
            this.tableLayoutPanel3.Controls.Add(this.TB_PositionAndGray, 1, 3);
            this.tableLayoutPanel3.SetColumnSpan(this.TB_PositionAndGray, 3);
            //this.tableLayoutPanel3.Controls.Add(this.Tb_CostTime, 5, 3);
            //this.tableLayoutPanel3.SetColumnSpan(this.Tb_CostTime, 2);

            // ===== LB_ModleName =====
            this.LB_ModleName.Name = "LB_ModleName";
            this.LB_ModleName.Text = "Model";
            this.LB_ModleName.Font = fontTitle;
            this.LB_ModleName.ForeColor = clrAccent;
            this.LB_ModleName.BackColor = System.Drawing.Color.Transparent;
            this.LB_ModleName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LB_ModleName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.LB_ModleName.Padding = new System.Windows.Forms.Padding(4, 0, 0, 0);

            // ===== LB_IMageSize =====
            this.LB_IMageSize.Name = "LB_IMageSize";
            this.LB_IMageSize.Text = "Size  R:--   C:--";
            this.LB_IMageSize.Font = fontSmall;
            this.LB_IMageSize.ForeColor = clrTextDim;
            this.LB_IMageSize.BackColor = System.Drawing.Color.Transparent;
            this.LB_IMageSize.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LB_IMageSize.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // ===== HWC_Display =====
            this.HWC_Display.BackColor = System.Drawing.Color.Black;
            this.HWC_Display.Dock = System.Windows.Forms.DockStyle.Fill;
            this.HWC_Display.Margin = new System.Windows.Forms.Padding(2);
            this.HWC_Display.Name = "HWC_Display";
            this.HWC_Display.TabIndex = 0;

            // ===== LoadImage =====
            this.LoadImage.Name = "LoadImage";
            this.LoadImage.Size = new System.Drawing.Size(25, 25);
            this.LoadImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.LoadImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LoadImage.Cursor = System.Windows.Forms.Cursors.Hand;
            this.LoadImage.BackColor = clrAccent;
            this.LoadImage.TabStop = false;
            this.LoadImage.Click += new System.EventHandler(this.LoadImage_Click);

            // ===== FullScrean =====
            this.FullScrean.Name = "FullScrean";
            this.FullScrean.Size = new System.Drawing.Size(25, 25);
            this.FullScrean.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.FullScrean.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FullScrean.Cursor = System.Windows.Forms.Cursors.Hand;
            this.FullScrean.BackColor = clrAccent;
            this.FullScrean.TabStop = false;
            this.FullScrean.Click += new System.EventHandler(this.FullScrean_Click);

            // ===== btnCrosshair (toggle crosshair visibility) =====
            this.btnCrosshair.Name = "btnCrosshair";
            this.btnCrosshair.Size = new System.Drawing.Size(28, 28);
            this.btnCrosshair.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnCrosshair.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCrosshair.FlatAppearance.BorderSize = 0;
            this.btnCrosshair.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(60, 60, 64);
            this.btnCrosshair.FlatAppearance.MouseDownBackColor = clrAccent;
            this.btnCrosshair.BackColor = clrPanel;
            this.btnCrosshair.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCrosshair.TabStop = false;
            this.btnCrosshair.Text = "";
            this.btnCrosshair.Click += new System.EventHandler(this.btnCrosshair_Click);
            this.btnCrosshair.Paint += new System.Windows.Forms.PaintEventHandler(this.btnCrosshair_Paint);

            // ===== TB_PositionAndGray =====
            this.TB_PositionAndGray.Name = "TB_PositionAndGray";
            this.TB_PositionAndGray.ReadOnly = true;
            this.TB_PositionAndGray.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TB_PositionAndGray.BackColor = clrBg;
            this.TB_PositionAndGray.ForeColor = clrTextDim;
            this.TB_PositionAndGray.Font = fontMono;
            this.TB_PositionAndGray.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TB_PositionAndGray.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.TB_PositionAndGray.Margin = new System.Windows.Forms.Padding(4, 6, 0, 0);

            // ===== Tb_CostTime =====
            //this.Tb_CostTime.Name = "Tb_CostTime";
            //this.Tb_CostTime.Text = "";
            //this.Tb_CostTime.Font = fontSmall;
            //this.Tb_CostTime.ForeColor = clrTextDim;
            //this.Tb_CostTime.BackColor = System.Drawing.Color.Transparent;
            //this.Tb_CostTime.Dock = System.Windows.Forms.DockStyle.Fill;
            //this.Tb_CostTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //this.Tb_CostTime.Padding = new System.Windows.Forms.Padding(0, 0, 8, 0);

            // ===== panelChartContainer =====
            this.panelChartContainer.ColumnCount = 2;
            this.panelChartContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.panelChartContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.panelChartContainer.RowCount = 1;
            this.panelChartContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.panelChartContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelChartContainer.BackColor = clrBg;
            this.panelChartContainer.Margin = new System.Windows.Forms.Padding(2, 2, 2, 0);
            this.panelChartContainer.Name = "panelChartContainer";
            this.panelChartContainer.Controls.Add(this.panelHProfile, 0, 0);
            this.panelChartContainer.Controls.Add(this.panelVProfile, 1, 0);

            // panelHProfile
            this.panelHProfile.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelHProfile.BackColor = System.Drawing.Color.FromArgb(30, 30, 33);
            this.panelHProfile.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.panelHProfile.Name = "panelHProfile";
            this.panelHProfile.Paint += new System.Windows.Forms.PaintEventHandler(this.PanelHProfile_Paint);

            // panelVProfile
            this.panelVProfile.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelVProfile.BackColor = System.Drawing.Color.FromArgb(30, 30, 33);
            this.panelVProfile.Margin = new System.Windows.Forms.Padding(2, 0, 0, 0);
            this.panelVProfile.Name = "panelVProfile";
            this.panelVProfile.Paint += new System.Windows.Forms.PaintEventHandler(this.PanelVProfile_Paint);

            // ===== groupBox2 =====
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Text = "";
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.BackColor = clrPanel;
            this.groupBox2.ForeColor = clrText;
            this.groupBox2.Font = fontNormal;
            this.groupBox2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.groupBox2.Padding = new System.Windows.Forms.Padding(6, 8, 6, 6);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2, 0, 0, 0);
            this.groupBox2.Controls.Add(this.tabControl1);


            // ===== tabControl1 (vertical Left alignment) =====
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Font = fontNormal;
            this.tabControl1.Margin = new System.Windows.Forms.Padding(0);
            this.tabControl1.Alignment = System.Windows.Forms.TabAlignment.Left;
            this.tabControl1.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tabControl1.ItemSize = new System.Drawing.Size(30, 100);
            this.tabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControl1.Multiline = true;
            this.tabControl1.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.tabControl1_DrawItem);
            this.tabControl1.TabPages.Add(this.tabPage_ROI1);
            this.tabControl1.TabPages.Add(this.tabPage_ROI2);
            this.tabControl1.TabPages.Add(this.tabPage_Measure1);
            this.tabControl1.TabPages.Add(this.tabPage_Measure2);
            this.tabControl1.TabPages.Add(this.tabPage_Result);


            // ===== ROI Tab Pages =====
            this.tabPage_ROI1.Name = "tabPage_ROI1";
            this.tabPage_ROI1.Text = "\u76f4\u7ebf1 ROI";
            this.tabPage_ROI1.BackColor = clrPanel;
            this.tabPage_ROI1.ForeColor = clrText;
            this.tabPage_ROI1.Padding = new System.Windows.Forms.Padding(6);
            this.tabPage_ROI1.Controls.Add(this.tlp_ROI1);

            this.tabPage_ROI2.Name = "tabPage_ROI2";
            this.tabPage_ROI2.Text = "\u76f4\u7ebf2 ROI";
            this.tabPage_ROI2.BackColor = clrPanel;
            this.tabPage_ROI2.ForeColor = clrText;
            this.tabPage_ROI2.Padding = new System.Windows.Forms.Padding(6);
            this.tabPage_ROI2.Controls.Add(this.tlp_ROI2);

            SetupROITable(this.tlp_ROI1, "L1",
                this.lbl_Num_L1, this.lbl_Width_L1, this.lbl_Height_L1,
                this.lbl_Angle_L1, this.lbl_MidRow_L1, this.lbl_MidCol_L1,
                this.NUD_RoiNum_L1, this.NUD_RoiWidth_L1, this.NUD_RoiHeight_L1,
                this.NUD_RoiAngle_L1, this.NUD_RoiMidRow_L1, this.NUD_RoiMidColumn_L1,
                new System.EventHandler(this.NUD_Roi_L1_ValueChanged),
                clrPanel, clrText, clrCard, clrCtrlBg, fontLabel, fontNUD);

            SetupROITable(this.tlp_ROI2, "L2",
                this.lbl_Num_L2, this.lbl_Width_L2, this.lbl_Height_L2,
                this.lbl_Angle_L2, this.lbl_MidRow_L2, this.lbl_MidCol_L2,
                this.NUD_RoiNum_L2, this.NUD_RoiWidth_L2, this.NUD_RoiHeight_L2,
                this.NUD_RoiAngle_L2, this.NUD_RoiMidRow_L2, this.NUD_RoiMidColumn_L2,
                new System.EventHandler(this.NUD_Roi_L2_ValueChanged),
                clrPanel, clrText, clrCard, clrCtrlBg, fontLabel, fontNUD);

            // ===== Measure Tab Pages =====
            this.tabPage_Measure1.Name = "tabPage_Measure1";
            this.tabPage_Measure1.Text = "\u76f4\u7ebf1 \u6d4b\u91cf";
            this.tabPage_Measure1.BackColor = clrPanel;
            this.tabPage_Measure1.ForeColor = clrText;
            this.tabPage_Measure1.Padding = new System.Windows.Forms.Padding(6);
            this.tabPage_Measure1.Controls.Add(this.panel_Measure1);

            this.tabPage_Measure2.Name = "tabPage_Measure2";
            this.tabPage_Measure2.Text = "\u76f4\u7ebf2 \u6d4b\u91cf";
            this.tabPage_Measure2.BackColor = clrPanel;
            this.tabPage_Measure2.ForeColor = clrText;
            this.tabPage_Measure2.Padding = new System.Windows.Forms.Padding(6);
            this.tabPage_Measure2.Controls.Add(this.panel_Measure2);

            SetupMeasurePanel(this.panel_Measure1, "L1",
                this.lbl_L1_Sigma, this.lbl_L1_Threshold, this.lbl_L1_Transition,
                this.lbl_L1_Select, this.lbl_L1_MinScore, this.lbl_L1_NumInst,
                this.lbl_L1_DistThresh, this.lbl_L1_MaxIter, this.lbl_L1_Interpolation, this.lbl_L1_IORegions,
                this.NUD_L1_Sigma, this.NUD_L1_Threshold,
                this.CB_L1_Transition, this.CB_L1_Select,
                this.NUD_L1_MinScore, this.NUD_L1_NumInstances,
                this.NUD_L1_DistThreshold, this.NUD_L1_MaxIterations,
                this.CB_L1_Interpolation, this.CB_L1_IORegions,
                new System.EventHandler(this.NUD_Measure_ValueChanged),
                new System.EventHandler(this.CB_Measure_SelectedIndexChanged),
                clrPanel, clrText, clrCtrlBg, fontLabel, fontNUD);

            SetupMeasurePanel(this.panel_Measure2, "L2",
                this.lbl_L2_Sigma, this.lbl_L2_Threshold, this.lbl_L2_Transition,
                this.lbl_L2_Select, this.lbl_L2_MinScore, this.lbl_L2_NumInst,
                this.lbl_L2_DistThresh, this.lbl_L2_MaxIter, this.lbl_L2_Interpolation, this.lbl_L2_IORegions,
                this.NUD_L2_Sigma, this.NUD_L2_Threshold,
                this.CB_L2_Transition, this.CB_L2_Select,
                this.NUD_L2_MinScore, this.NUD_L2_NumInstances,
                this.NUD_L2_DistThreshold, this.NUD_L2_MaxIterations,
                this.CB_L2_Interpolation, this.CB_L2_IORegions,
                new System.EventHandler(this.NUD_Measure_ValueChanged),
                new System.EventHandler(this.CB_Measure_SelectedIndexChanged),
                clrPanel, clrText, clrCtrlBg, fontLabel, fontNUD);

            // ===== Result Tab =====
            this.tabPage_Result.Name = "tabPage_Result";
            this.tabPage_Result.Text = "\u68c0\u6d4b\u7ed3\u679c";
            this.tabPage_Result.BackColor = clrPanel;
            this.tabPage_Result.ForeColor = clrText;
            this.tabPage_Result.Padding = new System.Windows.Forms.Padding(6);
            this.tabPage_Result.Controls.Add(this.panel_Result);

            this.panel_Result.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_Result.AutoScroll = true;
            // this.panel_Result.BackColor = clrTabPage;
            this.panel_Result.Name = "panel_Result";
            this.panel_Result.Padding = new System.Windows.Forms.Padding(8);

            int ry = 12; int rSpacing = 34;

            this.lbl_ResultTitle.Name = "lbl_ResultTitle";
            this.lbl_ResultTitle.Text = "Detection Result";
            this.lbl_ResultTitle.Font = new System.Drawing.Font("Microsoft YaHei UI", 11F, System.Drawing.FontStyle.Bold);
            this.lbl_ResultTitle.ForeColor = clrAccent;
            this.lbl_ResultTitle.BackColor = System.Drawing.Color.Transparent;
            this.lbl_ResultTitle.Location = new System.Drawing.Point(12, ry);
            this.lbl_ResultTitle.AutoSize = true;
            this.panel_Result.Controls.Add(this.lbl_ResultTitle);
            ry += rSpacing + 4;

            System.Windows.Forms.Label sepLine = new System.Windows.Forms.Label();
            sepLine.Text = ""; sepLine.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            sepLine.Location = new System.Drawing.Point(12, ry - 6);
            sepLine.Size = new System.Drawing.Size(220, 2); sepLine.BackColor = clrBorder;
            this.panel_Result.Controls.Add(sepLine);

            SetupResultLabel(this.LB_Line1Success, "LB_Line1Success", "\u7ebf1\u62df\u5408: --", 12, ry, fontNormal, clrText);
            this.panel_Result.Controls.Add(this.LB_Line1Success); ry += rSpacing;
            SetupResultLabel(this.LB_Line1Info, "LB_Line1Info", "\u7ebf1: \u672a\u68c0\u6d4b\u5230", 12, ry, fontSmall, clrTextDim);
            this.panel_Result.Controls.Add(this.LB_Line1Info); ry += rSpacing;
            SetupResultLabel(this.LB_Line2Success, "LB_Line2Success", "\u7ebf2\u62df\u5408: --", 12, ry, fontNormal, clrText);
            this.panel_Result.Controls.Add(this.LB_Line2Success); ry += rSpacing;
            SetupResultLabel(this.LB_Line2Info, "LB_Line2Info", "\u7ebf2: \u672a\u68c0\u6d4b\u5230", 12, ry, fontSmall, clrTextDim);
            this.panel_Result.Controls.Add(this.LB_Line2Info); ry += rSpacing + 8;

            System.Windows.Forms.Label sepLine2 = new System.Windows.Forms.Label();
            sepLine2.Text = ""; sepLine2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            sepLine2.Location = new System.Drawing.Point(12, ry - 6);
            sepLine2.Size = new System.Drawing.Size(220, 2); sepLine2.BackColor = clrBorder;
            this.panel_Result.Controls.Add(sepLine2);

            SetupResultLabel(this.LB_ResultRow, "LB_ResultRow", "\u4ea4\u70b9\u884c: --", 12, ry, fontNormal, clrText);
            this.panel_Result.Controls.Add(this.LB_ResultRow); ry += rSpacing;
            SetupResultLabel(this.LB_ResultCol, "LB_ResultCol", "\u4ea4\u70b9\u5217: --", 12, ry, fontNormal, clrText);
            this.panel_Result.Controls.Add(this.LB_ResultCol); ry += rSpacing;
            SetupResultLabel(this.LB_ResultAngle, "LB_ResultAngle", "½Ç¶È: --", 12, ry, fontNormal, clrText);
            this.panel_Result.Controls.Add(this.LB_ResultAngle); ry += rSpacing;
            SetupResultLabel(this.LB_IsParallel, "LB_IsParallel", "\u662f\u5426\u5e73\u884c: --", 12, ry, fontNormal, clrText);
            this.panel_Result.Controls.Add(this.LB_IsParallel);

            // ===== Timer =====
            this.timer1.Enabled = true;
            this.timer1.Interval = 200;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);

            // ===== ResumeLayout =====
            ((System.ComponentModel.ISupportInitialize)(this.LoadImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.FullScrean)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_RoiNum_L1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_RoiWidth_L1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_RoiHeight_L1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_RoiAngle_L1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_RoiMidRow_L1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_RoiMidColumn_L1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_RoiNum_L2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_RoiWidth_L2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_RoiHeight_L2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_RoiAngle_L2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_RoiMidRow_L2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_RoiMidColumn_L2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_L1_Sigma)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_L1_Threshold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_L1_MinScore)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_L1_NumInstances)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_L1_DistThreshold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_L1_MaxIterations)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_L2_Sigma)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_L2_Threshold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_L2_MinScore)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_L2_NumInstances)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_L2_DistThreshold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_L2_MaxIterations)).EndInit();
            this.tabPage_ROI1.ResumeLayout(false);
            this.tabPage_ROI2.ResumeLayout(false);
            this.tabPage_Measure1.ResumeLayout(false);
            this.tabPage_Measure2.ResumeLayout(false);
            this.tabPage_Result.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.panelChartContainer.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

            // ===== Form =====
            this.BackColor = clrBg;
            this.Controls.Add(this.tableLayoutPanel2);
            this.Name = "FormFindIntersection";
            this.Load += new System.EventHandler(this.FormFindIntersection_Load);
            this.HWC_Display.HMouseMove += new HalconDotNet.HMouseEventHandler(this.HWC_Display_HMouseMove);
            //this.HWC_Display.HMouseDoubleClick += new HalconDotNet.HMouseEventHandler(this.HWC_Display_HMouseDoubleClick);
        }

        #region Helper Methods

        private void SetupROITable(
            System.Windows.Forms.TableLayoutPanel tlp, string suffix,
            System.Windows.Forms.Label lblNum, System.Windows.Forms.Label lblWidth,
            System.Windows.Forms.Label lblHeight, System.Windows.Forms.Label lblAngle,
            System.Windows.Forms.Label lblMidRow, System.Windows.Forms.Label lblMidCol,
            System.Windows.Forms.NumericUpDown nudNum, System.Windows.Forms.NumericUpDown nudWidth,
            System.Windows.Forms.NumericUpDown nudHeight, System.Windows.Forms.NumericUpDown nudAngle,
            System.Windows.Forms.NumericUpDown nudMidRow, System.Windows.Forms.NumericUpDown nudMidCol,
            System.EventHandler nudValueChanged,
            System.Drawing.Color bgColor, System.Drawing.Color textColor,
            System.Drawing.Color cardColor, System.Drawing.Color ctrlBgColor,
            System.Drawing.Font labelFont, System.Drawing.Font nudFont)
        {
            tlp.ColumnCount = 2;
            tlp.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            tlp.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            tlp.RowCount = 7;
            for (int i = 0; i < 6; i++)
                tlp.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            tlp.Dock = System.Windows.Forms.DockStyle.Fill;
            tlp.BackColor = bgColor;
            tlp.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.None;
            tlp.Padding = new System.Windows.Forms.Padding(4);
            string[] labels = { "\u70b9\u6570", "\u5bbd\u5ea6", "\u9ad8\u5ea6", "\u89d2\u5ea6", "\u4e2d\u5fc3\u884c", "\u4e2d\u5fc3\u5217" };
            System.Windows.Forms.Label[] lblArr = { lblNum, lblWidth, lblHeight, lblAngle, lblMidRow, lblMidCol };
            System.Windows.Forms.NumericUpDown[] nudArr = { nudNum, nudWidth, nudHeight, nudAngle, nudMidRow, nudMidCol };
            for (int i = 0; i < 6; i++)
            {
                SetupTableLabel(lblArr[i], "lbl_" + labels[i] + "_" + suffix, labels[i], labelFont, textColor);
                tlp.Controls.Add(lblArr[i], 0, i);
                SetupTableNUD(nudArr[i], "NUD_Roi" + labels[i] + "_" + suffix, nudFont, ctrlBgColor, textColor);
                nudArr[i].ValueChanged += nudValueChanged;
                tlp.Controls.Add(nudArr[i], 1, i);
            }
            nudNum.Minimum = 1; nudNum.Maximum = 100; nudNum.Value = 20; nudNum.DecimalPlaces = 0;
            nudWidth.Minimum = 1; nudWidth.Maximum = 9999; nudWidth.Value = 100; nudWidth.DecimalPlaces = 0;
            nudHeight.Minimum = 1; nudHeight.Maximum = 9999; nudHeight.Value = 200; nudHeight.DecimalPlaces = 0;
            nudAngle.Minimum = -180; nudAngle.Maximum = 180; nudAngle.Value = 0; nudAngle.DecimalPlaces = 2; nudAngle.Increment = 0.1m;
            nudMidRow.Minimum = 0; nudMidRow.Maximum = 99999; nudMidRow.Value = 200; nudMidRow.DecimalPlaces = 1;
            nudMidCol.Minimum = 0; nudMidCol.Maximum = 99999; nudMidCol.Value = 300; nudMidCol.DecimalPlaces = 1;
        }

        private void SetupTableLabel(System.Windows.Forms.Label lbl, string name, string text,
            System.Drawing.Font font, System.Drawing.Color foreColor)
        {
            lbl.Name = name; lbl.Text = text; lbl.Font = font; lbl.ForeColor = foreColor;
            lbl.BackColor = System.Drawing.Color.Transparent;
            lbl.Dock = System.Windows.Forms.DockStyle.Fill;
            lbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            lbl.Padding = new System.Windows.Forms.Padding(8, 0, 0, 0);
            lbl.AutoSize = false;
        }

        private void SetupTableNUD(System.Windows.Forms.NumericUpDown nud, string name,
            System.Drawing.Font font, System.Drawing.Color bgColor, System.Drawing.Color foreColor)
        {
            nud.Name = name; nud.Font = font; nud.BackColor = bgColor; nud.ForeColor = foreColor;
            nud.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            nud.Dock = System.Windows.Forms.DockStyle.Fill;
            nud.Margin = new System.Windows.Forms.Padding(4, 6, 12, 6);
            nud.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
        }

        private void SetupMeasurePanel(
            System.Windows.Forms.Panel panel, string suffix,
            System.Windows.Forms.Label lSigma, System.Windows.Forms.Label lThresh,
            System.Windows.Forms.Label lTrans, System.Windows.Forms.Label lSel,
            System.Windows.Forms.Label lMinScore, System.Windows.Forms.Label lNumInst,
            System.Windows.Forms.Label lDistThresh, System.Windows.Forms.Label lMaxIter,
            System.Windows.Forms.Label lInterp, System.Windows.Forms.Label lIOReg,
            System.Windows.Forms.NumericUpDown nSigma, System.Windows.Forms.NumericUpDown nThresh,
            System.Windows.Forms.ComboBox cTrans, System.Windows.Forms.ComboBox cSel,
            System.Windows.Forms.NumericUpDown nMinScore, System.Windows.Forms.NumericUpDown nNumInst,
            System.Windows.Forms.NumericUpDown nDistThresh, System.Windows.Forms.NumericUpDown nMaxIter,
            System.Windows.Forms.ComboBox cInterp, System.Windows.Forms.ComboBox cIOReg,
            System.EventHandler nudChanged, System.EventHandler cbChanged,
            System.Drawing.Color bgColor, System.Drawing.Color textColor,
            System.Drawing.Color ctrlBgColor, System.Drawing.Font labelFont, System.Drawing.Font nudFont)
        {
            panel.Dock = System.Windows.Forms.DockStyle.Fill;
            panel.AutoScroll = true;
            panel.Name = "panel_Measure_" + suffix;
            panel.BackColor = bgColor;
            panel.Padding = new System.Windows.Forms.Padding(4);
            int y = 12, lblX = 12, ctrlX = 120, ctrlW = 140, rowH = 36;
            SetupLabel(lSigma, "lbl_" + suffix + "_Sigma", "Sigma", lblX, y, labelFont, textColor); panel.Controls.Add(lSigma);
            SetupNUD(nSigma, "NUD_" + suffix + "_Sigma", ctrlX, y, ctrlW, 0.5m, 10m, 1, 3.0m, nudFont, ctrlBgColor, textColor); nSigma.Increment = 0.1m; nSigma.ValueChanged += nudChanged; panel.Controls.Add(nSigma); y += rowH;
            SetupLabel(lThresh, "lbl_" + suffix + "_Threshold", "\u9608\u503c", lblX, y, labelFont, textColor); panel.Controls.Add(lThresh);
            SetupNUD(nThresh, "NUD_" + suffix + "_Threshold", ctrlX, y, ctrlW, 0, 255, 0, 20, nudFont, ctrlBgColor, textColor); nThresh.ValueChanged += nudChanged; panel.Controls.Add(nThresh); y += rowH;
            SetupLabel(lTrans, "lbl_" + suffix + "_Transition", "\u6781\u6027", lblX, y, labelFont, textColor); panel.Controls.Add(lTrans);
            SetupComboBox(cTrans, "CB_" + suffix + "_Transition", ctrlX, y, ctrlW, nudFont, ctrlBgColor, textColor); cTrans.SelectedIndexChanged += cbChanged; panel.Controls.Add(cTrans); y += rowH;
            SetupLabel(lSel, "lbl_" + suffix + "_Select", "\u9009\u8fb9", lblX, y, labelFont, textColor); panel.Controls.Add(lSel);
            SetupComboBox(cSel, "CB_" + suffix + "_Select", ctrlX, y, ctrlW, nudFont, ctrlBgColor, textColor); cSel.SelectedIndexChanged += cbChanged; panel.Controls.Add(cSel); y += rowH;
            SetupLabel(lMinScore, "lbl_" + suffix + "_MinScore", "\u6700\u5c0f\u5f97\u5206", lblX, y, labelFont, textColor); panel.Controls.Add(lMinScore);
            SetupNUD(nMinScore, "NUD_" + suffix + "_MinScore", ctrlX, y, ctrlW, 0.1m, 1m, 1, 0.5m, nudFont, ctrlBgColor, textColor); nMinScore.Increment = 0.1m; nMinScore.ValueChanged += nudChanged; panel.Controls.Add(nMinScore); y += rowH;
            SetupLabel(lNumInst, "lbl_" + suffix + "_NumInst", "\u5b9e\u4f8b\u6570", lblX, y, labelFont, textColor); panel.Controls.Add(lNumInst);
            SetupNUD(nNumInst, "NUD_" + suffix + "_NumInstances", ctrlX, y, ctrlW, 1, 9999, 0, 1, nudFont, ctrlBgColor, textColor); nNumInst.ValueChanged += nudChanged; panel.Controls.Add(nNumInst); y += rowH;
            SetupLabel(lDistThresh, "lbl_" + suffix + "_DistThresh", "\u8ddd\u79bb\u9608\u503c", lblX, y, labelFont, textColor); panel.Controls.Add(lDistThresh);
            SetupNUD(nDistThresh, "NUD_" + suffix + "_DistThreshold", ctrlX, y, ctrlW, 0.5m, 100m, 1, 3.5m, nudFont, ctrlBgColor, textColor); nDistThresh.Increment = 0.5m; nDistThresh.ValueChanged += nudChanged; panel.Controls.Add(nDistThresh); y += rowH;
            SetupLabel(lMaxIter, "lbl_" + suffix + "_MaxIter", "\u6700\u5927\u8fed\u4ee3", lblX, y, labelFont, textColor); panel.Controls.Add(lMaxIter);
            SetupNUD(nMaxIter, "NUD_" + suffix + "_MaxIterations", ctrlX, y, ctrlW, -1, 50, 0, -1, nudFont, ctrlBgColor, textColor); nMaxIter.ValueChanged += nudChanged; panel.Controls.Add(nMaxIter); y += rowH;
            SetupLabel(lInterp, "lbl_" + suffix + "_Interpolation", "\u63d2\u503c\u7c7b\u578b", lblX, y, labelFont, textColor); panel.Controls.Add(lInterp);
            SetupComboBox(cInterp, "CB_" + suffix + "_Interpolation", ctrlX, y, ctrlW, nudFont, ctrlBgColor, textColor); cInterp.SelectedIndexChanged += cbChanged; panel.Controls.Add(cInterp); y += rowH;
            SetupLabel(lIOReg, "lbl_" + suffix + "_IORegions", "\u533a\u57df\u9a8c\u8bc1", lblX, y, labelFont, textColor); panel.Controls.Add(lIOReg);
            SetupComboBox(cIOReg, "CB_" + suffix + "_IORegions", ctrlX, y, ctrlW, nudFont, ctrlBgColor, textColor); cIOReg.SelectedIndexChanged += cbChanged; panel.Controls.Add(cIOReg);
       
        }

        private void SetupLabel(System.Windows.Forms.Label lbl, string name, string text, int x, int y,
            System.Drawing.Font font, System.Drawing.Color foreColor)
        { lbl.Name = name; lbl.Text = text; lbl.Location = new System.Drawing.Point(x, y + 4); lbl.AutoSize = true; lbl.Font = font; lbl.ForeColor = foreColor; lbl.BackColor = System.Drawing.Color.Transparent; }

        private void SetupNUD(System.Windows.Forms.NumericUpDown nud, string name,
            int x, int y, int w, decimal min, decimal max, int decimals, decimal value,
            System.Drawing.Font font, System.Drawing.Color bgColor, System.Drawing.Color foreColor)
        { nud.Name = name; nud.Location = new System.Drawing.Point(x, y); nud.Size = new System.Drawing.Size(w, 28); nud.Minimum = min; nud.Maximum = max; nud.DecimalPlaces = decimals; nud.Value = value; nud.Font = font; nud.BackColor = bgColor; nud.ForeColor = foreColor; nud.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle; nud.TextAlign = System.Windows.Forms.HorizontalAlignment.Center; }

        private void SetupComboBox(System.Windows.Forms.ComboBox cb, string name,
            int x, int y, int w, System.Drawing.Font font,
            System.Drawing.Color bgColor, System.Drawing.Color foreColor)
        { cb.Name = name; cb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList; cb.Location = new System.Drawing.Point(x, y); cb.Size = new System.Drawing.Size(w, 28); cb.Font = font; cb.BackColor = bgColor; cb.ForeColor = foreColor; cb.FlatStyle = System.Windows.Forms.FlatStyle.Flat; }

        private void SetupResultLabel(System.Windows.Forms.Label lbl, string name, string text,
            int x, int y, System.Drawing.Font font, System.Drawing.Color foreColor)
        { lbl.Name = name; lbl.Text = text; lbl.Location = new System.Drawing.Point(x, y); lbl.AutoSize = true; lbl.Font = font; lbl.ForeColor = foreColor; lbl.BackColor = System.Drawing.Color.Transparent; lbl.Padding = new System.Windows.Forms.Padding(0, 4, 0, 4); }

        #endregion

        // ===== Field declarations =====
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        public System.Windows.Forms.Label LB_ModleName;
        public System.Windows.Forms.Label LB_IMageSize;
        private HalconDotNet.HWindowControl HWC_Display;
        private System.Windows.Forms.PictureBox LoadImage;
        private System.Windows.Forms.TextBox TB_PositionAndGray;
        private System.Windows.Forms.PictureBox FullScrean;
        //public System.Windows.Forms.Label Tb_CostTime;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TableLayoutPanel panelChartContainer;
        private System.Windows.Forms.Panel panelHProfile;
        private System.Windows.Forms.Panel panelVProfile;
        private System.Windows.Forms.Button btnCrosshair;
        internal System.Drawing.Color _clrBg;
        internal System.Drawing.Color _clrPanel;
        internal System.Drawing.Color _clrAccent;
        internal System.Drawing.Color _clrText;
        internal System.Drawing.Color _clrTextDim;
        internal System.Drawing.Color _clrBorder;
        private System.Windows.Forms.TabPage tabPage_ROI1;
        private System.Windows.Forms.TableLayoutPanel tlp_ROI1;
        private System.Windows.Forms.Label lbl_Num_L1;
        private System.Windows.Forms.Label lbl_Width_L1;
        private System.Windows.Forms.Label lbl_Height_L1;
        private System.Windows.Forms.Label lbl_Angle_L1;
        private System.Windows.Forms.Label lbl_MidRow_L1;
        private System.Windows.Forms.Label lbl_MidCol_L1;
        private System.Windows.Forms.NumericUpDown NUD_RoiNum_L1;
        private System.Windows.Forms.NumericUpDown NUD_RoiWidth_L1;
        private System.Windows.Forms.NumericUpDown NUD_RoiHeight_L1;
        private System.Windows.Forms.NumericUpDown NUD_RoiAngle_L1;
        private System.Windows.Forms.NumericUpDown NUD_RoiMidRow_L1;
        private System.Windows.Forms.NumericUpDown NUD_RoiMidColumn_L1;
        private System.Windows.Forms.TabPage tabPage_ROI2;
        private System.Windows.Forms.TableLayoutPanel tlp_ROI2;
        private System.Windows.Forms.Label lbl_Num_L2;
        private System.Windows.Forms.Label lbl_Width_L2;
        private System.Windows.Forms.Label lbl_Height_L2;
        private System.Windows.Forms.Label lbl_Angle_L2;
        private System.Windows.Forms.Label lbl_MidRow_L2;
        private System.Windows.Forms.Label lbl_MidCol_L2;
        private System.Windows.Forms.NumericUpDown NUD_RoiNum_L2;
        private System.Windows.Forms.NumericUpDown NUD_RoiWidth_L2;
        private System.Windows.Forms.NumericUpDown NUD_RoiHeight_L2;
        private System.Windows.Forms.NumericUpDown NUD_RoiAngle_L2;
        private System.Windows.Forms.NumericUpDown NUD_RoiMidRow_L2;
        private System.Windows.Forms.NumericUpDown NUD_RoiMidColumn_L2;
        private System.Windows.Forms.TabPage tabPage_Measure1;
        private System.Windows.Forms.Panel panel_Measure1;
        private System.Windows.Forms.Label lbl_L1_Sigma;
        private System.Windows.Forms.Label lbl_L1_Threshold;
        private System.Windows.Forms.Label lbl_L1_Transition;
        private System.Windows.Forms.Label lbl_L1_Select;
        private System.Windows.Forms.Label lbl_L1_MinScore;
        private System.Windows.Forms.Label lbl_L1_NumInst;
        private System.Windows.Forms.Label lbl_L1_DistThresh;
        private System.Windows.Forms.Label lbl_L1_MaxIter;
        private System.Windows.Forms.Label lbl_L1_Interpolation;
        private System.Windows.Forms.Label lbl_L1_IORegions;
        private System.Windows.Forms.NumericUpDown NUD_L1_Sigma;
        private System.Windows.Forms.NumericUpDown NUD_L1_Threshold;
        private System.Windows.Forms.ComboBox CB_L1_Transition;
        private System.Windows.Forms.ComboBox CB_L1_Select;
        private System.Windows.Forms.NumericUpDown NUD_L1_MinScore;
        private System.Windows.Forms.NumericUpDown NUD_L1_NumInstances;
        private System.Windows.Forms.NumericUpDown NUD_L1_DistThreshold;
        private System.Windows.Forms.NumericUpDown NUD_L1_MaxIterations;
        private System.Windows.Forms.ComboBox CB_L1_Interpolation;
        private System.Windows.Forms.ComboBox CB_L1_IORegions;
        private System.Windows.Forms.TabPage tabPage_Measure2;
        private System.Windows.Forms.Panel panel_Measure2;
        private System.Windows.Forms.Label lbl_L2_Sigma;
        private System.Windows.Forms.Label lbl_L2_Threshold;
        private System.Windows.Forms.Label lbl_L2_Transition;
        private System.Windows.Forms.Label lbl_L2_Select;
        private System.Windows.Forms.Label lbl_L2_MinScore;
        private System.Windows.Forms.Label lbl_L2_NumInst;
        private System.Windows.Forms.Label lbl_L2_DistThresh;
        private System.Windows.Forms.Label lbl_L2_MaxIter;
        private System.Windows.Forms.Label lbl_L2_Interpolation;
        private System.Windows.Forms.Label lbl_L2_IORegions;
        private System.Windows.Forms.NumericUpDown NUD_L2_Sigma;
        private System.Windows.Forms.NumericUpDown NUD_L2_Threshold;
        private System.Windows.Forms.ComboBox CB_L2_Transition;
        private System.Windows.Forms.ComboBox CB_L2_Select;
        private System.Windows.Forms.NumericUpDown NUD_L2_MinScore;
        private System.Windows.Forms.NumericUpDown NUD_L2_NumInstances;
        private System.Windows.Forms.NumericUpDown NUD_L2_DistThreshold;
        private System.Windows.Forms.NumericUpDown NUD_L2_MaxIterations;
        private System.Windows.Forms.ComboBox CB_L2_Interpolation;
        private System.Windows.Forms.ComboBox CB_L2_IORegions;
        private System.Windows.Forms.TabPage tabPage_Result;
        private System.Windows.Forms.Panel panel_Result;
        private System.Windows.Forms.Label lbl_ResultTitle;
        private System.Windows.Forms.Label LB_Line1Success;
        private System.Windows.Forms.Label LB_Line2Success;
        private System.Windows.Forms.Label LB_ResultRow;
        private System.Windows.Forms.Label LB_ResultCol;
        private System.Windows.Forms.Label LB_ResultAngle;
        private System.Windows.Forms.Label LB_IsParallel;
        private System.Windows.Forms.Label LB_Line1Info;
        private System.Windows.Forms.Label LB_Line2Info;
        private System.Windows.Forms.Timer timer1;
    }
}
#endregion
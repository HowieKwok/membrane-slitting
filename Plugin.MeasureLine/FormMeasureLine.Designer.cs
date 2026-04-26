namespace Plugin.MeasureLine
{
    partial class FormMeasureLine
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMeasureLine));
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.LB_IMageSize = new System.Windows.Forms.Label();
            this.TB_PositionAndGray = new System.Windows.Forms.TextBox();
            this.LB_ModleName = new System.Windows.Forms.Label();
            this.LoadImage = new System.Windows.Forms.PictureBox();
            this.HWC_Display = new HalconDotNet.HWindowControl();
            this.FullScrean = new System.Windows.Forms.PictureBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel7 = new System.Windows.Forms.TableLayoutPanel();
            this.label19 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.HWC_Image3 = new HalconDotNet.HWindowControl();
            this.HWC_Image2 = new HalconDotNet.HWindowControl();
            this.HWC_Image1 = new HalconDotNet.HWindowControl();
            this.label16 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.TB_ROIMoveIndex = new System.Windows.Forms.TabControl();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.NUD_RoiMidColumn = new System.Windows.Forms.NumericUpDown();
            this.NUD_RoiMidRow = new System.Windows.Forms.NumericUpDown();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.NUD_RoiHeight = new System.Windows.Forms.NumericUpDown();
            this.NUD_RoiWidth = new System.Windows.Forms.NumericUpDown();
            this.NUD_RoiNum = new System.Windows.Forms.NumericUpDown();
            this.label17 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.NUD_RoiAngle = new System.Windows.Forms.NumericUpDown();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.NUD_SharpenSigma = new System.Windows.Forms.NumericUpDown();
            this.LB_SharpenSigma = new System.Windows.Forms.Label();
            this.NUD_SharpenIterations = new System.Windows.Forms.NumericUpDown();
            this.LB_SharpenIterations = new System.Windows.Forms.Label();
            this.NUD_ScaleFactor = new System.Windows.Forms.NumericUpDown();
            this.LB_ScaleFactor = new System.Windows.Forms.Label();
            this.NUD_ScaleMean = new System.Windows.Forms.NumericUpDown();
            this.PB_SelectColor = new System.Windows.Forms.PictureBox();
            this.LB_ScaleMean = new System.Windows.Forms.Label();
            this.RB_ScaleSharpen = new System.Windows.Forms.RadioButton();
            this.RB_ProcessNone = new System.Windows.Forms.RadioButton();
            this.CB_DefaultLine = new System.Windows.Forms.CheckBox();
            this.CB_PeakValleySelect = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.CB_MeasureSelect = new System.Windows.Forms.ComboBox();
            this.CB_MeasureTransition = new System.Windows.Forms.ComboBox();
            this.NUD_MeasureSigma = new System.Windows.Forms.NumericUpDown();
            this.NUD_MeasureThreshold = new System.Windows.Forms.NumericUpDown();
            this.label23 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.panel2 = new System.Windows.Forms.Panel();
            this.CB_MeasureInterpolation = new System.Windows.Forms.ComboBox();
            this.CB_MeasureIORegions = new System.Windows.Forms.ComboBox();
            this.NUD_MaxNumIterations = new System.Windows.Forms.NumericUpDown();
            this.NUD_MeasureDistThreshold = new System.Windows.Forms.NumericUpDown();
            this.NUD_MeasureNumInstances = new System.Windows.Forms.NumericUpDown();
            this.NUD_MeasureMinScore = new System.Windows.Forms.NumericUpDown();
            this.label28 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.NUD_RoiStandardColumn = new System.Windows.Forms.NumericUpDown();
            this.NUD_RoiStandardRow = new System.Windows.Forms.NumericUpDown();
            this.LB_RoiStandardColumn = new System.Windows.Forms.Label();
            this.LB_RoiStandardRow = new System.Windows.Forms.Label();
            this.NUD_RoiStandardOffsetColumn = new System.Windows.Forms.NumericUpDown();
            this.NUD_RoiStandardOffsetRow = new System.Windows.Forms.NumericUpDown();
            this.LB_RoiStandardOffsetColumn = new System.Windows.Forms.Label();
            this.LB_RoiStandardOffsetRow = new System.Windows.Forms.Label();
            this.NUD_RoiIncreaseColumn = new System.Windows.Forms.NumericUpDown();
            this.NUD_RoiIncreaseRow = new System.Windows.Forms.NumericUpDown();
            this.NUD_RoiIncreaseNum = new System.Windows.Forms.NumericUpDown();
            this.LB_RoiIncreaseNum = new System.Windows.Forms.Label();
            this.LB_RoiIncreaseRow = new System.Windows.Forms.Label();
            this.LB_RoiIncreaseColumn = new System.Windows.Forms.Label();
            this.RB_IncreaseROI = new System.Windows.Forms.RadioButton();
            this.RB_StandardROI = new System.Windows.Forms.RadioButton();
            this.RB_StaticROI = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.TrB_ROIIndex = new System.Windows.Forms.TrackBar();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LoadImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.FullScrean)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel7.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.TB_ROIMoveIndex.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_RoiMidColumn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_RoiMidRow)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_RoiHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_RoiWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_RoiNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_RoiAngle)).BeginInit();
            this.tabPage1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_SharpenSigma)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_SharpenIterations)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_ScaleFactor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_ScaleMean)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PB_SelectColor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_MeasureSigma)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_MeasureThreshold)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_MaxNumIterations)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_MeasureDistThreshold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_MeasureNumInstances)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_MeasureMinScore)).BeginInit();
            this.tabPage5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_RoiStandardColumn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_RoiStandardRow)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_RoiStandardOffsetColumn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_RoiStandardOffsetRow)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_RoiIncreaseColumn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_RoiIncreaseRow)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_RoiIncreaseNum)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TrB_ROIIndex)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.groupBox1, 0, 11);
            this.tableLayoutPanel2.Controls.Add(this.groupBox2, 9, 0);
            this.tableLayoutPanel2.Controls.Add(this.groupBox3, 9, 10);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            resources.ApplyResources(this.tableLayoutPanel3, "tableLayoutPanel3");
            this.tableLayoutPanel2.SetColumnSpan(this.tableLayoutPanel3, 9);
            this.tableLayoutPanel3.Controls.Add(this.LB_IMageSize, 4, 0);
            this.tableLayoutPanel3.Controls.Add(this.TB_PositionAndGray, 3, 2);
            this.tableLayoutPanel3.Controls.Add(this.LB_ModleName, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.LoadImage, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.HWC_Display, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.FullScrean, 7, 2);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel2.SetRowSpan(this.tableLayoutPanel3, 11);
            // 
            // LB_IMageSize
            // 
            resources.ApplyResources(this.LB_IMageSize, "LB_IMageSize");
            this.LB_IMageSize.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.tableLayoutPanel3.SetColumnSpan(this.LB_IMageSize, 3);
            this.LB_IMageSize.ForeColor = System.Drawing.Color.White;
            this.LB_IMageSize.Name = "LB_IMageSize";
            // 
            // TB_PositionAndGray
            // 
            this.TB_PositionAndGray.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.TB_PositionAndGray.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tableLayoutPanel3.SetColumnSpan(this.TB_PositionAndGray, 3);
            this.TB_PositionAndGray.Cursor = System.Windows.Forms.Cursors.Default;
            resources.ApplyResources(this.TB_PositionAndGray, "TB_PositionAndGray");
            this.TB_PositionAndGray.ForeColor = System.Drawing.Color.White;
            this.TB_PositionAndGray.Name = "TB_PositionAndGray";
            this.TB_PositionAndGray.ShortcutsEnabled = false;
            this.TB_PositionAndGray.TabStop = false;
            // 
            // LB_ModleName
            // 
            resources.ApplyResources(this.LB_ModleName, "LB_ModleName");
            this.LB_ModleName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.tableLayoutPanel3.SetColumnSpan(this.LB_ModleName, 3);
            this.LB_ModleName.ForeColor = System.Drawing.Color.White;
            this.LB_ModleName.Name = "LB_ModleName";
            // 
            // LoadImage
            // 
            this.LoadImage.BackColor = System.Drawing.Color.Blue;
            resources.ApplyResources(this.LoadImage, "LoadImage");
            this.LoadImage.Name = "LoadImage";
            this.LoadImage.TabStop = false;
            this.LoadImage.Click += new System.EventHandler(this.LoadImage_Click);
            // 
            // HWC_Display
            // 
            this.HWC_Display.BackColor = System.Drawing.Color.White;
            this.HWC_Display.BorderColor = System.Drawing.Color.White;
            this.tableLayoutPanel3.SetColumnSpan(this.HWC_Display, 8);
            resources.ApplyResources(this.HWC_Display, "HWC_Display");
            this.HWC_Display.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.HWC_Display.Name = "HWC_Display";
            this.HWC_Display.WindowSize = new System.Drawing.Size(579, 475);
            this.HWC_Display.HMouseMove += new HalconDotNet.HMouseEventHandler(this.HWC_Display_HMouseMove);
            // 
            // FullScrean
            // 
            this.FullScrean.BackColor = System.Drawing.Color.Blue;
            resources.ApplyResources(this.FullScrean, "FullScrean");
            this.FullScrean.Name = "FullScrean";
            this.FullScrean.TabStop = false;
            this.FullScrean.Click += new System.EventHandler(this.FullScrean_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.tableLayoutPanel2.SetColumnSpan(this.groupBox1, 9);
            this.groupBox1.Controls.Add(this.tableLayoutPanel7);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.tableLayoutPanel2.SetRowSpan(this.groupBox1, 4);
            this.groupBox1.TabStop = false;
            // 
            // tableLayoutPanel7
            // 
            resources.ApplyResources(this.tableLayoutPanel7, "tableLayoutPanel7");
            this.tableLayoutPanel7.Controls.Add(this.label19, 2, 2);
            this.tableLayoutPanel7.Controls.Add(this.label18, 1, 2);
            this.tableLayoutPanel7.Controls.Add(this.HWC_Image3, 2, 0);
            this.tableLayoutPanel7.Controls.Add(this.HWC_Image2, 1, 0);
            this.tableLayoutPanel7.Controls.Add(this.HWC_Image1, 0, 0);
            this.tableLayoutPanel7.Controls.Add(this.label16, 0, 2);
            this.tableLayoutPanel7.Name = "tableLayoutPanel7";
            // 
            // label19
            // 
            resources.ApplyResources(this.label19, "label19");
            this.label19.Name = "label19";
            // 
            // label18
            // 
            resources.ApplyResources(this.label18, "label18");
            this.label18.Name = "label18";
            // 
            // HWC_Image3
            // 
            this.HWC_Image3.BackColor = System.Drawing.Color.Black;
            this.HWC_Image3.BorderColor = System.Drawing.Color.Black;
            resources.ApplyResources(this.HWC_Image3, "HWC_Image3");
            this.HWC_Image3.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.HWC_Image3.Name = "HWC_Image3";
            this.tableLayoutPanel7.SetRowSpan(this.HWC_Image3, 2);
            this.HWC_Image3.WindowSize = new System.Drawing.Size(109, 130);
            // 
            // HWC_Image2
            // 
            this.HWC_Image2.BackColor = System.Drawing.Color.Black;
            this.HWC_Image2.BorderColor = System.Drawing.Color.Black;
            resources.ApplyResources(this.HWC_Image2, "HWC_Image2");
            this.HWC_Image2.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.HWC_Image2.Name = "HWC_Image2";
            this.tableLayoutPanel7.SetRowSpan(this.HWC_Image2, 2);
            this.HWC_Image2.WindowSize = new System.Drawing.Size(109, 130);
            this.HWC_Image2.HMouseDown += new HalconDotNet.HMouseEventHandler(this.HWC_Image2_HMouseDown);
            // 
            // HWC_Image1
            // 
            this.HWC_Image1.BackColor = System.Drawing.Color.Black;
            this.HWC_Image1.BorderColor = System.Drawing.Color.Black;
            resources.ApplyResources(this.HWC_Image1, "HWC_Image1");
            this.HWC_Image1.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.HWC_Image1.Name = "HWC_Image1";
            this.tableLayoutPanel7.SetRowSpan(this.HWC_Image1, 2);
            this.HWC_Image1.WindowSize = new System.Drawing.Size(109, 130);
            this.HWC_Image1.HMouseDown += new HalconDotNet.HMouseEventHandler(this.HWC_Image1_HMouseDown);
            // 
            // label16
            // 
            resources.ApplyResources(this.label16, "label16");
            this.label16.Name = "label16";
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.tableLayoutPanel2.SetColumnSpan(this.groupBox2, 6);
            this.groupBox2.Controls.Add(this.TB_ROIMoveIndex);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.tableLayoutPanel2.SetRowSpan(this.groupBox2, 10);
            this.groupBox2.TabStop = false;
            // 
            // TB_ROIMoveIndex
            // 
            this.TB_ROIMoveIndex.Controls.Add(this.tabPage4);
            this.TB_ROIMoveIndex.Controls.Add(this.tabPage1);
            this.TB_ROIMoveIndex.Controls.Add(this.tabPage2);
            this.TB_ROIMoveIndex.Controls.Add(this.tabPage5);
            resources.ApplyResources(this.TB_ROIMoveIndex, "TB_ROIMoveIndex");
            this.TB_ROIMoveIndex.Name = "TB_ROIMoveIndex";
            this.TB_ROIMoveIndex.SelectedIndex = 0;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.tableLayoutPanel5);
            resources.ApplyResources(this.tabPage4, "tabPage4");
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel5
            // 
            resources.ApplyResources(this.tableLayoutPanel5, "tableLayoutPanel5");
            this.tableLayoutPanel5.Controls.Add(this.NUD_RoiMidColumn, 3, 10);
            this.tableLayoutPanel5.Controls.Add(this.NUD_RoiMidRow, 3, 8);
            this.tableLayoutPanel5.Controls.Add(this.label15, 0, 10);
            this.tableLayoutPanel5.Controls.Add(this.label14, 0, 8);
            this.tableLayoutPanel5.Controls.Add(this.NUD_RoiHeight, 3, 4);
            this.tableLayoutPanel5.Controls.Add(this.NUD_RoiWidth, 3, 2);
            this.tableLayoutPanel5.Controls.Add(this.NUD_RoiNum, 3, 0);
            this.tableLayoutPanel5.Controls.Add(this.label17, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.label12, 0, 6);
            this.tableLayoutPanel5.Controls.Add(this.label13, 0, 4);
            this.tableLayoutPanel5.Controls.Add(this.label11, 0, 2);
            this.tableLayoutPanel5.Controls.Add(this.NUD_RoiAngle, 3, 6);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            // 
            // NUD_RoiMidColumn
            // 
            this.tableLayoutPanel5.SetColumnSpan(this.NUD_RoiMidColumn, 3);
            resources.ApplyResources(this.NUD_RoiMidColumn, "NUD_RoiMidColumn");
            this.NUD_RoiMidColumn.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.NUD_RoiMidColumn.Minimum = new decimal(new int[] {
            100000,
            0,
            0,
            -2147483648});
            this.NUD_RoiMidColumn.Name = "NUD_RoiMidColumn";
            this.tableLayoutPanel5.SetRowSpan(this.NUD_RoiMidColumn, 2);
            this.NUD_RoiMidColumn.ValueChanged += new System.EventHandler(this.NUD_RoiMidColumn_ValueChanged);
            // 
            // NUD_RoiMidRow
            // 
            this.tableLayoutPanel5.SetColumnSpan(this.NUD_RoiMidRow, 3);
            resources.ApplyResources(this.NUD_RoiMidRow, "NUD_RoiMidRow");
            this.NUD_RoiMidRow.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.NUD_RoiMidRow.Minimum = new decimal(new int[] {
            100000,
            0,
            0,
            -2147483648});
            this.NUD_RoiMidRow.Name = "NUD_RoiMidRow";
            this.tableLayoutPanel5.SetRowSpan(this.NUD_RoiMidRow, 2);
            this.NUD_RoiMidRow.ValueChanged += new System.EventHandler(this.NUD_RoiMidRow_ValueChanged);
            // 
            // label15
            // 
            resources.ApplyResources(this.label15, "label15");
            this.tableLayoutPanel5.SetColumnSpan(this.label15, 3);
            this.label15.Name = "label15";
            this.tableLayoutPanel5.SetRowSpan(this.label15, 2);
            // 
            // label14
            // 
            resources.ApplyResources(this.label14, "label14");
            this.tableLayoutPanel5.SetColumnSpan(this.label14, 3);
            this.label14.Name = "label14";
            this.tableLayoutPanel5.SetRowSpan(this.label14, 2);
            // 
            // NUD_RoiHeight
            // 
            this.tableLayoutPanel5.SetColumnSpan(this.NUD_RoiHeight, 3);
            resources.ApplyResources(this.NUD_RoiHeight, "NUD_RoiHeight");
            this.NUD_RoiHeight.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.NUD_RoiHeight.Name = "NUD_RoiHeight";
            this.tableLayoutPanel5.SetRowSpan(this.NUD_RoiHeight, 2);
            this.NUD_RoiHeight.ValueChanged += new System.EventHandler(this.NUD_RoiHeight_ValueChanged);
            // 
            // NUD_RoiWidth
            // 
            this.tableLayoutPanel5.SetColumnSpan(this.NUD_RoiWidth, 3);
            resources.ApplyResources(this.NUD_RoiWidth, "NUD_RoiWidth");
            this.NUD_RoiWidth.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.NUD_RoiWidth.Name = "NUD_RoiWidth";
            this.tableLayoutPanel5.SetRowSpan(this.NUD_RoiWidth, 2);
            this.NUD_RoiWidth.ValueChanged += new System.EventHandler(this.NUD_RoiWidth_ValueChanged);
            // 
            // NUD_RoiNum
            // 
            this.tableLayoutPanel5.SetColumnSpan(this.NUD_RoiNum, 3);
            resources.ApplyResources(this.NUD_RoiNum, "NUD_RoiNum");
            this.NUD_RoiNum.Maximum = new decimal(new int[] {
            150,
            0,
            0,
            0});
            this.NUD_RoiNum.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NUD_RoiNum.Name = "NUD_RoiNum";
            this.tableLayoutPanel5.SetRowSpan(this.NUD_RoiNum, 2);
            this.NUD_RoiNum.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NUD_RoiNum.ValueChanged += new System.EventHandler(this.NUD_RoiNum_ValueChanged);
            // 
            // label17
            // 
            resources.ApplyResources(this.label17, "label17");
            this.tableLayoutPanel5.SetColumnSpan(this.label17, 3);
            this.label17.Name = "label17";
            this.tableLayoutPanel5.SetRowSpan(this.label17, 2);
            // 
            // label12
            // 
            resources.ApplyResources(this.label12, "label12");
            this.tableLayoutPanel5.SetColumnSpan(this.label12, 3);
            this.label12.Name = "label12";
            this.tableLayoutPanel5.SetRowSpan(this.label12, 2);
            // 
            // label13
            // 
            resources.ApplyResources(this.label13, "label13");
            this.tableLayoutPanel5.SetColumnSpan(this.label13, 3);
            this.label13.Name = "label13";
            this.tableLayoutPanel5.SetRowSpan(this.label13, 2);
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.tableLayoutPanel5.SetColumnSpan(this.label11, 3);
            this.label11.Name = "label11";
            this.tableLayoutPanel5.SetRowSpan(this.label11, 2);
            // 
            // NUD_RoiAngle
            // 
            this.tableLayoutPanel5.SetColumnSpan(this.NUD_RoiAngle, 3);
            resources.ApplyResources(this.NUD_RoiAngle, "NUD_RoiAngle");
            this.NUD_RoiAngle.Maximum = new decimal(new int[] {
            180,
            0,
            0,
            0});
            this.NUD_RoiAngle.Minimum = new decimal(new int[] {
            180,
            0,
            0,
            -2147483648});
            this.NUD_RoiAngle.Name = "NUD_RoiAngle";
            this.tableLayoutPanel5.SetRowSpan(this.NUD_RoiAngle, 2);
            this.NUD_RoiAngle.Value = new decimal(new int[] {
            180,
            0,
            0,
            0});
            this.NUD_RoiAngle.ValueChanged += new System.EventHandler(this.NUD_RoiAngle_ValueChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.panel1);
            resources.ApplyResources(this.tabPage1, "tabPage1");
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.NUD_SharpenSigma);
            this.panel1.Controls.Add(this.LB_SharpenSigma);
            this.panel1.Controls.Add(this.NUD_SharpenIterations);
            this.panel1.Controls.Add(this.LB_SharpenIterations);
            this.panel1.Controls.Add(this.NUD_ScaleFactor);
            this.panel1.Controls.Add(this.LB_ScaleFactor);
            this.panel1.Controls.Add(this.NUD_ScaleMean);
            this.panel1.Controls.Add(this.PB_SelectColor);
            this.panel1.Controls.Add(this.LB_ScaleMean);
            this.panel1.Controls.Add(this.RB_ScaleSharpen);
            this.panel1.Controls.Add(this.RB_ProcessNone);
            this.panel1.Controls.Add(this.CB_DefaultLine);
            this.panel1.Controls.Add(this.CB_PeakValleySelect);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.CB_MeasureSelect);
            this.panel1.Controls.Add(this.CB_MeasureTransition);
            this.panel1.Controls.Add(this.NUD_MeasureSigma);
            this.panel1.Controls.Add(this.NUD_MeasureThreshold);
            this.panel1.Controls.Add(this.label23);
            this.panel1.Controls.Add(this.label22);
            this.panel1.Controls.Add(this.label20);
            this.panel1.Controls.Add(this.label21);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // NUD_SharpenSigma
            // 
            resources.ApplyResources(this.NUD_SharpenSigma, "NUD_SharpenSigma");
            this.NUD_SharpenSigma.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.NUD_SharpenSigma.DecimalPlaces = 1;
            this.NUD_SharpenSigma.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.NUD_SharpenSigma.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            65536});
            this.NUD_SharpenSigma.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.NUD_SharpenSigma.Name = "NUD_SharpenSigma";
            this.NUD_SharpenSigma.Value = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.NUD_SharpenSigma.ValueChanged += new System.EventHandler(this.NUD_SharpenSigma_ValueChanged);
            // 
            // LB_SharpenSigma
            // 
            resources.ApplyResources(this.LB_SharpenSigma, "LB_SharpenSigma");
            this.LB_SharpenSigma.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.LB_SharpenSigma.Name = "LB_SharpenSigma";
            // 
            // NUD_SharpenIterations
            // 
            this.NUD_SharpenIterations.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.NUD_SharpenIterations, "NUD_SharpenIterations");
            this.NUD_SharpenIterations.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.NUD_SharpenIterations.Name = "NUD_SharpenIterations";
            this.NUD_SharpenIterations.ValueChanged += new System.EventHandler(this.NUD_SharpenIterations_ValueChanged);
            // 
            // LB_SharpenIterations
            // 
            resources.ApplyResources(this.LB_SharpenIterations, "LB_SharpenIterations");
            this.LB_SharpenIterations.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.LB_SharpenIterations.Name = "LB_SharpenIterations";
            // 
            // NUD_ScaleFactor
            // 
            this.NUD_ScaleFactor.DecimalPlaces = 1;
            resources.ApplyResources(this.NUD_ScaleFactor, "NUD_ScaleFactor");
            this.NUD_ScaleFactor.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.NUD_ScaleFactor.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.NUD_ScaleFactor.Name = "NUD_ScaleFactor";
            this.NUD_ScaleFactor.ValueChanged += new System.EventHandler(this.NUD_ScaleFactor_ValueChanged);
            // 
            // LB_ScaleFactor
            // 
            resources.ApplyResources(this.LB_ScaleFactor, "LB_ScaleFactor");
            this.LB_ScaleFactor.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.LB_ScaleFactor.Name = "LB_ScaleFactor";
            // 
            // NUD_ScaleMean
            // 
            resources.ApplyResources(this.NUD_ScaleMean, "NUD_ScaleMean");
            this.NUD_ScaleMean.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.NUD_ScaleMean.Name = "NUD_ScaleMean";
            this.NUD_ScaleMean.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.NUD_ScaleMean.ValueChanged += new System.EventHandler(this.NUD_ScaleMean_ValueChanged);
            // 
            // PB_SelectColor
            // 
            this.PB_SelectColor.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.PB_SelectColor, "PB_SelectColor");
            this.PB_SelectColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PB_SelectColor.Cursor = System.Windows.Forms.Cursors.Hand;
            this.PB_SelectColor.Name = "PB_SelectColor";
            this.PB_SelectColor.TabStop = false;
            // 
            // LB_ScaleMean
            // 
            resources.ApplyResources(this.LB_ScaleMean, "LB_ScaleMean");
            this.LB_ScaleMean.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.LB_ScaleMean.Name = "LB_ScaleMean";
            // 
            // RB_ScaleSharpen
            // 
            resources.ApplyResources(this.RB_ScaleSharpen, "RB_ScaleSharpen");
            this.RB_ScaleSharpen.Name = "RB_ScaleSharpen";
            this.RB_ScaleSharpen.UseVisualStyleBackColor = true;
            this.RB_ScaleSharpen.CheckedChanged += new System.EventHandler(this.RB_ImageProcess_CheckedChanged);
            // 
            // RB_ProcessNone
            // 
            resources.ApplyResources(this.RB_ProcessNone, "RB_ProcessNone");
            this.RB_ProcessNone.Checked = true;
            this.RB_ProcessNone.Name = "RB_ProcessNone";
            this.RB_ProcessNone.TabStop = true;
            this.RB_ProcessNone.UseVisualStyleBackColor = true;
            this.RB_ProcessNone.CheckedChanged += new System.EventHandler(this.RB_ImageProcess_CheckedChanged);
            // 
            // CB_DefaultLine
            // 
            resources.ApplyResources(this.CB_DefaultLine, "CB_DefaultLine");
            this.CB_DefaultLine.Name = "CB_DefaultLine";
            this.CB_DefaultLine.UseVisualStyleBackColor = true;
            this.CB_DefaultLine.CheckedChanged += new System.EventHandler(this.CB_DefaultLine_CheckedChanged);
            // 
            // CB_PeakValleySelect
            // 
            this.CB_PeakValleySelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.CB_PeakValleySelect, "CB_PeakValleySelect");
            this.CB_PeakValleySelect.FormattingEnabled = true;
            this.CB_PeakValleySelect.Name = "CB_PeakValleySelect";
            this.CB_PeakValleySelect.SelectedIndexChanged += new System.EventHandler(this.CB_PeakValleySelect_SelectedIndexChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // CB_MeasureSelect
            // 
            this.CB_MeasureSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.CB_MeasureSelect, "CB_MeasureSelect");
            this.CB_MeasureSelect.FormattingEnabled = true;
            this.CB_MeasureSelect.Name = "CB_MeasureSelect";
            this.CB_MeasureSelect.SelectedIndexChanged += new System.EventHandler(this.CB_MeasureSelect_SelectedIndexChanged);
            // 
            // CB_MeasureTransition
            // 
            this.CB_MeasureTransition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.CB_MeasureTransition, "CB_MeasureTransition");
            this.CB_MeasureTransition.FormattingEnabled = true;
            this.CB_MeasureTransition.Name = "CB_MeasureTransition";
            this.CB_MeasureTransition.SelectedIndexChanged += new System.EventHandler(this.CB_MeasureTransition_SelectedIndexChanged);
            // 
            // NUD_MeasureSigma
            // 
            resources.ApplyResources(this.NUD_MeasureSigma, "NUD_MeasureSigma");
            this.NUD_MeasureSigma.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.NUD_MeasureSigma.DecimalPlaces = 1;
            this.NUD_MeasureSigma.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.NUD_MeasureSigma.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.NUD_MeasureSigma.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.NUD_MeasureSigma.Name = "NUD_MeasureSigma";
            this.NUD_MeasureSigma.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.NUD_MeasureSigma.ValueChanged += new System.EventHandler(this.NUD_MeasureSigma_ValueChanged);
            // 
            // NUD_MeasureThreshold
            // 
            resources.ApplyResources(this.NUD_MeasureThreshold, "NUD_MeasureThreshold");
            this.NUD_MeasureThreshold.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.NUD_MeasureThreshold.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.NUD_MeasureThreshold.Name = "NUD_MeasureThreshold";
            this.NUD_MeasureThreshold.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.NUD_MeasureThreshold.ValueChanged += new System.EventHandler(this.NUD_MeasureThreshold_ValueChanged);
            // 
            // label23
            // 
            resources.ApplyResources(this.label23, "label23");
            this.label23.Name = "label23";
            // 
            // label22
            // 
            resources.ApplyResources(this.label22, "label22");
            this.label22.Name = "label22";
            // 
            // label20
            // 
            resources.ApplyResources(this.label20, "label20");
            this.label20.Name = "label20";
            // 
            // label21
            // 
            resources.ApplyResources(this.label21, "label21");
            this.label21.Name = "label21";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.panel2);
            resources.ApplyResources(this.tabPage2, "tabPage2");
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.CB_MeasureInterpolation);
            this.panel2.Controls.Add(this.CB_MeasureIORegions);
            this.panel2.Controls.Add(this.NUD_MaxNumIterations);
            this.panel2.Controls.Add(this.NUD_MeasureDistThreshold);
            this.panel2.Controls.Add(this.NUD_MeasureNumInstances);
            this.panel2.Controls.Add(this.NUD_MeasureMinScore);
            this.panel2.Controls.Add(this.label28);
            this.panel2.Controls.Add(this.label29);
            this.panel2.Controls.Add(this.label24);
            this.panel2.Controls.Add(this.label27);
            this.panel2.Controls.Add(this.label25);
            this.panel2.Controls.Add(this.label26);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // CB_MeasureInterpolation
            // 
            this.CB_MeasureInterpolation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.CB_MeasureInterpolation, "CB_MeasureInterpolation");
            this.CB_MeasureInterpolation.FormattingEnabled = true;
            this.CB_MeasureInterpolation.Name = "CB_MeasureInterpolation";
            this.CB_MeasureInterpolation.SelectedIndexChanged += new System.EventHandler(this.CB_MeasureInterpolation_SelectedIndexChanged);
            // 
            // CB_MeasureIORegions
            // 
            this.CB_MeasureIORegions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.CB_MeasureIORegions, "CB_MeasureIORegions");
            this.CB_MeasureIORegions.FormattingEnabled = true;
            this.CB_MeasureIORegions.Name = "CB_MeasureIORegions";
            this.CB_MeasureIORegions.SelectedIndexChanged += new System.EventHandler(this.CB_MeasureIORegions_SelectedIndexChanged);
            // 
            // NUD_MaxNumIterations
            // 
            resources.ApplyResources(this.NUD_MaxNumIterations, "NUD_MaxNumIterations");
            this.NUD_MaxNumIterations.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.NUD_MaxNumIterations.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.NUD_MaxNumIterations.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.NUD_MaxNumIterations.Name = "NUD_MaxNumIterations";
            this.NUD_MaxNumIterations.Value = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.NUD_MaxNumIterations.ValueChanged += new System.EventHandler(this.NUD_MaxNumIterations_ValueChanged);
            // 
            // NUD_MeasureDistThreshold
            // 
            resources.ApplyResources(this.NUD_MeasureDistThreshold, "NUD_MeasureDistThreshold");
            this.NUD_MeasureDistThreshold.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.NUD_MeasureDistThreshold.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.NUD_MeasureDistThreshold.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NUD_MeasureDistThreshold.Name = "NUD_MeasureDistThreshold";
            this.NUD_MeasureDistThreshold.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.NUD_MeasureDistThreshold.ValueChanged += new System.EventHandler(this.NUD_MeasureDistThreshold_ValueChanged);
            // 
            // NUD_MeasureNumInstances
            // 
            resources.ApplyResources(this.NUD_MeasureNumInstances, "NUD_MeasureNumInstances");
            this.NUD_MeasureNumInstances.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.NUD_MeasureNumInstances.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.NUD_MeasureNumInstances.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NUD_MeasureNumInstances.Name = "NUD_MeasureNumInstances";
            this.NUD_MeasureNumInstances.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NUD_MeasureNumInstances.ValueChanged += new System.EventHandler(this.NUD_MeasureNumInstances_ValueChanged);
            // 
            // NUD_MeasureMinScore
            // 
            resources.ApplyResources(this.NUD_MeasureMinScore, "NUD_MeasureMinScore");
            this.NUD_MeasureMinScore.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.NUD_MeasureMinScore.DecimalPlaces = 1;
            this.NUD_MeasureMinScore.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.NUD_MeasureMinScore.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NUD_MeasureMinScore.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.NUD_MeasureMinScore.Name = "NUD_MeasureMinScore";
            this.NUD_MeasureMinScore.Value = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.NUD_MeasureMinScore.ValueChanged += new System.EventHandler(this.NUD_MeasureMinScore_ValueChanged);
            // 
            // label28
            // 
            resources.ApplyResources(this.label28, "label28");
            this.label28.Name = "label28";
            // 
            // label29
            // 
            resources.ApplyResources(this.label29, "label29");
            this.label29.Name = "label29";
            // 
            // label24
            // 
            resources.ApplyResources(this.label24, "label24");
            this.label24.Name = "label24";
            // 
            // label27
            // 
            resources.ApplyResources(this.label27, "label27");
            this.label27.Name = "label27";
            // 
            // label25
            // 
            resources.ApplyResources(this.label25, "label25");
            this.label25.Name = "label25";
            // 
            // label26
            // 
            resources.ApplyResources(this.label26, "label26");
            this.label26.Name = "label26";
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.groupBox4);
            resources.ApplyResources(this.tabPage5, "tabPage5");
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.NUD_RoiStandardColumn);
            this.groupBox4.Controls.Add(this.NUD_RoiStandardRow);
            this.groupBox4.Controls.Add(this.LB_RoiStandardColumn);
            this.groupBox4.Controls.Add(this.LB_RoiStandardRow);
            this.groupBox4.Controls.Add(this.NUD_RoiStandardOffsetColumn);
            this.groupBox4.Controls.Add(this.NUD_RoiStandardOffsetRow);
            this.groupBox4.Controls.Add(this.LB_RoiStandardOffsetColumn);
            this.groupBox4.Controls.Add(this.LB_RoiStandardOffsetRow);
            this.groupBox4.Controls.Add(this.NUD_RoiIncreaseColumn);
            this.groupBox4.Controls.Add(this.NUD_RoiIncreaseRow);
            this.groupBox4.Controls.Add(this.NUD_RoiIncreaseNum);
            this.groupBox4.Controls.Add(this.LB_RoiIncreaseNum);
            this.groupBox4.Controls.Add(this.LB_RoiIncreaseRow);
            this.groupBox4.Controls.Add(this.LB_RoiIncreaseColumn);
            this.groupBox4.Controls.Add(this.RB_IncreaseROI);
            this.groupBox4.Controls.Add(this.RB_StandardROI);
            this.groupBox4.Controls.Add(this.RB_StaticROI);
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // NUD_RoiStandardColumn
            // 
            resources.ApplyResources(this.NUD_RoiStandardColumn, "NUD_RoiStandardColumn");
            this.NUD_RoiStandardColumn.ForeColor = System.Drawing.Color.Black;
            this.NUD_RoiStandardColumn.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.NUD_RoiStandardColumn.Minimum = new decimal(new int[] {
            100000,
            0,
            0,
            -2147483648});
            this.NUD_RoiStandardColumn.Name = "NUD_RoiStandardColumn";
            // 
            // NUD_RoiStandardRow
            // 
            resources.ApplyResources(this.NUD_RoiStandardRow, "NUD_RoiStandardRow");
            this.NUD_RoiStandardRow.ForeColor = System.Drawing.Color.Black;
            this.NUD_RoiStandardRow.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.NUD_RoiStandardRow.Minimum = new decimal(new int[] {
            100000,
            0,
            0,
            -2147483648});
            this.NUD_RoiStandardRow.Name = "NUD_RoiStandardRow";
            // 
            // LB_RoiStandardColumn
            // 
            resources.ApplyResources(this.LB_RoiStandardColumn, "LB_RoiStandardColumn");
            this.LB_RoiStandardColumn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.LB_RoiStandardColumn.Name = "LB_RoiStandardColumn";
            // 
            // LB_RoiStandardRow
            // 
            resources.ApplyResources(this.LB_RoiStandardRow, "LB_RoiStandardRow");
            this.LB_RoiStandardRow.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.LB_RoiStandardRow.Name = "LB_RoiStandardRow";
            // 
            // NUD_RoiStandardOffsetColumn
            // 
            resources.ApplyResources(this.NUD_RoiStandardOffsetColumn, "NUD_RoiStandardOffsetColumn");
            this.NUD_RoiStandardOffsetColumn.ForeColor = System.Drawing.Color.Black;
            this.NUD_RoiStandardOffsetColumn.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.NUD_RoiStandardOffsetColumn.Minimum = new decimal(new int[] {
            100000,
            0,
            0,
            -2147483648});
            this.NUD_RoiStandardOffsetColumn.Name = "NUD_RoiStandardOffsetColumn";
            // 
            // NUD_RoiStandardOffsetRow
            // 
            resources.ApplyResources(this.NUD_RoiStandardOffsetRow, "NUD_RoiStandardOffsetRow");
            this.NUD_RoiStandardOffsetRow.ForeColor = System.Drawing.Color.Black;
            this.NUD_RoiStandardOffsetRow.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.NUD_RoiStandardOffsetRow.Minimum = new decimal(new int[] {
            100000,
            0,
            0,
            -2147483648});
            this.NUD_RoiStandardOffsetRow.Name = "NUD_RoiStandardOffsetRow";
            // 
            // LB_RoiStandardOffsetColumn
            // 
            resources.ApplyResources(this.LB_RoiStandardOffsetColumn, "LB_RoiStandardOffsetColumn");
            this.LB_RoiStandardOffsetColumn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.LB_RoiStandardOffsetColumn.Name = "LB_RoiStandardOffsetColumn";
            // 
            // LB_RoiStandardOffsetRow
            // 
            resources.ApplyResources(this.LB_RoiStandardOffsetRow, "LB_RoiStandardOffsetRow");
            this.LB_RoiStandardOffsetRow.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.LB_RoiStandardOffsetRow.Name = "LB_RoiStandardOffsetRow";
            // 
            // NUD_RoiIncreaseColumn
            // 
            resources.ApplyResources(this.NUD_RoiIncreaseColumn, "NUD_RoiIncreaseColumn");
            this.NUD_RoiIncreaseColumn.ForeColor = System.Drawing.Color.Black;
            this.NUD_RoiIncreaseColumn.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.NUD_RoiIncreaseColumn.Minimum = new decimal(new int[] {
            100000,
            0,
            0,
            -2147483648});
            this.NUD_RoiIncreaseColumn.Name = "NUD_RoiIncreaseColumn";
            // 
            // NUD_RoiIncreaseRow
            // 
            resources.ApplyResources(this.NUD_RoiIncreaseRow, "NUD_RoiIncreaseRow");
            this.NUD_RoiIncreaseRow.ForeColor = System.Drawing.Color.Black;
            this.NUD_RoiIncreaseRow.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.NUD_RoiIncreaseRow.Minimum = new decimal(new int[] {
            100000,
            0,
            0,
            -2147483648});
            this.NUD_RoiIncreaseRow.Name = "NUD_RoiIncreaseRow";
            // 
            // NUD_RoiIncreaseNum
            // 
            resources.ApplyResources(this.NUD_RoiIncreaseNum, "NUD_RoiIncreaseNum");
            this.NUD_RoiIncreaseNum.ForeColor = System.Drawing.Color.Black;
            this.NUD_RoiIncreaseNum.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.NUD_RoiIncreaseNum.Name = "NUD_RoiIncreaseNum";
            // 
            // LB_RoiIncreaseNum
            // 
            resources.ApplyResources(this.LB_RoiIncreaseNum, "LB_RoiIncreaseNum");
            this.LB_RoiIncreaseNum.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.LB_RoiIncreaseNum.Name = "LB_RoiIncreaseNum";
            // 
            // LB_RoiIncreaseRow
            // 
            resources.ApplyResources(this.LB_RoiIncreaseRow, "LB_RoiIncreaseRow");
            this.LB_RoiIncreaseRow.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.LB_RoiIncreaseRow.Name = "LB_RoiIncreaseRow";
            // 
            // LB_RoiIncreaseColumn
            // 
            resources.ApplyResources(this.LB_RoiIncreaseColumn, "LB_RoiIncreaseColumn");
            this.LB_RoiIncreaseColumn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.LB_RoiIncreaseColumn.Name = "LB_RoiIncreaseColumn";
            // 
            // RB_IncreaseROI
            // 
            resources.ApplyResources(this.RB_IncreaseROI, "RB_IncreaseROI");
            this.RB_IncreaseROI.Name = "RB_IncreaseROI";
            this.RB_IncreaseROI.UseVisualStyleBackColor = true;
            // 
            // RB_StandardROI
            // 
            resources.ApplyResources(this.RB_StandardROI, "RB_StandardROI");
            this.RB_StandardROI.Name = "RB_StandardROI";
            this.RB_StandardROI.UseVisualStyleBackColor = true;
            // 
            // RB_StaticROI
            // 
            resources.ApplyResources(this.RB_StaticROI, "RB_StaticROI");
            this.RB_StaticROI.Checked = true;
            this.RB_StaticROI.Name = "RB_StaticROI";
            this.RB_StaticROI.TabStop = true;
            this.RB_StaticROI.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.tableLayoutPanel2.SetColumnSpan(this.groupBox3, 6);
            this.groupBox3.Controls.Add(this.TrB_ROIIndex);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.tableLayoutPanel2.SetRowSpan(this.groupBox3, 5);
            this.groupBox3.TabStop = false;
            // 
            // TrB_ROIIndex
            // 
            this.TrB_ROIIndex.AllowDrop = true;
            this.TrB_ROIIndex.BackColor = System.Drawing.Color.White;
            this.TrB_ROIIndex.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.TrB_ROIIndex, "TrB_ROIIndex");
            this.TrB_ROIIndex.LargeChange = 3;
            this.TrB_ROIIndex.Minimum = 1;
            this.TrB_ROIIndex.Name = "TrB_ROIIndex";
            this.TrB_ROIIndex.Value = 1;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // FormMeasureLine
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.tableLayoutPanel2);
            this.Name = "FormMeasureLine";
            this.Load += new System.EventHandler(this.FormMeasureLine_Load);
            this.Controls.SetChildIndex(this.tableLayoutPanel2, 0);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LoadImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.FullScrean)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.tableLayoutPanel7.ResumeLayout(false);
            this.tableLayoutPanel7.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.TB_ROIMoveIndex.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_RoiMidColumn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_RoiMidRow)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_RoiHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_RoiWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_RoiNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_RoiAngle)).EndInit();
            this.tabPage1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_SharpenSigma)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_SharpenIterations)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_ScaleFactor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_ScaleMean)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PB_SelectColor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_MeasureSigma)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_MeasureThreshold)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_MaxNumIterations)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_MeasureDistThreshold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_MeasureNumInstances)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_MeasureMinScore)).EndInit();
            this.tabPage5.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_RoiStandardColumn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_RoiStandardRow)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_RoiStandardOffsetColumn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_RoiStandardOffsetRow)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_RoiIncreaseColumn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_RoiIncreaseRow)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUD_RoiIncreaseNum)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TrB_ROIIndex)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox TB_PositionAndGray;
        public System.Windows.Forms.Label LB_ModleName;
        private System.Windows.Forms.PictureBox FullScrean;
        private System.Windows.Forms.PictureBox LoadImage;
        private HalconDotNet.HWindowControl HWC_Display;
        private System.Windows.Forms.TabControl TB_ROIMoveIndex;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel7;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label18;
        private HalconDotNet.HWindowControl HWC_Image3;
        private HalconDotNet.HWindowControl HWC_Image2;
        private HalconDotNet.HWindowControl HWC_Image1;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TrackBar TrB_ROIIndex;
        public System.Windows.Forms.Label LB_IMageSize;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.GroupBox groupBox4;
        public System.Windows.Forms.NumericUpDown NUD_RoiStandardColumn;
        public System.Windows.Forms.NumericUpDown NUD_RoiStandardRow;
        private System.Windows.Forms.Label LB_RoiStandardColumn;
        private System.Windows.Forms.Label LB_RoiStandardRow;
        public System.Windows.Forms.NumericUpDown NUD_RoiStandardOffsetColumn;
        public System.Windows.Forms.NumericUpDown NUD_RoiStandardOffsetRow;
        private System.Windows.Forms.Label LB_RoiStandardOffsetColumn;
        private System.Windows.Forms.Label LB_RoiStandardOffsetRow;
        public System.Windows.Forms.NumericUpDown NUD_RoiIncreaseColumn;
        public System.Windows.Forms.NumericUpDown NUD_RoiIncreaseRow;
        public System.Windows.Forms.NumericUpDown NUD_RoiIncreaseNum;
        private System.Windows.Forms.Label LB_RoiIncreaseNum;
        private System.Windows.Forms.Label LB_RoiIncreaseRow;
        private System.Windows.Forms.Label LB_RoiIncreaseColumn;
        private System.Windows.Forms.RadioButton RB_IncreaseROI;
        private System.Windows.Forms.RadioButton RB_StandardROI;
        private System.Windows.Forms.RadioButton RB_StaticROI;
        private System.Windows.Forms.NumericUpDown NUD_RoiMidColumn;
        private System.Windows.Forms.NumericUpDown NUD_RoiMidRow;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.NumericUpDown NUD_RoiHeight;
        private System.Windows.Forms.NumericUpDown NUD_RoiWidth;
        private System.Windows.Forms.NumericUpDown NUD_RoiNum;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.NumericUpDown NUD_RoiAngle;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox CB_MeasureSelect;
        private System.Windows.Forms.ComboBox CB_MeasureTransition;
        private System.Windows.Forms.NumericUpDown NUD_MeasureSigma;
        public System.Windows.Forms.NumericUpDown NUD_MeasureThreshold;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ComboBox CB_MeasureInterpolation;
        private System.Windows.Forms.ComboBox CB_MeasureIORegions;
        private System.Windows.Forms.NumericUpDown NUD_MaxNumIterations;
        private System.Windows.Forms.NumericUpDown NUD_MeasureDistThreshold;
        private System.Windows.Forms.NumericUpDown NUD_MeasureNumInstances;
        private System.Windows.Forms.NumericUpDown NUD_MeasureMinScore;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ComboBox CB_PeakValleySelect;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox CB_DefaultLine;
        public System.Windows.Forms.NumericUpDown NUD_SharpenSigma;
        private System.Windows.Forms.Label LB_SharpenSigma;
        public System.Windows.Forms.NumericUpDown NUD_SharpenIterations;
        private System.Windows.Forms.Label LB_SharpenIterations;
        public System.Windows.Forms.NumericUpDown NUD_ScaleFactor;
        private System.Windows.Forms.Label LB_ScaleFactor;
        public System.Windows.Forms.NumericUpDown NUD_ScaleMean;
        private System.Windows.Forms.PictureBox PB_SelectColor;
        private System.Windows.Forms.Label LB_ScaleMean;
        private System.Windows.Forms.RadioButton RB_ScaleSharpen;
        private System.Windows.Forms.RadioButton RB_ProcessNone;
    }
}
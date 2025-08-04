namespace Frame_Core
{
    partial class Form_Checkerboard
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Checkerboard));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.LoadImage = new System.Windows.Forms.PictureBox();
            this.TB_PositionAndGray = new System.Windows.Forms.TextBox();
            this.FullScrean = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel_Image1 = new System.Windows.Forms.TableLayoutPanel();
            this.HWC_Display = new HalconDotNet.HWindowControl();
            this.btn_Run = new System.Windows.Forms.Button();
            this.btn_Next = new System.Windows.Forms.Button();
            this.btn_Preview = new System.Windows.Forms.Button();
            this.tableLayoutPanel_Main = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel_MinImage = new System.Windows.Forms.TableLayoutPanel();
            this.label19 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.HWC_Image3 = new HalconDotNet.HWindowControl();
            this.HWC_Image2 = new HalconDotNet.HWindowControl();
            this.HWC_Image1 = new HalconDotNet.HWindowControl();
            this.label16 = new System.Windows.Forms.Label();
            this.dgv_Target = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btn_Calculate = new System.Windows.Forms.Button();
            this.nud_CalibrationPointNum = new System.Windows.Forms.NumericUpDown();
            this.btn_Calibration = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btn_Transition = new System.Windows.Forms.Button();
            this.nud_WorldCoordX = new System.Windows.Forms.NumericUpDown();
            this.nud_CamCoordX = new System.Windows.Forms.NumericUpDown();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.nud_WorldCoordY = new System.Windows.Forms.NumericUpDown();
            this.nud_CamCoordY = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.btn_Save = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.nud_CheckerboardBoxSideLengthPix_X = new System.Windows.Forms.NumericUpDown();
            this.label17 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.nud_MesureThreshold = new System.Windows.Forms.NumericUpDown();
            this.label22 = new System.Windows.Forms.Label();
            this.cbx_MeasureSelect = new System.Windows.Forms.ComboBox();
            this.cbx_MeasureTransition = new System.Windows.Forms.ComboBox();
            this.nud_CheckerboardBoxMaxThd = new System.Windows.Forms.NumericUpDown();
            this.nud_CheckerboardBoxMinThd = new System.Windows.Forms.NumericUpDown();
            this.label21 = new System.Windows.Forms.Label();
            this.dgv_Original = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label8 = new System.Windows.Forms.Label();
            this.nud_CheckerboardBoxSideLengthPix_Y = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.LoadImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.FullScrean)).BeginInit();
            this.tableLayoutPanel_Image1.SuspendLayout();
            this.tableLayoutPanel_Main.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel_MinImage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Target)).BeginInit();
            this.panel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_CalibrationPointNum)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_WorldCoordX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_CamCoordX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_WorldCoordY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_CamCoordY)).BeginInit();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_CheckerboardBoxSideLengthPix_X)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_MesureThreshold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_CheckerboardBoxMaxThd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_CheckerboardBoxMinThd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Original)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_CheckerboardBoxSideLengthPix_Y)).BeginInit();
            this.SuspendLayout();
            // 
            // LoadImage
            // 
            this.LoadImage.BackColor = System.Drawing.SystemColors.Control;
            this.LoadImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LoadImage.Image = ((System.Drawing.Image)(resources.GetObject("LoadImage.Image")));
            this.LoadImage.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.LoadImage.Location = new System.Drawing.Point(3, 453);
            this.LoadImage.Name = "LoadImage";
            this.LoadImage.Size = new System.Drawing.Size(32, 29);
            this.LoadImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.LoadImage.TabIndex = 11;
            this.LoadImage.TabStop = false;
            this.LoadImage.Click += new System.EventHandler(this.LoadImage_Click);
            // 
            // TB_PositionAndGray
            // 
            this.TB_PositionAndGray.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.TB_PositionAndGray.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tableLayoutPanel_Image1.SetColumnSpan(this.TB_PositionAndGray, 8);
            this.TB_PositionAndGray.Cursor = System.Windows.Forms.Cursors.Default;
            this.TB_PositionAndGray.Font = new System.Drawing.Font("黑体", 12F);
            this.TB_PositionAndGray.ForeColor = System.Drawing.Color.White;
            this.TB_PositionAndGray.Location = new System.Drawing.Point(5, 8);
            this.TB_PositionAndGray.Margin = new System.Windows.Forms.Padding(5, 8, 5, 6);
            this.TB_PositionAndGray.Name = "TB_PositionAndGray";
            this.TB_PositionAndGray.ShortcutsEnabled = false;
            this.TB_PositionAndGray.Size = new System.Drawing.Size(206, 19);
            this.TB_PositionAndGray.TabIndex = 9;
            this.TB_PositionAndGray.TabStop = false;
            this.TB_PositionAndGray.Text = "R: —  C: —  Value: — ";
            // 
            // FullScrean
            // 
            this.FullScrean.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.FullScrean.BackColor = System.Drawing.SystemColors.Control;
            this.FullScrean.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.FullScrean.Image = ((System.Drawing.Image)(resources.GetObject("FullScrean.Image")));
            this.FullScrean.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.FullScrean.Location = new System.Drawing.Point(505, 3);
            this.FullScrean.Name = "FullScrean";
            this.FullScrean.Size = new System.Drawing.Size(32, 29);
            this.FullScrean.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.FullScrean.TabIndex = 10;
            this.FullScrean.TabStop = false;
            // 
            // tableLayoutPanel_Image1
            // 
            this.tableLayoutPanel_Image1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.tableLayoutPanel_Image1.ColumnCount = 12;
            this.tableLayoutPanel_Main.SetColumnSpan(this.tableLayoutPanel_Image1, 4);
            this.tableLayoutPanel_Image1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666667F));
            this.tableLayoutPanel_Image1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666667F));
            this.tableLayoutPanel_Image1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666667F));
            this.tableLayoutPanel_Image1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666667F));
            this.tableLayoutPanel_Image1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666667F));
            this.tableLayoutPanel_Image1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666667F));
            this.tableLayoutPanel_Image1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666667F));
            this.tableLayoutPanel_Image1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666667F));
            this.tableLayoutPanel_Image1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666667F));
            this.tableLayoutPanel_Image1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666667F));
            this.tableLayoutPanel_Image1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666667F));
            this.tableLayoutPanel_Image1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666667F));
            this.tableLayoutPanel_Image1.Controls.Add(this.HWC_Display, 0, 1);
            this.tableLayoutPanel_Image1.Controls.Add(this.FullScrean, 11, 0);
            this.tableLayoutPanel_Image1.Controls.Add(this.LoadImage, 0, 2);
            this.tableLayoutPanel_Image1.Controls.Add(this.TB_PositionAndGray, 0, 0);
            this.tableLayoutPanel_Image1.Controls.Add(this.btn_Run, 10, 2);
            this.tableLayoutPanel_Image1.Controls.Add(this.btn_Next, 11, 2);
            this.tableLayoutPanel_Image1.Controls.Add(this.btn_Preview, 9, 2);
            this.tableLayoutPanel_Image1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel_Image1.Location = new System.Drawing.Point(8, 5);
            this.tableLayoutPanel_Image1.Margin = new System.Windows.Forms.Padding(8, 5, 8, 5);
            this.tableLayoutPanel_Image1.Name = "tableLayoutPanel_Image1";
            this.tableLayoutPanel_Image1.RowCount = 3;
            this.tableLayoutPanel_Main.SetRowSpan(this.tableLayoutPanel_Image1, 11);
            this.tableLayoutPanel_Image1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel_Image1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_Image1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel_Image1.Size = new System.Drawing.Size(540, 485);
            this.tableLayoutPanel_Image1.TabIndex = 0;
            // 
            // HWC_Display
            // 
            this.HWC_Display.BackColor = System.Drawing.Color.White;
            this.HWC_Display.BorderColor = System.Drawing.Color.White;
            this.tableLayoutPanel_Image1.SetColumnSpan(this.HWC_Display, 12);
            this.HWC_Display.Dock = System.Windows.Forms.DockStyle.Fill;
            this.HWC_Display.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.HWC_Display.Location = new System.Drawing.Point(3, 38);
            this.HWC_Display.Name = "HWC_Display";
            this.HWC_Display.Size = new System.Drawing.Size(534, 409);
            this.HWC_Display.TabIndex = 12;
            this.HWC_Display.WindowSize = new System.Drawing.Size(534, 409);
            // 
            // btn_Run
            // 
            this.btn_Run.BackColor = System.Drawing.SystemColors.Control;
            this.btn_Run.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn_Run.BackgroundImage")));
            this.btn_Run.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btn_Run.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold);
            this.btn_Run.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Run.Location = new System.Drawing.Point(453, 453);
            this.btn_Run.Name = "btn_Run";
            this.btn_Run.Size = new System.Drawing.Size(30, 29);
            this.btn_Run.TabIndex = 13;
            this.btn_Run.UseVisualStyleBackColor = false;
            this.btn_Run.Click += new System.EventHandler(this.btn_Run_Click);
            // 
            // btn_Next
            // 
            this.btn_Next.BackColor = System.Drawing.SystemColors.Control;
            this.btn_Next.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn_Next.BackgroundImage")));
            this.btn_Next.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btn_Next.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold);
            this.btn_Next.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Next.Location = new System.Drawing.Point(498, 453);
            this.btn_Next.Name = "btn_Next";
            this.btn_Next.Size = new System.Drawing.Size(34, 29);
            this.btn_Next.TabIndex = 15;
            this.btn_Next.UseVisualStyleBackColor = false;
            this.btn_Next.Click += new System.EventHandler(this.btn_Next_Click);
            // 
            // btn_Preview
            // 
            this.btn_Preview.BackColor = System.Drawing.SystemColors.Control;
            this.btn_Preview.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btn_Preview.BackgroundImage")));
            this.btn_Preview.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btn_Preview.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold);
            this.btn_Preview.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btn_Preview.Location = new System.Drawing.Point(408, 453);
            this.btn_Preview.Name = "btn_Preview";
            this.btn_Preview.Size = new System.Drawing.Size(30, 29);
            this.btn_Preview.TabIndex = 14;
            this.btn_Preview.UseVisualStyleBackColor = false;
            this.btn_Preview.Click += new System.EventHandler(this.btn_Preview_Click);
            // 
            // tableLayoutPanel_Main
            // 
            this.tableLayoutPanel_Main.ColumnCount = 10;
            this.tableLayoutPanel_Main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel_Main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel_Main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel_Main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel_Main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel_Main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel_Main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel_Main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel_Main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel_Main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel_Main.Controls.Add(this.groupBox1, 0, 11);
            this.tableLayoutPanel_Main.Controls.Add(this.dgv_Target, 5, 0);
            this.tableLayoutPanel_Main.Controls.Add(this.tableLayoutPanel_Image1, 0, 0);
            this.tableLayoutPanel_Main.Controls.Add(this.panel1, 6, 0);
            this.tableLayoutPanel_Main.Controls.Add(this.dgv_Original, 4, 0);
            this.tableLayoutPanel_Main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel_Main.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel_Main.Name = "tableLayoutPanel_Main";
            this.tableLayoutPanel_Main.RowCount = 15;
            this.tableLayoutPanel_Main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.666667F));
            this.tableLayoutPanel_Main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.666667F));
            this.tableLayoutPanel_Main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.666667F));
            this.tableLayoutPanel_Main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.666667F));
            this.tableLayoutPanel_Main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.666667F));
            this.tableLayoutPanel_Main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.666667F));
            this.tableLayoutPanel_Main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.666667F));
            this.tableLayoutPanel_Main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.666667F));
            this.tableLayoutPanel_Main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.666667F));
            this.tableLayoutPanel_Main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.666667F));
            this.tableLayoutPanel_Main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.666667F));
            this.tableLayoutPanel_Main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.666667F));
            this.tableLayoutPanel_Main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.666667F));
            this.tableLayoutPanel_Main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.666667F));
            this.tableLayoutPanel_Main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.666667F));
            this.tableLayoutPanel_Main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel_Main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel_Main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel_Main.Size = new System.Drawing.Size(1390, 683);
            this.tableLayoutPanel_Main.TabIndex = 18;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.tableLayoutPanel_Main.SetColumnSpan(this.groupBox1, 4);
            this.groupBox1.Controls.Add(this.tableLayoutPanel_MinImage);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.groupBox1.Location = new System.Drawing.Point(3, 498);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(8, 5, 8, 5);
            this.tableLayoutPanel_Main.SetRowSpan(this.groupBox1, 4);
            this.groupBox1.Size = new System.Drawing.Size(550, 182);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "图形变量";
            // 
            // tableLayoutPanel_MinImage
            // 
            this.tableLayoutPanel_MinImage.ColumnCount = 5;
            this.tableLayoutPanel_MinImage.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel_MinImage.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel_MinImage.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel_MinImage.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel_MinImage.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel_MinImage.Controls.Add(this.label19, 2, 2);
            this.tableLayoutPanel_MinImage.Controls.Add(this.label18, 1, 2);
            this.tableLayoutPanel_MinImage.Controls.Add(this.HWC_Image3, 2, 0);
            this.tableLayoutPanel_MinImage.Controls.Add(this.HWC_Image2, 1, 0);
            this.tableLayoutPanel_MinImage.Controls.Add(this.HWC_Image1, 0, 0);
            this.tableLayoutPanel_MinImage.Controls.Add(this.label16, 0, 2);
            this.tableLayoutPanel_MinImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel_MinImage.Location = new System.Drawing.Point(8, 27);
            this.tableLayoutPanel_MinImage.Name = "tableLayoutPanel_MinImage";
            this.tableLayoutPanel_MinImage.RowCount = 3;
            this.tableLayoutPanel_MinImage.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel_MinImage.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel_MinImage.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel_MinImage.Size = new System.Drawing.Size(534, 150);
            this.tableLayoutPanel_MinImage.TabIndex = 0;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label19.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label19.Location = new System.Drawing.Point(215, 120);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(100, 30);
            this.label19.TabIndex = 7;
            this.label19.Text = "结果图";
            this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label18.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label18.Location = new System.Drawing.Point(109, 120);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(100, 30);
            this.label18.TabIndex = 5;
            this.label18.Text = "图像处理";
            this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // HWC_Image3
            // 
            this.HWC_Image3.BackColor = System.Drawing.Color.Black;
            this.HWC_Image3.BorderColor = System.Drawing.Color.Black;
            this.HWC_Image3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.HWC_Image3.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.HWC_Image3.Location = new System.Drawing.Point(215, 3);
            this.HWC_Image3.Name = "HWC_Image3";
            this.tableLayoutPanel_MinImage.SetRowSpan(this.HWC_Image3, 2);
            this.HWC_Image3.Size = new System.Drawing.Size(100, 114);
            this.HWC_Image3.TabIndex = 2;
            this.HWC_Image3.WindowSize = new System.Drawing.Size(100, 114);
            // 
            // HWC_Image2
            // 
            this.HWC_Image2.BackColor = System.Drawing.Color.Black;
            this.HWC_Image2.BorderColor = System.Drawing.Color.Black;
            this.HWC_Image2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.HWC_Image2.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.HWC_Image2.Location = new System.Drawing.Point(109, 3);
            this.HWC_Image2.Name = "HWC_Image2";
            this.tableLayoutPanel_MinImage.SetRowSpan(this.HWC_Image2, 2);
            this.HWC_Image2.Size = new System.Drawing.Size(100, 114);
            this.HWC_Image2.TabIndex = 1;
            this.HWC_Image2.WindowSize = new System.Drawing.Size(100, 114);
            // 
            // HWC_Image1
            // 
            this.HWC_Image1.BackColor = System.Drawing.Color.Black;
            this.HWC_Image1.BorderColor = System.Drawing.Color.Black;
            this.HWC_Image1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.HWC_Image1.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.HWC_Image1.Location = new System.Drawing.Point(3, 3);
            this.HWC_Image1.Name = "HWC_Image1";
            this.tableLayoutPanel_MinImage.SetRowSpan(this.HWC_Image1, 2);
            this.HWC_Image1.Size = new System.Drawing.Size(100, 114);
            this.HWC_Image1.TabIndex = 0;
            this.HWC_Image1.WindowSize = new System.Drawing.Size(100, 114);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label16.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label16.Location = new System.Drawing.Point(3, 120);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(100, 30);
            this.label16.TabIndex = 3;
            this.label16.Text = "原图";
            this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // dgv_Target
            // 
            this.dgv_Target.AllowUserToAddRows = false;
            this.dgv_Target.AllowUserToDeleteRows = false;
            this.dgv_Target.AllowUserToOrderColumns = true;
            this.dgv_Target.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgv_Target.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgv_Target.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_Target.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn7,
            this.dataGridViewTextBoxColumn8});
            this.dgv_Target.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv_Target.Location = new System.Drawing.Point(698, 3);
            this.dgv_Target.Name = "dgv_Target";
            this.dgv_Target.RowHeadersVisible = false;
            this.tableLayoutPanel_Main.SetRowSpan(this.dgv_Target, 15);
            this.dgv_Target.RowTemplate.Height = 23;
            this.dgv_Target.Size = new System.Drawing.Size(133, 677);
            this.dgv_Target.TabIndex = 5;
            // 
            // dataGridViewTextBoxColumn7
            // 
            this.dataGridViewTextBoxColumn7.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn7.HeaderText = "目标Row";
            this.dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
            this.dataGridViewTextBoxColumn7.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dataGridViewTextBoxColumn8
            // 
            this.dataGridViewTextBoxColumn8.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn8.HeaderText = "目标Col";
            this.dataGridViewTextBoxColumn8.Name = "dataGridViewTextBoxColumn8";
            this.dataGridViewTextBoxColumn8.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // panel1
            // 
            this.tableLayoutPanel_Main.SetColumnSpan(this.panel1, 4);
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Controls.Add(this.groupBox3);
            this.panel1.Controls.Add(this.btn_Save);
            this.panel1.Controls.Add(this.groupBox4);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(837, 3);
            this.panel1.Name = "panel1";
            this.tableLayoutPanel_Main.SetRowSpan(this.panel1, 15);
            this.panel1.Size = new System.Drawing.Size(550, 677);
            this.panel1.TabIndex = 4;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.btn_Calculate);
            this.groupBox2.Controls.Add(this.nud_CalibrationPointNum);
            this.groupBox2.Controls.Add(this.btn_Calibration);
            this.groupBox2.Location = new System.Drawing.Point(9, 10);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(538, 205);
            this.groupBox2.TabIndex = 23;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "标定";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 22;
            this.label2.Text = "标定点数";
            // 
            // btn_Calculate
            // 
            this.btn_Calculate.Location = new System.Drawing.Point(21, 68);
            this.btn_Calculate.Name = "btn_Calculate";
            this.btn_Calculate.Size = new System.Drawing.Size(510, 46);
            this.btn_Calculate.TabIndex = 17;
            this.btn_Calculate.Text = "计算棋盘格方格坐标";
            this.btn_Calculate.UseVisualStyleBackColor = true;
            this.btn_Calculate.Click += new System.EventHandler(this.btn_Calculate_Click);
            // 
            // nud_CalibrationPointNum
            // 
            this.nud_CalibrationPointNum.Location = new System.Drawing.Point(86, 21);
            this.nud_CalibrationPointNum.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nud_CalibrationPointNum.Name = "nud_CalibrationPointNum";
            this.nud_CalibrationPointNum.Size = new System.Drawing.Size(109, 21);
            this.nud_CalibrationPointNum.TabIndex = 21;
            // 
            // btn_Calibration
            // 
            this.btn_Calibration.Location = new System.Drawing.Point(21, 133);
            this.btn_Calibration.Name = "btn_Calibration";
            this.btn_Calibration.Size = new System.Drawing.Size(510, 46);
            this.btn_Calibration.TabIndex = 18;
            this.btn_Calibration.Text = "标定";
            this.btn_Calibration.UseVisualStyleBackColor = true;
            this.btn_Calibration.Click += new System.EventHandler(this.btn_Calibration_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btn_Transition);
            this.groupBox3.Controls.Add(this.nud_WorldCoordX);
            this.groupBox3.Controls.Add(this.nud_CamCoordX);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Controls.Add(this.nud_WorldCoordY);
            this.groupBox3.Controls.Add(this.nud_CamCoordY);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Location = new System.Drawing.Point(9, 228);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(538, 115);
            this.groupBox3.TabIndex = 20;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "测试";
            // 
            // btn_Transition
            // 
            this.btn_Transition.Location = new System.Drawing.Point(442, 37);
            this.btn_Transition.Name = "btn_Transition";
            this.btn_Transition.Size = new System.Drawing.Size(89, 46);
            this.btn_Transition.TabIndex = 17;
            this.btn_Transition.Text = "转换";
            this.btn_Transition.UseVisualStyleBackColor = true;
            this.btn_Transition.Click += new System.EventHandler(this.btn_Transition_Click);
            // 
            // nud_WorldCoordX
            // 
            this.nud_WorldCoordX.DecimalPlaces = 3;
            this.nud_WorldCoordX.Location = new System.Drawing.Point(320, 67);
            this.nud_WorldCoordX.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.nud_WorldCoordX.Minimum = new decimal(new int[] {
            999999,
            0,
            0,
            -2147483648});
            this.nud_WorldCoordX.Name = "nud_WorldCoordX";
            this.nud_WorldCoordX.Size = new System.Drawing.Size(109, 21);
            this.nud_WorldCoordX.TabIndex = 23;
            // 
            // nud_CamCoordX
            // 
            this.nud_CamCoordX.DecimalPlaces = 3;
            this.nud_CamCoordX.Location = new System.Drawing.Point(320, 32);
            this.nud_CamCoordX.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.nud_CamCoordX.Minimum = new decimal(new int[] {
            999999,
            0,
            0,
            -2147483648});
            this.nud_CamCoordX.Name = "nud_CamCoordX";
            this.nud_CamCoordX.Size = new System.Drawing.Size(109, 21);
            this.nud_CamCoordX.TabIndex = 22;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(240, 72);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(77, 12);
            this.label11.TabIndex = 21;
            this.label11.Text = "世界坐标X(C)";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(240, 37);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(77, 12);
            this.label12.TabIndex = 20;
            this.label12.Text = "相机坐标X(C)";
            // 
            // nud_WorldCoordY
            // 
            this.nud_WorldCoordY.DecimalPlaces = 3;
            this.nud_WorldCoordY.Location = new System.Drawing.Point(106, 67);
            this.nud_WorldCoordY.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.nud_WorldCoordY.Minimum = new decimal(new int[] {
            999999,
            0,
            0,
            -2147483648});
            this.nud_WorldCoordY.Name = "nud_WorldCoordY";
            this.nud_WorldCoordY.Size = new System.Drawing.Size(109, 21);
            this.nud_WorldCoordY.TabIndex = 19;
            // 
            // nud_CamCoordY
            // 
            this.nud_CamCoordY.DecimalPlaces = 3;
            this.nud_CamCoordY.Location = new System.Drawing.Point(106, 32);
            this.nud_CamCoordY.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.nud_CamCoordY.Minimum = new decimal(new int[] {
            999999,
            0,
            0,
            -2147483648});
            this.nud_CamCoordY.Name = "nud_CamCoordY";
            this.nud_CamCoordY.Size = new System.Drawing.Size(109, 21);
            this.nud_CamCoordY.TabIndex = 18;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(24, 72);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(77, 12);
            this.label10.TabIndex = 17;
            this.label10.Text = "世界坐标Y(R)";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(24, 37);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(77, 12);
            this.label9.TabIndex = 16;
            this.label9.Text = "相机坐标Y(R)";
            // 
            // btn_Save
            // 
            this.btn_Save.Location = new System.Drawing.Point(451, 622);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(89, 46);
            this.btn_Save.TabIndex = 19;
            this.btn_Save.Text = "保存参数";
            this.btn_Save.UseVisualStyleBackColor = true;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label8);
            this.groupBox4.Controls.Add(this.nud_CheckerboardBoxSideLengthPix_Y);
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Controls.Add(this.nud_CheckerboardBoxSideLengthPix_X);
            this.groupBox4.Controls.Add(this.label17);
            this.groupBox4.Controls.Add(this.label15);
            this.groupBox4.Controls.Add(this.label14);
            this.groupBox4.Controls.Add(this.nud_MesureThreshold);
            this.groupBox4.Controls.Add(this.label22);
            this.groupBox4.Controls.Add(this.cbx_MeasureSelect);
            this.groupBox4.Controls.Add(this.cbx_MeasureTransition);
            this.groupBox4.Controls.Add(this.nud_CheckerboardBoxMaxThd);
            this.groupBox4.Controls.Add(this.nud_CheckerboardBoxMinThd);
            this.groupBox4.Controls.Add(this.label21);
            this.groupBox4.Location = new System.Drawing.Point(9, 349);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(538, 267);
            this.groupBox4.TabIndex = 16;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "检测";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 71);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(137, 12);
            this.label1.TabIndex = 19;
            this.label1.Text = "棋盘格方格边长X(Pixel)";
            // 
            // nud_CheckerboardBoxSideLengthPix_X
            // 
            this.nud_CheckerboardBoxSideLengthPix_X.Location = new System.Drawing.Point(199, 62);
            this.nud_CheckerboardBoxSideLengthPix_X.Maximum = new decimal(new int[] {
            99999999,
            0,
            0,
            0});
            this.nud_CheckerboardBoxSideLengthPix_X.Name = "nud_CheckerboardBoxSideLengthPix_X";
            this.nud_CheckerboardBoxSideLengthPix_X.Size = new System.Drawing.Size(56, 21);
            this.nud_CheckerboardBoxSideLengthPix_X.TabIndex = 20;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(25, 138);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(53, 12);
            this.label17.TabIndex = 9;
            this.label17.Text = "明暗方向";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(24, 172);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(53, 12);
            this.label15.TabIndex = 8;
            this.label15.Text = "边缘选择";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(24, 205);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(53, 12);
            this.label14.TabIndex = 7;
            this.label14.Text = "灰度阈值";
            // 
            // nud_MesureThreshold
            // 
            this.nud_MesureThreshold.Location = new System.Drawing.Point(119, 200);
            this.nud_MesureThreshold.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nud_MesureThreshold.Name = "nud_MesureThreshold";
            this.nud_MesureThreshold.Size = new System.Drawing.Size(136, 21);
            this.nud_MesureThreshold.TabIndex = 6;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(181, 35);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(11, 12);
            this.label22.TabIndex = 17;
            this.label22.Text = "-";
            // 
            // cbx_MeasureSelect
            // 
            this.cbx_MeasureSelect.FormattingEnabled = true;
            this.cbx_MeasureSelect.Location = new System.Drawing.Point(119, 167);
            this.cbx_MeasureSelect.Name = "cbx_MeasureSelect";
            this.cbx_MeasureSelect.Size = new System.Drawing.Size(136, 20);
            this.cbx_MeasureSelect.TabIndex = 5;
            // 
            // cbx_MeasureTransition
            // 
            this.cbx_MeasureTransition.FormattingEnabled = true;
            this.cbx_MeasureTransition.Location = new System.Drawing.Point(119, 130);
            this.cbx_MeasureTransition.Name = "cbx_MeasureTransition";
            this.cbx_MeasureTransition.Size = new System.Drawing.Size(136, 20);
            this.cbx_MeasureTransition.TabIndex = 4;
            // 
            // nud_CheckerboardBoxMaxThd
            // 
            this.nud_CheckerboardBoxMaxThd.Location = new System.Drawing.Point(199, 31);
            this.nud_CheckerboardBoxMaxThd.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nud_CheckerboardBoxMaxThd.Name = "nud_CheckerboardBoxMaxThd";
            this.nud_CheckerboardBoxMaxThd.Size = new System.Drawing.Size(56, 21);
            this.nud_CheckerboardBoxMaxThd.TabIndex = 16;
            // 
            // nud_CheckerboardBoxMinThd
            // 
            this.nud_CheckerboardBoxMinThd.Location = new System.Drawing.Point(119, 31);
            this.nud_CheckerboardBoxMinThd.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nud_CheckerboardBoxMinThd.Name = "nud_CheckerboardBoxMinThd";
            this.nud_CheckerboardBoxMinThd.Size = new System.Drawing.Size(56, 21);
            this.nud_CheckerboardBoxMinThd.TabIndex = 14;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(24, 37);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(89, 12);
            this.label21.TabIndex = 15;
            this.label21.Text = "棋盘格方格阈值";
            // 
            // dgv_Original
            // 
            this.dgv_Original.AllowUserToAddRows = false;
            this.dgv_Original.AllowUserToDeleteRows = false;
            this.dgv_Original.AllowUserToOrderColumns = true;
            this.dgv_Original.AllowUserToResizeRows = false;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgv_Original.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgv_Original.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_Original.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2});
            this.dgv_Original.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv_Original.Location = new System.Drawing.Point(559, 3);
            this.dgv_Original.Name = "dgv_Original";
            this.dgv_Original.RowHeadersVisible = false;
            this.tableLayoutPanel_Main.SetRowSpan(this.dgv_Original, 15);
            this.dgv_Original.RowTemplate.Height = 23;
            this.dgv_Original.Size = new System.Drawing.Size(133, 677);
            this.dgv_Original.TabIndex = 2;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn1.HeaderText = "原始Row";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn2.HeaderText = "原始Col";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(24, 100);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(137, 12);
            this.label8.TabIndex = 23;
            this.label8.Text = "棋盘格方格边长Y(Pixel)";
            // 
            // nud_CheckerboardBoxSideLengthPix_Y
            // 
            this.nud_CheckerboardBoxSideLengthPix_Y.Location = new System.Drawing.Point(199, 96);
            this.nud_CheckerboardBoxSideLengthPix_Y.Maximum = new decimal(new int[] {
            99999999,
            0,
            0,
            0});
            this.nud_CheckerboardBoxSideLengthPix_Y.Name = "nud_CheckerboardBoxSideLengthPix_Y";
            this.nud_CheckerboardBoxSideLengthPix_Y.Size = new System.Drawing.Size(56, 21);
            this.nud_CheckerboardBoxSideLengthPix_Y.TabIndex = 24;
            // 
            // Form_Checkerboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1390, 683);
            this.Controls.Add(this.tableLayoutPanel_Main);
            this.Name = "Form_Checkerboard";
            this.Text = "Form_MultiCam";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_MultiCam_FormClosing);
            this.Load += new System.EventHandler(this.Form_MultiCam_Load);
            ((System.ComponentModel.ISupportInitialize)(this.LoadImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.FullScrean)).EndInit();
            this.tableLayoutPanel_Image1.ResumeLayout(false);
            this.tableLayoutPanel_Image1.PerformLayout();
            this.tableLayoutPanel_Main.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.tableLayoutPanel_MinImage.ResumeLayout(false);
            this.tableLayoutPanel_MinImage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Target)).EndInit();
            this.panel1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_CalibrationPointNum)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_WorldCoordX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_CamCoordX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_WorldCoordY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_CamCoordY)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_CheckerboardBoxSideLengthPix_X)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_MesureThreshold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_CheckerboardBoxMaxThd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_CheckerboardBoxMinThd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Original)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_CheckerboardBoxSideLengthPix_Y)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_Image1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_Main;
        private System.Windows.Forms.PictureBox LoadImage;
        private System.Windows.Forms.TextBox TB_PositionAndGray;
        private System.Windows.Forms.PictureBox FullScrean;
        private HalconDotNet.HWindowControl HWC_Display;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown nud_CheckerboardBoxSideLengthPix_X;
        private System.Windows.Forms.DataGridView dgv_Original;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.NumericUpDown nud_MesureThreshold;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.ComboBox cbx_MeasureSelect;
        private System.Windows.Forms.ComboBox cbx_MeasureTransition;
        private System.Windows.Forms.NumericUpDown nud_CheckerboardBoxMaxThd;
        private System.Windows.Forms.NumericUpDown nud_CheckerboardBoxMinThd;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Button btn_Calibration;
        private System.Windows.Forms.Button btn_Calculate;
        private System.Windows.Forms.Button btn_Save;
        private System.Windows.Forms.DataGridView dgv_Target;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn8;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btn_Transition;
        private System.Windows.Forms.NumericUpDown nud_WorldCoordX;
        private System.Windows.Forms.NumericUpDown nud_CamCoordX;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.NumericUpDown nud_WorldCoordY;
        private System.Windows.Forms.NumericUpDown nud_CamCoordY;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nud_CalibrationPointNum;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_MinImage;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label18;
        private HalconDotNet.HWindowControl HWC_Image3;
        private HalconDotNet.HWindowControl HWC_Image2;
        private HalconDotNet.HWindowControl HWC_Image1;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Button btn_Run;
        private System.Windows.Forms.Button btn_Next;
        private System.Windows.Forms.Button btn_Preview;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown nud_CheckerboardBoxSideLengthPix_Y;
    }
}
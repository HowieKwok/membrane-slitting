namespace Plugin.HandEyeCalibration
{
    partial class FormHandEyeCalibration
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormHandEyeCalibration));
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.GB_DetectParams = new System.Windows.Forms.GroupBox();
            this.TB_ROIMoveIndex = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.gB_ImagePoint = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel_Image = new System.Windows.Forms.FlowLayoutPanel();
            this.gB_RobotPoint = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel_Robot = new System.Windows.Forms.FlowLayoutPanel();
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel7 = new System.Windows.Forms.TableLayoutPanel();
            this.but_SaveHomMat = new System.Windows.Forms.Button();
            this.but_FigureUpHomMat = new System.Windows.Forms.Button();
            this.label24 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label_ErrMean = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label_ErrMax = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.Btn_Test = new System.Windows.Forms.Button();
            this.num_TestImageY = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.num_TestImageX = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.TLP_Image3 = new System.Windows.Forms.TableLayoutPanel();
            this.HWC_Image3 = new HalconDotNet.HWindowControl();
            this.label9 = new System.Windows.Forms.Label();
            this.TLP_Image2 = new System.Windows.Forms.TableLayoutPanel();
            this.HWC_Image2 = new HalconDotNet.HWindowControl();
            this.label16 = new System.Windows.Forms.Label();
            this.TLP_Image1 = new System.Windows.Forms.TableLayoutPanel();
            this.HWC_Image1 = new HalconDotNet.HWindowControl();
            this.Label_Image = new System.Windows.Forms.Label();
            this.TLPanel_BGM1 = new System.Windows.Forms.TableLayoutPanel();
            this.LB_IMageSize = new System.Windows.Forms.Label();
            this.ImageReset = new System.Windows.Forms.PictureBox();
            this.LoadImage = new System.Windows.Forms.PictureBox();
            this.LB_ModleName = new System.Windows.Forms.Label();
            this.TB_PositionAndGray = new System.Windows.Forms.TextBox();
            this.HWC_Display = new HalconDotNet.HWindowControl();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.rtx_ErrorInfo = new System.Windows.Forms.RichTextBox();
            this.fileSystemWatcher1 = new System.IO.FileSystemWatcher();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.tableLayoutPanel2.SuspendLayout();
            this.GB_DetectParams.SuspendLayout();
            this.TB_ROIMoveIndex.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.gB_ImagePoint.SuspendLayout();
            this.gB_RobotPoint.SuspendLayout();
            this.tableLayoutPanel6.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.tableLayoutPanel7.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_TestImageY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_TestImageX)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.TLP_Image3.SuspendLayout();
            this.TLP_Image2.SuspendLayout();
            this.TLP_Image1.SuspendLayout();
            this.TLPanel_BGM1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ImageReset)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LoadImage)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.ColumnCount = 20;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 7.142856F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 7.142856F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 7.142856F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 7.142856F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 7.142856F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 7.142856F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 7.142856F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 7.142856F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 7.142856F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 7.142856F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 7.142856F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 7.142856F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 7.142856F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 7.142856F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 175F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 78F));
            this.tableLayoutPanel2.Controls.Add(this.GB_DetectParams, 14, 0);
            this.tableLayoutPanel2.Controls.Add(this.groupBox1, 0, 13);
            this.tableLayoutPanel2.Controls.Add(this.TLPanel_BGM1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.groupBox3, 14, 15);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 20;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.142856F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.142856F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.142856F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.142856F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.142856F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.142856F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.142856F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.142856F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.142856F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.142856F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.142856F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.142856F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.142856F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.142856F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1008, 771);
            this.tableLayoutPanel2.TabIndex = 16;
            // 
            // GB_DetectParams
            // 
            this.GB_DetectParams.AutoSize = true;
            this.tableLayoutPanel2.SetColumnSpan(this.GB_DetectParams, 6);
            this.GB_DetectParams.Controls.Add(this.TB_ROIMoveIndex);
            this.GB_DetectParams.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GB_DetectParams.Font = new System.Drawing.Font("黑体", 12.75F);
            this.GB_DetectParams.Location = new System.Drawing.Point(509, 5);
            this.GB_DetectParams.Margin = new System.Windows.Forms.Padding(5);
            this.GB_DetectParams.Name = "GB_DetectParams";
            this.tableLayoutPanel2.SetRowSpan(this.GB_DetectParams, 13);
            this.GB_DetectParams.Size = new System.Drawing.Size(494, 510);
            this.GB_DetectParams.TabIndex = 8;
            this.GB_DetectParams.TabStop = false;
            this.GB_DetectParams.Text = " 检测参数 ";
            // 
            // TB_ROIMoveIndex
            // 
            this.TB_ROIMoveIndex.Controls.Add(this.tabPage1);
            this.TB_ROIMoveIndex.Controls.Add(this.tabPage2);
            this.TB_ROIMoveIndex.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TB_ROIMoveIndex.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.TB_ROIMoveIndex.Location = new System.Drawing.Point(3, 28);
            this.TB_ROIMoveIndex.Name = "TB_ROIMoveIndex";
            this.TB_ROIMoveIndex.SelectedIndex = 0;
            this.TB_ROIMoveIndex.Size = new System.Drawing.Size(488, 479);
            this.TB_ROIMoveIndex.TabIndex = 67;
            // 
            // tabPage1
            // 
            this.tabPage1.AutoScroll = true;
            this.tabPage1.Controls.Add(this.tableLayoutPanel4);
            this.tabPage1.Location = new System.Drawing.Point(4, 36);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(480, 439);
            this.tabPage1.TabIndex = 5;
            this.tabPage1.Text = "坐标参数";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 1;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Controls.Add(this.tableLayoutPanel5, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.tableLayoutPanel6, 0, 1);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 2;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 180F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(480, 439);
            this.tableLayoutPanel4.TabIndex = 0;
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 2;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.Controls.Add(this.gB_ImagePoint, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.gB_RobotPoint, 1, 0);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 1;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(474, 253);
            this.tableLayoutPanel5.TabIndex = 1;
            // 
            // gB_ImagePoint
            // 
            this.gB_ImagePoint.Controls.Add(this.flowLayoutPanel_Image);
            this.gB_ImagePoint.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gB_ImagePoint.Location = new System.Drawing.Point(3, 3);
            this.gB_ImagePoint.Name = "gB_ImagePoint";
            this.gB_ImagePoint.Size = new System.Drawing.Size(231, 247);
            this.gB_ImagePoint.TabIndex = 0;
            this.gB_ImagePoint.TabStop = false;
            this.gB_ImagePoint.Text = "图像坐标值";
            // 
            // flowLayoutPanel_Image
            // 
            this.flowLayoutPanel_Image.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel_Image.Location = new System.Drawing.Point(3, 30);
            this.flowLayoutPanel_Image.Name = "flowLayoutPanel_Image";
            this.flowLayoutPanel_Image.Size = new System.Drawing.Size(225, 214);
            this.flowLayoutPanel_Image.TabIndex = 1;
            // 
            // gB_RobotPoint
            // 
            this.gB_RobotPoint.Controls.Add(this.flowLayoutPanel_Robot);
            this.gB_RobotPoint.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gB_RobotPoint.Location = new System.Drawing.Point(240, 3);
            this.gB_RobotPoint.Name = "gB_RobotPoint";
            this.gB_RobotPoint.Size = new System.Drawing.Size(231, 247);
            this.gB_RobotPoint.TabIndex = 2;
            this.gB_RobotPoint.TabStop = false;
            this.gB_RobotPoint.Text = "机械手坐标值";
            // 
            // flowLayoutPanel_Robot
            // 
            this.flowLayoutPanel_Robot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel_Robot.Location = new System.Drawing.Point(3, 30);
            this.flowLayoutPanel_Robot.Name = "flowLayoutPanel_Robot";
            this.flowLayoutPanel_Robot.Size = new System.Drawing.Size(225, 214);
            this.flowLayoutPanel_Robot.TabIndex = 1;
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.ColumnCount = 1;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.Controls.Add(this.groupBox8, 0, 1);
            this.tableLayoutPanel6.Controls.Add(this.groupBox2, 0, 0);
            this.tableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel6.Location = new System.Drawing.Point(3, 262);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 2;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel6.Size = new System.Drawing.Size(474, 174);
            this.tableLayoutPanel6.TabIndex = 2;
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.tableLayoutPanel7);
            this.groupBox8.Controls.Add(this.label24);
            this.groupBox8.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold);
            this.groupBox8.Location = new System.Drawing.Point(4, 91);
            this.groupBox8.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox8.Size = new System.Drawing.Size(466, 79);
            this.groupBox8.TabIndex = 180;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "操作";
            // 
            // tableLayoutPanel7
            // 
            this.tableLayoutPanel7.ColumnCount = 2;
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel7.Controls.Add(this.but_SaveHomMat, 0, 0);
            this.tableLayoutPanel7.Controls.Add(this.but_FigureUpHomMat, 0, 0);
            this.tableLayoutPanel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel7.Location = new System.Drawing.Point(4, 31);
            this.tableLayoutPanel7.Name = "tableLayoutPanel7";
            this.tableLayoutPanel7.RowCount = 1;
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel7.Size = new System.Drawing.Size(458, 44);
            this.tableLayoutPanel7.TabIndex = 146;
            // 
            // but_SaveHomMat
            // 
            this.but_SaveHomMat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.but_SaveHomMat.Location = new System.Drawing.Point(232, 3);
            this.but_SaveHomMat.Name = "but_SaveHomMat";
            this.but_SaveHomMat.Size = new System.Drawing.Size(223, 38);
            this.but_SaveHomMat.TabIndex = 4;
            this.but_SaveHomMat.Text = "保存矩阵";
            this.but_SaveHomMat.UseVisualStyleBackColor = true;
            this.but_SaveHomMat.Click += new System.EventHandler(this.but_SaveHomMat_Click);
            // 
            // but_FigureUpHomMat
            // 
            this.but_FigureUpHomMat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.but_FigureUpHomMat.Location = new System.Drawing.Point(3, 3);
            this.but_FigureUpHomMat.Name = "but_FigureUpHomMat";
            this.but_FigureUpHomMat.Size = new System.Drawing.Size(223, 38);
            this.but_FigureUpHomMat.TabIndex = 1;
            this.but_FigureUpHomMat.Text = "计算矩阵";
            this.but_FigureUpHomMat.UseVisualStyleBackColor = true;
            this.but_FigureUpHomMat.Click += new System.EventHandler(this.but_FigureUpHomMat_Click);
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label24.Location = new System.Drawing.Point(8, 229);
            this.label24.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(0, 20);
            this.label24.TabIndex = 145;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label_ErrMean);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label_ErrMax);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold);
            this.groupBox2.Location = new System.Drawing.Point(3, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(468, 81);
            this.groupBox2.TabIndex = 181;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "结果";
            // 
            // label_ErrMean
            // 
            this.label_ErrMean.AutoSize = true;
            this.label_ErrMean.Location = new System.Drawing.Point(372, 40);
            this.label_ErrMean.Name = "label_ErrMean";
            this.label_ErrMean.Size = new System.Drawing.Size(54, 27);
            this.label_ErrMean.TabIndex = 3;
            this.label_ErrMean.Text = "0.00";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(268, 40);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(98, 27);
            this.label4.TabIndex = 2;
            this.label4.Text = "平均误差:";
            // 
            // label_ErrMax
            // 
            this.label_ErrMax.AutoSize = true;
            this.label_ErrMax.Location = new System.Drawing.Point(112, 40);
            this.label_ErrMax.Name = "label_ErrMax";
            this.label_ErrMax.Size = new System.Drawing.Size(54, 27);
            this.label_ErrMax.TabIndex = 1;
            this.label_ErrMax.Text = "0.00";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(98, 27);
            this.label1.TabIndex = 0;
            this.label1.Text = "最大误差:";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox4);
            this.tabPage2.Location = new System.Drawing.Point(4, 36);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(480, 439);
            this.tabPage2.TabIndex = 6;
            this.tabPage2.Text = "测试";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.Btn_Test);
            this.groupBox4.Controls.Add(this.num_TestImageY);
            this.groupBox4.Controls.Add(this.label3);
            this.groupBox4.Controls.Add(this.num_TestImageX);
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Location = new System.Drawing.Point(6, 6);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(468, 166);
            this.groupBox4.TabIndex = 0;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "数据";
            // 
            // Btn_Test
            // 
            this.Btn_Test.Location = new System.Drawing.Point(262, 105);
            this.Btn_Test.Name = "Btn_Test";
            this.Btn_Test.Size = new System.Drawing.Size(191, 41);
            this.Btn_Test.TabIndex = 4;
            this.Btn_Test.Text = "测试矩阵";
            this.Btn_Test.UseVisualStyleBackColor = true;
            this.Btn_Test.Click += new System.EventHandler(this.Btn_Test_Click);
            // 
            // num_TestImageY
            // 
            this.num_TestImageY.Location = new System.Drawing.Point(333, 46);
            this.num_TestImageY.Name = "num_TestImageY";
            this.num_TestImageY.Size = new System.Drawing.Size(120, 34);
            this.num_TestImageY.TabIndex = 3;
            this.num_TestImageY.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(257, 48);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 27);
            this.label3.TabIndex = 2;
            this.label3.Text = "图像Y:";
            // 
            // num_TestImageX
            // 
            this.num_TestImageX.Location = new System.Drawing.Point(82, 46);
            this.num_TestImageX.Name = "num_TestImageX";
            this.num_TestImageX.Size = new System.Drawing.Size(120, 34);
            this.num_TestImageX.TabIndex = 1;
            this.num_TestImageX.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 27);
            this.label2.TabIndex = 0;
            this.label2.Text = "图像X:";
            // 
            // groupBox1
            // 
            this.groupBox1.AutoSize = true;
            this.tableLayoutPanel2.SetColumnSpan(this.groupBox1, 10);
            this.groupBox1.Controls.Add(this.tableLayoutPanel3);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Font = new System.Drawing.Font("黑体", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.groupBox1.Location = new System.Drawing.Point(5, 525);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(5);
            this.groupBox1.Name = "groupBox1";
            this.tableLayoutPanel2.SetRowSpan(this.groupBox1, 7);
            this.groupBox1.Size = new System.Drawing.Size(350, 241);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = " 图形变量 ";
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.AutoSize = true;
            this.tableLayoutPanel3.ColumnCount = 3;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.Controls.Add(this.TLP_Image3, 2, 0);
            this.tableLayoutPanel3.Controls.Add(this.TLP_Image2, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.TLP_Image1, 0, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 28);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(344, 210);
            this.tableLayoutPanel3.TabIndex = 0;
            // 
            // TLP_Image3
            // 
            this.TLP_Image3.AutoSize = true;
            this.TLP_Image3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.TLP_Image3.ColumnCount = 1;
            this.TLP_Image3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.TLP_Image3.Controls.Add(this.HWC_Image3, 0, 0);
            this.TLP_Image3.Controls.Add(this.label9, 0, 1);
            this.TLP_Image3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.TLP_Image3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TLP_Image3.Location = new System.Drawing.Point(238, 10);
            this.TLP_Image3.Margin = new System.Windows.Forms.Padding(10);
            this.TLP_Image3.Name = "TLP_Image3";
            this.TLP_Image3.RowCount = 2;
            this.TLP_Image3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TLP_Image3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.TLP_Image3.Size = new System.Drawing.Size(96, 190);
            this.TLP_Image3.TabIndex = 7;
            // 
            // HWC_Image3
            // 
            this.HWC_Image3.AutoSize = true;
            this.HWC_Image3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.HWC_Image3.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.HWC_Image3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.HWC_Image3.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.HWC_Image3.Location = new System.Drawing.Point(3, 3);
            this.HWC_Image3.Margin = new System.Windows.Forms.Padding(3, 3, 8, 1);
            this.HWC_Image3.Name = "HWC_Image3";
            this.HWC_Image3.Size = new System.Drawing.Size(89, 151);
            this.HWC_Image3.TabIndex = 2;
            this.HWC_Image3.WindowSize = new System.Drawing.Size(89, 151);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.BackColor = System.Drawing.Color.Transparent;
            this.label9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label9.Font = new System.Drawing.Font("黑体", 10F);
            this.label9.Location = new System.Drawing.Point(5, 156);
            this.label9.Margin = new System.Windows.Forms.Padding(5, 1, 8, 5);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(87, 29);
            this.label9.TabIndex = 1;
            this.label9.Text = "结果图";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TLP_Image2
            // 
            this.TLP_Image2.AutoSize = true;
            this.TLP_Image2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.TLP_Image2.ColumnCount = 1;
            this.TLP_Image2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.TLP_Image2.Controls.Add(this.HWC_Image2, 0, 0);
            this.TLP_Image2.Controls.Add(this.label16, 0, 1);
            this.TLP_Image2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.TLP_Image2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TLP_Image2.Location = new System.Drawing.Point(124, 10);
            this.TLP_Image2.Margin = new System.Windows.Forms.Padding(10);
            this.TLP_Image2.Name = "TLP_Image2";
            this.TLP_Image2.RowCount = 2;
            this.TLP_Image2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TLP_Image2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.TLP_Image2.Size = new System.Drawing.Size(94, 190);
            this.TLP_Image2.TabIndex = 5;
            // 
            // HWC_Image2
            // 
            this.HWC_Image2.AutoSize = true;
            this.HWC_Image2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.HWC_Image2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.HWC_Image2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.HWC_Image2.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.HWC_Image2.Location = new System.Drawing.Point(3, 3);
            this.HWC_Image2.Margin = new System.Windows.Forms.Padding(3, 3, 8, 1);
            this.HWC_Image2.Name = "HWC_Image2";
            this.HWC_Image2.Size = new System.Drawing.Size(89, 151);
            this.HWC_Image2.TabIndex = 2;
            this.HWC_Image2.WindowSize = new System.Drawing.Size(89, 151);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.BackColor = System.Drawing.Color.Transparent;
            this.label16.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label16.Font = new System.Drawing.Font("黑体", 10F);
            this.label16.Location = new System.Drawing.Point(5, 156);
            this.label16.Margin = new System.Windows.Forms.Padding(5, 1, 8, 5);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(87, 29);
            this.label16.TabIndex = 1;
            this.label16.Text = "过程图";
            this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TLP_Image1
            // 
            this.TLP_Image1.AutoSize = true;
            this.TLP_Image1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.TLP_Image1.ColumnCount = 1;
            this.TLP_Image1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.TLP_Image1.Controls.Add(this.HWC_Image1, 0, 0);
            this.TLP_Image1.Controls.Add(this.Label_Image, 0, 1);
            this.TLP_Image1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.TLP_Image1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TLP_Image1.Location = new System.Drawing.Point(10, 10);
            this.TLP_Image1.Margin = new System.Windows.Forms.Padding(10);
            this.TLP_Image1.Name = "TLP_Image1";
            this.TLP_Image1.RowCount = 2;
            this.TLP_Image1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TLP_Image1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.TLP_Image1.Size = new System.Drawing.Size(94, 190);
            this.TLP_Image1.TabIndex = 4;
            // 
            // HWC_Image1
            // 
            this.HWC_Image1.AutoSize = true;
            this.HWC_Image1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.HWC_Image1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.HWC_Image1.Cursor = System.Windows.Forms.Cursors.Default;
            this.HWC_Image1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.HWC_Image1.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.HWC_Image1.Location = new System.Drawing.Point(3, 3);
            this.HWC_Image1.Margin = new System.Windows.Forms.Padding(3, 3, 8, 1);
            this.HWC_Image1.Name = "HWC_Image1";
            this.HWC_Image1.Size = new System.Drawing.Size(89, 151);
            this.HWC_Image1.TabIndex = 2;
            this.HWC_Image1.WindowSize = new System.Drawing.Size(89, 151);
            // 
            // Label_Image
            // 
            this.Label_Image.AutoSize = true;
            this.Label_Image.BackColor = System.Drawing.Color.Transparent;
            this.Label_Image.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Label_Image.Font = new System.Drawing.Font("黑体", 10F);
            this.Label_Image.Location = new System.Drawing.Point(5, 156);
            this.Label_Image.Margin = new System.Windows.Forms.Padding(5, 1, 8, 5);
            this.Label_Image.Name = "Label_Image";
            this.Label_Image.Size = new System.Drawing.Size(87, 29);
            this.Label_Image.TabIndex = 1;
            this.Label_Image.Text = "原图";
            this.Label_Image.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TLPanel_BGM1
            // 
            this.TLPanel_BGM1.AutoSize = true;
            this.TLPanel_BGM1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.TLPanel_BGM1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.TLPanel_BGM1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.TLPanel_BGM1.ColumnCount = 10;
            this.tableLayoutPanel2.SetColumnSpan(this.TLPanel_BGM1, 14);
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
            this.TLPanel_BGM1.Controls.Add(this.LB_IMageSize, 6, 0);
            this.TLPanel_BGM1.Controls.Add(this.ImageReset, 9, 9);
            this.TLPanel_BGM1.Controls.Add(this.LoadImage, 0, 9);
            this.TLPanel_BGM1.Controls.Add(this.LB_ModleName, 0, 0);
            this.TLPanel_BGM1.Controls.Add(this.TB_PositionAndGray, 5, 9);
            this.TLPanel_BGM1.Controls.Add(this.HWC_Display, 0, 1);
            this.TLPanel_BGM1.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.TLPanel_BGM1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TLPanel_BGM1.Location = new System.Drawing.Point(5, 5);
            this.TLPanel_BGM1.Margin = new System.Windows.Forms.Padding(5);
            this.TLPanel_BGM1.Name = "TLPanel_BGM1";
            this.TLPanel_BGM1.RowCount = 10;
            this.tableLayoutPanel2.SetRowSpan(this.TLPanel_BGM1, 13);
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
            this.TLPanel_BGM1.Size = new System.Drawing.Size(494, 510);
            this.TLPanel_BGM1.TabIndex = 2;
            // 
            // LB_IMageSize
            // 
            this.LB_IMageSize.AutoSize = true;
            this.LB_IMageSize.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.TLPanel_BGM1.SetColumnSpan(this.LB_IMageSize, 3);
            this.LB_IMageSize.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LB_IMageSize.Font = new System.Drawing.Font("黑体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LB_IMageSize.ForeColor = System.Drawing.Color.White;
            this.LB_IMageSize.Location = new System.Drawing.Point(234, 5);
            this.LB_IMageSize.Margin = new System.Windows.Forms.Padding(13, 5, 3, 3);
            this.LB_IMageSize.Name = "LB_IMageSize";
            this.LB_IMageSize.Size = new System.Drawing.Size(207, 27);
            this.LB_IMageSize.TabIndex = 15;
            this.LB_IMageSize.Text = "Size  R:—   C:—";
            this.LB_IMageSize.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ImageReset
            // 
            this.ImageReset.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.ImageReset.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("ImageReset.BackgroundImage")));
            this.ImageReset.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ImageReset.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ImageReset.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ImageReset.Location = new System.Drawing.Point(450, 479);
            this.ImageReset.Margin = new System.Windows.Forms.Padding(6, 4, 15, 8);
            this.ImageReset.Name = "ImageReset";
            this.ImageReset.Size = new System.Drawing.Size(29, 23);
            this.ImageReset.TabIndex = 7;
            this.ImageReset.TabStop = false;
            this.ImageReset.Click += new System.EventHandler(this.ImageReset_Click);
            // 
            // LoadImage
            // 
            this.LoadImage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.LoadImage.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("LoadImage.BackgroundImage")));
            this.LoadImage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.LoadImage.Cursor = System.Windows.Forms.Cursors.Hand;
            this.LoadImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LoadImage.Location = new System.Drawing.Point(15, 479);
            this.LoadImage.Margin = new System.Windows.Forms.Padding(15, 4, 5, 7);
            this.LoadImage.Name = "LoadImage";
            this.LoadImage.Size = new System.Drawing.Size(30, 24);
            this.LoadImage.TabIndex = 4;
            this.LoadImage.TabStop = false;
            this.LoadImage.Click += new System.EventHandler(this.LoadImage_Click);
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
            this.LB_ModleName.Text = "残留检测：";
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
            this.TB_PositionAndGray.Location = new System.Drawing.Point(145, 483);
            this.TB_PositionAndGray.Margin = new System.Windows.Forms.Padding(5, 8, 5, 6);
            this.TB_PositionAndGray.Name = "TB_PositionAndGray";
            this.TB_PositionAndGray.ShortcutsEnabled = false;
            this.TB_PositionAndGray.Size = new System.Drawing.Size(294, 23);
            this.TB_PositionAndGray.TabIndex = 8;
            this.TB_PositionAndGray.TabStop = false;
            this.TB_PositionAndGray.Text = "R: —  C: —  Value: — ";
            // 
            // HWC_Display
            // 
            this.HWC_Display.BackColor = System.Drawing.Color.White;
            this.HWC_Display.BorderColor = System.Drawing.Color.White;
            this.TLPanel_BGM1.SetColumnSpan(this.HWC_Display, 10);
            this.HWC_Display.Dock = System.Windows.Forms.DockStyle.Fill;
            this.HWC_Display.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.HWC_Display.Location = new System.Drawing.Point(3, 38);
            this.HWC_Display.Name = "HWC_Display";
            this.TLPanel_BGM1.SetRowSpan(this.HWC_Display, 8);
            this.HWC_Display.Size = new System.Drawing.Size(488, 434);
            this.HWC_Display.TabIndex = 9;
            this.HWC_Display.WindowSize = new System.Drawing.Size(488, 434);
            this.HWC_Display.HMouseMove += new HalconDotNet.HMouseEventHandler(this.HWC_Display_HMouseMove);
            // 
            // groupBox3
            // 
            this.tableLayoutPanel2.SetColumnSpan(this.groupBox3, 6);
            this.groupBox3.Controls.Add(this.rtx_ErrorInfo);
            this.groupBox3.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold);
            this.groupBox3.Location = new System.Drawing.Point(507, 598);
            this.groupBox3.Name = "groupBox3";
            this.tableLayoutPanel2.SetRowSpan(this.groupBox3, 5);
            this.groupBox3.Size = new System.Drawing.Size(489, 170);
            this.groupBox3.TabIndex = 9;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "输出";
            // 
            // rtx_ErrorInfo
            // 
            this.rtx_ErrorInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtx_ErrorInfo.Font = new System.Drawing.Font("宋体", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rtx_ErrorInfo.Location = new System.Drawing.Point(3, 30);
            this.rtx_ErrorInfo.Margin = new System.Windows.Forms.Padding(4);
            this.rtx_ErrorInfo.Name = "rtx_ErrorInfo";
            this.rtx_ErrorInfo.Size = new System.Drawing.Size(483, 137);
            this.rtx_ErrorInfo.TabIndex = 1;
            this.rtx_ErrorInfo.Text = "";
            // 
            // fileSystemWatcher1
            // 
            this.fileSystemWatcher1.EnableRaisingEvents = true;
            this.fileSystemWatcher1.SynchronizingObject = this;
            // 
            // FormHandEyeCalibration
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(1008, 811);
            this.Controls.Add(this.tableLayoutPanel2);
            this.Name = "FormHandEyeCalibration";
            this.Text = "FormHandEyeCalibration";
            this.Load += new System.EventHandler(this.FormFormHandEyeCalibration_Load);
            this.Controls.SetChildIndex(this.tableLayoutPanel2, 0);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.GB_DetectParams.ResumeLayout(false);
            this.TB_ROIMoveIndex.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel5.ResumeLayout(false);
            this.gB_ImagePoint.ResumeLayout(false);
            this.gB_RobotPoint.ResumeLayout(false);
            this.tableLayoutPanel6.ResumeLayout(false);
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.tableLayoutPanel7.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_TestImageY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_TestImageX)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.TLP_Image3.ResumeLayout(false);
            this.TLP_Image3.PerformLayout();
            this.TLP_Image2.ResumeLayout(false);
            this.TLP_Image2.PerformLayout();
            this.TLP_Image1.ResumeLayout(false);
            this.TLP_Image1.PerformLayout();
            this.TLPanel_BGM1.ResumeLayout(false);
            this.TLPanel_BGM1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ImageReset)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LoadImage)).EndInit();
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.GroupBox GB_DetectParams;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.TableLayoutPanel TLP_Image3;
        private HalconDotNet.HWindowControl HWC_Image3;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TableLayoutPanel TLP_Image2;
        private HalconDotNet.HWindowControl HWC_Image2;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TableLayoutPanel TLP_Image1;
        private HalconDotNet.HWindowControl HWC_Image1;
        private System.Windows.Forms.Label Label_Image;
        private System.Windows.Forms.TableLayoutPanel TLPanel_BGM1;
        public System.Windows.Forms.Label LB_IMageSize;
        public System.Windows.Forms.PictureBox ImageReset;
        public System.Windows.Forms.PictureBox LoadImage;
        public System.Windows.Forms.Label LB_ModleName;
        private System.Windows.Forms.TextBox TB_PositionAndGray;
        private HalconDotNet.HWindowControl HWC_Display;
        private System.IO.FileSystemWatcher fileSystemWatcher1;
        private System.Windows.Forms.TabControl TB_ROIMoveIndex;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.GroupBox gB_ImagePoint;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel_Image;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.GroupBox gB_RobotPoint;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel_Robot;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel7;
        private System.Windows.Forms.Button but_FigureUpHomMat;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label_ErrMean;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label_ErrMax;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button but_SaveHomMat;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RichTextBox rtx_ErrorInfo;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.NumericUpDown num_TestImageY;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown num_TestImageX;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button Btn_Test;
    }
}
namespace Mef_VisionFrame
{
    partial class FormDataAnalysis
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
            this.l_link = new System.Windows.Forms.LinkLabel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.gb9 = new System.Windows.Forms.GroupBox();
            this.HWC_ImageScreen = new HalconDotNet.HWindowControl();
            this.gb8 = new System.Windows.Forms.GroupBox();
            this.HWC_Image8 = new HalconDotNet.HWindowControl();
            this.gb7 = new System.Windows.Forms.GroupBox();
            this.HWC_Image7 = new HalconDotNet.HWindowControl();
            this.gb6 = new System.Windows.Forms.GroupBox();
            this.HWC_Image6 = new HalconDotNet.HWindowControl();
            this.gb5 = new System.Windows.Forms.GroupBox();
            this.HWC_Image5 = new HalconDotNet.HWindowControl();
            this.gb4 = new System.Windows.Forms.GroupBox();
            this.HWC_Image4 = new HalconDotNet.HWindowControl();
            this.gb3 = new System.Windows.Forms.GroupBox();
            this.HWC_Image3 = new HalconDotNet.HWindowControl();
            this.gb2 = new System.Windows.Forms.GroupBox();
            this.HWC_Image2 = new HalconDotNet.HWindowControl();
            this.gb1 = new System.Windows.Forms.GroupBox();
            this.HWC_Image1 = new HalconDotNet.HWindowControl();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.LB_IMageSize = new System.Windows.Forms.Label();
            this.TB_PositionAndGray = new System.Windows.Forms.TextBox();
            this.LB_ModleName = new System.Windows.Forms.Label();
            this.LoadImage = new System.Windows.Forms.PictureBox();
            this.HWC_Display = new HalconDotNet.HWindowControl();
            this.FullScrean = new System.Windows.Forms.PictureBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.gb9.SuspendLayout();
            this.gb8.SuspendLayout();
            this.gb7.SuspendLayout();
            this.gb6.SuspendLayout();
            this.gb5.SuspendLayout();
            this.gb4.SuspendLayout();
            this.gb3.SuspendLayout();
            this.gb2.SuspendLayout();
            this.gb1.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LoadImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.FullScrean)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.tableLayoutPanel1.Controls.Add(this.l_link, 2, 5);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 2, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1214, 788);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // l_link
            // 
            this.l_link.Dock = System.Windows.Forms.DockStyle.Fill;
            this.l_link.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.l_link.Location = new System.Drawing.Point(367, 755);
            this.l_link.Name = "l_link";
            this.l_link.Size = new System.Drawing.Size(844, 33);
            this.l_link.TabIndex = 4;
            this.l_link.TabStop = true;
            this.l_link.Text = "默认路径";
            this.l_link.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.l_link_LinkClicked);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel1.SetColumnSpan(this.tableLayoutPanel2, 2);
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.gb9, 0, 4);
            this.tableLayoutPanel2.Controls.Add(this.gb8, 1, 3);
            this.tableLayoutPanel2.Controls.Add(this.gb7, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.gb6, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.gb5, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.gb4, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.gb3, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.gb2, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.gb1, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 5;
            this.tableLayoutPanel1.SetRowSpan(this.tableLayoutPanel2, 5);
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(364, 755);
            this.tableLayoutPanel2.TabIndex = 2;
            // 
            // gb9
            // 
            this.tableLayoutPanel2.SetColumnSpan(this.gb9, 2);
            this.gb9.Controls.Add(this.HWC_ImageScreen);
            this.gb9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gb9.Location = new System.Drawing.Point(3, 607);
            this.gb9.Name = "gb9";
            this.gb9.Size = new System.Drawing.Size(358, 145);
            this.gb9.TabIndex = 8;
            this.gb9.TabStop = false;
            this.gb9.Text = "窗口截图";
            // 
            // HWC_ImageScreen
            // 
            this.HWC_ImageScreen.BackColor = System.Drawing.Color.Black;
            this.HWC_ImageScreen.BorderColor = System.Drawing.Color.Black;
            this.HWC_ImageScreen.Dock = System.Windows.Forms.DockStyle.Fill;
            this.HWC_ImageScreen.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.HWC_ImageScreen.Location = new System.Drawing.Point(3, 17);
            this.HWC_ImageScreen.Name = "HWC_ImageScreen";
            this.HWC_ImageScreen.Size = new System.Drawing.Size(352, 125);
            this.HWC_ImageScreen.TabIndex = 4;
            this.HWC_ImageScreen.WindowSize = new System.Drawing.Size(352, 125);
            this.HWC_ImageScreen.HMouseDown += new HalconDotNet.HMouseEventHandler(this.HWC_Image1_HMouseDown);
            // 
            // gb8
            // 
            this.gb8.Controls.Add(this.HWC_Image8);
            this.gb8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gb8.Location = new System.Drawing.Point(185, 456);
            this.gb8.Name = "gb8";
            this.gb8.Size = new System.Drawing.Size(176, 145);
            this.gb8.TabIndex = 7;
            this.gb8.TabStop = false;
            this.gb8.Text = "叠台B相机4";
            // 
            // HWC_Image8
            // 
            this.HWC_Image8.BackColor = System.Drawing.Color.Black;
            this.HWC_Image8.BorderColor = System.Drawing.Color.Black;
            this.HWC_Image8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.HWC_Image8.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.HWC_Image8.Location = new System.Drawing.Point(3, 17);
            this.HWC_Image8.Name = "HWC_Image8";
            this.HWC_Image8.Size = new System.Drawing.Size(170, 125);
            this.HWC_Image8.TabIndex = 2;
            this.HWC_Image8.WindowSize = new System.Drawing.Size(170, 125);
            this.HWC_Image8.HMouseDown += new HalconDotNet.HMouseEventHandler(this.HWC_Image1_HMouseDown);
            // 
            // gb7
            // 
            this.gb7.Controls.Add(this.HWC_Image7);
            this.gb7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gb7.Location = new System.Drawing.Point(3, 456);
            this.gb7.Name = "gb7";
            this.gb7.Size = new System.Drawing.Size(176, 145);
            this.gb7.TabIndex = 6;
            this.gb7.TabStop = false;
            this.gb7.Text = "叠台B相机3";
            // 
            // HWC_Image7
            // 
            this.HWC_Image7.BackColor = System.Drawing.Color.Black;
            this.HWC_Image7.BorderColor = System.Drawing.Color.Black;
            this.HWC_Image7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.HWC_Image7.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.HWC_Image7.Location = new System.Drawing.Point(3, 17);
            this.HWC_Image7.Name = "HWC_Image7";
            this.HWC_Image7.Size = new System.Drawing.Size(170, 125);
            this.HWC_Image7.TabIndex = 2;
            this.HWC_Image7.WindowSize = new System.Drawing.Size(170, 125);
            this.HWC_Image7.HMouseDown += new HalconDotNet.HMouseEventHandler(this.HWC_Image1_HMouseDown);
            // 
            // gb6
            // 
            this.gb6.Controls.Add(this.HWC_Image6);
            this.gb6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gb6.Location = new System.Drawing.Point(185, 305);
            this.gb6.Name = "gb6";
            this.gb6.Size = new System.Drawing.Size(176, 145);
            this.gb6.TabIndex = 5;
            this.gb6.TabStop = false;
            this.gb6.Text = "叠台B相机2";
            // 
            // HWC_Image6
            // 
            this.HWC_Image6.BackColor = System.Drawing.Color.Black;
            this.HWC_Image6.BorderColor = System.Drawing.Color.Black;
            this.HWC_Image6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.HWC_Image6.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.HWC_Image6.Location = new System.Drawing.Point(3, 17);
            this.HWC_Image6.Name = "HWC_Image6";
            this.HWC_Image6.Size = new System.Drawing.Size(170, 125);
            this.HWC_Image6.TabIndex = 2;
            this.HWC_Image6.WindowSize = new System.Drawing.Size(170, 125);
            this.HWC_Image6.HMouseDown += new HalconDotNet.HMouseEventHandler(this.HWC_Image1_HMouseDown);
            // 
            // gb5
            // 
            this.gb5.Controls.Add(this.HWC_Image5);
            this.gb5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gb5.Location = new System.Drawing.Point(3, 305);
            this.gb5.Name = "gb5";
            this.gb5.Size = new System.Drawing.Size(176, 145);
            this.gb5.TabIndex = 4;
            this.gb5.TabStop = false;
            this.gb5.Text = "叠台B相机1";
            // 
            // HWC_Image5
            // 
            this.HWC_Image5.BackColor = System.Drawing.Color.Black;
            this.HWC_Image5.BorderColor = System.Drawing.Color.Black;
            this.HWC_Image5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.HWC_Image5.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.HWC_Image5.Location = new System.Drawing.Point(3, 17);
            this.HWC_Image5.Name = "HWC_Image5";
            this.HWC_Image5.Size = new System.Drawing.Size(170, 125);
            this.HWC_Image5.TabIndex = 2;
            this.HWC_Image5.WindowSize = new System.Drawing.Size(170, 125);
            this.HWC_Image5.HMouseDown += new HalconDotNet.HMouseEventHandler(this.HWC_Image1_HMouseDown);
            // 
            // gb4
            // 
            this.gb4.Controls.Add(this.HWC_Image4);
            this.gb4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gb4.Location = new System.Drawing.Point(185, 154);
            this.gb4.Name = "gb4";
            this.gb4.Size = new System.Drawing.Size(176, 145);
            this.gb4.TabIndex = 3;
            this.gb4.TabStop = false;
            this.gb4.Text = "叠台A相机4";
            // 
            // HWC_Image4
            // 
            this.HWC_Image4.BackColor = System.Drawing.Color.Black;
            this.HWC_Image4.BorderColor = System.Drawing.Color.Black;
            this.HWC_Image4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.HWC_Image4.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.HWC_Image4.Location = new System.Drawing.Point(3, 17);
            this.HWC_Image4.Name = "HWC_Image4";
            this.HWC_Image4.Size = new System.Drawing.Size(170, 125);
            this.HWC_Image4.TabIndex = 2;
            this.HWC_Image4.WindowSize = new System.Drawing.Size(170, 125);
            this.HWC_Image4.HMouseDown += new HalconDotNet.HMouseEventHandler(this.HWC_Image1_HMouseDown);
            // 
            // gb3
            // 
            this.gb3.Controls.Add(this.HWC_Image3);
            this.gb3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gb3.Location = new System.Drawing.Point(3, 154);
            this.gb3.Name = "gb3";
            this.gb3.Size = new System.Drawing.Size(176, 145);
            this.gb3.TabIndex = 2;
            this.gb3.TabStop = false;
            this.gb3.Text = "叠台A相机3";
            // 
            // HWC_Image3
            // 
            this.HWC_Image3.BackColor = System.Drawing.Color.Black;
            this.HWC_Image3.BorderColor = System.Drawing.Color.Black;
            this.HWC_Image3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.HWC_Image3.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.HWC_Image3.Location = new System.Drawing.Point(3, 17);
            this.HWC_Image3.Name = "HWC_Image3";
            this.HWC_Image3.Size = new System.Drawing.Size(170, 125);
            this.HWC_Image3.TabIndex = 2;
            this.HWC_Image3.WindowSize = new System.Drawing.Size(170, 125);
            this.HWC_Image3.HMouseDown += new HalconDotNet.HMouseEventHandler(this.HWC_Image1_HMouseDown);
            // 
            // gb2
            // 
            this.gb2.Controls.Add(this.HWC_Image2);
            this.gb2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gb2.Location = new System.Drawing.Point(185, 3);
            this.gb2.Name = "gb2";
            this.gb2.Size = new System.Drawing.Size(176, 145);
            this.gb2.TabIndex = 1;
            this.gb2.TabStop = false;
            this.gb2.Text = "叠台A相机2";
            // 
            // HWC_Image2
            // 
            this.HWC_Image2.BackColor = System.Drawing.Color.Black;
            this.HWC_Image2.BorderColor = System.Drawing.Color.Black;
            this.HWC_Image2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.HWC_Image2.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.HWC_Image2.Location = new System.Drawing.Point(3, 17);
            this.HWC_Image2.Name = "HWC_Image2";
            this.HWC_Image2.Size = new System.Drawing.Size(170, 125);
            this.HWC_Image2.TabIndex = 2;
            this.HWC_Image2.WindowSize = new System.Drawing.Size(170, 125);
            this.HWC_Image2.HMouseDown += new HalconDotNet.HMouseEventHandler(this.HWC_Image1_HMouseDown);
            // 
            // gb1
            // 
            this.gb1.BackColor = System.Drawing.SystemColors.Control;
            this.gb1.Controls.Add(this.HWC_Image1);
            this.gb1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gb1.Location = new System.Drawing.Point(3, 3);
            this.gb1.Name = "gb1";
            this.gb1.Size = new System.Drawing.Size(176, 145);
            this.gb1.TabIndex = 0;
            this.gb1.TabStop = false;
            this.gb1.Text = "叠台A相机1";
            // 
            // HWC_Image1
            // 
            this.HWC_Image1.BackColor = System.Drawing.Color.Black;
            this.HWC_Image1.BorderColor = System.Drawing.Color.Black;
            this.HWC_Image1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.HWC_Image1.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.HWC_Image1.Location = new System.Drawing.Point(3, 17);
            this.HWC_Image1.Name = "HWC_Image1";
            this.HWC_Image1.Size = new System.Drawing.Size(170, 125);
            this.HWC_Image1.TabIndex = 1;
            this.HWC_Image1.WindowSize = new System.Drawing.Size(170, 125);
            this.HWC_Image1.HMouseDown += new HalconDotNet.HMouseEventHandler(this.HWC_Image1_HMouseDown);
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.tableLayoutPanel3.ColumnCount = 8;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this.tableLayoutPanel3.Controls.Add(this.LB_IMageSize, 4, 0);
            this.tableLayoutPanel3.Controls.Add(this.TB_PositionAndGray, 3, 2);
            this.tableLayoutPanel3.Controls.Add(this.LB_ModleName, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.LoadImage, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.HWC_Display, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.FullScrean, 7, 2);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(372, 5);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(8, 5, 8, 5);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 3;
            this.tableLayoutPanel1.SetRowSpan(this.tableLayoutPanel3, 5);
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(834, 745);
            this.tableLayoutPanel3.TabIndex = 1;
            // 
            // LB_IMageSize
            // 
            this.LB_IMageSize.AutoSize = true;
            this.LB_IMageSize.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.tableLayoutPanel3.SetColumnSpan(this.LB_IMageSize, 3);
            this.LB_IMageSize.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LB_IMageSize.Font = new System.Drawing.Font("黑体", 12F);
            this.LB_IMageSize.ForeColor = System.Drawing.Color.White;
            this.LB_IMageSize.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.LB_IMageSize.Location = new System.Drawing.Point(427, 5);
            this.LB_IMageSize.Margin = new System.Windows.Forms.Padding(13, 5, 3, 3);
            this.LB_IMageSize.Name = "LB_IMageSize";
            this.LB_IMageSize.Size = new System.Drawing.Size(368, 27);
            this.LB_IMageSize.TabIndex = 14;
            this.LB_IMageSize.Text = "Size  R:—   C:—";
            this.LB_IMageSize.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // TB_PositionAndGray
            // 
            this.TB_PositionAndGray.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.TB_PositionAndGray.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tableLayoutPanel3.SetColumnSpan(this.TB_PositionAndGray, 3);
            this.TB_PositionAndGray.Cursor = System.Windows.Forms.Cursors.Default;
            this.TB_PositionAndGray.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TB_PositionAndGray.Font = new System.Drawing.Font("黑体", 12F);
            this.TB_PositionAndGray.ForeColor = System.Drawing.Color.White;
            this.TB_PositionAndGray.Location = new System.Drawing.Point(291, 718);
            this.TB_PositionAndGray.Margin = new System.Windows.Forms.Padding(5, 8, 5, 6);
            this.TB_PositionAndGray.Name = "TB_PositionAndGray";
            this.TB_PositionAndGray.ShortcutsEnabled = false;
            this.TB_PositionAndGray.Size = new System.Drawing.Size(374, 19);
            this.TB_PositionAndGray.TabIndex = 9;
            this.TB_PositionAndGray.TabStop = false;
            this.TB_PositionAndGray.Text = "R: —  C: —  Value: — ";
            // 
            // LB_ModleName
            // 
            this.LB_ModleName.AutoSize = true;
            this.LB_ModleName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.tableLayoutPanel3.SetColumnSpan(this.LB_ModleName, 3);
            this.LB_ModleName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LB_ModleName.Font = new System.Drawing.Font("黑体", 12.75F);
            this.LB_ModleName.ForeColor = System.Drawing.Color.White;
            this.LB_ModleName.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.LB_ModleName.Location = new System.Drawing.Point(13, 5);
            this.LB_ModleName.Margin = new System.Windows.Forms.Padding(13, 5, 3, 3);
            this.LB_ModleName.Name = "LB_ModleName";
            this.LB_ModleName.Size = new System.Drawing.Size(270, 27);
            this.LB_ModleName.TabIndex = 3;
            this.LB_ModleName.Text = "直线测量：";
            this.LB_ModleName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LoadImage
            // 
            this.LoadImage.BackColor = System.Drawing.SystemColors.Control;
            this.LoadImage.BackgroundImage = global::Mef_VisionFrame.Properties.Resources.交换机;
            this.LoadImage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.LoadImage.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.LoadImage.Location = new System.Drawing.Point(3, 713);
            this.LoadImage.Name = "LoadImage";
            this.LoadImage.Size = new System.Drawing.Size(24, 29);
            this.LoadImage.TabIndex = 11;
            this.LoadImage.TabStop = false;
            this.LoadImage.Click += new System.EventHandler(this.LoadImage_Click);
            // 
            // HWC_Display
            // 
            this.HWC_Display.BackColor = System.Drawing.Color.Black;
            this.HWC_Display.BorderColor = System.Drawing.Color.Black;
            this.tableLayoutPanel3.SetColumnSpan(this.HWC_Display, 9);
            this.HWC_Display.Dock = System.Windows.Forms.DockStyle.Fill;
            this.HWC_Display.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.HWC_Display.Location = new System.Drawing.Point(3, 38);
            this.HWC_Display.Name = "HWC_Display";
            this.HWC_Display.Size = new System.Drawing.Size(828, 669);
            this.HWC_Display.TabIndex = 12;
            this.HWC_Display.WindowSize = new System.Drawing.Size(828, 669);
            // 
            // FullScrean
            // 
            this.FullScrean.BackColor = System.Drawing.SystemColors.Control;
            this.FullScrean.BackgroundImage = global::Mef_VisionFrame.Properties.Resources.全屏11;
            this.FullScrean.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.FullScrean.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.FullScrean.Location = new System.Drawing.Point(801, 713);
            this.FullScrean.Name = "FullScrean";
            this.FullScrean.Size = new System.Drawing.Size(29, 29);
            this.FullScrean.TabIndex = 10;
            this.FullScrean.TabStop = false;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 300000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // FormDataAnalysis
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(1214, 788);
            this.Controls.Add(this.tableLayoutPanel1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormDataAnalysis";
            this.Text = "FormDataAnalysis";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormDataAnalysis_FormClosing);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.gb9.ResumeLayout(false);
            this.gb8.ResumeLayout(false);
            this.gb7.ResumeLayout(false);
            this.gb6.ResumeLayout(false);
            this.gb5.ResumeLayout(false);
            this.gb4.ResumeLayout(false);
            this.gb3.ResumeLayout(false);
            this.gb2.ResumeLayout(false);
            this.gb1.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LoadImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.FullScrean)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        public System.Windows.Forms.Label LB_IMageSize;
        private System.Windows.Forms.TextBox TB_PositionAndGray;
        public System.Windows.Forms.Label LB_ModleName;
        private System.Windows.Forms.PictureBox LoadImage;
        private HalconDotNet.HWindowControl HWC_Display;
        private System.Windows.Forms.PictureBox FullScrean;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.GroupBox gb8;
        private System.Windows.Forms.GroupBox gb7;
        private System.Windows.Forms.GroupBox gb6;
        private System.Windows.Forms.GroupBox gb5;
        private System.Windows.Forms.GroupBox gb4;
        private System.Windows.Forms.GroupBox gb3;
        private System.Windows.Forms.GroupBox gb2;
        private System.Windows.Forms.GroupBox gb1;
        private HalconDotNet.HWindowControl HWC_Image8;
        private HalconDotNet.HWindowControl HWC_Image7;
        private HalconDotNet.HWindowControl HWC_Image6;
        private HalconDotNet.HWindowControl HWC_Image5;
        private HalconDotNet.HWindowControl HWC_Image4;
        private HalconDotNet.HWindowControl HWC_Image3;
        private HalconDotNet.HWindowControl HWC_Image2;
        private HalconDotNet.HWindowControl HWC_Image1;
        private System.Windows.Forms.LinkLabel l_link;
        private System.Windows.Forms.GroupBox gb9;
        private HalconDotNet.HWindowControl HWC_ImageScreen;
        private System.Windows.Forms.Timer timer1;
    }
}
namespace Mef_VisionFrame
{
    partial class UC_HWCshow
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

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.TB_PositionAndGray = new System.Windows.Forms.TextBox();
            this.Label_CamName = new System.Windows.Forms.Label();
            this.HWC_Cam = new HalconDotNet.HWindowControl();
            this.PB_LoadImage = new System.Windows.Forms.PictureBox();
            this.PB_FItWindow = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PB_LoadImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PB_FItWindow)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.Black;
            this.tableLayoutPanel1.ColumnCount = 8;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.Controls.Add(this.TB_PositionAndGray, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.Label_CamName, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.HWC_Cam, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.PB_LoadImage, 6, 0);
            this.tableLayoutPanel1.Controls.Add(this.PB_FItWindow, 7, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(713, 564);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // TB_PositionAndGray
            // 
            this.TB_PositionAndGray.BackColor = System.Drawing.Color.Black;
            this.TB_PositionAndGray.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tableLayoutPanel1.SetColumnSpan(this.TB_PositionAndGray, 3);
            this.TB_PositionAndGray.Cursor = System.Windows.Forms.Cursors.Default;
            this.TB_PositionAndGray.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TB_PositionAndGray.Font = new System.Drawing.Font("黑体", 11F);
            this.TB_PositionAndGray.ForeColor = System.Drawing.Color.White;
            this.TB_PositionAndGray.Location = new System.Drawing.Point(225, 5);
            this.TB_PositionAndGray.Margin = new System.Windows.Forms.Padding(0, 5, 0, 3);
            this.TB_PositionAndGray.Name = "TB_PositionAndGray";
            this.TB_PositionAndGray.ShortcutsEnabled = false;
            this.TB_PositionAndGray.Size = new System.Drawing.Size(315, 17);
            this.TB_PositionAndGray.TabIndex = 14;
            this.TB_PositionAndGray.TabStop = false;
            this.TB_PositionAndGray.Text = "R: —  C: —  Value: — ";
            // 
            // Label_CamName
            // 
            this.Label_CamName.AutoSize = true;
            this.Label_CamName.BackColor = System.Drawing.Color.Black;
            this.Label_CamName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Label_CamName.Font = new System.Drawing.Font("黑体", 11F);
            this.Label_CamName.ForeColor = System.Drawing.Color.White;
            this.Label_CamName.Location = new System.Drawing.Point(10, 5);
            this.Label_CamName.Margin = new System.Windows.Forms.Padding(10, 5, 3, 3);
            this.Label_CamName.Name = "Label_CamName";
            this.Label_CamName.Size = new System.Drawing.Size(107, 22);
            this.Label_CamName.TabIndex = 3;
            this.Label_CamName.Text = "A面相机：";
            this.Label_CamName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // HWC_Cam
            // 
            this.HWC_Cam.BackColor = System.Drawing.Color.White;
            this.HWC_Cam.BorderColor = System.Drawing.Color.White;
            this.tableLayoutPanel1.SetColumnSpan(this.HWC_Cam, 8);
            this.HWC_Cam.Dock = System.Windows.Forms.DockStyle.Fill;
            this.HWC_Cam.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.HWC_Cam.Location = new System.Drawing.Point(3, 33);
            this.HWC_Cam.Name = "HWC_Cam";
            this.tableLayoutPanel1.SetRowSpan(this.HWC_Cam, 4);
            this.HWC_Cam.Size = new System.Drawing.Size(707, 528);
            this.HWC_Cam.TabIndex = 0;
            this.HWC_Cam.WindowSize = new System.Drawing.Size(707, 528);
            this.HWC_Cam.HMouseMove += new HalconDotNet.HMouseEventHandler(this.HWC_Cam_HMouseMove);
            // 
            // PB_LoadImage
            // 
            this.PB_LoadImage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.PB_LoadImage.Location = new System.Drawing.Point(648, 3);
            this.PB_LoadImage.Name = "PB_LoadImage";
            this.PB_LoadImage.Size = new System.Drawing.Size(23, 24);
            this.PB_LoadImage.TabIndex = 15;
            this.PB_LoadImage.TabStop = false;
            // 
            // PB_FItWindow
            // 
            this.PB_FItWindow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.PB_FItWindow.Location = new System.Drawing.Point(677, 3);
            this.PB_FItWindow.Name = "PB_FItWindow";
            this.PB_FItWindow.Size = new System.Drawing.Size(28, 24);
            this.PB_FItWindow.TabIndex = 16;
            this.PB_FItWindow.TabStop = false;
            this.PB_FItWindow.Click += new System.EventHandler(this.PB_FItWindow_Click);
            // 
            // UC_HWCshow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "UC_HWCshow";
            this.Size = new System.Drawing.Size(713, 564);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PB_LoadImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PB_FItWindow)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox TB_PositionAndGray;
        public System.Windows.Forms.Label Label_CamName;
        private System.Windows.Forms.PictureBox PB_LoadImage;
        private System.Windows.Forms.PictureBox PB_FItWindow;
        public HalconDotNet.HWindowControl HWC_Cam;
    }
}

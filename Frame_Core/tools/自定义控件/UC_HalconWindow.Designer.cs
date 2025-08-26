namespace Frame_Core
{
    partial class UC_HalconWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UC_HalconWindow));
            this.TLPanal_Cam1 = new System.Windows.Forms.TableLayoutPanel();
            this.LB_Process = new System.Windows.Forms.Label();
            this.HWC = new HalconDotNet.HWindowControl();
            this.TB_PositionAndGray = new System.Windows.Forms.TextBox();
            this.PB_FitWin = new System.Windows.Forms.PictureBox();
            this.TLPanal_Cam1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PB_FitWin)).BeginInit();
            this.SuspendLayout();
            // 
            // TLPanal_Cam1
            // 
            this.TLPanal_Cam1.BackColor = System.Drawing.Color.Black;
            this.TLPanal_Cam1.ColumnCount = 8;
            this.TLPanal_Cam1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.TLPanal_Cam1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.TLPanal_Cam1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.TLPanal_Cam1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.TLPanal_Cam1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.TLPanal_Cam1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.TLPanal_Cam1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.TLPanal_Cam1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 51F));
            this.TLPanal_Cam1.Controls.Add(this.LB_Process, 0, 0);
            this.TLPanal_Cam1.Controls.Add(this.HWC, 0, 1);
            this.TLPanal_Cam1.Controls.Add(this.TB_PositionAndGray, 3, 0);
            this.TLPanal_Cam1.Controls.Add(this.PB_FitWin, 7, 0);
            this.TLPanal_Cam1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TLPanal_Cam1.Location = new System.Drawing.Point(0, 0);
            this.TLPanal_Cam1.Margin = new System.Windows.Forms.Padding(0);
            this.TLPanal_Cam1.Name = "TLPanal_Cam1";
            this.TLPanal_Cam1.RowCount = 2;
            this.TLPanal_Cam1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.TLPanal_Cam1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TLPanal_Cam1.Size = new System.Drawing.Size(686, 423);
            this.TLPanal_Cam1.TabIndex = 17;
            // 
            // LB_Process
            // 
            this.LB_Process.AutoSize = true;
            this.LB_Process.BackColor = System.Drawing.Color.Black;
            this.TLPanal_Cam1.SetColumnSpan(this.LB_Process, 3);
            this.LB_Process.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LB_Process.Font = new System.Drawing.Font("黑体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LB_Process.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.LB_Process.Location = new System.Drawing.Point(3, 0);
            this.LB_Process.Name = "LB_Process";
            this.LB_Process.Size = new System.Drawing.Size(254, 25);
            this.LB_Process.TabIndex = 15;
            this.LB_Process.Text = "A面#涂布检测";
            this.LB_Process.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // HWC
            // 
            this.HWC.BackColor = System.Drawing.Color.White;
            this.HWC.BorderColor = System.Drawing.Color.White;
            this.TLPanal_Cam1.SetColumnSpan(this.HWC, 8);
            this.HWC.Dock = System.Windows.Forms.DockStyle.Fill;
            this.HWC.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.HWC.Location = new System.Drawing.Point(0, 25);
            this.HWC.Margin = new System.Windows.Forms.Padding(0);
            this.HWC.Name = "HWC";
            this.HWC.Size = new System.Drawing.Size(686, 398);
            this.HWC.TabIndex = 13;
            this.HWC.WindowSize = new System.Drawing.Size(686, 398);
            // 
            // TB_PositionAndGray
            // 
            this.TB_PositionAndGray.BackColor = System.Drawing.Color.Black;
            this.TB_PositionAndGray.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TLPanal_Cam1.SetColumnSpan(this.TB_PositionAndGray, 4);
            this.TB_PositionAndGray.Cursor = System.Windows.Forms.Cursors.Default;
            this.TB_PositionAndGray.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TB_PositionAndGray.Font = new System.Drawing.Font("黑体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.TB_PositionAndGray.ForeColor = System.Drawing.Color.White;
            this.TB_PositionAndGray.Location = new System.Drawing.Point(265, 8);
            this.TB_PositionAndGray.Margin = new System.Windows.Forms.Padding(5, 8, 5, 6);
            this.TB_PositionAndGray.Name = "TB_PositionAndGray";
            this.TB_PositionAndGray.ShortcutsEnabled = false;
            this.TB_PositionAndGray.Size = new System.Drawing.Size(365, 16);
            this.TB_PositionAndGray.TabIndex = 9;
            this.TB_PositionAndGray.TabStop = false;
            this.TB_PositionAndGray.Text = "R: —  C: —  Value: — ";
            // 
            // PB_FitWin
            // 
            this.PB_FitWin.BackColor = System.Drawing.SystemColors.ControlText;
            this.PB_FitWin.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("PB_FitWin.BackgroundImage")));
            this.PB_FitWin.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PB_FitWin.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PB_FitWin.ErrorImage = ((System.Drawing.Image)(resources.GetObject("PB_FitWin.ErrorImage")));
            this.PB_FitWin.Image = ((System.Drawing.Image)(resources.GetObject("PB_FitWin.Image")));
            this.PB_FitWin.InitialImage = ((System.Drawing.Image)(resources.GetObject("PB_FitWin.InitialImage")));
            this.PB_FitWin.Location = new System.Drawing.Point(638, 3);
            this.PB_FitWin.Name = "PB_FitWin";
            this.PB_FitWin.Size = new System.Drawing.Size(45, 19);
            this.PB_FitWin.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.PB_FitWin.TabIndex = 10;
            this.PB_FitWin.TabStop = false;
            // 
            // UC_HalconWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.TLPanal_Cam1);
            this.Name = "UC_HalconWindow";
            this.Size = new System.Drawing.Size(686, 423);
            this.TLPanal_Cam1.ResumeLayout(false);
            this.TLPanal_Cam1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PB_FitWin)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel TLPanal_Cam1;
        public HalconDotNet.HWindowControl HWC;
        public System.Windows.Forms.TextBox TB_PositionAndGray;
        public System.Windows.Forms.PictureBox PB_FitWin;
        public System.Windows.Forms.Label LB_Process;
    }
}

namespace Mef_VisionFrame
{
    partial class CamImageDataForm
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
            this.hWindowControl_OriginalImage = new HalconDotNet.HWindowControl();
            this.hWindowControl_DempImage = new HalconDotNet.HWindowControl();
            this.hWindowControl_DefectImage1 = new HalconDotNet.HWindowControl();
            this.hWindowControl_DefectImage2 = new HalconDotNet.HWindowControl();
            this.hWindowControl_DefectImage3 = new HalconDotNet.HWindowControl();
            this.gb_CamName = new System.Windows.Forms.GroupBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.gb_CamName.SuspendLayout();
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
            this.tableLayoutPanel1.Controls.Add(this.hWindowControl_OriginalImage, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.hWindowControl_DempImage, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.hWindowControl_DefectImage1, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.hWindowControl_DefectImage2, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.hWindowControl_DefectImage3, 4, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 21);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(880, 274);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // hWindowControl_OriginalImage
            // 
            this.hWindowControl_OriginalImage.BackColor = System.Drawing.Color.Black;
            this.hWindowControl_OriginalImage.BorderColor = System.Drawing.Color.Black;
            this.hWindowControl_OriginalImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hWindowControl_OriginalImage.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.hWindowControl_OriginalImage.Location = new System.Drawing.Point(5, 5);
            this.hWindowControl_OriginalImage.Margin = new System.Windows.Forms.Padding(5);
            this.hWindowControl_OriginalImage.Name = "hWindowControl_OriginalImage";
            this.hWindowControl_OriginalImage.Size = new System.Drawing.Size(166, 264);
            this.hWindowControl_OriginalImage.TabIndex = 5;
            this.hWindowControl_OriginalImage.WindowSize = new System.Drawing.Size(166, 264);
            this.hWindowControl_OriginalImage.HMouseDown += new HalconDotNet.HMouseEventHandler(this.hWindowControl_OriginalImage_HMouseDown);
            // 
            // hWindowControl_DempImage
            // 
            this.hWindowControl_DempImage.BackColor = System.Drawing.Color.Black;
            this.hWindowControl_DempImage.BorderColor = System.Drawing.Color.Black;
            this.hWindowControl_DempImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hWindowControl_DempImage.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.hWindowControl_DempImage.Location = new System.Drawing.Point(181, 5);
            this.hWindowControl_DempImage.Margin = new System.Windows.Forms.Padding(5);
            this.hWindowControl_DempImage.Name = "hWindowControl_DempImage";
            this.hWindowControl_DempImage.Size = new System.Drawing.Size(166, 264);
            this.hWindowControl_DempImage.TabIndex = 6;
            this.hWindowControl_DempImage.WindowSize = new System.Drawing.Size(166, 264);
            this.hWindowControl_DempImage.HMouseDown += new HalconDotNet.HMouseEventHandler(this.hWindowControl_DempImage_HMouseDown);
            // 
            // hWindowControl_DefectImage1
            // 
            this.hWindowControl_DefectImage1.BackColor = System.Drawing.Color.Black;
            this.hWindowControl_DefectImage1.BorderColor = System.Drawing.Color.Black;
            this.hWindowControl_DefectImage1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hWindowControl_DefectImage1.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.hWindowControl_DefectImage1.Location = new System.Drawing.Point(357, 5);
            this.hWindowControl_DefectImage1.Margin = new System.Windows.Forms.Padding(5);
            this.hWindowControl_DefectImage1.Name = "hWindowControl_DefectImage1";
            this.hWindowControl_DefectImage1.Size = new System.Drawing.Size(166, 264);
            this.hWindowControl_DefectImage1.TabIndex = 7;
            this.hWindowControl_DefectImage1.WindowSize = new System.Drawing.Size(166, 264);
            this.hWindowControl_DefectImage1.HMouseDown += new HalconDotNet.HMouseEventHandler(this.hWindowControl_DefectImage1_HMouseDown);
            // 
            // hWindowControl_DefectImage2
            // 
            this.hWindowControl_DefectImage2.BackColor = System.Drawing.Color.Black;
            this.hWindowControl_DefectImage2.BorderColor = System.Drawing.Color.Black;
            this.hWindowControl_DefectImage2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hWindowControl_DefectImage2.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.hWindowControl_DefectImage2.Location = new System.Drawing.Point(533, 5);
            this.hWindowControl_DefectImage2.Margin = new System.Windows.Forms.Padding(5);
            this.hWindowControl_DefectImage2.Name = "hWindowControl_DefectImage2";
            this.hWindowControl_DefectImage2.Size = new System.Drawing.Size(166, 264);
            this.hWindowControl_DefectImage2.TabIndex = 8;
            this.hWindowControl_DefectImage2.WindowSize = new System.Drawing.Size(166, 264);
            this.hWindowControl_DefectImage2.HMouseDown += new HalconDotNet.HMouseEventHandler(this.hWindowControl_DefectImage2_HMouseDown);
            // 
            // hWindowControl_DefectImage3
            // 
            this.hWindowControl_DefectImage3.BackColor = System.Drawing.Color.Black;
            this.hWindowControl_DefectImage3.BorderColor = System.Drawing.Color.Black;
            this.hWindowControl_DefectImage3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hWindowControl_DefectImage3.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.hWindowControl_DefectImage3.Location = new System.Drawing.Point(709, 5);
            this.hWindowControl_DefectImage3.Margin = new System.Windows.Forms.Padding(5);
            this.hWindowControl_DefectImage3.Name = "hWindowControl_DefectImage3";
            this.hWindowControl_DefectImage3.Size = new System.Drawing.Size(166, 264);
            this.hWindowControl_DefectImage3.TabIndex = 9;
            this.hWindowControl_DefectImage3.WindowSize = new System.Drawing.Size(166, 264);
            this.hWindowControl_DefectImage3.HMouseDown += new HalconDotNet.HMouseEventHandler(this.hWindowControl_DefectImage3_HMouseDown);
            // 
            // gb_CamName
            // 
            this.gb_CamName.Controls.Add(this.tableLayoutPanel1);
            this.gb_CamName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gb_CamName.Location = new System.Drawing.Point(0, 0);
            this.gb_CamName.Name = "gb_CamName";
            this.gb_CamName.Size = new System.Drawing.Size(886, 298);
            this.gb_CamName.TabIndex = 1;
            this.gb_CamName.TabStop = false;
            this.gb_CamName.Text = "相机名称";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // CamImageDataForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(886, 298);
            this.Controls.Add(this.gb_CamName);
            this.Name = "CamImageDataForm";
            this.Text = "CamImageDataForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CamImageDataForm_FormClosing);
            this.Load += new System.EventHandler(this.CamImageDataForm_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.gb_CamName.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox gb_CamName;
        private HalconDotNet.HWindowControl hWindowControl_OriginalImage;
        private HalconDotNet.HWindowControl hWindowControl_DempImage;
        private HalconDotNet.HWindowControl hWindowControl_DefectImage1;
        private HalconDotNet.HWindowControl hWindowControl_DefectImage2;
        private HalconDotNet.HWindowControl hWindowControl_DefectImage3;
        private System.Windows.Forms.Timer timer1;
    }
}
namespace Mef_VisionFrame
{
    partial class CamMinImageForm
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
            this.gb_CamName = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.hWindowControl_DefectImage1 = new HalconDotNet.HWindowControl();
            this.hWindowControl_DefectImage2 = new HalconDotNet.HWindowControl();
            this.hWindowControl_DefectImage3 = new HalconDotNet.HWindowControl();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.gb_CamName.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // gb_CamName
            // 
            this.gb_CamName.Controls.Add(this.tableLayoutPanel1);
            this.gb_CamName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gb_CamName.Location = new System.Drawing.Point(0, 0);
            this.gb_CamName.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.gb_CamName.Name = "gb_CamName";
            this.gb_CamName.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.gb_CamName.Size = new System.Drawing.Size(414, 146);
            this.gb_CamName.TabIndex = 2;
            this.gb_CamName.TabStop = false;
            this.gb_CamName.Text = "名称";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.Controls.Add(this.hWindowControl_DefectImage1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.hWindowControl_DefectImage2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.hWindowControl_DefectImage3, 2, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(2, 16);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(410, 128);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // hWindowControl_DefectImage1
            // 
            this.hWindowControl_DefectImage1.BackColor = System.Drawing.Color.Black;
            this.hWindowControl_DefectImage1.BorderColor = System.Drawing.Color.Black;
            this.hWindowControl_DefectImage1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hWindowControl_DefectImage1.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.hWindowControl_DefectImage1.Location = new System.Drawing.Point(4, 4);
            this.hWindowControl_DefectImage1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.hWindowControl_DefectImage1.Name = "hWindowControl_DefectImage1";
            this.hWindowControl_DefectImage1.Size = new System.Drawing.Size(128, 120);
            this.hWindowControl_DefectImage1.TabIndex = 7;
            this.hWindowControl_DefectImage1.WindowSize = new System.Drawing.Size(128, 120);
            this.hWindowControl_DefectImage1.HMouseDown += new HalconDotNet.HMouseEventHandler(this.hWindowControl_DefectImage1_HMouseDown);
            // 
            // hWindowControl_DefectImage2
            // 
            this.hWindowControl_DefectImage2.BackColor = System.Drawing.Color.Black;
            this.hWindowControl_DefectImage2.BorderColor = System.Drawing.Color.Black;
            this.hWindowControl_DefectImage2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hWindowControl_DefectImage2.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.hWindowControl_DefectImage2.Location = new System.Drawing.Point(140, 4);
            this.hWindowControl_DefectImage2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.hWindowControl_DefectImage2.Name = "hWindowControl_DefectImage2";
            this.hWindowControl_DefectImage2.Size = new System.Drawing.Size(128, 120);
            this.hWindowControl_DefectImage2.TabIndex = 8;
            this.hWindowControl_DefectImage2.WindowSize = new System.Drawing.Size(128, 120);
            this.hWindowControl_DefectImage2.HMouseDown += new HalconDotNet.HMouseEventHandler(this.hWindowControl_DefectImage2_HMouseDown);
            // 
            // hWindowControl_DefectImage3
            // 
            this.hWindowControl_DefectImage3.BackColor = System.Drawing.Color.Black;
            this.hWindowControl_DefectImage3.BorderColor = System.Drawing.Color.Black;
            this.hWindowControl_DefectImage3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hWindowControl_DefectImage3.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.hWindowControl_DefectImage3.Location = new System.Drawing.Point(276, 4);
            this.hWindowControl_DefectImage3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.hWindowControl_DefectImage3.Name = "hWindowControl_DefectImage3";
            this.hWindowControl_DefectImage3.Size = new System.Drawing.Size(130, 120);
            this.hWindowControl_DefectImage3.TabIndex = 9;
            this.hWindowControl_DefectImage3.WindowSize = new System.Drawing.Size(130, 120);
            this.hWindowControl_DefectImage3.HMouseDown += new HalconDotNet.HMouseEventHandler(this.hWindowControl_DefectImage3_HMouseDown);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // CamMinImageForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(414, 146);
            this.Controls.Add(this.gb_CamName);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "CamMinImageForm";
            this.Text = "CamMinImageForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CamMinImageForm_FormClosing);
            this.Load += new System.EventHandler(this.CamMinImageForm_Load);
            this.gb_CamName.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gb_CamName;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private HalconDotNet.HWindowControl hWindowControl_DefectImage1;
        private HalconDotNet.HWindowControl hWindowControl_DefectImage2;
        private HalconDotNet.HWindowControl hWindowControl_DefectImage3;
        private System.Windows.Forms.Timer timer1;
    }
}
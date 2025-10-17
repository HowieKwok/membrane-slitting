namespace Frame_Core
{
    partial class FormDefectPointInspection
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDefectPointInspection));
            this.btn_StartPointInspection = new System.Windows.Forms.Button();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.lb_Time = new System.Windows.Forms.Label();
            this.btn_Run = new System.Windows.Forms.Button();
            this.btn_Preview = new System.Windows.Forms.Button();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.tbx_PointCheckImgPath = new System.Windows.Forms.TextBox();
            this.linkLabel_ImgPath = new System.Windows.Forms.LinkLabel();
            this.btn_Next = new System.Windows.Forms.Button();
            this.btn_Save = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel_Image = new System.Windows.Forms.TableLayoutPanel();
            this.LoadImage = new System.Windows.Forms.PictureBox();
            this.TB_PositionAndGray = new System.Windows.Forms.TextBox();
            this.HWC_Display = new HalconDotNet.HWindowControl();
            this.RestoreWindow = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel3.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel_Image.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LoadImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RestoreWindow)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_StartPointInspection
            // 
            resources.ApplyResources(this.btn_StartPointInspection, "btn_StartPointInspection");
            this.btn_StartPointInspection.Name = "btn_StartPointInspection";
            this.btn_StartPointInspection.UseVisualStyleBackColor = true;
            this.btn_StartPointInspection.Click += new System.EventHandler(this.btn_StartPointInspection_Click);
            // 
            // linkLabel1
            // 
            resources.ApplyResources(this.linkLabel1, "linkLabel1");
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.TabStop = true;
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // lb_Time
            // 
            resources.ApplyResources(this.lb_Time, "lb_Time");
            this.lb_Time.ForeColor = System.Drawing.SystemColors.Highlight;
            this.lb_Time.Name = "lb_Time";
            // 
            // btn_Run
            // 
            this.btn_Run.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.btn_Run, "btn_Run");
            this.btn_Run.Name = "btn_Run";
            this.btn_Run.UseVisualStyleBackColor = false;
            this.btn_Run.Click += new System.EventHandler(this.btn_Run_Click);
            // 
            // btn_Preview
            // 
            this.btn_Preview.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.btn_Preview, "btn_Preview");
            this.btn_Preview.Name = "btn_Preview";
            this.btn_Preview.UseVisualStyleBackColor = false;
            this.btn_Preview.Click += new System.EventHandler(this.btn_Preview_Click);
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7});
            resources.ApplyResources(this.listView1, "listView1");
            this.listView1.GridLines = true;
            this.listView1.HideSelection = false;
            this.listView1.Name = "listView1";
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // columnHeader2
            // 
            resources.ApplyResources(this.columnHeader2, "columnHeader2");
            // 
            // columnHeader3
            // 
            resources.ApplyResources(this.columnHeader3, "columnHeader3");
            // 
            // columnHeader4
            // 
            resources.ApplyResources(this.columnHeader4, "columnHeader4");
            // 
            // columnHeader5
            // 
            resources.ApplyResources(this.columnHeader5, "columnHeader5");
            // 
            // columnHeader6
            // 
            resources.ApplyResources(this.columnHeader6, "columnHeader6");
            // 
            // columnHeader7
            // 
            resources.ApplyResources(this.columnHeader7, "columnHeader7");
            // 
            // tableLayoutPanel3
            // 
            resources.ApplyResources(this.tableLayoutPanel3, "tableLayoutPanel3");
            this.tableLayoutPanel1.SetColumnSpan(this.tableLayoutPanel3, 2);
            this.tableLayoutPanel3.Controls.Add(this.listView1, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.panel6, 0, 0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.panel6.Controls.Add(this.tbx_PointCheckImgPath);
            this.panel6.Controls.Add(this.linkLabel_ImgPath);
            resources.ApplyResources(this.panel6, "panel6");
            this.panel6.Name = "panel6";
            // 
            // tbx_PointCheckImgPath
            // 
            resources.ApplyResources(this.tbx_PointCheckImgPath, "tbx_PointCheckImgPath");
            this.tbx_PointCheckImgPath.Name = "tbx_PointCheckImgPath";
            this.tbx_PointCheckImgPath.ReadOnly = true;
            this.tbx_PointCheckImgPath.TextChanged += new System.EventHandler(this.tbx_PointCheckImgPath_TextChanged);
            // 
            // linkLabel_ImgPath
            // 
            resources.ApplyResources(this.linkLabel_ImgPath, "linkLabel_ImgPath");
            this.linkLabel_ImgPath.Name = "linkLabel_ImgPath";
            this.linkLabel_ImgPath.TabStop = true;
            this.linkLabel_ImgPath.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_ImgPath_LinkClicked);
            // 
            // btn_Next
            // 
            this.btn_Next.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.btn_Next, "btn_Next");
            this.btn_Next.Name = "btn_Next";
            this.btn_Next.UseVisualStyleBackColor = false;
            this.btn_Next.Click += new System.EventHandler(this.btn_Next_Click);
            // 
            // btn_Save
            // 
            resources.ApplyResources(this.btn_Save, "btn_Save");
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.UseVisualStyleBackColor = true;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // panel1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.panel1, 3);
            this.panel1.Controls.Add(this.btn_StartPointInspection);
            this.panel1.Controls.Add(this.linkLabel1);
            this.panel1.Controls.Add(this.lb_Time);
            this.panel1.Controls.Add(this.btn_Run);
            this.panel1.Controls.Add(this.btn_Preview);
            this.panel1.Controls.Add(this.btn_Next);
            this.panel1.Controls.Add(this.btn_Save);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel_Image, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 0, 1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // tableLayoutPanel_Image
            // 
            this.tableLayoutPanel_Image.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            resources.ApplyResources(this.tableLayoutPanel_Image, "tableLayoutPanel_Image");
            this.tableLayoutPanel1.SetColumnSpan(this.tableLayoutPanel_Image, 2);
            this.tableLayoutPanel_Image.Controls.Add(this.LoadImage, 0, 2);
            this.tableLayoutPanel_Image.Controls.Add(this.TB_PositionAndGray, 0, 0);
            this.tableLayoutPanel_Image.Controls.Add(this.HWC_Display, 0, 1);
            this.tableLayoutPanel_Image.Controls.Add(this.RestoreWindow, 14, 2);
            this.tableLayoutPanel_Image.Name = "tableLayoutPanel_Image";
            // 
            // LoadImage
            // 
            this.LoadImage.BackColor = System.Drawing.Color.Blue;
            resources.ApplyResources(this.LoadImage, "LoadImage");
            this.LoadImage.Name = "LoadImage";
            this.LoadImage.TabStop = false;
            this.LoadImage.Click += new System.EventHandler(this.LoadImage_Click);
            // 
            // TB_PositionAndGray
            // 
            this.TB_PositionAndGray.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.TB_PositionAndGray.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tableLayoutPanel_Image.SetColumnSpan(this.TB_PositionAndGray, 8);
            this.TB_PositionAndGray.Cursor = System.Windows.Forms.Cursors.Default;
            resources.ApplyResources(this.TB_PositionAndGray, "TB_PositionAndGray");
            this.TB_PositionAndGray.ForeColor = System.Drawing.Color.White;
            this.TB_PositionAndGray.Name = "TB_PositionAndGray";
            this.TB_PositionAndGray.ShortcutsEnabled = false;
            this.TB_PositionAndGray.TabStop = false;
            // 
            // HWC_Display
            // 
            this.HWC_Display.BackColor = System.Drawing.Color.White;
            this.HWC_Display.BorderColor = System.Drawing.Color.White;
            this.tableLayoutPanel_Image.SetColumnSpan(this.HWC_Display, 15);
            resources.ApplyResources(this.HWC_Display, "HWC_Display");
            this.HWC_Display.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.HWC_Display.Name = "HWC_Display";
            this.HWC_Display.WindowSize = new System.Drawing.Size(971, 424);
            // 
            // RestoreWindow
            // 
            this.RestoreWindow.BackColor = System.Drawing.Color.Blue;
            resources.ApplyResources(this.RestoreWindow, "RestoreWindow");
            this.RestoreWindow.Name = "RestoreWindow";
            this.RestoreWindow.TabStop = false;
            // 
            // FormDefectPointInspection
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "FormDefectPointInspection";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_DefectPointInspection_FormClosing);
            this.Load += new System.EventHandler(this.Form_DefectPointInspection_Load);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel_Image.ResumeLayout(false);
            this.tableLayoutPanel_Image.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LoadImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RestoreWindow)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btn_StartPointInspection;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Label lb_Time;
        private System.Windows.Forms.Button btn_Run;
        private System.Windows.Forms.Button btn_Preview;
        private System.Windows.Forms.Button btn_Next;
        private System.Windows.Forms.Button btn_Save;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.TextBox tbx_PointCheckImgPath;
        private System.Windows.Forms.LinkLabel linkLabel_ImgPath;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_Image;
        private System.Windows.Forms.PictureBox LoadImage;
        private System.Windows.Forms.TextBox TB_PositionAndGray;
        private HalconDotNet.HWindowControl HWC_Display;
        private System.Windows.Forms.PictureBox RestoreWindow;
    }
}
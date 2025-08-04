namespace Mef_VisionFrame
{
    partial class SliceDataForm
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
            this.lb_SliceSelect = new System.Windows.Forms.ListBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel_Cam1 = new System.Windows.Forms.Panel();
            this.panel_Cam2 = new System.Windows.Forms.Panel();
            this.panel_Cam3 = new System.Windows.Forms.Panel();
            this.panel_Cam4 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lb_SliceSelect
            // 
            this.lb_SliceSelect.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lb_SliceSelect.FormattingEnabled = true;
            this.lb_SliceSelect.ItemHeight = 15;
            this.lb_SliceSelect.Location = new System.Drawing.Point(3, 3);
            this.lb_SliceSelect.Name = "lb_SliceSelect";
            this.tableLayoutPanel1.SetRowSpan(this.lb_SliceSelect, 4);
            this.lb_SliceSelect.Size = new System.Drawing.Size(163, 693);
            this.lb_SliceSelect.TabIndex = 0;
            this.lb_SliceSelect.DoubleClick += new System.EventHandler(this.lb_SliceSelect_DoubleClick);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14.0249F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 85.97511F));
            this.tableLayoutPanel1.Controls.Add(this.panel_Cam4, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.panel_Cam3, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.panel_Cam2, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.lb_SliceSelect, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel_Cam1, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1205, 699);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // panel_Cam1
            // 
            this.panel_Cam1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_Cam1.Location = new System.Drawing.Point(172, 3);
            this.panel_Cam1.Name = "panel_Cam1";
            this.panel_Cam1.Size = new System.Drawing.Size(1030, 168);
            this.panel_Cam1.TabIndex = 1;
            // 
            // panel_Cam2
            // 
            this.panel_Cam2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_Cam2.Location = new System.Drawing.Point(172, 177);
            this.panel_Cam2.Name = "panel_Cam2";
            this.panel_Cam2.Size = new System.Drawing.Size(1030, 168);
            this.panel_Cam2.TabIndex = 2;
            // 
            // panel_Cam3
            // 
            this.panel_Cam3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_Cam3.Location = new System.Drawing.Point(172, 351);
            this.panel_Cam3.Name = "panel_Cam3";
            this.panel_Cam3.Size = new System.Drawing.Size(1030, 168);
            this.panel_Cam3.TabIndex = 3;
            // 
            // panel_Cam4
            // 
            this.panel_Cam4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_Cam4.Location = new System.Drawing.Point(172, 525);
            this.panel_Cam4.Name = "panel_Cam4";
            this.panel_Cam4.Size = new System.Drawing.Size(1030, 171);
            this.panel_Cam4.TabIndex = 4;
            // 
            // SliceDataForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1205, 699);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "SliceDataForm";
            this.Text = "SliceDataForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SliceDataForm_FormClosing);
            this.Load += new System.EventHandler(this.SliceDataForm_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lb_SliceSelect;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel_Cam4;
        private System.Windows.Forms.Panel panel_Cam3;
        private System.Windows.Forms.Panel panel_Cam2;
        private System.Windows.Forms.Panel panel_Cam1;
    }
}
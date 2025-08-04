namespace Frame_Core
{
    partial class EmulationPathForm
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
            this.tableLayoutPanel_Main = new System.Windows.Forms.TableLayoutPanel();
            this.tb_EmulationPath = new System.Windows.Forms.TextBox();
            this.linkLabel_EmulationPathLoad = new System.Windows.Forms.LinkLabel();
            this.tableLayoutPanel_Main.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel_Main
            // 
            this.tableLayoutPanel_Main.ColumnCount = 2;
            this.tableLayoutPanel_Main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14.875F));
            this.tableLayoutPanel_Main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 85.125F));
            this.tableLayoutPanel_Main.Controls.Add(this.tb_EmulationPath, 0, 0);
            this.tableLayoutPanel_Main.Controls.Add(this.linkLabel_EmulationPathLoad, 0, 0);
            this.tableLayoutPanel_Main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel_Main.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel_Main.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel_Main.Name = "tableLayoutPanel_Main";
            this.tableLayoutPanel_Main.RowCount = 1;
            this.tableLayoutPanel_Main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel_Main.Size = new System.Drawing.Size(600, 60);
            this.tableLayoutPanel_Main.TabIndex = 0;
            // 
            // tb_EmulationPath
            // 
            this.tb_EmulationPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tb_EmulationPath.Font = new System.Drawing.Font("宋体", 12F);
            this.tb_EmulationPath.Location = new System.Drawing.Point(91, 16);
            this.tb_EmulationPath.Margin = new System.Windows.Forms.Padding(2, 16, 15, 2);
            this.tb_EmulationPath.Name = "tb_EmulationPath";
            this.tb_EmulationPath.Size = new System.Drawing.Size(494, 26);
            this.tb_EmulationPath.TabIndex = 5;
            this.tb_EmulationPath.TextChanged += new System.EventHandler(this.tb_EmulationPath_TextChanged);
            // 
            // linkLabel_EmulationPathLoad
            // 
            this.linkLabel_EmulationPathLoad.AutoSize = true;
            this.linkLabel_EmulationPathLoad.Dock = System.Windows.Forms.DockStyle.Fill;
            this.linkLabel_EmulationPathLoad.Font = new System.Drawing.Font("宋体", 12F);
            this.linkLabel_EmulationPathLoad.Location = new System.Drawing.Point(2, 20);
            this.linkLabel_EmulationPathLoad.Margin = new System.Windows.Forms.Padding(2, 20, 15, 2);
            this.linkLabel_EmulationPathLoad.Name = "linkLabel_EmulationPathLoad";
            this.linkLabel_EmulationPathLoad.Size = new System.Drawing.Size(72, 38);
            this.linkLabel_EmulationPathLoad.TabIndex = 4;
            this.linkLabel_EmulationPathLoad.TabStop = true;
            this.linkLabel_EmulationPathLoad.Text = "仿真路径";
            this.linkLabel_EmulationPathLoad.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_EmulationPathLoad_LinkClicked);
            // 
            // EmulationPathForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 60);
            this.Controls.Add(this.tableLayoutPanel_Main);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "EmulationPathForm";
            this.Text = "EmulationPathForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.EmulationPathForm_FormClosing);
            this.Load += new System.EventHandler(this.EmulationPathForm_Load);
            this.tableLayoutPanel_Main.ResumeLayout(false);
            this.tableLayoutPanel_Main.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_Main;
        private System.Windows.Forms.LinkLabel linkLabel_EmulationPathLoad;
        private System.Windows.Forms.TextBox tb_EmulationPath;
    }
}
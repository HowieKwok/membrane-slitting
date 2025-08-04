namespace LogTool
{
    partial class LogForm
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

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("运行日志");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("设置日志");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("报错日志");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("当前日志", new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3});
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.richTextBox_LogInfo = new System.Windows.Forms.RichTextBox();
            this.Tv_Select = new System.Windows.Forms.TreeView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.nud_LogSaveDay = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.nud_SaveFileSize = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.linkLabel_LogSavePath = new System.Windows.Forms.LinkLabel();
            this.tbx_LogSavePath = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_LogSaveDay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_SaveFileSize)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 38.85965F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 61.14035F));
            this.tableLayoutPanel1.Controls.Add(this.richTextBox_LogInfo, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.Tv_Select, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1062, 601);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // richTextBox_LogInfo
            // 
            this.richTextBox_LogInfo.BackColor = System.Drawing.SystemColors.Window;
            this.richTextBox_LogInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox_LogInfo.Location = new System.Drawing.Point(415, 123);
            this.richTextBox_LogInfo.Name = "richTextBox_LogInfo";
            this.richTextBox_LogInfo.ReadOnly = true;
            this.richTextBox_LogInfo.Size = new System.Drawing.Size(644, 475);
            this.richTextBox_LogInfo.TabIndex = 5;
            this.richTextBox_LogInfo.Text = "";
            this.richTextBox_LogInfo.WordWrap = false;
            // 
            // Tv_Select
            // 
            this.Tv_Select.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Tv_Select.Font = new System.Drawing.Font("宋体", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Tv_Select.ItemHeight = 25;
            this.Tv_Select.Location = new System.Drawing.Point(2, 122);
            this.Tv_Select.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Tv_Select.Name = "Tv_Select";
            treeNode1.Name = "节点0";
            treeNode1.Text = "运行日志";
            treeNode2.Name = "节点3";
            treeNode2.Text = "设置日志";
            treeNode3.Name = "节点4";
            treeNode3.Text = "报错日志";
            treeNode4.Name = "节点0";
            treeNode4.Text = "当前日志";
            this.Tv_Select.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode4});
            this.Tv_Select.Size = new System.Drawing.Size(408, 477);
            this.Tv_Select.TabIndex = 2;
            this.Tv_Select.DrawNode += new System.Windows.Forms.DrawTreeNodeEventHandler(this.Tv_Select_DrawNode);
            this.Tv_Select.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.Tv_Select_NodeMouseDoubleClick);
            // 
            // panel1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.panel1, 2);
            this.panel1.Controls.Add(this.nud_LogSaveDay);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.nud_SaveFileSize);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.linkLabel_LogSavePath);
            this.panel1.Controls.Add(this.tbx_LogSavePath);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(2, 2);
            this.panel1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1058, 116);
            this.panel1.TabIndex = 4;
            // 
            // nud_LogSaveDay
            // 
            this.nud_LogSaveDay.Location = new System.Drawing.Point(933, 62);
            this.nud_LogSaveDay.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.nud_LogSaveDay.Name = "nud_LogSaveDay";
            this.nud_LogSaveDay.Size = new System.Drawing.Size(90, 21);
            this.nud_LogSaveDay.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(844, 66);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "日志保存天数：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(809, 64);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(11, 12);
            this.label2.TabIndex = 5;
            this.label2.Text = "M";
            // 
            // nud_SaveFileSize
            // 
            this.nud_SaveFileSize.Location = new System.Drawing.Point(715, 62);
            this.nud_SaveFileSize.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.nud_SaveFileSize.Name = "nud_SaveFileSize";
            this.nud_SaveFileSize.Size = new System.Drawing.Size(90, 21);
            this.nud_SaveFileSize.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(626, 66);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "保存文件大小：";
            // 
            // linkLabel_LogSavePath
            // 
            this.linkLabel_LogSavePath.AutoSize = true;
            this.linkLabel_LogSavePath.Font = new System.Drawing.Font("宋体", 12F);
            this.linkLabel_LogSavePath.Location = new System.Drawing.Point(14, 22);
            this.linkLabel_LogSavePath.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.linkLabel_LogSavePath.Name = "linkLabel_LogSavePath";
            this.linkLabel_LogSavePath.Size = new System.Drawing.Size(119, 16);
            this.linkLabel_LogSavePath.TabIndex = 2;
            this.linkLabel_LogSavePath.TabStop = true;
            this.linkLabel_LogSavePath.Text = "日志保存路径：";
            this.linkLabel_LogSavePath.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_LogSavePath_LinkClicked);
            // 
            // tbx_LogSavePath
            // 
            this.tbx_LogSavePath.Location = new System.Drawing.Point(140, 19);
            this.tbx_LogSavePath.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tbx_LogSavePath.Name = "tbx_LogSavePath";
            this.tbx_LogSavePath.Size = new System.Drawing.Size(884, 21);
            this.tbx_LogSavePath.TabIndex = 1;
            // 
            // LogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1062, 601);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "LogForm";
            this.Text = "LogInfo";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_LogSaveDay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_SaveFileSize)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TreeView Tv_Select;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.LinkLabel linkLabel_LogSavePath;
        private System.Windows.Forms.TextBox tbx_LogSavePath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown nud_LogSaveDay;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nud_SaveFileSize;
        private System.Windows.Forms.RichTextBox richTextBox_LogInfo;
    }
}


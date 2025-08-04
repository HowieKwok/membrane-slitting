namespace Frame_Core
{
    partial class UC_Message
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UC_Message));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage_LogAlarm = new System.Windows.Forms.TabPage();
            this.dgv_Alarm = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.清除ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabPage_LogDebug = new System.Windows.Forms.TabPage();
            this.rtbx_Debug = new System.Windows.Forms.RichTextBox();
            this.tabPage_LogError = new System.Windows.Forms.TabPage();
            this.rtbx_Error = new System.Windows.Forms.RichTextBox();
            this.tabPage_LogCommution = new System.Windows.Forms.TabPage();
            this.rtbx_Comm = new System.Windows.Forms.RichTextBox();
            this.timerUpdate = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel_Main = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lb_LogRow = new System.Windows.Forms.Label();
            this.LB_Log = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabPage_LogAlarm.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Alarm)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.tabPage_LogDebug.SuspendLayout();
            this.tabPage_LogError.SuspendLayout();
            this.tabPage_LogCommution.SuspendLayout();
            this.tableLayoutPanel_Main.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage_LogAlarm);
            this.tabControl1.Controls.Add(this.tabPage_LogDebug);
            this.tabControl1.Controls.Add(this.tabPage_LogError);
            this.tabControl1.Controls.Add(this.tabPage_LogCommution);
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPage_LogAlarm
            // 
            this.tabPage_LogAlarm.Controls.Add(this.dgv_Alarm);
            resources.ApplyResources(this.tabPage_LogAlarm, "tabPage_LogAlarm");
            this.tabPage_LogAlarm.Name = "tabPage_LogAlarm";
            this.tabPage_LogAlarm.UseVisualStyleBackColor = true;
            // 
            // dgv_Alarm
            // 
            this.dgv_Alarm.AllowUserToAddRows = false;
            this.dgv_Alarm.AllowUserToDeleteRows = false;
            this.dgv_Alarm.AllowUserToOrderColumns = true;
            this.dgv_Alarm.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_Alarm.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3});
            this.dgv_Alarm.ContextMenuStrip = this.contextMenuStrip1;
            resources.ApplyResources(this.dgv_Alarm, "dgv_Alarm");
            this.dgv_Alarm.Name = "dgv_Alarm";
            this.dgv_Alarm.RowHeadersVisible = false;
            this.dgv_Alarm.RowTemplate.Height = 23;
            // 
            // Column1
            // 
            this.Column1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column1.FillWeight = 20F;
            resources.ApplyResources(this.Column1, "Column1");
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Column2
            // 
            this.Column2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column2.FillWeight = 12F;
            resources.ApplyResources(this.Column2, "Column2");
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            this.Column2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Column3
            // 
            this.Column3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column3.FillWeight = 68F;
            resources.ApplyResources(this.Column3, "Column3");
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            this.Column3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.清除ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            resources.ApplyResources(this.contextMenuStrip1, "contextMenuStrip1");
            // 
            // 清除ToolStripMenuItem
            // 
            this.清除ToolStripMenuItem.Name = "清除ToolStripMenuItem";
            resources.ApplyResources(this.清除ToolStripMenuItem, "清除ToolStripMenuItem");
            this.清除ToolStripMenuItem.Click += new System.EventHandler(this.清除ToolStripMenuItem_Click);
            // 
            // tabPage_LogDebug
            // 
            this.tabPage_LogDebug.Controls.Add(this.rtbx_Debug);
            resources.ApplyResources(this.tabPage_LogDebug, "tabPage_LogDebug");
            this.tabPage_LogDebug.Name = "tabPage_LogDebug";
            this.tabPage_LogDebug.UseVisualStyleBackColor = true;
            // 
            // rtbx_Debug
            // 
            this.rtbx_Debug.ContextMenuStrip = this.contextMenuStrip1;
            resources.ApplyResources(this.rtbx_Debug, "rtbx_Debug");
            this.rtbx_Debug.Name = "rtbx_Debug";
            // 
            // tabPage_LogError
            // 
            this.tabPage_LogError.Controls.Add(this.rtbx_Error);
            resources.ApplyResources(this.tabPage_LogError, "tabPage_LogError");
            this.tabPage_LogError.Name = "tabPage_LogError";
            this.tabPage_LogError.UseVisualStyleBackColor = true;
            // 
            // rtbx_Error
            // 
            this.rtbx_Error.ContextMenuStrip = this.contextMenuStrip1;
            resources.ApplyResources(this.rtbx_Error, "rtbx_Error");
            this.rtbx_Error.Name = "rtbx_Error";
            // 
            // tabPage_LogCommution
            // 
            this.tabPage_LogCommution.Controls.Add(this.rtbx_Comm);
            resources.ApplyResources(this.tabPage_LogCommution, "tabPage_LogCommution");
            this.tabPage_LogCommution.Name = "tabPage_LogCommution";
            this.tabPage_LogCommution.UseVisualStyleBackColor = true;
            // 
            // rtbx_Comm
            // 
            this.rtbx_Comm.ContextMenuStrip = this.contextMenuStrip1;
            resources.ApplyResources(this.rtbx_Comm, "rtbx_Comm");
            this.rtbx_Comm.Name = "rtbx_Comm";
            // 
            // timerUpdate
            // 
            this.timerUpdate.Enabled = true;
            this.timerUpdate.Interval = 200;
            this.timerUpdate.Tick += new System.EventHandler(this.timerUpdate_Tick);
            // 
            // tableLayoutPanel_Main
            // 
            resources.ApplyResources(this.tableLayoutPanel_Main, "tableLayoutPanel_Main");
            this.tableLayoutPanel_Main.Controls.Add(this.tabControl1, 0, 1);
            this.tableLayoutPanel_Main.Controls.Add(this.tableLayoutPanel1, 0, 0);
            this.tableLayoutPanel_Main.Name = "tableLayoutPanel_Main";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.lb_LogRow, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.LB_Log, 1, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // lb_LogRow
            // 
            resources.ApplyResources(this.lb_LogRow, "lb_LogRow");
            this.lb_LogRow.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lb_LogRow.Name = "lb_LogRow";
            // 
            // LB_Log
            // 
            resources.ApplyResources(this.LB_Log, "LB_Log");
            this.LB_Log.BackColor = System.Drawing.SystemColors.Control;
            this.LB_Log.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.LB_Log.Name = "LB_Log";
            // 
            // UC_Message
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel_Main);
            this.Name = "UC_Message";
            this.Load += new System.EventHandler(this.UC_Message_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage_LogAlarm.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Alarm)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.tabPage_LogDebug.ResumeLayout(false);
            this.tabPage_LogError.ResumeLayout(false);
            this.tabPage_LogCommution.ResumeLayout(false);
            this.tableLayoutPanel_Main.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage_LogAlarm;
        private System.Windows.Forms.TabPage tabPage_LogDebug;
        private System.Windows.Forms.TabPage tabPage_LogError;
        private System.Windows.Forms.Timer timerUpdate;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_Main;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label LB_Log;
        private System.Windows.Forms.Label lb_LogRow;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 清除ToolStripMenuItem;
        private System.Windows.Forms.DataGridView dgv_Alarm;
        private System.Windows.Forms.RichTextBox rtbx_Debug;
        private System.Windows.Forms.RichTextBox rtbx_Error;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.TabPage tabPage_LogCommution;
        private System.Windows.Forms.RichTextBox rtbx_Comm;
    }
}

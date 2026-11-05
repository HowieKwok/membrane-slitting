namespace Mef_VisionFrame
{
    partial class UC_ToolView
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
            this.panel_DataShow = new System.Windows.Forms.Panel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.input_FilmRoll = new AntdUI.Input();
            this.label1 = new AntdUI.Label();
            this.btn_FilmRollSwitch = new Frame_Core.ControlButton();
            this.stackPanel1 = new AntdUI.StackPanel();
            this.btn_ClearCutData = new System.Windows.Forms.Button();
            this.btn_Reset = new System.Windows.Forms.Button();
            this.switch_LightOnOff = new AntdUI.Switch();
            this.divider2 = new AntdUI.Divider();
            this.btn_next = new System.Windows.Forms.Button();
            this.btn_run = new Frame_Core.ControlButton();
            this.btn_Prev = new System.Windows.Forms.Button();
            this.divider1 = new AntdUI.Divider();
            this.btn_AutoSim = new Frame_Core.ControlButton();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.stackPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.panel_DataShow, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1462, 763);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // panel_DataShow
            // 
            this.panel_DataShow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_DataShow.Location = new System.Drawing.Point(0, 40);
            this.panel_DataShow.Margin = new System.Windows.Forms.Padding(0);
            this.panel_DataShow.Name = "panel_DataShow";
            this.panel_DataShow.Size = new System.Drawing.Size(1462, 723);
            this.panel_DataShow.TabIndex = 21;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.BackColor = System.Drawing.SystemColors.Control;
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 45.70384F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 54.29616F));
            this.tableLayoutPanel2.Controls.Add(this.flowLayoutPanel1, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.stackPanel1, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1462, 40);
            this.tableLayoutPanel2.TabIndex = 22;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.input_FilmRoll);
            this.flowLayoutPanel1.Controls.Add(this.label1);
            this.flowLayoutPanel1.Controls.Add(this.btn_FilmRollSwitch);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(668, 0);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(794, 40);
            this.flowLayoutPanel1.TabIndex = 23;
            // 
            // input_FilmRoll
            // 
            this.input_FilmRoll.Location = new System.Drawing.Point(609, 3);
            this.input_FilmRoll.Name = "input_FilmRoll";
            this.input_FilmRoll.Size = new System.Drawing.Size(182, 34);
            this.input_FilmRoll.TabIndex = 0;
            this.input_FilmRoll.Text = "input1";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(544, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 34);
            this.label1.TabIndex = 9;
            this.label1.Text = "膜卷号：";
            // 
            // btn_FilmRollSwitch
            // 
            this.btn_FilmRollSwitch.BackColor = System.Drawing.Color.Lime;
            this.btn_FilmRollSwitch.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btn_FilmRollSwitch.CheckedBackColor = System.Drawing.Color.DodgerBlue;
            this.btn_FilmRollSwitch.CheckedText = "结束膜卷";
            this.btn_FilmRollSwitch.Location = new System.Drawing.Point(455, 3);
            this.btn_FilmRollSwitch.Name = "btn_FilmRollSwitch";
            this.btn_FilmRollSwitch.Size = new System.Drawing.Size(83, 34);
            this.btn_FilmRollSwitch.TabIndex = 9;
            this.btn_FilmRollSwitch.Text = "开始膜卷";
            this.btn_FilmRollSwitch.UnCheckedBackColor = System.Drawing.Color.Lime;
            this.btn_FilmRollSwitch.UnCheckedText = "开始膜卷";
            this.btn_FilmRollSwitch.UseVisualStyleBackColor = false;
            this.btn_FilmRollSwitch.Click += new System.EventHandler(this.btn_FilmRollSwitch_Click);
            // 
            // stackPanel1
            // 
            this.stackPanel1.Controls.Add(this.btn_ClearCutData);
            this.stackPanel1.Controls.Add(this.btn_Reset);
            this.stackPanel1.Controls.Add(this.switch_LightOnOff);
            this.stackPanel1.Controls.Add(this.divider2);
            this.stackPanel1.Controls.Add(this.btn_next);
            this.stackPanel1.Controls.Add(this.btn_run);
            this.stackPanel1.Controls.Add(this.btn_Prev);
            this.stackPanel1.Controls.Add(this.divider1);
            this.stackPanel1.Controls.Add(this.btn_AutoSim);
            this.stackPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stackPanel1.Location = new System.Drawing.Point(0, 0);
            this.stackPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.stackPanel1.Name = "stackPanel1";
            this.stackPanel1.Size = new System.Drawing.Size(668, 40);
            this.stackPanel1.TabIndex = 22;
            this.stackPanel1.Text = "stackPanel1";
            // 
            // btn_ClearCutData
            // 
            this.btn_ClearCutData.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btn_ClearCutData.Location = new System.Drawing.Point(385, 3);
            this.btn_ClearCutData.Name = "btn_ClearCutData";
            this.btn_ClearCutData.Size = new System.Drawing.Size(93, 34);
            this.btn_ClearCutData.TabIndex = 14;
            this.btn_ClearCutData.Text = "清除纠偏数据";
            this.btn_ClearCutData.UseVisualStyleBackColor = true;
            this.btn_ClearCutData.Click += new System.EventHandler(this.btn_ClearCutData_Click);
            // 
            // btn_Reset
            // 
            this.btn_Reset.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btn_Reset.Image = global::Mef_VisionFrame.Properties.Resources.icon_reset;
            this.btn_Reset.Location = new System.Drawing.Point(345, 3);
            this.btn_Reset.Name = "btn_Reset";
            this.btn_Reset.Size = new System.Drawing.Size(34, 34);
            this.btn_Reset.TabIndex = 13;
            this.btn_Reset.UseVisualStyleBackColor = true;
            this.btn_Reset.Click += new System.EventHandler(this.btn_Reset_Click);
            // 
            // switch_LightOnOff
            // 
            this.switch_LightOnOff.AutoCheck = true;
            this.switch_LightOnOff.CheckedText = "光源打开";
            this.switch_LightOnOff.Location = new System.Drawing.Point(264, 3);
            this.switch_LightOnOff.Name = "switch_LightOnOff";
            this.switch_LightOnOff.Size = new System.Drawing.Size(75, 34);
            this.switch_LightOnOff.TabIndex = 8;
            this.switch_LightOnOff.UnCheckedText = "光源关闭";
            this.switch_LightOnOff.Click += new System.EventHandler(this.switch_LightOnOff_Click);
            // 
            // divider2
            // 
            this.divider2.ColorSplit = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
            this.divider2.Location = new System.Drawing.Point(238, 3);
            this.divider2.Name = "divider2";
            this.divider2.Size = new System.Drawing.Size(20, 34);
            this.divider2.TabIndex = 5;
            this.divider2.Text = "";
            this.divider2.Vertical = true;
            // 
            // btn_next
            // 
            this.btn_next.Image = global::Mef_VisionFrame.Properties.Resources.next;
            this.btn_next.Location = new System.Drawing.Point(198, 3);
            this.btn_next.Name = "btn_next";
            this.btn_next.Size = new System.Drawing.Size(34, 34);
            this.btn_next.TabIndex = 11;
            this.btn_next.UseVisualStyleBackColor = true;
            this.btn_next.Click += new System.EventHandler(this.btn_next_Click);
            // 
            // btn_run
            // 
            this.btn_run.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btn_run.CheckedBackColor = System.Drawing.Color.Empty;
            this.btn_run.CheckedImage = global::Mef_VisionFrame.Properties.Resources.pause;
            this.btn_run.Image = global::Mef_VisionFrame.Properties.Resources.run;
            this.btn_run.Location = new System.Drawing.Point(158, 3);
            this.btn_run.Name = "btn_run";
            this.btn_run.Size = new System.Drawing.Size(34, 34);
            this.btn_run.TabIndex = 2;
            this.btn_run.UnCheckedBackColor = System.Drawing.Color.Empty;
            this.btn_run.UnCheckedImage = global::Mef_VisionFrame.Properties.Resources.run;
            this.btn_run.UseVisualStyleBackColor = false;
            this.btn_run.Click += new System.EventHandler(this.btn_run_Click);
            // 
            // btn_Prev
            // 
            this.btn_Prev.Image = global::Mef_VisionFrame.Properties.Resources.prev;
            this.btn_Prev.Location = new System.Drawing.Point(118, 3);
            this.btn_Prev.Name = "btn_Prev";
            this.btn_Prev.Size = new System.Drawing.Size(34, 34);
            this.btn_Prev.TabIndex = 0;
            this.btn_Prev.UseVisualStyleBackColor = true;
            this.btn_Prev.Click += new System.EventHandler(this.btn_Prev_Click);
            // 
            // divider1
            // 
            this.divider1.ColorSplit = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
            this.divider1.Location = new System.Drawing.Point(92, 3);
            this.divider1.Name = "divider1";
            this.divider1.Size = new System.Drawing.Size(20, 34);
            this.divider1.TabIndex = 1;
            this.divider1.Text = "";
            this.divider1.Vertical = true;
            // 
            // btn_AutoSim
            // 
            this.btn_AutoSim.BackColor = System.Drawing.Color.Lime;
            this.btn_AutoSim.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btn_AutoSim.CheckedBackColor = System.Drawing.Color.DodgerBlue;
            this.btn_AutoSim.CheckedText = "仿真模式";
            this.btn_AutoSim.Location = new System.Drawing.Point(3, 3);
            this.btn_AutoSim.Name = "btn_AutoSim";
            this.btn_AutoSim.Size = new System.Drawing.Size(83, 34);
            this.btn_AutoSim.TabIndex = 0;
            this.btn_AutoSim.Text = "自动模式";
            this.btn_AutoSim.UnCheckedBackColor = System.Drawing.Color.Lime;
            this.btn_AutoSim.UnCheckedText = "自动模式";
            this.btn_AutoSim.UseVisualStyleBackColor = false;
            this.btn_AutoSim.Click += new System.EventHandler(this.btn_AutoSim_Click);
            // 
            // UC_ToolView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "UC_ToolView";
            this.Size = new System.Drawing.Size(1462, 763);
            this.Load += new System.EventHandler(this.UC_MainView_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.stackPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        public System.Windows.Forms.Panel panel_DataShow;
        private AntdUI.Input input_FilmRoll;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private AntdUI.StackPanel stackPanel1;
        private AntdUI.Divider divider2;
        private AntdUI.Divider divider1;
        private AntdUI.Label label1;
        private AntdUI.Switch switch_LightOnOff;
        private Frame_Core.ControlButton btn_AutoSim;
        private Frame_Core.ControlButton btn_FilmRollSwitch;
        private Frame_Core.ControlButton btn_run;
        private System.Windows.Forms.Button btn_Prev;
        private System.Windows.Forms.Button btn_next;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button btn_Reset;
        private System.Windows.Forms.Button btn_ClearCutData;
    }
}

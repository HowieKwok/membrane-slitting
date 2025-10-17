namespace Frame_Core
{
    partial class FormGrayScaleAdaptive
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormGrayScaleAdaptive));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btn_Save = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.Tv_Select = new System.Windows.Forms.TreeView();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel10 = new System.Windows.Forms.Panel();
            this.nud_LightSourcePort = new System.Windows.Forms.NumericUpDown();
            this.label15 = new System.Windows.Forms.Label();
            this.nud_LightSourceChannel = new System.Windows.Forms.NumericUpDown();
            this.label14 = new System.Windows.Forms.Label();
            this.panel9 = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.nud_TargetGray = new System.Windows.Forms.NumericUpDown();
            this.cb_Enable = new System.Windows.Forms.CheckBox();
            this.panel8 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.nud_GrayRangeMin = new System.Windows.Forms.NumericUpDown();
            this.nud_GrayRangeMax = new System.Windows.Forms.NumericUpDown();
            this.label13 = new System.Windows.Forms.Label();
            this.panel7 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.nud_ContinuousNFrameOutOfScopeGray = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.nud_AdjustmentGrayTolerance = new System.Windows.Forms.NumericUpDown();
            this.cb_ContinuousNFrameOutOfScopeGrayAlarm = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.nud_TargetGrayTolerance = new System.Windows.Forms.NumericUpDown();
            this.panel6 = new System.Windows.Forms.Panel();
            this.label8 = new System.Windows.Forms.Label();
            this.nud_SingleAdjustmentRatio = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.nud_AdjustFactor = new System.Windows.Forms.NumericUpDown();
            this.panel5 = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.nud_IntervalFrame = new System.Windows.Forms.NumericUpDown();
            this.label11 = new System.Windows.Forms.Label();
            this.cb_NNotAdjustFinishAlarm = new System.Windows.Forms.CheckBox();
            this.label12 = new System.Windows.Forms.Label();
            this.nud_AdjustFinishNum = new System.Windows.Forms.NumericUpDown();
            this.panel4 = new System.Windows.Forms.Panel();
            this.label10 = new System.Windows.Forms.Label();
            this.cb_GrayBodongAlarm = new System.Windows.Forms.CheckBox();
            this.nud_GrayBodong = new System.Windows.Forms.NumericUpDown();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel10.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_LightSourcePort)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_LightSourceChannel)).BeginInit();
            this.panel9.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_TargetGray)).BeginInit();
            this.panel8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_GrayRangeMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_GrayRangeMax)).BeginInit();
            this.panel7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_ContinuousNFrameOutOfScopeGray)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_AdjustmentGrayTolerance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_TargetGrayTolerance)).BeginInit();
            this.panel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_SingleAdjustmentRatio)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_AdjustFactor)).BeginInit();
            this.panel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_IntervalFrame)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_AdjustFinishNum)).BeginInit();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_GrayBodong)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel3, 1, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tableLayoutPanel1.SetColumnSpan(this.panel1, 3);
            this.panel1.Controls.Add(this.btn_Save);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // btn_Save
            // 
            resources.ApplyResources(this.btn_Save, "btn_Save");
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.UseVisualStyleBackColor = true;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.Tv_Select);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            this.tableLayoutPanel1.SetRowSpan(this.panel2, 2);
            // 
            // Tv_Select
            // 
            this.Tv_Select.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.Tv_Select, "Tv_Select");
            this.Tv_Select.Name = "Tv_Select";
            this.Tv_Select.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            ((System.Windows.Forms.TreeNode)(resources.GetObject("Tv_Select.Nodes"))),
            ((System.Windows.Forms.TreeNode)(resources.GetObject("Tv_Select.Nodes1"))),
            ((System.Windows.Forms.TreeNode)(resources.GetObject("Tv_Select.Nodes2"))),
            ((System.Windows.Forms.TreeNode)(resources.GetObject("Tv_Select.Nodes3")))});
            this.Tv_Select.DrawNode += new System.Windows.Forms.DrawTreeNodeEventHandler(this.Tv_Select_DrawNode);
            this.Tv_Select.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.Tv_Select_AfterSelect);
            // 
            // panel3
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.panel3, 2);
            this.panel3.Controls.Add(this.panel10);
            this.panel3.Controls.Add(this.panel9);
            this.panel3.Controls.Add(this.panel8);
            this.panel3.Controls.Add(this.panel7);
            this.panel3.Controls.Add(this.panel6);
            this.panel3.Controls.Add(this.panel5);
            this.panel3.Controls.Add(this.panel4);
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.Name = "panel3";
            this.tableLayoutPanel1.SetRowSpan(this.panel3, 2);
            // 
            // panel10
            // 
            this.panel10.Controls.Add(this.nud_LightSourcePort);
            this.panel10.Controls.Add(this.label15);
            this.panel10.Controls.Add(this.nud_LightSourceChannel);
            this.panel10.Controls.Add(this.label14);
            resources.ApplyResources(this.panel10, "panel10");
            this.panel10.Name = "panel10";
            // 
            // nud_LightSourcePort
            // 
            resources.ApplyResources(this.nud_LightSourcePort, "nud_LightSourcePort");
            this.nud_LightSourcePort.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nud_LightSourcePort.Name = "nud_LightSourcePort";
            // 
            // label15
            // 
            resources.ApplyResources(this.label15, "label15");
            this.label15.Name = "label15";
            // 
            // nud_LightSourceChannel
            // 
            resources.ApplyResources(this.nud_LightSourceChannel, "nud_LightSourceChannel");
            this.nud_LightSourceChannel.Maximum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nud_LightSourceChannel.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nud_LightSourceChannel.Name = "nud_LightSourceChannel";
            this.nud_LightSourceChannel.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label14
            // 
            resources.ApplyResources(this.label14, "label14");
            this.label14.Name = "label14";
            // 
            // panel9
            // 
            this.panel9.Controls.Add(this.label5);
            this.panel9.Controls.Add(this.nud_TargetGray);
            this.panel9.Controls.Add(this.cb_Enable);
            resources.ApplyResources(this.panel9, "panel9");
            this.panel9.Name = "panel9";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // nud_TargetGray
            // 
            resources.ApplyResources(this.nud_TargetGray, "nud_TargetGray");
            this.nud_TargetGray.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nud_TargetGray.Name = "nud_TargetGray";
            // 
            // cb_Enable
            // 
            resources.ApplyResources(this.cb_Enable, "cb_Enable");
            this.cb_Enable.Name = "cb_Enable";
            this.cb_Enable.UseVisualStyleBackColor = true;
            // 
            // panel8
            // 
            this.panel8.Controls.Add(this.label1);
            this.panel8.Controls.Add(this.nud_GrayRangeMin);
            this.panel8.Controls.Add(this.nud_GrayRangeMax);
            this.panel8.Controls.Add(this.label13);
            resources.ApplyResources(this.panel8, "panel8");
            this.panel8.Name = "panel8";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // nud_GrayRangeMin
            // 
            resources.ApplyResources(this.nud_GrayRangeMin, "nud_GrayRangeMin");
            this.nud_GrayRangeMin.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nud_GrayRangeMin.Name = "nud_GrayRangeMin";
            // 
            // nud_GrayRangeMax
            // 
            resources.ApplyResources(this.nud_GrayRangeMax, "nud_GrayRangeMax");
            this.nud_GrayRangeMax.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nud_GrayRangeMax.Name = "nud_GrayRangeMax";
            // 
            // label13
            // 
            resources.ApplyResources(this.label13, "label13");
            this.label13.Name = "label13";
            // 
            // panel7
            // 
            this.panel7.Controls.Add(this.label2);
            this.panel7.Controls.Add(this.nud_ContinuousNFrameOutOfScopeGray);
            this.panel7.Controls.Add(this.label3);
            this.panel7.Controls.Add(this.label4);
            this.panel7.Controls.Add(this.nud_AdjustmentGrayTolerance);
            this.panel7.Controls.Add(this.cb_ContinuousNFrameOutOfScopeGrayAlarm);
            this.panel7.Controls.Add(this.label6);
            this.panel7.Controls.Add(this.nud_TargetGrayTolerance);
            resources.ApplyResources(this.panel7, "panel7");
            this.panel7.Name = "panel7";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // nud_ContinuousNFrameOutOfScopeGray
            // 
            resources.ApplyResources(this.nud_ContinuousNFrameOutOfScopeGray, "nud_ContinuousNFrameOutOfScopeGray");
            this.nud_ContinuousNFrameOutOfScopeGray.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nud_ContinuousNFrameOutOfScopeGray.Name = "nud_ContinuousNFrameOutOfScopeGray";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // nud_AdjustmentGrayTolerance
            // 
            resources.ApplyResources(this.nud_AdjustmentGrayTolerance, "nud_AdjustmentGrayTolerance");
            this.nud_AdjustmentGrayTolerance.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nud_AdjustmentGrayTolerance.Name = "nud_AdjustmentGrayTolerance";
            // 
            // cb_ContinuousNFrameOutOfScopeGrayAlarm
            // 
            resources.ApplyResources(this.cb_ContinuousNFrameOutOfScopeGrayAlarm, "cb_ContinuousNFrameOutOfScopeGrayAlarm");
            this.cb_ContinuousNFrameOutOfScopeGrayAlarm.Name = "cb_ContinuousNFrameOutOfScopeGrayAlarm";
            this.cb_ContinuousNFrameOutOfScopeGrayAlarm.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // nud_TargetGrayTolerance
            // 
            resources.ApplyResources(this.nud_TargetGrayTolerance, "nud_TargetGrayTolerance");
            this.nud_TargetGrayTolerance.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nud_TargetGrayTolerance.Name = "nud_TargetGrayTolerance";
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.label8);
            this.panel6.Controls.Add(this.nud_SingleAdjustmentRatio);
            this.panel6.Controls.Add(this.label9);
            this.panel6.Controls.Add(this.nud_AdjustFactor);
            resources.ApplyResources(this.panel6, "panel6");
            this.panel6.Name = "panel6";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // nud_SingleAdjustmentRatio
            // 
            this.nud_SingleAdjustmentRatio.DecimalPlaces = 2;
            this.nud_SingleAdjustmentRatio.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            resources.ApplyResources(this.nud_SingleAdjustmentRatio, "nud_SingleAdjustmentRatio");
            this.nud_SingleAdjustmentRatio.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nud_SingleAdjustmentRatio.Name = "nud_SingleAdjustmentRatio";
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // nud_AdjustFactor
            // 
            this.nud_AdjustFactor.DecimalPlaces = 5;
            resources.ApplyResources(this.nud_AdjustFactor, "nud_AdjustFactor");
            this.nud_AdjustFactor.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nud_AdjustFactor.Name = "nud_AdjustFactor";
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.label7);
            this.panel5.Controls.Add(this.nud_IntervalFrame);
            this.panel5.Controls.Add(this.label11);
            this.panel5.Controls.Add(this.cb_NNotAdjustFinishAlarm);
            this.panel5.Controls.Add(this.label12);
            this.panel5.Controls.Add(this.nud_AdjustFinishNum);
            resources.ApplyResources(this.panel5, "panel5");
            this.panel5.Name = "panel5";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // nud_IntervalFrame
            // 
            resources.ApplyResources(this.nud_IntervalFrame, "nud_IntervalFrame");
            this.nud_IntervalFrame.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nud_IntervalFrame.Name = "nud_IntervalFrame";
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            // 
            // cb_NNotAdjustFinishAlarm
            // 
            resources.ApplyResources(this.cb_NNotAdjustFinishAlarm, "cb_NNotAdjustFinishAlarm");
            this.cb_NNotAdjustFinishAlarm.Name = "cb_NNotAdjustFinishAlarm";
            this.cb_NNotAdjustFinishAlarm.UseVisualStyleBackColor = true;
            // 
            // label12
            // 
            resources.ApplyResources(this.label12, "label12");
            this.label12.Name = "label12";
            // 
            // nud_AdjustFinishNum
            // 
            resources.ApplyResources(this.nud_AdjustFinishNum, "nud_AdjustFinishNum");
            this.nud_AdjustFinishNum.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nud_AdjustFinishNum.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.nud_AdjustFinishNum.Name = "nud_AdjustFinishNum";
            this.nud_AdjustFinishNum.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.label10);
            this.panel4.Controls.Add(this.cb_GrayBodongAlarm);
            this.panel4.Controls.Add(this.nud_GrayBodong);
            resources.ApplyResources(this.panel4, "panel4");
            this.panel4.Name = "panel4";
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // cb_GrayBodongAlarm
            // 
            resources.ApplyResources(this.cb_GrayBodongAlarm, "cb_GrayBodongAlarm");
            this.cb_GrayBodongAlarm.Name = "cb_GrayBodongAlarm";
            this.cb_GrayBodongAlarm.UseVisualStyleBackColor = true;
            // 
            // nud_GrayBodong
            // 
            resources.ApplyResources(this.nud_GrayBodong, "nud_GrayBodong");
            this.nud_GrayBodong.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nud_GrayBodong.Name = "nud_GrayBodong";
            // 
            // FormGrayScaleAdaptive
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "FormGrayScaleAdaptive";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormGrayScaleAdaptive_FormClosing);
            this.Load += new System.EventHandler(this.FormGrayScaleAdaptive_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel10.ResumeLayout(false);
            this.panel10.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_LightSourcePort)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_LightSourceChannel)).EndInit();
            this.panel9.ResumeLayout(false);
            this.panel9.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_TargetGray)).EndInit();
            this.panel8.ResumeLayout(false);
            this.panel8.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_GrayRangeMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_GrayRangeMax)).EndInit();
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_ContinuousNFrameOutOfScopeGray)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_AdjustmentGrayTolerance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_TargetGrayTolerance)).EndInit();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_SingleAdjustmentRatio)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_AdjustFactor)).EndInit();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_IntervalFrame)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_AdjustFinishNum)).EndInit();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_GrayBodong)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btn_Save;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TreeView Tv_Select;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.CheckBox cb_Enable;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown nud_ContinuousNFrameOutOfScopeGray;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nud_GrayRangeMax;
        private System.Windows.Forms.NumericUpDown nud_GrayRangeMin;
        private System.Windows.Forms.NumericUpDown nud_TargetGrayTolerance;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown nud_TargetGray;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown nud_AdjustmentGrayTolerance;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown nud_IntervalFrame;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown nud_SingleAdjustmentRatio;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox cb_ContinuousNFrameOutOfScopeGrayAlarm;
        private System.Windows.Forms.NumericUpDown nud_GrayBodong;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.CheckBox cb_GrayBodongAlarm;
        private System.Windows.Forms.NumericUpDown nud_AdjustFactor;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown nud_AdjustFinishNum;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.CheckBox cb_NNotAdjustFinishAlarm;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.Panel panel9;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.NumericUpDown nud_LightSourceChannel;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.NumericUpDown nud_LightSourcePort;
        private System.Windows.Forms.Panel panel10;
    }
}
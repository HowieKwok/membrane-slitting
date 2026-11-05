namespace Mef_VisionFrame
{
    partial class Form_DefectRules
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
            this.label1 = new AntdUI.Label();
            this.input_detectionItem = new AntdUI.Input();
            this.label3 = new AntdUI.Label();
            this.Condition1_En = new AntdUI.Checkbox();
            this.Condition1_MembraneNum = new AntdUI.InputNumber();
            this.label4 = new AntdUI.Label();
            this.Condition2_Data1 = new AntdUI.InputNumber();
            this.Condition2_En = new AntdUI.Checkbox();
            this.label6 = new AntdUI.Label();
            this.Condition1AndCondition2_Logic = new AntdUI.Select();
            this.Condition2_DataSource1 = new AntdUI.Select();
            this.label5 = new AntdUI.Label();
            this.Condition2_Logic1 = new AntdUI.Select();
            this.label7 = new AntdUI.Label();
            this.Condition2_DataSource2 = new AntdUI.Select();
            this.Condition2_Data2 = new AntdUI.InputNumber();
            this.label8 = new AntdUI.Label();
            this.Condition2_DataSource3 = new AntdUI.Select();
            this.Condition2_Data3 = new AntdUI.InputNumber();
            this.Condition2_Logic2 = new AntdUI.Select();
            this.Condition2AndCondition3_Logic = new AntdUI.Select();
            this.Condition3_En = new AntdUI.Checkbox();
            this.label12 = new AntdUI.Label();
            this.label10 = new AntdUI.Label();
            this.Condition3_DataSource2 = new AntdUI.Select();
            this.Condition3_Data2 = new AntdUI.InputNumber();
            this.Condition3_Logic1 = new AntdUI.Select();
            this.label11 = new AntdUI.Label();
            this.Condition3_DataSource1 = new AntdUI.Select();
            this.Condition3_Data1 = new AntdUI.InputNumber();
            this.label13 = new AntdUI.Label();
            this.label15 = new AntdUI.Label();
            this.Condition4_DataSource2 = new AntdUI.Select();
            this.Condition4_Data2 = new AntdUI.InputNumber();
            this.Condition4_Logic1 = new AntdUI.Select();
            this.label16 = new AntdUI.Label();
            this.Condition4_DataSource1 = new AntdUI.Select();
            this.Condition4_Data1 = new AntdUI.InputNumber();
            this.Condition3AndCondition4_Logic = new AntdUI.Select();
            this.Condition4_En = new AntdUI.Checkbox();
            this.label17 = new AntdUI.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.select_detectionItem = new AntdUI.Select();
            this.input_ConditionalFormat = new AntdUI.Input();
            this.label9 = new AntdUI.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.cbx_HaveOrNot = new AntdUI.Checkbox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(45, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "检测项";
            // 
            // input_detectionItem
            // 
            this.input_detectionItem.Location = new System.Drawing.Point(113, 36);
            this.input_detectionItem.Name = "input_detectionItem";
            this.input_detectionItem.Size = new System.Drawing.Size(146, 23);
            this.input_detectionItem.TabIndex = 2;
            this.input_detectionItem.Text = "input1";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(45, 127);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 23);
            this.label3.TabIndex = 4;
            this.label3.Text = "条件1";
            // 
            // Condition1_En
            // 
            this.Condition1_En.AutoCheck = true;
            this.Condition1_En.Location = new System.Drawing.Point(113, 127);
            this.Condition1_En.Name = "Condition1_En";
            this.Condition1_En.Size = new System.Drawing.Size(78, 23);
            this.Condition1_En.TabIndex = 5;
            this.Condition1_En.Text = "使能";
            // 
            // Condition1_MembraneNum
            // 
            this.Condition1_MembraneNum.Location = new System.Drawing.Point(332, 127);
            this.Condition1_MembraneNum.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.Condition1_MembraneNum.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.Condition1_MembraneNum.Name = "Condition1_MembraneNum";
            this.Condition1_MembraneNum.Size = new System.Drawing.Size(102, 23);
            this.Condition1_MembraneNum.TabIndex = 6;
            this.Condition1_MembraneNum.Text = "0";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(221, 127);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(75, 23);
            this.label4.TabIndex = 7;
            this.label4.Text = "缺陷数量";
            // 
            // Condition2_Data1
            // 
            this.Condition2_Data1.DecimalPlaces = 2;
            this.Condition2_Data1.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.Condition2_Data1.Location = new System.Drawing.Point(332, 222);
            this.Condition2_Data1.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.Condition2_Data1.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.Condition2_Data1.Name = "Condition2_Data1";
            this.Condition2_Data1.Size = new System.Drawing.Size(102, 23);
            this.Condition2_Data1.TabIndex = 10;
            this.Condition2_Data1.Text = "0.00";
            // 
            // Condition2_En
            // 
            this.Condition2_En.AutoCheck = true;
            this.Condition2_En.Location = new System.Drawing.Point(113, 222);
            this.Condition2_En.Name = "Condition2_En";
            this.Condition2_En.Size = new System.Drawing.Size(78, 23);
            this.Condition2_En.TabIndex = 9;
            this.Condition2_En.Text = "使能";
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(45, 222);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(75, 23);
            this.label6.TabIndex = 8;
            this.label6.Text = "条件2";
            // 
            // Condition1AndCondition2_Logic
            // 
            this.Condition1AndCondition2_Logic.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(0)))), ((int)(((byte)(127)))), ((int)(((byte)(255)))));
            this.Condition1AndCondition2_Logic.List = true;
            this.Condition1AndCondition2_Logic.Location = new System.Drawing.Point(45, 175);
            this.Condition1AndCondition2_Logic.Name = "Condition1AndCondition2_Logic";
            this.Condition1AndCondition2_Logic.Size = new System.Drawing.Size(75, 23);
            this.Condition1AndCondition2_Logic.TabIndex = 12;
            this.Condition1AndCondition2_Logic.Text = "且/或";
            // 
            // Condition2_DataSource1
            // 
            this.Condition2_DataSource1.List = true;
            this.Condition2_DataSource1.Location = new System.Drawing.Point(209, 222);
            this.Condition2_DataSource1.Name = "Condition2_DataSource1";
            this.Condition2_DataSource1.Size = new System.Drawing.Size(75, 23);
            this.Condition2_DataSource1.TabIndex = 13;
            this.Condition2_DataSource1.Text = "数据源";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(304, 222);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(21, 23);
            this.label5.TabIndex = 14;
            this.label5.Text = ">";
            // 
            // Condition2_Logic1
            // 
            this.Condition2_Logic1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(0)))), ((int)(((byte)(127)))), ((int)(((byte)(255)))));
            this.Condition2_Logic1.List = true;
            this.Condition2_Logic1.Location = new System.Drawing.Point(457, 222);
            this.Condition2_Logic1.Name = "Condition2_Logic1";
            this.Condition2_Logic1.Size = new System.Drawing.Size(75, 23);
            this.Condition2_Logic1.TabIndex = 15;
            this.Condition2_Logic1.Text = "且/或";
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(655, 222);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(21, 23);
            this.label7.TabIndex = 18;
            this.label7.Text = ">";
            // 
            // Condition2_DataSource2
            // 
            this.Condition2_DataSource2.List = true;
            this.Condition2_DataSource2.Location = new System.Drawing.Point(560, 222);
            this.Condition2_DataSource2.Name = "Condition2_DataSource2";
            this.Condition2_DataSource2.Size = new System.Drawing.Size(75, 23);
            this.Condition2_DataSource2.TabIndex = 17;
            this.Condition2_DataSource2.Text = "数据源";
            // 
            // Condition2_Data2
            // 
            this.Condition2_Data2.DecimalPlaces = 2;
            this.Condition2_Data2.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.Condition2_Data2.Location = new System.Drawing.Point(683, 222);
            this.Condition2_Data2.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.Condition2_Data2.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.Condition2_Data2.Name = "Condition2_Data2";
            this.Condition2_Data2.Size = new System.Drawing.Size(102, 23);
            this.Condition2_Data2.TabIndex = 16;
            this.Condition2_Data2.Text = "0.00";
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(992, 222);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(21, 23);
            this.label8.TabIndex = 22;
            this.label8.Text = ">";
            // 
            // Condition2_DataSource3
            // 
            this.Condition2_DataSource3.Location = new System.Drawing.Point(897, 222);
            this.Condition2_DataSource3.Name = "Condition2_DataSource3";
            this.Condition2_DataSource3.Size = new System.Drawing.Size(75, 23);
            this.Condition2_DataSource3.TabIndex = 21;
            this.Condition2_DataSource3.Text = "数据源";
            // 
            // Condition2_Data3
            // 
            this.Condition2_Data3.DecimalPlaces = 2;
            this.Condition2_Data3.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.Condition2_Data3.Location = new System.Drawing.Point(1020, 222);
            this.Condition2_Data3.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.Condition2_Data3.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.Condition2_Data3.Name = "Condition2_Data3";
            this.Condition2_Data3.Size = new System.Drawing.Size(102, 23);
            this.Condition2_Data3.TabIndex = 20;
            this.Condition2_Data3.Text = "0.00";
            // 
            // Condition2_Logic2
            // 
            this.Condition2_Logic2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(0)))), ((int)(((byte)(127)))), ((int)(((byte)(255)))));
            this.Condition2_Logic2.List = true;
            this.Condition2_Logic2.Location = new System.Drawing.Point(801, 222);
            this.Condition2_Logic2.Name = "Condition2_Logic2";
            this.Condition2_Logic2.Size = new System.Drawing.Size(75, 23);
            this.Condition2_Logic2.TabIndex = 19;
            this.Condition2_Logic2.Text = "且/或";
            // 
            // Condition2AndCondition3_Logic
            // 
            this.Condition2AndCondition3_Logic.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(0)))), ((int)(((byte)(127)))), ((int)(((byte)(255)))));
            this.Condition2AndCondition3_Logic.List = true;
            this.Condition2AndCondition3_Logic.Location = new System.Drawing.Point(45, 288);
            this.Condition2AndCondition3_Logic.Name = "Condition2AndCondition3_Logic";
            this.Condition2AndCondition3_Logic.Size = new System.Drawing.Size(75, 23);
            this.Condition2AndCondition3_Logic.TabIndex = 26;
            this.Condition2AndCondition3_Logic.Text = "且/或";
            // 
            // Condition3_En
            // 
            this.Condition3_En.AutoCheck = true;
            this.Condition3_En.Location = new System.Drawing.Point(113, 335);
            this.Condition3_En.Name = "Condition3_En";
            this.Condition3_En.Size = new System.Drawing.Size(78, 23);
            this.Condition3_En.TabIndex = 24;
            this.Condition3_En.Text = "使能";
            // 
            // label12
            // 
            this.label12.Location = new System.Drawing.Point(45, 335);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(75, 23);
            this.label12.TabIndex = 23;
            this.label12.Text = "条件3";
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(655, 335);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(21, 23);
            this.label10.TabIndex = 33;
            this.label10.Text = ">";
            // 
            // Condition3_DataSource2
            // 
            this.Condition3_DataSource2.List = true;
            this.Condition3_DataSource2.Location = new System.Drawing.Point(560, 335);
            this.Condition3_DataSource2.Name = "Condition3_DataSource2";
            this.Condition3_DataSource2.Size = new System.Drawing.Size(75, 23);
            this.Condition3_DataSource2.TabIndex = 32;
            this.Condition3_DataSource2.Text = "数据源";
            // 
            // Condition3_Data2
            // 
            this.Condition3_Data2.DecimalPlaces = 2;
            this.Condition3_Data2.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.Condition3_Data2.Location = new System.Drawing.Point(683, 335);
            this.Condition3_Data2.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.Condition3_Data2.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.Condition3_Data2.Name = "Condition3_Data2";
            this.Condition3_Data2.Size = new System.Drawing.Size(102, 23);
            this.Condition3_Data2.TabIndex = 31;
            this.Condition3_Data2.Text = "0.00";
            // 
            // Condition3_Logic1
            // 
            this.Condition3_Logic1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(0)))), ((int)(((byte)(127)))), ((int)(((byte)(255)))));
            this.Condition3_Logic1.List = true;
            this.Condition3_Logic1.Location = new System.Drawing.Point(457, 335);
            this.Condition3_Logic1.Name = "Condition3_Logic1";
            this.Condition3_Logic1.Size = new System.Drawing.Size(75, 23);
            this.Condition3_Logic1.TabIndex = 30;
            this.Condition3_Logic1.Text = "且/或";
            // 
            // label11
            // 
            this.label11.Location = new System.Drawing.Point(304, 335);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(21, 23);
            this.label11.TabIndex = 29;
            this.label11.Text = ">";
            // 
            // Condition3_DataSource1
            // 
            this.Condition3_DataSource1.List = true;
            this.Condition3_DataSource1.Location = new System.Drawing.Point(209, 335);
            this.Condition3_DataSource1.Name = "Condition3_DataSource1";
            this.Condition3_DataSource1.Size = new System.Drawing.Size(75, 23);
            this.Condition3_DataSource1.TabIndex = 28;
            this.Condition3_DataSource1.Text = "数据源";
            // 
            // Condition3_Data1
            // 
            this.Condition3_Data1.DecimalPlaces = 2;
            this.Condition3_Data1.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.Condition3_Data1.Location = new System.Drawing.Point(332, 335);
            this.Condition3_Data1.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.Condition3_Data1.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.Condition3_Data1.Name = "Condition3_Data1";
            this.Condition3_Data1.Size = new System.Drawing.Size(102, 23);
            this.Condition3_Data1.TabIndex = 27;
            this.Condition3_Data1.Text = "0.00";
            // 
            // label13
            // 
            this.label13.Location = new System.Drawing.Point(305, 127);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(21, 23);
            this.label13.TabIndex = 38;
            this.label13.Text = ">";
            // 
            // label15
            // 
            this.label15.Location = new System.Drawing.Point(655, 437);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(21, 23);
            this.label15.TabIndex = 48;
            this.label15.Text = ">";
            // 
            // Condition4_DataSource2
            // 
            this.Condition4_DataSource2.List = true;
            this.Condition4_DataSource2.Location = new System.Drawing.Point(560, 437);
            this.Condition4_DataSource2.Name = "Condition4_DataSource2";
            this.Condition4_DataSource2.Size = new System.Drawing.Size(75, 23);
            this.Condition4_DataSource2.TabIndex = 47;
            this.Condition4_DataSource2.Text = "数据源";
            // 
            // Condition4_Data2
            // 
            this.Condition4_Data2.DecimalPlaces = 2;
            this.Condition4_Data2.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.Condition4_Data2.Location = new System.Drawing.Point(683, 437);
            this.Condition4_Data2.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.Condition4_Data2.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.Condition4_Data2.Name = "Condition4_Data2";
            this.Condition4_Data2.Size = new System.Drawing.Size(102, 23);
            this.Condition4_Data2.TabIndex = 46;
            this.Condition4_Data2.Text = "0.00";
            // 
            // Condition4_Logic1
            // 
            this.Condition4_Logic1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(0)))), ((int)(((byte)(127)))), ((int)(((byte)(255)))));
            this.Condition4_Logic1.List = true;
            this.Condition4_Logic1.Location = new System.Drawing.Point(457, 437);
            this.Condition4_Logic1.Name = "Condition4_Logic1";
            this.Condition4_Logic1.Size = new System.Drawing.Size(75, 23);
            this.Condition4_Logic1.TabIndex = 45;
            this.Condition4_Logic1.Text = "且/或";
            // 
            // label16
            // 
            this.label16.Location = new System.Drawing.Point(304, 437);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(21, 23);
            this.label16.TabIndex = 44;
            this.label16.Text = ">";
            // 
            // Condition4_DataSource1
            // 
            this.Condition4_DataSource1.List = true;
            this.Condition4_DataSource1.Location = new System.Drawing.Point(209, 437);
            this.Condition4_DataSource1.Name = "Condition4_DataSource1";
            this.Condition4_DataSource1.Size = new System.Drawing.Size(75, 23);
            this.Condition4_DataSource1.TabIndex = 43;
            this.Condition4_DataSource1.Text = "数据源";
            // 
            // Condition4_Data1
            // 
            this.Condition4_Data1.DecimalPlaces = 2;
            this.Condition4_Data1.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.Condition4_Data1.Location = new System.Drawing.Point(332, 437);
            this.Condition4_Data1.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.Condition4_Data1.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.Condition4_Data1.Name = "Condition4_Data1";
            this.Condition4_Data1.Size = new System.Drawing.Size(102, 23);
            this.Condition4_Data1.TabIndex = 42;
            this.Condition4_Data1.Text = "0.00";
            // 
            // Condition3AndCondition4_Logic
            // 
            this.Condition3AndCondition4_Logic.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(0)))), ((int)(((byte)(127)))), ((int)(((byte)(255)))));
            this.Condition3AndCondition4_Logic.List = true;
            this.Condition3AndCondition4_Logic.Location = new System.Drawing.Point(45, 390);
            this.Condition3AndCondition4_Logic.Name = "Condition3AndCondition4_Logic";
            this.Condition3AndCondition4_Logic.Size = new System.Drawing.Size(75, 23);
            this.Condition3AndCondition4_Logic.TabIndex = 41;
            this.Condition3AndCondition4_Logic.Text = "且/或";
            // 
            // Condition4_En
            // 
            this.Condition4_En.AutoCheck = true;
            this.Condition4_En.Location = new System.Drawing.Point(113, 437);
            this.Condition4_En.Name = "Condition4_En";
            this.Condition4_En.Size = new System.Drawing.Size(78, 23);
            this.Condition4_En.TabIndex = 40;
            this.Condition4_En.Text = "使能";
            // 
            // label17
            // 
            this.label17.Location = new System.Drawing.Point(45, 437);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(75, 23);
            this.label17.TabIndex = 39;
            this.label17.Text = "条件4";
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(912, 563);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 55;
            this.btnOk.Text = "确认";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(1047, 563);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 56;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // select_detectionItem
            // 
            this.select_detectionItem.List = true;
            this.select_detectionItem.Location = new System.Drawing.Point(279, 36);
            this.select_detectionItem.MaxCount = 100;
            this.select_detectionItem.Name = "select_detectionItem";
            this.select_detectionItem.Size = new System.Drawing.Size(155, 23);
            this.select_detectionItem.TabIndex = 57;
            this.select_detectionItem.Text = "检测项";
            this.select_detectionItem.SelectedIndexChanged += new AntdUI.IntEventHandler(this.detectionItem_SelectedIndexChanged);
            // 
            // input_ConditionalFormat
            // 
            this.input_ConditionalFormat.Enabled = false;
            this.input_ConditionalFormat.Location = new System.Drawing.Point(156, 80);
            this.input_ConditionalFormat.Name = "input_ConditionalFormat";
            this.input_ConditionalFormat.Size = new System.Drawing.Size(953, 23);
            this.input_ConditionalFormat.TabIndex = 59;
            this.input_ConditionalFormat.Text = "input1";
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(45, 80);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(105, 23);
            this.label9.TabIndex = 58;
            this.label9.Text = "当前缺陷规则";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // cbx_HaveOrNot
            // 
            this.cbx_HaveOrNot.AutoCheck = true;
            this.cbx_HaveOrNot.Location = new System.Drawing.Point(470, 36);
            this.cbx_HaveOrNot.Name = "cbx_HaveOrNot";
            this.cbx_HaveOrNot.Size = new System.Drawing.Size(78, 23);
            this.cbx_HaveOrNot.TabIndex = 60;
            this.cbx_HaveOrNot.Text = "有无检测";
            // 
            // Form_DefectRules
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(242)))), ((int)(((byte)(245)))));
            this.ClientSize = new System.Drawing.Size(1143, 620);
            this.Controls.Add(this.cbx_HaveOrNot);
            this.Controls.Add(this.input_ConditionalFormat);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.select_detectionItem);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.Condition4_DataSource2);
            this.Controls.Add(this.Condition4_Data2);
            this.Controls.Add(this.Condition4_Logic1);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.Condition4_DataSource1);
            this.Controls.Add(this.Condition4_Data1);
            this.Controls.Add(this.Condition3AndCondition4_Logic);
            this.Controls.Add(this.Condition4_En);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.Condition3_DataSource2);
            this.Controls.Add(this.Condition3_Data2);
            this.Controls.Add(this.Condition3_Logic1);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.Condition3_DataSource1);
            this.Controls.Add(this.Condition3_Data1);
            this.Controls.Add(this.Condition2AndCondition3_Logic);
            this.Controls.Add(this.Condition3_En);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.Condition2_DataSource3);
            this.Controls.Add(this.Condition2_Data3);
            this.Controls.Add(this.Condition2_Logic2);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.Condition2_DataSource2);
            this.Controls.Add(this.Condition2_Data2);
            this.Controls.Add(this.Condition2_Logic1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.Condition2_DataSource1);
            this.Controls.Add(this.Condition1AndCondition2_Logic);
            this.Controls.Add(this.Condition2_Data1);
            this.Controls.Add(this.Condition2_En);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.Condition1_MembraneNum);
            this.Controls.Add(this.Condition1_En);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.input_detectionItem);
            this.Controls.Add(this.label1);
            this.Name = "Form_DefectRules";
            this.Text = "Form_DefectRules";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_DefectRules_FormClosing);
            this.Load += new System.EventHandler(this.Form_DefectRules_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private AntdUI.Label label1;
        private AntdUI.Input input_detectionItem;
        private AntdUI.Label label3;
        private AntdUI.Checkbox Condition1_En;
        private AntdUI.InputNumber Condition1_MembraneNum;
        private AntdUI.Label label4;
        private AntdUI.InputNumber Condition2_Data1;
        private AntdUI.Checkbox Condition2_En;
        private AntdUI.Label label6;
        private AntdUI.Select Condition1AndCondition2_Logic;
        private AntdUI.Select Condition2_DataSource1;
        private AntdUI.Label label5;
        private AntdUI.Select Condition2_Logic1;
        private AntdUI.Label label7;
        private AntdUI.Select Condition2_DataSource2;
        private AntdUI.InputNumber Condition2_Data2;
        private AntdUI.Label label8;
        private AntdUI.Select Condition2_DataSource3;
        private AntdUI.InputNumber Condition2_Data3;
        private AntdUI.Select Condition2_Logic2;
        private AntdUI.Select Condition2AndCondition3_Logic;
        private AntdUI.Checkbox Condition3_En;
        private AntdUI.Label label12;
        private AntdUI.Label label10;
        private AntdUI.Select Condition3_DataSource2;
        private AntdUI.InputNumber Condition3_Data2;
        private AntdUI.Select Condition3_Logic1;
        private AntdUI.Label label11;
        private AntdUI.Select Condition3_DataSource1;
        private AntdUI.InputNumber Condition3_Data1;
        private AntdUI.Label label13;
        private AntdUI.Label label15;
        private AntdUI.Select Condition4_DataSource2;
        private AntdUI.InputNumber Condition4_Data2;
        private AntdUI.Select Condition4_Logic1;
        private AntdUI.Label label16;
        private AntdUI.Select Condition4_DataSource1;
        private AntdUI.InputNumber Condition4_Data1;
        private AntdUI.Select Condition3AndCondition4_Logic;
        private AntdUI.Checkbox Condition4_En;
        private AntdUI.Label label17;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private AntdUI.Select select_detectionItem;
        private AntdUI.Input input_ConditionalFormat;
        private AntdUI.Label label9;
        private System.Windows.Forms.Timer timer1;
        private AntdUI.Checkbox cbx_HaveOrNot;
    }
}
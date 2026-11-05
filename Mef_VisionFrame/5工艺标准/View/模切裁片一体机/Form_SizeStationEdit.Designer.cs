namespace Mef_VisionFrame
{
    partial class Form_SizeStationEdit
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_SizeStationEdit));
            this.select_detectionItem = new AntdUI.Select();
            this.MeasureOffset = new AntdUI.InputNumber();
            this.CalibrateCoeff = new AntdUI.InputNumber();
            this.btn_AutoCalib = new AntdUI.Button();
            this.CCDMeasureValue = new AntdUI.InputNumber();
            this.ArtificialMeasureValue = new AntdUI.InputNumber();
            this.label2 = new AntdUI.Label();
            this.EnableDataModel = new AntdUI.Checkbox();
            this.panel4 = new AntdUI.Panel();
            this.btn_FixReset = new AntdUI.Button();
            this.label10 = new AntdUI.Label();
            this.label9 = new AntdUI.Label();
            this.label8 = new AntdUI.Label();
            this.label3 = new AntdUI.Label();
            this.panel3 = new AntdUI.Panel();
            this.divider2 = new AntdUI.Divider();
            this.panel1 = new AntdUI.Panel();
            this.btn_SubscriptionData = new AntdUI.Button();
            this.label1 = new AntdUI.Label();
            this.panel2 = new AntdUI.Panel();
            this.divider1 = new AntdUI.Divider();
            this.label4 = new AntdUI.Label();
            this.CompensateO = new AntdUI.InputNumber();
            this.CompensateF = new AntdUI.InputNumber();
            this.stackPanel1 = new AntdUI.StackPanel();
            this.panel5 = new AntdUI.Panel();
            this.btn_Save = new AntdUI.Button();
            this.panel6 = new AntdUI.Panel();
            this.label11 = new AntdUI.Label();
            this.select_Cam2 = new AntdUI.Select();
            this.label7 = new AntdUI.Label();
            this.select_Cam1 = new AntdUI.Select();
            this.label6 = new AntdUI.Label();
            this.input_DataSource2 = new AntdUI.Input();
            this.label5 = new AntdUI.Label();
            this.input_DataSource1 = new AntdUI.Input();
            this.panel7 = new AntdUI.Panel();
            this.divider3 = new AntdUI.Divider();
            this.panel8 = new AntdUI.Panel();
            this.panel9 = new AntdUI.Panel();
            this.divider4 = new AntdUI.Divider();
            this.stackPanel2 = new AntdUI.StackPanel();
            this.panel10 = new AntdUI.Panel();
            this.tree1 = new AntdUI.Tree();
            this.label12 = new AntdUI.Label();
            this.select_CalcFormula = new AntdUI.Select();
            this.panel4.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.stackPanel1.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel7.SuspendLayout();
            this.panel8.SuspendLayout();
            this.panel9.SuspendLayout();
            this.stackPanel2.SuspendLayout();
            this.panel10.SuspendLayout();
            this.SuspendLayout();
            // 
            // select_detectionItem
            // 
            this.select_detectionItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(84)))), ((int)(((byte)(79)))), ((int)(((byte)(79)))));
            this.select_detectionItem.List = true;
            resources.ApplyResources(this.select_detectionItem, "select_detectionItem");
            this.select_detectionItem.MaxCount = 100;
            this.select_detectionItem.Name = "select_detectionItem";
            this.select_detectionItem.SelectedIndexChanged += new AntdUI.IntEventHandler(this.select_detectionItem_SelectedIndexChanged);
            // 
            // MeasureOffset
            // 
            this.MeasureOffset.DecimalPlaces = 3;
            this.MeasureOffset.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            resources.ApplyResources(this.MeasureOffset, "MeasureOffset");
            this.MeasureOffset.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.MeasureOffset.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.MeasureOffset.Name = "MeasureOffset";
            // 
            // CalibrateCoeff
            // 
            this.CalibrateCoeff.DecimalPlaces = 6;
            this.CalibrateCoeff.Increment = new decimal(new int[] {
            1,
            0,
            0,
            262144});
            resources.ApplyResources(this.CalibrateCoeff, "CalibrateCoeff");
            this.CalibrateCoeff.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.CalibrateCoeff.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.CalibrateCoeff.Name = "CalibrateCoeff";
            // 
            // btn_AutoCalib
            // 
            resources.ApplyResources(this.btn_AutoCalib, "btn_AutoCalib");
            this.btn_AutoCalib.Name = "btn_AutoCalib";
            this.btn_AutoCalib.Click += new System.EventHandler(this.btn_AutoCalib_Click);
            // 
            // CCDMeasureValue
            // 
            this.CCDMeasureValue.DecimalPlaces = 3;
            this.CCDMeasureValue.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            resources.ApplyResources(this.CCDMeasureValue, "CCDMeasureValue");
            this.CCDMeasureValue.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.CCDMeasureValue.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.CCDMeasureValue.Name = "CCDMeasureValue";
            // 
            // ArtificialMeasureValue
            // 
            this.ArtificialMeasureValue.DecimalPlaces = 3;
            this.ArtificialMeasureValue.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            resources.ApplyResources(this.ArtificialMeasureValue, "ArtificialMeasureValue");
            this.ArtificialMeasureValue.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.ArtificialMeasureValue.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.ArtificialMeasureValue.Name = "ArtificialMeasureValue";
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(100)))), ((int)(((byte)(146)))));
            resources.ApplyResources(this.label2, "label2");
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Name = "label2";
            // 
            // EnableDataModel
            // 
            this.EnableDataModel.AutoCheck = true;
            this.EnableDataModel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(100)))), ((int)(((byte)(146)))));
            this.EnableDataModel.ForeColor = System.Drawing.Color.White;
            resources.ApplyResources(this.EnableDataModel, "EnableDataModel");
            this.EnableDataModel.Name = "EnableDataModel";
            // 
            // panel4
            // 
            this.panel4.Back = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(100)))), ((int)(((byte)(146)))));
            this.panel4.BackColor = System.Drawing.SystemColors.Control;
            this.panel4.Controls.Add(this.btn_FixReset);
            this.panel4.Controls.Add(this.MeasureOffset);
            this.panel4.Controls.Add(this.label10);
            this.panel4.Controls.Add(this.btn_AutoCalib);
            this.panel4.Controls.Add(this.CalibrateCoeff);
            this.panel4.Controls.Add(this.label9);
            this.panel4.Controls.Add(this.label8);
            this.panel4.Controls.Add(this.label3);
            this.panel4.Controls.Add(this.panel3);
            this.panel4.Controls.Add(this.label2);
            this.panel4.Controls.Add(this.select_detectionItem);
            this.panel4.Controls.Add(this.CCDMeasureValue);
            this.panel4.Controls.Add(this.ArtificialMeasureValue);
            resources.ApplyResources(this.panel4, "panel4");
            this.panel4.ForeColor = System.Drawing.Color.White;
            this.panel4.Name = "panel4";
            // 
            // btn_FixReset
            // 
            resources.ApplyResources(this.btn_FixReset, "btn_FixReset");
            this.btn_FixReset.Name = "btn_FixReset";
            this.btn_FixReset.Click += new System.EventHandler(this.btn_FixReset_Click);
            // 
            // label10
            // 
            this.label10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(100)))), ((int)(((byte)(146)))));
            resources.ApplyResources(this.label10, "label10");
            this.label10.ForeColor = System.Drawing.Color.White;
            this.label10.Name = "label10";
            // 
            // label9
            // 
            this.label9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(100)))), ((int)(((byte)(146)))));
            resources.ApplyResources(this.label9, "label9");
            this.label9.ForeColor = System.Drawing.Color.White;
            this.label9.Name = "label9";
            // 
            // label8
            // 
            this.label8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(100)))), ((int)(((byte)(146)))));
            resources.ApplyResources(this.label8, "label8");
            this.label8.ForeColor = System.Drawing.Color.White;
            this.label8.Name = "label8";
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(100)))), ((int)(((byte)(146)))));
            resources.ApplyResources(this.label3, "label3");
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Name = "label3";
            // 
            // panel3
            // 
            this.panel3.Back = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(119)))), ((int)(((byte)(255)))));
            this.panel3.BackColor = System.Drawing.SystemColors.Control;
            this.panel3.Controls.Add(this.divider2);
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.Name = "panel3";
            // 
            // divider2
            // 
            this.divider2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(149)))), ((int)(((byte)(254)))));
            this.divider2.ColorSplit = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(121)))), ((int)(((byte)(229)))));
            resources.ApplyResources(this.divider2, "divider2");
            this.divider2.ForeColor = System.Drawing.Color.White;
            this.divider2.Name = "divider2";
            this.divider2.Orientation = AntdUI.TOrientation.Left;
            // 
            // panel1
            // 
            this.panel1.Back = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(100)))), ((int)(((byte)(146)))));
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.EnableDataModel);
            this.panel1.Controls.Add(this.CompensateO);
            this.panel1.Controls.Add(this.CompensateF);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.ForeColor = System.Drawing.Color.White;
            this.panel1.Name = "panel1";
            // 
            // btn_SubscriptionData
            // 
            resources.ApplyResources(this.btn_SubscriptionData, "btn_SubscriptionData");
            this.btn_SubscriptionData.Name = "btn_SubscriptionData";
            this.btn_SubscriptionData.Click += new System.EventHandler(this.btn_SubscriptionData_Click);
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(100)))), ((int)(((byte)(146)))));
            resources.ApplyResources(this.label1, "label1");
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Name = "label1";
            // 
            // panel2
            // 
            this.panel2.Back = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(119)))), ((int)(((byte)(255)))));
            this.panel2.BackColor = System.Drawing.SystemColors.Control;
            this.panel2.Controls.Add(this.divider1);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // divider1
            // 
            this.divider1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(149)))), ((int)(((byte)(254)))));
            this.divider1.ColorSplit = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(121)))), ((int)(((byte)(229)))));
            resources.ApplyResources(this.divider1, "divider1");
            this.divider1.ForeColor = System.Drawing.Color.White;
            this.divider1.Name = "divider1";
            this.divider1.Orientation = AntdUI.TOrientation.Left;
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(100)))), ((int)(((byte)(146)))));
            resources.ApplyResources(this.label4, "label4");
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Name = "label4";
            // 
            // CompensateO
            // 
            this.CompensateO.DecimalPlaces = 3;
            resources.ApplyResources(this.CompensateO, "CompensateO");
            this.CompensateO.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.CompensateO.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.CompensateO.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.CompensateO.Name = "CompensateO";
            // 
            // CompensateF
            // 
            this.CompensateF.DecimalPlaces = 3;
            resources.ApplyResources(this.CompensateF, "CompensateF");
            this.CompensateF.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.CompensateF.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.CompensateF.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.CompensateF.Name = "CompensateF";
            // 
            // stackPanel1
            // 
            this.stackPanel1.Controls.Add(this.panel5);
            this.stackPanel1.Controls.Add(this.panel1);
            this.stackPanel1.Controls.Add(this.panel6);
            this.stackPanel1.Controls.Add(this.panel4);
            resources.ApplyResources(this.stackPanel1, "stackPanel1");
            this.stackPanel1.Name = "stackPanel1";
            this.stackPanel1.Vertical = true;
            // 
            // panel5
            // 
            this.panel5.Back = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(100)))), ((int)(((byte)(146)))));
            this.panel5.Controls.Add(this.btn_Save);
            resources.ApplyResources(this.panel5, "panel5");
            this.panel5.Name = "panel5";
            // 
            // btn_Save
            // 
            resources.ApplyResources(this.btn_Save, "btn_Save");
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // panel6
            // 
            this.panel6.Back = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(100)))), ((int)(((byte)(146)))));
            this.panel6.BackColor = System.Drawing.SystemColors.Control;
            this.panel6.Controls.Add(this.label12);
            this.panel6.Controls.Add(this.label11);
            this.panel6.Controls.Add(this.select_CalcFormula);
            this.panel6.Controls.Add(this.select_Cam2);
            this.panel6.Controls.Add(this.label7);
            this.panel6.Controls.Add(this.select_Cam1);
            this.panel6.Controls.Add(this.label6);
            this.panel6.Controls.Add(this.input_DataSource2);
            this.panel6.Controls.Add(this.label5);
            this.panel6.Controls.Add(this.input_DataSource1);
            this.panel6.Controls.Add(this.panel7);
            resources.ApplyResources(this.panel6, "panel6");
            this.panel6.ForeColor = System.Drawing.Color.White;
            this.panel6.Name = "panel6";
            // 
            // label11
            // 
            this.label11.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(100)))), ((int)(((byte)(146)))));
            resources.ApplyResources(this.label11, "label11");
            this.label11.ForeColor = System.Drawing.Color.White;
            this.label11.Name = "label11";
            // 
            // select_Cam2
            // 
            this.select_Cam2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(84)))), ((int)(((byte)(79)))), ((int)(((byte)(79)))));
            resources.ApplyResources(this.select_Cam2, "select_Cam2");
            this.select_Cam2.List = true;
            this.select_Cam2.MaxCount = 100;
            this.select_Cam2.Name = "select_Cam2";
            this.select_Cam2.SelectedIndexChanged += new AntdUI.IntEventHandler(this.select_Cam2_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(100)))), ((int)(((byte)(146)))));
            resources.ApplyResources(this.label7, "label7");
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Name = "label7";
            // 
            // select_Cam1
            // 
            this.select_Cam1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(84)))), ((int)(((byte)(79)))), ((int)(((byte)(79)))));
            resources.ApplyResources(this.select_Cam1, "select_Cam1");
            this.select_Cam1.List = true;
            this.select_Cam1.MaxCount = 100;
            this.select_Cam1.Name = "select_Cam1";
            this.select_Cam1.SelectedIndexChanged += new AntdUI.IntEventHandler(this.select_Cam1_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(100)))), ((int)(((byte)(146)))));
            resources.ApplyResources(this.label6, "label6");
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Name = "label6";
            // 
            // input_DataSource2
            // 
            resources.ApplyResources(this.input_DataSource2, "input_DataSource2");
            this.input_DataSource2.Name = "input_DataSource2";
            this.input_DataSource2.Click += new System.EventHandler(this.input_DataSource2_Click);
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(100)))), ((int)(((byte)(146)))));
            resources.ApplyResources(this.label5, "label5");
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Name = "label5";
            // 
            // input_DataSource1
            // 
            resources.ApplyResources(this.input_DataSource1, "input_DataSource1");
            this.input_DataSource1.Name = "input_DataSource1";
            this.input_DataSource1.Click += new System.EventHandler(this.input_DataSource1_Click);
            // 
            // panel7
            // 
            this.panel7.Back = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(119)))), ((int)(((byte)(255)))));
            this.panel7.BackColor = System.Drawing.SystemColors.Control;
            this.panel7.Controls.Add(this.divider3);
            resources.ApplyResources(this.panel7, "panel7");
            this.panel7.Name = "panel7";
            // 
            // divider3
            // 
            this.divider3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(149)))), ((int)(((byte)(254)))));
            this.divider3.ColorSplit = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(121)))), ((int)(((byte)(229)))));
            resources.ApplyResources(this.divider3, "divider3");
            this.divider3.ForeColor = System.Drawing.Color.White;
            this.divider3.Name = "divider3";
            this.divider3.Orientation = AntdUI.TOrientation.Left;
            // 
            // panel8
            // 
            this.panel8.Back = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(100)))), ((int)(((byte)(146)))));
            this.panel8.BackColor = System.Drawing.SystemColors.Control;
            this.panel8.Controls.Add(this.tree1);
            this.panel8.Controls.Add(this.panel9);
            resources.ApplyResources(this.panel8, "panel8");
            this.panel8.ForeColor = System.Drawing.Color.White;
            this.panel8.Name = "panel8";
            // 
            // panel9
            // 
            this.panel9.Back = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(119)))), ((int)(((byte)(255)))));
            this.panel9.BackColor = System.Drawing.SystemColors.Control;
            this.panel9.Controls.Add(this.divider4);
            resources.ApplyResources(this.panel9, "panel9");
            this.panel9.Name = "panel9";
            // 
            // divider4
            // 
            this.divider4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(149)))), ((int)(((byte)(254)))));
            this.divider4.ColorSplit = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(121)))), ((int)(((byte)(229)))));
            resources.ApplyResources(this.divider4, "divider4");
            this.divider4.ForeColor = System.Drawing.Color.White;
            this.divider4.Name = "divider4";
            this.divider4.Orientation = AntdUI.TOrientation.Left;
            // 
            // stackPanel2
            // 
            this.stackPanel2.Controls.Add(this.panel10);
            this.stackPanel2.Controls.Add(this.panel8);
            resources.ApplyResources(this.stackPanel2, "stackPanel2");
            this.stackPanel2.Name = "stackPanel2";
            this.stackPanel2.Vertical = true;
            // 
            // panel10
            // 
            this.panel10.Back = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(100)))), ((int)(((byte)(146)))));
            this.panel10.Controls.Add(this.btn_SubscriptionData);
            resources.ApplyResources(this.panel10, "panel10");
            this.panel10.Name = "panel10";
            // 
            // tree1
            // 
            this.tree1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(100)))), ((int)(((byte)(146)))));
            resources.ApplyResources(this.tree1, "tree1");
            this.tree1.ForeActive = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(114)))), ((int)(((byte)(246)))));
            this.tree1.ForeColor = System.Drawing.Color.White;
            this.tree1.Name = "tree1";
            this.tree1.SelectChanged += new AntdUI.TreeSelectEventHandler(this.tree1_SelectChanged);
            // 
            // label12
            // 
            this.label12.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(100)))), ((int)(((byte)(146)))));
            resources.ApplyResources(this.label12, "label12");
            this.label12.ForeColor = System.Drawing.Color.White;
            this.label12.Name = "label12";
            // 
            // select_CalcFormula
            // 
            this.select_CalcFormula.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(84)))), ((int)(((byte)(79)))), ((int)(((byte)(79)))));
            resources.ApplyResources(this.select_CalcFormula, "select_CalcFormula");
            this.select_CalcFormula.List = true;
            this.select_CalcFormula.MaxCount = 100;
            this.select_CalcFormula.Name = "select_CalcFormula";
            // 
            // Form_SizeStationEdit
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.stackPanel2);
            this.Controls.Add(this.stackPanel1);
            this.Name = "Form_SizeStationEdit";
            this.Load += new System.EventHandler(this.Form_SizeStationEdit_Load);
            this.panel4.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.stackPanel1.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.panel8.ResumeLayout(false);
            this.panel9.ResumeLayout(false);
            this.stackPanel2.ResumeLayout(false);
            this.panel10.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private AntdUI.Select select_detectionItem;
        private AntdUI.InputNumber CCDMeasureValue;
        private AntdUI.InputNumber ArtificialMeasureValue;
        private AntdUI.Label label2;
        private AntdUI.Button btn_AutoCalib;
        private AntdUI.InputNumber CalibrateCoeff;
        private AntdUI.InputNumber MeasureOffset;
        private AntdUI.Checkbox EnableDataModel;
        private AntdUI.Panel panel4;
        private AntdUI.Panel panel3;
        private AntdUI.Divider divider2;
        private AntdUI.Label label8;
        private AntdUI.Label label3;
        private AntdUI.Label label10;
        private AntdUI.Label label9;
        private AntdUI.Button btn_FixReset;
        private AntdUI.Panel panel1;
        private AntdUI.Panel panel2;
        private AntdUI.Divider divider1;
        private AntdUI.Label label1;
        private AntdUI.Label label4;
        private AntdUI.InputNumber CompensateO;
        private AntdUI.InputNumber CompensateF;
        private AntdUI.StackPanel stackPanel1;
        private AntdUI.Panel panel5;
        private AntdUI.Button btn_Save;
        private AntdUI.Panel panel6;
        private AntdUI.Panel panel7;
        private AntdUI.Divider divider3;
        private AntdUI.Label label6;
        private AntdUI.Input input_DataSource2;
        private AntdUI.Label label5;
        private AntdUI.Input input_DataSource1;
        private AntdUI.Label label7;
        private AntdUI.Select select_Cam1;
        private AntdUI.Label label11;
        private AntdUI.Select select_Cam2;
        private AntdUI.Button btn_SubscriptionData;
        private AntdUI.Panel panel8;
        private AntdUI.Panel panel9;
        private AntdUI.Divider divider4;
        private AntdUI.StackPanel stackPanel2;
        private AntdUI.Panel panel10;
        private AntdUI.Tree tree1;
        private AntdUI.Label label12;
        private AntdUI.Select select_CalcFormula;
    }
}
namespace Mef_VisionFrame
{
    partial class Form_SizeEdit
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_SizeEdit));
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
            this.label1 = new AntdUI.Label();
            this.panel2 = new AntdUI.Panel();
            this.divider1 = new AntdUI.Divider();
            this.label4 = new AntdUI.Label();
            this.CompensateO = new AntdUI.InputNumber();
            this.CompensateF = new AntdUI.InputNumber();
            this.stackPanel1 = new AntdUI.StackPanel();
            this.panel5 = new AntdUI.Panel();
            this.btn_Save = new AntdUI.Button();
            this.panel4.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.stackPanel1.SuspendLayout();
            this.panel5.SuspendLayout();
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
            // Form_SizeEdit
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.stackPanel1);
            this.Name = "Form_SizeEdit";
            this.panel4.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.stackPanel1.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
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
    }
}
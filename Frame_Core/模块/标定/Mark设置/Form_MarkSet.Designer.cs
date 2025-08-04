namespace Frame_Core
{
    partial class Form_MarkSet
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
            this.label1 = new System.Windows.Forms.Label();
            this.cbx_CamSelect = new System.Windows.Forms.ComboBox();
            this.Nud_MarkY = new System.Windows.Forms.NumericUpDown();
            this.label33 = new System.Windows.Forms.Label();
            this.Nud_MarkX = new System.Windows.Forms.NumericUpDown();
            this.label32 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.nud_MarkPosRange = new System.Windows.Forms.NumericUpDown();
            this.label34 = new System.Windows.Forms.Label();
            this.cbx_MarkPosRangeBJTJ = new System.Windows.Forms.CheckBox();
            this.btn_Save = new System.Windows.Forms.Button();
            this.Nud_MarkR = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.nud_UnidentificationMarkNum = new System.Windows.Forms.NumericUpDown();
            this.cbx_UnidentificationMarkBJTJ = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cbx_AllCamOneClickSetup = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.Nud_MarkY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Nud_MarkX)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_MarkPosRange)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Nud_MarkR)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_UnidentificationMarkNum)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "相机选择";
            // 
            // cbx_CamSelect
            // 
            this.cbx_CamSelect.FormattingEnabled = true;
            this.cbx_CamSelect.Location = new System.Drawing.Point(90, 21);
            this.cbx_CamSelect.Name = "cbx_CamSelect";
            this.cbx_CamSelect.Size = new System.Drawing.Size(121, 20);
            this.cbx_CamSelect.TabIndex = 2;
            this.cbx_CamSelect.SelectedIndexChanged += new System.EventHandler(this.cbx_CamSelect_SelectedIndexChanged);
            // 
            // Nud_MarkY
            // 
            this.Nud_MarkY.DecimalPlaces = 3;
            this.Nud_MarkY.Location = new System.Drawing.Point(123, 78);
            this.Nud_MarkY.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.Nud_MarkY.Name = "Nud_MarkY";
            this.Nud_MarkY.Size = new System.Drawing.Size(120, 21);
            this.Nud_MarkY.TabIndex = 38;
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Location = new System.Drawing.Point(17, 82);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(95, 12);
            this.label33.TabIndex = 37;
            this.label33.Text = "Mark基准点Y/Pix";
            // 
            // Nud_MarkX
            // 
            this.Nud_MarkX.DecimalPlaces = 3;
            this.Nud_MarkX.Location = new System.Drawing.Point(123, 36);
            this.Nud_MarkX.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.Nud_MarkX.Name = "Nud_MarkX";
            this.Nud_MarkX.Size = new System.Drawing.Size(120, 21);
            this.Nud_MarkX.TabIndex = 36;
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Location = new System.Drawing.Point(17, 40);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(95, 12);
            this.label32.TabIndex = 35;
            this.label32.Text = "Mark基准点X/Pix";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.Nud_MarkR);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label32);
            this.groupBox1.Controls.Add(this.Nud_MarkY);
            this.groupBox1.Controls.Add(this.Nud_MarkX);
            this.groupBox1.Controls.Add(this.label33);
            this.groupBox1.Location = new System.Drawing.Point(28, 92);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(593, 179);
            this.groupBox1.TabIndex = 39;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "相机参数";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(272, 14);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(161, 41);
            this.button1.TabIndex = 40;
            this.button1.Text = "一键设置Mark基准";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // nud_MarkPosRange
            // 
            this.nud_MarkPosRange.Location = new System.Drawing.Point(174, 43);
            this.nud_MarkPosRange.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.nud_MarkPosRange.Name = "nud_MarkPosRange";
            this.nud_MarkPosRange.Size = new System.Drawing.Size(56, 21);
            this.nud_MarkPosRange.TabIndex = 42;
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Location = new System.Drawing.Point(17, 47);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(137, 12);
            this.label34.TabIndex = 41;
            this.label34.Text = "Mark中心位置波动+-/Pix";
            // 
            // cbx_MarkPosRangeBJTJ
            // 
            this.cbx_MarkPosRangeBJTJ.AutoSize = true;
            this.cbx_MarkPosRangeBJTJ.Location = new System.Drawing.Point(261, 45);
            this.cbx_MarkPosRangeBJTJ.Name = "cbx_MarkPosRangeBJTJ";
            this.cbx_MarkPosRangeBJTJ.Size = new System.Drawing.Size(72, 16);
            this.cbx_MarkPosRangeBJTJ.TabIndex = 43;
            this.cbx_MarkPosRangeBJTJ.Text = "报警停机";
            this.cbx_MarkPosRangeBJTJ.UseVisualStyleBackColor = true;
            // 
            // btn_Save
            // 
            this.btn_Save.Location = new System.Drawing.Point(482, 12);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(139, 61);
            this.btn_Save.TabIndex = 46;
            this.btn_Save.Text = "保存参数";
            this.btn_Save.UseVisualStyleBackColor = true;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // Nud_MarkR
            // 
            this.Nud_MarkR.DecimalPlaces = 3;
            this.Nud_MarkR.Location = new System.Drawing.Point(123, 116);
            this.Nud_MarkR.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.Nud_MarkR.Minimum = new decimal(new int[] {
            360,
            0,
            0,
            -2147483648});
            this.Nud_MarkR.Name = "Nud_MarkR";
            this.Nud_MarkR.Size = new System.Drawing.Size(120, 21);
            this.Nud_MarkR.TabIndex = 45;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 120);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 12);
            this.label2.TabIndex = 44;
            this.label2.Text = "Mark基准点R/Pix";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 87);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(155, 12);
            this.label3.TabIndex = 46;
            this.label3.Text = "连续N个未找到Mark报警停机";
            // 
            // nud_UnidentificationMarkNum
            // 
            this.nud_UnidentificationMarkNum.Location = new System.Drawing.Point(174, 83);
            this.nud_UnidentificationMarkNum.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.nud_UnidentificationMarkNum.Name = "nud_UnidentificationMarkNum";
            this.nud_UnidentificationMarkNum.Size = new System.Drawing.Size(56, 21);
            this.nud_UnidentificationMarkNum.TabIndex = 47;
            // 
            // cbx_UnidentificationMarkBJTJ
            // 
            this.cbx_UnidentificationMarkBJTJ.AutoSize = true;
            this.cbx_UnidentificationMarkBJTJ.Location = new System.Drawing.Point(261, 86);
            this.cbx_UnidentificationMarkBJTJ.Name = "cbx_UnidentificationMarkBJTJ";
            this.cbx_UnidentificationMarkBJTJ.Size = new System.Drawing.Size(72, 16);
            this.cbx_UnidentificationMarkBJTJ.TabIndex = 48;
            this.cbx_UnidentificationMarkBJTJ.Text = "报警停机";
            this.cbx_UnidentificationMarkBJTJ.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cbx_AllCamOneClickSetup);
            this.groupBox2.Controls.Add(this.cbx_UnidentificationMarkBJTJ);
            this.groupBox2.Controls.Add(this.label34);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.nud_MarkPosRange);
            this.groupBox2.Controls.Add(this.nud_UnidentificationMarkNum);
            this.groupBox2.Controls.Add(this.cbx_MarkPosRangeBJTJ);
            this.groupBox2.Location = new System.Drawing.Point(28, 304);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(593, 141);
            this.groupBox2.TabIndex = 47;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "异常设置";
            // 
            // cbx_AllCamOneClickSetup
            // 
            this.cbx_AllCamOneClickSetup.AutoSize = true;
            this.cbx_AllCamOneClickSetup.Location = new System.Drawing.Point(374, 47);
            this.cbx_AllCamOneClickSetup.Name = "cbx_AllCamOneClickSetup";
            this.cbx_AllCamOneClickSetup.Size = new System.Drawing.Size(120, 16);
            this.cbx_AllCamOneClickSetup.TabIndex = 49;
            this.cbx_AllCamOneClickSetup.Text = "全部相机一键设置";
            this.cbx_AllCamOneClickSetup.UseVisualStyleBackColor = true;
            // 
            // Form_MarkSet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(665, 563);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btn_Save);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbx_CamSelect);
            this.Name = "Form_MarkSet";
            this.Text = "Form_MarkSet";
            this.Load += new System.EventHandler(this.Form_MarkSet_Load);
            ((System.ComponentModel.ISupportInitialize)(this.Nud_MarkY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Nud_MarkX)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_MarkPosRange)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Nud_MarkR)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_UnidentificationMarkNum)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbx_CamSelect;
        private System.Windows.Forms.NumericUpDown Nud_MarkY;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.NumericUpDown Nud_MarkX;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.NumericUpDown nud_MarkPosRange;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.CheckBox cbx_MarkPosRangeBJTJ;
        private System.Windows.Forms.Button btn_Save;
        private System.Windows.Forms.NumericUpDown Nud_MarkR;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox cbx_UnidentificationMarkBJTJ;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown nud_UnidentificationMarkNum;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox cbx_AllCamOneClickSetup;
    }
}
namespace Frame_Core
{
    partial class Light_Form
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btn_Light_Send = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.yTextBox1 = new YYSD.UICore.Control.YTextBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.LightName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LightType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.LightCOM = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LightChannel1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LightChannel2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.save_parmeter = new System.Windows.Forms.Button();
            this.Cbx_IsEnble = new System.Windows.Forms.GroupBox();
            this.cbx_EnableOverHang = new System.Windows.Forms.CheckBox();
            this.UpLightValue2 = new System.Windows.Forms.NumericUpDown();
            this.JpValue2 = new System.Windows.Forms.NumericUpDown();
            this.DownLightValue2 = new System.Windows.Forms.NumericUpDown();
            this.UpLightValue1 = new System.Windows.Forms.NumericUpDown();
            this.JpValue1 = new System.Windows.Forms.NumericUpDown();
            this.DownLightValue1 = new System.Windows.Forms.NumericUpDown();
            this.cbx_LightChange = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.Cbx_IsEnble.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.UpLightValue2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.JpValue2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DownLightValue2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.UpLightValue1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.JpValue1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DownLightValue1)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_Light_Send
            // 
            this.btn_Light_Send.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_Light_Send.Location = new System.Drawing.Point(607, 17);
            this.btn_Light_Send.Name = "btn_Light_Send";
            this.btn_Light_Send.Size = new System.Drawing.Size(87, 27);
            this.btn_Light_Send.TabIndex = 122;
            this.btn_Light_Send.Text = "输入参数";
            this.btn_Light_Send.UseVisualStyleBackColor = true;
            this.btn_Light_Send.Click += new System.EventHandler(this.btn_Light_Send_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.yTextBox1);
            this.groupBox1.Controls.Add(this.dataGridView1);
            this.groupBox1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox1.Location = new System.Drawing.Point(10, 185);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(133, 133, 133, 53);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(13);
            this.groupBox1.Size = new System.Drawing.Size(1127, 329);
            this.groupBox1.TabIndex = 105;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "光源设置";
            // 
            // yTextBox1
            // 
            this.yTextBox1.Location = new System.Drawing.Point(804, 269);
            this.yTextBox1.name = "Name";
            this.yTextBox1.Name = "yTextBox1";
            this.yTextBox1.Size = new System.Drawing.Size(8, 8);
            this.yTextBox1.TabIndex = 1;
            this.yTextBox1.value = "";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToOrderColumns = true;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.BackgroundColor = System.Drawing.Color.PapayaWhip;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.LightName,
            this.LightType,
            this.LightCOM,
            this.LightChannel1,
            this.LightChannel2,
            this.Column1,
            this.Column2,
            this.Column3,
            this.Column4,
            this.Column5,
            this.Column6});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.GridColor = System.Drawing.Color.DarkOrange;
            this.dataGridView1.Location = new System.Drawing.Point(13, 27);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(2);
            this.dataGridView1.Name = "dataGridView1";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridView1.RowHeadersWidth = 15;
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.Size = new System.Drawing.Size(1101, 289);
            this.dataGridView1.TabIndex = 0;
            // 
            // LightName
            // 
            this.LightName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.LightName.FillWeight = 45F;
            this.LightName.HeaderText = "光源名称";
            this.LightName.MinimumWidth = 8;
            this.LightName.Name = "LightName";
            this.LightName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // LightType
            // 
            this.LightType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.LightType.FillWeight = 55F;
            this.LightType.HeaderText = "光源类型";
            this.LightType.Items.AddRange(new object[] {
            "OPTv1（9600）",
            "OPTv2（38400）",
            "OPTControllerAPI",
            "OPTv3",
            "乐视1（\'$\'）",
            "乐视2（\'#\'）",
            "乐视3（\'&\'）",
            "CST（康视达）",
            "WDP（沃德普38400）",
            "WDP（沃德普19200）",
            "嘉励（19200）"});
            this.LightType.MinimumWidth = 8;
            this.LightType.Name = "LightType";
            // 
            // LightCOM
            // 
            this.LightCOM.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.LightCOM.FillWeight = 30F;
            this.LightCOM.HeaderText = "光源连接COM";
            this.LightCOM.MinimumWidth = 15;
            this.LightCOM.Name = "LightCOM";
            this.LightCOM.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // LightChannel1
            // 
            this.LightChannel1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.LightChannel1.FillWeight = 28.21781F;
            this.LightChannel1.HeaderText = "亮度-通道1";
            this.LightChannel1.MinimumWidth = 8;
            this.LightChannel1.Name = "LightChannel1";
            this.LightChannel1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // LightChannel2
            // 
            this.LightChannel2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.LightChannel2.FillWeight = 31.35312F;
            this.LightChannel2.HeaderText = "亮度-通道2";
            this.LightChannel2.Name = "LightChannel2";
            this.LightChannel2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Column1
            // 
            this.Column1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column1.FillWeight = 31.35312F;
            this.Column1.HeaderText = "亮度-通道3";
            this.Column1.Name = "Column1";
            // 
            // Column2
            // 
            this.Column2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column2.FillWeight = 31.35312F;
            this.Column2.HeaderText = "亮度-通道4";
            this.Column2.Name = "Column2";
            // 
            // Column3
            // 
            this.Column3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column3.FillWeight = 31.35312F;
            this.Column3.HeaderText = "亮度-通道5";
            this.Column3.Name = "Column3";
            // 
            // Column4
            // 
            this.Column4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column4.FillWeight = 31.35312F;
            this.Column4.HeaderText = "亮度-通道6";
            this.Column4.Name = "Column4";
            // 
            // Column5
            // 
            this.Column5.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column5.FillWeight = 31.35312F;
            this.Column5.HeaderText = "亮度-通道7";
            this.Column5.Name = "Column5";
            // 
            // Column6
            // 
            this.Column6.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column6.FillWeight = 31.35312F;
            this.Column6.HeaderText = "亮度-通道8";
            this.Column6.Name = "Column6";
            // 
            // save_parmeter
            // 
            this.save_parmeter.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.save_parmeter.Location = new System.Drawing.Point(758, 12);
            this.save_parmeter.Name = "save_parmeter";
            this.save_parmeter.Size = new System.Drawing.Size(107, 37);
            this.save_parmeter.TabIndex = 123;
            this.save_parmeter.Text = "保存设置";
            this.save_parmeter.UseVisualStyleBackColor = true;
            this.save_parmeter.Click += new System.EventHandler(this.save_parmeter_Click);
            // 
            // Cbx_IsEnble
            // 
            this.Cbx_IsEnble.Controls.Add(this.cbx_EnableOverHang);
            this.Cbx_IsEnble.Controls.Add(this.UpLightValue2);
            this.Cbx_IsEnble.Controls.Add(this.JpValue2);
            this.Cbx_IsEnble.Controls.Add(this.DownLightValue2);
            this.Cbx_IsEnble.Controls.Add(this.UpLightValue1);
            this.Cbx_IsEnble.Controls.Add(this.JpValue1);
            this.Cbx_IsEnble.Controls.Add(this.DownLightValue1);
            this.Cbx_IsEnble.Controls.Add(this.cbx_LightChange);
            this.Cbx_IsEnble.Controls.Add(this.label6);
            this.Cbx_IsEnble.Controls.Add(this.label7);
            this.Cbx_IsEnble.Controls.Add(this.label8);
            this.Cbx_IsEnble.Controls.Add(this.label5);
            this.Cbx_IsEnble.Controls.Add(this.label4);
            this.Cbx_IsEnble.Controls.Add(this.label3);
            this.Cbx_IsEnble.Controls.Add(this.label2);
            this.Cbx_IsEnble.Controls.Add(this.label1);
            this.Cbx_IsEnble.Location = new System.Drawing.Point(10, 13);
            this.Cbx_IsEnble.Name = "Cbx_IsEnble";
            this.Cbx_IsEnble.Size = new System.Drawing.Size(591, 161);
            this.Cbx_IsEnble.TabIndex = 124;
            this.Cbx_IsEnble.TabStop = false;
            this.Cbx_IsEnble.Text = "OverHang亮度设置";
            this.Cbx_IsEnble.Visible = false;
            // 
            // cbx_EnableOverHang
            // 
            this.cbx_EnableOverHang.AutoSize = true;
            this.cbx_EnableOverHang.Location = new System.Drawing.Point(450, 63);
            this.cbx_EnableOverHang.Name = "cbx_EnableOverHang";
            this.cbx_EnableOverHang.Size = new System.Drawing.Size(72, 16);
            this.cbx_EnableOverHang.TabIndex = 21;
            this.cbx_EnableOverHang.Text = "是否启用";
            this.cbx_EnableOverHang.UseVisualStyleBackColor = true;
            // 
            // UpLightValue2
            // 
            this.UpLightValue2.Location = new System.Drawing.Point(301, 124);
            this.UpLightValue2.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.UpLightValue2.Name = "UpLightValue2";
            this.UpLightValue2.Size = new System.Drawing.Size(109, 21);
            this.UpLightValue2.TabIndex = 20;
            // 
            // JpValue2
            // 
            this.JpValue2.Location = new System.Drawing.Point(301, 91);
            this.JpValue2.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.JpValue2.Name = "JpValue2";
            this.JpValue2.Size = new System.Drawing.Size(109, 21);
            this.JpValue2.TabIndex = 19;
            // 
            // DownLightValue2
            // 
            this.DownLightValue2.Location = new System.Drawing.Point(301, 58);
            this.DownLightValue2.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.DownLightValue2.Name = "DownLightValue2";
            this.DownLightValue2.Size = new System.Drawing.Size(109, 21);
            this.DownLightValue2.TabIndex = 18;
            // 
            // UpLightValue1
            // 
            this.UpLightValue1.Location = new System.Drawing.Point(95, 124);
            this.UpLightValue1.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.UpLightValue1.Name = "UpLightValue1";
            this.UpLightValue1.Size = new System.Drawing.Size(109, 21);
            this.UpLightValue1.TabIndex = 17;
            // 
            // JpValue1
            // 
            this.JpValue1.Location = new System.Drawing.Point(95, 91);
            this.JpValue1.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.JpValue1.Name = "JpValue1";
            this.JpValue1.Size = new System.Drawing.Size(109, 21);
            this.JpValue1.TabIndex = 16;
            // 
            // DownLightValue1
            // 
            this.DownLightValue1.Location = new System.Drawing.Point(95, 58);
            this.DownLightValue1.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.DownLightValue1.Name = "DownLightValue1";
            this.DownLightValue1.Size = new System.Drawing.Size(109, 21);
            this.DownLightValue1.TabIndex = 15;
            // 
            // cbx_LightChange
            // 
            this.cbx_LightChange.AutoSize = true;
            this.cbx_LightChange.Location = new System.Drawing.Point(450, 27);
            this.cbx_LightChange.Name = "cbx_LightChange";
            this.cbx_LightChange.Size = new System.Drawing.Size(120, 16);
            this.cbx_LightChange.TabIndex = 14;
            this.cbx_LightChange.Text = "阴阳亮度控制切换";
            this.cbx_LightChange.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(221, 126);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 12);
            this.label6.TabIndex = 12;
            this.label6.Text = "上单片亮度";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(233, 93);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 12);
            this.label7.TabIndex = 10;
            this.label7.Text = "极片亮度";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(221, 60);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(65, 12);
            this.label8.TabIndex = 9;
            this.label8.Text = "下单片亮度";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(211, 27);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(71, 12);
            this.label5.TabIndex = 7;
            this.label5.Text = "光源通道2：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 126);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 5;
            this.label4.Text = "上单片亮度";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(27, 93);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 3;
            this.label3.Text = "极片亮度";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "下单片亮度";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "光源通道1：";
            // 
            // Light_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkSeaGreen;
            this.ClientSize = new System.Drawing.Size(1154, 532);
            this.Controls.Add(this.Cbx_IsEnble);
            this.Controls.Add(this.save_parmeter);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btn_Light_Send);
            this.MaximizeBox = false;
            this.Name = "Light_Form";
            this.Text = "光源设置";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Light_Form_FormClosing);
            this.Load += new System.EventHandler(this.Light_Form_Load);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.Cbx_IsEnble.ResumeLayout(false);
            this.Cbx_IsEnble.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.UpLightValue2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.JpValue2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DownLightValue2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.UpLightValue1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.JpValue1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DownLightValue1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btn_Light_Send;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button save_parmeter;
        private YYSD.UICore.Control.YTextBox yTextBox1;
        private System.Windows.Forms.GroupBox Cbx_IsEnble;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox cbx_LightChange;
        private System.Windows.Forms.NumericUpDown UpLightValue2;
        private System.Windows.Forms.NumericUpDown JpValue2;
        private System.Windows.Forms.NumericUpDown DownLightValue2;
        private System.Windows.Forms.NumericUpDown UpLightValue1;
        private System.Windows.Forms.NumericUpDown JpValue1;
        private System.Windows.Forms.NumericUpDown DownLightValue1;
        private System.Windows.Forms.CheckBox cbx_EnableOverHang;
        private System.Windows.Forms.DataGridViewTextBoxColumn LightName;
        private System.Windows.Forms.DataGridViewComboBoxColumn LightType;
        private System.Windows.Forms.DataGridViewTextBoxColumn LightCOM;
        private System.Windows.Forms.DataGridViewTextBoxColumn LightChannel1;
        private System.Windows.Forms.DataGridViewTextBoxColumn LightChannel2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column6;
    }
}
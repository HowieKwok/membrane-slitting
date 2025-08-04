namespace Frame_Core
{
    partial class Form_BTSCorrect
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
            this.cbx_JPType = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.btnELConnect = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tbxLocalMachinePort = new System.Windows.Forms.NumericUpDown();
            this.tbxELMachinePort = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.tbxLocalMachineIP = new System.Windows.Forms.TextBox();
            this.tbxELMachineIP = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.btnHZJP = new System.Windows.Forms.Button();
            this.num_Correctdata = new System.Windows.Forms.NumericUpDown();
            this.groupBox_自动开启纠偏 = new System.Windows.Forms.GroupBox();
            this.num_JP_HJLength = new System.Windows.Forms.NumericUpDown();
            this.cb_EnHJJP = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnManuJP = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btn_Save = new System.Windows.Forms.Button();
            this.cbx_JP_Sign = new System.Windows.Forms.CheckBox();
            this.cbx_ENAutoJP = new System.Windows.Forms.CheckBox();
            this.num_TJJP_Pro = new System.Windows.Forms.NumericUpDown();
            this.label22 = new System.Windows.Forms.Label();
            this.label34 = new System.Windows.Forms.Label();
            this.num_TJDistance = new System.Windows.Forms.NumericUpDown();
            this.num_TJJPSign_LX_num = new System.Windows.Forms.NumericUpDown();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.cbx_TJJPSign_LX_En = new System.Windows.Forms.CheckBox();
            this.cbx_maxBJTJEn = new System.Windows.Forms.CheckBox();
            this.num_maxBJTJNum = new System.Windows.Forms.NumericUpDown();
            this.label30 = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.num_maxTJJPnum = new System.Windows.Forms.NumericUpDown();
            this.num_minTJJPnum = new System.Windows.Forms.NumericUpDown();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.cbx_CorrectLogic = new Frame_Core.ControlComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.tbxLocalMachinePort)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbxELMachinePort)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Correctdata)).BeginInit();
            this.groupBox_自动开启纠偏.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_JP_HJLength)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_TJJP_Pro)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_TJDistance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_TJJPSign_LX_num)).BeginInit();
            this.groupBox6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_maxBJTJNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_maxTJJPnum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_minTJJPnum)).BeginInit();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbx_JPType
            // 
            this.cbx_JPType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbx_JPType.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbx_JPType.FormattingEnabled = true;
            this.cbx_JPType.Location = new System.Drawing.Point(6, 68);
            this.cbx_JPType.Name = "cbx_JPType";
            this.cbx_JPType.Size = new System.Drawing.Size(140, 22);
            this.cbx_JPType.TabIndex = 173;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("宋体", 10.5F);
            this.label12.Location = new System.Drawing.Point(15, 33);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(91, 14);
            this.label12.TabIndex = 172;
            this.label12.Text = "纠偏通讯类型";
            // 
            // btnELConnect
            // 
            this.btnELConnect.Location = new System.Drawing.Point(631, 29);
            this.btnELConnect.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.btnELConnect.Name = "btnELConnect";
            this.btnELConnect.Size = new System.Drawing.Size(202, 67);
            this.btnELConnect.TabIndex = 162;
            this.btnELConnect.Text = "连接";
            this.btnELConnect.UseVisualStyleBackColor = true;
            this.btnELConnect.Click += new System.EventHandler(this.btnELConnect_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 10.5F);
            this.label2.Location = new System.Drawing.Point(152, 74);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 14);
            this.label2.TabIndex = 170;
            this.label2.Text = "电脑本地IP";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 10.5F);
            this.label1.Location = new System.Drawing.Point(152, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 14);
            this.label1.TabIndex = 166;
            this.label1.Text = "纠偏器IP";
            // 
            // tbxLocalMachinePort
            // 
            this.tbxLocalMachinePort.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbxLocalMachinePort.Location = new System.Drawing.Point(515, 72);
            this.tbxLocalMachinePort.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.tbxLocalMachinePort.Minimum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.tbxLocalMachinePort.Name = "tbxLocalMachinePort";
            this.tbxLocalMachinePort.Size = new System.Drawing.Size(93, 21);
            this.tbxLocalMachinePort.TabIndex = 169;
            this.tbxLocalMachinePort.Value = new decimal(new int[] {
            200,
            0,
            0,
            0});
            // 
            // tbxELMachinePort
            // 
            this.tbxELMachinePort.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbxELMachinePort.Location = new System.Drawing.Point(515, 31);
            this.tbxELMachinePort.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.tbxELMachinePort.Minimum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.tbxELMachinePort.Name = "tbxELMachinePort";
            this.tbxELMachinePort.Size = new System.Drawing.Size(93, 21);
            this.tbxELMachinePort.TabIndex = 165;
            this.tbxELMachinePort.Value = new decimal(new int[] {
            200,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 10.5F);
            this.label3.Location = new System.Drawing.Point(402, 77);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(91, 14);
            this.label3.TabIndex = 168;
            this.label3.Text = "电脑本地端口";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("宋体", 10.5F);
            this.label19.Location = new System.Drawing.Point(402, 34);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(77, 14);
            this.label19.TabIndex = 164;
            this.label19.Text = "纠偏器端口";
            // 
            // tbxLocalMachineIP
            // 
            this.tbxLocalMachineIP.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbxLocalMachineIP.Location = new System.Drawing.Point(252, 69);
            this.tbxLocalMachineIP.Name = "tbxLocalMachineIP";
            this.tbxLocalMachineIP.Size = new System.Drawing.Size(141, 21);
            this.tbxLocalMachineIP.TabIndex = 171;
            // 
            // tbxELMachineIP
            // 
            this.tbxELMachineIP.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbxELMachineIP.Location = new System.Drawing.Point(252, 26);
            this.tbxELMachineIP.Name = "tbxELMachineIP";
            this.tbxELMachineIP.Size = new System.Drawing.Size(141, 21);
            this.tbxELMachineIP.TabIndex = 167;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("宋体", 10.5F);
            this.label18.Location = new System.Drawing.Point(16, 44);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(119, 14);
            this.label18.TabIndex = 154;
            this.label18.Text = "手动纠编量(mm)：";
            // 
            // btnHZJP
            // 
            this.btnHZJP.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnHZJP.Location = new System.Drawing.Point(472, 26);
            this.btnHZJP.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.btnHZJP.Name = "btnHZJP";
            this.btnHZJP.Size = new System.Drawing.Size(168, 48);
            this.btnHZJP.TabIndex = 152;
            this.btnHZJP.Text = "纠编伺服回中";
            this.btnHZJP.UseVisualStyleBackColor = true;
            this.btnHZJP.Visible = false;
            // 
            // num_Correctdata
            // 
            this.num_Correctdata.DecimalPlaces = 2;
            this.num_Correctdata.Font = new System.Drawing.Font("宋体", 9F);
            this.num_Correctdata.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.num_Correctdata.Location = new System.Drawing.Point(137, 39);
            this.num_Correctdata.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.num_Correctdata.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.num_Correctdata.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            -2147483648});
            this.num_Correctdata.Name = "num_Correctdata";
            this.num_Correctdata.Size = new System.Drawing.Size(97, 21);
            this.num_Correctdata.TabIndex = 155;
            this.num_Correctdata.Value = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            // 
            // groupBox_自动开启纠偏
            // 
            this.groupBox_自动开启纠偏.Controls.Add(this.num_JP_HJLength);
            this.groupBox_自动开启纠偏.Controls.Add(this.cb_EnHJJP);
            this.groupBox_自动开启纠偏.Controls.Add(this.label5);
            this.groupBox_自动开启纠偏.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox_自动开启纠偏.Location = new System.Drawing.Point(20, 502);
            this.groupBox_自动开启纠偏.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox_自动开启纠偏.Name = "groupBox_自动开启纠偏";
            this.groupBox_自动开启纠偏.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox_自动开启纠偏.Size = new System.Drawing.Size(663, 69);
            this.groupBox_自动开启纠偏.TabIndex = 165;
            this.groupBox_自动开启纠偏.TabStop = false;
            this.groupBox_自动开启纠偏.Text = "自动开启纠偏设置";
            // 
            // num_JP_HJLength
            // 
            this.num_JP_HJLength.Font = new System.Drawing.Font("宋体", 9F);
            this.num_JP_HJLength.Location = new System.Drawing.Point(200, 31);
            this.num_JP_HJLength.Margin = new System.Windows.Forms.Padding(4);
            this.num_JP_HJLength.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.num_JP_HJLength.Name = "num_JP_HJLength";
            this.num_JP_HJLength.Size = new System.Drawing.Size(47, 21);
            this.num_JP_HJLength.TabIndex = 156;
            this.num_JP_HJLength.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // cb_EnHJJP
            // 
            this.cb_EnHJJP.AutoSize = true;
            this.cb_EnHJJP.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cb_EnHJJP.Location = new System.Drawing.Point(268, 32);
            this.cb_EnHJJP.Margin = new System.Windows.Forms.Padding(4);
            this.cb_EnHJJP.Name = "cb_EnHJJP";
            this.cb_EnHJJP.Size = new System.Drawing.Size(122, 20);
            this.cb_EnHJJP.TabIndex = 158;
            this.cb_EnHJJP.Text = "自动开启纠偏";
            this.cb_EnHJJP.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(14, 32);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(183, 16);
            this.label5.TabIndex = 157;
            this.label5.Text = "换卷N米开启自动纠偏(m)";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label18);
            this.groupBox2.Controls.Add(this.btnHZJP);
            this.groupBox2.Controls.Add(this.btnManuJP);
            this.groupBox2.Controls.Add(this.num_Correctdata);
            this.groupBox2.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox2.Location = new System.Drawing.Point(20, 369);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox2.Size = new System.Drawing.Size(662, 107);
            this.groupBox2.TabIndex = 164;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "纠偏控制";
            // 
            // btnManuJP
            // 
            this.btnManuJP.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnManuJP.Location = new System.Drawing.Point(260, 26);
            this.btnManuJP.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.btnManuJP.Name = "btnManuJP";
            this.btnManuJP.Size = new System.Drawing.Size(168, 48);
            this.btnManuJP.TabIndex = 153;
            this.btnManuJP.Text = "手动纠偏";
            this.btnManuJP.UseVisualStyleBackColor = true;
            this.btnManuJP.Click += new System.EventHandler(this.btnManuJP_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbx_JPType);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.btnELConnect);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.tbxLocalMachinePort);
            this.groupBox1.Controls.Add(this.tbxELMachinePort);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label19);
            this.groupBox1.Controls.Add(this.tbxLocalMachineIP);
            this.groupBox1.Controls.Add(this.tbxELMachineIP);
            this.groupBox1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox1.Location = new System.Drawing.Point(21, 28);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(852, 121);
            this.groupBox1.TabIndex = 163;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "连接设置";
            // 
            // btn_Save
            // 
            this.btn_Save.Location = new System.Drawing.Point(903, 45);
            this.btn_Save.Margin = new System.Windows.Forms.Padding(5);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(117, 84);
            this.btn_Save.TabIndex = 162;
            this.btn_Save.Text = "保存参数";
            this.btn_Save.UseVisualStyleBackColor = true;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // cbx_JP_Sign
            // 
            this.cbx_JP_Sign.AutoSize = true;
            this.cbx_JP_Sign.Font = new System.Drawing.Font("宋体", 10.5F);
            this.cbx_JP_Sign.Location = new System.Drawing.Point(170, 26);
            this.cbx_JP_Sign.Margin = new System.Windows.Forms.Padding(4);
            this.cbx_JP_Sign.Name = "cbx_JP_Sign";
            this.cbx_JP_Sign.Size = new System.Drawing.Size(166, 18);
            this.cbx_JP_Sign.TabIndex = 162;
            this.cbx_JP_Sign.Text = "纠偏发送符号\"±\"反转";
            this.cbx_JP_Sign.UseVisualStyleBackColor = true;
            // 
            // cbx_ENAutoJP
            // 
            this.cbx_ENAutoJP.AutoSize = true;
            this.cbx_ENAutoJP.Font = new System.Drawing.Font("宋体", 10.5F);
            this.cbx_ENAutoJP.Location = new System.Drawing.Point(17, 26);
            this.cbx_ENAutoJP.Margin = new System.Windows.Forms.Padding(4);
            this.cbx_ENAutoJP.Name = "cbx_ENAutoJP";
            this.cbx_ENAutoJP.Size = new System.Drawing.Size(110, 18);
            this.cbx_ENAutoJP.TabIndex = 138;
            this.cbx_ENAutoJP.Text = "启用自动纠偏";
            this.cbx_ENAutoJP.UseVisualStyleBackColor = true;
            // 
            // num_TJJP_Pro
            // 
            this.num_TJJP_Pro.DecimalPlaces = 2;
            this.num_TJJP_Pro.Font = new System.Drawing.Font("宋体", 9F);
            this.num_TJJP_Pro.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.num_TJJP_Pro.Location = new System.Drawing.Point(146, 102);
            this.num_TJJP_Pro.Margin = new System.Windows.Forms.Padding(4);
            this.num_TJJP_Pro.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.num_TJJP_Pro.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.num_TJJP_Pro.Name = "num_TJJP_Pro";
            this.num_TJJP_Pro.Size = new System.Drawing.Size(95, 21);
            this.num_TJJP_Pro.TabIndex = 152;
            this.num_TJJP_Pro.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Font = new System.Drawing.Font("宋体", 10.5F);
            this.label22.Location = new System.Drawing.Point(16, 107);
            this.label22.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(105, 14);
            this.label22.TabIndex = 151;
            this.label22.Text = "纠偏比例系数：";
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Font = new System.Drawing.Font("宋体", 10.5F);
            this.label34.Location = new System.Drawing.Point(15, 66);
            this.label34.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(105, 14);
            this.label34.TabIndex = 3;
            this.label34.Text = "采样距离(mm)：";
            // 
            // num_TJDistance
            // 
            this.num_TJDistance.Font = new System.Drawing.Font("宋体", 9F);
            this.num_TJDistance.Location = new System.Drawing.Point(146, 61);
            this.num_TJDistance.Margin = new System.Windows.Forms.Padding(4);
            this.num_TJDistance.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.num_TJDistance.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.num_TJDistance.Name = "num_TJDistance";
            this.num_TJDistance.Size = new System.Drawing.Size(95, 21);
            this.num_TJDistance.TabIndex = 69;
            this.num_TJDistance.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // num_TJJPSign_LX_num
            // 
            this.num_TJJPSign_LX_num.Font = new System.Drawing.Font("宋体", 9F);
            this.num_TJJPSign_LX_num.Location = new System.Drawing.Point(181, 123);
            this.num_TJJPSign_LX_num.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.num_TJJPSign_LX_num.Name = "num_TJJPSign_LX_num";
            this.num_TJJPSign_LX_num.Size = new System.Drawing.Size(49, 21);
            this.num_TJJPSign_LX_num.TabIndex = 196;
            this.num_TJJPSign_LX_num.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.cbx_TJJPSign_LX_En);
            this.groupBox6.Controls.Add(this.cbx_maxBJTJEn);
            this.groupBox6.Controls.Add(this.num_maxBJTJNum);
            this.groupBox6.Controls.Add(this.num_TJJPSign_LX_num);
            this.groupBox6.Controls.Add(this.label30);
            this.groupBox6.Controls.Add(this.label28);
            this.groupBox6.Controls.Add(this.num_maxTJJPnum);
            this.groupBox6.Controls.Add(this.num_minTJJPnum);
            this.groupBox6.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox6.Location = new System.Drawing.Point(715, 174);
            this.groupBox6.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox6.Size = new System.Drawing.Size(304, 397);
            this.groupBox6.TabIndex = 160;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "纠偏异常设置";
            // 
            // cbx_TJJPSign_LX_En
            // 
            this.cbx_TJJPSign_LX_En.AutoSize = true;
            this.cbx_TJJPSign_LX_En.Font = new System.Drawing.Font("宋体", 10.5F);
            this.cbx_TJJPSign_LX_En.Location = new System.Drawing.Point(15, 126);
            this.cbx_TJJPSign_LX_En.Margin = new System.Windows.Forms.Padding(4);
            this.cbx_TJJPSign_LX_En.Name = "cbx_TJJPSign_LX_En";
            this.cbx_TJJPSign_LX_En.Size = new System.Drawing.Size(152, 18);
            this.cbx_TJJPSign_LX_En.TabIndex = 201;
            this.cbx_TJJPSign_LX_En.Text = "纠偏同符号报警数量";
            this.cbx_TJJPSign_LX_En.UseVisualStyleBackColor = true;
            // 
            // cbx_maxBJTJEn
            // 
            this.cbx_maxBJTJEn.AutoSize = true;
            this.cbx_maxBJTJEn.Font = new System.Drawing.Font("宋体", 10.5F);
            this.cbx_maxBJTJEn.Location = new System.Drawing.Point(15, 93);
            this.cbx_maxBJTJEn.Margin = new System.Windows.Forms.Padding(4);
            this.cbx_maxBJTJEn.Name = "cbx_maxBJTJEn";
            this.cbx_maxBJTJEn.Size = new System.Drawing.Size(138, 18);
            this.cbx_maxBJTJEn.TabIndex = 200;
            this.cbx_maxBJTJEn.Text = "超最大值报警数量";
            this.cbx_maxBJTJEn.UseVisualStyleBackColor = true;
            // 
            // num_maxBJTJNum
            // 
            this.num_maxBJTJNum.Font = new System.Drawing.Font("宋体", 9F);
            this.num_maxBJTJNum.Location = new System.Drawing.Point(181, 90);
            this.num_maxBJTJNum.Margin = new System.Windows.Forms.Padding(4);
            this.num_maxBJTJNum.Name = "num_maxBJTJNum";
            this.num_maxBJTJNum.Size = new System.Drawing.Size(49, 21);
            this.num_maxBJTJNum.TabIndex = 137;
            this.num_maxBJTJNum.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Font = new System.Drawing.Font("宋体", 10.5F);
            this.label30.Location = new System.Drawing.Point(12, 30);
            this.label30.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(91, 14);
            this.label30.TabIndex = 130;
            this.label30.Text = "最大值(mm)：";
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Font = new System.Drawing.Font("宋体", 10.5F);
            this.label28.Location = new System.Drawing.Point(12, 59);
            this.label28.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(91, 14);
            this.label28.TabIndex = 134;
            this.label28.Text = "最小值(mm)：";
            // 
            // num_maxTJJPnum
            // 
            this.num_maxTJJPnum.DecimalPlaces = 1;
            this.num_maxTJJPnum.Font = new System.Drawing.Font("宋体", 9F);
            this.num_maxTJJPnum.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.num_maxTJJPnum.Location = new System.Drawing.Point(155, 28);
            this.num_maxTJJPnum.Margin = new System.Windows.Forms.Padding(4);
            this.num_maxTJJPnum.Name = "num_maxTJJPnum";
            this.num_maxTJJPnum.Size = new System.Drawing.Size(74, 21);
            this.num_maxTJJPnum.TabIndex = 131;
            this.num_maxTJJPnum.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            // 
            // num_minTJJPnum
            // 
            this.num_minTJJPnum.DecimalPlaces = 2;
            this.num_minTJJPnum.Font = new System.Drawing.Font("宋体", 9F);
            this.num_minTJJPnum.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.num_minTJJPnum.Location = new System.Drawing.Point(155, 59);
            this.num_minTJJPnum.Margin = new System.Windows.Forms.Padding(4);
            this.num_minTJJPnum.Name = "num_minTJJPnum";
            this.num_minTJJPnum.Size = new System.Drawing.Size(74, 21);
            this.num_minTJJPnum.TabIndex = 133;
            this.num_minTJJPnum.Value = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.cbx_CorrectLogic);
            this.groupBox5.Controls.Add(this.label7);
            this.groupBox5.Controls.Add(this.cbx_JP_Sign);
            this.groupBox5.Controls.Add(this.cbx_ENAutoJP);
            this.groupBox5.Controls.Add(this.num_TJJP_Pro);
            this.groupBox5.Controls.Add(this.label22);
            this.groupBox5.Controls.Add(this.label34);
            this.groupBox5.Controls.Add(this.num_TJDistance);
            this.groupBox5.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox5.Location = new System.Drawing.Point(20, 174);
            this.groupBox5.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox5.Size = new System.Drawing.Size(663, 180);
            this.groupBox5.TabIndex = 161;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "纠偏参数";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("宋体", 10.5F);
            this.label7.Location = new System.Drawing.Point(16, 148);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(77, 14);
            this.label7.TabIndex = 175;
            this.label7.Text = "纠偏逻辑：";
            // 
            // cbx_CorrectLogic
            // 
            this.cbx_CorrectLogic.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbx_CorrectLogic.FormattingEnabled = true;
            this.cbx_CorrectLogic.Location = new System.Drawing.Point(146, 143);
            this.cbx_CorrectLogic.Name = "cbx_CorrectLogic";
            this.cbx_CorrectLogic.Size = new System.Drawing.Size(348, 24);
            this.cbx_CorrectLogic.TabIndex = 177;
            // 
            // Form_BTSCorrect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkSeaGreen;
            this.ClientSize = new System.Drawing.Size(1041, 598);
            this.Controls.Add(this.groupBox_自动开启纠偏);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btn_Save);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox5);
            this.Name = "Form_BTSCorrect";
            this.Text = "Form_BTSCorrect";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_BTSCorrect_FormClosing);
            this.Load += new System.EventHandler(this.Form_BTSCorrect_Load);
            ((System.ComponentModel.ISupportInitialize)(this.tbxLocalMachinePort)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbxELMachinePort)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_Correctdata)).EndInit();
            this.groupBox_自动开启纠偏.ResumeLayout(false);
            this.groupBox_自动开启纠偏.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_JP_HJLength)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_TJJP_Pro)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_TJDistance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_TJJPSign_LX_num)).EndInit();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_maxBJTJNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_maxTJJPnum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_minTJJPnum)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cbx_JPType;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button btnELConnect;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown tbxLocalMachinePort;
        private System.Windows.Forms.NumericUpDown tbxELMachinePort;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TextBox tbxLocalMachineIP;
        private System.Windows.Forms.TextBox tbxELMachineIP;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Button btnHZJP;
        private System.Windows.Forms.NumericUpDown num_Correctdata;
        private System.Windows.Forms.GroupBox groupBox_自动开启纠偏;
        private System.Windows.Forms.NumericUpDown num_JP_HJLength;
        private System.Windows.Forms.CheckBox cb_EnHJJP;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnManuJP;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btn_Save;
        private System.Windows.Forms.CheckBox cbx_JP_Sign;
        private System.Windows.Forms.CheckBox cbx_ENAutoJP;
        private System.Windows.Forms.NumericUpDown num_TJJP_Pro;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.NumericUpDown num_TJDistance;
        private System.Windows.Forms.NumericUpDown num_TJJPSign_LX_num;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.NumericUpDown num_maxBJTJNum;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.NumericUpDown num_maxTJJPnum;
        private System.Windows.Forms.NumericUpDown num_minTJJPnum;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox cbx_maxBJTJEn;
        private System.Windows.Forms.CheckBox cbx_TJJPSign_LX_En;
        private ControlComboBox cbx_CorrectLogic;
    }
}
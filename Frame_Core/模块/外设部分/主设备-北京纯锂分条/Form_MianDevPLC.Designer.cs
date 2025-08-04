namespace Frame_Core
{
    partial class Form_MianDevPLC
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
            this.btn_Save = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.num_DevPort = new System.Windows.Forms.NumericUpDown();
            this.label58 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tbx_Dev_IP = new System.Windows.Forms.TextBox();
            this.tbx_Local_IP = new System.Windows.Forms.TextBox();
            this.btn_connectPLC = new System.Windows.Forms.Button();
            this.cbx_IsConnect = new System.Windows.Forms.CheckBox();
            this.cbx_ComType = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.nud_CorrentSleep = new System.Windows.Forms.NumericUpDown();
            this.btn_Encoder = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.nud_EncoderPulse = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.nud_EncoderDiameter = new System.Windows.Forms.NumericUpDown();
            this.groupBox_自动开启纠偏 = new System.Windows.Forms.GroupBox();
            this.num_JP_HJLength = new System.Windows.Forms.NumericUpDown();
            this.cb_EnHJJP = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label18 = new System.Windows.Forms.Label();
            this.btnHZJP = new System.Windows.Forms.Button();
            this.btnManuJP = new System.Windows.Forms.Button();
            this.num_Correctdata = new System.Windows.Forms.NumericUpDown();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.cbx_TJJPSign_LX_En = new System.Windows.Forms.CheckBox();
            this.cbx_maxBJTJEn = new System.Windows.Forms.CheckBox();
            this.num_maxBJTJNum = new System.Windows.Forms.NumericUpDown();
            this.num_TJJPSign_LX_num = new System.Windows.Forms.NumericUpDown();
            this.label30 = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.num_maxTJJPnum = new System.Windows.Forms.NumericUpDown();
            this.num_minTJJPnum = new System.Windows.Forms.NumericUpDown();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.cbx_JP_Sign = new System.Windows.Forms.CheckBox();
            this.cbx_ENAutoJP = new System.Windows.Forms.CheckBox();
            this.num_TJJP_Pro = new System.Windows.Forms.NumericUpDown();
            this.label22 = new System.Windows.Forms.Label();
            this.label34 = new System.Windows.Forms.Label();
            this.num_TJDistance = new System.Windows.Forms.NumericUpDown();
            this.cbx_CorrectLogic = new Frame_Core.ControlComboBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_DevPort)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_CorrentSleep)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_EncoderPulse)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_EncoderDiameter)).BeginInit();
            this.groupBox_自动开启纠偏.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_JP_HJLength)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_Correctdata)).BeginInit();
            this.groupBox6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_maxBJTJNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_TJJPSign_LX_num)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_maxTJJPnum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_minTJJPnum)).BeginInit();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_TJJP_Pro)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_TJDistance)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_Save
            // 
            this.btn_Save.Location = new System.Drawing.Point(890, 39);
            this.btn_Save.Margin = new System.Windows.Forms.Padding(5);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(140, 82);
            this.btn_Save.TabIndex = 115;
            this.btn_Save.Text = "保存参数";
            this.btn_Save.UseVisualStyleBackColor = true;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.num_DevPort);
            this.groupBox1.Controls.Add(this.label58);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.tbx_Dev_IP);
            this.groupBox1.Controls.Add(this.tbx_Local_IP);
            this.groupBox1.Controls.Add(this.btn_connectPLC);
            this.groupBox1.Controls.Add(this.cbx_IsConnect);
            this.groupBox1.Controls.Add(this.cbx_ComType);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Location = new System.Drawing.Point(21, 21);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(830, 133);
            this.groupBox1.TabIndex = 114;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "连接设置";
            // 
            // num_DevPort
            // 
            this.num_DevPort.Location = new System.Drawing.Point(721, 78);
            this.num_DevPort.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.num_DevPort.Name = "num_DevPort";
            this.num_DevPort.Size = new System.Drawing.Size(87, 21);
            this.num_DevPort.TabIndex = 112;
            // 
            // label58
            // 
            this.label58.AutoSize = true;
            this.label58.Font = new System.Drawing.Font("宋体", 10.5F);
            this.label58.Location = new System.Drawing.Point(623, 82);
            this.label58.Name = "label58";
            this.label58.Size = new System.Drawing.Size(91, 14);
            this.label58.TabIndex = 111;
            this.label58.Text = "设备端口号：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 10.5F);
            this.label1.Location = new System.Drawing.Point(333, 82);
            this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 14);
            this.label1.TabIndex = 106;
            this.label1.Text = "设备IP：";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("宋体", 10.5F);
            this.label5.Location = new System.Drawing.Point(45, 82);
            this.label5.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 14);
            this.label5.TabIndex = 110;
            this.label5.Text = "电脑端IP：";
            // 
            // tbx_Dev_IP
            // 
            this.tbx_Dev_IP.Location = new System.Drawing.Point(406, 78);
            this.tbx_Dev_IP.Margin = new System.Windows.Forms.Padding(5);
            this.tbx_Dev_IP.Name = "tbx_Dev_IP";
            this.tbx_Dev_IP.Size = new System.Drawing.Size(147, 21);
            this.tbx_Dev_IP.TabIndex = 107;
            this.tbx_Dev_IP.Text = "127.0.0.2";
            // 
            // tbx_Local_IP
            // 
            this.tbx_Local_IP.Location = new System.Drawing.Point(128, 78);
            this.tbx_Local_IP.Margin = new System.Windows.Forms.Padding(5);
            this.tbx_Local_IP.Name = "tbx_Local_IP";
            this.tbx_Local_IP.Size = new System.Drawing.Size(149, 21);
            this.tbx_Local_IP.TabIndex = 109;
            this.tbx_Local_IP.Text = "192.168.5.100";
            // 
            // btn_connectPLC
            // 
            this.btn_connectPLC.Location = new System.Drawing.Point(426, 22);
            this.btn_connectPLC.Margin = new System.Windows.Forms.Padding(5);
            this.btn_connectPLC.Name = "btn_connectPLC";
            this.btn_connectPLC.Size = new System.Drawing.Size(140, 39);
            this.btn_connectPLC.TabIndex = 108;
            this.btn_connectPLC.Text = "连接PLC";
            this.btn_connectPLC.UseVisualStyleBackColor = true;
            this.btn_connectPLC.Click += new System.EventHandler(this.btn_connectPLC_Click);
            // 
            // cbx_IsConnect
            // 
            this.cbx_IsConnect.AutoSize = true;
            this.cbx_IsConnect.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbx_IsConnect.Location = new System.Drawing.Point(336, 39);
            this.cbx_IsConnect.Name = "cbx_IsConnect";
            this.cbx_IsConnect.Size = new System.Drawing.Size(82, 18);
            this.cbx_IsConnect.TabIndex = 105;
            this.cbx_IsConnect.Text = "启用通讯";
            this.cbx_IsConnect.UseVisualStyleBackColor = true;
            // 
            // cbx_ComType
            // 
            this.cbx_ComType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbx_ComType.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbx_ComType.FormattingEnabled = true;
            this.cbx_ComType.Location = new System.Drawing.Point(128, 35);
            this.cbx_ComType.Name = "cbx_ComType";
            this.cbx_ComType.Size = new System.Drawing.Size(151, 22);
            this.cbx_ComType.TabIndex = 104;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("宋体", 10.5F);
            this.label9.Location = new System.Drawing.Point(45, 39);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(77, 14);
            this.label9.TabIndex = 103;
            this.label9.Text = "通讯选择：";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.nud_CorrentSleep);
            this.groupBox3.Controls.Add(this.btn_Encoder);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.nud_EncoderPulse);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.nud_EncoderDiameter);
            this.groupBox3.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold);
            this.groupBox3.Location = new System.Drawing.Point(21, 171);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(999, 101);
            this.groupBox3.TabIndex = 170;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "其他参数";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("宋体", 10.5F);
            this.label6.Location = new System.Drawing.Point(333, 30);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(77, 14);
            this.label6.TabIndex = 159;
            this.label6.Text = "纠偏速度：";
            // 
            // nud_CorrentSleep
            // 
            this.nud_CorrentSleep.Font = new System.Drawing.Font("宋体", 9F);
            this.nud_CorrentSleep.Location = new System.Drawing.Point(429, 26);
            this.nud_CorrentSleep.Margin = new System.Windows.Forms.Padding(4);
            this.nud_CorrentSleep.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.nud_CorrentSleep.Name = "nud_CorrentSleep";
            this.nud_CorrentSleep.Size = new System.Drawing.Size(95, 21);
            this.nud_CorrentSleep.TabIndex = 158;
            this.nud_CorrentSleep.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // btn_Encoder
            // 
            this.btn_Encoder.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn_Encoder.Location = new System.Drawing.Point(623, 26);
            this.btn_Encoder.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.btn_Encoder.Name = "btn_Encoder";
            this.btn_Encoder.Size = new System.Drawing.Size(175, 48);
            this.btn_Encoder.TabIndex = 156;
            this.btn_Encoder.Text = "编码轮压下或抬起";
            this.btn_Encoder.UseVisualStyleBackColor = true;
            this.btn_Encoder.Click += new System.EventHandler(this.btn_Encoder_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 10.5F);
            this.label4.Location = new System.Drawing.Point(16, 64);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(91, 14);
            this.label4.TabIndex = 157;
            this.label4.Text = "编码器脉冲：";
            // 
            // nud_EncoderPulse
            // 
            this.nud_EncoderPulse.Font = new System.Drawing.Font("宋体", 9F);
            this.nud_EncoderPulse.Location = new System.Drawing.Point(152, 61);
            this.nud_EncoderPulse.Margin = new System.Windows.Forms.Padding(4);
            this.nud_EncoderPulse.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.nud_EncoderPulse.Name = "nud_EncoderPulse";
            this.nud_EncoderPulse.Size = new System.Drawing.Size(95, 21);
            this.nud_EncoderPulse.TabIndex = 156;
            this.nud_EncoderPulse.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 10.5F);
            this.label3.Location = new System.Drawing.Point(16, 36);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(119, 14);
            this.label3.TabIndex = 155;
            this.label3.Text = "编码轮直径(mm)：";
            // 
            // nud_EncoderDiameter
            // 
            this.nud_EncoderDiameter.Font = new System.Drawing.Font("宋体", 9F);
            this.nud_EncoderDiameter.Location = new System.Drawing.Point(152, 33);
            this.nud_EncoderDiameter.Margin = new System.Windows.Forms.Padding(4);
            this.nud_EncoderDiameter.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.nud_EncoderDiameter.Name = "nud_EncoderDiameter";
            this.nud_EncoderDiameter.Size = new System.Drawing.Size(95, 21);
            this.nud_EncoderDiameter.TabIndex = 70;
            this.nud_EncoderDiameter.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // groupBox_自动开启纠偏
            // 
            this.groupBox_自动开启纠偏.Controls.Add(this.num_JP_HJLength);
            this.groupBox_自动开启纠偏.Controls.Add(this.cb_EnHJJP);
            this.groupBox_自动开启纠偏.Controls.Add(this.label2);
            this.groupBox_自动开启纠偏.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox_自动开启纠偏.Location = new System.Drawing.Point(21, 629);
            this.groupBox_自动开启纠偏.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox_自动开启纠偏.Name = "groupBox_自动开启纠偏";
            this.groupBox_自动开启纠偏.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox_自动开启纠偏.Size = new System.Drawing.Size(663, 69);
            this.groupBox_自动开启纠偏.TabIndex = 174;
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
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(14, 32);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(183, 16);
            this.label2.TabIndex = 157;
            this.label2.Text = "换卷N米开启自动纠偏(m)";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label18);
            this.groupBox2.Controls.Add(this.btnHZJP);
            this.groupBox2.Controls.Add(this.btnManuJP);
            this.groupBox2.Controls.Add(this.num_Correctdata);
            this.groupBox2.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox2.Location = new System.Drawing.Point(21, 496);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox2.Size = new System.Drawing.Size(662, 107);
            this.groupBox2.TabIndex = 173;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "纠偏控制";
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
            this.btnHZJP.Click += new System.EventHandler(this.btnHZJP_Click);
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
            this.groupBox6.Location = new System.Drawing.Point(716, 301);
            this.groupBox6.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox6.Size = new System.Drawing.Size(304, 397);
            this.groupBox6.TabIndex = 171;
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
            this.groupBox5.Location = new System.Drawing.Point(21, 301);
            this.groupBox5.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox5.Size = new System.Drawing.Size(663, 180);
            this.groupBox5.TabIndex = 172;
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
            // cbx_CorrectLogic
            // 
            this.cbx_CorrectLogic.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbx_CorrectLogic.FormattingEnabled = true;
            this.cbx_CorrectLogic.Location = new System.Drawing.Point(146, 138);
            this.cbx_CorrectLogic.Name = "cbx_CorrectLogic";
            this.cbx_CorrectLogic.Size = new System.Drawing.Size(348, 24);
            this.cbx_CorrectLogic.TabIndex = 176;
            // 
            // Form_MianDevPLC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkSeaGreen;
            this.ClientSize = new System.Drawing.Size(1057, 724);
            this.Controls.Add(this.groupBox_自动开启纠偏);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.btn_Save);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form_MianDevPLC";
            this.Text = "Form_MianDevPLC";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_MianDevPLC_FormClosing);
            this.Load += new System.EventHandler(this.Form_MianDevPLC_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_DevPort)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_CorrentSleep)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_EncoderPulse)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_EncoderDiameter)).EndInit();
            this.groupBox_自动开启纠偏.ResumeLayout(false);
            this.groupBox_自动开启纠偏.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_JP_HJLength)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_Correctdata)).EndInit();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_maxBJTJNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_TJJPSign_LX_num)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_maxTJJPnum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_minTJJPnum)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_TJJP_Pro)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_TJDistance)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_Save;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown num_DevPort;
        private System.Windows.Forms.Label label58;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbx_Dev_IP;
        private System.Windows.Forms.TextBox tbx_Local_IP;
        private System.Windows.Forms.Button btn_connectPLC;
        private System.Windows.Forms.CheckBox cbx_IsConnect;
        private System.Windows.Forms.ComboBox cbx_ComType;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox_自动开启纠偏;
        private System.Windows.Forms.NumericUpDown num_JP_HJLength;
        private System.Windows.Forms.CheckBox cb_EnHJJP;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Button btnHZJP;
        private System.Windows.Forms.Button btnManuJP;
        private System.Windows.Forms.NumericUpDown num_Correctdata;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.CheckBox cbx_TJJPSign_LX_En;
        private System.Windows.Forms.CheckBox cbx_maxBJTJEn;
        private System.Windows.Forms.NumericUpDown num_maxBJTJNum;
        private System.Windows.Forms.NumericUpDown num_TJJPSign_LX_num;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.NumericUpDown num_maxTJJPnum;
        private System.Windows.Forms.NumericUpDown num_minTJJPnum;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox cbx_JP_Sign;
        private System.Windows.Forms.CheckBox cbx_ENAutoJP;
        private System.Windows.Forms.NumericUpDown num_TJJP_Pro;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.NumericUpDown num_TJDistance;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown nud_EncoderDiameter;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown nud_EncoderPulse;
        private System.Windows.Forms.Button btn_Encoder;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown nud_CorrentSleep;
        private ControlComboBox cbx_CorrectLogic;
    }
}
namespace Frame_Core
{
    partial class Cam_Form
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
            this.save_cam_parmeter = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.CameraName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CameraType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.CardSerial = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CameraConfig = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CameraPort = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ImageHeight = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ImageWidth = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CalibrationValue_X = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CalibrationValue_Y = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ToMainCamDistance = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // save_cam_parmeter
            // 
            this.save_cam_parmeter.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.save_cam_parmeter.Location = new System.Drawing.Point(1301, 9);
            this.save_cam_parmeter.Name = "save_cam_parmeter";
            this.save_cam_parmeter.Size = new System.Drawing.Size(107, 37);
            this.save_cam_parmeter.TabIndex = 103;
            this.save_cam_parmeter.Text = "保存设置";
            this.save_cam_parmeter.UseVisualStyleBackColor = true;
            this.save_cam_parmeter.Click += new System.EventHandler(this.save_cam_parmeter_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.dataGridView1);
            this.groupBox1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox1.Location = new System.Drawing.Point(14, 71);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(133, 133, 133, 53);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(13);
            this.groupBox1.Size = new System.Drawing.Size(1407, 393);
            this.groupBox1.TabIndex = 104;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "相机设置";
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
            this.CameraName,
            this.CameraType,
            this.CardSerial,
            this.CameraConfig,
            this.CameraPort,
            this.ImageHeight,
            this.ImageWidth,
            this.CalibrationValue_X,
            this.CalibrationValue_Y,
            this.ToMainCamDistance});
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
            this.dataGridView1.Size = new System.Drawing.Size(1381, 353);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(12, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(344, 12);
            this.label1.TabIndex = 105;
            this.label1.Text = "* 相机配置修改后需要重启程序才生效，禁止相机名称重复";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.ForeColor = System.Drawing.Color.Red;
            this.label2.Location = new System.Drawing.Point(12, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(266, 12);
            this.label2.TabIndex = 106;
            this.label2.Text = "* 修改相机名称会导致流程参数需要重新配置";
            // 
            // CameraName
            // 
            this.CameraName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.CameraName.FillWeight = 80F;
            this.CameraName.Frozen = true;
            this.CameraName.HeaderText = "相机名称";
            this.CameraName.MinimumWidth = 8;
            this.CameraName.Name = "CameraName";
            this.CameraName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.CameraName.Width = 120;
            // 
            // CameraType
            // 
            this.CameraType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.CameraType.FillWeight = 90F;
            this.CameraType.Frozen = true;
            this.CameraType.HeaderText = "相机类型";
            this.CameraType.Items.AddRange(new object[] {
            "海康相机",
            "海康彩色相机",
            "灰点相机",
            "DALSCameraLink相机",
            "DALSCameraLink2相机",
            "DALSGigEVision相机",
            "埃科(采集卡或黑白相机)",
            "埃科(采集卡或彩色相机)",
            "Basler(面阵)",
            "埃科CXP黑白相机",
            "埃科CXP彩色相机",
            "海康U口黑白相机",
            "海康U口彩色相机",
            "LMI"});
            this.CameraType.MinimumWidth = 8;
            this.CameraType.Name = "CameraType";
            this.CameraType.Width = 150;
            // 
            // CardSerial
            // 
            this.CardSerial.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.CardSerial.FillWeight = 90F;
            this.CardSerial.Frozen = true;
            this.CardSerial.HeaderText = "序列号/采集卡型号";
            this.CardSerial.MinimumWidth = 8;
            this.CardSerial.Name = "CardSerial";
            this.CardSerial.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.CardSerial.Width = 150;
            // 
            // CameraConfig
            // 
            this.CameraConfig.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.CameraConfig.FillWeight = 120F;
            this.CameraConfig.Frozen = true;
            this.CameraConfig.HeaderText = "相机IP/CCF路径";
            this.CameraConfig.MinimumWidth = 8;
            this.CameraConfig.Name = "CameraConfig";
            this.CameraConfig.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.CameraConfig.Width = 300;
            // 
            // CameraPort
            // 
            this.CameraPort.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.CameraPort.FillWeight = 80F;
            this.CameraPort.Frozen = true;
            this.CameraPort.HeaderText = "本地IP/采集卡端口";
            this.CameraPort.MinimumWidth = 8;
            this.CameraPort.Name = "CameraPort";
            this.CameraPort.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.CameraPort.Width = 150;
            // 
            // ImageHeight
            // 
            this.ImageHeight.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.ImageHeight.HeaderText = "图像高度";
            this.ImageHeight.Name = "ImageHeight";
            this.ImageHeight.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ImageHeight.Width = 80;
            // 
            // ImageWidth
            // 
            this.ImageWidth.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.ImageWidth.HeaderText = "图像宽度";
            this.ImageWidth.Name = "ImageWidth";
            this.ImageWidth.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ImageWidth.Width = 80;
            // 
            // CalibrationValue_X
            // 
            this.CalibrationValue_X.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.CalibrationValue_X.HeaderText = "横向标定值";
            this.CalibrationValue_X.Name = "CalibrationValue_X";
            this.CalibrationValue_X.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.CalibrationValue_X.Width = 154;
            // 
            // CalibrationValue_Y
            // 
            this.CalibrationValue_Y.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.CalibrationValue_Y.HeaderText = "纵向标定值";
            this.CalibrationValue_Y.Name = "CalibrationValue_Y";
            this.CalibrationValue_Y.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.CalibrationValue_Y.Width = 154;
            // 
            // ToMainCamDistance
            // 
            this.ToMainCamDistance.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.ToMainCamDistance.HeaderText = "到主相机的距离";
            this.ToMainCamDistance.Name = "ToMainCamDistance";
            this.ToMainCamDistance.Width = 154;
            // 
            // Cam_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkSeaGreen;
            this.ClientSize = new System.Drawing.Size(1429, 476);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.save_cam_parmeter);
            this.MaximizeBox = false;
            this.Name = "Cam_Form";
            this.Text = "相机设置";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Cam_Form_FormClosing);
            this.Load += new System.EventHandler(this.Cam_Form_Load);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button save_cam_parmeter;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridViewTextBoxColumn CameraName;
        private System.Windows.Forms.DataGridViewComboBoxColumn CameraType;
        private System.Windows.Forms.DataGridViewTextBoxColumn CardSerial;
        private System.Windows.Forms.DataGridViewTextBoxColumn CameraConfig;
        private System.Windows.Forms.DataGridViewTextBoxColumn CameraPort;
        private System.Windows.Forms.DataGridViewTextBoxColumn ImageHeight;
        private System.Windows.Forms.DataGridViewTextBoxColumn ImageWidth;
        private System.Windows.Forms.DataGridViewTextBoxColumn CalibrationValue_X;
        private System.Windows.Forms.DataGridViewTextBoxColumn CalibrationValue_Y;
        private System.Windows.Forms.DataGridViewTextBoxColumn ToMainCamDistance;
    }
}
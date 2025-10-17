namespace Frame_Core
{
    partial class UCCameraCard
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
            this.lb_CamName = new System.Windows.Forms.Label();
            this.lb_SizePointInspection = new System.Windows.Forms.Label();
            this.lb_SizePointInspectionTime = new System.Windows.Forms.Label();
            this.lb_SizePointInspectionState = new System.Windows.Forms.Label();
            this.btn_SizePointInspectionDebug = new System.Windows.Forms.Button();
            this.lb_DefectPointInspectionTime = new System.Windows.Forms.Label();
            this.lb_DefectPointInspectionState = new System.Windows.Forms.Label();
            this.lb_DefectPointInspection = new System.Windows.Forms.Label();
            this.btn_DefectPointInspectionDebug = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.OutsetDouble;
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 21.03251F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.07839F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20.65009F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Controls.Add(this.lb_DefectPointInspection, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.lb_CamName, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.lb_SizePointInspection, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.lb_SizePointInspectionTime, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.lb_SizePointInspectionState, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.btn_SizePointInspectionDebug, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.btn_DefectPointInspectionDebug, 3, 2);
            this.tableLayoutPanel1.Controls.Add(this.lb_DefectPointInspectionTime, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.lb_DefectPointInspectionState, 2, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.00001F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(523, 175);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // lb_CamName
            // 
            this.lb_CamName.AutoSize = true;
            this.lb_CamName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.tableLayoutPanel1.SetColumnSpan(this.lb_CamName, 4);
            this.lb_CamName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lb_CamName.Location = new System.Drawing.Point(6, 6);
            this.lb_CamName.Margin = new System.Windows.Forms.Padding(3);
            this.lb_CamName.Name = "lb_CamName";
            this.lb_CamName.Size = new System.Drawing.Size(511, 32);
            this.lb_CamName.TabIndex = 3;
            this.lb_CamName.Text = "相机名称";
            this.lb_CamName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lb_SizePointInspection
            // 
            this.lb_SizePointInspection.AutoSize = true;
            this.lb_SizePointInspection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lb_SizePointInspection.Location = new System.Drawing.Point(6, 47);
            this.lb_SizePointInspection.Margin = new System.Windows.Forms.Padding(3);
            this.lb_SizePointInspection.Name = "lb_SizePointInspection";
            this.lb_SizePointInspection.Size = new System.Drawing.Size(101, 56);
            this.lb_SizePointInspection.TabIndex = 0;
            this.lb_SizePointInspection.Text = "尺寸点检";
            this.lb_SizePointInspection.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lb_SizePointInspectionTime
            // 
            this.lb_SizePointInspectionTime.AutoSize = true;
            this.lb_SizePointInspectionTime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lb_SizePointInspectionTime.Location = new System.Drawing.Point(116, 47);
            this.lb_SizePointInspectionTime.Margin = new System.Windows.Forms.Padding(3);
            this.lb_SizePointInspectionTime.Name = "lb_SizePointInspectionTime";
            this.lb_SizePointInspectionTime.Size = new System.Drawing.Size(162, 56);
            this.lb_SizePointInspectionTime.TabIndex = 1;
            this.lb_SizePointInspectionTime.Text = "点检时间";
            this.lb_SizePointInspectionTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lb_SizePointInspectionState
            // 
            this.lb_SizePointInspectionState.AutoSize = true;
            this.lb_SizePointInspectionState.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lb_SizePointInspectionState.Location = new System.Drawing.Point(287, 47);
            this.lb_SizePointInspectionState.Margin = new System.Windows.Forms.Padding(3);
            this.lb_SizePointInspectionState.Name = "lb_SizePointInspectionState";
            this.lb_SizePointInspectionState.Size = new System.Drawing.Size(99, 56);
            this.lb_SizePointInspectionState.TabIndex = 2;
            this.lb_SizePointInspectionState.Text = "点检状态";
            this.lb_SizePointInspectionState.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btn_SizePointInspectionDebug
            // 
            this.btn_SizePointInspectionDebug.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_SizePointInspectionDebug.Location = new System.Drawing.Point(395, 47);
            this.btn_SizePointInspectionDebug.Name = "btn_SizePointInspectionDebug";
            this.btn_SizePointInspectionDebug.Size = new System.Drawing.Size(122, 56);
            this.btn_SizePointInspectionDebug.TabIndex = 4;
            this.btn_SizePointInspectionDebug.Text = "点检调试";
            this.btn_SizePointInspectionDebug.UseVisualStyleBackColor = true;
            this.btn_SizePointInspectionDebug.Click += new System.EventHandler(this.btn_SizePointInspectionDebug_Click);
            // 
            // lb_DefectPointInspectionTime
            // 
            this.lb_DefectPointInspectionTime.AutoSize = true;
            this.lb_DefectPointInspectionTime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lb_DefectPointInspectionTime.Location = new System.Drawing.Point(116, 112);
            this.lb_DefectPointInspectionTime.Margin = new System.Windows.Forms.Padding(3);
            this.lb_DefectPointInspectionTime.Name = "lb_DefectPointInspectionTime";
            this.lb_DefectPointInspectionTime.Size = new System.Drawing.Size(162, 57);
            this.lb_DefectPointInspectionTime.TabIndex = 5;
            this.lb_DefectPointInspectionTime.Text = "点检时间";
            this.lb_DefectPointInspectionTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lb_DefectPointInspectionState
            // 
            this.lb_DefectPointInspectionState.AutoSize = true;
            this.lb_DefectPointInspectionState.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lb_DefectPointInspectionState.Location = new System.Drawing.Point(287, 112);
            this.lb_DefectPointInspectionState.Margin = new System.Windows.Forms.Padding(3);
            this.lb_DefectPointInspectionState.Name = "lb_DefectPointInspectionState";
            this.lb_DefectPointInspectionState.Size = new System.Drawing.Size(99, 57);
            this.lb_DefectPointInspectionState.TabIndex = 6;
            this.lb_DefectPointInspectionState.Text = "点检状态";
            this.lb_DefectPointInspectionState.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lb_DefectPointInspection
            // 
            this.lb_DefectPointInspection.AutoSize = true;
            this.lb_DefectPointInspection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lb_DefectPointInspection.Location = new System.Drawing.Point(6, 112);
            this.lb_DefectPointInspection.Margin = new System.Windows.Forms.Padding(3);
            this.lb_DefectPointInspection.Name = "lb_DefectPointInspection";
            this.lb_DefectPointInspection.Size = new System.Drawing.Size(101, 57);
            this.lb_DefectPointInspection.TabIndex = 7;
            this.lb_DefectPointInspection.Text = "缺陷点检";
            this.lb_DefectPointInspection.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btn_DefectPointInspectionDebug
            // 
            this.btn_DefectPointInspectionDebug.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_DefectPointInspectionDebug.Location = new System.Drawing.Point(395, 112);
            this.btn_DefectPointInspectionDebug.Name = "btn_DefectPointInspectionDebug";
            this.btn_DefectPointInspectionDebug.Size = new System.Drawing.Size(122, 57);
            this.btn_DefectPointInspectionDebug.TabIndex = 8;
            this.btn_DefectPointInspectionDebug.Text = "点检调试";
            this.btn_DefectPointInspectionDebug.UseVisualStyleBackColor = true;
            this.btn_DefectPointInspectionDebug.Click += new System.EventHandler(this.btn_DefectPointInspectionDebug_Click);
            // 
            // UCCameraCard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "UCCameraCard";
            this.Size = new System.Drawing.Size(523, 175);
            this.Load += new System.EventHandler(this.UCCameraCard_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        public System.Windows.Forms.Label lb_CamName;
        public System.Windows.Forms.Label lb_SizePointInspection;
        public System.Windows.Forms.Label lb_SizePointInspectionTime;
        public System.Windows.Forms.Label lb_SizePointInspectionState;
        public System.Windows.Forms.Button btn_SizePointInspectionDebug;
        public System.Windows.Forms.Label lb_DefectPointInspection;
        public System.Windows.Forms.Button btn_DefectPointInspectionDebug;
        public System.Windows.Forms.Label lb_DefectPointInspectionTime;
        public System.Windows.Forms.Label lb_DefectPointInspectionState;
    }
}

namespace YYSD.UICore.Control
{
    partial class YComboBox
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
            this.lblObj = new System.Windows.Forms.Label();
            this.comObj = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // lblObj
            // 
            this.lblObj.AutoSize = true;
            this.lblObj.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblObj.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblObj.Location = new System.Drawing.Point(0, 0);
            this.lblObj.Name = "lblObj";
            this.lblObj.Size = new System.Drawing.Size(49, 19);
            this.lblObj.TabIndex = 0;
            this.lblObj.Text = "Name";
            // 
            // comObj
            // 
            this.comObj.Dock = System.Windows.Forms.DockStyle.Right;
            this.comObj.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.comObj.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.comObj.FormattingEnabled = true;
            this.comObj.Location = new System.Drawing.Point(56, 0);
            this.comObj.Name = "comObj";
            this.comObj.Size = new System.Drawing.Size(107, 22);
            this.comObj.TabIndex = 1;
            // 
            // YComboBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.comObj);
            this.Controls.Add(this.lblObj);
            this.Name = "YComboBox";
            this.Size = new System.Drawing.Size(163, 23);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblObj;
        private System.Windows.Forms.ComboBox comObj;
    }
}

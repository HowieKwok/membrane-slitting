namespace YYSD.UICore.Control
{
    partial class YNumericUpDown
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
            this.numObj = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.numObj)).BeginInit();
            this.SuspendLayout();
            // 
            // lblObj
            // 
            this.lblObj.AutoSize = true;
            this.lblObj.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblObj.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblObj.Location = new System.Drawing.Point(0, 0);
            this.lblObj.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblObj.Name = "lblObj";
            this.lblObj.Size = new System.Drawing.Size(58, 24);
            this.lblObj.TabIndex = 1;
            this.lblObj.Text = "Name";
            // 
            // numObj
            // 
            this.numObj.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.numObj.Dock = System.Windows.Forms.DockStyle.Right;
            this.numObj.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.numObj.Location = new System.Drawing.Point(77, 0);
            this.numObj.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.numObj.Name = "numObj";
            this.numObj.Size = new System.Drawing.Size(111, 26);
            this.numObj.TabIndex = 2;
            // 
            // YNumericUpDown
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.numObj);
            this.Controls.Add(this.lblObj);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "YNumericUpDown";
            this.Size = new System.Drawing.Size(188, 31);
            ((System.ComponentModel.ISupportInitialize)(this.numObj)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblObj;
        private System.Windows.Forms.NumericUpDown numObj;
    }
}

namespace Mef_VisionFrame
{
    partial class UC_StateView
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
            this._content = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // _content
            // 
            this._content.Dock = System.Windows.Forms.DockStyle.Fill;
            this._content.Location = new System.Drawing.Point(0, 0);
            this._content.Margin = new System.Windows.Forms.Padding(0);
            this._content.Name = "_content";
            this._content.Size = new System.Drawing.Size(706, 31);
            this._content.TabIndex = 0;
            // 
            // UC_StateView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._content);
            this.Name = "UC_StateView";
            this.Size = new System.Drawing.Size(706, 31);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel _content;
    }
}

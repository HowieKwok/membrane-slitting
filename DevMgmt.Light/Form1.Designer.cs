namespace DevMgmt.Light
{
    partial class Form1
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

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.btn_Light = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btn_Light
            // 
            this.btn_Light.Location = new System.Drawing.Point(78, 58);
            this.btn_Light.Name = "btn_Light";
            this.btn_Light.Size = new System.Drawing.Size(75, 23);
            this.btn_Light.TabIndex = 0;
            this.btn_Light.Text = "光源控制器管理";
            this.btn_Light.UseVisualStyleBackColor = true;
            this.btn_Light.Click += new System.EventHandler(this.btn_Light_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btn_Light);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_Light;
    }
}


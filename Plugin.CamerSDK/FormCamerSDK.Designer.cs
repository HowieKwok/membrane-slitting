namespace Plugin.CamerSDK
{
    partial class FormCamerSDK
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
            this.Btn_LoadLoacalImage = new System.Windows.Forms.Button();
            this.hWindowControl1 = new HalconDotNet.HWindowControl();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Btn_LoadLoacalImage
            // 
            this.Btn_LoadLoacalImage.Location = new System.Drawing.Point(476, 50);
            this.Btn_LoadLoacalImage.Name = "Btn_LoadLoacalImage";
            this.Btn_LoadLoacalImage.Size = new System.Drawing.Size(110, 45);
            this.Btn_LoadLoacalImage.TabIndex = 15;
            this.Btn_LoadLoacalImage.Text = "加载本地图片";
            this.Btn_LoadLoacalImage.UseVisualStyleBackColor = true;
            this.Btn_LoadLoacalImage.Click += new System.EventHandler(this.Btn_LoadLoacalImage_Click);
            // 
            // hWindowControl1
            // 
            this.hWindowControl1.BackColor = System.Drawing.Color.Black;
            this.hWindowControl1.BorderColor = System.Drawing.Color.Black;
            this.hWindowControl1.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.hWindowControl1.Location = new System.Drawing.Point(42, 26);
            this.hWindowControl1.Name = "hWindowControl1";
            this.hWindowControl1.Size = new System.Drawing.Size(366, 541);
            this.hWindowControl1.TabIndex = 16;
            this.hWindowControl1.WindowSize = new System.Drawing.Size(366, 541);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(622, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 17;
            this.label1.Text = "图片名：";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(476, 139);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(110, 45);
            this.button1.TabIndex = 18;
            this.button1.Text = "刷新图片";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // FormCamerSDK
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(1008, 811);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.hWindowControl1);
            this.Controls.Add(this.Btn_LoadLoacalImage);
            this.Name = "FormCamerSDK";
            this.Text = "FormCamerSDK";
            this.Controls.SetChildIndex(this.Btn_LoadLoacalImage, 0);
            this.Controls.SetChildIndex(this.hWindowControl1, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.button1, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Btn_LoadLoacalImage;
        private HalconDotNet.HWindowControl hWindowControl1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
    }
}
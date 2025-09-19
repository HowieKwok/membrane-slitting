namespace Mef_VisionFrame
{
    partial class Form_ImageManage
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
            this.imageToolPanel1 = new EHolly.ImageTool.ImageToolPanel();
            this.SuspendLayout();
            // 
            // imageToolPanel1
            // 
            this.imageToolPanel1.BackColor = System.Drawing.Color.DarkSeaGreen;
            this.imageToolPanel1.Location = new System.Drawing.Point(33, -9);
            this.imageToolPanel1.Name = "imageToolPanel1";
            this.imageToolPanel1.Size = new System.Drawing.Size(734, 469);
            this.imageToolPanel1.TabIndex = 1;
            // 
            // Form_ImageManage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 573);
            this.Controls.Add(this.imageToolPanel1);
            this.Name = "Form_ImageManage";
            this.Text = "Form_ImageManage";
            this.Load += new System.EventHandler(this.Form_ImageManage_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private EHolly.ImageTool.ImageToolPanel imageToolPanel1;
    }
}
namespace Frame_Core
{
    partial class PurviewForm
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
            this.dGV_Account = new System.Windows.Forms.DataGridView();
            this.Col_Type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Col_EngAccount = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Col_TecAccount = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Col_OptAccount = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dGV_Account)).BeginInit();
            this.SuspendLayout();
            // 
            // dGV_Account
            // 
            this.dGV_Account.AllowUserToAddRows = false;
            this.dGV_Account.AllowUserToDeleteRows = false;
            this.dGV_Account.AllowUserToResizeColumns = false;
            this.dGV_Account.AllowUserToResizeRows = false;
            this.dGV_Account.BackgroundColor = System.Drawing.SystemColors.ActiveCaption;
            this.dGV_Account.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dGV_Account.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Col_Type,
            this.Col_EngAccount,
            this.Col_TecAccount,
            this.Col_OptAccount});
            this.dGV_Account.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dGV_Account.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dGV_Account.Location = new System.Drawing.Point(0, 0);
            this.dGV_Account.Margin = new System.Windows.Forms.Padding(0);
            this.dGV_Account.MultiSelect = false;
            this.dGV_Account.Name = "dGV_Account";
            this.dGV_Account.RowHeadersVisible = false;
            this.dGV_Account.RowTemplate.Height = 23;
            this.dGV_Account.ShowEditingIcon = false;
            this.dGV_Account.Size = new System.Drawing.Size(1098, 383);
            this.dGV_Account.TabIndex = 1;
            // 
            // Col_Type
            // 
            this.Col_Type.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Col_Type.FillWeight = 110F;
            this.Col_Type.HeaderText = "权限类型";
            this.Col_Type.Name = "Col_Type";
            this.Col_Type.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Col_EngAccount
            // 
            this.Col_EngAccount.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Col_EngAccount.FillWeight = 105F;
            this.Col_EngAccount.HeaderText = "工程师";
            this.Col_EngAccount.Name = "Col_EngAccount";
            this.Col_EngAccount.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // Col_TecAccount
            // 
            this.Col_TecAccount.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Col_TecAccount.FillWeight = 105F;
            this.Col_TecAccount.HeaderText = "技术员";
            this.Col_TecAccount.Name = "Col_TecAccount";
            this.Col_TecAccount.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // Col_OptAccount
            // 
            this.Col_OptAccount.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Col_OptAccount.FillWeight = 105F;
            this.Col_OptAccount.HeaderText = "操作员";
            this.Col_OptAccount.Name = "Col_OptAccount";
            this.Col_OptAccount.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // PurviewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(1098, 383);
            this.Controls.Add(this.dGV_Account);
            this.Font = new System.Drawing.Font("宋体", 16F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PurviewForm";
            this.Text = "权限分配";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Purview_FormClosing);
            this.Load += new System.EventHandler(this.Purview_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dGV_Account)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dGV_Account;
        private System.Windows.Forms.DataGridViewTextBoxColumn Col_Type;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Col_EngAccount;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Col_TecAccount;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Col_OptAccount;
    }
}
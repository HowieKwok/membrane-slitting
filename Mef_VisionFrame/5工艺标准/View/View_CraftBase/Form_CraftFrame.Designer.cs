namespace Mef_VisionFrame
{
    partial class Form_CraftFrame
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
            this.components = new System.ComponentModel.Container();
            this.tableCrafts = new System.Windows.Forms.DataGridView();
            this.tableLayoutPanelRoot = new System.Windows.Forms.TableLayoutPanel();
            this.pageHeader = new AntdUI.PageHeader();
            this.panelLeft = new AntdUI.Panel();
            this.tableLayoutPanelLeft = new System.Windows.Forms.TableLayoutPanel();
            this.comb_materialCraf = new AntdUI.Select();
            this.inputSearch = new AntdUI.Input();
            this.labelCraftTime = new AntdUI.Label();
            this.btn_SaveCurrentCraft = new AntdUI.Button();
            this.tableLayoutPanelRight = new System.Windows.Forms.TableLayoutPanel();
            this.panel_Main = new AntdUI.Panel();
            this.toolTipCraftTime = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.tableCrafts)).BeginInit();
            this.tableLayoutPanelRoot.SuspendLayout();
            this.panelLeft.SuspendLayout();
            this.tableLayoutPanelLeft.SuspendLayout();
            this.tableLayoutPanelRight.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableCrafts
            // 
            this.tableCrafts.AllowUserToAddRows = false;
            this.tableCrafts.AllowUserToDeleteRows = false;
            this.tableCrafts.AllowUserToResizeRows = false;
            this.tableCrafts.BackgroundColor = System.Drawing.Color.White;
            this.tableCrafts.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tableCrafts.ColumnHeadersHeight = 38;
            this.tableCrafts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.tableCrafts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableCrafts.EnableHeadersVisualStyles = false;
            this.tableCrafts.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(231)))), ((int)(((byte)(235)))));
            this.tableCrafts.Location = new System.Drawing.Point(3, 117);
            this.tableCrafts.MultiSelect = false;
            this.tableCrafts.Name = "tableCrafts";
            this.tableCrafts.ReadOnly = true;
            this.tableCrafts.RowHeadersVisible = false;
            this.tableCrafts.RowHeadersWidth = 51;
            this.tableCrafts.RowTemplate.Height = 42;
            this.tableCrafts.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.tableCrafts.Size = new System.Drawing.Size(344, 525);
            this.tableCrafts.TabIndex = 3;
            this.tableCrafts.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.tableCrafts_CellDoubleClick);
            this.tableCrafts.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.tableCrafts_CellMouseClick);
            // 
            // tableLayoutPanelRoot
            // 
            this.tableLayoutPanelRoot.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.tableLayoutPanelRoot.ColumnCount = 2;
            this.tableLayoutPanelRoot.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 380F));
            this.tableLayoutPanelRoot.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelRoot.Controls.Add(this.pageHeader, 0, 0);
            this.tableLayoutPanelRoot.Controls.Add(this.panelLeft, 0, 1);
            this.tableLayoutPanelRoot.Controls.Add(this.tableLayoutPanelRight, 1, 1);
            this.tableLayoutPanelRoot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelRoot.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelRoot.Name = "tableLayoutPanelRoot";
            this.tableLayoutPanelRoot.Padding = new System.Windows.Forms.Padding(12);
            this.tableLayoutPanelRoot.RowCount = 2;
            this.tableLayoutPanelRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 72F));
            this.tableLayoutPanelRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelRoot.Size = new System.Drawing.Size(1362, 825);
            this.tableLayoutPanelRoot.TabIndex = 1;
            // 
            // pageHeader
            // 
            this.pageHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.tableLayoutPanelRoot.SetColumnSpan(this.pageHeader, 2);
            this.pageHeader.Description = "右键配方行进行新建、复制、重命名、删除和打开目录；双击配方切换生产配方";
            this.pageHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pageHeader.Location = new System.Drawing.Point(15, 15);
            this.pageHeader.Name = "pageHeader";
            this.pageHeader.Size = new System.Drawing.Size(1332, 66);
            this.pageHeader.SubText = "工艺配方加载、编辑、保存与生产切换";
            this.pageHeader.TabIndex = 0;
            this.pageHeader.Text = "工艺配方管理";
            // 
            // panelLeft
            // 
            this.panelLeft.Back = System.Drawing.Color.White;
            this.panelLeft.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(231)))), ((int)(((byte)(235)))));
            this.panelLeft.Controls.Add(this.tableLayoutPanelLeft);
            this.panelLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelLeft.Location = new System.Drawing.Point(15, 87);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Padding = new System.Windows.Forms.Padding(12);
            this.panelLeft.Radius = 8;
            this.panelLeft.Size = new System.Drawing.Size(374, 723);
            this.panelLeft.TabIndex = 1;
            // 
            // tableLayoutPanelLeft
            // 
            this.tableLayoutPanelLeft.ColumnCount = 1;
            this.tableLayoutPanelLeft.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelLeft.Controls.Add(this.comb_materialCraf, 0, 0);
            this.tableLayoutPanelLeft.Controls.Add(this.inputSearch, 0, 1);
            this.tableLayoutPanelLeft.Controls.Add(this.labelCraftTime, 0, 2);
            this.tableLayoutPanelLeft.Controls.Add(this.tableCrafts, 0, 3);
            this.tableLayoutPanelLeft.Controls.Add(this.btn_SaveCurrentCraft, 0, 4);
            this.tableLayoutPanelLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelLeft.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanelLeft.Name = "tableLayoutPanelLeft";
            this.tableLayoutPanelLeft.RowCount = 5;
            this.tableLayoutPanelLeft.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.tableLayoutPanelLeft.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.tableLayoutPanelLeft.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanelLeft.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelLeft.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 54F));
            this.tableLayoutPanelLeft.Size = new System.Drawing.Size(350, 699);
            this.tableLayoutPanelLeft.TabIndex = 0;
            // 
            // comb_materialCraf
            // 
            this.comb_materialCraf.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(231)))), ((int)(((byte)(235)))));
            this.comb_materialCraf.Dock = System.Windows.Forms.DockStyle.Fill;
            this.comb_materialCraf.List = true;
            this.comb_materialCraf.ListAutoWidth = true;
            this.comb_materialCraf.Location = new System.Drawing.Point(3, 3);
            this.comb_materialCraf.Name = "comb_materialCraf";
            this.comb_materialCraf.PlaceholderText = "选择工艺类型";
            this.comb_materialCraf.Size = new System.Drawing.Size(344, 36);
            this.comb_materialCraf.TabIndex = 0;
            this.comb_materialCraf.SelectedIndexChanged += new AntdUI.IntEventHandler(this.comb_materialCraf_SelectedIndexChanged);
            // 
            // inputSearch
            // 
            this.inputSearch.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(231)))), ((int)(((byte)(235)))));
            this.inputSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.inputSearch.Location = new System.Drawing.Point(3, 45);
            this.inputSearch.Name = "inputSearch";
            this.inputSearch.PlaceholderText = "搜索配方";
            this.inputSearch.Size = new System.Drawing.Size(344, 36);
            this.inputSearch.TabIndex = 1;
            this.inputSearch.TextChanged += new System.EventHandler(this.inputSearch_TextChanged);
            // 
            // labelCraftTime
            // 
            this.labelCraftTime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelCraftTime.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(112)))), ((int)(((byte)(133)))));
            this.labelCraftTime.Location = new System.Drawing.Point(3, 87);
            this.labelCraftTime.Name = "labelCraftTime";
            this.labelCraftTime.Size = new System.Drawing.Size(344, 24);
            this.labelCraftTime.TabIndex = 2;
            this.labelCraftTime.Text = "修改时间：-";
            // 
            // btn_SaveCurrentCraft
            // 
            this.btn_SaveCurrentCraft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_SaveCurrentCraft.Location = new System.Drawing.Point(3, 648);
            this.btn_SaveCurrentCraft.Name = "btn_SaveCurrentCraft";
            this.btn_SaveCurrentCraft.Size = new System.Drawing.Size(344, 48);
            this.btn_SaveCurrentCraft.TabIndex = 4;
            this.btn_SaveCurrentCraft.Text = "保存当前配方";
            this.btn_SaveCurrentCraft.Type = AntdUI.TTypeMini.Success;
            this.btn_SaveCurrentCraft.Click += new System.EventHandler(this.btn_SaveCurrentCraft_Click);
            // 
            // tableLayoutPanelRight
            // 
            this.tableLayoutPanelRight.ColumnCount = 1;
            this.tableLayoutPanelRight.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelRight.Controls.Add(this.panel_Main, 0, 0);
            this.tableLayoutPanelRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelRight.Location = new System.Drawing.Point(395, 87);
            this.tableLayoutPanelRight.Name = "tableLayoutPanelRight";
            this.tableLayoutPanelRight.RowCount = 1;
            this.tableLayoutPanelRight.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelRight.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanelRight.Size = new System.Drawing.Size(952, 723);
            this.tableLayoutPanelRight.TabIndex = 2;
            // 
            // panel_Main
            // 
            this.panel_Main.BackColor = System.Drawing.Color.White;
            this.panel_Main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_Main.Location = new System.Drawing.Point(3, 3);
            this.panel_Main.Name = "panel_Main";
            this.panel_Main.Size = new System.Drawing.Size(946, 717);
            this.panel_Main.TabIndex = 1;
            // 
            // Form_CraftFrame
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1362, 825);
            this.Controls.Add(this.tableLayoutPanelRoot);
            this.Name = "Form_CraftFrame";
            this.Text = "Form_CraftBase";
            this.Load += new System.EventHandler(this.Form_CraftFrame_Load);
            ((System.ComponentModel.ISupportInitialize)(this.tableCrafts)).EndInit();
            this.tableLayoutPanelRoot.ResumeLayout(false);
            this.panelLeft.ResumeLayout(false);
            this.tableLayoutPanelLeft.ResumeLayout(false);
            this.tableLayoutPanelRight.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView tableCrafts;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelRoot;
        private AntdUI.PageHeader pageHeader;
        private AntdUI.Panel panelLeft;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelLeft;
        private AntdUI.Select comb_materialCraf;
        private AntdUI.Input inputSearch;
        private AntdUI.Label labelCraftTime;
        private AntdUI.Button btn_SaveCurrentCraft;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelRight;
        private AntdUI.Panel panel_Main;
        private System.Windows.Forms.ToolTip toolTipCraftTime;
    }
}
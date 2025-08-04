namespace Frame_Core
{
    partial class SaveSetingForm
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("节点2");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("节点3");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("节点0", new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2});
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("节点1");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("节点5");
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("节点4", new System.Windows.Forms.TreeNode[] {
            treeNode5});
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("节点0");
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbx_HighDefinition_ProdScreenshot = new System.Windows.Forms.CheckBox();
            this.cbx_SaveImageSelect_ProdScreenshot = new System.Windows.Forms.ComboBox();
            this.cbx_SaveImageSelect_Screenshot = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cbx_SaveImageFormat_ProdScreenshot = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cbx_SaveImageSelect_ProdImg = new System.Windows.Forms.ComboBox();
            this.cbx_SaveImageFormat_ProdImg = new System.Windows.Forms.ComboBox();
            this.nud_NSaveImageNum = new System.Windows.Forms.NumericUpDown();
            this.cbx_EnNSaveImage = new System.Windows.Forms.CheckBox();
            this.cbx_HighDefinition_Screenshot = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cbx_SaveImageSelect_Primitive = new System.Windows.Forms.ComboBox();
            this.cbx_IsSaveImage_Min = new System.Windows.Forms.CheckBox();
            this.cbx_SaveImageFormat_Min = new System.Windows.Forms.ComboBox();
            this.cbx_IsShowImage_Min = new System.Windows.Forms.CheckBox();
            this.cbx_SaveImageFormat_Screenshot = new System.Windows.Forms.ComboBox();
            this.cbx_IsShowImage_Primitive = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.nud_IsDebug = new System.Windows.Forms.NumericUpDown();
            this.btn_SaveSystemInfo = new System.Windows.Forms.Button();
            this.cbx_SaveImageFormat_Primitive = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.Tv_Select = new System.Windows.Forms.TreeView();
            this.label7 = new System.Windows.Forms.Label();
            this.cbx_SaveImageFormat_Deepth = new System.Windows.Forms.ComboBox();
            this.chx_IsCompression = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_NSaveImageNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_IsDebug)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chx_IsCompression);
            this.groupBox1.Controls.Add(this.cbx_SaveImageFormat_Deepth);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.cbx_HighDefinition_ProdScreenshot);
            this.groupBox1.Controls.Add(this.cbx_SaveImageSelect_ProdScreenshot);
            this.groupBox1.Controls.Add(this.cbx_SaveImageSelect_Screenshot);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.cbx_SaveImageFormat_ProdScreenshot);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.cbx_SaveImageSelect_ProdImg);
            this.groupBox1.Controls.Add(this.cbx_SaveImageFormat_ProdImg);
            this.groupBox1.Controls.Add(this.nud_NSaveImageNum);
            this.groupBox1.Controls.Add(this.cbx_EnNSaveImage);
            this.groupBox1.Controls.Add(this.cbx_HighDefinition_Screenshot);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cbx_SaveImageSelect_Primitive);
            this.groupBox1.Controls.Add(this.cbx_IsSaveImage_Min);
            this.groupBox1.Controls.Add(this.cbx_SaveImageFormat_Min);
            this.groupBox1.Controls.Add(this.cbx_IsShowImage_Min);
            this.groupBox1.Controls.Add(this.cbx_SaveImageFormat_Screenshot);
            this.groupBox1.Controls.Add(this.cbx_IsShowImage_Primitive);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.nud_IsDebug);
            this.groupBox1.Controls.Add(this.btn_SaveSystemInfo);
            this.groupBox1.Controls.Add(this.cbx_SaveImageFormat_Primitive);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(2, 2);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(638, 331);
            this.groupBox1.TabIndex = 137;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "图像保存";
            // 
            // cbx_HighDefinition_ProdScreenshot
            // 
            this.cbx_HighDefinition_ProdScreenshot.AutoSize = true;
            this.cbx_HighDefinition_ProdScreenshot.Location = new System.Drawing.Point(409, 212);
            this.cbx_HighDefinition_ProdScreenshot.Margin = new System.Windows.Forms.Padding(2);
            this.cbx_HighDefinition_ProdScreenshot.Name = "cbx_HighDefinition_ProdScreenshot";
            this.cbx_HighDefinition_ProdScreenshot.Size = new System.Drawing.Size(96, 16);
            this.cbx_HighDefinition_ProdScreenshot.TabIndex = 177;
            this.cbx_HighDefinition_ProdScreenshot.Text = "高清晰度截图";
            this.cbx_HighDefinition_ProdScreenshot.UseVisualStyleBackColor = true;
            // 
            // cbx_SaveImageSelect_ProdScreenshot
            // 
            this.cbx_SaveImageSelect_ProdScreenshot.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbx_SaveImageSelect_ProdScreenshot.Font = new System.Drawing.Font("黑体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbx_SaveImageSelect_ProdScreenshot.FormattingEnabled = true;
            this.cbx_SaveImageSelect_ProdScreenshot.Location = new System.Drawing.Point(276, 206);
            this.cbx_SaveImageSelect_ProdScreenshot.Name = "cbx_SaveImageSelect_ProdScreenshot";
            this.cbx_SaveImageSelect_ProdScreenshot.Size = new System.Drawing.Size(120, 22);
            this.cbx_SaveImageSelect_ProdScreenshot.TabIndex = 176;
            // 
            // cbx_SaveImageSelect_Screenshot
            // 
            this.cbx_SaveImageSelect_Screenshot.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbx_SaveImageSelect_Screenshot.Font = new System.Drawing.Font("黑体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbx_SaveImageSelect_Screenshot.FormattingEnabled = true;
            this.cbx_SaveImageSelect_Screenshot.Location = new System.Drawing.Point(275, 90);
            this.cbx_SaveImageSelect_Screenshot.Name = "cbx_SaveImageSelect_Screenshot";
            this.cbx_SaveImageSelect_Screenshot.Size = new System.Drawing.Size(120, 22);
            this.cbx_SaveImageSelect_Screenshot.TabIndex = 175;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("黑体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(25, 209);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(63, 14);
            this.label6.TabIndex = 173;
            this.label6.Text = "产品截图";
            // 
            // cbx_SaveImageFormat_ProdScreenshot
            // 
            this.cbx_SaveImageFormat_ProdScreenshot.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbx_SaveImageFormat_ProdScreenshot.Font = new System.Drawing.Font("黑体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbx_SaveImageFormat_ProdScreenshot.FormattingEnabled = true;
            this.cbx_SaveImageFormat_ProdScreenshot.Location = new System.Drawing.Point(141, 206);
            this.cbx_SaveImageFormat_ProdScreenshot.Name = "cbx_SaveImageFormat_ProdScreenshot";
            this.cbx_SaveImageFormat_ProdScreenshot.Size = new System.Drawing.Size(120, 22);
            this.cbx_SaveImageFormat_ProdScreenshot.TabIndex = 172;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("黑体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(25, 173);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(49, 14);
            this.label4.TabIndex = 171;
            this.label4.Text = "产品图";
            // 
            // cbx_SaveImageSelect_ProdImg
            // 
            this.cbx_SaveImageSelect_ProdImg.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbx_SaveImageSelect_ProdImg.Font = new System.Drawing.Font("黑体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbx_SaveImageSelect_ProdImg.FormattingEnabled = true;
            this.cbx_SaveImageSelect_ProdImg.Location = new System.Drawing.Point(275, 170);
            this.cbx_SaveImageSelect_ProdImg.Name = "cbx_SaveImageSelect_ProdImg";
            this.cbx_SaveImageSelect_ProdImg.Size = new System.Drawing.Size(120, 22);
            this.cbx_SaveImageSelect_ProdImg.TabIndex = 170;
            // 
            // cbx_SaveImageFormat_ProdImg
            // 
            this.cbx_SaveImageFormat_ProdImg.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbx_SaveImageFormat_ProdImg.Font = new System.Drawing.Font("黑体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbx_SaveImageFormat_ProdImg.FormattingEnabled = true;
            this.cbx_SaveImageFormat_ProdImg.Location = new System.Drawing.Point(141, 170);
            this.cbx_SaveImageFormat_ProdImg.Name = "cbx_SaveImageFormat_ProdImg";
            this.cbx_SaveImageFormat_ProdImg.Size = new System.Drawing.Size(120, 22);
            this.cbx_SaveImageFormat_ProdImg.TabIndex = 169;
            // 
            // nud_NSaveImageNum
            // 
            this.nud_NSaveImageNum.Location = new System.Drawing.Point(171, 286);
            this.nud_NSaveImageNum.Margin = new System.Windows.Forms.Padding(2);
            this.nud_NSaveImageNum.Name = "nud_NSaveImageNum";
            this.nud_NSaveImageNum.Size = new System.Drawing.Size(90, 21);
            this.nud_NSaveImageNum.TabIndex = 168;
            this.nud_NSaveImageNum.Visible = false;
            // 
            // cbx_EnNSaveImage
            // 
            this.cbx_EnNSaveImage.AutoSize = true;
            this.cbx_EnNSaveImage.Location = new System.Drawing.Point(28, 291);
            this.cbx_EnNSaveImage.Margin = new System.Windows.Forms.Padding(2);
            this.cbx_EnNSaveImage.Name = "cbx_EnNSaveImage";
            this.cbx_EnNSaveImage.Size = new System.Drawing.Size(126, 16);
            this.cbx_EnNSaveImage.TabIndex = 167;
            this.cbx_EnNSaveImage.Text = "每N张图像保存一次";
            this.cbx_EnNSaveImage.UseVisualStyleBackColor = true;
            this.cbx_EnNSaveImage.Visible = false;
            // 
            // cbx_HighDefinition_Screenshot
            // 
            this.cbx_HighDefinition_Screenshot.AutoSize = true;
            this.cbx_HighDefinition_Screenshot.Location = new System.Drawing.Point(408, 94);
            this.cbx_HighDefinition_Screenshot.Margin = new System.Windows.Forms.Padding(2);
            this.cbx_HighDefinition_Screenshot.Name = "cbx_HighDefinition_Screenshot";
            this.cbx_HighDefinition_Screenshot.Size = new System.Drawing.Size(96, 16);
            this.cbx_HighDefinition_Screenshot.TabIndex = 166;
            this.cbx_HighDefinition_Screenshot.Text = "高清晰度截图";
            this.cbx_HighDefinition_Screenshot.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("黑体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(25, 133);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(49, 14);
            this.label5.TabIndex = 164;
            this.label5.Text = "NG小图";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("黑体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(25, 93);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 14);
            this.label3.TabIndex = 162;
            this.label3.Text = "截图";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("黑体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(25, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 14);
            this.label1.TabIndex = 161;
            this.label1.Text = "原图";
            // 
            // cbx_SaveImageSelect_Primitive
            // 
            this.cbx_SaveImageSelect_Primitive.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbx_SaveImageSelect_Primitive.Font = new System.Drawing.Font("黑体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbx_SaveImageSelect_Primitive.FormattingEnabled = true;
            this.cbx_SaveImageSelect_Primitive.Location = new System.Drawing.Point(275, 26);
            this.cbx_SaveImageSelect_Primitive.Name = "cbx_SaveImageSelect_Primitive";
            this.cbx_SaveImageSelect_Primitive.Size = new System.Drawing.Size(120, 22);
            this.cbx_SaveImageSelect_Primitive.TabIndex = 158;
            // 
            // cbx_IsSaveImage_Min
            // 
            this.cbx_IsSaveImage_Min.AutoSize = true;
            this.cbx_IsSaveImage_Min.Location = new System.Drawing.Point(276, 133);
            this.cbx_IsSaveImage_Min.Margin = new System.Windows.Forms.Padding(2);
            this.cbx_IsSaveImage_Min.Name = "cbx_IsSaveImage_Min";
            this.cbx_IsSaveImage_Min.Size = new System.Drawing.Size(84, 16);
            this.cbx_IsSaveImage_Min.TabIndex = 156;
            this.cbx_IsSaveImage_Min.Text = "保存NG小图";
            this.cbx_IsSaveImage_Min.UseVisualStyleBackColor = true;
            // 
            // cbx_SaveImageFormat_Min
            // 
            this.cbx_SaveImageFormat_Min.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbx_SaveImageFormat_Min.Font = new System.Drawing.Font("黑体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbx_SaveImageFormat_Min.FormattingEnabled = true;
            this.cbx_SaveImageFormat_Min.Location = new System.Drawing.Point(141, 130);
            this.cbx_SaveImageFormat_Min.Name = "cbx_SaveImageFormat_Min";
            this.cbx_SaveImageFormat_Min.Size = new System.Drawing.Size(120, 22);
            this.cbx_SaveImageFormat_Min.TabIndex = 154;
            // 
            // cbx_IsShowImage_Min
            // 
            this.cbx_IsShowImage_Min.AutoSize = true;
            this.cbx_IsShowImage_Min.Location = new System.Drawing.Point(409, 132);
            this.cbx_IsShowImage_Min.Margin = new System.Windows.Forms.Padding(2);
            this.cbx_IsShowImage_Min.Name = "cbx_IsShowImage_Min";
            this.cbx_IsShowImage_Min.Size = new System.Drawing.Size(84, 16);
            this.cbx_IsShowImage_Min.TabIndex = 152;
            this.cbx_IsShowImage_Min.Text = "显示NG小图";
            this.cbx_IsShowImage_Min.UseVisualStyleBackColor = true;
            // 
            // cbx_SaveImageFormat_Screenshot
            // 
            this.cbx_SaveImageFormat_Screenshot.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbx_SaveImageFormat_Screenshot.Font = new System.Drawing.Font("黑体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbx_SaveImageFormat_Screenshot.FormattingEnabled = true;
            this.cbx_SaveImageFormat_Screenshot.Location = new System.Drawing.Point(141, 90);
            this.cbx_SaveImageFormat_Screenshot.Name = "cbx_SaveImageFormat_Screenshot";
            this.cbx_SaveImageFormat_Screenshot.Size = new System.Drawing.Size(120, 22);
            this.cbx_SaveImageFormat_Screenshot.TabIndex = 147;
            // 
            // cbx_IsShowImage_Primitive
            // 
            this.cbx_IsShowImage_Primitive.AutoSize = true;
            this.cbx_IsShowImage_Primitive.Location = new System.Drawing.Point(408, 30);
            this.cbx_IsShowImage_Primitive.Margin = new System.Windows.Forms.Padding(2);
            this.cbx_IsShowImage_Primitive.Name = "cbx_IsShowImage_Primitive";
            this.cbx_IsShowImage_Primitive.Size = new System.Drawing.Size(72, 16);
            this.cbx_IsShowImage_Primitive.TabIndex = 145;
            this.cbx_IsShowImage_Primitive.Text = "显示原图";
            this.cbx_IsShowImage_Primitive.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("黑体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(25, 254);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 14);
            this.label2.TabIndex = 142;
            this.label2.Text = "调试模式";
            // 
            // nud_IsDebug
            // 
            this.nud_IsDebug.Location = new System.Drawing.Point(141, 250);
            this.nud_IsDebug.Margin = new System.Windows.Forms.Padding(2);
            this.nud_IsDebug.Name = "nud_IsDebug";
            this.nud_IsDebug.Size = new System.Drawing.Size(90, 21);
            this.nud_IsDebug.TabIndex = 140;
            // 
            // btn_SaveSystemInfo
            // 
            this.btn_SaveSystemInfo.Font = new System.Drawing.Font("黑体", 10.5F);
            this.btn_SaveSystemInfo.Location = new System.Drawing.Point(534, 18);
            this.btn_SaveSystemInfo.Margin = new System.Windows.Forms.Padding(2);
            this.btn_SaveSystemInfo.Name = "btn_SaveSystemInfo";
            this.btn_SaveSystemInfo.Size = new System.Drawing.Size(82, 43);
            this.btn_SaveSystemInfo.TabIndex = 139;
            this.btn_SaveSystemInfo.Text = "保存设置";
            this.btn_SaveSystemInfo.UseVisualStyleBackColor = true;
            this.btn_SaveSystemInfo.Click += new System.EventHandler(this.btn_SaveSystemInfo_Click);
            // 
            // cbx_SaveImageFormat_Primitive
            // 
            this.cbx_SaveImageFormat_Primitive.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbx_SaveImageFormat_Primitive.Font = new System.Drawing.Font("黑体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbx_SaveImageFormat_Primitive.FormattingEnabled = true;
            this.cbx_SaveImageFormat_Primitive.Location = new System.Drawing.Point(141, 26);
            this.cbx_SaveImageFormat_Primitive.Name = "cbx_SaveImageFormat_Primitive";
            this.cbx_SaveImageFormat_Primitive.Size = new System.Drawing.Size(120, 22);
            this.cbx_SaveImageFormat_Primitive.TabIndex = 134;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(177, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(642, 335);
            this.tableLayoutPanel1.TabIndex = 138;
            // 
            // Tv_Select
            // 
            this.Tv_Select.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Tv_Select.Dock = System.Windows.Forms.DockStyle.Left;
            this.Tv_Select.Font = new System.Drawing.Font("宋体", 12F);
            this.Tv_Select.Location = new System.Drawing.Point(0, 0);
            this.Tv_Select.Name = "Tv_Select";
            treeNode1.Name = "节点2";
            treeNode1.Text = "节点2";
            treeNode2.Name = "节点3";
            treeNode2.Text = "节点3";
            treeNode3.Name = "节点0";
            treeNode3.Text = "节点0";
            treeNode4.Name = "节点1";
            treeNode4.Text = "节点1";
            treeNode5.Name = "节点5";
            treeNode5.Text = "节点5";
            treeNode6.Name = "节点4";
            treeNode6.Text = "节点4";
            treeNode7.BackColor = System.Drawing.Color.DodgerBlue;
            treeNode7.Name = "节点0";
            treeNode7.Text = "节点0";
            this.Tv_Select.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode3,
            treeNode4,
            treeNode6,
            treeNode7});
            this.Tv_Select.Size = new System.Drawing.Size(177, 335);
            this.Tv_Select.TabIndex = 139;
            this.Tv_Select.DrawNode += new System.Windows.Forms.DrawTreeNodeEventHandler(this.Tv_Select_DrawNode);
            this.Tv_Select.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.Tv_Select_AfterSelect);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("黑体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label7.Location = new System.Drawing.Point(25, 59);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(63, 14);
            this.label7.TabIndex = 178;
            this.label7.Text = "3D深度图";
            // 
            // cbx_SaveImageFormat_Deepth
            // 
            this.cbx_SaveImageFormat_Deepth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbx_SaveImageFormat_Deepth.Font = new System.Drawing.Font("黑体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbx_SaveImageFormat_Deepth.FormattingEnabled = true;
            this.cbx_SaveImageFormat_Deepth.Location = new System.Drawing.Point(141, 59);
            this.cbx_SaveImageFormat_Deepth.Name = "cbx_SaveImageFormat_Deepth";
            this.cbx_SaveImageFormat_Deepth.Size = new System.Drawing.Size(120, 22);
            this.cbx_SaveImageFormat_Deepth.TabIndex = 179;
            // 
            // chx_IsCompression
            // 
            this.chx_IsCompression.AutoSize = true;
            this.chx_IsCompression.Location = new System.Drawing.Point(276, 61);
            this.chx_IsCompression.Margin = new System.Windows.Forms.Padding(2);
            this.chx_IsCompression.Name = "chx_IsCompression";
            this.chx_IsCompression.Size = new System.Drawing.Size(72, 16);
            this.chx_IsCompression.TabIndex = 180;
            this.chx_IsCompression.Text = "是否压缩";
            this.chx_IsCompression.UseVisualStyleBackColor = true;
            // 
            // SaveSetingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(819, 335);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.Tv_Select);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "SaveSetingForm";
            this.Text = "SaveSetingForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BaseSetingForm_FormClosing);
            this.Load += new System.EventHandler(this.BaseSetingForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_NSaveImageNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_IsDebug)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown nud_IsDebug;
        private System.Windows.Forms.Button btn_SaveSystemInfo;
        private System.Windows.Forms.ComboBox cbx_SaveImageFormat_Primitive;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TreeView Tv_Select;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox cbx_IsShowImage_Primitive;
        private System.Windows.Forms.ComboBox cbx_SaveImageFormat_Screenshot;
        private System.Windows.Forms.CheckBox cbx_IsShowImage_Min;
        private System.Windows.Forms.CheckBox cbx_IsSaveImage_Min;
        private System.Windows.Forms.ComboBox cbx_SaveImageFormat_Min;
        private System.Windows.Forms.ComboBox cbx_SaveImageSelect_Primitive;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox cbx_HighDefinition_Screenshot;
        private System.Windows.Forms.CheckBox cbx_EnNSaveImage;
        private System.Windows.Forms.NumericUpDown nud_NSaveImageNum;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cbx_SaveImageSelect_ProdImg;
        private System.Windows.Forms.ComboBox cbx_SaveImageFormat_ProdImg;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cbx_SaveImageFormat_ProdScreenshot;
        private System.Windows.Forms.ComboBox cbx_SaveImageSelect_Screenshot;
        private System.Windows.Forms.ComboBox cbx_SaveImageSelect_ProdScreenshot;
        private System.Windows.Forms.CheckBox cbx_HighDefinition_ProdScreenshot;
        private System.Windows.Forms.ComboBox cbx_SaveImageFormat_Deepth;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox chx_IsCompression;
    }
}
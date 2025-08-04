namespace Mef_VisionFrame
{
    partial class UC_ProcessSetting
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UC_ProcessSetting));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.Gp_ProcessList = new System.Windows.Forms.GroupBox();
            this.ListBox_ProcessList = new System.Windows.Forms.ListBox();
            this.panel_center = new System.Windows.Forms.Panel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton_Load = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_Save = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolstripBtn_RunOnce = new System.Windows.Forms.ToolStripButton();
            this.toolstripBtn_RunLoop = new System.Windows.Forms.ToolStripButton();
            this.toolstripBtn_RunStop = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolstripBtn_AddProcess = new System.Windows.Forms.ToolStripButton();
            this.toolstripBtn_DeleteProcess = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolstripBtn_AddModule = new System.Windows.Forms.ToolStripButton();
            this.toolstripBtn_DeleteModule = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolstripBtn_PluginToolBox = new System.Windows.Forms.ToolStripButton();
            this.toolstripBtn_Properties = new System.Windows.Forms.ToolStripButton();
            this.tSBtn_IniCamC = new System.Windows.Forms.ToolStripButton();
            this.picBox_FoldProperty = new System.Windows.Forms.PictureBox();
            this.Gp_ModuleList = new System.Windows.Forms.GroupBox();
            this.listBox_ModuleList = new System.Windows.Forms.ListBox();
            this.Gp_Information = new System.Windows.Forms.GroupBox();
            this.lb_ModuleNEnable = new System.Windows.Forms.Label();
            this.lb_ModuleRemarks = new System.Windows.Forms.Label();
            this.lb_ModuleID = new System.Windows.Forms.Label();
            this.lb_PluginName = new System.Windows.Forms.Label();
            this.lb_ModuleName = new System.Windows.Forms.Label();
            this.lb_ProcessID = new System.Windows.Forms.Label();
            this.lb_ProcessRunMode = new System.Windows.Forms.Label();
            this.lb_ProcessName = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.Tv_PluginTools = new System.Windows.Forms.TreeView();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.toolStripBtn_ExpandAll = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripTextBox_Search = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripBtn_Search = new System.Windows.Forms.ToolStripButton();
            this.toolStripBtn_NextMatch = new System.Windows.Forms.ToolStripButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.Gp_ProcessList.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picBox_FoldProperty)).BeginInit();
            this.Gp_ModuleList.SuspendLayout();
            this.Gp_Information.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tableLayoutPanel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = System.Drawing.Color.Transparent;
            this.splitContainer1.Panel2.Controls.Add(this.groupBox1);
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.Gp_ProcessList, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel_center, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.toolStrip1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.picBox_FoldProperty, 4, 0);
            this.tableLayoutPanel1.Controls.Add(this.Gp_ModuleList, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.Gp_Information, 0, 5);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // Gp_ProcessList
            // 
            this.Gp_ProcessList.Controls.Add(this.ListBox_ProcessList);
            resources.ApplyResources(this.Gp_ProcessList, "Gp_ProcessList");
            this.Gp_ProcessList.Name = "Gp_ProcessList";
            this.tableLayoutPanel1.SetRowSpan(this.Gp_ProcessList, 4);
            this.Gp_ProcessList.TabStop = false;
            // 
            // ListBox_ProcessList
            // 
            this.ListBox_ProcessList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.ListBox_ProcessList, "ListBox_ProcessList");
            this.ListBox_ProcessList.FormattingEnabled = true;
            this.ListBox_ProcessList.Name = "ListBox_ProcessList";
            this.ListBox_ProcessList.SelectedIndexChanged += new System.EventHandler(this.ListBox_ProcessList_SelectedIndexChanged);
            // 
            // panel_center
            // 
            this.panel_center.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.tableLayoutPanel1.SetColumnSpan(this.panel_center, 3);
            resources.ApplyResources(this.panel_center, "panel_center");
            this.panel_center.Name = "panel_center";
            this.tableLayoutPanel1.SetRowSpan(this.panel_center, 5);
            // 
            // toolStrip1
            // 
            resources.ApplyResources(this.toolStrip1, "toolStrip1");
            this.tableLayoutPanel1.SetColumnSpan(this.toolStrip1, 3);
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(30, 30);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton_Load,
            this.toolStripButton_Save,
            this.toolStripSeparator1,
            this.toolstripBtn_RunOnce,
            this.toolstripBtn_RunLoop,
            this.toolstripBtn_RunStop,
            this.toolStripSeparator2,
            this.toolstripBtn_AddProcess,
            this.toolstripBtn_DeleteProcess,
            this.toolStripSeparator3,
            this.toolstripBtn_AddModule,
            this.toolstripBtn_DeleteModule,
            this.toolStripSeparator4,
            this.toolstripBtn_PluginToolBox,
            this.toolstripBtn_Properties,
            this.tSBtn_IniCamC});
            this.toolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Stretch = true;
            // 
            // toolStripButton_Load
            // 
            this.toolStripButton_Load.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton_Load.Image = global::Mef_VisionFrame.Properties.Resources.加载;
            resources.ApplyResources(this.toolStripButton_Load, "toolStripButton_Load");
            this.toolStripButton_Load.Name = "toolStripButton_Load";
            this.toolStripButton_Load.Click += new System.EventHandler(this.toolStripButton_Load_Click);
            // 
            // toolStripButton_Save
            // 
            this.toolStripButton_Save.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton_Save.Image = global::Mef_VisionFrame.Properties.Resources.保存;
            resources.ApplyResources(this.toolStripButton_Save, "toolStripButton_Save");
            this.toolStripButton_Save.Name = "toolStripButton_Save";
            this.toolStripButton_Save.Click += new System.EventHandler(this.toolStripButton_Save_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // toolstripBtn_RunOnce
            // 
            this.toolstripBtn_RunOnce.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolstripBtn_RunOnce, "toolstripBtn_RunOnce");
            this.toolstripBtn_RunOnce.Image = global::Mef_VisionFrame.Properties.Resources.单次循环;
            this.toolstripBtn_RunOnce.Name = "toolstripBtn_RunOnce";
            this.toolstripBtn_RunOnce.Click += new System.EventHandler(this.toolstripBtn_RunOnce_Click);
            // 
            // toolstripBtn_RunLoop
            // 
            this.toolstripBtn_RunLoop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolstripBtn_RunLoop, "toolstripBtn_RunLoop");
            this.toolstripBtn_RunLoop.Image = global::Mef_VisionFrame.Properties.Resources.循环;
            this.toolstripBtn_RunLoop.Name = "toolstripBtn_RunLoop";
            this.toolstripBtn_RunLoop.Click += new System.EventHandler(this.toolstripBtn_RunLoop_Click);
            // 
            // toolstripBtn_RunStop
            // 
            this.toolstripBtn_RunStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolstripBtn_RunStop, "toolstripBtn_RunStop");
            this.toolstripBtn_RunStop.Image = global::Mef_VisionFrame.Properties.Resources.停止运行;
            this.toolstripBtn_RunStop.Name = "toolstripBtn_RunStop";
            this.toolstripBtn_RunStop.Click += new System.EventHandler(this.toolstripBtn_RunStop_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // toolstripBtn_AddProcess
            // 
            this.toolstripBtn_AddProcess.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolstripBtn_AddProcess, "toolstripBtn_AddProcess");
            this.toolstripBtn_AddProcess.Image = global::Mef_VisionFrame.Properties.Resources.增加子级;
            this.toolstripBtn_AddProcess.Name = "toolstripBtn_AddProcess";
            this.toolstripBtn_AddProcess.Click += new System.EventHandler(this.toolstripBtn_AddProcess_Click);
            // 
            // toolstripBtn_DeleteProcess
            // 
            this.toolstripBtn_DeleteProcess.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolstripBtn_DeleteProcess, "toolstripBtn_DeleteProcess");
            this.toolstripBtn_DeleteProcess.Image = global::Mef_VisionFrame.Properties.Resources.组删除;
            this.toolstripBtn_DeleteProcess.Name = "toolstripBtn_DeleteProcess";
            this.toolstripBtn_DeleteProcess.Click += new System.EventHandler(this.toolstripBtn_DeleteProcess_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            // 
            // toolstripBtn_AddModule
            // 
            this.toolstripBtn_AddModule.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolstripBtn_AddModule, "toolstripBtn_AddModule");
            this.toolstripBtn_AddModule.Image = global::Mef_VisionFrame.Properties.Resources.工程建设_增加;
            this.toolstripBtn_AddModule.Name = "toolstripBtn_AddModule";
            this.toolstripBtn_AddModule.Click += new System.EventHandler(this.toolstripBtn_AddModule_Click);
            // 
            // toolstripBtn_DeleteModule
            // 
            this.toolstripBtn_DeleteModule.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolstripBtn_DeleteModule, "toolstripBtn_DeleteModule");
            this.toolstripBtn_DeleteModule.Image = global::Mef_VisionFrame.Properties.Resources.删除;
            this.toolstripBtn_DeleteModule.Name = "toolstripBtn_DeleteModule";
            this.toolstripBtn_DeleteModule.Click += new System.EventHandler(this.toolstripBtn_DeleteModule_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
            // 
            // toolstripBtn_PluginToolBox
            // 
            this.toolstripBtn_PluginToolBox.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolstripBtn_PluginToolBox.Image = global::Mef_VisionFrame.Properties.Resources.工具箱;
            resources.ApplyResources(this.toolstripBtn_PluginToolBox, "toolstripBtn_PluginToolBox");
            this.toolstripBtn_PluginToolBox.Name = "toolstripBtn_PluginToolBox";
            this.toolstripBtn_PluginToolBox.Click += new System.EventHandler(this.toolstripBtn_PluginToolBox_Click);
            // 
            // toolstripBtn_Properties
            // 
            this.toolstripBtn_Properties.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolstripBtn_Properties.Image = global::Mef_VisionFrame.Properties.Resources.图查属性;
            resources.ApplyResources(this.toolstripBtn_Properties, "toolstripBtn_Properties");
            this.toolstripBtn_Properties.Name = "toolstripBtn_Properties";
            this.toolstripBtn_Properties.Click += new System.EventHandler(this.toolstripBtn_Properties_Click);
            // 
            // tSBtn_IniCamC
            // 
            this.tSBtn_IniCamC.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.tSBtn_IniCamC.Image = global::Mef_VisionFrame.Properties.Resources.初始化;
            resources.ApplyResources(this.tSBtn_IniCamC, "tSBtn_IniCamC");
            this.tSBtn_IniCamC.Name = "tSBtn_IniCamC";
            this.tSBtn_IniCamC.Click += new System.EventHandler(this.tSBtn_IniCamC_Click);
            // 
            // picBox_FoldProperty
            // 
            this.picBox_FoldProperty.BackgroundImage = global::Mef_VisionFrame.Properties.Resources.右_右;
            resources.ApplyResources(this.picBox_FoldProperty, "picBox_FoldProperty");
            this.picBox_FoldProperty.Name = "picBox_FoldProperty";
            this.picBox_FoldProperty.TabStop = false;
            this.picBox_FoldProperty.Click += new System.EventHandler(this.picBox_FoldProperty_Click);
            // 
            // Gp_ModuleList
            // 
            this.Gp_ModuleList.Controls.Add(this.listBox_ModuleList);
            resources.ApplyResources(this.Gp_ModuleList, "Gp_ModuleList");
            this.Gp_ModuleList.Name = "Gp_ModuleList";
            this.tableLayoutPanel1.SetRowSpan(this.Gp_ModuleList, 4);
            this.Gp_ModuleList.TabStop = false;
            // 
            // listBox_ModuleList
            // 
            this.listBox_ModuleList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.listBox_ModuleList, "listBox_ModuleList");
            this.listBox_ModuleList.FormattingEnabled = true;
            this.listBox_ModuleList.Name = "listBox_ModuleList";
            this.listBox_ModuleList.Click += new System.EventHandler(this.listBox_ModuleList_Click);
            // 
            // Gp_Information
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.Gp_Information, 2);
            this.Gp_Information.Controls.Add(this.lb_ModuleNEnable);
            this.Gp_Information.Controls.Add(this.lb_ModuleRemarks);
            this.Gp_Information.Controls.Add(this.lb_ModuleID);
            this.Gp_Information.Controls.Add(this.lb_PluginName);
            this.Gp_Information.Controls.Add(this.lb_ModuleName);
            this.Gp_Information.Controls.Add(this.lb_ProcessID);
            this.Gp_Information.Controls.Add(this.lb_ProcessRunMode);
            this.Gp_Information.Controls.Add(this.lb_ProcessName);
            this.Gp_Information.Controls.Add(this.label1);
            this.Gp_Information.Controls.Add(this.label2);
            this.Gp_Information.Controls.Add(this.label8);
            this.Gp_Information.Controls.Add(this.label5);
            this.Gp_Information.Controls.Add(this.label6);
            this.Gp_Information.Controls.Add(this.label3);
            this.Gp_Information.Controls.Add(this.label9);
            this.Gp_Information.Controls.Add(this.label10);
            resources.ApplyResources(this.Gp_Information, "Gp_Information");
            this.Gp_Information.Name = "Gp_Information";
            this.Gp_Information.TabStop = false;
            // 
            // lb_ModuleNEnable
            // 
            resources.ApplyResources(this.lb_ModuleNEnable, "lb_ModuleNEnable");
            this.lb_ModuleNEnable.Name = "lb_ModuleNEnable";
            // 
            // lb_ModuleRemarks
            // 
            resources.ApplyResources(this.lb_ModuleRemarks, "lb_ModuleRemarks");
            this.lb_ModuleRemarks.Name = "lb_ModuleRemarks";
            // 
            // lb_ModuleID
            // 
            resources.ApplyResources(this.lb_ModuleID, "lb_ModuleID");
            this.lb_ModuleID.Name = "lb_ModuleID";
            // 
            // lb_PluginName
            // 
            resources.ApplyResources(this.lb_PluginName, "lb_PluginName");
            this.lb_PluginName.Name = "lb_PluginName";
            // 
            // lb_ModuleName
            // 
            resources.ApplyResources(this.lb_ModuleName, "lb_ModuleName");
            this.lb_ModuleName.Name = "lb_ModuleName";
            // 
            // lb_ProcessID
            // 
            resources.ApplyResources(this.lb_ProcessID, "lb_ProcessID");
            this.lb_ProcessID.Name = "lb_ProcessID";
            // 
            // lb_ProcessRunMode
            // 
            resources.ApplyResources(this.lb_ProcessRunMode, "lb_ProcessRunMode");
            this.lb_ProcessRunMode.Name = "lb_ProcessRunMode";
            // 
            // lb_ProcessName
            // 
            resources.ApplyResources(this.lb_ProcessName, "lb_ProcessName");
            this.lb_ProcessName.Name = "lb_ProcessName";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tableLayoutPanel2);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.Tv_PluginTools, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.toolStrip2, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.groupBox2, 0, 2);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // Tv_PluginTools
            // 
            this.Tv_PluginTools.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.Tv_PluginTools, "Tv_PluginTools");
            this.Tv_PluginTools.Name = "Tv_PluginTools";
            this.Tv_PluginTools.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            ((System.Windows.Forms.TreeNode)(resources.GetObject("Tv_PluginTools.Nodes"))),
            ((System.Windows.Forms.TreeNode)(resources.GetObject("Tv_PluginTools.Nodes1"))),
            ((System.Windows.Forms.TreeNode)(resources.GetObject("Tv_PluginTools.Nodes2"))),
            ((System.Windows.Forms.TreeNode)(resources.GetObject("Tv_PluginTools.Nodes3")))});
            this.Tv_PluginTools.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.Tv_PluginTools_NodeMouseDoubleClick);
            // 
            // toolStrip2
            // 
            resources.ApplyResources(this.toolStrip2, "toolStrip2");
            this.toolStrip2.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripBtn_ExpandAll,
            this.toolStripSeparator5,
            this.toolStripTextBox_Search,
            this.toolStripBtn_Search,
            this.toolStripBtn_NextMatch});
            this.toolStrip2.Name = "toolStrip2";
            // 
            // toolStripBtn_ExpandAll
            // 
            this.toolStripBtn_ExpandAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripBtn_ExpandAll.Image = global::Mef_VisionFrame.Properties.Resources.展开;
            resources.ApplyResources(this.toolStripBtn_ExpandAll, "toolStripBtn_ExpandAll");
            this.toolStripBtn_ExpandAll.Name = "toolStripBtn_ExpandAll";
            this.toolStripBtn_ExpandAll.Click += new System.EventHandler(this.toolStripBtn_ExpandAll_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            resources.ApplyResources(this.toolStripSeparator5, "toolStripSeparator5");
            // 
            // toolStripTextBox_Search
            // 
            resources.ApplyResources(this.toolStripTextBox_Search, "toolStripTextBox_Search");
            this.toolStripTextBox_Search.Name = "toolStripTextBox_Search";
            // 
            // toolStripBtn_Search
            // 
            this.toolStripBtn_Search.BackColor = System.Drawing.Color.Transparent;
            this.toolStripBtn_Search.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripBtn_Search.Image = global::Mef_VisionFrame.Properties.Resources.搜索;
            resources.ApplyResources(this.toolStripBtn_Search, "toolStripBtn_Search");
            this.toolStripBtn_Search.Name = "toolStripBtn_Search";
            this.toolStripBtn_Search.Click += new System.EventHandler(this.toolStripBtn_Search_Click);
            // 
            // toolStripBtn_NextMatch
            // 
            this.toolStripBtn_NextMatch.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripBtn_NextMatch.Image = global::Mef_VisionFrame.Properties.Resources.箭头_向右;
            resources.ApplyResources(this.toolStripBtn_NextMatch, "toolStripBtn_NextMatch");
            this.toolStripBtn_NextMatch.Name = "toolStripBtn_NextMatch";
            this.toolStripBtn_NextMatch.Click += new System.EventHandler(this.toolStripBtn_NextMatch_Click);
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "数据模块.png");
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 200;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // UC_ProcessSetting
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "UC_ProcessSetting";
            this.Load += new System.EventHandler(this.UC_ProcessSetting_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.Gp_ProcessList.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picBox_FoldProperty)).EndInit();
            this.Gp_ModuleList.ResumeLayout(false);
            this.Gp_Information.ResumeLayout(false);
            this.Gp_Information.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolstripBtn_RunOnce;
        private System.Windows.Forms.ToolStripButton toolstripBtn_RunLoop;
        private System.Windows.Forms.ToolStripButton toolstripBtn_RunStop;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolstripBtn_AddProcess;
        private System.Windows.Forms.ToolStripButton toolstripBtn_DeleteProcess;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton toolstripBtn_AddModule;
        private System.Windows.Forms.ToolStripButton toolstripBtn_DeleteModule;
        private System.Windows.Forms.PictureBox picBox_FoldProperty;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton toolstripBtn_PluginToolBox;
        private System.Windows.Forms.ToolStripButton toolstripBtn_Properties;
        public System.Windows.Forms.Panel panel_center;
        private System.Windows.Forms.GroupBox Gp_ProcessList;
        private System.Windows.Forms.GroupBox Gp_ModuleList;
        private System.Windows.Forms.GroupBox Gp_Information;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListBox ListBox_ProcessList;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TreeView Tv_PluginTools;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripButton toolStripBtn_ExpandAll;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox_Search;
        private System.Windows.Forms.ToolStripButton toolStripBtn_Search;
        private System.Windows.Forms.ToolStripButton toolStripBtn_NextMatch;
        private System.Windows.Forms.Label lb_ModuleNEnable;
        private System.Windows.Forms.Label lb_ModuleRemarks;
        private System.Windows.Forms.Label lb_ModuleID;
        private System.Windows.Forms.Label lb_PluginName;
        private System.Windows.Forms.Label lb_ModuleName;
        private System.Windows.Forms.Label lb_ProcessID;
        private System.Windows.Forms.Label lb_ProcessRunMode;
        private System.Windows.Forms.Label lb_ProcessName;
        private System.Windows.Forms.ListBox listBox_ModuleList;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ToolStripButton tSBtn_IniCamC;
        private System.Windows.Forms.ToolStripButton toolStripButton_Load;
        private System.Windows.Forms.ToolStripButton toolStripButton_Save;
    }
}

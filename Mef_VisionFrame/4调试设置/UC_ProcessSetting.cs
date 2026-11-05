using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Frame_Core;
using System.IO;
using System.Threading;
using System.Windows.Controls.Primitives;
using System.Reflection;
using LogTool;

namespace Mef_VisionFrame
{
    public partial class UC_ProcessSetting : UserControl
    {
        private int i_CurProcessID = -1; //当前流程
        private Process proc; //当前选择的流程
        private IPlugin moduleObj_Selected;//当前选择的模块

        bool Is_toolboxFolded = true;//工具箱折叠标志位
        List<TreeNode> MatchNodes = new List<TreeNode>();//工具箱查找结果
        int matchCount = -9;//工具箱查找标志位
        bool Is_Expended = false;
        FormPluginBase moduleFormBase;
        public UC_ProcessSetting()
        {
            InitializeComponent();
            INI_TreeView();
            toolstripBtn_RunOnce.Enabled = false;
            toolstripBtn_RunLoop.Enabled = false;
            toolstripBtn_RunStop.Enabled = false;
        }
        private void UC_ProcessSetting_Load(object sender, EventArgs e)
        {
            IniUC_ProcessSetting();
        }
        public void IniUC_ProcessSetting()
        {
            // 在窗体加载时切换资源
            LanguageProject.ChangeLanguage(Project.Instance.systemInfo.m_CurrentLanguageType, this);
            //添加初始流程，避免报错
            panel_center.Controls.Clear();
            ListBox_ProcessList.Items.Clear();
            if (Project.Instance.m_ProcessList.Count == 0)
            {
                i_CurProcessID = Project.Instance.CreateProcess();
                proc = Project.Instance.GetProcessById(i_CurProcessID);
                ListBox_ProcessList.Items.Add(proc.processInfo.ProcessName);
                ListBox_ProcessList.SelectedIndex = ListBox_ProcessList.Items.Count - 1;
            }
            else
            {
                foreach (ProcessServices item in Project.Instance.m_ProcessList)
                {
                    proc = item.process;
                    ListBox_ProcessList.Items.Add(proc.processInfo.ProcessName);
                    ListBox_ProcessList.SelectedIndex = ListBox_ProcessList.Items.Count - 1;
                }
            }
        }


        /// <summary>
        /// 初始化插件工具箱
        /// </summary>
        public void INI_TreeView()
        {
            Tv_PluginTools.Nodes.Clear();
            Tv_PluginTools.Nodes.Add("相机SDK");
            Tv_PluginTools.Nodes.Add("尺寸测量");
            Tv_PluginTools.Nodes.Add("瑕疵检测");
            Tv_PluginTools.Nodes.Add("背光检测");
            Tv_PluginTools.Nodes.Add("其它");
            foreach (var item in PluginService.pluginsDictionary)
            {
                bool bl_Category = false;
                foreach (TreeNode item_node in Tv_PluginTools.Nodes)
                {
                    if (item.Value.PluginsCategory == item_node.Text)
                    {
                        Tv_PluginTools.Nodes[item_node.Index].Nodes.Add(item.Key);
                        bl_Category = true;
                    }
                }
                if (bl_Category == false)
                {
                    Tv_PluginTools.Nodes[Tv_PluginTools.Nodes.Count - 1].Nodes.Add(item.Key);
                }
            }
            Tv_PluginTools.ExpandAll();
            Is_Expended = true;
        }


        /// <summary>
        /// 刷新模块moduleList
        /// </summary>
        /// <param name="proc"></param>
        public void Update_ListModule(Process proc)
        {
            if (proc == null) return;
            listBox_ModuleList.BeginUpdate();
            listBox_ModuleList.Items.Clear();
            foreach (IPlugin item in proc.m_ModuleObjList)
            {
                this.listBox_ModuleList.Items.Add(item.moduleInfo.ModuleName);
            }
            this.listBox_ModuleList.EndUpdate();
        }
        /// <summary>
        /// 刷新属性栏-流程
        /// </summary>
        /// <param name="proc"></param>
        public void Update_Information(Process proc)
        {
            if (proc == null) return;
            lb_ProcessName.Text = proc.processInfo.ProcessName;
            lb_ProcessID.Text = proc.processInfo.ProcessID.ToString();
            // lb_ProcessRunMode.Text = proc.RunMode.ToString();
        }
        /// <summary>
        /// 刷新属性栏-模块
        /// </summary>
        /// <param name="module"></param>
        public void Update_Information(IPlugin module)
        {
            if (module == null) return;
            proc = Project.Instance.GetProcessByName(module.moduleInfo.ProcessName);
            lb_ModuleName.Text = module.moduleInfo.ModuleName;
            lb_ModuleID.Text = module.moduleInfo.ModuleEncode.ToString();
            lb_PluginName.Text = module.moduleInfo.PluginName;
            lb_ModuleNEnable.Text = module.moduleInfo.IsUse.ToString();
            lb_ModuleRemarks.Text = module.moduleInfo.ModuleRemarks;


        }

        #region toolstrip按钮事件
        /// <summary>
        /// 按钮-折叠侧边栏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void picBox_FoldProperty_Click(object sender, EventArgs e)
        {
            if (Is_toolboxFolded)
            {
                splitContainer1.SplitterDistance = this.Width;
                picBox_FoldProperty.BackgroundImage = Properties.Resources.左_左;
                Is_toolboxFolded = !Is_toolboxFolded;
            }
            else
            {
                Delay(100);
                splitContainer1.SplitterDistance = this.Width - 250;
                picBox_FoldProperty.BackgroundImage = Properties.Resources.右_右;
                Is_toolboxFolded = !Is_toolboxFolded;
            }

        }
        private void Delay(int milliSecond)
        {
            int start = Environment.TickCount;
            while (Math.Abs(Environment.TickCount - start) < milliSecond)
            {

                //可执行某无聊的操作
            }
        }

        private void toolstripBtn_PluginToolBox_Click(object sender, EventArgs e)
        {
            if (Is_toolboxFolded)
            {
                //splitContainer1.Panel2.Controls.Clear();
                splitContainer1.SplitterDistance = this.Width;
                picBox_FoldProperty.BackgroundImage = Properties.Resources.左_左;
                Is_toolboxFolded = !Is_toolboxFolded;
            }
            else
            {
                Delay(100);
                splitContainer1.SplitterDistance = this.Width - 250;
                picBox_FoldProperty.BackgroundImage = Properties.Resources.右_右;
                Is_toolboxFolded = !Is_toolboxFolded;
            }
        }

        private void toolstripBtn_Properties_Click(object sender, EventArgs e)
        {
            //// 每次添加后检查并选中第一个项目
            //if (ListBox_ProcessList.Items.Count > 0)
            //{
            //    ListBox_ProcessList.SelectedIndex = 0;
            //}
            string filePath = Project.Instance.paramPathSet.ProcessPath;//文件（文件夹）路径
            System.Diagnostics.Process.Start(filePath);

        }

        #endregion

        #region 流程List事件
        /// <summary>
        /// 按钮-新建流程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolstripBtn_AddProcess_Click(object sender, EventArgs e)
        {
            i_CurProcessID = Project.Instance.CreateProcess();
            proc = Project.Instance.GetProcessById(i_CurProcessID);
            ListBox_ProcessList.Items.Add(proc.processInfo.ProcessName);
            ListBox_ProcessList.SelectedIndex = ListBox_ProcessList.Items.Count - 1;
            Update_ListModule(proc);
        }
        /// <summary>
        /// 按钮-删除流程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolstripBtn_DeleteProcess_Click(object sender, EventArgs e)
        {
            if (ListBox_ProcessList.SelectedItem == null) return;
            if (ListBox_ProcessList.Items.Count == 1) return;
            string name = ListBox_ProcessList.SelectedItem.ToString();
            proc = Project.Instance.GetProcessByName(name);
            Project.Instance.DeleteProcess(proc);
            ListBox_ProcessList.Items.RemoveAt(ListBox_ProcessList.SelectedIndex);
            ListBox_ProcessList.SelectedIndex = ListBox_ProcessList.Items.Count - 1;
            name = ListBox_ProcessList.SelectedItem.ToString();
            proc = Project.Instance.GetProcessByName(name);
            Update_ListModule(proc);
        }
        private void ListBox_ProcessList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListBox_ProcessList.SelectedItem == null) return;
            string name = ListBox_ProcessList.SelectedItem.ToString();
            proc = Project.Instance.GetProcessByName(name);
            i_CurProcessID = proc.processInfo.ProcessID;
            Update_ListModule(proc);
            Update_Information(proc);
        }
        #endregion

        #region 模块List事件

        /// <summary>
        /// 添加模块
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolstripBtn_AddModule_Click(object sender, EventArgs e)
        {
            // if (ListBox_Tools.SelectedItem == null) return;
            // string pluginName = ListBox_Tools.SelectedItem.ToString();
            //string pluginName = "直线抓边工具";
            //IPlugin PluginLine = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
            //FormPluginBase formBaseFindLine = (FormPluginBase)Activator.CreateInstance(PluginService.pluginsDictionary[pluginName].PluginsFormType);       
            //PluginLine.moduleInfo.PluginName = pluginName;

            //string name = ListBox_ProcessList.SelectedItem.ToString();
            //proc = Project.Instance.GetProcessByName(name);
            //proc.AddModuleObj(PluginLine);
            //Update_ListModule(proc);

            if (Tv_PluginTools.SelectedNode == null) return;
            if (Tv_PluginTools.SelectedNode.Parent == null) return;
            string pluginName = Tv_PluginTools.SelectedNode.Text;
            IPlugin PluginLine = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
            moduleFormBase = (FormPluginBase)Activator.CreateInstance(PluginService.pluginsDictionary[pluginName].PluginsFormType);
            PluginLine.moduleInfo.PluginName = pluginName;

            string name = ListBox_ProcessList.SelectedItem.ToString();
            proc = Project.Instance.GetProcessByName(name);
            proc.AddModuleObj(PluginLine);
            Update_ListModule(proc);

        }
        /// <summary>
        /// 删除模块
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolstripBtn_DeleteModule_Click(object sender, EventArgs e)
        {
            if (listBox_ModuleList.SelectedItem == null) return;

            //获取当前流程
            string name = ListBox_ProcessList.SelectedItem.ToString();
            proc = Project.Instance.GetProcessByName(name);
            //根据名称获取对应的ModuleObjBase
            string moduleName = listBox_ModuleList.SelectedItem.ToString();
            IPlugin moduleObj = proc.GetModuleByName(moduleName);
            proc.DeleteModuleObj(moduleObj);
            Update_ListModule(proc);
            if (listBox_ModuleList.Items.Count > 0)
            {
                listBox_ModuleList.SelectedIndex = listBox_ModuleList.Items.Count - 1;
            }

            Update_Information(moduleObj);

        }
        /// <summary>
        /// 单击打开模块界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBox_ModuleList_Click(object sender, EventArgs e)
        {
            try
            {
                if (listBox_ModuleList.SelectedItem == null) return;
                //获取当前流程
                string name = ListBox_ProcessList.SelectedItem.ToString();

                proc = Project.Instance.GetProcessByName(name);
                //根据名称获取对应的ModuleObjBase
                string moduleName = listBox_ModuleList.SelectedItem.ToString();
                //var moduleNameEnum =LanguageProject.MatchStringToEnum<ModuleNameEnum>(moduleName);
                //if (moduleNameEnum != null)
                //{
                //    moduleName = ((ModuleNameEnum)moduleNameEnum).ToString();
                //}
                //else
                //{
                //    return;
                //}
                IPlugin moduleObj = proc.GetModuleByName(moduleName);
                if (moduleObj_Selected == moduleObj) return;
                moduleObj_Selected = moduleObj;
                if (moduleFormBase != null) moduleFormBase?.Close();
                //获取对应的ModuleFormBase
                PluginsInfo pluginsInfo = PluginService.pluginsDictionary[moduleObj.moduleInfo.PluginName];
                // FormPluginBase moduleFormBase = (FormPluginBase)pluginsInfo.PluginsObjType.Assembly.CreateInstance(pluginsInfo.PluginsFormType.FullName);
                moduleFormBase = (FormPluginBase)Activator.CreateInstance(PluginService.pluginsDictionary[pluginsInfo.PluginsDisplayName].PluginsFormType);
                //
                moduleFormBase.FormLanguage = Project.Instance.systemInfo.m_CurrentLanguageType;
                moduleFormBase.pluginsObjBase = moduleObj;
                panel_center.Controls.Clear();
                moduleFormBase.FormBorderStyle = FormBorderStyle.None;
                moduleFormBase.Dock = DockStyle.Fill;
                moduleFormBase.TopLevel = false;
                panel_center.Controls.Add(moduleFormBase);
                moduleFormBase.Show();
                Update_Information(moduleObj);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
            
        }
        #endregion

        #region 插件工具箱事件

        private void toolStripBtn_ExpandAll_Click(object sender, EventArgs e)
        {
            if (Is_Expended)
            {
                Tv_PluginTools.CollapseAll();
                toolStripBtn_ExpandAll.Image = Properties.Resources.收缩;
                Is_Expended = false;
            }
            else
            {
                Tv_PluginTools.ExpandAll();
                toolStripBtn_ExpandAll.Image = Properties.Resources.展开;
                Is_Expended = true;
            }
        }

        private void toolStripBtn_Search_Click(object sender, EventArgs e)
        {
            Tv_PluginTools.Focus();
            MatchNodes.Clear();
            if (toolStripTextBox_Search.TextLength == 0) return;
            foreach (TreeNode item in Tv_PluginTools.Nodes)
            {
                if (item.Text.Contains(toolStripTextBox_Search.Text))
                {
                    MatchNodes.Add(item);
                }
                foreach (TreeNode item1 in item.Nodes)
                {
                    if (item1.Text.Contains(toolStripTextBox_Search.Text))
                    {
                        MatchNodes.Add(item1);
                    }
                }
            }
            matchCount = MatchNodes.Count();
            toolStripBtn_NextMatch_Click(null, null);
        }

        private void toolStripBtn_NextMatch_Click(object sender, EventArgs e)
        {
            if (matchCount > 0)
            {
                Tv_PluginTools.SelectedNode = MatchNodes[MatchNodes.Count() - matchCount];
                matchCount--;
            }
        }

        private void Tv_PluginTools_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (Tv_PluginTools.SelectedNode == null) return;
            if (Tv_PluginTools.SelectedNode.Parent == null) return;
            string pluginName = Tv_PluginTools.SelectedNode.Text;
            IPlugin PluginLine = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
            //moduleFormBase = (FormPluginBase)Activator.CreateInstance(PluginService.pluginsDictionary[pluginName].PluginsFormType);
            PluginLine.moduleInfo.PluginName = pluginName;

            string name = ListBox_ProcessList.SelectedItem.ToString();
            proc = Project.Instance.GetProcessByName(name);
            proc.AddModuleObj(PluginLine);
            Update_ListModule(proc);
            listBox_ModuleList.SelectedIndex = listBox_ModuleList.Items.Count - 1;
        }
        #endregion




        private void toolstripBtn_RunOnce_Click(object sender, EventArgs e)
        {
            return;
        }

        private void toolstripBtn_RunLoop_Click(object sender, EventArgs e)
        {
            return;
        }

        private void toolstripBtn_RunStop_Click(object sender, EventArgs e)
        {
            return;
        }
        /// <summary>
        /// 定时器-刷新主流程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (Project.Instance.GetStates() == true)
            {
                toolstripBtn_RunOnce.Enabled = false;
                toolstripBtn_RunLoop.Enabled = false;
                toolstripBtn_RunStop.Enabled = true;
            }
            else
            {
                toolstripBtn_RunOnce.Enabled = true;
                toolstripBtn_RunLoop.Enabled = true;
                toolstripBtn_RunStop.Enabled = false;
            }
        }

        public bool MessageBoxShow()
        {
            DialogResult res = System.Windows.Forms.MessageBox.Show("确定操作？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (res == DialogResult.OK)  //1为确定，2为取消,...
            {
                return true;
            }
            else
            {
                return false;
            }
        }




        private void tSBtn_IniCamC_Click(object sender, EventArgs e)
        {
            if (System.Windows.Forms.MessageBox.Show("是否重建运行流程(ReBuild Process)？", "重建运行流程", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Project.Instance.DeleteALLProcess();
                Project.Instance.baseProDevice.INIPro();
                Project.Instance.baseProDevice.BuildProcessServiceList();
                IniUC_ProcessSetting();
                //保存流程
                string path = Project.Instance.paramPathSet.ProcessPath;
                bool bl = Directory.Exists(path);
                if (bl)
                {
                    DirectoryInfo di = new DirectoryInfo(path);
                    di.Delete(true);
                }
                Project.Instance.SaveProject();
            }

        }

        private void toolStripButton_Load_Click(object sender, EventArgs e)
        {
            if (System.Windows.Forms.MessageBox.Show("是否重新加载流程(Reload Process)？", "重新加载流程", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                panel_center.Controls.Clear();
                ListBox_ProcessList.Items.Clear();
                Project.Instance.ReadProject();
                foreach (ProcessServices item in Project.Instance.m_ProcessList)
                {
                    proc = item.process;
                    ListBox_ProcessList.Items.Add(proc.processInfo.ProcessName);
                    ListBox_ProcessList.SelectedIndex = ListBox_ProcessList.Items.Count - 1;
                }
            }
        }

        private void toolStripButton_Save_Click(object sender, EventArgs e)
        {
            DialogResult res = System.Windows.Forms.MessageBox.Show("确定保存流程(Save Process)？", "保存流程（Save）", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (res == DialogResult.OK)  //1为确定，2为取消,...
            {
                string path = Project.Instance.paramPathSet.ProcessPath;
                bool bl = Directory.Exists(path);
                if (bl)
                {
                    DirectoryInfo di = new DirectoryInfo(path);
                    di.Delete(true);
                }
                Project.Instance.SaveProject();
            }
            else
            {
                return;
            }

        }
    }
}

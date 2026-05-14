using Frame_Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Mef_VisionFrame.调试设置.WPF
{
    public partial class WPFDebugProcessView : UserControl
    {
        private readonly List<IPlugin> currentModules = new List<IPlugin>();
        private readonly DebugProcessCardFactory cardFactory;
        private readonly PluginDialogService pluginDialogService;

        private Frame_Core.Process currentProcess;
        private IPlugin selectedModule;
        private string pendingPluginName;
        private bool loaded;
        private bool pluginContainerVisible = true;

        public WPFDebugProcessView()
        {
            InitializeComponent();

            ModuleRunStateReader stateReader = new ModuleRunStateReader();
            cardFactory = new DebugProcessCardFactory(stateReader);
            pluginDialogService = new PluginDialogService();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (loaded) return;

            loaded = true;
            BuildPluginContainer();
            RefreshProcessList(true);
            SetStatus("调试界面已加载");
        }

        private void BuildPluginContainer()
        {
            PluginCategoryPanel.Children.Clear();
            if (PluginService.pluginsDictionary == null || PluginService.pluginsDictionary.Count == 0)
            {
                PluginCategoryPanel.Children.Add(cardFactory.CreateText("未读取到插件容器", 14, Brushes.OrangeRed));
                return;
            }

            var groups = PluginService.pluginsDictionary
                .OrderBy(p => p.Value.PluginsCategory)
                .ThenBy(p => p.Value.PluginsDisplayName)
                .GroupBy(p => string.IsNullOrWhiteSpace(p.Value.PluginsCategory) ? "未分类" : p.Value.PluginsCategory);

            foreach (var group in groups)
            {
                Border groupBox = CreatePluginGroupBox();
                DockPanel groupDock = new DockPanel();

                TextBlock title = cardFactory.CreateText(group.Key, 14, new SolidColorBrush(Color.FromRgb(0, 229, 255)));
                title.FontWeight = FontWeights.Bold;
                title.Margin = new Thickness(0, 0, 0, 8);
                DockPanel.SetDock(title, Dock.Top);
                groupDock.Children.Add(title);

                StackPanel panel = new StackPanel();
                foreach (var pair in group)
                {
                    Button btn = cardFactory.CreatePluginTile(pair.Value.PluginsDisplayName, pair.Value.PluginsCategory, false);
                    btn.Tag = pair.Value.PluginsDisplayName;
                    btn.Click += PluginTile_Click;
                    btn.MouseDoubleClick += PluginTile_DoubleClick;
                    btn.PreviewMouseDoubleClick += PluginTile_DoubleClick;
                    panel.Children.Add(btn);
                }

                groupDock.Children.Add(panel);
                groupBox.Child = groupDock;
                PluginCategoryPanel.Children.Add(groupBox);
            }
        }

        private Border CreatePluginGroupBox()
        {
            return new Border
            {
                Margin = new Thickness(0, 0, 0, 12),
                Padding = new Thickness(10),
                BorderThickness = new Thickness(1),
                BorderBrush = new SolidColorBrush(Color.FromRgb(21, 116, 138)),
                Background = new SolidColorBrush(Color.FromRgb(8, 18, 28))
            };
        }

        private void RefreshProcessList(bool createIfEmpty)
        {
            ProcessPanel.Children.Clear();

            if ((Project.Instance.m_ProcessList == null || Project.Instance.m_ProcessList.Count == 0) && createIfEmpty)
            {
                int id = Project.Instance.CreateProcess();
                currentProcess = Project.Instance.GetProcessById(id);
            }

            if (Project.Instance.m_ProcessList == null || Project.Instance.m_ProcessList.Count == 0)
            {
                currentProcess = null;
                ProcessPanel.Children.Add(cardFactory.CreateText("暂无流程", 14, Brushes.OrangeRed));
                RefreshModuleList();
                return;
            }

            if (currentProcess == null || Project.Instance.m_ProcessList.All(p => p.process != currentProcess))
            {
                currentProcess = Project.Instance.m_ProcessList[0].process;
            }

            foreach (ProcessServices service in Project.Instance.m_ProcessList)
            {
                Frame_Core.Process proc = service.process;
                Button btn = cardFactory.CreateProcessCard(proc, proc == currentProcess);
                btn.Tag = proc;
                btn.Click += ProcessCard_Click;
                ProcessPanel.Children.Add(btn);
            }

            RefreshModuleList();
        }

        private void RefreshModuleList()
        {
            SelectedModulePanel.Children.Clear();
            currentModules.Clear();

            if (currentProcess == null)
            {
                SelectedModulePanel.Children.Add(cardFactory.CreateText("请选择流程", 14, Brushes.OrangeRed));
                return;
            }

            if (currentProcess.m_ModuleObjList != null)
            {
                currentModules.AddRange(currentProcess.m_ModuleObjList);
            }

            if (currentModules.Count == 0)
            {
                SelectedModulePanel.Children.Add(cardFactory.CreateText("当前流程暂无模块，点击右侧模块容器添加", 14, new SolidColorBrush(Color.FromRgb(126, 230, 255))));
                SetStatus("当前流程：" + currentProcess.processInfo.ProcessName + "，模块数：0");
                return;
            }

            foreach (IPlugin module in currentModules)
            {
                Button btn = cardFactory.CreateSelectedModuleCard(module, module == selectedModule);
                btn.Tag = module;
                btn.Click += ModuleCard_Click;
                btn.MouseDoubleClick += ModuleCard_DoubleClick;
                SelectedModulePanel.Children.Add(btn);
            }

            SetStatus("当前流程：" + currentProcess.processInfo.ProcessName + "，模块数：" + currentModules.Count);
        }

        private void ProcessCard_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            currentProcess = btn == null ? null : btn.Tag as Frame_Core.Process;
            selectedModule = null;
            RefreshProcessList(false);
        }

        private void PluginTile_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            pendingPluginName = btn == null ? null : btn.Tag as string;
            SetStatus("已选择模块：" + pendingPluginName + "，双击或点添加模块加入流程");
        }

        private void PluginTile_DoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Button btn = sender as Button;
            string pluginName = btn == null ? pendingPluginName : btn.Tag as string;
            if (string.IsNullOrWhiteSpace(pluginName)) return;

            pendingPluginName = pluginName;
            AddPluginToCurrentProcess(pluginName, true);
            e.Handled = true;
        }

        private void ModuleCard_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            selectedModule = btn == null ? null : btn.Tag as IPlugin;
            RefreshModuleList();
        }

        private void ModuleCard_DoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Button btn = sender as Button;
            selectedModule = btn == null ? null : btn.Tag as IPlugin;
            if (selectedModule != null) OpenPluginDialog(selectedModule);
        }

        private void BtnAddProcess_Click(object sender, RoutedEventArgs e)
        {
            int id = Project.Instance.CreateProcess();
            currentProcess = Project.Instance.GetProcessById(id);
            RefreshProcessList(false);
        }

        private void BtnDeleteProcess_Click(object sender, RoutedEventArgs e)
        {
            if (currentProcess == null) return;
            if (Project.Instance.m_ProcessList == null || Project.Instance.m_ProcessList.Count <= 1) return;

            Project.Instance.DeleteProcess(currentProcess);
            currentProcess = Project.Instance.m_ProcessList.Count > 0
                ? Project.Instance.m_ProcessList[Project.Instance.m_ProcessList.Count - 1].process
                : null;
            selectedModule = null;
            RefreshProcessList(false);
        }

        private void BtnRebuild_Click(object sender, RoutedEventArgs e)
        {
            if (System.Windows.MessageBox.Show("是否重建运行流程(ReBuild Process)？", "重建运行流程", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;

            Project.Instance.DeleteALLProcess();
            Project.Instance.baseProDevice.INIPro();
            Project.Instance.baseProDevice.BuildProcessServiceList();
            currentProcess = Project.Instance.m_ProcessList.Count > 0
                ? Project.Instance.m_ProcessList[Project.Instance.m_ProcessList.Count - 1].process
                : null;
            RefreshProcessList(true);
            SetStatus("流程已重建");
        }

        private void BtnLoad_Click(object sender, RoutedEventArgs e)
        {
            Project.Instance.ReadProject();
            currentProcess = Project.Instance.m_ProcessList.Count > 0
                ? Project.Instance.m_ProcessList[Project.Instance.m_ProcessList.Count - 1].process
                : null;
            selectedModule = null;
            RefreshProcessList(true);
            SetStatus("流程已加载");
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            Project.Instance.SaveProject();
            SetStatus("流程已保存");
        }

        private void BtnAddModule_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(pendingPluginName)) return;
            AddPluginToCurrentProcess(pendingPluginName, false);
        }

        private void BtnDeleteModule_Click(object sender, RoutedEventArgs e)
        {
            if (currentProcess == null || selectedModule == null) return;

            currentProcess.DeleteModuleObj(selectedModule);
            selectedModule = null;
            RefreshModuleList();
        }

        //private void BtnRunOnce_Click(object sender, RoutedEventArgs e)
        //{
        //    if (currentProcess == null) return;

        //    Project.Instance.ExecuteOnce(currentProcess.processInfo.ProcessID);
        //    RefreshModuleList();
        //}

        private async void BtnRunOnce_Click(object sender, RoutedEventArgs e)
        {
            if (currentProcess == null) return;

            await Task.Run(() =>
            {
                foreach (IPlugin module in currentProcess.m_ModuleObjList)
                {
                    if (module == null || module.moduleInfo == null) continue;
                    if (!module.moduleInfo.IsUse) continue;

                    module.ExecuteObj();
                }
            });

            RefreshModuleList();
        }
        private void BtnRunLoop_Click(object sender, RoutedEventArgs e)
        {
            if (currentProcess != null) Project.Instance.StartRun(currentProcess.processInfo.ProcessID);
        }

        private void BtnRunStop_Click(object sender, RoutedEventArgs e)
        {
            if (currentProcess != null) Project.Instance.StopRun(currentProcess.processInfo.ProcessID);
        }

        private void BtnToggleContainer_Click(object sender, RoutedEventArgs e)
        {
            pluginContainerVisible = !pluginContainerVisible;
            PluginContainerPanel.Visibility = pluginContainerVisible ? Visibility.Visible : Visibility.Collapsed;
            PluginContainerColumn.Width = pluginContainerVisible ? new GridLength(360) : new GridLength(0);
            BtnToggleContainerTop.Content = pluginContainerVisible ? "隐藏容器" : "显示容器";
        }

        private void AddPluginToCurrentProcess(string pluginName, bool openAfterAdd)
        {
            if (currentProcess == null || string.IsNullOrWhiteSpace(pluginName)) return;
            if (!PluginService.pluginsDictionary.ContainsKey(pluginName)) return;

            IPlugin plugin = PluginService.GetInstance<IPlugin>(PluginService.pluginsDictionary[pluginName].PluginsBuildName);
            if (plugin == null) return;

            plugin.moduleInfo.PluginName = pluginName;
            currentProcess.AddModuleObj(plugin);
            selectedModule = plugin;
            RefreshModuleList();

            if (openAfterAdd) OpenPluginDialog(plugin);
        }

        private void OpenPluginDialog(IPlugin plugin)
        {
            try
            {
                pluginDialogService.Open(plugin, Window.GetWindow(this), OnPluginActionCompleted);
                RefreshProcessList(false);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("打开模块界面失败：" + ex.Message, "WPF 调试", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnPluginActionCompleted(PluginViewActionResult result)
        {
            Dispatcher.BeginInvoke(new Action(RefreshModuleList));
        }

        private void SetStatus(string text)
        {
            if (StatusText != null) StatusText.Text = "状态：" + text;
        }
    }
}
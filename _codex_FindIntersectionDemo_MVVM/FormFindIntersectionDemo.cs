using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.IO;
using System.Reflection;

namespace Plugin.FindIntersection
{
    public class FormFindIntersectionDemo : Window
    {
        private readonly FindIntersectionDemoViewModel viewModel = new FindIntersectionDemoViewModel();
        private readonly ListView moduleList = new ListView();

        public FormFindIntersectionDemo()
        {
            Title = "WPF ProcessSetting Demo";
            Width = 1120;
            Height = 700;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Background = new SolidColorBrush(Color.FromRgb(7, 13, 20));

            DataContext = viewModel;
            Content = BuildLayout();

            Loaded += delegate { viewModel.LoadPluginsFromMef(); };
            moduleList.MouseDoubleClick += ModuleList_MouseDoubleClick;
        }

        private UIElement BuildLayout()
        {
            var root = new Grid { Margin = new Thickness(18) };
            root.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(330) });
            root.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            var left = new Border
            {
                BorderBrush = new SolidColorBrush(Color.FromRgb(0, 229, 255)),
                BorderThickness = new Thickness(1),
                Background = new SolidColorBrush(Color.FromRgb(12, 22, 32)),
                Padding = new Thickness(12)
            };
            Grid.SetColumn(left, 0);

            var stack = new DockPanel();
            var title = new TextBlock
            {
                Text = "插件模块",
                Foreground = new SolidColorBrush(Color.FromRgb(0, 229, 255)),
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 12)
            };
            DockPanel.SetDock(title, Dock.Top);
            stack.Children.Add(title);

            moduleList.BorderThickness = new Thickness(0);
            moduleList.Background = new SolidColorBrush(Color.FromRgb(8, 16, 24));
            moduleList.Foreground = Brushes.White;
            moduleList.ItemTemplate = BuildModuleTemplate();
            moduleList.SetBinding(ItemsControl.ItemsSourceProperty, "Plugins");
            moduleList.SetBinding(System.Windows.Controls.Primitives.Selector.SelectedItemProperty, "SelectedPlugin");
            stack.Children.Add(moduleList);
            left.Child = stack;

            var right = new Border
            {
                Margin = new Thickness(14, 0, 0, 0),
                BorderBrush = new SolidColorBrush(Color.FromRgb(30, 92, 116)),
                BorderThickness = new Thickness(1),
                Background = new SolidColorBrush(Color.FromRgb(9, 18, 28)),
                Padding = new Thickness(20)
            };
            Grid.SetColumn(right, 1);

            var info = new StackPanel();
            info.Children.Add(new TextBlock
            {
                Text = "WPF 版 UC_ProcessSetting",
                Foreground = Brushes.White,
                FontSize = 24,
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 0, 0, 18)
            });
            info.Children.Add(new TextBlock
            {
                Text = "模块列表来自 PluginService.pluginsDictionary。双击模块时自动识别窗体类型：WPFFormPluginBase/Window 用 WPF 模态弹出，WinForms/FormPluginBase 用 WinForms 模态弹出。",
                Foreground = new SolidColorBrush(Color.FromRgb(158, 190, 204)),
                FontSize = 15,
                TextWrapping = TextWrapping.Wrap
            });
            right.Child = info;

            root.Children.Add(left);
            root.Children.Add(right);
            return root;
        }

        private void ModuleList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (viewModel.SelectedPlugin == null) return;
            OpenPluginDialog(viewModel.SelectedPlugin);
        }

        private void OpenPluginDialog(DemoPluginInfo pluginInfo)
        {
            if (pluginInfo == null) return;

            if (pluginInfo.BuildName == "FindIntersectionObj" || pluginInfo.DisplayName.Contains("交点"))
            {
                OpenWpfDialog(new FindIntersectionDemoDialog(pluginInfo));
                return;
            }

            var formType = pluginInfo.FormType;
            if (formType == null)
            {
                OpenWpfDialog(new FindIntersectionDemoDialog(pluginInfo));
                return;
            }

            if (typeof(Window).IsAssignableFrom(formType))
            {
                OpenWpfDialog((Window)System.Activator.CreateInstance(formType));
                return;
            }

            if (typeof(System.Windows.Forms.Form).IsAssignableFrom(formType))
            {
                using (var form = (System.Windows.Forms.Form)System.Activator.CreateInstance(formType))
                {
                    var helper = new WindowInteropHelper(this);
                    form.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
                    form.ShowDialog(new Win32WindowOwner(helper.Handle));
                }
                return;
            }

            OpenWpfDialog(new FindIntersectionDemoDialog(pluginInfo));
        }

        private bool OpenOriginalFindIntersectionForm()
        {
            try
            {
                string pluginPath = @"D:\BaiduNetdiskDownload\宽幅打孔新方案1107\宽幅打孔新方案1017\ProcessWideHoleLaser\Mef_VisionFrame\bin\Release\Plugins\Plugin.FindIntersection.dll";
                if (!File.Exists(pluginPath))
                {
                    pluginPath = @"D:\BaiduNetdiskDownload\宽幅打孔新方案1107\宽幅打孔新方案1017\ProcessWideHoleLaser\Mef_VisionFrame\bin\Release\Plugin.FindIntersection.dll";
                }

                if (!File.Exists(pluginPath)) return false;

                Assembly assembly = Assembly.LoadFrom(pluginPath);
                Type formType = assembly.GetType("Plugin.FindIntersection.FormFindIntersection");
                if (formType == null) return false;

                using (var form = (System.Windows.Forms.Form)Activator.CreateInstance(formType))
                {
                    var helper = new WindowInteropHelper(this);
                    form.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
                    form.ShowDialog(new Win32WindowOwner(helper.Handle));
                }

                return true;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("打开原 FindIntersection 窗体失败：" + ex.Message, "FindIntersection", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
        }

        private void OpenWpfDialog(Window dialog)
        {
            dialog.Owner = this;
            dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dialog.ShowDialog();
        }

        private static DataTemplate BuildModuleTemplate()
        {
            var stack = new FrameworkElementFactory(typeof(StackPanel));
            stack.SetValue(StackPanel.MarginProperty, new Thickness(8));

            var name = new FrameworkElementFactory(typeof(TextBlock));
            name.SetBinding(TextBlock.TextProperty, new System.Windows.Data.Binding("DisplayName"));
            name.SetValue(TextBlock.ForegroundProperty, Brushes.White);
            name.SetValue(TextBlock.FontSizeProperty, 15.0);
            name.SetValue(TextBlock.FontWeightProperty, FontWeights.SemiBold);
            stack.AppendChild(name);

            var meta = new FrameworkElementFactory(typeof(TextBlock));
            meta.SetBinding(TextBlock.TextProperty, new System.Windows.Data.Binding("UiKind") { StringFormat = "界面类型：{0}" });
            meta.SetValue(TextBlock.ForegroundProperty, new SolidColorBrush(Color.FromRgb(130, 190, 205)));
            meta.SetValue(TextBlock.FontSizeProperty, 12.0);
            stack.AppendChild(meta);

            return new DataTemplate { VisualTree = stack };
        }

        private sealed class Win32WindowOwner : System.Windows.Forms.IWin32Window
        {
            public Win32WindowOwner(System.IntPtr handle)
            {
                Handle = handle;
            }

            public System.IntPtr Handle { get; private set; }
        }
    }
}

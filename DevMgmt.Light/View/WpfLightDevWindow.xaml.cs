using DevMgmt.Light.Models;
using DevMgmt.Light.Tool;
using DevMgmt.Light.ViewModels;
using FrameCommunity;
using FrameCommunity.UserControls;
using FrameCommunity.语言;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace DevMgmt.Light.View
{
    /// <summary>
    /// WpfLightDevWindow.xaml 的交互逻辑
    /// </summary>
    public partial class WpfLightDevWindow : Window
    {
        private readonly LightControllerViewModel ViewModel;
        public WpfLightDevWindow(LightControllerParam param)
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            ViewModel = new LightControllerViewModel(param);
            DataContext = ViewModel;

            // ListBox选中联动右侧参数
            ViewModel.SelectSelectionIndex += SelectionChanged;
            ViewModel.UpdateUI += RefreshRightParameterPanel;
            DevItemPanel.SelectionChanged += DevItemPanel_SelectionChanged;

            //
            ViewModel.SelectSelectionIndex(ViewModel.Param.l_LightControllerDevs.Count - 1);
        }
        public void SelectionChanged(int index)
        {
            DevItemPanel.SelectedIndex = index;
            
        }

        // ListBox选中变化，同步右侧参数
        private void DevItemPanel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DevItemPanel.SelectedItem is LightControllerDev dev)
            {
                RefreshRightParameterPanel(dev);
            }
            else
            {
                DevParameterPanel.Children.Clear();
            }
        }


        #region 刷新右侧参数面板（根据选中设备动态生成控件）
        /// <summary>根据当前选中参数，动态构建右侧编辑面板</summary>
        private void RefreshRightParameterPanel(LightControllerDev dev)
        {
            dev.LightControllerParameter.InitControlDisplay();
            DevParameterPanel.DataContext = dev.LightControllerParameter;
            DevParameterPanel.Children.Clear();

            var param = dev.LightControllerParameter;
            if (param == null) return;


            Thickness thicknessSubitem = new Thickness(50, 0, 0, 0);
            int height = 35;

            PropertyInfo[] props = typeof(LightControllerParameterBase)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // 1. 收集所有需要显示的配置项
            var allDisplayItems = new List<(PropertyInfo Prop, ControlDisplayBase Cfg)>();
            foreach (var prop in props)
            {
                if (!param.ControlDisplay.TryGetValue(prop.Name, out var displayCfg))
                    continue;
                if (!displayCfg.IsShow)
                    continue;
                allDisplayItems.Add((prop, displayCfg));
            }

            // 2. 按 ParentName 分组
            var groupDict = allDisplayItems
         .GroupBy(x => x.Cfg.ParentName)
         // 取组内第一个配置的权重作为分组排序依据
         .OrderBy(g => g.Min(item => item.Cfg.GroupSortOrder));

            // 记录已创建分组标题，防止重复渲染
            var createdGroupTitles = new HashSet<string>();

            foreach (var group in groupDict)
            {
                string parentTitle = group.Key;
                // 渲染分组标题（ParentName）
                if (!createdGroupTitles.Contains(parentTitle) && !string.IsNullOrEmpty(parentTitle))
                {
                    createdGroupTitles.Add(parentTitle);
                    var firstDisplayCfg = group.First().Cfg;

                    // 判断当前分组是不是【光源参数】，是则标题右侧追加 + - 按钮
                    if (parentTitle == "光源参数")
                    {
                        // 标题行容器：水平布局
                        StackPanel groupTitleBar = new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            VerticalAlignment = VerticalAlignment.Center,
                            Margin = new Thickness(0, 15, 0, 8)
                        };

                        // 左侧分组标题文字
                        TextBlock groupHeader = new TextBlock
                        {
                            Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#E9FBFF"),
                            FontSize = 14,
                            FontWeight = FontWeights.SemiBold,
                        };
                        groupHeader.SetBinding(TextBlock.TextProperty, new Binding(nameof(ControlDisplayBase.ParentName))
                        {
                            Source = firstDisplayCfg
                        });

                        // 弹性空白，把按钮推到最右侧
                        FrameworkElement fillSpacer = new FrameworkElement();
                        fillSpacer.HorizontalAlignment = HorizontalAlignment.Stretch;
                        fillSpacer.Width = double.NaN;

                        // 加号新增通道按钮
                        Button btnAddChannel = new Button
                        {
                            Content = "+",
                            Width = 15,
                            
                            Margin = new Thickness(15, 0, 15, 0),
                            BorderBrush = new SolidColorBrush(Color.FromRgb(0, 169, 200)),
                            Background = new SolidColorBrush(Color.FromRgb(16, 37, 54)),
                            Foreground = new SolidColorBrush(Color.FromRgb(220, 247, 255)),
                            BorderThickness = new Thickness(1),
                            Command = ViewModel.AddLightChannel,
                            CommandParameter= dev
                        };
                      

                        // 减号删除通道按钮
                        Button btnDelChannel = new Button
                        {
                            Content = "-",
                            Width = 15,
                          
                            BorderBrush = new SolidColorBrush(Color.FromRgb(0, 169, 200)),
                            Background = new SolidColorBrush(Color.FromRgb(16, 37, 54)),
                            Foreground = new SolidColorBrush(Color.FromRgb(220, 247, 255)),
                            BorderThickness = new Thickness(1),
                            Command = ViewModel.RemoveLightChannel,
                            CommandParameter = dev
                        };               

                        // 组装标题栏
                        groupTitleBar.Children.Add(groupHeader);
                        //groupTitleBar.Children.Add(fillSpacer);
                        groupTitleBar.Children.Add(btnAddChannel);
                        groupTitleBar.Children.Add(btnDelChannel);
                        DevParameterPanel.Children.Add(groupTitleBar);
                    }
                    else
                    {
                        // 其他普通分组，保持原有纯文字标题
                        TextBlock groupHeader = new TextBlock
                        {
                            Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#E9FBFF"),
                            FontSize = 14,
                            Margin = new Thickness(0, 15, 0, 8),
                            FontWeight = FontWeights.SemiBold,
                        };
                        groupHeader.SetBinding(TextBlock.TextProperty, new Binding(nameof(ControlDisplayBase.ParentName))
                        {
                            Source = firstDisplayCfg
                        });
                        DevParameterPanel.Children.Add(groupHeader);
                    }
                }

                // 遍历当前分组内所有参数
                foreach (var (prop, displayCfg) in group)
                {
                    StackPanel itemRoot = new StackPanel { Margin = thicknessSubitem };

                    // 单行参数标签（DisplayName）
                    TextBlock title = new TextBlock
                    {
                        Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#DCF8FF"),

                    };
                    title.SetBinding(TextBlock.TextProperty, new Binding(nameof(ControlDisplayBase.DisplayName))
                    {
                        Source = displayCfg
                    });
                    itemRoot.Children.Add(title);

                    FrameworkElement inputElement = null;
                    Binding binding = new Binding(prop.Name) { Mode = BindingMode.TwoWay };

                    #region 字符串类型
                    if (prop.PropertyType == typeof(string))
                    {
                        if (displayCfg.ShowType == ControlShowTypeEnum.TextBoxAndSelectFile || displayCfg.ShowType == ControlShowTypeEnum.TextBoxAndSelectFolder)
                        {
                            Grid grid = new Grid { Height = height };
                            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                            TextBox txt = new TextBox { IsEnabled = displayCfg.IsEnabled };
                            txt.SetBinding(TextBox.TextProperty, binding);
                            Grid.SetColumn(txt, 0);
                            grid.Children.Add(txt);

                            if (displayCfg.IsEnabled)
                            {
                                Button btn = new Button { Style = Resources["PathSelectBtnStyle"] as Style };
                                Path icon = new Path
                                {
                                    Width = 16,
                                    Height = 16,
                                    Fill = (SolidColorBrush)new BrushConverter().ConvertFrom("#DCF8FF"),
                                    Data = Geometry.Parse("M5,3 H31 V9 H9 L5,6 Z M9,9 H33 V31 H9 Z")
                                };
                                btn.Content = icon;
                            
                                var paramTuple = Tuple.Create(param, prop.Name);
                                btn.CommandParameter = paramTuple;
                                if(displayCfg.ShowType == ControlShowTypeEnum.TextBoxAndSelectFile)
                                btn.Command = ViewModel.SelectFile;
                                else if(displayCfg.ShowType == ControlShowTypeEnum.TextBoxAndSelectFolder)
                                    btn.Command = ViewModel.SelectFolder;

                                Grid.SetColumn(btn, 1);
                                grid.Children.Add(btn);
                            }
                            inputElement = grid;
                        }
                        else
                        {
                            TextBox txt = new TextBox { IsEnabled = displayCfg.IsEnabled };
                            txt.SetBinding(TextBox.TextProperty, binding);
                            inputElement = txt;
                        }
                    }
                    #endregion

                    #region 数值类型
                    else if (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(double))
                    {
                        if (displayCfg.ShowType == ControlShowTypeEnum.ComboBox)
                        {
                            ComboBox cbx = new ComboBox
                            {
                                IsEnabled = displayCfg.IsEnabled,
                                Height = height,
                                VerticalContentAlignment = VerticalAlignment.Center,
                              
                            };

                            Binding cbxBinding = new Binding(prop.Name)
                            {
                                Mode = BindingMode.TwoWay,
                                Source = param,
                                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                            };

                            cbx.ItemsSource = displayCfg.ComboBoxItems;
                            cbx.DisplayMemberPath = "Text";
                            cbx.SelectedValuePath = "Value";
                            cbx.SetBinding(ComboBox.SelectedValueProperty, cbxBinding);

                            inputElement = cbx;
                        }

                        else
                        {
                            var numBox = new NumericBox
                            {
                                Height = height,
                                DecimalPlaces = displayCfg.DecimalPlaces,
                                Increment = displayCfg.Increment,
                                Minimum = displayCfg.MinValue,
                                Maximum = displayCfg.MaxValue,
                                IsEnabled = displayCfg.IsEnabled,
                                TextHorizontalContentAlignment= HorizontalAlignment.Left,
                            };
                            numBox.SetBinding(NumericBox.ValueProperty, binding);
                            inputElement = numBox;
                        }
                    }
                    #endregion

                    #region 光源通道集合 ListBox 两列布局：开关Button + 亮度NumericBox
                    else if (displayCfg.ShowType == ControlShowTypeEnum.NumericBoxAndBotton)
                    {                     
                        // 循环遍历所有通道项
                        foreach (var channelItem in dev.LightControllerParameter.LightChannelItems)
                        {
                            Grid rowGrid = new Grid() { Margin = thicknessSubitem };
                            //rowGrid.Height = 70;
                            rowGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30) });
                            rowGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(35) });
                            rowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(40) });
                            rowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                            // 第一行：通道子标题 DisplayName
                            TextBlock tbTitle = new TextBlock
                            {
                                VerticalAlignment = VerticalAlignment.Center,
                                FontSize = 13,
                                Foreground = new SolidColorBrush(Color.FromRgb(220, 247, 255))
                            };
                            tbTitle.SetBinding(TextBlock.TextProperty, new Binding(nameof(LightChannelItem.DisplayName)) { Source = channelItem });
                            Grid.SetRow(tbTitle, 0);
                            Grid.SetColumnSpan(tbTitle, 2);
                            rowGrid.Children.Add(tbTitle);

                            // 第二行左：开关Button
                            Button btnSwitch = new Button
                            {
                                Height = height,
                                Padding = new Thickness(0),
                                VerticalAlignment = VerticalAlignment.Center,
                                BorderBrush = new SolidColorBrush(Color.FromRgb(0, 169, 200)),
                                BorderThickness = new Thickness(1),
                                Foreground = new SolidColorBrush(Color.FromRgb(220, 247, 255)),
                                Command = ViewModel.OepnClosingLight
                            };
                            Grid.SetRow(btnSwitch, 1);
                            Grid.SetColumn(btnSwitch, 0);

                            Binding bgBinding = new Binding(nameof(LightChannelItem.SwitchState))
                            {
                                Source = channelItem,
                                Mode = BindingMode.OneWay,
                                Converter = new DevMgmt.Light.Tool.BoolToStatusBrushConverter()
                            };
                            btnSwitch.SetBinding(Button.BackgroundProperty, bgBinding);

                            Binding textBinding = new Binding(nameof(LightChannelItem.SwitchState))
                            {
                                Source = channelItem,
                                Mode = BindingMode.OneWay,
                                Converter = new DevMgmt.Light.Tool.BoolOpenClosingConnectTextConverter()
                            };
                            btnSwitch.SetBinding(Button.ContentProperty, textBinding);

                      
                            btnSwitch.CommandParameter = Tuple.Create(dev, channelItem) ;
                            rowGrid.Children.Add(btnSwitch);

                            // 第二行右：亮度NumericBox
                            NumericBox numBox = new NumericBox
                            {
                                Height = height,
                                Margin = new Thickness(10, 0, 0, 0),
                                DecimalPlaces = displayCfg.DecimalPlaces,
                                Increment = displayCfg.Increment,
                                Minimum = displayCfg.MinValue,
                                Maximum = displayCfg.MaxValue,
                                TextHorizontalContentAlignment = HorizontalAlignment.Left,
                            };
                            Grid.SetRow(numBox, 1);
                            Grid.SetColumn(numBox, 1);

                            Binding valueBinding = new Binding(nameof(LightChannelItem.Brightness))
                            {
                                Source = channelItem,
                                Mode = BindingMode.TwoWay,
                                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                            };
                            numBox.SetBinding(NumericBox.ValueProperty, valueBinding);
                            rowGrid.Children.Add(numBox);

                            DevParameterPanel.Children.Add(rowGrid);
                        }
                      
                      
                    }
                    #endregion

                    if (inputElement != null)
                    {
                        itemRoot.Children.Add(inputElement);
                        DevParameterPanel.Children.Add(itemRoot);
                    }
                }
            }
        }

        #endregion

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            BindingOperations.ClearAllBindings(this);  // 清除所有子元素的绑定
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            //this.DataContext = null;
        }
  
    }
    public class BindingProxy : Freezable
    {
        protected override Freezable CreateInstanceCore() => new BindingProxy();

        public object Data
        {
            get => GetValue(DataProperty);
            set => SetValue(DataProperty, value);
        }

        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register(nameof(Data), typeof(object), typeof(BindingProxy));
    }
}


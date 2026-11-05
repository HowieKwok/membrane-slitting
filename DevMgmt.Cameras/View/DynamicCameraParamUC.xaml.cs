using DevMgmt.Cameras.Models;
using DevMgmt.Cameras.Tool;
using FrameCommunity;
using FrameCommunity.UserControls;

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DevMgmt.Cameras.View
{
    /// <summary>
    /// DynamicCameraParamUC.xaml 的交互逻辑
    /// </summary>
    public partial class DynamicCameraParamUC : UserControl
    {
        public DynamicCameraParamUC()
        {
            InitializeComponent();
            Loaded += DynamicCameraParamUC_Loaded;
        }
        private void DynamicCameraParamUC_Loaded(object sender, RoutedEventArgs e)
        {
            GenerateControls();
        }
        private void GenerateControls()
        {
            MainStackPanel.Children.Clear();
            if (!(DataContext is CameraParameterBase param))
                return;

            PropertyInfo[] props = typeof(CameraParameterBase)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in props)
            {
                if (!param.ControlDisplay.TryGetValue(prop.Name, out var displayCfg))
                    continue;

                if (!displayCfg.IsShow)
                    continue;

                StackPanel itemRoot = new StackPanel { Margin = new Thickness(0, 0, 0, 10) };
                itemRoot.Children.Add(new TextBlock
                {
                    Text = displayCfg.DisplayName,
                    Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#DCF8FF")
                });

                FrameworkElement inputElement = null;
                Binding binding = new Binding(prop.Name) { Mode = BindingMode.TwoWay };

                #region 字符串类型
                if (prop.PropertyType == typeof(string))
                {
                    if (displayCfg.ShowType == ControlShowTypeEnum.TextBoxAndSelectFile || displayCfg.ShowType == ControlShowTypeEnum.TextBoxAndSelectFolder)
                    {
                        Grid grid = new Grid { Height = 35 };
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
                            btn.Tag = new Tuple<TextBox, ControlShowTypeEnum>(txt, displayCfg.ShowType);
                            btn.Click += PathSelectBtn_Click;

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

                #region 数值类型（根据 UseTextBox 自动区分控件）
                else if (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(double))
                {
                   

                    // 配置为使用文本框
                    if (displayCfg.ShowType == ControlShowTypeEnum.TextBox)
                    {
                        // 绑定转换器逻辑
                        if (prop.Name == nameof(CameraParameterBase.DevType))
                        {
                            binding.Converter = DevTypeConverter.Instance;
                        }

                        TextBox txt = new TextBox
                        {
                            IsEnabled = displayCfg.IsEnabled
                        };
                        txt.SetBinding(TextBox.TextProperty, binding);
                        inputElement = txt;
                    }
                    // 默认使用 NumericBox
                    else
                    {
                        var numBox = new NumericBox
                        {
                            DecimalPlaces = displayCfg.DecimalPlaces,
                            Increment = displayCfg.Increment,
                            Minimum = displayCfg.MinValue,
                            Maximum = displayCfg.MaxValue,
                            IsEnabled = displayCfg.IsEnabled,
                            TextHorizontalContentAlignment = HorizontalAlignment.Left,
                        };
                        numBox.SetBinding(NumericBox.ValueProperty, binding);
                        inputElement = numBox;
                    }
                }
                #endregion

                if (inputElement != null)
                {
                    itemRoot.Children.Add(inputElement);
                    MainStackPanel.Children.Add(itemRoot);
                }
            }
        }
        /// <summary> 文件/文件夹选择按钮点击事件（原有逻辑不变） </summary>
        private void PathSelectBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button btn) || !(btn.Tag is Tuple<TextBox, ControlShowTypeEnum> data))
                return;

            TextBox txt = data.Item1;
            ControlShowTypeEnum boxType = data.Item2;

            if (boxType == ControlShowTypeEnum.TextBoxAndSelectFile)
            {
                // 1. 选择文件（对应原来的 Button_Click 逻辑）
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "配置文件 (*.config;*.xml;*.cam)|*.config;*.xml;*.cam|所有文件 (*.*)|*.*",
                    Title = "选择相机配置文件",
                    Multiselect = false
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    txt.Text = openFileDialog.FileName;
                    // 安全更新数据源
                    var bindingExpr = txt.GetBindingExpression(TextBox.TextProperty);
                    bindingExpr?.UpdateSource();
                }
            }
            else if (boxType == ControlShowTypeEnum.TextBoxAndSelectFolder)
            {
                // 2. 选择文件夹（对应原来的 Btn_SelectEmulationPath_Click 逻辑）
                using (System.Windows.Forms.FolderBrowserDialog folderDialog = new System.Windows.Forms.FolderBrowserDialog())
                {
                    folderDialog.Description = "请选择仿真图像目录";
                    // 可选：默认打开当前已选路径
                    if (!string.IsNullOrEmpty(txt.Text))
                    {
                        folderDialog.SelectedPath = txt.Text;
                    }

                    if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        txt.Text = folderDialog.SelectedPath;
                        // 安全更新数据源
                        var bindingExpr = txt.GetBindingExpression(TextBox.TextProperty);
                        bindingExpr?.UpdateSource();
                    }
                }
            }
        }
    }
}

using Frame_Core;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using ShapeLine = System.Windows.Shapes.Line;

namespace Mef_VisionFrame.调试设置.WPF
{
    internal sealed class DebugProcessCardFactory
    {
        private readonly ModuleRunStateReader stateReader;

        public DebugProcessCardFactory(ModuleRunStateReader stateReader)
        {
            this.stateReader = stateReader;
        }

        public Button CreateProcessCard(Frame_Core.Process proc, bool selected)
        {
            StackPanel stack = new StackPanel();
            StackPanel header = new StackPanel { Orientation = Orientation.Horizontal };
            header.Children.Add(CreateBadge("相机", selected ? new SolidColorBrush(Color.FromRgb(66, 255, 158)) : new SolidColorBrush(Color.FromRgb(106, 135, 148))));
            header.Children.Add(CreateText(proc.processInfo.ProcessName, 15, Brushes.White));
            stack.Children.Add(header);
            stack.Children.Add(CreateText("流程 " + proc.processInfo.ProcessID + " / 模块 " + proc.m_ModuleObjList.Count, 12, new SolidColorBrush(Color.FromRgb(126, 230, 255))));
            return CreateCardButton(stack, selected ? new SolidColorBrush(Color.FromRgb(0, 229, 255)) : new SolidColorBrush(Color.FromRgb(37, 95, 114)), selected ? new SolidColorBrush(Color.FromRgb(16, 33, 49)) : new SolidColorBrush(Color.FromRgb(11, 20, 30)));
        }

        public Button CreatePluginTile(string name, string category, bool selected)
        {
            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(46) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            Border icon = CreateModuleIcon(category, false);
            Grid.SetColumn(icon, 0);
            grid.Children.Add(icon);

            StackPanel text = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
            text.Children.Add(CreateText(name, 13, Brushes.White));
            text.Children.Add(CreateText(string.IsNullOrWhiteSpace(category) ? "未分类" : category, 11, new SolidColorBrush(Color.FromRgb(126, 230, 255))));
            Grid.SetColumn(text, 1);
            grid.Children.Add(text);

            return CreateCardButton(grid, selected ? new SolidColorBrush(Color.FromRgb(255, 46, 154)) : new SolidColorBrush(Color.FromRgb(28, 86, 105)), new SolidColorBrush(Color.FromRgb(10, 26, 39)));
        }

        public Button CreateSelectedModuleCard(IPlugin module, bool selected)
        {
            string pluginName = module.moduleInfo.PluginName;
            string category = PluginService.pluginsDictionary.ContainsKey(pluginName) ? PluginService.pluginsDictionary[pluginName].PluginsCategory : string.Empty;

            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(46) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(122) });

            Border icon = CreateModuleIcon(category, true);
            Grid.SetColumn(icon, 0);
            grid.Children.Add(icon);

            StackPanel stack = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
            stack.Children.Add(CreateText(module.moduleInfo.ModuleName, 15, Brushes.White));
            stack.Children.Add(CreateText("插件：" + module.moduleInfo.PluginName, 12, new SolidColorBrush(Color.FromRgb(126, 230, 255))));
            stack.Children.Add(CreateText("编号：" + module.moduleInfo.ModuleEncode + " / 启用：" + module.moduleInfo.IsUse, 12, new SolidColorBrush(Color.FromRgb(174, 199, 211))));
            Grid.SetColumn(stack, 1);
            grid.Children.Add(stack);

            StackPanel runPanel = CreateRunStatusPanel(module);
            Grid.SetColumn(runPanel, 2);
            grid.Children.Add(runPanel);

            return CreateCardButton(grid, selected ? new SolidColorBrush(Color.FromRgb(255, 46, 154)) : new SolidColorBrush(Color.FromRgb(37, 95, 114)), selected ? new SolidColorBrush(Color.FromRgb(30, 20, 38)) : new SolidColorBrush(Color.FromRgb(11, 20, 30)));
        }

        public TextBlock CreateText(string text, double size, Brush brush)
        {
            return new TextBlock
            {
                Text = text,
                FontSize = size,
                Foreground = brush,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 1, 0, 1)
            };
        }

        private Button CreateCardButton(object content, Brush border, Brush background)
        {
            return new Button
            {
                Content = content,
                HorizontalContentAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 0, 0, 10),
                Padding = new Thickness(12),
                BorderThickness = new Thickness(1),
                BorderBrush = border,
                Background = background,
                Foreground = Brushes.White,
                Cursor = System.Windows.Input.Cursors.Hand
            };
        }

        private Border CreateModuleIcon(string category, bool moduleInstance)
        {
            Brush color = GetCategoryBrush(category);
            Grid iconGrid = new Grid { Width = 30, Height = 30 };
            iconGrid.Children.Add(new Ellipse { Stroke = color, StrokeThickness = 1.6, Fill = new SolidColorBrush(Color.FromRgb(8, 24, 34)), Width = 30, Height = 30 });

            FrameworkElement shape = CreateCategoryShape(category, moduleInstance, color);
            shape.HorizontalAlignment = HorizontalAlignment.Center;
            shape.VerticalAlignment = VerticalAlignment.Center;
            iconGrid.Children.Add(shape);

            return new Border
            {
                Width = 38,
                Height = 38,
                Margin = new Thickness(0, 0, 8, 0),
                BorderBrush = color,
                BorderThickness = new Thickness(1),
                Background = new SolidColorBrush(Color.FromRgb(6, 18, 28)),
                CornerRadius = new CornerRadius(8),
                Child = iconGrid
            };
        }

        private FrameworkElement CreateCategoryShape(string category, bool moduleInstance, Brush brush)
        {
            string c = category ?? string.Empty;
            if (c.Contains("定位") || c.Contains("找") || c.Contains("匹配"))
            {
                Grid target = new Grid { Width = 18, Height = 18 };
                target.Children.Add(new Ellipse { Stroke = brush, StrokeThickness = 1.8, Width = 18, Height = 18 });
                target.Children.Add(new Ellipse { Stroke = brush, StrokeThickness = 1.4, Width = 8, Height = 8, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center });
                target.Children.Add(new ShapeLine { X1 = 9, Y1 = 0, X2 = 9, Y2 = 18, Stroke = brush, StrokeThickness = 1 });
                target.Children.Add(new ShapeLine { X1 = 0, Y1 = 9, X2 = 18, Y2 = 9, Stroke = brush, StrokeThickness = 1 });
                return target;
            }

            if (c.Contains("测量") || c.Contains("尺寸") || c.Contains("角度"))
            {
                Grid ruler = new Grid { Width = 20, Height = 16 };
                ruler.Children.Add(new ShapeLine { X1 = 2, Y1 = 12, X2 = 18, Y2 = 4, Stroke = brush, StrokeThickness = 2 });
                ruler.Children.Add(new ShapeLine { X1 = 5, Y1 = 10, X2 = 5, Y2 = 15, Stroke = brush, StrokeThickness = 1.2 });
                ruler.Children.Add(new ShapeLine { X1 = 10, Y1 = 8, X2 = 10, Y2 = 13, Stroke = brush, StrokeThickness = 1.2 });
                ruler.Children.Add(new ShapeLine { X1 = 15, Y1 = 5, X2 = 15, Y2 = 10, Stroke = brush, StrokeThickness = 1.2 });
                return ruler;
            }

            if (c.Contains("检测") || c.Contains("缺陷") || c.Contains("Blob"))
            {
                Grid lens = new Grid { Width = 20, Height = 20 };
                lens.Children.Add(new Ellipse { Stroke = brush, StrokeThickness = 1.8, Width = 15, Height = 15, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top });
                lens.Children.Add(new ShapeLine { X1 = 13, Y1 = 13, X2 = 19, Y2 = 19, Stroke = brush, StrokeThickness = 2 });
                lens.Children.Add(new Ellipse { Fill = brush, Width = 4, Height = 4, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center });
                return lens;
            }

            if (c.Contains("标定"))
            {
                Grid check = new Grid { Width = 18, Height = 18 };
                for (int r = 0; r < 3; r++)
                {
                    for (int col = 0; col < 3; col++)
                    {
                        check.Children.Add(new Rectangle { Width = 4, Height = 4, Fill = (r + col) % 2 == 0 ? brush : Brushes.Transparent, Stroke = brush, StrokeThickness = 0.8, Margin = new Thickness(col * 6, r * 6, 0, 0), HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top });
                    }
                }
                return check;
            }

            Grid blocks = new Grid { Width = 18, Height = 18 };
            blocks.Children.Add(new Rectangle { Width = 7, Height = 7, RadiusX = 1, RadiusY = 1, Fill = brush, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top });
            blocks.Children.Add(new Rectangle { Width = 7, Height = 7, RadiusX = 1, RadiusY = 1, Fill = brush, HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Top });
            blocks.Children.Add(new Rectangle { Width = 7, Height = 7, RadiusX = 1, RadiusY = 1, Fill = brush, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Bottom });
            blocks.Children.Add(new Rectangle { Width = 7, Height = 7, RadiusX = 1, RadiusY = 1, Fill = moduleInstance ? brush : Brushes.Transparent, Stroke = brush, StrokeThickness = 1, HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Bottom });
            return blocks;
        }

        private StackPanel CreateRunStatusPanel(IPlugin module)
        {
            StackPanel panel = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center
            };

            panel.Children.Add(CreateRunStatusBadge(module));
            panel.Children.Add(new TextBlock
            {
                Text = "耗时：" + stateReader.GetElapsedText(module),
                FontSize = 11,
                Foreground = new SolidColorBrush(Color.FromRgb(174, 199, 211)),
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 4, 0, 0)
            });

            return panel;
        }

        private Border CreateRunStatusBadge(IPlugin module)
        {
            ModuleRunState state = stateReader.GetState(module);
            string text = state == ModuleRunState.Success ? "成功" : (state == ModuleRunState.Failed ? "失败" : "未执行");
            Color dotColor = state == ModuleRunState.Success ? Color.FromRgb(65, 255, 150) : (state == ModuleRunState.Failed ? Color.FromRgb(255, 70, 86) : Color.FromRgb(122, 141, 153));
            Color borderColor = state == ModuleRunState.Success ? Color.FromRgb(78, 210, 142) : (state == ModuleRunState.Failed ? Color.FromRgb(216, 78, 89) : Color.FromRgb(92, 112, 126));
            Color textColor = state == ModuleRunState.Success ? Color.FromRgb(190, 255, 219) : (state == ModuleRunState.Failed ? Color.FromRgb(255, 197, 202) : Color.FromRgb(184, 202, 213));

            Grid content = new Grid { Height = 22 };
            content.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(14) });
            content.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            content.Children.Add(new Ellipse { Width = 8, Height = 8, Fill = new SolidColorBrush(dotColor), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center });

            TextBlock label = new TextBlock { Text = text, FontSize = 12, Foreground = new SolidColorBrush(textColor), VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(2, 0, 0, 0) };
            Grid.SetColumn(label, 1);
            content.Children.Add(label);

            return new Border
            {
                MinWidth = 70,
                Height = 26,
                Padding = new Thickness(8, 1, 8, 1),
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
                CornerRadius = new CornerRadius(13),
                BorderThickness = new Thickness(1),
                BorderBrush = new SolidColorBrush(borderColor),
                Background = new SolidColorBrush(Color.FromRgb(7, 17, 27)),
                Child = content,
                ToolTip = stateReader.GetToolTip(module, state)
            };
        }

        private Border CreateBadge(string text, Brush brush)
        {
            return new Border
            {
                Height = 22,
                MinWidth = 38,
                Margin = new Thickness(0, 0, 8, 0),
                Padding = new Thickness(6, 1, 6, 1),
                BorderThickness = new Thickness(1),
                BorderBrush = brush,
                Background = new SolidColorBrush(Color.FromRgb(7, 17, 27)),
                Child = new TextBlock { Text = text, FontSize = 12, Foreground = brush, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center }
            };
        }

        private Brush GetCategoryBrush(string category)
        {
            if (string.IsNullOrWhiteSpace(category)) return new SolidColorBrush(Color.FromRgb(126, 230, 255));
            if (category.Contains("相机")) return new SolidColorBrush(Color.FromRgb(66, 255, 158));
            if (category.Contains("测量") || category.Contains("尺寸")) return new SolidColorBrush(Color.FromRgb(0, 229, 255));
            if (category.Contains("检测") || category.Contains("瑕疵")) return new SolidColorBrush(Color.FromRgb(255, 46, 154));
            if (category.Contains("定位")) return new SolidColorBrush(Color.FromRgb(255, 196, 87));
            if (category.Contains("标定")) return new SolidColorBrush(Color.FromRgb(177, 129, 255));
            return new SolidColorBrush(Color.FromRgb(126, 230, 255));
        }
    }
}
using Plugin.TemplateMatch.Controls;
using Plugin.TemplateMatch.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Plugin.TemplateMatch.WPF架构
{
    public sealed class TemplateCreateParameterDialog : Window
    {
        public TemplateCreateParameterDialog()
        {
            Title = "创建模板参数";
            Width = 520;
            Height = 500;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            ResizeMode = ResizeMode.NoResize;
            Background = new SolidColorBrush(Color.FromRgb(16, 25, 35));
            Foreground = new SolidColorBrush(Color.FromRgb(234, 247, 255));
            Content = BuildRoot();
        }

        private Grid BuildRoot()
        {
            Grid root = new Grid { Margin = new Thickness(18) };
            root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            root.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            StackPanel header = new StackPanel { Margin = new Thickness(0, 0, 0, 14) };
            header.Children.Add(new TextBlock
            {
                Text = "创建模板参数",
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(35, 217, 255))
            });
            header.Children.Add(new TextBlock
            {
                Text = "调整参数后点确定，将按当前弹窗参数创建或重新创建模板。",
                Margin = new Thickness(0, 6, 0, 0),
                Foreground = new SolidColorBrush(Color.FromRgb(185, 216, 232))
            });
            Grid.SetRow(header, 0);
            root.Children.Add(header);

            Grid body = BuildBody();
            Grid.SetRow(body, 1);
            root.Children.Add(body);

            StackPanel footer = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 16, 0, 0)
            };
            Button ok = new Button { Content = "确定", IsDefault = true, Width = 88, Height = 32, Margin = new Thickness(8, 0, 0, 0) };
            ok.Click += Ok_Click;
            footer.Children.Add(ok);
            footer.Children.Add(new Button { Content = "取消", IsCancel = true, Width = 88, Height = 32, Margin = new Thickness(8, 0, 0, 0) });
            Grid.SetRow(footer, 2);
            root.Children.Add(footer);
            return root;
        }

        private Grid BuildBody()
        {
            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(130) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(120) });
            for (int i = 0; i < 9; i++)
                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(i == 0 ? 32 : 36) });

            AddText(grid, 0, 1, "当前设置", null, "#23D9FF");
            AddText(grid, 0, 2, "参考值", null, "#23D9FF");
            AddText(grid, 1, 0, "金字塔层数", null, null);
            AddNumeric(grid, 1, 1, "NumLevels", 0);
            AddText(grid, 1, 2, null, "SuggestNumLevels", "#F7D46A");
            AddText(grid, 2, 0, "起始角度", null, null);
            AddNumeric(grid, 2, 1, "AngleStartDeg", 3);
            AddText(grid, 3, 0, "角度范围", null, null);
            AddNumeric(grid, 3, 1, "AngleExtentDeg", 3);
            AddText(grid, 4, 0, "角度步长", null, null);
            AddNumeric(grid, 4, 1, "AngleStepDeg", 6);
            AddText(grid, 4, 2, null, "SuggestAngleStepDeg", "#F7D46A");
            AddText(grid, 5, 0, "优化方式", null, null);
            AddCombo(grid, 5, 1, "Optimization", "OptimizationOptions");
            AddText(grid, 5, 2, null, "SuggestOptimization", "#F7D46A");
            AddText(grid, 6, 0, "匹配特征", null, null);
            AddCombo(grid, 6, 1, "Metric", "MetricOptions");
            AddText(grid, 6, 2, null, "SuggestMetric", "#F7D46A");
            AddText(grid, 7, 0, "对比度", null, null);
            AddNumeric(grid, 7, 1, "Contrast", 0);
            AddText(grid, 7, 2, null, "SuggestContrast", "#F7D46A");
            AddText(grid, 8, 0, "最小对比度", null, null);
            AddNumeric(grid, 8, 1, "MinContrast", 0);
            AddText(grid, 8, 2, null, "SuggestMinContrast", "#F7D46A");
            return grid;
        }

        private void AddText(Grid grid, int row, int column, string literal, string bindingPath, string colorHex)
        {
            TextBlock tb = new TextBlock { Margin = new Thickness(0, 3, 0, 3), VerticalAlignment = VerticalAlignment.Center };
            if (colorHex != null)
                tb.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFromString(colorHex));
            if (bindingPath == null)
                tb.Text = literal;
            else
                tb.SetBinding(TextBlock.TextProperty, new System.Windows.Data.Binding(bindingPath));
            Grid.SetRow(tb, row);
            Grid.SetColumn(tb, column);
            grid.Children.Add(tb);
        }

        private void AddNumeric(Grid grid, int row, int column, string path, int decimals)
        {
            NumericBox box = new NumericBox { DecimalPlaces = decimals };
            box.SetBinding(NumericBox.ValueProperty, new System.Windows.Data.Binding(path) { Mode = System.Windows.Data.BindingMode.TwoWay, UpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged });
            Grid.SetRow(box, row);
            Grid.SetColumn(box, column);
            grid.Children.Add(box);
        }

        private void AddCombo(Grid grid, int row, int column, string selectedPath, string itemsPath)
        {
            ComboBox combo = new ComboBox();
            combo.SetBinding(ItemsControl.ItemsSourceProperty, new System.Windows.Data.Binding(itemsPath));
            combo.SetBinding(Selector.SelectedItemProperty, new System.Windows.Data.Binding(selectedPath) { Mode = System.Windows.Data.BindingMode.TwoWay });
            Grid.SetRow(combo, row);
            Grid.SetColumn(combo, column);
            grid.Children.Add(combo);
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}

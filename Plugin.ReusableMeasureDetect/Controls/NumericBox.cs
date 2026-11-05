using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Plugin.ReusableMeasureDetect.Controls
{
    /// <summary>
    /// 深色风格数值输入控件。
    /// 支持双向绑定 Value、最小/最大值限制、步进按钮、键盘上下键和粘贴校验，用来替代普通 TextBox 输入数值。
    /// </summary>
    public sealed class NumericBox : UserControl
    {
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                nameof(Value),
                typeof(double),
                typeof(NumericBox),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged));

        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(nameof(Minimum), typeof(double), typeof(NumericBox), new PropertyMetadata(-100000000.0, OnRangeChanged));

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(nameof(Maximum), typeof(double), typeof(NumericBox), new PropertyMetadata(100000000.0, OnRangeChanged));

        public static readonly DependencyProperty IncrementProperty =
            DependencyProperty.Register(nameof(Increment), typeof(double), typeof(NumericBox), new PropertyMetadata(1.0));

        public static readonly DependencyProperty DecimalPlacesProperty =
            DependencyProperty.Register(nameof(DecimalPlaces), typeof(int), typeof(NumericBox), new PropertyMetadata(2, OnDecimalPlacesChanged));

        private readonly TextBox editor;
        private bool syncing;

        /// <summary>
        /// 构造数值控件，动态创建 TextBox 和上下步进按钮，避免每个参数界面重复写数值输入逻辑。
        /// </summary>
        public NumericBox()
        {
            Height = 28;
            MinHeight = 24;
            Margin = new Thickness(0, 2, 0, 6);

            Grid root = new Grid();
            root.ColumnDefinitions.Add(new ColumnDefinition());
            root.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(20) });

            Border border = new Border
            {
                BorderBrush = new SolidColorBrush(Color.FromRgb(43, 120, 146)),
                BorderThickness = new Thickness(1),
                Background = new SolidColorBrush(Color.FromRgb(7, 17, 28)),
                CornerRadius = new CornerRadius(2)
            };
            Grid.SetColumnSpan(border, 2);
            root.Children.Add(border);

            editor = new TextBox
            {
                BorderThickness = new Thickness(0),
                Background = Brushes.Transparent,
                Foreground = new SolidColorBrush(Color.FromRgb(234, 251, 255)),
                CaretBrush = new SolidColorBrush(Color.FromRgb(37, 217, 255)),
                Padding = new Thickness(6, 1, 4, 1),
                HorizontalContentAlignment = HorizontalAlignment.Right,
                VerticalContentAlignment = VerticalAlignment.Center,
                FontFamily = new FontFamily("Microsoft YaHei UI"),
                FontSize = 12
            };
            editor.PreviewTextInput += OnPreviewTextInput;
            editor.PreviewKeyDown += OnPreviewKeyDown;
            editor.LostKeyboardFocus += delegate { CommitText(); };
            editor.TextChanged += delegate
            {
                if (!syncing) CommitText(false);
            };
            DataObject.AddPastingHandler(editor, OnPaste);
            Grid.SetColumn(editor, 0);
            root.Children.Add(editor);

            Grid buttons = new Grid();
            buttons.RowDefinitions.Add(new RowDefinition());
            buttons.RowDefinitions.Add(new RowDefinition());
            Grid.SetColumn(buttons, 1);
            root.Children.Add(buttons);

            Button upButton = CreateButton("▲");
            upButton.Click += delegate { Step(1); };
            Grid.SetRow(upButton, 0);
            buttons.Children.Add(upButton);

            Button downButton = CreateButton("▼");
            downButton.Click += delegate { Step(-1); };
            Grid.SetRow(downButton, 1);
            buttons.Children.Add(downButton);

            Content = root;
            ApplyValue(Value);
        }

        /// <summary>
        /// 当前数值，默认双向绑定到 ViewModel 数值属性。
        /// </summary>
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// 允许输入的最小值。
        /// </summary>
        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        /// <summary>
        /// 允许输入的最大值。
        /// </summary>
        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        /// <summary>
        /// 点击上下按钮或键盘上下键时的步进值。
        /// </summary>
        public double Increment
        {
            get { return (double)GetValue(IncrementProperty); }
            set { SetValue(IncrementProperty, value); }
        }

        /// <summary>
        /// 显示和格式化的小数位数。
        /// </summary>
        public int DecimalPlaces
        {
            get { return (int)GetValue(DecimalPlacesProperty); }
            set { SetValue(DecimalPlacesProperty, value); }
        }

        /// <summary>
        /// Value 依赖属性变化时刷新文本显示。
        /// </summary>
        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((NumericBox)d).ApplyValue(Convert.ToDouble(e.NewValue, CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// 最小/最大值变化时重新夹紧当前值。
        /// </summary>
        private static void OnRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumericBox box = (NumericBox)d;
            box.Value = box.Clamp(box.Value);
            box.ApplyValue(box.Value);
        }

        /// <summary>
        /// 小数位数变化时重新格式化文本。
        /// </summary>
        private static void OnDecimalPlacesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumericBox box = (NumericBox)d;
            box.ApplyValue(box.Value);
        }

        /// <summary>
        /// 创建深色风格步进按钮。
        /// </summary>
        private Button CreateButton(string content)
        {
            return new Button
            {
                Content = content,
                Padding = new Thickness(0),
                FontSize = 8,
                Foreground = new SolidColorBrush(Color.FromRgb(191, 234, 255)),
                Background = new SolidColorBrush(Color.FromRgb(18, 48, 68)),
                BorderThickness = new Thickness(0)
            };
        }

        /// <summary>
        /// 按方向执行一步增减，并写回 Value。
        /// </summary>
        private void Step(int direction)
        {
            Value = Clamp(Value + Math.Max(0.000001, Increment) * direction);
            ApplyValue(Value);
        }

        /// <summary>
        /// 将数值夹紧后格式化到 TextBox，syncing 用于避免 TextChanged 递归提交。
        /// </summary>
        private void ApplyValue(double value)
        {
            syncing = true;
            editor.Text = Clamp(value).ToString("F" + Math.Max(0, DecimalPlaces), CultureInfo.InvariantCulture);
            syncing = false;
        }

        /// <summary>
        /// 提交文本并重新格式化。
        /// </summary>
        private void CommitText()
        {
            CommitText(true);
        }

        /// <summary>
        /// 解析文本、夹紧范围并写回 Value；可选择是否重新格式化显示。
        /// </summary>
        private void CommitText(bool reformat)
        {
            double parsed;
            if (!double.TryParse(editor.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out parsed))
                return;

            double clamped = Clamp(parsed);
            if (Math.Abs(Value - clamped) > 0.0000001)
                Value = clamped;
            if (reformat)
                ApplyValue(Value);
        }

        /// <summary>
        /// 将输入值限制到 Minimum 和 Maximum 范围内，非法数值回退为 0。
        /// </summary>
        private double Clamp(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value)) value = 0;
            if (value < Minimum) return Minimum;
            if (value > Maximum) return Maximum;
            return value;
        }

        /// <summary>
        /// 文本输入过滤，只允许能组成浮点数的字符序列进入编辑框。
        /// </summary>
        private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            string nextText = editor.Text.Insert(editor.SelectionStart, e.Text);
            double parsed;
            e.Handled = !double.TryParse(nextText, NumberStyles.Float, CultureInfo.InvariantCulture, out parsed) &&
                        nextText != "-" &&
                        nextText != "." &&
                        nextText != "-.";
        }

        /// <summary>
        /// 处理键盘上下键步进和回车提交。
        /// </summary>
        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up)
            {
                Step(1);
                e.Handled = true;
            }
            else if (e.Key == Key.Down)
            {
                Step(-1);
                e.Handled = true;
            }
            else if (e.Key == Key.Enter)
            {
                CommitText();
                e.Handled = true;
            }
        }

        /// <summary>
        /// 粘贴前校验文本是否能解析为浮点数，避免无效字符串进入数值框。
        /// </summary>
        private void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            if (!e.DataObject.GetDataPresent(DataFormats.Text))
            {
                e.CancelCommand();
                return;
            }

            string text = Convert.ToString(e.DataObject.GetData(DataFormats.Text), CultureInfo.InvariantCulture);
            double parsed;
            if (!double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out parsed))
                e.CancelCommand();
        }
    }
}

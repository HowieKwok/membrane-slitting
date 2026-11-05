using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FrameCommunity.UserControls
{

    /// <summary>
    /// 纯 WPF 实现的数值输入框，模仿 WinForms NumericUpDown 的交互与外观。
    /// 提供 Value、Minimum、Maximum、Increment、DecimalPlaces 依赖属性，支持绑定、键盘和鼠标操作。
    /// </summary>
    public class NumericBox : UserControl
    {
        // -------------------- 依赖属性定义 --------------------
        // 注意：ValueProperty 已设置 BindsTwoWayByDefault，默认双向绑定，
        // 因此 UpdateBindingSources 中可以直接通过 GetBindingExpression(ValueProperty) 强制更新。

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

        public static readonly DependencyProperty TextHorizontalContentAlignmentProperty =
 DependencyProperty.Register(
     nameof(TextHorizontalContentAlignment),
     typeof(HorizontalAlignment),
     typeof(NumericBox),
     new PropertyMetadata(HorizontalAlignment.Right));

        // -------------------- 预定义画刷（深色主题）--------------------
        private static readonly Brush BorderBrushValue = new SolidColorBrush(Color.FromRgb(43, 120, 146));   // 边框
        private static readonly Brush ButtonBrushValue = new SolidColorBrush(Color.FromRgb(18, 48, 68));   // 按钮背景
        private static readonly Brush TextBrushValue = new SolidColorBrush(Color.FromRgb(234, 251, 255));  // 文本颜色
        private static readonly Brush IconBrushValue = new SolidColorBrush(Color.FromRgb(191, 234, 255));  // 箭头颜色
        private static readonly Brush BackgroundBrushValue = new SolidColorBrush(Color.FromRgb(7, 17, 28)); // 整体背景

        // -------------------- 内部控件与状态 --------------------
        private readonly TextBox textBox;   // 用于显示和编辑数值的文本框
        private bool syncing;              // 防止程序更新文本框文本时触发 TextChanged 导致循环更新
        private bool editing;              // 标记用户是否正在编辑（拥有键盘焦点时设为 true）
        private bool committing;            // 标记当前是否正在提交文本，提交时允许 Value 回写格式化显示

        public NumericBox()
        {
            // 固定外观尺寸
            Height = 28;
            MinHeight = 24;
            Margin = new Thickness(0, 2, 0, 7);
            Focusable = false;                  // 控件本身不可聚焦，聚焦由内部 TextBox 处理
            UseLayoutRounding = true;           // 像素对齐，避免模糊
            SnapsToDevicePixels = true;

            // 根布局：两列（左侧文本框，右侧按钮面板）
            Grid root = new Grid
            {
                Background = BackgroundBrushValue,
                SnapsToDevicePixels = true,
                UseLayoutRounding = true
            };
            root.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            root.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(20) });

            // 外层圆角边框（模拟 WinForms FixedSingle 风格，但带圆角）
            Border border = new Border
            {
                BorderBrush = BorderBrushValue,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(2),
                Background = BackgroundBrushValue,
                SnapsToDevicePixels = true,
                UseLayoutRounding = true
            };
            Grid.SetColumnSpan(border, 2);       // 边框覆盖两列，形成整体外框
            root.Children.Add(border);

            // 数值输入框
            textBox = new TextBox
            {
                BorderThickness = new Thickness(0),            // 去掉自带边框，用外层 Border 统一绘制
                Background = Brushes.Transparent,
                Foreground = TextBrushValue,
                CaretBrush = new SolidColorBrush(Color.FromRgb(37, 217, 255)),
                Padding = new Thickness(6, 1, 4, 1),
                //HorizontalContentAlignment = HorizontalAlignment.Right,   // 数字右对齐
                VerticalContentAlignment = VerticalAlignment.Center,
                FontFamily = new FontFamily("Microsoft YaHei UI"),
                FontSize = 12,
                MinWidth = 0
            };

            textBox.SetBinding(
     TextBox.HorizontalContentAlignmentProperty,
     new Binding(nameof(TextHorizontalContentAlignment))
     {
         Source = this,
         Mode = BindingMode.OneWay
     });

            // 注册事件
            textBox.PreviewTextInput += OnPreviewTextInput;     // 拦截输入，只允许合法字符
            textBox.PreviewKeyDown += OnPreviewKeyDown;         // 处理上下箭头键与回车
            textBox.GotKeyboardFocus += OnGotKeyboardFocus;     // 获得焦点时选中全部文本
            textBox.LostKeyboardFocus += OnLostKeyboardFocus;   // 失去焦点时提交文本
            textBox.TextChanged += OnTextChanged;               // 实时解析文本并更新 Value（仅在 editing 时）
            DataObject.AddPastingHandler(textBox, OnPaste);     // 拦截粘贴内容
            Grid.SetColumn(textBox, 0);
            root.Children.Add(textBox);

            // 右侧按钮面板（上下等分）
            Grid buttonPanel = new Grid
            {
                Width = 20,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                SnapsToDevicePixels = true,
                UseLayoutRounding = true
            };
            buttonPanel.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            buttonPanel.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            Grid.SetColumn(buttonPanel, 1);
            root.Children.Add(buttonPanel);

            // 创建上下按钮并关联点击事件
            Button upButton = CreateStepButton(true);
            upButton.Click += delegate { Step(+1); };
            Grid.SetRow(upButton, 0);
            buttonPanel.Children.Add(upButton);

            Button downButton = CreateStepButton(false);
            downButton.Click += delegate { Step(-1); };
            Grid.SetRow(downButton, 1);
            buttonPanel.Children.Add(downButton);

            Content = root;            // 将构建好的布局设为控件内容
            ApplyValue(Value);        // 初始显示当前值
        }

        // -------------------- CLR 属性包装器 --------------------
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public double Increment
        {
            get { return (double)GetValue(IncrementProperty); }
            set { SetValue(IncrementProperty, value); }
        }

        public int DecimalPlaces
        {
            get { return (int)GetValue(DecimalPlacesProperty); }
            set { SetValue(DecimalPlacesProperty, value); }
        }

        public HorizontalAlignment TextHorizontalContentAlignment
        {
            get => (HorizontalAlignment)GetValue(TextHorizontalContentAlignmentProperty);
            set => SetValue(TextHorizontalContentAlignmentProperty, value);
        }

        // -------------------- 依赖属性变更回调 --------------------
        /// <summary>
        /// Value 属性改变时更新文本框显示（除非正在同步中）。
        /// </summary>
        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumericBox box = (NumericBox)d;
            if (box.editing && !box.committing)
                return;
            box.ApplyValue(Convert.ToDouble(e.NewValue, CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Minimum 或 Maximum 改变时，强制将当前值调整到新范围内。
        /// </summary>
        private static void OnRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumericBox box = (NumericBox)d;
            box.CoerceCurrentValue();
        }

        /// <summary>
        /// DecimalPlaces 改变时，重新格式化显示当前值（不改变底层 Value）。
        /// </summary>
        private static void OnDecimalPlacesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumericBox box = (NumericBox)d;
            box.ApplyValue(box.Value);
        }

        // -------------------- 按钮创建与样式 --------------------
        /// <summary>
        /// 创建一个上下箭头按钮，使用统一样式和自定义箭头图标。
        /// </summary>
        private static Button CreateStepButton(bool up)
        {
            return new Button
            {
                Style = CreateStepButtonStyle(),
                Content = CreateArrowIcon(up)
            };
        }

        /// <summary>
        /// 定义按钮样式：无内边距、左边框、深色背景、无焦点、最小尺寸为0。
        /// </summary>
        private static Style CreateStepButtonStyle()
        {
            Style style = new Style(typeof(Button));
            style.Setters.Add(new Setter(Control.PaddingProperty, new Thickness(0)));
            style.Setters.Add(new Setter(FrameworkElement.MarginProperty, new Thickness(0)));
            style.Setters.Add(new Setter(Control.BorderThicknessProperty, new Thickness(1, 0, 0, 0))); // 只显示左边分割线
            style.Setters.Add(new Setter(Control.BorderBrushProperty, BorderBrushValue));
            style.Setters.Add(new Setter(Control.BackgroundProperty, ButtonBrushValue));
            style.Setters.Add(new Setter(Control.HorizontalContentAlignmentProperty, HorizontalAlignment.Center));
            style.Setters.Add(new Setter(Control.VerticalContentAlignmentProperty, VerticalAlignment.Center));
            style.Setters.Add(new Setter(Control.FocusableProperty, false));      // 不让按钮获取焦点，保持焦点在 TextBox
            style.Setters.Add(new Setter(FrameworkElement.MinWidthProperty, 0.0));
            style.Setters.Add(new Setter(FrameworkElement.MinHeightProperty, 0.0));
            return style;
        }

        /// <summary>
        /// 生成三角形箭头图标（Path），用 Grid 居中。
        /// </summary>
        /// <param name="up">true 表示上箭头，false 下箭头</param>
        private static FrameworkElement CreateArrowIcon(bool up)
        {
            Grid host = new Grid
            {
                Width = 20,
                Height = 14,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                SnapsToDevicePixels = true,
                UseLayoutRounding = true
            };

            Path icon = new Path
            {
                Width = 10,
                Height = 6,
                // 上箭头：顶点在上，下箭头：顶点在下
                Data = Geometry.Parse(up ? "M 0 6 L 5 0 L 10 6 Z" : "M 0 0 L 10 0 L 5 6 Z"),
                Fill = IconBrushValue,
                Stretch = Stretch.Fill,
                StrokeThickness = 0,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                SnapsToDevicePixels = true,
                UseLayoutRounding = true
            };
            host.Children.Add(icon);
            return host;
        }

        // -------------------- 输入事件处理 --------------------
        /// <summary>
        /// 字符输入前验证，拒绝非数字、非法小数点或负号输入。
        /// </summary>
        private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            string candidate = BuildCandidateText(e.Text);   // 模拟输入后的文本
            e.Handled = !IsPotentialNumber(candidate);       // 不合法则拦截
        }

        /// <summary>
        /// 键盘事件：上/下箭头步进，回车提交并选中全部文本。
        /// </summary>
        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up)
            {
                Step(+1);
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Down)
            {
                Step(-1);
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Enter)
            {
                CommitText();               // 尝试解析并更新 Value
                textBox.SelectAll();        // 准备下次输入
                e.Handled = true;
            }
        }

        /// <summary>
        /// 获得键盘焦点：进入编辑状态，全选已有文本。
        /// </summary>
        private void OnGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            editing = true;
            textBox.SelectAll();
        }

        /// <summary>
        /// 失去键盘焦点：提交当前文本，退出编辑状态。
        /// </summary>
        private void OnLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            CommitText();
            editing = false;
        }

        /// <summary>
        /// 文本变化时只校验输入，不立刻写回 Value。
        /// 以前这里会边输入边触发 Value 回调和格式化，输入 1000.000 这类 double 时会造成光标乱跳和数字顺序错乱。
        /// 现在改为回车、失焦、上下按钮时统一提交，保证手动输入过程稳定。
        /// </summary>
        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (syncing || !editing) return;
            if (IsPotentialNumber(textBox.Text)) return;
            ApplyValue(Value);
            textBox.SelectAll();
        }

        /// <summary>
        /// 粘贴处理：检查粘贴后的文本是否为合法数字，非法则拒绝粘贴。
        /// </summary>
        private void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            if (!e.DataObject.GetDataPresent(DataFormats.Text))
            {
                e.CancelCommand();
                return;
            }

            string paste = e.DataObject.GetData(DataFormats.Text) as string;
            string candidate = BuildCandidateText(paste ?? string.Empty);
            if (!IsPotentialNumber(candidate)) e.CancelCommand();
        }

        // -------------------- 文本构造与验证 --------------------
        /// <summary>
        /// 模拟将指定文本插入到当前文本框光标位置后的结果字符串。
        /// </summary>
        private string BuildCandidateText(string input)
        {
            string text = textBox.Text ?? string.Empty;
            int start = Math.Max(0, textBox.SelectionStart);
            int length = Math.Max(0, textBox.SelectionLength);
            if (start > text.Length) start = text.Length;
            if (start + length > text.Length) length = text.Length - start;
            return text.Remove(start, length).Insert(start, input ?? string.Empty);
        }

        /// <summary>
        /// 判断字符串是否为合法数字的“中间状态”，允许负号、小数点的部分输入。
        /// 根据 Minimum 和 DecimalPlaces 进行额外限制。
        /// </summary>
        private bool IsPotentialNumber(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return true;
            if (text == "-" && Minimum < 0) return true;              // 允许单个负号
            if ((text == "." || text == "-.") && Minimum < 0) return DecimalPlaces > 0; // 允许小数点，但前提是小数位数>0且允许负数
            if (DecimalPlaces <= 0 && text.IndexOf('.') >= 0) return false; // 小数位数=0时禁止小数点
            // 尝试用不变文化和当前文化解析，任一成功即合法
            return double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out _) ||
                   double.TryParse(text, NumberStyles.Float, CultureInfo.CurrentCulture, out _);
        }

        /// <summary>
        /// 安全地将字符串解析为 double，支持不变文化和当前文化。
        /// </summary>
        private bool TryParseText(string text, out double value)
        {
            if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value)) return true;
            return double.TryParse(text, NumberStyles.Float, CultureInfo.CurrentCulture, out value);
        }

        // -------------------- 数值提交与步进 --------------------
        /// <summary>
        /// 强制将当前文本框内容解析并写入 Value，若解析失败则恢复原值。
        /// </summary>
        private void CommitText()
        {
            if (!TryParseText(textBox.Text, out double parsed))
            {
                ApplyValue(Value);      // 恢复原有显示
                return;
            }

            parsed = Clamp(Round(parsed));
            committing = true;
            try
            {
                SetCurrentValue(ValueProperty, parsed);  // 设置值（会触发回调刷新文本框）
                ApplyValue(parsed);                      // 确保文本框显示一致
            }
            finally
            {
                committing = false;
            }
        }

        /// <summary>
        /// 根据 direction 步进数值，先提交当前输入再计算新值。
        /// </summary>
        private void Step(int direction)
        {
            CommitText();  // 确保当前编辑内容已提交
            double step = Math.Max(0.000001, Math.Abs(Increment)); // 最小步进值防止死锁
            double value = Clamp(Round(Value + direction * step));
            SetCurrentValue(ValueProperty, value);
            ApplyValue(value);
        }

        /// <summary>
        /// 强制当前 Value 适应 Minimum/Maximum 范围，若值被修改则更新。
        /// </summary>
        private void CoerceCurrentValue()
        {
            double value = Clamp(Value);
            if (Math.Abs(value - Value) > 0.0000000001) SetCurrentValue(ValueProperty, value);
            ApplyValue(value);
        }

        /// <summary>
        /// 将指定值（已合规）格式化并显示到文本框，同时防止递归同步。
        /// </summary>
        private void ApplyValue(double value)
        {
            if (textBox == null) return;
            syncing = true;
            try
            {
                textBox.Text = FormatValue(Clamp(Round(value)));
            }
            finally
            {
                syncing = false;
            }
        }

        /// <summary>
        /// 按 DecimalPlaces 小数位数格式化数值，采用不变文化（防止逗号小数点问题）。
        /// </summary>
        private string FormatValue(double value)
        {
            int places = Math.Max(0, DecimalPlaces);
            return value.ToString("F" + places.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// 按 DecimalPlaces 进行四舍五入（远离零方向，与 NumericUpDown 一致）。
        /// </summary>
        private double Round(double value)
        {
            int places = Math.Max(0, DecimalPlaces);
            return Math.Round(value, places, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// 将值限制在 [Minimum, Maximum] 范围内，并处理 NaN/Infinity。
        /// </summary>
        private double Clamp(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value)) value = 0;
            double min = Minimum;
            double max = Maximum;
            if (max < min) max = min;        // 防止最大值小于最小值的异常情况
            if (value < min) value = min;
            if (value > max) value = max;
            return value;
        }
    }
    //public sealed class NumericBox : UserControl
    //{
    //    public static readonly DependencyProperty ValueProperty =
    //        DependencyProperty.Register(
    //            nameof(Value),
    //            typeof(double),
    //            typeof(NumericBox),
    //            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged));

    //    public static readonly DependencyProperty MinimumProperty =
    //        DependencyProperty.Register(nameof(Minimum), typeof(double), typeof(NumericBox), new PropertyMetadata(-100000000.0, OnRangeChanged));

    //    public static readonly DependencyProperty MaximumProperty =
    //        DependencyProperty.Register(nameof(Maximum), typeof(double), typeof(NumericBox), new PropertyMetadata(100000000.0, OnRangeChanged));

    //    public static readonly DependencyProperty IncrementProperty =
    //        DependencyProperty.Register(nameof(Increment), typeof(double), typeof(NumericBox), new PropertyMetadata(1.0, OnIncrementChanged));

    //    public static readonly DependencyProperty DecimalPlacesProperty =
    //        DependencyProperty.Register(nameof(DecimalPlaces), typeof(int), typeof(NumericBox), new PropertyMetadata(2, OnDecimalPlacesChanged));

    //    private readonly Forms.NumericUpDown numeric;
    //    private bool syncing;

    //    public NumericBox()
    //    {
    //        Height = 28;
    //        Margin = new Thickness(0, 2, 0, 7);

    //        numeric = new Forms.NumericUpDown
    //        {
    //            BorderStyle = Forms.BorderStyle.FixedSingle,
    //            DecimalPlaces = DecimalPlaces,
    //            Increment = ToDecimal(Increment),
    //            Minimum = ToDecimal(Minimum),
    //            Maximum = ToDecimal(Maximum),
    //            TextAlign = Forms.HorizontalAlignment.Right,
    //            Dock = Forms.DockStyle.Fill
    //        };
    //        numeric.ValueChanged += Numeric_ValueChanged;

    //        Content = new Wfi.WindowsFormsHost { Child = numeric };
    //        ApplyValue(Value);
    //    }

    //    public double Value
    //    {
    //        get { return (double)GetValue(ValueProperty); }
    //        set { SetValue(ValueProperty, value); }
    //    }

    //    public double Minimum
    //    {
    //        get { return (double)GetValue(MinimumProperty); }
    //        set { SetValue(MinimumProperty, value); }
    //    }

    //    public double Maximum
    //    {
    //        get { return (double)GetValue(MaximumProperty); }
    //        set { SetValue(MaximumProperty, value); }
    //    }

    //    public double Increment
    //    {
    //        get { return (double)GetValue(IncrementProperty); }
    //        set { SetValue(IncrementProperty, value); }
    //    }

    //    public int DecimalPlaces
    //    {
    //        get { return (int)GetValue(DecimalPlacesProperty); }
    //        set { SetValue(DecimalPlacesProperty, value); }
    //    }

    //    private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    //    {
    //        ((NumericBox)d).ApplyValue(Convert.ToDouble(e.NewValue));
    //    }

    //    private static void OnRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    //    {
    //        NumericBox box = (NumericBox)d;
    //        if (box.numeric == null) return;

    //        decimal min = ToDecimal(box.Minimum);
    //        decimal max = ToDecimal(box.Maximum);
    //        if (max < min) max = min;
    //        box.numeric.Minimum = min;
    //        box.numeric.Maximum = max;
    //        box.ApplyValue(box.Value);
    //    }

    //    private static void OnIncrementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    //    {
    //        NumericBox box = (NumericBox)d;
    //        if (box.numeric != null) box.numeric.Increment = ToDecimal(Math.Max(0.000001, box.Increment));
    //    }

    //    private static void OnDecimalPlacesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    //    {
    //        NumericBox box = (NumericBox)d;
    //        if (box.numeric != null) box.numeric.DecimalPlaces = Math.Max(0, box.DecimalPlaces);
    //    }

    //    private void Numeric_ValueChanged(object sender, EventArgs e)
    //    {
    //        if (syncing) return;
    //        SetCurrentValue(ValueProperty, decimal.ToDouble(numeric.Value));
    //    }

    //    private void ApplyValue(double value)
    //    {
    //        if (numeric == null) return;
    //        syncing = true;
    //        try
    //        {
    //            decimal v = ToDecimal(value);
    //            if (v < numeric.Minimum) v = numeric.Minimum;
    //            if (v > numeric.Maximum) v = numeric.Maximum;
    //            numeric.Value = v;
    //        }
    //        finally
    //        {
    //            syncing = false;
    //        }
    //    }

    //    private static decimal ToDecimal(double value)
    //    {
    //        if (double.IsNaN(value) || double.IsInfinity(value)) return 0m;
    //        if (value > (double)decimal.MaxValue) return decimal.MaxValue;
    //        if (value < (double)decimal.MinValue) return decimal.MinValue;
    //        return Convert.ToDecimal(value);
    //    }
    //}
}

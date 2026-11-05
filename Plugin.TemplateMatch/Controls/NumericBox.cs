using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Plugin.TemplateMatch.Controls
{
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

        private static readonly Brush BorderBrushValue = new SolidColorBrush(Color.FromRgb(43, 120, 146));
        private static readonly Brush ButtonBrushValue = new SolidColorBrush(Color.FromRgb(18, 48, 68));
        private static readonly Brush TextBrushValue = new SolidColorBrush(Color.FromRgb(234, 251, 255));
        private static readonly Brush IconBrushValue = new SolidColorBrush(Color.FromRgb(191, 234, 255));
        private static readonly Brush BackgroundBrushValue = new SolidColorBrush(Color.FromRgb(7, 17, 28));

        private readonly TextBox textBox;
        private bool syncing;
        private bool editing;

        public NumericBox()
        {
            Height = 28;
            MinHeight = 24;
            Margin = new Thickness(0, 2, 0, 7);
            Focusable = false;
            UseLayoutRounding = true;
            SnapsToDevicePixels = true;

            Grid root = new Grid
            {
                Background = BackgroundBrushValue,
                SnapsToDevicePixels = true,
                UseLayoutRounding = true
            };
            root.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            root.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(20) });

            Border border = new Border
            {
                BorderBrush = BorderBrushValue,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(2),
                Background = BackgroundBrushValue,
                SnapsToDevicePixels = true,
                UseLayoutRounding = true
            };
            Grid.SetColumnSpan(border, 2);
            root.Children.Add(border);

            textBox = new TextBox
            {
                BorderThickness = new Thickness(0),
                Background = Brushes.Transparent,
                Foreground = TextBrushValue,
                CaretBrush = new SolidColorBrush(Color.FromRgb(37, 217, 255)),
                Padding = new Thickness(6, 1, 4, 1),
                HorizontalContentAlignment = HorizontalAlignment.Right,
                VerticalContentAlignment = VerticalAlignment.Center,
                FontFamily = new FontFamily("Microsoft YaHei UI"),
                FontSize = 12,
                MinWidth = 0
            };
            textBox.PreviewTextInput += OnPreviewTextInput;
            textBox.PreviewKeyDown += OnPreviewKeyDown;
            textBox.GotKeyboardFocus += OnGotKeyboardFocus;
            textBox.LostKeyboardFocus += OnLostKeyboardFocus;
            textBox.TextChanged += OnTextChanged;
            DataObject.AddPastingHandler(textBox, OnPaste);
            Grid.SetColumn(textBox, 0);
            root.Children.Add(textBox);

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

            Button upButton = CreateStepButton(true);
            upButton.Click += delegate { Step(+1); };
            Grid.SetRow(upButton, 0);
            buttonPanel.Children.Add(upButton);

            Button downButton = CreateStepButton(false);
            downButton.Click += delegate { Step(-1); };
            Grid.SetRow(downButton, 1);
            buttonPanel.Children.Add(downButton);

            Content = root;
            ApplyValue(Value);
        }

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

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumericBox box = (NumericBox)d;
            box.ApplyValue(Convert.ToDouble(e.NewValue, CultureInfo.InvariantCulture));
        }

        private static void OnRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumericBox box = (NumericBox)d;
            box.CoerceCurrentValue();
        }

        private static void OnDecimalPlacesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumericBox box = (NumericBox)d;
            box.ApplyValue(box.Value);
        }

        private static Button CreateStepButton(bool up)
        {
            return new Button
            {
                Style = CreateStepButtonStyle(),
                Content = CreateArrowIcon(up)
            };
        }

        private static Style CreateStepButtonStyle()
        {
            Style style = new Style(typeof(Button));
            style.Setters.Add(new Setter(Control.PaddingProperty, new Thickness(0)));
            style.Setters.Add(new Setter(FrameworkElement.MarginProperty, new Thickness(0)));
            style.Setters.Add(new Setter(Control.BorderThicknessProperty, new Thickness(1, 0, 0, 0)));
            style.Setters.Add(new Setter(Control.BorderBrushProperty, BorderBrushValue));
            style.Setters.Add(new Setter(Control.BackgroundProperty, ButtonBrushValue));
            style.Setters.Add(new Setter(Control.HorizontalContentAlignmentProperty, HorizontalAlignment.Center));
            style.Setters.Add(new Setter(Control.VerticalContentAlignmentProperty, VerticalAlignment.Center));
            style.Setters.Add(new Setter(Control.FocusableProperty, false));
            style.Setters.Add(new Setter(FrameworkElement.MinWidthProperty, 0.0));
            style.Setters.Add(new Setter(FrameworkElement.MinHeightProperty, 0.0));
            return style;
        }

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

        private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            string candidate = BuildCandidateText(e.Text);
            e.Handled = !IsPotentialNumber(candidate);
        }

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
                CommitText();
                textBox.SelectAll();
                e.Handled = true;
            }
        }

        private void OnGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            editing = true;
            textBox.SelectAll();
        }

        private void OnLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            CommitText();
            editing = false;
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (syncing || !editing) return;
            if (!TryParseText(textBox.Text, out double parsed)) return;

            parsed = Clamp(parsed);
            if (Math.Abs(parsed - Value) > 0.0000000001)
            {
                SetCurrentValue(ValueProperty, parsed);
            }
        }

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

        private string BuildCandidateText(string input)
        {
            string text = textBox.Text ?? string.Empty;
            int start = Math.Max(0, textBox.SelectionStart);
            int length = Math.Max(0, textBox.SelectionLength);
            if (start > text.Length) start = text.Length;
            if (start + length > text.Length) length = text.Length - start;
            return text.Remove(start, length).Insert(start, input ?? string.Empty);
        }

        private bool IsPotentialNumber(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return true;
            if (text == "-" && Minimum < 0) return true;
            if ((text == "." || text == "-.") && Minimum < 0) return DecimalPlaces > 0;
            if (DecimalPlaces <= 0 && text.IndexOf('.') >= 0) return false;
            return double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out _) ||
                   double.TryParse(text, NumberStyles.Float, CultureInfo.CurrentCulture, out _);
        }

        private bool TryParseText(string text, out double value)
        {
            if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value)) return true;
            return double.TryParse(text, NumberStyles.Float, CultureInfo.CurrentCulture, out value);
        }

        private void CommitText()
        {
            if (!TryParseText(textBox.Text, out double parsed))
            {
                ApplyValue(Value);
                return;
            }

            parsed = Clamp(Round(parsed));
            SetCurrentValue(ValueProperty, parsed);
            ApplyValue(parsed);
        }

        private void Step(int direction)
        {
            CommitText();
            double step = Math.Max(0.000001, Math.Abs(Increment));
            double value = Clamp(Round(Value + direction * step));
            SetCurrentValue(ValueProperty, value);
            ApplyValue(value);
        }

        private void CoerceCurrentValue()
        {
            double value = Clamp(Value);
            if (Math.Abs(value - Value) > 0.0000000001) SetCurrentValue(ValueProperty, value);
            ApplyValue(value);
        }

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

        private string FormatValue(double value)
        {
            int places = Math.Max(0, DecimalPlaces);
            return value.ToString("F" + places.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
        }

        private double Round(double value)
        {
            int places = Math.Max(0, DecimalPlaces);
            return Math.Round(value, places, MidpointRounding.AwayFromZero);
        }

        private double Clamp(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value)) value = 0;
            double min = Minimum;
            double max = Maximum;
            if (max < min) max = min;
            if (value < min) value = min;
            if (value > max) value = max;
            return value;
        }
    }
}

using System;
using System.Windows;
using System.Windows.Controls;
using Forms = System.Windows.Forms;
using Wfi = System.Windows.Forms.Integration;

namespace Plugin.FindIntersection.Controls
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
            DependencyProperty.Register(nameof(Increment), typeof(double), typeof(NumericBox), new PropertyMetadata(1.0, OnIncrementChanged));

        public static readonly DependencyProperty DecimalPlacesProperty =
            DependencyProperty.Register(nameof(DecimalPlaces), typeof(int), typeof(NumericBox), new PropertyMetadata(2, OnDecimalPlacesChanged));

        private readonly Forms.NumericUpDown numeric;
        private bool syncing;

        public NumericBox()
        {
            Height = 28;
            Margin = new Thickness(0, 2, 0, 7);

            numeric = new Forms.NumericUpDown
            {
                BorderStyle = Forms.BorderStyle.FixedSingle,
                DecimalPlaces = DecimalPlaces,
                Increment = ToDecimal(Increment),
                Minimum = ToDecimal(Minimum),
                Maximum = ToDecimal(Maximum),
                TextAlign = Forms.HorizontalAlignment.Right,
                Dock = Forms.DockStyle.Fill
            };
            numeric.ValueChanged += Numeric_ValueChanged;

            Content = new Wfi.WindowsFormsHost { Child = numeric };
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
            ((NumericBox)d).ApplyValue(Convert.ToDouble(e.NewValue));
        }

        private static void OnRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumericBox box = (NumericBox)d;
            if (box.numeric == null) return;

            decimal min = ToDecimal(box.Minimum);
            decimal max = ToDecimal(box.Maximum);
            if (max < min) max = min;
            box.numeric.Minimum = min;
            box.numeric.Maximum = max;
            box.ApplyValue(box.Value);
        }

        private static void OnIncrementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumericBox box = (NumericBox)d;
            if (box.numeric != null) box.numeric.Increment = ToDecimal(Math.Max(0.000001, box.Increment));
        }

        private static void OnDecimalPlacesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumericBox box = (NumericBox)d;
            if (box.numeric != null) box.numeric.DecimalPlaces = Math.Max(0, box.DecimalPlaces);
        }

        private void Numeric_ValueChanged(object sender, EventArgs e)
        {
            if (syncing) return;
            SetCurrentValue(ValueProperty, decimal.ToDouble(numeric.Value));
        }

        private void ApplyValue(double value)
        {
            if (numeric == null) return;
            syncing = true;
            try
            {
                decimal v = ToDecimal(value);
                if (v < numeric.Minimum) v = numeric.Minimum;
                if (v > numeric.Maximum) v = numeric.Maximum;
                numeric.Value = v;
            }
            finally
            {
                syncing = false;
            }
        }

        private static decimal ToDecimal(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value)) return 0m;
            if (value > (double)decimal.MaxValue) return decimal.MaxValue;
            if (value < (double)decimal.MinValue) return decimal.MinValue;
            return Convert.ToDecimal(value);
        }
    }
}

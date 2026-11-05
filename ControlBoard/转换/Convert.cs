using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace ControlBoard.转换
{
    [ValueConversion(typeof(bool), typeof(Brush))]
    public class BoolToLedBrushConverter : IValueConverter
    {
        /// <summary>true=绿色点亮，false=灰色熄灭，异常红色</summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
            {
                return b
                    ? new SolidColorBrush(Color.FromRgb(34, 197, 94))
                    : new SolidColorBrush(Color.FromRgb(156, 163, 175));
            }
            return new SolidColorBrush(Color.FromRgb(239, 68, 68));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(bool), typeof(string))]
    public class BoolToTextConverter : MarkupExtension, IValueConverter
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isConnect)
            {
                return isConnect ? "已连接" : "断开";
            }
            return "未知";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }



    [ValueConversion(typeof(int), typeof(bool))]
    public class IntBoolConverter : MarkupExtension, IValueConverter
    {
        public int TrueValue { get; set; }
        public int FalseValue { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is int val && val == TrueValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool check && check)
                return TrueValue;
            return FalseValue;
        }
    }
}

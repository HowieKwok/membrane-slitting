using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace DevMgmt.Light.Tool
{
    [ValueConversion(typeof(bool), typeof(string))]
    public class BoolConnectTextConverter : MarkupExtension, IValueConverter
    {
        public static readonly BoolConnectTextConverter Instance = new BoolConnectTextConverter();
        public override object ProvideValue(IServiceProvider serviceProvider) => Instance;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool connState)
            {
                // true=已连接 显示断开；false=未连接 显示连接
                return connState ? "断开" : "连接";
            }
            return "连接";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("仅单向转换，不支持反向");
        }
    }

    [ValueConversion(typeof(bool), typeof(string))]
    public class BoolOpenClosingConnectTextConverter : MarkupExtension, IValueConverter
    {
        public static readonly BoolOpenClosingConnectTextConverter Instance = new BoolOpenClosingConnectTextConverter();
        public override object ProvideValue(IServiceProvider serviceProvider) => Instance;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool connState)
            {
             
                return connState ? "关闭" : "打开";
            }
            return "打开";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("仅单向转换，不支持反向");
        }
    }

    [ValueConversion(typeof(bool), typeof(SolidColorBrush))]  
    public class BoolToStatusBrushConverter : MarkupExtension, IValueConverter
    {
        // 🔥 加这个静态实例
        public static BoolToStatusBrushConverter Instance { get; } = new BoolToStatusBrushConverter();

        public override object ProvideValue(IServiceProvider serviceProvider) => Instance;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool connStatus)
            {
                return connStatus ? Brushes.LightGreen : Brushes.LightCoral;
            }
            return Brushes.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}

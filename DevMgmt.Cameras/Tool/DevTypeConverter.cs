using DevMgmt.Cameras.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace DevMgmt.Cameras.Tool
{
    public class DevTypeConverter : IValueConverter
    {
        // 🔥 加这个静态实例
        public static DevTypeConverter Instance { get; } = new DevTypeConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int intVal)
            {
                return (CamTypeEnum)intVal;
            }
            return "未知相机";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(bool), typeof(string))]
    public class BoolToStatusTextConverter : IValueConverter
    {
        // 🔥 加这个静态实例
        public static BoolToStatusTextConverter Instance { get; } = new BoolToStatusTextConverter();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool connStatus)
            {
                return connStatus ? "已连接" : "未连接";
            }
            return "未知状态";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 单向绑定，反向不需要实现
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(bool), typeof(SolidColorBrush))]
    public class BoolToStatusBrushConverter : IValueConverter
    {
        // 🔥 加这个静态实例
        public static BoolToStatusBrushConverter Instance { get; } = new BoolToStatusBrushConverter();
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

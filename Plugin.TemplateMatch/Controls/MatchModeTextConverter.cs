using Frame_Core.pluginParams;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Plugin.TemplateMatch.Controls
{
    public sealed class MatchModeTextConverter : IValueConverter
    {
        public static readonly MatchModeTextConverter Instance = new MatchModeTextConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TemplateMatchMode mode)
                return mode == TemplateMatchMode.Gray ? "灰度匹配" : "形状匹配";
            return "形状匹配";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string text = value as string;
            return string.Equals(text, "灰度匹配", StringComparison.OrdinalIgnoreCase) ? TemplateMatchMode.Gray : TemplateMatchMode.Shape;
        }
    }
}

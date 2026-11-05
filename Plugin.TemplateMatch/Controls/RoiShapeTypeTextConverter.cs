using System;
using System.Globalization;
using System.Windows.Data;
using ViewROI;

namespace Plugin.TemplateMatch.Controls
{
    public sealed class RoiShapeTypeTextConverter : IValueConverter
    {
        public static readonly RoiShapeTypeTextConverter Instance = new RoiShapeTypeTextConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int type = value is int v ? v : ROI.ROI_TYPE_RECTANCLE1;
            switch (type)
            {
                case ROI.ROI_TYPE_LINE:
                    return "直线";
                case ROI.ROI_TYPE_CIRCLE:
                    return "圆";
                case ROI.ROI_TYPE_CIRCLEARC:
                    return "圆弧";
                case ROI.ROI_TYPE_RECTANGLE2:
                    return "矩形2";
                case ROI.ROI_TYPE_RECTANCLE1:
                default:
                    return "矩形1";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string text = value as string;
            if (string.Equals(text, "直线", StringComparison.OrdinalIgnoreCase)) return ROI.ROI_TYPE_LINE;
            if (string.Equals(text, "圆", StringComparison.OrdinalIgnoreCase)) return ROI.ROI_TYPE_CIRCLE;
            if (string.Equals(text, "圆弧", StringComparison.OrdinalIgnoreCase)) return ROI.ROI_TYPE_CIRCLEARC;
            if (string.Equals(text, "矩形2", StringComparison.OrdinalIgnoreCase)) return ROI.ROI_TYPE_RECTANGLE2;
            return ROI.ROI_TYPE_RECTANCLE1;
        }
    }
}

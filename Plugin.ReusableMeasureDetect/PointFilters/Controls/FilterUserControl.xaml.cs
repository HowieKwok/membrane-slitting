using System.Windows.Controls;

namespace Plugin.ReusableMeasureDetect.PointFilters.Controls
{
    /// <summary>
    /// 点集滤波参数控件。
    /// XAML 通过 DataTemplate 显示不同滤波器的参数区域，代码隐藏只保留初始化，避免把滤波参数逻辑写死在控件里。
    /// </summary>
    public partial class FilterUserControl : UserControl
    {
        /// <summary>
        /// 初始化滤波控件 XAML。
        /// </summary>
        public FilterUserControl()
        {
            InitializeComponent();
        }
    }
}

using Plugin.BlobAnalysis.Models;
using System.Collections.Generic;
using System.Windows;

namespace Plugin.BlobAnalysis.WPFArchitecture
{
    public partial class AddFilterRuleDialog : Window
    {
        public FilterRuleParam Rule { get; } = new FilterRuleParam { Enabled = true, Feature = "area", Min = 0, Max = 999999, Operation = "and" };

        public IReadOnlyList<KeyValuePair<string, string>> FeatureItems { get; } = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("area", "面积 area"),
            new KeyValuePair<string, string>("row", "质心Row row"),
            new KeyValuePair<string, string>("column", "质心Col column"),
            new KeyValuePair<string, string>("width", "宽度 width"),
            new KeyValuePair<string, string>("height", "高度 height"),
            new KeyValuePair<string, string>("circularity", "圆度 circularity"),
            new KeyValuePair<string, string>("rectangularity", "矩形度 rectangularity"),
            new KeyValuePair<string, string>("convexity", "凸性 convexity"),
            new KeyValuePair<string, string>("compactness", "紧凑度 compactness"),
            new KeyValuePair<string, string>("holes_num", "孔洞数 holes_num"),
            new KeyValuePair<string, string>("orientation", "方向 orientation"),
            new KeyValuePair<string, string>("ra", "长轴 ra"),
            new KeyValuePair<string, string>("rb", "短轴 rb"),
            new KeyValuePair<string, string>("anisometry", "长宽比 anisometry")
        };

        public IReadOnlyList<KeyValuePair<string, string>> OperationItems { get; } = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("and", "并且 and"),
            new KeyValuePair<string, string>("or", "或者 or")
        };

        public string Feature { get { return Rule.Feature; } set { Rule.Feature = value; } }
        public double Min { get { return Rule.Min; } set { Rule.Min = value; } }
        public double Max { get { return Rule.Max; } set { Rule.Max = value; } }
        public string Operation { get { return Rule.Operation; } set { Rule.Operation = value; } }

        public AddFilterRuleDialog()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void OnOkClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
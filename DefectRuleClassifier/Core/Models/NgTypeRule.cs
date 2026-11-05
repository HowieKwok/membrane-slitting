using DefectRuleClassifier.ViewModels;
using System.Collections.ObjectModel;

namespace DefectRuleClassifier.Core.Models
{
    /// <summary>
    /// 单个 NG 类别的分类规则。
    /// 列表顺序即优先级，越靠前越先判断。
    /// </summary>
    public sealed class NgTypeRule : BindableBase
    {
        private string name = "新类别";
        private bool enableJudgeCondition = false;
        private JudgeConditionMode judgeCondition = JudgeConditionMode.AllConditionAnd;
        private double minLength = 0.2;
        private double maxLength = 999999999;
        private double minWidth = 0.2;
        private double maxWidth = 999999999;
        private double minArea = 0.2;
        private double maxArea = 999999999;

        /// <summary>命中后输出的 NG 类别名称。</summary>
        public string Name { get => name; set => SetProperty(ref name, value); }

        /// <summary>是否启用尺寸判定条件。</summary>
        public bool EnableJudgeCondition { get => enableJudgeCondition; set => SetProperty(ref enableJudgeCondition, value); }

        /// <summary>尺寸判定的组合方式。</summary>
        public JudgeConditionMode JudgeCondition
        {
            get => judgeCondition;
            set
            {
                if (SetProperty(ref judgeCondition, value))
                    OnPropertyChanged(nameof(JudgeConditionTemplateId));
            }
        }

        /// <summary>兼容旧 XML 的模板 Id 映射。</summary>
        public string JudgeConditionTemplateId
        {
            get => ToTemplateId(JudgeCondition);
            set
            {
                JudgeConditionMode mapped = FromTemplateId(value);
                if (SetProperty(ref judgeCondition, mapped, nameof(JudgeCondition)))
                    OnPropertyChanged(nameof(JudgeConditionTemplateId));
            }
        }

        public double MinLength { get => minLength; set => SetProperty(ref minLength, value); }
        public double MaxLength { get => maxLength; set => SetProperty(ref maxLength, value); }
        public double MinWidth { get => minWidth; set => SetProperty(ref minWidth, value); }
        public double MaxWidth { get => maxWidth; set => SetProperty(ref maxWidth, value); }
        public double MinArea { get => minArea; set => SetProperty(ref minArea, value); }
        public double MaxArea { get => maxArea; set => SetProperty(ref maxArea, value); }

        /// <summary>当前类别要判定的特征项。</summary>
        public ObservableCollection<FeatureRule> Features { get; set; } = new ObservableCollection<FeatureRule>();

        private static string ToTemplateId(JudgeConditionMode mode)
        {
            switch (mode)
            {
                case JudgeConditionMode.AllConditionOr:
                    return "size_or";
                case JudgeConditionMode.LengthAndWidthOrArea:
                    return "length_width_or_area";
                case JudgeConditionMode.LengthOrWidthAndArea:
                    return "length_or_width_and_area";
                default:
                    return "size_and";
            }
        }

        private static JudgeConditionMode FromTemplateId(string id)
        {
            switch (id)
            {
                case "size_or":
                    return JudgeConditionMode.AllConditionOr;
                case "length_width_or_area":
                    return JudgeConditionMode.LengthAndWidthOrArea;
                case "length_or_width_and_area":
                    return JudgeConditionMode.LengthOrWidthAndArea;
                default:
                    return JudgeConditionMode.AllConditionAnd;
            }
        }
    }
}

namespace DefectRuleClassifier.Core.Models
{
    /// <summary>
    /// 判定条件下拉框显示项。
    /// WPF 直接绑定枚举时中文显示不稳定，所以用这个轻量模型承载显示名。
    /// </summary>
    public sealed class JudgeConditionModeOption
    {
        public JudgeConditionMode Value { get; set; }
        public string DisplayName { get; set; }
        public string Remark { get; set; }
    }
}

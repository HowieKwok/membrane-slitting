namespace DefectRuleClassifier.Core.Models
{
    /// <summary>
    /// 单个缺陷类别命中后，用长、宽、面积再做一次尺寸判定的组合方式。
    /// </summary>
    public enum JudgeConditionMode
    {
        /// <summary>长度、宽度、面积都要满足。</summary>
        AllConditionAnd,

        /// <summary>长度、宽度、面积任意一项满足即可。</summary>
        AllConditionOr,

        /// <summary>(长 && 宽) || 面积。</summary>
        LengthAndWidthOrArea,

        /// <summary>(长 || 宽) && 面积。</summary>
        LengthOrWidthAndArea
    }
}

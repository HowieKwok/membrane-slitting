using DefectRuleClassifier.Core.Features;
using DefectRuleClassifier.Core.Models;
using System.Collections.Generic;

namespace DefectRuleClassifier.Core.Classifiers
{
    /// <summary>
    /// 单个候选缺陷执行规则分类时需要的上下文。
    /// </summary>
    public sealed class DefectClassifyContext
    {
        public DefectCandidate Candidate { get; set; }
        public IEnumerable<NgTypeRule> NgTypes { get; set; }
        public bool ClassifierEnabled { get; set; }
        public DefectFeatureContext FeatureContext { get; set; }
        public DefectFeatureExtractor FeatureExtractor { get; set; }
    }
}

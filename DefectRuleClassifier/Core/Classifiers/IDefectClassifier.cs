namespace DefectRuleClassifier.Core.Classifiers
{
    public interface IDefectClassifier
    {
        DefectClassifyDecision Classify(DefectClassifyContext context);
    }
}

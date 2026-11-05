namespace DefectRuleClassifier.Core.Models
{
    public sealed class FeatureTypeOption
    {
        public FeatureTypeOption(PicFeatureType value, string displayName, string featureKey, string description)
        {
            Value = value;
            DisplayName = displayName;
            FeatureKey = featureKey;
            Description = description;
        }

        public PicFeatureType Value { get; }
        public string DisplayName { get; }
        public string FeatureKey { get; }
        public string Description { get; }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}

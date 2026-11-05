using DefectRuleClassifier.ViewModels;

namespace DefectRuleClassifier.Core.Models
{
    public sealed class FeatureRule : BindableBase
    {
        private PicFeatureType featureType;
        private double minValue;
        private double maxValue = 999999999;

        public PicFeatureType FeatureType
        {
            get => featureType;
            set
            {
                if (SetProperty(ref featureType, value))
                {
                    OnPropertyChanged(nameof(FeatureName));
                    OnPropertyChanged(nameof(FeatureKey));
                    OnPropertyChanged(nameof(FeatureDescription));
                    OnPropertyChanged(nameof(FeatureRangeDescription));
                }
            }
        }

        public double MinValue { get => minValue; set => SetProperty(ref minValue, value); }
        public double MaxValue { get => maxValue; set => SetProperty(ref maxValue, value); }
        public string FeatureName => FeatureCatalog.Get(FeatureType).DisplayName;
        public string FeatureKey => FeatureCatalog.Get(FeatureType).FeatureKey;
        public string FeatureDescription => FeatureCatalog.Get(FeatureType).Description;
        public string FeatureRangeDescription => FeatureCatalog.GetRangeDescription(FeatureType);
    }
}

using DefectRuleClassifier.ViewModels;

namespace DefectRuleClassifier.Core.Models
{
    public sealed class FeatureValueRow : BindableBase
    {
        private int defectIndex;
        private string featureName;
        private string displayName;
        private double value;

        public int DefectIndex { get => defectIndex; set => SetProperty(ref defectIndex, value); }
        public string FeatureName { get => featureName; set => SetProperty(ref featureName, value); }
        public string DisplayName { get => displayName; set => SetProperty(ref displayName, value); }
        public double Value { get => value; set => SetProperty(ref this.value, value); }
    }
}

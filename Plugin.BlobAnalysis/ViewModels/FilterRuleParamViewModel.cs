using Plugin.BlobAnalysis.Models;

namespace Plugin.BlobAnalysis.ViewModels
{
    public sealed class FilterRuleParamViewModel : BindableBase
    {
        public FilterRuleParam Model { get; }

        public FilterRuleParamViewModel(FilterRuleParam model)
        {
            Model = model;
        }

        public bool Enabled { get => Model.Enabled; set { Model.Enabled = value; OnPropertyChanged(); } }
        public string Feature { get => Model.Feature; set { Model.Feature = value; OnPropertyChanged(); } }
        public double Min { get => Model.Min; set { Model.Min = value; OnPropertyChanged(); } }
        public double Max { get => Model.Max; set { Model.Max = value; OnPropertyChanged(); } }
        public string Operation { get => Model.Operation; set { Model.Operation = value; OnPropertyChanged(); } }
    }
}


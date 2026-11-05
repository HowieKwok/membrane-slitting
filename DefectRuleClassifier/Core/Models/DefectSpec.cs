using DefectRuleClassifier.ViewModels;
using System;

namespace DefectRuleClassifier.Core.Models
{
    [Serializable]
    public sealed class DefectSpec : BindableBase
    {
        private string defectName;
        private double minArea;
        private double minWidth;
        private double minHeight;
        private int priority;

        public DefectSpec()
        {
            defectName = "默认缺陷";
            minArea = 20;
            minWidth = 0;
            minHeight = 0;
            priority = 100;
        }

        public string DefectName { get => defectName; set => SetProperty(ref defectName, value); }
        public double MinArea { get => minArea; set => SetProperty(ref minArea, value); }
        public double MinWidth { get => minWidth; set => SetProperty(ref minWidth, value); }
        public double MinHeight { get => minHeight; set => SetProperty(ref minHeight, value); }
        public int Priority { get => priority; set => SetProperty(ref priority, value); }
    }
}

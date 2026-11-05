using DefectRuleClassifier.ViewModels;

namespace DefectRuleClassifier.Core.Models
{
    public sealed class DefectFeatureSummaryRow : BindableBase
    {
        private int defectIndex;
        private double area;
        private double length;
        private double width;
        private double height;
        private double row;
        private double column;
        private double meanGray;
        private double contrast;
        private double roundness;
        private double widthHeightRate;
        private double brightArea;
        private double darkArea;
        private string polarity;

        public int DefectIndex { get => defectIndex; set => SetProperty(ref defectIndex, value); }
        public double Area { get => area; set => SetProperty(ref area, value); }
        public double Length { get => length; set => SetProperty(ref length, value); }
        public double Width { get => width; set => SetProperty(ref width, value); }
        public double Height { get => height; set => SetProperty(ref height, value); }
        public double Row { get => row; set => SetProperty(ref row, value); }
        public double Column { get => column; set => SetProperty(ref column, value); }
        public double MeanGray { get => meanGray; set => SetProperty(ref meanGray, value); }
        public double Contrast { get => contrast; set => SetProperty(ref contrast, value); }
        public double Roundness { get => roundness; set => SetProperty(ref roundness, value); }
        public double WidthHeightRate { get => widthHeightRate; set => SetProperty(ref widthHeightRate, value); }
        public double BrightArea { get => brightArea; set => SetProperty(ref brightArea, value); }
        public double DarkArea { get => darkArea; set => SetProperty(ref darkArea, value); }
        public string Polarity { get => polarity; set => SetProperty(ref polarity, value); }
    }
}

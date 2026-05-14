using Plugin.BlobAnalysis.Models;

namespace Plugin.BlobAnalysis.ViewModels
{
    public sealed class RoiParamViewModel : BindableBase
    {
        public RoiParam Model { get; }
        public RoiParamViewModel(RoiParam model) { Model = model; }
        public bool Enabled { get => Model.Enabled; set { Model.Enabled = value; OnPropertyChanged(); } }
        public int Index { get => Model.Index; set { Model.Index = value; OnPropertyChanged(); } }
        public string Name { get => Model.Name; set { Model.Name = value; OnPropertyChanged(); } }
        public RoiShapeType Shape { get => Model.Shape; set { Model.Shape = value; OnPropertyChanged(); } }
        public double Row1 { get => Model.Row1; set { Model.Row1 = value; OnPropertyChanged(); } }
        public double Col1 { get => Model.Col1; set { Model.Col1 = value; OnPropertyChanged(); } }
        public double Row2 { get => Model.Row2; set { Model.Row2 = value; OnPropertyChanged(); } }
        public double Col2 { get => Model.Col2; set { Model.Col2 = value; OnPropertyChanged(); } }
        public double CenterRow { get => Model.CenterRow; set { Model.CenterRow = value; OnPropertyChanged(); } }
        public double CenterCol { get => Model.CenterCol; set { Model.CenterCol = value; OnPropertyChanged(); } }
        public double Angle { get => Model.Angle; set { Model.Angle = value; OnPropertyChanged(); } }
        public double Length1 { get => Model.Length1; set { Model.Length1 = value; OnPropertyChanged(); } }
        public double Length2 { get => Model.Length2; set { Model.Length2 = value; OnPropertyChanged(); } }
        public double Radius { get => Model.Radius; set { Model.Radius = value; OnPropertyChanged(); } }
    }
}



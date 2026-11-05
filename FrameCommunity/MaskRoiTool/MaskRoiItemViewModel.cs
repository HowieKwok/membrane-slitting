using System.ComponentModel;

namespace FrameCommunity.MaskRoiTool
{
    public sealed class MaskRoiItemViewModel : BindableOrdinary
    {
        public MaskRoiItemViewModel(MaskRoiParam model)
        {
            Model = model ?? new MaskRoiParam();
            Model.PropertyChanged += OnModelPropertyChanged;
        }

        public MaskRoiParam Model { get; }

        public bool Enabled { get { return Model.Enabled; } set { Model.Enabled = value; } }
        public int Index { get { return Model.Index; } set { Model.Index = value; } }
        public string Name { get { return Model.Name; } set { Model.Name = value; } }
        public MaskRoiShapeType Shape { get { return Model.Shape; } set { Model.Shape = value; } }
        public double Row1 { get { return Model.Row1; } set { Model.Row1 = value; UpdateRectangleDerived(); } }
        public double Col1 { get { return Model.Col1; } set { Model.Col1 = value; UpdateRectangleDerived(); } }
        public double Row2 { get { return Model.Row2; } set { Model.Row2 = value; UpdateRectangleDerived(); } }
        public double Col2 { get { return Model.Col2; } set { Model.Col2 = value; UpdateRectangleDerived(); } }
        public double CenterRow { get { return Model.CenterRow; } set { Model.CenterRow = value; } }
        public double CenterCol { get { return Model.CenterCol; } set { Model.CenterCol = value; } }
        public double Angle { get { return Model.Angle; } set { Model.Angle = value; } }
        public double Length1 { get { return Model.Length1; } set { Model.Length1 = value; } }
        public double Length2 { get { return Model.Length2; } set { Model.Length2 = value; } }
        public double Radius { get { return Model.Radius; } set { Model.Radius = value; } }
        public string Summary { get { return Model.Summary; } }

        public void Release()
        {
            Model.PropertyChanged -= OnModelPropertyChanged;
        }

        public void RefreshAll()
        {
            OnPropertyChanged(string.Empty);
        }

        private void UpdateRectangleDerived()
        {
            if (Model.Shape == MaskRoiShapeType.Rectangle1)
                Model.NormalizeFromRectangle1();
        }

        private void OnModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e == null || string.IsNullOrEmpty(e.PropertyName))
            {
                RefreshAll();
                return;
            }

            OnPropertyChanged(e.PropertyName);
            if (e.PropertyName != nameof(Summary))
                OnPropertyChanged(nameof(Summary));
        }
    }
}

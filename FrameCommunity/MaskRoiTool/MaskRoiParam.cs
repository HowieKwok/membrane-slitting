using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace FrameCommunity.MaskRoiTool
{
    [Serializable]
    public enum MaskRoiShapeType
    {
        Rectangle1 = 0,
        Rectangle2 = 1,
        Circle = 2
    }

    [Serializable]
    [XmlType("MaskRoiParam")]
    public class MaskRoiParam : INotifyPropertyChanged
    {
        private bool enabled = true;
        private int index = 1;
        private string name = "屏蔽ROI";
        private MaskRoiShapeType shape = MaskRoiShapeType.Rectangle1;
        private double row1;
        private double col1;
        private double row2 = 100;
        private double col2 = 100;
        private double centerRow = 50;
        private double centerCol = 50;
        private double angle;
        private double length1 = 50;
        private double length2 = 50;
        private double radius = 50;

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        [Description("是否启用该屏蔽ROI")]
        public bool Enabled { get { return enabled; } set { SetProperty(ref enabled, value); } }

        [Description("ROI序号")]
        public int Index { get { return index; } set { SetProperty(ref index, value); } }

        [Description("ROI名称")]
        public string Name { get { return name; } set { SetProperty(ref name, value ?? string.Empty); } }

        [Description("ROI形状")]
        public MaskRoiShapeType Shape
        {
            get { return shape; }
            set
            {
                if (!SetProperty(ref shape, value)) return;
                OnPropertyChanged(nameof(Summary));
            }
        }

        public double Row1 { get { return row1; } set { SetProperty(ref row1, value); } }
        public double Col1 { get { return col1; } set { SetProperty(ref col1, value); } }
        public double Row2 { get { return row2; } set { SetProperty(ref row2, value); } }
        public double Col2 { get { return col2; } set { SetProperty(ref col2, value); } }
        public double CenterRow { get { return centerRow; } set { SetProperty(ref centerRow, value); } }
        public double CenterCol { get { return centerCol; } set { SetProperty(ref centerCol, value); } }

        /// <summary>Rectangle2 uses radians.</summary>
        public double Angle { get { return angle; } set { SetProperty(ref angle, value); } }
        public double Length1 { get { return length1; } set { SetProperty(ref length1, value); } }
        public double Length2 { get { return length2; } set { SetProperty(ref length2, value); } }
        public double Radius { get { return radius; } set { SetProperty(ref radius, value); } }

        [XmlIgnore]
        public string Summary
        {
            get
            {
                switch (Shape)
                {
                    case MaskRoiShapeType.Rectangle2:
                        return string.Format("Center({0:0.##},{1:0.##}) L({2:0.##},{3:0.##})", CenterRow, CenterCol, Length1, Length2);
                    case MaskRoiShapeType.Circle:
                        return string.Format("Center({0:0.##},{1:0.##}) R{2:0.##}", CenterRow, CenterCol, Radius);
                    default:
                        return string.Format("R1C1({0:0.##},{1:0.##}) R2C2({2:0.##},{3:0.##})", Row1, Col1, Row2, Col2);
                }
            }
        }

        public MaskRoiParam Clone()
        {
            var clone = (MaskRoiParam)MemberwiseClone();
            clone.PropertyChanged = null;
            return clone;
        }

        public void NormalizeFromRectangle1()
        {
            double minRow = Math.Min(Row1, Row2);
            double maxRow = Math.Max(Row1, Row2);
            double minCol = Math.Min(Col1, Col2);
            double maxCol = Math.Max(Col1, Col2);

            Row1 = minRow;
            Row2 = maxRow;
            Col1 = minCol;
            Col2 = maxCol;
            CenterRow = (Row1 + Row2) / 2.0;
            CenterCol = (Col1 + Col2) / 2.0;
            Length1 = Math.Max(0.1, (Row2 - Row1) / 2.0);
            Length2 = Math.Max(0.1, (Col2 - Col1) / 2.0);
            OnPropertyChanged(nameof(Summary));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            if (propertyName != nameof(Summary)) OnPropertyChanged(nameof(Summary));
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

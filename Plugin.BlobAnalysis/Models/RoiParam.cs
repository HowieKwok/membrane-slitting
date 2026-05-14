using System;

namespace Plugin.BlobAnalysis.Models
{
    [Serializable]
    public class RoiParam
    {
        public bool Enabled { get; set; } = true;
        public int Index { get; set; }
        public string Name { get; set; } = "屏蔽ROI";
        public RoiShapeType Shape { get; set; } = RoiShapeType.Rectangle1;
        public double Row1 { get; set; }
        public double Col1 { get; set; }
        public double Row2 { get; set; }
        public double Col2 { get; set; }
        public double CenterRow { get; set; }
        public double CenterCol { get; set; }
        public double Angle { get; set; }
        public double Length1 { get; set; } = 50;
        public double Length2 { get; set; } = 20;
        public double Radius { get; set; } = 30;

        public RoiParam Clone()
        {
            return (RoiParam)MemberwiseClone();
        }
    }
}


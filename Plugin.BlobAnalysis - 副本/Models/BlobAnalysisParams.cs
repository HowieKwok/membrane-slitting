using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Plugin.BlobAnalysis.Models
{
    [Serializable]
    public class BlobAnalysisParams
    {
        [NonSerialized]
        [XmlIgnore]
        public HObject InputImage;

        public DetectRangeMode DetectMode { get; set; } = DetectRangeMode.LocalRoi;
        public RoiEditRole EditRole { get; set; } = RoiEditRole.Local;
        public bool EnableLocalRoi { get; set; } = true;
        public bool EnableMaskRoi { get; set; } = true;
        public bool ShowViewRoi { get; set; } = true;
        public bool EnablePositionFollow { get; set; } = false;

        public RoiParam LocalRoi { get; set; } = new RoiParam { Index = 1, Row1 = 1000, Col1 = 800, Row2 = 2600, Col2 = 3600 };
    
      
        public List<RoiParam> MaskRois { get; set; }=new List<RoiParam>();

        public double ThresholdMin { get; set; } = 0;
        public double ThresholdMax { get; set; } = 180;
        public MorphMode MorphMode { get; set; } = MorphMode.OpenClose;
        public double MorphRadius { get; set; } = 3.0;

        public SortFeature SortFeature { get; set; } = SortFeature.Area;
        public SortDirection SortDirection { get; set; } = SortDirection.Desc;

        public double BaseRow { get; set; }
        public double BaseCol { get; set; }
        public double BaseAngle { get; set; }
        public double CurrentRow { get; set; }
        public double CurrentCol { get; set; }
        public double CurrentAngle { get; set; }

        public List<FilterRuleParam> Filters { get; set; } = new List<FilterRuleParam>();

        public BlobAnalysisParams()
        {
        //    MaskRois = new List<RoiParam>();
        //    MaskRois.Add(new RoiParam { Index = 1, Enabled = true, Name = "屏蔽ROI-1", Shape = RoiShapeType.Rectangle1, Row1 = 1300, Col1 = 2100, Row2 = 1600, Col2 = 2600, CenterRow = 1450, CenterCol = 2350, Length1 = 250, Length2 = 150, Radius = 80 });
        //    Filters.Add(new FilterRuleParam { Feature = "area", Min = 200, Max = 999999, Operation = "and" });
        //    Filters.Add(new FilterRuleParam { Feature = "circularity", Min = 0.30, Max = 1.00, Operation = "and" });
        //    Filters.Add(new FilterRuleParam { Feature = "rectangularity", Min = 0.45, Max = 1.00, Operation = "and" });
        //
        }

        public void SetDefaultValues()
        {
            if (MaskRois.Count == 0)
            {
                MaskRois.Add(new RoiParam { Index = 1, Enabled = true, Name = "屏蔽ROI-1", Shape = RoiShapeType.Rectangle1, Row1 = 1300, Col1 = 2100, Row2 = 1600, Col2 = 2600, CenterRow = 1450, CenterCol = 2350, Length1 = 250, Length2 = 150, Radius = 80 });   }
            if (Filters.Count == 0)
            {
                Filters.Add(new FilterRuleParam { Feature = "area", Min = 200, Max = 999999, Operation = "and" });
                Filters.Add(new FilterRuleParam { Feature = "circularity", Min = 0.30, Max = 1.00, Operation = "and" });
                Filters.Add(new FilterRuleParam { Feature = "rectangularity", Min = 0.45, Max = 1.00, Operation = "and" });
            }
        }
        //[OnDeserialized]
        //void OnDeserialized(object sender)
        //{
        //    SetDefaultValues();
        //}
    }
}



using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Plugin.BlobAnalysis.Models
{
    public sealed class BlobRegionFeature
    {
        public int Index { get; set; }
        public double Area { get; set; }
        public double Row { get; set; }
        public double Col { get; set; }
        public double Rect2Row { get; set; }
        public double Rect2Col { get; set; }
        public double Ra { get; set; }
        public double Rb { get; set; }
        public double Ratio { get; set; }
        public double AngleDeg { get; set; }
        public double MeanGray { get; set; }
        public double DeviationGray { get; set; }
        public double MinGray { get; set; }
        public double MaxGray { get; set; }
        public double Circularity { get; set; }
        public double Rectangularity { get; set; }
        public double Phi { get; set; }
        public double Length1 { get; set; }
        public double Length2 { get; set; }

        [XmlIgnore]
        public double Gray
        {
            get { return MeanGray; }
            set { MeanGray = value; }
        }
    }

    public sealed class BlobAnalysisResult : IDisposable
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public long ElapsedMs { get; set; }
        public List<BlobRegionFeature> Regions { get; } = new List<BlobRegionFeature>();

        [XmlIgnore]
        public HObject ResultRegions { get; set; }

        [XmlIgnore]
        public HObject ThresholdRegions { get; set; }

        [XmlIgnore]
        public HObject LocalRoiRegion { get; set; }

        [XmlIgnore]
        public HObject MaskRoiRegion { get; set; }

        public void Dispose()
        {
            ResultRegions?.Dispose();
            ResultRegions = null;

            ThresholdRegions?.Dispose();
            ThresholdRegions = null;

            LocalRoiRegion?.Dispose();
            LocalRoiRegion = null;

            MaskRoiRegion?.Dispose();
            MaskRoiRegion = null;
        }
    }
}
using HalconDotNet;

namespace DefectRuleClassifier.Core.Models
{
    public sealed class DefectCandidate
    {
        public int Index { get; set; }
        public string Polarity { get; set; }
        public HObject Region { get; set; }
        public HObject CropImage { get; set; }
        public HObject DarkRegion { get; set; }
        public HObject LightRegion { get; set; }
        public HObject InvalidRegion { get; set; }
        public double WidthPx { get; set; }
        public double HeightPx { get; set; }
        public double PixelToMmH { get; set; } = 1;
        public double PixelToMmV { get; set; } = 1;
        public int ReferenceGray { get; set; }
        public DefectFeatureBag Features { get; set; } = new DefectFeatureBag();

        public void Dispose()
        {
            Region?.Dispose();
            Region = null;
            CropImage?.Dispose();
            CropImage = null;
            DarkRegion?.Dispose();
            DarkRegion = null;
            LightRegion?.Dispose();
            LightRegion = null;
            InvalidRegion?.Dispose();
            InvalidRegion = null;
        }
    }
}

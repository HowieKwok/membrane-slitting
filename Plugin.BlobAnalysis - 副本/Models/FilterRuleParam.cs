using System;

namespace Plugin.BlobAnalysis.Models
{
    [Serializable]
    public class FilterRuleParam
    {
        public bool Enabled { get; set; } = true;
        public string Feature { get; set; } = "area";
        public double Min { get; set; } = 0;
        public double Max { get; set; } = 999999;
        public string Operation { get; set; } = "and";
    }
}


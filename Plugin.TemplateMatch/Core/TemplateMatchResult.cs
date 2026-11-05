using Frame_Core.pluginParams;
using HalconDotNet;
using System;
using System.Collections.Generic;

namespace Plugin.TemplateMatch.Core
{
    public sealed class TemplateMatchResult : IDisposable
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public long ElapsedMs { get; set; }
        public HObject OutputContours { get; set; }
        public HObject OutputMatchRoiRegion { get; set; }
        public HObject OutputMaskRoiRegion { get; set; }
        public HTuple OutputMatrix { get; set; }
        public IList<TemplateMatchResultParam> Results { get; } = new List<TemplateMatchResultParam>();

        public void Dispose()
        {
            HObject contours = OutputContours;
            HalconObjectGuard.DisposeObject(ref contours);
            OutputContours = null;

            HObject matchRegion = OutputMatchRoiRegion;
            HalconObjectGuard.DisposeObject(ref matchRegion);
            OutputMatchRoiRegion = null;

            HObject maskRegion = OutputMaskRoiRegion;
            HalconObjectGuard.DisposeObject(ref maskRegion);
            OutputMaskRoiRegion = null;
            OutputMatrix = null;
        }
    }
}

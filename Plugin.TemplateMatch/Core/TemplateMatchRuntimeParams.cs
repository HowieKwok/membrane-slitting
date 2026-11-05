using Frame_Core.pluginParams;
using FrameCommunity.MaskRoiTool;
using System.Collections.Generic;

namespace Plugin.TemplateMatch.Core
{
    public sealed class TemplateMatchRuntimeParams
    {
        public bool UseLocalRoi { get; set; }
        public bool RoiFollowMatch { get; set; }
        public bool UsePositionFollow { get; set; }
        public double FollowBaseRow { get; set; }
        public double FollowBaseCol { get; set; }
        public double FollowBaseAngleDeg { get; set; }
        public double FollowCurrentRow { get; set; }
        public double FollowCurrentCol { get; set; }
        public double FollowCurrentAngleDeg { get; set; }
        public double LocalRoiRow1 { get; set; }
        public double LocalRoiCol1 { get; set; }
        public double LocalRoiRow2 { get; set; }
        public double LocalRoiCol2 { get; set; }
        public bool UseMaskRoi { get; set; }
        public IList<MaskRoiParam> MaskRois { get; set; }
        public TemplateFindOptions FindOptions { get; set; }
        public IList<TemplateModelParam> Templates { get; set; }
    }
}

using HalconDotNet;
using System.Collections.Generic;

namespace FrameCommunity.MaskRoiTool
{
    public static class MaskRoiRegionBuilder
    {
        public static HObject BuildUnion(IEnumerable<MaskRoiParam> rois, bool enableMaskRoi, MaskRoiFollowContext followContext)
        {
            if (!enableMaskRoi || rois == null) return null;

            HObject union = null;
            var transformer = new MaskRoiFollowTransformer(followContext);
            foreach (MaskRoiParam item in rois)
            {
                if (item == null || !item.Enabled) continue;
                HObject one = null;
                HObject concat = null;
                try
                {
                    MaskRoiParam roi = transformer.Transform(item);
                    one = CreateRegion(roi);
                    if (one == null || !one.IsInitialized()) continue;

                    if (union == null)
                    {
                        union = one.CopyObj(1, -1);
                    }
                    else
                    {
                        HOperatorSet.Union2(union, one, out concat);
                        union.Dispose();
                        union = concat;
                        concat = null;
                    }
                }
                finally
                {
                    if (one != null) one.Dispose();
                    if (concat != null) concat.Dispose();
                }
            }
            return union;
        }

        public static HObject CreateRegion(MaskRoiParam roi)
        {
            if (roi == null) return null;
            HObject region;
            switch (roi.Shape)
            {
                case MaskRoiShapeType.Rectangle2:
                    HOperatorSet.GenRectangle2(out region, roi.CenterRow, roi.CenterCol, roi.Angle, roi.Length1, roi.Length2);
                    break;
                case MaskRoiShapeType.Circle:
                    HOperatorSet.GenCircle(out region, roi.CenterRow, roi.CenterCol, roi.Radius);
                    break;
                default:
                    HOperatorSet.GenRectangle1(out region, roi.Row1, roi.Col1, roi.Row2, roi.Col2);
                    break;
            }
            return region;
        }
    }
}

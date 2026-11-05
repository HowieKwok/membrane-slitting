using HalconDotNet;
using System;
using System.Collections.Generic;

namespace FrameCommunity.MaskRoiTool
{
    public sealed class MaskRoiSearchDomainResult : IDisposable
    {
        public HObject EffectiveRegion { get; set; }
        public HObject MaskUnionRegion { get; set; }

        public void Dispose()
        {
            if (EffectiveRegion != null)
            {
                EffectiveRegion.Dispose();
                EffectiveRegion = null;
            }
            if (MaskUnionRegion != null)
            {
                MaskUnionRegion.Dispose();
                MaskUnionRegion = null;
            }
        }
    }

    public static class MaskRoiSearchDomainBuilder
    {
        public static MaskRoiSearchDomainResult Build(
            HObject image,
            bool useLocalRoi,
            double localRow1,
            double localCol1,
            double localRow2,
            double localCol2,
            bool enableMaskRoi,
            IEnumerable<MaskRoiParam> maskRois,
            MaskRoiFollowContext followContext)
        {
            if (image == null || !image.IsInitialized()) return new MaskRoiSearchDomainResult();

            HObject baseRegion = null;
            HObject maskUnion = null;
            HObject effective = null;
            try
            {
                if (useLocalRoi)
                {
                    HOperatorSet.GenRectangle1(out baseRegion,
                        Math.Min(localRow1, localRow2),
                        Math.Min(localCol1, localCol2),
                        Math.Max(localRow1, localRow2),
                        Math.Max(localCol1, localCol2));
                }
                else
                {
                    HOperatorSet.GetImageSize(image, out HTuple width, out HTuple height);
                    HOperatorSet.GenRectangle1(out baseRegion, 0, 0, height.I - 1, width.I - 1);
                }

                maskUnion = MaskRoiRegionBuilder.BuildUnion(maskRois, enableMaskRoi, followContext);
                if (maskUnion != null && maskUnion.IsInitialized())
                {
                    HOperatorSet.Difference(baseRegion, maskUnion, out effective);
                }
                else
                {
                    effective = baseRegion.CopyObj(1, -1);
                }

                HObject ownedEffective = effective;
                effective = null;
                return new MaskRoiSearchDomainResult
                {
                    EffectiveRegion = ownedEffective,
                    MaskUnionRegion = maskUnion == null ? null : maskUnion.CopyObj(1, -1)
                };
            }
            finally
            {
                if (baseRegion != null) baseRegion.Dispose();
                if (maskUnion != null) maskUnion.Dispose();
                if (effective != null) effective.Dispose();
            }
        }
    }
}

using Frame_Core.pluginParams;
using HalconDotNet;

namespace Plugin.TemplateMatch.Core.Matching
{
    internal sealed class ShapeTemplateMatcher : ITemplateMatcher
    {
        public TemplateMatchMode Mode => TemplateMatchMode.Shape;

        public void Create(HObject image, HObject reduceImage, HObject region, TemplateModelParam template, TemplateCreateOptions options, out HTuple modelId, out HObject previewContours)
        {
            HOperatorSet.TupleRad(options.AngleStartDeg, out HTuple angleStart);
            HOperatorSet.TupleRad(options.AngleExtentDeg, out HTuple angleExtent);
            object numLevels = options.NumLevels == 11 ? (object)"auto" : options.NumLevels;
            object angleStep = System.Math.Abs(options.AngleStepDeg) < 0.000001 ? (object)"auto" : AngleMath.DegToRad(options.AngleStepDeg);

            HOperatorSet.CreateShapeModel(
                reduceImage,
                new HTuple(numLevels),
                angleStart,
                angleExtent,
                new HTuple(angleStep),
                options.Optimization,
                options.Metric,
                options.Contrast,
                options.MinContrast,
                out modelId);

            HOperatorSet.AreaCenter(region, out HTuple area, out HTuple centerRow, out HTuple centerCol);
            HOperatorSet.SetShapeModelOrigin(modelId, template.BaseRow - centerRow.D, template.BaseCol - centerCol.D);

            HObject modelContours = null;
            try
            {
                HOperatorSet.GetShapeModelContours(out modelContours, modelId, 1);
                HOperatorSet.VectorAngleToRigid(0, 0, 0, template.BaseRow, template.BaseCol, AngleMath.DegToRad(template.BaseAngleDeg), out HTuple homMat2D);
                HOperatorSet.AffineTransContourXld(modelContours, out previewContours, homMat2D);
            }
            finally
            {
                HalconObjectGuard.DisposeObject(ref modelContours);
            }
        }

        public void Find(HObject image, TemplateModelParam template, TemplateFindOptions options, out HTuple row, out HTuple col, out HTuple angle, out HTuple score)
        {
            HOperatorSet.SetShapeModelParam(template.ModelId, "timeout", System.Math.Max(1, options.TimeoutMs));
            HOperatorSet.TupleRad(options.AngleStartDeg, out HTuple angleStart);
            HOperatorSet.TupleRad(options.AngleExtentDeg, out HTuple angleExtent);
            HOperatorSet.FindShapeModel(
                image,
                template.ModelId,
                angleStart,
                angleExtent,
                options.MinScore,
                options.NumMatches,
                options.MaxOverlap,
                options.SubPixel,
                options.NumLevels,
                options.Greediness,
                out row,
                out col,
                out angle,
                out score);
        }

        public void ClearModel(HTuple modelId)
        {
            HOperatorSet.ClearShapeModel(modelId);
        }
    }
}

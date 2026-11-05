using Frame_Core.pluginParams;
using HalconDotNet;

namespace Plugin.TemplateMatch.Core.Matching
{
    internal sealed class GrayNccTemplateMatcher : ITemplateMatcher
    {
        public TemplateMatchMode Mode => TemplateMatchMode.Gray;

        public void Create(HObject image, HObject reduceImage, HObject region, TemplateModelParam template, TemplateCreateOptions options, out HTuple modelId, out HObject previewContours)
        {
            HOperatorSet.TupleRad(options.AngleStartDeg, out HTuple angleStart);
            HOperatorSet.TupleRad(options.AngleExtentDeg, out HTuple angleExtent);
            object numLevels = options.NumLevels == 11 ? (object)"auto" : options.NumLevels;
            object angleStep = System.Math.Abs(options.AngleStepDeg) < 0.000001 ? (object)"auto" : AngleMath.DegToRad(options.AngleStepDeg);

            HOperatorSet.CreateNccModel(
                reduceImage,
                new HTuple(numLevels),
                angleStart,
                angleExtent,
                new HTuple(angleStep),
                options.Metric,
                out modelId);

            CreateRectangleContour(template, template.BaseRow, template.BaseCol, AngleMath.DegToRad(template.BaseAngleDeg), out previewContours);
        }

        public void Find(HObject image, TemplateModelParam template, TemplateFindOptions options, out HTuple row, out HTuple col, out HTuple angle, out HTuple score)
        {
            HOperatorSet.TupleRad(options.AngleStartDeg, out HTuple angleStart);
            HOperatorSet.TupleRad(options.AngleExtentDeg, out HTuple angleExtent);
            HOperatorSet.SetNccModelParam(template.ModelId, "timeout", System.Math.Max(1, options.TimeoutMs));
            HOperatorSet.FindNccModel(
                image,
                template.ModelId,
                angleStart,
                angleExtent,
                options.MinScore,
                options.NumMatches,
                options.MaxOverlap,
                ToNccSubPixel(options.SubPixel),
                options.NumLevels,
                out HTuple modelRow,
                out HTuple modelCol,
                out angle,
                out score);

            double centerRow = (template.RoiRow1 + template.RoiRow2) / 2.0;
            double centerCol = (template.RoiCol1 + template.RoiCol2) / 2.0;
            double baseOffsetRow = template.BaseRow - centerRow;
            double baseOffsetCol = template.BaseCol - centerCol;

            row = new HTuple();
            col = new HTuple();
            for (int i = 0; i < modelRow.Length; i++)
            {
                double deltaAngle = angle[i].D - AngleMath.DegToRad(template.BaseAngleDeg);
                double cos = System.Math.Cos(deltaAngle);
                double sin = System.Math.Sin(deltaAngle);
                double currentBaseRow = modelRow[i].D + baseOffsetRow * cos - baseOffsetCol * sin;
                double currentBaseCol = modelCol[i].D + baseOffsetRow * sin + baseOffsetCol * cos;
                row = row.TupleConcat(currentBaseRow);
                col = col.TupleConcat(currentBaseCol);
            }
        }

        public void ClearModel(HTuple modelId)
        {
            HOperatorSet.ClearNccModel(modelId);
        }

        private static string ToNccSubPixel(string subPixel)
        {
            return string.Equals(subPixel, "none", System.StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(subPixel, "false", System.StringComparison.OrdinalIgnoreCase)
                ? "false"
                : "true";
        }

        internal static void CreateRectangleContour(TemplateModelParam template, HTuple targetBaseRow, HTuple targetBaseCol, HTuple targetAngle, out HObject contour)
        {
            HOperatorSet.VectorAngleToRigid(template.BaseRow, template.BaseCol, AngleMath.DegToRad(template.BaseAngleDeg), targetBaseRow, targetBaseCol, targetAngle, out HTuple mat);
            HOperatorSet.GenContourPolygonXld(out HObject rect,
                new HTuple(new[] { template.RoiRow1, template.RoiRow1, template.RoiRow2, template.RoiRow2, template.RoiRow1 }),
                new HTuple(new[] { template.RoiCol1, template.RoiCol2, template.RoiCol2, template.RoiCol1, template.RoiCol1 }));
            HOperatorSet.AffineTransContourXld(rect, out contour, mat);
            rect.Dispose();
        }
    }
}

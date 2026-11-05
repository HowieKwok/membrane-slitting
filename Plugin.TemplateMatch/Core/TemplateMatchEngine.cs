using Frame_Core.pluginParams;
using FrameCommunity.MaskRoiTool;
using HalconDotNet;
using Plugin.TemplateMatch.Core.Matching;
using System;
using System.Diagnostics;
using System.Linq;

namespace Plugin.TemplateMatch.Core
{
    public sealed class TemplateMatchEngine : IDisposable
    {
        private readonly TemplateMatcherFactory matcherFactory = new TemplateMatcherFactory();
        private readonly TemplateStorageService storageService = new TemplateStorageService();

        public void Initialize(ParamsTemplateMatch pluginParams, string templateRootDir)
        {
            if (pluginParams == null) throw new ArgumentNullException(nameof(pluginParams));
            storageService.LoadTemplates(pluginParams.Templates, templateRootDir);
        }

        public TemplateMatchResult Run(HObject image, TemplateMatchRuntimeParams runtime)
        {
            TemplateMatchResult result = new TemplateMatchResult();
            Stopwatch watch = Stopwatch.StartNew();
            HObject contours = null;
            try
            {
                if (image == null || !image.IsInitialized())
                {
                    result.Success = false;
                    result.Message = "输入图像为空";
                    AddFailureRow(result);
                    return result;
                }

                if (runtime == null || runtime.Templates == null || runtime.Templates.Count == 0)
                {
                    result.Success = false;
                    result.Message = "没有可用模板";
                    AddFailureRow(result);
                    return result;
                }

                foreach (TemplateModelParam template in runtime.Templates)
                {
                    if (template == null || !template.IsEnabled || !template.IsCreated || template.ModelId == null || template.ModelId.Length == 0)
                        continue;

                    ITemplateMatcher matcher = matcherFactory.Resolve(template.MatchMode);
                    TemplateFindOptions options = template.FindOptions ?? runtime.FindOptions;
                    if (runtime.UseLocalRoi && runtime.UsePositionFollow)
                        RunWithPositionFollowLocalRoi(image, runtime, template, matcher, options, result, ref contours);
                    else

                        RunWithStaticLocalRoi(image, runtime, template, matcher, options, false, result, ref contours);
                }

                result.OutputContours = contours;
                contours = null;
                result.Success = result.Results.Count > 0;
                result.Message = result.Success ? "匹配成功" : "未找到匹配结果";
                if (!result.Success)
                    AddFailureRow(result);
                return result;
            }
            finally
            {
                watch.Stop();
                result.ElapsedMs = watch.ElapsedMilliseconds;
                HalconObjectGuard.DisposeObject(ref contours);
            }
        }

        private void RunWithPositionFollowLocalRoi(HObject image, TemplateMatchRuntimeParams runtime, TemplateModelParam template, ITemplateMatcher matcher, TemplateFindOptions options, TemplateMatchResult result, ref HObject contours)
        {
            HObject followRegion = null;
            HObject followImage = null;
            try
            {
                if (!TryBuildPositionFollowMatrix(runtime, out HTuple baseToCurrent))
                {
                    result.Message = "位置跟随矩阵生成失败";
                    return;
                }

                GetFollowLocalRoi(runtime, out double followRow1, out double followCol1, out double followRow2, out double followCol2);
                BuildFindImage(image, runtime, true, followRow1, followCol1, followRow2, followCol2, out followImage, out followRegion, result);
                matcher.Find(followImage, template, options, out HTuple row, out HTuple col, out HTuple angle, out HTuple score);
                if (row == null || row.Length == 0)
                {
                    result.Message = "位置跟随后未匹配到结果";
                    return;
                }

                AddMatchRows(result, runtime, template, row, col, angle, score, true, followRow1, followCol1, followRow2, followCol2, ref contours);
            }
            finally
            {
                HalconObjectGuard.DisposeObject(ref followImage);
                HalconObjectGuard.DisposeObject(ref followRegion);
            }
        }

        private static bool TryBuildPositionFollowMatrix(TemplateMatchRuntimeParams runtime, out HTuple baseToCurrent)
        {
            baseToCurrent = null;
            if (runtime == null || !runtime.UsePositionFollow) return false;
            HOperatorSet.VectorAngleToRigid(
                runtime.FollowBaseRow,
                runtime.FollowBaseCol,
                AngleMath.DegToRad(runtime.FollowBaseAngleDeg),
                runtime.FollowCurrentRow,
                runtime.FollowCurrentCol,
                AngleMath.DegToRad(runtime.FollowCurrentAngleDeg),
                out baseToCurrent);
            return baseToCurrent != null && baseToCurrent.Length > 0;
        }

        private void RunWithStaticLocalRoi(HObject image, TemplateMatchRuntimeParams runtime, TemplateModelParam template, ITemplateMatcher matcher, TemplateFindOptions options, bool hasFollowRoi, TemplateMatchResult result, ref HObject contours)
        {
            HObject findImage = null;
            HObject staticRegion = null;
            try
            {
                BuildFindImage(image, runtime, runtime.UseLocalRoi, runtime.LocalRoiRow1, runtime.LocalRoiCol1, runtime.LocalRoiRow2, runtime.LocalRoiCol2, out findImage, out staticRegion, result);
                matcher.Find(findImage, template, options, out HTuple row, out HTuple col, out HTuple angle, out HTuple score);
                AddMatchRows(result, runtime, template, row, col, angle, score, hasFollowRoi, runtime.LocalRoiRow1, runtime.LocalRoiCol1, runtime.LocalRoiRow2, runtime.LocalRoiCol2, ref contours);
            }
            finally
            {
                HalconObjectGuard.DisposeObject(ref findImage);
                HalconObjectGuard.DisposeObject(ref staticRegion);
            }
        }

        private static void BuildFindImage(HObject image, TemplateMatchRuntimeParams runtime, bool useLocalRoi, double row1, double col1, double row2, double col2, out HObject findImage, out HObject localRegion, TemplateMatchResult result)
        {
            localRegion = null;
            MaskRoiSearchDomainResult domain = null;
            try
            {
                domain = MaskRoiSearchDomainBuilder.Build(
                    image,
                    useLocalRoi,
                    row1,
                    col1,
                    row2,
                    col2,
                    runtime != null && runtime.UseMaskRoi,
                    runtime == null ? null : runtime.MaskRois,
                    CreateFollowContext(runtime));

                if (domain == null || domain.EffectiveRegion == null || !domain.EffectiveRegion.IsInitialized())
                {
                    findImage = image.CopyObj(1, 1);
                    return;
                }

                localRegion = domain.EffectiveRegion.CopyObj(1, -1);
                HOperatorSet.ReduceDomain(image, domain.EffectiveRegion, out findImage);
                AppendMaskRegion(result, domain.MaskUnionRegion);
            }
            finally
            {
                if (domain != null) domain.Dispose();
            }
        }

        private static MaskRoiFollowContext CreateFollowContext(TemplateMatchRuntimeParams runtime)
        {
            if (runtime == null) return new MaskRoiFollowContext();
            return new MaskRoiFollowContext
            {
                EnablePositionFollow = runtime.UsePositionFollow,
                BaseRow = runtime.FollowBaseRow,
                BaseCol = runtime.FollowBaseCol,
                BaseAngleDeg = runtime.FollowBaseAngleDeg,
                CurrentRow = runtime.FollowCurrentRow,
                CurrentCol = runtime.FollowCurrentCol,
                CurrentAngleDeg = runtime.FollowCurrentAngleDeg
            };
        }

        private static void AppendMaskRegion(TemplateMatchResult result, HObject maskRegion)
        {
            if (result == null || maskRegion == null || !maskRegion.IsInitialized()) return;
            if (result.OutputMaskRoiRegion == null || !result.OutputMaskRoiRegion.IsInitialized())
            {
                result.OutputMaskRoiRegion = maskRegion.CopyObj(1, -1);
                return;
            }

            HObject concat = null;
            try
            {
                HOperatorSet.Union2(result.OutputMaskRoiRegion, maskRegion, out concat);
                result.OutputMaskRoiRegion.Dispose();
                result.OutputMaskRoiRegion = concat;
                concat = null;
            }
            finally
            {
                if (concat != null) concat.Dispose();
            }
        }

        private static void AddFailureRow(TemplateMatchResult result)
        {
            result.Results.Add(new TemplateMatchResultParam
            {
                TemplateName = "-",
                StatusText = "NG",
                ErrorMessage = result.Message,
                IsSuccess = false,
                HasFollowRoi = false
            });
        }

        private static void AddMatchRows(TemplateMatchResult result, TemplateMatchRuntimeParams runtime, TemplateModelParam template, HTuple row, HTuple col, HTuple angle, HTuple score, bool hasLocalFollowRoi, double localFollowRow1, double localFollowCol1, double localFollowRow2, double localFollowCol2, ref HObject contours)
        {
            if (row == null || row.Length == 0) return;
            for (int i = 0; i < row.Length; i++)
            {
                if (i == 0 || score[i].D > template.LastScore)
                    template.LastScore = score[i].D;

                double angleDeg = AngleMath.RadToDeg(angle[i].D);
                HOperatorSet.VectorAngleToRigid(template.BaseRow, template.BaseCol, AngleMath.DegToRad(template.BaseAngleDeg), row[i].D, col[i].D, angle[i].D, out HTuple baseToCurrent);
                HOperatorSet.VectorAngleToRigid(row[i].D, col[i].D, angle[i].D, template.BaseRow, template.BaseCol, AngleMath.DegToRad(template.BaseAngleDeg), out HTuple currentToBase);
                result.Results.Add(new TemplateMatchResultParam
                {
                    TemplateName = template.Name,
                    MatchMode = template.MatchMode,
                    Index = i + 1,
                    Row = row[i].D,
                    Col = col[i].D,
                    AngleDeg = angleDeg,
                    Score = score[i].D,
                    OffsetX = col[i].D - template.BaseCol,
                    OffsetY = row[i].D - template.BaseRow,
                    OffsetRDeg = angleDeg - template.BaseAngleDeg,
                    MatrixText = FormatMatrix(baseToCurrent),
                    BaseToCurrentMatrixText = FormatMatrix(baseToCurrent),
                    CurrentToBaseMatrixText = FormatMatrix(currentToBase),
                    IsSuccess = true,
                    HasFollowRoi = hasLocalFollowRoi,
                    FollowRow1 = localFollowRow1,
                    FollowCol1 = localFollowCol1,
                    FollowRow2 = localFollowRow2,
                    FollowCol2 = localFollowCol2,
                    StatusText = "成功"
                });

                if (result.OutputMatrix == null)
                    result.OutputMatrix = baseToCurrent;

                if (runtime != null && runtime.RoiFollowMatch)
                {
                    HObject matchRegion = result.OutputMatchRoiRegion;
                    AppendMatchFollowContour(template, baseToCurrent, row[i].D, col[i].D, angle[i].D, ref matchRegion);
                    result.OutputMatchRoiRegion = matchRegion;
                }

                AppendContour(template, row[i], col[i], angle[i], ref contours);
            }
        }

        private static void AppendMatchFollowContour(TemplateModelParam template, HTuple baseToCurrent, double currentRow, double currentCol, double currentAngleRad, ref HObject target)
        {
            HObject trans = null;
            HObject transformed = null;
            try
            {

                HOperatorSet.GenRectangle1(out trans, template.RoiRow1,
                    template.RoiCol1,
                    template.RoiRow2,
                    template.RoiCol2);

                HOperatorSet.AffineTransRegion(trans, out transformed, baseToCurrent, "nearest_neighbor");

                //TransformRectCorners(
                //    template.RoiRow1,
                //    template.RoiCol1,
                //    template.RoiRow2,
                //    template.RoiCol2,
                //    template.BaseRow,
                //    template.BaseCol,
                //    template.BaseAngleDeg,
                //    currentRow,
                //    currentCol,
                //    AngleMath.RadToDeg(currentAngleRad),
                //    out HTuple rows,
                //    out HTuple cols);
                //HOperatorSet.GenContourPolygonXld(out trans, rows, cols);
              
                if (target == null || !target.IsInitialized())
                    HOperatorSet.CopyObj(transformed, out target, 1, -1);
                else
                {
                    HOperatorSet.ConcatObj(target, transformed, out HObject concat);
                    target.Dispose();
                    target = concat;
                }
            }
            finally
            {
                HalconObjectGuard.DisposeObject(ref trans);
                HalconObjectGuard.DisposeObject(ref transformed);
            }
        }

        private static void AppendContour(TemplateModelParam template, HTuple row, HTuple col, HTuple angle, ref HObject target)
        {
            HObject one = null;
            HObject transformed = null;
            try
            {
                if (template.MatchMode == TemplateMatchMode.Gray)
                {
                    HOperatorSet.GenContourPolygonXld(out one,
                        new HTuple(new[] { template.RoiRow1, template.RoiRow1, template.RoiRow2, template.RoiRow2, template.RoiRow1 }),
                        new HTuple(new[] { template.RoiCol1, template.RoiCol2, template.RoiCol2, template.RoiCol1, template.RoiCol1 }));
                    HOperatorSet.VectorAngleToRigid(template.BaseRow, template.BaseCol, AngleMath.DegToRad(template.BaseAngleDeg), row, col, angle, out HTuple mat);
                    HOperatorSet.AffineTransContourXld(one, out transformed, mat);
                }
                else
                {
                    HObject modelContours = null;
                    try
                    {
                        HOperatorSet.GetShapeModelContours(out modelContours, template.ModelId, 1);
                        HOperatorSet.VectorAngleToRigid(0, 0, 0, row, col, angle, out HTuple mat);
                        HOperatorSet.AffineTransContourXld(modelContours, out transformed, mat);
                    }
                    finally
                    {
                        HalconObjectGuard.DisposeObject(ref modelContours);
                    }
                }

                HObject contourToAppend = transformed ?? one;
                if (target == null || !target.IsInitialized())
                    HOperatorSet.CopyObj(contourToAppend, out target, 1, -1);
                else
                {
                    HOperatorSet.ConcatObj(target, contourToAppend, out HObject concat);
                    target.Dispose();
                    target = concat;
                }
            }
            finally
            {
                HalconObjectGuard.DisposeObject(ref one);
                HalconObjectGuard.DisposeObject(ref transformed);
            }
        }

        private static void GetFollowLocalRoi(TemplateMatchRuntimeParams runtime, out double row1, out double col1, out double row2, out double col2)
        {
            TransformRectCorners(
                runtime.LocalRoiRow1,
                runtime.LocalRoiCol1,
                runtime.LocalRoiRow2,
                runtime.LocalRoiCol2,
                runtime.FollowBaseRow,
                runtime.FollowBaseCol,
                runtime.FollowBaseAngleDeg,
                runtime.FollowCurrentRow,
                runtime.FollowCurrentCol,
                runtime.FollowCurrentAngleDeg,
                out HTuple rows,
                out HTuple cols);

            row1 = rows.TupleMin().D;
            col1 = cols.TupleMin().D;
            row2 = rows.TupleMax().D;
            col2 = cols.TupleMax().D;
        }

        private static void TransformRectCorners(double row1, double col1, double row2, double col2, double baseRow, double baseCol, double baseAngleDeg, double currentRow, double currentCol, double currentAngleDeg, out HTuple rows, out HTuple cols)
        {
            double[] srcRows = { row1, row1, row2, row2, row1 };
            double[] srcCols = { col1, col2, col2, col1, col1 };
            double[] dstRows = new double[srcRows.Length];
            double[] dstCols = new double[srcCols.Length];
            double deltaAngleRad = AngleMath.DegToRad(currentAngleDeg - baseAngleDeg);
            double cos = Math.Cos(deltaAngleRad);
            double sin = Math.Sin(deltaAngleRad);

            for (int i = 0; i < srcRows.Length; i++)
            {
                double dr = srcRows[i] - baseRow;
                double dc = srcCols[i] - baseCol;
                dstRows[i] = currentRow + dr * cos - dc * sin;
                dstCols[i] = currentCol + dr * sin + dc * cos;
            }

            rows = new HTuple(dstRows);
            cols = new HTuple(dstCols);
        }

        private static string FormatMatrix(HTuple matrix)
        {
            if (matrix == null || matrix.Length == 0) return string.Empty;
            return string.Join(",", matrix.DArr.Select(v => v.ToString("0.######")));
        }

        public void Dispose()
        {
        }
    }
}

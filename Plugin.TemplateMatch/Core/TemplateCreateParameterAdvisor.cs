using Frame_Core.pluginParams;
using HalconDotNet;
using Plugin.TemplateMatch.Core.Matching;
using System;
using System.Collections.Generic;

namespace Plugin.TemplateMatch.Core
{
    internal sealed class TemplateCreateParameterSuggestion
    {
        public TemplateMatchMode MatchMode { get; set; }
        public int NumLevels { get; set; }
        public double AngleStepDeg { get; set; }
        public string Optimization { get; set; }
        public string Metric { get; set; }
        public int Contrast { get; set; }
        public int MinContrast { get; set; }
    }

    internal sealed class TemplateCreateParameterAdvisor
    {
        public TemplateCreateParameterSuggestion Determine(HObject reduceImage, TemplateCreateOptions options)
        {
            if (reduceImage == null || !reduceImage.IsInitialized()) throw new ArgumentException("模板图像为空", nameof(reduceImage));
            if (options == null) throw new ArgumentNullException(nameof(options));

            return options.MatchMode == TemplateMatchMode.Gray
                ? DetermineNcc(reduceImage, options)
                : DetermineShape(reduceImage, options);
        }

        private static TemplateCreateParameterSuggestion DetermineShape(HObject reduceImage, TemplateCreateOptions options)
        {
            HOperatorSet.TupleRad(options.AngleStartDeg, out HTuple angleStart);
            HOperatorSet.TupleRad(options.AngleExtentDeg, out HTuple angleExtent);
            HOperatorSet.DetermineShapeModelParams(
                reduceImage,
                ToAutoNumLevels(options.NumLevels),
                angleStart,
                angleExtent,
                1.0,
                1.0,
                options.Optimization,
                options.Metric,
                options.Contrast,
                options.MinContrast,
                "all",
                out HTuple names,
                out HTuple values);

            Dictionary<string, HTuple> map = ToMap(names, values);
            return new TemplateCreateParameterSuggestion
            {
                MatchMode = TemplateMatchMode.Shape,
                NumLevels = GetInt(map, "num_levels", options.NumLevels),
                AngleStepDeg = AngleMath.RadToDeg(GetDouble(map, "angle_step", AngleMath.DegToRad(options.AngleStepDeg))),
                Optimization = GetString(map, "optimization", options.Optimization),
                Metric = GetString(map, "metric", options.Metric),
                Contrast = GetInt(map, "contrast", options.Contrast),
                MinContrast = GetInt(map, "min_contrast", options.MinContrast)
            };
        }

        private static TemplateCreateParameterSuggestion DetermineNcc(HObject reduceImage, TemplateCreateOptions options)
        {
            HOperatorSet.TupleRad(options.AngleStartDeg, out HTuple angleStart);
            HOperatorSet.TupleRad(options.AngleExtentDeg, out HTuple angleExtent);
            HOperatorSet.Rgb1ToGray(reduceImage,out HObject grayImage);
            HOperatorSet.DetermineNccModelParams(
                grayImage,
                ToAutoNumLevels(options.NumLevels),
                angleStart,
                angleExtent,
                options.Metric,
                "all",
                out HTuple names,
                out HTuple values);

            grayImage?.Dispose();

            Dictionary<string, HTuple> map = ToMap(names, values);
            return new TemplateCreateParameterSuggestion
            {
                MatchMode = TemplateMatchMode.Gray,
                NumLevels = GetInt(map, "num_levels", options.NumLevels),
                AngleStepDeg = AngleMath.RadToDeg(GetDouble(map, "angle_step", AngleMath.DegToRad(options.AngleStepDeg))),
                Optimization = options.Optimization,
                Metric = GetString(map, "metric", options.Metric),
                Contrast = options.Contrast,
                MinContrast = options.MinContrast
            };
        }

        private static HTuple ToAutoNumLevels(int numLevels)
        {
            return numLevels == 11 ? new HTuple("auto") : new HTuple(numLevels);
        }

        private static Dictionary<string, HTuple> ToMap(HTuple names, HTuple values)
        {
            Dictionary<string, HTuple> map = new Dictionary<string, HTuple>(StringComparer.OrdinalIgnoreCase);
            if (names == null || values == null) return map;
            int count = Math.Min(names.Length, values.Length);
            for (int i = 0; i < count; i++)
                map[names[i].S] = values[i];
            return map;
        }

        private static int GetInt(Dictionary<string, HTuple> map, string key, int fallback)
        {
            if (!map.TryGetValue(key, out HTuple value)) return fallback;
            try
            {
                if (value.Type == HTupleType.STRING)
                    return string.Equals(value.S, "auto", StringComparison.OrdinalIgnoreCase) ? 11 : fallback;
                return value.I;
            }
            catch
            {
                return fallback;
            }
        }

        private static double GetDouble(Dictionary<string, HTuple> map, string key, double fallback)
        {
            if (!map.TryGetValue(key, out HTuple value)) return fallback;
            try
            {
                if (value.Type == HTupleType.STRING)
                    return double.TryParse(value.S, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out double parsed) ? parsed : fallback;
                return value.D;
            }
            catch
            {
                return fallback;
            }
        }

        private static string GetString(Dictionary<string, HTuple> map, string key, string fallback)
        {
            if (!map.TryGetValue(key, out HTuple value)) return fallback;
            try { return value.S; }
            catch { return value.ToString(); }
        }
    }
}

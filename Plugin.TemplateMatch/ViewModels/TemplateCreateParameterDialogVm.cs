using Frame_Core.pluginParams;
using Plugin.TemplateMatch.Core;

namespace Plugin.TemplateMatch.ViewModels
{
    internal sealed class TemplateCreateParameterDialogVm : BindableBase
    {
        private int numLevels;
        private double angleStartDeg;
        private double angleExtentDeg;
        private double angleStepDeg;
        private string optimization;
        private string metric;
        private int contrast;
        private int minContrast;

        public TemplateCreateParameterDialogVm(TemplateCreateOptions current, TemplateCreateParameterSuggestion suggestion)
        {
            current = current ?? new TemplateCreateOptions();
            Suggestion = suggestion;
            MatchModeText = current.MatchMode == TemplateMatchMode.Gray ? "灰度匹配" : "形状匹配";
            NumLevels = current.NumLevels;
            AngleStartDeg = current.AngleStartDeg;
            AngleExtentDeg = current.AngleExtentDeg;
            AngleStepDeg = current.AngleStepDeg;
            Optimization = current.Optimization;
            Metric = current.Metric;
            Contrast = current.Contrast;
            MinContrast = current.MinContrast;
        }

        public string MatchModeText { get; private set; }
        public TemplateCreateParameterSuggestion Suggestion { get; private set; }
        public string[] OptimizationOptions { get { return new[] { "auto", "none", "point_reduction_low", "point_reduction_medium", "point_reduction_high", "pregeneration" }; } }
        public string[] MetricOptions { get { return new[] { "use_polarity", "ignore_global_polarity", "ignore_local_polarity", "ignore_color_polarity" }; } }

        public int NumLevels { get { return numLevels; } set { SetProperty(ref numLevels, value); } }
        public double AngleStartDeg { get { return angleStartDeg; } set { SetProperty(ref angleStartDeg, value); } }
        public double AngleExtentDeg { get { return angleExtentDeg; } set { SetProperty(ref angleExtentDeg, value); } }
        public double AngleStepDeg { get { return angleStepDeg; } set { SetProperty(ref angleStepDeg, value); } }
        public string Optimization { get { return optimization; } set { SetProperty(ref optimization, value ?? "auto"); } }
        public string Metric { get { return metric; } set { SetProperty(ref metric, value ?? "use_polarity"); } }
        public int Contrast { get { return contrast; } set { SetProperty(ref contrast, value); } }
        public int MinContrast { get { return minContrast; } set { SetProperty(ref minContrast, value); } }

        public string SuggestNumLevels { get { return Suggestion == null ? "-" : Suggestion.NumLevels.ToString(); } }
        public string SuggestAngleStepDeg { get { return Suggestion == null ? "-" : Suggestion.AngleStepDeg.ToString("0.######"); } }
        public string SuggestOptimization { get { return Suggestion == null ? "-" : Suggestion.Optimization; } }
        public string SuggestMetric { get { return Suggestion == null ? "-" : Suggestion.Metric; } }
        public string SuggestContrast { get { return Suggestion == null ? "-" : Suggestion.Contrast.ToString(); } }
        public string SuggestMinContrast { get { return Suggestion == null ? "-" : Suggestion.MinContrast.ToString(); } }

        public void ApplyTo(TemplateCreateOptions target)
        {
            if (target == null) return;
            target.NumLevels = NumLevels;
            target.AngleStartDeg = AngleStartDeg;
            target.AngleExtentDeg = AngleExtentDeg;
            target.AngleStepDeg = AngleStepDeg;
            target.Optimization = Optimization;
            target.Metric = Metric;
            target.Contrast = Contrast;
            target.MinContrast = MinContrast;
        }
    }
}

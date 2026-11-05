using Frame_Core.pluginParams;
using HalconDotNet;

namespace Plugin.TemplateMatch.Core.Matching
{
    internal interface ITemplateMatcher
    {
        TemplateMatchMode Mode { get; }
        void Create(HObject image, HObject reduceImage, HObject region, TemplateModelParam template, TemplateCreateOptions options, out HTuple modelId, out HObject previewContours);
        void Find(HObject image, TemplateModelParam template, TemplateFindOptions options, out HTuple row, out HTuple col, out HTuple angle, out HTuple score);
        void ClearModel(HTuple modelId);
    }
}

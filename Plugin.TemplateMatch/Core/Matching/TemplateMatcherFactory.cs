using Frame_Core.pluginParams;

namespace Plugin.TemplateMatch.Core.Matching
{
    internal sealed class TemplateMatcherFactory
    {
        private readonly ShapeTemplateMatcher shapeMatcher = new ShapeTemplateMatcher();
        private readonly GrayNccTemplateMatcher grayMatcher = new GrayNccTemplateMatcher();

        public ITemplateMatcher Resolve(TemplateMatchMode mode)
        {
            return mode == TemplateMatchMode.Gray ? (ITemplateMatcher)grayMatcher : shapeMatcher;
        }
    }
}

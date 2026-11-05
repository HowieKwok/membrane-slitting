using Frame_Core.pluginParams;
using HalconDotNet;

namespace Plugin.TemplateMatch.Core
{
    internal static class HalconObjectGuard
    {
        public static void DisposeObject(ref HObject obj)
        {
            if (obj == null) return;
            try { obj.Dispose(); } catch { }
            obj = null;
        }

        public static void ClearTemplateModel(TemplateMatchMode mode, ref HTuple modelId)
        {
            if (modelId == null || modelId.Length == 0) return;
            try
            {
                if (mode == TemplateMatchMode.Gray)
                    HOperatorSet.ClearNccModel(modelId);
                else
                    HOperatorSet.ClearShapeModel(modelId);
            }
            catch
            {
            }
            modelId = null;
        }
    }
}

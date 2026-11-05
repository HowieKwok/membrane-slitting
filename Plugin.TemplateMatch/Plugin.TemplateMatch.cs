using Frame_Core;
using Frame_Core.pluginParams;
using HalconDotNet;
using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using Plugin.TemplateMatch.Core;

namespace Plugin.TemplateMatch
{
    [Category("模板匹配")]
    [DisplayName("模板匹配工具")]
    [Export("TemplateMatchObj", typeof(IPlugin))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Serializable]
    public sealed class TemplateMatchObj : IPlugin
    {
        private readonly TemplateMatchEngine engine = new TemplateMatchEngine();

        public event EventHandler<PluginExecuteCompletedEventArgs> ExecuteCompleted;
        public ModuleInfo moduleInfo { get; set; } = new ModuleInfo();
        public PluginParamsBase PluginParams { get; set; } = new ParamsTemplateMatch();

        public bool ExecuteObj()
        {
            var p = PluginParams as ParamsTemplateMatch;
            if (p == null) return false;

            HOperatorSet.CountSeconds(out HTuple start);
            try
            {
                TemplateMatchParamsMapper.ReleaseRuntimeOutputs(p);
                if (!p.m_bIsEnable)
                {
                    p.m_bRunSuccess = true;
                    p.m_strRunMessage = "模板匹配未启用，已跳过";
                    return true;
                }

                TemplateMatchRuntimeParams runtime = TemplateMatchParamsMapper.ToRuntime(p);
                TemplateMatchResult result;
                using (result = engine.Run(p.ho_Image, runtime))
                {
                    TemplateMatchParamsMapper.ApplyResult(result, p);
                }
            }
            catch (Exception ex)
            {
                TemplateMatchParamsMapper.ApplyFailure(p, ex.Message);
            }

            HOperatorSet.CountSeconds(out HTuple end);
            p.m_fElapsedMs = (end - start) * 1000;
            ExecuteCompleted?.Invoke(this, new PluginExecuteCompletedEventArgs(p, p.m_bRunSuccess, p.m_strRunMessage, (long)p.m_fElapsedMs));
            return p.m_bRunSuccess;
        }

        bool DefaultFrist = false;

        public void IniObj()
        {
            var p = PluginParams as ParamsTemplateMatch;
            if (p == null) return;
            p.filePath = moduleInfo.filePath;
            p.EnsureDefaultTemplates();
            p.TemplateRootDir = TemplateStorageService.BuildTemplateRoot(Project.Instance.paramPathSet.ProcessPath, moduleInfo);
            engine.Initialize(p, p.TemplateRootDir);
            if(!DefaultFrist)
            {
                HOperatorSet.GenImageConst(out HObject img, "byte", 16384, 4000);
                p.ho_Image = img;
                ExecuteObj();
                img?.Dispose();
                DefaultFrist = true;
            }
        


        }

        public bool saveParams()
        {
            if (PluginParams == null) return false;
            PluginParams.filePath = moduleInfo.filePath;
            var p = PluginParams as ParamsTemplateMatch;
            if (p != null)
                p.TemplateRootDir = TemplateStorageService.BuildTemplateRoot(Project.Instance.paramPathSet.ProcessPath, moduleInfo);
            return PluginParams.Save();
        }

        public void SetParams(PluginParamsBase param)
        {
            if (param != null) PluginParams = param;
        }

        public void GetResultParams(string varName, object obj)
        {
        }

        public void DisPoseObj()
        {
            var p = PluginParams as ParamsTemplateMatch;
            if (p == null) return;
            TemplateMatchParamsMapper.ReleaseRuntimeOutputs(p);
            engine.Dispose();
        }
    }
}

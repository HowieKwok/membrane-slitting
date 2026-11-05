using Frame_Core;
using Frame_Core.pluginParams;
using HalconDotNet;
using LogTool;
using Plugin.BlobAnalysis.Core;
using Plugin.BlobAnalysis.Models;
using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows;

namespace Plugin.BlobAnalysis
{
    [Category("Blob分析")]
    [DisplayName("Blob分析工具")]
    [Export("BlobAnalysisObj", typeof(IPlugin))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Serializable]
    public class BlobAnalysisObj : IPlugin
    {
        private readonly IBlobAnalysisEngine engine = new BlobAnalysisEngine();

        public event EventHandler<PluginExecuteCompletedEventArgs> ExecuteCompleted;

        public ModuleInfo moduleInfo { get; set; } = new ModuleInfo();
        public PluginParamsBase PluginParams { get; set; } = new ParamsBlobAnalysis();

        public bool ExecuteObj()
        {
            ParamsBlobAnalysis p = PluginParams as ParamsBlobAnalysis;
            if (p == null) return false;
            HOperatorSet.CountSeconds(out HTuple start);
            try
            {
               
                // 执行前先释放上一次输出，避免运行失败时残留旧 HObject。
                // 注意：这里不释放 p.ho_Image，输入图像归上游流程或主框架管理。
                BlobParamsMapper.ReleaseOutputHObjects(p);

                if (!p.m_bIsEnable)
                {
                    p.m_bRunSuccess = true;
                    p.m_bIsFind = false;
                    p.m_strRunMessage = "Blob分析未启用，跳过执行";
                    return true;
                }

                BlobAnalysisParams runtime = BlobParamsMapper.ToRuntime(p);
                BlobAnalysisResult result = null;
                try
                {
                    result = engine.Run(p.ho_Image, runtime);
                    BlobParamsMapper.ApplyResult(result, p);
                }
                finally
                {
                    // result 是算法层临时结果；ApplyResult 已经 CopyObj 到 ParamsBlobAnalysis 输出字段。
                    // 无论写回输出是否异常，都必须释放 result 自己持有的 HObject。
                    result?.Dispose();
                }

               
            }
            catch (Exception ex)
            {
                p.m_bRunSuccess = false;
                p.m_bIsFind = false;
                p.m_strRunMessage = ex.Message;
                Log4netRecord.ErrorLog.Error("BlobAnalysis execute failed: " + ex);
                BlobParamsMapper.ReleaseOutputHObjects(p);
               
            }

            HOperatorSet.CountSeconds(out HTuple end);
            end = (end - start)*1000;
            p.m_fElapsedMs=end;
            ExecuteCompleted?.Invoke(
               this,
               new PluginExecuteCompletedEventArgs(
                   p,
                   p.m_bRunSuccess,
                   p.m_strRunMessage,
                   (long)p.m_fElapsedMs));

            return p.m_bRunSuccess;
        }

        public void IniObj()
        {
            ParamsBlobAnalysis p = PluginParams as ParamsBlobAnalysis;
            if (p == null) return;
            p.filePath = moduleInfo.filePath;
        }

        public bool saveParams()
        {
            if (PluginParams == null) return false;
            PluginParams.filePath = moduleInfo.filePath;
            return PluginParams.Save();
        }

        public void SetParams(PluginParamsBase param)
        {
            if (param != null) PluginParams = param;
        }

        public void GetResultParams(string varName, object obj)
        {
            // 结果统一从 ParamsBlobAnalysis 的输出字段读取。
        }

        public void DisPoseObj()
        {
            ParamsBlobAnalysis p = PluginParams as ParamsBlobAnalysis;
            if (p == null) return;

            // 只释放本插件生成的输出 HObject，不释放 ParamsBlobAnalysis.ho_Image。
            BlobParamsMapper.ReleaseOutputHObjects(p);
        }
    }
}
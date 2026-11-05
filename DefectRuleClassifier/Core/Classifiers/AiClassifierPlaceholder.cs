using DefectRuleClassifier.Core.Models;
using HalconDotNet;
using System.Collections.Generic;
using System.IO;

namespace DefectRuleClassifier.Core.Classifiers
{
    /// <summary>
    /// AI 分类占位实现。
    /// 后续接入真实 AI 时，Init 加载模型，Classify 接收原始整图执行推理。
    /// </summary>
    public sealed class AiClassifierPlaceholder : IAiDefectClassifier
    {
        private string modelPath;
        private bool initialized;

        public bool Init(string modelPath, out string message)
        {
            this.modelPath = modelPath;
            initialized = !string.IsNullOrWhiteSpace(modelPath) && File.Exists(modelPath);
            message = initialized
                ? "AI模型已初始化：" + modelPath
                : "AI模型未加载：模型路径为空或文件不存在。";
            return initialized;
        }

        public DefectClassifyDecision Classify(HObject sourceImage, IList<DefectCandidate> candidates)
        {
            return new DefectClassifyDecision
            {
                ClassName = initialized ? "AI未实现" : "未分类",
                Priority = 999,
                Source = initialized ? "AI分类预留" : "AI模型未加载",
                HitRule = initialized
                    ? string.Format("AI整图接口已调用；当前未接入真实推理。候选数量={0}", candidates == null ? 0 : candidates.Count)
                    : "AI模型未加载，ModelPath=" + (modelPath ?? ""),
                SkipClassifier = true
            };
        }
    }
}

using DefectRuleClassifier.Core.Models;
using HalconDotNet;
using System.Collections.Generic;

namespace DefectRuleClassifier.Core.Classifiers
{
    public interface IAiDefectClassifier
    {
        bool Init(string modelPath, out string message);

        /// <summary>
        /// AI 分类预留为整图接口。真实 AI 接入时使用原始图或整图上下文推理，
        /// 不依赖逐个 NG 小图；候选列表只作为辅助信息传入。
        /// </summary>
        DefectClassifyDecision Classify(HObject sourceImage, IList<DefectCandidate> candidates);
    }
}

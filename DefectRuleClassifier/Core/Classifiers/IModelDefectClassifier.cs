using DefectRuleClassifier.Core.Models;

namespace DefectRuleClassifier.Core.Classifiers
{
    /// <summary>
    /// 传统分类器分类接口，预留给 SVM/MLP/其他非 AI 模型。
    /// 模型对象在 InitObj 阶段根据路径初始化，运行时只负责推理。`r`n    /// 输入为与 HALCON 解耦的像素数据 ModelImageInput，分类器内部不依赖 HALCON。
    /// </summary>
    public interface IModelDefectClassifier
    {
        bool Init(string modelPath, out string message);
        DefectClassifyDecision Classify(ModelImageInput input);
    }
}

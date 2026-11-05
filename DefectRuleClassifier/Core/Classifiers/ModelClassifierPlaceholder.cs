using DefectRuleClassifier.Core.Models;
using System.IO;

namespace DefectRuleClassifier.Core.Classifiers
{
    /// <summary>
    /// 传统分类器占位实现。
    /// 后续接入 SVM/MLP 时，Init 加载模型，Classify 使用 candidate.CropImage 执行单缺陷小图推理。
    /// </summary>
    public sealed class ModelClassifierPlaceholder : IModelDefectClassifier
    {
        private string modelPath;
        private bool initialized;

        public bool Init(string modelPath, out string message)
        {
            this.modelPath = modelPath;
            initialized = !string.IsNullOrWhiteSpace(modelPath) && File.Exists(modelPath);
            message = initialized
                ? "分类器模型已初始化：" + modelPath
                : "分类器模型未加载：模型路径为空或文件不存在。";
            return initialized;
        }

        public DefectClassifyDecision Classify(ModelImageInput input)
        {
            return new DefectClassifyDecision
            {
                ClassName = initialized ? "分类器未实现" : "未分类",
                Priority = 999,
                Source = initialized ? "分类器分类预留" : "分类器模型未加载",
                HitRule = initialized
                    ? "传统分类器接口已调用；当前未接入真实 ONNX 推理。推理输入为单缺陷小图像素 ModelImageInput。"
                    : "分类器模型未加载，ModelPath=" + (modelPath ?? ""),
                SkipClassifier = true
            };
        }
    }
}

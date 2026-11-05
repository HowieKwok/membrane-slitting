using DefectRuleClassifier.Core.Serialization;
using System;
using System.IO;

namespace DefectRuleClassifier.Core.Classifiers
{
    /// <summary>
    /// AI 分类的独立参数对象。
    /// 当前只预留模型初始化入口，真实 AI 接入后在 InitObj 阶段按 ModelPath 加载模型。
    /// 本类继承分类规则 DLL 内部的 JsonSerializationOperation，接口形态对齐框架 JSON 保存方式，但不引用 Frame_Core。
    /// </summary>
    [Serializable]
    public sealed class AiClassifierConfig : JsonSerializationOperation
    {
        /// <summary>配置文件路径。允许传入带 .json 的完整路径，也允许传入不带扩展名的框架 filePath。</summary>
        public string ConfigPath { get; set; }

        /// <summary>AI 模型路径，真实 AI 接入时由 InitObj 读取并初始化模型。</summary>
        public string ModelPath { get; set; }

        /// <summary>AI 分类为整图接口，不做单缺陷进入数量限制。</summary>
        public string Remark { get; set; } = "AI分类为整图接口，运行时传入原始图和候选缺陷集合。";

        public static AiClassifierConfig LoadOrDefault(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return new AiClassifierConfig { ConfigPath = filePath };

            AiClassifierConfig reader = new AiClassifierConfig();
            reader.SetConfigPath(filePath);
            string fullPath = reader.fileFullPath;
            if (!File.Exists(fullPath))
                return new AiClassifierConfig { ConfigPath = filePath, filePath = reader.filePath, fileFullPath = fullPath };

            AiClassifierConfig config = reader.Read<AiClassifierConfig>() ?? new AiClassifierConfig();
            config.ConfigPath = filePath;
            config.filePath = reader.filePath;
            config.fileFullPath = fullPath;
            return config;
        }

        public void Save(string filePath)
        {
            SetConfigPath(filePath);
            Save();
        }

        public void SetConfigPath(string filePath)
        {
            ConfigPath = filePath;
            this.filePath = FeatureRuleClassifierConfig.NormalizeFrameworkJsonPath(filePath);
            string directory = Path.GetDirectoryName(this.filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            fileFullPath = this.filePath + ".json";
        }
    }
}

using DefectRuleClassifier.Core.Models;
using DefectRuleClassifier.Core.Serialization;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace DefectRuleClassifier.Core.Classifiers
{
    /// <summary>
    /// 传统分类器分类的独立参数对象。
    /// 检测主参数只保存规则入口路径，InitObj 根据本对象记录的模型路径初始化分类器。
    /// 本类继承分类规则 DLL 内部的 JsonSerializationOperation，接口形态对齐框架 JSON 保存方式，但不引用 Frame_Core。
    ///
    /// 序号绑定约定（与特征值分类参数解耦）：
    /// - 界面（分类器工具）独立读取特征值分类标签，让用户把“模型序号 → 特征值标签”绑定好并保存；
    /// - 运行时只用本配置自带的绑定缓存查表输出，不再依赖特征值分类参数做匹配；
    /// - 序号未绑定（或绑定值为空）时，统一输出 OverflowClassName（默认涂布不良）。
    /// </summary>
    [Serializable]
    public sealed class ModelClassifierConfig : JsonSerializationOperation
    {
        /// <summary>配置文件路径。允许传入带 .json 的完整路径，也允许传入不带扩展名的框架 filePath。</summary>
        public string ConfigPath { get; set; }

        /// <summary>传统分类器模型路径，InitObj 阶段据此初始化模型对象。</summary>
        public string ModelPath { get; set; }

        /// <summary>模型标签文件（labels.txt）路径；为空时自动在模型同目录查找 &lt;模型名&gt;.labels.txt 或 labels.txt。</summary>
        public string LabelsPath { get; set; }

        /// <summary>模型标签序号 → 特征值分类标签 绑定表（序号 0 起，顺序对应 labels.txt）。</summary>
        public ObservableCollection<ModelLabelMapItem> IndexToFeatureLabel { get; set; } = new ObservableCollection<ModelLabelMapItem>();

        /// <summary>候选数量超过硬上限时，整批不再进入传统分类器。</summary>
        public bool EnableCandidateCountLimit { get; set; } = true;

        /// <summary>传统分类器候选数量硬上限。</summary>
        public int MaxCandidateCount { get; set; } = 200;

        /// <summary>未触发硬超限时，是否只取面积最大的前 N 个进入传统分类器。</summary>
        public bool EnableMaxEnterClassifierLimit { get; set; } = true;

        /// <summary>未触发硬超限时，允许进入传统分类器的最大缺陷数量。</summary>
        public int MaxEnterClassifierCount { get; set; } = 20;

        /// <summary>超限、未进入分类器、或模型序号未绑定时输出的默认类别。</summary>
        public string OverflowClassName { get; set; } = "涂布不良";

        /// <summary>模型类别数量（InitObj 时由分类器写入，用于构建序号绑定缓存）。</summary>
        public int ModelClassCount { get; set; }

        [NonSerialized]
        private string[] resolvedLabels = new string[0];
        [NonSerialized]
        private bool[] resolvedMatched = new bool[0];
        [NonSerialized]
        private string fallbackLabel = "涂布不良";

        public static ModelClassifierConfig LoadOrDefault(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return new ModelClassifierConfig { ConfigPath = filePath };

            ModelClassifierConfig reader = new ModelClassifierConfig();
            reader.SetConfigPath(filePath);
            string fullPath = reader.fileFullPath;
            if (!File.Exists(fullPath))
                return new ModelClassifierConfig { ConfigPath = filePath, filePath = reader.filePath, fileFullPath = fullPath };

            ModelClassifierConfig config = reader.Read<ModelClassifierConfig>() ?? new ModelClassifierConfig();
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

        public string GetOverflowClassName()
        {
            return string.IsNullOrWhiteSpace(OverflowClassName) ? "涂布不良" : OverflowClassName.Trim();
        }

        /// <summary>按模型标签序号查找绑定的特征值标签（线性查表，仅界面展示用；推理用 GetResolvedLabel）。</summary>
        public string GetFeatureLabelByIndex(int index)
        {
            if (index < 0 || IndexToFeatureLabel == null)
                return null;
            foreach (ModelLabelMapItem item in IndexToFeatureLabel)
            {
                if (item != null && item.Index == index)
                    return string.IsNullOrWhiteSpace(item.FeatureLabel) ? null : item.FeatureLabel.Trim();
            }
            return null;
        }

        /// <summary>
        /// 预构建“序号 → 输出标签”绑定缓存（一次性构建，推理时 O(1) 查表，不做逐候选遍历）。
        /// 只依赖本配置自带的绑定表，不依赖特征值分类参数（参数解耦）：
        /// 序号绑定了特征值标签 → 输出该标签；未绑定 → 输出 OverflowClassName（默认涂布不良）。
        /// </summary>
        public void RebuildResolvedLabels()
        {
            string fallback = GetOverflowClassName();
            int count = Math.Max(ModelClassCount, 0);
            if (IndexToFeatureLabel != null)
            {
                foreach (ModelLabelMapItem item in IndexToFeatureLabel)
                {
                    if (item != null && item.Index >= count)
                        count = item.Index + 1;
                }
            }
            count = Math.Max(count, 1);

            string[] labels = new string[count];
            bool[] matched = new bool[count];
            for (int i = 0; i < count; i++)
            {
                string mapped = GetFeatureLabelByIndex(i);
                if (!string.IsNullOrWhiteSpace(mapped))
                {
                    labels[i] = mapped;
                    matched[i] = true;
                }
                else
                {
                    labels[i] = fallback;
                    matched[i] = false;
                }
            }
            resolvedLabels = labels;
            resolvedMatched = matched;
            fallbackLabel = fallback;
        }

        /// <summary>O(1) 查表：返回序号对应的输出标签（未绑定/绑定为空时为超限类别）。</summary>
        public string GetResolvedLabel(int index)
        {
            return index >= 0 && index < resolvedLabels.Length ? resolvedLabels[index] : fallbackLabel;
        }

        /// <summary>O(1) 查表：序号是否已绑定特征值标签。</summary>
        public bool IsResolvedMatched(int index)
        {
            return index >= 0 && index < resolvedMatched.Length && resolvedMatched[index];
        }
    }
}
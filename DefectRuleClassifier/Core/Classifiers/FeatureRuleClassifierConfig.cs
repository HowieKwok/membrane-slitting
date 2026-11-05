using DefectRuleClassifier.Core.Models;
using DefectRuleClassifier.Core.Serialization;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace DefectRuleClassifier.Core.Classifiers
{
    /// <summary>
    /// 特征值分类独立参数对象。
    /// 主检测参数只保存规则路径，具体分类规则由规则窗体独立加载、编辑和保存。
    /// 本类继承分类规则 DLL 内部的 JsonSerializationOperation，接口形态对齐框架 JSON 保存方式，但不引用 Frame_Core。
    /// </summary>
    [Serializable]
    public sealed class FeatureRuleClassifierConfig : JsonSerializationOperation
    {
        /// <summary>分类规则配置路径。允许传入带 .json 的完整路径，也允许传入不带扩展名的框架 filePath。</summary>
        public string ConfigPath { get; set; }

        /// <summary>传统分类器独立参数文件路径。</summary>
        public string ModelClassifierConfigPath { get; set; }

        /// <summary>AI 分类独立参数文件路径。</summary>
        public string AiClassifierConfigPath { get; set; }

        /// <summary>候选数量超过该值时，整批不再进入特征值分类。</summary>
        public bool EnableCandidateCountLimit { get; set; } = true;

        /// <summary>候选数量硬上限。</summary>
        public int MaxCandidateCount { get; set; } = 200;

        /// <summary>未超过硬上限时，是否只让面积最大的前 N 个进入特征值分类。</summary>
        public bool EnableMaxEnterClassifyLimit { get; set; } = true;

        /// <summary>允许进入特征值分类的最大数量。</summary>
        public int MaxEnterClassifyCount { get; set; } = 100;

        /// <summary>数量超限时输出的类别名称，默认按现场要求为“涂布不良”。</summary>
        public string OverflowClassName { get; set; } = "涂布不良";

        /// <summary>尺寸判定条件模板。运行时实际使用 NgTypeRule.JudgeCondition。</summary>
        public ObservableCollection<JudgeConditionTemplate> JudgeConditionTemplates { get; set; } = new ObservableCollection<JudgeConditionTemplate>
        {
            new JudgeConditionTemplate { Id = "size_and", Name = "所有条件且", Expression = "长度 && 宽度 && 面积", Remark = "长度、宽度、面积全部满足才通过" },
            new JudgeConditionTemplate { Id = "size_or", Name = "所有条件或", Expression = "长度 || 宽度 || 面积", Remark = "长度、宽度、面积任意一项满足就通过" },
            new JudgeConditionTemplate { Id = "length_width_or_area", Name = "(长&&宽)||面积", Expression = "(长度 && 宽度) || 面积", Remark = "长宽同时满足，或面积满足" },
            new JudgeConditionTemplate { Id = "length_or_width_and_area", Name = "(长||宽)&&面积", Expression = "(长度 || 宽度) && 面积", Remark = "长度或宽度满足，并且面积满足" }
        };

        /// <summary>当前生效的 NG 类型规则列表。列表顺序就是优先级，越靠前优先级越高。</summary>
        public ObservableCollection<NgTypeRule> NgTypes { get; set; } = new ObservableCollection<NgTypeRule>();

        /// <summary>兼容旧参数的暗缺陷规则列表。当前分类流程统一使用 NgTypes。</summary>
        public ObservableCollection<NgTypeRule> DarkNgTypes { get; set; } = new ObservableCollection<NgTypeRule>();

        public static FeatureRuleClassifierConfig LoadOrDefault(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return new FeatureRuleClassifierConfig { ConfigPath = filePath };

            FeatureRuleClassifierConfig reader = new FeatureRuleClassifierConfig();
            reader.SetConfigPath(filePath);
            string fullPath = reader.fileFullPath;
            if (!File.Exists(fullPath))
                return new FeatureRuleClassifierConfig { ConfigPath = filePath, filePath = reader.filePath, fileFullPath = fullPath };

            FeatureRuleClassifierConfig config = reader.Read<FeatureRuleClassifierConfig>() ?? new FeatureRuleClassifierConfig();
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
            this.filePath = NormalizeFrameworkJsonPath(filePath);
            string directory = Path.GetDirectoryName(this.filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            fileFullPath = this.filePath + ".json";
        }

        public static string NormalizeFrameworkJsonPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return path;
            return path.EndsWith(".json", StringComparison.OrdinalIgnoreCase)
                ? path.Substring(0, path.Length - ".json".Length)
                : path;
        }

        public string GetOverflowClassName(string className = null)
        {
            string name = string.IsNullOrWhiteSpace(className) ? OverflowClassName : className;
            if (string.IsNullOrWhiteSpace(name))
                name = "涂布不良";
            return name.Trim();
        }

        /// <summary>
        /// 创建运行时临时超限规则。
        /// 该对象只用于本次输出结果和可选尺寸判定，不加入 NgTypes，不参与优先级匹配，也不会污染分类规则文件。
        /// </summary>
        public NgTypeRule CreateOverflowRuleForRuntime(string className = null)
        {
            return new NgTypeRule
            {
                Name = GetOverflowClassName(className),
                EnableJudgeCondition = false,
                JudgeCondition = JudgeConditionMode.AllConditionAnd,
                MinLength = 0,
                MaxLength = 999999999,
                MinWidth = 0,
                MaxWidth = 999999999,
                MinArea = 0,
                MaxArea = 999999999
            };
        }
    }
}

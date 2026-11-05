using DefectRuleClassifier.Core.Features;
using DefectRuleClassifier.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace DefectRuleClassifier.Core.Classifiers
{
    /// <summary>
    /// 缺陷规则分类器。
    ///
    /// 分类流程对齐源程序思路：
    /// 1. 按 NG 类别列表顺序逐个判断，列表越靠前优先级越高。
    /// 2. 每个类别只计算自己配置的特征范围，不默认计算全部特征。
    /// 3. 特征范围命中后，再根据 EnableJudgeCondition 决定是否判断长宽面积条件。
    /// 4. 启用判定条件且未达标时，输出“类别名未达到设定要求”，方便现场知道是哪个类别的尺寸条件拦截了。
    /// </summary>
    public sealed class SpecRuleClassifier : IDefectClassifier
    {
        public DefectClassifyDecision Classify(DefectClassifyContext context)
        {
            if (context == null || context.Candidate == null)
                return Build("未分类", 999, "无候选", "无候选缺陷");

            if (!context.ClassifierEnabled)
                return Build("未分类", 999, "分类器停用", "ClassifierEnabled=false");

            int priority = 0;
            foreach (NgTypeRule rule in context.NgTypes ?? new NgTypeRule[0])
            {
                priority++;

                // 第一步：只判断当前类别配置的特征范围。
                // GetFeatureValue 会先查缓存，缓存没有才调用 DefectFeatureExtractor 计算。
                if (!MatchFeatures(rule, context))
                    continue;

                // 第二步：特征命中后，按开关决定是否执行长宽面积判定条件。
                // 这里不计算全部特征，只读取检测阶段已经写入的 length、width、area。
                string judgeMessage = "判定条件未启用";
                if (rule.EnableJudgeCondition && !MatchJudgeCondition(rule, context, out judgeMessage))
                    return Build(rule.Name, priority, "未达到设定要求", judgeMessage);

                return Build(rule.Name, priority, "已分类", BuildHitRule(rule, context.Candidate.Features, judgeMessage));
            }

            return Build("未分类", 999, "未分类", "未命中任何 NG 规则");
        }

        /// <summary>
        /// 判断当前类别的尺寸判定条件。
        /// 这里只读取候选缺陷已经缓存的基础特征：length、width、area。
        /// 这三个值在检测阶段已经生成，不会触发全特征提取，也不解析字符串公式。
        /// </summary>
        private static bool MatchJudgeCondition(NgTypeRule rule, DefectClassifyContext context, out string message)
        {
            message = "判定条件未启用";
            if (rule == null || context == null || context.Candidate == null || context.Candidate.Features == null)
                return false;

            DefectFeatureBag features = context.Candidate.Features;
            double length = features.Get("length");
            double width = features.Get("width");
            double area = features.Get("area");

            bool lengthOk = IsBetween(length, rule.MinLength, rule.MaxLength);
            bool widthOk = IsBetween(width, rule.MinWidth, rule.MaxWidth);
            bool areaOk = IsBetween(area, rule.MinArea, rule.MaxArea);

            bool pass;
            switch (rule.JudgeCondition)
            {
                case JudgeConditionMode.AllConditionOr:
                    pass = lengthOk || widthOk || areaOk;
                    break;
                case JudgeConditionMode.LengthAndWidthOrArea:
                    pass = (lengthOk && widthOk) || areaOk;
                    break;
                case JudgeConditionMode.LengthOrWidthAndArea:
                    pass = (lengthOk || widthOk) && areaOk;
                    break;
                default:
                    pass = lengthOk && widthOk && areaOk;
                    break;
            }

            message = string.Format(
                "判定条件={0}，长度{1:F4}[{2},{3}]={4}，宽度{5:F4}[{6},{7}]={8}，面积{9:F4}[{10},{11}]={12}",
                GetJudgeConditionName(rule.JudgeCondition),
                length,
                rule.MinLength,
                rule.MaxLength,
                lengthOk ? "通过" : "未通过",
                width,
                rule.MinWidth,
                rule.MaxWidth,
                widthOk ? "通过" : "未通过",
                area,
                rule.MinArea,
                rule.MaxArea,
                areaOk ? "通过" : "未通过");
            return pass;
        }

        /// <summary>
        /// 给 pipeline 的数量超限短路分支使用，只读取检测阶段已经写入的 length、width、area。
        /// </summary>
        public static bool MatchJudgeConditionForExternal(NgTypeRule rule, DefectCandidate candidate, out string message)
        {
            return MatchJudgeCondition(rule, new DefectClassifyContext { Candidate = candidate }, out message);
        }

        /// <summary>
        /// 判断当前类别配置的所有特征范围。
        /// 注意：这里不计算全部特征，只计算 rule.Features 中列出的项目。
        /// </summary>
        private static bool MatchFeatures(NgTypeRule rule, DefectClassifyContext context)
        {
            if (rule == null || context == null || context.Candidate == null || context.Candidate.Features == null)
                return false;

            foreach (FeatureRule feature in rule.Features)
            {
                double value = GetFeatureValue(context, feature.FeatureType);
                if (double.IsNaN(value) || value < feature.MinValue || value > feature.MaxValue)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 按需获取分类特征。
        /// 已有值直接读缓存；没有缓存才调用 DefectFeatureExtractor.GetFeatureValue 计算，并写回缓存。
        /// </summary>
        public static double GetFeatureValue(DefectClassifyContext context, PicFeatureType type)
        {
            if (context == null || context.Candidate == null || context.Candidate.Features == null)
                return double.NaN;

            DefectFeatureBag features = context.Candidate.Features;
            FeatureTypeOption option = FeatureCatalog.Get(type);
            double value;
            if (features.TryGet(option.FeatureKey, out value))
                return value;

            if (context.FeatureExtractor == null || context.FeatureContext == null)
                return double.NaN;

            value = context.FeatureExtractor.GetFeatureValue(type, context.FeatureContext);
            features.Set(option.FeatureKey, value);
            return value;
        }

        public static double GetFeatureValue(DefectFeatureBag features, PicFeatureType type)
        {
            FeatureTypeOption option = FeatureCatalog.Get(type);
            return features.Get(option.FeatureKey, double.NaN);
        }

        private static DefectClassifyDecision Build(string className, int priority, string source, string hitRule)
        {
            return new DefectClassifyDecision
            {
                ClassName = className,
                Priority = priority,
                Source = source,
                HitRule = hitRule
            };
        }

        private static string BuildHitRule(NgTypeRule rule, DefectFeatureBag features, string judgeMessage)
        {
            IEnumerable<string> featureRules = rule.Features.Select(item =>
            {
                FeatureTypeOption option = FeatureCatalog.Get(item.FeatureType);
                double value = GetFeatureValue(features, item.FeatureType);
                return string.Format("{0}={1:F3}，范围[{2},{3}]", option.DisplayName, value, item.MinValue, item.MaxValue);
            });

            string detail = string.Join("; ", featureRules);
            if (string.IsNullOrWhiteSpace(detail))
                detail = "无附加特征条件";

            return string.Format(
                "命中NG={0}；{1}；尺寸 长[{2},{3}] 宽[{4},{5}] 面积[{6},{7}]；{8}",
                rule.Name,
                rule.EnableJudgeCondition ? judgeMessage : "判定条件未启用",
                rule.MinLength,
                rule.MaxLength,
                rule.MinWidth,
                rule.MaxWidth,
                rule.MinArea,
                rule.MaxArea,
                detail);
        }

        private static string GetJudgeConditionName(JudgeConditionMode mode)
        {
            switch (mode)
            {
                case JudgeConditionMode.AllConditionOr:
                    return "所有条件或";
                case JudgeConditionMode.LengthAndWidthOrArea:
                    return "(长&&宽)||面积";
                case JudgeConditionMode.LengthOrWidthAndArea:
                    return "(长||宽)&&面积";
                default:
                    return "所有条件且";
            }
        }

        private static bool IsBetween(double value, double min, double max)
        {
            return value > min && value < max;
        }
    }
}

using System;
using System.IO;

namespace Plugin.DefectCoat.Services
{
    /// <summary>
    /// 统一生成 Demo 和后续插件集成使用的默认参数路径。
    /// 默认放在软件根目录下的 Config\Plugin.DefectCoat，用户仍可在界面手动改到其他模块路径。
    /// </summary>
    public static class DefaultConfigPathService
    {
        /// <summary>模块默认配置目录名。最终目录为 软件根目录\Config\Plugin.DefectCoat。</summary>
        private const string ModuleDirectoryName = "Plugin.DefectCoat";

        /// <summary>特征值分类规则默认文件名。传统分类器和 AI 配置会基于该路径改扩展名生成。</summary>
        private const string FeatureRuleFileName = "FeatureRuleClassifierConfig.json";

        /// <summary>获取当前宿主软件根目录。插件 DLL 被宿主加载后，该值通常是宿主 exe 所在目录。</summary>
        public static string GetSoftwareRoot()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        /// <summary>获取本插件默认配置目录，不负责创建目录。</summary>
        public static string GetModuleConfigDirectory()
        {
            return Path.Combine(GetSoftwareRoot(), "Config", ModuleDirectoryName);
        }

        /// <summary>获取特征值分类规则默认路径。</summary>
        public static string GetDefaultFeatureRulePath()
        {
            return Path.Combine(GetModuleConfigDirectory(), FeatureRuleFileName);
        }

        /// <summary>根据特征规则路径生成传统分类器配置路径，保证三类配置位于同一组文件。</summary>
        public static string GetDefaultModelClassifierPath(string featureRulePath)
        {
            return Path.ChangeExtension(EnsureFeatureRulePath(featureRulePath), ".ModelClassifier.json");
        }

        /// <summary>根据特征规则路径生成 AI 分类配置路径，保证三类配置位于同一组文件。</summary>
        public static string GetDefaultAiClassifierPath(string featureRulePath)
        {
            return Path.ChangeExtension(EnsureFeatureRulePath(featureRulePath), ".AiClassifier.json");
        }

        /// <summary>当外部未指定规则路径时回退到默认路径；不会检查文件是否存在。</summary>
        public static string EnsureFeatureRulePath(string path)
        {
            return string.IsNullOrWhiteSpace(path) ? GetDefaultFeatureRulePath() : path;
        }
    }
}

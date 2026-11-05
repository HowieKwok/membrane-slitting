using System;
using System.IO;

namespace DefectRuleClassifier.Services
{
    /// <summary>
    /// 统一生成 Demo 和后续插件集成使用的默认参数路径。
    /// 默认放在软件根目录下的 Config\DefectDetectProcessDemo，用户仍可在界面手动改到其他模块路径。
    /// </summary>
    public static class DefaultConfigPathService
    {
        private const string ModuleDirectoryName = "DefectDetectProcessDemo";
        private const string FeatureRuleFileName = "FeatureRuleClassifierConfig.xml";

        public static string GetSoftwareRoot()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        public static string GetModuleConfigDirectory()
        {
            return Path.Combine(GetSoftwareRoot(), "Config", ModuleDirectoryName);
        }

        public static string GetDefaultFeatureRulePath()
        {
            return Path.Combine(GetModuleConfigDirectory(), FeatureRuleFileName);
        }

        public static string GetDefaultModelClassifierPath(string featureRulePath)
        {
            return Path.ChangeExtension(EnsureFeatureRulePath(featureRulePath), ".ModelClassifier.xml");
        }

        public static string GetDefaultAiClassifierPath(string featureRulePath)
        {
            return Path.ChangeExtension(EnsureFeatureRulePath(featureRulePath), ".AiClassifier.xml");
        }

        public static string EnsureFeatureRulePath(string path)
        {
            return string.IsNullOrWhiteSpace(path) ? GetDefaultFeatureRulePath() : path;
        }
    }
}

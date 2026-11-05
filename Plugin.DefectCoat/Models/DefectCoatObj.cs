using DefectRuleClassifier.Core.Classifiers;
using Plugin.DefectCoat.Core.Detection;
using DefectRuleClassifier.Core.Models;
using Plugin.DefectCoat.Core.Pipeline;
using Plugin.DefectCoat.Services;
using Frame_Core;
using HalconDotNet;
using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using Frame_Core.pluginParams;

namespace Plugin.DefectCoat.Models
{
    /// <summary>
    /// 缺陷检测插件对象。
    /// Demo 里由 DefectCoatViewModel 持有；后续集成到宿主时可按 Plugin.MeasureLine 的方式由宿主创建、InitObj、ExecuteObj、SaveParams。
    /// </summary>
    [Category("缺陷检测")]
    [DisplayName("涂布缺陷检测")]
    [Export("DefectCoatObj", typeof(IPlugin))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Serializable]
    public sealed class DefectCoatObj : IPlugin
    {
        /// <summary>特征值分类器。按 FeatureRuleClassifierConfig.NgTypes 做规则匹配。</summary>
        private readonly SpecRuleClassifier featureRuleClassifier = new SpecRuleClassifier();

        /// <summary>传统分类器 ONNX 实现。每个插件对象独立持有会话，多模块共用同一模型互不干扰。</summary>
        private readonly OnnxModelClassifier modelClassifier;

        /// <summary>AI 分类占位对象。后续接入整图 AI 推理时替换该实现。</summary>
        private readonly AiClassifierPlaceholder aiClassifier = new AiClassifierPlaceholder();

        /// <summary>检测和分类调度主流程对象。</summary>
        private readonly DefectDetectPipeline pipeline;

        /// <summary>特征值分类独立参数。主参数只保存 RuleConfigPath。</summary>
        private FeatureRuleClassifierConfig featureRuleConfig = new FeatureRuleClassifierConfig();

        /// <summary>传统分类器独立参数。路径由 FeatureRuleClassifierConfig.ModelClassifierConfigPath 指向。</summary>
        private ModelClassifierConfig modelClassifierConfig = new ModelClassifierConfig();

        /// <summary>AI 分类独立参数。路径由 FeatureRuleClassifierConfig.AiClassifierConfigPath 指向。</summary>
        private AiClassifierConfig aiClassifierConfig = new AiClassifierConfig();

        /// <summary>InitObj 是否已执行成功。</summary>
        private bool initialized;

        public DefectCoatObj()
        {
            modelClassifier = new OnnxModelClassifier(modelClassifierConfig);
            pipeline = new DefectDetectPipeline(
                new HalconThresholdDefectDetector(),
                featureRuleClassifier,
                modelClassifier,
                aiClassifier);
            InitObj();
        }

        /// <summary>插件执行完成事件，通知 WPF 页面或宿主刷新运行结果和状态。</summary>
        public event EventHandler<PluginExecuteCompletedEventArgs> ExecuteCompleted;

        /// <summary>模块信息由框架注入，保存参数和输出结果时需要用到。</summary>
        public ModuleInfo moduleInfo { get; set; } = new ModuleInfo();

        /// <summary>
        /// 框架插件主参数入口。
        /// 所有需要宿主传入、保存、输出给后续模块的变量都必须落在这个 PluginParamsBase 对象里。
        /// 本插件实际类型为 ParamsDefectCoat；分类规则明细不直接塞进这里，只保存 RuleConfigPath。
        /// </summary>
        public PluginParamsBase PluginParams { get; set; } = new ParamsDefectCoat();

        /// <summary>强类型参数入口，供算法和界面内部使用。</summary>
        public ParamsDefectCoat Params => PluginParams as ParamsDefectCoat;

        /// <summary>最近一次完整运行结果。再次运行或释放时会 Dispose。</summary>
        public DefectDetectRunResult LastRunResult { get; private set; }

        /// <summary>当前已加载的特征规则参数。</summary>
        public FeatureRuleClassifierConfig FeatureRuleConfig => featureRuleConfig;

        /// <summary>当前已加载的传统分类器参数。</summary>
        public ModelClassifierConfig ModelClassifierConfig => modelClassifierConfig;

        /// <summary>当前已加载的 AI 分类参数。</summary>
        public AiClassifierConfig AiClassifierConfig => aiClassifierConfig;

        /// <summary>最近一次初始化说明，用于界面状态栏或宿主日志。</summary>
        public string InitMessage { get; private set; } = "";

        /// <summary>
        /// 初始化分类规则、传统分类器和 AI 分类器。
        /// 规则路径为空时会通过 DefaultConfigPathService 生成默认路径；
        /// 后续接插件时，宿主加载参数后应调用该方法刷新内部分类对象。
        /// </summary>
        public bool InitObj()
        {
            try
            {
                EnsureTypedParams();
                EnsureDefaultConfigPaths();
                FeatureRuleClassifierConfig loaded = FeatureRuleClassifierConfig.LoadOrDefault(Params.RuleConfigPath);
                CopyFeatureRuleConfig(loaded, featureRuleConfig);
                featureRuleConfig.ConfigPath = Params.RuleConfigPath;
                if (string.IsNullOrWhiteSpace(featureRuleConfig.ModelClassifierConfigPath))
                    featureRuleConfig.ModelClassifierConfigPath = DefaultConfigPathService.GetDefaultModelClassifierPath(Params.RuleConfigPath);
                if (string.IsNullOrWhiteSpace(featureRuleConfig.AiClassifierConfigPath))
                    featureRuleConfig.AiClassifierConfigPath = DefaultConfigPathService.GetDefaultAiClassifierPath(Params.RuleConfigPath);
                LoadClassifierConfigsFromFeatureRule(featureRuleConfig);

                bool modelOk = modelClassifier.Init(modelClassifierConfig.ModelPath, out string modelMessage);
                modelClassifierConfig.ModelClassCount = modelClassifier.Labels != null ? modelClassifier.Labels.Length : 0;
                modelClassifierConfig.RebuildResolvedLabels();
                bool aiOk = aiClassifier.Init(aiClassifierConfig.ModelPath, out string aiMessage);
                initialized = true;
                InitMessage = string.Format(
                    "特征值分类参数={0}；分类器模型={1}；AI模型={2}；{3}；{4}",
                    string.IsNullOrWhiteSpace(Params.RuleConfigPath) ? "默认内置" : Params.RuleConfigPath,
                    string.IsNullOrWhiteSpace(modelClassifierConfig.ModelPath) ? "未设置" : modelClassifierConfig.ModelPath,
                    string.IsNullOrWhiteSpace(aiClassifierConfig.ModelPath) ? "未设置" : aiClassifierConfig.ModelPath,
                    modelMessage,
                    aiMessage);
                return modelOk || aiOk || featureRuleConfig != null;
            }
            catch (Exception ex)
            {
                initialized = false;
                featureRuleConfig = new FeatureRuleClassifierConfig();
                modelClassifierConfig = new ModelClassifierConfig();
                aiClassifierConfig = new AiClassifierConfig();
                InitMessage = "InitObj初始化失败，已回退默认分类参数：" + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 执行一次完整检测。
        /// 顺序：释放上次输出 -> Pipeline.Run 检测和分类 -> 将输出 HObject/结果列表回写 PluginParams -> 触发 ExecuteCompleted。
        /// </summary>
        public bool ExecuteObj()
        {
            Stopwatch watch = Stopwatch.StartNew();
            try
            {
                if (!initialized)
                    InitObj();

                EnsureTypedParams();
                Params.ReleaseOutputHObjects();
                LastRunResult?.Dispose();
                LastRunResult = null;
                LastRunResult = pipeline.Run(Params, featureRuleConfig, modelClassifierConfig);
                ApplyOutputsToParams(LastRunResult);

                bool success = LastRunResult.DetectResult.Success;
                Params.m_bRunSuccess = success;
                Params.m_strRunMessage = LastRunResult.DetectResult.Message;
                Params.m_fElapsedMs = watch.ElapsedMilliseconds;
                watch.Stop();
                //PublishResultParams();
                ExecuteCompleted?.Invoke(this, new PluginExecuteCompletedEventArgs(Params, success, Params.m_strRunMessage, watch.ElapsedMilliseconds));
                return success;
            }
            catch (Exception ex)
            {
                watch.Stop();
                EnsureTypedParams();
                Params.ReleaseOutputHObjects();
                Params.m_bRunSuccess = false;
                Params.m_strRunMessage = "运行异常：" + ex.Message;
                Params.m_fElapsedMs = watch.ElapsedMilliseconds;
                ExecuteCompleted?.Invoke(this, new PluginExecuteCompletedEventArgs(Params, false, Params.m_strRunMessage, watch.ElapsedMilliseconds));
                return false;
            }
        }

        /// <summary>
        /// 对最近一次检测候选重新执行分类。
        /// 用于“Show分类结果”按钮调试，不重新跑图像检测。
        /// </summary>
        public System.Collections.Generic.IList<NgDefectResult> ClassifyLastResult()
        {
            if (LastRunResult == null || LastRunResult.DetectResult == null)
                return new System.Collections.Generic.List<NgDefectResult>();
            return pipeline.Classify(Params, LastRunResult.DetectResult, featureRuleConfig, modelClassifierConfig, LastRunResult.DetectResult.PreprocessImage);
        }

        /// <summary>
        /// 保存主检测参数。
        /// 注意：这里只是界面按钮和 Demo 调试的便捷入口，实际保存仍然委托给框架基类 PluginParams.Save()，
        /// 不在插件里绕开 Frame_Core 的底层保存逻辑。
        /// </summary>
        public bool SaveParams(string filePath)
        {
            EnsureTypedParams();
            if (string.IsNullOrWhiteSpace(filePath))
                filePath = Path.Combine(DefaultConfigPathService.GetModuleConfigDirectory(), "ParamsDefectCoat.xml");
            moduleInfo.filePath = filePath;
            PluginParams.filePath = filePath;
            return saveParams();
        }

        /// <summary>
        /// 确保分类规则、传统分类器参数、AI 参数都有默认路径。
        /// 默认路径位于软件根目录 Config/Plugin.DefectCoat 下。
        /// </summary>
        public void EnsureDefaultConfigPaths()
        {
            EnsureTypedParams();
            Params.RuleConfigPath = DefaultConfigPathService.EnsureFeatureRulePath(Params.RuleConfigPath);
            featureRuleConfig.ConfigPath = Params.RuleConfigPath;
            if (string.IsNullOrWhiteSpace(featureRuleConfig.ModelClassifierConfigPath))
                featureRuleConfig.ModelClassifierConfigPath = DefaultConfigPathService.GetDefaultModelClassifierPath(Params.RuleConfigPath);
            if (string.IsNullOrWhiteSpace(featureRuleConfig.AiClassifierConfigPath))
                featureRuleConfig.AiClassifierConfigPath = DefaultConfigPathService.GetDefaultAiClassifierPath(Params.RuleConfigPath);
            modelClassifierConfig.ConfigPath = featureRuleConfig.ModelClassifierConfigPath;
            aiClassifierConfig.ConfigPath = featureRuleConfig.AiClassifierConfigPath;
        }

        /// <summary>
        /// 释放运行态输出资源，不释放 PluginParams.ho_Image。
        /// 该方法用于下一次 ExecuteObj 运行前的旧输出清理，或者框架真正释放插件对象时调用。
        /// 单纯关闭插件可视化页面不调用该方法，因为页面只是观察和调参入口，输出可保留给宿主或下次执行前再释放。
        /// ho_Image 通常由插件宿主传入，宿主才拥有输入图像释放权；Demo 自己加载的图像由 ViewModel 在换图/读参时按 OwnsInputImage 释放。
        /// </summary>
        public void ReleaseRuntime()
        {
            LastRunResult?.Dispose();
            LastRunResult = null;
            Params.ReleaseOutputHObjects();
        }

        /// <summary>
        /// 只释放一次运行输出，不释放输入图像。
        /// </summary>
        public void ReleaseOutputs()
        {
            LastRunResult?.Dispose();
            LastRunResult = null;
            Params.ReleaseOutputHObjects();
        }

        /// <summary>
        /// 框架保存入口。
        /// 必须使用 PluginParamsBase.Save()，和 Plugin.MeasureLine 保持一致，避免插件绕开框架底层保存逻辑。
        /// </summary>
        public bool saveParams()
        {
            EnsureTypedParams();
            string path = string.IsNullOrWhiteSpace(moduleInfo.filePath) ? Params.filePath : moduleInfo.filePath;
            if (string.IsNullOrWhiteSpace(path))
                path = Path.Combine(DefaultConfigPathService.GetModuleConfigDirectory(), "ParamsDefectCoat.xml");
            PluginParams.filePath = path;
            return PluginParams.Save();
        }

        /// <summary>
        /// 框架加载模块参数后调用。
        /// 框架传入的是 PluginParamsBase，插件内部必须转换成 ParamsDefectCoat 后才能读取检测参数和输出容器。
        /// 类型不匹配时创建默认参数，避免空引用。
        /// </summary>
        public void SetParams(PluginParamsBase param)
        {
            PluginParams = param as ParamsDefectCoat ?? new ParamsDefectCoat();
            EnsureTypedParams();
        }

        /// <summary>
        /// 框架初始化入口。
        /// 参数加载完成后，先从 ParamsDefectCoat.RuleConfigPath 找到公共分类规则文件，
        /// 再加载传统分类器参数和 AI 参数，最后初始化各分类对象。
        /// </summary>
        public void IniObj()
        {
            EnsureTypedParams();
            InitObj();
        }

        /// <summary>
        /// 框架插件对象释放入口。
        /// 这里释放本插件运行产生的输出对象，不释放宿主传入的 Params.ho_Image。
        /// 注意：插件可视化页面关闭不等于插件对象释放，页面关闭只释放显示控件。
        /// </summary>
        public void DisPoseObj()
        {
            ReleaseRuntime();
            modelClassifier?.Dispose();
        }

        /// <summary>
        /// 将输出变量注册到当前流程，供后续模块按名称读取。
        /// </summary>
        public void GetResultParams(string varName, object obj)
        {
            try
            {
                Frame_Core.Process proc = Project.Instance.GetProcessById(moduleInfo.ProcessID);
                proc?.AddOutPutParam(moduleInfo.ModuleName, varName, obj);
            }
            catch
            {
                // Demo 或单独调试时没有宿主流程，忽略输出注册异常。
            }
        }

        /// <summary>
        /// 从 XML 加载主检测参数。
        /// 使用 PluginParamsBase.Load(string) 走框架基类加载逻辑，不在插件内自定义反序列化器。
        /// </summary>
        public static ParamsDefectCoat LoadParams(string filePath)
        {
            ParamsDefectCoat loaded = new ParamsDefectCoat();
            loaded.Load<ParamsDefectCoat>(filePath);
            return loaded;
        }

        /// <summary>
        /// 拷贝特征规则配置到目标对象。
        /// 不替换 target 引用，避免已绑定到界面的配置对象失效。
        /// </summary>
        private static void CopyFeatureRuleConfig(FeatureRuleClassifierConfig source, FeatureRuleClassifierConfig target)
        {
            if (source == null || target == null) return;
            target.ConfigPath = source.ConfigPath;
            target.ModelClassifierConfigPath = source.ModelClassifierConfigPath;
            target.AiClassifierConfigPath = source.AiClassifierConfigPath;
            target.EnableCandidateCountLimit = source.EnableCandidateCountLimit;
            target.MaxCandidateCount = source.MaxCandidateCount;
            target.EnableMaxEnterClassifyLimit = source.EnableMaxEnterClassifyLimit;
            target.MaxEnterClassifyCount = source.MaxEnterClassifyCount;
            target.OverflowClassName = source.OverflowClassName;

            target.JudgeConditionTemplates.Clear();
            if (source.JudgeConditionTemplates != null)
                foreach (JudgeConditionTemplate item in source.JudgeConditionTemplates) target.JudgeConditionTemplates.Add(item);

            target.NgTypes.Clear();
            if (source.NgTypes != null)
                foreach (NgTypeRule item in source.NgTypes) target.NgTypes.Add(item);

            target.DarkNgTypes.Clear();
            if (source.DarkNgTypes != null)
                foreach (NgTypeRule item in source.DarkNgTypes) target.DarkNgTypes.Add(item);
        }

        /// <summary>
        /// 根据特征规则文件中记录的路径加载分类器和 AI 的独立参数。
        /// 这样主检测参数只需要知道 RuleConfigPath，后续多个模块也能共用同一套分类配置入口。
        /// </summary>
        private void LoadClassifierConfigsFromFeatureRule(FeatureRuleClassifierConfig featureConfig)
        {
            if (featureConfig == null)
                return;

            if (!string.IsNullOrWhiteSpace(featureConfig.ModelClassifierConfigPath) && File.Exists(featureConfig.ModelClassifierConfigPath))
            {
                ModelClassifierConfig loadedModel = ModelClassifierConfig.LoadOrDefault(featureConfig.ModelClassifierConfigPath);
                CopyModelClassifierConfig(loadedModel, modelClassifierConfig);
            }
            else if (!string.IsNullOrWhiteSpace(featureConfig.ModelClassifierConfigPath))
            {
                modelClassifierConfig.ConfigPath = featureConfig.ModelClassifierConfigPath;
            }

            if (!string.IsNullOrWhiteSpace(featureConfig.AiClassifierConfigPath) && File.Exists(featureConfig.AiClassifierConfigPath))
            {
                AiClassifierConfig loadedAi = AiClassifierConfig.LoadOrDefault(featureConfig.AiClassifierConfigPath);
                CopyAiClassifierConfig(loadedAi, aiClassifierConfig);
            }
            else if (!string.IsNullOrWhiteSpace(featureConfig.AiClassifierConfigPath))
            {
                aiClassifierConfig.ConfigPath = featureConfig.AiClassifierConfigPath;
            }
        }

        /// <summary>
        /// 从规则窗口或外部配置同步分类器参数。
        /// 只复制配置字段，不在这里重新执行检测。
        /// </summary>
        public void CopyClassifierConfigs(ModelClassifierConfig modelConfig, AiClassifierConfig aiConfig)
        {
            CopyModelClassifierConfig(modelConfig, modelClassifierConfig);
            CopyAiClassifierConfig(aiConfig, aiClassifierConfig);
        }

        /// <summary>拷贝传统分类器配置到目标对象，保持目标引用稳定。</summary>
        private static void CopyModelClassifierConfig(ModelClassifierConfig source, ModelClassifierConfig target)
        {
            if (source == null || target == null) return;
            target.ConfigPath = source.ConfigPath;
            target.ModelPath = source.ModelPath;
            target.LabelsPath = source.LabelsPath;
            target.EnableCandidateCountLimit = source.EnableCandidateCountLimit;
            target.MaxCandidateCount = source.MaxCandidateCount;
            target.EnableMaxEnterClassifierLimit = source.EnableMaxEnterClassifierLimit;
            target.MaxEnterClassifierCount = source.MaxEnterClassifierCount;
            target.OverflowClassName = source.OverflowClassName;

            target.IndexToFeatureLabel.Clear();
            if (source.IndexToFeatureLabel != null)
                foreach (ModelLabelMapItem item in source.IndexToFeatureLabel)
                    target.IndexToFeatureLabel.Add(new ModelLabelMapItem { Index = item.Index, FeatureLabel = item.FeatureLabel });
        }

        /// <summary>拷贝 AI 分类器配置到目标对象，保持目标引用稳定。</summary>
        private static void CopyAiClassifierConfig(AiClassifierConfig source, AiClassifierConfig target)
        {
            if (source == null || target == null) return;
            target.ConfigPath = source.ConfigPath;
            target.ModelPath = source.ModelPath;
            target.Remark = source.Remark;
        }

        /// <summary>
        /// 将 Pipeline 输出复制回 ParamsDefectCoat。
        /// Params 作为框架输出入口，必须持有自己的 HObject 副本，不能直接引用 Pipeline 内部对象。
        /// </summary>
        private void ApplyOutputsToParams(DefectDetectRunResult runResult)
        {
            if (runResult == null || runResult.DetectResult == null) return;
            DefectDetectResult detect = runResult.DetectResult;
            Params.PreprocessInfo = detect.PreprocessInfo ?? "预处理：--";
            Params.ho_PreprocessImage = CopyHObject(detect.PreprocessImage);
            Params.ho_DarkRegion = CopyHObject(detect.DarkRegion);
            Params.ho_LightRegion = CopyHObject(detect.LightRegion);
            Params.ho_DetectRegion = CopyHObject(detect.RoiRegion);
            Params.ho_MaskRoiRegion = CopyHObject(detect.MaskRoiRegion);
            Params.ho_ResultRegion = CopyHObject(detect.ResultRegion);

            foreach (NgDefectResult item in runResult.Results)
                Params.NgResults.Add(CloneResult(item));

            BuildClassifiedDisplayRegions(Params.NgResults);
        }

        /// <summary>
        /// 按分类状态构造三类显示区域。
        /// 已分类、未分类、未达要求分别输出到 Params 的红/绿/橙叠加区域。
        /// </summary>
        private void BuildClassifiedDisplayRegions(System.Collections.Generic.IEnumerable<NgDefectResult> results)
        {
            HObject classified = null;
            HObject unclassified = null;
            HObject notMeet = null;
            foreach (NgDefectResult item in results)
            {
                if (item == null || item.Region == null || !item.Region.IsInitialized()) continue;
                if (string.Equals(item.Source, "未达到设定要求", StringComparison.OrdinalIgnoreCase))
                    AppendRegion(ref notMeet, item.Region);
                else if (string.Equals(item.ClassName, "未分类", StringComparison.OrdinalIgnoreCase) || string.Equals(item.Source, "未分类", StringComparison.OrdinalIgnoreCase))
                    AppendRegion(ref unclassified, item.Region);
                else
                    AppendRegion(ref classified, item.Region);
            }

            Params.ho_ClassifiedRegion = classified;
            Params.ho_UnclassifiedRegion = unclassified;
            Params.ho_NotMeetRegion = notMeet;
        }

        /// <summary>
        /// 合并一个结果 Region 到目标 Region。
        /// source 只读不释放；target 会被新 Union 区域替换并释放旧对象。
        /// </summary>
        private static void AppendRegion(ref HObject target, HObject source)
        {
            if (source == null || !source.IsInitialized()) return;
            if (target == null || !target.IsInitialized())
            {
                target = source.CopyObj(1, -1);
                return;
            }

            HObject merged = null;
            try
            {
                HOperatorSet.Union2(target, source, out merged);
                target.Dispose();
                target = merged;
                merged = null;
            }
            finally
            {
                merged?.Dispose();
            }
        }

        /// <summary>
        /// 复制一个缺陷结果给 Params.NgResults。
        /// 返回对象拥有独立的 Region、小图和子区域副本，避免 LastRunResult 释放时影响框架输出。
        /// </summary>
        private static NgDefectResult CloneResult(NgDefectResult source)
        {
            return new NgDefectResult
            {
                Index = source.Index,
                ClassName = source.ClassName,
                Source = source.Source,
                HitRule = source.HitRule,
                ClassIndex = source.ClassIndex,
                Confidence = source.Confidence,
                Area = source.Area,
                Length = source.Length,
                Width = source.Width,
                Height = source.Height,
                Row = source.Row,
                Column = source.Column,
                MeanGray = source.MeanGray,
                Polarity = source.Polarity,
                Features = source.Features?.Clone(),
                Region = CopyHObject(source.Region),
                CropImage = CopyHObject(source.CropImage),
                DarkRegion = CopyHObject(source.DarkRegion),
                LightRegion = CopyHObject(source.LightRegion),
                InvalidRegion = CopyHObject(source.InvalidRegion),
                WidthPx = source.WidthPx,
                HeightPx = source.HeightPx,
                PixelToMmH = source.PixelToMmH,
                PixelToMmV = source.PixelToMmV,
                ReferenceGray = source.ReferenceGray
            };
        }

        /// <summary>复制一个可用 HObject；空对象或未初始化对象返回 null，调用方负责释放副本。</summary>
        private static HObject CopyHObject(HObject source)
        {
            if (source == null || !source.IsInitialized()) return null;
            return source.CopyObj(1, -1);
        }

        /// <summary>确保 PluginParams 是本插件需要的 ParamsDefectCoat 类型。</summary>
        private void EnsureTypedParams()
        {
            if (Params == null)
                PluginParams = new ParamsDefectCoat();
        }

        /// <summary>
        /// 将运行结果发布到宿主流程输出参数。
        /// 当前主流程暂未调用，保留给后续需要按变量名串联后续模块时启用。
        /// </summary>
        private void PublishResultParams()
        {
            GetResultParams(nameof(Params.NgResults), Params.NgResults);
            GetResultParams(nameof(Params.ho_DetectRegion), Params.ho_DetectRegion);
            GetResultParams(nameof(Params.ho_ResultRegion), Params.ho_ResultRegion);
            GetResultParams(nameof(Params.ho_ClassifiedRegion), Params.ho_ClassifiedRegion);
            GetResultParams(nameof(Params.ho_UnclassifiedRegion), Params.ho_UnclassifiedRegion);
            GetResultParams(nameof(Params.ho_NotMeetRegion), Params.ho_NotMeetRegion);
            GetResultParams(nameof(Params.ho_MaskRoiRegion), Params.ho_MaskRoiRegion);
            GetResultParams(nameof(Params.ho_PreprocessImage), Params.ho_PreprocessImage);
            GetResultParams(nameof(Params.ho_DarkRegion), Params.ho_DarkRegion);
            GetResultParams(nameof(Params.ho_LightRegion), Params.ho_LightRegion);
            GetResultParams("NgCount", Params.NgResults == null ? 0 : Params.NgResults.Count);
            GetResultParams("ElapsedMs", Params.m_fElapsedMs);
        }
    }
}

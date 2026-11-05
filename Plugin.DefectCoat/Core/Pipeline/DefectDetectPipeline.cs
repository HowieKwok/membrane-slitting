using DefectRuleClassifier.Core.Classifiers;
using DefectRuleClassifier.Core.Features;
using DefectRuleClassifier.Core.Models;
using Frame_Core;
using Frame_Core.pluginParams;
using HalconDotNet;
using LogTool;
using Plugin.DefectCoat.Core.Detection;
using Plugin.DefectCoat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;

namespace Plugin.DefectCoat.Core.Pipeline
{
    /// <summary>
    /// 缺陷检测主流程。
    /// 流程顺序固定为：图像处理检测 -> 特征值分类 -> 传统分类器分类 -> AI整图分类 -> 输出最终结果。
    /// 分类对象均在 DefectCoatObj.InitObj 中按路径初始化，Pipeline 只负责调度和结果组装。
    /// </summary>
    public sealed class DefectDetectPipeline
    {
        /// <summary>检测器接口实现，当前默认是 HALCON 阈值检测器；Pipeline 不关心内部阈值和 ROI 细节。</summary>
        private readonly IDefectDetector detector;
        /// <summary>特征值规则分类器，按 FeatureRuleConfig.NgTypes 顺序对候选缺陷做第一层分类。</summary>
        private readonly IDefectClassifier featureRuleClassifier;
        /// <summary>传统模型分类器接口，使用检测阶段裁剪出的候选小图，不重新检测。</summary>
        private readonly IModelDefectClassifier modelClassifier;
        /// <summary>AI 分类器接口，当前主要是预留入口，真实模型接入时仍由该接口承载。</summary>
        private readonly IAiDefectClassifier aiClassifier;
        /// <summary>特征提取器实例。特征值规则分类只按规则需要提取特征，避免检测阶段一次性计算全部特征。</summary>
        private readonly DefectFeatureExtractor featureExtractor = new DefectFeatureExtractor();

        /// <summary>
        /// 构造检测调度流程。
        /// 四个依赖对象由 DefectCoatObj.InitObj 创建并注入，便于后续替换检测器或分类器实现。
        /// </summary>
        public DefectDetectPipeline(IDefectDetector detector, IDefectClassifier featureRuleClassifier, IModelDefectClassifier modelClassifier, IAiDefectClassifier aiClassifier)
        {
            this.detector = detector;
            this.featureRuleClassifier = featureRuleClassifier;
            this.modelClassifier = modelClassifier;
            this.aiClassifier = aiClassifier;
        }

        /// <summary>
        /// 执行完整检测流程。
        /// 从 ParamsDefectCoat 拷贝一次运行请求，先检测，再按固定顺序分类；返回对象拥有本次检测和分类输出的释放责任。
        /// </summary>
        public DefectDetectRunResult Run(ParamsDefectCoat parameters, FeatureRuleClassifierConfig featureConfig, ModelClassifierConfig modelConfig)
        {
            featureConfig = featureConfig ?? new FeatureRuleClassifierConfig();
            modelConfig = modelConfig ?? new ModelClassifierConfig();

            DefectDetectRequest detectRequest = new DefectDetectRequest
            {
                Image = parameters.ho_Image,
                PartitionCount = parameters.PartitionCount,
                PartitionDirection = parameters.PartitionDirection,
                PartitionLengthUnit = parameters.PartitionLengthUnit,
                ReferenceGray = parameters.ReferenceGray,
                UpGrayOffset = parameters.UpGrayOffset,
                DownGrayOffset = parameters.DownGrayOffset,
                OpenRadius = parameters.OpenRadius,
                CloseRadius = parameters.CloseRadius,
                MinCandidateArea = parameters.MinCandidateArea,
                EnableBackgroundGrayScale = parameters.EnableBackgroundGrayScale,
                EnableSmallRegionMerge = parameters.EnableSmallRegionMerge,
                SmallRegionMergeMaxArea = parameters.SmallRegionMergeMaxArea,
                SmallRegionMergeWidth = parameters.SmallRegionMergeWidth,
                SmallRegionMergeHeight = parameters.SmallRegionMergeHeight,
                MaxGrayScaleAbs = parameters.MaxGrayScaleAbs,
                PixelToMmH = parameters.PixelToMmH,
                PixelToMmV = parameters.PixelToMmV,
                EnableCandidateCountLimit = parameters.EnableFeatureRuleClassify && featureConfig.EnableCandidateCountLimit,
                MaxCandidateCount = featureConfig.MaxCandidateCount,
                OverflowClassName = featureConfig.GetOverflowClassName(),
                DetectMode = parameters.DetectMode,
                EnableLocalRoi = parameters.EnableLocalRoi,
                EnableMaskRoi = parameters.EnableMaskRoi,
                EnablePositionFollow = parameters.EnablePositionFollow,
                LocalRoi = parameters.LocalRoi,
                MaskRois = parameters.MaskRois.ToList(),
                Partitions = parameters.Partitions.ToList(),
                BaseRow = parameters.BaseRow,
                BaseCol = parameters.BaseCol,
                BaseAngle = parameters.BaseAngle,
                CurrentRow = parameters.CurrentRow,
                CurrentCol = parameters.CurrentCol,
                CurrentAngle = parameters.CurrentAngle
            };

            DefectDetectResult detectResult = null;
            DefectDetectRunResult output = null;
            try
            {

               
                detectResult = detector.Detect(detectRequest);

                output = new DefectDetectRunResult
                {
                    DetectResult = detectResult,
                    Results = new List<NgDefectResult>()
                };
                if (!detectResult.Success) return output;

                // 特征值分类的“候选总数硬上限”已经前移到检测阶段。
                // 检测阶段一旦超限，说明本次不允许构造单个候选，因此这里直接输出合并结果并结束流程。
                if (detectResult.IsCandidateCountOverflow)
                {
                    NgTypeRule overflowRule = featureConfig.CreateOverflowRuleForRuntime(detectResult.OverflowClassName);
                    output.Results.Add(BuildUnionOverflowResult(
                        parameters.ho_Image,
                        detectResult,
                        overflowRule,
                        "检测候选数量超限",
                        detectResult.OverflowCandidateCount,
                        detectResult.OverflowCandidateLimit));
                    return output;
                }

                foreach (NgDefectResult result in Classify(parameters, detectResult, featureConfig, modelConfig, detectResult.PreprocessImage))
                    output.Results.Add(result);

                return output;
            }
            catch
            {
                // 检测阶段已经生成的 HObject 必须在分类异常时释放，否则 ExecuteObj 捕获异常后拿不到本次局部结果。
                if (output != null)
                    output.Dispose();
                else
                    detectResult?.Dispose();
                throw;
            }
        }

        /// <summary>
        /// 对检测候选执行分类。
        /// 输入 detectResult 来自检测器，输出 NgDefectResult 会复制候选 Region/小图，调用方负责释放输出结果。
        /// </summary>
        public IList<NgDefectResult> Classify(
            ParamsDefectCoat parameters,
            DefectDetectResult detectResult,
            FeatureRuleClassifierConfig featureConfig = null,
            ModelClassifierConfig modelConfig = null,
            HObject featureImage = null)
        {
            featureConfig = featureConfig ?? new FeatureRuleClassifierConfig();
            modelConfig = modelConfig ?? new ModelClassifierConfig();

            List<NgDefectResult> output = new List<NgDefectResult>();
            List<DefectCandidate> unresolved = (detectResult.Candidates ?? new List<DefectCandidate>()).ToList();
            if (unresolved.Count == 0)
                return output;

            // 1. 特征值分类。
            // 特征值分类是第一层分类，输入是检测算法输出的候选缺陷。
            // 它不使用模型，也不遍历全部特征；只按规则文件里每个类别配置的特征范围按需取值。
            bool stopClassify = false;
            if (parameters.EnableFeatureRuleClassify)
                unresolved = RunFeatureRuleClassify(parameters, detectResult, featureConfig, unresolved, output, featureImage, out stopClassify);

            if (stopClassify)
                return output.OrderBy(item => item.Index).ToList();

            // 2. 传统分类器分类
            // 这一层是给传统分类器预留的独立接口。
            // 进入这里的候选，应该已经在检测阶段裁好了 NG 小图 candidate.CropImage；
            // 传统分类器只看小图和自己的模型参数，不再回头重新做检测。
            if (parameters.EnableModelClassify && unresolved.Count > 0)
            {
                if (IsHardOverflow(modelConfig.EnableCandidateCountLimit, modelConfig.MaxCandidateCount, unresolved.Count))
                {
                    NgTypeRule modelHardOverflowRule = BuildExternalOverflowRule(featureConfig, modelConfig.GetOverflowClassName());
                    output.Add(BuildUnionOverflowResult(parameters.ho_Image, detectResult, modelHardOverflowRule, "分类器分类数量超限", unresolved.Count, modelConfig.MaxCandidateCount));
                    return output;
                }
                // 与特征值分类一样，传统分类器也可以配置“最多允许多少个进入”。
                SplitByAreaLimit(unresolved, modelConfig.EnableMaxEnterClassifierLimit, modelConfig.MaxEnterClassifierCount, out List<DefectCandidate> enterModel, out List<DefectCandidate> skipModel);
                NgTypeRule modelEnterOverflowRule = BuildExternalOverflowRule(featureConfig, modelConfig.GetOverflowClassName());
                foreach (DefectCandidate candidate in skipModel)
                    output.Add(BuildOverflowCandidateResult(candidate, modelEnterOverflowRule, "分类器分类进入数量限制", unresolved.Count, modelConfig.MaxEnterClassifierCount));

                // 并行推理：ONNX 会话线程安全，各候选小图相互独立，可多线程同时分类。
                DefectClassifyDecision[] modelDecisions = new DefectClassifyDecision[enterModel.Count];
                Parallel.For(
                    0,
                    enterModel.Count,
                    new ParallelOptions { MaxDegreeOfParallelism = Math.Max(1, Environment.ProcessorCount - 1) },
                    i => { modelDecisions[i] = modelClassifier.Classify(BuildModelImageInput(enterModel[i])); });

                // 序号绑定缓存：一次预构建（O(类别数)），推理时逐候选 O(1) 查表，不做遍历匹配。
                modelConfig.RebuildResolvedLabels();

                unresolved = new List<DefectCandidate>();
                for (int i = 0; i < enterModel.Count; i++)
                {
                    DefectCandidate candidate = enterModel[i];
                    DefectClassifyDecision decision = modelDecisions[i];
                    if (IsFinalClassified(decision))
                    {
                        bool matched = modelConfig.IsResolvedMatched(decision.ClassIndex);
                        string mappedLabel = modelConfig.GetResolvedLabel(decision.ClassIndex);
                        string mapNote = matched
                            ? string.Format("模型序号 {{0}} 匹配特征值标签 {{1}}", decision.ClassIndex, mappedLabel)
                            : string.Format("模型序号 {{0}} 未绑定或标签不存在，统一归入 {{1}}", decision.ClassIndex, mappedLabel);
                        output.Add(BuildResult(
                            candidate,
                            mappedLabel,
                            matched ? "分类器分类" : "分类器标签未匹配",
                            decision.HitRule + "；" + mapNote,
                            decision.ClassIndex,
                            decision.Confidence));
                    }
                    else
                    {
                        unresolved.Add(candidate);
                    }
                }
            }

            // 3. AI 分类
            // AI 入口只保留接口，当前 Demo 仍是占位实现。
            // 这里传入的是原始整图，后续真接入 AI 时直接替换实现即可。
            if (parameters.EnableAiClassify && unresolved.Count > 0)
            {
                DefectClassifyDecision decision = aiClassifier.Classify(parameters.ho_Image, detectResult.Candidates);
                if (IsFinalClassified(decision))
                {
                    foreach (DefectCandidate candidate in unresolved)
                        output.Add(BuildResult(candidate, decision.ClassName, decision.Source, decision.HitRule));
                    unresolved.Clear();
                }
            }

            foreach (DefectCandidate candidate in unresolved)
            {
                output.Add(BuildResult(
                    candidate,
                    "未分类",
                    "未分类",
                    "已启用的分类方式均未命中，或对应分类对象仍为占位实现。"));
            }

            return output.OrderBy(item => item.Index).ToList();
        }

        /// <summary>
        /// 调用特征值规则分类器判断单个候选。
        /// featureImage 只读传入，不在这里释放；候选对象也不在这里复制。
        /// </summary>
        private DefectClassifyDecision ClassifyByFeatureRule(FeatureRuleClassifierConfig featureConfig, DefectCandidate candidate, HObject featureImage)
        {
            return featureRuleClassifier.Classify(new DefectClassifyContext
            {
                Candidate = candidate,
                NgTypes = featureConfig.NgTypes,
                ClassifierEnabled = true,
                FeatureExtractor = featureExtractor,
                FeatureContext = BuildFeatureContext(candidate, featureImage)
            });
        }

        /// <summary>
        /// 执行特征值分类，并返回还没有被分类的候选缺陷。
        /// 这个方法只负责“特征规则分类”这一层，不处理传统分类器和 AI。
        /// </summary>
        private List<DefectCandidate> RunFeatureRuleClassify(
            ParamsDefectCoat parameters,
            DefectDetectResult detectResult,
            FeatureRuleClassifierConfig featureConfig,
            List<DefectCandidate> candidatesToClassify,
            List<NgDefectResult> output,
            HObject featureImage,
            out bool stopClassify)
        {
            stopClassify = false;

            // 第一步：进入分类数量限制。
            // 候选总数硬上限已经在检测阶段处理。
            // 这里拿到的 candidatesToClassify 都已经完成 Region、小图和基础特征构造，
            // 因此这里只负责“最多允许多少个进入特征规则分类”，不再做整批超限合并。
            // 被跳过的候选不再参与本层分类，直接按 OverflowClassName 输出。
            SplitByAreaLimit(
                candidatesToClassify,
                featureConfig.EnableMaxEnterClassifyLimit,
                featureConfig.MaxEnterClassifyCount,
                out List<DefectCandidate> enterFeatureClassify,
                out List<DefectCandidate> skipFeatureClassify);

            NgTypeRule enterLimitRule = featureConfig.CreateOverflowRuleForRuntime();
            foreach (DefectCandidate candidate in skipFeatureClassify)
            {
                output.Add(BuildOverflowCandidateResult(
                    candidate,
                    enterLimitRule,
                    "特征值分类进入数量限制",
                    candidatesToClassify.Count,
                    featureConfig.MaxEnterClassifyCount));
            }

            //// 第二步：逐个候选按规则优先级匹配。
            //// 真正进入这里的候选，会交给 SpecRuleClassifier：
            ////   1. 按 NgTypes 列表顺序判断，列表越靠前优先级越高；
            ////   2. 每个类别只读取该类别配置的 FeatureRule；
            ////   3. FeatureRule 命中后，才按 EnableJudgeCondition 校验长宽面积；
            ////   4. 未命中的候选返回 unresolved，继续给后面的分类器分类或 AI 分类。
            //List<DefectCandidate> unresolved = new List<DefectCandidate>();
            //foreach (DefectCandidate candidate in enterFeatureClassify)
            //{
            //    DefectClassifyDecision decision = ClassifyByFeatureRule(featureConfig, candidate, featureImage);
            //    if (IsFinalClassified(decision))
            //        output.Add(BuildResult(candidate, decision.ClassName, decision.Source, decision.HitRule));
            //    else
            //        unresolved.Add(candidate);
            //}

            // 第二步：逐个候选按规则优先级匹配。
            // 并行只负责“算分类结果”，不直接写 output / unresolved，避免集合抢占。
            DefectClassifyDecision[] decisions = new DefectClassifyDecision[enterFeatureClassify.Count];
            HOperatorSet.CountSeconds(out HTuple start);
            Parallel.For(
                0,
                enterFeatureClassify.Count,
                new ParallelOptions
                {
                    MaxDegreeOfParallelism = Math.Max(1, Environment.ProcessorCount - 1)
                },
                i =>
                {
                    DefectCandidate candidate = enterFeatureClassify[i];

                    // 注意：
                    // 1. featureConfig 只能读，不能在这里 Ensure/Add/Save。
                    // 2. candidate 只能由当前线程处理一次。
                    // 3. featureImage 只读，不在这里 Dispose。
                    decisions[i] = ClassifyByFeatureRule(featureConfig, candidate, featureImage);
                });
            HOperatorSet.CountSeconds(out HTuple end);
            end = (end - start) * 1000;
            Console.WriteLine("并行速度："+end.D.ToString());

            // 单线程按原顺序回收结果，保证输出顺序稳定。
            HOperatorSet.CountSeconds(out  start);
            List<DefectCandidate> unresolved = new List<DefectCandidate>();
            for (int i = 0; i < enterFeatureClassify.Count; i++)
            {
                DefectCandidate candidate = enterFeatureClassify[i];
                DefectClassifyDecision decision = decisions[i];

                if (IsFinalClassified(decision))
                    output.Add(BuildResult(candidate, decision.ClassName, decision.Source, decision.HitRule));
                else
                    unresolved.Add(candidate);
            }
            HOperatorSet.CountSeconds(out  end);
            end = (end - start) * 1000;
            Console.WriteLine("回收速度：" + end.D.ToString());
            return unresolved;
        }

        /// <summary>判断某一层分类器是否触发候选总数硬上限。</summary>
        private static bool IsHardOverflow(bool enabled, int limit, int count)
        {
            return enabled && limit > 0 && count > limit;
        }

        /// <summary>
        /// 把候选缺陷的 HALCON 小图（CropImage）转成与分类器解耦的 RGB 像素输入。
        /// 转换在插件侧完成；分类器本身不依赖 HALCON。
        /// </summary>
        private static ModelImageInput BuildModelImageInput(DefectCandidate candidate)
        {
            if (candidate == null || candidate.CropImage == null || !candidate.CropImage.IsInitialized())
                return null;

            HImage image = null;
            bool ownsImage = false;
            try
            {
                // 所有权约定：CropImage 属于候选缺陷，分类转换只是读取像素。
                // 若 CropImage 本身就是 HImage，直接借用、绝不释放；
                // 若是普通 HObject 包装成 HImage，只释放包装句柄，不释放原始对象。
                image = candidate.CropImage as HImage;
                if (image == null)
                {
                    image = new HImage(candidate.CropImage);
                    ownsImage = true;
                }
                if (image.CountChannels().I == 1)
                {
                    HImage three = image.Compose3(image, image);
                    if (ownsImage)
                    {
                        image.Dispose();
                    }
                    image = three;
                    ownsImage = true; // Compose3 生成的新对象必须释放
                }
                if (!string.Equals(image.GetImageType().S, "byte", StringComparison.OrdinalIgnoreCase))
                {
                    HImage byteImage = image.ConvertImageType("byte");
                    if (ownsImage)
                    {
                        image.Dispose();
                    }
                    image = byteImage;
                    ownsImage = true; // ConvertImageType 生成的新对象必须释放
                }

                int width;
                int height;
                image.GetImageSize(out width, out height);
                byte[] rgb = ExtractRgbPlanes(image, width, height);
                return ModelImageInput.FromRgb(rgb, width, height, "候选#" + candidate.Index);
            }
            finally
            {
                if (ownsImage)
                {
                    image?.Dispose();
                }
            }
        }

        /// <summary>从 HALCON 三通道 byte 图取 R/G/B 三个平面，交织成 RGB。</summary>
        private static unsafe byte[] ExtractRgbPlanes(HImage image, int width, int height)
        {
            IntPtr red;
            IntPtr green;
            IntPtr blue;
            string type;
            image.GetImagePointer3(out red, out green, out blue, out type, out width, out height);

            var rgb = new byte[width * height * 3];
            var redPointer = (byte*)red;
            var greenPointer = (byte*)green;
            var bluePointer = (byte*)blue;
            var index = 0;
            for (var y = 0; y < height; y++)
            {
                var row = y * width;
                for (var x = 0; x < width; x++)
                {
                    var pixel = row + x;
                    rgb[index++] = redPointer[pixel];
                    rgb[index++] = greenPointer[pixel];
                    rgb[index++] = bluePointer[pixel];
                }
            }
            return rgb;
        }

        /// <summary>
        /// 按面积从大到小选择允许进入当前分类层的候选。
        /// enter 继续分类，skipped 直接按超限类别输出，避免超量候选继续耗时。
        /// </summary>
        private static void SplitByAreaLimit(
            IList<DefectCandidate> source,
            bool enabled,
            int maxEnterCount,
            out List<DefectCandidate> enter,
            out List<DefectCandidate> skipped)
        {
            if (!enabled || maxEnterCount <= 0 || source.Count <= maxEnterCount)
            {
                enter = source.ToList();
                skipped = new List<DefectCandidate>();
                return;
            }

            HashSet<DefectCandidate> selected = new HashSet<DefectCandidate>(
                source.OrderByDescending(item => item.Features.Get("area")).Take(maxEnterCount));
            enter = source.Where(selected.Contains).ToList();
            skipped = source.Where(item => !selected.Contains(item)).ToList();
        }

        /// <summary>判断分类决策是否已经得到有效类别，未分类或显式跳过都继续交给后续分类层。</summary>
        private static bool IsFinalClassified(DefectClassifyDecision decision)
        {
            if (decision == null || decision.SkipClassifier)
                return false;
            return !string.Equals(decision.Source, "未分类", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(decision.ClassName, "未分类", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 为传统分类器等外部分类层构造超限输出规则。
        /// 临时借用 FeatureRuleConfig 的运行时工厂，创建后立即恢复原 OverflowClassName。
        /// </summary>
        private static NgTypeRule BuildExternalOverflowRule(FeatureRuleClassifierConfig featureConfig, string className)
        {
            if (featureConfig == null)
                return new NgTypeRule { Name = string.IsNullOrWhiteSpace(className) ? "涂布不良" : className, EnableJudgeCondition = false };

            string oldName = featureConfig.OverflowClassName;
            featureConfig.OverflowClassName = string.IsNullOrWhiteSpace(className) ? "涂布不良" : className;
            NgTypeRule rule = featureConfig.CreateOverflowRuleForRuntime();
            featureConfig.OverflowClassName = oldName;
            return rule;
        }

        /// <summary>
        /// 为一个被分类进入数量限制跳过的候选构造输出结果。
        /// 仍会执行外部判定条件，保证超限类别也受长宽面积规则约束。
        /// </summary>
        private static NgDefectResult BuildOverflowCandidateResult(DefectCandidate candidate, NgTypeRule rule, string source, int count, int limit)
        {
            string judgeMessage = "判定条件未启用";
            if (rule.EnableJudgeCondition && !SpecRuleClassifier.MatchJudgeConditionForExternal(rule, candidate, out judgeMessage))
            {
                return BuildResult(
                    candidate,
                    rule.Name,
                    "未达到设定要求",
                    string.Format("{0}：候选数量 {1}，限制 {2}；输出 {3}，但尺寸判定未通过：{4}", source, count, limit, rule.Name, judgeMessage));
            }

            return BuildResult(
                candidate,
                rule.Name,
                source,
                string.Format("{0}：候选数量 {1}，限制 {2}；未进入当前分类对象，直接按 {3} 输出。{4}", source, count, limit, rule.Name, judgeMessage));
        }

        /// <summary>
        /// 候选总数硬上限触发时构造合并区域结果。
        /// 使用检测阶段已经准备好的 OverflowRegion/ResultRegion，不再逐个候选裁图。
        /// </summary>
        private static NgDefectResult BuildUnionOverflowResult(HObject sourceImage, DefectDetectResult detectResult, NgTypeRule rule, string source, int count, int limit)
        {
            HObject unionRegion = CopyHObject(detectResult == null ? null : detectResult.OverflowRegion)
                ?? CopyHObject(detectResult == null ? null : detectResult.ResultRegion);
            if (unionRegion == null)
                HOperatorSet.GenEmptyRegion(out unionRegion);

            try
            {
                NgDefectResult result = BuildAggregateResult(sourceImage, detectResult, unionRegion);
                unionRegion = null;
                string judgeMessage = "判定条件未启用";
                if (rule.EnableJudgeCondition && !MatchJudgeCondition(rule, result, out judgeMessage))
                {
                    result.ClassName = rule.Name;
                    result.Source = "未达到设定要求";
                    result.HitRule = string.Format("{0}：候选数量 {1} > 限制 {2}，合并输出 {3}，但尺寸判定未通过：{4}", source, count, limit, rule.Name, judgeMessage);
                    return result;
                }

                result.ClassName = rule.Name;
                result.Source = source;
                result.HitRule = string.Format("{0}：候选数量 {1} > 限制 {2}，直接使用检测阶段合并区域并按 {3} 输出。{4}", source, count, limit, rule.Name, judgeMessage);
                return result;
            }
            finally
            {
                unionRegion?.Dispose();
            }
        }

        /// <summary>
        /// 用合并结果临时包装成 DefectCandidate，复用规则分类器的长宽面积判定逻辑。
        /// </summary>
        private static bool MatchJudgeCondition(NgTypeRule rule, NgDefectResult result, out string message)
        {
            DefectCandidate candidate = new DefectCandidate
            {
                Features = result.Features
            };
            return SpecRuleClassifier.MatchJudgeConditionForExternal(rule, candidate, out message);
        }

        /// <summary>
        /// 从合并 Region 构造一个聚合缺陷结果。
        /// unionRegion 的所有权转交给返回的 NgDefectResult，正常返回后本方法不再释放它。
        /// </summary>
        private static NgDefectResult BuildAggregateResult(HObject sourceImage, DefectDetectResult detectResult, HObject unionRegion)
        {
            double pixelToMmH = detectResult == null || detectResult.PixelToMmH <= 0 ? 1 : detectResult.PixelToMmH;
            double pixelToMmV = detectResult == null || detectResult.PixelToMmV <= 0 ? 1 : detectResult.PixelToMmV;

            double areaPx = 0;
            double row = 0;
            double col = 0;
            double widthPx = 0;
            double heightPx = 0;
            HObject crop = null;

            try
            {
                HOperatorSet.AreaCenter(unionRegion, out HTuple areaTuple, out HTuple rowTuple, out HTuple colTuple);
                areaPx = areaTuple.TupleLength() == 0 ? 0 : areaTuple.TupleSum().D;
                row = rowTuple.TupleLength() == 0 ? 0 : rowTuple.TupleMean().D;
                col = colTuple.TupleLength() == 0 ? 0 : colTuple.TupleMean().D;
                HOperatorSet.SmallestRectangle1(unionRegion, out HTuple row1, out HTuple col1, out HTuple row2, out HTuple col2);
                widthPx = Math.Max(0, col2.D - col1.D + 1);
                heightPx = Math.Max(0, row2.D - row1.D + 1);
                crop = CropImageSafe(sourceImage, row1.D, col1.D, row2.D, col2.D);
            }
            catch
            {
            }

            double area = areaPx * pixelToMmH * pixelToMmV;
            double width = widthPx * pixelToMmH;
            double height = heightPx * pixelToMmV;
            double length = Math.Max(width, height);

            DefectFeatureBag features = new DefectFeatureBag();
            features.Set("area", area);
            features.Set("area_px", areaPx);
            features.Set("length", length);
            features.Set("width", width);
            features.Set("height", height);
            features.Set("width_px", widthPx);
            features.Set("height_px", heightPx);
            features.Set("row", row);
            features.Set("column", col);

            return new NgDefectResult
            {
                Index = 0,
                Area = area,
                Length = length,
                Width = width,
                Height = height,
                Row = row,
                Column = col,
                Polarity = "多区域合并",
                Features = features,
                Region = unionRegion,
                CropImage = crop,
                WidthPx = widthPx,
                HeightPx = heightPx,
                PixelToMmH = pixelToMmH,
                PixelToMmV = pixelToMmV,
                ReferenceGray = detectResult == null ? 0 : detectResult.ReferenceGray
            };
        }

        /// <summary>
        /// 安全裁剪合并区域外接矩形。
        /// 会把坐标夹紧到图像范围内，裁剪失败时返回 null 而不是中断整个分类流程。
        /// </summary>
        private static HObject CropImageSafe(HObject sourceImage, double row1, double col1, double row2, double col2)
        {
            if (sourceImage == null || !sourceImage.IsInitialized())
                return null;
            try
            {
                HOperatorSet.GetImageSize(sourceImage, out HTuple width, out HTuple height);
                double r1 = Math.Max(0, Math.Min(height.D - 1, Math.Floor(row1)));
                double c1 = Math.Max(0, Math.Min(width.D - 1, Math.Floor(col1)));
                double r2 = Math.Max(0, Math.Min(height.D - 1, Math.Ceiling(row2)));
                double c2 = Math.Max(0, Math.Min(width.D - 1, Math.Ceiling(col2)));
                if (r2 < r1 || c2 < c1) return null;
                HOperatorSet.CropRectangle1(sourceImage, out HObject crop, r1, c1, r2, c2);
                return crop;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 把单个检测候选复制成最终 NG 结果。
        /// 候选内部的 Region/小图会 CopyObj，返回结果独立拥有这些副本的释放权。
        /// </summary>
        private static NgDefectResult BuildResult(DefectCandidate candidate, string className, string source, string hitRule, int classIndex = -1, double confidence = 0)
        {
            return new NgDefectResult
            {
                Index = candidate.Index,
                ClassName = className,
                Source = source,
                HitRule = hitRule,
                ClassIndex = classIndex,
                Confidence = confidence,
                Area = candidate.Features.Get("area"),
                Length = candidate.Features.Get("length"),
                Width = candidate.Features.Get("width"),
                Height = candidate.Features.Get("height"),
                Row = candidate.Features.Get("row"),
                Column = candidate.Features.Get("column"),
                MeanGray = candidate.Features.Get("mean_gray"),
                Polarity = candidate.Polarity,
                Features = candidate.Features.Clone(),
                Region = CopyHObject(candidate.Region),
                CropImage = CopyHObject(candidate.CropImage),
                DarkRegion = CopyHObject(candidate.DarkRegion),
                LightRegion = CopyHObject(candidate.LightRegion),
                InvalidRegion = CopyHObject(candidate.InvalidRegion),
                WidthPx = candidate.WidthPx,
                HeightPx = candidate.HeightPx,
                PixelToMmH = candidate.PixelToMmH,
                PixelToMmV = candidate.PixelToMmV,
                ReferenceGray = candidate.ReferenceGray
            };
        }

        /// <summary>
        /// 为特征提取器构造上下文。
        /// 所有 HObject 都是只读引用，特征提取过程不应释放它们。
        /// </summary>
        private static DefectFeatureContext BuildFeatureContext(DefectCandidate candidate, HObject featureImage)
        {
            return new DefectFeatureContext
            {
                Image = featureImage,
                DefectRegion = candidate.Region,
                DarkRegion = candidate.DarkRegion,
                LightRegion = candidate.LightRegion,
                InvalidRegion = candidate.InvalidRegion,
                WidthPx = candidate.WidthPx,
                HeightPx = candidate.HeightPx,
                PixelToMmH = candidate.PixelToMmH,
                PixelToMmV = candidate.PixelToMmV,
                ReferenceGray = candidate.ReferenceGray
            };
        }

        /// <summary>复制一个 HALCON 对象；空对象或未初始化对象返回 null，调用方负责释放副本。</summary>
        private static HObject CopyHObject(HObject source)
        {
            if (source == null || !source.IsInitialized()) return null;
            return source.CopyObj(1, -1);
        }
    }

    /// <summary>
    /// 一次完整检测运行的输出容器。
    /// 同时持有检测阶段结果和最终分类结果，Dispose 时统一释放本次运行生成的 HObject。
    /// </summary>
    public sealed class DefectDetectRunResult
    {
        /// <summary>检测阶段原始输出，包含预处理图、ROI、候选区域和调试日志。</summary>
        public DefectDetectResult DetectResult { get; set; }
        /// <summary>最终对外展示和回写 Params.NgResults 的分类结果集合。</summary>
        public IList<NgDefectResult> Results { get; set; }

        /// <summary>释放本次运行持有的检测结果和分类结果，防止 HALCON HObject 泄漏。</summary>
        public void Dispose()
        {
            DetectResult?.Dispose();
            DetectResult = null;
            if (Results != null)
            {
                foreach (NgDefectResult item in Results)
                    item?.Dispose();
                Results.Clear();
            }
        }
    }
}

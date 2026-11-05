using Frame_Core.pluginParams;
using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Plugin.ReusableMeasureDetect.PointFilters.Core
{
    /// <summary>
    /// 点集滤波工具接口。
    /// 这个接口对应 PPT 里的 Strategy/Registry 思路：插件只编排 Pipeline，具体滤波器只负责处理点集。
    /// </summary>
    public interface IPointFilterTool
    {
        /// <summary>
        /// 滤波工具唯一标识，用于参数保存、界面模板选择和 Pipeline 编排。
        /// </summary>
        string Id { get; }

        /// <summary>
        /// 界面显示名称。
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// 该滤波器需要显示的参数描述，界面按 BindingPath 动态绑定到 Options。
        /// </summary>
        IReadOnlyList<PointFilterParameterDescriptor> Parameters { get; }

        /// <summary>
        /// 判断当前滤波器是否在参数中启用。
        /// </summary>
        bool IsEnabled(PointFilterOptionsParam options);

        /// <summary>
        /// 对输入点集执行滤波，返回新的点集；实现内部不得修改 input。
        /// </summary>
        List<MeasurePointParam> Apply(IReadOnlyList<MeasurePointParam> input, PointFilterOptionsParam options);
    }

    /// <summary>
    /// 单个滤波参数字段描述。
    /// Label 面向界面显示，BindingPath 面向 ViewModel 反射绑定。
    /// </summary>
    public sealed class PointFilterParameterDescriptor
    {
        public string Label { get; private set; }
        public string BindingPath { get; private set; }

        /// <summary>
        /// 创建参数字段描述。
        /// </summary>
        public PointFilterParameterDescriptor(string label, string bindingPath)
        {
            Label = label ?? string.Empty;
            BindingPath = bindingPath ?? string.Empty;
        }
    }

    /// <summary>
    /// 滤波工具描述对象，供界面构建可添加滤波列表和参数模板。
    /// </summary>
    public sealed class PointFilterToolDescriptor
    {
        public string Id { get; private set; }
        public string DisplayName { get; private set; }
        public IReadOnlyList<PointFilterParameterDescriptor> Parameters { get; private set; }

        /// <summary>
        /// 创建工具描述，不包含具体算法实例，便于后续抽到公共库时保留轻量元数据。
        /// </summary>
        public PointFilterToolDescriptor(string id, string displayName, IReadOnlyList<PointFilterParameterDescriptor> parameters)
        {
            Id = id;
            DisplayName = displayName;
            Parameters = parameters ?? new List<PointFilterParameterDescriptor>();
        }
    }

    /// <summary>
    /// 点集滤波 Pipeline。
    /// 规则：
    /// 1. RawPoints 永远保留原始输入；
    /// 2. 每一步滤波单独输出 Step，界面和检测只能选择其中一步，不把多步点集混在一起显示；
    /// 3. Enabled=false 时检测直接使用原始点；
    /// 4. Enabled=true 但没有启用具体滤波时，输出“未滤波”步骤，避免界面没有可选点集；
    /// 5. SelectedStepIndex=-1 表示自动使用最后一步，其他值表示用指定步骤参与检测和显示。
    /// </summary>
    public static class PointFilterPipelineCore
    {
        private static readonly List<IPointFilterTool> Catalog = new List<IPointFilterTool>
        {
            new MeanPointFilterTool(),
            new GaussianPointFilterTool(),
            new MedianPointFilterTool(),
            new BilateralPointFilterTool(),
            new GuidedPointFilterTool(),
            new CurvatureFlowPointFilterTool(),
            new SobelPointFilterTool(),
            new LaplacianPointFilterTool(),
            new GaborPointFilterTool()
        };

        /// <summary>
        /// 返回当前注册的滤波工具元数据，界面通过它生成列表和参数输入项。
        /// </summary>
        public static List<PointFilterToolDescriptor> GetToolDescriptors()
        {
            return Catalog.Select(t => new PointFilterToolDescriptor(t.Id, t.DisplayName, t.Parameters)).ToList();
        }

        /// <summary>
        /// 获取当前输入点；当没有足够真实点时生成演示直角点，供界面仿真和 ROI 重置使用。
        /// </summary>
        public static List<MeasurePointParam> BuildInputOrDemoPoints(ParamsReusableMeasureDetect p)
        {
            if (p.InputPoints == null) p.InputPoints = new List<MeasurePointParam>();
            if (p.InputPoints.Count >= 6) return Clone(p.InputPoints);

            List<MeasurePointParam> demo = new List<MeasurePointParam>();
            for (int i = 0; i < 24; i++) demo.Add(new MeasurePointParam(300, 120 + i * 16));
            demo.Add(new MeasurePointParam(300, 500));
            for (int i = 1; i < 24; i++) demo.Add(new MeasurePointParam(300 + i * 16, 500));
            return demo;
        }

        /// <summary>
        /// 运行滤波 Pipeline 并写回插件参数。
        /// RawPoints 保留原始输入，RuntimeDetectionPoints 只保存界面选中的步骤点集。
        /// </summary>
        public static void ApplyAndWriteOutput(ParamsReusableMeasureDetect p, IReadOnlyList<MeasurePointParam> input)
        {
            p.EnsureOutputs();
            if (p.FilterOptions == null) p.FilterOptions = new PointFilterOptionsParam();
            PointFilterOutputParam output = p.FilterOutput;
            output.Clear();

            // 这里不再按 ROI 二次筛点；ROI 只属于 MeasureLine 抓边阶段，否则会把 demo/副边点错误筛掉。
            List<MeasurePointParam> raw = Clone(input);
            List<PointFilterStepResult> steps = ApplyWithSteps(raw, p.FilterOptions);
            int selectedIndex = ResolveSelectedStepIndex(steps.Count, p.FilterOptions.SelectedStepIndex);
            PointFilterStepResult selected = steps[selectedIndex];

            output.RawPoints = raw;
            output.Steps = steps.Select(s => new PointFilterStepOutputParam
            {
                Name = s.Name,
                ToolId = s.ToolId,
                Points = Clone(s.Points)
            }).ToList();
            output.SelectedStepIndex = selectedIndex;
            output.SelectedStepName = selected.Name;
            output.SelectedToolId = selected.ToolId;
            output.SelectedPoints = Clone(selected.Points);
            output.FilterEnabled = p.FilterOptions.Enable;
            output.AppliedFilterCount = steps.Count(s => !string.IsNullOrEmpty(s.ToolId));
            output.PointCount = output.SelectedPoints.Count;

            if (output.ho_SelectedPointsCross != null) output.ho_SelectedPointsCross.Dispose();
            output.ho_SelectedPointsCross = BuildPointCross(output.SelectedPoints, p.FilterOptions.DisplayCrossSize);
            p.RuntimeDetectionPoints = Clone(output.SelectedPoints);
        }

        /// <summary>
        /// 执行 Pipeline 并返回每一步的点集，供界面选择“原始点/某一步滤波点/最后一步”。
        /// </summary>
        public static List<PointFilterStepResult> ApplyWithSteps(IReadOnlyList<MeasurePointParam> input, PointFilterOptionsParam options)
        {
            List<PointFilterStepResult> steps = new List<PointFilterStepResult>();
            List<MeasurePointParam> current = Clone(input);
            steps.Add(new PointFilterStepResult("原始点 Raw", "Raw", current));

            if (options == null || !options.Enable)
                return steps;

            string sortMode = NormalizeSortMode(options.PreSortMode);
            if (!string.IsNullOrEmpty(sortMode))
            {
                current = SortPoints(current, sortMode);
                steps.Add(new PointFilterStepResult(sortMode == "Row" ? "行突变排序 Row Sort" : "列突变排序 Col Sort", "Sort" + sortMode, current));
            }

            List<string> order = BuildOrder(options.FilterOrderCsv);
            foreach (string id in order)
            {
                IPointFilterTool tool = Catalog.FirstOrDefault(t => string.Equals(t.Id, id, StringComparison.OrdinalIgnoreCase));
                if (tool == null || !tool.IsEnabled(options)) continue;
                current = tool.Apply(current, options);
                steps.Add(new PointFilterStepResult(tool.DisplayName, tool.Id, current));
            }

            if (steps.Count == 1)
                steps.Add(new PointFilterStepResult("未滤波 No Filter", "NoFilter", current));

            return steps;
        }

        /// <summary>
        /// 仅使用滤波参数和输出对象运行 Pipeline，供滤波控件独立刷新时调用。
        /// </summary>
        public static void ApplyAndWriteOutput(PointFilterOptionsParam options, PointFilterOutputParam output, IReadOnlyList<MeasurePointParam> input)
        {
            if (options == null) options = new PointFilterOptionsParam();
            if (output == null) return;
            output.Clear();

            List<MeasurePointParam> raw = Clone(input);
            List<PointFilterStepResult> steps = ApplyWithSteps(raw, options);
            int selectedIndex = ResolveSelectedStepIndex(steps.Count, options.SelectedStepIndex);
            PointFilterStepResult selected = steps[selectedIndex];

            output.RawPoints = raw;
            output.Steps = steps.Select(step => new PointFilterStepOutputParam
            {
                Name = step.Name,
                ToolId = step.ToolId,
                Points = Clone(step.Points)
            }).ToList();
            output.FilterEnabled = options.Enable;
            output.AppliedFilterCount = steps.Count(step => !string.IsNullOrEmpty(step.ToolId));
            WriteSelectedStep(options, output, selectedIndex, selected.Name, selected.ToolId, selected.Points);
        }

        /// <summary>
        /// 在已有 Steps 中切换输出步骤，不重新计算滤波，避免界面切换时产生额外耗时。
        /// </summary>
        public static void SelectStepToOutput(PointFilterOptionsParam options, PointFilterOutputParam output, int selectedStepIndex)
        {
            if (options == null || output == null || output.Steps == null || output.Steps.Count == 0) return;
            int selectedIndex = ResolveSelectedStepIndex(output.Steps.Count, selectedStepIndex);
            PointFilterStepOutputParam step = output.Steps[selectedIndex];
            WriteSelectedStep(options, output, selectedIndex, step.Name, step.ToolId, step.Points);
        }

        /// <summary>
        /// 把指定步骤写为当前选中输出，并重新生成当前点集十字 XLD。
        /// </summary>
        private static void WriteSelectedStep(PointFilterOptionsParam options, PointFilterOutputParam output, int selectedIndex, string name, string toolId, IReadOnlyList<MeasurePointParam> points)
        {
            output.SelectedStepIndex = selectedIndex;
            output.SelectedStepName = name;
            output.SelectedToolId = toolId;
            output.SelectedPoints = Clone(points);
            output.PointCount = output.SelectedPoints.Count;
            if (output.ho_SelectedPointsCross != null) output.ho_SelectedPointsCross.Dispose();
            output.ho_SelectedPointsCross = BuildPointCross(output.SelectedPoints, options.DisplayCrossSize);
        }
        /// <summary>
        /// 根据保存的 CSV 顺序生成 Pipeline 顺序；缺失工具自动追加到默认顺序末尾。
        /// </summary>
        private static List<string> BuildOrder(string csv)
        {
            List<string> defaultOrder = Catalog.Select(t => t.Id).ToList();
            if (string.IsNullOrWhiteSpace(csv)) return defaultOrder;

            List<string> order = new List<string>();
            foreach (string item in csv.Split(new[] { ',', ';', '，', '；' }, StringSplitOptions.RemoveEmptyEntries))
            {
                string id = item.Trim();
                if (Catalog.Any(t => string.Equals(t.Id, id, StringComparison.OrdinalIgnoreCase)) &&
                    !order.Any(x => string.Equals(x, id, StringComparison.OrdinalIgnoreCase)))
                    order.Add(id);
            }

            foreach (string id in defaultOrder)
                if (!order.Any(x => string.Equals(x, id, StringComparison.OrdinalIgnoreCase)))
                    order.Add(id);
            return order;
        }

        /// <summary>
        /// 解析步骤索引，-1 表示自动选择最后一步，越界时也回退到最后一步。
        /// </summary>
        private static int ResolveSelectedStepIndex(int stepCount, int configuredIndex)
        {
            if (stepCount <= 0) return 0;
            if (configuredIndex < 0) return stepCount - 1;
            if (configuredIndex >= stepCount) return stepCount - 1;
            return configuredIndex;
        }

        /// <summary>
        /// 规范化预排序模式，只允许 Row、Col 或空。
        /// </summary>
        private static string NormalizeSortMode(string mode)
        {
            if (string.Equals(mode, "Row", StringComparison.OrdinalIgnoreCase)) return "Row";
            if (string.Equals(mode, "Col", StringComparison.OrdinalIgnoreCase)) return "Col";
            return string.Empty;
        }

        /// <summary>
        /// 按突变方向预排序：Row 模式按 Col 递增，Col 模式按 Row 递增。
        /// </summary>
        private static List<MeasurePointParam> SortPoints(IReadOnlyList<MeasurePointParam> points, string mode)
        {
            if (mode == "Row")
                return points.OrderBy(p => p.Col).ThenBy(p => p.Row).Select(ClonePoint).ToList();
            if (mode == "Col")
                return points.OrderBy(p => p.Row).ThenBy(p => p.Col).Select(ClonePoint).ToList();
            return Clone(points);
        }

        /// <summary>
        /// 均值滤波：对滑动窗口内 Row/Col 分别求平均。
        /// </summary>
        private static List<MeasurePointParam> Mean(IReadOnlyList<MeasurePointParam> points, int radius)
        {
            return Window(points, radius, delegate(int center, List<MeasurePointParam> values)
            {
                return new MeasurePointParam(values.Average(p => p.Row), values.Average(p => p.Col));
            });
        }

        /// <summary>
        /// 高斯滤波：按点序距离生成高斯权重，对 Row/Col 加权平均。
        /// </summary>
        private static List<MeasurePointParam> Gaussian(IReadOnlyList<MeasurePointParam> points, int radius, double sigma)
        {
            sigma = Math.Max(0.1, sigma);
            return Window(points, radius, delegate(int center, List<MeasurePointParam> values)
            {
                double sumW = 0;
                double sumRow = 0;
                double sumCol = 0;
                for (int i = 0; i < values.Count; i++)
                {
                    double d = Math.Abs(i - center);
                    double w = Math.Exp(-(d * d) / (2 * sigma * sigma));
                    sumW += w;
                    sumRow += values[i].Row * w;
                    sumCol += values[i].Col * w;
                }
                return sumW <= 1e-9 ? ClonePoint(values[center]) : new MeasurePointParam(sumRow / sumW, sumCol / sumW);
            });
        }

        /// <summary>
        /// 中值滤波：窗口内分别取 Row 和 Col 中值，抑制尖峰离群。
        /// </summary>
        private static List<MeasurePointParam> Median(IReadOnlyList<MeasurePointParam> points, int radius)
        {
            return Window(points, radius, delegate(int center, List<MeasurePointParam> values)
            {
                return new MeasurePointParam(
                    MedianValue(values.Select(p => p.Row).ToList()),
                    MedianValue(values.Select(p => p.Col).ToList()));
            });
        }

        /// <summary>
        /// 双边滤波：同时考虑点序空间距离和几何距离，尽量平滑噪声同时保留突变。
        /// </summary>
        private static List<MeasurePointParam> Bilateral(IReadOnlyList<MeasurePointParam> points, int radius, double spatialSigma, double rangeSigma)
        {
            spatialSigma = Math.Max(0.1, spatialSigma);
            rangeSigma = Math.Max(0.1, rangeSigma);
            radius = Math.Max(1, radius);
            List<MeasurePointParam> result = new List<MeasurePointParam>(points.Count);
            for (int i = 0; i < points.Count; i++)
            {
                int left = Math.Max(0, i - radius);
                int right = Math.Min(points.Count - 1, i + radius);
                double sumW = 0;
                double sumRow = 0;
                double sumCol = 0;
                for (int j = left; j <= right; j++)
                {
                    double ds = i - j;
                    double dr = Distance(points[i], points[j]);
                    double ws = Math.Exp(-(ds * ds) / (2 * spatialSigma * spatialSigma));
                    double wr = Math.Exp(-(dr * dr) / (2 * rangeSigma * rangeSigma));
                    double w = ws * wr;
                    sumW += w;
                    sumRow += points[j].Row * w;
                    sumCol += points[j].Col * w;
                }
                result.Add(sumW <= 1e-9 ? ClonePoint(points[i]) : new MeasurePointParam(sumRow / sumW, sumCol / sumW));
            }
            return result;
        }

        /// <summary>
        /// 引导滤波简化实现：以均值结果为 guide，根据 eps 和原点偏差做线性融合。
        /// </summary>
        private static List<MeasurePointParam> Guided(IReadOnlyList<MeasurePointParam> points, int radius, double eps)
        {
            List<MeasurePointParam> mean = Mean(points, radius);
            List<MeasurePointParam> result = new List<MeasurePointParam>(points.Count);
            for (int i = 0; i < points.Count; i++)
            {
                double guideWeight = 1.0 / (1.0 + Math.Max(0.000001, eps) + Distance(points[i], mean[i]));
                result.Add(Lerp(mean[i], points[i], guideWeight));
            }
            return result;
        }

        /// <summary>
        /// 曲率流滤波：按二阶差分迭代平滑点链，适合去除轻微毛刺。
        /// </summary>
        private static List<MeasurePointParam> CurvatureFlow(IReadOnlyList<MeasurePointParam> points, int iterations, double step)
        {
            List<MeasurePointParam> current = Clone(points);
            iterations = Math.Max(1, iterations);
            step = Math.Max(0.001, step);
            for (int it = 0; it < iterations; it++)
            {
                List<MeasurePointParam> next = Clone(current);
                for (int i = 1; i < current.Count - 1; i++)
                {
                    MeasurePointParam prev = current[i - 1];
                    MeasurePointParam cur = current[i];
                    MeasurePointParam nxt = current[i + 1];
                    next[i] = new MeasurePointParam(
                        cur.Row + step * (prev.Row - 2 * cur.Row + nxt.Row),
                        cur.Col + step * (prev.Col - 2 * cur.Col + nxt.Col));
                }
                current = next;
            }
            return current;
        }

        /// <summary>
        /// 边缘增强：Sobel 模式放大一阶差分，Laplacian 模式放大相对均值的高频分量。
        /// </summary>
        private static List<MeasurePointParam> EdgeEnhance(IReadOnlyList<MeasurePointParam> points, int radius, double amount, bool sobel)
        {
            List<MeasurePointParam> baseSmooth = Mean(points, radius);
            List<MeasurePointParam> result = new List<MeasurePointParam>(points.Count);
            for (int i = 0; i < points.Count; i++)
            {
                double row;
                double col;
                if (sobel)
                {
                    int a = Math.Max(0, i - 1);
                    int b = Math.Min(points.Count - 1, i + 1);
                    row = points[i].Row + (points[b].Row - points[a].Row) * amount * 0.15;
                    col = points[i].Col + (points[b].Col - points[a].Col) * amount * 0.15;
                }
                else
                {
                    row = points[i].Row + (points[i].Row - baseSmooth[i].Row) * amount;
                    col = points[i].Col + (points[i].Col - baseSmooth[i].Col) * amount;
                }
                result.Add(new MeasurePointParam(row, col));
            }
            return result;
        }

        /// <summary>
        /// Gabor-like 滤波：用高斯包络乘余弦权重，增强周期性或方向性变化。
        /// </summary>
        private static List<MeasurePointParam> GaborLike(IReadOnlyList<MeasurePointParam> points, int radius, double sigma)
        {
            sigma = Math.Max(0.1, sigma);
            radius = Math.Max(1, radius);
            List<MeasurePointParam> result = new List<MeasurePointParam>(points.Count);
            double frequency = Math.PI / Math.Max(2, radius * 2);
            for (int i = 0; i < points.Count; i++)
            {
                int left = Math.Max(0, i - radius);
                int right = Math.Min(points.Count - 1, i + radius);
                double sumW = 0;
                double sumRow = 0;
                double sumCol = 0;
                for (int j = left; j <= right; j++)
                {
                    double x = j - i;
                    double w = Math.Exp(-(x * x) / (2 * sigma * sigma)) * Math.Cos(frequency * x);
                    sumW += Math.Abs(w);
                    sumRow += points[j].Row * w;
                    sumCol += points[j].Col * w;
                }
                result.Add(sumW <= 1e-9 ? ClonePoint(points[i]) : new MeasurePointParam(sumRow / sumW, sumCol / sumW));
            }
            return result;
        }

        /// <summary>
        /// 通用滑动窗口执行器，为均值、高斯、中值等窗口滤波复用边界处理。
        /// </summary>
        private static List<MeasurePointParam> Window(IReadOnlyList<MeasurePointParam> points, int radius, Func<int, List<MeasurePointParam>, MeasurePointParam> map)
        {
            radius = Math.Max(1, radius);
            List<MeasurePointParam> result = new List<MeasurePointParam>(points.Count);
            for (int i = 0; i < points.Count; i++)
            {
                int left = Math.Max(0, i - radius);
                int right = Math.Min(points.Count - 1, i + radius);
                List<MeasurePointParam> values = new List<MeasurePointParam>();
                for (int j = left; j <= right; j++) values.Add(points[j]);
                result.Add(map(i - left, values));
            }
            return result;
        }

        /// <summary>
        /// 将点集转换为 HALCON 十字 XLD，调用方负责在输出对象 Clear/Dispose 时释放。
        /// </summary>
        public static HObject BuildPointCross(IReadOnlyList<MeasurePointParam> points, double size)
        {
            HObject cross;
            if (points == null || points.Count == 0)
            {
                HOperatorSet.GenEmptyObj(out cross);
                return cross;
            }

            double[] rows = points.Select(p => p.Row).ToArray();
            double[] cols = points.Select(p => p.Col).ToArray();
            HOperatorSet.GenCrossContourXld(out cross, new HTuple(rows), new HTuple(cols), Math.Max(2.0, size), 0.785398);
            return cross;
        }

        /// <summary>
        /// 在两个点之间做线性插值。
        /// </summary>
        private static MeasurePointParam Lerp(MeasurePointParam a, MeasurePointParam b, double t)
        {
            t = Math.Max(0, Math.Min(1, t));
            return new MeasurePointParam(a.Row + (b.Row - a.Row) * t, a.Col + (b.Col - a.Col) * t);
        }

        /// <summary>
        /// 计算窗口值中值，调用前要求集合非空。
        /// </summary>
        private static double MedianValue(List<double> values)
        {
            values.Sort();
            int mid = values.Count / 2;
            return values.Count % 2 == 1 ? values[mid] : (values[mid - 1] + values[mid]) * 0.5;
        }

        /// <summary>
        /// 计算两个点之间的欧氏距离。
        /// </summary>
        private static double Distance(MeasurePointParam a, MeasurePointParam b)
        {
            double dr = a.Row - b.Row;
            double dc = a.Col - b.Col;
            return Math.Sqrt(dr * dr + dc * dc);
        }

        /// <summary>
        /// 深拷贝点集，保证每一步滤波输出互相独立。
        /// </summary>
        private static List<MeasurePointParam> Clone(IReadOnlyList<MeasurePointParam> points)
        {
            if (points == null) return new List<MeasurePointParam>();
            return points.Select(ClonePoint).ToList();
        }

        /// <summary>
        /// 深拷贝单点；空点回退为默认点。
        /// </summary>
        private static MeasurePointParam ClonePoint(MeasurePointParam point)
        {
            return point == null ? new MeasurePointParam() : new MeasurePointParam(point.Row, point.Col);
        }

        /// <summary>
        /// Pipeline 内部步骤结果。构造时复制点集，防止后续步骤修改历史步骤。
        /// </summary>
        public sealed class PointFilterStepResult
        {
            public readonly string Name;
            public readonly string ToolId;
            public readonly List<MeasurePointParam> Points;
            /// <summary>
            /// 创建一步滤波结果。
            /// </summary>
            public PointFilterStepResult(string name, string toolId, List<MeasurePointParam> points)
            {
                Name = name;
                ToolId = toolId;
                Points = Clone(points);
            }
        }

        /// <summary>
        /// 均值滤波工具适配器，把 Options 中的 Mean 开关和 MeanRadius 参数接入 Pipeline。
        /// </summary>
        private sealed class MeanPointFilterTool : IPointFilterTool
        {
            public string Id { get { return "Mean"; } }
            public string DisplayName { get { return "均值滤波 Mean"; } }
            public IReadOnlyList<PointFilterParameterDescriptor> Parameters { get { return Fields(Field("Mean 半径", "MeanRadius")); } }
            public bool IsEnabled(PointFilterOptionsParam options) { return options.Mean; }
            public List<MeasurePointParam> Apply(IReadOnlyList<MeasurePointParam> input, PointFilterOptionsParam options) { return Mean(input, options.MeanRadius); }
        }

        /// <summary>
        /// 高斯滤波工具适配器，把 Gaussian 开关、半径和 Sigma 参数接入 Pipeline。
        /// </summary>
        private sealed class GaussianPointFilterTool : IPointFilterTool
        {
            public string Id { get { return "Gaussian"; } }
            public string DisplayName { get { return "高斯滤波 Gaussian"; } }
            public IReadOnlyList<PointFilterParameterDescriptor> Parameters { get { return Fields(Field("Gaussian 半径", "GaussianRadius"), Field("Gaussian Sigma", "GaussianSigma")); } }
            public bool IsEnabled(PointFilterOptionsParam options) { return options.Gaussian; }
            public List<MeasurePointParam> Apply(IReadOnlyList<MeasurePointParam> input, PointFilterOptionsParam options) { return Gaussian(input, options.GaussianRadius, options.GaussianSigma); }
        }

        /// <summary>
        /// 中值滤波工具适配器，把 Median 开关和 MedianRadius 参数接入 Pipeline。
        /// </summary>
        private sealed class MedianPointFilterTool : IPointFilterTool
        {
            public string Id { get { return "Median"; } }
            public string DisplayName { get { return "中值滤波 Median"; } }
            public IReadOnlyList<PointFilterParameterDescriptor> Parameters { get { return Fields(Field("Median 半径", "MedianRadius")); } }
            public bool IsEnabled(PointFilterOptionsParam options) { return options.Median; }
            public List<MeasurePointParam> Apply(IReadOnlyList<MeasurePointParam> input, PointFilterOptionsParam options) { return Median(input, options.MedianRadius); }
        }

        /// <summary>
        /// 双边滤波工具适配器，把空间 Sigma、值域 Sigma 和半径参数接入 Pipeline。
        /// </summary>
        private sealed class BilateralPointFilterTool : IPointFilterTool
        {
            public string Id { get { return "Bilateral"; } }
            public string DisplayName { get { return "双边滤波 Bilateral"; } }
            public IReadOnlyList<PointFilterParameterDescriptor> Parameters { get { return Fields(Field("Bilateral 半径", "BilateralRadius"), Field("空间 Sigma", "BilateralSpatialSigma"), Field("值域 Sigma", "BilateralRangeSigma")); } }
            public bool IsEnabled(PointFilterOptionsParam options) { return options.Bilateral; }
            public List<MeasurePointParam> Apply(IReadOnlyList<MeasurePointParam> input, PointFilterOptionsParam options) { return Bilateral(input, options.BilateralRadius, options.BilateralSpatialSigma, options.BilateralRangeSigma); }
        }

        /// <summary>
        /// 引导滤波工具适配器，把 Guided 开关、半径和 Epsilon 参数接入 Pipeline。
        /// </summary>
        private sealed class GuidedPointFilterTool : IPointFilterTool
        {
            public string Id { get { return "Guided"; } }
            public string DisplayName { get { return "引导滤波 Guided"; } }
            public IReadOnlyList<PointFilterParameterDescriptor> Parameters { get { return Fields(Field("Guided 半径", "GuidedRadius"), Field("Epsilon", "GuidedEpsilon")); } }
            public bool IsEnabled(PointFilterOptionsParam options) { return options.Guided; }
            public List<MeasurePointParam> Apply(IReadOnlyList<MeasurePointParam> input, PointFilterOptionsParam options) { return Guided(input, options.GuidedRadius, options.GuidedEpsilon); }
        }

        /// <summary>
        /// 曲率流滤波工具适配器，把迭代次数和步长参数接入 Pipeline。
        /// </summary>
        private sealed class CurvatureFlowPointFilterTool : IPointFilterTool
        {
            public string Id { get { return "CurvatureFlow"; } }
            public string DisplayName { get { return "曲率流滤波 CurvatureFlow"; } }
            public IReadOnlyList<PointFilterParameterDescriptor> Parameters { get { return Fields(Field("迭代次数", "CurvatureFlowIterations"), Field("Step", "CurvatureFlowStep")); } }
            public bool IsEnabled(PointFilterOptionsParam options) { return options.CurvatureFlow; }
            public List<MeasurePointParam> Apply(IReadOnlyList<MeasurePointParam> input, PointFilterOptionsParam options) { return CurvatureFlow(input, options.CurvatureFlowIterations, options.CurvatureFlowStep); }
        }

        /// <summary>
        /// Sobel 边缘增强工具适配器，把半径和增强量参数接入 Pipeline。
        /// </summary>
        private sealed class SobelPointFilterTool : IPointFilterTool
        {
            public string Id { get { return "Sobel"; } }
            public string DisplayName { get { return "Sobel 边缘增强"; } }
            public IReadOnlyList<PointFilterParameterDescriptor> Parameters { get { return Fields(Field("Sobel 半径", "SobelRadius"), Field("增强量", "SobelAmount")); } }
            public bool IsEnabled(PointFilterOptionsParam options) { return options.Sobel; }
            public List<MeasurePointParam> Apply(IReadOnlyList<MeasurePointParam> input, PointFilterOptionsParam options) { return EdgeEnhance(input, options.SobelRadius, options.SobelAmount, true); }
        }

        /// <summary>
        /// Laplacian 高频增强工具适配器，把半径和增强量参数接入 Pipeline。
        /// </summary>
        private sealed class LaplacianPointFilterTool : IPointFilterTool
        {
            public string Id { get { return "Laplacian"; } }
            public string DisplayName { get { return "拉普拉斯 Laplacian"; } }
            public IReadOnlyList<PointFilterParameterDescriptor> Parameters { get { return Fields(Field("Laplacian 半径", "LaplacianRadius"), Field("增强量", "LaplacianAmount")); } }
            public bool IsEnabled(PointFilterOptionsParam options) { return options.Laplacian; }
            public List<MeasurePointParam> Apply(IReadOnlyList<MeasurePointParam> input, PointFilterOptionsParam options) { return EdgeEnhance(input, options.LaplacianRadius, options.LaplacianAmount, false); }
        }

        /// <summary>
        /// Gabor-like 滤波工具适配器，把半径和 Sigma 参数接入 Pipeline。
        /// </summary>
        private sealed class GaborPointFilterTool : IPointFilterTool
        {
            public string Id { get { return "Gabor"; } }
            public string DisplayName { get { return "Gabor 滤波"; } }
            public IReadOnlyList<PointFilterParameterDescriptor> Parameters { get { return Fields(Field("Gabor 半径", "GaborRadius"), Field("Gabor Sigma", "GaborSigma")); } }
            public bool IsEnabled(PointFilterOptionsParam options) { return options.Gabor; }
            public List<MeasurePointParam> Apply(IReadOnlyList<MeasurePointParam> input, PointFilterOptionsParam options) { return GaborLike(input, options.GaborRadius, options.GaborSigma); }
        }

        /// <summary>
        /// 创建单个滤波参数字段描述。
        /// </summary>
        private static PointFilterParameterDescriptor Field(string label, string bindingPath)
        {
            return new PointFilterParameterDescriptor(label, bindingPath);
        }

        /// <summary>
        /// 创建参数字段只读列表，空数组回退为空列表，方便工具描述统一返回。
        /// </summary>
        private static IReadOnlyList<PointFilterParameterDescriptor> Fields(params PointFilterParameterDescriptor[] fields)
        {
            return fields == null ? new List<PointFilterParameterDescriptor>() : fields.ToList();
        }
    }
}




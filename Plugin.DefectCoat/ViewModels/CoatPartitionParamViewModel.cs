using Plugin.DefectCoat.Models;
using System;
using Frame_Core.pluginParams;

namespace Plugin.DefectCoat.ViewModels
{
    /// <summary>
    /// 分区参数界面转接对象。
    /// CoatPartitionParam 保持纯参数；本类负责 DataGrid 双向绑定、手动固定长度标记和属性刷新。
    /// </summary>
    public sealed class CoatPartitionParamViewModel : BindableBase
    {
        /// <summary>任一分区属性变化后的回调，通常用于刷新分区长度汇总、固定宽度文本等派生显示。</summary>
        private readonly Action changed;

        /// <summary>创建分区参数界面转接对象；model 为空时创建默认分区参数，避免表格绑定空引用。</summary>
        public CoatPartitionParamViewModel( CoatPartitionParam model, Action changed = null)
        {
            Model = model ?? new CoatPartitionParam();
            this.changed = changed;
        }

        /// <summary>真实分区参数对象。所有属性 setter 都直接写入该 Model。</summary>
        public CoatPartitionParam Model { get; }

        // 分区基础信息：用于界面表格显示、是否启用和用户固定长度控制。
        /// <summary>分区序号，从 1 开始，用于列表显示和日志追踪。</summary>
        public int Index { get => Model.Index; set { if (Model.Index == value) return; Model.Index = value; NotifyChanged(); } }
        /// <summary>分区名称，默认“分区1/分区2”等。</summary>
        public string Name { get => Model.Name; set { if (Model.Name == value) return; Model.Name = value; NotifyChanged(); } }
        /// <summary>是否启用该分区；关闭时不检测但仍占用长度位置。</summary>
        public bool Enabled { get => Model.Enabled; set { if (Model.Enabled == value) return; Model.Enabled = value; NotifyChanged(); } }
        /// <summary>用户是否固定该分区长度；固定后自动归一化不会覆盖该长度。</summary>
        public bool IsLengthFixed { get => Model.IsLengthFixed; set { if (Model.IsLengthFixed == value) return; Model.IsLengthFixed = value; NotifyChanged(); } }

        // 用户手动编辑 Length/LengthMm 时自动把 IsLengthFixed 置 true，避免 NormalizePartitions 覆盖用户输入。
        /// <summary>分区长度，单位 px；用户手动修改时自动固定该分区长度。</summary>
        public double Length { get => Model.Length; set { if (Model.Length == value) return; Model.Length = value; Model.IsLengthFixed = true; NotifyChanged(); OnPropertyChanged(nameof(IsLengthFixed)); } }
        /// <summary>分区长度，单位 mm；用户手动修改时自动固定该分区长度。</summary>
        public double LengthMm { get => Model.LengthMm; set { if (Model.LengthMm == value) return; Model.LengthMm = value; Model.IsLengthFixed = true; NotifyChanged(); OnPropertyChanged(nameof(IsLengthFixed)); } }

        // 分区检测参数：每个分区可拥有独立阈值、形态学和筛选参数。
        /// <summary>分区参考灰度，暗/亮阈值都以该值为中心。</summary>
        public int ReferenceGray { get => Model.ReferenceGray; set { if (Model.ReferenceGray == value) return; Model.ReferenceGray = value; NotifyChanged(); } }
        /// <summary>亮缺陷灰度上偏差，亮阈值下限 = ReferenceGray + UpGrayOffset。</summary>
        public int UpGrayOffset { get => Model.UpGrayOffset; set { if (Model.UpGrayOffset == value) return; Model.UpGrayOffset = value; NotifyChanged(); } }
        /// <summary>暗缺陷灰度下偏差，暗阈值上限 = ReferenceGray - DownGrayOffset。</summary>
        public int DownGrayOffset { get => Model.DownGrayOffset; set { if (Model.DownGrayOffset == value) return; Model.DownGrayOffset = value; NotifyChanged(); } }
        /// <summary>开运算半径，单位 px，用于去除小噪声。</summary>
        public int OpenRadius { get => Model.OpenRadius; set { if (Model.OpenRadius == value) return; Model.OpenRadius = value; NotifyChanged(); } }
        /// <summary>闭运算半径，单位 px，用于连接邻近缺陷区域。</summary>
        public int CloseRadius { get => Model.CloseRadius; set { if (Model.CloseRadius == value) return; Model.CloseRadius = value; NotifyChanged(); } }
        /// <summary>最小候选面积，单位 mm²，运行时会按像素单量换算为 px²。</summary>
        public double MinCandidateArea { get => Model.MinCandidateArea; set { if (Model.MinCandidateArea == value) return; Model.MinCandidateArea = value; NotifyChanged(); } }
        /// <summary>是否启用背景灰度归一化。</summary>
        public bool EnableBackgroundGrayScale { get => Model.EnableBackgroundGrayScale; set { if (Model.EnableBackgroundGrayScale == value) return; Model.EnableBackgroundGrayScale = value; NotifyChanged(); } }
        /// <summary>是否启用小区域 ClosingRectangle1 合并。</summary>
        public bool EnableSmallRegionMerge { get => Model.EnableSmallRegionMerge; set { if (Model.EnableSmallRegionMerge == value) return; Model.EnableSmallRegionMerge = value; NotifyChanged(); } }
        /// <summary>小区域合并最大面积阈值，单位 px²。</summary>
        public double SmallRegionMergeMaxArea { get => Model.SmallRegionMergeMaxArea; set { if (Model.SmallRegionMergeMaxArea == value) return; Model.SmallRegionMergeMaxArea = value; NotifyChanged(); } }
        /// <summary>小区域合并矩形宽度，单位 px。</summary>
        public int SmallRegionMergeWidth { get => Model.SmallRegionMergeWidth; set { if (Model.SmallRegionMergeWidth == value) return; Model.SmallRegionMergeWidth = value; NotifyChanged(); } }
        /// <summary>小区域合并矩形高度，单位 px。</summary>
        public int SmallRegionMergeHeight { get => Model.SmallRegionMergeHeight; set { if (Model.SmallRegionMergeHeight == value) return; Model.SmallRegionMergeHeight = value; NotifyChanged(); } }
        /// <summary>灰度归一化最大倍率绝对值，超过后回退为 1。</summary>
        public double MaxGrayScaleAbs { get => Model.MaxGrayScaleAbs; set { if (Model.MaxGrayScaleAbs == value) return; Model.MaxGrayScaleAbs = value; NotifyChanged(); } }

        /// <summary>外部批量修改 Model 后调用，强制刷新本行全部绑定属性。</summary>
        public void RefreshAll()
        {
            OnPropertyChanged(string.Empty);
        }

        /// <summary>统一触发属性变更通知和外层回调，保证 DataGrid 与分区汇总显示同步。</summary>
        private void NotifyChanged()
        {
            OnPropertyChanged();
            changed?.Invoke();
        }
    }
}

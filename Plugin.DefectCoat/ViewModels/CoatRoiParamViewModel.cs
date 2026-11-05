using Frame_Core.pluginParams;
using Plugin.DefectCoat.Models;
using System;
using System.ComponentModel;

namespace Plugin.DefectCoat.ViewModels
{
    /// <summary>
    /// ROI 参数界面转接对象。
    /// Model 是可迁移到 Frame_Core\pluginParams 的纯参数类；本类只在插件界面层负责属性变更通知。
    /// </summary>
    public sealed class CoatRoiParamViewModel : BindableBase
    {
        /// <summary>任一属性变化后的回调，通常用于通知主 ViewModel 刷新 ROI 信息或同步 ViewROI。</summary>
        private readonly Action changed;

        /// <summary>创建 ROI 界面转接对象；model 为空时创建默认 CoatRoiParam，避免 DataGrid 绑定空引用。</summary>
        public CoatRoiParamViewModel(CoatRoiParam model, Action changed = null)
        {
            Model = model ?? new CoatRoiParam();
            this.changed = changed;
        }

        /// <summary>真实 ROI 参数对象。所有属性 setter 都直接写入该 Model，保存和运行时再回写到 ParamsDefectCoat。</summary>
        public CoatRoiParam Model { get; }

        // 基础信息：用于屏蔽 ROI 列表显示、启用状态控制和 XML 保存。
        /// <summary>是否启用该 ROI；屏蔽 ROI 可单独开关，局部 ROI 通常由 EnableLocalRoi 控制。</summary>
        public bool Enabled { get => Model.Enabled; set { if (Model.Enabled == value) return; Model.Enabled = value; NotifyChanged(); } }
        /// <summary>ROI 序号，用于屏蔽 ROI 列表排序和显示。</summary>
        public int Index { get => Model.Index; set { if (Model.Index == value) return; Model.Index = value; NotifyChanged(); } }
        /// <summary>ROI 名称，用于界面列表和调试日志。</summary>
        public string Name { get => Model.Name; set { if (Model.Name == value) return; Model.Name = value; NotifyChanged(); } }
        // Rectangle1 主坐标：当前局部 ROI 和屏蔽 ROI 的真实运行坐标，单位 px。
        /// <summary>Rectangle1 的 Row1，单位 px；当前局部 ROI 和屏蔽 ROI 的主运行坐标。</summary>
        public double Row1 { get => Model.Row1; set { if (Model.Row1 == value) return; Model.Row1 = value; NotifyChanged(); } }
        /// <summary>Rectangle1 的 Col1，单位 px；当前局部 ROI 和屏蔽 ROI 的主运行坐标。</summary>
        public double Col1 { get => Model.Col1; set { if (Model.Col1 == value) return; Model.Col1 = value; NotifyChanged(); } }
        /// <summary>Rectangle1 的 Row2，单位 px；当前局部 ROI 和屏蔽 ROI 的主运行坐标。</summary>
        public double Row2 { get => Model.Row2; set { if (Model.Row2 == value) return; Model.Row2 = value; NotifyChanged(); } }
        /// <summary>Rectangle1 的 Col2，单位 px；当前局部 ROI 和屏蔽 ROI 的主运行坐标。</summary>
        public double Col2 { get => Model.Col2; set { if (Model.Col2 == value) return; Model.Col2 = value; NotifyChanged(); } }


        /// <summary>
        /// 上偏移(Row)
        /// </summary>
        [Description("上偏移(Row)")]
        public double TopOffsetRow { get => Model.TopOffsetRow; set { if (Model.TopOffsetRow == value) return; Model.TopOffsetRow = value; NotifyChanged(); } }
        /// <summary>
        /// 下偏移(Row)
        /// </summary>
        [Description("下偏移(Row)")]
        public double BottomOffsetRow { get => Model.BottomOffsetRow; set { if (Model.BottomOffsetRow == value) return; Model.BottomOffsetRow = value; NotifyChanged(); } }
        /// <summary>
        /// 左偏移(Col)
        /// </summary>
        [Description("左偏移(Col)")]
        public double LeftOffsetCol { get => Model.LeftOffsetCol; set { if (Model.LeftOffsetCol == value) return; Model.LeftOffsetCol = value; NotifyChanged(); } }
        /// <summary>
        /// 右偏移(Col)
        /// </summary>
        [Description("右偏移(Col)")]
        public double RightOffsetCol { get => Model.RightOffsetCol; set { if (Model.RightOffsetCol == value) return; Model.RightOffsetCol = value; NotifyChanged(); } }

        /// <summary>外部直接修改 Model 后调用，强制刷新本行全部绑定属性。</summary>
        public void RefreshAll()
        {
            OnPropertyChanged(string.Empty);
        }

        /// <summary>统一触发属性变更通知和外层回调，保证 DataGrid、参数面板和主图 ROI 有机会同步。</summary>
        private void NotifyChanged()
        {
            OnPropertyChanged();
            changed?.Invoke();
        }
    }
}

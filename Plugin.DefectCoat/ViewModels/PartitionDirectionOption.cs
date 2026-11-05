using Frame_Core.pluginParams;
using FrameCommunity.公共类;
using Plugin.DefectCoat.Models;

namespace Plugin.DefectCoat.ViewModels
{
    /// <summary>
    /// 分区方向下拉框选项。
    /// 只服务界面显示：Value 是真实枚举值，DisplayName 是中文文本，不参与算法计算。
    /// </summary>
    public sealed class PartitionDirectionOption
    {
        /// <summary>实际分区方向枚举。LongSide 表示沿 Row 高度切分，WideSide 表示沿 Col 宽度切分。</summary>
        public PartitionSplitDirection Value { get; set; }

        /// <summary>界面显示名称，例如“长向分段”“宽向分段”。</summary>
        public string DisplayName { get; set; }
    }
}

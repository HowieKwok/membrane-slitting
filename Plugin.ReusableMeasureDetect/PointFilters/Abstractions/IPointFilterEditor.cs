using Frame_Core.pluginParams;
using System.Collections.Generic;

namespace Plugin.ReusableMeasureDetect.PointFilters.Abstractions
{
    /// <summary>
    /// 点集滤波编辑器接口。
    /// 主插件 ViewModel 只依赖该接口完成参数挂载、提交和步骤刷新，便于后续把滤波控件抽到公共库。
    /// </summary>
    public interface IPointFilterEditor
    {
        /// <summary>
        /// 当前滤波参数对象，包含开关、顺序、选中步骤和每种滤波器参数。
        /// </summary>
        PointFilterOptionsParam Options { get; }

        /// <summary>
        /// 当前滤波输出对象，包含 Raw、Steps、SelectedPoints 和显示 XLD。
        /// </summary>
        PointFilterOutputParam Output { get; }

        /// <summary>
        /// 挂载宿主插件的滤波参数和输出对象。
        /// </summary>
        void Attach(PointFilterOptionsParam options, PointFilterOutputParam output);

        /// <summary>
        /// 将界面 Pipeline 顺序和选中步骤提交回 Options。
        /// </summary>
        void CommitToOptions();

        /// <summary>
        /// 根据 Output.Steps 刷新界面可选步骤列表。
        /// </summary>
        void RefreshSteps();

        /// <summary>
        /// 返回当前选中的点集，供宿主检测或显示使用。
        /// </summary>
        IReadOnlyList<MeasurePointParam> GetSelectedPoints();
    }
}


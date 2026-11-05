using DefectRuleClassifier.Core.Models;
using HalconDotNet;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Plugin.DefectCoat.Core.Detection
{
    /// <summary>
    /// 一次 HALCON 阈值检测的完整输出容器。
    /// 该对象由检测层创建，由 pipeline 和插件入口读取；内部持有多个运行态 HObject，
    /// 因此必须在下一次检测前或插件释放时调用 Dispose，避免 HALCON 句柄泄漏。
    /// </summary>
    public sealed class DefectDetectResult
    {
        /// <summary>检测是否成功。false 时 Message 保存失败原因，候选和 HObject 可能为空。</summary>
        public bool Success { get; set; }

        /// <summary>检测阶段状态说明或异常信息，最终会回写到 Params.m_strRunMessage。</summary>
        public string Message { get; set; }

        /// <summary>预处理说明文本，例如背景归一化倍率、阈值信息；仅用于界面显示和调试日志。</summary>
        public string PreprocessInfo { get; set; }

        /// <summary>预处理后的灰度图。由检测结果对象负责释放，不写 XML。</summary>
        public HObject PreprocessImage { get; set; }

        /// <summary>实际参与检测的 ROI Region，已经包含位置跟随后的仿射结果。由本对象负责释放。</summary>
        public HObject RoiRegion { get; set; }

        /// <summary>屏蔽 ROI 合并 Region，运行时从缺陷区域中扣除。由本对象负责释放。</summary>
        public HObject MaskRoiRegion { get; set; }

        /// <summary>候选缺陷合并 Region，供主窗口红色叠加显示和后续输出。由本对象负责释放。</summary>
        public HObject ResultRegion { get; set; }

        /// <summary>暗缺陷阈值区域，用于预处理窗口调试显示。由本对象负责释放。</summary>
        public HObject DarkRegion { get; set; }

        /// <summary>亮缺陷阈值区域，用于预处理窗口调试显示。由本对象负责释放。</summary>
        public HObject LightRegion { get; set; }

        /// <summary>检测阶段面积筛选后的候选数量。该数量来自 SelectShape 之后，AppendCandidates 之前。</summary>
        public int CandidateCount { get; set; }

        /// <summary>是否在检测阶段触发候选数量硬上限。触发后不再裁剪单个 NG 小图，也不生成单个候选对象。</summary>
        public bool IsCandidateCountOverflow { get; set; }

        /// <summary>检测阶段候选数量超限时输出的类别名称，例如“涂布不良”。</summary>
        public string OverflowClassName { get; set; }

        /// <summary>触发超限时的实际候选数量。</summary>
        public int OverflowCandidateCount { get; set; }

        /// <summary>触发超限时配置的候选数量上限。</summary>
        public int OverflowCandidateLimit { get; set; }

        /// <summary>
        /// 超限输出区域。
        /// 这里保存检测阶段已经算好的亮暗合并区域，不逐个 Union 单个候选，避免超限分支继续耗时。
        /// </summary>
        public HObject OverflowRegion { get; set; }

        /// <summary>横向像素单量，单位 mm/pixel，用于超限合并结果计算面积和宽度。</summary>
        public double PixelToMmH { get; set; } = 1;

        /// <summary>纵向像素单量，单位 mm/pixel，用于超限合并结果计算面积和高度。</summary>
        public double PixelToMmV { get; set; } = 1;

        /// <summary>参考灰度，供结果窗口和后续特征调试使用。</summary>
        public int ReferenceGray { get; set; }

        /// <summary>
        /// 未分类前的候选缺陷列表。
        /// 每个 DefectCandidate 都持有自己的 Region/CropImage/亮暗子区域，Dispose 时会逐个释放。
        /// </summary>
        public IList<DefectCandidate> Candidates { get; set; } = new List<DefectCandidate>();

        /// <summary>检测调试日志。使用 ObservableCollection 是为了界面可直接绑定显示。</summary>
        public ObservableCollection<string> DebugLogs { get; } = new ObservableCollection<string>();

        /// <summary>
        /// 释放本次检测产生的全部 HObject 和候选对象。
        /// 注意：这里不释放输入图像 request.Image，也不释放 Params.ho_Image。
        /// </summary>
        public void Dispose()
        {
            RoiRegion?.Dispose();
            RoiRegion = null;
            PreprocessImage?.Dispose();
            PreprocessImage = null;
            MaskRoiRegion?.Dispose();
            MaskRoiRegion = null;
            ResultRegion?.Dispose();
            ResultRegion = null;
            DarkRegion?.Dispose();
            DarkRegion = null;
            LightRegion?.Dispose();
            LightRegion = null;
            OverflowRegion?.Dispose();
            OverflowRegion = null;
            foreach (DefectCandidate candidate in Candidates)
                candidate?.Dispose();
            Candidates.Clear();
        }
    }
}


namespace Plugin.DefectCoat.Core.Detection
{
    /// <summary>
    /// 缺陷检测器抽象接口。
    /// Pipeline 只依赖该接口调用检测，具体实现可以是 HALCON 阈值、模型检测或后续其它检测方式。
    /// </summary>
    public interface IDefectDetector
    {
        /// <summary>
        /// 执行一次检测并返回检测结果。
        /// request 只作为输入参数使用；返回结果中的 HObject 由调用方在不再需要时统一释放。
        /// </summary>
        DefectDetectResult Detect(DefectDetectRequest request);
    }
}

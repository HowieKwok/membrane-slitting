namespace Plugin.DefectCoat.Services
{
    /// <summary>
    /// HALCON 窗口鼠标移动事件的轻量参数对象。
    /// 只保存图像坐标和格式化显示文本，不持有 HObject，不涉及资源释放。
    /// </summary>
    public sealed class HalconMouseInfoArgs
    {
        /// <summary>鼠标所在图像行坐标，单位 px，对应 HALCON 的 row。</summary>
        public double Row { get; set; }

        /// <summary>鼠标所在图像列坐标，单位 px，对应 HALCON 的 column。</summary>
        public double Column { get; set; }

        /// <summary>状态栏显示文本，通常包含 Row、Col、灰度值和当前统计信息。</summary>
        public string Text { get; set; }
    }
}

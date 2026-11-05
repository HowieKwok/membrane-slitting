namespace DefectRuleClassifier.Core.Models
{
    /// <summary>
    /// 传统分类器的输入图像（与 HALCON 解耦）：
    /// 插件侧把候选小图转成 RGB 交织像素字节后传入，分类器本身不依赖 HALCON。
    /// </summary>
    public sealed class ModelImageInput
    {
        /// <summary>RGB 交织像素（每像素 3 字节，顺序 R,G,B）。</summary>
        public byte[] Rgb { get; set; }

        /// <summary>图像宽度（像素）。</summary>
        public int Width { get; set; }

        /// <summary>图像高度（像素）。</summary>
        public int Height { get; set; }

        /// <summary>来源说明（可选，用于 HitRule 追溯）。</summary>
        public string Source { get; set; }

        public static ModelImageInput FromRgb(byte[] rgb, int width, int height, string source = null)
        {
            return new ModelImageInput { Rgb = rgb, Width = width, Height = height, Source = source };
        }
    }
}

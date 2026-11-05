using DefectRuleClassifier.ViewModels;

namespace DefectRuleClassifier.Core.Models
{
    /// <summary>
    /// 传统分类器“模型标签序号 → 特征值分类标签”映射项。
    /// 序号即模型输出类别索引（labels.txt 顺序，0 起）；
    /// 映射到特征值标签后，再按特征值标签决定最终归入类别；未配置或特征值标签不存在时统一归入超限类别。
    /// </summary>
    public sealed class ModelLabelMapItem : BindableBase
    {
        private int index;
        private string featureLabel;

        public int Index
        {
            get => index;
            set => SetProperty(ref index, value);
        }

        /// <summary>映射到的特征值分类标签名（与特征值分类 NgTypes 中的类别名一致）。</summary>
        public string FeatureLabel
        {
            get => featureLabel;
            set => SetProperty(ref featureLabel, value);
        }
    }
}

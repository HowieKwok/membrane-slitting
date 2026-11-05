namespace DefectRuleClassifier.Core.Classifiers
{
    public sealed class DefectClassifyDecision
    {
        public string ClassName { get; set; }
        /// <summary>模型标签序号（0 起）。用于界面显示与 switch(序号) 映射到特征值标签。</summary>
        public int ClassIndex { get; set; } = -1;
        /// <summary>模型置信度，范围 0..1；非模型分类时为 0。</summary>
        public double Confidence { get; set; }
        /// <summary>全部类别得分（softmax 后，顺序与 labels.txt 一致）；非模型分类时为 null。</summary>
        public float[] Scores { get; set; }
        public int Priority { get; set; }
        public string Source { get; set; }
        public string HitRule { get; set; }
        public bool SkipClassifier { get; set; }
    }
}

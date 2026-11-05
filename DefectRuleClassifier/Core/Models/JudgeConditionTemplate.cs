using DefectRuleClassifier.ViewModels;

namespace DefectRuleClassifier.Core.Models
{
    /// <summary>
    /// 兼容旧 XML 的判定条件模板。
    /// 运行时实际使用的是 JudgeConditionMode 枚举，这里只保留显示和迁移用途。
    /// </summary>
    public sealed class JudgeConditionTemplate : BindableBase
    {
        private string id = "size_and";
        private string name = "所有条件且";
        private string expression = "长度 && 宽度 && 面积";
        private string remark = "长度、宽度、面积都在范围内才通过";

        public string Id { get => id; set => SetProperty(ref id, value); }
        public string Name { get => name; set => SetProperty(ref name, value); }
        public string Expression { get => expression; set => SetProperty(ref expression, value); }
        public string Remark { get => remark; set => SetProperty(ref remark, value); }
    }
}

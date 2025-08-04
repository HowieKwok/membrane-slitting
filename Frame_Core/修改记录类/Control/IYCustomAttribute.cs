using System;

namespace YYSD.UICore.Control
{
    /// <summary>
    /// 自定义特性类接口
    /// </summary>
    public interface IYCustomAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        object Value { get; }
    }

    /// <summary>
    /// 自定义特性类【控件ID索引特性】
    /// </summary>
    public class ControlIdAttribute : Attribute, IYCustomAttribute
    {
        public ControlIdAttribute() { valueObj = 0; }
        public ControlIdAttribute(object obj) { valueObj = obj; }
        public virtual object Value { get { return valueObj; } }
        protected object valueObj { get; set; }
    }
    /// <summary>
    /// 自定义特性类【显示文本特性】
    /// </summary>
    public class ShowTextAttribute : Attribute, IYCustomAttribute
    {
        public ShowTextAttribute() { valueObj = "yyds"; }
        public ShowTextAttribute(object obj) { valueObj = obj; }
        public virtual object Value { get { return valueObj; } }
        protected object valueObj { get; set; }
    }

}

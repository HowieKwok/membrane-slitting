using System;

namespace YYSD.UICore.Control
{
    /// <summary>
    /// 自定义控件属性类接口
    /// </summary>
    public interface IYCustomControl
    {
        /// <summary>
        /// 控件值
        /// </summary>
        object value { set; get; }
        /// <summary>
        /// 控件名称
        /// </summary>
        string name { set; get; }
    }
}

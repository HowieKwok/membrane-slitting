using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    public class ExecuteResult
    {
        //
        // 摘要:
        //     具体的错误代码。 The specific error code.
        public int ErrorCode { get; set; } = 0;

        //
        // 摘要:
        //     具体的警告代码。 The specific warning code.
        public int WarningCode { get; set; } = 0;
        //
        // 摘要:
        //     指示本次操作是否成功。 Indicates whether this operation was successful.
        public bool IsSuccess { get; set; } = false;
        //
        // 摘要:
        //     具体的错误描述。 Specific error description.
        public string Message { get; set; } = "";


        /// <summary>
        /// 耗时
        /// </summary>
        public double Time { get; set; } = 0;
    }

    public class ExecuteResult<T>
    {
        //
        // 摘要:
        //     具体的错误代码。 The specific error code.
        public int ErrorCode { get; set; } = 0;

        //
        // 摘要:
        //     具体的警告代码。 The specific warning code.
        public int WarningCode { get; set; } = 0;
        //
        // 摘要:
        //     指示本次操作是否成功。 Indicates whether this operation was successful.
        public bool IsSuccess { get; set; } = false;
        //
        // 摘要:
        //     具体的错误描述。 Specific error description.
        public string Message { get; set; } = "";

        //
        // 摘要:
        //     具体的值。
        public T Value  { get; set; }

        /// <summary>
        /// 耗时
        /// </summary>
        public double Time { get; set; } = 0;
    }
}

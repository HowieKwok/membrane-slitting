using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    public class RowData
    {
        /// <summary>
        /// 检测数据
        /// </summary>
        public string dataStr = "";

        /// <summary>
        /// 检测结果类型
        /// </summary>
        public ResultType resultType = ResultType.OK;
    }

    public enum ResultType
    {
   
        /// <summary>
        /// 合格
        /// </summary>
        OK,
        /// <summary>
        /// 警告
        /// </summary>
        Warn,
        /// <summary>
        /// 不合格
        /// </summary>
        NG,
    }

 
}
